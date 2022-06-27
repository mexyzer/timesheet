using System.Net;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Core.Entities;
using Timesheet.Core.Events;
using Timesheet.Core.Interfaces;
using Timesheet.SharedKernel;
using Timesheet.WebApi.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Timesheet.WebApi.EndPoints.Users;

public class Login : EndpointBaseAsync.WithRequest<LoginRequest>.WithActionResult<LoginResponse>
{
	private readonly IUserService _userService;
	private readonly JwtService _jwtService;
	private readonly IDateTime _dateTime;
	private readonly IApplicationDbContext _dbContext;

	public Login(IUserService userService, JwtService jwtService, IDateTime dateTime, IApplicationDbContext dbContext)
	{
		_userService = userService;
		_jwtService = jwtService;
		_dateTime = dateTime;
		_dbContext = dbContext;
	}

	[HttpPost(LoginRequest.Route)]
	[SwaggerResponse((int)HttpStatusCode.BadRequest, "", typeof(ErrorResponse))]
	[SwaggerResponse((int)HttpStatusCode.InternalServerError, "", typeof(ErrorResponse))]
	[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(LoginResponse))]
	[SwaggerOperation(
		Summary = "API Log In",
		OperationId = "Login.Login",
		Tags = new[] {"Login"})
	]
	public override async Task<ActionResult<LoginResponse>> HandleAsync([FromBody] LoginRequest request,
		CancellationToken cancellationToken = new())
	{
		var validator = new LoginRequestValidator();
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			return BadRequest(ValidationExceptionBuilder.Build(validationResult.Errors));
		}

		var user = await _userService.GetByUsernameAsync(request.Username!, cancellationToken);
		if (user == null)
			return BadRequest(ErrorResponseExtension.Create("Invalid username or password"));

		var pass = await _userService.CheckPasswordAsync(user.UserId, request.Password!, cancellationToken);
		if (!pass)
			return BadRequest(ErrorResponseExtension.Create("Invalid username or password"));

		var userToken = new UserToken
		{
			RefreshToken = Guid.NewGuid().ToString("N").ToSHA512(),
			ExpiredUtcDt = _dateTime.ScopedUtcNow.AddMonths(1)
		};

		user.Events.Add(new LoginSuccessEvent(user));
		user.UserTokens.Add(userToken);

		// Add conditional on Request for bypass from unit test, Request is null if run by unit tester
		// ReSharper disable once ConstantConditionalAccessQualifier
		// ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
		user.UserLogins.Add(new UserLogin {UserAgent = Request?.Headers["User-Agent"].ToString()});

		await _dbContext.SaveChangesAsync(cancellationToken);

		return Ok(LoginResponseExtension.Build(_dateTime, _jwtService, user, userToken.UserTokenId.ToString(),
			userToken.RefreshToken));
	}
}
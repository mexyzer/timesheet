using System.Net;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Timesheet.Core.Entities;
using Timesheet.Core.Interfaces;
using Timesheet.SharedKernel;
using Timesheet.WebApi.Common;
using Timesheet.WebApi.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Timesheet.WebApi.EndPoints.Users;

public class RefreshToken : EndpointBaseAsync.WithRequest<RefreshTokenRequest>.WithActionResult<LoginResponse>
{
	private readonly ICurrentUserService _currentUserService;
	private readonly JwtService _jwtService;
	private readonly IDateTime _dateTime;
	private readonly IApplicationDbContext _dbContext;

	public RefreshToken(ICurrentUserService currentUserService, JwtService jwtService, IDateTime dateTime,
		IApplicationDbContext dbContext)
	{
		_currentUserService = currentUserService;
		_jwtService = jwtService;
		_dateTime = dateTime;
		_dbContext = dbContext;
	}

	[Authorize]
	[HttpGet(RefreshTokenRequest.Route)]
	[SwaggerResponse((int)HttpStatusCode.BadRequest, "", typeof(ErrorResponse))]
	[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(LoginResponse))]
	[SwaggerOperation(
		Summary = "API Refresh Token",
		OperationId = "Login.RefreshToken",
		Tags = new[] {"Login"})
	]
	public override async Task<ActionResult<LoginResponse>> HandleAsync([FromQuery] RefreshTokenRequest request,
		CancellationToken cancellationToken = new CancellationToken())
	{
		var id = new Guid(_currentUserService.Idt!);
		var userId = new Guid(_currentUserService.UserId!);

		var userToken = await _dbContext.UserTokens.Include(e => e.User)
			.Where(e => e.UserTokenId == id && e.UserId == userId)
			.FirstOrDefaultAsync(cancellationToken);

		if (userToken == null)
			return NotFound(ErrorResponseExtension.Create("Data not found"));

		if (userToken.IsUsed)
			return BadRequest(ErrorResponseExtension.Create("Token already used"));

		if (userToken.RefreshToken != request.Token)
			return BadRequest(ErrorResponseExtension.Create("Invalid token"));

		userToken.SetToInActive();
		userToken.IsUsed = true;

		if (userToken.ExpiredUtcDt < _dateTime.ScopedUtcNow)
		{
			await _dbContext.SaveChangesAsync(cancellationToken);
			return BadRequest(ErrorResponseExtension.Create("Token expired"));
		}

		var newUserToken = new UserToken
		{
			RefreshToken = Guid.NewGuid().ToString("N").ToSHA512(),
			ExpiredUtcDt = _dateTime.ScopedUtcNow.AddMonths(1)
		};
		userToken.User.UserTokens.Add(newUserToken);
		await _dbContext.SaveChangesAsync(cancellationToken);

		return Ok(LoginResponseExtension.Build(_dateTime, _jwtService, userToken.User, userToken.UserTokenId.ToString(),
			userToken.RefreshToken));
	}
}
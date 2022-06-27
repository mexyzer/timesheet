using System.Net;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Core.Entities;
using Timesheet.Core.Events;
using Timesheet.Core.Interfaces;
using Timesheet.WebApi.Common;
using Timesheet.WebApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Timesheet.WebApi.EndPoints.UserManagement;

public class CreateUser : EndpointBaseAsync.WithRequest<CreateUserRequest>.WithActionResult
{
	private readonly IUserService _userService;
	private readonly IApplicationDbContext _dbContext;

	public CreateUser(IUserService userService,
		IApplicationDbContext dbContext)
	{
		_userService = userService;
		_dbContext = dbContext;
	}

	[Authorize]
	[HttpPost(CreateUserRequest.Route)]
	[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(KeyDto))]
	[SwaggerOperation(
		Summary = "API Create Users",
		OperationId = "UserManagement.Post",
		Tags = new[] {"UserManagement"})
	]
	public override async Task<ActionResult> HandleAsync(CreateUserRequest request,
		CancellationToken cancellationToken = new CancellationToken())
	{
		var validator = new CreateUserRequestValidator(_userService);
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
			return BadRequest(ValidationExceptionBuilder.Build(validationResult.Errors));

		var user = await _userService.GetByUsernameAsync(request.Username!, cancellationToken);
		if (user != null)
			return BadRequest(ErrorResponseExtension.Create("Username already exists"));

		var newUser = new User();

		newUser.UpdateName(request.FirstName!, request.MiddleName, request.LastName);
		newUser.SetUsername(request.Username!);
		newUser.SetPassword(request.Password!);

		foreach (var item in request.RoleIds!)
			newUser.UserRoles.Add(new UserRole {RoleId = new Guid(item)});

		newUser.Events.Add(new UserRegisteredEvent(newUser));

		_dbContext.Users.Add(newUser);
		await _dbContext.SaveChangesAsync(cancellationToken);

		return Ok(KeyDtoHelper.Create(newUser.UserId.ToString(), KeyDto.Guid));
	}
}
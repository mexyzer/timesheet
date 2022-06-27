using System.Net;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Core.Entities;
using Timesheet.Core.Interfaces;
using Timesheet.WebApi.Common;
using Timesheet.WebApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Timesheet.WebApi.EndPoints.UserManagement;

public class EditUser : EndpointBaseAsync.WithRequest<EditUserRequest>.WithActionResult
{
	private readonly IUserService _userService;
	private readonly IApplicationDbContext _dbContext;
	private readonly ICurrentUserService _currentUserService;

	public EditUser(IUserService userService,
		IApplicationDbContext dbContext, ICurrentUserService currentUserService)
	{
		_userService = userService;
		_dbContext = dbContext;
		_currentUserService = currentUserService;
	}

	[Authorize]
	[HttpPut(EditUserRequest.Route)]
	[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(KeyDto))]
	[SwaggerOperation(
		Summary = "API Edit Users",
		OperationId = "UserManagement.Post",
		Tags = new[] {"UserManagement"})
	]
	public override async Task<ActionResult> HandleAsync([FromRoute] EditUserRequest request,
		CancellationToken cancellationToken = new CancellationToken())
	{
		var validator = new EditUserRequestValidator(_userService);
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
			return BadRequest(ValidationExceptionBuilder.Build(validationResult.Errors));

		var user = await _userService.GetUserByIdAsync(request.UserId, cancellationToken);
		if (user == null)
			return NotFound(ErrorResponseExtension.Create("Data not found"));

		user.UpdateName(request.FirstName, request.MiddleName, request.LastName);

		if (!string.IsNullOrWhiteSpace(request.NewPassword))
			user.SetPassword(request.NewPassword);

		//update current to obsolete if any
		foreach (var item in user.UserRoles)
		{
			if (request.RoleIds!.Any(e => new Guid(e) == item.RoleId))
				continue;

			item.IsActive = false;
			item.DeletedDt = DateTime.UtcNow;
			item.DeletedBy = _currentUserService.UserId!;
		}

		//add new roles if any
		foreach (var item in request.RoleIds!)
		{
			var roleId = new Guid(item);

			if (!user.UserRoles.Any(e => e.RoleId == roleId))
				user.UserRoles.Add(new UserRole {RoleId = roleId});
		}

		await _dbContext.SaveChangesAsync(cancellationToken);

		return Ok(KeyDtoHelper.Create(user.UserId.ToString(), KeyDto.Guid));
	}
}
using System.Net;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Core.Entities;
using Timesheet.Core.Interfaces;
using Timesheet.WebApi.Common;
using Timesheet.WebApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Timesheet.WebApi.EndPoints.RoleManagement;

public class CreateRole : EndpointBaseAsync.WithRequest<CreateRoleRequest>.WithActionResult
{
	private readonly IUserService _userService;
	private readonly IApplicationDbContext _dbContext;

	public CreateRole(IUserService userService,
		IApplicationDbContext dbContext)
	{
		_userService = userService;
		_dbContext = dbContext;
	}

	[Authorize]
	[HttpPost(CreateRoleRequest.Route)]
	[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(KeyDto))]
	[SwaggerOperation(
		Summary = "API Create Role",
		OperationId = "RoleManagement.Post",
		Tags = new[] {"RoleManagement"})
	]
	public override async Task<ActionResult> HandleAsync([FromBody] CreateRoleRequest request,
		CancellationToken cancellationToken = new CancellationToken())
	{
		var validator = new CreateRoleRequestValidator(_userService);
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
			return BadRequest(ValidationExceptionBuilder.Build(validationResult.Errors));

		var isExists = await _userService.IsRoleExistsByNameAsync(request.Name!, cancellationToken);
		if (isExists)
			return BadRequest(ErrorResponseExtension.Create("Role name already exists"));

		var role = new Role {Name = request.Name!};
		role.NormalizedName = role.Name.ToUpperInvariant();
		foreach (var item in request.PermissionIds!)
			role.RolePermissions.Add(new RolePermission {PermissionId = new Guid(item)});

		_dbContext.Roles.Add(role);
		await _dbContext.SaveChangesAsync(cancellationToken);

		return Ok(KeyDtoHelper.Create(role.RoleId.ToString(), KeyDto.Guid));
	}
}
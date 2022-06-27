using System.Net;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Core.Interfaces;
using Timesheet.WebApi.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace Timesheet.WebApi.EndPoints.RoleManagement;

public class Delete : EndpointBaseAsync.WithRequest<DeleteRequest>.WithActionResult
{
	private readonly IUserService _userService;
	private readonly IApplicationDbContext _dbContext;
	private readonly ICurrentUserService _currentUserService;

	public Delete(IUserService userService,
		IApplicationDbContext dbContext,
		ICurrentUserService currentUserService)
	{
		_userService = userService;
		_dbContext = dbContext;
		_currentUserService = currentUserService;
	}

	[Authorize]
	[HttpDelete(DeleteRequest.Route)]
	[SwaggerResponse((int)HttpStatusCode.NoContent)]
	[SwaggerOperation(
		Summary = "API Delete Roles",
		OperationId = "RoleManagement.Delete",
		Tags = new[] {"RoleManagement"})
	]
	public override async Task<ActionResult> HandleAsync(DeleteRequest request,
		CancellationToken cancellationToken = new CancellationToken())
	{
		var role = await _userService.GetRoleByIdAsync(request.RoleId, cancellationToken);
		if (role == null || !role.IsActive)
			return NotFound(ErrorResponseExtension.Create("Data not found"));

		if (role.DeletedDt.HasValue)
			return BadRequest(ErrorResponseExtension.Create("Data already deleted"));

		role.SetToSoftDelete(_currentUserService.UserId!, DateTime.UtcNow);

		await _dbContext.SaveChangesAsync(cancellationToken);

		return NoContent();
	}
}
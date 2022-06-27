using System.Net;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Core.Interfaces;
using Timesheet.WebApi.Common;
using Timesheet.WebApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Timesheet.WebApi.EndPoints.RoleManagement;

public class GetRoleById : EndpointBaseAsync.WithRequest<GetRoleByIdRequest>.WithActionResult<RoleResponse>
{
	private readonly IUserService _userService;

	public GetRoleById(IUserService userService)
	{
		_userService = userService;
	}

	[Authorize]
	[HttpGet(GetRoleByIdRequest.Route)]
	[SwaggerResponse((int)HttpStatusCode.NotFound, "", typeof(ErrorResponse))]
	[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(RoleResponse))]
	[SwaggerOperation(
		Summary = "API Get Roles by Id",
		OperationId = "RoleManagement.GetById",
		Tags = new[] {"RoleManagement"})
	]
	public override async Task<ActionResult<RoleResponse>> HandleAsync([FromQuery] GetRoleByIdRequest request,
		CancellationToken cancellationToken = new CancellationToken())
	{
		var role = await _userService.GetRoleByIdAsync(request.RoleId, cancellationToken);
		if (role == null)
			return new NotFoundObjectResult(new ErrorResponse {Message = "Data not found"});

		return new RoleResponse(role.RoleId, role.Name, role.NormalizedName);
	}
}
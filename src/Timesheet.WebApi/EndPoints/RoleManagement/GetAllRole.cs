using System.Net;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Core.Entities;
using Timesheet.WebApi.Common;
using Timesheet.WebApi.Models;
using SqlKata.Execution;
using Swashbuckle.AspNetCore.Annotations;

namespace Timesheet.WebApi.EndPoints.RoleManagement;

public class GetAllRole : EndpointBaseAsync.WithRequest<GetRoleRequest>.WithActionResult<PaginatedList<RoleResponse>>
{
	private readonly QueryFactory _queryFactory;

	public GetAllRole(QueryFactory queryFactory)
	{
		_queryFactory = queryFactory;
	}

	[Authorize]
	[HttpGet(GetRoleRequest.Route)]
	[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(PaginatedList<RoleResponse>))]
	[SwaggerOperation(
		Summary = "API Get All Roles",
		OperationId = "RoleManagement.Get",
		Tags = new[] {"RoleManagement"})
	]
	public override async Task<ActionResult<PaginatedList<RoleResponse>>> HandleAsync(
		[FromQuery] GetRoleRequest request,
		CancellationToken cancellationToken = new CancellationToken())
	{
		var query = _queryFactory.Query("MstRole").Where("IsActive", true).Where("DeletedDt", null);

		var result = await query.PaginateAsync<Role>(request.Page, request.PageSize,
			cancellationToken: cancellationToken);

		return new PaginatedList<RoleResponse>(
			result.List.Select(e => new RoleResponse(e.RoleId, e.Name, e.NormalizedName))
				.ToList(), result.Count,
			request.Page, request.PageSize);
	}
}
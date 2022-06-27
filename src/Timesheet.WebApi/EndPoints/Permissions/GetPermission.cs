using System.Net;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Core.Entities;
using Timesheet.WebApi.Common;
using SqlKata.Execution;
using Swashbuckle.AspNetCore.Annotations;

namespace Timesheet.WebApi.EndPoints.Permissions;

public class GetPermission : EndpointBaseAsync.WithoutRequest.WithActionResult<List<PermissionRecord>>
{
	private readonly QueryFactory _queryFactory;

	public GetPermission(QueryFactory queryFactory)
	{
		_queryFactory = queryFactory;
	}

	[Authorize]
	[HttpGet("/api/permissions")]
	[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(List<PermissionRecord>))]
	[SwaggerOperation(
		Summary = "API Get All Permission",
		OperationId = "Permissions.Get",
		Tags = new[] {"Permissions"})
	]
	public override async Task<ActionResult<List<PermissionRecord>>> HandleAsync(
		CancellationToken cancellationToken = new CancellationToken())
	{
		return (await _queryFactory.Query("MstPermission").GetAsync<Permission>(cancellationToken: cancellationToken))
			.Select(e => new PermissionRecord(e.PermissionId, e.Name))
			.ToList();
	}
}
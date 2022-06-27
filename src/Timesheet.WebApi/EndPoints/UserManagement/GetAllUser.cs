using System.Net;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Core.Entities;
using Timesheet.WebApi.Common;
using Timesheet.WebApi.Models;
using SqlKata.Execution;
using Swashbuckle.AspNetCore.Annotations;

namespace Timesheet.WebApi.EndPoints.UserManagement;

public class GetAllUser : EndpointBaseAsync.WithRequest<GetUserRequest>.WithActionResult<PaginatedList<UserResponse>>
{
	private readonly QueryFactory _queryFactory;

	public GetAllUser(QueryFactory queryFactory)
	{
		_queryFactory = queryFactory;
	}

	[Authorize]
	[HttpGet(GetUserRequest.Route)]
	[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(PaginatedList<UserResponse>))]
	[SwaggerOperation(
		Summary = "API Get All Users",
		OperationId = "UserManagement.Get",
		Tags = new[] {"UserManagement"})
	]
	public override async Task<ActionResult<PaginatedList<UserResponse>>> HandleAsync(
		[FromQuery] GetUserRequest request,
		CancellationToken cancellationToken = new CancellationToken())
	{
		var query = _queryFactory.Query("MstUser").Where("IsActive", true).Where("DeletedDt", null);

		if (!string.IsNullOrWhiteSpace(request.Name))
			query.WhereLike("FullName", request.Name!);

		query.Select("UserId", "FirstName", "MiddleName", "LastName", "FullName");

		var result = await query.PaginateAsync<User>(request.Page, request.PageSize,
			cancellationToken: cancellationToken);

		return new PaginatedList<UserResponse>(
			result.List.Select(e => new UserResponse(e.UserId, e.FirstName, e.MiddleName, e.LastName, e.FullName))
				.ToList(), result.Count,
			request.Page, request.PageSize);
	}
}
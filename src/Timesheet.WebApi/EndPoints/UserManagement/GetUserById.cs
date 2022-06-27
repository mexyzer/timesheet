using System.Net;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Core.Interfaces;
using Timesheet.WebApi.Common;
using Timesheet.WebApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Timesheet.WebApi.EndPoints.UserManagement;

public class GetUserById : EndpointBaseAsync.WithRequest<GetUserByIdRequest>.WithActionResult<UserResponse>
{
	private readonly IUserService _userService;

	public GetUserById(IUserService userService)
	{
		_userService = userService;
	}

	[Authorize]
	[HttpGet(GetUserByIdRequest.Route)]
	[SwaggerResponse((int)HttpStatusCode.NotFound, "", typeof(ErrorResponse))]
	[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(UserResponse))]
	[SwaggerOperation(
		Summary = "API Get Users by Id",
		OperationId = "UserManagement.GetById",
		Tags = new[] {"UserManagement"})
	]
	public override async Task<ActionResult<UserResponse>> HandleAsync([FromRoute] GetUserByIdRequest request,
		CancellationToken cancellationToken = new CancellationToken())
	{
		var user = await _userService.GetUserByIdAsync(request.UserId, cancellationToken);
		if (user == null)
			return new NotFoundObjectResult(new ErrorResponse {Message = "Data not found"});

		return new UserResponse(user.UserId, user.FirstName, user.MiddleName, user.LastName, user.FullName);
	}
}
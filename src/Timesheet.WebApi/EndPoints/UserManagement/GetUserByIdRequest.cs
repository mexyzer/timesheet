using Microsoft.AspNetCore.Mvc;

namespace Timesheet.WebApi.EndPoints.UserManagement;

public class GetUserByIdRequest
{
	public const string Route = "api/user-management/users/{UserId}";
	public static string BuildRoute(Guid userId) => Route.Replace("{UserId}", userId.ToString());
	public static string BuildRoute(string userId) => Route.Replace("{UserId}", userId);

	[FromRoute(Name = "UserId")] public string UserId { get; set; } = default!;
}
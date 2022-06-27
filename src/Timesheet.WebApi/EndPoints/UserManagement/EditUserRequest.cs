using Microsoft.AspNetCore.Mvc;

namespace Timesheet.WebApi.EndPoints.UserManagement;

public class EditUserRequest
{
	public const string Route = "api/user-management/users/{UserId}";
	public static string BuildRoute(Guid userId) => Route.Replace("{UserId}", userId.ToString());
	public static string BuildRoute(string userId) => Route.Replace("{UserId}", userId);

	[FromRoute(Name = "UserId")] public string UserId { get; set; } = default!;
	public string? FirstName { get; set; }
	public string? MiddleName { get; set; }
	public string? LastName { get; set; }
	public string? NewPassword { get; set; }
	public string[]? RoleIds { get; set; }
}
namespace Timesheet.WebApi.EndPoints.UserManagement;

public class CreateUserRequest
{
	public const string Route = "api/user-management/users";

	public string? Username { get; init; }
	public string? Password { get; init; }
	public string? ConfirmPassword { get; init; }
	public string? FirstName { get; init; }
	public string? MiddleName { get; init; }
	public string? LastName { get; init; }
	public string[]? RoleIds { get; init; }
}
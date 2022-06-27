namespace Timesheet.WebApi.EndPoints.RoleManagement;

public class CreateRoleRequest
{
	public const string Route = "api/role-management/roles";

	public string? Name { get; set; }
	public string[]? PermissionIds { get; set; }
}
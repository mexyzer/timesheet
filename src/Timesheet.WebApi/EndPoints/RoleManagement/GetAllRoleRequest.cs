using Timesheet.WebApi.Models;

namespace Timesheet.WebApi.EndPoints.RoleManagement;

public class GetRoleRequest : BaseFilterDto
{
	public const string Route = "api/role-management/roles";
}
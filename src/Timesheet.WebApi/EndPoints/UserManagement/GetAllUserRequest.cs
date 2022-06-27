using Timesheet.WebApi.Models;

namespace Timesheet.WebApi.EndPoints.UserManagement;

public class GetUserRequest : BaseFilterDto
{
    public const string Route = "api/user-management/users";

    public string? Name { get; set; }
}
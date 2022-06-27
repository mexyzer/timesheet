using Microsoft.AspNetCore.Mvc;

namespace Timesheet.WebApi.EndPoints.RoleManagement;

public class DeleteRequest
{
    public const string Route = "api/role-management/roles/{RoleId}";
    public static string BuildRoute(Guid roleId) => Route.Replace("{RoleId}", roleId.ToString());

    [FromRoute(Name = "RoleId")] public string RoleId { get; set; } = default!;
}
using Timesheet.SharedKernel;

namespace Timesheet.Core.Entities;

public class RolePermission : BaseEntity
{
    public RolePermission()
    {
        RolePermissionId = Guid.NewGuid();
    }

    public Guid RolePermissionId { get; set; }
    public Guid RoleId { get; set; }
    public Role? Role { get; set; }
    public Guid PermissionId { get; set; }
    public Permission? Permission { get; set; }
}
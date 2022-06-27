using Timesheet.SharedKernel;

namespace Timesheet.Core.Entities;

public class UserRole : BaseEntity
{
    public UserRole()
    {
        UserRoleId = Guid.NewGuid();
    }

    public Guid UserRoleId { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid RoleId { get; set; }
    public Role? Role { get; set; }
}
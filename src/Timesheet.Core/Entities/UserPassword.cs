using Timesheet.SharedKernel;

namespace Timesheet.Core.Entities;

public class UserPassword : BaseEntity
{
    public UserPassword()
    {
        UserPasswordId = Guid.NewGuid();
    }

    public Guid UserPasswordId { get; set; }
    public Guid UserId { get; set; }
    public string Salt { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
}
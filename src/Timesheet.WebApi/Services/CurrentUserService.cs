using Timesheet.Core.Interfaces;

namespace Timesheet.WebApi.Services;

public class CurrentUserService : ICurrentUserService
{
    public string? UserId { get; private set; }
    public string? Idt { get; private set; }

    public void SetUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(UserId))
            UserId = userId;
    }

    public void SetIdt(string idt)
    {
        if (string.IsNullOrWhiteSpace(Idt))
            Idt = idt;
    }
}
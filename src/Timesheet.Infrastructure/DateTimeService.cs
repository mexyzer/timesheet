using Timesheet.Core.Interfaces;

namespace Timesheet.Infrastructure;

public class DateTimeService : IDateTime
{
    public DateTimeService()
    {
        ScopedNow = DateTime.Now;
        ScopedUtcNow = DateTime.UtcNow;
    }

    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime ScopedNow { get; }
    public DateTime ScopedUtcNow { get; }
}
namespace Timesheet.Core.Interfaces;

public interface IDateTime
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    DateTime ScopedNow { get; }
    DateTime ScopedUtcNow { get; }
}
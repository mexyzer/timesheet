using Timesheet.Core.Interfaces;
using Timesheet.Infrastructure;

namespace Timesheet.UnitTests;

public class InterfaceDateTimeServiceBuilder
{
    private readonly IDateTime _dateTime;

    public InterfaceDateTimeServiceBuilder()
    {
        _dateTime = new DateTimeService();
    }

    public IDateTime Build()
    {
        return _dateTime;
    }
}
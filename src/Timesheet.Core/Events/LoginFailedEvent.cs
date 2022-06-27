using Timesheet.Core.Entities;
using Timesheet.SharedKernel;

namespace Timesheet.Core.Events;

public class LoginFailedEvent : BaseDomainEvent
{
	public User User { get; }

	public LoginFailedEvent(User user)
	{
		User = user;
	}
}
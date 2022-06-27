using Timesheet.Core.Entities;
using Timesheet.SharedKernel;

namespace Timesheet.Core.Events;

public class UserRegisteredEvent : BaseDomainEvent
{
	public User User { get; }

	public UserRegisteredEvent(User user)
	{
		User = user;
	}
}
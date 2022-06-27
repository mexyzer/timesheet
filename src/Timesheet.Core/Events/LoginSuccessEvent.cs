using Timesheet.Core.Entities;
using Timesheet.SharedKernel;

namespace Timesheet.Core.Events;

public class LoginSuccessEvent : BaseDomainEvent
{
	public User User { get; }

	public LoginSuccessEvent(User user)
	{
		User = user;
	}
}
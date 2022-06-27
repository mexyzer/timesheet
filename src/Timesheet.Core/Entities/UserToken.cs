using Timesheet.SharedKernel;

namespace Timesheet.Core.Entities;

public class UserToken : BaseEntity
{
	public UserToken()
	{
		UserTokenId = Guid.NewGuid();
		IsUsed = false;
	}

	public Guid UserTokenId { get; set; }
	public Guid UserId { get; set; }
	public User User { get; set; } = default!;
	public string RefreshToken { get; set; } = string.Empty;
	public DateTime ExpiredUtcDt { get; set; } = DateTime.MinValue;
	public bool IsUsed { get; set; }
}
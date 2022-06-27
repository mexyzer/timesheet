namespace Timesheet.Core.Interfaces;

public interface ICurrentUserService
{
	/// <summary>
	/// Represent user id
	/// </summary>
	string? UserId { get; }

	/// <summary>
	/// Represent user token
	/// </summary>
	string? Idt { get; }

	void SetUserId(string userId);
	void SetIdt(string idt);
}
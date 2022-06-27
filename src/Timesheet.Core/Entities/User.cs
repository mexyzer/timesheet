using Timesheet.SharedKernel;

namespace Timesheet.Core.Entities;

public class User : BaseEntity
{
	public User()
	{
		UserId = Guid.NewGuid();

		UserPasswords = new HashSet<UserPassword>();
		UserRoles = new HashSet<UserRole>();
		UserTokens = new HashSet<UserToken>();
		UserLogins = new HashSet<UserLogin>();
	}

	public Guid UserId { get; set; }
	public string Username { get; set; } = String.Empty;
	public string NormalizedUsername { get; set; } = String.Empty;
	public string? Salt { get; set; }
	public string? HashedPassword { get; set; }
	public string? FirstName { get; set; }
	public string? MiddleName { get; set; }
	public string? LastName { get; set; }
	public string? FullName { get; set; }

	public ICollection<UserPassword> UserPasswords { get; set; }
	public ICollection<UserLogin> UserLogins { get; set; }
	public ICollection<UserRole> UserRoles { get; set; }
	public virtual ICollection<UserToken> UserTokens { get; set; }

	public virtual void UpdateName(string? firstName, string? middleName, string? lastName)
	{
		FirstName = firstName;
		MiddleName = middleName;
		LastName = lastName;

		if (string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(MiddleName) &&
		    string.IsNullOrWhiteSpace(LastName))
			FullName = null;
		else
		{
			string s = FirstName!;
			if (MiddleName.IsNotNullOrWhitespace())
				s += $" {MiddleName}";

			if (LastName.IsNotNullOrWhitespace())
				s += $" {LastName}";

			FullName = s;
		}
	}

	public virtual void SetUsername(string username)
	{
		if (username.Length == 0)
			throw new InvalidOperationException("Username must no be empty");

		Username = username.ToLowerInvariant();
		NormalizedUsername = Username.ToUpperInvariant();
	}

	public virtual void SetPassword(string password)
	{
		if (password.Length == 0)
			throw new InvalidOperationException("Password must no be empty");

		Salt = RandomHelper.GetSecureRandomString(length: 64);
		HashedPassword = string.Concat(Salt, password).ToSHA512();
		UserPasswords.Add(new UserPassword {Salt = Salt, HashedPassword = HashedPassword});
	}
}
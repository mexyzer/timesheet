using System;
using Timesheet.Core.Entities;
using Timesheet.SharedKernel;

namespace Timesheet.UnitTests;

public class UserBuilder
{
	private User _user = new User();

	public UserBuilder WithDefaultValues(RoleType roleType = RoleType.SuperAdministrator)
	{
		string username = "test@mail.com";
		string salt = RandomHelper.GetSecureRandomString(64);
		string password = "Qwerty@123";

		_user = new User
		{
			UserId = Guid.NewGuid(),
			Username = username,
			NormalizedUsername = username.ToUpperInvariant(),
			Salt = salt,
			HashedPassword = $"{salt}{password}".ToSHA512(),
			FirstName = "Test",
			MiddleName = null,
			LastName = "User"
		};

		_user.UserPasswords.Add(new UserPassword
		{
			Salt = _user.Salt, HashedPassword = _user.HashedPassword, CreatedDt = DateTime.UtcNow
		});

		var role = new Role();
		role.Name = roleType switch
		{
			RoleType.Administrator => "Administrator",
			RoleType.SuperAdministrator => "Super Administrator",
			_ => role.Name
		};

		role.NormalizedName = role.Name.ToUpperInvariant();

		role.RolePermissions.Add(new RolePermission {Permission = new Permission {Name = "users.read"}});

		role.RolePermissions.Add(new RolePermission {Permission = new Permission {Name = "users.readwrite"}});

		_user.UserRoles.Add(new UserRole {UserId = _user.UserId, Role = role});

		return this;
	}

	public UserBuilder Id(Guid userId)
	{
		_user.UserId = userId;

		return this;
	}

	public UserBuilder InActive()
	{
		_user.IsActive = false;

		return this;
	}

	public UserBuilder FullName(string? firstName, string? middleName, string? lastName)
	{
		_user.FirstName = firstName;
		_user.MiddleName = middleName;
		_user.LastName = lastName;

		return this;
	}

	public UserBuilder Username(string username)
	{
		_user.Username = username.ToLowerInvariant();
		_user.NormalizedUsername = _user.Username.ToUpperInvariant();

		return this;
	}

	public UserBuilder Password(string password)
	{
		_user.Salt = RandomHelper.GetSecureRandomString(64);
		_user.HashedPassword = $"{_user.Salt}{password}".ToSHA512();

		_user.UserPasswords.Add(new UserPassword
		{
			Salt = _user.Salt, HashedPassword = _user.HashedPassword, CreatedDt = DateTime.UtcNow
		});

		return this;
	}

	public UserBuilder ClearRoles()
	{
		_user.UserRoles.Clear();
		return this;
	}

	public UserBuilder AddUserRole(string roleId)
	{
		_user.UserRoles.Add(
			new UserRole {UserId = _user.UserId, RoleId = new Guid(roleId), CreatedDt = DateTime.UtcNow});
		return this;
	}

	public UserBuilder AddUserRole(Guid roleId)
	{
		_user.UserRoles.Add(new UserRole {UserId = _user.UserId, RoleId = roleId, CreatedDt = DateTime.UtcNow});
		return this;
	}

	public User Build() => _user;
}

public enum RoleType
{
	SuperAdministrator,

	Administrator
}
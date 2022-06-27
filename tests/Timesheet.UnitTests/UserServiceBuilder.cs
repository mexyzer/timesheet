using System;
using System.Threading;
using Moq;
using Timesheet.Core.Entities;
using Timesheet.Core.Interfaces;

namespace Timesheet.UnitTests;

public class UserServiceBuilder
{
	private readonly Mock<IUserService> _mock;

	public UserServiceBuilder()
	{
		_mock = new Mock<IUserService>();
	}

	public UserServiceBuilder GetUserByIdAsync(Guid userId, User? callbackResult)
	{
		string s = userId.ToString();

		_mock.Setup(e => e.GetUserByIdAsync(s, It.IsAny<CancellationToken>()))
			.ReturnsAsync(callbackResult);

		return this;
	}

	public UserServiceBuilder GetRoleByIdAsync(Guid roleId, Role? returnValue)
	{
		_mock.Setup(e => e.GetRoleByIdAsync(roleId.ToString(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(returnValue);
		return this;
	}

	public UserServiceBuilder GetRoleByIdAsync(string roleId, Role? returnValue)
	{
		_mock.Setup(e => e.GetRoleByIdAsync(roleId, It.IsAny<CancellationToken>())).ReturnsAsync(returnValue);
		return this;
	}

	public UserServiceBuilder IsRoleNameExists(string name, bool returnValue)
	{
		_mock.Setup(e => e.IsRoleExistsByNameAsync(name, It.IsAny<CancellationToken>())).ReturnsAsync(returnValue);
		return this;
	}

	public UserServiceBuilder GetRoleByIdAllParams()
	{
		_mock.Setup(e => e.GetRoleByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new Role());
		return this;
	}

	public UserServiceBuilder SetupGetUserByUsername(string? username, User? callbackResult)
	{
		if (string.IsNullOrWhiteSpace(username))
		{
			_mock.Setup(e => e.GetByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(callbackResult);
		}
		else
		{
			_mock.Setup(e => e.GetByUsernameAsync(username, It.IsAny<CancellationToken>()))
				.ReturnsAsync(callbackResult);
		}

		return this;
	}

	public UserServiceBuilder SetupCheckPassword(Guid userId, string password, bool callbackResult)
	{
		_mock.Setup(e => e.CheckPasswordAsync(userId, password, It.IsAny<CancellationToken>()))
			.ReturnsAsync(callbackResult);

		return this;
	}

	public UserServiceBuilder SetupIsEmailAddressExists(string password, bool callbackResult)
	{
		_mock.Setup(e => e.IsEmailAddressExists(password, It.IsAny<CancellationToken>()))
			.ReturnsAsync(callbackResult);

		return this;
	}

	public UserServiceBuilder SetupGetPermissionByIdAsync(Guid userId, Permission? callbackResult)
	{
		string s = userId.ToString();

		_mock.Setup(e => e.GetPermissionByIdAsync(s, It.IsAny<CancellationToken>()))
			.ReturnsAsync(callbackResult);

		return this;
	}

	public UserServiceBuilder SetupGetPermissionByIdAllParams()
	{
		_mock.Setup(e => e.GetPermissionByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new Permission());

		return this;
	}

	public Mock<IUserService> Build()
	{
		return _mock;
	}
}
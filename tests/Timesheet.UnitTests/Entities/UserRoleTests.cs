using System;
using FluentAssertions;
using Timesheet.Core.Entities;
using Xunit;

namespace Timesheet.UnitTests.Entities;

public class UserRoleTests
{
	[Fact]
	public void UserRoleConstructorPropertyUserRoleIdShouldNotBeEmpty()
	{
		var entity = new UserRole();

		entity.UserRoleId.Should().NotBe(Guid.Empty);
	}
}
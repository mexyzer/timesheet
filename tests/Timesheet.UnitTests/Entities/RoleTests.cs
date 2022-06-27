using System;
using FluentAssertions;
using Timesheet.Core.Entities;
using Xunit;

namespace Timesheet.UnitTests.Entities;

public class RoleTests
{
	[Fact]
	public void RoleConstructorPropertyRoleIdShouldNotBeEmpty()
	{
		var role = new Role();

		role.RoleId.Should().NotBe(Guid.Empty);
	}
}
using System;
using FluentAssertions;
using Timesheet.Core.Entities;
using Xunit;

namespace Timesheet.UnitTests.Entities;

public class RolePermissionTests
{
	[Fact]
	public void RolePermissionConstructorShouldBeCorrect()
	{
		var rolePermission = new RolePermission();

		rolePermission.RolePermissionId.Should().NotBe(Guid.Empty);
	}
}
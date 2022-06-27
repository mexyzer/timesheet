using System;
using FluentAssertions;
using Timesheet.Core.Entities;
using Xunit;

namespace Timesheet.UnitTests.Entities;

public class PermissionTests
{
	[Fact]
	public void PermissionConstuctorShouldCorrect()
	{
		var permission = new Permission();

		permission.PermissionId.Should().NotBe(Guid.Empty);
	}
}
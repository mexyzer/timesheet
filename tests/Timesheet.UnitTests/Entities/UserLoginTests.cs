using System;
using FluentAssertions;
using Timesheet.Core.Entities;
using Xunit;

namespace Timesheet.UnitTests.Entities;

public class UserLoginTests
{
	[Fact]
	public void UserLoginConstructEntityPropertyUserLoginIdShouldNotBeEmpty()
	{
		var entity = new UserLogin();

		entity.UserLoginId.Should().NotBe(Guid.Empty);
	}
}
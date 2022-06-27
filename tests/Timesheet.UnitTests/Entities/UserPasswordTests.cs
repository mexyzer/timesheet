using System;
using FluentAssertions;
using Timesheet.Core.Entities;
using Xunit;

namespace Timesheet.UnitTests.Entities;

public class UserPasswordTests
{
	[Fact]
	public void UserPasswordConstructorEntityPropertyUserPasswordIdShouldNotBeEmpty()
	{
		var entity = new UserPassword();

		entity.UserPasswordId.Should().NotBe(Guid.Empty);
	}
}
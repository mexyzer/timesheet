using System;
using FluentAssertions;
using Timesheet.Core.Entities;
using Xunit;

namespace Timesheet.UnitTests.Entities;

public class UserTokenTests
{
	[Fact]
	public void UserTokenConstructorPropertyUserTokenIdShouldNotBeEmpty()
	{
		var entity = new UserToken();

		entity.UserTokenId.Should().NotBe(Guid.Empty);
	}
}
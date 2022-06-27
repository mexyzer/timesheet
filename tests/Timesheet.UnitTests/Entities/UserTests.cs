using System;
using FluentAssertions;
using Xunit;

namespace Timesheet.UnitTests.Entities;

public class UserTests
{
    [Fact]
    public void UserConstructEntityPropertyUserIdShouldNotBeEmpty()
    {
        var user = new UserBuilder().WithDefaultValues().Build();

        user.UserId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void UserConstructEntityICollectionsPropertiesMustNotNull()
    {
        var user = new UserBuilder().WithDefaultValues().Build();

        user.UserPasswords.Should().NotBeNull();
        user.UserLogins.Should().NotBeNull();
        user.UserRoles.Should().NotBeNull();
        user.UserTokens.Should().NotBeNull();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Timesheet.Core.Entities;
using Timesheet.Infrastructure.Services;
using Xunit;

namespace Timesheet.UnitTests.Services;

public class UserServiceTests
{
	[Fact]
	public void GetUserByUsernameShouldReturnCorrectUser()
	{
		const string s = "test@test.com";
		var fakeUsers = new List<User>();
		fakeUsers.Add(new UserBuilder().WithDefaultValues().Username(s).Build());

		var expectedUser = fakeUsers.First(e => e.NormalizedUsername == s.ToUpperInvariant());

		var userService =
			new DefaultUserService(new InterfaceApplicationDbContextBuilder().SetupUser(fakeUsers).Build().Object);

		var result = userService.GetByUsernameAsync(s, CancellationToken.None).GetAwaiter().GetResult();
		result.Should().Be(expectedUser);
	}

	[Fact]
	public void GetUserByUsernameShouldReturnNull()
	{
		const string s = "vendy@timesheet.com";
		var fakeUsers = new List<User>();
		fakeUsers.Add(new UserBuilder().WithDefaultValues().Build());

		var userService =
			new DefaultUserService(new InterfaceApplicationDbContextBuilder().SetupUser(fakeUsers).Build().Object);

		var result = userService.GetByUsernameAsync(s, CancellationToken.None).GetAwaiter().GetResult();

		result.Should().BeNull();
	}

	[Fact]
	public void GetUserByIdReturnCorrectUser()
	{
		var fakeUsers = new List<User>();
		fakeUsers.Add(new UserBuilder().WithDefaultValues().Build());
		var expectedUser = fakeUsers.First();

		var userService =
			new DefaultUserService(new InterfaceApplicationDbContextBuilder().SetupUser(fakeUsers).Build().Object);

		var result = userService.GetUserByIdAsync(expectedUser.UserId.ToString(), CancellationToken.None).GetAwaiter()
			.GetResult();
		result.Should().Be(expectedUser);
	}

	[Fact]
	public void CheckPasswordShouldReturnTrue()
	{
		string expectedPassword = "Qwerty@123";

		var fakeUsers = new List<User>();
		fakeUsers.Add(new UserBuilder().WithDefaultValues().Password(expectedPassword).Build());

		var expectedUser = fakeUsers.First();

		var userService =
			new DefaultUserService(new InterfaceApplicationDbContextBuilder().SetupUser(fakeUsers).Build().Object);

		var result = userService.CheckPasswordAsync(expectedUser.UserId, expectedPassword, CancellationToken.None)
			.GetAwaiter()
			.GetResult();

		result.Should().BeTrue();
	}

	[Fact]
	public void CheckPasswordShouldReturnFalse()
	{
		var fakeUsers = new List<User>();
		fakeUsers.Add(new UserBuilder().WithDefaultValues().Build());

		string fakePassword = "Qwerty@1234567890";
		var expectedUser = fakeUsers.First();

		var userService =
			new DefaultUserService(new InterfaceApplicationDbContextBuilder().SetupUser(fakeUsers).Build().Object);

		var result = userService.CheckPasswordAsync(expectedUser.UserId, fakePassword, CancellationToken.None)
			.GetAwaiter()
			.GetResult();

		result.Should().BeFalse();
	}

	[Fact]
	public void IsEmailAddressExistsShouldReturnFalse()
	{
		var fakeUsers = new List<User>();
		fakeUsers.Add(new UserBuilder().WithDefaultValues().Build());

		var interfaceDbContextBuilder = new InterfaceApplicationDbContextBuilder().SetupUser(fakeUsers).Build();

		var userService = new DefaultUserService(interfaceDbContextBuilder.Object);

		var result = userService.IsEmailAddressExists("vendy@timesheet.com", CancellationToken.None)
			.GetAwaiter()
			.GetResult();

		result.Should().BeFalse();
	}

	[Fact]
	public void IsEmailAddressExistsShouldReturnTrue()
	{
		var fakeUsers = new List<User>();
		var fakeUser = new UserBuilder().WithDefaultValues().Build();
		fakeUsers.Add(fakeUser);

		var interfaceDbContextBuilder = new InterfaceApplicationDbContextBuilder().SetupUser(fakeUsers).Build();

		var userService = new DefaultUserService(interfaceDbContextBuilder.Object);

		var result = userService.IsEmailAddressExists(fakeUser.Username, CancellationToken.None)
			.GetAwaiter()
			.GetResult();

		result.Should().BeTrue();
	}

	[Fact]
	public void GetRoleByIdShouldBeNull()
	{
		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();
		fakeApplicationDbContextBuilder.SetupRole();
		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var service = new DefaultUserService(fakeApplicationDbContext.Object);

		var result = service.GetRoleByIdAsync(Guid.NewGuid().ToString(), CancellationToken.None).GetAwaiter()
			.GetResult();

		result.Should().BeNull();
	}

	[Fact]
	public void GetRoleByIdShouldBeNotNull()
	{
		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();
		var fakeRole = new RoleBuilder().WithDefaultValues().Build();
		fakeApplicationDbContextBuilder.SetupRole(new List<Role> {fakeRole});
		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var service = new DefaultUserService(fakeApplicationDbContext.Object);

		var result = service.GetRoleByIdAsync(fakeRole.RoleId.ToString(), CancellationToken.None).GetAwaiter()
			.GetResult();

		result.Should().NotBeNull();
	}

	[Fact]
	public void IsRoleNameExistsShouldBeFalse()
	{
		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();
		fakeApplicationDbContextBuilder.SetupRole();
		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var service = new DefaultUserService(fakeApplicationDbContext.Object);

		var result = service.IsRoleExistsByNameAsync("abcdefghij", CancellationToken.None).GetAwaiter().GetResult();

		result.Should().BeFalse();
	}

	public static IEnumerable<object[]> TestIsRoleNameExistsCorrectData() =>
		new List<object[]>
		{
			new object[] {"Name using all lowercase", "test"},
			new object[] {"Normal name", "Test"},
			new object[] {"Fixed name", "tEsT"},
			new object[] {"All uppercase", "TEST"}
		};

	[Theory]
	[MemberData(nameof(TestIsRoleNameExistsCorrectData))]
	public void IsNameExistsShouldBeTrue(string message, string value)
	{
		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();
		var fakeRole = new RoleBuilder().WithDefaultValues().Name(value).Build();
		fakeApplicationDbContextBuilder.SetupRole(new List<Role> {fakeRole});
		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var service = new DefaultUserService(fakeApplicationDbContext.Object);

		var result = service.IsRoleExistsByNameAsync(value, CancellationToken.None).GetAwaiter().GetResult();

		result.Should().BeTrue(message);
	}
}
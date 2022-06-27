using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Timesheet.WebApi;
using Timesheet.WebApi.EndPoints.UserManagement;
using Xunit;

namespace Timesheet.UnitTests.Endpoints.UserManagement;

public class EditUserTests
{
	public static readonly EditUserRequest TestCorrectEditUserRequest = new()
	{
		UserId = Guid.NewGuid().ToString(),
		FirstName = "Test",
		MiddleName = null,
		LastName = "User",
		NewPassword = null,
		RoleIds = new[] {Guid.NewGuid().ToString()}
	};

	public static IEnumerable<object[]> TestEditUserValidatorFailData =>
		new List<object[]>
		{
			new object[] {"empty init", new EditUserRequest()},
			new object[]
			{
				"UserId is null",
				new EditUserRequest
				{
					UserId = null!,
					FirstName = "Test",
					MiddleName = null,
					LastName = "User",
					NewPassword = null,
					RoleIds = new[] {Guid.NewGuid().ToString()}
				}
			},
			new object[]
			{
				"UserId is empty",
				new EditUserRequest
				{
					UserId = "",
					FirstName = "Test",
					MiddleName = null,
					LastName = "User",
					NewPassword = null,
					RoleIds = new[] {Guid.NewGuid().ToString()}
				}
			},
			new object[]
			{
				"UserId is non guid",
				new EditUserRequest
				{
					UserId = "  testss",
					FirstName = "Test",
					MiddleName = null,
					LastName = "User",
					NewPassword = null,
					RoleIds = new[] {Guid.NewGuid().ToString()}
				}
			},
			new object[]
			{
				"First name is null",
				new EditUserRequest
				{
					UserId = Guid.NewGuid().ToString(),
					FirstName = null,
					MiddleName = null,
					LastName = "User",
					NewPassword = null,
					RoleIds = new[] {Guid.NewGuid().ToString()}
				}
			},
			new object[]
			{
				"First name is empty",
				new EditUserRequest
				{
					UserId = Guid.NewGuid().ToString(),
					FirstName = string.Empty,
					MiddleName = null,
					LastName = "User",
					NewPassword = null,
					RoleIds = new[] {Guid.NewGuid().ToString()}
				}
			},
			new object[]
			{
				"Role ids is null",
				new EditUserRequest
				{
					UserId = Guid.NewGuid().ToString(),
					FirstName = "Test",
					MiddleName = null,
					LastName = "User",
					NewPassword = null,
					RoleIds = null
				}
			},
			new object[]
			{
				"Role ids is empty",
				new EditUserRequest
				{
					UserId = Guid.NewGuid().ToString(),
					FirstName = "Test",
					MiddleName = null,
					LastName = "User",
					NewPassword = null,
					RoleIds = Array.Empty<string>()
				}
			},
			new object[]
			{
				"Role ids has duplication",
				new EditUserRequest
				{
					UserId = Guid.NewGuid().ToString(),
					FirstName = "Test",
					MiddleName = null,
					LastName = "User",
					NewPassword = null,
					RoleIds = new[]
					{
						"84dce331-ef41-428e-8b37-be67f4df8f27", Guid.NewGuid().ToString(),
						"84dce331-ef41-428e-8b37-be67f4df8f27"
					}
				}
			},
		};

	[Theory]
	[MemberData(nameof(TestEditUserValidatorFailData))]
	public async Task EditUserRequestValidatorShouldReturnFalse(string note, EditUserRequest request)
	{
		var fakeReadRepositoryBuilder = new UserServiceBuilder().GetRoleByIdAllParams();
		var fakeReadRepository = fakeReadRepositoryBuilder.Build();

		var validator =
			new EditUserRequestValidator(fakeReadRepository.Object);

		var validationResult = await validator.ValidateAsync(request, CancellationToken.None);

		validationResult.IsValid.Should().BeFalse($"note : {note}");
	}

	[Fact]
	public async Task EditUserRequestValidatorShouldReturnTrue()
	{
		var request = TestCorrectEditUserRequest;

		var fakeRoleReadRepoBuilder = new UserServiceBuilder();
		foreach (var item in request.RoleIds!)
			fakeRoleReadRepoBuilder.GetRoleByIdAsync(new Guid(item),
				new RoleBuilder().WithDefaultValues().Id(new Guid(item)).Build());

		var validator = new EditUserRequestValidator(fakeRoleReadRepoBuilder.Build().Object);

		var validationResult = await validator.ValidateAsync(request, CancellationToken.None);

		validationResult.IsValid.Should().BeTrue();
	}

	[Theory]
	[MemberData(nameof(TestEditUserValidatorFailData))]
	public async Task EditUserShouldReturnBadRequestWhenValidationIsFalse(string note, EditUserRequest request)
	{
		var fakeUserServiceBuilder = new UserServiceBuilder();
		fakeUserServiceBuilder.GetRoleByIdAllParams();

		var fakeUserService = fakeUserServiceBuilder.Build();

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();

		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var fakeCurrentUserServiceBuilder = new InterfaceCurrentUserServiceBuilder();
		var fakeCurrentUserService = fakeCurrentUserServiceBuilder.Build();

		var handler = new EditUser(fakeUserService.Object, fakeApplicationDbContext.Object,
			fakeCurrentUserService.Object);

		var result = await handler.HandleAsync(request, CancellationToken.None);
		result.Should().BeOfType<BadRequestObjectResult>(because: $"request note: {note}");

		var resultValue = (ObjectResult)result;
		resultValue.Value.Should().BeOfType<ErrorResponse>();

		var value = (ErrorResponse)resultValue.Value!;
		value.Payload.Should().NotBeNull();
	}

	[Fact]
	public async Task EditUserShouldReturnNotFound()
	{
		var request = TestCorrectEditUserRequest;

		var fakeUserServiceBuilder = new UserServiceBuilder();
		foreach (var item in request.RoleIds!)
			fakeUserServiceBuilder.GetRoleByIdAsync(new Guid(item),
				new RoleBuilder().WithDefaultValues().Id(new Guid(item)).Build());

		var fakeUserService = fakeUserServiceBuilder.Build();

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();

		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var fakeCurrentUserServiceBuilder = new InterfaceCurrentUserServiceBuilder();
		var fakeCurrentUserService = fakeCurrentUserServiceBuilder.Build();

		var handler = new EditUser(fakeUserService.Object, fakeApplicationDbContext.Object,
			fakeCurrentUserService.Object);

		var result = await handler.HandleAsync(request, CancellationToken.None);

		result.Should().BeOfType<NotFoundObjectResult>();

		var resultValue = (ObjectResult)result;
		resultValue.Value.Should().BeOfType<ErrorResponse>();
	}

	[Fact]
	public async Task EditUserCorrectFlowWithoutChangePassword()
	{
		var request = TestCorrectEditUserRequest;

		var fakeUserServiceBuilder = new UserServiceBuilder();
		foreach (var item in request.RoleIds!)
			fakeUserServiceBuilder.GetRoleByIdAsync(new Guid(item),
				new RoleBuilder().WithDefaultValues().Id(new Guid(item)).Build());

		const string firstName = "Vaa";
		const string middleName = "Zee";
		const string lastName = "Zuan";
		var fakeUser = new UserBuilder().WithDefaultValues().Id(new Guid(request.UserId))
			.FullName(firstName, middleName, lastName)
			.Build();

		fakeUserServiceBuilder.GetUserByIdAsync(new Guid(request.UserId), fakeUser);

		var fakeUserService = fakeUserServiceBuilder.Build();

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();

		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var fakeCurrentUserServiceBuilder = new InterfaceCurrentUserServiceBuilder();

		var fakeCurrentUserService = fakeCurrentUserServiceBuilder.Build();

		var handler = new EditUser(fakeUserService.Object, fakeApplicationDbContext.Object,
			fakeCurrentUserService.Object);

		var result = await handler.HandleAsync(request, CancellationToken.None);

		result.Should().BeOfType<OkObjectResult>();

		fakeUser.FirstName.Should().Be(request.FirstName);
		fakeUser.MiddleName.Should().Be(request.MiddleName);
		fakeUser.LastName.Should().Be(request.LastName);

		fakeApplicationDbContext.Verify(e => e.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task EditUserCorrectFlowWithChangePassword()
	{
		var request = TestCorrectEditUserRequest;
		request.NewPassword = "HahaHihi";

		var fakeUserServiceBuilder = new UserServiceBuilder();
		foreach (var item in request.RoleIds!)
			fakeUserServiceBuilder.GetRoleByIdAsync(new Guid(item),
				new RoleBuilder().WithDefaultValues().Id(new Guid(item)).Build());

		const string firstName = "Vaa";
		const string middleName = "Zee";
		const string lastName = "Zuan";
		var fakeUser = new UserBuilder().WithDefaultValues().Id(new Guid(request.UserId))
			.FullName(firstName, middleName, lastName)
			.Build();

		fakeUserServiceBuilder.GetUserByIdAsync(new Guid(request.UserId), fakeUser);

		var fakeUserService = fakeUserServiceBuilder.Build();

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();

		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var fakeCurrentUserServiceBuilder = new InterfaceCurrentUserServiceBuilder();

		var fakeCurrentUserService = fakeCurrentUserServiceBuilder.Build();

		var handler = new EditUser(fakeUserService.Object, fakeApplicationDbContext.Object,
			fakeCurrentUserService.Object);

		var saltBefore = fakeUser.Salt;
		var hashedPasswordBefore = fakeUser.HashedPassword;

		var result = await handler.HandleAsync(request, CancellationToken.None);

		result.Should().BeOfType<OkObjectResult>();

		fakeUser.FirstName.Should().Be(request.FirstName);
		fakeUser.MiddleName.Should().Be(request.MiddleName);
		fakeUser.LastName.Should().Be(request.LastName);

		//verify that fakeUser password already change
		fakeUser.Salt.Should().NotBe(saltBefore);
		fakeUser.HashedPassword.Should().NotBe(hashedPasswordBefore);
		fakeUser.UserPasswords.Count.Should().Match(e => e > 1);

		fakeApplicationDbContext.Verify(e => e.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task EditUserCorrectFlowAppendOneRole()
	{
		var request = TestCorrectEditUserRequest;
		var newRoleId = Guid.NewGuid();

		var fakeUserServiceBuilder = new UserServiceBuilder();

		foreach (var item in request.RoleIds!)
			fakeUserServiceBuilder.GetRoleByIdAsync(new Guid(item),
				new RoleBuilder().WithDefaultValues().Id(new Guid(item)).Build());

		//added role
		fakeUserServiceBuilder.GetRoleByIdAsync(newRoleId,
			new RoleBuilder().WithDefaultValues().Id(newRoleId).Build());

		#region Configure User Service Builder

		var fakeUserBuilder = new UserBuilder().WithDefaultValues().Id(new Guid(request.UserId)).ClearRoles();

		var roles = new List<string>();
		foreach (var item in request.RoleIds)
		{
			roles.Add(item);
			fakeUserBuilder.AddUserRole(item);
		}

		var fakeUser = fakeUserBuilder.Build();

		//add new role to the request
		roles.Add(newRoleId.ToString());
		request.RoleIds = roles.ToArray();

		fakeUserServiceBuilder.GetUserByIdAsync(new Guid(request.UserId), fakeUser);

		var fakeUserService = fakeUserServiceBuilder.Build();

		#endregion

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();
		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var fakeCurrentUserServiceBuilder = new InterfaceCurrentUserServiceBuilder();
		var fakeCurrentUserService = fakeCurrentUserServiceBuilder.Build();

		var handler = new EditUser(fakeUserService.Object, fakeApplicationDbContext.Object,
			fakeCurrentUserService.Object);

		var currentTotalUserRole = fakeUser.UserRoles.Count;

		var result = await handler.HandleAsync(request, CancellationToken.None);

		result.Should().BeOfType<OkObjectResult>();

		fakeUser.FirstName.Should().Be(request.FirstName);
		fakeUser.MiddleName.Should().Be(request.MiddleName);
		fakeUser.LastName.Should().Be(request.LastName);

		//because in user service handler, fake user first has 1 user role, then request come in with 2 roles, first one is current, second one is new
		fakeUser.UserRoles.Count.Should().Match(e => e > currentTotalUserRole);
		fakeUser.UserRoles.Any(e => e.RoleId == newRoleId).Should().BeTrue();

		fakeApplicationDbContext.Verify(e => e.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task EditUserCorrectFlowObsoleteExistingUserRoleAppendNewOneOrMoreRoles()
	{
		//default has 1 role
		var request = TestCorrectEditUserRequest;
		//clear default request`s role
		Array.Clear(request.RoleIds!, 0, request.RoleIds!.Length);

		var roles = new List<string>();
		var newRoleId = Guid.NewGuid();
		roles.Add(newRoleId.ToString());
		var newRoleId2 = Guid.NewGuid();
		roles.Add(newRoleId2.ToString());
		var newRoleId3 = Guid.NewGuid();
		roles.Add(newRoleId3.ToString());

		request.RoleIds = roles.ToArray();

		var fakeUserServiceBuilder = new UserServiceBuilder();
		foreach (var item in roles)
			fakeUserServiceBuilder.GetRoleByIdAsync(new Guid(item),
				new RoleBuilder().WithDefaultValues().Id(new Guid(item)).Build());

		//WithDefaultValues() has 1 role
		var fakeUserBuilder = new UserBuilder().WithDefaultValues().Id(new Guid(request.UserId));
		//add dummy current role
		fakeUserBuilder.AddUserRole(Guid.NewGuid());
		var fakeUser = fakeUserBuilder.Build();
		//this having 2 records
		var currentUserRoleIds = new List<Guid>();
		foreach (var item in fakeUser.UserRoles)
			currentUserRoleIds.Add(item.UserRoleId);

		fakeUserServiceBuilder.GetUserByIdAsync(new Guid(request.UserId), fakeUser);

		var fakeUserService = fakeUserServiceBuilder.Build();

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();

		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var fakeCurrentUserServiceBuilder = new InterfaceCurrentUserServiceBuilder();
		fakeCurrentUserServiceBuilder.SetupUserId(Guid.NewGuid());
		var fakeCurrentUserService = fakeCurrentUserServiceBuilder.Build();

		var handler = new EditUser(fakeUserService.Object, fakeApplicationDbContext.Object,
			fakeCurrentUserService.Object);

		var result = await handler.HandleAsync(request, CancellationToken.None);

		result.Should().BeOfType<OkObjectResult>();

		fakeUser.FirstName.Should().Be(request.FirstName);
		fakeUser.MiddleName.Should().Be(request.MiddleName);
		fakeUser.LastName.Should().Be(request.LastName);

		//current user roles should be false
		fakeUser.UserRoles.Where(e => currentUserRoleIds.Contains(e.UserRoleId)).ToList()
			.Should().AllSatisfy(e =>
			{
				e.IsActive.Should().BeFalse();
				e.DeletedBy.Should().NotBeNullOrWhiteSpace();
				e.DeletedDt.Should().NotBeNull();
			});

		fakeUser.UserRoles.Any(e => e.RoleId == newRoleId).Should().BeTrue();
		fakeUser.UserRoles.Any(e => e.RoleId == newRoleId2).Should().BeTrue();
		fakeUser.UserRoles.Any(e => e.RoleId == newRoleId3).Should().BeTrue();

		fakeApplicationDbContext.Verify(e => e.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}
}
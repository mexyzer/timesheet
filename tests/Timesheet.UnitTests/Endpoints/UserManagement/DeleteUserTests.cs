using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Timesheet.WebApi.EndPoints.UserManagement;
using Xunit;

namespace Timesheet.UnitTests.Endpoints.UserManagement;

public class DeleteUserTests
{
	[Fact]
	public async Task DeleteUserShouldReturnNotFound()
	{
		var fakeUserServiceBuilder = new UserServiceBuilder();
		var fakeUserService = fakeUserServiceBuilder.Build();

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();
		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var fakeCurrentUserServiceBuilder = new InterfaceCurrentUserServiceBuilder();
		var fakeCurrentUserService = fakeCurrentUserServiceBuilder.Build();

		var handler = new DeleteUser(fakeUserService.Object, fakeApplicationDbContext.Object,
			fakeCurrentUserService.Object);

		var result = await handler.HandleAsync(new DeleteUserRequest(), CancellationToken.None);
		result.Should().BeOfType<NotFoundObjectResult>();
	}

	[Fact]
	public async Task DeleteUserCorrectFlow()
	{
		var fakeUserServiceBuilder = new UserServiceBuilder();
		var fakeUser = new UserBuilder().WithDefaultValues().Build();
		fakeUserServiceBuilder.GetUserByIdAsync(fakeUser.UserId, fakeUser);
		var fakeUserService = fakeUserServiceBuilder.Build();

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();
		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var fakeCurrentUserServiceBuilder = new InterfaceCurrentUserServiceBuilder();
		fakeCurrentUserServiceBuilder.SetupUserId(Guid.NewGuid());
		var fakeCurrentUserService = fakeCurrentUserServiceBuilder.Build();

		var handler = new DeleteUser(fakeUserService.Object, fakeApplicationDbContext.Object,
			fakeCurrentUserService.Object);

		fakeUser.IsActive.Should().BeTrue();

		var result = await handler.HandleAsync(new DeleteUserRequest {UserId = fakeUser.UserId.ToString()},
			CancellationToken.None);
		result.Should().BeOfType<NoContentResult>();

		fakeUser.IsActive.Should().BeFalse();
		fakeUser.DeletedBy.Should().NotBeNullOrEmpty();
		fakeUser.DeletedDt.Should().NotBeNull();
		fakeApplicationDbContext.Verify(e => e.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}
}
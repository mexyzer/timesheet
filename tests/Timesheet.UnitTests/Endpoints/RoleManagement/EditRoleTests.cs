using System;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Timesheet.WebApi.EndPoints.RoleManagement;
using Xunit;

namespace Timesheet.UnitTests.Endpoints.RoleManagement;

public class EditRoleTests
{
	public static readonly EditRoleRequest TestCorrectEditRoleRequest = new()
	{
		RoleId = Guid.Empty.ToString(),
		Dto = new EditRoleDto
		{
			PermissionIds = new[]
			{
				Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()
			}
		}
	};

	[Fact]
	public void EditRoleCorrectFlow()
	{
		var request = TestCorrectEditRoleRequest;

		var list = request.Dto!.PermissionIds!.ToList();
		list.Add(Guid.NewGuid().ToString());
		list.Add(Guid.NewGuid().ToString());

		var fakeUserServiceBuilder = new UserServiceBuilder();
		foreach (var item in list)
			fakeUserServiceBuilder.SetupGetPermissionByIdAsync(new Guid(item),
				new PermissionBuilder().WithDefaultValues().Id(new Guid(item)).Build());

		var fakeRoleBuilder = new RoleBuilder().WithDefaultValues().Id(new Guid(request.RoleId));

		foreach (var item in request.Dto!.PermissionIds!)
			fakeRoleBuilder.AddPermission(item);

		var id1 = Guid.NewGuid();
		fakeRoleBuilder.AddPermission(id1);

		var fakeRole = fakeRoleBuilder.Build();

		//manipulate dto, add 2 new permissions, so count of roles, must greater
		var countBefore = fakeRole.RolePermissions.Count;

		request.Dto!.PermissionIds = list.ToArray();

		fakeUserServiceBuilder.GetRoleByIdAsync(new Guid(request.RoleId), fakeRole);

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();
		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var fakeCurrentUserServiceBuilder = new InterfaceCurrentUserServiceBuilder();
		fakeCurrentUserServiceBuilder.SetupUserId(Guid.NewGuid().ToString());
		var fakeCurrentUserService = fakeCurrentUserServiceBuilder.Build();

		fakeRole.RolePermissions.First(e => e.PermissionId == id1).DeletedDt.Should().BeNull();
		fakeRole.RolePermissions.First(e => e.PermissionId == id1).IsActive.Should().BeTrue();
		fakeRole.RolePermissions.Count.Should().Be(countBefore);

		var edit = new EditRole(fakeUserServiceBuilder.Build().Object, fakeApplicationDbContext.Object,
			fakeCurrentUserService.Object);

		var result = edit.HandleAsync(request, CancellationToken.None).GetAwaiter().GetResult();

		result.Should().BeOfType<OkObjectResult>();

		fakeRole.RolePermissions.First(e => e.PermissionId == id1).DeletedDt.Should().NotBeNull();
		fakeRole.RolePermissions.First(e => e.PermissionId == id1).IsActive.Should().BeFalse();
		fakeRole.RolePermissions.Count.Should().Match(e => e > countBefore);
		fakeApplicationDbContext.Verify(e => e.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}
}
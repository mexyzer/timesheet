using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Timesheet.Core.Entities;
using Timesheet.WebApi;
using Timesheet.WebApi.EndPoints.RoleManagement;
using Xunit;

namespace Timesheet.UnitTests.Endpoints.RoleManagement;

public class CreateRoleTests
{
	public static readonly CreateRoleRequest TestCorrectCreateRoleRequest = new()
	{
		Name = "Role Test", PermissionIds = new[] {Guid.NewGuid().ToString()}
	};

	public static IEnumerable<object[]> TestCreateRoleRequestValidatorFailData =>
		new List<object[]>
		{
			new object[] {"empty init", new CreateRoleRequest()},
			new object[]
			{
				"name is null",
				new CreateRoleRequest {Name = null, PermissionIds = new[] {Guid.NewGuid().ToString()}}
			},
			new object[]
			{
				"name is empty 1",
				new CreateRoleRequest {Name = string.Empty, PermissionIds = new[] {Guid.NewGuid().ToString()}}
			},
			new object[]
			{
				"name is empty 2",
				new CreateRoleRequest {Name = "", PermissionIds = new[] {Guid.NewGuid().ToString()}}
			},
			new object[]
			{
				"Permission ids is null",
				new CreateRoleRequest {Name = "Super Administrator", PermissionIds = null}
			},
			new object[]
			{
				"Permission ids is empty",
				new CreateRoleRequest {Name = "Super Administrator", PermissionIds = Array.Empty<string>()}
			},
			new object[]
			{
				"Permission ids contain duplication",
				new CreateRoleRequest
				{
					Name = "Super Administrator",
					PermissionIds = new[]
					{
						Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
						new Guid("fc822f58-6c82-4d24-b04d-1f0fe3751a18").ToString(),
						new Guid("fc822f58-6c82-4d24-b04d-1f0fe3751a18").ToString()
					}
				}
			}
		};

	[Theory]
	[MemberData(nameof(TestCreateRoleRequestValidatorFailData))]
	public async Task CreateRoleRequestValidatorShouldFail(string type, CreateRoleRequest request)
	{
		var validator =
			new CreateRoleRequestValidator(new UserServiceBuilder().SetupGetPermissionByIdAllParams().Build()
				.Object);

		var validationResult = await validator.ValidateAsync(request, CancellationToken.None);

		validationResult.IsValid.Should().BeFalse($"with type of {type}");
	}

	[Fact]
	public async Task CreateRoleRequestValidatorShouldReturnTrue()
	{
		var request = TestCorrectCreateRoleRequest;

		var fakeUserServiceBuilder = new UserServiceBuilder();
		foreach (var item in request.PermissionIds!)
			fakeUserServiceBuilder.SetupGetPermissionByIdAsync(new Guid(item),
				new PermissionBuilder().WithDefaultValues().Id(new Guid(item)).Build());

		var validator = new CreateRoleRequestValidator(fakeUserServiceBuilder.Build().Object);

		var validationResult = await validator.ValidateAsync(request, CancellationToken.None);

		validationResult.IsValid.Should().BeTrue();
	}

	[Theory]
	[MemberData(nameof(TestCreateRoleRequestValidatorFailData))]
	public async Task CreateRoleShouldReturnBadRequestWhenValidationFail(string note, CreateRoleRequest request)
	{
		var fakeUserServiceBuilder = new UserServiceBuilder();

		if (request.PermissionIds != null)
			foreach (var item in request.PermissionIds!)
				if (!string.IsNullOrWhiteSpace(item))
					fakeUserServiceBuilder.SetupGetPermissionByIdAsync(new Guid(item),
						new PermissionBuilder().WithDefaultValues().Id(new Guid(item)).Build());

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();
		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var handler = new CreateRole(fakeUserServiceBuilder.Build().Object,
			fakeApplicationDbContext.Object);

		var result = await handler.HandleAsync(request, CancellationToken.None);

		result.Should().BeOfType<BadRequestObjectResult>($"request note : {note}");

		var resultValue = (ObjectResult)result;

		resultValue.Value.Should().BeOfType<ErrorResponse>();
		var value = (ErrorResponse)resultValue.Value!;
		value.Payload.Should().NotBeNull();
	}

	[Fact]
	public void CreateRoleShouldReturnBadRequestWhenRequestNameAlreadyExists()
	{
		var request = TestCorrectCreateRoleRequest;

		var fakeUserServiceBuilder = new UserServiceBuilder();

		foreach (var item in request.PermissionIds!)
			fakeUserServiceBuilder.SetupGetPermissionByIdAsync(new Guid(item),
				new PermissionBuilder().WithDefaultValues().Id(new Guid(item)).Build());

		//set is name exists with parameter request.Name!
		fakeUserServiceBuilder.IsRoleNameExists(request.Name!, true);

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();
		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var handler = new CreateRole(fakeUserServiceBuilder.Build().Object,
			fakeApplicationDbContext.Object);

		var result = handler.HandleAsync(request, CancellationToken.None).GetAwaiter().GetResult();

		result.Should().BeOfType<BadRequestObjectResult>();

		var resultValue = (ObjectResult)result;

		resultValue.Value.Should().BeOfType<ErrorResponse>();
		var value = (ErrorResponse)resultValue.Value!;
		value.Message.Should().Be("Role name already exists");
	}

	[Fact]
	public void CreateRoleCorrectFlow()
	{
		var request = TestCorrectCreateRoleRequest;

		var fakeUserServiceBuilder = new UserServiceBuilder();

		foreach (var item in request.PermissionIds!)
			fakeUserServiceBuilder.SetupGetPermissionByIdAsync(new Guid(item),
				new PermissionBuilder().WithDefaultValues().Id(new Guid(item)).Build());
		//set is name exists with parameter request.Name!
		fakeUserServiceBuilder.IsRoleNameExists(request.Name!, false);

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();
		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		//check roles added to entity must correct from parameters
		fakeApplicationDbContext.Setup(e => e.Roles.Add(It.IsAny<Role>())).Callback<Role>(e =>
		{
			e.Name.Should().Be(request.Name);
			e.NormalizedName.Should().Be(request.Name!.ToUpperInvariant());
			e.RolePermissions.Should().HaveCount(x => x > 0);
			e.RolePermissions.Count.Should().Be(request.PermissionIds.Length);
		});

		var handler = new CreateRole(fakeUserServiceBuilder.Build().Object,
			fakeApplicationDbContext.Object);

		var result = handler.HandleAsync(request, CancellationToken.None).GetAwaiter().GetResult();

		result.Should().BeOfType<OkObjectResult>();

		fakeApplicationDbContext.Verify(e => e.Roles.Add(It.IsAny<Role>()), Times.Once);
		fakeApplicationDbContext.Verify(e => e.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}
}
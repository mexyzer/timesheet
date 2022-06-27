using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Timesheet.Core.Entities;
using Timesheet.Core.Interfaces;
using Timesheet.WebApi;
using Timesheet.WebApi.EndPoints.RoleManagement;
using Timesheet.WebApi.Models;
using Xunit;

namespace Timesheet.FunctionalTests.Endpoints;

[Collection("Role API Collection")]
public class CreateRoleFlow : BaseEndpointTest
{
	private readonly CustomWebApplicationFactory<WebMarker> _factory;
	private readonly HttpClient _client;

	public CreateRoleFlow(CustomWebApplicationFactory<WebMarker> factory) : base(factory)
	{
		_factory = factory;
		_client = factory.CreateClient();
	}

	/// <summary>
	/// PSEUDOCODE
	/// 1. Create ROLE
	/// 2. Get Role by ID
	/// 3. Result not null and exact value from create`s payload
	/// </summary>
	[Fact]
	public async Task CreateRoleFlowShouldBeOk()
	{
		Permission? permissionReadWriteUser;
		using (var scope = _factory.Services.CreateScope())
		{
			var userService = scope.ServiceProvider.GetService<IUserService>()!;

			permissionReadWriteUser =
				await userService.GetPermissionByNameAsync("users.readwrite", CancellationToken.None);
		}

		var accessToken = await GetAccessTokenAsync();

		var createRoleRequest = new CreateRoleRequest();
		createRoleRequest.Name = "Observer";
		createRoleRequest.PermissionIds = new[] {permissionReadWriteUser!.PermissionId.ToString()};

		var req = HttpHelper.CreateRequest($"{_client.BaseAddress!.AbsoluteUri}{CreateRoleRequest.Route}",
			HttpMethod.Post,
			new AuthenticationHeaderValue("Bearer", accessToken),
			null,
			new StringContent(JsonHelper.Serialize(createRoleRequest), Encoding.UTF8, "application/json"));

		var result = await _client.SendAsync(req);
		var resp = await result.Content.ReadAsStringAsync();

		result.StatusCode.Should().Be(HttpStatusCode.OK);

		var createRoleResponse = JsonHelper.Deserialize<KeyDto>(resp);

		req = HttpHelper.CreateRequest(
			$"{_client.BaseAddress!.AbsoluteUri}{GetRoleByIdRequest.BuildRoute(createRoleResponse.Id)}",
			HttpMethod.Get,
			new AuthenticationHeaderValue("Bearer", accessToken));
		result = await _client.SendAsync(req);

		resp = await result.Content.ReadAsStringAsync();

		result.StatusCode.Should().Be(HttpStatusCode.OK);

		var roleResponse = JsonHelper.Deserialize<RoleResponse>(resp);

		roleResponse.name.Should().Be(createRoleRequest.Name);
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Timesheet.SharedKernel;
using Timesheet.WebApi;
using Timesheet.WebApi.EndPoints.RoleManagement;
using Timesheet.WebApi.EndPoints.Users;
using Timesheet.WebApi.Models;

namespace Timesheet.FunctionalTests.Endpoints;

public class BaseEndpointTest
{
	private readonly HttpClient _client;

	protected BaseEndpointTest(CustomWebApplicationFactory<WebMarker> factory)
	{
		_client = factory.CreateClient();
		_accessToken = null;
		_roles = null;
		_superAdministratorRole = null;
	}

	private string? _accessToken;
	private List<RoleResponse>? _roles;
	private RoleResponse? _superAdministratorRole;

	protected async Task<RoleResponse> GetSuperAdministratorAsync()
	{
		if (_superAdministratorRole != null)
			return _superAdministratorRole;

		var accessToken = await GetAccessTokenAsync();

		var s = $"{_client.BaseAddress!.AbsoluteUri}{GetRoleByIdRequest.BuildRoute(Guid.Empty)}";
		var req = HttpHelper.CreateRequest(
			s,
			HttpMethod.Get,
			new AuthenticationHeaderValue("Bearer", accessToken));

		var result = await _client.SendAsync(req);

		var resp = await result.Content.ReadAsStringAsync();

		var roleResponse = JsonHelper.Deserialize<RoleResponse>(resp);

		_superAdministratorRole = roleResponse;

		return roleResponse;
	}

	protected async Task<List<RoleResponse>> GetRolesAsync()
	{
		if (_roles != null && _roles.Any())
			return _roles;

		var accessToken = await GetAccessTokenAsync();

		var req = HttpHelper.CreateRequest($"{_client.BaseAddress!.AbsoluteUri}{GetRoleRequest.Route}",
			HttpMethod.Get,
			new AuthenticationHeaderValue("Bearer", accessToken));

		var result = await _client.SendAsync(req);

		var resp = await result.Content.ReadAsStringAsync();

		var listResponse = JsonHelper.Deserialize<PaginatedList<RoleResponse>>(resp);

		_roles = new List<RoleResponse>();
		_roles.AddRange(listResponse.Items);

		return _roles;
	}

	protected async Task<string> GetAccessTokenAsync()
	{
		if (_accessToken.IsNotNullOrEmpty())
			return _accessToken!;

		var loginRequest = new LoginRequest {Username = "sa@mail.com", Password = "Qwerty@1234"};

		var req = HttpHelper.CreateRequest($"{_client.BaseAddress!.AbsoluteUri}{LoginRequest.Route}",
			HttpMethod.Post,
			null,
			null,
			new StringContent(JsonHelper.Serialize(loginRequest), Encoding.UTF8, "application/json"));

		var result = await _client.SendAsync(req);

		var resp = await result.Content.ReadAsStringAsync();

		var loginResponse = JsonHelper.Deserialize<LoginResponse>(resp);

		_accessToken = loginResponse.Token;

		return _accessToken;
	}
}
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Timesheet.WebApi;
using Timesheet.WebApi.EndPoints.UserManagement;
using Timesheet.WebApi.Models;
using Xunit;

namespace Timesheet.FunctionalTests.Endpoints;

[Collection("User API Collection")]
public class CreateUserFlow : BaseEndpointTest
{
	private readonly HttpClient _client;

	public CreateUserFlow(CustomWebApplicationFactory<WebMarker> factory) : base(factory)
	{
		_client = factory.CreateClient();
	}

	/// <summary>
	/// PSEUDOCODE
	/// 1. Create user
	/// 2. Get user by id
	/// 3. Result not null and exact value from create json request
	/// </summary>
	[Fact]
	public async Task CreateFlowShouldBeOk()
	{
		var role = await GetSuperAdministratorAsync();
		var accessToken = await GetAccessTokenAsync();

		CreateUserRequest createUserRequest = new()
		{
			Username = "vendy@timesheet.com",
			Password = "Test.@123",
			ConfirmPassword = "Test.@123",
			FirstName = "Test",
			MiddleName = "Vendy",
			LastName = "Radyalabs",
			RoleIds = new[] {role.roleId.ToString()}
		};

		string fullName = $"{createUserRequest.FirstName} {createUserRequest.MiddleName} {createUserRequest.LastName}";

		var req = HttpHelper.CreateRequest($"{_client.BaseAddress!.AbsoluteUri}{CreateUserRequest.Route}",
			HttpMethod.Post,
			new AuthenticationHeaderValue("Bearer", accessToken),
			null,
			new StringContent(JsonHelper.Serialize(createUserRequest), Encoding.UTF8, "application/json"));

		var result = await _client.SendAsync(req);
		var resp = await result.Content.ReadAsStringAsync();

		result.StatusCode.Should().Be(HttpStatusCode.OK);

		var createUserResponse = JsonHelper.Deserialize<KeyDto>(resp);

		req = HttpHelper.CreateRequest(
			$"{_client.BaseAddress!.AbsoluteUri}{GetUserByIdRequest.BuildRoute(createUserResponse.Id)}",
			HttpMethod.Get,
			new AuthenticationHeaderValue("Bearer", accessToken));

		result = await _client.SendAsync(req);

		resp = await result.Content.ReadAsStringAsync();

		result.StatusCode.Should().Be(HttpStatusCode.OK);

		var userResponse = JsonHelper.Deserialize<UserResponse>(resp);

		userResponse.firstName.Should().Be(createUserRequest.FirstName);
		userResponse.middleName.Should().Be(createUserRequest.MiddleName);
		userResponse.lastName.Should().Be(createUserRequest.LastName);
		userResponse.fullName.Should().Be(fullName);
	}
}
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Timesheet.WebApi;
using Timesheet.WebApi.EndPoints.Users;
using Xunit;

namespace Timesheet.FunctionalTests.Endpoints;

[Collection("User API Collection")]
public class Login
{
	private readonly HttpClient _client;

	public Login(CustomWebApplicationFactory<WebMarker> factory)
	{
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task ReturnOk()
	{
		var loginRequest = new LoginRequest {Username = "sa@mail.com", Password = "Qwerty@1234"};

		var req = HttpHelper.CreateRequest($"{_client.BaseAddress!.AbsoluteUri}{LoginRequest.Route}",
			HttpMethod.Post,
			null,
			null,
			new StringContent(JsonHelper.Serialize(loginRequest), Encoding.UTF8, "application/json"));

		var result = await _client.SendAsync(req);

		var resp = await result.Content.ReadAsStringAsync();

		var loginResponse = JsonHelper.Deserialize<LoginResponse>(resp);

		loginResponse.Should().NotBeNull();
		result.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task ReturnBadRequest()
	{
		var loginRequest = new LoginRequest {Username = "sa@mail.com", Password = "Test"};

		var req = HttpHelper.CreateRequest($"{_client.BaseAddress!.AbsoluteUri}{LoginRequest.Route}",
			HttpMethod.Post,
			null,
			null,
			new StringContent(JsonHelper.Serialize(loginRequest), Encoding.UTF8, "application/json"));

		var result = await _client.SendAsync(req);

		var resp = await result.Content.ReadAsStringAsync();

		var errorLoginResponse = JsonHelper.Deserialize<ErrorResponse>(resp);

		errorLoginResponse.Should().NotBeNull();
		errorLoginResponse.Message.Should().NotBeEmpty();
		result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}
}
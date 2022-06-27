using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FluentAssertions;
using Timesheet.WebApi;
using Timesheet.WebApi.EndPoints.Users;
using Xunit;

namespace Timesheet.FunctionalTests.Endpoints;

[Collection("User API Collection")]
public class RefreshTokenFlow
{
	private readonly HttpClient _client;

	public RefreshTokenFlow(CustomWebApplicationFactory<WebMarker> factory)
	{
		_client = factory.CreateClient();
	}

	/// <summary>
	/// PSEUDOCODE
	/// 1. Login success
	/// 2. Refresh token using login response`s refresh token property
	/// 3. Result should be ok
	/// </summary>
	[Fact]
	public async Task RefreshTokenFlowShouldOk()
	{
		var loginRequest = new LoginRequest {Username = "sa@mail.com", Password = "Qwerty@1234"};

		//login request
		var req = HttpHelper.CreateRequest($"{_client.BaseAddress!.AbsoluteUri}{LoginRequest.Route}",
			HttpMethod.Post,
			null,
			null,
			new StringContent(JsonHelper.Serialize(loginRequest), Encoding.UTF8, "application/json"));
		var result = await _client.SendAsync(req);
		var resp = await result.Content.ReadAsStringAsync();

		var loginResult = JsonHelper.Deserialize<LoginResponse>(resp);

		//refresh token request
		var uriBuilder = new UriBuilder($"{_client.BaseAddress!.AbsoluteUri}{RefreshTokenRequest.Route}");
		var query = HttpUtility.ParseQueryString(uriBuilder.Query);
		query["Token"] = loginResult.RefreshToken;
		uriBuilder.Query = query.ToString();

		req = HttpHelper.CreateRequest(uriBuilder.Uri.AbsoluteUri, HttpMethod.Get,
			new AuthenticationHeaderValue("Bearer", loginResult.Token));
		result = await _client.SendAsync(req);
		resp = await result.Content.ReadAsStringAsync();

		result.StatusCode.Should().Be(HttpStatusCode.OK, resp);
	}
}
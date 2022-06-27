using Timesheet.Core.Entities;
using Timesheet.Core.Interfaces;
using Timesheet.SharedKernel;
using Timesheet.WebApi.Services;

namespace Timesheet.WebApi.EndPoints.Users;

public class LoginResponse
{
	public string Token { get; init; } = string.Empty;
	public string RefreshToken { get; init; } = string.Empty;
	public string? FirstName { get; set; }
	public string? MiddleName { get; set; }
	public string? LastName { get; set; }
	public long Expires { get; set; }
}

public static class LoginResponseExtension
{
	public static LoginResponse Build(IDateTime dateTime, JwtService jwtService, User user, string identifier,
		string refreshToken)
	{
		return new LoginResponse()
		{
			Token = jwtService.GenerateJwtToken(user, identifier),
			RefreshToken = refreshToken,
			FirstName = user.FirstName,
			MiddleName = user.MiddleName,
			LastName = user.LastName,
			// just pre-caution of expires created on JwtService.GenerateJwtToken
			Expires = dateTime.ScopedUtcNow.AddDays(7).AddSeconds(-5).ToMilliseconds()
		};
	}
}
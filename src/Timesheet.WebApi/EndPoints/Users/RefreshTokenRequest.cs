using Microsoft.AspNetCore.Mvc;

namespace Timesheet.WebApi.EndPoints.Users;

public class RefreshTokenRequest
{
    public const string Route = "api/users/refresh-token";

    [FromQuery] public string Token { get; init; } = string.Empty;
}
namespace Timesheet.WebApi.EndPoints.Users;

public class LoginRequest
{
    public const string Route = "api/users/login";

    public string? Username { get; set; }
    public string? Password { get; set; }
}
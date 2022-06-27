using Microsoft.Extensions.Options;
using Timesheet.SharedKernel;
using Timesheet.WebApi;
using Timesheet.WebApi.Services;

namespace Timesheet.UnitTests;

public class JwtServiceBuilder
{
    private readonly JwtService _jwtService;

    public JwtServiceBuilder()
    {
        _jwtService =
            new JwtService(Options.Create(new ApplicationOption() {SecretKey = RandomHelper.GetSecureRandomString(64)}));
    }

    public JwtService Build()
    {
        return _jwtService;
    }
}
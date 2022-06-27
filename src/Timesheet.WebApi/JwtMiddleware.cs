using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Timesheet.Core.Interfaces;

namespace Timesheet.WebApi;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApplicationOption _option;

    public JwtMiddleware(RequestDelegate next, IOptions<ApplicationOption> options)
    {
        _next = next;
        _option = options.Value;
    }

    public async Task Invoke(HttpContext context, ICurrentUserService userService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
            AttachUserToContext(context, userService, token);

        await _next(context);
    }

    private void AttachUserToContext(HttpContext context, ICurrentUserService userService, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_option.SecretKey!);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            // attach user to context on successful jwt validation
            var userId = jwtToken.Claims.FirstOrDefault(e => e.Type == "id")!.Value;
            userService.SetUserId(userId);
            var idt = jwtToken.Claims.FirstOrDefault(e => e.Type == "idt")!.Value;
            userService.SetIdt(idt);
        }
        catch
        {
            // do nothing if jwt validation fails
            // user is not attached to context so request won't have access to secure routes
        }
    }
}
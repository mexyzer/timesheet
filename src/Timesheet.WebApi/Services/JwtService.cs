using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Timesheet.Core.Entities;

namespace Timesheet.WebApi.Services;

public class JwtService
{
    private readonly ApplicationOption _option;

    public JwtService(IOptions<ApplicationOption> options)
    {
        _option = options.Value;
    }

    public string GenerateJwtToken(User user, string identifier)
    {
        var claims = new List<Claim>();
        claims.Add(new Claim("id", user.UserId.ToString()));
        claims.Add(new Claim("idt", identifier));
        if (!string.IsNullOrWhiteSpace(user.FirstName) || !string.IsNullOrWhiteSpace(user.MiddleName) ||
            !string.IsNullOrWhiteSpace(user.LastName))
            claims.Add(new Claim("fn", $"{user.FirstName} {user.MiddleName} {user.LastName}".Trim()));

        if (user.UserRoles.Any())
            foreach (var item in user.UserRoles)
                if (item.Role!.RolePermissions.Any())
                    foreach (var item2 in item.Role.RolePermissions)
                        claims.Add(new Claim("prm", item2.Permission!.Name));

        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_option.SecretKey!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KrMicro.Core.CQS.Command.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace KrMicro.Core.Services;

public static class JwtUtils
{
    public static string GenerateToken(GenerateJwtCommandRequest user, string jwtKey, string? jwtIssuer = null,
        string? jwtAudience = null)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var token = new JwtSecurityToken(
            jwtIssuer,
            jwtAudience,
            claims,
            expires: DateTime.Now.AddHours(24),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // No need to split string
    public static string? GetUserNameByToken(string accessToken)
    {
        var token = accessToken.Split(' ').LastOrDefault();
        if (token == null) return null;
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        if (jsonToken == null) return null;
        return jsonToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
    }

    public static string? GetUserRoleByToken(string accessToken)
    {
        var token = accessToken.Split(' ').LastOrDefault();
        if (token == null) return null;
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        if (jsonToken == null) return null;
        return jsonToken.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;
    }
}
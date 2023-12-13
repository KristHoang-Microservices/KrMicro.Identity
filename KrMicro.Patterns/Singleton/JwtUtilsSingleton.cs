using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KrMicro.Core.CQS.Command.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace KrMicro.Patterns.Singleton;

public class JwtUtilsSingleton
{
    private static JwtUtilsSingleton? _instance;
    private static string? _key;
    private static string? _jwtIssuer;
    private static string? _jwtAudience;

    private JwtUtilsSingleton()
    {
    }

    public static JwtUtilsSingleton GetInstance()
    {
        return _instance ??= new JwtUtilsSingleton();
    }

    public static void SetKey(string key)
    {
        _key = key;
    }

    public static void SetIssuer(string jwtIssuer)
    {
        _jwtIssuer = jwtIssuer;
    }

    public static void SetAudience(string jwtAudience)
    {
        _jwtAudience = jwtAudience;
    }

    public string? GenerateToken(GenerateJwtCommandRequest user)
    {
        if (_key == null) return null;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var token = new JwtSecurityToken(
            _jwtIssuer,
            _jwtAudience,
            claims,
            expires: DateTime.Now.AddHours(24),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // No need to split string
    public string? GetUserNameByToken(string accessToken)
    {
        var token = accessToken.Split(' ').LastOrDefault();
        if (token == null) return null;
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        if (jsonToken == null) return null;
        return jsonToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
    }

    public string? GetUserRoleByToken(string accessToken)
    {
        var token = accessToken.Split(' ').LastOrDefault();
        if (token == null) return null;
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        if (jsonToken == null) return null;
        return jsonToken.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;
    }
}
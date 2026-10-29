using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Domain.Entities;
using Cinema.Shared.Constants;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Cinema.Application.Services.Auth;

public class JwtTokenService(IOptions<JwtOptions> jwtOptions) : ITokenService
{
    private readonly JwtOptions _options = jwtOptions.Value;

    public AccessTokenResult GenerateAccessToken(User user)
    {
        var claims = BuildBaseClaims(user);
        var token = BuildToken(claims, TimeSpan.FromSeconds(_options.AccessTokenExpirationSeconds));

        return new AccessTokenResult(token, _options.AccessTokenExpirationSeconds);
    }

    public string GenerateOpaqueToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);

        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }

    public AccessTokenResult GenerateSocketToken(User user)
    {
        var expirationSeconds = _options.SocketTokenExpirationSeconds;
        var claims = BuildBaseClaims(user);
        var token = BuildToken(claims, TimeSpan.FromSeconds(expirationSeconds));

        return new AccessTokenResult(token, expirationSeconds);
    }

    public string HashOpaqueToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes);
    }

    private static List<Claim> BuildBaseClaims(User user)
    {
        var claims = new List<Claim>
        {
            new(AppClaimTypes.UserId, user.UserId.ToString()),
            new(AppClaimTypes.Role, user.Role.RoleName),
            new(ClaimTypes.Role, user.Role.RoleName),
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
        };

        if (user.Employee is not null)
            claims.Add(new Claim(AppClaimTypes.EmployeeId, user.Employee.EmployeeId.ToString()));

        return claims;
    }

    private string BuildToken(IEnumerable<Claim> claims, TimeSpan lifetime)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(lifetime),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

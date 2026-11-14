using System.IdentityModel.Tokens.Jwt;
using Cinema.Application.Options;
using Cinema.Application.Services.Auth;
using Cinema.Domain.Entities;
using Cinema.Shared.Constants;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class JwtMustChangePasswordClaimTests
{
    private static JwtTokenService CreateService() =>
        new(
            Microsoft.Extensions.Options.Options.Create(
                new JwtOptions
                {
                    Issuer = "Perfo",
                    Audience = "Perfo.Client",
                    Secret = "unit-test-secret-key-at-least-32-chars-long",
                    AccessTokenExpirationSeconds = 3600,
                }
            )
        );

    private static User BuildUser(bool mustChange) =>
        new()
        {
            UserId = 5,
            FullName = "Staff",
            Email = "staff@test.local",
            PasswordHash = "hash",
            RoleId = 3,
            Role = new Role { RoleId = 3, RoleName = Roles.Employee },
            MustChangePassword = mustChange,
        };

    [Fact]
    public void GenerateAccessToken_MustChangePassword_ContainsClaim()
    {
        var token = CreateService().GenerateAccessToken(BuildUser(mustChange: true));

        new JwtSecurityTokenHandler()
            .ReadJwtToken(token.AccessToken)
            .Claims.ShouldContain(c => c.Type == AppClaimTypes.MustChangePassword && c.Value == "true");
    }

    [Fact]
    public void GenerateAccessToken_NoFlag_OmitsClaim()
    {
        var token = CreateService().GenerateAccessToken(BuildUser(mustChange: false));

        new JwtSecurityTokenHandler()
            .ReadJwtToken(token.AccessToken)
            .Claims.ShouldNotContain(c => c.Type == AppClaimTypes.MustChangePassword);
    }
}

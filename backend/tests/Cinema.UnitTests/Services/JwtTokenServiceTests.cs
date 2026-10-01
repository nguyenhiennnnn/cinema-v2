using System.IdentityModel.Tokens.Jwt;
using Cinema.Application.Options;
using Cinema.Application.Services.Auth;
using Cinema.Domain.Entities;
using Cinema.Shared.Constants;
using Microsoft.Extensions.Options;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class JwtTokenServiceTests
{
    private static JwtTokenService CreateService(JwtOptions? options = null) =>
        new(Options.Create(options ?? DefaultOptions()));

    private static JwtOptions DefaultOptions() =>
        new()
        {
            Issuer = "Cinema",
            Audience = "Cinema.Client",
            Secret = "unit-test-secret-key-at-least-32-chars-long",
            AccessTokenExpirationSeconds = 3600,
            RefreshTokenExpirationDays = 30,
        };

    private static User BuildUser(
        int userId = 42,
        string role = Roles.Customer,
        int? employeeId = null
    ) =>
        new()
        {
            UserId = userId,
            FullName = "Test User",
            Email = "user@test.local",
            PasswordHash = "hash",
            RoleId = role == Roles.Admin ? 2 : 1,
            Role = new Role { RoleId = role == Roles.Admin ? 2 : 1, RoleName = role },
            Employee = employeeId is null
                ? null
                : new Employee { EmployeeId = employeeId.Value, UserId = userId },
        };

    [Fact]
    public void GenerateAccessToken_ProducesTokenWithExpectedClaims()
    {
        var service = CreateService();
        var user = BuildUser(userId: 7, role: Roles.Customer);

        var result = service.GenerateAccessToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);
        jwt.Claims.ShouldContain(c => c.Type == AppClaimTypes.UserId && c.Value == "7");
        jwt.Claims.ShouldContain(c => c.Type == AppClaimTypes.Role && c.Value == Roles.Customer);
        jwt.Claims.ShouldContain(c =>
            c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email
        );
        jwt.Issuer.ShouldBe("Cinema");
        jwt.Audiences.ShouldContain("Cinema.Client");
        result.ExpiresIn.ShouldBe(3600);
    }

    [Fact]
    public void GenerateAccessToken_ForEmployee_IncludesEmployeeIdClaim()
    {
        var service = CreateService();
        var user = BuildUser(role: Roles.Admin, employeeId: 99);

        var token = service.GenerateAccessToken(user);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.AccessToken);

        jwt.Claims.ShouldContain(c => c.Type == AppClaimTypes.EmployeeId && c.Value == "99");
    }

    [Fact]
    public void GenerateAccessToken_ForCustomerWithoutEmployee_OmitsEmployeeIdClaim()
    {
        var service = CreateService();
        var user = BuildUser(role: Roles.Customer, employeeId: null);

        var token = service.GenerateAccessToken(user);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.AccessToken);

        jwt.Claims.ShouldNotContain(c => c.Type == AppClaimTypes.EmployeeId);
    }

    [Fact]
    public void GenerateSocketToken_UsesFixed5MinuteExpiration_RegardlessOfAccessTokenOptions()
    {
        var options = DefaultOptions();
        options.AccessTokenExpirationSeconds = 3600;
        var service = CreateService(options);
        var user = BuildUser();

        var result = service.GenerateSocketToken(user);

        result.ExpiresIn.ShouldBe(300);
    }

    [Fact]
    public void GenerateOpaqueToken_ProducesUrlSafeNonEmptyToken()
    {
        var service = CreateService();

        var token = service.GenerateOpaqueToken();

        token.ShouldNotBeNullOrWhiteSpace();
        token.ShouldNotContain("+");
        token.ShouldNotContain("/");
        token.ShouldNotContain("=");
    }

    [Fact]
    public void GenerateOpaqueToken_ProducesDifferentTokensEachCall()
    {
        var service = CreateService();

        service.GenerateOpaqueToken().ShouldNotBe(service.GenerateOpaqueToken());
    }

    [Fact]
    public void HashOpaqueToken_IsDeterministic()
    {
        var service = CreateService();

        service.HashOpaqueToken("same-input").ShouldBe(service.HashOpaqueToken("same-input"));
    }

    [Fact]
    public void HashOpaqueToken_DifferentInputsProduceDifferentHashes()
    {
        var service = CreateService();

        service.HashOpaqueToken("input-a").ShouldNotBe(service.HashOpaqueToken("input-b"));
    }

    [Fact]
    public void HashOpaqueToken_IsNotReversibleToOriginalPlainToken()
    {
        var service = CreateService();
        const string original = "my-raw-refresh-token";

        service.HashOpaqueToken(original).ShouldNotBe(original);
    }
}

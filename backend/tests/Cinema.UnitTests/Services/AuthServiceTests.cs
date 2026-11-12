using Cinema.Application.DTOs.Auth;
using Cinema.Domain.Entities;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class AuthServiceLoginTests(PostgresContainerFixture fixture) : AuthServiceTestBase(fixture)
{
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsTokensAndUserDto()
    {
        var user = TestDataSeeder.AddCustomer(Db, email: "login@test.local");
        PasswordHasherMock
            .Setup(h =>
                h.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), "correct-password")
            )
            .Returns(PasswordVerificationResult.Success);
        var sut = CreateAuthService();

        var response = await sut.LoginAsync(
            new LoginRequest("login@test.local", "correct-password"),
            adminOnly: false,
            CancellationToken.None
        );

        response.AccessToken.ShouldBe("fake-jwt");
        response.User.UserId.ShouldBe(user.UserId);
        response.User.Role.ShouldBe(Roles.Customer);
        (await Db.RefreshTokens.CountAsync(rt => rt.UserId == user.UserId)).ShouldBe(1);
    }

    [Fact]
    public async Task LoginAsync_UnknownEmail_ThrowsUnauthorized()
    {
        var sut = CreateAuthService();

        await Should.ThrowAsync<UnauthorizedAppException>(() =>
            sut.LoginAsync(
                new LoginRequest("nobody@test.local", "whatever"),
                false,
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ThrowsUnauthorized()
    {
        TestDataSeeder.AddCustomer(Db, email: "login2@test.local");
        PasswordHasherMock
            .Setup(h =>
                h.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(PasswordVerificationResult.Failed);
        var sut = CreateAuthService();

        await Should.ThrowAsync<UnauthorizedAppException>(() =>
            sut.LoginAsync(
                new LoginRequest("login2@test.local", "wrong"),
                false,
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task LoginAsync_InactiveAccount_ThrowsForbidden()
    {
        TestDataSeeder.AddCustomer(Db, email: "inactive@test.local", isActive: false);
        PasswordHasherMock
            .Setup(h =>
                h.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(PasswordVerificationResult.Success);
        var sut = CreateAuthService();

        await Should.ThrowAsync<ForbiddenAppException>(() =>
            sut.LoginAsync(
                new LoginRequest("inactive@test.local", "whatever"),
                false,
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task LoginAsync_UnconfirmedEmail_ThrowsForbidden()
    {
        TestDataSeeder.AddCustomer(Db, email: "unconfirmed@test.local", emailConfirmed: false);
        PasswordHasherMock
            .Setup(h =>
                h.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(PasswordVerificationResult.Success);
        var sut = CreateAuthService();

        await Should.ThrowAsync<ForbiddenAppException>(() =>
            sut.LoginAsync(
                new LoginRequest("unconfirmed@test.local", "whatever"),
                false,
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task LoginAsync_AdminUsingCustomerLoginEndpoint_ThrowsForbidden()
    {
        var admin = TestDataSeeder.AddCustomer(Db, email: "admin@test.local");
        admin.RoleId = TestDataSeeder.AdminRoleId;
        await Db.SaveChangesAsync();
        PasswordHasherMock
            .Setup(h =>
                h.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(PasswordVerificationResult.Success);
        var sut = CreateAuthService();

        await Should.ThrowAsync<ForbiddenAppException>(() =>
            sut.LoginAsync(
                new LoginRequest("admin@test.local", "whatever"),
                adminOnly: false,
                CancellationToken.None
            )
        );
    }

    private void AllowPassword() =>
        PasswordHasherMock
            .Setup(h =>
                h.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(PasswordVerificationResult.Success);

    [Fact]
    public async Task LoginAsync_AdminPortalWithCustomerAccount_ThrowsForbidden()
    {
        TestDataSeeder.AddCustomer(Db, email: "cust@test.local");
        AllowPassword();

        await Should.ThrowAsync<ForbiddenAppException>(() =>
            CreateAuthService()
                .LoginAsync(
                    new LoginRequest("cust@test.local", "pw"),
                    adminOnly: true,
                    CancellationToken.None
                )
        );
        (await Db.RefreshTokens.AnyAsync()).ShouldBeFalse();
    }

    [Fact]
    public async Task LoginAsync_AdminPortalWithUnknownRole_ThrowsForbidden()
    {
        var user = TestDataSeeder.AddCustomer(Db, email: "odd@test.local");
        var oddRole = new Role { RoleName = "Auditor" };
        Db.Roles.Add(oddRole);
        await Db.SaveChangesAsync();
        user.RoleId = oddRole.RoleId;
        await Db.SaveChangesAsync();
        AllowPassword();

        await Should.ThrowAsync<ForbiddenAppException>(() =>
            CreateAuthService()
                .LoginAsync(
                    new LoginRequest("odd@test.local", "pw"),
                    adminOnly: true,
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task LoginAsync_AdminPortalWithAdmin_Succeeds()
    {
        var (admin, _) = TestDataSeeder.AddEmployee(Db, email: "boss@test.local");
        AllowPassword();

        var response = await CreateAuthService()
            .LoginAsync(
                new LoginRequest("boss@test.local", "pw"),
                adminOnly: true,
                CancellationToken.None
            );

        response.User.UserId.ShouldBe(admin.UserId);
        response.User.Role.ShouldBe(Roles.Admin);
    }

    [Fact]
    public async Task LoginAsync_AdminPortalWithDeactivatedEmployeeProfile_ThrowsForbidden()
    {
        var (user, employee) = TestDataSeeder.AddEmployee(Db, email: "staff@test.local");
        var employeeRole = await Db.Roles.SingleAsync(r => r.RoleName == Roles.Employee);
        user.RoleId = employeeRole.RoleId;
        employee.IsActive = false;
        await Db.SaveChangesAsync();
        AllowPassword();

        await Should.ThrowAsync<ForbiddenAppException>(() =>
            CreateAuthService()
                .LoginAsync(
                    new LoginRequest("staff@test.local", "pw"),
                    adminOnly: true,
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task LoginAsync_RehashNeeded_TransparentlyUpgradesStoredHash()
    {
        var user = TestDataSeeder.AddCustomer(Db, email: "rehash@test.local");
        PasswordHasherMock
            .Setup(h =>
                h.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(PasswordVerificationResult.SuccessRehashNeeded);
        PasswordHasherMock
            .Setup(h => h.HashPassword(It.IsAny<User>(), "correct-password"))
            .Returns("new-stronger-hash");
        var sut = CreateAuthService();

        await sut.LoginAsync(
            new LoginRequest("rehash@test.local", "correct-password"),
            false,
            CancellationToken.None
        );

        var reloaded = await Db.Users.SingleAsync(u => u.UserId == user.UserId);
        reloaded.PasswordHash.ShouldBe("new-stronger-hash");
    }
}

public class AuthServiceRefreshAndLogoutTests(PostgresContainerFixture fixture) : AuthServiceTestBase(fixture)
{
    private async Task<(User User, RefreshToken Token)> SeedActiveRefreshTokenAsync()
    {
        var user = TestDataSeeder.AddCustomer(Db);
        var token = new RefreshToken
        {
            UserId = user.UserId,
            TokenHash = "hash:refresh-raw",
            ExpiresAt = DateTime.UtcNow.AddDays(10),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
        };
        Db.RefreshTokens.Add(token);
        await Db.SaveChangesAsync();
        return (user, token);
    }

    [Fact]
    public async Task RefreshAsync_ValidToken_IssuesNewPairAndRevokesOldOne()
    {
        var (user, oldToken) = await SeedActiveRefreshTokenAsync();
        TokenServiceMock.Setup(t => t.GenerateOpaqueToken()).Returns("new-refresh-raw");
        TokenServiceMock
            .Setup(t => t.HashOpaqueToken("new-refresh-raw"))
            .Returns("hash:new-refresh-raw");
        var sut = CreateAuthService();

        var response = await sut.RefreshAsync(
            new RefreshRequest("refresh-raw"),
            CancellationToken.None
        );

        response.RefreshToken.ShouldBe("new-refresh-raw");
        var reloadedOld = await Db.RefreshTokens.SingleAsync(rt =>
            rt.RefreshTokenId == oldToken.RefreshTokenId
        );
        reloadedOld.IsRevoked.ShouldBeTrue();
        reloadedOld.ReplacedByTokenId.ShouldNotBeNull();
        (
            await Db.RefreshTokens.CountAsync(rt => rt.UserId == user.UserId && !rt.IsRevoked)
        ).ShouldBe(1);
    }

    [Fact]
    public async Task RefreshAsync_UnknownToken_ThrowsUnauthorized()
    {
        var sut = CreateAuthService();

        await Should.ThrowAsync<UnauthorizedAppException>(() =>
            sut.RefreshAsync(new RefreshRequest("nonexistent"), CancellationToken.None)
        );
    }

    [Fact]
    public async Task RefreshAsync_RevokedToken_ThrowsUnauthorized()
    {
        var (_, token) = await SeedActiveRefreshTokenAsync();
        token.IsRevoked = true;
        await Db.SaveChangesAsync();
        var sut = CreateAuthService();

        await Should.ThrowAsync<UnauthorizedAppException>(() =>
            sut.RefreshAsync(new RefreshRequest("refresh-raw"), CancellationToken.None)
        );
    }

    [Fact]
    public async Task RefreshAsync_ReusingRotatedToken_RevokesWholeTokenChain()
    {
        var (user, oldToken) = await SeedActiveRefreshTokenAsync();
        var second = new RefreshToken
        {
            UserId = user.UserId,
            TokenHash = "hash:second",
            ExpiresAt = DateTime.UtcNow.AddDays(10),
            CreatedAt = DateTime.UtcNow,
        };
        Db.RefreshTokens.Add(second);
        await Db.SaveChangesAsync();
        var third = new RefreshToken
        {
            UserId = user.UserId,
            TokenHash = "hash:third",
            ExpiresAt = DateTime.UtcNow.AddDays(10),
            CreatedAt = DateTime.UtcNow,
        };
        Db.RefreshTokens.Add(third);
        await Db.SaveChangesAsync();
        oldToken.IsRevoked = true;
        oldToken.ReplacedByTokenId = second.RefreshTokenId;
        second.IsRevoked = true;
        second.ReplacedByTokenId = third.RefreshTokenId;
        await Db.SaveChangesAsync();
        var sut = CreateAuthService();

        await Should.ThrowAsync<UnauthorizedAppException>(() =>
            sut.RefreshAsync(new RefreshRequest("refresh-raw"), CancellationToken.None)
        );

        var reloadedThird = await Db
            .RefreshTokens.AsNoTracking()
            .SingleAsync(rt => rt.RefreshTokenId == third.RefreshTokenId);
        reloadedThird.IsRevoked.ShouldBeTrue();
        reloadedThird.RevokedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task RefreshAsync_ReusingLoggedOutToken_DoesNotRevokeOtherSessions()
    {
        var (user, token) = await SeedActiveRefreshTokenAsync();
        var other = new RefreshToken
        {
            UserId = user.UserId,
            TokenHash = "hash:other-session",
            ExpiresAt = DateTime.UtcNow.AddDays(10),
            CreatedAt = DateTime.UtcNow,
        };
        Db.RefreshTokens.Add(other);
        token.IsRevoked = true;
        await Db.SaveChangesAsync();
        var sut = CreateAuthService();

        await Should.ThrowAsync<UnauthorizedAppException>(() =>
            sut.RefreshAsync(new RefreshRequest("refresh-raw"), CancellationToken.None)
        );

        (
            await Db
                .RefreshTokens.AsNoTracking()
                .SingleAsync(rt => rt.RefreshTokenId == other.RefreshTokenId)
        ).IsRevoked.ShouldBeFalse();
    }

    [Fact]
    public async Task RefreshAsync_ExpiredToken_ThrowsUnauthorized()
    {
        var (_, token) = await SeedActiveRefreshTokenAsync();
        token.ExpiresAt = DateTime.UtcNow.AddDays(-1);
        await Db.SaveChangesAsync();
        var sut = CreateAuthService();

        await Should.ThrowAsync<UnauthorizedAppException>(() =>
            sut.RefreshAsync(new RefreshRequest("refresh-raw"), CancellationToken.None)
        );
    }

    [Fact]
    public async Task LogoutAsync_ActiveToken_RevokesIt()
    {
        var (_, token) = await SeedActiveRefreshTokenAsync();
        var sut = CreateAuthService();

        await sut.LogoutAsync(new LogoutRequest("refresh-raw"), CancellationToken.None);

        var reloaded = await Db.RefreshTokens.SingleAsync(rt =>
            rt.RefreshTokenId == token.RefreshTokenId
        );
        reloaded.IsRevoked.ShouldBeTrue();
    }

    [Fact]
    public async Task LogoutAsync_UnknownToken_StillReturnsSuccessMessage()
    {
        var sut = CreateAuthService();

        var result = await sut.LogoutAsync(
            new LogoutRequest("never-issued"),
            CancellationToken.None
        );

        result.Message.ShouldNotBeNullOrWhiteSpace();
    }
}

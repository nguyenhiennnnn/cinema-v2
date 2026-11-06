using Cinema.Application.DTOs.Auth;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class AuthServiceRegisterAndConfirmTests : AuthServiceTestBase
{
    [Fact]
    public async Task RegisterAsync_NewEmail_CreatesUserAndCustomerProfileAndQueuesVerificationEmail()
    {
        var sut = CreateSut();
        var request = new RegisterRequest("Nguyen Van A", "new@test.local", "Str0ngPass", null);

        var response = await sut.RegisterAsync(request, CancellationToken.None);

        response.Email.ShouldBe("new@test.local");
        var user = await Db.Users.SingleAsync(u => u.UserId == response.UserId);
        user.RoleId.ShouldBe(TestDataSeeder.CustomerRoleId);
        user.EmailConfirmed.ShouldBeFalse();
        (await Db.CustomerProfiles.AnyAsync(p => p.UserId == user.UserId)).ShouldBeTrue();
        (
            await Db.UserVerificationTokens.AnyAsync(t =>
                t.UserId == user.UserId && t.Purpose == VerificationTokenPurpose.EmailConfirmation
            )
        ).ShouldBeTrue();
        BackgroundJobClientMock.Verify(
            c => c.Create(It.IsAny<Job>(), It.IsAny<IState>()),
            Times.Once
        );
    }

    [Fact]
    public async Task RegisterAsync_EmailAlreadyExists_ThrowsValidationAppException()
    {
        TestDataSeeder.AddCustomer(Db, email: "existing@test.local");
        var sut = CreateSut();
        var request = new RegisterRequest(
            "Nguyen Van A",
            "existing@test.local",
            "Str0ngPass",
            null
        );

        var ex = await Should.ThrowAsync<ValidationAppException>(() =>
            sut.RegisterAsync(request, CancellationToken.None)
        );
        ex.Errors!.ShouldContainKey("email");
    }

    [Fact]
    public async Task ConfirmEmailAsync_ValidToken_ConfirmsEmailAndMarksTokenUsed()
    {
        var user = TestDataSeeder.AddCustomer(Db, emailConfirmed: false);
        TokenServiceMock.Setup(t => t.HashOpaqueToken("raw-token")).Returns("hashed-token");
        Db.UserVerificationTokens.Add(
            new UserVerificationToken
            {
                UserId = user.UserId,
                TokenHash = "hashed-token",
                Purpose = VerificationTokenPurpose.EmailConfirmation,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                Used = false,
                CreatedAt = DateTime.UtcNow,
            }
        );
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await sut.ConfirmEmailAsync(new ConfirmEmailRequest("raw-token"), CancellationToken.None);

        var reloadedUser = await Db.Users.SingleAsync(u => u.UserId == user.UserId);
        reloadedUser.EmailConfirmed.ShouldBeTrue();
        var reloadedToken = await Db.UserVerificationTokens.SingleAsync(t =>
            t.UserId == user.UserId
        );
        reloadedToken.Used.ShouldBeTrue();
    }

    [Fact]
    public async Task ConfirmEmailAsync_ExpiredToken_ThrowsValidationAppException()
    {
        var user = TestDataSeeder.AddCustomer(Db, emailConfirmed: false);
        TokenServiceMock.Setup(t => t.HashOpaqueToken("raw-token")).Returns("hashed-token");
        Db.UserVerificationTokens.Add(
            new UserVerificationToken
            {
                UserId = user.UserId,
                TokenHash = "hashed-token",
                Purpose = VerificationTokenPurpose.EmailConfirmation,
                ExpiresAt = DateTime.UtcNow.AddHours(-1),
                Used = false,
                CreatedAt = DateTime.UtcNow.AddHours(-2),
            }
        );
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await Should.ThrowAsync<ValidationAppException>(() =>
            sut.ConfirmEmailAsync(new ConfirmEmailRequest("raw-token"), CancellationToken.None)
        );
    }

    [Fact]
    public async Task ConfirmEmailAsync_AlreadyUsedToken_ThrowsValidationAppException()
    {
        var user = TestDataSeeder.AddCustomer(Db, emailConfirmed: false);
        TokenServiceMock.Setup(t => t.HashOpaqueToken("raw-token")).Returns("hashed-token");
        Db.UserVerificationTokens.Add(
            new UserVerificationToken
            {
                UserId = user.UserId,
                TokenHash = "hashed-token",
                Purpose = VerificationTokenPurpose.EmailConfirmation,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                Used = true, 
                CreatedAt = DateTime.UtcNow,
            }
        );
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await Should.ThrowAsync<ValidationAppException>(() =>
            sut.ConfirmEmailAsync(new ConfirmEmailRequest("raw-token"), CancellationToken.None)
        );
    }
}

public class AuthServiceLoginTests : AuthServiceTestBase
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
        var sut = CreateSut();

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
        var sut = CreateSut();

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
        var sut = CreateSut();

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
        var sut = CreateSut();

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
        var sut = CreateSut();

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
        var sut = CreateSut();

        // adminOnly: false == the regular customer-facing /auth/login endpoint.
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
            CreateSut()
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
            CreateSut()
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

        var response = await CreateSut()
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
            CreateSut()
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
        var sut = CreateSut();

        await sut.LoginAsync(
            new LoginRequest("rehash@test.local", "correct-password"),
            false,
            CancellationToken.None
        );

        var reloaded = await Db.Users.SingleAsync(u => u.UserId == user.UserId);
        reloaded.PasswordHash.ShouldBe("new-stronger-hash");
    }
}

public class AuthServiceRefreshAndLogoutTests : AuthServiceTestBase
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
        var sut = CreateSut();

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
        var sut = CreateSut();

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
        var sut = CreateSut();

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
        var sut = CreateSut();

        await Should.ThrowAsync<UnauthorizedAppException>(() =>
            sut.RefreshAsync(new RefreshRequest("refresh-raw"), CancellationToken.None)
        );

        var reloadedThird = await Db.RefreshTokens.AsNoTracking().SingleAsync(rt => rt.RefreshTokenId == third.RefreshTokenId);
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
        var sut = CreateSut();

        await Should.ThrowAsync<UnauthorizedAppException>(() =>
            sut.RefreshAsync(new RefreshRequest("refresh-raw"), CancellationToken.None)
        );

        (await Db.RefreshTokens.AsNoTracking().SingleAsync(rt => rt.RefreshTokenId == other.RefreshTokenId))
            .IsRevoked.ShouldBeFalse();
    }

    [Fact]
    public async Task RefreshAsync_ExpiredToken_ThrowsUnauthorized()
    {
        var (_, token) = await SeedActiveRefreshTokenAsync();
        token.ExpiresAt = DateTime.UtcNow.AddDays(-1);
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await Should.ThrowAsync<UnauthorizedAppException>(() =>
            sut.RefreshAsync(new RefreshRequest("refresh-raw"), CancellationToken.None)
        );
    }

    [Fact]
    public async Task LogoutAsync_ActiveToken_RevokesIt()
    {
        var (_, token) = await SeedActiveRefreshTokenAsync();
        var sut = CreateSut();

        await sut.LogoutAsync(new LogoutRequest("refresh-raw"), CancellationToken.None);

        var reloaded = await Db.RefreshTokens.SingleAsync(rt =>
            rt.RefreshTokenId == token.RefreshTokenId
        );
        reloaded.IsRevoked.ShouldBeTrue();
    }

    [Fact]
    public async Task LogoutAsync_UnknownToken_StillReturnsSuccessMessage()
    {
        var sut = CreateSut();

        var result = await sut.LogoutAsync(
            new LogoutRequest("never-issued"),
            CancellationToken.None
        );

        result.Message.ShouldNotBeNullOrWhiteSpace();
    }
}

public class AuthServicePasswordTests : AuthServiceTestBase
{
    [Fact]
    public async Task ChangePasswordAsync_WrongCurrentPassword_ThrowsValidationAppException()
    {
        var user = TestDataSeeder.AddCustomer(Db);
        PasswordHasherMock
            .Setup(h =>
                h.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(PasswordVerificationResult.Failed);
        var sut = CreateSut();

        var ex = await Should.ThrowAsync<ValidationAppException>(() =>
            sut.ChangePasswordAsync(
                user.UserId,
                new ChangePasswordRequest("wrong", "Str0ngPass"),
                CancellationToken.None
            )
        );
        ex.Errors!.ShouldContainKey("currentPassword");
    }

    [Fact]
    public async Task ChangePasswordAsync_CorrectCurrentPassword_UpdatesHash()
    {
        var user = TestDataSeeder.AddCustomer(Db);
        PasswordHasherMock
            .Setup(h => h.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), "current"))
            .Returns(PasswordVerificationResult.Success);
        PasswordHasherMock
            .Setup(h => h.HashPassword(It.IsAny<User>(), "NewStr0ngPass"))
            .Returns("new-hash");
        var sut = CreateSut();

        await sut.ChangePasswordAsync(
            user.UserId,
            new ChangePasswordRequest("current", "NewStr0ngPass"),
            CancellationToken.None
        );

        var reloaded = await Db.Users.SingleAsync(u => u.UserId == user.UserId);
        reloaded.PasswordHash.ShouldBe("new-hash");
    }

    [Fact]
    public async Task ForgotPasswordAsync_UnknownEmail_ReturnsStandardMessageWithoutQueueingEmail()
    {
        var sut = CreateSut();

        var result = await sut.ForgotPasswordAsync(
            new ForgotPasswordRequest("nobody@test.local"),
            CancellationToken.None
        );

        result.Message.ShouldNotBeNullOrWhiteSpace();
        BackgroundJobClientMock.Verify(
            c => c.Create(It.IsAny<Job>(), It.IsAny<IState>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ResetPasswordAsync_ValidToken_UpdatesPasswordAndRevokesAllActiveRefreshTokens()
    {
        var user = TestDataSeeder.AddCustomer(Db);
        Db.RefreshTokens.Add(
            new RefreshToken
            {
                UserId = user.UserId,
                TokenHash = "some-active-refresh-hash",
                ExpiresAt = DateTime.UtcNow.AddDays(5),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow,
            }
        );
        TokenServiceMock.Setup(t => t.HashOpaqueToken("reset-raw")).Returns("hashed-reset-token");
        Db.UserVerificationTokens.Add(
            new UserVerificationToken
            {
                UserId = user.UserId,
                TokenHash = "hashed-reset-token",
                Purpose = VerificationTokenPurpose.PasswordReset,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                Used = false,
                CreatedAt = DateTime.UtcNow,
            }
        );
        await Db.SaveChangesAsync();
        PasswordHasherMock
            .Setup(h => h.HashPassword(It.IsAny<User>(), "NewStr0ngPass"))
            .Returns("reset-hash");
        var sut = CreateSut();

        await sut.ResetPasswordAsync(
            new ResetPasswordRequest("reset-raw", "NewStr0ngPass"),
            CancellationToken.None
        );

        var reloadedUser = await Db.Users.SingleAsync(u => u.UserId == user.UserId);
        reloadedUser.PasswordHash.ShouldBe("reset-hash");
        (
            await Db.RefreshTokens.CountAsync(rt => rt.UserId == user.UserId && !rt.IsRevoked)
        ).ShouldBe(0);
    }
}

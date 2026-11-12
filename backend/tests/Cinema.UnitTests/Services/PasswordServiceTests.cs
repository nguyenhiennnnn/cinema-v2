using Cinema.Application.DTOs.Auth;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class PasswordServiceTests(PostgresContainerFixture fixture) : AuthServiceTestBase(fixture), IClassFixture<PostgresContainerFixture>
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
        var sut = CreatePasswordService();

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
        var sut = CreatePasswordService();

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
        var sut = CreatePasswordService();

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
        var sut = CreatePasswordService();

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

    [Fact]
    public async Task ResetPasswordAsync_StaffInviteToken_SetsPasswordAndConsumesToken()
    {
        var user = TestDataSeeder.AddCustomer(Db, "staff@test.local");
        user.PasswordHash = User.PendingPasswordHash;
        TokenServiceMock.Setup(t => t.HashOpaqueToken("invite-raw")).Returns("hashed-invite");
        Db.UserVerificationTokens.Add(
            new UserVerificationToken
            {
                UserId = user.UserId,
                TokenHash = "hashed-invite",
                Purpose = VerificationTokenPurpose.StaffInvite,
                ExpiresAt = DateTime.UtcNow.AddHours(10),
                CreatedAt = DateTime.UtcNow,
            }
        );
        await Db.SaveChangesAsync();
        PasswordHasherMock
            .Setup(h => h.HashPassword(It.IsAny<User>(), "NewStr0ngPass"))
            .Returns("invite-hash");

        await CreatePasswordService()
            .ResetPasswordAsync(
                new ResetPasswordRequest("invite-raw", "NewStr0ngPass"),
                CancellationToken.None
            );

        (
            await Db.Users.AsNoTracking().SingleAsync(u => u.UserId == user.UserId)
        ).PasswordHash.ShouldBe("invite-hash");
        (
            await Db
                .UserVerificationTokens.AsNoTracking()
                .SingleAsync(t => t.TokenHash == "hashed-invite")
        ).Used.ShouldBeTrue();
    }

    [Fact]
    public async Task ResetPasswordAsync_ExpiredStaffInviteToken_Throws()
    {
        var user = TestDataSeeder.AddCustomer(Db, "staff@test.local");
        TokenServiceMock.Setup(t => t.HashOpaqueToken("old-raw")).Returns("hashed-old");
        Db.UserVerificationTokens.Add(
            new UserVerificationToken
            {
                UserId = user.UserId,
                TokenHash = "hashed-old",
                Purpose = VerificationTokenPurpose.StaffInvite,
                ExpiresAt = DateTime.UtcNow.AddMinutes(-1),
                CreatedAt = DateTime.UtcNow.AddHours(-73),
            }
        );
        await Db.SaveChangesAsync();

        await Should.ThrowAsync<ValidationAppException>(() =>
            CreatePasswordService()
                .ResetPasswordAsync(
                    new ResetPasswordRequest("old-raw", "NewStr0ngPass"),
                    CancellationToken.None
                )
        );
    }
}

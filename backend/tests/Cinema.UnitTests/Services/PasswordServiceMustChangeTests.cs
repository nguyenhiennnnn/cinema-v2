using Cinema.Application.DTOs.Auth;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Cinema.UnitTests.TestSupport;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class PasswordServiceMustChangeTests(PostgresContainerFixture fixture)
    : AuthServiceTestBase(fixture),
        IClassFixture<PostgresContainerFixture>
{
    [Fact]
    public async Task ChangePasswordAsync_NewEqualsCurrent_ThrowsAndKeepsFlag()
    {
        var user = TestDataSeeder.AddCustomer(Db);
        user.MustChangePassword = true;
        await Db.SaveChangesAsync();
        PasswordHasherMock
            .Setup(h => h.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), "SamePass1"))
            .Returns(PasswordVerificationResult.Success);

        var ex = await Should.ThrowAsync<ValidationAppException>(() =>
            CreatePasswordService()
                .ChangePasswordAsync(
                    user.UserId,
                    new ChangePasswordRequest("SamePass1", "SamePass1"),
                    CancellationToken.None
                )
        );

        ex.Errors!.ShouldContainKey("newPassword");
        ex.Errors!["newPassword"].ShouldContain(Messages.Invalid_NewPasswordSameAsCurrent);
        (await Db.Users.AsNoTracking().SingleAsync(u => u.UserId == user.UserId))
            .MustChangePassword.ShouldBeTrue();
    }

    [Fact]
    public async Task ChangePasswordAsync_Success_ClearsMustChangeFlag()
    {
        var user = TestDataSeeder.AddCustomer(Db);
        user.MustChangePassword = true;
        await Db.SaveChangesAsync();
        PasswordHasherMock
            .Setup(h => h.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), "OldPass123"))
            .Returns(PasswordVerificationResult.Success);

        await CreatePasswordService()
            .ChangePasswordAsync(
                user.UserId,
                new ChangePasswordRequest("OldPass123", "NewPass456"),
                CancellationToken.None
            );

        (await Db.Users.AsNoTracking().SingleAsync(u => u.UserId == user.UserId))
            .MustChangePassword.ShouldBeFalse();
    }

    [Fact]
    public async Task ResetPasswordAsync_ValidLink_ClearsMustChangeFlag()
    {
        var user = TestDataSeeder.AddCustomer(Db, "flag@test.local");
        user.MustChangePassword = true;
        TokenServiceMock.Setup(t => t.HashOpaqueToken("raw")).Returns("hashed-raw");
        Db.UserVerificationTokens.Add(
            new UserVerificationToken
            {
                UserId = user.UserId,
                TokenHash = "hashed-raw",
                Purpose = VerificationTokenPurpose.PasswordReset,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                CreatedAt = DateTime.UtcNow,
            }
        );
        await Db.SaveChangesAsync();

        await CreatePasswordService()
            .ResetPasswordAsync(new ResetPasswordRequest("raw", "NewPass456"), CancellationToken.None);

        (await Db.Users.AsNoTracking().SingleAsync(u => u.UserId == user.UserId))
            .MustChangePassword.ShouldBeFalse();
    }

    [Fact]
    public async Task SendStaffInviteAsync_LinkPointsToAdminResetPasswordPage()
    {
        var user = TestDataSeeder.AddCustomer(Db, "invite@test.local");
        Job? captured = null;
        BackgroundJobClientMock
            .Setup(c => c.Create(It.IsAny<Job>(), It.IsAny<IState>()))
            .Callback<Job, IState>((job, _) => captured = job)
            .Returns("job-1");

        await CreateRegistrationService().SendStaffInviteAsync(user, CancellationToken.None);

        captured.ShouldNotBeNull();
        var text = string.Join("\n", captured.Args.OfType<string>());
        text.ShouldContain("/admin/reset-password?token=");
        text.ShouldNotContain("/auth/reset-password");
    }

    [Fact]
    public async Task ForgotPasswordAsync_LinkStillPointsToCustomerResetPasswordPage()
    {
        var user = TestDataSeeder.AddCustomer(Db, "forgot@test.local");
        Job? captured = null;
        BackgroundJobClientMock
            .Setup(c => c.Create(It.IsAny<Job>(), It.IsAny<IState>()))
            .Callback<Job, IState>((job, _) => captured = job)
            .Returns("job-1");

        await CreatePasswordService()
            .ForgotPasswordAsync(new ForgotPasswordRequest(user.Email), CancellationToken.None);

        captured.ShouldNotBeNull();
        string.Join("\n", captured.Args.OfType<string>())
            .ShouldContain("/auth/reset-password?token=");
    }
}

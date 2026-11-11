using Cinema.Application.DTOs.Auth;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class RegistrationServiceRegisterAndConfirmTests : AuthServiceTestBase
{
    [Fact]
    public async Task RegisterAsync_NewEmail_CreatesUserAndCustomerProfileAndQueuesVerificationEmail()
    {
        var sut = CreateRegistrationService();
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
        var sut = CreateRegistrationService();
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
        var sut = CreateRegistrationService();

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
        var sut = CreateRegistrationService();

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
        var sut = CreateRegistrationService();

        await Should.ThrowAsync<ValidationAppException>(() =>
            sut.ConfirmEmailAsync(new ConfirmEmailRequest("raw-token"), CancellationToken.None)
        );
    }
}

public class RegistrationServiceStaffInviteTests : AuthServiceTestBase
{
    private static readonly TimeSpan InviteValidity = TimeSpan.FromHours(72);

    [Fact]
    public async Task SendStaffInviteAsync_IssuesStaffInviteTokenValidFor72HoursAndQueuesEmail()
    {
        var user = TestDataSeeder.AddCustomer(Db, "staff@test.local");
        var sut = CreateRegistrationService();

        await sut.SendStaffInviteAsync(user, CancellationToken.None);

        var token = await Db.UserVerificationTokens.SingleAsync(t => t.UserId == user.UserId);
        token.Purpose.ShouldBe(VerificationTokenPurpose.StaffInvite);
        token.Used.ShouldBeFalse();
        (token.ExpiresAt - DateTime.UtcNow).ShouldBeInRange(
            InviteValidity - TimeSpan.FromMinutes(1),
            InviteValidity
        );
        BackgroundJobClientMock.Verify(
            c => c.Create(It.IsAny<Job>(), It.IsAny<IState>()),
            Times.Once
        );
    }

    [Fact]
    public async Task SendStaffInviteAsync_Resend_InvalidatesPreviousInviteTokens()
    {
        var user = TestDataSeeder.AddCustomer(Db, "staff@test.local");

        await CreateRegistrationService().SendStaffInviteAsync(user, CancellationToken.None);
        await CreateRegistrationService().SendStaffInviteAsync(user, CancellationToken.None);

        var tokens = await Db.UserVerificationTokens.Where(t => t.UserId == user.UserId).ToListAsync();
        tokens.Count.ShouldBe(2);
        tokens.Count(t => !t.Used).ShouldBe(1);
    }
}

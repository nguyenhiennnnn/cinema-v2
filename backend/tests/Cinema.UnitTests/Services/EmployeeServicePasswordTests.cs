using Cinema.Application.DTOs.Employees;
using Cinema.Application.Interfaces;
using Cinema.Application.Services.Employees;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Cinema.UnitTests.TestSupport;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class EmployeeServicePasswordTests : IDisposable, IClassFixture<PostgresContainerFixture>
{
    private const int EmployeeRoleId = 3;
    private const string TempPassword = "Temp1234";

    private readonly PostgresAppDbContextHandle _handle;
    private readonly PasswordHasher<User> _hasher = new();

    static EmployeeServicePasswordTests()
    {
        Mapster.TypeAdapterConfig.GlobalSettings.Scan(
            typeof(Cinema.Application.MappingConfig.MapsterConfig).Assembly
        );
    }

    public EmployeeServicePasswordTests(PostgresContainerFixture fixture)
    {
        _handle = new(fixture.MasterConnectionString);
        _handle.Db.UserVerificationTokens.RemoveRange(_handle.Db.UserVerificationTokens);
        _handle.Db.Employees.RemoveRange(_handle.Db.Employees);
        _handle.Db.Users.RemoveRange(_handle.Db.Users);
        _handle.Db.SaveChanges();
        TestDataSeeder.SetCinemaSettings(_handle.Db);
        var settings = _handle.Db.CinemaSettings.Single(s => s.Id == 1);
        settings.DefaultEmployeePassword = "Default123";
        _handle.Db.SaveChanges();
    }

    private EmployeeService CreateSut()
    {
        _handle.Db.ChangeTracker.Clear();
        return new EmployeeService(
            _handle.Db,
            Mock.Of<IRegistrationService>(),
            _hasher,
            NullLogger<EmployeeService>.Instance
        );
    }

    private static CreateEmployeeDirectRequest Direct(string email = "direct@test.local") =>
        new("Nguyen Van B", email, null, null, new DateOnly(2026, 11, 1), TempPassword);

    private (User User, Employee Employee) AddStaff(
        string email,
        int roleId = EmployeeRoleId,
        bool employeeActive = true,
        bool userActive = true,
        string? passwordHash = null
    )
    {
        var user = new User
        {
            FullName = "Staff",
            Email = email,
            PasswordHash = passwordHash ?? "old-hash",
            RoleId = roleId,
            IsActive = userActive,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
        };
        _handle.Db.Users.Add(user);
        _handle.Db.SaveChanges();
        var employee = new Employee { UserId = user.UserId, IsActive = employeeActive };
        _handle.Db.Employees.Add(employee);
        _handle.Db.SaveChanges();
        return (user, employee);
    }

    [Fact]
    public async Task CreateEmployeeDirectAsync_CreatesUsableAccountWithMustChangeFlag()
    {
        var result = await CreateSut().CreateEmployeeDirectAsync(Direct(), CancellationToken.None);

        var user = await _handle.Db.Users.AsNoTracking().SingleAsync(u => u.Email == "direct@test.local");
        user.RoleId.ShouldBe(EmployeeRoleId);
        user.EmailConfirmed.ShouldBeTrue();
        user.IsActive.ShouldBeTrue();
        user.MustChangePassword.ShouldBeTrue();
        user.PasswordHash.ShouldNotBe(User.PendingPasswordHash);
        _hasher
            .VerifyHashedPassword(user, user.PasswordHash, TempPassword)
            .ShouldNotBe(PasswordVerificationResult.Failed);
        result.MustChangePassword.ShouldBeTrue();
        result.IsPendingActivation.ShouldBeFalse();
        (await _handle.Db.Employees.AnyAsync(e => e.UserId == user.UserId)).ShouldBeTrue();
    }

    [Fact]
    public async Task CreateEmployeeDirectAsync_DuplicateEmailOfCustomer_Throws()
    {
        TestDataSeeder.AddCustomer(_handle.Db, "dup@test.local");

        await Should.ThrowAsync<ValidationAppException>(() =>
            CreateSut().CreateEmployeeDirectAsync(Direct("dup@test.local"), CancellationToken.None)
        );

        (await _handle.Db.Employees.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task CreateEmployeeDirectAsync_UnknownPosition_Throws()
    {
        await Should.ThrowAsync<ValidationAppException>(() =>
            CreateSut()
                .CreateEmployeeDirectAsync(Direct() with { PositionId = 9999 }, CancellationToken.None)
        );

        (await _handle.Db.Users.AnyAsync(u => u.Email == "direct@test.local")).ShouldBeFalse();
    }

    [Fact]
    public async Task ResetEmployeePasswordAsync_SetsDefaultPasswordFlagRevokesSessionsAndLinks()
    {
        var admin = AddStaff("admin@test.local", roleId: TestDataSeeder.AdminRoleId);
        var (user, employee) = AddStaff("emp@test.local");
        _handle.Db.RefreshTokens.Add(
            new RefreshToken
            {
                UserId = user.UserId,
                TokenHash = "rt-1",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                CreatedAt = DateTime.UtcNow,
            }
        );
        _handle.Db.UserVerificationTokens.AddRange(
            NewToken(user.UserId, "t-invite", VerificationTokenPurpose.StaffInvite),
            NewToken(user.UserId, "t-forgot", VerificationTokenPurpose.PasswordReset),
            NewToken(user.UserId, "t-confirm", VerificationTokenPurpose.EmailConfirmation)
        );
        _handle.Db.SaveChanges();

        var result = await CreateSut()
            .ResetEmployeePasswordAsync(employee.EmployeeId, admin.User.UserId, CancellationToken.None);

        _handle.Db.ChangeTracker.Clear();
        var reloaded = await _handle.Db.Users.AsNoTracking().SingleAsync(u => u.UserId == user.UserId);
        reloaded.MustChangePassword.ShouldBeTrue();
        _hasher
            .VerifyHashedPassword(reloaded, reloaded.PasswordHash, "Default123")
            .ShouldNotBe(PasswordVerificationResult.Failed);
        result.MustChangePassword.ShouldBeTrue();
        (await _handle.Db.RefreshTokens.AllAsync(t => t.UserId != user.UserId || t.IsRevoked))
            .ShouldBeTrue();
        var tokens = await _handle
            .Db.UserVerificationTokens.AsNoTracking()
            .Where(t => t.UserId == user.UserId)
            .ToListAsync();
        tokens
            .Where(t => t.Purpose != VerificationTokenPurpose.EmailConfirmation)
            .ShouldAllBe(t => t.Used);
        tokens
            .Single(t => t.Purpose == VerificationTokenPurpose.EmailConfirmation)
            .Used.ShouldBeFalse();
    }

    [Fact]
    public async Task ResetEmployeePasswordAsync_PendingInvite_BecomesActivated()
    {
        var admin = AddStaff("admin@test.local", roleId: TestDataSeeder.AdminRoleId);
        var (_, employee) = AddStaff("pending@test.local", passwordHash: User.PendingPasswordHash);

        var result = await CreateSut()
            .ResetEmployeePasswordAsync(employee.EmployeeId, admin.User.UserId, CancellationToken.None);

        result.IsPendingActivation.ShouldBeFalse();
    }

    [Fact]
    public async Task ResetEmployeePasswordAsync_ChangingConfigDoesNotAffectEarlierReset()
    {
        var admin = AddStaff("admin@test.local", roleId: TestDataSeeder.AdminRoleId);
        var (user, employee) = AddStaff("emp@test.local");
        await CreateSut()
            .ResetEmployeePasswordAsync(employee.EmployeeId, admin.User.UserId, CancellationToken.None);

        _handle.Db.ChangeTracker.Clear();
        _handle.Db.CinemaSettings.Single(s => s.Id == 1).DefaultEmployeePassword = "Other4567";
        _handle.Db.SaveChanges();

        _handle.Db.ChangeTracker.Clear();
        var reloaded = await _handle.Db.Users.AsNoTracking().SingleAsync(u => u.UserId == user.UserId);
        _hasher
            .VerifyHashedPassword(reloaded, reloaded.PasswordHash, "Default123")
            .ShouldNotBe(PasswordVerificationResult.Failed);
    }

    [Fact]
    public async Task ResetEmployeePasswordAsync_Self_Throws()
    {
        var (user, employee) = AddStaff("self@test.local", roleId: TestDataSeeder.AdminRoleId);

        var ex = await Should.ThrowAsync<BusinessRuleException>(() =>
            CreateSut()
                .ResetEmployeePasswordAsync(employee.EmployeeId, user.UserId, CancellationToken.None)
        );

        ex.Message.ShouldBe(Messages.Rule_CannotResetOwnPassword);
    }

    [Fact]
    public async Task ResetEmployeePasswordAsync_OtherAdmin_Throws()
    {
        var caller = AddStaff("admin1@test.local", roleId: TestDataSeeder.AdminRoleId);
        var (_, otherAdmin) = AddStaff("admin2@test.local", roleId: TestDataSeeder.AdminRoleId);

        var ex = await Should.ThrowAsync<BusinessRuleException>(() =>
            CreateSut()
                .ResetEmployeePasswordAsync(otherAdmin.EmployeeId, caller.User.UserId, CancellationToken.None)
        );

        ex.Message.ShouldBe(Messages.Rule_OnlyEmployeePasswordCanBeReset);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task ResetEmployeePasswordAsync_DisabledEmployee_Throws(
        bool employeeActive,
        bool userActive
    )
    {
        var caller = AddStaff("admin@test.local", roleId: TestDataSeeder.AdminRoleId);
        var (_, employee) = AddStaff(
            "off@test.local",
            employeeActive: employeeActive,
            userActive: userActive
        );

        var ex = await Should.ThrowAsync<BusinessRuleException>(() =>
            CreateSut()
                .ResetEmployeePasswordAsync(employee.EmployeeId, caller.User.UserId, CancellationToken.None)
        );

        ex.Message.ShouldBe(Messages.Rule_CannotResetInactiveEmployeePassword);
    }

    private static UserVerificationToken NewToken(int userId, string hash, VerificationTokenPurpose purpose) =>
        new()
        {
            UserId = userId,
            TokenHash = hash,
            Purpose = purpose,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            CreatedAt = DateTime.UtcNow,
        };

    public void Dispose() => _handle.Dispose();
}

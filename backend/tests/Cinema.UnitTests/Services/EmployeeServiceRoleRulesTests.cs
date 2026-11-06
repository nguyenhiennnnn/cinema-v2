using Cinema.Application.DTOs.Employees;
using Cinema.Application.Services.Employees;
using Cinema.Domain.Entities;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class EmployeeServiceRoleRulesTests : IDisposable
{
    private const int EmployeeRoleId = 3; // seeded by SeedData.Apply

    private readonly SqliteAppDbContextHandle _handle = new();

    public EmployeeServiceRoleRulesTests()
    {
        _handle.Db.Employees.RemoveRange(_handle.Db.Employees);
        _handle.Db.Users.RemoveRange(_handle.Db.Users);
        _handle.Db.SaveChanges();
    }

    private EmployeeService CreateSut()
    {
        _handle.Db.ChangeTracker.Clear();
        return new EmployeeService(_handle.Db);
    }

    private User AddAdmin(string email)
    {
        var (user, _) = TestDataSeeder.AddEmployee(_handle.Db, email);
        return user;
    }

    [Fact]
    public async Task UpdateUserRoleAsync_ChangingOwnRole_Throws()
    {
        var admin = AddAdmin("a1@test.local");
        AddAdmin("a2@test.local");

        var ex = await Should.ThrowAsync<BusinessRuleException>(() =>
            CreateSut().UpdateUserRoleAsync(
                admin.UserId,
                new UpdateUserRoleRequest(EmployeeRoleId),
                currentUserId: admin.UserId,
                CancellationToken.None
            )
        );

        ex.Message.ShouldBe(Messages.Rule_CannotChangeOwnRole);
        (await _handle.Db.Users.AsNoTracking().FirstAsync(u => u.UserId == admin.UserId))
            .RoleId.ShouldBe(TestDataSeeder.AdminRoleId);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_DemotingLastActiveAdmin_Throws()
    {
        var onlyAdmin = AddAdmin("only@test.local");
        var caller = TestDataSeeder.AddCustomer(_handle.Db, "caller@test.local");

        var ex = await Should.ThrowAsync<BusinessRuleException>(() =>
            CreateSut().UpdateUserRoleAsync(
                onlyAdmin.UserId,
                new UpdateUserRoleRequest(EmployeeRoleId),
                currentUserId: caller.UserId,
                CancellationToken.None
            )
        );

        ex.Message.ShouldBe(Messages.Rule_CannotDemoteLastAdmin);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_DemotingAdminWhenAnotherActiveAdminExists_Succeeds()
    {
        var target = AddAdmin("target@test.local");
        var caller = AddAdmin("caller@test.local");

        var result = await CreateSut().UpdateUserRoleAsync(
            target.UserId,
            new UpdateUserRoleRequest(EmployeeRoleId),
            currentUserId: caller.UserId,
            CancellationToken.None
        );

        result.RoleId.ShouldBe(EmployeeRoleId);
    }

    [Fact]
    public async Task UpdateUserStatusAsync_LockingSelf_Throws()
    {
        var admin = AddAdmin("a1@test.local");
        AddAdmin("a2@test.local");

        var ex = await Should.ThrowAsync<BusinessRuleException>(() =>
            CreateSut().UpdateUserStatusAsync(
                admin.UserId,
                new UpdateUserStatusRequest(false),
                currentUserId: admin.UserId,
                CancellationToken.None
            )
        );

        ex.Message.ShouldBe(Messages.Rule_CannotLockSelf);
    }

    [Fact]
    public async Task UpdateUserStatusAsync_LockingLastActiveAdmin_Throws()
    {
        var onlyAdmin = AddAdmin("only@test.local");
        var caller = TestDataSeeder.AddCustomer(_handle.Db, "caller@test.local");

        var ex = await Should.ThrowAsync<BusinessRuleException>(() =>
            CreateSut().UpdateUserStatusAsync(
                onlyAdmin.UserId,
                new UpdateUserStatusRequest(false),
                currentUserId: caller.UserId,
                CancellationToken.None
            )
        );

        ex.Message.ShouldBe(Messages.Rule_CannotLockLastAdmin);
    }

    [Fact]
    public async Task UpdateEmployeeAsync_DeactivatingOwnProfile_Throws()
    {
        var (user, employee) = TestDataSeeder.AddEmployee(_handle.Db, "me@test.local");

        var ex = await Should.ThrowAsync<BusinessRuleException>(() =>
            CreateSut().UpdateEmployeeAsync(
                employee.EmployeeId,
                new UpdateEmployeeRequest(null, null, IsActive: false),
                currentUserId: user.UserId,
                CancellationToken.None
            )
        );

        ex.Message.ShouldBe(Messages.Rule_CannotDeactivateSelfEmployee);
    }

    public void Dispose() => _handle.Dispose();
}

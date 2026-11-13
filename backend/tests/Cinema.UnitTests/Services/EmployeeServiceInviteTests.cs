using Cinema.Application.DTOs.Employees;
using Cinema.Application.Interfaces;
using Cinema.Application.Services.Employees;
using Cinema.Domain.Entities;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class EmployeeServiceInviteTests : IDisposable, IClassFixture<PostgresContainerFixture>
{
    private const int EmployeeRoleId = 3;

    private readonly PostgresAppDbContextHandle _handle;
    private readonly Mock<IRegistrationService> _registrationMock = new();

    static EmployeeServiceInviteTests()
    {
        Mapster.TypeAdapterConfig.GlobalSettings.Scan(
            typeof(Cinema.Application.MappingConfig.MapsterConfig).Assembly
        );
    }

    public EmployeeServiceInviteTests(PostgresContainerFixture fixture)
    {
        _handle = new(fixture.MasterConnectionString);
        _handle.Db.Employees.RemoveRange(_handle.Db.Employees);
        _handle.Db.Users.RemoveRange(_handle.Db.Users);
        _handle.Db.SaveChanges();
    }

    private EmployeeService CreateSut()
    {
        _handle.Db.ChangeTracker.Clear();
        return new EmployeeService(_handle.Db, _registrationMock.Object);
    }

    private UserAdminService CreateUserAdminSut()
    {
        _handle.Db.ChangeTracker.Clear();
        return new UserAdminService(_handle.Db);
    }

    private static InviteEmployeeRequest Invite(string email = "new.staff@test.local") =>
        new("Nguyen Van A", email, "0901234567", null, new DateOnly(2026, 11, 1));

    [Fact]
    public async Task InviteEmployeeAsync_CreatesUserAndEmployeeAndSendsInvite()
    {
        var result = await CreateSut().InviteEmployeeAsync(Invite(), CancellationToken.None);

        var user = await _handle
            .Db.Users.AsNoTracking()
            .SingleAsync(u => u.Email == "new.staff@test.local");
        user.RoleId.ShouldBe(EmployeeRoleId);
        user.EmailConfirmed.ShouldBeTrue();
        user.IsActive.ShouldBeTrue();
        user.PasswordHash.ShouldBe(User.PendingPasswordHash);
        user.Phone.ShouldBe("0901234567");
        var employee = await _handle
            .Db.Employees.AsNoTracking()
            .SingleAsync(e => e.UserId == user.UserId);
        employee.HireDate.ShouldBe(new DateOnly(2026, 11, 1));
        result.EmployeeId.ShouldBe(employee.EmployeeId);
        result.IsPendingActivation.ShouldBeTrue();
        _registrationMock.Verify(
            a =>
                a.SendStaffInviteAsync(
                    It.Is<User>(u => u.UserId == user.UserId),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task InviteEmployeeAsync_DuplicateEmail_ThrowsAndSendsNothing()
    {
        TestDataSeeder.AddCustomer(_handle.Db, "dup@test.local");

        await Should.ThrowAsync<ValidationAppException>(() =>
            CreateSut().InviteEmployeeAsync(Invite("dup@test.local"), CancellationToken.None)
        );

        _registrationMock.Verify(
            a => a.SendStaffInviteAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        (await _handle.Db.Employees.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task InviteEmployeeAsync_UnknownPosition_Throws()
    {
        var request = Invite() with { PositionId = 9999 };

        await Should.ThrowAsync<ValidationAppException>(() =>
            CreateSut().InviteEmployeeAsync(request, CancellationToken.None)
        );

        (await _handle.Db.Users.AnyAsync(u => u.Email == request.Email)).ShouldBeFalse();
    }

    [Fact]
    public async Task InviteEmployeeAsync_SendInviteFails_RollsBackUserAndEmployee()
    {
        _registrationMock
            .Setup(a => a.SendStaffInviteAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("mail down"));

        await Should.ThrowAsync<InvalidOperationException>(() =>
            CreateSut().InviteEmployeeAsync(Invite(), CancellationToken.None)
        );

        _handle.Db.ChangeTracker.Clear();
        (await _handle.Db.Users.AnyAsync(u => u.Email == "new.staff@test.local")).ShouldBeFalse();
        (await _handle.Db.Employees.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task ResendEmployeeInviteAsync_PendingEmployee_SendsInvite()
    {
        var created = await CreateSut().InviteEmployeeAsync(Invite(), CancellationToken.None);
        _registrationMock.Invocations.Clear();

        await CreateSut().ResendEmployeeInviteAsync(created.EmployeeId, CancellationToken.None);

        _registrationMock.Verify(
            a =>
                a.SendStaffInviteAsync(
                    It.Is<User>(u => u.UserId == created.UserId),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task ResendEmployeeInviteAsync_ActivatedEmployee_Throws()
    {
        var (_, employee) = TestDataSeeder.AddEmployee(_handle.Db, "active@test.local");

        var ex = await Should.ThrowAsync<BusinessRuleException>(() =>
            CreateSut().ResendEmployeeInviteAsync(employee.EmployeeId, CancellationToken.None)
        );

        ex.Message.ShouldBe(Messages.Rule_EmployeeAlreadyActivated);
        _registrationMock.Verify(
            a => a.SendStaffInviteAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ResendEmployeeInviteAsync_UnknownEmployee_ThrowsNotFound() =>
        await Should.ThrowAsync<NotFoundException>(() =>
            CreateSut().ResendEmployeeInviteAsync(9999, CancellationToken.None)
        );

    [Fact]
    public async Task CreateEmployeeAsync_PromoteCustomer_ChangesRoleCreatesEmployeeAndRevokesTokens()
    {
        var caller = TestDataSeeder.AddEmployee(_handle.Db, "admin@test.local").User;
        var customer = TestDataSeeder.AddCustomer(_handle.Db, "cust@test.local");
        _handle.Db.RefreshTokens.Add(
            new RefreshToken
            {
                UserId = customer.UserId,
                TokenHash = "t1",
                ExpiresAt = DateTime.UtcNow.AddDays(5),
                CreatedAt = DateTime.UtcNow,
            }
        );
        await _handle.Db.SaveChangesAsync();

        var result = await CreateSut()
            .CreateEmployeeAsync(
                new CreateEmployeeRequest(customer.UserId, null, null, PromoteCustomer: true),
                caller.UserId,
                CancellationToken.None
            );

        result.UserId.ShouldBe(customer.UserId);
        (
            await _handle.Db.Users.AsNoTracking().SingleAsync(u => u.UserId == customer.UserId)
        ).RoleId.ShouldBe(EmployeeRoleId);
        (await _handle.Db.Employees.AnyAsync(e => e.UserId == customer.UserId)).ShouldBeTrue();
        (
            await _handle
                .Db.RefreshTokens.AsNoTracking()
                .Where(t => t.UserId == customer.UserId)
                .ToListAsync()
        ).ShouldAllBe(t => t.IsRevoked);
        (
            await _handle.Db.CustomerProfiles.AnyAsync(p => p.UserId == customer.UserId)
        ).ShouldBeTrue();
    }

    [Fact]
    public async Task CreateEmployeeAsync_CustomerWithoutPromoteFlag_StillThrows()
    {
        var customer = TestDataSeeder.AddCustomer(_handle.Db, "cust@test.local");

        var ex = await Should.ThrowAsync<BusinessRuleException>(() =>
            CreateSut()
                .CreateEmployeeAsync(
                    new CreateEmployeeRequest(customer.UserId, null, null),
                    currentUserId: 0,
                    CancellationToken.None
                )
        );

        ex.Message.ShouldBe(Messages.Rule_UserMustBeStaffRole);
        (
            await _handle.Db.Users.AsNoTracking().SingleAsync(u => u.UserId == customer.UserId)
        ).RoleId.ShouldBe(TestDataSeeder.CustomerRoleId);
    }

    [Fact]
    public async Task CreateEmployeeAsync_PromotingSelf_Throws()
    {
        var customer = TestDataSeeder.AddCustomer(_handle.Db, "cust@test.local");

        var ex = await Should.ThrowAsync<BusinessRuleException>(() =>
            CreateSut()
                .CreateEmployeeAsync(
                    new CreateEmployeeRequest(customer.UserId, null, null, PromoteCustomer: true),
                    customer.UserId,
                    CancellationToken.None
                )
        );

        ex.Message.ShouldBe(Messages.Rule_CannotChangeOwnRole);
    }

    [Fact]
    public async Task CreateEmployeeAsync_ExistingAdminWithPromoteFlag_KeepsAdminRole()
    {
        var caller = TestDataSeeder.AddEmployee(_handle.Db, "caller@test.local").User;
        var admin = TestDataSeeder.AddEmployee(_handle.Db, "other@test.local").User;
        _handle.Db.Employees.RemoveRange(_handle.Db.Employees.Where(e => e.UserId == admin.UserId));
        await _handle.Db.SaveChangesAsync();

        await CreateSut()
            .CreateEmployeeAsync(
                new CreateEmployeeRequest(admin.UserId, null, null, PromoteCustomer: true),
                caller.UserId,
                CancellationToken.None
            );

        (
            await _handle.Db.Users.AsNoTracking().SingleAsync(u => u.UserId == admin.UserId)
        ).RoleId.ShouldBe(TestDataSeeder.AdminRoleId);
    }

    [Fact]
    public async Task GetAdminUsersForSelectAsync_IncludeCustomers_ReturnsCustomersToo()
    {
        TestDataSeeder.AddCustomer(_handle.Db, "cust@test.local");
        TestDataSeeder.AddEmployee(_handle.Db, "staff@test.local");

        var withoutCustomers = await CreateUserAdminSut()
            .GetAdminUsersForSelectAsync(null, false, CancellationToken.None);
        var withCustomers = await CreateUserAdminSut()
            .GetAdminUsersForSelectAsync(
                null,
                false,
                CancellationToken.None,
                includeCustomers: true
            );

        withoutCustomers.Select(u => u.Email).ShouldBe(["staff@test.local"]);
        withCustomers
            .Select(u => u.Email)
            .ShouldBe(["cust@test.local", "staff@test.local"], ignoreOrder: true);
    }

    public void Dispose() => _handle.Dispose();
}

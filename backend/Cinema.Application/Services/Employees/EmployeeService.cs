using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Employees;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema.Application.Services.Employees;

public class EmployeeService(
    IAppDbContext db,
    IRegistrationService registrationService,
    IPasswordHasher<User> passwordHasher,
    ILogger<EmployeeService> logger
) : IEmployeeService
{
    public async Task<OffsetPageResult<AdminEmployeeDto>> GetEmployeesAdminAsync(
        int? positionId,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.Employees.Include(e => e.User).Include(e => e.Position).AsQueryable();
        if (positionId is not null)
            query = query.Where(e => e.PositionId == positionId);
        query = query.OrderBy(e => e.User.FullName);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<AdminEmployeeDto>()
            .ToListAsync(cancellationToken);

        return OffsetPageResult<AdminEmployeeDto>.Create(items, page, pageSize, total);
    }

    public async Task<AdminEmployeeDto> CreateEmployeeAsync(
        CreateEmployeeRequest request,
        int currentUserId,
        CancellationToken cancellationToken
    )
    {
        var user =
            await db
                .Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_UserId);

        var alreadyEmployee = await db.Employees.AnyAsync(
            e => e.UserId == request.UserId,
            cancellationToken
        );
        if (alreadyEmployee)
            throw new ConflictAppException(Messages.Conflict_EmployeeProfileExists);

        var promote = request.PromoteCustomer && user.Role.RoleName == Roles.Customer;
        if (promote && user.UserId == currentUserId)
            throw new BusinessRuleException(Messages.Rule_CannotChangeOwnRole);
        if (!promote && user.Role.RoleName != Roles.Admin && user.Role.RoleName != Roles.Employee)
            throw new BusinessRuleException(Messages.Rule_UserMustBeStaffRole);

        if (
            request.PositionId is not null
            && !await db.Positions.AnyAsync(
                p => p.PositionId == request.PositionId,
                cancellationToken
            )
        )
            throw new NotFoundException(Messages.NotFound_PositionId);

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        if (promote)
        {
            user.Role =
                await db.Roles.FirstOrDefaultAsync(
                    r => r.RoleName == Roles.Employee,
                    cancellationToken
                ) ?? throw new BusinessRuleException(Messages.Rule_EmployeeRoleNotSeeded);
            user.UpdatedAt = DateTime.UtcNow;
            await db.RevokeActiveTokensAsync(user.UserId, cancellationToken);
        }

        var employee = new Employee
        {
            UserId = request.UserId,
            PositionId = request.PositionId,
            HireDate = request.HireDate,
            IsActive = true,
        };
        db.Employees.Add(employee);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return await LoadEmployeeDtoAsync(employee.EmployeeId, cancellationToken);
    }

    public async Task<AdminEmployeeDto> InviteEmployeeAsync(
        InviteEmployeeRequest request,
        CancellationToken cancellationToken
    )
    {
        var email = request.Email.Trim();
        if (await db.Users.AnyAsync(u => u.Email == email, cancellationToken))
            throw new ValidationAppException(
                Messages.Invalid_EmailExists,
                new Dictionary<string, string[]> { ["email"] = [Messages.Invalid_EmailInUse] }
            );

        if (
            request.PositionId is not null
            && !await db.Positions.AnyAsync(
                p => p.PositionId == request.PositionId,
                cancellationToken
            )
        )
            throw new ValidationAppException(
                Messages.NotFound_PositionId,
                new Dictionary<string, string[]> { ["positionId"] = [Messages.NotFound_PositionId] }
            );

        var employeeRole =
            await db.Roles.FirstOrDefaultAsync(r => r.RoleName == Roles.Employee, cancellationToken)
            ?? throw new BusinessRuleException(Messages.Rule_EmployeeRoleNotSeeded);

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            PasswordHash = User.PendingPasswordHash,
            RoleId = employeeRole.RoleId,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
        };
        var employee = new Employee
        {
            User = user,
            PositionId = request.PositionId,
            HireDate = request.JoiningDate,
            IsActive = true,
        };
        db.Employees.Add(employee);
        await db.SaveChangesAsync(cancellationToken);

        await registrationService.SendStaffInviteAsync(user, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return await LoadEmployeeDtoAsync(employee.EmployeeId, cancellationToken);
    }

    public async Task<AdminEmployeeDto> CreateEmployeeDirectAsync(
        CreateEmployeeDirectRequest request,
        CancellationToken cancellationToken
    )
    {
        var email = request.Email.Trim();
        if (await db.Users.AnyAsync(u => u.Email == email, cancellationToken))
            throw new ValidationAppException(
                Messages.Invalid_EmailExists,
                new Dictionary<string, string[]> { ["email"] = [Messages.Invalid_EmailInUse] }
            );

        if (
            request.PositionId is not null
            && !await db.Positions.AnyAsync(
                p => p.PositionId == request.PositionId,
                cancellationToken
            )
        )
            throw new ValidationAppException(
                Messages.NotFound_PositionId,
                new Dictionary<string, string[]> { ["positionId"] = [Messages.NotFound_PositionId] }
            );

        var employeeRole =
            await db.Roles.FirstOrDefaultAsync(r => r.RoleName == Roles.Employee, cancellationToken)
            ?? throw new BusinessRuleException(Messages.Rule_EmployeeRoleNotSeeded);

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            RoleId = employeeRole.RoleId,
            IsActive = true,
            EmailConfirmed = true,
            MustChangePassword = true,
            CreatedAt = DateTime.UtcNow,
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.TemporaryPassword);

        var employee = new Employee
        {
            User = user,
            PositionId = request.PositionId,
            HireDate = request.JoiningDate,
            IsActive = true,
        };
        db.Employees.Add(employee);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation(
            "EmployeeCreatedDirect UserId {UserId} EmployeeId {EmployeeId}",
            user.UserId,
            employee.EmployeeId
        );

        return await LoadEmployeeDtoAsync(employee.EmployeeId, cancellationToken);
    }

    public async Task<AdminEmployeeDto> ResetEmployeePasswordAsync(
        int employeeId,
        int currentUserId,
        CancellationToken cancellationToken
    )
    {
        var employee =
            await db
                .Employees.Include(e => e.User)
                    .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Employee);
        var user = employee.User;

        if (user.UserId == currentUserId)
            throw new BusinessRuleException(Messages.Rule_CannotResetOwnPassword);
        if (user.Role.RoleName != Roles.Employee)
            throw new BusinessRuleException(Messages.Rule_OnlyEmployeePasswordCanBeReset);
        if (!employee.IsActive || !user.IsActive)
            throw new BusinessRuleException(Messages.Rule_CannotResetInactiveEmployeePassword);

        var defaultPassword = await db
            .CinemaSettings.Where(s => s.Id == AppDbContextExtensions.CinemaSettingsId)
            .Select(s => s.DefaultEmployeePassword)
            .FirstOrDefaultAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(defaultPassword))
            throw new BusinessRuleException(Messages.Rule_DefaultEmployeePasswordNotConfigured);

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var now = DateTime.UtcNow;
        user.PasswordHash = passwordHasher.HashPassword(user, defaultPassword);
        user.MustChangePassword = true;
        user.UpdatedAt = now;

        var revokedCount = await db.RevokeActiveTokensAsync(user.UserId, cancellationToken, now);

        var pendingLinks = await db
            .UserVerificationTokens.Where(t =>
                t.UserId == user.UserId
                && !t.Used
                && t.ExpiresAt > now
                && (
                    t.Purpose == VerificationTokenPurpose.PasswordReset
                    || t.Purpose == VerificationTokenPurpose.StaffInvite
                )
            )
            .ToListAsync(cancellationToken);
        foreach (var token in pendingLinks)
            token.Used = true;

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation(
            "EmployeePasswordReset ActorUserId {ActorUserId} TargetUserId {TargetUserId} EmployeeId {EmployeeId} — đã thu hồi {RevokedCount} refresh token, vô hiệu hóa {LinkCount} link.",
            currentUserId,
            user.UserId,
            employee.EmployeeId,
            revokedCount,
            pendingLinks.Count
        );

        return await LoadEmployeeDtoAsync(employee.EmployeeId, cancellationToken);
    }

    public async Task<AdminEmployeeDto> ResendEmployeeInviteAsync(
        int employeeId,
        CancellationToken cancellationToken
    )
    {
        var employee =
            await db
                .Employees.Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Employee);

        if (employee.User.PasswordHash != User.PendingPasswordHash)
            throw new BusinessRuleException(Messages.Rule_EmployeeAlreadyActivated);

        await registrationService.SendStaffInviteAsync(employee.User, cancellationToken);

        return await LoadEmployeeDtoAsync(employee.EmployeeId, cancellationToken);
    }

    public async Task<AdminEmployeeDto> UpdateEmployeeAsync(
        int employeeId,
        UpdateEmployeeRequest request,
        int currentUserId,
        CancellationToken cancellationToken
    )
    {
        var employee =
            await db.Employees.FirstOrDefaultAsync(
                e => e.EmployeeId == employeeId,
                cancellationToken
            ) ?? throw new NotFoundException(Messages.NotFound_Employee);

        if (employee.UserId == currentUserId && employee.IsActive && !request.IsActive)
            throw new BusinessRuleException(Messages.Rule_CannotDeactivateSelfEmployee);

        if (
            request.PositionId is not null
            && !await db.Positions.AnyAsync(
                p => p.PositionId == request.PositionId,
                cancellationToken
            )
        )
            throw new ValidationAppException(
                Messages.NotFound_PositionId,
                new Dictionary<string, string[]> { ["positionId"] = [Messages.NotFound_PositionId] }
            );

        var wasActive = employee.IsActive;
        employee.PositionId = request.PositionId;
        employee.HireDate = request.HireDate;
        employee.IsActive = request.IsActive;

        if (wasActive && !request.IsActive)
            await db.RevokeActiveTokensAsync(employee.UserId, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return await LoadEmployeeDtoAsync(employee.EmployeeId, cancellationToken);
    }

    private async Task<AdminEmployeeDto> LoadEmployeeDtoAsync(
        int employeeId,
        CancellationToken cancellationToken
    )
    {
        var employee = await db
            .Employees.Include(e => e.User)
                .ThenInclude(u => u.Role)
            .Include(e => e.Position)
            .FirstAsync(e => e.EmployeeId == employeeId, cancellationToken);
        return employee.Adapt<AdminEmployeeDto>();
    }
}

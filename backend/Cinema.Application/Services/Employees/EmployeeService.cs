using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Employees;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Employees;

public class EmployeeService : IEmployeeService
{
    private const int UserLookupLimit = 50;

    private readonly IAppDbContext db;
    private readonly IOptions<BookingOptions> bookingOptions;

    public EmployeeService(IAppDbContext db, IOptions<BookingOptions>? bookingOptions = null)
    {
        this.db = db;
        this.bookingOptions =
            bookingOptions ?? Microsoft.Extensions.Options.Options.Create(new BookingOptions());
    }

    public async Task<OffsetPageResult<AdminUserListItemDto>> GetUsersAdminAsync(
        int? roleId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.Users.Include(u => u.Role).AsQueryable();

        if (roleId is not null)
            query = query.Where(u => u.RoleId == roleId);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));
        query = query.OrderByDescending(u => u.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<AdminUserListItemDto>()
            .ToListAsync(cancellationToken);

        return OffsetPageResult<AdminUserListItemDto>.Create(items, page, pageSize, total);
    }

    public async Task<List<AdminUserSelectDto>> GetAdminUsersForSelectAsync(
        string? search,
        bool availableOnly,
        CancellationToken cancellationToken
    )
    {
        var staffRoleIds = await db
            .Roles.Where(r => r.RoleName == Roles.Admin || r.RoleName == Roles.Employee)
            .Select(r => r.RoleId)
            .ToListAsync(cancellationToken);
        if (staffRoleIds.Count == 0)
            return [];

        var query = db
            .Users.AsNoTracking()
            .Where(u => staffRoleIds.Contains(u.RoleId) && u.IsActive);

        if (availableOnly)
        {
            var employeeUserIds = db.Employees.Select(e => e.UserId);
            query = query.Where(u => !employeeUserIds.Contains(u.UserId));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var trimmed = search.Trim();
            query = query.Where(u => u.FullName.Contains(trimmed) || u.Email.Contains(trimmed));
        }

        return await query
            .OrderBy(u => u.FullName)
            .Take(UserLookupLimit)
            .Select(u => new AdminUserSelectDto(u.UserId, u.FullName, u.Email))
            .ToListAsync(cancellationToken);
    }

    public async Task<UpdateUserStatusResponse> UpdateUserStatusAsync(
        int userId,
        UpdateUserStatusRequest request,
        CancellationToken cancellationToken
    )
    {
        var user =
            await db.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_User);

        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        if (!request.IsActive)
        {
            var now = DateTime.UtcNow;
            var activeTokens = await db
                .RefreshTokens.Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync(cancellationToken);
            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = now;
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        return new UpdateUserStatusResponse(userId, user.IsActive);
    }

    public async Task<BulkActionResponse> BulkUpdateUserStatusAsync(
        BulkStatusRequest request,
        int currentUserId,
        CancellationToken cancellationToken
    )
    {
        var ids = request.Ids.Distinct().ToList();
        var users = await db
            .Users.Include(u => u.Role)
            .Where(u => ids.Contains(u.UserId))
            .ToDictionaryAsync(u => u.UserId, cancellationToken);
        var activeAdmins = request.IsActive
            ? 0
            : await db.Users.CountAsync(
                u => u.IsActive && u.Role.RoleName == Roles.Admin,
                cancellationToken
            );

        var now = DateTime.UtcNow;
        var results = new List<BulkActionResultItem>();
        var changedIds = new List<int>();
        foreach (var id in ids)
        {
            if (!users.TryGetValue(id, out var user))
            {
                results.Add(new BulkActionResultItem(id, false, Messages.NotFound_User));
                continue;
            }
            if (user.IsActive == request.IsActive)
            {
                results.Add(new BulkActionResultItem(id, true, null));
                continue;
            }
            if (!request.IsActive)
            {
                if (id == currentUserId)
                {
                    results.Add(new BulkActionResultItem(id, false, Messages.Rule_CannotLockSelf));
                    continue;
                }
                if (user.Role.RoleName == Roles.Admin)
                {
                    if (activeAdmins <= 1)
                    {
                        results.Add(
                            new BulkActionResultItem(id, false, Messages.Rule_CannotLockLastAdmin)
                        );
                        continue;
                    }
                    activeAdmins--;
                }
            }

            user.IsActive = request.IsActive;
            user.UpdatedAt = now;
            changedIds.Add(id);
            results.Add(new BulkActionResultItem(id, true, null));
        }

        if (changedIds.Count > 0)
        {
            var employees = await db
                .Employees.Where(e => changedIds.Contains(e.UserId))
                .ToListAsync(cancellationToken);
            foreach (var employee in employees)
                employee.IsActive = request.IsActive;

            if (!request.IsActive)
            {
                var activeTokens = await db
                    .RefreshTokens.Where(rt => changedIds.Contains(rt.UserId) && !rt.IsRevoked)
                    .ToListAsync(cancellationToken);
                foreach (var token in activeTokens)
                {
                    token.IsRevoked = true;
                    token.RevokedAt = now;
                }
            }

            await db.SaveChangesAsync(cancellationToken);
        }

        return BulkOperation.Build(results);
    }

    public async Task<UpdateUserRoleResponse> UpdateUserRoleAsync(
        int userId,
        UpdateUserRoleRequest request,
        CancellationToken cancellationToken
    )
    {
        var user =
            await db
                .Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_User);

        var newRole =
            await db.Roles.FirstOrDefaultAsync(r => r.RoleId == request.RoleId, cancellationToken)
            ?? throw new ValidationAppException(
                Messages.Invalid_RoleIdNotFound,
                new Dictionary<string, string[]> { ["roleId"] = [Messages.Invalid_RoleIdNotFound] }
            );

        if (newRole.RoleName == Roles.Customer)
        {
            var hasActiveEmployee = await db.Employees.AnyAsync(
                e => e.UserId == userId && e.IsActive,
                cancellationToken
            );
            if (hasActiveEmployee)
                throw new ConflictAppException(
                    Messages.Conflict_CannotDowngradeRoleWithActiveEmployee
                );
        }

        user.RoleId = request.RoleId;
        user.UpdatedAt = DateTime.UtcNow;

        var now = DateTime.UtcNow;
        var activeTokens = await db
            .RefreshTokens.Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);
        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = now;
        }

        await db.SaveChangesAsync(cancellationToken);

        return new UpdateUserRoleResponse(userId, newRole.RoleId, newRole.RoleName);
    }

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
        CancellationToken cancellationToken
    )
    {
        var user =
            await db.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_UserId);

        var alreadyEmployee = await db.Employees.AnyAsync(
            e => e.UserId == request.UserId,
            cancellationToken
        );
        if (alreadyEmployee)
            throw new ConflictAppException(Messages.Conflict_EmployeeProfileExists);

        var role = await db.Roles.FirstAsync(r => r.RoleId == user.RoleId, cancellationToken);
        if (role.RoleName != Roles.Admin && role.RoleName != Roles.Employee)
            throw new BusinessRuleException(
                Messages.Rule_UserMustBeStaffRole
            );

        if (
            request.PositionId is not null
            && !await db.Positions.AnyAsync(
                p => p.PositionId == request.PositionId,
                cancellationToken
            )
        )
            throw new NotFoundException(Messages.NotFound_PositionId);

        var employee = new Employee
        {
            UserId = request.UserId,
            PositionId = request.PositionId,
            HireDate = request.HireDate,
            IsActive = true,
        };
        db.Employees.Add(employee);
        await db.SaveChangesAsync(cancellationToken);

        return await LoadEmployeeDtoAsync(employee.EmployeeId, cancellationToken);
    }

    public async Task<AdminEmployeeDto> UpdateEmployeeAsync(
        int employeeId,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken
    )
    {
        var employee =
            await db.Employees.FirstOrDefaultAsync(
                e => e.EmployeeId == employeeId,
                cancellationToken
            ) ?? throw new NotFoundException(Messages.NotFound_Employee);

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
        {
            var now = DateTime.UtcNow;
            var activeTokens = await db
                .RefreshTokens.Where(rt => rt.UserId == employee.UserId && !rt.IsRevoked)
                .ToListAsync(cancellationToken);
            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = now;
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        return await LoadEmployeeDtoAsync(employee.EmployeeId, cancellationToken);
    }

    public async Task<List<CustomerLookupItemDto>> SearchCustomersForCounterAsync(
        string? query,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        var trimmed = query.Trim();
        var limit = bookingOptions.Value.CustomerLookupLimit;

        var users = await db
            .Users.AsNoTracking()
            .Include(u => u.Role)
            .Include(u => u.CustomerProfile)
                .ThenInclude(cp => cp!.Tier)
            .Where(u =>
                u.Role.RoleName == Roles.Customer
                && u.IsActive
                && (
                    (u.Phone != null && u.Phone.Contains(trimmed))
                    || u.Email.Contains(trimmed)
                    || u.FullName.Contains(trimmed)
                )
            )
            .OrderBy(u => u.FullName)
            .Take(limit)
            .ToListAsync(cancellationToken);

        // Hạng/giảm giá hiển thị cho nhân viên phải khớp với mức giảm thực tế khi giữ ghế
        // (tính từ sổ điểm 12 tháng), không dùng Tier cache trên profile.
        var standings = await LoyaltyWindow.GetStandingsAsync(
            db,
            users.Select(u => u.UserId).ToList(),
            DateTime.UtcNow,
            cancellationToken
        );

        return users
            .Select(u =>
            {
                var tier = standings.TryGetValue(u.UserId, out var st) ? st.Tier : null;
                return new CustomerLookupItemDto(
                    UserId: u.UserId,
                    FullName: u.FullName,
                    PhoneLast4: u.Phone?.Length >= 4 ? u.Phone[^4..] : null,
                    MaskedEmail: MaskEmail(u.Email),
                    TierName: tier?.TierName,
                    DiscountPercent: tier?.DiscountPercent ?? 0m
                );
            })
            .ToList();
    }

    private static string MaskEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        if (atIndex < 0)
            return email;

        var local = email[..atIndex];
        var domain = email[atIndex..]; // includes '@'

        if (local.Length <= 2)
            return local[..1] + "***" + domain;

        return local[..2] + "***" + domain;
    }

    private async Task<AdminEmployeeDto> LoadEmployeeDtoAsync(
        int employeeId,
        CancellationToken cancellationToken
    )
    {
        var employee = await db
            .Employees.Include(e => e.User)
            .Include(e => e.Position)
            .FirstAsync(e => e.EmployeeId == employeeId, cancellationToken);
        return employee.Adapt<AdminEmployeeDto>();
    }
}

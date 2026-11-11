using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Employees;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Employees;

public class UserAdminService(IAppDbContext db, IOptions<BookingOptions>? bookingOptions = null)
    : IUserAdminService
{
    private const int UserLookupLimit = 50;

    private readonly BookingOptions _bookingOptions = bookingOptions?.Value ?? new BookingOptions();

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
        CancellationToken cancellationToken,
        bool includeCustomers = false
    )
    {
        var staffRoleIds = await db
            .Roles.Where(r =>
                r.RoleName == Roles.Admin
                || r.RoleName == Roles.Employee
                || (includeCustomers && r.RoleName == Roles.Customer)
            )
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

    public async Task<List<CustomerLookupItemDto>> SearchCustomersForCounterAsync(
        string? query,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        var trimmed = query.Trim();
        var limit = _bookingOptions.CustomerLookupLimit;

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

    public async Task<UpdateUserStatusResponse> UpdateUserStatusAsync(
        int userId,
        UpdateUserStatusRequest request,
        int currentUserId,
        CancellationToken cancellationToken
    )
    {
        var user =
            await db
                .Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_User);

        if (!request.IsActive && user.IsActive)
        {
            if (userId == currentUserId)
                throw new BusinessRuleException(Messages.Rule_CannotLockSelf);
            if (
                user.Role.RoleName == Roles.Admin
                && await CountOtherActiveAdminsAsync(userId, cancellationToken) == 0
            )
                throw new BusinessRuleException(Messages.Rule_CannotLockLastAdmin);
        }

        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        var employee = await db.Employees.FirstOrDefaultAsync(
            e => e.UserId == userId,
            cancellationToken
        );
        if (employee is not null)
            employee.IsActive = request.IsActive;

        if (!request.IsActive)
            await db.RevokeActiveTokensAsync(userId, cancellationToken);

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
                await db.RevokeActiveTokensAsync(changedIds, cancellationToken, now);

            await db.SaveChangesAsync(cancellationToken);
        }

        return BulkOperation.Build(results);
    }

    public async Task<UpdateUserRoleResponse> UpdateUserRoleAsync(
        int userId,
        UpdateUserRoleRequest request,
        int currentUserId,
        CancellationToken cancellationToken
    )
    {
        if (userId == currentUserId)
            throw new BusinessRuleException(Messages.Rule_CannotChangeOwnRole);

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

        if (
            user.Role.RoleName == Roles.Admin
            && newRole.RoleName != Roles.Admin
            && await CountOtherActiveAdminsAsync(userId, cancellationToken) == 0
        )
            throw new BusinessRuleException(Messages.Rule_CannotDemoteLastAdmin);

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

        await db.RevokeActiveTokensAsync(userId, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return new UpdateUserRoleResponse(userId, newRole.RoleId, newRole.RoleName);
    }

    private Task<int> CountOtherActiveAdminsAsync(
        int excludeUserId,
        CancellationToken cancellationToken
    ) =>
        db.Users.CountAsync(
            u => u.UserId != excludeUserId && u.IsActive && u.Role.RoleName == Roles.Admin,
            cancellationToken
        );

    private static string MaskEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        if (atIndex < 0)
            return email;

        var local = email[..atIndex];
        var domain = email[atIndex..];

        if (local.Length <= 2)
            return local[..1] + "***" + domain;

        return local[..2] + "***" + domain;
    }
}

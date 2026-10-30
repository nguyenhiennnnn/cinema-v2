using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Employees;
using Cinema.Application.Interfaces;
using Cinema.Domain.Entities;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Employees;

public class EmployeeService(IAppDbContext db) : IEmployeeService
{
    private const int UserLookupLimit = 50;

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
            ?? throw new NotFoundException("Không tìm thấy người dùng.");

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
            ?? throw new NotFoundException("Không tìm thấy người dùng.");

        var newRole =
            await db.Roles.FirstOrDefaultAsync(r => r.RoleId == request.RoleId, cancellationToken)
            ?? throw new ValidationAppException(
                "roleId không tồn tại.",
                new Dictionary<string, string[]> { ["roleId"] = ["roleId không tồn tại."] }
            );

        if (newRole.RoleName == Roles.Customer)
        {
            var hasActiveEmployee = await db.Employees.AnyAsync(
                e => e.UserId == userId && e.IsActive,
                cancellationToken
            );
            if (hasActiveEmployee)
                throw new ConflictAppException(
                    "Không thể hạ role xuống Customer khi còn hồ sơ Employee đang hoạt động — vô hiệu hóa Employee trước."
                );
        }

        user.RoleId = request.RoleId;
        user.UpdatedAt = DateTime.UtcNow;
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
            ?? throw new NotFoundException("userId không tồn tại.");

        var alreadyEmployee = await db.Employees.AnyAsync(
            e => e.UserId == request.UserId,
            cancellationToken
        );
        if (alreadyEmployee)
            throw new ConflictAppException("userId đã có hồ sơ Employee.");

        var role = await db.Roles.FirstAsync(r => r.RoleId == user.RoleId, cancellationToken);
        if (role.RoleName != Roles.Admin && role.RoleName != Roles.Employee)
            throw new BusinessRuleException(
                "User phải có RoleId là Admin hoặc Employee trước khi tạo hồ sơ Employee (đổi role qua PUT /admin/users/{id}/role)."
            );

        if (
            request.PositionId is not null
            && !await db.Positions.AnyAsync(
                p => p.PositionId == request.PositionId,
                cancellationToken
            )
        )
            throw new NotFoundException("positionId không tồn tại.");

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
            ) ?? throw new NotFoundException("Không tìm thấy nhân viên.");

        if (
            request.PositionId is not null
            && !await db.Positions.AnyAsync(
                p => p.PositionId == request.PositionId,
                cancellationToken
            )
        )
            throw new ValidationAppException(
                "positionId không tồn tại.",
                new Dictionary<string, string[]> { ["positionId"] = ["positionId không tồn tại."] }
            );

        employee.PositionId = request.PositionId;
        employee.HireDate = request.HireDate;
        employee.IsActive = request.IsActive;
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
            .Include(e => e.Position)
            .FirstAsync(e => e.EmployeeId == employeeId, cancellationToken);
        return employee.Adapt<AdminEmployeeDto>();
    }
}

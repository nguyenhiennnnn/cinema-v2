using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Employees;

namespace Cinema.Application.Interfaces;

public interface IUserAdminService
{
    Task<OffsetPageResult<AdminUserListItemDto>> GetUsersAdminAsync(
        int? roleId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );

    Task<List<AdminUserSelectDto>> GetAdminUsersForSelectAsync(
        string? search,
        bool availableOnly,
        CancellationToken cancellationToken,
        bool includeCustomers = false
    );

    Task<List<CustomerLookupItemDto>> SearchCustomersForCounterAsync(
        string? query,
        CancellationToken cancellationToken
    );

    Task<UpdateUserStatusResponse> UpdateUserStatusAsync(
        int userId,
        UpdateUserStatusRequest request,
        int currentUserId,
        CancellationToken cancellationToken
    );

    Task<BulkActionResponse> BulkUpdateUserStatusAsync(
        BulkStatusRequest request,
        int currentUserId,
        CancellationToken cancellationToken
    );

    Task<UpdateUserRoleResponse> UpdateUserRoleAsync(
        int userId,
        UpdateUserRoleRequest request,
        int currentUserId,
        CancellationToken cancellationToken
    );
}

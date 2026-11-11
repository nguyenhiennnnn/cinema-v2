using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Employees;

namespace Cinema.Application.Interfaces;

public interface IEmployeeService
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

    Task<OffsetPageResult<AdminEmployeeDto>> GetEmployeesAdminAsync(
        int? positionId,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<AdminEmployeeDto> CreateEmployeeAsync(
        CreateEmployeeRequest request,
        int currentUserId,
        CancellationToken cancellationToken
    );
    Task<AdminEmployeeDto> InviteEmployeeAsync(
        InviteEmployeeRequest request,
        CancellationToken cancellationToken
    );
    Task<AdminEmployeeDto> ResendEmployeeInviteAsync(
        int employeeId,
        CancellationToken cancellationToken
    );
    Task<AdminEmployeeDto> UpdateEmployeeAsync(
        int employeeId,
        UpdateEmployeeRequest request,
        int currentUserId,
        CancellationToken cancellationToken
    );
}
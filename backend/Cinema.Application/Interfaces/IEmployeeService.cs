using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Employees;

namespace Cinema.Application.Interfaces;

public interface IEmployeeService
{
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

namespace Cinema.Application.DTOs.Employees;

public record AdminUserListItemDto(
    int UserId,
    string FullName,
    string Email,
    string RoleId,
    string RoleName,
    bool IsActive,
    DateTime CreatedAt
);

public record AdminUserSelectDto(int UserId, string FullName, string Email);

public record UpdateUserStatusRequest(bool IsActive);

public record UpdateUserStatusResponse(int UserId, bool IsActive);

public record UpdateUserRoleRequest(int RoleId);

public record UpdateUserRoleResponse(int UserId, int RoleId, string RoleName);

public record AdminEmployeeDto(
    int EmployeeId,
    string FullName,
    string Email,
    int? PositionId,
    string? PositionName,
    DateOnly? HireDate,
    bool IsActive
);

public record CreateEmployeeRequest(int UserId, int? PositionId, DateOnly? HireDate);

public record UpdateEmployeeRequest(int? PositionId, DateOnly? HireDate, bool IsActive);

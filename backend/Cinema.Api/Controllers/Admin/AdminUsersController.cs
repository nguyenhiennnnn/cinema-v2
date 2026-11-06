using Cinema.Api.Filters;
using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Employees;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Cinema.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/users")]
[Authorize]
public class AdminUsersController(IEmployeeService employeeService) : AppControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int? roleId,
        [FromQuery] string? search,
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    ) =>
        Ok(
            await employeeService.GetUsersAdminAsync(
                roleId,
                search,
                page,
                pageSize,
                cancellationToken
            )
        );

    [HttpGet("admins")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetAdminUsers(
        [FromQuery] string? search,
        [FromQuery] bool availableOnly = true,
        CancellationToken cancellationToken = default
    ) =>
        Ok(
            await employeeService.GetAdminUsersForSelectAsync(
                search,
                availableOnly,
                cancellationToken
            )
        );

    [HttpGet("customers")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> SearchCustomers(
        [FromQuery] string? q,
        CancellationToken cancellationToken
    ) => Ok(await employeeService.SearchCustomersForCounterAsync(q, cancellationToken));

    [HttpPut("{id:int}/status")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateUserStatusRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await employeeService.UpdateUserStatusAsync(
            id,
            request,
            CurrentUserId,
            cancellationToken
        );
        return Ok(
            ApiResponse.SuccessResponse(
                result,
                request.IsActive ? Messages.Success_UserUnlocked : Messages.Success_UserLocked
            )
        );
    }

    [HttpPut("{id:int}/role")]
    [SuccessMessage(nameof(Messages.Success_UserRoleUpdated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> UpdateRole(
        int id,
        [FromBody] UpdateUserRoleRequest request,
        CancellationToken cancellationToken
    ) => Ok(
            await employeeService.UpdateUserRoleAsync(
                id,
                request,
                CurrentUserId,
                cancellationToken
            )
        );

    [HttpPut("bulk/status")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> BulkUpdateStatus(
        [FromBody] BulkStatusRequest request,
        CancellationToken cancellationToken
    ) =>
        Ok(
            await employeeService.BulkUpdateUserStatusAsync(
                request,
                CurrentUserId,
                cancellationToken
            )
        );
}

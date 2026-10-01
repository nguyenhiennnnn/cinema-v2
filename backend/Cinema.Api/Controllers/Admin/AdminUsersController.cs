using Cinema.Application.DTOs.Employees;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/users")]
[Authorize]
public class AdminUsersController(IEmployeeService employeeService) : ControllerBase
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

    [HttpPut("{id:int}/status")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateUserStatusRequest request,
        CancellationToken cancellationToken
    ) => Ok(await employeeService.UpdateUserStatusAsync(id, request, cancellationToken));

    [HttpPut("{id:int}/role")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> UpdateRole(
        int id,
        [FromBody] UpdateUserRoleRequest request,
        CancellationToken cancellationToken
    ) => Ok(await employeeService.UpdateUserRoleAsync(id, request, cancellationToken));
}

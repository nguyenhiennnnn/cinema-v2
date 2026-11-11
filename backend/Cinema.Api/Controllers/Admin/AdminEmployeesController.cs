using Cinema.Api.Filters;
using Cinema.Application.DTOs.Employees;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Cinema.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/employees")]
[Authorize]
public class AdminEmployeesController(IEmployeeService employeeService) : AppControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetEmployees(
        [FromQuery] int? positionId,
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    ) =>
        Ok(
            await employeeService.GetEmployeesAdminAsync(
                positionId,
                page,
                pageSize,
                cancellationToken
            )
        );

    [HttpPost]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Create(
        [FromBody] CreateEmployeeRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await employeeService.CreateEmployeeAsync(
            request,
            CurrentUserId,
            cancellationToken
        );
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse.SuccessResponse(
                result,
                request.PromoteCustomer
                    ? Messages.Success_EmployeePromoted
                    : Messages.Success_EmployeeCreated
            )
        );
    }

    [HttpPost("invite")]
    [SuccessMessage(nameof(Messages.Success_EmployeeInvited))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Invite(
        [FromBody] InviteEmployeeRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await employeeService.InviteEmployeeAsync(request, cancellationToken)
        );

    [HttpPost("{id:int}/resend-invite")]
    [SuccessMessage(nameof(Messages.Success_EmployeeInviteResent))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> ResendInvite(int id, CancellationToken cancellationToken) =>
        Ok(await employeeService.ResendEmployeeInviteAsync(id, cancellationToken));

    [HttpPut("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_EmployeeUpdated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateEmployeeRequest request,
        CancellationToken cancellationToken
    ) =>
        Ok(
            await employeeService.UpdateEmployeeAsync(id, request, CurrentUserId, cancellationToken)
        );
}

using Cinema.Application.DTOs.Employees;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/employees")]
[Authorize]
public class AdminEmployeesController(IEmployeeService employeeService) : ControllerBase
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
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await employeeService.CreateEmployeeAsync(request, cancellationToken)
        );

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateEmployeeRequest request,
        CancellationToken cancellationToken
    ) => Ok(await employeeService.UpdateEmployeeAsync(id, request, cancellationToken));
}

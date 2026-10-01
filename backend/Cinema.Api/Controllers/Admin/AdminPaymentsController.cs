using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/payments")]
[Authorize]
public class AdminPaymentsController(IPaymentAdminService paymentAdminService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetPayments(
        [FromQuery] AdminPaymentListFilter filter,
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    )
    {
        var result = await paymentAdminService.GetPaymentsAsync(
            filter,
            page,
            pageSize,
            cancellationToken
        );
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetDetail(int id, CancellationToken cancellationToken)
    {
        var result = await paymentAdminService.GetPaymentDetailAsync(id, cancellationToken);
        return Ok(result);
    }
}

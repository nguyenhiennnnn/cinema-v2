using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/payments")]
[Authorize]
public class AdminPaymentsController(
    IPaymentAdminService paymentAdminService,
    IPaymentExportService paymentExportService
) : ControllerBase
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

    [HttpGet("export")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> ExportPayments(
        [FromQuery] AdminPaymentListFilter filter,
        [FromQuery] string? format,
        CancellationToken cancellationToken
    )
    {
        var exportFormat = ExportFormats.Parse(format);
        var file = await paymentExportService.ExportPaymentsAsync(
            filter,
            exportFormat,
            cancellationToken
        );
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetDetail(int id, CancellationToken cancellationToken)
    {
        var result = await paymentAdminService.GetPaymentDetailAsync(id, cancellationToken);
        return Ok(result);
    }
}

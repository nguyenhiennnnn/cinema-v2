using Cinema.Application.DTOs.Reviews;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/reviews")]
[Authorize]
public class AdminReviewsController(IReviewService reviewService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetReviews(
        [FromQuery] int? movieId,
        [FromQuery] int? rating,
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    ) =>
        Ok(
            await reviewService.GetReviewsAdminAsync(
                movieId,
                rating,
                page,
                pageSize,
                cancellationToken
            )
        );

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        Ok(await reviewService.DeleteReviewAsync(id, cancellationToken));
    [HttpDelete("bulk")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> BulkDelete(
        [FromBody] BulkDeleteReviewsRequest request,
        CancellationToken cancellationToken
    ) => Ok(await reviewService.BulkDeleteReviewsAsync(request, cancellationToken));
}

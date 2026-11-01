using Cinema.Api.Filters;
using Cinema.Application.DTOs.Reviews;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Client;

[ApiController]
[Route("api/v1/movies")]
public class MoviesController(IMovieService movieService, IReviewService reviewService)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMovies(
        [FromQuery] MovieListFilter filter,
        [FromQuery] string? cursor,
        [FromQuery] int? limit,
        CancellationToken cancellationToken
    )
    {
        var result = await movieService.GetMoviesAsync(
            filter,
            cursor,
            ClampLimit(limit),
            cancellationToken
        );
        return Ok(result);
    }

    [HttpGet("{movieId:int}")]
    public async Task<IActionResult> GetMovieDetail(
        int movieId,
        CancellationToken cancellationToken
    )
    {
        var result = await movieService.GetMovieDetailAsync(movieId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{movieId:int}/reviews")]
    public async Task<IActionResult> GetReviews(
        int movieId,
        [FromQuery] string? cursor,
        [FromQuery] int? limit,
        CancellationToken cancellationToken
    )
    {
        var result = await reviewService.GetMovieReviewsAsync(
            movieId,
            cursor,
            ClampLimit(limit),
            cancellationToken
        );
        return Ok(result);
    }

    [HttpGet("{movieId:int}/can-review")]
    public async Task<ActionResult<CanReviewResponse>> GetCanReviewStatus(
        int movieId,
        CancellationToken cancellationToken
    )
    {
        if (CurrentUserIdOrNull is not { } userId)
        {
            return Ok(new CanReviewResponse(false));
        }

        var result = await reviewService.GetUserMovieReviewStatusAsync(
            userId,
            movieId,
            cancellationToken
        );
        return Ok(result);
    }

    [HttpPost("{movieId:int}/reviews")]
    [SuccessMessage(nameof(Messages.Success_ReviewCreated))]
    [Authorize(Policy = Policies.CustomerOnly)]
    public async Task<IActionResult> CreateReview(
        int movieId,
        [FromBody] CreateReviewRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await reviewService.CreateReviewAsync(
            CurrentUserId,
            movieId,
            request,
            cancellationToken
        );
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{movieId:int}/reviews/{id:int}")]
    [SuccessMessage(nameof(Messages.Success_ReviewUpdated))]
    [Authorize(Policy = Policies.CustomerOnly)]
    public async Task<IActionResult> UpdateReview(
        int movieId,
        int id,
        [FromBody] CreateReviewRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await reviewService.UpdateReviewAsync(
            CurrentUserId,
            movieId,
            id,
            request,
            cancellationToken
        );
        return Ok(result);
    }

    [HttpDelete("{movieId:int}/reviews/{id:int}")]
    [SuccessMessage(nameof(Messages.Success_ReviewDeleted))]
    [Authorize(Policy = Policies.CustomerOnly)]
    public async Task<IActionResult> DeleteReview(
        int movieId,
        int id,
        CancellationToken cancellationToken
    )
    {
        var result = await reviewService.DeleteMyReviewAsync(
            CurrentUserId,
            movieId,
            id,
            cancellationToken
        );
        return Ok(result);
    }
}

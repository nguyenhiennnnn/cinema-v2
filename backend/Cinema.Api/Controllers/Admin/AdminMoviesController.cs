using Cinema.Api.Filters;
using Cinema.Application.DTOs.Movies;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/movies")]
[Authorize]
public class AdminMoviesController(IMovieService movieService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> GetMovies(
        [FromQuery] AdminMovieListFilter filter,
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    )
    {
        var result = await movieService.GetMoviesAdminAsync(
            filter,
            page,
            pageSize,
            cancellationToken
        );

        return Ok(result);
    }

    [HttpPost]
    [SuccessMessage(nameof(Messages.Success_MovieCreated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Create(
        [FromBody] UpsertMovieRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await movieService.CreateMovieAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_MovieUpdated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpsertMovieRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await movieService.UpdateMovieAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_MovieDeleted))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await movieService.DeleteMovieAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("bulk")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> BulkDelete(
        [FromBody] BulkDeleteMoviesRequest request,
        CancellationToken cancellationToken
    ) => Ok(await movieService.BulkDeleteMoviesAsync(request, cancellationToken));
}

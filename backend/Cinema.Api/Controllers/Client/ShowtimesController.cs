using Cinema.Application.Interfaces;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Client;

[ApiController]
[Route("api/v1/showtimes")]
public class ShowtimesController(IShowtimeQueryService showtimeQueryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSchedule(
        [FromQuery] DateOnly? date,
        [FromQuery] string? screenType,
        CancellationToken cancellationToken
    )
    {
        if (date is null)
            throw new ValidationAppException(
                Messages.Invalid_DateRequired,
                new Dictionary<string, string[]> { ["date"] = [Messages.Invalid_DateRequired] }
            );

        var result = await showtimeQueryService.GetWholeCinemaScheduleAsync(
            date.Value,
            screenType,
            cancellationToken
        );
        return Ok(result);
    }

    [HttpGet("{showtimeId:int}/seats")]
    public async Task<IActionResult> GetSeats(int showtimeId, CancellationToken cancellationToken)
    {
        var result = await showtimeQueryService.GetShowtimeSeatsAsync(
            showtimeId,
            cancellationToken
        );
        return Ok(result);
    }

    [HttpGet("{showtimeId:int}/seat-suggestions")]
    public async Task<IActionResult> GetSeatSuggestions(
        int showtimeId,
        [FromQuery] int count,
        CancellationToken cancellationToken
    )
    {
        if (count is < 1 or > 10)
            throw new ValidationAppException(
                Messages.Invalid_CountOutOfRange,
                new Dictionary<string, string[]> { ["count"] = [Messages.Invalid_CountOutOfRange] }
            );

        var result = await showtimeQueryService.GetSeatSuggestionsAsync(
            showtimeId,
            count,
            cancellationToken
        );
        return Ok(result);
    }
}

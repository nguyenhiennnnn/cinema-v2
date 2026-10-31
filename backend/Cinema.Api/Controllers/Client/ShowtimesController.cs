using Cinema.Application.Interfaces;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Client;

[ApiController]
[Route("api/v1/showtimes")]
public class ShowtimesController(IShowtimeService showtimeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSchedule(
        [FromQuery] DateOnly? date,
        CancellationToken cancellationToken
    )
    {
        if (date is null)
            throw new ValidationAppException(
                Messages.Invalid_DateRequired,
                new Dictionary<string, string[]> { ["date"] = [Messages.Invalid_DateRequired] }
            );

        var result = await showtimeService.GetWholeCinemaScheduleAsync(
            date.Value,
            cancellationToken
        );
        return Ok(result);
    }

    [HttpGet("{showtimeId:int}/seats")]
    public async Task<IActionResult> GetSeats(int showtimeId, CancellationToken cancellationToken)
    {
        var result = await showtimeService.GetShowtimeSeatsAsync(showtimeId, cancellationToken);
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

        var result = await showtimeService.GetSeatSuggestionsAsync(
            showtimeId,
            count,
            cancellationToken
        );
        return Ok(result);
    }
}

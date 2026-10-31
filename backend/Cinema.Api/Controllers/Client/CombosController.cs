using Cinema.Application.Interfaces;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Client;

[ApiController]
[Route("api/v1/combos")]
public class CombosController(IComboService comboService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCombos(CancellationToken cancellationToken) =>
        Ok(await comboService.GetActiveCombosAsync(cancellationToken));

    [HttpGet("suggestions")]
    public async Task<IActionResult> GetSuggestions(
        [FromQuery] int showtimeId,
        [FromQuery] int seatCount,
        CancellationToken cancellationToken
    )
    {
        if (showtimeId < 1 || seatCount is < 1 or > 20)
            throw new ValidationAppException(
                Messages.Invalid_ShowtimeIdAndSeatCountRequired,
                new Dictionary<string, string[]>
                {
                    ["seatCount"] = [Messages.Invalid_ShowtimeIdAndSeatCountRequired],
                }
            );

        return Ok(
            await comboService.GetComboSuggestionsAsync(showtimeId, seatCount, cancellationToken)
        );
    }
}

using System.Security.Claims;
using Cinema.Application.DTOs.Ai;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Client;

[ApiController]
[Route("api/v1/ai")]
public class AiController(IAiAssistantService aiAssistantService) : ControllerBase
{
    [HttpPost("chat")]
    public async Task<ActionResult<AiChatResponse>> Chat(
        [FromBody] AiChatRequest request,
        CancellationToken cancellationToken
    )
    {
        int? userId = null;
        var userIdClaim = User.FindFirstValue(AppClaimTypes.UserId);
        if (int.TryParse(userIdClaim, out var id))
        {
            userId = id;
        }

        var result = await aiAssistantService.ChatAsync(userId, request, cancellationToken);
        return Ok(result);
    }
}

using Cinema.Api.Filters;
using Cinema.Application.DTOs.Auth;
using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Reviews;
using Cinema.Application.DTOs.Users;
using Cinema.Application.Interfaces;
using Cinema.Shared.Resources;
using Cinema.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Client;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController(
    IUserProfileService userProfileService,
    IPasswordService passwordService,
    IReviewService reviewService
) : AppControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken) =>
        Ok(await userProfileService.GetMyProfileAsync(CurrentUserId, cancellationToken));

    [HttpPut("me")]
    [SuccessMessage(nameof(Messages.Success_ProfileUpdated))]
    public async Task<IActionResult> UpdateMyProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken
    ) =>
        Ok(
            await userProfileService.UpdateMyProfileAsync(CurrentUserId, request, cancellationToken)
        );

    [HttpGet("me/reviews")]
    public async Task<ActionResult<CursorPageResult<UserMyReviewDto>>> GetMyReviews(
        [FromQuery] string? cursor,
        [FromQuery] int? limit = null,
        CancellationToken cancellationToken = default
    )
    {
        var result = await reviewService.GetMyReviewsAsync(
            CurrentUserId,
            cursor,
            ClampLimit(limit),
            cancellationToken
        );
        return Ok(result);
    }

    [HttpPost("me/change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await passwordService.ChangePasswordAsync(
            CurrentUserId,
            request,
            cancellationToken
        );
        return Ok(ApiResponse.SuccessResponse(result, result.Message));
    }
}

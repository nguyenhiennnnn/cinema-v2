using Cinema.Application.DTOs.Auth;
using Cinema.Application.Interfaces;
using Cinema.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Cinema.Api.Controllers.Client;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(
    IAuthService authService,
    IRegistrationService registrationService,
    IPasswordService passwordService
) : AppControllerBase
{
    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await registrationService.RegisterAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(
        [FromBody] ConfirmEmailRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await registrationService.ConfirmEmailAsync(request, cancellationToken);

        return Ok(result);
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await authService.LoginAsync(request, adminOnly: false, cancellationToken);

        return Ok(result);
    }

    [HttpPost("admin-login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> AdminLogin(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await authService.LoginAsync(request, adminOnly: true, cancellationToken);
        return Ok(result);
    }

    [HttpPost("google")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> GoogleLogin(
        [FromBody] GoogleLoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await authService.GoogleLoginAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await authService.RefreshAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await authService.LogoutAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await passwordService.ForgotPasswordAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await passwordService.ResetPasswordAsync(request, cancellationToken);
        return Ok(ApiResponse.SuccessResponse(result, result.Message));
    }

    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmationEmail(
        [FromBody] ResendConfirmationEmailRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await registrationService.ResendConfirmationEmailAsync(
            request,
            cancellationToken
        );
        return Ok(result);
    }

    [HttpPost("socket-token")]
    [Authorize]
    public async Task<IActionResult> GenerateSocketToken(CancellationToken cancellationToken)
    {
        var result = await authService.GenerateSocketTokenAsync(CurrentUserId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var result = await authService.GetMeAsync(CurrentUserId, cancellationToken);
        return Ok(result);
    }
}

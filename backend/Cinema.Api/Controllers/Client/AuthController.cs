using Cinema.Api.Filters;
using Cinema.Api.Security;
using Cinema.Application.DTOs.Auth;
using Cinema.Application.Interfaces;
using Cinema.Shared.Exceptions;
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
    IPasswordService passwordService,
    RefreshCookie refreshCookie
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
    [CsrfProtection]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await authService.LoginAsync(request, adminOnly: false, cancellationToken);

        return RespondWithSession(result);
    }

    [HttpPost("admin-login")]
    [EnableRateLimiting("auth")]
    [CsrfProtection]
    public async Task<IActionResult> AdminLogin(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await authService.LoginAsync(request, adminOnly: true, cancellationToken);
        return RespondWithSession(result);
    }

    [HttpPost("google")]
    [EnableRateLimiting("auth")]
    [CsrfProtection]
    public async Task<IActionResult> GoogleLogin(
        [FromBody] GoogleLoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await authService.GoogleLoginAsync(request, cancellationToken);
        return RespondWithSession(result);
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("auth")]
    [CsrfProtection]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        try
        {
            var result = await authService.RefreshAsync(
                refreshCookie.Read(Request),
                cancellationToken
            );
            return RespondWithSession(result);
        }
        catch (UnauthorizedAppException)
        {
            refreshCookie.Clear(Response);
            throw;
        }
    }

    [HttpPost("logout")]
    [CsrfProtection]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        try
        {
            var result = await authService.LogoutAsync(
                refreshCookie.Read(Request),
                cancellationToken
            );
            return Ok(result);
        }
        finally
        {
            refreshCookie.Clear(Response);
        }
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

    private OkObjectResult RespondWithSession<T>(AuthResult<T> result)
    {
        if (result.RefreshToken is not null)
            refreshCookie.Write(Response, result.RefreshToken);

        return Ok(result.Response);
    }
}

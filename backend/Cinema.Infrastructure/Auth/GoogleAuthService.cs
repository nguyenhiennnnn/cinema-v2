using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Shared.Exceptions;
using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Infrastructure.Auth;

public class GoogleAuthService(
    IOptions<GoogleOptions> googleOptions,
    ILogger<GoogleAuthService> logger
) : IGoogleAuthService
{
    public async Task<GoogleUserInfo> VerifyIdTokenAsync(
        string idToken,
        CancellationToken cancellationToken
    )
    {
        var clientId = googleOptions.Value.ClientId;
        if (string.IsNullOrWhiteSpace(clientId))
            throw new BusinessRuleException("Google OAuth chưa được cấu hình (thiếu ClientId).");

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(
                idToken,
                new GoogleJsonWebSignature.ValidationSettings { Audience = [clientId] }
            );
        }
        catch (InvalidJwtException ex)
        {
            logger.LogWarning(ex, "GoogleIdTokenInvalid");
            throw new UnauthorizedAppException("Google ID token không hợp lệ hoặc đã hết hạn.");
        }

        return new GoogleUserInfo(
            payload.Subject,
            payload.Email,
            payload.EmailVerified,
            payload.Name,
            payload.Picture
        );
    }
}

using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Auth;

public class VerificationTokenSender(
    IAppDbContext db,
    ITokenService tokenService,
    IOptions<AppUrlOptions> appUrlOptions,
    INotificationService notificationService,
    ILogger<VerificationTokenSender> logger
) : IVerificationTokenSender
{
    public async Task IssueVerificationTokenAndSendEmailAsync(
        User user,
        VerificationTokenPurpose purpose,
        TimeSpan validFor,
        CancellationToken cancellationToken
    )
    {
        var rawToken = tokenService.GenerateOpaqueToken();
        db.UserVerificationTokens.Add(
            new UserVerificationToken
            {
                UserId = user.UserId,
                TokenHash = tokenService.HashOpaqueToken(rawToken),
                Purpose = purpose,
                ExpiresAt = DateTime.UtcNow.Add(validFor),
                Used = false,
                CreatedAt = DateTime.UtcNow,
            }
        );

        await db.SaveChangesAsync(cancellationToken);

        var link =
            $"{appUrlOptions.Value.FrontendBaseUrl.TrimEnd('/')}/{GetFrontendPath(purpose)}?token={Uri.EscapeDataString(rawToken)}";

        notificationService.SendVerificationLink(
            purpose,
            user.Email,
            user.FullName,
            link,
            validFor
        );

        logger.LogInformation(
            "SecurityEmailQueued UserId {UserId} Purpose {Purpose}",
            user.UserId,
            purpose
        );
    }

    private static string GetFrontendPath(VerificationTokenPurpose purpose) =>
        purpose switch
        {
            VerificationTokenPurpose.EmailConfirmation => "auth/confirm-email",
            VerificationTokenPurpose.PasswordReset => "auth/reset-password",
            VerificationTokenPurpose.StaffInvite => "auth/reset-password",
            _ => throw new ArgumentOutOfRangeException(nameof(purpose), purpose, null),
        };
}

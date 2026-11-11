using Cinema.Application.DTOs.Auth;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Auth;

public class PasswordService(
    IAppDbContext db,
    ITokenService tokenService,
    IPasswordHasher<User> passwordHasher,
    IOptions<EmailOptions> emailOptions,
    IVerificationTokenSender verificationTokenSender,
    ILogger<PasswordService> logger
) : IPasswordService
{
    public async Task<MessageResponse> ChangePasswordAsync(
        int userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var user =
            await db.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_User);

        if (!user.EmailConfirmed)
            throw new ForbiddenAppException(
                Messages.Forbidden_EmailNotVerifiedCheckInbox
            );

        var verifyResult = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.CurrentPassword
        );
        if (verifyResult == PasswordVerificationResult.Failed)
            throw new ValidationAppException(
                Messages.Invalid_CurrentPassword,
                new Dictionary<string, string[]>
                {
                    ["currentPassword"] = [Messages.Invalid_CurrentPassword],
                }
            );

        user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        var revokedCount = await db.RevokeActiveTokensAsync(userId, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "PasswordChanged UserId {UserId} — đã thu hồi {Count} refresh token cũ.",
            userId,
            revokedCount
        );

        return new MessageResponse(Messages.Success_PasswordChanged);
    }

    public async Task<MessageResponse> ForgotPasswordAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var standardMessage = Messages.Success_PasswordResetEmailSent;

        var user = await db.Users.FirstOrDefaultAsync(
            u => u.Email == request.Email,
            cancellationToken
        );
        if (user is null)
        {
            logger.LogInformation(
                "ForgotPassword — email không tồn tại, bỏ qua (không lộ thông tin)."
            );
            return new MessageResponse(standardMessage);
        }

        var oldTokens = await db
            .UserVerificationTokens.Where(t =>
                t.UserId == user.UserId
                && t.Purpose == VerificationTokenPurpose.PasswordReset
                && !t.Used
                && t.ExpiresAt > DateTime.UtcNow
            )
            .ToListAsync(cancellationToken);
        foreach (var t in oldTokens)
            t.Used = true;
        if (oldTokens.Count > 0)
            await db.SaveChangesAsync(cancellationToken);

        await verificationTokenSender.IssueVerificationTokenAndSendEmailAsync(
            user,
            VerificationTokenPurpose.PasswordReset,
            TimeSpan.FromMinutes(emailOptions.Value.PasswordResetExpirationMinutes),
            cancellationToken
        );

        return new MessageResponse(standardMessage);
    }

    public async Task<MessageResponse> ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var tokenHash = tokenService.HashOpaqueToken(request.Token);

        var verificationToken = await db
            .UserVerificationTokens.Include(t => t.User)
            .FirstOrDefaultAsync(
                t =>
                    t.TokenHash == tokenHash
                    && (
                        t.Purpose == VerificationTokenPurpose.PasswordReset
                        || t.Purpose == VerificationTokenPurpose.StaffInvite
                    ),
                cancellationToken
            );

        if (
            verificationToken is null
            || verificationToken.Used
            || verificationToken.ExpiresAt <= DateTime.UtcNow
        )
            throw new ValidationAppException(Messages.Invalid_TokenExpiredOrUsed);

        verificationToken.Used = true;
        verificationToken.User.PasswordHash = passwordHasher.HashPassword(
            verificationToken.User,
            request.NewPassword
        );
        verificationToken.User.UpdatedAt = DateTime.UtcNow;

        var revokedCount = await db.RevokeActiveTokensAsync(
            verificationToken.UserId,
            cancellationToken
        );

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "PasswordReset UserId {UserId} — đã thu hồi {Count} refresh token cũ.",
            verificationToken.UserId,
            revokedCount
        );

        return new MessageResponse(Messages.Success_PasswordReset);
    }
}

using Cinema.Application.DTOs.Auth;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Auth;

public class RegistrationService(
    IAppDbContext db,
    ITokenService tokenService,
    IPasswordHasher<User> passwordHasher,
    IOptions<EmailOptions> emailOptions,
    IVerificationTokenSender verificationTokenSender,
    ILogger<RegistrationService> logger
) : IRegistrationService
{
    private const int StaffInviteExpirationHours = 72;

    public async Task<RegisterResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken
    )
    {
        var emailExists = await db.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailExists)
            throw new ValidationAppException(
                Messages.Invalid_EmailExists,
                new Dictionary<string, string[]> { ["email"] = [Messages.Invalid_EmailInUse] }
            );

        var customerRole =
            await db.Roles.FirstOrDefaultAsync(r => r.RoleName == Roles.Customer, cancellationToken)
            ?? throw new BusinessRuleException(Messages.Rule_CustomerRoleNotSeeded);

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            RoleId = customerRole.RoleId,
            IsActive = true,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow,
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        db.Users.Add(user);

        var customerProfile = new CustomerProfile
        {
            User = user,
            Points = 0,
            TotalSpent = 0,
        };
        db.CustomerProfiles.Add(customerProfile);

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("UserRegistered UserId {UserId}", user.UserId);

        await verificationTokenSender.IssueVerificationTokenAndSendEmailAsync(
            user,
            VerificationTokenPurpose.EmailConfirmation,
            TimeSpan.FromHours(emailOptions.Value.EmailConfirmationExpirationHours),
            cancellationToken
        );

        return new RegisterResponse(
            user.UserId,
            user.Email,
            Messages.Success_RegistrationCheckEmail
        );
    }

    public async Task<MessageResponse> ConfirmEmailAsync(
        ConfirmEmailRequest request,
        CancellationToken cancellationToken
    )
    {
        var tokenHash = tokenService.HashOpaqueToken(request.Token);

        var verificationToken = await db
            .UserVerificationTokens.Include(t => t.User)
            .FirstOrDefaultAsync(
                t =>
                    t.TokenHash == tokenHash
                    && t.Purpose == VerificationTokenPurpose.EmailConfirmation,
                cancellationToken
            );

        if (
            verificationToken is null
            || verificationToken.Used
            || verificationToken.ExpiresAt <= DateTime.UtcNow
        )
            throw new ValidationAppException(Messages.Invalid_TokenExpiredOrUsed);

        verificationToken.Used = true;

        if (!verificationToken.User.EmailConfirmed)
        {
            verificationToken.User.EmailConfirmed = true;
            verificationToken.User.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(cancellationToken);

        return new MessageResponse(Messages.Success_EmailConfirmed);
    }

    public async Task<MessageResponse> ResendConfirmationEmailAsync(
        ResendConfirmationEmailRequest request,
        CancellationToken cancellationToken
    )
    {
        var standardMessage = Messages.Success_ConfirmationEmailSent;

        var user = await db.Users.FirstOrDefaultAsync(
            u => u.Email == request.Email,
            cancellationToken
        );

        if (user is null || user.EmailConfirmed)
        {
            logger.LogInformation(
                "ResendConfirmationEmail — email không tồn tại hoặc đã xác thực, bỏ qua."
            );
            return new MessageResponse(standardMessage);
        }

        var oldTokens = await db
            .UserVerificationTokens.Where(t =>
                t.UserId == user.UserId
                && t.Purpose == VerificationTokenPurpose.EmailConfirmation
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
            VerificationTokenPurpose.EmailConfirmation,
            TimeSpan.FromHours(emailOptions.Value.EmailConfirmationExpirationHours),
            cancellationToken
        );

        return new MessageResponse(standardMessage);
    }

    public async Task SendStaffInviteAsync(User user, CancellationToken cancellationToken)
    {
        var oldTokens = await db
            .UserVerificationTokens.Where(t =>
                t.UserId == user.UserId
                && t.Purpose == VerificationTokenPurpose.StaffInvite
                && !t.Used
                && t.ExpiresAt > DateTime.UtcNow
            )
            .ToListAsync(cancellationToken);
        foreach (var t in oldTokens)
            t.Used = true;

        await verificationTokenSender.IssueVerificationTokenAndSendEmailAsync(
            user,
            VerificationTokenPurpose.StaffInvite,
            TimeSpan.FromHours(StaffInviteExpirationHours),
            cancellationToken
        );
    }
}

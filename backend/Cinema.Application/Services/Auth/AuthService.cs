using Cinema.Application.BackgroundJobs;
using Cinema.Application.DTOs.Auth;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Auth;

public class AuthService(
    IAppDbContext db,
    ITokenService tokenService,
    IGoogleAuthService googleAuthService,
    IPasswordHasher<User> passwordHasher,
    IOptions<AppUrlOptions> appurlOptions,
    IOptions<JwtOptions> jwtOptions,
    IOptions<EmailOptions> emailOptions,
    IBackgroundJobClient backgroundJobClient,
    ILogger<AuthService> logger,
    IOptions<BrandOptions>? brandOptions = null
) : IAuthService
{
    private readonly BrandOptions _brand = brandOptions?.Value ?? new();
    private readonly int _refreshTokenExpirationDays = jwtOptions.Value.RefreshTokenExpirationDays;

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

        await IssueVerificationTokenAndSendEmailAsync(
            user,
            VerificationTokenPurpose.EmailConfirmation,
            TimeSpan.FromHours(emailOptions.Value.EmailConfirmationExpirationHours),
            "auth/confirm-email",
            $"Xác thực email — {_brand.Name}",
            "Vui lòng xác thực email bằng cách bấm vào liên kết sau",
            cancellationToken
        );

        return new RegisterResponse(user.UserId, user.Email, "Vui lòng kiểm tra email để xác thực");
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

        return new MessageResponse("Xác thực email thành công");
    }

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        bool adminOnly,
        CancellationToken cancellationToken
    )
    {
        var user = await db
            .Users.Include(u => u.Role)
            .Include(u => u.Employee)
            .Include(u => u.CustomerProfile!)
                .ThenInclude(cp => cp.Tier)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
            throw new UnauthorizedAppException(Messages.Unauthorized_InvalidCredentials);

        var verifyResult = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password
        );
        if (verifyResult == PasswordVerificationResult.Failed)
            throw new UnauthorizedAppException(Messages.Unauthorized_InvalidCredentials);

        if (!user.IsActive)
            throw new ForbiddenAppException(Messages.Forbidden_AccountLocked);

        if (!user.EmailConfirmed)
            throw new ForbiddenAppException(
                Messages.Forbidden_EmailNotVerifiedCheckInbox
            );

        var isCustomer = user.Role.RoleName == Roles.Customer;
        if (!adminOnly && !isCustomer)
            throw new ForbiddenAppException(
                Messages.Forbidden_UseAdminPortal
            );
        if (adminOnly && isCustomer)
            throw new ForbiddenAppException(
                Messages.Forbidden_CustomerCannotUseAdminPortal
            );

        if (verifyResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
            await db.SaveChangesAsync(cancellationToken);
        }

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = await IssueRefreshTokenAsync(user.UserId, cancellationToken);

        var userDto = user.Adapt<LoginUserDto>();

        return new LoginResponse(
            accessToken.AccessToken,
            refreshToken,
            accessToken.ExpiresIn,
            userDto
        );
    }

    public async Task<LoginResponse> GoogleLoginAsync(
        GoogleLoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var googleUser = await googleAuthService.VerifyIdTokenAsync(
            request.IdToken,
            cancellationToken
        );

        if (!googleUser.EmailVerified)
            throw new UnauthorizedAppException(Messages.Unauthorized_GoogleEmailNotVerified);

        var user = await db
            .Users.Include(u => u.Role)
            .Include(u => u.Employee)
            .Include(u => u.CustomerProfile!)
                .ThenInclude(cp => cp.Tier)
            .FirstOrDefaultAsync(
                u => u.GoogleId == googleUser.GoogleId || u.Email == googleUser.Email,
                cancellationToken
            );

        if (user is null)
        {
            var customerRole =
                await db.Roles.FirstOrDefaultAsync(
                    r => r.RoleName == Roles.Customer,
                    cancellationToken
                )
                ?? throw new BusinessRuleException(
                    Messages.Rule_CustomerRoleNotSeeded
                );

            user = new User
            {
                FullName = googleUser.FullName ?? googleUser.Email,
                Email = googleUser.Email,
                GoogleId = googleUser.GoogleId,
                Avatar = googleUser.Picture,
                RoleId = customerRole.RoleId,
                IsActive = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
            };

            user.PasswordHash = passwordHasher.HashPassword(
                user,
                tokenService.GenerateOpaqueToken()
            );

            db.Users.Add(user);
            db.CustomerProfiles.Add(
                new CustomerProfile
                {
                    User = user,
                    Points = 0,
                    TotalSpent = 0,
                }
            );

            await db.SaveChangesAsync(cancellationToken);

            user = await db
                .Users.Include(u => u.Role)
                .Include(u => u.Employee)
                .Include(u => u.CustomerProfile!)
                    .ThenInclude(cp => cp.Tier)
                .FirstAsync(u => u.UserId == user.UserId, cancellationToken);

            logger.LogInformation("GoogleUserRegistered UserId {UserId}", user.UserId);
        }
        else
        {
            if (!user.IsActive)
                throw new ForbiddenAppException(Messages.Forbidden_AccountLocked);

            if (user.Role.RoleName != Roles.Customer)
                throw new ForbiddenAppException(
                    Messages.Forbidden_AdminCannotUseGoogleLogin
                );

            var isDirty = false;

            if (user.GoogleId is null)
            {
                user.GoogleId = googleUser.GoogleId;
                isDirty = true;
            }

            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                isDirty = true;
            }

            if (isDirty)
            {
                user.UpdatedAt = DateTime.UtcNow;
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = await IssueRefreshTokenAsync(user.UserId, cancellationToken);
        var userDto = user.Adapt<LoginUserDto>();

        return new LoginResponse(
            accessToken.AccessToken,
            refreshToken,
            accessToken.ExpiresIn,
            userDto
        );
    }

    public async Task<RefreshResponse> RefreshAsync(
        RefreshRequest request,
        CancellationToken cancellationToken
    )
    {
        var tokenHash = tokenService.HashOpaqueToken(request.RefreshToken);

        var existing = await db
            .RefreshTokens.Include(rt => rt.User)
                .ThenInclude(u => u.Role)
            .Include(rt => rt.User)
                .ThenInclude(rt => rt.Employee)
            .Include(rt => rt.User)
                .ThenInclude(u => u.CustomerProfile!)
                    .ThenInclude(cp => cp.Tier)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

        if (existing is null)
            throw new UnauthorizedAppException(Messages.Unauthorized_RefreshTokenInvalid);

        if (existing.IsRevoked)
        {
            if (existing.ReplacedByTokenId is not null)
                await RevokeTokenChainAsync(existing.ReplacedByTokenId, cancellationToken);

            throw new UnauthorizedAppException(Messages.Unauthorized_RefreshTokenExpiredOrRevoked);
        }

        if (existing.ExpiresAt <= DateTime.UtcNow)
            throw new UnauthorizedAppException(Messages.Unauthorized_RefreshTokenExpiredOrRevoked);

        if (!existing.User.IsActive)
            throw new ForbiddenAppException(Messages.Forbidden_AccountLocked);

        if (!existing.User.EmailConfirmed)
            throw new ForbiddenAppException(Messages.Forbidden_EmailNotVerified);

        var accessToken = tokenService.GenerateAccessToken(existing.User);
        var newRefreshTokenValue = tokenService.GenerateOpaqueToken();
        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = existing.UserId,
            TokenHash = tokenService.HashOpaqueToken(newRefreshTokenValue),
            ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
        };

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        db.RefreshTokens.Add(newRefreshTokenEntity);
        existing.IsRevoked = true;
        existing.RevokedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        existing.ReplacedByTokenId = newRefreshTokenEntity.RefreshTokenId;
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var userDto = existing.User.Adapt<LoginUserDto>();

        return new RefreshResponse(
            accessToken.AccessToken,
            newRefreshTokenValue,
            userDto,
            accessToken.ExpiresIn
        );
    }

    private async Task RevokeTokenChainAsync(int? nextTokenId, CancellationToken cancellationToken)
    {
        var visited = new HashSet<int>();
        var now = DateTime.UtcNow;
        while (nextTokenId is not null && visited.Add(nextTokenId.Value))
        {
            var token = await db.RefreshTokens.FirstOrDefaultAsync(
                rt => rt.RefreshTokenId == nextTokenId,
                cancellationToken
            );
            if (token is null)
                break;

            if (!token.IsRevoked)
            {
                token.IsRevoked = true;
                token.RevokedAt = now;
            }

            nextTokenId = token.ReplacedByTokenId;
        }

        await db.SaveChangesAsync(cancellationToken);
        logger.LogWarning("RefreshTokenReuseDetected — đã thu hồi toàn bộ chuỗi refresh token.");
    }

    public async Task<MessageResponse> LogoutAsync(
        LogoutRequest request,
        CancellationToken cancellationToken
    )
    {
        var tokenHash = tokenService.HashOpaqueToken(request.RefreshToken);
        var existing = await db.RefreshTokens.FirstOrDefaultAsync(
            rt => rt.TokenHash == tokenHash,
            cancellationToken
        );

        if (existing is not null && !existing.IsRevoked)
        {
            existing.IsRevoked = true;
            existing.RevokedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
        }

        return new MessageResponse("Đăng xuất thành công");
    }

    public async Task<MeResponse> GetMeAsync(int userId, CancellationToken cancellationToken)
    {
        var user =
            await db
                .Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_User);

        return user.Adapt<MeResponse>();
    }

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

        var activeTokens = await db
            .RefreshTokens.Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);
        foreach (var rt in activeTokens)
        {
            rt.IsRevoked = true;
            rt.RevokedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "PasswordChanged UserId {UserId} — đã thu hồi {Count} refresh token cũ.",
            userId,
            activeTokens.Count
        );

        return new MessageResponse("Đổi mật khẩu thành công");
    }

    public async Task<MessageResponse> ForgotPasswordAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        const string standardMessage = "Nếu email tồn tại, hướng dẫn đặt lại mật khẩu đã được gửi";

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

        await IssueVerificationTokenAndSendEmailAsync(
            user,
            VerificationTokenPurpose.PasswordReset,
            TimeSpan.FromMinutes(emailOptions.Value.PasswordResetExpirationMinutes),
            "reset-password",
            $"Đặt lại mật khẩu — {_brand.Name}",
            "Vui lòng bấm vào liên kết sau để đặt lại mật khẩu",
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
                    t.TokenHash == tokenHash && t.Purpose == VerificationTokenPurpose.PasswordReset,
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

        var activeRefreshTokens = await db
            .RefreshTokens.Where(rt => rt.UserId == verificationToken.UserId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);
        foreach (var rt in activeRefreshTokens)
        {
            rt.IsRevoked = true;
            rt.RevokedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "PasswordReset UserId {UserId} — đã thu hồi {Count} refresh token cũ.",
            verificationToken.UserId,
            activeRefreshTokens.Count
        );

        return new MessageResponse("Đặt lại mật khẩu thành công");
    }

    public async Task<MessageResponse> ResendConfirmationEmailAsync(
        ResendConfirmationEmailRequest request,
        CancellationToken cancellationToken
    )
    {
        const string standardMessage =
            "Nếu email tồn tại và chưa xác thực, liên kết xác thực mới đã được gửi";

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

        await IssueVerificationTokenAndSendEmailAsync(
            user,
            VerificationTokenPurpose.EmailConfirmation,
            TimeSpan.FromHours(emailOptions.Value.EmailConfirmationExpirationHours),
            "auth/confirm-email",
            $"Xác thực email — {_brand.Name}",
            "Vui lòng xác thực email bằng cách bấm vào liên kết sau",
            cancellationToken
        );

        return new MessageResponse(standardMessage);
    }

    public async Task<SocketTokenResponse> GenerateSocketTokenAsync(
        int userId,
        CancellationToken cancellationToken
    )
    {
        var user =
            await db
                .Users.Include(u => u.Role)
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_User);

        var token = tokenService.GenerateSocketToken(user);
        return new SocketTokenResponse(token.AccessToken, token.ExpiresIn);
    }

    private async Task<string> IssueRefreshTokenAsync(
        int userId,
        CancellationToken cancellationToken
    )
    {
        var rawToken = tokenService.GenerateOpaqueToken();
        db.RefreshTokens.Add(
            new RefreshToken
            {
                UserId = userId,
                TokenHash = tokenService.HashOpaqueToken(rawToken),
                ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow,
            }
        );

        await db.SaveChangesAsync(cancellationToken);
        return rawToken;
    }

    private async Task IssueVerificationTokenAndSendEmailAsync(
        User user,
        VerificationTokenPurpose purpose,
        TimeSpan validFor,
        string frontendPath,
        string subject,
        string introText,
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
            $"{appurlOptions.Value.FrontendBaseUrl.TrimEnd('/')}/{frontendPath.TrimStart('/')}?token={Uri.EscapeDataString(rawToken)}";

        var buttonLabel = purpose switch
        {
            VerificationTokenPurpose.EmailConfirmation => "Xác thực email",
            VerificationTokenPurpose.PasswordReset => "Đặt lại mật khẩu",
            _ => "Tiếp tục",
        };

        var html = EmailTemplates.ActionLink(
            _brand,
            user.FullName,
            subject,
            introText,
            buttonLabel,
            link,
            DateTimeUtils.FormatDuration(validFor)
        );

        backgroundJobClient.Enqueue<SendEmailJob>(j =>
            j.RunAsync(user.Email, subject, html, CancellationToken.None)
        );

        logger.LogInformation(
            "SecurityEmailQueued UserId {UserId} Purpose {Purpose}",
            user.UserId,
            purpose
        );
    }
}

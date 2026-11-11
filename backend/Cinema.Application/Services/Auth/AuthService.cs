using Cinema.Application.DTOs.Auth;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
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
    IOptions<JwtOptions> jwtOptions,
    ILogger<AuthService> logger
) : IAuthService
{
    private readonly int _refreshTokenExpirationDays = jwtOptions.Value.RefreshTokenExpirationDays;

    private async Task<LoginUserDto> ToLoginUserDtoAsync(
        User user,
        CancellationToken cancellationToken
    )
    {
        var dto = user.Adapt<LoginUserDto>();
        if (user.CustomerProfile is null)
            return dto;

        var (points, tier) = await LoyaltyWindow.GetStandingAsync(
            db,
            user.UserId,
            DateTime.UtcNow,
            cancellationToken
        );
        return dto with { MembershipTierName = tier?.TierName, Points = points };
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

        var roleName = user.Role.RoleName;
        var isCustomer = roleName == Roles.Customer;
        var isStaff = roleName is Roles.Admin or Roles.Employee;
        if (!adminOnly && !isCustomer)
            throw new ForbiddenAppException(
                Messages.Forbidden_UseAdminPortal
            );
        if (adminOnly && !isStaff)
            throw new ForbiddenAppException(
                Messages.Forbidden_CustomerCannotUseAdminPortal
            );

        if (adminOnly && roleName == Roles.Employee && user.Employee is not { IsActive: true })
            throw new ForbiddenAppException(Messages.Forbidden_AccountLocked);

        if (verifyResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
            await db.SaveChangesAsync(cancellationToken);
        }

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = await IssueRefreshTokenAsync(user.UserId, cancellationToken);

        var userDto = await ToLoginUserDtoAsync(user, cancellationToken);

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
        var userDto = await ToLoginUserDtoAsync(user, cancellationToken);

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
        existing.Revoke();
        await db.SaveChangesAsync(cancellationToken);
        existing.ReplacedByTokenId = newRefreshTokenEntity.RefreshTokenId;
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var userDto = await ToLoginUserDtoAsync(existing.User, cancellationToken);

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

            token.Revoke(now);

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

        if (existing is not null && existing.Revoke())
            await db.SaveChangesAsync(cancellationToken);

        return new MessageResponse(Messages.Success_LoggedOut);
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
}

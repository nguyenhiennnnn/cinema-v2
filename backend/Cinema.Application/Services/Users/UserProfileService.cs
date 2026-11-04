using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Users;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Users;

public class UserProfileService(IAppDbContext db) : IUserProfileService
{
    public async Task<UserProfileDto> GetMyProfileAsync(
        int userId,
        CancellationToken cancellationToken
    )
    {
        var user =
            await db.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_User);
        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);

        return await BuildProfileDtoAsync(user, profile, cancellationToken);
    }

    public async Task<UserProfileDto> UpdateMyProfileAsync(
        int userId,
        UpdateProfileRequest request,
        CancellationToken cancellationToken
    )
    {
        var user =
            await db.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_User);

        user.FullName = request.FullName;
        user.Phone = request.Phone;
        user.Avatar = request.Avatar;
        user.DateOfBirth = request.DateOfBirth;
        user.Gender = EnumConverters.ParseNullableEnum<Gender>(request.Gender);
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);
        return await BuildProfileDtoAsync(user, profile, cancellationToken);
    }

    private async Task<CustomerProfile> GetOrCreateProfileAsync(
        int userId,
        CancellationToken cancellationToken
    )
    {
        var profile = await db.CustomerProfiles.FirstOrDefaultAsync(
            p => p.UserId == userId,
            cancellationToken
        );
        if (profile is not null)
            return profile;

        profile = new CustomerProfile
        {
            UserId = userId,
            Points = 0,
            TotalSpent = 0,
        };
        db.CustomerProfiles.Add(profile);
        await db.SaveChangesAsync(cancellationToken);
        return profile;
    }

    private async Task<UserProfileDto> BuildProfileDtoAsync(
        User user,
        CustomerProfile profile,
        CancellationToken cancellationToken
    )
    {
        // Điểm/hạng tính trực tiếp từ sổ điểm 12 tháng để khớp với mức giảm giá khi đặt vé,
        // không đọc profile.Points/TierId (bản cache, có thể cũ tới 1 giờ).
        var (points, currentTier) = await LoyaltyWindow.GetStandingAsync(
            db,
            user.UserId,
            DateTime.UtcNow,
            cancellationToken
        );
        var tierDto = currentTier?.Adapt<MembershipTierBriefDto>();

        var nextTierEntity = await db
            .MembershipTiers.Where(t => t.MinPoints > points)
            .OrderBy(t => t.MinPoints)
            .FirstOrDefaultAsync(cancellationToken);
        var nextTierDto = nextTierEntity is null
            ? null
            : new NextTierDto(
                nextTierEntity.TierId,
                nextTierEntity.TierName,
                nextTierEntity.MinPoints - points
            );

        return new UserProfileDto(
            user.UserId,
            user.FullName,
            user.Email,
            user.Phone,
            user.Avatar,
            user.DateOfBirth,
            user.Gender.ToDtoGender(),
            points,
            profile.TotalSpent,
            tierDto,
            nextTierDto
        );
    }
}

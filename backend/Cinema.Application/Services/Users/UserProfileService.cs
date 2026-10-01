using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Users;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
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
            ?? throw new NotFoundException("Không tìm thấy người dùng.");
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
            ?? throw new NotFoundException("Không tìm thấy người dùng.");

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
        MembershipTierBriefDto? tierDto = null;
        if (profile.TierId is not null)
        {
            var tier = await db.MembershipTiers.FirstOrDefaultAsync(
                t => t.TierId == profile.TierId,
                cancellationToken
            );
            if (tier is not null)
                tierDto = tier.Adapt<MembershipTierBriefDto>();
        }

        var nextTierEntity = await db
            .MembershipTiers.Where(t => t.MinPoints > profile.Points)
            .OrderBy(t => t.MinPoints)
            .FirstOrDefaultAsync(cancellationToken);
        var nextTierDto = nextTierEntity is null
            ? null
            : new NextTierDto(
                nextTierEntity.TierId,
                nextTierEntity.TierName,
                nextTierEntity.MinPoints - profile.Points
            );

        return new UserProfileDto(
            user.UserId,
            user.FullName,
            user.Email,
            user.Phone,
            user.Avatar,
            user.DateOfBirth,
            user.Gender.ToDtoGender(),
            profile.Points,
            profile.TotalSpent,
            tierDto,
            nextTierDto
        );
    }
}

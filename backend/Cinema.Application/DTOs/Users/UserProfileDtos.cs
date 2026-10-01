namespace Cinema.Application.DTOs.Users;

public record MembershipTierBriefDto(int TierId, string TierName, decimal DiscountPercent);

public record NextTierDto(int TierId, string TierName, int PointsNeeded);

public record UserProfileDto(
    int UserId,
    string FullName,
    string Email,
    string? Phone,
    string? Avatar,
    DateOnly? DateOfBirth,
    string? Gender,
    int Points,
    decimal TotalSpent,
    MembershipTierBriefDto? Tier,
    NextTierDto? NextTier
);

public record UpdateProfileRequest(
    string FullName,
    string? Phone,
    string? Avatar,
    DateOnly? DateOfBirth,
    string? Gender
);

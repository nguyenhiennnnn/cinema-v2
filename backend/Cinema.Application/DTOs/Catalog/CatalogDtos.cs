namespace Cinema.Application.DTOs.Catalog;

public record GenreDto(int GenreId, string Name);

public record LanguageDto(int LanguageId, string Name);

public record AgeRatingDto(int AgeRatingId, string Code, string? Description);

public record MembershipTierDto(
    int TierId,
    string TierName,
    int MinPoints,
    decimal DiscountPercent
);

public record DirectorDto(int DirectorId, string FullName, string? AvatarUrl);

public record ActorDto(int ActorId, string FullName, string? AvatarUrl);

public record PositionDto(int PositionId, string Name);

public record ScreenTypeDto(int ScreenTypeId, string Name);

public record SeatTypeDto(int SeatTypeId, string Name, decimal PriceMultiplier);

public record UpsertGenreRequest(string Name);

public record UpsertLanguageRequest(string Name);

public record UpsertAgeRatingRequest(string Code, string? Description);

public record UpsertPersonRequest(string FullName, string? AvatarUrl);

public record UpsertPositionRequest(string Name);

public record UpsertScreenTypeRequest(string Name);

public record UpsertSeatTypeRequest(string Name, decimal PriceMultiplier);

public record UpsertMembershipTierRequest(string TierName, int MinPoints, decimal DiscountPercent);

public record DeleteCategoryResponse(int Id, bool Deleted);

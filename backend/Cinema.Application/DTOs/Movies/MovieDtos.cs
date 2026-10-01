using Cinema.Application.DTOs.Catalog;

namespace Cinema.Application.DTOs.Movies;

public record AgeRatingCodeDto(string Code);

public record MovieListItemDto(
    int MovieId,
    string Title,
    string? PosterUrl,
    int DurationMinutes,
    string Status,
    decimal AverageRating,
    AgeRatingCodeDto? AgeRating,
    List<string> Genres
);

public record CastMemberDto(int ActorId, string FullName, string? AvatarUrl, string? CharacterName);

public record UpcomingShowtimeDto(
    int ShowtimeId,
    int RoomId,
    string RoomName,
    string? ScreenType,
    DateTime StartTime,
    DateTime EndTime,
    decimal BasePrice,
    int AvailableSeats,
    int TotalSeats
);

public record MovieDetailDto(
    int MovieId,
    string Title,
    string? OriginalTitle,
    string? Description,
    int DurationMinutes,
    DateOnly? ReleaseDate,
    string? Subtitle,
    string? PosterUrl,
    string? TrailerUrl,
    string Status,
    decimal AverageRating,
    DirectorDto? Director,
    LanguageDto? Language,
    AgeRatingDto? AgeRating,
    List<GenreDto> Genres,
    List<CastMemberDto> Cast,
    List<UpcomingShowtimeDto> UpcomingShowtimes
);

public record AdminMovieListItemDto(
    int MovieId,
    string Title,
    string? PosterUrl,
    string Status,
    DateOnly? ReleaseDate,
    string? DirectorName,
    string? LanguageName
);

public record MovieCastItemRequest(int ActorId, string? CharacterName);

public record UpsertMovieRequest(
    string Title,
    string? OriginalTitle,
    string? Description,
    int? DirectorId,
    int DurationMinutes,
    DateOnly? ReleaseDate,
    int? LanguageId,
    string? Subtitle,
    int? AgeRatingId,
    string? PosterUrl,
    string? TrailerUrl,
    string Status,
    List<int> GenreIds,
    List<MovieCastItemRequest> Cast
);

public record DeleteMovieResponse(int MovieId, bool Deleted, bool SoftDeleted);

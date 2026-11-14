namespace Cinema.Application.DTOs.Movies;

public sealed record MovieImportRow(
    string? Title,
    string? OriginalTitle,
    string? Description,
    string? Director,
    string? DurationMinutes,
    string? ReleaseDate,
    string? Language,
    string? Subtitle,
    string? AgeRating,
    string? PosterUrl,
    string? TrailerUrl,
    string? Status,
    string? Genres,
    string? Cast,
    UpsertMovieRequest? Request = null,
    int? ExistingMovieId = null
);

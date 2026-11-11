using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Movies;

namespace Cinema.Application.Interfaces;

public record MovieListFilter(
    string? Status,
    int? GenreId,
    int? LanguageId,
    int? AgeRatingId,
    string? Search
);

public record AdminMovieListFilter(
    string? Search,
    string? Status,
    int? DirectorId,
    int? LanguageId,
    int? AgeRatingId,
    int? GenreId
);

public interface IMovieService
{
    Task<CursorPageResult<MovieListItemDto>> GetMoviesAsync(
        MovieListFilter filter,
        string? cursor,
        int limit,
        CancellationToken cancellationToken
    );
    Task<MovieDetailDto> GetMovieDetailAsync(int movieId, CancellationToken cancellationToken);

    Task<OffsetPageResult<AdminMovieListItemDto>> GetMoviesAdminAsync(
        AdminMovieListFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<MovieDetailDto> CreateMovieAsync(
        UpsertMovieRequest request,
        CancellationToken cancellationToken
    );
    Task<MovieDetailDto> UpdateMovieAsync(
        int movieId,
        UpsertMovieRequest request,
        CancellationToken cancellationToken
    );
    Task<DeleteMovieResponse> DeleteMovieAsync(int movieId, CancellationToken cancellationToken);
    Task<BulkDeleteMoviesResponse> BulkDeleteMoviesAsync(
        BulkDeleteMoviesRequest request,
        CancellationToken cancellationToken
    );
}

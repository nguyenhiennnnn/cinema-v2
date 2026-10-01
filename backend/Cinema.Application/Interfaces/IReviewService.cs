using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Reviews;

namespace Cinema.Application.Interfaces;

public interface IReviewService
{
    Task<CursorPageResult<ReviewDto>> GetMovieReviewsAsync(
        int movieId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken
    );
    Task<CanReviewResponse> GetUserMovieReviewStatusAsync(
        int userId,
        int movieId,
        CancellationToken cancellationToken
    );
    Task<ReviewDto> CreateReviewAsync(
        int userId,
        int movieId,
        CreateReviewRequest request,
        CancellationToken cancellationToken
    );

    Task<ReviewDto> UpdateReviewAsync(
        int userId,
        int movieId,
        int reviewId,
        CreateReviewRequest request,
        CancellationToken cancellationToken
    );
    Task<DeleteReviewResponse> DeleteMyReviewAsync(
        int userId,
        int movieId,
        int reviewId,
        CancellationToken cancellationToken
    );

    Task<CursorPageResult<UserMyReviewDto>> GetMyReviewsAsync(
        int userId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken
    );

    Task<OffsetPageResult<AdminReviewDto>> GetReviewsAdminAsync(
        int? movieId,
        int? rating,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<DeleteReviewResponse> DeleteReviewAsync(int reviewId, CancellationToken cancellationToken);
}

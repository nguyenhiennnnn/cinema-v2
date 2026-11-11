namespace Cinema.Application.DTOs.Reviews;

public record ReviewDto(
    int ReviewId,
    int UserId,
    string UserName,
    string? UserAvatar,
    int Rating,
    string? Comment,
    DateTime CreatedAt
);

public record CreateReviewRequest(int Rating, string? Comment);

public record AdminReviewDto(
    int ReviewId,
    int MovieId,
    string MovieTitle,
    int UserId,
    string UserName,
    int Rating,
    string? Comment,
    DateTime CreatedAt
);

public record DeleteReviewResponse(int ReviewId, bool Deleted);

public record CanReviewResponse(bool CanReview);

public record UserMyReviewDto(
    int ReviewId,
    int MovieId,
    string MovieTitle,
    string? MoviePosterUrl,
    int Rating,
    string? Comment,
    DateTime CreatedAt
);

public record BulkDeleteReviewsRequest(List<int> ReviewIds);

public record BulkDeleteReviewsResultItem(int ReviewId, bool Success, string? Reason);

public record BulkDeleteReviewsResponse(
    List<BulkDeleteReviewsResultItem> Results,
    int TotalSuccess,
    int TotalFailed
);

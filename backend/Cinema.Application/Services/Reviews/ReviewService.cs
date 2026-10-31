using Cinema.Application.DTOs.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Cinema.Application.DTOs.Reviews;
using Cinema.Application.Interfaces;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Reviews;

public class ReviewService : IReviewService
{
    private readonly IAppDbContext db;
    private readonly ILogger<ReviewService> logger;

    public ReviewService(IAppDbContext db, ILogger<ReviewService>? logger = null)
    {
        this.db = db;
        this.logger = logger ?? NullLogger<ReviewService>.Instance;
    }
    public async Task<CursorPageResult<ReviewDto>> GetMovieReviewsAsync(
        int movieId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken
    )
    {
        var movieExists = await db.Movies.AnyAsync(m => m.MovieId == movieId, cancellationToken);
        if (!movieExists)
            throw new NotFoundException(Messages.NotFound_Movie);

        var query = db.Reviews.Where(r => r.MovieId == movieId);

        var cursorId = CursorCodec.Decode(cursor);
        if (cursorId is not null)
            query = query.Where(r => r.ReviewId < cursorId);
        query = query.OrderByDescending(r => r.ReviewId);

        var rows = await query
            .Take(limit + 1)
            .ProjectToType<ReviewDto>()
            .ToListAsync(cancellationToken);

        var hasMore = rows.Count > limit;
        var items = hasMore ? rows.Take(limit).ToList() : rows;
        var nextCursor = hasMore ? CursorCodec.Encode(items[^1].ReviewId) : null;

        return new CursorPageResult<ReviewDto>
        {
            Items = items,
            NextCursor = nextCursor,
            HasMore = hasMore,
        };
    }

    public async Task<CanReviewResponse> GetUserMovieReviewStatusAsync(
        int userId,
        int movieId,
        CancellationToken cancellationToken
    )
    {
        if (userId <= 0)
            return new CanReviewResponse(false);

        var alreadyReviewed = await db.Reviews.AnyAsync(
            r => r.MovieId == movieId && r.UserId == userId,
            cancellationToken
        );
        if (alreadyReviewed)
            return new CanReviewResponse(false);

        var hasCheckedIn = await db.BookingTickets.AnyAsync(
            t =>
                t.IsCheckedIn
                && t.Booking.UserId == userId
                && t.ShowtimeSeat.Showtime.MovieId == movieId,
            cancellationToken
        );

        return new CanReviewResponse(hasCheckedIn);
    }

    public async Task<ReviewDto> CreateReviewAsync(
        int userId,
        int movieId,
        CreateReviewRequest request,
        CancellationToken cancellationToken
    )
    {
        var movie =
            await db.Movies.FirstOrDefaultAsync(m => m.MovieId == movieId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Movie);

        var alreadyReviewed = await db.Reviews.AnyAsync(
            r => r.MovieId == movieId && r.UserId == userId,
            cancellationToken
        );
        if (alreadyReviewed)
            throw new ConflictAppException(Messages.Conflict_ReviewAlreadyExists);

        var hasCheckedIn = await db.BookingTickets.AnyAsync(
            t =>
                t.IsCheckedIn
                && t.Booking.UserId == userId
                && t.ShowtimeSeat.Showtime.MovieId == movieId,
            cancellationToken
        );
        if (!hasCheckedIn)
            throw new BusinessRuleException(Messages.Rule_ReviewRequiresCheckIn);

        var user = await db.Users.FirstAsync(u => u.UserId == userId, cancellationToken);
        var review = new Domain.Entities.Review
        {
            MovieId = movieId,
            UserId = userId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow,
        };
        db.Reviews.Add(review);
        await db.SaveChangesAsync(cancellationToken);

        await RecalculateMovieAverageRatingAsync(movieId, cancellationToken);

        review.User = user;
        return review.Adapt<ReviewDto>();
    }

    public async Task<ReviewDto> UpdateReviewAsync(
        int userId,
        int movieId,
        int reviewId,
        CreateReviewRequest request,
        CancellationToken cancellationToken
    )
    {
        var review =
            await db
                .Reviews.Include(r => r.User)
                .FirstOrDefaultAsync(
                    r => r.ReviewId == reviewId && r.MovieId == movieId,
                    cancellationToken
                )
            ?? throw new NotFoundException(Messages.NotFound_Review);
        if (review.UserId != userId)
            throw new ForbiddenAppException(Messages.Forbidden_NotReviewOwner);

        review.Rating = request.Rating;
        review.Comment = request.Comment;
        await db.SaveChangesAsync(cancellationToken);

        await RecalculateMovieAverageRatingAsync(movieId, cancellationToken);

        return review.Adapt<ReviewDto>();
    }

    public async Task<DeleteReviewResponse> DeleteMyReviewAsync(
        int userId,
        int movieId,
        int reviewId,
        CancellationToken cancellationToken
    )
    {
        var review =
            await db.Reviews.FirstOrDefaultAsync(
                r => r.ReviewId == reviewId && r.MovieId == movieId,
                cancellationToken
            ) ?? throw new NotFoundException(Messages.NotFound_Review);
        if (review.UserId != userId)
            throw new ForbiddenAppException(Messages.Forbidden_NotReviewOwner);

        db.Reviews.Remove(review);
        await db.SaveChangesAsync(cancellationToken);

        await RecalculateMovieAverageRatingAsync(movieId, cancellationToken);

        return new DeleteReviewResponse(reviewId, true);
    }

    public async Task<CursorPageResult<UserMyReviewDto>> GetMyReviewsAsync(
        int userId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken
    )
    {
        var query = db.Reviews.Where(r => r.UserId == userId);

        var cursorId = CursorCodec.Decode(cursor);
        if (cursorId is not null)
            query = query.Where(r => r.ReviewId < cursorId);
        query = query.OrderByDescending(r => r.ReviewId);

        var rows = await query
            .Take(limit + 1)
            .ProjectToType<UserMyReviewDto>()
            .ToListAsync(cancellationToken);

        var hasMore = rows.Count > limit;
        var items = hasMore ? rows.Take(limit).ToList() : rows;
        var nextCursor = hasMore ? CursorCodec.Encode(items[^1].ReviewId) : null;

        return new CursorPageResult<UserMyReviewDto>
        {
            Items = items,
            NextCursor = nextCursor,
            HasMore = hasMore,
        };
    }

    public async Task<OffsetPageResult<AdminReviewDto>> GetReviewsAdminAsync(
        int? movieId,
        int? rating,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.Reviews.AsQueryable();
        if (movieId is not null)
            query = query.Where(r => r.MovieId == movieId);
        if (rating is not null)
            query = query.Where(r => r.Rating == rating);
        query = query.OrderByDescending(r => r.ReviewId);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<AdminReviewDto>()
            .ToListAsync(cancellationToken);

        return OffsetPageResult<AdminReviewDto>.Create(items, page, pageSize, total);
    }

    public async Task<DeleteReviewResponse> DeleteReviewAsync(
        int reviewId,
        CancellationToken cancellationToken
    )
    {
        var review =
            await db.Reviews.FirstOrDefaultAsync(r => r.ReviewId == reviewId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Review);

        var movieId = review.MovieId;
        db.Reviews.Remove(review);
        await db.SaveChangesAsync(cancellationToken);

        await RecalculateMovieAverageRatingAsync(movieId, cancellationToken);

        return new DeleteReviewResponse(reviewId, true);
    }


    public async Task<BulkDeleteReviewsResponse> BulkDeleteReviewsAsync(
        BulkDeleteReviewsRequest request,
        CancellationToken cancellationToken
    )
    {
        var results = new List<BulkDeleteReviewsResultItem>();
        var affectedMovieIds = new HashSet<int>();
        foreach (var reviewId in request.ReviewIds.Distinct())
        {
            try
            {
                var review = await db.Reviews.FirstOrDefaultAsync(
                    r => r.ReviewId == reviewId,
                    cancellationToken
                ) ?? throw new NotFoundException(Messages.NotFound_Review);
                affectedMovieIds.Add(review.MovieId);
                db.Reviews.Remove(review);
                await db.SaveChangesAsync(cancellationToken);
                results.Add(new BulkDeleteReviewsResultItem(reviewId, true, null));
            }
            catch (NotFoundException)
            {
                results.Add(new BulkDeleteReviewsResultItem(reviewId, false, "Kh\u00f4ng t\u00ecm th\u1ea5y \u0111\u00e1nh gi\u00e1."));
            }
            catch (AppException ex)
            {
                results.Add(new BulkDeleteReviewsResultItem(reviewId, false, ex.Message));
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "BulkDeleteReview th\u1ea5t b\u1ea1i ReviewId {ReviewId}", reviewId);
                results.Add(new BulkDeleteReviewsResultItem(reviewId, false, "L\u1ed7i kh\u00f4ng x\u00e1c \u0111\u1ecbnh."));
            }
        }
        foreach (var movieId in affectedMovieIds)
            await RecalculateMovieAverageRatingAsync(movieId, cancellationToken);
        var successCount = results.Count(r => r.Success);
        return new BulkDeleteReviewsResponse(results, successCount, results.Count - successCount);
    }
    private async Task RecalculateMovieAverageRatingAsync(
        int movieId,
        CancellationToken cancellationToken
    )
    {
        var average =
            await db
                .Reviews.Where(r => r.MovieId == movieId)
                .Select(r => (decimal?)r.Rating)
                .AverageAsync(cancellationToken)
            ?? 0m;

        await db
            .Movies.Where(m => m.MovieId == movieId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(m => m.AverageRating, Math.Round(average, 2)),
                cancellationToken
            );
    }
}

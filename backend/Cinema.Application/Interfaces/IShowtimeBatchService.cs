using Cinema.Application.DTOs.Showtimes;

namespace Cinema.Application.Interfaces;

public record ShowtimeDraft(
    int MovieId,
    int RoomId,
    DateTime StartTime,
    decimal BasePrice,
    bool AllowAfterHours
);

public interface IShowtimeBatchService
{
    Task<BulkCreateShowtimesResponse> BulkCreateShowtimesAsync(
        BulkCreateShowtimesRequest request,
        CancellationToken cancellationToken
    );
    Task<BulkDeleteShowtimesResponse> BulkDeleteShowtimesAsync(
        BulkDeleteShowtimesRequest request,
        CancellationToken cancellationToken
    );
    Task<BulkCreateShowtimesResponse> CloneShowtimesAsync(
        CloneShowtimesRequest request,
        CancellationToken cancellationToken
    );

    Task<BulkCreateShowtimesResponse> CreateShowtimesBatchAsync(
        IReadOnlyList<ShowtimeDraft> drafts,
        string overlapMessage,
        CancellationToken cancellationToken
    );
}

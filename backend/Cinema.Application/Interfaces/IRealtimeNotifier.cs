using Cinema.Application.DTOs.ImportExport;

namespace Cinema.Application.Interfaces;

public interface IRealtimeNotifier
{
    Task NotifySeatHeldAsync(
        int showtimeId,
        List<int> seatIds,
        int heldByUserId,
        DateTime heldUntil,
        CancellationToken cancellationToken
    );

    Task NotifySeatReleasedAsync(
        int showtimeId,
        List<int> seatIds,
        CancellationToken cancellationToken
    );

    Task NotifySeatBookedAsync(
        int showtimeId,
        List<int> seatIds,
        CancellationToken cancellationToken
    );

    Task NotifyHoldExpiredAsync(
        int showtimeId,
        int bookingId,
        List<int> seatIds,
        CancellationToken cancellationToken
    );

    Task NotifyImportExportJobProgressAsync(
        int userId,
        ImportExportJobDto job,
        CancellationToken cancellationToken
    );

    Task NotifyImportExportJobCompletedAsync(
        int userId,
        ImportExportJobDto job,
        CancellationToken cancellationToken
    );

    Task NotifyImportExportJobFailedAsync(
        int userId,
        ImportExportJobDto job,
        CancellationToken cancellationToken
    );
}

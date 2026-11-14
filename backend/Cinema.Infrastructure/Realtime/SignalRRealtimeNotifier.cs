using Cinema.Application.DTOs.ImportExport;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.SignalR;

namespace Cinema.Infrastructure.Realtime;

public class SignalRRealtimeNotifier(IHubContext<ShowtimeHub> hubContext) : IRealtimeNotifier
{
    private static string GroupName(int showtimeId) => $"showtime-{showtimeId}";

    private static string UserGroupName(int userId) => $"user-{userId}";

    public Task NotifyHoldExpiredAsync(
        int showtimeId,
        int bookingId,
        List<int> seatIds,
        CancellationToken cancellationToken
    ) =>
        hubContext
            .Clients.Group(GroupName(showtimeId))
            .SendAsync(
                SignalREvents.HoldExpired,
                new
                {
                    showtimeId,
                    bookingId,
                    seatIds,
                },
                cancellationToken
            );

    public Task NotifySeatBookedAsync(
        int showtimeId,
        List<int> seatIds,
        CancellationToken cancellationToken
    ) =>
        hubContext
            .Clients.Group(GroupName(showtimeId))
            .SendAsync(SignalREvents.SeatBooked, new { showtimeId, seatIds }, cancellationToken);

    public Task NotifySeatHeldAsync(
        int showtimeId,
        List<int> seatIds,
        int heldByUserId,
        DateTime heldUntil,
        CancellationToken cancellationToken
    ) =>
        hubContext
            .Clients.Group(GroupName(showtimeId))
            .SendAsync(
                SignalREvents.SeatHeld,
                new
                {
                    showtimeId,
                    seatIds,
                    // Không gửi heldByUserId ra client để tránh lộ thông tin người dùng khác
                    heldUntil,
                },
                cancellationToken
            );

    public Task NotifySeatReleasedAsync(
        int showtimeId,
        List<int> seatIds,
        CancellationToken cancellationToken
    ) =>
        hubContext
            .Clients.Group(GroupName(showtimeId))
            .SendAsync(SignalREvents.SeatReleased, new { showtimeId, seatIds }, cancellationToken);

    public Task NotifyImportExportJobProgressAsync(
        int userId,
        ImportExportJobDto job,
        CancellationToken cancellationToken
    ) =>
        hubContext
            .Clients.Group(UserGroupName(userId))
            .SendAsync(SignalREvents.ImportExportJobProgress, job, cancellationToken);

    public Task NotifyImportExportJobCompletedAsync(
        int userId,
        ImportExportJobDto job,
        CancellationToken cancellationToken
    ) =>
        hubContext
            .Clients.Group(UserGroupName(userId))
            .SendAsync(SignalREvents.ImportExportJobCompleted, job, cancellationToken);

    public Task NotifyImportExportJobFailedAsync(
        int userId,
        ImportExportJobDto job,
        CancellationToken cancellationToken
    ) =>
        hubContext
            .Clients.Group(UserGroupName(userId))
            .SendAsync(SignalREvents.ImportExportJobFailed, job, cancellationToken);
}

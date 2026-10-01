using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.SignalR;

namespace Cinema.Infrastructure.Realtime;

public class SignalRRealtimeNotifier(IHubContext<ShowtimeHub> hubContext) : IRealtimeNotifier
{
    private static string GroupName(int showtimeId) => $"showtime-{showtimeId}";

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
                    heldByUserId,
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
}

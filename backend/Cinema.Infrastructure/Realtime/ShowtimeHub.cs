using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Cinema.Infrastructure.Realtime;

[Authorize]
public class ShowtimeHub : Hub
{
    private static string GroupName(int showtimeId) => $"showtime-{showtimeId}";

    public async Task JoinShowtime(int showtimeId)
    {
        if (showtimeId <= 0)
            throw new HubException("showtimeId không hợp lệ.");

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(showtimeId));
    }

    public async Task LeaveShowtime(int showtimeId)
    {
        if (showtimeId <= 0)
            throw new HubException("showtimeId không hợp lệ.");

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(showtimeId));
    }
}

using Microsoft.AspNetCore.SignalR;

namespace Cinema.Infrastructure.Realtime;

public class ShowtimeHub : Hub
{
    private static string GroupName(int showtimeId) => $"showtime-{showtimeId}";

    public async Task JoinShowtime(int showtimeId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(showtimeId));
    }

    public async Task LeaveShowtime(int showtimeId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(showtimeId));
    }
}

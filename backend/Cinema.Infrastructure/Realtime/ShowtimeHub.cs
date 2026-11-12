using Cinema.Shared.Resources;
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
            throw new HubException(Messages.Validation_ShowtimeId_Invalid);

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(showtimeId));
    }

    public async Task LeaveShowtime(int showtimeId)
    {
        if (showtimeId <= 0)
            throw new HubException(Messages.Validation_ShowtimeId_Invalid);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(showtimeId));
    }
}

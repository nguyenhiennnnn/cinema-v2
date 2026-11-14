using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Cinema.Infrastructure.Realtime;

[Authorize]
public class ShowtimeHub : Hub
{
    private static string GroupName(int showtimeId) => $"showtime-{showtimeId}";

    private static string UserGroupName(int userId) => $"user-{userId}";

    public override async Task OnConnectedAsync()
    {
        var rawUserId = Context.User?.FindFirst(AppClaimTypes.UserId)?.Value;
        if (int.TryParse(rawUserId, out var userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroupName(userId));

        await base.OnConnectedAsync();
    }

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

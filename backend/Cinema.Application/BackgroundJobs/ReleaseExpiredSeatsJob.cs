using Cinema.Application.Interfaces;

namespace Cinema.Application.BackgroundJobs;

public class ReleaseExpiredSeatsJob(ISeatExpirationService seatExpirationService)
{
    public Task RunAsync(CancellationToken cancellationToken) =>
        seatExpirationService.ExpireDueBookingsAsync(cancellationToken);
}

using Cinema.Application.Interfaces;

namespace Cinema.Application.BackgroundJobs;

public class ShowtimeLifecycleJob(IShowtimeLifecycleService showtimeLifecycleService)
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        await showtimeLifecycleService.CompleteEndedShowtimesAsync(cancellationToken);
        await showtimeLifecycleService.EndMoviesWithoutUpcomingShowtimesAsync(cancellationToken);
    }
}

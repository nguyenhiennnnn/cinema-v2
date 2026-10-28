using Cinema.Application.Interfaces;

namespace Cinema.Application.BackgroundJobs;

public class ShowtimeLifecycleJob(IShowtimeService showtimeService)
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        await showtimeService.CompleteEndedShowtimesAsync(cancellationToken);
        await showtimeService.EndMoviesWithoutUpcomingShowtimesAsync(cancellationToken);
    }
}

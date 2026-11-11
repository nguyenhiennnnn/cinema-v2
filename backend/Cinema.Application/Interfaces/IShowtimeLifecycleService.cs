namespace Cinema.Application.Interfaces;

public interface IShowtimeLifecycleService
{
    Task<int> CompleteEndedShowtimesAsync(CancellationToken cancellationToken);
    Task<int> EndMoviesWithoutUpcomingShowtimesAsync(CancellationToken cancellationToken);
}

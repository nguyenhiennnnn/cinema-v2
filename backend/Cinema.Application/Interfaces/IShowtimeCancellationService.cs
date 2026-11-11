using Cinema.Application.DTOs.Showtimes;

namespace Cinema.Application.Interfaces;

public interface IShowtimeCancellationService
{
    Task<DeleteShowtimeResponse> DeleteShowtimeAsync(
        int showtimeId,
        CancellationToken cancellationToken
    );
}

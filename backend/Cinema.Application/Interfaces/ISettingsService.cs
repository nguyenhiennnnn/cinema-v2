using Cinema.Application.DTOs.Settings;

namespace Cinema.Application.Interfaces;

public interface ISettingsService
{
    Task<CinemaSettingsDto> GetSettingsAsync(CancellationToken cancellationToken);
    Task<CinemaSettingsDto> UpdateSettingsAsync(
        UpdateSettingsRequest request,
        CancellationToken cancellationToken
    );
}

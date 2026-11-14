using Cinema.Application.DTOs.Settings;

namespace Cinema.Application.Interfaces;

public interface ISettingsService
{
    Task<CinemaSettingsDto> GetSettingsAsync(CancellationToken cancellationToken);
    Task<AdminCinemaSettingsDto> GetAdminSettingsAsync(CancellationToken cancellationToken);
    Task<AdminCinemaSettingsDto> UpdateSettingsAsync(
        UpdateSettingsRequest request,
        CancellationToken cancellationToken
    );
}

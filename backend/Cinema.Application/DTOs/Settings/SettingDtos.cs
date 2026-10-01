namespace Cinema.Application.DTOs.Settings;

public record CinemaSettingsDto(
    string Name,
    string Address,
    string? Phone,
    string? Email,
    string? TaxCode,
    string? LogoUrl,
    string? OpeningTime,
    string? ClosingTime,
    decimal PointsPerAmount,
    decimal CancelRefundPercent,
    int CancelDeadlineHours
);

public record UpdateSettingsRequest(
    string Name,
    string Address,
    string? Phone,
    string? Email,
    string? TaxCode,
    string? LogoUrl,
    string? OpeningTime,
    string? ClosingTime,
    decimal PointsPerAmount,
    decimal CancelRefundPercent,
    int CancelDeadlineHours
);

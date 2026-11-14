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
    int CancelDeadlineHours,
    int StopSellingMinutesBefore,
    int HoldDurationMinutes,
    int CheckInOpenMinutes,
    string SeatGapRuleMode,
    int RoomBufferMinutes,
    int MaxSeatsPerBooking,
    int MaxComboQuantity,
    int LoyaltyWindowMonths,
    int MovieEndGraceDays,
    int ShowtimeLookaheadDays,
    int SchedulerStepMinutes,
    int SchedulerMinSameMovieGapMinutes,
    int SchedulerHeatLookbackDays,
    double SchedulerHeatSalesWeight,
    double SchedulerHeatRatingWeight,
    double SchedulerMaxRating
)
{
    public string TimeZoneId { get; init; } = string.Empty;

    public string Locale { get; init; } = string.Empty;

    public string CurrencyCode { get; init; } = string.Empty;

    public string CurrencySymbol { get; init; } = string.Empty;
}

/// <summary>
/// Chỉ trả qua endpoint admin. TUYỆT ĐỐI không dùng cho endpoint công khai
/// (GET /api/v1/settings) vì chứa mật khẩu mặc định của nhân viên.
/// </summary>
public record AdminCinemaSettingsDto : CinemaSettingsDto
{
    public AdminCinemaSettingsDto(CinemaSettingsDto source, string defaultEmployeePassword)
        : base(source)
    {
        DefaultEmployeePassword = defaultEmployeePassword;
    }

    public string DefaultEmployeePassword { get; init; }
}

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
    int CancelDeadlineHours,
    int StopSellingMinutesBefore = 0,
    int? HoldDurationMinutes = null,
    int? CheckInOpenMinutes = null,
    string? SeatGapRuleMode = null,
    int? RoomBufferMinutes = null,
    int? MaxSeatsPerBooking = null,
    int? MaxComboQuantity = null,
    int? LoyaltyWindowMonths = null,
    int? MovieEndGraceDays = null,
    int? ShowtimeLookaheadDays = null,
    int? SchedulerStepMinutes = null,
    int? SchedulerMinSameMovieGapMinutes = null,
    int? SchedulerHeatLookbackDays = null,
    double? SchedulerHeatSalesWeight = null,
    double? SchedulerHeatRatingWeight = null,
    double? SchedulerMaxRating = null,
    string? DefaultEmployeePassword = null
);

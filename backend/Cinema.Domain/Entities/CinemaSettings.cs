using Cinema.Domain.Enums;

namespace Cinema.Domain.Entities;

public class CinemaSettings
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxCode { get; set; }
    public string? LogoUrl { get; set; }
    public TimeOnly? OpeningTime { get; set; }
    public TimeOnly? ClosingTime { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public decimal PointsPerAmount { get; set; } = 10000m;

    public decimal CancelRefundPercent { get; set; } = 100m;

    public int CancelDeadlineHours { get; set; } = 2;

    public int StopSellingMinutesBefore { get; set; }

    public const int DefaultHoldDurationMinutes = 15;
    public const int DefaultCheckInOpenMinutes = 30;

    public int HoldDurationMinutes { get; set; } = DefaultHoldDurationMinutes;

    public int CheckInOpenMinutes { get; set; } = DefaultCheckInOpenMinutes;

    public SeatGapRuleMode SeatGapRuleMode { get; set; } = SeatGapRuleMode.Strict;

    public const int DefaultRoomBufferMinutes = 15;
    public const int DefaultMaxSeatsPerBooking = 10;
    public const int DefaultMaxComboQuantity = 10;
    public const int DefaultLoyaltyWindowMonths = 12;
    public const int DefaultMovieEndGraceDays = 1;
    public const int DefaultShowtimeLookaheadDays = 7;

    public int RoomBufferMinutes { get; set; } = DefaultRoomBufferMinutes;

    public int MaxSeatsPerBooking { get; set; } = DefaultMaxSeatsPerBooking;

    public int MaxComboQuantity { get; set; } = DefaultMaxComboQuantity;

    public int LoyaltyWindowMonths { get; set; } = DefaultLoyaltyWindowMonths;

    public int MovieEndGraceDays { get; set; } = DefaultMovieEndGraceDays;

    public int ShowtimeLookaheadDays { get; set; } = DefaultShowtimeLookaheadDays;

    public const int DefaultSchedulerStepMinutes = 5;
    public const int DefaultSchedulerMinSameMovieGapMinutes = 60;
    public const int DefaultSchedulerHeatLookbackDays = 7;
    public const double DefaultSchedulerHeatSalesWeight = 0.7;
    public const double DefaultSchedulerHeatRatingWeight = 0.3;
    public const double DefaultSchedulerMaxRating = 5;

    public int SchedulerStepMinutes { get; set; } = DefaultSchedulerStepMinutes;

    public int SchedulerMinSameMovieGapMinutes { get; set; } = DefaultSchedulerMinSameMovieGapMinutes;

    public int SchedulerHeatLookbackDays { get; set; } = DefaultSchedulerHeatLookbackDays;

    public double SchedulerHeatSalesWeight { get; set; } = DefaultSchedulerHeatSalesWeight;

    public double SchedulerHeatRatingWeight { get; set; } = DefaultSchedulerHeatRatingWeight;

    public double SchedulerMaxRating { get; set; } = DefaultSchedulerMaxRating;
}

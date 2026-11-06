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

    public decimal CancelRefundPercent { get; set; } = 0m;

    public int CancelDeadlineHours { get; set; } = 2;

    public int StopSellingMinutesBefore { get; set; }

    public const int DefaultHoldDurationMinutes = 15;
    public const int DefaultCheckInOpenMinutes = 30;

    public int HoldDurationMinutes { get; set; } = DefaultHoldDurationMinutes;

    public int CheckInOpenMinutes { get; set; } = DefaultCheckInOpenMinutes;

    public SeatGapRuleMode SeatGapRuleMode { get; set; } = SeatGapRuleMode.Strict;

    // ── Lịch chiếu & đặt vé ──────────────────────────────────────────────
    public const int DefaultRoomBufferMinutes = 15;
    public const int DefaultMaxSeatsPerBooking = 10;
    public const int DefaultMaxComboQuantity = 10;
    public const int DefaultLoyaltyWindowMonths = 12;
    public const int DefaultMovieEndGraceDays = 1;
    public const int DefaultShowtimeLookaheadDays = 7;

    /// <summary>Khoảng nghỉ (dọn phòng) cộng vào sau mỗi suất chiếu, tính bằng phút.</summary>
    public int RoomBufferMinutes { get; set; } = DefaultRoomBufferMinutes;

    /// <summary>Số ghế tối đa cho một lần giữ chỗ / booking.</summary>
    public int MaxSeatsPerBooking { get; set; } = DefaultMaxSeatsPerBooking;

    /// <summary>Số lượng tối đa cho mỗi loại combo trong một booking.</summary>
    public int MaxComboQuantity { get; set; } = DefaultMaxComboQuantity;

    /// <summary>Điểm thưởng chỉ có hiệu lực trong N tháng kể từ lúc tích lũy.</summary>
    public int LoyaltyWindowMonths { get; set; } = DefaultLoyaltyWindowMonths;

    /// <summary>Số ngày gia hạn sau suất chiếu cuối cùng trước khi phim tự chuyển sang Ended.</summary>
    public int MovieEndGraceDays { get; set; } = DefaultMovieEndGraceDays;

    /// <summary>Số ngày lịch chiếu hiển thị cho khách (trang phim, trợ lý AI).</summary>
    public int ShowtimeLookaheadDays { get; set; } = DefaultShowtimeLookaheadDays;

    // ── Xếp lịch tự động (auto-schedule) ─────────────────────────────────
    public const int DefaultSchedulerStepMinutes = 5;
    public const int DefaultSchedulerMinSameMovieGapMinutes = 60;
    public const int DefaultSchedulerHeatLookbackDays = 7;
    public const double DefaultSchedulerHeatSalesWeight = 0.7;
    public const double DefaultSchedulerHeatRatingWeight = 0.3;
    public const double DefaultSchedulerMaxRating = 5;

    /// <summary>Bước thời gian (phút) khi dò giờ bắt đầu suất.</summary>
    public int SchedulerStepMinutes { get; set; } = DefaultSchedulerStepMinutes;

    /// <summary>Khoảng cách tối thiểu (phút) giữa hai suất cùng phim.</summary>
    public int SchedulerMinSameMovieGapMinutes { get; set; } = DefaultSchedulerMinSameMovieGapMinutes;

    /// <summary>Số ngày gần nhất dùng để tính doanh số vé khi tính Heat.</summary>
    public int SchedulerHeatLookbackDays { get; set; } = DefaultSchedulerHeatLookbackDays;

    /// <summary>Trọng số doanh số vé trong công thức Heat.</summary>
    public double SchedulerHeatSalesWeight { get; set; } = DefaultSchedulerHeatSalesWeight;

    /// <summary>Trọng số điểm đánh giá trong công thức Heat.</summary>
    public double SchedulerHeatRatingWeight { get; set; } = DefaultSchedulerHeatRatingWeight;

    /// <summary>Điểm đánh giá tối đa (mẫu số khi chuẩn hóa rating trong Heat).</summary>
    public double SchedulerMaxRating { get; set; } = DefaultSchedulerMaxRating;
}

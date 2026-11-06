namespace Cinema.Application.Options;

public class SchedulerOptions
{
    public const string SectionName = "Scheduler";

    // Các thuộc tính chính sách bên dưới (StepMinutes, MinSameMovieGapMinutes, HeatLookbackDays,
    // HeatSalesWeight, HeatRatingWeight, MaxRating) được ShowtimeService ghi đè bằng CinemaSettings;
    // giá trị ở đây chỉ là mặc định khi dùng ShowtimeScheduler độc lập (ví dụ trong test).
    public int StepMinutes { get; set; } = 5;
    public int MinSameMovieGapMinutes { get; set; } = 60;
    public int DefaultWindowMinutes { get; set; } = 16 * 60;
    public double SpreadMinutes { get; set; } = 30;
    public double FitWeight { get; set; } = 4;
    public double LoadWeight { get; set; } = 0.5;
    public int HeatLookbackDays { get; set; } = 7;

    // Heat = SalesWeight * (vé bán / vé bán cao nhất) + RatingWeight * (điểm đánh giá / MaxRating)
    public double HeatSalesWeight { get; set; } = 0.7;
    public double HeatRatingWeight { get; set; } = 0.3;
    public double MaxRating { get; set; } = 5;
}

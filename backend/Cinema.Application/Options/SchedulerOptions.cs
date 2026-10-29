namespace Cinema.Application.Options;

public class SchedulerOptions
{
    public const string SectionName = "Scheduler";

    public int StepMinutes { get; set; } = 5;
    public int MinSameMovieGapMinutes { get; set; } = 60;
    public int DefaultWindowMinutes { get; set; } = 16 * 60;
    public double SpreadMinutes { get; set; } = 30;
    public double FitWeight { get; set; } = 4;
    public double LoadWeight { get; set; } = 0.5;
    public int HeatLookbackDays { get; set; } = 7;
}

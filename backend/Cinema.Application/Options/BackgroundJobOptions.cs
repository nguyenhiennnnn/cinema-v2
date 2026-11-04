namespace Cinema.Application.Options;

public class BackgroundJobOptions
{
    public const string SectionName = "BackgroundJobs";

    public string ReleaseExpiredSeatsCron { get; set; } = "* * * * *";
    public string ShowtimeLifecycleCron { get; set; } = "* * * * *";
    public string ExpireLoyaltyPointsCron { get; set; } = "5 * * * *";
    public int RetryAttempts { get; set; } = 3;
    public int[] RetryDelaysSeconds { get; set; } = [30, 120, 600];
}

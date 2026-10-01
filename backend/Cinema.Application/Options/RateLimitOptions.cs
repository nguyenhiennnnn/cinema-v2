namespace Cinema.Application.Options;

public class RateLimitOptions
{
    public const string SectionName = "RateLimit";

    public int GlobalPermitLimit { get; set; } = 100;
    public int GlobalWindowSeconds { get; set; } = 60;

    public int AuthPermitLimit { get; set; } = 5;
    public int AuthWindowSeconds { get; set; } = 60;

    public int BookingHoldPermitLimit { get; set; } = 10;
    public int BookingHoldWindowSeconds { get; set; } = 60;
}

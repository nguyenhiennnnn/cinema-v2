namespace Cinema.Application.Options;

public class StripeOptions
{
    public const string SectionName = "Stripe";

    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = "http://localhost:5173/booking/result";
    public string Currency { get; set; } = "vnd";
}

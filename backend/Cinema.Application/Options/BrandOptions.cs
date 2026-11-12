
namespace Cinema.Application.Options;

public class BrandOptions
{
    public const string SectionName = "Brand";

    public string Name { get; set; } = "Perfo";
    public string DisplayName { get; set; } = "PERFO";
    public string Tagline { get; set; } = string.Empty;
    public string SupportEmail { get; set; } = string.Empty;

    public string TimeZoneId { get; set; } = string.Empty;

    public string Locale { get; set; } = string.Empty;

    public string CurrencyCode { get; set; } = string.Empty;

    public string CurrencySymbol { get; set; } = string.Empty;

    public const string DefaultPhoneRegex = @"^(0|\+84)[0-9]{9,10}$";

    public string PhoneRegex { get; set; } = DefaultPhoneRegex;

    public const string DefaultBookingCodePrefix = "BK";

    public string BookingCodePrefix { get; set; } = DefaultBookingCodePrefix;
}

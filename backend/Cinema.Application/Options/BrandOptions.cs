namespace Cinema.Application.Options;

public class BrandOptions
{
    public const string SectionName = "Brand";

    public string Name { get; set; } = "Perfo";
    public string DisplayName { get; set; } = "PERFO";
    public string Tagline { get; set; } = "Vé điện tử · Không cần in";
    public string SupportEmail { get; set; } = string.Empty;

    public string TimeZoneId { get; set; } = "Asia/Ho_Chi_Minh";

    public string Locale { get; set; } = "vi-VN";

    public string CurrencyCode { get; set; } = "VND";

    public string BookingCodePrefix { get; set; } = "BK";
}

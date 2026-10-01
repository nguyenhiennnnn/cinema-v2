namespace Cinema.Application.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "Cinema";
    public string Audience { get; set; } = "Cinema.Client";
    public string Secret { get; set; } = string.Empty;
    public int AccessTokenExpirationSeconds { get; set; } = 3600;
    public int RefreshTokenExpirationDays { get; set; } = 30;
}

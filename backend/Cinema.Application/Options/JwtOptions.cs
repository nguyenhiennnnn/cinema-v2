namespace Cinema.Application.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "Perfo";
    public string Audience { get; set; } = "Perfo.Client";
    public string Secret { get; set; } = string.Empty;
    public int AccessTokenExpirationSeconds { get; set; } = 3600;
    public int RefreshTokenExpirationDays { get; set; } = 30;
    public int SocketTokenExpirationSeconds { get; set; } = 300;
    public int ClockSkewSeconds { get; set; } = 30;
}

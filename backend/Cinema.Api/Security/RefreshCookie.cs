using Cinema.Application.Options;
using Microsoft.Extensions.Options;

namespace Cinema.Api.Security;

public class RefreshCookieOptions
{
    public const string SectionName = "Auth:RefreshCookie";

    public string Name { get; set; } = "perfo_refresh_token";
    public string Path { get; set; } = "/api/v1/auth";
    public string? Domain { get; set; }
    public string SameSite { get; set; } = "Lax";
    public bool Secure { get; set; } = true;

    public SameSiteMode ResolveSameSite() =>
        Enum.TryParse<SameSiteMode>(SameSite, ignoreCase: true, out var mode)
            ? mode
            : throw new InvalidOperationException(
                $"{SectionName}:SameSite must be Strict, Lax or None."
            );
}

public class RefreshCookie(
    IOptions<RefreshCookieOptions> cookieOptions,
    IOptions<JwtOptions> jwtOptions
)
{
    private readonly RefreshCookieOptions _options = cookieOptions.Value;
    private readonly JwtOptions _jwt = jwtOptions.Value;

    public string? Read(HttpRequest request) =>
        request.Cookies.TryGetValue(_options.Name, out var value)
        && !string.IsNullOrWhiteSpace(value)
            ? value
            : null;

    public void Write(HttpResponse response, string token)
    {
        var options = Build();
        options.MaxAge = TimeSpan.FromDays(_jwt.RefreshTokenExpirationDays);
        response.Cookies.Append(_options.Name, token, options);
    }

    public void Clear(HttpResponse response) => response.Cookies.Delete(_options.Name, Build());

    private CookieOptions Build() =>
        new()
        {
            HttpOnly = true,
            Secure = _options.Secure,
            SameSite = _options.ResolveSameSite(),
            Path = _options.Path,
            Domain = string.IsNullOrWhiteSpace(_options.Domain) ? null : _options.Domain,
            IsEssential = true,
        };
}

public static class RefreshCookieExtensions
{
    public static IServiceCollection AddRefreshCookie(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var section = configuration.GetSection(RefreshCookieOptions.SectionName);
        var options = section.Get<RefreshCookieOptions>() ?? new RefreshCookieOptions();

        if (options.ResolveSameSite() == SameSiteMode.None && !options.Secure)
            throw new InvalidOperationException(
                $"{RefreshCookieOptions.SectionName}: SameSite=None requires Secure=true."
            );

        if (!options.Path.StartsWith('/'))
            throw new InvalidOperationException(
                $"{RefreshCookieOptions.SectionName}:Path must start with '/'."
            );

        services.Configure<RefreshCookieOptions>(section);
        services.AddScoped<RefreshCookie>();
        return services;
    }
}

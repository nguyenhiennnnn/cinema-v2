using Cinema.Application.Options;

namespace Cinema.Api.Extensions;

public static class CorsExtensions
{
    public const string PolicyName = "Default";
    private const string AllowedOriginsKey = "Cors:AllowedOrigins";

    private static readonly string[] ExposedHeaders = ["Date", "Content-Disposition"];
    private static readonly TimeSpan PreflightMaxAge = TimeSpan.FromMinutes(10);

    public static string[] GetCorsOrigins(this IConfiguration configuration) =>
        configuration.GetSection(AllowedOriginsKey).Get<string[]>() ?? [];

    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var corsOrigins = configuration.GetCorsOrigins();
        if (corsOrigins.Length == 0)
        {
            var frontend = configuration
                .BindSection<AppUrlOptions>(AppUrlOptions.SectionName)
                .FrontendBaseUrl;
            corsOrigins = Uri.TryCreate(frontend, UriKind.Absolute, out var uri)
                ? [uri.GetLeftPart(UriPartial.Authority)]
                : [];
        }

        return services.AddCors(options =>
        {
            options.AddPolicy(
                PolicyName,
                policy =>
                    policy
                        .WithOrigins(corsOrigins)
                        .WithExposedHeaders(ExposedHeaders)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .SetPreflightMaxAge(PreflightMaxAge)
            );
        });
    }
}

namespace Cinema.Api.Extensions;

public static class CorsExtensions
{
    public const string PolicyName = "Default";
    private const string AllowedOriginsKey = "Cors:AllowedOrigins";

    private static readonly string[] ExposedHeaders = ["Date"];

    public static string[] GetCorsOrigins(this IConfiguration configuration) =>
        configuration.GetSection(AllowedOriginsKey).Get<string[]>() ?? [];

    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var corsOrigins = configuration.GetCorsOrigins();

        return services.AddCors(options =>
        {
            options.AddPolicy(
                PolicyName,
                policy =>
                {
                    if (corsOrigins.Length > 0)
                        policy
                            .WithOrigins(corsOrigins)
                            .WithExposedHeaders(ExposedHeaders)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    else
                        policy
                            .WithExposedHeaders(ExposedHeaders)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowed(_ => true)
                            .AllowCredentials();
                }
            );
        });
    }
}

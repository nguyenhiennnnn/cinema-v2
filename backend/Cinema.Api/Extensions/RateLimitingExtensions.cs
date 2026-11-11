using System.Threading.RateLimiting;
using Cinema.Application.Options;

namespace Cinema.Api.Extensions;

public static class RateLimitingExtensions
{
    public const string AuthPolicy = "auth";
    public const string BookingHoldPolicy = "booking-hold";

    public static IServiceCollection AddRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var rateLimitOptions = configuration.BindSection<RateLimitOptions>(
            RateLimitOptions.SectionName
        );

        return services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: ClientIp(httpContext),
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = rateLimitOptions.GlobalPermitLimit,
                            Window = TimeSpan.FromSeconds(rateLimitOptions.GlobalWindowSeconds),
                            SegmentsPerWindow = rateLimitOptions.GlobalSegmentsPerWindow,
                            QueueLimit = 0,
                        }
                    )
            );

            options.AddPolicy(
                AuthPolicy,
                httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ClientIp(httpContext),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = rateLimitOptions.AuthPermitLimit,
                            Window = TimeSpan.FromSeconds(rateLimitOptions.AuthWindowSeconds),
                            QueueLimit = 0,
                        }
                    )
            );

            options.AddPolicy(
                BookingHoldPolicy,
                httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? ClientIp(httpContext),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = rateLimitOptions.BookingHoldPermitLimit,
                            Window = TimeSpan.FromSeconds(
                                rateLimitOptions.BookingHoldWindowSeconds
                            ),
                            QueueLimit = 0,
                        }
                    )
            );
        });
    }

    private static string ClientIp(HttpContext httpContext) =>
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}

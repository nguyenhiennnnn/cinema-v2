using Cinema.Application.BackgroundJobs;
using Cinema.Application.Options;
using Cinema.Infrastructure.Persistence;
using Cinema.Shared.Constants;
using Hangfire;
using Hangfire.PostgreSql;

namespace Cinema.Api.Extensions;

public static class BackgroundJobExtensions
{
    public static IServiceCollection AddBackgroundJobs(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment
    )
    {
        if (environment.IsEnvironment("Testing"))
            return services;

        var connectionString = configuration.GetPostgresConnectionString();
        var options = configuration.BindSection<BackgroundJobOptions>(
            BackgroundJobOptions.SectionName
        );

        services.AddHangfire(config =>
            config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(o => o.UseNpgsqlConnection(connectionString))
        );
        services.AddHangfireServer();
        GlobalJobFilters.Filters.Add(
            new AutomaticRetryAttribute
            {
                Attempts = options.RetryAttempts,
                DelaysInSeconds = options.RetryDelaysSeconds,
            }
        );

        return services;
    }

    public static WebApplication UseRecurringJobs(this WebApplication app)
    {
        if (app.Environment.IsEnvironment("Testing"))
            return app;

        var options = app.Configuration.BindSection<BackgroundJobOptions>(
            BackgroundJobOptions.SectionName
        );
        var jobOptions = new RecurringJobOptions { TimeZone = Cinema.Application.Utils.BusinessTime.Zone };

        RecurringJob.AddOrUpdate<ReleaseExpiredSeatsJob>(
            RecurringJobIds.ReleaseExpiredSeats,
            j => j.RunAsync(CancellationToken.None),
            options.ReleaseExpiredSeatsCron,
            jobOptions
        );
        RecurringJob.AddOrUpdate<ShowtimeLifecycleJob>(
            RecurringJobIds.UpdateShowtimeLifecycle,
            j => j.RunAsync(CancellationToken.None),
            options.ShowtimeLifecycleCron,
            jobOptions
        );
        RecurringJob.AddOrUpdate<ExpireLoyaltyPointsJob>(
            RecurringJobIds.ExpireLoyaltyPoints,
            j => j.RunAsync(CancellationToken.None),
            options.ExpireLoyaltyPointsCron,
            jobOptions
        );

        return app;
    }
}

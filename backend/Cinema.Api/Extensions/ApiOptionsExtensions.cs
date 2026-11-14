using Cinema.Application.Options;
using Microsoft.AspNetCore.HttpOverrides;

namespace Cinema.Api.Extensions;

public static class ApiOptionsExtensions
{
    public static IServiceCollection AddApiOptions(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        return services
            .AddSection<BookingOptions>(configuration, BookingOptions.SectionName)
            .AddSection<JwtOptions>(configuration, JwtOptions.SectionName)
            .AddSection<RateLimitOptions>(configuration, RateLimitOptions.SectionName)
            .AddSection<CloudinaryOptions>(configuration, CloudinaryOptions.SectionName)
            .AddSection<EmailOptions>(configuration, EmailOptions.SectionName)
            .AddSection<VnPayOptions>(configuration, VnPayOptions.SectionName)
            .AddSection<MomoOptions>(configuration, MomoOptions.SectionName)
            .AddSection<StripeOptions>(configuration, StripeOptions.SectionName)
            .AddSection<AppUrlOptions>(configuration, AppUrlOptions.SectionName)
            .AddSection<GoogleOptions>(configuration, GoogleOptions.SectionName)
            .AddSection<BrandOptions>(configuration, BrandOptions.SectionName)
            .AddSection<SchedulerOptions>(configuration, SchedulerOptions.SectionName)
            .AddSection<BackgroundJobOptions>(configuration, BackgroundJobOptions.SectionName)
            .AddSection<ImportExportOptions>(configuration, ImportExportOptions.SectionName)
            .AddSection<PdfOptions>(configuration, PdfOptions.SectionName);
    }

    public static T BindSection<T>(this IConfiguration configuration, string sectionName)
        where T : new() => configuration.GetSection(sectionName).Get<T>() ?? new T();

    private static IServiceCollection AddSection<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName
    )
        where T : class => services.Configure<T>(configuration.GetSection(sectionName));
}

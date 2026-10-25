using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Infrastructure.Auth;
using Cinema.Infrastructure.Email;
using Cinema.Infrastructure.FileStorage;
using Cinema.Infrastructure.Payments;
using Cinema.Infrastructure.Persistence;
using Cinema.Infrastructure.Realtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cinema.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");

        var env = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";
        var isDev = string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase);

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var conn =
                config.GetConnectionString("DefaultConnection")
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");

            var env = config["ASPNETCORE_ENVIRONMENT"] ?? "Development";
            var isDev = string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase);

            options.UseNpgsql(
                conn,
                sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            );

            options.ConfigureWarnings(w =>
                w.Ignore(
                    Microsoft
                        .EntityFrameworkCore
                        .Diagnostics
                        .RelationalEventId
                        .PendingModelChangesWarning
                )
            );

            if (isDev)
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<IFileStorage, CloudinaryFileStorage>();

        // Chọn email provider dựa trên config:
        // - Nếu có Email:ResendApiKey → dùng Resend HTTP API (hoạt động trên Render Free Tier)
        // - Ngược lại → dùng MailKit SMTP (local / paid hosting)
        var resendApiKey = configuration["Email:ResendApiKey"];
        if (!string.IsNullOrWhiteSpace(resendApiKey))
        {
            services.AddHttpClient("Resend");
            services.AddScoped<IEmailService, ResendEmailService>();
        }
        else
        {
            services.AddScoped<IEmailService, MailKitEmailService>();
        }

        services.AddScoped<IRealtimeNotifier, SignalRRealtimeNotifier>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();

        services.AddHttpClient<IPaymentGateway, VnPayGateway>();
        services.AddHttpClient<IPaymentGateway, MomoGateway>();
        services.AddScoped<IPaymentGateway, StripeGateway>();

        return services;
    }
}

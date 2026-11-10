using Cinema.Application.MappingConfig;
using Cinema.Application.Options;
using Cinema.Api.Filters;
using Cinema.Infrastructure.Persistence.Seed;
using Cinema.Shared.Resources;
using Cinema.Shared.Results;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Cinema.Infrastructure.Persistence;

namespace Cinema.Api.Extensions;

public static class ApiStartupExtensions
{
    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services
            .AddControllers(options =>
            {
                options.Filters.Add<ApiResponseFilter>();
                options.Filters.Add<ValidationActionFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new Cinema.Application.Utils.DateOnlyJsonConverter()
                );
                options.JsonSerializerOptions.Converters.Add(
                    new Cinema.Application.Utils.DateTimeJsonConverter()
                );
            });

        services.AddValidatorsFromAssembly(typeof(MapsterConfig).Assembly);

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context
                    .ModelState.Where(kv => kv.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kv => char.ToLowerInvariant(kv.Key[0]) + kv.Key[1..],
                        kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return new BadRequestObjectResult(
                    ApiResponse.ErrorResponse(Messages.Common_InvalidData, errors)
                );
            };
        });

        services.AddMapsterConfig();

        return services;
    }

    public static WebApplicationBuilder AddApiLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(
            (context, services, loggerConfig) =>
                loggerConfig
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File("logs/perfo-.log", rollingInterval: RollingInterval.Day)
        );
        return builder;
    }

    public static WebApplicationBuilder UsePortFromEnvironment(this WebApplicationBuilder builder)
    {
        var port = Environment.GetEnvironmentVariable("PORT");
        if (!string.IsNullOrWhiteSpace(port))
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
        return builder;
    }

    public static WebApplicationBuilder ValidateProductionOptions(this WebApplicationBuilder builder)
    {
        if (!builder.Environment.IsProduction())
            return builder;

        var configuration = builder.Configuration;
        var productionErrors = ProductionOptionsValidator.Validate(
            configuration.BindSection<AppUrlOptions>(AppUrlOptions.SectionName),
            configuration.BindSection<JwtOptions>(JwtOptions.SectionName),
            configuration.GetCorsOrigins(),
            configuration.BindSection<VnPayOptions>(VnPayOptions.SectionName),
            configuration.BindSection<MomoOptions>(MomoOptions.SectionName),
            configuration.BindSection<StripeOptions>(StripeOptions.SectionName)
        );
        if (productionErrors.Count > 0)
            throw new InvalidOperationException(
                "Cấu hình Production không hợp lệ:"
                    + string.Concat(productionErrors.Select(e => $"\n- {e}"))
            );

        return builder;
    }

    public static async Task MigrateAndSeedAsync(this WebApplication app)
    {
        if (!app.Configuration.GetValue("RunMigrationsOnStartup", true))
            return;

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        try
        {
            await dbContext.Database.MigrateAsync();
            await DbInitializer.SeedAsync(dbContext, app.Logger);
        }
        catch (Exception ex)
        {
            app.Logger.LogError(
                ex,
                "Lỗi trong quá trình Database Migration hoặc Seeding khi khởi động."
            );
            throw;
        }
    }
}

using Cinema.Api.Filters;
using Cinema.Api.Middlewares;
using Cinema.Infrastructure.Realtime;
using Cinema.Shared.Constants;
using Hangfire;
using Serilog;

namespace Cinema.Api.Extensions;

public static class ApiPipelineExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        app.UseForwardedHeaders();

        app.Use(
            async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            }
        );

        app.UseSerilogRequestLogging();

        app.UseAppExceptionHandling();

        app.UseApiStatusCodePages();

        app.UseSwaggerDocs();

        if (app.Environment.IsDevelopment())
            app.UseHttpsRedirection();

        app.UseCors(CorsExtensions.PolicyName);

        if (!app.Environment.IsEnvironment("Testing"))
            app.UseRateLimiter();

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapControllers();

        if (!app.Environment.IsEnvironment("Testing"))
        {
            app.MapHangfireDashboard(
                ApiPaths.HangfireDashboard,
                new DashboardOptions { Authorization = [new HangfireAdminAuthorizationFilter()] }
            );
        }

        app.MapHub<ShowtimeHub>(ApiPaths.ShowtimeHub);

        return app;
    }
}

using Cinema.Shared.Constants;
using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Cinema.Api.Filters;

public class HangfireAdminAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        var env = context
            .GetHttpContext()
            .RequestServices.GetRequiredService<IWebHostEnvironment>();
        if (env.IsDevelopment())
        {
            return true;
        }

        var httpContext = context.GetHttpContext();
        return httpContext.User.Identity?.IsAuthenticated == true
            && httpContext.User.IsInRole(Roles.Admin);
    }
}

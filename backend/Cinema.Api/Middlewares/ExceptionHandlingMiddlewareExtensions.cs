namespace Cinema.Api.Middlewares;

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseAppExceptionHandling(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlingMiddleware>();
}

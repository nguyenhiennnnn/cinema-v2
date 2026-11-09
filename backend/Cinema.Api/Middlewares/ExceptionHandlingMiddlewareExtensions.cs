namespace Cinema.Api.Middlewares;

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseAppExceptionHandling(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlingMiddleware>();

    public static IApplicationBuilder UseApiStatusCodePages(this IApplicationBuilder app) =>
        app.UseStatusCodePages(ApiErrorWriter.WriteStatusCodeAsync);
}

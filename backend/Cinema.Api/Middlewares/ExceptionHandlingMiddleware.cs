namespace Cinema.Api.Middlewares;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment env
)
{
    private const int ClientClosedRequest = 499;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            logger.LogDebug(
                "Request cancelled by client: {Method} {Path}",
                context.Request.Method,
                context.Request.Path
            );
            if (!context.Response.HasStarted)
                context.Response.StatusCode = ClientClosedRequest;
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                logger.LogError(
                    ex,
                    "Exception after response started on [{Method} {Path}], cannot write error body",
                    context.Request.Method,
                    context.Request.Path
                );
                throw;
            }

            var error = ApiErrorMapper.Map(ex, env.IsDevelopment());

            logger.Log(
                error.LogLevel,
                ex,
                "{ExceptionType} on [{Method} {Path}{Query}] -> {StatusCode}: {Message} | Inner: {InnerMessage}",
                ex.GetType().FullName,
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                error.StatusCode,
                ex.Message,
                ex.InnerException?.Message ?? "None"
            );

            await ApiErrorWriter.WriteAsync(context, error);
        }
    }
}

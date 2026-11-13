using System.Text.Encodings.Web;
using System.Text.Json;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Cinema.Shared.Results;
using Microsoft.AspNetCore.Diagnostics;

namespace Cinema.Api.Middlewares;

public static class ApiErrorWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private static readonly string[] SkippedPathPrefixes = [ApiPaths.HangfireDashboard, "/swagger"];

    public static Task WriteAsync(HttpContext context, ApiError error) =>
        WriteAsync(
            context,
            error.StatusCode,
            ApiResponse.ErrorResponse(error.Message, error.Errors)
        );

    public static Task WriteAsync(HttpContext context, int statusCode, ApiResponse response)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        var json = JsonSerializer.Serialize(response, JsonOptions);
        return context.Response.WriteAsync(json);
    }

    public static Task WriteStatusCodeAsync(StatusCodeContext statusCodeContext)
    {
        var context = statusCodeContext.HttpContext;

        if (context.RequestAborted.IsCancellationRequested)
            return Task.CompletedTask;

        if (SkippedPathPrefixes.Any(p => context.Request.Path.StartsWithSegments(p)))
            return Task.CompletedTask;

        var message = DefaultMessageFor(context.Response.StatusCode);
        return message is null
            ? Task.CompletedTask
            : WriteAsync(context, context.Response.StatusCode, ApiResponse.ErrorResponse(message));
    }

    private static string? DefaultMessageFor(int statusCode) =>
        statusCode switch
        {
            StatusCodes.Status400BadRequest => Messages.System_BadRequest,
            StatusCodes.Status401Unauthorized => Messages.Unauthorized_Required,
            StatusCodes.Status403Forbidden => Messages.Forbidden_AccessDenied,
            StatusCodes.Status404NotFound => Messages.NotFound_Resource,
            StatusCodes.Status405MethodNotAllowed => Messages.System_MethodNotAllowed,
            StatusCodes.Status429TooManyRequests => Messages.System_TooManyRequests,
            >= StatusCodes.Status500InternalServerError => Messages.System_UnexpectedError,
            _ => null,
        };
}

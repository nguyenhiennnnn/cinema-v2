using System.Text.Encodings.Web;
using System.Text.Json;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Results;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Cinema.Api.Middlewares;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment env
)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppException ex)
        {
            logger.LogWarning(ex, "Handled application exception: {Message}", ex.Message);

            if (ex is SeatConflictException seatEx)
            {
                // Return ApiResponse with conflictSeatIds encoded in errors dict.
                // Frontend reads: errors.conflictSeatIds (string[] of seat IDs)
                var errors = new Dictionary<string, string[]>
                {
                    ["conflictSeatIds"] = seatEx.ConflictSeatIds
                        .Select(id => id.ToString())
                        .ToArray(),
                };
                await WriteResponse(
                    context,
                    seatEx.StatusCode,
                    ApiResponse.ErrorResponse(seatEx.Message, errors)
                );
                return;
            }

            await WriteResponse(
                context,
                ex.StatusCode,
                ApiResponse.ErrorResponse(ex.Message, ex.Errors)
            );
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.LogWarning(
                ex,
                "Concurrency conflict detected: {Message}",
                ex.Message
            );
            await WriteResponse(
                context,
                StatusCodes.Status409Conflict,
                ApiResponse.ErrorResponse("Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng tải lại và thử lại.")
            );
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            // PostgreSQL error codes: https://www.postgresql.org/docs/current/errcodes-appendix.html
            // 23503 = foreign_key_violation
            // 23505 = unique_violation
            // 23514 = check_violation
            var (status, message) = pgEx.SqlState switch
            {
                "23503" => (
                    StatusCodes.Status400BadRequest,
                    "Dữ liệu tham chiếu (khóa ngoại) không hợp lệ."
                ),
                "23505" => (
                    StatusCodes.Status409Conflict,
                    "Dữ liệu đã tồn tại (vi phạm ràng buộc duy nhất)."
                ),
                "23514" => (
                    StatusCodes.Status400BadRequest,
                    "Dữ liệu không thỏa mãn ràng buộc kiểm tra."
                ),
                _ => (StatusCodes.Status400BadRequest, "Lỗi ràng buộc dữ liệu."),
            };

            logger.LogWarning(
                ex,
                "Database constraint violation, SqlState {SqlState}, Detail: {Detail}",
                pgEx.SqlState,
                pgEx.MessageText
            );
            await WriteResponse(context, status, ApiResponse.ErrorResponse(message));
        }
        catch (Exception ex)
        {
            var requestPath = $"{context.Request.Method} {context.Request.Path}{context.Request.QueryString}";
            logger.LogError(
                ex,
                "🔥 Unhandled Exception on [{RequestPath}]: {ExceptionType} - {Message} | Inner: {InnerMessage}",
                requestPath,
                ex.GetType().FullName,
                ex.Message,
                ex.InnerException?.Message ?? "None"
            );

            var message = env.IsDevelopment()
                ? $"[DEV ONLY] {ex.GetType().Name}: {ex.Message}"
                : "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.";

            var errors = env.IsDevelopment()
                ? new Dictionary<string, string[]>
                {
                    ["exceptionType"] = [ex.GetType().FullName ?? ex.GetType().Name],
                    ["message"] = [ex.Message],
                    ["innerException"] = ex.InnerException is not null ? [ex.InnerException.Message] : [],
                    ["stackTrace"] = ex.StackTrace?.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                                                  .Select(s => s.Trim())
                                                  .Take(8)
                                                  .ToArray() ?? [],
                }
                : null;

            await WriteResponse(
                context,
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse(message, errors)
            );
        }
    }

    private static Task WriteResponse(HttpContext context, int statusCode, ApiResponse response)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        var json = JsonSerializer.Serialize(response, JsonOptions);
        return context.Response.WriteAsync(json);
    }
}

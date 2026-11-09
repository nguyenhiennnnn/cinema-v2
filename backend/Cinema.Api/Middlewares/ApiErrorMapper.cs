using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Cinema.Api.Middlewares;

public static class ApiErrorMapper
{
    private const string ShowtimeOverlapConstraint = "EX_Showtimes_Room_NoOverlap";

    public static ApiError Map(Exception exception, bool includeDebugDetails) =>
        exception switch
        {
            AppException app => new ApiError(app.StatusCode, app.Message, app.Errors),

            DbUpdateConcurrencyException => new ApiError(
                StatusCodes.Status409Conflict,
                Messages.Db_ConcurrencyConflict
            ),

            DbUpdateException { InnerException: PostgresException pg } => MapPostgres(pg),

            BadHttpRequestException bad => new ApiError(
                bad.StatusCode,
                bad.StatusCode == StatusCodes.Status413PayloadTooLarge
                    ? Messages.Common_InvalidData
                    : Messages.System_BadRequest
            ),

            _ => Unexpected(exception, includeDebugDetails),
        };

    private static ApiError MapPostgres(PostgresException pg)
    {
        var (status, message) = pg.SqlState switch
        {
            PostgresErrorCodes.ForeignKeyViolation => (
                StatusCodes.Status400BadRequest,
                Messages.Db_ForeignKeyViolation
            ),
            PostgresErrorCodes.UniqueViolation => (
                StatusCodes.Status409Conflict,
                Messages.Db_UniqueViolation
            ),
            PostgresErrorCodes.CheckViolation => (
                StatusCodes.Status400BadRequest,
                Messages.Db_CheckViolation
            ),
            PostgresErrorCodes.ExclusionViolation => (
                StatusCodes.Status409Conflict,
                pg.ConstraintName == ShowtimeOverlapConstraint
                    ? Messages.Conflict_ShowtimeOverlapInRoom
                    : Messages.Db_ExclusionViolation
            ),
            _ => (StatusCodes.Status400BadRequest, Messages.Db_ConstraintError),
        };

        return new ApiError(status, message);
    }

    private static ApiError Unexpected(Exception ex, bool includeDebugDetails)
    {
        if (!includeDebugDetails)
        {
            return new ApiError(
                StatusCodes.Status500InternalServerError,
                Messages.System_UnexpectedError,
                LogLevel: LogLevel.Error
            );
        }

        var errors = new Dictionary<string, string[]>
        {
            ["exceptionType"] = [ex.GetType().FullName ?? ex.GetType().Name],
            ["message"] = [ex.Message],
            ["innerException"] = ex.InnerException is not null ? [ex.InnerException.Message] : [],
            ["stackTrace"] =
                ex.StackTrace?.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Take(8)
                    .ToArray()
                ?? [],
        };

        return new ApiError(
            StatusCodes.Status500InternalServerError,
            $"[DEV ONLY] {ex.GetType().Name}: {ex.Message}",
            errors,
            LogLevel.Error
        );
    }
}

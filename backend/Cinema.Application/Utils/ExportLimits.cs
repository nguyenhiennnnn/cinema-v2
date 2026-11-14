using System.Globalization;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Utils;

public static class ExportLimits
{
    public const int MaxRows = 20_000;

    public static Task<int> CountAsync<T>(
        IQueryable<T> query,
        CancellationToken cancellationToken
    ) => query.Take(MaxRows + 1).CountAsync(cancellationToken);

    public static async Task EnsureWithinLimitAsync<T>(
        IQueryable<T> query,
        CancellationToken cancellationToken
    ) => EnsureWithinLimit(await CountAsync(query, cancellationToken));

    public static void EnsureWithinLimit(int count)
    {
        if (count <= MaxRows)
            return;

        var message = string.Format(
            Messages.Export_TooManyRows,
            MaxRows.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"))
        );
        throw new ValidationAppException(
            message,
            new Dictionary<string, string[]> { ["filter"] = [message] }
        );
    }
}

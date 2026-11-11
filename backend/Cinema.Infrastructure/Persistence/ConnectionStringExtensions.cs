using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Cinema.Infrastructure.Persistence;

public static class ConnectionStringExtensions
{
    public static string GetPostgresConnectionString(this IConfiguration configuration)
    {
        var raw =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");

        raw = raw.Trim().Trim('"').Trim('\'');

        if (
            !raw.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
            && !raw.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)
        )
            return raw;

        var queryIndex = raw.IndexOf('?');
        var baseUri = queryIndex >= 0 ? raw[..queryIndex] : raw;
        var queryString = queryIndex >= 0 ? raw[(queryIndex + 1)..] : string.Empty;

        var uri = new Uri(baseUri);
        var userInfo = uri.UserInfo.Split(':', 2);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : null,
            Database = uri.AbsolutePath.TrimStart('/'),
        };

        if (!string.IsNullOrEmpty(queryString))
        {
            var sslMode = queryString
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Split('=', 2))
                .Where(p =>
                    p.Length == 2 && p[0].Equals("sslmode", StringComparison.OrdinalIgnoreCase)
                )
                .Select(p => Uri.UnescapeDataString(p[1]))
                .FirstOrDefault();

            if (
                !string.IsNullOrEmpty(sslMode)
                && Enum.TryParse<Npgsql.SslMode>(sslMode, ignoreCase: true, out var parsedSslMode)
            )
            {
                builder.SslMode = parsedSslMode;
            }
        }

        return builder.ConnectionString;
    }
}

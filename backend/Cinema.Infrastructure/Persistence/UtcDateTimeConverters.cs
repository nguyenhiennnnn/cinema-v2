using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cinema.Infrastructure.Persistence;

public sealed class UtcDateTimeConverter()
    : ValueConverter<DateTime, DateTime>(value => ToStoredUtc(value), value => AsUtc(value))
{
    // Npgsql maps "timestamp without time zone" columns to DateTime with Kind=Unspecified.
    // We normalise every incoming value to UTC then strip the Kind flag so that
    // Npgsql is happy both at runtime and when generating EnsureCreated/seed SQL literals.
    internal static DateTime ToStoredUtc(DateTime value)
    {
        var utc = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
        };
        // Strip Kind so Npgsql does not complain about UTC DateTime on a
        // "timestamp without time zone" column.
        return DateTime.SpecifyKind(utc, DateTimeKind.Unspecified);
    }

    internal static DateTime AsUtc(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
        };
}

public sealed class NullableUtcDateTimeConverter()
    : ValueConverter<DateTime?, DateTime?>(
        value => value.HasValue ? UtcDateTimeConverter.ToStoredUtc(value.Value) : value,
        value => value.HasValue ? UtcDateTimeConverter.AsUtc(value.Value) : value
    );

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cinema.Infrastructure.Persistence;

public sealed class UtcDateTimeConverter()
    : ValueConverter<DateTime, DateTime>(value => ToStoredUtc(value), value => AsUtc(value))
{
    internal static DateTime ToStoredUtc(DateTime value) =>
        value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : AsUtc(value);

    internal static DateTime AsUtc(DateTime value) =>
        value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
}

public sealed class NullableUtcDateTimeConverter()
    : ValueConverter<DateTime?, DateTime?>(
        value => value.HasValue ? UtcDateTimeConverter.ToStoredUtc(value.Value) : value,
        value => value.HasValue ? UtcDateTimeConverter.AsUtc(value.Value) : value
    );

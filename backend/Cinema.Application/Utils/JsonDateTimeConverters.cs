using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cinema.Application.Utils;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
            return default;

        if (DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;

        if (DateOnly.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            return date;

        if (DateOnly.TryParse(value, out date))
            return date;

        throw new JsonException($"Không thể chuyển đổi '{value}' sang DateOnly.");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}

public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
            return default;

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt))
            return dt.ToUniversalTime();

        if (DateTime.TryParseExact(value, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);

        if (DateTime.TryParseExact(value, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);

        if (DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);

        if (DateTime.TryParse(value, out dt))
            return dt.ToUniversalTime();

        throw new JsonException($"Không thể chuyển đổi '{value}' sang DateTime UTC.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var utc = value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
        writer.WriteStringValue(utc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture));
    }
}

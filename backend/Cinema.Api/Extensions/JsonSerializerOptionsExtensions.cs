using System.Text.Json;
using Cinema.Application.Utils;

namespace Cinema.Api.Extensions;

public static class JsonSerializerOptionsExtensions
{
    public static JsonSerializerOptions AddCinemaConverters(this JsonSerializerOptions options)
    {
        options.Converters.Add(new DateOnlyJsonConverter());
        options.Converters.Add(new DateTimeJsonConverter());
        return options;
    }
}

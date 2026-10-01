using System.Text;
using System.Text.Json;

namespace Cinema.Application.DTOs.Common;

public class CursorPageResult<T>
{
    public List<T> Items { get; init; } = [];
    public string? NextCursor { get; init; }
    public bool HasMore { get; init; }
}

public class OffsetPageResult<T>
{
    public List<T> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }

    public static OffsetPageResult<T> Create(
        List<T> items,
        int page,
        int pageSize,
        int totalItems
    ) =>
        new()
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = pageSize > 0 ? (int)Math.Ceiling(totalItems / (double)pageSize) : 0,
        };
}

public static class CursorCodec
{
    private record CursorPayload(int Id);

    public static string Encode(int id)
    {
        var json = JsonSerializer.Serialize(new CursorPayload(id));
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    public static int? Decode(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
            return null;
        try
        {
            var bytes = Convert.FromBase64String(cursor);
            var payload = JsonSerializer.Deserialize<CursorPayload>(bytes);

            return payload?.Id;
        }
        catch
        {
            return null;
        }
    }
}

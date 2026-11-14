using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.Extensions.Caching.Memory;

namespace Cinema.Application.Services.Import;

public class ImportSessionStore(IMemoryCache cache) : IImportSessionStore
{
    public ImportSession<TRow> Create<TRow>(
        int userId,
        IReadOnlyList<string> headers,
        IReadOnlyList<ImportRow<TRow>> rows
    )
    {
        var session = new ImportSession<TRow>(Guid.NewGuid().ToString("N"), userId, headers, rows);
        cache.Set(
            CacheKey(session.PreviewId),
            session,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ImportLimits.PreviewTtl,
            }
        );
        return session;
    }

    public ImportSession<TRow> Get<TRow>(string previewId, int userId)
    {
        if (
            cache.TryGetValue(CacheKey(previewId), out var cached)
            && cached is ImportSession<TRow> session
            && session.UserId == userId
        )
            return session;

        throw new NotFoundException(Messages.NotFound_ImportPreview);
    }

    public void Remove(string previewId) => cache.Remove(CacheKey(previewId));

    private static string CacheKey(string previewId) => $"import:{previewId}";
}

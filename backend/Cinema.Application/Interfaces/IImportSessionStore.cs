using Cinema.Application.Services.Import;

namespace Cinema.Application.Interfaces;

public interface IImportSessionStore
{
    ImportSession<TRow> Create<TRow>(
        int userId,
        string? fileName,
        ImportFileFormat format,
        IReadOnlyList<string> headers,
        IReadOnlyList<ImportRow<TRow>> rows
    );

    ImportSession<TRow> Get<TRow>(string previewId, int userId);

    void Remove(string previewId);
}

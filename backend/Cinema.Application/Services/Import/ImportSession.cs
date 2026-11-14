using Cinema.Application.Interfaces;

namespace Cinema.Application.Services.Import;

public sealed class ImportSession<TRow>(
    string previewId,
    int userId,
    IReadOnlyList<string> headers,
    IReadOnlyList<ImportRow<TRow>> rows
)
{
    private int applying;

    public string PreviewId { get; } = previewId;
    public int UserId { get; } = userId;
    public IReadOnlyList<string> Headers { get; } = headers;
    public IReadOnlyList<ImportRow<TRow>> Rows { get; } = rows;

    public bool TryBeginApply() => Interlocked.CompareExchange(ref applying, 1, 0) == 0;

    public void EndApply() => Interlocked.Exchange(ref applying, 0);
}

using Cinema.Application.DTOs.Common;

namespace Cinema.Application.DTOs.Import;

public record ImportCellError(string Column, string Message);

public record ImportPreviewRow(
    int RowNumber,
    IReadOnlyDictionary<string, string?> Data,
    List<ImportCellError> Errors
);

public record ImportPreviewResponse(
    string PreviewId,
    int TotalRows,
    int ValidRows,
    int InvalidRows,
    List<ImportPreviewRow> Rows
);

public record ImportApplyRequest(string PreviewId, bool SkipInvalidRows);

public record ImportApplyOutcome(BulkActionResponse? Result, Guid? JobId);

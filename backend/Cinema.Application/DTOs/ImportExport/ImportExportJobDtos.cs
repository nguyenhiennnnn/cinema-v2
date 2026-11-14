namespace Cinema.Application.DTOs.ImportExport;

public record ImportExportJobDto(
    Guid JobId,
    string Type,
    string Module,
    string Format,
    string Status,
    int Progress,
    int TotalRows,
    int SuccessRows,
    int FailedRows,
    string? FileName,
    string? ErrorMessage,
    int CreatedByUserId,
    DateTime CreatedAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    DateTime? ExpiresAt,
    bool IsExpired,
    bool CanDownload
);

public record ImportExportJobAcceptedDto(Guid JobId);

public record ImportExportJobListFilter(string? Type, string? Module, string? Status);

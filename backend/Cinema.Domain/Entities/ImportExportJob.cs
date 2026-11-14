using Cinema.Domain.Enums;

namespace Cinema.Domain.Entities;

public class ImportExportJob
{
    public Guid JobId { get; set; }
    public ImportExportJobType Type { get; set; }
    public string Module { get; set; } = null!;
    public string Format { get; set; } = null!;
    public ImportExportJobStatus Status { get; set; }
    public int Progress { get; set; }
    public int TotalRows { get; set; }
    public int SuccessRows { get; set; }
    public int FailedRows { get; set; }
    public string? FileName { get; set; }
    public string? StoragePath { get; set; }
    public string? FileUrl { get; set; }
    public string? ErrorMessage { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public User CreatedByUser { get; set; } = null!;
}

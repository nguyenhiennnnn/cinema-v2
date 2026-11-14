namespace Cinema.Application.Options;

public class ImportExportOptions
{
    public const string SectionName = "ImportExport";

    public int ExportBackgroundThreshold { get; set; } = 5000;
    public int ImportBackgroundThreshold { get; set; } = 500;
    public int ExportProgressBatchSize { get; set; } = 1000;
    public int ImportProgressBatchSize { get; set; } = 50;
    public int FileRetentionHours { get; set; } = 24;
    public int CleanupBatchSize { get; set; } = 100;
    public string StorageFolder { get; set; } = "import-export";
    public int[] SuperAdminUserIds { get; set; } = [];
}

using Cinema.Application.Interfaces;

namespace Cinema.Application.Services.ImportExport;

public sealed class JobProgressReporter(
    IImportExportJobTracker tracker,
    Guid jobId,
    int totalRows,
    int batchSize
) : IJobProgressReporter
{
    private const int MaxRunningProgress = 99;

    private readonly int _batchSize = Math.Max(1, batchSize);
    private int _lastProgress = -1;

    public int Processed { get; private set; }

    public Task ReportAsync(int processedRows, CancellationToken cancellationToken)
    {
        Processed = processedRows;

        if (processedRows < totalRows && processedRows % _batchSize != 0)
            return Task.CompletedTask;

        var progress =
            totalRows <= 0
                ? 0
                : (int)Math.Min(MaxRunningProgress, processedRows * 100L / totalRows);
        if (progress == _lastProgress)
            return Task.CompletedTask;

        _lastProgress = progress;
        return tracker.ReportProgressAsync(jobId, progress, cancellationToken);
    }
}

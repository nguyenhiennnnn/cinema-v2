using Cinema.Application.Interfaces;

namespace Cinema.Application.Services.ImportExport;

public sealed class RowCountReporter : IJobProgressReporter
{
    public int Count { get; private set; }

    public Task ReportAsync(int processedRows, CancellationToken cancellationToken)
    {
        Count = processedRows;
        return Task.CompletedTask;
    }
}

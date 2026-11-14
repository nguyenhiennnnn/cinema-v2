namespace Cinema.Application.Interfaces;

public interface IJobProgressReporter
{
    Task ReportAsync(int processedRows, CancellationToken cancellationToken);
}

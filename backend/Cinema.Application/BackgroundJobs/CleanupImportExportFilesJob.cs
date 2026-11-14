using Cinema.Application.Interfaces;

namespace Cinema.Application.BackgroundJobs;

public class CleanupImportExportFilesJob(IImportExportJobService service)
{
    public Task RunAsync(CancellationToken cancellationToken) =>
        service.CleanupExpiredFilesAsync(cancellationToken);
}

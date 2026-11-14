using Cinema.Application.DTOs.Common;
using Cinema.Application.Utils;

namespace Cinema.Application.Interfaces;

public interface IExportSource<TFilter>
{
    string Module { get; }

    Task<int> CountAsync(TFilter filter, CancellationToken cancellationToken);

    Task<ExportFileDto> ExportAsync(
        TFilter filter,
        ExportFormat format,
        IJobProgressReporter? progress,
        CancellationToken cancellationToken
    );
}

public interface IExportModule
{
    string Module { get; }

    Task<int> CountAsync(string filterJson, CancellationToken cancellationToken);

    Task<ExportFileDto> ExportAsync(
        string filterJson,
        ExportFormat format,
        IJobProgressReporter? progress,
        CancellationToken cancellationToken
    );
}

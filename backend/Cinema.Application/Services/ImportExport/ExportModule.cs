using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;

namespace Cinema.Application.Services.ImportExport;

public class ExportModule<TFilter>(IExportSource<TFilter> source) : IExportModule
{
    public string Module => source.Module;

    public Task<int> CountAsync(string filterJson, CancellationToken cancellationToken) =>
        source.CountAsync(ImportExportJson.Deserialize<TFilter>(filterJson), cancellationToken);

    public Task<ExportFileDto> ExportAsync(
        string filterJson,
        ExportFormat format,
        IJobProgressReporter? progress,
        CancellationToken cancellationToken
    ) =>
        source.ExportAsync(
            ImportExportJson.Deserialize<TFilter>(filterJson),
            format,
            progress,
            cancellationToken
        );
}

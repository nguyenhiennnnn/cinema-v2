using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Import;
using Cinema.Application.DTOs.Rooms;
using Cinema.Application.Utils;

namespace Cinema.Application.Interfaces;

public interface ISeatLayoutImportService
{
    Task<ExportFileDto> GetTemplateAsync(int roomId, CancellationToken cancellationToken);

    Task<ImportPreviewResponse> PreviewAsync(
        int roomId,
        int userId,
        string? fileName,
        Stream content,
        CancellationToken cancellationToken
    );

    Task<UpdateSeatLayoutResponse> ApplyAsync(
        int roomId,
        int userId,
        ImportApplyRequest request,
        CancellationToken cancellationToken
    );

    Task<ExportFileDto> GetErrorFileAsync(
        int roomId,
        int userId,
        string previewId,
        CancellationToken cancellationToken
    );

    Task<ExportFileDto> ExportAsync(
        int roomId,
        ExportFormat format,
        CancellationToken cancellationToken
    );
}

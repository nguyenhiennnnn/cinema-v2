using Cinema.Application.DTOs.Common;
using Cinema.Application.Utils;

namespace Cinema.Application.Interfaces;

public interface IPaymentExportService
{
    Task<ExportFileDto> ExportPaymentsAsync(
        AdminPaymentListFilter filter,
        ExportFormat format,
        CancellationToken cancellationToken
    );
}

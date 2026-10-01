using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Payments;

namespace Cinema.Application.Interfaces;

public record AdminPaymentListFilter(
    string? Status,
    string? PaymentMethod,
    DateOnly? FromDate,
    DateOnly? ToDate,
    string? Search
);

public interface IPaymentAdminService
{
    Task<OffsetPageResult<AdminPaymentListItemDto>> GetPaymentsAsync(
        AdminPaymentListFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<AdminPaymentDetailDto> GetPaymentDetailAsync(
        int paymentId,
        CancellationToken cancellationToken
    );
}

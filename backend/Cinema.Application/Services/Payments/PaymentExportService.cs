using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Payments;

public class PaymentExportService(IAppDbContext db, IExportWriter exportWriter)
    : IPaymentExportService
{
    private const string Module = "payments";

    public async Task<ExportFileDto> ExportPaymentsAsync(
        AdminPaymentListFilter filter,
        ExportFormat format,
        CancellationToken cancellationToken
    )
    {
        var query = db.Payments.AsNoTracking().ApplyAdminFilter(filter);
        await ExportLimits.EnsureWithinLimitAsync(query, cancellationToken);

        var rows = query
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.PaymentId)
            .Take(ExportLimits.MaxRows)
            .Select(p => new PaymentExportRow(
                p.Booking.BookingCode,
                p.PaymentMethod,
                p.TransactionId,
                p.Amount,
                p.Status,
                p.PaidAt,
                p.RefundTransactionId,
                p.RefundedAmount,
                p.RefundedAt
            ))
            .AsAsyncEnumerable();

        return await ExportFileBuilder.BuildAsync(
            exportWriter,
            Module,
            Messages.Export_SheetPayments,
            format,
            Columns(),
            rows,
            cancellationToken
        );
    }

    private static IReadOnlyList<ExportColumn<PaymentExportRow>> Columns() =>
        [
            new(Messages.Export_ColBookingCode, x => x.BookingCode),
            new(Messages.Export_ColPaymentMethod, x => ExportLabels.Method(x.Method)),
            new(Messages.Export_ColTransactionId, x => x.TransactionId),
            new(Messages.Export_ColAmount, x => x.Amount, ExportColumnType.Money),
            new(Messages.Export_ColStatus, x => ExportLabels.Payment(x.Status)),
            new(Messages.Export_ColPaidAt, x => x.PaidAt, ExportColumnType.DateTime),
            new(Messages.Export_ColRefundTransactionId, x => x.RefundTransactionId),
            new(Messages.Export_ColRefundedAmount, x => x.RefundedAmount, ExportColumnType.Money),
            new(Messages.Export_ColRefundedAt, x => x.RefundedAt, ExportColumnType.DateTime),
        ];

    private sealed record PaymentExportRow(
        string BookingCode,
        PaymentMethod Method,
        string? TransactionId,
        decimal Amount,
        PaymentStatus Status,
        DateTime? PaidAt,
        string? RefundTransactionId,
        decimal? RefundedAmount,
        DateTime? RefundedAt
    );
}

using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Payments;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Payments;

public class PaymentAdminService(IAppDbContext db) : IPaymentAdminService
{
    public async Task<OffsetPageResult<AdminPaymentListItemDto>> GetPaymentsAsync(
        AdminPaymentListFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.Payments.Include(p => p.Booking).AsQueryable();

        if (EnumConverters.TryParseEnum<PaymentStatus>(filter.Status, out var status))
            query = query.Where(p => p.Status == status);
        if (EnumConverters.TryParseEnum<PaymentMethod>(filter.PaymentMethod, out var method))
            query = query.Where(p => p.PaymentMethod == method);
        if (filter.NeedsRefund == true)
            query = query.Where(p =>
                p.Status == PaymentStatus.Success
                && p.PaymentMethod != PaymentMethod.Cash
                && p.Booking.Status == BookingStatus.Cancelled
            );
        if (filter.FromDate is not null)
            query = query.Where(p =>
                p.CreatedAt >= DateTimeUtils.GetStartOfDay(filter.FromDate.Value)
            );
        if (filter.ToDate is not null)
            query = query.Where(p =>
                p.CreatedAt < DateTimeUtils.GetStartOfNextDay(filter.ToDate.Value)
            );
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(p =>
                (p.TransactionId != null && p.TransactionId.Contains(filter.Search))
                || p.Booking.BookingCode.Contains(filter.Search)
            );

        query = query.OrderByDescending(p => p.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<AdminPaymentListItemDto>()
            .ToListAsync(cancellationToken);

        return OffsetPageResult<AdminPaymentListItemDto>.Create(items, page, pageSize, total);
    }

    public async Task<AdminPaymentDetailDto> GetPaymentDetailAsync(
        int paymentId,
        CancellationToken cancellationToken
    )
    {
        var payment =
            await db
                .Payments.Include(p => p.Booking)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Payment);

        return payment.Adapt<AdminPaymentDetailDto>();
    }
}

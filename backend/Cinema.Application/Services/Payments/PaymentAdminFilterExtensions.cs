using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;

namespace Cinema.Application.Services.Payments;

internal static class PaymentAdminFilterExtensions
{
    public static IQueryable<Payment> ApplyAdminFilter(
        this IQueryable<Payment> query,
        AdminPaymentListFilter filter
    )
    {
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

        return query;
    }
}

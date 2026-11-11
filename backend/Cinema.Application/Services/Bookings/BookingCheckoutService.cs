using Cinema.Application.DTOs.Bookings;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema.Application.Services.Bookings;

public class BookingCheckoutService(
    IAppDbContext db,
    IEnumerable<IPaymentGateway> paymentGateways,
    ILogger<BookingCheckoutService> logger
) : IBookingCheckoutService
{
    public async Task<CheckoutResponse> CheckoutAsync(
        int userId,
        int bookingId,
        CheckoutRequest request,
        CancellationToken cancellationToken
    )
    {
        var booking =
            await db.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Booking);
        if (booking.UserId != userId)
            throw new ForbiddenAppException(Messages.Forbidden_NotBookingOwner);
        if (booking.Status != BookingStatus.Pending || booking.ExpiresAt <= DateTime.UtcNow)
            throw new BusinessRuleException(
                Messages.Rule_BookingNotPendingOrExpired
            );

        var gateway =
            paymentGateways.FindByMethod(request.PaymentMethod)
            ?? throw new BusinessRuleException(
                string.Format(Messages.Rule_PaymentGatewayNotConfigured, request.PaymentMethod)
            );

        await db
            .Payments.Where(p => p.BookingId == bookingId && p.Status == PaymentStatus.Pending)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(p => p.Status, PaymentStatus.Failed),
                cancellationToken
            );

        var transactionId = CodeGenerator.GeneratePaymentTransactionId(booking.BookingCode);

        var gatewayResult = await gateway.CreatePaymentAsync(
            new CreatePaymentRequest(
                booking.BookingId,
                booking.FinalAmount,
                string.Format(Messages.Payment_BookingDescription, booking.BookingCode),
                transactionId
            ),
            cancellationToken
        );

        var payment = new Payment
        {
            BookingId = booking.BookingId,
            PaymentMethod = EnumConverters.ParseEnum<PaymentMethod>(
                request.PaymentMethod,
                "paymentMethod"
            ),
            Status = PaymentStatus.Pending,
            Amount = booking.FinalAmount,
            TransactionId = transactionId,
            CreatedAt = DateTime.UtcNow,
        };
        db.Payments.Add(payment);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "PaymentCreated PaymentId {PaymentId} BookingId {BookingId} Method {Method}",
            payment.PaymentId,
            booking.BookingId,
            request.PaymentMethod
        );

        return new CheckoutResponse(gatewayResult.PaymentUrl, payment.PaymentId);
    }
}

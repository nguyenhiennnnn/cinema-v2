using Cinema.Domain.Enums;

namespace Cinema.Application.Interfaces;

public interface INotificationService
{
    void SendVerificationLink(
        VerificationTokenPurpose purpose,
        string toEmail,
        string fullName,
        string link,
        TimeSpan validFor
    );

    void SendBookingConfirmed(
        string toEmail,
        string customerName,
        string bookingCode,
        string movieTitle,
        int seatCount
    );

    void SendBookingCancelledRefunded(
        string toEmail,
        string customerName,
        string bookingCode,
        decimal refundAmount
    );

    void SendSeatsExpiredToAdmin(
        string toEmail,
        bool autoRefunded,
        int bookingId,
        int paymentId,
        decimal amount
    );

    void SendSeatsExpiredToCustomer(
        string toEmail,
        string customerName,
        string bookingCode,
        bool autoRefunded,
        decimal amount
    );

    void SendShowtimeCancelledRefunded(
        string toEmail,
        string customerName,
        string bookingCode,
        decimal refundAmount
    );

    void SendShowtimeCancelledManualRefund(
        string toEmail,
        string customerName,
        string bookingCode,
        decimal amountDue
    );
}

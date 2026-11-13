using Cinema.Domain.Enums;
using Cinema.Shared.Resources;

namespace Cinema.Application.Utils;

public static class ExportLabels
{
    public static string Booking(BookingStatus status) =>
        status switch
        {
            BookingStatus.Pending => Messages.Export_StatusPending,
            BookingStatus.Confirmed => Messages.Export_StatusConfirmed,
            BookingStatus.Cancelled => Messages.Export_StatusCancelled,
            _ => Messages.Export_StatusExpired,
        };

    public static string Channel(BookingChannel channel) =>
        channel == BookingChannel.Online
            ? Messages.Export_ChannelOnline
            : Messages.Export_ChannelCounter;

    public static string Payment(PaymentStatus status) =>
        status switch
        {
            PaymentStatus.Pending => Messages.Export_StatusPending,
            PaymentStatus.Success => Messages.Export_StatusSuccess,
            PaymentStatus.Failed => Messages.Export_StatusFailed,
            _ => Messages.Export_StatusRefunded,
        };

    public static string Method(PaymentMethod method) =>
        method == PaymentMethod.Cash ? Messages.Export_MethodCash : method.ToDtoMethod();

    public static string Showtime(ShowtimeStatus status) =>
        status switch
        {
            ShowtimeStatus.Scheduled => Messages.Export_StatusScheduled,
            ShowtimeStatus.Cancelled => Messages.Export_StatusCancelled,
            _ => Messages.Export_StatusCompleted,
        };
}

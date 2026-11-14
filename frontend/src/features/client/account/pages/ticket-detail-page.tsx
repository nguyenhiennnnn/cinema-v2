import { Link, useParams } from "react-router";
import { Copy, Ticket as TicketIcon } from "lucide-react";
import { Skeleton } from "@/components/ui/skeleton";
import { Button } from "@/components/ui/button";
import { BookingStatusBadge, PaymentMethodBadge, PaymentStatusBadge } from "@/components/common/status-badge";
import { PerfoTicket } from "@/components/common/perfo-ticket";
import { DownloadTicketsPdfButton } from "@/components/common/download-tickets-pdf-button";
import { useBookingDetail, useCancelBooking } from "@/features/client/bookings/hooks/use-bookings";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";
import { useCountdown } from "@/hooks/use-countdown";
import { formatCurrency, formatDate, formatDateTime, formatTime } from "@/lib/formatters";
import { BookingStatus } from "@/types/enum";
import { useNow } from "@/hooks/use-now";
import { useCinemaSettings } from "@/features/client/settings/hooks/use-settings";
import { ROUTES } from "@/config/routes";
import { ENDPOINTS } from "@/api/endpoints";

const AccountTicketDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const now = useNow();
  const bookingId = Number(id);

  const { data: booking, isLoading } = useBookingDetail(bookingId);
  const { data: settings } = useCinemaSettings();
  const cancelBooking = useCancelBooking();
  const openAlert = useAlertDialogOpen();
  const holdCountdown = useCountdown(booking?.holdExpiresAt ?? null);

  if (isLoading || !booking) {
    return (
      <div className="mx-auto max-w-4xl space-y-4 px-4 py-8">
        <Skeleton className="h-24 w-full " />
        <Skeleton className="h-64 w-full " />
      </div>
    );
  }

  const cancelDeadlineHours = settings?.cancelDeadlineHours ?? 2;
  const isRefundable = settings ? settings.cancelRefundPercent > 0 : true;

  const canCancel =
    booking.status === BookingStatus.Confirmed &&
    isRefundable &&
    new Date(booking.showtime.startTime).getTime() - now > cancelDeadlineHours * 60 * 60 * 1000;

  const isPending = booking.status === BookingStatus.Pending;
  const isHoldExpired = isPending && (!booking.holdExpiresAt || holdCountdown.remainingMs <= 0);
  const canResumePayment = isPending && !isHoldExpired;

  const handleCancel = () => {
    openAlert({
      title: "Hủy vé & hoàn tiền",
      description: "Bạn có chắc muốn hủy vé này? Số tiền sẽ được hoàn theo chính sách của rạp.",
      confirmText: "Hủy vé",
      variant: "destructive",
      onConfirm: () => cancelBooking.mutate(booking.bookingId),
    });
  };

  return (
    <div className="mx-auto max-w-4xl space-y-6 px-4 py-8">
      <div className="flex flex-wrap items-center justify-between gap-3 bg-card p-4">
        <div>
          <div className="flex items-center gap-2">
            <span className="t-code text-lg text-foreground">{booking.bookingCode}</span>
            <button onClick={() => navigator.clipboard.writeText(booking.bookingCode)} className="text-muted-foreground hover:text-foreground">
              <Copy className="size-4" />
            </button>
          </div>
          <p className="mt-1 text-xs text-muted-foreground">Đặt lúc {formatDateTime(booking.createdAt)}</p>
        </div>
        <BookingStatusBadge status={booking.status} />
      </div>

      {isPending && (
        <div className="flex flex-wrap items-center justify-between gap-3 border border-warning/40 bg-warning/10 p-4">
          <div>
            <p className="text-sm font-medium text-foreground">
              {canResumePayment
                ? "Booking đang chờ thanh toán — hoàn tất trước khi hết hạn giữ ghế."
                : "Thời gian giữ ghế đã hết hạn."}
            </p>
            {canResumePayment && (
              <p className="mt-0.5 text-xs font-semibold text-warning">
                Còn {holdCountdown.label} để hoàn tất thanh toán
              </p>
            )}
          </div>
          {canResumePayment ? (
            <Button>
              <Link to={`${ROUTES.booking.detail(booking.showtime.showtimeId)}?resume=${booking.bookingId}`}>
                Thanh toán ngay
              </Link>
            </Button>
          ) : (
            <Button variant="outline">
              <Link to={ROUTES.booking.detail(booking.showtime.showtimeId)}>Đặt vé mới</Link>
            </Button>
          )}
        </div>
      )}

      <div className="flex gap-4 bg-card p-4">
        <div className="aspect-2/3 w-24 shrink-0 overflow-hidden bg-muted">
          {booking.movie.posterUrl ? (
            <img src={booking.movie.posterUrl} alt={booking.movie.title} className="h-full w-full object-cover" />
          ) : (
            <div className="flex h-full w-full items-center justify-center text-muted-foreground">
              <TicketIcon className="size-6" />
            </div>
          )}
        </div>
        <div>
          <h1 className="font-heading text-lg font-semibold text-foreground">{booking.movie.title}</h1>
          <p className="mt-1 text-sm text-muted-foreground">{booking.showtime.roomName}</p>
          <p className="mt-1 text-sm text-muted-foreground">{formatDateTime(booking.showtime.startTime)}</p>
        </div>
      </div>

      <div className="bg-card p-4">
        <h2 className="font-heading text-base font-semibold text-foreground">Ghế & Combo</h2>
        <div className="mt-3 space-y-1.5 text-sm">
          {booking.seats.map((seat, i) => (
            <div key={i} className="flex justify-between text-foreground">
              <span>
                Ghế {seat.seatRow}
                {seat.seatNumber}
              </span>
              <span className="tabular-nums">{formatCurrency(seat.price)}</span>
            </div>
          ))}
          {booking.combos.map((combo, i) => (
            <div key={i} className="flex justify-between text-foreground">
              <span>
                {combo.quantity}x {combo.name}
              </span>
              <span className="tabular-nums">{formatCurrency(combo.unitPrice * combo.quantity)}</span>
            </div>
          ))}
        </div>
        <div className="mt-3 space-y-1.5 border-t border-border pt-3 text-sm">
          <div className="flex justify-between text-muted-foreground">
            <span>Tổng tiền</span>
            <span className="tabular-nums">{formatCurrency(booking.totalAmount)}</span>
          </div>
          {booking.discountAmount > 0 && (
            <div className="flex justify-between text-muted-foreground">
              <span>Giảm giá</span>
              <span className="tabular-nums">-{formatCurrency(booking.discountAmount)}</span>
            </div>
          )}
          <div className="flex justify-between text-base font-semibold text-foreground">
            <span>Thành tiền</span>
            <span className="tabular-nums text-primary">{formatCurrency(booking.finalAmount)}</span>
          </div>
        </div>
      </div>

      {booking.payments.length > 0 && (
        <div className="bg-card p-4">
          <h2 className="font-heading text-base font-semibold text-foreground">Thanh toán</h2>
          <div className="mt-3 space-y-2">
            {booking.payments.map((payment) => (
              <div key={payment.paymentId} className="flex items-center justify-between text-sm">
                <PaymentMethodBadge method={payment.paymentMethod} />
                <span className="tabular-nums text-foreground">{formatCurrency(payment.amount)}</span>
                <PaymentStatusBadge status={payment.status} />
              </div>
            ))}
          </div>
        </div>
      )}

      <div>
        <div className="flex flex-wrap items-center justify-between gap-3">
          <h2 className="font-heading text-base font-semibold text-foreground">Vé điện tử & Mã QR</h2>
          {booking.status === BookingStatus.Confirmed && (
            <DownloadTicketsPdfButton
              endpoint={ENDPOINTS.bookings.ticketsPdf(booking.bookingId)}
              fallbackFilename={`tickets-${booking.bookingCode}.pdf`}
            />
          )}
        </div>
        <div className="mt-3 flex flex-col gap-4">
          {booking.tickets.map((ticket) => (
            <PerfoTicket
              key={ticket.ticketId}
              movieTitle={booking.movie.title}
              showtime={`${formatTime(booking.showtime.startTime)}, ${formatDate(booking.showtime.startTime)}`}
              room={booking.showtime.roomName}
              seat={`${ticket.seatRow}${ticket.seatNumber}`}
              totalAmount={formatCurrency(booking.finalAmount)}
              ticketCode={`${booking.bookingCode}-${ticket.seatRow}${ticket.seatNumber}`}
              qrValue={ticket.qrCodeImage}
              isCheckedIn={ticket.isCheckedIn}
            />
          ))}
        </div>
      </div>

      {canCancel && (
        <div className="flex justify-end">
          <Button variant="destructive" onClick={handleCancel}>
            Hủy vé & Hoàn tiền
          </Button>
        </div>
      )}
    </div>
  );
}

export default AccountTicketDetailPage;
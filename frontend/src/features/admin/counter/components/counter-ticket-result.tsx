import { CheckCircle2, PlusCircle } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { PerfoTicket } from "@/components/common/perfo-ticket";
import { formatCurrency, formatDate, formatTime } from "@/lib/formatters";
import type {
  AdminBookingDetailDto,
  CounterCheckoutResponse,
} from "@/features/admin/bookings/types";

interface CounterTicketResultProps {
  isLoading: boolean;
  booking: AdminBookingDetailDto | undefined;
  checkout: CounterCheckoutResponse;
  onNewSale: () => void;
}

export const CounterTicketResult = ({ isLoading, booking, checkout, onNewSale }: CounterTicketResultProps) => {
  const seats = booking?.seats?.map((s) => `${s.seatRow}${s.seatNumber}`).join(", ") ?? "";
  const showtime = booking?.showtime
    ? `${formatTime(booking.showtime.startTime)}, ${formatDate(booking.showtime.startTime)}`
    : "";
  const room = booking?.showtime?.roomName ?? "";
  const total = booking ? formatCurrency(booking.finalAmount) : "";

  return (
    <div className="flex flex-col items-center gap-6 cut-lg bg-card p-6 text-center">
      <CheckCircle2 className="size-10 text-success" />
      <div>
        <p className="font-heading text-lg font-semibold text-foreground">Thanh toán thành công</p>
        <p className="text-sm text-muted-foreground">Tiền thừa trả khách: {formatCurrency(checkout.change)}</p>
      </div>

      {isLoading ? (
        <Skeleton className="h-48 w-full max-w-3xl" />
      ) : booking && booking.tickets && booking.tickets.length > 0 ? (
        <div className="flex w-full max-w-3xl flex-col gap-4">
          {booking.tickets.map((ticket) => (
            <PerfoTicket
              key={ticket.ticketId}
              movieTitle={booking.movie?.title ?? ""}
              showtime={showtime}
              room={room}
              seat={`${ticket.seatRow}${ticket.seatNumber}`}
              totalAmount={total}
              ticketCode={`${booking.bookingCode}-${ticket.seatRow}${ticket.seatNumber}`}
              qrValue={ticket.qrCodeImage}
              isCheckedIn={ticket.isCheckedIn}
            />
          ))}
        </div>
      ) : booking ? (
        <PerfoTicket
          movieTitle={booking.movie?.title ?? ""}
          showtime={showtime}
          room={room}
          seat={seats}
          totalAmount={total}
          ticketCode={booking.bookingCode}
          qrValue={booking.bookingCode}
        />
      ) : null}

      <Button onClick={onNewSale}>
        <PlusCircle className="size-4" />
        <span>Bán vé mới</span>
      </Button>
    </div>
  );
};

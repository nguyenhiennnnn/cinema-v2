import { CheckCircle2 } from "lucide-react";
import { formatDateTime } from "@/lib/formatters";
import type { BookingTicketDto } from "@/features/client/bookings/types";

interface TicketQrCardProps {
  ticket: BookingTicketDto;
}

export const TicketQrCard = ({ ticket }: TicketQrCardProps) => {
  return (
    <div className="flex flex-col items-center gap-3 border border-border bg-card p-4">
      <p className="font-heading text-sm font-semibold text-foreground">
        Ghế {ticket.seatRow}
        {ticket.seatNumber}
      </p>
      <img src={ticket.qrCodeImage} alt={`QR ghế ${ticket.seatRow}${ticket.seatNumber}`} className="size-40 border border-border bg-primary-foreground p-2" />
      {ticket.isCheckedIn ? (
        <span className="flex items-center gap-1 bg-muted px-2.5 py-1 text-xs font-medium text-foreground">
          <CheckCircle2 className="size-3.5" />
          Đã check-in {ticket.checkedInAt ? formatDateTime(ticket.checkedInAt) : ""}
        </span>
      ) : (
        <span className="bg-primary/10 px-2.5 py-1 text-xs font-medium text-primary">Chưa soát vé</span>
      )}
    </div>
  );
}
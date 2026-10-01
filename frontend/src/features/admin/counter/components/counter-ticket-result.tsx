import { CheckCircle2, PlusCircle } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { formatCurrency } from "@/lib/formatters";
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
  return (
    <div className="flex flex-col items-center gap-4 rounded-xl border border-border bg-card p-6 text-center">
      <CheckCircle2 className="size-10 text-success" />
      <div>
        <p className="font-heading text-lg font-semibold text-foreground">Thanh toán thành công</p>
        <p className="text-sm text-muted-foreground">Tiền thừa trả khách: {formatCurrency(checkout.change)}</p>
      </div>

      {isLoading ? (
        <Skeleton className="h-40 w-full max-w-sm rounded-xl" />
      ) : (
        <div className="grid w-full max-w-sm grid-cols-2 gap-3">
          {booking?.tickets?.map((ticket) => (
            <div
              key={ticket.ticketId}
              className="flex flex-col items-center gap-2 rounded-lg border border-border bg-background p-3"
            >
              <img src={ticket.qrCodeImage} alt={`Vé ${ticket.seatRow}${ticket.seatNumber}`} className="size-24" />
              <p className="text-xs font-semibold">
                Ghế {ticket.seatRow}
                {ticket.seatNumber}
              </p>
            </div>
          ))}
        </div>
      )}

      <Button onClick={onNewSale}>
        <PlusCircle className="size-4" />
        <span>Bán vé mới</span>
      </Button>
    </div>
  );
}
import { Separator } from "@/components/ui/separator";
import { formatCurrency } from "@/lib/formatters";
import type { ComboOption } from "@/components/common/combo-picker/combo-picker";
import type { HoldBookingResponse, PricedSeat } from "../../types";

interface BookingPriceSummaryProps {
  seats: PricedSeat[];
  combos: ComboOption[] | undefined;
  comboQuantities: Record<number, number>;
  hold?: HoldBookingResponse | null;
}

export const BookingPriceSummary = ({ seats, combos, comboQuantities, hold }: BookingPriceSummaryProps) => {
  const seatsTotal = seats.reduce((sum, s) => sum + s.price, 0);
  const combosTotal = Object.entries(comboQuantities).reduce((sum, [comboId, qty]) => {
    const combo = combos?.find((c) => c.comboId === Number(comboId));
    return sum + (combo?.price ?? 0) * qty;
  }, 0);
  const estimatedTotal = seatsTotal + combosTotal;

  return (
    <div className="flex flex-col gap-2 rounded-xl border border-border bg-card p-4">
      <h3 className="mb-1 font-heading text-sm font-semibold text-foreground">Chi tiết đơn hàng</h3>
      <div className="flex items-center justify-between text-sm">
        <span className="text-muted-foreground">
          Ghế ({seats.length}): {seats.map((s) => `${s.seatRow}${s.seatNumber}`).join(", ")}
        </span>
        <span className="font-medium">{formatCurrency(seatsTotal)}</span>
      </div>
      {combosTotal > 0 && (
        <div className="flex items-center justify-between text-sm">
          <span className="text-muted-foreground">Combo</span>
          <span className="font-medium">{formatCurrency(combosTotal)}</span>
        </div>
      )}
      {hold && hold.discountAmount > 0 && (
        <div className="flex items-center justify-between text-sm">
          <span className="text-muted-foreground">Giảm giá</span>
          <span className="font-medium text-primary">-{formatCurrency(hold.discountAmount)}</span>
        </div>
      )}
      <Separator />
      <div className="flex items-center justify-between">
        <span className="font-medium text-foreground">Tổng cộng</span>
        <span className="text-lg font-bold tabular-nums text-primary">
          {formatCurrency(hold?.finalAmount ?? estimatedTotal)}
        </span>
      </div>
    </div>
  );
}
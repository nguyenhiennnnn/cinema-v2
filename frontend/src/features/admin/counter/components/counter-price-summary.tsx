import { Separator } from "@/components/ui/separator";
import { formatCurrency } from "@/lib/formatters";
import type { ShowtimeSeatItemDto } from "@/features/client/showtimes/types";
import type { ComboOption } from "@/components/common/combo-picker/combo-picker";

interface CounterPriceSummaryProps {
  seats: ShowtimeSeatItemDto[];
  combos: ComboOption[] | undefined;
  comboQuantities: Record<number, number>;
  finalAmount?: number;
}

export const CounterPriceSummary = ({
  seats,
  combos,
  comboQuantities,
  finalAmount,
}: CounterPriceSummaryProps) => {
  const seatsTotal = seats.reduce((sum, s) => sum + s.price, 0);
  const combosTotal = Object.entries(comboQuantities).reduce((sum, [comboId, qty]) => {
    const combo = combos?.find((c) => c.comboId === Number(comboId));
    return sum + (combo?.price ?? 0) * qty;
  }, 0);
  const estimatedTotal = seatsTotal + combosTotal;

  return (
    <div className="flex flex-col gap-2 rounded-xl border border-border bg-card p-4">
      <div className="flex items-center justify-between text-sm">
        <span className="text-muted-foreground">Ghế ({seats.length})</span>
        <span className="font-medium">{formatCurrency(seatsTotal)}</span>
      </div>
      {combosTotal > 0 && (
        <div className="flex items-center justify-between text-sm">
          <span className="text-muted-foreground">Combo</span>
          <span className="font-medium">{formatCurrency(combosTotal)}</span>
        </div>
      )}
      <Separator />
      <div className="flex items-center justify-between">
        <span className="font-medium text-foreground">Tổng cộng</span>
        <span className="text-lg font-bold tabular-nums text-primary">
          {formatCurrency(finalAmount ?? estimatedTotal)}
        </span>
      </div>
    </div>
  );
}

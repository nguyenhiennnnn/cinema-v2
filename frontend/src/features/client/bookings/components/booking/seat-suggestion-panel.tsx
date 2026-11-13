import { useState } from "react";
import { Sparkles } from "lucide-react";
import { Button } from "@/components/ui/button";
import { ComboQuantityStepper } from "@/components/common/combo-picker/combo-quantity-stepper";
import { useSeatSuggestions } from "@/features/client/showtimes/hooks/use-showtimes";
import { formatCurrency } from "@/lib/formatters";
import { cn } from "@/lib/utils";
import { ShowtimeSeatStatus } from "@/types/enum";

import { SEAT_SUGGESTION_MAX_COUNT } from "@/config/default-constants";
interface SeatSuggestionPanelProps {
  showtimeId: number;
  statusMap: Map<number, ShowtimeSeatStatus>;
  selectedIds: Set<number>;
  onSelect: (showtimeSeatIds: number[]) => void;
  onHighlight: (showtimeSeatIds: number[]) => void;
}

export const SeatSuggestionPanel = ({
  showtimeId,
  statusMap,
  selectedIds,
  onSelect,
  onHighlight,
}: SeatSuggestionPanelProps) => {
  const [count, setCount] = useState(2);
  const [requested, setRequested] = useState<number | null>(null);
  const query = useSeatSuggestions(showtimeId, requested);

  const options = (query.data ?? []).filter((o) =>
    o.showtimeSeatIds.every(
      (id) => (statusMap.get(id) ?? ShowtimeSeatStatus.Available) === ShowtimeSeatStatus.Available,
    ),
  );

  const request = () => {
    if (requested === count) query.refetch();
    else setRequested(count);
  };

  return (
    <div className="flex flex-col gap-3 bg-card p-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h3 className="font-heading text-sm font-semibold text-foreground">Gợi ý ghế liền kề</h3>
        <div className="flex items-center gap-3">
          <span className="text-xs text-muted-foreground">Số người</span>
          <ComboQuantityStepper value={count} max={SEAT_SUGGESTION_MAX_COUNT} onChange={(v) => setCount(Math.max(1, v))} />
          <Button size="sm" onClick={request} disabled={query.isFetching}>
            <Sparkles className="size-4" />
            <span>{query.isFetching ? "Đang tìm..." : "Gợi ý"}</span>
          </Button>
        </div>
      </div>

      {requested != null && !query.isFetching && options.length === 0 && (
        <p className="text-xs text-muted-foreground">
          Không còn dãy {requested} ghế liền kề phù hợp, bạn có thể chọn ghế thủ công trên sơ đồ.
        </p>
      )}

      {options.length > 0 && (
        <div className="grid grid-cols-1 gap-2 sm:grid-cols-3">
          {options.map((o) => {
            const active =
              o.showtimeSeatIds.length === selectedIds.size &&
              o.showtimeSeatIds.every((id) => selectedIds.has(id));
            const first = `${o.seatRow}${o.seatNumbers[0]}`;
            const last = `${o.seatRow}${o.seatNumbers[o.seatNumbers.length - 1]}`;
            return (
              <button
                key={o.showtimeSeatIds.join("-")}
                type="button"
                onClick={() => onSelect(o.showtimeSeatIds)}
                onMouseEnter={() => onHighlight(o.showtimeSeatIds)}
                onMouseLeave={() => onHighlight([])}
                onFocus={() => onHighlight(o.showtimeSeatIds)}
                onBlur={() => onHighlight([])}
                className={cn(
                  "flex flex-col items-start gap-0.5 border bg-card p-3 text-left transition-colors hover:border-primary",
                  active ? "border-primary" : "border-border",
                )}
              >
                <span className="t-code text-sm text-foreground">
                  {o.seatNumbers.length > 1 ? `${first}–${last}` : first}
                </span>
                <span className="text-xs text-muted-foreground">{o.seatNumbers.length} ghế</span>
                <span className="text-xs font-semibold tabular-nums text-primary">
                  {formatCurrency(o.totalPrice)}
                </span>
              </button>
            );
          })}
        </div>
      )}
    </div>
  );
};

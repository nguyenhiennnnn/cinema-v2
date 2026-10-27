import { useState } from "react";
import { Skeleton } from "@/components/ui/skeleton";
import { Button } from "@/components/ui/button";
import { SelectableSeatMap } from "@/components/common/seat-map/selectable-seat-map";
import { ComboPicker } from "@/components/common/combo-picker/combo-picker";
import { useActiveCombos, useComboSuggestions } from "@/features/client/combos/hooks/use-combos";
import { ShowtimeSeatStatus } from "@/types/enum";
import type { ShowtimeSeatsResponseDto } from "@/features/client/showtimes/types";

import { BookingCustomerForm } from "./booking-customer-form";
import { BookingPriceSummary } from "./booking-price-summary";
import { SeatSuggestionPanel } from "./seat-suggestion-panel";

interface BookingSelectionStepProps {
  isLoading: boolean;
  data: ShowtimeSeatsResponseDto | undefined;
  statusMap: Map<number, ShowtimeSeatStatus>;
  selectedIds: Set<number>;
  onToggleSeat: (showtimeSeatId: number) => void;
  onSelectSeats: (showtimeSeatIds: number[]) => void;
  comboQuantities: Record<number, number>;
  onComboChange: (comboId: number, quantity: number) => void;
  customerName: string;
  onCustomerNameChange: (value: string) => void;
  customerPhone: string;
  onCustomerPhoneChange: (value: string) => void;
  isAuthenticated: boolean;
  canHold: boolean;
  isHolding: boolean;
  onContinue: () => void;
}

export const BookingSelectionStep = ({
  isLoading,
  data,
  statusMap,
  selectedIds,
  onToggleSeat,
  onSelectSeats,
  comboQuantities,
  onComboChange,
  customerName,
  onCustomerNameChange,
  customerPhone,
  onCustomerPhoneChange,
  isAuthenticated,
  canHold,
  isHolding,
  onContinue,
}: BookingSelectionStepProps) => {
  const activeCombos = useActiveCombos();
  const comboSuggestions = useComboSuggestions(data?.showtimeId ?? 0, selectedIds.size);
  const [highlightedIds, setHighlightedIds] = useState<Set<number>>(new Set());

  if (isLoading || !data) {
    return <Skeleton className="h-96 w-full " />;
  }

  const selectedSeats = data.seats.filter((s) => selectedIds.has(s.showtimeSeatId));
  const suggestions = comboSuggestions.data ?? [];
  const comboRank = new Map(suggestions.map((s, i) => [s.comboId, i]));
  const comboLabels = Object.fromEntries(suggestions.map((s) => [s.comboId, s.label]));
  const sortedCombos = activeCombos.data
    ? [...activeCombos.data].sort(
      (a, b) => (comboRank.get(a.comboId) ?? 99) - (comboRank.get(b.comboId) ?? 99),
    )
    : undefined;

  return (
    <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
      <div className="flex flex-col gap-4 lg:col-span-2">
        <SeatSuggestionPanel
          showtimeId={data.showtimeId}
          statusMap={statusMap}
          selectedIds={selectedIds}
          onSelect={onSelectSeats}
          onHighlight={(ids) => setHighlightedIds(new Set(ids))}
        />

        <SelectableSeatMap
          rows={data.rows}
          cols={data.cols}
          aisles={data.aisles}
          seats={data.seats}
          statusMap={statusMap}
          selectedIds={selectedIds}
          onToggle={(seat) => onToggleSeat(seat.showtimeSeatId)}
          highlightedIds={highlightedIds}
        />

        <div className="border border-border bg-card p-4">
          <h3 className="mb-3 font-heading text-sm font-semibold text-foreground">Chọn combo (tuỳ chọn)</h3>
          <ComboPicker
            combos={sortedCombos}
            isLoading={activeCombos.isLoading}
            quantities={comboQuantities}
            onChange={onComboChange}
            labels={comboLabels}
          />
        </div>

        <BookingCustomerForm
          customerName={customerName}
          onCustomerNameChange={onCustomerNameChange}
          customerPhone={customerPhone}
          onCustomerPhoneChange={onCustomerPhoneChange}
          isAuthenticated={isAuthenticated}
        />
      </div>

      <div className="flex flex-col gap-4">
        <BookingPriceSummary seats={selectedSeats} combos={activeCombos.data} comboQuantities={comboQuantities} />
        <Button disabled={!canHold || isHolding} onClick={onContinue}>
          {isHolding ? "Đang giữ ghế..." : "Giữ ghế & tiếp tục thanh toán"}
        </Button>
        {selectedSeats.length === 0 && (
          <p className="text-center text-xs text-muted-foreground">Vui lòng chọn ít nhất 1 ghế.</p>
        )}
      </div>
    </div>
  );
}

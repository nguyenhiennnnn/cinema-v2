import { Button } from "@/components/ui/button";
import { ComboPicker } from "@/components/common/combo-picker/combo-picker";
import { useActiveCombos } from "@/features/client/combos/hooks/use-combos";

import type { CounterFlow } from "../hooks/use-counter-flow";
import { CounterSeatPanel } from "./counter-seat-panel";
import { CounterCustomerForm } from "./counter-customer-form";
import { CounterPriceSummary } from "./counter-price-summary";

interface CounterSeatsStepProps {
  flow: CounterFlow;
}

export function CounterSeatsStep({ flow }: CounterSeatsStepProps) {
  const activeCombos = useActiveCombos();

  return (
    <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
      <div className="lg:col-span-2">
        <CounterSeatPanel
          isLoading={flow.showtimeSeats.isLoading}
          data={flow.showtimeSeats.data}
          statusMap={flow.seatStatusMap}
          selectedIds={flow.selectedSeatIds}
          onToggleSeat={flow.toggleSeat}
        />
      </div>

      <div className="flex flex-col gap-4">
        <div className="bg-card p-4">
          <h3 className="mb-3 font-heading text-sm font-semibold text-foreground">Combo</h3>
          <ComboPicker
            combos={activeCombos.data}
            isLoading={activeCombos.isLoading}
            quantities={flow.comboQuantities}
            onChange={flow.setComboQuantity}
          />
        </div>

        <div className="bg-card p-4">
          <h3 className="mb-3 font-heading text-sm font-semibold text-foreground">Thông tin khách</h3>
          <CounterCustomerForm
            customerName={flow.customerName}
            onCustomerNameChange={flow.setCustomerName}
            customerPhone={flow.customerPhone}
            onCustomerPhoneChange={flow.setCustomerPhone}
            userId={flow.userId}
            onUserIdChange={flow.setUserId}
          />
        </div>

        <CounterPriceSummary
          seats={flow.selectedSeatsInfo}
          combos={activeCombos.data}
          comboQuantities={flow.comboQuantities}
        />

        <Button disabled={!flow.canHold || flow.isHolding} onClick={flow.holdSeats}>
          {flow.isHolding ? "Đang giữ ghế..." : "Giữ ghế & tiếp tục"}
        </Button>
      </div>
    </div>
  );
}

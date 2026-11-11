import { useActiveCombos } from "@/features/client/combos/hooks/use-combos";

import type { CounterFlow } from "../hooks/use-counter-flow";
import { CounterPriceSummary } from "./counter-price-summary";
import { CounterCheckoutPanel } from "./counter-checkout-panel";

interface CounterCheckoutStepProps {
  flow: CounterFlow;
}

export function CounterCheckoutStep({ flow }: CounterCheckoutStepProps) {
  const activeCombos = useActiveCombos();

  if (!flow.holdResult) return null;

  return (
    <div className="mx-auto flex w-full max-w-md flex-col gap-4">
      <CounterPriceSummary
        seats={flow.selectedSeatsInfo}
        combos={activeCombos.data}
        comboQuantities={flow.comboQuantities}
        finalAmount={flow.holdResult.finalAmount}
      />
      <CounterCheckoutPanel
        hold={flow.holdResult}
        onConfirm={flow.confirmCheckout}
        onChangeSeats={flow.backToSeats}
        onCancelOrder={flow.resetFlow}
        isSubmitting={flow.isCheckingOut || flow.isCancelling}
      />
    </div>
  );
}

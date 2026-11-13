import { useEffect, useReducer, useRef } from "react";
import {
  counterFlowReducer,
  initialCounterStep,
  type CounterStep,
} from "./counter-flow-reducer";
import { useCounterSelection } from "./use-counter-selection";
import { useCounterHoldFlow } from "./use-counter-hold";
import { useCounterCheckoutFlow } from "./use-counter-checkout";

export type { CounterStep };

export function useCounterFlow() {
  const [step, dispatch] = useReducer(counterFlowReducer, initialCounterStep);

  const stepRef = useRef(step);

  useEffect(() => {
    stepRef.current = step;
  }, [stepRef, step]);

  const selection = useCounterSelection();
  const hold = useCounterHoldFlow({
    showtimeId: selection.showtimeId,
    initialSeats: selection.showtimeSeats.data?.seats,
    onHoldExpired: () => {
      if (stepRef.current !== "done") resetLocalFlow();
    },
  });
  const checkout = useCounterCheckoutFlow(hold.holdResult);

  const setStep = (next: CounterStep) =>
    dispatch({ type: "SET_STEP", step: next });

  const pickShowtime = async (id: number) => {
    if (id !== selection.showtimeId && !(await hold.cancelHold())) return;
    selection.selectShowtime(id);
    dispatch({ type: "SHOWTIME_PICKED" });
  };

  const toggleSeat = (showtimeSeatId: number) =>
    selection.toggleSeat(
      showtimeSeatId,
      hold.seatStatusMap.get(showtimeSeatId),
    );

  const holdSeats = async () => {
    const request = selection.buildHoldRequest();
    if (!request) return;
    await hold.holdSeats(request, () => dispatch({ type: "HOLD_SUCCEEDED" }));
  };

  const confirmCheckout = async (amountReceived: number) => {
    await checkout.confirmCheckout(amountReceived, () =>
      dispatch({ type: "CHECKOUT_SUCCEEDED" }),
    );
  };

  const backToSeats = async () => {
    if (!(await hold.cancelHold())) return;
    dispatch({ type: "BACK_TO_SEATS" });
  };

  const resetLocalFlow = () => {
    dispatch({ type: "RESET" });
    selection.resetSelection();
    hold.clearHold();
    checkout.clearCheckout();
  };

  const resetFlow = async () => {
    if (step !== "done" && !(await hold.cancelHold())) return;
    resetLocalFlow();
  };

  return {
    step,
    setStep,
    selectedDate: selection.selectedDate,
    setSelectedDate: selection.setSelectedDate,
    showtimeId: selection.showtimeId,
    showtimeSeats: selection.showtimeSeats,
    seatStatusMap: hold.seatStatusMap,
    selectedSeatIds: selection.selectedSeatIds,
    selectedSeatsInfo: selection.selectedSeatsInfo,
    toggleSeat,
    comboQuantities: selection.comboQuantities,
    setComboQuantity: selection.setComboQuantity,
    combosPayload: selection.combosPayload,
    customerName: selection.customerName,
    setCustomerName: selection.setCustomerName,
    customerPhone: selection.customerPhone,
    setCustomerPhone: selection.setCustomerPhone,
    userId: selection.userId,
    setUserId: selection.setUserId,
    estimatedTotal: selection.estimatedTotal,
    canHold: selection.canHold,
    holdSeats,
    isHolding: hold.isHolding,
    holdResult: hold.holdResult,
    confirmCheckout,
    isCheckingOut: checkout.isCheckingOut,
    isCancelling: hold.isCancelling,
    checkoutResult: checkout.checkoutResult,
    bookingDetail: checkout.bookingDetail,
    pickShowtime,
    cancelHold: hold.cancelHold,
    backToSeats,
    resetFlow,
  };
}

export type CounterFlow = ReturnType<typeof useCounterFlow>;

import { useState } from "react";
import { toast } from "@/components/ui/toast";
import { useRealtimeSeatMap } from "@/hooks/use-realtime-seat-map";
import {
  useCounterHold,
  useCounterCancel,
} from "@/features/admin/bookings/hooks/use-admin-booking";
import type {
  CounterHoldRequest,
  CounterHoldResponse,
} from "@/features/admin/bookings/types";
import type { ShowtimeSeatItemDto } from "@/features/client/showtimes/types";

interface UseCounterHoldFlowParams {
  showtimeId: number | null;
  initialSeats: ShowtimeSeatItemDto[] | undefined;
  onHoldExpired: () => void;
}

export function useCounterHoldFlow({
  showtimeId,
  initialSeats,
  onHoldExpired,
}: UseCounterHoldFlowParams) {
  const [holdResult, setHoldResult] = useState<CounterHoldResponse | null>(
    null,
  );

  const counterHold = useCounterHold();
  const counterCancel = useCounterCancel();

  const seatStatusMap = useRealtimeSeatMap({
    showtimeId,
    initialSeats,
    ownBookingId: holdResult?.bookingId ?? null,
    onHoldExpired: () => {
      toast.add({ title: "Thời gian giữ ghế đã hết hạn.", type: "warning" });
      onHoldExpired();
    },
  });

  const cancelHold = async () => {
    if (!holdResult) return true;
    try {
      await counterCancel.mutateAsync(holdResult.bookingId);
    } catch {
      return false;
    }
    setHoldResult(null);
    return true;
  };

  const holdSeats = async (
    request: CounterHoldRequest,
    onHeld: () => void,
  ) => {
    await counterHold.mutateAsync(request, {
      onSuccess: (res) => {
        setHoldResult(res.data);
        onHeld();
      },
    });
  };

  const clearHold = () => setHoldResult(null);

  return {
    holdResult,
    seatStatusMap,
    holdSeats,
    cancelHold,
    clearHold,
    isHolding: counterHold.isPending,
    isCancelling: counterCancel.isPending,
  };
}

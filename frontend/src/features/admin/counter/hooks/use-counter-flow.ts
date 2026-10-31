import { useMemo, useState } from "react";
import { toast } from "@/components/ui/toast";
import { getBusinessToday } from "@/lib/formatters";
import { ShowtimeSeatStatus } from "@/types/enum";
import { useShowtimeSeats } from "@/features/client/showtimes/hooks/use-showtimes";
import { useRealtimeSeatMap } from "@/hooks/use-realtime-seat-map";
import {
  useCounterHold,
  useCounterCheckout,
  useAdminBookingDetail,
} from "@/features/admin/bookings/hooks/use-admin-booking";
import type {
  CounterHoldResponse,
  CounterCheckoutResponse,
} from "@/features/admin/bookings/types";

export type CounterStep = "showtime" | "seats" | "checkout" | "done";

export function useCounterFlow() {
  const [step, setStep] = useState<CounterStep>("showtime");
  const [selectedDate, setSelectedDate] = useState(() => getBusinessToday());
  const [showtimeId, setShowtimeId] = useState<number | null>(null);
  const [selectedSeatIds, setSelectedSeatIds] = useState<Set<number>>(
    new Set(),
  );
  const [comboQuantities, setComboQuantities] = useState<
    Record<number, number>
  >({});
  const [customerName, setCustomerName] = useState("");
  const [customerPhone, setCustomerPhone] = useState("");
  const [userId, setUserId] = useState<number | null>(null);
  const [holdResult, setHoldResult] = useState<CounterHoldResponse | null>(
    null,
  );
  const [checkoutResult, setCheckoutResult] =
    useState<CounterCheckoutResponse | null>(null);

  const showtimeSeats = useShowtimeSeats(showtimeId ?? 0);
  const seatStatusMap = useRealtimeSeatMap({
    showtimeId,
    initialSeats: showtimeSeats.data?.seats,
    ownBookingId: holdResult?.bookingId ?? null,
    onHoldExpired: () => {
      toast.add({ title: "Thời gian giữ ghế đã hết hạn.", type: "warning" });
      resetFlow();
    },
  });

  const bookingDetail = useAdminBookingDetail(holdResult?.bookingId ?? 0);

  const counterHold = useCounterHold();
  const counterCheckout = useCounterCheckout();

  const combosPayload = useMemo(
    () =>
      Object.entries(comboQuantities)
        .filter(([, qty]) => qty > 0)
        .map(([comboId, quantity]) => ({ comboId: Number(comboId), quantity })),
    [comboQuantities],
  );

  const selectedSeatsInfo = useMemo(
    () =>
      (showtimeSeats.data?.seats ?? []).filter((s) =>
        selectedSeatIds.has(s.showtimeSeatId),
      ),
    [showtimeSeats.data, selectedSeatIds],
  );

  const estimatedTotal = useMemo(() => {
    const seatsTotal = selectedSeatsInfo.reduce((sum, s) => sum + s.price, 0);
    return seatsTotal;
  }, [selectedSeatsInfo]);

  const pickShowtime = (id: number) => {
    setShowtimeId(id);
    setSelectedSeatIds(new Set());
    setStep("seats");
  };

  const toggleSeat = (showtimeSeatId: number) => {
    const status = seatStatusMap.get(showtimeSeatId);
    if (status && status !== ShowtimeSeatStatus.Available) return;

    setSelectedSeatIds((prev) => {
      const next = new Set(prev);
      if (next.has(showtimeSeatId)) next.delete(showtimeSeatId);
      else next.add(showtimeSeatId);
      return next;
    });
  };

  const setComboQuantity = (comboId: number, quantity: number) => {
    setComboQuantities((prev) => ({ ...prev, [comboId]: quantity }));
  };

  const canHold =
    showtimeId != null &&
    selectedSeatIds.size > 0 &&
    // Có userId (khách đã định danh) hoặc có tên thủ công
    (userId != null || customerName.trim().length > 0);

  const holdSeats = async () => {
    if (!showtimeId) return;
    await counterHold.mutateAsync(
      {
        showtimeId,
        seatIds: Array.from(selectedSeatIds),
        combos: combosPayload.length > 0 ? combosPayload : undefined,
        userId: userId ?? undefined,
        customerName: customerName.trim(),
        customerPhone: customerPhone.trim() || undefined,
      },
      {
        onSuccess: (res) => {
          setHoldResult(res.data);
          setStep("checkout");
        },
      },
    );
  };

  const confirmCheckout = async (amountReceived: number) => {
    if (!holdResult) return;
    await counterCheckout.mutateAsync(
      {
        bookingId: holdResult.bookingId,
        body: { amountReceived },
      },
      {
        onSuccess: (res) => {
          setCheckoutResult(res.data);
          setStep("done");
        },
      },
    );
  };

  const resetFlow = () => {
    setStep("showtime");
    setShowtimeId(null);
    setSelectedSeatIds(new Set());
    setComboQuantities({});
    setCustomerName("");
    setCustomerPhone("");
    setUserId(null);
    setHoldResult(null);
    setCheckoutResult(null);
  };

  return {
    step,
    setStep,
    selectedDate,
    setSelectedDate,
    showtimeId,
    showtimeSeats,
    seatStatusMap,
    selectedSeatIds,
    selectedSeatsInfo,
    toggleSeat,
    comboQuantities,
    setComboQuantity,
    combosPayload,
    customerName,
    setCustomerName,
    customerPhone,
    setCustomerPhone,
    userId,
    setUserId,
    estimatedTotal,
    canHold,
    holdSeats,
    isHolding: counterHold.isPending,
    holdResult,
    confirmCheckout,
    isCheckingOut: counterCheckout.isPending,
    checkoutResult,
    bookingDetail,
    pickShowtime,
    resetFlow,
  };
}

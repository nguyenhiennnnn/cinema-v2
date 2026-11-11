import { useMemo, useState } from "react";
import { getBusinessToday } from "@/lib/business-time";
import { ShowtimeSeatStatus } from "@/types/enum";
import { useShowtimeSeats } from "@/features/client/showtimes/hooks/use-showtimes";
import { useActiveCombos } from "@/features/client/combos/hooks/use-combos";
import type { CounterHoldRequest } from "@/features/admin/bookings/types";

export function useCounterSelection() {
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

  const showtimeSeats = useShowtimeSeats(showtimeId ?? 0);
  const activeCombos = useActiveCombos();

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
    const combosTotal = combosPayload.reduce((sum, item) => {
      const combo = activeCombos.data?.find((c) => c.comboId === item.comboId);
      return sum + (combo?.price ?? 0) * item.quantity;
    }, 0);
    return seatsTotal + combosTotal;
  }, [selectedSeatsInfo, combosPayload, activeCombos.data]);

  const selectShowtime = (id: number) => {
    setShowtimeId(id);
    setSelectedSeatIds(new Set());
  };

  const toggleSeat = (
    showtimeSeatId: number,
    status: ShowtimeSeatStatus | undefined,
  ) => {
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
    (userId != null || customerName.trim().length > 0);

  const buildHoldRequest = (): CounterHoldRequest | null => {
    if (!showtimeId) return null;
    return {
      showtimeId,
      seatIds: Array.from(selectedSeatIds),
      combos: combosPayload.length > 0 ? combosPayload : undefined,
      userId: userId ?? undefined,
      customerName: customerName.trim(),
      customerPhone: customerPhone.trim() || undefined,
    };
  };

  const resetSelection = () => {
    setShowtimeId(null);
    setSelectedSeatIds(new Set());
    setComboQuantities({});
    setCustomerName("");
    setCustomerPhone("");
    setUserId(null);
  };

  return {
    selectedDate,
    setSelectedDate,
    showtimeId,
    showtimeSeats,
    selectShowtime,
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
    buildHoldRequest,
    resetSelection,
  };
}

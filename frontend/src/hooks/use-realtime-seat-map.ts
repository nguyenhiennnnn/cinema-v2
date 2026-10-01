import { useEffect, useMemo, useState } from "react";
import { useShowtimeHub } from "./use-showtime-hub";
import { ShowtimeSeatStatus } from "@/types/enum";
import type {
  HoldExpiredPayload,
  SeatBookedPayload,
  SeatHeldPayload,
  SeatReleasedPayload,
} from "@/lib/signalr";
import type { ShowtimeSeatItemDto } from "@/features/client/showtimes/types";

interface UseRealtimeSeatMapParams {
  showtimeId: number | null;
  initialSeats: ShowtimeSeatItemDto[] | undefined;
  ownBookingId?: number | null;
  onHoldExpired?: (payload: HoldExpiredPayload) => void;
}

export function useRealtimeSeatMap({
  showtimeId,
  initialSeats,
  ownBookingId,
  onHoldExpired,
}: UseRealtimeSeatMapParams) {
  const [statusOverrides, setStatusOverrides] = useState<
    Map<number, ShowtimeSeatStatus>
  >(() => new Map());

  useEffect(() => {
    setStatusOverrides(new Map());
  }, [initialSeats]);

  const applyStatus = (ids: number[], status: ShowtimeSeatStatus) => {
    setStatusOverrides((prev) => {
      const next = new Map(prev);
      ids.forEach((id) => next.set(id, status));
      return next;
    });
  };

  useShowtimeHub({
    showtimeId,
    onSeatHeld: (payload: SeatHeldPayload) =>
      applyStatus(payload.seatIds, ShowtimeSeatStatus.Held),
    onSeatBooked: (payload: SeatBookedPayload) =>
      applyStatus(payload.seatIds, ShowtimeSeatStatus.Booked),
    onSeatReleased: (payload: SeatReleasedPayload) =>
      applyStatus(payload.seatIds, ShowtimeSeatStatus.Available),
    onHoldExpired: (payload: HoldExpiredPayload) => {
      applyStatus(payload.seatIds, ShowtimeSeatStatus.Available);
      if (ownBookingId && payload.bookingId === ownBookingId) {
        onHoldExpired?.(payload);
      }
    },
  });

  const seatStatusMap = useMemo(() => {
    const map = new Map<number, ShowtimeSeatStatus>();
    initialSeats?.forEach((seat) =>
      map.set(seat.showtimeSeatId, seat.status as ShowtimeSeatStatus),
    );
    statusOverrides.forEach((status, id) => map.set(id, status));
    return map;
  }, [initialSeats, statusOverrides]);

  return seatStatusMap;
}

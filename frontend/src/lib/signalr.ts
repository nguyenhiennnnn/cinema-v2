export const SIGNALR_HUB_URL = `${import.meta.env.VITE_API_BASE_URL?.replace("/api/v1", "") ?? ""}/hubs/showtimes`;

export const SignalREvents = {
  SeatHeld: "seat.held",
  SeatBooked: "seat.booked",
  SeatReleased: "seat.released",
  HoldExpired: "hold.expired",
} as const;

export type SignalREventName =
  (typeof SignalREvents)[keyof typeof SignalREvents];

// ── Payload types gửi từ server ───────────────────────────────────────────────

export interface SeatHeldPayload {
  showtimeId: number;
  seatIds: number[];
  heldByUserId: number;
  heldUntil: string;
}

export interface SeatBookedPayload {
  showtimeId: number;
  seatIds: number[];
}

export interface SeatReleasedPayload {
  showtimeId: number;
  seatIds: number[];
}

export interface HoldExpiredPayload {
  showtimeId: number;
  bookingId: number;
  seatIds: number[];
}

import { resolveSeatVisualKind, type SeatVisualKind } from "./seat-map-types";

export const SEAT_KIND_LABEL: Record<SeatVisualKind, string> = {
  standard: "Ghế thường",
  vip: "Ghế VIP",
  couple: "Ghế đôi",
};

const DEFAULT_NAMES: Record<SeatVisualKind, RegExp> = {
  standard: /^(standard|thường|thuong)$/i,
  vip: /^vip$/i,
  couple: /^(couple|sweetbox|đôi)$/i,
};

export const seatTypeLabel = (name: string | null | undefined): string => {
  const trimmed = (name ?? "").trim();
  const kind = resolveSeatVisualKind(trimmed);
  if (trimmed === "" || DEFAULT_NAMES[kind].test(trimmed)) return SEAT_KIND_LABEL[kind];
  return trimmed;
};

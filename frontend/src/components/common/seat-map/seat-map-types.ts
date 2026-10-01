export type SeatVisualKind = "standard" | "vip" | "couple";

export type SeatCellState =
  | "available"
  | "selected"
  | "held"
  | "booked"
  | "inactive"
  | "aisle";

export interface SeatCellData {
  seatRow: string;
  seatNumber: number;
  seatId?: number;
  showtimeSeatId?: number;
  seatTypeId?: number | null;
  seatTypeName?: string | null;
  isActive: boolean;
  isAisle: boolean;
  coupleId?: string | null;
  price?: number;
}

export interface SeatGridCell extends SeatCellData {
  coupleSpan?: number;
  isCoupleSecondary?: boolean;
}

export function generateRowLabel(index: number): string {
  let n = index;
  let label = "";
  do {
    label = String.fromCharCode(65 + (n % 26)) + label;
    n = Math.floor(n / 26) - 1;
  } while (n >= 0);
  return label;
}

export function resolveSeatVisualKind(
  seatTypeName?: string | null,
): SeatVisualKind {
  const name = (seatTypeName ?? "").toLowerCase();
  if (
    name.includes("couple") ||
    name.includes("đôi") ||
    name.includes("sweetbox")
  ) {
    return "couple";
  }
  if (name.includes("vip")) return "vip";
  return "standard";
}

export function rowLabelToIndex(label: string): number {
  let n = 0;
  for (const ch of label.toUpperCase()) {
    n = n * 26 + (ch.charCodeAt(0) - 64);
  }
  return n - 1;
}

export function deriveGridDimensions(
  seats: { seatRow: string; seatNumber: number }[] | undefined,
  fallbackRows: number,
  fallbackCols: number,
): { rows: number; cols: number } {
  if (!seats || seats.length === 0) {
    return { rows: fallbackRows, cols: fallbackCols };
  }
  let maxRowIndex = -1;
  let maxCol = 0;
  for (const seat of seats) {
    const rowIndex = rowLabelToIndex(seat.seatRow);
    if (rowIndex > maxRowIndex) maxRowIndex = rowIndex;
    if (seat.seatNumber > maxCol) maxCol = seat.seatNumber;
  }
  return {
    rows: Math.max(fallbackRows, maxRowIndex + 1),
    cols: Math.max(fallbackCols, maxCol),
  };
}

export function seatCellKey(seatRow: string, seatNumber: number): string {
  return `${seatRow}-${seatNumber}`;
}

import { ShowtimeSeatStatus } from "@/types/enum";
import type { SeatCellState } from "./seat-map-types";

export function toCellState(status: ShowtimeSeatStatus, isSelected: boolean): SeatCellState {
  if (isSelected) return "selected";
  if (status === ShowtimeSeatStatus.Booked) return "booked";
  if (status === ShowtimeSeatStatus.Held) return "held";
  return "available";
}

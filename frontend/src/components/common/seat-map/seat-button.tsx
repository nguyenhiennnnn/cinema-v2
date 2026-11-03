import { cn } from "@/lib/utils";
import type { SeatCellState, SeatMark, SeatVisualKind } from "./seat-map-types";

interface SeatButtonProps {
  label: string;
  kind?: SeatVisualKind;
  state: SeatCellState;
  /** Dấu nhấn phụ (gợi ý, đang chọn vùng, ghế đôi chờ ghép). Vẽ bằng outline trong. */
  mark?: SeatMark;
  onClick?: () => void;
  disabled?: boolean;
  title?: string;
  className?: string;
  editable?: boolean;
  onMouseDown?: () => void;
  onMouseEnter?: () => void;
  ariaLabel?: string;
}

export const SeatButton = ({
  label,
  kind = "standard",
  state,
  mark,
  onClick,
  disabled,
  title,
  className,
  editable = false,
  onMouseDown,
  onMouseEnter,
  ariaLabel,
}: SeatButtonProps) => {
  // Lối đi ngoài trình soạn thảo là khoảng trắng, không vẽ gì.
  if (state === "aisle" && !editable) {
    return <div className={cn("h-[min(2.25rem,var(--fit,2.25rem))] w-full", className)} aria-hidden />;
  }

  const interactive = editable || state === "available" || state === "selected";

  return (
    <button
      type="button"
      title={title}
      aria-label={ariaLabel}
      aria-pressed={state === "selected"}
      disabled={disabled || !interactive}
      onClick={onClick}
      onMouseDown={onMouseDown}
      onMouseEnter={onMouseEnter}
      data-state={state}
      data-type={kind}
      data-mark={mark}
      className={cn(
        "seat seat--fill select-none",
        interactive ? "cursor-pointer" : "cursor-not-allowed",
        className,
      )}
    >
      {state === "booked" ? "" : label}
    </button>
  );
};

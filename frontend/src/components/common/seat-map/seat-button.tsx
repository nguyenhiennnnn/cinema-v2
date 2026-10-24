import { cn } from "@/lib/utils";
import type { SeatCellState, SeatVisualKind } from "./seat-map-types";

interface SeatButtonProps {
  label: string;
  kind?: SeatVisualKind;
  state: SeatCellState;
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
  onClick,
  disabled,
  title,
  className,
  editable = false,
  onMouseDown,
  onMouseEnter,
  ariaLabel,
}: SeatButtonProps) => {
  if (state === "aisle" && !editable) {
    return <div className={cn("h-7 w-full", className)} aria-hidden />;
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
      className={cn(
        "seat seat--fill w-full h-7 transition-colors cursor-pointer select-none",
        !interactive && "cursor-not-allowed",
        state === "inactive" && "opacity-30 bg-muted! cursor-not-allowed",
        state === "aisle" && "bg-transparent! border border-dashed border-border/50 text-muted-foreground/50 hover:bg-muted/40!",
        className,
      )}
    >
      {state === "booked" ? "" : label}
    </button>
  );
};

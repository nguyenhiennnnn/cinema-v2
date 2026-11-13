import { cn } from "@/lib/utils";
import { SeatKindIcon } from "./seat-kind-icon";
import type { SeatCellState, SeatMark, SeatVisualKind } from "./seat-map-types";

interface SeatButtonProps {
  label: string;
  kind?: SeatVisualKind;
  state: SeatCellState;
  mark?: SeatMark;
  onClick?: () => void;
  disabled?: boolean;
  title?: string;
  className?: string;
  editable?: boolean;
  onMouseDown?: () => void;
  onMouseEnter?: () => void;
  ariaLabel?: string;
  seatKey?: string;
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
  seatKey,
}: SeatButtonProps) => {
  if (state === "aisle" && !editable) {
    return <div className={cn("h-[min(2.25rem,var(--fit,2.25rem))] w-full", className)} aria-hidden />;
  }

  const interactive = editable || state === "available" || state === "selected";
  const showKindIcon =
    kind !== "standard" && (state === "available" || state === "selected" || state === "held");

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
      data-seat-key={seatKey}
      className={cn(
        "seat seat--fill select-none",
        interactive ? "cursor-pointer" : "cursor-not-allowed",
        className,
      )}
    >
      <span className="seat__head" aria-hidden />
      {showKindIcon && (
        <span
          className="pointer-events-none absolute top-0 right-1.5 flex h-[22%] items-center text-background/90 [&_svg]:size-2"
          aria-hidden
        >
          <SeatKindIcon kind={kind} className="stroke-[3]" />
        </span>
      )}
      <span className="seat__label">{state === "booked" ? "" : label}</span>
    </button>
  );
};

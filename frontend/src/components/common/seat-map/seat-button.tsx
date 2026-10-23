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

const KIND_CLASS: Record<SeatVisualKind, string> = {
  standard: "bg-secondary text-secondary-foreground border-border hover:border-primary/50",
  vip: "bg-vip/15 border-vip/50 text-vip-foreground hover:bg-vip/25 font-semibold",
  couple: "bg-couple/12 border-couple/45 text-couple hover:bg-couple/22",
};

const STATE_OVERRIDE_CLASS: Partial<Record<SeatCellState, string>> = {
  selected: "bg-primary! text-primary-foreground! border-primary!",
  held: "bg-held/20! border-held! text-held! cursor-not-allowed",
  booked: "bg-muted/50! text-muted-foreground/35! border-transparent! cursor-not-allowed",
  inactive: "bg-muted/20! text-muted-foreground/25! border-dashed! border-border/50! opacity-60",
};

const EDITABLE_AISLE_CLASS =
  "border-dashed! border-border/50! bg-transparent! text-muted-foreground/50! hover:bg-muted/40!";

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
    return <div className={cn("h-8 w-full", className)} aria-hidden />;
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
      className={cn(
        "flex h-8 w-full items-center justify-center border text-[11px] font-medium tabular transition-colors",
        state === "aisle" ? EDITABLE_AISLE_CLASS : KIND_CLASS[kind],
        STATE_OVERRIDE_CLASS[state],
        className,
      )}
    >
      {label}
    </button>
  );
};

import { cn } from "@/lib/utils";
import type { SeatCellState, SeatVisualKind } from "./seat-map-types";

export interface SeatLegendItem {
  label: string;
  state?: SeatCellState;
  type?: SeatVisualKind;
  hint?: string;
  group?: string;
}

const LEGEND_STD = 20;
const LEGEND_GAP = 4;
const LEGEND_COUPLE_W = LEGEND_STD * 2 + LEGEND_GAP;

export const SeatLegend = ({ items, className }: { items: SeatLegendItem[]; className?: string }) => {
  return (
    <ul className={cn("flex flex-wrap items-center gap-x-5 gap-y-2.5", className)}>
      {items.map((item) => (
        <li key={item.label} className="flex items-center gap-2">
          <span
            className="seat pointer-events-none shrink-0"
            data-state={item.state ?? "available"}
            data-type={item.type ?? "standard"}
            style={{
              width: item.type === "couple" ? LEGEND_COUPLE_W : LEGEND_STD,
              height: LEGEND_STD,
              fontSize: 10,
            }}
            aria-hidden
          >
            <span className="seat__head" />
            <span className="seat__label">{item.state === "inactive" ? "1" : null}</span>
          </span>
          <span className="flex flex-col leading-tight">
            <span className="whitespace-nowrap text-[13px] text-foreground/90">{item.label}</span>
            {item.hint && (
              <span className="t-code whitespace-nowrap text-[11px] text-muted-foreground">{item.hint}</span>
            )}
          </span>
        </li>
      ))}
    </ul>
  );
};

export const CinemaScreen = ({ className }: { className?: string }) => {
  return (
    <div className={cn("relative h-14 text-foreground", className)} aria-hidden>
      <svg
        className="absolute inset-0 h-full w-full"
        viewBox="0 0 100 40"
        preserveAspectRatio="none"
      >
        <defs>
          <linearGradient id="seat-screen-glow" x1="0" y1="0" x2="0" y2="1">
            <stop offset="0" stopColor="currentColor" stopOpacity="0.28" />
            <stop offset="1" stopColor="currentColor" stopOpacity="0" />
          </linearGradient>
        </defs>
        <path d="M0 6 Q50 0 100 6 L93 40 L7 40 Z" fill="url(#seat-screen-glow)" />
        <path
          d="M0 6 Q50 0 100 6"
          fill="none"
          stroke="currentColor"
          strokeWidth="3"
          vectorEffect="non-scaling-stroke"
        />
      </svg>
      <p className="absolute inset-x-0 top-[40%] text-center text-[12px] uppercase tracking-[0.35em] text-muted-foreground">
        Màn hình
      </p>
    </div>
  );
};

import type { SeatCellState, SeatVisualKind } from "./seat-map-types";

export interface SeatLegendItem {
  label: string;
  state?: SeatCellState;
  type?: SeatVisualKind;
}

const LEGEND_STD = 24;
const LEGEND_GAP = 4;
const LEGEND_COUPLE_W = LEGEND_STD * 2 + LEGEND_GAP;

export const SeatLegend = ({ items }: { items: SeatLegendItem[] }) => {
  return (
    <ul className="grid grid-cols-2 gap-x-4 gap-y-3 @min-[420px]:grid-cols-3 @min-[600px]:flex @min-[600px]:flex-wrap @min-[600px]:items-center">
      {items.map((item) => (
        <li key={item.label} className="flex items-center gap-2">
          <span
            className="seat pointer-events-none shrink-0"
            data-state={item.state ?? "available"}
            data-type={item.type ?? "standard"}
            style={{
              width: item.type === "couple" ? LEGEND_COUPLE_W : LEGEND_STD,
              height: LEGEND_STD,
            }}
            aria-hidden
          >
            {item.state === "inactive" ? "1" : null}
          </span>
          <span className="whitespace-nowrap text-[13px] text-muted-foreground">{item.label}</span>
        </li>
      ))}
    </ul>
  );
};

export const CinemaScreen = ({ className }: { className?: string }) => {
  return (
    <div className={className}>
      <div className="h-1.5 w-full bg-foreground cut-xs" />
      <p className="mt-2 text-center text-[13px] text-muted-foreground">Màn hình</p>
    </div>
  );
};

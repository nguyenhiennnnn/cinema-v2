import { Fragment } from "react";
import type { SeatCellState, SeatVisualKind } from "./seat-map-types";

export interface SeatLegendItem {
  label: string;
  state?: SeatCellState;
  type?: SeatVisualKind;
  hint?: string;
  group?: string;
}

const LEGEND_STD = 24;
const LEGEND_GAP = 4;
const LEGEND_COUPLE_W = LEGEND_STD * 2 + LEGEND_GAP;

export const SeatLegend = ({ items }: { items: SeatLegendItem[] }) => {
  return (
    <ul className="grid grid-cols-2 gap-x-4 gap-y-3 @min-[420px]:grid-cols-3 @min-[600px]:flex @min-[600px]:flex-wrap @min-[600px]:items-center">
      {items.map((item, i) => {
        const startsNewGroup = i > 0 && item.group !== items[i - 1].group;
        return (
          <Fragment key={item.label}>
            {startsNewGroup && (
              <li aria-hidden className="mx-1 hidden h-5 w-px bg-border @min-[600px]:block" />
            )}
            <li className="flex items-center gap-2">
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
              <span className="flex flex-col leading-tight @min-[600px]:flex-row @min-[600px]:items-baseline @min-[600px]:gap-1.5">
                <span className="whitespace-nowrap text-[13px] text-muted-foreground">{item.label}</span>
                {item.hint && (
                  <span className="t-code whitespace-nowrap text-[12px] text-foreground">{item.hint}</span>
                )}
              </span>
            </li>
          </Fragment>
        );
      })}
    </ul>
  );
};

export const CinemaScreen = ({ className }: { className?: string }) => {
  return (
    <div className={className}>
      <div className="relative">
        <div className="h-1.5 w-full bg-foreground cut-xs" />
        <div
          aria-hidden
          className="pointer-events-none absolute inset-x-[4%] top-1.5 h-12 bg-linear-to-b from-foreground/20 to-transparent"
          style={{ clipPath: "polygon(0 0, 100% 0, 90% 100%, 10% 100%)" }}
        />
        <p className="relative mt-2 text-center text-[13px] uppercase tracking-[0.2em] text-muted-foreground">
          Màn hình
        </p>
      </div>
    </div>
  );
};

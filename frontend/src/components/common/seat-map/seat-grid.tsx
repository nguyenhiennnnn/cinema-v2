import type { CSSProperties, ReactNode } from "react";
import { cn } from "@/lib/utils";
import { CinemaScreen } from "./seat-legend";
import type { SeatGridCell } from "./seat-map-types";

interface SeatGridProps {
  rowLabels: string[];
  cols: number;
  cellAt: (rowLabel: string, col: number) => SeatGridCell | undefined;
  renderCell: (cell: SeatGridCell | undefined, rowLabel: string, col: number) => ReactNode;
  className?: string;
  onRowLabelClick?: (rowLabel: string) => void;
  rowLabelClassName?: (rowLabel: string) => string | undefined;
  rowLabelTitle?: (rowLabel: string) => string | undefined;
  fitWidth?: boolean;
}

const ROW_LABEL = "t-code w-6 shrink-0 text-center text-[13px] text-muted-foreground";

export const SeatGrid = ({
  rowLabels,
  cols,
  cellAt,
  renderCell,
  className,
  onRowLabelClick,
  rowLabelClassName,
  rowLabelTitle,
  fitWidth = false,
}: SeatGridProps) => {
  return (
    <div className={cn("w-full pb-2", fitWidth ? "@container" : "overflow-x-auto", className)}>
      <div
        className={cn(
          "mx-auto flex w-max flex-col gap-1.5",
          fitWidth &&
            "[--fit-gap:0.25rem] @xl:[--fit-gap:0.375rem] [&_.seat]:[--seat-size:min(2.25rem,var(--fit))] [&_.seat]:text-[clamp(7px,calc(var(--fit)*0.45),13px)]",
        )}
        style={
          fitWidth
            ? ({
                "--fit": `min(2.5rem, calc((100cqw - 4rem - ${cols - 1} * var(--fit-gap)) / ${cols}))`,
              } as CSSProperties)
            : undefined
        }
      >
        <CinemaScreen className="mx-8 mb-8" />

        {rowLabels.map((rowLabel) => (
          <div key={rowLabel} className="flex items-center gap-2">
            {onRowLabelClick ? (
              <button
                type="button"
                onClick={() => onRowLabelClick(rowLabel)}
                title={rowLabelTitle?.(rowLabel)}
                className={cn(
                  ROW_LABEL,
                  "h-9 transition-colors hover:bg-muted hover:text-foreground",
                  rowLabelClassName?.(rowLabel),
                )}
              >
                {rowLabel}
              </button>
            ) : (
              <span className={ROW_LABEL}>{rowLabel}</span>
            )}
            <div
              className={cn("grid", fitWidth ? "gap-(--fit-gap)" : "gap-1.5")}
              style={{
                gridTemplateColumns: fitWidth
                  ? `repeat(${cols}, var(--fit))`
                  : `repeat(${cols}, minmax(2.25rem, 2.5rem))`,
              }}
            >
              {Array.from({ length: cols }, (_, i) => i + 1).map((col) => {
                const cell = cellAt(rowLabel, col);
                if (cell?.isCoupleSecondary) return null;
                const span = cell?.coupleSpan ?? 1;
                return (
                  <div key={col} style={{ gridColumn: `${col} / span ${span}` }} className="w-full">
                    {renderCell(cell, rowLabel, col)}
                  </div>
                );
              })}
            </div>
            <span className={ROW_LABEL} aria-hidden>
              {rowLabel}
            </span>
          </div>
        ))}
      </div>
    </div>
  );
};

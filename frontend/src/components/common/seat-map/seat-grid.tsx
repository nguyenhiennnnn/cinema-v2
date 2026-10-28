import type { ReactNode } from "react";
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
}: SeatGridProps) => {
  return (
    <div className={cn("w-full overflow-x-auto pb-2", className)}>
      <div className="mx-auto flex w-max flex-col gap-1.5">
        {/* thanh màn hình rộng đúng bằng phần ghế (trừ hai cột chữ cái hàng) */}
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
              className="grid gap-1.5"
              style={{ gridTemplateColumns: `repeat(${cols}, minmax(2.25rem, 2.5rem))` }}
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

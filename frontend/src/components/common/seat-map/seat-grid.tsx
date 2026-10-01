import type { ReactNode } from "react";
import { cn } from "@/lib/utils";
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
      <div className="mx-auto flex w-max min-w-full flex-col items-center gap-1.5">
        {rowLabels.map((rowLabel) => (
          <div key={rowLabel} className="flex items-center gap-1.5">
            {onRowLabelClick ? (
              <button
                type="button"
                onClick={() => onRowLabelClick(rowLabel)}
                title={rowLabelTitle?.(rowLabel)}
                className={cn(
                  "w-5 shrink-0 rounded-sm text-center text-xs font-semibold text-muted-foreground transition-colors hover:bg-muted hover:text-foreground",
                  rowLabelClassName?.(rowLabel),
                )}
              >
                {rowLabel}
              </button>
            ) : (
              <span className="w-5 shrink-0 text-center text-xs font-semibold text-muted-foreground">
                {rowLabel}
              </span>
            )}
            <div
              className="grid gap-1.5"
              style={{ gridTemplateColumns: `repeat(${cols}, minmax(1.75rem, 2.25rem))` }}
            >
              {Array.from({ length: cols }, (_, i) => i + 1).map((col) => {
                const cell = cellAt(rowLabel, col);
                if (cell?.isCoupleSecondary) return null;
                const span = cell?.coupleSpan ?? 1;
                return (
                  <div key={col} style={{ gridColumn: `${col} / span ${span}` }}>
                    {renderCell(cell, rowLabel, col)}
                  </div>
                );
              })}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

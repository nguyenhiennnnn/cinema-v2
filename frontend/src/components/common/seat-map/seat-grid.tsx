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
  compactAisles?: boolean;
  curve?: number;
}

const MAX_TILT_DEG = 2.5;

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
  compactAisles = false,
  curve = 6,
}: SeatGridProps) => {
  const curveDepth = cols >= 6 ? curve : 0;
  const mid = (cols + 1) / 2;
  const half = Math.max(1, (cols - 1) / 2);
  const curveTransform = (col: number, span: number) => {
    if (curveDepth === 0) return undefined;
    const norm = (col + (span - 1) / 2 - mid) / half;
    const y = Math.round((1 - norm * norm) * curveDepth * 10) / 10;
    const rotate = Math.round(-norm * MAX_TILT_DEG * 10) / 10;
    return `translateY(${y}px) rotate(${rotate}deg)`;
  };

  const aisleCols = new Set<number>();
  if (compactAisles && rowLabels.length > 0) {
    for (let col = 1; col <= cols; col++) {
      if (rowLabels.every((r) => cellAt(r, col)?.isAisle)) aisleCols.add(col);
    }
  }
  const AISLE_RATIO = 0.45;
  const effectiveCols = cols - aisleCols.size * (1 - AISLE_RATIO);

  const columnTemplate = Array.from({ length: cols }, (_, i) => i + 1)
    .map((col) => {
      const isAisle = aisleCols.has(col);
      if (fitWidth) return isAisle ? `calc(var(--fit) * ${AISLE_RATIO})` : "var(--fit)";
      return isAisle ? "1rem" : "minmax(2.25rem, 2.5rem)";
    })
    .join(" ");

  return (
    <div className={cn("w-full pb-2", fitWidth ? "@container overflow-x-auto" : "overflow-x-auto", className)}>
      <div
        className={cn(
          "mx-auto flex w-max flex-col gap-1.5",
          curveDepth > 0 && "pb-(--curve)",
          fitWidth &&
          "[--fit-gap:0.25rem] @xl:[--fit-gap:0.375rem] [&_.seat]:[--seat-size:min(2.25rem,var(--fit))] [&_.seat]:text-[clamp(7px,calc(var(--fit)*0.45),13px)]",
        )}
        style={
          {
            "--curve": `${curveDepth}px`,
            ...(fitWidth
              ? {
                "--fit": `max(1.75rem, min(2.5rem, calc((100cqw - 4rem - ${cols - 1} * var(--fit-gap)) / ${effectiveCols})))`,
              }
              : {}),
          } as CSSProperties
        }
      >
        <CinemaScreen className="mx-8 mb-4" />

        {rowLabels.map((rowLabel) => (
          <div key={rowLabel} role="group" aria-label={`Hàng ${rowLabel}`} className="flex items-center gap-2">
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
              style={{ gridTemplateColumns: columnTemplate }}
            >
              {Array.from({ length: cols }, (_, i) => i + 1).map((col) => {
                const cell = cellAt(rowLabel, col);
                if (cell?.isCoupleSecondary) return null;
                const span = cell?.coupleSpan ?? 1;
                return (
                  <div
                    key={col}
                    style={{
                      gridColumn: `${col} / span ${span}`,
                      transform: curveTransform(col, span),
                    }}
                    className="w-full"
                  >
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

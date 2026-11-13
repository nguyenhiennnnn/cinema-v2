import { useEffect, useRef, useState } from "react";
import { cn } from "@/lib/utils";
import { seatTypeLabel } from "@/components/common/seat-map/seat-kind";
import {
  EDITOR_LEGEND_ITEMS,
  PREVIEW_LEGEND_ITEMS,
} from "@/components/common/seat-map/seat-legend-items";
import { SeatMapFrame } from "@/components/common/seat-map/seat-map-frame";
import { SeatButton } from "@/components/common/seat-map/seat-button";
import { SeatGrid } from "@/components/common/seat-map/seat-grid";
import { useSeatGridModel } from "@/components/common/seat-map/use-seat-grid-model";
import {
  resolveSeatVisualKind,
  seatCellKey,
  type SeatMark,
} from "@/components/common/seat-map/seat-map-types";
import type { SeatTypeDto } from "@/features/admin/catalog/types";
import type { EditorCell, SeatEditorBrush } from "../types/seat-editor";

const HOVER_PREVIEW = "hover:[box-shadow:inset_0_0_0_2px_var(--primary)]!";

const cursorClassFor = (
  brush: SeatEditorBrush | undefined,
  selectionMode: boolean,
  previewMode: boolean,
) => {
  if (previewMode) return undefined;
  if (selectionMode) return "[&_.seat:not(:disabled)]:cursor-crosshair";
  if (brush?.type === "pairCouple") return "[&_.seat:not(:disabled)]:cursor-copy";
  return "[&_.seat:not(:disabled)]:cursor-cell";
};

interface SeatEditorGridProps {
  rows: number;
  cols: number;
  cellMap: Map<string, EditorCell>;
  seatTypes: SeatTypeDto[] | undefined;
  pendingCoupleKey: string | null;
  onCellClick: (rowLabel: string, col: number) => void;
  activeBrush?: SeatEditorBrush;
  issueKeys?: Set<string>;

  previewMode?: boolean;

  selectionMode?: boolean;
  selectedKeys?: Set<string>;
  onSelectionChange?: (keys: string[]) => void;

  onRowLabelClick?: (rowLabel: string) => void;
  onColLabelClick?: (col: number) => void;
  copySourceRow?: string | null;
  isPickingCopySource?: boolean;
}

export const SeatEditorGrid = ({
  rows,
  cols,
  cellMap,
  seatTypes,
  pendingCoupleKey,
  onCellClick,
  activeBrush,
  issueKeys,
  previewMode = false,
  selectionMode = false,
  selectedKeys,
  onSelectionChange,
  onRowLabelClick,
  onColLabelClick,
  copySourceRow,
  isPickingCopySource,
}: SeatEditorGridProps) => {
  const seatTypeName = (id: number | null) =>
    seatTypes?.find((t) => t.seatTypeId === id)?.name ?? null;

  const cells = Array.from(cellMap.values()).map((c) => ({
    seatRow: c.seatRow,
    seatNumber: c.seatNumber,
    isActive: c.isActive,
    isAisle: c.isAisle,
    coupleId: c.coupleId,
    seatTypeId: c.seatTypeId,
    seatTypeName: seatTypeName(c.seatTypeId),
  }));

  const { rowLabels, cellAt } = useSeatGridModel({ rowsCount: rows, cols, cells });

  const [isDragging, setIsDragging] = useState(false);
  const dragAnchorRef = useRef<{ r: number; c: number } | null>(null);

  useEffect(() => {
    if (!isDragging) return;
    const stopDragging = () => setIsDragging(false);
    window.addEventListener("mouseup", stopDragging);
    return () => window.removeEventListener("mouseup", stopDragging);
  }, [isDragging]);

  const rectKeys = (r1: number, c1: number, r2: number, c2: number) => {
    const rMin = Math.min(r1, r2);
    const rMax = Math.max(r1, r2);
    const cMin = Math.min(c1, c2);
    const cMax = Math.max(c1, c2);
    const keys: string[] = [];
    for (let r = rMin; r <= rMax; r++) {
      const rowLabel = rowLabels[r];
      if (!rowLabel) continue;
      for (let c = cMin; c <= cMax; c++) {
        keys.push(seatCellKey(rowLabel, c));
      }
    }
    return keys;
  };

  const handleCellMouseDown = (rowLabel: string, col: number) => {
    if (!selectionMode || !onSelectionChange) return;
    const r = rowLabels.indexOf(rowLabel);
    dragAnchorRef.current = { r, c: col };
    setIsDragging(true);
    onSelectionChange([seatCellKey(rowLabel, col)]);
  };

  const handleCellMouseEnter = (rowLabel: string, col: number) => {
    if (!selectionMode || !isDragging || !dragAnchorRef.current || !onSelectionChange) return;
    const r = rowLabels.indexOf(rowLabel);
    onSelectionChange(rectKeys(dragAnchorRef.current.r, dragAnchorRef.current.c, r, col));
  };

  const hasSelection = !!selectedKeys && selectedKeys.size > 0;

  const isRowSelected = (rowLabel: string) =>
    hasSelection &&
    Array.from({ length: cols }, (_, i) => seatCellKey(rowLabel, i + 1)).every((k) => !!selectedKeys?.has(k));

  const isColSelected = (col: number) =>
    hasSelection && rowLabels.length > 0 && rowLabels.every((r) => !!selectedKeys?.has(seatCellKey(r, col)));

  const rowLabelClassName = (rowLabel: string) => {
    if (copySourceRow === rowLabel) return "bg-primary text-primary-foreground";
    if (isPickingCopySource) return "bg-accent text-accent-foreground";
    if (isRowSelected(rowLabel)) return "bg-primary/15 text-primary";
    return undefined;
  };

  const colLabelClassName = (col: number) => (isColSelected(col) ? "bg-primary/15 text-primary" : undefined);

  const rowLabelTitle = (rowLabel: string) => {
    if (isPickingCopySource) return `Bấm để sao chép hàng ${rowLabel}`;
    if (copySourceRow) {
      return copySourceRow === rowLabel
        ? `Hàng ${rowLabel} đã được sao chép`
        : `Bấm để dán hàng ${copySourceRow} vào hàng ${rowLabel}`;
    }
    return `Chọn cả hàng ${rowLabel}`;
  };

  return (
    <SeatMapFrame legend={previewMode ? PREVIEW_LEGEND_ITEMS : EDITOR_LEGEND_ITEMS} className={cn(cursorClassFor(activeBrush, selectionMode, previewMode))}>
      <SeatGrid
        rowLabels={rowLabels}
        cols={cols}
        cellAt={cellAt}
        curve={previewMode ? undefined : 0}
        fitWidth={previewMode}
        compactAisles={previewMode}
        showColLabels={!previewMode}
        onColLabelClick={!previewMode ? onColLabelClick : undefined}
        colLabelClassName={colLabelClassName}
        onRowLabelClick={!previewMode ? onRowLabelClick : undefined}
        rowLabelClassName={rowLabelClassName}
        rowLabelTitle={rowLabelTitle}
        renderCell={(cell, rowLabel, col) => {
          if (!cell) return null;
          if (previewMode && !cell.isAisle && !cell.isActive) return null;
          const key = seatCellKey(rowLabel, col);
          const isPending = pendingCoupleKey === key;
          const isSelected = !previewMode && selectedKeys?.has(key);
          const hasIssue = !previewMode && issueKeys?.has(key);
          const mark: SeatMark | undefined = isPending
            ? "pending"
            : hasIssue
              ? "issue"
              : isSelected
                ? "range"
                : undefined;
          const state = cell.isAisle ? "aisle" : cell.isActive ? "available" : "inactive";
          const kind = resolveSeatVisualKind(cell.seatTypeName);
          const label = cell.isAisle
            ? ""
            : cell.coupleSpan === 2
              ? `${col}-${col + 1}`
              : `${col}`;
          const isCouple = cell.coupleSpan === 2;
          const code = isCouple
            ? `${rowLabel}${col}-${rowLabel}${col + 1}`
            : `${rowLabel}${col}`;
          const typeLabel = cell.seatTypeName ? seatTypeLabel(cell.seatTypeName) : null;
          const description = cell.isAisle
            ? `Ô ${code}, lối đi`
            : `Ghế ${code}${typeLabel ? `, ${typeLabel}` : ""}, ${cell.isActive ? "đang bật" : "đã tắt"}`;
          return (
            <SeatButton
              label={label}
              kind={kind}
              state={state}
              editable={!previewMode}
              disabled={previewMode}
              onClick={previewMode || selectionMode ? undefined : () => onCellClick(rowLabel, col)}
              onMouseDown={!previewMode && selectionMode ? () => handleCellMouseDown(rowLabel, col) : undefined}
              onMouseEnter={!previewMode && selectionMode ? () => handleCellMouseEnter(rowLabel, col) : undefined}
              mark={mark}
              seatKey={key}
              className={cn(!previewMode && HOVER_PREVIEW)}
              ariaLabel={description}
              title={description.replace(/^Ghế /, "")}
            />
          );
        }}
      />
    </SeatMapFrame>
  );
};

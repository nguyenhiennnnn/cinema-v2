import { useEffect, useRef, useState } from "react";
import { cn } from "@/lib/utils";
import { CinemaScreen, SeatLegend } from "@/components/common/seat-map/seat-legend";
import { SeatButton } from "@/components/common/seat-map/seat-button";
import { SeatGrid } from "@/components/common/seat-map/seat-grid";
import { useSeatGridModel } from "@/components/common/seat-map/use-seat-grid-model";
import { resolveSeatVisualKind, seatCellKey } from "@/components/common/seat-map/seat-map-types";
import type { SeatTypeDto } from "@/features/admin/catalog/types";
import type { EditorCell } from "../hooks/use-seat-editor-state";

interface SeatEditorGridProps {
  rows: number;
  cols: number;
  cellMap: Map<string, EditorCell>;
  seatTypes: SeatTypeDto[] | undefined;
  pendingCoupleKey: string | null;
  onCellClick: (rowLabel: string, col: number) => void;

  previewMode?: boolean;

  selectionMode?: boolean;
  selectedKeys?: Set<string>;
  onSelectionChange?: (keys: string[]) => void;

  onRowLabelClick?: (rowLabel: string) => void;
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
  previewMode = false,
  selectionMode = false,
  selectedKeys,
  onSelectionChange,
  onRowLabelClick,
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

  // ── Drag-to-select ─────────────────────────────────────────────────────
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

  const rowLabelClassName = (rowLabel: string) => {
    if (copySourceRow === rowLabel) return "bg-primary/15 text-primary ring-1 ring-primary/40";
    if (isPickingCopySource) return "bg-accent/15 text-accent-foreground";
    return undefined;
  };

  const rowLabelTitle = (rowLabel: string) => {
    if (isPickingCopySource) return `Bấm để sao chép hàng ${rowLabel}`;
    if (copySourceRow) {
      return copySourceRow === rowLabel
        ? `Hàng ${rowLabel} đã được sao chép`
        : `Bấm để dán hàng ${copySourceRow} vào hàng ${rowLabel}`;
    }
    return undefined;
  };

  return (
    <div className="border border-border bg-card p-4">
      <CinemaScreen />
      <SeatGrid
        rowLabels={rowLabels}
        cols={cols}
        cellAt={cellAt}
        onRowLabelClick={!previewMode ? onRowLabelClick : undefined}
        rowLabelClassName={rowLabelClassName}
        rowLabelTitle={rowLabelTitle}
        renderCell={(cell, rowLabel, col) => {
          if (!cell) return null;
          const key = seatCellKey(rowLabel, col);
          const isPending = pendingCoupleKey === key;
          const isSelected = !previewMode && selectedKeys?.has(key);
          const state = cell.isAisle ? "aisle" : cell.isActive ? "available" : "inactive";
          const label = cell.isAisle
            ? ""
            : cell.coupleSpan === 2
              ? `${col}-${col + 1}`
              : `${col}`;
          return (
            <SeatButton
              label={label}
              kind={resolveSeatVisualKind(cell.seatTypeName)}
              state={state}
              editable={!previewMode}
              disabled={previewMode}
              onClick={previewMode || selectionMode ? undefined : () => onCellClick(rowLabel, col)}
              onMouseDown={!previewMode && selectionMode ? () => handleCellMouseDown(rowLabel, col) : undefined}
              onMouseEnter={!previewMode && selectionMode ? () => handleCellMouseEnter(rowLabel, col) : undefined}
              className={cn(
                isPending && "ring-2 ring-couple ring-offset-1 ring-offset-background",
                isSelected && "ring-2 ring-primary ring-offset-1 ring-offset-background bg-primary/10!",
              )}
              title={cell.coupleId ? "Ghế đôi" : undefined}
            />
          );
        }}
      />
      <div className="mt-6">
        <SeatLegend
          items={
            previewMode
              ? [
                { label: "Trống", className: "bg-secondary border-border" },
                { label: "VIP", className: "bg-vip/15 border-vip/50" },
                { label: "Ghế đôi", className: "bg-couple/15 border-couple/50" },
                { label: "Đã tắt", className: "bg-muted/20 border-dashed border-border/40" },
                { label: "Lối đi", className: "bg-transparent border-transparent" },
              ]
              : [
                { label: "Trống", className: "bg-secondary border-border" },
                { label: "VIP", className: "bg-accent/20 border-accent/60" },
                { label: "Ghế đôi", className: "bg-couple/15 border-couple/50" },
                { label: "Đã tắt", className: "bg-muted/20 border-dashed border-border/40" },
                { label: "Lối đi", className: "bg-transparent border-transparent" },
              ]
          }
        />
      </div>
    </div>
  );
}

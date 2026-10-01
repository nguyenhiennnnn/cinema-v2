import { useState } from "react";
import {
  generateRowLabel,
  rowLabelToIndex,
  seatCellKey,
} from "@/components/common/seat-map/seat-map-types";
import type { SeatDto, UpdateSeatLayoutRequest } from "../types";

export interface EditorCell {
  seatRow: string;
  seatNumber: number;
  seatTypeId: number | null;
  isActive: boolean;
  isAisle: boolean;
  coupleId: string | null;
}

export type SeatEditorBrush =
  | { type: "seatType"; seatTypeId: number }
  | { type: "toggleActive" }
  | { type: "toggleAisle" }
  | { type: "pairCouple" };

export interface ValidationIssue {
  level: "error" | "warning";
  message: string;
}

export interface AutoGenerateOptions {
  rows: number;
  cols: number;
  standardSeatTypeId: number | null;
  vipSeatTypeId: number | null;
  vipRowCount: number;
  coupleRowCount: number;
  aisleCols: number[];
}

function newCoupleId(seed: string): string {
  return typeof crypto !== "undefined" && "randomUUID" in crypto
    ? crypto.randomUUID()
    : `couple-${seed}-${Date.now()}-${Math.random().toString(16).slice(2)}`;
}

function buildInitialCells(
  rows: number,
  cols: number,
  seats: SeatDto[] | undefined,
  defaultSeatTypeId: number | null,
): Map<string, EditorCell> {
  const bySeat = new Map<string, SeatDto>();
  seats?.forEach((s) => bySeat.set(seatCellKey(s.seatRow, s.seatNumber), s));

  const map = new Map<string, EditorCell>();
  for (let r = 0; r < rows; r++) {
    const rowLabel = generateRowLabel(r);
    for (let c = 1; c <= cols; c++) {
      const key = seatCellKey(rowLabel, c);
      const existing = bySeat.get(key);
      map.set(
        key,
        existing
          ? {
              seatRow: existing.seatRow,
              seatNumber: existing.seatNumber,
              seatTypeId: existing.seatTypeId ?? defaultSeatTypeId,
              isActive: existing.isActive,
              isAisle: existing.isAisle,
              coupleId: existing.coupleId ?? null,
            }
          : {
              seatRow: rowLabel,
              seatNumber: c,
              seatTypeId: defaultSeatTypeId,
              isActive: true,
              isAisle: false,
              coupleId: null,
            },
      );
    }
  }
  return map;
}

interface Snapshot {
  rows: number;
  cols: number;
  cells: Map<string, EditorCell>;
}

const HISTORY_LIMIT = 40;

interface UseSeatEditorStateParams {
  initialRows: number;
  initialCols: number;
  initialSeats: SeatDto[] | undefined;
  defaultSeatTypeId: number | null;
}

export function useSeatEditorState({
  initialRows,
  initialCols,
  initialSeats,
  defaultSeatTypeId,
}: UseSeatEditorStateParams) {
  const [present, setPresent] = useState<Snapshot>(() => ({
    rows: initialRows,
    cols: initialCols,
    cells: buildInitialCells(initialRows, initialCols, initialSeats, defaultSeatTypeId),
  }));
  const { rows, cols, cells } = present;

  const [past, setPast] = useState<Snapshot[]>([]);
  const [future, setFuture] = useState<Snapshot[]>([]);

  const [pendingCoupleKey, setPendingCoupleKey] = useState<string | null>(null);
  const [isDirty, setIsDirty] = useState(false);

  const [selectedKeys, setSelectedKeys] = useState<Set<string>>(new Set());

  const [copiedRow, setCopiedRow] = useState<{ sourceRow: string; cells: EditorCell[] } | null>(
    null,
  );

  const commit = (next: Snapshot) => {
    setPast((prev) => [...prev, present].slice(-HISTORY_LIMIT));
    setFuture([]);
    setPresent(next);
    setIsDirty(true);
  };

  const undo = () => {
    if (past.length === 0) return;
    const previous = past[past.length - 1];
    setPast((prev) => prev.slice(0, -1));
    setFuture((prev) => [present, ...prev]);
    setPresent(previous);
    setPendingCoupleKey(null);
    setSelectedKeys(new Set());
    setIsDirty(true);
  };

  const redo = () => {
    if (future.length === 0) return;
    const next = future[0];
    setFuture((prev) => prev.slice(1));
    setPast((prev) => [...prev, present]);
    setPresent(next);
    setPendingCoupleKey(null);
    setSelectedKeys(new Set());
    setIsDirty(true);
  };

  const resize = (newRows: number, newCols: number) => {
    const next = new Map<string, EditorCell>();
    for (let r = 0; r < newRows; r++) {
      const rowLabel = generateRowLabel(r);
      for (let c = 1; c <= newCols; c++) {
        const key = seatCellKey(rowLabel, c);
        next.set(
          key,
          cells.get(key) ?? {
            seatRow: rowLabel,
            seatNumber: c,
            seatTypeId: defaultSeatTypeId,
            isActive: true,
            isAisle: false,
            coupleId: null,
          },
        );
      }
    }
    commit({ rows: newRows, cols: newCols, cells: next });
    setPendingCoupleKey(null);
    setSelectedKeys(new Set());
  };

  const applyPairCouple = (key: string, cell: EditorCell): string | null => {
    if (cell.isAisle) return "Không thể ghép ghế đôi tại ô lối đi.";

    if (cell.coupleId) {
      const coupleId = cell.coupleId;
      const next = new Map(cells);
      next.forEach((c, k) => {
        if (c.coupleId === coupleId) next.set(k, { ...c, coupleId: null });
      });
      commit({ rows, cols, cells: next });
      setPendingCoupleKey(null);
      return null;
    }

    if (!pendingCoupleKey) {
      setPendingCoupleKey(key);
      return null;
    }

    if (pendingCoupleKey === key) {
      setPendingCoupleKey(null);
      return null;
    }

    const pendingCell = cells.get(pendingCoupleKey);
    if (
      !pendingCell ||
      pendingCell.isAisle ||
      pendingCell.seatRow !== cell.seatRow ||
      Math.abs(pendingCell.seatNumber - cell.seatNumber) !== 1
    ) {
      setPendingCoupleKey(key);
      return "Chỉ ghép được 2 ghế liền kề trong cùng một hàng.";
    }

    const coupleId = newCoupleId(key);
    const next = new Map(cells);
    next.set(pendingCoupleKey, { ...pendingCell, coupleId });
    next.set(key, { ...cell, coupleId });
    commit({ rows, cols, cells: next });
    setPendingCoupleKey(null);
    return null;
  };

  const applyBrush = (
    rowLabel: string,
    col: number,
    brush: SeatEditorBrush,
  ): string | null => {
    const key = seatCellKey(rowLabel, col);
    const cell = cells.get(key);
    if (!cell) return null;

    if (brush.type === "pairCouple") {
      return applyPairCouple(key, cell);
    }

    const next = new Map(cells);
    const current = next.get(key)!;
    if (brush.type === "seatType") {
      next.set(key, { ...current, seatTypeId: brush.seatTypeId, isAisle: false });
    } else if (brush.type === "toggleActive") {
      next.set(key, { ...current, isActive: !current.isActive });
    } else if (brush.type === "toggleAisle") {
      next.set(key, {
        ...current,
        isAisle: !current.isAisle,
        coupleId: !current.isAisle ? null : current.coupleId,
      });
    }
    commit({ rows, cols, cells: next });
    return null;
  };

  const setSelectionKeys = (keys: string[]) => setSelectedKeys(new Set(keys));

  const clearSelection = () => setSelectedKeys(new Set());

  const applyBrushToSelection = (brush: SeatEditorBrush): string | null => {
    if (selectedKeys.size === 0) return "Chưa chọn ô ghế nào để áp dụng.";
    if (brush.type === "pairCouple") {
      return "Không thể ghép ghế đôi hàng loạt — hãy chọn từng cặp ghế bằng công cụ ghép ghế đôi.";
    }

    const next = new Map(cells);
    selectedKeys.forEach((key) => {
      const current = next.get(key);
      if (!current) return;
      if (brush.type === "seatType") {
        next.set(key, { ...current, seatTypeId: brush.seatTypeId, isAisle: false });
      } else if (brush.type === "toggleActive") {
        next.set(key, { ...current, isActive: !current.isActive });
      } else if (brush.type === "toggleAisle") {
        next.set(key, {
          ...current,
          isAisle: !current.isAisle,
          coupleId: !current.isAisle ? null : current.coupleId,
        });
      }
    });
    commit({ rows, cols, cells: next });
    setSelectedKeys(new Set());
    return null;
  };

  const copyRow = (rowLabel: string) => {
    const rowCells = Array.from(cells.values())
      .filter((c) => c.seatRow === rowLabel)
      .sort((a, b) => a.seatNumber - b.seatNumber);
    if (rowCells.length === 0) return;
    setCopiedRow({ sourceRow: rowLabel, cells: rowCells });
  };

  const cancelCopyRow = () => setCopiedRow(null);

  const pasteRow = (targetRowLabel: string): string | null => {
    if (!copiedRow) return "Chưa sao chép hàng ghế nào.";
    if (targetRowLabel === copiedRow.sourceRow) {
      return "Không thể dán vào chính hàng ghế đã sao chép.";
    }

    const coupleIdMap = new Map<string, string>();
    const next = new Map(cells);
    copiedRow.cells.forEach((source) => {
      const key = seatCellKey(targetRowLabel, source.seatNumber);
      if (!next.has(key)) return;

      let coupleId: string | null = null;
      if (source.coupleId) {
        if (!coupleIdMap.has(source.coupleId)) {
          coupleIdMap.set(source.coupleId, newCoupleId(`${targetRowLabel}-${source.seatNumber}`));
        }
        coupleId = coupleIdMap.get(source.coupleId)!;
      }

      next.set(key, {
        seatRow: targetRowLabel,
        seatNumber: source.seatNumber,
        seatTypeId: source.seatTypeId,
        isActive: source.isActive,
        isAisle: source.isAisle,
        coupleId,
      });
    });

    commit({ rows, cols, cells: next });
    return null;
  };

  const validate = (): ValidationIssue[] => {
    const issues: ValidationIssue[] = [];
    const list = Array.from(cells.values());

    list.forEach((c) => {
      const rowIndex = rowLabelToIndex(c.seatRow);
      if (rowIndex < 0 || rowIndex >= rows || c.seatNumber < 1 || c.seatNumber > cols) {
        issues.push({
          level: "error",
          message: `Ghế ${c.seatRow}${c.seatNumber} nằm ngoài kích thước phòng đã khai báo (${rows} hàng x ${cols} cột).`,
        });
      }
    });

    const seenKeys = new Set<string>();
    list.forEach((c) => {
      const key = seatCellKey(c.seatRow, c.seatNumber);
      if (seenKeys.has(key)) {
        issues.push({ level: "error", message: `Ghế ${c.seatRow}${c.seatNumber} bị trùng vị trí.` });
      }
      seenKeys.add(key);
    });

    const coupleGroups = new Map<string, EditorCell[]>();
    list.forEach((c) => {
      if (!c.coupleId) return;
      const arr = coupleGroups.get(c.coupleId) ?? [];
      arr.push(c);
      coupleGroups.set(c.coupleId, arr);
    });
    coupleGroups.forEach((group) => {
      if (group.length !== 2) {
        const where = group[0] ? `hàng ${group[0].seatRow}` : "";
        issues.push({
          level: "error",
          message: `Có cặp ghế đôi ${where} gồm ${group.length} ghế thay vì 2 (ghế đôi bị lẻ).`,
        });
        return;
      }
      const [a, b] = group.sort((x, y) => x.seatNumber - y.seatNumber);
      if (a.seatRow !== b.seatRow || b.seatNumber - a.seatNumber !== 1) {
        issues.push({
          level: "warning",
          message: `Cặp ghế đôi hàng ${a.seatRow} (${a.seatNumber} - ${b.seatNumber}) không liền kề nhau.`,
        });
      }
    });

    const activeByRow = new Map<string, number>();
    const aisleByRow = new Map<string, number>();
    list.forEach((c) => {
      if (c.isAisle) {
        aisleByRow.set(c.seatRow, (aisleByRow.get(c.seatRow) ?? 0) + 1);
      } else if (c.isActive) {
        activeByRow.set(c.seatRow, (activeByRow.get(c.seatRow) ?? 0) + 1);
      }
    });
    if (cols > 1) {
      activeByRow.forEach((activeCount, rowLabel) => {
        if (activeCount > 0 && !aisleByRow.has(rowLabel)) {
          issues.push({
            level: "warning",
            message: `Hàng ${rowLabel} không có ô lối đi nào — ghế trong hàng có thể bị "mồ côi", không có lối vào/ra.`,
          });
        }
      });
    }

    return issues;
  };

  const autoGenerate = (options: AutoGenerateOptions) => {
    const {
      rows: newRows,
      cols: newCols,
      standardSeatTypeId,
      vipSeatTypeId,
      vipRowCount,
      coupleRowCount,
      aisleCols,
    } = options;

    const next = new Map<string, EditorCell>();

    for (let r = 0; r < newRows; r++) {
      const rowLabel = generateRowLabel(r);
      const isVipRow = vipRowCount > 0 && r >= newRows - vipRowCount;
      const isCoupleRow = coupleRowCount > 0 && r >= newRows - coupleRowCount;
      const rowSeatTypeId = isVipRow && vipSeatTypeId ? vipSeatTypeId : standardSeatTypeId;

      const coupleIdByCol = new Map<number, string>();
      if (isCoupleRow) {
        let pending: number | null = null;
        for (let c = 1; c <= newCols; c++) {
          if (aisleCols.includes(c)) {
            pending = null;
            continue;
          }
          if (pending === null) {
            pending = c;
          } else {
            const id = newCoupleId(`${rowLabel}-${pending}`);
            coupleIdByCol.set(pending, id);
            coupleIdByCol.set(c, id);
            pending = null;
          }
        }
      }

      for (let c = 1; c <= newCols; c++) {
        const key = seatCellKey(rowLabel, c);
        const isAisleCol = aisleCols.includes(c);
        next.set(key, {
          seatRow: rowLabel,
          seatNumber: c,
          seatTypeId: isAisleCol ? null : rowSeatTypeId,
          isActive: !isAisleCol,
          isAisle: isAisleCol,
          coupleId: isAisleCol ? null : coupleIdByCol.get(c) ?? null,
        });
      }
    }

    commit({ rows: newRows, cols: newCols, cells: next });
    setPendingCoupleKey(null);
    setSelectedKeys(new Set());
    setCopiedRow(null);
  };

  const cellList = Array.from(cells.values());

  const buildPayload = (): UpdateSeatLayoutRequest => ({
    rows,
    cols,
    seats: cellList.map((c) => ({
      seatRow: c.seatRow,
      seatNumber: c.seatNumber,
      seatTypeId: c.isAisle ? null : c.seatTypeId,
      isActive: c.isAisle ? false : c.isActive,
      isAisle: c.isAisle,
      coupleId: c.coupleId,
    })),
  });

  return {
    rows,
    cols,
    cellMap: cells,
    cellList,
    pendingCoupleKey,
    isDirty,
    resize,
    applyBrush,
    buildPayload,
    markSaved: () => setIsDirty(false),

    // history
    canUndo: past.length > 0,
    canRedo: future.length > 0,
    undo,
    redo,

    // multi-select / bulk edit
    selectedKeys,
    setSelectionKeys,
    clearSelection,
    applyBrushToSelection,

    // copy/paste row
    copiedRow,
    copyRow,
    cancelCopyRow,
    pasteRow,

    // validation
    validate,

    // auto-generate
    autoGenerate,
  };
}

import { rowLabelToIndex, seatCellKey } from "@/components/common/seat-map/seat-map-types";
import type { EditorCell, ValidationIssue } from "../types/seat-editor";

const increment = (map: Map<string, number>, key: string) =>
  map.set(key, (map.get(key) ?? 0) + 1);

function validateBounds(list: EditorCell[], rows: number, cols: number): ValidationIssue[] {
  return list
    .filter((c) => {
      const rowIndex = rowLabelToIndex(c.seatRow);
      return rowIndex < 0 || rowIndex >= rows || c.seatNumber < 1 || c.seatNumber > cols;
    })
    .map((c) => ({
      level: "error",
      message: `Ghế ${c.seatRow}${c.seatNumber} nằm ngoài kích thước phòng đã khai báo (${rows} hàng x ${cols} cột).`,
    }));
}

function validateDuplicates(list: EditorCell[]): ValidationIssue[] {
  const issues: ValidationIssue[] = [];
  const seenKeys = new Set<string>();
  list.forEach((c) => {
    const key = seatCellKey(c.seatRow, c.seatNumber);
    if (seenKeys.has(key)) {
      issues.push({ level: "error", message: `Ghế ${c.seatRow}${c.seatNumber} bị trùng vị trí.` });
    }
    seenKeys.add(key);
  });
  return issues;
}

function validateCouples(list: EditorCell[]): ValidationIssue[] {
  const issues: ValidationIssue[] = [];
  const groups = new Map<string, EditorCell[]>();
  list.forEach((c) => {
    if (!c.coupleId) return;
    groups.set(c.coupleId, [...(groups.get(c.coupleId) ?? []), c]);
  });

  groups.forEach((group) => {
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
  return issues;
}

function validateAisles(list: EditorCell[], cols: number): ValidationIssue[] {
  if (cols <= 1) return [];
  const activeByRow = new Map<string, number>();
  const aisleByRow = new Map<string, number>();
  list.forEach((c) => {
    if (c.isAisle) increment(aisleByRow, c.seatRow);
    else if (c.isActive) increment(activeByRow, c.seatRow);
  });

  const issues: ValidationIssue[] = [];
  activeByRow.forEach((activeCount, rowLabel) => {
    if (activeCount > 0 && !aisleByRow.has(rowLabel)) {
      issues.push({
        level: "warning",
        message: `Hàng ${rowLabel} không có ô lối đi nào — ghế trong hàng có thể bị "mồ côi", không có lối vào/ra.`,
      });
    }
  });
  return issues;
}

export function validateSeatLayout(
  list: EditorCell[],
  rows: number,
  cols: number,
): ValidationIssue[] {
  return [
    ...validateBounds(list, rows, cols),
    ...validateDuplicates(list),
    ...validateCouples(list),
    ...validateAisles(list, cols),
  ];
}

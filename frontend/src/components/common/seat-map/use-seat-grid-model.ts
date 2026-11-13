import { useMemo } from "react";
import {
  generateRowLabel,
  rowLabelToIndex,
  seatCellKey,
  type SeatCellData,
  type SeatGridCell,
} from "./seat-map-types";

interface UseSeatGridModelParams {
  rowsCount: number;
  cols: number;
  cells: SeatCellData[];
}

export function useSeatGridModel({
  rowsCount,
  cols,
  cells,
}: UseSeatGridModelParams) {
  return useMemo(() => {
    const maxRowFromCells = cells.reduce(
      (max, c) => (c.seatRow ? Math.max(max, rowLabelToIndex(c.seatRow) + 1) : max),
      0,
    );
    const finalRowCount =
      maxRowFromCells > 0 ? Math.min(rowsCount, maxRowFromCells) : rowsCount;

    const rowLabels = Array.from({ length: finalRowCount }, (_, i) =>
      generateRowLabel(i),
    );

    const map = new Map<string, SeatGridCell>();
    for (const cell of cells) {
      map.set(seatCellKey(cell.seatRow, cell.seatNumber), { ...cell });
    }

    const byCouple = new Map<string, SeatCellData[]>();
    for (const cell of cells) {
      if (!cell.coupleId) continue;
      const groupKey = `${cell.coupleId}|${cell.seatRow}`;
      const group = byCouple.get(groupKey);
      if (group) group.push(cell);
      else byCouple.set(groupKey, [cell]);
    }

    const processedKeys = new Set<string>();
    for (const cell of cells) {
      const key = seatCellKey(cell.seatRow, cell.seatNumber);
      if (!cell.coupleId || processedKeys.has(key)) continue;

      const partner = byCouple
        .get(`${cell.coupleId}|${cell.seatRow}`)
        ?.find((c) => c.seatNumber !== cell.seatNumber);

      if (partner) {
        const [first, second] = [cell, partner].sort(
          (a, b) => a.seatNumber - b.seatNumber,
        );
        const firstKey = seatCellKey(first.seatRow, first.seatNumber);
        const secondKey = seatCellKey(second.seatRow, second.seatNumber);

        map.set(firstKey, { ...map.get(firstKey)!, coupleSpan: 2 });
        map.set(secondKey, { ...map.get(secondKey)!, isCoupleSecondary: true });
        processedKeys.add(firstKey);
        processedKeys.add(secondKey);
      }
    }

    return {
      rowLabels,
      cellAt: (rowLabel: string, col: number) =>
        map.get(seatCellKey(rowLabel, col)),
    };
  }, [rowsCount, cols, cells]);
}

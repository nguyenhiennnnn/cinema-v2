import { useMemo } from "react";
import type { SeatTypeDto } from "@/features/admin/catalog/types";
import type { EditorCellMap } from "../types/seat-editor";
import { seatTypeLabel } from "@/components/common/seat-map/seat-kind";

interface SeatEditorStatsProps {
  cellMap: EditorCellMap;
  seatTypes: SeatTypeDto[] | undefined;
}

const Stat = ({ label, value }: { label: string; value: number }) => (
  <div className="flex items-baseline gap-1.5">
    <dt className="text-muted-foreground">{label}</dt>
    <dd className="font-semibold tabular-nums text-foreground">{value}</dd>
  </div>
);

export const SeatEditorStats = ({ cellMap, seatTypes }: SeatEditorStatsProps) => {
  const stats = useMemo(() => {
    const byType = new Map<number, number>();
    const couples = new Set<string>();
    let sellable = 0;
    let disabled = 0;
    let aisles = 0;

    cellMap.forEach((cell) => {
      if (cell.isAisle) {
        aisles += 1;
        return;
      }
      if (!cell.isActive) {
        disabled += 1;
        return;
      }
      sellable += 1;
      if (cell.seatTypeId !== null) byType.set(cell.seatTypeId, (byType.get(cell.seatTypeId) ?? 0) + 1);
      if (cell.coupleId) couples.add(cell.coupleId);
    });

    return { byType, couplePairs: couples.size, sellable, disabled, aisles };
  }, [cellMap]);

  return (
    <dl className="flex flex-wrap gap-x-6 gap-y-1.5 bg-card px-4 py-3 text-xs" aria-label="Thống kê sơ đồ ghế">
      <Stat label="Tổng số ghế" value={stats.sellable} />
      {seatTypes?.map((st) => (
        <Stat key={st.seatTypeId} label={seatTypeLabel(st.name)} value={stats.byType.get(st.seatTypeId) ?? 0} />
      ))}
      <Stat label="Cặp ghế đôi" value={stats.couplePairs} />
      <Stat label="Ghế đã tắt" value={stats.disabled} />
      <Stat label="Ô lối đi" value={stats.aisles} />
    </dl>
  );
};

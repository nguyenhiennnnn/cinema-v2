import { ShowtimeSeatStatus } from "@/types/enum";
import type {
  AislePositionDto,
  ShowtimeSeatItemDto,
} from "@/features/client/showtimes/types";
import { formatCurrency } from "@/lib/formatters";
import type { SeatLegendItem } from "./seat-legend";
import { SeatButton } from "./seat-button";
import { SeatGrid } from "./seat-grid";
import { SeatMapFrame } from "./seat-map-frame";
import { useSeatGridModel } from "./use-seat-grid-model";
import { toCellState } from "./seat-map-utils";
import {
  resolveSeatVisualKind,
  seatCellKey,
  type SeatCellData,
  type SeatCellState,
  type SeatVisualKind,
} from "./seat-map-types";

interface SelectableSeatMapProps {
  rows: number;
  cols: number;
  aisles: AislePositionDto[];
  seats: ShowtimeSeatItemDto[];
  statusMap: Map<number, ShowtimeSeatStatus>;
  selectedIds: Set<number>;
  onToggle: (seat: ShowtimeSeatItemDto) => void;
  highlightedIds?: Set<number>;
  fitWidth?: boolean;
  className?: string;
}

const STATE_TEXT: Record<SeatCellState, string> = {
  available: "trống",
  selected: "đang chọn",
  held: "đang giữ",
  booked: "đã bán",
  inactive: "đã tắt",
  aisle: "lối đi",
};

const LEGEND: SeatLegendItem[] = [
  { label: "Thường", state: "available", type: "standard", group: "type" },
  { label: "VIP", state: "available", type: "vip", group: "type" },
  { label: "Ghế đôi", state: "available", type: "couple", group: "type" },
  { label: "Đang chọn", state: "selected", type: "standard", group: "status" },
  { label: "Đang giữ", state: "held", type: "standard", group: "status" },
  { label: "Đã bán", state: "booked", type: "standard", group: "status" },
];

const buildLegend = (seats: ShowtimeSeatItemDto[]): SeatLegendItem[] => {
  const priceByKind = new Map<SeatVisualKind, number>();
  for (const seat of seats) {
    const kind = resolveSeatVisualKind(seat.seatType);
    if (!priceByKind.has(kind) && seat.price != null) priceByKind.set(kind, seat.price);
  }
  return LEGEND.map((item) => {
    const price = item.group === "type" && item.type ? priceByKind.get(item.type) : undefined;
    return price != null ? { ...item, hint: `${formatCurrency(price)}/ghế` } : item;
  }).filter((item) => item.group !== "type" || item.hint || item.type === "standard");
};

export const SelectableSeatMap = ({
  rows,
  cols,
  aisles,
  seats,
  statusMap,
  selectedIds,
  onToggle,
  highlightedIds,
  fitWidth,
  className,
}: SelectableSeatMapProps) => {
  const cells: SeatCellData[] = seats.map((seat) => ({
    seatRow: seat.seatRow,
    seatNumber: seat.seatNumber,
    showtimeSeatId: seat.showtimeSeatId,
    seatTypeId: seat.seatTypeId,
    seatTypeName: seat.seatType,
    coupleId: seat.coupleId,
    price: seat.price,
    isActive: true,
    isAisle: false,
  }));

  aisles.forEach((a) => {
    const key = seatCellKey(a.seatRow, a.seatNumber);
    if (!cells.some((c) => seatCellKey(c.seatRow, c.seatNumber) === key)) {
      cells.push({
        seatRow: a.seatRow,
        seatNumber: a.seatNumber,
        isActive: false,
        isAisle: true,
      });
    }
  });

  const { rowLabels, cellAt } = useSeatGridModel({ rowsCount: rows, cols, cells });

  return (
    <SeatMapFrame legend={buildLegend(seats)} className={className}>
      <SeatGrid
        rowLabels={rowLabels}
        cols={cols}
        cellAt={cellAt}
        fitWidth={fitWidth}
        compactAisles
        renderCell={(cell, _rowLabel, col) => {
          if (!cell) return null;
          if (cell.isAisle) {
            return <SeatButton label="" state="aisle" />;
          }
          if (cell.showtimeSeatId == null) return null;

          const status = statusMap.get(cell.showtimeSeatId) ?? ShowtimeSeatStatus.Available;
          const isSelected = selectedIds.has(cell.showtimeSeatId);
          const state = toCellState(status, isSelected);
          const isCouple = cell.coupleSpan === 2;
          const label = isCouple ? `${col}-${col + 1}` : `${col}`;
          const code = isCouple
            ? `${cell.seatRow}${cell.seatNumber}-${cell.seatRow}${cell.seatNumber + 1}`
            : `${cell.seatRow}${cell.seatNumber}`;

          return (
            <SeatButton
              label={label}
              ariaLabel={`Ghế ${code}, ${STATE_TEXT[state]}`}
              kind={resolveSeatVisualKind(cell.seatTypeName)}
              state={state}
              mark={highlightedIds?.has(cell.showtimeSeatId) ? "suggested" : undefined}
              onClick={() =>
                onToggle({
                  showtimeSeatId: cell.showtimeSeatId!,
                  seatRow: cell.seatRow,
                  seatNumber: cell.seatNumber,
                  seatTypeId: cell.seatTypeId ?? null,
                  seatType: cell.seatTypeName ?? null,
                  coupleId: cell.coupleId ?? null,
                  status,
                  price: cell.price ?? 0,
                })
              }
              title={`${code}${cell.seatTypeName ? `, ${cell.seatTypeName}` : ""}`}
            />
          );
        }}
      />
    </SeatMapFrame>
  );
};

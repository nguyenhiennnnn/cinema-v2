import { cn } from "@/lib/utils";
import { ShowtimeSeatStatus } from "@/types/enum";
import type {
  AislePositionDto,
  ShowtimeSeatItemDto,
} from "@/features/client/showtimes/types";
import { CinemaScreen, SeatLegend } from "./seat-legend";
import { SeatButton } from "./seat-button";
import { SeatGrid } from "./seat-grid";
import { useSeatGridModel } from "./use-seat-grid-model";
import { resolveSeatVisualKind, seatCellKey, type SeatCellData, type SeatCellState } from "./seat-map-types";

interface SelectableSeatMapProps {
  rows: number;
  cols: number;
  aisles: AislePositionDto[];
  seats: ShowtimeSeatItemDto[];
  statusMap: Map<number, ShowtimeSeatStatus>;
  selectedIds: Set<number>;
  onToggle: (seat: ShowtimeSeatItemDto) => void;
  className?: string;
}

function toCellState(status: ShowtimeSeatStatus, isSelected: boolean): SeatCellState {
  if (isSelected) return "selected";
  if (status === ShowtimeSeatStatus.Booked) return "booked";
  if (status === ShowtimeSeatStatus.Held) return "held";
  return "available";
}

export const SelectableSeatMap = ({
  rows,
  cols,
  aisles,
  seats,
  statusMap,
  selectedIds,
  onToggle,
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
    <div className={cn("bg-card p-6 cut-lg", className)}>
      <CinemaScreen />

      <SeatGrid
        rowLabels={rowLabels}
        cols={cols}
        cellAt={cellAt}
        renderCell={(cell, col) => {
          if (!cell) return null;
          if (cell.isAisle) {
            return <SeatButton label="" state="aisle" />;
          }
          if (cell.showtimeSeatId == null) return null;

          const status = statusMap.get(cell.showtimeSeatId) ?? ShowtimeSeatStatus.Available;
          const isSelected = selectedIds.has(cell.showtimeSeatId);
          const label = cell.coupleSpan === 2 ? `${cell.seatRow}${col}` : `${col}`;

          return (
            <SeatButton
              label={label}
              ariaLabel={`Ghế ${cell.seatRow}${cell.seatNumber}`}
              kind={resolveSeatVisualKind(cell.seatTypeName)}
              state={toCellState(status, isSelected)}
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
              title={`${cell.seatRow}${cell.seatNumber}, ${cell.seatTypeName ?? ""}`}
            />
          );
        }}
      />

      <div className="mt-8 border-t border-border/50 pt-5">
        <SeatLegend
          items={[
            { label: "Trống", state: "available", type: "standard" },
            { label: "VIP", state: "available", type: "vip" },
            { label: "Ghế đôi", state: "available", type: "couple" },
            { label: "Đang chọn", state: "selected", type: "standard" },
            { label: "Đang giữ", state: "held", type: "standard" },
            { label: "Đã bán", state: "booked", type: "standard" },
          ]}
        />
      </div>
    </div>
  );
};

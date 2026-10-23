import { Skeleton } from "@/components/ui/skeleton";
import { SelectableSeatMap } from "@/components/common/seat-map/selectable-seat-map";
import { ShowtimeSeatStatus } from "@/types/enum";
import type { ShowtimeSeatsResponseDto } from "@/features/client/showtimes/types";
import { formatDateTime } from "@/lib/formatters";

interface CounterSeatPanelProps {
  isLoading: boolean;
  data: ShowtimeSeatsResponseDto | undefined;
  statusMap: Map<number, ShowtimeSeatStatus>;
  selectedIds: Set<number>;
  onToggleSeat: (showtimeSeatId: number) => void;
}

export const CounterSeatPanel = ({
  isLoading,
  data,
  statusMap,
  selectedIds,
  onToggleSeat,
}: CounterSeatPanelProps) => {
  if (isLoading || !data) {
    return <Skeleton className="h-96 w-full " />;
  }

  return (
    <div className="flex flex-col gap-2">
      <p className="text-sm text-muted-foreground">
        {data.movieTitle}, Phòng {data.roomName}, {formatDateTime(data.startTime)}
      </p>
      <SelectableSeatMap
        rows={data.rows}
        cols={data.cols}
        aisles={data.aisles}
        seats={data.seats}
        statusMap={statusMap}
        selectedIds={selectedIds}
        onToggle={(seat) => onToggleSeat(seat.showtimeSeatId)}
      />
    </div>
  );
}

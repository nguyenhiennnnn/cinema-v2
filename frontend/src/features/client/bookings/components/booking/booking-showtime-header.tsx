import { Skeleton } from "@/components/ui/skeleton";
import type { ShowtimeSeatsResponseDto } from "@/features/client/showtimes/types";
import { formatDateTime } from "@/lib/formatters";

export const BookingShowtimeHeader = ({
  data,
  isLoading,
}: {
  data: ShowtimeSeatsResponseDto | undefined;
  isLoading: boolean;
}) => {
  if (isLoading || !data) {
    return <Skeleton className="h-10 w-2/3 rounded-lg" />;
  }

  return (
    <div>
      <h1 className="font-heading text-2xl font-bold tracking-tight text-foreground">
        {data.movieTitle}
      </h1>
      <p className="text-sm text-muted-foreground">
        Phòng {data.roomName} · {formatDateTime(data.startTime)}
      </p>
    </div>
  );
}

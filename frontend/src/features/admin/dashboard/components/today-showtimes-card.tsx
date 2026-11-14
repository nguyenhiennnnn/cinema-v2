import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { EmptyState } from "@/components/common/empty-state";
import type { DashboardShowtimeItemDto } from "@/features/admin/dashboard/types";
import { Film } from "lucide-react";
import { formatShowtimeHour } from "@/lib/formatters";

interface TodayShowtimesCardProps {
  showtimes: DashboardShowtimeItemDto[] | undefined;
  isLoading: boolean;
}


export const TodayShowtimesCard = ({ showtimes, isLoading }: TodayShowtimesCardProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Suất chiếu hôm nay</CardTitle>
        <CardDescription>Tình trạng lấp đầy theo suất chiếu</CardDescription>
      </CardHeader>
      <CardContent className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-3">
        {isLoading ? (
          Array.from({ length: 6 }).map((_, i) => <Skeleton key={i} className="h-16 w-full" />)
        ) : !showtimes || showtimes.length === 0 ? (
          <div className="col-span-full">
            <EmptyState icon={Film} title="Không có suất chiếu" description="Chưa có suất chiếu nào hôm nay." />
          </div>
        ) : (
          showtimes.map((s) => {
            const percent = s.totalSeats > 0 ? Math.round((s.bookedSeats / s.totalSeats) * 100) : 0;
            return (
              <div key={s.showtimeId} className="cut-sm bg-secondary/50 p-3">
                <div className="flex items-center justify-between gap-2">
                  <div className="min-w-0">
                    <p className="truncate text-sm font-medium text-foreground">{s.movieTitle}</p>
                    <p className="text-xs text-muted-foreground">
                      {formatShowtimeHour(s.startTime)}, Phòng {s.roomName}
                    </p>
                  </div>
                  <span className="shrink-0 text-xs font-semibold tabular-nums text-foreground">
                    {s.bookedSeats}/{s.totalSeats}
                  </span>
                </div>
                <div className="mt-2 h-1.5 w-full overflow-hidden bg-muted">
                  <div className="h-full bg-primary transition-all" style={{ width: `${percent}%` }} />
                </div>
              </div>
            );
          })
        )}
      </CardContent>
    </Card>
  );
}
import { useMemo, useState } from "react";
import { CalendarX } from "lucide-react";
import { DateStrip } from "@/components/common/date-strip";
import { EmptyState } from "@/components/common/empty-state";
import { ScheduleFilterBar } from "@/features/client/schedule/components/schedule-filter-bar";
import { ScheduleMovieRow } from "@/features/client/schedule/components/schedule-movie-row";
import { useSchedule } from "@/features/client/showtimes/hooks/use-showtimes";
import { Skeleton } from "@/components/ui/skeleton";
import { useScheduleParams } from "@/hooks/use-query-params";
import { toDateKey } from "@/lib/cinema-format";
import { Seo } from "@/components/common/seo";

const SchedulePage = () => {
  const [{ date }, setScheduleParams] = useScheduleParams();
  const selectedDate = date || toDateKey(new Date());
  const [activeFormat, setActiveFormat] = useState<string | null>(null);

  const { data, isLoading } = useSchedule(selectedDate);

  const formats = useMemo(() => {
    const set = new Set<string>();
    data?.movies.forEach((m) => m.showtimes.forEach((s) => set.add(s.screenType ?? "2D")));
    return Array.from(set);
  }, [data]);

  const movies = useMemo(() => {
    if (!data) return [];
    if (!activeFormat) return data.movies;
    return data.movies
      .map((m) => ({ ...m, showtimes: m.showtimes.filter((s) => (s.screenType ?? "2D") === activeFormat) }))
      .filter((m) => m.showtimes.length > 0);
  }, [data, activeFormat]);

  return (
    <div className="mx-auto max-w-5xl px-4 py-8 sm:px-6 lg:px-8">
      <Seo
        title="Lịch chiếu phim"
        description="Tra cứu lịch chiếu toàn bộ phim tại rạp Perfo theo từng ngày. Chọn suất chiếu yêu thích và đặt vé ngay."
        canonical="/schedule"
      />
      <div className="mb-6">
        <h1 className="font-heading text-2xl font-bold tracking-tight text-foreground md:text-3xl">Lịch chiếu phim</h1>
        <p className="mt-1 text-sm text-muted-foreground">Tra cứu lịch chiếu toàn bộ phim tại rạp theo từng ngày.</p>
      </div>

      <DateStrip value={selectedDate} onChange={(value) => setScheduleParams({ date: value })} />

      <div className="mt-4">
        <ScheduleFilterBar formats={formats} activeFormat={activeFormat} onFormatChange={setActiveFormat} />
      </div>

      <div className="mt-6 space-y-4">
        {isLoading ? (
          Array.from({ length: 4 }).map((_, i) => <Skeleton key={i} className="h-40 w-full" />)
        ) : movies.length === 0 ? (
          <EmptyState icon={CalendarX} title="Không có suất chiếu" description="Không có phim nào chiếu vào ngày đã chọn." />
        ) : (
          movies.map((movie) => <ScheduleMovieRow key={movie.movieId} movie={movie} />)
        )}
      </div>
    </div>
  );
}

export default SchedulePage;
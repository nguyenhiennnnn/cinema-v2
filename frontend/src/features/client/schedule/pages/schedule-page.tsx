import { useEffect } from "react";
import { addDays } from "date-fns";
import { CalendarX } from "lucide-react";
import { DateStrip } from "@/components/common/date-strip";
import { EmptyState } from "@/components/common/empty-state";
import { ScheduleFilterBar } from "@/features/client/schedule/components/schedule-filter-bar";
import { ScheduleMovieRow } from "@/features/client/schedule/components/schedule-movie-row";
import { useSchedule } from "@/features/client/showtimes/hooks/use-showtimes";
import { Skeleton } from "@/components/ui/skeleton";
import { useScheduleParams } from "@/hooks/use-query-params";
import { getBusinessToday, getBusinessTodayDate } from "@/lib/business-time";
import { toDateKey } from "@/lib/cinema-format";
import { useCinemaSettings } from "@/features/client/settings/hooks/use-settings";
import { Seo } from "@/components/common/seo";
import { BRAND } from "@/config/brand";

const SchedulePage = () => {
  const [{ date, format }, setScheduleParams] = useScheduleParams();
  const { data: settings } = useCinemaSettings();
  const daysCount = settings?.showtimeLookaheadDays ?? 7;
  const today = getBusinessToday();
  const maxDate = toDateKey(addDays(getBusinessTodayDate(), daysCount - 1));
  const isOutOfRange =
    !!settings && !!date && (!/^\d{4}-\d{2}-\d{2}$/.test(date) || date < today || date > maxDate);
  const selectedDate = !date || isOutOfRange ? today : date;

  useEffect(() => {
    if (isOutOfRange) setScheduleParams({ date: null, format: null });
  }, [isOutOfRange, setScheduleParams]);

  const { data, isLoading } = useSchedule(selectedDate, format);
  const movies = data?.movies ?? [];

  return (
    <div className="mx-auto max-w-5xl px-4 py-8 sm:px-6 lg:px-8">
      <Seo
        title="Lịch chiếu phim"
        description={`Tra cứu lịch chiếu toàn bộ phim tại rạp ${BRAND.name} theo từng ngày. Chọn suất chiếu yêu thích và đặt vé ngay.`}
        canonical="/schedule"
      />
      <div className="mb-6">
        <h1 className="font-heading text-2xl font-bold tracking-tight text-foreground md:text-3xl">Lịch chiếu phim</h1>
        <p className="mt-1 text-sm text-muted-foreground">Tra cứu lịch chiếu toàn bộ phim tại rạp theo từng ngày.</p>
      </div>

      <DateStrip
        value={selectedDate}
        daysCount={daysCount}
        onChange={(value) => setScheduleParams({ date: value, format: null })}
      />

      <div className="mt-4">
        <ScheduleFilterBar
          formats={data?.formats ?? []}
          activeFormat={format}
          onFormatChange={(value) => setScheduleParams({ format: value })}
        />
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
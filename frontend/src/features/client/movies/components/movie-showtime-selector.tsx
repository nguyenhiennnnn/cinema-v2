import { useMemo, useState } from "react";
import { CalendarX } from "lucide-react";
import { DateStrip } from "@/components/common/date-strip";
import { ShowtimeGroupList } from "@/components/common/showtime-group-list";
import { EmptyState } from "@/components/common/empty-state";
import { getBusinessToday } from "@/lib/business-time";
import { toDateKey } from "@/lib/cinema-format";
import type { UpcomingShowtimeDto } from "@/features/client/movies/types";

interface MovieShowtimeSelectorProps {
  showtimes: UpcomingShowtimeDto[];
}

export const MovieShowtimeSelector = ({ showtimes }: MovieShowtimeSelectorProps) => {
  const dateKeys = useMemo(() => {
    const set = new Set(showtimes.map((s) => toDateKey(s.startTime)));
    // set.delete("");
    return Array.from(set).sort();
  }, [showtimes]);


  const [manualSelectedDate, setManualSelectedDate] = useState<string | null>(null);
  const selectedDate = manualSelectedDate ?? dateKeys[0] ?? getBusinessToday();

  const filtered = useMemo(
    () => showtimes.filter((s) => toDateKey(s.startTime) === selectedDate),
    [showtimes, selectedDate],
  );

  return (
    <div id="showtimes" className="scroll-mt-24">
      <h2 className="font-heading text-2xl font-semibold tracking-tight text-foreground md:text-3xl">Lịch chiếu & Đặt vé</h2>

      <DateStrip value={selectedDate} onChange={setManualSelectedDate} className="mt-4" />

      <div className="mt-5">
        {filtered.length === 0 ? (
          <EmptyState icon={CalendarX} title="Chưa có suất chiếu" description="Vui lòng chọn ngày khác để xem lịch chiếu." />
        ) : (
          <ShowtimeGroupList
            showtimes={filtered}
          />
        )}
      </div>
    </div>
  );
}
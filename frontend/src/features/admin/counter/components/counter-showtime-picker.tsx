import { Film } from "lucide-react";
import { ShowtimeStatus } from "@/types/enum";
import { Label } from "@/components/ui/label";
import { Skeleton } from "@/components/ui/skeleton";
import { formatCurrency } from "@/lib/formatters";
import { useSchedule } from "@/features/client/showtimes/hooks/use-showtimes";
import { formatShowtimeRange } from "@/lib/cinema-format";
import { DatePickerField } from "@/components/common/date-time/date-picker-field";

interface CounterShowtimePickerProps {
  date: string;
  onDateChange: (date: string) => void;
  onPickShowtime: (showtimeId: number) => void;
}

export const CounterShowtimePicker = ({ date, onDateChange, onPickShowtime }: CounterShowtimePickerProps) => {
  const schedule = useSchedule(date);

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col gap-1.5 sm:w-56">
        <Label htmlFor="counter-date">Ngày chiếu</Label>
        <DatePickerField
          id="counter-date"
          value={date}
          onChange={(value) => onDateChange(value ?? date)}
          placeholder="Chọn ngày chiếu"
          clearable={false}
        />
      </div>

      {schedule.isLoading && (
        <Skeleton className="h-64 w-full " />
      )}

      {!schedule.isLoading && (schedule.data?.movies.length ?? 0) === 0 && (
        <div className="flex flex-col items-center gap-2 border border-dashed border-border py-12 text-muted-foreground">
          <Film className="size-6" />
          <p className="text-sm">Không có suất chiếu nào trong ngày này.</p>
        </div>
      )}

      <div className="flex flex-col gap-4">
        {schedule.data?.movies.map((movie) => (
          <div key={movie.movieId} className="border border-border bg-card p-4">
            <div className="mb-3 flex items-center gap-3">
              {movie.moviePosterUrl && (
                <img
                  src={movie.moviePosterUrl}
                  alt={movie.movieTitle}
                  className="h-16 w-11 object-cover"
                />
              )}
              <h3 className="font-heading text-base font-medium text-foreground">
                {movie.movieTitle}
              </h3>
            </div>
            <div className="flex flex-wrap gap-2">
              {movie.showtimes.map((st) => (
                <button
                  key={st.showtimeId}
                  type="button"
                  disabled={st.availableSeats <= 0 || st.status !== ShowtimeStatus.Scheduled}
                  onClick={() => onPickShowtime(st.showtimeId)}
                  className="flex flex-col items-start cut-sm bg-secondary px-3 py-2 text-left text-xs font-medium transition-all hover:bg-muted disabled:cursor-not-allowed disabled:opacity-40"
                >
                  <span>
                    {formatShowtimeRange(st.startTime, st.endTime)}
                  </span>
                  <span className="text-muted-foreground">
                    P.{st.roomName}, {st.availableSeats}/{st.totalSeats} ghế trống
                  </span>
                  <span className="font-semibold text-primary">{formatCurrency(st.basePrice)}</span>
                </button>
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

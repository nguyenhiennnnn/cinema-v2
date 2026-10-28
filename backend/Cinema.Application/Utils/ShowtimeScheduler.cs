namespace Cinema.Application.Utils;

public record SchedulerRoom(int RoomId, int Seats);

public record SchedulerMovie(int MovieId, int DurationMinutes, double Heat, int Shows);

public record SchedulerBusy(int RoomId, DateTime Start, DateTime End);

public record SchedulerSlot(int MovieId, int RoomId, DateTime Start, DateTime End);

public static class ShowtimeScheduler
{
    private const int StepMinutes = 5;
    private const int MinSameMovieGapMinutes = 60;
    private const int PrimeStartMinute = 18 * 60;
    private const int PrimeEndMinute = 21 * 60;
    private const double PrimeFalloffMinutes = 360;

    public static double Heat(int tickets, int maxTickets, decimal rating) =>
        Math.Clamp(
            (maxTickets > 0 ? 0.7 * tickets / maxTickets : 0) + 0.3 * (double)rating / 10,
            0,
            1
        );

    public static double Prime(DateTime start, DateOnly day)
    {
        var minute = (start - day.ToDateTime(TimeOnly.MinValue)).TotalMinutes;
        var outside =
            minute < PrimeStartMinute
                ? PrimeStartMinute - minute
                : Math.Max(0, minute - PrimeEndMinute);
        return Math.Max(0, 1 - outside / PrimeFalloffMinutes);
    }

    public static double PriceFactor(DateTime start, DateOnly day, double heat)
    {
        var time = 0.85 + 0.30 * Prime(start, day);
        var weekend = day.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday or DayOfWeek.Sunday
            ? 1.10
            : 1.0;
        var demand = 0.95 + 0.15 * Math.Clamp(heat, 0, 1);
        return Math.Clamp(time * weekend * demand, 0.7, 1.4);
    }

    public static decimal Price(decimal basePrice, double factor, decimal step = 5000m)
    {
        if (basePrice < step)
            return basePrice;

        return Math.Max(
            step,
            Math.Round(basePrice * (decimal)factor / step, MidpointRounding.AwayFromZero) * step
        );
    }

    public static List<SchedulerSlot> Build(
        DateOnly day,
        TimeOnly? opening,
        TimeOnly? closing,
        bool allowAfterHours,
        int bufferMinutes,
        IReadOnlyList<SchedulerRoom> rooms,
        IReadOnlyList<SchedulerMovie> movies,
        IReadOnlyList<SchedulerBusy> busy
    )
    {
        var placed = new List<SchedulerSlot>();
        if (rooms.Count == 0 || movies.Count == 0)
            return placed;

        var (open, close) = OperatingHours.Window(day, opening, closing);
        var dayStart = day.ToDateTime(TimeOnly.MinValue);
        var maxSeats = Math.Max(1, rooms.Max(r => r.Seats));
        var occupied = rooms.ToDictionary(
            r => r.RoomId,
            r => busy.Where(b => b.RoomId == r.RoomId).Select(b => (b.Start, b.End)).ToList()
        );
        var queue = movies.OrderByDescending(m => m.Heat).ThenBy(m => m.MovieId).ToList();
        var remaining = queue.ToDictionary(m => m.MovieId, m => m.Shows);

        while (remaining.Values.Any(v => v > 0))
        {
            foreach (var movie in queue.Where(m => remaining[m.MovieId] > 0))
            {
                var block = movie.DurationMinutes + bufferMinutes;
                var options = rooms
                    .SelectMany(room =>
                        Candidates(open, close, movie.DurationMinutes, block, dayStart, occupied[room.RoomId])
                            .Where(c =>
                                IsFree(c, block, movie.DurationMinutes, open, close, allowAfterHours, occupied[room.RoomId])
                                && placed.All(p =>
                                    p.MovieId != movie.MovieId
                                    || Math.Abs((c - p.Start).TotalMinutes) >= MinSameMovieGapMinutes
                                )
                            )
                            .Select(c => (
                                Room: room,
                                Start: c,
                                Score: Prime(c, day) * 100
                                    + movie.Heat * room.Seats / maxSeats * 10
                                    - occupied[room.RoomId].Count * 0.5
                            ))
                    )
                    .ToList();

                if (options.Count == 0)
                {
                    remaining[movie.MovieId] = 0;
                    continue;
                }

                var best = options
                    .OrderByDescending(x => x.Score)
                    .ThenBy(x => x.Start)
                    .ThenBy(x => x.Room.RoomId)
                    .First();
                var end = best.Start.AddMinutes(block);
                occupied[best.Room.RoomId].Add((best.Start, end));
                placed.Add(new SchedulerSlot(movie.MovieId, best.Room.RoomId, best.Start, end));
                remaining[movie.MovieId]--;
            }
        }

        return placed.OrderBy(p => p.Start).ThenBy(p => p.RoomId).ToList();
    }

    private static IEnumerable<DateTime> Candidates(
        DateTime open,
        DateTime? close,
        int duration,
        int block,
        DateTime dayStart,
        List<(DateTime Start, DateTime End)> intervals
    )
    {
        var points = new List<DateTime>
        {
            Up(open),
            Up(dayStart.AddMinutes(PrimeStartMinute)),
            Up(dayStart.AddMinutes(PrimeEndMinute)),
        };
        if (close is not null)
            points.Add(Down(close.Value.AddMinutes(-duration)));
        foreach (var (start, end) in intervals)
        {
            points.Add(Up(end));
            points.Add(Down(start.AddMinutes(-block)));
        }

        return points.Distinct();
    }

    private static bool IsFree(
        DateTime start,
        int block,
        int duration,
        DateTime open,
        DateTime? close,
        bool allowAfterHours,
        List<(DateTime Start, DateTime End)> intervals
    )
    {
        if (start < open)
            return false;
        if (close is not null && (allowAfterHours ? start >= close : start.AddMinutes(duration) > close))
            return false;

        var end = start.AddMinutes(block);
        return !intervals.Any(i => start < i.End && end > i.Start);
    }

    private static DateTime Up(DateTime value) =>
        value.Date.AddMinutes(Math.Ceiling((value - value.Date).TotalMinutes / StepMinutes) * StepMinutes);

    private static DateTime Down(DateTime value) =>
        value.Date.AddMinutes(Math.Floor((value - value.Date).TotalMinutes / StepMinutes) * StepMinutes);
}

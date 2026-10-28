namespace Cinema.Application.Utils;

public record SchedulerRoom(int RoomId, int Seats);

public record SchedulerMovie(int MovieId, int DurationMinutes, double Heat, int Shows);

public record SchedulerBusy(int RoomId, DateTime Start, DateTime End);

public record SchedulerSlot(int MovieId, int RoomId, DateTime Start, DateTime End);

public static class ShowtimeScheduler
{
    private const int StepMinutes = 5;
    private const int MinSameMovieGapMinutes = 60;
    private const int DefaultWindowMinutes = 16 * 60;
    private const double SpreadMinutes = 30;
    private const double FitWeight = 4;
    private const double LoadWeight = 0.5;

    public static double Heat(int tickets, int maxTickets, decimal rating) =>
        Math.Clamp(
            (maxTickets > 0 ? 0.7 * tickets / maxTickets : 0) + 0.3 * (double)rating / 10,
            0,
            1
        );

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
        var windowEnd = close ?? open.AddMinutes(DefaultWindowMinutes);
        var maxSeats = Math.Max(1, rooms.Max(r => r.Seats));
        var occupied = rooms.ToDictionary(
            r => r.RoomId,
            r => busy.Where(b => b.RoomId == r.RoomId).Select(b => (b.Start, b.End)).ToList()
        );
        var queue = movies.OrderByDescending(m => m.Heat).ThenBy(m => m.MovieId).ToList();
        var remaining = queue.ToDictionary(m => m.MovieId, m => m.Shows);
        var ideals = queue.ToDictionary(
            m => m.MovieId,
            m => Ideals(open, windowEnd, m.Shows, m.DurationMinutes, allowAfterHours)
        );

        while (remaining.Values.Any(v => v > 0))
        {
            foreach (var movie in queue.Where(m => remaining[m.MovieId] > 0))
            {
                var block = movie.DurationMinutes + bufferMinutes;
                var targets = ideals[movie.MovieId];
                var options = rooms
                    .SelectMany(room =>
                        Candidates(open, close, movie.DurationMinutes, block, targets, occupied[room.RoomId])
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
                                Score: Score(c, targets, movie.Heat, room.Seats, maxSeats, occupied[room.RoomId].Count)
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

                if (targets.Count > 0)
                    targets.Remove(targets.MinBy(t => Math.Abs((best.Start - t).TotalMinutes)));

                var end = best.Start.AddMinutes(block);
                occupied[best.Room.RoomId].Add((best.Start, end));
                placed.Add(new SchedulerSlot(movie.MovieId, best.Room.RoomId, best.Start, end));
                remaining[movie.MovieId]--;
            }
        }

        return placed.OrderBy(p => p.Start).ThenBy(p => p.RoomId).ToList();
    }

    private static double Score(
        DateTime start,
        List<DateTime> targets,
        double heat,
        int seats,
        int maxSeats,
        int load
    )
    {
        var distance = targets.Count == 0 ? 0 : targets.Min(t => Math.Abs((start - t).TotalMinutes));
        var fit = 1 - Math.Abs(heat - (double)seats / maxSeats);
        return -distance / SpreadMinutes + fit * FitWeight - load * LoadWeight;
    }

    private static List<DateTime> Ideals(
        DateTime open,
        DateTime windowEnd,
        int shows,
        int duration,
        bool allowAfterHours
    )
    {
        var latest = allowAfterHours ? windowEnd.AddMinutes(-1) : windowEnd.AddMinutes(-duration);
        var span = (latest - open).TotalMinutes;
        if (span < 0 || shows <= 0)
            return [];

        return Enumerable
            .Range(0, shows)
            .Select(i => Up(open.AddMinutes(span * (i + 0.5) / shows)))
            .ToList();
    }

    private static IEnumerable<DateTime> Candidates(
        DateTime open,
        DateTime? close,
        int duration,
        int block,
        List<DateTime> targets,
        List<(DateTime Start, DateTime End)> intervals
    )
    {
        var points = new List<DateTime> { Up(open) };
        points.AddRange(targets);
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

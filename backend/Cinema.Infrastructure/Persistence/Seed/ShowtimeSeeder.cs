using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema.Infrastructure.Persistence.Seed;


internal static class ShowtimeSeeder
{
    internal const int PastDays = 7;
    internal const int FutureDays = 7;

    internal const string SpecialOngoing = "ongoing";
    internal const string SpecialStartingSoon = "starting-soon";
    internal const string SpecialRefundClosed = "refund-closed";
    internal const string SpecialSoldOut = "sold-out";
    internal const string SpecialCancelled = "cancelled";

    private const int MaxShowsPerRoomDay = 4;
    private const int MinShowsPerRoomDay = 3;
    private const int BufferMinutes = 15; // = CinemaSettings.DefaultRoomBufferMinutes
    private const double SafetyBufferHours = 0.5;

    /// <summary>0h (giờ VN) của ngày VN chứa <paramref name="now"/>, cộng <paramref name="offsetDays"/> ngày, tính bằng UTC.</summary>
    internal static DateTime LocalDayStartUtc(DateTime now, int offsetDays = 0) =>
        VietnamTime.StartOfDayUtc(VietnamTime.LocalDate(now).AddDays(offsetDays));

    private readonly record struct Busy(int RoomId, DateTime Start, DateTime End);

    private sealed class Context
    {
        public List<Movie> NowShowing { get; init; } = [];
        public List<Movie> PastPool { get; init; } = []; // NowShowing + Ended
        public List<CinemaRoom> Rooms { get; init; } = [];
        public Dictionary<int, List<Seat>> SeatsByRoom { get; init; } = [];
        public TimeOnly Opening { get; init; }
        public TimeOnly Closing { get; init; }
        public double MinLeadHours { get; init; }
        public List<Busy> Busy { get; init; } = [];
    }

    // ───────────────────────────── Tương lai ─────────────────────────────

    /// <summary>Đảm bảo mỗi phòng active có đủ suất chiếu cho hôm nay + 7 ngày tới. Idempotent.</summary>
    public static async Task<int> TopUpFutureAsync(
        AppDbContext db,
        ILogger logger,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        var ctx = await LoadContextAsync(db, now, cancellationToken);
        if (
            ctx.NowShowing.Count == 0
            || ctx.Rooms.Count == 0
        )
        {
            logger.LogWarning(
                "Skip seeding future showtimes: không có phim NowShowing hoặc phòng active có ghế."
            );
            return 0;
        }

        var cutoff = now.AddHours(ctx.MinLeadHours);
        var created = new List<Showtime>();

        for (var offset = 0; offset <= FutureDays; offset++)
        {
            var day = LocalDayStartUtc(now, offset);
            var rotation = offset * 3;
            created.AddRange(
                PlanDay(ctx, day, offset == 0 ? cutoff : null, ctx.NowShowing, ref rotation, now)
            );
        }

        if (created.Count == 0)
            return 0;

        db.Showtimes.AddRange(created);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded {Count} future showtimes.", created.Count);
        return created.Count;
    }

    // ───────────────────────────── Quá khứ ─────────────────────────────

    /// <summary>Suất chiếu của 7 ngày gần nhất (Completed) để có dữ liệu báo cáo / dashboard / review.</summary>
    public static async Task<int> SeedPastAsync(
        AppDbContext db,
        ILogger logger,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        var ctx = await LoadContextAsync(db, now, cancellationToken);
        if (ctx.PastPool.Count == 0 || ctx.Rooms.Count == 0)
            return 0;

        var created = new List<Showtime>();
        for (var offset = -PastDays; offset <= -1; offset++)
        {
            var day = LocalDayStartUtc(now, offset);
            var rotation = (offset + PastDays) * 3;
            created.AddRange(PlanDay(ctx, day, null, ctx.PastPool, ref rotation, now));
        }

        if (created.Count == 0)
            return 0;

        db.Showtimes.AddRange(created);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded {Count} past (completed) showtimes.", created.Count);
        return created.Count;
    }

    // ───────────────────────────── Suất đặc biệt ─────────────────────────────

    /// <summary>
    /// Tạo các suất phục vụ kịch bản cụ thể (xem <see cref="BookingScenarioSeeder"/>).
    /// Trả về dictionary theo key <c>Special*</c>.
    /// </summary>
    public static async Task<Dictionary<string, Showtime>> CreateSpecialsAsync(
        AppDbContext db,
        ILogger logger,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        var result = new Dictionary<string, Showtime>();
        var ctx = await LoadContextAsync(db, now, cancellationToken);
        if (ctx.NowShowing.Count == 0 || ctx.Rooms.Count == 0)
            return result;

        var nowMinute = new DateTime(now.Ticks - now.Ticks % TimeSpan.TicksPerMinute, now.Kind);
        var today = LocalDayStartUtc(now);

        // (key, movieId ưu tiên, giờ bắt đầu, phòng ưu tiên, status)
        var specs = new (string Key, int MovieId, DateTime Start, int PreferredRoomId, ShowtimeStatus Status)[]
        {
            // Đang chiếu: cửa sổ check-in đang mở, có vé đã/chưa check-in.
            (SpecialOngoing, 8, nowMinute.AddMinutes(-20), 1, ShowtimeStatus.Scheduled),
            // Sắp chiếu (20 phút nữa): check-in mở, đã quá hạn hoàn tiền.
            (SpecialStartingSoon, 7, nowMinute.AddMinutes(20), 2, ShowtimeStatus.Scheduled),
            // 90 phút nữa: < CancelDeadlineHours (2h) nên không được hoàn tiền, nhưng chưa mở check-in.
            (SpecialRefundClosed, 6, nowMinute.AddMinutes(90), 4, ShowtimeStatus.Scheduled),
            // Hết vé 100%: ngày mai 19:00, phòng nhỏ nhất.
            (SpecialSoldOut, 3, today.AddDays(1).AddHours(19), 3, ShowtimeStatus.Scheduled),
            // Suất bị hủy bởi rạp: có booking bị hủy + hoàn tiền (tự động & thủ công).
            (SpecialCancelled, 14, today.AddDays(2).AddHours(15), 4, ShowtimeStatus.Cancelled),
        };

        var created = new List<Showtime>();
        foreach (var spec in specs)
        {
            var movie =
                ctx.NowShowing.FirstOrDefault(m => m.MovieId == spec.MovieId) ?? ctx.NowShowing[0];
            var end = spec.Start.AddMinutes(movie.DurationMinutes + BufferMinutes);

            var room = PickFreeRoom(ctx, spec.PreferredRoomId, spec.Start, end);
            if (room is null)
            {
                logger.LogWarning("Không tìm được phòng trống cho suất đặc biệt {Key}.", spec.Key);
                continue;
            }

            var showtime = BuildShowtime(
                ctx,
                movie,
                room,
                spec.Start,
                end,
                spec.Status,
                createdAt: spec.Start < now ? spec.Start.AddDays(-3) : now
            );
            created.Add(showtime);
            result[spec.Key] = showtime;

            if (spec.Status != ShowtimeStatus.Cancelled)
                ctx.Busy.Add(new Busy(room.RoomId, spec.Start, end));
        }

        if (created.Count > 0)
        {
            db.Showtimes.AddRange(created);
            await db.SaveChangesAsync(cancellationToken);
        }

        logger.LogInformation("Seeded {Count} special showtimes.", created.Count);
        return result;
    }

    private static CinemaRoom? PickFreeRoom(
        Context ctx,
        int preferredRoomId,
        DateTime start,
        DateTime end
    )
    {
        var ordered = ctx
            .Rooms.OrderBy(r => r.RoomId == preferredRoomId ? 0 : 1)
            .ThenBy(r => r.RoomId)
            .ToList();

        foreach (var room in ordered)
        {
            var clash = ctx.Busy.Any(b => b.RoomId == room.RoomId && b.Start < end && b.End > start);
            if (!clash)
                return room;
        }

        return null;
    }

    // ───────────────────────────── Nội bộ ─────────────────────────────

    private static async Task<Context> LoadContextAsync(
        AppDbContext db,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        var settings = await db.CinemaSettings.FirstOrDefaultAsync(cancellationToken);

        var movies = await db
            .Movies.Where(m => m.Status != MovieStatus.ComingSoon)
            .OrderBy(m => m.MovieId)
            .ToListAsync(cancellationToken);

        var rooms = await db
            .CinemaRooms.Include(r => r.ScreenType)
            .Where(r => r.IsActive)
            .OrderBy(r => r.RoomId)
            .ToListAsync(cancellationToken);

        var seatsByRoom = (
            await db
                .Seats.Include(s => s.SeatType)
                .Where(s => s.IsActive && !s.IsAisle)
                .ToListAsync(cancellationToken)
        )
            .GroupBy(s => s.RoomId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var from = LocalDayStartUtc(now, -PastDays - 1);
        var to = LocalDayStartUtc(now, FutureDays + 2);
        var busyRows = await db
            .Showtimes.Where(s =>
                s.Status != ShowtimeStatus.Cancelled && s.EndTime > from && s.StartTime < to
            )
            .Select(s => new
            {
                s.RoomId,
                s.StartTime,
                s.EndTime,
            })
            .ToListAsync(cancellationToken);

        return new Context
        {
            NowShowing = movies.Where(m => m.Status == MovieStatus.NowShowing).ToList(),
            PastPool = movies,
            Rooms = rooms.Where(r => seatsByRoom.ContainsKey(r.RoomId)).ToList(),
            SeatsByRoom = seatsByRoom,
            Opening = settings?.OpeningTime ?? new TimeOnly(8, 0),
            Closing = settings?.ClosingTime ?? new TimeOnly(23, 59),
            MinLeadHours = (settings?.CancelDeadlineHours ?? 2) + SafetyBufferHours,
            Busy = busyRows.Select(b => new Busy(b.RoomId, b.StartTime, b.EndTime)).ToList(),
        };
    }

    /// <summary>
    /// Xếp lịch cho một ngày: mỗi phòng tối đa <see cref="MaxShowsPerRoomDay"/> suất, tuần tự, làm tròn 15 phút,
    /// không vượt giờ đóng cửa, không chồng suất đã có. Bỏ qua phòng đã đủ <see cref="MinShowsPerRoomDay"/> suất
    /// trong ngày (idempotent giữa các lần khởi động).
    /// </summary>
    private static List<Showtime> PlanDay(
        Context ctx,
        DateTime day,
        DateTime? notBefore,
        List<Movie> pool,
        ref int rotation,
        DateTime now
    )
    {
        var result = new List<Showtime>();
        if (pool.Count == 0)
            return result;

        var close = day.Add(ctx.Closing.ToTimeSpan());
        if (ctx.Closing <= ctx.Opening)
            close = close.AddDays(1);

        for (var roomIndex = 0; roomIndex < ctx.Rooms.Count; roomIndex++)
        {
            var room = ctx.Rooms[roomIndex];
            var roomId = room.RoomId;
            var existing = ctx.Busy.Count(b => b.RoomId == roomId && b.Start >= day && b.Start < day.AddDays(1));
            if (existing >= MinShowsPerRoomDay)
                continue;

            var toAdd = MaxShowsPerRoomDay - existing;

            // Suất đầu ~ 1 giờ sau giờ mở cửa, lệch 15 phút giữa các phòng cho tự nhiên.
            var cursor = day.Add(ctx.Opening.ToTimeSpan()).AddMinutes(60 + roomIndex * 15);
            if (notBefore is { } floor && cursor < floor)
                cursor = floor;

            var added = 0;
            var guard = 0;
            while (added < toAdd && guard++ < 60)
            {
                var movie = pool[rotation % pool.Count];
                var start = RoundUp(cursor);
                var runEnd = start.AddMinutes(movie.DurationMinutes);
                if (runEnd > close)
                    break;

                var end = runEnd.AddMinutes(BufferMinutes);
                var clash = ctx.Busy.FirstOrDefault(b =>
                    b.RoomId == roomId && b.Start < end && b.End > start
                );
                if (clash.RoomId != 0)
                {
                    cursor = clash.End;
                    continue;
                }

                var status = end < now ? ShowtimeStatus.Completed : ShowtimeStatus.Scheduled;
                var createdAt = start < now ? start.AddDays(-3) : now;

                result.Add(BuildShowtime(ctx, movie, room, start, end, status, createdAt));
                ctx.Busy.Add(new Busy(roomId, start, end));

                rotation++;
                cursor = end;
                added++;
            }
        }

        return result;
    }

    private static DateTime RoundUp(DateTime value)
    {
        var step = TimeSpan.FromMinutes(15).Ticks;
        var ticks = (value.Ticks + step - 1) / step * step;
        return new DateTime(ticks, value.Kind);
    }

    private static decimal GetBasePrice(string? screenTypeName) =>
        screenTypeName switch
        {
            "IMAX" => 130_000m,
            "3D" => 95_000m,
            _ => 75_000m,
        };

    private static Showtime BuildShowtime(
        Context ctx,
        Movie movie,
        CinemaRoom room,
        DateTime start,
        DateTime end,
        ShowtimeStatus status,
        DateTime createdAt
    )
    {
        var basePrice = GetBasePrice(room.ScreenType?.Name);
        var showtime = new Showtime
        {
            MovieId = movie.MovieId,
            RoomId = room.RoomId,
            StartTime = start,
            EndTime = end,
            BasePrice = basePrice,
            Status = status,
            CreatedAt = createdAt,
        };

        foreach (var seat in ctx.SeatsByRoom[room.RoomId])
        {
            showtime.ShowtimeSeats.Add(
                new ShowtimeSeat
                {
                    SeatId = seat.SeatId,
                    Status = ShowtimeSeatStatus.Available,
                    Price = Math.Round(basePrice * (seat.SeatType?.PriceMultiplier ?? 1.0m), 0),
                }
            );
        }

        return showtime;
    }
}

using System.Globalization;
using System.Text.Json.Nodes;
using Cinema.Application.DTOs.Ai;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema.Application.Services.Ai;

public class AiToolExecutor(IAppDbContext db, ILogger<AiToolExecutor> logger)
{
    public const string SearchShowtimes = "search_showtimes";
    public const string GetSeatAvailability = "get_seat_availability";
    public const string GetMyBookings = "get_my_bookings";

    private const int MaxShowtimeResults = 8;
    private const int MaxBookingResults = 5;
    private const int MaxSeatLabels = 40;

    public record ToolResult(JsonObject Response, List<AiCardDto> Cards);

    public static IReadOnlyList<AiToolDefinition> GetToolDefinitions(bool isAuthenticated)
    {
        var tools = new List<AiToolDefinition>
        {
            new(
                SearchShowtimes,
                Messages.Ai_Tool_SearchShowtimes_Description,
                [
                    new AiToolParameter(
                        "movie_id",
                        AiToolParameterType.Integer,
                        Messages.Ai_Param_MovieId
                    ),
                    new AiToolParameter(
                        "movie_title",
                        AiToolParameterType.String,
                        Messages.Ai_Param_MovieTitle
                    ),
                    new AiToolParameter(
                        "date",
                        AiToolParameterType.String,
                        Messages.Ai_Param_Date
                    ),
                ]
            ),
            new(
                GetSeatAvailability,
                Messages.Ai_Tool_GetSeatAvailability_Description,
                [
                    new AiToolParameter(
                        "showtime_id",
                        AiToolParameterType.Integer,
                        Messages.Ai_Param_ShowtimeId,
                        Required: true
                    ),
                ]
            ),
        };

        if (isAuthenticated)
        {
            tools.Add(
                new AiToolDefinition(
                    GetMyBookings,
                    Messages.Ai_Tool_GetMyBookings_Description,
                    [
                        new AiToolParameter(
                            "upcoming_only",
                            AiToolParameterType.Boolean,
                            Messages.Ai_Param_UpcomingOnly
                        ),
                        new AiToolParameter(
                            "status",
                            AiToolParameterType.String,
                            Messages.Ai_Param_BookingStatus,
                            AllowedValues: Enum.GetValues<BookingStatus>().Select(s => s.ToDtoStatus()).ToList()
                        ),
                    ]
                )
            );
        }

        return tools;
    }

    public async Task<ToolResult> ExecuteAsync(
        string name,
        JsonObject? args,
        int? userId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return name switch
            {
                SearchShowtimes => await SearchShowtimesAsync(args, cancellationToken),
                GetSeatAvailability => await GetSeatAvailabilityAsync(args, cancellationToken),
                GetMyBookings => await GetMyBookingsAsync(args, userId, cancellationToken),
                _ => Error(string.Format(Messages.Ai_Error_UnknownFunction, name)),
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AI tool {Tool} failed.", name);
            return Error(Messages.Ai_Error_QueryFailed);
        }
    }

    private async Task<ToolResult> SearchShowtimesAsync(JsonObject? args, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var movieId = GetInt(args, "movie_id");
        var title = GetString(args, "movie_title")?.Trim().ToLowerInvariant();

        var query = db
            .Showtimes.AsNoTracking()
            .Where(s => s.Status == ShowtimeStatus.Scheduled && s.StartTime >= now);

        if (GetString(args, "date") is { } dateStr)
        {
            if (
                !DateOnly.TryParseExact(
                    dateStr,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date
                )
            )
                return Error(Messages.Ai_Error_DateFormat);

            var dayStart = VietnamTime.StartOfDayUtc(date);
            var dayEnd = VietnamTime.StartOfNextDayUtc(date);
            query = query.Where(s => s.StartTime >= dayStart && s.StartTime < dayEnd);
        }
        else
        {
            var end = now.AddDays(await db.LoadShowtimeLookaheadDaysAsync(ct));
            query = query.Where(s => s.StartTime <= end);
        }

        if (movieId.HasValue)
            query = query.Where(s => s.MovieId == movieId.Value);
        else if (!string.IsNullOrEmpty(title))
            query = query.Where(s =>
                s.Movie.Title.ToLower().Contains(title)
                || (
                    s.Movie.OriginalTitle != null && s.Movie.OriginalTitle.ToLower().Contains(title)
                )
            );

        var rows = await query
            .OrderBy(s => s.StartTime)
            .Take(MaxShowtimeResults)
            .Select(s => new
            {
                s.ShowtimeId,
                s.MovieId,
                MovieTitle = s.Movie.Title,
                s.Movie.PosterUrl,
                RoomName = s.Room.RoomName,
                ScreenType = s.Room.ScreenType != null ? s.Room.ScreenType.Name : null,
                s.StartTime,
                s.BasePrice,
                AvailableSeats = s.ShowtimeSeats.Count(ss =>
                    ss.Status == ShowtimeSeatStatus.Available
                    || (ss.Status == ShowtimeSeatStatus.Held && ss.HeldUntil < now)
                ),
            })
            .ToListAsync(ct);

        var items = new JsonArray();
        var cards = new List<AiCardDto>();
        foreach (var r in rows)
        {
            items.Add(
                new JsonObject
                {
                    ["showtimeId"] = r.ShowtimeId,
                    ["movieId"] = r.MovieId,
                    ["movieTitle"] = r.MovieTitle,
                    ["room"] = r.RoomName,
                    ["screenType"] = r.ScreenType,
                    ["startTime"] = VietnamTime.ToLocal(r.StartTime).ToString("dd/MM/yyyy HH:mm"),
                    ["basePriceVnd"] = r.BasePrice,
                    ["availableSeats"] = r.AvailableSeats,
                }
            );
            cards.Add(
                new AiCardDto(
                    "showtime",
                    r.MovieTitle,
                    string.Format(
                        Messages.Ai_Card_ShowtimeSubtitle,
                        VietnamTime.ToLocal(r.StartTime),
                        r.RoomName
                    ),
                    string.Format(Messages.Ai_Card_PriceAndSeats, r.BasePrice, r.AvailableSeats),
                    r.PosterUrl,
                    $"/booking/{r.ShowtimeId}"
                )
            );
        }

        return new ToolResult(
            new JsonObject { ["count"] = items.Count, ["showtimes"] = items },
            cards
        );
    }

    private async Task<ToolResult> GetSeatAvailabilityAsync(JsonObject? args, CancellationToken ct)
    {
        var showtimeId = GetInt(args, "showtime_id");
        if (showtimeId is null)
            return Error(Messages.Ai_Error_MissingShowtimeId);

        var now = DateTime.UtcNow;
        var showtime = await db
            .Showtimes.AsNoTracking()
            .Where(s => s.ShowtimeId == showtimeId.Value)
            .Select(s => new
            {
                s.ShowtimeId,
                s.Status,
                s.StartTime,
                s.BasePrice,
                MovieTitle = s.Movie.Title,
                s.Movie.PosterUrl,
                RoomName = s.Room.RoomName,
            })
            .FirstOrDefaultAsync(ct);

        if (showtime is null)
            return Error(Messages.NotFound_Showtime);
        if (showtime.Status != ShowtimeStatus.Scheduled || showtime.StartTime < now)
            return Error(Messages.Ai_Error_ShowtimeEnded);

        var defaultSeatTypeName = Messages.Ai_SeatTypeDefault;
        var seats = await db
            .ShowtimeSeats.AsNoTracking()
            .Where(ss => ss.ShowtimeId == showtimeId.Value && ss.Seat.IsActive && !ss.Seat.IsAisle)
            .Select(ss => new
            {
                ss.Seat.SeatRow,
                ss.Seat.SeatNumber,
                TypeName = ss.Seat.SeatType != null ? ss.Seat.SeatType.Name : defaultSeatTypeName,
                ss.Price,
                IsFree = ss.Status == ShowtimeSeatStatus.Available
                    || (ss.Status == ShowtimeSeatStatus.Held && ss.HeldUntil < now),
            })
            .ToListAsync(ct);

        var free = seats.Where(x => x.IsFree).ToList();

        var byType = new JsonArray();
        foreach (var g in seats.GroupBy(x => x.TypeName))
        {
            byType.Add(
                new JsonObject
                {
                    ["seatType"] = g.Key,
                    ["priceVnd"] = g.Max(x => x.Price),
                    ["total"] = g.Count(),
                    ["available"] = g.Count(x => x.IsFree),
                }
            );
        }

        var byRow = new JsonArray();
        foreach (var g in free.GroupBy(x => x.SeatRow).OrderBy(g => g.Key))
            byRow.Add(new JsonObject { ["row"] = g.Key, ["available"] = g.Count() });

        var labels = new JsonArray();
        foreach (
            var s in free.OrderBy(x => x.SeatRow, StringComparer.Ordinal)
                .ThenBy(x => x.SeatNumber)
                .Take(MaxSeatLabels)
        )
            labels.Add($"{s.SeatRow}{s.SeatNumber}");

        var response = new JsonObject
        {
            ["showtimeId"] = showtime.ShowtimeId,
            ["movieTitle"] = showtime.MovieTitle,
            ["room"] = showtime.RoomName,
            ["startTime"] = VietnamTime.ToLocal(showtime.StartTime).ToString("dd/MM/yyyy HH:mm"),
            ["totalSeats"] = seats.Count,
            ["availableSeats"] = free.Count,
            ["bySeatType"] = byType,
            ["availableByRow"] = byRow,
            ["availableSeatLabels"] = labels,
            ["labelsTruncated"] = free.Count > MaxSeatLabels,
        };

        var card = new AiCardDto(
            "showtime",
            showtime.MovieTitle,
            string.Format(
                Messages.Ai_Card_ShowtimeSubtitle,
                VietnamTime.ToLocal(showtime.StartTime),
                showtime.RoomName
            ),
            string.Format(Messages.Ai_Card_SeatsLeft, free.Count, seats.Count),
            showtime.PosterUrl,
            $"/booking/{showtime.ShowtimeId}"
        );

        return new ToolResult(response, [card]);
    }

    private async Task<ToolResult> GetMyBookingsAsync(
        JsonObject? args,
        int? userId,
        CancellationToken ct
    )
    {
        if (userId is null)
            return Error(Messages.Ai_Error_LoginRequired);

        var now = DateTime.UtcNow;
        var query = db.Bookings.AsNoTracking().Where(b => b.UserId == userId.Value);

        if (
            GetString(args, "status") is { } statusStr
            && Enum.TryParse<BookingStatus>(statusStr, true, out var status)
        )
            query = query.Where(b => b.Status == status);

        if (GetBool(args, "upcoming_only") == true)
            query = query.Where(b => b.Showtime.StartTime >= now);

        var rows = await query
            .OrderByDescending(b => b.BookingId)
            .Take(MaxBookingResults)
            .Select(b => new
            {
                b.BookingId,
                b.BookingCode,
                b.Status,
                b.FinalAmount,
                MovieTitle = b.Showtime.Movie.Title,
                b.Showtime.Movie.PosterUrl,
                RoomName = b.Showtime.Room.RoomName,
                b.Showtime.StartTime,
                Seats = b
                    .Tickets.Select(t => new
                    {
                        t.ShowtimeSeat.Seat.SeatRow,
                        t.ShowtimeSeat.Seat.SeatNumber,
                    })
                    .ToList(),
            })
            .ToListAsync(ct);

        var items = new JsonArray();
        var cards = new List<AiCardDto>();
        foreach (var r in rows)
        {
            var seats = r
                .Seats.OrderBy(x => x.SeatRow, StringComparer.Ordinal)
                .ThenBy(x => x.SeatNumber)
                .Select(x => $"{x.SeatRow}{x.SeatNumber}")
                .ToList();
            var statusText = StatusText(r.Status);
            items.Add(
                new JsonObject
                {
                    ["bookingId"] = r.BookingId,
                    ["bookingCode"] = r.BookingCode,
                    ["status"] = statusText,
                    ["movieTitle"] = r.MovieTitle,
                    ["room"] = r.RoomName,
                    ["startTime"] = VietnamTime.ToLocal(r.StartTime).ToString("dd/MM/yyyy HH:mm"),
                    ["seats"] = new JsonArray(seats.Select(s => (JsonNode?)s).ToArray()),
                    ["finalAmountVnd"] = r.FinalAmount,
                }
            );
            cards.Add(
                new AiCardDto(
                    "booking",
                    r.MovieTitle,
                    string.Format(
                        Messages.Ai_Card_BookingSubtitle,
                        VietnamTime.ToLocal(r.StartTime),
                        r.RoomName,
                        string.Join(", ", seats)
                    ),
                    string.Format(
                        Messages.Ai_Card_BookingStatusLine,
                        r.BookingCode,
                        statusText,
                        r.FinalAmount
                    ),
                    r.PosterUrl,
                    $"/account/tickets/{r.BookingId}"
                )
            );
        }

        return new ToolResult(
            new JsonObject { ["count"] = items.Count, ["bookings"] = items },
            cards
        );
    }

    private static string StatusText(BookingStatus s) =>
        s switch
        {
            BookingStatus.Pending => Messages.Ai_BookingStatus_Pending,
            BookingStatus.Confirmed => Messages.Ai_BookingStatus_Confirmed,
            BookingStatus.Cancelled => Messages.Ai_BookingStatus_Cancelled,
            BookingStatus.Expired => Messages.Ai_BookingStatus_Expired,
            _ => s.ToString(),
        };

    private static ToolResult Error(string message) =>
        new(new JsonObject { ["error"] = message }, []);

    private static string? GetString(JsonObject? args, string key)
    {
        if (args is null || !args.TryGetPropertyValue(key, out var node) || node is null)
            return null;
        try
        {
            var s = node.GetValue<string>();
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }
        catch
        {
            return node.ToString() is { Length: > 0 } t ? t : null;
        }
    }

    private static int? GetInt(JsonObject? args, string key)
    {
        if (args is null || !args.TryGetPropertyValue(key, out var node) || node is null)
            return null;
        return int.TryParse(
            node.ToString(),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var v
        )
            ? v
            : null;
    }

    private static bool? GetBool(JsonObject? args, string key)
    {
        if (args is null || !args.TryGetPropertyValue(key, out var node) || node is null)
            return null;
        return bool.TryParse(node.ToString(), out var v) ? v : null;
    }
}

using System.Globalization;
using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Import;
using Cinema.Application.DTOs.Showtimes;
using Cinema.Application.Interfaces;
using Cinema.Application.Services.Import;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema.Application.Services.Showtimes;

internal static class ShowtimeImportColumns
{
    public const string Movie = nameof(Movie);
    public const string Room = nameof(Room);
    public const string StartTime = nameof(StartTime);
    public const string BasePrice = nameof(BasePrice);
    public const string AllowAfterHours = nameof(AllowAfterHours);
}

public class ShowtimeImportProcessor(
    IImportFileReader fileReader,
    IAppDbContext db,
    IShowtimeService showtimeService,
    IExportWriter exportWriter,
    ILogger<ShowtimeImportProcessor> logger
) : ImportProcessorBase<ShowtimeImportRow>(fileReader)
{
    private const decimal MaxBasePrice = 1_000_000_000m;
    private const string TrueKey = "true";
    private const string FalseKey = "false";

    private static readonly string[] DateTimeFormats =
    [
        "dd/MM/yyyy HH:mm",
        "d/M/yyyy H:mm",
        "dd/MM/yyyy HH:mm:ss",
        "d/M/yyyy H:mm:ss",
    ];

    private static readonly string YesKey = Key(Messages.Import_Yes);
    private static readonly string NoKey = Key(Messages.Import_No);

    public override string Module => "showtimes";

    public override string SheetName => Messages.Import_SheetShowtimes;

    protected override IReadOnlyList<ImportColumn> Columns { get; } =
        [
            new(
                ShowtimeImportColumns.Movie,
                true,
                Messages.Import_Showtime_Movie_Desc,
                Messages.Import_Showtime_Movie_Format
            ),
            new(
                ShowtimeImportColumns.Room,
                true,
                Messages.Import_Showtime_Room_Desc,
                Messages.Import_Showtime_Room_Format
            ),
            new(
                ShowtimeImportColumns.StartTime,
                true,
                Messages.Import_Showtime_StartTime_Desc,
                Messages.Import_Showtime_StartTime_Format
            ),
            new(
                ShowtimeImportColumns.BasePrice,
                true,
                Messages.Import_Showtime_BasePrice_Desc,
                Messages.Import_Showtime_BasePrice_Format
            ),
            new(
                ShowtimeImportColumns.AllowAfterHours,
                false,
                Messages.Import_Showtime_AllowAfterHours_Desc,
                Messages.Import_Showtime_AllowAfterHours_Format
            ),
        ];

    protected override ShowtimeImportRow MapRow(IReadOnlyDictionary<string, string?> values) =>
        new(
            values[ShowtimeImportColumns.Movie],
            values[ShowtimeImportColumns.Room],
            values[ShowtimeImportColumns.StartTime],
            values[ShowtimeImportColumns.BasePrice],
            values[ShowtimeImportColumns.AllowAfterHours]
        );

    public override async Task<Stream> BuildTemplateAsync(CancellationToken cancellationToken)
    {
        var movies = await db
            .Movies.AsNoTracking()
            .Where(m => m.Status != MovieStatus.Ended)
            .OrderBy(m => m.Title)
            .Select(m => new { m.Title, m.Status })
            .Take(ExportLimits.MaxRows)
            .ToListAsync(cancellationToken);
        var rooms = await db
            .CinemaRooms.AsNoTracking()
            .Where(r => r.IsActive)
            .OrderBy(r => r.RoomName)
            .Select(r => new
            {
                r.RoomName,
                ScreenType = r.ScreenType != null ? r.ScreenType.Name : null,
            })
            .Take(ExportLimits.MaxRows)
            .ToListAsync(cancellationToken);

        var catalogRowCount = Math.Max(movies.Count, rooms.Count);
        var catalogRows = new List<IReadOnlyList<object?>>(catalogRowCount);
        for (var i = 0; i < catalogRowCount; i++)
        {
            var movie = i < movies.Count ? movies[i] : null;
            var room = i < rooms.Count ? rooms[i] : null;
            catalogRows.Add(
                new object?[]
                {
                    movie?.Title,
                    movie is null ? null : MovieStatusText(movie.Status),
                    room?.RoomName,
                    room?.ScreenType,
                }
            );
        }

        var sheets = new List<ExportSheet>
        {
            new(
                SheetName,
                Columns.Select(c => c.Name).ToList(),
                ExampleRows(
                    movies.FirstOrDefault()?.Title ?? "Phim ví dụ",
                    rooms.FirstOrDefault()?.RoomName ?? "Phòng ví dụ"
                )
            ),
            new(
                Messages.Import_SheetGuide,
                [
                    Messages.Import_GuideColColumn,
                    Messages.Import_GuideColRequirement,
                    Messages.Import_GuideColDescription,
                    Messages.Import_GuideColFormat,
                ],
                GuideRows()
            ),
            new(
                Messages.Import_SheetCatalog,
                [
                    Messages.Import_LabelMovie,
                    Messages.Import_LabelMovieStatus,
                    Messages.Import_LabelRoom,
                    Messages.Import_LabelScreenType,
                ],
                catalogRows
            ),
        };

        return await exportWriter.WriteXlsxWorkbookAsync(sheets, cancellationToken);
    }

    public override async Task<IImportContext> CreateContextAsync(
        IReadOnlyList<ImportRow<ShowtimeImportRow>> rows,
        CancellationToken cancellationToken
    )
    {
        var movies = await db
            .Movies.AsNoTracking()
            .Select(m => new
            {
                m.MovieId,
                m.Title,
                m.Status,
                m.DurationMinutes,
            })
            .ToListAsync(cancellationToken);
        var rooms = await db
            .CinemaRooms.AsNoTracking()
            .Select(r => new
            {
                r.RoomId,
                r.RoomName,
                r.IsActive,
            })
            .ToListAsync(cancellationToken);
        var roomsWithSeats = await db
            .Seats.AsNoTracking()
            .Where(s => s.IsActive && !s.IsAisle)
            .Select(s => s.RoomId)
            .Distinct()
            .ToListAsync(cancellationToken);
        var settings = await db.LoadCinemaSettingsOrDefaultAsync(cancellationToken);

        var starts = new List<DateTime>();
        foreach (var row in rows)
            if (TryParseStartTime(row.Data.StartTime, out var start))
                starts.Add(start);

        var existing = new Dictionary<int, List<(DateTime Start, DateTime End)>>();
        if (starts.Count > 0)
        {
            var from = starts.Min().AddDays(-1);
            var to = starts.Max().AddDays(ShowtimeRules.ConflictLookaheadDays);
            var scheduled = await db
                .Showtimes.AsNoTracking()
                .Where(s =>
                    s.Status == ShowtimeStatus.Scheduled && s.EndTime > from && s.StartTime < to
                )
                .Select(s => new
                {
                    s.RoomId,
                    s.StartTime,
                    s.EndTime,
                })
                .ToListAsync(cancellationToken);

            foreach (var item in scheduled)
            {
                if (!existing.TryGetValue(item.RoomId, out var intervals))
                {
                    intervals = [];
                    existing[item.RoomId] = intervals;
                }

                intervals.Add((item.StartTime, item.EndTime));
            }
        }

        return new ShowtimeImportContext(
            Index(
                movies.Select(m => new MovieInfo(m.MovieId, m.Title, m.Status, m.DurationMinutes)),
                m => m.Title
            ),
            Index(rooms.Select(r => new RoomInfo(r.RoomId, r.RoomName, r.IsActive)), r => r.Name),
            roomsWithSeats.ToHashSet(),
            settings.OpeningTime,
            settings.ClosingTime,
            settings.RoomBufferMinutes,
            existing
        );
    }

    public override ImportRow<ShowtimeImportRow> ValidateRow(
        ImportRow<ShowtimeImportRow> row,
        IImportContext context
    )
    {
        var lookups = (ShowtimeImportContext)context;
        var data = row.Data;
        var errors = new List<ImportCellError>();

        var movie = ResolveMovie(data.Movie, lookups, errors);
        var room = ResolveRoom(data.Room, lookups, errors);

        DateTime? startUtc = null;
        if (data.StartTime is null)
            errors.Add(
                new ImportCellError(
                    ShowtimeImportColumns.StartTime,
                    Messages.Invalid_ImportRequired
                )
            );
        else if (!TryParseStartTime(data.StartTime, out var parsedStart))
            errors.Add(
                new ImportCellError(
                    ShowtimeImportColumns.StartTime,
                    Messages.Invalid_ImportDateTimeFormat
                )
            );
        else if (parsedStart <= DateTime.UtcNow)
            errors.Add(
                new ImportCellError(
                    ShowtimeImportColumns.StartTime,
                    Messages.Rule_ShowtimeStartMustBeFuture
                )
            );
        else
            startUtc = parsedStart;

        decimal? basePrice = null;
        if (data.BasePrice is null)
            errors.Add(
                new ImportCellError(
                    ShowtimeImportColumns.BasePrice,
                    Messages.Invalid_ImportRequired
                )
            );
        else if (
            !decimal.TryParse(
                data.BasePrice,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out var parsedPrice
            )
        )
            errors.Add(
                new ImportCellError(
                    ShowtimeImportColumns.BasePrice,
                    Messages.Invalid_ImportDecimal
                )
            );
        else if (parsedPrice <= 0 || parsedPrice > MaxBasePrice)
            errors.Add(
                new ImportCellError(
                    ShowtimeImportColumns.BasePrice,
                    string.Format(
                        Messages.Invalid_ImportBasePriceRange,
                        MaxBasePrice.ToString("0", CultureInfo.InvariantCulture)
                    )
                )
            );
        else
            basePrice = parsedPrice;

        var allowAfterHours = false;
        if (data.AllowAfterHours is not null)
        {
            var flag = Key(data.AllowAfterHours);
            if (flag == YesKey || flag == TrueKey)
                allowAfterHours = true;
            else if (flag != NoKey && flag != FalseKey)
                errors.Add(
                    new ImportCellError(
                        ShowtimeImportColumns.AllowAfterHours,
                        Messages.Invalid_ImportBoolean
                    )
                );
        }

        if (movie is not null && room is not null && startUtc is { } start)
        {
            var showEnd = start.AddMinutes(movie.DurationMinutes);
            var blockedUntil = showEnd.AddMinutes(lookups.BufferMinutes);

            var reason =
                OperatingHours.Violation(
                    start,
                    showEnd,
                    lookups.OpeningTime,
                    lookups.ClosingTime,
                    allowAfterHours
                ) ?? OverlapReason(lookups, room.Id, start, blockedUntil);

            if (reason is not null)
                errors.Add(new ImportCellError(ShowtimeImportColumns.StartTime, reason));
            else if (errors.Count == 0)
                lookups.Reserve(room.Id, start, blockedUntil, row.RowNumber);
        }

        var request =
            errors.Count == 0
                ? new UpsertShowtimeRequest(
                    movie!.Id,
                    room!.Id,
                    startUtc!.Value,
                    basePrice!.Value,
                    allowAfterHours
                )
                : null;

        return row with
        {
            Data = data with { Request = request },
            Errors = errors,
        };
    }

    public override async Task<BulkActionResponse> ApplyRowsAsync(
        IReadOnlyList<ImportRow<ShowtimeImportRow>> rows,
        IJobProgressReporter? progress,
        CancellationToken cancellationToken
    )
    {
        var results = new List<BulkActionResultItem>(rows.Count);

        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            try
            {
                var created = await showtimeService.CreateShowtimeAsync(
                    row.Data.Request!,
                    cancellationToken
                );
                results.Add(
                    new BulkActionResultItem(created.ShowtimeId, true, null, row.RowNumber)
                );
            }
            catch (AppException ex)
            {
                results.Add(new BulkActionResultItem(0, false, ex.Message, row.RowNumber));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(
                    ex,
                    "Import suất chiếu thất bại tại dòng {RowNumber}",
                    row.RowNumber
                );
                results.Add(
                    new BulkActionResultItem(0, false, Messages.Bulk_UnknownError, row.RowNumber)
                );

                for (var j = i + 1; j < rows.Count; j++)
                    results.Add(
                        new BulkActionResultItem(
                            0,
                            false,
                            Messages.Import_NotProcessed,
                            rows[j].RowNumber
                        )
                    );

                break;
            }

            if (progress is not null)
                await progress.ReportAsync(i + 1, cancellationToken);
        }

        return BulkOperation.Build(results);
    }

    private static MovieInfo? ResolveMovie(
        string? name,
        ShowtimeImportContext lookups,
        List<ImportCellError> errors
    )
    {
        if (name is null)
        {
            errors.Add(
                new ImportCellError(ShowtimeImportColumns.Movie, Messages.Invalid_ImportRequired)
            );
            return null;
        }

        if (!lookups.Movies.TryGetValue(Key(name), out var matches))
        {
            errors.Add(
                ReferenceError(
                    false,
                    ShowtimeImportColumns.Movie,
                    Messages.Import_LabelMovie,
                    name
                )
            );
            return null;
        }

        var open = matches.Where(m => m.Status != MovieStatus.Ended).ToList();
        if (open.Count == 0)
        {
            errors.Add(
                new ImportCellError(
                    ShowtimeImportColumns.Movie,
                    Messages.Rule_MovieEndedCannotCreateShowtime
                )
            );
            return null;
        }

        if (open.Count > 1)
        {
            errors.Add(
                ReferenceError(
                    true,
                    ShowtimeImportColumns.Movie,
                    Messages.Import_LabelMovie,
                    name
                )
            );
            return null;
        }

        return open[0];
    }

    private static RoomInfo? ResolveRoom(
        string? name,
        ShowtimeImportContext lookups,
        List<ImportCellError> errors
    )
    {
        if (name is null)
        {
            errors.Add(
                new ImportCellError(ShowtimeImportColumns.Room, Messages.Invalid_ImportRequired)
            );
            return null;
        }

        if (!lookups.Rooms.TryGetValue(Key(name), out var matches))
        {
            errors.Add(
                ReferenceError(
                    false,
                    ShowtimeImportColumns.Room,
                    Messages.Import_LabelRoom,
                    name
                )
            );
            return null;
        }

        var active = matches.Where(r => r.IsActive).ToList();
        if (active.Count == 0)
        {
            errors.Add(
                new ImportCellError(ShowtimeImportColumns.Room, Messages.Rule_RoomInactive)
            );
            return null;
        }

        if (active.Count > 1)
        {
            errors.Add(
                ReferenceError(
                    true,
                    ShowtimeImportColumns.Room,
                    Messages.Import_LabelRoom,
                    name
                )
            );
            return null;
        }

        if (!lookups.RoomsWithSeats.Contains(active[0].Id))
        {
            errors.Add(
                new ImportCellError(
                    ShowtimeImportColumns.Room,
                    Messages.Rule_RoomHasNoSeatLayout
                )
            );
            return null;
        }

        return active[0];
    }

    private static string? OverlapReason(
        ShowtimeImportContext lookups,
        int roomId,
        DateTime start,
        DateTime blockedUntil
    )
    {
        if (
            lookups.Existing.TryGetValue(roomId, out var existing)
            && existing.Any(i => i.End > start && i.Start < blockedUntil)
        )
            return Messages.Conflict_ShowtimeOverlapExisting;

        if (lookups.Planned.TryGetValue(roomId, out var planned))
        {
            var clash = planned.FirstOrDefault(i => i.End > start && i.Start < blockedUntil);
            if (clash.RowNumber != 0)
                return string.Format(
                    Messages.Invalid_ImportShowtimeOverlapInFile,
                    clash.RowNumber
                );
        }

        return null;
    }

    private static ImportCellError ReferenceError(
        bool ambiguous,
        string column,
        string label,
        string name
    ) =>
        new(
            column,
            string.Format(
                ambiguous
                    ? Messages.Invalid_ImportReferenceAmbiguous
                    : Messages.Invalid_ImportReferenceNotFound,
                label,
                name
            )
        );

    private static bool TryParseStartTime(string? text, out DateTime startUtc)
    {
        startUtc = default;
        if (
            text is null
            || !DateTime.TryParseExact(
                text,
                DateTimeFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var local
            )
        )
            return false;

        startUtc = BusinessTime.ToUtc(local);
        return true;
    }

    private static string Key(string? value) => NameNormalizer.Normalize(value).ToLowerInvariant();

    private static Dictionary<string, List<T>> Index<T>(
        IEnumerable<T> items,
        Func<T, string> nameOf
    )
    {
        var result = new Dictionary<string, List<T>>(StringComparer.Ordinal);
        foreach (var item in items)
        {
            var key = Key(nameOf(item));
            if (!result.TryGetValue(key, out var list))
            {
                list = [];
                result[key] = list;
            }

            list.Add(item);
        }

        return result;
    }

    private static string MovieStatusText(MovieStatus status) =>
        status == MovieStatus.NowShowing
            ? Messages.Import_StatusNowShowing
            : Messages.Import_StatusComingSoon;

    private static IReadOnlyList<IReadOnlyList<object?>> ExampleRows(string movie, string room)
    {
        var day = BusinessTime.Today().AddDays(1);
        var first = day.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        var second = day.AddDays(1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

        return
        [
            new object?[] { movie, room, $"{first} 18:30", "90000", Messages.Import_No },
            new object?[] { movie, room, $"{second} 21:30", "120000", Messages.Import_Yes },
        ];
    }

    private IReadOnlyList<IReadOnlyList<object?>> GuideRows()
    {
        var rows = Columns
            .Select(c =>
                (IReadOnlyList<object?>)
                    new object?[]
                    {
                        c.Name,
                        c.Required ? Messages.Import_Required : Messages.Import_Optional,
                        c.Description,
                        c.Format,
                    }
            )
            .ToList();

        rows.Add(new object?[4]);
        rows.Add(
            GuideNote(
                string.Format(
                    Messages.Import_Note_Limits,
                    ImportLimits.MaxRows,
                    ImportLimits.MaxFileBytes / (1024 * 1024)
                )
            )
        );
        rows.Add(GuideNote(Messages.Import_Note_Showtime_Create));
        rows.Add(GuideNote(Messages.Import_Note_Showtime_Lookup));
        rows.Add(GuideNote(Messages.Import_Note_Showtime_Examples));

        return rows;
    }

    private static object?[] GuideNote(string text) =>
        [Messages.Import_GuideNote, null, text, null];

    private sealed record MovieInfo(int Id, string Title, MovieStatus Status, int DurationMinutes);

    private sealed record RoomInfo(int Id, string Name, bool IsActive);

    private sealed class ShowtimeImportContext(
        Dictionary<string, List<MovieInfo>> movies,
        Dictionary<string, List<RoomInfo>> rooms,
        HashSet<int> roomsWithSeats,
        TimeOnly? openingTime,
        TimeOnly? closingTime,
        int bufferMinutes,
        Dictionary<int, List<(DateTime Start, DateTime End)>> existing
    ) : IImportContext
    {
        public Dictionary<string, List<MovieInfo>> Movies { get; } = movies;
        public Dictionary<string, List<RoomInfo>> Rooms { get; } = rooms;
        public HashSet<int> RoomsWithSeats { get; } = roomsWithSeats;
        public TimeOnly? OpeningTime { get; } = openingTime;
        public TimeOnly? ClosingTime { get; } = closingTime;
        public int BufferMinutes { get; } = bufferMinutes;
        public Dictionary<int, List<(DateTime Start, DateTime End)>> Existing { get; } = existing;
        public Dictionary<
            int,
            List<(DateTime Start, DateTime End, int RowNumber)>
        > Planned { get; } = [];

        public void Reserve(int roomId, DateTime start, DateTime end, int rowNumber)
        {
            if (!Planned.TryGetValue(roomId, out var intervals))
            {
                intervals = [];
                Planned[roomId] = intervals;
            }

            intervals.Add((start, end, rowNumber));
        }
    }
}

using System.Globalization;
using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Import;
using Cinema.Application.DTOs.Rooms;
using Cinema.Application.Interfaces;
using Cinema.Application.Services.Import;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Rooms;

internal static class SeatLayoutImportColumns
{
    public const string SeatRow = nameof(SeatRow);
    public const string SeatNumber = nameof(SeatNumber);
    public const string SeatType = nameof(SeatType);
    public const string IsActive = nameof(IsActive);
    public const string IsAisle = nameof(IsAisle);
    public const string CoupleGroup = nameof(CoupleGroup);
}

public sealed class SeatLayoutImportProcessor(
    int roomId,
    IImportFileReader fileReader,
    IAppDbContext db,
    IRoomService roomService,
    IExportWriter exportWriter
) : ImportProcessorBase<SeatLayoutImportRow>(fileReader)
{
    private const int MaxCoupleGroupLength = 50;
    private const int MaxSeatRowLength = 5;
    private const int SampleSeatCount = 5;
    private const string TrueKey = "true";
    private const string FalseKey = "false";

    private static readonly string YesKey = Key(Messages.Import_Yes);
    private static readonly string NoKey = Key(Messages.Import_No);

    public override string Module => ImportExportModules.SeatLayout;

    public override string SheetName => Messages.Import_SheetSeatLayout;

    protected override IReadOnlyList<ImportColumn> Columns { get; } =
        [
            new(
                SeatLayoutImportColumns.SeatRow,
                true,
                Messages.Import_SeatLayout_SeatRow_Desc,
                Messages.Import_SeatLayout_SeatRow_Format
            ),
            new(
                SeatLayoutImportColumns.SeatNumber,
                true,
                Messages.Import_SeatLayout_SeatNumber_Desc,
                Messages.Import_SeatLayout_SeatNumber_Format
            ),
            new(
                SeatLayoutImportColumns.SeatType,
                true,
                Messages.Import_SeatLayout_SeatType_Desc,
                Messages.Import_SeatLayout_SeatType_Format
            ),
            new(
                SeatLayoutImportColumns.IsActive,
                false,
                Messages.Import_SeatLayout_IsActive_Desc,
                Messages.Import_SeatLayout_IsActive_Format
            ),
            new(
                SeatLayoutImportColumns.IsAisle,
                false,
                Messages.Import_SeatLayout_IsAisle_Desc,
                Messages.Import_SeatLayout_IsAisle_Format
            ),
            new(
                SeatLayoutImportColumns.CoupleGroup,
                false,
                Messages.Import_SeatLayout_CoupleGroup_Desc,
                Messages.Import_SeatLayout_CoupleGroup_Format
            ),
        ];

    protected override SeatLayoutImportRow MapRow(IReadOnlyDictionary<string, string?> values) =>
        new(
            values[SeatLayoutImportColumns.SeatRow],
            values[SeatLayoutImportColumns.SeatNumber],
            values[SeatLayoutImportColumns.SeatType],
            values[SeatLayoutImportColumns.IsActive],
            values[SeatLayoutImportColumns.IsAisle],
            values[SeatLayoutImportColumns.CoupleGroup]
        );

    public override async Task<Stream> BuildTemplateAsync(CancellationToken cancellationToken)
    {
        var room = await LoadRoomAsync(cancellationToken);
        var seatTypes = await db
            .SeatTypes.AsNoTracking()
            .OrderBy(st => st.Name)
            .Select(st => new { st.Name, st.PriceMultiplier })
            .Take(ExportLimits.MaxRows)
            .ToListAsync(cancellationToken);

        var first = seatTypes.FirstOrDefault()?.Name ?? "Ghế thường";
        var second = seatTypes.Skip(1).FirstOrDefault()?.Name ?? first;

        var sheets = new List<ExportSheet>
        {
            new(
                SheetName,
                Columns.Select(c => c.Name).ToList(),
                ExampleRows(first, second)
            ),
            new(
                Messages.Import_SheetGuide,
                [
                    Messages.Import_GuideColColumn,
                    Messages.Import_GuideColRequirement,
                    Messages.Import_GuideColDescription,
                    Messages.Import_GuideColFormat,
                ],
                GuideRows(room)
            ),
            new(
                Messages.Import_SheetCatalog,
                [Messages.Import_LabelSeatType, Messages.Import_LabelPriceMultiplier],
                seatTypes
                    .Select(st =>
                        (IReadOnlyList<object?>)new object?[] { st.Name, st.PriceMultiplier }
                    )
                    .ToList()
            ),
        };

        return await exportWriter.WriteXlsxWorkbookAsync(sheets, cancellationToken);
    }

    public override async Task<IImportContext> CreateContextAsync(
        IReadOnlyList<ImportRow<SeatLayoutImportRow>> rows,
        CancellationToken cancellationToken
    )
    {
        var room = await LoadRoomAsync(cancellationToken);

        var seatTypes = await db
            .SeatTypes.AsNoTracking()
            .Select(st => new SeatTypeInfo(st.SeatTypeId, st.Name))
            .ToListAsync(cancellationToken);

        var current = (
            await db
                .Seats.AsNoTracking()
                .Where(s => s.RoomId == roomId)
                .Select(s => new CurrentSeat(
                    s.SeatId,
                    s.SeatRow,
                    s.SeatNumber,
                    s.SeatTypeId,
                    s.IsActive,
                    s.IsAisle,
                    s.CoupleId
                ))
                .ToListAsync(cancellationToken)
        ).ToDictionary(s => (s.Row, s.Number));

        var now = DateTime.UtcNow;
        var liveRows = await db
            .ShowtimeSeats.AsNoTracking()
            .Where(ss =>
                ss.Seat.RoomId == roomId
                && (ss.Status == ShowtimeSeatStatus.Booked || ss.Status == ShowtimeSeatStatus.Held)
                && ss.Showtime.Status == ShowtimeStatus.Scheduled
                && ss.Showtime.StartTime > now
            )
            .Select(ss => new { ss.SeatId, ss.ShowtimeId })
            .ToListAsync(cancellationToken);
        var liveShowtimesBySeat = liveRows
            .GroupBy(x => x.SeatId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ShowtimeId).ToHashSet());

        var historySeatIds = (
            await db
                .ShowtimeSeats.AsNoTracking()
                .Where(ss => ss.Seat.RoomId == roomId)
                .Select(ss => ss.SeatId)
                .Distinct()
                .ToListAsync(cancellationToken)
        ).ToHashSet();

        var futureShowtimeCount = await db.Showtimes.CountAsync(
            s => s.RoomId == roomId && s.Status == ShowtimeStatus.Scheduled && s.StartTime > now,
            cancellationToken
        );

        var context = new SeatLayoutImportContext
        {
            RoomId = room.RoomId,
            Rows = room.Rows,
            Cols = room.Cols,
            SeatTypes = Index(seatTypes, st => st.Name),
            Current = current,
            LiveShowtimesBySeat = liveShowtimesBySeat,
            HistorySeatIds = historySeatIds,
            FutureShowtimeCount = futureShowtimeCount,
        };

        AnalyzeLayout(rows, context);
        return context;
    }

    public override ImportRow<SeatLayoutImportRow> ValidateRow(
        ImportRow<SeatLayoutImportRow> row,
        IImportContext context
    )
    {
        var lookups = (SeatLayoutImportContext)context;
        var data = row.Data;
        var errors = new List<ImportCellError>();

        string? seatRow = null;
        if (data.SeatRow is null)
            errors.Add(Required(SeatLayoutImportColumns.SeatRow));
        else if (!TryParseSeatRow(data.SeatRow, out var parsedRow))
            errors.Add(
                new ImportCellError(
                    SeatLayoutImportColumns.SeatRow,
                    string.Format(Messages.Invalid_ImportSeatRowFormat, MaxSeatRowLength)
                )
            );
        else if (RowIndex(parsedRow) >= lookups.Rows)
            errors.Add(
                new ImportCellError(
                    SeatLayoutImportColumns.SeatRow,
                    string.Format(Messages.Invalid_ImportSeatRowOutOfRange, lookups.Rows)
                )
            );
        else
            seatRow = parsedRow;

        int? seatNumber = null;
        if (data.SeatNumber is null)
            errors.Add(Required(SeatLayoutImportColumns.SeatNumber));
        else if (!TryParseSeatNumber(data.SeatNumber, out var parsedNumber))
            errors.Add(
                new ImportCellError(
                    SeatLayoutImportColumns.SeatNumber,
                    Messages.Invalid_ImportInteger
                )
            );
        else if (parsedNumber < 1 || parsedNumber > lookups.Cols)
            errors.Add(
                new ImportCellError(
                    SeatLayoutImportColumns.SeatNumber,
                    string.Format(Messages.Invalid_ImportSeatNumberOutOfRange, lookups.Cols)
                )
            );
        else
            seatNumber = parsedNumber;

        if (!TryParseFlag(data.IsActive, true, out var isActive))
            errors.Add(
                new ImportCellError(
                    SeatLayoutImportColumns.IsActive,
                    Messages.Invalid_ImportBoolean
                )
            );

        if (!TryParseFlag(data.IsAisle, false, out var isAisle))
            errors.Add(
                new ImportCellError(
                    SeatLayoutImportColumns.IsAisle,
                    Messages.Invalid_ImportBoolean
                )
            );

        int? seatTypeId = null;
        if (data.SeatType is null)
        {
            if (!isAisle)
                errors.Add(Required(SeatLayoutImportColumns.SeatType));
        }
        else
            seatTypeId = ResolveSeatType(data.SeatType, lookups, errors);

        var coupleKey = CoupleKey(data.CoupleGroup);
        if (data.CoupleGroup is not null)
        {
            if (data.CoupleGroup.Length > MaxCoupleGroupLength)
                errors.Add(
                    new ImportCellError(
                        SeatLayoutImportColumns.CoupleGroup,
                        string.Format(
                            Messages.Invalid_ImportCoupleGroupTooLong,
                            MaxCoupleGroupLength
                        )
                    )
                );
            else if (isAisle)
                errors.Add(
                    new ImportCellError(
                        SeatLayoutImportColumns.CoupleGroup,
                        Messages.Invalid_ImportCoupleOnAisle
                    )
                );
        }

        if (lookups.CrossErrors.TryGetValue(row.RowNumber, out var crossErrors))
            errors.AddRange(crossErrors);

        SeatLayoutItem? item = null;
        if (errors.Count == 0)
            item = new SeatLayoutItem(
                seatRow!,
                seatNumber!.Value,
                seatTypeId,
                isActive,
                isAisle,
                coupleKey is null ? null : lookups.CoupleIds[coupleKey]
            );

        return row with
        {
            Data = data with { RoomId = lookups.RoomId, Item = item },
            Errors = errors,
        };
    }

    public IReadOnlyList<string> BuildWarnings(
        IReadOnlyList<ImportRow<SeatLayoutImportRow>> rows,
        IImportContext context
    )
    {
        var lookups = (SeatLayoutImportContext)context;
        var items = rows.Where(r => r.IsValid && r.Data.Item is not null)
            .Select(r => r.Data.Item!)
            .ToList();
        if (items.Count == 0)
            return [];

        var planned = items.ToDictionary(i => (i.SeatRow, i.SeatNumber));
        var created = items.Count(i => !lookups.Current.ContainsKey((i.SeatRow, i.SeatNumber)));
        var changed = new List<CurrentSeat>();
        var dropped = new List<CurrentSeat>();

        foreach (var seat in lookups.Current.Values)
        {
            if (!planned.TryGetValue((seat.Row, seat.Number), out var item))
                dropped.Add(seat);
            else if (
                seat.SeatTypeId != item.SeatTypeId
                || seat.IsActive != item.IsActive
                || seat.IsAisle != item.IsAisle
                || seat.CoupleId != item.CoupleId
            )
                changed.Add(seat);
        }

        var warnings = new List<string>();

        var blocked = changed
            .Concat(dropped)
            .Where(s => lookups.LiveShowtimesBySeat.ContainsKey(s.SeatId))
            .OrderBy(s => s.Row.Length)
            .ThenBy(s => s.Row, StringComparer.Ordinal)
            .ThenBy(s => s.Number)
            .ToList();
        if (blocked.Count > 0)
        {
            var showtimeCount = blocked
                .SelectMany(s => lookups.LiveShowtimesBySeat[s.SeatId])
                .Distinct()
                .Count();
            var sample = string.Join(
                ", ",
                blocked.Take(SampleSeatCount).Select(s => $"{s.Row}{s.Number}")
            );
            if (blocked.Count > SampleSeatCount)
                sample += ", …";

            warnings.Add(
                string.Format(
                    Messages.Import_Warn_SeatLayoutLiveSeats,
                    blocked.Count,
                    showtimeCount,
                    sample
                )
            );
        }

        var removed = dropped.Count(s => !lookups.HistorySeatIds.Contains(s.SeatId));
        if (removed > 0)
            warnings.Add(string.Format(Messages.Import_Warn_SeatLayoutRemoved, removed));

        var withHistory = dropped.Where(s => lookups.HistorySeatIds.Contains(s.SeatId)).ToList();
        var deactivated = withHistory.Count(s => s.IsActive);
        if (deactivated > 0)
            warnings.Add(string.Format(Messages.Import_Warn_SeatLayoutDeactivated, deactivated));

        var touched = created + changed.Count + withHistory.Count;
        if (touched > 0 && lookups.FutureShowtimeCount > 0)
            warnings.Add(
                string.Format(
                    Messages.Import_Warn_SeatLayoutFutureShowtimes,
                    lookups.FutureShowtimeCount
                )
            );

        return warnings;
    }

    public async Task<UpdateSeatLayoutResponse> ApplyLayoutAsync(
        IReadOnlyList<ImportRow<SeatLayoutImportRow>> rows,
        CancellationToken cancellationToken
    )
    {
        var room = await LoadRoomAsync(cancellationToken);
        var items = rows.Select(r => r.Data.Item!).ToList();

        if (HasGaps(items))
            throw new BusinessRuleException(Messages.Rule_ImportSeatLayoutSkipBreaksRows);

        return await roomService.UpdateSeatLayoutAsync(
            roomId,
            new UpdateSeatLayoutRequest(room.Rows, room.Cols, items),
            cancellationToken
        );
    }

    public override async Task<BulkActionResponse> ApplyRowsAsync(
        IReadOnlyList<ImportRow<SeatLayoutImportRow>> rows,
        IJobProgressReporter? progress,
        CancellationToken cancellationToken
    )
    {
        var results = new List<BulkActionResultItem>(rows.Count);

        try
        {
            await ApplyLayoutAsync(rows, cancellationToken);
            results.AddRange(
                rows.Select(row => new BulkActionResultItem(0, true, null, row.RowNumber))
            );
        }
        catch (AppException ex)
        {
            results.AddRange(
                rows.Select(row => new BulkActionResultItem(0, false, ex.Message, row.RowNumber))
            );
        }

        if (progress is not null)
            await progress.ReportAsync(rows.Count, cancellationToken);

        return BulkOperation.Build(results);
    }

    private static void AnalyzeLayout(
        IReadOnlyList<ImportRow<SeatLayoutImportRow>> rows,
        SeatLayoutImportContext context
    )
    {
        var firstRowByPosition = new Dictionary<(string Row, int Number), int>();
        var positions = new List<ParsedPosition>(rows.Count);

        foreach (var row in rows)
        {
            if (
                !TryParseSeatRow(row.Data.SeatRow, out var seatRow)
                || !TryParseSeatNumber(row.Data.SeatNumber, out var number)
                || number < 1
            )
                continue;

            if (firstRowByPosition.TryGetValue((seatRow, number), out var firstRow))
            {
                context.AddCrossError(
                    row.RowNumber,
                    SeatLayoutImportColumns.SeatNumber,
                    string.Format(Messages.Invalid_ImportSeatDuplicateInFile, firstRow)
                );
                continue;
            }

            firstRowByPosition[(seatRow, number)] = row.RowNumber;

            TryParseFlag(row.Data.IsAisle, false, out var isAisle);
            positions.Add(
                new ParsedPosition(
                    row.RowNumber,
                    seatRow,
                    number,
                    isAisle,
                    CoupleKey(row.Data.CoupleGroup),
                    NameNormalizer.Normalize(row.Data.CoupleGroup)
                )
            );
        }

        foreach (
            var group in positions
                .Where(p => RowIndex(p.Row) < context.Rows && p.Number <= context.Cols)
                .GroupBy(p => p.Row)
        )
        {
            var previous = 0;
            foreach (var position in group.OrderBy(p => p.Number))
            {
                if (position.Number > previous + 1)
                    context.AddCrossError(
                        position.RowNumber,
                        SeatLayoutImportColumns.SeatNumber,
                        string.Format(
                            Messages.Invalid_ImportSeatGap,
                            position.Row,
                            RangeText(previous + 1, position.Number - 1)
                        )
                    );

                previous = position.Number;
            }
        }

        foreach (
            var group in positions
                .Where(p => !p.IsAisle && p.CoupleKey is not null)
                .GroupBy(p => p.CoupleKey!)
        )
        {
            var members = group
                .OrderBy(p => p.Row.Length)
                .ThenBy(p => p.Row, StringComparer.Ordinal)
                .ThenBy(p => p.Number)
                .ToList();
            var label = members[0].CoupleLabel;

            string? error = null;
            if (members.Count != 2)
                error = string.Format(Messages.Invalid_ImportCoupleGroupSize, label, members.Count);
            else if (members[0].Row != members[1].Row)
                error = string.Format(Messages.Invalid_ImportCoupleGroupRow, label);
            else if (members[1].Number - members[0].Number != 1)
                error = string.Format(Messages.Invalid_ImportCoupleGroupAdjacent, label);

            if (error is not null)
            {
                foreach (var member in members)
                    context.AddCrossError(
                        member.RowNumber,
                        SeatLayoutImportColumns.CoupleGroup,
                        error
                    );
                continue;
            }

            var existingIds = members
                .Select(m =>
                    context.Current.TryGetValue((m.Row, m.Number), out var seat)
                        ? seat.CoupleId
                        : null
                )
                .Distinct()
                .ToList();
            context.CoupleIds[group.Key] =
                existingIds.Count == 1 && existingIds[0] is { } existing ? existing : Guid.NewGuid();
        }
    }

    private static bool HasGaps(IEnumerable<SeatLayoutItem> items) =>
        items
            .GroupBy(i => i.SeatRow)
            .Any(group =>
            {
                var previous = 0;
                foreach (var number in group.Select(i => i.SeatNumber).Order())
                {
                    if (number > previous + 1)
                        return true;

                    previous = number;
                }

                return false;
            });

    private async Task<RoomInfo> LoadRoomAsync(CancellationToken cancellationToken) =>
        await db
            .CinemaRooms.AsNoTracking()
            .Where(r => r.RoomId == roomId)
            .Select(r => new RoomInfo(r.RoomId, r.Rows, r.Cols))
            .FirstOrDefaultAsync(cancellationToken)
        ?? throw new NotFoundException(Messages.NotFound_Room);

    private static int? ResolveSeatType(
        string name,
        SeatLayoutImportContext lookups,
        List<ImportCellError> errors
    )
    {
        if (!lookups.SeatTypes.TryGetValue(Key(name), out var matches))
        {
            errors.Add(
                ReferenceError(
                    false,
                    SeatLayoutImportColumns.SeatType,
                    Messages.Import_LabelSeatType,
                    name
                )
            );
            return null;
        }

        if (matches.Count > 1)
        {
            errors.Add(
                ReferenceError(
                    true,
                    SeatLayoutImportColumns.SeatType,
                    Messages.Import_LabelSeatType,
                    name
                )
            );
            return null;
        }

        return matches[0].Id;
    }

    private static ImportCellError Required(string column) =>
        new(column, Messages.Invalid_ImportRequired);

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

    private static bool TryParseSeatRow(string? text, out string seatRow)
    {
        seatRow = string.Empty;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var candidate = text.Trim().ToUpperInvariant();
        if (candidate.Length > MaxSeatRowLength || !candidate.All(c => c is >= 'A' and <= 'Z'))
            return false;

        seatRow = candidate;
        return true;
    }

    private static int RowIndex(string seatRow)
    {
        var index = 0;
        foreach (var ch in seatRow)
            index = index * 26 + (ch - 'A' + 1);

        return index - 1;
    }

    private static bool TryParseSeatNumber(string? text, out int number)
    {
        number = 0;
        return text is not null
            && int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out number);
    }

    private static bool TryParseFlag(string? text, bool defaultValue, out bool value)
    {
        value = defaultValue;
        if (text is null)
            return true;

        var flag = Key(text);
        if (flag == YesKey || flag == TrueKey)
        {
            value = true;
            return true;
        }

        if (flag == NoKey || flag == FalseKey)
        {
            value = false;
            return true;
        }

        return false;
    }

    private static string? CoupleKey(string? text)
    {
        var key = Key(text);
        return key.Length == 0 ? null : key;
    }

    private static string RangeText(int from, int to) =>
        from == to
            ? from.ToString(CultureInfo.InvariantCulture)
            : $"{from.ToString(CultureInfo.InvariantCulture)}–{to.ToString(CultureInfo.InvariantCulture)}";

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

    private static IReadOnlyList<IReadOnlyList<object?>> ExampleRows(string first, string second) =>
        [
            new object?[] { "A", 1, first, Messages.Import_Yes, Messages.Import_No, null },
            new object?[] { "A", 2, first, Messages.Import_Yes, Messages.Import_No, null },
            new object?[] { "A", 3, null, Messages.Import_Yes, Messages.Import_Yes, null },
            new object?[] { "B", 1, second, Messages.Import_Yes, Messages.Import_No, "C1" },
            new object?[] { "B", 2, second, Messages.Import_Yes, Messages.Import_No, "C1" },
        ];

    private IReadOnlyList<IReadOnlyList<object?>> GuideRows(RoomInfo room)
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
        rows.Add(
            GuideNote(string.Format(Messages.Import_Note_SeatLayout_Bounds, room.Rows, room.Cols))
        );
        rows.Add(GuideNote(Messages.Import_Note_SeatLayout_Replace));
        rows.Add(GuideNote(Messages.Import_Note_SeatLayout_Contiguous));
        rows.Add(GuideNote(Messages.Import_Note_SeatLayout_Couple));

        return rows;
    }

    private static object?[] GuideNote(string text) =>
        [Messages.Import_GuideNote, null, text, null];

    private sealed record RoomInfo(int RoomId, int Rows, int Cols);

    private sealed record SeatTypeInfo(int Id, string Name);

    private sealed record CurrentSeat(
        int SeatId,
        string Row,
        int Number,
        int? SeatTypeId,
        bool IsActive,
        bool IsAisle,
        Guid? CoupleId
    );

    private sealed record ParsedPosition(
        int RowNumber,
        string Row,
        int Number,
        bool IsAisle,
        string? CoupleKey,
        string CoupleLabel
    );

    private sealed class SeatLayoutImportContext : IImportContext
    {
        public required int RoomId { get; init; }
        public required int Rows { get; init; }
        public required int Cols { get; init; }
        public required Dictionary<string, List<SeatTypeInfo>> SeatTypes { get; init; }
        public required Dictionary<(string Row, int Number), CurrentSeat> Current { get; init; }
        public required Dictionary<int, HashSet<int>> LiveShowtimesBySeat { get; init; }
        public required HashSet<int> HistorySeatIds { get; init; }
        public required int FutureShowtimeCount { get; init; }
        public Dictionary<int, List<ImportCellError>> CrossErrors { get; } = [];
        public Dictionary<string, Guid> CoupleIds { get; } = [];

        public void AddCrossError(int rowNumber, string column, string message)
        {
            if (!CrossErrors.TryGetValue(rowNumber, out var errors))
            {
                errors = [];
                CrossErrors[rowNumber] = errors;
            }

            errors.Add(new ImportCellError(column, message));
        }
    }
}

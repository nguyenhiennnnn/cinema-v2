using Cinema.Application.DTOs.Showtimes;
using Cinema.Application.Interfaces;
using Cinema.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Utils;

public record SeatSlot(
    int ShowtimeSeatId,
    string Row,
    int Number,
    int? SeatTypeId,
    Guid? CoupleId,
    ShowtimeSeatStatus Status,
    decimal Price
);

public static class SeatLayoutRules
{
    public static Task<List<SeatSlot>> LoadSeatSlotsAsync(
        this IAppDbContext db,
        int showtimeId,
        CancellationToken cancellationToken
    ) =>
        db
            .ShowtimeSeats.Where(ss => ss.ShowtimeId == showtimeId)
            .Select(ss => new SeatSlot(
                ss.ShowtimeSeatId,
                ss.Seat.SeatRow,
                ss.Seat.SeatNumber,
                ss.Seat.SeatTypeId,
                ss.Seat.CoupleId,
                ss.Status,
                ss.Price
            ))
            .ToListAsync(cancellationToken);

    public static List<SeatSuggestionDto> Suggest(
        IReadOnlyList<SeatSlot> seats,
        int cols,
        int count,
        int top = 3
    )
    {
        var rows = GroupRows(seats);
        var bestPerRow = new List<(SeatSuggestionDto Dto, double Score)>();

        for (var r = 0; r < rows.Count; r++)
        {
            var ordered = rows[r].OrderBy(s => s.Number).ToList();
            var coupleSizes = ordered
                .Where(s => s.CoupleId.HasValue)
                .GroupBy(s => s.CoupleId!.Value)
                .ToDictionary(g => g.Key, g => g.Count());
            var depth = (r + 1d) / rows.Count;
            var rowScore = Math.Max(0, 1 - Math.Max(Math.Max(0.5 - depth, depth - 0.7), 0) * 2);
            var bestWindow = (Window: (List<SeatSlot>?)null, Score: -1d);

            foreach (var run in SplitRuns(ordered))
            {
                for (var i = 0; i + count <= run.Count; i++)
                {
                    if (i == 1 || run.Count - i - count == 1)
                        continue;

                    var window = run.GetRange(i, count);
                    if (
                        window
                            .Where(s => s.CoupleId.HasValue)
                            .GroupBy(s => s.CoupleId!.Value)
                            .Any(g => g.Count() != coupleSizes[g.Key])
                    )
                        continue;

                    var center = (window[0].Number + window[^1].Number) / 2d;
                    var centerScore = Math.Max(
                        0,
                        1 - Math.Abs(center - (cols + 1) / 2d) / (cols / 2d)
                    );
                    var sameType = window.Select(s => s.SeatTypeId).Distinct().Count() == 1;
                    var score = 0.45 * centerScore + 0.45 * rowScore + (sameType ? 0.1 : 0);

                    if (score > bestWindow.Score)
                        bestWindow = (window, score);
                }
            }

            if (bestWindow.Window is { } w)
                bestPerRow.Add(
                    (
                        new SeatSuggestionDto(
                            w[0].Row,
                            w.Select(s => s.Number).ToList(),
                            w.Select(s => s.ShowtimeSeatId).ToList(),
                            w.Sum(s => s.Price),
                            Math.Round(bestWindow.Score, 3)
                        ),
                        bestWindow.Score
                    )
                );
        }

        return bestPerRow.OrderByDescending(x => x.Score).Take(top).Select(x => x.Dto).ToList();
    }

    public static string? ValidateSelection(
        IReadOnlyList<SeatSlot> seats,
        ICollection<int> selectedSeatIds
    )
    {
        var pickedIds = seats
            .Where(s => selectedSeatIds.Contains(s.ShowtimeSeatId))
            .Select(s => s.ShowtimeSeatId)
            .ToHashSet();
        if (pickedIds.Count == 0)
            return null;

        var pickedCoupleIds = seats
            .Where(s => pickedIds.Contains(s.ShowtimeSeatId) && s.CoupleId.HasValue)
            .Select(s => s.CoupleId!.Value)
            .ToHashSet();
        if (
            seats.Any(s =>
                s.CoupleId is { } id
                && pickedCoupleIds.Contains(id)
                && !pickedIds.Contains(s.ShowtimeSeatId)
            )
        )
            return "Ghế đôi phải được chọn cùng nhau.";

        foreach (
            var row in seats
                .GroupBy(s => s.Row)
                .Where(g => g.Any(s => pickedIds.Contains(s.ShowtimeSeatId)))
        )
        {
            var ordered = row.OrderBy(s => s.Number).ToList();
            var pickedNumbers = ordered
                .Where(s => pickedIds.Contains(s.ShowtimeSeatId))
                .Select(s => s.Number)
                .ToHashSet();
            var afterPick = ordered
                .Select(s =>
                    pickedIds.Contains(s.ShowtimeSeatId)
                        ? s with
                        {
                            Status = ShowtimeSeatStatus.Held,
                        }
                        : s
                )
                .ToList();

            foreach (var lone in SplitRuns(afterPick).Where(run => run.Count == 1))
            {
                var n = lone[0].Number;
                if (pickedNumbers.Contains(n - 1) || pickedNumbers.Contains(n + 1))
                    return $"Không thể để trống ghế lẻ {lone[0].Row}{n}. Vui lòng chọn lại để không bỏ lẻ ghế.";
            }
        }

        return null;
    }

    private static List<IGrouping<string, SeatSlot>> GroupRows(IEnumerable<SeatSlot> seats) =>
        seats
            .GroupBy(s => s.Row)
            .OrderBy(g => g.Key.Length)
            .ThenBy(g => g.Key, StringComparer.Ordinal)
            .ToList();

    private static IEnumerable<List<SeatSlot>> SplitRuns(IEnumerable<SeatSlot> sortedRow)
    {
        List<SeatSlot>? run = null;
        foreach (var seat in sortedRow)
        {
            if (seat.Status != ShowtimeSeatStatus.Available)
            {
                if (run != null)
                    yield return run;
                run = null;
                continue;
            }

            if (run != null && seat.Number != run[^1].Number + 1)
            {
                yield return run;
                run = null;
            }

            (run ??= []).Add(seat);
        }

        if (run != null)
            yield return run;
    }
}

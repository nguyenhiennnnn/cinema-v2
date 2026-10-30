using System.Text.RegularExpressions;
using Cinema.Application.DTOs.Combos;
using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Combos;

public class ComboService(IAppDbContext db) : IComboService
{
    private static readonly Regex DrinkRegex = new(
        @"(\d+)\s*(?:Coca|Pepsi|Sprite|Fanta|Nước|Trà|Cà phê)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    public async Task<List<ComboSuggestionDto>> GetComboSuggestionsAsync(
        int showtimeId,
        int seatCount,
        CancellationToken cancellationToken
    )
    {
        var showtime =
            await db
                .Showtimes.Where(s => s.ShowtimeId == showtimeId)
                .Select(s => new
                {
                    s.StartTime,
                    s.MovieId,
                    AgeCode = s.Movie.AgeRating != null ? s.Movie.AgeRating.Code : null,
                })
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy suất chiếu.");

        var combos = await db
            .Combos.Where(c => c.IsActive)
            .Select(c => new
            {
                c.ComboId,
                c.Description,
                c.Price,
            })
            .ToListAsync(cancellationToken);
        if (combos.Count == 0)
            return [];

        var genreIds = await db
            .MovieGenres.Where(mg => mg.MovieId == showtime.MovieId)
            .Select(mg => mg.GenreId)
            .ToListAsync(cancellationToken);

        var sales = await db
            .BookingCombos.Where(bc =>
                bc.Booking.Status == BookingStatus.Confirmed
                && db.MovieGenres.Any(mg =>
                    mg.MovieId == bc.Booking.Showtime.MovieId && genreIds.Contains(mg.GenreId)
                )
            )
            .GroupBy(bc => bc.ComboId)
            .Select(g => new { ComboId = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .ToDictionaryAsync(x => x.ComboId, x => x.Quantity, cancellationToken);

        var maxSales = sales.Values.DefaultIfEmpty(0).Max();
        var medianPrice = combos.Select(c => c.Price).Order().ElementAt(combos.Count / 2);
        var localHour = VietnamTime.ToLocal(showtime.StartTime).Hour;
        var lateNight = localHour >= 21 || localHour < 6;
        var kidsMovie = showtime.AgeCode is "P" or "K";

        return combos
            .Select(c =>
            {
                var size = DrinkRegex
                    .Matches(c.Description ?? string.Empty)
                    .Sum(m => int.Parse(m.Groups[1].Value));
                var sold = sales.GetValueOrDefault(c.ComboId);
                var quantity = size > 0 ? (int)Math.Ceiling(seatCount / (double)size) : 1;
                var fit =
                    size > 0 ? 3 - Math.Min(3, quantity * size - seatCount + (quantity - 1)) : 0;
                var score =
                    fit
                    + (kidsMovie && size >= 3 ? 1 : 0)
                    + (lateNight && c.Price <= medianPrice ? 0.5 : 0)
                    + (maxSales > 0 ? 2d * sold / maxSales : 0);
                var label =
                    size > 0
                        ? quantity == 1
                            ? $"Phù hợp cho {size} người"
                            : $"Phù hợp cho {quantity * size} người (x{quantity})"
                        : "Bán chạy cùng thể loại";
                return (
                    Score: score,
                    c.Price,
                    Keep: size > 0 || sold > 0,
                    Dto: new ComboSuggestionDto(
                        c.ComboId,
                        quantity,
                        label,
                        sold > 0 && sold == maxSales
                    )
                );
            })
            .Where(x => x.Keep)
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Price)
            .Take(3)
            .Select(x => x.Dto)
            .ToList();
    }

    public async Task<List<ComboDto>> GetActiveCombosAsync(CancellationToken cancellationToken) =>
        await db
            .Combos.Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ProjectToType<ComboDto>()
            .ToListAsync(cancellationToken);

    public async Task<OffsetPageResult<AdminComboDto>> GetCombosAdminAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.Combos.OrderBy(c => c.Name);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<AdminComboDto>()
            .ToListAsync(cancellationToken);
        return OffsetPageResult<AdminComboDto>.Create(items, page, pageSize, total);
    }

    public async Task<AdminComboDto> CreateComboAsync(
        UpsertComboRequest request,
        CancellationToken cancellationToken
    )
    {
        var combo = new Combo
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            IsActive = true,
        };
        db.Combos.Add(combo);
        await db.SaveChangesAsync(cancellationToken);
        return combo.Adapt<AdminComboDto>();
    }

    public async Task<AdminComboDto> UpdateComboAsync(
        int comboId,
        UpsertComboRequest request,
        CancellationToken cancellationToken
    )
    {
        var combo =
            await db.Combos.FirstOrDefaultAsync(c => c.ComboId == comboId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy combo.");

        combo.Name = request.Name;
        combo.Description = request.Description;
        combo.Price = request.Price;
        combo.ImageUrl = request.ImageUrl;
        await db.SaveChangesAsync(cancellationToken);
        return combo.Adapt<AdminComboDto>();
    }

    public async Task<ToggleComboResponse> ToggleComboAsync(
        int comboId,
        CancellationToken cancellationToken
    )
    {
        var combo =
            await db.Combos.FirstOrDefaultAsync(c => c.ComboId == comboId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy combo.");

        combo.IsActive = !combo.IsActive;
        await db.SaveChangesAsync(cancellationToken);
        return new ToggleComboResponse(comboId, combo.IsActive);
    }
}

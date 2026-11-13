using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Showtimes;

public class ShowtimeExportService(IAppDbContext db, IExportWriter exportWriter)
    : IShowtimeExportService
{
    private const string Module = "showtimes";

    public async Task<ExportFileDto> ExportShowtimesAsync(
        AdminShowtimeListFilter filter,
        ExportFormat format,
        CancellationToken cancellationToken
    )
    {
        var query = db.Showtimes.AsNoTracking().ApplyAdminFilter(filter);
        await ExportLimits.EnsureWithinLimitAsync(query, cancellationToken);

        var rows = query
            .OrderByDescending(s => s.StartTime)
            .ThenByDescending(s => s.ShowtimeId)
            .Take(ExportLimits.MaxRows)
            .Select(s => new ShowtimeExportRow(
                s.Movie.Title,
                s.Room.RoomName,
                s.Room.ScreenType == null ? null : s.Room.ScreenType.Name,
                s.StartTime,
                s.EndTime,
                s.BasePrice,
                s.Status,
                s.ShowtimeSeats.Count(ss => ss.Status == ShowtimeSeatStatus.Booked),
                s.ShowtimeSeats.Count()
            ))
            .AsAsyncEnumerable();

        return await ExportFileBuilder.BuildAsync(
            exportWriter,
            Module,
            Messages.Export_SheetShowtimes,
            format,
            Columns(),
            rows,
            cancellationToken
        );
    }

    private static IReadOnlyList<ExportColumn<ShowtimeExportRow>> Columns() =>
        [
            new(Messages.Export_ColMovieTitle, x => x.MovieTitle),
            new(Messages.Export_ColRoom, x => x.RoomName),
            new(Messages.Export_ColScreenType, x => x.ScreenType),
            new(Messages.Export_ColStartTime, x => x.StartTime, ExportColumnType.DateTime),
            new(Messages.Export_ColEndTime, x => x.EndTime, ExportColumnType.DateTime),
            new(Messages.Export_ColBasePrice, x => x.BasePrice, ExportColumnType.Money),
            new(Messages.Export_ColStatus, x => ExportLabels.Showtime(x.Status)),
            new(Messages.Export_ColSeatsSold, x => x.SeatsSold, ExportColumnType.Number),
            new(Messages.Export_ColSeatsTotal, x => x.SeatsTotal, ExportColumnType.Number),
        ];

    private sealed record ShowtimeExportRow(
        string MovieTitle,
        string RoomName,
        string? ScreenType,
        DateTime StartTime,
        DateTime EndTime,
        decimal BasePrice,
        ShowtimeStatus Status,
        int SeatsSold,
        int SeatsTotal
    );
}

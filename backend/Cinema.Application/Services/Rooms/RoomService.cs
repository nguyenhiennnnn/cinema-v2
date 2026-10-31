using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Rooms;
using Cinema.Application.Interfaces;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Rooms;

public class RoomService(IAppDbContext db) : IRoomService
{
    public async Task<OffsetPageResult<AdminRoomDto>> GetRoomsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.CinemaRooms.Include(r => r.ScreenType).OrderBy(r => r.RoomName);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<AdminRoomDto>()
            .ToListAsync(cancellationToken);

        return OffsetPageResult<AdminRoomDto>.Create(items, page, pageSize, total);
    }

    public async Task<AdminRoomDto> CreateRoomAsync(
        CreateRoomRequest request,
        CancellationToken cancellationToken
    )
    {
        var nameExists = await db.CinemaRooms.AnyAsync(
            r => r.RoomName == request.RoomName,
            cancellationToken
        );
        if (nameExists)
            throw new ConflictAppException(Messages.Conflict_RoomNameExists);

        if (
            request.ScreenTypeId is not null
            && !await db.ScreenTypes.AnyAsync(
                st => st.ScreenTypeId == request.ScreenTypeId,
                cancellationToken
            )
        )
            throw new ValidationAppException(
                Messages.Invalid_ScreenTypeNotFound,
                new Dictionary<string, string[]>
                {
                    ["screenTypeId"] = [Messages.Invalid_ScreenTypeIdNotFound],
                }
            );

        var room = new CinemaRoom
        {
            RoomName = request.RoomName,
            ScreenTypeId = request.ScreenTypeId,
            TotalSeats = 0,
            IsActive = true,
            Rows = request.Rows,
            Cols = request.Cols,
        };

        db.CinemaRooms.Add(room);
        await db.SaveChangesAsync(cancellationToken);

        return await LoadRoomDtoAsync(room.RoomId, cancellationToken);
    }

    public async Task<AdminRoomDto> UpdateRoomAsync(
        int roomId,
        UpdateRoomRequest request,
        CancellationToken cancellationToken
    )
    {
        var room =
            await db.CinemaRooms.FirstOrDefaultAsync(r => r.RoomId == roomId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Room);

        var nameExists = await db.CinemaRooms.AnyAsync(
            r => r.RoomName == request.RoomName && r.RoomId != roomId,
            cancellationToken
        );
        if (nameExists)
            throw new ConflictAppException(Messages.Conflict_RoomNameExists);

        if (
            request.ScreenTypeId is not null
            && !await db.ScreenTypes.AnyAsync(
                st => st.ScreenTypeId == request.ScreenTypeId,
                cancellationToken
            )
        )
            throw new ValidationAppException(
                Messages.Invalid_ScreenTypeIdNotFound,
                new Dictionary<string, string[]>
                {
                    ["screenTypeId"] = [Messages.Invalid_ScreenTypeIdNotFound],
                }
            );

        if (room.IsActive && !request.IsActive)
        {
            var seatIdsInRoom = db.Seats.Where(s => s.RoomId == roomId).Select(s => s.SeatId);
            if (await HasLiveFutureSeatsAsync(seatIdsInRoom, cancellationToken))
                throw new ConflictAppException(
                    Messages.Conflict_RoomHasHeldSeatsCannotDeactivate
                );
        }

        room.RoomName = request.RoomName;
        room.ScreenTypeId = request.ScreenTypeId;
        room.IsActive = request.IsActive;
        await db.SaveChangesAsync(cancellationToken);

        return await LoadRoomDtoAsync(room.RoomId, cancellationToken);
    }

    public async Task<DeleteRoomResponse> DeleteRoomAsync(
        int roomId,
        CancellationToken cancellationToken
    )
    {
        var room =
            await db.CinemaRooms.FirstOrDefaultAsync(r => r.RoomId == roomId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Room);

        var hasFutureShowtimes = await db.Showtimes.AnyAsync(
            s =>
                s.RoomId == roomId
                && s.Status == ShowtimeStatus.Scheduled
                && s.StartTime > DateTime.UtcNow,
            cancellationToken
        );
        if (hasFutureShowtimes)
            throw new BusinessRuleException(Messages.Rule_RoomHasUpcomingShowtimes);

        var hasAnyShowtimes = await db.Showtimes.AnyAsync(
            s => s.RoomId == roomId,
            cancellationToken
        );
        if (hasAnyShowtimes)
        {
            room.IsActive = false;
            await db.SaveChangesAsync(cancellationToken);
            return new DeleteRoomResponse(roomId, false, true);
        }

        db.CinemaRooms.Remove(room);
        await db.SaveChangesAsync(cancellationToken);

        return new DeleteRoomResponse(roomId, true, false);
    }

    public async Task<List<SeatDto>> GetRoomSeatsAsync(
        int roomId,
        CancellationToken cancellationToken
    )
    {
        var roomExists = await db.CinemaRooms.AnyAsync(r => r.RoomId == roomId, cancellationToken);
        if (!roomExists)
            throw new NotFoundException(Messages.NotFound_Room);

        return await db
            .Seats.Include(s => s.SeatType)
            .Where(s => s.RoomId == roomId)
            .OrderBy(s => s.SeatRow)
            .ThenBy(s => s.SeatNumber)
            .ProjectToType<SeatDto>()
            .ToListAsync(cancellationToken);
    }

    public async Task<UpdateSeatLayoutResponse> UpdateSeatLayoutAsync(
        int roomId,
        UpdateSeatLayoutRequest request,
        CancellationToken cancellationToken
    )
    {
        var room =
            await db.CinemaRooms.FirstOrDefaultAsync(r => r.RoomId == roomId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Room);

        var seatTypeIds = request
            .Seats.Where(s => s.SeatTypeId is not null)
            .Select(s => s.SeatTypeId!.Value)
            .Distinct()
            .ToList();
        if (seatTypeIds.Count > 0)
        {
            var existingCount = await db.SeatTypes.CountAsync(
                st => seatTypeIds.Contains(st.SeatTypeId),
                cancellationToken
            );
            if (existingCount != seatTypeIds.Count)
                throw new ValidationAppException(
                    Messages.Invalid_ReferenceData,
                    new Dictionary<string, string[]>
                    {
                        ["seats"] = [Messages.Invalid_SeatTypeIdsNotFound],
                    }
                );
        }

        var coupleGroups = request
            .Seats.Where(s => s.CoupleId is not null)
            .GroupBy(s => s.CoupleId!.Value)
            .ToList();
        var invalidCoupleIds = coupleGroups
            .Where(g => g.Count() != 2 || g.Select(s => s.SeatRow).Distinct().Count() != 1)
            .Select(g => g.Key)
            .ToList();
        if (invalidCoupleIds.Count > 0)
            throw new ValidationAppException(
                Messages.Invalid_CoupleSeatData,
                new Dictionary<string, string[]>
                {
                    ["seats"] = [Messages.Invalid_CoupleSeatsUnpaired],
                }
            );

        var currentSeats = await db
            .Seats.Where(s => s.RoomId == roomId)
            .ToListAsync(cancellationToken);
        var currentMap = currentSeats.ToDictionary(s => (s.SeatRow, s.SeatNumber));
        var requestMap = request.Seats.ToDictionary(s => (s.SeatRow, s.SeatNumber));

        int created = 0,
            updated = 0,
            deactivated = 0,
            removed = 0;
        var finalSeats = new List<Seat>();
        var touchedSeats = new List<Seat>();

        var seatsToDrop = currentSeats
            .Where(s => !requestMap.ContainsKey((s.SeatRow, s.SeatNumber)))
            .ToList();
        var dropIds = seatsToDrop.Select(s => s.SeatId).ToList();

        var changedIds = request
            .Seats.Where(item =>
                currentMap.TryGetValue((item.SeatRow, item.SeatNumber), out var existing)
                && (
                    existing.SeatTypeId != item.SeatTypeId
                    || existing.IsActive != item.IsActive
                    || existing.IsAisle != item.IsAisle
                    || existing.CoupleId != item.CoupleId
                )
            )
            .Select(item => currentMap[(item.SeatRow, item.SeatNumber)].SeatId)
            .Concat(dropIds)
            .ToList();
        if (
            changedIds.Count > 0
            && await HasLiveFutureSeatsAsync(
                db.Seats.Where(s => changedIds.Contains(s.SeatId)).Select(s => s.SeatId),
                cancellationToken
            )
        )
            throw new ConflictAppException(
                Messages.Conflict_SeatsHeldOrBookedCannotChange
            );
        var idsWithHistory = (
            await db
                .ShowtimeSeats.Where(ss => dropIds.Contains(ss.SeatId))
                .Select(ss => ss.SeatId)
                .Distinct()
                .ToListAsync(cancellationToken)
        ).ToHashSet();

        foreach (var seat in seatsToDrop)
        {
            if (idsWithHistory.Contains(seat.SeatId))
            {
                if (seat.IsActive)
                {
                    seat.IsActive = false;
                    deactivated++;
                }
                finalSeats.Add(seat);
                touchedSeats.Add(seat);
            }
            else
            {
                db.Seats.Remove(seat);
                removed++;
            }
        }

        foreach (var item in request.Seats)
        {
            if (currentMap.TryGetValue((item.SeatRow, item.SeatNumber), out var existing))
            {
                if (
                    existing.SeatTypeId != item.SeatTypeId
                    || existing.IsActive != item.IsActive
                    || existing.IsAisle != item.IsAisle
                    || existing.CoupleId != item.CoupleId
                )
                {
                    existing.SeatTypeId = item.SeatTypeId;
                    existing.IsActive = item.IsActive;
                    existing.IsAisle = item.IsAisle;
                    existing.CoupleId = item.CoupleId;
                    updated++;
                    touchedSeats.Add(existing);
                }
                finalSeats.Add(existing);
            }
            else
            {
                var newSeat = new Seat
                {
                    RoomId = roomId,
                    SeatRow = item.SeatRow,
                    SeatNumber = item.SeatNumber,
                    SeatTypeId = item.SeatTypeId,
                    IsActive = item.IsActive,
                    IsAisle = item.IsAisle,
                    CoupleId = item.CoupleId,
                };
                db.Seats.Add(newSeat);
                created++;
                finalSeats.Add(newSeat);
                touchedSeats.Add(newSeat);
            }
        }

        room.TotalSeats = finalSeats.Count(s => s.IsActive && !s.IsAisle);
        room.Rows = request.Rows;
        room.Cols = request.Cols;

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        await SyncFutureShowtimeSeatsAsync(roomId, touchedSeats, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new UpdateSeatLayoutResponse(
            created,
            updated,
            deactivated,
            removed,
            room.TotalSeats
        );
    }

    public async Task<AdminRoomDto> GetRoomByIdAsync(
        int roomId,
        CancellationToken cancellationToken
    )
    {
        var room =
            await db
                .CinemaRooms.Include(r => r.ScreenType)
                .FirstOrDefaultAsync(r => r.RoomId == roomId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Room);

        return room.Adapt<AdminRoomDto>();
    }

    private async Task<bool> HasLiveFutureSeatsAsync(
        IQueryable<int> seatIds,
        CancellationToken cancellationToken
    )
    {
        var now = DateTime.UtcNow;
        return await db.ShowtimeSeats.AnyAsync(
            ss =>
                seatIds.Contains(ss.SeatId)
                && (ss.Status == ShowtimeSeatStatus.Booked || ss.Status == ShowtimeSeatStatus.Held)
                && ss.Showtime.Status == ShowtimeStatus.Scheduled
                && ss.Showtime.StartTime > now,
            cancellationToken
        );
    }

    private async Task SyncFutureShowtimeSeatsAsync(
        int roomId,
        List<Seat> touchedSeats,
        CancellationToken cancellationToken
    )
    {
        if (touchedSeats.Count == 0)
            return;

        var now = DateTime.UtcNow;
        var showtimes = await db
            .Showtimes.Where(s =>
                s.RoomId == roomId && s.Status == ShowtimeStatus.Scheduled && s.StartTime > now
            )
            .Select(s => new { s.ShowtimeId, s.BasePrice })
            .ToListAsync(cancellationToken);
        if (showtimes.Count == 0)
            return;

        var showtimeIds = showtimes.Select(s => s.ShowtimeId).ToList();
        var seatIds = touchedSeats.Select(s => s.SeatId).ToList();
        var seatTypeIds = touchedSeats
            .Where(s => s.SeatTypeId is not null)
            .Select(s => s.SeatTypeId!.Value)
            .Distinct()
            .ToList();
        var multipliers = await db
            .SeatTypes.Where(st => seatTypeIds.Contains(st.SeatTypeId))
            .ToDictionaryAsync(st => st.SeatTypeId, st => st.PriceMultiplier, cancellationToken);
        var existing = await db
            .ShowtimeSeats.Where(ss => showtimeIds.Contains(ss.ShowtimeId) && seatIds.Contains(ss.SeatId))
            .ToListAsync(cancellationToken);
        var existingMap = existing.ToDictionary(ss => (ss.ShowtimeId, ss.SeatId));

        foreach (var showtime in showtimes)
        foreach (var seat in touchedSeats)
        {
            var eligible = seat.IsActive && !seat.IsAisle;
            var multiplier =
                seat.SeatTypeId is not null && multipliers.TryGetValue(seat.SeatTypeId.Value, out var m)
                    ? m
                    : 1.0m;
            if (existingMap.TryGetValue((showtime.ShowtimeId, seat.SeatId), out var showtimeSeat))
            {
                if (showtimeSeat.Status != ShowtimeSeatStatus.Available)
                    continue;
                if (eligible)
                    showtimeSeat.Price = showtime.BasePrice * multiplier;
                else
                    db.ShowtimeSeats.Remove(showtimeSeat);
            }
            else if (eligible)
            {
                db.ShowtimeSeats.Add(
                    new ShowtimeSeat
                    {
                        ShowtimeId = showtime.ShowtimeId,
                        SeatId = seat.SeatId,
                        Status = ShowtimeSeatStatus.Available,
                        Price = showtime.BasePrice * multiplier,
                    }
                );
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private Task<AdminRoomDto> LoadRoomDtoAsync(
        int roomId,
        CancellationToken cancellationToken
    ) => GetRoomByIdAsync(roomId, cancellationToken);
}

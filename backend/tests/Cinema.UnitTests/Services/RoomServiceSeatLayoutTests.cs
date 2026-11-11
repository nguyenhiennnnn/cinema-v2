using Cinema.Application.DTOs.Rooms;
using Cinema.Application.Services.Rooms;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class RoomServiceSeatLayoutTests : IDisposable
{
    private readonly SqliteAppDbContextHandle _handle = new();

    private RoomService CreateSut()
    {
        _handle.Db.ChangeTracker.Clear();
        return new RoomService(_handle.Db);
    }

    private (CinemaRoom Room, List<Seat> Seats, Showtime Showtime) Seed(DateTime? startTime = null)
    {
        var db = _handle.Db;
        var room = TestDataSeeder.AddRoom(db);
        var seats = TestDataSeeder.AddSeats(db, room.RoomId, count: 2);
        var movie = TestDataSeeder.AddMovie(db);
        var showtime = TestDataSeeder.AddShowtime(db, movie, room, seats, startTime);
        return (room, seats, showtime);
    }

    private async Task SetStatusAsync(int showtimeId, int seatId, ShowtimeSeatStatus status)
    {
        var ss = await _handle.Db.ShowtimeSeats.SingleAsync(x =>
            x.ShowtimeId == showtimeId && x.SeatId == seatId
        );
        ss.Status = status;
        await _handle.Db.SaveChangesAsync();
    }

    private static UpdateSeatLayoutRequest Layout(
        IEnumerable<Seat> seats,
        Func<Seat, SeatLayoutItem?>? change = null,
        params SeatLayoutItem[] extra
    )
    {
        var items = seats
            .Select(s =>
                change?.Invoke(s)
                ?? new SeatLayoutItem(
                    s.SeatRow,
                    s.SeatNumber,
                    s.SeatTypeId,
                    s.IsActive,
                    s.IsAisle,
                    s.CoupleId
                )
            )
            .Concat(extra)
            .ToList();
        return new UpdateSeatLayoutRequest(1, items.Count, items);
    }

    [Fact]
    public async Task UpdateSeatLayoutAsync_ChangingSeatTypeOfBookedSeatInFutureShowtime_ThrowsConflict()
    {
        var (room, seats, showtime) = Seed();
        await SetStatusAsync(showtime.ShowtimeId, seats[0].SeatId, ShowtimeSeatStatus.Booked);
        var request = Layout(
            seats,
            s =>
                s.SeatId == seats[0].SeatId
                    ? new SeatLayoutItem(
                        s.SeatRow,
                        s.SeatNumber,
                        TestDataSeeder.VipSeatTypeId,
                        true,
                        false,
                        null
                    )
                    : null
        );

        await Should.ThrowAsync<ConflictAppException>(() =>
            CreateSut().UpdateSeatLayoutAsync(room.RoomId, request, CancellationToken.None)
        );
    }

    [Fact]
    public async Task UpdateSeatLayoutAsync_DeactivatingHeldSeatInFutureShowtime_ThrowsConflict()
    {
        var (room, seats, showtime) = Seed();
        await SetStatusAsync(showtime.ShowtimeId, seats[1].SeatId, ShowtimeSeatStatus.Held);
        var request = Layout(
            seats,
            s =>
                s.SeatId == seats[1].SeatId
                    ? new SeatLayoutItem(s.SeatRow, s.SeatNumber, s.SeatTypeId, false, false, null)
                    : null
        );

        await Should.ThrowAsync<ConflictAppException>(() =>
            CreateSut().UpdateSeatLayoutAsync(room.RoomId, request, CancellationToken.None)
        );
    }

    [Fact]
    public async Task UpdateSeatLayoutAsync_BookedSeatOnlyInPastShowtime_AllowsChange()
    {
        var (room, seats, showtime) = Seed(DateTime.UtcNow.AddDays(-2));
        await SetStatusAsync(showtime.ShowtimeId, seats[0].SeatId, ShowtimeSeatStatus.Booked);
        var request = Layout(
            seats,
            s =>
                s.SeatId == seats[0].SeatId
                    ? new SeatLayoutItem(
                        s.SeatRow,
                        s.SeatNumber,
                        TestDataSeeder.VipSeatTypeId,
                        true,
                        false,
                        null
                    )
                    : null
        );

        var result = await CreateSut()
            .UpdateSeatLayoutAsync(room.RoomId, request, CancellationToken.None);

        result.Updated.ShouldBe(1);
    }

    [Fact]
    public async Task UpdateSeatLayoutAsync_ChangingSeatTypeOfAvailableSeat_RepricesFutureShowtimeSeat()
    {
        var (room, seats, showtime) = Seed();
        var request = Layout(
            seats,
            s =>
                s.SeatId == seats[1].SeatId
                    ? new SeatLayoutItem(
                        s.SeatRow,
                        s.SeatNumber,
                        TestDataSeeder.VipSeatTypeId,
                        true,
                        false,
                        null
                    )
                    : null
        );

        await CreateSut().UpdateSeatLayoutAsync(room.RoomId, request, CancellationToken.None);

        var ss = await _handle
            .Db.ShowtimeSeats.AsNoTracking()
            .SingleAsync(x => x.ShowtimeId == showtime.ShowtimeId && x.SeatId == seats[1].SeatId);
        ss.Price.ShouldBe(showtime.BasePrice * 1.5m);
    }

    [Fact]
    public async Task UpdateSeatLayoutAsync_AddingSeat_CreatesAvailableShowtimeSeatForFutureShowtime()
    {
        var (room, seats, showtime) = Seed();
        var request = Layout(
            seats,
            null,
            new SeatLayoutItem("A", 3, TestDataSeeder.VipSeatTypeId, true, false, null)
        );

        await CreateSut().UpdateSeatLayoutAsync(room.RoomId, request, CancellationToken.None);

        var newSeat = await _handle
            .Db.Seats.AsNoTracking()
            .SingleAsync(s => s.RoomId == room.RoomId && s.SeatNumber == 3);
        var ss = await _handle
            .Db.ShowtimeSeats.AsNoTracking()
            .SingleAsync(x => x.ShowtimeId == showtime.ShowtimeId && x.SeatId == newSeat.SeatId);
        ss.Status.ShouldBe(ShowtimeSeatStatus.Available);
        ss.Price.ShouldBe(showtime.BasePrice * 1.5m);
    }

    [Fact]
    public async Task UpdateSeatLayoutAsync_DeactivatingAvailableSeat_RemovesItFromFutureShowtime()
    {
        var (room, seats, showtime) = Seed();
        var request = Layout(
            seats,
            s =>
                s.SeatId == seats[1].SeatId
                    ? new SeatLayoutItem(s.SeatRow, s.SeatNumber, s.SeatTypeId, false, false, null)
                    : null
        );

        await CreateSut().UpdateSeatLayoutAsync(room.RoomId, request, CancellationToken.None);

        (
            await _handle.Db.ShowtimeSeats.CountAsync(x =>
                x.ShowtimeId == showtime.ShowtimeId && x.SeatId == seats[1].SeatId
            )
        ).ShouldBe(0);
        (
            await _handle.Db.ShowtimeSeats.CountAsync(x => x.ShowtimeId == showtime.ShowtimeId)
        ).ShouldBe(1);
    }

    [Fact]
    public async Task UpdateRoomAsync_DeactivatingRoomWithBookedSeatInFutureShowtime_ThrowsConflict()
    {
        var (room, seats, showtime) = Seed();
        await SetStatusAsync(showtime.ShowtimeId, seats[0].SeatId, ShowtimeSeatStatus.Booked);

        await Should.ThrowAsync<ConflictAppException>(() =>
            CreateSut()
                .UpdateRoomAsync(
                    room.RoomId,
                    new UpdateRoomRequest(room.RoomName, room.ScreenTypeId, false),
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public async Task UpdateRoomAsync_DeactivatingRoomWithoutLiveSeats_Succeeds()
    {
        var (room, _, _) = Seed();

        var result = await CreateSut()
            .UpdateRoomAsync(
                room.RoomId,
                new UpdateRoomRequest(room.RoomName, room.ScreenTypeId, false),
                CancellationToken.None
            );

        result.IsActive.ShouldBeFalse();
    }

    public void Dispose() => _handle.Dispose();
}

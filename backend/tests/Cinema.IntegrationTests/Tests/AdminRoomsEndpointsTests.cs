using System.Net;
using System.Net.Http.Json;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Infrastructure.Persistence;
using Cinema.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cinema.IntegrationTests.Tests;

public class AdminRoomsEndpointsTests(CinemaApiFactory factory) : IntegrationTestBase(factory)
{
    private record AdminRoomDto(
        int RoomId,
        string RoomName,
        int? ScreenTypeId,
        string? ScreenTypeName,
        int TotalSeats,
        bool IsActive,
        int Rows,
        int Cols
    );

    private record UpdateSeatLayoutResponseDto(
        int Created,
        int Updated,
        int Deactivated,
        int Removed,
        int TotalSeats
    );

    private record SeatDto(
        int SeatId,
        string SeatRow,
        int SeatNumber,
        int? SeatTypeId,
        string? SeatTypeName,
        bool IsActive,
        bool IsAisle,
        Guid? CoupleId
    );

    private async Task<HttpClient> LoginAsAdminAsync()
    {
        var (_, _, email, password) = await IntegrationTestDataSeeder.CreateEmployeeAsync(
            Factory.Services
        );
        var token = await LoginAsync(email, password, admin: true);
        return CreateAuthenticatedClient(token);
    }

    [Fact]
    public async Task CreateRoom_ValidRequest_Returns201AndPersistsRoomWithZeroSeats()
    {
        using var client = await LoginAsAdminAsync();
        var roomName = $"Room-{Guid.NewGuid():N}";

        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/rooms",
            new
            {
                roomName,
                screenTypeId = (int?)null,
                rows = 5,
                cols = 8,
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var room = await ReadDataAsync<AdminRoomDto>(response);
        room.RoomName.ShouldBe(roomName);
        room.TotalSeats.ShouldBe(0);
        room.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task CreateRoom_DuplicateName_Returns409()
    {
        using var client = await LoginAsAdminAsync();
        var roomName = $"Room-{Guid.NewGuid():N}";
        await client.PostAsJsonAsync(
            "/api/v1/admin/rooms",
            new
            {
                roomName,
                screenTypeId = (int?)null,
                rows = 5,
                cols = 8,
            }
        );

        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/rooms",
            new
            {
                roomName,
                screenTypeId = (int?)null,
                rows = 5,
                cols = 8,
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateSeatLayout_FreshRoom_CreatesAllNonAisleActiveSeatsAndComputesTotal()
    {
        using var client = await LoginAsAdminAsync();
        var roomId = await CreateRoomAsync(client);

        var response = await client.PutAsJsonAsync(
            $"/api/v1/admin/rooms/{roomId}/seats",
            new
            {
                rows = 1,
                cols = 3,
                seats = new[]
                {
                    new
                    {
                        seatRow = "A",
                        seatNumber = 1,
                        seatTypeId = (int?)1,
                        isActive = true,
                        isAisle = false,
                        coupleId = (Guid?)null,
                    },
                    new
                    {
                        seatRow = "A",
                        seatNumber = 2,
                        seatTypeId = (int?)null,
                        isActive = true,
                        isAisle = true,
                        coupleId = (Guid?)null,
                    },
                    new
                    {
                        seatRow = "A",
                        seatNumber = 3,
                        seatTypeId = (int?)2,
                        isActive = true,
                        isAisle = false,
                        coupleId = (Guid?)null,
                    },
                },
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await ReadDataAsync<UpdateSeatLayoutResponseDto>(response);
        result.Created.ShouldBe(3);
        result.TotalSeats.ShouldBe(2);

        var seatsResponse = await client.GetAsync($"/api/v1/admin/rooms/{roomId}/seats");
        var seats = await ReadDataAsync<List<SeatDto>>(seatsResponse);
        seats.Count.ShouldBe(3);
    }

    [Fact]
    public async Task UpdateSeatLayout_RemovingASeatWithNoBookingHistory_DeletesIt()
    {
        using var client = await LoginAsAdminAsync();
        var roomId = await CreateRoomAsync(client);
        await PutSeatLayoutAsync(client, roomId, ("A", 1, 1), ("A", 2, 1));

        var result = await PutSeatLayoutAsync(client, roomId, ("A", 1, 1));

        result.Removed.ShouldBe(1);
        result.TotalSeats.ShouldBe(1);
    }

    [Fact]
    public async Task UpdateSeatLayout_RemovingASeatThatHasBookingHistory_DeactivatesInsteadOfDeleting()
    {
        using var client = await LoginAsAdminAsync();
        var roomId = await CreateRoomAsync(client);
        await PutSeatLayoutAsync(client, roomId, ("A", 1, 1), ("A", 2, 1));

        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var seatA2 = await db.Seats.FirstAsync(s =>
                s.RoomId == roomId && s.SeatRow == "A" && s.SeatNumber == 2
            );
            var movie = await db.Movies.FirstAsync(m => m.Status == MovieStatus.NowShowing);
            var start = DateTime.UtcNow.Date.AddDays(2).AddHours(14);
            var showtime = new Showtime
            {
                MovieId = movie.MovieId,
                RoomId = roomId,
                StartTime = start,
                EndTime = start.AddMinutes(movie.DurationMinutes + 15),
                BasePrice = 100_000m,
                Status = ShowtimeStatus.Scheduled,
                CreatedAt = DateTime.UtcNow,
            };
            db.Showtimes.Add(showtime);
            await db.SaveChangesAsync();
            db.ShowtimeSeats.Add(
                new ShowtimeSeat
                {
                    ShowtimeId = showtime.ShowtimeId,
                    SeatId = seatA2.SeatId,
                    Status = ShowtimeSeatStatus.Available,
                    Price = 100_000m,
                }
            );
            await db.SaveChangesAsync();
        }

        var result = await PutSeatLayoutAsync(client, roomId, ("A", 1, 1));

        result.Removed.ShouldBe(0);
        result.Deactivated.ShouldBe(1);
        var seatsResponse = await client.GetAsync($"/api/v1/admin/rooms/{roomId}/seats");
        var seats = await ReadDataAsync<List<SeatDto>>(seatsResponse);
        seats.ShouldContain(s => s.SeatRow == "A" && s.SeatNumber == 2 && !s.IsActive);
    }

    [Fact]
    public async Task UpdateSeatLayout_CoupleSeatMissingItsPair_Returns400()
    {
        using var client = await LoginAsAdminAsync();
        var roomId = await CreateRoomAsync(client);
        var coupleId = Guid.NewGuid();

        var response = await client.PutAsJsonAsync(
            $"/api/v1/admin/rooms/{roomId}/seats",
            new
            {
                rows = 1,
                cols = 1,
                seats = new[]
                {
                    new
                    {
                        seatRow = "A",
                        seatNumber = 1,
                        seatTypeId = 3,
                        isActive = true,
                        isAisle = false,
                        coupleId,
                    },
                },
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteRoom_RoomWithNoShowtimes_HardDeletesIt()
    {
        using var client = await LoginAsAdminAsync();
        var roomId = await CreateRoomAsync(client);

        var response = await client.DeleteAsync($"/api/v1/admin/rooms/{roomId}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var getResponse = await client.GetAsync($"/api/v1/admin/rooms/{roomId}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteRoom_RoomWithFutureScheduledShowtime_Returns422AndKeepsRoom()
    {
        using var client = await LoginAsAdminAsync();
        var roomId = await CreateRoomAsync(client);
        await PutSeatLayoutAsync(client, roomId, ("A", 1, 1));
        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var movie = await db.Movies.FirstAsync(m => m.Status == MovieStatus.NowShowing);
            var start = DateTime.UtcNow.Date.AddDays(2).AddHours(14);
            db.Showtimes.Add(
                new Showtime
                {
                    MovieId = movie.MovieId,
                    RoomId = roomId,
                    StartTime = start,
                    EndTime = start.AddMinutes(movie.DurationMinutes + 15),
                    BasePrice = 100_000m,
                    Status = ShowtimeStatus.Scheduled,
                    CreatedAt = DateTime.UtcNow,
                }
            );
            await db.SaveChangesAsync();
        }

        var response = await client.DeleteAsync($"/api/v1/admin/rooms/{roomId}");

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        var getResponse = await client.GetAsync($"/api/v1/admin/rooms/{roomId}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    private static async Task<int> CreateRoomAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/rooms",
            new
            {
                roomName = $"Room-{Guid.NewGuid():N}",
                screenTypeId = (int?)null,
                rows = 5,
                cols = 8,
            }
        );
        var room = await ReadDataAsync<AdminRoomDto>(response);
        return room.RoomId;
    }

    private async Task<UpdateSeatLayoutResponseDto> PutSeatLayoutAsync(
        HttpClient client,
        int roomId,
        params (string Row, int Number, int SeatTypeId)[] seats
    )
    {
        var response = await client.PutAsJsonAsync(
            $"/api/v1/admin/rooms/{roomId}/seats",
            new
            {
                rows = 1,
                cols = seats.Length,
                seats = seats.Select(s => new
                {
                    seatRow = s.Row,
                    seatNumber = s.Number,
                    seatTypeId = s.SeatTypeId,
                    isActive = true,
                    isAisle = false,
                    coupleId = (Guid?)null,
                }),
            }
        );
        return await ReadDataAsync<UpdateSeatLayoutResponseDto>(response);
    }
}

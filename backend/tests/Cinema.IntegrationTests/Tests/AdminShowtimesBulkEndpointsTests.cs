using System.Net;
using System.Net.Http.Json;
using Cinema.Domain.Enums;
using Cinema.Infrastructure.Persistence;
using Cinema.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cinema.IntegrationTests.Tests;

public class AdminShowtimesBulkEndpointsTests(CinemaApiFactory factory)
    : IntegrationTestBase(factory)
{
    private const int SeededActiveRoomId = 1;

    private record BulkCreatedItemDto(int ShowtimeId, DateTime StartTime, int SeatsGenerated);

    private record BulkSkippedItemDto(string Date, string Time, string Reason);

    private record BulkCreateResponseDto(
        List<BulkCreatedItemDto> Created,
        List<BulkSkippedItemDto> Skipped,
        int TotalCreated,
        int TotalSkipped
    );

    private record BulkDeleteResultItemDto(int ShowtimeId, bool Success, string? Reason);

    private record BulkDeleteResponseDto(
        List<BulkDeleteResultItemDto> Results,
        int TotalSuccess,
        int TotalFailed
    );

    private async Task<HttpClient> LoginAsAdminAsync()
    {
        var (_, _, email, password) = await IntegrationTestDataSeeder.CreateEmployeeAsync(
            Factory.Services
        );
        var token = await LoginAsync(email, password, admin: true);
        return CreateAuthenticatedClient(token);
    }

    private async Task<int> GetNowShowingMovieIdAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var movie = await db.Movies.FirstAsync(m => m.Status == MovieStatus.NowShowing);
        return movie.MovieId;
    }

    private static object BuildBulkCreatePayload(
        int movieId,
        int roomId,
        DateOnly from,
        DateOnly to,
        string[] daysOfWeek,
        string[] times,
        decimal basePrice = 100_000m
    ) =>
        new
        {
            movieId,
            roomId,
            dateRange = new { from = from.ToString("yyyy-MM-dd"), to = to.ToString("yyyy-MM-dd") },
            daysOfWeek,
            times,
            basePrice,
        };

    [Fact]
    public async Task BulkCreate_SingleMatchingDayAndTime_CreatesOneShowtimeWithAllRoomSeatsGenerated()
    {
        using var client = await LoginAsAdminAsync();
        var movieId = await GetNowShowingMovieIdAsync();
        var targetDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(5));

        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/showtimes/bulk",
            BuildBulkCreatePayload(
                movieId,
                SeededActiveRoomId,
                targetDate,
                targetDate,
                [targetDate.DayOfWeek.ToString()[..3]],
                ["10:00"]
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var result = await ReadDataAsync<BulkCreateResponseDto>(response);
        result.TotalCreated.ShouldBe(1);
        result.TotalSkipped.ShouldBe(0);
        result.Created[0].SeatsGenerated.ShouldBe(50);
        Cinema.Application.Utils.VietnamTime.ToLocal(result.Created[0].StartTime).Hour.ShouldBe(10);
    }

    [Fact]
    public async Task BulkCreate_DaysOfWeekNotMatchingDateRange_CreatesNothing()
    {
        using var client = await LoginAsAdminAsync();
        var movieId = await GetNowShowingMovieIdAsync();
        var targetDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(6));
        var nonMatchingDay = targetDate.AddDays(1).DayOfWeek.ToString()[..3];

        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/showtimes/bulk",
            BuildBulkCreatePayload(
                movieId,
                SeededActiveRoomId,
                targetDate,
                targetDate,
                [nonMatchingDay],
                ["11:00"]
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var result = await ReadDataAsync<BulkCreateResponseDto>(response);
        result.TotalCreated.ShouldBe(0);
        result.TotalSkipped.ShouldBe(0);
        result.Created.ShouldBeEmpty();
    }

    [Fact]
    public async Task BulkCreate_SlotOverlapsAnAlreadyCreatedShowtime_SkipsTheDuplicateWithReason()
    {
        using var client = await LoginAsAdminAsync();
        var movieId = await GetNowShowingMovieIdAsync();
        var targetDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(7));
        var payload = BuildBulkCreatePayload(
            movieId,
            SeededActiveRoomId,
            targetDate,
            targetDate,
            [targetDate.DayOfWeek.ToString()[..3]],
            ["14:00"]
        );

        var firstResponse = await client.PostAsJsonAsync("/api/v1/admin/showtimes/bulk", payload);
        var firstResult = await ReadDataAsync<BulkCreateResponseDto>(firstResponse);
        firstResult.TotalCreated.ShouldBe(1);

        var secondResponse = await client.PostAsJsonAsync("/api/v1/admin/showtimes/bulk", payload);
        var secondResult = await ReadDataAsync<BulkCreateResponseDto>(secondResponse);

        secondResult.TotalCreated.ShouldBe(0);
        secondResult.TotalSkipped.ShouldBe(1);
        secondResult.Skipped[0].Reason.ShouldContain("Trùng khung giờ");
    }

    [Fact]
    public async Task BulkCreate_NonExistentMovieId_Returns400()
    {
        using var client = await LoginAsAdminAsync();
        var targetDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(8));

        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/showtimes/bulk",
            BuildBulkCreatePayload(
                movieId: 999_999,
                SeededActiveRoomId,
                targetDate,
                targetDate,
                [targetDate.DayOfWeek.ToString()[..3]],
                ["09:00"]
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var envelope = await ReadEnvelopeAsync(response);
        envelope.GetProperty("errors").TryGetProperty("movieId", out _).ShouldBeTrue();
    }

    [Fact]
    public async Task BulkCreate_NonExistentRoomId_Returns400()
    {
        using var client = await LoginAsAdminAsync();
        var movieId = await GetNowShowingMovieIdAsync();
        var targetDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(9));

        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/showtimes/bulk",
            BuildBulkCreatePayload(
                movieId,
                roomId: 999_999,
                targetDate,
                targetDate,
                [targetDate.DayOfWeek.ToString()[..3]],
                ["09:00"]
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var envelope = await ReadEnvelopeAsync(response);
        envelope.GetProperty("errors").TryGetProperty("roomId", out _).ShouldBeTrue();
    }

    [Fact]
    public async Task BulkDelete_MixOfExistingAndNonExistentIds_ReturnsPartialSuccessForEach()
    {
        using var client = await LoginAsAdminAsync();
        var movieId = await GetNowShowingMovieIdAsync();
        var targetDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(10));
        var createResponse = await client.PostAsJsonAsync(
            "/api/v1/admin/showtimes/bulk",
            BuildBulkCreatePayload(
                movieId,
                SeededActiveRoomId,
                targetDate,
                targetDate,
                [targetDate.DayOfWeek.ToString()[..3]],
                ["16:00"]
            )
        );
        var created = await ReadDataAsync<BulkCreateResponseDto>(createResponse);
        var createdShowtimeId = created.Created[0].ShowtimeId;

        var deleteResponse = await client.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, "/api/v1/admin/showtimes/bulk")
            {
                Content = JsonContent.Create(
                    new { showtimeIds = new[] { createdShowtimeId, 999_999 } }
                ),
            }
        );

        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await ReadDataAsync<BulkDeleteResponseDto>(deleteResponse);
        result.TotalSuccess.ShouldBe(1);
        result.TotalFailed.ShouldBe(1);
        result.Results.ShouldContain(r => r.ShowtimeId == createdShowtimeId && r.Success);
        result.Results.ShouldContain(r => r.ShowtimeId == 999_999 && !r.Success);

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var showtime = await db.Showtimes.SingleAsync(s => s.ShowtimeId == createdShowtimeId);
        showtime.Status.ShouldBe(ShowtimeStatus.Cancelled);
    }
}

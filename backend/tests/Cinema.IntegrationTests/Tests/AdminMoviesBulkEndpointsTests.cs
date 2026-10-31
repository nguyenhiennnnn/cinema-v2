using System.Net;
using System.Net.Http.Json;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Infrastructure.Persistence;
using Cinema.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cinema.IntegrationTests.Tests;

public class AdminMoviesBulkEndpointsTests(CinemaApiFactory factory)
    : IntegrationTestBase(factory)
{
    private record BulkDeleteResultItemDto(int MovieId, bool Success, string? Reason);

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

    private static Movie BuildMovie(string title = "Test Movie") =>
        new()
        {
            Title = title,
            DurationMinutes = 120,
            Status = MovieStatus.NowShowing,
            CreatedAt = DateTime.UtcNow,
        };

    private async Task<int> SeedMovieWithoutShowtimesAsync(string title = "Test Movie")
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var movie = BuildMovie(title);
        db.Movies.Add(movie);
        await db.SaveChangesAsync();
        return movie.MovieId;
    }

    [Fact]
    public async Task BulkDelete_ExistingMoviesNoShowtimes_DeletesAllAndReturnsSuccess()
    {
        using var client = await LoginAsAdminAsync();
        var id1 = await SeedMovieWithoutShowtimesAsync("Bulk Movie A");
        var id2 = await SeedMovieWithoutShowtimesAsync("Bulk Movie B");

        var response = await client.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, "/api/v1/admin/movies/bulk")
            {
                Content = JsonContent.Create(new { movieIds = new[] { id1, id2 } }),
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await ReadDataAsync<BulkDeleteResponseDto>(response);
        result.TotalSuccess.ShouldBe(2);
        result.TotalFailed.ShouldBe(0);
        result.Results.ShouldAllBe(r => r.Success);
    }

    [Fact]
    public async Task BulkDelete_SomeNotFound_ReturnsPartialSuccess()
    {
        using var client = await LoginAsAdminAsync();
        var existingId = await SeedMovieWithoutShowtimesAsync("Partial Delete Movie");
        const int nonExistentId = 999_999;

        var response = await client.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, "/api/v1/admin/movies/bulk")
            {
                Content = JsonContent.Create(new { movieIds = new[] { existingId, nonExistentId } }),
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await ReadDataAsync<BulkDeleteResponseDto>(response);
        result.TotalSuccess.ShouldBe(1);
        result.TotalFailed.ShouldBe(1);
        result.Results.ShouldContain(r => r.MovieId == existingId && r.Success);
        result.Results.ShouldContain(r => r.MovieId == nonExistentId && !r.Success);
    }

    [Fact]
    public async Task BulkDelete_ExceedsLimit_ReturnsBadRequest()
    {
        using var client = await LoginAsAdminAsync();
        // Default BulkDeleteLimit = 50; send 51 ids
        var ids = Enumerable.Range(1, 51).ToArray();

        var response = await client.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, "/api/v1/admin/movies/bulk")
            {
                Content = JsonContent.Create(new { movieIds = ids }),
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}

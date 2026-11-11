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

public class AdminReviewsBulkEndpointsTests(CinemaApiFactory factory) : IntegrationTestBase(factory)
{
    private record BulkDeleteResultItemDto(int ReviewId, bool Success, string? Reason);

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

    private async Task<(int MovieId, int ReviewId)> SeedMovieAndReviewAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var movie = new Movie
        {
            Title = $"Review Test Movie {Guid.NewGuid():N}",
            DurationMinutes = 90,
            Status = MovieStatus.NowShowing,
            CreatedAt = DateTime.UtcNow,
        };
        db.Movies.Add(movie);
        await db.SaveChangesAsync();

        var customer = new User
        {
            FullName = "Reviewer",
            Email = $"reviewer-{Guid.NewGuid():N}@test.local",
            PasswordHash = "irrelevant",
            RoleId = IntegrationTestDataSeeder.CustomerRoleId,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
        };
        db.Users.Add(customer);
        await db.SaveChangesAsync();

        var review = new Review
        {
            MovieId = movie.MovieId,
            UserId = customer.UserId,
            Rating = 4,
            Comment = "Great movie",
            CreatedAt = DateTime.UtcNow,
        };
        db.Reviews.Add(review);
        await db.SaveChangesAsync();

        return (movie.MovieId, review.ReviewId);
    }

    [Fact]
    public async Task BulkDelete_ExistingReviews_DeletesAndRecalculatesRating()
    {
        using var client = await LoginAsAdminAsync();
        var (movieId, reviewId) = await SeedMovieAndReviewAsync();

        var response = await client.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, "/api/v1/admin/reviews/bulk")
            {
                Content = JsonContent.Create(new { reviewIds = new[] { reviewId } }),
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await ReadDataAsync<BulkDeleteResponseDto>(response);
        result.TotalSuccess.ShouldBe(1);
        result.TotalFailed.ShouldBe(0);

        // Rating should be recalculated to 0 (no reviews left)
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var movie = await db.Movies.SingleAsync(m => m.MovieId == movieId);
        movie.AverageRating.ShouldBe(0m);
    }

    [Fact]
    public async Task BulkDelete_SomeNotFound_ReturnsPartialSuccess()
    {
        using var client = await LoginAsAdminAsync();
        var (_, reviewId) = await SeedMovieAndReviewAsync();
        const int nonExistentId = 999_999;

        var response = await client.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, "/api/v1/admin/reviews/bulk")
            {
                Content = JsonContent.Create(new { reviewIds = new[] { reviewId, nonExistentId } }),
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await ReadDataAsync<BulkDeleteResponseDto>(response);
        result.TotalSuccess.ShouldBe(1);
        result.TotalFailed.ShouldBe(1);
        result.Results.ShouldContain(r => r.ReviewId == reviewId && r.Success);
        result.Results.ShouldContain(r => r.ReviewId == nonExistentId && !r.Success);
    }

    [Fact]
    public async Task BulkDelete_ExceedsLimit_ReturnsBadRequest()
    {
        using var client = await LoginAsAdminAsync();
        var ids = Enumerable.Range(1, 51).ToArray();

        var response = await client.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, "/api/v1/admin/reviews/bulk")
            {
                Content = JsonContent.Create(new { reviewIds = ids }),
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}

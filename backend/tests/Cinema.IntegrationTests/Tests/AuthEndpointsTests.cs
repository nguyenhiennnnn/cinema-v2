using System.Net;
using System.Net.Http.Json;
using Cinema.Domain.Enums;
using Cinema.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cinema.IntegrationTests.Tests;

public class AuthEndpointsTests(CinemaApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Register_NewEmail_Returns201AndPersistsUnconfirmedUser()
    {
        var email = $"new-{Guid.NewGuid():N}@test.local";

        var response = await Client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                fullName = "Nguyen Van A",
                email,
                password = "Str0ngPass",
                phone = (string?)null,
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var envelope = await ReadEnvelopeAsync(response);
        envelope.GetProperty("success").GetBoolean().ShouldBeTrue();
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns400WithEmailValidationError()
    {
        var (_, email, _) = await IntegrationTestDataSeeder.CreateConfirmedCustomerAsync(
            Factory.Services
        );

        var response = await Client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                fullName = "Nguyen Van B",
                email,
                password = "Str0ngPass",
                phone = (string?)null,
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var envelope = await ReadEnvelopeAsync(response);
        envelope.GetProperty("errors").TryGetProperty("email", out _).ShouldBeTrue();
    }

    [Fact]
    public async Task Login_ConfirmedCustomerWithCorrectPassword_ReturnsAccessAndRefreshTokens()
    {
        var (_, email, password) = await IntegrationTestDataSeeder.CreateConfirmedCustomerAsync(
            Factory.Services
        );

        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", new { email, password });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var login = await ReadDataAsync<LoginResponseDto>(response);
        login.AccessToken.ShouldNotBeNullOrWhiteSpace();
        login.RefreshToken.ShouldNotBeNullOrWhiteSpace();
        login.User.Email.ShouldBe(email);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var (_, email, _) = await IntegrationTestDataSeeder.CreateConfirmedCustomerAsync(
            Factory.Services
        );

        var response = await Client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email, password = "totally-wrong" }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_UnknownEmail_Returns401WithoutRevealingWhichPartWasWrong()
    {
        var response = await Client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email = "nobody@test.local", password = "whatever" }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_WithoutBearerToken_Returns401()
    {
        var response = await Client.GetAsync("/api/v1/auth/me");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_WithValidBearerToken_ReturnsCurrentUser()
    {
        var (accessToken, userId) = await RegisterConfirmedCustomerAndLoginAsync();
        using var client = CreateAuthenticatedClient(accessToken);

        var response = await client.GetAsync("/api/v1/auth/me");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var me = await ReadEnvelopeAsync(response);
        me.GetProperty("data").GetProperty("userId").GetInt32().ShouldBe(userId);
    }

    [Fact]
    public async Task AdminUploads_WithCustomerToken_Returns403()
    {
        var (accessToken, _) = await RegisterConfirmedCustomerAndLoginAsync();
        using var client = CreateAuthenticatedClient(accessToken);

        var response = await client.PostAsync(
            "/api/v1/admin/uploads/image?folder=misc",
            content: null
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdminUploads_WithoutToken_Returns401()
    {
        var response = await Client.PostAsync(
            "/api/v1/admin/uploads/image?folder=misc",
            content: null
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AdminShowtimesCreate_WithCustomerToken_Returns403()
    {
        var (accessToken, _) = await RegisterConfirmedCustomerAndLoginAsync();
        using var client = CreateAuthenticatedClient(accessToken);
        using var scope = Factory.Services.CreateScope();
        var db =
            scope.ServiceProvider.GetRequiredService<Cinema.Infrastructure.Persistence.AppDbContext>();
        var room = db.CinemaRooms.First(r => r.IsActive);
        var movie = db.Movies.First(m => m.Status == MovieStatus.NowShowing);

        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/showtimes",
            new
            {
                movieId = movie.MovieId,
                roomId = room.RoomId,
                startTime = DateTime.UtcNow.Date.AddDays(5).AddHours(14),
                basePrice = 90_000m,
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdminShowtimesCreate_WithAdminToken_Returns201()
    {
        var (_, _, email, password) = await IntegrationTestDataSeeder.CreateEmployeeAsync(
            Factory.Services
        );
        var token = await LoginAsync(email, password, admin: true);
        using var client = CreateAuthenticatedClient(token);
        using var scope = Factory.Services.CreateScope();
        var db =
            scope.ServiceProvider.GetRequiredService<Cinema.Infrastructure.Persistence.AppDbContext>();
        var room = db.CinemaRooms.First(r => r.IsActive);
        var movie = db.Movies.First(m => m.Status == MovieStatus.NowShowing);

        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/showtimes",
            new
            {
                movieId = movie.MovieId,
                roomId = room.RoomId,
                startTime = DateTime.UtcNow.Date.AddDays(12).AddHours(14),
                basePrice = 90_000m,
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}

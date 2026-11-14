using System.Net;
using System.Net.Http.Json;
using Cinema.Infrastructure.Persistence;
using Cinema.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cinema.IntegrationTests.Tests;

public class MustChangePasswordEnforcementTests(CinemaApiFactory factory)
    : IntegrationTestBase(factory)
{
    private async Task<string> LoginFlaggedEmployeeAsync()
    {
        var (userId, _, email, password) = await IntegrationTestDataSeeder.CreateEmployeeAsync(
            Factory.Services
        );
        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db
                .Users.Where(u => u.UserId == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.MustChangePassword, true));
        }

        return await LoginAsync(email, password, admin: true);
    }

    [Fact]
    public async Task FlaggedUser_OtherApis_Return403WithStableErrorCode()
    {
        var token = await LoginFlaggedEmployeeAsync();
        using var client = CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/v1/users/me");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        var envelope = await ReadEnvelopeAsync(response);
        envelope.GetProperty("errorCode").GetString().ShouldBe("PASSWORD_CHANGE_REQUIRED");
    }

    [Fact]
    public async Task FlaggedUser_AuthMe_IsAllowedAndExposesFlag()
    {
        var token = await LoginFlaggedEmployeeAsync();
        using var client = CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/v1/auth/me");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var envelope = await ReadEnvelopeAsync(response);
        envelope.GetProperty("data").GetProperty("mustChangePassword").GetBoolean().ShouldBeTrue();
    }

    [Fact]
    public async Task FlaggedUser_ChangePassword_IsNotBlockedByTheGate()
    {
        var token = await LoginFlaggedEmployeeAsync();
        using var client = CreateAuthenticatedClient(token);

        var response = await client.PostAsJsonAsync(
            "/api/v1/users/me/change-password",
            new { currentPassword = "Str0ngPass", newPassword = "Str0ngPass" }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var envelope = await ReadEnvelopeAsync(response);
        if (envelope.TryGetProperty("errorCode", out var code))
            code.ValueKind.ShouldBe(System.Text.Json.JsonValueKind.Null);
    }

    [Fact]
    public async Task UnflaggedEmployee_IsNotBlocked()
    {
        var (_, _, email, password) = await IntegrationTestDataSeeder.CreateEmployeeAsync(
            Factory.Services
        );
        var token = await LoginAsync(email, password, admin: true);
        using var client = CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/v1/users/me");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PublicSettings_DoesNotExposeDefaultEmployeePassword()
    {
        var response = await Client.GetAsync("/api/v1/settings");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.ShouldNotContain("defaultEmployeePassword", Case.Insensitive);
        body.ShouldNotContain("Employee@123");
    }
}

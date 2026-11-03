using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Cinema.IntegrationTests.Infrastructure;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.SignalR.Client;
using Shouldly;

namespace Cinema.IntegrationTests.Tests;

public class SignalRShowtimeHubTests(CinemaApiFactory factory) : IntegrationTestBase(factory)
{
    private HubConnection CreateHubConnection(string accessToken)
    {
        return new HubConnectionBuilder()
            .WithUrl(
                new Uri(Factory.Server.BaseAddress, "/hubs/showtimes"),
                options =>
                {
                    options.HttpMessageHandlerFactory = _ => Factory.Server.CreateHandler();
                    options.AccessTokenProvider = () => Task.FromResult<string?>(accessToken);
                }
            )
            .Build();
    }

    [Fact]
    public async Task HoldSeats_BroadcastsSeatHeldEventToClientsInShowtimeGroup()
    {
        var (showtimeId, seatIds, _) = await IntegrationTestDataSeeder.CreateScheduledShowtimeAsync(
            Factory.Services
        );
        var targetSeatId = seatIds[0];

        var (token, _) = await RegisterConfirmedCustomerAndLoginAsync();
        using var client = CreateAuthenticatedClient(token);

        await using var hubConnection = CreateHubConnection(token);
        var eventReceivedTcs = new TaskCompletionSource<JsonElement>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );

        hubConnection.On<JsonElement>(
            SignalREvents.SeatHeld,
            data =>
            {
                eventReceivedTcs.TrySetResult(data);
            }
        );

        await hubConnection.StartAsync();
        await hubConnection.InvokeAsync("JoinShowtime", showtimeId);

        var holdResponse = await client.PostAsJsonAsync(
            "/api/v1/bookings/hold",
            new
            {
                showtimeId,
                customerName = "SignalR Test Customer",
                customerPhone = (string?)null,
                seatIds = new[] { targetSeatId },
                combos = (object?)null,
            }
        );
        holdResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        var completedTask = await Task.WhenAny(eventReceivedTcs.Task, Task.Delay(5000));
        completedTask.ShouldBe(
            eventReceivedTcs.Task,
            "SignalR SeatHeld event was not received within timeout."
        );

        var payload = await eventReceivedTcs.Task;
        payload.GetProperty("showtimeId").GetInt32().ShouldBe(showtimeId);
        payload.TryGetProperty("heldByUserId", out _).ShouldBeFalse();
        payload.TryGetProperty("heldUntil", out _).ShouldBeTrue();
        payload
            .GetProperty("seatIds")
            .EnumerateArray()
            .Select(e => e.GetInt32())
            .ShouldContain(targetSeatId);

        await hubConnection.StopAsync();
    }

    [Fact]
    public async Task CancelBooking_BroadcastsSeatReleasedEventToClientsInShowtimeGroup()
    {
        var (showtimeId, seatIds, _) = await IntegrationTestDataSeeder.CreateScheduledShowtimeAsync(
            Factory.Services
        );
        var targetSeatId = seatIds[0];

        var (token, _) = await RegisterConfirmedCustomerAndLoginAsync();
        using var client = CreateAuthenticatedClient(token);

        var holdResponse = await client.PostAsJsonAsync(
            "/api/v1/bookings/hold",
            new
            {
                showtimeId,
                customerName = "SignalR Release Customer",
                customerPhone = (string?)null,
                seatIds = new[] { targetSeatId },
                combos = (object?)null,
            }
        );
        holdResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var holdData = await ReadEnvelopeAsync(holdResponse);
        var bookingId = holdData.GetProperty("data").GetProperty("bookingId").GetInt32();

        await using var hubConnection = CreateHubConnection(token);
        var releaseReceivedTcs = new TaskCompletionSource<JsonElement>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );

        hubConnection.On<JsonElement>(
            SignalREvents.SeatReleased,
            data =>
            {
                releaseReceivedTcs.TrySetResult(data);
            }
        );

        await hubConnection.StartAsync();
        await hubConnection.InvokeAsync("JoinShowtime", showtimeId);

        var cancelResponse = await client.PostAsync($"/api/v1/bookings/{bookingId}/cancel", null);
        cancelResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var completedTask = await Task.WhenAny(releaseReceivedTcs.Task, Task.Delay(5000));
        completedTask.ShouldBe(
            releaseReceivedTcs.Task,
            "SignalR SeatReleased event was not received within timeout."
        );

        var payload = await releaseReceivedTcs.Task;
        payload.GetProperty("showtimeId").GetInt32().ShouldBe(showtimeId);
        payload
            .GetProperty("seatIds")
            .EnumerateArray()
            .Select(e => e.GetInt32())
            .ShouldContain(targetSeatId);

        await hubConnection.StopAsync();
    }

    [Fact]
    public async Task LeaveShowtime_DoesNotReceiveEventsAfterLeaving()
    {
        var (showtimeId, seatIds, _) = await IntegrationTestDataSeeder.CreateScheduledShowtimeAsync(
            Factory.Services
        );
        var targetSeatId = seatIds[0];

        var (token, _) = await RegisterConfirmedCustomerAndLoginAsync();
        using var client = CreateAuthenticatedClient(token);

        await using var hubConnection = CreateHubConnection(token);
        var eventReceived = false;

        hubConnection.On<JsonElement>(
            SignalREvents.SeatHeld,
            _ =>
            {
                eventReceived = true;
            }
        );

        await hubConnection.StartAsync();
        await hubConnection.InvokeAsync("JoinShowtime", showtimeId);
        await hubConnection.InvokeAsync("LeaveShowtime", showtimeId);

        var holdResponse = await client.PostAsJsonAsync(
            "/api/v1/bookings/hold",
            new
            {
                showtimeId,
                customerName = "Left Group Customer",
                customerPhone = (string?)null,
                seatIds = new[] { targetSeatId },
                combos = (object?)null,
            }
        );
        holdResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        await Task.Delay(1500);
        eventReceived.ShouldBeFalse();

        await hubConnection.StopAsync();
    }
}

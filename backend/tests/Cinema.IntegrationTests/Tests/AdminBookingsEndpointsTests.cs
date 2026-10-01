using System.Net;
using System.Net.Http.Json;
using Cinema.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Cinema.IntegrationTests.Tests;

public class AdminBookingsEndpointsTests(CinemaApiFactory factory) : IntegrationTestBase(factory)
{
    private record CounterHoldResponseDto(
        int BookingId,
        string BookingCode,
        string Status,
        decimal TotalAmount,
        decimal DiscountAmount,
        decimal FinalAmount,
        DateTime ExpiresAt,
        string Channel,
        int? CreatedByEmployeeId
    );

    private record CounterCheckoutResponseDto(int BookingId, string Status, decimal Change);

    private record CheckinResponseDto(
        int TicketId,
        int BookingId,
        string SeatRow,
        int SeatNumber,
        string MovieTitle,
        DateTime StartTime,
        DateTime CheckedInAt
    );

    private async Task<HttpClient> LoginAsEmployeeAsync()
    {
        var (_, _, email, password) = await IntegrationTestDataSeeder.CreateEmployeeAsync(Factory.Services);
        var token = await LoginAsync(email, password, admin: true);
        return CreateAuthenticatedClient(token);
    }

    [Fact]
    public async Task CounterHold_WalkInCustomerNoAccount_CreatesPendingCounterBookingWithoutUserId()
    {
        using var staffClient = await LoginAsEmployeeAsync();
        var (showtimeId, seatIds, seatPrice) = await IntegrationTestDataSeeder.CreateScheduledShowtimeAsync(Factory.Services);

        var response = await staffClient.PostAsJsonAsync(
            "/api/v1/admin/bookings/counter-hold",
            new
            {
                showtimeId,
                seatIds = seatIds.Take(2),
                combos = (object?)null,
                userId = (int?)null,
                customerName = "Khach le tai quay",
                customerPhone = "0909000000",
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var hold = await ReadDataAsync<CounterHoldResponseDto>(response);
        hold.Channel.ShouldBe("Counter");
        hold.Status.ShouldBe("Pending");
        hold.FinalAmount.ShouldBe(seatPrice * 2);
        hold.CreatedByEmployeeId.ShouldNotBeNull();
    }

    [Fact]
    public async Task CounterHold_CustomerCaller_Returns403Forbidden()
    {
        var (accessToken, _) = await RegisterConfirmedCustomerAndLoginAsync();
        using var client = CreateAuthenticatedClient(accessToken);
        var (showtimeId, seatIds, _) = await IntegrationTestDataSeeder.CreateScheduledShowtimeAsync(Factory.Services);

        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/bookings/counter-hold",
            new
            {
                showtimeId,
                seatIds = seatIds.Take(1),
                combos = (object?)null,
                userId = (int?)null,
                customerName = "Khach le",
                customerPhone = (string?)null,
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CounterHold_CallerWithoutEmployeeRecord_Returns422()
    {
        var (_, email, password) = await IntegrationTestDataSeeder.CreateAdminUserWithoutEmployeeAsync(Factory.Services);
        var token = await LoginAsync(email, password, admin: true);
        using var client = CreateAuthenticatedClient(token);
        var (showtimeId, seatIds, _) = await IntegrationTestDataSeeder.CreateScheduledShowtimeAsync(Factory.Services);

        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/bookings/counter-hold",
            new
            {
                showtimeId,
                seatIds = seatIds.Take(1),
                combos = (object?)null,
                userId = (int?)null,
                customerName = "Khach le",
                customerPhone = (string?)null,
            }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CounterCheckout_CashAmountAboveTotal_ConfirmsBookingAndReturnsCorrectChange()
    {
        using var staffClient = await LoginAsEmployeeAsync();
        var (showtimeId, seatIds, seatPrice) = await IntegrationTestDataSeeder.CreateScheduledShowtimeAsync(Factory.Services);
        var holdResponse = await staffClient.PostAsJsonAsync(
            "/api/v1/admin/bookings/counter-hold",
            new
            {
                showtimeId,
                seatIds = seatIds.Take(1),
                combos = (object?)null,
                userId = (int?)null,
                customerName = "Khach le",
                customerPhone = (string?)null,
            }
        );
        var hold = await ReadDataAsync<CounterHoldResponseDto>(holdResponse);
        var amountReceived = seatPrice + 50_000m;

        var checkoutResponse = await staffClient.PostAsJsonAsync(
            $"/api/v1/admin/bookings/{hold.BookingId}/counter-checkout",
            new { amountReceived }
        );

        checkoutResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var checkout = await ReadDataAsync<CounterCheckoutResponseDto>(checkoutResponse);
        checkout.Status.ShouldBe("Confirmed");
        checkout.Change.ShouldBe(50_000m);

        var detailResponse = await staffClient.GetAsync($"/api/v1/admin/bookings/{hold.BookingId}");
        var detail = await ReadEnvelopeAsync(detailResponse);
        detail.GetProperty("data").GetProperty("status").GetString().ShouldBe("Confirmed");
    }

    [Fact]
    public async Task CounterCheckout_InsufficientCashReceived_Returns422AndLeavesBookingPending()
    {
        using var staffClient = await LoginAsEmployeeAsync();
        var (showtimeId, seatIds, seatPrice) = await IntegrationTestDataSeeder.CreateScheduledShowtimeAsync(Factory.Services);
        var holdResponse = await staffClient.PostAsJsonAsync(
            "/api/v1/admin/bookings/counter-hold",
            new
            {
                showtimeId,
                seatIds = seatIds.Take(1),
                combos = (object?)null,
                userId = (int?)null,
                customerName = "Khach le",
                customerPhone = (string?)null,
            }
        );
        var hold = await ReadDataAsync<CounterHoldResponseDto>(holdResponse);

        var response = await staffClient.PostAsJsonAsync(
            $"/api/v1/admin/bookings/{hold.BookingId}/counter-checkout",
            new { amountReceived = seatPrice - 1_000m }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        var detailResponse = await staffClient.GetAsync($"/api/v1/admin/bookings/{hold.BookingId}");
        var detail = await ReadEnvelopeAsync(detailResponse);
        detail.GetProperty("data").GetProperty("status").GetString().ShouldBe("Pending");
    }

    [Fact]
    public async Task Checkin_ValidUncheckedTicketWithinWindow_MarksItCheckedIn()
    {
        using var staffClient = await LoginAsEmployeeAsync();
        var (showtimeId, seatIds, seatPrice) = await IntegrationTestDataSeeder.CreateScheduledShowtimeAsync(
            Factory.Services,
            startTime: DateTime.UtcNow.AddMinutes(5)
        );
        var holdResponse = await staffClient.PostAsJsonAsync(
            "/api/v1/admin/bookings/counter-hold",
            new
            {
                showtimeId,
                seatIds = seatIds.Take(1),
                combos = (object?)null,
                userId = (int?)null,
                customerName = "Khach le",
                customerPhone = (string?)null,
            }
        );
        var hold = await ReadDataAsync<CounterHoldResponseDto>(holdResponse);
        await staffClient.PostAsJsonAsync($"/api/v1/admin/bookings/{hold.BookingId}/counter-checkout", new { amountReceived = seatPrice });
        var qrCode = await GetFirstTicketQrCodeAsync(hold.BookingId);

        var response = await staffClient.PostAsJsonAsync("/api/v1/admin/bookings/checkin", new { qrCode });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var checkin = await ReadDataAsync<CheckinResponseDto>(response);
        checkin.BookingId.ShouldBe(hold.BookingId);
    }

    [Fact]
    public async Task Checkin_AlreadyCheckedInTicket_Returns409()
    {
        using var staffClient = await LoginAsEmployeeAsync();
        var (showtimeId, seatIds, seatPrice) = await IntegrationTestDataSeeder.CreateScheduledShowtimeAsync(
            Factory.Services,
            startTime: DateTime.UtcNow.AddMinutes(5)
        );
        var holdResponse = await staffClient.PostAsJsonAsync(
            "/api/v1/admin/bookings/counter-hold",
            new
            {
                showtimeId,
                seatIds = seatIds.Take(1),
                combos = (object?)null,
                userId = (int?)null,
                customerName = "Khach le",
                customerPhone = (string?)null,
            }
        );
        var hold = await ReadDataAsync<CounterHoldResponseDto>(holdResponse);
        await staffClient.PostAsJsonAsync($"/api/v1/admin/bookings/{hold.BookingId}/counter-checkout", new { amountReceived = seatPrice });
        var qrCode = await GetFirstTicketQrCodeAsync(hold.BookingId);
        await staffClient.PostAsJsonAsync("/api/v1/admin/bookings/checkin", new { qrCode });

        var response = await staffClient.PostAsJsonAsync("/api/v1/admin/bookings/checkin", new { qrCode });

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Checkin_UnknownQrCode_Returns404()
    {
        using var staffClient = await LoginAsEmployeeAsync();

        var response = await staffClient.PostAsJsonAsync("/api/v1/admin/bookings/checkin", new { qrCode = "does-not-exist" });

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Checkin_TicketForShowtimeMoreThan30MinutesAway_Returns422()
    {
        using var staffClient = await LoginAsEmployeeAsync();
        var (showtimeId, seatIds, seatPrice) = await IntegrationTestDataSeeder.CreateScheduledShowtimeAsync(Factory.Services);
        var holdResponse = await staffClient.PostAsJsonAsync(
            "/api/v1/admin/bookings/counter-hold",
            new
            {
                showtimeId,
                seatIds = seatIds.Take(1),
                combos = (object?)null,
                userId = (int?)null,
                customerName = "Khach le",
                customerPhone = (string?)null,
            }
        );
        var hold = await ReadDataAsync<CounterHoldResponseDto>(holdResponse);
        await staffClient.PostAsJsonAsync($"/api/v1/admin/bookings/{hold.BookingId}/counter-checkout", new { amountReceived = seatPrice });
        var qrCode = await GetFirstTicketQrCodeAsync(hold.BookingId);

        var response = await staffClient.PostAsJsonAsync("/api/v1/admin/bookings/checkin", new { qrCode });

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }

    private async Task<string> GetFirstTicketQrCodeAsync(int bookingId)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Cinema.Infrastructure.Persistence.AppDbContext>();
        return await db
            .BookingTickets.Where(t => t.BookingId == bookingId)
            .Select(t => t.QrCode)
            .FirstAsync();
    }
}


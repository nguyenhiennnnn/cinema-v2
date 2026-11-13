using Cinema.Api.Extensions;
using Cinema.Api.Filters;
using Cinema.Application.DTOs.Bookings;
using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Cinema.Api.Controllers.Client;

[ApiController]
[Route("api/v1/bookings")]
[Authorize]
public class BookingsController(
    IBookingHoldService bookingHoldService,
    IBookingQueryService bookingQueryService,
    IBookingCheckoutService bookingCheckoutService,
    IBookingCancellationService bookingCancellationService
) : AppControllerBase
{
    [HttpPost("hold")]
    [EnableRateLimiting(RateLimitingExtensions.BookingHoldPolicy)]
    public async Task<ActionResult<HoldBookingResponse>> HoldSeats(
        [FromBody] HoldSeatsRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await bookingHoldService.HoldSeatsAsync(
            CurrentUserId,
            request,
            cancellationToken
        );
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet]
    public async Task<ActionResult<CursorPageResult<BookingListItemDto>>> GetMyBookings(
        [FromQuery] string? status,
        [FromQuery] string? cursor,
        [FromQuery] int? limit = null,
        CancellationToken cancellationToken = default
    )
    {
        var result = await bookingQueryService.GetMyBookingsAsync(
            CurrentUserId,
            status,
            cursor,
            ClampLimit(limit),
            cancellationToken
        );
        return Ok(result);
    }

    [HttpGet("{bookingId:int}")]
    public async Task<ActionResult<BookingDetailDto>> GetBookingDetail(
        int bookingId,
        CancellationToken cancellationToken
    )
    {
        var result = await bookingQueryService.GetBookingDetailAsync(
            CurrentUserId,
            bookingId,
            cancellationToken
        );
        return Ok(result);
    }

    [HttpPost("{bookingId:int}/checkout")]
    public async Task<ActionResult<CheckoutResponse>> Checkout(
        int bookingId,
        [FromBody] CheckoutRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await bookingCheckoutService.CheckoutAsync(
            CurrentUserId,
            bookingId,
            request,
            cancellationToken
        );
        return Ok(result);
    }

    [HttpPost("{bookingId:int}/cancel")]
    [SuccessMessage(nameof(Messages.Success_BookingCancelled))]
    public async Task<ActionResult<CancelBookingResponse>> CancelBooking(
        int bookingId,
        CancellationToken cancellationToken
    )
    {
        var result = await bookingCancellationService.CancelBookingAsync(
            CurrentUserId,
            bookingId,
            cancellationToken
        );
        return Ok(result);
    }
}

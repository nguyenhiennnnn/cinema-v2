using Cinema.Application.DTOs.Bookings;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/bookings")]
[Authorize]
public class AdminCounterController(ICounterBookingService counterBookingService)
    : AppControllerBase
{
    private int GetCurrentEmployeeId() =>
        CurrentEmployeeIdOrNull
        ?? throw new BusinessRuleException(Messages.Rule_NoEmployeeProfileForCounterBooking);

    [HttpPost("counter-hold")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<ActionResult<CounterHoldResponse>> CounterHoldSeats(
        [FromBody] CounterHoldRequest request,
        CancellationToken cancellationToken
    )
    {
        var employeeId = GetCurrentEmployeeId();
        var result = await counterBookingService.CounterHoldSeatsAsync(
            employeeId,
            CurrentUserId,
            request,
            cancellationToken
        );
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("{bookingId:int}/counter-checkout")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<ActionResult<CounterCheckoutResponse>> CounterCheckout(
        int bookingId,
        [FromBody] CounterCheckoutRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await counterBookingService.CounterCheckoutAsync(
            bookingId,
            request,
            cancellationToken
        );
        return Ok(result);
    }

    [HttpPost("{bookingId:int}/counter-cancel")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<ActionResult<CancelBookingResponse>> CounterCancel(
        int bookingId,
        CancellationToken cancellationToken
    )
    {
        var result = await counterBookingService.CounterCancelAsync(
            bookingId,
            GetCurrentEmployeeId(),
            cancellationToken
        );
        return Ok(result);
    }

    [HttpPost("checkin")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<ActionResult<CheckinResponse>> Checkin(
        [FromBody] CheckinRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await counterBookingService.CheckinAsync(request, cancellationToken);
        return Ok(result);
    }
}

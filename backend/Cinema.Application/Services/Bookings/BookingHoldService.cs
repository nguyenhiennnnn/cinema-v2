using Cinema.Application.DTOs.Bookings;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema.Application.Services.Bookings;

public class BookingHoldService(
    IAppDbContext db,
    IRealtimeNotifier realtimeNotifier,
    IEnumerable<IBookingHoldPolicy> holdPolicies,
    IBookingFulfillmentService fulfillmentService,
    ILogger<BookingHoldService> logger
) : IBookingHoldService
{
    public async Task<HoldBookingResponse> HoldSeatsAsync(
        int userId,
        HoldSeatsRequest request,
        CancellationToken cancellationToken
    )
    {
        await EnsureWithinBookingLimitsAsync(
            request.SeatIds.Count,
            request.Combos,
            cancellationToken
        );
        var slots = await db.LoadSeatSlotsAsync(request.ShowtimeId, cancellationToken);
        var gapRuleMode = await db.LoadSeatGapRuleModeAsync(cancellationToken);
        if (
            SeatLayoutRules.ValidateSelection(slots, request.SeatIds, gapRuleMode) is { } seatError
        )
            throw new BusinessRuleException(seatError);

        var booking = await CreateHoldBookingAsync(
            request.ShowtimeId,
            request.SeatIds,
            request.Combos,
            holderUserId: userId,
            channel: BookingChannel.Online,
            bookingUserId: userId,
            createdByEmployeeId: null,
            customerName: request.CustomerName,
            customerPhone: request.CustomerPhone,
            cancellationToken
        );

        return booking.Adapt<HoldBookingResponse>();
    }

    public async Task<Booking> CreateHoldBookingAsync(
        int showtimeId,
        List<int> showtimeSeatIds,
        List<ComboSelectionRequest>? comboSelections,
        int holderUserId,
        BookingChannel channel,
        int? bookingUserId,
        int? createdByEmployeeId,
        string customerName,
        string? customerPhone,
        CancellationToken cancellationToken
    )
    {
        var showtime =
            await db.Showtimes.FirstOrDefaultAsync(
                s => s.ShowtimeId == showtimeId,
                cancellationToken
            ) ?? throw new NotFoundException(Messages.NotFound_Showtime);
        if (showtime.Status != ShowtimeStatus.Scheduled)
            throw new BusinessRuleException(
                Messages.Rule_ShowtimeUnavailableForBooking
            );
        if (showtime.StartTime <= DateTime.UtcNow)
            throw new BusinessRuleException(
                Messages.Rule_ShowtimeStartedOrEnded
            );

        var holdSettings = await db
            .CinemaSettings.Where(s => s.Id == AppDbContextExtensions.CinemaSettingsId)
            .Select(s => new { s.StopSellingMinutesBefore, s.HoldDurationMinutes })
            .FirstOrDefaultAsync(cancellationToken);
        var holdMinutes =
            holdSettings?.HoldDurationMinutes ?? CinemaSettings.DefaultHoldDurationMinutes;

        var policy = holdPolicies.First(p => p.Channel == channel);
        policy.EnsureSalesOpen(showtime, holdSettings?.StopSellingMinutesBefore ?? 0);

        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ValidationAppException(
                Messages.Invalid_CustomerNameMissing,
                new Dictionary<string, string[]>
                {
                    ["customerName"] = [Messages.Invalid_CustomerNameRequired],
                }
            );
        }

        await policy.EnsureNoConflictingPendingAsync(
            new BookingHoldPolicyContext(showtimeId, holderUserId, createdByEmployeeId),
            cancellationToken
        );

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var holdUntil = now.AddMinutes(holdMinutes);
        if (holdUntil > showtime.StartTime)
            holdUntil = showtime.StartTime;
        var distinctSeatIds = showtimeSeatIds.Distinct().ToList();

        var affected = await db
            .ShowtimeSeats.Where(ss =>
                distinctSeatIds.Contains(ss.ShowtimeSeatId)
                && ss.ShowtimeId == showtimeId
                && (
                    ss.Status == ShowtimeSeatStatus.Available
                    || (
                        ss.Status == ShowtimeSeatStatus.Held
                        && ss.HeldByUserId == holderUserId
                        && ss.HeldUntil != null
                        && ss.HeldUntil < now
                    )
                )
            )
            .ExecuteUpdateAsync(
                setters =>
                    setters
                        .SetProperty(x => x.Status, ShowtimeSeatStatus.Held)
                        .SetProperty(x => x.HeldByUserId, holderUserId)
                        .SetProperty(x => x.HeldByBookingId, (int?)null)
                        .SetProperty(x => x.HeldUntil, holdUntil),
                cancellationToken
            );

        if (affected != distinctSeatIds.Count)
        {
            var heldSeatIds = await db
                .ShowtimeSeats.Where(ss =>
                    distinctSeatIds.Contains(ss.ShowtimeSeatId)
                    && ss.ShowtimeId == showtimeId
                    && ss.HeldByUserId == holderUserId
                    && ss.Status == ShowtimeSeatStatus.Held
                    && ss.HeldUntil == holdUntil
                )
                .Select(ss => ss.ShowtimeSeatId)
                .ToListAsync(cancellationToken);
            var conflictSeatIds = distinctSeatIds.Except(heldSeatIds).ToList();

            await transaction.RollbackAsync(cancellationToken);
            throw new SeatConflictException(
                Messages.Conflict_SeatsAlreadyTaken,
                conflictSeatIds
            );
        }

        var heldSeats = await db
            .ShowtimeSeats.Include(ss => ss.Seat)
            .Where(ss => distinctSeatIds.Contains(ss.ShowtimeSeatId) && ss.ShowtimeId == showtimeId)
            .ToListAsync(cancellationToken);
        var (comboTotal, comboEntities) = await BuildBookingCombosAsync(
            comboSelections,
            cancellationToken
        );
        var seatTotal = heldSeats.Sum(s => s.Price);
        var totalAmount = seatTotal + comboTotal;
        // Giảm giá hạng thành viên chỉ áp dụng trên tiền vé, KHÔNG áp dụng cho combo.
        var discountAmount = await CalculateDiscountAmountAsync(
            bookingUserId,
            seatTotal,
            cancellationToken
        );
        var finalAmount = totalAmount - discountAmount;

        var booking = new Booking
        {
            UserId = bookingUserId,
            ShowtimeId = showtimeId,
            BookingCode = await fulfillmentService.GenerateUniqueBookingCodeAsync(cancellationToken),
            Channel = channel,
            CreatedByEmployeeId = createdByEmployeeId,
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            TotalAmount = totalAmount,
            DiscountAmount = discountAmount,
            FinalAmount = finalAmount,
            Status = BookingStatus.Pending,
            CreatedAt = now,
            ExpiresAt = holdUntil,
        };

        foreach (var combo in comboEntities)
            booking.BookingCombos.Add(combo);

        db.Bookings.Add(booking);
        await db.SaveChangesAsync(cancellationToken);
        await db
            .ShowtimeSeats.Where(ss =>
                distinctSeatIds.Contains(ss.ShowtimeSeatId) && ss.ShowtimeId == showtimeId
            )
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(x => x.HeldByBookingId, booking.BookingId),
                cancellationToken
            );
        await transaction.CommitAsync(cancellationToken);

        await realtimeNotifier.NotifySeatHeldAsync(
            showtimeId,
            distinctSeatIds,
            holderUserId,
            holdUntil,
            cancellationToken
        );
        logger.LogInformation(
            "SeatsHeld BookingId {BookingId} ShowtimeId {ShowtimeId} SeatCount {Count}",
            booking.BookingId,
            showtimeId,
            distinctSeatIds.Count
        );

        return booking;
    }

    public async Task EnsureWithinBookingLimitsAsync(
        int seatCount,
        List<ComboSelectionRequest>? combos,
        CancellationToken cancellationToken
    )
    {
        var limits = await db
            .CinemaSettings.AsNoTracking()
            .Where(s => s.Id == AppDbContextExtensions.CinemaSettingsId)
            .Select(s => new { s.MaxSeatsPerBooking, s.MaxComboQuantity })
            .FirstOrDefaultAsync(cancellationToken);
        var maxSeats = limits?.MaxSeatsPerBooking ?? CinemaSettings.DefaultMaxSeatsPerBooking;
        var maxCombo = limits?.MaxComboQuantity ?? CinemaSettings.DefaultMaxComboQuantity;

        if (seatCount > maxSeats)
        {
            var message = string.Format(Messages.Validation_Seats_MaxPerBooking, maxSeats);
            throw new ValidationAppException(
                message,
                new Dictionary<string, string[]> { ["seatIds"] = [message] }
            );
        }

        if (combos is { Count: > 0 } && combos.Any(c => c.Quantity > maxCombo))
        {
            var message = string.Format(Messages.Validation_Quantity_Max, maxCombo);
            throw new ValidationAppException(
                message,
                new Dictionary<string, string[]> { ["combos"] = [message] }
            );
        }
    }

    private async Task<decimal> CalculateDiscountAmountAsync(
        int? bookingUserId,
        decimal ticketAmount,
        CancellationToken cancellationToken
    )
    {
        if (bookingUserId is null)
            return 0m;

        var hasProfile = await db.CustomerProfiles.AnyAsync(
            p => p.UserId == bookingUserId,
            cancellationToken
        );
        if (!hasProfile)
            return 0m;

        // Hạng tính thẳng từ sổ điểm trong cửa sổ 12 tháng (không tin profile.TierId là bản cache),
        // nên điểm vừa hết hạn có hiệu lực ngay, không phải chờ job hằng giờ.
        var points = await LoyaltyWindow.SumActiveAsync(
            db,
            bookingUserId.Value,
            DateTime.UtcNow,
            cancellationToken
        );
        var tier = await LoyaltyWindow.ResolveTierAsync(db, points, cancellationToken);
        if (tier is null)
            return 0m;

        return Math.Round(
            ticketAmount * tier.DiscountPercent / 100m,
            0,
            MidpointRounding.AwayFromZero
        );
    }

    private async Task<(decimal ComboTotal, List<BookingCombo> Entities)> BuildBookingCombosAsync(
        List<ComboSelectionRequest>? comboSelections,
        CancellationToken cancellationToken
    )
    {
        var entities = new List<BookingCombo>();
        if (comboSelections is null || comboSelections.Count == 0)
            return (0m, entities);

        var comboIds = comboSelections.Select(c => c.ComboId).Distinct().ToList();
        var combos = await db
            .Combos.Where(c => comboIds.Contains(c.ComboId) && c.IsActive)
            .ToListAsync(cancellationToken);
        if (combos.Count != comboIds.Count)
            throw new ValidationAppException(
                Messages.Invalid_ComboIdsUnavailable,
                new Dictionary<string, string[]>
                {
                    ["combos"] = [Messages.Invalid_ComboIdUnavailable],
                }
            );

        var comboLookup = combos.ToDictionary(c => c.ComboId);
        decimal total = 0;
        foreach (var selection in comboSelections)
        {
            var combo = comboLookup[selection.ComboId];
            entities.Add(
                new BookingCombo
                {
                    ComboId = combo.ComboId,
                    Quantity = selection.Quantity,
                    UnitPrice = combo.Price,
                }
            );
            total += combo.Price * selection.Quantity;
        }
        return (total, entities);
    }
}

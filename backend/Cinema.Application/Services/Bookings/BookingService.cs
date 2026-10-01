using Cinema.Application.BackgroundJobs;
using Cinema.Application.DTOs.Bookings;
using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Hangfire;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Bookings;

public class BookingService(
    IAppDbContext db,
    IOptions<BookingOptions> bookingOptions,
    IRealtimeNotifier realtimeNotifier,
    IEnumerable<IPaymentGateway> paymentGateways,
    IBackgroundJobClient backgroundJobClient,
    ILogger<BookingService> logger
) : IBookingService
{
    private readonly int _holdMinutes = bookingOptions.Value.HoldDurationMinutes;

    public async Task<HoldBookingResponse> HoldSeatsAsync(
        int userId,
        HoldSeatsRequest request,
        CancellationToken cancellationToken
    )
    {
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

    public async Task<CursorPageResult<BookingListItemDto>> GetMyBookingsAsync(
        int userId,
        string? status,
        string? cursor,
        int limit,
        CancellationToken cancellationToken
    )
    {
        var query = db
            .Bookings.Include(b => b.Showtime)
                .ThenInclude(s => s.Movie)
            .Where(b => b.UserId == userId);

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<BookingStatus>(status, out var st))
            query = query.Where(b => b.Status == st);

        var cursorId = CursorCodec.Decode(cursor);
        if (cursorId is not null)
            query = query.Where(b => b.BookingId < cursorId);
        query = query.OrderByDescending(b => b.BookingId);

        var rows = await query
            .Take(limit + 1)
            .ProjectToType<BookingListItemDto>()
            .ToListAsync(cancellationToken);

        var hasMore = rows.Count > limit;
        var items = hasMore ? rows.Take(limit).ToList() : rows;
        var nextCursor = hasMore ? CursorCodec.Encode(items[^1].BookingId) : null;

        return new CursorPageResult<BookingListItemDto>
        {
            Items = items,
            NextCursor = nextCursor,
            HasMore = hasMore,
        };
    }

    public async Task<BookingDetailDto> GetBookingDetailAsync(
        int userId,
        int bookingId,
        CancellationToken cancellationToken
    )
    {
        var booking = await LoadBookingOrThrowAsync(bookingId, cancellationToken);
        if (booking.UserId != userId)
            throw new ForbiddenAppException("Bạn không phải chủ booking này.");

        var seats = await GetBookingSeatsAsync(booking, cancellationToken);
        var combos = await GetBookingCombosAsync(bookingId, cancellationToken);
        var tickets = await GetBookingTicketsAsync(booking, cancellationToken);
        var payments = await GetBookingPaymentsAsync(bookingId, cancellationToken);

        return new BookingDetailDto(
            booking.BookingId,
            booking.BookingCode,
            booking.Status.ToDtoStatus(),
            booking.Adapt<BookingMovieBriefDto>(),
            booking.Adapt<BookingShowtimeBriefDto>(),
            seats,
            combos,
            booking.TotalAmount,
            booking.DiscountAmount,
            booking.FinalAmount,
            booking.CreatedAt,
            tickets,
            payments,
            booking.Status == BookingStatus.Pending ? booking.ExpiresAt : null
        );
    }

    public async Task<CheckoutResponse> CheckoutAsync(
        int userId,
        int bookingId,
        CheckoutRequest request,
        CancellationToken cancellationToken
    )
    {
        var booking =
            await db.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy booking.");
        if (booking.UserId != userId)
            throw new ForbiddenAppException("Bạn không phải chủ booking này.");
        if (booking.Status != BookingStatus.Pending || booking.ExpiresAt <= DateTime.UtcNow)
            throw new BusinessRuleException("Booking không ở trạng thái Pending hoặc đã hết hạn.");

        var gateway =
            paymentGateways.FirstOrDefault(g => g.MethodName == request.PaymentMethod)
            ?? throw new BusinessRuleException(
                $"Payment gateway '{request.PaymentMethod}' chưa được cấu hình."
            );

        var transactionId = CodeGenerator.GeneratePaymentTransactionId(booking.BookingCode);

        var gatewayResult = await gateway.CreatePaymentAsync(
            new CreatePaymentRequest(
                booking.BookingId,
                booking.FinalAmount,
                $"Thanh toán booking {booking.BookingCode}",
                transactionId
            ),
            cancellationToken
        );

        var payment = new Payment
        {
            BookingId = booking.BookingId,
            PaymentMethod = EnumConverters.ParseEnum<PaymentMethod>(
                request.PaymentMethod,
                "paymentMethod"
            ),
            Status = PaymentStatus.Pending,
            Amount = booking.FinalAmount,
            TransactionId = transactionId,
            CreatedAt = DateTime.UtcNow,
        };
        db.Payments.Add(payment);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "PaymentCreated PaymentId {PaymentId} BookingId {BookingId} Method {Method}",
            payment.PaymentId,
            booking.BookingId,
            request.PaymentMethod
        );

        return new CheckoutResponse(gatewayResult.PaymentUrl, payment.PaymentId);
    }

    public async Task<RefundEligibility> CalculateRefundEligibilityAsync(
        DateTime showtimeStartTime,
        decimal paidAmount,
        CancellationToken cancellationToken
    )
    {
        var settings = await GetCinemaSettingsAsync(cancellationToken);
        return CalculateRefundEligibility(showtimeStartTime, paidAmount, settings);
    }

    private static RefundEligibility CalculateRefundEligibility(
        DateTime showtimeStartTime,
        decimal paidAmount,
        CinemaSettings settings
    )
    {
        if (settings.CancelRefundPercent <= 0)
            return new RefundEligibility(false, 0m, "Rạp hiện không hỗ trợ hoàn tiền.");

        if (DateTime.UtcNow > showtimeStartTime.AddHours(-settings.CancelDeadlineHours))
            return new RefundEligibility(
                false,
                0m,
                $"Đã quá hạn hoàn tiền — chỉ áp dụng trước giờ chiếu ít nhất {settings.CancelDeadlineHours} giờ."
            );

        var refundAmount = Math.Round(
            paidAmount * settings.CancelRefundPercent / 100m,
            0,
            MidpointRounding.AwayFromZero
        );
        return new RefundEligibility(true, refundAmount, null);
    }

    public async Task<CancelBookingResponse> CancelBookingAsync(
        int userId,
        int bookingId,
        CancellationToken cancellationToken
    )
    {
        var booking =
            await db
                .Bookings.Include(b => b.Showtime)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy booking.");
        if (booking.UserId != userId)
            throw new ForbiddenAppException("Bạn không phải chủ booking này.");

        if (booking.Status == BookingStatus.Pending)
        {
            await ReleaseAndCancelBookingAsync(booking, cancellationToken);
            logger.LogInformation(
                "BookingCancelled(Pending) BookingId {BookingId} UserId {UserId}",
                bookingId,
                userId
            );
            return new CancelBookingResponse(
                bookingId,
                "Cancelled",
                RefundAmount: null,
                RefundStatus: null
            );
        }

        if (booking.Status != BookingStatus.Confirmed)
            throw new BusinessRuleException(
                "Booking không ở trạng thái có thể hủy (đã Cancelled/Expired)."
            );

        var hasCheckedInTicket = await db.BookingTickets.AnyAsync(
            t => t.BookingId == bookingId && t.IsCheckedIn,
            cancellationToken
        );
        if (hasCheckedInTicket)
            throw new BusinessRuleException(
                "Booking đã có vé được check-in — vui lòng liên hệ trực tiếp rạp để được hỗ trợ."
            );

        var payment = booking
            .Payments.Where(p => p.Status == PaymentStatus.Success)
            .OrderByDescending(p => p.PaymentId)
            .FirstOrDefault();
        if (payment is null || payment.PaymentMethod == PaymentMethod.Cash)
            throw new BusinessRuleException(
                "Booking thanh toán tại quầy hoặc chưa xác định được giao dịch — vui lòng liên hệ trực tiếp rạp để được hỗ trợ hủy vé."
            );

        var settings = await GetCinemaSettingsAsync(cancellationToken);

        var eligibility = CalculateRefundEligibility(
            booking.Showtime.StartTime,
            payment.Amount,
            settings
        );
        if (!eligibility.Eligible)
            throw new BusinessRuleException(eligibility.IneligibleReason!);

        var gateway =
            paymentGateways.FirstOrDefault(g => g.MethodName == payment.PaymentMethod.ToString())
            ?? throw new BusinessRuleException(
                $"Payment gateway '{payment.PaymentMethod}' chưa được cấu hình."
            );

        var refundResult = await gateway.RefundAsync(
            new RefundRequest(
                payment.TransactionId!,
                eligibility.RefundAmount,
                $"Hoàn tiền huỷ vé {booking.BookingCode}",
                payment.PaidAt ?? payment.CreatedAt,
                GatewayTransactionId: payment.GatewayTransactionId
            ),
            cancellationToken
        );

        if (!refundResult.Success)
        {
            logger.LogWarning(
                "RefundFailed BookingId {BookingId} PaymentId {PaymentId} Message {Message}",
                bookingId,
                payment.PaymentId,
                refundResult.Message
            );
            throw new BusinessRuleException(
                $"Hoàn tiền thất bại: {refundResult.Message}. Vui lòng thử lại sau hoặc liên hệ rạp."
            );
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var releasedSeatIds = await ReleaseConfirmedBookingSeatsAsync(booking, cancellationToken);
        booking.Status = BookingStatus.Cancelled;
        payment.Status = PaymentStatus.Refunded;
        payment.RefundedAmount = eligibility.RefundAmount;
        payment.RefundedAt = DateTime.UtcNow;
        payment.RefundTransactionId = refundResult.RefundTransactionId;

        if (booking.UserId is not null)
            await ReverseLoyaltyAsync(
                booking.UserId.Value,
                booking.FinalAmount,
                settings,
                cancellationToken
            );

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await realtimeNotifier.NotifySeatReleasedAsync(
            booking.ShowtimeId,
            releasedSeatIds,
            cancellationToken
        );
        logger.LogInformation(
            "BookingCancelled(Confirmed+Refunded) BookingId {BookingId} UserId {UserId} RefundAmount {Amount}",
            bookingId,
            userId,
            eligibility.RefundAmount
        );

        var html = EmailTemplates.BookingCancelledRefunded(
            booking.CustomerName,
            booking.BookingCode,
            eligibility.RefundAmount
        );
        var userEmail = await db
            .Users.Where(u => u.UserId == userId)
            .Select(u => u.Email)
            .FirstAsync(cancellationToken);
        backgroundJobClient.Enqueue<SendEmailJob>(j =>
            j.RunAsync(
                userEmail,
                "Xác nhận hủy vé & hoàn tiền — Cinema System",
                html,
                CancellationToken.None
            )
        );

        return new CancelBookingResponse(
            bookingId,
            "Cancelled",
            eligibility.RefundAmount,
            "Refunded"
        );
    }

    public async Task<CounterHoldResponse> CounterHoldSeatsAsync(
        int employeeId,
        int employeeUserId,
        CounterHoldRequest request,
        CancellationToken cancellationToken
    )
    {
        var booking = await CreateHoldBookingAsync(
            request.ShowtimeId,
            request.SeatIds,
            request.Combos,
            holderUserId: employeeUserId,
            channel: BookingChannel.Counter,
            bookingUserId: request.UserId,
            createdByEmployeeId: employeeId,
            customerName: request.CustomerName,
            customerPhone: request.CustomerPhone,
            cancellationToken
        );

        return booking.Adapt<CounterHoldResponse>();
    }

    public async Task<OffsetPageResult<AdminBookingListItemDto>> GetBookingsAdminAsync(
        AdminBookingListFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.Bookings.AsQueryable();

        if (
            !string.IsNullOrWhiteSpace(filter.Status)
            && Enum.TryParse<BookingStatus>(filter.Status, out var st)
        )
            query = query.Where(b => b.Status == st);
        if (
            !string.IsNullOrWhiteSpace(filter.Channel)
            && Enum.TryParse<BookingChannel>(filter.Channel, out var ch)
        )
            query = query.Where(b => b.Channel == ch);
        if (filter.FromDate is not null)
            query = query.Where(b =>
                b.CreatedAt >= filter.FromDate.Value.ToDateTime(TimeOnly.MinValue)
            );
        if (filter.ToDate is not null)
            query = query.Where(b =>
                b.CreatedAt < filter.ToDate.Value.AddDays(1).ToDateTime(TimeOnly.MinValue)
            );
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(b =>
                b.BookingCode.Contains(filter.Search)
                || b.CustomerName.Contains(filter.Search)
                || (b.CustomerPhone != null && b.CustomerPhone.Contains(filter.Search))
            );

        query = query.OrderByDescending(b => b.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<AdminBookingListItemDto>()
            .ToListAsync(cancellationToken);

        return OffsetPageResult<AdminBookingListItemDto>.Create(items, page, pageSize, total);
    }

    public async Task<AdminBookingDetailDto> GetBookingDetailAdminAsync(
        int bookingId,
        CancellationToken cancellationToken
    )
    {
        var booking = await LoadBookingOrThrowAsync(bookingId, cancellationToken);

        var seats = await GetBookingSeatsAsync(booking, cancellationToken);
        var combos = await GetBookingCombosAsync(bookingId, cancellationToken);
        var tickets = await GetBookingTicketsAsync(booking, cancellationToken);
        var payments = await GetBookingPaymentsAsync(bookingId, cancellationToken);

        BookingEmployeeBriefDto? employeeDto = null;
        if (booking.CreatedByEmployeeId is not null)
        {
            var employee = await db
                .Employees.Include(e => e.User)
                .FirstOrDefaultAsync(
                    e => e.EmployeeId == booking.CreatedByEmployeeId,
                    cancellationToken
                );
            if (employee is not null)
                employeeDto = employee.Adapt<BookingEmployeeBriefDto>();
        }

        return new AdminBookingDetailDto(
            booking.BookingId,
            booking.BookingCode,
            booking.Status.ToDtoStatus(),
            booking.Adapt<BookingMovieBriefDto>(),
            booking.Adapt<BookingShowtimeBriefDto>(),
            seats,
            combos,
            booking.TotalAmount,
            booking.DiscountAmount,
            booking.FinalAmount,
            booking.CreatedAt,
            booking.Channel.ToDtoChannel(),
            employeeDto,
            tickets,
            payments
        );
    }

    public async Task<CounterCheckoutResponse> CounterCheckoutAsync(
        int bookingId,
        CounterCheckoutRequest request,
        CancellationToken cancellationToken
    )
    {
        var booking =
            await db.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy booking.");

        if (booking.Status != BookingStatus.Pending || booking.ExpiresAt <= DateTime.UtcNow)
            throw new BusinessRuleException(
                "Booking không ở trạng thái Pending hoặc đã hết hạn giữ ghế."
            );
        if (request.AmountReceived < booking.FinalAmount)
            throw new BusinessRuleException("Số tiền nhận không đủ.");

        var change = request.AmountReceived - booking.FinalAmount;

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        db.Payments.Add(
            new Payment
            {
                BookingId = booking.BookingId,
                PaymentMethod = PaymentMethod.Cash,
                Status = PaymentStatus.Success,
                Amount = booking.FinalAmount,
                AmountReceived = request.AmountReceived,
                ChangeGiven = change,
                PaidAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            }
        );
        await db.SaveChangesAsync(cancellationToken);
        await ConfirmBookingAndGenerateTicketsAsync(bookingId, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new CounterCheckoutResponse(bookingId, "Confirmed", change);
    }

    public async Task<CheckinResponse> CheckinAsync(
        CheckinRequest request,
        CancellationToken cancellationToken
    )
    {
        var ticket =
            await db
                .BookingTickets.Include(t => t.ShowtimeSeat)
                    .ThenInclude(ss => ss.Seat)
                .Include(t => t.ShowtimeSeat)
                    .ThenInclude(ss => ss.Showtime)
                        .ThenInclude(s => s.Movie)
                .Include(t => t.Booking)
                .FirstOrDefaultAsync(t => t.QrCode == request.QrCode, cancellationToken)
            ?? throw new NotFoundException("QR không hợp lệ.");

        if (ticket.IsCheckedIn)
            throw new ConflictAppException("Vé đã check-in trước đó.");

        var showtime = ticket.ShowtimeSeat.Showtime;
        var now = DateTime.UtcNow;

        if (now < showtime.StartTime.AddMinutes(-30))
            throw new BusinessRuleException("Chưa tới giờ check-in (mở trước giờ chiếu 30 phút).");
        if (now > showtime.EndTime)
            throw new BusinessRuleException("Suất chiếu đã kết thúc.");

        ticket.IsCheckedIn = true;
        ticket.CheckedInAt = now;
        await db.SaveChangesAsync(cancellationToken);

        return ticket.Adapt<CheckinResponse>();
    }

    public async Task ConfirmBookingAndGenerateTicketsAsync(
        int bookingId,
        CancellationToken cancellationToken
    )
    {
        var booking =
            await db.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy booking.");

        if (booking.Status == BookingStatus.Confirmed)
        {
            logger.LogInformation("PaymentAlreadyProcessed BookingId {BookingId}", bookingId);
            return;
        }

        var holderUserId = await ResolveHolderUserIdAsync(booking, cancellationToken);
        var heldSeats = await db
            .ShowtimeSeats.Include(ss => ss.Seat)
            .Where(ss =>
                ss.ShowtimeId == booking.ShowtimeId
                && ss.HeldByUserId == holderUserId
                && ss.Status == ShowtimeSeatStatus.Held
            )
            .ToListAsync(cancellationToken);

        if (heldSeats.Count == 0)
        {
            booking.Status = BookingStatus.Cancelled;
            await db.SaveChangesAsync(cancellationToken);
            logger.LogWarning(
                "ConfirmBookingFailed_SeatsExpired BookingId {BookingId} — ghế đã hết hạn giữ trước khi confirm.",
                bookingId
            );
            throw new SeatsNoLongerHeldException(bookingId);
        }

        var ownsConfirmTransaction = db.Database.CurrentTransaction is null;
        var transaction = ownsConfirmTransaction
            ? await db.Database.BeginTransactionAsync(cancellationToken)
            : db.Database.CurrentTransaction!;

        var seatIds = new List<int>();
        try
        {
            booking.Status = BookingStatus.Confirmed;

            foreach (var seat in heldSeats)
            {
                seat.Status = ShowtimeSeatStatus.Booked;
                seat.HeldByUserId = null;
                seat.HeldUntil = null;
                seatIds.Add(seat.ShowtimeSeatId);

                db.BookingTickets.Add(
                    new BookingTicket
                    {
                        BookingId = booking.BookingId,
                        ShowtimeSeatId = seat.ShowtimeSeatId,
                        Price = seat.Price,
                        QrCode = CodeGenerator.GenerateTicketQrPayload(
                            booking.BookingCode,
                            seat.Seat.SeatRow,
                            seat.Seat.SeatNumber
                        ),
                        IsCheckedIn = false,
                    }
                );
            }

            if (booking.UserId is not null)
                await ApplyLoyaltyAsync(
                    booking.UserId.Value,
                    booking.FinalAmount,
                    cancellationToken
                );

            await db.SaveChangesAsync(cancellationToken);
            if (ownsConfirmTransaction)
                await transaction.CommitAsync(cancellationToken);

            await realtimeNotifier.NotifySeatBookedAsync(
                booking.ShowtimeId,
                seatIds,
                cancellationToken
            );
            logger.LogInformation(
                "BookingCreated->Confirmed BookingId {BookingId} SeatsBooked {Count}",
                bookingId,
                seatIds.Count
            );
        }
        finally
        {
            if (ownsConfirmTransaction)
                await transaction.DisposeAsync();
        }

        if (booking.UserId is not null)
        {
            var email = await db
                .Users.Where(u => u.UserId == booking.UserId)
                .Select(u => u.Email)
                .FirstAsync(cancellationToken);
            var html = EmailTemplates.BookingConfirmed(
                booking.CustomerName,
                booking.BookingCode,
                movieTitle: string.Empty,
                seatCount: seatIds.Count
            );
            backgroundJobClient.Enqueue<SendEmailJob>(j =>
                j.RunAsync(
                    email,
                    "Vé xem phim của bạn — Cinema",
                    html,
                    CancellationToken.None
                )
            );
            logger.LogInformation(
                "EmailQueued BookingId {BookingId} Type TicketConfirmation",
                bookingId
            );
        }
    }

    public async Task ProcessPaymentFailureAsync(int bookingId, CancellationToken cancellationToken)
    {
        var booking =
            await db.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy booking.");

        if (booking.Status != BookingStatus.Pending)
        {
            logger.LogInformation(
                "ProcessPaymentFailure bỏ qua — BookingId {BookingId} không ở trạng thái Pending.",
                bookingId
            );
            return;
        }

        await ReleaseAndCancelBookingAsync(booking, cancellationToken);
        logger.LogInformation(
            "PaymentFailed BookingId {BookingId} — ghế đã được release.",
            bookingId
        );
    }

    public async Task ReverseLoyaltyAsync(
        int userId,
        decimal finalAmount,
        CancellationToken cancellationToken
    )
    {
        var settings = await GetCinemaSettingsAsync(cancellationToken);
        await ReverseLoyaltyAsync(userId, finalAmount, settings, cancellationToken);
    }

    private async Task ReverseLoyaltyAsync(
        int userId,
        decimal finalAmount,
        CinemaSettings settings,
        CancellationToken cancellationToken
    )
    {
        var profile = await db.CustomerProfiles.FirstOrDefaultAsync(
            p => p.UserId == userId,
            cancellationToken
        );
        if (profile is null)
            return;

        var pointsToRevoke = (int)Math.Floor(finalAmount / settings.PointsPerAmount);
        profile.Points = Math.Max(0, profile.Points - pointsToRevoke);
        profile.TotalSpent = Math.Max(0, profile.TotalSpent - finalAmount);
    }

    private async Task ReleaseAndCancelBookingAsync(
        Booking booking,
        CancellationToken cancellationToken
    )
    {
        var releasedSeatIds = await ReleaseBookingSeatsAsync(booking, cancellationToken);
        booking.Status = BookingStatus.Cancelled;

        await db
            .Payments.Where(p =>
                p.BookingId == booking.BookingId && p.Status == PaymentStatus.Pending
            )
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(p => p.Status, PaymentStatus.Failed),
                cancellationToken
            );

        await db.SaveChangesAsync(cancellationToken);

        await realtimeNotifier.NotifySeatReleasedAsync(
            booking.ShowtimeId,
            releasedSeatIds,
            cancellationToken
        );
    }

    private async Task ApplyLoyaltyAsync(
        int userId,
        decimal finalAmount,
        CancellationToken cancellationToken
    )
    {
        var profile = await db.CustomerProfiles.FirstOrDefaultAsync(
            p => p.UserId == userId,
            cancellationToken
        );
        if (profile is null)
            return;

        var settings = await GetCinemaSettingsAsync(cancellationToken);

        profile.Points += (int)Math.Floor(finalAmount / settings.PointsPerAmount);
        profile.TotalSpent += finalAmount;

        var eligibleTier = await db
            .MembershipTiers.Where(t => t.MinPoints <= profile.Points)
            .OrderByDescending(t => t.MinPoints)
            .FirstOrDefaultAsync(cancellationToken);

        if (eligibleTier is not null && eligibleTier.TierId != profile.TierId)
        {
            logger.LogInformation(
                "TierUpgraded UserId {UserId} FromTierId {From} ToTierId {To} Points {Points}",
                userId,
                profile.TierId,
                eligibleTier.TierId,
                profile.Points
            );
            profile.TierId = eligibleTier.TierId;
        }
    }

    private async Task<Booking> CreateHoldBookingAsync(
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
            ) ?? throw new NotFoundException("Không tìm thấy suất chiếu.");
        if (showtime.Status != ShowtimeStatus.Scheduled)
            throw new BusinessRuleException("Suất chiếu không còn khả dụng để đặt vé.");
        if (showtime.StartTime <= DateTime.UtcNow)
            throw new BusinessRuleException("Suất chiếu đã bắt đầu hoặc đã kết thúc.");

        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ValidationAppException(
                "Thiếu tên khách hàng.",
                new Dictionary<string, string[]> { ["customerName"] = ["customerName là bắt buộc"] }
            );
        }

        var hasActivePending = await db.Bookings.AnyAsync(
            b =>
                b.ShowtimeId == showtimeId
                && b.Status == BookingStatus.Pending
                && b.ExpiresAt > DateTime.UtcNow
                && (
                    (channel == BookingChannel.Online && b.UserId == holderUserId)
                    || (
                        channel == BookingChannel.Counter
                        && b.CreatedByEmployeeId == createdByEmployeeId
                    )
                ),
            cancellationToken
        );
        if (hasActivePending)
            throw new ConflictAppException(
                "Đang có một booking Pending khác chưa hoàn tất cho suất chiếu này."
            );

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var holdUntil = now.AddMinutes(_holdMinutes);
        var distinctSeatIds = showtimeSeatIds.Distinct().ToList();

        var affected = await db
            .ShowtimeSeats.Where(ss =>
                distinctSeatIds.Contains(ss.ShowtimeSeatId)
                && ss.ShowtimeId == showtimeId
                && ss.Status == ShowtimeSeatStatus.Available
            )
            .ExecuteUpdateAsync(
                setters =>
                    setters
                        .SetProperty(x => x.Status, ShowtimeSeatStatus.Held)
                        .SetProperty(x => x.HeldByUserId, holderUserId)
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
                "Một hoặc nhiều ghế đã bị giữ/đặt bởi người khác.",
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
        var discountAmount = await CalculateDiscountAmountAsync(
            bookingUserId,
            totalAmount,
            cancellationToken
        );
        var finalAmount = totalAmount - discountAmount;

        var booking = new Booking
        {
            UserId = bookingUserId,
            ShowtimeId = showtimeId,
            BookingCode = await GenerateUniqueBookingCodeAsync(cancellationToken),
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

    private Task<CinemaSettings> GetCinemaSettingsAsync(CancellationToken cancellationToken) =>
        db.CinemaSettings.FirstAsync(s => s.Id == 1, cancellationToken);

    private async Task<decimal> CalculateDiscountAmountAsync(
        int? bookingUserId,
        decimal totalAmount,
        CancellationToken cancellationToken
    )
    {
        if (bookingUserId is null)
            return 0m;

        var profile = await db.CustomerProfiles.FirstOrDefaultAsync(
            p => p.UserId == bookingUserId,
            cancellationToken
        );
        if (profile?.TierId is null)
            return 0m;

        var tier = await db.MembershipTiers.FirstOrDefaultAsync(
            t => t.TierId == profile.TierId,
            cancellationToken
        );
        if (tier is null)
            return 0m;

        return Math.Round(
            totalAmount * tier.DiscountPercent / 100m,
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
                "Một hoặc nhiều comboId không tồn tại hoặc đã ngừng bán.",
                new Dictionary<string, string[]>
                {
                    ["combos"] = ["comboId không hợp lệ hoặc đã ngừng bán."],
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

    private async Task<List<int>> ReleaseBookingSeatsAsync(
        Booking booking,
        CancellationToken cancellationToken
    )
    {
        var holderUserId = await ResolveHolderUserIdAsync(booking, cancellationToken);
        var seats = await db
            .ShowtimeSeats.Where(ss =>
                ss.ShowtimeId == booking.ShowtimeId
                && ss.HeldByUserId == holderUserId
                && ss.Status == ShowtimeSeatStatus.Held
            )
            .ToListAsync(cancellationToken);

        var seatIds = seats.Select(s => s.ShowtimeSeatId).ToList();
        foreach (var seat in seats)
        {
            seat.Status = ShowtimeSeatStatus.Available;
            seat.HeldByUserId = null;
            seat.HeldUntil = null;
        }
        return seatIds;
    }

    private async Task<List<int>> ReleaseConfirmedBookingSeatsAsync(
        Booking booking,
        CancellationToken cancellationToken
    )
    {
        var seatIds = await db
            .BookingTickets.Where(t => t.BookingId == booking.BookingId)
            .Select(t => t.ShowtimeSeatId)
            .ToListAsync(cancellationToken);

        var seats = await db
            .ShowtimeSeats.Where(ss =>
                seatIds.Contains(ss.ShowtimeSeatId) && ss.Status == ShowtimeSeatStatus.Booked
            )
            .ToListAsync(cancellationToken);

        foreach (var seat in seats)
        {
            seat.Status = ShowtimeSeatStatus.Available;
            seat.HeldByUserId = null;
            seat.HeldUntil = null;
        }

        return seats.Select(s => s.ShowtimeSeatId).ToList();
    }

    private async Task<List<BookingSeatDto>> GetBookingSeatsAsync(
        Booking booking,
        CancellationToken cancellationToken
    )
    {
        if (booking.Status == BookingStatus.Pending)
        {
            var holderUserId = await ResolveHolderUserIdAsync(booking, cancellationToken);
            return await db
                .ShowtimeSeats.Where(ss =>
                    ss.ShowtimeId == booking.ShowtimeId
                    && ss.HeldByUserId == holderUserId
                    && ss.Status == ShowtimeSeatStatus.Held
                )
                .ProjectToType<BookingSeatDto>()
                .ToListAsync(cancellationToken);
        }

        return await db
            .BookingTickets.Where(t => t.BookingId == booking.BookingId)
            .ProjectToType<BookingSeatDto>()
            .ToListAsync(cancellationToken);
    }

    private async Task<List<BookingTicketDto>> GetBookingTicketsAsync(
        Booking booking,
        CancellationToken cancellationToken
    )
    {
        if (booking.Status != BookingStatus.Confirmed)
            return [];

        return await db
            .BookingTickets.Where(t => t.BookingId == booking.BookingId)
            .OrderBy(t => t.ShowtimeSeat.Seat.SeatRow)
            .ThenBy(t => t.ShowtimeSeat.Seat.SeatNumber)
            .ProjectToType<BookingTicketDto>()
            .ToListAsync(cancellationToken);
    }

    private async Task<List<BookingComboItemDto>> GetBookingCombosAsync(
        int bookingId,
        CancellationToken cancellationToken
    ) =>
        await db
            .BookingCombos.Where(bc => bc.BookingId == bookingId)
            .ProjectToType<BookingComboItemDto>()
            .ToListAsync(cancellationToken);

    private async Task<int> ResolveHolderUserIdAsync(
        Booking booking,
        CancellationToken cancellationToken
    )
    {
        if (booking.UserId is not null)
            return booking.UserId.Value;
        if (booking.CreatedByEmployeeId is not null)
        {
            var employee = await db.Employees.FirstAsync(
                e => e.EmployeeId == booking.CreatedByEmployeeId,
                cancellationToken
            );
            return employee.UserId;
        }
        throw new BusinessRuleException("Booking thiếu cả UserId lẫn CreatedByEmployeeId.");
    }

    private async Task<List<BookingPaymentDto>> GetBookingPaymentsAsync(
        int bookingId,
        CancellationToken cancellationToken
    ) =>
        await db
            .Payments.Where(p => p.BookingId == bookingId)
            .OrderByDescending(p => p.CreatedAt)
            .ProjectToType<BookingPaymentDto>()
            .ToListAsync(cancellationToken);

    private async Task<Booking> LoadBookingOrThrowAsync(
        int bookingId,
        CancellationToken cancellationToken
    ) =>
        await db
            .Bookings.Include(b => b.Showtime)
                .ThenInclude(s => s.Movie)
            .Include(b => b.Showtime)
                .ThenInclude(s => s.Room)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken)
        ?? throw new NotFoundException("Không tìm thấy booking.");

    private async Task<string> GenerateUniqueBookingCodeAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var code = CodeGenerator.GenerateBookingCode();
            if (!await db.Bookings.AnyAsync(b => b.BookingCode == code, cancellationToken))
                return code;
        }
        throw new BusinessRuleException("Không thể sinh mã booking duy nhất, vui lòng thử lại.");
    }
}

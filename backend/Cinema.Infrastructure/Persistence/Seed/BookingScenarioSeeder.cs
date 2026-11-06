using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema.Infrastructure.Persistence.Seed;

internal static class BookingScenarioSeeder
{
    internal const string CodePrefix = "BKD";

    internal const int ReviewReadyPerUser = 3;

    public static async Task SeedAsync(
        AppDbContext db,
        ILogger logger,
        DateTime now,
        IReadOnlyDictionary<string, Showtime> specials,
        CancellationToken cancellationToken
    )
    {
        var runner = new Runner(db, logger, now);
        await runner.RunAsync(specials, cancellationToken);
    }

    private enum CheckIn
    {
        None,
        All,
        First,
    }

    private enum CancelKind
    {
        None,
        FromPending,
        RefundedByUser,
        LatePaymentRefund,
        ShowtimeCancelledRefunded,
        ShowtimeCancelledManual,
    }

    private sealed class ShowtimeCtx
    {
        public Showtime Showtime { get; init; } = null!;
        public List<ShowtimeSeat> Seats { get; init; } = [];
        public HashSet<int> Claimed { get; } = [];

        public bool IsFree(ShowtimeSeat seat) =>
            seat.Status == ShowtimeSeatStatus.Available && !Claimed.Contains(seat.ShowtimeSeatId);
    }

    private sealed class Plan
    {
        public ShowtimeCtx St { get; init; } = null!;
        public BookingChannel Channel { get; init; }
        public User? User { get; init; }
        public Employee? Employee { get; init; }
        public string CustomerName { get; init; } = "";
        public string? CustomerPhone { get; init; }
        public List<ShowtimeSeat> Seats { get; init; } = [];
        public (int ComboId, int Qty)[] Combos { get; init; } = [];
        public BookingStatus Status { get; init; }
        public CancelKind Cancel { get; init; }
        public DateTime CreatedAt { get; set; }
        public PaymentMethod? Method { get; init; }
        public bool PaymentRecord { get; init; }
        public string? FailCode { get; init; }
        public CheckIn CheckIn { get; init; }

        public bool ClaimsSeats =>
            Status is BookingStatus.Confirmed or BookingStatus.Pending
            || Cancel is CancelKind.ShowtimeCancelledRefunded or CancelKind.ShowtimeCancelledManual;
    }

    private sealed record ReviewCandidate(int UserId, int MovieId, DateTime ShowtimeEnd);

    private sealed class Runner(AppDbContext db, ILogger logger, DateTime now)
    {
        private readonly Random _rng = new(20261028);
        private readonly List<Plan> _plans = [];
        private readonly List<ReviewCandidate> _reviewCandidates = [];

        // (khách, phim) đã check-in nhưng cố tình KHÔNG seed review để khách đánh giá được ngay.
        private readonly HashSet<(int UserId, int MovieId)> _reviewReady = [];
        private HashSet<int> _nowShowingMovieIds = [];
        private int _seq;

        private CinemaSettings _settings = null!;
        private List<MembershipTier> _tiersDesc = [];
        private Dictionary<int, MembershipTier> _tiersById = [];
        private List<Combo> _combos = [];
        private Dictionary<string, User> _users = [];
        private List<User> _pool = [];
        private Dictionary<int, CustomerProfile> _profiles = [];
        private List<Employee> _employees = [];
        private List<Employee> _activeEmployees = [];

        private static readonly PaymentMethod[] OnlineMethods =
        [
            PaymentMethod.VNPay,
            PaymentMethod.Momo,
            PaymentMethod.Stripe,
        ];

        // ───────────────────────────── Chạy ─────────────────────────────

        public async Task RunAsync(
            IReadOnlyDictionary<string, Showtime> specials,
            CancellationToken cancellationToken
        )
        {
            if (!await LoadReferenceDataAsync(cancellationToken))
                return;

            var contexts = await LoadShowtimesAsync(specials, cancellationToken);
            var specialIds = specials.Values.Select(s => s.ShowtimeId).ToHashSet();

            BuildSpecialScenarios(specials, contexts);
            BuildUpcomingScenarios(contexts, specialIds);
            BuildReviewReadyScenarios(contexts, specialIds); // trước lấp ghế ngẫu nhiên để còn ghế trống ở suất đã chiếu
            BuildRandomOccupancy(contexts, specialIds);

            // Xử lý theo thứ tự thời gian để giảm giá/hạng thành viên khớp thời điểm đặt.
            foreach (var plan in _plans.OrderBy(p => p.CreatedAt).ThenBy(p => p.St.Showtime.StartTime))
                Apply(plan);

            await db.SaveChangesAsync(cancellationToken);
            await SeedReviewsAsync(cancellationToken);

            logger.LogInformation(
                "Review-ready: {Pairs} cặp (khách, phim) đã check-in và chưa có review.",
                _reviewReady.Count
            );
            logger.LogInformation(
                "Seeded {Bookings} bookings (confirmed {Confirmed}, pending {Pending}, expired {Expired}, cancelled {Cancelled}).",
                _plans.Count,
                _plans.Count(p => p.Status == BookingStatus.Confirmed),
                _plans.Count(p => p.Status == BookingStatus.Pending),
                _plans.Count(p => p.Status == BookingStatus.Expired),
                _plans.Count(p => p.Status == BookingStatus.Cancelled)
            );
        }

        private async Task<bool> LoadReferenceDataAsync(CancellationToken ct)
        {
            var settings = await db.CinemaSettings.FirstOrDefaultAsync(ct);
            if (settings is null)
            {
                logger.LogWarning("Skip booking scenarios: thiếu CinemaSettings.");
                return false;
            }
            _settings = settings;

            _tiersDesc = await db.MembershipTiers.OrderByDescending(t => t.MinPoints).ToListAsync(ct);
            _tiersById = _tiersDesc.ToDictionary(t => t.TierId);
            _combos = await db.Combos.OrderBy(c => c.ComboId).ToListAsync(ct);
            _nowShowingMovieIds = (
                await db.Movies.Where(m => m.Status == MovieStatus.NowShowing).Select(m => m.MovieId).ToListAsync(ct)
            ).ToHashSet();

            var emails = new[]
            {
                SeedEmails.Customer1,
                SeedEmails.Customer2,
                SeedEmails.Customer3,
                SeedEmails.Customer4,
                SeedEmails.CustomerGoogle,
            };
            _users = await db.Users.Where(u => emails.Contains(u.Email)).ToDictionaryAsync(u => u.Email, ct);
            _pool = emails.Where(_users.ContainsKey).Select(e => _users[e]).ToList();

            var poolIds = _pool.Select(u => u.UserId).ToList();
            _profiles = await db
                .CustomerProfiles.Where(p => poolIds.Contains(p.UserId))
                .ToDictionaryAsync(p => p.UserId, ct);

            _employees = await db.Employees.Include(e => e.User).OrderBy(e => e.EmployeeId).ToListAsync(ct);
            _activeEmployees = _employees.Where(e => e.IsActive && e.User.IsActive).ToList();

            if (_pool.Count < 4 || _activeEmployees.Count == 0 || _combos.Count == 0)
            {
                logger.LogWarning("Skip booking scenarios: thiếu khách hàng / nhân viên / combo seed.");
                return false;
            }

            return true;
        }

        private async Task<Dictionary<int, ShowtimeCtx>> LoadShowtimesAsync(
            IReadOnlyDictionary<string, Showtime> specials,
            CancellationToken ct
        )
        {
            var from = ShowtimeSeeder.LocalDayStartUtc(now, -ShowtimeSeeder.PastDays - 1);
            var to = ShowtimeSeeder.LocalDayStartUtc(now, 5);
            var specialIds = specials.Values.Select(s => s.ShowtimeId).ToList();

            var showtimes = await db
                .Showtimes.Include(s => s.ShowtimeSeats)
                    .ThenInclude(ss => ss.Seat)
                .Where(s =>
                    (s.StartTime >= from && s.StartTime < to && s.Status != ShowtimeStatus.Cancelled)
                    || specialIds.Contains(s.ShowtimeId)
                )
                .AsSplitQuery()
                .ToListAsync(ct);

            return showtimes.ToDictionary(
                s => s.ShowtimeId,
                s => new ShowtimeCtx
                {
                    Showtime = s,
                    Seats = s
                        .ShowtimeSeats.Where(ss => ss.Seat != null)
                        .OrderBy(ss => ss.Seat.SeatRow)
                        .ThenBy(ss => ss.Seat.SeatNumber)
                        .ToList(),
                }
            );
        }

        // ───────────────────────── Kịch bản: suất đặc biệt ─────────────────────────

        private void BuildSpecialScenarios(
            IReadOnlyDictionary<string, Showtime> specials,
            Dictionary<int, ShowtimeCtx> contexts
        )
        {
            var c1 = U(SeedEmails.Customer1);
            var c2 = U(SeedEmails.Customer2);
            var c3 = U(SeedEmails.Customer3);
            var c4 = U(SeedEmails.Customer4);
            var staff = Emp(SeedEmails.Employee);
            var manager = Emp(SeedEmails.Manager);
            var shiftLead = Emp(SeedEmails.ShiftLead);

            // 1) ĐANG CHIẾU — check-in đang mở, có vé đã check-in / chưa check-in.
            if (TryCtx(specials, contexts, ShowtimeSeeder.SpecialOngoing, out var ongoing))
            {
                var start = ongoing.Showtime.StartTime;
                Online(ongoing, c4, Pick(ongoing, 2, 2), BookingStatus.Confirmed, start.AddHours(-3), PaymentMethod.Stripe,
                    checkIn: CheckIn.First, combos: [(3, 1)]);
                Counter(ongoing, staff, null, Pick(ongoing, 2, 1), BookingStatus.Confirmed, start.AddMinutes(-15));
                Online(ongoing, c1, PickCouple(ongoing), BookingStatus.Confirmed, start.AddDays(-1), PaymentMethod.VNPay,
                    checkIn: CheckIn.All);
                Online(ongoing, c2, Pick(ongoing, 1, 1), BookingStatus.Confirmed, start.AddHours(-1), PaymentMethod.Momo);
                FillRandom(ongoing, 0.30, past: false);
            }

            // 2) SẮP CHIẾU (20 phút nữa) — check-in mở, đã quá hạn hoàn tiền, 1 booking Pending đang giữ ghế.
            if (TryCtx(specials, contexts, ShowtimeSeeder.SpecialStartingSoon, out var soon))
            {
                Online(soon, c3, Pick(soon, 2, 1), BookingStatus.Confirmed, now.AddDays(-1), PaymentMethod.Momo);
                Counter(soon, staff, null, Pick(soon, 3, 1), BookingStatus.Confirmed, now.AddMinutes(-30));
                Online(soon, c2, Pick(soon, 2, 1), BookingStatus.Pending, now.AddMinutes(-7), PaymentMethod.VNPay);
                FillRandom(soon, 0.25, past: false);
            }

            // 3) 90 PHÚT NỮA — trong ngưỡng CancelDeadlineHours nên không được hoàn tiền; có hold của quầy.
            if (TryCtx(specials, contexts, ShowtimeSeeder.SpecialRefundClosed, out var closed))
            {
                Online(closed, c1, Pick(closed, 2, 1), BookingStatus.Confirmed, now.AddDays(-2), PaymentMethod.Stripe);
                Counter(closed, manager, null, Pick(closed, 4, 1), BookingStatus.Confirmed, now.AddHours(-2));
                Counter(closed, shiftLead, null, Pick(closed, 2, 1), BookingStatus.Pending, now.AddMinutes(-3));
                FillRandom(closed, 0.30, past: false);
            }

            // 4) HẾT VÉ 100%.
            if (TryCtx(specials, contexts, ShowtimeSeeder.SpecialSoldOut, out var soldOut))
            {
                var methodCycle = 0;
                var guard = 0;
                while (guard++ < 300)
                {
                    var seats =
                        PickCouple(soldOut)
                        ?? PickAdjacent(soldOut, 4, null)
                        ?? PickAdjacent(soldOut, 3, null)
                        ?? PickAdjacent(soldOut, 2, null)
                        ?? PickAdjacent(soldOut, 1, null);
                    if (seats is null)
                        break;

                    var created = now.AddMinutes(-_rng.Next(60, 60 * 72));
                    if (guard % 3 == 0)
                    {
                        var user = _pool[guard % _pool.Count];
                        var method = OnlineMethods[methodCycle++ % OnlineMethods.Length];
                        Online(soldOut, user, seats, BookingStatus.Confirmed, created, method);
                    }
                    else
                    {
                        var emp = _activeEmployees[guard % _activeEmployees.Count];
                        Counter(soldOut, emp, null, seats, BookingStatus.Confirmed, created);
                    }
                }
            }

            // 5) SUẤT BỊ HỦY BỞI RẠP — booking bị hủy: hoàn tự động (cổng), hoàn thủ công (Cash), hold bị hủy.
            if (TryCtx(specials, contexts, ShowtimeSeeder.SpecialCancelled, out var cancelled))
            {
                Online(cancelled, c3, Pick(cancelled, 2, null), BookingStatus.Cancelled, now.AddDays(-3), PaymentMethod.Stripe,
                    cancel: CancelKind.ShowtimeCancelledRefunded);
                Online(cancelled, c2, Pick(cancelled, 2, null), BookingStatus.Cancelled, now.AddDays(-2), PaymentMethod.VNPay,
                    cancel: CancelKind.ShowtimeCancelledRefunded);
                Counter(cancelled, staff, null, Pick(cancelled, 2, null), BookingStatus.Cancelled, now.AddDays(-1),
                    cancel: CancelKind.ShowtimeCancelledManual);
                Online(cancelled, c1, Pick(cancelled, 1, null), BookingStatus.Cancelled, now.AddHours(-5), PaymentMethod.Momo,
                    cancel: CancelKind.FromPending, paymentRecord: false);
            }
        }

        // ───────────────────────── Kịch bản: suất tương lai thường ─────────────────────────

        private void BuildUpcomingScenarios(Dictionary<int, ShowtimeCtx> contexts, HashSet<int> specialIds)
        {
            var upcoming = contexts
                .Values.Where(c =>
                    c.Showtime.Status == ShowtimeStatus.Scheduled
                    && c.Showtime.StartTime >= now.AddHours(6)
                    && !specialIds.Contains(c.Showtime.ShowtimeId)
                )
                .OrderBy(c => c.Showtime.StartTime)
                .ThenBy(c => c.Showtime.RoomId)
                .ToList();
            if (upcoming.Count == 0)
                return;

            var c1 = U(SeedEmails.Customer1);
            var c2 = U(SeedEmails.Customer2);
            var c3 = U(SeedEmails.Customer3);
            var c4 = U(SeedEmails.Customer4);
            var manager = Emp(SeedEmails.Manager);
            var shiftLead = Emp(SeedEmails.ShiftLead);

            // u0: các booking Confirmed của khách theo từng hạng (giảm 5% / 10% / 15%) + 1 Pending đang giữ ghế (Stripe).
            var u0 = upcoming[0];
            Online(u0, c2, Pick(u0, 2, 2), BookingStatus.Confirmed, now.AddHours(-5), PaymentMethod.VNPay, combos: [(1, 2)]);
            Online(u0, c3, Pick(u0, 2, 1), BookingStatus.Confirmed, now.AddDays(-1), PaymentMethod.Stripe);
            Online(u0, c4, PickCouple(u0), BookingStatus.Confirmed, now.AddDays(-2), PaymentMethod.Momo,
                combos: [(4, 1), (5, 1)]);
            Online(u0, c1, Pick(u0, 2, 1), BookingStatus.Pending, now.AddMinutes(-6), PaymentMethod.Stripe);

            // u1: Pending quầy, Confirmed quầy cho thành viên (có giảm giá), huỷ khi Pending, hết hạn, thanh toán thất bại.
            var u1 = upcoming[Math.Min(3, upcoming.Count - 1)];
            Counter(u1, shiftLead, null, Pick(u1, 3, 1), BookingStatus.Pending, now.AddMinutes(-4));
            Counter(u1, manager, c2, Pick(u1, 2, 2), BookingStatus.Confirmed, now.AddHours(-3), combos: [(2, 1)]);
            Online(u1, c1, Pick(u1, 2, 1), BookingStatus.Cancelled, now.AddHours(-2), PaymentMethod.VNPay,
                cancel: CancelKind.FromPending, failCode: "24");
            Online(u1, c4, Pick(u1, 2, 1), BookingStatus.Expired, now.AddHours(-3), PaymentMethod.Momo);
            Online(u1, c3, Pick(u1, 1, 1), BookingStatus.Cancelled, now.AddMinutes(-90), PaymentMethod.Stripe,
                cancel: CancelKind.FromPending, paymentRecord: false);

            // u2: huỷ sau thanh toán (hoàn tiền Stripe / Momo) và thanh toán xong nhưng ghế đã hết hạn giữ (tự hoàn).
            var u2 = upcoming[Math.Min(6, upcoming.Count - 1)];
            Online(u2, c3, Pick(u2, 2, 1), BookingStatus.Cancelled, now.AddDays(-1), PaymentMethod.Stripe,
                cancel: CancelKind.RefundedByUser);
            Online(u2, c4, Pick(u2, 2, 2), BookingStatus.Cancelled, now.AddDays(-2), PaymentMethod.Momo,
                cancel: CancelKind.RefundedByUser);
            Online(u2, c2, Pick(u2, 2, 1), BookingStatus.Cancelled, now.AddHours(-4), PaymentMethod.VNPay,
                cancel: CancelKind.LatePaymentRefund);
        }

        // ───────────────────────── Kịch bản: khách đã xem, chưa review ─────────────────────────

        /// <summary>
        /// Với mỗi khách trong pool, tạo sẵn <see cref="ReviewReadyPerUser"/> booking Confirmed đã thanh toán và
        /// check-in ở các suất Completed của những phim khác nhau (ưu tiên phim đang chiếu). Các cặp (khách, phim)
        /// này bị loại khỏi <see cref="SeedReviewsAsync"/> nên khách vào trang phim là thấy form đánh giá ngay,
        /// khớp điều kiện của ReviewService (có BookingTicket.IsCheckedIn của phim và chưa review).
        /// Mỗi khách bắt đầu ở một vị trí khác nhau trong danh sách phim để bộ phim được review không trùng hệt nhau.
        /// </summary>
        private void BuildReviewReadyScenarios(Dictionary<int, ShowtimeCtx> contexts, HashSet<int> specialIds)
        {
            var moviesWithPastShows = contexts
                .Values.Where(c =>
                    c.Showtime.Status == ShowtimeStatus.Completed
                    && c.Showtime.StartTime < now
                    && !specialIds.Contains(c.Showtime.ShowtimeId)
                )
                .GroupBy(c => c.Showtime.MovieId)
                .OrderByDescending(g => _nowShowingMovieIds.Contains(g.Key))
                .ThenBy(g => g.Key)
                .ToList();
            if (moviesWithPastShows.Count == 0)
                return;

            for (var k = 0; k < _pool.Count; k++)
            {
                var user = _pool[k];
                var assigned = 0;

                for (var j = 0; j < moviesWithPastShows.Count && assigned < ReviewReadyPerUser; j++)
                {
                    var movie = moviesWithPastShows[(k * 2 + j) % moviesWithPastShows.Count];

                    // Suất gần đây nhất còn ghế trống.
                    foreach (var st in movie.OrderByDescending(c => c.Showtime.StartTime))
                    {
                        var seats = Pick(st, 2, null) ?? Pick(st, 1, null);
                        if (seats is null)
                            continue;

                        var createdAt = st.Showtime.StartTime.AddHours(-_rng.Next(2, 48));
                        Online(st, user, seats, BookingStatus.Confirmed, createdAt,
                            OnlineMethods[(k + assigned) % OnlineMethods.Length], checkIn: CheckIn.All);

                        _reviewReady.Add((user.UserId, movie.Key));
                        assigned++;
                        break;
                    }
                }
            }
        }

        // ───────────────────────── Kịch bản: lấp ghế ngẫu nhiên ─────────────────────────

        private void BuildRandomOccupancy(Dictionary<int, ShowtimeCtx> contexts, HashSet<int> specialIds)
        {
            foreach (var ctx in contexts.Values.OrderBy(c => c.Showtime.StartTime).ThenBy(c => c.Showtime.RoomId))
            {
                var showtime = ctx.Showtime;
                if (specialIds.Contains(showtime.ShowtimeId))
                    continue;

                if (showtime.Status == ShowtimeStatus.Completed && showtime.StartTime < now)
                {
                    FillRandom(ctx, 0.15 + _rng.NextDouble() * 0.60, past: true);
                }
                else if (
                    showtime.Status == ShowtimeStatus.Scheduled
                    && showtime.StartTime > now
                    && showtime.StartTime < now.AddDays(4)
                    && _rng.NextDouble() < 0.7
                )
                {
                    FillRandom(ctx, _rng.NextDouble() * 0.5, past: false);
                }
            }
        }

        private void FillRandom(ShowtimeCtx st, double fraction, bool past)
        {
            var target = (int)(st.Seats.Count * fraction);
            var guard = 0;
            while (st.Claimed.Count < target && guard++ < 80)
            {
                if (!AddRandomPlan(st, past))
                    break;
            }
        }

        private bool AddRandomPlan(ShowtimeCtx st, bool past)
        {
            var showtime = st.Showtime;

            List<ShowtimeSeat>? seats = null;
            if (_rng.NextDouble() < 0.2)
                seats = PickCouple(st);
            if (seats is null)
            {
                var count = _rng.NextDouble() switch
                {
                    < 0.2 => 1,
                    < 0.6 => 2,
                    < 0.8 => 3,
                    _ => 4,
                };
                seats = PickAdjacent(st, count, null) ?? PickAdjacent(st, 1, null);
            }
            if (seats is null)
                return false;

            var online = _rng.NextDouble() < 0.4;
            var combos = RandomCombos();

            DateTime createdAt;
            if (past)
            {
                createdAt = online
                    ? showtime.StartTime.AddMinutes(-(_rng.NextDouble() < 0.25 ? _rng.Next(20, 180) : _rng.Next(180, 60 * 72)))
                    : showtime.StartTime.AddMinutes(-_rng.Next(5, 120));
            }
            else
            {
                createdAt = online
                    ? now.AddMinutes(-_rng.Next(30, 60 * 48))
                    : now.AddMinutes(-_rng.Next(15, 600));
            }

            var status = BookingStatus.Confirmed;
            var cancel = CancelKind.None;
            var paymentRecord = true;
            string? failCode = null;

            if (past)
            {
                var roll = _rng.NextDouble();
                if (online)
                {
                    if (roll < 0.06 && showtime.StartTime - createdAt >= TimeSpan.FromHours(3))
                    {
                        status = BookingStatus.Cancelled;
                        cancel = CancelKind.RefundedByUser;
                    }
                    else if (roll < 0.09)
                    {
                        status = BookingStatus.Expired;
                        paymentRecord = _rng.NextDouble() < 0.7;
                    }
                    else if (roll < 0.11)
                    {
                        status = BookingStatus.Cancelled;
                        cancel = CancelKind.FromPending;
                    }
                }
                else if (roll < 0.04)
                {
                    status = BookingStatus.Expired;
                    paymentRecord = false;
                }
            }

            var checkIn = CheckIn.None;
            if (status == BookingStatus.Confirmed && past)
                checkIn = _rng.NextDouble() < 0.88 ? CheckIn.All : CheckIn.None; // ~12% no-show

            // Khách thành viên: online luôn có user; quầy 15% gắn thành viên.
            var user =
                online || _rng.NextDouble() < 0.15 ? _pool[_rng.Next(_pool.Count)] : null;

            Plan plan;
            if (online)
            {
                var method = OnlineMethods[_rng.Next(OnlineMethods.Length)];
                plan = NewPlan(st, BookingChannel.Online, user, null, seats, status, createdAt, method,
                    checkIn, cancel, combos, paymentRecord, failCode);
            }
            else
            {
                var pool = past ? _employees : _activeEmployees;
                var emp = pool[_rng.Next(pool.Count)];
                plan = NewPlan(st, BookingChannel.Counter, user, emp, seats, status, createdAt,
                    PaymentMethod.Cash, checkIn, cancel, combos, paymentRecord: false, failCode);
            }

            Register(plan);
            return true;
        }

        // ───────────────────────── Tạo plan ─────────────────────────

        private User U(string email) => _users.TryGetValue(email, out var u) ? u : _pool[0];

        private Employee Emp(string email) =>
            _activeEmployees.FirstOrDefault(e => e.User.Email == email) ?? _activeEmployees[0];

        private static bool TryCtx(
            IReadOnlyDictionary<string, Showtime> specials,
            Dictionary<int, ShowtimeCtx> contexts,
            string key,
            out ShowtimeCtx ctx
        )
        {
            if (
                specials.TryGetValue(key, out var showtime)
                && contexts.TryGetValue(showtime.ShowtimeId, out var found)
            )
            {
                ctx = found;
                return true;
            }

            ctx = null!;
            return false;
        }

        private void Online(
            ShowtimeCtx st,
            User user,
            List<ShowtimeSeat>? seats,
            BookingStatus status,
            DateTime createdAt,
            PaymentMethod method,
            CheckIn checkIn = CheckIn.None,
            CancelKind cancel = CancelKind.None,
            (int ComboId, int Qty)[]? combos = null,
            bool paymentRecord = true,
            string? failCode = null
        )
        {
            if (seats is null)
                return;

            Register(
                NewPlan(st, BookingChannel.Online, user, null, seats, status, createdAt, method,
                    checkIn, cancel, combos ?? [], paymentRecord, failCode)
            );
        }

        private void Counter(
            ShowtimeCtx st,
            Employee employee,
            User? member,
            List<ShowtimeSeat>? seats,
            BookingStatus status,
            DateTime createdAt,
            CheckIn checkIn = CheckIn.None,
            CancelKind cancel = CancelKind.None,
            (int ComboId, int Qty)[]? combos = null
        )
        {
            if (seats is null)
                return;

            Register(
                NewPlan(st, BookingChannel.Counter, member, employee, seats, status, createdAt,
                    PaymentMethod.Cash, checkIn, cancel, combos ?? [], paymentRecord: false, failCode: null)
            );
        }

        private Plan NewPlan(
            ShowtimeCtx st,
            BookingChannel channel,
            User? user,
            Employee? employee,
            List<ShowtimeSeat> seats,
            BookingStatus status,
            DateTime createdAt,
            PaymentMethod method,
            CheckIn checkIn,
            CancelKind cancel,
            (int ComboId, int Qty)[] combos,
            bool paymentRecord,
            string? failCode
        ) =>
            new()
            {
                St = st,
                Channel = channel,
                User = user,
                Employee = employee,
                CustomerName = user?.FullName ?? RandomName(),
                CustomerPhone = user?.Phone ?? (_rng.NextDouble() < 0.25 ? null : RandomPhone()),
                Seats = seats,
                Combos = combos,
                Status = status,
                Cancel = cancel,
                CreatedAt = createdAt,
                Method = method,
                PaymentRecord = paymentRecord,
                FailCode = failCode,
                CheckIn = checkIn,
            };

        private void Register(Plan plan)
        {
            // Không bao giờ tạo booking sau giờ chiếu (BookingService từ chối).
            var start = plan.St.Showtime.StartTime;
            if (plan.CreatedAt >= start)
                plan.CreatedAt = start.AddMinutes(-5);
            if (plan.CreatedAt > now)
                plan.CreatedAt = now.AddMinutes(-1);

            if (plan.ClaimsSeats)
            {
                foreach (var seat in plan.Seats)
                    plan.St.Claimed.Add(seat.ShowtimeSeatId);
            }

            _plans.Add(plan);
        }

        // ───────────────────────── Chọn ghế ─────────────────────────

        private List<ShowtimeSeat>? Pick(ShowtimeCtx st, int count, int? seatTypeId) =>
            PickAdjacent(st, count, seatTypeId) ?? PickAdjacent(st, count, null);

        /// <summary>Chọn <paramref name="count"/> ghế đơn liền kề cùng hàng (không qua lối đi), còn trống.</summary>
        private List<ShowtimeSeat>? PickAdjacent(ShowtimeCtx st, int count, int? seatTypeId)
        {
            var rows = st
                .Seats.Where(s =>
                    s.Seat.CoupleId is null
                    && st.IsFree(s)
                    && (seatTypeId is null || s.Seat.SeatTypeId == seatTypeId)
                )
                .GroupBy(s => s.Seat.SeatRow)
                .OrderBy(_ => _rng.Next())
                .ToList();

            foreach (var row in rows)
            {
                var ordered = row.OrderBy(s => s.Seat.SeatNumber).ToList();
                var starts = Enumerable
                    .Range(0, Math.Max(0, ordered.Count - count + 1))
                    .OrderBy(_ => _rng.Next())
                    .ToList();

                foreach (var i in starts)
                {
                    var window = ordered.Skip(i).Take(count).ToList();
                    if (
                        window.Count == count
                        && window[^1].Seat.SeatNumber - window[0].Seat.SeatNumber == count - 1
                    )
                        return window;
                }
            }

            return null;
        }

        /// <summary>Chọn 1 cặp ghế đôi (cùng CoupleId) mà cả hai ghế còn trống.</summary>
        private List<ShowtimeSeat>? PickCouple(ShowtimeCtx st)
        {
            var pairs = st
                .Seats.Where(s => s.Seat.CoupleId is not null && st.IsFree(s))
                .GroupBy(s => s.Seat.CoupleId!.Value)
                .Where(g => g.Count() == 2)
                .ToList();
            if (pairs.Count == 0)
                return null;

            return pairs[_rng.Next(pairs.Count)].OrderBy(s => s.Seat.SeatNumber).ToList();
        }

        private (int ComboId, int Qty)[] RandomCombos()
        {
            if (_rng.NextDouble() >= 0.45)
                return [];

            var count = _rng.Next(1, 3);
            return _combos
                .OrderBy(_ => _rng.Next())
                .Take(count)
                .Select(c => (c.ComboId, _rng.Next(1, 3)))
                .ToArray();
        }

        // ───────────────────────── Áp plan -> entity ─────────────────────────

        private void Apply(Plan p)
        {
            var showtime = p.St.Showtime;
            var code = $"{CodePrefix}{p.CreatedAt:yyyyMMdd}{++_seq:D4}";

            // Combo
            decimal comboTotal = 0m;
            var comboRows = new List<BookingCombo>();
            foreach (var (comboId, qty) in p.Combos)
            {
                var combo = _combos.First(c => c.ComboId == comboId);
                comboTotal += combo.Price * qty;
                comboRows.Add(
                    new BookingCombo
                    {
                        ComboId = comboId,
                        Quantity = qty,
                        UnitPrice = combo.Price,
                    }
                );
            }

            var seatTotal = p.Seats.Sum(s => s.Price);
            var total = seatTotal + comboTotal;

            // Giảm giá theo hạng thành viên tại thời điểm đặt, CHỈ trên tiền vé (giống CalculateDiscountAmountAsync).
            CustomerProfile? profile = null;
            var discount = 0m;
            if (
                p.User is not null
                && _profiles.TryGetValue(p.User.UserId, out profile)
                && profile.TierId is { } tierId
                && _tiersById.TryGetValue(tierId, out var tier)
            )
            {
                discount = Math.Round(
                    seatTotal * tier.DiscountPercent / 100m,
                    0,
                    MidpointRounding.AwayFromZero
                );
            }

            var final = total - discount;

            var booking = new Booking
            {
                UserId = p.User?.UserId,
                ShowtimeId = showtime.ShowtimeId,
                BookingCode = code,
                Channel = p.Channel,
                CreatedByEmployeeId = p.Employee?.EmployeeId,
                CustomerName = p.CustomerName,
                CustomerPhone = p.CustomerPhone,
                TotalAmount = total,
                DiscountAmount = discount,
                FinalAmount = final,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                ExpiresAt = p.CreatedAt.AddMinutes(_settings.HoldDurationMinutes),
            };
            foreach (var row in comboRows)
                booking.BookingCombos.Add(row);
            db.Bookings.Add(booking);

            switch (p.Status)
            {
                case BookingStatus.Confirmed:
                    ApplyConfirmed(p, booking, final, profile);
                    break;
                case BookingStatus.Pending:
                    ApplyPending(p, booking, final);
                    break;
                case BookingStatus.Expired:
                    if (p.PaymentRecord && p.Method is { } m && m != PaymentMethod.Cash)
                        AddPayment(booking, m, final, PaymentStatus.Failed, p.CreatedAt.AddMinutes(1), null, TimeoutCode(m));
                    break;
                case BookingStatus.Cancelled:
                    ApplyCancelled(p, booking, final);
                    break;
            }
        }

        private void ApplyConfirmed(Plan p, Booking booking, decimal final, CustomerProfile? profile)
        {
            IssueTickets(p, booking);

            var method = p.Method ?? PaymentMethod.Cash;
            var paidAt = p.CreatedAt.AddMinutes(method == PaymentMethod.Cash ? 1 : 2);
            AddPayment(booking, method, final, PaymentStatus.Success, p.CreatedAt.AddMinutes(1), paidAt, SuccessCode(method));

            // Cộng điểm / tổng chi tiêu / nâng hạng (giống ApplyLoyaltyAsync).
            if (profile is not null)
            {
                var earned = (int)Math.Floor(final / _settings.PointsPerAmount);
                if (earned > 0)
                    db.LoyaltyPointEntries.Add(
                        new LoyaltyPointEntry
                        {
                            UserId = profile.UserId,
                            Booking = booking,
                            Points = earned,
                            EarnedAt = paidAt,
                        }
                    );

                // Chỉ điểm còn trong cửa sổ điểm thưởng (mặc định 12 tháng) mới tính vào Points hiện tại.
                if (paidAt > Cinema.Application.Utils.LoyaltyWindow.Cutoff(now, _settings.LoyaltyWindowMonths))
                    profile.Points += earned;
                profile.TotalSpent += final;
                var eligible = _tiersDesc.FirstOrDefault(t => t.MinPoints <= profile.Points);
                if (eligible is not null)
                    profile.TierId = eligible.TierId;
            }
        }

        private void ApplyPending(Plan p, Booking booking, decimal final)
        {
            var holderUserId =
                p.Channel == BookingChannel.Online ? p.User!.UserId : p.Employee!.User.UserId;

            foreach (var seat in p.Seats)
            {
                seat.Status = ShowtimeSeatStatus.Held;
                seat.HeldByUserId = holderUserId;
                seat.HeldUntil = booking.ExpiresAt;
            }

            if (p.PaymentRecord && p.Method is { } m && m != PaymentMethod.Cash)
                AddPayment(booking, m, final, PaymentStatus.Pending, p.CreatedAt.AddMinutes(1), null, null);
        }

        private void ApplyCancelled(Plan p, Booking booking, decimal final)
        {
            var showtime = p.St.Showtime;
            var method = p.Method ?? PaymentMethod.Cash;
            var created = p.CreatedAt.AddMinutes(1);
            var paidAt = p.CreatedAt.AddMinutes(2);

            switch (p.Cancel)
            {
                case CancelKind.FromPending:
                    // Hủy khi còn Pending / thanh toán thất bại: ghế đã trả về, payment Pending -> Failed.
                    if (p.PaymentRecord && method != PaymentMethod.Cash)
                        AddPayment(booking, method, final, PaymentStatus.Failed, created, null, p.FailCode ?? UserCancelCode(method));
                    break;

                case CancelKind.RefundedByUser:
                {
                    // Khách hủy sau khi thanh toán, còn trước hạn hoàn tiền (CancelDeadlineHours).
                    var payment = AddPayment(booking, method, final, PaymentStatus.Refunded, created, paidAt, SuccessCode(method));
                    var latest = showtime.StartTime.AddHours(-_settings.CancelDeadlineHours).AddMinutes(-1);
                    var spanMinutes = Math.Max(6, (int)(latest - p.CreatedAt).TotalMinutes);
                    var refundedAt = p.CreatedAt.AddMinutes(_rng.Next(5, spanMinutes));
                    if (refundedAt > now)
                        refundedAt = now.AddMinutes(-1);

                    payment.RefundedAmount = Math.Round(
                        final * _settings.CancelRefundPercent / 100m,
                        0,
                        MidpointRounding.AwayFromZero
                    );
                    payment.RefundedAt = refundedAt;
                    payment.RefundTransactionId = RefundTxn(method);
                    break;
                }

                case CancelKind.LatePaymentRefund:
                {
                    // Thanh toán thành công nhưng ghế đã hết hạn giữ: booking Cancelled + hoàn toàn bộ tự động.
                    var payment = AddPayment(booking, method, final, PaymentStatus.Refunded, created, paidAt, SuccessCode(method));
                    payment.RefundedAmount = final;
                    payment.RefundedAt = paidAt.AddMinutes(1);
                    payment.RefundTransactionId = RefundTxn(method);
                    break;
                }

                case CancelKind.ShowtimeCancelledRefunded:
                {
                    // Rạp hủy suất: ghế Booked/vé giữ nguyên (giống ShowtimeService), cổng đã hoàn tiền tự động.
                    IssueTickets(p, booking);
                    var payment = AddPayment(booking, method, final, PaymentStatus.Refunded, created, paidAt, SuccessCode(method));
                    payment.RefundedAmount = final;
                    payment.RefundedAt = now.AddHours(-_rng.Next(1, 24));
                    payment.RefundTransactionId = RefundTxn(method);
                    break;
                }

                case CancelKind.ShowtimeCancelledManual:
                    // Thanh toán Cash: giữ Success, rạp phải hoàn tiền thủ công.
                    IssueTickets(p, booking);
                    AddPayment(booking, PaymentMethod.Cash, final, PaymentStatus.Success, created, paidAt, null);
                    break;
            }
        }

        private void IssueTickets(Plan p, Booking booking)
        {
            var showtime = p.St.Showtime;

            for (var i = 0; i < p.Seats.Count; i++)
            {
                var seat = p.Seats[i];
                seat.Status = ShowtimeSeatStatus.Booked;
                seat.HeldByUserId = null;
                seat.HeldUntil = null;

                var isCheckedIn =
                    p.CheckIn == CheckIn.All || (p.CheckIn == CheckIn.First && i == 0);
                DateTime? checkedInAt = null;
                if (isCheckedIn)
                {
                    var at = showtime.StartTime.AddMinutes(-_rng.Next(3, 26));
                    checkedInAt = at > now ? now : at;

                    if (p.User is not null && showtime.Status == ShowtimeStatus.Completed)
                        _reviewCandidates.Add(
                            new ReviewCandidate(p.User.UserId, showtime.MovieId, showtime.EndTime)
                        );
                }

                booking.Tickets.Add(
                    new BookingTicket
                    {
                        ShowtimeSeat = seat,
                        Price = seat.Price,
                        QrCode = $"{booking.BookingCode}-{seat.Seat.SeatRow}{seat.Seat.SeatNumber}",
                        IsCheckedIn = isCheckedIn,
                        CheckedInAt = checkedInAt,
                    }
                );
            }
        }

        private Payment AddPayment(
            Booking booking,
            PaymentMethod method,
            decimal amount,
            PaymentStatus status,
            DateTime createdAt,
            DateTime? paidAt,
            string? responseCode
        )
        {
            var payment = new Payment
            {
                PaymentMethod = method,
                Amount = amount,
                Status = status,
                CreatedAt = createdAt,
                PaidAt = paidAt,
            };

            if (method == PaymentMethod.Cash)
            {
                if (status == PaymentStatus.Success)
                {
                    var received = Math.Ceiling(amount / 50_000m) * 50_000m;
                    payment.AmountReceived = received;
                    payment.ChangeGiven = received - amount;
                }
            }
            else
            {
                payment.TransactionId = $"{booking.BookingCode}{ToUnixSeconds(createdAt)}";
                payment.GatewayResponseCode = responseCode;
                if (status is PaymentStatus.Success or PaymentStatus.Refunded)
                    payment.GatewayTransactionId = GatewayTxn(method);
            }

            booking.Payments.Add(payment);
            return payment;
        }

        // ───────────────────────── Review ─────────────────────────

        private async Task SeedReviewsAsync(CancellationToken cancellationToken)
        {
            // Mỗi (user, phim) tối đa 1 review (UQ_User_Movie_Review); chỉ user đã check-in suất Completed.
            var candidates = _reviewCandidates
                .Where(c => !_reviewReady.Contains((c.UserId, c.MovieId))) // giữ lại để khách tự review
                .GroupBy(c => (c.UserId, c.MovieId))
                .Select(g => g.OrderBy(x => x.ShowtimeEnd).First())
                .OrderBy(c => c.UserId)
                .ThenBy(c => c.MovieId)
                .ToList();

            var reviews = new List<Review>();
            foreach (var c in candidates)
            {
                if (_rng.NextDouble() > 0.8) // ~20% khách đã xem nhưng chưa đánh giá
                    continue;

                var rating = RollRating();
                var createdAt = c.ShowtimeEnd.AddMinutes(_rng.Next(60, 60 * 36));
                if (createdAt > now)
                    createdAt = now.AddMinutes(-1);

                reviews.Add(
                    new Review
                    {
                        MovieId = c.MovieId,
                        UserId = c.UserId,
                        Rating = rating,
                        Comment = _rng.NextDouble() < 0.2 ? null : PickComment(rating),
                        CreatedAt = createdAt,
                    }
                );
            }

            if (reviews.Count == 0)
                return;

            db.Reviews.AddRange(reviews);
            await db.SaveChangesAsync(cancellationToken);

            // Giống ReviewService.RecalculateMovieAverageRatingAsync.
            foreach (var movieId in reviews.Select(r => r.MovieId).Distinct())
            {
                var average = await db
                    .Reviews.Where(r => r.MovieId == movieId)
                    .Select(r => (decimal?)r.Rating)
                    .AverageAsync(cancellationToken);

                var movie = await db.Movies.FirstAsync(m => m.MovieId == movieId, cancellationToken);
                movie.AverageRating = Math.Round(average ?? 0m, 2);
            }

            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded {Count} reviews.", reviews.Count);
        }

        private int RollRating() =>
            _rng.NextDouble() switch
            {
                < 0.35 => 5,
                < 0.70 => 4,
                < 0.85 => 3,
                < 0.95 => 2,
                _ => 1,
            };

        private string PickComment(int rating)
        {
            string[] pool = rating switch
            {
                5 => new[]
                {
                    "Phim quá đỉnh, xem xong vẫn còn nổi da gà!",
                    "Diễn xuất, âm thanh, hình ảnh đều xuất sắc. Rất đáng tiền vé.",
                    "Một trong những phim hay nhất năm, nhất định phải xem ở rạp.",
                },
                4 => new[]
                {
                    "Phim hay, nội dung cuốn hút, chỉ hơi dài một chút.",
                    "Hình ảnh đẹp, diễn viên diễn tốt. Sẽ giới thiệu cho bạn bè.",
                    "Khá ổn, đáng xem cuối tuần.",
                },
                3 => new[]
                {
                    "Phim tạm được, phần giữa hơi chậm.",
                    "Xem giải trí thì ok, chưa thật sự ấn tượng.",
                    "Có điểm sáng nhưng kịch bản còn nhiều chỗ chưa thuyết phục.",
                },
                2 => new[]
                {
                    "Kỳ vọng nhiều hơn, kịch bản khá rời rạc.",
                    "Hình ảnh đẹp nhưng nội dung nhạt.",
                },
                _ => new[]
                {
                    "Không hợp gu của mình, cảm thấy hơi phí thời gian.",
                    "Nội dung kém hấp dẫn, khó theo dõi.",
                },
            };

            return pool[_rng.Next(pool.Length)];
        }

        // ───────────────────────── Tiện ích ─────────────────────────

        private static string SuccessCode(PaymentMethod method) =>
            method switch
            {
                PaymentMethod.VNPay => "00",
                PaymentMethod.Momo => "0",
                PaymentMethod.Stripe => "succeeded",
                _ => "",
            };

        private static string UserCancelCode(PaymentMethod method) =>
            method switch
            {
                PaymentMethod.VNPay => "24", // khách hủy giao dịch
                PaymentMethod.Momo => "1006", // người dùng từ chối xác nhận
                _ => "canceled",
            };

        private static string TimeoutCode(PaymentMethod method) =>
            method switch
            {
                PaymentMethod.VNPay => "11", // hết hạn chờ thanh toán
                PaymentMethod.Momo => "1005", // giao dịch hết hạn
                _ => "canceled",
            };

        private string GatewayTxn(PaymentMethod method) =>
            method switch
            {
                PaymentMethod.VNPay => "14" + Digits(6),
                PaymentMethod.Momo => "28" + Digits(9),
                PaymentMethod.Stripe => "pi_" + Alnum(24),
                _ => "",
            };

        private string RefundTxn(PaymentMethod method) =>
            method switch
            {
                PaymentMethod.VNPay => Digits(8),
                PaymentMethod.Momo => "29" + Digits(9),
                PaymentMethod.Stripe => "re_" + Alnum(24),
                _ => "",
            };

        private string Digits(int length) =>
            new(Enumerable.Range(0, length).Select(_ => (char)('0' + _rng.Next(10))).ToArray());

        private string Alnum(int length)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(
                Enumerable.Range(0, length).Select(_ => chars[_rng.Next(chars.Length)]).ToArray()
            );
        }

        private static long ToUnixSeconds(DateTime value) =>
            new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)).ToUnixTimeSeconds();

        private static readonly string[] LastNames =
        [
            "Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Huỳnh", "Phan", "Vũ", "Võ", "Đặng", "Bùi", "Đỗ",
        ];

        private static readonly string[] MiddleNames =
        [
            "Văn", "Thị", "Minh", "Quốc", "Ngọc", "Thanh", "Hữu", "Gia",
        ];

        private static readonly string[] FirstNames =
        [
            "An", "Bình", "Châu", "Dũng", "Hà", "Khoa", "Lan", "Mai", "Nam", "Phúc", "Quân", "Trang", "Uyên", "Vy",
        ];

        private string RandomName() =>
            $"{LastNames[_rng.Next(LastNames.Length)]} {MiddleNames[_rng.Next(MiddleNames.Length)]} {FirstNames[_rng.Next(FirstNames.Length)]}";

        private string RandomPhone() => "09" + Digits(8);
    }
}

using System.Security.Cryptography;
using System.Text;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema.Infrastructure.Persistence.Seed;

/// <summary>
/// Seed runtime (chạy sau khi migrate). Dữ liệu tĩnh (role, phòng, ghế, phim, combo...) nằm trong
/// <see cref="SeedData"/> (HasData). File này bổ sung dữ liệu động, phụ thuộc thời gian hiện tại:
/// tài khoản test, phòng đặc biệt, suất chiếu (quá khứ/hiện tại/tương lai) và toàn bộ kịch bản
/// booking - payment - check-in - review (xem <see cref="BookingScenarioSeeder"/>).
/// Mọi bước đều idempotent: chạy lại nhiều lần không tạo trùng.
/// </summary>
public static class DbInitializer
{
    private const int CustomerRoleId = 1;
    private const int EmployeeRoleId = 3;

    private const int PositionManagerId = 1;
    private const int PositionShiftLeadId = 2;
    private const int PositionTicketSellerId = 3;

    private const string LoungeRoomName = "Premium Lounge 05";
    private const string MaintenanceRoomName = "Cinema Hall 06 (Bảo trì)";

    // Token demo (plain text) — chỉ để test các flow verify email / reset password.
    internal const string DemoEmailConfirmToken = "demo-email-confirm-token";
    internal const string DemoPasswordResetToken = "demo-password-reset-token";
    internal const string DemoExpiredResetToken = "demo-expired-reset-token";
    internal const string DemoUsedConfirmToken = "demo-used-confirm-token";

    private sealed record UserSeed(
        string Email,
        string Password,
        string FullName,
        string Phone,
        Gender Gender,
        DateOnly DateOfBirth,
        bool IsActive = true,
        bool EmailConfirmed = true,
        string? GoogleId = null
    );

    private sealed record CustomerSeed(UserSeed User, int Points, decimal TotalSpent);

    private sealed record EmployeeSeed(
        UserSeed User,
        int PositionId,
        bool IsActive = true,
        DateOnly? HireDate = null
    );

    private static readonly CustomerSeed[] TestCustomers =
    [
        // Thành viên (0+ điểm, giảm 0%)
        new(
            new UserSeed(
                SeedEmails.Customer1,
                "Customer@123",
                "Nguyễn Văn Customer",
                "0901234567",
                Gender.Male,
                new DateOnly(1995, 3, 14)
            ),
            300,
            1_000_000m
        ),
        // Bạc (500+ điểm, giảm 5%)
        new(
            new UserSeed(
                SeedEmails.Customer2,
                "Customer@123",
                "Trần Thị Bạc",
                "0901234568",
                Gender.Female,
                new DateOnly(1993, 7, 2)
            ),
            800,
            3_500_000m
        ),
        // Vàng (2000+ điểm, giảm 10%)
        new(
            new UserSeed(
                SeedEmails.Customer3,
                "Customer@123",
                "Lê Văn Vàng",
                "0901234569",
                Gender.Male,
                new DateOnly(1988, 11, 25)
            ),
            2_500,
            12_000_000m
        ),
        // Kim Cương (5000+ điểm, giảm 15%)
        new(
            new UserSeed(
                SeedEmails.Customer4,
                "Customer@123",
                "Phạm Thị Kim Cương",
                "0901234570",
                Gender.Female,
                new DateOnly(1990, 1, 9)
            ),
            6_000,
            30_000_000m
        ),
        // Khách mới: chưa có booking/điểm nào (test empty state)
        new(
            new UserSeed(
                SeedEmails.CustomerNew,
                "Customer@123",
                "Hoàng Minh Mới",
                "0901234572",
                Gender.Other,
                new DateOnly(2001, 5, 20)
            ),
            0,
            0m
        ),
        // Tài khoản bị khóa (IsActive = false) — login phải bị từ chối
        new(
            new UserSeed(
                SeedEmails.CustomerInactive,
                "Customer@123",
                "Vũ Thị Bị Khóa",
                "0901234573",
                Gender.Female,
                new DateOnly(1997, 9, 30),
                IsActive: false
            ),
            100,
            500_000m
        ),
        // Chưa xác thực email (EmailConfirmed = false) — login phải bị từ chối / yêu cầu xác thực
        new(
            new UserSeed(
                SeedEmails.CustomerUnverified,
                "Customer@123",
                "Đỗ Văn Chưa Xác Thực",
                "0901234574",
                Gender.Male,
                new DateOnly(1999, 12, 1),
                EmailConfirmed: false
            ),
            0,
            0m
        ),
        // Đăng ký bằng Google (có GoogleId, mật khẩu ngẫu nhiên)
        new(
            new UserSeed(
                SeedEmails.CustomerGoogle,
                "Google@123",
                "Ngô Thị Google",
                "0901234575",
                Gender.Female,
                new DateOnly(1996, 4, 18),
                GoogleId: "seed-google-sub-000000000001"
            ),
            0,
            0m
        ),
    ];

    private static readonly EmployeeSeed[] TestEmployees =
    [
        // Nhân viên bán vé — test quầy / check-in (StaffOnly: role Admin|Employee + có Employee record)
        new(
            new UserSeed(
                SeedEmails.Employee,
                "Employee@123",
                "Đặng Văn Nhân Viên",
                "0901234571",
                Gender.Male,
                new DateOnly(1998, 6, 6)
            ),
            PositionTicketSellerId,
            HireDate: new DateOnly(2026, 2, 1)
        ),
        new(
            new UserSeed(
                SeedEmails.Manager,
                "Employee@123",
                "Bùi Thị Quản Lý",
                "0901234576",
                Gender.Female,
                new DateOnly(1985, 8, 12)
            ),
            PositionManagerId,
            HireDate: new DateOnly(2026, 1, 15)
        ),
        new(
            new UserSeed(
                SeedEmails.ShiftLead,
                "Employee@123",
                "Ngô Văn Trưởng Ca",
                "0901234577",
                Gender.Male,
                new DateOnly(1992, 2, 28)
            ),
            PositionShiftLeadId,
            HireDate: new DateOnly(2026, 1, 20)
        ),
        // Nhân viên đã bị vô hiệu hóa (Employee.IsActive = false) — còn booking quầy trong lịch sử
        new(
            new UserSeed(
                SeedEmails.EmployeeInactive,
                "Employee@123",
                "Lý Văn Nghỉ Việc",
                "0901234578",
                Gender.Male,
                new DateOnly(1994, 10, 10)
            ),
            PositionTicketSellerId,
            IsActive: false,
            HireDate: new DateOnly(2026, 1, 25)
        ),
    ];

    public static async Task SeedAsync(
        AppDbContext db,
        ILogger logger,
        CancellationToken cancellationToken = default,
        bool seedDemoScenarios = true
    )
    {
        await SeedTestAccountsAsync(db, logger, cancellationToken);
        await SeedVerificationTokensAsync(db, logger, cancellationToken);
        await SeedExtraRoomsAsync(db, logger, cancellationToken);

        var scenariosPending =
            seedDemoScenarios
            && !await db.Bookings.AnyAsync(
                b => b.BookingCode.StartsWith(BookingScenarioSeeder.CodePrefix),
                cancellationToken
            );

        if (!scenariosPending)
        {
            // Đã có dữ liệu kịch bản (hoặc tắt seed demo): chỉ đảm bảo luôn có suất chiếu tương lai.
            await ShowtimeSeeder.TopUpFutureAsync(db, logger, DateTime.UtcNow, cancellationToken);
            return;
        }

        await SeedScenariosAsync(db, logger, cancellationToken);
    }

    private static async Task SeedScenariosAsync(
        AppDbContext db,
        ILogger logger,
        CancellationToken cancellationToken
    )
    {
        var now = DateTime.UtcNow;
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            logger.LogInformation("Seeding demo scenarios (showtimes, bookings, payments, reviews)...");

            // Thứ tự quan trọng: suất đặc biệt trước (giữ chỗ phòng/giờ), rồi mới lấp suất thường xung quanh.
            var specials = await ShowtimeSeeder.CreateSpecialsAsync(
                db,
                logger,
                now,
                cancellationToken
            );
            await ShowtimeSeeder.TopUpFutureAsync(db, logger, now, cancellationToken);
            await ShowtimeSeeder.SeedPastAsync(db, logger, now, cancellationToken);
            await BookingScenarioSeeder.SeedAsync(db, logger, now, specials, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            logger.LogInformation("Demo scenarios seeded successfully.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            db.ChangeTracker.Clear();
            logger.LogError(
                ex,
                "Seed demo scenarios thất bại — đã rollback. Hệ thống vẫn chạy với dữ liệu tĩnh + suất chiếu cơ bản."
            );

            // Fallback: tối thiểu vẫn có suất chiếu tương lai để dùng app.
            await ShowtimeSeeder.TopUpFutureAsync(db, logger, DateTime.UtcNow, cancellationToken);
        }
    }

    // ───────────────────────────── Accounts ─────────────────────────────

    private static async Task SeedTestAccountsAsync(
        AppDbContext db,
        ILogger logger,
        CancellationToken cancellationToken
    )
    {
        var hasher = new PasswordHasher<User>();

        foreach (var seed in TestCustomers)
        {
            var user = await EnsureUserAsync(
                db,
                hasher,
                logger,
                seed.User,
                CustomerRoleId,
                cancellationToken
            );

            var hasProfile = await db.CustomerProfiles.AnyAsync(
                p => p.UserId == user.UserId,
                cancellationToken
            );
            if (hasProfile)
                continue;

            var tier = await db
                .MembershipTiers.Where(t => t.MinPoints <= seed.Points)
                .OrderByDescending(t => t.MinPoints)
                .FirstOrDefaultAsync(cancellationToken);

            db.CustomerProfiles.Add(
                new CustomerProfile
                {
                    UserId = user.UserId,
                    TierId = tier?.TierId,
                    Points = seed.Points,
                    TotalSpent = seed.TotalSpent,
                }
            );
            if (seed.Points > 0)
                db.LoyaltyPointEntries.Add(
                    new LoyaltyPointEntry
                    {
                        UserId = user.UserId,
                        Points = seed.Points,
                        EarnedAt = DateTime.UtcNow,
                    }
                );
            await db.SaveChangesAsync(cancellationToken);
        }

        foreach (var seed in TestEmployees)
        {
            var user = await EnsureUserAsync(
                db,
                hasher,
                logger,
                seed.User,
                EmployeeRoleId,
                cancellationToken
            );

            var hasEmployee = await db.Employees.AnyAsync(
                e => e.UserId == user.UserId,
                cancellationToken
            );
            if (hasEmployee)
                continue;

            db.Employees.Add(
                new Employee
                {
                    UserId = user.UserId,
                    PositionId = seed.PositionId,
                    HireDate = seed.HireDate,
                    IsActive = seed.IsActive,
                }
            );
            await db.SaveChangesAsync(cancellationToken);
        }

        // Admin (HasData) — đảm bảo mật khẩu đúng như tài liệu: Admin@123.
        var adminUser = await db.Users.FirstOrDefaultAsync(
            u => u.Email == SeedEmails.Admin,
            cancellationToken
        );
        if (adminUser != null && !IsPasswordValid(hasher, adminUser, "Admin@123"))
        {
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");
            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Reset admin password hash to match documented Admin@123.");
        }
    }

    private static async Task<User> EnsureUserAsync(
        AppDbContext db,
        PasswordHasher<User> hasher,
        ILogger logger,
        UserSeed seed,
        int roleId,
        CancellationToken cancellationToken
    )
    {
        var existing = await db.Users.FirstOrDefaultAsync(
            u => u.Email == seed.Email,
            cancellationToken
        );
        if (existing != null)
            return existing;

        var user = new User
        {
            FullName = seed.FullName,
            Email = seed.Email,
            Phone = seed.Phone,
            Gender = seed.Gender,
            DateOfBirth = seed.DateOfBirth,
            GoogleId = seed.GoogleId,
            RoleId = roleId,
            IsActive = seed.IsActive,
            EmailConfirmed = seed.EmailConfirmed,
            CreatedAt = DateTime.UtcNow,
        };
        // Tài khoản Google: mật khẩu ngẫu nhiên (giống AuthService khi đăng nhập Google lần đầu).
        var password =
            seed.GoogleId is null ? seed.Password : Guid.NewGuid().ToString("N") + "Aa1!";
        user.PasswordHash = hasher.HashPassword(user, password);

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Seeded test user: {Email} / {Password}",
            seed.Email,
            seed.GoogleId is null ? seed.Password : "(Google login)"
        );
        return user;
    }

    private static bool IsPasswordValid(PasswordHasher<User> hasher, User user, string password)
    {
        if (string.IsNullOrEmpty(user.PasswordHash))
            return false;

        try
        {
            return hasher.VerifyHashedPassword(user, user.PasswordHash, password)
                != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    // ─────────────────────── Verification tokens (demo) ───────────────────────

    private static async Task SeedVerificationTokensAsync(
        AppDbContext db,
        ILogger logger,
        CancellationToken cancellationToken
    )
    {
        var now = DateTime.UtcNow;
        var tokens = new (
            string Email,
            string Raw,
            VerificationTokenPurpose Purpose,
            TimeSpan ExpiresIn,
            bool Used
        )[]
        {
            // Hợp lệ: dùng để test API xác thực email cho tài khoản chưa xác thực.
            (
                SeedEmails.CustomerUnverified,
                DemoEmailConfirmToken,
                VerificationTokenPurpose.EmailConfirmation,
                TimeSpan.FromDays(30),
                false
            ),
            // Hợp lệ: dùng để test reset password.
            (
                SeedEmails.CustomerNew,
                DemoPasswordResetToken,
                VerificationTokenPurpose.PasswordReset,
                TimeSpan.FromDays(30),
                false
            ),
            // Đã hết hạn.
            (
                SeedEmails.Customer1,
                DemoExpiredResetToken,
                VerificationTokenPurpose.PasswordReset,
                TimeSpan.FromDays(-1),
                false
            ),
            // Đã sử dụng.
            (
                SeedEmails.Customer2,
                DemoUsedConfirmToken,
                VerificationTokenPurpose.EmailConfirmation,
                TimeSpan.FromDays(-3),
                true
            ),
        };

        var added = false;
        foreach (var t in tokens)
        {
            var hash = HashToken(t.Raw);
            if (await db.UserVerificationTokens.AnyAsync(x => x.TokenHash == hash, cancellationToken))
                continue;

            var userId = await db
                .Users.Where(u => u.Email == t.Email)
                .Select(u => (int?)u.UserId)
                .FirstOrDefaultAsync(cancellationToken);
            if (userId is null)
                continue;

            db.UserVerificationTokens.Add(
                new UserVerificationToken
                {
                    UserId = userId.Value,
                    TokenHash = hash,
                    Purpose = t.Purpose,
                    ExpiresAt = now.Add(t.ExpiresIn),
                    Used = t.Used,
                    // Token đã hết hạn/đã dùng thì cũng phải được tạo từ trước đó.
                    CreatedAt = t.ExpiresIn < TimeSpan.Zero ? now.AddDays(-4) : now,
                }
            );
            added = true;
        }

        if (added)
        {
            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation(
                "Seeded demo verification tokens: {Confirm} (email confirm), {Reset} (password reset)",
                DemoEmailConfirmToken,
                DemoPasswordResetToken
            );
        }
    }

    // Giống JwtTokenService.HashOpaqueToken (SHA-256 -> hex hoa).
    private static string HashToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    // ───────────────────────── Phòng đặc biệt ─────────────────────────

    private static async Task SeedExtraRoomsAsync(
        AppDbContext db,
        ILogger logger,
        CancellationToken cancellationToken
    )
    {
        // Phòng có lối đi (aisle), ghế đôi chia 2 khối và 1 ghế đang hỏng (IsActive = false).
        if (!await db.CinemaRooms.AnyAsync(r => r.RoomName == LoungeRoomName, cancellationToken))
        {
            var room = new CinemaRoom
            {
                RoomName = LoungeRoomName,
                ScreenTypeId = 2, // 3D
                Rows = 5,
                Cols = 13,
                TotalSeats = 0,
                IsActive = true,
            };
            db.CinemaRooms.Add(room);
            await db.SaveChangesAsync(cancellationToken);

            var seats = BuildLoungeSeats(room.RoomId);
            room.TotalSeats = seats.Count(s => s.IsActive && !s.IsAisle);
            db.Seats.AddRange(seats);
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Seeded room {Room} ({Seats} seats, có aisle + 1 ghế hỏng).",
                LoungeRoomName,
                room.TotalSeats
            );
        }

        // Phòng đang bảo trì (IsActive = false) — không được xếp suất chiếu.
        if (
            !await db.CinemaRooms.AnyAsync(r => r.RoomName == MaintenanceRoomName, cancellationToken)
        )
        {
            var room = new CinemaRoom
            {
                RoomName = MaintenanceRoomName,
                ScreenTypeId = 1, // 2D
                Rows = 4,
                Cols = 10,
                TotalSeats = 0,
                IsActive = false,
            };
            db.CinemaRooms.Add(room);
            await db.SaveChangesAsync(cancellationToken);

            var seats = BuildMaintenanceSeats(room.RoomId);
            room.TotalSeats = seats.Count;
            db.Seats.AddRange(seats);
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Seeded inactive room {Room}.", MaintenanceRoomName);
        }
    }

    // Khớp định dạng GetCoupleId trong SeedData: c0000000-0000-0000-{room:D4}-{row*1000+pair:D12}
    private static Guid CoupleGuid(int roomId, int rowIdx, int pairIndex) =>
        Guid.Parse($"c0000000-0000-0000-{roomId:D4}-{((rowIdx + 1) * 1000 + pairIndex):D12}");

    private static List<Seat> BuildLoungeSeats(int roomId)
    {
        const int rows = 5;
        const int cols = 13;
        const int aisleCol = 7;
        var seats = new List<Seat>();

        for (var rowIdx = 0; rowIdx < rows; rowIdx++)
        {
            var row = ((char)('A' + rowIdx)).ToString();
            var seatTypeId = rowIdx switch
            {
                0 or 1 => 1, // Standard
                2 or 3 => 2, // VIP
                _ => 3, // Couple
            };

            for (var col = 1; col <= cols; col++)
            {
                if (col == aisleCol)
                {
                    seats.Add(
                        new Seat
                        {
                            RoomId = roomId,
                            SeatRow = row,
                            SeatNumber = col,
                            SeatTypeId = null,
                            IsActive = true,
                            IsAisle = true,
                        }
                    );
                    continue;
                }

                Guid? coupleId = null;
                if (seatTypeId == 3)
                {
                    // Khối trái: ghế 1-6 -> cặp 1..3 ; khối phải: ghế 8-13 -> cặp 4..6
                    var pair = col < aisleCol ? (col + 1) / 2 : 3 + (col - aisleCol + 1) / 2;
                    coupleId = CoupleGuid(roomId, rowIdx, pair);
                }

                seats.Add(
                    new Seat
                    {
                        RoomId = roomId,
                        SeatRow = row,
                        SeatNumber = col,
                        SeatTypeId = seatTypeId,
                        CoupleId = coupleId,
                        // C3: ghế hỏng, tạm ngưng sử dụng
                        IsActive = !(rowIdx == 2 && col == 3),
                        IsAisle = false,
                    }
                );
            }
        }

        return seats;
    }

    private static List<Seat> BuildMaintenanceSeats(int roomId)
    {
        var seats = new List<Seat>();
        for (var rowIdx = 0; rowIdx < 4; rowIdx++)
        {
            var row = ((char)('A' + rowIdx)).ToString();
            var seatTypeId = rowIdx switch
            {
                0 or 1 => 1,
                2 => 2,
                _ => 3,
            };

            for (var col = 1; col <= 10; col++)
            {
                seats.Add(
                    new Seat
                    {
                        RoomId = roomId,
                        SeatRow = row,
                        SeatNumber = col,
                        SeatTypeId = seatTypeId,
                        CoupleId = seatTypeId == 3 ? CoupleGuid(roomId, rowIdx, (col + 1) / 2) : null,
                        IsActive = true,
                        IsAisle = false,
                    }
                );
            }
        }

        return seats;
    }
}

/// <summary>Email các tài khoản seed (dùng chung giữa các seeder).</summary>
internal static class SeedEmails
{
    public const string Admin = "admin@cinema.local";

    public const string Customer1 = "customer@cinema.local"; // Thành viên
    public const string Customer2 = "customer2@cinema.local"; // Bạc
    public const string Customer3 = "customer3@cinema.local"; // Vàng
    public const string Customer4 = "customer4@cinema.local"; // Kim Cương
    public const string CustomerNew = "newbie@cinema.local";
    public const string CustomerInactive = "inactive@cinema.local";
    public const string CustomerUnverified = "unverified@cinema.local";
    public const string CustomerGoogle = "google@cinema.local";

    public const string Employee = "employee@cinema.local";
    public const string Manager = "manager@cinema.local";
    public const string ShiftLead = "shiftlead@cinema.local";
    public const string EmployeeInactive = "employee.inactive@cinema.local";
}

using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema.Infrastructure.Persistence.Seed;

public static class DbInitializer
{
    private const int MinValidFutureShowtimes = 20;

    private const int DaysAhead = 7;

    private const double SafetyBufferHours = 0.5;

    private static readonly TimeSpan[] DailySlots =
    [
        TimeSpan.FromHours(9),
        TimeSpan.FromHours(13),
        TimeSpan.FromHours(17),
        TimeSpan.FromHours(21),
    ];

    public static async Task SeedAsync(
        AppDbContext db,
        ILogger logger,
        CancellationToken cancellationToken = default
    )
    {
        await SeedTestAccountsAsync(db, logger, cancellationToken);
        await SeedFutureShowtimesAsync(db, logger, cancellationToken);
    }

    private const int CustomerRoleId = 1;
    private const int EmployeeRoleId = 3;
    private const int TicketSellerPositionId = 3;

    private static readonly (
        string Email,
        string Password,
        string FullName,
        string Phone,
        int Points,
        decimal TotalSpent
    )[] TestCustomers =
    [
        // Thành viên tier (0+ points) — the original seeded test customer
        (
            "customer@cinema.local",
            "Customer@123",
            "Nguyễn Văn Customer",
            "0901234567",
            300,
            1_000_000m
        ),
        // Bạc tier (500+ points)
        ("customer2@cinema.local", "Customer@123", "Trần Thị Bạc", "0901234568", 800, 3_500_000m),
        // Vàng tier (2000+ points)
        (
            "customer3@cinema.local",
            "Customer@123",
            "Lê Văn Vàng",
            "0901234569",
            2_500,
            12_000_000m
        ),
        // Kim Cương tier (5000+ points)
        (
            "customer4@cinema.local",
            "Customer@123",
            "Phạm Thị Kim Cương",
            "0901234570",
            6_000,
            30_000_000m
        ),
    ];

    private static readonly (
        string Email,
        string Password,
        string FullName,
        string Phone,
        int PositionId
    )[] TestEmployees =
    [
        // For testing the counter/check-in flows (StaffOnly policy: Admin or Employee role + Employee record)
        (
            "employee@cinema.local",
            "Employee@123",
            "Đặng Văn Nhân Viên",
            "0901234571",
            TicketSellerPositionId
        ),
    ];

    private static async Task SeedTestAccountsAsync(
        AppDbContext db,
        ILogger logger,
        CancellationToken cancellationToken
    )
    {
        var hasher = new PasswordHasher<User>();

        foreach (var seed in TestCustomers)
        {
            await SeedCustomerAsync(db, hasher, logger, seed, cancellationToken);
        }

        foreach (var seed in TestEmployees)
        {
            await SeedEmployeeAsync(db, hasher, logger, seed, cancellationToken);
        }

        var adminUser = await db.Users.FirstOrDefaultAsync(
            u => u.Email == "admin@cinema.local",
            cancellationToken
        );
        if (adminUser != null && !IsPasswordValid(hasher, adminUser, "Admin@123"))
        {
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");
            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Reset admin password hash to match documented Admin@123.");
        }
    }

    private static async Task SeedCustomerAsync(
        AppDbContext db,
        PasswordHasher<User> hasher,
        ILogger logger,
        (
            string Email,
            string Password,
            string FullName,
            string Phone,
            int Points,
            decimal TotalSpent
        ) seed,
        CancellationToken cancellationToken
    )
    {
        var existing = await db.Users.FirstOrDefaultAsync(
            u => u.Email == seed.Email,
            cancellationToken
        );
        if (existing != null)
            return;

        var customer = new User
        {
            FullName = seed.FullName,
            Email = seed.Email,
            Phone = seed.Phone,
            RoleId = CustomerRoleId,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
        };
        customer.PasswordHash = hasher.HashPassword(customer, seed.Password);
        db.Users.Add(customer);
        await db.SaveChangesAsync(cancellationToken);

        var matchingTier = await db
            .MembershipTiers.Where(t => t.MinPoints <= seed.Points)
            .OrderByDescending(t => t.MinPoints)
            .FirstOrDefaultAsync(cancellationToken);

        db.CustomerProfiles.Add(
            new CustomerProfile
            {
                UserId = customer.UserId,
                TierId = matchingTier?.TierId,
                Points = seed.Points,
                TotalSpent = seed.TotalSpent,
            }
        );
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Seeded test customer user: {Email} / {Password}",
            seed.Email,
            seed.Password
        );
    }

    private static async Task SeedEmployeeAsync(
        AppDbContext db,
        PasswordHasher<User> hasher,
        ILogger logger,
        (string Email, string Password, string FullName, string Phone, int PositionId) seed,
        CancellationToken cancellationToken
    )
    {
        var existing = await db.Users.FirstOrDefaultAsync(
            u => u.Email == seed.Email,
            cancellationToken
        );
        if (existing != null)
            return;

        var employeeUser = new User
        {
            FullName = seed.FullName,
            Email = seed.Email,
            Phone = seed.Phone,
            RoleId = EmployeeRoleId,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
        };
        employeeUser.PasswordHash = hasher.HashPassword(employeeUser, seed.Password);
        db.Users.Add(employeeUser);
        await db.SaveChangesAsync(cancellationToken);

        db.Employees.Add(
            new Employee
            {
                UserId = employeeUser.UserId,
                PositionId = seed.PositionId,
                HireDate = DateOnly.FromDateTime(DateTime.UtcNow),
                IsActive = true,
            }
        );
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Seeded test employee user: {Email} / {Password}",
            seed.Email,
            seed.Password
        );
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

    private static async Task SeedFutureShowtimesAsync(
        AppDbContext db,
        ILogger logger,
        CancellationToken cancellationToken
    )
    {
        var settings = await db.CinemaSettings.FirstOrDefaultAsync(cancellationToken);

        var minLeadHours = (settings?.CancelDeadlineHours ?? 2) + SafetyBufferHours;
        var now = DateTime.UtcNow;
        var cutoff = now.AddHours(minLeadHours);

        var validFutureShowtimes = await db.Showtimes.CountAsync(
            s => s.StartTime >= cutoff && s.Status == ShowtimeStatus.Scheduled,
            cancellationToken
        );

        if (validFutureShowtimes >= MinValidFutureShowtimes)
            return;

        logger.LogInformation("Seeding dynamic future showtimes and available seats...");
        var movies = await db
            .Movies.Where(m => m.Status == MovieStatus.NowShowing)
            .OrderBy(m => m.MovieId)
            .ToListAsync(cancellationToken);

        var rooms = await db
            .CinemaRooms.Include(r => r.ScreenType)
            .Where(r => r.IsActive)
            .OrderBy(r => r.RoomId)
            .ToListAsync(cancellationToken);

        var seatsLookup = (
            await db
                .Seats.Include(s => s.SeatType)
                .Where(s => s.IsActive && !s.IsAisle)
                .ToListAsync(cancellationToken)
        )
            .GroupBy(s => s.RoomId)
            .ToDictionary(g => g.Key, g => g.ToList());

        if (movies.Count == 0 || rooms.Count == 0 || seatsLookup.Count == 0)
        {
            logger.LogWarning(
                "Skip seeding showtimes: no NowShowing movie, active room or seat found."
            );
            return;
        }

        var newShowtimes = new List<Showtime>();
        var today = now.Date;

        for (var dayOffset = 0; dayOffset <= DaysAhead; dayOffset++)
        {
            var day = today.AddDays(dayOffset);

            for (var roomIndex = 0; roomIndex < rooms.Count; roomIndex++)
            {
                var room = rooms[roomIndex];
                if (
                    !seatsLookup.TryGetValue(room.RoomId, out var roomSeats)
                    || roomSeats.Count == 0
                )
                    continue;

                var movie = movies[(dayOffset + roomIndex) % movies.Count];
                var basePrice = GetBasePrice(room.ScreenType?.Name);

                foreach (var slot in DailySlots)
                {
                    var startTime = day.Add(slot);
                    if (startTime < cutoff)
                        continue;

                    newShowtimes.Add(BuildShowtime(movie, room, startTime, basePrice, roomSeats));
                }
            }
        }

        if (newShowtimes.Count == 0)
            return;

        db.Showtimes.AddRange(newShowtimes);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Seeded {Count} future showtimes with 100% available seats successfully!",
            newShowtimes.Count
        );
    }

    private static decimal GetBasePrice(string? screenTypeName) =>
        screenTypeName switch
        {
            "IMAX" => 130_000m,
            "3D" => 95_000m,
            _ => 75_000m,
        };

    private static Showtime BuildShowtime(
        Movie movie,
        CinemaRoom room,
        DateTime startTime,
        decimal basePrice,
        List<Seat> roomSeats
    )
    {
        var showtime = new Showtime
        {
            MovieId = movie.MovieId,
            RoomId = room.RoomId,
            StartTime = startTime,
            EndTime = startTime.AddMinutes(movie.DurationMinutes + 15),
            BasePrice = basePrice,
            Status = ShowtimeStatus.Scheduled,
            CreatedAt = DateTime.UtcNow,
        };

        foreach (var seat in roomSeats)
        {
            showtime.ShowtimeSeats.Add(
                new ShowtimeSeat
                {
                    SeatId = seat.SeatId,
                    Status = ShowtimeSeatStatus.Available,
                    Price = Math.Round(basePrice * (seat.SeatType?.PriceMultiplier ?? 1.0m), 0),
                }
            );
        }

        return showtime;
    }
}

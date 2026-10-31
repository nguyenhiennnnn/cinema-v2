using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Infrastructure.Persistence;

namespace Cinema.UnitTests.TestSupport;

public static class TestDataSeeder
{
    public const int CustomerRoleId = 1; // seeded by SeedData.Apply
    public const int AdminRoleId = 2; // seeded by SeedData.Apply
    public const int StandardSeatTypeId = 1; // seeded: PriceMultiplier 1.0
    public const int VipSeatTypeId = 2; // seeded: PriceMultiplier 1.5

    public static User AddCustomer(
        AppDbContext db,
        string email = "customer@test.local",
        bool emailConfirmed = true,
        bool isActive = true
    )
    {
        var user = new User
        {
            FullName = "Test Customer",
            Email = email,
            PasswordHash = "irrelevant-for-these-tests",
            RoleId = CustomerRoleId,
            IsActive = isActive,
            EmailConfirmed = emailConfirmed,
            CreatedAt = DateTime.UtcNow,
        };
        db.Users.Add(user);
        db.SaveChanges();

        db.CustomerProfiles.Add(
            new CustomerProfile
            {
                UserId = user.UserId,
                Points = 0,
                TotalSpent = 0m,
            }
        );
        db.SaveChanges();

        return user;
    }

    public static CinemaRoom AddRoom(AppDbContext db, string name = "Room 1", bool isActive = true)
    {
        var room = new CinemaRoom
        {
            RoomName = name,
            TotalSeats = 0,
            IsActive = isActive,
            Rows = 5,
            Cols = 5,
        };
        db.CinemaRooms.Add(room);
        db.SaveChanges();
        return room;
    }

    /// <summary>Adds <paramref name="count"/> active, non-aisle standard seats to a room.</summary>
    public static List<Seat> AddSeats(AppDbContext db, int roomId, int count = 5, string row = "A")
    {
        var seats = Enumerable
            .Range(1, count)
            .Select(n => new Seat
            {
                RoomId = roomId,
                SeatRow = row,
                SeatNumber = n,
                SeatTypeId = StandardSeatTypeId,
                IsActive = true,
                IsAisle = false,
            })
            .ToList();

        db.Seats.AddRange(seats);
        db.SaveChanges();
        return seats;
    }

    public static Movie AddMovie(
        AppDbContext db,
        string title = "Test Movie",
        int durationMinutes = 120,
        MovieStatus status = MovieStatus.NowShowing
    )
    {
        var movie = new Movie
        {
            Title = title,
            DurationMinutes = durationMinutes,
            Status = status,
            CreatedAt = DateTime.UtcNow,
        };
        db.Movies.Add(movie);
        db.SaveChanges();
        return movie;
    }

    /// <summary>Creates a Scheduled showtime with one ShowtimeSeat per seat in <paramref name="seats"/>.</summary>
    public static Showtime AddShowtime(
        AppDbContext db,
        Movie movie,
        CinemaRoom room,
        IEnumerable<Seat> seats,
        DateTime? startTime = null,
        decimal basePrice = 100_000m,
        ShowtimeStatus status = ShowtimeStatus.Scheduled
    )
    {
        var start = startTime ?? DateTime.UtcNow.AddHours(6);
        var showtime = new Showtime
        {
            MovieId = movie.MovieId,
            RoomId = room.RoomId,
            StartTime = start,
            EndTime = start.AddMinutes(movie.DurationMinutes + 15),
            BasePrice = basePrice,
            Status = status,
            CreatedAt = DateTime.UtcNow,
        };
        db.Showtimes.Add(showtime);
        db.SaveChanges();

        foreach (var seat in seats)
        {
            db.ShowtimeSeats.Add(
                new ShowtimeSeat
                {
                    ShowtimeId = showtime.ShowtimeId,
                    SeatId = seat.SeatId,
                    Status = ShowtimeSeatStatus.Available,
                    Price = basePrice,
                }
            );
        }
        db.SaveChanges();

        return showtime;
    }

    /// <summary>Adds an active Employee (with backing admin User) for counter/staff-flow tests.</summary>
    public static (User User, Employee Employee) AddEmployee(
        AppDbContext db,
        string email = "employee@test.local",
        int? positionId = null
    )
    {
        var user = new User
        {
            FullName = "Test Employee",
            Email = email,
            PasswordHash = "irrelevant-for-these-tests",
            RoleId = AdminRoleId,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
        };
        db.Users.Add(user);
        db.SaveChanges();

        var employee = new Employee
        {
            UserId = user.UserId,
            PositionId = positionId,
            IsActive = true,
        };
        db.Employees.Add(employee);
        db.SaveChanges();

        return (user, employee);
    }

    public static CinemaSettings SetCinemaSettings(
        AppDbContext db,
        decimal cancelRefundPercent = 100m,
        int cancelDeadlineHours = 2,
        decimal pointsPerAmount = 10_000m,
        int holdDurationMinutes = CinemaSettings.DefaultHoldDurationMinutes,
        int checkInOpenMinutes = CinemaSettings.DefaultCheckInOpenMinutes
    )
    {
        var settings = db.CinemaSettings.FirstOrDefault(s => s.Id == 1);
        if (settings is null)
        {
            settings = new CinemaSettings
            {
                Id = 1,
                Name = "Test Cinema",
                Address = "N/A",
            };
            db.CinemaSettings.Add(settings);
        }

        settings.CancelRefundPercent = cancelRefundPercent;
        settings.CancelDeadlineHours = cancelDeadlineHours;
        settings.PointsPerAmount = pointsPerAmount;
        settings.HoldDurationMinutes = holdDurationMinutes;
        settings.CheckInOpenMinutes = checkInOpenMinutes;
        db.SaveChanges();
        return settings;
    }
}

using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Cinema.IntegrationTests.Infrastructure;

public static class IntegrationTestDataSeeder
{
    public const int CustomerRoleId = 1;
    public const int AdminRoleId = 2;

    public static async Task<(
        int UserId,
        string Email,
        string PlainPassword
    )> CreateConfirmedCustomerAsync(IServiceProvider services, string? email = null)
    {
        email ??= $"customer-{Guid.NewGuid():N}@test.local";
        const string plainPassword = "Str0ngPass";

        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher =
            scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.IPasswordHasher<User>>();

        var user = new User
        {
            FullName = "Integration Test Customer",
            Email = email,
            RoleId = CustomerRoleId,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
        };
        user.PasswordHash = hasher.HashPassword(user, plainPassword);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        db.CustomerProfiles.Add(
            new CustomerProfile
            {
                UserId = user.UserId,
                Points = 0,
                TotalSpent = 0m,
            }
        );
        await db.SaveChangesAsync();

        return (user.UserId, email, plainPassword);
    }

    public static async Task<(
        int UserId,
        string Email,
        string PlainPassword
    )> CreateAdminUserWithoutEmployeeAsync(IServiceProvider services, string? email = null)
    {
        email ??= $"admin-noemp-{Guid.NewGuid():N}@test.local";
        const string plainPassword = "Str0ngPass";

        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher =
            scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.IPasswordHasher<User>>();

        var user = new User
        {
            FullName = "Admin Without Employee Record",
            Email = email,
            RoleId = AdminRoleId,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
        };
        user.PasswordHash = hasher.HashPassword(user, plainPassword);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        return (user.UserId, email, plainPassword);
    }

    public static async Task<(
        int UserId,
        int EmployeeId,
        string Email,
        string PlainPassword
    )> CreateEmployeeAsync(IServiceProvider services, string? email = null)
    {
        email ??= $"employee-{Guid.NewGuid():N}@test.local";
        const string plainPassword = "Str0ngPass";

        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher =
            scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.IPasswordHasher<User>>();

        var user = new User
        {
            FullName = "Integration Test Counter Staff",
            Email = email,
            RoleId = AdminRoleId,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
        };
        user.PasswordHash = hasher.HashPassword(user, plainPassword);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var employee = new Employee { UserId = user.UserId, IsActive = true };
        db.Employees.Add(employee);
        await db.SaveChangesAsync();

        return (user.UserId, employee.EmployeeId, email, plainPassword);
    }

    public static async Task<(
        int ShowtimeId,
        List<int> ShowtimeSeatIds,
        decimal SeatPrice
    )> CreateScheduledShowtimeAsync(
        IServiceProvider services,
        decimal basePrice = 100_000m,
        DateTime? startTime = null
    )
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var room = db.CinemaRooms.First(r => r.IsActive);
        var seats = db
            .Seats.Where(s => s.RoomId == room.RoomId && s.IsActive && !s.IsAisle)
            .OrderBy(s => s.SeatRow)
            .ThenBy(s => s.SeatNumber)
            .Take(4)
            .ToList();
        var movie = db.Movies.First(m => m.Status == MovieStatus.NowShowing);

        var start = startTime ?? DateTime.UtcNow.Date.AddDays(3).AddHours(14);
        var showtime = new Showtime
        {
            MovieId = movie.MovieId,
            RoomId = room.RoomId,
            StartTime = start,
            EndTime = start.AddMinutes(movie.DurationMinutes + 15),
            BasePrice = basePrice,
            Status = ShowtimeStatus.Scheduled,
            CreatedAt = DateTime.UtcNow,
        };
        db.Showtimes.Add(showtime);
        await db.SaveChangesAsync();

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
        await db.SaveChangesAsync();

        var seatIds = db
            .ShowtimeSeats.Where(ss => ss.ShowtimeId == showtime.ShowtimeId)
            .OrderBy(ss => ss.ShowtimeSeatId)
            .Select(ss => ss.ShowtimeSeatId)
            .ToList();
        return (showtime.ShowtimeId, seatIds, basePrice);
    }
}

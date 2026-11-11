using Cinema.Application.DTOs.Reviews;
using Cinema.Application.Services.Reviews;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class ReviewServiceTests : IDisposable
{
    private readonly SqliteAppDbContextHandle _handle = new();

    private ReviewService CreateSut()
    {
        _handle.Db.ChangeTracker.Clear();
        return new ReviewService(_handle.Db);
    }

    private async Task<(User Customer, Movie Movie)> SeedCheckedInCustomerAsync(string email)
    {
        var db = _handle.Db;
        var customer = TestDataSeeder.AddCustomer(db, email: email);
        var room = TestDataSeeder.AddRoom(db, name: $"Room-{Guid.NewGuid():N}");
        var seats = TestDataSeeder.AddSeats(db, room.RoomId, count: 1);
        var movie = db.Movies.FirstOrDefault() ?? TestDataSeeder.AddMovie(db);
        var showtime = TestDataSeeder.AddShowtime(db, movie, room, seats);
        var showtimeSeat = await db.ShowtimeSeats.SingleAsync(ss =>
            ss.ShowtimeId == showtime.ShowtimeId
        );
        var booking = new Booking
        {
            UserId = customer.UserId,
            ShowtimeId = showtime.ShowtimeId,
            BookingCode = $"BK{Guid.NewGuid():N}"[..14].ToUpperInvariant(),
            CustomerName = "Nguyen Van A",
            TotalAmount = 100_000m,
            FinalAmount = 100_000m,
            Status = BookingStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
        };
        db.Bookings.Add(booking);
        await db.SaveChangesAsync();
        db.BookingTickets.Add(
            new BookingTicket
            {
                BookingId = booking.BookingId,
                ShowtimeSeatId = showtimeSeat.ShowtimeSeatId,
                Price = 100_000m,
                QrCode = $"QR-{Guid.NewGuid():N}",
                IsCheckedIn = true,
                CheckedInAt = DateTime.UtcNow,
            }
        );
        await db.SaveChangesAsync();
        return (customer, movie);
    }

    [Fact]
    public async Task CreateReviewAsync_FirstReview_UpdatesMovieAverageRating()
    {
        var (customer, movie) = await SeedCheckedInCustomerAsync("first@test.local");
        var sut = CreateSut();

        await sut.CreateReviewAsync(
            customer.UserId,
            movie.MovieId,
            new CreateReviewRequest(4, "Hay"),
            CancellationToken.None
        );

        var reloaded = await _handle
            .Db.Movies.AsNoTracking()
            .SingleAsync(m => m.MovieId == movie.MovieId);
        reloaded.AverageRating.ShouldBe(4m);
    }

    [Fact]
    public async Task CreateReviewAsync_SecondReview_RecalculatesAverageAcrossAllReviews()
    {
        var (first, movie) = await SeedCheckedInCustomerAsync("a@test.local");
        var (second, _) = await SeedCheckedInCustomerAsync("b@test.local");
        var sut = CreateSut();
        await sut.CreateReviewAsync(
            first.UserId,
            movie.MovieId,
            new CreateReviewRequest(5, null),
            CancellationToken.None
        );

        sut = CreateSut();
        await sut.CreateReviewAsync(
            second.UserId,
            movie.MovieId,
            new CreateReviewRequest(2, null),
            CancellationToken.None
        );

        var reloaded = await _handle
            .Db.Movies.AsNoTracking()
            .SingleAsync(m => m.MovieId == movie.MovieId);
        reloaded.AverageRating.ShouldBe(3.5m);
    }

    public void Dispose() => _handle.Dispose();
}

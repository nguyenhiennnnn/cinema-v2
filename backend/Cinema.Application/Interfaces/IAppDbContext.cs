using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cinema.Application.Interfaces;

public interface IAppDbContext
{
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    DbSet<CinemaSettings> CinemaSettings { get; }
    DbSet<Role> Roles { get; }
    DbSet<User> Users { get; }
    DbSet<MembershipTier> MembershipTiers { get; }
    DbSet<CustomerProfile> CustomerProfiles { get; }
    DbSet<LoyaltyPointEntry> LoyaltyPointEntries { get; }
    DbSet<Position> Positions { get; }
    DbSet<Employee> Employees { get; }
    DbSet<ScreenType> ScreenTypes { get; }
    DbSet<CinemaRoom> CinemaRooms { get; }
    DbSet<SeatType> SeatTypes { get; }
    DbSet<Seat> Seats { get; }
    DbSet<Genre> Genres { get; }
    DbSet<Language> Languages { get; }
    DbSet<AgeRating> AgeRatings { get; }
    DbSet<Director> Directors { get; }
    DbSet<Movie> Movies { get; }
    DbSet<MovieGenre> MovieGenres { get; }
    DbSet<Actor> Actors { get; }
    DbSet<MovieCast> MovieCast { get; }
    DbSet<Review> Reviews { get; }
    DbSet<Showtime> Showtimes { get; }
    DbSet<ShowtimeSeat> ShowtimeSeats { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<BookingTicket> BookingTickets { get; }
    DbSet<Combo> Combos { get; }
    DbSet<BookingCombo> BookingCombos { get; }
    DbSet<Payment> Payments { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<UserVerificationToken> UserVerificationTokens { get; }
}

public static class AppDbContextExtensions
{
    public const int CinemaSettingsId = 1;

    public static Task<CinemaSettings> GetCinemaSettingsAsync(
        this IAppDbContext db,
        CancellationToken cancellationToken
    ) => db.CinemaSettings.FirstAsync(s => s.Id == CinemaSettingsId, cancellationToken);

    public static async Task<CinemaSettings> LoadCinemaSettingsOrDefaultAsync(
        this IAppDbContext db,
        CancellationToken cancellationToken
    ) =>
        await db
            .CinemaSettings.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == CinemaSettingsId, cancellationToken)
        ?? new CinemaSettings();

    public static async Task<int> LoadShowtimeLookaheadDaysAsync(
        this IAppDbContext db,
        CancellationToken cancellationToken
    ) =>
        await db
            .CinemaSettings.Where(s => s.Id == CinemaSettingsId)
            .Select(s => (int?)s.ShowtimeLookaheadDays)
            .FirstOrDefaultAsync(cancellationToken)
        ?? CinemaSettings.DefaultShowtimeLookaheadDays;
}

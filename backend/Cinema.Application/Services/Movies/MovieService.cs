using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Movies;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Movies;

public class MovieService(IAppDbContext db) : IMovieService
{
    public async Task<CursorPageResult<MovieListItemDto>> GetMoviesAsync(
        MovieListFilter filter,
        string? cursor,
        int limit,
        CancellationToken cancellationToken
    )
    {
        var query = db.Movies.AsQueryable();

        if (
            EnumConverters.TryParseEnum<MovieStatus>(filter.Status, out var status)
        )
            query = query.Where(m => m.Status == status);
        if (filter.GenreId is not null)
            query = query.Where(m => m.MovieGenres.Any(mg => mg.GenreId == filter.GenreId));
        if (filter.LanguageId is not null)
            query = query.Where(m => m.LanguageId == filter.LanguageId);
        if (filter.AgeRatingId is not null)
            query = query.Where(m => m.AgeRatingId == filter.AgeRatingId);
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(m => m.Title.Contains(filter.Search));

        var cursorId = CursorCodec.Decode(cursor);
        if (cursorId is not null)
            query = query.Where(m => m.MovieId > cursorId);

        query = query.OrderBy(m => m.MovieId);

        var rows = await query
            .Take(limit + 1)
            .ProjectToType<MovieListItemDto>()
            .ToListAsync(cancellationToken);

        var hasMore = rows.Count > limit;
        var items = hasMore ? rows.Take(limit).ToList() : rows;
        var nextCursor = hasMore ? CursorCodec.Encode(items[^1].MovieId) : null;

        return new CursorPageResult<MovieListItemDto>
        {
            Items = items,
            NextCursor = nextCursor,
            HasMore = hasMore,
        };
    }

    public async Task<MovieDetailDto> GetMovieDetailAsync(
        int movieId,
        CancellationToken cancellationToken
    )
    {
        var movie =
            await db
                .Movies.Include(m => m.Director)
                .Include(m => m.Language)
                .Include(m => m.AgeRating)
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieCast)
                    .ThenInclude(mc => mc.Actor)
                .FirstOrDefaultAsync(m => m.MovieId == movieId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy phim.");

        var dto = movie.Adapt<MovieDetailDto>();

        var now = DateTime.UtcNow;
        var upcomingShowtimes = await db
            .Showtimes.Include(s => s.Room)
                .ThenInclude(r => r.ScreenType)
            .Where(s =>
                s.MovieId == movieId
                && s.Status == ShowtimeStatus.Scheduled
                && s.StartTime >= now
                && s.StartTime <= now.AddDays(ShowtimeLookahead.Days)
            )
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);

        var showtimeIds = upcomingShowtimes.Select(s => s.ShowtimeId).ToList();

        var seatsCounts = await db
            .ShowtimeSeats.Where(ss => showtimeIds.Contains(ss.ShowtimeId))
            .GroupBy(ss => ss.ShowtimeId)
            .Select(g => new
            {
                ShowtimeId = g.Key,
                Total = g.Count(),
                Available = g.Count(x => x.Status == ShowtimeSeatStatus.Available),
            })
            .ToListAsync(cancellationToken);

        var seatCountLookup = seatsCounts.ToDictionary(x => x.ShowtimeId, x => x);

        var upcoming = upcomingShowtimes
            .Select(s =>
            {
                var counts = seatCountLookup.GetValueOrDefault(s.ShowtimeId);
                var up = s.Adapt<UpcomingShowtimeDto>();
                return up with
                {
                    AvailableSeats = counts?.Available ?? 0,
                    TotalSeats = counts?.Total ?? 0,
                };
            })
            .ToList();

        return dto with
        {
            UpcomingShowtimes = upcoming,
        };
    }

    public async Task<OffsetPageResult<AdminMovieListItemDto>> GetMoviesAdminAsync(
        AdminMovieListFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.Movies.AsQueryable();

        if (
            EnumConverters.TryParseEnum<MovieStatus>(filter.Status, out var status)
        )
            query = query.Where(m => m.Status == status);
        if (filter.GenreId is not null)
            query = query.Where(m => m.MovieGenres.Any(mg => mg.GenreId == filter.GenreId));
        if (filter.LanguageId is not null)
            query = query.Where(m => m.LanguageId == filter.LanguageId);
        if (filter.AgeRatingId is not null)
            query = query.Where(m => m.AgeRatingId == filter.AgeRatingId);
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(m => m.Title.Contains(filter.Search));

        query = query.OrderByDescending(m => m.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<AdminMovieListItemDto>()
            .ToListAsync(cancellationToken);

        return OffsetPageResult<AdminMovieListItemDto>.Create(items, page, pageSize, total);
    }

    public async Task<MovieDetailDto> CreateMovieAsync(
        UpsertMovieRequest request,
        CancellationToken cancellationToken
    )
    {
        await ValidateForeignKeysAsync(request, cancellationToken);

        var movie = new Movie
        {
            Title = request.Title,
            OriginalTitle = request.OriginalTitle,
            Description = request.Description,
            DirectorId = request.DirectorId,
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            LanguageId = request.LanguageId,
            Subtitle = request.Subtitle,
            AgeRatingId = request.AgeRatingId,
            PosterUrl = request.PosterUrl,
            TrailerUrl = request.TrailerUrl,
            Status = EnumConverters.ParseEnum<MovieStatus>(request.Status, "status"),
            CreatedAt = DateTime.UtcNow,
        };

        foreach (var genreId in request.GenreIds.Distinct())
            movie.MovieGenres.Add(new MovieGenre { GenreId = genreId });

        foreach (var cast in request.Cast)
            movie.MovieCast.Add(
                new MovieCast { ActorId = cast.ActorId, CharacterName = cast.CharacterName }
            );

        db.Movies.Add(movie);
        await db.SaveChangesAsync(cancellationToken);

        return await GetMovieDetailAsync(movie.MovieId, cancellationToken);
    }

    public async Task<MovieDetailDto> UpdateMovieAsync(
        int movieId,
        UpsertMovieRequest request,
        CancellationToken cancellationToken
    )
    {
        var movie =
            await db
                .Movies.Include(m => m.MovieGenres)
                .Include(m => m.MovieCast)
                .FirstOrDefaultAsync(m => m.MovieId == movieId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy phim.");

        await ValidateForeignKeysAsync(request, cancellationToken);

        movie.Title = request.Title;
        movie.OriginalTitle = request.OriginalTitle;
        movie.Description = request.Description;
        movie.DirectorId = request.DirectorId;
        movie.DurationMinutes = request.DurationMinutes;
        movie.ReleaseDate = request.ReleaseDate;
        movie.LanguageId = request.LanguageId;
        movie.Subtitle = request.Subtitle;
        movie.AgeRatingId = request.AgeRatingId;
        movie.PosterUrl = request.PosterUrl;
        movie.TrailerUrl = request.TrailerUrl;
        movie.Status = EnumConverters.ParseEnum<MovieStatus>(request.Status, "status");

        var newGenreIds = request.GenreIds.Distinct().ToHashSet();
        var currentGenreIds = movie.MovieGenres.Select(mg => mg.GenreId).ToHashSet();

        var genresToRemove = movie
            .MovieGenres.Where(mg => !newGenreIds.Contains(mg.GenreId))
            .ToList();
        foreach (var mg in genresToRemove)
            movie.MovieGenres.Remove(mg);
        foreach (var genreId in newGenreIds.Except(currentGenreIds))
        {
            movie.MovieGenres.Add(new MovieGenre { MovieId = movie.MovieId, GenreId = genreId });
        }

        var newCastByActorId = request.Cast.ToDictionary(c => c.ActorId, c => c.CharacterName);
        var currentActorIds = movie.MovieCast.Select(mc => mc.ActorId).ToHashSet();
        var castToRemove = movie
            .MovieCast.Where(mc => !newCastByActorId.ContainsKey(mc.ActorId))
            .ToList();
        foreach (var mc in castToRemove)
            movie.MovieCast.Remove(mc);
        foreach (var mc in movie.MovieCast.Where(mc => newCastByActorId.ContainsKey(mc.ActorId)))
            mc.CharacterName = newCastByActorId[mc.ActorId];

        foreach (var actorId in newCastByActorId.Keys.Except(currentActorIds))
            movie.MovieCast.Add(
                new MovieCast
                {
                    MovieId = movie.MovieId,
                    ActorId = actorId,
                    CharacterName = newCastByActorId[actorId],
                }
            );

        await db.SaveChangesAsync(cancellationToken);

        return await GetMovieDetailAsync(movie.MovieId, cancellationToken);
    }

    public async Task<DeleteMovieResponse> DeleteMovieAsync(
        int movieId,
        CancellationToken cancellationToken
    )
    {
        var movie =
            await db.Movies.FirstOrDefaultAsync(m => m.MovieId == movieId)
            ?? throw new NotFoundException("Không tìm thấy phim.");

        var hasShowtimes = await db.Showtimes.AnyAsync(
            s => s.MovieId == movieId,
            cancellationToken
        );
        if (hasShowtimes)
        {
            movie.Status = MovieStatus.Ended;
            await db.SaveChangesAsync(cancellationToken);
            return new DeleteMovieResponse(movieId, false, true);
        }

        db.Movies.Remove(movie);
        await db.SaveChangesAsync(cancellationToken);

        return new DeleteMovieResponse(movieId, true, false);
    }

    private async Task ValidateForeignKeysAsync(
        UpsertMovieRequest request,
        CancellationToken cancellationToken
    )
    {
        var errors = new Dictionary<string, string[]>();

        if (
            request.DirectorId is not null
            && !await db.Directors.AnyAsync(
                d => d.DirectorId == request.DirectorId,
                cancellationToken
            )
        )
            errors["directorId"] = ["directorId không tồn tại."];

        if (
            request.LanguageId is not null
            && !await db.Languages.AnyAsync(
                l => l.LanguageId == request.LanguageId,
                cancellationToken
            )
        )
            errors["languageId"] = ["languageId không tồn tại."];

        if (
            request.AgeRatingId is not null
            && !await db.AgeRatings.AnyAsync(
                a => a.AgeRatingId == request.AgeRatingId,
                cancellationToken
            )
        )
            errors["ageRatingId"] = ["ageRatingId không tồn tại."];

        if (request.GenreIds.Count > 0)
        {
            var existingGenreCount = await db.Genres.CountAsync(
                g => request.GenreIds.Contains(g.GenreId),
                cancellationToken
            );
            if (existingGenreCount != request.GenreIds.Distinct().Count())
                errors["genreIds"] = ["Một hoặc nhiều genreId không tồn tại."];
        }

        if (request.Cast.Count > 0)
        {
            var actorIds = request.Cast.Select(c => c.ActorId).Distinct().ToList();
            var existingActorCount = await db.Actors.CountAsync(
                a => actorIds.Contains(a.ActorId),
                cancellationToken
            );
            if (existingActorCount != actorIds.Count)
                errors["cast"] = ["Một hoặc nhiều actorId không tồn tại."];
        }

        if (errors.Count > 0)
            throw new ValidationAppException("Dữ liệu tham chiếu không hợp lệ.", errors);
    }
}

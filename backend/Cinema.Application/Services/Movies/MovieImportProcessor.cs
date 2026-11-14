using System.Globalization;
using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Import;
using Cinema.Application.DTOs.Movies;
using Cinema.Application.Interfaces;
using Cinema.Application.Services.Import;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Cinema.Application.Services.Movies;

internal static class MovieImportColumns
{
    public const string Title = nameof(Title);
    public const string OriginalTitle = nameof(OriginalTitle);
    public const string Description = nameof(Description);
    public const string Director = nameof(Director);
    public const string DurationMinutes = nameof(DurationMinutes);
    public const string ReleaseDate = nameof(ReleaseDate);
    public const string Language = nameof(Language);
    public const string Subtitle = nameof(Subtitle);
    public const string AgeRating = nameof(AgeRating);
    public const string PosterUrl = nameof(PosterUrl);
    public const string TrailerUrl = nameof(TrailerUrl);
    public const string Status = nameof(Status);
    public const string Genres = nameof(Genres);
    public const string Cast = nameof(Cast);
}

public class MovieImportProcessor(
    IImportFileReader fileReader,
    IAppDbContext db,
    IMovieService movieService,
    IExportWriter exportWriter,
    IValidator<UpsertMovieRequest> requestValidator,
    ILogger<MovieImportProcessor> logger
) : ImportProcessorBase<MovieImportRow>(fileReader)
{
    private static readonly string[] DateFormats = ["dd/MM/yyyy", "d/M/yyyy"];
    private static readonly string[] StatusNames = Enum.GetNames<MovieStatus>();

    private static readonly Dictionary<string, string>[] ExampleData =
    [
        new()
        {
            [MovieImportColumns.Title] = "Phim ví dụ 1",
            [MovieImportColumns.OriginalTitle] = "Example Movie 1",
            [MovieImportColumns.Description] = "Mô tả ngắn về nội dung phim.",
            [MovieImportColumns.Director] = "Christopher Nolan",
            [MovieImportColumns.DurationMinutes] = "120",
            [MovieImportColumns.ReleaseDate] = "21/07/2023",
            [MovieImportColumns.Language] = "Tiếng Anh",
            [MovieImportColumns.Subtitle] = "Tiếng Việt",
            [MovieImportColumns.AgeRating] = "T13",
            [MovieImportColumns.PosterUrl] = "https://example.com/posters/example-1.jpg",
            [MovieImportColumns.TrailerUrl] = "https://example.com/trailers/example-1.mp4",
            [MovieImportColumns.Status] = "NowShowing",
            [MovieImportColumns.Genres] = "Hành động;Phiêu lưu",
            [MovieImportColumns.Cast] =
                "Cillian Murphy:Nhân vật chính; Robert Downey Jr.:Nhân vật phụ",
        },
        new()
        {
            [MovieImportColumns.Title] = "Phim ví dụ 2",
            [MovieImportColumns.DurationMinutes] = "95",
            [MovieImportColumns.Language] = "Tiếng Việt",
            [MovieImportColumns.AgeRating] = "P",
            [MovieImportColumns.Status] = "ComingSoon",
            [MovieImportColumns.Genres] = "Hài",
        },
    ];

    public override string Module => "movies";

    public override string SheetName => Messages.Import_SheetMovies;

    protected override IReadOnlyList<ImportColumn> Columns { get; } =
        [
            new(
                MovieImportColumns.Title,
                true,
                Messages.Import_Movie_Title_Desc,
                Messages.Import_Movie_Title_Format
            ),
            new(
                MovieImportColumns.OriginalTitle,
                false,
                Messages.Import_Movie_OriginalTitle_Desc,
                Messages.Import_Movie_OriginalTitle_Format
            ),
            new(
                MovieImportColumns.Description,
                false,
                Messages.Import_Movie_Description_Desc,
                Messages.Import_Movie_Description_Format
            ),
            new(
                MovieImportColumns.Director,
                false,
                Messages.Import_Movie_Director_Desc,
                Messages.Import_Movie_Director_Format
            ),
            new(
                MovieImportColumns.DurationMinutes,
                true,
                Messages.Import_Movie_DurationMinutes_Desc,
                Messages.Import_Movie_DurationMinutes_Format
            ),
            new(
                MovieImportColumns.ReleaseDate,
                false,
                Messages.Import_Movie_ReleaseDate_Desc,
                Messages.Import_Movie_ReleaseDate_Format
            ),
            new(
                MovieImportColumns.Language,
                false,
                Messages.Import_Movie_Language_Desc,
                Messages.Import_Movie_Language_Format
            ),
            new(
                MovieImportColumns.Subtitle,
                false,
                Messages.Import_Movie_Subtitle_Desc,
                Messages.Import_Movie_Subtitle_Format
            ),
            new(
                MovieImportColumns.AgeRating,
                false,
                Messages.Import_Movie_AgeRating_Desc,
                Messages.Import_Movie_AgeRating_Format
            ),
            new(
                MovieImportColumns.PosterUrl,
                false,
                Messages.Import_Movie_PosterUrl_Desc,
                Messages.Import_Movie_PosterUrl_Format
            ),
            new(
                MovieImportColumns.TrailerUrl,
                false,
                Messages.Import_Movie_TrailerUrl_Desc,
                Messages.Import_Movie_TrailerUrl_Format
            ),
            new(
                MovieImportColumns.Status,
                true,
                Messages.Import_Movie_Status_Desc,
                string.Format(Messages.Import_Movie_Status_Format, string.Join(", ", StatusNames))
            ),
            new(
                MovieImportColumns.Genres,
                true,
                Messages.Import_Movie_Genres_Desc,
                Messages.Import_Movie_Genres_Format
            ),
            new(
                MovieImportColumns.Cast,
                false,
                Messages.Import_Movie_Cast_Desc,
                Messages.Import_Movie_Cast_Format
            ),
        ];

    protected override MovieImportRow MapRow(IReadOnlyDictionary<string, string?> values) =>
        new(
            values[MovieImportColumns.Title],
            values[MovieImportColumns.OriginalTitle],
            values[MovieImportColumns.Description],
            values[MovieImportColumns.Director],
            values[MovieImportColumns.DurationMinutes],
            values[MovieImportColumns.ReleaseDate],
            values[MovieImportColumns.Language],
            values[MovieImportColumns.Subtitle],
            values[MovieImportColumns.AgeRating],
            values[MovieImportColumns.PosterUrl],
            values[MovieImportColumns.TrailerUrl],
            values[MovieImportColumns.Status],
            values[MovieImportColumns.Genres],
            values[MovieImportColumns.Cast]
        );

    public override async Task<Stream> BuildTemplateAsync(CancellationToken cancellationToken)
    {
        var catalogRows = await CatalogRowsAsync(cancellationToken);

        var sheets = new List<ExportSheet>
        {
            new(SheetName, Columns.Select(c => c.Name).ToList(), ExampleRows()),
            new(
                Messages.Import_SheetGuide,
                [
                    Messages.Import_GuideColColumn,
                    Messages.Import_GuideColRequirement,
                    Messages.Import_GuideColDescription,
                    Messages.Import_GuideColFormat,
                ],
                GuideRows()
            ),
            new(
                Messages.Import_SheetCatalog,
                [
                    Messages.Import_LabelGenre,
                    Messages.Import_LabelDirector,
                    Messages.Import_LabelLanguage,
                    Messages.Import_LabelAgeRating,
                    Messages.Import_LabelActor,
                ],
                catalogRows
            ),
        };

        return await exportWriter.WriteXlsxWorkbookAsync(sheets, cancellationToken);
    }

    public override async Task<IImportContext> CreateContextAsync(
        IReadOnlyList<ImportRow<MovieImportRow>> rows,
        CancellationToken cancellationToken
    )
    {
        var genres = await db
            .Genres.AsNoTracking()
            .Select(g => new { g.GenreId, g.Name })
            .ToListAsync(cancellationToken);
        var directors = await db
            .Directors.AsNoTracking()
            .Select(d => new { d.DirectorId, d.FullName })
            .ToListAsync(cancellationToken);
        var languages = await db
            .Languages.AsNoTracking()
            .Select(l => new { l.LanguageId, l.Name })
            .ToListAsync(cancellationToken);
        var ageRatings = await db
            .AgeRatings.AsNoTracking()
            .Select(a => new { a.AgeRatingId, a.Code })
            .ToListAsync(cancellationToken);
        var actors = await db
            .Actors.AsNoTracking()
            .Select(a => new { a.ActorId, a.FullName })
            .ToListAsync(cancellationToken);
        var movies = await db
            .Movies.AsNoTracking()
            .Select(m => new
            {
                m.MovieId,
                m.Title,
                m.ReleaseDate,
            })
            .ToListAsync(cancellationToken);

        var existingMovies = new Dictionary<string, List<int>>(StringComparer.Ordinal);
        foreach (var movie in movies)
        {
            var key = MovieKey(movie.Title, movie.ReleaseDate);
            if (!existingMovies.TryGetValue(key, out var ids))
            {
                ids = [];
                existingMovies[key] = ids;
            }

            ids.Add(movie.MovieId);
        }

        return new MovieImportContext(
            new NameLookup(genres.Select(g => (g.GenreId, g.Name))),
            new NameLookup(directors.Select(d => (d.DirectorId, d.FullName))),
            new NameLookup(languages.Select(l => (l.LanguageId, l.Name))),
            new NameLookup(ageRatings.Select(a => (a.AgeRatingId, a.Code))),
            new NameLookup(actors.Select(a => (a.ActorId, a.FullName))),
            existingMovies
        );
    }

    public override ImportRow<MovieImportRow> ValidateRow(
        ImportRow<MovieImportRow> row,
        IImportContext context
    )
    {
        var lookups = (MovieImportContext)context;
        var data = row.Data;
        var errors = new List<ImportCellError>();

        var duration = 0;
        if (
            data.DurationMinutes is not null
            && !int.TryParse(
                data.DurationMinutes,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out duration
            )
        )
            errors.Add(
                new ImportCellError(
                    MovieImportColumns.DurationMinutes,
                    Messages.Invalid_ImportInteger
                )
            );

        DateOnly? releaseDate = null;
        var releaseDateInvalid = false;
        if (data.ReleaseDate is not null)
        {
            if (
                DateOnly.TryParseExact(
                    data.ReleaseDate,
                    DateFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsedDate
                )
            )
            {
                releaseDate = parsedDate;
            }
            else
            {
                releaseDateInvalid = true;
                errors.Add(
                    new ImportCellError(
                        MovieImportColumns.ReleaseDate,
                        Messages.Invalid_ImportDateFormat
                    )
                );
            }
        }

        var directorId = ResolveSingle(
            data.Director,
            lookups.Directors,
            MovieImportColumns.Director,
            Messages.Import_LabelDirector,
            errors
        );
        var languageId = ResolveSingle(
            data.Language,
            lookups.Languages,
            MovieImportColumns.Language,
            Messages.Import_LabelLanguage,
            errors
        );
        var ageRatingId = ResolveSingle(
            data.AgeRating,
            lookups.AgeRatings,
            MovieImportColumns.AgeRating,
            Messages.Import_LabelAgeRating,
            errors
        );

        var status = string.Empty;
        if (data.Status is { } statusText)
            status =
                StatusNames.FirstOrDefault(name =>
                    string.Equals(name, statusText, StringComparison.OrdinalIgnoreCase)
                ) ?? statusText;

        var genreIds = new List<int>();
        foreach (var genreName in SplitList(data.Genres))
        {
            var result = lookups.Genres.Find(genreName);
            if (result.Status != LookupStatus.Found)
                errors.Add(
                    ReferenceError(
                        result.Status,
                        MovieImportColumns.Genres,
                        Messages.Import_LabelGenre,
                        genreName
                    )
                );
            else if (!genreIds.Contains(result.Id))
                genreIds.Add(result.Id);
        }

        var cast = new List<MovieCastItemRequest>();
        var castActorIds = new HashSet<int>();
        foreach (var item in SplitList(data.Cast))
        {
            var separator = item.IndexOf(':');
            var actorName = (separator < 0 ? item : item[..separator]).Trim();
            var character = separator < 0 ? null : item[(separator + 1)..].Trim();

            if (actorName.Length == 0)
            {
                errors.Add(
                    new ImportCellError(MovieImportColumns.Cast, Messages.Invalid_ImportCastFormat)
                );
                continue;
            }

            var result = lookups.Actors.Find(actorName);
            if (result.Status != LookupStatus.Found)
            {
                errors.Add(
                    ReferenceError(
                        result.Status,
                        MovieImportColumns.Cast,
                        Messages.Import_LabelActor,
                        actorName
                    )
                );
                continue;
            }

            if (!castActorIds.Add(result.Id))
            {
                errors.Add(
                    new ImportCellError(
                        MovieImportColumns.Cast,
                        string.Format(Messages.Invalid_ImportCastDuplicate, actorName)
                    )
                );
                continue;
            }

            cast.Add(
                new MovieCastItemRequest(result.Id, string.IsNullOrEmpty(character) ? null : character)
            );
        }

        var request = new UpsertMovieRequest(
            data.Title ?? string.Empty,
            data.OriginalTitle,
            data.Description,
            directorId,
            duration,
            releaseDate,
            languageId,
            data.Subtitle,
            ageRatingId,
            data.PosterUrl,
            data.TrailerUrl,
            status,
            genreIds,
            cast
        );

        var reportedColumns = errors.Select(e => e.Column).ToHashSet();
        foreach (var failure in requestValidator.Validate(request).Errors)
        {
            var column = ColumnFor(failure.PropertyName);
            if (!reportedColumns.Contains(column))
                errors.Add(new ImportCellError(column, failure.ErrorMessage));
        }

        int? existingMovieId = null;
        if (data.Title is not null && !releaseDateInvalid)
        {
            var key = MovieKey(data.Title, releaseDate);
            if (lookups.SeenKeys.TryGetValue(key, out var firstRowNumber))
            {
                errors.Add(
                    new ImportCellError(
                        MovieImportColumns.Title,
                        string.Format(Messages.Invalid_ImportDuplicateInFile, firstRowNumber)
                    )
                );
            }
            else
            {
                lookups.SeenKeys[key] = row.RowNumber;
                if (lookups.ExistingMovies.TryGetValue(key, out var movieIds))
                {
                    if (movieIds.Count == 1)
                        existingMovieId = movieIds[0];
                    else
                        errors.Add(
                            new ImportCellError(
                                MovieImportColumns.Title,
                                Messages.Invalid_ImportMovieAmbiguous
                            )
                        );
                }
            }
        }

        return row with
        {
            Data = data with
            {
                Request = errors.Count == 0 ? request : null,
                ExistingMovieId = existingMovieId,
            },
            Errors = errors,
        };
    }

    public override async Task<BulkActionResponse> ApplyRowsAsync(
        IReadOnlyList<ImportRow<MovieImportRow>> rows,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var results = new List<BulkActionResultItem>(rows.Count);

        foreach (var row in rows)
        {
            try
            {
                var request = row.Data.Request!;
                var detail = row.Data.ExistingMovieId is { } movieId
                    ? await movieService.UpdateMovieAsync(movieId, request, cancellationToken)
                    : await movieService.CreateMovieAsync(request, cancellationToken);

                results.Add(new BulkActionResultItem(detail.MovieId, true, null, row.RowNumber));
            }
            catch (AppException ex)
            {
                return await RollbackAsync(transaction, rows, row.RowNumber, ex.Message);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(ex, "Import phim thất bại tại dòng {RowNumber}", row.RowNumber);
                return await RollbackAsync(
                    transaction,
                    rows,
                    row.RowNumber,
                    Messages.Bulk_UnknownError
                );
            }
        }

        await transaction.CommitAsync(cancellationToken);
        return BulkOperation.Build(results);
    }

    private static async Task<BulkActionResponse> RollbackAsync(
        IDbContextTransaction transaction,
        IReadOnlyList<ImportRow<MovieImportRow>> rows,
        int failedRowNumber,
        string reason
    )
    {
        await transaction.RollbackAsync();

        var results = rows.Select(row => new BulkActionResultItem(
                0,
                false,
                row.RowNumber == failedRowNumber ? reason : Messages.Import_RolledBack,
                row.RowNumber
            ))
            .ToList();

        return BulkOperation.Build(results);
    }

    private async Task<IReadOnlyList<IReadOnlyList<object?>>> CatalogRowsAsync(
        CancellationToken cancellationToken
    )
    {
        var genres = await db
            .Genres.AsNoTracking()
            .OrderBy(g => g.Name)
            .Select(g => g.Name)
            .Take(ExportLimits.MaxRows)
            .ToListAsync(cancellationToken);
        var directors = await db
            .Directors.AsNoTracking()
            .OrderBy(d => d.FullName)
            .Select(d => d.FullName)
            .Take(ExportLimits.MaxRows)
            .ToListAsync(cancellationToken);
        var languages = await db
            .Languages.AsNoTracking()
            .OrderBy(l => l.Name)
            .Select(l => l.Name)
            .Take(ExportLimits.MaxRows)
            .ToListAsync(cancellationToken);
        var ageRatings = await db
            .AgeRatings.AsNoTracking()
            .OrderBy(a => a.Code)
            .Select(a => a.Code)
            .Take(ExportLimits.MaxRows)
            .ToListAsync(cancellationToken);
        var actors = await db
            .Actors.AsNoTracking()
            .OrderBy(a => a.FullName)
            .Select(a => a.FullName)
            .Take(ExportLimits.MaxRows)
            .ToListAsync(cancellationToken);

        var columns = new[] { genres, directors, languages, ageRatings, actors };
        var rowCount = columns.Max(column => column.Count);
        var rows = new List<IReadOnlyList<object?>>(rowCount);

        for (var i = 0; i < rowCount; i++)
        {
            var index = i;
            rows.Add(
                columns.Select(column => index < column.Count ? (object?)column[index] : null).ToArray()
            );
        }

        return rows;
    }

    private IReadOnlyList<IReadOnlyList<object?>> ExampleRows() =>
        ExampleData
            .Select(example =>
                (IReadOnlyList<object?>)
                    Columns.Select(c => (object?)example.GetValueOrDefault(c.Name)).ToArray()
            )
            .ToList();

    private IReadOnlyList<IReadOnlyList<object?>> GuideRows()
    {
        var rows = Columns
            .Select(c =>
                (IReadOnlyList<object?>)
                    new object?[]
                    {
                        c.Name,
                        c.Required ? Messages.Import_Required : Messages.Import_Optional,
                        c.Description,
                        c.Format,
                    }
            )
            .ToList();

        rows.Add(new object?[4]);
        rows.Add(
            GuideNote(
                string.Format(
                    Messages.Import_Note_Limits,
                    ImportLimits.MaxRows,
                    ImportLimits.MaxFileBytes / (1024 * 1024)
                )
            )
        );
        rows.Add(GuideNote(Messages.Import_Note_Upsert));
        rows.Add(GuideNote(Messages.Import_Note_Lookup));
        rows.Add(GuideNote(Messages.Import_Note_Examples));

        return rows;
    }

    private static object?[] GuideNote(string text) =>
        [Messages.Import_GuideNote, null, text, null];

    private static string[] SplitList(string? value) =>
        value?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        ?? [];

    private static int? ResolveSingle(
        string? name,
        NameLookup lookup,
        string column,
        string label,
        List<ImportCellError> errors
    )
    {
        if (name is null)
            return null;

        var result = lookup.Find(name);
        if (result.Status == LookupStatus.Found)
            return result.Id;

        errors.Add(ReferenceError(result.Status, column, label, name));
        return null;
    }

    private static ImportCellError ReferenceError(
        LookupStatus status,
        string column,
        string label,
        string name
    ) =>
        new(
            column,
            string.Format(
                status == LookupStatus.Ambiguous
                    ? Messages.Invalid_ImportReferenceAmbiguous
                    : Messages.Invalid_ImportReferenceNotFound,
                label,
                name
            )
        );

    private static string ColumnFor(string propertyName)
    {
        var root = propertyName.Split('.', '[')[0];
        return root switch
        {
            nameof(UpsertMovieRequest.DirectorId) => MovieImportColumns.Director,
            nameof(UpsertMovieRequest.LanguageId) => MovieImportColumns.Language,
            nameof(UpsertMovieRequest.AgeRatingId) => MovieImportColumns.AgeRating,
            nameof(UpsertMovieRequest.GenreIds) => MovieImportColumns.Genres,
            nameof(UpsertMovieRequest.Cast) => MovieImportColumns.Cast,
            _ => root,
        };
    }

    private static string MovieKey(string? title, DateOnly? releaseDate) =>
        $"{NameLookup.Key(title)}|{releaseDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";

    private enum LookupStatus
    {
        Found,
        NotFound,
        Ambiguous,
    }

    private readonly record struct LookupResult(LookupStatus Status, int Id);

    private sealed class NameLookup(IEnumerable<(int Id, string Name)> items)
    {
        private readonly Dictionary<string, List<int>> map = Build(items);

        public static string Key(string? value) => NameNormalizer.Normalize(value).ToLowerInvariant();

        public LookupResult Find(string name)
        {
            if (!map.TryGetValue(Key(name), out var ids))
                return new LookupResult(LookupStatus.NotFound, 0);

            return ids.Count == 1
                ? new LookupResult(LookupStatus.Found, ids[0])
                : new LookupResult(LookupStatus.Ambiguous, 0);
        }

        private static Dictionary<string, List<int>> Build(IEnumerable<(int Id, string Name)> items)
        {
            var result = new Dictionary<string, List<int>>(StringComparer.Ordinal);
            foreach (var (id, name) in items)
            {
                var key = Key(name);
                if (!result.TryGetValue(key, out var ids))
                {
                    ids = [];
                    result[key] = ids;
                }

                ids.Add(id);
            }

            return result;
        }
    }

    private sealed class MovieImportContext(
        NameLookup genres,
        NameLookup directors,
        NameLookup languages,
        NameLookup ageRatings,
        NameLookup actors,
        Dictionary<string, List<int>> existingMovies
    ) : IImportContext
    {
        public NameLookup Genres { get; } = genres;
        public NameLookup Directors { get; } = directors;
        public NameLookup Languages { get; } = languages;
        public NameLookup AgeRatings { get; } = ageRatings;
        public NameLookup Actors { get; } = actors;
        public Dictionary<string, List<int>> ExistingMovies { get; } = existingMovies;
        public Dictionary<string, int> SeenKeys { get; } = new(StringComparer.Ordinal);
    }
}

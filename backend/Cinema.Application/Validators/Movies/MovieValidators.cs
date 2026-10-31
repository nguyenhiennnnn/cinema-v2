using Cinema.Application.DTOs.Movies;
using Cinema.Application.Options;
using Microsoft.Extensions.Options;
using Cinema.Domain.Enums;
using FluentValidation;

namespace Cinema.Application.Validators.Movies;

public class MovieCastItemRequestValidator : AbstractValidator<MovieCastItemRequest>
{
    public MovieCastItemRequestValidator()
    {
        RuleFor(x => x.ActorId)
            .GreaterThan(0)
            .WithMessage("actorId phải lớn hơn 0.");

        RuleFor(x => x.CharacterName)
            .MaximumLength(150)
            .WithMessage("characterName không được vượt quá 150 ký tự.");
    }
}

public class UpsertMovieRequestValidator : AbstractValidator<UpsertMovieRequest>
{
    private static readonly string[] ValidStatuses = Enum.GetNames<MovieStatus>();

    public UpsertMovieRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Tiêu đề phim không được để trống.")
            .MaximumLength(200)
            .WithMessage("Tiêu đề phim không được vượt quá 200 ký tự.");

        RuleFor(x => x.OriginalTitle)
            .MaximumLength(200)
            .WithMessage("Tên gốc không được vượt quá 200 ký tự.");

        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(1, 600)
            .WithMessage("Thời lượng phim phải từ 1 đến 600 phút.");

        RuleFor(x => x.DirectorId)
            .GreaterThan(0)
            .When(x => x.DirectorId is not null)
            .WithMessage("directorId phải lớn hơn 0.");

        RuleFor(x => x.LanguageId)
            .GreaterThan(0)
            .When(x => x.LanguageId is not null)
            .WithMessage("languageId phải lớn hơn 0.");

        RuleFor(x => x.AgeRatingId)
            .GreaterThan(0)
            .When(x => x.AgeRatingId is not null)
            .WithMessage("ageRatingId phải lớn hơn 0.");

        RuleFor(x => x.Subtitle)
            .MaximumLength(50)
            .WithMessage("Phụ đề không được vượt quá 50 ký tự.");

        RuleFor(x => x.PosterUrl)
            .MaximumLength(255)
            .WithMessage("posterUrl không được vượt quá 255 ký tự.");

        RuleFor(x => x.TrailerUrl)
            .MaximumLength(255)
            .WithMessage("trailerUrl không được vượt quá 255 ký tự.");

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Trạng thái phim không được để trống.")
            .Must(s => ValidStatuses.Contains(s))
            .WithMessage($"Status phải là một trong: {string.Join(", ", ValidStatuses)}");

        RuleFor(x => x.GenreIds)
            .NotEmpty()
            .WithMessage("Phải chọn ít nhất 1 thể loại phim.");

        RuleForEach(x => x.GenreIds)
            .GreaterThan(0)
            .WithMessage("genreId phải lớn hơn 0.");

        RuleFor(x => x.Cast)
            .NotNull()
            .WithMessage("Danh sách diễn viên không được null.");

        RuleForEach(x => x.Cast)
            .SetValidator(new MovieCastItemRequestValidator());
    }
}

public class BulkDeleteMoviesRequestValidator : AbstractValidator<BulkDeleteMoviesRequest>
{
    public BulkDeleteMoviesRequestValidator(IOptions<BookingOptions> options)
    {
        var limit = options.Value.BulkDeleteLimit;
        RuleFor(x => x.MovieIds)
            .NotEmpty()
            .WithMessage("Danh s\u00e1ch movieIds kh\u00f4ng \u0111\u01b0\u1ee3c r\u1ed7ng.");
        RuleFor(x => x.MovieIds)
            .Must(ids => ids.Count <= limit)
            .WithMessage($"T\u1ed1i \u0111a {{limit}} phim m\u1ed7i l\u1ea7n.");
        RuleForEach(x => x.MovieIds)
            .GreaterThan(0)
            .WithMessage("movieId ph\u1ea3i l\u1edbn h\u01a1n 0.");
    }
}

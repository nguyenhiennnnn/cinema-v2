using Cinema.Application.DTOs.Movies;
using Cinema.Application.Options;
using Cinema.Domain.Enums;
using Cinema.Shared.Resources;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Validators.Movies;

public class MovieCastItemRequestValidator : AbstractValidator<MovieCastItemRequest>
{
    public MovieCastItemRequestValidator()
    {
        RuleFor(x => x.ActorId).GreaterThan(0).WithMessage(Messages.Validation_ActorId_Positive);

        RuleFor(x => x.CharacterName)
            .MaximumLength(150)
            .WithMessage(Messages.Validation_CharacterName_MaxLength150);
    }
}

public class UpsertMovieRequestValidator : AbstractValidator<UpsertMovieRequest>
{
    private static readonly string[] ValidStatuses = Enum.GetNames<MovieStatus>();

    public UpsertMovieRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(Messages.Validation_MovieTitle_Required)
            .MaximumLength(200)
            .WithMessage(Messages.Validation_MovieTitle_MaxLength200);

        RuleFor(x => x.OriginalTitle)
            .MaximumLength(200)
            .WithMessage(Messages.Validation_OriginalTitle_MaxLength200);

        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(1, 600)
            .WithMessage(Messages.Validation_MovieDuration_Range1To600);

        RuleFor(x => x.DirectorId)
            .GreaterThan(0)
            .When(x => x.DirectorId is not null)
            .WithMessage(Messages.Validation_DirectorId_Positive);

        RuleFor(x => x.LanguageId)
            .GreaterThan(0)
            .When(x => x.LanguageId is not null)
            .WithMessage(Messages.Validation_LanguageId_Positive);

        RuleFor(x => x.AgeRatingId)
            .GreaterThan(0)
            .When(x => x.AgeRatingId is not null)
            .WithMessage(Messages.Validation_AgeRatingId_Positive);

        RuleFor(x => x.Subtitle)
            .MaximumLength(50)
            .WithMessage(Messages.Validation_Subtitle_MaxLength50);

        RuleFor(x => x.PosterUrl)
            .MaximumLength(255)
            .WithMessage(Messages.Validation_PosterUrl_MaxLength255);

        RuleFor(x => x.TrailerUrl)
            .MaximumLength(255)
            .WithMessage(Messages.Validation_TrailerUrl_MaxLength255);

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage(Messages.Validation_MovieStatus_Required)
            .Must(s => ValidStatuses.Contains(s))
            .WithMessage(
                string.Format(
                    Messages.Validation_Status_AllowedValues,
                    string.Join(", ", ValidStatuses)
                )
            );

        RuleFor(x => x.GenreIds).NotEmpty().WithMessage(Messages.Validation_MovieGenres_AtLeastOne);

        RuleForEach(x => x.GenreIds)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_GenreId_Positive);

        RuleFor(x => x.Cast).NotNull().WithMessage(Messages.Validation_ActorList_NotNull);

        RuleForEach(x => x.Cast).SetValidator(new MovieCastItemRequestValidator());
    }
}

public class BulkDeleteMoviesRequestValidator : AbstractValidator<BulkDeleteMoviesRequest>
{
    public BulkDeleteMoviesRequestValidator(IOptions<BookingOptions> options)
    {
        var limit = options.Value.BulkDeleteLimit;
        RuleFor(x => x.MovieIds).NotEmpty().WithMessage(Messages.Validation_MovieIds_NotEmpty);
        RuleFor(x => x.MovieIds)
            .Must(ids => ids.Count <= limit)
            .WithMessage(string.Format(Messages.Validation_MovieIds_MaxCount, limit));
        RuleForEach(x => x.MovieIds)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_MovieId_Positive);
    }
}

using Cinema.Application.DTOs.Catalog;
using Cinema.Application.Validators.Common;
using FluentValidation;

namespace Cinema.Application.Validators.Catalog;

public class UpsertLanguageRequestValidator : AbstractValidator<UpsertLanguageRequest>
{
    public UpsertLanguageRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50).NoControlCharacters();
    }
}

public class UpsertGenreRequestValidator : AbstractValidator<UpsertGenreRequest>
{
    public UpsertGenreRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50).NoControlCharacters();
    }
}

public class UpsertScreenTypeRequestValidator : AbstractValidator<UpsertScreenTypeRequest>
{
    public UpsertScreenTypeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50).NoControlCharacters();
    }
}

public class UpsertSeatTypeRequestValidator : AbstractValidator<UpsertSeatTypeRequest>
{
    public UpsertSeatTypeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PriceMultiplier).GreaterThan(0);
    }
}

public class UpsertMembershipTierRequestValidator : AbstractValidator<UpsertMembershipTierRequest>
{
    public UpsertMembershipTierRequestValidator()
    {
        RuleFor(x => x.TierName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.MinPoints).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DiscountPercent).InclusiveBetween(0, 100);
    }
}

public class UpsertAgeRatingRequestValidator : AbstractValidator<UpsertAgeRatingRequest>
{
    public UpsertAgeRatingRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10).NoControlCharacters();
        RuleFor(x => x.Description).MaximumLength(255);
    }
}

public class UpsertPersonRequestValidator : AbstractValidator<UpsertPersonRequest>
{
    public UpsertPersonRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150).NoControlCharacters();
        RuleFor(x => x.AvatarUrl).MaximumLength(255);
    }
}

public class UpsertPositionRequestValidator : AbstractValidator<UpsertPositionRequest>
{
    public UpsertPositionRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50).NoControlCharacters();
    }
}

public class QuickCreatePersonRequestValidator : AbstractValidator<QuickCreatePersonRequest>
{
    public QuickCreatePersonRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150).NoControlCharacters();
    }
}

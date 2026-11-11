using Cinema.Application.DTOs.Bookings;
using Cinema.Application.Utils;
using Cinema.Shared.Resources;
using FluentValidation;

namespace Cinema.Application.Validators.Bookings;

public class CounterHoldRequestValidator : AbstractValidator<CounterHoldRequest>
{
    public CounterHoldRequestValidator()
    {
        RuleFor(x => x.ShowtimeId)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_ShowtimeId_Positive);

        RuleFor(x => x.SeatIds).NotEmpty().WithMessage(Messages.Validation_Seats_AtLeastOne);

        RuleForEach(x => x.SeatIds).GreaterThan(0).WithMessage(Messages.Validation_SeatId_Positive);

        RuleFor(x => x.SeatIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage(Messages.Validation_Seats_Duplicated);

        RuleForEach(x => x.Combos).SetValidator(new ComboSelectionRequestValidator());

        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .When(x => x.UserId is not null)
            .WithMessage(Messages.Validation_UserId_Positive);

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .When(x => x.UserId is null)
            .WithMessage(Messages.Validation_UserIdOrCustomerNameRequired)
            .WithName("customerName");

        RuleFor(x => x.CustomerName).MaximumLength(100).When(x => x.CustomerName is not null);

        RuleFor(x => x.CustomerPhone)
            .Matches(PhoneRules.Pattern)
            .When(x => !string.IsNullOrEmpty(x.CustomerPhone))
            .WithMessage(Messages.Validation_Phone_Invalid)
            .MaximumLength(20);
    }
}

public class CounterCheckoutRequestValidator : AbstractValidator<CounterCheckoutRequest>
{
    public CounterCheckoutRequestValidator()
    {
        RuleFor(x => x.AmountReceived)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_AmountReceived_Positive);
    }
}

public class CheckinRequestValidator : AbstractValidator<CheckinRequest>
{
    public CheckinRequestValidator()
    {
        RuleFor(x => x.QrCode).NotEmpty().WithMessage(Messages.Validation_QrCode_Required);
    }
}

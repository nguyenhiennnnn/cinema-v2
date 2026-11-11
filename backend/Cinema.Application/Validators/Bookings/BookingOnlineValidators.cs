using Cinema.Application.DTOs.Bookings;
using Cinema.Application.Utils;
using Cinema.Shared.Resources;
using FluentValidation;

namespace Cinema.Application.Validators.Bookings;

public class ComboSelectionRequestValidator : AbstractValidator<ComboSelectionRequest>
{
    // Mức tối đa của số lượng combo đọc từ CinemaSettings (BookingHoldService), không kiểm tra ở đây.
    public ComboSelectionRequestValidator()
    {
        RuleFor(x => x.ComboId)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_ComboId_Positive);

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_Quantity_Positive);
    }
}

public class HoldSeatsRequestValidator : AbstractValidator<HoldSeatsRequest>
{
    public HoldSeatsRequestValidator()
    {
        RuleFor(x => x.ShowtimeId)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_ShowtimeId_Positive);

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage(Messages.Validation_CustomerName_Required)
            .MaximumLength(100)
            .WithMessage(Messages.Validation_CustomerName_MaxLength100);

        RuleFor(x => x.CustomerPhone)
            .Matches(PhoneRules.Pattern)
            .When(x => !string.IsNullOrEmpty(x.CustomerPhone))
            .WithMessage(Messages.Validation_Phone_Invalid)
            .MaximumLength(20);

        RuleFor(x => x.SeatIds)
            .NotEmpty()
            .WithMessage(Messages.Validation_Seats_AtLeastOne);

        RuleForEach(x => x.SeatIds)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_SeatId_Positive);

        RuleFor(x => x.SeatIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage(Messages.Validation_Seats_Duplicated);

        RuleForEach(x => x.Combos)
            .SetValidator(new ComboSelectionRequestValidator());
    }
}

public class CheckoutRequestValidator : AbstractValidator<CheckoutRequest>
{
    private static readonly string[] ValidMethods = ["VNPay", "Momo", "Stripe"];

    public CheckoutRequestValidator()
    {
        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .Must(m => ValidMethods.Contains(m))
            .WithMessage(Messages.Validation_PaymentMethod_Allowed);
    }
}

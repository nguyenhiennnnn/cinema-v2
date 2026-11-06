using System.Text.RegularExpressions;
using Cinema.Application.DTOs.Bookings;
using Cinema.Shared.Resources;
using FluentValidation;

namespace Cinema.Application.Validators.Bookings;

public class ComboSelectionRequestValidator : AbstractValidator<ComboSelectionRequest>
{
    // Mức tối đa của số lượng combo đọc từ CinemaSettings (BookingService), không kiểm tra ở đây.
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

public partial class HoldSeatsRequestValidator : AbstractValidator<HoldSeatsRequest>
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
            .Matches(PhoneRegex())
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

    [GeneratedRegex(@"^(0|\+84)[0-9]{9,10}$")]
    private static partial Regex PhoneRegex();
}

public partial class CounterHoldRequestValidator : AbstractValidator<CounterHoldRequest>
{
    public CounterHoldRequestValidator()
    {
        RuleFor(x => x.ShowtimeId)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_ShowtimeId_Positive);

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

        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .When(x => x.UserId is not null)
            .WithMessage(Messages.Validation_UserId_Positive);

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .When(x => x.UserId is null)
            .WithMessage(Messages.Validation_UserIdOrCustomerNameRequired)
            .WithName("customerName");

        RuleFor(x => x.CustomerName)
            .MaximumLength(100)
            .When(x => x.CustomerName is not null);

        RuleFor(x => x.CustomerPhone)
            .Matches(PhoneRegex())
            .When(x => !string.IsNullOrEmpty(x.CustomerPhone))
            .WithMessage(Messages.Validation_Phone_Invalid)
            .MaximumLength(20);
    }

    [GeneratedRegex(@"^(0|\+84)[0-9]{9,10}$")]
    private static partial Regex PhoneRegex();
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
        RuleFor(x => x.QrCode)
            .NotEmpty()
            .WithMessage(Messages.Validation_QrCode_Required);
    }
}

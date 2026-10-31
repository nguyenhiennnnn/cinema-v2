using System.Text.RegularExpressions;
using Cinema.Application.DTOs.Bookings;
using Cinema.Application.Options;
using Cinema.Shared.Resources;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Validators.Bookings;

public class ComboSelectionRequestValidator : AbstractValidator<ComboSelectionRequest>
{
    public ComboSelectionRequestValidator(IOptions<BookingOptions>? options = null)
    {
        var maxQuantity = (options?.Value ?? new BookingOptions()).MaxComboQuantity;

        RuleFor(x => x.ComboId)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_ComboId_Positive);

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_Quantity_Positive)
            .LessThanOrEqualTo(maxQuantity)
            .WithMessage(string.Format(Messages.Validation_Quantity_Max, maxQuantity));
    }
}

public partial class HoldSeatsRequestValidator : AbstractValidator<HoldSeatsRequest>
{
    public HoldSeatsRequestValidator(IOptions<BookingOptions>? options = null)
    {
        var bookingOptions = options ?? Microsoft.Extensions.Options.Options.Create(new BookingOptions());
        var maxSeats = bookingOptions.Value.MaxSeatsPerBooking;

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

        RuleFor(x => x.SeatIds)
            .Must(ids => ids.Count <= maxSeats)
            .WithMessage(string.Format(Messages.Validation_Seats_MaxPerBooking, maxSeats));

        RuleForEach(x => x.Combos)
            .SetValidator(new ComboSelectionRequestValidator(bookingOptions));
    }

    [GeneratedRegex(@"^(0|\+84)[0-9]{9,10}$")]
    private static partial Regex PhoneRegex();
}

public partial class CounterHoldRequestValidator : AbstractValidator<CounterHoldRequest>
{
    public CounterHoldRequestValidator(IOptions<BookingOptions>? options = null)
    {
        var bookingOptions = options ?? Microsoft.Extensions.Options.Options.Create(new BookingOptions());
        var maxSeats = bookingOptions.Value.MaxSeatsPerBooking;

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

        RuleFor(x => x.SeatIds)
            .Must(ids => ids.Count <= maxSeats)
            .WithMessage(string.Format(Messages.Validation_Seats_MaxPerBooking, maxSeats));

        RuleForEach(x => x.Combos)
            .SetValidator(new ComboSelectionRequestValidator(bookingOptions));

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

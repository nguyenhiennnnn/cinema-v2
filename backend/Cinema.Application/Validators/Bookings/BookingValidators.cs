using System.Text.RegularExpressions;
using Cinema.Application.DTOs.Bookings;
using FluentValidation;

namespace Cinema.Application.Validators.Bookings;

public class ComboSelectionRequestValidator : AbstractValidator<ComboSelectionRequest>
{
    public ComboSelectionRequestValidator()
    {
        RuleFor(x => x.ComboId)
            .GreaterThan(0)
            .WithMessage("comboId phải lớn hơn 0.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("quantity phải lớn hơn 0.");
    }
}

public partial class HoldSeatsRequestValidator : AbstractValidator<HoldSeatsRequest>
{
    public HoldSeatsRequestValidator()
    {
        RuleFor(x => x.ShowtimeId)
            .GreaterThan(0)
            .WithMessage("showtimeId phải lớn hơn 0.");

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("customerName không được để trống.")
            .MaximumLength(100)
            .WithMessage("customerName không được vượt quá 100 ký tự.");

        RuleFor(x => x.CustomerPhone)
            .Matches(PhoneRegex())
            .When(x => !string.IsNullOrEmpty(x.CustomerPhone))
            .WithMessage("Số điện thoại không hợp lệ.")
            .MaximumLength(20);

        RuleFor(x => x.SeatIds)
            .NotEmpty()
            .WithMessage("Phải chọn ít nhất 1 ghế.");

        RuleForEach(x => x.SeatIds)
            .GreaterThan(0)
            .WithMessage("seatId phải lớn hơn 0.");

        RuleFor(x => x.SeatIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Danh sách ghế bị trùng lặp.");

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
            .WithMessage("showtimeId phải lớn hơn 0.");

        RuleFor(x => x.SeatIds)
            .NotEmpty()
            .WithMessage("Phải chọn ít nhất 1 ghế.");

        RuleForEach(x => x.SeatIds)
            .GreaterThan(0)
            .WithMessage("seatId phải lớn hơn 0.");

        RuleFor(x => x.SeatIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Danh sách ghế bị trùng lặp.");

        RuleForEach(x => x.Combos)
            .SetValidator(new ComboSelectionRequestValidator());

        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .When(x => x.UserId is not null)
            .WithMessage("userId phải lớn hơn 0.");

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .When(x => x.UserId is null)
            .WithMessage("Phải cung cấp userId hoặc customerName.")
            .WithName("customerName");

        RuleFor(x => x.CustomerName)
            .MaximumLength(100)
            .When(x => x.CustomerName is not null);

        RuleFor(x => x.CustomerPhone)
            .Matches(PhoneRegex())
            .When(x => !string.IsNullOrEmpty(x.CustomerPhone))
            .WithMessage("Số điện thoại không hợp lệ.")
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
            .WithMessage("paymentMethod phải là VNPay, Momo hoặc Stripe.");
    }
}

public class CounterCheckoutRequestValidator : AbstractValidator<CounterCheckoutRequest>
{
    public CounterCheckoutRequestValidator()
    {
        RuleFor(x => x.AmountReceived)
            .GreaterThan(0)
            .WithMessage("amountReceived phải lớn hơn 0.");
    }
}

public class CheckinRequestValidator : AbstractValidator<CheckinRequest>
{
    public CheckinRequestValidator()
    {
        RuleFor(x => x.QrCode)
            .NotEmpty()
            .WithMessage("qrCode không được để trống.");
    }
}

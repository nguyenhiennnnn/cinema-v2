using System.Globalization;
using Cinema.Application.DTOs.Settings;
using FluentValidation;

namespace Cinema.Application.Validators.Settings;

public class UpdateSettingsRequestValidator : AbstractValidator<UpdateSettingsRequest>
{
    public UpdateSettingsRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên rạp không được để trống.")
            .MaximumLength(150)
            .WithMessage("Tên rạp không được vượt quá 150 ký tự.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Địa chỉ không được để trống.")
            .MaximumLength(255)
            .WithMessage("Địa chỉ không được vượt quá 255 ký tự.");

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .WithMessage("Số điện thoại không được vượt quá 20 ký tự.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email không đúng định dạng.")
            .MaximumLength(150)
            .WithMessage("Email không được vượt quá 150 ký tự.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.TaxCode)
            .MaximumLength(50)
            .WithMessage("Mã số thuế không được vượt quá 50 ký tự.");

        RuleFor(x => x.LogoUrl)
            .MaximumLength(255)
            .WithMessage("logoUrl không được vượt quá 255 ký tự.");

        RuleFor(x => x.OpeningTime)
            .Must(BeValidTime)
            .When(x => x.OpeningTime is not null)
            .WithMessage("openingTime phải có định dạng HH:mm.");

        RuleFor(x => x.ClosingTime)
            .Must(BeValidTime)
            .When(x => x.ClosingTime is not null)
            .WithMessage("closingTime phải có định dạng HH:mm.");

        RuleFor(x => x.PointsPerAmount).GreaterThan(0).WithMessage("pointsPerAmount phải > 0.");

        RuleFor(x => x.CancelRefundPercent)
            .InclusiveBetween(0, 100)
            .WithMessage("cancelRefundPercent phải từ 0 đến 100.");

        RuleFor(x => x.CancelDeadlineHours)
            .GreaterThanOrEqualTo(0)
            .WithMessage("cancelDeadlineHours phải >= 0.");

        RuleFor(x => x.StopSellingMinutesBefore)
            .InclusiveBetween(0, 1440)
            .WithMessage("stopSellingMinutesBefore phải từ 0 đến 1440.");

        RuleFor(x => x.HoldDurationMinutes)
            .Must(v => v is >= 1 and <= 120)
            .When(x => x.HoldDurationMinutes is not null)
            .WithMessage("holdDurationMinutes phải từ 1 đến 120.");

        RuleFor(x => x.CheckInOpenMinutes)
            .Must(v => v is >= 0 and <= 240)
            .When(x => x.CheckInOpenMinutes is not null)
            .WithMessage("checkInOpenMinutes phải từ 0 đến 240.");
    }

    private static bool BeValidTime(string? value) =>
        value is not null
        && TimeOnly.TryParseExact(
            value,
            "HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _
        );
}

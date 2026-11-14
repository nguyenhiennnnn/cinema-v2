using System.Globalization;
using Cinema.Application.DTOs.Settings;
using Cinema.Application.Validators.Auth;
using Cinema.Domain.Enums;
using Cinema.Shared.Resources;
using FluentValidation;

namespace Cinema.Application.Validators.Settings;

public class UpdateSettingsRequestValidator : AbstractValidator<UpdateSettingsRequest>
{
    public UpdateSettingsRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(Messages.Validation_CinemaName_Required)
            .MaximumLength(150)
            .WithMessage(Messages.Validation_CinemaName_MaxLength150);

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage(Messages.Validation_Address_Required)
            .MaximumLength(255)
            .WithMessage(Messages.Validation_Address_MaxLength255);

        RuleFor(x => x.Phone).MaximumLength(20).WithMessage(Messages.Validation_Phone_MaxLength20);

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage(Messages.Validation_Email_Invalid)
            .MaximumLength(150)
            .WithMessage(Messages.Validation_Email_MaxLength150)
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.TaxCode)
            .MaximumLength(50)
            .WithMessage(Messages.Validation_TaxCode_MaxLength50);

        RuleFor(x => x.LogoUrl)
            .MaximumLength(255)
            .WithMessage(Messages.Validation_LogoUrl_MaxLength255);

        RuleFor(x => x.OpeningTime)
            .Must(BeValidTime)
            .When(x => x.OpeningTime is not null)
            .WithMessage(Messages.Validation_OpeningTime_TimeFormat);

        RuleFor(x => x.ClosingTime)
            .Must(BeValidTime)
            .When(x => x.ClosingTime is not null)
            .WithMessage(Messages.Validation_ClosingTime_TimeFormat);

        RuleFor(x => x.PointsPerAmount)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_PointsPerAmount_GreaterThanZero);

        RuleFor(x => x.CancelRefundPercent)
            .InclusiveBetween(0, 100)
            .WithMessage(Messages.Validation_CancelRefundPercent_Range0To100);

        RuleFor(x => x.CancelDeadlineHours)
            .GreaterThanOrEqualTo(0)
            .WithMessage(Messages.Validation_CancelDeadlineHours_NonNegative);

        RuleFor(x => x.StopSellingMinutesBefore)
            .InclusiveBetween(0, 1440)
            .WithMessage(Messages.Validation_StopSellingMinutesBefore_Range0To1440);

        RuleFor(x => x.HoldDurationMinutes)
            .Must(v => v is >= 1 and <= 120)
            .When(x => x.HoldDurationMinutes is not null)
            .WithMessage(Messages.Validation_HoldDurationMinutes_Range1To120);

        RuleFor(x => x.CheckInOpenMinutes)
            .Must(v => v is >= 0 and <= 240)
            .When(x => x.CheckInOpenMinutes is not null)
            .WithMessage(Messages.Validation_CheckInOpenMinutes_Range0To240);

        RuleFor(x => x.SeatGapRuleMode)
            .Must(v => Enum.TryParse<SeatGapRuleMode>(v, out _) && !int.TryParse(v, out _))
            .When(x => x.SeatGapRuleMode is not null)
            .WithMessage(Messages.Validation_SeatGapRuleMode_Invalid);

        RuleFor(x => x.DefaultEmployeePassword!)
            .StrongPassword()
            .MaximumLength(100)
            .WithMessage(Messages.Validation_DefaultEmployeePassword_MaxLength100)
            .When(x => x.DefaultEmployeePassword is not null);

        IntRange(x => x.RoomBufferMinutes, "roomBufferMinutes", 0, 120);
        IntRange(x => x.MaxSeatsPerBooking, "maxSeatsPerBooking", 1, 50);
        IntRange(x => x.MaxComboQuantity, "maxComboQuantity", 1, 50);
        IntRange(x => x.LoyaltyWindowMonths, "loyaltyWindowMonths", 1, 120);
        IntRange(x => x.MovieEndGraceDays, "movieEndGraceDays", 0, 365);
        IntRange(x => x.ShowtimeLookaheadDays, "showtimeLookaheadDays", 1, 60);
        IntRange(x => x.SchedulerStepMinutes, "schedulerStepMinutes", 1, 60);
        IntRange(
            x => x.SchedulerMinSameMovieGapMinutes,
            "schedulerMinSameMovieGapMinutes",
            0,
            1440
        );
        IntRange(x => x.SchedulerHeatLookbackDays, "schedulerHeatLookbackDays", 1, 365);
        DoubleRange(x => x.SchedulerHeatSalesWeight, "schedulerHeatSalesWeight", 0, 10);
        DoubleRange(x => x.SchedulerHeatRatingWeight, "schedulerHeatRatingWeight", 0, 10);
        DoubleRange(x => x.SchedulerMaxRating, "schedulerMaxRating", 1, 10);

        RuleFor(x => x)
            .Must(x => x.SchedulerHeatSalesWeight!.Value + x.SchedulerHeatRatingWeight!.Value > 0)
            .When(x =>
                x.SchedulerHeatSalesWeight is not null && x.SchedulerHeatRatingWeight is not null
            )
            .WithName("schedulerHeatSalesWeight")
            .WithMessage(Messages.Validation_SchedulerHeatWeights_SumPositive);
    }

    private void IntRange(
        System.Linq.Expressions.Expression<Func<UpdateSettingsRequest, int?>> selector,
        string name,
        int min,
        int max
    )
    {
        var get = selector.Compile();
        RuleFor(selector)
            .Must(v => v is not null && v >= min && v <= max)
            .When(x => get(x) is not null)
            .WithMessage(string.Format(Messages.Validation_Setting_NumberRange, name, min, max));
    }

    private void DoubleRange(
        System.Linq.Expressions.Expression<Func<UpdateSettingsRequest, double?>> selector,
        string name,
        double min,
        double max
    )
    {
        var get = selector.Compile();
        RuleFor(selector)
            .Must(v => v is not null && !double.IsNaN(v.Value) && v >= min && v <= max)
            .When(x => get(x) is not null)
            .WithMessage(string.Format(Messages.Validation_Setting_NumberRange, name, min, max));
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

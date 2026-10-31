using Cinema.Application.DTOs.Showtimes;
using Cinema.Application.Interfaces;
using Cinema.Shared.Resources;
using FluentValidation;

namespace Cinema.Application.Validators.Showtimes;

public class AdminShowtimeCalendarFilterValidator : AbstractValidator<AdminShowtimeCalendarFilter>
{
    private const int MaxRangeDays = 92;

    public AdminShowtimeCalendarFilterValidator()
    {
        RuleFor(x => x.From).NotEmpty().WithMessage(Messages.Validation_From_Required);

        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage(Messages.Validation_To_Required)
            .GreaterThanOrEqualTo(x => x.From)
            .WithMessage(Messages.Validation_To_BeforeFrom);

        RuleFor(x => x)
            .Must(x => x.To.DayNumber - x.From.DayNumber <= MaxRangeDays)
            .WithMessage(string.Format(Messages.Validation_DateRange_TooLong, MaxRangeDays));
    }
}

public class UpsertShowtimeRequestValidator : AbstractValidator<UpsertShowtimeRequest>
{
    public UpsertShowtimeRequestValidator()
    {
        RuleFor(x => x.MovieId).GreaterThan(0).WithMessage(Messages.Validation_MovieId_Positive);

        RuleFor(x => x.RoomId).GreaterThan(0).WithMessage(Messages.Validation_RoomId_Positive);

        RuleFor(x => x.StartTime).NotEmpty().WithMessage(Messages.Validation_StartTime_Required);

        RuleFor(x => x.BasePrice).GreaterThanOrEqualTo(0).WithMessage(Messages.Validation_BasePrice_NonNegative);
    }
}

public class BulkDateRangeValidator : AbstractValidator<BulkDateRange>
{
    public BulkDateRangeValidator()
    {
        RuleFor(x => x.From).NotEmpty().WithMessage(Messages.Validation_DateRangeFrom_Required);

        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage(Messages.Validation_DateRangeTo_Required)
            .GreaterThanOrEqualTo(x => x.From)
            .WithMessage(Messages.Validation_DateRangeTo_BeforeFrom);
    }
}

public class BulkCreateShowtimesRequestValidator : AbstractValidator<BulkCreateShowtimesRequest>
{
    private static readonly string[] ValidDays = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

    public BulkCreateShowtimesRequestValidator()
    {
        RuleFor(x => x.MovieId).GreaterThan(0).WithMessage(Messages.Validation_MovieId_Positive);

        RuleFor(x => x.RoomId).GreaterThan(0).WithMessage(Messages.Validation_RoomId_Positive);

        RuleFor(x => x.DateRange)
            .NotNull()
            .WithMessage(Messages.Validation_DateRange_NotNull)
            .SetValidator(new BulkDateRangeValidator());

        RuleFor(x => x.DaysOfWeek).NotEmpty().WithMessage(Messages.Validation_DaysOfWeek_Required);

        RuleForEach(x => x.DaysOfWeek)
            .Must(d => ValidDays.Contains(d))
            .WithMessage(string.Format(Messages.Validation_DaysOfWeek_AllowedValues, string.Join(", ", ValidDays)));

        RuleFor(x => x.Times).NotEmpty().WithMessage(Messages.Validation_Times_Required);

        RuleForEach(x => x.Times)
            .Matches(@"^([01]\d|2[0-3]):[0-5]\d$")
            .WithMessage(Messages.Validation_Times_TimeFormat);

        RuleFor(x => x.BasePrice).GreaterThanOrEqualTo(0).WithMessage(Messages.Validation_BasePrice_NonNegative);
    }
}

public class BulkDeleteShowtimesRequestValidator : AbstractValidator<BulkDeleteShowtimesRequest>
{
    public BulkDeleteShowtimesRequestValidator()
    {
        RuleFor(x => x.ShowtimeIds)
            .NotEmpty()
            .WithMessage(Messages.Validation_ShowtimeIds_NotEmpty);

        RuleForEach(x => x.ShowtimeIds).GreaterThan(0).WithMessage(Messages.Validation_ShowtimeId_Positive);
    }
}

public class AutoScheduleRequestValidator : AbstractValidator<AutoScheduleRequest>
{
    private const int MaxRangeDays = 31;

    public AutoScheduleRequestValidator()
    {
        RuleFor(x => x.Date).NotEmpty().WithMessage(Messages.Validation_Date_Required);

        RuleFor(x => x)
            .Cascade(CascadeMode.Stop)
            .Must(x => x.ToDate is null || x.ToDate.Value >= x.Date)
            .WithMessage(Messages.Invalid_ToDateBeforeDate)
            .Must(x => x.ToDate is null || x.ToDate.Value.DayNumber - x.Date.DayNumber < MaxRangeDays)
            .WithMessage(string.Format(Messages.Invalid_AutoScheduleRangeTooLong, MaxRangeDays));

        RuleFor(x => x.Movies)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(Messages.Validation_Movies_Required)
            .Must(m => m.Count <= 20)
            .WithMessage(Messages.Validation_AutoSchedule_MaxMovies)
            .Must(m => m.Select(x => x.MovieId).Distinct().Count() == m.Count)
            .WithMessage(Messages.Validation_Movies_Duplicated);

        RuleForEach(x => x.Movies)
            .ChildRules(m =>
            {
                m.RuleFor(x => x.MovieId).GreaterThan(0).WithMessage(Messages.Validation_MovieId_Positive);
                m.RuleFor(x => x.Shows)
                    .InclusiveBetween(1, 20)
                    .WithMessage(Messages.Validation_Shows_Range1To20);
                m.RuleFor(x => x.BasePrice)
                    .GreaterThanOrEqualTo(0)
                    .When(x => x.BasePrice.HasValue)
                    .WithMessage(Messages.Validation_MovieBasePrice_NonNegative);
            });

        RuleFor(x => x.BasePrice).GreaterThanOrEqualTo(0).WithMessage(Messages.Validation_BasePrice_NonNegative);

        RuleForEach(x => x.RoomIds).GreaterThan(0).WithMessage(Messages.Validation_RoomId_Positive);
    }
}

public class ApplyAutoScheduleRequestValidator : AbstractValidator<ApplyAutoScheduleRequest>
{
    public ApplyAutoScheduleRequestValidator()
    {
        RuleFor(x => x.Items)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(Messages.Validation_Items_Required)
            .Must(i => i.Count <= 1000)
            .WithMessage(Messages.Validation_Showtimes_MaxPerApply);

        RuleForEach(x => x.Items).SetValidator(new UpsertShowtimeRequestValidator());
    }
}

public class CloneShowtimesRequestValidator : AbstractValidator<CloneShowtimesRequest>
{
    private const int MaxSourceRangeDays = 31;

    public CloneShowtimesRequestValidator()
    {
        RuleFor(x => x.SourceFrom).NotEmpty().WithMessage(Messages.Validation_SourceFrom_Required);

        RuleFor(x => x.SourceTo)
            .NotEmpty()
            .WithMessage(Messages.Validation_SourceTo_Required)
            .GreaterThanOrEqualTo(x => x.SourceFrom)
            .WithMessage(Messages.Validation_SourceTo_BeforeSourceFrom);

        RuleFor(x => x)
            .Must(x => x.SourceTo.DayNumber - x.SourceFrom.DayNumber < MaxSourceRangeDays)
            .WithMessage(string.Format(Messages.Validation_SourceRange_TooLong, MaxSourceRangeDays));

        RuleFor(x => x.ShiftDays)
            .InclusiveBetween(1, 366)
            .WithMessage(Messages.Validation_ShiftDays_Range1To366);

        RuleForEach(x => x.RoomIds).GreaterThan(0).WithMessage(Messages.Validation_RoomId_Positive);
    }
}

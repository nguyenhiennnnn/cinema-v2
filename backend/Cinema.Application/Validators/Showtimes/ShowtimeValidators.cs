using Cinema.Application.DTOs.Showtimes;
using Cinema.Application.Interfaces;
using FluentValidation;

namespace Cinema.Application.Validators.Showtimes;

public class AdminShowtimeCalendarFilterValidator : AbstractValidator<AdminShowtimeCalendarFilter>
{
    private const int MaxRangeDays = 92;

    public AdminShowtimeCalendarFilterValidator()
    {
        RuleFor(x => x.From).NotEmpty().WithMessage("from không được để trống (yyyy-MM-dd).");

        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage("to không được để trống (yyyy-MM-dd).")
            .GreaterThanOrEqualTo(x => x.From)
            .WithMessage("to phải >= from.");

        RuleFor(x => x)
            .Must(x => x.To.DayNumber - x.From.DayNumber <= MaxRangeDays)
            .WithMessage($"Khoảng ngày (from → to) không được vượt quá {MaxRangeDays} ngày.");
    }
}

public class UpsertShowtimeRequestValidator : AbstractValidator<UpsertShowtimeRequest>
{
    public UpsertShowtimeRequestValidator()
    {
        RuleFor(x => x.MovieId).GreaterThan(0).WithMessage("movieId phải lớn hơn 0.");

        RuleFor(x => x.RoomId).GreaterThan(0).WithMessage("roomId phải lớn hơn 0.");

        RuleFor(x => x.StartTime).NotEmpty().WithMessage("startTime không được để trống.");

        RuleFor(x => x.BasePrice).GreaterThanOrEqualTo(0).WithMessage("basePrice phải >= 0.");
    }
}

public class BulkDateRangeValidator : AbstractValidator<BulkDateRange>
{
    public BulkDateRangeValidator()
    {
        RuleFor(x => x.From).NotEmpty().WithMessage("dateRange.from không được để trống.");

        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage("dateRange.to không được để trống.")
            .GreaterThanOrEqualTo(x => x.From)
            .WithMessage("dateRange.to phải >= dateRange.from.");
    }
}

public class BulkCreateShowtimesRequestValidator : AbstractValidator<BulkCreateShowtimesRequest>
{
    private static readonly string[] ValidDays = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

    public BulkCreateShowtimesRequestValidator()
    {
        RuleFor(x => x.MovieId).GreaterThan(0).WithMessage("movieId phải lớn hơn 0.");

        RuleFor(x => x.RoomId).GreaterThan(0).WithMessage("roomId phải lớn hơn 0.");

        RuleFor(x => x.DateRange)
            .NotNull()
            .WithMessage("dateRange không được null.")
            .SetValidator(new BulkDateRangeValidator());

        RuleFor(x => x.DaysOfWeek).NotEmpty().WithMessage("daysOfWeek không được để trống.");

        RuleForEach(x => x.DaysOfWeek)
            .Must(d => ValidDays.Contains(d))
            .WithMessage($"daysOfWeek chỉ chấp nhận: {string.Join(", ", ValidDays)}");

        RuleFor(x => x.Times).NotEmpty().WithMessage("times không được để trống.");

        RuleForEach(x => x.Times)
            .Matches(@"^([01]\d|2[0-3]):[0-5]\d$")
            .WithMessage("times phải theo định dạng HH:mm.");

        RuleFor(x => x.BasePrice).GreaterThanOrEqualTo(0).WithMessage("basePrice phải >= 0.");
    }
}

public class BulkDeleteShowtimesRequestValidator : AbstractValidator<BulkDeleteShowtimesRequest>
{
    public BulkDeleteShowtimesRequestValidator()
    {
        RuleFor(x => x.ShowtimeIds)
            .NotEmpty()
            .WithMessage("Danh sách showtimeIds không được rỗng.");

        RuleForEach(x => x.ShowtimeIds).GreaterThan(0).WithMessage("showtimeId phải lớn hơn 0.");
    }
}

public class AutoScheduleRequestValidator : AbstractValidator<AutoScheduleRequest>
{
    public AutoScheduleRequestValidator()
    {
        RuleFor(x => x.Date).NotEmpty().WithMessage("date không được để trống.");

        RuleFor(x => x.Movies)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("movies không được để trống.")
            .Must(m => m.Count <= 20)
            .WithMessage("Tối đa 20 phim mỗi lần xếp lịch.")
            .Must(m => m.Select(x => x.MovieId).Distinct().Count() == m.Count)
            .WithMessage("movies không được chứa phim trùng lặp.");

        RuleForEach(x => x.Movies)
            .ChildRules(m =>
            {
                m.RuleFor(x => x.MovieId).GreaterThan(0).WithMessage("movieId phải lớn hơn 0.");
                m.RuleFor(x => x.Shows)
                    .InclusiveBetween(1, 20)
                    .WithMessage("shows phải từ 1 đến 20.");
                m.RuleFor(x => x.BasePrice)
                    .GreaterThanOrEqualTo(0)
                    .When(x => x.BasePrice.HasValue)
                    .WithMessage("basePrice của phim phải >= 0.");
            });

        RuleFor(x => x.BasePrice).GreaterThanOrEqualTo(0).WithMessage("basePrice phải >= 0.");

        RuleForEach(x => x.RoomIds).GreaterThan(0).WithMessage("roomId phải lớn hơn 0.");
    }
}

public class ApplyAutoScheduleRequestValidator : AbstractValidator<ApplyAutoScheduleRequest>
{
    public ApplyAutoScheduleRequestValidator()
    {
        RuleFor(x => x.Items)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("items không được để trống.")
            .Must(i => i.Count <= 200)
            .WithMessage("Tối đa 200 suất chiếu mỗi lần áp dụng.");

        RuleForEach(x => x.Items).SetValidator(new UpsertShowtimeRequestValidator());
    }
}

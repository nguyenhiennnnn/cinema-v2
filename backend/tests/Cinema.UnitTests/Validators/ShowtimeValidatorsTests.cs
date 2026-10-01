using Cinema.Application.DTOs.Showtimes;
using Cinema.Application.Interfaces;
using Cinema.Application.Validators.Showtimes;
using FluentValidation.TestHelper;
using Shouldly;

namespace Cinema.UnitTests.Validators;

public class UpsertShowtimeRequestValidatorTests
{
    private readonly UpsertShowtimeRequestValidator _validator = new();

    [Fact]
    public void ValidRequest_PassesValidation()
    {
        var request = new UpsertShowtimeRequest(
            MovieId: 1,
            RoomId: 1,
            StartTime: DateTime.UtcNow.AddDays(1),
            BasePrice: 90_000m
        );

        _validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NonPositiveMovieId_FailsValidation()
    {
        var request = new UpsertShowtimeRequest(0, 1, DateTime.UtcNow, 90_000m);

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.MovieId);
    }

    [Fact]
    public void NonPositiveRoomId_FailsValidation()
    {
        var request = new UpsertShowtimeRequest(1, 0, DateTime.UtcNow, 90_000m);

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.RoomId);
    }

    [Fact]
    public void NegativeBasePrice_FailsValidation()
    {
        var request = new UpsertShowtimeRequest(1, 1, DateTime.UtcNow, -1m);

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.BasePrice);
    }

    [Fact]
    public void ZeroBasePrice_IsAllowed()
    {
        var request = new UpsertShowtimeRequest(1, 1, DateTime.UtcNow, 0m);

        _validator.TestValidate(request).ShouldNotHaveValidationErrorFor(x => x.BasePrice);
    }
}

public class AdminShowtimeCalendarFilterValidatorTests
{
    private readonly AdminShowtimeCalendarFilterValidator _validator = new();

    [Fact]
    public void ToBeforeFrom_FailsValidation()
    {
        var filter = new AdminShowtimeCalendarFilter(
            From: new DateOnly(2026, 3, 10),
            To: new DateOnly(2026, 3, 1),
            RoomId: null,
            MovieId: null,
            Status: null
        );

        _validator.TestValidate(filter).ShouldHaveValidationErrorFor(x => x.To);
    }

    [Fact]
    public void RangeWiderThanMax92Days_FailsValidation()
    {
        var filter = new AdminShowtimeCalendarFilter(
            From: new DateOnly(2026, 1, 1),
            To: new DateOnly(2026, 12, 31),
            RoomId: null,
            MovieId: null,
            Status: null
        );

        var result = _validator.TestValidate(filter);
        result.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public void ValidNarrowRange_PassesValidation()
    {
        var filter = new AdminShowtimeCalendarFilter(
            From: new DateOnly(2026, 3, 1),
            To: new DateOnly(2026, 3, 7),
            RoomId: 1,
            MovieId: null,
            Status: "Scheduled"
        );

        _validator.TestValidate(filter).ShouldNotHaveAnyValidationErrors();
    }
}

public class BulkCreateShowtimesRequestValidatorTests
{
    private readonly BulkCreateShowtimesRequestValidator _validator = new();

    private static BulkCreateShowtimesRequest Valid() =>
        new(
            MovieId: 1,
            RoomId: 1,
            DateRange: new BulkDateRange(new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 7)),
            DaysOfWeek: ["Mon", "Fri"],
            Times: ["09:00", "13:30"],
            BasePrice: 90_000m
        );

    [Fact]
    public void ValidRequest_PassesValidation()
    {
        _validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void InvalidDayOfWeekCode_FailsValidation()
    {
        var request = Valid() with { DaysOfWeek = ["Monday"] };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor("DaysOfWeek[0]");
    }

    [Fact]
    public void InvalidTimeFormat_FailsValidation()
    {
        var request = Valid() with { Times = ["9:00 AM"] };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor("Times[0]");
    }

    [Fact]
    public void EmptyTimes_FailsValidation()
    {
        var request = Valid() with { Times = [] };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Times);
    }

    [Fact]
    public void DateRangeToBeforeFrom_FailsValidation()
    {
        var request = Valid() with
        {
            DateRange = new BulkDateRange(new DateOnly(2026, 3, 10), new DateOnly(2026, 3, 1)),
        };

        _validator.TestValidate(request).ShouldHaveValidationErrorFor("DateRange.To");
    }
}

public class BulkDeleteShowtimesRequestValidatorTests
{
    private readonly BulkDeleteShowtimesRequestValidator _validator = new();

    [Fact]
    public void EmptyShowtimeIds_FailsValidation()
    {
        _validator
            .TestValidate(new BulkDeleteShowtimesRequest([]))
            .ShouldHaveValidationErrorFor(x => x.ShowtimeIds);
    }

    [Fact]
    public void NonPositiveShowtimeId_FailsValidation()
    {
        _validator
            .TestValidate(new BulkDeleteShowtimesRequest([1, 0]))
            .ShouldHaveValidationErrorFor("ShowtimeIds[1]");
    }
}

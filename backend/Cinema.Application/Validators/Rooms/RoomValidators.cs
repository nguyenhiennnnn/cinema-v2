using Cinema.Application.DTOs.Rooms;
using Cinema.Shared.Resources;
using FluentValidation;

namespace Cinema.Application.Validators.Rooms;

public class CreateRoomRequestValidator : AbstractValidator<CreateRoomRequest>
{
    public CreateRoomRequestValidator()
    {
        RuleFor(x => x.RoomName)
            .NotEmpty()
            .WithMessage(Messages.Validation_RoomName_Required)
            .MaximumLength(50)
            .WithMessage(Messages.Validation_RoomName_MaxLength50);

        RuleFor(x => x.ScreenTypeId)
            .GreaterThan(0)
            .When(x => x.ScreenTypeId is not null)
            .WithMessage(Messages.Validation_ScreenTypeId_Positive);
    }
}

public class UpdateRoomRequestValidator : AbstractValidator<UpdateRoomRequest>
{
    public UpdateRoomRequestValidator()
    {
        RuleFor(x => x.RoomName)
            .NotEmpty()
            .WithMessage(Messages.Validation_RoomName_Required)
            .MaximumLength(50)
            .WithMessage(Messages.Validation_RoomName_MaxLength50);

        RuleFor(x => x.ScreenTypeId)
            .GreaterThan(0)
            .When(x => x.ScreenTypeId is not null)
            .WithMessage(Messages.Validation_ScreenTypeId_Positive);
    }
}

public class SeatLayoutItemValidator : AbstractValidator<SeatLayoutItem>
{
    public SeatLayoutItemValidator()
    {
        RuleFor(s => s.SeatRow)
            .NotEmpty()
            .WithMessage(Messages.Validation_SeatRow_Required)
            .MaximumLength(5)
            .WithMessage(Messages.Validation_SeatRow_MaxLength5);

        RuleFor(s => s.SeatNumber)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_SeatNumber_Positive);

        RuleFor(s => s.SeatTypeId)
            .GreaterThan(0)
            .When(s => s.SeatTypeId is not null)
            .WithMessage(Messages.Validation_SeatTypeId_Positive);
    }
}

public class UpdateSeatLayoutRequestValidator : AbstractValidator<UpdateSeatLayoutRequest>
{
    public UpdateSeatLayoutRequestValidator()
    {
        RuleFor(x => x.Seats).NotEmpty().WithMessage(Messages.Validation_Seats_NotEmpty);

        RuleForEach(x => x.Seats).SetValidator(new SeatLayoutItemValidator());

        RuleFor(x => x.Seats)
            .Must(seats =>
                seats
                    .Select(s => (s.SeatRow.Trim().ToUpperInvariant(), s.SeatNumber))
                    .Distinct()
                    .Count() == seats.Count
            )
            .WithMessage(Messages.Validation_Seats_DuplicatedPosition)
            .WithName("seats");
    }
}

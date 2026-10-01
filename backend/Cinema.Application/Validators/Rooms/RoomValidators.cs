using Cinema.Application.DTOs.Rooms;
using FluentValidation;

namespace Cinema.Application.Validators.Rooms;

public class CreateRoomRequestValidator : AbstractValidator<CreateRoomRequest>
{
    public CreateRoomRequestValidator()
    {
        RuleFor(x => x.RoomName)
            .NotEmpty()
            .WithMessage("Tên phòng chiếu không được để trống.")
            .MaximumLength(50)
            .WithMessage("Tên phòng chiếu không được vượt quá 50 ký tự.");

        RuleFor(x => x.ScreenTypeId)
            .GreaterThan(0)
            .When(x => x.ScreenTypeId is not null)
            .WithMessage("screenTypeId phải lớn hơn 0.");
    }
}

public class UpdateRoomRequestValidator : AbstractValidator<UpdateRoomRequest>
{
    public UpdateRoomRequestValidator()
    {
        RuleFor(x => x.RoomName)
            .NotEmpty()
            .WithMessage("Tên phòng chiếu không được để trống.")
            .MaximumLength(50)
            .WithMessage("Tên phòng chiếu không được vượt quá 50 ký tự.");

        RuleFor(x => x.ScreenTypeId)
            .GreaterThan(0)
            .When(x => x.ScreenTypeId is not null)
            .WithMessage("screenTypeId phải lớn hơn 0.");
    }
}

public class SeatLayoutItemValidator : AbstractValidator<SeatLayoutItem>
{
    public SeatLayoutItemValidator()
    {
        RuleFor(s => s.SeatRow)
            .NotEmpty()
            .WithMessage("seatRow không được để trống.")
            .MaximumLength(5)
            .WithMessage("seatRow không được vượt quá 5 ký tự.");

        RuleFor(s => s.SeatNumber)
            .GreaterThan(0)
            .WithMessage("seatNumber phải lớn hơn 0.");

        RuleFor(s => s.SeatTypeId)
            .GreaterThan(0)
            .When(s => s.SeatTypeId is not null)
            .WithMessage("seatTypeId phải lớn hơn 0.");
    }
}

public class UpdateSeatLayoutRequestValidator : AbstractValidator<UpdateSeatLayoutRequest>
{
    public UpdateSeatLayoutRequestValidator()
    {
        RuleFor(x => x.Seats)
            .NotEmpty()
            .WithMessage("Danh sách seats không được rỗng.");

        RuleForEach(x => x.Seats)
            .SetValidator(new SeatLayoutItemValidator());

        RuleFor(x => x.Seats)
            .Must(seats =>
                seats.Select(s => (s.SeatRow.Trim().ToUpperInvariant(), s.SeatNumber)).Distinct().Count() == seats.Count
            )
            .WithMessage("Tồn tại ghế trùng vị trí (SeatRow + SeatNumber) trong danh sách.")
            .WithName("seats");
    }
}

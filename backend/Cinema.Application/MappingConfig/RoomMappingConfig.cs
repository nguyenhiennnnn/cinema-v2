using Cinema.Application.DTOs.Rooms;
using Cinema.Domain.Entities;
using Mapster;

namespace Cinema.Application.MappingConfig;

public class RoomMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<CinemaRoom, AdminRoomDto>()
            .Map(
                dest => dest.ScreenTypeName,
                src => src.ScreenType != null ? src.ScreenType.Name : null
            )
            .Map(dest => dest.ScreenTypeId, src => src.ScreenType!.ScreenTypeId);

        config
            .NewConfig<Seat, SeatDto>()
            .Map(dest => dest.SeatTypeName, src => src.SeatType != null ? src.SeatType.Name : null);
    }
}

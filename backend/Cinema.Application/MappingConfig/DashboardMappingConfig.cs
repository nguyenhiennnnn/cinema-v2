using Cinema.Application.DTOs.Dashboard;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Mapster;

namespace Cinema.Application.MappingConfig;

public class DashboardMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<Showtime, DashboardShowtimeItemDto>()
            .Map(dest => dest.MovieTitle, src => src.Movie.Title)
            .Map(dest => dest.RoomName, src => src.Room.RoomName)
            .Map(
                dest => dest.BookedSeats,
                src => src.ShowtimeSeats.Count(ss => ss.Status == ShowtimeSeatStatus.Booked)
            )
            .Map(dest => dest.TotalSeats, src => src.ShowtimeSeats.Count());
    }
}

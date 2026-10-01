namespace Cinema.Application.MappingConfig;

using Cinema.Application.DTOs.Movies;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Mapster;

public class MovieMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<Movie, MovieListItemDto>()
            .Map(dest => dest.Status, src => src.Status.ToDtoStatus())
            .Map(
                dest => dest.AgeRating,
                src => src.AgeRating == null ? null : new AgeRatingCodeDto(src.AgeRating.Code)
            )
            .Map(dest => dest.Genres, src => src.MovieGenres.Select(mg => mg.Genre.Name).ToList());

        config
            .NewConfig<Movie, MovieDetailDto>()
            .Map(dest => dest.Status, src => src.Status.ToDtoStatus())
            .Map(dest => dest.Genres, src => src.MovieGenres.Select(mg => mg.Genre).ToList())
            .Map(
                dest => dest.Cast,
                src =>
                    src.MovieCast.Select(mc => new CastMemberDto(
                            mc.ActorId,
                            mc.Actor.FullName,
                            mc.Actor.AvatarUrl,
                            mc.CharacterName
                        ))
                        .ToList()
            )
            .Map(dest => dest.UpcomingShowtimes, _ => new List<UpcomingShowtimeDto>());

        config
            .NewConfig<Movie, AdminMovieListItemDto>()
            .Map(dest => dest.Status, src => src.Status.ToDtoStatus())
            .Map(
                dest => dest.DirectorName,
                src => src.Director != null ? src.Director.FullName : null
            )
            .Map(dest => dest.LanguageName, src => src.Language != null ? src.Language.Name : null);

        config
            .NewConfig<Showtime, UpcomingShowtimeDto>()
            .Map(dest => dest.RoomName, src => src.Room.RoomName)
            .Map(dest => dest.ScreenType, src => src.Room.ScreenType != null ? src.Room.ScreenType.Name : null);
    }
}


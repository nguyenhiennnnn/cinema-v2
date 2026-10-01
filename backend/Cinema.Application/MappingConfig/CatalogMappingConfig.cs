using Cinema.Application.DTOs.Catalog;
using Cinema.Domain.Entities;
using Mapster;

namespace Cinema.Application.MappingConfig;

public class CatalogMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Genre, GenreDto>();
        config.NewConfig<Language, LanguageDto>();
        config.NewConfig<AgeRating, AgeRatingDto>();
        config.NewConfig<Director, DirectorDto>();
        config.NewConfig<Actor, ActorDto>();
        config.NewConfig<Position, PositionDto>();
        config.NewConfig<ScreenType, ScreenTypeDto>();
        config.NewConfig<SeatType, SeatTypeDto>();
        config.NewConfig<MembershipTier, MembershipTierDto>();
    }
}

using Cinema.Application.DTOs.Reviews;
using Cinema.Domain.Entities;
using Mapster;

namespace Cinema.Application.MappingConfig;

public class ReviewMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Review, ReviewDto>()
            .Map(dest => dest.UserName, src => src.User.FullName)
            .Map(dest => dest.UserAvatar, src => src.User.Avatar);

        config.NewConfig<Review, UserMyReviewDto>()
            .Map(dest => dest.MovieTitle, src => src.Movie.Title)
            .Map(dest => dest.MoviePosterUrl, src => src.Movie.PosterUrl);

        config.NewConfig<Review, AdminReviewDto>()
            .Map(dest => dest.MovieTitle, src => src.Movie.Title)
            .Map(dest => dest.UserName, src => src.User.FullName);
    }
}

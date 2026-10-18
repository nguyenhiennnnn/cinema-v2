using Cinema.Application.DTOs.Auth;
using Cinema.Domain.Entities;
using Mapster;

namespace Cinema.Application.MappingConfig;

public class AuthMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, MeResponse>().Map(dest => dest.Role, src => src.Role.RoleName);

        config.NewConfig<User, LoginUserDto>()
            .Map(dest => dest.Role, src => src.Role.RoleName)
            .Map(dest => dest.MembershipTierName, src => src.CustomerProfile != null && src.CustomerProfile.Tier != null ? src.CustomerProfile.Tier.TierName : null)
            .Map(dest => dest.Points, src => src.CustomerProfile != null ? (int?)src.CustomerProfile.Points : null)
            .Map(dest => dest.IsEmailConfirmed, src => src.EmailConfirmed);
    }
}

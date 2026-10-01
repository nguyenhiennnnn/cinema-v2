using Cinema.Application.DTOs.Employees;
using Cinema.Application.DTOs.Users;
using Cinema.Domain.Entities;
using Mapster;

namespace Cinema.Application.MappingConfig;

public class UserEmployeeMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<User, AdminUserListItemDto>()
            .Map(dest => dest.RoleName, src => src.Role.RoleName)
            .Map(dest => dest.RoleId, src => src.Role.RoleId);

        config
            .NewConfig<Employee, AdminEmployeeDto>()
            .Map(dest => dest.FullName, src => src.User.FullName)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.PositionName, src => src.Position != null ? src.Position.Name : null);

        config.NewConfig<MembershipTier, MembershipTierBriefDto>();
    }
}

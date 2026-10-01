using Cinema.Application.DTOs.Combos;
using Cinema.Domain.Entities;
using Mapster;

namespace Cinema.Application.MappingConfig;

public class ComboMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Combo, ComboDto>();
        config.NewConfig<Combo, AdminComboDto>();
    }
}

using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Cinema.Application.MappingConfig;

public static class MapsterConfig
{
    public static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(MapsterConfig).Assembly);

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}

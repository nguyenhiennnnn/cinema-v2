using Microsoft.OpenApi;

namespace Cinema.Api.Extensions;

public static class SwaggerExtensions
{
    private const string ClientDoc = "client";
    private const string AdminDoc = "admin";
    private const string AdminRoutePrefix = "api/v1/admin";

    public static IServiceCollection AddSwaggerDocs(
        this IServiceCollection services,
        string brandName
    )
    {
        services.AddEndpointsApiExplorer();
        return services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                ClientDoc,
                new OpenApiInfo { Title = $"{brandName} — Client API", Version = "v1" }
            );
            options.SwaggerDoc(
                AdminDoc,
                new OpenApiInfo { Title = $"{brandName} — Admin API", Version = "v1" }
            );

            options.DocInclusionPredicate(
                (docName, apiDesc) =>
                {
                    var isAdminRoute = (apiDesc.RelativePath ?? string.Empty).StartsWith(
                        AdminRoutePrefix,
                        StringComparison.OrdinalIgnoreCase
                    );
                    return docName switch
                    {
                        AdminDoc => isAdminRoute,
                        ClientDoc => !isAdminRoute,
                        _ => false,
                    };
                }
            );

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header. Nhập: Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                }
            );
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                { new OpenApiSecuritySchemeReference("Bearer"), new List<string>() },
            });
        });
    }

    public static WebApplication UseSwaggerDocs(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return app;

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/client/swagger.json", "Client API");
            options.SwaggerEndpoint("/swagger/admin/swagger.json", "Admin API");
        });
        return app;
    }
}

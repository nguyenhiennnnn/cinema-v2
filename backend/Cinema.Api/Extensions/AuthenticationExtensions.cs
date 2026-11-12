using System.Text;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Cinema.Api.Extensions;

public static class AuthenticationExtensions
{
    private const string DevOnlyPlaceholderSecret = "DEV-ONLY-PLACEHOLDER-SECRET-CHANGE-ME-32CHARS";

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        JwtOptions jwtOptions
    )
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            string.IsNullOrEmpty(jwtOptions.Secret)
                                ? DevOnlyPlaceholderSecret
                                : jwtOptions.Secret
                        )
                    ),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(jwtOptions.ClockSkewSeconds),
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = ValidateActiveUserAsync,
                    OnMessageReceived = ReadSignalRTokenFromQuery,
                };
            });

        services
            .AddAuthorizationBuilder()
            .AddPolicy(Policies.CustomerOnly, p => p.RequireRole(Roles.Customer))
            .AddPolicy(Policies.AdminOnly, p => p.RequireRole(Roles.Admin))
            .AddPolicy(Policies.EmployeeOnly, p => p.RequireRole(Roles.Employee))
            .AddPolicy(Policies.StaffOnly, p => p.RequireRole(Roles.Admin, Roles.Employee));

        return services;
    }

    private static async Task ValidateActiveUserAsync(TokenValidatedContext context)
    {
        var rawUserId = context.Principal?.FindFirst(AppClaimTypes.UserId)?.Value;
        if (!int.TryParse(rawUserId, out var userId))
        {
            context.Fail(Messages.Unauthorized_InvalidToken);
            return;
        }

        var db = context.HttpContext.RequestServices.GetRequiredService<IAppDbContext>();
        var isActive = await db.Users.AnyAsync(
            u => u.UserId == userId && u.IsActive,
            context.HttpContext.RequestAborted
        );
        if (!isActive)
            context.Fail(Messages.Forbidden_AccountLocked);
    }

    private static Task ReadSignalRTokenFromQuery(MessageReceivedContext context)
    {
        var accessToken = context.Request.Query["access_token"];
        if (
            !string.IsNullOrEmpty(accessToken)
            && context.HttpContext.Request.Path.StartsWithSegments("/hubs")
        )
            context.Token = accessToken;
        return Task.CompletedTask;
    }
}

using Cinema.Application.Interfaces;
using Cinema.Application.Services.Ai;
using Cinema.Application.Services.Auth;
using Cinema.Application.Services.Bookings;
using Cinema.Application.Services.Catalog;
using Cinema.Application.Services.Combos;
using Cinema.Application.Services.Dashboard;
using Cinema.Application.Services.Employees;
using Cinema.Application.Services.Movies;
using Cinema.Application.Services.Payments;
using Cinema.Application.Services.Reports;
using Cinema.Application.Services.Reviews;
using Cinema.Application.Services.Rooms;
using Cinema.Application.Services.Settings;
using Cinema.Application.Services.Showtimes;
using Cinema.Application.Services.Users;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Cinema.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IMovieService, MovieService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<ISeatExpirationService, SeatExpirationService>();
        services.AddScoped<IShowtimeService, ShowtimeService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<LoyaltyExpiryService>();
        services.AddScoped<IComboService, ComboService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IPaymentAdminService, PaymentAdminService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<AiToolExecutor>();
        services.AddHttpClient<IAiAssistantService, GeminiAiAssistantService>();
        return services;
    }
}

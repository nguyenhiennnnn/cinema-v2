using System.Text;
using System.Threading.RateLimiting;
using Cinema.Api.Filters;
using Cinema.Api.Middlewares;
using Cinema.Application;
using Cinema.Application.BackgroundJobs;
using Cinema.Application.MappingConfig;
using Cinema.Application.Options;
using Cinema.Infrastructure;
using Cinema.Infrastructure.Persistence;
using Cinema.Infrastructure.Realtime;
using Cinema.Shared.Constants;
using Cinema.Shared.Results;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;


// Npgsql: cho phep DateTime voi Kind=UTC ghi vao cot "timestamp without time zone".
// Can thiet khi chuyen tu SQL Server vi tat ca cot datetime dung "timestamp without time zone".
// Xem: https://www.npgsql.org/efcore/release-notes/6.0.html#opting-out-of-the-new-timestamp-mapping-logic
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Serilog — structured logging
// Doc config tu appsettings.json (Serilog section),
// ghi log ra Console va file logs/cinema-{date}.log
builder.Host.UseSerilog(
    (context, services, loggerConfig) =>
        loggerConfig
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/cinema-.log", rollingInterval: RollingInterval.Day)
);

// Options pattern — anh xa cac section trong appsettings.json
// vao cac POCO class de inject qua IOptions<T> / IOptionsSnapshot<T>
builder.Services.Configure<BookingOptions>(
    builder.Configuration.GetSection(BookingOptions.SectionName)
);
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<RateLimitOptions>(
    builder.Configuration.GetSection(RateLimitOptions.SectionName)
);
builder.Services.Configure<CloudinaryOptions>(
    builder.Configuration.GetSection(CloudinaryOptions.SectionName)
);
builder.Services.Configure<EmailOptions>(
    builder.Configuration.GetSection(EmailOptions.SectionName)
);
builder.Services.Configure<VnPayOptions>(
    builder.Configuration.GetSection(VnPayOptions.SectionName)
);
builder.Services.Configure<MomoOptions>(builder.Configuration.GetSection(MomoOptions.SectionName));
builder.Services.Configure<StripeOptions>(
    builder.Configuration.GetSection(StripeOptions.SectionName)
);
builder.Services.Configure<AppUrlOptions>(
    builder.Configuration.GetSection(AppUrlOptions.SectionName)
);
builder.Services.Configure<GeminiOptions>(
    builder.Configuration.GetSection(GeminiOptions.SectionName)
);
builder.Services.Configure<GoogleOptions>(
    builder.Configuration.GetSection(GoogleOptions.SectionName)
);

// Doc JwtOptions de cau hinh TokenValidationParameters o duoi
var jwtOptions =
    builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

// MVC Controllers
// ApiResponseFilter  — boc moi response thanh ApiResponse<T> chuan
// ValidationActionFilter — xu ly FluentValidation truoc khi vao action
builder
    .Services.AddControllers(options =>
    {
        options.Filters.Add<ApiResponseFilter>();
        options.Filters.Add<ValidationActionFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new Cinema.Application.Utils.DateOnlyJsonConverter()
        );
        options.JsonSerializerOptions.Converters.Add(
            new Cinema.Application.Utils.DateTimeJsonConverter()
        );
    });

// FluentValidation — quet toan bo assembly chua MapsterConfig de dang ky tat ca IValidator<T>
builder.Services.AddValidatorsFromAssembly(typeof(MapsterConfig).Assembly);

// Tat model-state auto-response mac dinh cua ASP.NET, thay bang ApiResponse chuan
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context
            .ModelState.Where(kv => kv.Value?.Errors.Count > 0)
            .ToDictionary(
                kv => char.ToLowerInvariant(kv.Key[0]) + kv.Key[1..],
                kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        return new BadRequestObjectResult(
            ApiResponse.ErrorResponse("Dữ liệu không hợp lệ.", errors)
        );
    };
});

// Mapster — object mapping (thay the AutoMapper)
// AddMapsterConfig() dang ky cac TypeAdapterConfig tu Cinema.Application
builder.Services.AddMapsterConfig();

// Infrastructure layer — EF Core (AppDbContext), repositories,
// Cloudinary, Email (MailKit), VnPay, MoMo, va cac service khac
// Xem: Cinema.Infrastructure/DependencyInjection.cs
builder.Services.AddInfrastructure(builder.Configuration);

// Application layer — MediatR (CQRS handlers), domain services,
// business logic use cases
// Xem: Cinema.Application/DependencyInjection.cs
builder.Services.AddApplication();

// JWT Bearer Authentication
// Xac thuc token tu header "Authorization: Bearer <token>"
// SignalR: doc token tu query string ?access_token= cho /hubs/*
builder
    .Services.AddAuthentication(options =>
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
                        ? "DEV-ONLY-PLACEHOLDER-SECRET-CHANGE-ME-32CHARS"
                        : jwtOptions.Secret
                )
            ),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
        };

        // Ho tro SignalR: doc JWT tu query string thay vi header
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    context.Token = accessToken;
                return Task.CompletedTask;
            },
        };
    });

// Authorization policies
// CustomerOnly — chi cho phep role "Customer"
// AdminOnly    — chi cho phep role "Admin"
builder
    .Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.CustomerOnly, p => p.RequireRole(Roles.Customer))
    .AddPolicy(Policies.AdminOnly, p => p.RequireRole(Roles.Admin))
    .AddPolicy(Policies.EmployeeOnly, p => p.RequireRole(Roles.Employee))
    .AddPolicy(Policies.StaffOnly, p => p.RequireRole(Roles.Admin, Roles.Employee));

// Swagger / OpenAPI (Swashbuckle)
// Chia 2 doc: "client" (public API) va "admin" (quan tri)
// Chi hien thi khi ASPNETCORE_ENVIRONMENT=Development
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "client",
        new OpenApiInfo { Title = "Cinema System — Client API", Version = "v1" }
    );
    options.SwaggerDoc(
        "admin",
        new OpenApiInfo { Title = "Cinema System — Admin API", Version = "v1" }
    );

    // Phan loai endpoint: admin doc bat dau bang api/v1/admin
    options.DocInclusionPredicate(
        (docName, apiDesc) =>
        {
            var route = apiDesc.RelativePath ?? string.Empty;
            return docName switch
            {
                "admin" => route.StartsWith("api/v1/admin", StringComparison.OrdinalIgnoreCase),
                "client" => !route.StartsWith("api/v1/admin", StringComparison.OrdinalIgnoreCase),
                _ => false,
            };
        }
    );

    // Them nut "Authorize" vao Swagger UI de test voi JWT
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

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");

// Hangfire — background job processing
// Storage: SQL Server (cung DB voi ung dung)
// Server: xu ly job trong cung process (single-instance dev)
// GlobalJobFilters: tu dong retry 3 lan (30s, 2p, 10p)
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddHangfire(config =>
        config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(
                options => options.UseNpgsqlConnection(connectionString)
            )
    );
    builder.Services.AddHangfireServer();
    GlobalJobFilters.Filters.Add(
        new AutomaticRetryAttribute { Attempts = 3, DelaysInSeconds = [30, 120, 600] }
    );
}

// SignalR — realtime WebSocket / long-polling
// Dung cho ShowtimeHub: cap nhat trang thai ghe dat theo thoi gian thuc
builder.Services.AddSignalR();

// CORS
// Lay danh sach origin tu Cors:AllowedOrigins trong appsettings / env
// Neu rong (dev) => cho phep tat ca origin + credentials
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "Default",
        policy =>
        {
            if (corsOrigins.Length > 0)
                policy
                    .WithOrigins(corsOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            else
                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed(_ => true)
                    .AllowCredentials();
        }
    );
});

// Rate Limiting (ASP.NET Core built-in, .NET 7+)
// Global  : SlidingWindow — 100 req / 60s moi IP
// "auth"  : FixedWindow   — 5 req / 60s moi IP (login, register)
// "booking-hold": FixedWindow — 10 req / 60s moi user (giu ghe)
var rateLimitOptions =
    builder.Configuration.GetSection(RateLimitOptions.SectionName).Get<RateLimitOptions>()
    ?? new RateLimitOptions();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = rateLimitOptions.GlobalPermitLimit,
                Window = TimeSpan.FromSeconds(rateLimitOptions.GlobalWindowSeconds),
                SegmentsPerWindow = 4,
                QueueLimit = 0,
            }
        )
    );

    options.AddPolicy(
        "auth",
        httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = rateLimitOptions.AuthPermitLimit,
                    Window = TimeSpan.FromSeconds(rateLimitOptions.AuthWindowSeconds),
                    QueueLimit = 0,
                }
            )
    );

    options.AddPolicy(
        "booking-hold",
        httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.User.Identity?.Name
                    ?? httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = rateLimitOptions.BookingHoldPermitLimit,
                    Window = TimeSpan.FromSeconds(rateLimitOptions.BookingHoldWindowSeconds),
                    QueueLimit = 0,
                }
            )
    );
});

// Build app
var app = builder.Build();

// ---- EF Core Migration & Seeding tu dong khi khoi dong ----
var runMigrationsOnStartup = builder.Configuration.GetValue("RunMigrationsOnStartup", true);
if (runMigrationsOnStartup)
{
    using var initScope = app.Services.CreateScope();
    var dbContext = initScope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await dbContext.Database.MigrateAsync();
        await Cinema.Infrastructure.Persistence.Seed.DbInitializer.SeedAsync(dbContext, app.Logger);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(
            ex,
            "Lỗi trong quá trình Database Migration hoặc Seeding khi khởi động."
        );
        throw;
    }
}

// Middleware pipeline (thu tu quan trong!)

// Enable request body buffering truoc Serilog de stream co the doc nhieu lan.
// Can thiet cho Stripe webhook signature verification (EventUtility.ConstructEvent).
app.Use(
    async (context, next) =>
    {
        context.Request.EnableBuffering();
        await next();
    }
);

// Serilog request logging — ghi method, path, status, time
app.UseSerilogRequestLogging();

// Global exception handler — tra ve ApiResponse<T> khi co unhandled exception
app.UseAppExceptionHandling();

// Swagger UI — chi hien thi trong Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/client/swagger.json", "Client API");
        options.SwaggerEndpoint("/swagger/admin/swagger.json", "Admin API");
    });
}

app.UseHttpsRedirection();
app.UseCors("Default"); // CORS truoc Authentication
if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseRateLimiter(); // Rate Limiting
}
app.UseAuthentication(); // Xac thuc JWT
app.UseAuthorization(); // Kiem tra policy / role

// Endpoint mapping

// Controllers — tat ca REST API endpoints
app.MapControllers();

// Hangfire Dashboard — /admin/hangfire (chi admin moi vao duoc)
if (!app.Environment.IsEnvironment("Testing"))
{
    app.MapHangfireDashboard(
        "/admin/hangfire",
        new DashboardOptions { Authorization = [new HangfireAdminAuthorizationFilter()] }
    );
}

// SignalR Hub — /hubs/showtimes
app.MapHub<ShowtimeHub>("/hubs/showtimes");

// Recurring Jobs (Hangfire)
// ReleaseExpiredSeats — giai phong ghe het thoi gian giu, chay moi phut
if (!app.Environment.IsEnvironment("Testing"))
{
    RecurringJob.AddOrUpdate<ReleaseExpiredSeatsJob>(
        RecurringJobIds.ReleaseExpiredSeats,
        j => j.RunAsync(CancellationToken.None),
        Cron.Minutely
    );
}

app.Run();

public partial class Program { }

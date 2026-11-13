using Cinema.Api.Extensions;
using Cinema.Api.Security;
using Cinema.Application;
using Cinema.Application.Options;
using Cinema.Infrastructure;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.UsePortFromEnvironment();
builder.AddApiLogging();

var jwtOptions = configuration.BindSection<JwtOptions>(JwtOptions.SectionName);
var brandOptions = configuration.BindSection<BrandOptions>(BrandOptions.SectionName);

Cinema.Application.Utils.BusinessTime.Configure(brandOptions.TimeZoneId);

Cinema.Application.Utils.CodeGenerator.Configure(brandOptions.BookingCodePrefix);

Cinema.Application.Utils.PhoneRules.Configure(brandOptions.PhoneRegex);

builder
    .Services.AddApiOptions(configuration)
    .AddApiControllers()
    .AddInfrastructure(configuration)
    .AddApplication()
    .AddJwtAuthentication(jwtOptions)
    .AddSwaggerDocs(brandOptions.Name)
    .AddBackgroundJobs(configuration, builder.Environment)
    .AddSignalR()
    .AddJsonProtocol(options => options.PayloadSerializerOptions.AddCinemaConverters());

builder.Services.AddCorsPolicy(configuration);
builder.Services.AddRefreshCookie(configuration);
builder.Services.AddRateLimiting(configuration);

builder.ValidateProductionOptions();

var app = builder.Build();

await app.MigrateAndSeedAsync();

app.UseApiPipeline();
app.MapApiEndpoints();
app.UseRecurringJobs();

app.Run();

public partial class Program { }

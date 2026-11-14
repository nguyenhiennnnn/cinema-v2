using System.Text.Json;
using Cinema.Application.DTOs.Settings;
using Cinema.Application.Services.Settings;
using Cinema.Application.Validators.Settings;
using Cinema.UnitTests.TestSupport;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class PublicSettingsDoesNotLeakDefaultPasswordTests(PostgresContainerFixture fixture)
    : IDisposable,
        IClassFixture<PostgresContainerFixture>
{
    private readonly PostgresAppDbContextHandle _handle = new(fixture.MasterConnectionString);

    private SettingsService CreateSut()
    {
        _handle.Db.ChangeTracker.Clear();
        Cinema.Application.Utils.BusinessTime.Configure("Asia/Ho_Chi_Minh");
        return new SettingsService(
            _handle.Db,
            Microsoft.Extensions.Options.Options.Create(
                new Cinema.Application.Options.BrandOptions { TimeZoneId = "Asia/Ho_Chi_Minh" }
            )
        );
    }

    private static UpdateSettingsRequest Request(string? defaultEmployeePassword) =>
        new(
            "Perfo Cinema",
            "1 Nguyen Hue",
            null,
            null,
            null,
            null,
            "09:00",
            "23:30",
            10_000m,
            100m,
            2,
            DefaultEmployeePassword: defaultEmployeePassword
        );

    [Fact]
    public async Task PublicGet_NeverSerializesDefaultEmployeePassword()
    {
        await CreateSut().UpdateSettingsAsync(Request("Secret123x"), CancellationToken.None);

        var publicDto = await CreateSut().GetSettingsAsync(CancellationToken.None);

        publicDto.GetType().ShouldBe(typeof(CinemaSettingsDto));
        var json = JsonSerializer.Serialize<object>(publicDto);
        json.ShouldNotContain("DefaultEmployeePassword", Case.Insensitive);
        json.ShouldNotContain("Secret123x");
    }

    [Fact]
    public async Task AdminGet_ReturnsDefaultEmployeePassword()
    {
        await CreateSut().UpdateSettingsAsync(Request("Secret123x"), CancellationToken.None);

        var adminDto = await CreateSut().GetAdminSettingsAsync(CancellationToken.None);

        adminDto.DefaultEmployeePassword.ShouldBe("Secret123x");
    }

    [Fact]
    public async Task Update_OmittedPassword_KeepsExistingValue()
    {
        await CreateSut().UpdateSettingsAsync(Request("Secret123x"), CancellationToken.None);

        var result = await CreateSut().UpdateSettingsAsync(Request(null), CancellationToken.None);

        result.DefaultEmployeePassword.ShouldBe("Secret123x");
    }

    [Theory]
    [InlineData("short1A", false)]
    [InlineData("alllowercase1", false)]
    [InlineData("ALLUPPERCASE1", false)]
    [InlineData("NoDigitsHere", false)]
    [InlineData("Valid1234", true)]
    public void Validator_AppliesStrongPasswordRules(string password, bool valid)
    {
        var result = new UpdateSettingsRequestValidator().Validate(Request(password));

        result.Errors.Any(e => e.PropertyName == "DefaultEmployeePassword").ShouldBe(!valid);
    }

    [Fact]
    public void Validator_NullPassword_IsAccepted()
    {
        var result = new UpdateSettingsRequestValidator().Validate(Request(null));

        result.Errors.ShouldNotContain(e => e.PropertyName == "DefaultEmployeePassword");
    }

    public void Dispose() => _handle.Dispose();
}

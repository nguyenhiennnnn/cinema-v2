using Cinema.Application.Options;
using Shouldly;

namespace Cinema.UnitTests.Validators;

public class ProductionOptionsValidatorTests
{
    private static readonly string[] Origins = ["https://perfo.example.com"];

    private static IReadOnlyList<string> Validate(
        AppUrlOptions? app = null,
        JwtOptions? jwt = null,
        string[]? origins = null,
        VnPayOptions? vnPay = null,
        MomoOptions? momo = null,
        StripeOptions? stripe = null
    ) =>
        ProductionOptionsValidator.Validate(
            app
                ?? new AppUrlOptions
                {
                    ApiBaseUrl = "https://api.example.com",
                    FrontendBaseUrl = "https://perfo.example.com",
                },
            jwt ?? new JwtOptions { Secret = new string('s', 40) },
            origins ?? Origins,
            vnPay ?? new VnPayOptions(),
            momo ?? new MomoOptions(),
            stripe ?? new StripeOptions(),
            new BrandOptions
            {
                TimeZoneId = "UTC",
                Locale = "vi-VN",
                CurrencyCode = "VND",
                CurrencySymbol = "VNĐ",
            }
        );

    [Fact]
    public void Validate_CompleteConfiguration_ReturnsNoErrors()
    {
        Validate().ShouldBeEmpty();
    }

    [Fact]
    public void Validate_DefaultOptions_ReportsLocalhostAndMissingSecrets()
    {
        var errors = ProductionOptionsValidator.Validate(
            new AppUrlOptions(),
            new JwtOptions(),
            [],
            new VnPayOptions(),
            new MomoOptions(),
            new StripeOptions(),
            new BrandOptions
            {
                TimeZoneId = "UTC",
                Locale = "vi-VN",
                CurrencyCode = "VND",
                CurrencySymbol = "VNĐ",
            }
        );

        errors.Count.ShouldBe(4);
        errors.ShouldContain(e => e.StartsWith("Jwt:Secret"));
        errors.ShouldContain(e => e.StartsWith("App:ApiBaseUrl"));
        errors.ShouldContain(e => e.StartsWith("App:FrontendBaseUrl"));
        errors.ShouldContain(e => e.StartsWith("Cors:AllowedOrigins"));
    }

    [Fact]
    public void Validate_VnPayEnabledWithoutPublicUrls_ReportsUrls()
    {
        var errors = Validate(
            vnPay: new VnPayOptions
            {
                TmnCode = "code",
                HashSecret = "secret",
                ReturnUrl = "http://localhost:5173/booking/result",
            }
        );

        errors.ShouldContain(e => e.StartsWith("VnPay:ReturnUrl"));
        errors.ShouldContain(e => e.StartsWith("VnPay:IpnUrl"));
        errors.Count.ShouldBe(2);
    }

    [Fact]
    public void Validate_StripeDefaultReturnUrlWithSecretKey_ReportsReturnUrl()
    {
        var errors = Validate(stripe: new StripeOptions { SecretKey = "sk_test" });

        errors.Count.ShouldBe(1);
        errors[0].ShouldStartWith("Stripe:ReturnUrl");
    }

    [Fact]
    public void Validate_DisabledGateways_AreNotChecked()
    {
        Validate(stripe: new StripeOptions(), momo: new MomoOptions()).ShouldBeEmpty();
    }
}

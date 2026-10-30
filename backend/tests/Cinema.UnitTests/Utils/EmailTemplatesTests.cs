using System.Globalization;
using System.Text.RegularExpressions;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Shouldly;

namespace Cinema.UnitTests.Utils;

public class EmailTemplatesTests
{
    private static readonly BrandOptions Brand = new()
    {
        DisplayName = "ACME",
        Tagline = "Slogan thử nghiệm",
        SupportEmail = "help@acme.test",
    };

    private static IEnumerable<(string Name, string Html)> AllTemplates(
        BrandOptions brand,
        string customerName = "An"
    )
    {
        yield return (
            nameof(EmailTemplates.ActionLink),
            EmailTemplates.ActionLink(
                brand,
                customerName,
                "Xác nhận email",
                "Nhấn nút bên dưới để xác nhận địa chỉ email.",
                "Xác nhận email",
                "https://perfo.test/confirm?token=abc",
                "24 giờ"
            )
        );
        yield return (
            nameof(EmailTemplates.BookingConfirmed),
            EmailTemplates.BookingConfirmed(brand, customerName, "BK001", "Phim", 2)
        );
        yield return (
            nameof(EmailTemplates.BookingCancelledRefunded),
            EmailTemplates.BookingCancelledRefunded(brand, customerName, "BK001", 120_000m)
        );
        yield return (
            nameof(EmailTemplates.ShowtimeCancelledAutoRefunded),
            EmailTemplates.ShowtimeCancelledAutoRefunded(brand, customerName, "BK001", 120_000m)
        );
        yield return (
            nameof(EmailTemplates.ShowtimeCancelledManualRefund),
            EmailTemplates.ShowtimeCancelledManualRefund(brand, customerName, "BK001", 120_000m)
        );
        yield return (
            $"{nameof(EmailTemplates.SeatsExpiredAfterPaymentCustomer)}:auto",
            EmailTemplates.SeatsExpiredAfterPaymentCustomer(brand, customerName, "BK001", true, 120_000m)
        );
        yield return (
            $"{nameof(EmailTemplates.SeatsExpiredAfterPaymentCustomer)}:manual",
            EmailTemplates.SeatsExpiredAfterPaymentCustomer(brand, customerName, "BK001", false, 120_000m)
        );
        yield return (
            $"{nameof(EmailTemplates.SeatsExpiredAfterPayment)}:auto",
            EmailTemplates.SeatsExpiredAfterPayment(brand, true, 1, 2, 120_000m)
        );
        yield return (
            $"{nameof(EmailTemplates.SeatsExpiredAfterPayment)}:manual",
            EmailTemplates.SeatsExpiredAfterPayment(brand, false, 1, 2, 120_000m)
        );
    }

    [Fact]
    public void BookingConfirmed_UsesBrandOptions()
    {
        var html = EmailTemplates.BookingConfirmed(Brand, "An", "BK001", "Phim", 2);

        html.ShouldContain("ACME");
        html.ShouldContain("Slogan thử nghiệm");
        html.ShouldContain("help@acme.test");
        html.ShouldNotContain("PERFO");
    }

    [Fact]
    public void BookingConfirmed_DefaultBrand_OmitsSupportLine()
    {
        var html = EmailTemplates.BookingConfirmed(new BrandOptions(), "An", "BK001", "Phim", 2);

        html.ShouldContain("PERFO");
        html.ShouldNotContain("Cần hỗ trợ?");
    }

    [Fact]
    public void AllTemplates_FollowDesignSystemRules()
    {
        // Quy tắc trong .agents/skills/perfo-design-system: không viết hoa cả chuỗi, không monospace,
        // không vạch viền trái, không bóng và gradient, không dấu gạch dài và dấu chấm giữa.
        string[] forbidden =
        [
            "text-transform",
            "monospace",
            "border-left",
            "box-shadow",
            "gradient",
            "oklch(",
            "—",
            " · ",
            "→",
        ];

        foreach (var (name, html) in AllTemplates(Brand))
        {
            foreach (var token in forbidden)
                html.ShouldNotContain(token, name);

            // Không có cỡ chữ dưới 13px (font-size:0 dùng cho ô đệm không tính).
            var sizes = Regex
                .Matches(html, @"font-size:\s*(\d+)px")
                .Select(m => int.Parse(m.Groups[1].Value))
                .ToList();
            sizes.Min().ShouldBeGreaterThanOrEqualTo(13, name);
        }
    }

    [Fact]
    public void OnlyBookingConfirmed_HasPerforation()
    {
        foreach (var (name, html) in AllTemplates(Brand))
        {
            if (name == nameof(EmailTemplates.BookingConfirmed))
                html.ShouldContain("dotted", name);
            else
                html.ShouldNotContain("dotted", name);
        }
    }

    [Fact]
    public void BookingConfirmed_ShowsBookingCodeOnTicketStub()
    {
        var html = EmailTemplates.BookingConfirmed(Brand, "An", "BK001", "Phim", 2);

        html.ShouldContain("Mã đặt vé");
        html.ShouldContain("BK001");
    }

    [Fact]
    public void Money_UsesDotThousandsSeparator_RegardlessOfCulture()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            var html = EmailTemplates.BookingCancelledRefunded(Brand, "An", "BK001", 1_250_000m);

            html.ShouldContain("1.250.000");
            html.ShouldNotContain("1,250,000");
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void AllTemplates_EncodeCustomerName()
    {
        foreach (var (name, html) in AllTemplates(Brand, "<script>alert(1)</script>"))
            html.ShouldNotContain("<script>", name);
    }
}

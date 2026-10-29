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
}

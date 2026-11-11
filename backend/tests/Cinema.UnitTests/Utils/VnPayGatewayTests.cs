using System.Net;
using System.Text.Json;
using System.Web;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Infrastructure.Payments;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace Cinema.UnitTests.Utils;

[CollectionDefinition(Name, DisableParallelization = true)]
public class BusinessTimeGlobalStateCollection
{
    public const string Name = "BusinessTime global state";
}

[Collection(BusinessTimeGlobalStateCollection.Name)]
public class VnPayGatewayTests
{
    private static readonly DateTime FixedUtcNow = new(2026, 3, 15, 10, 0, 0, DateTimeKind.Utc);

    private sealed class FixedTimeProvider(DateTime utc) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(utc, TimeSpan.Zero);
    }

    private sealed class CapturingHandler : HttpMessageHandler
    {
        public string? Body { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            Body = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"vnp_ResponseCode\":\"00\",\"vnp_TransactionNo\":\"123\"}"
                ),
            };
        }
    }

    private static VnPayOptions Options() =>
        new()
        {
            TmnCode = "TMN",
            HashSecret = "secret",
            ReturnUrl = "https://example.test/return",
            BaseUrl = "https://pay.test/vpcpay",
            ApiUrl = "https://pay.test/api",
        };

    private static VnPayGateway CreateSut(DateTime utcNow, HttpMessageHandler? handler = null) =>
        new(
            Microsoft.Extensions.Options.Options.Create(Options()),
            new HttpClient(handler ?? new CapturingHandler()),
            NullLogger<VnPayGateway>.Instance,
            new FixedTimeProvider(utcNow)
        );

    private static async Task<string> CreateDateAsync(DateTime utcNow)
    {
        var result = await CreateSut(utcNow)
            .CreatePaymentAsync(new CreatePaymentRequest(1, 100_000m, "info", "TXN1"));
        var query = HttpUtility.ParseQueryString(new Uri(result.PaymentUrl).Query);
        return query["vnp_CreateDate"]!;
    }

    [Fact]
    public async Task CreatePayment_CreateDate_IsUtcPlusSevenHours() =>
        (await CreateDateAsync(FixedUtcNow)).ShouldBe("20260315170000");

    [Fact]
    public async Task CreatePayment_CreateDate_RollsOverToNextDay() =>
        (await CreateDateAsync(new DateTime(2026, 12, 31, 20, 30, 15, DateTimeKind.Utc))).ShouldBe(
            "20270101033015"
        );

    [Theory]
    [InlineData("America/New_York")]
    [InlineData("Asia/Tokyo")]
    [InlineData("UTC")]
    public async Task CreatePayment_CreateDate_IgnoresConfiguredBrandTimeZone(string brandZone)
    {
        var previous = BusinessTime.IanaId;
        BusinessTime.Configure(brandZone);
        try
        {
            (await CreateDateAsync(FixedUtcNow)).ShouldBe("20260315170000");
        }
        finally
        {
            BusinessTime.Configure(previous);
        }
    }

    [Fact]
    public async Task CreatePayment_CreateDate_Is14DigitFormat()
    {
        var value = await CreateDateAsync(FixedUtcNow);

        value.Length.ShouldBe(14);
        value.ShouldAllBe(c => char.IsDigit(c));
    }

    [Fact]
    public async Task CreatePayment_SignatureCoversCreateDate()
    {
        var first = await CreateSut(FixedUtcNow)
            .CreatePaymentAsync(new CreatePaymentRequest(1, 100_000m, "info", "TXN1"));
        var second = await CreateSut(FixedUtcNow.AddHours(1))
            .CreatePaymentAsync(new CreatePaymentRequest(1, 100_000m, "info", "TXN1"));

        HttpUtility
            .ParseQueryString(new Uri(first.PaymentUrl).Query)["vnp_SecureHash"]
            .ShouldNotBe(
                HttpUtility.ParseQueryString(new Uri(second.PaymentUrl).Query)["vnp_SecureHash"]
            );
    }

    [Fact]
    public async Task Refund_CreateDateAndTransactionDate_AreUtcPlusSevenHours_EvenWhenBrandZoneDiffers()
    {
        var handler = new CapturingHandler();
        var sut = CreateSut(FixedUtcNow, handler);
        var originalPaymentUtc = new DateTime(2026, 3, 14, 18, 0, 0, DateTimeKind.Unspecified);

        var previous = BusinessTime.IanaId;
        BusinessTime.Configure("America/New_York");
        try
        {
            var result = await sut.RefundAsync(
                new RefundRequest("TXN1", 100_000m, "refund", originalPaymentUtc, "999")
            );
            result.Success.ShouldBeTrue();
        }
        finally
        {
            BusinessTime.Configure(previous);
        }

        using var doc = JsonDocument.Parse(handler.Body!);
        doc.RootElement.GetProperty("vnp_CreateDate").GetString().ShouldBe("20260315170000");
        doc.RootElement.GetProperty("vnp_TransactionDate").GetString().ShouldBe("20260315010000");
    }
}

using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Cinema.Infrastructure.Payments;

public class StripeGateway(IOptions<StripeOptions> options, ILogger<StripeGateway> logger)
    : IPaymentGateway
{
    private readonly StripeOptions _options = options.Value;
    private readonly StripeClient _client = new(options.Value.SecretKey);

    public string MethodName => PaymentMethodNames.Stripe;

    public async Task<CreatePaymentResult> CreatePaymentAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(_options.SecretKey))
            throw new BusinessRuleException(Messages.Rule_StripeSecretKeyMissing);

        var currency = string.IsNullOrWhiteSpace(_options.Currency)
            ? "vnd"
            : _options.Currency.ToLowerInvariant();
        var isZeroDecimal = currency is "vnd" or "jpy" or "krw";
        var unitAmount = isZeroDecimal
            ? (long)Math.Round(request.Amount, 0, MidpointRounding.AwayFromZero)
            : (long)Math.Round(request.Amount * 100m, 0, MidpointRounding.AwayFromZero);

        var sessionService = new SessionService(_client);
        var sessionCreateOptions = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            Mode = "payment",
            ClientReferenceId = request.TransactionId,
            SuccessUrl =
                $"{_options.ReturnUrl}?bookingId={request.BookingId}&session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{_options.ReturnUrl}?bookingId={request.BookingId}&canceled=true",
            LineItems =
            [
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = currency,
                        UnitAmount = unitAmount,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = string.IsNullOrWhiteSpace(request.OrderInfo)
                                ? $"Vé xem phim (Mã GD: {request.TransactionId})"
                                : request.OrderInfo,
                        },
                    },
                    Quantity = 1,
                },
            ],
            Metadata = new Dictionary<string, string>
            {
                ["bookingId"] = request.BookingId.ToString(),
                ["transactionId"] = request.TransactionId,
            },
        };

        var session = await sessionService.CreateAsync(
            sessionCreateOptions,
            cancellationToken: cancellationToken
        );

        return new CreatePaymentResult(session.Url, request.TransactionId);
    }

    public GatewayCallbackResult VerifyCallback(
        IReadOnlyDictionary<string, string> queryOrFormParams
    )
    {
        var sessionId = queryOrFormParams.GetValueOrDefault("session_id", string.Empty);
        var isCanceled = queryOrFormParams.GetValueOrDefault("canceled", "false") == "true";

        return new GatewayCallbackResult(
            SignatureValid: true,
            IsSuccess: !isCanceled,
            TransactionId: sessionId,
            GatewayResponseCode: isCanceled ? "canceled" : "succeeded",
            Amount: 0m,
            BookingId: 0
        );
    }

    public GatewayCallbackResult VerifyWebhook(string rawBody, string? signatureHeader)
    {
        if (string.IsNullOrWhiteSpace(_options.WebhookSecret))
        {
            try
            {
                var unverifiedEvent = EventUtility.ParseEvent(rawBody);
                return ParseStripeEvent(unverifiedEvent, signatureValid: true);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Stripe ParseEvent failed when no WebhookSecret configured.");
                return new GatewayCallbackResult(false, false, string.Empty, "parse_failed", 0m, 0);
            }
        }

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                rawBody,
                signatureHeader,
                _options.WebhookSecret,
                tolerance: 300,
                throwOnApiVersionMismatch: false
            );
            return ParseStripeEvent(stripeEvent, signatureValid: true);
        }
        catch (StripeException ex)
        {
            logger.LogWarning(
                "Stripe webhook verification failed: {Error}. Secret prefix: {SecretPrefix}",
                ex.Message,
                _options.WebhookSecret.Length > 8 ? _options.WebhookSecret[..8] + "..." : "empty"
            );
            return new GatewayCallbackResult(
                false,
                false,
                string.Empty,
                "invalid_signature",
                0m,
                0
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in Stripe VerifyWebhook");
            return new GatewayCallbackResult(
                false,
                false,
                string.Empty,
                "invalid_signature",
                0m,
                0
            );
        }
    }

    private static GatewayCallbackResult ParseStripeEvent(Event stripeEvent, bool signatureValid)
    {
        if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
        {
            if (stripeEvent.Data.Object is Session session)
            {
                var transactionId =
                    session.ClientReferenceId
                    ?? session.Metadata?.GetValueOrDefault("transactionId")
                    ?? string.Empty;
                var bookingIdStr = session.Metadata?.GetValueOrDefault("bookingId") ?? "0";
                _ = int.TryParse(bookingIdStr, out var bookingId);
                var isSuccess = string.Equals(
                    session.PaymentStatus,
                    "paid",
                    StringComparison.OrdinalIgnoreCase
                );
                var currency = session.Currency?.ToLowerInvariant() ?? "vnd";
                var isZeroDecimal = currency is "vnd" or "jpy" or "krw";
                var rawAmount = session.AmountTotal ?? 0;
                var amount = isZeroDecimal ? rawAmount : rawAmount / 100m;

                return new GatewayCallbackResult(
                    SignatureValid: signatureValid,
                    IsSuccess: isSuccess,
                    TransactionId: transactionId,
                    GatewayResponseCode: session.PaymentStatus ?? "paid",
                    Amount: amount,
                    BookingId: bookingId,
                    GatewayTransactionId: session.PaymentIntentId,
                    Currency: currency
                );
            }
        }

        return new GatewayCallbackResult(
            SignatureValid: signatureValid,
            IsSuccess: false,
            TransactionId: string.Empty,
            GatewayResponseCode: stripeEvent.Type,
            Amount: 0m,
            BookingId: 0
        );
    }

    public async Task<RefundResult> RefundAsync(
        RefundRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(_options.SecretKey))
            return new RefundResult(false, null, "Stripe SecretKey chưa được cấu hình.");

        try
        {
            var refundService = new RefundService(_client);
            var currency = string.IsNullOrWhiteSpace(_options.Currency)
                ? "vnd"
                : _options.Currency.ToLowerInvariant();
            var isZeroDecimal = currency is "vnd" or "jpy" or "krw";
            var refundAmount = isZeroDecimal
                ? (long)Math.Round(request.Amount, 0, MidpointRounding.AwayFromZero)
                : (long)Math.Round(request.Amount * 100m, 0, MidpointRounding.AwayFromZero);

            var refundOptions = new RefundCreateOptions
            {
                Amount = refundAmount,
                Reason = RefundReasons.RequestedByCustomer,
                Metadata = new Dictionary<string, string>
                {
                    ["originalTransactionId"] = request.OriginalTransactionId,
                    ["orderInfo"] = request.OrderInfo,
                },
            };

            if (string.IsNullOrWhiteSpace(request.GatewayTransactionId))
            {
                return new RefundResult(
                    false,
                    null,
                    "Không tìm thấy mã giao dịch Stripe gốc (GatewayTransactionId) để hoàn tiền. "
                        + "Đây có thể là giao dịch cũ trước khi hệ thống lưu mã Stripe thật — "
                        + "vui lòng hoàn tiền thủ công qua Stripe Dashboard."
                );
            }

            if (request.GatewayTransactionId.StartsWith("pi_"))
            {
                refundOptions.PaymentIntent = request.GatewayTransactionId;
            }
            else if (request.GatewayTransactionId.StartsWith("ch_"))
            {
                refundOptions.Charge = request.GatewayTransactionId;
            }
            else
            {
                refundOptions.PaymentIntent = request.GatewayTransactionId;
            }

            var refund = await refundService.CreateAsync(
                refundOptions,
                cancellationToken: cancellationToken
            );

            return refund.Status == "succeeded" || refund.Status == "pending"
                ? new RefundResult(true, refund.Id, null)
                : new RefundResult(false, refund.Id, $"Stripe Refund status: {refund.Status}");
        }
        catch (StripeException ex)
        {
            return new RefundResult(
                false,
                null,
                $"Lỗi gọi Stripe refund API: {ex.StripeError?.Message ?? ex.Message}"
            );
        }
        catch (Exception ex)
        {
            return new RefundResult(
                false,
                null,
                $"Lỗi không xác định khi hoàn tiền Stripe: {ex.Message}"
            );
        }
    }
}

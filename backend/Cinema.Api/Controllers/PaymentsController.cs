using System.Text.Json;
using Cinema.Api.Filters;
using Cinema.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers;

[ApiController]
[Route("api/v1/payments")]
public class PaymentsController(IPaymentService paymentService) : ControllerBase
{
    [HttpGet("vnpay/ipn")]
    [HttpPost("vnpay/ipn")]
    [RawResponse]
    public async Task<IActionResult> VnPayIpn(CancellationToken cancellationToken)
    {
        var parameters = await BuildParamsFromQueryAndFormAsync();
        var result = await paymentService.ProcessVnPayIpnAsync(parameters, cancellationToken);
        return Ok(new { RspCode = result.RspCode, Message = result.Message });
    }

    [HttpPost("momo/ipn")]
    [RawResponse]
    public async Task<IActionResult> MomoIpn(CancellationToken cancellationToken)
    {
        var parameters = await BuildParamsFromJsonBodyAsync(cancellationToken);
        var result = await paymentService.ProcessMomoIpnAsync(parameters, cancellationToken);
        return Ok(new { resultCode = result.ResultCode, message = result.Message });
    }

    [HttpPost("stripe/webhook")]
    [RawResponse]
    public async Task<IActionResult> StripeWebhook(CancellationToken cancellationToken)
    {
        Request.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(Request.Body, System.Text.Encoding.UTF8, leaveOpen: true);
        var json = await reader.ReadToEndAsync(cancellationToken);
        var signature = Request.Headers["Stripe-Signature"].ToString();
        var result = await paymentService.ProcessStripeWebhookAsync(json, signature, cancellationToken);

        if (!result.Success && result.Message == "Invalid signature")
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    private async Task<Dictionary<string, string>> BuildParamsFromQueryAndFormAsync()
    {
        var result = new Dictionary<string, string>();
        foreach (var kv in Request.Query)
            result[kv.Key] = kv.Value.ToString();

        if (Request.HasFormContentType)
        {
            var form = await Request.ReadFormAsync();
            foreach (var kv in form)
                result[kv.Key] = kv.Value.ToString();
        }

        return result;
    }

    private async Task<Dictionary<string, string>> BuildParamsFromJsonBodyAsync(
        CancellationToken cancellationToken
    )
    {
        var result = new Dictionary<string, string>();
        using var doc = await JsonDocument.ParseAsync(
            Request.Body,
            cancellationToken: cancellationToken
        );

        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            result[prop.Name] = prop.Value.ValueKind switch
            {
                JsonValueKind.String => prop.Value.GetString() ?? string.Empty,
                JsonValueKind.Number => prop.Value.GetRawText(),
                JsonValueKind.True or JsonValueKind.False => prop.Value.GetBoolean().ToString(),
                _ => prop.Value.GetRawText(),
            };
        }

        return result;
    }
}

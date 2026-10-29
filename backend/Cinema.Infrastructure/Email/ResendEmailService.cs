using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Infrastructure.Email;

public class ResendEmailService(
    IHttpClientFactory httpClientFactory,
    IOptions<EmailOptions> options,
    IOptions<BrandOptions> brandOptions,
    ILogger<ResendEmailService> logger
) : IEmailService
{
    private readonly EmailOptions _options = options.Value;
    private string FromName =>
        string.IsNullOrWhiteSpace(_options.FromName) ? brandOptions.Value.Name : _options.FromName;
    private const string ResendApiUrl = "https://api.resend.com/emails";

    public async Task SendAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default
    )
    {
        var client = httpClientFactory.CreateClient("Resend");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.ResendApiKey);

        var payload = new
        {
            from = $"{FromName} <{(string.IsNullOrWhiteSpace(_options.ResendFromAddress) ? _options.FromAddress : _options.ResendFromAddress)}>",
            to = new[] { toEmail },
            subject,
            html = htmlBody,
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync(ResendApiUrl, content, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError(
                    "ResendEmailFailed To: {ToEmail}, Status: {Status}, Body: {Body}",
                    toEmail,
                    (int)response.StatusCode,
                    body
                );
                throw new InvalidOperationException(
                    $"Resend API trả về {(int)response.StatusCode}: {body}"
                );
            }

            logger.LogInformation(
                "Email sent successfully via Resend to {ToEmail} with subject '{Subject}'",
                toEmail,
                subject
            );
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            logger.LogError(
                ex,
                "ResendEmailFailed To: {ToEmail}, Error: {Message}",
                toEmail,
                ex.Message
            );
            throw;
        }
    }
}

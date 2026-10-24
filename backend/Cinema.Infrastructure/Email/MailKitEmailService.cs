using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Cinema.Infrastructure.Email;

public class MailKitEmailService(
    IOptions<EmailOptions> options,
    ILogger<MailKitEmailService> logger
) : IEmailService
{
    private readonly EmailOptions _options = options.Value;

    public async Task SendAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default
    )
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(
                _options.SmtpHost,
                _options.SmtpPort,
                SecureSocketOptions.Auto,
                cancellationToken
            );
            if (!string.IsNullOrEmpty(_options.SmtpUser))
                await client.AuthenticateAsync(
                    _options.SmtpUser,
                    _options.SmtpPassword,
                    cancellationToken
                );

            await client.SendAsync(message, cancellationToken);
            logger.LogInformation("Email sent successfully to {ToEmail} with subject '{Subject}'", toEmail, subject);
        }
        finally
        {
            if (client.IsConnected)
                await client.DisconnectAsync(true, cancellationToken);
        }
    }
}

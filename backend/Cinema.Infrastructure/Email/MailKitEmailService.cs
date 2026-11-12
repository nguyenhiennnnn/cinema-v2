using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Shared.Resources;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Cinema.Infrastructure.Email;

public class MailKitEmailService(
    IOptions<EmailOptions> options,
    IOptions<BrandOptions> brandOptions,
    ILogger<MailKitEmailService> logger
) : IEmailService
{
    private readonly EmailOptions _options = options.Value;
    private string FromName =>
        string.IsNullOrWhiteSpace(_options.FromName) ? brandOptions.Value.Name : _options.FromName;

    public async Task SendAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default
    )
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(FromName, _options.FromAddress));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient { Timeout = 15_000 };
        try
        {
            if (string.IsNullOrWhiteSpace(_options.SmtpHost))
                throw new InvalidOperationException(Messages.Config_EmailNotConfigured);

            var socketOptions =
                _options.SmtpPort == 465
                    ? SecureSocketOptions.SslOnConnect
                    : SecureSocketOptions.StartTls;

            await client.ConnectAsync(
                _options.SmtpHost,
                _options.SmtpPort,
                socketOptions,
                cancellationToken
            );
            if (!string.IsNullOrEmpty(_options.SmtpUser))
                await client.AuthenticateAsync(
                    _options.SmtpUser,
                    _options.SmtpPassword,
                    cancellationToken
                );

            await client.SendAsync(message, cancellationToken);
            logger.LogInformation(
                "Email sent successfully to {ToEmail} with subject '{Subject}'",
                toEmail,
                subject
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "EmailSendFailed To: {ToEmail}, SmtpHost: {Host}:{Port}, User: {User}, Error: {Message}",
                toEmail,
                _options.SmtpHost,
                _options.SmtpPort,
                _options.SmtpUser,
                ex.Message
            );
            throw;
        }
        finally
        {
            if (client.IsConnected)
                await client.DisconnectAsync(true, cancellationToken);
        }
    }
}

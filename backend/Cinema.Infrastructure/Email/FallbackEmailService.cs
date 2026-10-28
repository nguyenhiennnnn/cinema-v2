using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Microsoft.Extensions.Options;

namespace Cinema.Infrastructure.Email;

public class FallbackEmailService(
    ResendEmailService resend,
    MailKitEmailService smtp,
    IOptions<EmailOptions> options
) : IEmailService
{
    public async Task SendAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default
    )
    {
        var settings = options.Value;
        var hasSmtp = !string.IsNullOrWhiteSpace(settings.SmtpHost);

        if (!string.IsNullOrWhiteSpace(settings.ResendApiKey))
        {
            try
            {
                await resend.SendAsync(toEmail, subject, htmlBody, cancellationToken);
                return;
            }
            catch when (hasSmtp) { }
        }

        await smtp.SendAsync(toEmail, subject, htmlBody, cancellationToken);
    }
}

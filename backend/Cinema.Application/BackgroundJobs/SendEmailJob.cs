using Cinema.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cinema.Application.BackgroundJobs;

public class SendEmailJob(IEmailService emailService, ILogger<SendEmailJob> logger)
{
    public async Task RunAsync(
        string toEmail,
        string subject,
        string htmlbody,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "EmailQueued dispatch — To: {ToEmail}, Subject: {Subject}",
            toEmail,
            subject
        );
        await emailService.SendAsync(toEmail, subject, htmlbody, cancellationToken);
    }
}

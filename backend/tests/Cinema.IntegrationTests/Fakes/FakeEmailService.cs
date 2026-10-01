using System.Collections.Concurrent;
using Cinema.Application.Interfaces;

namespace Cinema.IntegrationTests.Fakes;

public record SentEmail(string ToEmail, string Subject, string HtmlBody, DateTime SentAtUtc);

public class FakeEmailService : IEmailService
{
    private readonly ConcurrentBag<SentEmail> _sent = [];

    public IReadOnlyCollection<SentEmail> SentEmails => _sent.ToList();

    public Task SendAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default
    )
    {
        _sent.Add(new SentEmail(toEmail, subject, htmlBody, DateTime.UtcNow));
        return Task.CompletedTask;
    }

    public void Clear() => _sent.Clear();
}

namespace Cinema.Application.Options;

public class EmailOptions
{
    public const string SectionName = "Email";

    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUser { get; set; } = string.Empty;

    public string SmtpPassword { get; set; } = string.Empty;

    public string FromAddress { get; set; } = "no-reply@cinema.local";
    public string FromName { get; set; } = "Cinema";

    public int EmailConfirmationExpirationHours { get; set; } = 24;

    public int PasswordResetExpirationMinutes { get; set; } = 30;
}

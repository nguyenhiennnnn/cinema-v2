namespace Cinema.Application.Options;

public class AppUrlOptions
{
    public const string SectionName = "App";

    public string ApiBaseUrl { get; set; } = "https://localhost:8080";

    public string FrontendBaseUrl { get; set; } = "http://localhost:5173";
}

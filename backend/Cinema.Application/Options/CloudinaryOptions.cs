namespace Cinema.Application.Options;

public class CloudinaryOptions
{
    public const string SectionName = "Cloudinary";

    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;

    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;
    public string[] AllowedExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".webp"];

    public long MaxVideoFileSizeBytes { get; set; } = 100 * 1024 * 1024;
    public string[] AllowedVideoExtensions { get; set; } = [".mp4", ".mov", ".webm", ".avi"];
}

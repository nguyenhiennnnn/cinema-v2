namespace Cinema.Application.Utils;

public static class ImportLimits
{
    public const int MaxFileBytes = 5 * 1024 * 1024;
    public const int MaxRequestBytes = MaxFileBytes + 1024 * 1024;
    public const int MaxRows = 1000;
    public const long MaxUncompressedBytes = 50L * 1024 * 1024;
    public static readonly TimeSpan PreviewTtl = TimeSpan.FromMinutes(30);
}

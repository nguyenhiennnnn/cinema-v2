namespace Cinema.Application.Interfaces;

public record FileUploadResult(string Url, string PublicId);

public interface IFileStorage
{
    Task<FileUploadResult> UploadImageAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken = default
    );

    Task<FileUploadResult> UploadVideoAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken = default
    );

    Task<FileUploadResult> UploadRawAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken = default
    );

    Task<Stream> OpenReadAsync(string url, CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string publicId,
        string resourceType = "image",
        CancellationToken cancellationToken = default
    );
}

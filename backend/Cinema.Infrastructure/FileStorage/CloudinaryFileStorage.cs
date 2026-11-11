using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace Cinema.Infrastructure.FileStorage;

public class CloudinaryFileStorage : IFileStorage
{
    private readonly Cloudinary _cloudinary;
    private readonly CloudinaryOptions _options;

    public CloudinaryFileStorage(IOptions<CloudinaryOptions> options)
    {
        _options = options.Value;
        var account = new Account(_options.CloudName, _options.ApiKey, _options.ApiSecret);
        _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
    }

    public async Task<FileUploadResult> UploadImageAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken = default
    )
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!_options.AllowedExtensions.Contains(extension))
            throw new ValidationAppException(
                string.Format(
                    Messages.Invalid_FileExtensionNotAllowed,
                    string.Join(", ", _options.AllowedExtensions)
                )
            );

        if (fileStream.Length > _options.MaxFileSizeBytes)
            throw new ValidationAppException(
                string.Format(
                    Messages.Invalid_FileTooLarge,
                    _options.MaxFileSizeBytes / (1024 * 1024)
                )
            );

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = folder,
            UseFilename = true,
            UniqueFilename = true,
            Overwrite = false,
        };

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
        if (result.Error is not null)
            throw new BusinessRuleException(
                string.Format(Messages.Rule_CloudinaryUploadFailed, result.Error.Message)
            );

        return new FileUploadResult(result.SecureUrl.ToString(), result.PublicId);
    }

    public async Task<FileUploadResult> UploadVideoAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken = default
    )
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!_options.AllowedVideoExtensions.Contains(extension))
            throw new ValidationAppException(
                string.Format(
                    Messages.Invalid_VideoExtensionNotAllowed,
                    string.Join(", ", _options.AllowedVideoExtensions)
                )
            );

        if (fileStream.Length > _options.MaxVideoFileSizeBytes)
            throw new ValidationAppException(
                string.Format(
                    Messages.Invalid_VideoTooLarge,
                    _options.MaxVideoFileSizeBytes / (1024 * 1024)
                )
            );

        var uploadParams = new VideoUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = folder,
            UseFilename = true,
            UniqueFilename = true,
            Overwrite = false,
        };

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
        if (result.Error is not null)
            throw new BusinessRuleException(
                string.Format(Messages.Rule_CloudinaryUploadFailed, result.Error.Message)
            );

        return new FileUploadResult(result.SecureUrl.ToString(), result.PublicId);
    }

    public async Task DeleteAsync(
        string publicId,
        string resourceType = "image",
        CancellationToken cancellationToken = default
    )
    {
        var deleteParams = new DeletionParams(publicId)
        {
            ResourceType = resourceType.Equals("video", StringComparison.OrdinalIgnoreCase)
                ? ResourceType.Video
                : ResourceType.Image,
        };

        await _cloudinary.DestroyAsync(deleteParams);
    }
}

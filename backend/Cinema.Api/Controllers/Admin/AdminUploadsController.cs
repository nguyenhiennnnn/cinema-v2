using Cinema.Application.DTOs.Uploads;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/uploads")]
[Authorize(Policy = Policies.AdminOnly)]
public class AdminUploadsController(IFileStorage fileStorage) : ControllerBase
{
    [HttpPost("image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage(
        [FromQuery] string folder = "misc",
        IFormFile? file = null,
        CancellationToken cancellationToken = default
    )
    {
        if (file is null || file.Length == 0)
            throw new ValidationAppException(
                Messages.Invalid_UploadFileMissing,
                new Dictionary<string, string[]> { ["file"] = [Messages.Invalid_FileEmpty] }
            );

        await using var stream = file.OpenReadStream();
        var result = await fileStorage.UploadImageAsync(
            stream,
            file.FileName,
            file.ContentType,
            folder,
            cancellationToken
        );

        return Ok(new UploadImageResponse(result.Url, result.PublicId));
    }

    [HttpPost("video")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(100 * 1024 * 1024)]
    public async Task<IActionResult> UploadVideo(
        [FromQuery] string folder = "misc",
        IFormFile? file = null,
        CancellationToken cancellationToken = default
    )
    {
        if (file is null || file.Length == 0)
            throw new ValidationAppException(
                Messages.Invalid_UploadFileMissing,
                new Dictionary<string, string[]> { ["file"] = [Messages.Invalid_FileEmpty] }
            );

        await using var stream = file.OpenReadStream();
        var result = await fileStorage.UploadVideoAsync(
            stream,
            file.FileName,
            file.ContentType,
            folder,
            cancellationToken
        );

        return Ok(new UploadImageResponse(result.Url, result.PublicId));
    }
}

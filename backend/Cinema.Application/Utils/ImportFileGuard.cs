using System.IO.Compression;
using Cinema.Application.Interfaces;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;

namespace Cinema.Application.Utils;

public sealed record ImportUpload(MemoryStream Content, ImportFileFormat Format) : IDisposable
{
    public void Dispose() => Content.Dispose();
}

public static class ImportFileGuard
{
    private const int TextProbeBytes = 8192;
    private const string ContentTypesEntry = "[Content_Types].xml";
    private const string WorkbookEntry = "xl/workbook.xml";

    private static readonly byte[] ZipSignature = [0x50, 0x4B, 0x03, 0x04];

    private static readonly byte[][] BinarySignatures =
    [
        [0x50, 0x4B],
        [0xD0, 0xCF, 0x11, 0xE0],
        [0x25, 0x50, 0x44, 0x46],
        [0x4D, 0x5A],
        [0x7F, 0x45, 0x4C, 0x46],
        [0x89, 0x50, 0x4E, 0x47],
        [0xFF, 0xD8, 0xFF],
        [0x47, 0x49, 0x46, 0x38],
        [0x52, 0x61, 0x72, 0x21],
        [0x1F, 0x8B],
    ];

    public static async Task<ImportUpload> ReadAsync(
        string? fileName,
        Stream content,
        CancellationToken cancellationToken
    )
    {
        var format = ResolveFormat(fileName);
        var buffer = new MemoryStream();

        try
        {
            var chunk = new byte[81920];
            int read;
            while (
                (read = await content.ReadAsync(chunk.AsMemory(), cancellationToken)) > 0
            )
            {
                if (buffer.Length + read > ImportLimits.MaxFileBytes)
                    throw Invalid(
                        string.Format(
                            Messages.Invalid_FileTooLarge,
                            ImportLimits.MaxFileBytes / (1024 * 1024)
                        )
                    );

                buffer.Write(chunk, 0, read);
            }

            if (buffer.Length == 0)
                throw Invalid(Messages.Invalid_FileEmpty);

            if (format == ImportFileFormat.Xlsx)
                EnsureWorkbook(buffer);
            else
                EnsureText(buffer);

            buffer.Position = 0;
            return new ImportUpload(buffer, format);
        }
        catch
        {
            buffer.Dispose();
            throw;
        }
    }

    private static ImportFileFormat ResolveFormat(string? fileName) =>
        Path.GetExtension(fileName ?? string.Empty).ToLowerInvariant() switch
        {
            ".xlsx" => ImportFileFormat.Xlsx,
            ".csv" => ImportFileFormat.Csv,
            _ => throw Invalid(string.Format(Messages.Invalid_FileExtensionNotAllowed, ".xlsx, .csv")),
        };

    private static void EnsureWorkbook(MemoryStream buffer)
    {
        var bytes = buffer.GetBuffer();
        if (buffer.Length < ZipSignature.Length || !StartsWith(bytes, ZipSignature))
            throw Invalid(Messages.Invalid_ImportFileContent);

        buffer.Position = 0;
        try
        {
            using var archive = new ZipArchive(buffer, ZipArchiveMode.Read, leaveOpen: true);
            long total = 0;
            var hasContentTypes = false;
            var hasWorkbook = false;

            foreach (var entry in archive.Entries)
            {
                total += entry.Length;
                if (total > ImportLimits.MaxUncompressedBytes)
                    throw Invalid(Messages.Invalid_ImportFileContent);

                if (entry.FullName == ContentTypesEntry)
                    hasContentTypes = true;
                else if (entry.FullName == WorkbookEntry)
                    hasWorkbook = true;
            }

            if (!hasContentTypes || !hasWorkbook)
                throw Invalid(Messages.Invalid_ImportFileContent);
        }
        catch (InvalidDataException)
        {
            throw Invalid(Messages.Invalid_ImportFileContent);
        }
    }

    private static void EnsureText(MemoryStream buffer)
    {
        var bytes = buffer.GetBuffer();
        var probe = (int)Math.Min(buffer.Length, TextProbeBytes);

        if (BinarySignatures.Any(signature => StartsWith(bytes, signature, probe)))
            throw Invalid(Messages.Invalid_ImportFileContent);

        if (Array.IndexOf(bytes, (byte)0, 0, probe) >= 0)
            throw Invalid(Messages.Invalid_ImportFileContent);
    }

    private static bool StartsWith(byte[] bytes, byte[] signature, int? available = null)
    {
        var length = available ?? bytes.Length;
        if (length < signature.Length)
            return false;

        for (var i = 0; i < signature.Length; i++)
        {
            if (bytes[i] != signature[i])
                return false;
        }

        return true;
    }

    private static ValidationAppException Invalid(string message) =>
        new(message, new Dictionary<string, string[]> { ["file"] = [message] });
}

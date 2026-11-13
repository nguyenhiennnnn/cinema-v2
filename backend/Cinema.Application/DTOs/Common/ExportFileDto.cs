namespace Cinema.Application.DTOs.Common;

public record ExportFileDto(Stream Content, string ContentType, string FileName);

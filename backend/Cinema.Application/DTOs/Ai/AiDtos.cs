namespace Cinema.Application.DTOs.Ai;

public record ChatMessageDto(string Role, string Content);

public record AiChatRequest(
    string Message,
    List<ChatMessageDto>? History
);

public record AiCardDto(
    string Type,
    string Title,
    string? Subtitle,
    string? Detail,
    string? ImageUrl,
    string Link
);

public record AiChatResponse(
    string Reply,
    DateTime Timestamp,
    List<AiCardDto>? Cards = null
)
{
    public List<AiCardDto> Cards { get; init; } = Cards ?? [];
}

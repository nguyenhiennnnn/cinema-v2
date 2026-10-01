namespace Cinema.Application.DTOs.Ai;

public record ChatMessageDto(string Role, string Content);

public record AiChatRequest(
    string Message,
    List<ChatMessageDto>? History
);

public record AiChatResponse(
    string Reply,
    DateTime Timestamp
);

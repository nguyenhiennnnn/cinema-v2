using Cinema.Application.DTOs.Ai;

namespace Cinema.Application.Interfaces;

public interface IAiAssistantService
{
    Task<AiChatResponse> ChatAsync(int? userId, AiChatRequest request, CancellationToken cancellationToken);
}

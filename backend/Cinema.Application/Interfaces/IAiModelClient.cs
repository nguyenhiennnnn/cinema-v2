using Cinema.Application.Services.Ai;

namespace Cinema.Application.Interfaces;

public interface IAiModelClient
{
    bool IsConfigured { get; }

    Task<AiMessage> GenerateAsync(AiModelRequest request, CancellationToken cancellationToken);
}

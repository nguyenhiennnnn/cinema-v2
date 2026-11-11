using System.Text.Json.Nodes;
using Cinema.Application.DTOs.Ai;
using Cinema.Application.Interfaces;
using Cinema.Shared.Resources;
using Microsoft.Extensions.Logging;

namespace Cinema.Application.Services.Ai;

public class AiAssistantService(
    IAiModelClient modelClient,
    AiSystemPromptBuilder promptBuilder,
    AiToolExecutor toolExecutor,
    ILogger<AiAssistantService> logger
) : IAiAssistantService
{
    private const int MaxToolRounds = 4;

    private const int MaxTotalToolCalls = 8;

    private const int MaxCards = 6;

    private const double Temperature = 0.5;

    private const int MaxOutputTokens = 2048;

    public async Task<AiChatResponse> ChatAsync(
        int? userId,
        AiChatRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!modelClient.IsConfigured)
        {
            logger.LogWarning("AI model client is not configured (missing API key).");
            return new AiChatResponse(Messages.Ai_NotConfigured, DateTime.UtcNow);
        }

        var systemPrompt = await promptBuilder.BuildSystemPromptAsync(userId, cancellationToken);
        var tools = AiToolExecutor.GetToolDefinitions(userId.HasValue);

        var messages = new List<AiMessage>();
        if (request.History is not null)
        {
            foreach (var h in request.History)
            {
                var role =
                    h.Role.ToLowerInvariant() == "user" ? AiMessageRole.User : AiMessageRole.Model;
                messages.Add(AiMessage.FromText(role, h.Content));
            }
        }
        messages.Add(AiMessage.FromText(AiMessageRole.User, request.Message));

        var cards = new List<AiCardDto>();
        var totalToolCalls = 0;

        try
        {
            for (var round = 0; round <= MaxToolRounds; round++)
            {
                var allowTools = round < MaxToolRounds;

                AiMessage modelMessage;
                try
                {
                    modelMessage = await modelClient.GenerateAsync(
                        new AiModelRequest(
                            systemPrompt,
                            messages,
                            allowTools ? tools : [],
                            Temperature,
                            MaxOutputTokens
                        ),
                        cancellationToken
                    );
                }
                catch (AiModelException ex)
                {
                    logger.LogError(ex, "AI model call failed.");
                    return new AiChatResponse(
                        Messages.Ai_ConnectionFailed,
                        DateTime.UtcNow,
                        FinalizeCards(cards)
                    );
                }

                var functionCalls = modelMessage.Parts.OfType<AiFunctionCallPart>().ToList();

                if (!allowTools || functionCalls.Count == 0)
                {
                    var reply = string.Concat(
                        modelMessage
                            .Parts.OfType<AiTextPart>()
                            .Select(p => p.Text)
                            .Where(t => !string.IsNullOrEmpty(t))
                    );

                    if (string.IsNullOrWhiteSpace(reply))
                        reply = Messages.Ai_NoReply;

                    return new AiChatResponse(reply.Trim(), DateTime.UtcNow, FinalizeCards(cards));
                }

                messages.Add(modelMessage);

                var results = new List<AiMessagePart>();
                foreach (var call in functionCalls)
                {
                    AiToolExecutor.ToolResult result;
                    if (++totalToolCalls > MaxTotalToolCalls)
                    {
                        result = new AiToolExecutor.ToolResult(
                            new JsonObject { ["error"] = Messages.Ai_ToolCallLimitExceeded },
                            []
                        );
                    }
                    else
                    {
                        result = await toolExecutor.ExecuteAsync(
                            call.Name,
                            call.Args,
                            userId,
                            cancellationToken
                        );
                        cards.AddRange(result.Cards);
                    }

                    results.Add(new AiFunctionResultPart(call.Name, result.Response));
                }

                messages.Add(new AiMessage(AiMessageRole.Tool, results));
            }

            return new AiChatResponse(Messages.Ai_NoReply, DateTime.UtcNow);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi xử lý hội thoại với Trợ lý AI.");
            return new AiChatResponse(Messages.Ai_ProcessingError, DateTime.UtcNow);
        }
    }

    private static List<AiCardDto> FinalizeCards(List<AiCardDto> cards) =>
        cards.DistinctBy(c => $"{c.Type}|{c.Link}").Take(MaxCards).ToList();
}

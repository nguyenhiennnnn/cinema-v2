using System.Text.Json.Nodes;

namespace Cinema.Application.Services.Ai;

public enum AiMessageRole
{
    User,
    Model,
    Tool,
}

public abstract record AiMessagePart;

public sealed record AiTextPart(string Text) : AiMessagePart;

public sealed record AiFunctionCallPart(
    string Name,
    JsonObject? Args,
    string? ProviderState = null
) : AiMessagePart;

public sealed record AiFunctionResultPart(string Name, JsonObject Response) : AiMessagePart;

public sealed record AiMessage(AiMessageRole Role, IReadOnlyList<AiMessagePart> Parts)
{
    public static AiMessage FromText(AiMessageRole role, string text) =>
        new(role, [new AiTextPart(text)]);
}

public enum AiToolParameterType
{
    String,
    Integer,
    Number,
    Boolean,
}

public sealed record AiToolParameter(
    string Name,
    AiToolParameterType Type,
    string Description,
    bool Required = false,
    IReadOnlyList<string>? AllowedValues = null
);

public sealed record AiToolDefinition(
    string Name,
    string Description,
    IReadOnlyList<AiToolParameter> Parameters
);

public sealed record AiModelRequest(
    string SystemPrompt,
    IReadOnlyList<AiMessage> Messages,
    IReadOnlyList<AiToolDefinition> Tools,
    double Temperature,
    int MaxOutputTokens
);

public class AiModelException(string message, Exception? inner = null)
    : Exception(message, inner);

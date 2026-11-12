using System.Text;
using System.Text.Json.Nodes;
using Cinema.Application.Interfaces;
using Cinema.Application.Services.Ai;
using Cinema.Shared.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Infrastructure.Ai;

public class GeminiAiModelClient(
    HttpClient httpClient,
    IOptions<GeminiOptions> options,
    ILogger<GeminiAiModelClient> logger
) : IAiModelClient
{
    private readonly GeminiOptions _options = options.Value;

    public bool IsConfigured => !string.IsNullOrWhiteSpace(ResolveApiKey());

    public async Task<AiMessage> GenerateAsync(
        AiModelRequest request,
        CancellationToken cancellationToken
    )
    {
        var apiKey = ResolveApiKey();
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new AiModelException("Gemini API key is not configured.");

        var payload = BuildPayload(request);
        var url = $"{_options.BaseUrl.TrimEnd('/')}/{_options.Model}:generateContent";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"),
        };
        httpRequest.Headers.Add("x-goog-api-key", apiKey);

        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new AiModelException($"Gemini API error ({(int)response.StatusCode}): {body}");

        return ParseResponse(body);
    }

    private string ResolveApiKey() =>
        string.IsNullOrWhiteSpace(_options.ApiKey)
            ? Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? string.Empty
            : _options.ApiKey;

    private static JsonObject BuildPayload(AiModelRequest request)
    {
        var payload = new JsonObject
        {
            ["system_instruction"] = new JsonObject
            {
                ["parts"] = new JsonArray(new JsonObject { ["text"] = request.SystemPrompt }),
            },
            ["contents"] = new JsonArray(request.Messages.Select(ToContent).ToArray<JsonNode?>()),
            ["generationConfig"] = new JsonObject
            {
                ["temperature"] = request.Temperature,
                ["maxOutputTokens"] = request.MaxOutputTokens,
            },
        };

        if (request.Tools.Count > 0)
        {
            payload["tools"] = new JsonArray(
                new JsonObject
                {
                    ["functionDeclarations"] = new JsonArray(
                        request.Tools.Select(ToFunctionDeclaration).ToArray<JsonNode?>()
                    ),
                }
            );
        }

        return payload;
    }

    private static JsonObject ToContent(AiMessage message) =>
        new()
        {
            ["role"] = message.Role == AiMessageRole.Model ? "model" : "user",
            ["parts"] = new JsonArray(message.Parts.Select(ToPart).ToArray<JsonNode?>()),
        };

    private static JsonObject ToPart(AiMessagePart part)
    {
        switch (part)
        {
            case AiTextPart text:
                return new JsonObject { ["text"] = text.Text };

            case AiFunctionCallPart call:
                var callPart = new JsonObject
                {
                    ["functionCall"] = new JsonObject
                    {
                        ["name"] = call.Name,
                        ["args"] = call.Args?.DeepClone() ?? new JsonObject(),
                    },
                };
                if (!string.IsNullOrEmpty(call.ProviderState))
                    callPart["thoughtSignature"] = call.ProviderState;
                return callPart;

            case AiFunctionResultPart result:
                return new JsonObject
                {
                    ["functionResponse"] = new JsonObject
                    {
                        ["name"] = result.Name,
                        ["response"] = result.Response.DeepClone(),
                    },
                };

            default:
                throw new NotSupportedException(
                    string.Format(Messages.Ai_UnsupportedMessagePart, part.GetType().Name)
                );
        }
    }

    private static JsonObject ToFunctionDeclaration(AiToolDefinition tool)
    {
        var declaration = new JsonObject
        {
            ["name"] = tool.Name,
            ["description"] = tool.Description,
        };

        if (tool.Parameters.Count == 0)
            return declaration;

        var properties = new JsonObject();
        foreach (var p in tool.Parameters)
        {
            var property = new JsonObject
            {
                ["type"] = ToGeminiType(p.Type),
                ["description"] = p.Description,
            };
            if (p.AllowedValues is { Count: > 0 })
                property["enum"] = new JsonArray(
                    p.AllowedValues.Select(v => (JsonNode?)v).ToArray()
                );
            properties[p.Name] = property;
        }

        var parameters = new JsonObject { ["type"] = "OBJECT", ["properties"] = properties };

        var required = tool
            .Parameters.Where(p => p.Required)
            .Select(p => (JsonNode?)p.Name)
            .ToArray();
        if (required.Length > 0)
            parameters["required"] = new JsonArray(required);

        declaration["parameters"] = parameters;
        return declaration;
    }

    private static string ToGeminiType(AiToolParameterType type) =>
        type switch
        {
            AiToolParameterType.Integer => "INTEGER",
            AiToolParameterType.Number => "NUMBER",
            AiToolParameterType.Boolean => "BOOLEAN",
            AiToolParameterType.String => "STRING",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };

    private AiMessage ParseResponse(string body)
    {
        var root = JsonNode.Parse(body);
        if (root?["candidates"] is JsonArray { Count: > 0 } candidates)
        {
            if (candidates[0]?["content"] is JsonObject content)
                return new AiMessage(
                    AiMessageRole.Model,
                    ParseParts(content["parts"] as JsonArray)
                );

            logger.LogWarning(
                "Gemini returned no content. FinishReason: {Reason}",
                candidates[0]?["finishReason"]?.ToString()
            );
            return new AiMessage(AiMessageRole.Model, []);
        }

        logger.LogWarning("Gemini returned no candidates. Body: {Body}", body);
        return new AiMessage(AiMessageRole.Model, []);
    }

    private static List<AiMessagePart> ParseParts(JsonArray? parts)
    {
        var result = new List<AiMessagePart>();
        if (parts is null)
            return result;

        foreach (var part in parts.OfType<JsonObject>())
        {
            if (part["functionCall"] is JsonObject call)
            {
                result.Add(
                    new AiFunctionCallPart(
                        call["name"]?.GetValue<string>() ?? string.Empty,
                        call["args"] as JsonObject,
                        part["thoughtSignature"]?.GetValue<string>()
                    )
                );
            }
            else if (part["text"]?.GetValue<string>() is { Length: > 0 } text)
            {
                result.Add(new AiTextPart(text));
            }
        }

        return result;
    }
}

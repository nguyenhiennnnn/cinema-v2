using System.Text;
using System.Text.Json.Nodes;
using Cinema.Application.DTOs.Ai;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Ai;

public class GeminiAiAssistantService(
    IAppDbContext db,
    HttpClient httpClient,
    AiToolExecutor toolExecutor,
    IOptions<GeminiOptions> options,
    ILogger<GeminiAiAssistantService> logger,
    IOptions<BrandOptions>? brandOptions = null
) : IAiAssistantService
{
    private readonly BrandOptions _brand = brandOptions?.Value ?? new();

    private const int MaxToolRounds = 4;

    private const int MaxTotalToolCalls = 8;

    private const int MaxCards = 6;

    private readonly GeminiOptions _options = options.Value;

    public async Task<AiChatResponse> ChatAsync(
        int? userId,
        AiChatRequest request,
        CancellationToken cancellationToken
    )
    {
        var apiKey = string.IsNullOrWhiteSpace(_options.ApiKey)
            ? Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? string.Empty
            : _options.ApiKey;

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogWarning("Gemini API key is not configured.");
            return new AiChatResponse(
                "Xin lỗi, tính năng AI Assistant hiện chưa được cấu hình API Key trên hệ thống. Vui lòng liên hệ Quản trị viên để thiết lập GEMINI_API_KEY.",
                DateTime.UtcNow
            );
        }

        var systemPrompt = await BuildSystemPromptAsync(userId, cancellationToken);

        var contents = new JsonArray();
        if (request.History is not null)
        {
            foreach (var h in request.History)
            {
                var role = h.Role.ToLowerInvariant() == "user" ? "user" : "model";
                contents.Add(TextContent(role, h.Content));
            }
        }
        contents.Add(TextContent("user", request.Message));

        var cards = new List<AiCardDto>();
        var totalToolCalls = 0;

        try
        {
            for (var round = 0; round <= MaxToolRounds; round++)
            {
                var allowTools = round < MaxToolRounds;

                var payload = new JsonObject
                {
                    ["system_instruction"] = new JsonObject
                    {
                        ["parts"] = new JsonArray(new JsonObject { ["text"] = systemPrompt }),
                    },
                    ["contents"] = contents.DeepClone(),
                    ["generationConfig"] = new JsonObject
                    {
                        ["temperature"] = 0.5,
                        ["maxOutputTokens"] = 2048,
                    },
                };
                if (allowTools)
                    payload["tools"] = AiToolExecutor.BuildFunctionDeclarations(userId.HasValue);

                var modelContent = await CallGeminiAsync(payload, apiKey, cancellationToken);
                if (modelContent is null)
                {
                    return new AiChatResponse(
                        "Rất tiếc, kết nối với trợ lý AI Gemini thất bại. Vui lòng thử lại sau.",
                        DateTime.UtcNow,
                        FinalizeCards(cards)
                    );
                }

                var parts = modelContent["parts"] as JsonArray;
                var functionCalls = parts
                    ?.OfType<JsonObject>()
                    .Where(p => p["functionCall"] is JsonObject)
                    .ToList();

                if (!allowTools || functionCalls is null || functionCalls.Count == 0)
                {
                    var reply = string.Concat(
                        parts
                            ?.OfType<JsonObject>()
                            .Select(p => p["text"]?.GetValue<string>())
                            .Where(t => !string.IsNullOrEmpty(t))
                            ?? []
                    );

                    if (string.IsNullOrWhiteSpace(reply))
                        reply = "Trợ lý AI không tạo được câu trả lời phù hợp. Vui lòng thử lại.";

                    return new AiChatResponse(reply.Trim(), DateTime.UtcNow, FinalizeCards(cards));
                }

                var echoed = (JsonObject)modelContent.DeepClone();
                echoed["role"] ??= "model";
                contents.Add(echoed);

                var responseParts = new JsonArray();
                foreach (var part in functionCalls)
                {
                    var call = (JsonObject)part["functionCall"]!;
                    var name = call["name"]?.GetValue<string>() ?? string.Empty;
                    var args = call["args"] as JsonObject;

                    AiToolExecutor.ToolResult result;
                    if (++totalToolCalls > MaxTotalToolCalls)
                    {
                        result = new AiToolExecutor.ToolResult(
                            new JsonObject
                            {
                                ["error"] =
                                    "Đã vượt quá số lần truy vấn cho phép, hãy trả lời với dữ liệu đã có.",
                            },
                            []
                        );
                    }
                    else
                    {
                        result = await toolExecutor.ExecuteAsync(
                            name,
                            args,
                            userId,
                            cancellationToken
                        );
                        cards.AddRange(result.Cards);
                    }

                    responseParts.Add(
                        new JsonObject
                        {
                            ["functionResponse"] = new JsonObject
                            {
                                ["name"] = name,
                                ["response"] = result.Response,
                            },
                        }
                    );
                }

                contents.Add(new JsonObject { ["role"] = "user", ["parts"] = responseParts });
            }

            return new AiChatResponse(
                "Trợ lý AI không tạo được câu trả lời phù hợp. Vui lòng thử lại.",
                DateTime.UtcNow
            );
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi gọi Gemini AI Assistant API.");
            return new AiChatResponse(
                "Đã xảy ra lỗi trong quá trình xử lý yêu cầu với Trợ lý AI. Vui lòng thử lại sau.",
                DateTime.UtcNow
            );
        }
    }

    private async Task<JsonObject?> CallGeminiAsync(
        JsonObject payload,
        string apiKey,
        CancellationToken cancellationToken
    )
    {
        var url = $"{_options.BaseUrl.TrimEnd('/')}/{_options.Model}:generateContent";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"),
        };
        httpRequest.Headers.Add("x-goog-api-key", apiKey);

        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Gemini API error ({StatusCode}): {Body}", response.StatusCode, body);
            return null;
        }

        var root = JsonNode.Parse(body);
        if (root?["candidates"] is JsonArray { Count: > 0 } candidates)
        {
            if (candidates[0]?["content"] is JsonObject content)
                return content;

            logger.LogWarning(
                "Gemini returned no content. FinishReason: {Reason}",
                candidates[0]?["finishReason"]?.ToString()
            );
            return new JsonObject { ["role"] = "model", ["parts"] = new JsonArray() };
        }

        logger.LogWarning("Gemini returned no candidates. Body: {Body}", body);
        return new JsonObject { ["role"] = "model", ["parts"] = new JsonArray() };
    }

    private static JsonObject TextContent(string role, string text) =>
        new() { ["role"] = role, ["parts"] = new JsonArray(new JsonObject { ["text"] = text }) };

    private static List<AiCardDto> FinalizeCards(List<AiCardDto> cards) =>
        cards.DistinctBy(c => $"{c.Type}|{c.Link}").Take(MaxCards).ToList();

    private async Task<string> BuildSystemPromptAsync(
        int? userId,
        CancellationToken cancellationToken
    )
    {
        var settings = await db.CinemaSettings.FirstOrDefaultAsync(
            s => s.Id == AppDbContextExtensions.CinemaSettingsId,
            cancellationToken
        );
        var cinemaName = settings?.Name ?? _brand.Name;
        var address = settings?.Address ?? "Chưa cập nhật";
        var phone = settings?.Phone ?? "Chưa cập nhật";
        var opening = settings?.OpeningTime?.ToString("HH:mm") ?? "08:00";
        var closing = settings?.ClosingTime?.ToString("HH:mm") ?? "23:30";

        var now = DateTime.UtcNow;

        var movies = await db
            .Movies.Include(m => m.AgeRating)
            .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
            .Where(m => m.Status == MovieStatus.NowShowing || m.Status == MovieStatus.ComingSoon)
            .ToListAsync(cancellationToken);

        var combos = await db.Combos.Where(c => c.IsActive).ToListAsync(cancellationToken);

        string userInfoStr = "Khách vãng lai (Chưa đăng nhập)";
        if (userId.HasValue)
        {
            var user = await db
                .Users.Include(u => u.CustomerProfile)
                    .ThenInclude(cp => cp!.Tier)
                .FirstOrDefaultAsync(u => u.UserId == userId.Value, cancellationToken);
            if (user is not null)
            {
                var standing = await LoyaltyWindow.GetStandingAsync(
                    db,
                    user.UserId,
                    DateTime.UtcNow,
                    cancellationToken
                );
                var points = standing.Points;
                var tier = standing.Tier?.TierName ?? "Thành viên";
                userInfoStr =
                    $"Tên: {user.FullName}, Email: {user.Email}, Điểm tích lũy: {points}, Hạng: {tier}";
            }
        }

        var movieInfoLines = movies.Select(m =>
            $"- [{m.MovieId}] {m.Title} ({m.Subtitle ?? "Phụ đề"}) | Thể loại: {string.Join(", ", m.MovieGenres.Select(g => g.Genre.Name))} | Thời lượng: {m.DurationMinutes}p | Độ tuổi: {m.AgeRating?.Code ?? "P"} | Trạng thái: {(m.Status == MovieStatus.NowShowing ? "Đang chiếu" : "Sắp chiếu")}"
        );

        var comboLines = combos.Select(c => $"- {c.Name}: {c.Price:N0} VNĐ ({c.Description})");

        var loginRule = userId.HasValue
            ? "Khách đã đăng nhập: khi hỏi về vé/đơn đã đặt, hãy gọi hàm get_my_bookings."
            : "Khách CHƯA đăng nhập: bạn không có quyền xem vé đã đặt, hãy mời khách đăng nhập để xem vé.";

        return $"""
            Bạn là Trợ lý AI thông minh và thân thiện của Rạp chiếu phim {cinemaName}.
            Nhiệm vụ của bạn là hỗ trợ khách hàng tư vấn phim, lịch chiếu, giá vé, tình trạng ghế, combo bắp nước, và thông tin rạp.

            === DỮ LIỆU THỜI GIAN THỰC TỪ HỆ THỐNG DB RẠP CHÍNH THỨC ===
            - Thời gian hiện tại: {VietnamTime.ToLocal(now):dd/MM/yyyy HH:mm:ss}
            - Thông tin Rạp: {cinemaName} | Địa chỉ: {address} | Hotline: {phone} | Giờ mở/đóng cửa: {opening} - {closing}
            - Thông tin khách hàng đang trò chuyện: {userInfoStr}

            --- DANH SÁCH PHIM DỰ KIẾN / ĐANG CHIẾU (số trong [] là movie_id) ---
            {string.Join("\n", movieInfoLines)}

            --- DANH SÁCH COMBO BẮP NƯỚC ---
            {string.Join("\n", comboLines)}
            ==========================================================

            QUY TẮC PHẢN HỒI:
            1. Luôn trả lời lịch sự, thân thiện bằng tiếng Việt, xưng "Trợ lý {_brand.Name}" hoặc "Em", gọi khách là "Anh/Chị" hoặc "Bạn".
            2. Chỉ tư vấn dựa trên thông tin chính xác từ dữ liệu hệ thống ở trên và kết quả các hàm. Nếu khách hỏi thông tin không có, hãy trả lời khéo léo, tuyệt đối không bịa.
            3. Trình bày phản hồi đẹp mắt bằng Markdown (dùng in đậm **, danh sách đầu dòng -).
            4. Khuyến khích khách hàng chọn suất chiếu và đặt vé trên ứng dụng.

            QUY TẮC DÙNG HÀM (FUNCTION CALLING):
            5. Lịch chiếu, giờ chiếu, giá vé: PHẢI gọi search_showtimes, không tự đoán. Khi khách nói "hôm nay/ngày mai/cuối tuần", hãy đổi sang ngày yyyy-MM-dd dựa trên thời gian hiện tại ở trên.
            6. Tình trạng ghế/còn chỗ không: PHẢI gọi get_seat_availability với showtime_id lấy từ search_showtimes (nếu chưa có thì gọi search_showtimes trước).
            7. {loginRule}
            8. Các thẻ suất chiếu/vé sẽ tự hiển thị bên dưới câu trả lời, nên KHÔNG cần chép lại toàn bộ danh sách; chỉ tóm tắt ngắn gọn và nêu điểm đáng chú ý.
            9. Nếu hàm trả về lỗi hoặc không có kết quả, hãy báo lại cho khách một cách tự nhiên và gợi ý lựa chọn khác.
            """;
    }
}

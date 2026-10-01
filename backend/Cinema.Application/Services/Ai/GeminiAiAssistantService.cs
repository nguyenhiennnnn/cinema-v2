using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Cinema.Application.DTOs.Ai;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Ai;

public class GeminiAiAssistantService(
    IAppDbContext db,
    HttpClient httpClient,
    IOptions<GeminiOptions> options,
    ILogger<GeminiAiAssistantService> logger
) : IAiAssistantService
{
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
        var url = $"{_options.BaseUrl.TrimEnd('/')}/{_options.Model}:generateContent?key={apiKey}";

        var contents = new List<object>();

        if (request.History is not null)
        {
            foreach (var h in request.History)
            {
                var role = h.Role.ToLowerInvariant() switch
                {
                    "user" => "user",
                    _ => "model",
                };
                contents.Add(new { role, parts = new[] { new { text = h.Content } } });
            }
        }

        contents.Add(new { role = "user", parts = new[] { new { text = request.Message } } });

        var payload = new
        {
            system_instruction = new { parts = new[] { new { text = systemPrompt } } },
            contents,
            generationConfig = new { temperature = 0.7, maxOutputTokens = 1000 },
        };

        try
        {
            using var response = await httpClient.PostAsJsonAsync(url, payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError(
                    "Gemini API error ({StatusCode}): {Body}",
                    response.StatusCode,
                    errorBody
                );
                return new AiChatResponse(
                    "Rất tiếc, kết nối với trợ lý AI Gemini thất bại. Vui lòng thử lại sau.",
                    DateTime.UtcNow
                );
            }

            var result = await response.Content.ReadFromJsonAsync<GeminiApiResponse>(
                cancellationToken: cancellationToken
            );

            var replyText = result
                ?.Candidates?.FirstOrDefault()
                ?.Content?.Parts?.FirstOrDefault()
                ?.Text;

            if (string.IsNullOrWhiteSpace(replyText))
            {
                replyText = "Trợ lý AI không tạo được câu trả lời phù hợp. Vui lòng thử lại.";
            }

            return new AiChatResponse(replyText, DateTime.UtcNow);
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

    private async Task<string> BuildSystemPromptAsync(
        int? userId,
        CancellationToken cancellationToken
    )
    {
        var settings = await db.CinemaSettings.FirstOrDefaultAsync(
            s => s.Id == 1,
            cancellationToken
        );
        var cinemaName = settings?.Name ?? "Cinema System";
        var address = settings?.Address ?? "Chưa cập nhật";
        var phone = settings?.Phone ?? "Chưa cập nhật";
        var opening = settings?.OpeningTime?.ToString("HH:mm") ?? "08:00";
        var closing = settings?.ClosingTime?.ToString("HH:mm") ?? "23:30";

        var now = DateTime.UtcNow;
        var endSearch = now.AddDays(3);

        var movies = await db
            .Movies.Include(m => m.AgeRating)
            .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
            .Where(m => m.Status == MovieStatus.NowShowing || m.Status == MovieStatus.ComingSoon)
            .ToListAsync(cancellationToken);

        var showtimes = await db
            .Showtimes.Include(s => s.Movie)
            .Include(s => s.Room)
            .Where(s =>
                s.Status == ShowtimeStatus.Scheduled
                && s.StartTime >= now
                && s.StartTime <= endSearch
            )
            .OrderBy(s => s.StartTime)
            .Take(20)
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
                var points = user.CustomerProfile?.Points ?? 0;
                var tier = user.CustomerProfile?.Tier?.TierName ?? "Thành viên";
                userInfoStr =
                    $"Tên: {user.FullName}, Email: {user.Email}, Điểm tích lũy: {points}, Hạng: {tier}";
            }
        }

        var movieInfoLines = movies.Select(m =>
            $"- [{m.MovieId}] {m.Title} ({m.Subtitle ?? "Phụ đề"}) | Thể loại: {string.Join(", ", m.MovieGenres.Select(g => g.Genre.Name))} | Thời lượng: {m.DurationMinutes}p | Độ tuổi: {m.AgeRating?.Code ?? "P"} | Trạng thái: {(m.Status == MovieStatus.NowShowing ? "Đang chiếu" : "Sắp chiếu")}"
        );

        var showtimeLines = showtimes.Select(s =>
            $"- Suất chiếu ID {s.ShowtimeId}: Phim '{s.Movie.Title}' | Phòng '{s.Room.RoomName}' | Bắt đầu: {s.StartTime:dd/MM/yyyy HH:mm} | Giá vé gốc: {s.BasePrice:N0} VNĐ"
        );

        var comboLines = combos.Select(c => $"- {c.Name}: {c.Price:N0} VNĐ ({c.Description})");

        return $"""
            Bạn là Trợ lý AI thông minh và thân thiện của Rạp chiếu phim {cinemaName}.
            Nhiệm vụ của bạn là hỗ trợ khách hàng tư vấn phim, lịch chiếu, giá vé, combo bắp nước, và thông tin rạp.

            === DỮ LIỆU THỜI GIAN THỰC TỪ HỆ THỐNG DB RẠP CHÍNH THỨC ===
            - Thời gian hiện tại: {now:dd/MM/yyyy HH:mm:ss}
            - Thông tin Rạp: {cinemaName} | Địa chỉ: {address} | Hotline: {phone} | Giờ mở/đóng cửa: {opening} - {closing}
            - Thông tin khách hàng đang trò chuyện: {userInfoStr}

            --- DANH SÁCH PHIM DỰ KIẾN / ĐANG CHIẾU ---
            {string.Join("\n", movieInfoLines)}

            --- LỊCH CHIẾU MỚI NHẤT (3 NGÀY TỚI) ---
            {string.Join("\n", showtimeLines)}

            --- DANH SÁCH COMBO BẮP NƯỚC ---
            {string.Join("\n", comboLines)}
            ==========================================================

            QUY TẮC PHẢN HỒI:
            1. Luôn trả lời lịch sự, thân thiện bằng tiếng Việt, xưng "Trợ lý Cinema" hoặc "Em", gọi khách là "Anh/Chị" hoặc "Bạn".
            2. Chỉ tư vấn dựa trên thông tin chính xác từ dữ liệu hệ thống ở trên. Nếu khách hỏi thông tin không có trong danh sách, hãy trả lời khéo léo.
            3. Trình bày phản hồi đẹp mắt bằng Markdown (dùng in đậm **, danh sách đầu dòng -).
            4. Khuyến khích khách hàng chọn suất chiếu và đặt vé trên ứng dụng.
            """;
    }

    private record GeminiApiResponse(
        [property: JsonPropertyName("candidates")] List<GeminiCandidate>? Candidates
    );

    private record GeminiCandidate([property: JsonPropertyName("content")] GeminiContent? Content);

    private record GeminiContent([property: JsonPropertyName("parts")] List<GeminiPart>? Parts);

    private record GeminiPart([property: JsonPropertyName("text")] string? Text);
}

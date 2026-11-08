using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Ai;

public class AiSystemPromptBuilder(
    IAppDbContext db,
    IOptions<BrandOptions>? brandOptions = null
)
{
    private readonly BrandOptions _brand = brandOptions?.Value ?? new();

    public async Task<string> BuildSystemPromptAsync(
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

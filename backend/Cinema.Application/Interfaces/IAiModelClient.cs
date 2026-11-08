using Cinema.Application.Services.Ai;

namespace Cinema.Application.Interfaces;

/// <summary>
/// Cổng giao tiếp với mô hình ngôn ngữ. Application chỉ biết cổng này;
/// nhà cung cấp cụ thể chỉ là một cài đặt cắm vào ở Infrastructure.
/// </summary>
public interface IAiModelClient
{
    /// <summary>Đã có đủ cấu hình (API key, ...) để gọi mô hình hay chưa.</summary>
    bool IsConfigured { get; }

    /// <summary>
    /// Gửi một lượt hội thoại và nhận về tin nhắn của mô hình.
    /// Ném <see cref="AiModelException"/> khi nhà cung cấp trả lỗi.
    /// </summary>
    Task<AiMessage> GenerateAsync(AiModelRequest request, CancellationToken cancellationToken);
}

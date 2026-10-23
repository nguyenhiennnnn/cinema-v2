using System.Net;

namespace Cinema.Application.Utils;

public static class EmailTemplates
{
    private const string ColorBackground = "#f7f2e9";
    private const string ColorCard = "#fffdfa";
    private const string ColorForeground = "#20140f";
    private const string ColorMuted = "#6f6259";
    private const string ColorBorder = "#e2d3c2";
    private const string ColorPrimary = "#7a1420";
    private const string ColorPrimaryHover = "#671019";
    private const string ColorPrimaryForeground = "#fbf7f1";
    private const string ColorAccent = "#b3823c";
    private const string ColorAccentForeground = "#2b1c12";
    private const string ColorSuccess = "#256b41";
    private const string ColorSuccessBg = "#e9f4ee";
    private const string ColorSuccessBorder = "#c6e0d0";
    private const string ColorDestructive = "#b8241f";
    private const string ColorDestructiveBg = "#fbe9e7";
    private const string ColorDestructiveBorder = "#f0c7c2";
    private const string ColorWarningBg = "#f8ecd8";
    private const string ColorWarningBorder = "#ecd6ab";

    private const string FontFamily = "'Geist', 'Segoe UI', Helvetica, Arial, sans-serif";
    private const string FontHeading = "'Oswald', 'Arial Narrow', Arial, sans-serif";

    private const string BrandName = "PERFO";

    private static string Layout(string preheader, string bodyHtml)
    {
        var year = DateTime.UtcNow.Year;

        return $"""
            <!DOCTYPE html>
            <html lang="vi">
              <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                <title>{BrandName}</title>
                <!--[if !mso]><!-->
                <link href="https://fonts.googleapis.com/css2?family=Oswald:wght@500;600;700&display=swap" rel="stylesheet" />
                <!--<![endif]-->
              </head>
              <body style="margin:0; padding:0; background-color:{ColorBackground}; font-family:{FontFamily}; color:{ColorForeground};">
                <span style="display:none; max-height:0; overflow:hidden; opacity:0;">{WebUtility.HtmlEncode(
                preheader
            )}</span>
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background-color:{ColorBackground}; padding:32px 16px;">
                  <tr>
                    <td align="center">
                      <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="max-width:560px;">
                        <tr>
                          <td style="padding:0 8px 18px 8px;">
                            <table role="presentation" cellpadding="0" cellspacing="0">
                              <tr>
                                <td style="width:30px; height:30px; text-align:center; vertical-align:middle;">
                                  <svg width="30" height="30" viewBox="0 0 48 48" xmlns="http://www.w3.org/2000/svg">
                                    <rect width="48" height="48" rx="7" fill="{ColorPrimary}" />
                                    <circle cx="0" cy="24" r="5" fill="{ColorCard}" />
                                    <circle cx="48" cy="24" r="5" fill="{ColorCard}" />
                                    <path d="M15.8 5v3.4M32.2 5v3.4M15.8 39.6V43M32.2 39.6V43" stroke="{ColorAccent}" stroke-width="2.2" stroke-linecap="round" />
                                    <circle cx="24" cy="24" r="9.5" fill="{ColorPrimaryForeground}" />
                                    <path d="M21.2 19.6c0-.86.94-1.39 1.67-.94l7.36 4.4a1.1 1.1 0 0 1 0 1.88l-7.36 4.4a1.1 1.1 0 0 1-1.67-.94V19.6Z" fill="{ColorAccent}" />
                                  </svg>
                                </td>
                                <td style="padding-left:10px; font-family:{FontHeading}; font-size:20px; font-weight:700; letter-spacing:0.04em; color:{ColorForeground}; text-transform:uppercase;">{BrandName}</td>
                              </tr>
                            </table>
                          </td>
                        </tr>

                        <!-- dải lỗ phim — mô phỏng sprocket-rule của web -->
                        <tr>
                          <td style="padding:0 8px;">
                            <div style="border-top:2px dashed {ColorBorder}; font-size:0; line-height:0;">&nbsp;</div>
                          </td>
                        </tr>

                        <tr>
                          <td style="padding-top:20px;">
                            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background-color:{ColorCard}; border:1px solid {ColorBorder}; border-top:3px solid {ColorPrimary};">
                              <tr>
                                <td style="padding:32px 28px;">
                                  {bodyHtml}
                                </td>
                              </tr>
                            </table>
                          </td>
                        </tr>

                        <tr>
                          <td style="padding:22px 8px 0 8px; text-align:center; font-family:{FontHeading}; font-size:11px; font-weight:500; letter-spacing:0.08em; color:{ColorMuted}; text-transform:uppercase;">
                            Vé điện tử &middot; Không cần in
                          </td>
                        </tr>
                        <tr>
                          <td style="padding:10px 8px 0 8px; text-align:center; font-size:12px; line-height:1.6; color:{ColorMuted};">
                            Email này được gửi tự động từ hệ thống {BrandName}, vui lòng không trả lời trực tiếp email này.<br />
                            &copy; {year} {BrandName}. Đã đăng ký bản quyền.
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
              </body>
            </html>
            """;
    }

    private static string Heading(string text) =>
        $"""<h1 style="margin:0 0 16px 0; font-family:{FontHeading}; font-size:22px; line-height:1.3; font-weight:600; letter-spacing:-0.01em; color:{ColorForeground}; text-transform:uppercase;">{text}</h1>""";

    private static string Paragraph(string html) =>
        $"""<p style="margin:0 0 16px 0; font-size:15px; line-height:1.65; color:{ColorForeground};">{html}</p>""";

    private static string MutedParagraph(string html) =>
        $"""<p style="margin:0; font-size:13px; line-height:1.6; color:{ColorMuted};">{html}</p>""";

    private static string Button(string label, string url) =>
        $"""
            <table role="presentation" cellpadding="0" cellspacing="0" style="margin:8px 0 22px 0;">
              <tr>
                <td style="background-color:{ColorPrimary};">
                  <a href="{url}" target="_blank" style="display:inline-block; padding:13px 30px; font-family:{FontHeading}; font-size:14px; font-weight:600; letter-spacing:0.06em; color:{ColorPrimaryForeground}; text-decoration:none; text-transform:uppercase;">{label}</a>
                </td>
              </tr>
            </table>
            """;

    private static string Badge(string text, string color, string bgColor, string borderColor) =>
        $"""<div style="display:inline-block; padding:4px 13px; border-radius:999px; border:1px solid {borderColor}; background-color:{bgColor}; color:{color}; font-family:{FontHeading}; font-size:11px; font-weight:600; letter-spacing:0.06em; text-transform:uppercase; margin-bottom:16px;">{text}</div>""";

    private static string Callout(string html, string bgColor, string borderColor) =>
        $"""
            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background-color:{bgColor}; border:1px solid {borderColor}; border-left:3px solid {borderColor}; margin:0 0 20px 0;">
              <tr>
                <td style="padding:14px 16px; font-size:14px; line-height:1.6; color:{ColorForeground};">{html}</td>
              </tr>
            </table>
            """;

    private static string DetailRow(string label, string value, bool isLast = false) =>
        $"""
            <tr>
              <td style="padding:11px 0; {(
                isLast ? "" : $"border-bottom:1px dashed {ColorBorder};"
            )} font-family:{FontHeading}; font-size:11px; font-weight:500; letter-spacing:0.04em; color:{ColorMuted}; text-transform:uppercase;">{label}</td>
              <td style="padding:11px 0; {(
                isLast ? "" : $"border-bottom:1px dashed {ColorBorder};"
            )} font-size:14px; font-weight:600; color:{ColorForeground}; text-align:right;">{value}</td>
            </tr>
            """;

    private static string DetailRowEmphasis(string label, string value) =>
        $"""
            <tr>
              <td style="padding:12px 0 2px 0; font-family:{FontHeading}; font-size:12px; font-weight:600; letter-spacing:0.04em; color:{ColorForeground}; text-transform:uppercase;">{label}</td>
              <td style="padding:12px 0 2px 0; font-family:{FontHeading}; font-size:19px; font-weight:700; color:{ColorPrimary}; text-align:right;">{value}</td>
            </tr>
            """;

    private static string DetailCard(string bgColor, string borderColor, params string[] rows) =>
        $"""
            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background-color:{bgColor}; border:1px solid {borderColor}; padding:2px 16px; margin:4px 0 20px 0;">
              {string.Concat(rows)}
            </table>
            """;

    public static string ActionLink(
        string fullName,
        string title,
        string introText,
        string buttonLabel,
        string link,
        string expiresIn
    )
    {
        var body = $"""
            {Heading(title)}
            {Paragraph($"Xin chào <strong>{WebUtility.HtmlEncode(fullName)}</strong>,")}
            {Paragraph(introText)}
            {Button(buttonLabel, link)}
            {MutedParagraph(
                $"Liên kết sẽ hết hạn sau <strong>{expiresIn}</strong>. Nếu không phải bạn yêu cầu, vui lòng bỏ qua email này."
            )}
            """;

        return Layout(title, body);
    }

    public static string BookingConfirmed(
        string customerName,
        string bookingCode,
        string movieTitle,
        int seatCount
    )
    {
        var rows = new List<string> { DetailRow("Mã đặt vé", bookingCode) };
        if (!string.IsNullOrWhiteSpace(movieTitle))
            rows.Add(DetailRow("Phim", WebUtility.HtmlEncode(movieTitle)));
        rows.Add(DetailRow("Số ghế", $"{seatCount} ghế", isLast: true));

        var body = $"""
            {Badge("Đặt vé thành công", ColorSuccess, ColorSuccessBg, ColorSuccessBorder)}
            {Heading($"Cảm ơn bạn, {WebUtility.HtmlEncode(customerName)}!")}
            {Paragraph(
                "Vé của bạn đã được xác nhận và sẵn sàng sử dụng. Vui lòng vào mục <strong>\"Vé của tôi\"</strong> trên hệ thống để xem chi tiết và mã QR check-in tại rạp."
            )}
            {DetailCard(ColorSuccessBg, ColorSuccessBorder, [.. rows])}
            {MutedParagraph(
                "Vui lòng có mặt trước giờ chiếu ít nhất 15 phút và xuất trình mã QR tại quầy soát vé."
            )}
            """;

        return Layout($"Vé xem phim {bookingCode} đã được xác nhận", body);
    }

    public static string BookingCancelledRefunded(
        string customerName,
        string bookingCode,
        decimal refundAmount
    )
    {
        var body = $"""
            {Badge("Đã hủy vé", ColorAccentForeground, ColorWarningBg, ColorWarningBorder)}
            {Heading($"Booking của {WebUtility.HtmlEncode(customerName)} đã được hủy")}
            {Paragraph($"Booking mã <strong>{bookingCode}</strong> đã được hủy thành công theo yêu cầu của bạn.")}
            {DetailCard(ColorBackground, ColorBorder,
                DetailRow("Mã đặt vé", bookingCode),
                DetailRowEmphasis("Số tiền hoàn lại", $"{refundAmount:N0} VNĐ"))}
            {MutedParagraph("Khoản hoàn tiền sẽ được chuyển về tài khoản/thẻ đã thanh toán trong vài ngày làm việc, tùy thời gian xử lý của ngân hàng.")}
            """;

        return Layout($"Booking {bookingCode} đã hủy — hoàn {refundAmount:N0} VNĐ", body);
    }

    public static string ShowtimeCancelledAutoRefunded(
        string customerName,
        string bookingCode,
        decimal refundAmount
    )
    {
        var body = $"""
            {Badge("Suất chiếu đã bị hủy", ColorAccentForeground, ColorWarningBg, ColorWarningBorder)}
            {Heading($"Rất tiếc, {WebUtility.HtmlEncode(customerName)}")}
            {Paragraph(
                $"Suất chiếu trong booking mã <strong>{bookingCode}</strong> của bạn đã bị rạp hủy. Chúng tôi thành thật xin lỗi vì sự bất tiện này và đã hoàn lại 100% số tiền vào phương thức thanh toán bạn đã sử dụng."
            )}
            {DetailCard(ColorBackground, ColorBorder,
                DetailRow("Mã đặt vé", bookingCode),
                DetailRowEmphasis("Số tiền đã hoàn", $"{refundAmount:N0} VNĐ"))}
            {MutedParagraph("Khoản hoàn tiền sẽ được chuyển về tài khoản/thẻ đã thanh toán trong vài ngày làm việc, tùy thời gian xử lý của ngân hàng/cổng thanh toán.")}
            """;

        return Layout($"Suất chiếu {bookingCode} đã bị hủy — đã hoàn {refundAmount:N0} VNĐ", body);
    }

    public static string ShowtimeCancelledManualRefund(
        string customerName,
        string bookingCode,
        decimal amountDue
    )
    {
        var body = $"""
            {Badge("Suất chiếu đã bị hủy", ColorAccentForeground, ColorWarningBg, ColorWarningBorder)}
            {Heading($"Rất tiếc, {WebUtility.HtmlEncode(customerName)}")}
            {Paragraph(
                $"Suất chiếu trong booking mã <strong>{bookingCode}</strong> của bạn đã bị rạp hủy. Chúng tôi thành thật xin lỗi vì sự bất tiện này."
            )}
            {DetailCard(ColorBackground, ColorBorder,
                DetailRow("Mã đặt vé", bookingCode),
                DetailRowEmphasis("Số tiền cần hoàn", $"{amountDue:N0} VNĐ"))}
            {MutedParagraph("Giao dịch của bạn cần được nhân viên rạp hỗ trợ hoàn tiền thủ công. Vui lòng mang theo mã đặt vé đến quầy hoặc liên hệ hotline rạp để được hoàn tiền trong thời gian sớm nhất.")}
            """;

        return Layout($"Suất chiếu {bookingCode} đã bị hủy — cần hoàn {amountDue:N0} VNĐ", body);
    }

    public static string SeatsExpiredAfterPayment(
        bool autoRefunded,
        int bookingId,
        int paymentId,
        decimal amount
    )
    {
        var rows = new[]
        {
            DetailRow("Booking", $"#{bookingId}"),
            DetailRow("Payment", $"#{paymentId}"),
            DetailRowEmphasis(
                autoRefunded ? "Số tiền đã hoàn" : "Số tiền cần hoàn",
                $"{amount:N0} VNĐ"
            ),
        };

        var body = autoRefunded
            ? $"""
                {Badge("Đã tự động hoàn tiền", ColorSuccess, ColorSuccessBg, ColorSuccessBorder)}
                {Heading("Ghế hết hạn giữ ngay sau khi thanh toán")}
                {Paragraph(
                    "Booking đã thanh toán thành công nhưng ghế bị hệ thống nhả do hết hạn giữ chỗ đúng thời điểm đó."
                )}
                {Paragraph(
                    "Hệ thống đã <strong>tự động hoàn tiền</strong> cho khách qua cổng thanh toán. Vui lòng kiểm tra lại giao dịch hoàn tiền để chắc chắn."
                )}
                {DetailCard(ColorSuccessBg, ColorSuccessBorder, [.. rows])}
                """
            : $"""
                {Badge("Cần xử lý thủ công", ColorDestructive, ColorDestructiveBg, ColorDestructiveBorder)}
                {Heading("Ghế hết hạn giữ ngay sau khi thanh toán")}
                {Paragraph(
                    "Booking đã thanh toán thành công nhưng ghế bị hệ thống nhả do hết hạn giữ chỗ đúng thời điểm đó."
                )}
                {Callout(
                    "<strong>Hoàn tiền tự động qua cổng thanh toán thất bại hoặc không khả dụng.</strong> Vui lòng hoàn tiền thủ công cho khách.",
                    ColorDestructiveBg,
                    ColorDestructiveBorder
                )}
                {DetailCard(ColorBackground, ColorBorder, [.. rows])}
                """;

        return Layout(
            autoRefunded
                ? $"Đã tự động hoàn tiền — Booking #{bookingId}"
                : $"CẦN HOÀN TIỀN THỦ CÔNG — Booking #{bookingId}",
            body
        );
    }
}

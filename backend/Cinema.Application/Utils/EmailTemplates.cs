using System.Net;
using Cinema.Application.Options;

namespace Cinema.Application.Utils;

public static class EmailTemplates
{
    private const string ColorBackground = "#f8f3eb";
    private const string ColorCard = "#fefbf7";
    private const string ColorForeground = "#1e1311";
    private const string ColorMuted = "#6a5a51";
    private const string ColorBorder = "#dad3ca";
    private const string ColorPrimary = "#a50013";
    private const string ColorPrimaryForeground = "#fdfaf4";
    private const string ColorSuccess = "#187c49";
    private const string ColorSuccessBg = "#e7eee6";
    private const string ColorSuccessBorder = "#b9d5c3";
    private const string ColorDestructive = "#d41101";
    private const string ColorDestructiveBg = "#fae4de";
    private const string ColorDestructiveBorder = "#f1b5ad";
    private const string ColorWarningForeground = "#211201";
    private const string ColorWarningBg = "#faf0df";
    private const string ColorWarningBorder = "#f3d9b0";

    private const string FontFamily = "'Be Vietnam Pro', 'Segoe UI', Helvetica, Arial, sans-serif";
    private const string FontHeading = "'Archivo', 'Arial Narrow', Arial, sans-serif";
    private const string FontMono = "ui-monospace, 'SFMono-Regular', Menlo, Consolas, monospace";

    private sealed record Tone(string Fg, string Bg, string Border, string Css);

    private static readonly Tone Success =
        new(ColorSuccess, ColorSuccessBg, ColorSuccessBorder, "ok");
    private static readonly Tone Danger =
        new(ColorDestructive, ColorDestructiveBg, ColorDestructiveBorder, "err");
    private static readonly Tone Warning =
        new(ColorWarningForeground, ColorWarningBg, ColorWarningBorder, "warn");
    private static readonly Tone Neutral =
        new(ColorForeground, ColorBackground, ColorBorder, "neutral");

    private static string Layout(
        BrandOptions brand,
        string preheader,
        string kicker,
        string bodyHtml
    )
    {
        var year = DateTime.UtcNow.Year;
        var support = string.IsNullOrWhiteSpace(brand.SupportEmail)
            ? string.Empty
            : $"<br />Cần hỗ trợ? Liên hệ {WebUtility.HtmlEncode(brand.SupportEmail)}.";

        return $$"""
            <!DOCTYPE html>
            <html lang="vi">
              <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                <meta name="color-scheme" content="light dark" />
                <meta name="supported-color-schemes" content="light dark" />
                <title>{{brand.DisplayName}}</title>
                <!--[if !mso]><!-->
                <link href="https://fonts.googleapis.com/css2?family=Archivo:wght@500;600;700&family=Be+Vietnam+Pro:wght@400;500;600&display=swap" rel="stylesheet" />
                <!--<![endif]-->
                <style>
                  @media (prefers-color-scheme: dark) {
                    .bg-page { background-color:#0a0404 !important; }
                    .bg-card { background-color:#1a0e0c !important; }
                    .bg-pri { background-color:#d4212d !important; }
                    .tx { color:#f2eee7 !important; }
                    .tx-mut { color:#a89c92 !important; }
                    .tx-pri { color:#d4212d !important; }
                    .bd { border-color:#322523 !important; }
                    .tone-ok { background-color:#1d261a !important; border-color:#22472e !important; color:#2fa465 !important; }
                    .tone-err { background-color:#3c1510 !important; border-color:#6a1e16 !important; color:#ed3726 !important; }
                    .tone-warn { background-color:#3b2410 !important; border-color:#684216 !important; color:#e69825 !important; }
                    .tone-neutral { background-color:#0a0404 !important; border-color:#322523 !important; color:#f2eee7 !important; }
                    .stripe-err { border-left-color:#ed3726 !important; }
                    .stripe-ok { border-left-color:#2fa465 !important; }
                    .stripe-warn { border-left-color:#e69825 !important; }
                  }
                </style>
              </head>
              <body class="bg-page tx" style="margin:0; padding:0; background-color:{{ColorBackground}}; font-family:{{FontFamily}}; color:{{ColorForeground}};">
                <span style="display:none; max-height:0; overflow:hidden; opacity:0;">{{WebUtility.HtmlEncode(preheader)}}</span>
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" class="bg-page" style="background-color:{{ColorBackground}}; padding:32px 16px;">
                  <tr>
                    <td align="center">
                      <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="max-width:560px;">
                        <tr>
                          <td style="padding:0 0 18px 0;">
                            <table role="presentation" width="100%" cellpadding="0" cellspacing="0">
                              <tr>
                                <td class="tx" style="font-family:{{FontHeading}}; font-size:22px; font-weight:700; letter-spacing:0.02em; line-height:1; color:{{ColorForeground}};">{{brand.DisplayName}}<span class="bg-pri" style="display:inline-block; width:7px; height:7px; border-radius:50%; background-color:{{ColorPrimary}}; margin-left:3px;"></span></td>
                              </tr>
                            </table>
                          </td>
                        </tr>

                        <tr>
                          <td>
                            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" class="bg-card bd" style="background-color:{{ColorCard}}; border:1px solid {{ColorBorder}};">
                              <tr>
                                <td class="bg-pri" style="background-color:{{ColorPrimary}}; padding:20px 28px;">
                                  <span style="font-family:{{FontHeading}}; font-size:12px; font-weight:600; letter-spacing:0.14em; color:{{ColorPrimaryForeground}}; text-transform:uppercase;">{{WebUtility.HtmlEncode(kicker)}}</span>
                                </td>
                              </tr>
                              <tr>
                                <td style="padding:0;">
                                  <table role="presentation" width="100%" cellpadding="0" cellspacing="0">
                                    <tr>
                                      <td class="bg-page" width="10" style="width:10px; height:20px; background-color:{{ColorBackground}}; border-radius:0 10px 10px 0; font-size:0; line-height:0;">&nbsp;</td>
                                      <td style="height:20px; padding:0 10px; vertical-align:middle;">
                                        <div class="bd" style="border-top:2px dashed {{ColorBorder}}; font-size:0; line-height:0;">&nbsp;</div>
                                      </td>
                                      <td class="bg-page" width="10" style="width:10px; height:20px; background-color:{{ColorBackground}}; border-radius:10px 0 0 10px; font-size:0; line-height:0;">&nbsp;</td>
                                    </tr>
                                  </table>
                                </td>
                              </tr>
                              <tr>
                                <td style="padding:20px 28px 32px 28px;">
                                  {{bodyHtml}}
                                </td>
                              </tr>
                            </table>
                          </td>
                        </tr>

                        <tr>
                          <td class="tx-mut" style="padding:22px 8px 0 8px; text-align:center; font-family:{{FontHeading}}; font-size:11px; font-weight:600; letter-spacing:0.1em; color:{{ColorMuted}}; text-transform:uppercase;">
                            {{WebUtility.HtmlEncode(brand.Tagline)}}
                          </td>
                        </tr>
                        <tr>
                          <td class="tx-mut" style="padding:10px 8px 0 8px; text-align:center; font-size:12px; line-height:1.6; color:{{ColorMuted}};">
                            Email này được gửi tự động từ hệ thống {{brand.DisplayName}}, vui lòng không trả lời trực tiếp email này.{{support}}<br />
                            &copy; {{year}} {{brand.DisplayName}}. Đã đăng ký bản quyền.
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
        $"""<h1 class="tx" style="margin:0 0 16px 0; font-family:{FontHeading}; font-size:24px; line-height:1.25; font-weight:600; letter-spacing:-0.01em; color:{ColorForeground};">{text}</h1>""";

    private static string Paragraph(string html) =>
        $"""<p class="tx" style="margin:0 0 16px 0; font-size:15px; line-height:1.65; color:{ColorForeground};">{html}</p>""";

    private static string MutedParagraph(string html) =>
        $"""<p class="tx-mut" style="margin:0; font-size:13px; line-height:1.6; color:{ColorMuted};">{html}</p>""";

    private static string Button(string label, string url) =>
        $"""
            <table role="presentation" cellpadding="0" cellspacing="0" style="margin:8px 0 18px 0;">
              <tr>
                <td class="bg-pri" style="background-color:{ColorPrimary};">
                  <a href="{WebUtility.HtmlEncode(url)}" target="_blank" style="display:inline-block; padding:14px 32px; font-family:{FontHeading}; font-size:14px; font-weight:600; letter-spacing:0.08em; color:{ColorPrimaryForeground}; text-decoration:none; text-transform:uppercase;">{label}</a>
                </td>
              </tr>
            </table>
            <p class="tx-mut" style="margin:0 0 20px 0; font-size:12px; line-height:1.6; color:{ColorMuted}; word-break:break-all;">Nếu nút không hoạt động, hãy sao chép liên kết sau vào trình duyệt:<br /><a href="{WebUtility.HtmlEncode(url)}" target="_blank" class="tx-pri" style="color:{ColorPrimary};">{WebUtility.HtmlEncode(url)}</a></p>
            """;

    private static string Badge(string text, Tone tone) =>
        $"""<div class="tone-{tone.Css}" style="display:inline-block; padding:5px 12px; border:1px solid {tone.Border}; background-color:{tone.Bg}; color:{tone.Fg}; font-family:{FontHeading}; font-size:11px; font-weight:600; letter-spacing:0.1em; text-transform:uppercase; margin-bottom:16px;">{text}</div>""";

    private static string Callout(string html, Tone tone) =>
        $"""
            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" class="tone-{tone.Css} stripe-{tone.Css}" style="background-color:{tone.Bg}; border:1px solid {tone.Border}; border-left:3px solid {tone.Fg}; margin:0 0 20px 0;">
              <tr>
                <td class="tx" style="padding:14px 16px; font-size:14px; line-height:1.6; color:{ColorForeground};">{html}</td>
              </tr>
            </table>
            """;

    private static string DetailRow(string label, string value, bool isLast = false, bool mono = false) =>
        $"""
            <tr>
              <td class="tx-mut bd" style="padding:12px 0; {(
                isLast ? "" : $"border-bottom:1px dashed {ColorBorder};"
            )} font-family:{FontHeading}; font-size:11px; font-weight:600; letter-spacing:0.1em; color:{ColorMuted}; text-transform:uppercase;">{label}</td>
              <td class="tx bd" style="padding:12px 0; {(
                isLast ? "" : $"border-bottom:1px dashed {ColorBorder};"
            )} {(mono ? $"font-family:{FontMono}; letter-spacing:0.04em;" : "")} font-size:14px; font-weight:600; color:{ColorForeground}; text-align:right;">{value}</td>
            </tr>
            """;

    private static string DetailRowEmphasis(string label, string value) =>
        $"""
            <tr>
              <td class="tx" style="padding:14px 0 14px 0; font-family:{FontHeading}; font-size:12px; font-weight:600; letter-spacing:0.1em; color:{ColorForeground}; text-transform:uppercase;">{label}</td>
              <td class="tx-pri" style="padding:14px 0 14px 0; font-family:{FontHeading}; font-size:22px; font-weight:700; color:{ColorPrimary}; text-align:right;">{value}</td>
            </tr>
            """;

    private static string DetailCard(Tone tone, params string[] rows) =>
        $"""
            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" class="tone-{tone.Css}" style="background-color:{tone.Bg}; border:1px solid {tone.Border}; padding:2px 18px; margin:4px 0 20px 0;">
              {string.Concat(rows)}
            </table>
            """;

    public static string ActionLink(
        BrandOptions brand,
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

        return Layout(brand, title, "Tài khoản", body);
    }

    public static string BookingConfirmed(
        BrandOptions brand,
        string customerName,
        string bookingCode,
        string movieTitle,
        int seatCount
    )
    {
        var rows = new List<string> { DetailRow("Mã đặt vé", bookingCode, mono: true) };
        if (!string.IsNullOrWhiteSpace(movieTitle))
            rows.Add(DetailRow("Phim", WebUtility.HtmlEncode(movieTitle)));
        rows.Add(DetailRow("Số ghế", $"{seatCount} ghế", isLast: true));

        var body = $"""
            {Badge("Đặt vé thành công", Success)}
            {Heading($"Cảm ơn bạn, {WebUtility.HtmlEncode(customerName)}!")}
            {Paragraph(
                "Vé của bạn đã được xác nhận và sẵn sàng sử dụng. Vui lòng vào mục <strong>\"Vé của tôi\"</strong> trên hệ thống để xem chi tiết và mã QR check-in tại rạp."
            )}
            {DetailCard(Success, [.. rows])}
            {MutedParagraph(
                "Vui lòng có mặt trước giờ chiếu ít nhất 15 phút và xuất trình mã QR tại quầy soát vé."
            )}
            """;

        return Layout(brand, $"Vé xem phim {bookingCode} đã được xác nhận", "Vé điện tử", body);
    }

    public static string BookingCancelledRefunded(
        BrandOptions brand,
        string customerName,
        string bookingCode,
        decimal refundAmount
    )
    {
        var body = $"""
            {Badge("Đã hủy vé", Warning)}
            {Heading($"Booking của {WebUtility.HtmlEncode(customerName)} đã được hủy")}
            {Paragraph($"Booking mã <strong>{bookingCode}</strong> đã được hủy thành công theo yêu cầu của bạn.")}
            {DetailCard(Neutral,
                DetailRow("Mã đặt vé", bookingCode, mono: true),
                DetailRowEmphasis("Số tiền hoàn lại", $"{refundAmount:N0} VNĐ"))}
            {MutedParagraph("Khoản hoàn tiền sẽ được chuyển về tài khoản/thẻ đã thanh toán trong vài ngày làm việc, tùy thời gian xử lý của ngân hàng.")}
            """;

        return Layout(
            brand,
            $"Booking {bookingCode} đã hủy — hoàn {refundAmount:N0} VNĐ",
            "Thông báo hoàn tiền",
            body
        );
    }

    public static string ShowtimeCancelledAutoRefunded(
        BrandOptions brand,
        string customerName,
        string bookingCode,
        decimal refundAmount
    )
    {
        var body = $"""
            {Badge("Suất chiếu đã bị hủy", Warning)}
            {Heading($"Rất tiếc, {WebUtility.HtmlEncode(customerName)}")}
            {Paragraph(
                $"Suất chiếu trong booking mã <strong>{bookingCode}</strong> của bạn đã bị rạp hủy. Chúng tôi thành thật xin lỗi vì sự bất tiện này và đã hoàn lại 100% số tiền vào phương thức thanh toán bạn đã sử dụng."
            )}
            {DetailCard(Neutral,
                DetailRow("Mã đặt vé", bookingCode, mono: true),
                DetailRowEmphasis("Số tiền đã hoàn", $"{refundAmount:N0} VNĐ"))}
            {MutedParagraph("Khoản hoàn tiền sẽ được chuyển về tài khoản/thẻ đã thanh toán trong vài ngày làm việc, tùy thời gian xử lý của ngân hàng/cổng thanh toán.")}
            """;

        return Layout(
            brand,
            $"Suất chiếu {bookingCode} đã bị hủy — đã hoàn {refundAmount:N0} VNĐ",
            "Thông báo từ rạp",
            body
        );
    }

    public static string ShowtimeCancelledManualRefund(
        BrandOptions brand,
        string customerName,
        string bookingCode,
        decimal amountDue
    )
    {
        var body = $"""
            {Badge("Suất chiếu đã bị hủy", Warning)}
            {Heading($"Rất tiếc, {WebUtility.HtmlEncode(customerName)}")}
            {Paragraph(
                $"Suất chiếu trong booking mã <strong>{bookingCode}</strong> của bạn đã bị rạp hủy. Chúng tôi thành thật xin lỗi vì sự bất tiện này."
            )}
            {DetailCard(Neutral,
                DetailRow("Mã đặt vé", bookingCode, mono: true),
                DetailRowEmphasis("Số tiền cần hoàn", $"{amountDue:N0} VNĐ"))}
            {MutedParagraph("Giao dịch của bạn cần được nhân viên rạp hỗ trợ hoàn tiền thủ công. Vui lòng mang theo mã đặt vé đến quầy hoặc liên hệ hotline rạp để được hoàn tiền trong thời gian sớm nhất.")}
            """;

        return Layout(
            brand,
            $"Suất chiếu {bookingCode} đã bị hủy — cần hoàn {amountDue:N0} VNĐ",
            "Thông báo từ rạp",
            body
        );
    }

    public static string SeatsExpiredAfterPaymentCustomer(
        BrandOptions brand,
        string customerName,
        string bookingCode,
        bool autoRefunded,
        decimal amount
    )
    {
        var rows = new[]
        {
            DetailRow("Mã đặt vé", bookingCode, mono: true),
            DetailRowEmphasis(
                autoRefunded ? "Số tiền đã hoàn" : "Số tiền sẽ được hoàn",
                $"{amount:N0} VNĐ"
            ),
        };

        var body = autoRefunded
            ? $"""
                {Badge("Đã hoàn tiền tự động", Warning)}
                {Heading($"Rất tiếc, {WebUtility.HtmlEncode(customerName)}")}
                {Paragraph(
                    $"Thanh toán cho booking <strong>{bookingCode}</strong> đã thành công nhưng ghế đã hết thời gian giữ chỗ nên chúng tôi không thể xuất vé. Chúng tôi đã tự động hoàn lại toàn bộ số tiền vào phương thức thanh toán bạn đã sử dụng."
                )}
                {DetailCard(Neutral, [.. rows])}
                {MutedParagraph("Khoản hoàn tiền sẽ về tài khoản/thẻ trong vài ngày làm việc, tùy thời gian xử lý của ngân hàng.")}
                """
            : $"""
                {Badge("Chờ hoàn tiền", Warning)}
                {Heading($"Rất tiếc, {WebUtility.HtmlEncode(customerName)}")}
                {Paragraph(
                    $"Thanh toán cho booking <strong>{bookingCode}</strong> đã thành công nhưng ghế đã hết thời gian giữ chỗ nên chúng tôi không thể xuất vé. Rạp sẽ hoàn lại toàn bộ số tiền cho bạn thủ công trong thời gian sớm nhất."
                )}
                {DetailCard(Neutral, [.. rows])}
                {MutedParagraph("Nếu cần hỗ trợ, vui lòng liên hệ trực tiếp rạp và cung cấp mã đặt vé ở trên.")}
                """;

        return Layout(
            brand,
            $"Booking {bookingCode} chưa xuất được vé — {(autoRefunded ? "đã hoàn tiền" : "chờ hoàn tiền")}",
            "Thông báo hoàn tiền",
            body
        );
    }

    public static string SeatsExpiredAfterPayment(
        BrandOptions brand,
        bool autoRefunded,
        int bookingId,
        int paymentId,
        decimal amount
    )
    {
        var rows = new[]
        {
            DetailRow("Booking", $"#{bookingId}", mono: true),
            DetailRow("Payment", $"#{paymentId}", mono: true),
            DetailRowEmphasis(
                autoRefunded ? "Số tiền đã hoàn" : "Số tiền cần hoàn",
                $"{amount:N0} VNĐ"
            ),
        };

        var body = autoRefunded
            ? $"""
                {Badge("Đã tự động hoàn tiền", Success)}
                {Heading("Ghế hết hạn giữ ngay sau khi thanh toán")}
                {Paragraph(
                    "Booking đã thanh toán thành công nhưng ghế bị hệ thống nhả do hết hạn giữ chỗ đúng thời điểm đó."
                )}
                {Paragraph(
                    "Hệ thống đã <strong>tự động hoàn tiền</strong> cho khách qua cổng thanh toán. Vui lòng kiểm tra lại giao dịch hoàn tiền để chắc chắn."
                )}
                {DetailCard(Success, [.. rows])}
                """
            : $"""
                {Badge("Cần xử lý thủ công", Danger)}
                {Heading("Ghế hết hạn giữ ngay sau khi thanh toán")}
                {Paragraph(
                    "Booking đã thanh toán thành công nhưng ghế bị hệ thống nhả do hết hạn giữ chỗ đúng thời điểm đó."
                )}
                {Callout(
                    "<strong>Hoàn tiền tự động qua cổng thanh toán thất bại hoặc không khả dụng.</strong> Vui lòng hoàn tiền thủ công cho khách.",
                    Danger
                )}
                {DetailCard(Neutral, [.. rows])}
                """;

        return Layout(
            brand,
            autoRefunded
                ? $"Đã tự động hoàn tiền — Booking #{bookingId}"
                : $"CẦN HOÀN TIỀN THỦ CÔNG — Booking #{bookingId}",
            "Thông báo hệ thống",
            body
        );
    }
}

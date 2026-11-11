using System.Globalization;
using System.Net;
using Cinema.Application.Options;

namespace Cinema.Application.Utils;

public static class EmailTemplates
{
    private const string ColorBackground = "#f8f3eb";
    private const string ColorCard = "#fefbf7";
    private const string ColorSecondary = "#ece5dc";
    private const string ColorForeground = "#1e1311";
    private const string ColorMuted = "#6a5a51";
    private const string ColorBorder = "#dad3ca";
    private const string ColorPrimary = "#a50013";
    private const string ColorPrimaryForeground = "#fdfaf4";
    private const string ColorSuccess = "#187c49";
    private const string ColorDestructive = "#d41101";
    private const string ColorWarningForeground = "#211201";

    private const string TintSuccess = "#e7eee6";
    private const string TintDestructive = "#fae4de";
    private const string TintWarning = "#faf0df";

    private const string FontBody = "'Be Vietnam Pro', 'Segoe UI', Helvetica, Arial, sans-serif";
    private const string FontHeading = "'Archivo', 'Arial Narrow', Arial, sans-serif";
    private const string FontSymbol = "'Segoe UI Symbol', 'Apple Symbols', Arial, sans-serif";

    private const string StyleH2 =
        $"font-family:{FontHeading}; font-stretch:100%; font-weight:700; font-size:28px; line-height:1.15; letter-spacing:-0.01em;";
    private const string StyleNumeral =
        $"font-family:{FontHeading}; font-stretch:75%; font-variation-settings:'wdth' 75; font-weight:800; font-variant-numeric:tabular-nums; line-height:1.05;";
    private const string StyleCode =
        $"font-family:{FontHeading}; font-stretch:80%; font-variation-settings:'wdth' 80; font-weight:600; font-variant-numeric:tabular-nums; letter-spacing:0.04em;";

    private const string SymbolCheck = "&#10003;&#xFE0E;";
    private const string SymbolCross = "&#10005;&#xFE0E;";
    private const string SymbolAlert = "!";

    private static readonly NumberFormatInfo MoneyFormat = new() { NumberGroupSeparator = "." };

    private sealed record Tone(string Css, string Bg, string Mark, string MarkCss);

    private static readonly Tone Success = new("ok", TintSuccess, ColorSuccess, "mk-ok");
    private static readonly Tone Danger = new("err", TintDestructive, ColorDestructive, "mk-err");
    private static readonly Tone Warning = new(
        "warn",
        TintWarning,
        ColorWarningForeground,
        "mk-warn"
    );
    private static readonly Tone Neutral = new("neutral", ColorSecondary, ColorForeground, "tx");

    private static string Enc(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);

    private static string MoneyDigits(decimal amount) => amount.ToString("N0", MoneyFormat);

    private static string Money(decimal amount) => $"{MoneyDigits(amount)} VNĐ";

    private static string Layout(
        BrandOptions brand,
        string preheader,
        string bodyHtml,
        string ticketHtml = ""
    )
    {
        var year = BusinessTime.Today().Year;
        var name = Enc(brand.DisplayName);
        var support = string.IsNullOrWhiteSpace(brand.SupportEmail)
            ? string.Empty
            : $"<br />Cần hỗ trợ? Liên hệ <a href=\"mailto:{Enc(brand.SupportEmail)}\" class=\"tx\" style=\"color:{ColorForeground}; text-decoration:underline;\">{Enc(brand.SupportEmail)}</a>.";

        return $$"""
            <!DOCTYPE html>
            <html lang="vi">
              <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                <meta name="color-scheme" content="light dark" />
                <meta name="supported-color-schemes" content="light dark" />
                <title>{{name}}</title>
                <!--[if !mso]><!-->
                <link href="https://fonts.googleapis.com/css2?family=Archivo:wdth,wght@75..100,500..800&family=Be+Vietnam+Pro:wght@400;500;600&display=swap" rel="stylesheet" />
                <!--<![endif]-->
                <style>
                  @media (prefers-color-scheme: dark) {
                    .bg-page { background-color:#0a0404 !important; }
                    .bg-card { background-color:#1a0e0c !important; }
                    .bg-sec { background-color:#221816 !important; }
                    .bg-pri { background-color:#d4212d !important; }
                    .tx { color:#f2eee7 !important; }
                    .tx-mut { color:#a89c92 !important; }
                    .bd { border-color:#322523 !important; }
                    .tone-ok { background-color:#1e2c1e !important; }
                    .tone-err { background-color:#441611 !important; }
                    .tone-warn { background-color:#432a11 !important; }
                    .tone-neutral { background-color:#221816 !important; }
                    .mk-ok { color:#2fa465 !important; }
                    .mk-err { color:#ed3726 !important; }
                    .mk-warn { color:#e69825 !important; }
                  }
                </style>
              </head>
              <body class="bg-page tx" style="margin:0; padding:0; background-color:{{ColorBackground}}; font-family:{{FontBody}}; font-size:15px; line-height:1.55; color:{{ColorForeground}};">
                <span style="display:none; max-height:0; overflow:hidden; opacity:0; mso-hide:all;">{{Enc(
                preheader
            )}}</span>
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" class="bg-page" style="background-color:{{ColorBackground}};">
                  <tr>
                    <td align="center" style="padding:32px 16px;">
                      <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="max-width:560px;">
                        <tr>
                          <td class="tx" style="padding:0 0 16px 0; {{StyleNumeral}} font-size:24px; color:{{ColorForeground}};">{{name}}<span class="bg-pri" style="display:inline-block; width:7px; height:7px; border-radius:50%; background-color:{{ColorPrimary}}; margin-left:3px;"></span></td>
                        </tr>
                        <tr>
                          <td>
                            <table role="presentation" width="100%" cellpadding="0" cellspacing="0">
                              <tr>
                                <td class="bg-card" style="background-color:{{ColorCard}}; padding:32px 28px;">
                                  {{bodyHtml}}
                                </td>
                              </tr>
                              {{ticketHtml}}
                            </table>
                          </td>
                        </tr>
                        <tr>
                          <td class="tx-mut" style="padding:24px 0 0 0; font-size:13px; line-height:1.5; font-weight:600; color:{{ColorMuted}};">
                            {{Enc(brand.Tagline)}}
                          </td>
                        </tr>
                        <tr>
                          <td class="tx-mut" style="padding:8px 0 0 0; font-size:13px; line-height:1.5; color:{{ColorMuted}};">
                            Email này được gửi tự động từ hệ thống {{name}}, vui lòng không trả lời trực tiếp email này.{{support}}<br />
                            &copy; {{year}} {{name}}. Đã đăng ký bản quyền.
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
        $"""<h1 class="tx" style="margin:0 0 16px 0; {StyleH2} color:{ColorForeground};">{text}</h1>""";

    private static string Paragraph(string html) =>
        $"""<p class="tx" style="margin:0 0 16px 0; font-size:15px; line-height:1.55; color:{ColorForeground};">{html}</p>""";

    private static string MutedParagraph(string html) =>
        $"""<p class="tx-mut" style="margin:0; font-size:13px; line-height:1.5; color:{ColorMuted};">{html}</p>""";

    private static string Button(string label, string url) =>
        $"""
            <table role="presentation" cellpadding="0" cellspacing="0" style="margin:8px 0 20px 0;">
              <tr>
                <td class="bg-pri" style="background-color:{ColorPrimary};">
                  <a href="{Enc(
                url
            )}" target="_blank" style="display:inline-block; padding:13px 24px; font-family:{FontBody}; font-size:15px; line-height:18px; font-weight:500; color:{ColorPrimaryForeground}; text-decoration:none;">{label}</a>
                </td>
              </tr>
            </table>
            <p class="tx-mut" style="margin:0 0 20px 0; font-size:13px; line-height:1.5; color:{ColorMuted};">Nếu nút không mở được, hãy sao chép liên kết này vào trình duyệt:<br /><a href="{Enc(
                url
            )}" target="_blank" class="tx" style="color:{ColorForeground}; text-decoration:underline; word-break:break-all;">{Enc(
                url
            )}</a></p>
            """;

    private static string Badge(string text, Tone tone, string symbol) =>
        $"""
            <table role="presentation" cellpadding="0" cellspacing="0" style="margin:0 0 16px 0;">
              <tr>
                <td class="tone-{tone.Css}" style="background-color:{tone.Bg}; padding:6px 12px; font-size:13px; line-height:1.4; font-weight:600; color:{ColorForeground};"><span class="{tone.MarkCss}" style="font-family:{FontSymbol}; font-weight:700; color:{tone.Mark};">{symbol}</span><span class="tx" style="padding-left:8px; color:{ColorForeground};">{text}</span></td>
              </tr>
            </table>
            """;

    private static string Callout(string html, Tone tone, string symbol) =>
        $"""
            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" class="tone-{tone.Css}" style="background-color:{tone.Bg}; margin:0 0 20px 0;">
              <tr>
                <td class="{tone.MarkCss}" width="20" valign="top" style="width:20px; padding:14px 0 14px 16px; font-family:{FontSymbol}; font-size:15px; line-height:1.55; font-weight:700; color:{tone.Mark};">{symbol}</td>
                <td class="tx" style="padding:14px 16px 14px 8px; font-size:15px; line-height:1.55; color:{ColorForeground};">{html}</td>
              </tr>
            </table>
            """;

    private static string DetailRow(
        string label,
        string value,
        bool isLast = false,
        bool code = false
    )
    {
        var line = isLast ? string.Empty : $"border-bottom:1px solid {ColorBorder};";
        var valueStyle = code ? $"{StyleCode} font-size:15px;" : "font-size:15px; font-weight:600;";

        return $"""
            <tr>
              <td class="tx-mut bd" style="padding:12px 16px 12px 0; {line} vertical-align:baseline; font-size:13px; line-height:1.4; font-weight:500; color:{ColorMuted};">{label}</td>
              <td class="tx bd" style="padding:12px 0; {line} vertical-align:baseline; {valueStyle} line-height:1.4; color:{ColorForeground}; text-align:right;">{value}</td>
            </tr>
            """;
    }

    private static string DetailRowTotal(string label, decimal amount) =>
        $"""
            <tr>
              <td class="tx" style="padding:16px 16px 12px 0; vertical-align:baseline; font-size:15px; line-height:1.4; font-weight:600; color:{ColorForeground};">{label}</td>
              <td class="tx" style="padding:12px 0; vertical-align:baseline; white-space:nowrap; {StyleNumeral} font-size:28px; color:{ColorForeground}; text-align:right;">{MoneyDigits(
                amount
            )}<span style="font-family:{FontBody}; font-size:15px; font-weight:600; letter-spacing:0;">&nbsp;VNĐ</span></td>
            </tr>
            """;

    private static string DetailCard(params string[] rows) =>
        $"""
            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" class="bg-sec" style="background-color:{ColorSecondary}; margin:4px 0 20px 0;">
              <tr>
                <td style="padding:4px 16px;">
                  <table role="presentation" width="100%" cellpadding="0" cellspacing="0">
                    {string.Concat(rows)}
                  </table>
                </td>
              </tr>
            </table>
            """;

    private static string TicketStub(string bookingCode) =>
        $"""
            <tr>
              <td style="padding:0;">
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0">
                  <tr>
                    <td class="bg-card" width="10" style="width:10px; height:10px; padding:0; background-color:{ColorCard}; font-size:0; line-height:0;"><div class="bg-page" style="width:10px; height:10px; background-color:{ColorBackground}; border-radius:0 10px 0 0; font-size:0; line-height:0;">&nbsp;</div></td>
                    <td class="bg-card" style="height:10px; padding:0; background-color:{ColorCard}; font-size:0; line-height:0;">&nbsp;</td>
                    <td class="bg-card" width="10" style="width:10px; height:10px; padding:0; background-color:{ColorCard}; font-size:0; line-height:0;"><div class="bg-page" style="width:10px; height:10px; background-color:{ColorBackground}; border-radius:10px 0 0 0; font-size:0; line-height:0;">&nbsp;</div></td>
                  </tr>
                  <tr>
                    <td class="bg-pri" width="10" style="width:10px; height:10px; padding:0; background-color:{ColorPrimary}; font-size:0; line-height:0;"><div class="bg-page" style="width:10px; height:10px; background-color:{ColorBackground}; border-radius:0 0 10px 0; font-size:0; line-height:0;">&nbsp;</div></td>
                    <td class="bg-pri" style="height:7px; padding:3px 0 0 0; background-color:{ColorPrimary}; font-size:0; line-height:0;"><div style="height:0; border-top:3px dotted {ColorPrimaryForeground}; font-size:0; line-height:0;">&nbsp;</div></td>
                    <td class="bg-pri" width="10" style="width:10px; height:10px; padding:0; background-color:{ColorPrimary}; font-size:0; line-height:0;"><div class="bg-page" style="width:10px; height:10px; background-color:{ColorBackground}; border-radius:0 0 0 10px; font-size:0; line-height:0;">&nbsp;</div></td>
                  </tr>
                </table>
              </td>
            </tr>
            <tr>
              <td class="bg-pri" style="background-color:{ColorPrimary}; padding:8px 28px 28px 28px;">
                <p style="margin:0 0 4px 0; font-size:13px; line-height:1.5; font-weight:500; color:{ColorPrimaryForeground};">Mã đặt vé</p>
                <p style="margin:0; {StyleNumeral} font-size:32px; letter-spacing:0.04em; word-break:break-all; color:{ColorPrimaryForeground};">{Enc(
                bookingCode
            )}</p>
              </td>
            </tr>
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
            {Paragraph($"Xin chào <strong>{Enc(fullName)}</strong>,")}
            {Paragraph(introText)}
            {Button(buttonLabel, link)}
            {MutedParagraph(
                $"Liên kết hết hạn sau <strong>{expiresIn}</strong>. Nếu bạn không yêu cầu, hãy bỏ qua email này."
            )}
            """;

        return Layout(brand, title, body);
    }

    public static string BookingConfirmed(
        BrandOptions brand,
        string customerName,
        string bookingCode,
        string movieTitle,
        int seatCount
    )
    {
        var rows = new List<string>();
        if (!string.IsNullOrWhiteSpace(movieTitle))
            rows.Add(DetailRow("Phim", Enc(movieTitle)));
        rows.Add(DetailRow("Số ghế", $"{seatCount} ghế", isLast: true));

        var body = $"""
            {Badge("Đã xác nhận", Success, SymbolCheck)}
            {Heading("Vé của bạn đã sẵn sàng")}
            {Paragraph(
                $"Xin chào <strong>{Enc(customerName)}</strong>, đơn đặt vé của bạn đã được xác nhận. Vào mục <strong>Vé của tôi</strong> trên hệ thống để xem chi tiết và mã QR."
            )}
            {DetailCard([.. rows])}
            {MutedParagraph(
                "Có mặt trước giờ chiếu ít nhất 15 phút và xuất trình mã QR tại quầy soát vé."
            )}
            """;

        return Layout(brand, $"Vé {bookingCode} đã được xác nhận", body, TicketStub(bookingCode));
    }

    public static string BookingCancelledRefunded(
        BrandOptions brand,
        string customerName,
        string bookingCode,
        decimal refundAmount
    )
    {
        var body = $"""
            {Badge("Đã hủy vé", Danger, SymbolCross)}
            {Heading("Đơn đặt vé đã được hủy")}
            {Paragraph(
                $"Xin chào <strong>{Enc(customerName)}</strong>, đơn đặt vé <strong>{Enc(bookingCode)}</strong> đã được hủy theo yêu cầu của bạn."
            )}
            {DetailCard(
                DetailRow("Mã đặt vé", Enc(bookingCode), code: true),
                DetailRowTotal("Số tiền hoàn lại", refundAmount)
            )}
            {MutedParagraph(
                "Tiền hoàn về tài khoản hoặc thẻ bạn đã dùng để thanh toán trong vài ngày làm việc, tùy thời gian xử lý của ngân hàng."
            )}
            """;

        return Layout(brand, $"Đơn đặt vé {bookingCode} đã hủy, hoàn {Money(refundAmount)}", body);
    }

    public static string ShowtimeCancelledAutoRefunded(
        BrandOptions brand,
        string customerName,
        string bookingCode,
        decimal refundAmount
    )
    {
        var body = $"""
            {Badge("Suất chiếu bị hủy", Danger, SymbolCross)}
            {Heading("Suất chiếu của bạn đã bị hủy")}
            {Paragraph(
                $"Xin chào <strong>{Enc(customerName)}</strong>, rạp đã hủy suất chiếu trong đơn đặt vé <strong>{Enc(bookingCode)}</strong>. Chúng tôi đã hoàn 100% số tiền về phương thức thanh toán bạn đã dùng."
            )}
            {DetailCard(
                DetailRow("Mã đặt vé", Enc(bookingCode), code: true),
                DetailRowTotal("Số tiền đã hoàn", refundAmount)
            )}
            {MutedParagraph(
                "Tiền hoàn về tài khoản hoặc thẻ bạn đã dùng để thanh toán trong vài ngày làm việc, tùy thời gian xử lý của ngân hàng hoặc cổng thanh toán."
            )}
            """;

        return Layout(
            brand,
            $"Suất chiếu của đơn {bookingCode} đã bị hủy, đã hoàn {Money(refundAmount)}",
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
            {Badge("Suất chiếu bị hủy", Danger, SymbolCross)}
            {Heading("Suất chiếu của bạn đã bị hủy")}
            {Paragraph(
                $"Xin chào <strong>{Enc(customerName)}</strong>, rạp đã hủy suất chiếu trong đơn đặt vé <strong>{Enc(bookingCode)}</strong>. Khoản hoàn tiền cần nhân viên rạp xử lý thủ công."
            )}
            {DetailCard(
                DetailRow("Mã đặt vé", Enc(bookingCode), code: true),
                DetailRowTotal("Số tiền cần hoàn", amountDue)
            )}
            {MutedParagraph(
                "Mang mã đặt vé đến quầy hoặc liên hệ rạp để được hoàn tiền sớm nhất."
            )}
            """;

        return Layout(
            brand,
            $"Suất chiếu của đơn {bookingCode} đã bị hủy, cần hoàn {Money(amountDue)}",
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
            DetailRow("Mã đặt vé", Enc(bookingCode), code: true),
            DetailRowTotal(autoRefunded ? "Số tiền đã hoàn" : "Số tiền sẽ được hoàn", amount),
        };

        var body = autoRefunded
            ? $"""
                {Badge("Đã hoàn tiền", Neutral, SymbolCheck)}
                {Heading("Không xuất được vé")}
                {Paragraph(
                    $"Xin chào <strong>{Enc(customerName)}</strong>, thanh toán cho đơn đặt vé <strong>{Enc(bookingCode)}</strong> đã thành công nhưng ghế hết thời gian giữ chỗ trước khi vé được xuất. Chúng tôi đã hoàn toàn bộ số tiền về phương thức thanh toán bạn đã dùng."
                )}
                {DetailCard([.. rows])}
                {MutedParagraph(
                    "Tiền hoàn về tài khoản hoặc thẻ bạn đã dùng để thanh toán trong vài ngày làm việc, tùy thời gian xử lý của ngân hàng."
                )}
                """
            : $"""
                {Badge("Chờ hoàn tiền", Warning, SymbolAlert)}
                {Heading("Không xuất được vé")}
                {Paragraph(
                    $"Xin chào <strong>{Enc(customerName)}</strong>, thanh toán cho đơn đặt vé <strong>{Enc(bookingCode)}</strong> đã thành công nhưng ghế hết thời gian giữ chỗ trước khi vé được xuất. Rạp sẽ hoàn toàn bộ số tiền cho bạn sớm nhất."
                )}
                {DetailCard([.. rows])}
                {MutedParagraph(
                    "Cần hỗ trợ thì liên hệ trực tiếp rạp và cung cấp mã đặt vé ở trên."
                )}
                """;

        return Layout(
            brand,
            $"Đơn đặt vé {bookingCode} chưa xuất được vé, {(autoRefunded ? "đã hoàn tiền" : "chờ hoàn tiền")}",
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
            DetailRow("Đơn đặt vé", $"#{bookingId}", code: true),
            DetailRow("Thanh toán", $"#{paymentId}", code: true),
            DetailRowTotal(autoRefunded ? "Số tiền đã hoàn" : "Số tiền cần hoàn", amount),
        };

        var body = autoRefunded
            ? $"""
                {Badge("Đã tự động hoàn tiền", Success, SymbolCheck)}
                {Heading("Ghế hết hạn giữ ngay sau khi thanh toán")}
                {Paragraph(
                    "Đơn đặt vé đã thanh toán thành công nhưng ghế bị hệ thống nhả do hết hạn giữ chỗ đúng lúc đó."
                )}
                {Paragraph(
                    "Hệ thống đã <strong>tự động hoàn tiền</strong> cho khách qua cổng thanh toán. Kiểm tra lại giao dịch hoàn tiền để chắc chắn."
                )}
                {DetailCard([.. rows])}
                """
            : $"""
                {Badge("Cần xử lý thủ công", Danger, SymbolAlert)}
                {Heading("Ghế hết hạn giữ ngay sau khi thanh toán")}
                {Paragraph(
                    "Đơn đặt vé đã thanh toán thành công nhưng ghế bị hệ thống nhả do hết hạn giữ chỗ đúng lúc đó."
                )}
                {Callout(
                    "<strong>Hoàn tiền tự động qua cổng thanh toán thất bại hoặc không khả dụng.</strong> Hoàn tiền thủ công cho khách.",
                    Danger,
                    SymbolAlert
                )}
                {DetailCard([.. rows])}
                """;

        return Layout(
            brand,
            autoRefunded
                ? $"Đã tự động hoàn tiền cho đơn đặt vé #{bookingId}"
                : $"Cần hoàn tiền thủ công cho đơn đặt vé #{bookingId}",
            body
        );
    }
}

import docx
from docx.shared import Pt
import os

def update_docx():
    doc = docx.Document('docs/bao-cao.docx')
    print(f"Loaded document. Paragraphs: {len(doc.paragraphs)}, Tables: {len(doc.tables)}")

    # 1. P42: Tóm tắt tiếng Việt
    p42 = doc.paragraphs[42]
    new_p42 = p42.text.replace(
        "417 test đơn vị (đếm [Fact] và [Theory], tương ứng 466 ca chạy thuộc 47 tệp), 57 test tích hợp,",
        "420 test đơn vị (đếm [Fact] và [Theory], tương ứng 471 ca chạy thuộc 47 tệp), 61 test tích hợp,"
    ).replace(
        "bộ test backend gồm 466 ca chạy đơn vị và 57 test tích hợp đều đạt (523/523, 80 giây);",
        "bộ test backend gồm 471 ca chạy đơn vị và 61 test tích hợp đều đạt (532/532, 5 phút 49 giây);"
    )
    p42.text = new_p42
    print("Updated P42")

    # 2. P48: Abstract tiếng Anh
    p48 = doc.paragraphs[48]
    new_p48 = p48.text.replace(
        "417 unit tests (counting [Fact] and [Theory], equal to 466 executed cases across 47 files), 57 integration tests,",
        "420 unit tests (counting [Fact] and [Theory], equal to 471 executed cases across 47 files), 61 integration tests,"
    ).replace(
        "the backend suites (466 unit-test cases and 57 integration tests) all passed (523 of 523, 80 seconds);",
        "the backend suites (471 unit-test cases and 61 integration tests) all passed (532 of 532, 5 minutes 49 seconds);"
    )
    p48.text = new_p48
    print("Updated P48")

    # 3. P1998: Hiện thực xác thực, RefreshCookie, CsrfProtection, Grace Window
    p1998 = doc.paragraphs[1998]
    new_p1998 = (
        "Access token là JWT ký HMAC-SHA256, mang các claim mã người dùng, vai trò, sub, email và mã nhân viên (nếu có); "
        "thời hạn mặc định AccessTokenExpirationSeconds = 3600. Refresh token là chuỗi ngẫu nhiên 32 byte "
        "(RandomNumberGenerator.GetBytes(32)), được băm trước khi lưu (HashOpaqueToken), hết hạn sau RefreshTokenExpirationDays "
        "(mặc định 30 ngày). Nhằm triệt tiêu rủi ro rò rỉ token qua tấn công XSS, refresh token không truyền trong JSON body "
        "mà được chuyển giao qua HttpOnly, Secure, SameSite=Lax Cookie (RefreshCookie.cs, đường dẫn /api/v1/auth). Đồng thời, "
        "bộ lọc CsrfProtectionFilter áp dụng trên toàn bộ endpoint xác thực thay đổi trạng thái, bắt buộc client gửi kèm header "
        "X-Requested-With: XMLHttpRequest và kiểm tra Origin hợp lệ trong danh sách whitelist CORS. Khi client gửi yêu cầu làm mới, "
        "RefreshAsync áp dụng cơ chế xoay vòng: phát hành token mới, đánh dấu token cũ IsRevoked và ghi ReplacedByTokenId. "
        "Đặc biệt, hệ thống tích hợp cơ chế cửa sổ ân hạn (IsBenignReplayAsync): nếu một refresh token vừa bị thu hồi được gửi lại "
        "trong khoảng thời gian ân hạn ngắn (grace window do nhiều tab trình duyệt hoặc request song song), hệ thống cấp lại "
        "Access Token mới mà không thu hồi chuỗi; ngược lại, nếu phát hiện hành vi dùng lại ngoài cửa sổ ân hạn (replay attack), "
        "hàm RevokeTokenChainAsync lập tức thu hồi toàn bộ chuỗi token kế thừa. Đổi mật khẩu (ChangePasswordAsync) và đặt lại "
        "mật khẩu (ResetPasswordAsync) đều thu hồi mọi refresh token còn hiệu lực của người dùng. Token cho SignalR do "
        "GenerateSocketToken phát hành với thời hạn SocketTokenExpirationSeconds (mặc định 300 giây)."
    )
    p1998.text = new_p1998
    print("Updated P1998")

    # 4. P2377: Kim tự tháp kiểm thử
    p2377 = doc.paragraphs[2377]
    new_p2377 = p2377.text.replace(
        "417 test đơn vị (466 ca chạy), 57 test tích hợp,",
        "420 test đơn vị (471 ca chạy), 61 test tích hợp,"
    )
    p2377.text = new_p2377
    print("Updated P2377")

    # 5. P2389: Kiểm thử đơn vị Nhóm Xác thực (59 test)
    p2389 = doc.paragraphs[2389]
    new_p2389 = (
        "Nhóm kiểm thử xác thực bao quát toàn diện quy trình bảo mật tài khoản với 59 test: "
        "AuthServiceTests (22 test bao quát đăng ký, đăng nhập mật khẩu, cổng quản trị, Google OAuth, xoay vòng refresh token, "
        "kiểm tra cửa sổ ân hạn Grace Window chống race condition, thu hồi toàn bộ chuỗi token khi phát hiện replay attack và đăng xuất), "
        "RegistrationServiceTests (7 test kiểm tra đăng ký tài khoản, phát hành và xác nhận email), "
        "PasswordServiceTests (6 test kiểm tra đổi mật khẩu, quên mật khẩu, đặt lại mật khẩu với token và thu hồi chuỗi phiên đăng nhập), "
        "JwtTokenServiceTests (9 test kiểm tra tính chất claim, hạn 5 phút của socket token), và "
        "AuthValidatorsTests (15 test trong đó có 3 [Theory] kiểm tra quy tắc hợp lệ của dữ liệu đầu vào)."
    )
    p2389.text = new_p2389
    print("Updated P2389")

    # 6. P2416: Kết quả chạy test backend
    p2416 = doc.paragraphs[2416]
    new_p2416 = (
        "Kết quả chạy được thu bằng npm run test:unit và npm run test:integration cho tổng số test, số đạt, lỗi, bỏ qua và thời gian; "
        "độ bao phủ thu bằng npm run test:backend:coverage. Kết quả chạy được ghi ở Bảng 6.11: toàn bộ test đều đạt. "
        "Số test đếm từ mã nguồn là 420 (đơn vị, thuộc 47 tệp kiểm thử) và 61 (tích hợp, thuộc 8 tệp kiểm thử); "
        "số liệu này là số thuộc tính [Fact]/[Theory], chưa tính số bộ dữ liệu của [Theory], nên số ca chạy thực tế của bộ đơn vị "
        "là 471 ca chạy. Toàn bộ 471 ca kiểm thử đơn vị và 61 ca tích hợp đều vượt qua (tổng cộng 532/532 ca kiểm thử đạt tỷ lệ 100%)."
    )
    p2416.text = new_p2416
    print("Updated P2416")

    # 7. P2428: Danh sách test tích hợp
    p2428 = doc.paragraphs[2428]
    new_p2428 = p2428.text.replace(
        "Có 57 test tích hợp, phân bố trong tám file: AdminBookingsEndpointsTests 17 test; AdminMoviesBulkEndpointsTests 3 test; AdminReviewsBulkEndpointsTests 3 test; AdminRoomsEndpointsTests 8 test; AdminShowtimesBulkEndpointsTests 6 test; AuthEndpointsTests 11 test;",
        "Có 61 test tích hợp, phân bố trong tám file: AdminBookingsEndpointsTests 17 test; AdminMoviesBulkEndpointsTests 3 test; AdminReviewsBulkEndpointsTests 3 test; AdminRoomsEndpointsTests 8 test; AdminShowtimesBulkEndpointsTests 6 test; AuthEndpointsTests 15 test (bổ sung kiểm thử lọc CSRF header X-Requested-With, Whitelist Origin và làm mới token qua HttpOnly Cookie);"
    )
    p2428.text = new_p2428
    print("Updated P2428")

    # 8. P2532: Mức đạt mục tiêu Chương 7
    p2532 = doc.paragraphs[2532]
    new_p2532 = p2532.text.replace(
        "Toàn bộ 523 test xUnit backend (466 unit test và 57 integration test),",
        "Toàn bộ 532 bài test xUnit backend (471 ca chạy unit test và 61 integration test),"
    )
    p2532.text = new_p2532
    print("Updated P2532")

    # 9. P2593: MT5 ở Kết luận Chương 8
    p2593 = doc.paragraphs[2593]
    new_p2593 = p2593.text.replace(
        "Bộ kiểm thử xUnit gồm 523 bài test đạt 100% trong 80 giây gồm 466 unit test và 57 integration test;",
        "Bộ kiểm thử xUnit gồm 532 bài test đạt 100% trong 5 phút 49 giây gồm 471 ca chạy unit test và 61 integration test;"
    )
    p2593.text = new_p2593
    print("Updated P2593")

    # 10. Table 3: Bảng 1.1 (MT5)
    t3 = doc.tables[3]
    row5 = t3.rows[5]
    row5.cells[4].text = row5.cells[4].text.replace(
        "523/523 test xUnit pass 100% gồm 466 unit test và 57 integration test",
        "532/532 bài test xUnit backend pass 100% gồm 471 ca chạy unit test và 61 integration test"
    )
    print("Updated Table 3")

    # 11. Table 20: Bảng 2.6 (NFR-01 and NFR-06)
    t20 = doc.tables[20]
    # NFR-01
    r1 = t20.rows[1]
    r1.cells[2].text = (
        "Access token JWT ký HMAC SHA-256, hạn mặc định 3600 giây (AccessTokenExpirationSeconds). "
        "Refresh token là chuỗi ngẫu nhiên 32 byte băm SHA-256 lưu trong cơ sở dữ liệu và chuyển giao tới trình duyệt qua "
        "HttpOnly, Secure, SameSite=Lax Cookie (RefreshCookie.cs, path /api/v1/auth) triệt tiêu nguy cơ XSS. Bộ lọc "
        "CsrfProtectionFilter bảo vệ toàn bộ endpoint xác thực thay đổi trạng thái qua header X-Requested-With và CORS whitelist. "
        "Áp dụng cơ chế xoay vòng token có cửa sổ ân hạn Grace Window chống race condition song song và thu hồi toàn bộ chuỗi "
        "token (RevokeTokenChainAsync) khi phát hiện replay attack. Mỗi yêu cầu có token đều kiểm tra tài khoản còn hoạt động (OnTokenValidated). "
        "Mật khẩu được băm bằng IPasswordHasher và băm lại khi cần"
    )
    r1.cells[3].text = (
        "backend/Cinema.Application/Services/Auth/JwtTokenService.cs\n"
        "backend/Cinema.Application/Services/Auth/AuthService.cs → RefreshAsync\n"
        "backend/Cinema.Api/Security/RefreshCookie.cs\n"
        "backend/Cinema.Api/Filters/CsrfProtectionFilter.cs\n"
        "backend/Cinema.Api/Program.cs"
    )
    r1.cells[4].text = (
        "Chạy các test nhóm Xác thực (AuthServiceTests, JwtTokenServiceTests, AuthValidatorsTests) "
        "và test tích hợp AuthEndpointsTests (15 test bao quát đăng ký, cookie refresh, CSRF filter và phân quyền)"
    )

    # NFR-06
    r6 = t20.rows[6]
    r6.cells[2].text = (
        "Mã nguồn phân tầng (Api, Application, Domain, Infrastructure, Shared) và nhận phụ thuộc qua giao diện; "
        "bộ kiểm thử tự động gồm 532 bài test xUnit (420 test đơn vị tương ứng 471 ca chạy và 61 integration test xUnit "
        "với WebApplicationFactory môi trường Testing tắt Hangfire và bộ giới hạn tốc độ, 55 file test class, pass 100%) "
        "và cấu hình Playwright cho E2E (frontend/playwright.config.ts)"
    )
    print("Updated Table 20")

    # 12. Table 129: Bảng 6.1
    t129 = doc.tables[129]
    t129.rows[1].cells[2].text = "420 (471 ca chạy)"
    t129.rows[1].cells[4].text = "Đạt 471/471 test (100% pass, thời gian 4 phút 34 giây)"
    t129.rows[2].cells[1].text = "8"
    t129.rows[2].cells[2].text = "61"
    t129.rows[2].cells[4].text = "Đạt 61/61 test (100% pass, thời gian 1 phút 15 giây)"
    print("Updated Table 129")

    # 13. Table 139: Bảng 6.11
    t139 = doc.tables[139]
    t139.rows[1].cells[1].text = "471"
    t139.rows[1].cells[2].text = "471"
    t139.rows[1].cells[5].text = "4 phút 34 giây"
    t139.rows[2].cells[1].text = "61"
    t139.rows[2].cells[2].text = "61"
    t139.rows[2].cells[5].text = "1 phút 15 giây"
    t139.rows[3].cells[1].text = "532"
    t139.rows[3].cells[2].text = "532"
    t139.rows[3].cells[5].text = "5 phút 49 giây"
    print("Updated Table 139")

    # 14. Table 140: Bảng 6.12 (Cập nhật Row 32 và thêm 4 ca test mới)
    t140 = doc.tables[140]
    r32 = t140.rows[32]
    r32.cells[1].text = "Login_ConfirmedCustomerWithCorrectPassword_ReturnsAccessTokenAndSetsRefreshCookie"
    if len(r32.cells[1].paragraphs[0].runs) > 0:
        r32.cells[1].paragraphs[0].runs[0].font.name = "Consolas"
        r32.cells[1].paragraphs[0].runs[0].font.size = Pt(11)
    r32.cells[2].text = "Khách đã xác nhận đăng nhập đúng mật khẩu: 200 OK, cấp access token và thiết lập cookie HttpOnly perfo_refresh_token."

    new_tests = [
        ("Login_WithoutCsrfHeader_Returns403", "Đăng nhập thiếu header X-Requested-With: 403 Forbidden do bộ lọc CSRF ngăn chặn."),
        ("Login_FromDisallowedOrigin_Returns403", "Đăng nhập từ Origin không thuộc whitelist CORS: 403 Forbidden do bộ lọc CSRF từ chối."),
        ("Refresh_WithCookieFromLogin_ReturnsNewAccessTokenAndRotatesCookie", "Làm mới token với HttpOnly cookie hợp lệ: 200 OK, cấp access token mới và xoay vòng cookie refresh token mới."),
        ("Refresh_WithoutCookie_Returns401", "Làm mới token khi thiếu HttpOnly cookie: 401 Unauthorized.")
    ]

    target_row = t140.rows[33]
    for method_name, desc in new_tests:
        new_row = t140.add_row()
        target_row._tr.addprevious(new_row._tr)
        
        c0 = new_row.cells[0]
        c0.text = "AuthEndpointsTests"
        
        c1 = new_row.cells[1]
        c1.text = method_name
        if len(c1.paragraphs[0].runs) > 0:
            c1.paragraphs[0].runs[0].font.name = "Consolas"
            c1.paragraphs[0].runs[0].font.size = Pt(11)
            
        c2 = new_row.cells[2]
        c2.text = desc

        c0.width = Pt(118.45) # 2369 dxa
        c1.width = Pt(127.8)  # 2556 dxa
        c2.width = Pt(164.8)  # 3296 dxa

    print("Updated Table 140 (total rows now:", len(t140.rows), ")")

    # 15. Table 158: Traceability matrix
    t158 = doc.tables[158]
    r1_158 = t158.rows[1]
    r1_158.cells[1].text = "AuthController, AuthService, RegistrationService, PasswordService, VerificationTokenSender, RefreshCookie, CsrfProtectionFilter, JwtTokenService"
    print("Updated Table 158")

    # 16. Table 159: Bảng 7.2 (MT5)
    t159 = doc.tables[159]
    r5_159 = t159.rows[5]
    r5_159.cells[1].text = r5_159.cells[1].text.replace(
        "523 test xUnit backend (466 unit test, 57 integration test",
        "532 bài test xUnit backend (471 ca chạy unit test, 61 integration test"
    )
    r5_159.cells[2].text = r5_159.cells[2].text.replace(
        "523/523 test xUnit backend đạt 100% trong 80 giây gồm 466 unit test và 57 integration test",
        "532/532 test xUnit backend đạt 100% trong 5 phút 49 giây gồm 471 ca chạy unit test và 61 integration test"
    )
    print("Updated Table 159")

    # 17. Table 161: Bảng 8.1 (MT5)
    t161 = doc.tables[161]
    r5_161 = t161.rows[5]
    r5_161.cells[2].text = r5_161.cells[2].text.replace(
        "523/523 test xUnit backend đạt 100% (466 unit test trong 34 s, 57 integration test trong 46 s)",
        "532/532 test xUnit backend đạt 100% (471 ca chạy unit test trong 4 phút 34 s, 61 integration test trong 1 phút 15 s)"
    )
    print("Updated Table 161")

    # Ensure font formatting
    for p_idx in [42, 48, 1998, 2377, 2389, 2416, 2428, 2532, 2593]:
        p = doc.paragraphs[p_idx]
        for run in p.runs:
            run.font.name = "Times New Roman"
            if p_idx not in [42, 48]:
                run.font.size = Pt(13)

    out_path = 'docs/bao-cao-updated.docx'
    doc.save(out_path)
    print(f"Saved updated document to {out_path} ({os.path.getsize(out_path)} bytes)")

if __name__ == '__main__':
    update_docx()

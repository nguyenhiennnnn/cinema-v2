import docx
from docx.shared import Pt
import re

def update_report(doc_path, output_path=None):
    if output_path is None:
        output_path = doc_path
    
    doc = docx.Document(doc_path)
    print(f"Loaded {doc_path} successfully. Total paragraphs: {len(doc.paragraphs)}, Tables: {len(doc.tables)}")

    # 1. Update Paragraph 42 (Tóm tắt tiếng Việt)
    p42 = doc.paragraphs[42]
    assert "364 test đơn vị" in p42.text, "P42 text assertion failed"
    p42.text = p42.text.replace("364 test đơn vị (đếm [Fact] và [Theory], tương ứng 399 ca chạy)", "377 test đơn vị (đếm [Fact] và [Theory], tương ứng 422 ca chạy)")
    p42.text = p42.text.replace("bộ test backend gồm 399 ca chạy đơn vị và 49 test tích hợp đều đạt (448/448, 77 giây)", "bộ test backend gồm 422 ca chạy đơn vị và 49 test tích hợp đều đạt (471/471, 82 giây)")
    print("Updated P42 (Tóm tắt tiếng Việt)")

    # 2. Update Paragraph 48 (Abstract tiếng Anh)
    p48 = doc.paragraphs[48]
    assert "364 unit tests" in p48.text, "P48 text assertion failed"
    p48.text = p48.text.replace("364 unit tests (counting [Fact] and [Theory], equal to 399 executed cases)", "377 unit tests (counting [Fact] and [Theory], equal to 422 executed cases)")
    p48.text = p48.text.replace("the backend suites (399 unit-test cases and 49 integration tests) all passed (448 of 448, 77 seconds)", "the backend suites (422 unit-test cases and 49 integration tests) all passed (471 of 471, 82 seconds)")
    print("Updated P48 (Abstract tiếng Anh)")

    # 3. Update Paragraph 1108 (Cinema.Application interfaces)
    p1108 = doc.paragraphs[1108]
    for r in p1108.runs:
        if r.text == "IEmailService":
            r.text = "IEmailService, INotificationService"
            print("Updated P1108: Added INotificationService to interfaces")
            break

    # 4. Update Paragraph 1110 (Cinema.Api modular extensions)
    p1110 = doc.paragraphs[1110]
    p1110.runs[-1].text += " Mã nguồn cấu hình khởi động trong Program.cs được tổ chức theo các phương thức mở rộng trong thư mục Cinema.Api/Extensions (ApiStartupExtensions, ApiPipelineExtensions, ApiOptionsExtensions, AuthenticationExtensions, BackgroundJobExtensions, CorsExtensions, RateLimitingExtensions, SwaggerExtensions), tách biệt rõ ràng việc cấu hình dịch vụ, pipeline, xác thực, giới hạn tần suất và tài liệu OpenAPI."
    print("Updated P1110: Added Api extensions modularization")

    # 5. Update Paragraph 1113 (DI registration)
    p1113 = doc.paragraphs[1113]
    for r in p1113.runs:
        if r.text == ", cùng ":
            r.text = ", INotificationService (hiện thực bởi NotificationService điều phối thông báo email), cùng "
            print("Updated P1113: Added INotificationService in AddApplication")
            break

    # 6. Update Paragraph 1119 (Tổ chức mã nguồn frontend)
    p1119 = doc.paragraphs[1119]
    for r in p1119.runs:
        if "components/common (thành phần ghép dùng lại như data-table, seat-map, form, combo-picker, filter)" in r.text:
            r.text = r.text.replace("combo-picker, filter)", "combo-picker, filter, route-fallback)")
        if "hooks (hook dùng chung như use-showtime-hub, use-realtime-seat-map)" in r.text:
            r.text = r.text.replace("use-realtime-seat-map)", "use-realtime-seat-map, use-server-field-errors)")
        if "config (env.ts, routes.ts, brand.ts, default-constants.ts)" in r.text:
            r.text = r.text.replace("default-constants.ts)", "default-constants.ts, error-messages.ts)")
        if "lib (hàm tiện ích, định dạng, signalr.ts)" in r.text:
            r.text = r.text.replace("signalr.ts)", "signalr.ts, error.ts)")
    print("Updated P1119: Added frontend shared components & hooks")

    # 7. Update Paragraph 1122 (Lớp api frontend)
    p1122 = doc.paragraphs[1122]
    for r in p1122.runs:
        if ") gồm bốn tệp. " in r.text:
            r.text = r.text.replace(") gồm bốn tệp. ", ") gồm axios.ts, endpoints.ts, query-client.ts, query-keys.ts và query-invalidation.ts. Trong đó, endpoints.ts định nghĩa tập trung toàn bộ các URL endpoint theo hằng số định kiểu nghiêm ngặt (AUTH_ENDPOINTS, ADMIN_ENDPOINTS, CLIENT_ENDPOINTS, CATALOG_ENDPOINTS,...), loại trừ hoàn toàn chuỗi viết cứng. ")
            print("Updated P1122: Added endpoints.ts to src/api")
            break

    # 8. Update Paragraph 1131 (Xử lý lỗi - Kiến trúc)
    p1131 = doc.paragraphs[1131]
    for r in p1131.runs:
        if ". Nhờ vậy service không cần bi" in r.text:
            r.text = r.text.replace(". Nhờ vậy service không cần bi", ". Middleware được chuẩn hóa theo nguyên lý đơn trách nhiệm (Single Responsibility Principle) kết hợp cùng ApiError, ApiErrorMapper (chuyển đổi ngoại lệ sang mã HTTP) và ApiErrorWriter (ghi phản hồi JSON). Nhờ vậy service không cần bi")
            print("Updated P1131: Added ApiError / ApiErrorMapper / ApiErrorWriter")
            break

    # 9. Update Paragraph 1133 (Xử lý lỗi - OperationCanceled & Messages)
    p1133 = doc.paragraphs[1133]
    p1133.runs[-1].text += " Khi client chủ động hủy yêu cầu (OperationCanceledException), middleware chỉ ghi nhận nhật ký mức Debug và trả về mã 499 (Client Closed Request) mà không ghi thân phản hồi lỗi hay phát sinh nhật ký lỗi 500 sai lệch. Việc ghi phản hồi cũng được bảo vệ qua kiểm tra context.Response.HasStarted để tránh lỗi ghi đè tiêu đề HTTP. Mọi thông điệp lỗi nghiệp vụ và hệ thống đều được truy xuất từ tài nguyên Messages.resx trong Cinema.Shared/Resources thay vì chuỗi viết cứng."
    print("Updated P1133: Added OperationCanceledException and Messages.resx")

    # 10. Update Paragraph 2005 (Module danh mục - Quick Create)
    p2005 = doc.paragraphs[2005]
    for r in p2005.runs:
        if "CatalogService" in r.text and r.bold:
            pass
    # Append Quick create info to P2005
    p2005.runs[-1].text += " Đối với diễn viên và đạo diễn, hệ thống hỗ trợ cơ chế tạo nhanh (Quick Create) qua hai endpoint POST /api/admin/actors/quick và POST /api/admin/directors/quick (tương ứng hai phương thức QuickCreateActorAsync và QuickCreateDirectorAsync trong CatalogService). Khi người dùng nhập tên, hệ thống dùng tiện ích NameNormalizer để chuẩn hóa khoảng trắng và chữ hoa chữ thường. Nếu đã có bản ghi cùng tên trong cơ sở dữ liệu, dịch vụ sẽ tái sử dụng và trả về ngay bản ghi đó thay vì tạo bản ghi trùng hoặc báo lỗi, giúp người quản trị có thể vừa soạn thảo phim vừa bổ sung nhanh diễn viên/đạo diễn ngay trên cùng một biểu mẫu."
    print("Updated P2005: Added Quick Create and NameNormalizer to catalog module")

    # 11. Update Paragraph 2071 (Module email - NotificationService)
    p2071 = doc.paragraphs[2071]
    p2071.runs[-1].text += " Nhằm giảm sự phụ thuộc trực tiếp của các dịch vụ nghiệp vụ (AuthService, BookingService, PaymentService, ShowtimeService) vào giao diện IEmailService và các khuôn mẫu HTML, hệ thống xây dựng INotificationService cùng lớp cài đặt NotificationService (Cinema.Application/Services/Notifications). NotificationService đóng vai trò façade tập trung phát hành mọi loại thông báo: email xác thực tài khoản, đặt lại mật khẩu, xác nhận đặt vé kèm mã vé và mã QR, thông báo hủy vé hoàn tiền và thông báo hủy suất chiếu. Toàn bộ tác vụ gửi email được đưa vào hàng đợi Hangfire qua SendEmailJob để xử lý bất đồng bộ, giúp thao tác nghiệp vụ của người dùng không bị tắc nghẽn."
    print("Updated P2071: Added NotificationService facade to email module")

    # 12. Update Paragraph 2295 (5.4.4.2 Sơ đồ ghế - OKLCH Theme)
    p2295 = doc.paragraphs[2295]
    for r in p2295.runs:
        if r.text == "Trạng thái ghế.":
            r.text = "Trạng thái ghế và hệ thống màu sắc theo Perfo Design System."
            break
    p2295.runs[-1].text += " Giao diện sơ đồ ghế và chú giải (SeatLegend) được đồng bộ hoàn toàn với không gian màu OKLCH của Tailwind CSS v4, đảm bảo độ tương phản cao và nhất quán giữa các chủ đề sáng/tối."
    print("Updated P2295: Added Perfo Design System OKLCH to seat map")

    # 13. Update Paragraph 2302 (5.4.4.3 Trường biểu mẫu - Inline Quick Create & Server Field Errors)
    p2302 = doc.paragraphs[2302]
    p2302.runs[-1].text += ' Đặc biệt, cả hai thành phần đều được tích hợp khả năng tạo nhanh danh mục trực tiếp (inline quick create) thông qua thành phần SelectCreateItem và hook useSelectCreate kết hợp cùng useQuickCreate. Khi quản trị viên tìm kiếm một diễn viên hoặc đạo diễn chưa có sẵn, hệ thống hiển thị tùy chọn "Tạo mới: {từ khóa}" ngay trong danh sách xổ xuống; thao tác chọn sẽ kích hoạt API tạo nhanh, tự động thêm mới vào cơ sở dữ liệu và gắn ngay giá trị vào form. Ngoài ra, hook useServerFieldErrors (frontend/src/hooks/use-server-field-errors.ts) tự động liên kết các lỗi xác thực từ ApiResponse.Errors của backend (do FluentValidation hoặc ModelState trả về) vào từng trường của form qua form.setError, giúp hiển thị thông báo lỗi chính xác ở từng ô nhập liệu. Các biểu mẫu cũng sử dụng chung thành phần FormActions để đồng bộ giao diện các nút thao tác (Lưu, Hủy) và trạng thái tải.'
    print("Updated P2302: Added SelectCreateItem, useServerFieldErrors, FormActions")

    # 14. Update Paragraph 2305 (5.4.4.4 Lớp truy cập API - endpoints.ts)
    p2305 = doc.paragraphs[2305]
    p2305.runs[-1].text += " Toàn bộ URL của các endpoint trong hệ thống được định nghĩa tập trung trong tệp api/endpoints.ts dưới dạng các hằng số định kiểu nghiêm ngặt (AUTH_ENDPOINTS, ADMIN_ENDPOINTS, CLIENT_ENDPOINTS, CATALOG_ENDPOINTS,...), loại trừ hoàn toàn việc viết cứng URL trong mã nguồn tính năng. Tương tự, cấu hình định tuyến ứng dụng được gom tại config/routes.ts, và danh mục thông điệp lỗi giao diện được chuẩn hóa trong config/error-messages.ts cùng bộ xử lý lib/error.ts."
    print("Updated P2305: Added endpoints.ts, routes.ts, error-messages.ts")

    # 15. Update Paragraph 2365 (5.7.2 Docker - DOTNET_EnableDiagnostics=0)
    p2365 = doc.paragraphs[2365]
    p2365.runs[-1].text += " Đáng chú ý, giai đoạn production được bổ sung thiết lập DOTNET_EnableDiagnostics=0 nhằm ngăn chặn triệt để hiện tượng lỗi phân đoạn bộ nhớ (SIGSEGV) do dịch vụ chẩn đoán .NET gây ra khi vận hành trên thư viện musl libc của Alpine Linux."
    print("Updated P2365: Added DOTNET_EnableDiagnostics=0")

    # 16. Update Paragraph 2377 (6.1 Chiến lược kiểm thử)
    p2377 = doc.paragraphs[2377]
    for r in p2377.runs:
        if "364 test đơn vị" in r.text:
            r.text = r.text.replace("364 test đơn vị", "377 test đơn vị")
            print("Updated P2377: Changed 364 to 377 test đơn vị")
            break

    # 17. Update Paragraph 2410 (6.2.8 Nhóm Cấu hình, đánh giá, nhân viên & danh mục)
    p2410 = doc.paragraphs[2410]
    for r in p2410.runs:
        if "Nhóm gồm 39 test: " in r.text:
            r.text = r.text.replace("Nhóm gồm 39 test: ", "Nhóm gồm 47 test: CatalogServiceQuickCreateTests 8 test; ")
        if "), quản lý tài khoản nhân viên" in r.text:
            r.text = r.text.replace("), quản lý tài khoản nhân viên", "), kiểm tra tạo nhanh diễn viên/đạo diễn và chuẩn hóa tên (CatalogServiceQuickCreateTests), quản lý tài khoản nhân viên")
    print("Updated P2410: Added CatalogServiceQuickCreateTests to 6.2.8")

    # 18. Update Paragraph 2413 (6.2.9 Nhóm Tiện ích)
    p2413 = doc.paragraphs[2413]
    for r in p2413.runs:
        if "Nhóm gồm 31 test: " in r.text:
            r.text = r.text.replace("Nhóm gồm 31 test: ", "Nhóm gồm 36 test: NameNormalizerTests 5 test (trong đó 3 [Theory] với 13 ca dữ liệu); ")
        if "). Các test kiểm tra hàm sinh mã" in r.text:
            r.text = r.text.replace("). Các test kiểm tra hàm sinh mã", "). Các test kiểm tra chuẩn hóa chuỗi họ tên tiếng Việt và dấu thanh (NameNormalizerTests), hàm sinh mã")
    print("Updated P2413: Added NameNormalizerTests to 6.2.9")

    # 19. Update Paragraph 2416 (6.2.10 Kết quả chạy test backend)
    p2416 = doc.paragraphs[2416]
    for r in p2416.runs:
        if "364 (đơn vị)" in r.text:
            r.text = r.text.replace("364 (đơn vị)", "377 (đơn vị, thuộc 37 tệp kiểm thử)")
        if "399." in r.text:
            r.text = r.text.replace("399.", "422 ca chạy (toàn bộ 422 ca kiểm thử đơn vị và 49 ca tích hợp đều đạt 100%).")
    print("Updated P2416: Updated test counts in 6.2.10")

    # 20. Update Paragraph 2532 (Mục 7.2)
    p2532 = doc.paragraphs[2532]
    if "448 test xUnit backend" in p2532.text:
        p2532.text = p2532.text.replace("448 test xUnit backend (399 unit test và 49 integration test)", "471 test xUnit backend (422 unit test và 49 integration test)")
        print("Updated P2532: Updated test count to 471")

    # 21. Update Paragraph 2593 (Mục 8 - MT5)
    p2593 = doc.paragraphs[2593]
    if "448 bài test" in p2593.text:
        p2593.text = p2593.text.replace("Bộ kiểm thử xUnit gồm 448 bài test đạt 100% trong 77 giây gồm 399 unit test và 49 integration test", "Bộ kiểm thử xUnit gồm 471 bài test đạt 100% trong 82 giây gồm 422 unit test và 49 integration test")
        print("Updated P2593: Updated test count to 471")

    # 22. Update Tables:
    # Table 3 (Bảng 1.1)
    t3 = doc.tables[3]
    cell_t3 = t3.rows[5].cells[4]
    if "448/448 test xUnit pass 100%" in cell_t3.text:
        cell_t3.text = cell_t3.text.replace("448/448 test xUnit pass 100% gồm 399 unit test và 49 integration test qua 43 file test", "471/471 test xUnit pass 100% gồm 422 unit test và 49 integration test qua 45 file test")
        print("Updated Table 3 (Bảng 1.1)")

    # Table 20 (Bảng Yêu cầu phi chức năng)
    t20 = doc.tables[20]
    cell_t20 = t20.rows[6].cells[2]
    if "448 test xUnit" in cell_t20.text:
        cell_t20.text = cell_t20.text.replace("448 test xUnit (399 unit test và 49 integration test xUnit với WebApplicationFactory môi trường Testing tắt Hangfire và bộ giới hạn tốc độ, 43 file test class, pass 100%)", "471 test xUnit (422 unit test và 49 integration test xUnit với WebApplicationFactory môi trường Testing tắt Hangfire và bộ giới hạn tốc độ, 45 file test class, pass 100%)")
        print("Updated Table 20 (Yêu cầu phi chức năng)")

    # Table 129 (Bảng 6.1 Các tầng kiểm thử)
    t129 = doc.tables[129]
    t129.rows[1].cells[1].text = "37"
    t129.rows[1].cells[2].text = "377 (422 ca chạy)"
    t129.rows[1].cells[4].text = "Đạt 422/422 test (100% pass, thời gian 31 giây)"
    print("Updated Table 129 (Bảng 6.1)")

    # Table 137 (Bảng 6.9 Test nhóm Cấu hình, đánh giá, nhân viên)
    t137 = doc.tables[137]
    quick_create_tests = [
        ("CatalogServiceQuickCreateTests", "CreateGenreAsync_NameHasExtraSpaces_SavesNormalizedName", "Fact", "Chuẩn hóa khoảng trắng khi tạo thể loại mới."),
        ("CatalogServiceQuickCreateTests", "CreateGenreAsync_SeededNameInDifferentCaseAndSpacing_ThrowsConflict", "Fact", "Từ chối tạo thể loại nếu tên đã tồn tại dù khác chữ hoa/thường hoặc khoảng trắng thừa."),
        ("CatalogServiceQuickCreateTests", "CreateScreenTypeAsync_SeededNameInDifferentCase_ThrowsConflict", "Fact", "Từ chối tạo loại màn hình trùng tên bất kể chữ hoa/thường."),
        ("CatalogServiceQuickCreateTests", "CreateAgeRatingAsync_CodeDiffersByCase_ThrowsConflict", "Fact", "Từ chối tạo phân loại độ tuổi trùng mã (Code) bất kể chữ hoa/thường."),
        ("CatalogServiceQuickCreateTests", "QuickCreateActorAsync_NewName_CreatesNormalizedRecord", "Fact", "Tạo nhanh diễn viên mới với tên đã chuẩn hóa và các trường mặc định hợp lệ."),
        ("CatalogServiceQuickCreateTests", "QuickCreateActorAsync_ExistingNameInDifferentCase_ReturnsExistingRecord", "Fact", "Tạo nhanh diễn viên trả về bản ghi sẵn có nếu tên đã tồn tại (không tạo trùng)."),
        ("CatalogServiceQuickCreateTests", "QuickCreateDirectorAsync_SeededNameInDifferentCase_ReturnsSeededRecord", "Fact", "Tạo nhanh đạo diễn trả về bản ghi có sẵn trong dữ liệu mẫu khi trùng tên."),
        ("CatalogServiceQuickCreateTests", "GetActorsAdminAsync_SearchIgnoresCase", "Fact", "Tìm kiếm danh sách diễn viên không phân biệt chữ hoa/thường.")
    ]
    for file_name, test_name, test_type, desc in quick_create_tests:
        r = t137.add_row()
        for c in r.cells:
            c.paragraphs[0].style = "Nội dung bảng"
        r.cells[0].paragraphs[0].text = file_name
        p1 = r.cells[1].paragraphs[0]
        p1.text = ""
        run1 = p1.add_run(test_name)
        run1.font.name = "Consolas"
        run1.font.size = Pt(11)
        r.cells[2].paragraphs[0].text = test_type
        r.cells[3].paragraphs[0].text = desc
    print(f"Updated Table 137 (Bảng 6.9): Added {len(quick_create_tests)} rows. Total rows now: {len(t137.rows)}")

    # Table 138 (Bảng 6.10 Test nhóm Tiện ích)
    t138 = doc.tables[138]
    normalizer_tests = [
        ("NameNormalizerTests", "Normalize_TrimsAndCollapsesWhitespace", "Theory", "Chuẩn hóa chuỗi họ tên: cắt tỉa khoảng trắng đầu/cuối và rút gọn khoảng trắng liên tiếp."),
        ("NameNormalizerTests", "Normalize_ComposedAndDecomposedForms_AreEqual", "Fact", "Đảm bảo tính tương đương giữa dạng ký tự tổ hợp (NFC) và phân rã (NFD) trong tiếng Việt."),
        ("NameNormalizerTests", "AreSame_IgnoresCaseAndExtraWhitespace", "Theory", "So sánh tương đương hai chuỗi tên bất kể chữ hoa/thường và khoảng trắng dư thừa."),
        ("NameNormalizerTests", "AreSame_DifferentDiacritics_AreDifferent", "Fact", "Phân biệt chính xác các dấu thanh tiếng Việt khác nhau (không xem là giống nhau khi khác dấu)."),
        ("NameNormalizerTests", "ContainsDisallowedCharacters_OnlyFlagsNonWhitespaceControls", "Theory", "Kiểm tra và phát hiện các ký tự điều khiển không hợp lệ trong chuỗi nhập liệu họ tên.")
    ]
    for file_name, test_name, test_type, desc in normalizer_tests:
        r = t138.add_row()
        for c in r.cells:
            c.paragraphs[0].style = "Nội dung bảng"
        r.cells[0].paragraphs[0].text = file_name
        p1 = r.cells[1].paragraphs[0]
        p1.text = ""
        run1 = p1.add_run(test_name)
        run1.font.name = "Consolas"
        run1.font.size = Pt(11)
        r.cells[2].paragraphs[0].text = test_type
        r.cells[3].paragraphs[0].text = desc
    print(f"Updated Table 138 (Bảng 6.10): Added {len(normalizer_tests)} rows. Total rows now: {len(t138.rows)}")

    # Table 139 (Bảng 6.11 Kết quả chạy test backend)
    t139 = doc.tables[139]
    t139.rows[1].cells[1].text = "422"
    t139.rows[1].cells[2].text = "422"
    t139.rows[1].cells[5].text = "31 giây"
    t139.rows[3].cells[1].text = "471"
    t139.rows[3].cells[2].text = "471"
    t139.rows[3].cells[5].text = "82 giây"
    print("Updated Table 139 (Bảng 6.11)")

    # Table 158 (Phụ lục: Ánh xạ package sang test)
    t158 = doc.tables[158]
    t158.rows[12].cells[2].text = "CatalogServiceQuickCreateTests (8 test), NameNormalizerTests (5 test)"
    if ", NotificationService" not in t158.rows[21].cells[1].text:
        t158.rows[21].cells[1].text += ", NotificationService"
    print("Updated Table 158 (Ánh xạ package)")

    # Table 159 (Phụ lục: Đánh giá mục tiêu)
    t159 = doc.tables[159]
    cell_t159 = t159.rows[5].cells[1]
    if "448 test xUnit backend" in cell_t159.text:
        cell_t159.text = cell_t159.text.replace("448 test xUnit backend (399 unit test, 49 integration test qua 43 file test class)", "471 test xUnit backend (422 unit test, 49 integration test qua 45 file test class)")
        print("Updated Table 159")

    # Table 161 (Phụ lục: Tổng kết kết quả thực hiện)
    t161 = doc.tables[161]
    cell_t161 = t161.rows[5].cells[2]
    if "448/448 test xUnit backend đạt 100%" in cell_t161.text:
        cell_t161.text = cell_t161.text.replace("448/448 test xUnit backend đạt 100% (399 unit test trong 26 s, 49 integration test trong 51 s)", "471/471 test xUnit backend đạt 100% (422 unit test trong 31 s, 49 integration test trong 51 s)")
        print("Updated Table 161")

    # Save output
    doc.save(output_path)
    print(f"Successfully saved updated document to {output_path}")

if __name__ == "__main__":
    update_report("docs/bao-cao.docx")

import docx
from docx.shared import Pt

def update_report_v3(doc_path, output_path=None):
    if output_path is None:
        output_path = doc_path
    
    doc = docx.Document(doc_path)
    print(f"Loaded {doc_path}. Paras: {len(doc.paragraphs)}, Tables: {len(doc.tables)}")

    # 1. Update P1113 (Mục 4.1.2 Backend Architecture DI registration)
    p1113 = doc.paragraphs[1113]
    old_p1113 = p1113.text
    # Add new decoupled interfaces into AddApplication
    target_str = "IAuthService, ITokenService, ICatalogService, IMovieService, IRoomService, IShowtimeService, IBookingService"
    replacement_str = ("IAuthService (cùng IRegistrationService, IPasswordService, IVerificationTokenSender), "
                       "ITokenService, ICatalogService, IMovieService, IRoomService, "
                       "IShowtimeService (cùng IShowtimeQueryService, IShowtimeBatchService, IShowtimeAutoScheduleService, IShowtimeCancellationService, IShowtimeLifecycleService), "
                       "IBookingService")
    if target_str in old_p1113:
        p1113.text = old_p1113.replace(target_str, replacement_str)
        # Also ensure IUserAdminService is mentioned near IEmployeeService
        if "IEmployeeService" in p1113.text and "IUserAdminService" not in p1113.text:
            p1113.text = p1113.text.replace("IEmployeeService", "IEmployeeService, IUserAdminService")
        print("Updated P1113: Added decoupled service interfaces in AddApplication")

    # 2. Update P1996 (Mục 5.3.1 Module xác thực)
    p1996 = doc.paragraphs[1996]
    if "Lớp dịch vụ nghiệp vụ ban đầu là AuthService" not in p1996.text:
        p1996.text += (" Để đảm bảo nguyên lý đơn trách nhiệm (SRP), lớp dịch vụ nghiệp vụ xác thực được phân rã thành các dịch vụ chuyên trách: "
                       "RegistrationService (IRegistrationService, phụ trách đăng ký tài khoản và phát hành mã xác nhận email), "
                       "PasswordService (IPasswordService, phụ trách đổi mật khẩu, quên mật khẩu và đặt lại mật khẩu với token), "
                       "VerificationTokenSender (IVerificationTokenSender, trừu tượng hóa việc phát hành mã xác thực qua NotificationService), "
                       "và AuthService (tập trung xử lý đăng nhập mật khẩu, đăng nhập Google OAuth, cấp phát JWT và xoay vòng refresh token).")
        print("Updated P1996: Added AuthService decomposition details")

    # 3. Update P2014 (Mục 5.3.6 Module suất chiếu)
    p2014 = doc.paragraphs[2014]
    p2014.text = ("Module cung cấp tạo, sửa, hủy và xóa suất chiếu; tạo hàng loạt; xem trước và áp dụng xếp lịch tự động; "
                  "nhân bản suất chiếu; lịch dạng calendar; sơ đồ ghế của suất chiếu và quản lý vòng đời suất chiếu. "
                  "Nhằm tối ưu hóa cấu trúc và tuân thủ ISP, ShowtimeService (ban đầu quy mô khoảng 1.400 dòng) được phân rã thành "
                  "5 dịch vụ chuyên biệt trong Cinema.Application/Services/Showtimes: "
                  "ShowtimeQueryService (IShowtimeQueryService, tra cứu suất chiếu, lịch calendar và kiểm tra ghế trống), "
                  "ShowtimeBatchService (IShowtimeBatchService, tạo suất lẻ, tạo hàng loạt theo ngày/phòng và nhân bản lịch), "
                  "ShowtimeAutoScheduleService (IShowtimeAutoScheduleService, giải thuật tự động xếp lịch chiếu), "
                  "ShowtimeCancellationService (IShowtimeCancellationService, hủy suất chiếu và kích hoạt hoàn tiền tự động), "
                  "và ShowtimeLifecycleService (IShowtimeLifecycleService, hoàn tất suất chiếu đã kết thúc và chuyển trạng thái phim), "
                  "kết hợp cùng ShowtimeRules.cs tập trung các quy tắc nghiệp vụ rạp; "
                  "controller tiếp nhận gồm AdminShowtimesController (quản trị) và ShowtimesController (khách hàng).")
    print("Updated P2014: Added ShowtimeService decomposition details")

    # 4. Update P2389 (Mục 6.2.1 Nhóm Xác thực)
    p2389 = doc.paragraphs[2389]
    p2389.text = ("Nhóm kiểm thử xác thực bao quát toàn diện quy trình bảo mật tài khoản với 92 test: "
                  "AuthServiceTests (32 test bao quát đăng nhập mật khẩu, cổng quản trị, Google OAuth, xoay vòng và thu hồi refresh token), "
                  "RegistrationServiceTests (6 test kiểm tra đăng ký tài khoản, phát hành và xác nhận email), "
                  "PasswordServiceTests (30 test kiểm tra đổi mật khẩu, quên mật khẩu, đặt lại mật khẩu với token và thu hồi chuỗi phiên đăng nhập), "
                  "JwtTokenServiceTests (9 test kiểm tra tính chất claim, hạn 5 phút của socket token), "
                  "và AuthValidatorsTests (15 test trong đó có 3 [Theory] kiểm tra quy tắc hợp lệ của dữ liệu đầu vào).")
    print("Updated P2389: Updated auth test group details")

    # 5. Update P2401 (Mục 6.2.5 Nhóm Suất chiếu)
    p2401 = doc.paragraphs[2401]
    if "ShowtimeQueryService" not in p2401.text:
        p2401.text = p2401.text.replace(
            "ShowtimeServiceAutoScheduleTests 4 test; ShowtimeServiceCreateTests 14 test; ShowtimeServiceFixesTests 19 test",
            "ShowtimeServiceAutoScheduleTests 4 test; ShowtimeServiceCreateTests 14 test; ShowtimeServiceFixesTests 19 test (bao quát các service tách ShowtimeQueryService, ShowtimeBatchService, ShowtimeCancellationService, ShowtimeLifecycleService)"
        )
        print("Updated P2401: Added decoupled showtime test details")

    # 6. Update P2410 (Mục 6.2.8 Nhóm Cấu hình, đánh giá, nhân viên)
    p2410 = doc.paragraphs[2410]
    if "EmployeeServiceUserStatusTests" in p2410.text:
        p2410.text = p2410.text.replace("EmployeeServiceUserStatusTests", "UserAdminServiceUserStatusTests")
        print("Updated P2410: Replaced EmployeeServiceUserStatusTests with UserAdminServiceUserStatusTests")

    # 7. Update P2377 (Mục 6.1)
    p2377 = doc.paragraphs[2377]
    for r in p2377.runs:
        if "377 test đơn vị" in r.text:
            r.text = r.text.replace("377 test đơn vị", "417 test đơn vị (466 ca chạy)")
            print("Updated P2377 to 417 unit tests")
            break

    # 8. Update Table 129 (Bảng 6.1 Các tầng kiểm thử)
    t129 = doc.tables[129]
    t129.rows[1].cells[1].text = "47"
    t129.rows[1].cells[2].text = "417 (466 ca chạy)"
    t129.rows[1].cells[4].text = "Đạt 466/466 test (100% pass, thời gian 34 giây)"
    t129.rows[2].cells[4].text = "Đạt 57/57 test (100% pass, thời gian 46 giây)"
    print("Updated Table 129: Unit tests to 47 files, 417 tests (466 runs)")

    # 9. Update P2416 (Mục 6.2.10)
    p2416 = doc.paragraphs[2416]
    p2416.text = ("Kết quả chạy được thu bằng npm run test:unit và npm run test:integration cho tổng số test, số đạt, lỗi, bỏ qua và thời gian; "
                  "độ bao phủ thu bằng npm run test:backend:coverage. Kết quả chạy được ghi ở Bảng 6.11: toàn bộ test đều đạt. "
                  "Số test đếm từ mã nguồn là 417 (đơn vị, thuộc 47 tệp kiểm thử) và 57 (tích hợp, thuộc 8 tệp kiểm thử); "
                  "số liệu này là số thuộc tính [Fact]/[Theory], chưa tính số bộ dữ liệu của [Theory], nên số ca chạy thực tế của bộ đơn vị là 466 ca chạy. "
                  "Toàn bộ 466 ca kiểm thử đơn vị và 57 ca tích hợp đều vượt qua (tổng cộng 523/523 ca kiểm thử đạt tỷ lệ 100%).")
    print("Updated P2416: 417 tests, 466 runs, 523 total")

    # 10. Update Table 139 (Bảng 6.11 Kết quả chạy test backend)
    t139 = doc.tables[139]
    t139.rows[1].cells[1].text = "466"
    t139.rows[1].cells[2].text = "466"
    t139.rows[1].cells[5].text = "34 giây"
    t139.rows[2].cells[5].text = "46 giây"
    t139.rows[3].cells[1].text = "523"
    t139.rows[3].cells[2].text = "523"
    t139.rows[3].cells[5].text = "80 giây"
    print("Updated Table 139: Total backend tests to 523 (100% pass in 80s)")

    # 11. Update Abstracts and Key Summaries
    # P42 (Tóm tắt tiếng Việt)
    p42 = doc.paragraphs[42]
    p42.text = ("Về kiểm thử, mã nguồn có 417 test đơn vị (đếm [Fact] và [Theory], tương ứng 466 ca chạy thuộc 47 tệp), "
                "57 test tích hợp, 8 kịch bản E2E bằng Playwright và 2 kịch bản tải bằng k6. Kết quả thực thi: bộ test backend gồm 466 ca chạy đơn vị "
                "và 57 test tích hợp đều đạt (523/523, 80 giây); 8/8 kịch bản E2E đạt trong 100,09 giây; "
                "hai kịch bản k6 đạt ngưỡng đề ra với p95 tối đa 1.210 ms ở 50 người dùng ảo và tỷ lệ lỗi 0,00%. "
                "Nhờ đó các mục tiêu về không đặt trùng ghế, thanh toán an toàn và cập nhật thời gian thực được kiểm chứng bằng số liệu thực thi.")
    print("Updated P42 (Tóm tắt tiếng Việt)")

    # P48 (Abstract tiếng Anh)
    p48 = doc.paragraphs[48]
    p48.text = ("The source code contains 417 unit tests (counting [Fact] and [Theory], equal to 466 executed cases across 47 files), "
                "57 integration tests, 8 Playwright end-to-end scenarios and 2 k6 load scenarios. "
                "Execution results: the backend suites (466 unit-test cases and 57 integration tests) all passed (523 of 523, 80 seconds); "
                "8 of 8 end-to-end scenarios passed in 100.09 seconds; and both k6 load scenarios met their thresholds, "
                "with a maximum p95 latency of 1,210 ms at 50 virtual users and a 0.00% error rate. "
                "The objectives on seat double-booking prevention, payment safety and real-time updates are therefore supported by execution evidence.")
    print("Updated P48 (Abstract tiếng Anh)")

    # P2532 (Mục 7.2)
    p2532 = doc.paragraphs[2532]
    p2532.text = ("Bảng 7.2 đối chiếu năm mục tiêu MT1 đến MT5 (Bảng 1.1) với bằng chứng thực nghiệm cụ thể đã được kiểm chứng ở Chương 6. "
                  "Toàn bộ 523 test xUnit backend (466 unit test và 57 integration test), 8 kịch bản E2E Playwright và các kịch bản kiểm thử tải k6 "
                  "đều đã được thực thi và xác nhận đạt 100%.")
    print("Updated P2532 to 523 backend tests")

    # P2593 (Mục 8 - MT5)
    p2593 = doc.paragraphs[2593]
    p2593.text = ("MT5: Đạt (Bộ kiểm thử xUnit gồm 523 bài test đạt 100% trong 80 giây gồm 466 unit test và 57 integration test; "
                  "E2E Playwright đạt 8/8 kịch bản trong 100,09 giây; k6 đạt ngưỡng đã khai báo (p95 tối đa 1.210 ms ở 50 VU, lỗi 0,00%)).")
    print("Updated P2593 to 523 backend tests")

    # Table 3 (Bảng 1.1)
    t3 = doc.tables[3]
    t3.rows[5].cells[4].text = "Đạt (523/523 test xUnit pass 100% gồm 466 unit test và 57 integration test qua 55 file test; E2E Playwright và k6 đạt chỉ tiêu)"
    print("Updated Table 3 (Bảng 1.1)")

    # Table 20 (Yêu cầu phi chức năng)
    t20 = doc.tables[20]
    t20.rows[6].cells[2].text = "Mã nguồn phân tầng (Api, Application, Domain, Infrastructure, Shared) và nhận phụ thuộc qua giao diện; bộ kiểm thử tự động gồm 523 test xUnit (466 unit test và 57 integration test xUnit với WebApplicationFactory môi trường Testing tắt Hangfire và bộ giới hạn tốc độ, 55 file test class, pass 100%) và cấu hình Playwright cho E2E (frontend/playwright.config.ts)"
    print("Updated Table 20 (Yêu cầu phi chức năng)")

    # Table 158 (Phụ lục Ánh xạ)
    t158 = doc.tables[158]
    # Row 0: XT Xác thực
    t158.rows[0].cells[1].text = "AuthController, AuthService, RegistrationService, PasswordService, VerificationTokenSender, JwtTokenService"
    t158.rows[0].cells[2].text = "AuthServiceTests, RegistrationServiceTests, PasswordServiceTests, JwtTokenServiceTests, AuthValidatorsTests"
    # Row 13: QS Quản lý suất chiếu
    t158.rows[13].cells[1].text = "AdminShowtimesController, ShowtimesController, ShowtimeQueryService, ShowtimeBatchService, ShowtimeAutoScheduleService, ShowtimeCancellationService, ShowtimeLifecycleService, ShowtimeRules, ShowtimeScheduler"
    # Row 14: QN Quản lý người dùng
    t158.rows[14].cells[1].text = "AdminUsersController, AdminEmployeesController, UserAdminService, EmployeeService"
    t158.rows[14].cells[2].text = "UserAdminServiceUserStatusTests, EmployeeServiceRoleRulesTests, EmployeeServiceInviteTests (12 test)"
    print("Updated Table 158 (Phụ lục Ánh xạ)")

    # Table 159 & Table 161
    t159 = doc.tables[159]
    t159.rows[5].cells[1].text = "Bộ kiểm thử tự động gồm 523 test xUnit backend (466 unit test, 57 integration test qua 55 file test class); 8 kịch bản E2E Playwright; 2 kịch bản tải k6"
    print("Updated Table 159")

    t161 = doc.tables[161]
    t161.rows[5].cells[2].text = "523/523 test xUnit backend đạt 100% (466 unit test trong 34 s, 57 integration test trong 46 s); E2E Playwright đạt 8/8 kịch bản (100,09 s); kiểm thử tải k6 đạt ngưỡng đã khai báo (p95 tối đa 1.210 ms ở 50 VU, lỗi 0,00%)"
    print("Updated Table 161")

    doc.save(output_path)
    print(f"Successfully updated and saved document to {output_path}")

if __name__ == "__main__":
    update_report_v3("docs/bao-cao.docx")

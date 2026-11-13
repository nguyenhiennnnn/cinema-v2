import docx

def set_cell_text(cell, text, style='Nội dung bảng'):
    cell.text = text
    for p in cell.paragraphs:
        p.style = style

def update_report_v4(doc_path, output_path=None):
    if output_path is None:
        output_path = doc_path
    
    print(f"Loading {doc_path}...")
    doc = docx.Document(doc_path)
    print(f"Loaded {doc_path}. Paras: {len(doc.paragraphs)}, Tables: {len(doc.tables)}")

    # 1. P1113 - Mục 4.1.2 Backend Architecture DI registration
    p1113 = doc.paragraphs[1113]
    old_p1113 = p1113.text
    # Replace outdated BookingService registration mention with decomposed services
    target_booking_p1113 = "IBookingService (cùng IBookingHoldService, IBookingCheckoutService, IBookingQueryService, IBookingCancellationService, IBookingCounterService, IBookingCheckinService)"
    replacement_booking_p1113 = "các dịch vụ đặt vé phân rã chuyên biệt gồm IBookingHoldService, IBookingCheckoutService, IBookingQueryService, IBookingCancellationService, IBookingFulfillmentService, ICounterBookingService (cùng BookingSeatReleaser và các chính sách IBookingHoldPolicy)"
    if target_booking_p1113 in old_p1113:
        p1113.text = old_p1113.replace(target_booking_p1113, replacement_booking_p1113)
        print("Updated P1113: Refactored Booking service registrations")
    elif "IBookingService" in old_p1113 and "IBookingFulfillmentService" not in old_p1113:
        p1113.text = old_p1113.replace("IBookingService", "IBookingHoldService, IBookingCheckoutService, IBookingQueryService, IBookingCancellationService, IBookingFulfillmentService, ICounterBookingService")
        print("Updated P1113: Replaced IBookingService")

    # 2. P1219 - Mục 4.2 Package Bán vé tại quầy
    p1219 = doc.paragraphs[1219]
    p1219.text = (
        "Hình 4.12 thể hiện các controller tiếp nhận phụ thuộc các service chuyên biệt: "
        "AdminCounterController phụ thuộc ICounterBookingService (hiện thực bởi CounterBookingService xử lý giữ ghế, "
        "thanh toán và soát vé tại quầy), AdminBookingsController phụ thuộc IBookingQueryService; CounterBookingService "
        "phối hợp cùng BookingHoldService, BookingCheckoutService, BookingSeatReleaser, IAppDbContext và các thực thể "
        "Booking, BookingTicket, Payment, Employee, User; CounterHoldRequestValidator và CounterCheckoutRequestValidator "
        "kiểm tra hai yêu cầu của quầy. Ảnh mã QR của vé do QrCodeHelper sinh khi BookingMappingConfig ánh xạ vé sang DTO, "
        "còn nội dung mã QR do CodeGenerator.GenerateTicketQrPayload tạo."
    )
    print("Updated P1219: Package Bán vé tại quầy controllers and services")

    # 3. P1257 - Mục 4.2 Package Quản lý người dùng controllers
    p1257 = doc.paragraphs[1257]
    p1257.text = (
        "Hai controller được phân tách theo đúng trách nhiệm: AdminUsersController gọi IUserAdminService "
        "(hiện thực bởi UserAdminService phụ trách quản lý tài khoản người dùng, phân quyền, khóa tài khoản và tìm kiếm khách hàng cho quầy), "
        "còn AdminEmployeesController gọi IEmployeeService (hiện thực bởi EmployeeService phụ trách hồ sơ nhân viên, phân công chức vụ "
        "và quy trình gửi thư mời nhân viên qua email). Các service nhận IAppDbContext và IOptions<BookingOptions>, "
        "truy cập các thực thể User, Role, Employee, Position; lớp tiện ích BulkOperation phục vụ cập nhật trạng thái hàng loạt."
    )
    print("Updated P1257: Decoupled UserAdminService and EmployeeService")

    # 4. P1260 - Mục 4.2 Use case mapping for User/Employee
    p1260 = doc.paragraphs[1260]
    p1260.text = (
        "Các phương thức tương ứng với từng use case (xem Mục 3.3.15): "
        "UC-QN-01 “Xem danh sách người dùng” ứng với GetUsers → UserAdminService.GetUsersAdminAsync; "
        "UC-QN-02 “Xem danh sách tài khoản quản trị” ứng với GetAdminUsers → UserAdminService.GetAdminUsersForSelectAsync; "
        "UC-QN-03 “Tìm kiếm khách hàng” ứng với SearchCustomers → UserAdminService.SearchCustomersForCounterAsync; "
        "UC-QN-04 “Khóa hoặc mở khóa tài khoản” ứng với UpdateStatus → UserAdminService.UpdateUserStatusAsync; "
        "UC-QN-05 “Đổi vai trò tài khoản” ứng với UpdateRole → UserAdminService.UpdateUserRoleAsync; "
        "UC-QN-06 “Cập nhật trạng thái nhiều tài khoản” ứng với BulkUpdateStatus → UserAdminService.BulkUpdateUserStatusAsync; "
        "UC-QN-07 “Quản lý nhân viên” ứng với GetEmployees, Create, Update, Invite của AdminEmployeesController → "
        "EmployeeService.GetEmployeesAdminAsync, CreateEmployeeAsync, UpdateEmployeeAsync, InviteEmployeeAsync."
    )
    print("Updated P1260: UserAdminService and EmployeeService use case methods")

    # 5. P1299 - Mục 4.2 Shared services
    p1299 = doc.paragraphs[1299]
    p1299.text = (
        "Một số lớp dịch vụ được dùng chung giữa nhiều package. Khối dịch vụ Booking "
        "(BookingHoldService, BookingCheckoutService, BookingQueryService, BookingCancellationService, CounterBookingService, BookingFulfillmentService) "
        "phục vụ các package Đặt vé trực tuyến, Thanh toán trực tuyến, Bán vé tại quầy, Soát vé và Quản lý booking; "
        "ShowtimeService cùng các service chuyên biệt (ShowtimeQueryService, ShowtimeBatchService, ShowtimeAutoScheduleService, ShowtimeCancellationService, ShowtimeLifecycleService) "
        "phục vụ Duyệt phim, Đặt vé trực tuyến, Quản lý suất chiếu và Vận hành hệ thống; ReviewService phục vụ Hồ sơ cá nhân, "
        "Duyệt phim, Đánh giá phim và Quản lý đánh giá; CatalogService phục vụ Duyệt phim và Quản lý danh mục; "
        "ComboService phục vụ Duyệt phim, Đặt vé trực tuyến và Quản lý danh mục; EmployeeService và UserAdminService phục vụ Bán vé tại quầy "
        "và Quản lý người dùng; MovieService phục vụ Duyệt phim và Quản lý phim. Vì vậy, mỗi interface dùng chung được trình bày "
        "ở Hình lớp của từng package theo phần phương thức package sử dụng, còn danh sách đầy đủ phương thức của các interface xem trong mã nguồn."
    )
    print("Updated P1299: Shared services descriptions")

    # 6. P1638 - Mục 4.4 Identity rules
    p1638 = doc.paragraphs[1638]
    p1638.text = (
        "Bên cạnh việc thu hồi token, UserAdminService và EmployeeService thiết lập các quy tắc nghiệp vụ bảo vệ an toàn danh tính: "
        "ngăn chặn người dùng tự khóa tài khoản của chính mình (Rule_CannotLockSelf), tự đổi vai trò của chính mình (Rule_CannotChangeOwnRole), "
        "tự vô hiệu hóa hồ sơ nhân viên của chính mình (Rule_CannotDeactivateSelfEmployee), đồng thời bảo vệ không cho phép khóa hoặc hạ vai trò "
        "của Quản trị viên đang hoạt động cuối cùng trong hệ thống (Rule_CannotLockLastAdmin, Rule_CannotDemoteLastAdmin dựa trên CountOtherActiveAdminsAsync)."
    )
    print("Updated P1638: UserAdminService and EmployeeService identity rules")

    # 7. P2019 - Mục 5.3.7 Module đặt vé
    p2019 = doc.paragraphs[2019]
    p2019.text = (
        "Module cung cấp giữ ghế, thanh toán (checkout), hủy vé có hoàn tiền, tra cứu đơn vé, bán vé tại quầy và soát vé. "
        "Để tuân thủ chặt chẽ nguyên lý đơn trách nhiệm (SRP) và phân tách giao diện (ISP), khối nghiệp vụ đặt vé "
        "(ban đầu tập trung trong BookingService quy mô 1.447 dòng) đã được phân rã thành các service chuyên trách trong "
        "Cinema.Application/Services/Bookings: BookingHoldService (IBookingHoldService, quản lý giữ ghế trực tuyến và giải phóng ghế), "
        "BookingCheckoutService (IBookingCheckoutService, tính toán tổng tiền, chiết khấu hạng thành viên và tạo phiên thanh toán), "
        "BookingQueryService (IBookingQueryService, tra cứu đơn cá nhân và quản trị), "
        "BookingCancellationService (IBookingCancellationService, xử lý hủy vé và hoàn tiền theo chính sách), "
        "CounterBookingService (ICounterBookingService, xử lý giữ ghế quầy, thanh toán tiền mặt và soát vé check-in tại rạp), "
        "và BookingFulfillmentService (IBookingFulfillmentService, hoàn tất đơn vé khi thanh toán thành công), "
        "cùng BookingSeatReleaser và các chính sách thời gian giữ ghế IBookingHoldPolicy (OnlineHoldPolicy, CounterHoldPolicy). "
        "Phía tiếp nhận API được phân tách rõ ràng giữa BookingsController (dành cho khách hàng), "
        "AdminBookingsController (quản trị tra cứu danh sách đơn) và AdminCounterController (dành cho nhân viên quầy và nhân viên soát vé). "
        "Luồng nghiệp vụ được thiết kế ở Chương 4 (xem Hình 4.51 cho luồng phát hành vé, Hình 4.54 cho luồng hủy vé)."
    )
    print("Updated P2019: Decomposed Booking module services")

    # 8. P2389 - Mục 6.2.1 Nhóm Xác thực (56 tests)
    p2389 = doc.paragraphs[2389]
    p2389.text = (
        "Nhóm kiểm thử xác thực bao quát toàn diện quy trình bảo mật tài khoản với 56 test: "
        "AuthServiceTests (19 test bao quát đăng nhập mật khẩu, cổng quản trị, Google OAuth, xoay vòng và thu hồi refresh token), "
        "RegistrationServiceTests (7 test kiểm tra đăng ký tài khoản, phát hành và xác nhận email), "
        "PasswordServiceTests (6 test kiểm tra đổi mật khẩu, quên mật khẩu, đặt lại mật khẩu với token và thu hồi chuỗi phiên đăng nhập), "
        "JwtTokenServiceTests (9 test kiểm tra tính chất claim, hạn 5 phút của socket token), "
        "và AuthValidatorsTests (15 test trong đó có 3 [Theory] kiểm tra quy tắc hợp lệ của dữ liệu đầu vào)."
    )
    print("Updated P2389: Exact test counts for Auth group (56 tests)")

    # 9. P2392 - Mục 6.2.2 Nhóm Đặt vé (98 tests)
    p2392 = doc.paragraphs[2392]
    p2392.text = (
        "Nhóm gồm 98 test: BookingCancellationServiceTests 4 test; BookingCheckoutServiceTests 5 test; "
        "BookingFulfillmentServiceTests 6 test; BookingHoldServiceTests 6 test; BookingServiceCounterCheckinTests 18 test; "
        "BookingServiceLoyaltyTests 4 test; BookingServiceSeatHoldTests 15 test (trong đó 1 [Theory]); "
        "CounterBookingServiceCancelTests 1 test; LoyaltyWindowTests 19 test; BookingValidatorsTests 20 test (trong đó 2 [Theory]). "
        "Nhóm này bao quát giữ ghế trực tuyến và tại quầy, thanh toán tiền mặt tại quầy, soát vé check-in, "
        "hoàn tất đơn khi thanh toán qua cổng, xác nhận booking và sinh vé, hủy vé và hoàn tiền theo chính sách, "
        "xử lý thanh toán thất bại, cùng các quy tắc điểm thành viên và kiểm tra tính hợp lệ của dữ liệu đầu vào. "
        "Các file BookingFulfillmentServiceTests, BookingServiceLoyaltyTests và BookingServiceCounterCheckinTests "
        "kiểm tra các quy tắc điểm thành viên: giảm giá chỉ tính trên tiền vé, điểm hiệu lực lấy từ sổ điểm trong cửa sổ trượt 12 tháng, "
        "thu hồi điểm vào đúng dòng sổ điểm của booking và tính lại hạng sau thu hồi. File LoyaltyWindowTests.cs gồm ba lớp: "
        "LoyaltyDiscountTests (8 test về giảm giá chỉ tính trên tiền vé và hạng xét theo điểm trong cửa sổ trượt 12 tháng), "
        "LoyaltyLedgerTests (6 test về ghi sổ điểm khi xác nhận booking và thu hồi theo từng booking) và "
        "LoyaltyExpiryServiceTests (5 test về việc RecalculateAllAsync loại điểm hết hạn, hạ hạng và không đổi gì ở lần chạy thứ hai). "
        "Lớp hỗ trợ TestSupport/LoyaltyTestHelper.cs thay các hạng seed bằng ba hạng cố định (Bronze 0 điểm/0%, Silver 100 điểm/5%, Gold 200 điểm/10%), "
        "tạo booking Confirmed tối thiểu và thêm dòng sổ điểm với thời điểm EarnedAt tùy ý, giúp các test dựng được điểm nằm trong hoặc ngoài cửa sổ 12 tháng."
    )
    print("Updated P2392: Exact test counts for Booking group (98 tests across decomposed services)")

    # 10. P2410 - Mục 6.2.8 Nhóm Cấu hình, đánh giá, nhân viên (60 tests)
    p2410 = doc.paragraphs[2410]
    p2410.text = (
        "Nhóm gồm 60 test: CatalogServiceQuickCreateTests 8 test; ProductionOptionsValidatorTests 5 test; "
        "UserAdminServiceUserStatusTests 2 test; EmployeeServiceRoleRulesTests 6 test; EmployeeServiceInviteTests 12 test; "
        "ReviewServiceTests 2 test; ReviewValidatorsTests 3 test (trong đó 2 [Theory]); CinemaSettingsPolicyTests 10 test; "
        "SettingsServiceTests 12 test (trong đó 2 [Theory]). Nhóm gồm kiểm tra cấu hình production (ProductionOptionsValidator), "
        "kiểm tra tạo nhanh diễn viên/đạo diễn và chuẩn hóa tên (CatalogServiceQuickCreateTests), "
        "quản lý tài khoản người dùng và nhân viên (khóa tài khoản, đổi vai trò, bảo vệ quản trị viên hoạt động cuối cùng, "
        "quy trình mời nhân viên qua email và xử lý token mời), đánh giá phim và cập nhật điểm trung bình, "
        "cùng chính sách cấu hình rạp (giới hạn đặt vé, cửa sổ điểm thành viên, hạn hủy và tỷ lệ hoàn tiền, chế độ khoảng trống ghế, giờ hoạt động)."
    )
    print("Updated P2410: Exact test counts for Config/Employee group (60 tests)")

    # 11. P2413 - Mục 6.2.9 Nhóm Tiện ích (49 tests)
    p2413 = doc.paragraphs[2413]
    p2413.text = (
        "Nhóm gồm 49 test: NameNormalizerTests 5 test (trong đó 3 [Theory] với 13 ca dữ liệu); "
        "CodeGeneratorTests 5 test (trong đó 1 [Theory]); DateTimeUtilsTests 9 test (trong đó 1 [Theory]); "
        "DateTimeJsonConverterTests 4 test; BusinessTimeTests 3 test; EmailTemplatesTests 7 test; "
        "EnumConvertersTests 10 test (trong đó 3 [Theory]); VnPayGatewayTests 6 test (trong đó 1 [Theory]). "
        "Các test kiểm tra chuẩn hóa chuỗi họ tên tiếng Việt và dấu thanh (NameNormalizerTests), "
        "hàm sinh mã (mã booking, nội dung QR, mã giao dịch thanh toán), xử lý ngày giờ và múi giờ nghiệp vụ Việt Nam "
        "(DateTimeUtilsTests, BusinessTimeTests, DateTimeJsonConverterTests), mẫu email HTML, các bộ chuyển đổi enum "
        "và tạo chữ ký/xác thực URL cổng thanh toán VNPay."
    )
    print("Updated P2413: Exact test counts for Utils group (49 tests)")

    # 12. Table 20 - NFR-02 source and test methods
    t20 = doc.tables[20]
    t20.rows[2].cells[2].text = (
        "backend/Cinema.Application/Services/Bookings/BookingHoldService.cs\n"
        "backend/Cinema.Infrastructure/Persistence/Configurations/*Configuration.cs\n"
        "backend/Cinema.Api/Middlewares/ExceptionHandlingMiddleware.cs"
    )
    t20.rows[2].cells[4].text = (
        "Test HoldSeat_HighConcurrencyParallelRequests_OnlyOneWinsAndOthersGet409 (BookingFlowEndpointsTests), "
        "BookingHoldServiceTests, BookingServiceSeatHoldTests, SeatExpirationServiceTests"
    )
    for p in t20.rows[2].cells[2].paragraphs:
        p.style = 'Nội dung bảng'
    for p in t20.rows[2].cells[4].paragraphs:
        p.style = 'Nội dung bảng'
    print("Updated Table 20 (NFR-02): Updated source and test references to BookingHoldService")

    # 13. Table 158 - Fix shifted rows bug and update implementations/tests
    t158 = doc.tables[158]
    # Row 0: Restore Header
    set_cell_text(t158.rows[0].cells[0], "Package")
    set_cell_text(t158.rows[0].cells[1], "Cài đặt mã nguồn")
    set_cell_text(t158.rows[0].cells[2], "Test đơn vị")
    set_cell_text(t158.rows[0].cells[3], "Test tích hợp")
    set_cell_text(t158.rows[0].cells[4], "E2E")

    # Row 1: XT Xác thực
    set_cell_text(t158.rows[1].cells[1], "AuthController, AuthService, RegistrationService, PasswordService, VerificationTokenSender, JwtTokenService")
    set_cell_text(t158.rows[1].cells[2], "AuthServiceTests, RegistrationServiceTests, PasswordServiceTests, JwtTokenServiceTests, AuthValidatorsTests")

    # Row 5: DV Đặt vé trực tuyến
    set_cell_text(t158.rows[5].cells[1], "BookingsController, BookingHoldService, BookingCheckoutService, BookingCancellationService, BookingQueryService")
    set_cell_text(t158.rows[5].cells[2], "BookingHoldServiceTests, BookingCheckoutServiceTests, BookingCancellationServiceTests, BookingServiceSeatHoldTests, BookingServiceLoyaltyTests, BookingOnlineValidators")

    # Row 8: BQ Bán vé tại quầy
    set_cell_text(t158.rows[8].cells[1], "AdminCounterController, AdminBookingsController, CounterBookingService, BookingHoldService, BookingCheckoutService")
    set_cell_text(t158.rows[8].cells[2], "BookingServiceCounterCheckinTests, CounterBookingServiceCancelTests, BookingCounterValidators")

    # Row 9: SV Soát vé
    set_cell_text(t158.rows[9].cells[1], "AdminCounterController, CounterBookingService")
    set_cell_text(t158.rows[9].cells[2], "BookingServiceCounterCheckinTests")

    # Row 13: QR Quản lý phòng chiếu (RESTORED from mistaken showtimes overwrite!)
    set_cell_text(t158.rows[13].cells[1], "AdminRoomsController, RoomService")
    set_cell_text(t158.rows[13].cells[2], "RoomServiceSeatLayoutTests, SeatLayoutRulesTests")

    # Row 14: QS Quản lý suất chiếu (RESTORED from mistaken useradmin overwrite!)
    set_cell_text(t158.rows[14].cells[1], "AdminShowtimesController, ShowtimesController, ShowtimeQueryService, ShowtimeBatchService, ShowtimeAutoScheduleService, ShowtimeCancellationService, ShowtimeLifecycleService, ShowtimeRules, ShowtimeScheduler")
    set_cell_text(t158.rows[14].cells[2], "ShowtimeServiceCreateTests, ShowtimeServiceAutoScheduleTests, ShowtimeServiceFixesTests, ShowtimeSchedulerTests, ShowtimeLifecycleTests, OperatingHoursTests, ShowtimeValidatorsTests")

    # Row 15: QN Quản lý người dùng
    set_cell_text(t158.rows[15].cells[1], "AdminUsersController, AdminEmployeesController, UserAdminService, EmployeeService")
    set_cell_text(t158.rows[15].cells[2], "UserAdminServiceUserStatusTests, EmployeeServiceRoleRulesTests, EmployeeServiceInviteTests (12 test)")
    print("Updated Table 158: Fixed row alignment, restored headers and updated modular services")

    # 14. Table 159 - Update MT5 cell 2
    t159 = doc.tables[159]
    set_cell_text(
        t159.rows[5].cells[2],
        "Đạt (523/523 test xUnit backend đạt 100% trong 80 giây gồm 466 unit test và 57 integration test; E2E đạt 8/8 kịch bản trong 100,09 giây; k6 đạt ngưỡng đã khai báo (p95 tối đa 1.210 ms ở 50 VU, lỗi 0,00%))"
    )
    print("Updated Table 159 (MT5 result)")

    # 15. Table 163 - Admin Counter Controller mapping
    t163 = doc.tables[163]
    # Row 10: /api/v1/admin/bookings/counter-hold -> AdminCounter
    set_cell_text(t163.rows[10].cells[1], "AdminCounter")
    # Row 13: /api/v1/admin/bookings/{bookingId:int}/counter-checkout -> AdminCounter
    set_cell_text(t163.rows[13].cells[1], "AdminCounter")
    # Row 14: /api/v1/admin/bookings/checkin -> AdminCounter
    set_cell_text(t163.rows[14].cells[1], "AdminCounter")
    print("Updated Table 163 (Admin Counter Controller endpoints)")

    doc.save(output_path)
    print(f"Document successfully updated and saved to: {output_path}")

if __name__ == "__main__":
    update_report_v4("docs/bao-cao.docx")

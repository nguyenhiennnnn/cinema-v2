# CHƯƠNG 7. ĐÁNH GIÁ KẾT QUẢ

Chương này đánh giá kết quả của đề tài ở ba khía cạnh: truy vết từng package use case đến mã cài đặt và các bộ kiểm thử (Mục 7.1), đối chiếu mức đạt của năm mục tiêu đã đặt ra ở Bảng 1.1 (Mục 7.2) và thảo luận các hạn chế còn lại của hệ thống cùng hướng khắc phục (Mục 7.3). Chương chỉ khẳng định những kết quả có căn cứ ở Chương 6; phần chưa có số liệu chạy thực tế được đánh dấu rõ. Các hạn chế ở Mục 7.3 đều được đối chiếu lại với mã nguồn hiện tại; hạn chế đã được sửa được nêu là đã khắc phục.

## 7.1 TRUY VẾT YÊU CẦU

### 7.1.1 Truy vết use case đến kiểm thử

Bảng 7.1 truy vết 21 package use case của Chương 3 (Bảng 3.2) đến thành phần cài đặt và đến từng tầng kiểm thử của Chương 6. Cột "Cài đặt" nêu controller và dịch vụ chính trong `backend/`; các cột kiểm thử nêu tên lớp test có trong `backend/tests/` và kịch bản E2E trong `frontend/e2e/` (mã E2E-01 đến E2E-08 theo Bảng 6.14). Ô ghi "chưa có test" nghĩa là không tìm thấy lớp test nào tham chiếu đến thành phần cài đặt tương ứng. Một test chỉ được ghi cho package khi nó kiểm tra đúng hành vi của package đó; lớp test dùng chung được ghi ở mọi package mà nó chạm tới.

**Bảng 7.1.** Truy vết package use case đến kiểm thử

| Package | Cài đặt | Test đơn vị | Test tích hợp | E2E |
|---|---|---|---|---|
| XT Xác thực | `AuthController`, `AuthService`, `JwtTokenService` | `AuthServiceTests`, `JwtTokenServiceTests`, `AuthValidatorsTests` | `AuthEndpointsTests` | Gián tiếp: đăng nhập là bước tiền đề của E2E-01 đến E2E-08 (`loginAsCustomer`, `loginAsAdmin`) |
| HS Hồ sơ cá nhân | `UsersController`, `UserProfileService` | Chưa có test | Chưa có test | E2E-05 |
| DP Duyệt phim | `MoviesController`, `ShowtimesController`, `CatalogController`, `SettingsController` | Chưa có test | Chưa có test | E2E-01 (chọn phim, suất) |
| DG Đánh giá phim | `ReviewService` | `ReviewServiceTests` (2 test tạo đánh giá), `ReviewValidatorsTests` | Chưa có test phía khách hàng | Chưa có |
| DV Đặt vé trực tuyến | `BookingsController`, `BookingService` | `BookingServiceTests`, `BookingServiceSeatHoldTests`, `BookingServiceLoyaltyTests`, `BookingValidatorsTests` | `BookingFlowEndpointsTests` | E2E-01, E2E-04 |
| TT Thanh toán trực tuyến | `PaymentsController`, `PaymentService`, `VnPayGateway`, `MomoGateway`, `StripeGateway` | `PaymentServiceVnPayTests`, `PaymentServiceMomoTests`, `PaymentServiceStripeTests` | `BookingFlowEndpointsTests` (cổng giả `FakePaymentGateway`, IPN VNPay giả) | E2E-02, E2E-03 (trang kết quả; không đi qua cổng thật) |
| AI Trợ lý AI | `AiController`, `GeminiAiAssistantService` | Chưa có test | Chưa có test | Chưa có |
| BQ Bán vé tại quầy | `AdminBookingsController`, `BookingService` | `BookingServiceCounterCheckinTests` | `AdminBookingsEndpointsTests` | Không có E2E qua giao diện; `helpers.ts` gọi API `counter-hold`, `counter-checkout` để chuẩn bị dữ liệu cho E2E-06 |
| SV Soát vé | `AdminBookingsController`, `BookingService` | `BookingServiceCounterCheckinTests` | `AdminBookingsEndpointsTests` | E2E-06 |
| QB Quản lý booking | `AdminBookingsController` | Chưa có test riêng cho tra cứu | `AdminBookingsEndpointsTests` (các test hiện có thuộc bán quầy và soát vé, chưa có test tra cứu) | Chưa có |
| QP Quản lý phim | `AdminMoviesController`, `MovieService` | Chưa có test | `AdminMoviesBulkEndpointsTests` (xóa hàng loạt) | E2E-07 |
| QD Quản lý danh mục | `Admin*Controller` của diễn viên, đạo diễn, thể loại, ngôn ngữ, độ tuổi, loại màn hình, loại ghế, chức vụ, combo, hạng thành viên; `CatalogService`, `ComboService` | Chưa có test | Chưa có test | Chưa có |
| QR Quản lý phòng chiếu | `AdminRoomsController`, `RoomService` | `RoomServiceSeatLayoutTests`, `SeatLayoutRulesTests` | `AdminRoomsEndpointsTests` | Chưa có |
| QS Quản lý suất chiếu | `AdminShowtimesController`, `ShowtimeService`, `ShowtimeScheduler` | `ShowtimeServiceCreateTests`, `ShowtimeServiceAutoScheduleTests`, `ShowtimeServiceFixesTests`, `ShowtimeSchedulerTests`, `ShowtimeLifecycleTests`, `OperatingHoursTests`, `ShowtimeValidatorsTests` | `AdminShowtimesBulkEndpointsTests` | E2E-08 |
| QN Quản lý người dùng | `AdminUsersController`, `AdminEmployeesController`, `EmployeeService` | `EmployeeServiceUserStatusTests`, `EmployeeServiceRoleRulesTests` | Chưa có test | Chưa có |
| QV Quản lý đánh giá | `AdminReviewsController`, `ReviewService` | `ReviewServiceTests` | `AdminReviewsBulkEndpointsTests` | Chưa có |
| QT Quản lý thanh toán | `AdminPaymentsController`, `PaymentAdminService` | Chưa có test | Chưa có test | Chưa có |
| BC Báo cáo thống kê | `AdminReportsController`, `AdminDashboardController`, `ReportService`, `DashboardService` | `ReportServiceTests`, `DashboardServiceTests` | Chưa có test | Chưa có |
| CH Cấu hình rạp | `AdminSettingsController`, `SettingsService` | `SettingsServiceTests`, `CinemaSettingsPolicyTests`, `ProductionOptionsValidatorTests` (cấu hình Production) | Chưa có test | Chưa có |
| TP Tải tệp | `AdminUploadsController`, `CloudinaryFileStorage` | Chưa có test | `AuthEndpointsTests` chỉ gọi `/admin/uploads/image` để kiểm tra phân quyền (401, 403), không kiểm tra việc tải tệp | Chưa có |
| HT Vận hành hệ thống | `ReleaseExpiredSeatsJob`, `ShowtimeLifecycleJob`, `ExpireLoyaltyPointsJob`, `ShowtimeRefundJob`, `SendEmailJob`, `SeatExpirationService`, `LoyaltyExpiryService` | `SeatExpirationServiceTests`, `ShowtimeLifecycleTests`, `ShowtimeServiceFixesTests` (`ShowtimeRefundJob`), `LoyaltyWindowTests` (`LoyaltyExpiryService`), `EmailTemplatesTests` | `SignalRShowtimeHubTests` (thời gian thực) | E2E-04 (gián tiếp, ép hết hạn giữ ghế) |

Ma trận cho thấy bốn mức bao phủ. Các package có đủ ba tầng đơn vị, tích hợp và E2E là DV, QS và phần lớn luồng của SV. Các package có test đơn vị và tích hợp nhưng chưa có E2E là TT (E2E chỉ kiểm tra trang kết quả), QR, QV và HT. Các package chỉ có một tầng kiểm thử gồm DG, BQ, QN, BC, CH. Ba package không có test nào gọi trực tiếp vào thành phần cài đặt là AI, QD và QT; riêng HS, DP và QP chỉ được kiểm tra gián tiếp qua E2E hoặc qua thao tác xóa hàng loạt. Các lớp công việc nền `ReleaseExpiredSeatsJob`, `ShowtimeLifecycleJob` và `ExpireLoyaltyPointsJob` không được test gọi trực tiếp; logic bên dưới được kiểm tra qua `SeatExpirationService`, `ShowtimeService` và `LoyaltyExpiryService`. Bảng 7.1 phản ánh việc có test hay không, không phản ánh test có chạy đạt hay không, vì Chương 6 chưa có log chạy (Mục 6.2.10).

## 7.2 ĐỐI CHIẾU MỤC TIÊU

### 7.2.1 Mức đạt từng mục tiêu

Bảng 7.2 đối chiếu năm mục tiêu MT1 đến MT5 (Bảng 1.1) với bằng chứng ở Chương 6. Mục này chỉ khẳng định mức đạt khi Chương 6 có số liệu chạy; khi mới có test được viết mà chưa có kết quả chạy, mức đạt để trống và cần bổ sung. Mã nguồn xác nhận các test tồn tại và các phép kiểm tra (assertion) mà chúng đặt ra, nhưng một test tồn tại không đồng nghĩa với việc nó đã được chạy và đạt.

**Bảng 7.2.** Mức đạt mục tiêu

| Mục tiêu | Bằng chứng | Mức đạt | Mục tham chiếu |
|---|---|---|---|
| MT1 Đủ chức năng | 103 use case ở Chương 3 (Bảng 3.2); module đã cài đặt ở Chương 5; Bảng 7.1 xác nhận mỗi package đều có controller và dịch vụ tương ứng trong mã nguồn | [CẦN BỔ SUNG: đối chiếu từng trong 103 use case với mã cài đặt để có tỷ lệ use case đã cài đặt; hiện mới đối chiếu ở mức package] | 3.3, 5.3 |
| MT2 Không đặt trùng ghế | Cơ chế chiếm ghế bằng một câu lệnh `ExecuteUpdateAsync` có điều kiện trong `CreateHoldBookingAsync`; test `HoldSeat_HighConcurrencyParallelRequests_OnlyOneWinsAndOthersGet409` (10 yêu cầu song song, kỳ vọng 1 phản hồi 201 và 9 phản hồi 409) | [CẦN BỔ SUNG: kết quả chạy thật của test; Chương 6 chưa có log nên Bảng 6.26 còn trống] | 6.5 |
| MT3 Thanh toán an toàn | Test đơn vị `PaymentServiceVnPayTests` (8), `PaymentServiceMomoTests` (8), `PaymentServiceStripeTests` (16); test tích hợp luồng IPN VNPay giả trong `BookingFlowEndpointsTests` | [CẦN BỔ SUNG: kết quả chạy; số lượng test chưa phải số test đạt] | 6.2 |
| MT4 Thời gian thực | `SignalRShowtimeHubTests` (3 test: `seat.held`, `seat.released`, không nhận sự kiện sau `LeaveShowtime`) | [CẦN BỔ SUNG: kết quả chạy; bộ test hiện chỉ kiểm tra hai sự kiện `seat.held` và `seat.released`] | 6.3 |
| MT5 Chất lượng kiểm thử | Mã nguồn có 364 test đơn vị, 49 test tích hợp, 8 kịch bản E2E và 2 kịch bản k6 (Bảng 6.1); báo cáo E2E ngày 07/09/2026 ghi 8/8 kịch bản đạt (Mục 6.4.11) | Một phần: có bộ test với số lượng đã đếm; E2E có một báo cáo đạt nhưng chưa đối chiếu được với báo cáo gốc, trong khi `test-results/.last-run.json` trong kho mã ghi một lần chạy thất bại (E2E-06). [CẦN BỔ SUNG: log `dotnet test`, độ bao phủ và kết quả k6] | 6.2 đến 6.6 |

Từ Bảng 7.2 có thể rút ra hai nhận xét. Thứ nhất, về mặt thiết kế, các cơ chế phục vụ MT2, MT3 và MT4 đều có mã và test tương ứng, nên hướng kiểm chứng đã rõ; điều còn thiếu là bằng chứng thực thi. Thứ hai, cột "Kết quả" của Bảng 1.1 đang ghi mức đạt cụ thể hơn những gì Chương 6 chứng minh được, và cần được điều chỉnh theo Bảng 7.2 khi hoàn thiện báo cáo.

## 7.3 THẢO LUẬN HẠN CHẾ

Mỗi hạn chế dưới đây được trình bày theo ba phần: bằng chứng trong mã nguồn hiện tại, tác động có thể xảy ra và đề xuất khắc phục. Tác động được nêu là khả năng, không khẳng định đã xảy ra trong thực tế vận hành. Mỗi bằng chứng đã được mở lại ở file và hàm nêu tên; hạn chế đã được sửa trong mã nguồn được ghi rõ.

### 7.3.1 Chống trùng suất chiếu chỉ ở tầng ứng dụng

Việc chống hai suất chiếu chồng giờ trong cùng một phòng được thực hiện trong `ShowtimeService`: hàm `EnsureNoOverlapAsync` gọi `HasOverlapAsync`, hàm này truy vấn các suất `Scheduled` cùng `RoomId` có `EndTime > startTime` và `StartTime < endTime`. `ExceptionHandlingMiddleware.cs` có nhánh xử lý lỗi PostgreSQL với `SqlState` là `23P01` và so sánh tên ràng buộc `EX_Showtimes_Room_NoOverlap`, nhưng tên ràng buộc này không xuất hiện ở bất kỳ file nào khác của kho mã: không có trong `Migrations`, trong `ShowtimeConfiguration.cs` hay trong tệp SQL nào. Nói cách khác, middleware chuẩn bị xử lý một ràng buộc loại trừ mà cơ sở dữ liệu hiện không có.

Vì bước kiểm tra và bước ghi là hai thao tác tách rời, hai yêu cầu tạo suất đồng thời cho cùng phòng và khung giờ có thể cùng vượt qua bước kiểm tra trước khi một trong hai được lưu. Hướng khắc phục là thêm ràng buộc loại trừ (exclusion constraint) ở mức cơ sở dữ liệu trên `RoomId` và khoảng thời gian của suất chiếu, đúng với tên mà middleware đã tham chiếu, để cơ sở dữ liệu là nơi bảo đảm cuối cùng.

### 7.3.2 Ghế hết hạn chỉ được giải phóng bởi job nền

Trong `SeatExpirationService`, `ReleaseStaleHeldSeatsAsync` chuyển các ghế `Held` có `HeldUntil` nhỏ hơn thời điểm hiện tại về `Available`. Phương thức này chỉ được gọi từ `ExpireDueBookingsAsync`, và `ExpireDueBookingsAsync` chỉ được gọi từ `ReleaseExpiredSeatsJob`, chạy theo `BackgroundJobs:ReleaseExpiredSeatsCron` (mặc định `* * * * *`, mỗi phút). Hàm `ShowtimeService.GetShowtimeSeatsAsync` trả trạng thái ghế bằng `ss.Status.ToString()` đúng như đang lưu, không xét `HeldUntil`. Câu lệnh chiếm ghế trong `CreateHoldBookingAsync` chỉ cho phép chiếm lại ghế `Held` đã hết hạn khi `HeldByUserId` là chính người đang yêu cầu.

Do đó ghế đã quá hạn giữ vẫn có thể hiển thị là `Held`, và người khác không giữ được ghế đó, cho đến khi job chạy lần kế tiếp. Khả năng ảnh hưởng lớn hơn khi tiến trình ứng dụng không chạy liên tục, chẳng hạn khi dịch vụ gói miễn phí tạm ngừng; [CẦN BỔ SUNG: kiểm chứng hành vi của dịch vụ trên môi trường triển khai]. Hướng khắc phục là coi ghế quá hạn là trống ngay khi truy vấn danh sách ghế và khi chiếm ghế (thêm điều kiện `HeldUntil < now` cho mọi người giữ), để tính đúng đắn không phụ thuộc vào chu kỳ của job.

### 7.3.3 Thành phần đánh giá trong Heat dùng thang sai

Hạn chế này đã được khắc phục trong mã nguồn hiện tại. Hàm `ShowtimeScheduler.Heat` nay chia điểm đánh giá cho `MaxRating` (`cfg.HeatRatingWeight * rating / maxRating`) và kẹp kết quả trong đoạn từ 0 đến 1. `MaxRating` có mặc định 5 trong `SchedulerOptions`, được `ShowtimeService` ghi đè bằng `CinemaSettings.SchedulerMaxRating`, và validator của cấu hình giới hạn giá trị trong khoảng 1 đến 10. Điều này phù hợp với ràng buộc `CK_Movies_AverageRating` (0 đến 5). Test `Heat_CalculatesHeatCorrectlyOn1To5RatingScale` trong `ShowtimeSchedulerTests` kiểm tra bốn trường hợp, chẳng hạn điểm 5 với doanh số bằng 0 cho độ nóng 0,3, đúng bằng trọng số đánh giá mặc định. Vì vậy trọng số đánh giá không còn bị giảm một nửa. Mục này được giữ lại trong danh sách để Bảng 7.3 khớp với dàn ý, với trạng thái đã khắc phục.

### 7.3.4 Seed dữ liệu demo chạy ở mọi môi trường

Trong `Program.cs`, nếu cấu hình `RunMigrationsOnStartup` là `true` thì ứng dụng gọi `MigrateAsync` rồi `DbInitializer.SeedAsync`. `SeedAsync` luôn tạo các tài khoản kiểm thử (ví dụ `admin@cinema.local`, `customer@cinema.local`), phòng bổ sung và, mặc định (`seedDemoScenarios = true`), các kịch bản demo gồm suất chiếu, booking, thanh toán và đánh giá; hàm không kiểm tra môi trường chạy. `render.yaml` đặt `ASPNETCORE_ENVIRONMENT` là `Production` và `RunMigrationsOnStartup` là `true`. Tệp `appsettings.json` đặt mặc định `false`, nên tình huống trên là do cấu hình triển khai.

Tác động có thể xảy ra là tài khoản và dữ liệu demo tồn tại ở môi trường Production. Hướng khắc phục là tách phần seed demo khỏi Production, chẳng hạn chỉ gọi `SeedAsync` khi môi trường không phải Production hoặc khi có cờ cấu hình riêng, và chỉ giữ lại dữ liệu nền bắt buộc như vai trò và hạng thành viên.

### 7.3.5 Refresh token lưu localStorage, chưa có CSP

Trong `frontend/src/stores/auth-store.ts`, đối tượng `refreshTokenStorage` đọc, ghi và xóa refresh token bằng `localStorage` với khóa `perfo_refresh_token`; access token chỉ nằm trong trạng thái của store. Tệp `vercel.json` ở thư mục gốc của kho mã khai báo các tiêu đề `Cache-Control`, `X-Robots-Tag`, `X-Content-Type-Options: nosniff` và `Referrer-Policy`, nhưng không có `Content-Security-Policy`; không tìm thấy khai báo CSP ở nơi nào khác của kho mã.

Nếu xuất hiện lỗ hổng XSS, mã độc chạy trong trang có thể đọc `localStorage` và đánh cắp refresh token. Hướng khắc phục là chuyển refresh token sang cookie `HttpOnly` (kèm `Secure`, `SameSite`) do máy chủ đặt, đồng thời thêm CSP để giảm bề mặt XSS.

### 7.3.6 Điểm yếu của endpoint AI

`AiController` (`POST api/v1/ai/chat`) không có `[Authorize]` và không gắn chính sách giới hạn riêng; endpoint chỉ chịu bộ giới hạn toàn cục theo địa chỉ IP, khác với `auth` và `booking-hold` ở các controller khác. Trong `AiValidators.cs`, danh sách vai trò hợp lệ gồm `user`, `model`, `assistant` và `system`, và validator không giới hạn số phần tử của `History` (chỉ giới hạn độ dài từng nội dung). `GeminiAiAssistantService` ánh xạ mọi vai trò khác `user` thành `model`, nên một lượt `system` do client gửi sẽ được chuyển cho mô hình như lời của `model`. Phần system prompt do `BuildSystemPromptAsync` dựng có dòng thông tin khách hàng gồm tên, email, điểm và hạng khi người dùng đã đăng nhập, cùng danh sách phim và combo.

Tác động có thể xảy ra gồm chi phí gọi mô hình bên ngoài do endpoint mở cho khách vãng lai, khả năng giả mạo lịch sử hội thoại, và việc gửi dữ liệu cá nhân (email) cho bên thứ ba là dịch vụ mô hình. Hướng khắc phục là thêm giới hạn tốc độ riêng cho endpoint, bỏ vai trò `system` khỏi dữ liệu client gửi lên và giới hạn số lượt lịch sử, và giảm dữ liệu cá nhân đưa vào system prompt (bỏ email, chỉ giữ thông tin cần cho câu trả lời).

### 7.3.7 vnp_IpAddr cố định

`VnPayGateway` khai báo hằng `DefaultIpAddress = "127.0.0.1"` và dùng nó cho tham số `vnp_IpAddr` khi tạo yêu cầu thanh toán và ở đoạn xử lý hoàn tiền (`ipAddr`). Giá trị này không phản ánh địa chỉ IP thật của khách hàng. Tác động có thể xảy ra là dữ liệu IP gửi cho cổng thanh toán không đúng thực tế, làm giảm giá trị của thông tin này trong việc đối soát hoặc phát hiện gian lận phía cổng. Hướng khắc phục là truyền địa chỉ IP thật của yêu cầu (có xét tiêu đề chuyển tiếp của proxy) vào cổng thanh toán.

### 7.3.8 Khai báo chưa dùng

Lớp `PaymentMethodNames` trong `Cinema.Shared/Constants` khai báo hằng `ZaloPay`, trong khi enum `PaymentMethod` chỉ có `VNPay`, `Momo`, `Cash` và `Stripe`, không có lớp cổng thanh toán nào cho ZaloPay, và hằng này không được tham chiếu ở backend hay frontend. Tác động có thể xảy ra là gây hiểu nhầm rằng hệ thống hỗ trợ ZaloPay. Hướng khắc phục là xóa hằng này hoặc cài đặt đầy đủ cổng tương ứng.

### 7.3.9 Chú thích lỗi thời

Ba chỗ chú thích hoặc đăng ký không còn khớp với mã. Thứ nhất, `Program.cs` chú thích tầng Application là "MediatR (CQRS handlers)", trong khi kho mã không tham chiếu thư viện MediatR; chú thích khối Hangfire ghi bộ lưu trữ là "SQL Server", trong khi mã cấu hình `UsePostgreSqlStorage`. Thứ hai, `Cinema.Shared/Resources/README.md` nói test `MessagesResourceTests` sẽ báo lỗi nếu hai tệp tài nguyên lệch nhau, nhưng không có lớp test này trong `backend/tests`. Thứ ba, `IAiAssistantService` được đăng ký hai lần với cùng cài đặt `GeminiAiAssistantService`: bằng `AddScoped` trong `Infrastructure/DependencyInjection.cs` và bằng `AddHttpClient` trong `Application/DependencyInjection.cs`. Do `AddApplication` được gọi sau `AddInfrastructure` trong `Program.cs`, khi giải quyết một dịch vụ đơn thì đăng ký sau cùng được dùng; hai đăng ký khác cách khai báo vòng đời, nên dễ gây nhầm khi bảo trì.

Tác động có thể xảy ra là gây hiểu nhầm khi bảo trì. Hướng khắc phục là cập nhật chú thích và README, và giữ một đăng ký duy nhất cho `IAiAssistantService`.

### 7.3.10 Migration thiếu Designer, bỏ qua cảnh báo thay đổi mô hình

Hạn chế này đã được khắc phục một phần. Thư mục `Infrastructure/Migrations` hiện có migration `20261106040650_InitialCreate` đi kèm tệp `.Designer.cs` và `AppDbContextModelSnapshot.cs`, nên phần "thiếu Designer" không còn đúng. Tuy nhiên `Infrastructure/DependencyInjection.cs` vẫn cấu hình `ConfigureWarnings` để bỏ qua `RelationalEventId.PendingModelChangesWarning`. Với cấu hình này, nếu mô hình thay đổi mà chưa tạo migration mới thì ứng dụng không cảnh báo và `MigrateAsync` vẫn chạy, nên snapshot có thể lệch mô hình mà không bị phát hiện. Việc snapshot hiện tại có khớp mô hình hay không chưa được kiểm tra, vì môi trường làm việc không có công cụ `dotnet`. Hướng khắc phục là bật lại cảnh báo (hoặc kiểm tra bằng lệnh `dotnet ef migrations has-pending-model-changes` khi tích hợp liên tục) và tạo lại migration nếu phát hiện lệch.

### 7.3.11 Kiểm tra tài khoản bằng truy vấn CSDL mỗi yêu cầu

Trong `Program.cs`, sự kiện `OnTokenValidated` của JWT bearer đọc `UserId` từ token rồi gọi `db.Users.AnyAsync(u => u.UserId == userId && u.IsActive)`; nếu tài khoản không còn hoạt động thì xác thực bị từ chối với thông báo "Tài khoản đã bị khóa." Cách làm này bảo đảm tài khoản bị khóa mất quyền ngay lập tức, kể cả khi token còn hạn. Đổi lại, mỗi yêu cầu có xác thực phát sinh thêm một truy vấn cơ sở dữ liệu. Hướng khắc phục là dùng bộ nhớ đệm ngắn hạn cho trạng thái hoạt động của người dùng, chấp nhận độ trễ tối đa bằng thời gian sống của bộ nhớ đệm khi khóa tài khoản; [CẦN BỔ SUNG: đo ảnh hưởng của truy vấn này lên độ trễ trước khi quyết định].

### 7.3.12 Thiếu CI/CD, test frontend, kiểm thử tải

Kho mã không có thư mục `.github` hay tệp pipeline tích hợp liên tục nào; các tệp triển khai hiện có là `render.yaml`, `vercel.json` và `docker-compose.yml`. Phía frontend, `frontend/package.json` và `package.json` gốc không khai báo bộ chạy test đơn vị (không có Vitest hay Jest) và không có tệp `*.test.*` hay `*.spec.*` trong `frontend/src`; chất lượng frontend được kiểm tra bằng ESLint, kiểm tra kiểu và bộ E2E Playwright. Về kiểm thử tải, thư mục `load-tests/` có tồn tại với hai kịch bản k6 (`browse-movies-stress.js`, `peak-ticket-rush.js`) nhưng thư mục `results/` chỉ có tệp giữ chỗ, tức là chưa có kết quả chạy (Mục 6.6.2), và cả hai kịch bản chỉ đo thao tác đọc.

Tác động có thể xảy ra là không có kiểm tra tự động khi đẩy mã, và hồi quy ở frontend hay hiệu năng chỉ được phát hiện khi chạy thủ công. Hướng khắc phục là thêm pipeline chạy `dotnet test`, lint và build, thêm Vitest cho logic frontend, và chạy k6 để thu số liệu trong môi trường có cấu hình giới hạn tần suất được nêu rõ (Mục 6.6.1).

### 7.3.13 Hạn chế của bộ E2E

Chi tiết nằm ở Mục 6.4.16 và Bảng 6.25. Tóm lại: các kịch bản làm thay đổi dữ liệu thật và không có cơ sở dữ liệu cô lập; hàm `runSql` trong `helpers.ts` phụ thuộc Docker và chuỗi kết nối `E2E_DATABASE_URL` hoặc `DATABASE_URL`; E2E-08 hủy dòng đầu tiên khớp tên phim thay vì dòng vừa tạo; `dotenv` chỉ khai báo ở `package.json` gốc; và `cleanFutureTestShowtimes` dùng mốc ngày cố định. Báo cáo 8/8 đạt ngày 07/09/2026 có trước các migration PostgreSQL, trong khi `test-results/.last-run.json` trong kho mã ghi một lần chạy thất bại ở E2E-06 do lỗi cấu hình `baseURL`. Phiên bản `helpers.ts` hiện tại đã dùng PostgreSQL qua `psql`.

Tác động có thể xảy ra là kết quả E2E cần được chạy lại trên môi trường và cơ sở dữ liệu hiện tại trước khi dùng làm bằng chứng. Hướng khắc phục là dùng cơ sở dữ liệu kiểm thử riêng, kiểm tra giá trị trả về của `runSql`, xác định đúng dòng vừa tạo ở E2E-08 và chạy lại toàn bộ bộ E2E.

### 7.3.14 Kiểu dữ liệu client lệch server

Trong `frontend/src/lib/signalr.ts`, giao diện `SeatHeldPayload` khai báo `heldByUserId: number` là trường bắt buộc. Phía máy chủ, `SignalRRealtimeNotifier` nhận tham số `heldByUserId` nhưng chú thích rằng không gửi trường này ra client để tránh lộ thông tin người dùng khác, và test `HoldSeats_BroadcastsSeatHeldEventToClientsInShowtimeGroup` khẳng định payload không có `heldByUserId`. Trường này chỉ được khai báo trong `signalr.ts`, không có nơi nào khác ở frontend dùng. Tác động có thể xảy ra là kiểu dữ liệu khai báo không khớp giá trị thật (luôn `undefined` khi chạy), dễ gây lỗi nếu sau này có mã dựa vào trường này. Hướng khắc phục là đồng bộ kiểu bằng cách bỏ trường `heldByUserId` khỏi `SeatHeldPayload`.

### 7.3.15 Hạn chế của cơ chế điểm thành viên 12 tháng

Cơ chế điểm dùng sổ điểm (`LoyaltyPointEntry`) và cửa sổ trượt 12 tháng (Hình 4.58). Đối chiếu với mã nguồn hiện tại, bốn hạn chế được nêu ở dàn ý có mức độ còn lại khác nhau.

Thứ nhất, độ trễ đồng bộ. Điểm và hạng lưu trên `CustomerProfile` (cột `Points`, `TierId`) được tính lại cho mọi khách bởi `ExpireLoyaltyPointsJob` qua `LoyaltyExpiryService.RecalculateAllAsync`, theo cron `5 * * * *` (mỗi giờ, phút 05), nên có thể cũ tối đa một giờ. Tuy nhiên các đường đọc chính không dùng cột này: `UserProfileService` (chú thích trong mã nêu rõ không đọc `profile.Points/TierId` vì là bản cache), `AuthService.ToLoginUserDtoAsync`, `GeminiAiAssistantService`, `EmployeeService` (danh sách người dùng) và việc tính giảm giá khi đặt vé đều tính trực tiếp từ sổ bằng `LoyaltyWindow.GetStandingAsync`, `GetStandingsAsync` hoặc `SumActiveAsync`. Độ trễ vì vậy chỉ còn tác động tới cột lưu trữ trên hồ sơ. [CẦN BỔ SUNG: xác nhận không còn báo cáo hay truy vấn nào khác đọc trực tiếp `CustomerProfile.Points`].

Thứ hai, độ dài cửa sổ. Hạn chế "cố định trong code" không còn đúng: `LoyaltyWindow.LoadMonthsAsync` đọc `CinemaSettings.LoyaltyWindowMonths` (mặc định `DefaultLoyaltyWindowMonths` = 12, validator cho phép 1 đến 120) và giao diện cài đặt quản trị có trường `loyaltyWindowMonths`. Đổi giá trị này làm đổi cách tính lần chạy sau, nhưng chưa được cập nhật vào cột lưu trữ cho tới khi job chạy.

Thứ ba, `TotalSpent` không theo cửa sổ trượt. `BookingService` cộng `FinalAmount` khi xác nhận (dòng 941) và trừ khi hoàn tiền (dòng 875), không loại bỏ khoản chi đã ra khỏi cửa sổ; giá trị này được trả ra trong hồ sơ (`UserProfileDto.TotalSpent`) cùng với điểm theo cửa sổ nên hai chỉ số có phạm vi thời gian khác nhau.

Thứ tư, thông báo và hiển thị hết hạn. Không tìm thấy mã nào gửi thông báo điểm sắp hết hạn hay hiển thị ngày hết hạn của điểm ở cả backend lẫn frontend; các trường `expiresAt` ở frontend thuộc về thời hạn giữ ghế của booking.

Thứ năm, khách cũ chưa có sổ. `LoyaltyWindowTests` ghi lại hành vi rằng khách chưa được backfill sổ điểm sẽ bị job đưa điểm về 0, và chú thích kèm theo yêu cầu chạy script backfill trước khi bật job. Script `Migrations/_manual/backfill_loyalty_entries.sql` được nêu ở dàn ý không có trong kho mã hiện tại, nên việc backfill chưa được tự động hóa hay cung cấp kèm mã. Với cơ sở dữ liệu tạo mới từ migration `InitialCreate` (đã có bảng `LoyaltyPointEntries`) thì không cần backfill; rủi ro chỉ áp dụng cho dữ liệu cũ có điểm trước khi có sổ.

Tác động có thể xảy ra là cột điểm và hạng lưu trên hồ sơ lệch tạm thời so với sổ, khách không được cảnh báo trước khi mất điểm, và khách cũ có thể bị mất điểm nếu bật job mà chưa có sổ. Hướng khắc phục là thêm thông báo điểm sắp hết hạn và hiển thị ngày hết hạn trên hồ sơ; thu hẹp hoặc bỏ cột lưu trữ để mọi nơi đọc từ sổ, hoặc rút ngắn lịch job; xác định rõ phạm vi của `TotalSpent`; và tự động hóa backfill bằng migration dữ liệu.

### 7.3.16 Tổng hợp hạn chế

Bảng 7.3 tổng hợp 15 hạn chế ở Mục 7.3.1 đến 7.3.15, kèm hiện trạng đối chiếu với mã nguồn hiện tại và đề xuất khắc phục.

**Bảng 7.3.** Hạn chế, đề xuất khắc phục

| STT | Hạn chế | Hiện trạng | Đề xuất |
|---|---|---|---|
| 1 | Chống trùng suất chiếu chỉ ở tầng ứng dụng | Còn | Thêm ràng buộc loại trừ ở CSDL |
| 2 | Ghế hết hạn chỉ được giải phóng bởi job nền | Còn | Giải phóng ngay khi truy vấn hoặc khi giữ ghế |
| 3 | Thành phần đánh giá trong Heat dùng thang sai | Đã khắc phục (chia cho `MaxRating`, có test) | Không cần thêm; giữ test `Heat_CalculatesHeatCorrectlyOn1To5RatingScale` |
| 4 | Seed dữ liệu demo chạy ở mọi môi trường | Còn | Tách seed demo khỏi Production |
| 5 | Refresh token lưu localStorage, chưa có CSP | Còn | Cookie HttpOnly và CSP |
| 6 | Điểm yếu của endpoint AI | Còn | Giới hạn tốc độ riêng, bỏ vai trò system, giảm dữ liệu cá nhân |
| 7 | vnp_IpAddr cố định | Còn | Truyền IP thật |
| 8 | Khai báo chưa dùng (`ZaloPay`) | Còn | Xóa hoặc cài đặt |
| 9 | Chú thích lỗi thời | Còn | Cập nhật chú thích, dọn đăng ký trùng |
| 10 | Migration thiếu Designer, bỏ qua cảnh báo thay đổi mô hình | Một phần (đã có Designer; cảnh báo vẫn bị bỏ qua) | Bật lại cảnh báo, kiểm tra snapshot, tạo lại migration nếu lệch |
| 11 | Kiểm tra tài khoản bằng truy vấn CSDL mỗi yêu cầu | Còn | Bộ nhớ đệm ngắn hạn [CẦN BỔ SUNG: đo ảnh hưởng] |
| 12 | Thiếu CI/CD, test frontend, kiểm thử tải | Còn (k6 đã có kịch bản nhưng chưa có kết quả) | Thêm pipeline, Vitest, chạy k6 |
| 13 | Hạn chế của bộ E2E | Còn | Cơ sở dữ liệu kiểm thử riêng, sửa helper và E2E-08, chạy lại |
| 14 | Kiểu dữ liệu client lệch server | Còn | Đồng bộ kiểu |
| 15 | Điểm thành viên 12 tháng: cột điểm và hạng trên hồ sơ trễ tối đa 1 giờ, chưa có thông báo hoặc hiển thị ngày hết hạn, `TotalSpent` không theo cửa sổ, khách cũ phải backfill | Một phần (cửa sổ đã cấu hình được; các đường đọc chính đã đọc từ sổ) | Bỏ hoặc đồng bộ ngay cột lưu trữ, thêm thông báo và ngày hết hạn, làm rõ `TotalSpent`, tự động hóa backfill trong migration |

<!-- GHI CHÚ ĐỐI CHIẾU -->
<!--
(a) [CẦN BỔ SUNG] còn lại:
- Bảng 7.2, MT1: đối chiếu từng use case trong 103 với mã cài đặt (mới làm ở mức package).
- Bảng 7.2, MT2, MT3, MT4, MT5: kết quả chạy thật (log dotnet test, coverage, k6; Chương 6 chưa có). Không có `dotnet` trong môi trường nên không tự chạy được.
- 7.3.2: kiểm chứng hành vi khi dịch vụ gói miễn phí ngủ.
- 7.3.11: đo ảnh hưởng của truy vấn trong OnTokenValidated.
- 7.3.10: chưa kiểm được snapshot có lệch mô hình hay không (cần dotnet ef).
- 7.3.15: xác nhận không còn nơi nào đọc trực tiếp CustomerProfile.Points/TierId (đã thấy AuthMappingConfig ánh xạ Points từ cột cache nhưng AuthService.ToLoginUserDtoAsync ghi đè bằng giá trị từ sổ; chưa rà báo cáo/dashboard).
- Hình 4.58 được dẫn chiếu ở 7.3.15, không dựng lại.

(b) Chỗ outline lệch code:
- Bảng 1.1 (Chương 1, cột "Kết quả") ghi 103/103 use case, 448/448 test pass, k6 đạt chỉ tiêu, SignalR nhận seat.booked và hold.expired: Chương 6 không có log chạy, số test đếm được là 364 + 49 = 413, test SignalR chỉ kiểm tra seat.held, seat.released, kết quả k6 chưa có, `.last-run.json` trong repo ghi failed. Cần sửa cột "Kết quả" của Bảng 1.1 theo Bảng 7.2.
- Bảng 7.1: outline thiếu PaymentServiceMomoTests, LoyaltyWindowTests, BookingServiceLoyaltyTests, SettingsServiceTests, CinemaSettingsPolicyTests, EmployeeServiceRoleRulesTests, SeatLayoutRulesTests, ShowtimeValidatorsTests, ReviewValidatorsTests, EmailTemplatesTests (đã thêm). AdminReviewsBulkEndpointsTests chuyển khỏi DG (outline ghi "phía quản trị" nhưng đặt ở DG) sang QV. Outline ghi E2E-01…08 cho XT: đổi thành gián tiếp. Ô QT, TP, AI, QD giữ "chưa có test". ShowtimeService*Tests, ShowtimeValidatorsTests thêm cho QS. Các lớp job không có test gọi trực tiếp.
- Bảng 7.2, MT1: outline ghi 103 use case (khớp Bảng 3.2).
- 7.3.3: outline nói Heat chia cho 10, chưa có test; code hiện chia cho MaxRating (mặc định 5) và có test Heat_CalculatesHeatCorrectlyOn1To5RatingScale. Hạn chế đã khắc phục; vẫn giữ mục để khớp 15 hạn chế.
- 7.3.5: outline ghi `frontend/vercel.json`; tệp thực tế là `vercel.json` ở thư mục gốc.
- 7.3.9: confirm cả ba chỗ; thêm chi tiết Hangfire thực tế dùng UsePostgreSqlStorage.
- 7.3.10: outline nêu `Migrations/20261106040650_InitialCreate.cs` thiếu Designer; code hiện có `20261106040650_InitialCreate` kèm Designer. Chỉ còn phần bỏ qua PendingModelChangesWarning.
- 7.3.12: outline nói `load-tests/` vắng; thực tế có thư mục với 2 kịch bản k6, chưa có kết quả. Chưa tìm thấy Vitest/Jest hay file test ở frontend.
- 7.3.15: outline ghi `LoyaltyWindow.Months = 12` cố định trong code; code hiện đọc CinemaSettings.LoyaltyWindowMonths (mặc định 12, validator 1–120, có ô trên trang cài đặt quản trị). Outline ghi hồ sơ hiển thị điểm cũ tối đa 1 giờ; code hiện tính hồ sơ, đăng nhập, AI, danh sách người dùng và giảm giá đặt vé trực tiếp từ sổ, chỉ cột cache CustomerProfile.Points/TierId có thể cũ tối đa 1 giờ. Script `Migrations/_manual/backfill_loyalty_entries.sql` không có trong kho mã (chỉ có chú thích ở LoyaltyWindowTests.cs). Cron `5 * * * *` và việc TotalSpent không theo cửa sổ (BookingService dòng 875, 941) vẫn đúng. Cờ "thay đổi giao diện chỉ về nhãn chữ" của outline không kiểm chứng được và không dùng.
- Tên file quy định: prompt gọi `quy-dinh-trinh-bay.md`, file thực tế là `quy-dinh-format-bao-cao.md`.
-->

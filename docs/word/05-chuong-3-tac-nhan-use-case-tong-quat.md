# CHƯƠNG 3. PHÂN TÍCH YÊU CẦU

Chương này xác định các tác nhân tương tác với hệ thống Perfo và tổng hợp các chức năng thành tập use case. Mục 3.1 trình bày danh sách tác nhân, quan hệ giữa các tác nhân và phạm vi của hệ thống. Mục 3.2 giới thiệu 21 package use case ở mức tổng quát cùng ma trận tác nhân theo package; chi tiết từng package được trình bày ở Mục 3.3.

## 3.1 TÁC NHÂN

### 3.1.1 Danh sách tác nhân

Tác nhân được xác định từ mã nguồn backend, không suy diễn từ giao diện. Hai nguồn được đối chiếu với nhau. Nguồn thứ nhất là hằng số vai trò `Roles` (`Customer`, `Admin`, `Employee`) và hằng số policy `Policies` (`CustomerOnly`, `AdminOnly`, `EmployeeOnly`, `StaffOnly`), được đăng ký trong `Program.cs` qua `AddAuthorizationBuilder()`: `CustomerOnly` yêu cầu vai trò `Customer`, `AdminOnly` yêu cầu vai trò `Admin`, `EmployeeOnly` yêu cầu vai trò `Employee`, còn `StaffOnly` chấp nhận vai trò `Admin` hoặc `Employee`. Nguồn thứ hai là thuộc tính `[Authorize]` ở cấp lớp và cấp phương thức của toàn bộ 33 controller trong `Cinema.Api/Controllers/`.

Kết quả rà soát cho thấy 136 action được phân bổ theo mức truy cập như sau: 29 action công khai (không có thuộc tính xác thực), 11 action chỉ yêu cầu đăng nhập, 3 action thuộc `CustomerOnly`, 9 action thuộc `StaffOnly` và 84 action thuộc `AdminOnly`. Một action khai báo đồng thời hai thuộc tính HTTP (như `VnPayIpn` với `GET` và `POST`) chỉ được tính một lần. Ngoài các controller, `ShowtimeHub` (đường dẫn `/hubs/showtimes`) có thuộc tính `[Authorize]`, còn bảng điều khiển Hangfire tại `/admin/hangfire` được bảo vệ bởi `HangfireAdminAuthorizationFilter`: bộ lọc này cho phép mọi yêu cầu khi môi trường là Development, và ở các môi trường khác chỉ cho phép người dùng đã xác thực có vai trò `Admin`.

Mỗi tài khoản chỉ có một vai trò (`User.Role`), và `JwtTokenService` ghi vai trò đó vào access token. Do đó ba vai trò trong `Roles` tương ứng với ba tác nhân người dùng cụ thể là khách hàng, nhân viên và quản trị viên. Hai tác nhân người dùng còn lại không phải là vai trò: "khách vãng lai" là người chưa xác thực, còn "người dùng đã đăng nhập" là tác nhân tổng quát tương ứng với các action chỉ có `[Authorize]` mà không ràng buộc vai trò. Hai cổng đăng nhập tách biệt càng củng cố cách phân biệt này: `AuthService.LoginAsync` từ chối tài khoản không phải `Customer` ở cổng `login` (`Forbidden_UseAdminPortal`) và từ chối tài khoản `Customer` ở cổng `admin-login` (`Forbidden_CustomerCannotUseAdminPortal`).

Các tác nhân còn lại là hệ thống. Hệ thống tác vụ nền là thành phần chạy trong cùng tiến trình với API, còn các cổng thanh toán, Google, Gemini, dịch vụ email và Cloudinary là hệ thống ngoài, được truy cập qua các giao diện trừu tượng của lớp `Application` (`IPaymentGateway`, `IGoogleAuthService`, `IAiAssistantService`, `IEmailService`, `IFileStorage`) và đăng ký trong `DependencyInjection.cs` của lớp `Infrastructure`. Bảng 3.1 tổng hợp các tác nhân cùng căn cứ trong mã nguồn.

**Bảng 3.1. Danh sách tác nhân**

| Tác nhân | Loại | Mô tả | Quyền/Policy | Căn cứ trong mã nguồn |
|---|---|---|---|---|
| Khách vãng lai | Người dùng | Người chưa đăng nhập: xem danh sách phim, chi tiết phim, đánh giá, lịch chiếu, sơ đồ ghế, gợi ý ghế, combo, hạng thành viên, thông tin rạp; đăng ký, đăng nhập, khôi phục mật khẩu; hỏi trợ lý AI | Công khai | 26 action không có thuộc tính xác thực: `AuthController` (10 action từ `register` đến `resend-confirmation`), `AiController.Chat`, `CatalogController` (4), `CombosController` (2), `MoviesController` (4 action đọc, gồm `can-review`), `SettingsController` (1), `ShowtimesController` (3), `HealthController` (1) |
| Người dùng đã đăng nhập | Người dùng (tổng quát) | Người đã đăng nhập, không phân biệt vai trò: xem thông tin phiên, xem và cập nhật hồ sơ, đổi mật khẩu, lấy socket token để kết nối SignalR, thao tác trên đơn đặt vé của chính mình | Đăng nhập (`[Authorize]`, không ràng buộc vai trò) | `AuthController` (`socket-token`, `me`); `UsersController` (4 action, `[Authorize]` ở cấp lớp); `BookingsController` (5 action, `[Authorize]` ở cấp lớp); `ShowtimeHub` |
| Khách hàng | Người dùng | Người dùng có vai trò `Customer`, được tạo qua `AuthService.RegisterAsync` hoặc `GoogleLoginAsync`; đặt vé, thanh toán, hủy đơn, viết, sửa và xóa đánh giá | `CustomerOnly` (vai trò `Customer`) cho 3 thao tác đánh giá; các thao tác đặt vé, thanh toán, hủy đơn chỉ yêu cầu đăng nhập (`[Authorize]`) | `MoviesController`: `POST`, `PUT`, `DELETE` của `reviews` (3 action); `BookingsController` không có policy vai trò, `BookingService.HoldSeatsAsync` không kiểm tra vai trò |
| Nhân viên | Người dùng | Người dùng có vai trò `Employee`: bán vé tại quầy, soát vé, tra cứu booking, phim, suất chiếu và danh sách khách hàng | `StaffOnly` (vai trò `Admin` hoặc `Employee`) | 9 action `StaffOnly`: `AdminBookingsController` (5: `counter-hold`, danh sách, chi tiết, `counter-checkout`, `checkin`), `AdminMoviesController` (`GET` danh sách), `AdminShowtimesController` (`GET` danh sách và `calendar`), `AdminUsersController` (`GET customers`); đăng nhập qua `admin-login` |
| Quản trị viên | Người dùng | Người dùng có vai trò `Admin`: quản lý danh mục, phim, phòng chiếu, suất chiếu, người dùng, nhân sự, đánh giá, giao dịch thanh toán, báo cáo, cấu hình rạp, tải tệp; xem bảng điều khiển Hangfire; đồng thời thực hiện được mọi thao tác của nhân viên | `AdminOnly` (vai trò `Admin`); đồng thời thỏa `StaffOnly` | 84 action `AdminOnly` thuộc 21 controller quản trị (riêng `AdminBookingsController` chỉ có `StaffOnly`); `HangfireAdminAuthorizationFilter` |
| Hệ thống tác vụ nền | Hệ thống | Hangfire chạy trong tiến trình API: giải phóng ghế hết hạn, cập nhật vòng đời suất chiếu, gửi email, hoàn tiền, tính lại điểm thành viên hết hạn | Không truy cập qua HTTP; bảng điều khiển `/admin/hangfire` chỉ dành cho quản trị | `Program.cs` đăng ký 3 job định kỳ (`ReleaseExpiredSeatsJob`, `ShowtimeLifecycleJob`, `ExpireLoyaltyPointsJob`) qua `RecurringJob.AddOrUpdate` với mã trong `RecurringJobIds`; 2 job xếp hàng khi có sự kiện (`SendEmailJob`, `ShowtimeRefundJob`); lưu trữ bằng PostgreSQL (`UsePostgreSqlStorage`); không khởi tạo ở môi trường Testing |
| Cổng VNPay, Cổng MoMo, Cổng Stripe | Hệ thống ngoài | Cổng thanh toán: tạo giao dịch (`CreatePaymentAsync`), gửi IPN/webhook về hệ thống, hoàn tiền (`RefundAsync`) | Công khai ở mức HTTP, tính hợp lệ dựa trên xác minh chữ ký | `PaymentsController` (3 action không có thuộc tính xác thực); `PaymentService` gọi `VerifyCallback` hoặc `VerifyWebhook`; VNPay ký bằng HMAC-SHA512 và MoMo bằng HMAC-SHA256 (`HmacSigner`); Stripe dùng `EventUtility.ConstructEvent` với dung sai 300 giây khi đã cấu hình `WebhookSecret` |
| Google | Hệ thống ngoài | Xác minh ID token của người dùng đăng nhập bằng Google | Xác minh ID token, cần cấu hình `GoogleOptions` | `IGoogleAuthService.VerifyIdTokenAsync` do `GoogleAuthService` cài đặt (`GoogleJsonWebSignature`); chưa cấu hình thì báo `Rule_GoogleOAuthNotConfigured` |
| Gemini | Hệ thống ngoài | Mô hình ngôn ngữ cho trợ lý AI, nhận lịch sử hội thoại và khai báo hàm, trả lời hoặc yêu cầu gọi hàm | Khóa API (`GeminiOptions.ApiKey`) | `GeminiAiAssistantService` gọi `{BaseUrl}/{Model}:generateContent`; mô hình mặc định `gemini-2.5-flash`; danh sách hàm phụ thuộc trạng thái đăng nhập (`AiToolExecutor.BuildFunctionDeclarations(userId.HasValue)`) |
| Dịch vụ email | Hệ thống ngoài | Gửi email xác nhận, khôi phục mật khẩu, xác nhận hủy vé và các thư vận hành | Cấu hình `EmailOptions` | `FallbackEmailService`: dùng `ResendEmailService` khi có `ResendApiKey`; nếu lỗi và có `SmtpHost` thì chuyển sang `MailKitEmailService` (SMTP) |
| Cloudinary | Hệ thống ngoài | Lưu trữ ảnh và video do quản trị tải lên | Khóa API (cấu hình Cloudinary) | `CloudinaryFileStorage` cài đặt `IFileStorage` (`UploadImageAsync`, `UploadVideoAsync`, `DeleteAsync`); `AdminUploadsController` là nơi duy nhất tiêm `IFileStorage` |

Policy `EmployeeOnly` được định nghĩa trong `Policies.cs` và đăng ký trong `Program.cs`, nhưng không có controller nào sử dụng. Mọi chức năng dành cho nhân viên đều dùng `StaffOnly`, vì vậy cùng một chức năng luôn mở cho cả quản trị viên.

### 3.1.2 Quan hệ giữa các tác nhân

Giữa các tác nhân người dùng tồn tại quan hệ tổng quát hóa, thể hiện ở Hình 3.1. Người dùng đã đăng nhập kế thừa mọi quyền của khách vãng lai: các action công khai không chặn người đã xác thực, và một số action còn xử lý khác nhau theo trạng thái đăng nhập, chẳng hạn `AiController.Chat` đọc mã người dùng nếu có và `MoviesController.GetCanReviewStatus` trả `CanReviewResponse(false)` cho người chưa đăng nhập. Khách hàng và nhân viên là hai tác nhân con của người dùng đã đăng nhập, vì cả hai đều dùng được các action chỉ có `[Authorize]`, như xem hồ sơ qua `UsersController` hay lấy socket token qua `AuthController`. Quản trị viên kế thừa nhân viên: mọi action mà nhân viên truy cập được đều thuộc `StaffOnly`, vốn chấp nhận cả `Admin` lẫn `Employee`, hoặc thuộc nhóm công khai và nhóm chỉ yêu cầu đăng nhập, nên quản trị viên cũng truy cập được.

Cần lưu ý rằng quan hệ này là quan hệ của mô hình use case, không phải một phân cấp vai trò trong mã. Policy `AdminOnly` chỉ yêu cầu vai trò `Admin`, nên chiều ngược lại không thành lập: nhân viên không truy cập được 84 action `AdminOnly`. Quản trị viên cũng không kế thừa khách hàng; `CustomerOnly` loại tài khoản `Admin` khỏi các thao tác viết, sửa, xóa đánh giá. Các hệ thống ngoài và hệ thống tác vụ nền không tham gia quan hệ tổng quát hóa này.

![Hình 3.1. Quan hệ giữa các tác nhân](hinh/3.1.png)

*Hình 3.1. Quan hệ giữa các tác nhân*

### 3.1.3 Sơ đồ ngữ cảnh hệ thống

Hệ thống Perfo gồm giao diện web React và backend .NET (`Cinema.Api`) cùng lớp ứng dụng, lớp hạ tầng và cơ sở dữ liệu PostgreSQL. Cơ sở dữ liệu lưu cả dữ liệu nghiệp vụ lẫn dữ liệu của Hangfire (`UsePostgreSqlStorage`), nên nằm bên trong ranh giới hệ thống. Hình 3.2 đặt hệ thống ở trung tâm và thể hiện các luồng dữ liệu vào và ra với tác nhân người dùng và hệ thống ngoài.

Với các tác nhân người dùng, dữ liệu đi vào hệ thống dưới dạng yêu cầu HTTP tới các đường dẫn `/api/v1/...` và kết nối SignalR tới `/hubs/showtimes`. Kết nối SignalR dùng token ngắn hạn do `POST /api/v1/auth/socket-token` cấp, được đọc từ tham số `access_token` của chuỗi truy vấn (`OnMessageReceived` trong `Program.cs`). Dữ liệu đi ra là phản hồi JSON và bốn sự kiện thời gian thực trong `SignalREvents`: `seat.held`, `seat.released`, `seat.booked` và `hold.expired`. Khách vãng lai gửi yêu cầu xem dữ liệu công khai và yêu cầu xác thực; khách hàng gửi yêu cầu giữ ghế, thanh toán, hủy đơn và đánh giá; nhân viên và quản trị viên gửi các yêu cầu quản trị và bán vé tại quầy qua các đường dẫn `/api/v1/admin/...`.

Với các cổng thanh toán, luồng đi ra là yêu cầu tạo liên kết thanh toán và yêu cầu hoàn tiền, còn luồng đi vào là thông báo kết quả thanh toán do cổng gọi về các đường dẫn `/api/v1/payments/vnpay/ipn`, `/api/v1/payments/momo/ipn` và `/api/v1/payments/stripe/webhook`. Với Google, hệ thống gửi ID token nhận từ giao diện để xác minh và nhận lại thông tin người dùng (`GoogleUserInfo`). Với Gemini, hệ thống gửi lịch sử hội thoại, chỉ dẫn hệ thống và khai báo hàm, rồi nhận về câu trả lời hoặc yêu cầu gọi hàm; các hàm được thực thi nội bộ bởi `AiToolExecutor` trên dữ liệu của hệ thống. Với dịch vụ email, hệ thống gửi nội dung thư qua `IEmailService.SendAsync`. Với Cloudinary, hệ thống gửi tệp ảnh hoặc video và nhận lại địa chỉ truy cập cùng mã `PublicId`.

Hệ thống tác vụ nền chạy trong tiến trình của API và không đi qua ranh giới hệ thống; nó được mô hình hóa như một tác nhân ở Hình 3.3 vì kích hoạt các use case theo lịch hoặc theo sự kiện, chứ không phải vì là một hệ thống bên ngoài.

![Hình 3.2. Sơ đồ ngữ cảnh hệ thống](hinh/3.2.png)

*Hình 3.2. Sơ đồ ngữ cảnh hệ thống*

## 3.2 USE CASE TỔNG QUÁT

### 3.2.1 Danh mục package use case

Các chức năng của hệ thống được nhóm thành 21 package use case, tương ứng với use case mức 0. Mỗi package có một sơ đồ use case và một bảng danh sách use case mức 1 ở Mục 3.3. Use case chỉ có hai mức là package và use case; không có use case con. Quan hệ include và extend chỉ được vẽ giữa các use case mức 1 trong cùng sơ đồ package. Tổng số use case mức 1 là 103; số này được đếm lại từ các dòng `UC-` trong các bảng ở Mục 3.3 và khớp với số ghi cho từng package ở Bảng 3.2. Package Xác thực có nhiều use case nhất (11), tiếp theo là Quản lý danh mục (10), còn Đặt vé trực tuyến và Quản lý suất chiếu đều có 9 use case.

**Bảng 3.2. Danh mục package use case**

| Mục | Mã | Package | Mô tả | Mã use case | Số use case | Tác nhân |
|---|---|---|---|---|---|---|
| 3.3.1 | XT | Xác thực | Đăng ký, đăng nhập, khôi phục mật khẩu, duy trì phiên. | UC-XT-01 đến UC-XT-11 | 11 | Khách vãng lai, Dịch vụ email, Google, Nhân viên, Quản trị viên, Người dùng đã đăng nhập |
| 3.3.2 | HS | Hồ sơ cá nhân | Xem và cập nhật hồ sơ, đổi mật khẩu. | UC-HS-01 đến UC-HS-04 | 4 | Người dùng đã đăng nhập |
| 3.3.3 | DP | Duyệt phim | Xem phim, lịch chiếu và thông tin công khai của rạp. | UC-DP-01 đến UC-DP-08 | 8 | Khách vãng lai |
| 3.3.4 | DG | Đánh giá phim | Khách hàng đã xem phim viết, sửa, xóa đánh giá. | UC-DG-01 đến UC-DG-04 | 4 | Khách hàng |
| 3.3.5 | DV | Đặt vé trực tuyến | Chọn suất, chọn ghế, giữ ghế, quản lý đơn đặt vé. | UC-DV-01 đến UC-DV-09 | 9 | Khách hàng, Khách vãng lai |
| 3.3.6 | TT | Thanh toán trực tuyến | Tạo giao dịch, nhận IPN/webhook, xác nhận vé, hoàn tiền. | UC-TT-01 đến UC-TT-06 | 6 | Khách hàng, Cổng VNPay, Cổng MoMo, Cổng Stripe, Hệ thống tác vụ nền, Dịch vụ email |
| 3.3.7 | AI | Trợ lý AI | Trò chuyện hỏi đáp suất chiếu, ghế trống, vé của tôi. | UC-AI-01 | 1 | Khách vãng lai, Khách hàng, Gemini |
| 3.3.8 | BQ | Bán vé tại quầy | Nhân viên bán vé trực tiếp, thu tiền mặt. | UC-BQ-01 | 1 | Nhân viên, Quản trị viên |
| 3.3.9 | SV | Soát vé | Quét mã QR hoặc nhập mã để soát vé vào rạp. | UC-SV-01 | 1 | Nhân viên, Quản trị viên |
| 3.3.10 | QB | Quản lý booking | Tra cứu đơn đặt vé. | UC-QB-01, UC-QB-02 | 2 | Nhân viên, Quản trị viên |
| 3.3.11 | QP | Quản lý phim | Thêm, sửa, xóa phim. | UC-QP-01 đến UC-QP-05 | 5 | Nhân viên, Quản trị viên, Cloudinary |
| 3.3.12 | QD | Quản lý danh mục | Danh mục phim, danh mục phòng, nhân sự, thành viên, combo. | UC-QD-01 đến UC-QD-10 | 10 | Quản trị viên, Cloudinary |
| 3.3.13 | QR | Quản lý phòng chiếu | Phòng chiếu và sơ đồ ghế. | UC-QR-01 đến UC-QR-03 | 3 | Quản trị viên |
| 3.3.14 | QS | Quản lý suất chiếu | Tạo, sửa, hủy, xếp lịch suất chiếu. | UC-QS-01 đến UC-QS-09 | 9 | Nhân viên, Quản trị viên, Dịch vụ email |
| 3.3.15 | QN | Quản lý người dùng | Tài khoản người dùng và nhân viên. | UC-QN-01 đến UC-QN-07 | 7 | Quản trị viên, Nhân viên |
| 3.3.16 | QV | Quản lý đánh giá | Kiểm duyệt đánh giá phim. | UC-QV-01 đến UC-QV-03 | 3 | Quản trị viên |
| 3.3.17 | QT | Quản lý thanh toán | Tra cứu giao dịch thanh toán. | UC-QT-01, UC-QT-02 | 2 | Quản trị viên |
| 3.3.18 | BC | Báo cáo thống kê | Dashboard và các báo cáo. | UC-BC-01 đến UC-BC-05 | 5 | Quản trị viên |
| 3.3.19 | CH | Cấu hình rạp | Thông tin rạp và tham số vận hành. | UC-CH-01, UC-CH-02 | 2 | Quản trị viên, Cloudinary |
| 3.3.20 | TP | Tải tệp | Tải ảnh và video lên dịch vụ lưu trữ. | UC-TP-01, UC-TP-02 | 2 | Quản trị viên, Cloudinary |
| 3.3.21 | HT | Vận hành hệ thống | Công việc chạy nền và chức năng vận hành hệ thống. | UC-HT-01 đến UC-HT-08 | 8 | Hệ thống tác vụ nền, Dịch vụ email, Cổng VNPay, Cổng MoMo, Cổng Stripe, Quản trị viên, Khách vãng lai |
|  |  | **Tổng** |  |  | **103** |  |

### 3.2.2 Sơ đồ package use case tổng quát

Hình 3.3 thể hiện 21 package và các tác nhân sử dụng chúng. Sơ đồ có 13 tác nhân: năm tác nhân người dùng (Khách vãng lai, Người dùng đã đăng nhập, Khách hàng, Nhân viên, Quản trị viên) và tám tác nhân gắn nhãn `<<Hệ thống>>` (Hệ thống tác vụ nền, Cổng VNPay, Cổng MoMo, Cổng Stripe, Google, Gemini, Dịch vụ email, Cloudinary). Mỗi package được vẽ bằng một khung chữ nhật gồm mã, tên và số use case, ví dụ "XT, Xác thực, (11 use case)". Sơ đồ có 51 liên kết giữa tác nhân và package; các liên kết này chỉ biểu diễn quan hệ tham gia, không vẽ quan hệ tổng quát hóa giữa các tác nhân (đã thể hiện ở Hình 3.1) và không vẽ quan hệ include hoặc extend (chỉ có ở sơ đồ từng package trong Mục 3.3).

Theo từng tác nhân, Khách vãng lai liên kết với 5 package (XT, DP, DV, AI, HT). Người dùng đã đăng nhập liên kết với 2 package (XT, HS). Khách hàng liên kết với 4 package (DG, DV, TT, AI). Nhân viên liên kết với 7 package (XT, BQ, SV, QB, QP, QS, QN). Quản trị viên liên kết với 15 package: XT, BQ, SV, QB, QP, QD, QR, QS, QN, QV, QT, BC, CH, TP và HT. Hệ thống tác vụ nền liên kết với TT và HT. Mỗi cổng thanh toán (VNPay, MoMo, Stripe) liên kết với TT và HT. Google chỉ liên kết với XT; Gemini chỉ liên kết với AI; Dịch vụ email liên kết với XT, TT, QS và HT; Cloudinary liên kết với QP, QD, CH và TP.

Các liên kết trong sơ đồ được suy ra từ cột "Tác nhân" của các bảng use case ở Mục 3.3; việc đối chiếu cho thấy tập tác nhân của từng package trong sơ đồ trùng với tập tác nhân tổng hợp từ các bảng ở Mục 3.3 và với cột "Tác nhân" của Bảng 3.2.

![Hình 3.3. Sơ đồ package use case tổng quát](hinh/3.3.png)

*Hình 3.3. Sơ đồ package use case tổng quát*

### 3.2.3 Ma trận tác nhân theo package

Bảng 3.3 đánh dấu các tác nhân tham gia từng package. Dấu "x" được đặt khi tác nhân xuất hiện trực tiếp trong cột "Tác nhân" của ít nhất một use case thuộc package đó ở Mục 3.3; không đánh dấu theo quan hệ kế thừa. Hai cột hệ thống được tách ra: cột "Hệ thống tác vụ nền" chỉ có dấu "x", còn cột "Hệ thống ngoài" ghi tên các hệ thống ngoài tham gia package.

**Bảng 3.3. Ma trận tác nhân theo package**

| Package | Khách vãng lai | Người dùng đã đăng nhập | Khách hàng | Nhân viên | Quản trị viên | Hệ thống tác vụ nền | Hệ thống ngoài |
|---|---|---|---|---|---|---|---|
| XT Xác thực | x | x |  | x | x |  | Dịch vụ email; Google |
| HS Hồ sơ cá nhân |  | x |  |  |  |  |  |
| DP Duyệt phim | x |  |  |  |  |  |  |
| DG Đánh giá phim |  |  | x |  |  |  |  |
| DV Đặt vé trực tuyến | x |  | x |  |  |  |  |
| TT Thanh toán trực tuyến |  |  | x |  |  | x | Cổng VNPay; Cổng MoMo; Cổng Stripe; Dịch vụ email |
| AI Trợ lý AI | x |  | x |  |  |  | Gemini |
| BQ Bán vé tại quầy |  |  |  | x | x |  |  |
| SV Soát vé |  |  |  | x | x |  |  |
| QB Quản lý booking |  |  |  | x | x |  |  |
| QP Quản lý phim |  |  |  | x | x |  | Cloudinary |
| QD Quản lý danh mục |  |  |  |  | x |  | Cloudinary |
| QR Quản lý phòng chiếu |  |  |  |  | x |  |  |
| QS Quản lý suất chiếu |  |  |  | x | x |  | Dịch vụ email |
| QN Quản lý người dùng |  |  |  | x | x |  |  |
| QV Quản lý đánh giá |  |  |  |  | x |  |  |
| QT Quản lý thanh toán |  |  |  |  | x |  |  |
| BC Báo cáo thống kê |  |  |  |  | x |  |  |
| CH Cấu hình rạp |  |  |  |  | x |  | Cloudinary |
| TP Tải tệp |  |  |  |  | x |  | Cloudinary |
| HT Vận hành hệ thống | x |  |  |  | x | x | Dịch vụ email; Cổng VNPay; Cổng MoMo; Cổng Stripe |

Ma trận cho thấy quản trị viên tham gia 15 trong 21 package, nhân viên tham gia 7 package, khách hàng 4 package và khách vãng lai 5 package. Nhân viên chỉ xuất hiện ở những package mà policy `StaffOnly` cho phép (BQ, SV, QB, và các use case chỉ đọc của QP, QS, QN), và ở XT với use case đăng nhập trang quản trị. Do quan hệ tổng quát hóa ở Hình 3.1, các tác nhân con còn tham gia thêm những use case của tác nhân cha mà ma trận không đánh dấu riêng: khách hàng, nhân viên và quản trị viên đều thực hiện được các use case của package HS vì kế thừa người dùng đã đăng nhập, và các use case công khai của package DP cũng dùng được khi đã đăng nhập.

<!-- GHI CHÚ ĐỐI CHIẾU -->
<!--
(a) [CẦN BỔ SUNG] còn lại
1. Mã PlantUML của Hình 3.1 (quan hệ giữa các tác nhân) và Hình 3.2 (sơ đồ ngữ cảnh) không có trong `ma-nguon-so-do-bo-sung.md` (file chỉ bắt đầu từ Hình 3.3) và `ma-nguon-so-do.md` chưa được cung cấp. Mục 3.1.2 và 3.1.3 được viết từ mã nguồn backend và quan hệ nêu trong outline, không phải từ mã sơ đồ. Cần đối chiếu lại khi có mã.
2. Ảnh `hinh/3.1.png`, `hinh/3.2.png`, `hinh/3.3.png` cần xuất từ mã sơ đồ.
3. Prompt nhắc file `quy-dinh-trinh-bay.md`; file đính kèm có tên `quy-dinh-format-bao-cao.md` và đã dùng file này (caption hình đặt dưới, caption bảng đặt trên). Cỡ chữ caption trong quy định vẫn đang ở trạng thái [CẦN BỔ SUNG].
4. Không dùng trích dẫn tài liệu tham khảo trong phần này.

(b) Chỗ outline lệch code (theo code)
1. Bảng 3.1, Khách hàng. Outline: quyền `CustomerOnly` cho đặt vé, thanh toán, hủy đơn, viết đánh giá. Code: `CustomerOnly` chỉ có ở 3 action đánh giá của `MoviesController` (POST/PUT/DELETE `reviews`). `BookingsController` chỉ có `[Authorize]` ở cấp lớp (5 action) và `BookingService.HoldSeatsAsync` không kiểm tra vai trò; việc ngăn nhân viên/quản trị dùng cổng khách hàng chỉ thực hiện ở `AuthService.LoginAsync` (cổng `login`). Bảng 3.1 đã sửa theo code; các use case UC-DV-*, UC-TT-01 ở Mục 3.3 vẫn ghi tác nhân "Khách hàng" theo outline.
2. Bảng 3.1, Quản trị viên. Outline: "AdminOnly (kế thừa các quyền của nhân viên)". Code: `AdminOnly` chỉ là `RequireRole(Roles.Admin)`; việc quản trị thực hiện được thao tác của nhân viên là do `StaffOnly` gồm cả `Admin` và `Employee`. Ngoài ra `Policies.EmployeeOnly` tồn tại (Policies.cs, Program.cs dòng 232) nhưng không được dùng ở controller nào; outline chỉ nhắc `CustomerOnly`, `StaffOnly`, `AdminOnly`.
3. Bảng 3.1, Google. Outline ghi "OAuth 2.0"; code (`GoogleAuthService.VerifyIdTokenAsync`) xác minh ID token bằng `GoogleJsonWebSignature`. Đã ghi theo code là "xác minh ID token".
4. Bảng 3.1, Cổng Stripe. Outline ghi "Xác minh chữ ký". Code `StripeGateway.VerifyWebhook`: khi `WebhookSecret` trống, webhook được phân tích bằng `EventUtility.ParseEvent` và trả `signatureValid: true` mà không xác minh chữ ký; chỉ khi có `WebhookSecret` mới gọi `ConstructEvent` (dung sai 300 giây). Bảng 3.1 ghi điều kiện "khi đã cấu hình `WebhookSecret`".
5. Bảng 3.3. Outline chỉ có 5 cột và đánh dấu Khách hàng ở XT và HS, trong khi các bảng 3.3 ghi tác nhân là "Người dùng đã đăng nhập" (UC-XT-08 đến UC-XT-11, UC-HS-01 đến UC-HS-04), không ghi Khách hàng. Đã thêm cột "Người dùng đã đăng nhập", bỏ dấu Khách hàng ở XT và HS, tách "Hệ thống ngoài/nền" thành hai cột. Các hàng còn lại (19 hàng) khớp với outline ở các cột tác nhân người dùng và ở package có hệ thống ngoài/nền.
6. Xác nhận khớp (không lệch): Bảng 3.2 đếm lại từ các dòng `UC-` ở Mục 3.3 cho đúng 103 use case, 21 package, số use case từng package và cột "Tác nhân" đều khớp outline; tập tác nhân của 21 package trong mã PlantUML Hình 3.3 (13 tác nhân, 21 package, 51 liên kết) trùng với cột "Tác nhân" của các bảng ở Mục 3.3.

(c) Quan sát để rà soát ở các phần sau (ngoài phạm vi 3.1–3.2)
1. Số liệu 136 action (29 công khai, 11 `[Authorize]`, 3 `CustomerOnly`, 9 `StaffOnly`, 84 `AdminOnly`) do tôi đếm bằng script từ thuộc tính trong `Controllers/**`, mỗi action tính một lần kể cả khi có hai thuộc tính HTTP (`VnPayIpn`). Cách đếm khác (đếm theo thuộc tính HTTP) sẽ cho 137.
2. Giao diện khách hàng (`features/client/account/components/avatar-upload.tsx`) dùng `useUploadImage` gọi `/admin/uploads/image`, vốn là `AdminOnly` (`AdminUploadsController`). Liên quan tới UC-HS-02 (cập nhật ảnh đại diện) và gợi ý cần kiểm tra lại khi viết Mục 3.3.2.
3. UC-DG-01 ghi tác nhân Khách hàng, nhưng `MoviesController.GetCanReviewStatus` không có thuộc tính xác thực và trả `CanReviewResponse(false)` cho người chưa đăng nhập.
4. `IFileStorage` chỉ được tiêm vào `AdminUploadsController`; các package QP, QD, CH được outline gắn với Cloudinary thông qua các form giao diện gọi endpoint tải lên (movie, combo, settings).
5. Chú thích trong `Program.cs` ghi Hangfire dùng "SQL Server" nhưng mã dùng `UsePostgreSqlStorage`; mã được ưu tiên.
-->

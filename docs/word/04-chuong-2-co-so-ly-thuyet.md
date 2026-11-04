## 2.5 CƠ SỞ LÝ THUYẾT

### 2.5.1 Mô hình hóa yêu cầu bằng use case

Mô hình use case mô tả hệ thống từ góc nhìn của người dùng bên ngoài. Tác nhân (actor) là một người hoặc một hệ thống bên ngoài tương tác với hệ thống; use case là một chức năng mà hệ thống cung cấp cho tác nhân để đạt một mục đích. Giữa các use case có ba loại quan hệ: `include` (use case này luôn gọi use case kia), `extend` (use case kia chỉ mở rộng trong một điều kiện nào đó) và tổng quát hóa (một use case là trường hợp riêng của use case khác). [CẦN BỔ SUNG: trích dẫn nhóm T2]

Mô hình use case phù hợp với đề tài vì hệ thống có nhiều nhóm người dùng với quyền khác nhau (khách hàng, nhân viên, quản trị) và có cả hệ thống ngoài (cổng thanh toán); cách mô tả này cho phép liệt kê chức năng theo từng nhóm trước khi thiết kế. Trong mã nguồn, ba vai trò được khai báo ở lớp `Roles` là `Customer`, `Admin` và `Employee`.

Perfo áp dụng use case theo hai mức: package use case (nhóm chức năng) và use case. Quan hệ `include`, `extend` và tổng quát hóa chỉ được vẽ giữa các use case ở mức use case. Các tác nhân được xác định bằng cách đọc thuộc tính `[Authorize]` và policy ở từng controller chứ không suy diễn (xem Mục 3.1). Mục này làm cơ sở cho Chương 3.

### 2.5.2 Mô hình thực thể liên kết

Mô hình thực thể liên kết biểu diễn dữ liệu bằng các thực thể, thuộc tính của thực thể, khóa (khóa chính và khóa ngoại) và các quan hệ giữa thực thể với bội số 1–1, 1–n hoặc n–n. Mô hình được chuẩn hóa để giảm dư thừa và dị thường khi cập nhật, rồi ánh xạ sang các quan hệ (bảng) của cơ sở dữ liệu quan hệ. [CẦN BỔ SUNG: trích dẫn nhóm T3]

Bài toán đặt vé có dữ liệu quan hệ rõ ràng (phim, suất chiếu, phòng, ghế, đơn đặt vé, vé, thanh toán) và cần các ràng buộc toàn vẹn mạnh, nên cơ sở dữ liệu quan hệ là lựa chọn phù hợp.

Trong Perfo, mỗi thực thể được cài đặt bằng một lớp trong `Cinema.Domain/Entities` và cấu hình ánh xạ bằng một lớp `IEntityTypeConfiguration<T>` trong `Cinema.Infrastructure/Persistence/Configurations`. Các cấu hình này khai báo khóa, khóa ngoại với hành vi xóa (`DeleteBehavior`), chỉ mục duy nhất và ràng buộc kiểm tra. Ví dụ, `ShowtimeSeatConfiguration` khai báo chỉ mục duy nhất `UQ_Showtime_Seat` trên cặp (`ShowtimeId`, `SeatId`), và `BookingTicketConfiguration` có chỉ mục duy nhất trên `ShowtimeSeatId` để mỗi ghế của suất chiếu chỉ gắn với tối đa một vé. Mục này làm cơ sở cho Mục 4.3.

### 2.5.3 Kiến trúc phân tầng

Kiến trúc phân tầng chia hệ thống thành các tầng có trách nhiệm riêng, trong đó mỗi tầng chỉ phụ thuộc vào tầng được phép. Trong Clean Architecture, chiều phụ thuộc hướng vào lõi: lõi nghiệp vụ không biết gì về cơ sở dữ liệu, giao thức web hay dịch vụ ngoài; các chi tiết đó được cài đặt ở tầng ngoài và gắn vào lõi qua giao diện (interface). [CẦN BỔ SUNG: trích dẫn nhóm T2]

Cách tổ chức này phù hợp với đề tài vì các phần dễ thay đổi (cổng thanh toán, kênh gửi email, kênh thời gian thực) được tách khỏi phần nghiệp vụ giữ ghế và thanh toán, nhờ đó phần nghiệp vụ có thể được kiểm thử đơn vị mà không cần các dịch vụ ngoài.

Phần backend của Perfo gồm năm project. Quan hệ `ProjectReference` trong các tệp `.csproj` như sau: `Cinema.Shared` không tham chiếu project nào; `Cinema.Domain` tham chiếu `Cinema.Shared`; `Cinema.Application` tham chiếu `Cinema.Domain` và `Cinema.Shared`; `Cinema.Infrastructure` tham chiếu `Cinema.Application`, `Cinema.Domain` và `Cinema.Shared`; `Cinema.Api` tham chiếu cả bốn project còn lại. Mọi mũi tên phụ thuộc đều hướng từ ngoài vào trong, không có tham chiếu vòng. Hình 2.3 biểu diễn mô hình này.

![Hình 2.3 Mô hình phân tầng các project](hinh/2.3.png)

*Hình 2.3. Mô hình phân tầng các project*

Tầng `Cinema.Application` khai báo các giao diện như `IPaymentGateway`, `IRealtimeNotifier` và `IGoogleAuthService`; tầng `Cinema.Infrastructure` cài đặt chúng bằng `VnPayGateway`, `MomoGateway`, `StripeGateway`, `SignalRRealtimeNotifier` và `GoogleAuthService`. `Cinema.Api` chứa controller, middleware và `Program.cs`, nơi đăng ký các tầng qua `AddApplication()` và `AddInfrastructure(...)`. Cần lưu ý `Cinema.Application` vẫn tham chiếu trực tiếp `Microsoft.EntityFrameworkCore` (qua giao diện `IAppDbContext`) và `Hangfire.Core`, nên tầng này không hoàn toàn độc lập với công nghệ như mô hình Clean Architecture thuần. Mục này làm cơ sở cho Mục 4.1.2 và Mục 4.2.

### 2.5.4 REST API

REST là phong cách thiết kế API trên giao thức HTTP, trong đó tài nguyên được định danh bằng đường dẫn, thao tác được biểu diễn bằng phương thức HTTP (GET, POST, PUT, DELETE) và kết quả được biểu diễn bằng mã trạng thái HTTP (ví dụ 200, 400, 401, 403, 404, 409, 429). Các kỹ thuật thường đi kèm là đưa phiên bản vào đường dẫn, bọc phản hồi theo một cấu trúc thống nhất, và phân trang danh sách bằng chỉ số trang (offset) hoặc con trỏ (cursor). [CẦN BỔ SUNG: trích dẫn nhóm T3]

REST phù hợp vì giao diện web và các công cụ kiểm thử đều gọi hệ thống qua HTTP, và cổng thanh toán gọi ngược về hệ thống cũng qua HTTP.

Perfo đặt các controller dưới `api/v1/...`; các điểm cuối quản trị nằm dưới `api/v1/admin/...`, và Swagger tách hai tài liệu `client` và `admin` theo tiền tố này (`DocInclusionPredicate`). Mọi phản hồi thành công được bọc bởi lớp `ApiResponse` (`Cinema.Shared/Results/ApiResponse.cs`) gồm bốn trường `Success`, `Data`, `Message`, `Errors`; việc bọc do `ApiResponseFilter` thực hiện với các `ObjectResult` có mã dưới 400, trừ các action gắn `RawResponseAttribute`. Lỗi nghiệp vụ được ném thành ngoại lệ `AppException` và `ExceptionHandlingMiddleware` chuyển thành phản hồi lỗi cùng kiểu; xung đột ghế và xung đột nghiệp vụ trả về mã 409, vượt giới hạn tốc độ trả về mã 429 (`RejectionStatusCode`). Với danh sách, `CursorPageResult<T>` (`Items`, `NextCursor`, `HasMore`) được dùng cho danh sách đặt vé của người dùng (`GetMyBookingsAsync`) và `OffsetPageResult<T>` (`Page`, `PageSize`, `TotalItems`, `TotalPages`) được dùng cho danh sách phía quản trị (`GetBookingsAdminAsync`). Mục này làm cơ sở cho Mục 4.1.4 và Mục 4.10.

### 2.5.5 ASP.NET Core, Entity Framework Core, PostgreSQL

ASP.NET Core xử lý mỗi yêu cầu HTTP qua một chuỗi middleware theo thứ tự đăng ký, và cung cấp sẵn tiêm phụ thuộc (DI) cùng mẫu Options để gắn các mục cấu hình vào lớp cấu hình có kiểu. Entity Framework Core là bộ ánh xạ đối tượng–quan hệ có hỗ trợ migration để quản lý phiên bản lược đồ, và các lệnh `ExecuteUpdate`/`ExecuteDelete` để cập nhật hàng loạt bằng một câu lệnh SQL. PostgreSQL là hệ quản trị cơ sở dữ liệu quan hệ hỗ trợ ràng buộc `CHECK`, chỉ mục duy nhất và kiểm soát đồng thời đa phiên bản. [CẦN BỔ SUNG: trích dẫn nhóm T3]

Bộ ba này phù hợp vì các ràng buộc quan trọng (một ghế một dòng trạng thái, trạng thái hợp lệ) có thể đặt ngay trong cơ sở dữ liệu, và việc giữ ghế có thể viết thành một câu lệnh cập nhật có điều kiện.

Trong `Program.cs`, các middleware được đăng ký theo thứ tự: `UseForwardedHeaders`, bật bộ đệm thân yêu cầu, `UseSerilogRequestLogging`, `UseAppExceptionHandling`, Swagger (chỉ ở môi trường Development), `UseCors("Default")`, `UseRateLimiter` (không áp dụng ở môi trường `Testing`), `UseAuthentication`, `UseAuthorization`, rồi `MapControllers`. Các mục cấu hình như `Booking`, `Jwt`, `RateLimit`, `Scheduler`, `BackgroundJobs` được gắn vào các lớp `BookingOptions`, `JwtOptions`, `RateLimitOptions`, `SchedulerOptions`, `BackgroundJobOptions` bằng `Configure<T>`. FluentValidation được đăng ký bằng `AddValidatorsFromAssembly` và Mapster bằng `AddMapsterConfig()`. Cơ sở dữ liệu dùng Npgsql; migration có thể chạy lúc khởi động khi `RunMigrationsOnStartup` là `true` (giá trị trong `appsettings.json` là `false`, trong `docker-compose.yml` và `render.yaml` là `true`). Các cột thời gian dùng kiểu `timestamp without time zone` và `Program.cs` bật công tắc `Npgsql.EnableLegacyTimestampBehavior`. Mục này làm cơ sở cho Mục 4.3.

### 2.5.6 Xác thực bằng JWT

JSON Web Token (JWT) gồm ba phần là tiêu đề, nội dung (các claim) và chữ ký; máy chủ kiểm tra chữ ký và thời hạn mà không cần tra cứu phiên. Mô hình thường dùng là access token sống ngắn để gọi API và refresh token sống dài để xin access token mới. Refresh token nên được xoay vòng (mỗi lần dùng cấp một token mới và thu hồi token cũ), được phát hiện khi bị dùng lại, và chỉ lưu dưới dạng băm trong cơ sở dữ liệu. [CẦN BỔ SUNG: trích dẫn nhóm T4]

Cách này phù hợp vì API không giữ phiên phía máy chủ, thuận tiện cho triển khai trên một container nhỏ.

`JwtTokenService` ký access token bằng `HmacSha256` với các claim `UserId`, `Role` (theo cả `AppClaimTypes.Role` và `ClaimTypes.Role`), `sub`, `email` và `EmployeeId` (nếu là nhân viên). Thời hạn access token là `AccessTokenExpirationSeconds` (3600 giây trong `appsettings.json`), refresh token là `RefreshTokenExpirationDays` (30 ngày) và token cho kết nối thời gian thực là `SocketTokenExpirationSeconds` (300 giây). Refresh token không phải JWT mà là chuỗi ngẫu nhiên 32 byte (`GenerateOpaqueToken`), được lưu dưới dạng SHA-256 thập lục phân (`HashOpaqueToken`). Trong `AuthService.RefreshAsync`, mỗi lần làm mới, một token mới được tạo, token cũ được đánh dấu `IsRevoked` và ghi `ReplacedByTokenId`. Nếu một token đã bị thu hồi mà vẫn được gửi lại, `RevokeTokenChainAsync` thu hồi toàn bộ chuỗi token kế tiếp và nhật ký ghi `RefreshTokenReuseDetected`. Ở phía API, `OnTokenValidated` tra bảng `Users` ở mỗi yêu cầu để từ chối tài khoản đã bị khóa (`IsActive`). Mục này làm cơ sở cho Mục 4.4.

### 2.5.7 Phân quyền theo vai trò

Phân quyền theo vai trò (RBAC) gán quyền cho vai trò và gán vai trò cho người dùng, thay vì gán quyền trực tiếp từng người; ASP.NET Core biểu diễn các quy tắc này bằng policy. OAuth 2.0 là khung ủy quyền cho phép ứng dụng nhận kết quả xác thực từ nhà cung cấp danh tính bên ngoài như Google. Mật khẩu phải được lưu dưới dạng băm có muối chứ không lưu bản rõ. [CẦN BỔ SUNG: trích dẫn nhóm T4]

Hệ thống có ba nhóm người dùng với quyền rõ rệt (khách hàng, nhân viên quầy, quản trị), nên RBAC theo policy là phù hợp; đăng nhập bằng Google giúp người dùng không phải tạo thêm mật khẩu.

`Program.cs` định nghĩa bốn policy dựa trên `Roles`: `CustomerOnly` (`Customer`), `AdminOnly` (`Admin`), `EmployeeOnly` (`Employee`) và `StaffOnly` (`Admin` hoặc `Employee`). Mật khẩu được băm bằng `IPasswordHasher<User>`, cài đặt là `PasswordHasher<User>` trong `Microsoft.Extensions.Identity.Core` (`AuthService` gọi `HashPassword` và `VerifyHashedPassword`). Với Google, giao diện gửi mã thông báo ID của Google lên backend và `GoogleAuthService.VerifyIdTokenAsync` kiểm tra bằng `GoogleJsonWebSignature.ValidateAsync` với `Audience` là `Google:ClientId`; nếu `Google:ClientId` chưa được cấu hình, hệ thống ném `BusinessRuleException`, nếu mã thông báo không hợp lệ thì ném `UnauthorizedAppException`. Mục này làm cơ sở cho Mục 4.4.8.

### 2.5.8 Kiểm soát đồng thời

Giao dịch cơ sở dữ liệu bảo đảm bốn tính chất ACID: nguyên tử, nhất quán, cô lập và bền vững. Mức cô lập quyết định mức độ các giao dịch đồng thời nhìn thấy nhau; PostgreSQL cài đặt kiểm soát đồng thời đa phiên bản (MVCC) để người đọc không chặn người ghi. Bên cạnh giao dịch, có thể bảo vệ dữ liệu bằng cập nhật có điều kiện (kiểm tra số dòng bị ảnh hưởng), ràng buộc duy nhất ở cơ sở dữ liệu, và thiết kế thao tác idempotent để xử lý lặp lại không đổi kết quả. [CẦN BỔ SUNG: trích dẫn nhóm T5]

Các kỹ thuật này đáp ứng trực tiếp mục tiêu MT2 và MT3 (Bảng 1.1): giữ ghế không trùng và xử lý thông báo thanh toán lặp.

Perfo áp dụng như sau. Mã nguồn không đặt mức cô lập riêng mà dùng mức mặc định của kết nối (không có `IsolationLevel` trong `Cinema.Application` và `Cinema.Infrastructure`). Việc giữ ghế dùng cập nhật có điều kiện trong giao dịch và so số dòng bị ảnh hưởng (`CreateHoldBookingAsync`, xem Mục 2.2.2). Chỉ mục duy nhất `UQ_Showtime_Seat` và chỉ mục duy nhất trên `BookingTickets.ShowtimeSeatId` là lớp bảo vệ ở cơ sở dữ liệu. Tính idempotent thể hiện ở ba chỗ: `PaymentService.IsAlreadyProcessed` bỏ qua thông báo thanh toán đã xử lý; `ConfirmBookingAndGenerateTicketsAsync` trả về ngay nếu đơn đã `Confirmed`; và `CancelBookingAsync` chuyển đơn từ `Confirmed` sang `Cancelled` bằng một câu lệnh cập nhật có điều kiện, nếu số dòng bị ảnh hưởng bằng 0 thì ném `ConflictAppException` (mã 409). Mục này làm cơ sở cho Mục 4.5.1.

### 2.5.9 Truyền thông thời gian thực với SignalR

WebSocket là kết nối hai chiều lâu dài giữa trình duyệt và máy chủ, cho phép máy chủ đẩy dữ liệu mà không cần máy khách hỏi liên tục. SignalR là thư viện của ASP.NET Core bao bọc WebSocket (và các phương án dự phòng); máy khách kết nối tới một hub, và máy chủ gửi thông điệp tới từng máy khách hoặc tới một nhóm (group). Do WebSocket của trình duyệt không đặt được tiêu đề `Authorization`, token thường được gửi qua tham số truy vấn `access_token`. [CẦN BỔ SUNG: trích dẫn nhóm T6]

Sơ đồ ghế cần cập nhật ngay khi người khác giữ, đặt hoặc nhả ghế, nếu không người dùng sẽ chọn ghế đã mất và nhận lỗi 409.

`ShowtimeHub` được đánh dấu `[Authorize]` và có hai phương thức `JoinShowtime` và `LeaveShowtime`, đưa kết nối vào hoặc ra khỏi nhóm `showtime-{showtimeId}`; `showtimeId` không dương thì ném `HubException`. Hub được ánh xạ tại `/hubs/showtimes`. `SignalRRealtimeNotifier` gửi bốn sự kiện tới nhóm của suất chiếu: `seat.held`, `seat.booked`, `seat.released` và `hold.expired`. Trong `Program.cs`, `OnMessageReceived` chỉ lấy token từ `access_token` khi đường dẫn bắt đầu bằng `/hubs`. Ở frontend, `use-showtime-hub.ts` tạo `HubConnectionBuilder`, lấy token qua `authApi.getSocketToken()` (đường dẫn `socket-token`, token 300 giây; nếu lỗi thì dùng access token hiện có), bật `withAutomaticReconnect` và gọi `JoinShowtime` sau khi kết nối. Mục này làm cơ sở cho Mục 4.7.

### 2.5.10 Xử lý tác vụ nền với Hangfire

Hangfire là thư viện chạy tác vụ nền trong ứng dụng .NET, lưu hàng đợi và lịch trên một kho dữ liệu bền vững. Có ba kiểu tác vụ thường dùng: tác vụ lặp theo biểu thức cron, tác vụ chạy một lần không chờ kết quả (fire-and-forget), và cơ chế tự thử lại khi tác vụ lỗi. [CẦN BỔ SUNG: trích dẫn nhóm T7]

Việc nhả ghế hết hạn, cập nhật vòng đời suất chiếu, hết hạn điểm thành viên và gửi email đều không nên chạy trong yêu cầu của người dùng; Hangfire dùng chính PostgreSQL của hệ thống nên không cần thêm hạ tầng.

Trong `Program.cs`, Hangfire dùng `UsePostgreSqlStorage`, và `AutomaticRetryAttribute` được đặt làm bộ lọc toàn cục với `Attempts = RetryAttempts` (3) và `DelaysInSeconds = RetryDelaysSeconds` ([30, 120, 600]). Ba tác vụ lặp được đăng ký bằng `RecurringJob.AddOrUpdate` theo múi giờ `VietnamTime.Zone`: `ReleaseExpiredSeatsJob` (cron `* * * * *`), `ShowtimeLifecycleJob` (`* * * * *`) và `ExpireLoyaltyPointsJob` (`5 * * * *`). Các tác vụ gửi một lần gồm `SendEmailJob` (gọi từ `AuthService`, `BookingService`, `PaymentService`, `ShowtimeService`) và `ShowtimeRefundJob` (gọi từ `ShowtimeService`), đưa vào hàng đợi bằng `Enqueue<T>`. Bảng điều khiển Hangfire đặt tại `/admin/hangfire` với `HangfireAdminAuthorizationFilter`. Ở môi trường `Testing`, Hangfire và các tác vụ lặp không được đăng ký. Mục này làm cơ sở cho Mục 4.5.3.

### 2.5.11 Tích hợp cổng thanh toán

Cổng thanh toán trực tuyến thường hoạt động theo mô hình chuyển hướng và gọi ngược: hệ thống tạo giao dịch rồi chuyển người dùng sang trang thanh toán của cổng; sau khi người dùng thanh toán, cổng chuyển người dùng về địa chỉ trả về và đồng thời gửi thông báo máy chủ tới máy chủ (IPN hoặc webhook) để xác nhận kết quả. Thông báo được ký bằng mã xác thực thông điệp dựa trên băm (HMAC); VNPay dùng HMAC-SHA512, MoMo dùng HMAC-SHA256 và Stripe dùng chữ ký trong tiêu đề webhook. Vì cổng có thể gửi lặp nên xử lý thông báo phải idempotent; hoàn tiền được thực hiện bằng một lời gọi riêng tới cổng. [CẦN BỔ SUNG: trích dẫn nhóm T8]

Ba cổng được chọn là VNPay và MoMo (phổ biến trong nước, có môi trường sandbox) và Stripe (thanh toán thẻ quốc tế).

Giao diện `IPaymentGateway` gồm `CreatePaymentAsync`, `VerifyCallback`, `VerifyWebhook` và `RefundAsync`, được ba lớp `VnPayGateway`, `MomoGateway` và `StripeGateway` cài đặt. `HmacSigner` cung cấp `Sha256Hex` và `Sha512Hex`. `VnPayGateway` ký `vnp_SecureHash` bằng SHA-512 trên các tham số đã sắp theo thứ tự ordinal; `MomoGateway` ký tham số `signature` bằng SHA-256; `StripeGateway` tạo phiên Checkout (`Mode = "payment"`), kiểm tra webhook bằng `EventUtility.ConstructEvent` và xử lý sự kiện `checkout.session.completed`. Ba điểm cuối nhận thông báo nằm trong `PaymentsController`: `GET`/`POST api/v1/payments/vnpay/ipn`, `POST api/v1/payments/momo/ipn` và `POST api/v1/payments/stripe/webhook`. Trình tự xử lý của `PaymentService` gồm kiểm tra chữ ký, tìm giao dịch, bỏ qua thông báo đã xử lý, đối chiếu số tiền (và loại tiền với Stripe) rồi mới ghi kết quả (xem Mục 1.1.2). Mục này làm cơ sở cho Mục 4.5.2.

### 2.5.12 Công nghệ giao diện

Ứng dụng một trang (SPA) tải mã giao diện một lần rồi cập nhật nội dung bằng các lời gọi API. React xây dựng giao diện từ các thành phần; TanStack Query quản lý dữ liệu lấy từ máy chủ (bộ nhớ đệm, trạng thái tải); Zustand quản lý trạng thái cục bộ của ứng dụng; React Hook Form kết hợp Zod để kiểm tra biểu mẫu; Tailwind CSS cung cấp các lớp tiện ích để dựng giao diện. [CẦN BỔ SUNG: trích dẫn nhóm T9]

Hệ thống có cả giao diện khách hàng và trang quản trị với nhiều biểu mẫu và bảng dữ liệu, nên cần các thư viện chuyên trách cho dữ liệu máy chủ, biểu mẫu và bảng.

Trong mã nguồn `frontend/`, React 19.2.8 và Vite 8.2.2 được cấu hình trong `vite.config.ts` cùng plugin `@tailwindcss/vite` và `reactCompilerPreset` (React Compiler). TanStack Query được dùng trong 33 tệp (`useQuery`, `useMutation`); React Hook Form với `zodResolver` được dùng trong 43 tệp; Zustand được dùng trong 4 tệp (ví dụ `auth-store.ts`, `dialog-store.ts`); TanStack Table (`@tanstack/react-table`) được dùng trong `components/common/data-table`; `axios` được dùng trong 26 tệp để gọi API. Thư viện giao diện gồm shadcn và `@base-ui/react`; `react-router` 7.18.2 xử lý định tuyến; `qrcode.react` và `jsqr` có trong phụ thuộc cho mã QR; `recharts` cho biểu đồ. Mục này làm cơ sở cho Mục 5.4.

### 2.5.13 Trợ lý AI

Mô hình ngôn ngữ lớn (LLM) có thể gọi hàm (function calling): ứng dụng khai báo các hàm mà mô hình được phép yêu cầu, mô hình trả về lời gọi hàm kèm tham số, ứng dụng thực thi hàm rồi trả kết quả lại cho mô hình, và vòng lặp này lặp đến khi mô hình trả lời bằng văn bản. Các rủi ro gồm chèn lệnh (prompt injection), lộ dữ liệu cá nhân và chi phí gọi mô hình. [CẦN BỔ SUNG: trích dẫn nhóm T10]

Người dùng thường hỏi về suất chiếu, ghế còn trống và đơn của mình; gọi hàm cho phép trợ lý trả lời bằng dữ liệu thật thay vì để mô hình tự suy đoán.

`GeminiAiAssistantService.ChatAsync` gọi điểm cuối `generateContent` của Gemini (mô hình `gemini-2.5-flash` trong cấu hình) qua HTTP. Khóa API lấy từ `Gemini:ApiKey` hoặc biến môi trường `GEMINI_API_KEY`; nếu thiếu thì dịch vụ trả về thông báo chưa cấu hình. Vòng lặp gọi hàm bị chặn bởi `MaxToolRounds = 4`, `MaxTotalToolCalls = 8`, và số thẻ trả về tối đa `MaxCards = 6`. `AiToolExecutor` chỉ thực thi ba hàm khai báo sẵn: `search_showtimes`, `get_seat_availability` và `get_my_bookings`; hàm cuối kiểm tra `userId` trước khi truy vấn và chỉ lọc đơn của người dùng đó. `AiController` (`POST api/v1/ai/chat`) không gắn `[Authorize]` và không có chính sách giới hạn tốc độ riêng, nên khách chưa đăng nhập vẫn gọi được; chỉ giới hạn toàn cục theo IP (`GlobalPermitLimit`) áp dụng. Mục này làm cơ sở cho Mục 4.8.

### 2.5.14 Kiểm thử phần mềm

Kim tự tháp kiểm thử khuyến nghị nhiều kiểm thử đơn vị ở đáy (nhanh, rẻ), ít hơn kiểm thử tích hợp ở giữa, và ít nhất kiểm thử đầu cuối ở đỉnh (chậm, tốn kém nhưng sát hành vi người dùng). Kiểm thử hiệu năng đo độ trễ và thông lượng khi nhiều người dùng truy cập đồng thời. [CẦN BỔ SUNG: trích dẫn nhóm T11]

Đề tài cần kiểm chứng các hành vi phụ thuộc vào cơ sở dữ liệu thật (ràng buộc, cập nhật có điều kiện) nên ngoài kiểm thử đơn vị còn cần kiểm thử tích hợp trên PostgreSQL.

Bộ kiểm thử backend dùng xUnit 2.9.3 và Shouldly 4.3.0. `Cinema.UnitTests` dùng thêm Moq 4.20.72 và `Microsoft.EntityFrameworkCore.Sqlite` 10.0.11. `Cinema.IntegrationTests` dùng `Microsoft.AspNetCore.Mvc.Testing` 10.0.11, `Testcontainers.PostgreSql` 4.4.0 (`CinemaApiFactory` khởi tạo container `postgres:16-alpine`), Respawn 7.0.0, `Microsoft.AspNetCore.SignalR.Client` 10.0.11 và `Hangfire.InMemory` 1.0.0. Kiểm thử đầu cuối dùng Playwright 1.63.0 với cấu hình trong `frontend/playwright.config.ts`: `testDir` là `./e2e`, `timeout` 60 giây, `workers` 1, `fullyParallel` false, một dự án `chromium` (Desktop Chrome), `trace` bật ở lần thử lại đầu tiên và chụp màn hình khi lỗi. Kiểm thử tải dùng k6 theo `package.json`, nhưng thư mục `load-tests/` chưa có trong dự án (xem Mục 2.4.2). Mục này làm cơ sở cho Chương 6.

### 2.5.15 Container hóa

Container đóng gói ứng dụng cùng phụ thuộc thành một ảnh chạy giống nhau ở mọi môi trường. Dockerfile nhiều giai đoạn (multi-stage) dùng ảnh nặng để dựng và ảnh nhẹ để chạy. PaaS cho phép triển khai ảnh container mà không quản lý máy chủ; trang web tĩnh có thể đặt trên dịch vụ lưu trữ tĩnh; PostgreSQL dạng serverless là dịch vụ cơ sở dữ liệu được cấp phát theo nhu cầu. [CẦN BỔ SUNG: trích dẫn nhóm T12]

Đề tài chạy trên tài nguyên giới hạn nên cần ảnh nhỏ và cấu hình bộ nhớ chặt chẽ.

`backend/Dockerfile` có bốn giai đoạn: `restore`, `build`, `dev` và `production`, dựa trên ảnh `mcr.microsoft.com/dotnet/sdk` và `aspnet` bản `10.0-alpine`. Ảnh `production` chạy bằng người dùng không có quyền root (`appuser`), cổng 8080 và đặt `DOTNET_GCConserveMemory=9`, `DOTNET_GCHeapHardLimit=402653184` (384 MB) và `DOTNET_GCDynamicAdaptationMode=0`; chú thích trong tệp ghi rõ cấu hình này dành cho môi trường RAM khoảng 512 MB. `docker-compose.yml` dựng hai dịch vụ `api` và `frontend` ở giai đoạn `dev`, kết nối cơ sở dữ liệu Neon qua biến `DATABASE_URL` và có kiểm tra sức khỏe tại `/api/v1/health`. `render.yaml` khai báo dịch vụ web `cinema-api` (Docker, gói `free`, vùng `singapore`) và dịch vụ tĩnh `cinema-web` (có quy tắc `rewrite` về `/index.html` và các tiêu đề `Cache-Control`, `X-Robots-Tag`, `X-Content-Type-Options`); `vercel.json` cấu hình tương đương cho lưu trữ tĩnh trên Vercel. Mục này làm cơ sở cho Mục 5.7.

## 2.6 CÔNG NGHỆ SỬ DỤNG

Bảng 2.3 tổng hợp công nghệ và phiên bản. Phiên bản backend lấy từ thuộc tính `Version` của `PackageReference` và `TargetFramework` trong các tệp `.csproj`; phiên bản công cụ dòng lệnh lấy từ `dotnet-tools.json`. Phiên bản frontend là phiên bản đã khóa trong `frontend/package-lock.json`; `package.json` khai báo bằng ký hiệu `^` (ví dụ `^19.2.8`) và riêng TypeScript bằng `~6.0.2`.

**Bảng 2.3. Công nghệ sử dụng và phiên bản**

| Nhóm | Công nghệ | Phiên bản |
|---|---|---|
| Nền tảng | .NET / ASP.NET Core | net10.0 |
| ORM | Entity Framework Core / Npgsql | 10.0.11 / 10.0.3 |
| Cơ sở dữ liệu | PostgreSQL (dịch vụ Neon; kiểm thử tích hợp dùng ảnh `postgres:16-alpine`) | [CẦN BỔ SUNG: phiên bản PostgreSQL trên Neon] |
| Tác vụ nền | Hangfire / Hangfire.PostgreSql | 1.8.24 / 1.21.1 |
| Kiểm tra dữ liệu | FluentValidation | 12.1.1 |
| Ánh xạ | Mapster | 10.0.11 |
| Nhật ký | Serilog.AspNetCore | 10.0.0 |
| Tài liệu API | Swashbuckle | 10.2.3 |
| Xác thực | Microsoft.AspNetCore.Authentication.JwtBearer / System.IdentityModel.Tokens.Jwt | 10.0.11 / 8.22.0 |
| Thời gian thực (máy chủ) | Microsoft.AspNetCore.SignalR.Core | 1.2.12 |
| Mã QR | QRCoder | 1.8.0 |
| Email | MailKit | 4.17.0 |
| Thanh toán | Stripe.net | 52.4.0 |
| Lưu trữ tệp | CloudinaryDotNet | 1.29.3 |
| Đăng nhập Google | Google.Apis.Auth | 1.76.0 |
| Công cụ .NET | dotnet-ef / CSharpier | 10.0.11 / 1.3.0 |
| Giao diện | React / Vite / TypeScript | 19.2.8 / 8.2.2 / 6.0.3 |
| CSS | Tailwind CSS | 4.3.3 |
| Dữ liệu phía client | TanStack Query / React Router / Zod | 5.102.8 / 7.18.2 / 3.25.76 |
| Biểu mẫu và trạng thái | React Hook Form / Zustand | 7.86.0 / 5.0.15 |
| Thời gian thực (máy khách) | @microsoft/signalr | 10.0.11 |
| Biểu đồ | Recharts | 3.8.0 |
| Kiểm thử backend | xUnit / Moq / Shouldly / Testcontainers.PostgreSql | 2.9.3 / 4.20.72 / 4.3.0 / 4.4.0 |
| Kiểm thử E2E | Playwright | 1.63.0 |
| Container | Ảnh .NET SDK/ASP.NET (alpine); ảnh Node (alpine) | 10.0; 22 |
| Trợ lý AI | Gemini (mô hình theo cấu hình `Gemini:Model`) | gemini-2.5-flash |

<!-- GHI CHÚ ĐỐI CHIẾU
(a) [CẦN BỔ SUNG] còn lại:
- Trích dẫn khái niệm: nhóm T2 (2.5.1, 2.5.3), T3 (2.5.2, 2.5.4, 2.5.5), T4 (2.5.6, 2.5.7), T5 (2.5.8), T6 (2.5.9), T7 (2.5.10), T8 (2.5.11), T9 (2.5.12), T10 (2.5.13), T11 (2.5.14), T12 (2.5.15). Không tạo tên sách, RFC, URL hay năm.
- Bảng 2.3: phiên bản PostgreSQL trên Neon (repo không ghi; chỉ biết ảnh kiểm thử `postgres:16-alpine`).
- Hình 2.3: chưa có `ma-nguon-so-do.md`; mô tả dựng từ ProjectReference thật. Cạnh phụ thuộc cần vẽ: Shared (không phụ thuộc); Domain → Shared; Application → Domain, Shared; Infrastructure → Application, Domain, Shared; Api → Application, Domain, Infrastructure, Shared.
- 2.5.1: phần "Perfo áp dụng" lấy từ outline (hai mức package/use case); chưa đối chiếu Chương 3 vì ngoài phạm vi.

(b) Chỗ outline lệch code hoặc cần lưu ý:
- 2.5.4: outline ghi `ApiResponse<T>`; code là lớp không generic `ApiResponse` (`Data` kiểu `object?`), đúng theo code.
- 2.5.3: outline nêu "chiều phụ thuộc hướng vào lõi"; thực tế `Cinema.Domain` còn tham chiếu `Cinema.Shared`, và `Cinema.Application` tham chiếu trực tiếp EF Core và `Hangfire.Core`. Đã nêu ở 2.5.3. Comment trong `Program.cs` còn nhắc "MediatR (CQRS handlers)" nhưng không có package MediatR trong csproj; chưa dùng làm nguồn.
- 2.5.5: `RunMigrationsOnStartup`: `appsettings.json` là `false`, mã đọc mặc định `true` nếu thiếu cấu hình, compose và render đặt `true`.
- 2.5.6: refresh token là chuỗi ngẫu nhiên (opaque), không phải JWT; outline nói "băm token", khớp. Chưa rà cách frontend lưu refresh token (cookie hay bộ nhớ) vì thuộc Mục 4.4/5.6; outline Mục 8.3 nhắc "refresh token qua cookie HttpOnly".
- 2.5.7: outline ghi "OAuth 2.0 với Google"; code chỉ xác minh ID token của Google (`GoogleJsonWebSignature`); `Google:ClientSecret` có trong cấu hình nhưng chưa thấy dùng ở `GoogleAuthService`.
- 2.5.9: `SignalRRealtimeNotifier.NotifySeatHeldAsync` nhận `heldByUserId` nhưng không đưa vào payload gửi đi (chỉ `showtimeId`, `seatIds`, `heldUntil`); kiểu `SeatHeldPayload` ở `frontend/src/lib/signalr.ts` khai báo thêm `heldByUserId`. Cần đối chiếu ở Mục 4.7.
- 2.5.11: Stripe: code xử lý `checkout.session.completed`; các loại sự kiện khác trả "Event ignored". Chưa xác nhận Stripe dùng khóa test (cấu hình không có URL sandbox).
- 2.5.12: `package.json` ở thư mục gốc và `frontend/package.json` cùng khai báo các thư viện giao diện (nội dung phụ thuộc giống nhau); mô tả ở root nói đây là "package.json duy nhất của repo". Phiên bản lấy từ `frontend/package-lock.json`. TypeScript: `package.json` `~6.0.2`, đã khóa `6.0.3` (outline ghi `~6.0.2`). Các phiên bản outline ghi rút gọn (5.102, 7.18, 3.25, 3.8, 1.63) đã thay bằng phiên bản đầy đủ. `jsqr`, `qrcode.react`, `recharts` chỉ xác nhận có trong phụ thuộc, chưa rà nơi dùng.
- 2.5.13: `AiController` không có `[Authorize]` và không có policy giới hạn tốc độ riêng; liên quan hạn chế "xác thực và giới hạn tốc độ riêng cho AI" ở Mục 8.3. Chưa đọc `BuildSystemPromptAsync` và cách xử lý dữ liệu cá nhân/chống chèn lệnh nên không khẳng định về các biện pháp này.
- 2.5.14: outline ghi xUnit/Moq/Shouldly/Testcontainers/Playwright/k6; k6 chỉ có ở `package.json` (script gọi `load-tests/run-load-test.ps1`, thư mục không có trong zip). Thư mục `e2e` không có trong zip nên chưa xác nhận số kịch bản.
- 2.5.15: tên `cinema-api`, `cinema-web` trong `render.yaml` và `name: cinema` trong `docker-compose.yml` là tên cũ trước khi đổi sang perfo.
- Bảng 2.3: outline không có các dòng PostgreSQL, Xác thực, SignalR, Công cụ .NET, Biểu mẫu và trạng thái, Kiểm thử backend, Container, Trợ lý AI; đã bổ sung từ csproj/package.json/Dockerfile/appsettings.json. Không có `global.json` trong dự án; phiên bản SDK .NET chỉ suy ra từ `TargetFramework` net10.0 và ARG `DOTNET_VERSION=10.0` của Dockerfile.
- Tên file quy định: prompt gọi `quy-dinh-trinh-bay.md`; file thực tế là `quy-dinh-format-bao-cao.md`. Caption: bảng đặt trên, hình đặt dưới.
-->

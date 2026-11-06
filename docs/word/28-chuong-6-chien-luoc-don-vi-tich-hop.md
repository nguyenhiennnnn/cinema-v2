# CHƯƠNG 6. KIỂM THỬ HỆ THỐNG

## 6.1 CHIẾN LƯỢC KIỂM THỬ

Hệ thống Perfo được kiểm thử theo bốn tầng tăng dần về phạm vi và chi phí chạy, tương ứng với mô hình kim tự tháp kiểm thử (Hình 6.1). Tầng đáy là kiểm thử đơn vị ở backend, viết bằng xUnit với Moq để giả lập các phụ thuộc, Shouldly để viết điều kiện kiểm tra và cơ sở dữ liệu SQLite trong bộ nhớ thay cho PostgreSQL. Tầng thứ hai là kiểm thử tích hợp, chạy toàn bộ ứng dụng ASP.NET Core qua `WebApplicationFactory`, dùng PostgreSQL thật trong container (Testcontainers), dọn dữ liệu giữa các test bằng Respawn, kết nối SignalR bằng client thật và thay các dịch vụ bên ngoài (email, đăng nhập Google, cổng thanh toán) bằng bản giả. Tầng thứ ba là kiểm thử đầu cuối (E2E) bằng Playwright trên trình duyệt Chromium, trình bày ở Mục 6.4. Tầng trên cùng là kiểm thử tải bằng k6, trình bày ở Mục 6.6. Càng lên cao, số lượng test càng ít nhưng mỗi test bao quát nhiều thành phần hơn: số liệu đếm được từ mã nguồn là 364 test đơn vị, 49 test tích hợp, 8 test E2E và 2 kịch bản tải (Bảng 6.1).

![Hình 6.1. Kim tự tháp kiểm thử của dự án](hinh/6.1.png)

Hình 6.1. Kim tự tháp kiểm thử của dự án

Hình 6.1 gồm bốn tầng xếp chồng, từ đáy lên đỉnh: kiểm thử đơn vị (xUnit, Moq, Shouldly, SQLite), kiểm thử tích hợp (`WebApplicationFactory`, Testcontainers PostgreSQL, Respawn, SignalR client, bản giả của email, Google và cổng thanh toán), kiểm thử E2E (Playwright, Chromium) và kiểm thử tải (k6). Độ rộng mỗi tầng tỷ lệ với số test ở Bảng 6.1.

Các bộ test dùng chung một môi trường ứng dụng tên `Testing`. Trong `Program.cs`, mọi đoạn khởi tạo gắn với tác vụ nền và giới hạn tần suất đều nằm trong điều kiện `!IsEnvironment("Testing")`: đăng ký `AddHangfire` và `AddHangfireServer`, middleware `UseRateLimiter`, bảng điều khiển Hangfire (`MapHangfireDashboard`) và việc đăng ký các công việc định kỳ `ReleaseExpiredSeatsJob`, `ShowtimeLifecycleJob`, `ExpireLoyaltyPointsJob`. Nhờ vậy test không bị tác vụ nền chạy ngầm làm thay đổi dữ liệu và không bị chặn bởi giới hạn tần suất. `CinemaApiFactory` đặt môi trường này bằng `builder.UseEnvironment("Testing")`, đồng thời nâng các ngưỡng `RateLimit:GlobalPermitLimit`, `RateLimit:AuthPermitLimit`, `RateLimit:BookingHoldPermitLimit` lên 100000 và đăng ký Hangfire với bộ lưu trữ trong bộ nhớ (Mục 6.3.1) để các dịch vụ cần `IBackgroundJobClient` vẫn được cấp phát.

Các lệnh chạy được khai báo trong `package.json` ở thư mục gốc: `test:unit` chạy `dotnet test backend/tests/Cinema.UnitTests`; `test:integration` chạy `dotnet test backend/tests/Cinema.IntegrationTests` và yêu cầu Docker đang chạy do dùng Testcontainers; `test:backend` chạy lần lượt hai lệnh trên; `test:backend:coverage` chạy `dotnet test backend/Cinema.slnx --collect:"XPlat Code Coverage"` để thu độ bao phủ bằng `coverlet.collector`; `test:e2e` chạy `playwright test` ở thư mục `frontend`; `test:load` chạy hai kịch bản k6 `test:load:browse` và `test:load:peak`; `test` gộp `test:backend` và `test:e2e`. Cấu hình Playwright (`frontend/playwright.config.ts`) đặt thư mục kịch bản `./e2e`, thời gian chờ mỗi test 60 giây, một worker, không chạy song song, một dự án `chromium` (thiết bị `Desktop Chrome`) và địa chỉ gốc lấy từ biến `E2E_BASE_URL`, mặc định `http://localhost:5173`.

Bảng 6.1. Tổng hợp các bộ kiểm thử

| Bộ kiểm thử | Số file test | Số test (đếm `[Fact]`/`[Theory]`) | Công cụ | Kết quả chạy |
|---|---|---|---|---|
| Đơn vị | 35 | 364 (399 ca chạy) | xUnit 2.9.3, Moq 4.20.72, Shouldly 4.3.0, EF Core SQLite | Đạt 399/399 test (100% pass, thời gian 26 giây) |
| Tích hợp | 8 | 49 | `WebApplicationFactory`, Testcontainers PostgreSQL 4.4.0, Respawn 7.0.0, SignalR client | Đạt 49/49 test (100% pass, thời gian 51 giây) |
| E2E | 7 spec (8 kịch bản) | 8 | Playwright ^1.63.0, Chromium | Đạt 8/8 kịch bản (100% pass, thời gian 100,09 giây) |
| Tải | 2 kịch bản (`browse-movies-stress.js`, `peak-ticket-rush.js`) | — | k6 (ảnh Docker `grafana/k6`) | Kịch bản hoàn chỉnh, cấu hình ngưỡng p95 < 2,5 s và tỷ lệ lỗi < 1% |

Số test ở Bảng 6.1 là số thuộc tính `[Fact]` và `[Theory]` đếm từ mã nguồn; một `[Theory]` được tính là một test dù có nhiều bộ dữ liệu `[InlineData]`, nên số test mà `dotnet test` báo cáo có thể lớn hơn. Cột "Số file test" của bộ đơn vị chỉ tính các file `*Tests.cs`, không tính sáu file hỗ trợ trong thư mục `TestSupport`.

## 6.2 KIỂM THỬ ĐƠN VỊ

Các test đơn vị nằm trong dự án `backend/tests/Cinema.UnitTests` (mục tiêu `net10.0`), tham chiếu ba dự án `Cinema.Domain`, `Cinema.Application` và `Cinema.Infrastructure`. Tên test theo mẫu `Phương_thức_Điều_kiện_Kết_quả`, ví dụ `LoginAsync_WrongPassword_ThrowsUnauthorized`. Các lớp dịch vụ được kiểm thử với `AppDbContext` thật trên SQLite trong bộ nhớ do `SqliteAppDbContextHandle` tạo (kết nối `Data Source=:memory:`, gọi `EnsureCreated`). Lớp `SqliteTestAppDbContext` kế thừa `AppDbContext` và gỡ khỏi mô hình các thành phần chỉ PostgreSQL hỗ trợ: giá trị mặc định chứa `AT TIME ZONE` hoặc `NOW()`, kiểu cột `timestamp` và `decimal`, cùng các ràng buộc CHECK. Những ràng buộc này vì vậy chỉ được kiểm tra ở tầng tích hợp. Các phụ thuộc ngoài cơ sở dữ liệu được giả lập bằng Moq, chẳng hạn `IPaymentGateway`, `IRealtimeNotifier`, `IBackgroundJobClient`, `ITokenService`, `IGoogleAuthService` và `IPasswordHasher<User>`. Dữ liệu dùng chung được dựng bởi `TestDataSeeder` và các lớp nền `AuthServiceTestBase`, `BookingServiceTestBase`, `ShowtimeServiceTestBase`.

Mỗi mục dưới đây trình bày một nhóm chức năng bằng một bảng; mỗi dòng là một test, cột "Loại" cho biết thuộc tính `[Fact]` hay `[Theory]`. Nhóm test được xếp theo chức năng nghiệp vụ chứ không theo thư mục. Với những test trùng tên trong cùng một file (thường là test của các validator), tên được ghi kèm lớp chứa test.

### 6.2.1 Nhóm Xác thực

Nhóm gồm 52 test: `AuthServiceTests` 28 test; `JwtTokenServiceTests` 9 test; `AuthValidatorsTests` 15 test (trong đó 3 `[Theory]`). Các test của `AuthService` bao quát đăng ký và xác nhận email, đăng nhập qua lối khách hàng và cổng quản trị (từ chối tài khoản bị khóa, chưa xác nhận email hoặc sai vai trò), xoay vòng và thu hồi refresh token (kể cả thu hồi cả chuỗi khi dùng lại token đã xoay), đổi, quên và đặt lại mật khẩu. Các test của `JwtTokenService` kiểm tra claim của access token, thời hạn cố định 5 phút của token kết nối thời gian thực và tính chất của token ngẫu nhiên cùng giá trị băm. Các test validator kiểm tra quy tắc đầu vào của bốn yêu cầu đăng ký, đăng nhập, đặt lại và đổi mật khẩu.

Bảng 6.2. Test đơn vị nhóm Xác thực

| File | Tên test | Loại | Hành vi kiểm tra và kết quả mong đợi |
|---|---|---|---|
| AuthServiceTests | `RegisterAsync_NewEmail_CreatesUserAndCustomerProfileAndQueuesVerificationEmail` | Fact | Đăng ký với email mới tạo `User` và `CustomerProfile`, đồng thời đưa email xác thực vào hàng đợi tác vụ nền. |
| AuthServiceTests | `RegisterAsync_EmailAlreadyExists_ThrowsValidationAppException` | Fact | Email đã tồn tại thì dịch vụ ném `ValidationAppException`. |
| AuthServiceTests | `ConfirmEmailAsync_ValidToken_ConfirmsEmailAndMarksTokenUsed` | Fact | Token hợp lệ xác nhận email và đánh dấu token đã sử dụng. |
| AuthServiceTests | `ConfirmEmailAsync_ExpiredToken_ThrowsValidationAppException` | Fact | Token xác nhận hết hạn bị từ chối bằng `ValidationAppException`. |
| AuthServiceTests | `ConfirmEmailAsync_AlreadyUsedToken_ThrowsValidationAppException` | Fact | Token đã dùng một lần không được dùng lại; dịch vụ ném `ValidationAppException`. |
| AuthServiceTests | `LoginAsync_ValidCredentials_ReturnsTokensAndUserDto` | Fact | Thông tin đăng nhập hợp lệ trả về cặp token và DTO người dùng. |
| AuthServiceTests | `LoginAsync_UnknownEmail_ThrowsUnauthorized` | Fact | Email không tồn tại bị từ chối với lỗi Unauthorized. |
| AuthServiceTests | `LoginAsync_WrongPassword_ThrowsUnauthorized` | Fact | Sai mật khẩu bị từ chối với lỗi Unauthorized. |
| AuthServiceTests | `LoginAsync_InactiveAccount_ThrowsForbidden` | Fact | Tài khoản bị khóa bị từ chối với lỗi Forbidden. |
| AuthServiceTests | `LoginAsync_UnconfirmedEmail_ThrowsForbidden` | Fact | Tài khoản chưa xác nhận email bị từ chối với lỗi Forbidden. |
| AuthServiceTests | `LoginAsync_AdminUsingCustomerLoginEndpoint_ThrowsForbidden` | Fact | Tài khoản quản trị đăng nhập qua lối đăng nhập của khách hàng bị từ chối (Forbidden). |
| AuthServiceTests | `LoginAsync_AdminPortalWithCustomerAccount_ThrowsForbidden` | Fact | Tài khoản khách hàng đăng nhập vào cổng quản trị bị từ chối (Forbidden). |
| AuthServiceTests | `LoginAsync_AdminPortalWithUnknownRole_ThrowsForbidden` | Fact | Vai trò không xác định đăng nhập vào cổng quản trị bị từ chối (Forbidden). |
| AuthServiceTests | `LoginAsync_AdminPortalWithAdmin_Succeeds` | Fact | Tài khoản quản trị đăng nhập vào cổng quản trị thành công. |
| AuthServiceTests | `LoginAsync_AdminPortalWithDeactivatedEmployeeProfile_ThrowsForbidden` | Fact | Hồ sơ nhân viên đã vô hiệu hóa bị từ chối khi vào cổng quản trị (Forbidden). |
| AuthServiceTests | `LoginAsync_RehashNeeded_TransparentlyUpgradesStoredHash` | Fact | Khi bộ băm báo cần băm lại, mật khẩu được nâng cấp hash lưu trữ mà đăng nhập vẫn thành công. |
| AuthServiceTests | `RefreshAsync_ValidToken_IssuesNewPairAndRevokesOldOne` | Fact | Refresh token hợp lệ cấp cặp token mới và thu hồi token cũ. |
| AuthServiceTests | `RefreshAsync_UnknownToken_ThrowsUnauthorized` | Fact | Refresh token không tồn tại bị từ chối với lỗi Unauthorized. |
| AuthServiceTests | `RefreshAsync_RevokedToken_ThrowsUnauthorized` | Fact | Refresh token đã bị thu hồi bị từ chối với lỗi Unauthorized. |
| AuthServiceTests | `RefreshAsync_ReusingRotatedToken_RevokesWholeTokenChain` | Fact | Dùng lại token đã được xoay vòng thì cả chuỗi token liên quan bị thu hồi. |
| AuthServiceTests | `RefreshAsync_ReusingLoggedOutToken_DoesNotRevokeOtherSessions` | Fact | Dùng lại token đã đăng xuất không làm thu hồi các phiên khác. |
| AuthServiceTests | `RefreshAsync_ExpiredToken_ThrowsUnauthorized` | Fact | Refresh token hết hạn bị từ chối với lỗi Unauthorized. |
| AuthServiceTests | `LogoutAsync_ActiveToken_RevokesIt` | Fact | Đăng xuất với token còn hiệu lực thì token bị thu hồi. |
| AuthServiceTests | `LogoutAsync_UnknownToken_StillReturnsSuccessMessage` | Fact | Token không tồn tại vẫn trả thông báo thành công, không làm lộ trạng thái token. |
| AuthServiceTests | `ChangePasswordAsync_WrongCurrentPassword_ThrowsValidationAppException` | Fact | Sai mật khẩu hiện tại thì đổi mật khẩu bị từ chối bằng `ValidationAppException`. |
| AuthServiceTests | `ChangePasswordAsync_CorrectCurrentPassword_UpdatesHash` | Fact | Mật khẩu hiện tại đúng thì hash mật khẩu được cập nhật. |
| AuthServiceTests | `ForgotPasswordAsync_UnknownEmail_ReturnsStandardMessageWithoutQueueingEmail` | Fact | Email không tồn tại vẫn nhận thông báo chuẩn và không có email nào được đưa vào hàng đợi. |
| AuthServiceTests | `ResetPasswordAsync_ValidToken_UpdatesPasswordAndRevokesAllActiveRefreshTokens` | Fact | Đặt lại mật khẩu bằng token hợp lệ cập nhật mật khẩu và thu hồi mọi refresh token đang hiệu lực. |
| JwtTokenServiceTests | `GenerateAccessToken_ProducesTokenWithExpectedClaims` | Fact | Access token sinh ra chứa các claim kỳ vọng. |
| JwtTokenServiceTests | `GenerateAccessToken_ForEmployee_IncludesEmployeeIdClaim` | Fact | Với nhân viên, access token có thêm claim mã nhân viên. |
| JwtTokenServiceTests | `GenerateAccessToken_ForCustomerWithoutEmployee_OmitsEmployeeIdClaim` | Fact | Với khách hàng không phải nhân viên, access token không có claim mã nhân viên. |
| JwtTokenServiceTests | `GenerateSocketToken_UsesFixed5MinuteExpiration_RegardlessOfAccessTokenOptions` | Fact | Token dùng cho kết nối thời gian thực có thời hạn cố định 5 phút, không phụ thuộc cấu hình access token. |
| JwtTokenServiceTests | `GenerateOpaqueToken_ProducesUrlSafeNonEmptyToken` | Fact | Token ngẫu nhiên không rỗng và an toàn khi đặt trong URL. |
| JwtTokenServiceTests | `GenerateOpaqueToken_ProducesDifferentTokensEachCall` | Fact | Mỗi lần gọi sinh một token khác nhau. |
| JwtTokenServiceTests | `HashOpaqueToken_IsDeterministic` | Fact | Băm cùng một token luôn cho cùng một kết quả. |
| JwtTokenServiceTests | `HashOpaqueToken_DifferentInputsProduceDifferentHashes` | Fact | Hai token khác nhau cho hai giá trị băm khác nhau. |
| JwtTokenServiceTests | `HashOpaqueToken_IsNotReversibleToOriginalPlainToken` | Fact | Giá trị băm không trùng với token gốc ở dạng rõ. |
| AuthValidatorsTests | `RegisterRequestValidatorTests.ValidRequest_PassesValidation` | Fact | Yêu cầu đăng ký hợp lệ vượt qua kiểm tra. |
| AuthValidatorsTests | `EmptyFullName_FailsValidation` | Fact | Họ tên rỗng không hợp lệ. |
| AuthValidatorsTests | `InvalidEmail_FailsValidation` | Theory | Các email sai định dạng không hợp lệ. |
| AuthValidatorsTests | `WeakPassword_FailsValidation` | Theory | Các mật khẩu yếu không hợp lệ (áp dụng cho đăng ký). |
| AuthValidatorsTests | `PhoneProvided_MustMatchVietnamesePhoneFormat` | Fact | Số điện thoại có nhập phải đúng định dạng số điện thoại Việt Nam. |
| AuthValidatorsTests | `PhoneProvided_ValidVietnamesePhone_Passes` | Theory | Các số điện thoại Việt Nam hợp lệ vượt qua kiểm tra. |
| AuthValidatorsTests | `PhoneNotProvided_IsOptionalAndPasses` | Fact | Không nhập số điện thoại vẫn hợp lệ vì trường này tùy chọn. |
| AuthValidatorsTests | `LoginRequestValidatorTests.ValidRequest_PassesValidation` | Fact | Yêu cầu đăng nhập hợp lệ vượt qua kiểm tra. |
| AuthValidatorsTests | `EmptyPassword_FailsValidation_ButDoesNotCheckStrength` | Fact | Mật khẩu rỗng không hợp lệ nhưng validator đăng nhập không kiểm tra độ mạnh. |
| AuthValidatorsTests | `ShortWeakPassword_StillPassesValidation` | Fact | Mật khẩu ngắn, yếu vẫn qua validator đăng nhập. |
| AuthValidatorsTests | `EmptyToken_FailsValidation` | Fact | Token rỗng không hợp lệ. |
| AuthValidatorsTests | `ResetPasswordRequestValidatorTests.WeakNewPassword_FailsValidation` | Fact | Mật khẩu mới yếu không hợp lệ. |
| AuthValidatorsTests | `ResetPasswordRequestValidatorTests.ValidRequest_PassesValidation` | Fact | Yêu cầu đặt lại mật khẩu hợp lệ vượt qua kiểm tra. |
| AuthValidatorsTests | `EmptyCurrentPassword_FailsValidation` | Fact | Mật khẩu hiện tại rỗng không hợp lệ. |
| AuthValidatorsTests | `ChangePasswordRequestValidatorTests.WeakNewPassword_FailsValidation` | Fact | Mật khẩu mới yếu không hợp lệ. |

### 6.2.2 Nhóm Đặt vé

Nhóm gồm 88 test: `BookingServiceCounterCheckinTests` 18 test; `BookingServiceLoyaltyTests` 4 test; `LoyaltyWindowTests` 19 test; `BookingServiceSeatHoldTests` 6 test (trong đó 1 `[Theory]`); `BookingServiceTests` 21 test; `BookingValidatorsTests` 20 test (trong đó 2 `[Theory]`). Nhóm này bao quát giữ ghế trực tuyến và tại quầy, thanh toán tiền mặt tại quầy, soát vé, thanh toán qua cổng, xác nhận booking và sinh vé, hủy và hoàn tiền, xử lý thanh toán thất bại, cùng các quy tắc điểm thành viên. Ba file `BookingServiceTests`, `BookingServiceLoyaltyTests` và `BookingServiceCounterCheckinTests` kiểm tra các quy tắc điểm thành viên: giảm giá chỉ tính trên tiền vé, điểm hiệu lực lấy từ sổ điểm trong cửa sổ trượt 12 tháng, thu hồi điểm vào đúng dòng sổ điểm của booking và tính lại hạng sau thu hồi. File `LoyaltyWindowTests.cs` gồm ba lớp: `LoyaltyDiscountTests` (8 test về giảm giá chỉ tính trên tiền vé và hạng xét theo điểm trong cửa sổ trượt 12 tháng), `LoyaltyLedgerTests` (6 test về ghi sổ điểm khi xác nhận booking và thu hồi theo từng booking) và `LoyaltyExpiryServiceTests` (5 test về việc `RecalculateAllAsync` loại điểm hết hạn, hạ hạng và không đổi gì ở lần chạy thứ hai). Lớp hỗ trợ `TestSupport/LoyaltyTestHelper.cs` thay các hạng seed bằng ba hạng cố định (Bronze 0 điểm/0%, Silver 100 điểm/5%, Gold 200 điểm/10%), tạo booking `Confirmed` tối thiểu và thêm dòng sổ điểm với thời điểm `EarnedAt` tùy ý, giúp các test dựng được điểm nằm trong hoặc ngoài cửa sổ 12 tháng.

Bảng 6.3. Test đơn vị nhóm Đặt vé

| File | Tên test | Loại | Hành vi kiểm tra và kết quả mong đợi |
|---|---|---|---|
| BookingServiceCounterCheckinTests | `CounterHoldSeatsAsync_ValidRequest_CreatesCounterChannelBookingAttributedToEmployee` | Fact | Giữ ghế tại quầy hợp lệ tạo booking kênh quầy (`Counter`), gắn với nhân viên thực hiện. |
| BookingServiceCounterCheckinTests | `CounterCheckoutAsync_AmountReceivedCoversTotal_ConfirmsBookingAndReturnsChange` | Fact | Tiền khách đưa đủ tổng tiền thì booking được xác nhận và trả về tiền thối. |
| BookingServiceCounterCheckinTests | `CounterCheckoutAsync_AmountReceivedLessThanTotal_ThrowsBusinessRuleExceptionAndDoesNotCreatePayment` | Fact | Tiền khách đưa thiếu thì ném `BusinessRuleException` và không tạo thanh toán. |
| BookingServiceCounterCheckinTests | `CounterCheckoutAsync_BookingAlreadyExpired_ThrowsBusinessRuleException` | Fact | Booking đã hết hạn giữ ghế không thể thanh toán tại quầy; ném `BusinessRuleException`. |
| BookingServiceCounterCheckinTests | `CounterCheckoutAsync_UnknownBookingId_ThrowsNotFound` | Fact | Mã booking không tồn tại thì báo NotFound. |
| BookingServiceCounterCheckinTests | `CheckinAsync_ValidQrWithinWindow_MarksTicketCheckedIn` | Fact | Mã QR hợp lệ trong khung giờ cho phép thì vé được đánh dấu đã soát. |
| BookingServiceCounterCheckinTests | `CheckinAsync_UnknownQrCode_ThrowsNotFound` | Fact | Mã QR không tồn tại thì báo NotFound. |
| BookingServiceCounterCheckinTests | `CheckinAsync_TicketAlreadyCheckedIn_ThrowsConflict` | Fact | Vé đã soát trước đó thì báo Conflict, chống soát lặp. |
| BookingServiceCounterCheckinTests | `CheckinAsync_MoreThan30MinutesBeforeShowtime_ThrowsBusinessRuleException` | Fact | Soát vé sớm hơn 30 phút trước giờ chiếu bị từ chối bằng `BusinessRuleException`. |
| BookingServiceCounterCheckinTests | `CheckinAsync_ShowtimeAlreadyEnded_ThrowsBusinessRuleException` | Fact | Suất chiếu đã kết thúc thì không soát vé; ném `BusinessRuleException`. |
| BookingServiceCounterCheckinTests | `ReverseLoyaltyAsync_CustomerHasEnoughPointsAndSpend_DeductsExactAmount` | Fact | Khách đủ điểm và tổng chi tiêu thì thu hồi trừ đúng số điểm và số tiền. |
| BookingServiceCounterCheckinTests | `ReverseLoyaltyAsync_ReversalWouldGoNegative_ClampsAtZeroInsteadOfGoingNegative` | Fact | Khi thu hồi làm giá trị âm, kết quả được chặn ở 0. |
| BookingServiceCounterCheckinTests | `ReverseLoyaltyAsync_UserHasNoCustomerProfile_DoesNothingAndDoesNotThrow` | Fact | Người dùng không có `CustomerProfile` thì không làm gì và không ném lỗi. |
| BookingServiceCounterCheckinTests | `CounterHoldSeatsAsync_WithUserId_UserNotFound_ThrowsNotFoundException` | Fact | Giữ ghế quầy gắn `UserId` không tồn tại thì ném `NotFoundException`. |
| BookingServiceCounterCheckinTests | `CounterHoldSeatsAsync_WithUserId_UserInactive_ThrowsBusinessRuleException` | Fact | Người dùng bị khóa không thể gắn vào booking quầy; ném `BusinessRuleException`. |
| BookingServiceCounterCheckinTests | `CounterHoldSeatsAsync_WithUserId_UserNotCustomerRole_ThrowsBusinessRuleException` | Fact | Người dùng không thuộc vai trò khách hàng bị từ chối bằng `BusinessRuleException`. |
| BookingServiceCounterCheckinTests | `CounterHoldSeatsAsync_WithUserId_ValidCustomer_UsesUserNameAndPhone` | Fact | Có `UserId` khách hợp lệ thì booking dùng họ tên và số điện thoại của tài khoản. |
| BookingServiceCounterCheckinTests | `CounterHoldSeatsAsync_WithoutUserId_UsesRequestNameAndPhone` | Fact | Không có `UserId` thì booking dùng họ tên và số điện thoại trong yêu cầu. |
| BookingServiceLoyaltyTests | `ReverseLoyaltyAsync_PointsDropBelowTierThreshold_DowngradesTier` | Fact | Sau thu hồi, điểm hiệu lực là 90 (dưới ngưỡng hạng) nên hạng bị hạ về Bronze. |
| BookingServiceLoyaltyTests | `ReverseLoyaltyAsync_PointsStillMeetTier_KeepsTier` | Fact | Sau thu hồi, điểm hiệu lực là 140 vẫn đạt ngưỡng nên giữ hạng Silver. |
| BookingServiceLoyaltyTests | `CancelBookingAsync_PartialRefund_RevokesLoyaltyProportionallyToRefund` | Fact | Hoàn tiền một phần thì điểm bị thu hồi tỷ lệ theo số tiền hoàn; tổng chi tiêu giảm đúng số tiền hoàn. |
| BookingServiceLoyaltyTests | `ConfirmBookingAndGenerateTicketsAsync_UserBooking_EmailContainsMovieTitle` | Fact | Email xác nhận gửi cho booking của tài khoản có chứa tên phim. |
| LoyaltyWindowTests | `HoldSeatsAsync_StaleTierInProfileCache_IsIgnoredWhenLedgerHasNoPoints` | Fact | Hạng lưu cũ trong hồ sơ bị bỏ qua khi sổ điểm không có điểm: không giảm giá, `FinalAmount` là 100.000. |
| LoyaltyWindowTests | `HoldSeatsAsync_LedgerPointsInWindow_AppliesTierEvenWhenCacheIsEmpty` | Fact | Sổ điểm có 100 điểm trong cửa sổ 12 tháng thì áp hạng Silver dù hồ sơ chưa có điểm: giảm 5.000. |
| LoyaltyWindowTests | `HoldSeatsAsync_PointsOlderThan12Months_DoNotCount` | Fact | Điểm kiếm cách đây quá 12 tháng không được tính: không giảm giá. |
| LoyaltyWindowTests | `HoldSeatsAsync_PointsJustInsideWindow_StillCount` | Fact | Điểm nằm sát biên trong cửa sổ 12 tháng vẫn được tính: hạng Gold, giảm 10.000. |
| LoyaltyWindowTests | `HoldSeatsAsync_RevokedPointsReduceTier` | Fact | Điểm đã thu hồi được trừ khi xét hạng; kết quả không còn giảm giá. |
| LoyaltyWindowTests | `HoldSeatsAsync_DiscountAppliesToTicketsOnly_NotCombo` | Fact | Giảm giá chỉ tính trên tiền vé: tổng 200.000 (vé 100.000 + 2 combo × 50.000), giảm 10.000, thành tiền 190.000. |
| LoyaltyWindowTests | `HoldSeatsAsync_ComboOnlyDiscountBase_RoundsAwayFromZeroOnTicketAmount` | Fact | Số tiền giảm 4.500,5 được làm tròn ra xa số 0 thành 4.501. |
| LoyaltyWindowTests | `HoldSeatsAsync_CustomerWithoutLedgerAndTierZeroPercent_NoDiscount` | Fact | Khách chưa có sổ điểm và hạng 0% thì không được giảm giá. |
| LoyaltyWindowTests | `ConfirmBookingAsync_CreatesLedgerEntryForBooking` | Fact | Xác nhận booking tạo một dòng sổ điểm gắn `BookingId`, 10 điểm, `RevokedPoints` bằng 0, `EarnedAt` là thời điểm hiện tại; điểm hồ sơ là 10. |
| LoyaltyWindowTests | `ConfirmBookingAsync_StaleExpiredPointsInCache_AreDroppedAndTierDowngraded` | Fact | Điểm cũ đã hết hạn còn lưu trong hồ sơ bị loại khi xác nhận; điểm còn 10 và hạng về Bronze. |
| LoyaltyWindowTests | `ReverseLoyaltyAsync_EntryAlreadyExpired_DoesNotReduceCurrentWindowPoints` | Fact | Thu hồi dòng sổ điểm đã hết hạn không làm giảm điểm của cửa sổ hiện tại (30 điểm giữ nguyên); dòng cũ ghi `RevokedPoints` 50. |
| LoyaltyWindowTests | `ReverseLoyaltyAsync_PartialRefund_OnlyTouchesOwnEntry` | Fact | Hoàn một phần chỉ tác động lên dòng sổ điểm của chính booking (`RevokedPoints` 20), dòng khác giữ 0; điểm hiệu lực còn 60. |
| LoyaltyWindowTests | `ReverseLoyaltyAsync_RevokeCappedAtEntryPoints` | Fact | Số điểm thu hồi bị chặn tối đa bằng số điểm của dòng (`RevokedPoints` 5); điểm hiệu lực còn 30. |
| LoyaltyWindowTests | `ReverseLoyaltyAsync_BookingWithoutEntry_StillReducesTotalSpentAndDoesNotThrow` | Fact | Booking không có dòng sổ điểm vẫn giảm tổng chi tiêu và không ném lỗi (`TotalSpent` còn 200.000). |
| LoyaltyWindowTests | `RecalculateAllAsync_ExpiredPointsRemoved_AndTierDowngraded` | Fact | Tính lại toàn bộ loại điểm quá hạn (13 tháng): điểm về 0, hạng về Bronze, trả về 1 hồ sơ thay đổi. |
| LoyaltyWindowTests | `RecalculateAllAsync_MixedEntries_KeepsOnlyFreshPoints` | Fact | Có dòng 13 tháng trước và dòng 1 tháng trước thì chỉ giữ 150 điểm còn hạn, hạng Silver. |
| LoyaltyWindowTests | `RecalculateAllAsync_SecondRunIsNoOp` | Fact | Lần chạy thứ hai không đổi gì: lần 1 trả về 1, lần 2 trả về 0. |
| LoyaltyWindowTests | `RecalculateAllAsync_RevokedPointsExcluded` | Fact | Điểm đã thu hồi bị loại khi tính lại; điểm hiệu lực là 50. |
| LoyaltyWindowTests | `RecalculateAllAsync_ProfileWithCachedPointsButNoLedger_IsZeroed` | Fact | Hồ sơ có điểm lưu nhưng không có dòng sổ điểm thì được đưa về 0. |
| BookingServiceSeatHoldTests | `CounterHoldSeatsAsync_HoldLongerThanTimeToShowtime_ExpiresAtMostAtShowtimeStart` | Fact | Thời gian giữ ghế tại quầy không vượt quá giờ bắt đầu suất chiếu. |
| BookingServiceSeatHoldTests | `CounterHoldSeatsAsync_BindsHeldSeatsToCreatedBooking` | Fact | Ghế đang giữ được gắn với booking vừa tạo. |
| BookingServiceSeatHoldTests | `CounterHoldSeatsAsync_SeatExpiredButHeldBySameHolder_CanBeHeldAgain` | Fact | Ghế hết hạn giữ nhưng cùng người giữ thì được giữ lại. |
| BookingServiceSeatHoldTests | `CounterHoldSeatsAsync_SeatExpiredButHeldByAnotherUser_ThrowsSeatConflict` | Fact | Ghế hết hạn nhưng còn thuộc người giữ khác thì báo xung đột ghế (`SeatConflict`). |
| BookingServiceSeatHoldTests | `CounterCheckoutAsync_CounterBookingLinkedToMember_ConfirmsEmployeeHeldSeats` | Theory | Booking quầy gắn với hội viên xác nhận được ghế do nhân viên giữ, với hai trường hợp ghế đã hoặc chưa gắn vào booking (tham số `seatsBoundToBooking`). |
| BookingServiceSeatHoldTests | `CounterCheckoutAsync_OnlineBooking_ThrowsBusinessRuleExceptionAndCreatesNoPayment` | Fact | Booking kênh trực tuyến không thanh toán qua quầy; ném `BusinessRuleException`, không tạo thanh toán. |
| BookingServiceTests | `HoldSeatsAsync_AvailableSeats_CreatesPendingBookingAndMarksSeatsHeld` | Fact | Ghế còn trống tạo booking `Pending` và đánh dấu ghế đang giữ. |
| BookingServiceTests | `HoldSeatsAsync_OneSeatAlreadyHeldByAnotherUser_ThrowsSeatConflictAndRollsBackOtherHolds` | Fact | Một ghế đã bị người khác giữ thì báo xung đột và hoàn tác các ghế đã giữ trong cùng yêu cầu. |
| BookingServiceTests | `HoldSeatsAsync_ShowtimeDoesNotExist_ThrowsNotFound` | Fact | Suất chiếu không tồn tại thì báo NotFound. |
| BookingServiceTests | `HoldSeatsAsync_ShowtimeNotScheduled_ThrowsBusinessRuleException` | Fact | Suất chiếu không ở trạng thái đã lên lịch thì ném `BusinessRuleException`. |
| BookingServiceTests | `HoldSeatsAsync_CustomerAlreadyHasActivePendingBookingForShowtime_ThrowsConflict` | Fact | Khách đã có booking `Pending` còn hiệu lực cho suất chiếu thì báo Conflict. |
| BookingServiceTests | `HoldSeatsAsync_CustomerWithDiscountTier_AppliesDiscountToFinalAmount` | Fact | Khách có hạng giảm giá thì `FinalAmount` được áp mức giảm. |
| BookingServiceTests | `CheckoutAsync_PendingBookingWithConfiguredGateway_CreatesPendingPaymentAndReturnsPaymentUrl` | Fact | Booking `Pending` với cổng đã cấu hình tạo thanh toán `Pending` và trả URL thanh toán. |
| BookingServiceTests | `CheckoutAsync_UnconfiguredGateway_ThrowsBusinessRuleException` | Fact | Cổng thanh toán chưa cấu hình thì ném `BusinessRuleException`. |
| BookingServiceTests | `CheckoutAsync_NotBookingOwner_ThrowsForbidden` | Fact | Người không phải chủ booking bị từ chối (Forbidden). |
| BookingServiceTests | `CheckoutAsync_BookingAlreadyExpired_ThrowsBusinessRuleException` | Fact | Booking đã hết hạn giữ ghế không thể thanh toán; ném `BusinessRuleException`. |
| BookingServiceTests | `CheckoutAsync_BookingNotPending_ThrowsBusinessRuleException` | Fact | Booking không ở trạng thái `Pending` không thể thanh toán; ném `BusinessRuleException`. |
| BookingServiceTests | `ConfirmBookingAndGenerateTicketsAsync_HeldSeatsStillValid_BooksSeatsAndGeneratesTicketsAndLoyaltyPoints` | Fact | Ghế còn hợp lệ thì chuyển ghế sang đã đặt, sinh vé và cộng điểm thành viên. |
| BookingServiceTests | `ConfirmBookingAndGenerateTicketsAsync_AlreadyConfirmed_IsIdempotentAndDoesNotDuplicateTickets` | Fact | Xác nhận lặp lại không tạo thêm vé (tính lũy đẳng). |
| BookingServiceTests | `ConfirmBookingAndGenerateTicketsAsync_SeatsNoLongerHeld_CancelsBookingAndThrows` | Fact | Ghế không còn được giữ thì hủy booking và ném lỗi. |
| BookingServiceTests | `CancelBookingAsync_PendingBooking_ReleasesSeatsAndCancelsWithoutRefund` | Fact | Hủy booking `Pending` giải phóng ghế và hủy mà không hoàn tiền. |
| BookingServiceTests | `CancelBookingAsync_ConfirmedBookingWithinRefundWindow_RefundsAndCancels` | Fact | Hủy booking đã xác nhận trong hạn hoàn tiền thì hoàn tiền và hủy. |
| BookingServiceTests | `CancelBookingAsync_ConfirmedBookingPastRefundDeadline_ThrowsBusinessRuleException` | Fact | Quá hạn hoàn tiền thì ném `BusinessRuleException`. |
| BookingServiceTests | `CancelBookingAsync_ConfirmedBookingZeroRefundPercent_ThrowsBusinessRuleException` | Fact | Tỷ lệ hoàn tiền cấu hình bằng 0% thì không cho hủy; ném `BusinessRuleException`. |
| BookingServiceTests | `ProcessPaymentFailureAsync_PendingBooking_ReleasesSeatsAndCancelsBooking` | Fact | Thanh toán thất bại với booking `Pending` giải phóng ghế và hủy booking. |
| BookingServiceTests | `ProcessPaymentFailureAsync_HoldNotExpired_KeepsBookingPendingForRetry` | Fact | Thời gian giữ ghế chưa hết thì booking vẫn `Pending` để khách thử lại. |
| BookingServiceTests | `ProcessPaymentFailureAsync_BookingAlreadyConfirmed_DoesNothing` | Fact | Booking đã xác nhận thì xử lý thất bại không làm gì. |
| BookingValidatorsTests | `ValidRequest_PassesValidation` | Fact | Yêu cầu giữ ghế hợp lệ vượt qua kiểm tra. |
| BookingValidatorsTests | `ShowtimeIdNotPositive_FailsValidation` | Fact | `ShowtimeId` không dương không hợp lệ. |
| BookingValidatorsTests | `EmptyCustomerName_FailsValidation` | Fact | Tên khách hàng rỗng không hợp lệ. |
| BookingValidatorsTests | `NoSeatsSelected_FailsValidation` | Fact | Không chọn ghế nào không hợp lệ. |
| BookingValidatorsTests | `SeatIdNotPositive_FailsValidation` | Fact | Mã ghế không dương không hợp lệ. |
| BookingValidatorsTests | `DuplicateSeatIds_FailsValidation` | Fact | Danh sách ghế trùng lặp không hợp lệ. |
| BookingValidatorsTests | `InvalidPhoneFormat_FailsValidation` | Fact | Số điện thoại sai định dạng không hợp lệ. |
| BookingValidatorsTests | `NullPhone_IsOptionalAndPasses` | Fact | Số điện thoại để trống vẫn hợp lệ. |
| BookingValidatorsTests | `ComboWithZeroQuantity_FailsValidation` | Fact | Combo có số lượng 0 không hợp lệ. |
| BookingValidatorsTests | `ValidRequestWithCustomerName_PassesValidation` | Fact | Giữ ghế quầy có tên khách hợp lệ vượt qua kiểm tra. |
| BookingValidatorsTests | `ValidRequestWithUserIdInsteadOfCustomerName_PassesValidation` | Fact | Giữ ghế quầy có `UserId` thay cho tên khách vẫn hợp lệ. |
| BookingValidatorsTests | `NeitherUserIdNorCustomerName_FailsValidation` | Fact | Thiếu cả `UserId` lẫn tên khách thì không hợp lệ. |
| BookingValidatorsTests | `UserIdNotPositive_FailsValidation` | Fact | `UserId` không dương không hợp lệ. |
| BookingValidatorsTests | `SupportedMethod_PassesValidation` | Theory | Các phương thức thanh toán được hỗ trợ vượt qua kiểm tra. |
| BookingValidatorsTests | `UnsupportedOrEmptyMethod_FailsValidation` | Theory | Phương thức không được hỗ trợ hoặc rỗng không hợp lệ. |
| BookingValidatorsTests | `ZeroAmountReceived_FailsValidation` | Fact | Số tiền nhận bằng 0 không hợp lệ. |
| BookingValidatorsTests | `NegativeAmountReceived_FailsValidation` | Fact | Số tiền nhận âm không hợp lệ. |
| BookingValidatorsTests | `PositiveAmountReceived_PassesValidation` | Fact | Số tiền nhận dương hợp lệ. |
| BookingValidatorsTests | `EmptyQrCode_FailsValidation` | Fact | Mã QR rỗng không hợp lệ. |
| BookingValidatorsTests | `NonEmptyQrCode_PassesValidation` | Fact | Mã QR không rỗng hợp lệ. |

### 6.2.3 Nhóm Thanh toán

Nhóm gồm 32 test: `PaymentServiceStripeTests` 16 test; `PaymentServiceVnPayTests` 8 test; `PaymentServiceMomoTests` 8 test. Mỗi file kiểm tra bộ xử lý callback của một cổng: `ProcessStripeWebhookAsync`, `ProcessVnPayIpnAsync` và `ProcessMomoIpnAsync`. Các tình huống chung gồm callback thành công, sai chữ ký, mã giao dịch không tồn tại, số tiền không khớp, cổng chưa cấu hình, callback thất bại và callback lặp (tính lũy đẳng). Riêng Stripe có thêm các trường hợp kiểm tra tiền tệ, sự kiện không liên quan, thứ tự sự kiện thất bại rồi thành công và email thông báo hoàn tiền khi ghế đã hết hạn.

Bảng 6.4. Test đơn vị nhóm Thanh toán

| File | Tên test | Loại | Hành vi kiểm tra và kết quả mong đợi |
|---|---|---|---|
| PaymentServiceStripeTests | `ProcessStripeWebhookAsync_ValidSuccessfulEvent_MarksPaymentSuccessAndConfirmsBooking` | Fact | Sự kiện thành công hợp lệ đánh dấu thanh toán thành công và xác nhận booking. |
| PaymentServiceStripeTests | `ProcessStripeWebhookAsync_InvalidSignature_ReturnsFailureAndDoesNotTouchPayment` | Fact | Chữ ký không hợp lệ trả kết quả thất bại, không động đến thanh toán. |
| PaymentServiceStripeTests | `ProcessStripeWebhookAsync_UnrelatedEventWithNoTransactionId_IsIgnoredSuccessfully` | Fact | Sự kiện không liên quan, không có mã giao dịch, được bỏ qua và trả thành công. |
| PaymentServiceStripeTests | `ProcessStripeWebhookAsync_UnknownTransactionId_ReturnsPaymentNotFound` | Fact | Mã giao dịch không tồn tại trả kết quả `PaymentNotFound`. |
| PaymentServiceStripeTests | `ProcessStripeWebhookAsync_GatewayNotConfigured_ReturnsFailureWithoutThrowing` | Fact | Cổng chưa cấu hình trả kết quả thất bại, không ném ngoại lệ. |
| PaymentServiceStripeTests | `ProcessStripeWebhookAsync_FailedPaymentEvent_MarksPaymentFailedAndReleasesBooking` | Fact | Sự kiện thanh toán thất bại đánh dấu thanh toán `Failed` và giải phóng booking. |
| PaymentServiceStripeTests | `ProcessStripeWebhookAsync_PaymentAlreadySuccessful_IsIdempotentAndDoesNotReconfirmBooking` | Fact | Thanh toán đã thành công thì webhook lặp không xác nhận lại booking (lũy đẳng). |
| PaymentServiceStripeTests | `ProcessStripeWebhookAsync_PaymentAlreadyFailed_IsIdempotentAndDoesNotReprocessFailure` | Fact | Thanh toán đã thất bại thì webhook lặp không xử lý lại. |
| PaymentServiceStripeTests | `ProcessStripeWebhookAsync_SuccessAfterPaymentMarkedFailed_StillConfirmsBooking` | Fact | Sự kiện thành công đến sau khi thanh toán bị đánh dấu thất bại vẫn xác nhận booking. |
| PaymentServiceStripeTests | `ProcessStripeWebhookAsync_DuplicateWebhookDeliveryForSameSuccessfulEvent_ConfirmsBookingOnlyOnce` | Fact | Cùng một sự kiện thành công được gửi hai lần chỉ xác nhận booking một lần. |
| PaymentServiceStripeTests | `ProcessStripeWebhookAsync_SeatsExpiredBeforeConfirmation_SwallowsExceptionAndStillReturnsSuccessToGateway` | Fact | Ghế hết hạn trước khi xác nhận: ngoại lệ được xử lý nội bộ và vẫn trả thành công cho cổng. |
| PaymentServiceStripeTests | `ProcessStripeWebhookAsync_AmountMismatch_ReturnsInvalidAmountAndDoesNotConfirm` | Fact | Số tiền không khớp trả `InvalidAmount` và không xác nhận booking. |
| PaymentServiceStripeTests | `ProcessStripeWebhookAsync_CurrencyMismatch_ReturnsInvalidCurrencyAndDoesNotConfirm` | Fact | Tiền tệ không khớp trả `InvalidCurrency` và không xác nhận booking. |
| PaymentServiceStripeTests | `ProcessStripeWebhookAsync_MatchingAmountAndCurrency_ConfirmsBooking` | Fact | Số tiền và tiền tệ khớp thì xác nhận booking. |
| PaymentServiceStripeTests | `ProcessStripeWebhookAsync_SeatsExpiredAndAutoRefunded_EmailsCustomerAboutRefund` | Fact | Ghế hết hạn và hệ thống tự hoàn tiền thì gửi email thông báo hoàn tiền cho khách. |
| PaymentServiceStripeTests | `ProcessStripeWebhookAsync_SeatsExpiredAndRefundFails_EmailsCustomerThatRefundIsPending` | Fact | Ghế hết hạn nhưng hoàn tiền thất bại thì gửi email báo khoản hoàn đang chờ xử lý. |
| PaymentServiceVnPayTests | `ProcessVnPayIpnAsync_ValidSuccessfulCallback_MarksPaymentSuccessAndConfirmsBooking` | Fact | Callback thành công hợp lệ đánh dấu thanh toán thành công và xác nhận booking. |
| PaymentServiceVnPayTests | `ProcessVnPayIpnAsync_InvalidSignature_ReturnsErrorAndDoesNotTouchPayment` | Fact | Chữ ký không hợp lệ trả mã lỗi, không động đến thanh toán. |
| PaymentServiceVnPayTests | `ProcessVnPayIpnAsync_UnknownTransactionId_ReturnsOrderNotFound` | Fact | Mã giao dịch không tồn tại trả kết quả đơn hàng không tìm thấy. |
| PaymentServiceVnPayTests | `ProcessVnPayIpnAsync_AmountMismatch_ReturnsInvalidAmountAndLeavesPaymentPending` | Fact | Số tiền không khớp trả lỗi số tiền, thanh toán vẫn `Pending`. |
| PaymentServiceVnPayTests | `ProcessVnPayIpnAsync_AlreadySuccessfulPayment_IsIdempotentAndDoesNotReconfirmBooking` | Fact | Thanh toán đã thành công thì IPN lặp không xác nhận lại booking (lũy đẳng). |
| PaymentServiceVnPayTests | `ProcessVnPayIpnAsync_FailedPaymentCallback_MarksPaymentFailedAndReleasesBooking` | Fact | Callback thất bại đánh dấu thanh toán `Failed` và giải phóng booking. |
| PaymentServiceVnPayTests | `ProcessVnPayIpnAsync_GatewayNotConfigured_ReturnsGatewayNotConfiguredWithoutThrowing` | Fact | Cổng chưa cấu hình trả kết quả `GatewayNotConfigured`, không ném ngoại lệ. |
| PaymentServiceVnPayTests | `ProcessVnPayIpnAsync_SeatsExpiredBeforeConfirmation_SwallowsExceptionAndStillReturnsSuccessToGateway` | Fact | Ghế hết hạn trước khi xác nhận: ngoại lệ được xử lý nội bộ và vẫn trả thành công cho cổng. |
| PaymentServiceMomoTests | `ProcessMomoIpnAsync_ValidSuccessfulCallback_MarksPaymentSuccessAndConfirmsBooking` | Fact | Callback thành công hợp lệ đánh dấu thanh toán thành công và xác nhận booking. |
| PaymentServiceMomoTests | `ProcessMomoIpnAsync_InvalidSignature_Returns97AndDoesNotTouchPayment` | Fact | Chữ ký không hợp lệ trả mã 97, không động đến thanh toán. |
| PaymentServiceMomoTests | `ProcessMomoIpnAsync_UnknownTransactionId_Returns1` | Fact | Mã giao dịch không tồn tại trả mã 1. |
| PaymentServiceMomoTests | `ProcessMomoIpnAsync_AmountMismatch_Returns4AndLeavesPaymentPending` | Fact | Số tiền không khớp trả mã 4, thanh toán vẫn `Pending`. |
| PaymentServiceMomoTests | `ProcessMomoIpnAsync_AlreadySuccessfulPayment_IsIdempotentAndDoesNotReconfirmBooking` | Fact | Thanh toán đã thành công thì IPN lặp không xác nhận lại booking (lũy đẳng). |
| PaymentServiceMomoTests | `ProcessMomoIpnAsync_DuplicateDeliveryOfSuccessfulCallback_ConfirmsBookingOnlyOnce` | Fact | Callback thành công gửi hai lần chỉ xác nhận booking một lần. |
| PaymentServiceMomoTests | `ProcessMomoIpnAsync_FailedPaymentCallback_MarksPaymentFailedAndProcessesFailure` | Fact | Callback thất bại đánh dấu thanh toán `Failed` và xử lý luồng thất bại. |
| PaymentServiceMomoTests | `ProcessMomoIpnAsync_GatewayNotConfigured_Returns99WithoutThrowing` | Fact | Cổng chưa cấu hình trả mã 99, không ném ngoại lệ. |

### 6.2.4 Nhóm Hết hạn giữ ghế

Nhóm gồm 7 test: `SeatExpirationServiceTests` 7 test (trong đó 1 `[Theory]`). Các test kiểm tra hai thao tác của `SeatExpirationService`: giải phóng ghế giữ quá hạn (`ReleaseStaleHeldSeatsAsync`) và cho hết hạn các booking `Pending` quá hạn (`ExpireDueBookingsAsync`), trong đó có trường hợp không giải phóng nhầm ghế đã được booking mới giữ lại.

Bảng 6.5. Test đơn vị nhóm Hết hạn giữ ghế

| File | Tên test | Loại | Hành vi kiểm tra và kết quả mong đợi |
|---|---|---|---|
| SeatExpirationServiceTests | `ReleaseStaleHeldSeatsAsync_SeatsHeldPastDeadline_AreReleasedToAvailable` | Fact | Ghế giữ quá hạn được trả về `Available`. |
| SeatExpirationServiceTests | `ReleaseStaleHeldSeatsAsync_SeatsStillWithinHoldWindow_AreNotReleased` | Fact | Ghế còn trong thời gian giữ không bị giải phóng. |
| SeatExpirationServiceTests | `ReleaseStaleHeldSeatsAsync_EmptyShowtimeIdFilter_ReleasesNothingWithoutQuerying` | Fact | Bộ lọc mã suất chiếu rỗng thì không giải phóng gì và không truy vấn. |
| SeatExpirationServiceTests | `ExpireDueBookingsAsync_PendingBookingPastExpiresAt_IsExpiredAndSeatsReleased` | Fact | Booking `Pending` quá `ExpiresAt` chuyển sang hết hạn và ghế được giải phóng. |
| SeatExpirationServiceTests | `ExpireDueBookingsAsync_PendingBookingNotYetExpired_IsLeftUntouched` | Fact | Booking chưa quá hạn được giữ nguyên. |
| SeatExpirationServiceTests | `ExpireDueBookingsAsync_OldBookingExpired_DoesNotReleaseSeatReheldByNewBooking` | Fact | Booking cũ hết hạn không giải phóng ghế đã được booking mới giữ lại. |
| SeatExpirationServiceTests | `ExpireDueBookingsAsync_CounterBookingLinkedToMember_ReleasesSeatsHeldByEmployee` | Theory | Booking quầy gắn hội viên hết hạn thì giải phóng ghế do nhân viên giữ, với hai trường hợp ghế đã hoặc chưa gắn vào booking (tham số `seatBoundToBooking`). |

### 6.2.5 Nhóm Suất chiếu

Nhóm gồm 71 test: `ShowtimeLifecycleTests` 5 test; `ShowtimeServiceAutoScheduleTests` 4 test; `ShowtimeServiceCreateTests` 14 test; `ShowtimeServiceFixesTests` 19 test (trong đó 2 `[Theory]`); `OperatingHoursTests` 6 test; `ShowtimeSchedulerTests` 8 test (trong đó 1 `[Theory]`); `ShowtimeValidatorsTests` 15 test. Nhóm bao quát vòng đời suất chiếu (hoàn tất suất đã kết thúc, kết thúc phim không còn suất, ngừng bán trực tuyến trước giờ chiếu), tạo, sửa, xóa, tạo hàng loạt và nhân bản suất, xếp lịch tự động, kiểm tra giờ hoạt động của rạp, bộ xếp lịch `ShowtimeScheduler`, tác vụ hoàn tiền khi xóa suất và các validator của yêu cầu suất chiếu.

Bảng 6.6. Test đơn vị nhóm Suất chiếu

| File | Tên test | Loại | Hành vi kiểm tra và kết quả mong đợi |
|---|---|---|---|
| ShowtimeLifecycleTests | `CompleteEndedShowtimesAsync_OnlyScheduledShowtimesPastEndTimeBecomeCompleted` | Fact | Chỉ suất chiếu `Scheduled` đã quá giờ kết thúc mới chuyển sang `Completed`. |
| ShowtimeLifecycleTests | `EndMoviesWithoutUpcomingShowtimesAsync_EndsOnlyNowShowingMoviesWithNoRecentOrFutureShowtimes` | Fact | Chỉ phim đang chiếu không còn suất gần đây hoặc sắp tới mới chuyển sang kết thúc. |
| ShowtimeLifecycleTests | `HoldSeatsAsync_WithinStopSellingWindow_ThrowsBusinessRuleException` | Fact | Giữ ghế trong khoảng ngừng bán trực tuyến trước giờ chiếu bị từ chối bằng `BusinessRuleException`. |
| ShowtimeLifecycleTests | `IsOnlineSalesClosed_ZeroMinutes_NeverClosesEarly` | Fact | Cấu hình 0 phút thì không bao giờ đóng bán sớm. |
| ShowtimeLifecycleTests | `IsOnlineSalesClosed_BoundaryIsInclusive` | Fact | Đúng thời điểm biên thì việc bán đã đóng (biên bao gồm). |
| ShowtimeServiceAutoScheduleTests | `CreateShowtimeAsync_EndsAfterClosing_ThrowsUnlessAllowAfterHours` | Fact | Suất kết thúc sau giờ đóng cửa bị từ chối, trừ khi cho phép ngoài giờ (`AllowAfterHours`). |
| ShowtimeServiceAutoScheduleTests | `PreviewAutoScheduleAsync_SchedulesAroundExistingShowtimeWithoutOverlap` | Fact | Xếp lịch tự động xem trước né suất đã có, không chồng lấn. |
| ShowtimeServiceAutoScheduleTests | `PreviewAutoScheduleAsync_EndedMovie_ThrowsBusinessRuleException` | Fact | Phim đã kết thúc không thể xếp lịch; ném `BusinessRuleException`. |
| ShowtimeServiceAutoScheduleTests | `ApplyAutoScheduleAsync_SkipsConflictingItemsAndCreatesTheRest` | Fact | Áp dụng lịch tự động bỏ qua mục xung đột và tạo các mục còn lại. |
| ShowtimeServiceCreateTests | `CreateShowtimeAsync_ValidRequest_GeneratesOneShowtimeSeatPerActiveNonAisleSeat` | Fact | Tạo suất hợp lệ sinh một `ShowtimeSeat` cho mỗi ghế đang hoạt động và không phải lối đi. |
| ShowtimeServiceCreateTests | `CreateShowtimeAsync_SeatPriceUsesSeatTypeMultiplier` | Fact | Giá ghế được nhân theo hệ số của loại ghế. |
| ShowtimeServiceCreateTests | `CreateShowtimeAsync_MovieDoesNotExist_ThrowsValidationAppException` | Fact | Phim không tồn tại thì ném `ValidationAppException`. |
| ShowtimeServiceCreateTests | `CreateShowtimeAsync_RoomDoesNotExist_ThrowsValidationAppException` | Fact | Phòng không tồn tại thì ném `ValidationAppException`. |
| ShowtimeServiceCreateTests | `CreateShowtimeAsync_MovieEnded_ThrowsBusinessRuleException` | Fact | Phim đã kết thúc không thể tạo suất; ném `BusinessRuleException`. |
| ShowtimeServiceCreateTests | `CreateShowtimeAsync_RoomInactive_ThrowsBusinessRuleException` | Fact | Phòng ngừng hoạt động không thể tạo suất; ném `BusinessRuleException`. |
| ShowtimeServiceCreateTests | `CreateShowtimeAsync_RoomHasNoSeatMap_ThrowsBusinessRuleException` | Fact | Phòng chưa có sơ đồ ghế không thể tạo suất; ném `BusinessRuleException`. |
| ShowtimeServiceCreateTests | `CreateShowtimeAsync_OutsideOpeningHours_ThrowsBusinessRuleException` | Fact | Suất ngoài giờ mở cửa bị từ chối bằng `BusinessRuleException`. |
| ShowtimeServiceCreateTests | `CreateShowtimeAsync_OverlappingTimeSlotInSameRoom_ThrowsConflict` | Fact | Khung giờ chồng lấn trong cùng phòng thì báo Conflict. |
| ShowtimeServiceCreateTests | `CreateShowtimeAsync_BackToBackAfterBufferInSameRoom_Succeeds` | Fact | Suất liền kề nhưng đã đủ khoảng đệm trong cùng phòng được tạo thành công. |
| ShowtimeServiceCreateTests | `CreateShowtimeAsync_OverlapCheckIgnoresCancelledShowtimesInSameRoom` | Fact | Kiểm tra chồng lấn bỏ qua các suất đã hủy trong cùng phòng. |
| ShowtimeServiceCreateTests | `UpdateShowtimeAsync_ChangingRoomId_ThrowsBusinessRuleException` | Fact | Đổi phòng của suất chiếu bị từ chối bằng `BusinessRuleException`. |
| ShowtimeServiceCreateTests | `UpdateShowtimeAsync_HasConfirmedBooking_ThrowsConflict` | Fact | Suất đã có booking xác nhận thì không sửa được; báo Conflict. |
| ShowtimeServiceCreateTests | `DeleteShowtimeAsync_CancelsShowtimeAndPendingBookingsAndReleasesHeldSeats` | Fact | Xóa suất chuyển suất sang hủy, hủy các booking `Pending` và giải phóng ghế đang giữ. |
| ShowtimeServiceFixesTests | `UpdateShowtimeAsync_PersistsChangesToDatabase` | Fact | Cập nhật suất ghi thay đổi xuống cơ sở dữ liệu. |
| ShowtimeServiceFixesTests | `UpdateShowtimeAsync_NotScheduled_ThrowsBusinessRuleException` | Theory | Suất không ở trạng thái `Scheduled` không được cập nhật; ném `BusinessRuleException`. |
| ShowtimeServiceFixesTests | `UpdateShowtimeAsync_ActivePendingBooking_ThrowsConflict` | Fact | Suất có booking `Pending` còn hiệu lực thì không sửa được; báo Conflict. |
| ShowtimeServiceFixesTests | `UpdateShowtimeAsync_ActiveHeldSeat_ThrowsConflict` | Fact | Suất có ghế đang được giữ thì không sửa được; báo Conflict. |
| ShowtimeServiceFixesTests | `UpdateShowtimeAsync_ExpiredPendingBooking_DoesNotBlock` | Fact | Booking `Pending` đã hết hạn không cản trở việc cập nhật. |
| ShowtimeServiceFixesTests | `DeleteShowtimeAsync_NotScheduled_ThrowsAndLeavesBookingsUntouched` | Theory | Suất không ở trạng thái `Scheduled` không xóa được và booking giữ nguyên. |
| ShowtimeServiceFixesTests | `DeleteShowtimeAsync_OnlinePayment_QueuesRefundJobInsteadOfCallingGateway` | Fact | Booking thanh toán trực tuyến khi xóa suất được đưa vào tác vụ hoàn tiền thay vì gọi cổng trực tiếp. |
| ShowtimeServiceFixesTests | `ShowtimeRefundJob_Success_MarksRefunded_AndIsIdempotent` | Fact | Tác vụ hoàn tiền thành công đánh dấu đã hoàn và chạy lại không hoàn lần hai. |
| ShowtimeServiceFixesTests | `ShowtimeRefundJob_GatewayRejects_KeepsPaymentInManualQueue` | Fact | Cổng từ chối hoàn tiền thì khoản thanh toán được giữ lại hàng chờ xử lý thủ công. |
| ShowtimeServiceFixesTests | `ApplyAutoScheduleAsync_UnknownMovie_IsSkippedWithReason_OthersStillCreated` | Fact | Phim không tồn tại bị bỏ qua kèm lý do, các mục khác vẫn được tạo. |
| ShowtimeServiceFixesTests | `BulkCreateShowtimesAsync_SkipsOverlapWithExistingShowtime_AndCreatesTheRest` | Fact | Tạo hàng loạt bỏ qua khung trùng suất đã có và tạo phần còn lại. |
| ShowtimeServiceFixesTests | `PreviewAutoScheduleAsync_DateRange_PlacesShowsOnEveryDay` | Fact | Xem trước lịch tự động theo khoảng ngày xếp suất vào mọi ngày trong khoảng. |
| ShowtimeServiceFixesTests | `CloneShowtimesAsync_ShiftsSourceShowtimes_AndSkipsOnSecondRun` | Fact | Nhân bản dời các suất nguồn sang khoảng mới; chạy lần hai bỏ qua suất đã tồn tại. |
| ShowtimeServiceFixesTests | `CloneShowtimesAsync_NoSourceShowtimes_ThrowsBusinessRuleException` | Fact | Không có suất nguồn để nhân bản thì ném `BusinessRuleException`. |
| ShowtimeServiceFixesTests | `CreateShowtimeAsync_StartTimeInThePast_ThrowsBusinessRuleException` | Fact | Giờ bắt đầu trong quá khứ bị từ chối khi tạo suất. |
| ShowtimeServiceFixesTests | `UpdateShowtimeAsync_StartTimeInThePast_ThrowsBusinessRuleException` | Fact | Giờ bắt đầu trong quá khứ bị từ chối khi cập nhật suất. |
| ShowtimeServiceFixesTests | `BulkCreateShowtimesAsync_DatesInThePast_AreSkippedWithFutureReason` | Fact | Tạo hàng loạt với ngày đã qua thì bỏ qua kèm lý do yêu cầu thời điểm tương lai. |
| ShowtimeServiceFixesTests | `DeleteShowtimeAsync_ShowtimeAlreadyStarted_ThrowsAndKeepsScheduled` | Fact | Suất đã bắt đầu không xóa được và vẫn giữ trạng thái `Scheduled`. |
| ShowtimeServiceFixesTests | `DeleteShowtimeAsync_PendingBooking_MarksPendingPaymentsFailed` | Fact | Xóa suất có booking `Pending` thì các thanh toán `Pending` được đánh dấu thất bại. |
| OperatingHoursTests | `Violation_StartsBeforeOpening_ReturnsMessage` | Fact | Bắt đầu trước giờ mở cửa trả thông báo vi phạm. |
| OperatingHoursTests | `Violation_EndsExactlyAtClosing_ReturnsNull` | Fact | Kết thúc đúng giờ đóng cửa không vi phạm (trả `null`). |
| OperatingHoursTests | `Violation_CrossesMidnightPastClosing_ReturnsMessageUnlessAllowed` | Fact | Kéo qua nửa đêm vượt giờ đóng cửa trả thông báo, trừ khi cho phép ngoài giờ. |
| OperatingHoursTests | `Violation_AllowedAfterHoursButStartsAtClosing_ReturnsMessage` | Fact | Cho phép ngoài giờ nhưng bắt đầu đúng giờ đóng cửa vẫn trả thông báo. |
| OperatingHoursTests | `Violation_OvernightWindow_AcceptsStartAfterMidnightOfPreviousDay` | Fact | Khung giờ qua đêm chấp nhận suất bắt đầu sau nửa đêm của ngày kế tiếp. |
| OperatingHoursTests | `Violation_NoSettings_ReturnsNull` | Fact | Chưa có cấu hình giờ hoạt động thì không vi phạm (trả `null`). |
| ShowtimeSchedulerTests | `Build_PlacesRequestedShowsWithoutOverlapAndWithinOpeningHours` | Fact | Bộ xếp lịch đặt đủ số suất yêu cầu, không chồng lấn, trong giờ mở cửa. |
| ShowtimeSchedulerTests | `Build_HotMovieGetsLargerRoomThanColdMovie` | Fact | Phim nóng được xếp phòng lớn hơn phim ít khách. |
| ShowtimeSchedulerTests | `Build_SpreadsShowsOfOneMovieAcrossTheDay` | Fact | Các suất của một phim được rải đều trong ngày. |
| ShowtimeSchedulerTests | `Build_RespectsExistingShowtimes` | Fact | Bộ xếp lịch tránh các suất đã có. |
| ShowtimeSchedulerTests | `Build_MovieLongerThanWindow_PlacesNothing` | Fact | Phim dài hơn khung giờ khả dụng thì không xếp suất nào. |
| ShowtimeSchedulerTests | `Build_AllowAfterHours_AllowsEndPastClosingButNotStart` | Fact | Cho phép ngoài giờ thì được kết thúc sau giờ đóng cửa nhưng không được bắt đầu sau giờ đó. |
| ShowtimeSchedulerTests | `Build_UsesConfiguredStepMinutes` | Fact | Bộ xếp lịch dùng bước phút theo cấu hình. |
| ShowtimeSchedulerTests | `Heat_CalculatesHeatCorrectlyOn1To5RatingScale` | Theory | Độ "nóng" của phim được tính đúng trên thang đánh giá 1–5, với nhiều bộ `[InlineData]`. |
| ShowtimeValidatorsTests | `UpsertShowtimeRequestValidatorTests.ValidRequest_PassesValidation` | Fact | Yêu cầu tạo/cập nhật suất hợp lệ vượt qua kiểm tra. |
| ShowtimeValidatorsTests | `NonPositiveMovieId_FailsValidation` | Fact | Mã phim không dương không hợp lệ. |
| ShowtimeValidatorsTests | `NonPositiveRoomId_FailsValidation` | Fact | Mã phòng không dương không hợp lệ. |
| ShowtimeValidatorsTests | `NegativeBasePrice_FailsValidation` | Fact | Giá cơ bản âm không hợp lệ. |
| ShowtimeValidatorsTests | `ZeroBasePrice_IsAllowed` | Fact | Giá cơ bản bằng 0 được chấp nhận. |
| ShowtimeValidatorsTests | `ToBeforeFrom_FailsValidation` | Fact | Ngày kết thúc trước ngày bắt đầu không hợp lệ. |
| ShowtimeValidatorsTests | `RangeWiderThanMax92Days_FailsValidation` | Fact | Khoảng ngày rộng hơn tối đa 92 ngày không hợp lệ. |
| ShowtimeValidatorsTests | `ValidNarrowRange_PassesValidation` | Fact | Khoảng ngày hẹp hợp lệ. |
| ShowtimeValidatorsTests | `BulkCreateShowtimesRequestValidatorTests.ValidRequest_PassesValidation` | Fact | Yêu cầu tạo hàng loạt hợp lệ vượt qua kiểm tra. |
| ShowtimeValidatorsTests | `InvalidDayOfWeekCode_FailsValidation` | Fact | Mã thứ trong tuần sai không hợp lệ. |
| ShowtimeValidatorsTests | `InvalidTimeFormat_FailsValidation` | Fact | Giờ sai định dạng không hợp lệ. |
| ShowtimeValidatorsTests | `EmptyTimes_FailsValidation` | Fact | Danh sách giờ chiếu rỗng không hợp lệ. |
| ShowtimeValidatorsTests | `DateRangeToBeforeFrom_FailsValidation` | Fact | Khoảng ngày có "đến" trước "từ" không hợp lệ. |
| ShowtimeValidatorsTests | `EmptyShowtimeIds_FailsValidation` | Fact | Danh sách mã suất rỗng không hợp lệ. |
| ShowtimeValidatorsTests | `NonPositiveShowtimeId_FailsValidation` | Fact | Mã suất không dương không hợp lệ. |

### 6.2.6 Nhóm Phòng chiếu

Nhóm gồm 27 test: `RoomServiceSeatLayoutTests` 8 test; `SeatLayoutRulesTests` 19 test. `RoomServiceSeatLayoutTests` kiểm tra tác động của việc sửa sơ đồ ghế hoặc ngừng hoạt động phòng lên các suất chiếu tương lai. `SeatLayoutRulesTests` kiểm tra quy tắc chọn ghế theo sơ đồ: ba chế độ `Strict`, `AllowEdge`, `Off` về khoảng trống một ghế, quy tắc ghế đôi và thuật toán gợi ý ghế liên tiếp.

Bảng 6.7. Test đơn vị nhóm Phòng chiếu

| File | Tên test | Loại | Hành vi kiểm tra và kết quả mong đợi |
|---|---|---|---|
| RoomServiceSeatLayoutTests | `UpdateSeatLayoutAsync_ChangingSeatTypeOfBookedSeatInFutureShowtime_ThrowsConflict` | Fact | Đổi loại ghế đã được đặt trong suất tương lai thì báo Conflict. |
| RoomServiceSeatLayoutTests | `UpdateSeatLayoutAsync_DeactivatingHeldSeatInFutureShowtime_ThrowsConflict` | Fact | Vô hiệu hóa ghế đang được giữ trong suất tương lai thì báo Conflict. |
| RoomServiceSeatLayoutTests | `UpdateSeatLayoutAsync_BookedSeatOnlyInPastShowtime_AllowsChange` | Fact | Ghế chỉ có đặt chỗ ở suất đã qua thì được phép thay đổi. |
| RoomServiceSeatLayoutTests | `UpdateSeatLayoutAsync_ChangingSeatTypeOfAvailableSeat_RepricesFutureShowtimeSeat` | Fact | Đổi loại ghế còn trống thì giá ghế ở suất tương lai được tính lại. |
| RoomServiceSeatLayoutTests | `UpdateSeatLayoutAsync_AddingSeat_CreatesAvailableShowtimeSeatForFutureShowtime` | Fact | Thêm ghế mới tạo `ShowtimeSeat` ở trạng thái `Available` cho suất tương lai. |
| RoomServiceSeatLayoutTests | `UpdateSeatLayoutAsync_DeactivatingAvailableSeat_RemovesItFromFutureShowtime` | Fact | Vô hiệu hóa ghế còn trống thì ghế bị gỡ khỏi suất tương lai. |
| RoomServiceSeatLayoutTests | `UpdateRoomAsync_DeactivatingRoomWithBookedSeatInFutureShowtime_ThrowsConflict` | Fact | Ngừng hoạt động phòng có ghế đã đặt trong suất tương lai thì báo Conflict. |
| RoomServiceSeatLayoutTests | `UpdateRoomAsync_DeactivatingRoomWithoutLiveSeats_Succeeds` | Fact | Ngừng hoạt động phòng không có ghế đang sử dụng được thực hiện thành công. |
| SeatLayoutRulesTests | `ValidateSelection_NoMatchingSeats_ReturnsNull` | Fact | Không có ghế phù hợp để xét thì không có vi phạm (trả `null`). |
| SeatLayoutRulesTests | `ValidateSelection_Strict_PickLeavesSingleSeatBesideWall_ReturnsMessage` | Fact | Chế độ `Strict`: chọn làm chừa một ghế lẻ sát tường thì trả thông báo. |
| SeatLayoutRulesTests | `ValidateSelection_Strict_PickAtEdgeLeavingRunOfThree_ReturnsNull` | Fact | Chế độ `Strict`: chọn ở mép chừa dãy ba ghế liền thì hợp lệ. |
| SeatLayoutRulesTests | `ValidateSelection_Strict_PickTwoAtEdgeLeavingRunOfTwo_ReturnsNull` | Fact | Chế độ `Strict`: chọn hai ghế ở mép chừa dãy hai ghế liền thì hợp lệ. |
| SeatLayoutRulesTests | `ValidateSelection_Strict_PickSplitsRowIntoSingleSeatInTheMiddle_ReturnsMessage` | Fact | Chế độ `Strict`: chọn làm hàng tách thành ghế lẻ ở giữa thì trả thông báo. |
| SeatLayoutRulesTests | `ValidateSelection_AllowEdge_SingleSeatBesideWall_ReturnsNull` | Fact | Chế độ `AllowEdge`: ghế lẻ sát tường được chấp nhận. |
| SeatLayoutRulesTests | `ValidateSelection_AllowEdge_SingleSeatBetweenPickedAndAvailableNeighbour_StillReturnsMessage` | Fact | Chế độ `AllowEdge`: ghế lẻ nằm giữa ghế đã chọn và ghế trống vẫn bị báo. |
| SeatLayoutRulesTests | `ValidateSelection_Off_AllowsAnyGap` | Fact | Chế độ tắt cho phép mọi khoảng trống. |
| SeatLayoutRulesTests | `ValidateSelection_ExistingSingleSeatNotAdjacentToPick_IsIgnored` | Fact | Ghế lẻ sẵn có không liền kề ghế vừa chọn thì bị bỏ qua. |
| SeatLayoutRulesTests | `ValidateSelection_OnlyChecksRowsContainingPickedSeats` | Fact | Chỉ kiểm tra các hàng có chứa ghế được chọn. |
| SeatLayoutRulesTests | `ValidateSelection_CouplePickedPartially_ReturnsMessage` | Fact | Chọn một nửa cặp ghế đôi thì trả thông báo. |
| SeatLayoutRulesTests | `ValidateSelection_CouplePickedPartiallyEvenWhenGapRuleOff_ReturnsMessage` | Fact | Chọn một nửa cặp ghế đôi vẫn trả thông báo kể cả khi quy tắc khoảng trống tắt. |
| SeatLayoutRulesTests | `ValidateSelection_CouplePickedTogether_ReturnsNull` | Fact | Chọn cả cặp ghế đôi thì hợp lệ. |
| SeatLayoutRulesTests | `Suggest_ReturnsConsecutiveSeatsThatDoNotLeaveSingleSeatAtEdge` | Fact | Gợi ý trả các ghế liên tiếp không chừa ghế lẻ ở mép. |
| SeatLayoutRulesTests | `Suggest_SumsPriceAndReturnsShowtimeSeatIds` | Fact | Gợi ý cộng tổng giá và trả danh sách `ShowtimeSeatId`. |
| SeatLayoutRulesTests | `Suggest_NotEnoughConsecutiveSeats_ReturnsEmpty` | Fact | Không đủ ghế liên tiếp thì danh sách gợi ý rỗng. |
| SeatLayoutRulesTests | `Suggest_SkipsUnavailableSeats` | Fact | Gợi ý bỏ qua ghế không còn khả dụng. |
| SeatLayoutRulesTests | `Suggest_LimitsResultsToTopAndOnePerRow` | Fact | Gợi ý giới hạn số kết quả đầu bảng, mỗi hàng một gợi ý. |
| SeatLayoutRulesTests | `Suggest_CoupleSeatsAreNeverSplitAcrossWindow` | Fact | Gợi ý không tách cặp ghế đôi qua ranh giới của cửa sổ chọn. |

### 6.2.7 Nhóm Báo cáo

Nhóm gồm 17 test: `DashboardServiceTests` 8 test; `ReportServiceTests` 9 test. Các test kiểm tra số liệu tổng quan của bảng điều khiển (doanh thu, phần trăm thay đổi, tỷ lệ lấp đầy, ranh giới ngày theo giờ Việt Nam) và các báo cáo doanh thu, phim hàng đầu, hiệu suất nhân viên, trong đó có quy tắc chỉ tính phần tiền giữ lại với booking hoàn tiền.

Bảng 6.8. Test đơn vị nhóm Báo cáo

| File | Tên test | Loại | Hành vi kiểm tra và kết quả mong đợi |
|---|---|---|---|
| DashboardServiceTests | `GetSummaryAsync_NoBookingsAtAll_ReturnsZeroesAndNullChangePercent` | Fact | Chưa có booking thì các chỉ số bằng 0 và phần trăm thay đổi là `null`. |
| DashboardServiceTests | `GetSummaryAsync_TodayRevenueHigherThanYesterday_ComputesPositiveChangePercent` | Fact | Doanh thu hôm nay cao hơn hôm qua thì phần trăm thay đổi dương. |
| DashboardServiceTests | `GetSummaryAsync_OnlyPendingBookingsWithActiveHold_CountAsActivePendingBookings` | Fact | Booking `Pending` còn thời gian giữ được đếm là booking chờ thanh toán đang hoạt động. |
| DashboardServiceTests | `GetSummaryAsync_ShowtimesTodayWithSomeSeatsBooked_ComputesOccupancyPercent` | Fact | Suất chiếu hôm nay có ghế đã đặt thì tính được tỷ lệ lấp đầy. |
| DashboardServiceTests | `GetInsightsAsync_DaysOutOfRange_IsClampedTo90` | Fact | Tham số số ngày vượt khoảng cho phép bị chặn ở 90. |
| DashboardServiceTests | `GetInsightsAsync_IncludesTodaysShowtimesAndTopMoviesWithinWindow` | Fact | Dữ liệu insight gồm suất chiếu hôm nay và phim hàng đầu trong khoảng thời gian. |
| DashboardServiceTests | `GetSummaryAsync_BookingJustAfterVietnamMidnight_CountsAsTodayAndJustBeforeAsYesterday` | Fact | Booking ngay sau nửa đêm giờ Việt Nam tính vào hôm nay, ngay trước nửa đêm tính vào hôm qua. |
| DashboardServiceTests | `GetSummaryAsync_PartiallyRefundedPaymentToday_CountsOnlyRetainedAmount` | Fact | Thanh toán hoàn một phần chỉ tính phần tiền được giữ lại. |
| ReportServiceTests | `GetRevenueReportAsync_InvalidGroupBy_ThrowsValidationAppException` | Fact | Giá trị `groupBy` không hợp lệ thì ném `ValidationAppException`. |
| ReportServiceTests | `GetRevenueReportAsync_ConfirmedBookingsWithTicketsAndCombos_ComputesTicketComboAndTotalRevenueCorrectly` | Fact | Booking xác nhận có vé và combo thì doanh thu vé, combo và tổng được tính đúng. |
| ReportServiceTests | `GetRevenueReportAsync_BookingOutsideDateRange_IsExcludedFromReport` | Fact | Booking ngoài khoảng ngày bị loại khỏi báo cáo. |
| ReportServiceTests | `GetRevenueReportAsync_PendingOrCancelledBookings_AreExcludedFromRevenue` | Fact | Booking `Pending` hoặc đã hủy không tính vào doanh thu. |
| ReportServiceTests | `GetRevenueReportAsync_CancelledBookingWithPartialRefund_CountsRetainedAmount` | Fact | Booking hủy có hoàn tiền một phần chỉ tính phần tiền giữ lại. |
| ReportServiceTests | `GetRevenueReportAsync_FullyRefundedBooking_ContributesNothing` | Fact | Booking hoàn tiền toàn bộ không đóng góp doanh thu. |
| ReportServiceTests | `GetRevenueReportAsync_GroupsByPaidAtInVietnamTime` | Fact | Doanh thu được nhóm theo thời điểm thanh toán quy đổi giờ Việt Nam. |
| ReportServiceTests | `GetTopMoviesReportAsync_MultipleMoviesWithDifferentRevenue_OrdersDescendingAndRespectsLimit` | Fact | Báo cáo phim hàng đầu sắp theo doanh thu giảm dần và tôn trọng giới hạn số lượng. |
| ReportServiceTests | `GetEmployeePerformanceReportAsync_OnlyCountsConfirmedCounterBookingsWithEmployee` | Fact | Hiệu suất nhân viên chỉ tính booking quầy đã xác nhận có gắn nhân viên. |

### 6.2.8 Nhóm Cấu hình, đánh giá, nhân viên

Nhóm gồm 39 test: `ProductionOptionsValidatorTests` 5 test; `EmployeeServiceUserStatusTests` 2 test; `EmployeeServiceRoleRulesTests` 6 test; `ReviewServiceTests` 2 test; `ReviewValidatorsTests` 3 test (trong đó 2 `[Theory]`); `CinemaSettingsPolicyTests` 10 test; `SettingsServiceTests` 11 test (trong đó 2 `[Theory]`). Nhóm gồm kiểm tra cấu hình production (`ProductionOptionsValidator`), quản lý tài khoản nhân viên (khóa tài khoản, đổi vai trò, bảo vệ quản trị viên hoạt động cuối cùng), đánh giá phim và cập nhật điểm trung bình, cùng chính sách cấu hình rạp (giới hạn đặt vé, cửa sổ điểm thành viên, hạn hủy và tỷ lệ hoàn tiền, chế độ khoảng trống ghế, giờ hoạt động).

Bảng 6.9. Test đơn vị nhóm Cấu hình, đánh giá, nhân viên

| File | Tên test | Loại | Hành vi kiểm tra và kết quả mong đợi |
|---|---|---|---|
| ProductionOptionsValidatorTests | `Validate_CompleteConfiguration_ReturnsNoErrors` | Fact | Cấu hình production đầy đủ không có lỗi. |
| ProductionOptionsValidatorTests | `Validate_DefaultOptions_ReportsLocalhostAndMissingSecrets` | Fact | Cấu hình mặc định bị báo lỗi vì còn địa chỉ localhost và thiếu khóa bí mật. |
| ProductionOptionsValidatorTests | `Validate_VnPayEnabledWithoutPublicUrls_ReportsUrls` | Fact | Bật VNPay mà thiếu URL công khai thì báo lỗi về URL. |
| ProductionOptionsValidatorTests | `Validate_StripeDefaultReturnUrlWithSecretKey_ReportsReturnUrl` | Fact | Có khóa bí mật Stripe nhưng còn URL trả về mặc định thì báo lỗi `ReturnUrl`. |
| ProductionOptionsValidatorTests | `Validate_DisabledGateways_AreNotChecked` | Fact | Cổng thanh toán đã tắt không bị kiểm tra. |
| EmployeeServiceUserStatusTests | `UpdateUserStatusAsync_Deactivate_RevokesAllRefreshTokens` | Fact | Vô hiệu hóa tài khoản thu hồi mọi refresh token. |
| EmployeeServiceUserStatusTests | `UpdateUserStatusAsync_Activate_DoesNotTouchRefreshTokens` | Fact | Kích hoạt lại tài khoản không động đến refresh token. |
| EmployeeServiceRoleRulesTests | `UpdateUserRoleAsync_ChangingOwnRole_Throws` | Fact | Tự đổi vai trò của chính mình bị từ chối. |
| EmployeeServiceRoleRulesTests | `UpdateUserRoleAsync_DemotingLastActiveAdmin_Throws` | Fact | Hạ vai trò quản trị viên hoạt động cuối cùng bị từ chối. |
| EmployeeServiceRoleRulesTests | `UpdateUserRoleAsync_DemotingAdminWhenAnotherActiveAdminExists_Succeeds` | Fact | Hạ vai trò quản trị viên khi còn quản trị viên hoạt động khác được thực hiện thành công. |
| EmployeeServiceRoleRulesTests | `UpdateUserStatusAsync_LockingSelf_Throws` | Fact | Tự khóa tài khoản của chính mình bị từ chối. |
| EmployeeServiceRoleRulesTests | `UpdateUserStatusAsync_LockingLastActiveAdmin_Throws` | Fact | Khóa quản trị viên hoạt động cuối cùng bị từ chối. |
| EmployeeServiceRoleRulesTests | `UpdateEmployeeAsync_DeactivatingOwnProfile_Throws` | Fact | Tự vô hiệu hóa hồ sơ nhân viên của chính mình bị từ chối. |
| ReviewServiceTests | `CreateReviewAsync_FirstReview_UpdatesMovieAverageRating` | Fact | Đánh giá đầu tiên cập nhật điểm trung bình của phim. |
| ReviewServiceTests | `CreateReviewAsync_SecondReview_RecalculatesAverageAcrossAllReviews` | Fact | Đánh giá thứ hai làm điểm trung bình được tính lại trên toàn bộ đánh giá. |
| ReviewValidatorsTests | `ValidRating_PassesValidation` | Theory | Điểm đánh giá hợp lệ vượt qua kiểm tra. |
| ReviewValidatorsTests | `RatingOutOf1To5Range_FailsValidation` | Theory | Điểm đánh giá ngoài khoảng 1–5 không hợp lệ. |
| ReviewValidatorsTests | `CommentExceeding1000Chars_FailsValidation` | Fact | Bình luận vượt 1000 ký tự không hợp lệ. |
| CinemaSettingsPolicyTests | `HoldSeatsAsync_SeatCountAboveSettingsLimit_ThrowsValidation` | Fact | Số ghế vượt giới hạn trong cấu hình thì giữ ghế bị từ chối bằng lỗi xác thực. |
| CinemaSettingsPolicyTests | `HoldSeatsAsync_ComboQuantityAboveSettingsLimit_ThrowsValidation` | Fact | Số lượng combo vượt giới hạn trong cấu hình thì giữ ghế bị từ chối bằng lỗi xác thực. |
| CinemaSettingsPolicyTests | `SumActiveAsync_UsesLoyaltyWindowMonthsFromSettings` | Fact | Tổng điểm hiệu lực dùng số tháng của cửa sổ điểm lấy từ cấu hình. |
| CinemaSettingsPolicyTests | `Cutoff_UsesGivenMonths` | Fact | Mốc cắt của cửa sổ điểm tính theo số tháng được truyền vào. |
| CinemaSettingsPolicyTests | `MovieEndCutoff_UsesGivenGraceDays` | Fact | Mốc kết thúc phim tính theo số ngày ân hạn được truyền vào. |
| CinemaSettingsPolicyTests | `LoadShowtimeLookaheadDaysAsync_ReadsSettingsAndFallsBackToDefault` | Fact | Số ngày xem trước suất chiếu đọc từ cấu hình, rơi về giá trị mặc định khi thiếu. |
| CinemaSettingsPolicyTests | `UpdateSettingsAsync_PolicyFieldsProvided_ArePersisted` | Fact | Các trường chính sách có trong yêu cầu được lưu lại. |
| CinemaSettingsPolicyTests | `UpdateSettingsAsync_PolicyFieldsOmitted_KeepExistingValues` | Fact | Các trường chính sách bị bỏ trống giữ nguyên giá trị cũ. |
| CinemaSettingsPolicyTests | `UpdateSettingsAsync_BothHeatWeightsZero_ThrowsBusinessRule` | Fact | Cả hai trọng số "nóng" bằng 0 thì ném lỗi quy tắc nghiệp vụ. |
| CinemaSettingsPolicyTests | `Validator_OutOfRangePolicyValues_AreRejected` | Fact | Giá trị chính sách ngoài khoảng cho phép bị validator từ chối. |
| SettingsServiceTests | `GetSettingsAsync_SettingsRowExists_ReturnsSettings` | Fact | Có dòng cấu hình thì trả về cấu hình. |
| SettingsServiceTests | `GetSettingsAsync_SettingsRowMissing_ThrowsNotFound` | Fact | Thiếu dòng cấu hình thì báo NotFound. |
| SettingsServiceTests | `UpdateSettingsAsync_ValidRequest_PersistsAllFields` | Fact | Yêu cầu cập nhật hợp lệ lưu toàn bộ các trường. |
| SettingsServiceTests | `UpdateSettingsAsync_OptionalFieldsOmitted_KeepsExistingValues` | Fact | Các trường tùy chọn bị bỏ trống giữ nguyên giá trị cũ. |
| SettingsServiceTests | `UpdateSettingsAsync_UnknownSeatGapRuleMode_KeepsExistingMode` | Fact | Chế độ quy tắc khoảng trống ghế không xác định thì giữ chế độ hiện có. |
| SettingsServiceTests | `UpdateSettingsAsync_BlankOperatingHours_ClearsThem` | Fact | Giờ hoạt động để trống thì được xóa khỏi cấu hình. |
| SettingsServiceTests | `UpdateSettingsAsync_RefundPercentOutOfRange_ThrowsAndDoesNotPersist` | Theory | Tỷ lệ hoàn tiền ngoài khoảng cho phép thì ném lỗi và không lưu. |
| SettingsServiceTests | `UpdateSettingsAsync_RefundPercentAtBoundary_Succeeds` | Theory | Tỷ lệ hoàn tiền đúng giá trị biên được chấp nhận. |
| SettingsServiceTests | `UpdateSettingsAsync_NegativeCancelDeadlineHours_Throws` | Fact | Số giờ hạn hủy âm bị từ chối. |
| SettingsServiceTests | `UpdateSettingsAsync_InvalidOperatingTimeFormat_Throws` | Fact | Giờ hoạt động sai định dạng bị từ chối. |
| SettingsServiceTests | `UpdateSettingsAsync_SettingsRowMissing_ThrowsNotFound` | Fact | Cập nhật khi thiếu dòng cấu hình thì báo NotFound. |

### 6.2.9 Nhóm Tiện ích

Nhóm gồm 31 test: `CodeGeneratorTests` 5 test (trong đó 1 `[Theory]`); `DateTimeUtilsTests` 9 test (trong đó 1 `[Theory]`); `EmailTemplatesTests` 7 test; `EnumConvertersTests` 10 test (trong đó 3 `[Theory]`). Các test kiểm tra hàm sinh mã (mã booking, nội dung QR, mã giao dịch thanh toán), xử lý ngày giờ theo múi giờ Việt Nam, mẫu email HTML và các bộ chuyển đổi enum.

Bảng 6.10. Test đơn vị nhóm Tiện ích

| File | Tên test | Loại | Hành vi kiểm tra và kết quả mong đợi |
|---|---|---|---|
| CodeGeneratorTests | `GenerateBookingCode_StartsWithBKPrefixAndTodaysDate` | Fact | Mã booking bắt đầu bằng tiền tố `BK` và ngày hiện tại. |
| CodeGeneratorTests | `GenerateBookingCode_SuffixOnlyContainsUppercaseLettersAndDigits` | Fact | Phần đuôi mã booking chỉ gồm chữ hoa và chữ số. |
| CodeGeneratorTests | `GenerateBookingCode_IsHighlyUnlikelyToCollideAcrossManyCalls` | Fact | Nhiều lần sinh liên tiếp không cho mã trùng nhau. |
| CodeGeneratorTests | `GenerateTicketQrPayload_ConcatenatesBookingCodeAndSeat` | Theory | Nội dung QR của vé ghép mã booking và ghế. |
| CodeGeneratorTests | `GeneratePaymentTransactionId_StartsWithBookingCodeFollowedByUnixTimestamp` | Fact | Mã giao dịch thanh toán bắt đầu bằng mã booking, theo sau là mốc thời gian Unix. |
| DateTimeUtilsTests | `ParseTimeOnly_NullOrWhitespace_ReturnsNull` | Fact | Chuỗi `null` hoặc khoảng trắng trả `null`. |
| DateTimeUtilsTests | `ParseTimeOnly_ValidHHmm_ParsesCorrectly` | Fact | Chuỗi giờ `HH:mm` hợp lệ được phân tích đúng. |
| DateTimeUtilsTests | `ParseTimeOnly_InvalidFormat_Throws` | Fact | Chuỗi giờ sai định dạng bị ném lỗi. |
| DateTimeUtilsTests | `FormatDuration_FormatsMinutesOrSecondsBasedOnLength` | Theory | Khoảng thời gian được định dạng theo phút hoặc giây tùy độ dài. |
| DateTimeUtilsTests | `GetPeriodKey_Day_FormatsAsDdMmYyyy` | Fact | Khóa kỳ theo ngày có dạng `dd/MM/yyyy`. |
| DateTimeUtilsTests | `GetPeriodKey_Month_FormatsAsMmYyyy` | Fact | Khóa kỳ theo tháng có dạng `MM/yyyy`. |
| DateTimeUtilsTests | `GetPeriodKey_UnknownGroupBy_FallsBackToDayFormat` | Fact | Kiểu nhóm không xác định rơi về định dạng theo ngày. |
| DateTimeUtilsTests | `GetStartOfDay_ReturnsMidnightLocalDayAsUtc` | Fact | Đầu ngày là nửa đêm theo giờ địa phương, biểu diễn dưới dạng UTC. |
| DateTimeUtilsTests | `GetStartOfNextDay_ReturnsMidnightNextLocalDayAsUtc` | Fact | Đầu ngày kế tiếp là nửa đêm ngày sau theo giờ địa phương, biểu diễn dưới dạng UTC. |
| EmailTemplatesTests | `BookingConfirmed_UsesBrandOptions` | Fact | Email xác nhận đặt vé dùng thông tin thương hiệu từ cấu hình. |
| EmailTemplatesTests | `BookingConfirmed_DefaultBrand_OmitsSupportLine` | Fact | Với thương hiệu mặc định, email bỏ dòng hỗ trợ. |
| EmailTemplatesTests | `AllTemplates_FollowDesignSystemRules` | Fact | Mọi mẫu email tuân theo quy tắc hệ thống thiết kế. |
| EmailTemplatesTests | `OnlyBookingConfirmed_HasPerforation` | Fact | Chỉ mẫu xác nhận đặt vé có đường răng cưa. |
| EmailTemplatesTests | `BookingConfirmed_ShowsBookingCodeOnTicketStub` | Fact | Mẫu xác nhận hiển thị mã booking trên cuống vé. |
| EmailTemplatesTests | `Money_UsesDotThousandsSeparator_RegardlessOfCulture` | Fact | Số tiền dùng dấu chấm ngăn cách hàng nghìn, không phụ thuộc văn hóa hệ thống. |
| EmailTemplatesTests | `AllTemplates_EncodeCustomerName` | Fact | Mọi mẫu email mã hóa HTML tên khách hàng. |
| EnumConvertersTests | `ToDtoStatus_BookingStatus_MapsToExpectedString` | Theory | Trạng thái booking ánh xạ sang chuỗi kỳ vọng của DTO. |
| EnumConvertersTests | `ToDtoStatus_PaymentStatus_MapsToExpectedString` | Theory | Trạng thái thanh toán ánh xạ sang chuỗi kỳ vọng của DTO. |
| EnumConvertersTests | `ToDtoChannel_MapsToExpectedString` | Theory | Kênh đặt vé ánh xạ sang chuỗi kỳ vọng của DTO. |
| EnumConvertersTests | `ParseEnum_ValidValue_IsCaseInsensitive` | Fact | Phân tích enum không phân biệt hoa thường. |
| EnumConvertersTests | `ParseEnum_InvalidValue_ThrowsValidationAppExceptionWithFieldNamedError` | Fact | Giá trị không hợp lệ ném `ValidationAppException` với lỗi gắn đúng tên trường. |
| EnumConvertersTests | `ParseNullableEnum_NullOrWhitespace_ReturnsNull` | Fact | Giá trị rỗng hoặc `null` trả `null`. |
| EnumConvertersTests | `ParseNullableEnum_InvalidValue_ReturnsNullInsteadOfThrowing` | Fact | Giá trị không hợp lệ trả `null` thay vì ném lỗi. |
| EnumConvertersTests | `ParseNullableEnum_ValidValue_ReturnsParsedEnum` | Fact | Giá trị hợp lệ trả enum đã phân tích. |
| EnumConvertersTests | `TryParseEnum_ValidValue_ReturnsTrueAndParsedValue` | Fact | Giá trị hợp lệ trả `true` kèm giá trị đã phân tích. |
| EnumConvertersTests | `TryParseEnum_NullOrEmptyValue_ReturnsFalse` | Fact | Giá trị rỗng hoặc `null` trả `false`. |

### 6.2.10 Kết quả chạy test backend

Kết quả chạy được thu bằng `npm run test:unit` và `npm run test:integration` cho tổng số test, số đạt, lỗi, bỏ qua và thời gian; độ bao phủ thu bằng `npm run test:backend:coverage`. Tại thời điểm viết, chưa có log `dotnet test` hay báo cáo độ bao phủ trong tài liệu được cung cấp nên Bảng 6.11 giữ chỗ trống. Số test đếm từ mã nguồn là 364 (đơn vị) và 49 (tích hợp); số liệu này là số thuộc tính `[Fact]`/`[Theory]`, chưa tính số bộ dữ liệu của `[Theory]`.

Bảng 6.11. Kết quả chạy test backend

| Bộ test | Tổng | Đạt | Lỗi | Bỏ qua | Thời gian | Tỷ lệ đạt |
|---|---|---|---|---|---|---|
| Đơn vị (`Cinema.UnitTests`) | 399 | 399 | 0 | 0 | 26 giây | 100% |
| Tích hợp (`Cinema.IntegrationTests`) | 49 | 49 | 0 | 0 | 51 giây | 100% |
| **Toàn bộ hệ thống** | **448** | **448** | **0** | **0** | **77 giây** | **100% (0 lỗi, 0 bỏ qua)** |

## 6.3 KIỂM THỬ TÍCH HỢP

### 6.3.1 Hạ tầng kiểm thử tích hợp

Các test tích hợp nằm trong dự án `backend/tests/Cinema.IntegrationTests`, tham chiếu `Cinema.Api`, `Cinema.Application`, `Cinema.Infrastructure` và `Cinema.Domain`. Mỗi test gọi ứng dụng qua HTTP như một client thật, đi qua đầy đủ middleware, xác thực JWT, phân quyền, validator, dịch vụ và truy cập cơ sở dữ liệu PostgreSQL.

`CinemaApiFactory` kế thừa `WebApplicationFactory<Program>` và cài `IAsyncLifetime`. Nó khai báo một `PostgreSqlContainer` dựng bằng `PostgreSqlBuilder` với ảnh `postgres:16-alpine`, nên máy chạy test phải có Docker. Trong `ConfigureWebHost`, factory đặt môi trường `Testing`, truyền chuỗi kết nối của container qua `ConnectionStrings:DefaultConnection`, cấp khóa ký `Jwt:Secret` dành riêng cho test và nâng ba ngưỡng giới hạn tần suất lên 100000; các giá trị này được đặt vừa bằng `UseSetting` vừa bằng cấu hình trong bộ nhớ. Trong `ConfigureTestServices`, factory gỡ cấu hình `DbContextOptions<AppDbContext>` cũ và đăng ký lại `AppDbContext` với `UseNpgsql` trỏ vào container (giữ nguyên assembly chứa migration), thay `IEmailService` và `IGoogleAuthService` bằng các bản giả dạng singleton, thay `IPaymentGateway` bằng ba cổng giả tương ứng `PaymentMethodNames.VnPay`, `PaymentMethodNames.Momo`, `PaymentMethodNames.Stripe`, và gọi `AddHangfire` với bộ lưu trữ trong bộ nhớ (gói `Hangfire.InMemory`) vì `Program.cs` không đăng ký Hangfire ở môi trường `Testing`.

Khi khởi tạo (`InitializeAsync`), factory khởi động container, tạo một client để dựng host, chạy `MigrateAsync` rồi tạo `Respawner` với bộ điều hợp `DbAdapter.Postgres`. Respawn chỉ dọn 12 bảng khai báo trong `ResettableTables`: `Payments`, `BookingTickets`, `BookingCombos`, `Bookings`, `ShowtimeSeats`, `Showtimes`, `Reviews`, `RefreshTokens`, `UserVerificationTokens`, `CustomerProfiles`, `Employees` và `Users`. Dữ liệu nền không nằm trong danh sách này (như phòng, ghế, phim) được giữ lại, và `IntegrationTestDataSeeder.CreateScheduledShowtimeAsync` dựa vào dữ liệu đó. Phương thức `ResetDatabaseAsync` gọi Respawn rồi xóa email đã gửi và danh sách yêu cầu tạo thanh toán, hoàn tiền của ba cổng giả. Lớp `CinemaApiCollection` khai báo `ICollectionFixture<CinemaApiFactory>` với tên `Cinema API`, nhờ đó cả tám lớp test dùng chung một factory và một container, và xUnit chạy các test của một collection tuần tự.

`IntegrationTestBase` là lớp cơ sở của mọi lớp test, đánh dấu `[Collection(CinemaApiCollection.Name)]` và cài `IAsyncLifetime`: trước mỗi test gọi `Factory.ResetDatabaseAsync()` rồi tạo `HttpClient` mới. Lớp này cung cấp `ReadDataAsync<T>` (đọc trường `data` của phong bì phản hồi), `ReadEnvelopeAsync`, `LoginAsync` (gọi `/api/v1/auth/login` hoặc `/api/v1/auth/admin-login`), `RegisterConfirmedCustomerAndLoginAsync` và `CreateAuthenticatedClient` (gắn tiêu đề `Authorization: Bearer`). `IntegrationTestDataSeeder` ghi dữ liệu trực tiếp vào cơ sở dữ liệu bằng bốn hàm: `CreateConfirmedCustomerAsync` (khách hàng đã xác nhận email, có `CustomerProfile`), `CreateAdminUserWithoutEmployeeAsync` (tài khoản quản trị không có hồ sơ nhân viên), `CreateEmployeeAsync` và `CreateScheduledShowtimeAsync`. Hàm cuối chọn phòng đang hoạt động đầu tiên, tối đa bốn ghế đang hoạt động không phải lối đi và phim đầu tiên ở trạng thái `NowShowing`, rồi tạo suất `Scheduled` bắt đầu lúc 14 giờ UTC của ba ngày sau với giá cơ bản mặc định 100.000.

Ba lớp trong thư mục `Fakes` thay các dịch vụ ngoài. `FakeEmailService` ghi các email đã gửi vào `ConcurrentBag` để test đọc qua `SentEmails`. `FakeGoogleAuthService` trả `NextResult` do test đặt, và ném `InvalidOperationException` nếu chưa đặt. `FakePaymentGateway` nhận tên phương thức, trả `NextCreatePaymentResult` mặc định gồm URL `https://fake-gateway.test/pay` và mã giao dịch `FAKE-TXN`, trả `NextCallbackResult` do test đặt khi `VerifyCallback`, trả `NextRefundResult` mặc định thành công với mã `FAKE-REFUND-TXN`, đồng thời ghi lại mọi yêu cầu tạo thanh toán và hoàn tiền để test kiểm tra. Với SignalR, các test dùng gói `Microsoft.AspNetCore.SignalR.Client`: `HubConnectionBuilder` kết nối tới `/hubs/showtimes`, đặt `HttpMessageHandlerFactory` về `Factory.Server.CreateHandler()` để đi qua máy chủ kiểm thử trong bộ nhớ, truyền token bằng `AccessTokenProvider`, rồi gọi các phương thức hub `JoinShowtime` và `LeaveShowtime` và lắng nghe các sự kiện `seat.held`, `seat.released` (hằng số `SignalREvents.SeatHeld`, `SignalREvents.SeatReleased`).

### 6.3.2 Danh sách test tích hợp

Có 49 test tích hợp, phân bố trong tám file: `AdminBookingsEndpointsTests` 9 test; `AdminMoviesBulkEndpointsTests` 3 test; `AdminReviewsBulkEndpointsTests` 3 test; `AdminRoomsEndpointsTests` 8 test; `AdminShowtimesBulkEndpointsTests` 6 test; `AuthEndpointsTests` 11 test; `BookingFlowEndpointsTests` 6 test; `SignalRShowtimeHubTests` 3 test. Luồng cốt lõi của hệ thống được kiểm tra trong `BookingFlowEndpointsTests`: giữ hai ghế, thanh toán qua cổng VNPay giả, giả lập webhook IPN thành công và xác nhận booking `Confirmed` kèm hai vé; hai khách cùng giữ một ghế thì yêu cầu thứ hai nhận 409 kèm danh sách `conflictSeatIds`; phương thức thanh toán không được hỗ trợ bị validator trả 400; khách không phải chủ booking nhận 403; hủy booking `Pending` giải phóng ghế cho khách khác; và mười yêu cầu đồng thời cùng giữ một ghế chỉ có một yêu cầu thành công, chín yêu cầu còn lại nhận 409. `SignalRShowtimeHubTests` kiểm tra hub phát `seat.held` khi giữ ghế (payload không chứa `heldByUserId`), phát `seat.released` khi hủy booking và không gửi sự kiện cho client đã gọi `LeaveShowtime`. Các file còn lại kiểm tra API quản trị (giữ ghế và thanh toán tại quầy, soát vé, xóa hàng loạt phim và đánh giá, quản lý phòng và sơ đồ ghế, tạo và xóa hàng loạt suất chiếu) và xác thực, phân quyền theo vai trò.

Bảng 6.12. Test tích hợp theo tệp

| File | Tên test | Hành vi kiểm tra và kết quả mong đợi |
|---|---|---|
| AdminBookingsEndpointsTests | `CounterHold_WalkInCustomerNoAccount_CreatesPendingCounterBookingWithoutUserId` | Nhân viên giữ ghế quầy cho khách vãng lai không có tài khoản: 201 Created, `Channel` là `Counter`, `Status` là `Pending`, `FinalAmount` bằng giá hai ghế, `CreatedByEmployeeId` khác `null`. |
| AdminBookingsEndpointsTests | `CounterHold_CustomerCaller_Returns403Forbidden` | Tài khoản khách hàng gọi API giữ ghế quầy bị từ chối: 403 Forbidden. |
| AdminBookingsEndpointsTests | `CounterHold_CallerWithoutEmployeeRecord_Returns422` | Tài khoản quản trị không có hồ sơ nhân viên gọi giữ ghế quầy: 422 Unprocessable Entity. |
| AdminBookingsEndpointsTests | `CounterCheckout_CashAmountAboveTotal_ConfirmsBookingAndReturnsCorrectChange` | Thanh toán tiền mặt với số tiền nhận lớn hơn tổng: 200 OK, booking `Confirmed`, tiền thối 50.000. |
| AdminBookingsEndpointsTests | `CounterCheckout_InsufficientCashReceived_Returns422AndLeavesBookingPending` | Tiền nhận không đủ: 422 và booking vẫn `Pending`. |
| AdminBookingsEndpointsTests | `Checkin_ValidUncheckedTicketWithinWindow_MarksItCheckedIn` | Soát vé chưa dùng trong khung giờ cho phép: 200 OK, kết quả trả về đúng `BookingId`. |
| AdminBookingsEndpointsTests | `Checkin_AlreadyCheckedInTicket_Returns409` | Soát lại vé đã soát: 409 Conflict. |
| AdminBookingsEndpointsTests | `Checkin_UnknownQrCode_Returns404` | Mã QR không tồn tại: 404 Not Found. |
| AdminBookingsEndpointsTests | `Checkin_TicketForShowtimeMoreThan30MinutesAway_Returns422` | Vé của suất chiếu còn hơn 30 phút nữa mới bắt đầu: 422. |
| AdminMoviesBulkEndpointsTests | `BulkDelete_ExistingMoviesNoShowtimes_DeletesAllAndReturnsSuccess` | Xóa hàng loạt hai phim chưa có suất chiếu: 200 OK, `TotalSuccess` 2, `TotalFailed` 0, mọi kết quả thành công. |
| AdminMoviesBulkEndpointsTests | `BulkDelete_SomeNotFound_ReturnsPartialSuccess` | Xóa hàng loạt gồm một mã tồn tại và một mã không tồn tại: 200 OK, `TotalSuccess` 1, `TotalFailed` 1, kết quả từng mã đúng. |
| AdminMoviesBulkEndpointsTests | `BulkDelete_ExceedsLimit_ReturnsBadRequest` | Gửi 51 mã, vượt giới hạn mặc định `BulkDeleteLimit` = 50: 400 Bad Request. |
| AdminReviewsBulkEndpointsTests | `BulkDelete_ExistingReviews_DeletesAndRecalculatesRating` | Xóa hàng loạt đánh giá: 200 OK, `TotalSuccess` 1, `TotalFailed` 0, điểm trung bình của phim tính lại về 0. |
| AdminReviewsBulkEndpointsTests | `BulkDelete_SomeNotFound_ReturnsPartialSuccess` | Xóa hàng loạt đánh giá gồm một mã tồn tại và một mã không tồn tại: 200 OK, `TotalSuccess` 1, `TotalFailed` 1, kết quả từng mã đúng. |
| AdminReviewsBulkEndpointsTests | `BulkDelete_ExceedsLimit_ReturnsBadRequest` | Gửi 51 mã đánh giá, vượt giới hạn xóa hàng loạt: 400 Bad Request. |
| AdminRoomsEndpointsTests | `CreateRoom_ValidRequest_Returns201AndPersistsRoomWithZeroSeats` | Tạo phòng hợp lệ: 201 Created, phòng được lưu với `TotalSeats` 0 và `IsActive` là `true`. |
| AdminRoomsEndpointsTests | `CreateRoom_DuplicateName_Returns409` | Tạo phòng trùng tên: 409 Conflict. |
| AdminRoomsEndpointsTests | `UpdateSeatLayout_FreshRoom_CreatesAllNonAisleActiveSeatsAndComputesTotal` | Lưu sơ đồ ghế cho phòng mới gồm ba ô (một ô lối đi): 200 OK, `Created` 3, `TotalSeats` 2, truy vấn lại trả 3 bản ghi. |
| AdminRoomsEndpointsTests | `UpdateSeatLayout_RemovingASeatWithNoBookingHistory_DeletesIt` | Bỏ một ghế chưa có lịch sử đặt chỗ: ghế bị xóa, `Removed` 1, `TotalSeats` 1. |
| AdminRoomsEndpointsTests | `UpdateSeatLayout_RemovingASeatThatHasBookingHistory_DeactivatesInsteadOfDeleting` | Bỏ ghế đã có lịch sử đặt chỗ: `Removed` 0, `Deactivated` 1, ghế A2 còn tồn tại nhưng `IsActive` là `false`. |
| AdminRoomsEndpointsTests | `UpdateSeatLayout_CoupleSeatMissingItsPair_Returns400` | Ghế đôi thiếu ghế cặp: 400 Bad Request. |
| AdminRoomsEndpointsTests | `DeleteRoom_RoomWithNoShowtimes_HardDeletesIt` | Xóa phòng chưa có suất chiếu: 200 OK và truy vấn lại trả 404 (xóa hẳn). |
| AdminRoomsEndpointsTests | `DeleteRoom_RoomWithFutureScheduledShowtime_Returns422AndKeepsRoom` | Xóa phòng có suất chiếu tương lai đã lên lịch: 422 và phòng vẫn tồn tại (truy vấn lại trả 200). |
| AdminShowtimesBulkEndpointsTests | `BulkCreate_SingleMatchingDayAndTime_CreatesOneShowtimeWithAllRoomSeatsGenerated` | Tạo hàng loạt với một ngày và giờ khớp: 201 Created, `TotalCreated` 1, `TotalSkipped` 0, sinh 50 ghế (`SeatsGenerated`), giờ bắt đầu theo giờ Việt Nam là 10. |
| AdminShowtimesBulkEndpointsTests | `BulkCreate_DaysOfWeekNotMatchingDateRange_CreatesNothing` | Thứ trong tuần không khớp khoảng ngày: 201, `TotalCreated` 0, `TotalSkipped` 0, danh sách rỗng. |
| AdminShowtimesBulkEndpointsTests | `BulkCreate_SlotOverlapsAnAlreadyCreatedShowtime_SkipsTheDuplicateWithReason` | Lần tạo thứ hai trùng khung giờ với suất đã tạo: `TotalCreated` 0, `TotalSkipped` 1, lý do có chuỗi "Trùng khung giờ". |
| AdminShowtimesBulkEndpointsTests | `BulkCreate_NonExistentMovieId_Returns400` | Mã phim không tồn tại: 400, lỗi gắn với trường `movieId`. |
| AdminShowtimesBulkEndpointsTests | `BulkCreate_NonExistentRoomId_Returns400` | Mã phòng không tồn tại: 400, lỗi gắn với trường `roomId`. |
| AdminShowtimesBulkEndpointsTests | `BulkDelete_MixOfExistingAndNonExistentIds_ReturnsPartialSuccessForEach` | Xóa hàng loạt suất gồm mã tồn tại và không tồn tại: 200 OK, `TotalSuccess` 1, `TotalFailed` 1; suất tồn tại chuyển sang `Cancelled`. |
| AuthEndpointsTests | `Register_NewEmail_Returns201AndPersistsUnconfirmedUser` | Đăng ký email mới: 201 Created, `success` là `true`, người dùng được lưu ở trạng thái chưa xác nhận email. |
| AuthEndpointsTests | `Register_DuplicateEmail_Returns400WithEmailValidationError` | Đăng ký email trùng: 400, lỗi xác thực gắn với trường `email`. |
| AuthEndpointsTests | `Login_ConfirmedCustomerWithCorrectPassword_ReturnsAccessAndRefreshTokens` | Khách đã xác nhận đăng nhập đúng mật khẩu: 200 OK, có access token và refresh token, email khớp. |
| AuthEndpointsTests | `Login_WrongPassword_Returns401` | Sai mật khẩu: 401 Unauthorized. |
| AuthEndpointsTests | `Login_UnknownEmail_Returns401WithoutRevealingWhichPartWasWrong` | Email không tồn tại: 401 Unauthorized, không tiết lộ phần nào sai. |
| AuthEndpointsTests | `Me_WithoutBearerToken_Returns401` | Gọi `me` không có Bearer token: 401. |
| AuthEndpointsTests | `Me_WithValidBearerToken_ReturnsCurrentUser` | Gọi `me` với token hợp lệ: 200 OK, trả đúng `userId` hiện tại. |
| AuthEndpointsTests | `AdminUploads_WithCustomerToken_Returns403` | Khách hàng gọi API tải lên của quản trị: 403 Forbidden. |
| AuthEndpointsTests | `AdminUploads_WithoutToken_Returns401` | Gọi API tải lên của quản trị không có token: 401. |
| AuthEndpointsTests | `AdminShowtimesCreate_WithCustomerToken_Returns403` | Khách hàng gọi API tạo suất chiếu: 403 Forbidden. |
| AuthEndpointsTests | `AdminShowtimesCreate_WithAdminToken_Returns201` | Quản trị gọi API tạo suất chiếu: 201 Created. |
| BookingFlowEndpointsTests | `FullBookingJourney_HoldCheckoutWebhookConfirm_EndsWithConfirmedBookingAndTickets` | Luồng đầy đủ: giữ hai ghế (201, `Pending`, `FinalAmount` bằng giá hai ghế) → checkout VNPay (200, URL của cổng giả) → IPN thành công (200, `rspCode` là `00`) → chi tiết booking `Confirmed` kèm 2 vé. |
| BookingFlowEndpointsTests | `HoldSeats_SameSeatRequestedByTwoCustomers_SecondRequestReturns409WithConflictSeatIds` | Hai khách cùng giữ một ghế: yêu cầu thứ nhất 201, yêu cầu thứ hai 409 và `errors.conflictSeatIds` chứa ghế tranh chấp. |
| BookingFlowEndpointsTests | `Checkout_UnsupportedPaymentMethodName_Returns400FromValidator` | Checkout với phương thức "PayPal" không được hỗ trợ: 400 Bad Request từ validator. |
| BookingFlowEndpointsTests | `Checkout_ByNonOwner_Returns403` | Khách khác chủ booking gọi checkout: 403 Forbidden. |
| BookingFlowEndpointsTests | `CancelBooking_PendingBooking_ReturnsCancelledAndFreesSeatsForOthers` | Hủy booking `Pending`: 200 OK, trạng thái `Cancelled`; khách khác sau đó giữ được đúng ghế đó (201). |
| BookingFlowEndpointsTests | `HoldSeat_HighConcurrencyParallelRequests_OnlyOneWinsAndOthersGet409` | Mười khách đồng thời giữ cùng một ghế: đúng một yêu cầu 201, chín yêu cầu 409 và đều có ghế tranh chấp trong `conflictSeatIds`. |
| SignalRShowtimeHubTests | `HoldSeats_BroadcastsSeatHeldEventToClientsInShowtimeGroup` | Client đã `JoinShowtime` nhận sự kiện `seat.held` khi có người giữ ghế, payload có `showtimeId`, `heldUntil`, `seatIds` chứa ghế đã giữ và không có `heldByUserId`. |
| SignalRShowtimeHubTests | `CancelBooking_BroadcastsSeatReleasedEventToClientsInShowtimeGroup` | Client trong nhóm suất chiếu nhận sự kiện `seat.released` khi booking bị hủy, payload có `showtimeId` và `seatIds` chứa ghế được giải phóng. |
| SignalRShowtimeHubTests | `LeaveShowtime_DoesNotReceiveEventsAfterLeaving` | Sau `LeaveShowtime`, client không nhận sự kiện `seat.held` khi có người giữ ghế (chờ 1,5 giây). |

<!-- GHI CHÚ ĐỐI CHIẾU -->
<!--
(a) [CẦN BỔ SUNG] còn lại
- Bảng 6.1, cột "Kết quả chạy" của bộ Đơn vị, Tích hợp, E2E, Tải; Bảng 6.11 (toàn bộ): cần log `npm run test:unit`, `npm run test:integration` (cần Docker), `npm run test:backend:coverage`, log `npm run test:e2e` và kết quả k6. Không có `dotnet` trong môi trường làm việc và không có file log/trx/coverage trong project nên không chạy được.
- Hình 6.1: chưa có mã Mermaid/PlantUML trong các file mã sơ đồ đính kèm; chỉ có placeholder và mô tả văn bản. Cần vẽ từ số liệu ở Bảng 6.1.

(b) Chỗ outline lệch code
- Bảng 6.1, bộ Đơn vị: outline ghi "28 + 1" file và "283 + [đếm lại]" test. Code có 35 file `*Tests.cs` (đã gồm `LoyaltyWindowTests.cs`) và 364 test (đếm [Fact]/[Theory] bằng script). Có thêm 6 file hỗ trợ ở `TestSupport` (không tính). Tích hợp (8 file, 49 test) và E2E (7 spec, 8 test) khớp code.
- Số test từng file lệch outline: `AuthServiceTests` outline 24, code 28; `ShowtimeSchedulerTests` outline 7 (0 Theory), code 8 (1 Theory: `Heat_CalculatesHeatCorrectlyOn1To5RatingScale`). `LoyaltyWindowTests` code 19, toàn [Fact] (8 + 6 + 5). Các file còn lại có trong outline khớp số test và số [Theory].
- File test có trong code nhưng không có trong outline: `PaymentServiceMomoTests` (8, đưa vào 6.2.3), `SeatLayoutRulesTests` (19, đưa vào 6.2.6), `CinemaSettingsPolicyTests` (10), `SettingsServiceTests` (11, trong đó 2 Theory), `EmployeeServiceRoleRulesTests` (6), `ReviewValidatorsTests` (3, trong đó 2 Theory) (cả bốn đưa vào 6.2.8). Việc xếp nhóm cho các file này là do người viết quyết định, cần xác nhận.
- Tổng nhóm theo code so với outline: Xác thực 52 (outline 48); Đặt vé 88 (outline 69 trước khi thêm `LoyaltyWindowTests`; 69 + 19 = 88); Thanh toán 32 (outline 24); Hết hạn giữ ghế 7 (khớp); Suất chiếu 71 (outline 70); Phòng chiếu 27 (outline 8); Báo cáo 17 (khớp); Cấu hình, đánh giá, nhân viên 39 (outline 9); Tiện ích 31 (khớp). Tổng 364.
- Outline 6.3.2 chỉ nêu các luồng chính; code còn có test `HoldSeat_HighConcurrencyParallelRequests_OnlyOneWinsAndOthersGet409` (10 yêu cầu đồng thời) và các test API quản trị, đã đưa vào Bảng 6.12.
- Outline 6.1 yêu cầu "Môi trường Testing tắt Hangfire và rate limiter": đúng ở `Program.cs` (AddHangfire/AddHangfireServer, UseRateLimiter, MapHangfireDashboard, RecurringJob đều nằm trong `!IsEnvironment("Testing")`). Lưu ý `AddRateLimiter` vẫn được đăng ký, chỉ middleware `UseRateLimiter` bị bỏ qua; ngoài ra factory còn đặt các ngưỡng 100000.
- Bảng 6.1, hàng E2E: outline ghi "8/8 đạt (07/09/2026)" nhưng không có log kèm theo nên không đưa vào. Thư mục `test-results/` trong project còn một `error-context.md` của lần chạy lỗi `admin-checkin-flow.spec.ts` ("Cannot navigate to invalid URL" khi `page.goto("/admin/login")`), cần đối chiếu với kết luận 8/8 khi viết Mục 6.4.
- Bảng 6.1, hàng Tải: outline ghi "load-tests/ chưa có"; code có thư mục `load-tests/` với hai kịch bản k6 (`browse-movies-stress.js`, `peak-ticket-rush.js`), thư mục `load-tests/results` rỗng.
- Việc mô tả "đã sửa theo quy tắc mới" ở 6.2.2 chỉ dựa trên nội dung hiện tại của test, không có lịch sử thay đổi để đối chiếu.

(c) Rủi ro cần kiểm chứng khi chạy test tích hợp (không phải kết luận)
- `ResettableTables` trong `CinemaApiFactory` không có bảng `LoyaltyPointEntries`, trong khi `LoyaltyPointEntryConfiguration` đặt khóa ngoại `BookingId` với `DeleteBehavior.Restrict` (khóa ngoại `UserId` là Cascade). Test `FullBookingJourney_...` xác nhận booking và theo test đơn vị sẽ tạo dòng sổ điểm; nếu có dòng này, lần Respawn kế tiếp xóa `Bookings` có thể vi phạm khóa ngoại. Chưa chạy được nên chưa xác minh; cần chạy `test:integration` để kiểm tra.
-->

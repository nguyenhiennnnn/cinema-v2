## 4.4 XÁC THỰC

Hệ thống Perfo xác thực người dùng bằng hai loại thông tin phiên: access token là JSON Web Token (JWT) có thời hạn ngắn, dùng để gọi API, và refresh token là chuỗi ngẫu nhiên có thời hạn dài, chỉ dùng để xin cặp token mới. Toàn bộ nghiệp vụ nằm trong lớp `AuthService` (`Cinema.Application/Services/Auth/AuthService.cs`), phối hợp với `JwtTokenService` (phát hành và băm token), `GoogleAuthService` (xác minh ID token của Google) và các tùy chọn cấu hình `JwtOptions`, `EmailOptions`, `AppUrlOptions`. Các endpoint tương ứng được khai báo trong `AuthController` (`api/v1/auth`). Mục này lần lượt trình bày sáu luồng (đăng ký, đăng nhập, đăng nhập Google, làm mới token, phát hiện dùng lại token, khôi phục mật khẩu), sau đó so sánh các loại token (Mục 4.4.7) và tổng hợp phân quyền theo controller (Mục 4.4.8).

### 4.4.1 Luồng đăng ký tài khoản

Luồng đăng ký gồm hai bước: tạo tài khoản ở trạng thái chưa xác nhận email, rồi xác nhận email bằng mã gửi qua thư. Endpoint `POST api/v1/auth/register` được gắn chính sách giới hạn tần suất `auth` và trả về mã trạng thái 201.

Trong `RegisterAsync`, hệ thống trước hết kiểm tra email đã tồn tại trong bảng `Users` hay chưa; nếu có, phương thức ném `ValidationAppException` với thông điệp `Messages.Invalid_EmailExists` và lỗi theo trường `email`. Tiếp theo, hệ thống tìm vai trò có tên `Roles.Customer` ("Customer"); nếu dữ liệu khởi tạo chưa có vai trò này thì ném `BusinessRuleException` với `Messages.Rule_CustomerRoleNotSeeded`. Đối tượng `User` mới được tạo với `IsActive = true`, `EmailConfirmed = false`, `CreatedAt = DateTime.UtcNow`; mật khẩu được băm bằng `IPasswordHasher<User>` (được đăng ký là `PasswordHasher<User>` trong `Cinema.Application/DependencyInjection.cs`) nên cơ sở dữ liệu không lưu mật khẩu gốc. Cùng với `User`, hệ thống tạo `CustomerProfile` với `Points = 0` và `TotalSpent = 0`, rồi lưu cả hai trong một lần gọi `SaveChangesAsync`.

Sau khi lưu tài khoản, `RegisterAsync` gọi phương thức dùng chung `IssueVerificationTokenAndSendEmailAsync` với mục đích `VerificationTokenPurpose.EmailConfirmation` và thời hạn `EmailOptions.EmailConfirmationExpirationHours` giờ (mặc định 24). Phương thức này sinh mã bằng `GenerateOpaqueToken()`, chỉ lưu giá trị băm SHA-256 vào bảng `UserVerificationTokens` (các cột `TokenHash`, `Purpose`, `ExpiresAt`, `Used = false`, `CreatedAt`), dựng liên kết dạng `{App:FrontendBaseUrl}/auth/confirm-email?token={mã}` (mã được mã hóa URL), dựng nội dung thư bằng `EmailTemplates.ActionLink`, rồi xếp job `SendEmailJob` vào hàng đợi Hangfire thông qua `IBackgroundJobClient.Enqueue`. Như vậy việc gửi thư không chặn yêu cầu đăng ký. Phản hồi gồm `UserId`, `Email` và thông điệp yêu cầu kiểm tra email.

Khi khách bấm liên kết, giao diện gọi `POST api/v1/auth/confirm-email`. `ConfirmEmailAsync` băm mã nhận được, tìm bản ghi `UserVerificationToken` có cùng `TokenHash` và `Purpose = EmailConfirmation`, kèm `User`. Nếu không có bản ghi, hoặc `Used = true`, hoặc `ExpiresAt <= DateTime.UtcNow`, phương thức ném `ValidationAppException` với `Messages.Invalid_TokenExpiredOrUsed`. Ngược lại, hệ thống đặt `Used = true`, đặt `EmailConfirmed = true` (kèm `UpdatedAt`) nếu tài khoản chưa xác nhận, rồi lưu thay đổi và trả `Messages.Success_EmailConfirmed`. Mã xác nhận do đó chỉ dùng được một lần. Trường hợp khách không nhận được thư, `ResendConfirmationEmailAsync` (`POST api/v1/auth/resend-confirmation`) đánh dấu `Used = true` cho các mã xác nhận còn hiệu lực rồi cấp mã mới; khi email không tồn tại hoặc đã xác nhận, phương thức trả cùng thông điệp chuẩn `Messages.Success_ConfirmationEmailSent` mà không phát hành mã.

![Hình 4.41. Tuần tự đăng ký tài khoản](hinh/4.41.png)

*Hình 4.41. Tuần tự đăng ký tài khoản*

### 4.4.2 Luồng đăng nhập

Đăng nhập bằng email dùng chung phương thức `LoginAsync(request, adminOnly, cancellationToken)` cho hai endpoint: `POST api/v1/auth/login` truyền `adminOnly: false` (cổng khách hàng) và `POST api/v1/auth/admin-login` truyền `adminOnly: true` (cổng quản trị). Cả hai endpoint đều chịu giới hạn tần suất `auth` (mặc định 5 yêu cầu mỗi 60 giây cho mỗi địa chỉ IP, theo `RateLimitOptions.AuthPermitLimit` và `AuthWindowSeconds`).

Các bước trong `LoginAsync` được thực hiện theo thứ tự sau. Hệ thống truy vấn `User` theo email, nạp kèm `Role`, `Employee` và `CustomerProfile` cùng hạng thành viên (`Tier`). Nếu không tìm thấy người dùng, phương thức ném `UnauthorizedAppException` với `Messages.Unauthorized_InvalidCredentials`. Mật khẩu được kiểm tra bằng `VerifyHashedPassword`; kết quả `Failed` cũng ném đúng thông điệp trên, nên phản hồi không phân biệt "sai email" với "sai mật khẩu". Chỉ sau khi mật khẩu đúng, hệ thống mới xét trạng thái tài khoản: `IsActive = false` ném `ForbiddenAppException` với `Messages.Forbidden_AccountLocked`; `EmailConfirmed = false` ném `ForbiddenAppException` với `Messages.Forbidden_EmailNotVerifiedCheckInbox`.

Bước tiếp theo là kiểm tra cổng đăng nhập theo vai trò. Với `isCustomer = (Role.RoleName == Roles.Customer)`, nếu đăng nhập ở cổng khách hàng (`adminOnly = false`) mà tài khoản không phải khách hàng thì ném `Messages.Forbidden_UseAdminPortal`; nếu đăng nhập ở cổng quản trị (`adminOnly = true`) mà tài khoản là khách hàng thì ném `Messages.Forbidden_CustomerCannotUseAdminPortal`. Cổng quản trị vì vậy dành cho cả vai trò Admin lẫn Employee, còn quyền thao tác cụ thể do các policy ở Mục 4.4.8 quyết định. Nếu kết quả kiểm tra mật khẩu là `SuccessRehashNeeded` (thuật toán băm đã lỗi thời), hệ thống băm lại mật khẩu bằng cùng giá trị người dùng vừa nhập và lưu lại.

Khi mọi điều kiện đạt, `JwtTokenService.GenerateAccessToken` phát hành access token, và `IssueRefreshTokenAsync` sinh refresh token ngẫu nhiên, lưu giá trị băm SHA-256 cùng `ExpiresAt = UtcNow + RefreshTokenExpirationDays` ngày vào bảng `RefreshTokens`. Cuối cùng, `ToLoginUserDtoAsync` dựng thông tin người dùng trả về; với khách hàng, điểm và hạng thành viên được tính từ sổ điểm trong cửa sổ trượt 12 tháng bằng `LoyaltyWindow.GetStandingAsync`, không lấy bản lưu đệm trên `CustomerProfile`. Phản hồi `LoginResponse` gồm access token, refresh token (giá trị gốc, chỉ xuất hiện ở thời điểm này), thời hạn access token (`ExpiresIn`, tính bằng giây) và thông tin người dùng.

Access token là JWT ký bằng HMAC-SHA256 (`SecurityAlgorithms.HmacSha256`) với khóa `Jwt:Secret`, `Issuer` mặc định `Perfo` và `Audience` mặc định `Perfo.Client`. Các claim do `BuildBaseClaims` đặt gồm `uid` (mã người dùng), `role` và `ClaimTypes.Role` (tên vai trò), `sub`, `email`, và `eid` (mã nhân viên) khi người dùng có hồ sơ `Employee`. Ở phía máy chủ, `Program.cs` cấu hình JWT Bearer kiểm tra issuer, audience, chữ ký và thời hạn với `ClockSkew` 30 giây; ngoài ra sự kiện `OnTokenValidated` truy vấn cơ sở dữ liệu ở mỗi yêu cầu để từ chối token của tài khoản đã bị khóa (`IsActive = false`), dù access token chưa hết hạn.

![Hình 4.42. Tuần tự đăng nhập bằng email](hinh/4.42.png)

*Hình 4.42. Tuần tự đăng nhập bằng email*

### 4.4.3 Luồng đăng nhập Google

Endpoint `POST api/v1/auth/google` (giới hạn tần suất `auth`) nhận `GoogleLoginRequest` chứa `IdToken` do Google cấp cho giao diện. `AuthService.GoogleLoginAsync` gọi `IGoogleAuthService.VerifyIdTokenAsync`, được hiện thực bởi `GoogleAuthService` ở lớp Infrastructure. Phương thức này trước hết kiểm tra `Google:ClientId` đã được cấu hình; nếu chuỗi rỗng, nó ném `BusinessRuleException` với `Messages.Rule_GoogleOAuthNotConfigured`. Sau đó, thư viện `Google.Apis.Auth` xác minh ID token qua `GoogleJsonWebSignature.ValidateAsync` với `Audience` bắt buộc là `ClientId` của hệ thống; nếu token không hợp lệ (`InvalidJwtException`), hệ thống ghi log cảnh báo và ném `UnauthorizedAppException` với `Messages.Unauthorized_GoogleTokenInvalid`. Kết quả hợp lệ được gói trong `GoogleUserInfo` gồm mã Google (`Subject`), email, cờ `EmailVerified`, tên và ảnh đại diện.

Quay lại `AuthService`, nếu `EmailVerified` là sai thì phương thức ném `UnauthorizedAppException` với `Messages.Unauthorized_GoogleEmailNotVerified`. Nếu đạt, hệ thống tìm `User` thỏa `GoogleId` trùng mã Google hoặc `Email` trùng email Google. Có hai nhánh. Khi không tìm thấy, hệ thống tạo tài khoản khách hàng mới: `FullName` lấy từ tên Google (hoặc email nếu không có tên), `GoogleId`, `Avatar` lấy từ ảnh Google, `IsActive = true`, `EmailConfirmed = true` vì Google đã xác minh email; mật khẩu được đặt bằng bản băm của một chuỗi ngẫu nhiên sinh từ `GenerateOpaqueToken()`, nên tài khoản này không thể đăng nhập bằng mật khẩu cho đến khi người dùng đặt lại mật khẩu. Hệ thống cũng tạo `CustomerProfile` với `Points = 0`, `TotalSpent = 0`, lưu, rồi nạp lại `User` kèm các quan hệ cần thiết.

Khi đã có tài khoản, hệ thống kiểm tra tài khoản bị khóa (`Messages.Forbidden_AccountLocked`), rồi chặn mọi tài khoản không phải khách hàng bằng `ForbiddenAppException` với `Messages.Forbidden_AdminCannotUseGoogleLogin`; vì vậy quản trị viên và nhân viên không thể đăng nhập bằng Google. Nếu đạt, hệ thống liên kết `GoogleId` khi tài khoản chưa có, đặt `EmailConfirmed = true` khi chưa xác nhận, và chỉ lưu khi có thay đổi. Hai nhánh cùng kết thúc giống `LoginAsync`: phát hành access token, phát hành và lưu refresh token, dựng `LoginUserDto` và trả `LoginResponse`.

![Hình 4.43. Tuần tự đăng nhập bằng Google](hinh/4.43.png)

*Hình 4.43. Tuần tự đăng nhập bằng Google*

### 4.4.4 Làm mới token xoay vòng

Access token có thời hạn `Jwt:AccessTokenExpirationSeconds` = 3600 giây, còn refresh token có thời hạn `Jwt:RefreshTokenExpirationDays` = 30 ngày. Khi access token hết hạn, giao diện gọi `POST api/v1/auth/refresh` (giới hạn tần suất `auth`) với refresh token hiện có. Cơ chế "xoay vòng" nghĩa là mỗi refresh token chỉ dùng được một lần: mỗi lần làm mới hệ thống cấp một refresh token mới và vô hiệu hóa token cũ.

Trong `RefreshAsync`, hệ thống băm refresh token nhận được bằng `HashOpaqueToken` (SHA-256) và tìm bản ghi `RefreshToken` có `TokenHash` tương ứng, nạp kèm `User`, `Role`, `Employee` và hạng thành viên. Các điều kiện được kiểm tra theo thứ tự: không tìm thấy bản ghi thì ném `UnauthorizedAppException` với `Messages.Unauthorized_RefreshTokenInvalid`; bản ghi đã bị thu hồi (`IsRevoked`) thì xử lý phát hiện dùng lại (Mục 4.4.5) và ném `Messages.Unauthorized_RefreshTokenExpiredOrRevoked`; `ExpiresAt <= DateTime.UtcNow` thì ném cùng thông điệp hết hạn hoặc bị thu hồi; tài khoản bị khóa thì ném `ForbiddenAppException` với `Messages.Forbidden_AccountLocked`; email chưa xác nhận thì ném `Messages.Forbidden_EmailNotVerified`.

Khi mọi điều kiện đạt, hệ thống phát hành access token mới cho `existing.User`, sinh giá trị refresh token mới và dựng thực thể `RefreshToken` mới (băm SHA-256, `ExpiresAt` = hiện tại cộng 30 ngày). Việc ghi cơ sở dữ liệu được thực hiện trong một giao dịch (`BeginTransactionAsync`): thêm token mới, đặt `IsRevoked = true` và `RevokedAt` cho token cũ, gọi `SaveChangesAsync` để cơ sở dữ liệu sinh `RefreshTokenId` của token mới; sau đó gán `existing.ReplacedByTokenId = newRefreshTokenEntity.RefreshTokenId`, lưu lần nữa và `CommitAsync`. Cột `ReplacedByTokenId` vì thế nối các refresh token của một phiên thành một chuỗi theo thứ tự cấp phát, là cơ sở cho việc phát hiện dùng lại ở Mục 4.4.5. Phản hồi `RefreshResponse` gồm access token mới, refresh token mới (giá trị gốc), thông tin người dùng và `ExpiresIn`.

Ở phía client, `frontend/src/api/axios.ts` dùng một interceptor phản hồi để làm mới tự động. Access token được giữ trong bộ nhớ của store `useAuthStore` và gắn vào tiêu đề `Authorization: Bearer` bởi interceptor yêu cầu; refresh token được lưu trong `localStorage` dưới khóa `BRAND.storageKeys.refreshToken` (giá trị `perfo_refresh_token`). Khi một yêu cầu nhận mã 401, chưa được thử lại (`_retry`) và địa chỉ không thuộc danh sách loại trừ `AUTH_EXCLUDED_URLS` (các endpoint `/auth/...` như đăng nhập, đăng ký, làm mới, đăng xuất), interceptor gọi `refreshAccessToken()`. Hàm này dùng biến mô-đun `refreshPromise` làm promise dùng chung: nếu đã có một lần làm mới đang chạy, các yêu cầu 401 đồng thời cùng chờ promise đó thay vì mỗi yêu cầu gửi một lần làm mới riêng. Điều này quan trọng vì refresh token chỉ dùng được một lần; nếu hai yêu cầu cùng gửi một refresh token, yêu cầu thứ hai sẽ bị xem là dùng lại token đã thu hồi (Mục 4.4.5). Khi làm mới thành công, client lưu refresh token mới, cập nhật access token trong store rồi gửi lại yêu cầu ban đầu với token mới. Khi làm mới thất bại do không nhận được phản hồi, hoặc nhận 401 hoặc 403, client đăng xuất và chuyển hướng về trang đăng nhập; với các lỗi khác (ví dụ lỗi máy chủ tạm thời) client giữ nguyên phiên.

![Hình 4.44. Tuần tự làm mới token xoay vòng](hinh/4.44.png)

*Hình 4.44. Tuần tự làm mới token xoay vòng*

### 4.4.5 Phát hiện dùng lại token

Vì refresh token chỉ có giá trị một lần, việc xuất hiện một refresh token đã bị thu hồi là dấu hiệu token bị sao chép hoặc bị dùng song song. `RefreshAsync` xử lý tình huống này ngay sau khi tìm thấy bản ghi: nếu `existing.IsRevoked` là đúng và `existing.ReplacedByTokenId` khác `null`, phương thức gọi `RevokeTokenChainAsync(existing.ReplacedByTokenId, ...)` rồi ném `UnauthorizedAppException` với `Messages.Unauthorized_RefreshTokenExpiredOrRevoked`. Yêu cầu dùng lại token luôn bị từ chối, bất kể chuỗi có được thu hồi hay không.

`RevokeTokenChainAsync` đi theo chuỗi tiến về phía các token được cấp sau. Phương thức bắt đầu từ `RefreshTokenId` của token thay thế, tải từng bản ghi; nếu bản ghi chưa bị thu hồi thì đặt `IsRevoked = true` và `RevokedAt` bằng cùng một mốc thời gian, rồi chuyển sang `ReplacedByTokenId` của bản ghi đó, cho đến khi hết chuỗi hoặc không tìm thấy bản ghi. Một tập `visited` (`HashSet<int>`) ghi lại các mã đã duyệt để tránh vòng lặp vô hạn nếu dữ liệu có chu trình. Sau vòng lặp, phương thức lưu thay đổi một lần và ghi log mức cảnh báo `RefreshTokenReuseDetected`. Kết quả là token hiện hành mà bên dùng hợp lệ (hoặc kẻ tấn công) đang giữ cũng bị vô hiệu, buộc cả hai phải đăng nhập lại; đây là cách chấp nhận bắt buộc đăng nhập lại để chặn phiên có nguy cơ bị chiếm.

Ngoài cơ chế chuỗi, hai thao tác đổi mật khẩu cũng thu hồi token. `ChangePasswordAsync` (sau khi kiểm tra mật khẩu hiện tại) và `ResetPasswordAsync` (Mục 4.4.6) đều truy vấn mọi `RefreshToken` của người dùng có `IsRevoked = false`, đặt `IsRevoked = true` và `RevokedAt`, rồi ghi log số token đã thu hồi. Riêng `LogoutAsync` chỉ thu hồi đúng refresh token được gửi lên, không thu hồi chuỗi. Access token là JWT không lưu trong cơ sở dữ liệu nên không thể thu hồi từng chiếc; sau khi đổi hoặc đặt lại mật khẩu, các access token đã phát hành vẫn hợp lệ đến khi hết hạn (tối đa 3600 giây), trừ trường hợp tài khoản bị khóa như đã nêu ở Mục 4.4.2.

![Hình 4.45. Lưu đồ phát hiện dùng lại refresh token](hinh/4.45.png)

*Hình 4.45. Lưu đồ phát hiện dùng lại refresh token*

### 4.4.6 Khôi phục mật khẩu

Khôi phục mật khẩu gồm hai endpoint công khai, cùng chịu giới hạn tần suất `auth`: `POST api/v1/auth/forgot-password` (yêu cầu) và `POST api/v1/auth/reset-password` (đặt lại).

`ForgotPasswordAsync` luôn trả về cùng một thông điệp chuẩn `Messages.Success_PasswordResetEmailSent`, dù email có tồn tại hay không, để không lộ thông tin tài khoản. Nếu không tìm thấy người dùng theo email, phương thức chỉ ghi log mức thông tin và trả thông điệp chuẩn mà không phát hành mã. Nếu tìm thấy, hệ thống trước hết vô hiệu hóa các mã `PasswordReset` còn hiệu lực của người dùng đó bằng cách đặt `Used = true`, nên tại mỗi thời điểm chỉ có mã mới nhất dùng được. Sau đó phương thức dùng chung `IssueVerificationTokenAndSendEmailAsync` sinh mã ngẫu nhiên, lưu bản băm SHA-256 với `Purpose = PasswordReset` và thời hạn `EmailOptions.PasswordResetExpirationMinutes` phút (mặc định 30), dựng liên kết `{App:FrontendBaseUrl}/reset-password?token={mã}` và xếp job `SendEmailJob` vào hàng đợi Hangfire, giống luồng đăng ký.

`ResetPasswordAsync` băm mã trong yêu cầu rồi tìm bản ghi `UserVerificationToken` có `Purpose = PasswordReset`. Nếu không có bản ghi, mã đã dùng (`Used`) hoặc đã hết hạn (`ExpiresAt <= DateTime.UtcNow`), phương thức ném `ValidationAppException` với `Messages.Invalid_TokenExpiredOrUsed`. Nếu hợp lệ, hệ thống đặt `Used = true`, băm mật khẩu mới bằng `IPasswordHasher<User>` và gán vào `PasswordHash` (kèm `UpdatedAt`), thu hồi mọi refresh token chưa thu hồi của người dùng (xem Mục 4.4.5), lưu thay đổi và trả `Messages.Success_PasswordReset`. Như vậy mã đặt lại mật khẩu có hạn, chỉ dùng được một lần, và việc đặt lại mật khẩu kết thúc mọi phiên đăng nhập đang có.

![Hình 4.46. Tuần tự khôi phục mật khẩu](hinh/4.46.png)

*Hình 4.46. Tuần tự khôi phục mật khẩu*

### 4.4.7 Các loại token

Hệ thống sử dụng năm loại token với thời hạn, cách lưu trữ và mục đích khác nhau, tổng hợp ở Bảng 4.79. Hai loại là JWT ký bằng khóa `Jwt:Secret` (access token và socket token) nên không được lưu trong cơ sở dữ liệu. Ba loại còn lại là chuỗi ngẫu nhiên 32 byte do `RandomNumberGenerator.GetBytes(32)` sinh ra, mã hóa Base64 theo dạng an toàn cho URL (thay `+` bằng `-`, `/` bằng `_`, bỏ ký tự đệm `=`); cơ sở dữ liệu chỉ lưu giá trị băm SHA-256 của chúng (chuỗi hex 64 ký tự do `Convert.ToHexString` tạo ra), nên người có quyền đọc cơ sở dữ liệu cũng không khôi phục được giá trị gốc để sử dụng. Socket token được `AuthController.GenerateSocketToken` (`POST api/v1/auth/socket-token`, yêu cầu đăng nhập) phát hành với cùng tập claim như access token nhưng thời hạn ngắn hơn, dùng để kết nối SignalR.

**Bảng 4.79. Các loại token**

| Token | Thời hạn | Lưu trữ | Mục đích | Ý nghĩa |
|---|---|---|---|---|
| Access token | 3600 giây (`Jwt:AccessTokenExpirationSeconds`) | Không lưu (JWT, HMAC-SHA256); client giữ trong bộ nhớ (`useAuthStore`) | Gọi API | Mang claim `uid`, `role`, `sub`, `email`, `eid` (nếu là nhân viên); được xác thực ở mỗi yêu cầu và bị từ chối khi tài khoản bị khóa |
| Refresh token | 30 ngày (`Jwt:RefreshTokenExpirationDays`) | CSDL bảng `RefreshTokens`, dạng SHA-256; client giữ trong `localStorage` (`perfo_refresh_token`) | Làm mới phiên | Dùng một lần; xoay vòng qua `ReplacedByTokenId`; bị thu hồi khi đăng xuất, đổi/đặt lại mật khẩu hoặc phát hiện dùng lại |
| Socket token | 300 giây (`Jwt:SocketTokenExpirationSeconds`) | Không lưu (JWT) | Kết nối SignalR | Cùng tập claim với access token; hub `/hubs/showtimes` yêu cầu đăng nhập và đọc token từ tham số `access_token` trên đường dẫn `/hubs` |
| Mã xác nhận email | 24 giờ (`Email:EmailConfirmationExpirationHours`) | CSDL bảng `UserVerificationTokens`, dạng SHA-256 | `VerificationTokenPurpose.EmailConfirmation` | Dùng một lần (`Used`); cấp lại thì mã cũ bị vô hiệu |
| Mã đặt lại mật khẩu | 30 phút (`Email:PasswordResetExpirationMinutes`) | CSDL bảng `UserVerificationTokens`, dạng SHA-256 | `VerificationTokenPurpose.PasswordReset` | Dùng một lần (`Used`); yêu cầu mới thì mã cũ bị vô hiệu |

*Ghi chú: giá trị thời hạn là giá trị mặc định trong `JwtOptions` và `EmailOptions`, trùng với `appsettings.json`; có thể ghi đè bằng cấu hình môi trường.*

### 4.4.8 Ma trận phân quyền

Việc phân quyền dựa trên vai trò (`Roles.Customer`, `Roles.Admin`, `Roles.Employee`) nằm trong claim của JWT và được cấu hình trong `Program.cs` bằng `AddAuthorizationBuilder`. Hệ thống khai báo bốn policy trong lớp hằng `Policies`: `CustomerOnly` (chỉ vai trò Customer), `AdminOnly` (chỉ vai trò Admin), `EmployeeOnly` (chỉ vai trò Employee) và `StaffOnly` (vai trò Admin hoặc Employee). Trong các controller hiện tại không endpoint nào dùng `EmployeeOnly`, nên policy này không xuất hiện trong ma trận. Middleware được xếp theo thứ tự `UseCors`, `UseRateLimiter`, `UseAuthentication`, `UseAuthorization`, sau đó `MapControllers`.

Bảng 4.80 thống kê số endpoint của từng controller theo mức bảo vệ, tính theo từng action HTTP (`[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpPatch]`, `[HttpDelete]`) với quy tắc sau. "Công khai" là endpoint không có `[Authorize]` ở cấp lớp lẫn cấp phương thức. "Đã đăng nhập" là endpoint chỉ yêu cầu `[Authorize]` không kèm policy (mọi vai trò đã xác thực). Bốn cột còn lại ứng với policy được gắn trực tiếp ở phương thức hoặc ở lớp. Với các controller quản trị, lớp mang `[Authorize]` (hoặc trực tiếp một policy) và từng phương thức được thu hẹp bằng `Policies.AdminOnly` hoặc `Policies.StaffOnly`; cột được tính theo policy ở cấp phương thức. Có 33 controller (22 controller quản trị, 9 controller phía khách hàng, 2 controller khác) với tổng cộng 137 endpoint: 84 endpoint chỉ dành cho Admin, 9 endpoint dành cho Admin hoặc Employee, 3 endpoint chỉ dành cho khách hàng, 11 endpoint chỉ cần đăng nhập và 30 endpoint công khai.

Các điểm đáng chú ý như sau. Nhóm `Admin/*` hầu hết thuộc `AdminOnly`; chỉ `AdminBookings` (5 endpoint), 1 endpoint của `AdminMovies`, 2 của `AdminShowtimes` và 1 của `AdminUsers` dùng `StaffOnly`. Ba endpoint `CustomerOnly` đều nằm trong `Client/Movies`. Trong `Client/Auth`, hai endpoint cần đăng nhập là `socket-token` và `me`; mười endpoint còn lại là công khai, trong đó `logout`, `confirm-email` và `resend-confirmation` không có `[Authorize]`, còn bảy endpoint khác (`register`, `login`, `admin-login`, `google`, `refresh`, `forgot-password`, `reset-password`) gắn thêm chính sách giới hạn tần suất `auth`. `Client/Bookings` và `Client/Users` yêu cầu đăng nhập ở cấp lớp. `Khác/Payments` gồm bốn endpoint công khai nhận callback từ cổng thanh toán (`vnpay/ipn` ở hai phương thức GET và POST, `momo/ipn`, `stripe/webhook`); các endpoint này không dùng JWT nên việc xác thực nguồn gọi không thuộc cơ chế phân quyền của mục này.

Ngoài các controller, hai điểm vào khác cũng có kiểm soát truy cập: hub SignalR `/hubs/showtimes` mang `[Authorize]`, và bảng điều khiển Hangfire tại `/admin/hangfire` dùng `HangfireAdminAuthorizationFilter`, cho phép mọi truy cập khi môi trường là Development và chỉ cho người dùng đã xác thực có vai trò Admin ở các môi trường còn lại. Hai điểm vào này không được tính vào Bảng 4.80.

**Bảng 4.80. Ma trận phân quyền theo controller**

| Controller | Công khai | Đã đăng nhập | CustomerOnly | StaffOnly | AdminOnly | Tổng |
|---|---|---|---|---|---|---|
| Admin/AdminActors |  |  |  |  | 5 | 5 |
| Admin/AdminAgeRatings |  |  |  |  | 4 | 4 |
| Admin/AdminBookings |  |  |  | 5 |  | 5 |
| Admin/AdminCombos |  |  |  |  | 5 | 5 |
| Admin/AdminDashboard |  |  |  |  | 2 | 2 |
| Admin/AdminDirectors |  |  |  |  | 5 | 5 |
| Admin/AdminEmployees |  |  |  |  | 3 | 3 |
| Admin/AdminGenres |  |  |  |  | 4 | 4 |
| Admin/AdminLanguages |  |  |  |  | 4 | 4 |
| Admin/AdminMembershipTiers |  |  |  |  | 4 | 4 |
| Admin/AdminMovies |  |  |  | 1 | 4 | 5 |
| Admin/AdminPayments |  |  |  |  | 2 | 2 |
| Admin/AdminPositions |  |  |  |  | 4 | 4 |
| Admin/AdminReports |  |  |  |  | 3 | 3 |
| Admin/AdminReviews |  |  |  |  | 3 | 3 |
| Admin/AdminRooms |  |  |  |  | 7 | 7 |
| Admin/AdminScreenTypes |  |  |  |  | 4 | 4 |
| Admin/AdminSeatTypes |  |  |  |  | 4 | 4 |
| Admin/AdminSettings |  |  |  |  | 2 | 2 |
| Admin/AdminShowtimes |  |  |  | 2 | 8 | 10 |
| Admin/AdminUploads |  |  |  |  | 2 | 2 |
| Admin/AdminUsers |  |  |  | 1 | 5 | 6 |
| Client/Ai | 1 |  |  |  |  | 1 |
| Client/Auth | 10 | 2 |  |  |  | 12 |
| Client/Bookings |  | 5 |  |  |  | 5 |
| Client/Catalog | 4 |  |  |  |  | 4 |
| Client/Combos | 2 |  |  |  |  | 2 |
| Client/Movies | 4 |  | 3 |  |  | 7 |
| Client/Settings | 1 |  |  |  |  | 1 |
| Client/Showtimes | 3 |  |  |  |  | 3 |
| Client/Users |  | 4 |  |  |  | 4 |
| Khác/Health | 1 |  |  |  |  | 1 |
| Khác/Payments | 4 |  |  |  |  | 4 |
| **Tổng** | **30** | **11** | **3** | **9** | **84** | **137** |

*Ghi chú: "Khác/Health" và "Khác/Payments" là `HealthController` và `PaymentsController` nằm ngay dưới thư mục `Controllers`, không thuộc nhóm `Admin` hay `Client`. `AppControllerBase` là lớp cơ sở trừu tượng, không có endpoint.*

<!-- GHI CHÚ ĐỐI CHIẾU
(a) [CẦN BỔ SUNG] còn lại:
- Mã Mermaid Hình 4.41–4.46 (`ma-nguon-so-do.md`) KHÔNG có trong các file đính kèm (chỉ có `ma-nguon-so-do-bo-sung.md` gồm PlantUML Hình 3.3–3.24, 4.4–4.25 và `ma-nguon-so-do-mermaid-loyalty.md` gồm Hình 4.26, 4.34, 4.51, 4.54, 4.58). Phần văn bản của 4.4.1–4.4.6 được viết theo mã nguồn `AuthService.cs`, `GoogleAuthService.cs`, `JwtTokenService.cs`, `axios.ts`, chưa đối chiếu được với node/bước trong mã sơ đồ. Cần gửi file `ma-nguon-so-do.md` (hoặc đoạn Hình 4.41–4.46; theo ghi chú trong bo-sung thì đây là Hình 4.19–4.24 cũ cộng 22) để rà khớp và ghi lệch nếu có.
- Cơ chế xác thực nguồn gọi của 4 endpoint `Khác/Payments` (chữ ký VNPay/MoMo/Stripe) chưa đọc trong phần này; mục 4.4.8 chỉ nêu là không dùng JWT. Cần đối chiếu với phần thanh toán.
- Quy định format (`quy-dinh-format-bao-cao.md`) chưa nêu cỡ chữ caption hình/bảng ([CẦN BỔ SUNG] ở quy định); file này dùng caption hình dưới hình, caption bảng trên bảng, chưa định dạng cỡ chữ. Tên file quy định trong prompt là `quy-dinh-trinh-bay.md`, file thực tế đính kèm là `quy-dinh-format-bao-cao.md`.
(b) Chỗ outline lệch code:
- Mục 4.4.5: outline ghi "refresh token đã thu hồi bị dùng lại → thu hồi toàn bộ chuỗi token". Code (`AuthService.RefreshAsync`, `RevokeTokenChainAsync`) chỉ thu hồi tiến theo chuỗi từ `existing.ReplacedByTokenId`, không đi ngược về token trước đó; và chỉ gọi khi token bị thu hồi có `ReplacedByTokenId != null`. Token bị thu hồi do đăng xuất, đổi/đặt lại mật khẩu (không có `ReplacedByTokenId`) khi bị dùng lại chỉ bị từ chối, không kích hoạt thu hồi chuỗi.
- Mục 4.4.8: outline không có cột `EmployeeOnly`; `Program.cs` khai báo policy `EmployeeOnly` (`Policies.EmployeeOnly`) nhưng không controller nào dùng. Đã nêu trong văn bản, không thêm cột.
- Mục 4.4.8: số liệu Bảng 4.80 quét lại từ code bằng script đếm action HTTP, khớp từng dòng và dòng tổng (30/11/3/9/84/137) với bảng mẫu trong outline; không có dòng nào lệch.
- Mục 4.4.7: outline để [CẦN BỔ SUNG] thời hạn mã xác nhận email và mã đặt lại mật khẩu; đã điền theo `EmailOptions` (24 giờ, 30 phút) và `appsettings.json`.
- Comment trong `Program.cs` mô tả policy `auth` áp dụng cho "login, register", nhưng code gắn `[EnableRateLimiting("auth")]` cho 7 endpoint (register, login, admin-login, google, refresh, forgot-password, reset-password) và không gắn cho confirm-email, logout, resend-confirmation.
- Comment đầu `AddAuthorizationBuilder` trong `Program.cs` chỉ nêu CustomerOnly và AdminOnly; code có đủ 4 policy.
-->

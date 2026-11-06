# PHỤ LỤC 2. Danh mục API

Các bảng dưới đây liệt kê toàn bộ endpoint khai báo trong `backend/Cinema.Api/Controllers/**`, đối chiếu từng action với thuộc tính `[Http*]`, `[Authorize]` và `[EnableRateLimiting]` trong mã. Mọi đường dẫn có tiền tố `/api/v1`. Cột "Phân quyền" dùng bốn giá trị: "Công khai" (không có `[Authorize]`), "Đã đăng nhập" (`[Authorize]` không kèm policy), và các policy `CustomerOnly` (vai trò `Customer`), `AdminOnly` (vai trò `Admin`), `StaffOnly` (vai trò `Admin` hoặc `Employee`) khai báo trong `Program.cs`. Cột "Giới hạn tốc độ" ghi policy được gắn riêng cho action; dấu "—" nghĩa là action không có policy riêng. Ngoài ra, mọi yêu cầu (trừ môi trường `Testing`) còn chịu giới hạn toàn cục dạng cửa sổ trượt theo địa chỉ IP, mặc định 100 yêu cầu trong 60 giây (`RateLimit:GlobalPermitLimit`, `GlobalWindowSeconds`); policy `auth` là cửa sổ cố định theo IP, mặc định 5 yêu cầu trong 60 giây; policy `booking-hold` là cửa sổ cố định theo tên người dùng (hoặc IP nếu chưa xác thực), mặc định 10 yêu cầu trong 60 giây. Khi vượt giới hạn, hệ thống trả mã 429.

Cột "Request" ghi nguồn dữ liệu đầu vào của action: tham số đường dẫn (Route), chuỗi truy vấn (Query) và kiểu của thân yêu cầu (Body, ở dạng DTO); khi tham số truy vấn là một đối tượng bộ lọc thì ghi tên kiểu của bộ lọc. Cột "Response" ghi kiểu dữ liệu mà service trả về cho action; ở các endpoint thông thường, `ApiResponseFilter` bọc kiểu này vào `ApiResponse` gồm các trường `Success`, `Data`, `Message`, `Errors`. Action nào trả mã HTTP 201 được ghi chú ngay trong cột này. Các endpoint gắn `[RawResponse]` (thông báo thanh toán) trả JSON riêng theo yêu cầu của cổng thanh toán, không bọc `ApiResponse`. Danh sách phân trang theo trang (`page`, mặc định 1; `pageSize`, mặc định 20) trả `OffsetPageResult<T>`; danh sách phân trang theo con trỏ (`cursor`, `limit`) trả `CursorPageResult<T>`, với `limit` mặc định 10 và được giới hạn trong khoảng 1 đến 50 ở các action dùng `ClampLimit`.

## P2.1 ENDPOINT PHÍA KHÁCH HÀNG

Phía khách hàng có 39 endpoint thuộc chín controller: `Ai` (1), `Auth` (12), `Bookings` (5), `Catalog` (4), `Combos` (2), `Movies` (7), `Settings` (1), `Showtimes` (3) và `Users` (4). Các action đọc dữ liệu danh mục, phim, suất chiếu đều công khai; các action liên quan đến booking và hồ sơ yêu cầu đăng nhập; việc tạo, sửa, xóa đánh giá yêu cầu policy `CustomerOnly`. Endpoint `/api/v1/ai/chat` không có `[Authorize]` nên được xếp là công khai.

**Bảng P2.1. Endpoint phía khách hàng**

| STT | Controller | Phương thức | Đường dẫn | Phân quyền | Action | Giới hạn tốc độ | Mô tả | Request | Response |
|---|---|---|---|---|---|---|---|---|---|
| 1 | Ai | POST | `/api/v1/ai/chat` | Công khai | `Chat` | — | Trò chuyện với trợ lý AI | Body: `AiChatRequest` | `AiChatResponse` |
| 2 | Auth | POST | `/api/v1/auth/register` | Công khai | `Register` | `auth` | Đăng ký tài khoản khách hàng | Body: `RegisterRequest` | `RegisterResponse` (HTTP 201) |
| 3 | Auth | POST | `/api/v1/auth/confirm-email` | Công khai | `ConfirmEmail` | — | Xác nhận email | Body: `ConfirmEmailRequest` | `MessageResponse` |
| 4 | Auth | POST | `/api/v1/auth/login` | Công khai | `Login` | `auth` | Đăng nhập khách hàng | Body: `LoginRequest` | `LoginResponse` |
| 5 | Auth | POST | `/api/v1/auth/admin-login` | Công khai | `AdminLogin` | `auth` | Đăng nhập trang quản trị | Body: `LoginRequest` | `LoginResponse` |
| 6 | Auth | POST | `/api/v1/auth/google` | Công khai | `GoogleLogin` | `auth` | Đăng nhập bằng Google | Body: `GoogleLoginRequest` | `LoginResponse` |
| 7 | Auth | POST | `/api/v1/auth/refresh` | Công khai | `Refresh` | `auth` | Làm mới access token bằng refresh token | Body: `RefreshRequest` | `RefreshResponse` |
| 8 | Auth | POST | `/api/v1/auth/logout` | Công khai | `Logout` | — | Đăng xuất (thu hồi refresh token) | Body: `LogoutRequest` | `MessageResponse` |
| 9 | Auth | POST | `/api/v1/auth/forgot-password` | Công khai | `ForgotPassword` | `auth` | Yêu cầu đặt lại mật khẩu qua email | Body: `ForgotPasswordRequest` | `MessageResponse` |
| 10 | Auth | POST | `/api/v1/auth/reset-password` | Công khai | `ResetPassword` | `auth` | Đặt lại mật khẩu | Body: `ResetPasswordRequest` | `MessageResponse` |
| 11 | Auth | POST | `/api/v1/auth/resend-confirmation` | Công khai | `ResendConfirmationEmail` | — | Gửi lại email xác nhận | Body: `ResendConfirmationEmailRequest` | `MessageResponse` |
| 12 | Auth | POST | `/api/v1/auth/socket-token` | Đã đăng nhập | `GenerateSocketToken` | — | Cấp token ngắn hạn để kết nối SignalR | — | `SocketTokenResponse` |
| 13 | Auth | GET | `/api/v1/auth/me` | Đã đăng nhập | `Me` | — | Thông tin tài khoản đang đăng nhập | — | `MeResponse` |
| 14 | Bookings | POST | `/api/v1/bookings/hold` | Đã đăng nhập | `HoldSeats` | `booking-hold` | Giữ ghế cho một suất chiếu, tạo booking chờ thanh toán | Body: `HoldSeatsRequest` | `HoldBookingResponse` (HTTP 201) |
| 15 | Bookings | GET | `/api/v1/bookings` | Đã đăng nhập | `GetMyBookings` | — | Danh sách booking của tôi, lọc theo trạng thái, phân trang theo con trỏ | Query: `status`, `cursor`, `limit` | `CursorPageResult<BookingListItemDto>` |
| 16 | Bookings | GET | `/api/v1/bookings/{bookingId:int}` | Đã đăng nhập | `GetBookingDetail` | — | Chi tiết booking của tôi | Route: `bookingId` | `BookingDetailDto` |
| 17 | Bookings | POST | `/api/v1/bookings/{bookingId:int}/checkout` | Đã đăng nhập | `Checkout` | — | Thanh toán (checkout) booking đã giữ ghế | Route: `bookingId`; Body: `CheckoutRequest` | `CheckoutResponse` |
| 18 | Bookings | POST | `/api/v1/bookings/{bookingId:int}/cancel` | Đã đăng nhập | `CancelBooking` | — | Hủy booking của tôi | Route: `bookingId` | `CancelBookingResponse` |
| 19 | Catalog | GET | `/api/v1/genres` | Công khai | `GetGenres` | — | Danh sách thể loại công khai | — | `List<GenreDto>` |
| 20 | Catalog | GET | `/api/v1/languages` | Công khai | `GetLanguages` | — | Danh sách ngôn ngữ công khai | — | `List<LanguageDto>` |
| 21 | Catalog | GET | `/api/v1/age-ratings` | Công khai | `GetAgeRatings` | — | Danh sách phân loại độ tuổi công khai | — | `List<AgeRatingDto>` |
| 22 | Catalog | GET | `/api/v1/membership-tiers` | Công khai | `GetMembershipTiers` | — | Danh sách hạng thành viên công khai | — | `List<MembershipTierDto>` |
| 23 | Combos | GET | `/api/v1/combos` | Công khai | `GetCombos` | — | Danh sách combo đang bán | — | `List<ComboDto>` |
| 24 | Combos | GET | `/api/v1/combos/suggestions` | Công khai | `GetSuggestions` | — | Gợi ý combo theo suất chiếu và số ghế | Query: `showtimeId`, `seatCount` | `List<ComboSuggestionDto>` |
| 25 | Movies | GET | `/api/v1/movies` | Công khai | `GetMovies` | — | Danh sách phim, lọc và phân trang theo con trỏ | Query: `MovieListFilter`, `cursor`, `limit` | `CursorPageResult<MovieListItemDto>` |
| 26 | Movies | GET | `/api/v1/movies/{movieId:int}` | Công khai | `GetMovieDetail` | — | Chi tiết một phim | Route: `movieId` | `MovieDetailDto` |
| 27 | Movies | GET | `/api/v1/movies/{movieId:int}/reviews` | Công khai | `GetReviews` | — | Danh sách đánh giá của phim, phân trang theo con trỏ | Route: `movieId`; Query: `cursor`, `limit` | `CursorPageResult<ReviewDto>` |
| 28 | Movies | GET | `/api/v1/movies/{movieId:int}/can-review` | Công khai | `GetCanReviewStatus` | — | Kiểm tra khách có được đánh giá phim hay không | Route: `movieId` | `CanReviewResponse` |
| 29 | Movies | POST | `/api/v1/movies/{movieId:int}/reviews` | CustomerOnly | `CreateReview` | — | Tạo đánh giá phim | Route: `movieId`; Body: `CreateReviewRequest` | `ReviewDto` (HTTP 201) |
| 30 | Movies | PUT | `/api/v1/movies/{movieId:int}/reviews/{id:int}` | CustomerOnly | `UpdateReview` | — | Cập nhật đánh giá của mình | Route: `movieId`, `id`; Body: `CreateReviewRequest` | `ReviewDto` |
| 31 | Movies | DELETE | `/api/v1/movies/{movieId:int}/reviews/{id:int}` | CustomerOnly | `DeleteReview` | — | Xóa đánh giá của mình | Route: `movieId`, `id` | `DeleteReviewResponse` |
| 32 | Settings | GET | `/api/v1/settings` | Công khai | `GetSettings` | — | Cấu hình rạp công khai | — | `CinemaSettingsDto` |
| 33 | Showtimes | GET | `/api/v1/showtimes` | Công khai | `GetSchedule` | — | Lịch chiếu toàn rạp, lọc theo ngày và loại màn hình | Query: `date`, `screenType` | `WholeCinemaScheduleDto` |
| 34 | Showtimes | GET | `/api/v1/showtimes/{showtimeId:int}/seats` | Công khai | `GetSeats` | — | Sơ đồ và trạng thái ghế của một suất chiếu | Route: `showtimeId` | `ShowtimeSeatsResponseDto` |
| 35 | Showtimes | GET | `/api/v1/showtimes/{showtimeId:int}/seat-suggestions` | Công khai | `GetSeatSuggestions` | — | Gợi ý ghế theo số lượng ghế | Route: `showtimeId`; Query: `count` | `List<SeatSuggestionDto>` |
| 36 | Users | GET | `/api/v1/users/me` | Đã đăng nhập | `GetMyProfile` | — | Xem hồ sơ cá nhân | — | `UserProfileDto` |
| 37 | Users | PUT | `/api/v1/users/me` | Đã đăng nhập | `UpdateMyProfile` | — | Cập nhật hồ sơ cá nhân | Body: `UpdateProfileRequest` | `UserProfileDto` |
| 38 | Users | GET | `/api/v1/users/me/reviews` | Đã đăng nhập | `GetMyReviews` | — | Danh sách đánh giá của tôi, phân trang theo con trỏ | Query: `cursor`, `limit` | `CursorPageResult<UserMyReviewDto>` |
| 39 | Users | POST | `/api/v1/users/me/change-password` | Đã đăng nhập | `ChangePassword` | — | Đổi mật khẩu | Body: `ChangePasswordRequest` | `MessageResponse` |

## P2.2 ENDPOINT PHÍA QUẢN TRỊ

Phía quản trị có 93 endpoint thuộc 22 controller có tiền tố `Admin`, đường dẫn bắt đầu bằng `/api/v1/admin`. Phần lớn endpoint dùng policy `AdminOnly`; các endpoint phục vụ bán vé tại quầy và soát vé (`AdminBookings`), tra cứu khách hàng (`/admin/users/customers`) và các endpoint đọc danh sách phim, suất chiếu, lịch suất chiếu dùng policy `StaffOnly` để nhân viên cũng truy cập được. Ở `AdminMovies` và `AdminShowtimes`, policy `StaffOnly` của các action đọc khác policy `AdminOnly` của các action ghi trong cùng controller.

**Bảng P2.2. Endpoint phía quản trị**

| STT | Controller | Phương thức | Đường dẫn | Phân quyền | Action | Giới hạn tốc độ | Mô tả | Request | Response |
|---|---|---|---|---|---|---|---|---|---|
| 1 | AdminActors | GET | `/api/v1/admin/actors` | AdminOnly | `GetAll` | — | Danh sách diễn viên, tìm theo tên, phân trang theo trang | Query: `search`, `page`, `pageSize` | `OffsetPageResult<ActorDto>` |
| 2 | AdminActors | POST | `/api/v1/admin/actors` | AdminOnly | `Create` | — | Tạo diễn viên | Body: `UpsertPersonRequest` | `ActorDto` (HTTP 201) |
| 3 | AdminActors | PUT | `/api/v1/admin/actors/{id:int}` | AdminOnly | `Update` | — | Cập nhật diễn viên | Route: `id`; Body: `UpsertPersonRequest` | `ActorDto` |
| 4 | AdminActors | DELETE | `/api/v1/admin/actors/{id:int}` | AdminOnly | `Delete` | — | Xóa diễn viên | Route: `id` | `DeleteCategoryResponse` |
| 5 | AdminActors | DELETE | `/api/v1/admin/actors/bulk` | AdminOnly | `BulkDelete` | — | Xóa hàng loạt diễn viên | Body: `BulkIdsRequest` | `BulkActionResponse` |
| 6 | AdminAgeRatings | GET | `/api/v1/admin/age-ratings` | AdminOnly | `GetAll` | — | Danh sách phân loại độ tuổi, phân trang theo trang | Query: `page`, `pageSize` | `OffsetPageResult<AgeRatingDto>` |
| 7 | AdminAgeRatings | POST | `/api/v1/admin/age-ratings` | AdminOnly | `Create` | — | Tạo phân loại độ tuổi | Body: `UpsertAgeRatingRequest` | `AgeRatingDto` (HTTP 201) |
| 8 | AdminAgeRatings | PUT | `/api/v1/admin/age-ratings/{id:int}` | AdminOnly | `Update` | — | Cập nhật phân loại độ tuổi | Route: `id`; Body: `UpsertAgeRatingRequest` | `AgeRatingDto` |
| 9 | AdminAgeRatings | DELETE | `/api/v1/admin/age-ratings/{id:int}` | AdminOnly | `Delete` | — | Xóa phân loại độ tuổi | Route: `id` | `DeleteCategoryResponse` |
| 10 | AdminBookings | POST | `/api/v1/admin/bookings/counter-hold` | StaffOnly | `CounterHoldSeats` | — | Giữ ghế tại quầy cho khách | Body: `CounterHoldRequest` | `CounterHoldResponse` (HTTP 201) |
| 11 | AdminBookings | GET | `/api/v1/admin/bookings` | StaffOnly | `GetBookings` | — | Danh sách booking, lọc và phân trang theo trang | Query: `AdminBookingListFilter`, `page`, `pageSize` | `OffsetPageResult<AdminBookingListItemDto>` |
| 12 | AdminBookings | GET | `/api/v1/admin/bookings/{bookingId:int}` | StaffOnly | `GetBookingDetail` | — | Chi tiết một booking | Route: `bookingId` | `AdminBookingDetailDto` |
| 13 | AdminBookings | POST | `/api/v1/admin/bookings/{bookingId:int}/counter-checkout` | StaffOnly | `CounterCheckout` | — | Thanh toán tại quầy cho booking đã giữ ghế | Route: `bookingId`; Body: `CounterCheckoutRequest` | `CounterCheckoutResponse` |
| 14 | AdminBookings | POST | `/api/v1/admin/bookings/checkin` | StaffOnly | `Checkin` | — | Soát vé (check-in) bằng mã vé | Body: `CheckinRequest` | `CheckinResponse` |
| 15 | AdminCombos | GET | `/api/v1/admin/combos` | AdminOnly | `GetCombos` | — | Danh sách combo, phân trang theo trang | Query: `page`, `pageSize` | `OffsetPageResult<AdminComboDto>` |
| 16 | AdminCombos | POST | `/api/v1/admin/combos` | AdminOnly | `Create` | — | Tạo combo | Body: `UpsertComboRequest` | `AdminComboDto` (HTTP 201) |
| 17 | AdminCombos | PUT | `/api/v1/admin/combos/{id:int}` | AdminOnly | `Update` | — | Cập nhật combo | Route: `id`; Body: `UpsertComboRequest` | `AdminComboDto` |
| 18 | AdminCombos | PUT | `/api/v1/admin/combos/{id:int}/toggle` | AdminOnly | `Toggle` | — | Bật hoặc tắt trạng thái bán của combo | Route: `id` | `ToggleComboResponse` |
| 19 | AdminCombos | PUT | `/api/v1/admin/combos/bulk/status` | AdminOnly | `BulkSetActive` | — | Đặt trạng thái bán hàng loạt cho nhiều combo | Body: `BulkStatusRequest` | `BulkActionResponse` |
| 20 | AdminDashboard | GET | `/api/v1/admin/dashboard/summary` | AdminOnly | `GetSummary` | — | Số liệu tổng quan của bảng điều khiển | — | `DashboardSummaryDto` |
| 21 | AdminDashboard | GET | `/api/v1/admin/dashboard/insights` | AdminOnly | `GetInsights` | — | Số liệu phân tích của bảng điều khiển theo số ngày | Query: `days` | `DashboardInsightsDto` |
| 22 | AdminDirectors | GET | `/api/v1/admin/directors` | AdminOnly | `GetAll` | — | Danh sách đạo diễn, tìm theo tên, phân trang theo trang | Query: `search`, `page`, `pageSize` | `OffsetPageResult<DirectorDto>` |
| 23 | AdminDirectors | POST | `/api/v1/admin/directors` | AdminOnly | `Create` | — | Tạo đạo diễn | Body: `UpsertPersonRequest` | `DirectorDto` (HTTP 201) |
| 24 | AdminDirectors | PUT | `/api/v1/admin/directors/{id:int}` | AdminOnly | `Update` | — | Cập nhật đạo diễn | Route: `id`; Body: `UpsertPersonRequest` | `DirectorDto` |
| 25 | AdminDirectors | DELETE | `/api/v1/admin/directors/{id:int}` | AdminOnly | `Delete` | — | Xóa đạo diễn | Route: `id` | `DeleteCategoryResponse` |
| 26 | AdminDirectors | DELETE | `/api/v1/admin/directors/bulk` | AdminOnly | `BulkDelete` | — | Xóa hàng loạt đạo diễn | Body: `BulkIdsRequest` | `BulkActionResponse` |
| 27 | AdminEmployees | GET | `/api/v1/admin/employees` | AdminOnly | `GetEmployees` | — | Danh sách nhân viên, lọc theo chức vụ, phân trang theo trang | Query: `positionId`, `page`, `pageSize` | `OffsetPageResult<AdminEmployeeDto>` |
| 28 | AdminEmployees | POST | `/api/v1/admin/employees` | AdminOnly | `Create` | — | Tạo nhân viên | Body: `CreateEmployeeRequest` | `AdminEmployeeDto` (HTTP 201) |
| 29 | AdminEmployees | PUT | `/api/v1/admin/employees/{id:int}` | AdminOnly | `Update` | — | Cập nhật nhân viên | Route: `id`; Body: `UpdateEmployeeRequest` | `AdminEmployeeDto` |
| 30 | AdminGenres | GET | `/api/v1/admin/genres` | AdminOnly | `GetAll` | — | Danh sách thể loại, phân trang theo trang | Query: `page`, `pageSize` | `OffsetPageResult<GenreDto>` |
| 31 | AdminGenres | POST | `/api/v1/admin/genres` | AdminOnly | `Create` | — | Tạo thể loại | Body: `UpsertGenreRequest` | `GenreDto` (HTTP 201) |
| 32 | AdminGenres | PUT | `/api/v1/admin/genres/{id:int}` | AdminOnly | `Update` | — | Cập nhật thể loại | Route: `id`; Body: `UpsertGenreRequest` | `GenreDto` |
| 33 | AdminGenres | DELETE | `/api/v1/admin/genres/{id:int}` | AdminOnly | `Delete` | — | Xóa thể loại | Route: `id` | `DeleteCategoryResponse` |
| 34 | AdminLanguages | GET | `/api/v1/admin/languages` | AdminOnly | `GetAll` | — | Danh sách ngôn ngữ, phân trang theo trang | Query: `page`, `pageSize` | `OffsetPageResult<LanguageDto>` |
| 35 | AdminLanguages | POST | `/api/v1/admin/languages` | AdminOnly | `Create` | — | Tạo ngôn ngữ | Body: `UpsertLanguageRequest` | `LanguageDto` (HTTP 201) |
| 36 | AdminLanguages | PUT | `/api/v1/admin/languages/{id:int}` | AdminOnly | `Update` | — | Cập nhật ngôn ngữ | Route: `id`; Body: `UpsertLanguageRequest` | `LanguageDto` |
| 37 | AdminLanguages | DELETE | `/api/v1/admin/languages/{id:int}` | AdminOnly | `Delete` | — | Xóa ngôn ngữ | Route: `id` | `DeleteCategoryResponse` |
| 38 | AdminMembershipTiers | GET | `/api/v1/admin/membership-tiers` | AdminOnly | `GetAll` | — | Danh sách hạng thành viên, phân trang theo trang | Query: `page`, `pageSize` | `OffsetPageResult<MembershipTierDto>` |
| 39 | AdminMembershipTiers | POST | `/api/v1/admin/membership-tiers` | AdminOnly | `Create` | — | Tạo hạng thành viên | Body: `UpsertMembershipTierRequest` | `MembershipTierDto` (HTTP 201) |
| 40 | AdminMembershipTiers | PUT | `/api/v1/admin/membership-tiers/{id:int}` | AdminOnly | `Update` | — | Cập nhật hạng thành viên | Route: `id`; Body: `UpsertMembershipTierRequest` | `MembershipTierDto` |
| 41 | AdminMembershipTiers | DELETE | `/api/v1/admin/membership-tiers/{id:int}` | AdminOnly | `Delete` | — | Xóa hạng thành viên | Route: `id` | `DeleteCategoryResponse` |
| 42 | AdminMovies | GET | `/api/v1/admin/movies` | StaffOnly | `GetMovies` | — | Danh sách phim phía quản trị, lọc và phân trang theo trang | Query: `AdminMovieListFilter`, `page`, `pageSize` | `OffsetPageResult<AdminMovieListItemDto>` |
| 43 | AdminMovies | POST | `/api/v1/admin/movies` | AdminOnly | `Create` | — | Tạo phim | Body: `UpsertMovieRequest` | `MovieDetailDto` (HTTP 201) |
| 44 | AdminMovies | PUT | `/api/v1/admin/movies/{id:int}` | AdminOnly | `Update` | — | Cập nhật phim | Route: `id`; Body: `UpsertMovieRequest` | `MovieDetailDto` |
| 45 | AdminMovies | DELETE | `/api/v1/admin/movies/{id:int}` | AdminOnly | `Delete` | — | Xóa phim | Route: `id` | `DeleteMovieResponse` |
| 46 | AdminMovies | DELETE | `/api/v1/admin/movies/bulk` | AdminOnly | `BulkDelete` | — | Xóa hàng loạt phim | Body: `BulkDeleteMoviesRequest` | `BulkDeleteMoviesResponse` |
| 47 | AdminPayments | GET | `/api/v1/admin/payments` | AdminOnly | `GetPayments` | — | Danh sách giao dịch thanh toán, lọc và phân trang theo trang | Query: `AdminPaymentListFilter`, `page`, `pageSize` | `OffsetPageResult<AdminPaymentListItemDto>` |
| 48 | AdminPayments | GET | `/api/v1/admin/payments/{id:int}` | AdminOnly | `GetDetail` | — | Chi tiết một giao dịch thanh toán | Route: `id` | `AdminPaymentDetailDto` |
| 49 | AdminPositions | GET | `/api/v1/admin/positions` | AdminOnly | `GetAll` | — | Danh sách chức vụ, phân trang theo trang | Query: `page`, `pageSize` | `OffsetPageResult<PositionDto>` |
| 50 | AdminPositions | POST | `/api/v1/admin/positions` | AdminOnly | `Create` | — | Tạo chức vụ | Body: `UpsertPositionRequest` | `PositionDto` (HTTP 201) |
| 51 | AdminPositions | PUT | `/api/v1/admin/positions/{id:int}` | AdminOnly | `Update` | — | Cập nhật chức vụ | Route: `id`; Body: `UpsertPositionRequest` | `PositionDto` |
| 52 | AdminPositions | DELETE | `/api/v1/admin/positions/{id:int}` | AdminOnly | `Delete` | — | Xóa chức vụ | Route: `id` | `DeleteCategoryResponse` |
| 53 | AdminReports | GET | `/api/v1/admin/reports/revenue` | AdminOnly | `GetRevenue` | — | Báo cáo doanh thu theo khoảng ngày và cách nhóm | Query: `fromDate`, `toDate`, `groupBy` | `RevenueReportDto` |
| 54 | AdminReports | GET | `/api/v1/admin/reports/top-movies` | AdminOnly | `GetTopMovies` | — | Báo cáo các phim có doanh thu cao nhất trong khoảng ngày | Query: `fromDate`, `toDate`, `limit` | `List<TopMovieDto>` |
| 55 | AdminReports | GET | `/api/v1/admin/reports/employee-performance` | AdminOnly | `GetEmployeePerformance` | — | Báo cáo hiệu suất của nhân viên trong khoảng ngày | Query: `fromDate`, `toDate` | `List<EmployeePerformanceDto>` |
| 56 | AdminReviews | GET | `/api/v1/admin/reviews` | AdminOnly | `GetReviews` | — | Danh sách đánh giá phía quản trị, lọc theo phim và số sao | Query: `movieId`, `rating`, `page`, `pageSize` | `OffsetPageResult<AdminReviewDto>` |
| 57 | AdminReviews | DELETE | `/api/v1/admin/reviews/{id:int}` | AdminOnly | `Delete` | — | Xóa một đánh giá | Route: `id` | `DeleteReviewResponse` |
| 58 | AdminReviews | DELETE | `/api/v1/admin/reviews/bulk` | AdminOnly | `BulkDelete` | — | Xóa hàng loạt đánh giá | Body: `BulkDeleteReviewsRequest` | `BulkDeleteReviewsResponse` |
| 59 | AdminRooms | GET | `/api/v1/admin/rooms` | AdminOnly | `GetRooms` | — | Danh sách phòng chiếu, phân trang theo trang | Query: `page`, `pageSize` | `OffsetPageResult<AdminRoomDto>` |
| 60 | AdminRooms | POST | `/api/v1/admin/rooms` | AdminOnly | `Create` | — | Tạo phòng chiếu | Body: `CreateRoomRequest` | `AdminRoomDto` (HTTP 201) |
| 61 | AdminRooms | GET | `/api/v1/admin/rooms/{id:int}` | AdminOnly | `GetById` | — | Chi tiết một phòng chiếu | Route: `id` | `AdminRoomDto` |
| 62 | AdminRooms | PUT | `/api/v1/admin/rooms/{id:int}` | AdminOnly | `Update` | — | Cập nhật phòng chiếu | Route: `id`; Body: `UpdateRoomRequest` | `AdminRoomDto` |
| 63 | AdminRooms | DELETE | `/api/v1/admin/rooms/{id:int}` | AdminOnly | `Delete` | — | Xóa phòng chiếu | Route: `id` | `DeleteRoomResponse` |
| 64 | AdminRooms | GET | `/api/v1/admin/rooms/{id:int}/seats` | AdminOnly | `GetSeats` | — | Lấy sơ đồ ghế của phòng chiếu | Route: `id` | `List<SeatDto>` |
| 65 | AdminRooms | PUT | `/api/v1/admin/rooms/{id:int}/seats` | AdminOnly | `UpdateSeatLayout` | — | Lưu sơ đồ ghế của phòng chiếu | Route: `id`; Body: `UpdateSeatLayoutRequest` | `UpdateSeatLayoutResponse` |
| 66 | AdminScreenTypes | GET | `/api/v1/admin/screen-types` | AdminOnly | `GetAll` | — | Danh sách loại màn hình, phân trang theo trang | Query: `page`, `pageSize` | `OffsetPageResult<ScreenTypeDto>` |
| 67 | AdminScreenTypes | POST | `/api/v1/admin/screen-types` | AdminOnly | `Create` | — | Tạo loại màn hình | Body: `UpsertScreenTypeRequest` | `ScreenTypeDto` (HTTP 201) |
| 68 | AdminScreenTypes | PUT | `/api/v1/admin/screen-types/{id:int}` | AdminOnly | `Update` | — | Cập nhật loại màn hình | Route: `id`; Body: `UpsertScreenTypeRequest` | `ScreenTypeDto` |
| 69 | AdminScreenTypes | DELETE | `/api/v1/admin/screen-types/{id:int}` | AdminOnly | `Delete` | — | Xóa loại màn hình | Route: `id` | `DeleteCategoryResponse` |
| 70 | AdminSeatTypes | GET | `/api/v1/admin/seat-types` | AdminOnly | `GetAll` | — | Danh sách loại ghế, phân trang theo trang | Query: `page`, `pageSize` | `OffsetPageResult<SeatTypeDto>` |
| 71 | AdminSeatTypes | POST | `/api/v1/admin/seat-types` | AdminOnly | `Create` | — | Tạo loại ghế | Body: `UpsertSeatTypeRequest` | `SeatTypeDto` (HTTP 201) |
| 72 | AdminSeatTypes | PUT | `/api/v1/admin/seat-types/{id:int}` | AdminOnly | `Update` | — | Cập nhật loại ghế | Route: `id`; Body: `UpsertSeatTypeRequest` | `SeatTypeDto` |
| 73 | AdminSeatTypes | DELETE | `/api/v1/admin/seat-types/{id:int}` | AdminOnly | `Delete` | — | Xóa loại ghế | Route: `id` | `DeleteCategoryResponse` |
| 74 | AdminSettings | GET | `/api/v1/admin/settings` | AdminOnly | `Get` | — | Xem cấu hình rạp | — | `CinemaSettingsDto` |
| 75 | AdminSettings | PUT | `/api/v1/admin/settings` | AdminOnly | `Update` | — | Cập nhật cấu hình rạp | Body: `UpdateSettingsRequest` | `CinemaSettingsDto` |
| 76 | AdminShowtimes | GET | `/api/v1/admin/showtimes` | StaffOnly | `GetShowtimes` | — | Danh sách suất chiếu phía quản trị, lọc và phân trang theo trang | Query: `AdminShowtimeListFilter`, `page`, `pageSize` | `OffsetPageResult<AdminShowtimeDto>` |
| 77 | AdminShowtimes | GET | `/api/v1/admin/showtimes/calendar` | StaffOnly | `GetCalendar` | — | Lịch suất chiếu dạng lịch theo bộ lọc | Query: `AdminShowtimeCalendarFilter` | `ShowtimeCalendarResponseDto` |
| 78 | AdminShowtimes | POST | `/api/v1/admin/showtimes` | AdminOnly | `Create` | — | Tạo suất chiếu | Body: `UpsertShowtimeRequest` | `CreateShowtimeResponse` (HTTP 201) |
| 79 | AdminShowtimes | PUT | `/api/v1/admin/showtimes/{id:int}` | AdminOnly | `Update` | — | Cập nhật suất chiếu | Route: `id`; Body: `UpsertShowtimeRequest` | `AdminShowtimeDto` |
| 80 | AdminShowtimes | DELETE | `/api/v1/admin/showtimes/{id:int}` | AdminOnly | `Delete` | — | Xóa suất chiếu | Route: `id` | `DeleteShowtimeResponse` |
| 81 | AdminShowtimes | POST | `/api/v1/admin/showtimes/bulk` | AdminOnly | `BulkCreate` | — | Tạo hàng loạt suất chiếu | Body: `BulkCreateShowtimesRequest` | `BulkCreateShowtimesResponse` (HTTP 201) |
| 82 | AdminShowtimes | POST | `/api/v1/admin/showtimes/auto-schedule/preview` | AdminOnly | `PreviewAutoSchedule` | — | Xem trước kết quả xếp lịch chiếu tự động | Body: `AutoScheduleRequest` | `AutoScheduleResponseDto` |
| 83 | AdminShowtimes | POST | `/api/v1/admin/showtimes/auto-schedule/apply` | AdminOnly | `ApplyAutoSchedule` | — | Áp dụng kết quả xếp lịch chiếu tự động | Body: `ApplyAutoScheduleRequest` | `BulkCreateShowtimesResponse` (HTTP 201) |
| 84 | AdminShowtimes | POST | `/api/v1/admin/showtimes/clone` | AdminOnly | `CloneShowtimes` | — | Sao chép suất chiếu sang ngày khác | Body: `CloneShowtimesRequest` | `BulkCreateShowtimesResponse` (HTTP 201) |
| 85 | AdminShowtimes | DELETE | `/api/v1/admin/showtimes/bulk` | AdminOnly | `BulkDelete` | — | Xóa hàng loạt suất chiếu | Body: `BulkDeleteShowtimesRequest` | `BulkDeleteShowtimesResponse` |
| 86 | AdminUploads | POST | `/api/v1/admin/uploads/image` | AdminOnly | `UploadImage` | — | Tải ảnh lên (thư mục lưu qua tham số folder) | Query: `folder`; Form: `file` | `FileUploadResult` |
| 87 | AdminUploads | POST | `/api/v1/admin/uploads/video` | AdminOnly | `UploadVideo` | — | Tải video lên (thư mục lưu qua tham số folder) | Query: `folder`; Form: `file` | `FileUploadResult` |
| 88 | AdminUsers | GET | `/api/v1/admin/users` | AdminOnly | `GetUsers` | — | Danh sách người dùng, lọc theo vai trò và từ khóa, phân trang theo trang | Query: `roleId`, `search`, `page`, `pageSize` | `OffsetPageResult<AdminUserListItemDto>` |
| 89 | AdminUsers | GET | `/api/v1/admin/users/admins` | AdminOnly | `GetAdminUsers` | — | Danh sách tài khoản quản trị dùng cho ô chọn (mặc định chỉ tài khoản còn khả dụng) | Query: `search`, `availableOnly` | `List<AdminUserSelectDto>` |
| 90 | AdminUsers | GET | `/api/v1/admin/users/customers` | StaffOnly | `SearchCustomers` | — | Tìm khách hàng phục vụ bán vé tại quầy | Query: `q` | `List<CustomerLookupItemDto>` |
| 91 | AdminUsers | PUT | `/api/v1/admin/users/{id:int}/status` | AdminOnly | `UpdateStatus` | — | Cập nhật trạng thái hoạt động của người dùng | Route: `id`; Body: `UpdateUserStatusRequest` | `UpdateUserStatusResponse` |
| 92 | AdminUsers | PUT | `/api/v1/admin/users/{id:int}/role` | AdminOnly | `UpdateRole` | — | Cập nhật vai trò của người dùng | Route: `id`; Body: `UpdateUserRoleRequest` | `UpdateUserRoleResponse` |
| 93 | AdminUsers | PUT | `/api/v1/admin/users/bulk/status` | AdminOnly | `BulkUpdateStatus` | — | Cập nhật trạng thái hàng loạt cho nhiều người dùng | Body: `BulkStatusRequest` | `BulkActionResponse` |

## P2.3 ENDPOINT THANH TOÁN, KIỂM TRA SỨC KHỎE

Nhóm này gồm 5 endpoint: một endpoint kiểm tra sức khỏe dịch vụ và bốn endpoint nhận thông báo từ cổng thanh toán (VNPay hai phương thức GET và POST, MoMo, Stripe). Các endpoint thanh toán không yêu cầu đăng nhập vì do máy chủ cổng thanh toán gọi tới; tính hợp lệ của yêu cầu được xác minh bên trong `PaymentService` (xử lý IPN của VNPay và MoMo, và chữ ký `Stripe-Signature` của Stripe), không phải bằng `[Authorize]`. Các endpoint này không có policy giới hạn tốc độ riêng nhưng vẫn chịu giới hạn toàn cục theo IP. Địa chỉ `/api/v1/health` cũng là địa chỉ kiểm tra sức khỏe của container `api` trong `docker-compose.yml` (Phụ lục 1, Mục P1.3).

**Bảng P2.3. Endpoint thanh toán, kiểm tra sức khỏe**

| STT | Controller | Phương thức | Đường dẫn | Phân quyền | Action | Giới hạn tốc độ | Mô tả | Request | Response |
|---|---|---|---|---|---|---|---|---|---|
| 1 | Health | GET | `/api/v1/health` | Công khai | `Get` | — | Kiểm tra sức khỏe dịch vụ | — | `{ status, time }` |
| 2 | Payments | GET | `/api/v1/payments/vnpay/ipn` | Công khai | `VnPayIpn` | — | Nhận thông báo IPN của VNPay (GET), xác minh và cập nhật thanh toán | — | `{ RspCode, Message }` (không bọc `ApiResponse`) |
| 3 | Payments | POST | `/api/v1/payments/vnpay/ipn` | Công khai | `VnPayIpn` | — | Nhận thông báo IPN của VNPay (POST), xác minh và cập nhật thanh toán | — | `{ RspCode, Message }` (không bọc `ApiResponse`) |
| 4 | Payments | POST | `/api/v1/payments/momo/ipn` | Công khai | `MomoIpn` | — | Nhận thông báo IPN của MoMo, xác minh và cập nhật thanh toán | — | `{ resultCode, message }` (không bọc `ApiResponse`) |
| 5 | Payments | POST | `/api/v1/payments/stripe/webhook` | Công khai | `StripeWebhook` | — | Nhận webhook của Stripe, xác minh chữ ký Stripe-Signature | — | `{ message }`; 400 khi chữ ký không hợp lệ (không bọc `ApiResponse`) |

<!-- GHI CHÚ ĐỐI CHIẾU -->
<!--
(a) [CẦN BỔ SUNG] còn lại
- Không có `[CẦN BỔ SUNG]` trong phần này: cột Mô tả viết từ tên action và DTO theo yêu cầu outline; mô tả mang tính tóm tắt, chưa đọc nội dung từng service nên không nêu quy tắc nghiệp vụ.
- Cột Request chỉ liệt kê tên tham số hoặc DTO, chưa mô tả trường của DTO (ngoài phạm vi); nếu cần, đối chiếu thêm Chương 4.

(b) Đối chiếu với outline và Bảng 4.91
- Đã trích 137 endpoint từ 33 controller (22 Admin, 9 Client, Health, Payments) bằng cách đọc thuộc tính trong mã. Tổng: Admin 93, Client 39, Khác 5 (Health 1 + Payments 4, trong đó VnPayIpn tính là hai endpoint GET và POST) = 137; khớp Bảng 4.91 và các số 39/93/5 ở P2.1 đến P2.3.
- Số endpoint theo từng controller khớp từng dòng Bảng 4.91 (33 dòng): AdminActors 5, AdminAgeRatings 4, AdminBookings 5, AdminCombos 5, AdminDashboard 2, AdminDirectors 5, AdminEmployees 3, AdminGenres 4, AdminLanguages 4, AdminMembershipTiers 4, AdminMovies 5, AdminPayments 2, AdminPositions 4, AdminReports 3, AdminReviews 3, AdminRooms 7, AdminScreenTypes 4, AdminSeatTypes 4, AdminSettings 2, AdminShowtimes 10, AdminUploads 2, AdminUsers 6, Ai 1, Auth 12, Bookings 5, Catalog 4, Combos 2, Movies 7, Settings 1, Showtimes 3, Users 4, Health 1, Payments 4. Không lệch.
- So khớp từng dòng (phương thức, đường dẫn, phân quyền, tên action, giới hạn tốc độ) giữa mã và các bảng P2.1 đến P2.3 của outline (so theo tập hợp, vì outline xếp khách hàng trước quản trị): 137/137 dòng khớp.
- Quan sát thêm (không lệch outline): policy `EmployeeOnly` được khai báo trong Program.cs nhưng không endpoint nào dùng; `/api/v1/ai/chat` không có [Authorize] (đã nêu ở Mục 7.3.6); các endpoint Payments dùng [RawResponse] nên phản hồi không bọc ApiResponse; Health không có [RawResponse] nên được bọc ApiResponse (cột Response ghi kiểu dữ liệu bên trong).
- Cột Response lấy từ kiểu trả về của phương thức service mà action gọi đầu tiên (tra trong `Cinema.Application/Interfaces`), không phải từ `ProducesResponseType` (controller không khai báo thuộc tính này).
- Tệp quy định trình bày được cung cấp tên `quy-dinh-format-bao-cao.md` (prompt ghi `quy-dinh-trinh-bay.md`); caption bảng đặt phía trên theo Mục 0.3.
-->

### 4.3.11 Module Thành viên

Module Thành viên lưu cấu hình các hạng thành viên và sổ điểm của từng khách hàng. Module gồm hai thực thể sở hữu là `MembershipTier` (bảng `MembershipTiers`) và `LoyaltyPointEntry` (bảng `LoyaltyPointEntries`). `MembershipTier` được tham chiếu từ `CustomerProfile.TierId` (xem Mục 4.3.4); `LoyaltyPointEntry` tham chiếu tới `User` (khách hàng sở hữu dòng điểm) và `Booking` (đơn đặt vé sinh ra điểm).

Sổ điểm (`LoyaltyPointEntry`) là nguồn dữ liệu gốc của toàn bộ nghiệp vụ điểm thành viên. Mỗi lần thanh toán thành công, `BookingService.ApplyLoyaltyAsync` ghi một dòng sổ điểm với số điểm `LoyaltyWindow.PointsFor(finalAmount, settings.PointsPerAmount)`, tức phần nguyên của `FinalAmount / PointsPerAmount`; dòng chỉ được tạo khi số điểm lớn hơn 0. Khi hoàn tiền, `BookingService.ReverseLoyaltyAsync` không xóa dòng mà tăng `RevokedPoints` của đúng dòng thuộc booking đó. Điểm hiệu lực của một khách hàng là tổng `Points − RevokedPoints` của các dòng có `EarnedAt` lớn hơn mốc `LoyaltyWindow.Cutoff(now)`, trong đó mốc này là thời điểm hiện tại lùi 12 tháng (`LoyaltyWindow.Months = 12`); như vậy điểm được tính theo cửa sổ trượt 12 tháng. Hạng thành viên là hạng có `MinPoints` lớn nhất không vượt quá điểm hiệu lực (`LoyaltyWindow.ResolveTierAsync`).

Hai cột `CustomerProfile.Points` và `CustomerProfile.TierId` chỉ là giá trị đồng bộ, dùng để đọc nhanh khi hiển thị hồ sơ. Chúng được ghi lại bởi `LoyaltyWindow.ResyncProfileAsync` sau mỗi lần cộng hoặc thu hồi điểm, và được tính lại định kỳ bởi `LoyaltyExpiryService.RecalculateAllAsync` do `ExpireLoyaltyPointsJob` gọi (cron mặc định `5 * * * *`, múi giờ Việt Nam). Giảm giá khi đặt vé không đọc hai cột này: `BookingService.CalculateDiscountAmountAsync` tính lại điểm hiệu lực trực tiếp từ sổ điểm bằng `LoyaltyWindow.SumActiveAsync` rồi xác định hạng, nên điểm vừa hết hạn có hiệu lực ngay mà không phải chờ job. Mức giảm là `DiscountPercent` của hạng áp lên tiền vé (không gồm combo), làm tròn đến đơn vị đồng theo `MidpointRounding.AwayFromZero`. Các hạng được quản trị qua `AdminMembershipTiersController` (xem, tạo, sửa, xóa).

#### 4.3.11.1 Sơ đồ ERD của module

Hình 4.34 thể hiện hai thực thể sở hữu với đầy đủ thuộc tính. `MembershipTiers` có `TierId` (khóa chính), `TierName` (khóa duy nhất), `MinPoints` và `DiscountPercent`. `LoyaltyPointEntries` có `EntryId` (khóa chính), `UserId` (khóa ngoại), `BookingId` (khóa ngoại, cho phép NULL và duy nhất khi khác NULL), `Points`, `RevokedPoints` và `EarnedAt`. Ba thực thể thuộc module khác là `Users`, `Bookings` và `CustomerProfiles` chỉ xuất hiện dưới dạng tham chiếu với cột khóa chính (`UserId`, `BookingId`, `UserId`).

Sơ đồ có ba quan hệ. Quan hệ `Users` – `LoyaltyPointEntries` là một-nhiều, trong đó mỗi dòng sổ điểm bắt buộc thuộc một khách hàng qua `UserId`. Quan hệ `Bookings` – `LoyaltyPointEntries` là không-hoặc-một ở cả hai phía qua `BookingId`: một booking có tối đa một dòng điểm và một dòng điểm có thể không gắn booking nào. Quan hệ `MembershipTiers` – `CustomerProfiles` là một-nhiều tùy chọn qua `TierId`, vì hồ sơ khách hàng có thể chưa có hạng.

![Hình 4.34. ERD module Thành viên](hinh/4.34.png)

*Hình 4.34. ERD module Thành viên*

#### 4.3.11.2 Mô tả thực thể

`MembershipTier` mô tả một hạng thành viên. Một hạng được xác định bởi tên duy nhất, ngưỡng điểm hiệu lực tối thiểu để đạt hạng (`MinPoints`) và phần trăm giảm giá trên tiền vé (`DiscountPercent`). Các hạng không liên kết trực tiếp với khách hàng; hạng của khách hàng được suy ra từ điểm hiệu lực, và `CustomerProfile.TierId` chỉ lưu bản đồng bộ của kết quả đó.

*Bảng 4.71. Thực thể MembershipTier (bảng MembershipTiers)*

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `TierId` | int | Không | PK | Tự tăng | Mã định danh hạng thành viên |
| `TierName` | varchar(50) | Không | UK | — | Tên hạng, không trùng nhau (chỉ mục `IX_MembershipTiers_TierName`) |
| `MinPoints` | int | Không | — | 0 | Điểm hiệu lực tối thiểu để đạt hạng |
| `DiscountPercent` | numeric(5,2) | Không | — | 0 | Phần trăm giảm giá trên tiền vé, trong đoạn từ 0 đến 100 |

`LoyaltyPointEntry` là một dòng của sổ điểm. Mỗi dòng ghi lại số điểm một khách hàng tích được từ một booking, số điểm đã bị thu hồi do hoàn tiền và thời điểm tích điểm. Thời điểm `EarnedAt` là mốc để xét dòng điểm còn nằm trong cửa sổ trượt 12 tháng hay không. Dòng điểm không gắn booking (`BookingId` là NULL) dùng cho điểm khởi tạo của tài khoản mẫu (xem Mục 4.3.16). Khi booking đã Confirmed bị hủy và hoàn tiền, dòng điểm của booking đó không bị xóa mà `RevokedPoints` được tăng, nhưng không vượt quá `Points`.

*Bảng 4.72. Thực thể LoyaltyPointEntry (bảng LoyaltyPointEntries)*

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `EntryId` | int | Không | PK | Tự tăng | Mã định danh dòng sổ điểm |
| `UserId` | int | Không | FK | Tham chiếu `Users.UserId` | Khách hàng sở hữu dòng điểm |
| `BookingId` | int | Có | FK | Tham chiếu `Bookings.BookingId`; NULL cho dòng điểm không gắn booking | Booking sinh ra điểm |
| `Points` | int | Không | — | Không có giá trị mặc định; ràng buộc `Points >= 0` | Điểm tích được từ booking |
| `RevokedPoints` | int | Không | — | 0; ràng buộc `0 <= RevokedPoints <= Points` | Điểm đã thu hồi do hoàn tiền |
| `EarnedAt` | timestamp with time zone | Không | — | Không có giá trị mặc định; ứng dụng ghi `DateTime.UtcNow` hoặc thời điểm thanh toán (dữ liệu mẫu); kiểu cột được xác lập trong migration `InitialCreate` (`20261106040650_InitialCreate`) | Thời điểm tích điểm; mốc xét cửa sổ 12 tháng |

Về cách đọc bảng: `Points` và `RevokedPoints` là hai số không âm lưu riêng để sổ điểm giữ được lịch sử tích điểm; điểm hiệu lực của một dòng là hiệu của chúng. Cấu hình `LoyaltyPointEntryConfiguration.cs` chỉ khai báo giá trị mặc định 0 cho `RevokedPoints`.

#### 4.3.11.3 Ràng buộc toàn vẹn

Các ràng buộc CHECK bảo vệ miền giá trị của từng cột, còn khóa ngoại bảo vệ quan hệ giữa sổ điểm với khách hàng và booking. Ràng buộc `CK_LoyaltyPointEntries_Revoked` khớp với phép `Math.Min(entry.Points, entry.RevokedPoints + revoke)` trong `BookingService.ReverseLoyaltyAsync`: dù hoàn tiền nhiều lần, điểm thu hồi của một dòng không vượt số điểm đã tích, nên điểm hiệu lực của dòng không bao giờ âm.

*Bảng 4.73. Ràng buộc toàn vẹn module Thành viên*

| Thực thể | Tên | Loại | Biểu thức | Mục đích / hành vi khi xóa |
|---|---|---|---|---|
| `MembershipTier` | `CK_MembershipTiers_Discount` | CHECK | `"DiscountPercent" BETWEEN 0 AND 100` | Giữ phần trăm giảm giá trong đoạn hợp lệ, tránh giá vé âm hoặc giảm quá 100% |
| `LoyaltyPointEntry` | `CK_LoyaltyPointEntries_Points` | CHECK | `"Points" >= 0` | Không cho phép dòng điểm âm |
| `LoyaltyPointEntry` | `CK_LoyaltyPointEntries_Revoked` | CHECK | `"RevokedPoints" >= 0 AND "RevokedPoints" <= "Points"` | Giữ điểm thu hồi trong khoảng từ 0 đến số điểm đã tích |
| `LoyaltyPointEntry` | FK `UserId` | Khóa ngoại | `UserId` → `Users(UserId)` | `DeleteBehavior.Cascade`: xóa người dùng thì xóa các dòng sổ điểm của người đó |
| `LoyaltyPointEntry` | FK `BookingId` | Khóa ngoại | `BookingId` → `Bookings(BookingId)` | `DeleteBehavior.Restrict`: không cho xóa booking còn dòng sổ điểm tham chiếu |

Ngoài ra, khóa ngoại `CustomerProfiles.TierId` tới `MembershipTiers` dùng `DeleteBehavior.Restrict` (xem Mục 4.3.4), nên một hạng đang được hồ sơ khách hàng sử dụng không thể bị xóa ở tầng CSDL.

#### 4.3.11.4 Chỉ mục

Chỉ mục duy nhất `UX_LoyaltyPointEntries_Booking` được khai báo trong `LoyaltyPointEntryConfiguration.cs` bằng `HasIndex(x => x.BookingId).IsUnique().HasFilter("\"BookingId\" IS NOT NULL").HasDatabaseName("UX_LoyaltyPointEntries_Booking")`. Chỉ mục này bảo đảm mỗi booking sinh tối đa một dòng điểm, nhờ đó việc cổng thanh toán gọi lại webhook không cộng điểm hai lần; điều kiện lọc cho phép nhiều dòng không gắn booking cùng tồn tại. Chỉ mục `IX_LoyaltyPointEntries_User_EarnedAt` phục vụ truy vấn lọc theo khách hàng và theo mốc `EarnedAt` trong `LoyaltyWindow.SumActiveAsync` và `LoyaltyWindow.GetStandingsAsync`. Chỉ mục duy nhất trên `TierName` được sinh trong migration `InitialCreate` với tên `IX_MembershipTiers_TierName`. Cả hai chỉ mục của `LoyaltyPointEntries` đã được tạo hoàn chỉnh ngay trong migration duy nhất `20261106040650_InitialCreate`.

*Bảng 4.74. Chỉ mục module Thành viên*

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc | Mục đích |
|---|---|---|---|---|---|
| `MembershipTier` | `IX_MembershipTiers_TierName` | Duy nhất | `TierName` | — | Không trùng tên hạng |
| `LoyaltyPointEntry` | `UX_LoyaltyPointEntries_Booking` | Duy nhất (có điều kiện lọc) | `BookingId` | `"BookingId" IS NOT NULL` | Mỗi booking một dòng điểm, chống cộng đôi |
| `LoyaltyPointEntry` | `IX_LoyaltyPointEntries_User_EarnedAt` | Thường | `UserId`, `EarnedAt` | — | Tính điểm hiệu lực theo khách hàng trong cửa sổ 12 tháng |

### 4.3.12 Module Cấu hình rạp

Module Cấu hình rạp chỉ có một thực thể là `CinemaSettings` (bảng `CinemaSettings`). Bảng này lưu thông tin chung của rạp và các tham số vận hành của nghiệp vụ đặt vé. Bảng không có khóa ngoại và không bị bảng nào tham chiếu; liên hệ với các module khác là liên hệ về logic: `BookingService` đọc các tham số điểm (`PointsPerAmount`), hoàn tiền (`CancelRefundPercent`, `CancelDeadlineHours`), dừng bán (`StopSellingMinutesBefore`), giữ ghế (`HoldDurationMinutes`) và soát vé (`CheckInOpenMinutes`); `SeatLayoutRules` dùng `SeatGapRuleMode`; `SettingsService` đọc và cập nhật bản ghi.

#### 4.3.12.1 Sơ đồ ERD của module

Hình 4.35 chỉ có một thực thể `CinemaSettings` với đủ 28 thuộc tính của Bảng 4.75 (bao gồm 16 thuộc tính cơ sở và 12 thuộc tính tham số vận hành mở rộng) và không có quan hệ nào với thực thể khác. Khóa chính `Id` luôn bằng 1 (xem Mục 4.3.12.3).

![Hình 4.35. ERD module Cấu hình rạp](hinh/4.35.png)

*Hình 4.35. ERD module Cấu hình rạp*

#### 4.3.12.2 Mô tả thực thể

Bảng `CinemaSettings` là bảng đơn bản ghi: toàn hệ thống chỉ có một dòng cấu hình với `Id = 1`, được khởi tạo bằng dữ liệu tĩnh `HasData` (xem Mục 4.3.16). Các tham số có hai nguồn giá trị mặc định: mặc định ở cột CSDL (khai báo trong cấu hình EF hoặc migration) và mặc định ở tầng C# (khởi tạo thuộc tính của lớp `CinemaSettings`). Một số cột chỉ có mặc định ở tầng C#; cột "Giá trị mặc định / ghi chú" của Bảng 4.75 phân biệt hai nguồn này.

*Bảng 4.75. Thực thể CinemaSettings (bảng CinemaSettings)*

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `Id` | int | Không | PK | Không tự sinh (`ValueGeneratedNever`); luôn bằng 1 | Mã định danh của bản ghi cấu hình duy nhất |
| `Name` | varchar(150) | Không | — | — | Tên rạp |
| `Address` | varchar(255) | Không | — | — | Địa chỉ rạp |
| `Phone` | varchar(20) | Có | — | — | Số điện thoại liên hệ |
| `Email` | varchar(150) | Có | — | — | Email của rạp (dùng làm nơi nhận thông báo vận hành, ví dụ khi cần hoàn tiền thủ công) |
| `TaxCode` | varchar(50) | Có | — | — | Mã số thuế |
| `LogoUrl` | varchar(255) | Có | — | — | Đường dẫn ảnh logo |
| `OpeningTime` | time | Có | — | — | Giờ mở cửa |
| `ClosingTime` | time | Có | — | — | Giờ đóng cửa |
| `UpdatedAt` | timestamp without time zone | Có | — | — | Thời điểm cập nhật cấu hình gần nhất |
| `PointsPerAmount` | numeric(18,2) | Không | — | 10000 (cột CSDL và C#) | Số tiền thanh toán tương ứng một điểm thành viên |
| `CancelRefundPercent` | numeric(5,2) | Không | — | 0 (cột CSDL và C#) | Phần trăm hoàn tiền khi khách hủy booking đã thanh toán; bằng 0 nghĩa là rạp không hoàn tiền |
| `CancelDeadlineHours` | int | Không | — | 2 (chỉ ở C#; dòng dữ liệu tĩnh có giá trị 2) | Số giờ tối thiểu trước giờ chiếu mà khách còn được hủy và hoàn tiền |
| `StopSellingMinutesBefore` | int | Không | — | 0 (chỉ ở C#; dòng dữ liệu tĩnh có giá trị 0) | Số phút trước giờ chiếu thì dừng bán vé trực tuyến; 0 nghĩa là không dừng bán sớm |
| `HoldDurationMinutes` | int | Không | — | 15 (`CinemaSettings.DefaultHoldDurationMinutes`; chỉ ở C#; dòng dữ liệu tĩnh có giá trị 15) | Thời gian giữ ghế tính bằng phút |
| `CheckInOpenMinutes` | int | Không | — | 30 (`CinemaSettings.DefaultCheckInOpenMinutes`; chỉ ở C#; dòng dữ liệu tĩnh có giá trị 30) | Số phút trước giờ chiếu thì bắt đầu cho soát vé |
| `SeatGapRuleMode` | varchar(20) | Không | — | `Strict` (`SeatGapRuleMode.Strict`; mặc định ở cả cột CSDL và C#) | Chế độ luật ghế lẻ (`Strict`, `AllowEdge`, `Off`) |
| `RoomBufferMinutes` | int | Không | — | 15 (mặc định ở cả cột CSDL và C#; dòng seed có giá trị 15) | Thời gian dọn phòng/giãn cách (phút) cộng vào `EndTime = StartTime + DurationMinutes + RoomBufferMinutes` |
| `MaxSeatsPerBooking` | int | Không | — | 10 (mặc định ở cả cột CSDL và C#; dòng seed có giá trị 10) | Số lượng ghế tối đa cho phép đặt trong một giao dịch (áp dụng cho cả luồng Online và Quầy) |
| `MaxComboQuantity` | int | Không | — | 10 (mặc định ở cả cột CSDL và C#; dòng seed có giá trị 10) | Số lượng tối đa cho mỗi loại combo trong một giao dịch đặt vé |
| `LoyaltyWindowMonths` | int | Không | — | 12 (mặc định ở cả cột CSDL và C#; dòng seed có giá trị 12) | Cửa sổ trượt (tháng) dùng tính tổng điểm hiệu lực và xác định hạng thành viên của khách hàng |
| `MovieEndGraceDays` | int | Không | — | 1 (mặc định ở cả cột CSDL và C#; dòng seed có giá trị 1) | Số ngày đệm sau suất chiếu cuối cùng trước khi `ShowtimeLifecycleJob` tự động chuyển phim sang `Ended` |
| `ShowtimeLookaheadDays` | int | Không | — | 7 (mặc định ở cả cột CSDL và C#; dòng seed có giá trị 7) | Số ngày suất chiếu hiển thị tối đa trong tương lai cho khách hàng tra cứu và trợ lý AI trả lời |
| `SchedulerStepMinutes` | int | Không | — | 5 (mặc định ở cả cột CSDL và C#; dòng seed có giá trị 5) | Bước nhảy thời gian (phút) khi thuật toán tự động xếp lịch tìm kiếm khung giờ bắt đầu suất chiếu |
| `SchedulerMinSameMovieGapMinutes` | int | Không | — | 60 (mặc định ở cả cột CSDL và C#; dòng seed có giá trị 60) | Giãn cách tối thiểu (phút) giữa 2 suất chiếu của cùng một phim khi tự động xếp lịch |
| `SchedulerHeatLookbackDays` | int | Không | — | 7 (mặc định ở cả cột CSDL và C#; dòng seed có giá trị 7) | Số ngày tra cứu lịch sử doanh số vé phục vụ đánh giá độ hot của phim |
| `SchedulerHeatSalesWeight` | double precision | Không | — | 0.7 (mặc định ở cả cột CSDL và C#; dòng seed có giá trị 0.7) | Trọng số của thành phần vé đã bán trong công thức tính độ hot (`Heat`) |
| `SchedulerHeatRatingWeight` | double precision | Không | — | 0.3 (mặc định ở cả cột CSDL và C#; dòng seed có giá trị 0.3) | Trọng số của điểm đánh giá người xem trong công thức tính độ hot (`Heat`) |
| `SchedulerMaxRating` | double precision | Không | — | 5.0 (mặc định ở cả cột CSDL và C#; dòng seed có giá trị 5.0) | Mẫu số chuẩn hóa điểm đánh giá (thang điểm 5) trong công thức tính độ hot |

Cách các tham số được dùng trong code: số điểm tích được là `⌊FinalAmount / PointsPerAmount⌋`; điều kiện hoàn tiền trong `BookingService.CalculateRefundEligibility` yêu cầu `CancelRefundPercent > 0` và thời điểm hủy không muộn hơn `StartTime − CancelDeadlineHours` giờ, khi đó số tiền hoàn là `Amount × CancelRefundPercent / 100` làm tròn đến đơn vị đồng; thời hạn giữ ghế là `HoldDurationMinutes` phút nhưng không vượt quá giờ bắt đầu suất chiếu; `MaxSeatsPerBooking` và `MaxComboQuantity` được kiểm tra tập trung trong `BookingService.EnsureWithinBookingLimitsAsync`; `LoyaltyWindowMonths` xác định mốc cắt sổ điểm trong `LoyaltyWindow`; `RoomBufferMinutes` cộng vào thời lượng phim để xác định `EndTime` chống xung đột phòng chiếu; `MovieEndGraceDays` điều khiển việc tự động đóng trạng thái phim trong tác vụ nền `ShowtimeLifecycleJob`; `ShowtimeLookaheadDays` giới hạn khoảng ngày tra cứu lịch chiếu công khai; nhóm tham số `Scheduler*` cấu hình các trọng số và bước nhảy cho thuật toán lập lịch tự động `ShowtimeScheduler`; còn `SeatGapRuleMode` quyết định luật chặn ghế lẻ khi chọn ghế (xem Mục 4.6.1).

#### 4.3.12.3 Ràng buộc toàn vẹn

Ràng buộc `CK_CinemaSettings_Id` ép cột `Id` luôn bằng 1, cùng với cấu hình `ValueGeneratedNever`, bảo đảm bảng không thể có nhiều hơn một dòng cấu hình có khóa khác 1. Bảng không có khóa ngoại.

*Bảng 4.76. Ràng buộc toàn vẹn module Cấu hình rạp*

| Thực thể | Tên | Loại | Biểu thức | Mục đích |
|---|---|---|---|---|
| `CinemaSettings` | `CK_CinemaSettings_Id` | CHECK | `"Id" = 1` | Chỉ cho phép một bản ghi cấu hình duy nhất |

#### 4.3.12.4 Chỉ mục

Module không khai báo chỉ mục riêng ngoài khóa chính. Khóa chính `PK_CinemaSettings` trên cột `Id` là chỉ mục duy nhất của bảng; vì bảng chỉ có một dòng nên không cần chỉ mục bổ sung.

*Bảng 4.77. Chỉ mục module Cấu hình rạp*

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| `CinemaSettings` | `PK_CinemaSettings` | Khóa chính | `Id` | — |

### 4.3.13 Kiểu liệt kê

Hệ thống định nghĩa 10 kiểu liệt kê trong `backend/Cinema.Domain/Enums`. Mọi kiểu đều được cấu hình lưu dạng chuỗi (`HasConversion<string>()`) nên giá trị trong CSDL là tên thành viên của enum, không phải số thứ tự. Tám trong mười kiểu có ràng buộc CHECK giới hạn cột chỉ nhận các giá trị hợp lệ; hai kiểu `SeatGapRuleMode` và `VerificationTokenPurpose` không có CHECK tương ứng, việc kiểm tra giá trị chỉ do tầng ứng dụng đảm nhiệm.

*Bảng 4.78. Các kiểu liệt kê*

| Kiểu | Giá trị | Cột lưu | Độ dài cột | Ràng buộc CHECK |
|---|---|---|---|---|
| `BookingChannel` | `Online`, `Counter` | `Bookings.Channel` | varchar(20) | `CK_Bookings_Channel` |
| `BookingStatus` | `Pending`, `Confirmed`, `Cancelled`, `Expired` | `Bookings.Status` | varchar(20) | `CK_Bookings_Status` |
| `Gender` | `Male`, `Female`, `Other` | `Users.Gender` | varchar(10) | `CK_Users_Gender` |
| `MovieStatus` | `ComingSoon`, `NowShowing`, `Ended` | `Movies.Status` | varchar(20) | `CK_Movies_Status` |
| `PaymentMethod` | `VNPay`, `Momo`, `Cash`, `Stripe` | `Payments.PaymentMethod` | varchar(50) | `CK_Payments_Method` |
| `PaymentStatus` | `Pending`, `Success`, `Failed`, `Refunded` | `Payments.Status` | varchar(20) | `CK_Payments_Status` |
| `SeatGapRuleMode` | `Strict`, `AllowEdge`, `Off` | `CinemaSettings.SeatGapRuleMode` | varchar(20) | Không có |
| `ShowtimeSeatStatus` | `Available`, `Held`, `Booked` | `ShowtimeSeats.Status` | varchar(20) | `CK_ShowtimeSeats_Status` |
| `ShowtimeStatus` | `Scheduled`, `Cancelled`, `Completed` | `Showtimes.Status` | varchar(20) | `CK_Showtimes_Status` |
| `VerificationTokenPurpose` | `EmailConfirmation`, `PasswordReset` | `UserVerificationTokens.Purpose` | varchar(30) | Không có |

### 4.3.14 Vòng đời trạng thái

Các chuyển trạng thái dưới đây được xác định bằng cách đọc các service, không suy ra từ định nghĩa enum. Mỗi hình mô tả trạng thái khởi tạo, các trạng thái và những chuyển trạng thái có thật trong code; trạng thái nào không có chuyển đi trong code được gọi là trạng thái cuối. Các mục dưới đây đều ghi hàm thực hiện việc chuyển đổi.

#### 4.3.14.1 Vòng đời Booking

Booking bắt đầu ở trạng thái `Pending` khi `BookingService.CreateHoldBookingAsync` tạo đơn giữ ghế; hàm này được `HoldSeatsAsync` (khách đặt trực tuyến) và `CounterHoldSeatsAsync` (nhân viên bán tại quầy) gọi. Từ `Pending`, booking có thể sang ba trạng thái khác.

Sang `Confirmed` khi thanh toán thành công: `ConfirmBookingAndGenerateTicketsAsync` đổi trạng thái, chuyển các ghế đang giữ sang `Booked`, tạo vé kèm mã QR và ghi sổ điểm. Hàm này được `PaymentService.ApplyPaymentResultAsync` gọi khi cổng thanh toán báo thành công và được `CounterCheckoutAsync` gọi khi thu tiền mặt tại quầy. Hàm có tính lũy đẳng: nếu booking đã `Confirmed` thì trả về mà không xử lý lại.

Sang `Expired` khi quá hạn giữ ghế: `SeatExpirationService.ExpireDueBookingsAsync` chọn các booking `Pending` có `ExpiresAt` nhỏ hơn thời điểm hiện tại, nhả ghế và đặt các payment `Pending` của booking thành `Failed`. Hàm này do `ReleaseExpiredSeatsJob` gọi theo cron `ReleaseExpiredSeatsCron` (mặc định `* * * * *`, mỗi phút).

Sang `Cancelled` qua bốn đường: (a) khách hủy khi còn `Pending` (`CancelBookingAsync`, nhánh `Pending`, gọi `ReleaseAndCancelBookingAsync`); (b) thanh toán thất bại và đã quá hạn giữ ghế (`ProcessPaymentFailureAsync` gọi `ReleaseAndCancelBookingAsync`; nếu hạn giữ còn thì hàm giữ nguyên `Pending` để khách thanh toán lại); (c) thanh toán thành công nhưng không còn ghế đang giữ (`ConfirmBookingAndGenerateTicketsAsync` đặt `Cancelled` rồi ném `SeatsNoLongerHeldException`); (d) rạp hủy suất chiếu (`ShowtimeService.DeleteShowtimeAsync`).

Từ `Confirmed`, booking chỉ sang `Cancelled`, qua hai đường: khách tự hủy trong `CancelBookingAsync` (nhánh `Confirmed`) hoặc rạp hủy suất chiếu trong `ShowtimeService.DeleteShowtimeAsync`. Việc khách tự hủy yêu cầu chưa có vé nào đã check-in, có payment `Success` không phải `Cash` và còn trong thời hạn hoàn tiền; booking thanh toán tiền mặt tại quầy bị từ chối ở nhánh này (`Rule_BookingCancelAtCounter`) nên chỉ kết thúc ở `Cancelled` qua việc hủy suất. Hai trạng thái `Expired` và `Cancelled` là trạng thái cuối: `ConfirmBookingAndGenerateTicketsAsync` ném `SeatsNoLongerHeldException` với booking ở hai trạng thái này, còn `CancelBookingAsync` từ chối với booking không ở `Pending` hoặc `Confirmed`.

![Hình 4.36. Máy trạng thái Booking](hinh/4.36.png)

*Hình 4.36. Máy trạng thái Booking*

#### 4.3.14.2 Vòng đời ShowtimeSeat

Mỗi ghế của một suất chiếu bắt đầu ở trạng thái `Available` khi suất chiếu được tạo (`ShowtimeService.GenerateShowtimeSeatsAsync` và `CreateShowtimesBatchAsync`); giá trị này cũng là mặc định của cột `ShowtimeSeats.Status`.

Từ `Available` sang `Held` khi khách hoặc nhân viên giữ ghế: `BookingService.CreateHoldBookingAsync` cập nhật đồng thời các ghế được chọn bằng một câu lệnh có điều kiện; ghế cũng được giữ lại nếu đang `Held` bởi cùng người nhưng đã quá `HeldUntil`. Nếu số ghế cập nhật được ít hơn số ghế yêu cầu thì hàm hoàn tác giao dịch và ném `SeatConflictException`.

Từ `Held` sang `Booked` khi thanh toán thành công trong `ConfirmBookingAndGenerateTicketsAsync`. Từ `Held` về `Available` qua bốn đường: nhả ghế quá hạn (`SeatExpirationService.ExpireDueBookingsAsync` và `ReleaseStaleHeldSeatsAsync`, cả hai gọi qua `ReleaseExpiredSeatsJob`); hủy booking còn `Pending` (`ReleaseBookingSeatsAsync` do `ReleaseAndCancelBookingAsync` gọi); và khi rạp hủy suất, `ShowtimeService.DeleteShowtimeAsync` đặt mọi ghế `Held` của suất về `Available`.

Từ `Booked` về `Available` khi khách hủy booking đã `Confirmed` và được hoàn tiền: `ReleaseConfirmedBookingSeatsAsync` trong `CancelBookingAsync` nhả các ghế `Booked` của vé thuộc booking. Đây là chuyển trạng thái duy nhất rời khỏi `Booked`; khi rạp hủy suất, các ghế `Booked` của suất đó không bị đổi trạng thái vì `DeleteShowtimeAsync` chỉ xử lý ghế `Held`. Code không có chuyển trực tiếp từ `Booked` sang `Held`.

![Hình 4.37. Máy trạng thái ShowtimeSeat](hinh/4.37.png)

*Hình 4.37. Máy trạng thái ShowtimeSeat*

#### 4.3.14.3 Vòng đời Payment

Payment có hai cách khởi tạo. Với thanh toán trực tuyến, `BookingService.CheckoutAsync` tạo payment ở trạng thái `Pending` sau khi đặt các payment `Pending` cũ của cùng booking thành `Failed`. Với thanh toán tiền mặt tại quầy, `CounterCheckoutAsync` tạo payment với phương thức `Cash` và trạng thái `Success` ngay, kèm `AmountReceived` và `ChangeGiven`, nên payment tiền mặt không có trạng thái `Pending`.

Từ `Pending`, kết quả của cổng thanh toán được áp dụng trong `PaymentService.ApplyPaymentResultAsync`: payment sang `Success` (ghi `PaidAt`, mã giao dịch của cổng, rồi xác nhận booking) hoặc sang `Failed` (rồi gọi `ProcessPaymentFailureAsync`). Payment `Pending` còn sang `Failed` khi khách bắt đầu một lần thanh toán mới (`CheckoutAsync`), khi booking bị nhả hoặc hết hạn (`ReleaseAndCancelBookingAsync`, `SeatExpirationService.ExpireDueBookingsAsync`) và khi rạp hủy suất (`DeleteShowtimeAsync`, đối với booking `Pending`). Hàm `PaymentService.IsAlreadyProcessed` coi payment `Failed` là chưa xử lý xong nếu gọi lại báo thành công, nên một payment `Failed` có thể chuyển sang `Success` khi cổng gửi kết quả thành công muộn; payment đã `Success` hoặc `Refunded` thì bị bỏ qua.

Từ `Success`, payment sang `Refunded` qua ba đường, đều ghi `RefundedAmount`, `RefundedAt` và `RefundTransactionId`: khách hủy booking trong thời hạn hoàn tiền (`CancelBookingAsync`, số tiền hoàn theo `CancelRefundPercent`); rạp hủy suất (`ShowtimeRefundJob`, hoàn toàn bộ `Amount` qua cổng); và tự động hoàn tiền khi ghế đã hết hạn giữ trước khi thanh toán được xác nhận (`PaymentService.HandleSeatsExpiredAfterPaymentAsync`). Payment tiền mặt của booking thuộc suất bị hủy không được đổi sang `Refunded` trong code mà được đếm là cần hoàn tiền thủ công (`manualRefundCount`). Code không có chuyển từ `Refunded` sang trạng thái khác.

![Hình 4.38. Máy trạng thái Payment](hinh/4.38.png)

*Hình 4.38. Máy trạng thái Payment*

#### 4.3.14.4 Vòng đời Showtime

Suất chiếu bắt đầu ở trạng thái `Scheduled` khi được tạo (`ShowtimeService.CreateShowtimeAsync` và `CreateShowtimesBatchAsync`); đây cũng là mặc định của cột `Showtimes.Status`. Có hai chuyển trạng thái.

Từ `Scheduled` sang `Completed` khi suất đã kết thúc: `ShowtimeService.CompleteEndedShowtimesAsync` đặt `Completed` cho các suất `Scheduled` có `EndTime` nhỏ hơn thời điểm hiện tại. Hàm này được `ShowtimeLifecycleJob` gọi theo cron `ShowtimeLifecycleCron` (mặc định `* * * * *`).

Từ `Scheduled` sang `Cancelled` khi rạp hủy suất: `ShowtimeService.DeleteShowtimeAsync` yêu cầu suất đang `Scheduled` (`EnsureScheduled`) và chưa bắt đầu (`StartTime` lớn hơn thời điểm hiện tại), rồi hủy các booking `Pending` và `Confirmed` của suất, thu hồi điểm của booking đã `Confirmed`, nhả ghế `Held` và xếp công việc hoàn tiền. `Completed` và `Cancelled` là trạng thái cuối vì các thao tác sửa suất đều yêu cầu `Scheduled`.

![Hình 4.39. Máy trạng thái Showtime](hinh/4.39.png)

*Hình 4.39. Máy trạng thái Showtime*

#### 4.3.14.5 Vòng đời Movie

Trạng thái phim có hai cơ chế chuyển: do quản trị viên đặt và do công việc nền. Khi tạo phim (`MovieService.CreateMovieAsync`) và khi cập nhật phim (`UpdateMovieAsync`), trạng thái được lấy trực tiếp từ yêu cầu; bộ kiểm tra `MovieValidators` chỉ yêu cầu giá trị thuộc ba tên của `MovieStatus`, nên về mặt thao tác của quản trị viên mọi chuyển giữa ba trạng thái đều được phép. Mặc định của cột `Movies.Status` là `ComingSoon`.

Chuyển `ComingSoon` sang `NowShowing` không có cơ chế tự động trong code: không có job hay service nào đặt `NowShowing`; chuyển này chỉ xảy ra khi quản trị viên cập nhật phim.

Chuyển `NowShowing` sang `Ended` có hai cơ chế. Thứ nhất, `ShowtimeService.EndMoviesWithoutUpcomingShowtimesAsync` (do `ShowtimeLifecycleJob` gọi sau khi hoàn tất suất chiếu) đặt `Ended` cho phim `NowShowing` đã từng có suất không bị hủy và không còn suất `Scheduled` nào có `EndTime` lớn hơn mốc `ShowtimeLifecycle.MovieEndCutoff(now)`, tức thời điểm hiện tại lùi `MovieEndGraceDays = 1` ngày. Thứ hai, `MovieService.DeleteMovieAsync` xóa mềm phim đã có suất chiếu bằng cách đặt `Ended` (phim chưa có suất thì bị xóa hẳn). Phim `Ended` không thể được xếp suất mới (`Rule_MovieEndedCannotCreateShowtime`), nhưng `Ended` không phải trạng thái cuối tuyệt đối vì quản trị viên vẫn có thể cập nhật lại trạng thái.

![Hình 4.40. Máy trạng thái Movie](hinh/4.40.png)

*Hình 4.40. Máy trạng thái Movie*

### 4.3.15 Migration

Hệ thống dùng EF Core Code First; các migration nằm trong `backend/Cinema.Infrastructure/Migrations`. Hiện tại, toàn bộ lược đồ cơ sở dữ liệu đã được tái cấu trúc và khởi tạo hoàn chỉnh từ đầu trong một migration duy nhất:

- `InitialCreate` (`20261106040650_InitialCreate.cs`, ngày 06/11/2026): khởi tạo toàn bộ cấu trúc cơ sở dữ liệu gồm 30 bảng thực thể (bao gồm đầy đủ bảng sổ điểm `LoyaltyPointEntries`, toàn bộ 28 cột của `CinemaSettings` với 12 tham số vận hành mở rộng, cột `SeatGapRuleMode` và các tham số thời gian), 36 khóa ngoại bảo đảm toàn vẹn tham chiếu, 48 chỉ mục (trong đó có chỉ mục duy nhất có lọc `UX_LoyaltyPointEntries_Booking` trên `BookingId IS NOT NULL` và chỉ mục `IX_LoyaltyPointEntries_User_EarnedAt`), 31 ràng buộc kiểm tra CHECK cho các trường dữ liệu (bao gồm `CK_Reviews_Rating` giới hạn thang điểm 1–5, `CK_CinemaSettings_Id`, `CK_Combos_Price`, `CK_LoyaltyPointEntries_Points`, `CK_LoyaltyPointEntries_Revoked`, check constraint cho 8/10 enum); đồng thời chèn dữ liệu tĩnh ban đầu (`Roles`, `Positions`, `AgeRatings`, `ScreenTypes`, `SeatTypes`, `Languages`, `Genres`, `MembershipTiers`, và bản ghi cấu hình rạp duy nhất `CinemaSettings` với `Id = 1`, `PointsPerAmount = 10000`, `CancelRefundPercent = 0`, `CancelDeadlineHours = 2`, `StopSellingMinutesBefore = 0`, `HoldDurationMinutes = 15`, `CheckInOpenMinutes = 30`, `SeatGapRuleMode = "Strict"`, cùng các giá trị mặc định cho 12 tham số vận hành mở rộng: `RoomBufferMinutes = 15`, `MaxSeatsPerBooking = 10`, `MaxComboQuantity = 10`, `LoyaltyWindowMonths = 12`, `MovieEndGraceDays = 1`, `ShowtimeLookaheadDays = 7`, `SchedulerStepMinutes = 5`, `SchedulerMinSameMovieGapMinutes = 60`, `SchedulerHeatLookbackDays = 7`, `SchedulerHeatSalesWeight = 0.7`, `SchedulerHeatRatingWeight = 0.3`, `SchedulerMaxRating = 5.0`). Cấu hình này khớp hoàn toàn với `AppDbContextModelSnapshot.cs`.

Về việc nạp dữ liệu sổ điểm (backfill) cho các đơn hàng cũ trước khi có bảng sổ điểm: Hệ thống thực hiện nạp dữ liệu khởi tạo mẫu trong `DbInitializer.SeedTestAccountsAsync` (tạo các dòng điểm khởi tạo theo từng tài khoản mẫu). Đối với dữ liệu lịch sử sản xuất, câu lệnh SQL nạp bù được thực hiện an toàn từ các booking đã xác nhận (`INSERT INTO "LoyaltyPointEntries" ("UserId", "BookingId", "Points", "RevokedPoints", "EarnedAt") SELECT "UserId", "BookingId", FLOOR("FinalAmount" / 10000), 0, "CreatedAt" FROM "Bookings" WHERE "Status" = 'Confirmed' AND "UserId" IS NOT NULL;`), hoặc tự động đồng bộ qua phương thức `LoyaltyExpiryService.RecalculateAllAsync`. Do `RecalculateAllAsync` đặt `CustomerProfile.Points` bằng tổng điểm trong sổ điểm của khách hàng trong cửa sổ 12 tháng gần nhất, việc nạp trước sổ điểm bảo đảm quyền lợi điểm tích lũy và hạng thành viên của khách hàng cũ khi tác vụ nền `ExpireLoyaltyPointsJob` vận hành định kỳ.

Cách áp dụng migration: lệnh `npm run db:update` (định nghĩa trong `package.json`) chạy `dotnet ef database update` với dự án `backend/Cinema.Infrastructure` và dự án khởi động `backend/Cinema.Api`. Các lệnh liên quan trong `package.json` gồm `db:migration:add` (tạo migration), `db:migration:remove`, `db:migration:list`, `db:migration:script` (xuất script idempotent ra `backend/migration.sql`), `db:drop`, `db:reset` (xóa rồi cập nhật lại) và `db:info`. Ngoài ra, `Program.cs` đọc cờ `RunMigrationsOnStartup` (giá trị mặc định trong code là `true`): khi cờ bật, ứng dụng gọi `Database.MigrateAsync()` rồi `DbInitializer.SeedAsync` lúc khởi động. Tệp `appsettings.json` đặt cờ này là `false`, còn `docker-compose.yml` đặt qua biến môi trường `RUN_MIGRATIONS_ON_STARTUP` với mặc định `true`.

### 4.3.16 Dữ liệu khởi tạo

Dữ liệu khởi tạo gồm hai lớp. Lớp thứ nhất là dữ liệu tĩnh khai báo bằng `HasData` trong `SeedData.Apply` và được ghi vào CSDL qua migration. Lớp thứ hai là dữ liệu động do `DbInitializer.SeedAsync` tạo sau khi migration chạy, phụ thuộc thời điểm hiện tại. Cả hai lớp được chạy khi cờ `RunMigrationsOnStartup` bật (xem Mục 4.3.15).

**Dữ liệu tĩnh (`SeedData`).** Danh mục gồm 3 vai trò (`Customer`, `Admin`, `Employee`), 3 chức vụ (`Quản lý rạp`, `Trưởng ca`, `Nhân viên bán vé`), 3 loại màn hình (`2D`, `3D`, `IMAX`), 3 loại ghế (`Standard` hệ số giá 1,0; `VIP` 1,5; `Couple` 2,0), 10 thể loại, 4 ngôn ngữ, 6 nhãn độ tuổi (`P`, `K`, `T13`, `T16`, `T18`, `C`) và 5 combo (giá từ 65.000 đến 189.000 đồng). Dữ liệu tài khoản gồm một người dùng quản trị với mật khẩu đã băm và một bản ghi nhân viên gắn với người dùng đó ở chức vụ quản lý. Báo cáo không ghi mật khẩu.

Bốn hạng thành viên được seed như sau.

| `TierId` | `TierName` | `MinPoints` | `DiscountPercent` |
|---|---|---|---|
| 1 | Thành viên | 0 | 0 |
| 2 | Bạc | 500 | 5 |
| 3 | Vàng | 2000 | 10 |
| 4 | Kim Cương | 5000 | 15 |

Dòng `CinemaSettings` duy nhất (`Id = 1`) có `Name = "Cinema"`, `Address = "Đang cập nhật"`, giờ mở cửa 08:00, giờ đóng cửa 23:59 và `PointsPerAmount = 10000`; các tham số còn lại lấy giá trị mặc định được seed trực tiếp trong migration (xem Bảng 4.75), tức `CancelRefundPercent = 0`, `CancelDeadlineHours = 2`, `StopSellingMinutesBefore = 0`, `HoldDurationMinutes = 15`, `CheckInOpenMinutes = 30`, `SeatGapRuleMode = Strict`, `RoomBufferMinutes = 15`, `MaxSeatsPerBooking = 10`, `MaxComboQuantity = 10`, `LoyaltyWindowMonths = 12`, `MovieEndGraceDays = 1`, `ShowtimeLookaheadDays = 7`, `SchedulerStepMinutes = 5`, `SchedulerMinSameMovieGapMinutes = 60`, `SchedulerHeatLookbackDays = 7`, `SchedulerHeatSalesWeight = 0.7`, `SchedulerHeatRatingWeight = 0.3`, `SchedulerMaxRating = 5.0`.

Dữ liệu về phim và phòng gồm 15 đạo diễn, 38 diễn viên, 19 phim (8 phim `NowShowing`, 10 phim `Ended`, 1 phim `ComingSoon`), 51 dòng liên kết phim – thể loại và 44 dòng diễn viên của phim. Có 4 phòng chiếu với tổng 200 ghế: `Cinema Hall 01` và `Cinema Hall 02` (màn hình 2D, 5 hàng × 10 cột, 50 ghế mỗi phòng), `3D Experience Hall 03` (màn hình 3D, 4 hàng × 10 cột, 40 ghế) và `IMAX Grand Theatre 04` (màn hình IMAX, 6 hàng × 10 cột, 60 ghế). Loại ghế được gán theo hàng, hàng cuối của mỗi phòng là ghế đôi (`Couple`); ghế đôi cùng cặp dùng chung một giá trị `CoupleId` cố định sinh theo phòng, hàng và số cặp.

**Dữ liệu động (`DbInitializer.SeedAsync`).** Mọi bước đều idempotent: chạy lại không tạo bản ghi trùng. Thứ tự thực hiện như sau.

Thứ nhất, `SeedTestAccountsAsync` tạo 8 tài khoản khách hàng kiểm thử và 4 tài khoản nhân viên. Các khách hàng gồm bốn người ứng với bốn hạng (điểm khởi tạo lần lượt 300, 800, 2.500 và 6.000; tổng chi tiêu lần lượt 1.000.000, 3.500.000, 12.000.000 và 30.000.000 đồng), một khách mới chưa có điểm, một tài khoản bị khóa (`IsActive = false`, 100 điểm), một tài khoản chưa xác thực email và một tài khoản đăng ký bằng Google. Bốn nhân viên gồm nhân viên bán vé, quản lý, trưởng ca và một nhân viên đã bị vô hiệu hóa. Hạng của mỗi khách được chọn là hạng có `MinPoints` lớn nhất không vượt điểm khởi tạo. Với khách có điểm khởi tạo lớn hơn 0, `DbInitializer` tạo thêm một dòng sổ điểm không gắn booking với `Points` bằng điểm khởi tạo và `EarnedAt` là thời điểm seed. Bước này cũng đặt lại mật khẩu quản trị cho khớp tài liệu nếu băm hiện tại không khớp.

Thứ hai, `SeedVerificationTokensAsync` tạo 4 mã xác thực mẫu (lưu dạng băm SHA-256): một mã xác nhận email còn hạn, một mã đặt lại mật khẩu còn hạn, một mã đặt lại mật khẩu đã hết hạn và một mã xác nhận email đã sử dụng, phục vụ kiểm thử các luồng xác thực.

Thứ ba, `SeedExtraRoomsAsync` tạo hai phòng đặc biệt: `Premium Lounge 05` (màn hình 3D, 5 hàng × 13 cột, có lối đi ở cột 7, ghế C3 bị đánh dấu không hoạt động, nên `TotalSeats` được gán là 59 theo cách đếm ghế hoạt động không phải lối đi) và `Cinema Hall 06 (Bảo trì)` (màn hình 2D, 4 hàng × 10 cột, `IsActive = false`, không được xếp suất).

Thứ tư, nếu `seedDemoScenarios` bật và chưa có booking nào có mã bắt đầu bằng `BKD` (`BookingScenarioSeeder.CodePrefix`), `SeedScenariosAsync` chạy trong một giao dịch: tạo các suất đặc biệt (`ShowtimeSeeder.CreateSpecialsAsync`), bổ sung suất chiếu cho hôm nay và 7 ngày tới (`TopUpFutureAsync`, `FutureDays = 7`), tạo suất của 7 ngày gần nhất để có dữ liệu báo cáo (`SeedPastAsync`, `PastDays = 7`) rồi tạo kịch bản booking (`BookingScenarioSeeder.SeedAsync`). Nếu có lỗi thì giao dịch bị hoàn tác và chỉ còn bước bổ sung suất tương lai; nếu đã có dữ liệu kịch bản hoặc tắt seed demo thì chỉ chạy `TopUpFutureAsync`. Mỗi phòng hoạt động có từ 3 đến 4 suất mỗi ngày, cách nhau `BufferMinutes = 15` phút. Năm suất đặc biệt phục vụ kiểm thử gồm: suất đang chiếu (check-in đang mở), suất bắt đầu sau 20 phút, suất bắt đầu sau 90 phút (nằm trong ngưỡng `CancelDeadlineHours` nên không được hoàn tiền), suất hết vé 100% vào 19:00 ngày mai, và suất bị rạp hủy vào 15:00 ngày kia (trạng thái `Cancelled`).

`BookingScenarioSeeder` dùng bộ sinh số ngẫu nhiên có hạt giống cố định (20261028) nên kết quả lặp lại được. Kịch bản bao phủ cả bốn trạng thái booking (`Pending`, `Confirmed`, `Cancelled`, `Expired`), cả hai kênh (`Online`, `Counter`), các phương thức thanh toán, các tình huống hoàn tiền (khách hủy, thanh toán trễ, rạp hủy suất hoàn tự động hoặc thủ công), vé đã check-in và chưa check-in, mỗi khách có sẵn `ReviewReadyPerUser = 3` booking đã check-in chưa đánh giá, và các đánh giá mẫu (`SeedReviewsAsync`). Các kế hoạch được áp dụng theo thứ tự thời gian tạo để giảm giá và hạng khớp với thời điểm đặt. Với booking của khách hàng, seeder tính giảm giá chỉ trên tiền vé (tổng giá ghế, không gồm combo) theo `DiscountPercent` của hạng hiện tại trong hồ sơ, làm tròn đến đơn vị đồng theo `MidpointRounding.AwayFromZero`. Với booking `Confirmed` có hồ sơ khách hàng, seeder ghi một dòng sổ điểm gắn booking với số điểm `⌊FinalAmount / PointsPerAmount⌋` (chỉ khi lớn hơn 0) và `EarnedAt` là thời điểm thanh toán; điểm của dòng đó chỉ được cộng vào `CustomerProfile.Points` nếu còn trong cửa sổ 12 tháng; đồng thời `TotalSpent` được cộng thêm `FinalAmount` và hạng được cập nhật theo điểm hiện tại.

<!-- GHI CHÚ ĐỐI CHIẾU -->
<!--
(a) Các điểm đã hoàn thiện và đối chiếu
1. Mục 4.3.11.2, Bảng 4.72: Kiểu cột thực tế của `LoyaltyPointEntries.EarnedAt` là `timestamp with time zone`, đã đối chiếu chính xác theo migration `20261106040650_InitialCreate.cs`.
2. Mục 4.3.15: Migration duy nhất `InitialCreate` (mã `20261106040650`) đã được đối chiếu chi tiết trong mã nguồn.
3. Mục 4.3.15: Việc nạp sổ điểm lịch sử đã làm rõ phương án nạp qua SQL / `DbInitializer` / `RecalculateAllAsync`.
4. Hình 4.35–4.40: Cần chèn ảnh kết xuất từ sơ đồ máy trạng thái vào thư mục `hinh/` khi hoàn thiện bản in Word.

(b) Chỗ outline/tài liệu đính kèm lệch code
1. 4.3.11.2 (Bảng 4.71, 4.72): outline ghi `DiscountPercent` là decimal; code khai báo `decimal(5,2)` (numeric(5,2) trong migration). `EarnedAt` có kiểu `timestamp with time zone` trong migration `InitialCreate`.
2. 4.3.11.1 (Hình 4.34): bản vá vẽ quan hệ Bookings–LoyaltyPointEntries là 0..1–0..1. Cấu hình EF dùng `HasOne(x => x.Booking).WithMany()` kèm chỉ mục duy nhất có lọc; tính một-một đến từ chỉ mục, không phải từ khai báo quan hệ. Tên khóa chính `UserId`, `BookingId`, `UserId` của `Users`, `Bookings`, `CustomerProfiles` khớp code.
3. 4.3.11.4 (Bảng 4.74): outline ghi tên chỉ mục `TierName` là "tự sinh"; migration `InitialCreate` đặt tên `IX_MembershipTiers_TierName`. Chỉ mục `UX_LoyaltyPointEntries_Booking` và `IX_LoyaltyPointEntries_User_EarnedAt` đã được tạo trực tiếp trong migration `InitialCreate` và có trong snapshot.
4. 4.3.12.2 (Bảng 4.75): outline để trống mặc định của `CancelDeadlineHours`, `StopSellingMinutesBefore`, `HoldDurationMinutes`, `CheckInOpenMinutes`. Code: các cột này không có DEFAULT ở CSDL, mặc định chỉ ở C# (2, 0, 15, 30); `CinemaSettingsConfiguration.cs` chỉ đặt DEFAULT cho `PointsPerAmount`, `CancelRefundPercent`, `SeatGapRuleMode`. Kiểu `PointsPerAmount` là numeric(18,2), `CancelRefundPercent` là numeric(5,2).
5. 4.3.12.4 (Bảng 4.77): outline nói module không có chỉ mục riêng nên bảng không có dòng; để đủ số hiệu, bảng liệt kê khóa chính `PK_CinemaSettings`.
6. 4.3.13 (Bảng 4.78): outline nói 10 kiểu liệt kê đều "kèm ràng buộc CHECK tương ứng". Code chỉ có CHECK cho 8 kiểu; `SeatGapRuleMode` và `VerificationTokenPurpose` không có CHECK trong cấu hình, migration hay snapshot.
7. 4.3.14.1: outline chỉ nêu Pending→Confirmed, Pending→Expired, Confirmed→Cancelled. Code còn có bốn đường Pending→Cancelled (CancelBookingAsync nhánh Pending, ProcessPaymentFailureAsync, ConfirmBookingAndGenerateTicketsAsync khi hết ghế giữ, DeleteShowtimeAsync). Hình 4.36 cần có các cạnh này.
8. 4.3.14.2: đã xác minh Booked→Available xảy ra trong `CancelBookingAsync` qua `ReleaseConfirmedBookingSeatsAsync`; khi hủy suất, ghế Booked không đổi trạng thái (chỉ nhả ghế Held).
9. 4.3.14.3: đã xác minh thanh toán tiền mặt tại quầy tạo payment trực tiếp ở trạng thái Success (không qua Pending). Code còn cho phép Failed→Success khi cổng báo thành công muộn (`IsAlreadyProcessed`), outline không nêu.
10. 4.3.14.5: đã xác minh không có cơ chế tự động ComingSoon→NowShowing; code còn có NowShowing→Ended tự động và xóa mềm phim thành Ended khi phim đã có suất, outline chưa nêu cụ thể.
11. 4.3.15: Hệ thống có 1 migration duy nhất `20261106040650_InitialCreate`. Toàn bộ 30 bảng gồm `LoyaltyPointEntries` và cột `SeatGapRuleMode` đã hoàn chỉnh trong snapshot và migration duy nhất này.
12. 4.3.16: comment trong `BookingScenarioSeeder.Apply` nói giảm giá "giống CalculateDiscountAmountAsync", nhưng seeder lấy hạng từ `profile.TierId` (giá trị đồng bộ), còn `CalculateDiscountAmountAsync` tính hạng từ sổ điểm trong cửa sổ 12 tháng. Với dữ liệu mẫu hai cách có thể cho kết quả khác nhau.
13. Hình 4.58 (trong bản vá `ma-nguon-so-do-mermaid-loyalty.md`, ngoài phạm vi phần này): nút thu hồi ghi `⌊RefundedAmount / PointsPerAmount⌋`; code tính số tiền thu hồi theo tỷ lệ `FinalAmount × RefundAmount / payment.Amount` (khi hủy suất dùng toàn bộ `FinalAmount`) rồi mới chia cho `PointsPerAmount`. Chưa sửa vì ngoài phạm vi.
-->

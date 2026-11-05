## 4.2 SƠ ĐỒ LỚP

Sơ đồ lớp trong báo cáo này mô tả lớp, thuộc tính, phương thức công khai chính và quan hệ giữa các lớp của mã nguồn; không lặp lại mô hình dữ liệu quan hệ ở Mục 4.3 (ERD) và sơ đồ kiến trúc ở Mục 4.1. Mục 4.2.1 và 4.2.2 nêu nguyên tắc và ký hiệu dùng chung cho toàn bộ các sơ đồ lớp; Mục 4.2.3 trình bày sơ đồ lớp miền, làm cơ sở cho các sơ đồ lớp theo package ở các mục tiếp theo.

### 4.2.1 Nguyên tắc vẽ sơ đồ lớp

Các sơ đồ lớp của Perfo được vẽ theo năm nguyên tắc sau để vừa phản ánh đúng mã nguồn, vừa đủ gọn để đọc trên một trang báo cáo.

Thứ nhất, mỗi mục chỉ có một hình. Sơ đồ lớp miền (Mục 4.2.3) gồm toàn bộ 30 thực thể và 10 kiểu liệt kê của tầng `Cinema.Domain`. Các sơ đồ còn lại được vẽ theo từng package chức năng và đi theo chuỗi gọi của tầng Application và Api: lớp điều khiển (Controller) gọi giao diện dịch vụ, giao diện được hiện thực bởi lớp dịch vụ, và lớp dịch vụ sử dụng thực thể, DTO cùng các thành phần phụ trợ (cổng thanh toán, tác vụ nền, lớp cấu hình). Chuỗi này khớp với phân tầng đã trình bày ở Mục 4.1.2.

Thứ hai, tên lớp, giao diện và phương thức được lấy nguyên văn từ mã nguồn. Tham số `CancellationToken` có mặt ở hầu hết phương thức bất đồng bộ nên được lược bỏ khỏi chữ ký để sơ đồ gọn hơn; các tham số khác giữ nguyên.

Thứ ba, số phương thức công khai hiển thị tối đa là 8 cho mỗi lớp hoặc giao diện. Lớp có nhiều hơn 8 phương thức công khai được vẽ 8 phương thức đầu và thêm một dòng ghi chú dạng `.. rút gọn: x/y ..` (x là số phương thức hiển thị, y là tổng số); danh sách đầy đủ nằm ở bảng đi kèm sơ đồ của package đó. Giới hạn này áp dụng cho phương thức; đối với thực thể, sơ đồ lớp miền hiển thị đầy đủ thuộc tính.

Thứ tư, trong các sơ đồ theo package, thực thể và DTO chỉ ghi tên lớp kèm stereotype, không lặp lại thuộc tính; thuộc tính của thực thể xem ở sơ đồ lớp miền (Hình 4.4).

Thứ năm, các lớp thuộc thư viện bên ngoài (ví dụ `IPasswordHasher<User>`, `HttpClient`) không được vẽ, vì chúng không thuộc thiết kế của hệ thống.

### 4.2.2 Ký hiệu sơ đồ lớp

Các sơ đồ lớp được viết bằng PlantUML. Phần khai báo `skinparam` ở đầu mỗi sơ đồ quy định hình thức chung: tắt bóng đổ (`shadowing false`), ẩn biểu tượng thuộc tính (`classAttributeIconSize 0`) và ẩn các ngăn trống của lớp (`hide empty members`); đối với kiểu liệt kê, ngăn phương thức bị ẩn (`hide enum methods`). Màu nền của lớp được xác định theo stereotype, giúp nhận biết vai trò của lớp ngay khi nhìn vào sơ đồ. Bảng 4.4 liệt kê các ký hiệu được sử dụng, kèm màu nền khai báo trong `skinparam`.

*Bảng 4.4. Ký hiệu sơ đồ lớp*

| Ký hiệu | Ý nghĩa | Màu nền (`skinparam`) |
|---|---|---|
| `<<Controller>>` | Lớp điều khiển API (`Cinema.Api/Controllers`) | `#DCEBFA` |
| `interface` | Giao diện dịch vụ hoặc hạ tầng (`Cinema.Application/Interfaces`) | Không khai báo; dùng màu mặc định của PlantUML |
| `<<Service>>` | Lớp dịch vụ hoặc lớp hiện thực giao diện | `#E3F4E1` |
| `<<Gateway>>` | Cổng thanh toán (hiện thực `IPaymentGateway`) | `#FDE9D0` |
| `<<Job>>` | Công việc nền Hangfire | `#FFF3C4` |
| `<<Hub>>` | SignalR Hub | `#E8DFF5` |
| `<<Utility>>` | Lớp tiện ích tĩnh | `#EEEEEE` |
| `<<Entity>>` | Thực thể miền (`Cinema.Domain/Entities`) | `#FFFFFF` |
| `<<DTO>>` | Đối tượng truyền dữ liệu | `#F7F7F7` |
| `<<record>>` | Kiểu `record` | `#F7F7F7` |
| `<<Options>>` | Lớp cấu hình theo Options pattern (Mục 4.1.6) | `#F7F7F7` |
| `<<Validator>>` | Lớp kiểm tra dữ liệu đầu vào (FluentValidation) | `#F7F7F7` |
| `<<Exception>>` | Ngoại lệ nghiệp vụ | `#FAD9D9` |
| `enum` | Kiểu liệt kê; các giá trị liệt kê mỗi dòng một giá trị | Không khai báo; dùng màu mặc định của PlantUML |
| `+Tên : kiểu` | Thuộc tính công khai; kiểu có hậu tố `?` là kiểu cho phép `null` | — |
| `+Tên(tham số) : kiểu trả về` | Phương thức công khai, đọc là: tên phương thức, danh sách tham số trong ngoặc, dấu hai chấm rồi kiểu trả về | — |
| `.. rút gọn: x/y ..` | Dòng ngăn trong thân lớp: chỉ hiển thị x trên tổng số y phương thức công khai | — |
| `A ..\|> B` | A hiện thực giao diện B | — |
| `A ..> B` | A phụ thuộc B (tiêm phụ thuộc, sử dụng) | — |
| `A "m" --> "n" B : Tên` | A kết hợp có hướng với B; `Tên` là thuộc tính điều hướng của A trỏ tới B; `m` và `n` là bội số ở phía A và phía B (`1`, `0..1`, `*`) | — |
| `A "m" -- "n" B : Tên` | Kết hợp không hướng giữa A và B, dùng cho quan hệ một–một mà cả hai thực thể đều có thuộc tính điều hướng | — |

Trong Hình 4.4 chỉ xuất hiện stereotype `<<Entity>>`, từ khóa `enum`, các mũi tên `-->`, `--` và `..>`; các stereotype còn lại và mũi tên `..|>` xuất hiện ở các sơ đồ theo package (Hình 4.5 đến Hình 4.25). Về cách đọc bội số, `Booking "*" --> "0..1" User : User` có nghĩa là một booking có không hoặc một người dùng (booking có thể không gắn tài khoản, vì `UserId` của `Booking` cho phép `null`) và một người dùng có thể có nhiều booking.

### 4.2.3 Sơ đồ lớp miền

Tầng miền (`Cinema.Domain`) gồm 30 lớp thực thể trong thư mục `Entities` và 10 kiểu liệt kê trong thư mục `Enums`; số lượng này được đối chiếu trực tiếp với mã nguồn (mỗi thực thể một tệp `.cs`, mỗi kiểu liệt kê một tệp). Hình 4.4 thể hiện toàn bộ các thực thể cùng thuộc tính, các kiểu liệt kê và quan hệ giữa chúng.

![Hình 4.4. Sơ đồ lớp miền](hinh/4.4.png)

*Hình 4.4. Sơ đồ lớp miền*

**Cách đọc và khác biệt so với ERD.** Mỗi thực thể được vẽ với các thuộc tính công khai và kiểu của chúng, đúng như khai báo trong lớp C# tương ứng; các thuộc tính định danh và khóa ngoại (như `BookingId`, `UserId`) xuất hiện vì chúng là thuộc tính của lớp, nhưng sơ đồ không đánh dấu khóa chính, khóa ngoại, chỉ mục, ràng buộc `CHECK` hay hành vi xóa dây chuyền. Các thông tin đó thuộc về mô hình dữ liệu quan hệ và được thể hiện ở ERD (Mục 4.3, Hình 4.26). Mỗi mũi tên của Hình 4.4 tương ứng với một thuộc tính điều hướng, tên thuộc tính được ghi làm nhãn mũi tên. Chẳng hạn, nhãn `Tier` trên mũi tên từ `CustomerProfile` đến `MembershipTier` là thuộc tính `CustomerProfile.Tier`. Các thuộc tính điều hướng dạng tập hợp (phía “nhiều”, ví dụ `Booking.Tickets`) là chiều ngược của chính các quan hệ đó nên không được vẽ thành mũi tên riêng.

**Các nhóm thực thể.** Ba mươi thực thể có thể chia thành năm nhóm theo vai trò:

- *Người dùng, xác thực và thành viên (9):* `Role`, `User`, `CustomerProfile`, `Employee`, `Position`, `RefreshToken`, `UserVerificationToken`, `MembershipTier`, `LoyaltyPointEntry`.
- *Phim và đánh giá (9):* `Movie`, `Genre`, `Language`, `AgeRating`, `Director`, `Actor`, `MovieGenre`, `MovieCast`, `Review`.
- *Phòng chiếu và suất chiếu (6):* `CinemaRoom`, `ScreenType`, `Seat`, `SeatType`, `Showtime`, `ShowtimeSeat`.
- *Đặt vé và thanh toán (5):* `Booking`, `BookingTicket`, `BookingCombo`, `Combo`, `Payment`.
- *Cấu hình rạp (1):* `CinemaSettings`, thực thể đơn lẻ không có quan hệ với thực thể nào khác; gồm thông tin rạp, tỷ lệ quy đổi điểm `PointsPerAmount`, các tham số hủy vé, giữ ghế, mở soát vé và chế độ kiểm tra ghế lẻ `SeatGapRuleMode`.

**Các loại quan hệ.** Phần lớn quan hệ là nhiều–một: phía “nhiều” giữ khóa ngoại và thuộc tính điều hướng đơn trỏ tới phía “một” (ví dụ `Showtime.Movie`, `Seat.Room`). Khi khóa ngoại cho phép `null`, bội số phía liên quan là `0..1` thay vì `1` (ví dụ `Movie.Director`, `Booking.User`). Ba cặp quan hệ một–một, mà cả hai thực thể đều có thuộc tính điều hướng, được vẽ bằng một đường không hướng `--`: `CustomerProfile` với `User` (mỗi người dùng có không hoặc một hồ sơ khách hàng), `Employee` với `User` (mỗi người dùng có không hoặc một hồ sơ nhân viên) và `BookingTicket` với `ShowtimeSeat` (mỗi ghế của suất chiếu có không hoặc một vé). Quan hệ tự tham chiếu duy nhất là `RefreshToken.ReplacedByToken`, trỏ từ một refresh token tới token thay thế nó khi xoay vòng, có bội số `0..1`. Bốn thực thể liên kết biểu diễn quan hệ nhiều–nhiều giữa các thực thể khác: `MovieGenre` (phim–thể loại), `MovieCast` (phim–diễn viên, kèm thuộc tính `CharacterName`), `BookingCombo` (booking–combo, kèm `Quantity` và `UnitPrice`) và `BookingTicket` (booking–ghế của suất chiếu, kèm giá vé `Price`, mã `QrCode` và trạng thái soát vé `IsCheckedIn`, `CheckedInAt`). Toàn bộ 38 thuộc tính điều hướng đơn của các thực thể được liệt kê ở Bảng 4.6; ba cặp một–một ở trên được vẽ thành một đường nên Hình 4.4 có 35 đường quan hệ giữa các thực thể. Ngoài ra, các thực thể còn có 24 thuộc tính điều hướng dạng tập hợp (như `Booking.Tickets`, `Showtime.ShowtimeSeats`, `Role.Users`); đây là chiều ngược của các quan hệ nhiều–một nên không thêm quan hệ mới. Tồn tại một khóa ngoại không có thuộc tính điều hướng là `ShowtimeSeat.HeldByBookingId` (ghế đang được giữ cho booking nào); vì lớp không khai báo thuộc tính điều hướng nên quan hệ này không xuất hiện ở Hình 4.4 và Bảng 4.6, mà được thể hiện ở ERD.

**Sổ điểm thành viên.** `LoyaltyPointEntry` là thực thể ghi sổ điểm tích lũy của khách hàng, mỗi dòng gồm `EntryId`, `UserId`, `BookingId` (có thể `null`), `Points` (điểm cộng), `RevokedPoints` (điểm đã thu hồi, ví dụ khi hoàn tiền) và `EarnedAt` (thời điểm ghi nhận). Thực thể nối với `User` theo quan hệ nhiều–một (một người dùng có nhiều dòng sổ điểm; mỗi dòng thuộc đúng một người dùng) và với `Booking` theo bội số `0..1`: một dòng sổ điểm có thể không gắn booking nào, và mỗi booking có tối đa một dòng sổ điểm. Điều kiện “tối đa một dòng” được bảo đảm ở lớp cơ sở dữ liệu, không thể hiện ở sơ đồ lớp mà ở ERD (Hình 4.34). Điểm hiệu lực của khách hàng được tính trên các dòng sổ điểm có `EarnedAt` nằm trong cửa sổ trượt 12 tháng, bằng tổng `Points − RevokedPoints` (`LoyaltyExpiryService`); cách ghi sổ, thu hồi và đồng bộ được trình bày ở phần thiết kế nghiệp vụ liên quan (xem Hình 4.58). Mã nguồn không khai báo thuộc tính tập hợp trỏ từ `User` hay `Booking` tới `LoyaltyPointEntry`, nên quan hệ này chỉ có một chiều điều hướng, từ `LoyaltyPointEntry` sang `User` và `Booking`; vì vậy tổng số thuộc tính điều hướng dạng tập hợp vẫn là 24.

**Các kiểu liệt kê.** Mười kiểu liệt kê của tầng miền được tổng hợp ở Bảng 4.5; trong Hình 4.4 mỗi kiểu được nối bằng mũi tên phụ thuộc đứt nét `..>` từ thực thể sử dụng nó (ví dụ `Booking ..> BookingStatus`).

*Bảng 4.5. Danh sách enum*

| Enum | Giá trị | Dùng tại | Ý nghĩa |
|---|---|---|---|
| `BookingChannel` | `Online`, `Counter` | `Booking.Channel` | Kênh tạo booking: trực tuyến hoặc bán tại quầy. |
| `BookingStatus` | `Pending`, `Confirmed`, `Cancelled`, `Expired` | `Booking.Status` | Trạng thái booking: đang chờ thanh toán, đã xác nhận, đã hủy, đã hết hạn giữ ghế. Giá trị mặc định là `Pending`. |
| `Gender` | `Male`, `Female`, `Other` | `User.Gender` | Giới tính của người dùng (thuộc tính cho phép `null`). |
| `MovieStatus` | `ComingSoon`, `NowShowing`, `Ended` | `Movie.Status` | Trạng thái phim: sắp chiếu, đang chiếu, đã kết thúc. Giá trị mặc định là `ComingSoon`. |
| `PaymentMethod` | `VNPay`, `Momo`, `Cash`, `Stripe` | `Payment.PaymentMethod` | Phương thức thanh toán: ba cổng trực tuyến và tiền mặt. |
| `PaymentStatus` | `Pending`, `Success`, `Failed`, `Refunded` | `Payment.Status` | Trạng thái giao dịch: đang chờ, thành công, thất bại, đã hoàn tiền. Giá trị mặc định là `Pending`. |
| `SeatGapRuleMode` | `Strict`, `AllowEdge`, `Off` | `CinemaSettings.SeatGapRuleMode` | Chế độ kiểm tra ghế lẻ khi chọn ghế: `Strict` không cho để trống một ghế đơn lẻ giữa các ghế đã chọn, `AllowEdge` cho phép ghế lẻ ở rìa hàng, `Off` không kiểm tra. Giá trị mặc định là `Strict`. |
| `ShowtimeSeatStatus` | `Available`, `Held`, `Booked` | `ShowtimeSeat.Status` | Trạng thái ghế của một suất chiếu: còn trống, đang được giữ, đã đặt. Giá trị mặc định là `Available`. |
| `ShowtimeStatus` | `Scheduled`, `Cancelled`, `Completed` | `Showtime.Status` | Trạng thái suất chiếu: đã lên lịch, đã hủy, đã hoàn tất. Giá trị mặc định là `Scheduled`. |
| `VerificationTokenPurpose` | `EmailConfirmation`, `PasswordReset` | `UserVerificationToken.Purpose` | Mục đích của mã xác minh: xác nhận email hoặc đặt lại mật khẩu. |

**Quan hệ giữa các thực thể.** Bảng 4.6 liệt kê từng thuộc tính điều hướng đơn của các thực thể theo đúng khai báo trong mã nguồn. Cột “Bội số phía liên quan” là bội số ở phía thực thể được trỏ tới; cột “Loại quan hệ” phân loại theo các nhóm đã nêu ở trên.

*Bảng 4.6. Quan hệ giữa các thực thể*

| Thực thể | Thuộc tính điều hướng | Thực thể liên quan | Bội số phía liên quan | Loại quan hệ |
|---|---|---|---|---|
| `Booking` | `User` | `User` | 0..1 | Nhiều–một |
| `Booking` | `Showtime` | `Showtime` | 1 | Nhiều–một |
| `Booking` | `CreatedByEmployee` | `Employee` | 0..1 | Nhiều–một |
| `BookingCombo` | `Booking` | `Booking` | 1 | Nhiều–một (thực thể liên kết) |
| `BookingCombo` | `Combo` | `Combo` | 1 | Nhiều–một (thực thể liên kết) |
| `BookingTicket` | `Booking` | `Booking` | 1 | Nhiều–một (thực thể liên kết) |
| `BookingTicket` | `ShowtimeSeat` | `ShowtimeSeat` | 1 | Một–một |
| `CinemaRoom` | `ScreenType` | `ScreenType` | 0..1 | Nhiều–một |
| `CustomerProfile` | `User` | `User` | 1 | Một–một |
| `CustomerProfile` | `Tier` | `MembershipTier` | 0..1 | Nhiều–một |
| `Employee` | `User` | `User` | 1 | Một–một |
| `Employee` | `Position` | `Position` | 0..1 | Nhiều–một |
| `LoyaltyPointEntry` | `User` | `User` | 1 | Nhiều–một |
| `LoyaltyPointEntry` | `Booking` | `Booking` | 0..1 | Tùy chọn hai phía (0..1 – 0..1) |
| `Movie` | `Director` | `Director` | 0..1 | Nhiều–một |
| `Movie` | `Language` | `Language` | 0..1 | Nhiều–một |
| `Movie` | `AgeRating` | `AgeRating` | 0..1 | Nhiều–một |
| `MovieCast` | `Movie` | `Movie` | 1 | Nhiều–một (thực thể liên kết) |
| `MovieCast` | `Actor` | `Actor` | 1 | Nhiều–một (thực thể liên kết) |
| `MovieGenre` | `Movie` | `Movie` | 1 | Nhiều–một (thực thể liên kết) |
| `MovieGenre` | `Genre` | `Genre` | 1 | Nhiều–một (thực thể liên kết) |
| `Payment` | `Booking` | `Booking` | 1 | Nhiều–một |
| `RefreshToken` | `User` | `User` | 1 | Nhiều–một |
| `RefreshToken` | `ReplacedByToken` | `RefreshToken` | 0..1 | Tự tham chiếu |
| `Review` | `Movie` | `Movie` | 1 | Nhiều–một |
| `Review` | `User` | `User` | 1 | Nhiều–một |
| `Seat` | `Room` | `CinemaRoom` | 1 | Nhiều–một |
| `Seat` | `SeatType` | `SeatType` | 0..1 | Nhiều–một |
| `Showtime` | `Movie` | `Movie` | 1 | Nhiều–một |
| `Showtime` | `Room` | `CinemaRoom` | 1 | Nhiều–một |
| `ShowtimeSeat` | `Showtime` | `Showtime` | 1 | Nhiều–một |
| `ShowtimeSeat` | `Seat` | `Seat` | 1 | Nhiều–một |
| `ShowtimeSeat` | `HeldByUser` | `User` | 0..1 | Nhiều–một |
| `ShowtimeSeat` | `BookingTicket` | `BookingTicket` | 0..1 | Một–một |
| `User` | `Role` | `Role` | 1 | Nhiều–một |
| `User` | `CustomerProfile` | `CustomerProfile` | 0..1 | Một–một |
| `User` | `Employee` | `Employee` | 0..1 | Một–một |
| `UserVerificationToken` | `User` | `User` | 1 | Nhiều–một |

Tổng số thuộc tính điều hướng dạng tập hợp (phía “nhiều”) là 24 sau khi thêm `LoyaltyPointEntry`; `User` và `Booking` không có thuộc tính tập hợp trỏ tới `LoyaltyPointEntry`.

<!-- GHI CHÚ ĐỐI CHIẾU

(a) [CẦN BỔ SUNG] còn lại
- Hình 4.4: chỉ để placeholder `hinh/4.4.png`; cần xuất ảnh từ mã PlantUML Hình 4.4 trong `ma-nguon-so-do-bo-sung.md` (mã đã có LoyaltyPointEntry).
- Tham chiếu chéo cần kiểm khi gộp báo cáo: "Hình 4.26" (ERD đầy đủ), "Hình 4.34" (ERD module Thành viên), "Hình 4.58" (lưu đồ ghi sổ điểm) theo bảng đổi số trong `ma-nguon-so-do-bo-sung.md` và `ma-nguon-so-do-mermaid-loyalty.md`; "Mục 4.3" (ERD) theo outline. Bài chưa dẫn số mục của phần nghiệp vụ điểm thành viên vì phạm vi phần này không chứa.
- Bảng 4.4 nhắc "Hình 4.5 đến Hình 4.25" là phạm vi các sơ đồ lớp package theo `ma-nguon-so-do-bo-sung.md`; outline có thể còn đánh số khác, cần kiểm khi gộp.

(b) Chỗ outline lệch code / mã sơ đồ
1. Số thực thể và enum: đếm lại `Cinema.Domain/Entities` = 30 tệp/30 lớp, `Enums` = 10 tệp/10 enum: KHỚP outline. Mã PlantUML Hình 4.4 cũng có đủ 30 lớp, 10 enum; đã đối chiếu tự động toàn bộ thuộc tính vô hướng (tên, kiểu, nullable) của 30 thực thể với mã PlantUML: không có chênh lệch.
2. `LoyaltyPointEntry` (EntryId, UserId, BookingId?, Points, RevokedPoints, EarnedAt; điều hướng `User`, `Booking?`) khớp với `Domain/Entities/LoyaltyPointEntry.cs` và với mã PlantUML (`LoyaltyPointEntry "*" --> "1" User`; `LoyaltyPointEntry "0..1" --> "0..1" Booking`).
3. Dòng "[CẦN BỔ SUNG: đếm lại ... 24 ...]" cuối mục 4.2.3 của outline: đã đếm = 24 thuộc tính `ICollection<>` trong Entities; `User` và `Booking` không có collection tới `LoyaltyPointEntry` (EF cấu hình `.WithMany()` không điều hướng ngược), nên giữ nguyên 24. Outline nên sửa dòng này, không còn [CẦN BỔ SUNG].
4. Bảng 4.6: 38 dòng của outline khớp 100% với 38 thuộc tính điều hướng đơn trong code (đối chiếu tự động tên, kiểu liên quan, nullable). PlantUML có 35 đường quan hệ vì ba cặp một–một (CustomerProfile–User, Employee–User, BookingTicket–ShowtimeSeat) vẽ một đường `--`.
5. Bảng 4.5: 10 dòng khớp code về tên enum, giá trị, thuộc tính sử dụng. Cột "Ý nghĩa" là phần thêm; ý nghĩa `SeatGapRuleMode` suy từ `SeatLayoutRules.ValidateSelection`; ý nghĩa `BookingStatus.Expired`, `ShowtimeStatus.Completed` suy từ `SeatExpirationService`, `ShowtimeService`. Các ý nghĩa còn lại suy từ tên giá trị, enum không có chú thích XML.
6. Bảng 4.4: outline có 9 dòng; đã giữ đủ và thêm cột màu nền (từ `skinparam`) cùng các dòng `enum`, thuộc tính `+Tên : kiểu`, `.. rút gọn: x/y ..`, `A "m" -- "n" B` vì các ký hiệu này có thật trong mã PlantUML. Màu của `interface` và `enum` không khai báo trong `skinparam`.
7. Outline 4.2.3 yêu cầu "mũi tên hiện thực, phụ thuộc, kết hợp"; `..|>` không xuất hiện trong Hình 4.4, chỉ ở Hình 4.5–4.25 (đã nêu trong 4.2.2).
8. Cột "Loại quan hệ" của Bảng 4.6 là phần thêm: `LoyaltyPointEntry.Booking` ghi "Tùy chọn hai phía (0..1 – 0..1)" theo mã PlantUML; trong cấu hình EF quan hệ khai báo `HasOne(Booking).WithMany()` kèm chỉ mục duy nhất có điều kiện (`UX_LoyaltyPointEntries_Booking`) để mỗi booking tối đa một dòng.
9. `ShowtimeSeat.HeldByBookingId` có khóa ngoại tới Booking (cấu hình `HasOne<Booking>()`, SetNull) nhưng không có thuộc tính điều hướng, nên không nằm trong Hình 4.4/Bảng 4.6; bài chỉ nêu trong đoạn "Các loại quan hệ".
-->

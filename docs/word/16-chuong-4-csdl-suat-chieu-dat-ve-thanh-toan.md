### 4.3.8 Module Suất chiếu

Module Suất chiếu biểu diễn lịch chiếu và trạng thái từng ghế theo từng suất. Module gồm hai thực thể: `Showtime` (suất chiếu: phim nào, phòng nào, khung giờ nào, giá cơ sở bao nhiêu) và `ShowtimeSeat` (ghế của một suất chiếu cụ thể, mang trạng thái, hạn giữ và giá). `Showtime` tham chiếu `Movie` của module Phim (Mục 4.3.6) và `CinemaRoom` của module Phòng chiếu (Mục 4.3.7). `ShowtimeSeat` tham chiếu `Seat` (Mục 4.3.7), `User` (người đang giữ ghế) và `Booking` (booking đang giữ ghế, Mục 4.3.9); ở chiều ngược lại, `Booking` tham chiếu `Showtime` và `BookingTicket` tham chiếu `ShowtimeSeat`. Tách `Seat` (ghế vật lý của phòng) khỏi `ShowtimeSeat` (ghế trong một suất) cho phép cùng một ghế vật lý có trạng thái và giá độc lập ở mỗi suất chiếu.

#### 4.3.8.1 Sơ đồ ERD của module

Hình 4.31 thể hiện hai thực thể sở hữu của module với đầy đủ thuộc tính. Các thực thể `Movie`, `CinemaRoom`, `Seat`, `User`, `Booking` và `BookingTicket` chỉ xuất hiện với khóa chính. Các quan hệ gồm: `Movie` – `Showtime` và `CinemaRoom` – `Showtime` (một–nhiều, bắt buộc ở phía `Showtime`); `Showtime` – `ShowtimeSeat` và `Seat` – `ShowtimeSeat` (một–nhiều, bắt buộc ở phía `ShowtimeSeat`); `User` – `ShowtimeSeat` và `Booking` – `ShowtimeSeat` (một–nhiều không bắt buộc, qua `HeldByUserId` và `HeldByBookingId`); `ShowtimeSeat` – `BookingTicket` (một–không hoặc một, vì `BookingTicket.ShowtimeSeatId` là duy nhất).

![Hình 4.31. ERD module Suất chiếu](hinh/4.31.png)

*Hình 4.31. ERD module Suất chiếu*

#### 4.3.8.2 Mô tả thực thể

**Thực thể Showtime.** `Showtime` là một lần chiếu của một phim trong một phòng. Trạng thái `Status` được lưu dưới dạng chuỗi tương ứng với enum `ShowtimeStatus` gồm `Scheduled` (mặc định), `Cancelled`, `Completed`. Suất chuyển sang `Cancelled` khi bị hủy (`ShowtimeService`), và `ShowtimeService.CompleteEndedShowtimesAsync` chuyển các suất `Scheduled` có `EndTime` đã qua sang `Completed`. Mốc thời gian được lưu kiểu `timestamp without time zone`; mã nguồn xử lý thời điểm hiện tại bằng `DateTime.UtcNow` khi so sánh. Việc phát hiện suất chiếu trùng giờ trong cùng một phòng không do ràng buộc cơ sở dữ liệu đảm nhiệm mà do hàm `EnsureNoOverlapAsync` ở tầng dịch vụ, được gọi khi tạo và cập nhật suất.

Bảng 4.58. Thực thể Showtime (bảng `Showtimes`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `ShowtimeId` | int | Không | PK | Identity (tự tăng) | Mã định danh suất chiếu. |
| `MovieId` | int | Không | FK | Tham chiếu `Movies`; `Restrict` | Phim được chiếu trong suất. |
| `RoomId` | int | Không | FK | Tham chiếu `CinemaRooms`; `Restrict` | Phòng chiếu của suất. |
| `StartTime` | timestamp without time zone | Không | — | — | Thời điểm bắt đầu suất chiếu. |
| `EndTime` | timestamp without time zone | Không | — | CHECK `> StartTime` | Thời điểm kết thúc suất chiếu; dùng để kiểm tra trùng lịch phòng. |
| `BasePrice` | decimal(18,2) | Không | — | CHECK `>= 0` | Giá vé cơ sở của suất; giá từng ghế = `BasePrice` × `PriceMultiplier` của loại ghế. |
| `Status` | varchar(20) | Không | — | `ShowtimeStatus.Scheduled` (lưu chuỗi `Scheduled`); CHECK thuộc 3 giá trị | Trạng thái suất: đã lên lịch, đã hủy, đã hoàn thành. |
| `CreatedAt` | timestamp without time zone | Không | — | SQL: `NOW()` | Thời điểm tạo suất chiếu. |

**Thực thể ShowtimeSeat.** `ShowtimeSeat` là bản ghi trạng thái của một ghế trong một suất chiếu; các dòng này được sinh khi tạo suất (giá = `BasePrice` × hệ số loại ghế, hệ số `1.0` nếu ghế chưa có loại). Trạng thái `Status` tương ứng enum `ShowtimeSeatStatus`: `Available` (mặc định, còn trống), `Held` (đang được giữ chờ thanh toán) và `Booked` (đã bán). Ba cột `HeldByUserId`, `HeldByBookingId`, `HeldUntil` mô tả việc giữ ghế: khi `BookingService` giữ ghế, trạng thái chuyển `Held`, `HeldByUserId` ghi người giữ (với booking tại quầy là `UserId` của nhân viên tạo booking, theo `BookingSeatHold.ResolveHolderUserIdAsync`), `HeldUntil` ghi hạn giữ (không vượt quá `StartTime` của suất), và sau khi bản ghi `Booking` được tạo thì `HeldByBookingId` được gán `BookingId`. Khi thanh toán thành công, ghế chuyển `Booked` và cả ba cột trên được đặt về NULL; khi hết hạn hoặc hủy, ghế trở về `Available` và ba cột được xóa (`SeatExpirationService`).

Bảng 4.59. Thực thể ShowtimeSeat (bảng `ShowtimeSeats`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `ShowtimeSeatId` | int | Không | PK | Identity (tự tăng) | Mã định danh ghế theo suất. |
| `ShowtimeId` | int | Không | FK | Tham chiếu `Showtimes`; `Cascade`; thuộc `UQ_Showtime_Seat` | Suất chiếu chứa ghế. |
| `SeatId` | int | Không | FK | Tham chiếu `Seats`; `Restrict`; thuộc `UQ_Showtime_Seat` | Ghế vật lý tương ứng của phòng. |
| `Status` | varchar(20) | Không | — | `ShowtimeSeatStatus.Available` (lưu chuỗi `Available`); CHECK thuộc 3 giá trị | Trạng thái ghế trong suất: trống, đang giữ, đã bán. |
| `HeldByUserId` | int | Có | FK | Tham chiếu `Users`; `Restrict` | Người dùng đang giữ ghế; NULL khi ghế không ở trạng thái giữ. |
| `HeldByBookingId` | int | Có | FK | Tham chiếu `Bookings`; `SetNull` | Booking đang giữ ghế; NULL khi chưa gắn booking hoặc đã giải phóng. |
| `HeldUntil` | timestamp without time zone | Có | — | — | Hạn giữ ghế; quá hạn mà chưa thanh toán thì ghế được giải phóng. |
| `Price` | decimal(18,2) | Không | — | CHECK `>= 0` | Giá bán của ghế trong suất, được chốt khi sinh `ShowtimeSeat` và sao chép sang vé khi xác nhận booking. |

#### 4.3.8.3 Ràng buộc toàn vẹn

Các ràng buộc CHECK bảo vệ miền giá trị của giờ chiếu, giá và trạng thái. Về khóa ngoại, suất chiếu không thể bị xóa khi còn booking tham chiếu (`Booking.ShowtimeId` dùng `Restrict`, xem Bảng 4.66), phim và phòng không thể bị xóa khi còn suất chiếu (`Restrict`); ngược lại, xóa một suất chiếu sẽ xóa toàn bộ ghế theo suất của nó (`Cascade`). Khóa ngoại `HeldByBookingId` dùng `SetNull` để khi booking bị xóa, ghế không giữ tham chiếu treo mà tự trả về giá trị NULL. Cột "Hành vi xóa" và "Mục đích bảo vệ" được bổ sung vào bảng khung theo cấu hình EF và `ModelSnapshot`. Cơ sở dữ liệu không có ràng buộc CHECK nào liên kết `Status` với các cột giữ ghế (ví dụ ghế `Held` phải có `HeldUntil`); tính nhất quán này do tầng dịch vụ bảo đảm.

Bảng 4.60. Ràng buộc toàn vẹn module Suất chiếu

| Thực thể | Tên | Loại | Biểu thức | Hành vi xóa | Mục đích bảo vệ |
|---|---|---|---|---|---|
| Showtime | `CK_Showtime_Time` | CHECK | `"EndTime" > "StartTime"` | — | Suất chiếu phải kết thúc sau khi bắt đầu. |
| Showtime | `CK_Showtimes_BasePrice` | CHECK | `"BasePrice" >= 0` | — | Giá cơ sở không âm. |
| Showtime | `CK_Showtimes_Status` | CHECK | `"Status" IN ('Scheduled','Cancelled','Completed')` | — | Trạng thái thuộc tập giá trị của `ShowtimeStatus`. |
| ShowtimeSeat | `CK_ShowtimeSeats_Price` | CHECK | `"Price" >= 0` | — | Giá ghế không âm. |
| ShowtimeSeat | `CK_ShowtimeSeats_Status` | CHECK | `"Status" IN ('Available','Held','Booked')` | — | Trạng thái thuộc tập giá trị của `ShowtimeSeatStatus`. |
| Showtime | FK `MovieId` | Khóa ngoại | — | Restrict | Không xóa phim còn suất chiếu. |
| Showtime | FK `RoomId` | Khóa ngoại | — | Restrict | Không xóa phòng còn suất chiếu. |
| ShowtimeSeat | FK `ShowtimeId` | Khóa ngoại | — | Cascade | Xóa suất chiếu thì xóa ghế theo suất. |
| ShowtimeSeat | FK `SeatId` | Khóa ngoại | — | Restrict | Không xóa ghế vật lý còn gắn với ghế theo suất. |
| ShowtimeSeat | FK `HeldByUserId` | Khóa ngoại | — | Restrict | Không xóa người dùng đang giữ ghế. |
| ShowtimeSeat | FK `HeldByBookingId` | Khóa ngoại | — | SetNull | Xóa booking thì bỏ liên kết giữ ghế, không xóa ghế. |

#### 4.3.8.4 Chỉ mục

`UQ_Showtime_Seat` trên `(ShowtimeId, SeatId)` là chỉ mục duy nhất không có điều kiện lọc (cấu hình không khai báo `HasFilter`); nó bảo đảm mỗi ghế vật lý chỉ có đúng một dòng `ShowtimeSeat` trong một suất, nên mọi thao tác giữ ghế và bán ghế đều tác động lên cùng một dòng duy nhất. Chống đặt trùng ghế dựa trên hai lớp. Lớp thứ nhất là điều kiện khóa trong câu lệnh cập nhật có điều kiện của `BookingService`: trong một giao dịch, câu `ExecuteUpdate` chỉ đặt `Status = Held` cho các ghế đang `Available`, hoặc đang `Held` bởi chính người giữ và đã quá `HeldUntil`; nếu số dòng bị tác động khác số ghế yêu cầu, giao dịch bị hoàn tác và trả `SeatConflictException` kèm danh sách ghế xung đột. Lớp thứ hai là chỉ mục duy nhất trên `BookingTicket.ShowtimeSeatId` (Bảng 4.67), bảo đảm một ghế theo suất không thể có hai vé. `IX_ShowtimeSeats_Status` trên `(ShowtimeId, Status)` phục vụ truy vấn sơ đồ ghế của một suất theo trạng thái và tác vụ quét ghế `Held` quá hạn. `IX_Showtime_Room_Time` trên `(RoomId, StartTime, EndTime)` phục vụ kiểm tra trùng lịch phòng và tra cứu lịch theo phòng; `IX_Showtime_Movie` trên `(MovieId, StartTime)` phục vụ liệt kê suất của một phim theo thời gian. Ba chỉ mục khóa ngoại do EF tự sinh (`HeldByBookingId`, `HeldByUserId`, `SeatId`) được bổ sung vào bảng vì có trong snapshot và migration `InitialCreate`.

Bảng 4.61. Chỉ mục module Suất chiếu

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| Showtime | `IX_Showtime_Room_Time` | Thường | `RoomId`, `StartTime`, `EndTime` | — |
| Showtime | `IX_Showtime_Movie` | Thường | `MovieId`, `StartTime` | — |
| ShowtimeSeat | `UQ_Showtime_Seat` | Duy nhất | `ShowtimeId`, `SeatId` | — |
| ShowtimeSeat | `IX_ShowtimeSeats_Status` | Thường | `ShowtimeId`, `Status` | — |
| ShowtimeSeat | `IX_ShowtimeSeats_SeatId` (tên tự sinh) | Thường | `SeatId` | — |
| ShowtimeSeat | `IX_ShowtimeSeats_HeldByUserId` (tên tự sinh) | Thường | `HeldByUserId` | — |
| ShowtimeSeat | `IX_ShowtimeSeats_HeldByBookingId` (tên tự sinh) | Thường | `HeldByBookingId` | — |

### 4.3.9 Module Đặt vé

Module Đặt vé lưu giao dịch đặt vé từ lúc giữ ghế đến khi phát hành vé. Module gồm bốn thực thể: `Booking` (đơn đặt vé), `BookingTicket` (vé của từng ghế), `BookingCombo` (dòng combo trong đơn) và `Combo` (danh mục bắp nước). `Booking` tham chiếu `User` (khách hàng có tài khoản), `Employee` (nhân viên bán tại quầy) của module Người dùng và `Showtime` của module Suất chiếu; `BookingTicket` tham chiếu `ShowtimeSeat` (Mục 4.3.8). `Payment` của module Thanh toán (Mục 4.3.10) và `LoyaltyPointEntry` của module Thành viên (Mục 4.3.11) tham chiếu ngược về `Booking`. Một đơn thuộc một trong hai kênh `Online` hoặc `Counter` (bán tại quầy).

#### 4.3.9.1 Sơ đồ ERD của module

Hình 4.32 thể hiện bốn thực thể sở hữu của module với đầy đủ thuộc tính. Các thực thể `User`, `Employee`, `Showtime`, `ShowtimeSeat` và `Payment` chỉ xuất hiện với khóa chính. Các quan hệ gồm: `User` – `Booking` và `Employee` – `Booking` (một–nhiều không bắt buộc ở phía `Booking`); `Showtime` – `Booking` (một–nhiều bắt buộc); `Booking` – `BookingTicket` và `Booking` – `BookingCombo` (một–nhiều bắt buộc); `ShowtimeSeat` – `BookingTicket` (một–không hoặc một); `Combo` – `BookingCombo` (một–nhiều bắt buộc); `Booking` – `Payment` (một–nhiều).

![Hình 4.32. ERD module Đặt vé](hinh/4.32.png)

*Hình 4.32. ERD module Đặt vé*

#### 4.3.9.2 Mô tả thực thể

**Thực thể Booking.** `Booking` là đơn đặt vé. Khi giữ ghế, `BookingService` tạo đơn ở trạng thái `Pending` (enum `BookingStatus`: `Pending`, `Confirmed`, `Cancelled`, `Expired`) với `ExpiresAt` bằng hạn giữ ghế; thanh toán thành công chuyển đơn sang `Confirmed` và phát hành vé; đơn quá hạn chưa thanh toán được `SeatExpirationService.ExpireDueBookingsAsync` chuyển sang `Expired`. Kênh bán `Channel` (enum `BookingChannel`: `Online`, `Counter`) gắn với quy tắc: đơn `Counter` bắt buộc có `CreatedByEmployeeId`, đơn `Online` bắt buộc để trống cột này. `UserId` cho phép NULL để hỗ trợ khách mua tại quầy không có tài khoản; khi đó thông tin khách nằm ở `CustomerName` và `CustomerPhone`. Ba cột tiền thể hiện công thức `FinalAmount = TotalAmount − DiscountAmount`, trong đó `TotalAmount` là tổng tiền ghế và combo, còn giảm giá hạng thành viên chỉ tính trên tiền vé, không áp dụng cho combo (chú thích trong `BookingService`). Mã đặt vé `BookingCode` do `CodeGenerator.GenerateBookingCode` sinh theo dạng tiền tố, ngày (`yyyyMMdd`) và 4 ký tự ngẫu nhiên, và được kiểm tra duy nhất khi tạo.

Bảng 4.62. Thực thể Booking (bảng `Bookings`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `BookingId` | int | Không | PK | Identity (tự tăng) | Mã định danh đơn đặt vé. |
| `UserId` | int | Có | FK | Tham chiếu `Users`; `Restrict` | Khách hàng có tài khoản; NULL với khách vãng lai mua tại quầy. |
| `ShowtimeId` | int | Không | FK | Tham chiếu `Showtimes`; `Restrict` | Suất chiếu được đặt. |
| `BookingCode` | varchar(20) | Không | UK | — | Mã đặt vé hiển thị cho khách và dùng tra cứu, không trùng nhau. |
| `Channel` | varchar(20) | Không | — | `BookingChannel.Online` (lưu chuỗi `Online`); CHECK `Online`/`Counter` | Kênh tạo đơn: trực tuyến hoặc tại quầy. |
| `CreatedByEmployeeId` | int | Có | FK | Tham chiếu `Employees`; `Restrict` | Nhân viên lập đơn tại quầy; NULL với đơn trực tuyến. |
| `CustomerName` | varchar(100) | Không | — | — | Họ tên khách đặt vé (tên người dùng hoặc tên khách tại quầy). |
| `CustomerPhone` | varchar(20) | Có | — | — | Số điện thoại khách, nếu có. |
| `TotalAmount` | decimal(18,2) | Không | — | CHECK `>= 0` | Tổng tiền ghế và combo trước giảm giá. |
| `DiscountAmount` | decimal(18,2) | Không | — | `0m`; CHECK `>= 0` | Số tiền giảm theo hạng thành viên (tính trên tiền vé). |
| `FinalAmount` | decimal(18,2) | Không | — | CHECK `>= 0` | Số tiền phải thanh toán sau giảm giá. |
| `Status` | varchar(20) | Không | — | `BookingStatus.Pending` (lưu chuỗi `Pending`); CHECK thuộc 4 giá trị | Trạng thái đơn: chờ thanh toán, đã xác nhận, đã hủy, hết hạn. |
| `CreatedAt` | timestamp without time zone | Không | — | SQL: `NOW()` | Thời điểm tạo đơn. |
| `ExpiresAt` | timestamp without time zone | Có | — | — | Hạn thanh toán của đơn `Pending`, trùng với hạn giữ ghế. |

**Thực thể BookingTicket.** `BookingTicket` là vé của một ghế, chỉ được tạo khi đơn được xác nhận (`ConfirmBookingAndGenerateTicketsAsync`): mỗi ghế đã giữ sinh một vé với `Price` sao chép từ `ShowtimeSeat.Price` và `QrCode` có dạng `<BookingCode>-<SeatRow><SeatNumber>` (`CodeGenerator.GenerateTicketQrPayload`). Khi soát vé, cột `IsCheckedIn` chuyển `true` và `CheckedInAt` ghi thời điểm; trạng thái đã soát vé cũng là điều kiện để khách được đánh giá phim (Mục 4.3.6). Xóa đơn kéo theo xóa vé (`Cascade`); ghế theo suất đã có vé không thể bị xóa (`Restrict`).

Bảng 4.63. Thực thể BookingTicket (bảng `BookingTickets`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `TicketId` | int | Không | PK | Identity (tự tăng) | Mã định danh vé. |
| `BookingId` | int | Không | FK | Tham chiếu `Bookings`; `Cascade` | Đơn đặt vé chứa vé. |
| `ShowtimeSeatId` | int | Không | FK, UK | Tham chiếu `ShowtimeSeats`; `Restrict`; chỉ mục duy nhất | Ghế theo suất mà vé ứng với; mỗi ghế chỉ có một vé. |
| `Price` | decimal(18,2) | Không | — | CHECK `>= 0` | Giá vé của ghế tại thời điểm xác nhận. |
| `QrCode` | varchar(255) | Có | — | — | Chuỗi mã QR dùng để soát vé. |
| `IsCheckedIn` | boolean | Không | — | `false` | Vé đã được soát tại cửa phòng chiếu hay chưa. |
| `CheckedInAt` | timestamp without time zone | Có | — | — | Thời điểm soát vé; NULL nếu chưa soát. |

**Thực thể BookingCombo.** `BookingCombo` là dòng chi tiết combo trong một đơn, quan hệ nhiều–nhiều giữa `Booking` và `Combo` có thuộc tính riêng là số lượng và đơn giá. `UnitPrice` lưu giá combo tại thời điểm đặt nên việc đổi giá `Combo` sau này không làm thay đổi đơn cũ.

Bảng 4.64. Thực thể BookingCombo (bảng `BookingCombos`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `BookingComboId` | int | Không | PK | Identity (tự tăng) | Mã định danh dòng combo. |
| `BookingId` | int | Không | FK | Tham chiếu `Bookings`; `Cascade` | Đơn đặt vé chứa combo. |
| `ComboId` | int | Không | FK | Tham chiếu `Combos`; `Restrict` | Combo được chọn. |
| `Quantity` | int | Không | — | `1`; CHECK `> 0` | Số lượng combo. |
| `UnitPrice` | decimal(18,2) | Không | — | CHECK `>= 0` | Đơn giá combo tại thời điểm đặt. |

**Thực thể Combo.** `Combo` là danh mục sản phẩm bắp nước bán kèm vé. Cột `IsActive` cho phép ngừng bán một combo mà không xóa, giữ nguyên lịch sử các đơn đã dùng combo đó.

Bảng 4.65. Thực thể Combo (bảng `Combos`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `ComboId` | int | Không | PK | Identity (tự tăng) | Mã định danh combo. |
| `Name` | varchar(150) | Không | — | — | Tên combo hiển thị cho khách. |
| `Description` | varchar(500) | Có | — | — | Mô tả thành phần combo. |
| `Price` | decimal(18,2) | Không | — | CHECK `>= 0` | Giá bán hiện hành của combo. |
| `ImageUrl` | varchar(255) | Có | — | — | Đường dẫn ảnh combo. |
| `IsActive` | boolean | Không | — | `true` | Combo còn được bán hay đã ngừng. |

#### 4.3.9.3 Ràng buộc toàn vẹn

Các ràng buộc CHECK về tiền bảo đảm không có giá trị âm; `CK_Bookings_Channel` và `CK_Bookings_Status` giới hạn giá trị theo hai enum tương ứng. `CK_Booking_Channel_Employee` ràng buộc chéo kênh và nhân viên: đơn quầy phải biết nhân viên lập đơn, đơn trực tuyến không được mang nhân viên, tránh dữ liệu mâu thuẫn giữa kênh và người tạo. `CK_Booking_Customer_Identity` yêu cầu đơn phải xác định được khách qua `UserId` hoặc `CustomerName`; do cột `CustomerName` được khai báo NOT NULL nên trong thực tế điều kiện này luôn thỏa. Về khóa ngoại, đơn không thể bị xóa khi còn tham chiếu từ `Payment` (Bảng 4.69), người dùng, nhân viên và suất chiếu liên quan đến đơn được bảo vệ (`Restrict`); vé và dòng combo là dữ liệu phụ thuộc hoàn toàn vào đơn nên bị xóa theo (`Cascade`).

Bảng 4.66. Ràng buộc toàn vẹn module Đặt vé

| Thực thể | Tên | Loại | Biểu thức | Hành vi xóa | Mục đích bảo vệ |
|---|---|---|---|---|---|
| Booking | `CK_Bookings_TotalAmount` | CHECK | `"TotalAmount" >= 0` | — | Tổng tiền không âm. |
| Booking | `CK_Bookings_DiscountAmount` | CHECK | `"DiscountAmount" >= 0` | — | Tiền giảm không âm. |
| Booking | `CK_Bookings_FinalAmount` | CHECK | `"FinalAmount" >= 0` | — | Tiền phải trả không âm. |
| Booking | `CK_Bookings_Channel` | CHECK | `"Channel" IN ('Online','Counter')` | — | Kênh thuộc tập giá trị của `BookingChannel`. |
| Booking | `CK_Bookings_Status` | CHECK | `"Status" IN ('Pending','Confirmed','Cancelled','Expired')` | — | Trạng thái thuộc tập giá trị của `BookingStatus`. |
| Booking | `CK_Booking_Channel_Employee` | CHECK | `("Channel" = 'Counter' AND "CreatedByEmployeeId" IS NOT NULL) OR ("Channel" = 'Online' AND "CreatedByEmployeeId" IS NULL)` | — | Đơn quầy phải có nhân viên lập, đơn trực tuyến không có. |
| Booking | `CK_Booking_Customer_Identity` | CHECK | `"UserId" IS NOT NULL OR "CustomerName" IS NOT NULL` | — | Đơn phải xác định được khách. |
| BookingTicket | `CK_BookingTickets_Price` | CHECK | `"Price" >= 0` | — | Giá vé không âm. |
| BookingCombo | `CK_BookingCombos_Quantity` | CHECK | `"Quantity" > 0` | — | Số lượng combo phải dương. |
| BookingCombo | `CK_BookingCombos_UnitPrice` | CHECK | `"UnitPrice" >= 0` | — | Đơn giá combo không âm. |
| Combo | `CK_Combos_Price` | CHECK | `"Price" >= 0` | — | Giá combo không âm. |
| Booking | FK `UserId` | Khóa ngoại | — | Restrict | Không xóa người dùng còn đơn đặt vé. |
| Booking | FK `ShowtimeId` | Khóa ngoại | — | Restrict | Không xóa suất chiếu còn đơn đặt vé. |
| Booking | FK `CreatedByEmployeeId` | Khóa ngoại | — | Restrict | Không xóa nhân viên còn đơn do họ lập. |
| BookingTicket | FK `BookingId` | Khóa ngoại | — | Cascade | Xóa đơn thì xóa vé của đơn. |
| BookingTicket | FK `ShowtimeSeatId` | Khóa ngoại | — | Restrict | Không xóa ghế theo suất còn vé. |
| BookingCombo | FK `BookingId` | Khóa ngoại | — | Cascade | Xóa đơn thì xóa các dòng combo. |
| BookingCombo | FK `ComboId` | Khóa ngoại | — | Restrict | Không xóa combo còn được đơn sử dụng. |

#### 4.3.9.4 Chỉ mục

Chỉ mục duy nhất trên `BookingCode` bảo vệ tính duy nhất của mã đặt vé và phục vụ tra cứu đơn theo mã (tìm kiếm, soát vé). `IX_Bookings_User` trên `(UserId, CreatedAt)` với `CreatedAt` sắp giảm dần phục vụ truy vấn lịch sử đặt vé của một khách, mới nhất trước. `IX_Bookings_Showtime`, `IX_Bookings_Employee`, `IX_Bookings_Status` và `IX_Bookings_Channel` phục vụ lọc đơn theo suất chiếu, nhân viên, trạng thái và kênh trong màn hình quản lý booking và báo cáo. Chỉ mục duy nhất trên `BookingTicket.ShowtimeSeatId` bảo đảm mỗi ghế theo suất chỉ có một vé (lớp bảo vệ thứ hai chống đặt trùng, xem Mục 4.3.8.4). `IX_BookingCombos_Booking` phục vụ tải các combo của một đơn. Các chỉ mục `IX_BookingTickets_BookingId` và `IX_BookingCombos_ComboId` do EF tự sinh cho khóa ngoại được bổ sung vào bảng.

Bảng 4.67. Chỉ mục module Đặt vé

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| Booking | `IX_Bookings_BookingCode` (tên tự sinh) | Duy nhất | `BookingCode` | — |
| Booking | `IX_Bookings_User` | Thường | `UserId`, `CreatedAt` (giảm dần) | — |
| Booking | `IX_Bookings_Showtime` | Thường | `ShowtimeId` | — |
| Booking | `IX_Bookings_Employee` | Thường | `CreatedByEmployeeId` | — |
| Booking | `IX_Bookings_Status` | Thường | `Status` | — |
| Booking | `IX_Bookings_Channel` | Thường | `Channel` | — |
| BookingTicket | `IX_BookingTickets_ShowtimeSeatId` (tên tự sinh) | Duy nhất | `ShowtimeSeatId` | — |
| BookingTicket | `IX_BookingTickets_BookingId` (tên tự sinh) | Thường | `BookingId` | — |
| BookingCombo | `IX_BookingCombos_Booking` | Thường | `BookingId` | — |
| BookingCombo | `IX_BookingCombos_ComboId` (tên tự sinh) | Thường | `ComboId` | — |

### 4.3.10 Module Thanh toán

Module Thanh toán lưu các lần thanh toán và hoàn tiền của đơn đặt vé; gồm một thực thể là `Payment`. `Payment` tham chiếu `Booking` của module Đặt vé (Mục 4.3.9): một đơn có thể có nhiều bản ghi thanh toán, ví dụ một lần thanh toán thất bại rồi thử lại, hoặc một lần thanh toán thành công sau đó được hoàn tiền. Thanh toán qua cổng trực tuyến (`VNPay`, `Momo`, `Stripe`) và thanh toán tiền mặt tại quầy (`Cash`) cùng dùng một bảng; các cột `AmountReceived` và `ChangeGiven` chỉ có ý nghĩa với tiền mặt.

#### 4.3.10.1 Sơ đồ ERD của module

Hình 4.33 thể hiện thực thể sở hữu `Payment` với đầy đủ thuộc tính và thực thể tham chiếu `Booking` chỉ có khóa chính. Quan hệ `Booking` – `Payment` là một–nhiều, bắt buộc ở phía `Payment` (`BookingId` không cho phép NULL).

![Hình 4.33. ERD module Thanh toán](hinh/4.33.png)

*Hình 4.33. ERD module Thanh toán*

#### 4.3.10.2 Mô tả thực thể

**Thực thể Payment.** Một bản ghi `Payment` được tạo ở trạng thái `Pending` khi khách chuyển sang cổng thanh toán; khi cổng gọi lại, `PaymentService` ghi `GatewayResponseCode` và chuyển trạng thái sang `Success` (kèm `PaidAt`) hoặc `Failed`. Khi đơn quá hạn mà còn thanh toán `Pending`, thanh toán bị chuyển `Failed`. Với thanh toán tại quầy, `BookingService.CounterCheckoutAsync` tạo ngay bản ghi `Cash` ở trạng thái `Success` với `AmountReceived` là tiền khách đưa và `ChangeGiven = AmountReceived − FinalAmount`. Khi hoàn tiền (hủy vé hoặc hoàn tự động khi suất bị hủy), trạng thái chuyển `Refunded` và ba cột `RefundedAmount`, `RefundedAt`, `RefundTransactionId` được ghi; các báo cáo doanh thu tính giá trị thuần bằng `Amount` trừ `RefundedAmount`. `PaymentMethod` lưu dưới dạng chuỗi theo enum `PaymentMethod` và không có giá trị mặc định ở cơ sở dữ liệu. Cột `TransactionId` là mã giao dịch phía hệ thống (sinh từ mã đặt vé và thời điểm bằng `CodeGenerator.GeneratePaymentTransactionId`), còn `GatewayTransactionId` là mã giao dịch do cổng thanh toán trả về.

Bảng 4.68. Thực thể Payment (bảng `Payments`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `PaymentId` | int | Không | PK | Identity (tự tăng) | Mã định danh bản ghi thanh toán. |
| `BookingId` | int | Không | FK | Tham chiếu `Bookings`; `Restrict` | Đơn đặt vé được thanh toán. |
| `PaymentMethod` | varchar(50) | Không | — | Lưu chuỗi theo enum `PaymentMethod` (`VNPay`, `Momo`, `Cash`, `Stripe`); CHECK thuộc 4 giá trị; không có default | Phương thức thanh toán. |
| `TransactionId` | varchar(150) | Có | — | — | Mã giao dịch do hệ thống sinh, gửi sang cổng thanh toán. |
| `GatewayTransactionId` | varchar(150) | Có | — | — | Mã giao dịch do cổng thanh toán trả về. |
| `GatewayResponseCode` | varchar(20) | Có | — | — | Mã phản hồi của cổng khi gọi lại kết quả. |
| `Amount` | decimal(18,2) | Không | — | CHECK `>= 0` | Số tiền thanh toán (bằng `FinalAmount` của đơn). |
| `Status` | varchar(20) | Không | — | `PaymentStatus.Pending` (lưu chuỗi `Pending`); CHECK thuộc 4 giá trị | Trạng thái thanh toán: chờ, thành công, thất bại, đã hoàn tiền. |
| `PaidAt` | timestamp without time zone | Có | — | — | Thời điểm thanh toán thành công. |
| `CreatedAt` | timestamp without time zone | Không | — | SQL: `NOW() AT TIME ZONE 'UTC'` | Thời điểm tạo bản ghi thanh toán (giờ UTC). |
| `RefundTransactionId` | varchar(150) | Có | — | — | Mã giao dịch hoàn tiền do cổng trả về. |
| `RefundedAmount` | decimal(18,2) | Có | — | — | Số tiền đã hoàn; NULL nếu chưa hoàn. |
| `RefundedAt` | timestamp without time zone | Có | — | — | Thời điểm hoàn tiền. |
| `AmountReceived` | decimal(18,2) | Có | — | — | Số tiền mặt khách đưa (chỉ với thanh toán `Cash`). |
| `ChangeGiven` | decimal(18,2) | Có | — | — | Tiền thối lại khách (chỉ với thanh toán `Cash`). |

#### 4.3.10.3 Ràng buộc toàn vẹn

Ba ràng buộc CHECK giới hạn phương thức và trạng thái theo hai enum và bảo đảm số tiền không âm. Khóa ngoại `BookingId` dùng `Restrict`: không xóa được đơn còn lịch sử thanh toán, nhằm giữ chứng từ tài chính. Cơ sở dữ liệu không đặt ràng buộc CHECK cho các cột hoàn tiền (ví dụ `RefundedAmount` không vượt `Amount`); điều kiện này do tầng dịch vụ kiểm soát.

Bảng 4.69. Ràng buộc toàn vẹn module Thanh toán

| Thực thể | Tên | Loại | Biểu thức | Hành vi xóa | Mục đích bảo vệ |
|---|---|---|---|---|---|
| Payment | `CK_Payments_Method` | CHECK | `"PaymentMethod" IN ('VNPay','Momo','Cash','Stripe')` | — | Phương thức thuộc tập giá trị của `PaymentMethod`. |
| Payment | `CK_Payments_Amount` | CHECK | `"Amount" >= 0` | — | Số tiền thanh toán không âm. |
| Payment | `CK_Payments_Status` | CHECK | `"Status" IN ('Pending','Success','Failed','Refunded')` | — | Trạng thái thuộc tập giá trị của `PaymentStatus`. |
| Payment | FK `BookingId` | Khóa ngoại | — | Restrict | Không xóa đơn còn bản ghi thanh toán. |

#### 4.3.10.4 Chỉ mục

`IX_Payments_Booking` phục vụ tải các lần thanh toán của một đơn (kiểm tra thanh toán thành công khi hủy vé, xử lý gọi lại của cổng). `IX_Payments_Status` phục vụ lọc theo trạng thái trong quản lý thanh toán và báo cáo doanh thu. Cấu hình không khai báo chỉ mục duy nhất trên `TransactionId` hay `GatewayTransactionId`.

Bảng 4.70. Chỉ mục module Thanh toán

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| Payment | `IX_Payments_Booking` | Thường | `BookingId` | — |
| Payment | `IX_Payments_Status` | Thường | `Status` | — |

<!-- GHI CHÚ ĐỐI CHIẾU
(a) [CẦN BỔ SUNG] còn lại:
- Mã Mermaid ERD Hình 4.31, 4.32, 4.33 (`ma-nguon-so-do.md`) KHÔNG có trong tài liệu đính kèm (`ma-nguon-so-do-bo-sung.md` chỉ có use case và sơ đồ lớp; `ma-nguon-so-do-mermaid-loyalty.md` chỉ có Hình 4.26, 4.34, 4.51, 4.54, 4.58). Phần mô tả Hình 4.31–4.33 viết theo cấu hình EF và snapshot, chưa đối chiếu được với mã Mermaid. Cần gửi `ma-nguon-so-do.md`; ảnh `hinh/4.31.png`–`4.33.png` chưa có.
- Không có trích dẫn tài liệu tham khảo trong phạm vi này.
- Quan hệ Booking–LoyaltyPointEntry (Hình 4.34) chỉ nhắc tới, thuộc Mục 4.3.11.
(b) Chỗ outline lệch code:
- Các cột tiền trong outline ghi `decimal`; code/snapshot: `decimal(18,2)` cho BasePrice, Price, TotalAmount, DiscountAmount, FinalAmount, Amount, RefundedAmount, AmountReceived, ChangeGiven, UnitPrice.
- Outline ghi `timestamp`; code: `timestamp without time zone`.
- Bảng 4.60, 4.66, 4.69: outline chỉ có 4 cột; đã bổ sung "Hành vi xóa" và "Mục đích bảo vệ" (đọc từ Configurations, khớp snapshot). Hành vi xóa: ShowtimeSeat.HeldByBookingId = SetNull (cấu hình `HasOne<Booking>()` không có navigation); ShowtimeSeat.ShowtimeId = Cascade; BookingTicket.BookingId và BookingCombo.BookingId = Cascade; các FK còn lại Restrict.
- Chỉ mục thiếu trong outline (EF tự sinh cho FK, có trong snapshot và migration InitialCreate): `IX_ShowtimeSeats_SeatId`, `IX_ShowtimeSeats_HeldByUserId`, `IX_ShowtimeSeats_HeldByBookingId` (Bảng 4.61); `IX_BookingTickets_BookingId`, `IX_BookingCombos_ComboId` (Bảng 4.67). Đã thêm; tên chỉ mục tự sinh ghi theo migration (IX_Bookings_BookingCode, IX_BookingTickets_ShowtimeSeatId).
- `IX_Bookings_User` có CreatedAt sắp giảm dần (`IsDescending(false, true)`); outline không nêu thứ tự.
- `CK_Booking_Customer_Identity`: vì `Booking.CustomerName` là NOT NULL (`IsRequired`) nên điều kiện CHECK luôn đúng; đã nêu trong 4.3.9.3.
- `Payments.PaymentMethod`: outline ghi "enum PaymentMethod"; code lưu chuỗi varchar(50), không có default SQL.
- Không có chỉ mục lọc (`HasFilter`) nào trong các bảng của phạm vi này; chống đặt trùng ghế dựa vào `UQ_Showtime_Seat` (không điều kiện), chỉ mục duy nhất `BookingTickets.ShowtimeSeatId` và câu `ExecuteUpdate` có điều kiện trong `BookingService` (khoảng dòng 1038–1078). Không có CHECK liên kết Status với HeldBy*/HeldUntil của ShowtimeSeat.
- Chống trùng lịch phòng chiếu không có ràng buộc DB (IX_Showtime_Room_Time không duy nhất); do `EnsureNoOverlapAsync` (ShowtimeService, gọi ở dòng 129 và 264) — chưa đọc thân hàm này, chỉ dựa vào tên và vị trí gọi; cần xác nhận khi viết phần cài đặt.
- Mô tả "quy tắc sinh `BookingCode`: tiền tố + yyyyMMdd + 4 ký tự" lấy từ CodeGenerator.GenerateBookingCode; tiền tố có thể cấu hình (`DefaultBookingCodePrefix`), chưa kiểm tra giá trị mặc định. Việc "kiểm tra duy nhất khi tạo" dựa trên tên hàm `GenerateUniqueBookingCodeAsync`, chưa đọc thân hàm.
-->

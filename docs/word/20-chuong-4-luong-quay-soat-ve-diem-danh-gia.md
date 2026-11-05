### 4.5.5 Bán vé tại quầy

Kênh bán vé tại quầy dành cho nhân viên và quản trị viên (chính sách `StaffOnly`, gồm hai vai trò `Admin` và `Employee`). Luồng gồm hai lời gọi liên tiếp: `POST /api/v1/admin/bookings/counter-hold` để giữ ghế và tạo booking ở trạng thái chờ, sau đó `POST /api/v1/admin/bookings/{bookingId}/counter-checkout` để ghi nhận thanh toán tiền mặt, xác nhận booking và phát vé. Tại `AdminBookingsController`, lời gọi giữ ghế chỉ được xử lý khi tài khoản đăng nhập có hồ sơ nhân viên; nếu không, hệ thống ném `BusinessRuleException` với thông báo `Messages.Rule_NoEmployeeProfileForCounterBooking`.

![Hình 4.55. Tuần tự bán vé tại quầy](hinh/4.55.png)

*Hình 4.55. Tuần tự bán vé tại quầy*

**Giữ ghế kênh Counter.** Hàm `CounterHoldSeatsAsync` thực hiện theo thứ tự sau. Trước hết, hàm nạp sơ đồ ghế của suất chiếu bằng `LoadSeatSlotsAsync` rồi kiểm tra lựa chọn bằng `SeatLayoutRules.ValidateSelection` với chế độ `SeatGapRuleMode.Off`; nghĩa là kênh quầy không áp dụng luật chừa ghế lẻ (Mục 4.6.1), trong khi các ràng buộc còn lại của lựa chọn ghế vẫn được kiểm tra. Tiếp theo, hàm xác định thông tin khách. Nếu yêu cầu có `UserId`, hệ thống tra người dùng cùng vai trò, ném `NotFoundException` (`Messages.NotFound_Customer`) khi không thấy, ném `BusinessRuleException` khi tài khoản bị khóa (`Messages.Rule_CustomerAccountDisabled`) hoặc không thuộc vai trò `Customer` (`Messages.Rule_UserIsNotCustomer`), và lấy `FullName`, `Phone` của khách làm thông tin booking. Nếu không có `UserId` (khách vãng lai), tên và số điện thoại lấy trực tiếp từ yêu cầu. Lược đồ kiểm tra `CounterHoldRequestValidator` quy định: danh sách ghế không rỗng, không trùng, không vượt `BookingOptions.MaxSeatsPerBooking` (mặc định 10); `CustomerName` bắt buộc khi không có `UserId` và tối đa 100 ký tự; số điện thoại, nếu có, phải khớp biểu thức `^(0|\+84)[0-9]{9,10}$`.

Phần giữ ghế dùng chung hàm riêng `CreateHoldBookingAsync` với kênh Online (cơ chế giữ ghế nguyên tử đã trình bày ở Mục 4.5.1.2), với các tham số đặc thù của quầy: `channel = BookingChannel.Counter`, `holderUserId` là `UserId` của nhân viên (người giữ ghế là nhân viên, không phải khách), `bookingUserId` là `UserId` của khách thành viên nếu có, và `createdByEmployeeId` là mã nhân viên. Trong hàm này, suất chiếu phải ở trạng thái `Scheduled` và chưa bắt đầu; quy tắc ngừng bán trực tuyến (`StopSellingMinutesBefore`) chỉ áp dụng cho kênh Online nên nhân viên vẫn bán được sát giờ chiếu. Mỗi nhân viên chỉ được có một booking chờ còn hạn trên cùng một suất chiếu, nếu không hệ thống ném `ConflictAppException` (`Messages.Conflict_PendingBookingExistsForShowtime`). Thời điểm hết hạn giữ là `now + HoldDurationMinutes` (mặc định 15 phút, `CinemaSettings.DefaultHoldDurationMinutes`) và bị chặn không vượt quá giờ bắt đầu suất chiếu. Các ghế được chuyển sang `Held` bằng một lệnh cập nhật có điều kiện (ghế phải đang `Available`, hoặc đang `Held` bởi cùng người giữ nhưng đã quá hạn); nếu số dòng được cập nhật khác số ghế yêu cầu thì giao dịch bị hủy và hệ thống ném `SeatConflictException` kèm danh sách ghế xung đột. Cuối cùng, hệ thống tính `TotalAmount` bằng tổng giá ghế cộng tiền combo, tính `DiscountAmount` bằng `CalculateDiscountAmountAsync` (chỉ áp dụng trên tiền vé, xem Mục 4.5.1.3 và Mục 4.5.8), ghi `FinalAmount = TotalAmount − DiscountAmount`, tạo booking `Pending` với `ExpiresAt` bằng thời điểm hết hạn giữ, gắn `HeldByBookingId` cho các ghế, xác nhận giao dịch rồi phát thông báo ghế đã được giữ qua kênh thời gian thực. Phản hồi `CounterHoldResponse` gồm mã và mã code booking, trạng thái, ba khoản tiền, `ExpiresAt`, kênh và mã nhân viên tạo.

**Thanh toán tiền mặt, tiền thối, xác nhận và phát vé.** Hàm `CounterCheckoutAsync` kiểm tra lần lượt: booking tồn tại (`NotFoundException`); booking thuộc kênh Counter (`Messages.Rule_CounterPaymentOnlyForCounterBooking`), nhờ đó booking trực tuyến không thể bị thanh toán qua đường này; booking còn ở trạng thái `Pending` và chưa quá `ExpiresAt` (`Messages.Rule_BookingNotPendingOrHoldExpired`); và số tiền nhận `AmountReceived` không nhỏ hơn `FinalAmount` (`Messages.Rule_AmountReceivedInsufficient`). `CounterCheckoutRequestValidator` còn yêu cầu `AmountReceived` lớn hơn 0. Tiền thối được tính là `change = AmountReceived − FinalAmount`. Trong một giao dịch cơ sở dữ liệu, hàm thêm một bản ghi `Payment` với `PaymentMethod.Cash`, `PaymentStatus.Success`, `Amount = FinalAmount`, `AmountReceived`, `ChangeGiven = change` và `PaidAt`, lưu bản ghi, gọi `ConfirmBookingAndGenerateTicketsAsync` rồi mới xác nhận giao dịch; do hàm xác nhận phát hiện đã có giao dịch đang mở nên không tự mở giao dịch riêng. Phản hồi là `CounterCheckoutResponse(bookingId, "Confirmed", change)`.

Hàm `ConfirmBookingAndGenerateTicketsAsync` là bước phát vé dùng chung với kênh trực tuyến (Mục 4.5.2.3), nên ở đây chỉ nêu các điểm liên quan đến quầy. Hàm này bỏ qua nếu booking đã `Confirmed`; ném `SeatsNoLongerHeldException` nếu booking đã `Cancelled` hoặc `Expired`, hoặc nếu không còn ghế nào đang được giữ cho booking. Với mỗi ghế đang giữ, hàm chuyển ghế sang `Booked`, xóa các trường giữ chỗ và tạo một `BookingTicket` có mã QR theo định dạng `{BookingCode}-{SeatRow}{SeatNumber}` (do `CodeGenerator.GenerateTicketQrPayload` sinh). Điểm thưởng chỉ được cộng khi booking gắn với một tài khoản (`booking.UserId` khác `null`, xem Mục 4.5.8), và email xác nhận cũng chỉ được xếp hàng cho trường hợp này; khách vãng lai mua tại quầy do đó không được tích điểm và không nhận email. Sau khi lưu, hệ thống phát thông báo ghế đã được bán qua kênh thời gian thực.

**Giao diện quầy.** Hook `useCounterFlow` (`frontend/src/features/admin/counter/hooks/use-counter-flow.ts`) mô hình hóa luồng thành bốn bước `showtime` → `seats` → `checkout` → `done`. Nút giữ ghế chỉ được bật khi đã chọn suất, đã chọn ít nhất một ghế và có `userId` của khách thành viên hoặc tên khách nhập tay. Hook dùng `useRealtimeSeatMap` để cập nhật trạng thái ghế; khi hết hạn giữ, giao diện hiển thị cảnh báo "Thời gian giữ ghế đã hết hạn." và quay về bước chọn suất. Ở bước thanh toán, `counter-checkout-panel.tsx` khởi tạo số tiền nhận bằng `finalAmount`, hiển thị tiền thối bằng `max(0, amountReceived − finalAmount)` và đánh dấu thiếu tiền khi `amountReceived < finalAmount`; việc kiểm tra quyết định vẫn do máy chủ thực hiện như trên.

### 4.5.6 Soát vé

Soát vé được thực hiện qua `POST /api/v1/admin/bookings/checkin` (chính sách `StaffOnly`) với thân yêu cầu `CheckinRequest(QrCode)`; `CheckinRequestValidator` yêu cầu `QrCode` không rỗng. Hàm `CheckinAsync` tra vé theo `QrCode` kèm ghế, suất chiếu, phim và booking, rồi áp dụng chuỗi điều kiện theo thứ tự dưới đây; điều kiện nào không thỏa thì dừng với lỗi tương ứng.

![Hình 4.56. Lưu đồ soát vé](hinh/4.56.png)

*Hình 4.56. Lưu đồ soát vé*

Thứ nhất, không tìm thấy vé có mã QR đó thì ném `NotFoundException` (`Messages.NotFound_QrCode`). Thứ hai, booking của vé phải ở trạng thái `Confirmed`, nếu không ném `BusinessRuleException` (`Messages.Rule_TicketNoLongerValid`), điều này loại các vé thuộc booking đã hủy. Thứ ba, vé chưa được soát: nếu `IsCheckedIn` đã bằng `true`, hệ thống ném `ConflictAppException` (`Messages.Conflict_TicketAlreadyCheckedIn`). Thứ tư, suất chiếu không ở trạng thái `Cancelled` (`Messages.Rule_ShowtimeCancelled`). Thứ năm, thời điểm hiện tại (UTC) không sớm hơn `StartTime − CheckInOpenMinutes`; `CheckInOpenMinutes` đọc từ `CinemaSettings`, mặc định 30 phút (`DefaultCheckInOpenMinutes`), và vi phạm thì lỗi là `Messages.Rule_CheckInNotOpenYet` kèm số phút. Thứ sáu, thời điểm hiện tại không muộn hơn `EndTime` của suất chiếu (`Messages.Rule_ShowtimeEnded`); `EndTime` được ghi lúc tạo hoặc sửa suất bằng `StartTime` cộng thời lượng phim và `RoomBufferMinutes`, vì vậy khoảng thời gian đệm giữa các suất cũng nằm trong thời hạn cho phép soát.

Để chống soát lặp khi có hai thiết bị cùng quét một vé, hàm không dựa vào giá trị `IsCheckedIn` đã đọc ở bước trên mà thực hiện một lệnh `ExecuteUpdateAsync` có điều kiện `TicketId = @id AND IsCheckedIn = false`, đặt `IsCheckedIn = true` và `CheckedInAt = now`. Nếu số dòng bị ảnh hưởng bằng 0, tức một lời gọi khác đã soát vé trước, hệ thống lại ném `Conflict_TicketAlreadyCheckedIn`. Cách làm này chuyển việc phân xử cạnh tranh về một câu lệnh nguyên tử của cơ sở dữ liệu, tương tự cơ chế giữ ghế ở Mục 4.5.1.2. Khi thành công, phản hồi `CheckinResponse` gồm `TicketId`, `BookingId`, hàng và số ghế, tên phim, `StartTime` và `CheckedInAt`. Việc soát vé là điều kiện để khách được phép đánh giá phim (Mục 4.5.9).

**Quét camera hoặc nhập mã.** Trang soát vé (`checkin-page.tsx`) cho phép hai cách nhập: gõ hoặc dán mã vào ô nhập rồi bấm "Soát vé ngay", hoặc bật camera. Hàm xử lý chung `handleScan` cắt khoảng trắng, bỏ qua giá trị rỗng hoặc khi yêu cầu trước còn đang xử lý, gọi API soát vé, rồi phát âm thanh thành công hoặc lỗi (`useCheckinSound`) và hiển thị thông báo lỗi của máy chủ khi có. Hook `useQrCameraScanner` mở camera sau (`facingMode: "environment"`), duyệt từng khung hình bằng `requestAnimationFrame`, và chọn bộ giải mã theo khả năng của trình duyệt: nếu có `window.BarcodeDetector` thì dùng với định dạng `qr_code`; nếu không thì vẽ khung hình lên canvas và giải mã bằng thư viện `jsQR` (`inversionAttempts: "dontInvert"`). Để tránh gửi lặp cùng một mã khi mã còn nằm trong khung hình, hook bỏ qua giá trị trùng với lần quét trước nếu chưa qua `QR_SCAN_REPEAT_GUARD_MS` (2500 ms). Hook cũng phân biệt các lỗi camera: bị từ chối quyền, không có thiết bị, hoặc kết nối không phải HTTPS (ngoại trừ `localhost`).

### 4.5.7 Hủy suất chiếu

Hệ thống không xóa vật lý suất chiếu đã có thể phát sinh booking; thao tác "xóa" của quản trị viên thực chất là hủy: `DeleteShowtimeAsync` đặt `ShowtimeStatus.Cancelled` và xử lý hệ quả lên các booking, thanh toán liên quan. Cách thiết kế này đi cùng quy tắc sửa suất chiếu của `UpdateShowtimeAsync`: chỉ suất `Scheduled` mới được sửa; không được đổi phòng (`Messages.Rule_CannotChangeShowtimeRoom`); không được sửa khi đã có booking `Confirmed` (`Messages.Conflict_ShowtimeHasConfirmedBookings`); và cũng không được sửa khi còn booking `Pending` còn hạn hoặc còn ghế đang `Booked` hay đang `Held` còn hạn (`Messages.Conflict_ShowtimeHasPendingBookings`). Như vậy, một suất đã có người đặt chỉ có thể kết thúc bằng con đường hủy, và việc hoàn tiền cho khách được xử lý tập trung tại đó.

![Hình 4.57. Tuần tự hủy suất chiếu](hinh/4.57.png)

*Hình 4.57. Tuần tự hủy suất chiếu*

Thứ tự thực thi của `DeleteShowtimeAsync` như sau. Hàm kiểm tra suất chiếu tồn tại, phải ở trạng thái `Scheduled` (`EnsureScheduled`, thông báo `Messages.Rule_ShowtimeMustBeScheduled`) và chưa bắt đầu (`Messages.Rule_ShowtimeStartedCannotCancel`). Sau đó, trong một giao dịch cơ sở dữ liệu, hàm đặt trạng thái suất là `Cancelled`, nạp mọi booking `Pending` hoặc `Confirmed` của suất kèm các thanh toán và người dùng, rồi xử lý từng booking:

- Chọn thanh toán thành công mới nhất (`PaymentStatus.Success`, sắp theo `PaymentId` giảm dần) làm thanh toán cần hoàn, và chuyển booking sang `Cancelled`.
- Với booking vốn `Pending`, mọi thanh toán `Pending` chuyển sang `Failed`.
- Với booking vốn `Confirmed` và có tài khoản, gọi `bookingService.ReverseLoyaltyAsync(userId, bookingId, FinalAmount)` để thu hồi điểm theo công thức (4.7) (Mục 4.5.8); thao tác này diễn ra trong cùng giao dịch và không chờ kết quả hoàn tiền của cổng thanh toán.
- Nếu booking không có thanh toán thành công thì bỏ qua bước hoàn tiền. Nếu thanh toán là tiền mặt, hệ thống không thể hoàn tự động: bộ đếm `manualRefundCount` tăng lên và, khi booking có tài khoản, một email `ShowtimeCancelledManualRefund` được đưa vào danh sách gửi. Các thanh toán còn lại được ghi vào danh sách `refundPaymentIds`.

Cuối giao dịch, mọi ghế của suất đang `Held` được đưa về `Available` và xóa các trường giữ chỗ bằng một lệnh cập nhật hàng loạt, rồi giao dịch được xác nhận. Chỉ sau khi giao dịch thành công, hàm mới xếp hàng các công việc nền: gửi từng email (`SendEmailJob`), rồi xếp `ShowtimeRefundJob.RunAsync(paymentId, …)` cho từng thanh toán cần hoàn tự động. Lỗi khi xếp hàng được ghi nhật ký và không làm hỏng việc hủy suất. Kết quả trả về là `DeleteShowtimeResponse(showtimeId, "Cancelled", manualRefundCount, refundPaymentIds.Count)`, cho phép giao diện quản trị báo số booking cần hoàn tiền thủ công và số booking đã xếp hàng hoàn tự động. Hàm `BulkDeleteShowtimesAsync` gọi lại `DeleteShowtimeAsync` lần lượt cho từng mã suất (đã loại trùng) và ghi kết quả thành công hoặc lỗi riêng cho mỗi suất.

**Thử lại hoàn tiền.** `ShowtimeRefundJob` được tách khỏi giao dịch hủy suất để lỗi của cổng thanh toán không làm hỏng việc hủy. Công việc nạp thanh toán, thoát sớm nếu thanh toán không còn ở trạng thái `Success`, nếu booking hoặc suất chiếu chưa ở trạng thái `Cancelled` (kèm cảnh báo), hoặc nếu là tiền mặt. Công việc tìm cổng theo `PaymentMethod` và gọi `RefundAsync` với toàn bộ `payment.Amount`, lý do "Hoàn tiền do rạp hủy suất chiếu" cùng mã booking; thiếu cổng cấu hình hoặc ngoại lệ đều được coi là hoàn tiền thất bại. Khi thành công, thanh toán chuyển sang `Refunded`, ghi `RefundedAmount = Amount`, `RefundedAt`, `RefundTransactionId` và gửi email `ShowtimeCancelledAutoRefunded` cho khách có tài khoản. Khi thất bại, công việc đọc tham số `RetryCount` của Hangfire (nếu không có ngữ cảnh thì coi như đã dùng hết lượt) và so với `BackgroundJobOptions.RetryAttempts` (mặc định 3): khi còn lượt, công việc ném `InvalidOperationException` để Hangfire thử lại; ở lượt cuối, công việc gửi email `ShowtimeCancelledManualRefund` thông báo khách sẽ được hoàn tiền thủ công rồi vẫn ném ngoại lệ để công việc được ghi nhận là thất bại và quản trị viên có thể xử lý thủ công.

### 4.5.8 Điểm thưởng

**Sổ điểm.** Điểm thành viên được ghi theo sổ điểm: mỗi dòng là một thực thể `LoyaltyPointEntry` gồm `EntryId`, `UserId`, `BookingId` (có thể `null`), `Points`, `RevokedPoints` và `EarnedAt`. Mỗi booking đã xác nhận, có tài khoản và sinh ra điểm dương thì tạo một dòng, với `EarnedAt` là thời điểm xác nhận thanh toán thành công. `BookingId = null` được dùng cho các dòng không gắn với booking; trong mã nguồn hiện có, trường hợp này là dữ liệu mẫu của khách thử nghiệm do `DbInitializer` tạo. Cấu hình `LoyaltyPointEntryConfiguration` đặt các ràng buộc ở mức cơ sở dữ liệu: `Points ≥ 0` (`CK_LoyaltyPointEntries_Points`); `0 ≤ RevokedPoints ≤ Points` (`CK_LoyaltyPointEntries_Revoked`); chỉ mục duy nhất có điều kiện `BookingId IS NOT NULL` tên `UX_LoyaltyPointEntries_Booking`, bảo đảm mỗi booking tối đa một dòng và chống cộng điểm hai lần khi cổng thanh toán gọi lại; và chỉ mục `IX_LoyaltyPointEntries_User_EarnedAt` phục vụ truy vấn theo cửa sổ thời gian.

**Điểm tích, điểm hiệu lực và hạng.** Điểm tích của một booking tính trên số tiền khách thực trả `FinalAmount` (bằng `LoyaltyWindow.PointsFor`), theo công thức (4.4), với `PointsPerAmount` đọc từ `CinemaSettings` (mặc định 10.000, khởi tạo trong `SeedData.cs`). Vì `FinalAmount` đã gồm tiền combo, phần combo vẫn được tích điểm dù không được giảm giá.

```
earned = ⌊FinalAmount / PointsPerAmount⌋                                          (4.4)
```

Điểm của khách không phải là tổng trọn đời mà là điểm hiệu lực trong cửa sổ trượt 12 tháng (`LoyaltyWindow.Months = 12`). Theo (4.5), chỉ các dòng có `EarnedAt` lớn hơn mốc `t − 12 tháng` (`LoyaltyWindow.Cutoff`) được cộng, sau khi trừ phần đã thu hồi (`LoyaltyWindow.SumActiveAsync`).

```
P(u, t) = Σ (Points − RevokedPoints), lấy trên các dòng của u có EarnedAt > t − 12 tháng   (4.5)
```

Hạng thành viên là hạng có `MinPoints` lớn nhất mà không vượt điểm hiệu lực (`LoyaltyWindow.ResolveTierAsync`), theo (4.6). Do `P(u, t)` giảm khi dòng cũ trượt khỏi cửa sổ, hạng của khách có thể bị hạ theo thời gian mà không cần phát sinh giao dịch nào.

```
tier(u, t) = hạng có MinPoints lớn nhất thỏa MinPoints ≤ P(u, t)                  (4.6)
```

Bảng 4.84 liệt kê bốn hạng được khởi tạo trong `SeedData.cs`.

*Bảng 4.84. Hạng thành viên*

| TierId | Hạng (`TierName`) | `MinPoints` | `DiscountPercent` (%) | Ý nghĩa |
|---|---|---|---|---|
| 1 | Thành viên | 0 | 0 | Hạng mặc định của mọi khách có hồ sơ; không giảm giá |
| 2 | Bạc | 500 | 5 | Giảm 5% tiền vé khi điểm hiệu lực từ 500 |
| 3 | Vàng | 2000 | 10 | Giảm 10% tiền vé khi điểm hiệu lực từ 2000 |
| 4 | Kim Cương | 5000 | 15 | Giảm 15% tiền vé khi điểm hiệu lực từ 5000 |

*Nguồn: `Cinema.Infrastructure/Persistence/Seed/SeedData.cs` (dữ liệu khởi tạo của `MembershipTier`).*

**Cộng điểm khi xác nhận.** Hàm `ApplyLoyaltyAsync`, được gọi từ `ConfirmBookingAndGenerateTicketsAsync` trong cùng giao dịch phát vé, tính `earned` theo (4.4); nếu `earned > 0` thì thêm dòng sổ điểm với `BookingId` và `EarnedAt = now`. Sau đó hàm cộng `FinalAmount` vào `CustomerProfile.TotalSpent` và gọi `LoyaltyWindow.ResyncProfileAsync` để tính lại `Points` và `TierId` của hồ sơ từ sổ; vì dòng mới chưa được lưu nên hàm cộng thêm `earned` vào tổng các dòng đã lưu. Khi hạng thay đổi, hàm ghi nhật ký `TierChanged`.

**Thu hồi khi hoàn tiền.** Hàm `ReverseLoyaltyAsync(userId, bookingId, …)` chỉ trừ điểm vào đúng dòng sổ của booking đó. Gọi `ReversedAmount` là số tiền được truyền vào hàm (tham số `finalAmount` trong mã), điểm thu hồi thêm là `⌊ReversedAmount / PointsPerAmount⌋` và `RevokedPoints` bị chặn tối đa bằng `Points`, như công thức (4.7). Với hủy vé của khách, `CancelBookingAsync` truyền số tiền quy đổi theo tỷ lệ hoàn: `FinalAmount × RefundAmount / payment.Amount` làm tròn về số nguyên đồng (`Math.Round` với `MidpointRounding.AwayFromZero`); với hủy suất chiếu, `DeleteShowtimeAsync` truyền toàn bộ `FinalAmount` (Mục 4.5.7).

```
RevokedPoints ← min(Points, RevokedPoints + ⌊ReversedAmount / PointsPerAmount⌋)    (4.7)
```

Do thu hồi tác động lên dòng của booking, một booking đã nằm ngoài cửa sổ 12 tháng không làm thay đổi điểm hiệu lực hiện tại. Hàm cũng trừ `TotalSpent` đi đúng `ReversedAmount` (không để âm) rồi đồng bộ lại `Points` và `TierId` của hồ sơ bằng `ResyncProfileAsync`.

**Giảm giá lúc đặt vé.** Hàm `CalculateDiscountAmountAsync` không đọc `CustomerProfile.TierId` mà tính điểm hiệu lực trực tiếp từ sổ (`SumActiveAsync`) rồi tra hạng (`ResolveTierAsync`); khoản giảm là `ticketAmount × DiscountPercent / 100`, làm tròn về số nguyên đồng, và chỉ áp dụng trên tiền vé (Mục 4.5.1.3). Hàm trả về 0 nếu booking không gắn tài khoản hoặc tài khoản chưa có hồ sơ khách hàng.

**Đồng bộ điểm hết hạn.** Điểm trượt khỏi cửa sổ không kích hoạt sự kiện nào, nên hệ thống dùng công việc định kỳ. `ExpireLoyaltyPointsJob` gọi `LoyaltyExpiryService.RecalculateAllAsync` theo biểu thức cron `5 * * * *` (phút 05 của mỗi giờ), được đăng ký trong `Program.cs` bằng `RecurringJob.AddOrUpdate` với múi giờ `VietnamTime.Zone` (mặc định `Asia/Ho_Chi_Minh`). Biểu thức này là giá trị mặc định của `BackgroundJobOptions.ExpireLoyaltyPointsCron` và cũng được ghi trong `appsettings.json`. Hàm `RecalculateAllAsync` tính một lần tổng điểm hiệu lực của mọi khách từ sổ, nạp các hồ sơ có `Points > 0` hoặc có `TierId`, rồi chỉ cập nhật những hồ sơ có `Points` hoặc `TierId` lệch so với giá trị tính lại, ghi nhật ký số hồ sơ đã đổi.

![Hình 4.58. Lưu đồ ghi sổ điểm, thu hồi và đồng bộ điểm hết hạn](hinh/4.58.png)

*Hình 4.58. Lưu đồ ghi sổ điểm, thu hồi và đồng bộ điểm hết hạn*

Hình 4.58 gồm ba nhánh cùng hội tụ vào bước tính điểm hiệu lực `P(u, t)` từ các dòng sổ có `EarnedAt` trong 12 tháng. Nhánh thứ nhất bắt đầu từ "Thanh toán thành công": `ApplyLoyaltyAsync` tạo dòng sổ điểm với `earned = ⌊FinalAmount / PointsPerAmount⌋`, tính `P(u, t)`, chọn hạng có `MinPoints` lớn nhất không vượt `P`, rồi cập nhật `CustomerProfile.Points` và `TierId`. Nhánh thứ hai bắt đầu từ "Hoàn tiền thành công": `ReverseLoyaltyAsync` kèm `bookingId` tăng `RevokedPoints` theo (4.7) rồi quay lại bước tính `P(u, t)`. Nhánh thứ ba là `ExpireLoyaltyPointsJob` chạy theo cron `5 * * * *`, gọi `LoyaltyExpiryService.RecalculateAllAsync` cho từng khách và cũng quay lại bước tính `P(u, t)`.

**Độ trễ và các tính chất còn lại.** Hồ sơ khách được đồng bộ ngay khi cộng hoặc thu hồi điểm; độ trễ chỉ xuất hiện ở trường hợp điểm hết hạn do trượt khỏi cửa sổ. Khi đó, `Points` và `TierId` trên hồ sơ có thể chậm tối đa một giờ so với giá trị đúng cho đến lần chạy kế tiếp của job. Độ trễ này không ảnh hưởng đến giảm giá vì `CalculateDiscountAmountAsync` luôn đọc trực tiếp từ sổ. `TotalSpent` không theo cửa sổ trượt: nó chỉ tăng khi xác nhận và giảm khi thu hồi, và `RecalculateAllAsync` không điều chỉnh trường này. Về dữ liệu lịch sử nếu chuyển đổi từ hệ thống cũ chưa có bảng sổ điểm, việc nạp bù sổ cái được thực hiện an toàn qua câu lệnh SQL khởi tạo từ các booking đã xác nhận (`INSERT INTO "LoyaltyPointEntries" ("UserId", "BookingId", "Points", "RevokedPoints", "EarnedAt") SELECT "UserId", "BookingId", FLOOR("FinalAmount" / 10000), 0, "CreatedAt" FROM "Bookings" WHERE "Status" = 'Confirmed' AND "UserId" IS NOT NULL;`). Nếu chưa nạp sổ, phương thức `RecalculateAllAsync` sẽ tính tổng điểm trong cửa sổ 12 tháng bằng 0 cho khách đó, đặt `Points = 0` và đưa `TierId` về hạng có `MinPoints` thấp nhất (hạng "Thành viên"). Do đó quy trình khởi tạo dữ liệu trong `DbInitializer` luôn chủ động tạo sẵn các bản ghi sổ điểm tương ứng cho tài khoản kiểm thử. Các hạn chế liên quan xem Mục 7.3.15.

### 4.5.9 Đánh giá phim

Việc đánh giá do khách hàng thực hiện qua `POST /api/v1/movies/{movieId}/reviews` (chính sách `CustomerOnly`, trả 201 khi thành công) và được xử lý trong `ReviewService.CreateReviewAsync`. Trước hàm này, `CreateReviewRequestValidator` kiểm tra điểm `Rating` nằm trong khoảng 1 đến 10 và `Comment` dài tối đa 1000 ký tự.

![Hình 4.59. Lưu đồ viết đánh giá phim](hinh/4.59.png)

*Hình 4.59. Lưu đồ viết đánh giá phim*

Hàm `CreateReviewAsync` áp dụng ba điều kiện theo thứ tự. Phim phải tồn tại (`NotFoundException`, `Messages.NotFound_Movie`). Khách chưa đánh giá phim này: nếu đã có đánh giá với cùng cặp `(UserId, MovieId)` thì hàm ném `ConflictAppException` (`Messages.Conflict_ReviewAlreadyExists`); ràng buộc này còn được bảo đảm ở cơ sở dữ liệu bằng chỉ mục duy nhất `UQ_User_Movie_Review` trên `(UserId, MovieId)`, nên hai yêu cầu đồng thời cũng không tạo được hai đánh giá. Cuối cùng, khách phải từng soát vé thành công cho phim này: tồn tại một `BookingTicket` có `IsCheckedIn = true` thuộc booking của khách và thuộc một suất chiếu của phim, nếu không hàm ném `BusinessRuleException` (`Messages.Rule_ReviewRequiresCheckIn`). Điều kiện này gắn việc đánh giá với việc đã đi xem thực tế (Mục 4.5.6) chứ không chỉ với việc đã mua vé. Sau khi thỏa cả ba điều kiện, hàm tạo `Review` với `CreatedAt` theo UTC, lưu lại, rồi tính lại điểm trung bình của phim.

Điểm trung bình được cập nhật bởi `RecalculateMovieAverageRatingAsync`: tính trung bình `Rating` của mọi đánh giá thuộc phim (bằng 0 khi phim không còn đánh giá nào), làm tròn 2 chữ số thập phân và ghi vào `Movie.AverageRating` bằng một lệnh `ExecuteUpdateAsync`. Hàm này được gọi lại sau mỗi lần tạo, sửa và xóa đánh giá, nên giá trị lưu luôn nhất quán với tập đánh giá hiện có. Hàm `GetUserMovieReviewStatusAsync` (kết quả `CanReviewResponse`) dùng đúng hai điều kiện "chưa đánh giá" và "đã soát vé" để giao diện quyết định có hiển thị biểu mẫu đánh giá hay không.

Khách sửa hoặc xóa đánh giá của chính mình qua `UpdateReviewAsync` và `DeleteMyReviewAsync`; người không phải chủ đánh giá bị từ chối bằng `ForbiddenAppException` (`Messages.Forbidden_NotReviewOwner`). Về kiểm duyệt, hệ thống áp dụng hình thức hậu kiểm: đánh giá hiển thị ngay sau khi đăng, còn quản trị viên (chính sách `AdminOnly`) xem danh sách có lọc theo phim và điểm (`GetReviewsAdminAsync`) và xóa từng đánh giá (`DeleteReviewAsync`) hoặc xóa hàng loạt (`BulkDeleteReviewsAsync`, tối đa `BookingOptions.BulkDeleteLimit` = 50 mã mỗi yêu cầu). Mỗi lần xóa đều tính lại điểm trung bình của phim bị ảnh hưởng; trong xóa hàng loạt, mỗi đánh giá được lưu riêng, kết quả báo theo từng mã và điểm trung bình được tính lại một lần cho mỗi phim sau vòng xử lý.

<!-- GHI CHÚ ĐỐI CHIẾU -->
<!--
(a) [CẦN BỔ SUNG] còn lại
1. Mã Mermaid Hình 4.55, 4.56, 4.57, 4.59 (`ma-nguon-so-do.md`): Văn bản mô tả các hình này được viết bám sát 100% mã nguồn thực tế; cần chèn file ảnh kết xuất vào thư mục `hinh/` khi hoàn thiện bản in Word.
2. Nạp bù sổ điểm cho dữ liệu cũ: Đã làm rõ câu lệnh SQL nạp bù từ các booking `Confirmed` và cơ chế nạp sẵn trong `DbInitializer.SeedTestAccountsAsync`.
3. Không có trích dẫn tài liệu tham khảo nào trong phạm vi này.

(b) Chỗ outline/mã sơ đồ lệch code
1. 4.5.8, sổ điểm: outline ghi "mỗi booking đã xác nhận sinh một dòng"; code (`ApplyLoyaltyAsync`) chỉ thêm dòng khi `earned > 0` (booking có `FinalAmount` dưới `PointsPerAmount` không có dòng nào).
2. 4.5.8, công thức (4.7): outline và Hình 4.58 (nút H) dùng `RefundedAmount`; code dùng tham số `finalAmount` của `ReverseLoyaltyAsync`. Với hủy vé khách (`CancelBookingAsync`) đó là `Round(FinalAmount × RefundAmount / payment.Amount)`; với hủy suất (`ShowtimeService.DeleteShowtimeAsync`) là toàn bộ `FinalAmount`. Bản viết dùng ký hiệu `ReversedAmount`; cần quyết định có sửa nhãn nút H của Hình 4.58 cho khớp hay không.
3. 4.5.8, `TotalSpent`: outline chỉ ghi "tổng chi tiêu trọn đời"; code còn trừ `TotalSpent` (không âm) khi thu hồi điểm trong `ReverseLoyaltyAsync`. Hình 4.58 không thể hiện việc cộng/trừ `TotalSpent` và điều kiện `earned > 0`.
4. 4.5.8, `BookingId = null` "điểm điều chỉnh": không có đường xử lý nào trong code tạo dòng như vậy; chỉ có dữ liệu mẫu từ `DbInitializer` (khách thử nghiệm có `Points > 0`). Bản viết chỉ nêu trường hợp dữ liệu mẫu.
5. 4.5.8, cron `5 * * * *`, múi giờ VN, độ trễ tối đa 1 giờ, giảm giá đọc từ sổ, combo vẫn tích điểm: khớp code (`BackgroundJobOptions`, `appsettings.json`, `Program.cs`, `CalculateDiscountAmountAsync`).
6. 4.5.9, "kiểm duyệt của quản trị": code không có trạng thái chờ duyệt/ẩn; chỉ có xóa sau đăng (đơn lẻ và hàng loạt). Bản viết mô tả là hậu kiểm.
7. 4.5.9, thang điểm: `CreateReviewRequestValidator` cho phép `Rating` 1–10 nhưng `ReviewConfiguration` có ràng buộc `CK_Reviews_Rating` chỉ cho 1–5 (`"Rating" BETWEEN 1 AND 5`). Giá trị 6–10 qua được validator nhưng sẽ vi phạm ràng buộc CSDL khi lưu. Đề nghị xem xét đưa vào Mục 7.3 (hạn chế) hoặc thống nhất thang điểm; bản viết nêu đúng cả hai nơi, không chọn thay.
8. 4.5.7: dòng `[CẦN BỔ SUNG: đọc DeleteShowtimeAsync…]` của outline đã được giải quyết bằng mã. Quy tắc "không đổi phòng, không sửa khi có booking" nằm ở `UpdateShowtimeAsync`, không phải `DeleteShowtimeAsync` (outline chỉ dẫn `DeleteShowtimeAsync` và `ShowtimeRefundJob.cs` ở dòng nguồn). Khi hủy suất, thu hồi điểm được thực hiện trong giao dịch, trước và độc lập với kết quả hoàn tiền của `ShowtimeRefundJob`.
9. 4.5.6: hạn soát vé là `Showtime.EndTime` (đã gồm `RoomBufferMinutes`), không phải hết thời lượng phim; bản viết nêu rõ.
-->

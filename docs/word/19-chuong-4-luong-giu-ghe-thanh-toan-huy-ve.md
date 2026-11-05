## 4.5 LUỒNG NGHIỆP VỤ

Mục này trình bày các luồng nghiệp vụ cốt lõi của quy trình đặt vé trực tuyến theo đúng thứ tự thực thi trong mã nguồn: giữ ghế (Mục 4.5.1), thanh toán trực tuyến (Mục 4.5.2), giải phóng ghế khi hết hạn giữ (Mục 4.5.3) và hủy vé (Mục 4.5.4). Phần lớn logic nằm trong `BookingService` (`Cinema.Application/Services/Bookings/BookingService.cs`) và `PaymentService` (`Cinema.Application/Services/Payments/PaymentService.cs`); ba cổng thanh toán được hiện thực ở `Cinema.Infrastructure/Payments`. Trạng thái của đơn (`BookingStatus`: `Pending`, `Confirmed`, `Cancelled`, `Expired`) và của giao dịch (`PaymentStatus`: `Pending`, `Success`, `Failed`, `Refunded`) là các giá trị xuyên suốt các luồng dưới đây.

### 4.5.1 Giữ ghế

Giữ ghế là bước đầu tiên của quy trình đặt vé trực tuyến, ứng với use case UC-DV-04 (xem Mục 3.3.5). Khách hàng đã đăng nhập gửi `POST api/v1/bookings/hold` (`BookingsController.HoldSeats`, yêu cầu `[Authorize]` và chịu chính sách giới hạn tần suất `booking-hold`, mặc định 10 yêu cầu mỗi 60 giây theo `RateLimitOptions.BookingHoldPermitLimit` và `BookingHoldWindowSeconds`). Nội dung yêu cầu gồm mã suất chiếu, danh sách mã ghế theo suất (`ShowtimeSeatId`), tên và số điện thoại khách, và danh sách combo tùy chọn. `BookingService.HoldSeatsAsync` kiểm tra luật chọn ghế rồi giao cho `CreateHoldBookingAsync` thực hiện phần còn lại; kết quả là một đơn `Booking` ở trạng thái `Pending`, có hạn giữ ghế `ExpiresAt`, và phản hồi mã trạng thái 201.

#### 4.5.1.1 Kiểm tra đầu vào

Việc kiểm tra diễn ra ở nhiều tầng. Trước khi vào action, `ValidationActionFilter` chạy `HoldSeatsRequestValidator` (FluentValidation) để kiểm tra dạng dữ liệu. Sau đó, trong service, `HoldSeatsAsync` tải sơ đồ ghế của suất chiếu (`LoadSeatSlotsAsync`) và chế độ luật ghế lẻ (`SeatGapRuleMode` trong cài đặt rạp) rồi gọi `SeatLayoutRules.ValidateSelection`; khi hàm này trả về chuỗi lỗi, `BusinessRuleException` được ném. Cuối cùng, `CreateHoldBookingAsync` kiểm tra trạng thái và thời điểm của suất chiếu, tên khách và đơn `Pending` sẵn có trước khi chạm đến dữ liệu ghế. Bảng 4.81 liệt kê các kiểm tra theo thứ tự thực thi trong mã.

Luật chọn ghế phía máy chủ gồm hai phần. Thứ nhất, các ghế thuộc cùng một cặp ghế đôi (cùng `CoupleId`) phải được chọn đồng thời; nếu chỉ chọn một ghế của cặp, hàm trả về thông báo "Ghế đôi phải được chọn cùng nhau.". Thứ hai, tùy `SeatGapRuleMode`, hệ thống không cho phép việc chọn ghế để lại đúng một ghế trống đơn lẻ trong cùng hàng (giữa các ghế đã chọn, đã giữ hoặc đã đặt). Ở chế độ `Strict` (giá trị mặc định của `CinemaSettings.SeatGapRuleMode`), mọi ghế lẻ phát sinh đều bị chặn, kể cả ghế sát mép hàng; ở chế độ `AllowEdge`, ghế lẻ sát mép hàng được chấp nhận; ở chế độ `Off`, luật ghế lẻ không được áp dụng (luật ghế đôi vẫn áp dụng). Khi vi phạm, thông báo có dạng "Không thể để trống ghế lẻ {hàng}{số}. Vui lòng chọn lại để không bỏ lẻ ghế.". Chỉ các ghế lẻ nằm cạnh ghế vừa chọn mới bị xét; những ghế lẻ vốn đã tồn tại từ trước trên sơ đồ không bị coi là vi phạm.

**Bảng 4.81. Các kiểm tra khi giữ ghế**

| Thứ tự | Kiểm tra | Nơi kiểm tra | Lỗi trả về |
|---|---|---|---|
| 1 | Dữ liệu đầu vào hợp lệ: `ShowtimeId` > 0; `CustomerName` không rỗng, tối đa 100 ký tự; `CustomerPhone` (nếu có) khớp `^(0\|\+84)[0-9]{9,10}$`, tối đa 20 ký tự; `SeatIds` không rỗng, mỗi giá trị > 0, không trùng, tối đa `MaxSeatsPerBooking` (mặc định 10); mỗi combo có `ComboId` > 0 và `Quantity` từ 1 đến `MaxComboQuantity` (mặc định 10) | `HoldSeatsRequestValidator`, `ComboSelectionRequestValidator` | Lỗi xác thực, ví dụ `Validation_Seats_AtLeastOne` ("Phải chọn ít nhất 1 ghế."), `Validation_Seats_Duplicated`, `Validation_Seats_MaxPerBooking` ("Mỗi booking tối đa {0} ghế."), `Validation_Phone_Invalid` |
| 2 | Ghế đôi được chọn cùng nhau | `SeatLayoutRules.ValidateSelection` | `BusinessRuleException` (422): "Ghế đôi phải được chọn cùng nhau." |
| 3 | Luật ghế lẻ theo `SeatGapRuleMode` (`Strict`, `AllowEdge`, `Off`) | `SeatLayoutRules.ValidateSelection` | `BusinessRuleException` (422): "Không thể để trống ghế lẻ {hàng}{số}. Vui lòng chọn lại để không bỏ lẻ ghế." |
| 4 | Suất chiếu tồn tại | `CreateHoldBookingAsync` | `NotFoundException` (404): `NotFound_Showtime` ("Không tìm thấy suất chiếu.") |
| 5 | Suất chiếu ở trạng thái `Scheduled` | `CreateHoldBookingAsync` | `BusinessRuleException` (422): `Rule_ShowtimeUnavailableForBooking` ("Suất chiếu không còn khả dụng để đặt vé.") |
| 6 | Suất chiếu chưa bắt đầu (`StartTime` > thời điểm hiện tại) | `CreateHoldBookingAsync` | `BusinessRuleException` (422): `Rule_ShowtimeStartedOrEnded` ("Suất chiếu đã bắt đầu hoặc đã kết thúc.") |
| 7 | Chưa quá thời điểm dừng bán online (chỉ kênh `Online`): đóng bán khi `StopSellingMinutesBefore` > 0 và thời điểm hiện tại ≥ `StartTime` trừ số phút đó | `ShowtimeLifecycle.IsOnlineSalesClosed` | `BusinessRuleException` (422): `Rule_OnlineSalesClosed` ("Đã ngừng bán vé online cho suất chiếu này (trước giờ chiếu {0} phút).") |
| 8 | Tên khách không rỗng | `CreateHoldBookingAsync` | `ValidationAppException` (400): `Invalid_CustomerNameMissing` ("Thiếu tên khách hàng."), lỗi theo trường `customerName` |
| 9 | Chưa có booking `Pending` còn hạn của cùng người giữ cho suất chiếu này (kênh `Online`: cùng `UserId`; kênh `Counter`: cùng `CreatedByEmployeeId`) | `CreateHoldBookingAsync` | `ConflictAppException` (409): `Conflict_PendingBookingExistsForShowtime` ("Đang có một booking Pending khác chưa hoàn tất cho suất chiếu này.") |
| 10 | Mọi ghế yêu cầu còn giữ được (câu lệnh cập nhật có điều kiện, Mục 4.5.1.2) | `CreateHoldBookingAsync` | `SeatConflictException` (409): `Conflict_SeatsAlreadyTaken` ("Một hoặc nhiều ghế đã bị giữ/đặt bởi người khác."), kèm danh sách `ConflictSeatIds` |
| 11 | Mọi combo được chọn tồn tại và đang bán (`IsActive`) | `BuildBookingCombosAsync` | `ValidationAppException` (400): `Invalid_ComboIdsUnavailable` ("Một hoặc nhiều comboId không tồn tại hoặc đã ngừng bán.") |
| 12 | Sinh được mã booking duy nhất sau tối đa `BookingCodeMaxAttempts` (mặc định 5) lần thử | `GenerateUniqueBookingCodeAsync` | `BusinessRuleException` (422): `Rule_BookingCodeGenerationFailed` ("Không thể sinh mã booking duy nhất, vui lòng thử lại.") |

*Ghi chú: các kiểm tra 11 và 12 diễn ra sau câu lệnh giữ ghế nhưng trước khi giao dịch cơ sở dữ liệu được xác nhận (`CommitAsync`); vì giao dịch được khai báo bằng `await using`, ngoại lệ ném ra ở các bước này khiến giao dịch bị hủy khi giải phóng. Các thông báo ở dòng 2 và 3 là chuỗi cố định trong `SeatLayoutRules.cs`, không lấy từ tài nguyên `Messages`.*

#### 4.5.1.2 Giữ ghế nguyên tử

Điểm quan trọng nhất của luồng giữ ghế là bảo đảm hai khách không thể cùng giữ một ghế. `CreateHoldBookingAsync` mở một giao dịch cơ sở dữ liệu (`BeginTransactionAsync`), tính thời điểm hết hạn giữ `holdUntil` bằng thời điểm hiện tại cộng `HoldDurationMinutes` của cài đặt rạp (mặc định 15 phút, `CinemaSettings.DefaultHoldDurationMinutes`), và nếu mốc này vượt quá giờ bắt đầu suất chiếu thì lấy giờ bắt đầu làm hạn. Sau đó hệ thống thực hiện một câu lệnh `ExecuteUpdateAsync` duy nhất trên bảng `ShowtimeSeats`. Câu lệnh chọn các dòng có `ShowtimeSeatId` thuộc danh sách ghế (đã loại trùng) và `ShowtimeId` khớp suất chiếu, đồng thời thỏa một trong hai điều kiện: ghế đang `Available`, hoặc ghế đang `Held` bởi chính người giữ hiện tại (`HeldByUserId` trùng) với `HeldUntil` đã nhỏ hơn thời điểm hiện tại, tức là lượt giữ cũ của chính khách đã hết hạn. Các dòng thỏa điều kiện được đặt `Status = Held`, `HeldByUserId` bằng người giữ, `HeldByBookingId = null` (chưa có đơn) và `HeldUntil = holdUntil`.

Giá trị trả về của câu lệnh là số dòng thực sự được cập nhật, và đây là căn cứ phát hiện xung đột: nếu số dòng khác số ghế yêu cầu thì ít nhất một ghế đã không còn thỏa điều kiện (đã bị người khác giữ hoặc đặt). Vì điều kiện và việc cập nhật nằm trong cùng một câu lệnh, hai yêu cầu đồng thời cho cùng một ghế không thể cùng thành công: yêu cầu đến sau thấy điều kiện không còn đúng nên không cập nhật dòng đó, và số dòng bị lệch kéo theo thất bại của cả yêu cầu. Khi lệch số dòng, hệ thống truy vấn lại các ghế mà người giữ hiện tại đang giữ với đúng `holdUntil` vừa đặt, lấy phần còn lại làm danh sách ghế xung đột, hủy giao dịch (`RollbackAsync`, trả các ghế đã cập nhật về trạng thái cũ) và ném `SeatConflictException` kèm `ConflictSeatIds` để giao diện đánh dấu các ghế đó. Việc "tất cả hoặc không ghế nào" là có chủ đích: một đơn không bao giờ giữ một phần số ghế khách chọn.

Ở tầng dữ liệu còn hai chỉ mục duy nhất làm lớp bảo vệ độc lập với đoạn mã trên. Chỉ mục `UQ_Showtime_Seat` trên `(ShowtimeId, SeatId)` của `ShowtimeSeats` bảo đảm mỗi ghế vật lý chỉ có một dòng trạng thái trong mỗi suất chiếu, nên câu lệnh cập nhật luôn nhắm vào một dòng duy nhất cho mỗi ghế. Chỉ mục duy nhất trên `BookingTicket.ShowtimeSeatId` bảo đảm một dòng ghế theo suất chỉ gắn với tối đa một vé, nên một ghế không thể được phát hành vé cho hai đơn khác nhau. Đoạn mã rút gọn của câu lệnh cập nhật có điều kiện được đặt ở Phụ lục 5.

![Hình 4.47. Lưu đồ giữ ghế nguyên tử](hinh/4.47.png)

*Hình 4.47. Lưu đồ giữ ghế nguyên tử*

Hình 4.47 thể hiện luồng trên theo thứ tự: mở giao dịch; tính `holdUntil`; cập nhật có điều kiện các dòng `ShowtimeSeat`; so sánh số dòng bị ảnh hưởng với số ghế yêu cầu. Nhánh lệch số dòng truy vấn danh sách ghế xung đột, hủy giao dịch và ném `SeatConflictException`. Nhánh khớp số dòng tiếp tục tính tiền và tạo đơn (Mục 4.5.1.3), gán `HeldByBookingId` cho các ghế rồi xác nhận giao dịch.

#### 4.5.1.3 Tính tiền

Khi các ghế đã được giữ, `CreateHoldBookingAsync` tải lại các dòng `ShowtimeSeat` cùng thông tin ghế để tính tiền. Tiền vé `TicketAmount` là tổng giá của các ghế đã giữ theo suất chiếu, tức tổng `ShowtimeSeat.Price` (biến `seatTotal` trong mã). Tiền combo `ComboAmount` do `BuildBookingCombosAsync` tính bằng tổng của đơn giá hiện tại của combo nhân số lượng, sau khi kiểm tra mọi combo được chọn đều tồn tại và còn bán; đơn giá được sao chép vào `BookingCombo.UnitPrice` để thay đổi giá về sau không làm lệch đơn đã tạo. Tổng tiền của đơn là tiền vé cộng tiền combo:

$$\text{TotalAmount} = \text{TicketAmount} + \text{ComboAmount} \qquad (4.1)$$

Giảm giá theo hạng thành viên chỉ áp dụng trên tiền vé, không áp dụng cho combo (mã nguồn ghi rõ điều này bằng chú thích tại nơi gọi). `CalculateDiscountAmountAsync` nhận `ticketAmount` làm đầu vào. Với đơn không gắn người dùng (`bookingUserId` rỗng) hoặc người dùng không có hồ sơ khách hàng, mức giảm là 0. Với khách hàng có hồ sơ, hệ thống không đọc `CustomerProfile.TierId` (chỉ là bản lưu đệm) mà tính hạng trực tiếp từ sổ điểm: `LoyaltyWindow.SumActiveAsync` cộng giá trị `Points − RevokedPoints` của các dòng sổ điểm có `EarnedAt` lớn hơn mốc cắt (thời điểm hiện tại lùi 12 tháng, `LoyaltyWindow.Months`), cho ra điểm hiệu lực trong cửa sổ trượt 12 tháng; sau đó `LoyaltyWindow.ResolveTierAsync` chọn hạng `MembershipTier` có `MinPoints` lớn nhất không vượt quá điểm hiệu lực. Nhờ vậy điểm vừa trượt khỏi cửa sổ có hiệu lực ngay, không phải chờ công việc nền hằng giờ đồng bộ lại hồ sơ. Nếu không có hạng nào phù hợp thì mức giảm là 0; nếu có, mức giảm được làm tròn về số nguyên theo quy tắc làm tròn ra xa 0 (`MidpointRounding.AwayFromZero`):

$$\text{DiscountAmount} = \operatorname{round}\!\left(\frac{\text{TicketAmount} \times \text{DiscountPercent}}{100}\right) \qquad (4.2)$$

trong đó `DiscountPercent` là `MembershipTier.DiscountPercent` của hạng xác định ở trên và hàm làm tròn lấy 0 chữ số thập phân, làm tròn ra xa 0. Số tiền cuối cùng khách phải trả là:

$$\text{FinalAmount} = \text{TotalAmount} - \text{DiscountAmount} \qquad (4.3)$$

Ba giá trị `TotalAmount`, `DiscountAmount`, `FinalAmount` được lưu vào đơn; `FinalAmount` cũng là số tiền gửi sang cổng thanh toán (Mục 4.5.2.1) và là cơ sở tính điểm tích lũy (xem Mục 4.5.8). Mã booking do `GenerateUniqueBookingCodeAsync` sinh qua `CodeGenerator.GenerateBookingCode` với dạng tiền tố, ngày và 4 ký tự: tiền tố `BrandOptions.BookingCodePrefix` (mặc định "BK", viết hoa), ngày hiện tại theo múi giờ nghiệp vụ định dạng `yyyyMMdd`, và bốn ký tự ngẫu nhiên lấy từ bảng chữ cái hoa A–Z và chữ số 0–9. Hệ thống kiểm tra mã chưa tồn tại trong bảng `Bookings` (chỉ mục duy nhất trên `BookingCode`) và thử lại tối đa `BookingCodeMaxAttempts` lần.

#### 4.5.1.4 Tuần tự giữ ghế

![Hình 4.48. Tuần tự giữ ghế](hinh/4.48.png)

*Hình 4.48. Tuần tự giữ ghế*

Hình 4.48 mô tả trình tự từ thao tác của khách đến phản hồi. Khách chọn ghế và combo trên giao diện, giao diện gửi `POST api/v1/bookings/hold` kèm access token. Tại `BookingsController.HoldSeats`, yêu cầu đi qua bộ giới hạn tần suất `booking-hold` và bộ kiểm tra dữ liệu, rồi gọi `BookingService.HoldSeatsAsync(CurrentUserId, request, ...)`. Service tải sơ đồ ghế và chế độ luật ghế lẻ từ cơ sở dữ liệu, kiểm tra luật chọn ghế, sau đó `CreateHoldBookingAsync` đọc suất chiếu và cài đặt rạp (`StopSellingMinutesBefore`, `HoldDurationMinutes`), kiểm tra đơn `Pending` sẵn có, rồi thực hiện giữ ghế nguyên tử và tạo đơn trong một giao dịch: thêm `Booking` (trạng thái `Pending`, `ExpiresAt = holdUntil`, kèm các `BookingCombo`), gán `HeldByBookingId` cho các ghế và xác nhận giao dịch. Sau khi xác nhận, service gọi `IRealtimeNotifier.NotifySeatHeldAsync(showtimeId, seatIds, holderUserId, holdUntil)` để máy chủ phát sự kiện thời gian thực qua hub `/hubs/showtimes` cho các khách đang xem cùng suất chiếu, ghi log `SeatsHeld` và trả `HoldBookingResponse` với mã 201. Khi có lỗi ở bất kỳ bước kiểm tra nào, phản hồi là lỗi tương ứng ở Bảng 4.81 và không có sự kiện nào được phát.

### 4.5.2 Thanh toán trực tuyến

Thanh toán trực tuyến nối tiếp bước giữ ghế và ứng với các use case UC-TT-01 đến UC-TT-06 (xem Mục 3.3.6). Luồng gồm bốn pha: khách tạo giao dịch và được chuyển sang cổng (Mục 4.5.2.1); cổng gọi ngược về hệ thống bằng IPN hoặc webhook (Mục 4.5.2.2); khi thanh toán thành công, đơn được xác nhận và vé được phát hành (Mục 4.5.2.3); nếu ghế đã hết hạn giữ trước khi tiền về, hệ thống hoàn tiền tự động (Mục 4.5.2.4). Ba cổng được so sánh ở Mục 4.5.2.5. Hệ thống đăng ký ba hiện thực `IPaymentGateway` trong `Cinema.Infrastructure/DependencyInjection.cs`: `VnPayGateway`, `MomoGateway` và `StripeGateway`.

#### 4.5.2.1 Tạo giao dịch thanh toán

Khách chọn phương thức và gửi `POST api/v1/bookings/{bookingId}/checkout`. `CheckoutRequestValidator` chỉ chấp nhận ba giá trị `PaymentMethod` là "VNPay", "Momo" và "Stripe"; tiền mặt chỉ dùng ở quầy. `BookingService.CheckoutAsync` thực hiện các bước sau theo đúng thứ tự trong mã. Hệ thống tìm đơn theo mã (không thấy thì `NotFoundException` `NotFound_Booking`), kiểm tra `booking.UserId` trùng người đang đăng nhập (nếu không thì `ForbiddenAppException` `Forbidden_NotBookingOwner`: "Bạn không phải chủ booking này.") và kiểm tra đơn còn ở trạng thái `Pending` và `ExpiresAt` chưa qua (nếu không thì `BusinessRuleException` `Rule_BookingNotPendingOrExpired`: "Booking không ở trạng thái Pending hoặc đã hết hạn."). Tiếp theo, `PaymentGatewayExtensions.FindByMethod` chọn cổng có `MethodName` trùng chính xác tên phương thức; nếu không có, ném `Rule_PaymentGatewayNotConfigured` ("Payment gateway '{0}' chưa được cấu hình."). Các giao dịch `Pending` trước đó của cùng đơn được đặt thành `Failed`, để mỗi đơn chỉ có một giao dịch đang chờ.

Mã giao dịch phía hệ thống (`TransactionId`) do `CodeGenerator.GeneratePaymentTransactionId` sinh bằng cách nối mã booking với dấu thời gian Unix (giây). Hệ thống gọi `gateway.CreatePaymentAsync` với `CreatePaymentRequest(BookingId, FinalAmount, "Thanh toán booking {BookingCode}", TransactionId)` để nhận đường dẫn thanh toán. Chỉ sau khi cổng trả kết quả, hệ thống mới ghi bản ghi `Payment` với `PaymentMethod` (chuyển từ chuỗi sang enum `PaymentMethod`), `Status = Pending`, `Amount = FinalAmount`, `TransactionId` và `CreatedAt`, rồi trả `CheckoutResponse(PaymentUrl, PaymentId)`. Giao diện chuyển hướng khách đến `PaymentUrl`. Số tiền gửi cổng luôn là `FinalAmount` của đơn, đã gồm giảm giá theo công thức (4.3).

![Hình 4.49. Tuần tự tạo giao dịch thanh toán](hinh/4.49.png)

*Hình 4.49. Tuần tự tạo giao dịch thanh toán*

#### 4.5.2.2 Xử lý IPN

Sau khi khách thanh toán, cổng gọi ngược về ba endpoint công khai của `PaymentsController` (không có `[Authorize]`, đáp ứng dạng thô nhờ `[RawResponse]`): `GET` hoặc `POST api/v1/payments/vnpay/ipn` (tham số lấy từ chuỗi truy vấn và biểu mẫu), `POST api/v1/payments/momo/ipn` (thân JSON được quy về các cặp tên–giá trị chuỗi) và `POST api/v1/payments/stripe/webhook` (thân thô và tiêu đề `Stripe-Signature`). Ba phương thức `ProcessVnPayIpnAsync`, `ProcessMomoIpnAsync`, `ProcessStripeWebhookAsync` của `PaymentService` có cấu trúc chung như sau.

Thứ nhất, hệ thống tìm cổng tương ứng; nếu cổng chưa đăng ký thì trả mã "99" (VNPay, MoMo) hoặc thông điệp "Gateway not configured" (Stripe). Thứ hai, cổng xác minh chữ ký (`VerifyCallback` cho VNPay và MoMo, `VerifyWebhook` cho Stripe); chữ ký sai trả mã "97" với "Invalid signature" (Stripe: `Success = false`; riêng trường hợp này `PaymentsController` đáp 400, còn mọi phản hồi khác của Stripe là 200). Thứ ba, hệ thống tìm `Payment` có `TransactionId` trùng mã giao dịch do cổng gửi lại (`vnp_TxnRef`, `orderId`, hoặc `ClientReferenceId` của phiên Stripe); không thấy thì trả mã "01" (VNPay), "1" (MoMo) hoặc "Payment not found" (Stripe). Riêng Stripe, sự kiện không có mã giao dịch (mọi loại sự kiện khác `checkout.session.completed`) được bỏ qua với "Event ignored" ngay trước bước tìm giao dịch.

Thứ tư là kiểm tra lặp (idempotent) bằng `IsAlreadyProcessed(payment, callbackSuccess)`: callback được coi là đã xử lý khi giao dịch đã `Success` hoặc `Refunded`, hoặc khi giao dịch đã `Failed` và callback lần này cũng báo thất bại. Khi đã xử lý, hệ thống trả mã thành công mà không làm gì thêm ("00" cho VNPay, "0" cho MoMo, "Payment already processed" cho Stripe), nên cổng có thể gửi lại cùng một IPN nhiều lần mà không phát hành vé hai lần. Điều kiện này cũng cho phép một giao dịch đã `Failed` được xử lý nếu về sau cổng báo thành công. Thứ năm là kiểm tra số tiền: nếu `|payment.Amount − callback.Amount| > 0,01` thì trả "04" (VNPay), "4" (MoMo) hoặc "Invalid amount" (Stripe) và không áp dụng kết quả. Riêng Stripe, nếu callback có mã tiền tệ khác `StripeOptions.Currency` (mặc định "vnd", so sánh không phân biệt hoa thường) thì trả "Invalid currency"; VNPay và MoMo không có bước kiểm tra tiền tệ này.

Sau cùng, `ApplyPaymentResultAsync` áp dụng kết quả trong một giao dịch cơ sở dữ liệu. Hệ thống ghi `GatewayResponseCode` và đặt `Status` thành `Success` hoặc `Failed`; với thanh toán thành công còn ghi `PaidAt` và `GatewayTransactionId` (mã giao dịch phía cổng, dùng cho hoàn tiền sau này). Nếu thất bại, giao dịch được xác nhận rồi gọi `ProcessPaymentFailureAsync`: nếu đơn không còn `Pending` thì bỏ qua; nếu đơn còn hạn giữ thì giữ nguyên ghế để khách có thể thanh toán lại; chỉ khi đơn đã quá hạn thì ghế được trả và đơn chuyển `Cancelled`. Nếu thành công, hệ thống gọi `ConfirmBookingAndGenerateTicketsAsync` (Mục 4.5.2.3) trong cùng giao dịch rồi xác nhận giao dịch; nếu ném `SeatsNoLongerHeldException`, hệ thống xác nhận giao dịch `Success` của khoản thanh toán rồi chuyển sang `HandleSeatsExpiredAfterPaymentAsync` (Mục 4.5.2.4). Cuối cùng phản hồi gửi lại cổng theo định dạng riêng của từng cổng ("00"/"Confirm Success", `resultCode` 0/"success", hoặc `message` của Stripe).

![Hình 4.50. Lưu đồ xử lý IPN](hinh/4.50.png)

*Hình 4.50. Lưu đồ xử lý IPN*

#### 4.5.2.3 Phát hành vé sau thanh toán

`BookingService.ConfirmBookingAndGenerateTicketsAsync` chuyển đơn từ `Pending` sang `Confirmed` và phát hành vé. Hệ thống tải đơn; nếu đơn đã `Confirmed` thì ghi log `PaymentAlreadyProcessed` và thoát (lớp bảo vệ lặp thứ hai bên cạnh `IsAlreadyProcessed`); nếu đơn đã `Cancelled` hoặc `Expired` thì ném `SeatsNoLongerHeldException`. Hệ thống xác định người giữ ghế (`BookingSeatHold.ResolveHolderUserIdAsync`) và tải các ghế còn ở trạng thái `Held` của đơn (`HeldFor`: ghế có `HeldByBookingId` bằng mã đơn, hoặc chưa gán đơn và do cùng người giữ). Nếu không còn ghế nào được giữ, đơn được đặt `Cancelled`, lưu lại và ném `SeatsNoLongerHeldException`.

Nếu còn ghế, thao tác chạy trong giao dịch hiện có của `ApplyPaymentResultAsync` (hoặc tự mở giao dịch nếu chưa có). Hệ thống đặt `booking.Status = Confirmed`, xóa các vé cũ trỏ vào các ghế này nhưng thuộc đơn chưa xác nhận (tránh vi phạm chỉ mục duy nhất trên `ShowtimeSeatId`), rồi với mỗi ghế: chuyển `ShowtimeSeat` sang `Booked` và xóa các trường giữ chỗ (`HeldByUserId`, `HeldByBookingId`, `HeldUntil`), đồng thời tạo `BookingTicket` với `Price` bằng giá ghế theo suất, `IsCheckedIn = false` và `QrCode` do `CodeGenerator.GenerateTicketQrPayload` sinh theo dạng mã booking, hàng, số ghế (`{BookingCode}-{Hàng}{Số}`). Chuỗi này là nội dung lưu trong cơ sở dữ liệu; ảnh mã QR (PNG, mức sửa lỗi Q) được `QrCodeHelper.GenerateDataUri` dựng thành data URI khi vé được trả về cho giao diện (cấu hình ánh xạ `BookingMappingConfig`).

Tiếp theo là bước điểm thành viên, chỉ thực hiện khi đơn có `UserId` và người dùng có hồ sơ khách hàng, được cập nhật theo hướng ghi sổ điểm. Sau bước tạo vé và mã QR, `BookingService` ghi một dòng sổ điểm `LoyaltyPointEntry(BookingId, Points, EarnedAt)` với `Points = ⌊FinalAmount / PointsPerAmount⌋` (`LoyaltyWindow.PointsFor`; `PointsPerAmount` mặc định 10000) và `EarnedAt` là thời điểm hiện tại (UTC); dòng này chỉ được thêm khi số điểm lớn hơn 0 và `BookingId` có chỉ mục duy nhất trong sổ điểm (`UX_LoyaltyPointEntries_Booking`) nên mỗi đơn chỉ có tối đa một dòng. Sau đó `BookingService` tính lại `Points` và `TierId` của hồ sơ từ sổ điểm (`LoyaltyWindow.ResyncProfileAsync`): điểm hiệu lực trong cửa sổ trượt 12 tháng gồm tổng các dòng đã lưu cộng số điểm vừa ghi (dòng mới chưa lưu nên được cộng thêm bằng tham số), và `TierId` là hạng có `MinPoints` lớn nhất không vượt quá điểm đó; ngoài ra `TotalSpent` được cộng thêm `FinalAmount`. Nếu hạng thay đổi, log `TierChanged` được ghi. Cả hai thao tác ghi dòng sổ điểm và cập nhật hồ sơ được lưu cùng với các vé trong một lần `SaveChangesAsync`, rồi giao dịch được xác nhận.

Sau khi lưu, hệ thống phát sự kiện `NotifySeatBookedAsync(showtimeId, seatIds)` qua kênh thời gian thực. Nếu đơn gắn với người dùng, hệ thống dựng thư `EmailTemplates.BookingConfirmed` (mã booking, tên phim, số ghế) và xếp job `SendEmailJob` vào hàng đợi Hangfire với tiêu đề "Vé xem phim của bạn — {tên thương hiệu}", nên việc gửi thư không chặn xử lý IPN.

![Hình 4.51. Tuần tự phát hành vé sau thanh toán](hinh/4.51.png)

*Hình 4.51. Tuần tự phát hành vé sau thanh toán*

Hình 4.51 (phiên bản đã cập nhật) thể hiện trình tự giữa cổng thanh toán, `PaymentsController`, `PaymentService`, `BookingService`, cơ sở dữ liệu, kênh thời gian thực và hàng đợi thư. Sau khi `PaymentService` ghi kết quả thanh toán, `BookingService` xác nhận đơn, chuyển ghế sang `Booked`, tạo vé và mã QR. Tiếp đó có hai thông điệp mới từ `BookingService` đến cơ sở dữ liệu: ghi `LoyaltyPointEntry(BookingId, Points, EarnedAt)`, rồi tính lại `Points` và `TierId` từ sổ điểm. Sau khi lưu và xác nhận giao dịch, `BookingService` phát sự kiện ghế đã đặt và xếp thư xác nhận vé.

#### 4.5.2.4 Ghế hết hạn sau khi khách đã trả tiền

Tình huống này xảy ra khi tiền đã về nhưng ghế không còn được giữ (ví dụ công việc nền đã giải phóng ghế trước khi IPN đến, Mục 4.5.3). `ApplyPaymentResultAsync` bắt `SeatsNoLongerHeldException`, xác nhận giao dịch (khoản thanh toán vẫn ghi nhận `Success`) rồi gọi `HandleSeatsExpiredAfterPaymentAsync`. Phương thức này ghi log mức nghiêm trọng `SeatsExpiredAfterPayment` và cố hoàn tiền tự động toàn bộ `payment.Amount` qua cổng đã dùng (`RefundAsync` với nội dung "Hoàn tiền tự động - ghế hết hạn giữ - Payment {PaymentId}"); lỗi ngoại lệ khi gọi cổng được bắt và ghi log `AutoRefundFailed`, không làm hỏng luồng. Nếu hoàn tiền thành công, hệ thống đặt `Status = Refunded`, `RefundedAmount = Amount`, `RefundedAt`, `RefundTransactionId` và lưu lại; khoản hoàn ở đây là toàn bộ số tiền đã trả, không áp dụng `CancelRefundPercent`.

Sau đó hệ thống thông báo bằng thư qua hàng đợi, độc lập với việc hoàn tiền có thành công hay không. Nếu cài đặt rạp có email (`CinemaSettings.Email`), thư gửi rạp (`EmailTemplates.SeatsExpiredAfterPayment`) có tiêu đề "Đã tự động hoàn tiền — Booking #{BookingId}" khi hoàn tiền thành công, hoặc "CẦN HOÀN TIỀN THỦ CÔNG — Booking #{BookingId}" khi không thành công. Nếu đơn có người dùng và email, thư gửi khách (`EmailTemplates.SeatsExpiredAfterPaymentCustomer`) có tiêu đề "Đã hoàn tiền — Booking {BookingCode}" hoặc "Chờ hoàn tiền — Booking {BookingCode}". Khi hoàn tiền tự động thất bại, giao dịch vẫn ở trạng thái `Success` và việc hoàn tiền thủ công do rạp xử lý.

![Hình 4.52. Lưu đồ xử lý ghế hết hạn sau thanh toán](hinh/4.52.png)

*Hình 4.52. Lưu đồ xử lý ghế hết hạn sau thanh toán*

#### 4.5.2.5 So sánh ba cổng thanh toán

Bảng 4.82 so sánh ba cổng dựa trên nội dung của `VnPayGateway.cs`, `MomoGateway.cs`, `StripeGateway.cs` và `HmacSigner.cs`. `HmacSigner` cung cấp `Sha256Hex` và `Sha512Hex`, trả chuỗi hex viết thường. Việc kiểm tra chữ ký của VNPay và MoMo so sánh không phân biệt hoa thường.

**Bảng 4.82. So sánh VNPay, MoMo, Stripe**

| Tiêu chí | VNPay | MoMo | Stripe |
|---|---|---|---|
| Thuật toán ký/xác minh | HMAC-SHA512 (`HashSecret`) trên chuỗi tham số đã sắp theo tên (so sánh thứ tự thông thường), mã hóa URL, bỏ `vnp_SecureHash`, `vnp_SecureHashType` và giá trị rỗng | HMAC-SHA256 (`SecretKey`) trên chuỗi các trường theo thứ tự cố định (`accessKey`, `amount`, `extraData`, `message`, `orderId`, `orderInfo`, `orderType`, `partnerCode`, `payType`, `requestId`, `responseTime`, `resultCode`, `transId`) | Chữ ký webhook của Stripe: `EventUtility.ConstructEvent` với `WebhookSecret`, dung sai 300 giây; nếu `WebhookSecret` để trống, sự kiện được phân tích mà không kiểm tra chữ ký |
| Sự kiện/callback | IPN `GET` và `POST api/v1/payments/vnpay/ipn` | IPN `POST api/v1/payments/momo/ipn` (thân JSON) | Webhook `POST api/v1/payments/stripe/webhook`, chỉ xử lý sự kiện `checkout.session.completed` |
| Điều kiện thành công | `vnp_ResponseCode` = "00" | `resultCode` = "0" | `PaymentStatus` của phiên thanh toán là "paid" |
| Trường khớp giao dịch | `vnp_TxnRef` | `orderId` | `ClientReferenceId` (nếu thiếu thì `metadata.transactionId`) |
| Mã giao dịch phía cổng lưu vào `GatewayTransactionId` | `vnp_TransactionNo` | `transId` | `PaymentIntentId` |
| Cách tạo thanh toán | Dựng trực tiếp URL (`BaseUrl`) kèm `vnp_SecureHash`, không gọi mạng; `vnp_Version` 2.1.0, `vnp_OrderType` "other", `vnp_Locale` "vn" | Gọi API `Endpoint` với `requestType` "captureWallet"; `resultCode` khác 0 thì ném `Rule_MomoCreatePaymentFailed` | Tạo `Checkout Session` (`mode` "payment", phương thức "card"); thiếu `SecretKey` thì ném `Rule_StripeSecretKeyMissing` |
| Hoàn tiền (`RefundAsync`) | Có: gọi `ApiUrl` với lệnh `refund`, `vnp_TransactionType` "02", chữ ký HMAC-SHA512 trên các trường nối bằng "\|"; thành công khi `vnp_ResponseCode` = "00", mã hoàn là `vnp_TransactionNo` | Có: gọi `RefundEndpoint`, chữ ký HMAC-SHA256; dùng `transId` = `GatewayTransactionId` hoặc mã giao dịch gốc; thành công khi `resultCode` = 0, mã hoàn là `orderId` mới (GUID) | Có: `RefundService` với `PaymentIntent` (mã bắt đầu "pi_") hoặc `Charge` ("ch_"); bắt buộc có `GatewayTransactionId`; thành công khi trạng thái hoàn là `succeeded` hoặc `pending` |
| Đơn vị tiền / định dạng số tiền | `vnp_CurrCode` "VND"; số tiền nhân 100 (`AmountScale`) rồi ép kiểu `long`; callback chia 100 | Số tiền ép kiểu `long` rồi gửi dạng chuỗi (không nhân hệ số); callback đọc bằng `decimal.TryParse` | Tiền tệ lấy từ `StripeOptions.Currency` (mặc định "vnd"); với "vnd", "jpy", "krw" gửi số nguyên làm tròn ra xa 0, các loại khác nhân 100; webhook quy đổi ngược theo cùng quy tắc |
| Kiểm tra tiền tệ ở IPN | Không | Không | Có, so với `StripeOptions.Currency` |
| Địa chỉ IP gửi cổng | `vnp_IpAddr` cố định 127.0.0.1 (`DefaultIpAddress`) | — | — |
| Múi giờ thời gian gửi cổng | `vnp_CreateDate` định dạng `yyyyMMddHHmmss` theo múi giờ `Asia/Ho_Chi_Minh` (dự phòng: `SE Asia Standard Time`, rồi UTC+7) | — | — |
| Phản hồi gửi lại cổng | JSON `RspCode`, `Message` | JSON `resultCode`, `message` | JSON `message`; mã 400 chỉ khi chữ ký sai |

### 4.5.3 Hết hạn giữ ghế

Ghế giữ chỉ có hiệu lực đến `HeldUntil`/`ExpiresAt`. Việc giải phóng ghế hết hạn do công việc định kỳ `ReleaseExpiredSeatsJob` thực hiện. Job này được đăng ký bằng `RecurringJob.AddOrUpdate` trong `Program.cs` (mã `RecurringJobIds.ReleaseExpiredSeats`, múi giờ `VietnamTime.Zone`, bỏ qua trong môi trường `Testing`) với biểu thức cron `BackgroundJobOptions.ReleaseExpiredSeatsCron`, mặc định `* * * * *` (mỗi phút). Job chỉ gọi `ISeatExpirationService.ExpireDueBookingsAsync`.

`ExpireDueBookingsAsync` xử lý theo thứ tự sau. Hệ thống lấy mọi đơn `Pending` có `ExpiresAt` nhỏ hơn thời điểm hiện tại. Với từng đơn, hệ thống xác định người giữ ghế, tải các ghế đang `Held` của đơn (`HeldFor`), đặt từng ghế về `Available` và xóa `HeldByUserId`, `HeldByBookingId`, `HeldUntil`; đặt đơn thành `Expired`; đặt mọi giao dịch `Pending` của đơn thành `Failed`; lưu thay đổi của đơn đó; và nếu có ghế được trả thì gọi `NotifyHoldExpiredAsync(showtimeId, bookingId, seatIds)` để thông báo qua kênh thời gian thực, rồi ghi log `BookingExpired`. Sau khi duyệt hết các đơn, hệ thống gọi `ReleaseStaleHeldSeatsAsync(null, ...)`: một câu lệnh `ExecuteUpdateAsync` đưa về `Available` mọi dòng `ShowtimeSeat` còn `Held` có `HeldUntil` nhỏ hơn thời điểm hiện tại mà không thuộc đơn `Pending` nào đã xử lý ở bước trước, rồi ghi log số ghế đã giải phóng nếu lớn hơn 0.

Về nơi gọi: trong mã nguồn ứng dụng (không tính kiểm thử), `ReleaseStaleHeldSeatsAsync` chỉ được gọi từ `ExpireDueBookingsAsync` với tham số `showtimeIds` là `null`, tức quét toàn bộ các suất chiếu. Nhánh lọc theo danh sách mã suất chiếu (và nhánh trả 0 khi danh sách rỗng) hiện chỉ được đặt trong chính hàm này và được kiểm thử đơn vị, chưa có nơi gọi nào trong luồng nghiệp vụ. Hàm này không phát sự kiện thời gian thực cho các ghế mà nó giải phóng. Một hệ quả khác của thiết kế là trong luồng giữ ghế, ghế đang `Held` với `HeldUntil` đã qua chỉ có thể được giữ lại bởi chính người giữ cũ (Mục 4.5.1.2); khách khác phải chờ đến lần chạy kế tiếp của job để thấy ghế `Available`. Các nhận xét đánh giá về hành vi này thuộc Mục 7.3.

![Hình 4.53. Lưu đồ giải phóng ghế hết hạn](hinh/4.53.png)

*Hình 4.53. Lưu đồ giải phóng ghế hết hạn*

### 4.5.4 Hủy vé

#### 4.5.4.1 Điều kiện hoàn tiền

Khách hủy đơn của mình bằng `POST api/v1/bookings/{bookingId}/cancel` (`BookingService.CancelBookingAsync`). Hệ thống chỉ cho chủ đơn hủy (`Forbidden_NotBookingOwner`). Với đơn `Pending`, việc hủy chỉ là trả ghế, không có hoàn tiền. Với đơn `Confirmed`, hệ thống áp dụng lần lượt các điều kiện: chưa có vé nào đã soát (`IsCheckedIn`); có giao dịch `Success` gần nhất và không phải tiền mặt; thỏa chính sách hoàn tiền của rạp; cổng thanh toán đã được cấu hình. Chính sách hoàn tiền nằm trong `CalculateRefundEligibility` và dùng hai tham số của `CinemaSettings`: `CancelRefundPercent` (tỷ lệ hoàn, mặc định 0) và `CancelDeadlineHours` (số giờ tối thiểu trước giờ chiếu, mặc định 2). Nếu `CancelRefundPercent` không dương thì không đủ điều kiện ("Rạp hiện không hỗ trợ hoàn tiền."); nếu thời điểm hiện tại đã quá `StartTime` trừ `CancelDeadlineHours` giờ thì không đủ điều kiện ("Đã quá hạn hoàn tiền — chỉ áp dụng trước giờ chiếu ít nhất {n} giờ."); ngược lại số tiền hoàn là số tiền đã thanh toán nhân `CancelRefundPercent` chia 100, làm tròn về số nguyên theo quy tắc làm tròn ra xa 0. Dữ liệu khởi tạo (`SeedData`) không đặt giá trị cho `CancelRefundPercent` nên mặc định rạp chưa hỗ trợ hoàn tiền cho đến khi quản trị viên cấu hình tỷ lệ này.

**Bảng 4.83. Chính sách và tình huống hoàn tiền**

| Tình huống | Kết quả | Nguồn |
|---|---|---|
| Hủy booking `Pending` | Trả ghế về `Available`, đơn `Cancelled`, giao dịch `Pending` chuyển `Failed`, phát sự kiện ghế được trả; không hoàn tiền (`RefundAmount` rỗng) | `CancelBookingAsync`, `ReleaseAndCancelBookingAsync` |
| Booking đã `Cancelled` hoặc `Expired` | Không cho hủy: `Rule_BookingNotCancellable` ("Booking không ở trạng thái có thể hủy (đã Cancelled/Expired).") | `CancelBookingAsync` |
| Hủy booking `Confirmed` đủ điều kiện | Hoàn `CancelRefundPercent` % số tiền đã thanh toán qua cổng; đơn `Cancelled`, giao dịch `Refunded`; thu hồi điểm vào đúng dòng sổ điểm của booking | `CalculateRefundEligibility`, `ReverseLoyaltyAsync` |
| Hủy booking `Confirmed` đã soát vé | Không cho hủy: `Rule_BookingHasCheckedInTicket` ("Booking đã có vé được check-in — vui lòng liên hệ trực tiếp rạp để được hỗ trợ.") | `CancelBookingAsync` |
| Thanh toán tiền mặt, hoặc không xác định được giao dịch `Success` | Không hủy qua cổng: `Rule_BookingCancelAtCounter` ("Booking thanh toán tại quầy hoặc chưa xác định được giao dịch — vui lòng liên hệ trực tiếp rạp để được hỗ trợ hủy vé.") | `CancelBookingAsync` |
| `CancelRefundPercent` ≤ 0 | Không đủ điều kiện hoàn: "Rạp hiện không hỗ trợ hoàn tiền." | `CalculateRefundEligibility` |
| Quá hạn hoàn tiền (trong vòng `CancelDeadlineHours` giờ trước giờ chiếu) | Không đủ điều kiện hoàn: "Đã quá hạn hoàn tiền — chỉ áp dụng trước giờ chiếu ít nhất {n} giờ." | `CalculateRefundEligibility` |
| Cổng của giao dịch chưa cấu hình | `Rule_RefundGatewayNotConfigured` ("Cổng thanh toán '{0}' chưa được cấu hình.") | `CancelBookingAsync` |
| Cổng từ chối hoàn tiền | `Rule_RefundFailed` ("Hoàn tiền thất bại: {0}. Vui lòng thử lại sau hoặc liên hệ rạp."); giao dịch cơ sở dữ liệu bị hủy nên đơn vẫn `Confirmed` | `CancelBookingAsync` |
| Đơn đang được xử lý hoặc đã bị hủy bởi yêu cầu khác | `Conflict_BookingBeingProcessedOrCancelled` ("Booking đang được xử lý hoặc đã được hủy.") | `CancelBookingAsync` |
| Hủy suất chiếu | Đơn `Pending` và `Confirmed` của suất chuyển `Cancelled`; với đơn `Confirmed` có người dùng, thu hồi điểm theo `FinalAmount`; thanh toán qua cổng được xếp job hoàn toàn bộ `Payment.Amount`; thanh toán tiền mặt gửi thư để hoàn thủ công | `ShowtimeService.DeleteShowtimeAsync`, `ShowtimeRefundJob` |
| Ghế hết hạn sau thanh toán | Tự hoàn toàn bộ số tiền đã trả; nếu không hoàn được thì gửi thư yêu cầu hoàn thủ công | `HandleSeatsExpiredAfterPaymentAsync` |

*Ghi chú: ở dòng "Hủy suất chiếu", `ShowtimeRefundJob` hoàn lại toàn bộ số tiền đã thanh toán, chạy lại theo `BackgroundJobOptions.RetryAttempts` (mặc định 3) và `RetryDelaysSeconds` (mặc định 30, 120, 600 giây); ở lần chạy cuối cùng không thành công, hệ thống gửi thư thông báo hoàn tiền thủ công cho khách rồi ném lỗi để job được ghi nhận thất bại.*

#### 4.5.4.2 Luồng hủy vé

Sau khi các điều kiện ở Mục 4.5.4.1 thỏa, phần quan trọng của luồng được thực hiện trong một giao dịch cơ sở dữ liệu. Bước đầu tiên là chiếm quyền hủy bằng một câu lệnh cập nhật có điều kiện: `ExecuteUpdateAsync` đặt `Status = Cancelled` cho đơn có `BookingId` khớp và `Status` còn là `Confirmed`. Nếu số dòng bị ảnh hưởng là 0, tức một yêu cầu khác (hủy đồng thời hoặc xử lý khác) đã đổi trạng thái đơn, hệ thống ném `ConflictAppException` với `Conflict_BookingBeingProcessedOrCancelled`. Cơ chế này giống nguyên tắc giữ ghế nguyên tử ở Mục 4.5.1.2: chỉ một yêu cầu thắng, nên một đơn không thể bị hoàn tiền hai lần.

Chỉ yêu cầu đã chiếm được quyền mới gọi cổng thanh toán: `gateway.RefundAsync` với mã giao dịch gốc, số tiền hoàn theo chính sách, nội dung "Hoàn tiền hủy vé {BookingCode}", ngày thanh toán gốc (`PaidAt`, hoặc `CreatedAt` nếu thiếu) và `GatewayTransactionId`. Nếu cổng không hoàn được, hệ thống ghi log `RefundFailed` và ném `Rule_RefundFailed`; do giao dịch cơ sở dữ liệu chưa được xác nhận, việc chiếm quyền cũng bị hủy và đơn trở lại `Confirmed`. Nếu hoàn tiền thành công, hệ thống trả các ghế đã đặt về `Available` (`ReleaseConfirmedBookingSeatsAsync`), đặt đơn `Cancelled`, đặt giao dịch `Refunded` với `RefundedAmount`, `RefundedAt` và `RefundTransactionId`.

Tiếp theo là thu hồi điểm, chỉ khi đơn có `UserId`. Hệ thống tính phần giá trị cần thu hồi theo tỷ lệ số tiền hoàn trên số tiền đã thanh toán, `amountToReverse = round(FinalAmount × RefundAmount / payment.Amount)` (làm tròn ra xa 0; bằng 0 nếu `payment.Amount` không dương), rồi gọi `ReverseLoyaltyAsync(userId, bookingId, amountToReverse, ...)`. Phương thức này không trừ chung vào tổng điểm mà thu hồi vào đúng dòng sổ điểm của booking (tìm theo `bookingId`): `RevokedPoints = min(Points, RevokedPoints + ⌊amountToReverse / PointsPerAmount⌋)`, nên số điểm thu hồi không bao giờ vượt quá số điểm dòng đó đã cộng. Nếu dòng sổ điểm của đơn đã trượt khỏi cửa sổ 12 tháng thì việc hoàn tiền không làm giảm điểm hiệu lực hiện tại. Sau đó `TotalSpent` bị trừ đi `amountToReverse` (không xuống dưới 0), và `Points`, `TierId` của hồ sơ được tính lại từ sổ điểm (`ResyncProfileAsync`, không có điểm mới chờ lưu).

Cuối cùng, hệ thống lưu thay đổi và xác nhận giao dịch trong khối `try`. Đây là điểm có rủi ro: lúc này cổng đã hoàn tiền nhưng cơ sở dữ liệu có thể chưa ghi nhận. Nếu lưu hoặc xác nhận thất bại, `catch` ghi log mức nghiêm trọng `RefundCommitFailed` kèm `BookingId`, `PaymentId` và `RefundTransactionId`, với nội dung nêu rõ đã hoàn tiền ở cổng nhưng cơ sở dữ liệu chưa ghi nhận, rồi ném lại ngoại lệ để bộ xử lý lỗi toàn cục trả lỗi. Log này là căn cứ để đối soát thủ công. Khi xác nhận thành công, hệ thống phát `NotifySeatReleasedAsync(showtimeId, seatIds)`, xếp job gửi thư `EmailTemplates.BookingCancelledRefunded` với tiêu đề "Xác nhận hủy vé & hoàn tiền — {tên thương hiệu}", và trả `CancelBookingResponse` với trạng thái "Cancelled", `RefundAmount` và `RefundStatus` "Refunded".

![Hình 4.54. Lưu đồ hủy vé](hinh/4.54.png)

*Hình 4.54. Lưu đồ hủy vé*

Hình 4.54 (phiên bản đã cập nhật) thể hiện luồng trên. Nhánh `Pending` dẫn đến trả ghế và hủy đơn. Nhánh `Confirmed` đi qua các điều kiện: chưa soát vé, không thanh toán tiền mặt, thỏa chính sách hoàn tiền, cổng đã cấu hình; sau đó chiếm quyền hủy bằng cập nhật có điều kiện (không chiếm được thì báo xung đột) và hoàn tiền qua cổng (thất bại thì hủy giao dịch và báo lỗi). Sau bước hoàn tiền qua cổng, bước thu hồi điểm là: thu hồi điểm vào đúng dòng sổ điểm của booking (`bookingId`), tính lại điểm và hạng của hồ sơ. Luồng kết thúc bằng lưu và xác nhận giao dịch (có nhánh ghi log `RefundCommitFailed` nếu thất bại), phát sự kiện, gửi thư và trả kết quả.

<!-- GHI CHÚ ĐỐI CHIẾU
(a) [CẦN BỔ SUNG] còn lại:
- Mã Mermaid Hình 4.47–4.54 (`ma-nguon-so-do.md`) KHÔNG có trong file đính kèm; `ma-nguon-so-do-mermaid-loyalty.md` chỉ chứa bản vá cho Hình 4.51 và 4.54 (dạng mô tả bước, không phải mã đầy đủ). Văn bản mô tả Hình 4.47–4.54 được viết theo mã nguồn (`BookingService.cs`, `PaymentService.cs`, `SeatExpirationService.cs`, ba gateway) và bản vá, chưa đối chiếu được với node/bước trong mã sơ đồ gốc. Cần gửi `ma-nguon-so-do.md` (hoặc đoạn Hình 4.47–4.54; theo ghi chú trong `ma-nguon-so-do-bo-sung.md`, đây là Hình 4.25–4.32 cũ cộng 22) để rà khớp.
- Phụ lục 5: đoạn mã rút gọn ≤ 15 dòng của câu lệnh giữ ghế nguyên tử chỉ được dẫn chiếu, chưa dựng (thuộc phần Phụ lục).
- Use case UC-DV-04 (Mục 3.3.5) và UC-TT-01…06 (Mục 3.3.6) chỉ dẫn chiếu theo outline, chưa đối chiếu nội dung Chương 3 trong phần này.
- Mục 4.5.8 (điểm tích theo `FinalAmount`) được dẫn chiếu, nằm ngoài phạm vi Phần 19.
(b) Chỗ outline lệch code / điểm cần lưu ý:
- Bảng 4.81: outline đặt thứ tự 1 Validation, 2 trạng thái suất chiếu, 3 booking Pending, 4 luật chọn ghế. Code: `SeatLayoutRules.ValidateSelection` chạy trong `HoldSeatsAsync` TRƯỚC khi vào `CreateHoldBookingAsync` (nên đứng trước các kiểm tra suất chiếu và booking Pending). Bảng đã sắp theo thứ tự code, tách luật chọn ghế thành 2 dòng (ghế đôi, ghế lẻ) và bổ sung các dòng suất chiếu không tồn tại, thời điểm dừng bán online, tên khách, mã combo, sinh mã booking, xung đột ghế. Các thông báo luật chọn ghế là chuỗi tiếng Việt cố định trong `SeatLayoutRules.cs`, không nằm trong `Messages.resx`.
- Mục 4.5.1.3: outline nói "Mã booking: tiền tố + ngày + 4 ký tự". Đúng với code; tiền tố cấu hình qua `Brand:BookingCodePrefix` (`CodeGenerator.Configure`, mặc định "BK"), ngày theo `VietnamTime.Today()`.
- Công thức (4.2): outline viết `round(TicketAmount × DiscountPercent / 100)` — khớp `CalculateDiscountAmountAsync` (`Math.Round(..., 0, MidpointRounding.AwayFromZero)`); hạng đọc từ sổ điểm bằng `LoyaltyWindow.SumActiveAsync` + `ResolveTierAsync`, không đọc `CustomerProfile.TierId`.
- Mục 4.5.2.2 / Bảng 4.82: outline ghi Stripe "chữ ký webhook"; code còn cho phép bỏ qua xác minh chữ ký khi `Stripe:WebhookSecret` trống (`VerifyWebhook` trả `signatureValid: true`). Chỉ ghi nhận sự thật; nếu cần đánh giá thì đưa vào Mục 7.3. Outline ghi "kiểm tra tiền tệ (Stripe)" — đúng, VNPay/MoMo không có.
- Bảng 4.82: ba gateway đều so sánh chữ ký không phân biệt hoa thường (VNPay, MoMo) — đã ghi. `StripeGateway.VerifyCallback` (tham số `session_id`, `canceled`) tồn tại nhưng luồng IPN dùng `VerifyWebhook`; `VerifyCallback` của Stripe không được dùng trong `PaymentService` (không nêu trong bài).
- Bảng 4.83: outline có 6 dòng; đã giữ đủ và bổ sung các dòng điều kiện (trạng thái không hủy được, `CancelRefundPercent` ≤ 0, quá hạn, cổng chưa cấu hình, cổng từ chối, xung đột). Giá trị mặc định `CancelRefundPercent` = 0 và dữ liệu khởi tạo không đặt giá trị nên mặc định chưa hỗ trợ hoàn tiền; thông báo "Rạp hiện không hỗ trợ hoàn tiền." và thông báo quá hạn là chuỗi cố định trong `CalculateRefundEligibility`.
- Bảng 4.83 dòng "Hủy suất chiếu": outline chỉ ghi "Hoàn tiền từng thanh toán bằng job". Thực tế (`ShowtimeService.DeleteShowtimeAsync`, nằm ngoài danh sách nguồn của outline nhưng đã đọc) còn hủy cả đơn `Pending`, thu hồi điểm theo toàn bộ `FinalAmount` cho đơn `Confirmed` và gửi thư hoàn thủ công cho thanh toán tiền mặt.
- Hình 4.51: bản vá viết hai thông điệp "ghi LoyaltyPointEntry" rồi "tính lại Points, TierId". Trong code cả hai thao tác chỉ được chuẩn bị trong bộ nhớ (`db.LoyaltyPointEntries.Add`, `ResyncProfileAsync` đọc tổng các dòng đã lưu cộng điểm mới) và được lưu cùng lúc bằng một `SaveChangesAsync` trong giao dịch; dòng sổ điểm chỉ được thêm khi `earned > 0`; `TotalSpent` cũng được cộng. Văn bản đã mô tả đúng như vậy.
- Hình 4.54: bản vá khớp code (`ReverseLoyaltyAsync` tìm dòng theo `BookingId`, `RevokedPoints = min(Points, RevokedPoints + ⌊amountToReverse/PointsPerAmount⌋)`). Lưu ý `amountToReverse = round(FinalAmount × RefundAmount / Payment.Amount)`, còn Hình 4.58 (file loyalty) ghi `⌊RefundedAmount / PointsPerAmount⌋`; hai biểu thức trùng khi `Payment.Amount = FinalAmount`. Nên đối chiếu khi viết phần sổ điểm.
- Giới hạn tần suất `booking-hold`: `Program.cs` dùng `httpContext.User.Identity?.Name` làm khóa phân vùng (chú thích "mỗi user"), nhưng access token do `JwtTokenService.BuildBaseClaims` không chứa claim tên; có thể khóa rơi về địa chỉ IP. Chưa kiểm chứng bằng chạy thử nên bài không khẳng định, chỉ nêu "10 yêu cầu mỗi 60 giây".
- `PaymentMethodNames` có hằng `ZaloPay` nhưng không có gateway và `CheckoutRequestValidator` chỉ cho "VNPay", "Momo", "Stripe"; không nêu ZaloPay trong bài.
- Mục 4.5.3: outline yêu cầu "chỉ trình bày sự thật" về nơi gọi `ReleaseStaleHeldSeatsAsync`; đã ghi: chỉ gọi từ `ExpireDueBookingsAsync` với `null`; nhánh lọc theo danh sách suất chiếu chỉ có test.
-->

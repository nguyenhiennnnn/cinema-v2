# CHƯƠNG 2
# TỔNG QUAN VÀ CƠ SỞ LÝ THUYẾT

## 2.1 BỐI CẢNH BÀI TOÁN

### 2.1.1 Vòng đời của một lần đặt vé

Một lần đặt vé đi qua sáu bước: chọn suất chiếu, chọn ghế, giữ chỗ có thời hạn, thanh toán, phát hành vé có mã QR và soát vé. Ba thực thể thay đổi trạng thái trong quá trình này là đơn đặt vé (`Booking`), ghế của suất chiếu (`ShowtimeSeat`) và thanh toán (`Payment`). Các tập trạng thái được định nghĩa trong ba kiểu liệt kê của `Cinema.Domain/Enums`: `BookingStatus` gồm `Pending`, `Confirmed`, `Cancelled`, `Expired`; `ShowtimeSeatStatus` gồm `Available`, `Held`, `Booked`; `PaymentStatus` gồm `Pending`, `Success`, `Failed`, `Refunded`. Hình 2.1 mô tả sự thay đổi trạng thái theo từng bước.

![Hình 2.1 Vòng đời của một lần đặt vé](hinh/2.1.png)

*Hình 2.1. Vòng đời của một lần đặt vé*

Khi người dùng chọn ghế, `BookingService.HoldSeatsAsync` (qua `CreateHoldBookingAsync`) chuyển các ghế đã chọn từ `Available` sang `Held`, ghi `HeldByUserId` và `HeldUntil`, đồng thời tạo `Booking` ở trạng thái `Pending` với `ExpiresAt` bằng thời hạn giữ chỗ. Thời hạn giữ chỗ là `HoldDurationMinutes` (mặc định 15 phút) và không vượt giờ bắt đầu suất chiếu. Sau khi `Booking` được tạo, mã đơn được gán vào `HeldByBookingId` của các ghế đã giữ.

Khi người dùng thanh toán, `BookingService.CheckoutAsync` chỉ chấp nhận `Booking` còn ở trạng thái `Pending` và chưa quá `ExpiresAt`. Các `Payment` đang `Pending` của đơn đó bị chuyển sang `Failed`, rồi một `Payment` mới ở trạng thái `Pending` được tạo theo phương thức thanh toán đã chọn, kèm địa chỉ thanh toán của cổng.

Kết quả thanh toán đến sau, qua thông báo của cổng. Với kết quả thành công, `PaymentService.ApplyPaymentResultAsync` đặt `Payment` sang `Success` và gọi `BookingService.ConfirmBookingAndGenerateTicketsAsync`: `Booking` chuyển sang `Confirmed`, các ghế chuyển từ `Held` sang `Booked`, và mỗi ghế được phát hành một `BookingTicket` có `QrCode` và cờ `IsCheckedIn` ban đầu là `false`. Với kết quả thất bại, `Payment` chuyển sang `Failed`; nếu `Booking` còn hạn giữ chỗ thì ghế vẫn được giữ để người dùng thanh toán lại, nếu đã hết hạn thì `Booking` chuyển sang `Cancelled` và các ghế trở về `Available` (`ProcessPaymentFailureAsync`).

Có ba đường thoát khác khỏi trạng thái `Pending`. Người dùng có thể hủy đơn chưa thanh toán: `Booking` chuyển sang `Cancelled`, ghế trở về `Available` và các `Payment` còn `Pending` chuyển sang `Failed`. Nếu hết hạn giữ chỗ, tác vụ nền `ReleaseExpiredSeatsJob` gọi `SeatExpirationService.ExpireDueBookingsAsync` để chuyển `Booking` sang `Expired`, trả ghế về `Available` và chuyển các `Payment` còn `Pending` sang `Failed`. Nếu thanh toán thành công nhưng ghế đã không còn được giữ (`SeatsNoLongerHeldException`), `PaymentService.HandleSeatsExpiredAfterPaymentAsync` thử hoàn tiền tự động; khi cổng hoàn tiền thành công, `Payment` chuyển sang `Refunded`.

Với đơn đã `Confirmed`, người dùng có thể hủy theo điều kiện hoàn tiền (`CancelBookingAsync`): `Booking` chuyển sang `Cancelled`, `Payment` chuyển sang `Refunded` và các ghế `Booked` trở về `Available`. Khi khách vào rạp, nhân viên soát vé bằng `BookingService.CheckinAsync`; việc đã soát được ghi ở cờ `IsCheckedIn` của từng `BookingTicket` chứ không phải ở một giá trị riêng của `BookingStatus`.

Đơn bán tại quầy (`BookingChannel.Counter`) dùng cùng cơ chế giữ ghế nhưng thanh toán tại chỗ: `CounterCheckoutAsync` tạo `Payment` ở trạng thái `Success` ngay, không đi qua cổng thanh toán.

### 2.1.2 Phát biểu bài toán

Bài toán của đề tài chia thành ba nhóm, mỗi nhóm được mô tả theo đầu vào, đầu ra và ràng buộc.

**Nhóm thứ nhất: tranh chấp ghế.** Đầu vào là một tập ghế (`showtimeSeatIds`) của một suất chiếu do một người dùng gửi lên, và có thể có nhiều yêu cầu như vậy đến cùng lúc cho các tập ghế giao nhau. Đầu ra là một đơn `Pending` giữ toàn bộ các ghế đã chọn cho đúng một người, hoặc một lỗi xung đột kèm danh sách ghế không giữ được (`SeatConflictException`, mã 409). Ràng buộc: yêu cầu giữ ghế theo nguyên tắc tất cả hoặc không ghế nào; một ghế tại một thời điểm chỉ thuộc một người giữ; ghế giữ không vượt `HoldDurationMinutes` và giờ bắt đầu suất chiếu; mỗi người chỉ có một đơn `Pending` còn hạn cho mỗi suất (hoặc mỗi nhân viên, với kênh quầy).

**Nhóm thứ hai: nhất quán thanh toán.** Đầu vào là các thông báo bất đồng bộ từ cổng (VNPay, MoMo, Stripe) có thể trễ, lặp, sai chữ ký hoặc sai số tiền, và có thể đến sau khi ghế đã hết hạn. Đầu ra là trạng thái `Payment`, `Booking` và `ShowtimeSeat` luôn khớp nhau: thanh toán hợp lệ cho đúng một lần xác nhận đơn và phát hành vé; thanh toán hợp lệ nhưng ghế đã mất thì được hoàn tiền. Ràng buộc: thông báo xử lý lại không được tạo thêm tác dụng phụ (`IsAlreadyProcessed`); số tiền đối chiếu với `Payment.Amount` với sai số không quá 0,01; thông báo Stripe còn được đối chiếu loại tiền tệ.

**Nhóm thứ ba: vận hành.** Gồm ba bài toán nhỏ. Một là xếp lịch chiếu: `ShowtimeScheduler.Build` nhận ngày, giờ mở và đóng cửa, thời gian đệm, danh sách phòng, danh sách phim kèm độ nóng (`Heat`) và số suất cần xếp, và các khoảng thời gian phòng đã bận; trả về danh sách `SchedulerSlot` gồm phim, phòng, giờ bắt đầu và kết thúc, sao cho các suất không chồng nhau trong cùng phòng, các suất của cùng một phim cách nhau tối thiểu `MinSameMovieGapMinutes` (mặc định 60 phút) và nằm trong khung giờ cho phép. Hai là gợi ý ghế: `SeatLayoutRules.Suggest` chọn các cụm ghế liền nhau theo số lượng khách. Ba là luật ghế: `SeatLayoutRules.ValidateSelection` yêu cầu ghế đôi phải được chọn cùng nhau và, theo `SeatGapRuleMode` (`Strict`, `AllowEdge`, `Off`), không để lại một ghế trống lẻ giữa hàng.

## 2.2 CÁC HỆ THỐNG LIÊN QUAN

### 2.2.1 Hệ thống đặt vé xem phim hiện có

Phần này so sánh một số hệ thống đặt vé xem phim đang hoạt động theo các tiêu chí đã nêu ở Mục 1.1.3. Chỉ những hệ thống đã được khảo sát, có tên và ngày truy cập mới được đưa vào Bảng 2.1. Cột cuối là hệ thống Perfo, được điền theo mã nguồn để đối chiếu.

[CẦN BỔ SUNG: kết quả khảo sát 3–5 hệ thống đặt vé công khai: tên hệ thống, địa chỉ, ngày truy cập, và kết quả quan sát theo từng tiêu chí.]

**Bảng 2.1. So sánh các hệ thống đặt vé hiện có**

| Tiêu chí | Hệ thống 1 | Hệ thống 2 | Hệ thống 3 | Perfo |
|---|---|---|---|---|
| Tên hệ thống, ngày truy cập | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: kết quả khảo sát] | Không áp dụng |
| Chọn ghế trực quan | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: đối chiếu giao diện frontend] |
| Giữ ghế có đếm ngược | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: kết quả khảo sát] | Giữ ghế đến `ExpiresAt` (mặc định 15 phút); đếm ngược ở giao diện [CẦN BỔ SUNG: đối chiếu frontend] |
| Phương thức thanh toán | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: kết quả khảo sát] | VNPay, MoMo, Stripe (sandbox/test); ZaloPay chỉ có hằng số |
| Ghế đôi | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: kết quả khảo sát] | Có (`CoupleId`; phải chọn cùng nhau) |
| Soát vé QR | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: kết quả khảo sát] | Có (`BookingTicket.QrCode`, `CheckinAsync`) |
| Trợ lý AI | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: kết quả khảo sát] | Có (`Gemini`, xem Mục 4.8) |
| Hoàn tiền | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: kết quả khảo sát] | [CẦN BỔ SUNG: kết quả khảo sát] | Có (hủy đơn `Confirmed`; tự hoàn khi ghế hết hạn sau thanh toán) |

### 2.2.2 Hướng tiếp cận chống xung đột khi đặt chỗ

Có bốn hướng tiếp cận thường dùng để ngăn hai người cùng giữ một chỗ. Khóa bi quan khóa dòng dữ liệu của ghế ngay khi đọc (ví dụ `SELECT … FOR UPDATE` trong cơ sở dữ liệu quan hệ), các yêu cầu khác phải chờ đến khi giao dịch kết thúc; cách này đơn giản và đúng, nhưng giữ khóa lâu khi nhiều người tranh cùng ghế và dễ gây chờ hoặc bế tắc. Khóa lạc quan không khóa khi đọc mà gắn một số phiên bản cho dòng dữ liệu và chỉ ghi nếu số phiên bản chưa đổi; người ghi thua nhận lỗi và phải thử lại, nên hiệu quả khi xung đột ít. Cập nhật có điều kiện nguyên tử gộp bước kiểm tra và bước ghi vào một câu lệnh duy nhất (ví dụ `UPDATE … WHERE Status = 'Available'`) và dựa vào số dòng bị ảnh hưởng để biết có giữ được hay không; cơ sở dữ liệu tự bảo đảm tính nguyên tử của từng dòng, không cần giữ khóa qua nhiều bước ở tầng ứng dụng. Hàng đợi hoặc phòng chờ ảo xếp người dùng vào hàng đợi trước khi cho vào chọn ghế, giảm tải đột biến khi mở bán nhưng cần thêm thành phần quản lý hàng đợi. [CẦN BỔ SUNG: trích dẫn nhóm Tx cho các hướng tiếp cận này]

Perfo dùng hướng thứ ba, và mã nguồn thể hiện như sau. Trong `BookingService.CreateHoldBookingAsync`, sau khi kiểm tra suất chiếu và đơn `Pending` hiện có, hệ thống mở một giao dịch (`BeginTransactionAsync`) rồi chạy một câu lệnh `ExecuteUpdateAsync` trên `ShowtimeSeats`. Câu lệnh chỉ cập nhật các dòng thuộc đúng suất chiếu và có `ShowtimeSeatId` nằm trong danh sách đã chọn, với điều kiện `Status` là `Available`, hoặc `Status` là `Held` do chính người đang yêu cầu giữ (`HeldByUserId` bằng người đó) và đã quá `HeldUntil`. Các dòng thỏa điều kiện được đặt `Status = Held`, `HeldByUserId`, `HeldUntil`. Sau câu lệnh, hệ thống so số dòng bị ảnh hưởng với số ghế khác nhau đã yêu cầu. Nếu hai số này khác nhau, giao dịch bị hủy (`RollbackAsync`) và `SeatConflictException` được ném ra kèm danh sách ghế xung đột, được middleware trả về với mã 409 và khóa `conflictSeatIds`. Nếu bằng nhau, hệ thống tạo `Booking` `Pending`, gán `HeldByBookingId` cho các ghế rồi xác nhận giao dịch (`CommitAsync`).

Lớp `ShowtimeSeatConfiguration` bổ sung các ràng buộc ở cơ sở dữ liệu cho cơ chế trên. Chỉ mục duy nhất `UQ_Showtime_Seat` trên cặp (`ShowtimeId`, `SeatId`) bảo đảm mỗi ghế vật lý chỉ có đúng một dòng trạng thái cho mỗi suất chiếu. Ràng buộc kiểm tra `CK_ShowtimeSeats_Status` giới hạn `Status` trong ba giá trị `Available`, `Held`, `Booked`, và `CK_ShowtimeSeats_Price` yêu cầu `Price >= 0`. Việc loại trừ hai người giữ cùng một ghế do điều kiện `Status = Available` trong câu lệnh cập nhật đảm nhiệm; chỉ mục duy nhất không tự thực hiện việc đó. Chỉ mục `IX_ShowtimeSeats_Status` trên (`ShowtimeId`, `Status`) hỗ trợ tra cứu ghế theo trạng thái.

Hai điểm cần lưu ý khi đối chiếu với các hướng còn lại. Thứ nhất, mã nguồn không dùng khóa bi quan (không có câu lệnh khóa dòng) và không cấu hình số phiên bản dòng (không có `RowVersion` hay token đồng thời); middleware có xử lý `DbUpdateConcurrencyException` thành mã 409 nhưng đây là xử lý chung, không phải cơ chế giữ ghế. Thứ hai, mã nguồn không có hàng đợi ảo. Điều kiện cho phép chiếm lại ghế `Held` quá hạn chỉ áp dụng cho chính người đã giữ; ghế `Held` quá hạn của người khác được trả về `Available` bởi tác vụ nền (`SeatExpirationService`, chạy theo `ReleaseExpiredSeatsCron`, mặc định mỗi phút) trước khi người khác giữ được.

**Bảng 2.2. So sánh các hướng tiếp cận chống xung đột đặt chỗ**

| Hướng tiếp cận | Cơ chế | Ưu điểm | Nhược điểm | Trong Perfo |
|---|---|---|---|---|
| Khóa bi quan | Khóa dòng khi đọc, giữ đến hết giao dịch | Đúng và dễ hiểu | Giữ khóa lâu khi tranh chấp nhiều; có nguy cơ chờ và bế tắc | Không dùng |
| Khóa lạc quan | Số phiên bản dòng; ghi chỉ thành công nếu số phiên bản chưa đổi | Không giữ khóa; phù hợp khi ít xung đột | Người thua phải thử lại; cần thêm cột phiên bản | Không dùng (chỉ có xử lý chung `DbUpdateConcurrencyException` → 409) |
| Cập nhật có điều kiện nguyên tử | Một câu lệnh `UPDATE … WHERE` kiểm tra và ghi cùng lúc; so số dòng bị ảnh hưởng | Gọn; cơ sở dữ liệu bảo đảm tính nguyên tử theo dòng; không cần khóa ở tầng ứng dụng | Phải xử lý trường hợp một phần ghế không giữ được (hủy cả giao dịch) | Dùng trong `CreateHoldBookingAsync` (`ExecuteUpdateAsync` trong giao dịch), kèm `UQ_Showtime_Seat`, `CK_ShowtimeSeats_Status` |
| Hàng đợi/phòng chờ ảo | Xếp người dùng vào hàng đợi trước khi cho chọn ghế | Giảm tải đột biến khi mở bán | Cần thành phần quản lý hàng đợi; tăng độ phức tạp | Không dùng |

Lý do chọn cập nhật có điều kiện nguyên tử cho đề tài là cơ chế này đủ để bảo đảm mục tiêu MT2 (Bảng 1.1) mà không thêm cột phiên bản hay thành phần hạ tầng mới, và phù hợp với phạm vi một rạp (Mục 1.4). Hiệu quả của lựa chọn này được kiểm chứng ở Mục 6.5.

### 2.2.3 Khoảng trống đề tài giải quyết

Các kỹ thuật dùng trong đề tài (cập nhật có điều kiện, xử lý thông báo thanh toán idempotent, truyền thông thời gian thực) là những kỹ thuật đã được biết đến. Đóng góp của đề tài vì vậy được định vị ở việc thiết kế và tích hợp chúng thành một hệ thống đặt vé thống nhất cho một rạp, từ giữ ghế, thanh toán, hoàn tiền đến bán vé quầy và soát vé, chứ không phải đề xuất một kỹ thuật mới. [CẦN BỔ SUNG: khoảng trống so với các hệ thống hiện có, rút ra từ kết quả khảo sát ở Mục 2.2.1; chỉ nêu điểm khác biệt đã được khảo sát xác nhận.]

## 2.3 PHẠM VI ĐỀ TÀI

Về chức năng, các package use case thuộc phạm vi đề tài được phân tích ở Mục 3.3, và các nội dung ngoài phạm vi (nhiều chi nhánh, ứng dụng di động gốc, đối soát kế toán, thanh toán ZaloPay, đổi điểm lấy quà hoặc voucher, thông báo điểm sắp hết hạn) đã nêu ở Mục 1.5.

Về giả định vận hành, hệ thống phục vụ một rạp: bảng `CinemaSettings` bị ràng buộc `Id = 1` và lưu các tham số vận hành của rạp như `HoldDurationMinutes` (mặc định 15), `CheckInOpenMinutes` (mặc định 30), `CancelDeadlineHours` (mặc định 2), `CancelRefundPercent` (mặc định 0), `StopSellingMinutesBefore`, `PointsPerAmount` (mặc định 10.000) và `SeatGapRuleMode` (mặc định `Strict`). Tiền tệ là VND và múi giờ là `Asia/Ho_Chi_Minh` (giờ UTC+7), thời gian được lưu theo UTC. Các cổng thanh toán chạy ở môi trường sandbox hoặc thử nghiệm.

## 2.4 PHƯƠNG PHÁP NGHIÊN CỨU

### 2.4.1 Phương pháp thực hiện

Đề tài được thực hiện theo sáu bước nối tiếp nhau (Hình 2.2). Bước đầu là nghiên cứu lý thuyết về các vấn đề của bài toán đặt vé, gồm kiểm soát đồng thời, thanh toán bất đồng bộ và truyền thông thời gian thực (Mục 2.5). Từ đó xác định yêu cầu bằng mô hình use case (Chương 3). Bước thiết kế xác định kiến trúc, mô hình dữ liệu và các luồng nghiệp vụ (Chương 4). Bước cài đặt được chia theo module, gồm backend và frontend (Chương 5). Hệ thống sau đó được kiểm thử tự động ở các mức đơn vị, tích hợp và đầu cuối (Chương 6). Bước cuối là đóng gói bằng Docker và triển khai (Mục 5.7).

![Hình 2.2 Quy trình thực hiện đề tài](hinh/2.2.png)

*Hình 2.2. Quy trình thực hiện đề tài*

Các công cụ hỗ trợ có trong dự án gồm:

- Swagger (Swashbuckle.AspNetCore, phiên bản 10.2.3), cấu hình trong `Program.cs` (`AddSwaggerGen`, `UseSwagger`, `UseSwaggerUI`), để xem và thử các điểm cuối API.
- Migration của Entity Framework Core thông qua công cụ `dotnet-ef` (phiên bản 10.0.11, khai báo trong `dotnet-tools.json`), với các script `db:migration:add`, `db:migration:list`, `db:migration:script` và `db:update` trong `package.json`.
- Các script npm ở thư mục gốc để cài đặt, chạy, dựng và kiểm thử, ví dụ `setup`, `dev`, `build`, `test:unit`, `test:integration`, `test:e2e`, `check`.
- CSharpier (phiên bản 1.3.0, khai báo trong `dotnet-tools.json`), chạy qua `format` và `format:check`, để định dạng mã C#.
- ESLint, cấu hình ở `frontend/eslint.config.js` (kết hợp `typescript-eslint`, `eslint-plugin-react-hooks` và `eslint-plugin-react-refresh`), chạy qua script `lint`, kèm kiểm tra kiểu bằng `typecheck` (`tsc -b`).

### 2.4.2 Phương pháp đánh giá

Đề tài đánh giá hệ thống theo năm hướng, tương ứng các mục tiêu ở Bảng 1.1.

Về chức năng, đề tài lập bảng truy vết từ yêu cầu (package use case) đến phần cài đặt và các test (Mục 7.1). Về an toàn đồng thời khi đặt ghế, đề tài thực hiện một thực nghiệm gửi 10 yêu cầu giữ cùng một ghế song song và kiểm tra có đúng một yêu cầu thành công (Mục 6.5). Về kiểm thử tự động ở backend, dự án có hai bộ test là `backend/tests/Cinema.UnitTests` và `backend/tests/Cinema.IntegrationTests`, chạy bằng `test:unit` và `test:integration`; độ phủ có thể thu thập bằng `test:backend:coverage` (`XPlat Code Coverage`). Về giao diện, kiểm thử đầu cuối dùng Playwright (`@playwright/test`, cấu hình ở `frontend/playwright.config.ts`, thư mục kịch bản `./e2e`), chạy bằng `test:e2e`. Về hiệu năng, `package.json` có các script `test:load:browse` và `test:load:peak` gọi `load-tests/run-load-test.ps1` để chạy kịch bản k6; tuy nhiên thư mục `load-tests/` chưa có trong dự án. [CẦN BỔ SUNG: thư mục `load-tests/` (kịch bản k6 `browse-movies-stress.js`, `peak-ticket-rush.js`) và kết quả đo, ngưỡng p95 mong muốn.] Về bảo mật, đề tài rà soát các cơ chế đã cài đặt (Mục 5.6) và không thực hiện kiểm thử xâm nhập.

<!-- GHI CHÚ ĐỐI CHIẾU
(a) [CẦN BỔ SUNG] còn lại:
- 2.2.1 / Bảng 2.1: toàn bộ kết quả khảo sát 3–5 hệ thống (tên, ngày truy cập, quan sát theo 7 tiêu chí). Cột Perfo đã điền theo code, riêng "chọn ghế trực quan" và "đếm ngược" chưa đối chiếu giao diện frontend.
- 2.2.2: trích dẫn nhóm Tx cho bốn hướng tiếp cận chống xung đột.
- 2.2.3: khoảng trống đề tài giải quyết, phải dựa trên kết quả khảo sát. Đã viết định vị "thiết kế và tích hợp", không nói "mới hoàn toàn".
- 2.4.2: thư mục `load-tests/` (k6) và ngưỡng/kết quả đo; `package.json` tham chiếu `load-tests/run-load-test.ps1` nhưng thư mục không có trong zip.
- Hình 2.1, 2.2: chưa có `ma-nguon-so-do.md` trong tài liệu được cung cấp nên chưa đối chiếu mã sơ đồ; phần mô tả Hình 2.1 dựng từ code (BookingService, PaymentService, SeatExpirationService) và 3 enum, Hình 2.2 từ outline 2.4.1. Cần đối chiếu lại khi có mã sơ đồ; mọi node trạng thái trong hình chỉ được dùng đúng giá trị enum nêu ở Mục 2.1.1.
- Thư mục E2E: `frontend/playwright.config.ts` có `testDir: "./e2e"` nhưng thư mục `frontend/e2e` và `bo_sung/e2e` không có trong zip; chưa đối chiếu số kịch bản.

(b) Chỗ outline lệch code hoặc cần lưu ý:
- Outline 2.2.2 mô tả cách Perfo dùng là "cập nhật có điều kiện nguyên tử"; code đúng như vậy. Outline Chương 8 gọi là "ràng buộc duy nhất"; thực tế `UQ_Showtime_Seat` chỉ bảo đảm mỗi ghế có một dòng trạng thái mỗi suất, còn việc loại trừ đặt trùng do điều kiện `WHERE` của `ExecuteUpdateAsync`. Đã viết theo code.
- Code không có khóa bi quan (không tìm thấy FOR UPDATE), không có RowVersion/ConcurrencyToken/xmin, không có hàng đợi ảo; ghi "Không dùng" trong Bảng 2.2.
- Điều kiện chiếm lại ghế `Held` quá hạn chỉ cho chính người giữ (`HeldByUserId == holderUserId`); ghế quá hạn của người khác phụ thuộc tác vụ nền (cron `* * * * *`). Đã nêu ở 2.2.2; liên quan hạn chế "giải phóng ghế hết hạn ngay khi truy vấn" ở Mục 8.3.
- Hình 2.1: `BookingStatus.Expired` chỉ do `SeatExpirationService.ExpireDueBookingsAsync` đặt; `ProcessPaymentFailureAsync` và hủy đơn `Pending` đặt `Cancelled`. Trong `ConfirmBookingAndGenerateTicketsAsync`, nếu không còn ghế giữ thì `Booking` bị đặt `Cancelled` rồi ném `SeatsNoLongerHeldException`. Không có trạng thái "đã soát vé" ở `BookingStatus`; soát vé là cờ `BookingTicket.IsCheckedIn`.
- Mục 2.1.2: số 0,01 là sai số đối chiếu số tiền ở `PaymentService` (`Math.Abs(payment.Amount - callback.Amount) > 0.01m`).
- Mục 2.3: outline nêu "UTC+7"; code chỉ có `Brand:TimeZoneId = Asia/Ho_Chi_Minh` (kiểm tra ở `VietnamTime.cs`), chưa có chuỗi "UTC+7" trong code.
- Mục 2.4.1: số script npm và công cụ lấy từ `package.json`, `dotnet-tools.json`, `frontend/eslint.config.js`; "đóng gói bằng Docker" dựa trên script `docker:*` và `docker-compose.yml`, chi tiết thuộc Mục 5.7.
- Prompt gọi `quy-dinh-trinh-bay.md`; file thực tế là `quy-dinh-format-bao-cao.md`. Caption: bảng đặt trên, hình đặt dưới (Mục 0.3).
-->

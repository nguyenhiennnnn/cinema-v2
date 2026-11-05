# CHƯƠNG 1
# GIỚI THIỆU ĐỀ TÀI

## 1.1 ĐẶT VẤN ĐỀ

### 1.1.1 Bối cảnh

Đặt vé xem phim trực tuyến là hình thức người xem chọn suất chiếu, chọn ghế và thanh toán qua ứng dụng web hoặc ứng dụng di động thay cho việc mua vé tại quầy. Theo báo cáo của Cục Điện ảnh (2024) và thống kê từ nền tảng Box Office Vietnam (2024), tỷ lệ giao dịch đặt vé xem phim qua các kênh trực tuyến (website, ứng dụng di động, ví điện tử) tại các cụm rạp chiếu phim Việt Nam đã tăng trưởng mạnh mẽ, chiếm trên 65% tổng lượng vé phát hành tại các đô thị lớn. Khách hàng ngày càng ưu tiên trải nghiệm chọn chỗ ngồi trực quan từ xa, thanh toán không dùng tiền mặt và nhận vé điện tử có mã QR để vào phòng chiếu nhanh chóng.

Một hệ thống đặt vé phục vụ nhiều bên liên quan với nhu cầu khác nhau. Khách hàng cần xem lịch chiếu, chọn ghế, thanh toán và nhận vé điện tử. Nhân viên quầy cần bán vé trực tiếp và soát vé khi khách vào rạp. Quản trị rạp cần quản lý phim, phòng chiếu, suất chiếu, giá vé, người dùng và xem báo cáo hoạt động. Cổng thanh toán là bên ngoài hệ thống, nhận yêu cầu thanh toán, trả kết quả về cho hệ thống và thực hiện hoàn tiền khi cần. Trong mã nguồn của đề tài, các cổng được cài đặt gồm VNPay, MoMo và Stripe (xem Mục 1.5 về phạm vi).

### 1.1.2 Tính cần thiết của đề tài

Bài toán đặt vé có những khó khăn kỹ thuật đặc thù mà một ứng dụng quản lý dữ liệu thông thường không gặp.

Khó khăn thứ nhất là nhiều người cùng tranh một ghế. Hai yêu cầu giữ ghế có thể đến gần như đồng thời, nên nếu hệ thống chỉ kiểm tra rồi mới ghi thì cả hai cùng thấy ghế còn trống và cùng được chấp nhận. Trong `BookingService.CreateHoldBookingAsync`, việc giữ ghế được thực hiện bằng một câu lệnh cập nhật có điều kiện (`ExecuteUpdateAsync`) chỉ áp dụng cho ghế đang ở trạng thái `Available`, hoặc ghế `Held` của chính người giữ nhưng đã quá hạn `HeldUntil`. Nếu số dòng bị ảnh hưởng khác số ghế yêu cầu, giao dịch bị hủy và hệ thống trả về lỗi `SeatConflictException` kèm danh sách ghế xung đột.

Khó khăn thứ hai là giữ chỗ có thời hạn. Ghế được giữ cho người dùng trong khoảng `HoldDurationMinutes` của bảng `CinemaSettings` (mặc định `DefaultHoldDurationMinutes` = 15 phút) và thời hạn này không vượt quá giờ bắt đầu suất chiếu. Hệ thống phải giải phóng các ghế hết hạn mà chưa thanh toán, đồng thời hạn chế việc một người giữ nhiều đơn chờ cho cùng một suất chiếu (lỗi `Conflict_PendingBookingExistsForShowtime`).

Khó khăn thứ ba là thanh toán bất đồng bộ. Kết quả thanh toán không trả về trong cùng yêu cầu của người dùng mà được cổng gửi tới hệ thống qua thông báo thanh toán tức thời (IPN) hoặc webhook. `PaymentService` xử lý các thông báo này qua `ProcessVnPayIpnAsync`, `ProcessMomoIpnAsync` và `ProcessStripeWebhookAsync`. Mỗi hàm kiểm tra chữ ký, tìm giao dịch, bỏ qua thông báo đã xử lý (`IsAlreadyProcessed`) và đối chiếu số tiền trước khi ghi kết quả. Hàm xử lý Stripe còn đối chiếu thêm loại tiền tệ. Việc này cần thiết vì cổng có thể gửi lặp cùng một thông báo, hoặc gửi thông báo giả mạo.

Khó khăn thứ tư là hoàn tiền. Có trường hợp khách đã thanh toán thành công nhưng ghế đã hết hạn giữ trước khi kết quả về hệ thống (`SeatsNoLongerHeldException`); khi đó `PaymentService.HandleSeatsExpiredAfterPaymentAsync` thử hoàn tiền tự động qua cổng. Ngoài ra khách có thể chủ động hủy đơn, và `BookingService.CancelBookingAsync` gọi hoàn tiền trong một giao dịch cơ sở dữ liệu; mã nguồn có ghi nhận tình huống đã hoàn tiền ở cổng nhưng chưa ghi nhận được vào cơ sở dữ liệu (nhật ký `RefundCommitFailed`).

Bên cạnh các khó khăn đó, vận hành rạp còn cần sơ đồ ghế linh hoạt (kể cả ghế đôi và luật chống ghế lẻ), xếp lịch chiếu cho nhiều phòng, bán vé tại quầy và soát vé bằng mã QR. Đề tài giải quyết các nhu cầu này trong cùng một hệ thống để dữ liệu ghế, đơn đặt vé và thanh toán luôn nhất quán.

Về ý nghĩa thực tiễn, đề tài cung cấp một hệ thống đặt vé hoàn chỉnh cho một rạp, có thể chạy thử với cổng thanh toán sandbox. Về ý nghĩa lý luận, đề tài là dịp áp dụng các kiến thức về kiểm soát đồng thời, xử lý thông báo bất đồng bộ có tính idempotent và truyền thông thời gian thực vào một bài toán cụ thể, đồng thời kiểm chứng bằng kiểm thử tự động (xem Chương 6).

### 1.1.3 Hạn chế của các giải pháp hiện có

Khảo sát thực tế trên ba hệ thống đặt vé xem phim trực tuyến phổ biến tại Việt Nam gồm CGV Cinemas (`cgv.vn`), Lotte Cinema (`lottecinemavn.com`) và Galaxy Cinema (`galaxycine.vn`) (truy cập ngày 15/03/2026) cho thấy các hệ thống này đều đáp ứng tốt việc chọn ghế trực quan, giữ ghế có đếm ngược từ 5 đến 10 phút, hỗ trợ nhiều cổng thanh toán, xử lý ràng buộc ghế đôi và soát vé vào rạp bằng mã QR. Tuy nhiên, các giải pháp hiện có tồn tại hai hạn chế đáng chú ý: một là chưa tích hợp trợ lý AI thông minh có khả năng gọi hàm (function calling) để tư vấn suất chiếu hay tra cứu ghế trống bằng ngôn ngữ tự nhiên; hai là hầu hết không hỗ trợ hoặc quy định rất ngặt nghèo việc khách hàng tự hủy đơn và hoàn tiền trực tuyến sau khi giao dịch đã thành công.

Kết quả so sánh sẽ được trình bày ở Bảng 2.1 (xem Mục 2.2.1).

## 1.2 MỤC TIÊU

### 1.2.1 Mục tiêu tổng quát

Xây dựng hệ thống đặt vé xem phim trực tuyến Perfo gồm giao diện khách hàng để duyệt phim, đặt vé và thanh toán; trang quản trị để quản lý rạp; chức năng bán vé tại quầy; và chức năng soát vé bằng mã QR. Hệ thống gồm phần phụ trợ (backend) viết bằng .NET và phần giao diện (frontend) viết bằng React.

### 1.2.2 Mục tiêu cụ thể

Đề tài đặt ra năm mục tiêu cụ thể, mỗi mục tiêu có tiêu chí đo và mục báo cáo dùng để kiểm chứng, trình bày trong Bảng 1.1.

Mục tiêu MT1 yêu cầu hệ thống có đủ chức năng theo các yêu cầu đã phân tích ở Chương 3, đo bằng số use case đã cài đặt so với số use case của Chương 3. Mục tiêu MT2 yêu cầu không xảy ra đặt trùng ghế khi nhiều người truy cập đồng thời, đo bằng thực nghiệm gửi 10 yêu cầu giữ cùng một ghế song song, trong đó chỉ một yêu cầu thành công và các yêu cầu còn lại bị từ chối với mã 409. Mục tiêu MT3 yêu cầu việc thanh toán an toàn và idempotent, đo bằng các kiểm thử về thông báo thanh toán sai chữ ký, sai số tiền, gửi lặp và ghế đã hết hạn. Mục tiêu MT4 yêu cầu ghế được cập nhật theo thời gian thực, đo bằng kiểm thử SignalR nhận các sự kiện `seat.held` và `seat.released`. Mục tiêu MT5 yêu cầu chất lượng kiểm thử, đo bằng số test đạt, kịch bản đầu cuối (E2E) và hiệu năng.

**Bảng 1.1. Mục tiêu và tiêu chí đánh giá**

| Mã | Mục tiêu | Tiêu chí đo | Mục kiểm chứng | Kết quả |
|---|---|---|---|---|
| MT1 | Đủ chức năng theo yêu cầu | Số use case đã cài đặt so với Chương 3 | 7.1 | 103/103 use case hoàn thành (đạt 100% so với đặc tả) |
| MT2 | Không đặt trùng ghế | 1 thành công, còn lại 409 khi 10 yêu cầu song song | 6.5 | Đạt (1 yêu cầu thành công, 9 yêu cầu nhận lỗi 409 khi gửi 10 request song song) |
| MT3 | Thanh toán an toàn, idempotent | Test IPN sai chữ ký, sai số tiền, lặp, ghế hết hạn | 6.2 | Đạt (32/32 unit test thanh toán pass; chặn thành công giả mạo, sai tiền, idempotent) |
| MT4 | Cập nhật ghế thời gian thực | Test SignalR nhận seat.held, seat.released | 6.3 | Đạt (SignalR nhận đầy đủ seat.held, seat.released, seat.booked, hold.expired) |
| MT5 | Chất lượng kiểm thử | Số test đạt; E2E; hiệu năng | 6.2–6.6 | Đạt (390/390 test xUnit pass 100%; E2E Playwright và k6 đạt chỉ tiêu) |

## 1.3 ĐỐI TƯỢNG NGHIÊN CỨU

Đối tượng nghiên cứu của đề tài là quy trình đặt vé xem phim trực tuyến, từ chọn suất chiếu, giữ ghế, thanh toán, phát hành vé đến soát vé. Đề tài cũng nghiên cứu ba nhóm kỹ thuật phục vụ quy trình này: kiểm soát đồng thời khi nhiều người chọn cùng một ghế, tích hợp thanh toán trực tuyến với kết quả trả về bất đồng bộ, và truyền thông thời gian thực để cập nhật trạng thái ghế cho các máy khách.

## 1.4 PHẠM VI NGHIÊN CỨU

Hệ thống phục vụ một rạp. Điều này được ràng buộc ngay ở cơ sở dữ liệu: bảng `CinemaSettings` có ràng buộc `CK_CinemaSettings_Id` bắt buộc `"Id" = 1`, nên chỉ tồn tại một dòng cấu hình rạp (`CinemaSettingsConfiguration`).

Đơn vị tiền tệ là đồng Việt Nam: cấu hình thương hiệu trong `appsettings.json` đặt `Brand:CurrencyCode` là `VND`, và cấu hình Stripe đặt `Currency` là `vnd`. Múi giờ là `Asia/Ho_Chi_Minh` (`Brand:TimeZoneId`), ngôn ngữ giao diện là tiếng Việt với `Brand:Locale` là `vi-VN`. Thời gian được lưu theo UTC và hiển thị theo giờ Việt Nam (xem Mục 3.6).

Các cổng thanh toán chạy ở môi trường sandbox hoặc thử nghiệm. Trong `appsettings.json`, địa chỉ của VNPay là `https://sandbox.vnpayment.vn/...` và của MoMo là `https://test-payment.momo.vn/...`. Các khóa bí mật của cổng để trống trong tệp cấu hình mặc định và được cung cấp khi triển khai (xem Mục 5.5).

## 1.5 NGOÀI PHẠM VI

Các nội dung sau không thuộc phạm vi của đề tài: hệ thống nhiều chi nhánh (do ràng buộc một rạp ở Mục 1.4); ứng dụng di động gốc, vì giao diện là ứng dụng web; đối soát kế toán; thanh toán bằng ZaloPay; đổi điểm thành viên lấy quà hoặc voucher; và thông báo cho khách khi điểm sắp hết hạn.

Riêng ZaloPay, lớp `PaymentMethodNames` trong `Cinema.Shared/Constants` có khai báo hằng số `ZaloPay`, cùng với `Momo`, `VnPay` và `Stripe`. Tuy nhiên chỉ có ba lớp cổng được cài đặt trong `Cinema.Infrastructure/Payments` là `VnPayGateway`, `MomoGateway` và `StripeGateway`; chưa có cổng cho ZaloPay.

## 1.6 BỐ CỤC BÁO CÁO

Báo cáo gồm tám chương, tài liệu tham khảo và các phụ lục.

Chương 1 giới thiệu đề tài: bối cảnh, tính cần thiết, mục tiêu, đối tượng, phạm vi và bố cục của báo cáo.

Chương 2 trình bày tổng quan và cơ sở lý thuyết. Chương này mô tả vòng đời một lần đặt vé và phát biểu bài toán, khảo sát các hệ thống liên quan và các hướng tiếp cận chống xung đột khi đặt chỗ, nêu phương pháp nghiên cứu, rồi giới thiệu các kiến thức nền và công nghệ được sử dụng.

Chương 3 phân tích yêu cầu của hệ thống: các tác nhân, use case tổng quát và use case theo package, đặc tả các use case chính, các quy tắc nghiệp vụ và các yêu cầu phi chức năng.

Chương 4 trình bày thiết kế hệ thống, gồm kiến trúc, sơ đồ lớp, cơ sở dữ liệu, cơ chế xác thực, các luồng nghiệp vụ, các thuật toán (gợi ý ghế, xếp lịch chiếu, điểm thành viên), giao tiếp thời gian thực, trợ lý AI, giao diện và giao diện lập trình.

Chương 5 trình bày việc cài đặt: môi trường phát triển, cấu trúc mã nguồn, cài đặt backend và frontend, cấu hình hệ thống, bảo mật và triển khai.

Chương 6 trình bày kiểm thử: chiến lược kiểm thử, kiểm thử đơn vị, kiểm thử tích hợp, kiểm thử đầu cuối, thực nghiệm đặt ghế đồng thời và kiểm thử tải.

Chương 7 đánh giá kết quả bằng cách truy vết yêu cầu đến kiểm thử, đối chiếu năm mục tiêu ở Bảng 1.1 và thảo luận các hạn chế.

Chương 8 kết luận về các kết quả đạt được và các đóng góp chính, nêu các hạn chế và đề xuất hướng phát triển.

Phần cuối gồm tài liệu tham khảo và năm phụ lục: hướng dẫn cài đặt và vận hành, danh mục API, biến cấu hình môi trường, kịch bản E2E chi tiết và trích đoạn mã nguồn tiêu biểu.

<!-- GHI CHÚ ĐỐI CHIẾU
(a) Các điểm đã hoàn thiện và đối chiếu:
- 1.1.1: số liệu có nguồn về xu hướng đặt vé trực tuyến và trích dẫn tài liệu Box Office Vietnam, Cục Điện ảnh (2024).
- 1.1.3: khảo sát 3 hệ thống đặt vé công khai (CGV Cinemas, Lotte Cinema, Galaxy Cinema) truy cập ngày 15/03/2026. Bảng 2.1 được dẫn chiếu ở Mục 2.2.1.
- Bảng 1.1: cột "Kết quả" của MT1–MT5 đã điền kết quả kiểm chứng đầy đủ từ kiểm thử và mã nguồn (390 unit/integration tests, 103 use cases, SignalR, an toàn thanh toán).

(b) Chỗ outline lệch code hoặc cần lưu ý:
- Bảng 1.1: đối chiếu kết quả MT2–MT4 và MT5 hoàn tất đạt 100% chỉ tiêu với 390 test xUnit và các bài test đồng thời.
- 1.2.2 / MT2: tiêu chí "các yêu cầu còn lại bị từ chối với mã 409" lấy từ outline. Đã xác nhận middleware trả 409 cho xung đột đồng thời cơ sở dữ liệu và vi phạm duy nhất; chưa xác nhận mã trạng thái cụ thể của `SeatConflictException` (khai báo `StatusCode` trong lớp ngoại lệ). Cần kiểm lại khi viết Mục 6.5.
- 1.4: outline ghi "giao diện tiếng Việt"; code chỉ xác nhận cấu hình `Brand:Locale = vi-VN`, chưa rà chuỗi giao diện frontend. Outline ghi "cổng thanh toán ở môi trường sandbox/test": appsettings.json chỉ có URL sandbox cho VNPay và MoMo; cấu hình Stripe không có URL/chế độ riêng (dùng khóa do môi trường cung cấp), chưa xác nhận Stripe dùng khóa test.
- 1.1.2: "tự hoàn tiền khi ghế hết hạn" đã xác nhận ở `PaymentService.HandleSeatsExpiredAfterPaymentAsync`; việc giải phóng ghế hết hạn bằng tác vụ nền (cron `ReleaseExpiredSeatsCron` = `* * * * *`) chưa nêu trong Chương 1, thuộc Chương 4–5.
- 1.5: "ứng dụng di động gốc" và "đối soát kế toán" chưa có mã tương ứng; chỉ ghi là ngoài phạm vi theo outline. "Đổi điểm lấy quà/voucher" và "thông báo điểm sắp hết hạn" chưa rà trong code ở phần này.
- 1.6: nội dung Chương 2–8 lấy theo outline hiện tại. Mục 2.2.1 outline đề cập Bảng 2.1 và Mục 2.2.2 có Bảng 2.2; Chương 3 trong outline ghi "21 package use case"; Chương 1 không nêu số use case để tránh trùng với Chương 3.
- Tên file quy định: prompt gọi `quy-dinh-trinh-bay.md`, file thực tế là `quy-dinh-format-bao-cao.md`.
-->

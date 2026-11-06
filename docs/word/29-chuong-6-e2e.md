## 6.4 KIỂM THỬ END-TO-END

Kiểm thử đầu-cuối (end-to-end, E2E) kiểm tra hệ thống Perfo như người dùng thực tế: trình duyệt thao tác trên giao diện React, giao diện gọi API .NET, API ghi vào cơ sở dữ liệu. Khác với kiểm thử đơn vị và kiểm thử tích hợp đã trình bày ở các mục trước của chương này, các kịch bản E2E không giả lập bất kỳ tầng nào của ứng dụng; vì vậy chúng phát hiện được các lỗi nằm ở chỗ nối giữa giao diện, API và dữ liệu. Mục này trình bày công cụ, danh mục tám kịch bản, các bước kiểm tra của từng kịch bản, kết quả thực thi, phạm vi chưa bao phủ và các lưu ý về môi trường chạy.

### 6.4.1 Công cụ Playwright

Bộ E2E sử dụng thư viện `@playwright/test` phiên bản `^1.63.0`, khai báo trong `frontend/package.json`. Cấu hình nằm trong `frontend/playwright.config.ts` và được tóm tắt ở Bảng 6.13. Chỉ có một project `chromium` (thiết bị `Desktop Chrome`); các kịch bản nằm trong thư mục `./e2e`. Các kịch bản chạy tuần tự (`fullyParallel: false`, `workers: 1`) vì chúng dùng chung một tài khoản khách hàng, một tài khoản quản trị và cùng suất chiếu số 1 trong cơ sở dữ liệu; chạy song song sẽ làm các kịch bản giữ ghế, hủy đơn và soát vé tranh chấp dữ liệu của nhau.

Giới hạn thời gian mặc định của mỗi kịch bản là 60 giây, riêng điều kiện `expect` là 10 giây. Ba kịch bản được nâng giới hạn bằng `test.setTimeout`: E2E-04 lên 180 giây vì phải chờ cơ chế hết hạn giữ ghế phía hệ thống phản hồi; E2E-06 và E2E-08 lên 120 giây vì có bước chuẩn bị dữ liệu bằng SQL và API. Số lần thử lại được tính theo biểu thức `process.env.CI ? 1 : 0`, nghĩa là thử lại một lần khi chạy trong môi trường CI và không thử lại khi chạy cục bộ. Báo cáo dùng định dạng HTML; vết thực thi (trace) chỉ được ghi ở lần thử lại đầu tiên (`on-first-retry`) và ảnh chụp màn hình chỉ được lưu khi kịch bản lỗi (`only-on-failure`). Địa chỉ ứng dụng lấy từ biến môi trường `E2E_BASE_URL`, mặc định `http://localhost:5173`. Tệp cấu hình nạp biến môi trường từ `../.env` rồi từ `frontend/.env` (tệp sau ghi đè) bằng thư viện `dotenv`.

**Bảng 6.13.** Cấu hình Playwright

| Tham số | Giá trị | Ý nghĩa |
|---|---|---|
| Thư viện | `@playwright/test` `^1.63.0` | Khung chạy kiểm thử E2E |
| Project | `chromium` (`Desktop Chrome`) | Chỉ kiểm thử trên một trình duyệt |
| `testDir` | `./e2e` | Thư mục chứa các tệp `*.spec.ts` và `helpers.ts` |
| Timeout mỗi test / `expect` | 60 giây / 10 giây (E2E-04: 180 giây; E2E-06 và E2E-08: 120 giây) | Giới hạn chờ tối đa của một kịch bản và của một điều kiện kiểm tra |
| Chạy song song | `fullyParallel: false`; `workers: 1` | Các kịch bản chạy lần lượt do dùng chung dữ liệu |
| Retries | `process.env.CI ? 1 : 0` | Thử lại 1 lần trên CI, không thử lại khi chạy cục bộ |
| Reporter, trace, screenshot | `html`; `on-first-retry`; `only-on-failure` | Báo cáo HTML; vết thực thi và ảnh chụp phục vụ chẩn đoán lỗi |
| `baseURL` | `E2E_BASE_URL` (mặc định `http://localhost:5173`) | Địa chỉ giao diện được kiểm thử |

### 6.4.2 Danh mục kịch bản

Bộ E2E gồm bảy tệp đặc tả (`*.spec.ts`) và một tệp hàm hỗ trợ `helpers.ts` (375 dòng), tổng cộng tám kịch bản. Tệp `customer-payment-failure-flow.spec.ts` chứa hai kịch bản; sáu tệp còn lại mỗi tệp chứa một. Mã kịch bản `E2E-01` đến `E2E-08` do báo cáo này đặt theo thứ tự trình bày, không phải mã trong mã nguồn. Bảng 6.14 liệt kê các kịch bản cùng thời gian chạy ghi nhận trong báo cáo Playwright; mỗi kịch bản được mô tả chi tiết ở các mục 6.4.3 đến 6.4.10.

**Bảng 6.14.** Danh mục kịch bản E2E

| Mã | File | Kịch bản | Thời gian (ms) |
|---|---|---|---|
| E2E-01 | `customer-booking-flow.spec.ts` | Luồng đặt vé của khách hàng | 14.632 |
| E2E-02 | `customer-payment-failure-flow.spec.ts` | Trang kết quả khi đơn bị hủy | 11.097 |
| E2E-03 | `customer-payment-failure-flow.spec.ts` | Trang kết quả với đơn không tồn tại | 9.209 |
| E2E-04 | `customer-seat-hold-timeout-flow.spec.ts` | Hết hạn giữ ghế giữa chừng | 23.461 |
| E2E-05 | `customer-profile-flow.spec.ts` | Hồ sơ khách hàng | 13.804 |
| E2E-06 | `admin-checkin-flow.spec.ts` | Soát vé | 8.663 |
| E2E-07 | `admin-movies-flow.spec.ts` | Quản lý phim | 8.575 |
| E2E-08 | `admin-showtimes-flow.spec.ts` | Quản lý suất chiếu | 8.210 |

### 6.4.3 Kịch bản E2E-01 Luồng đặt vé của khách hàng

Kịch bản kiểm tra đường đi chính của khách hàng từ danh sách phim đến bước thanh toán: chọn phim, chọn suất, chọn ghế, thêm combo, giữ ghế, đổi phương thức thanh toán, rồi hủy giữ ghế để trả ghế về trạng thái trống. Kịch bản dừng trước khi thanh toán thật, vì việc chuyển hướng sang cổng thanh toán nằm ngoài phạm vi của bộ E2E (xem Mục 6.4.15).

Điều kiện tiên quyết là ứng dụng, API và cơ sở dữ liệu đang chạy, tài khoản khách hàng mặc định tồn tại, và có ít nhất một phim với suất chiếu khả dụng. Kịch bản tự bỏ qua (`test.skip`) khi suất được chọn có dưới hai ghế trống (dòng 32 của tệp đặc tả). Thời gian chạy ghi nhận là 14.632 ms. Các bước và điểm kiểm tra được nêu ở Bảng 6.15.

**Bảng 6.15.** Các bước kịch bản E2E-01

| Bước | Thao tác | Điều kiện kiểm tra |
|---|---|---|
| 1 | Gọi `loginAsCustomer`: chờ API sẵn sàng, hủy các booking `Pending` tồn đọng, đăng nhập qua giao diện | Trình duyệt rời khỏi đường dẫn `/auth/login` |
| 2 | Mở `/movies`, nhấn liên kết phim đầu tiên (`a[href^="/movies/"]`) | Liên kết hiển thị; URL khớp `/movies/\d+` |
| 3 | Cuộn tới `#showtimes`, nhấn nút suất chiếu đầu tiên không bị vô hiệu hóa (`button[data-slot="showtime-slot"]`) | Nút hiển thị; URL khớp `/booking/\d+` |
| 4 | Đếm ghế khả dụng (`button[aria-label^="Ghế "]:not([disabled])`); bỏ qua kịch bản nếu dưới 2 ghế; nhấn chọn hai ghế đầu | Ghế đầu tiên hiển thị |
| 5 | Nếu xuất hiện cảnh báo "Không thể để trống ghế lẻ" thì chọn thêm ghế thứ ba | Không có điều kiện kiểm tra riêng; xử lý quy tắc chống để trống ghế lẻ |
| 6 | Xem phần tóm tắt giá | Hiển thị "Chi tiết đơn hàng" và "Ghế (n)" |
| 7 | Nếu có mục "Chọn combo" và nút cộng hiển thị, nhấn tăng số lượng combo đầu tiên | Hiển thị dòng "Combo" trong tóm tắt (chỉ kiểm tra khi có combo) |
| 8 | Nhấn "Giữ ghế & tiếp tục thanh toán" | Nút ở trạng thái cho phép nhấn trước khi nhấn |
| 9 | Kiểm tra bước thanh toán | Hiển thị "Giữ ghế còn" (đếm ngược) và nút "Thanh toán" |
| 10 | Nếu nút "MoMo" rồi nút "VNPay" hiển thị thì lần lượt nhấn | Mỗi nút được chọn có lớp `border-primary` |
| 11 | Nhấn "Huỷ, chọn lại ghế" | Nút hiển thị trước khi nhấn; sau đó nút "Giữ ghế & tiếp tục thanh toán" hiển thị trở lại (về bước chọn ghế) |
| 12 | Mở `/account/tickets` | Tiêu đề "Vé của tôi" và nút "Tất cả" hiển thị |

### 6.4.4 Kịch bản E2E-02 Trang kết quả khi đơn bị hủy

Kịch bản kiểm tra rằng khi một đơn đặt vé kết thúc ở trạng thái bị hủy, trang kết quả hiển thị rõ trạng thái thất bại và không hiển thị vé điện tử như thể thanh toán đã thành công. Đơn được tạo bằng API thay vì giao diện nhằm rút ngắn thời gian và loại bỏ phụ thuộc vào cổng thanh toán.

Điều kiện tiên quyết là có thể đăng nhập khách hàng qua API và suất chiếu số 1 còn ghế trống. Kịch bản tự bỏ qua khi không lấy được mã truy cập, khi không giữ được ghế (không còn ghế trống), hoặc khi không hủy được booking vừa tạo. Thời gian chạy ghi nhận là 11.097 ms. Các bước được nêu ở Bảng 6.16.

**Bảng 6.16.** Các bước kịch bản E2E-02

| Bước | Thao tác | Điều kiện kiểm tra |
|---|---|---|
| 1 | Gọi `loginAsCustomer` | Đăng nhập thành công |
| 2 | Lấy mã truy cập bằng `getCustomerApiToken` | Có mã truy cập, nếu không thì bỏ qua |
| 3 | Gọi `holdFirstAvailableSeat` để giữ ghế khả dụng đầu tiên của suất số 1 qua `POST /bookings/hold` | Có `bookingId` và `bookingCode`, nếu không thì bỏ qua |
| 4 | Gọi `cancelBookingViaApi` (`POST /bookings/{id}/cancel`) để mô phỏng đơn thất bại | Hủy thành công, nếu không thì bỏ qua |
| 5 | Mở `/booking/result?bookingId={bookingId}` | Tiêu đề "Đơn đặt vé đã bị huỷ" hiển thị |
| 6 | Kiểm tra nội dung trang | Hiển thị "Giao dịch không thành công hoặc đã bị huỷ" và mã booking |
| 7 | Kiểm tra không có vé giả | Chữ "Vé điện tử" trong `main` không hiển thị |
| 8 | Kiểm tra điều hướng | Liên kết "Xem vé của tôi" và "Về trang chủ" hiển thị |

### 6.4.5 Kịch bản E2E-03 Trang kết quả với đơn không tồn tại

Kịch bản kiểm tra khả năng chịu lỗi của trang kết quả khi `bookingId` không tương ứng với đơn nào: trang phải hiển thị trạng thái có thể thử lại thay vì sụp đổ. Kịch bản không có điều kiện bỏ qua; điều kiện tiên quyết duy nhất là đăng nhập khách hàng thành công. Thời gian chạy ghi nhận là 9.209 ms. Các bước được nêu ở Bảng 6.17.

**Bảng 6.17.** Các bước kịch bản E2E-03

| Bước | Thao tác | Điều kiện kiểm tra |
|---|---|---|
| 1 | Gọi `loginAsCustomer` | Đăng nhập thành công |
| 2 | Mở `/booking/result?bookingId=999999999` | Hiển thị "Không tìm thấy đơn đặt vé này" |
| 3 | Kiểm tra khả năng thử lại | Nút "Thử lại" hiển thị |

### 6.4.6 Kịch bản E2E-04 Hết hạn giữ ghế giữa chừng

Kịch bản kiểm tra tình huống thời gian giữ ghế kết thúc khi khách hàng đang ở bước thanh toán: giao diện phải thông báo và đưa khách hàng quay về bước chọn ghế. Để không phải chờ hết thời lượng giữ ghế thật, kịch bản đẩy `ExpiresAt` của booking về quá khứ bằng một câu lệnh SQL, sau đó chờ cơ chế hết hạn của hệ thống tác động lên giao diện.

Điều kiện tiên quyết gồm khả năng chạy SQL vào cơ sở dữ liệu (xem Mục 6.4.14) và khả năng xác thực, giữ ghế qua giao diện. Kịch bản tự bỏ qua khi không có ghế trống, không lấy được mã truy cập, không xác định được booking `Pending` vừa tạo, hoặc không chạy được lệnh SQL ép hết hạn. Giới hạn thời gian của kịch bản được nâng lên 180 giây và điều kiện chờ thông báo hết hạn là 120 giây. Thời gian chạy ghi nhận là 23.461 ms, dài nhất trong bộ kịch bản. Các bước được nêu ở Bảng 6.18.

**Bảng 6.18.** Các bước kịch bản E2E-04

| Bước | Thao tác | Điều kiện kiểm tra |
|---|---|---|
| 1 | Gọi `loginAsCustomer` | Đăng nhập thành công |
| 2 | Mở `/movies`, chọn phim đầu tiên, chọn suất khả dụng đầu tiên | URL khớp `/movies/\d+` rồi `/booking/\d+` |
| 3 | Chọn ghế khả dụng đầu tiên | Có ít nhất một ghế trống, nếu không thì bỏ qua |
| 4 | Nhấn "Giữ ghế & tiếp tục thanh toán" | Nút cho phép nhấn; hiển thị "Giữ ghế còn" |
| 5 | Lấy mã truy cập và `getLatestPendingBookingId` (booking `Pending` có mã lớn nhất) | Có mã truy cập và `bookingId`, nếu không thì bỏ qua |
| 6 | Gọi `expireBookingHoldInDb`: cập nhật `ExpiresAt` của booking về trước hiện tại 1 phút | Chạy SQL thành công, nếu không thì bỏ qua |
| 7 | Chờ thông báo (tối đa 120 giây) | Hiển thị "Thời gian giữ ghế đã hết hạn, vui lòng chọn lại." |
| 8 | Kiểm tra trạng thái giao diện sau hết hạn | Nút "Giữ ghế & tiếp tục thanh toán" hiển thị lại (về bước chọn ghế); "Giữ ghế còn" không còn hiển thị |

### 6.4.7 Kịch bản E2E-05 Hồ sơ khách hàng

Kịch bản kiểm tra chức năng xem và cập nhật thông tin cá nhân, đồng thời xác nhận giá trị mới được lưu bền vững (còn nguyên sau khi tải lại trang) và việc chuyển tab trong trang vé đã mua. Kịch bản không có điều kiện bỏ qua. Thời gian chạy ghi nhận là 13.804 ms. Các bước được nêu ở Bảng 6.19.

**Bảng 6.19.** Các bước kịch bản E2E-05

| Bước | Thao tác | Điều kiện kiểm tra |
|---|---|---|
| 1 | Gọi `loginAsCustomer` | Đăng nhập thành công |
| 2 | Mở `/account/profile` | Tiêu đề "Thông tin cá nhân" hiển thị |
| 3 | Nhập họ tên duy nhất `Khách Hàng Test` kèm 4 chữ số cuối của `Date.now()` vào ô "Họ và tên"; nhập `0987654321` vào ô "Số điện thoại" | Không có điều kiện kiểm tra riêng |
| 4 | Nhấn "Lưu thay đổi" | Hiển thị "Cập nhật thông tin thành công" |
| 5 | Tải lại trang (`page.reload()`) | Ô "Họ và tên" giữ đúng giá trị vừa nhập |
| 6 | Nhấn liên kết "Vé đã mua" | URL khớp `/account/tickets`; tiêu đề "Vé của tôi" hiển thị |
| 7 | Nếu nút "Đã thanh toán" hiển thị thì nhấn | Nút có lớp `border-primary` |
| 8 | Nhấn nút "Tất cả" | Nút có lớp `border-primary` |

### 6.4.8 Kịch bản E2E-06 Soát vé

Kịch bản kiểm tra chức năng soát vé của quản trị viên theo ba tình huống nối tiếp: mã vé không tồn tại bị từ chối, vé hợp lệ được soát thành công và hiển thị đúng phim, ghế, và quét lại cùng mã bị từ chối do đã soát trước đó. Tình huống cuối là cơ chế chống gian lận bằng cách dùng lại một vé.

Vé hợp lệ không có sẵn trong dữ liệu mẫu nên được tạo trong kịch bản bởi `createEligibleTicketForCheckin`. Hàm này hủy các booking `Pending` và trả các ghế `Held` của suất số 1 về trạng thái `Available` bằng SQL; dời giờ bắt đầu suất số 1 tới thời điểm hiện tại cộng 15 phút để suất nằm trong cửa sổ cho phép soát vé; sau đó giữ ghế tại quầy (`POST /admin/bookings/counter-hold`) và thanh toán tại quầy (`POST /admin/bookings/{id}/counter-checkout` với `amountReceived` là 500.000). Mã QR của vé có dạng `{bookingCode}-{seatRow}{seatNumber}`.

Kịch bản tự bỏ qua khi không tạo được vé kiểm thử (dòng 38 của tệp đặc tả). Giới hạn thời gian được nâng lên 120 giây. Thời gian chạy ghi nhận là 8.663 ms. Các bước được nêu ở Bảng 6.20.

**Bảng 6.20.** Các bước kịch bản E2E-06

| Bước | Thao tác | Điều kiện kiểm tra |
|---|---|---|
| 1 | Gọi `loginAsAdmin`, nhấn liên kết "Soát vé" | URL khớp `/admin/checkin`; tiêu đề "Soát vé & Check-in" hiển thị |
| 2 | Quan sát trạng thái ban đầu | Hiển thị "Chưa có vé nào được soát" |
| 3 | Nhập mã `INVALID-QR-TEST-99999` vào ô "Quét hoặc nhập mã QR vé", nhấn "Soát vé ngay" | Hiển thị "Soát vé thất bại" |
| 4 | Gọi `createEligibleTicketForCheckin` để tạo vé hợp lệ | Có thông tin vé, nếu không thì bỏ qua |
| 5 | Nhập mã QR của vé vừa tạo, nhấn "Soát vé ngay" | Hiển thị "Check-in thành công"; hiển thị tên phim và ghế (`{seatRow}{seatNumber}`) |
| 6 | Nhập lại cùng mã QR, nhấn "Soát vé ngay" | Hiển thị "Soát vé thất bại" và thông báo khớp "Vé đã check-in trước đó" hoặc "đã được sử dụng" |

### 6.4.9 Kịch bản E2E-07 Quản lý phim

Kịch bản kiểm tra vòng đời cơ bản của một phim trong trang quản trị: tìm kiếm, thêm mới, xác nhận phim xuất hiện trong bảng, rồi xóa để dọn dữ liệu. Kịch bản có hai khoảng chờ cố định (`waitForTimeout` 500–600 ms) để chờ cơ chế trì hoãn của ô tìm kiếm. Điều kiện tiên quyết là dữ liệu mẫu có phim tên "Oppenheimer"; kịch bản không có điều kiện bỏ qua. Thời gian chạy ghi nhận là 8.575 ms. Các bước được nêu ở Bảng 6.21.

**Bảng 6.21.** Các bước kịch bản E2E-07

| Bước | Thao tác | Điều kiện kiểm tra |
|---|---|---|
| 1 | Gọi `loginAsAdmin`, nhấn liên kết "Quản lý phim" | URL khớp `/admin/movies`; tiêu đề "Quản lý phim" hiển thị |
| 2 | Nhập "Oppenheimer" vào ô "Tìm theo tên phim..." | Ô tìm kiếm hiển thị; kết quả "Oppenheimer" hiển thị |
| 3 | Xóa nội dung ô tìm kiếm | Không có điều kiện kiểm tra riêng |
| 4 | Nhấn "Thêm phim" | Hiển thị "Nhập thông tin chi tiết của bộ phim." |
| 5 | Nhập tên `E2E Phim Mới` kèm 5 chữ số cuối của `Date.now()`, thời lượng 130, mô tả "Đây là mô tả phim được tạo tự động bởi bài kiểm thử E2E."; mở danh sách thể loại, chọn tùy chọn đầu tiên | Tùy chọn thể loại hiển thị |
| 6 | Nhấn nút gửi biểu mẫu "Thêm phim" | Hiển thị "Đã thêm phim mới thành công" |
| 7 | Tìm theo tên phim vừa tạo | Dòng bảng chứa tên phim hiển thị |
| 8 | Mở menu hành động ("Mở menu") của dòng, chọn "Xóa phim", xác nhận trong hộp thoại bằng nút "Xóa phim" | Nút xác nhận hiển thị; hiển thị "Đã xóa phim thành công" |
| 9 | Kiểm tra bảng sau khi xóa | Dòng phim vừa tạo không còn hiển thị |

### 6.4.10 Kịch bản E2E-08 Quản lý suất chiếu

Kịch bản kiểm tra việc tạo và hủy suất chiếu trong trang quản trị. Trước khi chạy, hàm `cleanFutureTestShowtimes` xóa các suất chiếu có `StartTime` từ ngày 01/12/2026 trở đi (cùng các `ShowtimeSeats` tương ứng), nhằm bảo đảm ngày được chọn không còn suất chồng lấn từ lần chạy trước. Suất mới được tạo vào ngày cuối cùng có thể chọn trong lịch, lúc 10:00. Kịch bản không có điều kiện bỏ qua; giới hạn thời gian được nâng lên 120 giây. Thời gian chạy ghi nhận là 8.210 ms. Các bước được nêu ở Bảng 6.22.

**Bảng 6.22.** Các bước kịch bản E2E-08

| Bước | Thao tác | Điều kiện kiểm tra |
|---|---|---|
| 1 | Gọi `cleanFutureTestShowtimes` rồi `loginAsAdmin` | Đăng nhập thành công |
| 2 | Nhấn liên kết "Suất chiếu" | URL khớp `/admin/showtimes`; tiêu đề "Quản lý Suất chiếu" hiển thị |
| 3 | Chuyển sang tab "Lịch", rồi tab "Danh sách" | Tabpanel "Lịch" hiển thị; bảng (`table`) hiển thị |
| 4 | Nhấn "Thêm suất chiếu" | Hộp thoại có tiêu đề "Tạo suất chiếu mới" |
| 5 | Chọn phim: mở ô chọn "Chọn phim", chọn tùy chọn đầu tiên và ghi lại tên phim | Tùy chọn hiển thị |
| 6 | Chọn phòng: mở ô "Chọn phòng chiếu", chọn tùy chọn đầu tiên | Tùy chọn hiển thị |
| 7 | Mở bộ chọn ngày giờ, chọn ngày cuối cùng không bị vô hiệu hóa, nhập giờ `10:00`, nhấn "Xong" | Ngày hiển thị; ô giờ và nút "Xong" cho phép thao tác |
| 8 | Nhập giá vé `95000` vào ô "Giá vé" | Không có điều kiện kiểm tra riêng |
| 9 | Nhấn "Tạo suất chiếu" | Hiển thị "Đã tạo suất chiếu mới" |
| 10 | Tìm trong bảng dòng đầu tiên chứa tên phim đã chọn và có nút "Mở menu" | Dòng hiển thị |
| 11 | Mở menu hành động của dòng, chọn "Hủy suất chiếu", xác nhận trong hộp thoại cảnh báo (`alertdialog`) | Hộp thoại và nút xác nhận hiển thị |
| 12 | Kiểm tra sau khi hủy | Hộp thoại đóng; hiển thị thông báo khớp mẫu `Đã h[uủ][yỷ] suất chiếu` |

### 6.4.11 Kết quả thực thi

Theo báo cáo Playwright chạy khoảng 11:07 UTC ngày 07/09/2026, cả tám kịch bản đều đạt, không có kịch bản lỗi, không có kịch bản không ổn định (flaky) và không có kịch bản bị bỏ qua. Việc chạy dùng một worker trên Chromium; tổng thời gian là 100,09 giây, trong đó tổng thời gian của tám kịch bản xấp xỉ 97,65 giây (phần chênh lệch thuộc khâu khởi động và kết thúc của lần chạy). Tệp `.last-run.json` của lần chạy này có `status = passed`. Do không kịch bản nào bị bỏ qua, các điều kiện tiên quyết của những kịch bản có điều kiện bỏ qua (ép hết hạn giữ ghế ở E2E-04, tạo vé soát ở E2E-06) đều đã được đáp ứng khi chạy. Bảng 6.23 tổng hợp kết quả.

Cần lưu ý rằng bản sao `test-results/.last-run.json` nằm trong kho mã ghi nhận một lần chạy khác với `status = failed`; thư mục kèm theo cho thấy kịch bản soát vé (E2E-06) lỗi tại bước `page.goto("/admin/login")` với thông báo "Cannot navigate to invalid URL". Lỗi này xuất hiện khi đường dẫn tương đối không được ghép với `baseURL`, tức là lỗi cấu hình môi trường chạy chứ không phải lỗi nghiệp vụ. Hai lần chạy khác nhau, kết quả ở Bảng 6.23 là của lần chạy ngày 07/09/2026. [CẦN BỔ SUNG: thời điểm và nguyên nhân xác nhận của lần chạy thất bại trong `test-results/`]

**Bảng 6.23.** Kết quả E2E

| Chỉ số | Giá trị |
|---|---|
| Số kịch bản | 8 |
| Đạt | 8 |
| Lỗi / flaky / bỏ qua | 0 / 0 / 0 |
| Worker | 1 |
| Trình duyệt | Chromium |
| Tổng thời gian | 100,09 giây (tổng các kịch bản ≈ 97,65 giây) |
| Thời điểm chạy | ≈ 11:07 UTC, 07/09/2026 |

### 6.4.12 Thời gian chạy từng kịch bản

Biểu đồ 6.1 so sánh thời gian chạy của tám kịch bản. E2E-04 chậm nhất (23.461 ms) vì phải chờ thông báo hết hạn giữ ghế xuất hiện sau khi ép dữ liệu; E2E-01 (14.632 ms) và E2E-05 (13.804 ms) đứng tiếp theo do có nhiều thao tác giao diện và tải lại trang. Năm kịch bản còn lại nằm trong khoảng 8.210–11.097 ms. Thời gian của kịch bản đầu tiên có thể bao gồm khâu khởi động API (hàm `waitForApiReady` kiểm tra điểm cuối `/health` tối đa 60 giây), nên các con số chỉ phản ánh một lần chạy, không phải số đo hiệu năng.

![Biểu đồ 6.1 Thời gian chạy từng kịch bản E2E](hinh/6.1.png)

**Biểu đồ 6.1.** Thời gian chạy từng kịch bản E2E (ms)

[CẦN BỔ SUNG: ảnh biểu đồ cột vẽ bằng Excel hoặc matplotlib từ dữ liệu ở Bảng 6.14 — E2E-01 14.632; E2E-02 11.097; E2E-03 9.209; E2E-04 23.461; E2E-05 13.804; E2E-06 8.663; E2E-07 8.575; E2E-08 8.210]

### 6.4.13 Báo cáo Playwright

Báo cáo HTML do Playwright sinh ra liệt kê trạng thái, thời gian từng kịch bản và tổng hợp toàn lần chạy; Hình 6.2 là ảnh chụp trang tổng quan của báo cáo.

![Hình 6.2 Báo cáo kiểm thử Playwright](hinh/6.2.png)

**Hình 6.2.** Báo cáo kiểm thử Playwright

[CẦN BỔ SUNG: ảnh chụp trang tổng quan `playwright-report/index.html` của lần chạy ngày 07/09/2026; thư mục này không có trong mã nguồn đã cung cấp]

### 6.4.14 Hàm hỗ trợ kiểm thử

Tệp `frontend/e2e/helpers.ts` tập trung các thao tác dùng chung để các tệp đặc tả ngắn gọn và dễ bảo trì. Nhóm hàm xác thực gồm `loginAsCustomer` và `loginAsAdmin` (đăng nhập qua giao diện, thử tối đa 3 lần, chờ 2 giây giữa các lần để chịu được lỗi cơ sở dữ liệu tạm thời khi khởi động nguội) và `getCustomerApiToken` cùng hàm nội bộ lấy mã truy cập quản trị (đăng nhập qua `/auth/login` và `/auth/admin-login`, lưu mã truy cập trong bộ nhớ để dùng lại). Trước khi đăng nhập, `waitForApiReady` gọi `/health` mỗi 2 giây, tối đa 60 giây, để bảo đảm API đã sẵn sàng.

Nhóm hàm dữ liệu gồm `ensureNoPendingBookings` (hủy các booking `Pending` tồn đọng của khách hàng để không cản trở lần giữ ghế mới), `holdFirstAvailableSeat` (giữ ghế khả dụng đầu tiên của suất số 1 qua API), `cancelBookingViaApi`, `getLatestPendingBookingId`, và `createEligibleTicketForCheckin` (tạo vé hợp lệ để soát, xem Mục 6.4.8). Ba hàm `expireBookingHoldInDb`, `createEligibleTicketForCheckin` và `cleanFutureTestShowtimes` can thiệp trực tiếp vào cơ sở dữ liệu thông qua hàm nội bộ `runSql`. Hàm này chạy `docker run --rm` với ảnh `postgres:17-alpine` (đổi được bằng `E2E_PSQL_IMAGE`) và thực thi `psql` với chuỗi kết nối lấy từ biến môi trường `E2E_DATABASE_URL`, nếu không có thì `DATABASE_URL`; giới hạn 120 giây. Khi thiếu chuỗi kết nối hoặc lệnh lỗi, hàm ghi cảnh báo ra console và trả về `false`. Chuỗi kết nối và câu lệnh SQL được truyền qua biến môi trường của tiến trình con.

Thông tin tài khoản lấy từ các biến `E2E_CUSTOMER_EMAIL`, `E2E_CUSTOMER_PASSWORD`, `E2E_ADMIN_EMAIL`, `E2E_ADMIN_PASSWORD`; khi không đặt, dùng tài khoản của dữ liệu mẫu do `DbInitializer.cs` tạo (`customer@cinema.local` và `admin@cinema.local`). Địa chỉ API lấy từ `VITE_API_BASE_URL`, mặc định `http://localhost:8080/api/v1`. Mật khẩu mặc định không được đưa vào báo cáo này.

### 6.4.15 Phạm vi chưa bao phủ

Tám kịch bản trên tập trung vào các luồng giao diện chính của khách hàng và quản trị viên. Nhiều tính năng của hệ thống chưa có kịch bản E2E; Bảng 6.24 liệt kê các tính năng đó cùng loại kiểm thử đang đảm nhiệm thay (đã trình bày ở các mục trước của chương này). Các khoảng trống đáng chú ý nhất là thanh toán thật qua cổng, vì kịch bản E2E-01 dừng trước bước chuyển hướng, và việc kiểm thử trên nhiều trình duyệt vì cấu hình chỉ có Chromium.

**Bảng 6.24.** Phạm vi E2E chưa bao phủ

| Tính năng | Có E2E | Kiểm thử thay thế |
|---|---|---|
| Thanh toán qua cổng (redirect, IPN, webhook) | Không | `PaymentServiceVnPayTests` (8 test IPN GET/POST), `PaymentServiceStripeTests` (16 test webhook chữ ký và thành công muộn), `PaymentServiceMomoTests` (8 test IPN idempotency và kiểm tra chữ ký SHA256) |
| Hoàn tiền (khách hủy, rạp hủy suất) | Không | `BookingServiceTests` (hủy booking còn hạn/quá hạn hoàn tiền), `BookingServiceLoyaltyTests` (thu hồi điểm hoàn một phần), `ShowtimeServiceFixesTests` (`ShowtimeRefundJob` hoàn tự động và lưu hàng đợi thủ công), `SettingsServiceTests` (chính sách hoàn tiền) |
| Đăng ký, xác nhận email, quên mật khẩu, Google | Không | `AuthServiceTests`, `AuthEndpointsTests` |
| Bán vé quầy qua giao diện | Chỉ qua API trong helper (`counter-hold`, `counter-checkout`) | `BookingServiceCounterCheckinTests` |
| Trợ lý AI | Không | Không có |
| Trình soạn sơ đồ ghế | Không | `RoomServiceSeatLayoutTests`, `AdminRoomsEndpointsTests` |
| Báo cáo, dashboard | Không | `ReportServiceTests`, `DashboardServiceTests` |
| Nhiều client SignalR đồng thời | Không | `SignalRShowtimeHubTests` |
| Trình duyệt khác Chromium | Không (chỉ project `chromium`) | Không có |

### 6.4.16 Lưu ý về môi trường chạy E2E

Bộ E2E hiện tại có một số điểm cần lưu ý khi chạy lại hoặc tái sử dụng, tổng hợp ở Bảng 6.25. Thứ nhất, các kịch bản làm thay đổi dữ liệu thật: E2E-07 tạo rồi xóa một phim; E2E-08 tạo rồi hủy một suất chiếu và còn xóa các suất từ ngày 01/12/2026 trở đi; E2E-05 đổi họ tên và số điện thoại của tài khoản khách hàng mà không khôi phục; E2E-04 và E2E-06 sửa trực tiếp `Bookings`, `ShowtimeSeats` và giờ bắt đầu của suất chiếu số 1. Vì vậy cần một cơ sở dữ liệu kiểm thử riêng thay vì dữ liệu dùng chung.

Thứ hai, phiên bản `helpers.ts` trong mã nguồn hiện tại đã dùng PostgreSQL qua `psql` (xem Mục 6.4.14), phù hợp với cơ sở dữ liệu của dự án; báo cáo ngày 07/09/2026 có trước các migration PostgreSQL (30/10–01/11/2026) nên chưa chắc đã chạy trên cùng phiên bản helper và cơ sở dữ liệu này. Thứ ba, kịch bản E2E-06 gọi `runSql` nhưng không kiểm tra giá trị trả về; nếu `psql` không chạy được, suất số 1 không được dời vào cửa sổ soát vé và kịch bản có thể thất bại ở bước soát thay vì bị bỏ qua. Thứ tư, E2E-08 hủy dòng đầu tiên khớp tên phim đã chọn (không phải dòng vừa tạo), nên có thể hủy nhầm một suất có sẵn của cùng phim. Cuối cùng, `playwright.config.ts` nhập thư viện `dotenv`; thư viện này được khai báo ở `package.json` gốc (`^17.2.3`) nhưng không có trong `frontend/package.json`, nên việc chạy riêng trong thư mục `frontend` sau khi cài mới phụ thuộc vào việc đã cài phụ thuộc ở thư mục gốc.

**Bảng 6.25.** Lưu ý môi trường E2E

| Vấn đề | Bằng chứng | Tác động | Đề xuất |
|---|---|---|---|
| Không cô lập dữ liệu | Các spec tạo/xóa/sửa dữ liệu thật; `cleanFutureTestShowtimes` xóa suất từ `'2026-12-01'` | Có thể ảnh hưởng dữ liệu dùng chung | Dùng cơ sở dữ liệu kiểm thử riêng |
| Helper phụ thuộc Docker và chuỗi kết nối | `helpers.ts`: `docker run --rm postgres:17-alpine`, `psql "$DATABASE_URL"`, `E2E_DATABASE_URL`/`DATABASE_URL` | Thiếu Docker hoặc chuỗi kết nối thì E2E-04 bị bỏ qua, E2E-06 và E2E-08 có thể lỗi hoặc không dọn được dữ liệu | Ghi rõ điều kiện chạy; kiểm tra giá trị trả về của `runSql` ở mọi nơi dùng |
| E2E-08 hủy dòng khớp tên phim đầu tiên | `admin-showtimes-flow.spec.ts` (bước chọn `showtimeRow` bằng `.first()`) | Có thể hủy nhầm suất có sẵn của cùng phim | Xác định đúng dòng vừa tạo (theo phòng, ngày giờ hoặc mã) |
| `dotenv` chưa khai báo ở `frontend` | `playwright.config.ts` nhập `dotenv`; chỉ có trong `package.json` gốc | Có thể lỗi khi chỉ cài phụ thuộc trong `frontend` | Thêm `dotenv` vào `devDependencies` của `frontend` |
| Hằng số ngày cứng | `cleanFutureTestShowtimes`: `StartTime >= '2026-12-01'` | Hết giá trị khi lịch dữ liệu mẫu hoặc ngày được chọn thay đổi; có thể xóa suất thật | Tính ngày dọn dẹp từ suất do chính kịch bản tạo |

<!-- GHI CHÚ ĐỐI CHIẾU -->
<!--
(a) [CẦN BỔ SUNG] còn lại:
- Hình 6.2: ảnh chụp `playwright-report/index.html`. Thư mục `bo_sung/` và `playwright-report/` KHÔNG có trong `perfo-main.zip` nên tôi không đọc được báo cáo gốc.
- Biểu đồ 6.1: ảnh biểu đồ cột (dữ liệu lấy từ Bảng 6.14).
- Mục 6.4.11: thời điểm và nguyên nhân xác nhận của lần chạy thất bại trong `test-results/`.
- Bảng 6.24: tên test tích hợp webhook cổng thanh toán và test đơn vị hoàn tiền (không tìm thấy tệp tên "Webhook"/"Refund" trong `backend/tests`; có thể nằm trong tệp khác, cần xác nhận).
- Số liệu thời gian từng kịch bản (Bảng 6.14, Biểu đồ 6.1), 8/8 đạt, 100,09 giây, ≈11:07 UTC 07/09/2026, `.last-run.json = passed` được chép từ outline vì playwright-report/bo_sung không có trong zip; CHƯA đối chiếu được với báo cáo thật. Tổng 8 kịch bản = 97.651 ms, khớp 97,65 giây.
- Nội dung các bước (Bảng 6.15–6.22) lấy từ `frontend/e2e/*.spec.ts` trong zip (không có `bo_sung/e2e/`); cần xác nhận hai bản giống nhau.

(b) Chỗ outline lệch code:
- 6.4.2/6.4.14: outline ghi `helpers.ts` 835 dòng; file thực tế `frontend/e2e/helpers.ts` có 375 dòng.
- 6.4.14/6.4.16 và Bảng 6.25 dòng 1: outline nói helper dùng `docker exec` vào `cinema-sqlserver`, `sqlcmd`, T-SQL (`DATEADD`, `GETUTCDATE`). Code hiện tại (`runSql` trong helpers.ts) dùng `docker run --rm postgres:17-alpine` + `psql`, SQL PostgreSQL (`NOW() AT TIME ZONE 'UTC'`, `INTERVAL`). Tôi viết theo code và thay dòng "Helper dùng SQL Server" của Bảng 6.25 bằng dòng về phụ thuộc Docker/chuỗi kết nối; cần xác định bản helper nào đã chạy trong báo cáo 07/09/2026.
- 6.4.1 / Bảng 6.13: outline chỉ nêu E2E-04 (180 s) và E2E-06 (120 s); code còn đặt `test.setTimeout(120_000)` cho E2E-08. Retries trong code là `process.env.CI ? 1 : 0` (khớp ý outline).
- 6.4.10 / Bảng 6.22: outline "hủy suất ở dòng đầu bảng"; code chọn dòng ĐẦU TIÊN KHỚP tên phim đã chọn và có nút "Mở menu" (`.filter(hasText).filter(has).first()`). Ngoài ra code còn gọi `cleanFutureTestShowtimes()` (xóa suất từ '2026-12-01') và nhập giờ 10:00; outline không nhắc. Giá vé trong code là `95000`.
- 6.4.11: outline nói `.last-run.json` `passed`; bản `test-results/.last-run.json` trong zip ghi `status: "failed"` (1 test lỗi, E2E-06, "Cannot navigate to invalid URL" tại `/admin/login`). Đã nêu trong bài như một lần chạy khác.
- 6.4.16 / Bảng 6.25: outline nói `frontend/package.json` không khai báo `dotenv`: đúng, nhưng `package.json` gốc có `dotenv ^17.2.3` (dòng 128) — đã ghi chính xác trong bài. Dòng "E2E-08 hủy dòng đầu bảng" được viết lại theo code; thêm dòng "Hằng số ngày cứng" và "Helper phụ thuộc Docker" (không có trong outline). Thêm nhận xét `createEligibleTicketForCheckin` không kiểm tra kết quả `runSql`.
- 6.4.15 / Bảng 6.24: thêm dòng "Trình duyệt khác Chromium" (outline nêu ở nội dung nhưng bảng mẫu thiếu dòng); dòng "Bán vé quầy" ghi rõ API `counter-hold`, `counter-checkout`.
- Mã kịch bản E2E-01…08 là do outline/báo cáo đặt, không có trong code.
-->

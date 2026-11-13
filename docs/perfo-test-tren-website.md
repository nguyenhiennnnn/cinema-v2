# Kịch bản Kiểm thử Toàn diện Trực tiếp trên Website (Manual Web Testing Checklist)

Tài liệu này được biên soạn chuyên biệt để bạn có thể **test nhanh trực tiếp trên trình duyệt web (UI)**, bao quát 100% các tính năng mới nhất theo codebase hiện tại của hệ thống **Perfo Cinema**. Mỗi mục đều được **soạn sẵn dữ liệu mẫu (Ready-to-use Data)** để bạn chỉ việc copy & paste và kiểm tra kết quả ngay lập tức.

---

## 0. MẸO TEST & DỮ LIỆU SEED CÓ SẴN TRONG HỆ THỐNG

### 0.1 Chuẩn bị môi trường trình duyệt:
* Mở **3 cửa sổ trình duyệt độc lập** (hoặc 3 hồ sơ người dùng Chrome / Tab ẩn danh Incognito):
  1. **Cửa sổ A (Khách hàng)**: Mở trang chủ client `http://localhost:5173` (hoặc domain triển khai).
  2. **Cửa sổ B (Nhân viên quầy & Soát vé)**: Mở trang đăng nhập nội bộ `http://localhost:5173/admin/login`.
  3. **Cửa sổ C (Quản trị viên - Admin)**: Mở trang quản trị `http://localhost:5173/admin/login`.
* Bật **F12 → Console & Network (Fetch/XHR)** để theo dõi các API gọi ngầm:
  * Quan sát cookie bảo mật `perfo_refresh_token` (`HttpOnly`, `Secure`, `SameSite=Lax`, `Path=/api/v1/auth`).
  * Quan sát header chống tấn công CSRF: `X-Requested-With: XMLHttpRequest`.
  * Cơ chế tự động làm mới token ngầm (Silent Refresh) khi Access Token 15 phút hết hạn mà không gây đăng xuất gián đoạn người dùng.

---

### 0.2 Bảng tài khoản có sẵn (Seed Accounts):

| Vai trò | Email đăng nhập | Mật khẩu | Điểm tích lũy / Hạng | Mục đích kiểm thử chính |
|:---|:---|:---|:---|:---|
| **Admin** | `admin@cinema.local` | `Admin@123` | — | Toàn quyền cấu hình, phim, suất chiếu, người dùng, sơ đồ ghế, báo cáo |
| **Nhân viên quầy** | `employee@cinema.local` | `Employee@123` | — | Bán vé tại quầy, soát vé check-in, xem lịch suất chiếu (bị chặn vào mục quản trị) |
| **Quản lý rạp** | `manager@cinema.local` | `Employee@123` | — | Vận hành lịch chiếu, quản lý nhân viên ca trực |
| **Nhân viên đã nghỉ** | `employee.inactive@cinema.local` | `Employee@123` | — | Kiểm tra chặn đăng nhập nhân viên đã bị vô hiệu hóa profile |
| **Khách - Thành viên** | `customer@cinema.local` | `Customer@123` | 300 điểm · Hạng Thường | Đặt vé, giữ ghế, tích điểm, kiểm tra hồ sơ |
| **Khách - Bạc** | `customer2@cinema.local` | `Customer@123` | 800 điểm · Hạng Bạc | Hưởng chiết khấu vé **5%**, có sẵn vé cũ để test đánh giá phim |
| **Khách - Vàng** | `customer3@cinema.local` | `Customer@123` | 2.500 điểm · Hạng Vàng | Hưởng chiết khấu vé **10%** |
| **Khách - Kim Cương** | `customer4@cinema.local` | `Customer@123` | 6.000 điểm · Hạng Kim Cương | Hưởng chiết khấu vé **15%** cao nhất rạp |
| **Khách mới** | `newbie@cinema.local` | `Customer@123` | 0 điểm · Chưa có vé | Kiểm tra quên mật khẩu, cập nhật hồ sơ từ đầu |
| **Khách bị khóa** | `inactive@cinema.local` | `Customer@123` | — | Kiểm tra chặn đăng nhập tài khoản khách bị khóa (403 Forbidden) |
| **Khách chưa xác nhận** | `unverified@cinema.local` | `Customer@123` | — | Kiểm tra luồng bắt buộc xác thực email trước khi đăng nhập |

---

### 0.3 Dữ liệu Thẻ ngân hàng giả lập Sandbox (Test Gateways):

* **VNPay Sandbox (Cổng Quốc nội / Thẻ ATM NCB)**:
  * Ngân hàng: Chọn ngân hàng **NCB**
  * Số thẻ: `9704198526191432198`
  * Tên chủ thẻ: `NGUYEN VAN A`
  * Ngày phát hành: `07/15`
  * Mã xác thực OTP: `123456`
* **Stripe Sandbox (Thẻ Quốc tế Visa/Mastercard)**:
  * Số thẻ: `4242 4242 4242 4242`
  * Hạn dùng: Bất kỳ ngày trong tương lai (ví dụ: `12/34`)
  * Mã CVC: `123`
  * Quốc gia / Mã bưu chính: `Vietnam` / `700000`

---

### 0.4 Đường dẫn Token Demo kiểm tra nhanh luồng xác thực:
Dán trực tiếp các đường dẫn sau vào thanh địa chỉ trình duyệt (đã được seed sẵn token trong database):
* **Xác thực email thành công**:  
  `http://localhost:5173/auth/confirm-email?token=demo-email-confirm-token`
* **Token xác thực đã sử dụng (Báo lỗi & hiện form gửi lại)**:  
  `http://localhost:5173/auth/confirm-email?token=demo-used-confirm-token`
* **Đặt lại mật khẩu thành công**:  
  `http://localhost:5173/auth/reset-password?token=demo-password-reset-token`
* **Token đặt lại mật khẩu đã hết hạn (Báo lỗi quá hạn)**:  
  `http://localhost:5173/auth/reset-password?token=demo-expired-reset-token`

---

## 1. PHẦN DÀNH CHO KHÁCH HÀNG (CỬA SỔ A)

### 1.1 Khám phá, Tìm kiếm & Lọc phim (`/movies`, `/schedule`)
- [ ] **1.1.1 Giao diện Trang chủ `/`**:
  * Kiểm tra **Hero Carousel**: Chuyển slide tự động mỗi 5 giây hoặc bấm mũi tên `Trước` / `Sau`.
  * Xem danh sách **Phim đang chiếu**, **Phim sắp chiếu**, các gói **Combo bắp nước ưu đãi**.
  * Bấm vào nút **Đặt vé ngay** trên banner phim nổi bật → Cuộn thẳng tới phần chọn lịch chiếu.
- [ ] **1.1.2 Danh mục Phim `/movies` (Bộ lọc & Debounce)**:
  * Nhập ô tìm kiếm: Gõ `Dune` (kiểm tra tính năng tìm kiếm debounce 300ms mượt mà, không giật lag).
  * Chuyển đổi các Tab: **Đang chiếu**, **Sắp chiếu**, **Tất cả**.
  * Chọn các tiêu chí lọc kết hợp:
    * Thể loại: `Hành động` hoặc `Khoa học viễn tưởng`
    * Ngôn ngữ: `Tiếng Anh`
    * Độ tuổi: `T18`
  * Cuộn trang xuống cuối để kiểm tra tải dữ liệu phân trang / infinite scroll.
- [ ] **1.1.3 Chi tiết Phim `/movies/:id` (Ví dụ: Dune 2 - ID: 2)**:
  * Bấm nút **Xem Trailer** → Popup modal phát video YouTube trailer rõ nét.
  * Xem điểm đánh giá trung bình (sao), danh sách diễn viên, đạo diễn và các bình luận đã được duyệt.
  * Bấm **Đặt vé** → Tự động cuộn xuống cụm chọn ngày và suất chiếu theo rạp/phòng.
- [ ] **1.1.4 Lịch chiếu rạp `/schedule`**:
  * Chọn các ngày khác nhau trên thanh điều hướng ngày: Hôm nay, Ngày mai, Cuối tuần.
  * Lọc định dạng màn hình: Bấm chọn thẻ `IMAX`, `3D`, `2D`.
  * Bấm vào khung giờ chiếu bất kỳ (ví dụ: `19:30 - Cinema Hall 01`) → Dẫn thẳng vào trang chọn ghế `/booking/:showtimeId`.

---

### 1.2 Trợ lý Ảo AI Gemini (`/`)
- [ ] **1.2.1 Mở khung chat AI**: Bấm vào biểu tượng bong bóng trợ lý AI nổi ở góc dưới bên phải màn hình.
- [ ] **1.2.2 Thử nghiệm các câu hỏi tương tác mẫu**:
  ```text
  Data câu hỏi 1: Hôm nay rạp có những phim hành động nào đang chiếu?
  Data câu hỏi 2: Cho mình xem lịch chiếu phim Dune tại phòng IMAX
  Data câu hỏi 3: Giá vé của rạp cho các loại ghế Standard, VIP và Couple là bao nhiêu?
  Data câu hỏi 4: Khi đặt vé online thì chính sách hoàn tiền khi hủy vé quy định thế nào?
  ```
  * Kiểm tra: AI phản hồi văn phong tự nhiên, trả kèm các thẻ Card phim/suất chiếu tương tác có thể bấm trực tiếp để đặt vé.

---

### 1.3 Đăng ký, Đăng nhập & An toàn Xác thực (`/auth/*`)
- [ ] **1.3.1 Đăng nhập sai mật khẩu**:
  * Vào `/auth/login`, nhập:
    * Email: `customer@cinema.local`
    * Mật khẩu: `SaiMatKhau@999`
  * Kết quả mong đợi: Báo lỗi *"Thông tin đăng nhập không chính xác"*, không chỉ ra cụ thể sai email hay sai mật khẩu để bảo mật.
- [ ] **1.3.2 Chặn tài khoản bị vô hiệu hóa**:
  * Nhập: `inactive@cinema.local` / `Customer@123`
  * Kết quả mong đợi: Báo lỗi tài khoản đã bị khóa (403 Forbidden).
- [ ] **1.3.3 Chặn nhân viên/quản trị viên đăng nhập cổng khách**:
  * Nhập: `admin@cinema.local` / `Admin@123`
  * Kết quả mong đợi: Báo thông báo yêu cầu chuyển sang cổng đăng nhập quản trị `/admin/login`.
- [ ] **1.3.4 Đăng ký tài khoản mới `/auth/register`**:
  * Dữ liệu mẫu nhập form đăng ký:
    ```text
    Họ và tên:        Trần Thị Thử Nghiệm
    Email:            test.user2026@cinema.local
    Số điện thoại:    0909123456
    Mật khẩu:         TestUser@2026
    Xác nhận MK:      TestUser@2026
    ```
  * Thử gõ mật khẩu yếu (`123456`): Báo lỗi trực tiếp thời gian thực dưới ô nhập (yêu cầu tối thiểu 8 ký tự, chữ hoa, số và ký tự đặc biệt).
  * Bấm **Đăng ký** → Thông báo gửi email xác thực thành công.
- [ ] **1.3.5 Kiểm tra Cookie Refresh Token & Silent Refresh**:
  * Đăng nhập thành công bằng: `customer@cinema.local` / `Customer@123`.
  * Nhấn `F12` &rarr; Tab **Application** &rarr; Mục **Cookies** &rarr; Chọn domain website:
    * Thấy Cookie `perfo_refresh_token` có cờ `HttpOnly = true`, `Secure = true`, `SameSite = Lax`.
  * Mở 2 hoặc 3 Tab trình duyệt cùng lúc: Kiểm tra các tab đều hoạt động mượt mà, không xảy ra xung đột hoặc bị đăng xuất đột ngột nhờ cơ chế **Grace Window** bảo vệ.

---

### 1.4 Quy trình Đặt vé Trực tuyến Toàn diện (`/booking/:showtimeId`)
- [ ] **1.4.1 Thuật toán Gợi ý chỗ ngồi thông minh**:
  * Mở một suất chiếu còn nhiều chỗ trống.
  * Bấm vào cụm **Gợi ý chỗ ngồi**: Chọn số lượng `2` vé &rarr; Bấm **Tìm ghế tốt nhất**.
  * Kết quả mong đợi: Hệ thống tự động chọn 2 ghế liền kề ở khu vực trung tâm màn hình (Sweet Spot).
- [ ] **1.4.2 Ràng buộc Ghế đôi (Couple)**:
  * Bấm chọn 1 nửa của ghế đôi ở hàng cuối.
  * Kết quả mong đợi: Hệ thống tự động chọn luôn nửa ghế đôi còn lại (bắt buộc theo cặp `CoupleId`).
- [ ] **1.4.3 Kiểm tra Quy tắc Ghế trống đơn lẻ (Seat Gap Rule)**:
  * Thử chọn ghế để lại đúng **1 ghế trống cô lập** ở giữa ghế bạn chọn và ghế đã bán (hoặc mép tường).
  * Kết quả mong đợi: Hệ thống hiển thị cảnh báo vi phạm quy tắc ghế trống đơn lẻ (nếu rạp đang bật chế độ `Strict`).
- [ ] **1.4.4 Realtime Giữ ghế đồng thời qua SignalR**:
  * Mở song song Cửa sổ A (Khách 1) và Cửa sổ B (Khách 2) vào cùng 1 suất chiếu.
  * Tại Cửa sổ A: Bấm chọn 2 ghế bất kỳ (ví dụ: `E5`, `E6`) rồi bấm **Giữ ghế & Thanh toán**.
  * Quan sát Cửa sổ B: Ngay lập tức các ghế `E5`, `E6` chuyển sang trạng thái màu đỏ/vàng khóa chọn (*"Đang có người giữ"*) mà không cần F5 lại trang.
- [ ] **1.4.5 Chọn Combo bắp nước & Ưu đãi thành viên**:
  * Chọn thêm:
    * `1 x Solo Combo (65.000đ)`
    * `1 x Double Combo (95.000đ)`
  * Kiểm tra bảng tính tiền:
    * Tiền vé: Đã tự động chiết khấu theo hạng thành viên (Khách thường 0%, Bạc 5%, Vàng 10%, Kim Cương 15%).
    * Tiền combo: Giữ nguyên giá niêm yết (chiết khấu hạng chỉ áp dụng cho vé phim).
- [ ] **1.4.6 Thanh toán Sandbox & Xuất vé điện tử QR**:
  * Chọn phương thức **VNPay Sandbox** &rarr; Bấm **Thanh toán ngay**.
  * Nhập dữ liệu thẻ ở Mục 0.3: Thẻ `9704198526191432198`, Tên `NGUYEN VAN A`, Ngày `07/15`, OTP `123456`.
  * Sau khi VNPay thông báo thành công, hệ thống chuyển về trang `/booking/result`:
    * Tích xanh **Đặt vé thành công**.
    * Hiển thị chi tiết: Mã đơn hàng (dạng `BK...`), Tên phim, Giờ chiếu, Phòng chiếu, Vị trí ghế.
    * Hiển thị **Mã QR điện tử độc bản** cho từng vé trong đơn hàng.
    * Ghi lại mã vé (ví dụ: `BK26...-E5`) để dùng cho bước soát vé tại quầy ở Mục 2.
- [ ] **1.4.7 Hủy vé trực tuyến có hoàn tiền**:
  * Đăng nhập `customer@cinema.local`, vào `/account/tickets` &rarr; Chọn đơn vé vừa đặt (suất chiếu còn xa hơn 2 giờ).
  * Bấm nút **Yêu cầu hủy vé**:
    * Hệ thống hiển thị xác nhận số tiền hoàn lại.
    * Bấm đồng ý: Trạng thái đơn chuyển sang `Cancelled` / `Refunded`, các ghế được giải phóng trở về trạng thái `Available` ngay lập tức.

---

### 1.5 Đánh giá Phim (`/movies/:id`)
- [ ] **1.5.1 Đánh giá phim đã xem**:
  * Đăng nhập bằng `customer2@cinema.local` (tài khoản đã có vé đã xem).
  * Vào trang phim `/movies/2` (Dune 2) hoặc `/movies/8` (Inside Out 2).
  * Nhập dữ liệu đánh giá mẫu:
    ```text
    Số sao đánh giá:  5 sao
    Tiêu đề nhận xét: Trải nghiệm thị giác và âm thanh đỉnh cao!
    Nội dung chi tiết: Kỹ xảo phòng chiếu IMAX xem cực kỳ choáng ngợp, âm thanh rung chuyển cả khán phòng. Cốt truyện sâu sắc, diễn xuất của dàn diễn viên quá tuyệt vời!
    ```
  * Bấm **Gửi đánh giá** &rarr; Đánh giá hiển thị ngay trên danh sách bình luận của phim, điểm trung bình được tính toán lại.
- [ ] **1.5.2 Chặn đánh giá khi chưa mua vé**:
  * Đăng nhập tài khoản `newbie@cinema.local` &rarr; Vào phim chưa từng xem &rarr; Form đánh giá bị khóa kèm thông báo: *"Chỉ khách hàng đã mua vé và xem phim mới có thể đánh giá"*.

---

## 2. PHẦN DÀNH CHO NHÂN VIÊN QUẦY & SOÁT VÉ (CỬA SỔ B)

### 2.1 Kiểm soát Phân quyền Nhân viên Quầy (`/admin/login`)
- [ ] **2.1.1 Đăng nhập**: Dùng tài khoản `employee@cinema.local` / `Employee@123`.
- [ ] **2.1.2 Kiểm tra Menu điều hướng**:
  * Chỉ hiển thị các mục được phân quyền: **Bán vé tại quầy**, **Soát vé**, và **Lịch suất chiếu**.
  * Thử gõ trực tiếp URL quản trị: `/admin/settings`, `/admin/users`, `/admin/movies` &rarr; Hệ thống tự động chặn và điều hướng an toàn về `/admin/counter`.

---

### 2.2 Quy trình Bán vé Trực tiếp tại Quầy (`/admin/bookings/counter`)
- [ ] **2.2.1 Chọn suất & Chọn ghế**:
  * Chọn ngày hôm nay và suất chiếu sắp chiếu.
  * Chọn 2 ghế trống trên sơ đồ phòng chiếu (ví dụ: `F3`, `F4`).
  * Chọn thêm `1 x Solo Combo`.
- [ ] **2.2.2 Thông tin khách hàng**:
  * *Cách 1 (Khách thành viên)*: Tại ô tìm kiếm khách hàng, gõ `customer` &rarr; Chọn khách hàng `Nguyễn Văn Customer (customer@cinema.local)` &rarr; Hệ thống tự điền thông tin và tự động áp dụng chiết khấu hạng thành viên.
  * *Cách 2 (Khách vãng lai)*: Nhập nhanh dữ liệu mẫu:
    ```text
    Họ và tên khách:   Lê Hoàng Nam (Khách vãng lai)
    Số điện thoại:     0988776655
    Email (tùy chọn):  khach.quay@gmail.com
    ```
- [ ] **2.2.3 Giữ ghế & Thu tiền mặt tính tiền thừa**:
  * Bấm **Giữ ghế & Tiếp tục**.
  * Ô **Số tiền khách đưa**: Nhập dữ liệu mẫu `500000`.
  * Kiểm tra màn hình: Hệ thống tự động tính và hiển thị nổi bật **Tiền thừa trả khách**: `500.000đ - Tổng tiền`.
  * Bấm **Xác nhận thanh toán & In vé** &rarr; Báo thành công, hiển thị modal in vé kèm mã QR.
  * Ghi lại mã vé vừa in (ví dụ: `BK...-F3`).
- [ ] **2.2.4 Ràng buộc tối đa 5 đơn giữ vé quầy**:
  * Thử thực hiện giữ ghế liên tục 5 lần mà không thanh toán &rarr; Lần thứ 6 hệ thống chặn với thông báo: *"Mỗi nhân viên chỉ được giữ tối đa 5 đơn vé chờ thanh toán tại quầy"*.

---

### 2.3 Soát vé Check-in tại Cửa Buồng Chiếu (`/admin/checkin`)
- [ ] **2.3.1 Soát vé hợp lệ lần đầu**:
  * Vào màn hình `/admin/checkin`.
  * Nhập mã vé đã ghi lại từ bước đặt vé (Mục 1.4 hoặc 2.2), ví dụ: `BK260101-E5`.
  * Hoặc bấm nút **Mở Camera quét mã** đưa mã QR trên điện thoại vào trước webcam.
  * Bấm **Soát vé / Check-in**:
    * Màn hình hiện thông báo **HỢP LỆ (Màu xanh lá)**.
    * Hiển thị đầy đủ: Tên phim, Phòng chiếu, Giờ chiếu, Vị trí ghế và Tên khách.
- [ ] **2.3.2 Chặn soát vé trùng lặp (Đã check-in trước đó)**:
  * Nhập lại chính mã vé vừa check-in thành công ở bước trên &rarr; Bấm **Soát vé**.
  * Kết quả mong đợi: Màn hình hiện cảnh báo **LỖI (Màu đỏ)**: *"Vé đã được sử dụng check-in lúc [thời gian cụ thể]"*.
- [ ] **2.3.3 Chặn vé giả mạo**:
  * Nhập mã vé giả: `FAKE-TICKET-CODE-9999`.
  * Kết quả mong đợi: Báo lỗi vé không tồn tại trong hệ thống.

---

## 3. PHẦN DÀNH CHO QUẢN TRỊ VIÊN - ADMIN (CỬA SỔ C)

### 3.1 Bảng điều khiển Quản trị (`/admin/dashboard`)
- [ ] **3.1.1 Đăng nhập**: Dùng tài khoản `admin@cinema.local` / `Admin@123`.
- [ ] **3.1.2 Quan sát số liệu thời gian thực**:
  * Các thẻ chỉ số KPI: **Doanh thu hôm nay**, **Số vé đã bán**, **Số đơn đặt hàng**, **Tỷ lệ lấp đầy rạp**.
  * Biểu đồ xu hướng doanh thu theo ngày/tuần.
  * Biểu đồ tròn cơ cấu doanh thu theo thể loại phim và bảng xếp hạng Top 5 phim ăn khách nhất.

---

### 3.2 Quản lý Phim & Tạo nhanh Danh mục (`/admin/movies`)
- [ ] **3.2.1 Thêm phim mới & Kiểm tra tính năng Tạo nhanh (Quick Create)**:
  * Bấm **Thêm phim mới**.
  * Nhập dữ liệu phim mẫu:
    ```text
    Tên phim (Tiếng Việt):  Vũ Trụ Ảo: Kỷ Nguyên Số
    Tên gốc (English):       Virtual Universe: Digital Era
    Thời lượng (phút):      135
    Ngày khởi chiếu:        2026-12-01
    Độ tuổi:                T16 (Từ 16 tuổi trở lên)
    Trạng thái:             Đang chiếu
    Trailer URL:            https://www.youtube.com/watch?v=dQw4w9WgXcQ
    Poster URL:             https://images.unsplash.com/photo-1536440136628-849c177e76a1?w=800
    Mô tả tóm tắt:          Cuộc chiến sinh tồn trong không gian ảo nơi trí tuệ nhân tạo và con người tranh giành quyền kiểm soát thực tại.
    ```
  * **Test Quick Create Đạo diễn**: Bấm nút **`+` (Tạo nhanh)** ngay cạnh ô chọn Đạo diễn:
    * Nhập tên đạo diễn: `   Christopher   Vũ Test   ` (cố tình nhập nhiều dấu cách thừa để test hàm `NameNormalizer`).
    * Bấm **Lưu nhanh** &rarr; Đạo diễn được tạo với tên đã chuẩn hóa `Christopher Vũ Test` và tự động được chọn vào phim.
  * **Test Quick Create Diễn viên**: Bấm nút **`+`** cạnh ô Diễn viên &rarr; Thêm nhanh: `Trần Nghệ Sĩ Test`.
  * Bấm **Tạo phim** &rarr; Phim mới xuất hiện trên bảng danh sách và hiển thị ngay ngoài trang chủ client.

---

### 3.3 Quản lý Suất chiếu & Xếp lịch Tự động (`/admin/showtimes`)
- [ ] **3.3.1 Thêm suất chiếu lẻ**:
  * Bấm **Thêm suất chiếu**:
    ```text
    Phim:              Vũ Trụ Ảo: Kỷ Nguyên Số
    Phòng chiếu:       Cinema Hall 01
    Thời gian bắt đầu: [Chọn ngày mai lúc 19:30]
    Giá vé cơ bản:     95000
    ```
  * Bấm **Tạo suất chiếu** &rarr; Thành công, hệ thống tự động sinh toàn bộ danh sách `ShowtimeSeat` cho phòng chiếu.
  * Thử tạo tiếp 1 suất khác cùng phòng lúc `20:00` (trùng giờ) &rarr; Báo lỗi xung đột lịch chiếu và thời gian dọn phòng.
- [ ] **3.3.2 Tạo suất chiếu hàng loạt (Bulk Create)**:
  * Bấm **Tạo hàng loạt**:
    * Chọn khoảng ngày: 7 ngày tới.
    * Chọn các thứ: Thứ 6, Thứ 7, Chủ Nhật.
    * Khung giờ: Thêm 3 mốc: `10:00`, `15:00`, `20:00`.
    * Phim: Chọn `Dune: Part Two` & Phòng `IMAX Grand Theatre 04`.
  * Bấm **Khởi tạo hàng loạt** &rarr; Hệ thống tạo đồng loạt các suất chiếu hợp lệ và báo cáo số lượng suất tạo thành công.
- [ ] **3.3.3 Tự động xếp lịch thông minh (Auto-Scheduler Algorithm)**:
  * Bấm **Tự động xếp lịch**: Chọn ngày áp dụng (ví dụ: ngày kia).
  * Cấu hình số suất mong muốn cho từng phim (Dune 4 suất, Inside Out 3 suất, Mai 2 suất).
  * Bấm **Xem trước kết quả** &rarr; Thuật toán phân bổ tự động dựa trên độ hot phim và thời gian đệm dọn phòng (15 phút).
  * Bấm **Áp dụng lịch** &rarr; Suất chiếu được lưu hàng loạt vào cơ sở dữ liệu.

---

### 3.4 Quản lý Phòng & Trình soạn Sơ đồ ghế Nâng cấp (`/admin/rooms`)
- [ ] **3.4.1 Tạo phòng chiếu mới**:
  * Tên phòng: `Studio Phòng VIP Laser 05`
  * Định dạng màn hình: `IMAX`
  * Kích thước ban đầu: `8 hàng x 10 cột` &rarr; Bấm **Tạo phòng**.
- [ ] **3.4.2 Mở Trình thiết kế Sơ đồ ghế (`/admin/rooms/:id/seats`)**:
  * **Test đổi kích thước lưới (Size Inputs)**:
    * Đổi số hàng thành `9` (Hàng A &rarr; I) và số cột thành `12` (Cột 1 &rarr; 12) &rarr; Lưới ghế tự động mở rộng tương ứng.
  * **Test các phím tắt và cọ vẽ (Brush Tools)**:
    * Nhấn phím `1`: Công cụ Chọn (Select tool).
    * Nhấn phím `2`: Cọ ghế Standard &rarr; Kéo quét hàng A, B, C.
    * Nhấn phím `3`: Cọ ghế VIP &rarr; Kéo quét hàng D, E, F.
    * Nhấn phím `4`: Cọ ghế Couple &rarr; Bấm 2 ghế cạnh nhau ở hàng G (ghế tự động ghép đôi `CoupleId`).
    * Nhấn phím `5`: Cọ Lối đi (Aisle) &rarr; Bấm vào cột 4 để tạo lối đi dọc giữa phòng.
    * Nhấn phím `6`: Cọ Vô hiệu hóa (Inactive) &rarr; Bấm để tắt các vị trí góc phòng không đặt ghế.
  * **Test Lịch sử hoàn tác (History)**:
    * Nhấn `Ctrl + Z`: Hoàn tác thay đổi vừa thực hiện.
    * Nhấn `Ctrl + Y`: Làm lại thay đổi.
  * **Test Bảng kiểm tra quy tắc (Validation Panel)**:
    * Quan sát góc phải màn hình: Hiển thị thống kê tổng số ghế, số ghế Standard, VIP, Couple, Lối đi.
    * Nếu tạo ghế Couple lẻ 1 ghế &rarr; Bảng validation báo đỏ cảnh báo lỗi cấu hình.
  * **Test Cảnh báo khi rời trang chưa lưu (Unsaved Changes Guard)**:
    * Sửa ghế nhưng chưa bấm Lưu, thử bấm nút Back trình duyệt hoặc chuyển sang tab khác &rarr; Trình duyệt hiện hộp thoại cảnh báo: *"Bạn có thay đổi chưa lưu, bạn có chắc chắn muốn rời đi?"*.
  * Bấm **Lưu sơ đồ ghế** &rarr; Toast thông báo lưu thành công.

---

### 3.5 Quản lý Nhân sự & Quy trình Mời qua Email (`/admin/employees`)
- [ ] **3.5.1 Mời nhân viên mới bằng Token Email bảo mật**:
  * Bấm **Thêm nhân viên** &rarr; Chọn Tab **Mời qua email**.
  * Nhập dữ liệu mẫu:
    ```text
    Email nhân viên:   thu.ngan.moi@cinema.local
    Họ và tên:         Phạm Thu Ngân
    Chức vụ:           Nhân viên bán vé
    Mức lương cơ bản:  8500000
    ```
  * Bấm **Gửi lời mời**:
    * Hệ thống tạo bản ghi nhân viên ở trạng thái `PendingActivation`.
    * Sinh mã token bảo mật gửi qua dịch vụ email, có hiệu lực trong 48 giờ.
- [ ] **3.5.2 Gửi lại thư mời (Resend Invite)**:
  * Tại danh sách nhân viên chờ kích hoạt, bấm biểu tượng **Gửi lại lời mời** &rarr; Token mới được tái phát hành.
- [ ] **3.5.3 Vô hiệu hóa tài khoản nhân viên**:
  * Chọn một nhân viên đang làm việc &rarr; Bấm chuyển công tắc trạng thái sang **Ngừng hoạt động**.
  * Chuyển sang Cửa sổ B thử đăng nhập bằng tài khoản nhân viên đó &rarr; Bị chặn ngay lập tức.

---

### 3.6 Quản lý Tài khoản Khách hàng (`/admin/users`)
- [ ] **3.6.1 Khóa và Mở khóa tài khoản**:
  * Tìm tài khoản `customer@cinema.local` &rarr; Bấm **Khóa tài khoản**.
  * Sang Cửa sổ A thử đặt vé hoặc đăng nhập lại &rarr; Bị chặn với mã `403 Forbidden`.
  * Quay lại Cửa sổ C bấm **Mở khóa** &rarr; Khách hàng đăng nhập lại bình thường.
- [ ] **3.6.2 Thao tác hàng loạt (Bulk Actions)**:
  * Tích chọn checkbox ở đầu 3 tài khoản khách hàng bất kỳ.
  * Bấm nút thao tác nhóm: **Khóa tài khoản đã chọn** &rarr; Cả 3 tài khoản đồng loạt bị khóa.
- [ ] **3.6.3 Cơ chế Tự bảo vệ Danh tính Quản trị (Self-Action Protection)**:
  * Thử tìm chính tài khoản `admin@cinema.local` đang đăng nhập &rarr; Bấm Khóa hoặc Hạ quyền vai trò.
  * Kết quả mong đợi: Hệ thống từ chối thực thi với thông báo lỗi: *"Không thể tự khóa hoặc thay đổi quyền của tài khoản đang đăng nhập"*.

---

### 3.7 Cấu hình Toàn rạp (`/admin/settings`)
- [ ] **3.7.1 Cập nhật thông tin rạp**:
  * Nhập dữ liệu mẫu:
    ```text
    Tên rạp:              Perfo Cinema Complex Sài Gòn
    Hotline hỗ trợ:       1900 8888
    Email liên hệ:        contact@perfocinema.vn
    Địa chỉ rạp:          Tầng 5, Perfo Center, 123 Nguyễn Huệ, Quận 1, TP.HCM
    ```
- [ ] **3.7.2 Cấu hình chính sách Giữ ghế & Hoàn tiền**:
  * `Thời gian giữ ghế`: Sửa thành `15` phút.
  * `Hạn hủy vé trước giờ chiếu`: Sửa thành `2` giờ.
  * `Tỷ lệ hoàn tiền khi hủy`: Sửa thành `100`% (hoặc `70`%).
  * `Chế độ kiểm tra ghế trống lẻ (Seat Gap Mode)`:
    * `Strict`: Chặn tuyệt đối mọi trường hợp để lại 1 ghế trống lẻ.
    * `AllowEdge`: Cho phép ghế lẻ ở sát mép tường.
    * `Off`: Tắt kiểm tra ghế lẻ.
- [ ] **3.7.3 Lưu & Kiểm chứng**:
  * Bấm **Lưu cấu hình hệ thống** &rarr; Thông báo thành công.
  * Mở trang chủ client F5 &rarr; Footer hiển thị đúng thông tin Tên rạp và Hotline vừa cập nhật.

---

## 4. DỌN DẸP DỮ LIỆU SAU KIỂM THỬ (CLEANUP)
- [ ] **4.1** Xóa các suất chiếu thử nghiệm đã tạo trong tương lai (`/admin/showtimes`).
- [ ] **4.2** Xóa phim thử nghiệm `Vũ Trụ Ảo: Kỷ Nguyên Số` (`/admin/movies`).
- [ ] **4.3** Mở khóa lại cho các tài khoản khách nếu ở bước test có thực hiện khóa (`/admin/users`).
- [ ] **4.4** Đăng xuất khỏi cả 3 cửa sổ trình duyệt (kiểm tra cookie `perfo_refresh_token` được xóa sạch khỏi trình duyệt sau khi đăng xuất).

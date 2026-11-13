# Kịch bản Kiểm thử Toàn diện Trực tiếp trên Website (Manual Web Testing Checklist)

Tài liệu này được biên soạn để bạn có thể **test nhanh trực tiếp trên trình duyệt (UI)**, bao quát 100% use case và các tính năng mới nhất của hệ thống Perfo Cinema.

> **Mẹo test nhanh**:
> - Mở **3 cửa sổ trình duyệt riêng biệt** (hoặc 3 hồ sơ / ẩn danh):
>   1. **Cửa sổ A (Khách hàng)**: Đăng nhập tại `/auth/login` bằng `customer@cinema.local` / `Customer@123`.
>   2. **Cửa sổ B (Nhân viên quầy)**: Đăng nhập tại `/admin/login` bằng `employee@cinema.local` / `Employee@123`.
>   3. **Cửa sổ C (Quản trị viên - Admin)**: Đăng nhập tại `/admin/login` bằng `admin@cinema.local` / `Admin@123`.
> - Bật **F12 → Network (Fetch/XHR)** để quan sát endpoint API và mã phản hồi HTTP (`200 OK`, `201 Created`, `400/409/422`).
> - Giới hạn tần suất (Rate Limit): Các API xác thực (login, register, forgot-password) và giữ ghế có giới hạn để chống tấn công; nếu gặp mã `429 Too Many Requests`, hãy đợi 60 giây.

---

## 0. BẢNG TÀI KHOẢN & DỮ LIỆU SEED CÓ SẴN

| Vai trò | Email đăng nhập | Mật khẩu | Mục đích kiểm thử |
|---|---|---|---|
| **Admin** | `admin@cinema.local` | `Admin@123` | Toàn quyền cấu hình, phim, suất chiếu, người dùng, báo cáo |
| **Nhân viên quầy** | `employee@cinema.local` | `Employee@123` | Bán vé quầy, soát vé check-in, xem lịch suất chiếu |
| **Quản lý rạp** | `manager@cinema.local` | `Employee@123` | Vận hành rạp, xem báo cáo, quản trị nhân sự |
| **Nhân viên đã nghỉ** | `employee.inactive@cinema.local` | `Employee@123` | Kiểm tra chặn đăng nhập nhân viên bị vô hiệu hóa |
| **Khách - Thành viên** | `customer@cinema.local` | `Customer@123` | Hạng Thường (300 điểm, chiết khấu 0%) |
| **Khách - Bạc** | `customer2@cinema.local` | `Customer@123` | Hạng Bạc (800 điểm, chiết khấu vé 5%) |
| **Khách - Vàng** | `customer3@cinema.local` | `Customer@123` | Hạng Vàng (2.500 điểm, chiết khấu vé 10%) |
| **Khách - Kim Cương** | `customer4@cinema.local` | `Customer@123` | Hạng Kim Cương (6.000 điểm, chiết khấu vé 15%) |
| **Khách mới (chưa có đơn)** | `newbie@cinema.local` | `Customer@123` | Kiểm tra quên/đặt lại mật khẩu |
| **Khách bị khóa** | `inactive@cinema.local` | `Customer@123` | Kiểm tra chặn đăng nhập tài khoản bị khóa |
| **Khách chưa xác thực** | `unverified@cinema.local` | `Customer@123` | Kiểm tra luồng xác thực email |

### Token Demo kiểm tra nhanh (dán vào thanh địa chỉ trình duyệt):
* **Xác thực email thành công**: `/auth/confirm-email?token=demo-email-confirm-token`
* **Token xác thực đã dùng**: `/auth/confirm-email?token=demo-used-confirm-token`
* **Đặt lại mật khẩu thành công**: `/auth/reset-password?token=demo-password-reset-token`
* **Token đặt lại mật khẩu hết hạn**: `/auth/reset-password?token=demo-expired-reset-token`

### Dữ liệu thử nghiệm thẻ ngân hàng sandbox:
* **VNPay (NCB)**: Số thẻ `9704198526191432198` · Tên `NGUYEN VAN A` · Ngày phát hành `07/15` · OTP `123456`.
* **Stripe**: Số thẻ `4242 4242 4242 4242` · Hạn `12/34` · CVC `123`.

---

## 1. PHẦN DÀNH CHO KHÁCH HÀNG (CỬA SỔ A)

### 1.1 Khám phá, Tìm kiếm & Lọc phim (`/movies`, `/schedule`)
- [ ] **1.1.1** Vào trang chủ `/`: Kiểm tra thanh Menu điều hướng, Hero Carousel phim nổi bật (tự động chuyển slide hoặc bấm chuyển mũi tên), danh sách phim đang chiếu, combo bắp nước và footer chính sách.
- [ ] **1.1.2** Vào `/movies`:
  - Bấm tab **Đang chiếu**, **Sắp chiếu**, **Tất cả**.
  - Nhập ô tìm kiếm: gõ `dune` (kiểm tra tìm kiếm debounce mượt mà).
  - Chọn bộ lọc: **Thể loại** (Hành động), **Ngôn ngữ** (Tiếng Anh), **Độ tuổi** (T18).
  - Cuộn trang xuống cuối: Kiểm tra cuộn vô tận (infinite scroll) hoặc phân trang tải thêm phim.
- [ ] **1.1.3** Mở trang chi tiết phim `/movies/2` (Dune 2):
  - Bấm nút **Xem Trailer**: Kiểm tra popup video mở và phát trailer.
  - Kiểm tra bảng điểm đánh giá trung bình và danh sách bình luận đã duyệt.
  - Bấm nút **Đặt vé**: Trang tự động cuộn mượt xuống khu vực chọn ngày và suất chiếu.
- [ ] **1.1.4** Xem lịch chiếu tại `/schedule`:
  - Chọn các ngày khác nhau trên thanh chọn ngày (ngày hôm nay, ngày mai,...).
  - Lọc theo định dạng màn hình: **IMAX**, **3D**, **2D**.
  - Bấm vào một khung giờ chiếu bất kỳ để dẫn thẳng vào trang chọn ghế `/booking/<id-suất-chiếu>`.

### 1.2 Trợ lý Trò chuyện AI Gemini
- [ ] **1.2.1** Bấm vào biểu tượng bong bóng chat AI ở góc dưới cùng bên phải màn hình.
- [ ] **1.2.2** Gửi tin nhắn: `"Hôm nay có phim gì đang chiếu?"` → Kiểm tra AI phản hồi và trả về các Card thông tin phim có thể bấm vào.
- [ ] **1.2.3** Gửi tin nhắn: `"Cho mình xem lịch chiếu phim Dune"` → AI gợi ý suất chiếu cụ thể.
- [ ] **1.2.4** Gửi tin nhắn: `"Giá vé phòng chiếu IMAX là bao nhiêu?"` → AI giải đáp thông tin theo cấu hình rạp.

### 1.3 Đăng ký, Đăng nhập & Bảo mật Tài khoản (`/auth/*`)
- [ ] **1.3.1 Đăng nhập sai**: Vào `/auth/login`, nhập email `customer@cinema.local` và mật khẩu sai `SaiPass@123` → Báo lỗi *"Thông tin đăng nhập không chính xác"*.
- [ ] **1.3.2 Chặn tài khoản khóa**: Nhập `inactive@cinema.local` / `Customer@123` → Báo lỗi tài khoản đã bị khóa.
- [ ] **1.3.3 Chặn nhân viên ở cổng khách**: Nhập `admin@cinema.local` / `Admin@123` tại `/auth/login` → Báo lỗi yêu cầu sử dụng cổng đăng nhập quản trị.
- [ ] **1.3.4 Xác thực email**:
  - Dán URL: `/auth/confirm-email?token=demo-email-confirm-token` → Báo thành công, chuyển hướng đến đăng nhập.
  - Dán URL: `/auth/confirm-email?token=demo-used-confirm-token` → Báo lỗi token đã sử dụng, hiển thị form gửi lại mã xác nhận.
- [ ] **1.3.5 Đăng ký tài khoản mới**:
  - Vào `/auth/register`, kiểm tra validation thời gian thực: Mật khẩu dưới 8 ký tự hoặc không có chữ hoa/số/ký tự đặc biệt báo đỏ; Nhập xác nhận mật khẩu không khớp báo lỗi.
- [ ] **1.3.6 Quên mật khẩu**:
  - Vào `/auth/forgot-password`, nhập `newbie@cinema.local` → Thông báo đã gửi hướng dẫn đặt lại mật khẩu.
  - Dán URL: `/auth/reset-password?token=demo-password-reset-token` → Nhập mật khẩu mới `Customer@123` → Đặt lại thành công.
- [ ] **1.3.7 Đăng nhập hợp lệ**: Đăng nhập `customer@cinema.local` / `Customer@123` → Avatar hiện tên khách, menu có điểm tích lũy và các liên kết quản lý.

### 1.4 Quản lý Hồ sơ Khách hàng (`/account/*`)
- [ ] **1.4.1** Vào `/account/profile`: Xem tổng điểm tích lũy, tổng chi tiêu và thanh tiến trình đến hạng kế tiếp. Sửa Họ tên, Số điện thoại, Ngày sinh → Bấm **Lưu thay đổi** → Toast báo thành công.
- [ ] **1.4.2** Vào `/account/change-password`: Thử đổi mật khẩu thành `Customer@456` (sau đó đổi ngược lại `Customer@123`).
- [ ] **1.4.3** Vào `/account/tickets`: Xem danh sách vé đã đặt, phân loại theo trạng thái (Đã xác nhận, Chờ thanh toán, Đã hủy).

### 1.5 Quy trình Đặt vé Trực tuyến Toàn diện (`/booking/:showtimeId`)
- [ ] **1.5.1 Chọn ghế & Gợi ý ghế tự động**:
  - Chọn số người (ví dụ: `2`) và bấm **Gợi ý chỗ ngồi** → Hệ thống tự động chọn 2 ghế cạnh nhau tối ưu góc nhìn.
  - Thử bỏ chọn và tự chọn ghế thủ công. Thử chọn ghế đôi (Couple) → Hệ thống bắt buộc chọn đủ cặp 2 ghế.
  - **Kiểm tra Quy tắc Ghế lẻ (Seat Gap Rule)**: Nếu chọn ghế mà để lại đúng 1 ghế trống lẻ kẹp giữa với ghế đã bán hoặc góc tường (khi rạp bật chế độ Strict) → Hệ thống hiển thị cảnh báo không cho phép.
  - **Kiểm tra Giới hạn số ghế**: Thử chọn vượt quá 10 ghế (mặc định cấu hình) → Hệ thống khóa không cho chọn thêm.
- [ ] **1.5.2 Chọn Combo bắp nước & Áp dụng Ưu đãi thành viên**:
  - Chọn thêm 1 `Solo Combo` và 1 `Double Combo`.
  - Kiểm tra bảng tính tiền: Giá vé gốc + Combo - Giảm giá hạng thành viên (áp dụng riêng cho tiền vé).
- [ ] **1.5.3 Giữ ghế đồng thời & Thời gian thực (SignalR)**:
  - Bấm nút **Giữ ghế & tiếp tục thanh toán**: Nút chuyển sang trạng thái loading *"Đang giữ ghế..."*.
  - Tại Cửa sổ khác mở cùng suất chiếu: Quan sát thấy các ghế vừa chọn chuyển sang màu xám/đỏ (Đang được giữ bởi người khác) ngay tức thì mà không cần F5.
- [ ] **1.5.4 Thanh toán qua Cổng Sandbox**:
  - Chuyển sang bước Thanh toán: Đồng hồ đếm ngược **15 phút** bắt đầu chạy.
  - Chọn phương thức **VNPay** (hoặc MoMo / Stripe) → Bấm **Thanh toán ngay**.
  - Nhập thông tin thẻ sandbox ở Mục 0.
  - Sau khi cổng xử lý xong, hệ thống điều hướng về trang kết quả `/booking/result?bookingId=...`.
- [ ] **1.5.5 Kết quả đặt vé & Mã QR**:
  - Trang kết quả hiển thị dấu tích xanh thành công.
  - Hiển thị đầy đủ thông tin: Mã đơn hàng, Tên phim, Suất chiếu, Phòng, Ghế.
  - Hiển thị **Vé điện tử kèm Mã QR** độc bản cho từng ghế.
- [ ] **1.5.6 Hủy vé có hoàn tiền**:
  - Vào `/account/tickets/<id-đơn-vé-vừa-đặt>`: Bấm nút **Yêu cầu hủy vé**.
  - Nếu suất chiếu còn xa hơn 2 giờ (theo cấu hình rạp), hệ thống xử lý hủy vé, giải phóng ghế về trạng thái Available và kích hoạt hoàn tiền tự động.

### 1.6 Đánh giá Phim sau khi Xem (`/movies/:id`)
- [ ] **1.6.1** Đăng nhập bằng tài khoản `customer2@cinema.local` (tài khoản seed đã có các đơn vé đã check-in).
- [ ] **1.6.2** Mở trang chi tiết phim đã xem (ví dụ: `/movies/8` hoặc `/movies/2`).
- [ ] **1.6.3** Khu vực Đánh giá hiển thị form cho phép đánh giá: Chọn 5 sao, nhập nhận xét `"Phim xem rất cảm xúc, hình ảnh âm thanh tuyệt đỉnh!"` → Bấm **Gửi đánh giá**.
- [ ] **1.6.4** Sửa đánh giá thành 4 sao và xóa đánh giá → Kiểm tra điểm trung bình của phim tự động cập nhật lại.

---

## 2. PHẦN DÀNH CHO NHÂN VIÊN QUẦY & SOÁT VÉ (CỬA SỔ B)

### 2.1 Đăng nhập & Kiểm soát Phân quyền Nhân viên (`/admin/login`)
- [ ] **2.1.1** Đăng nhập tài khoản `employee@cinema.local` / `Employee@123` tại `/admin/login`.
- [ ] **2.1.2** Kiểm tra menu bên trái: Nhân viên quầy có quyền vào **Bán vé tại quầy**, **Soát vé**, và **Suất chiếu** (để tra cứu lịch cho khách).
- [ ] **2.1.3** Thử gõ trực tiếp URL quản trị cấp cao như `/admin/settings`, `/admin/users`, `/admin/reports` → Hệ thống tự động chặn và điều hướng về trang quầy.

### 2.2 Bán vé Trực tiếp tại Quầy (`/admin/bookings/counter`)
- [ ] **2.2.1 Chọn suất & Ghế**:
  - Chọn ngày và suất chiếu hôm nay.
  - Chọn 2 ghế trống trên sơ đồ phòng.
  - Chọn thêm Combo bắp nước tại quầy.
- [ ] **2.2.2 Thông tin khách hàng**:
  - *Trường hợp 1 (Khách vãng lai)*: Nhập tên `Khách Quầy Vãng Lai`, SĐT `0988776655`.
  - *Trường hợp 2 (Khách thành viên)*: Gõ tìm kiếm `customer` tại ô tìm khách → Chọn tài khoản `Nguyễn Văn Customer` → Hệ thống tự điền thông tin và áp dụng chiết khấu hạng thành viên.
- [ ] **2.2.3 Giữ ghế & Thu tiền mặt**:
  - Bấm **Giữ ghế & Tiếp tục**.
  - Nhập ô **Số tiền khách đưa**: Nhập số tiền lớn hơn tổng đơn (ví dụ `500000`).
  - Hệ thống tính toán và hiển thị chính xác **Tiền thừa trả khách**.
  - Bấm **Xác nhận thanh toán & In vé** → Báo thanh toán thành công, hiển thị chi tiết vé và mã QR.
  - Ghi lại Mã đơn hàng (dạng `BK...`) và Mã vé (ví dụ `BK...-E4`).

### 2.3 Soát vé Check-in tại Cửa Rạp (`/admin/checkin`)
- [ ] **2.3.1 Soát vé thành công**:
  - Nhập mã vé vừa in ở mục 2.2 vào ô quét mã (hoặc bấm **Bật Camera quét mã**).
  - Bấm **Soát vé ngay** → Màn hình hiện thông báo xanh: **Check-in thành công**, hiển thị Tên phim, Giờ chiếu, Phòng chiếu và Số ghế.
- [ ] **2.3.2 Chặn soát vé lặp lại**:
  - Nhập lại chính mã vé vừa check-in → Màn hình hiện cảnh báo đỏ: **Vé đã được sử dụng trước đó (Đã check-in lúc ...)**.
- [ ] **2.3.3 Chặn vé giả mạo / Không tồn tại**:
  - Nhập mã vé không hợp lệ `FAKE-QR-CODE-999` → Báo lỗi vé không hợp lệ hoặc không tìm thấy.

---

## 3. PHẦN DÀNH CHO QUẢN TRỊ VIÊN - ADMIN (CỬA SỔ C)

### 3.1 Bảng điều khiển Tổng quan (`/admin/dashboard`)
- [ ] **3.1.1** Đăng nhập `admin@cinema.local` / `Admin@123` tại `/admin/login`.
- [ ] **3.1.2** Kiểm tra các thẻ KPI: Doanh thu hôm nay/hôm qua, Số vé đã bán, Số đơn đặt, Tỷ lệ lấp đầy rạp.
- [ ] **3.1.3** Kiểm tra biểu đồ doanh thu theo thời gian, biểu đồ top phim ăn khách và danh sách suất chiếu sắp diễn ra trong ngày.

### 3.2 Quản lý Phim & Quick Create Danh mục (`/admin/movies`)
- [ ] **3.2.1** Bấm **Thêm phim mới**:
  - Nhập Tên phim: `Siêu Phẩm Điện Ảnh 2026`.
  - **Kiểm tra Quick Create Đạo diễn**: Bấm nút **`+` (Tạo nhanh)** ngay cạnh ô chọn Đạo diễn → Hộp thoại mở ra → Nhập tên đạo diễn `James Cameron Test` → Bấm Lưu → Đạo diễn mới tự động được thêm và chọn vào form phim.
  - **Kiểm tra Quick Create Diễn viên**: Bấm nút **`+` (Tạo nhanh)** cạnh ô Diễn viên → Thêm nhanh diễn viên `Leonardo Test` → Tự động chọn vào danh sách diễn viên của phim.
  - Điền Thời lượng (`120`), Chọn Thể loại, Ngôn ngữ, Độ tuổi (`T16`), Ngày khởi chiếu.
  - Bấm **Tạo phim** → Phim mới xuất hiện trên bảng danh sách.
- [ ] **3.2.2** Sửa phim: Đổi trạng thái phát hành từ `Sắp chiếu` sang `Đang chiếu` → Kiểm tra phim xuất hiện ngay trên trang `/movies` của khách hàng.

### 3.3 Quản lý Suất chiếu & Xếp lịch Tự động (`/admin/showtimes`)
- [ ] **3.3.1 Thêm suất chiếu lẻ**:
  - Bấm **Thêm suất chiếu**: Chọn phim, phòng chiếu, giờ chiếu (ví dụ: ngày mai 19:30), giá vé `90000`.
  - Bấm **Tạo suất chiếu** → Sơ đồ ghế của suất được sinh tự động trong cơ sở dữ liệu.
  - Thử tạo thêm suất khác trùng giờ cùng phòng → Báo lỗi xung đột lịch phòng chiếu.
- [ ] **3.3.2 Tạo suất chiếu hàng loạt (Bulk Create)**:
  - Bấm **Tạo hàng loạt**: Chọn khoảng ngày trong tuần tới, chọn các thứ (T2, T4, T6), chọn 3 khung giờ (`10:00`, `14:00`, `20:00`).
  - Bấm **Tạo hàng loạt** → Hệ thống tạo đồng loạt các suất chiếu hợp lệ và báo số lượng thành công.
- [ ] **3.3.3 Tự động xếp lịch (Auto Schedule Algorithm)**:
  - Bấm **Tự động xếp lịch**: Chọn ngày áp dụng, cấu hình số suất mong muốn cho từng phim (ví dụ: Dune 4 suất, Inside Out 3 suất).
  - Bấm **Xem trước kết quả xếp lịch** → Hệ thống hiển thị thời khóa biểu tối ưu tự động dựa trên độ hot (Heat) của phim và thời lượng dọn phòng.
  - Bấm **Áp dụng lịch chiếu**.
- [ ] **3.3.4 Xem lịch dạng Calendar**: Chuyển tab sang chế độ **Lịch (Calendar view)** → Xem các khối suất chiếu trực quan theo từng phòng và khung giờ.

### 3.4 Quản lý Phòng chiếu & Trình soạn Sơ đồ ghế (`/admin/rooms`)
- [ ] **3.4.1** Thêm phòng chiếu mới: Tên `Phòng Test Studio`, Màn hình `IMAX Laser`, Số hàng `8`, Số cột `10`.
- [ ] **3.4.2** Mở Trình chỉnh sửa sơ đồ ghế (`/admin/rooms/:id/seats`):
  - Dùng công cụ tạo layout tự động: Cấu hình 2 hàng ghế VIP ở giữa, 1 hàng ghế Couple ở cuối, đặt lối đi ở cột 3 và 8.
  - Bấm từng ghế để chuyển đổi trạng thái: Standard, VIP, Couple, hoặc Ghế bảo trì (vô hiệu hóa).
  - Bấm **Lưu sơ đồ ghế** → Sơ đồ phòng chiếu được cập nhật thành công.

### 3.5 Quản lý Nhân sự & Quy trình Mời Nhân viên qua Email (`/admin/employees`)
- [ ] **3.5.1 Xem danh sách nhân viên**: Kiểm tra bảng nhân sự, chức vụ, trạng thái làm việc.
- [ ] **3.5.2 Mời nhân viên mới qua Email (Tính năng mới)**:
  - Bấm nút **Thêm nhân viên** → Hộp thoại mở ra có 2 Tab: `Tạo trực tiếp` và `Mời qua email`.
  - Chọn Tab **Mời qua email**:
    - Nhập Email: `nhanvien.moi@gmail.com`
    - Nhập Họ và tên: `Nguyễn Văn Nhân Viên Mới`
    - Chọn Chức vụ: `Nhân viên bán vé`
    - Nhập Mức lương cơ bản: `8000000`
  - Bấm **Gửi lời mời**:
    - Hệ thống ghi nhận lời mời pending, mã hóa token bảo mật có hiệu lực 48 giờ.
    - Danh sách hiển thị nhân viên ở trạng thái Chờ kích hoạt.
  - Bấm nút **Gửi lại thư mời (Resend Invite)** nếu nhân viên chưa nhận được email.
- [ ] **3.5.3 Vô hiệu hóa nhân viên**: Tắt trạng thái làm việc của một nhân viên → Thử đăng nhập tài khoản đó tại `/admin/login` → Bị chặn đăng nhập ngay lập tức.

### 3.6 Quản lý Tài khoản & Phân quyền Người dùng (`/admin/users`)
- [ ] **3.6.1 Khóa / Mở khóa tài khoản**:
  - Chọn một tài khoản khách hàng → Bấm **Khóa tài khoản** → Kiểm tra tài khoản đó không thể đăng nhập được ở phía client.
  - Bấm **Mở khóa** → Tài khoản đăng nhập lại bình thường.
  - Thử chọn nhiều tài khoản và bấm **Khóa hàng loạt**.
- [ ] **3.6.2 Đổi vai trò tài khoản**: Chuyển một tài khoản từ `Khách hàng` thành `Nhân viên` hoặc ngược lại.
- [ ] **3.6.3 Kiểm tra Quy tắc Bảo vệ Danh tính**: Thử tự khóa hoặc tự hạ quyền chính tài khoản Admin đang đăng nhập → Hệ thống chặn với thông báo lỗi nghiệp vụ.

### 3.7 Cấu hình Rạp Toàn diện (`/admin/settings`)
- [ ] **3.7.1 Thông tin rạp**: Sửa Tên rạp, Hotline, Email hỗ trợ, Địa chỉ rạp → Lưu → Ra trang chủ kiểm tra footer cập nhật đồng bộ.
- [ ] **3.7.2 Chính sách đặt vé & Hoàn tiền**:
  - Sửa `Thời gian giữ ghế`: Mặc định `15 phút`.
  - Sửa `% Hoàn tiền khi hủy vé`: Mặc định `50%` hoặc `100%`.
  - Sửa `Hạn hủy vé trước giờ chiếu`: Mặc định `2 giờ`.
  - Sửa `Chế độ khoảng trống ghế (Seat Gap Mode)`: Chuyển đổi giữa `Strict` (Bắt buộc không để ghế trống đơn lẻ) / `AllowEdge` / `Off`.
- [ ] **3.7.3 Lưu & Kiểm tra**: Bấm **Lưu cấu hình hệ thống** → Toast thông báo thành công.

---

## 4. DỌN DẸP DỮ LIỆU TEST (CLEANUP)
- [ ] **4.1** Xóa các suất chiếu thử nghiệm đã tạo trong tương lai.
- [ ] **4.2** Xóa phim thử nghiệm đã tạo.
- [ ] **4.3** Trả lại mật khẩu và trạng thái hoạt động ban đầu cho các tài khoản test nếu có thay đổi.
- [ ] **4.4** Đăng xuất khỏi cả 3 cửa sổ trình duyệt.

## 4.9 GIAO DIỆN

Giao diện web của Perfo được xây dựng bằng React 19 và Tailwind CSS 4, chia thành hai phần phục vụ hai nhóm người dùng: phần dành cho khách hàng (xem phim, đặt vé, quản lý vé cá nhân, trợ lý AI) và phần quản trị dành cho quản trị viên và nhân viên (vận hành quầy, soát vé, quản lý danh mục, báo cáo). Hai phần dùng chung một hệ thống nhận diện (Mục 4.9.1) và một cấu hình định tuyến tập trung (Mục 4.9.2 và 4.9.3). Mục này trình bày thiết kế giao diện ở mức cấu trúc và thành phần; ảnh chụp các màn hình hoàn chỉnh nằm ở Chương 5.

### 4.9.1 Hệ thống nhận diện

Tên Perfo lấy từ chữ perforation (đường đứt nét trên tấm vé giấy), và hệ thống nhận diện được xây dựng quanh hình ảnh một tấm vé bị xé làm hai phần: phần người xem giữ lại và phần đưa đi. Các quy tắc thiết kế được ghi trong `.agents/skills/perfo-design-system/SKILL.md` và quy tắc luôn bật `.agents/rules/perfo-design.md`; giá trị token nằm trong `references/tokens.css` và được sao nguyên vẹn vào `frontend/src/index.css` (các giá trị màu, `--radius`, `--cut-*` và `--notch` trong hai file trùng nhau).

Ý tưởng đường xé được cụ thể hóa bằng ba họa tiết, mỗi họa tiết có một việc riêng. Họa tiết vát góc 45° (các lớp `cut-xs`, `cut-sm`, `cut-md`, `cut-lg`, `cut-flip`, cài đặt bằng `clip-path`) thay cho bo tròn ở mọi khối có hình như nút, thẻ, ô ghế. Họa tiết khấc (`notch-x`, `notch-y`, cài đặt bằng `mask`) là hình bán nguyệt cắt vào cạnh và chỉ dùng nơi nội dung thật sự tách đôi, như vé điện tử. Họa tiết đục lỗ (`perf-h`, `perf-v`) là hàng chấm dùng làm đường tách giữa phần giữ lại và phần đưa đi. Quy tắc thiết kế giới hạn mỗi màn hình tối đa một vùng dùng khấc hoặc đục lỗ. Ngoài ra có một chuyển động đặc trưng duy nhất là xé cuống vé khi đặt vé thành công (lớp `.tear-stub` với thuộc tính `data-torn`, thời lượng 420 ms, được tắt khi người dùng bật `prefers-reduced-motion`).

Về màu sắc, toàn bộ token màu viết trong không gian màu OKLCH, với nền trung tính ấm (kem ở chế độ sáng, nâu đỏ đậm ở chế độ tối), điểm nhấn là đỏ nhung `--primary` và vàng ánh kim `--accent`. Quy tắc luôn bật cấm viết mã màu (HEX, RGB, OKLCH) trực tiếp trong component, cấm `rounded-*` (trừ ảnh đại diện), `shadow-*`, `backdrop-blur` và `bg-gradient-*`; thay vào đó component chỉ dùng lớp ngữ nghĩa như `bg-card`, `text-muted-foreground`. Hình khối phẳng và đặc: `--radius` bằng `0px`, phân cấp bằng bậc tông giữa `--background`, `--card` và `--secondary`. Về chữ, tiêu đề và chữ số dùng Archivo (font biến thiên có trục độ rộng `wdth`, khai báo `--font-heading`), nội dung dùng Be Vietnam Pro các độ đậm 400, 500, 600 (khai báo `--font-sans`), cỡ chữ thân bài 15 px và giãn dòng 1,55. Các lớp `t-display`, `t-h1`, `t-h2`, `t-h3`, `t-numeral`, `t-code` quy định thang chữ; số liệu như giờ chiếu, giá tiền, mã ghế dùng `t-numeral` hoặc `t-code` với chữ số cùng độ rộng (`tabular-nums`). Việc chọn Be Vietnam Pro nhằm bảo đảm hiển thị đúng dấu tiếng Việt chồng nhau; quy tắc thiết kế yêu cầu tiêu đề có `line-height` từ 1,05 trở lên.

Trạng thái ghế được định nghĩa bằng lớp `.seat` với thuộc tính `data-state` nhận các giá trị `available`, `selected`, `held`, `booked`, `inactive`, `aisle` (cùng `data-type` cho loại ghế `vip` và `couple`). Nguyên tắc "trạng thái đọc được không cần màu" được thể hiện ở hình dạng: ghế đang chọn là khối đỏ `--primary` đặc, ghế đang giữ có nền sọc chéo màu `--held`, ghế đã bán có hai đường chéo gạch ngang ô, ghế VIP có một vạch vàng ở mép trên, ghế đôi rộng gấp đôi (60 px so với 28 px). Chú giải ở màn hình chọn ghế (Mục 4.9.4) dùng chính các ô ghế làm mẫu. Huy hiệu trạng thái được cấu hình tập trung trong `components/common/status-badge.config.ts`, gán mỗi trạng thái một nhãn, một biến thể màu và (với trạng thái đặt vé và thanh toán) một biểu tượng riêng: ví dụ trạng thái đặt vé `Pending` là "Chờ thanh toán" (`warning`, biểu tượng đồng hồ), `Confirmed` là "Đã xác nhận" (`success`, biểu tượng dấu tích), `Cancelled` là "Đã huỷ" (`destructive`) và `Expired` là "Hết hạn giữ vé" (`outline`).

Giao diện hỗ trợ chế độ sáng và tối. `ThemeProvider` (`app/providers/theme-provider.tsx`) nhận một trong ba giá trị `light`, `dark`, `system`, mặc định `system`, lưu lựa chọn vào `localStorage` theo khóa lấy từ `BRAND.storageKeys.theme` và gắn lớp `light` hoặc `dark` lên phần tử gốc; khối `.dark` trong `index.css` định nghĩa lại các token màu. Nút `ModeToggle` chuyển qua lại giữa hai chế độ. Hàm `readThemePalette` trong `lib/theme-palette.ts` đọc giá trị token hiện hành từ `document.documentElement` để các thành phần cần giá trị màu cụ thể (như biểu đồ) lấy được màu theo chế độ đang dùng.

Bảng 4.90 tổng hợp các token tiêu biểu được chọn từ `tokens.css`, kèm giá trị ở hai chế độ và lý do chọn. Các token thanh bên (`--sidebar-*`), biểu đồ (`--chart-*`) và các token `*-foreground` đi kèm không đưa vào bảng vì cùng quy tắc với token gốc.

Bảng 4.90. Token thiết kế rút gọn

| Token | Giá trị (sáng) | Giá trị (tối) | Mục đích | Lý do chọn |
|---|---|---|---|---|
| `--background` | `oklch(0.965 0.012 80)` | `oklch(0.12 0.014 30)` | Nền trang | Nền trung tính ấm (kem/nâu đỏ đậm) tạo không khí phòng chiếu; là bậc tông thấp nhất để phân cấp thay cho bóng đổ |
| `--foreground` | `oklch(0.2 0.02 30)` | `oklch(0.95 0.01 80)` | Chữ chính | Chữ chính trên mọi nền; cùng họ sắc độ ấm với nền |
| `--card` | `oklch(0.99 0.006 80)` | `oklch(0.18 0.02 30)` | Khối nổi một bậc (thẻ, hộp thoại) | Phân cấp bằng bậc tông so với `--background` thay cho viền và bóng |
| `--secondary` (cùng giá trị `--muted`) | `oklch(0.925 0.014 75)` | `oklch(0.22 0.018 30)` | Khối tách khỏi nền: ô nhập, ô ghế thường | Là màu nền của ô ghế trống trong `.seat`, nên chọn để thấy trạng thái cơ bản của sơ đồ ghế |
| `--muted-foreground` | `oklch(0.48 0.025 50)` | `oklch(0.7 0.02 60)` | Chữ phụ, đường chéo ghế đã bán | Phân cấp chữ; cũng dùng làm màu hàng chấm `perf-h`/`perf-v` |
| `--primary` | `oklch(0.45 0.19 25)` | `oklch(0.56 0.21 25)` | Đỏ nhung Perfo: nút chính, ghế đang chọn, cuống vé | Điểm nhấn duy nhất của mỗi màn hình; ở chế độ tối được tăng độ sáng và độ bão hòa để giữ độ nổi bật |
| `--accent` (`--vip` trỏ tới) | `oklch(0.72 0.15 82)` | `oklch(0.72 0.15 82)` | Vàng ánh kim: vạch ghế VIP, sao đánh giá | Giá trị giữ nguyên ở hai chế độ; chỉ dùng làm nền hoặc biểu tượng vì chữ vàng trên nền sáng không đủ tương phản |
| `--destructive` | `oklch(0.55 0.22 30)` | `oklch(0.62 0.22 30)` | Hủy vé, xóa dữ liệu, lỗi | Tách biệt ngữ nghĩa nguy hiểm khỏi `--primary` |
| `--success` | `oklch(0.52 0.12 155)` | `oklch(0.64 0.14 155)` | Đã thanh toán, thành công | Biến thể `success` của huy hiệu trạng thái |
| `--warning` | `oklch(0.7 0.15 70)` | `oklch(0.74 0.15 70)` | Chờ thanh toán, sắp hết hạn giữ ghế | Biến thể `warning` của huy hiệu; màu đồng hồ đếm ngược giữ ghế |
| `--held` | `oklch(0.7 0.15 62)` | `oklch(0.74 0.15 62)` | Ghế đang giữ | Token riêng cho trạng thái ghế, dùng trong nền sọc chéo của ghế `held` |
| `--couple` | `oklch(0.52 0.15 350)` | `oklch(0.64 0.15 350)` | Ghế đôi | Token riêng cho loại ghế đôi; cũng được dùng làm `--chart-4` |
| `--border` | `oklch(0.87 0.014 75)` | `oklch(0.28 0.02 30)` | Đường kẻ bảng, đáy ô nhập | Quy tắc thiết kế chỉ cho dùng làm đường kẻ, không bao quanh khối |
| `--radius` | `0px` | `0px` | Bo góc | Thể hiện nguyên tắc không bo tròn; thay bằng vát góc |
| `--cut-xs`, `--cut-sm`, `--cut-md`, `--cut-lg` | `4px`, `6px`, `12px`, `20px` | `4px`, `6px`, `12px`, `20px` | Kích thước vát góc cho chip, nút/ô nhập, thẻ, hộp thoại và vé | Bốn cấp vát tương ứng bốn cấp khối |
| `--notch` | `10px` | `10px` | Bán kính khấc của họa tiết `notch-x`, `notch-y` | Dùng ở vé điện tử và dòng suất chiếu có cuống |
| `--font-heading` | `"Archivo Variable", "Arial Narrow", sans-serif` | (như chế độ sáng) | Tiêu đề, chữ số, mã vé/mã ghế | Font hẹp, trục `wdth` cho phép số liệu nổi bật |
| `--font-sans` | `"Be Vietnam Pro", system-ui, sans-serif` | (như chế độ sáng) | Nội dung | Hỗ trợ dấu tiếng Việt chồng |

*Ghi chú:* Giá trị lấy nguyên từ `.agents/skills/perfo-design-system/references/tokens.css`; giá trị ở các khối `:root` và `.dark` trong `frontend/src/index.css` khớp với các giá trị trên.

### 4.9.2 Sơ đồ trang khách hàng

Phần khách hàng được khai báo trong `app/routes/client-routes.tsx` và ghép vào bộ định tuyến gốc `app/routes/router.tsx`, cùng với trang lỗi `ErrorPage` (đặt ở `errorElement`), trang `/403` và trang `*` (không tìm thấy). Các đường dẫn được chia làm ba nhóm theo cách bảo vệ. Hình 4.66 thể hiện cấu trúc này.

Nhóm thứ nhất là các trang công khai, đặt dưới `ClientLayout` (gồm đầu trang, chân trang và `AiAssistantWidget` của trợ lý AI) và không có lớp bảo vệ: `/` (trang chủ), `/movies` (danh sách phim), `/movies/:id` (chi tiết phim), `/schedule` (lịch chiếu), `/membership` (thành viên), `/policies/:slug` (trang chính sách), `/booking/:showtimeId` (đặt vé) và `/booking/result` (kết quả đặt vé). Trang đặt vé không bị chặn ở mức định tuyến; việc yêu cầu đăng nhập được xử lý trong trang: khi khách chưa đăng nhập bấm giữ ghế hoặc thanh toán, `useBookingFlow` mở hộp thoại "Yêu cầu đăng nhập" và chuyển tới `/auth/login?redirect=…` để quay lại đúng trang sau khi đăng nhập.

Nhóm thứ hai là các trang tài khoản dưới `/account`, được bọc bởi `CustomerProtectedRoute`: khi chưa đăng nhập, người dùng bị chuyển tới `/auth/login?redirect=…` kèm đường dẫn hiện tại. Nhóm này gồm `/account/profile`, `/account/tickets`, `/account/reviews`, `/account/change-password` (dùng `AccountLayout` có thanh điều hướng tài khoản) và `/account/tickets/:id` (chi tiết vé, không dùng `AccountLayout`). Lớp bảo vệ chỉ kiểm tra trạng thái đăng nhập, không kiểm tra vai trò.

Nhóm thứ ba là các trang xác thực dưới `/auth`, được bọc bởi `GuestRoute` (chuyển hướng về `/` hoặc đường dẫn trong tham số `redirect` nếu đã đăng nhập) và dùng `AuthLayout`: `/auth/login`, `/auth/register`, `/auth/confirm-email`, `/auth/forgot-password`, `/auth/reset-password`.

`config/routes.ts` gom hằng số đường dẫn `ROUTES` cho trang chủ, đăng nhập, đăng ký, bốn trang tài khoản và toàn bộ trang quản trị; các đường dẫn còn lại của phần khách hàng được viết trực tiếp trong `client-routes.tsx`.

![Hình 4.66. Sơ đồ trang khách hàng](hinh/4.66.png)

*Hình 4.66. Sơ đồ trang khách hàng*

### 4.9.3 Sơ đồ trang quản trị

Phần quản trị được khai báo trong `app/routes/admin-routes.tsx`. Trang đăng nhập quản trị `/admin/login` được bọc bởi `GuestRoute` với đích chuyển hướng là `/admin/dashboard` và dùng `AdminAuthLayout`. Mọi đường dẫn còn lại dưới `/admin` được bọc bởi `AdminProtectedRoute`: chưa đăng nhập thì chuyển tới `/admin/login`, người dùng đã đăng nhập nhưng không phải `Role.Admin` hoặc `Role.Employee` thì chuyển về `/`. Các trang bên trong dùng `AdminLayout` gồm thanh bên và thanh trên, và có hai cụm. Cụm vận hành gồm `/admin/dashboard`, `/admin/bookings`, `/admin/bookings/:id`, `/admin/bookings/counter` (quầy bán vé), `/admin/checkin` (soát vé), `/admin/payments`, `/admin/reports`. Cụm quản lý dữ liệu gồm `/admin/movies`, `/admin/showtimes`, `/admin/rooms`, `/admin/rooms/:id/seats` (trình soạn sơ đồ ghế), `/admin/combos`, `/admin/users`, `/admin/employees`, `/admin/reviews`, `/admin/settings` và trung tâm danh mục `/admin/catalog` cùng chín trang con `/admin/catalog/genres`, `languages`, `age-ratings`, `seat-types`, `screen-types`, `membership-tiers`, `positions`, `actors`, `directors`.

Đường vào mặc định `/admin` được xử lý bởi `AdminIndexRedirect`, gọi hàm `getAdminHomePath(role)` trong `config/routes.ts`: nhân viên (`Role.Employee`) được chuyển thẳng tới quầy bán vé `/admin/bookings/counter`, vai trò còn lại tới `/admin/dashboard`. Hàm này cũng được dùng ở biểu mẫu đăng nhập quản trị (`admin-login-form.tsx`) để chọn trang đích khi không có tham số `redirect`.

Phân quyền theo vai trò ở phía giao diện thể hiện qua cấu hình menu `ADMIN_NAV_GROUPS` trong `config/admin-nav.ts`, mỗi mục khai báo danh sách `roles` được thấy; hàm `filterNavGroupsByRole` lọc các nhóm theo vai trò hiện tại. Quản trị viên thấy tất cả sáu nhóm menu. Nhân viên chỉ thấy năm mục: Bán vé tại quầy, Soát vé, Quản lý phim, Suất chiếu và Quản lý đặt vé; các mục còn lại (Dashboard, Phòng chiếu, Bắp nước và Combo, Giao dịch và thanh toán, Báo cáo và doanh thu, Khách hàng, Nhân viên, Đánh giá phim, Trung tâm danh mục, Cài đặt hệ thống) chỉ dành cho quản trị viên. Ở mức định tuyến, cả hai vai trò cùng được vào mọi đường dẫn `/admin/*` (chỉ có một lớp `AdminProtectedRoute`); bên trong trang, `movies-page.tsx` và `showtimes-page.tsx` dùng biến `canManage = role === Role.Admin` để ẩn thao tác thêm, sửa. Quyền truy cập dữ liệu được thực thi ở phía API bằng chính sách `AdminOnly` và `StaffOnly` (Mục 4.10.1), nên lớp giao diện chỉ đóng vai trò điều hướng.

![Hình 4.67. Sơ đồ trang quản trị](hinh/4.67.png)

*Hình 4.67. Sơ đồ trang quản trị*

### 4.9.4 Màn hình chọn ghế

Màn hình chọn ghế (đường dẫn `/booking/:showtimeId`, trang `BookingPage`) cho phép khách hàng chọn ghế và combo của một suất chiếu, giữ ghế rồi chuyển sang thanh toán. Ảnh chụp xem Hình 5.6 (chọn ghế) và Hình 5.7 (thanh toán). Trang gồm đầu trang suất chiếu `BookingShowtimeHeader`, thanh bước `BookingStepper` (ba bước "Chọn ghế", "Thanh toán", "Vé") và nội dung theo bước do hook `useBookingFlow` điều khiển; ở bước chọn ghế, `BookingSelectionStep` chia lưới ba cột trên màn hình rộng, hai cột bên trái cho ghế và combo, một cột bên phải cho tổng tiền.

Cột chính gồm các thành phần sau. `SeatSuggestionPanel` ("Gợi ý ghế liền kề") cho chọn số người (từ 1 đến 10, mặc định 2) và gọi `useSeatSuggestions`; các phương án trả về chỉ được giữ lại nếu mọi ghế vẫn đang trống trong bản đồ trạng thái thời gian thực, khi bấm vào một phương án thì các ghế đó được chọn và được đánh dấu nổi bật trên sơ đồ (thuộc tính `data-mark="suggested"`); thuật toán gợi ý ở Mục 4.6.2. `SelectableSeatMap` vẽ sơ đồ ghế trong `SeatMapFrame` gồm chú giải `SeatLegend` (sáu mục: Trống, VIP, Ghế đôi, Đang chọn, Đang giữ, Đã bán), thanh màn hình `CinemaScreen` và lưới `SeatGrid` gồm các `SeatButton`; trạng thái từng ghế lấy từ danh sách ghế do API trả về chồng với các thay đổi thời gian thực nhận qua `useRealtimeSeatMap` (Mục 4.7.3). Mỗi ghế có nhãn đọc màn hình dạng "Ghế H12, trống". Tiếp theo là `ComboPicker` (chọn combo, danh sách được sắp xếp theo gợi ý từ `useComboSuggestions` theo số ghế đã chọn) và `BookingCustomerForm` (họ tên, số điện thoại).

Cột phụ gồm `BookingPriceSummary` (tạm tính theo ghế và combo, hàm `calcBookingEstimate`), nút "Giữ ghế & tiếp tục thanh toán" và dòng nhắc thời gian giữ ghế lấy từ cấu hình `holdDurationMinutes`. Trên màn hình hẹp, nút và tổng tiền chuyển thành thanh cố định ở đáy trang. Nút giữ ghế chỉ được bật (`canHold`) khi đã chọn ít nhất một ghế, không có ghế lẻ (`loneSeats` rỗng, theo luật ghế lẻ ở Mục 4.6.1; khi vi phạm, giao diện hiển thị cảnh báo kèm danh sách ghế lẻ), họ tên không rỗng đối với người đã đăng nhập, và số điện thoại hoặc để trống hoặc hợp lệ theo `isValidVnPhone`.

Sau khi giữ ghế thành công, luồng chuyển sang bước thanh toán (`BookingPaymentStep`): đồng hồ đếm ngược `BookingHoldTimer` (đổi sang màu cảnh báo khi còn dưới 60 giây), `BookingPriceSummary` với số tiền chính thức từ phản hồi giữ ghế, `BookingPaymentMethod` để chọn phương thức, nút "Thanh toán" (chuyển trình duyệt tới `paymentUrl` của cổng thanh toán) và nút "Huỷ, chọn lại ghế". Khi hết hạn giữ ghế, màn hình hiển thị thông báo và quay về bước chọn ghế theo cơ chế ở Mục 4.7.3. Tham số truy vấn `?resume=<bookingId>` cho phép tiếp tục thanh toán một đơn đang chờ từ trang chi tiết vé; nếu đơn không còn hợp lệ, trang hiển thị lý do (không còn ở trạng thái chờ thanh toán, đã hết hạn, hoặc không tải được) cùng hai nút điều hướng.

### 4.9.5 Màn hình vé điện tử

Vé điện tử cho khách hàng xem thông tin đơn đặt vé và mã QR để vào rạp; màn hình là trang chi tiết vé `/account/tickets/:id` (`AccountTicketDetailPage`), có danh sách vé ở `/account/tickets` (`AccountTicketsPage`, dùng `TicketCard`). Ảnh chụp xem Hình 5.9 và Hình 5.10.

Trang chi tiết vé gồm các khối theo thứ tự: khối mã đặt vé (`bookingCode` dùng kiểu `t-code`, có nút sao chép, thời điểm đặt và huy hiệu `BookingStatusBadge`); khối cảnh báo cho đơn đang chờ thanh toán, có đếm ngược giữ ghế (`useCountdown`) và nút "Thanh toán ngay" dẫn về `/booking/:showtimeId?resume=<bookingId>`, hoặc nút "Đặt vé mới" nếu đã hết hạn; khối phim (poster, tên phim, phòng, giờ chiếu); khối "Ghế & Combo" liệt kê giá từng ghế, từng combo, tổng tiền, giảm giá (khi lớn hơn 0) và thành tiền; khối "Thanh toán" liệt kê từng giao dịch cùng phương thức và trạng thái; và khối "Vé điện tử & Mã QR" gồm một `PerfoTicket` cho mỗi vé. Nút "Hủy vé & Hoàn tiền" chỉ hiện khi đơn ở trạng thái `Confirmed`, chính sách có hoàn tiền (`cancelRefundPercent > 0`) và thời điểm hiện tại còn cách giờ chiếu nhiều hơn `cancelDeadlineHours` giờ (mặc định 2 nếu chưa tải được cấu hình); bấm nút mở hộp thoại xác nhận trước khi gọi `useCancelBooking`.

`PerfoTicket` (`components/common/perfo-ticket.tsx`) là một ảnh SVG kích thước `viewBox` 920×380 vẽ hai phần của tấm vé: phần thân bên trái ghi tên phim (cắt ở 25 ký tự), suất chiếu, phòng, mã ghế, tổng tiền và đường nét đứt; phần cuống bên phải nền đỏ gồm mã QR và mã vé. Mã QR hiển thị bằng thẻ ảnh khi giá trị truyền vào là ảnh (`data:image` hoặc địa chỉ `http`), ngược lại được vẽ bằng `QRCodeSVG` (mức sửa lỗi `M`). Với vé đã soát (`isCheckedIn`), một lớp phủ tối kèm dòng chữ "ĐÃ SOÁT VÉ" được vẽ lên phần thân. Cùng thành phần này được dùng lại ở trang kết quả đặt vé và màn hình quầy (Mục 4.9.7).

### 4.9.6 Màn hình trình soạn sơ đồ ghế

Trình soạn sơ đồ ghế (đường dẫn `/admin/rooms/:id/seats`, trang `AdminRoomSeatsPage`, chỉ hiện trong menu của quản trị viên) cho phép quản trị viên thiết kế lại bố cục ghế của một phòng chiếu. Ảnh chụp xem Hình 5.22. Trang tải thông tin phòng, danh sách ghế hiện có và danh sách loại ghế, rồi dựng bộ soạn thảo gồm thanh công cụ `SeatEditorToolbar`, tùy chọn bảng kiểm tra `SeatEditorValidationPanel` và lưới `SeatEditorGrid`; trạng thái soạn thảo do hook `useSeatEditorState` quản lý.

Thanh công cụ gồm nhóm "cọ vẽ" `SeatEditorBrushTools`, mỗi lần chọn một cọ rồi bấm vào ô ghế để áp dụng. Các cọ được định nghĩa trong kiểu `SeatEditorBrush`: gán một loại ghế (`seatType`, mỗi loại ghế trong danh mục là một nút), "Ghép ghế đôi" (`pairCouple`), "Đặt/bỏ lối đi" (`toggleAisle`) và "Bật/tắt ghế" (`toggleActive`, cọ mặc định). Các thao tác còn lại gồm "Chọn nhiều ô (kéo chuột)" để chọn vùng hình chữ nhật rồi dùng nút "Áp dụng công cụ hiện tại cho vùng đã chọn" (`SeatEditorSelectionBar`); "Sao chép hàng ghế" để chọn hàng nguồn rồi bấm nhãn hàng đích để dán (`SeatEditorCopyRowBar`); "Tạo layout tự động" mở biểu mẫu `SeatEditorAutoGenerateForm` với các tham số số hàng, số cột, loại ghế mặc định, loại ghế VIP, số hàng VIP, số hàng ghế đôi (tính từ hàng cuối) và các cột lối đi cách nhau bởi dấu phẩy; ô nhập kích thước (`SeatEditorSizeInputs`) giới hạn tối đa 52 hàng và 60 cột; "Xem trước" chuyển lưới sang chế độ chỉ xem, và hai nút "Hoàn tác", "Làm lại" (`SeatEditorHistoryButtons`) dựa trên lịch sử `useSeatEditorHistory` giữ tối đa 40 trạng thái (`HISTORY_LIMIT`). Nút "Lưu sơ đồ ghế" gửi toàn bộ bố cục lên API (Mục 4.10.2) và bị vô hiệu khi đang lưu hoặc đang xem trước.

Nút "Kiểm tra layout" chạy hàm `validateSeatLayout` và hiển thị kết quả trong `SeatEditorValidationPanel` dưới dạng danh sách vấn đề có mức `error` hoặc `warning`. Bộ kiểm tra gồm bốn nhóm: ghế nằm ngoài kích thước phòng đã khai báo (`error`), ghế trùng vị trí (`error`), cặp ghế đôi có số ghế khác 2 (`error`) hoặc không liền kề (`warning`), và hàng có ghế hoạt động nhưng không có ô lối đi nào (`warning`). Việc kiểm tra này chạy ở phía trình duyệt; các ràng buộc phía máy chủ do `RoomService` thực hiện (Mục 4.6).

### 4.9.7 Màn hình quầy bán vé

Màn hình quầy bán vé (đường dẫn `/admin/bookings/counter`, trang `AdminCounterPage`) phục vụ nhân viên và quản trị viên bán vé trực tiếp cho khách. Đây là trang đầu tiên nhân viên thấy sau khi đăng nhập (Mục 4.9.3). Ảnh chụp xem Hình 5.23. Hook `useCounterFlow` điều khiển bốn bước `showtime`, `seats`, `checkout`, `done`; nút "Bắt đầu lại" xuất hiện từ bước thứ hai trở đi để quay về bước chọn suất.

Ở bước chọn suất, `CounterShowtimePicker` cho chọn ngày và hiển thị các suất lấy từ `useSchedule`; suất hết ghế hoặc không ở trạng thái `Scheduled` bị vô hiệu. Ở bước chọn ghế, `CounterSeatPanel` hiển thị sơ đồ ghế (cùng cơ chế cập nhật thời gian thực như phía khách hàng, Mục 4.7.3), cột bên phải gồm `ComboPicker`, `CounterCustomerForm` (họ tên, số điện thoại và `CustomerSearchCombobox` để tìm tài khoản khách hàng có sẵn qua `useCustomerLookup` với từ khóa được làm trễ bằng `useDebouncedValue`, khi liên kết tài khoản thì họ tên và số điện thoại lấy từ tài khoản), `CounterPriceSummary` và nút "Giữ ghế & tiếp tục". Ở bước thanh toán, `CounterCheckoutPanel` hiển thị mã đơn, đồng hồ đếm ngược giữ ghế, số tiền cần thanh toán, ô nhập "Số tiền khách đưa (tiền mặt / chuyển khoản)" (mặc định bằng số tiền cần thanh toán), tiền thừa tính theo công thức số tiền khách đưa trừ số tiền cần thanh toán (không âm), và nút "Xác nhận thanh toán" bị vô hiệu khi số tiền chưa đủ hoặc đã hết hạn giữ. Khi hết hạn giữ ghế, thông báo thời gian thực làm màn hình đặt lại luồng. Bước cuối `CounterTicketResult` thông báo "Thanh toán thành công", tiền thừa và hiển thị `PerfoTicket` cho từng vé của đơn, cùng nút "Bán vé mới".

### 4.9.8 Màn hình soát vé

Màn hình soát vé (đường dẫn `/admin/checkin`, trang `AdminCheckinPage`) cho nhân viên xác nhận khách vào rạp bằng mã QR của vé. Ảnh chụp xem Hình 5.24. Trang chia hai khung: khung nhập liệu và khung kết quả.

Khung nhập liệu có hai cách đưa mã vào. Cách thứ nhất là nhập mã vào ô `CheckinCodeInput` rồi bấm "Soát vé ngay" (hoặc gửi biểu mẫu); sau mỗi lần xử lý, ô nhập được xóa và lấy lại tiêu điểm để quét tiếp bằng đầu đọc mã. Cách thứ hai là quét bằng camera qua `CheckinCameraPanel`, dùng hook `useQrCameraScanner`: hook xin quyền camera sau (`facingMode: "environment"`), đọc từng khung hình bằng `requestAnimationFrame`, dùng `BarcodeDetector` của trình duyệt nếu có và dùng thư viện `jsQR` làm phương án dự phòng. Để tránh gửi lặp một mã, hook bỏ qua cùng một giá trị xuất hiện lại trong khoảng `QR_SCAN_REPEAT_GUARD_MS` (2500 ms). Hook cũng phân loại lỗi camera thành các thông báo riêng: không hỗ trợ camera hoặc không dùng HTTPS, bị từ chối quyền, không tìm thấy camera.

Mỗi mã được gửi bằng `useCheckin` (API `POST` tới nhóm `bookings` của quản trị, Mục 4.10.2) và bỏ qua nếu một yêu cầu trước đó đang xử lý. Kết quả hiển thị ở `CheckinResultCard` với ba trạng thái: chưa có vé nào được soát ("Chưa có vé nào được soát"); thành công ("Check-in thành công", giờ soát, tên phim, mã ghế, giờ chiếu theo `CheckinResponse`); thất bại ("Soát vé thất bại" cùng thông báo lấy từ phản hồi lỗi của máy chủ, hoặc "Mã vé không hợp lệ hoặc đã được sử dụng" nếu không có). Hook `useCheckinSound` phát âm báo riêng cho thành công và thất bại.

### 4.9.9 Màn hình dashboard

Dashboard (đường dẫn `/admin/dashboard`, trang `AdminDashboardPage`, dành cho quản trị viên) cung cấp cái nhìn tổng quan về hoạt động trong ngày và xu hướng doanh thu. Ảnh chụp xem Hình 5.16. Trang gọi hai truy vấn: `useDashboardSummary` cho số liệu trong ngày và `useDashboardInsights({ days })` cho dữ liệu xu hướng, với bộ lọc `DashboardDaysFilter` chọn 7, 14 hoặc 30 ngày qua (mặc định 14 ngày).

Dòng đầu là `DashboardStatCards` gồm năm thẻ số liệu: "Doanh thu hôm nay" (kèm phần trăm thay đổi so với hôm qua khi có), "Vé bán hôm nay", "Đơn đã xác nhận", "Đơn đang chờ" (kèm nhắc "Cần theo dõi" khi lớn hơn 0) và "Tỉ lệ lấp đầy hôm nay" (hiển thị dấu "—" khi không có giá trị); mỗi thẻ có số lớn dùng kiểu `t-numeral` và nhãn bên dưới. Phần giữa gồm `RevenueTrendChart`, biểu đồ cột chồng (thư viện `recharts`) của doanh thu vé và doanh thu combo theo kỳ, và `TopMoviesCard`, biểu đồ cột ngang xếp hạng phim theo số vé bán ra (tên phim dài được cắt ở 18 ký tự). Phần dưới là `TodayShowtimesCard`, liệt kê các suất chiếu trong ngày với số ghế đã đặt trên tổng số ghế. Mỗi khối có trạng thái trống riêng (`EmptyState`) khi chưa có dữ liệu.

Trang báo cáo `/admin/reports` (`AdminReportsPage`) cho phép phân tích sâu hơn với bộ lọc `ReportsFilterBar` gồm ngày bắt đầu, ngày kết thúc và cách gom nhóm theo ngày, tuần hoặc tháng. Trang gồm thẻ tổng hợp `RevenueSummaryCards`, biểu đồ `RevenueReportChart`, bảng phim bán chạy `TopMoviesReportTable` (giới hạn `TOP_MOVIES_REPORT_LIMIT`) và bảng hiệu suất nhân viên `EmployeePerformanceTable`. Ảnh chụp xem Hình 5.27.

## 4.10 GIAO DIỆN LẬP TRÌNH

Hệ thống cung cấp giao diện lập trình dạng REST do các controller trong `backend/Cinema.Api/Controllers` đảm bảo; giao diện web (Mục 4.9) và các cổng thanh toán đều là bên gọi các endpoint này. Mục này nêu các quy ước chung và số lượng endpoint theo nhóm; danh mục đầy đủ từng endpoint (phương thức, đường dẫn, quyền, tham số) được liệt kê ở Phụ lục 2.

### 4.10.1 Quy ước thiết kế API

**Tiền tố và tách tài liệu.** Mọi controller khai báo đường dẫn bắt đầu bằng `api/v1`. Các endpoint quản trị nằm dưới `api/v1/admin/...` (ví dụ `api/v1/admin/movies`), các endpoint cho khách hàng nằm dưới tiền tố riêng của từng tài nguyên (ví dụ `api/v1/bookings`, `api/v1/movies`); `CatalogController` dùng tiền tố `api/v1` chung. Tài liệu OpenAPI được sinh bằng Swashbuckle và chia làm hai tài liệu: `client` chứa mọi endpoint không bắt đầu bằng `api/v1/admin` (kể cả `health` và `payments`) và `admin` chứa các endpoint bắt đầu bằng `api/v1/admin`, được chọn bằng `DocInclusionPredicate`. Giao diện Swagger UI, với hai tài liệu "Client API" và "Admin API", chỉ được bật khi môi trường là `Development`.

**Xác thực và phân quyền.** API xác thực bằng JWT Bearer (cấu hình ở Mục 4.4); token đọc từ tiêu đề `Authorization`, riêng các đường dẫn bắt đầu bằng `/hubs` còn nhận từ tham số truy vấn `access_token` (Mục 4.7.1). Phân quyền dùng bốn chính sách `CustomerOnly`, `AdminOnly`, `EmployeeOnly`, `StaffOnly` (`StaffOnly` cho phép cả `Admin` và `Employee`). Tất cả controller quản trị đều khai báo `[Authorize]` ở mức lớp; ở mức phương thức, chín trong 93 endpoint quản trị dùng `StaffOnly` (năm endpoint của `AdminBookings`, hai của `AdminShowtimes`, một của `AdminMovies`, một của `AdminUsers`) và 84 endpoint còn lại dùng `AdminOnly`. Phía khách hàng, `BookingsController` và `UsersController` yêu cầu đăng nhập ở mức lớp, ba endpoint đánh giá phim của `MoviesController` dùng `CustomerOnly`, hai endpoint `socket-token` và `me` của `AuthController` yêu cầu đăng nhập; các endpoint còn lại, trong đó có `AiController`, không khai báo yêu cầu đăng nhập và nhận danh tính (nếu có) từ claim của token.

**Định dạng phản hồi.** Kết quả thành công của mọi action được bọc tự động thành `ApiResponse` gồm bốn trường `success`, `data`, `message` và `errors` bởi bộ lọc `ApiResponseFilter`; thông điệp thành công có thể gắn qua thuộc tính `SuccessMessage`. Ba action của `PaymentsController` (IPN VNPay, IPN MoMo, webhook Stripe) gắn thuộc tính `RawResponse` để trả về đúng định dạng mà từng cổng thanh toán yêu cầu thay vì `ApiResponse`.

**Xác thực dữ liệu và mã lỗi.** Bộ lọc `ValidationActionFilter` tìm `IValidator<T>` (FluentValidation) của từng tham số action; nếu không hợp lệ, trả về mã 400 với `message` là "Dữ liệu không hợp lệ." và `errors` là từ điển tên trường (chữ cái đầu viết thường) tới danh sách thông báo. Middleware `ExceptionHandlingMiddleware` chuyển các ngoại lệ ứng dụng thành `ApiResponse` lỗi với mã trạng thái do lớp ngoại lệ quyết định: `ValidationAppException` 400, `UnauthorizedAppException` 401, `ForbiddenAppException` 403, `NotFoundException` 404, `ConflictAppException` 409, `BusinessRuleException` 422. `SeatConflictException` (mã 409) trả thêm trường `errors.conflictSeatIds` chứa danh sách ghế xung đột để giao diện tô lại sơ đồ. Lỗi cơ sở dữ liệu PostgreSQL được ánh xạ theo `SqlState`: `23503` (khóa ngoại) và `23514` (ràng buộc kiểm tra) thành 400, `23505` (trùng giá trị duy nhất) thành 409, `23P01` (ràng buộc loại trừ) thành 409 (riêng ràng buộc `EX_Showtimes_Room_NoOverlap` trả thông báo trùng lịch phòng), các `SqlState` khác thành 400; xung đột đồng thời (`DbUpdateConcurrencyException`) thành 409. Mọi ngoại lệ khác trả mã 500, trong môi trường `Development` kèm tên và chi tiết ngoại lệ, ngoài ra chỉ trả thông báo chung. `ApiResponse` không có trường mã lỗi riêng; phân loại lỗi dựa vào mã trạng thái HTTP, `message` và `errors`.

**Phân trang.** Có hai kiểu. Danh sách phía khách hàng dùng phân trang theo con trỏ `CursorPageResult<T>` gồm `items`, `nextCursor`, `hasMore` (áp dụng cho danh sách đặt vé của tôi, danh sách phim, đánh giá của phim và đánh giá của tôi), với giới hạn mặc định `CursorPagination.DefaultLimit` bằng 10 và hàm `ClampLimit` ép trong khoảng từ 1 đến 50. Danh sách quản trị dùng phân trang theo trang `OffsetPageResult<T>` gồm `items`, `page`, `pageSize`, `totalItems`, `totalPages`.

**Endpoint thanh toán công khai nhưng xác minh chữ ký.** `PaymentsController` không khai báo `[Authorize]` vì bên gọi là máy chủ của cổng thanh toán, không có JWT của hệ thống: `vnpay/ipn` (nhận cả `GET` và `POST`), `momo/ipn` và `stripe/webhook`. Việc bảo vệ nằm ở `PaymentService`: IPN VNPay và MoMo kiểm tra chữ ký của tham số nhận được và trả mã `97` kèm thông báo "Invalid signature" khi chữ ký sai; webhook Stripe chuyển nguyên văn thân yêu cầu và tiêu đề `Stripe-Signature` cho `StripeGateway` (dùng `EventUtility.ConstructEvent`) để kiểm tra, và trả 400 khi chữ ký không hợp lệ.

**Giới hạn tốc độ.** Hệ thống dùng bộ giới hạn tốc độ tích hợp sẵn của ASP.NET Core, trả mã 429 khi vượt ngưỡng, gồm bộ giới hạn toàn cục và hai chính sách đặt tên. Các thông số đọc từ mục `RateLimit` của cấu hình (lớp `RateLimitOptions`), giá trị mặc định và giá trị trong `appsettings.json` trùng nhau như sau: bộ giới hạn toàn cục là cửa sổ trượt 100 yêu cầu trong 60 giây (chia 4 đoạn) theo địa chỉ IP; chính sách `auth` là cửa sổ cố định 5 yêu cầu trong 60 giây theo địa chỉ IP, áp dụng cho bảy endpoint của `AuthController` là `register`, `login`, `admin-login`, `google`, `refresh`, `forgot-password`, `reset-password`; chính sách `booking-hold` là cửa sổ cố định 10 yêu cầu trong 60 giây theo tên người dùng (nếu không có thì theo IP), áp dụng cho endpoint `POST /api/v1/bookings/hold`. Các chính sách không dùng hàng đợi (`QueueLimit = 0`). Middleware giới hạn tốc độ không được đăng ký khi môi trường là `Testing`.

### 4.10.2 Tổng hợp endpoint theo nhóm

Số endpoint được đếm bằng cách đếm các thuộc tính `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpPatch]` và `[HttpDelete]` trên từng phương thức của 34 controller trong `Controllers/`. Hai thuộc tính trên cùng một phương thức (như `vnpay/ipn` nhận cả `GET` và `POST`) được tính là hai endpoint. Kết quả được tổng hợp ở Bảng 4.91 theo ba nhóm: nhóm quản trị (22 controller, đường dẫn dưới `api/v1/admin`), nhóm khách hàng (9 controller trong thư mục `Client`) và nhóm khác (`Health` và `Payments`, nằm ngoài hai thư mục trên). Tổng cộng có 137 endpoint, trong đó nhóm quản trị 93, nhóm khách hàng 39 và nhóm khác 5.

Bảng 4.91. Tổng hợp endpoint theo controller

| Nhóm | Controller | Tiền tố đường dẫn | Số endpoint |
|---|---|---|---|
| Admin | AdminActors | `api/v1/admin/actors` | 5 |
| Admin | AdminAgeRatings | `api/v1/admin/age-ratings` | 4 |
| Admin | AdminBookings | `api/v1/admin/bookings` | 5 |
| Admin | AdminCombos | `api/v1/admin/combos` | 5 |
| Admin | AdminDashboard | `api/v1/admin/dashboard` | 2 |
| Admin | AdminDirectors | `api/v1/admin/directors` | 5 |
| Admin | AdminEmployees | `api/v1/admin/employees` | 3 |
| Admin | AdminGenres | `api/v1/admin/genres` | 4 |
| Admin | AdminLanguages | `api/v1/admin/languages` | 4 |
| Admin | AdminMembershipTiers | `api/v1/admin/membership-tiers` | 4 |
| Admin | AdminMovies | `api/v1/admin/movies` | 5 |
| Admin | AdminPayments | `api/v1/admin/payments` | 2 |
| Admin | AdminPositions | `api/v1/admin/positions` | 4 |
| Admin | AdminReports | `api/v1/admin/reports` | 3 |
| Admin | AdminReviews | `api/v1/admin/reviews` | 3 |
| Admin | AdminRooms | `api/v1/admin/rooms` | 7 |
| Admin | AdminScreenTypes | `api/v1/admin/screen-types` | 4 |
| Admin | AdminSeatTypes | `api/v1/admin/seat-types` | 4 |
| Admin | AdminSettings | `api/v1/admin/settings` | 2 |
| Admin | AdminShowtimes | `api/v1/admin/showtimes` | 10 |
| Admin | AdminUploads | `api/v1/admin/uploads` | 2 |
| Admin | AdminUsers | `api/v1/admin/users` | 6 |
| Client | Ai | `api/v1/ai` | 1 |
| Client | Auth | `api/v1/auth` | 12 |
| Client | Bookings | `api/v1/bookings` | 5 |
| Client | Catalog | `api/v1` | 4 |
| Client | Combos | `api/v1/combos` | 2 |
| Client | Movies | `api/v1/movies` | 7 |
| Client | Settings | `api/v1/settings` | 1 |
| Client | Showtimes | `api/v1/showtimes` | 3 |
| Client | Users | `api/v1/users` | 4 |
| Khác | Health | `api/v1/health` | 1 |
| Khác | Payments | `api/v1/payments` | 4 |
| **Tổng** | Admin 93 · Client 39 · Khác 5 | | 137 |

*Ghi chú:* Số liệu đếm từ mã nguồn `backend/Cinema.Api/Controllers/**`; danh mục từng endpoint ở Phụ lục 2.

<!-- GHI CHÚ ĐỐI CHIẾU -->
<!--
(a) [CẦN BỔ SUNG] còn lại
- Mã Mermaid Hình 4.66 và 4.67: không có trong các file đính kèm (`ma-nguon-so-do-bo-sung.md` chỉ có PlantUML Hình 3.x/4.4–4.25; `ma-nguon-so-do-mermaid-loyalty.md` không liên quan; `ma-nguon-so-do.md` chưa đính kèm). Văn bản mô tả Hình 4.66/4.67 được viết theo cấu hình route thật (`client-routes.tsx`, `admin-routes.tsx`, `protected-route/*`, `config/routes.ts`, `config/admin-nav.ts`); khi có mã Mermaid cần đối chiếu node và quan hệ với mô tả. Lưu ý cần có node cho `/403`, `*`, `ErrorPage` nếu muốn khớp mô tả.
- Phụ lục 2 (PHẦN 34) chưa có để đối chiếu; số 137 (Admin 93, Client 39, Khác 5) đã đếm lại từ controller và khớp bảng mẫu trong outline từng dòng; khi Phụ lục 2 xong cần kiểm tra lại, đặc biệt quy ước đếm `vnpay/ipn` (GET + POST) là 2 endpoint.
- Ảnh chụp Hình 5.6, 5.7, 5.9, 5.10, 5.16, 5.22, 5.23, 5.24, 5.27 (Chương 5) chưa có; số hiệu lấy từ danh mục hình trong outline.
- Tham chiếu chéo Mục 4.4 (cấu hình JWT), 4.6.1, 4.6.2, 4.6, 4.7.1, 4.7.3 dựa trên outline; cần kiểm tra số mục khi gộp.
- Không có trích dẫn tài liệu tham khảo trong phần này; nếu cần cho OKLCH, REST, OpenAPI: [CẦN BỔ SUNG: trích dẫn nhóm Tx].

(b) Chỗ outline lệch / cần lưu ý so với code
- Tên file quy định: prompt ghi `quy-dinh-trinh-bay.md`, file đính kèm là `quy-dinh-format-bao-cao.md`; đã theo file thực tế (chú thích hình dưới, chú thích bảng trên).
- Outline 4.9.1 gọi nguồn `frontend/src/lib/theme-palette.ts`: file chỉ có hàm `readThemePalette(tokens)` đọc giá trị token từ `document.documentElement`, không định nghĩa bảng màu; chế độ sáng/tối thực ra do `app/providers/theme-provider.tsx` và `components/common/mode-toggle.tsx`. Đã mô tả đúng theo code.
- Outline 4.9.1: "quy tắc không bóng, không gradient, không bo góc" chỉ đúng cho token/quy tắc thiết kế; trong code không hoàn toàn tuân thủ: `components/common/perfo-ticket.tsx` hardcode mã HEX (#970013, #FFFDFB, #170C09, #B97F12, …), dùng nhãn viết hoa ("PHIM", "SUẤT CHIẾU", "PHÒNG", "GHẾ", "TỔNG TIỀN", "ĐÃ SOÁT VÉ") và `rx="4"` bo góc ô ghế; và có 1 file .tsx khác còn dùng `rounded-*`/`shadow-*` (đếm bằng grep, chưa xác định file). Bài viết mô tả quy tắc của design system và không khẳng định toàn bộ code tuân thủ; có thể đưa vào Chương 7 (hạn chế) nếu cần.
- Outline 4.9.2: `/booking/:showtimeId` không nằm sau `CustomerProtectedRoute`; việc yêu cầu đăng nhập thực hiện trong `useBookingFlow` (hộp thoại + chuyển tới `/auth/login?redirect=…`). `/account/tickets/:id` nằm trong `CustomerProtectedRoute` nhưng ngoài `AccountLayout`.
- Outline 4.9.3 "phân quyền theo vai trò": ở frontend chỉ có một lớp `AdminProtectedRoute` cho phép cả Admin và Employee vào mọi `/admin/*`; hạn chế theo vai trò chỉ ở menu (`filterNavGroupsByRole`) và biến `canManage` trong `movies-page.tsx`, `showtimes-page.tsx`. Nhân viên gõ trực tiếp URL trang chỉ dành cho quản trị viên vẫn vào được trang (API sẽ từ chối bằng `AdminOnly`). Trang `/403` (`ForbiddenPage`) có khai báo route nhưng chưa kiểm tra có nơi nào chuyển tới. Cũng lưu ý: `GuestRoute` ở `/admin/login` để nguyên trang đăng nhập (không chuyển hướng) khi người đã đăng nhập không phải Admin.
- Outline 4.9.4 "đếm ngược giữ ghế": đồng hồ `BookingHoldTimer` xuất hiện ở bước thanh toán (`BookingPaymentStep`), không phải ở bước chọn ghế; bước chọn ghế chỉ có dòng nhắc thời gian giữ ghế (`holdDurationMinutes`). "Chi tiết đơn hàng" tương ứng `BookingPriceSummary`.
- Outline 4.9.5 mô tả "trạng thái" của vé: trạng thái hiển thị ở huy hiệu `BookingStatusBadge` của đơn đặt vé, và trạng thái đã soát của từng vé qua `isCheckedIn`.
- Outline 4.9.6 "sinh tự động": đã mô tả theo `SeatEditorAutoGenerateForm`/`buildAutoGeneratedCells` (chỉ đọc tham số biểu mẫu, chưa mô tả thuật toán sinh); "bảng kiểm tra hợp lệ" ứng với `validateSeatLayout` chạy ở trình duyệt.
- Outline 4.9.8: API check-in của frontend là `POST {BASE}/checkin` trong `features/admin/bookings/api` (BASE chưa đọc cụ thể); bài viết chỉ ghi "nhóm bookings của quản trị". Chưa xác nhận tên action trong `AdminBookingsController`.
- Outline 4.9.9: mô tả trang dashboard là "theo thời gian thực" trong `PageHeader`, nhưng dashboard không dùng SignalR (dữ liệu lấy bằng truy vấn); bài viết không dùng cụm này.
- Outline 4.10.1 "policy giới hạn tốc độ `auth`, `booking-hold`": đúng; ngoài ra có bộ giới hạn toàn cục (100 yêu cầu/60 giây/IP) mà outline không nêu. `auth` áp dụng cho 7/12 endpoint của AuthController (không gồm confirm-email, logout, resend-confirmation, socket-token, me). Theo yêu cầu riêng, các hạn chế của endpoint AI (xác thực, giới hạn tốc độ) không nêu ở đây mà để cho Mục 7.3.6; bài chỉ nêu `AiController` không khai báo yêu cầu đăng nhập như một nhận xét mô tả chung của nhóm endpoint không yêu cầu đăng nhập.
- Outline 4.10.2: bảng mẫu khớp 100% với số đếm từ code (34 controller, 137 endpoint). Đã bổ sung cột "Tiền tố đường dẫn".
- `PaymentsController` có 3 action nhưng 4 endpoint vì `VnPayIpn` gắn cả `HttpGet` và `HttpPost`; thuộc tính `RawResponse` cũng là 3 action.
-->

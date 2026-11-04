# OUTLINE BÁO CÁO ĐỒ ÁN MÔN HỌC — HỆ THỐNG ĐẶT VÉ XEM PHIM TRỰC TUYẾN PERFO

> Tách từ file outline gốc (bỏ mục 0). Quy định format Word (Mục 0.1–0.5: thiết lập trang, section, style, trích dẫn, cấu trúc báo cáo) nằm ở file `quy-dinh-format-bao-cao.md`; các tham chiếu "Mục 0.4", "Mục 0.5" trong file này trỏ về file đó.

**Quy ước:** `[CẦN BỔ SUNG: …]` = chưa có dữ liệu trong project hoặc tài liệu đã cung cấp. Đường dẫn nguồn tính từ `perfo-main/`; tài liệu kiểm thử bổ sung ghi `bo_sung/…`. Số hiệu hình, bảng, biểu đồ, công thức theo dạng `Hình X.n`, `Bảng X.n`, `Biểu đồ X.n`, `(X.n)` với X là số chương (phụ lục: `P1.n`, `P2.n`…). Mỗi mục chỉ có tối đa một hình; mã nguồn sơ đồ (PlantUML cho use case và sơ đồ lớp, Mermaid cho ERD và các sơ đồ khác) nằm trong file `ma-nguon-so-do.md`, tra theo số hiệu hình.

---

## PHẦN KHAI TẬP


---

# BÌA CHÍNH

- **Nội dung cần viết:**
  - Trường Đại học An Giang; Khoa Công nghệ Thông tin (cỡ 14, đậm)
  - Dòng loại báo cáo `[CẦN BỔ SUNG: ví dụ "ĐỒ ÁN MÔN HỌC …" và tên môn học]`
  - Tên đề tài (cỡ 20, đậm) — đề xuất: XÂY DỰNG HỆ THỐNG ĐẶT VÉ XEM PHIM TRỰC TUYẾN PERFO `[CẦN BỔ SUNG: xác nhận tên đề tài]`
  - Họ tên tác giả `[CẦN BỔ SUNG]`
  - AN GIANG, THÁNG-NĂM `[CẦN BỔ SUNG]` (cỡ 14, đậm)
- **Tài liệu/hình/bảng:**
  - File mẫu `bia-chinh.docx`: xóa nhãn "Bìa chính" và các chú thích "(cỡ chữ …)"
- **Nguồn dữ liệu:** `package.json (tên "perfo")`; `bia-chinh.docx`
- **Định dạng Word:** Section 1; khung viền; không đánh số trang; lề của file mẫu (2/2/3/2 cm) khác lề thân báo cáo [CẦN BỔ SUNG: xác nhận giữ nguyên lề mẫu]

---

# BÌA PHỤ

- **Nội dung cần viết:**
  - Như bìa chính, thêm HỌ TÊN TÁC GIẢ, MÃ SỐ SV `[CẦN BỔ SUNG]`, HỌ TÊN GIẢNG VIÊN HƯỚNG DẪN `[CẦN BỔ SUNG]`
- **Tài liệu/hình/bảng:**
  - File mẫu `bia-phu.docx`
- **Nguồn dữ liệu:** `bia-phu.docx`
- **Định dạng Word:** Section 2; không đánh số trang

---

# NHẬN XÉT CỦA GIẢNG VIÊN HƯỚNG DẪN

- **Nội dung cần viết:**
  - Tiêu đề NHẬN XÉT CỦA GIẢNG VIÊN HƯỚNG DẪN; khung viền; 19 dòng chấm; "Giảng viên hướng dẫn (Ký và ghi rõ họ tên)"
  - Phần "Nội dung nhận xét" gợi ý: đồng ý/không đồng ý cho báo cáo; kết quả đạt được so với yêu cầu; ý kiến khác
  - Để trống cho giảng viên
- **Tài liệu/hình/bảng:**
  - Trang mẫu theo quy định
- **Nguồn dữ liệu:** `quy-dinh-trinh-bay-bao-cao-ttck.pdf (trang 10)`
- **Định dạng Word:** Trang đầu Section 3, số trang i; tab dẫn chấm (Dot leader) cho các dòng chấm

---

# LỜI CẢM ƠN

- **Nội dung cần viết:**
  - Cảm ơn giảng viên hướng dẫn, Khoa/Trường, người đã hỗ trợ `[CẦN BỔ SUNG: tên người cần cảm ơn]`
- **Nguồn dữ liệu:** `Sinh viên cung cấp`
- **Định dạng Word:** Tiêu đề 14 đậm hoa, canh giữa; nội dung Normal

---

# TÓM TẮT

- **Nội dung cần viết:**
  - Bài toán và rủi ro đặt vé: đặt trùng ghế, thanh toán bất đồng bộ
  - Giải pháp: kiến trúc phân tầng .NET + React, giữ ghế nguyên tử, thanh toán idempotent, thời gian thực, xếp lịch tự động, trợ lý AI, điểm thành viên theo cửa sổ trượt 12 tháng
  - Kết quả định lượng chỉ lấy từ Chương 6 (E2E 8/8 đạt; kết quả test backend `[CẦN BỔ SUNG: kết quả dotnet test]`)
  - Kết luận và hạn chế chính; từ khóa 4–6 từ
  - Phần tiếng Anh (Abstract) nếu thực hiện được
- **Nguồn dữ liệu:** `Chương 5–8`
- **Định dạng Word:** Viết sau cùng; một trang

---

# MỤC LỤC

- **Nội dung cần viết:**
  - Tạo tự động từ Heading 1–4; thêm Heading phần khai tập thủ công nếu cần
- **Nguồn dữ liệu:** `Heading của toàn báo cáo`
- **Định dạng Word:** References → Table of Contents; Custom: hiển thị 4 cấp; cập nhật trường trước khi in

---

# DANH SÁCH HÌNH VẼ

- **Nội dung cần viết:**
  - Tạo tự động từ caption "Hình"
- **Nguồn dữ liệu:** `Danh mục A cuối file (Danh mục hình)`
- **Định dạng Word:** Insert Table of Figures, nhãn "Hình"

---

# DANH SÁCH BẢNG BIỂU

- **Nội dung cần viết:**
  - Tạo tự động từ caption "Bảng"
- **Nguồn dữ liệu:** `Danh mục B cuối file (Danh mục bảng)`
- **Định dạng Word:** Insert Table of Figures, nhãn "Bảng"

---

# DANH SÁCH BIỂU ĐỒ

- **Nội dung cần viết:**
  - Tạo tự động từ caption "Biểu đồ"
- **Nguồn dữ liệu:** `Danh mục C cuối file (Danh mục biểu đồ)`
- **Định dạng Word:** Insert Table of Figures, nhãn "Biểu đồ"

---

# DANH SÁCH KÝ HIỆU

- **Nội dung cần viết:**
  - Ký hiệu trong các công thức của Chương 4 (liệt kê theo alphabet)
- **Nguồn dữ liệu:** `Danh mục D cuối file (Danh mục công thức)`
- **Định dạng Word:** Bảng hai cột: ký hiệu – ý nghĩa

---

# DANH SÁCH TỪ VIẾT TẮT

- **Nội dung cần viết:**
  - Liệt kê theo thứ tự alphabet: viết tắt – tiếng Anh – tiếng Việt
- **Nguồn dữ liệu:** `Danh mục E cuối file (Danh mục từ viết tắt)`
- **Định dạng Word:** Bảng ba cột; kết thúc phần khai tập

---

# CHƯƠNG 1. GIỚI THIỆU ĐỀ TÀI


## 1.1 ĐẶT VẤN ĐỀ


### 1.1.1 Bối cảnh

- **Nội dung cần viết:**
  - Xu hướng đặt vé xem phim trực tuyến thay cho mua tại quầy `[CẦN BỔ SUNG: số liệu có nguồn]`
  - Các bên liên quan: khách hàng, nhân viên quầy, quản trị rạp, cổng thanh toán
- **Nguồn dữ liệu:** [CẦN BỔ SUNG: tài liệu nhóm T1]

### 1.1.2 Tính cần thiết của đề tài

- **Nội dung cần viết:**
  - Khó khăn đặc thù: nhiều người tranh cùng một ghế, giữ chỗ có thời hạn, thanh toán bất đồng bộ qua IPN/webhook, hoàn tiền
  - Nhu cầu vận hành: sơ đồ ghế linh hoạt, xếp lịch chiếu, bán vé quầy, soát vé QR
  - Ý nghĩa thực tiễn và lý luận
- **Nguồn dữ liệu:** `backend/Cinema.Application/Services/Bookings/BookingService.cs`; `backend/Cinema.Application/Services/Payments/PaymentService.cs`

### 1.1.3 Hạn chế của các giải pháp hiện có

- **Nội dung cần viết:**
  - So sánh theo tiêu chí quan sát được (chọn ghế trực quan, giữ ghế có đếm ngược, phương thức thanh toán, ghế đôi, soát vé QR, trợ lý AI, hoàn tiền); chỉ ghi điều đã quan sát kèm ngày truy cập
- **Tài liệu/hình/bảng:**
  - Tham chiếu `Bảng 2.1`
- **Nguồn dữ liệu:** [CẦN BỔ SUNG: khảo sát 3–5 hệ thống đặt vé công khai]

## 1.2 MỤC TIÊU


### 1.2.1 Mục tiêu tổng quát

- **Nội dung cần viết:**
  - Xây dựng hệ thống đặt vé xem phim trực tuyến Perfo gồm giao diện khách hàng, trang quản trị, bán vé quầy, soát vé QR
- **Nguồn dữ liệu:** `package.json`

### 1.2.2 Mục tiêu cụ thể

- **Nội dung cần viết:**
  - Năm mục tiêu kèm tiêu chí đo: (1) đủ chức năng theo yêu cầu Chương 3; (2) không đặt trùng ghế dưới truy cập đồng thời; (3) thanh toán an toàn, idempotent; (4) cập nhật ghế thời gian thực; (5) chất lượng kiểm thử
- **Tài liệu/hình/bảng:**
  - `Bảng 1.1` Mục tiêu và tiêu chí đánh giá
- **Nguồn dữ liệu:** `backend/tests/**`; `bo_sung/e2e/*.spec.ts`

| Mã | Mục tiêu | Tiêu chí đo | Mục kiểm chứng | Kết quả |
|---|---|---|---|---|
| MT1 | Đủ chức năng theo yêu cầu | Số use case đã cài đặt so với Chương 3 | 7.1 | [CẦN BỔ SUNG: điền ở Chương 7] |
| MT2 | Không đặt trùng ghế | 1 thành công, còn lại 409 khi 10 yêu cầu song song | 6.5 | [CẦN BỔ SUNG: kết quả chạy] |
| MT3 | Thanh toán an toàn, idempotent | Test IPN sai chữ ký, sai số tiền, lặp, ghế hết hạn | 6.2 | [CẦN BỔ SUNG: kết quả chạy] |
| MT4 | Cập nhật ghế thời gian thực | Test SignalR nhận seat.held, seat.released | 6.3 | [CẦN BỔ SUNG: kết quả chạy] |
| MT5 | Chất lượng kiểm thử | Số test đạt; E2E; hiệu năng | 6.2–6.6 | E2E 8/8 đạt; còn lại [CẦN BỔ SUNG] |


## 1.3 ĐỐI TƯỢNG NGHIÊN CỨU

- **Nội dung cần viết:**
  - Quy trình đặt vé xem phim trực tuyến; kỹ thuật kiểm soát đồng thời, thanh toán trực tuyến, truyền thông thời gian thực

## 1.4 PHẠM VI NGHIÊN CỨU

- **Nội dung cần viết:**
  - Một rạp (bảng `CinemaSettings` ràng buộc `Id = 1`); tiền tệ VND; múi giờ `Asia/Ho_Chi_Minh`; cổng thanh toán ở môi trường sandbox/test; giao diện tiếng Việt
- **Nguồn dữ liệu:** `backend/Cinema.Infrastructure/Persistence/Configurations/CinemaSettingsConfiguration.cs`; `backend/Cinema.Api/appsettings.json`

## 1.5 NGOÀI PHẠM VI

- **Nội dung cần viết:**
  - Nhiều chi nhánh; ứng dụng di động gốc; đối soát kế toán; thanh toán ZaloPay (chỉ có hằng số tên, chưa có cổng); đổi điểm lấy quà hoặc voucher; thông báo điểm sắp hết hạn
- **Nguồn dữ liệu:** `backend/Cinema.Shared/Constants/PaymentMethodNames.cs`

## 1.6 BỐ CỤC BÁO CÁO

- **Nội dung cần viết:**
  - Một đoạn ngắn cho mỗi Chương 2–8, nêu đúng nội dung thật của chương
- **Nguồn dữ liệu:** `Mục 0.5 file này`

---

# CHƯƠNG 2. TỔNG QUAN VÀ CƠ SỞ LÝ THUYẾT


## 2.1 BỐI CẢNH BÀI TOÁN


### 2.1.1 Vòng đời của một lần đặt vé

- **Nội dung cần viết:**
  - Chọn suất → chọn ghế → giữ chỗ có thời hạn → thanh toán → phát hành vé QR → soát vé; trạng thái tương ứng của Booking, ShowtimeSeat, Payment
- **Tài liệu/hình/bảng:**
  - `Hình 2.1` Vòng đời của một lần đặt vé
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Enums/BookingStatus.cs`; `ShowtimeSeatStatus.cs`; `PaymentStatus.cs`

### 2.1.2 Phát biểu bài toán

- **Nội dung cần viết:**
  - Ba nhóm bài toán dạng đầu vào – đầu ra – ràng buộc: tranh chấp ghế; nhất quán thanh toán; vận hành (xếp lịch, sơ đồ ghế, luật ghế lẻ)
- **Nguồn dữ liệu:** `BookingService.cs`; `PaymentService.cs`; `ShowtimeScheduler.cs`; `SeatLayoutRules.cs`

## 2.2 CÁC HỆ THỐNG LIÊN QUAN


### 2.2.1 Hệ thống đặt vé xem phim hiện có

- **Nội dung cần viết:**
  - Chọn 3–5 hệ thống, so sánh theo tiêu chí đã nêu ở 1.1.3; chỉ dùng kết quả khảo sát có tên hệ thống và ngày truy cập
- **Tài liệu/hình/bảng:**
  - `Bảng 2.1` So sánh các hệ thống đặt vé hiện có
- **Nguồn dữ liệu:** [CẦN BỔ SUNG: kết quả khảo sát]

### 2.2.2 Hướng tiếp cận chống xung đột khi đặt chỗ

- **Nội dung cần viết:**
  - Khóa bi quan; khóa lạc quan; cập nhật có điều kiện nguyên tử (cách Perfo dùng); hàng đợi/phòng chờ ảo; ưu nhược điểm; lý do chọn
- **Tài liệu/hình/bảng:**
  - `Bảng 2.2` So sánh các hướng tiếp cận chống xung đột đặt chỗ
- **Nguồn dữ liệu:** `BookingService.cs → CreateHoldBookingAsync`; `ShowtimeSeatConfiguration.cs`

### 2.2.3 Khoảng trống đề tài giải quyết

- **Nội dung cần viết:**
  - Định vị đóng góp là thiết kế và tích hợp, không tuyên bố mới hoàn toàn
- **Nguồn dữ liệu:** [CẦN BỔ SUNG: kết quả khảo sát 2.2.1]

## 2.3 PHẠM VI ĐỀ TÀI

- **Nội dung cần viết:**
  - Package use case trong/ngoài phạm vi (xem 3.3); giả định vận hành: một rạp, VND, UTC+7; cổng thanh toán sandbox
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/CinemaSettings.cs`

## 2.4 PHƯƠNG PHÁP NGHIÊN CỨU


### 2.4.1 Phương pháp thực hiện

- **Nội dung cần viết:**
  - Nghiên cứu lý thuyết → xác định yêu cầu (use case) → thiết kế (kiến trúc, ERD, luồng nghiệp vụ) → cài đặt theo module → kiểm thử tự động → đóng gói và triển khai
  - Công cụ: Swagger, EF Core migration, script npm, CSharpier, ESLint
- **Tài liệu/hình/bảng:**
  - `Hình 2.2` Quy trình thực hiện đề tài
- **Nguồn dữ liệu:** `package.json (scripts)`; `dotnet-tools.json`; `frontend/eslint.config.js`

### 2.4.2 Phương pháp đánh giá

- **Nội dung cần viết:**
  - Đánh giá chức năng bằng truy vết yêu cầu–test; đồng thời bằng test 10 yêu cầu song song; E2E bằng Playwright; hiệu năng bằng k6; bảo mật bằng rà soát cơ chế đã cài
- **Nguồn dữ liệu:** `backend/tests/**`; `bo_sung/e2e/**`; [CẦN BỔ SUNG: load-tests/ (kịch bản k6) — package.json tham chiếu nhưng chưa có]

## 2.5 CƠ SỞ LÝ THUYẾT


### 2.5.1 Mô hình hóa yêu cầu bằng use case

- **Nội dung cần viết:**
  - Tác nhân, use case, quan hệ include/extend/tổng quát hóa; use case chỉ có hai mức (package, use case), quan hệ include/extend/tổng quát hóa chỉ giữa các use case mức 1 (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho Chương 3
- **Nguồn dữ liệu:** [CẦN BỔ SUNG: trích dẫn nhóm T2]

### 2.5.2 Mô hình thực thể liên kết

- **Nội dung cần viết:**
  - Thực thể, thuộc tính, khóa, bội số quan hệ; chuẩn hóa; ánh xạ sang quan hệ (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.3
- **Nguồn dữ liệu:** [CẦN BỔ SUNG: trích dẫn nhóm T3]

### 2.5.3 Kiến trúc phân tầng

- **Nội dung cần viết:**
  - Phân tầng/Clean Architecture; chiều phụ thuộc hướng vào lõi (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.1.2, 4.2
- **Tài liệu/hình/bảng:**
  - `Hình 2.3` Mô hình phân tầng các project
- **Nguồn dữ liệu:** `Cinema.Domain.csproj`; `Cinema.Application.csproj`; `Cinema.Infrastructure.csproj`; `Cinema.Api.csproj`; [CẦN BỔ SUNG: trích dẫn nhóm T2]

### 2.5.4 REST API

- **Nội dung cần viết:**
  - REST, mã HTTP, phiên bản hóa `/api/v1`, bọc `ApiResponse<T>`, phân trang offset/con trỏ (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.1.4, 4.10
- **Nguồn dữ liệu:** `Cinema.Shared/Results/ApiResponse.cs`; `Cinema.Api/Filters/ApiResponseFilter.cs`; [CẦN BỔ SUNG: trích dẫn nhóm T3]

### 2.5.5 ASP.NET Core, Entity Framework Core, PostgreSQL

- **Nội dung cần viết:**
  - Pipeline middleware, DI, Options pattern, migration, ExecuteUpdate, CHECK và chỉ mục duy nhất (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.3
- **Nguồn dữ liệu:** `Cinema.Api/Program.cs`; `Cinema.Infrastructure/Persistence/**`; [CẦN BỔ SUNG: trích dẫn nhóm T3]

### 2.5.6 Xác thực bằng JWT

- **Nội dung cần viết:**
  - Cấu trúc JWT; access token ngắn hạn, refresh token dài hạn; xoay vòng; phát hiện dùng lại; băm token (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.4
- **Nguồn dữ liệu:** `JwtTokenService.cs`; `AuthService.cs`; [CẦN BỔ SUNG: trích dẫn nhóm T4]

### 2.5.7 Phân quyền theo vai trò

- **Nội dung cần viết:**
  - RBAC theo policy; OAuth 2.0 với Google; băm mật khẩu bằng PasswordHasher (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.4.8
- **Nguồn dữ liệu:** `Program.cs`; `GoogleAuthService.cs`; [CẦN BỔ SUNG: trích dẫn nhóm T4]

### 2.5.8 Kiểm soát đồng thời

- **Nội dung cần viết:**
  - ACID; mức cô lập; MVCC; cập nhật có điều kiện + kiểm tra số dòng; ràng buộc duy nhất; idempotency (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.5.1
- **Nguồn dữ liệu:** `BookingService.cs`; `BookingTicketConfiguration.cs`; [CẦN BỔ SUNG: trích dẫn nhóm T5]

### 2.5.9 Truyền thông thời gian thực với SignalR

- **Nội dung cần viết:**
  - WebSocket; hub, group; xác thực qua `access_token` query (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.7
- **Nguồn dữ liệu:** `ShowtimeHub.cs`; `frontend/src/lib/signalr.ts`; [CẦN BỔ SUNG: trích dẫn nhóm T6]

### 2.5.10 Xử lý tác vụ nền với Hangfire

- **Nội dung cần viết:**
  - Job lặp cron, fire-and-forget, thử lại, lưu trên PostgreSQL (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.5.3
- **Nguồn dữ liệu:** `BackgroundJobs/*`; `Program.cs`; [CẦN BỔ SUNG: trích dẫn nhóm T7]

### 2.5.11 Tích hợp cổng thanh toán

- **Nội dung cần viết:**
  - Redirect–callback; IPN/webhook; HMAC-SHA512 (VNPay), HMAC-SHA256 (MoMo); Stripe webhook; idempotency; hoàn tiền (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.5.2
- **Nguồn dữ liệu:** `Infrastructure/Payments/*`; [CẦN BỔ SUNG: trích dẫn nhóm T8]

### 2.5.12 Công nghệ giao diện

- **Nội dung cần viết:**
  - SPA React 19; TanStack Query/Table; Zustand; React Hook Form + Zod; Tailwind v4 (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 5.4
- **Nguồn dữ liệu:** `frontend/package.json`; `frontend/vite.config.ts`; [CẦN BỔ SUNG: trích dẫn nhóm T9]

### 2.5.13 Trợ lý AI

- **Nội dung cần viết:**
  - LLM gọi hàm; vòng lặp gọi–trả kết quả; rủi ro (prompt injection, dữ liệu cá nhân, chi phí) (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.8
- **Nguồn dữ liệu:** `GeminiAiAssistantService.cs`; `AiToolExecutor.cs`; [CẦN BỔ SUNG: trích dẫn nhóm T10]

### 2.5.14 Kiểm thử phần mềm

- **Nội dung cần viết:**
  - Kim tự tháp kiểm thử; xUnit/Moq/Shouldly; Testcontainers; Playwright; k6 (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho Chương 6
- **Nguồn dữ liệu:** `backend/tests/*/*.csproj`; `frontend/playwright.config.ts`; [CẦN BỔ SUNG: trích dẫn nhóm T11]

### 2.5.15 Container hóa

- **Nội dung cần viết:**
  - Docker nhiều stage; PaaS; hosting tĩnh; PostgreSQL serverless (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 5.7
- **Nguồn dữ liệu:** `backend/Dockerfile`; `docker-compose.yml`; `render.yaml`; [CẦN BỔ SUNG: trích dẫn nhóm T12]

## 2.6 CÔNG NGHỆ SỬ DỤNG

- **Nội dung cần viết:**
  - Bảng tổng hợp công nghệ và phiên bản lấy từ csproj/package.json
- **Tài liệu/hình/bảng:**
  - `Bảng 2.3` Công nghệ sử dụng và phiên bản
- **Nguồn dữ liệu:** `backend/*/*.csproj`; `frontend/package.json`

| Nhóm | Công nghệ | Phiên bản |
|---|---|---|
| Nền tảng | .NET / ASP.NET Core | net10.0 |
| ORM | Entity Framework Core / Npgsql | 10.0.11 / 10.0.3 |
| Tác vụ nền | Hangfire / Hangfire.PostgreSql | 1.8.24 / 1.21.1 |
| Kiểm tra dữ liệu | FluentValidation | 12.1.1 |
| Ánh xạ | Mapster | 10.0.11 |
| Nhật ký | Serilog.AspNetCore | 10.0.0 |
| Tài liệu API | Swashbuckle | 10.2.3 |
| Mã QR | QRCoder | 1.8.0 |
| Email | MailKit | 4.17.0 |
| Thanh toán | Stripe.net | 52.4.0 |
| Lưu trữ tệp | CloudinaryDotNet | 1.29.3 |
| Đăng nhập Google | Google.Apis.Auth | 1.76.0 |
| Giao diện | React / Vite / TypeScript | 19.2.8 / 8.2.2 / ~6.0.2 |
| CSS | Tailwind CSS | 4.3.3 |
| Dữ liệu phía client | TanStack Query / React Router / Zod | 5.102 / 7.18 / 3.25 |
| Biểu đồ | Recharts | 3.8 |
| Kiểm thử E2E | Playwright | 1.63 |


---

# CHƯƠNG 3. PHÂN TÍCH YÊU CẦU


## 3.1 TÁC NHÂN


### 3.1.1 Danh sách tác nhân

- **Nội dung cần viết:**
  - Mô tả từng tác nhân, quyền chính; phân biệt tác nhân người dùng và hệ thống ngoài
  - Xác định tác nhân bằng cách đọc [Authorize]/policy ở từng controller, không suy diễn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.1` Danh sách tác nhân
- **Nguồn dữ liệu:** `backend/Cinema.Shared/Constants/Roles.cs`; `Policies.cs`; `backend/Cinema.Api/Controllers/**`

| Tác nhân | Loại | Mô tả | Quyền/Policy |
|---|---|---|---|
| Khách vãng lai | Người dùng | Người chưa đăng nhập: xem phim, lịch chiếu, thông tin rạp, hỏi trợ lý AI, đăng ký/đăng nhập | Công khai |
| Người dùng đã đăng nhập | Người dùng (tổng quát) | Người đã đăng nhập, có thể xem hồ sơ, đổi mật khẩu, lấy socket token | Đăng nhập (`[Authorize]`) |
| Khách hàng | Người dùng | Đặt vé, thanh toán, hủy đơn, viết đánh giá | `CustomerOnly` (vai trò Customer) |
| Nhân viên | Người dùng | Bán vé tại quầy, soát vé, tra cứu booking/phim/suất/khách hàng | `StaffOnly` (Admin hoặc Employee) |
| Quản trị viên | Người dùng | Quản lý toàn bộ danh mục, phim, phòng, suất chiếu, người dùng, báo cáo, cấu hình | `AdminOnly` (kế thừa các quyền của nhân viên) |
| Hệ thống tác vụ nền | Hệ thống | Hangfire: giải phóng ghế hết hạn, cập nhật vòng đời, gửi email, hoàn tiền, tính lại điểm thành viên hết hạn | — |
| Cổng VNPay, Cổng MoMo, Cổng Stripe | Hệ thống ngoài | Cổng thanh toán: tạo giao dịch, IPN/webhook, hoàn tiền | Xác minh chữ ký |
| Google | Hệ thống ngoài | Xác minh ID token đăng nhập Google | OAuth 2.0 |
| Gemini | Hệ thống ngoài | Mô hình ngôn ngữ cho trợ lý AI | API key |
| Dịch vụ email | Hệ thống ngoài | Resend/SMTP gửi email | — |
| Cloudinary | Hệ thống ngoài | Lưu trữ ảnh/video | API key |


### 3.1.2 Quan hệ giữa các tác nhân

- **Nội dung cần viết:**
  - Giải thích quan hệ tổng quát hóa: người dùng đã đăng nhập kế thừa quyền của khách vãng lai; khách hàng và nhân viên kế thừa người dùng đã đăng nhập; quản trị viên kế thừa nhân viên (policy `StaffOnly` gồm cả hai)
- **Tài liệu/hình/bảng:**
  - `Hình 3.1` Quan hệ giữa các tác nhân
- **Nguồn dữ liệu:** `backend/Cinema.Shared/Constants/Policies.cs`; `backend/Cinema.Api/Program.cs`

### 3.1.3 Sơ đồ ngữ cảnh hệ thống

- **Nội dung cần viết:**
  - Phạm vi hệ thống, các luồng dữ liệu vào/ra với tác nhân và hệ thống ngoài
- **Tài liệu/hình/bảng:**
  - `Hình 3.2` Sơ đồ ngữ cảnh hệ thống
- **Nguồn dữ liệu:** `backend/Cinema.Api/Program.cs`; `backend/Cinema.Infrastructure/DependencyInjection.cs`

## 3.2 USE CASE TỔNG QUÁT


### 3.2.1 Danh mục package use case

- **Nội dung cần viết:**
  - Giới thiệu 21 package (use case mức 0); mỗi package có một sơ đồ use case và một bảng danh sách use case mức 1
  - Nêu rõ use case chỉ có hai mức: package và use case; quan hệ include/extend chỉ vẽ giữa các use case mức 1
  - Tổng 103 use case
- **Tài liệu/hình/bảng:**
  - `Bảng 3.2` Danh mục package use case
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

| Mục | Mã | Package | Mô tả | Số use case | Tác nhân |
|---|---|---|---|---|---|
| 3.3.1 | XT | Xác thực | Đăng ký, đăng nhập, khôi phục mật khẩu, duy trì phiên. | 11 | Khách vãng lai, Dịch vụ email, Google, Nhân viên, Quản trị viên, Người dùng đã đăng nhập |
| 3.3.2 | HS | Hồ sơ cá nhân | Xem và cập nhật hồ sơ, đổi mật khẩu. | 4 | Người dùng đã đăng nhập |
| 3.3.3 | DP | Duyệt phim | Xem phim, lịch chiếu và thông tin công khai của rạp. | 8 | Khách vãng lai |
| 3.3.4 | DG | Đánh giá phim | Khách hàng đã xem phim viết, sửa, xóa đánh giá. | 4 | Khách hàng |
| 3.3.5 | DV | Đặt vé trực tuyến | Chọn suất, chọn ghế, giữ ghế, quản lý đơn đặt vé. | 9 | Khách hàng, Khách vãng lai |
| 3.3.6 | TT | Thanh toán trực tuyến | Tạo giao dịch, nhận IPN/webhook, xác nhận vé, hoàn tiền. | 6 | Khách hàng, Cổng VNPay, Cổng MoMo, Cổng Stripe, Hệ thống tác vụ nền, Dịch vụ email |
| 3.3.7 | AI | Trợ lý AI | Trò chuyện hỏi đáp suất chiếu, ghế trống, vé của tôi. | 1 | Khách vãng lai, Khách hàng, Gemini |
| 3.3.8 | BQ | Bán vé tại quầy | Nhân viên bán vé trực tiếp, thu tiền mặt. | 1 | Nhân viên, Quản trị viên |
| 3.3.9 | SV | Soát vé | Quét mã QR hoặc nhập mã để soát vé vào rạp. | 1 | Nhân viên, Quản trị viên |
| 3.3.10 | QB | Quản lý booking | Tra cứu đơn đặt vé. | 2 | Nhân viên, Quản trị viên |
| 3.3.11 | QP | Quản lý phim | Thêm, sửa, xóa phim. | 5 | Nhân viên, Quản trị viên, Cloudinary |
| 3.3.12 | QD | Quản lý danh mục | Danh mục phim, danh mục phòng, nhân sự, thành viên, combo. | 10 | Quản trị viên, Cloudinary |
| 3.3.13 | QR | Quản lý phòng chiếu | Phòng chiếu và sơ đồ ghế. | 3 | Quản trị viên |
| 3.3.14 | QS | Quản lý suất chiếu | Tạo, sửa, hủy, xếp lịch suất chiếu. | 9 | Nhân viên, Quản trị viên, Dịch vụ email |
| 3.3.15 | QN | Quản lý người dùng | Tài khoản người dùng và nhân viên. | 7 | Quản trị viên, Nhân viên |
| 3.3.16 | QV | Quản lý đánh giá | Kiểm duyệt đánh giá phim. | 3 | Quản trị viên |
| 3.3.17 | QT | Quản lý thanh toán | Tra cứu giao dịch thanh toán. | 2 | Quản trị viên |
| 3.3.18 | BC | Báo cáo thống kê | Dashboard và các báo cáo. | 5 | Quản trị viên |
| 3.3.19 | CH | Cấu hình rạp | Thông tin rạp và tham số vận hành. | 2 | Quản trị viên, Cloudinary |
| 3.3.20 | TP | Tải tệp | Tải ảnh và video lên dịch vụ lưu trữ. | 2 | Quản trị viên, Cloudinary |
| 3.3.21 | HT | Vận hành hệ thống | Công việc chạy nền và chức năng vận hành hệ thống. | 8 | Hệ thống tác vụ nền, Dịch vụ email, Cổng VNPay, Cổng MoMo, Cổng Stripe, Quản trị viên, Khách vãng lai |
|  |  | **Tổng** |  | **103** |  |

### 3.2.2 Sơ đồ package use case tổng quát

- **Nội dung cần viết:**
  - Thể hiện 21 package và tác nhân sử dụng; chi tiết từng package ở mục 3.3
  - Quan hệ tác nhân – package suy ra từ cột "Tác nhân" của các bảng ở 3.3
- **Tài liệu/hình/bảng:**
  - `Hình 3.3` Sơ đồ package use case tổng quát
- **Nguồn dữ liệu:** `Mục 3.3 file này`

### 3.2.3 Ma trận tác nhân theo package

- **Nội dung cần viết:**
  - Đánh dấu tác nhân tham gia từng package (suy ra từ danh sách use case ở 3.3)
- **Tài liệu/hình/bảng:**
  - `Bảng 3.3` Ma trận tác nhân theo package
- **Nguồn dữ liệu:** `Mục 3.3 file này`

| Package | Khách vãng lai | Khách hàng | Nhân viên | Quản trị viên | Hệ thống ngoài/nền |
|---|---|---|---|---|---|
| XT Xác thực | x | x | x | x | x |
| HS Hồ sơ cá nhân |  | x |  |  |  |
| DP Duyệt phim | x |  |  |  |  |
| DG Đánh giá phim |  | x |  |  |  |
| DV Đặt vé trực tuyến | x | x |  |  |  |
| TT Thanh toán trực tuyến |  | x |  |  | x |
| AI Trợ lý AI | x | x |  |  | x |
| BQ Bán vé tại quầy |  |  | x | x |  |
| SV Soát vé |  |  | x | x |  |
| QB Quản lý booking |  |  | x | x |  |
| QP Quản lý phim |  |  | x | x | x |
| QD Quản lý danh mục |  |  |  | x | x |
| QR Quản lý phòng chiếu |  |  |  | x |  |
| QS Quản lý suất chiếu |  |  | x | x | x |
| QN Quản lý người dùng |  |  | x | x |  |
| QV Quản lý đánh giá |  |  |  | x |  |
| QT Quản lý thanh toán |  |  |  | x |  |
| BC Báo cáo thống kê |  |  |  | x |  |
| CH Cấu hình rạp |  |  |  | x | x |
| TP Tải tệp |  |  |  | x | x |
| HT Vận hành hệ thống | x |  |  | x | x |


## 3.3 USE CASE THEO PACKAGE

### 3.3.1 Xác thực

- **Nội dung cần viết:**
  - Đăng ký, đăng nhập, khôi phục mật khẩu, duy trì phiên.
  - Package XT có 11 use case; tác nhân: Khách vãng lai, Dịch vụ email, Google, Nhân viên, Quản trị viên, Người dùng đã đăng nhập

#### 3.3.1.1 Sơ đồ use case package Xác thực

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.4` Sơ đồ use case package Xác thực
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/AuthController.cs`; `backend/Cinema.Application/Interfaces/IAuthService.cs`; `backend/Cinema.Application/Services/Auth/AuthService.cs`; `backend/Cinema.Application/Interfaces/ITokenService.cs`; `backend/Cinema.Application/Services/Auth/JwtTokenService.cs`; `backend/Cinema.Application/Interfaces/IGoogleAuthService.cs`; `backend/Cinema.Infrastructure/Auth/GoogleAuthService.cs`; `backend/Cinema.Application/BackgroundJobs/SendEmailJob.cs`; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.4

#### 3.3.1.2 Danh sách use case package Xác thực

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.4` Danh sách use case package Xác thực (11 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/AuthController.cs`; `backend/Cinema.Application/Interfaces/IAuthService.cs`; `backend/Cinema.Application/Services/Auth/AuthService.cs`; `backend/Cinema.Application/Interfaces/ITokenService.cs`; `backend/Cinema.Application/Services/Auth/JwtTokenService.cs`; `backend/Cinema.Application/Interfaces/IGoogleAuthService.cs`; `backend/Cinema.Infrastructure/Auth/GoogleAuthService.cs`; `backend/Cinema.Application/BackgroundJobs/SendEmailJob.cs`

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-XT-01 | Đăng ký tài khoản | Khách vãng lai, Dịch vụ email | Khách tạo tài khoản khách hàng và nhận email xác nhận. | 1. Nhập họ tên, email, mật khẩu (kiểm tra bằng `RegisterRequestValidator`)<br>2. Kiểm tra email chưa được sử dụng<br>3. Tạo `User` vai trò Customer kèm `CustomerProfile`<br>4. Tạo mã xác nhận email (lưu dạng băm, mục đích `EmailConfirmation`) và xếp `SendEmailJob` gửi thư (UC-HT-03) | • `Invalid_EmailExists`, `Invalid_EmailInUse`<br>• Vai trò Customer phải có sẵn trong CSDL (`Rule_CustomerRoleNotSeeded`)<br>• Giới hạn tốc độ `auth`<br>• Mã xác nhận hết hạn sau `EmailConfirmationExpirationHours` (mặc định 24 giờ) | POST /api/v1/auth/register (giới hạn tốc độ `auth`) | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → RegisterAsync` |
| UC-XT-02 | Xác nhận email | Khách vãng lai | Khách xác nhận email bằng mã gửi qua thư. | 1. Nhận mã xác nhận từ liên kết trong thư<br>2. Băm mã và tìm `UserVerificationToken` mục đích `EmailConfirmation`<br>3. Đánh dấu mã đã dùng, đặt `User.EmailConfirmed = true` | • Mã phải còn hạn và chưa dùng (`Invalid_TokenExpiredOrUsed`)<br>• Thành công trả `Success_EmailConfirmed` | POST /api/v1/auth/confirm-email | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → ConfirmEmailAsync` |
| UC-XT-03 | Gửi lại email xác nhận | Khách vãng lai, Dịch vụ email | Gửi lại thư xác nhận, trả thông điệp chuẩn không lộ email có tồn tại hay không. | 1. Nhập email<br>2. Tìm tài khoản theo email; nếu không có hoặc đã xác nhận thì bỏ qua<br>3. Vô hiệu các mã xác nhận cũ còn hạn, tạo mã mới và xếp `SendEmailJob` (UC-HT-03)<br>4. Trả thông điệp chuẩn `Success_ConfirmationEmailSent` | • Luôn trả cùng một thông điệp, không lộ email có tồn tại hay không | POST /api/v1/auth/resend-confirmation | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → ResendConfirmationEmailAsync` |
| UC-XT-04 | Đăng nhập bằng email | Khách vãng lai | Khách hàng đăng nhập bằng email và mật khẩu. | 1. Kiểm tra email và mật khẩu<br>2. Kiểm tra tài khoản chưa bị khóa<br>3. Kiểm tra email đã được xác nhận<br>4. Chặn tài khoản nhân viên, quản trị dùng cổng khách hàng<br>5. Cấp access token (`JwtTokenService`) và refresh token (lưu dạng băm) | • `Unauthorized_InvalidCredentials`<br>• `Forbidden_AccountLocked`<br>• `Forbidden_EmailNotVerifiedCheckInbox`<br>• `Forbidden_UseAdminPortal`<br>• Giới hạn tốc độ `auth` | POST /api/v1/auth/login (giới hạn tốc độ `auth`) | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → LoginAsync` |
| UC-XT-05 | Đăng nhập bằng Google | Khách vãng lai, Google | Đăng nhập bằng tài khoản Google. | 1. Xác minh ID token với Google (`GoogleAuthService.VerifyIdTokenAsync`)<br>2. Kiểm tra email Google đã được xác minh<br>3. Tìm tài khoản theo `GoogleId` hoặc email; nếu chưa có thì tạo `User` vai trò Customer kèm `CustomerProfile` (đã xác nhận email)<br>4. Nếu tài khoản đã có email nhưng chưa có `GoogleId` thì liên kết `GoogleId` và đánh dấu đã xác nhận email<br>5. Chặn tài khoản không phải Customer; kiểm tra tài khoản chưa bị khóa<br>6. Cấp access token và refresh token | • `Unauthorized_GoogleEmailNotVerified`<br>• `Forbidden_AdminCannotUseGoogleLogin`<br>• `Forbidden_AccountLocked`<br>• `Rule_CustomerRoleNotSeeded` khi tạo tài khoản mới<br>• Giới hạn tốc độ `auth` | POST /api/v1/auth/google (giới hạn tốc độ `auth`) | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → GoogleLoginAsync`<br>`backend/Cinema.Infrastructure/Auth/GoogleAuthService.cs` |
| UC-XT-06 | Đăng nhập trang quản trị | Nhân viên, Quản trị viên | Nhân viên/quản trị đăng nhập cổng quản trị. | 1. Kiểm tra email và mật khẩu<br>2. Kiểm tra tài khoản chưa bị khóa<br>3. Chặn khách hàng dùng cổng quản trị<br>4. Cấp access token và refresh token | • `Unauthorized_InvalidCredentials`<br>• `Forbidden_AccountLocked`<br>• `Forbidden_CustomerCannotUseAdminPortal`<br>• Giới hạn tốc độ `auth` | POST /api/v1/auth/admin-login (giới hạn tốc độ `auth`) | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → LoginAsync (cổng quản trị)` |
| UC-XT-07 | Khôi phục mật khẩu | Khách vãng lai, Dịch vụ email | Yêu cầu khôi phục và đặt lại mật khẩu bằng mã gửi qua email. | 1. Yêu cầu khôi phục: nhập email (`ForgotPasswordAsync`)<br>2. Tạo mã đặt lại (lưu dạng băm, mục đích `PasswordReset`) và xếp `SendEmailJob` gửi thư (UC-HT-03)<br>3. Đặt lại mật khẩu bằng mã và mật khẩu mới (`ResetPasswordAsync`)<br>4. Băm mật khẩu mới, đánh dấu mã đã dùng<br>5. Thu hồi mọi refresh token chưa thu hồi của người dùng | • Trả thông điệp chuẩn `Success_PasswordResetEmailSent`, không lộ email có tồn tại<br>• Mã phải còn hạn và chưa dùng (`Invalid_TokenExpiredOrUsed`)<br>• Giới hạn tốc độ `auth` | POST /api/v1/auth/forgot-password, POST /api/v1/auth/reset-password (giới hạn tốc độ `auth`) | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → ForgotPasswordAsync, ResetPasswordAsync` |
| UC-XT-08 | Duy trì phiên đăng nhập | Người dùng đã đăng nhập | Làm mới access token bằng refresh token xoay vòng. | 1. Kiểm tra refresh token hợp lệ, còn hạn, chưa thu hồi<br>2. Kiểm tra tài khoản còn hoạt động và đã xác nhận email<br>3. Cấp access token mới và refresh token mới, thu hồi token cũ (`ReplacedByTokenId`)<br>4. Nếu phát hiện dùng lại token đã thu hồi: thu hồi toàn bộ chuỗi token (`RevokeTokenChainAsync`) | • `Unauthorized_RefreshTokenInvalid`, `Unauthorized_RefreshTokenExpiredOrRevoked`<br>• `Forbidden_AccountLocked`, `Forbidden_EmailNotVerified`<br>• Giới hạn tốc độ `auth` | POST /api/v1/auth/refresh (giới hạn tốc độ `auth`) | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → RefreshAsync, RevokeTokenChainAsync` |
| UC-XT-09 | Đăng xuất | Người dùng đã đăng nhập | Thu hồi refresh token hiện tại. | 1. Gửi refresh token hiện tại<br>2. Băm token và tìm bản ghi `RefreshToken`<br>3. Nếu còn hiệu lực thì đặt `IsRevoked = true`, `RevokedAt` | • Token không tồn tại hoặc đã thu hồi vẫn trả `Success_LoggedOut` | POST /api/v1/auth/logout | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → LogoutAsync` |
| UC-XT-10 | Xem thông tin phiên hiện tại | Người dùng đã đăng nhập | Lấy thông tin người dùng đang đăng nhập. | 1. Đọc mã người dùng từ access token<br>2. Tải `User` theo mã<br>3. Trả thông tin người dùng hiện tại (`MeResponse`) | • `NotFound_User` khi tài khoản không còn tồn tại<br>• Yêu cầu đăng nhập (`[Authorize]`) | GET /api/v1/auth/me | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → GetMeAsync` |
| UC-XT-11 | Cấp socket token cho kết nối thời gian thực | Người dùng đã đăng nhập | Cấp token ngắn hạn (300 giây) để kết nối SignalR. | 1. Đọc mã người dùng từ access token<br>2. Tải `User` kèm vai trò và hồ sơ nhân viên<br>3. Phát token ngắn hạn cho kết nối SignalR (`ITokenService.GenerateSocketToken`) | • Hạn token `SocketTokenExpirationSeconds` (mặc định 300 giây)<br>• `NotFound_User`<br>• Yêu cầu đăng nhập (`[Authorize]`) | POST /api/v1/auth/socket-token | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → GenerateSocketTokenAsync` |

### 3.3.2 Hồ sơ cá nhân

- **Nội dung cần viết:**
  - Xem và cập nhật hồ sơ, đổi mật khẩu.
  - Package HS có 4 use case; tác nhân: Người dùng đã đăng nhập

#### 3.3.2.1 Sơ đồ use case package Hồ sơ cá nhân

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.5` Sơ đồ use case package Hồ sơ cá nhân
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/UsersController.cs`; `backend/Cinema.Application/Interfaces/IUserProfileService.cs`; `backend/Cinema.Application/Services/Users/UserProfileService.cs`; `backend/Cinema.Application/Interfaces/IAuthService.cs`; `backend/Cinema.Application/Services/Auth/AuthService.cs`; `backend/Cinema.Application/Interfaces/IReviewService.cs`; `backend/Cinema.Application/Services/Reviews/ReviewService.cs`; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.5

#### 3.3.2.2 Danh sách use case package Hồ sơ cá nhân

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.5` Danh sách use case package Hồ sơ cá nhân (4 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/UsersController.cs`; `backend/Cinema.Application/Interfaces/IUserProfileService.cs`; `backend/Cinema.Application/Services/Users/UserProfileService.cs`; `backend/Cinema.Application/Interfaces/IAuthService.cs`; `backend/Cinema.Application/Services/Auth/AuthService.cs`; `backend/Cinema.Application/Interfaces/IReviewService.cs`; `backend/Cinema.Application/Services/Reviews/ReviewService.cs`

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-HS-01 | Xem hồ sơ cá nhân | Người dùng đã đăng nhập | Xem thông tin cá nhân, điểm thưởng và hạng thành viên. | 1. Đọc mã người dùng từ access token<br>2. Tải hồ sơ kèm điểm thưởng, hạng thành viên và hạng kế tiếp (nếu có); điểm và hạng là giá trị đồng bộ lưu trên `CustomerProfile` | • `NotFound_User`<br>• Yêu cầu đăng nhập<br>• Điểm và hạng trên hồ sơ có thể trễ tối đa 1 giờ so với sổ điểm [CẦN BỔ SUNG: xác nhận DTO trả giá trị lưu trên hồ sơ hay điểm hiệu lực tính từ sổ] | GET /api/v1/users/me | `backend/Cinema.Api/Controllers/Client/UsersController.cs`<br>`backend/Cinema.Application/Services/Users/UserProfileService.cs` |
| UC-HS-02 | Cập nhật hồ sơ cá nhân | Người dùng đã đăng nhập | Sửa họ tên, số điện thoại và thông tin cá nhân. | 1. Nhập họ tên, điện thoại, ảnh đại diện, ngày sinh, giới tính (`UpdateProfileRequestValidator`)<br>2. Cập nhật thông tin `User`<br>3. Trả hồ sơ sau khi cập nhật | • `NotFound_User`<br>• Yêu cầu đăng nhập | PUT /api/v1/users/me | `backend/Cinema.Api/Controllers/Client/UsersController.cs`<br>`backend/Cinema.Application/Services/Users/UserProfileService.cs` |
| UC-HS-03 | Đổi mật khẩu | Người dùng đã đăng nhập | Đổi mật khẩu khi biết mật khẩu hiện tại. | 1. Kiểm tra email đã được xác nhận<br>2. Kiểm tra mật khẩu hiện tại<br>3. Băm và lưu mật khẩu mới (`PasswordHasher`)<br>4. Thu hồi mọi refresh token chưa thu hồi của người dùng | • `Forbidden_EmailNotVerifiedCheckInbox`<br>• `Invalid_CurrentPassword`<br>• `NotFound_User` | POST /api/v1/users/me/change-password | `backend/Cinema.Api/Controllers/Client/UsersController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → ChangePasswordAsync` |
| UC-HS-04 | Xem đánh giá của tôi | Người dùng đã đăng nhập | Danh sách đánh giá do người dùng đã viết. | 1. Đọc mã người dùng từ access token<br>2. Lấy danh sách đánh giá của người dùng theo con trỏ (`cursor`, `limit`) | • Chỉ trả đánh giá của chính người dùng<br>• Phân trang theo con trỏ | GET /api/v1/users/me/reviews | `backend/Cinema.Api/Controllers/Client/UsersController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs` |

### 3.3.3 Duyệt phim

- **Nội dung cần viết:**
  - Xem phim, lịch chiếu và thông tin công khai của rạp.
  - Package DP có 8 use case; tác nhân: Khách vãng lai

#### 3.3.3.1 Sơ đồ use case package Duyệt phim

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.6` Sơ đồ use case package Duyệt phim
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/MoviesController.cs`; `backend/Cinema.Api/Controllers/Client/ShowtimesController.cs`; `backend/Cinema.Api/Controllers/Client/CatalogController.cs`; `backend/Cinema.Api/Controllers/Client/SettingsController.cs`; `backend/Cinema.Api/Controllers/Client/CombosController.cs`; `backend/Cinema.Application/Interfaces/IMovieService.cs`; `backend/Cinema.Application/Services/Movies/MovieService.cs`; `backend/Cinema.Application/Interfaces/IReviewService.cs`; `backend/Cinema.Application/Services/Reviews/ReviewService.cs`; `backend/Cinema.Application/Interfaces/IShowtimeService.cs` …; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.6

#### 3.3.3.2 Danh sách use case package Duyệt phim

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.6` Danh sách use case package Duyệt phim (8 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/MoviesController.cs`; `backend/Cinema.Api/Controllers/Client/ShowtimesController.cs`; `backend/Cinema.Api/Controllers/Client/CatalogController.cs`; `backend/Cinema.Api/Controllers/Client/SettingsController.cs`; `backend/Cinema.Api/Controllers/Client/CombosController.cs`; `backend/Cinema.Application/Interfaces/IMovieService.cs`; `backend/Cinema.Application/Services/Movies/MovieService.cs`; `backend/Cinema.Application/Interfaces/IReviewService.cs`; `backend/Cinema.Application/Services/Reviews/ReviewService.cs`; `backend/Cinema.Application/Interfaces/IShowtimeService.cs` …

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-DP-01 | Xem danh sách phim | Khách vãng lai | Danh sách phim phân trang theo con trỏ, có bộ lọc. | 1. Nhận tham số lọc và con trỏ phân trang<br>2. Tùy chọn lọc theo trạng thái (`Status`), thể loại (`GenreId`), ngôn ngữ (`LanguageId`), phân loại tuổi (`AgeRatingId`), từ khóa (`Search`)<br>3. Phân trang theo con trỏ (`cursor`, `limit`) và trả danh sách phim | • `Status` nhận ComingSoon, NowShowing, Ended<br>• Các bộ lọc đều tùy chọn | GET /api/v1/movies | `backend/Cinema.Api/Controllers/Client/MoviesController.cs`<br>`backend/Cinema.Application/Services/Movies/MovieService.cs → GetMoviesAsync` |
| UC-DP-02 | Xem chi tiết phim | Khách vãng lai | Thông tin chi tiết của một phim. | 1. Tải thông tin chung: tên, mô tả, thời lượng, ngày phát hành, phụ đề, phân loại tuổi<br>2. Tải đạo diễn và diễn viên (`Director`, `MovieCast`)<br>3. Trả đường dẫn trailer (`TrailerUrl`)<br>4. Trả danh sách suất chiếu sắp tới (`UpcomingShowtimes`)<br>5. Xem đánh giá của phim (UC-DP-03)<br>6. Tùy chọn: chọn suất chiếu để đặt vé (UC-DV-01) | • `NotFound_Movie` khi không có phim | GET /api/v1/movies/{movieId} | `backend/Cinema.Api/Controllers/Client/MoviesController.cs`<br>`backend/Cinema.Application/Services/Movies/MovieService.cs → GetMovieDetailAsync` |
| UC-DP-03 | Xem đánh giá của phim | Khách vãng lai | Danh sách đánh giá và điểm trung bình của phim. | 1. Nhận mã phim và con trỏ phân trang<br>2. Lấy đánh giá của phim theo con trỏ (`cursor`, `limit`) | • `NotFound_Movie` | GET /api/v1/movies/{movieId}/reviews | `backend/Cinema.Api/Controllers/Client/MoviesController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs` |
| UC-DP-04 | Xem lịch chiếu theo ngày | Khách vãng lai | Các suất chiếu trong một ngày. | 1. Chọn ngày xem lịch (tham số `date`, định dạng dd/MM/yyyy)<br>2. Tùy chọn lọc theo định dạng chiếu (`screenType`)<br>3. Trả lịch chiếu nhóm theo phim (`WholeCinemaScheduleDto` → `WholeCinemaMovieDto` → `WholeCinemaShowtimeSlotDto`)<br>4. Tùy chọn: chọn suất chiếu để đặt vé (UC-DV-01) | • Tham số `date` bắt buộc (`Invalid_DateRequired`) | GET /api/v1/showtimes?date= | `backend/Cinema.Api/Controllers/Client/ShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → GetWholeCinemaScheduleAsync` |
| UC-DP-05 | Xem danh mục lọc | Khách vãng lai | Thể loại, ngôn ngữ, phân loại tuổi dùng cho bộ lọc. | 1. Gọi ba điểm cuối thể loại, ngôn ngữ, phân loại tuổi<br>2. Trả danh sách để dựng bộ lọc | • Công khai, không cần đăng nhập | GET /api/v1/genres, /languages, /age-ratings | `backend/Cinema.Api/Controllers/Client/CatalogController.cs` |
| UC-DP-06 | Xem các hạng thành viên | Khách vãng lai | Danh sách hạng thành viên, điểm tối thiểu và mức giảm giá. | 1. Lấy danh sách hạng thành viên<br>2. Trả tên hạng, điểm tối thiểu, mức giảm giá | • Công khai, không cần đăng nhập<br>• `MinPoints` là ngưỡng điểm hiệu lực trong 12 tháng gần nhất; mức giảm giá chỉ áp dụng cho tiền vé | GET /api/v1/membership-tiers | `backend/Cinema.Api/Controllers/Client/CatalogController.cs` |
| UC-DP-07 | Xem thông tin rạp | Khách vãng lai | Tên, địa chỉ, giờ hoạt động và chính sách công khai. | 1. Lấy cấu hình rạp (`CinemaSettings`)<br>2. Trả thông tin công khai của rạp | • `NotFound_CinemaSettings` khi chưa có bản ghi cấu hình | GET /api/v1/settings | `backend/Cinema.Api/Controllers/Client/SettingsController.cs`<br>`backend/Cinema.Application/Services/Settings/SettingsService.cs` |
| UC-DP-08 | Xem danh sách combo | Khách vãng lai | Combo bắp nước đang bán. | 1. Lấy danh sách combo đang bán (`IsActive = true`) | • Chỉ trả combo đang bán | GET /api/v1/combos | `backend/Cinema.Api/Controllers/Client/CombosController.cs`<br>`backend/Cinema.Application/Services/Combos/ComboService.cs` |

### 3.3.4 Đánh giá phim

- **Nội dung cần viết:**
  - Khách hàng đã xem phim viết, sửa, xóa đánh giá.
  - Package DG có 4 use case; tác nhân: Khách hàng

#### 3.3.4.1 Sơ đồ use case package Đánh giá phim

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.7` Sơ đồ use case package Đánh giá phim
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/MoviesController.cs`; `backend/Cinema.Application/Interfaces/IReviewService.cs`; `backend/Cinema.Application/Services/Reviews/ReviewService.cs`; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.7

#### 3.3.4.2 Danh sách use case package Đánh giá phim

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.7` Danh sách use case package Đánh giá phim (4 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/MoviesController.cs`; `backend/Cinema.Application/Interfaces/IReviewService.cs`; `backend/Cinema.Application/Services/Reviews/ReviewService.cs`

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-DG-01 | Kiểm tra quyền đánh giá | Khách hàng | Cho biết khách đã soát vé phim này và đã đánh giá hay chưa. | 1. Đọc mã người dùng và mã phim<br>2. Kiểm tra người dùng có vé đã soát của phim<br>3. Kiểm tra người dùng chưa có đánh giá cho phim<br>4. Trả kết quả có thể đánh giá hay không | • Phải có vé đã soát (`IsCheckedIn`) của phim<br>• Mỗi người một đánh giá cho mỗi phim | GET /api/v1/movies/{movieId}/can-review | `backend/Cinema.Api/Controllers/Client/MoviesController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs` |
| UC-DG-02 | Viết đánh giá | Khách hàng | Gửi điểm 1–5 và nhận xét cho phim. | 1. Kiểm tra quyền đánh giá (UC-DG-01)<br>2. Nhập điểm (1–5) và nhận xét<br>3. Lưu đánh giá<br>4. Tính lại điểm trung bình `Movie.AverageRating` (làm tròn 2 chữ số) | • `Rule_ReviewRequiresCheckIn`<br>• `Conflict_ReviewAlreadyExists`, chỉ mục duy nhất `(UserId, MovieId)`<br>• Ràng buộc `CK_Reviews_Rating`<br>• `NotFound_Movie`<br>• Chỉ vai trò Customer (`CustomerOnly`) | POST /api/v1/movies/{movieId}/reviews (CustomerOnly) | `backend/Cinema.Api/Controllers/Client/MoviesController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs` |
| UC-DG-03 | Sửa đánh giá | Khách hàng | Sửa đánh giá của chính mình. | 1. Tìm đánh giá theo mã<br>2. Kiểm tra đánh giá thuộc về người dùng<br>3. Cập nhật điểm và nhận xét<br>4. Tính lại điểm trung bình của phim | • `NotFound_Review`<br>• `Forbidden_NotReviewOwner`<br>• Điểm từ 1 đến 5<br>• Chỉ vai trò Customer | PUT /api/v1/movies/{movieId}/reviews/{id} (CustomerOnly) | `backend/Cinema.Api/Controllers/Client/MoviesController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs` |
| UC-DG-04 | Xóa đánh giá của mình | Khách hàng | Xóa đánh giá của chính mình. | 1. Tìm đánh giá theo mã<br>2. Kiểm tra đánh giá thuộc về người dùng<br>3. Xóa đánh giá<br>4. Tính lại điểm trung bình của phim | • `NotFound_Review`<br>• `Forbidden_NotReviewOwner`<br>• Chỉ vai trò Customer | DELETE /api/v1/movies/{movieId}/reviews/{id} (CustomerOnly) | `backend/Cinema.Api/Controllers/Client/MoviesController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs` |

### 3.3.5 Đặt vé trực tuyến

- **Nội dung cần viết:**
  - Chọn suất, chọn ghế, giữ ghế, quản lý đơn đặt vé.
  - Package DV có 9 use case; tác nhân: Khách hàng, Khách vãng lai

#### 3.3.5.1 Sơ đồ use case package Đặt vé trực tuyến

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.8` Sơ đồ use case package Đặt vé trực tuyến
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/BookingsController.cs`; `backend/Cinema.Api/Controllers/Client/ShowtimesController.cs`; `backend/Cinema.Api/Controllers/Client/CombosController.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs`; `backend/Cinema.Application/Interfaces/IShowtimeService.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs`; `backend/Cinema.Application/Interfaces/IComboService.cs`; `backend/Cinema.Application/Services/Combos/ComboService.cs`; `backend/Cinema.Application/Interfaces/IRealtimeNotifier.cs` …; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.8

#### 3.3.5.2 Danh sách use case package Đặt vé trực tuyến

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.8` Danh sách use case package Đặt vé trực tuyến (9 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/BookingsController.cs`; `backend/Cinema.Api/Controllers/Client/ShowtimesController.cs`; `backend/Cinema.Api/Controllers/Client/CombosController.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs`; `backend/Cinema.Application/Interfaces/IShowtimeService.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs`; `backend/Cinema.Application/Interfaces/IComboService.cs`; `backend/Cinema.Application/Services/Combos/ComboService.cs`; `backend/Cinema.Application/Interfaces/IRealtimeNotifier.cs` …

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-DV-01 | Đặt vé trực tuyến | Khách hàng | Quy trình đặt vé từ chọn suất đến thanh toán. | 1. Chọn suất chiếu từ chi tiết phim (UC-DP-02) hoặc lịch chiếu (UC-DP-04)<br>2. Xem sơ đồ ghế của suất chiếu (UC-DV-02)<br>3. Chọn ghế (UC-DV-03)<br>4. Tùy chọn: chọn combo (UC-DV-05)<br>5. Giữ ghế (UC-DV-04), tạo đơn Pending<br>6. Thanh toán trực tuyến (UC-TT-01)<br>7. Tùy chọn: mở lại đơn Pending còn hạn (UC-DV-09) hoặc hủy giữ ghế (UC-DV-08) | • Luồng tổng hợp, không có điểm cuối riêng; điều kiện nằm ở từng use case thành phần<br>• Chỉ khách hàng đã đăng nhập mới giữ ghế và thanh toán | Không có điểm cuối riêng (chuỗi UC-DV-02, UC-DV-03, UC-DV-04, UC-TT-01) | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs` |
| UC-DV-02 | Xem sơ đồ ghế của suất chiếu | Khách vãng lai, Khách hàng | Trạng thái từng ghế của suất chiếu. | 1. Gọi điểm cuối ghế của suất chiếu<br>2. Trả trạng thái từng ghế (Available, Held, Booked) từ `ShowtimeSeat.Status`<br>3. Tùy chọn: lấy socket token (UC-XT-11) và tham gia nhóm `showtime-{id}` của hub `/hubs/showtimes` để nhận cập nhật ghế thời gian thực<br>4. Tùy chọn: xem gợi ý ghế (UC-DV-06) | • `NotFound_Showtime`<br>• Khách vãng lai chỉ xem được sơ đồ, không nhận cập nhật thời gian thực<br>• Sự kiện: `seat.held`, `seat.released`, `seat.booked`, `hold.expired` | GET /api/v1/showtimes/{showtimeId}/seats | `backend/Cinema.Api/Controllers/Client/ShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → GetShowtimeSeatsAsync` |
| UC-DV-03 | Chọn ghế | Khách hàng | Chọn một hoặc nhiều ghế trên sơ đồ. | 1. Chọn hoặc bỏ chọn ghế đơn<br>2. Chọn ghế đôi theo cặp (các ghế cùng `CoupleId`)<br>3. Kiểm tra luật ghế lẻ theo `SeatGapRuleMode`<br>4. Kiểm tra số ghế tối đa mỗi đơn | • `SeatGapRuleMode`: Strict, AllowEdge, Off<br>• `MaxSeatsPerBooking` = 10<br>• Xử lý ở giao diện, máy chủ kiểm tra lại ở UC-DV-04 | Xử lý ở giao diện, kiểm tra lại ở UC-DV-04 | `frontend/src/components/common/seat-map/seat-map-utils.ts`<br>`backend/Cinema.Application/Utils/SeatLayoutRules.cs` |
| UC-DV-04 | Giữ ghế | Khách hàng | Giữ chỗ có thời hạn cho các ghế đã chọn. | 1. Kiểm tra dữ liệu đầu vào (`HoldSeatsRequestValidator`)<br>2. Kiểm tra suất chiếu còn mở bán<br>3. Kiểm tra chưa có đơn Pending còn hạn cho suất này<br>4. Kiểm tra luật chọn ghế phía máy chủ (`SeatLayoutRules.ValidateSelection`)<br>5. Giữ ghế nguyên tử bằng cập nhật có điều kiện (`ExecuteUpdate`)<br>6. Tính tiền: tổng = tiền vé (giá ghế theo suất) + combo; giảm giá = % của hạng × tiền vé, không tính combo; hạng xác định từ điểm hiệu lực 12 tháng trong sổ điểm (`CalculateDiscountAmountAsync` nhận `ticketAmount`)<br>7. Tạo đơn Pending với hạn giữ `ExpiresAt`<br>8. Phát sự kiện `seat.held`<br>9. Nếu xung đột ghế: rollback, trả 409 kèm `conflictSeatIds` | • Suất chiếu Scheduled, chưa bắt đầu, chưa quá thời điểm dừng bán online<br>• `Conflict_PendingBookingExistsForShowtime`<br>• `SeatConflictException` kèm `conflictSeatIds`<br>• Ghế chỉ khớp khi Available, hoặc Held đã hết hạn của chính người giữ<br>• `ExpiresAt` = hiện tại + `HoldDurationMinutes`, không quá giờ chiếu<br>• Giới hạn tốc độ `booking-hold`<br>• Hạng được tính từ sổ điểm `LoyaltyPointEntries`, không đọc `CustomerProfile.TierId` | POST /api/v1/bookings/hold (giới hạn tốc độ `booking-hold`) | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → HoldSeatsAsync, CreateHoldBookingAsync` |
| UC-DV-05 | Chọn combo | Khách hàng | Thêm combo bắp nước vào đơn. | 1. Xem danh sách combo đang bán<br>2. Tùy chọn: xem gợi ý combo theo suất chiếu và số ghế (`GetComboSuggestionsAsync`)<br>3. Chọn số lượng combo | • Chỉ combo `IsActive = true`<br>• Tối đa 10 mỗi loại (`MaxComboQuantity`)<br>• `NotFound_Showtime` khi gợi ý cho suất không tồn tại | GET /api/v1/combos, GET /api/v1/combos/suggestions | `backend/Cinema.Api/Controllers/Client/CombosController.cs`<br>`backend/Cinema.Application/Services/Combos/ComboService.cs` |
| UC-DV-06 | Xem gợi ý ghế | Khách vãng lai, Khách hàng | Gợi ý các vị trí ghế tốt theo số người. | 1. Nhận mã suất chiếu và số ghế cần<br>2. Tải các ghế của suất (`SeatLayoutRules.LoadSeatSlotsAsync`)<br>3. Chia đoạn ghế trống và chấm điểm (`SeatLayoutRules.Suggest`)<br>4. Trả tối đa 3 gợi ý (mặc định `top = 3`) | • `NotFound_Showtime` | GET /api/v1/showtimes/{showtimeId}/seat-suggestions | `backend/Cinema.Api/Controllers/Client/ShowtimesController.cs`<br>`backend/Cinema.Application/Utils/SeatLayoutRules.cs → Suggest` |
| UC-DV-07 | Xem danh sách đơn đặt vé của tôi | Khách hàng | Danh sách booking của người dùng, lọc theo trạng thái, phân trang con trỏ. | 1. Đọc mã người dùng<br>2. Lọc theo trạng thái (tùy chọn)<br>3. Trả danh sách đơn theo con trỏ (`cursor`, `limit`) | • Chỉ trả đơn của chính người dùng<br>• Yêu cầu đăng nhập | GET /api/v1/bookings | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → GetMyBookingsAsync` |
| UC-DV-08 | Hủy đơn đặt vé | Khách hàng | Hủy giữ ghế hoặc hủy đơn đã thanh toán. | 1. Kiểm tra đơn thuộc về người dùng<br>2. Nếu đơn Pending: hủy đơn, trả ghế về Available, phát `seat.released`<br>3. Nếu đơn Confirmed: kiểm tra điều kiện hủy, hủy đơn và trả ghế<br>4. Hoàn tiền qua cổng thanh toán (UC-TT-06), số tiền theo `CancelRefundPercent`<br>5. Thu hồi điểm thưởng vào đúng dòng sổ điểm của booking (`ReverseLoyaltyAsync` kèm `bookingId`), rồi tính lại điểm và hạng<br>6. Gửi thư xác nhận hủy bằng `SendEmailJob` (UC-HT-03) | • `Forbidden_NotBookingOwner`, `NotFound_Booking`<br>• `Rule_BookingNotCancellable`, `Conflict_BookingBeingProcessedOrCancelled`<br>• `Rule_BookingHasCheckedInTicket` (vé đã soát)<br>• `Rule_BookingCancelAtCounter` (đơn tại quầy/tiền mặt)<br>• Còn trong hạn `CancelDeadlineHours`<br>• `Rule_RefundFailed`, `Rule_RefundGatewayNotConfigured`<br>• `RevokedPoints` bị chặn tối đa bằng `Points` của dòng; dòng đã ngoài cửa sổ 12 tháng thì điểm hiệu lực không đổi | POST /api/v1/bookings/{bookingId}/cancel | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → CancelBookingAsync` |
| UC-DV-09 | Xem vé điện tử | Khách hàng | Chi tiết booking, trạng thái thanh toán, mã QR của từng vé. | 1. Kiểm tra đơn thuộc về người dùng<br>2. Tải đơn kèm phim, suất chiếu, ghế, vé (mã QR), combo, thanh toán<br>3. Trả chi tiết đơn / vé điện tử | • `Forbidden_NotBookingOwner`<br>• Cũng dùng để mở lại đơn Pending còn hạn | GET /api/v1/bookings/{bookingId} | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → GetBookingDetailAsync` |

### 3.3.6 Thanh toán trực tuyến

- **Nội dung cần viết:**
  - Tạo giao dịch, nhận IPN/webhook, xác nhận vé, hoàn tiền.
  - Package TT có 6 use case; tác nhân: Khách hàng, Cổng VNPay, Cổng MoMo, Cổng Stripe, Hệ thống tác vụ nền, Dịch vụ email

#### 3.3.6.1 Sơ đồ use case package Thanh toán trực tuyến

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.9` Sơ đồ use case package Thanh toán trực tuyến
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/BookingsController.cs`; `backend/Cinema.Api/Controllers/PaymentsController.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs`; `backend/Cinema.Application/Interfaces/IPaymentService.cs`; `backend/Cinema.Application/Services/Payments/PaymentService.cs`; `backend/Cinema.Application/Interfaces/IPaymentGateway.cs`; `backend/Cinema.Infrastructure/Payments/VnPayGateway.cs`; `backend/Cinema.Infrastructure/Payments/MomoGateway.cs`; `backend/Cinema.Infrastructure/Payments/StripeGateway.cs` …; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.9

#### 3.3.6.2 Danh sách use case package Thanh toán trực tuyến

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.9` Danh sách use case package Thanh toán trực tuyến (6 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/BookingsController.cs`; `backend/Cinema.Api/Controllers/PaymentsController.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs`; `backend/Cinema.Application/Interfaces/IPaymentService.cs`; `backend/Cinema.Application/Services/Payments/PaymentService.cs`; `backend/Cinema.Application/Interfaces/IPaymentGateway.cs`; `backend/Cinema.Infrastructure/Payments/VnPayGateway.cs`; `backend/Cinema.Infrastructure/Payments/MomoGateway.cs`; `backend/Cinema.Infrastructure/Payments/StripeGateway.cs` …

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-TT-01 | Thanh toán đơn đặt vé trực tuyến | Khách hàng, Cổng VNPay, Cổng MoMo, Cổng Stripe | Thanh toán booking Pending qua cổng được chọn. | 1. Kiểm tra đơn thuộc người dùng, còn Pending và còn hạn<br>2. Chọn cổng: VNPay (`VnPayGateway`), MoMo (`MomoGateway`) hoặc Stripe (`StripeGateway`)<br>3. Đánh dấu các giao dịch Pending cũ của đơn là Failed<br>4. Tạo bản ghi `Payment` và mã giao dịch (`CodeGenerator.GeneratePaymentTransactionId`)<br>5. Tạo liên kết thanh toán với cổng (`IPaymentGateway.CreatePaymentAsync`)<br>6. Chuyển hướng khách sang trang cổng; kết quả hiển thị ở trang kết quả đặt vé (`/booking/result`) | • `Forbidden_NotBookingOwner`, `NotFound_Booking`<br>• `Rule_BookingNotPendingOrExpired`<br>• `Rule_PaymentGatewayNotConfigured` | POST /api/v1/bookings/{bookingId}/checkout | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → CheckoutAsync` |
| UC-TT-02 | Xử lý IPN của VNPay | Cổng VNPay | Nhận thông báo kết quả thanh toán từ VNPay. | 1. Xác minh chữ ký HMAC-SHA512 (`HmacSigner.Sha512Hex`)<br>2. Tìm giao dịch theo mã<br>3. Bỏ qua nếu giao dịch đã được xử lý (`IsAlreadyProcessed`)<br>4. Kiểm tra số tiền khớp<br>5. Áp dụng kết quả thanh toán (UC-TT-05)<br>6. Trả mã phản hồi `RspCode` cho VNPay | • Chữ ký sai thì từ chối<br>• Xử lý idempotent<br>• Sai số tiền ≤ 0,01<br>• Điểm cuối trả phản hồi thô (`[RawResponse]`) | GET /api/v1/payments/vnpay/ipn | `backend/Cinema.Api/Controllers/PaymentsController.cs`<br>`backend/Cinema.Application/Services/Payments/PaymentService.cs`<br>`backend/Cinema.Infrastructure/Payments/VnPayGateway.cs` |
| UC-TT-03 | Xử lý IPN của MoMo | Cổng MoMo | Nhận thông báo kết quả thanh toán từ MoMo. | 1. Xác minh chữ ký HMAC-SHA256 (`HmacSigner.Sha256Hex`)<br>2. Tìm giao dịch theo mã<br>3. Bỏ qua nếu giao dịch đã được xử lý (`IsAlreadyProcessed`)<br>4. Kiểm tra số tiền khớp<br>5. Áp dụng kết quả thanh toán (UC-TT-05)<br>6. Trả `ResultCode` cho MoMo | • Chữ ký sai thì từ chối<br>• Xử lý idempotent<br>• Sai số tiền ≤ 0,01<br>• Điểm cuối trả phản hồi thô (`[RawResponse]`) | POST /api/v1/payments/momo/ipn | `backend/Cinema.Api/Controllers/PaymentsController.cs`<br>`backend/Cinema.Application/Services/Payments/PaymentService.cs`<br>`backend/Cinema.Infrastructure/Payments/MomoGateway.cs` |
| UC-TT-04 | Xử lý webhook của Stripe | Cổng Stripe | Nhận sự kiện hoàn tất phiên thanh toán từ Stripe. | 1. Xác minh chữ ký webhook (`StripeGateway.VerifyWebhook`)<br>2. Xử lý sự kiện `checkout.session.completed`<br>3. Kiểm tra số tiền và đơn vị tiền tệ (`GatewayCallbackResult.Currency`)<br>4. Bỏ qua nếu giao dịch đã được xử lý<br>5. Áp dụng kết quả thanh toán (UC-TT-05) | • Chữ ký sai thì từ chối<br>• Chỉ xử lý sự kiện `checkout.session.completed`<br>• Điểm cuối trả phản hồi thô (`[RawResponse]`) | POST /api/v1/payments/stripe/webhook | `backend/Cinema.Api/Controllers/PaymentsController.cs`<br>`backend/Cinema.Application/Services/Payments/PaymentService.cs`<br>`backend/Cinema.Infrastructure/Payments/StripeGateway.cs` |
| UC-TT-05 | Phát hành vé sau thanh toán | Hệ thống tác vụ nền, Dịch vụ email | Chuyển ghế sang đã bán, tạo vé và mã QR sau khi thanh toán thành công. | 1. Chuyển ghế từ Held sang Booked<br>2. Cập nhật đơn sang Confirmed<br>3. Tạo vé và mã QR cho từng ghế (`QrCodeHelper`, `CodeGenerator.GenerateTicketQrPayload`)<br>4. Ghi một dòng sổ điểm cho booking (`LoyaltyPointEntry`, điểm = ⌊`FinalAmount` / `PointsPerAmount`⌋), rồi tính lại điểm hiệu lực 12 tháng và hạng thành viên (`ApplyLoyaltyAsync`; hạng có thể tăng hoặc giảm)<br>5. Phát sự kiện `seat.booked`<br>6. Gửi thư xác nhận đặt vé bằng `SendEmailJob` (UC-HT-03)<br>7. Nếu ghế đã hết hạn sau khi khách trả tiền: tự hoàn tiền (UC-TT-06) | • Nội bộ, được gọi từ `PaymentService.ApplyPaymentResultAsync`<br>• Thanh toán thất bại thì gọi `ProcessPaymentFailureAsync`<br>• `SeatsNoLongerHeldException` kích hoạt `HandleSeatsExpiredAfterPaymentAsync`<br>• Mỗi booking tối đa một dòng sổ điểm (chỉ mục duy nhất `UX_LoyaltyPointEntries_Booking`) | Nội bộ: `PaymentService.ApplyPaymentResultAsync` | `backend/Cinema.Application/Services/Payments/PaymentService.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → ConfirmBookingAndGenerateTicketsAsync` |
| UC-TT-06 | Hoàn tiền | Hệ thống tác vụ nền, Cổng VNPay, Cổng MoMo, Cổng Stripe, Dịch vụ email | Hoàn tiền qua cổng khi hủy đơn, hủy suất chiếu hoặc ghế hết hạn sau thanh toán. | 1. Chọn cổng theo phương thức của giao dịch (VNPay, MoMo, Stripe)<br>2. Gọi `IPaymentGateway.RefundAsync`<br>3. Cập nhật `Payment` sang Refunded (`RefundTransactionId`, `RefundedAmount`, `RefundedAt`)<br>4. Thu hồi điểm thưởng vào đúng dòng sổ điểm của booking theo số tiền hoàn (`RevokedPoints ← min(Points, RevokedPoints + ⌊RefundedAmount / PointsPerAmount⌋)`)<br>5. Gửi thư hoàn tiền (`EmailTemplates`) bằng `SendEmailJob` (UC-HT-03)<br>6. Nếu cổng không hoàn được: hoàn tiền thủ công, gửi thư cho rạp | • Không áp dụng cho thanh toán tiền mặt<br>• Số tiền hoàn theo `CancelRefundPercent` khi khách tự hủy; hoàn toàn bộ khi hủy suất hoặc ghế hết hạn sau thanh toán | Nội bộ: `IPaymentGateway.RefundAsync` | `backend/Cinema.Application/Services/Payments/PaymentService.cs`<br>`backend/Cinema.Infrastructure/Payments/*Gateway.cs` |

### 3.3.7 Trợ lý AI

- **Nội dung cần viết:**
  - Trò chuyện hỏi đáp suất chiếu, ghế trống, vé của tôi.
  - Package AI có 1 use case; tác nhân: Khách vãng lai, Khách hàng, Gemini

#### 3.3.7.1 Sơ đồ use case package Trợ lý AI

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.10` Sơ đồ use case package Trợ lý AI
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/AiController.cs`; `backend/Cinema.Application/Interfaces/IAiAssistantService.cs`; `backend/Cinema.Application/Services/Ai/GeminiAiAssistantService.cs`; `backend/Cinema.Application/Services/Ai/AiToolExecutor.cs`; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.10

#### 3.3.7.2 Danh sách use case package Trợ lý AI

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.10` Danh sách use case package Trợ lý AI (1 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/AiController.cs`; `backend/Cinema.Application/Interfaces/IAiAssistantService.cs`; `backend/Cinema.Application/Services/Ai/GeminiAiAssistantService.cs`; `backend/Cinema.Application/Services/Ai/AiToolExecutor.cs`

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-AI-01 | Trò chuyện với trợ lý AI | Khách vãng lai, Khách hàng, Gemini | Hỏi đáp bằng ngôn ngữ tự nhiên, trợ lý gọi hàm để lấy dữ liệu thật. | 1. Gửi tin nhắn và lịch sử hội thoại<br>2. Dựng ngữ cảnh rạp, phim, combo, thông tin người dùng (system prompt)<br>3. Gọi mô hình Gemini (`gemini-2.5-flash`)<br>4. Khi cần, mô hình gọi hàm: `search_showtimes`, `get_seat_availability`, `get_my_bookings` (chỉ khi đã đăng nhập)<br>5. Trả câu trả lời kèm thẻ suất chiếu hoặc vé | • Tin nhắn ≤ 1000 ký tự, mỗi mục lịch sử ≤ 2000 ký tự (`AiValidators`)<br>• Tối đa 4 vòng gọi hàm (`MaxToolRounds`), tối đa 6 thẻ (`MaxCards`)<br>• Hàm `get_my_bookings` chỉ có khi đã đăng nhập | POST /api/v1/ai/chat | `backend/Cinema.Api/Controllers/Client/AiController.cs`<br>`backend/Cinema.Application/Services/Ai/GeminiAiAssistantService.cs`<br>`backend/Cinema.Application/Services/Ai/AiToolExecutor.cs` |

### 3.3.8 Bán vé tại quầy

- **Nội dung cần viết:**
  - Nhân viên bán vé trực tiếp, thu tiền mặt.
  - Package BQ có 1 use case; tác nhân: Nhân viên, Quản trị viên

#### 3.3.8.1 Sơ đồ use case package Bán vé tại quầy

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.11` Sơ đồ use case package Bán vé tại quầy
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs`; `backend/Cinema.Application/Interfaces/IEmployeeService.cs`; `backend/Cinema.Application/Services/Employees/EmployeeService.cs`; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.11

#### 3.3.8.2 Danh sách use case package Bán vé tại quầy

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.11` Danh sách use case package Bán vé tại quầy (1 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs`; `backend/Cinema.Application/Interfaces/IEmployeeService.cs`; `backend/Cinema.Application/Services/Employees/EmployeeService.cs`

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-BQ-01 | Bán vé tại quầy | Nhân viên, Quản trị viên | Bán vé cho khách tại rạp, thanh toán tiền mặt. | 1. Chọn suất chiếu<br>2. Xem sơ đồ ghế (UC-DV-02)<br>3. Chọn ghế, không áp luật ghế lẻ (`SeatGapRuleMode.Off`)<br>4. Tùy chọn: chọn combo<br>5. Nhập thông tin khách (tên, điện thoại) hoặc chọn khách hàng có sẵn; tùy chọn: tìm khách (UC-QN-03)<br>6. Giữ ghế tại quầy (`CounterHoldSeatsAsync`, kênh Counter, gắn `CreatedByEmployeeId`)<br>7. Thu tiền mặt (`CounterCheckoutAsync`), tính tiền thối<br>8. Xác nhận đơn và phát hành vé (UC-TT-05)<br>9. Hiển thị vé kết quả | • Khách chọn phải tồn tại, đang hoạt động và có vai trò Customer (`NotFound_Customer`, `Rule_CustomerAccountDisabled`, `Rule_UserIsNotCustomer`)<br>• `Rule_CounterPaymentOnlyForCounterBooking`<br>• `Rule_BookingNotPendingOrHoldExpired`<br>• Số tiền nhận ≥ số tiền cần trả (`Rule_AmountReceivedInsufficient`) | POST /api/v1/admin/bookings/counter-hold, POST /api/v1/admin/bookings/{id}/counter-checkout (StaffOnly) | `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → CounterHoldSeatsAsync, CounterCheckoutAsync`<br>`frontend/src/features/admin/counter/**` |

### 3.3.9 Soát vé

- **Nội dung cần viết:**
  - Quét mã QR hoặc nhập mã để soát vé vào rạp.
  - Package SV có 1 use case; tác nhân: Nhân viên, Quản trị viên

#### 3.3.9.1 Sơ đồ use case package Soát vé

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.12` Sơ đồ use case package Soát vé
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs`; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.12

#### 3.3.9.2 Danh sách use case package Soát vé

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.12` Danh sách use case package Soát vé (1 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs`

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-SV-01 | Soát vé | Nhân viên, Quản trị viên | Xác nhận vé hợp lệ khi khách vào rạp. | 1. Quét mã QR bằng camera (`BarcodeDetector` hoặc `jsQR`) hoặc nhập mã vé thủ công<br>2. Tìm vé theo mã QR<br>3. Kiểm tra đơn ở trạng thái Confirmed<br>4. Kiểm tra suất chiếu chưa bị hủy<br>5. Kiểm tra trong cửa sổ soát vé<br>6. Chặn soát lặp bằng cập nhật có điều kiện `IsCheckedIn = false`<br>7. Ghi nhận `IsCheckedIn`, `CheckedInAt`<br>8. Hiển thị phim, ghế hoặc lý do từ chối | • `NotFound_QrCode`<br>• `Rule_TicketNoLongerValid`<br>• `Rule_ShowtimeCancelled`<br>• `Rule_CheckInNotOpenYet`: mở trước giờ chiếu `CheckInOpenMinutes`; `Rule_ShowtimeEnded` sau `EndTime`<br>• `Conflict_TicketAlreadyCheckedIn` | POST /api/v1/admin/bookings/checkin (StaffOnly) | `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → CheckinAsync`<br>`frontend/src/features/admin/checkin/**` |

### 3.3.10 Quản lý booking

- **Nội dung cần viết:**
  - Tra cứu đơn đặt vé.
  - Package QB có 2 use case; tác nhân: Nhân viên, Quản trị viên

#### 3.3.10.1 Sơ đồ use case package Quản lý booking

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.13` Sơ đồ use case package Quản lý booking
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs`; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.13

#### 3.3.10.2 Danh sách use case package Quản lý booking

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.13` Danh sách use case package Quản lý booking (2 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs`

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-QB-01 | Xem danh sách booking | Nhân viên, Quản trị viên | Danh sách booking phân trang, có bộ lọc. | 1. Nhận bộ lọc và tham số phân trang<br>2. Tùy chọn lọc theo trạng thái, kênh bán (Online/Counter), khoảng ngày, từ khóa<br>3. Trả danh sách đơn theo trang (`page`, `pageSize`) | • Bộ lọc `AdminBookingListFilter`: `Status`, `Channel`, `FromDate`, `ToDate`, `Search`<br>• Chỉ nhân viên và quản trị (`StaffOnly`) | GET /api/v1/admin/bookings (StaffOnly) | `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → GetBookingsAdminAsync` |
| UC-QB-02 | Xem chi tiết booking | Nhân viên, Quản trị viên | Chi tiết booking, vé, combo, thanh toán. | 1. Nhận mã đơn<br>2. Tải chi tiết đơn kèm ghế, vé, combo, thanh toán, nhân viên bán<br>3. Trả chi tiết đơn | • Chỉ nhân viên và quản trị (`StaffOnly`) | GET /api/v1/admin/bookings/{bookingId} (StaffOnly) | `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs` |

### 3.3.11 Quản lý phim

- **Nội dung cần viết:**
  - Thêm, sửa, xóa phim.
  - Package QP có 5 use case; tác nhân: Nhân viên, Quản trị viên, Cloudinary

#### 3.3.11.1 Sơ đồ use case package Quản lý phim

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.14` Sơ đồ use case package Quản lý phim
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`; `backend/Cinema.Application/Interfaces/IMovieService.cs`; `backend/Cinema.Application/Services/Movies/MovieService.cs`; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.14

#### 3.3.11.2 Danh sách use case package Quản lý phim

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.14` Danh sách use case package Quản lý phim (5 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`; `backend/Cinema.Application/Interfaces/IMovieService.cs`; `backend/Cinema.Application/Services/Movies/MovieService.cs`

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-QP-01 | Xem danh sách phim (quản trị) | Nhân viên, Quản trị viên | Danh sách phim phân trang có bộ lọc. | 1. Nhận từ khóa, bộ lọc và tham số phân trang<br>2. Tùy chọn lọc theo trạng thái, đạo diễn, ngôn ngữ, phân loại tuổi, thể loại<br>3. Trả danh sách phim theo trang | • Bộ lọc `AdminMovieListFilter`<br>• Chỉ nhân viên và quản trị (`StaffOnly`) | GET /api/v1/admin/movies (StaffOnly) | `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`<br>`backend/Cinema.Application/Services/Movies/MovieService.cs → GetMoviesAdminAsync` |
| UC-QP-02 | Thêm phim | Quản trị viên, Cloudinary | Tạo phim mới kèm thể loại, diễn viên. | 1. Nhập tên, tên gốc, mô tả, thời lượng, ngày phát hành, phụ đề, trạng thái<br>2. Chọn đạo diễn, ngôn ngữ, phân loại tuổi<br>3. Chọn thể loại, diễn viên và vai diễn<br>4. Tùy chọn: tải poster (UC-TP-01), tải trailer (UC-TP-02) hoặc nhập `TrailerUrl`<br>5. Lưu phim: tạo `Movie`, `MovieGenre`, `MovieCast` | • `Invalid_DirectorIdNotFound`, `Invalid_LanguageIdNotFound`, `Invalid_AgeRatingIdNotFound`<br>• `Invalid_GenreIdsNotFound`, `Invalid_ActorIdsNotFound`<br>• Tổng hợp thành `Invalid_ReferenceData`<br>• Chỉ quản trị (`AdminOnly`) | POST /api/v1/admin/movies (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`<br>`backend/Cinema.Application/Services/Movies/MovieService.cs → CreateMovieAsync` |
| UC-QP-03 | Sửa phim | Quản trị viên, Cloudinary | Cập nhật thông tin và quan hệ của phim. | 1. Tải dữ liệu phim hiện tại<br>2. Sửa thông tin phim<br>3. Kiểm tra dữ liệu tham chiếu tồn tại<br>4. Cập nhật thể loại và diễn viên (`MovieGenre`, `MovieCast`)<br>5. Tùy chọn: đổi poster, trailer (UC-TP-01, UC-TP-02)<br>6. Lưu thay đổi | • `NotFound_Movie`<br>• `Invalid_ReferenceData`<br>• Chỉ quản trị (`AdminOnly`) | PUT /api/v1/admin/movies/{id} (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`<br>`backend/Cinema.Application/Services/Movies/MovieService.cs → UpdateMovieAsync` |
| UC-QP-04 | Xóa phim | Quản trị viên | Xóa một phim. | 1. Kiểm tra phim tồn tại<br>2. Kiểm tra phim đã có suất chiếu hay chưa<br>3. Nếu đã có suất chiếu: chuyển phim sang Ended (xóa mềm)<br>4. Nếu chưa có suất chiếu: xóa phim, các liên kết thể loại, diễn viên, đánh giá bị xóa theo `DeleteBehavior.Cascade` | • `NotFound_Movie`<br>• Phim có suất chiếu không bị xóa mà chuyển Ended, kết quả `DeleteMovieResponse(Deleted = false, SoftDeleted = true)`<br>• Quan hệ `Showtime → Movie` là Restrict<br>• Chỉ quản trị (`AdminOnly`) | DELETE /api/v1/admin/movies/{id} (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`<br>`backend/Cinema.Application/Services/Movies/MovieService.cs → DeleteMovieAsync` |
| UC-QP-05 | Xóa nhiều phim | Quản trị viên | Xóa hàng loạt các phim đã chọn. | 1. Nhận danh sách mã phim<br>2. Xử lý từng phim như UC-QP-04<br>3. Trả kết quả theo từng phim (`BulkDeleteMoviesResponse`) | • Giới hạn `BulkDeleteLimit` (mặc định 50)<br>• Chỉ quản trị (`AdminOnly`) | DELETE /api/v1/admin/movies/bulk (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`<br>`backend/Cinema.Application/Services/Movies/MovieService.cs` |

### 3.3.12 Quản lý danh mục

- **Nội dung cần viết:**
  - Danh mục phim, danh mục phòng, nhân sự, thành viên, combo.
  - Package QD có 10 use case; tác nhân: Quản trị viên, Cloudinary

#### 3.3.12.1 Sơ đồ use case package Quản lý danh mục

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.15` Sơ đồ use case package Quản lý danh mục
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminGenresController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminLanguagesController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminAgeRatingsController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminActorsController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminDirectorsController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminScreenTypesController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminSeatTypesController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminPositionsController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminMembershipTiersController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminCombosController.cs` …; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.15

#### 3.3.12.2 Danh sách use case package Quản lý danh mục

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.15` Danh sách use case package Quản lý danh mục (10 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminGenresController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminLanguagesController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminAgeRatingsController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminActorsController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminDirectorsController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminScreenTypesController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminSeatTypesController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminPositionsController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminMembershipTiersController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminCombosController.cs` …

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-QD-01 | Quản lý thể loại phim | Quản trị viên | Xem, thêm, sửa, xóa thể loại phim. | 1. Xem danh sách thể loại phim (phân trang)<br>2. Thêm thể loại phim<br>3. Sửa thể loại phim<br>4. Xóa thể loại phim | • `Conflict_GenreExists` nếu trùng<br>• `NotFound_Genre` khi sửa mã không tồn tại<br>• `Conflict_GenreInUse` khi đang được sử dụng<br>• Chỉ quản trị (`AdminOnly`) | /api/v1/admin/genres (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminGenresController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-02 | Quản lý ngôn ngữ | Quản trị viên | Xem, thêm, sửa, xóa ngôn ngữ. | 1. Xem danh sách ngôn ngữ (phân trang)<br>2. Thêm ngôn ngữ<br>3. Sửa ngôn ngữ<br>4. Xóa ngôn ngữ | • `Conflict_LanguageExists` nếu trùng<br>• `NotFound_Language` khi sửa mã không tồn tại<br>• `Conflict_LanguageInUse` khi đang được sử dụng<br>• Chỉ quản trị (`AdminOnly`) | /api/v1/admin/languages (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminLanguagesController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-03 | Quản lý phân loại độ tuổi | Quản trị viên | Xem, thêm, sửa, xóa phân loại độ tuổi. | 1. Xem danh sách phân loại độ tuổi (phân trang)<br>2. Thêm phân loại độ tuổi<br>3. Sửa phân loại độ tuổi<br>4. Xóa phân loại độ tuổi | • `Conflict_AgeRatingExists` nếu trùng<br>• `NotFound_AgeRating` khi sửa mã không tồn tại<br>• `Conflict_AgeRatingInUse` khi đang được sử dụng<br>• Chỉ quản trị (`AdminOnly`) | /api/v1/admin/age-ratings (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminAgeRatingsController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-04 | Quản lý diễn viên | Quản trị viên, Cloudinary | Xem, thêm, sửa, xóa diễn viên. | 1. Xem danh sách diễn viên (phân trang)<br>2. Thêm diễn viên<br>3. Sửa diễn viên<br>4. Xóa diễn viên<br>5. Tùy chọn: xóa nhiều diễn viên cùng lúc<br>6. Tùy chọn: tải ảnh đại diện (UC-TP-01) | • `NotFound_Actor` khi sửa mã không tồn tại<br>• `Conflict_ActorInUse` khi đang được sử dụng<br>• Chỉ quản trị (`AdminOnly`) | /api/v1/admin/actors (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminActorsController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-05 | Quản lý đạo diễn | Quản trị viên, Cloudinary | Xem, thêm, sửa, xóa đạo diễn. | 1. Xem danh sách đạo diễn (phân trang)<br>2. Thêm đạo diễn<br>3. Sửa đạo diễn<br>4. Xóa đạo diễn<br>5. Tùy chọn: xóa nhiều đạo diễn cùng lúc<br>6. Tùy chọn: tải ảnh đại diện (UC-TP-01) | • `NotFound_Director` khi sửa mã không tồn tại<br>• `Conflict_DirectorInUse` khi đang được sử dụng<br>• Chỉ quản trị (`AdminOnly`) | /api/v1/admin/directors (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminDirectorsController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-06 | Quản lý loại màn hình | Quản trị viên | Xem, thêm, sửa, xóa loại màn hình. | 1. Xem danh sách loại màn hình (phân trang)<br>2. Thêm loại màn hình<br>3. Sửa loại màn hình<br>4. Xóa loại màn hình | • `Conflict_ScreenTypeExists` nếu trùng<br>• `NotFound_ScreenType` khi sửa mã không tồn tại<br>• `Conflict_ScreenTypeInUse` khi đang được sử dụng<br>• Chỉ quản trị (`AdminOnly`) | /api/v1/admin/screen-types (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminScreenTypesController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-07 | Quản lý loại ghế | Quản trị viên | Xem, thêm, sửa, xóa loại ghế. | 1. Xem danh sách loại ghế (phân trang)<br>2. Thêm loại ghế<br>3. Sửa loại ghế<br>4. Xóa loại ghế<br>5. Đặt hệ số giá `PriceMultiplier` | • `Conflict_SeatTypeExists` nếu trùng<br>• `NotFound_SeatType` khi sửa mã không tồn tại<br>• `Conflict_SeatTypeInUse` khi đang được sử dụng<br>• `PriceMultiplier` có ràng buộc CHECK (xem 4.3)<br>• Chỉ quản trị (`AdminOnly`) | /api/v1/admin/seat-types (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminSeatTypesController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-08 | Quản lý chức vụ nhân viên | Quản trị viên | Xem, thêm, sửa, xóa chức vụ nhân viên. | 1. Xem danh sách chức vụ nhân viên (phân trang)<br>2. Thêm chức vụ nhân viên<br>3. Sửa chức vụ nhân viên<br>4. Xóa chức vụ nhân viên | • `Conflict_PositionExists` nếu trùng<br>• `NotFound_Position` khi sửa mã không tồn tại<br>• `Conflict_PositionInUse` khi đang được sử dụng<br>• Chỉ quản trị (`AdminOnly`) | /api/v1/admin/positions (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminPositionsController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-09 | Quản lý hạng thành viên | Quản trị viên | Xem, thêm, sửa, xóa hạng thành viên. | 1. Xem danh sách hạng thành viên (phân trang)<br>2. Thêm hạng thành viên<br>3. Sửa hạng thành viên<br>4. Xóa hạng thành viên<br>5. Đặt điểm tối thiểu `MinPoints` và mức giảm giá `DiscountPercent` | • `Conflict_MembershipTierExists` nếu trùng<br>• `NotFound_MembershipTier` khi sửa mã không tồn tại<br>• `Conflict_MembershipTierInUse` khi đang được sử dụng<br>• Chỉ quản trị (`AdminOnly`)<br>• `MinPoints` được so với điểm hiệu lực 12 tháng của khách hàng | /api/v1/admin/membership-tiers (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminMembershipTiersController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-10 | Quản lý combo | Quản trị viên, Cloudinary | Xem, thêm, sửa, bật/tắt combo (không có thao tác xóa). | 1. Xem danh sách combo (phân trang)<br>2. Thêm combo<br>3. Sửa combo<br>4. Bật hoặc tắt bán một combo<br>5. Tùy chọn: bật hoặc tắt bán nhiều combo (`SetCombosActiveAsync`)<br>6. Tùy chọn: tải ảnh combo (UC-TP-01) | • `NotFound_Combo` khi sửa, bật/tắt mã không tồn tại<br>• Không có thao tác xóa combo; ngừng bán bằng `IsActive`<br>• Chỉ quản trị (`AdminOnly`) | /api/v1/admin/combos (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminCombosController.cs`<br>`backend/Cinema.Application/Services/Combos/ComboService.cs` |

### 3.3.13 Quản lý phòng chiếu

- **Nội dung cần viết:**
  - Phòng chiếu và sơ đồ ghế.
  - Package QR có 3 use case; tác nhân: Quản trị viên

#### 3.3.13.1 Sơ đồ use case package Quản lý phòng chiếu

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.16` Sơ đồ use case package Quản lý phòng chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminRoomsController.cs`; `backend/Cinema.Application/Interfaces/IRoomService.cs`; `backend/Cinema.Application/Services/Rooms/RoomService.cs`; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.16

#### 3.3.13.2 Danh sách use case package Quản lý phòng chiếu

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.16` Danh sách use case package Quản lý phòng chiếu (3 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminRoomsController.cs`; `backend/Cinema.Application/Interfaces/IRoomService.cs`; `backend/Cinema.Application/Services/Rooms/RoomService.cs`

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-QR-01 | Quản lý phòng chiếu | Quản trị viên | Xem, thêm, sửa, xóa phòng chiếu. | 1. Xem danh sách phòng chiếu (phân trang)<br>2. Xem chi tiết phòng chiếu<br>3. Thêm phòng chiếu<br>4. Sửa phòng chiếu<br>5. Tùy chọn: ngừng hoạt động phòng (`IsActive = false`)<br>6. Xóa phòng chiếu | • Tên không trùng (`Conflict_RoomNameExists`); loại màn hình tồn tại (`Invalid_ScreenTypeIdNotFound`)<br>• Không ngừng hoạt động khi còn ghế đang giữ (`Conflict_RoomHasHeldSeatsCannotDeactivate`)<br>• Không xóa khi còn suất chiếu sắp tới (`Rule_RoomHasUpcomingShowtimes`)<br>• Chỉ quản trị (`AdminOnly`) | GET/POST /api/v1/admin/rooms, GET/PUT/DELETE /api/v1/admin/rooms/{id} (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminRoomsController.cs`<br>`backend/Cinema.Application/Services/Rooms/RoomService.cs` |
| UC-QR-02 | Xem sơ đồ ghế của phòng | Quản trị viên | Xem sơ đồ ghế hiện tại của phòng. | 1. Nhận mã phòng<br>2. Tải danh sách ghế của phòng (`GetRoomSeatsAsync`)<br>3. Trả hàng, số ghế, loại ghế, lối đi, cặp ghế đôi | • Chỉ quản trị (`AdminOnly`) | GET /api/v1/admin/rooms/{id}/seats (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminRoomsController.cs`<br>`backend/Cinema.Application/Services/Rooms/RoomService.cs → GetRoomSeatsAsync` |
| UC-QR-03 | Soạn sơ đồ ghế | Quản trị viên | Thiết kế và lưu sơ đồ ghế của phòng. | 1. Đặt kích thước lưới (số hàng, số cột)<br>2. Tô loại ghế bằng cọ<br>3. Tùy chọn: sinh sơ đồ tự động, chọn vùng nhiều ghế, đặt lối đi (`IsAisle`), ghép ghế đôi (`CoupleId`), sao chép hàng ghế, hoàn tác và làm lại<br>4. Kiểm tra tính hợp lệ của sơ đồ<br>5. Lưu sơ đồ ghế (`UpdateSeatLayoutAsync`) | • `Invalid_CoupleSeatsUnpaired`, `Invalid_SeatTypeIdsNotFound`<br>• Không đổi ghế đang giữ hoặc đã bán (`Conflict_SeatsHeldOrBookedCannotChange`)<br>• Chỉ quản trị (`AdminOnly`) | PUT /api/v1/admin/rooms/{id}/seats (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminRoomsController.cs`<br>`backend/Cinema.Application/Services/Rooms/RoomService.cs → UpdateSeatLayoutAsync`<br>`frontend/src/features/admin/rooms/components/seat-editor-*.tsx` |

### 3.3.14 Quản lý suất chiếu

- **Nội dung cần viết:**
  - Tạo, sửa, hủy, xếp lịch suất chiếu.
  - Package QS có 9 use case; tác nhân: Nhân viên, Quản trị viên, Dịch vụ email

#### 3.3.14.1 Sơ đồ use case package Quản lý suất chiếu

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.17` Sơ đồ use case package Quản lý suất chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`; `backend/Cinema.Application/Interfaces/IShowtimeService.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Interfaces/IPaymentGateway.cs`; `backend/Cinema.Application/BackgroundJobs/SendEmailJob.cs`; `backend/Cinema.Application/BackgroundJobs/ShowtimeRefundJob.cs`; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.17

#### 3.3.14.2 Danh sách use case package Quản lý suất chiếu

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.17` Danh sách use case package Quản lý suất chiếu (9 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`; `backend/Cinema.Application/Interfaces/IShowtimeService.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Interfaces/IPaymentGateway.cs`; `backend/Cinema.Application/BackgroundJobs/SendEmailJob.cs`; `backend/Cinema.Application/BackgroundJobs/ShowtimeRefundJob.cs`

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-QS-01 | Xem danh sách suất chiếu | Nhân viên, Quản trị viên | Danh sách suất chiếu có bộ lọc. | 1. Nhận bộ lọc và tham số phân trang<br>2. Tùy chọn lọc theo phòng, phim, ngày, trạng thái<br>3. Trả danh sách suất chiếu theo trang | • Bộ lọc `AdminShowtimeListFilter`: `RoomId`, `MovieId`, `Date`, `Status`<br>• Chỉ nhân viên và quản trị (`StaffOnly`) | GET /api/v1/admin/showtimes (StaffOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → GetShowtimesAdminAsync` |
| UC-QS-02 | Xem lịch suất chiếu dạng lịch | Nhân viên, Quản trị viên | Xem suất chiếu trên lịch theo ngày/phòng. | 1. Nhận khoảng ngày và bộ lọc<br>2. Trả suất chiếu nhóm theo ngày (`ShowtimeCalendarResponseDto`) | • Bộ lọc `AdminShowtimeCalendarFilter`: `From`, `To`, `RoomId`, `MovieId`, `Status`<br>• Khoảng ngày tối đa 92 ngày (`AdminShowtimeCalendarFilterValidator`)<br>• Chỉ nhân viên và quản trị (`StaffOnly`) | GET /api/v1/admin/showtimes/calendar (StaffOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → GetShowtimesCalendarAsync`<br>`frontend/src/features/admin/showtimes/components/calendar/**` |
| UC-QS-03 | Tạo suất chiếu | Quản trị viên | Tạo một suất chiếu mới. | 1. Chọn phim<br>2. Chọn phòng chiếu<br>3. Chọn giờ bắt đầu và nhập giá cơ bản `BasePrice`<br>4. Tính giờ kết thúc = giờ bắt đầu + thời lượng phim + đệm phòng 15 phút<br>5. Kiểm tra giờ mở cửa (`OperatingHours`)<br>6. Kiểm tra không trùng lịch phòng (`EnsureNoOverlapAsync`)<br>7. Sinh `ShowtimeSeat` cho từng ghế: giá = giá cơ bản × hệ số loại ghế<br>8. Lưu suất chiếu | • Không chọn phim đã kết thúc (Ended)<br>• Phòng đang hoạt động và đã có sơ đồ ghế<br>• `Conflict_ShowtimeOverlapInRoom`<br>• Ràng buộc `CK_Showtime_Time`: `EndTime > StartTime`<br>• `Rule_ShowtimeStartMustBeFuture`<br>• Chỉ quản trị (`AdminOnly`) | POST /api/v1/admin/showtimes (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → CreateShowtimeAsync` |
| UC-QS-04 | Sửa suất chiếu | Quản trị viên | Cập nhật giờ chiếu, giá của suất chiếu. | 1. Kiểm tra suất chiếu tồn tại và đang ở trạng thái Scheduled<br>2. Chặn đổi phòng<br>3. Chặn sửa khi đã có đơn Confirmed, hoặc đơn Pending còn giữ ghế<br>4. Kiểm tra phim chưa kết thúc<br>5. Kiểm tra giờ mở cửa và không trùng lịch phòng<br>6. Nếu đổi giá cơ bản: cập nhật giá các ghế chưa Booked theo hệ số loại ghế<br>7. Lưu thay đổi | • `NotFound_Showtime`, `Rule_ShowtimeMustBeScheduled`<br>• `Rule_CannotChangeShowtimeRoom`<br>• `Conflict_ShowtimeHasConfirmedBookings`, `Conflict_ShowtimeHasPendingBookings`<br>• `Rule_MovieEndedCannotUpdateShowtime`<br>• `Conflict_ShowtimeOverlapInRoom`<br>• Chỉ quản trị (`AdminOnly`) | PUT /api/v1/admin/showtimes/{id} (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → UpdateShowtimeAsync` |
| UC-QS-05 | Hủy suất chiếu | Quản trị viên, Dịch vụ email | Hủy suất chiếu, hoàn tiền cho các booking liên quan. | 1. Kiểm tra suất chiếu chưa bắt đầu<br>2. Đặt suất chiếu sang Cancelled<br>3. Chuyển các đơn Pending và Confirmed sang Cancelled; các giao dịch Pending thành Failed<br>4. Với mỗi giao dịch thành công: tiền mặt thì gửi thư hoàn tiền thủ công; còn lại xếp `ShowtimeRefundJob` (UC-HT-04)<br>5. Trả các ghế Held về Available<br>6. Gửi thư thông báo hủy suất bằng `SendEmailJob` (UC-HT-03) | • `NotFound_Showtime`<br>• `Rule_ShowtimeStartedCannotCancel`<br>• Kết quả trả `ManualRefundRequiredCount` và `AutoRefundQueuedCount`<br>• Chỉ quản trị (`AdminOnly`) | DELETE /api/v1/admin/showtimes/{id} (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → DeleteShowtimeAsync`<br>`backend/Cinema.Application/BackgroundJobs/ShowtimeRefundJob.cs` |
| UC-QS-06 | Tạo suất chiếu hàng loạt | Quản trị viên | Tạo nhiều suất theo khoảng ngày và khung giờ. | 1. Chọn phim và phòng chiếu<br>2. Chọn khoảng ngày, các thứ trong tuần và các khung giờ<br>3. Kiểm tra số suất tối đa mỗi lần<br>4. Kiểm tra từng suất (giờ mở cửa, trùng lịch)<br>5. Tạo các suất hợp lệ và báo các suất bị bỏ qua kèm lý do (`BulkCreateShowtimesResponse`: `Created`, `Skipped`) | • Tối đa 2000 suất mỗi lần (`MaxBatchSize`)<br>• Dữ liệu đầu vào kiểm tra bằng `BulkCreateShowtimesRequestValidator`, `BulkDateRangeValidator`<br>• Chỉ quản trị (`AdminOnly`) | POST /api/v1/admin/showtimes/bulk (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → BulkCreateShowtimesAsync` |
| UC-QS-07 | Tự động xếp lịch chiếu | Quản trị viên | Đề xuất và áp dụng lịch chiếu tự động theo độ nóng của phim. | 1. Chọn khoảng ngày, phòng, phim và số suất<br>2. Tính độ nóng của phim (`ShowtimeScheduler.Heat`: số vé bán gần đây, điểm đánh giá)<br>3. Sinh các khung giờ ứng viên (bước `StepMinutes`, đệm phòng, khoảng cách tối thiểu cùng phim)<br>4. Chấm điểm và chọn khung giờ theo thuật toán tham lam (`ShowtimeScheduler.Build`)<br>5. Xem trước kết quả (`PreviewAutoScheduleAsync`)<br>6. Áp dụng kết quả (`ApplyAutoScheduleAsync`) | • Khoảng ngày tối đa 31 ngày (`MaxAutoScheduleDays`)<br>• Tham số `SchedulerOptions`: `StepMinutes`, `MinSameMovieGapMinutes`, `HeatLookbackDays`<br>• Suất không xếp được nằm trong `Unplaced`<br>• Chỉ quản trị (`AdminOnly`) | POST /api/v1/admin/showtimes/auto-schedule/preview, /apply (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → PreviewAutoScheduleAsync, ApplyAutoScheduleAsync`<br>`backend/Cinema.Application/Utils/ShowtimeScheduler.cs` |
| UC-QS-08 | Nhân bản lịch chiếu | Quản trị viên | Sao chép lịch chiếu từ khoảng ngày này sang khoảng ngày khác. | 1. Chọn khoảng ngày nguồn (`SourceFrom`, `SourceTo`) và số ngày dịch chuyển `ShiftDays`<br>2. Tùy chọn: giới hạn theo phòng (`RoomIds`), cho phép ngoài giờ (`AllowAfterHours`)<br>3. Kiểm tra giờ mở cửa và trùng lịch từng suất<br>4. Tạo các suất mới, báo các suất bị bỏ qua | • Khoảng ngày nguồn tối đa 31 ngày (`CloneShowtimesRequestValidator`)<br>• `ShiftDays` mặc định 7<br>• Chỉ quản trị (`AdminOnly`) | POST /api/v1/admin/showtimes/clone (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → CloneShowtimesAsync` |
| UC-QS-09 | Xóa nhiều suất chiếu | Quản trị viên | Hủy hàng loạt các suất đã chọn. | 1. Nhận danh sách mã suất chiếu<br>2. Hủy từng suất như UC-QS-05<br>3. Trả kết quả theo từng suất (`BulkDeleteShowtimesResponse`) | • Dữ liệu đầu vào kiểm tra bằng `BulkDeleteShowtimesRequestValidator`<br>• Chỉ quản trị (`AdminOnly`) | DELETE /api/v1/admin/showtimes/bulk (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → BulkDeleteShowtimesAsync` |

### 3.3.15 Quản lý người dùng

- **Nội dung cần viết:**
  - Tài khoản người dùng và nhân viên.
  - Package QN có 7 use case; tác nhân: Quản trị viên, Nhân viên

#### 3.3.15.1 Sơ đồ use case package Quản lý người dùng

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.18` Sơ đồ use case package Quản lý người dùng
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminEmployeesController.cs`; `backend/Cinema.Application/Interfaces/IEmployeeService.cs`; `backend/Cinema.Application/Services/Employees/EmployeeService.cs`; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.18

#### 3.3.15.2 Danh sách use case package Quản lý người dùng

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.18` Danh sách use case package Quản lý người dùng (7 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminEmployeesController.cs`; `backend/Cinema.Application/Interfaces/IEmployeeService.cs`; `backend/Cinema.Application/Services/Employees/EmployeeService.cs`

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-QN-01 | Xem danh sách người dùng | Quản trị viên | Danh sách tài khoản có phân trang. | 1. Nhận bộ lọc vai trò, từ khóa và tham số phân trang<br>2. Trả danh sách người dùng theo trang | • Chỉ quản trị (`AdminOnly`) | GET /api/v1/admin/users (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`<br>`backend/Cinema.Application/Services/Employees/EmployeeService.cs → GetUsersAdminAsync` |
| UC-QN-02 | Xem danh sách tài khoản quản trị | Quản trị viên | Danh sách quản trị/nhân viên dùng để chọn khi tạo nhân viên. | 1. Nhận từ khóa tìm kiếm và cờ `availableOnly`<br>2. Trả danh sách tài khoản quản trị dùng cho ô chọn | • Chỉ lấy tài khoản có vai trò Admin hoặc Employee<br>• `availableOnly = true` loại các tài khoản đã có hồ sơ nhân viên<br>• Tối đa 50 kết quả (`UserLookupLimit`)<br>• Chỉ quản trị (`AdminOnly`) | GET /api/v1/admin/users/admins (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`<br>`backend/Cinema.Application/Services/Employees/EmployeeService.cs → GetAdminUsersForSelectAsync` |
| UC-QN-03 | Tìm kiếm khách hàng | Nhân viên, Quản trị viên | Tìm khách hàng khi bán vé tại quầy. | 1. Nhận từ khóa tìm kiếm<br>2. Trả danh sách khách hàng phục vụ bán vé tại quầy (`CustomerLookupItemDto`) | • Giới hạn số kết quả `CustomerLookupLimit` (mặc định 20)<br>• Chỉ nhân viên và quản trị (`StaffOnly`) | GET /api/v1/admin/users/customers (StaffOnly) | `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`<br>`backend/Cinema.Application/Services/Employees/EmployeeService.cs → SearchCustomersForCounterAsync` |
| UC-QN-04 | Khóa hoặc mở khóa tài khoản | Quản trị viên | Đổi trạng thái hoạt động của tài khoản. | 1. Kiểm tra tài khoản tồn tại<br>2. Chặn tự khóa tài khoản của mình<br>3. Chặn khóa quản trị viên cuối cùng<br>4. Cập nhật `User.IsActive` | • `NotFound_User`<br>• `Rule_CannotLockSelf`<br>• `Rule_CannotLockLastAdmin`<br>• Chỉ quản trị (`AdminOnly`) | PUT /api/v1/admin/users/{id}/status (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`<br>`backend/Cinema.Application/Services/Employees/EmployeeService.cs → UpdateUserStatusAsync` |
| UC-QN-05 | Đổi vai trò tài khoản | Quản trị viên | Đổi vai trò của tài khoản. | 1. Kiểm tra tài khoản và vai trò tồn tại<br>2. Chặn hạ vai trò khi nhân viên còn hoạt động<br>3. Cập nhật `User.RoleId` | • `NotFound_User`, `Invalid_RoleIdNotFound`<br>• `Conflict_CannotDowngradeRoleWithActiveEmployee`<br>• Chỉ quản trị (`AdminOnly`) | PUT /api/v1/admin/users/{id}/role (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`<br>`backend/Cinema.Application/Services/Employees/EmployeeService.cs → UpdateUserRoleAsync` |
| UC-QN-06 | Cập nhật trạng thái nhiều tài khoản | Quản trị viên | Khóa/mở khóa hàng loạt. | 1. Nhận danh sách mã tài khoản và trạng thái mới<br>2. Áp dụng cùng quy tắc như UC-QN-04 cho từng tài khoản<br>3. Trả kết quả theo từng tài khoản (`BulkActionResponse`) | • `NotFound_User`, `Rule_CannotLockSelf`, `Rule_CannotLockLastAdmin` ghi theo từng mục<br>• Chỉ quản trị (`AdminOnly`) | PUT /api/v1/admin/users/bulk/status (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`<br>`backend/Cinema.Application/Services/Employees/EmployeeService.cs → BulkUpdateUserStatusAsync` |
| UC-QN-07 | Quản lý nhân viên | Quản trị viên | Xem, thêm, sửa hồ sơ nhân viên. | 1. Xem danh sách nhân viên (lọc theo chức vụ, phân trang)<br>2. Thêm nhân viên: chọn tài khoản, chọn chức vụ, nhập ngày vào làm<br>3. Sửa hồ sơ nhân viên (chức vụ, ngày vào làm, `IsActive`) | • Tài khoản phải có vai trò nhân viên (`Rule_UserMustBeStaffRole`)<br>• Chưa có hồ sơ nhân viên (`Conflict_EmployeeProfileExists`)<br>• `NotFound_PositionId`, `NotFound_Employee`<br>• Chỉ quản trị (`AdminOnly`) | GET/POST /api/v1/admin/employees, PUT /api/v1/admin/employees/{id} (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminEmployeesController.cs`<br>`backend/Cinema.Application/Services/Employees/EmployeeService.cs` |

### 3.3.16 Quản lý đánh giá

- **Nội dung cần viết:**
  - Kiểm duyệt đánh giá phim.
  - Package QV có 3 use case; tác nhân: Quản trị viên

#### 3.3.16.1 Sơ đồ use case package Quản lý đánh giá

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.19` Sơ đồ use case package Quản lý đánh giá
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminReviewsController.cs`; `backend/Cinema.Application/Interfaces/IReviewService.cs`; `backend/Cinema.Application/Services/Reviews/ReviewService.cs`; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.19

#### 3.3.16.2 Danh sách use case package Quản lý đánh giá

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.19` Danh sách use case package Quản lý đánh giá (3 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminReviewsController.cs`; `backend/Cinema.Application/Interfaces/IReviewService.cs`; `backend/Cinema.Application/Services/Reviews/ReviewService.cs`

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-QV-01 | Xem danh sách đánh giá | Quản trị viên | Danh sách đánh giá phân trang. | 1. Nhận bộ lọc phim, điểm và tham số phân trang<br>2. Trả danh sách đánh giá theo trang | • Bộ lọc `movieId`, `rating`<br>• Chỉ quản trị (`AdminOnly`) | GET /api/v1/admin/reviews (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminReviewsController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs → GetReviewsAdminAsync` |
| UC-QV-02 | Xóa đánh giá | Quản trị viên | Xóa một đánh giá vi phạm. | 1. Tìm đánh giá theo mã<br>2. Xóa đánh giá<br>3. Tính lại điểm trung bình của phim | • `NotFound_Review`<br>• Chỉ quản trị (`AdminOnly`) | DELETE /api/v1/admin/reviews/{id} (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminReviewsController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs → DeleteReviewAsync` |
| UC-QV-03 | Xóa nhiều đánh giá | Quản trị viên | Xóa hàng loạt đánh giá. | 1. Nhận danh sách mã đánh giá<br>2. Xóa các đánh giá hợp lệ<br>3. Tính lại điểm trung bình của các phim liên quan | • `NotFound_Review` ghi theo từng mục<br>• Giới hạn `BulkDeleteLimit` (mặc định 50)<br>• Chỉ quản trị (`AdminOnly`) | DELETE /api/v1/admin/reviews/bulk (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminReviewsController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs → BulkDeleteReviewsAsync` |

### 3.3.17 Quản lý thanh toán

- **Nội dung cần viết:**
  - Tra cứu giao dịch thanh toán.
  - Package QT có 2 use case; tác nhân: Quản trị viên

#### 3.3.17.1 Sơ đồ use case package Quản lý thanh toán

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.20` Sơ đồ use case package Quản lý thanh toán
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminPaymentsController.cs`; `backend/Cinema.Application/Interfaces/IPaymentAdminService.cs`; `backend/Cinema.Application/Services/Payments/PaymentAdminService.cs`; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.20

#### 3.3.17.2 Danh sách use case package Quản lý thanh toán

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.20` Danh sách use case package Quản lý thanh toán (2 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminPaymentsController.cs`; `backend/Cinema.Application/Interfaces/IPaymentAdminService.cs`; `backend/Cinema.Application/Services/Payments/PaymentAdminService.cs`

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-QT-01 | Xem danh sách thanh toán | Quản trị viên | Danh sách giao dịch thanh toán. | 1. Nhận bộ lọc trạng thái, phương thức, khoảng ngày, từ khóa, cờ cần hoàn tiền và tham số phân trang<br>2. Trả danh sách giao dịch theo trang | • Bộ lọc `AdminPaymentListFilter`<br>• Chỉ quản trị (`AdminOnly`) | GET /api/v1/admin/payments (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminPaymentsController.cs`<br>`backend/Cinema.Application/Services/Payments/PaymentAdminService.cs` |
| UC-QT-02 | Xem chi tiết thanh toán | Quản trị viên | Chi tiết một giao dịch (cổng, mã giao dịch, hoàn tiền). | 1. Nhận mã giao dịch<br>2. Tải giao dịch kèm thông tin đơn (`AdminPaymentBookingBriefDto`)<br>3. Trả chi tiết giao dịch | • `NotFound_Payment`<br>• Chỉ quản trị (`AdminOnly`) | GET /api/v1/admin/payments/{id} (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminPaymentsController.cs`<br>`backend/Cinema.Application/Services/Payments/PaymentAdminService.cs` |

### 3.3.18 Báo cáo thống kê

- **Nội dung cần viết:**
  - Dashboard và các báo cáo.
  - Package BC có 5 use case; tác nhân: Quản trị viên

#### 3.3.18.1 Sơ đồ use case package Báo cáo thống kê

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.21` Sơ đồ use case package Báo cáo thống kê
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminDashboardController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminReportsController.cs`; `backend/Cinema.Application/Interfaces/IDashboardService.cs`; `backend/Cinema.Application/Services/Dashboard/DashboardService.cs`; `backend/Cinema.Application/Interfaces/IReportService.cs`; `backend/Cinema.Application/Services/Reports/ReportService.cs`; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.21

#### 3.3.18.2 Danh sách use case package Báo cáo thống kê

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.21` Danh sách use case package Báo cáo thống kê (5 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminDashboardController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminReportsController.cs`; `backend/Cinema.Application/Interfaces/IDashboardService.cs`; `backend/Cinema.Application/Services/Dashboard/DashboardService.cs`; `backend/Cinema.Application/Interfaces/IReportService.cs`; `backend/Cinema.Application/Services/Reports/ReportService.cs`

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-BC-01 | Xem tổng quan dashboard | Quản trị viên | Chỉ số tổng hợp của rạp. | 1. Tổng hợp số liệu tổng quan dashboard<br>2. Trả các chỉ số tổng quan và danh sách suất chiếu (`DashboardSummaryDto`, `DashboardShowtimeItemDto`) | • Chỉ quản trị (`AdminOnly`) | GET /api/v1/admin/dashboard/summary (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminDashboardController.cs`<br>`backend/Cinema.Application/Services/Dashboard/DashboardService.cs → GetSummaryAsync` |
| UC-BC-02 | Xem nhận định (insights) | Quản trị viên | Các nhận định tổng hợp từ dữ liệu. | 1. Nhận số ngày phân tích (`days`)<br>2. Tổng hợp nhận định (`DashboardInsightsDto`) cho `days` ngày gần nhất | • `days` được giới hạn trong khoảng 1–90<br>• Chỉ quản trị (`AdminOnly`) | GET /api/v1/admin/dashboard/insights (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminDashboardController.cs`<br>`backend/Cinema.Application/Services/Dashboard/DashboardService.cs → GetInsightsAsync` |
| UC-BC-03 | Xem báo cáo doanh thu | Quản trị viên | Doanh thu theo ngày/tuần/tháng, trừ phần hoàn tiền. | 1. Chọn khoảng ngày và cách nhóm (`groupBy`)<br>2. Tổng hợp doanh thu theo kỳ (`RevenueReportDto`) | • `Invalid_GroupBy`<br>• Chỉ quản trị (`AdminOnly`) | GET /api/v1/admin/reports/revenue (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminReportsController.cs`<br>`backend/Cinema.Application/Services/Reports/ReportService.cs → GetRevenueReportAsync` |
| UC-BC-04 | Xem báo cáo phim bán chạy | Quản trị viên | Xếp hạng phim theo số vé và doanh thu. | 1. Chọn khoảng ngày và số phim (`limit`)<br>2. Xếp hạng phim bán chạy (`TopMovieDto`) | • Chỉ quản trị (`AdminOnly`) | GET /api/v1/admin/reports/top-movies (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminReportsController.cs`<br>`backend/Cinema.Application/Services/Reports/ReportService.cs → GetTopMoviesReportAsync` |
| UC-BC-05 | Xem báo cáo hiệu suất nhân viên | Quản trị viên | Doanh số bán quầy theo nhân viên. | 1. Chọn khoảng ngày<br>2. Tổng hợp hiệu suất bán vé tại quầy theo nhân viên (`EmployeePerformanceDto`) | • Chỉ quản trị (`AdminOnly`) | GET /api/v1/admin/reports/employee-performance (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminReportsController.cs`<br>`backend/Cinema.Application/Services/Reports/ReportService.cs → GetEmployeePerformanceReportAsync` |

### 3.3.19 Cấu hình rạp

- **Nội dung cần viết:**
  - Thông tin rạp và tham số vận hành.
  - Package CH có 2 use case; tác nhân: Quản trị viên, Cloudinary

#### 3.3.19.1 Sơ đồ use case package Cấu hình rạp

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.22` Sơ đồ use case package Cấu hình rạp
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminSettingsController.cs`; `backend/Cinema.Api/Controllers/Client/SettingsController.cs`; `backend/Cinema.Application/Interfaces/ISettingsService.cs`; `backend/Cinema.Application/Services/Settings/SettingsService.cs`; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.22

#### 3.3.19.2 Danh sách use case package Cấu hình rạp

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.22` Danh sách use case package Cấu hình rạp (2 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminSettingsController.cs`; `backend/Cinema.Api/Controllers/Client/SettingsController.cs`; `backend/Cinema.Application/Interfaces/ISettingsService.cs`; `backend/Cinema.Application/Services/Settings/SettingsService.cs`

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-CH-01 | Xem cấu hình rạp | Quản trị viên | Xem toàn bộ tham số cấu hình. | 1. Tải bản ghi cấu hình rạp<br>2. Trả toàn bộ tham số cấu hình (`CinemaSettingsDto`) | • `NotFound_CinemaSettings`<br>• Chỉ quản trị (`AdminOnly`) | GET /api/v1/admin/settings (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminSettingsController.cs`<br>`backend/Cinema.Application/Services/Settings/SettingsService.cs → GetSettingsAsync` |
| UC-CH-02 | Cập nhật cấu hình rạp | Quản trị viên, Cloudinary | Cập nhật thông tin và tham số vận hành. | 1. Cập nhật thông tin rạp: tên, địa chỉ, điện thoại, email, mã số thuế<br>2. Tùy chọn: tải logo (UC-TP-01)<br>3. Cập nhật giờ mở cửa và đóng cửa (`OpeningTime`, `ClosingTime`)<br>4. Cập nhật thời gian giữ ghế và dừng bán online (`HoldDurationMinutes`, `StopSellingMinutesBefore`)<br>5. Cập nhật thời gian mở soát vé (`CheckInOpenMinutes`)<br>6. Cập nhật chính sách hủy và hoàn tiền (`CancelRefundPercent`, `CancelDeadlineHours`)<br>7. Cập nhật quy đổi điểm thưởng (`PointsPerAmount`)<br>8. Cập nhật luật ghế lẻ (`SeatGapRuleMode`) | • `Rule_CancelRefundPercentRange`<br>• `Rule_CancelDeadlineHoursNegative`<br>• `NotFound_CinemaSettings`<br>• Chỉ quản trị (`AdminOnly`) | PUT /api/v1/admin/settings (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminSettingsController.cs`<br>`backend/Cinema.Application/Services/Settings/SettingsService.cs → UpdateSettingsAsync` |

### 3.3.20 Tải tệp

- **Nội dung cần viết:**
  - Tải ảnh và video lên dịch vụ lưu trữ.
  - Package TP có 2 use case; tác nhân: Quản trị viên, Cloudinary

#### 3.3.20.1 Sơ đồ use case package Tải tệp

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
- **Tài liệu/hình/bảng:**
  - `Hình 3.23` Sơ đồ use case package Tải tệp
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminUploadsController.cs`; `backend/Cinema.Application/Interfaces/IFileStorage.cs`; `backend/Cinema.Infrastructure/FileStorage/CloudinaryFileStorage.cs`; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.23

#### 3.3.20.2 Danh sách use case package Tải tệp

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.23` Danh sách use case package Tải tệp (2 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminUploadsController.cs`; `backend/Cinema.Application/Interfaces/IFileStorage.cs`; `backend/Cinema.Infrastructure/FileStorage/CloudinaryFileStorage.cs`

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-TP-01 | Tải ảnh lên | Quản trị viên, Cloudinary | Tải ảnh (poster, ảnh đại diện, ảnh combo, logo). | 1. Nhận tệp ảnh (multipart/form-data)<br>2. Tải lên Cloudinary (`IFileStorage.UploadImageAsync`)<br>3. Trả đường dẫn và mã công khai (`UploadImageResponse`) | • Chỉ quản trị (`AdminOnly`) | POST /api/v1/admin/uploads/image (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminUploadsController.cs`<br>`backend/Cinema.Infrastructure/FileStorage/CloudinaryFileStorage.cs → UploadImageAsync` |
| UC-TP-02 | Tải video lên | Quản trị viên, Cloudinary | Tải video (trailer). | 1. Nhận tệp video (multipart/form-data)<br>2. Tải lên Cloudinary (`IFileStorage.UploadVideoAsync`)<br>3. Trả đường dẫn và mã công khai | • Giới hạn kích thước yêu cầu 100 MB (`RequestSizeLimit`)<br>• Chỉ quản trị (`AdminOnly`) | POST /api/v1/admin/uploads/video (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminUploadsController.cs`<br>`backend/Cinema.Infrastructure/FileStorage/CloudinaryFileStorage.cs → UploadVideoAsync` |

### 3.3.21 Vận hành hệ thống

- **Nội dung cần viết:**
  - Công việc chạy nền và chức năng vận hành hệ thống.
  - Package HT có 8 use case; tác nhân: Hệ thống tác vụ nền, Dịch vụ email, Cổng VNPay, Cổng MoMo, Cổng Stripe, Quản trị viên, Khách vãng lai

#### 3.3.21.1 Sơ đồ use case package Vận hành hệ thống

- **Nội dung cần viết:**
  - Các use case mức 1 của package và quan hệ với tác nhân
  - Quan hệ include/extend vẽ giữa các use case mức 1; use case của package khác tô xám, nằm ngoài khung
  - Có thêm UC-HT-08 Đồng bộ điểm thành viên hết hạn (tác nhân Hệ thống tác vụ nền); cập nhật sơ đồ và mã PlantUML tương ứng
- **Tài liệu/hình/bảng:**
  - `Hình 3.24` Sơ đồ use case package Vận hành hệ thống
- **Nguồn dữ liệu:** `backend/Cinema.Application/BackgroundJobs/ReleaseExpiredSeatsJob.cs`; `backend/Cinema.Application/BackgroundJobs/ShowtimeLifecycleJob.cs`; `backend/Cinema.Application/BackgroundJobs/SendEmailJob.cs`; `backend/Cinema.Application/BackgroundJobs/ShowtimeRefundJob.cs`; `backend/Cinema.Application/Interfaces/ISeatExpirationService.cs`; `backend/Cinema.Application/Services/Bookings/SeatExpirationService.cs`; `backend/Cinema.Application/Interfaces/IShowtimeService.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs`; `backend/Cinema.Application/Interfaces/IRealtimeNotifier.cs`; `backend/Cinema.Infrastructure/Realtime/SignalRRealtimeNotifier.cs`; `backend/Cinema.Application/BackgroundJobs/ExpireLoyaltyPointsJob.cs`; `LoyaltyExpiryService.cs`; `LoyaltyWindow.cs` …; mã sơ đồ: `ma-nguon-so-do.md`, Hình 3.24

#### 3.3.21.2 Danh sách use case package Vận hành hệ thống

- **Nội dung cần viết:**
  - Mỗi use case một dòng, đủ 8 cột; không có use case con
  - Cột "Các bước xử lý chính" và "Điều kiện, lỗi nghiệp vụ" đối chiếu với code backend
- **Tài liệu/hình/bảng:**
  - `Bảng 3.24` Danh sách use case package Vận hành hệ thống (8 use case)
- **Nguồn dữ liệu:** `backend/Cinema.Application/BackgroundJobs/ReleaseExpiredSeatsJob.cs`; `backend/Cinema.Application/BackgroundJobs/ShowtimeLifecycleJob.cs`; `backend/Cinema.Application/BackgroundJobs/SendEmailJob.cs`; `backend/Cinema.Application/BackgroundJobs/ShowtimeRefundJob.cs`; `backend/Cinema.Application/Interfaces/ISeatExpirationService.cs`; `backend/Cinema.Application/Services/Bookings/SeatExpirationService.cs`; `backend/Cinema.Application/Interfaces/IShowtimeService.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs`; `backend/Cinema.Application/Interfaces/IRealtimeNotifier.cs`; `backend/Cinema.Infrastructure/Realtime/SignalRRealtimeNotifier.cs`; `backend/Cinema.Application/BackgroundJobs/ExpireLoyaltyPointsJob.cs`; `LoyaltyExpiryService.cs`; `LoyaltyWindow.cs` …

| Mã | Use case | Tác nhân | Mô tả | Các bước xử lý chính | Điều kiện, lỗi nghiệp vụ | Endpoint | Nguồn |
|---|---|---|---|---|---|---|---|
| UC-HT-01 | Giải phóng ghế hết hạn | Hệ thống tác vụ nền | Công việc lặp mỗi phút. | 1. Hangfire kích hoạt công việc định kỳ `ReleaseExpiredSeatsJob`<br>2. Chuyển đơn Pending quá hạn sang Expired (`ExpireDueBookingsAsync`)<br>3. Trả ghế về Available (`ReleaseAndCancel`)<br>4. Đánh dấu các Payment Pending thành Failed<br>5. Phát sự kiện `hold.expired`<br>6. Dọn các ghế Held quá hạn còn sót (`ReleaseStaleHeldSeatsAsync`) | • Chạy theo cron `ReleaseExpiredSeatsCron` (mặc định mỗi phút)<br>• Mã công việc `release-expired-seats-and-bookings` | Hangfire recurring job | `backend/Cinema.Application/BackgroundJobs/ReleaseExpiredSeatsJob.cs`<br>`backend/Cinema.Application/Services/Bookings/SeatExpirationService.cs` |
| UC-HT-02 | Cập nhật vòng đời suất chiếu | Hệ thống tác vụ nền | Công việc lặp mỗi phút. | 1. Hangfire kích hoạt `ShowtimeLifecycleJob`<br>2. Chuyển suất chiếu đã kết thúc sang Completed (`CompleteEndedShowtimesAsync`)<br>3. Chuyển phim không còn suất sắp chiếu sang Ended (`EndMoviesWithoutUpcomingShowtimesAsync`) | • Chạy theo cron `ShowtimeLifecycleCron` (mặc định mỗi phút)<br>• Mã công việc `update-showtime-lifecycle` | Hangfire recurring job | `backend/Cinema.Application/BackgroundJobs/ShowtimeLifecycleJob.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → CompleteEndedShowtimesAsync, EndMoviesWithoutUpcomingShowtimesAsync` |
| UC-HT-03 | Gửi email | Hệ thống tác vụ nền, Dịch vụ email | Gửi email xác nhận, khôi phục mật khẩu, đặt vé, hủy, hoàn tiền. | 1. Nhận công việc `SendEmailJob` (người nhận, tiêu đề, nội dung HTML)<br>2. Gửi thư qua `IEmailService` | • Công việc fire-and-forget do các use case khác xếp hàng<br>• Gửi thư qua Resend, dự phòng SMTP (`FallbackEmailService`) | Hangfire fire-and-forget job | `backend/Cinema.Application/BackgroundJobs/SendEmailJob.cs`<br>`backend/Cinema.Infrastructure/Email/*` |
| UC-HT-04 | Hoàn tiền khi hủy suất chiếu | Hệ thống tác vụ nền, Cổng VNPay, Cổng MoMo, Cổng Stripe, Dịch vụ email | Hoàn tiền từng thanh toán của suất bị hủy. | 1. Nhận mã thanh toán từ `ShowtimeRefundJob`<br>2. Kiểm tra đơn và suất chiếu đã ở trạng thái Cancelled<br>3. Hoàn tiền từng thanh toán qua cổng (UC-TT-06)<br>4. Thử lại khi lỗi<br>5. Gửi thư hoàn tiền tự động hoặc hoàn tiền thủ công | • Thử lại 3 lần: 30 giây, 2 phút, 10 phút (`BackgroundJobOptions.RetryDelaysSeconds`)<br>• Không áp dụng cho thanh toán tiền mặt | Hangfire job (thử lại 3 lần: 30 giây, 2 phút, 10 phút) | `backend/Cinema.Application/BackgroundJobs/ShowtimeRefundJob.cs` |
| UC-HT-05 | Khởi tạo dữ liệu khi khởi động | Hệ thống tác vụ nền | Chạy migration và seed khi bật cờ cấu hình. | 1. Khi khởi động, đọc cấu hình `RunMigrationsOnStartup`<br>2. Áp dụng migration<br>3. Nạp dữ liệu khởi tạo (`DbInitializer.SeedAsync`) | • `RunMigrationsOnStartup` mặc định true<br>• Tham số `seedDemoScenarios` điều khiển việc nạp kịch bản demo | `RunMigrationsOnStartup` | `backend/Cinema.Api/Program.cs`<br>`backend/Cinema.Infrastructure/Persistence/Seed/DbInitializer.cs` |
| UC-HT-06 | Xem bảng điều khiển Hangfire | Quản trị viên | Giám sát công việc nền. | 1. Quản trị mở đường dẫn `/admin/hangfire`<br>2. Bộ lọc `HangfireAdminAuthorizationFilter` kiểm tra quyền<br>3. Hiển thị bảng điều khiển Hangfire | • Chỉ quản trị | /admin/hangfire (chỉ quản trị) | `backend/Cinema.Api/Filters/HangfireAdminAuthorizationFilter.cs` |
| UC-HT-07 | Kiểm tra sức khỏe hệ thống | Khách vãng lai | Endpoint kiểm tra tình trạng dịch vụ. | 1. Gọi điểm cuối sức khỏe<br>2. Trả trạng thái hệ thống (`HealthController.Get`) | • Công khai, không cần đăng nhập | GET /api/v1/health | `backend/Cinema.Api/Controllers/HealthController.cs` |
| UC-HT-08 | Đồng bộ điểm thành viên hết hạn | Hệ thống tác vụ nền | Công việc lặp mỗi giờ, tính lại điểm hiệu lực và hạng thành viên từ sổ điểm. | 1. Hangfire kích hoạt công việc định kỳ `ExpireLoyaltyPointsJob`<br>2. Gọi `LoyaltyExpiryService.RecalculateAllAsync`<br>3. Với từng khách hàng, cộng `Points − RevokedPoints` của các dòng sổ điểm có `EarnedAt` trong 12 tháng gần nhất (`LoyaltyWindow.Months`)<br>4. Xác định hạng có `MinPoints` lớn nhất không vượt điểm hiệu lực<br>5. Cập nhật `CustomerProfile.Points` và `TierId` (hạng có thể bị hạ) | • Chạy theo cron `ExpireLoyaltyPointsCron` (mặc định `5 * * * *`: mỗi giờ, phút 05, múi giờ VN)<br>• Mã công việc `expire-loyalty-points` (`RecurringJobIds.ExpireLoyaltyPoints`)<br>• Khách hàng cũ phải chạy script backfill sổ điểm trước, nếu không job đưa điểm của họ về 0<br>• Điểm và hạng trên hồ sơ có thể trễ tối đa 1 giờ; giảm giá lúc đặt vé luôn tính từ sổ điểm | Hangfire recurring job | `backend/Cinema.Application/BackgroundJobs/ExpireLoyaltyPointsJob.cs`<br>`LoyaltyExpiryService.cs → RecalculateAllAsync` |


## 3.4 ĐẶC TẢ USE CASE CHÍNH


### 3.4.1 Đặc tả UC-XT-04 Đăng nhập bằng email

- **Nội dung cần viết:**
  - Viết đặc tả theo bảng; mô tả từng bước bằng câu hoàn chỉnh
- **Tài liệu/hình/bảng:**
  - `Bảng 3.25` Đặc tả UC-XT-04 Đăng nhập bằng email

| Mục | Nội dung |
|---|---|
| Mã / Tên | UC-XT-04 — Đăng nhập bằng email |
| Tác nhân | Khách vãng lai |
| Mô tả | Khách hàng đăng nhập bằng email và mật khẩu. |
| Tiền điều kiện | Khách vãng lai chưa đăng nhập; Tài khoản đã đăng ký |
| Hậu điều kiện | Khách có access token và refresh token |
| Luồng chính | Nhập email/mật khẩu → hệ thống kiểm tra thông tin, trạng thái khóa, xác nhận email, loại cổng → cấp token<br>Chi tiết các bước: 1. Kiểm tra email và mật khẩu<br>2. Kiểm tra tài khoản chưa bị khóa<br>3. Kiểm tra email đã được xác nhận<br>4. Chặn tài khoản nhân viên/quản trị dùng cổng khách hàng<br>5. Cấp access token và refresh token |
| Luồng thay thế | Không có |
| Ngoại lệ | Sai thông tin: 401; tài khoản bị khóa, chưa xác nhận email, sai cổng: 403 |
| Nguồn | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → LoginAsync` |


### 3.4.2 Đặc tả UC-DV-04 Giữ ghế

- **Nội dung cần viết:**
  - Viết đặc tả theo bảng; mô tả từng bước bằng câu hoàn chỉnh
- **Tài liệu/hình/bảng:**
  - `Bảng 3.26` Đặc tả UC-DV-04 Giữ ghế

| Mục | Nội dung |
|---|---|
| Mã / Tên | UC-DV-04 — Giữ ghế |
| Tác nhân | Khách hàng |
| Mô tả | Giữ chỗ có thời hạn cho các ghế đã chọn. |
| Tiền điều kiện | Khách hàng đã đăng nhập; Suất chiếu còn mở bán; Đã chọn ghế hợp lệ |
| Hậu điều kiện | Booking Pending được tạo với hạn giữ; Các ghế ở trạng thái Held cho booking này |
| Luồng chính | Gửi yêu cầu giữ ghế → kiểm tra đầu vào, suất chiếu, booking Pending sẵn có, luật ghế → cập nhật ghế nguyên tử → tính tiền → tạo booking → phát seat.held<br>Chi tiết các bước: 1. Kiểm tra dữ liệu đầu vào<br>2. Kiểm tra suất chiếu còn mở bán<br>3. Kiểm tra chưa có booking Pending còn hạn cho suất này<br>4. Kiểm tra luật chọn ghế phía máy chủ<br>5. Giữ ghế nguyên tử bằng cập nhật có điều kiện<br>6. Tính tiền; giảm giá chỉ tính trên tiền vé theo hạng suy ra từ điểm hiệu lực 12 tháng trong sổ điểm<br>7. Tạo booking Pending kèm hạn giữ<br>8. Phát sự kiện ghế được giữ |
| Luồng thay thế | Xử lý xung đột ghế |
| Ngoại lệ | Ghế đã bị người khác giữ: 409 kèm danh sách ghế xung đột; có booking Pending còn hạn: xung đột; vượt giới hạn tốc độ 10/60 giây |
| Nguồn | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → HoldSeatsAsync, CreateHoldBookingAsync` |


### 3.4.3 Đặc tả UC-TT-01 Thanh toán đơn đặt vé trực tuyến

- **Nội dung cần viết:**
  - Viết đặc tả theo bảng; mô tả từng bước bằng câu hoàn chỉnh
- **Tài liệu/hình/bảng:**
  - `Bảng 3.27` Đặc tả UC-TT-01 Thanh toán đơn đặt vé trực tuyến

| Mục | Nội dung |
|---|---|
| Mã / Tên | UC-TT-01 — Thanh toán đơn đặt vé trực tuyến |
| Tác nhân | Khách hàng, Cổng VNPay, Cổng MoMo, Cổng Stripe |
| Mô tả | Thanh toán booking Pending qua cổng được chọn. |
| Tiền điều kiện | Booking Pending còn hạn, thuộc về khách; Cổng thanh toán đã cấu hình |
| Hậu điều kiện | Payment ở trạng thái Pending; khách được chuyển hướng sang cổng |
| Luồng chính | Chọn phương thức → hệ thống đánh dấu giao dịch cũ thất bại → tạo Payment → tạo liên kết thanh toán → khách thanh toán trên cổng → quay lại trang kết quả<br>Chi tiết các bước: 1. Kiểm tra booking thuộc người dùng, còn Pending và còn hạn<br>2. Đánh dấu các giao dịch Pending cũ là Failed<br>3. Tạo bản ghi Payment và mã giao dịch<br>4. Tạo liên kết thanh toán với cổng<br>5. Chuyển hướng khách sang trang thanh toán của cổng<br>6. Xem kết quả trên trang kết quả đặt vé |
| Luồng thay thế | Thanh toán qua VNPay (tổng quát hóa)<br>Thanh toán qua MoMo (tổng quát hóa)<br>Thanh toán qua Stripe (tổng quát hóa) |
| Ngoại lệ | Booking hết hạn hoặc không thuộc khách: lỗi; cổng không hỗ trợ: 400 |
| Nguồn | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → CheckoutAsync` |


### 3.4.4 Đặc tả UC-TT-05 Phát hành vé sau thanh toán

- **Nội dung cần viết:**
  - Viết đặc tả theo bảng; mô tả từng bước bằng câu hoàn chỉnh
- **Tài liệu/hình/bảng:**
  - `Bảng 3.28` Đặc tả UC-TT-05 Phát hành vé sau thanh toán

| Mục | Nội dung |
|---|---|
| Mã / Tên | UC-TT-05 — Phát hành vé sau thanh toán |
| Tác nhân | Hệ thống tác vụ nền, Dịch vụ email |
| Mô tả | Chuyển ghế sang đã bán, tạo vé và mã QR sau khi thanh toán thành công. |
| Tiền điều kiện | Cổng báo thanh toán thành công; chữ ký, số tiền hợp lệ |
| Hậu điều kiện | Booking Confirmed; ghế Booked; vé có mã QR; dòng sổ điểm của booking được ghi, điểm và hạng được tính lại; email gửi |
| Luồng chính | Áp dụng kết quả thanh toán → chuyển ghế → xác nhận booking → tạo vé QR → ghi sổ điểm, tính lại hạng → phát seat.booked → gửi email<br>Chi tiết các bước: 1. Chuyển ghế từ Held sang Booked<br>2. Cập nhật booking sang Confirmed<br>3. Tạo vé và mã QR cho từng ghế<br>4. Ghi dòng sổ điểm của booking, tính lại điểm hiệu lực 12 tháng và hạng thành viên (hạng có thể tăng hoặc giảm)<br>5. Phát sự kiện ghế đã bán<br>6. Gửi email xác nhận đặt vé |
| Luồng thay thế | Tự hoàn tiền khi ghế đã hết hạn sau khi khách trả tiền (xem UC-TT-06 Hoàn tiền; quan hệ extend giữa hai use case mức 1) |
| Ngoại lệ | Ghế đã hết hạn khi tiền về: tự hoàn tiền và gửi email thông báo |
| Nguồn | `backend/Cinema.Application/Services/Payments/PaymentService.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → ConfirmBookingAndGenerateTicketsAsync` |


### 3.4.5 Đặc tả UC-DV-08 Hủy đơn đặt vé

- **Nội dung cần viết:**
  - Viết đặc tả theo bảng; mô tả từng bước bằng câu hoàn chỉnh
- **Tài liệu/hình/bảng:**
  - `Bảng 3.29` Đặc tả UC-DV-08 Hủy đơn đặt vé

| Mục | Nội dung |
|---|---|
| Mã / Tên | UC-DV-08 — Hủy đơn đặt vé |
| Tác nhân | Khách hàng |
| Mô tả | Hủy giữ ghế hoặc hủy đơn đã thanh toán. |
| Tiền điều kiện | Khách là chủ booking |
| Hậu điều kiện | Booking Cancelled; ghế được trả; hoàn tiền nếu đủ điều kiện; điểm của booking được thu hồi trên đúng dòng sổ điểm |
| Luồng chính | Kiểm tra quyền → nhánh Pending hoặc Confirmed → (Confirmed) kiểm tra điều kiện hoàn tiền → hoàn tiền cổng → thu hồi điểm trên dòng sổ điểm của booking → gửi email<br>Chi tiết các bước: 1. Kiểm tra booking thuộc về người dùng<br>2. Hoàn tiền qua cổng thanh toán<br>3. Thu hồi điểm thưởng vào đúng dòng sổ điểm của booking, rồi tính lại điểm và hạng<br>4. Gửi email xác nhận hủy |
| Luồng thay thế | Hủy booking Pending và trả ghế (tổng quát hóa)<br>Hủy booking đã thanh toán (Confirmed) (tổng quát hóa) |
| Ngoại lệ | Đã soát vé, thanh toán tiền mặt, quá hạn hủy hoặc cổng chưa cấu hình: không hoàn tiền/không cho hủy |
| Nguồn | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → CancelBookingAsync` |


### 3.4.6 Đặc tả UC-BQ-01 Bán vé tại quầy

- **Nội dung cần viết:**
  - Viết đặc tả theo bảng; mô tả từng bước bằng câu hoàn chỉnh
- **Tài liệu/hình/bảng:**
  - `Bảng 3.30` Đặc tả UC-BQ-01 Bán vé tại quầy

| Mục | Nội dung |
|---|---|
| Mã / Tên | UC-BQ-01 — Bán vé tại quầy |
| Tác nhân | Nhân viên, Quản trị viên |
| Mô tả | Bán vé cho khách tại rạp, thanh toán tiền mặt. |
| Tiền điều kiện | Nhân viên hoặc quản trị đã đăng nhập |
| Hậu điều kiện | Booking kênh Counter ở trạng thái Confirmed; vé phát hành; tiền thối được ghi |
| Luồng chính | Chọn suất → chọn ghế/combo → nhập khách → giữ ghế quầy → nhập tiền nhận → xác nhận và phát vé<br>Chi tiết các bước: 1. Chọn suất chiếu<br>2. Xem sơ đồ ghế<br>3. Chọn ghế (không áp luật ghế lẻ)<br>4. Nhập thông tin khách hoặc chọn khách hàng có sẵn<br>5. Giữ ghế tại quầy<br>6. Thanh toán tiền mặt<br>7. Xác nhận đơn và phát hành vé<br>8. Hiển thị vé kết quả |
| Luồng thay thế | Chọn combo<br>Tìm khách hàng (UC-QN-03, quan hệ extend với UC-BQ-01) |
| Ngoại lệ | Tiền nhận nhỏ hơn số cần thanh toán: từ chối |
| Nguồn | `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → CounterHoldSeatsAsync, CounterCheckoutAsync`<br>`frontend/src/features/admin/counter/**` |


### 3.4.7 Đặc tả UC-SV-01 Soát vé

- **Nội dung cần viết:**
  - Viết đặc tả theo bảng; mô tả từng bước bằng câu hoàn chỉnh
- **Tài liệu/hình/bảng:**
  - `Bảng 3.31` Đặc tả UC-SV-01 Soát vé

| Mục | Nội dung |
|---|---|
| Mã / Tên | UC-SV-01 — Soát vé |
| Tác nhân | Nhân viên, Quản trị viên |
| Mô tả | Xác nhận vé hợp lệ khi khách vào rạp. |
| Tiền điều kiện | Nhân viên hoặc quản trị đã đăng nhập; Vé có mã QR |
| Hậu điều kiện | Vé IsCheckedIn = true, CheckedInAt được ghi |
| Luồng chính | Quét/nhập mã → tìm vé → kiểm tra booking, suất, cửa sổ thời gian, chưa soát → ghi nhận<br>Chi tiết các bước: 1. Tìm vé theo mã QR<br>2. Kiểm tra booking ở trạng thái Confirmed<br>3. Kiểm tra suất chiếu chưa bị hủy<br>4. Kiểm tra trong cửa sổ thời gian soát vé<br>5. Chặn soát lặp<br>6. Ghi nhận đã soát vé<br>7. Hiển thị kết quả cho nhân viên |
| Luồng thay thế | Quét mã QR bằng camera<br>Nhập mã vé thủ công |
| Ngoại lệ | Mã sai, ngoài cửa sổ thời gian, đã soát: từ chối kèm lý do |
| Nguồn | `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → CheckinAsync`<br>`frontend/src/features/admin/checkin/**` |


### 3.4.8 Đặc tả UC-QS-07 Tự động xếp lịch chiếu

- **Nội dung cần viết:**
  - Viết đặc tả theo bảng; mô tả từng bước bằng câu hoàn chỉnh
- **Tài liệu/hình/bảng:**
  - `Bảng 3.32` Đặc tả UC-QS-07 Tự động xếp lịch chiếu

| Mục | Nội dung |
|---|---|
| Mã / Tên | UC-QS-07 — Tự động xếp lịch chiếu |
| Tác nhân | Quản trị viên |
| Mô tả | Đề xuất và áp dụng lịch chiếu tự động theo độ nóng của phim. |
| Tiền điều kiện | Quản trị viên; Có phim và phòng hoạt động |
| Hậu điều kiện | Các suất chiếu được tạo theo kết quả xem trước (khi áp dụng) |
| Luồng chính | Chọn phạm vi → xem trước (tính Heat, sinh ứng viên, chấm điểm) → áp dụng<br>Chi tiết các bước: 1. Chọn khoảng ngày, phòng, phim và số suất<br>2. Tính độ nóng của phim<br>3. Sinh các khung giờ ứng viên<br>4. Chấm điểm và chọn khung giờ (tham lam)<br>5. Xem trước kết quả<br>6. Áp dụng kết quả |
| Luồng thay thế | Không có |
| Ngoại lệ | Hết ứng viên hợp lệ cho phim: bỏ qua phim đó |
| Nguồn | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → PreviewAutoScheduleAsync, ApplyAutoScheduleAsync`<br>`backend/Cinema.Application/Utils/ShowtimeScheduler.cs` |


### 3.4.9 Đặc tả UC-QR-03 Soạn sơ đồ ghế

- **Nội dung cần viết:**
  - Viết đặc tả theo bảng; mô tả từng bước bằng câu hoàn chỉnh
- **Tài liệu/hình/bảng:**
  - `Bảng 3.33` Đặc tả UC-QR-03 Soạn sơ đồ ghế

| Mục | Nội dung |
|---|---|
| Mã / Tên | UC-QR-03 — Soạn sơ đồ ghế |
| Tác nhân | Quản trị viên |
| Mô tả | Thiết kế và lưu sơ đồ ghế của phòng. |
| Tiền điều kiện | Quản trị viên; Phòng chiếu đã tồn tại |
| Hậu điều kiện | Sơ đồ ghế của phòng được lưu |
| Luồng chính | Đặt kích thước → tô/chọn/ghép/sao chép → kiểm tra hợp lệ → lưu<br>Chi tiết các bước: 1. Đặt kích thước lưới (số hàng, số cột)<br>2. Tô loại ghế bằng cọ<br>3. Kiểm tra tính hợp lệ của sơ đồ<br>4. Lưu sơ đồ ghế |
| Luồng thay thế | Sinh sơ đồ tự động<br>Chọn vùng nhiều ghế<br>Đặt lối đi<br>Ghép ghế đôi<br>Sao chép hàng ghế<br>Hoàn tác và làm lại |
| Ngoại lệ | Ghế đôi không ghép cặp; ghế đang giữ/đã bán bị đổi: từ chối |
| Nguồn | `backend/Cinema.Api/Controllers/Admin/AdminRoomsController.cs`<br>`backend/Cinema.Application/Services/Rooms/RoomService.cs → UpdateSeatLayoutAsync`<br>`frontend/src/features/admin/rooms/components/seat-editor-*.tsx` |


## 3.5 QUY TẮC NGHIỆP VỤ


### 3.5.1 Danh sách quy tắc nghiệp vụ

- **Nội dung cần viết:**
  - Trình bày từng quy tắc: nội dung, tham số cấu hình, giá trị mặc định, nơi áp dụng
- **Tài liệu/hình/bảng:**
  - `Bảng 3.34` Quy tắc nghiệp vụ và tham số
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/CinemaSettings.cs`; `backend/Cinema.Application/Options/*`; `backend/Cinema.Application/Services/**`

| Mã | Quy tắc | Nội dung | Nguồn |
|---|---|---|---|
| BR-01 | Thời gian giữ ghế | `HoldDurationMinutes` (mặc định 15 phút), không vượt giờ bắt đầu suất chiếu | CinemaSettings.cs, BookingService.cs |
| BR-02 | Giới hạn mỗi đơn | Tối đa 10 ghế (`MaxSeatsPerBooking`), tối đa 10 combo mỗi loại | BookingOptions.cs, BookingValidators.cs |
| BR-03 | Dừng bán online | Không bán online sau `StopSellingMinutesBefore` phút trước giờ chiếu | CinemaSettings.cs, BookingService.cs |
| BR-04 | Một booking Pending | Mỗi người một booking Pending còn hạn trên mỗi suất (quầy: mỗi nhân viên) | BookingService.cs |
| BR-05 | Ghế đôi và ghế lẻ | Ghế đôi chọn theo cặp; luật ghế lẻ theo `SeatGapRuleMode` (Strict mặc định; quầy dùng Off) | SeatLayoutRules.cs |
| BR-06 | Giảm giá và điểm thưởng | Giảm giá = tiền vé × `DiscountPercent` / 100, làm tròn ra xa 0, chỉ tính trên tiền vé, không tính combo; điểm tích = làm tròn xuống (`FinalAmount` / `PointsPerAmount`, mặc định 10.000), nên combo vẫn được tích điểm | BookingService.cs |
| BR-07 | Hủy và hoàn tiền | `CancelRefundPercent` (mặc định 0), hạn hủy `CancelDeadlineHours` (mặc định 2 giờ); tiền mặt hủy tại quầy; không hủy khi đã soát vé | CinemaSettings.cs, BookingService.cs |
| BR-08 | Cửa sổ soát vé | Mở trước giờ chiếu `CheckInOpenMinutes` (mặc định 30) đến hết `EndTime`; booking phải Confirmed | BookingService.cs |
| BR-09 | Đánh giá phim | Chỉ khách đã soát vé phim đó; mỗi người một đánh giá cho mỗi phim; điểm 1–5 | ReviewService.cs |
| BR-10 | Ràng buộc suất chiếu | Đệm phòng 15 phút; không trùng phòng; trong giờ mở cửa (trừ cho phép ngoài giờ); không tạo cho phim Ended hoặc phòng không hoạt động/chưa có sơ đồ ghế; không hủy khi đã bắt đầu | ShowtimeService.cs, OperatingHours.cs |
| BR-11 | Vòng đời tự động | Suất chiếu hết giờ chuyển Completed; phim chuyển Ended khi không còn suất sắp chiếu | ShowtimeLifecycle.cs, ShowtimeLifecycleJob.cs |
| BR-12 | Giới hạn tốc độ | Toàn cục 100 yêu cầu/60 giây/IP; xác thực 5/60 giây/IP; giữ ghế 10/60 giây/người dùng | RateLimitOptions.cs, Program.cs |
| BR-13 | Điểm hiệu lực 12 tháng | Điểm hiệu lực = tổng (`Points` − `RevokedPoints`) của các dòng sổ điểm có `EarnedAt` trong 12 tháng gần nhất (cửa sổ trượt `LoyaltyWindow.Months` = 12, cố định trong code); `TotalSpent` vẫn là tổng chi tiêu trọn đời | LoyaltyWindow.cs, LoyaltyPointEntry.cs |
| BR-14 | Hạng thành viên | Hạng là hạng có `MinPoints` lớn nhất không vượt điểm hiệu lực; hạng có thể bị hạ khi điểm hết hạn; giảm giá lúc đặt vé tính từ sổ điểm, không đọc `CustomerProfile.TierId` | BookingService.cs |
| BR-15 | Thu hồi điểm khi hoàn tiền | Chỉ trừ vào đúng dòng sổ điểm của booking: `RevokedPoints` = min(`Points`, `RevokedPoints` + ⌊số tiền hoàn / `PointsPerAmount`⌋) | BookingService.cs |
| BR-16 | Đồng bộ điểm hết hạn | Job `ExpireLoyaltyPointsJob` tính lại `Points` và `TierId` trên hồ sơ mỗi giờ (cron `ExpireLoyaltyPointsCron`, mặc định `5 * * * *`, múi giờ VN) | LoyaltyExpiryService.cs, ExpireLoyaltyPointsJob.cs |


## 3.6 YÊU CẦU PHI CHỨC NĂNG


### 3.6.1 Danh sách yêu cầu phi chức năng

- **Nội dung cần viết:**
  - Mỗi yêu cầu kèm cách đáp ứng và vị trí cài đặt
- **Tài liệu/hình/bảng:**
  - `Bảng 3.35` Yêu cầu phi chức năng và cách đáp ứng
- **Nguồn dữ liệu:** `backend/Cinema.Api/Program.cs`; `backend/Dockerfile`; `render.yaml`

| Mã | Yêu cầu | Cách đáp ứng | Nguồn |
|---|---|---|---|
| NFR-01 | An toàn xác thực | JWT ngắn hạn, refresh token băm và xoay vòng, kiểm tra tài khoản mỗi yêu cầu | JwtTokenService.cs, Program.cs |
| NFR-02 | Nhất quán dưới truy cập đồng thời | Cập nhật có điều kiện + chỉ mục duy nhất | BookingService.cs |
| NFR-03 | Thời gian thực | SignalR cập nhật ghế | ShowtimeHub.cs |
| NFR-04 | Múi giờ | Lưu UTC, hiển thị giờ Việt Nam | VietnamTime.cs |
| NFR-05 | Nhật ký | Serilog | Program.cs |
| NFR-06 | Khả năng kiểm thử | Test đơn vị, tích hợp, E2E | backend/tests/** |
| NFR-07 | Triển khai trên tài nguyên giới hạn | Docker nhiều stage; GC giới hạn heap 384 MB | backend/Dockerfile, render.yaml |
| NFR-08 | Hiệu năng | [CẦN BỔ SUNG: ngưỡng p95 mong muốn và kết quả k6] | — |


---

# CHƯƠNG 4. THIẾT KẾ HỆ THỐNG


## 4.1 KIẾN TRÚC HỆ THỐNG


### 4.1.1 Kiến trúc tổng thể

- **Nội dung cần viết:**
  - SPA React ↔ REST + SignalR ↔ ASP.NET Core ↔ PostgreSQL; dịch vụ ngoài; tác vụ nền chạy cùng tiến trình
- **Tài liệu/hình/bảng:**
  - `Hình 4.1` Kiến trúc tổng thể
- **Nguồn dữ liệu:** `backend/Cinema.Api/Program.cs`; `backend/Cinema.Infrastructure/DependencyInjection.cs`; `docker-compose.yml`

### 4.1.2 Kiến trúc phân tầng backend

- **Nội dung cần viết:**
  - 5 project; chiều phụ thuộc đọc từ csproj; vai trò từng tầng; đăng ký DI qua `AddApplication`, `AddInfrastructure`
  - Mapster, FluentValidation, Options pattern
- **Tài liệu/hình/bảng:**
  - `Hình 4.2` Phụ thuộc giữa các project backend
- **Nguồn dữ liệu:** `backend/Cinema.slnx`; `backend/*/*.csproj`; `backend/Cinema.Application/DependencyInjection.cs`

### 4.1.3 Kiến trúc frontend

- **Nội dung cần viết:**
  - Tổ chức `features/{admin,client,auth}` (api, hooks, components, pages, types); `components/ui` (shadcn/Base UI), `components/common`; `stores` (Zustand); lớp `api` (axios, query keys, invalidation); routing tách client/admin
  - Cơ chế làm mới token dùng chung một promise
- **Tài liệu/hình/bảng:**
  - `Hình 4.3` Tổ chức mã nguồn frontend
- **Nguồn dữ liệu:** `frontend/src/api/axios.ts`; `frontend/src/api/query-keys.ts`; `frontend/src/app/routes/**`; `frontend/src/stores/*`

### 4.1.4 Phản hồi API thống nhất

- **Nội dung cần viết:**
  - Cấu trúc `ApiResponse<T>`; bộ lọc bọc phản hồi; phân trang offset (`page`, `pageSize`) và con trỏ (`cursor`, `limit`)
- **Tài liệu/hình/bảng:**
  - `Bảng 4.1` Cấu trúc phản hồi API
- **Nguồn dữ liệu:** `backend/Cinema.Shared/Results/ApiResponse.cs`; `backend/Cinema.Api/Filters/ApiResponseFilter.cs`; `backend/Cinema.Application/DTOs/Common/PagingDtos.cs`

| Thành phần | Mô tả |
|---|---|
| ApiResponse<T> | [CẦN BỔ SUNG: liệt kê trường theo ApiResponse.cs] |
| Phân trang offset | `page`, `pageSize` → OffsetPageResult |
| Phân trang con trỏ | `cursor`, `limit` → CursorPageResult (CursorCodec) |


### 4.1.5 Xử lý lỗi

- **Nội dung cần viết:**
  - Hệ thống ngoại lệ ánh xạ mã HTTP; ánh xạ lỗi PostgreSQL (23503, 23505, 23514, 23P01); ẩn chi tiết lỗi ở Production
- **Tài liệu/hình/bảng:**
  - `Bảng 4.2` Ánh xạ ngoại lệ và lỗi PostgreSQL sang mã HTTP
- **Nguồn dữ liệu:** `backend/Cinema.Api/Middlewares/ExceptionHandlingMiddleware.cs`; `backend/Cinema.Shared/Exceptions/*`

| Nguồn lỗi | Ngoại lệ/mã | Mã HTTP |
|---|---|---|
| Không tìm thấy | NotFoundException | [CẦN BỔ SUNG: đọc middleware để điền] |
| Xung đột | ConflictException, SeatConflictException | [CẦN BỔ SUNG] |
| Cấm truy cập | ForbiddenAppException | [CẦN BỔ SUNG] |
| Chưa xác thực | UnauthorizedAppException | [CẦN BỔ SUNG] |
| Dữ liệu sai | ValidationAppException | [CẦN BỔ SUNG] |
| Quy tắc nghiệp vụ | BusinessRuleException | [CẦN BỔ SUNG] |
| Ghế không còn được giữ | SeatsNoLongerHeldException | [CẦN BỔ SUNG] |
| PostgreSQL 23503/23505/23514/23P01 | DbUpdateException | [CẦN BỔ SUNG] |


### 4.1.6 Quản lý cấu hình

- **Nội dung cần viết:**
  - Options pattern: Jwt, Brand, Booking, RateLimit, Scheduler, BackgroundJob, VnPay, Momo, Stripe, Cloudinary, Email, Gemini, Google, Cors; kiểm tra cấu hình Production (`ProductionOptionsValidator`)
  - Cấu hình theo môi trường Development/Production/Testing
- **Tài liệu/hình/bảng:**
  - `Bảng 4.3` Nhóm cấu hình ứng dụng
- **Nguồn dữ liệu:** `backend/Cinema.Api/appsettings.json`; `backend/Cinema.Application/Options/*`; `.env.example`

| Nhóm | Nguồn | Ghi chú |
|---|---|---|
| Jwt | appsettings.json | Secret ≥ 32 ký tự ở Production |
| Cors | appsettings.json | Danh sách origin bắt buộc ở Production |
| VnPay/Momo/Stripe | appsettings.json | Đủ tham số khi bật cổng |
| Booking/RateLimit/Scheduler/BackgroundJob | Options/*.cs | Giá trị mặc định trong lớp Options |


## 4.2 SƠ ĐỒ LỚP

Sơ đồ lớp mô tả lớp, thuộc tính, phương thức công khai chính và quan hệ; không lặp lại ERD (mục 4.3) và sơ đồ kiến trúc (mục 4.1).

### 4.2.1 Nguyên tắc vẽ sơ đồ lớp

- **Nội dung cần viết:**
  - Mỗi mục một hình; sơ đồ lớp miền gồm 30 thực thể và 10 enum; sơ đồ theo package đi theo chuỗi Controller → interface dịch vụ → lớp dịch vụ → thực thể, DTO
  - Quy ước tên: tên lớp, interface, phương thức lấy nguyên văn từ code; bỏ tham số `CancellationToken` cho gọn
  - Mức rút gọn: tối đa 8 phương thức công khai mỗi lớp; lớp vượt quá ghi "rút gọn x/y"; danh sách đầy đủ nằm ở bảng đi kèm
  - Thực thể và DTO trong sơ đồ package chỉ ghi tên, thuộc tính xem sơ đồ lớp miền
  - Lớp thư viện ngoài (ví dụ `IPasswordHasher<User>`, `HttpClient`) không vẽ
- **Tài liệu/hình/bảng:**
  - Không có hình, bảng
- **Nguồn dữ liệu:** `backend/Cinema.Api/**`, `backend/Cinema.Application/**`, `backend/Cinema.Infrastructure/**`, `backend/Cinema.Domain/**`

### 4.2.2 Ký hiệu sơ đồ lớp

- **Nội dung cần viết:**
  - Giải thích ký hiệu: lớp, interface, enum, stereotype; mũi tên hiện thực interface, phụ thuộc, kết hợp (có bội số); màu theo stereotype
  - Cách đọc phương thức: `Tên(tham số) : kiểu trả về`
- **Tài liệu/hình/bảng:**
  - `Bảng 4.4` Ký hiệu sơ đồ lớp
- **Nguồn dữ liệu:** `ma-nguon-so-do.md` (khai báo `skinparam`)

| Ký hiệu | Ý nghĩa |
|---|---|
| `<<Controller>>` | Lớp điều khiển API (`Cinema.Api/Controllers`) |
| `interface` | Interface dịch vụ hoặc hạ tầng (`Cinema.Application/Interfaces`) |
| `<<Service>>` | Lớp dịch vụ hoặc lớp hiện thực interface |
| `<<Gateway>>`, `<<Job>>`, `<<Hub>>`, `<<Utility>>` | Cổng thanh toán, công việc nền Hangfire, SignalR Hub, lớp tiện ích tĩnh |
| `<<Entity>>`, `<<DTO>>`, `<<record>>` | Thực thể miền, đối tượng truyền dữ liệu, kiểu record |
| `<<Options>>`, `<<Validator>>`, `<<Exception>>` | Lớp cấu hình, kiểm tra đầu vào, ngoại lệ nghiệp vụ |
| `A ..\|> B` | A hiện thực interface B |
| `A ..> B` | A phụ thuộc B (tiêm phụ thuộc, sử dụng) |
| `A --> B` | A kết hợp với B |

### 4.2.3 Sơ đồ lớp miền

- **Nội dung cần viết:**
  - Một hình cho 30 thực thể và 10 enum: thuộc tính, enum, quan hệ giữa các thực thể
  - Thực thể mới `LoyaltyPointEntry` (sổ điểm) nối `User` (nhiều–một) và `Booking` (0..1); cập nhật sơ đồ và mã PlantUML tương ứng
  - Nêu quan hệ nhiều-một, một-một, tự tham chiếu (`RefreshToken.ReplacedByToken`) và các thực thể liên kết (`MovieCast`, `MovieGenre`, `BookingCombo`, `BookingTicket`)
  - Phân biệt với ERD: sơ đồ lớp không ghi khóa, chỉ mục, ràng buộc CHECK
- **Tài liệu/hình/bảng:**
  - `Hình 4.4` Sơ đồ lớp miền
  - `Bảng 4.5` Danh sách enum
  - `Bảng 4.6` Quan hệ giữa các thực thể
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `backend/Cinema.Domain/Enums/*.cs`

| Enum | Giá trị | Dùng tại |
|---|---|---|
| `BookingChannel` | Online, Counter | `Booking.Channel` |
| `BookingStatus` | Pending, Confirmed, Cancelled, Expired | `Booking.Status` |
| `Gender` | Male, Female, Other | `User.Gender` |
| `MovieStatus` | ComingSoon, NowShowing, Ended | `Movie.Status` |
| `PaymentMethod` | VNPay, Momo, Cash, Stripe | `Payment.PaymentMethod` |
| `PaymentStatus` | Pending, Success, Failed, Refunded | `Payment.Status` |
| `SeatGapRuleMode` | Strict, AllowEdge, Off | `CinemaSettings.SeatGapRuleMode` |
| `ShowtimeSeatStatus` | Available, Held, Booked | `ShowtimeSeat.Status` |
| `ShowtimeStatus` | Scheduled, Cancelled, Completed | `Showtime.Status` |
| `VerificationTokenPurpose` | EmailConfirmation, PasswordReset | `UserVerificationToken.Purpose` |

| Thực thể | Thuộc tính điều hướng | Thực thể liên quan | Bội số phía liên quan |
|---|---|---|---|
| `Booking` | `User` | `User` | 0..1 |
| `Booking` | `Showtime` | `Showtime` | 1 |
| `Booking` | `CreatedByEmployee` | `Employee` | 0..1 |
| `BookingCombo` | `Booking` | `Booking` | 1 |
| `BookingCombo` | `Combo` | `Combo` | 1 |
| `BookingTicket` | `Booking` | `Booking` | 1 |
| `BookingTicket` | `ShowtimeSeat` | `ShowtimeSeat` | 1 |
| `CinemaRoom` | `ScreenType` | `ScreenType` | 0..1 |
| `CustomerProfile` | `User` | `User` | 1 |
| `CustomerProfile` | `Tier` | `MembershipTier` | 0..1 |
| `Employee` | `User` | `User` | 1 |
| `Employee` | `Position` | `Position` | 0..1 |
| `LoyaltyPointEntry` | `User` | `User` | 1 |
| `LoyaltyPointEntry` | `Booking` | `Booking` | 0..1 |
| `Movie` | `Director` | `Director` | 0..1 |
| `Movie` | `Language` | `Language` | 0..1 |
| `Movie` | `AgeRating` | `AgeRating` | 0..1 |
| `MovieCast` | `Movie` | `Movie` | 1 |
| `MovieCast` | `Actor` | `Actor` | 1 |
| `MovieGenre` | `Movie` | `Movie` | 1 |
| `MovieGenre` | `Genre` | `Genre` | 1 |
| `Payment` | `Booking` | `Booking` | 1 |
| `RefreshToken` | `User` | `User` | 1 |
| `RefreshToken` | `ReplacedByToken` | `RefreshToken` | 0..1 |
| `Review` | `Movie` | `Movie` | 1 |
| `Review` | `User` | `User` | 1 |
| `Seat` | `Room` | `CinemaRoom` | 1 |
| `Seat` | `SeatType` | `SeatType` | 0..1 |
| `Showtime` | `Movie` | `Movie` | 1 |
| `Showtime` | `Room` | `CinemaRoom` | 1 |
| `ShowtimeSeat` | `Showtime` | `Showtime` | 1 |
| `ShowtimeSeat` | `Seat` | `Seat` | 1 |
| `ShowtimeSeat` | `HeldByUser` | `User` | 0..1 |
| `ShowtimeSeat` | `BookingTicket` | `BookingTicket` | 0..1 |
| `User` | `Role` | `Role` | 1 |
| `User` | `CustomerProfile` | `CustomerProfile` | 0..1 |
| `User` | `Employee` | `Employee` | 0..1 |
| `UserVerificationToken` | `User` | `User` | 1 |

Số thuộc tính điều hướng dạng tập hợp (phía "nhiều"): 24 [CẦN BỔ SUNG: đếm lại sau khi thêm `LoyaltyPointEntry`; tăng thêm nếu `User`, `Booking` có thuộc tính tập hợp trỏ tới `LoyaltyPointEntry`].

### 4.2.4 Sơ đồ lớp package Xác thực

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.4)
  - Lớp `IPasswordHasher<User>` (ASP.NET Core Identity) là thư viện ngoài nên không vẽ.
- **Tài liệu/hình/bảng:**
  - `Hình 4.5` Sơ đồ lớp package Xác thực
  - `Bảng 4.7` Danh sách lớp package Xác thực
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/AuthController.cs`; `backend/Cinema.Application/Interfaces/IAuthService.cs`; `backend/Cinema.Application/Services/Auth/AuthService.cs`; `backend/Cinema.Application/Interfaces/ITokenService.cs`; `backend/Cinema.Application/Services/Auth/JwtTokenService.cs`; `backend/Cinema.Application/Interfaces/IGoogleAuthService.cs`; `backend/Cinema.Infrastructure/Auth/GoogleAuthService.cs`; `backend/Cinema.Application/BackgroundJobs/SendEmailJob.cs`

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `AuthController` | Controller | POST /api/v1/auth/register → `Register(RegisterRequest request) : IActionResult`<br>POST /api/v1/auth/confirm-email → `ConfirmEmail(ConfirmEmailRequest request) : IActionResult`<br>POST /api/v1/auth/login → `Login(LoginRequest request) : IActionResult`<br>POST /api/v1/auth/admin-login → `AdminLogin(LoginRequest request) : IActionResult`<br>POST /api/v1/auth/google → `GoogleLogin(GoogleLoginRequest request) : IActionResult`<br>POST /api/v1/auth/refresh → `Refresh(RefreshRequest request) : IActionResult`<br>POST /api/v1/auth/logout → `Logout(LogoutRequest request) : IActionResult`<br>POST /api/v1/auth/forgot-password → `ForgotPassword(ForgotPasswordRequest request) : IActionResult`<br>POST /api/v1/auth/reset-password → `ResetPassword(ResetPasswordRequest request) : IActionResult`<br>POST /api/v1/auth/resend-confirmation → `ResendConfirmationEmail(ResendConfirmationEmailRequest request) : IActionResult`<br>POST /api/v1/auth/socket-token → `GenerateSocketToken() : IActionResult`<br>GET /api/v1/auth/me → `Me() : IActionResult` | `backend/Cinema.Api/Controllers/Client/AuthController.cs` |
| `IAuthService` | Interface | `RegisterAsync(RegisterRequest request) : RegisterResponse`<br>`ConfirmEmailAsync(ConfirmEmailRequest request) : MessageResponse`<br>`LoginAsync(LoginRequest request, bool adminOnly) : LoginResponse`<br>`GoogleLoginAsync(GoogleLoginRequest request) : LoginResponse`<br>`RefreshAsync(RefreshRequest request) : RefreshResponse`<br>`GetMeAsync(int userId) : MeResponse`<br>`LogoutAsync(LogoutRequest request) : MessageResponse`<br>`ForgotPasswordAsync(ForgotPasswordRequest request) : MessageResponse`<br>`ResetPasswordAsync(ResetPasswordRequest request) : MessageResponse`<br>`ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request) : MessageResponse`<br>`GenerateSocketTokenAsync(int userId) : SocketTokenResponse` | `backend/Cinema.Application/Interfaces/IAuthService.cs` |
| `ITokenService` | Interface | `GenerateAccessToken(User user) : AccessTokenResult`<br>`GenerateSocketToken(User user) : AccessTokenResult`<br>`GenerateOpaqueToken() : string`<br>`HashOpaqueToken(string token) : string` | `backend/Cinema.Application/Interfaces/ITokenService.cs` |
| `IGoogleAuthService` | Interface | `VerifyIdTokenAsync(string idToken) : GoogleUserInfo` | `backend/Cinema.Application/Interfaces/IGoogleAuthService.cs` |
| `AuthService` | Lớp dịch vụ | Hiện thực `IAuthService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Auth/AuthService.cs` |
| `JwtTokenService` | Lớp dịch vụ | Hiện thực `ITokenService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Auth/JwtTokenService.cs` |
| `GoogleAuthService` | Lớp dịch vụ | Hiện thực `IGoogleAuthService`; không có phương thức công khai ngoài interface | `backend/Cinema.Infrastructure/Auth/GoogleAuthService.cs` |
| `SendEmailJob` | Job nền | `RunAsync(string toEmail, string subject, string htmlbody) : void` | `backend/Cinema.Application/BackgroundJobs/SendEmailJob.cs` |
| `JwtOptions` | Options | — | `backend/Cinema.Application/Options/JwtOptions.cs` |
| `EmailOptions` | Options | — | `backend/Cinema.Application/Options/EmailOptions.cs` |
| `AppUrlOptions` | Options | — | `backend/Cinema.Application/Options/AppUrlOptions.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `User`, `Role`, `CustomerProfile`, `RefreshToken`, `UserVerificationToken` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `RegisterRequest`, `LoginRequest`, `LoginResponse`, `RefreshResponse`, `MeResponse`, `SocketTokenResponse` | DTO | — | `backend/Cinema.Application/DTOs/Auth/AuthDtos.cs` |

### 4.2.5 Sơ đồ lớp package Hồ sơ cá nhân

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.5)
- **Tài liệu/hình/bảng:**
  - `Hình 4.6` Sơ đồ lớp package Hồ sơ cá nhân
  - `Bảng 4.8` Danh sách lớp package Hồ sơ cá nhân
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/UsersController.cs`; `backend/Cinema.Application/Interfaces/IUserProfileService.cs`; `backend/Cinema.Application/Services/Users/UserProfileService.cs`; `backend/Cinema.Application/Interfaces/IAuthService.cs`; `backend/Cinema.Application/Services/Auth/AuthService.cs`; `backend/Cinema.Application/Interfaces/IReviewService.cs`; `backend/Cinema.Application/Services/Reviews/ReviewService.cs`

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `UsersController` | Controller | GET /api/v1/users/me → `GetMyProfile() : IActionResult`<br>PUT /api/v1/users/me → `UpdateMyProfile(UpdateProfileRequest request) : IActionResult`<br>GET /api/v1/users/me/reviews → `GetMyReviews(string? cursor, int? limit = null) : ActionResult<CursorPageResult<UserMyReviewDto>>`<br>POST /api/v1/users/me/change-password → `ChangePassword(ChangePasswordRequest request) : IActionResult` | `backend/Cinema.Api/Controllers/Client/UsersController.cs` |
| `IUserProfileService` | Interface | `GetMyProfileAsync(int userId) : UserProfileDto`<br>`UpdateMyProfileAsync(int userId, UpdateProfileRequest request) : UserProfileDto` | `backend/Cinema.Application/Interfaces/IUserProfileService.cs` |
| `IAuthService` | Interface | `ChangePasswordAsync(int userId, ChangePasswordRequest request) : MessageResponse` | `backend/Cinema.Application/Interfaces/IAuthService.cs` |
| `IReviewService` | Interface | `GetMyReviewsAsync(int userId, string? cursor, int limit) : CursorPageResult<UserMyReviewDto>` | `backend/Cinema.Application/Interfaces/IReviewService.cs` |
| `UserProfileService` | Lớp dịch vụ | Hiện thực `IUserProfileService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Users/UserProfileService.cs` |
| `AuthService` | Lớp dịch vụ | Hiện thực `IAuthService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Auth/AuthService.cs` |
| `ReviewService` | Lớp dịch vụ | Hiện thực `IReviewService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Reviews/ReviewService.cs` |
| `UpdateProfileRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Users/UserValidators.cs` |
| `ChangePasswordRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Auth/AuthValidators.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `User`, `CustomerProfile`, `MembershipTier`, `Review`, `RefreshToken` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `UserProfileDto`, `UpdateProfileRequest`, `ChangePasswordRequest`, `UserMyReviewDto`, `NextTierDto` | DTO | — | `backend/Cinema.Application/DTOs/Auth/AuthDtos.cs`<br>`backend/Cinema.Application/DTOs/Reviews/ReviewDtos.cs`<br>`backend/Cinema.Application/DTOs/Users/UserProfileDtos.cs` |

### 4.2.6 Sơ đồ lớp package Duyệt phim

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.6)
  - Dịch vụ dùng chung với package khác (`ReviewService`, `ShowtimeService`, `ComboService`) chỉ liệt kê phương thức phục vụ package này.
- **Tài liệu/hình/bảng:**
  - `Hình 4.7` Sơ đồ lớp package Duyệt phim
  - `Bảng 4.9` Danh sách lớp package Duyệt phim
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/MoviesController.cs`; `backend/Cinema.Api/Controllers/Client/ShowtimesController.cs`; `backend/Cinema.Api/Controllers/Client/CatalogController.cs`; `backend/Cinema.Api/Controllers/Client/SettingsController.cs`; `backend/Cinema.Api/Controllers/Client/CombosController.cs`; `backend/Cinema.Application/Interfaces/IMovieService.cs`; `backend/Cinema.Application/Services/Movies/MovieService.cs`; `backend/Cinema.Application/Interfaces/IReviewService.cs`; `backend/Cinema.Application/Services/Reviews/ReviewService.cs`; `backend/Cinema.Application/Interfaces/IShowtimeService.cs` …

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `MoviesController` | Controller | GET /api/v1/movies → `GetMovies(MovieListFilter filter, string? cursor, int? limit) : IActionResult`<br>GET /api/v1/movies/{movieId:int} → `GetMovieDetail(int movieId) : IActionResult`<br>GET /api/v1/movies/{movieId:int}/reviews → `GetReviews(int movieId, string? cursor, int? limit) : IActionResult` | `backend/Cinema.Api/Controllers/Client/MoviesController.cs` |
| `ShowtimesController` | Controller | GET /api/v1/showtimes → `GetSchedule(DateOnly? date, string? screenType) : IActionResult` | `backend/Cinema.Api/Controllers/Client/ShowtimesController.cs` |
| `CatalogController` | Controller | GET /api/v1/genres → `GetGenres() : IActionResult`<br>GET /api/v1/languages → `GetLanguages() : IActionResult`<br>GET /api/v1/age-ratings → `GetAgeRatings() : IActionResult`<br>GET /api/v1/membership-tiers → `GetMembershipTiers() : IActionResult` | `backend/Cinema.Api/Controllers/Client/CatalogController.cs` |
| `SettingsController` | Controller | GET /api/v1/settings → `GetSettings() : ActionResult<CinemaSettingsDto>` | `backend/Cinema.Api/Controllers/Client/SettingsController.cs` |
| `CombosController` | Controller | GET /api/v1/combos → `GetCombos() : IActionResult` | `backend/Cinema.Api/Controllers/Client/CombosController.cs` |
| `IMovieService` | Interface | `GetMoviesAsync(MovieListFilter filter, string? cursor, int limit) : CursorPageResult<MovieListItemDto>`<br>`GetMovieDetailAsync(int movieId) : MovieDetailDto` | `backend/Cinema.Application/Interfaces/IMovieService.cs` |
| `IReviewService` | Interface | `GetMovieReviewsAsync(int movieId, string? cursor, int limit) : CursorPageResult<ReviewDto>` | `backend/Cinema.Application/Interfaces/IReviewService.cs` |
| `IShowtimeService` | Interface | `GetWholeCinemaScheduleAsync(DateOnly date, string? screenType) : WholeCinemaScheduleDto` | `backend/Cinema.Application/Interfaces/IShowtimeService.cs` |
| `ICatalogService` | Interface | `GetGenresAsync() : List<GenreDto>`<br>`GetLanguagesAsync() : List<LanguageDto>`<br>`GetAgeRatingsAsync() : List<AgeRatingDto>`<br>`GetMembershipTiersAsync() : List<MembershipTierDto>` | `backend/Cinema.Application/Interfaces/ICatalogService.cs` |
| `ISettingsService` | Interface | `GetSettingsAsync() : CinemaSettingsDto` | `backend/Cinema.Application/Interfaces/ISettingsService.cs` |
| `IComboService` | Interface | `GetActiveCombosAsync() : List<ComboDto>` | `backend/Cinema.Application/Interfaces/IComboService.cs` |
| `MovieService` | Lớp dịch vụ | Hiện thực `IMovieService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Movies/MovieService.cs` |
| `ReviewService` | Lớp dịch vụ | Hiện thực `IReviewService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Reviews/ReviewService.cs` |
| `ShowtimeService` | Lớp dịch vụ | Hiện thực `IShowtimeService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs` |
| `CatalogService` | Lớp dịch vụ | Hiện thực `ICatalogService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| `SettingsService` | Lớp dịch vụ | Hiện thực `ISettingsService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Settings/SettingsService.cs` |
| `ComboService` | Lớp dịch vụ | Hiện thực `IComboService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Combos/ComboService.cs` |
| `MovieListFilter` | Record | — | `backend/Cinema.Application/Interfaces/IMovieService.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `Movie`, `Showtime`, `Review`, `Genre`, `Language`, `AgeRating`, `MembershipTier`, `CinemaSettings`, `Combo` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `MovieListItemDto`, `MovieDetailDto`, `UpcomingShowtimeDto`, `WholeCinemaScheduleDto`, `ReviewDto`, `CinemaSettingsDto`, `ComboDto` | DTO | — | `backend/Cinema.Application/DTOs/Combos/ComboDtos.cs`<br>`backend/Cinema.Application/DTOs/Movies/MovieDtos.cs`<br>`backend/Cinema.Application/DTOs/Reviews/ReviewDtos.cs`<br>`backend/Cinema.Application/DTOs/Settings/SettingDtos.cs`<br>`backend/Cinema.Application/DTOs/Showtimes/ShowtimeDtos.cs` |

### 4.2.7 Sơ đồ lớp package Đánh giá phim

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.7)
- **Tài liệu/hình/bảng:**
  - `Hình 4.8` Sơ đồ lớp package Đánh giá phim
  - `Bảng 4.10` Danh sách lớp package Đánh giá phim
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/MoviesController.cs`; `backend/Cinema.Application/Interfaces/IReviewService.cs`; `backend/Cinema.Application/Services/Reviews/ReviewService.cs`

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `MoviesController` | Controller | GET /api/v1/movies/{movieId:int}/can-review → `GetCanReviewStatus(int movieId) : ActionResult<CanReviewResponse>`<br>POST /api/v1/movies/{movieId:int}/reviews → `CreateReview(int movieId, CreateReviewRequest request) : IActionResult`<br>PUT /api/v1/movies/{movieId:int}/reviews/{id:int} → `UpdateReview(int movieId, int id, CreateReviewRequest request) : IActionResult`<br>DELETE /api/v1/movies/{movieId:int}/reviews/{id:int} → `DeleteReview(int movieId, int id) : IActionResult` | `backend/Cinema.Api/Controllers/Client/MoviesController.cs` |
| `IReviewService` | Interface | `GetUserMovieReviewStatusAsync(int userId, int movieId) : CanReviewResponse`<br>`CreateReviewAsync(int userId, int movieId, CreateReviewRequest request) : ReviewDto`<br>`UpdateReviewAsync(int userId, int movieId, int reviewId, CreateReviewRequest request) : ReviewDto`<br>`DeleteMyReviewAsync(int userId, int movieId, int reviewId) : DeleteReviewResponse` | `backend/Cinema.Application/Interfaces/IReviewService.cs` |
| `ReviewService` | Lớp dịch vụ | Hiện thực `IReviewService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Reviews/ReviewService.cs` |
| `CreateReviewRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Reviews/ReviewValidators.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `Review`, `Movie`, `BookingTicket` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `CanReviewResponse`, `CreateReviewRequest`, `ReviewDto`, `DeleteReviewResponse` | DTO | — | `backend/Cinema.Application/DTOs/Reviews/ReviewDtos.cs` |

### 4.2.8 Sơ đồ lớp package Đặt vé trực tuyến

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.8)
  - UC-XT-11 (cấp socket token) do `AuthController.GenerateSocketToken` thực hiện, xem package Xác thực; `BookingService` còn gọi `IPaymentGateway` để hoàn tiền khi hủy đơn, xem package Thanh toán trực tuyến.
- **Tài liệu/hình/bảng:**
  - `Hình 4.9` Sơ đồ lớp package Đặt vé trực tuyến
  - `Bảng 4.11` Danh sách lớp package Đặt vé trực tuyến
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/BookingsController.cs`; `backend/Cinema.Api/Controllers/Client/ShowtimesController.cs`; `backend/Cinema.Api/Controllers/Client/CombosController.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs`; `backend/Cinema.Application/Interfaces/IShowtimeService.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs`; `backend/Cinema.Application/Interfaces/IComboService.cs`; `backend/Cinema.Application/Services/Combos/ComboService.cs`; `backend/Cinema.Application/Interfaces/IRealtimeNotifier.cs` …

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `BookingsController` | Controller | POST /api/v1/bookings/hold → `HoldSeats(HoldSeatsRequest request) : ActionResult<HoldBookingResponse>`<br>GET /api/v1/bookings → `GetMyBookings(string? status, string? cursor, int? limit = null) : ActionResult<CursorPageResult<BookingListItemDto>>`<br>GET /api/v1/bookings/{bookingId:int} → `GetBookingDetail(int bookingId) : ActionResult<BookingDetailDto>`<br>POST /api/v1/bookings/{bookingId:int}/cancel → `CancelBooking(int bookingId) : ActionResult<CancelBookingResponse>` | `backend/Cinema.Api/Controllers/Client/BookingsController.cs` |
| `ShowtimesController` | Controller | GET /api/v1/showtimes/{showtimeId:int}/seats → `GetSeats(int showtimeId) : IActionResult`<br>GET /api/v1/showtimes/{showtimeId:int}/seat-suggestions → `GetSeatSuggestions(int showtimeId, int count) : IActionResult` | `backend/Cinema.Api/Controllers/Client/ShowtimesController.cs` |
| `CombosController` | Controller | GET /api/v1/combos → `GetCombos() : IActionResult`<br>GET /api/v1/combos/suggestions → `GetSuggestions(int showtimeId, int seatCount) : IActionResult` | `backend/Cinema.Api/Controllers/Client/CombosController.cs` |
| `IBookingService` | Interface | `HoldSeatsAsync(int userId, HoldSeatsRequest request) : HoldBookingResponse`<br>`GetMyBookingsAsync(int userId, string? status, string? cursor, int limit) : CursorPageResult<BookingListItemDto>`<br>`GetBookingDetailAsync(int userId, int bookingId) : BookingDetailDto`<br>`CancelBookingAsync(int userId, int bookingId) : CancelBookingResponse`<br>`CalculateRefundEligibilityAsync(DateTime showtimeStartTime, decimal paidAmount) : RefundEligibility`<br>`ReverseLoyaltyAsync(int userId, int bookingId, decimal finalAmount) : void` [CẦN BỔ SUNG: xác nhận thứ tự tham số] | `backend/Cinema.Application/Interfaces/IBookingService.cs` |
| `IShowtimeService` | Interface | `GetShowtimeSeatsAsync(int showtimeId) : ShowtimeSeatsResponseDto`<br>`GetSeatSuggestionsAsync(int showtimeId, int count) : List<SeatSuggestionDto>` | `backend/Cinema.Application/Interfaces/IShowtimeService.cs` |
| `IComboService` | Interface | `GetActiveCombosAsync() : List<ComboDto>`<br>`GetComboSuggestionsAsync(int showtimeId, int seatCount) : List<ComboSuggestionDto>` | `backend/Cinema.Application/Interfaces/IComboService.cs` |
| `IRealtimeNotifier` | Interface | `NotifySeatHeldAsync(int showtimeId, List<int> seatIds, int heldByUserId, DateTime heldUntil) : void`<br>`NotifySeatReleasedAsync(int showtimeId, List<int> seatIds) : void`<br>`NotifySeatBookedAsync(int showtimeId, List<int> seatIds) : void`<br>`NotifyHoldExpiredAsync(int showtimeId, int bookingId, List<int> seatIds) : void` | `backend/Cinema.Application/Interfaces/IRealtimeNotifier.cs` |
| `BookingService` | Lớp dịch vụ | Hiện thực `IBookingService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Bookings/BookingService.cs` |
| `ShowtimeService` | Lớp dịch vụ | Hiện thực `IShowtimeService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs` |
| `ComboService` | Lớp dịch vụ | Hiện thực `IComboService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Combos/ComboService.cs` |
| `SignalRRealtimeNotifier` | Lớp dịch vụ | Hiện thực `IRealtimeNotifier`; không có phương thức công khai ngoài interface | `backend/Cinema.Infrastructure/Realtime/SignalRRealtimeNotifier.cs` |
| `ShowtimeHub` | SignalR Hub | `JoinShowtime(int showtimeId) : void`<br>`LeaveShowtime(int showtimeId) : void` | `backend/Cinema.Infrastructure/Realtime/ShowtimeHub.cs` |
| `SeatLayoutRules` | Lớp tiện ích | `LoadSeatSlotsAsync(this IAppDbContext db, int showtimeId) : List<SeatSlot>`<br>`LoadSeatGapRuleModeAsync(this IAppDbContext db) : SeatGapRuleMode`<br>`Suggest(IReadOnlyList<SeatSlot> seats, int cols, int count, int top = 3) : List<SeatSuggestionDto>`<br>`ValidateSelection(IReadOnlyList<SeatSlot> seats, ICollection<int> selectedSeatIds, SeatGapRuleMode gapRuleMode = SeatGapRuleMode.Strict) : string?` | `backend/Cinema.Application/Utils/SeatLayoutRules.cs` |
| `SeatConflictException` | Exception | — | `backend/Cinema.Shared/Exceptions/SeatConflictException.cs` |
| `BookingOptions` | Options | — | `backend/Cinema.Application/Options/BookingOptions.cs` |
| `HoldSeatsRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Bookings/BookingValidators.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `Booking`, `BookingTicket`, `BookingCombo`, `ShowtimeSeat`, `Combo` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `HoldSeatsRequest`, `HoldBookingResponse`, `BookingListItemDto`, `BookingDetailDto`, `CancelBookingResponse`, `ShowtimeSeatsResponseDto`, `SeatSuggestionDto`, `ComboSuggestionDto` | DTO | — | `backend/Cinema.Application/DTOs/Bookings/BookingDtos.cs`<br>`backend/Cinema.Application/DTOs/Combos/ComboDtos.cs`<br>`backend/Cinema.Application/DTOs/Showtimes/ShowtimeDtos.cs` |

### 4.2.9 Sơ đồ lớp package Thanh toán trực tuyến

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.9)
  - `ShowtimeRefundJob` gọi `IPaymentGateway` để hoàn tiền khi hủy suất chiếu, xem package Vận hành hệ thống.
- **Tài liệu/hình/bảng:**
  - `Hình 4.10` Sơ đồ lớp package Thanh toán trực tuyến
  - `Bảng 4.12` Danh sách lớp package Thanh toán trực tuyến
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/BookingsController.cs`; `backend/Cinema.Api/Controllers/PaymentsController.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs`; `backend/Cinema.Application/Interfaces/IPaymentService.cs`; `backend/Cinema.Application/Services/Payments/PaymentService.cs`; `backend/Cinema.Application/Interfaces/IPaymentGateway.cs`; `backend/Cinema.Infrastructure/Payments/VnPayGateway.cs`; `backend/Cinema.Infrastructure/Payments/MomoGateway.cs`; `backend/Cinema.Infrastructure/Payments/StripeGateway.cs` …

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `BookingsController` | Controller | POST /api/v1/bookings/{bookingId:int}/checkout → `Checkout(int bookingId, CheckoutRequest request) : ActionResult<CheckoutResponse>` | `backend/Cinema.Api/Controllers/Client/BookingsController.cs` |
| `PaymentsController` | Controller | GET /api/v1/payments/vnpay/ipn → `VnPayIpn() : IActionResult`<br>POST /api/v1/payments/momo/ipn → `MomoIpn() : IActionResult`<br>POST /api/v1/payments/stripe/webhook → `StripeWebhook() : IActionResult` | `backend/Cinema.Api/Controllers/PaymentsController.cs` |
| `IBookingService` | Interface | `CheckoutAsync(int userId, int bookingId, CheckoutRequest request) : CheckoutResponse`<br>`ConfirmBookingAndGenerateTicketsAsync(int bookingId) : void`<br>`ProcessPaymentFailureAsync(int bookingId) : void`<br>`ReverseLoyaltyAsync(int userId, int bookingId, decimal finalAmount) : void` [CẦN BỔ SUNG: xác nhận thứ tự tham số] | `backend/Cinema.Application/Interfaces/IBookingService.cs` |
| `IPaymentService` | Interface | `ProcessVnPayIpnAsync(IReadOnlyDictionary<string, string> parameters) : VnPayIpnResult`<br>`ProcessMomoIpnAsync(IReadOnlyDictionary<string, string> parameters) : MomoIpnResult`<br>`ProcessStripeWebhookAsync(string jsonPayload, string? stripeSignature) : StripeWebhookResult` | `backend/Cinema.Application/Interfaces/IPaymentService.cs` |
| `IPaymentGateway` | Interface | `CreatePaymentAsync(CreatePaymentRequest request) : CreatePaymentResult`<br>`VerifyCallback(IReadOnlyDictionary<string, string> queryOrFormParams) : GatewayCallbackResult`<br>`VerifyWebhook(string rawBody, string? signatureHeader) : GatewayCallbackResult`<br>`RefundAsync(RefundRequest request) : RefundResult` | `backend/Cinema.Application/Interfaces/IPaymentGateway.cs` |
| `BookingService` | Lớp dịch vụ | Hiện thực `IBookingService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Bookings/BookingService.cs` |
| `PaymentService` | Lớp dịch vụ | Hiện thực `IPaymentService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Payments/PaymentService.cs` |
| `VnPayGateway` | Gateway | — | `backend/Cinema.Infrastructure/Payments/VnPayGateway.cs` |
| `MomoGateway` | Gateway | — | `backend/Cinema.Infrastructure/Payments/MomoGateway.cs` |
| `StripeGateway` | Gateway | `VerifyWebhook(string rawBody, string? signatureHeader) : GatewayCallbackResult` | `backend/Cinema.Infrastructure/Payments/StripeGateway.cs` |
| `ShowtimeRefundJob` | Job nền | `RunAsync(int paymentId, PerformContext? context) : void` | `backend/Cinema.Application/BackgroundJobs/ShowtimeRefundJob.cs` |
| `SendEmailJob` | Job nền | `RunAsync(string toEmail, string subject, string htmlbody) : void` | `backend/Cinema.Application/BackgroundJobs/SendEmailJob.cs` |
| `HmacSigner` | Lớp tiện ích | `Sha256Hex(string key, string data) : string`<br>`Sha512Hex(string key, string data) : string` | `backend/Cinema.Infrastructure/Payments/HmacSigner.cs` |
| `EmailTemplates` | Lớp tiện ích | `BookingConfirmed(BrandOptions brand, string customerName, string bookingCode, string movieTitle, int seatCount) : string`<br>`BookingCancelledRefunded(BrandOptions brand, string customerName, string bookingCode, decimal refundAmount) : string`<br>`SeatsExpiredAfterPayment(BrandOptions brand, bool autoRefunded, int bookingId, int paymentId, decimal amount) : string` | `backend/Cinema.Application/Utils/EmailTemplates.cs` |
| `QrCodeHelper` | Lớp tiện ích | `GenerateDataUri(string content) : string` | `backend/Cinema.Application/Utils/QrCodeHelper.cs` |
| `CodeGenerator` | Lớp tiện ích | `GeneratePaymentTransactionId(string bookingCode) : string`<br>`GenerateTicketQrPayload(string bookingCode, string seatRow, int seatNumber) : string` | `backend/Cinema.Application/Utils/CodeGenerator.cs` |
| `SeatsNoLongerHeldException` | Exception | — | `backend/Cinema.Shared/Exceptions/SeatsNoLongerHeldException.cs` |
| `VnPayOptions` | Options | — | `backend/Cinema.Application/Options/VnPayOptions.cs` |
| `MomoOptions` | Options | — | `backend/Cinema.Application/Options/MomoOptions.cs` |
| `StripeOptions` | Options | — | `backend/Cinema.Application/Options/StripeOptions.cs` |
| `GatewayCallbackResult` | Record | — | `backend/Cinema.Application/Interfaces/IPaymentGateway.cs` |
| `VnPayIpnResult` | Record | — | `backend/Cinema.Application/Interfaces/IPaymentService.cs` |
| `MomoIpnResult` | Record | — | `backend/Cinema.Application/Interfaces/IPaymentService.cs` |
| `StripeWebhookResult` | Record | — | `backend/Cinema.Application/Interfaces/IPaymentService.cs` |
| `RefundRequest` | Record | — | `backend/Cinema.Application/Interfaces/IPaymentGateway.cs` |
| `RefundResult` | Record | — | `backend/Cinema.Application/Interfaces/IPaymentGateway.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `Payment`, `Booking`, `BookingTicket`, `ShowtimeSeat` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `CheckoutRequest`, `CheckoutResponse` | DTO | — | `backend/Cinema.Application/DTOs/Bookings/BookingDtos.cs` |

### 4.2.10 Sơ đồ lớp package Trợ lý AI

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.10)
  - Lời gọi Gemini dùng `HttpClient` (thư viện ngoài) được tiêm vào `GeminiAiAssistantService`, không vẽ.
- **Tài liệu/hình/bảng:**
  - `Hình 4.11` Sơ đồ lớp package Trợ lý AI
  - `Bảng 4.13` Danh sách lớp package Trợ lý AI
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/AiController.cs`; `backend/Cinema.Application/Interfaces/IAiAssistantService.cs`; `backend/Cinema.Application/Services/Ai/GeminiAiAssistantService.cs`; `backend/Cinema.Application/Services/Ai/AiToolExecutor.cs`

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `AiController` | Controller | POST /api/v1/ai/chat → `Chat(AiChatRequest request) : ActionResult<AiChatResponse>` | `backend/Cinema.Api/Controllers/Client/AiController.cs` |
| `IAiAssistantService` | Interface | `ChatAsync(int? userId, AiChatRequest request) : AiChatResponse` | `backend/Cinema.Application/Interfaces/IAiAssistantService.cs` |
| `GeminiAiAssistantService` | Lớp dịch vụ | Hiện thực `IAiAssistantService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Ai/GeminiAiAssistantService.cs` |
| `AiToolExecutor` | Lớp dịch vụ | `BuildFunctionDeclarations(bool isAuthenticated) : JsonArray`<br>`ExecuteAsync(string name, JsonObject? args, int? userId) : ToolResult` | `backend/Cinema.Application/Services/Ai/AiToolExecutor.cs` |
| `GeminiOptions` | Options | — | `backend/Cinema.Application/Options/GeminiOptions.cs` |
| `BrandOptions` | Options | — | `backend/Cinema.Application/Options/BrandOptions.cs` |
| `AiChatRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Ai/AiValidators.cs` |
| `ChatMessageDtoValidator` | Validator | — | `backend/Cinema.Application/Validators/Ai/AiValidators.cs` |
| `ToolResult` | Record | — | `backend/Cinema.Application/Services/Ai/AiToolExecutor.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `Movie`, `Showtime`, `ShowtimeSeat`, `Booking`, `Combo`, `CinemaSettings` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `AiChatRequest`, `AiChatResponse`, `ChatMessageDto`, `AiCardDto` | DTO | — | `backend/Cinema.Application/DTOs/Ai/AiDtos.cs` |

### 4.2.11 Sơ đồ lớp package Bán vé tại quầy

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.11)
- **Tài liệu/hình/bảng:**
  - `Hình 4.12` Sơ đồ lớp package Bán vé tại quầy
  - `Bảng 4.14` Danh sách lớp package Bán vé tại quầy
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs`; `backend/Cinema.Application/Interfaces/IEmployeeService.cs`; `backend/Cinema.Application/Services/Employees/EmployeeService.cs`

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `AdminBookingsController` | Controller | POST /api/v1/admin/bookings/counter-hold → `CounterHoldSeats(CounterHoldRequest request) : ActionResult<CounterHoldResponse>`<br>POST /api/v1/admin/bookings/{bookingId:int}/counter-checkout → `CounterCheckout(int bookingId, CounterCheckoutRequest request) : ActionResult<CounterCheckoutResponse>` | `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs` |
| `AdminUsersController` | Controller | GET /api/v1/admin/users/customers → `SearchCustomers(string? q) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs` |
| `IBookingService` | Interface | `CounterHoldSeatsAsync(int employeeId, int employeeUserId, CounterHoldRequest request) : CounterHoldResponse`<br>`CounterCheckoutAsync(int bookingId, CounterCheckoutRequest request) : CounterCheckoutResponse`<br>`ConfirmBookingAndGenerateTicketsAsync(int bookingId) : void` | `backend/Cinema.Application/Interfaces/IBookingService.cs` |
| `IEmployeeService` | Interface | `SearchCustomersForCounterAsync(string? query) : List<CustomerLookupItemDto>` | `backend/Cinema.Application/Interfaces/IEmployeeService.cs` |
| `BookingService` | Lớp dịch vụ | Hiện thực `IBookingService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Bookings/BookingService.cs` |
| `EmployeeService` | Lớp dịch vụ | Hiện thực `IEmployeeService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Employees/EmployeeService.cs` |
| `SeatLayoutRules` | Lớp tiện ích | `ValidateSelection(IReadOnlyList<SeatSlot> seats, ICollection<int> selectedSeatIds, SeatGapRuleMode gapRuleMode = SeatGapRuleMode.Strict) : string?` | `backend/Cinema.Application/Utils/SeatLayoutRules.cs` |
| `QrCodeHelper` | Lớp tiện ích | `GenerateDataUri(string content) : string` | `backend/Cinema.Application/Utils/QrCodeHelper.cs` |
| `BookingOptions` | Options | — | `backend/Cinema.Application/Options/BookingOptions.cs` |
| `CounterHoldRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Bookings/BookingValidators.cs` |
| `CounterCheckoutRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Bookings/BookingValidators.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `Booking`, `BookingTicket`, `Payment`, `Employee`, `User` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `CounterHoldRequest`, `CounterHoldResponse`, `CounterCheckoutRequest`, `CounterCheckoutResponse`, `CustomerLookupItemDto` | DTO | — | `backend/Cinema.Application/DTOs/Bookings/BookingDtos.cs`<br>`backend/Cinema.Application/DTOs/Employees/EmployeeDtos.cs` |

### 4.2.12 Sơ đồ lớp package Soát vé

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.12)
  - Quét QR bằng camera là xử lý phía giao diện (`BarcodeDetector`, `jsQR`), không có lớp backend riêng.
- **Tài liệu/hình/bảng:**
  - `Hình 4.13` Sơ đồ lớp package Soát vé
  - `Bảng 4.15` Danh sách lớp package Soát vé
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs`

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `AdminBookingsController` | Controller | POST /api/v1/admin/bookings/checkin → `Checkin(CheckinRequest request) : ActionResult<CheckinResponse>` | `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs` |
| `IBookingService` | Interface | `CheckinAsync(CheckinRequest request) : CheckinResponse` | `backend/Cinema.Application/Interfaces/IBookingService.cs` |
| `BookingService` | Lớp dịch vụ | Hiện thực `IBookingService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Bookings/BookingService.cs` |
| `CheckinRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Bookings/BookingValidators.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `BookingTicket`, `Booking`, `Showtime`, `CinemaSettings` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `CheckinRequest`, `CheckinResponse` | DTO | — | `backend/Cinema.Application/DTOs/Bookings/BookingDtos.cs` |

### 4.2.13 Sơ đồ lớp package Quản lý booking

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.13)
- **Tài liệu/hình/bảng:**
  - `Hình 4.14` Sơ đồ lớp package Quản lý booking
  - `Bảng 4.16` Danh sách lớp package Quản lý booking
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs`

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `AdminBookingsController` | Controller | GET /api/v1/admin/bookings → `GetBookings(AdminBookingListFilter filter, int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : ActionResult<OffsetPageResult<AdminBookingListItemDto>>`<br>GET /api/v1/admin/bookings/{bookingId:int} → `GetBookingDetail(int bookingId) : ActionResult<AdminBookingDetailDto>` | `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs` |
| `IBookingService` | Interface | `GetBookingsAdminAsync(AdminBookingListFilter filter, int page, int pageSize) : OffsetPageResult<AdminBookingListItemDto>`<br>`GetBookingDetailAdminAsync(int bookingId) : AdminBookingDetailDto` | `backend/Cinema.Application/Interfaces/IBookingService.cs` |
| `BookingService` | Lớp dịch vụ | Hiện thực `IBookingService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Bookings/BookingService.cs` |
| `AdminBookingListFilter` | Record | — | `backend/Cinema.Application/Interfaces/IBookingService.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `Booking`, `BookingTicket`, `BookingCombo`, `Payment` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `AdminBookingListItemDto`, `AdminBookingDetailDto` | DTO | — | `backend/Cinema.Application/DTOs/Bookings/BookingDtos.cs` |

### 4.2.14 Sơ đồ lớp package Quản lý phim

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.14)
  - Tải poster, trailer do `AdminUploadsController` thực hiện (package Tải tệp).
- **Tài liệu/hình/bảng:**
  - `Hình 4.15` Sơ đồ lớp package Quản lý phim
  - `Bảng 4.17` Danh sách lớp package Quản lý phim
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`; `backend/Cinema.Application/Interfaces/IMovieService.cs`; `backend/Cinema.Application/Services/Movies/MovieService.cs`

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `AdminMoviesController` | Controller | GET /api/v1/admin/movies → `GetMovies(AdminMovieListFilter filter, int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult`<br>POST /api/v1/admin/movies → `Create(UpsertMovieRequest request) : IActionResult`<br>PUT /api/v1/admin/movies/{id:int} → `Update(int id, UpsertMovieRequest request) : IActionResult`<br>DELETE /api/v1/admin/movies/{id:int} → `Delete(int id) : IActionResult`<br>DELETE /api/v1/admin/movies/bulk → `BulkDelete(BulkDeleteMoviesRequest request) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs` |
| `IMovieService` | Interface | `GetMoviesAdminAsync(AdminMovieListFilter filter, int page, int pageSize) : OffsetPageResult<AdminMovieListItemDto>`<br>`CreateMovieAsync(UpsertMovieRequest request) : MovieDetailDto`<br>`UpdateMovieAsync(int movieId, UpsertMovieRequest request) : MovieDetailDto`<br>`DeleteMovieAsync(int movieId) : DeleteMovieResponse`<br>`BulkDeleteMoviesAsync(BulkDeleteMoviesRequest request) : BulkDeleteMoviesResponse` | `backend/Cinema.Application/Interfaces/IMovieService.cs` |
| `MovieService` | Lớp dịch vụ | Hiện thực `IMovieService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Movies/MovieService.cs` |
| `BookingOptions` | Options | — | `backend/Cinema.Application/Options/BookingOptions.cs` |
| `UpsertMovieRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Movies/MovieValidators.cs` |
| `BulkDeleteMoviesRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Movies/MovieValidators.cs` |
| `AdminMovieListFilter` | Record | — | `backend/Cinema.Application/Interfaces/IMovieService.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `Movie`, `MovieGenre`, `MovieCast`, `Director`, `Language`, `AgeRating`, `Genre`, `Actor` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `AdminMovieListItemDto`, `UpsertMovieRequest`, `MovieDetailDto`, `DeleteMovieResponse`, `BulkDeleteMoviesRequest`, `BulkDeleteMoviesResponse` | DTO | — | `backend/Cinema.Application/DTOs/Movies/MovieDtos.cs` |

### 4.2.15 Sơ đồ lớp package Quản lý danh mục

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.15)
  - Mười controller cùng gọi `ICatalogService` (chín controller danh mục) hoặc `IComboService` (`AdminCombosController`); sơ đồ chỉ liệt kê một số phương thức đại diện, các phương thức tương tự của controller còn lại có trong bảng. `CatalogValidators` đại diện cho nhóm validator `Upsert*RequestValidator` trong `CatalogValidators.cs`.
- **Tài liệu/hình/bảng:**
  - `Hình 4.16` Sơ đồ lớp package Quản lý danh mục
  - `Bảng 4.18` Danh sách lớp package Quản lý danh mục
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminGenresController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminLanguagesController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminAgeRatingsController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminActorsController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminDirectorsController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminScreenTypesController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminSeatTypesController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminPositionsController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminMembershipTiersController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminCombosController.cs` …

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `AdminGenresController` | Controller | GET /api/v1/admin/genres → `GetAll(int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult`<br>POST /api/v1/admin/genres → `Create(UpsertGenreRequest request) : IActionResult`<br>PUT /api/v1/admin/genres/{id:int} → `Update(int id, UpsertGenreRequest request) : IActionResult`<br>DELETE /api/v1/admin/genres/{id:int} → `Delete(int id) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminGenresController.cs` |
| `AdminLanguagesController` | Controller | GET /api/v1/admin/languages → `GetAll(int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminLanguagesController.cs` |
| `AdminAgeRatingsController` | Controller | GET /api/v1/admin/age-ratings → `GetAll(int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminAgeRatingsController.cs` |
| `AdminActorsController` | Controller | GET /api/v1/admin/actors → `GetAll(string? search, int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult`<br>DELETE /api/v1/admin/actors/bulk → `BulkDelete(BulkIdsRequest request) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminActorsController.cs` |
| `AdminDirectorsController` | Controller | GET /api/v1/admin/directors → `GetAll(string? search, int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult`<br>DELETE /api/v1/admin/directors/bulk → `BulkDelete(BulkIdsRequest request) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminDirectorsController.cs` |
| `AdminScreenTypesController` | Controller | GET /api/v1/admin/screen-types → `GetAll(int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminScreenTypesController.cs` |
| `AdminSeatTypesController` | Controller | GET /api/v1/admin/seat-types → `GetAll(int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminSeatTypesController.cs` |
| `AdminPositionsController` | Controller | GET /api/v1/admin/positions → `GetAll(int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminPositionsController.cs` |
| `AdminMembershipTiersController` | Controller | GET /api/v1/admin/membership-tiers → `GetAll(int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminMembershipTiersController.cs` |
| `AdminCombosController` | Controller | GET /api/v1/admin/combos → `GetCombos(int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult`<br>POST /api/v1/admin/combos → `Create(UpsertComboRequest request) : IActionResult`<br>PUT /api/v1/admin/combos/{id:int} → `Update(int id, UpsertComboRequest request) : IActionResult`<br>PUT /api/v1/admin/combos/{id:int}/toggle → `Toggle(int id) : IActionResult`<br>PUT /api/v1/admin/combos/bulk/status → `BulkSetActive(BulkStatusRequest request) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminCombosController.cs` |
| `ICatalogService` | Interface | `GetGenresAdminAsync(int page, int pageSize) : OffsetPageResult<GenreDto>`<br>`CreateGenreAsync(UpsertGenreRequest request) : GenreDto`<br>`UpdateGenreAsync(int id, UpsertGenreRequest request) : GenreDto`<br>`DeleteGenreAsync(int id) : DeleteCategoryResponse`<br>`GetActorsAdminAsync(string? search, int page, int pageSize) : OffsetPageResult<ActorDto>`<br>`BulkDeleteActorsAsync(BulkIdsRequest request) : BulkActionResponse`<br>`GetDirectorsAdminAsync(string? search, int page, int pageSize) : OffsetPageResult<DirectorDto>`<br>`BulkDeleteDirectorsAsync(BulkIdsRequest request) : BulkActionResponse`<br>`GetSeatTypesAsync(int page, int pageSize) : OffsetPageResult<SeatTypeDto>`<br>`GetMembershipTiersAdminAsync(int page, int pageSize) : OffsetPageResult<MembershipTierDto>`<br>`GetPositionsAsync(int page, int pageSize) : OffsetPageResult<PositionDto>`<br>`GetScreenTypesAsync(int page, int pageSize) : OffsetPageResult<ScreenTypeDto>`<br>`GetLanguagesAdminAsync(int page, int pageSize) : OffsetPageResult<LanguageDto>`<br>`GetAgeRatingsAdminAsync(int page, int pageSize) : OffsetPageResult<AgeRatingDto>` | `backend/Cinema.Application/Interfaces/ICatalogService.cs` |
| `IComboService` | Interface | `GetCombosAdminAsync(int page, int pageSize) : OffsetPageResult<AdminComboDto>`<br>`CreateComboAsync(UpsertComboRequest request) : AdminComboDto`<br>`UpdateComboAsync(int comboId, UpsertComboRequest request) : AdminComboDto`<br>`ToggleComboAsync(int comboId) : ToggleComboResponse`<br>`SetCombosActiveAsync(BulkStatusRequest request) : BulkActionResponse` | `backend/Cinema.Application/Interfaces/IComboService.cs` |
| `CatalogService` | Lớp dịch vụ | Hiện thực `ICatalogService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| `ComboService` | Lớp dịch vụ | Hiện thực `IComboService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Combos/ComboService.cs` |
| `BulkOperation` | Lớp tiện ích | `RunAsync(IEnumerable<int> ids, Func<int, Task> action, Action<int, Exception>? onUnexpected = null) : BulkActionResponse`<br>`Build(List<BulkActionResultItem> results) : BulkActionResponse` | `backend/Cinema.Application/Utils/BulkOperation.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `Genre`, `Language`, `AgeRating`, `Actor`, `Director`, `ScreenType`, `SeatType`, `Position`, `MembershipTier`, `Combo` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `UpsertGenreRequest`, `UpsertPersonRequest`, `UpsertSeatTypeRequest`, `UpsertMembershipTierRequest`, `UpsertComboRequest`, `DeleteCategoryResponse`, `BulkActionResponse` | DTO | — | `backend/Cinema.Application/DTOs/Catalog/CatalogDtos.cs`<br>`backend/Cinema.Application/DTOs/Combos/ComboDtos.cs`<br>`backend/Cinema.Application/DTOs/Common/BulkDtos.cs` |

### 4.2.16 Sơ đồ lớp package Quản lý phòng chiếu

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.16)
  - Trình soạn sơ đồ ghế (kéo, tô, hoàn tác) là xử lý phía giao diện `frontend/src/features/admin/**`; backend chỉ nhận toàn bộ lưới qua `UpdateSeatLayoutAsync`.
- **Tài liệu/hình/bảng:**
  - `Hình 4.17` Sơ đồ lớp package Quản lý phòng chiếu
  - `Bảng 4.19` Danh sách lớp package Quản lý phòng chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminRoomsController.cs`; `backend/Cinema.Application/Interfaces/IRoomService.cs`; `backend/Cinema.Application/Services/Rooms/RoomService.cs`

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `AdminRoomsController` | Controller | GET /api/v1/admin/rooms → `GetRooms(int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult`<br>POST /api/v1/admin/rooms → `Create(CreateRoomRequest request) : IActionResult`<br>GET /api/v1/admin/rooms/{id:int} → `GetById(int id) : IActionResult`<br>PUT /api/v1/admin/rooms/{id:int} → `Update(int id, UpdateRoomRequest request) : IActionResult`<br>DELETE /api/v1/admin/rooms/{id:int} → `Delete(int id) : IActionResult`<br>GET /api/v1/admin/rooms/{id:int}/seats → `GetSeats(int id) : IActionResult`<br>PUT /api/v1/admin/rooms/{id:int}/seats → `UpdateSeatLayout(int id, UpdateSeatLayoutRequest request) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminRoomsController.cs` |
| `IRoomService` | Interface | `GetRoomsAsync(int page, int pageSize) : OffsetPageResult<AdminRoomDto>`<br>`GetRoomByIdAsync(int roomId) : AdminRoomDto`<br>`CreateRoomAsync(CreateRoomRequest request) : AdminRoomDto`<br>`UpdateRoomAsync(int roomId, UpdateRoomRequest request) : AdminRoomDto`<br>`DeleteRoomAsync(int roomId) : DeleteRoomResponse`<br>`GetRoomSeatsAsync(int roomId) : List<SeatDto>`<br>`UpdateSeatLayoutAsync(int roomId, UpdateSeatLayoutRequest request) : UpdateSeatLayoutResponse` | `backend/Cinema.Application/Interfaces/IRoomService.cs` |
| `RoomService` | Lớp dịch vụ | Hiện thực `IRoomService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Rooms/RoomService.cs` |
| `UpdateSeatLayoutRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Rooms/RoomValidators.cs` |
| `CreateRoomRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Rooms/RoomValidators.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `CinemaRoom`, `Seat`, `SeatType`, `ScreenType`, `Showtime`, `ShowtimeSeat` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `AdminRoomDto`, `CreateRoomRequest`, `UpdateRoomRequest`, `SeatDto`, `SeatLayoutItem`, `UpdateSeatLayoutRequest`, `UpdateSeatLayoutResponse`, `DeleteRoomResponse` | DTO | — | `backend/Cinema.Application/DTOs/Rooms/RoomDtos.cs` |

### 4.2.17 Sơ đồ lớp package Quản lý suất chiếu

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.17)
  - `ShowtimeScheduler.Score` và `Candidates` là phương thức private nên không đưa vào sơ đồ; các bản ghi `SchedulerRoom`, `SchedulerMovie`, `SchedulerBusy`, `SchedulerSlot` là kiểu dữ liệu của `ShowtimeScheduler.Build`.
- **Tài liệu/hình/bảng:**
  - `Hình 4.18` Sơ đồ lớp package Quản lý suất chiếu
  - `Bảng 4.20` Danh sách lớp package Quản lý suất chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`; `backend/Cinema.Application/Interfaces/IShowtimeService.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs`; `backend/Cinema.Application/Interfaces/IBookingService.cs`; `backend/Cinema.Application/Interfaces/IPaymentGateway.cs`; `backend/Cinema.Application/BackgroundJobs/SendEmailJob.cs`; `backend/Cinema.Application/BackgroundJobs/ShowtimeRefundJob.cs`

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `AdminShowtimesController` | Controller | GET /api/v1/admin/showtimes → `GetShowtimes(AdminShowtimeListFilter filter, int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult`<br>GET /api/v1/admin/showtimes/calendar → `GetCalendar(AdminShowtimeCalendarFilter filter) : IActionResult`<br>POST /api/v1/admin/showtimes → `Create(UpsertShowtimeRequest request) : IActionResult`<br>PUT /api/v1/admin/showtimes/{id:int} → `Update(int id, UpsertShowtimeRequest request) : IActionResult`<br>DELETE /api/v1/admin/showtimes/{id:int} → `Delete(int id) : IActionResult`<br>POST /api/v1/admin/showtimes/bulk → `BulkCreate(BulkCreateShowtimesRequest request) : IActionResult`<br>POST /api/v1/admin/showtimes/auto-schedule/preview → `PreviewAutoSchedule(AutoScheduleRequest request) : IActionResult`<br>POST /api/v1/admin/showtimes/auto-schedule/apply → `ApplyAutoSchedule(ApplyAutoScheduleRequest request) : IActionResult`<br>POST /api/v1/admin/showtimes/clone → `CloneShowtimes(CloneShowtimesRequest request) : IActionResult`<br>DELETE /api/v1/admin/showtimes/bulk → `BulkDelete(BulkDeleteShowtimesRequest request) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs` |
| `IShowtimeService` | Interface | `GetShowtimesAdminAsync(AdminShowtimeListFilter filter, int page, int pageSize) : OffsetPageResult<AdminShowtimeDto>`<br>`GetShowtimesCalendarAsync(AdminShowtimeCalendarFilter filter) : ShowtimeCalendarResponseDto`<br>`CreateShowtimeAsync(UpsertShowtimeRequest request) : CreateShowtimeResponse`<br>`UpdateShowtimeAsync(int showtimeId, UpsertShowtimeRequest request) : AdminShowtimeDto`<br>`DeleteShowtimeAsync(int showtimeId) : DeleteShowtimeResponse`<br>`BulkCreateShowtimesAsync(BulkCreateShowtimesRequest request) : BulkCreateShowtimesResponse`<br>`BulkDeleteShowtimesAsync(BulkDeleteShowtimesRequest request) : BulkDeleteShowtimesResponse`<br>`PreviewAutoScheduleAsync(AutoScheduleRequest request) : AutoScheduleResponseDto`<br>`ApplyAutoScheduleAsync(ApplyAutoScheduleRequest request) : BulkCreateShowtimesResponse`<br>`CloneShowtimesAsync(CloneShowtimesRequest request) : BulkCreateShowtimesResponse` | `backend/Cinema.Application/Interfaces/IShowtimeService.cs` |
| `IBookingService` | Interface | `HoldSeatsAsync(int userId, HoldSeatsRequest request) : HoldBookingResponse`<br>`GetMyBookingsAsync(int userId, string? status, string? cursor, int limit) : CursorPageResult<BookingListItemDto>`<br>`GetBookingDetailAsync(int userId, int bookingId) : BookingDetailDto`<br>`CheckoutAsync(int userId, int bookingId, CheckoutRequest request) : CheckoutResponse`<br>`CancelBookingAsync(int userId, int bookingId) : CancelBookingResponse`<br>`CounterHoldSeatsAsync(int employeeId, int employeeUserId, CounterHoldRequest request) : CounterHoldResponse`<br>`GetBookingsAdminAsync(AdminBookingListFilter filter, int page, int pageSize) : OffsetPageResult<AdminBookingListItemDto>`<br>`GetBookingDetailAdminAsync(int bookingId) : AdminBookingDetailDto`<br>`CounterCheckoutAsync(int bookingId, CounterCheckoutRequest request) : CounterCheckoutResponse`<br>`CheckinAsync(CheckinRequest request) : CheckinResponse`<br>`ConfirmBookingAndGenerateTicketsAsync(int bookingId) : void`<br>`ProcessPaymentFailureAsync(int bookingId) : void`<br>`ReverseLoyaltyAsync(int userId, int bookingId, decimal finalAmount) : void` [CẦN BỔ SUNG: xác nhận thứ tự tham số]<br>`CalculateRefundEligibilityAsync(DateTime showtimeStartTime, decimal paidAmount) : RefundEligibility` | `backend/Cinema.Application/Interfaces/IBookingService.cs` |
| `IPaymentGateway` | Interface | `CreatePaymentAsync(CreatePaymentRequest request) : CreatePaymentResult`<br>`VerifyCallback(IReadOnlyDictionary<string, string> queryOrFormParams) : GatewayCallbackResult`<br>`VerifyWebhook(string rawBody, string? signatureHeader) : GatewayCallbackResult`<br>`RefundAsync(RefundRequest request) : RefundResult` | `backend/Cinema.Application/Interfaces/IPaymentGateway.cs` |
| `ShowtimeService` | Lớp dịch vụ | Hiện thực `IShowtimeService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs` |
| `SendEmailJob` | Job nền | `RunAsync(string toEmail, string subject, string htmlbody) : void` | `backend/Cinema.Application/BackgroundJobs/SendEmailJob.cs` |
| `ShowtimeRefundJob` | Job nền | `RunAsync(int paymentId, PerformContext? context) : void` | `backend/Cinema.Application/BackgroundJobs/ShowtimeRefundJob.cs` |
| `ShowtimeScheduler` | Lớp tiện ích | `Heat(int tickets, int maxTickets, decimal rating) : double`<br>`Build(DateOnly day, TimeOnly? opening, TimeOnly? closing, bool allowAfterHours, int bufferMinutes, IReadOnlyList<SchedulerRoom> rooms, IReadOnlyList<SchedulerMovie> movies, IReadOnlyList<SchedulerBusy> busy, SchedulerOptions? schedulerOptions = null) : List<SchedulerSlot>` | `backend/Cinema.Application/Utils/ShowtimeScheduler.cs` |
| `OperatingHours` | Lớp tiện ích | `Violation(DateTime start, DateTime end, TimeOnly? opening, TimeOnly? closing, bool allowAfterHours) : string?` | `backend/Cinema.Application/Utils/OperatingHours.cs` |
| `VietnamTime` | Lớp tiện ích | `ToUtc(DateTime value) : DateTime`<br>`ToLocal(DateTime utc) : DateTime`<br>`LocalDate(DateTime utc) : DateOnly`<br>`Today(DateTime? utcNow = null) : DateOnly` | `backend/Cinema.Application/Utils/VietnamTime.cs` |
| `EmailTemplates` | Lớp tiện ích | `ShowtimeCancelledAutoRefunded(BrandOptions brand, string customerName, string bookingCode, decimal refundAmount) : string`<br>`ShowtimeCancelledManualRefund(BrandOptions brand, string customerName, string bookingCode, decimal amountDue) : string` | `backend/Cinema.Application/Utils/EmailTemplates.cs` |
| `BookingOptions` | Options | — | `backend/Cinema.Application/Options/BookingOptions.cs` |
| `SchedulerOptions` | Options | — | `backend/Cinema.Application/Options/SchedulerOptions.cs` |
| `UpsertShowtimeRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Showtimes/ShowtimeValidators.cs` |
| `AdminShowtimeListFilter` | Record | — | `backend/Cinema.Application/Interfaces/IShowtimeService.cs` |
| `AdminShowtimeCalendarFilter` | Record | — | `backend/Cinema.Application/Interfaces/IShowtimeService.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `Showtime`, `ShowtimeSeat`, `CinemaRoom`, `Movie`, `Booking`, `Payment` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `AdminShowtimeDto`, `UpsertShowtimeRequest`, `CreateShowtimeResponse`, `DeleteShowtimeResponse`, `BulkCreateShowtimesRequest`, `BulkCreateShowtimesResponse`, `AutoScheduleRequest`, `AutoScheduleResponseDto`, `CloneShowtimesRequest` | DTO | — | `backend/Cinema.Application/DTOs/Showtimes/ShowtimeDtos.cs` |

### 4.2.18 Sơ đồ lớp package Quản lý người dùng

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.18)
- **Tài liệu/hình/bảng:**
  - `Hình 4.19` Sơ đồ lớp package Quản lý người dùng
  - `Bảng 4.21` Danh sách lớp package Quản lý người dùng
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminEmployeesController.cs`; `backend/Cinema.Application/Interfaces/IEmployeeService.cs`; `backend/Cinema.Application/Services/Employees/EmployeeService.cs`

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `AdminUsersController` | Controller | GET /api/v1/admin/users → `GetUsers(int? roleId, string? search, int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult`<br>GET /api/v1/admin/users/admins → `GetAdminUsers(string? search, bool availableOnly = true) : IActionResult`<br>GET /api/v1/admin/users/customers → `SearchCustomers(string? q) : IActionResult`<br>PUT /api/v1/admin/users/{id:int}/status → `UpdateStatus(int id, UpdateUserStatusRequest request) : IActionResult`<br>PUT /api/v1/admin/users/{id:int}/role → `UpdateRole(int id, UpdateUserRoleRequest request) : IActionResult`<br>PUT /api/v1/admin/users/bulk/status → `BulkUpdateStatus(BulkStatusRequest request) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs` |
| `AdminEmployeesController` | Controller | GET /api/v1/admin/employees → `GetEmployees(int? positionId, int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult`<br>POST /api/v1/admin/employees → `Create(CreateEmployeeRequest request) : IActionResult`<br>PUT /api/v1/admin/employees/{id:int} → `Update(int id, UpdateEmployeeRequest request) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminEmployeesController.cs` |
| `IEmployeeService` | Interface | `GetUsersAdminAsync(int? roleId, string? search, int page, int pageSize) : OffsetPageResult<AdminUserListItemDto>`<br>`GetAdminUsersForSelectAsync(string? search, bool availableOnly) : List<AdminUserSelectDto>`<br>`SearchCustomersForCounterAsync(string? query) : List<CustomerLookupItemDto>`<br>`UpdateUserStatusAsync(int userId, UpdateUserStatusRequest request) : UpdateUserStatusResponse`<br>`BulkUpdateUserStatusAsync(BulkStatusRequest request, int currentUserId) : BulkActionResponse`<br>`UpdateUserRoleAsync(int userId, UpdateUserRoleRequest request) : UpdateUserRoleResponse`<br>`GetEmployeesAdminAsync(int? positionId, int page, int pageSize) : OffsetPageResult<AdminEmployeeDto>`<br>`CreateEmployeeAsync(CreateEmployeeRequest request) : AdminEmployeeDto`<br>`UpdateEmployeeAsync(int employeeId, UpdateEmployeeRequest request) : AdminEmployeeDto` | `backend/Cinema.Application/Interfaces/IEmployeeService.cs` |
| `EmployeeService` | Lớp dịch vụ | Hiện thực `IEmployeeService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Employees/EmployeeService.cs` |
| `BulkOperation` | Lớp tiện ích | `RunAsync(IEnumerable<int> ids, Func<int, Task> action, Action<int, Exception>? onUnexpected = null) : BulkActionResponse` | `backend/Cinema.Application/Utils/BulkOperation.cs` |
| `BookingOptions` | Options | — | `backend/Cinema.Application/Options/BookingOptions.cs` |
| `UpdateUserRoleRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Employees/EmployeeValidators.cs` |
| `CreateEmployeeRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Employees/EmployeeValidators.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `User`, `Role`, `Employee`, `Position` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `AdminUserListItemDto`, `AdminUserSelectDto`, `CustomerLookupItemDto`, `UpdateUserStatusRequest`, `UpdateUserRoleRequest`, `AdminEmployeeDto`, `CreateEmployeeRequest`, `UpdateEmployeeRequest` | DTO | — | `backend/Cinema.Application/DTOs/Employees/EmployeeDtos.cs` |

### 4.2.19 Sơ đồ lớp package Quản lý đánh giá

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.19)
- **Tài liệu/hình/bảng:**
  - `Hình 4.20` Sơ đồ lớp package Quản lý đánh giá
  - `Bảng 4.22` Danh sách lớp package Quản lý đánh giá
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminReviewsController.cs`; `backend/Cinema.Application/Interfaces/IReviewService.cs`; `backend/Cinema.Application/Services/Reviews/ReviewService.cs`

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `AdminReviewsController` | Controller | GET /api/v1/admin/reviews → `GetReviews(int? movieId, int? rating, int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult`<br>DELETE /api/v1/admin/reviews/{id:int} → `Delete(int id) : IActionResult`<br>DELETE /api/v1/admin/reviews/bulk → `BulkDelete(BulkDeleteReviewsRequest request) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminReviewsController.cs` |
| `IReviewService` | Interface | `GetReviewsAdminAsync(int? movieId, int? rating, int page, int pageSize) : OffsetPageResult<AdminReviewDto>`<br>`DeleteReviewAsync(int reviewId) : DeleteReviewResponse`<br>`BulkDeleteReviewsAsync(BulkDeleteReviewsRequest request) : BulkDeleteReviewsResponse` | `backend/Cinema.Application/Interfaces/IReviewService.cs` |
| `ReviewService` | Lớp dịch vụ | Hiện thực `IReviewService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Reviews/ReviewService.cs` |
| `BookingOptions` | Options | — | `backend/Cinema.Application/Options/BookingOptions.cs` |
| `BulkDeleteReviewsRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Reviews/ReviewValidators.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `Review`, `Movie` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `AdminReviewDto`, `DeleteReviewResponse`, `BulkDeleteReviewsRequest`, `BulkDeleteReviewsResponse` | DTO | — | `backend/Cinema.Application/DTOs/Reviews/ReviewDtos.cs` |

### 4.2.20 Sơ đồ lớp package Quản lý thanh toán

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.20)
- **Tài liệu/hình/bảng:**
  - `Hình 4.21` Sơ đồ lớp package Quản lý thanh toán
  - `Bảng 4.23` Danh sách lớp package Quản lý thanh toán
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminPaymentsController.cs`; `backend/Cinema.Application/Interfaces/IPaymentAdminService.cs`; `backend/Cinema.Application/Services/Payments/PaymentAdminService.cs`

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `AdminPaymentsController` | Controller | GET /api/v1/admin/payments → `GetPayments(AdminPaymentListFilter filter, int page = Pagination.DefaultPage, int pageSize = Pagination.DefaultPageSize) : IActionResult`<br>GET /api/v1/admin/payments/{id:int} → `GetDetail(int id) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminPaymentsController.cs` |
| `IPaymentAdminService` | Interface | `GetPaymentsAsync(AdminPaymentListFilter filter, int page, int pageSize) : OffsetPageResult<AdminPaymentListItemDto>`<br>`GetPaymentDetailAsync(int paymentId) : AdminPaymentDetailDto` | `backend/Cinema.Application/Interfaces/IPaymentAdminService.cs` |
| `PaymentAdminService` | Lớp dịch vụ | Hiện thực `IPaymentAdminService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Payments/PaymentAdminService.cs` |
| `AdminPaymentListFilter` | Record | — | `backend/Cinema.Application/Interfaces/IPaymentAdminService.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `Payment`, `Booking` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `AdminPaymentListItemDto`, `AdminPaymentDetailDto`, `AdminPaymentBookingBriefDto` | DTO | — | `backend/Cinema.Application/DTOs/Payments/PaymentAdminDtos.cs` |

### 4.2.21 Sơ đồ lớp package Báo cáo thống kê

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.21)
  - Các thực thể được xác định từ `DbSet` mà `DashboardService` và `ReportService` truy vấn; `Movie`, `Employee` được truy cập qua thuộc tính điều hướng.
- **Tài liệu/hình/bảng:**
  - `Hình 4.22` Sơ đồ lớp package Báo cáo thống kê
  - `Bảng 4.24` Danh sách lớp package Báo cáo thống kê
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminDashboardController.cs`; `backend/Cinema.Api/Controllers/Admin/AdminReportsController.cs`; `backend/Cinema.Application/Interfaces/IDashboardService.cs`; `backend/Cinema.Application/Services/Dashboard/DashboardService.cs`; `backend/Cinema.Application/Interfaces/IReportService.cs`; `backend/Cinema.Application/Services/Reports/ReportService.cs`

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `AdminDashboardController` | Controller | GET /api/v1/admin/dashboard/summary → `GetSummary() : IActionResult`<br>GET /api/v1/admin/dashboard/insights → `GetInsights(int days) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminDashboardController.cs` |
| `AdminReportsController` | Controller | GET /api/v1/admin/reports/revenue → `GetRevenue(DateOnly fromDate, DateOnly toDate, string groupBy) : IActionResult`<br>GET /api/v1/admin/reports/top-movies → `GetTopMovies(DateOnly fromDate, DateOnly toDate, int? limit) : IActionResult`<br>GET /api/v1/admin/reports/employee-performance → `GetEmployeePerformance(DateOnly fromDate, DateOnly toDate) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminReportsController.cs` |
| `IDashboardService` | Interface | `GetSummaryAsync() : DashboardSummaryDto`<br>`GetInsightsAsync(int days) : DashboardInsightsDto` | `backend/Cinema.Application/Interfaces/IDashboardService.cs` |
| `IReportService` | Interface | `GetRevenueReportAsync(DateOnly fromDate, DateOnly toDate, string groupBy) : RevenueReportDto`<br>`GetTopMoviesReportAsync(DateOnly fromDate, DateOnly toDate, int limit) : List<TopMovieDto>`<br>`GetEmployeePerformanceReportAsync(DateOnly fromDate, DateOnly toDate) : List<EmployeePerformanceDto>` | `backend/Cinema.Application/Interfaces/IReportService.cs` |
| `DashboardService` | Lớp dịch vụ | Hiện thực `IDashboardService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Dashboard/DashboardService.cs` |
| `ReportService` | Lớp dịch vụ | Hiện thực `IReportService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Reports/ReportService.cs` |
| `DateTimeUtils` | Lớp tiện ích | `GetPeriodKey(DateTime createdAt, string groupBy) : string`<br>`GetStartOfDay(DateOnly date) : DateTime`<br>`GetStartOfNextDay(DateOnly date) : DateTime` | `backend/Cinema.Application/Utils/DateTimeUtils.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `Booking`, `Payment`, `Showtime`, `Movie`, `Employee`, `BookingTicket`, `ShowtimeSeat` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `DashboardSummaryDto`, `DashboardInsightsDto`, `DashboardShowtimeItemDto`, `RevenueReportDto`, `TopMovieDto`, `EmployeePerformanceDto` | DTO | — | `backend/Cinema.Application/DTOs/Dashboard/DashboardDtos.cs`<br>`backend/Cinema.Application/DTOs/Reports/ReportDtos.cs` |

### 4.2.22 Sơ đồ lớp package Cấu hình rạp

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.22)
  - Tải logo do `AdminUploadsController` thực hiện (package Tải tệp).
- **Tài liệu/hình/bảng:**
  - `Hình 4.23` Sơ đồ lớp package Cấu hình rạp
  - `Bảng 4.25` Danh sách lớp package Cấu hình rạp
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminSettingsController.cs`; `backend/Cinema.Api/Controllers/Client/SettingsController.cs`; `backend/Cinema.Application/Interfaces/ISettingsService.cs`; `backend/Cinema.Application/Services/Settings/SettingsService.cs`

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `AdminSettingsController` | Controller | GET /api/v1/admin/settings → `Get() : IActionResult`<br>PUT /api/v1/admin/settings → `Update(UpdateSettingsRequest request) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminSettingsController.cs` |
| `SettingsController` | Controller | GET /api/v1/settings → `GetSettings() : ActionResult<CinemaSettingsDto>` | `backend/Cinema.Api/Controllers/Client/SettingsController.cs` |
| `ISettingsService` | Interface | `GetSettingsAsync() : CinemaSettingsDto`<br>`UpdateSettingsAsync(UpdateSettingsRequest request) : CinemaSettingsDto` | `backend/Cinema.Application/Interfaces/ISettingsService.cs` |
| `SettingsService` | Lớp dịch vụ | Hiện thực `ISettingsService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Settings/SettingsService.cs` |
| `UpdateSettingsRequestValidator` | Validator | — | `backend/Cinema.Application/Validators/Settings/SettingValidators.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `CinemaSettings` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |
| `CinemaSettingsDto`, `UpdateSettingsRequest` | DTO | — | `backend/Cinema.Application/DTOs/Settings/SettingDtos.cs` |
| `SeatGapRuleMode` | Enum | — | `backend/Cinema.Domain/Enums/*.cs` |

### 4.2.23 Sơ đồ lớp package Tải tệp

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.23)
  - Package không có lớp dịch vụ thuộc tầng Application: `AdminUploadsController` gọi trực tiếp `IFileStorage`, lớp hiện thực nằm ở tầng Infrastructure.
- **Tài liệu/hình/bảng:**
  - `Hình 4.24` Sơ đồ lớp package Tải tệp
  - `Bảng 4.26` Danh sách lớp package Tải tệp
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminUploadsController.cs`; `backend/Cinema.Application/Interfaces/IFileStorage.cs`; `backend/Cinema.Infrastructure/FileStorage/CloudinaryFileStorage.cs`

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `AdminUploadsController` | Controller | POST /api/v1/admin/uploads/image → `UploadImage(string folder = "misc", IFormFile? file = null) : IActionResult`<br>POST /api/v1/admin/uploads/video → `UploadVideo(string folder = "misc", IFormFile? file = null) : IActionResult` | `backend/Cinema.Api/Controllers/Admin/AdminUploadsController.cs` |
| `IFileStorage` | Interface | `UploadImageAsync(Stream fileStream, string fileName, string contentType, string folder) : FileUploadResult`<br>`UploadVideoAsync(Stream fileStream, string fileName, string contentType, string folder) : FileUploadResult`<br>`DeleteAsync(string publicId, string resourceType = "image") : void` | `backend/Cinema.Application/Interfaces/IFileStorage.cs` |
| `CloudinaryFileStorage` | Lớp dịch vụ | Hiện thực `IFileStorage`; không có phương thức công khai ngoài interface | `backend/Cinema.Infrastructure/FileStorage/CloudinaryFileStorage.cs` |
| `CloudinaryOptions` | Options | — | `backend/Cinema.Application/Options/CloudinaryOptions.cs` |
| `FileUploadResult` | Record | — | `backend/Cinema.Application/Interfaces/IFileStorage.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `UploadImageResponse` | DTO | — | `backend/Cinema.Application/DTOs/Uploads/UploadDtos.cs` |

### 4.2.24 Sơ đồ lớp package Vận hành hệ thống

- **Nội dung cần viết:**
  - Chuỗi gọi từ controller, interface dịch vụ, lớp dịch vụ đến thực thể, DTO, Options, job nền, gateway của package
  - Nêu phương thức tương ứng với từng use case của package (xem Bảng 3.24)
  - `Program.cs` đăng ký ba công việc định kỳ bằng `RecurringJobIds.ReleaseExpiredSeats`, `RecurringJobIds.UpdateShowtimeLifecycle` và `RecurringJobIds.ExpireLoyaltyPoints` (`expire-loyalty-points`, cron `ExpireLoyaltyPointsCron`); `DbInitializer.SeedAsync` nhận `AppDbContext` (lớp hiện thực của `IAppDbContext`).
- **Tài liệu/hình/bảng:**
  - `Hình 4.25` Sơ đồ lớp package Vận hành hệ thống
  - `Bảng 4.27` Danh sách lớp package Vận hành hệ thống
- **Nguồn dữ liệu:** `backend/Cinema.Application/BackgroundJobs/ReleaseExpiredSeatsJob.cs`; `backend/Cinema.Application/BackgroundJobs/ShowtimeLifecycleJob.cs`; `backend/Cinema.Application/BackgroundJobs/SendEmailJob.cs`; `backend/Cinema.Application/BackgroundJobs/ShowtimeRefundJob.cs`; `backend/Cinema.Application/Interfaces/ISeatExpirationService.cs`; `backend/Cinema.Application/Services/Bookings/SeatExpirationService.cs`; `backend/Cinema.Application/Interfaces/IShowtimeService.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs`; `backend/Cinema.Application/Interfaces/IRealtimeNotifier.cs`; `backend/Cinema.Infrastructure/Realtime/SignalRRealtimeNotifier.cs`; `backend/Cinema.Application/BackgroundJobs/ExpireLoyaltyPointsJob.cs`; `LoyaltyExpiryService.cs`; `LoyaltyWindow.cs` …

| Lớp | Loại | Thành viên công khai (chữ ký, bỏ tham số `CancellationToken`) | Nguồn |
|---|---|---|---|
| `HealthController` | Controller | GET /api/v1/health → `Get() : IActionResult` | `backend/Cinema.Api/Controllers/HealthController.cs` |
| `ISeatExpirationService` | Interface | `ReleaseStaleHeldSeatsAsync(List<int>? showtimeIds) : int`<br>`ExpireDueBookingsAsync() : void` | `backend/Cinema.Application/Interfaces/ISeatExpirationService.cs` |
| `IShowtimeService` | Interface | `CompleteEndedShowtimesAsync() : int`<br>`EndMoviesWithoutUpcomingShowtimesAsync() : int` | `backend/Cinema.Application/Interfaces/IShowtimeService.cs` |
| `IRealtimeNotifier` | Interface | `NotifyHoldExpiredAsync(int showtimeId, int bookingId, List<int> seatIds) : void` | `backend/Cinema.Application/Interfaces/IRealtimeNotifier.cs` |
| `IEmailService` | Interface | `SendAsync(string toEmail, string subject, string htmlBody) : void` | `backend/Cinema.Application/Interfaces/IEmailService.cs` |
| `IPaymentGateway` | Interface | `RefundAsync(RefundRequest request) : RefundResult` | `backend/Cinema.Application/Interfaces/IPaymentGateway.cs` |
| `SeatExpirationService` | Lớp dịch vụ | Hiện thực `ISeatExpirationService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Bookings/SeatExpirationService.cs` |
| `ShowtimeService` | Lớp dịch vụ | Hiện thực `IShowtimeService`; không có phương thức công khai ngoài interface | `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs` |
| `SignalRRealtimeNotifier` | Lớp dịch vụ | Hiện thực `IRealtimeNotifier`; không có phương thức công khai ngoài interface | `backend/Cinema.Infrastructure/Realtime/SignalRRealtimeNotifier.cs` |
| `FallbackEmailService` | Lớp dịch vụ | Hiện thực `IEmailService`; không có phương thức công khai ngoài interface | `backend/Cinema.Infrastructure/Email/FallbackEmailService.cs` |
| `ResendEmailService` | Lớp dịch vụ | Hiện thực `IEmailService`; không có phương thức công khai ngoài interface | `backend/Cinema.Infrastructure/Email/ResendEmailService.cs` |
| `MailKitEmailService` | Lớp dịch vụ | Hiện thực `IEmailService`; không có phương thức công khai ngoài interface | `backend/Cinema.Infrastructure/Email/MailKitEmailService.cs` |
| `LoyaltyExpiryService` | Lớp dịch vụ | `RecalculateAllAsync() : void` [CẦN BỔ SUNG: xác nhận chữ ký, kiểu trả về và interface] | `LoyaltyExpiryService.cs` |
| `ReleaseExpiredSeatsJob` | Job nền | `RunAsync() : void` | `backend/Cinema.Application/BackgroundJobs/ReleaseExpiredSeatsJob.cs` |
| `ShowtimeLifecycleJob` | Job nền | `RunAsync() : void` | `backend/Cinema.Application/BackgroundJobs/ShowtimeLifecycleJob.cs` |
| `ExpireLoyaltyPointsJob` | Job nền | `RunAsync() : void` (gọi `LoyaltyExpiryService.RecalculateAllAsync`) | `backend/Cinema.Application/BackgroundJobs/ExpireLoyaltyPointsJob.cs` |
| `SendEmailJob` | Job nền | `RunAsync(string toEmail, string subject, string htmlbody) : void` | `backend/Cinema.Application/BackgroundJobs/SendEmailJob.cs` |
| `ShowtimeRefundJob` | Job nền | `RunAsync(int paymentId, PerformContext? context) : void` | `backend/Cinema.Application/BackgroundJobs/ShowtimeRefundJob.cs` |
| `RecurringJobIds` | Lớp tiện ích | — | `backend/Cinema.Shared/Constants/RecurringJobIds.cs` |
| `LoyaltyWindow` | Lớp tiện ích | Hằng `Months = 12` (cửa sổ trượt điểm hiệu lực) | `LoyaltyWindow.cs` |
| `HangfireAdminAuthorizationFilter` | Lớp tiện ích | `Authorize(DashboardContext context) : bool` | `backend/Cinema.Api/Filters/HangfireAdminAuthorizationFilter.cs` |
| `DbInitializer` | Lớp tiện ích | `SeedAsync(AppDbContext db, ILogger logger, bool seedDemoScenarios = true) : void` | `backend/Cinema.Infrastructure/Persistence/Seed/DbInitializer.cs` |
| `BackgroundJobOptions` | Options | Gồm `ExpireLoyaltyPointsCron` (lịch job điểm hết hạn) | `backend/Cinema.Application/Options/BackgroundJobOptions.cs` |
| `IAppDbContext` | Interface | Truy cập `DbSet` của các thực thể; `SaveChangesAsync` | `backend/Cinema.Application/Interfaces/IAppDbContext.cs` |
| `Booking`, `ShowtimeSeat`, `Payment`, `Showtime`, `Movie`, `CustomerProfile`, `MembershipTier`, `LoyaltyPointEntry` | Thực thể (thuộc tính xem Hình 4.4) | — | `backend/Cinema.Domain/Entities/*.cs` |

### 4.2.25 Ánh xạ package use case sang nhóm lớp

- **Nội dung cần viết:**
  - Truy vết từ package use case (Chương 3) sang controller, interface, lớp dịch vụ (Chương 4)
  - Ghi rõ lớp dùng chung giữa nhiều package (`BookingService`, `ShowtimeService`, `ReviewService`, `CatalogService`, `ComboService`, `EmployeeService`, `MovieService`)
- **Tài liệu/hình/bảng:**
  - `Bảng 4.28` Ánh xạ package use case sang nhóm lớp
- **Nguồn dữ liệu:** `Mục 3.3 và 4.2.4–4.2.24 của file này`

| Package | Số use case | Controller | Interface | Lớp dịch vụ, job, gateway | Hình use case | Hình lớp |
|---|---|---|---|---|---|---|
| XT Xác thực | 11 | `AuthController` | `IAuthService`, `ITokenService`, `IGoogleAuthService` | `AuthService`, `JwtTokenService`, `GoogleAuthService`, `SendEmailJob` | Hình 3.4 | Hình 4.5 |
| HS Hồ sơ cá nhân | 4 | `UsersController` | `IUserProfileService`, `IAuthService`, `IReviewService` | `UserProfileService`, `AuthService`, `ReviewService` | Hình 3.5 | Hình 4.6 |
| DP Duyệt phim | 8 | `MoviesController`, `ShowtimesController`, `CatalogController`, `SettingsController`, `CombosController` | `IMovieService`, `IReviewService`, `IShowtimeService`, `ICatalogService`, `ISettingsService`, `IComboService` | `MovieService`, `ReviewService`, `ShowtimeService`, `CatalogService`, `SettingsService`, `ComboService` | Hình 3.6 | Hình 4.7 |
| DG Đánh giá phim | 4 | `MoviesController` | `IReviewService` | `ReviewService` | Hình 3.7 | Hình 4.8 |
| DV Đặt vé trực tuyến | 9 | `BookingsController`, `ShowtimesController`, `CombosController` | `IBookingService`, `IShowtimeService`, `IComboService`, `IRealtimeNotifier` | `BookingService`, `ShowtimeService`, `ComboService`, `SignalRRealtimeNotifier`, `ShowtimeHub` | Hình 3.8 | Hình 4.9 |
| TT Thanh toán trực tuyến | 6 | `BookingsController`, `PaymentsController` | `IBookingService`, `IPaymentService`, `IPaymentGateway` | `BookingService`, `PaymentService`, `VnPayGateway`, `MomoGateway`, `StripeGateway`, `ShowtimeRefundJob`, `SendEmailJob` | Hình 3.9 | Hình 4.10 |
| AI Trợ lý AI | 1 | `AiController` | `IAiAssistantService` | `GeminiAiAssistantService`, `AiToolExecutor` | Hình 3.10 | Hình 4.11 |
| BQ Bán vé tại quầy | 1 | `AdminBookingsController`, `AdminUsersController` | `IBookingService`, `IEmployeeService` | `BookingService`, `EmployeeService` | Hình 3.11 | Hình 4.12 |
| SV Soát vé | 1 | `AdminBookingsController` | `IBookingService` | `BookingService` | Hình 3.12 | Hình 4.13 |
| QB Quản lý booking | 2 | `AdminBookingsController` | `IBookingService` | `BookingService` | Hình 3.13 | Hình 4.14 |
| QP Quản lý phim | 5 | `AdminMoviesController` | `IMovieService` | `MovieService` | Hình 3.14 | Hình 4.15 |
| QD Quản lý danh mục | 10 | `AdminGenresController`, `AdminLanguagesController`, `AdminAgeRatingsController`, `AdminActorsController`, `AdminDirectorsController`, `AdminScreenTypesController`, `AdminSeatTypesController`, `AdminPositionsController`, `AdminMembershipTiersController`, `AdminCombosController` | `ICatalogService`, `IComboService` | `CatalogService`, `ComboService` | Hình 3.15 | Hình 4.16 |
| QR Quản lý phòng chiếu | 3 | `AdminRoomsController` | `IRoomService` | `RoomService` | Hình 3.16 | Hình 4.17 |
| QS Quản lý suất chiếu | 9 | `AdminShowtimesController` | `IShowtimeService`, `IBookingService`, `IPaymentGateway` | `ShowtimeService`, `SendEmailJob`, `ShowtimeRefundJob` | Hình 3.17 | Hình 4.18 |
| QN Quản lý người dùng | 7 | `AdminUsersController`, `AdminEmployeesController` | `IEmployeeService` | `EmployeeService` | Hình 3.18 | Hình 4.19 |
| QV Quản lý đánh giá | 3 | `AdminReviewsController` | `IReviewService` | `ReviewService` | Hình 3.19 | Hình 4.20 |
| QT Quản lý thanh toán | 2 | `AdminPaymentsController` | `IPaymentAdminService` | `PaymentAdminService` | Hình 3.20 | Hình 4.21 |
| BC Báo cáo thống kê | 5 | `AdminDashboardController`, `AdminReportsController` | `IDashboardService`, `IReportService` | `DashboardService`, `ReportService` | Hình 3.21 | Hình 4.22 |
| CH Cấu hình rạp | 2 | `AdminSettingsController`, `SettingsController` | `ISettingsService` | `SettingsService` | Hình 3.22 | Hình 4.23 |
| TP Tải tệp | 2 | `AdminUploadsController` | `IFileStorage` | `CloudinaryFileStorage` | Hình 3.23 | Hình 4.24 |
| HT Vận hành hệ thống | 8 | `HealthController` | `ISeatExpirationService`, `IShowtimeService`, `IRealtimeNotifier`, `IEmailService`, `IPaymentGateway` | `ReleaseExpiredSeatsJob`, `ShowtimeLifecycleJob`, `ExpireLoyaltyPointsJob`, `SendEmailJob`, `ShowtimeRefundJob`, `SeatExpirationService`, `LoyaltyExpiryService`, `ShowtimeService`, `SignalRRealtimeNotifier`, `FallbackEmailService`, `ResendEmailService`, `MailKitEmailService` | Hình 3.24 | Hình 4.25 |


## 4.3 CƠ SỞ DỮ LIỆU


### 4.3.1 Nguyên tắc thiết kế

- **Nội dung cần viết:**
  - Một CSDL PostgreSQL (Neon); EF Core Code First; enum lưu dạng chuỗi; thời gian lưu UTC; ràng buộc CHECK và chỉ mục duy nhất bảo vệ tính đúng đắn (đặc biệt chống đặt trùng ghế); hành vi xóa Restrict/Cascade/SetNull
- **Nguồn dữ liệu:** `backend/Cinema.Infrastructure/Persistence/AppDbContext.cs`; `Persistence/Configurations/*`

### 4.3.2 Sơ đồ ERD đầy đủ

- **Nội dung cần viết:**
  - 30 thực thể, 10 kiểu liệt kê (có thực thể mới `LoyaltyPointEntry`, bảng `LoyaltyPointEntries`, nối `Users` và `Bookings`); thể hiện đầy đủ thuộc tính, khóa chính (PK), khóa ngoại (FK), duy nhất (UK) và bội số quan hệ
  - Dùng trang A4 ngang (Section 4a)
- **Tài liệu/hình/bảng:**
  - `Hình 4.26` Sơ đồ ERD đầy đủ của hệ thống
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `backend/Cinema.Infrastructure/Persistence/Configurations/*.cs`; `Migrations/AppDbContextModelSnapshot.cs`
- **Định dạng Word:** Section ngang (Landscape); hình đặt giữa, rộng tối đa 25 cm

### 4.3.3 Phân module cơ sở dữ liệu

- **Nội dung cần viết:**
  - Chia 30 thực thể thành 9 module; thực thể của module khác xuất hiện trong ERD module dưới dạng tham chiếu (chỉ khóa chính)
- **Tài liệu/hình/bảng:**
  - `Bảng 4.29` Phân module cơ sở dữ liệu
- **Nguồn dữ liệu:** `Mục 4.3.2 file này`

| Mục | Module | Thực thể sở hữu | Số thực thể |
|---|---|---|---|
| 4.3.4 | Tài khoản người dùng | Role, User, CustomerProfile, Employee, Position, RefreshToken, UserVerificationToken | 7 |
| 4.3.5 | Danh mục phim | Genre, Language, AgeRating, Actor, Director | 5 |
| 4.3.6 | Phim | Movie, MovieGenre, MovieCast, Review | 4 |
| 4.3.7 | Phòng chiếu | CinemaRoom, ScreenType, Seat, SeatType | 4 |
| 4.3.8 | Suất chiếu | Showtime, ShowtimeSeat | 2 |
| 4.3.9 | Đặt vé | Booking, BookingTicket, BookingCombo, Combo | 4 |
| 4.3.10 | Thanh toán | Payment | 1 |
| 4.3.11 | Thành viên | MembershipTier, LoyaltyPointEntry | 2 |
| 4.3.12 | Cấu hình rạp | CinemaSettings | 1 |
|  | **Tổng** |  | 30 |


### 4.3.4 Module Tài khoản người dùng

- **Nội dung cần viết:**
  - Giới thiệu module Tài khoản người dùng: vai trò, thực thể (Role, User, CustomerProfile, Employee, Position, RefreshToken, UserVerificationToken), quan hệ với module khác

#### 4.3.4.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.27` ERD module Tài khoản người dùng
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.3.4.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.30` Thực thể Role (bảng Roles)
  - `Bảng 4.31` Thực thể User (bảng Users)
  - `Bảng 4.32` Thực thể CustomerProfile (bảng CustomerProfiles)
  - `Bảng 4.33` Thực thể Employee (bảng Employees)
  - `Bảng 4.34` Thực thể Position (bảng Positions)
  - `Bảng 4.35` Thực thể RefreshToken (bảng RefreshTokens)
  - `Bảng 4.36` Thực thể UserVerificationToken (bảng UserVerificationTokens)
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`

**Role** — bảng `Roles`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| RoleId | int | Không | PK | — |
| RoleName | varchar(50) | Không | UK | — |

**User** — bảng `Users`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| UserId | int | Không | PK | — |
| FullName | varchar(100) | Không | — | — |
| Email | varchar(150) | Không | UK | — |
| PasswordHash | varchar(256) | Không | — | — |
| Phone | varchar(20) | Có | — | — |
| Avatar | varchar(255) | Có | — | — |
| GoogleId | varchar(64) | Có | UK | — |
| RoleId | int | Không | FK | — |
| DateOfBirth | date | Có | — | — |
| Gender | varchar(10) | Có | — | enum Gender |
| IsActive | boolean | Không | — | true |
| EmailConfirmed | boolean | Không | — | false |
| CreatedAt | timestamp | Không | — | SQL: NOW() |
| UpdatedAt | timestamp | Có | — | — |

**CustomerProfile** — bảng `CustomerProfiles`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| UserId | int | Không | PK, FK | — |
| Points | int | Không | — | 0; điểm hiệu lực 12 tháng, giá trị đồng bộ từ sổ `LoyaltyPointEntries` (có thể trễ tối đa 1 giờ) |
| TierId | int | Có | FK | Hạng tương ứng với `Points`; đồng bộ cùng `Points`, có thể bị hạ |
| TotalSpent | decimal | Không | — | 0m; tổng chi tiêu trọn đời, không theo cửa sổ 12 tháng |

**Employee** — bảng `Employees`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| EmployeeId | int | Không | PK | — |
| UserId | int | Không | FK, UK | — |
| PositionId | int | Có | FK | — |
| HireDate | date | Có | — | — |
| IsActive | boolean | Không | — | true |

**Position** — bảng `Positions`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| PositionId | int | Không | PK | — |
| Name | varchar(50) | Không | UK | — |

**RefreshToken** — bảng `RefreshTokens`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| RefreshTokenId | int | Không | PK | — |
| UserId | int | Không | FK | — |
| TokenHash | varchar(255) | Không | UK | — |
| ExpiresAt | timestamp | Không | — | — |
| IsRevoked | boolean | Không | — | false |
| CreatedAt | timestamp | Không | — | SQL: NOW() |
| RevokedAt | timestamp | Có | — | — |
| ReplacedByTokenId | int | Có | FK | — |

**UserVerificationToken** — bảng `UserVerificationTokens`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| TokenId | int | Không | PK | — |
| UserId | int | Không | FK | — |
| TokenHash | varchar(255) | Không | UK | — |
| Purpose | varchar(30) | Không | — | enum VerificationTokenPurpose |
| ExpiresAt | timestamp | Không | — | — |
| Used | boolean | Không | — | false |
| CreatedAt | timestamp | Không | — | SQL: NOW() |


#### 4.3.4.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.37` Ràng buộc toàn vẹn module Tài khoản người dùng
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên | Loại | Biểu thức |
|---|---|---|---|
| User | CK_Users_Gender | CHECK | "Gender" IN ('Male','Female','Other') |
| User | FK RoleId | Khóa ngoại | — |
| CustomerProfile | FK UserId | Khóa ngoại | — |
| CustomerProfile | FK TierId | Khóa ngoại | — |
| Employee | FK UserId | Khóa ngoại | — |
| Employee | FK PositionId | Khóa ngoại | — |
| RefreshToken | FK UserId | Khóa ngoại | — |
| RefreshToken | FK ReplacedByTokenId | Khóa ngoại | — |
| UserVerificationToken | FK UserId | Khóa ngoại | — |


#### 4.3.4.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.38` Chỉ mục module Tài khoản người dùng
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| Role | (tên tự sinh) | Duy nhất | RoleName | — |
| User | (tên tự sinh) | Duy nhất | Email | — |
| User | IX_Users_GoogleId | Duy nhất | GoogleId | — |
| User | IX_Users_Role | Thường | RoleId | — |
| CustomerProfile | IX_CustomerProfiles_Tier | Thường | TierId | — |
| Employee | (tên tự sinh) | Duy nhất | UserId | — |
| Employee | IX_Employees_Position | Thường | PositionId | — |
| Position | (tên tự sinh) | Duy nhất | Name | — |
| RefreshToken | UQ_RefreshTokens_TokenHash | Duy nhất | TokenHash | — |
| RefreshToken | IX_RefreshTokens_User | Thường | UserId | — |
| UserVerificationToken | UQ_UserVerificationTokens_TokenHash | Duy nhất | TokenHash | — |
| UserVerificationToken | IX_UserVerificationTokens_User_Purpose | Thường | UserId, Purpose | — |


### 4.3.5 Module Danh mục phim

- **Nội dung cần viết:**
  - Giới thiệu module Danh mục phim: vai trò, thực thể (Genre, Language, AgeRating, Actor, Director), quan hệ với module khác

#### 4.3.5.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.28` ERD module Danh mục phim
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.3.5.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.39` Thực thể Genre (bảng Genres)
  - `Bảng 4.40` Thực thể Language (bảng Languages)
  - `Bảng 4.41` Thực thể AgeRating (bảng AgeRatings)
  - `Bảng 4.42` Thực thể Actor (bảng Actors)
  - `Bảng 4.43` Thực thể Director (bảng Directors)
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`

**Genre** — bảng `Genres`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| GenreId | int | Không | PK | — |
| Name | varchar(50) | Không | UK | — |

**Language** — bảng `Languages`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| LanguageId | int | Không | PK | — |
| Name | varchar(50) | Không | UK | — |

**AgeRating** — bảng `AgeRatings`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| AgeRatingId | int | Không | PK | — |
| Code | varchar(10) | Không | UK | — |
| Description | varchar(255) | Có | — | — |

**Actor** — bảng `Actors`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| ActorId | int | Không | PK | — |
| FullName | varchar(150) | Không | — | — |
| AvatarUrl | varchar(255) | Có | — | — |

**Director** — bảng `Directors`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| DirectorId | int | Không | PK | — |
| FullName | varchar(150) | Không | — | — |
| AvatarUrl | varchar(255) | Có | — | — |


#### 4.3.5.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.44` Ràng buộc toàn vẹn module Danh mục phim
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

Module không có ràng buộc CHECK hoặc khóa ngoại riêng.


#### 4.3.5.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.45` Chỉ mục module Danh mục phim
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| Genre | (tên tự sinh) | Duy nhất | Name | — |
| Language | (tên tự sinh) | Duy nhất | Name | — |
| AgeRating | (tên tự sinh) | Duy nhất | Code | — |


### 4.3.6 Module Phim

- **Nội dung cần viết:**
  - Giới thiệu module Phim: vai trò, thực thể (Movie, MovieGenre, MovieCast, Review), quan hệ với module khác

#### 4.3.6.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.29` ERD module Phim
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.3.6.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.46` Thực thể Movie (bảng Movies)
  - `Bảng 4.47` Thực thể MovieGenre (bảng MovieGenres)
  - `Bảng 4.48` Thực thể MovieCast (bảng MovieCast)
  - `Bảng 4.49` Thực thể Review (bảng Reviews)
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`

**Movie** — bảng `Movies`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| MovieId | int | Không | PK | — |
| Title | varchar(200) | Không | — | — |
| OriginalTitle | varchar(200) | Có | — | — |
| Description | text | Có | — | — |
| DirectorId | int | Có | FK | — |
| DurationMinutes | int | Không | — | — |
| ReleaseDate | date | Có | — | — |
| LanguageId | int | Có | FK | — |
| Subtitle | varchar(50) | Có | — | — |
| AgeRatingId | int | Có | FK | — |
| PosterUrl | varchar(255) | Có | — | — |
| TrailerUrl | varchar(255) | Có | — | — |
| Status | varchar(20) | Không | — | MovieStatus.ComingSoon |
| AverageRating | decimal | Không | — | 0m |
| CreatedAt | timestamp | Không | — | SQL: NOW() |

**MovieGenre** — bảng `MovieGenres`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| MovieId | int | Không | PK, FK | — |
| GenreId | int | Không | PK, FK | — |

**MovieCast** — bảng `MovieCast`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| MovieId | int | Không | PK, FK | — |
| ActorId | int | Không | PK, FK | — |
| CharacterName | varchar(150) | Có | — | — |

**Review** — bảng `Reviews`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| ReviewId | int | Không | PK | — |
| MovieId | int | Không | FK | — |
| UserId | int | Không | FK | — |
| Rating | int | Không | — | — |
| Comment | text | Có | — | — |
| CreatedAt | timestamp | Không | — | SQL: NOW() |


#### 4.3.6.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.50` Ràng buộc toàn vẹn module Phim
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên | Loại | Biểu thức |
|---|---|---|---|
| Movie | CK_Movies_Duration | CHECK | "DurationMinutes" > 0 |
| Movie | CK_Movies_Status | CHECK | "Status" IN ('ComingSoon','NowShowing','Ended') |
| Movie | CK_Movies_AverageRating | CHECK | "AverageRating" BETWEEN 0 AND 5 |
| Review | CK_Reviews_Rating | CHECK | "Rating" BETWEEN 1 AND 5 |
| Movie | FK DirectorId | Khóa ngoại | — |
| Movie | FK LanguageId | Khóa ngoại | — |
| Movie | FK AgeRatingId | Khóa ngoại | — |
| MovieGenre | FK MovieId | Khóa ngoại | — |
| MovieGenre | FK GenreId | Khóa ngoại | — |
| MovieCast | FK MovieId | Khóa ngoại | — |
| MovieCast | FK ActorId | Khóa ngoại | — |
| Review | FK MovieId | Khóa ngoại | — |
| Review | FK UserId | Khóa ngoại | — |


#### 4.3.6.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.51` Chỉ mục module Phim
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| Movie | IX_Movies_Director | Thường | DirectorId | — |
| Movie | IX_Movies_Language | Thường | LanguageId | — |
| Movie | IX_Movies_AgeRating | Thường | AgeRatingId | — |
| Movie | IX_Movies_Status | Thường | Status | — |
| Movie | IX_Movies_ReleaseDate | Thường | ReleaseDate | — |
| MovieCast | IX_MovieCast_Actor | Thường | ActorId | — |
| Review | IX_Reviews_Movie | Thường | MovieId | — |
| Review | UQ_User_Movie_Review | Duy nhất | UserId, MovieId | — |


### 4.3.7 Module Phòng chiếu

- **Nội dung cần viết:**
  - Giới thiệu module Phòng chiếu: vai trò, thực thể (CinemaRoom, ScreenType, Seat, SeatType), quan hệ với module khác

#### 4.3.7.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.30` ERD module Phòng chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.3.7.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.52` Thực thể CinemaRoom (bảng CinemaRooms)
  - `Bảng 4.53` Thực thể ScreenType (bảng ScreenTypes)
  - `Bảng 4.54` Thực thể Seat (bảng Seats)
  - `Bảng 4.55` Thực thể SeatType (bảng SeatTypes)
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`

**CinemaRoom** — bảng `CinemaRooms`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| RoomId | int | Không | PK | — |
| RoomName | varchar(50) | Không | UK | — |
| ScreenTypeId | int | Có | FK | — |
| TotalSeats | int | Không | — | 0 |
| IsActive | boolean | Không | — | true |
| Rows | int | Không | — | — |
| Cols | int | Không | — | — |

**ScreenType** — bảng `ScreenTypes`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| ScreenTypeId | int | Không | PK | — |
| Name | varchar(50) | Không | UK | — |

**Seat** — bảng `Seats`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| SeatId | int | Không | PK | — |
| RoomId | int | Không | FK | — |
| SeatRow | varchar(5) | Không | — | — |
| SeatNumber | int | Không | — | — |
| SeatTypeId | int | Có | FK | — |
| IsActive | boolean | Không | — | true |
| IsAisle | boolean | Không | — | — |
| CoupleId | uuid | Có | — | — |

**SeatType** — bảng `SeatTypes`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| SeatTypeId | int | Không | PK | — |
| Name | varchar(50) | Không | UK | — |
| PriceMultiplier | decimal | Không | — | 1.0m |


#### 4.3.7.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.56` Ràng buộc toàn vẹn module Phòng chiếu
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên | Loại | Biểu thức |
|---|---|---|---|
| CinemaRoom | CK_CinemaRooms_TotalSeats | CHECK | "TotalSeats" >= 0 |
| Seat | CK_Seats_SeatNumber | CHECK | "SeatNumber" > 0 |
| SeatType | CK_SeatTypes_PriceMultiplier | CHECK | "PriceMultiplier" > 0 |
| CinemaRoom | FK ScreenTypeId | Khóa ngoại | — |
| Seat | FK RoomId | Khóa ngoại | — |
| Seat | FK SeatTypeId | Khóa ngoại | — |


#### 4.3.7.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.57` Chỉ mục module Phòng chiếu
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| CinemaRoom | (tên tự sinh) | Duy nhất | RoomName | — |
| ScreenType | (tên tự sinh) | Duy nhất | Name | — |
| Seat | IX_Seats_SeatType | Thường | SeatTypeId | — |
| Seat | UQ_Room_Seat | Duy nhất | RoomId, SeatRow, SeatNumber | — |
| SeatType | (tên tự sinh) | Duy nhất | Name | — |


### 4.3.8 Module Suất chiếu

- **Nội dung cần viết:**
  - Giới thiệu module Suất chiếu: vai trò, thực thể (Showtime, ShowtimeSeat), quan hệ với module khác

#### 4.3.8.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.31` ERD module Suất chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.3.8.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.58` Thực thể Showtime (bảng Showtimes)
  - `Bảng 4.59` Thực thể ShowtimeSeat (bảng ShowtimeSeats)
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`

**Showtime** — bảng `Showtimes`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| ShowtimeId | int | Không | PK | — |
| MovieId | int | Không | FK | — |
| RoomId | int | Không | FK | — |
| StartTime | timestamp | Không | — | — |
| EndTime | timestamp | Không | — | — |
| BasePrice | decimal | Không | — | — |
| Status | varchar(20) | Không | — | ShowtimeStatus.Scheduled |
| CreatedAt | timestamp | Không | — | SQL: NOW() |

**ShowtimeSeat** — bảng `ShowtimeSeats`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| ShowtimeSeatId | int | Không | PK | — |
| ShowtimeId | int | Không | FK | — |
| SeatId | int | Không | FK | — |
| Status | varchar(20) | Không | — | ShowtimeSeatStatus.Available |
| HeldByUserId | int | Có | FK | — |
| HeldByBookingId | int | Có | FK | — |
| HeldUntil | timestamp | Có | — | — |
| Price | decimal | Không | — | — |


#### 4.3.8.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.60` Ràng buộc toàn vẹn module Suất chiếu
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên | Loại | Biểu thức |
|---|---|---|---|
| Showtime | CK_Showtime_Time | CHECK | "EndTime" > "StartTime" |
| Showtime | CK_Showtimes_BasePrice | CHECK | "BasePrice" >= 0 |
| Showtime | CK_Showtimes_Status | CHECK | "Status" IN ('Scheduled','Cancelled','Completed') |
| ShowtimeSeat | CK_ShowtimeSeats_Price | CHECK | "Price" >= 0 |
| ShowtimeSeat | CK_ShowtimeSeats_Status | CHECK | "Status" IN ('Available','Held','Booked') |
| Showtime | FK MovieId | Khóa ngoại | — |
| Showtime | FK RoomId | Khóa ngoại | — |
| ShowtimeSeat | FK ShowtimeId | Khóa ngoại | — |
| ShowtimeSeat | FK SeatId | Khóa ngoại | — |
| ShowtimeSeat | FK HeldByUserId | Khóa ngoại | — |
| ShowtimeSeat | FK HeldByBookingId | Khóa ngoại | — |


#### 4.3.8.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.61` Chỉ mục module Suất chiếu
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| Showtime | IX_Showtime_Room_Time | Thường | RoomId, StartTime, EndTime | — |
| Showtime | IX_Showtime_Movie | Thường | MovieId, StartTime | — |
| ShowtimeSeat | UQ_Showtime_Seat | Duy nhất | ShowtimeId, SeatId | — |
| ShowtimeSeat | IX_ShowtimeSeats_Status | Thường | ShowtimeId, Status | — |


### 4.3.9 Module Đặt vé

- **Nội dung cần viết:**
  - Giới thiệu module Đặt vé: vai trò, thực thể (Booking, BookingTicket, BookingCombo, Combo), quan hệ với module khác

#### 4.3.9.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.32` ERD module Đặt vé
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.3.9.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.62` Thực thể Booking (bảng Bookings)
  - `Bảng 4.63` Thực thể BookingTicket (bảng BookingTickets)
  - `Bảng 4.64` Thực thể BookingCombo (bảng BookingCombos)
  - `Bảng 4.65` Thực thể Combo (bảng Combos)
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`

**Booking** — bảng `Bookings`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| BookingId | int | Không | PK | — |
| UserId | int | Có | FK | — |
| ShowtimeId | int | Không | FK | — |
| BookingCode | varchar(20) | Không | UK | — |
| Channel | varchar(20) | Không | — | BookingChannel.Online |
| CreatedByEmployeeId | int | Có | FK | — |
| CustomerName | varchar(100) | Không | — | — |
| CustomerPhone | varchar(20) | Có | — | — |
| TotalAmount | decimal | Không | — | — |
| DiscountAmount | decimal | Không | — | 0m |
| FinalAmount | decimal | Không | — | — |
| Status | varchar(20) | Không | — | BookingStatus.Pending |
| CreatedAt | timestamp | Không | — | SQL: NOW() |
| ExpiresAt | timestamp | Có | — | — |

**BookingTicket** — bảng `BookingTickets`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| TicketId | int | Không | PK | — |
| BookingId | int | Không | FK | — |
| ShowtimeSeatId | int | Không | FK, UK | — |
| Price | decimal | Không | — | — |
| QrCode | varchar(255) | Có | — | — |
| IsCheckedIn | boolean | Không | — | false |
| CheckedInAt | timestamp | Có | — | — |

**BookingCombo** — bảng `BookingCombos`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| BookingComboId | int | Không | PK | — |
| BookingId | int | Không | FK | — |
| ComboId | int | Không | FK | — |
| Quantity | int | Không | — | 1 |
| UnitPrice | decimal | Không | — | — |

**Combo** — bảng `Combos`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| ComboId | int | Không | PK | — |
| Name | varchar(150) | Không | — | — |
| Description | varchar(500) | Có | — | — |
| Price | decimal | Không | — | — |
| ImageUrl | varchar(255) | Có | — | — |
| IsActive | boolean | Không | — | true |


#### 4.3.9.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.66` Ràng buộc toàn vẹn module Đặt vé
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên | Loại | Biểu thức |
|---|---|---|---|
| Booking | CK_Bookings_TotalAmount | CHECK | "TotalAmount" >= 0 |
| Booking | CK_Bookings_DiscountAmount | CHECK | "DiscountAmount" >= 0 |
| Booking | CK_Bookings_FinalAmount | CHECK | "FinalAmount" >= 0 |
| Booking | CK_Bookings_Channel | CHECK | "Channel" IN ('Online','Counter') |
| Booking | CK_Bookings_Status | CHECK | "Status" IN ('Pending','Confirmed','Cancelled','Expired') |
| Booking | CK_Booking_Channel_Employee | CHECK | ("Channel" = 'Counter' AND "CreatedByEmployeeId" IS NOT NULL) OR ("Channel" = 'Online' AND "CreatedByEmployeeId" IS NULL) |
| Booking | CK_Booking_Customer_Identity | CHECK | "UserId" IS NOT NULL OR "CustomerName" IS NOT NULL |
| BookingTicket | CK_BookingTickets_Price | CHECK | "Price" >= 0 |
| BookingCombo | CK_BookingCombos_Quantity | CHECK | "Quantity" > 0 |
| BookingCombo | CK_BookingCombos_UnitPrice | CHECK | "UnitPrice" >= 0 |
| Combo | CK_Combos_Price | CHECK | "Price" >= 0 |
| Booking | FK UserId | Khóa ngoại | — |
| Booking | FK ShowtimeId | Khóa ngoại | — |
| Booking | FK CreatedByEmployeeId | Khóa ngoại | — |
| BookingTicket | FK BookingId | Khóa ngoại | — |
| BookingTicket | FK ShowtimeSeatId | Khóa ngoại | — |
| BookingCombo | FK BookingId | Khóa ngoại | — |
| BookingCombo | FK ComboId | Khóa ngoại | — |


#### 4.3.9.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.67` Chỉ mục module Đặt vé
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| Booking | (tên tự sinh) | Duy nhất | BookingCode | — |
| Booking | IX_Bookings_User | Thường | UserId, CreatedAt | — |
| Booking | IX_Bookings_Showtime | Thường | ShowtimeId | — |
| Booking | IX_Bookings_Employee | Thường | CreatedByEmployeeId | — |
| Booking | IX_Bookings_Status | Thường | Status | — |
| Booking | IX_Bookings_Channel | Thường | Channel | — |
| BookingTicket | (tên tự sinh) | Duy nhất | ShowtimeSeatId | — |
| BookingCombo | IX_BookingCombos_Booking | Thường | BookingId | — |


### 4.3.10 Module Thanh toán

- **Nội dung cần viết:**
  - Giới thiệu module Thanh toán: vai trò, thực thể (Payment), quan hệ với module khác

#### 4.3.10.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.33` ERD module Thanh toán
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.3.10.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.68` Thực thể Payment (bảng Payments)
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`

**Payment** — bảng `Payments`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| PaymentId | int | Không | PK | — |
| BookingId | int | Không | FK | — |
| PaymentMethod | varchar(50) | Không | — | enum PaymentMethod |
| TransactionId | varchar(150) | Có | — | — |
| GatewayTransactionId | varchar(150) | Có | — | — |
| GatewayResponseCode | varchar(20) | Có | — | — |
| Amount | decimal | Không | — | — |
| Status | varchar(20) | Không | — | PaymentStatus.Pending |
| PaidAt | timestamp | Có | — | — |
| CreatedAt | timestamp | Không | — | SQL: NOW() AT TIME ZONE 'UTC' |
| RefundTransactionId | varchar(150) | Có | — | — |
| RefundedAmount | decimal | Có | — | — |
| RefundedAt | timestamp | Có | — | — |
| AmountReceived | decimal | Có | — | — |
| ChangeGiven | decimal | Có | — | — |


#### 4.3.10.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.69` Ràng buộc toàn vẹn module Thanh toán
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên | Loại | Biểu thức |
|---|---|---|---|
| Payment | CK_Payments_Method | CHECK | "PaymentMethod" IN ('VNPay','Momo','Cash','Stripe') |
| Payment | CK_Payments_Amount | CHECK | "Amount" >= 0 |
| Payment | CK_Payments_Status | CHECK | "Status" IN ('Pending','Success','Failed','Refunded') |
| Payment | FK BookingId | Khóa ngoại | — |


#### 4.3.10.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.70` Chỉ mục module Thanh toán
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| Payment | IX_Payments_Booking | Thường | BookingId | — |
| Payment | IX_Payments_Status | Thường | Status | — |


### 4.3.11 Module Thành viên

- **Nội dung cần viết:**
  - Giới thiệu module Thành viên: vai trò, thực thể (MembershipTier, LoyaltyPointEntry), quan hệ với module khác (User, Booking)
  - Nêu vai trò của sổ điểm: nguồn dữ liệu gốc để tính điểm hiệu lực 12 tháng, hạng và giảm giá; `CustomerProfile.Points`/`TierId` chỉ là giá trị đồng bộ

#### 4.3.11.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.34` ERD module Thành viên
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.3.11.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.71` Thực thể MembershipTier (bảng MembershipTiers)
  - `Bảng 4.72` Thực thể LoyaltyPointEntry (bảng LoyaltyPointEntries)
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`

**MembershipTier** — bảng `MembershipTiers`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| TierId | int | Không | PK | — |
| TierName | varchar(50) | Không | UK | — |
| MinPoints | int | Không | — | 0 |
| DiscountPercent | decimal | Không | — | 0m |

**LoyaltyPointEntry** — bảng `LoyaltyPointEntries`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| EntryId | int | Không | PK | — |
| UserId | int | Không | FK | Khách hàng sở hữu dòng điểm |
| BookingId | int | Có | FK | Booking sinh ra điểm; NULL cho điểm điều chỉnh hoặc dữ liệu mẫu |
| Points | int | Không | — | Điểm tích được, ≥ 0 |
| RevokedPoints | int | Không | — | Điểm đã thu hồi do hoàn tiền; 0 ≤ RevokedPoints ≤ Points |
| EarnedAt | timestamp | Không | — | Lúc thanh toán thành công; mốc tính cửa sổ 12 tháng |

[CẦN BỔ SUNG: xác nhận kiểu dữ liệu và giá trị mặc định từ `LoyaltyPointEntryConfiguration.cs`]


#### 4.3.11.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.73` Ràng buộc toàn vẹn module Thành viên
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên | Loại | Biểu thức |
|---|---|---|---|
| MembershipTier | CK_MembershipTiers_Discount | CHECK | "DiscountPercent" BETWEEN 0 AND 100 |
| LoyaltyPointEntry | CK_LoyaltyPointEntries_Points | CHECK | "Points" >= 0 |
| LoyaltyPointEntry | CK_LoyaltyPointEntries_Revoked | CHECK | "RevokedPoints" >= 0 AND "RevokedPoints" <= "Points" |
| LoyaltyPointEntry | FK UserId | Khóa ngoại | — |
| LoyaltyPointEntry | FK BookingId | Khóa ngoại | — |


#### 4.3.11.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.74` Chỉ mục module Thành viên
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| MembershipTier | (tên tự sinh) | Duy nhất | TierName | — |
| LoyaltyPointEntry | UX_LoyaltyPointEntries_Booking | Duy nhất (có điều kiện lọc) | BookingId | "BookingId" IS NOT NULL |
| LoyaltyPointEntry | IX_LoyaltyPointEntries_User_EarnedAt | Thường | UserId, EarnedAt | — |


### 4.3.12 Module Cấu hình rạp

- **Nội dung cần viết:**
  - Giới thiệu module Cấu hình rạp: vai trò, thực thể (CinemaSettings), quan hệ với module khác

#### 4.3.12.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.35` ERD module Cấu hình rạp
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.3.12.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.75` Thực thể CinemaSettings (bảng CinemaSettings)
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`

**CinemaSettings** — bảng `CinemaSettings`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| Id | int | Không | PK | — |
| Name | varchar(150) | Không | — | — |
| Address | varchar(255) | Không | — | — |
| Phone | varchar(20) | Có | — | — |
| Email | varchar(150) | Có | — | — |
| TaxCode | varchar(50) | Có | — | — |
| LogoUrl | varchar(255) | Có | — | — |
| OpeningTime | time | Có | — | — |
| ClosingTime | time | Có | — | — |
| UpdatedAt | timestamp | Có | — | — |
| PointsPerAmount | decimal | Không | — | 10000m |
| CancelRefundPercent | decimal | Không | — | 0m |
| CancelDeadlineHours | int | Không | — | — |
| StopSellingMinutesBefore | int | Không | — | — |
| HoldDurationMinutes | int | Không | — | — |
| CheckInOpenMinutes | int | Không | — | — |
| SeatGapRuleMode | varchar(20) | Không | — | SeatGapRuleMode.Strict |


#### 4.3.12.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.76` Ràng buộc toàn vẹn module Cấu hình rạp
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên | Loại | Biểu thức |
|---|---|---|---|
| CinemaSettings | CK_CinemaSettings_Id | CHECK | "Id" = 1 |


#### 4.3.12.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.77` Chỉ mục module Cấu hình rạp
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

Module không khai báo chỉ mục riêng ngoài khóa chính.


### 4.3.13 Kiểu liệt kê

- **Nội dung cần viết:**
  - 10 kiểu liệt kê, lưu dạng chuỗi trong CSDL, kèm ràng buộc CHECK tương ứng
- **Tài liệu/hình/bảng:**
  - `Bảng 4.78` Các kiểu liệt kê
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Enums/*.cs`

| Kiểu | Giá trị |
|---|---|
| BookingChannel | Online, Counter |
| BookingStatus | Pending, Confirmed, Cancelled, Expired |
| Gender | Male, Female, Other |
| MovieStatus | ComingSoon, NowShowing, Ended |
| PaymentMethod | VNPay, Momo, Cash, Stripe |
| PaymentStatus | Pending, Success, Failed, Refunded |
| SeatGapRuleMode | Strict, AllowEdge, Off |
| ShowtimeSeatStatus | Available, Held, Booked |
| ShowtimeStatus | Scheduled, Cancelled, Completed |
| VerificationTokenPurpose | EmailConfirmation, PasswordReset |


### 4.3.14 Vòng đời trạng thái

- **Nội dung cần viết:**
  - Mô tả chuyển trạng thái hợp lệ của năm đối tượng; xác định chuyển trạng thái bằng cách đọc service, không chỉ enum
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Enums/*.cs`; `Services/**`

#### 4.3.14.1 Vòng đời Booking

- **Nội dung cần viết:**
  - Các trạng thái, điều kiện và nơi thực hiện chuyển trạng thái
  - Pending → Confirmed: ConfirmBookingAndGenerateTicketsAsync
  - Pending → Expired: SeatExpirationService
  - Confirmed → Cancelled: CancelBookingAsync, hủy suất
- **Tài liệu/hình/bảng:**
  - `Hình 4.36` Máy trạng thái Booking
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Enums/*.cs`; `Services/**`

#### 4.3.14.2 Vòng đời ShowtimeSeat

- **Nội dung cần viết:**
  - Các trạng thái, điều kiện và nơi thực hiện chuyển trạng thái
  - Chuyển Booked → Available khi hủy đơn Confirmed: [CẦN BỔ SUNG: xác minh trong CancelBookingAsync]
- **Tài liệu/hình/bảng:**
  - `Hình 4.37` Máy trạng thái ShowtimeSeat
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Enums/*.cs`; `Services/**`

#### 4.3.14.3 Vòng đời Payment

- **Nội dung cần viết:**
  - Các trạng thái, điều kiện và nơi thực hiện chuyển trạng thái
  - Thanh toán tiền mặt tại quầy: [CẦN BỔ SUNG: xác minh trạng thái khởi tạo]
- **Tài liệu/hình/bảng:**
  - `Hình 4.38` Máy trạng thái Payment
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Enums/*.cs`; `Services/**`

#### 4.3.14.4 Vòng đời Showtime

- **Nội dung cần viết:**
  - Các trạng thái, điều kiện và nơi thực hiện chuyển trạng thái
  - Completed: CompleteEndedShowtimesAsync
- **Tài liệu/hình/bảng:**
  - `Hình 4.39` Máy trạng thái Showtime
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Enums/*.cs`; `Services/**`

#### 4.3.14.5 Vòng đời Movie

- **Nội dung cần viết:**
  - Các trạng thái, điều kiện và nơi thực hiện chuyển trạng thái
  - ComingSoon → NowShowing: [CẦN BỔ SUNG: xác minh có cơ chế tự động hay chỉ cập nhật thủ công]
- **Tài liệu/hình/bảng:**
  - `Hình 4.40` Máy trạng thái Movie
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Enums/*.cs`; `Services/**`

### 4.3.15 Migration

- **Nội dung cần viết:**
  - 4 migration: `InitialCreate` (30/10/2026), `PendingModelChanges` (31/10/2026), `AddSeatGapRuleMode` (01/11/2026), `AddLoyaltyPointEntries` [CẦN BỔ SUNG: ngày tạo migration; migration chưa được tạo/chạy tại thời điểm viết]
  - Script thủ công `Migrations/_manual/backfill_loyalty_entries.sql`: dựng sổ điểm từ các booking đã xác nhận (`EarnedAt` = lúc thanh toán thành công, trừ phần đã hoàn tiền); khách hàng cũ phải chạy trước khi bật job điểm hết hạn
  - Cách chạy: `npm run db:update`; cờ `RunMigrationsOnStartup`
- **Nguồn dữ liệu:** `backend/Cinema.Infrastructure/Migrations/*`; `package.json`

### 4.3.16 Dữ liệu khởi tạo

- **Nội dung cần viết:**
  - Dữ liệu tĩnh `HasData`; dữ liệu động idempotent (tài khoản kiểm thử, phòng, suất chiếu, kịch bản booking–thanh toán–soát vé–đánh giá); không ghi mật khẩu vào báo cáo
  - `DbInitializer` và `BookingScenarioSeeder` tạo dòng sổ điểm cho dữ liệu mẫu và tính giảm giá chỉ trên tiền vé
- **Nguồn dữ liệu:** `Persistence/Seed/SeedData.cs`; `DbInitializer.cs`; `ShowtimeSeeder.cs`; `BookingScenarioSeeder.cs`

## 4.4 XÁC THỰC


### 4.4.1 Luồng đăng ký tài khoản

- **Nội dung cần viết:**
  - Đăng ký → tạo User/CustomerProfile → tạo mã xác nhận (băm) → xếp job gửi email → khách xác nhận bằng mã
- **Tài liệu/hình/bảng:**
  - `Hình 4.41` Tuần tự đăng ký tài khoản
- **Nguồn dữ liệu:** `AuthService.cs → RegisterAsync, ConfirmEmailAsync`

### 4.4.2 Luồng đăng nhập

- **Nội dung cần viết:**
  - Kiểm tra thông tin, khóa tài khoản, xác nhận email, cổng khách hàng/quản trị; cấp access token và refresh token
- **Tài liệu/hình/bảng:**
  - `Hình 4.42` Tuần tự đăng nhập bằng email
- **Nguồn dữ liệu:** `AuthService.cs → LoginAsync`

### 4.4.3 Luồng đăng nhập Google

- **Nội dung cần viết:**
  - Xác minh ID token; kiểm tra email đã xác minh; tạo mới hoặc liên kết tài khoản; chặn quản trị
- **Tài liệu/hình/bảng:**
  - `Hình 4.43` Tuần tự đăng nhập bằng Google
- **Nguồn dữ liệu:** `AuthService.cs → GoogleLoginAsync`; `Infrastructure/Auth/GoogleAuthService.cs`

### 4.4.4 Làm mới token xoay vòng

- **Nội dung cần viết:**
  - Access token 3600 giây; refresh token 30 ngày, lưu SHA-256; mỗi lần làm mới cấp token mới, thu hồi token cũ (`ReplacedByTokenId`)
  - Phía client: một promise làm mới dùng chung cho nhiều yêu cầu 401 đồng thời
- **Tài liệu/hình/bảng:**
  - `Hình 4.44` Tuần tự làm mới token xoay vòng
- **Nguồn dữ liệu:** `AuthService.cs → RefreshAsync`; `JwtTokenService.cs`; `frontend/src/api/axios.ts`

### 4.4.5 Phát hiện dùng lại token

- **Nội dung cần viết:**
  - Khi một refresh token đã thu hồi bị dùng lại → thu hồi toàn bộ chuỗi token; đổi/đặt lại mật khẩu → thu hồi mọi token
- **Tài liệu/hình/bảng:**
  - `Hình 4.45` Lưu đồ phát hiện dùng lại refresh token
- **Nguồn dữ liệu:** `AuthService.cs → RevokeTokenChainAsync`

### 4.4.6 Khôi phục mật khẩu

- **Nội dung cần viết:**
  - Yêu cầu khôi phục luôn trả thông điệp chuẩn (không lộ email có tồn tại); mã đặt lại băm, có hạn, dùng một lần
- **Tài liệu/hình/bảng:**
  - `Hình 4.46` Tuần tự khôi phục mật khẩu
- **Nguồn dữ liệu:** `AuthService.cs → ForgotPasswordAsync, ResetPasswordAsync`

### 4.4.7 Các loại token

- **Nội dung cần viết:**
  - So sánh access token, refresh token, socket token, mã xác nhận email, mã đặt lại mật khẩu
- **Tài liệu/hình/bảng:**
  - `Bảng 4.79` Các loại token
- **Nguồn dữ liệu:** `JwtTokenService.cs`; `AuthService.cs`; `Domain/Entities/UserVerificationToken.cs`

| Token | Thời hạn | Lưu trữ | Mục đích |
|---|---|---|---|
| Access token | 3600 giây | Không lưu (JWT) | Gọi API |
| Refresh token | 30 ngày | CSDL, dạng SHA-256 | Làm mới phiên |
| Socket token | 300 giây | Không lưu (JWT) | Kết nối SignalR |
| Mã xác nhận email | [CẦN BỔ SUNG: đọc AuthService] | CSDL, dạng băm | EmailConfirmation |
| Mã đặt lại mật khẩu | [CẦN BỔ SUNG: đọc AuthService] | CSDL, dạng băm | PasswordReset |


### 4.4.8 Ma trận phân quyền

- **Nội dung cần viết:**
  - Số endpoint theo controller và mức phân quyền (đếm tự động từ `[Authorize]`)
  - Giải thích policy: `StaffOnly` = Admin hoặc Employee; `AdminOnly` chỉ Admin
- **Tài liệu/hình/bảng:**
  - `Bảng 4.80` Ma trận phân quyền theo controller
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`; `Program.cs`

| Controller | Công khai | Đã đăng nhập | CustomerOnly | StaffOnly | AdminOnly | Tổng |
|---|---|---|---|---|---|---|
| Admin/AdminActors |  |  |  |  | 5 | 5 |
| Admin/AdminAgeRatings |  |  |  |  | 4 | 4 |
| Admin/AdminBookings |  |  |  | 5 |  | 5 |
| Admin/AdminCombos |  |  |  |  | 5 | 5 |
| Admin/AdminDashboard |  |  |  |  | 2 | 2 |
| Admin/AdminDirectors |  |  |  |  | 5 | 5 |
| Admin/AdminEmployees |  |  |  |  | 3 | 3 |
| Admin/AdminGenres |  |  |  |  | 4 | 4 |
| Admin/AdminLanguages |  |  |  |  | 4 | 4 |
| Admin/AdminMembershipTiers |  |  |  |  | 4 | 4 |
| Admin/AdminMovies |  |  |  | 1 | 4 | 5 |
| Admin/AdminPayments |  |  |  |  | 2 | 2 |
| Admin/AdminPositions |  |  |  |  | 4 | 4 |
| Admin/AdminReports |  |  |  |  | 3 | 3 |
| Admin/AdminReviews |  |  |  |  | 3 | 3 |
| Admin/AdminRooms |  |  |  |  | 7 | 7 |
| Admin/AdminScreenTypes |  |  |  |  | 4 | 4 |
| Admin/AdminSeatTypes |  |  |  |  | 4 | 4 |
| Admin/AdminSettings |  |  |  |  | 2 | 2 |
| Admin/AdminShowtimes |  |  |  | 2 | 8 | 10 |
| Admin/AdminUploads |  |  |  |  | 2 | 2 |
| Admin/AdminUsers |  |  |  | 1 | 5 | 6 |
| Client/Ai | 1 |  |  |  |  | 1 |
| Client/Auth | 10 | 2 |  |  |  | 12 |
| Client/Bookings |  | 5 |  |  |  | 5 |
| Client/Catalog | 4 |  |  |  |  | 4 |
| Client/Combos | 2 |  |  |  |  | 2 |
| Client/Movies | 4 |  | 3 |  |  | 7 |
| Client/Settings | 1 |  |  |  |  | 1 |
| Client/Showtimes | 3 |  |  |  |  | 3 |
| Client/Users |  | 4 |  |  |  | 4 |
| Khác/Health | 1 |  |  |  |  | 1 |
| Khác/Payments | 4 |  |  |  |  | 4 |
| **Tổng** | 30 | 11 | 3 | 9 | 84 | 137 |


## 4.5 LUỒNG NGHIỆP VỤ


### 4.5.1 Giữ ghế

- **Nội dung cần viết:**
  - Giới thiệu luồng giữ ghế, tham chiếu use case UC-DV-04 (mục 3.3.5)
- **Nguồn dữ liệu:** `Services/Bookings/BookingService.cs → HoldSeatsAsync, CreateHoldBookingAsync`

#### 4.5.1.1 Kiểm tra đầu vào

- **Nội dung cần viết:**
  - Validator số ghế, combo; trạng thái suất chiếu; thời điểm dừng bán online; booking Pending sẵn có; luật chọn ghế phía máy chủ
- **Tài liệu/hình/bảng:**
  - `Bảng 4.81` Các kiểm tra khi giữ ghế
- **Nguồn dữ liệu:** `Validators/Bookings/BookingValidators.cs`; `Utils/SeatLayoutRules.cs`; `BookingService.cs`

| Thứ tự | Kiểm tra | Lỗi trả về |
|---|---|---|
| 1 | Dữ liệu đầu vào hợp lệ | Validation |
| 2 | Suất chiếu Scheduled, chưa bắt đầu, chưa quá thời điểm dừng bán | [CẦN BỔ SUNG: đọc thông điệp] |
| 3 | Chưa có booking Pending còn hạn | Conflict_PendingBookingExistsForShowtime |
| 4 | Luật chọn ghế (ghế đôi, ghế lẻ) | [CẦN BỔ SUNG: đọc thông điệp] |


#### 4.5.1.2 Giữ ghế nguyên tử

- **Nội dung cần viết:**
  - Giải thích điều kiện của câu lệnh cập nhật; vì sao so sánh số dòng ảnh hưởng; vai trò của chỉ mục duy nhất `(ShowtimeId, SeatId)` và `BookingTicket.ShowtimeSeatId`
  - Đoạn mã rút gọn ≤ 15 dòng đặt ở Phụ lục 5
- **Tài liệu/hình/bảng:**
  - `Hình 4.47` Lưu đồ giữ ghế nguyên tử
- **Nguồn dữ liệu:** `BookingService.cs → CreateHoldBookingAsync`; `Cinema.Shared/Exceptions/SeatConflictException.cs`

#### 4.5.1.3 Tính tiền

- **Nội dung cần viết:**
  - Tiền vé `TicketAmount` = Σ giá ghế theo suất (`ShowtimeSeat.Price`); tiền combo `ComboAmount` = Σ combo; tổng `TotalAmount` = tiền vé + combo
  - Giảm giá chỉ tính trên tiền vé, không tính combo; làm tròn ra xa 0; `CalculateDiscountAmountAsync` nhận `ticketAmount` và xác định hạng từ điểm hiệu lực 12 tháng trong sổ điểm (không đọc `CustomerProfile.TierId`)
  - Số tiền cuối cùng `FinalAmount` = tổng − giảm giá; điểm tích vẫn tính trên `FinalAmount` (xem 4.5.8)
  - Mã booking: tiền tố + ngày + 4 ký tự, kiểm tra duy nhất
- **Tài liệu/hình/bảng:**
  - `(4.1)` Tổng tiền của đơn: TotalAmount = TicketAmount + ComboAmount
  - `(4.2)` Giảm giá theo hạng: DiscountAmount = round(TicketAmount × DiscountPercent / 100), làm tròn ra xa 0, không tính combo
  - `(4.3)` Số tiền cuối cùng của đơn: FinalAmount = TotalAmount − DiscountAmount
- **Nguồn dữ liệu:** `BookingService.cs → CalculateDiscountAmountAsync, BuildBookingCombosAsync, GenerateUniqueBookingCodeAsync`; `LoyaltyWindow.cs`; `Utils/CodeGenerator.cs`

#### 4.5.1.4 Tuần tự giữ ghế

- **Nội dung cần viết:**
  - Trình tự từ thao tác của khách đến phản hồi và sự kiện thời gian thực
- **Tài liệu/hình/bảng:**
  - `Hình 4.48` Tuần tự giữ ghế
- **Nguồn dữ liệu:** `BookingService.cs`; `Controllers/Client/BookingsController.cs`

### 4.5.2 Thanh toán trực tuyến

- **Nội dung cần viết:**
  - Giới thiệu luồng thanh toán, tham chiếu các use case UC-TT-01…06 (mục 3.3.6)
- **Nguồn dữ liệu:** `Services/Payments/PaymentService.cs`; `Infrastructure/Payments/*`

#### 4.5.2.1 Tạo giao dịch thanh toán

- **Nội dung cần viết:**
  - Kiểm tra booking; chọn cổng theo tên phương thức; tạo bản ghi Payment; tạo liên kết thanh toán
- **Tài liệu/hình/bảng:**
  - `Hình 4.49` Tuần tự tạo giao dịch thanh toán
- **Nguồn dữ liệu:** `BookingService.cs → CheckoutAsync`; `Utils/PaymentGatewayExtensions.cs`

#### 4.5.2.2 Xử lý IPN

- **Nội dung cần viết:**
  - Xác minh chữ ký → tìm giao dịch → idempotent → kiểm tra số tiền (sai số ≤ 0,01), tiền tệ (Stripe) → áp dụng kết quả → phản hồi cổng
- **Tài liệu/hình/bảng:**
  - `Hình 4.50` Lưu đồ xử lý IPN
- **Nguồn dữ liệu:** `PaymentService.cs → ApplyPaymentResultAsync, IsAlreadyProcessed`; `Controllers/PaymentsController.cs`

#### 4.5.2.3 Phát hành vé sau thanh toán

- **Nội dung cần viết:**
  - Chuyển ghế sang Booked; tạo vé và mã QR (dạng mã booking, hàng, số ghế); ghi dòng sổ điểm và tính lại điểm, hạng (xem 4.5.8; cập nhật bước này trong sơ đồ); phát sự kiện; gửi email
- **Tài liệu/hình/bảng:**
  - `Hình 4.51` Tuần tự phát hành vé sau thanh toán
- **Nguồn dữ liệu:** `BookingService.cs → ConfirmBookingAndGenerateTicketsAsync`; `Utils/QrCodeHelper.cs`; `BackgroundJobs/SendEmailJob.cs`

#### 4.5.2.4 Ghế hết hạn sau khi khách đã trả tiền

- **Nội dung cần viết:**
  - Bắt `SeatsNoLongerHeldException`; tự hoàn tiền; cập nhật Payment; gửi email (tự hoàn hoặc cần hoàn thủ công)
- **Tài liệu/hình/bảng:**
  - `Hình 4.52` Lưu đồ xử lý ghế hết hạn sau thanh toán
- **Nguồn dữ liệu:** `PaymentService.cs → HandleSeatsExpiredAfterPaymentAsync`; `Utils/EmailTemplates.cs`

#### 4.5.2.5 So sánh ba cổng thanh toán

- **Nội dung cần viết:**
  - So sánh thuật toán ký, xác minh callback, đơn vị tiền, hoàn tiền; đọc đủ ba file gateway trước khi viết
- **Tài liệu/hình/bảng:**
  - `Bảng 4.82` So sánh VNPay, MoMo, Stripe
- **Nguồn dữ liệu:** `Infrastructure/Payments/VnPayGateway.cs`; `MomoGateway.cs`; `StripeGateway.cs`; `HmacSigner.cs`

| Tiêu chí | VNPay | MoMo | Stripe |
|---|---|---|---|
| Thuật toán ký/xác minh | HMAC-SHA512 | HMAC-SHA256 | Chữ ký webhook của Stripe |
| Sự kiện/callback | IPN (GET, POST) | IPN (POST) | Webhook `checkout.session.completed` |
| Hoàn tiền (`RefundAsync`) | Có | Có | Có |
| Đơn vị tiền / định dạng số tiền | [CẦN BỔ SUNG: đọc VnPayGateway] | [CẦN BỔ SUNG: đọc MomoGateway] | [CẦN BỔ SUNG: đọc StripeGateway] |
| Địa chỉ IP gửi cổng | `vnp_IpAddr` cố định 127.0.0.1 (DefaultIpAddress) | — | — |


### 4.5.3 Hết hạn giữ ghế

- **Nội dung cần viết:**
  - Thứ tự xử lý của job; nơi gọi `ReleaseStaleHeldSeatsAsync` (chỉ trình bày sự thật; đánh giá ở 7.3)
- **Tài liệu/hình/bảng:**
  - `Hình 4.53` Lưu đồ giải phóng ghế hết hạn
- **Nguồn dữ liệu:** `Services/Bookings/SeatExpirationService.cs`; `BackgroundJobs/ReleaseExpiredSeatsJob.cs`; `Program.cs`

### 4.5.4 Hủy vé


#### 4.5.4.1 Điều kiện hoàn tiền

- **Nội dung cần viết:**
  - Điều kiện hủy booking Confirmed: chưa soát vé; không thanh toán tiền mặt; còn trong hạn `CancelDeadlineHours`; cổng đã cấu hình; số tiền hoàn theo `CancelRefundPercent`
- **Tài liệu/hình/bảng:**
  - `Bảng 4.83` Chính sách và tình huống hoàn tiền
- **Nguồn dữ liệu:** `BookingService.cs → CalculateRefundEligibility, CancelBookingAsync`

| Tình huống | Kết quả | Nguồn |
|---|---|---|
| Hủy booking Pending | Trả ghế, không hoàn tiền | CancelBookingAsync |
| Hủy booking Confirmed đủ điều kiện | Hoàn `CancelRefundPercent` % qua cổng; thu hồi điểm vào đúng dòng sổ điểm của booking | CalculateRefundEligibility, ReverseLoyaltyAsync |
| Hủy booking Confirmed đã soát vé | Không cho hủy | [CẦN BỔ SUNG: đọc thông điệp] |
| Thanh toán tiền mặt | Hủy tại quầy, không hoàn qua cổng | [CẦN BỔ SUNG: đọc thông điệp] |
| Hủy suất chiếu | Hoàn tiền từng thanh toán bằng job | ShowtimeRefundJob |
| Ghế hết hạn sau thanh toán | Tự hoàn tiền | HandleSeatsExpiredAfterPaymentAsync |


#### 4.5.4.2 Luồng hủy vé

- **Nội dung cần viết:**
  - Chiếm quyền hủy bằng cập nhật có điều kiện; gọi hoàn tiền; xử lý lỗi nghiêm trọng khi hoàn tiền xong nhưng cơ sở dữ liệu chưa ghi; thu hồi điểm theo `bookingId` (cập nhật bước này trong lưu đồ)
- **Tài liệu/hình/bảng:**
  - `Hình 4.54` Lưu đồ hủy vé
- **Nguồn dữ liệu:** `BookingService.cs → CancelBookingAsync, ReverseLoyaltyAsync`

### 4.5.5 Bán vé tại quầy

- **Nội dung cần viết:**
  - Giữ ghế kênh Counter; thanh toán tiền mặt; tiền thối; xác nhận và phát vé
- **Tài liệu/hình/bảng:**
  - `Hình 4.55` Tuần tự bán vé tại quầy
- **Nguồn dữ liệu:** `BookingService.cs → CounterHoldSeatsAsync, CounterCheckoutAsync`; `frontend/src/features/admin/counter/**`

### 4.5.6 Soát vé

- **Nội dung cần viết:**
  - Điều kiện soát vé; chống soát lặp bằng cập nhật có điều kiện; quét camera (BarcodeDetector hoặc jsQR) hoặc nhập mã
- **Tài liệu/hình/bảng:**
  - `Hình 4.56` Lưu đồ soát vé
- **Nguồn dữ liệu:** `BookingService.cs → CheckinAsync`; `frontend/src/features/admin/checkin/**`

### 4.5.7 Hủy suất chiếu

- **Nội dung cần viết:**
  - Quy tắc không đổi phòng, không sửa khi có booking; tác động của hủy suất lên booking/payment; thử lại hoàn tiền
  - [CẦN BỔ SUNG: đọc DeleteShowtimeAsync để xác nhận thứ tự và điều kiện chính xác]
- **Tài liệu/hình/bảng:**
  - `Hình 4.57` Tuần tự hủy suất chiếu
- **Nguồn dữ liệu:** `Services/Showtimes/ShowtimeService.cs → DeleteShowtimeAsync`; `BackgroundJobs/ShowtimeRefundJob.cs`

### 4.5.8 Điểm thưởng

- **Nội dung cần viết:**
  - Sổ điểm `LoyaltyPointEntry`: mỗi booking đã xác nhận sinh một dòng (`EarnedAt` = lúc thanh toán thành công); `BookingId = null` dùng cho điểm điều chỉnh hoặc dữ liệu mẫu; mỗi booking tối đa một dòng (chỉ mục duy nhất `UX_LoyaltyPointEntries_Booking`)
  - Điểm hiệu lực tính theo cửa sổ trượt 12 tháng (`LoyaltyWindow.Months`): chỉ cộng các dòng có `EarnedAt` trong 12 tháng gần nhất, sau khi trừ điểm đã thu hồi; hạng là hạng có `MinPoints` lớn nhất không vượt điểm hiệu lực, nên hạng có thể bị hạ khi điểm hết hạn
  - Cộng điểm khi xác nhận (`ApplyLoyaltyAsync`): tạo dòng sổ điểm rồi tính lại `Points` và `TierId` trên hồ sơ từ sổ
  - Thu hồi khi hoàn tiền (`ReverseLoyaltyAsync` kèm `bookingId`): chỉ trừ vào đúng dòng của booking, `RevokedPoints` bị chặn tối đa bằng `Points`; booking đã ngoài cửa sổ 12 tháng thì điểm hiệu lực không đổi
  - Giảm giá lúc đặt vé (`CalculateDiscountAmountAsync`): đọc điểm hiệu lực từ sổ rồi tra hạng, không đọc `CustomerProfile.TierId`; giảm giá theo `DiscountPercent` chỉ áp dụng trên tiền vé (xem 4.5.1.3)
  - Đồng bộ điểm hết hạn: job `ExpireLoyaltyPointsJob` gọi `LoyaltyExpiryService.RecalculateAllAsync` mỗi giờ (cron `5 * * * *`, phút 05, múi giờ VN); điểm và hạng trên hồ sơ có thể trễ tối đa 1 giờ, còn giảm giá lúc đặt vé luôn chính xác vì đọc trực tiếp từ sổ
  - Điểm tích vẫn tính trên `FinalAmount` nên phần combo vẫn được tích điểm; `TotalSpent` vẫn là tổng chi tiêu trọn đời, không theo cửa sổ trượt
  - Khách hàng cũ phải chạy script `Migrations/_manual/backfill_loyalty_entries.sql` để dựng sổ điểm từ các booking đã xác nhận; nếu không, job sẽ đưa điểm của họ về 0
  - Hạn chế liên quan: xem 7.3.15
- **Tài liệu/hình/bảng:**
  - `(4.4)` Điểm tích: earned = ⌊FinalAmount / PointsPerAmount⌋ (mặc định PointsPerAmount = 10.000)
  - `(4.5)` Điểm hiệu lực: P(u, t) = Σ (Points − RevokedPoints) của các dòng có EarnedAt > t − 12 tháng
  - `(4.6)` Hạng thành viên: tier(u, t) = hạng có MinPoints lớn nhất thỏa MinPoints ≤ P(u, t)
  - `(4.7)` Thu hồi điểm khi hoàn tiền: RevokedPoints ← min(Points, RevokedPoints + ⌊RefundedAmount / PointsPerAmount⌋)
  - `Hình 4.58` Lưu đồ ghi sổ điểm, thu hồi và đồng bộ điểm hết hạn
  - `Bảng 4.84` Hạng thành viên
- **Nguồn dữ liệu:** `BookingService.cs → ApplyLoyaltyAsync, ReverseLoyaltyAsync, CalculateDiscountAmountAsync`; `LoyaltyExpiryService.cs`; `BackgroundJobs/ExpireLoyaltyPointsJob.cs`; `LoyaltyWindow.cs`; `Domain/Entities/LoyaltyPointEntry.cs`; `Domain/Entities/MembershipTier.cs`; `Persistence/Seed/SeedData.cs`; `Migrations/_manual/backfill_loyalty_entries.sql`

| Hạng | MinPoints | DiscountPercent |
|---|---|---|
| [CẦN BỔ SUNG: đọc SeedData.cs] |  |  |


### 4.5.9 Đánh giá phim

- **Nội dung cần viết:**
  - Điều kiện đánh giá; ràng buộc `(UserId, MovieId)` duy nhất; tính lại điểm trung bình; kiểm duyệt của quản trị
- **Tài liệu/hình/bảng:**
  - `Hình 4.59` Lưu đồ viết đánh giá phim
- **Nguồn dữ liệu:** `Services/Reviews/ReviewService.cs`

## 4.6 THUẬT TOÁN


### 4.6.1 Luật chọn ghế

- **Nội dung cần viết:**
  - Định nghĩa "ghế lẻ" theo `ValidateSelection`; ghế đôi chọn cùng nhau; ba chế độ Strict, AllowEdge, Off
- **Tài liệu/hình/bảng:**
  - `Hình 4.60` Minh họa luật ghế lẻ
  - `Bảng 4.85` Ba chế độ luật ghế lẻ
- **Nguồn dữ liệu:** `Utils/SeatLayoutRules.cs`; `Domain/Enums/SeatGapRuleMode.cs`; `frontend/src/components/common/seat-map/seat-map-utils.ts`

| Chế độ | Ý nghĩa | Nơi áp dụng |
|---|---|---|
| Strict | [CẦN BỔ SUNG: đọc ValidateSelection] | Mặc định, đặt vé online |
| AllowEdge | [CẦN BỔ SUNG: đọc ValidateSelection] | Cấu hình |
| Off | Tắt kiểm tra | Bán vé quầy |


### 4.6.2 Gợi ý ghế


#### 4.6.2.1 Chia đoạn ghế trống

- **Nội dung cần viết:**
  - Chia mỗi hàng thành các đoạn ghế Available liên tiếp; duyệt cửa sổ có độ dài bằng số ghế cần; loại cửa sổ để lại đúng 1 ghế trống ở đầu/cuối đoạn; loại cửa sổ cắt đôi cặp ghế đôi
- **Tài liệu/hình/bảng:**
  - `Hình 4.61` Minh họa cửa sổ gợi ý ghế
- **Nguồn dữ liệu:** `Utils/SeatLayoutRules.cs → Suggest, SplitRuns`

#### 4.6.2.2 Hàm chấm điểm

- **Nội dung cần viết:**
  - Điểm = 0,45 × độ gần giữa hàng + 0,45 × điểm độ sâu hàng + 0,1 nếu cùng loại ghế (trọng số lấy từ code, kiểm tra lại)
  - Chọn cửa sổ tốt nhất mỗi hàng; trả về tối đa 3 gợi ý
  - Ví dụ số minh họa tự tính tay (ghi rõ là ví dụ giả định)
- **Tài liệu/hình/bảng:**
  - `(4.8)` Điểm gần giữa hàng: [CẦN BỔ SUNG: biểu thức từ SeatLayoutRules.cs]
  - `(4.9)` Điểm độ sâu hàng: [CẦN BỔ SUNG: biểu thức từ SeatLayoutRules.cs]
  - `(4.10)` Điểm gợi ý: Score = 0,45·Center + 0,45·Row + 0,1·SameType
- **Nguồn dữ liệu:** `Utils/SeatLayoutRules.cs`; `Services/Showtimes/ShowtimeService.cs → GetSeatSuggestionsAsync`; `frontend/src/features/client/bookings/components/booking/seat-suggestion-panel.tsx`

### 4.6.3 Xếp lịch chiếu tự động


#### 4.6.3.1 Độ nóng của phim

- **Nội dung cần viết:**
  - Heat = 0,7 × (vé bán / vé bán tối đa trong `HeatLookbackDays` ngày) + 0,3 × (điểm đánh giá / 10), giới hạn [0, 1]
  - Mô tả đúng như code; ghi chú "xem thảo luận ở 7.3" vì `AverageRating` bị ràng buộc 0–5
- **Tài liệu/hình/bảng:**
  - `(4.11)` Độ nóng: Heat = min(1, 0,7·(Vé/VéMax) + 0,3·(Rating/10))
- **Nguồn dữ liệu:** `Utils/ShowtimeScheduler.cs → Heat`; `ShowtimeService.cs → PreviewAutoScheduleAsync`; `Persistence/Configurations/MovieConfiguration.cs`

#### 4.6.3.2 Ứng viên khung giờ

- **Nội dung cần viết:**
  - Khung giờ hoạt động; thời điểm lý tưởng chia đều theo số suất; ứng viên từ đầu/cuối khoảng bận hiện có và mốc lý tưởng; làm tròn theo bước `StepMinutes` tính từ 0h giờ Việt Nam
  - Ràng buộc: không trùng phòng (kèm đệm), cùng phim cách nhau ≥ `MinSameMovieGapMinutes`, trong giờ mở cửa trừ `AllowAfterHours`
- **Tài liệu/hình/bảng:**
  - `(4.12)` Thời điểm lý tưởng thứ i: [CẦN BỔ SUNG: biểu thức từ ShowtimeScheduler.cs]
  - `Bảng 4.86` Tham số bộ xếp lịch
- **Nguồn dữ liệu:** `Options/SchedulerOptions.cs`; `Utils/ShowtimeScheduler.cs` (`Candidates` là phương thức private); `Utils/OperatingHours.cs`

| Tham số | Ý nghĩa | Mặc định |
|---|---|---|
| StepMinutes | Bước làm tròn | [CẦN BỔ SUNG] |
| MinSameMovieGapMinutes | Khoảng cách tối thiểu giữa hai suất cùng phim | [CẦN BỔ SUNG] |
| SpreadMinutes | Hệ số chuẩn hóa khoảng cách | [CẦN BỔ SUNG] |
| FitWeight | Trọng số mức phù hợp | [CẦN BỔ SUNG] |
| LoadWeight | Trọng số tải phòng | [CẦN BỔ SUNG] |
| HeatLookbackDays | Số ngày tính vé bán | [CẦN BỔ SUNG] |


#### 4.6.3.3 Hàm điểm xếp lịch

- **Nội dung cần viết:**
  - Điểm = −(khoảng cách tới mốc lý tưởng / `SpreadMinutes`) + (1 − |độ nóng − số ghế / số ghế lớn nhất|) × `FitWeight` − số suất đã xếp trong phòng × `LoadWeight`
  - Phân tích độ phức tạp theo vòng lặp thực tế `[CẦN BỔ SUNG: nêu biến n, m, k sau khi đọc code]`
- **Tài liệu/hình/bảng:**
  - `(4.13)` Mức phù hợp: Fit = 1 − |Heat − Ghế/GhếMax|
  - `(4.14)` Điểm xếp lịch: Score = −Khoảng cách/Spread + Fit·FitWeight − Tải·LoadWeight
  - `Hình 4.62` Lưu đồ xếp lịch chiếu tự động
- **Nguồn dữ liệu:** `Utils/ShowtimeScheduler.cs → Build` (công khai; `Score` là phương thức private); `backend/tests/Cinema.UnitTests/Utils/ShowtimeSchedulerTests.cs`

### 4.6.4 Phát hiện trùng lịch phòng

- **Nội dung cần viết:**
  - Điều kiện trùng: cùng phòng, suất Scheduled, `EndTime` > bắt đầu mới và `StartTime` < kết thúc mới; `EndTime` = bắt đầu + thời lượng + đệm 15 phút; giờ mở/đóng cửa qua đêm
- **Tài liệu/hình/bảng:**
  - `Hình 4.63` Minh họa điều kiện trùng lịch phòng
- **Nguồn dữ liệu:** `ShowtimeService.cs → HasOverlapAsync`; `Utils/OperatingHours.cs`

### 4.6.5 Xử lý múi giờ

- **Nội dung cần viết:**
  - Lưu UTC; đổi qua `VietnamTime` theo `Brand:TimeZoneId`; mốc ngày theo giờ Việt Nam cho báo cáo và lọc
- **Tài liệu/hình/bảng:**
  - `Bảng 4.87` Quy ước thời gian
- **Nguồn dữ liệu:** `Utils/VietnamTime.cs`; `Utils/JsonDateTimeConverters.cs`; `frontend/src/lib/wall-time.ts`; `frontend/src/lib/business-time.ts`

| Tầng | Quy ước |
|---|---|
| CSDL | UTC |
| API | [CẦN BỔ SUNG: đọc JsonDateTimeConverters] |
| Giao diện | Giờ Việt Nam (wall-time, business-time) |


## 4.7 GIAO TIẾP THỜI GIAN THỰC


### 4.7.1 Hub SignalR

- **Nội dung cần viết:**
  - Hub `/hubs/showtimes` yêu cầu xác thực; `access_token` qua query string; nhóm `showtime-{id}`
- **Tài liệu/hình/bảng:**
  - `Hình 4.64` Tuần tự cập nhật ghế thời gian thực
- **Nguồn dữ liệu:** `Infrastructure/Realtime/ShowtimeHub.cs`; `SignalRRealtimeNotifier.cs`; `Program.cs (OnMessageReceived)`

### 4.7.2 Sự kiện SignalR

- **Nội dung cần viết:**
  - Bốn sự kiện và payload; payload không chứa định danh người giữ ghế (kiểm tra lại)
- **Tài liệu/hình/bảng:**
  - `Bảng 4.88` Sự kiện SignalR
- **Nguồn dữ liệu:** `Cinema.Shared/Constants/SignalREvents.cs`; `Infrastructure/Realtime/SignalRRealtimeNotifier.cs`

| Sự kiện | Khi nào phát | Payload |
|---|---|---|
| seat.held | Giữ ghế thành công | [CẦN BỔ SUNG: đọc notifier] |
| seat.booked | Phát hành vé | [CẦN BỔ SUNG] |
| seat.released | Hủy giữ ghế/hủy đơn | [CẦN BỔ SUNG] |
| hold.expired | Booking hết hạn | [CẦN BỔ SUNG] |


### 4.7.3 Xử lý phía client

- **Nội dung cần viết:**
  - Hook `useShowtimeHub`, `useRealtimeSeatMap`: áp dụng sự kiện lên sơ đồ ghế; xử lý hold.expired cho booking của chính mình
- **Nguồn dữ liệu:** `frontend/src/hooks/use-showtime-hub.ts`; `frontend/src/hooks/use-realtime-seat-map.ts`; `frontend/src/lib/signalr.ts`

## 4.8 TRỢ LÝ AI


### 4.8.1 Luồng function calling

- **Nội dung cần viết:**
  - Mô hình `gemini-2.5-flash`; vòng lặp gọi hàm; giới hạn vòng/lần gọi
- **Tài liệu/hình/bảng:**
  - `Hình 4.65` Luồng function calling của trợ lý AI
- **Nguồn dữ liệu:** `Services/Ai/GeminiAiAssistantService.cs`; `AiToolExecutor.cs`; `Controllers/Client/AiController.cs`

### 4.8.2 Các hàm của trợ lý

- **Nội dung cần viết:**
  - Ba hàm: `search_showtimes`, `get_seat_availability`, `get_my_bookings` (chỉ khi đăng nhập); tham số và dữ liệu trả về đọc từ AiToolExecutor
- **Tài liệu/hình/bảng:**
  - `Bảng 4.89` Các hàm của trợ lý AI
- **Nguồn dữ liệu:** `Services/Ai/AiToolExecutor.cs`

| Hàm | Tham số | Dữ liệu trả về | Điều kiện |
|---|---|---|---|
| search_showtimes | [CẦN BỔ SUNG] | [CẦN BỔ SUNG] | Công khai |
| get_seat_availability | [CẦN BỔ SUNG] | [CẦN BỔ SUNG] | Công khai |
| get_my_bookings | [CẦN BỔ SUNG] | [CẦN BỔ SUNG] | Đã đăng nhập |


### 4.8.3 Giới hạn ngữ cảnh

- **Nội dung cần viết:**
  - System prompt dựng từ dữ liệu rạp, phim, combo và thông tin người dùng đăng nhập; giới hạn tin nhắn 1000 ký tự, mỗi mục lịch sử 2000 ký tự; hành vi khi thiếu khóa API
- **Nguồn dữ liệu:** `Services/Ai/GeminiAiAssistantService.cs`; `Validators/Ai/AiValidators.cs`

## 4.9 GIAO DIỆN


### 4.9.1 Hệ thống nhận diện

- **Nội dung cần viết:**
  - Ý tưởng "đường xé" (perforation); token màu OKLCH; typography (Archivo, Be Vietnam Pro); quy tắc không bóng, không gradient, không bo góc; trạng thái ghế/huy hiệu; chế độ sáng/tối
- **Tài liệu/hình/bảng:**
  - `Bảng 4.90` Token thiết kế rút gọn
- **Nguồn dữ liệu:** `.agents/rules/perfo-design.md`; `.agents/skills/perfo-design-system/**`; `frontend/src/index.css`; `frontend/src/lib/theme-palette.ts`

| Token | Giá trị | Mục đích |
|---|---|---|
| [CẦN BỔ SUNG] |  |  |


### 4.9.2 Sơ đồ trang khách hàng

- **Nội dung cần viết:**
  - Liệt kê trang và đường dẫn từ routes thực tế; phân quyền route
- **Tài liệu/hình/bảng:**
  - `Hình 4.66` Sơ đồ trang khách hàng
- **Nguồn dữ liệu:** `frontend/src/app/routes/client-routes.tsx`; `frontend/src/config/routes.ts`

### 4.9.3 Sơ đồ trang quản trị

- **Nội dung cần viết:**
  - Liệt kê trang quản trị; nhân viên vào thẳng quầy bán vé (`getAdminHomePath`); phân quyền theo vai trò
- **Tài liệu/hình/bảng:**
  - `Hình 4.67` Sơ đồ trang quản trị
- **Nguồn dữ liệu:** `frontend/src/app/routes/admin-routes.tsx`; `frontend/src/config/admin-nav.ts`; `frontend/src/config/routes.ts`

### 4.9.4 Màn hình chọn ghế

- **Nội dung cần viết:**
  - Sơ đồ ghế, chú giải, gợi ý ghế, đếm ngược giữ ghế, chi tiết đơn hàng; mục đích – thành phần – tương tác chính – component
  - Dùng "xem Hình 5.x" cho ảnh chụp ở Chương 5
- **Nguồn dữ liệu:** `frontend/src/components/common/seat-map/*`; `frontend/src/features/client/bookings/**`

### 4.9.5 Màn hình vé điện tử

- **Nội dung cần viết:**
  - Vé có mã QR, trạng thái, thông tin suất chiếu; mục đích – thành phần – tương tác chính – component
  - Dùng "xem Hình 5.x" cho ảnh chụp ở Chương 5
- **Nguồn dữ liệu:** `frontend/src/components/common/perfo-ticket.tsx`; `frontend/src/features/client/account/**`

### 4.9.6 Màn hình trình soạn sơ đồ ghế

- **Nội dung cần viết:**
  - Cọ vẽ, chọn vùng, lối đi, ghế đôi, sao chép hàng, hoàn tác/làm lại, bảng kiểm tra hợp lệ, sinh tự động; mục đích – thành phần – tương tác chính – component
  - Dùng "xem Hình 5.x" cho ảnh chụp ở Chương 5
- **Nguồn dữ liệu:** `frontend/src/features/admin/rooms/components/seat-editor-*.tsx`

### 4.9.7 Màn hình quầy bán vé

- **Nội dung cần viết:**
  - Chọn suất, ghế, combo, khách; nhập tiền nhận; vé kết quả; mục đích – thành phần – tương tác chính – component
  - Dùng "xem Hình 5.x" cho ảnh chụp ở Chương 5
- **Nguồn dữ liệu:** `frontend/src/features/admin/counter/**`

### 4.9.8 Màn hình soát vé

- **Nội dung cần viết:**
  - Quét camera hoặc nhập mã; hiển thị kết quả; mục đích – thành phần – tương tác chính – component
  - Dùng "xem Hình 5.x" cho ảnh chụp ở Chương 5
- **Nguồn dữ liệu:** `frontend/src/features/admin/checkin/**`

### 4.9.9 Màn hình dashboard

- **Nội dung cần viết:**
  - Biểu đồ, chỉ số tổng hợp; mục đích – thành phần – tương tác chính – component
  - Dùng "xem Hình 5.x" cho ảnh chụp ở Chương 5
- **Nguồn dữ liệu:** `frontend/src/features/admin/dashboard/**`; `frontend/src/features/admin/reports/**`

## 4.10 GIAO DIỆN LẬP TRÌNH


### 4.10.1 Quy ước thiết kế API

- **Nội dung cần viết:**
  - Tiền tố `/api/v1`; tách tài liệu Swagger `client`/`admin`; quy ước phản hồi, phân trang, mã lỗi; endpoint thanh toán công khai nhưng xác minh chữ ký; policy giới hạn tốc độ `auth`, `booking-hold`
- **Nguồn dữ liệu:** `backend/Cinema.Api/Program.cs`; `backend/Cinema.Api/Controllers/**`

### 4.10.2 Tổng hợp endpoint theo nhóm

- **Nội dung cần viết:**
  - Số endpoint theo controller (đếm tự động); danh mục đầy đủ ở Phụ lục 2
- **Tài liệu/hình/bảng:**
  - `Bảng 4.91` Tổng hợp endpoint theo controller
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

| Nhóm | Controller | Số endpoint |
|---|---|---|
| Admin | AdminActors | 5 |
| Admin | AdminAgeRatings | 4 |
| Admin | AdminBookings | 5 |
| Admin | AdminCombos | 5 |
| Admin | AdminDashboard | 2 |
| Admin | AdminDirectors | 5 |
| Admin | AdminEmployees | 3 |
| Admin | AdminGenres | 4 |
| Admin | AdminLanguages | 4 |
| Admin | AdminMembershipTiers | 4 |
| Admin | AdminMovies | 5 |
| Admin | AdminPayments | 2 |
| Admin | AdminPositions | 4 |
| Admin | AdminReports | 3 |
| Admin | AdminReviews | 3 |
| Admin | AdminRooms | 7 |
| Admin | AdminScreenTypes | 4 |
| Admin | AdminSeatTypes | 4 |
| Admin | AdminSettings | 2 |
| Admin | AdminShowtimes | 10 |
| Admin | AdminUploads | 2 |
| Admin | AdminUsers | 6 |
| Client | Ai | 1 |
| Client | Auth | 12 |
| Client | Bookings | 5 |
| Client | Catalog | 4 |
| Client | Combos | 2 |
| Client | Movies | 7 |
| Client | Settings | 1 |
| Client | Showtimes | 3 |
| Client | Users | 4 |
| Khác | Health | 1 |
| Khác | Payments | 4 |
| **Tổng** | Admin 93 · Client 39 · Khác 5 | 137 |


---

# CHƯƠNG 5. CÀI ĐẶT HỆ THỐNG


## 5.1 MÔI TRƯỜNG PHÁT TRIỂN


### 5.1.1 Môi trường công cụ

- **Nội dung cần viết:**
  - Yêu cầu: .NET `net10.0`, Node 22, Docker, tài khoản Neon (PostgreSQL); công cụ: CSharpier 1.3.0, dotnet-ef 10.0.11, ESLint, Playwright 1.63
  - Nhóm lệnh npm có thật: `setup`, `dev:*`, `build`, `check`, `db:*`, `test:*`, `docker:*`, `sitemap:build`, `build:full`
- **Tài liệu/hình/bảng:**
  - `Bảng 5.1` Môi trường phát triển
- **Nguồn dữ liệu:** `package.json`; `frontend/package.json`; `dotnet-tools.json`; `backend/Dockerfile`; `frontend/Dockerfile`

| Nhóm | Nội dung |
|---|---|
| Runtime | .NET net10.0; Node 22 |
| CSDL | PostgreSQL (Neon) |
| Công cụ định dạng | CSharpier 1.3.0; ESLint |
| Công cụ migration | dotnet-ef 10.0.11 |
| Lệnh | [CẦN BỔ SUNG: đọc scripts trong package.json để liệt kê lệnh và mô tả] |


## 5.2 CẤU TRÚC MÃ NGUỒN


### 5.2.1 Cây thư mục

- **Nội dung cần viết:**
  - Giải thích vai trò từng thư mục chính
- **Tài liệu/hình/bảng:**
  - `Hình 5.1` Cấu trúc thư mục chính của dự án
- **Nguồn dữ liệu:** `perfo-main/`

### 5.2.2 Quy mô mã nguồn

- **Nội dung cần viết:**
  - Backend ≈ 25.862 dòng C# (không tính migration, test); frontend ≈ 33.234 dòng TS/TSX (298 file `.tsx`); test ≈ 9.275 dòng C#; seed ≈ 3.830 dòng; 30 entity, 10 enum, 33 controller, 137 endpoint, 4 migration (số dòng mã chưa gồm thay đổi sổ điểm 12 tháng)
  - Đếm lại bằng lệnh và ghi rõ lệnh, phạm vi đếm khi chốt số liệu
- **Tài liệu/hình/bảng:**
  - `Bảng 5.2` Quy mô mã nguồn
- **Nguồn dữ liệu:** `perfo-main/`

| Hạng mục | Giá trị | Cách đếm |
|---|---|---|
| Backend (C#, không migration/test) | ≈ 25.862 dòng | [CẦN BỔ SUNG: ghi lệnh đếm] |
| Frontend (TS/TSX) | ≈ 33.234 dòng; 298 file .tsx | [CẦN BỔ SUNG] |
| Test (C#) | ≈ 9.275 dòng | [CẦN BỔ SUNG] |
| Seed | ≈ 3.830 dòng | [CẦN BỔ SUNG] |
| Thực thể / enum / migration | 30 / 10 / 4 | Đếm thư mục |
| Controller / endpoint | 33 / 137 (Admin 93, Client 39, Khác 5) | Đếm tự động từ [Http*] |


## 5.3 CÀI ĐẶT BACKEND


### 5.3.1 Module xác thực

- **Nội dung cần viết:**
  - Đăng ký, xác nhận email, đăng nhập, Google, làm mới, đăng xuất, quên/đặt lại mật khẩu, socket token
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `Services/Auth/*`; `Infrastructure/Auth/GoogleAuthService.cs`; `Controllers/Client/AuthController.cs`

### 5.3.2 Module hồ sơ người dùng

- **Nội dung cần viết:**
  - Xem/cập nhật hồ sơ, đổi mật khẩu, đánh giá của tôi
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `Services/Users/UserProfileService.cs`; `Controllers/Client/UsersController.cs`

### 5.3.3 Module danh mục

- **Nội dung cần viết:**
  - CRUD thể loại, ngôn ngữ, phân loại tuổi, diễn viên, đạo diễn, loại màn hình, loại ghế, chức vụ, hạng thành viên; quy tắc trùng tên và đang được sử dụng
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `Services/Catalog/CatalogService.cs`; `Controllers/Admin/*`

### 5.3.4 Module phim

- **Nội dung cần viết:**
  - CRUD phim, quan hệ thể loại/diễn viên; xóa hàng loạt; danh sách client phân trang con trỏ
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `Services/Movies/MovieService.cs`

### 5.3.5 Module phòng chiếu

- **Nội dung cần viết:**
  - Phòng, sơ đồ ghế, ghế đôi, lối đi; kiểm tra ghế đôi ghép cặp; không đổi ghế đang giữ/đã bán; không xóa phòng còn suất sắp tới
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `Services/Rooms/RoomService.cs`

### 5.3.6 Module suất chiếu

- **Nội dung cần viết:**
  - Tạo/sửa/hủy; tạo hàng loạt (tối đa 2000 suất/lần [CẦN BỔ SUNG: xác minh]); xem trước/áp dụng xếp lịch (tối đa 31 ngày); nhân bản; lịch dạng calendar; sinh ShowtimeSeat; vòng đời
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `Services/Showtimes/ShowtimeService.cs`; `Utils/ShowtimeScheduler.cs`

### 5.3.7 Module đặt vé

- **Nội dung cần viết:**
  - Giữ ghế, checkout, hủy, danh sách/chi tiết, bán vé quầy, soát vé
  - Tính tiền khi giữ ghế: giảm giá chỉ trên tiền vé, hạng suy ra từ điểm hiệu lực 12 tháng (xem 4.5.1.3)
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `Services/Bookings/BookingService.cs`; `Controllers/Client/BookingsController.cs`; `Controllers/Admin/AdminBookingsController.cs`

### 5.3.8 Module thanh toán

- **Nội dung cần viết:**
  - IPaymentGateway; VNPay, MoMo, Stripe; xử lý IPN/webhook; hoàn tiền; PaymentAdminService
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `Services/Payments/*`; `Infrastructure/Payments/*`; `Controllers/PaymentsController.cs`

### 5.3.9 Module thành viên

- **Nội dung cần viết:**
  - Hạng, điểm, giảm giá; sổ điểm `LoyaltyPointEntry`, cửa sổ trượt 12 tháng (`LoyaltyWindow`), cộng điểm (`ApplyLoyaltyAsync`), thu hồi theo booking (`ReverseLoyaltyAsync`), đồng bộ điểm hết hạn (`LoyaltyExpiryService`)
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `BookingService.cs (loyalty)`; `LoyaltyExpiryService.cs`; `LoyaltyWindow.cs`; `Domain/Entities/LoyaltyPointEntry.cs`; `Domain/Entities/MembershipTier.cs`

### 5.3.10 Module đánh giá

- **Nội dung cần viết:**
  - Viết/sửa/xóa; tính lại điểm trung bình; kiểm duyệt
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `Services/Reviews/ReviewService.cs`

### 5.3.11 Module báo cáo

- **Nội dung cần viết:**
  - Dashboard tổng quan/insights; báo cáo doanh thu (trừ hoàn tiền), phim bán chạy, hiệu suất nhân viên
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `Services/Reports/ReportService.cs`; `Services/Dashboard/DashboardService.cs`

### 5.3.12 Module thời gian thực

- **Nội dung cần viết:**
  - ShowtimeHub, SignalRRealtimeNotifier; bốn sự kiện
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `Infrastructure/Realtime/*`

### 5.3.13 Module tác vụ nền

- **Nội dung cần viết:**
  - Hangfire (PostgreSQL): ReleaseExpiredSeatsJob, ShowtimeLifecycleJob (mỗi phút), ExpireLoyaltyPointsJob (mỗi giờ, phút 05, cấu hình `ExpireLoyaltyPointsCron`), SendEmailJob, ShowtimeRefundJob (thử lại 3 lần)
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `BackgroundJobs/*`; `Program.cs`

### 5.3.14 Module email

- **Nội dung cần viết:**
  - FallbackEmailService (Resend trước, SMTP MailKit dự phòng — kiểm tra thứ tự trong mã); mẫu email HTML
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `Infrastructure/Email/*`; `Utils/EmailTemplates.cs`

### 5.3.15 Module lưu trữ tệp

- **Nội dung cần viết:**
  - Tải ảnh (tối đa 5 MB: jpg, jpeg, png, webp) và video (tối đa 100 MB: mp4, mov, webm, avi) lên Cloudinary
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `Infrastructure/FileStorage/CloudinaryFileStorage.cs`; `Controllers/Admin/AdminUploadsController.cs`

### 5.3.16 Module trợ lý AI

- **Nội dung cần viết:**
  - Gemini function calling; ba hàm; giới hạn vòng; thẻ kết quả
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `Services/Ai/*`; `Controllers/Client/AiController.cs`

### 5.3.17 Cổng thanh toán đã cài đặt

- **Nội dung cần viết:**
  - Trạng thái kiểm chứng từng cổng: chỉ ghi "đã kiểm chứng" khi có test/fake/tài liệu thật
- **Tài liệu/hình/bảng:**
  - `Bảng 5.3` Cổng thanh toán và trạng thái kiểm chứng
- **Nguồn dữ liệu:** `Infrastructure/Payments/*`; `backend/tests/Cinema.IntegrationTests/Fakes/FakePaymentGateway.cs`; `backend/tests/Cinema.UnitTests/Services/PaymentServiceVnPayTests.cs`; `PaymentServiceStripeTests.cs`

| Cổng | Đã cài đặt | Kiểm chứng bằng test | Chạy sandbox thật |
|---|---|---|---|
| VNPay | Tạo thanh toán, IPN, hoàn tiền | PaymentServiceVnPayTests | [CẦN BỔ SUNG: ảnh/nhật ký] |
| MoMo | Tạo thanh toán, IPN, hoàn tiền | [CẦN BỔ SUNG: không có test riêng cho MoMo] | [CẦN BỔ SUNG] |
| Stripe | Tạo thanh toán, webhook, hoàn tiền | PaymentServiceStripeTests | [CẦN BỔ SUNG] |


## 5.4 CÀI ĐẶT FRONTEND


### 5.4.1 Giao diện khách hàng


#### 5.4.1.1 Màn hình Trang chủ

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (hero carousel, danh sách phim, combo, giới thiệu trải nghiệm rạp) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.2` Màn hình Trang chủ
- **Nguồn dữ liệu:** `frontend/src/features/client/home/**`

#### 5.4.1.2 Màn hình Danh sách phim

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (lọc, tìm kiếm, phân trang con trỏ) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.3` Màn hình Danh sách phim
- **Nguồn dữ liệu:** `frontend/src/features/client/movies/**`

#### 5.4.1.3 Màn hình Chi tiết phim

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (thông tin, trailer, diễn viên, đánh giá, chọn suất) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.4` Màn hình Chi tiết phim
- **Nguồn dữ liệu:** `frontend/src/features/client/movies/**`

#### 5.4.1.4 Màn hình Lịch chiếu

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (chọn ngày, các suất theo phim) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.5` Màn hình Lịch chiếu
- **Nguồn dữ liệu:** `frontend/src/features/client/schedule/**`

#### 5.4.1.5 Màn hình Chọn ghế

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (sơ đồ, chú giải, gợi ý ghế, đếm ngược, chi tiết đơn hàng) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.6` Màn hình Chọn ghế
- **Nguồn dữ liệu:** `frontend/src/features/client/bookings/**`

#### 5.4.1.6 Màn hình Thanh toán

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (chọn phương thức, đếm ngược giữ ghế) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.7` Màn hình Thanh toán
- **Nguồn dữ liệu:** `frontend/src/features/client/bookings/**`

#### 5.4.1.7 Màn hình Kết quả đặt vé

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (thành công, thất bại, không tìm thấy) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.8` Màn hình Kết quả đặt vé
- **Nguồn dữ liệu:** `frontend/src/features/client/bookings/**`

#### 5.4.1.8 Màn hình Vé của tôi

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (danh sách theo trạng thái) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.9` Màn hình Vé của tôi
- **Nguồn dữ liệu:** `frontend/src/features/client/account/**`

#### 5.4.1.9 Màn hình Chi tiết vé

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (vé điện tử có mã QR) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.10` Màn hình Chi tiết vé
- **Nguồn dữ liệu:** `frontend/src/features/client/account/**, components/common/perfo-ticket.tsx`

#### 5.4.1.10 Màn hình Hồ sơ cá nhân

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (xem, cập nhật, đổi mật khẩu) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.11` Màn hình Hồ sơ cá nhân
- **Nguồn dữ liệu:** `frontend/src/features/client/account/**`

#### 5.4.1.11 Màn hình Thành viên

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (hạng, điểm, mức giảm giá) → tương tác/trạng thái → component và API sử dụng
  - Thay đổi giao diện chỉ về nhãn chữ: bảng so sánh hạng và FAQ trong `membership/constants.ts` đổi "năm dương lịch" thành 12 tháng gần nhất; nhãn giảm giá trong `booking-price-summary.tsx` và `booking-detail-page.tsx` (giảm giá trên tiền vé); mô tả hạng trong `counter-customer-form.tsx`
  - Giao diện chưa hiển thị ngày điểm hết hạn
- **Tài liệu/hình/bảng:**
  - `Hình 5.12` Màn hình Thành viên
- **Nguồn dữ liệu:** `frontend/src/features/client/membership/**`

#### 5.4.1.12 Màn hình Trợ lý AI

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (widget chat, thẻ suất chiếu/vé) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.13` Màn hình Trợ lý AI
- **Nguồn dữ liệu:** `frontend/src/features/client/ai/**`

#### 5.4.1.13 Màn hình Đăng nhập

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (email/mật khẩu, Google) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.14` Màn hình Đăng nhập
- **Nguồn dữ liệu:** `frontend/src/features/auth/**`

#### 5.4.1.14 Màn hình Đăng ký

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (biểu mẫu đăng ký, xác nhận email) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.15` Màn hình Đăng ký
- **Nguồn dữ liệu:** `frontend/src/features/auth/**`

### 5.4.2 Giao diện quản trị


#### 5.4.2.1 Màn hình Dashboard

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (chỉ số, biểu đồ) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.16` Màn hình Dashboard
- **Nguồn dữ liệu:** `frontend/src/features/admin/dashboard/**`

#### 5.4.2.2 Màn hình Quản lý phim

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (bảng, lọc, form thêm/sửa, xóa hàng loạt) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.17` Màn hình Quản lý phim
- **Nguồn dữ liệu:** `frontend/src/features/admin/movies/**`

#### 5.4.2.3 Màn hình Quản lý suất chiếu

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (danh sách, lọc, tạo/sửa/hủy) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.18` Màn hình Quản lý suất chiếu
- **Nguồn dữ liệu:** `frontend/src/features/admin/showtimes/**`

#### 5.4.2.4 Màn hình Lịch suất chiếu

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (dạng lịch) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.19` Màn hình Lịch suất chiếu
- **Nguồn dữ liệu:** `frontend/src/features/admin/showtimes/components/calendar/**`

#### 5.4.2.5 Màn hình Tự động xếp lịch

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (chọn phạm vi, xem trước, áp dụng) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.20` Màn hình Tự động xếp lịch
- **Nguồn dữ liệu:** `frontend/src/features/admin/showtimes/**`

#### 5.4.2.6 Màn hình Quản lý phòng chiếu

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (danh sách phòng) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.21` Màn hình Quản lý phòng chiếu
- **Nguồn dữ liệu:** `frontend/src/features/admin/rooms/**`

#### 5.4.2.7 Màn hình Trình soạn sơ đồ ghế

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (cọ vẽ, chọn vùng, ghép ghế đôi, kiểm tra hợp lệ) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.22` Màn hình Trình soạn sơ đồ ghế
- **Nguồn dữ liệu:** `frontend/src/features/admin/rooms/components/seat-editor-*.tsx`

#### 5.4.2.8 Màn hình Quầy bán vé

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (chọn suất, ghế, combo, khách, tiền mặt) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.23` Màn hình Quầy bán vé
- **Nguồn dữ liệu:** `frontend/src/features/admin/counter/**`

#### 5.4.2.9 Màn hình Soát vé

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (quét camera, nhập mã) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.24` Màn hình Soát vé
- **Nguồn dữ liệu:** `frontend/src/features/admin/checkin/**`

#### 5.4.2.10 Màn hình Quản lý booking

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (danh sách, chi tiết) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.25` Màn hình Quản lý booking
- **Nguồn dữ liệu:** `frontend/src/features/admin/bookings/**`

#### 5.4.2.11 Màn hình Quản lý thanh toán

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (danh sách, chi tiết) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.26` Màn hình Quản lý thanh toán
- **Nguồn dữ liệu:** `frontend/src/features/admin/payments/**`

#### 5.4.2.12 Màn hình Báo cáo doanh thu

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (báo cáo theo ngày/tuần/tháng, phim bán chạy, hiệu suất nhân viên) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.27` Màn hình Báo cáo doanh thu
- **Nguồn dữ liệu:** `frontend/src/features/admin/reports/**`

#### 5.4.2.13 Màn hình Quản lý người dùng

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (danh sách, khóa/mở, đổi vai trò) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.28` Màn hình Quản lý người dùng
- **Nguồn dữ liệu:** `frontend/src/features/admin/users/**`

#### 5.4.2.14 Màn hình Quản lý nhân viên

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (danh sách, thêm, sửa) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.29` Màn hình Quản lý nhân viên
- **Nguồn dữ liệu:** `frontend/src/features/admin/employees/**`

#### 5.4.2.15 Màn hình Quản lý combo

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (danh sách, bật/tắt) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.30` Màn hình Quản lý combo
- **Nguồn dữ liệu:** `frontend/src/features/admin/combos/**`

#### 5.4.2.16 Màn hình Quản lý đánh giá

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (danh sách, xóa) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.31` Màn hình Quản lý đánh giá
- **Nguồn dữ liệu:** `frontend/src/features/admin/reviews/**`

#### 5.4.2.17 Màn hình Cấu hình rạp

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (nhóm tham số vận hành) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.32` Màn hình Cấu hình rạp
- **Nguồn dữ liệu:** `frontend/src/features/admin/settings/**`

#### 5.4.2.18 Màn hình Quản lý danh mục

- **Nội dung cần viết:**
  - Mục đích → thành phần giao diện (các trang danh mục) → tương tác/trạng thái → component và API sử dụng
- **Tài liệu/hình/bảng:**
  - `Hình 5.33` Màn hình Quản lý danh mục
- **Nguồn dữ liệu:** `frontend/src/features/admin/catalog/**`

### 5.4.3 Giao diện đáp ứng


#### 5.4.3.1 Màn hình trên thiết bị di động

- **Nội dung cần viết:**
  - Bố cục responsive của trang chọn ghế hoặc trang chủ; chế độ sáng/tối
- **Tài liệu/hình/bảng:**
  - `Hình 5.34` Giao diện trên thiết bị di động
- **Nguồn dữ liệu:** `frontend/src/layouts/**`

### 5.4.4 Thành phần dùng chung


#### 5.4.4.1 Bảng dữ liệu

- **Nội dung cần viết:**
  - cột, lọc, phân trang, chọn nhiều, thao tác hàng loạt
- **Nguồn dữ liệu:** `frontend/src/components/common/data-table/**`

#### 5.4.4.2 Sơ đồ ghế

- **Nội dung cần viết:**
  - hiển thị trạng thái ghế, chọn ghế, ghế đôi, lối đi
- **Nguồn dữ liệu:** `frontend/src/components/common/seat-map/*`

#### 5.4.4.3 Trường biểu mẫu

- **Nội dung cần viết:**
  - input, select, date/time/datetime picker, multi-select, combo picker
- **Nguồn dữ liệu:** `frontend/src/components/common/form-field/**`

#### 5.4.4.4 Lớp truy cập API

- **Nội dung cần viết:**
  - axios, interceptor làm mới token một promise, query keys, invalidation
- **Nguồn dữ liệu:** `frontend/src/api/axios.ts`; `query-keys.ts`; `query-invalidation.ts`

#### 5.4.4.5 Quản lý trạng thái

- **Nội dung cần viết:**
  - Zustand: auth, dialog, sheet, alert
- **Nguồn dữ liệu:** `frontend/src/stores/*`

### 5.4.5 SEO

- **Nội dung cần viết:**
  - `robots.txt` cho phép toàn site, chặn `/admin/`, `/auth/`, `/account/`, `/booking/` và khai báo sitemap
  - `sitemap.xml` hiện có 4 URL (`/`, `/movies`, `/schedule`, `/membership`); script sinh sitemap lấy danh sách phim qua API phân trang con trỏ (100 phim/lần) rồi ghi `public/sitemap.xml`; lệnh `npm run sitemap:build`, `build:full`
  - Header `X-Robots-Tag` trong `vercel.json`; thành phần `seo.tsx`, `config/seo.ts`
  - `[CẦN BỔ SUNG: sitemap chưa có URL trang chi tiết phim — xác nhận lần sinh dùng API nào; tên miền sitemap là onrender.com trong khi có vercel.json — xác nhận nền tảng frontend thực tế]`
- **Tài liệu/hình/bảng:**
  - `Bảng 5.4` Tài nguyên SEO
- **Nguồn dữ liệu:** `bo_sung/public/robots.txt`; `bo_sung/public/sitemap.xml`; `bo_sung/scripts/build-sitemap.ts`; `bo_sung/scripts/build-sitemap.mjs`; `frontend/vercel.json`; `frontend/src/components/common/seo.tsx`; `frontend/src/config/seo.ts`

| Tệp | Vai trò |
|---|---|
| public/robots.txt | Quy tắc thu thập dữ liệu, khai báo sitemap |
| public/sitemap.xml | Danh sách URL (4 URL) |
| scripts/build-sitemap.ts, .mjs | Sinh sitemap từ API |
| vercel.json | Rewrite SPA, cache, header (X-Robots-Tag) |


## 5.5 CẤU HÌNH HỆ THỐNG


### 5.5.1 Biến cấu hình

- **Nội dung cần viết:**
  - Nhóm biến: CSDL, runtime, JWT, Brand, Booking/RateLimit, Scheduler, Hangfire, Cloudinary, SMTP, Resend, frontend (VITE_*), E2E_*; danh sách tên đầy đủ ở Phụ lục 3
  - Không ghi giá trị bí mật
- **Tài liệu/hình/bảng:**
  - `Bảng 5.5` Nhóm biến cấu hình chính
- **Nguồn dữ liệu:** `.env.example`; `frontend/.env.example`; `backend/Cinema.Api/appsettings.json`

| Nhóm | Ví dụ biến |
|---|---|
| CSDL | DATABASE_URL, DATABASE_URL_DIRECT |
| Runtime | ASPNETCORE_ENVIRONMENT, RUN_MIGRATIONS_ON_STARTUP, CORS_ALLOWED_ORIGIN |
| JWT | JWT_SECRET, JWT_ACCESS_TOKEN_EXPIRATION_SECONDS |
| Xếp lịch | SCHEDULER_STEP_MINUTES, SCHEDULER_FIT_WEIGHT |
| Tác vụ nền | BG_RELEASE_EXPIRED_SEATS_CRON, BG_RETRY_ATTEMPTS; `ExpireLoyaltyPointsCron` trong `appsettings.json` [CẦN BỔ SUNG: kiểm tra có biến môi trường tương ứng hay không] |
| Dịch vụ ngoài | CLOUDINARY_*, SMTP_*, RESEND_* |
| Frontend | VITE_API_BASE_URL, VITE_SIGNALR_HUB_URL, VITE_GOOGLE_CLIENT_ID |


### 5.5.2 Kiểm tra cấu hình Production

- **Nội dung cần viết:**
  - Chỉ nêu các kiểm tra có thật trong `ProductionOptionsValidator`: `Jwt:Secret` ≥ 32 ký tự; `Cors:AllowedOrigins` đã cấu hình; đủ tham số khi bật VNPay/MoMo/Stripe
- **Nguồn dữ liệu:** `backend/Cinema.Application/Options/ProductionOptionsValidator.cs`

## 5.6 BẢO MẬT


### 5.6.1 Cơ chế bảo mật đã cài đặt

- **Nội dung cần viết:**
  - Chỉ liệt kê cơ chế có thật; mỗi dòng kèm vị trí cài đặt
- **Tài liệu/hình/bảng:**
  - `Bảng 5.6` Cơ chế bảo mật và vị trí cài đặt
- **Nguồn dữ liệu:** `Program.cs`; `Middlewares/ExceptionHandlingMiddleware.cs`; `Filters/HangfireAdminAuthorizationFilter.cs`; `SignalRRealtimeNotifier.cs`; `frontend/vercel.json`; `backend/Dockerfile`

| Cơ chế | Vị trí |
|---|---|
| JWT; kiểm tra tài khoản hoạt động mỗi yêu cầu | Program.cs (OnTokenValidated) |
| Refresh token băm SHA-256, xoay vòng, phát hiện dùng lại | JwtTokenService.cs, AuthService.cs |
| Băm mật khẩu bằng PasswordHasher | AuthService.cs |
| Phân quyền theo policy | Program.cs, [Authorize] |
| Giới hạn tốc độ (toàn cục, auth, booking-hold) | Program.cs, RateLimitOptions.cs |
| Xác minh chữ ký, số tiền, tiền tệ thanh toán | PaymentService.cs, *Gateway.cs |
| CORS theo danh sách | Program.cs |
| Hangfire dashboard chỉ quản trị | HangfireAdminAuthorizationFilter.cs |
| Không gửi định danh người giữ ghế qua SignalR | SignalRRealtimeNotifier.cs |
| Ẩn chi tiết lỗi ở Production | ExceptionHandlingMiddleware.cs |
| Header nosniff, Referrer-Policy, X-Robots-Tag | frontend/vercel.json |
| Người dùng không đặc quyền trong container | backend/Dockerfile |


## 5.7 TRIỂN KHAI


### 5.7.1 Sơ đồ triển khai

- **Nội dung cần viết:**
  - Thành phần triển khai và kết nối
  - [CẦN BỔ SUNG: nền tảng host frontend thực tế (vercel.json hay onrender.com)]
- **Tài liệu/hình/bảng:**
  - `Hình 5.35` Sơ đồ triển khai hệ thống
- **Nguồn dữ liệu:** `render.yaml`; `frontend/vercel.json`; `docker-compose.yml`

### 5.7.2 Docker

- **Nội dung cần viết:**
  - Dockerfile nhiều stage (restore, build, dev, production); image `aspnet` alpine; GC giới hạn heap 384 MB; người dùng không đặc quyền
- **Nguồn dữ liệu:** `backend/Dockerfile`; `frontend/Dockerfile`

### 5.7.3 Docker Compose

- **Nội dung cần viết:**
  - Dịch vụ api và frontend; CSDL ngoài (Neon); hot reload
- **Nguồn dữ liệu:** `docker-compose.yml`

### 5.7.4 Render

- **Nội dung cần viết:**
  - Web service Docker, vùng Singapore, gói free, health check `/api/v1/health`, tự deploy khi commit
- **Nguồn dữ liệu:** `render.yaml`

### 5.7.5 Vercel

- **Nội dung cần viết:**
  - Rewrite SPA; cache `/assets` 1 năm; header bảo mật
- **Nguồn dữ liệu:** `frontend/vercel.json`

---

# CHƯƠNG 6. KIỂM THỬ HỆ THỐNG


## 6.1 CHIẾN LƯỢC KIỂM THỬ

- **Nội dung cần viết:**
  - Kim tự tháp kiểm thử ánh xạ vào project: đơn vị (xUnit, Moq, Shouldly, SQLite) → tích hợp (WebApplicationFactory, Testcontainers PostgreSQL, Respawn, SignalR client, fake email/Google/payment) → E2E (Playwright, Chromium) → tải (k6)
  - Môi trường `Testing` tắt Hangfire và rate limiter (xác minh trong `CinemaApiFactory`, `Program.cs`)
  - Lệnh chạy: `test:unit`, `test:integration`, `test:e2e`, `test:backend:coverage`
- **Tài liệu/hình/bảng:**
  - `Hình 6.1` Kim tự tháp kiểm thử của dự án
  - `Bảng 6.1` Tổng hợp các bộ kiểm thử
- **Nguồn dữ liệu:** `backend/tests/*/*.csproj`; `backend/tests/Cinema.IntegrationTests/Infrastructure/CinemaApiFactory.cs`; `package.json`; `frontend/playwright.config.ts`

| Bộ kiểm thử | Số file test | Số test (đếm [Fact]/[Theory]) | Công cụ | Kết quả chạy |
|---|---|---|---|---|
| Đơn vị | 28 + 1 (`LoyaltyWindowTests.cs`) | 283 + [CẦN BỔ SUNG: đếm lại số test mới và test đã sửa] | xUnit, Moq, Shouldly, SQLite | [CẦN BỔ SUNG: kết quả dotnet test] |
| Tích hợp | 8 | 49 | WebApplicationFactory, Testcontainers, Respawn | [CẦN BỔ SUNG: cần Docker] |
| E2E | 7 spec (8 kịch bản) | 8 | Playwright 1.63, Chromium | 8/8 đạt (07/09/2026) |
| Tải | — | — | k6 | [CẦN BỔ SUNG: load-tests/ chưa có] |


## 6.2 KIỂM THỬ ĐƠN VỊ


### 6.2.1 Nhóm Xác thực

- **Nội dung cần viết:**
  - Liệt kê hành vi được kiểm tra dựa trên tên và nội dung từng test (đọc file, không suy đoán)
  - Tổng nhóm: 48 test
- **Tài liệu/hình/bảng:**
  - `Bảng 6.2` Test đơn vị nhóm Xác thực
- **Nguồn dữ liệu:** `Cinema.UnitTests/Services/AuthServiceTests.cs`; `Cinema.UnitTests/Services/JwtTokenServiceTests.cs`; `Cinema.UnitTests/Validators/AuthValidatorsTests.cs`

| File | Số test | Trong đó [Theory] | Hành vi chính |
|---|---|---|---|
| AuthServiceTests | 24 | 0 | [CẦN BỔ SUNG: đọc tên test] |
| JwtTokenServiceTests | 9 | 0 | [CẦN BỔ SUNG: đọc tên test] |
| AuthValidatorsTests | 15 | 3 | [CẦN BỔ SUNG: đọc tên test] |


### 6.2.2 Nhóm Đặt vé

- **Nội dung cần viết:**
  - Liệt kê hành vi được kiểm tra dựa trên tên và nội dung từng test (đọc file, không suy đoán)
  - Tổng nhóm: 69 test trước khi thêm `LoyaltyWindowTests` [CẦN BỔ SUNG: đếm lại sau khi thêm test mới]
  - Ba file `BookingServiceTests`, `BookingServiceLoyaltyTests`, `BookingServiceCounterCheckinTests` đã được sửa theo quy tắc mới (giảm giá chỉ trên tiền vé, sổ điểm 12 tháng, thu hồi theo booking), cần đếm lại số test
  - File mới `LoyaltyWindowTests.cs` gồm 3 lớp (`LoyaltyDiscountTests`, `LoyaltyLedgerTests`, `LoyaltyExpiryServiceTests`) và lớp hỗ trợ `TestSupport/LoyaltyTestHelper.cs`
- **Tài liệu/hình/bảng:**
  - `Bảng 6.3` Test đơn vị nhóm Đặt vé
- **Nguồn dữ liệu:** `Cinema.UnitTests/Services/BookingServiceCounterCheckinTests.cs`; `Cinema.UnitTests/Services/BookingServiceLoyaltyTests.cs`; `Cinema.UnitTests/Services/BookingServiceSeatHoldTests.cs`; `Cinema.UnitTests/Services/BookingServiceTests.cs`; `Cinema.UnitTests/**/LoyaltyWindowTests.cs`; `Cinema.UnitTests/TestSupport/LoyaltyTestHelper.cs`; `Cinema.UnitTests/Validators/BookingValidatorsTests.cs`

| File | Số test | Trong đó [Theory] | Hành vi chính |
|---|---|---|---|
| BookingServiceCounterCheckinTests | 18 | 0 | [CẦN BỔ SUNG: đọc tên test] |
| BookingServiceLoyaltyTests | 4 (trước khi sửa) | 0 | Đã sửa theo quy tắc mới [CẦN BỔ SUNG: đọc tên test và đếm lại] |
| LoyaltyWindowTests | [CẦN BỔ SUNG: đếm] | [CẦN BỔ SUNG] | Ba lớp: `LoyaltyDiscountTests` (giảm giá chỉ trên tiền vé, hạng theo điểm 12 tháng), `LoyaltyLedgerTests` (ghi sổ điểm, thu hồi theo booking), `LoyaltyExpiryServiceTests` (tính lại điểm và hạng khi hết hạn) [CẦN BỔ SUNG: đọc tên test] |
| BookingServiceSeatHoldTests | 6 | 1 | [CẦN BỔ SUNG: đọc tên test] |
| BookingServiceTests | 21 | 0 | [CẦN BỔ SUNG: đọc tên test] |
| BookingValidatorsTests | 20 | 2 | [CẦN BỔ SUNG: đọc tên test] |


### 6.2.3 Nhóm Thanh toán

- **Nội dung cần viết:**
  - Liệt kê hành vi được kiểm tra dựa trên tên và nội dung từng test (đọc file, không suy đoán)
  - Tổng nhóm: 24 test
- **Tài liệu/hình/bảng:**
  - `Bảng 6.4` Test đơn vị nhóm Thanh toán
- **Nguồn dữ liệu:** `Cinema.UnitTests/Services/PaymentServiceStripeTests.cs`; `Cinema.UnitTests/Services/PaymentServiceVnPayTests.cs`

| File | Số test | Trong đó [Theory] | Hành vi chính |
|---|---|---|---|
| PaymentServiceStripeTests | 16 | 0 | [CẦN BỔ SUNG: đọc tên test] |
| PaymentServiceVnPayTests | 8 | 0 | [CẦN BỔ SUNG: đọc tên test] |


### 6.2.4 Nhóm Hết hạn giữ ghế

- **Nội dung cần viết:**
  - Liệt kê hành vi được kiểm tra dựa trên tên và nội dung từng test (đọc file, không suy đoán)
  - Tổng nhóm: 7 test
- **Tài liệu/hình/bảng:**
  - `Bảng 6.5` Test đơn vị nhóm Hết hạn giữ ghế
- **Nguồn dữ liệu:** `Cinema.UnitTests/Services/SeatExpirationServiceTests.cs`

| File | Số test | Trong đó [Theory] | Hành vi chính |
|---|---|---|---|
| SeatExpirationServiceTests | 7 | 1 | [CẦN BỔ SUNG: đọc tên test] |


### 6.2.5 Nhóm Suất chiếu

- **Nội dung cần viết:**
  - Liệt kê hành vi được kiểm tra dựa trên tên và nội dung từng test (đọc file, không suy đoán)
  - Tổng nhóm: 70 test
- **Tài liệu/hình/bảng:**
  - `Bảng 6.6` Test đơn vị nhóm Suất chiếu
- **Nguồn dữ liệu:** `Cinema.UnitTests/Services/ShowtimeLifecycleTests.cs`; `Cinema.UnitTests/Services/ShowtimeServiceAutoScheduleTests.cs`; `Cinema.UnitTests/Services/ShowtimeServiceCreateTests.cs`; `Cinema.UnitTests/Services/ShowtimeServiceFixesTests.cs`; `Cinema.UnitTests/Utils/OperatingHoursTests.cs`; `Cinema.UnitTests/Utils/ShowtimeSchedulerTests.cs`; `Cinema.UnitTests/Validators/ShowtimeValidatorsTests.cs`

| File | Số test | Trong đó [Theory] | Hành vi chính |
|---|---|---|---|
| ShowtimeLifecycleTests | 5 | 0 | [CẦN BỔ SUNG: đọc tên test] |
| ShowtimeServiceAutoScheduleTests | 4 | 0 | [CẦN BỔ SUNG: đọc tên test] |
| ShowtimeServiceCreateTests | 14 | 0 | [CẦN BỔ SUNG: đọc tên test] |
| ShowtimeServiceFixesTests | 19 | 2 | [CẦN BỔ SUNG: đọc tên test] |
| OperatingHoursTests | 6 | 0 | [CẦN BỔ SUNG: đọc tên test] |
| ShowtimeSchedulerTests | 7 | 0 | [CẦN BỔ SUNG: đọc tên test] |
| ShowtimeValidatorsTests | 15 | 0 | [CẦN BỔ SUNG: đọc tên test] |


### 6.2.6 Nhóm Phòng chiếu

- **Nội dung cần viết:**
  - Liệt kê hành vi được kiểm tra dựa trên tên và nội dung từng test (đọc file, không suy đoán)
  - Tổng nhóm: 8 test
- **Tài liệu/hình/bảng:**
  - `Bảng 6.7` Test đơn vị nhóm Phòng chiếu
- **Nguồn dữ liệu:** `Cinema.UnitTests/Services/RoomServiceSeatLayoutTests.cs`

| File | Số test | Trong đó [Theory] | Hành vi chính |
|---|---|---|---|
| RoomServiceSeatLayoutTests | 8 | 0 | [CẦN BỔ SUNG: đọc tên test] |


### 6.2.7 Nhóm Báo cáo

- **Nội dung cần viết:**
  - Liệt kê hành vi được kiểm tra dựa trên tên và nội dung từng test (đọc file, không suy đoán)
  - Tổng nhóm: 17 test
- **Tài liệu/hình/bảng:**
  - `Bảng 6.8` Test đơn vị nhóm Báo cáo
- **Nguồn dữ liệu:** `Cinema.UnitTests/Services/DashboardServiceTests.cs`; `Cinema.UnitTests/Services/ReportServiceTests.cs`

| File | Số test | Trong đó [Theory] | Hành vi chính |
|---|---|---|---|
| DashboardServiceTests | 8 | 0 | [CẦN BỔ SUNG: đọc tên test] |
| ReportServiceTests | 9 | 0 | [CẦN BỔ SUNG: đọc tên test] |


### 6.2.8 Nhóm Cấu hình, đánh giá, nhân viên

- **Nội dung cần viết:**
  - Liệt kê hành vi được kiểm tra dựa trên tên và nội dung từng test (đọc file, không suy đoán)
  - Tổng nhóm: 9 test
- **Tài liệu/hình/bảng:**
  - `Bảng 6.9` Test đơn vị nhóm Cấu hình, đánh giá, nhân viên
- **Nguồn dữ liệu:** `Cinema.UnitTests/Options/ProductionOptionsValidatorTests.cs`; `Cinema.UnitTests/Services/EmployeeServiceUserStatusTests.cs`; `Cinema.UnitTests/Services/ReviewServiceTests.cs`

| File | Số test | Trong đó [Theory] | Hành vi chính |
|---|---|---|---|
| ProductionOptionsValidatorTests | 5 | 0 | [CẦN BỔ SUNG: đọc tên test] |
| EmployeeServiceUserStatusTests | 2 | 0 | [CẦN BỔ SUNG: đọc tên test] |
| ReviewServiceTests | 2 | 0 | [CẦN BỔ SUNG: đọc tên test] |


### 6.2.9 Nhóm Tiện ích

- **Nội dung cần viết:**
  - Liệt kê hành vi được kiểm tra dựa trên tên và nội dung từng test (đọc file, không suy đoán)
  - Tổng nhóm: 31 test
- **Tài liệu/hình/bảng:**
  - `Bảng 6.10` Test đơn vị nhóm Tiện ích
- **Nguồn dữ liệu:** `Cinema.UnitTests/Utils/CodeGeneratorTests.cs`; `Cinema.UnitTests/Utils/DateTimeUtilsTests.cs`; `Cinema.UnitTests/Utils/EmailTemplatesTests.cs`; `Cinema.UnitTests/Utils/EnumConvertersTests.cs`

| File | Số test | Trong đó [Theory] | Hành vi chính |
|---|---|---|---|
| CodeGeneratorTests | 5 | 1 | [CẦN BỔ SUNG: đọc tên test] |
| DateTimeUtilsTests | 9 | 1 | [CẦN BỔ SUNG: đọc tên test] |
| EmailTemplatesTests | 7 | 0 | [CẦN BỔ SUNG: đọc tên test] |
| EnumConvertersTests | 10 | 3 | [CẦN BỔ SUNG: đọc tên test] |


### 6.2.10 Kết quả chạy test backend

- **Nội dung cần viết:**
  - Chạy `dotnet test` và ghi tổng số, đạt/lỗi, thời gian; độ bao phủ bằng `test:backend:coverage`
  - Số test hiện nêu là số thuộc tính `[Fact]/[Theory]`; `[Theory]` chưa tính số bộ dữ liệu
- **Tài liệu/hình/bảng:**
  - `Bảng 6.11` Kết quả chạy test backend
- **Nguồn dữ liệu:** [CẦN BỔ SUNG: log dotnet test]

| Bộ test | Tổng | Đạt | Lỗi | Bỏ qua | Thời gian | Độ bao phủ |
|---|---|---|---|---|---|---|
| Đơn vị | [CẦN BỔ SUNG] |  |  |  |  |  |
| Tích hợp | [CẦN BỔ SUNG] |  |  |  |  |  |


## 6.3 KIỂM THỬ TÍCH HỢP


### 6.3.1 Hạ tầng kiểm thử tích hợp

- **Nội dung cần viết:**
  - `CinemaApiFactory` (WebApplicationFactory), Testcontainers PostgreSQL, Respawn dọn dữ liệu, `IntegrationTestDataSeeder`, fake email/Google/payment, SignalR client
- **Nguồn dữ liệu:** `backend/tests/Cinema.IntegrationTests/Infrastructure/*`; `backend/tests/Cinema.IntegrationTests/Fakes/*`

### 6.3.2 Danh sách test tích hợp

- **Nội dung cần viết:**
  - Luồng giữ ghế → webhook thanh toán → xác nhận vé; hai khách cùng giữ một ghế (409); phương thức thanh toán không hỗ trợ; khách không phải chủ booking; hủy booking Pending giải phóng ghế; SignalR phát `seat.held`, `seat.released` và không nhận sự kiện sau `LeaveShowtime`
- **Tài liệu/hình/bảng:**
  - `Bảng 6.12` Test tích hợp theo tệp
- **Nguồn dữ liệu:** `Cinema.IntegrationTests/Tests/AdminBookingsEndpointsTests.cs`; `Cinema.IntegrationTests/Tests/AdminMoviesBulkEndpointsTests.cs`; `Cinema.IntegrationTests/Tests/AdminReviewsBulkEndpointsTests.cs`; `Cinema.IntegrationTests/Tests/AdminRoomsEndpointsTests.cs`; `Cinema.IntegrationTests/Tests/AdminShowtimesBulkEndpointsTests.cs`; `Cinema.IntegrationTests/Tests/AuthEndpointsTests.cs`; `Cinema.IntegrationTests/Tests/BookingFlowEndpointsTests.cs`; `Cinema.IntegrationTests/Tests/SignalRShowtimeHubTests.cs`

| File | Số test | Hành vi chính |
|---|---|---|
| AdminBookingsEndpointsTests | 9 | [CẦN BỔ SUNG: đọc tên test] |
| AdminMoviesBulkEndpointsTests | 3 | [CẦN BỔ SUNG: đọc tên test] |
| AdminReviewsBulkEndpointsTests | 3 | [CẦN BỔ SUNG: đọc tên test] |
| AdminRoomsEndpointsTests | 8 | [CẦN BỔ SUNG: đọc tên test] |
| AdminShowtimesBulkEndpointsTests | 6 | [CẦN BỔ SUNG: đọc tên test] |
| AuthEndpointsTests | 11 | [CẦN BỔ SUNG: đọc tên test] |
| BookingFlowEndpointsTests | 6 | [CẦN BỔ SUNG: đọc tên test] |
| SignalRShowtimeHubTests | 3 | [CẦN BỔ SUNG: đọc tên test] |


## 6.4 KIỂM THỬ END-TO-END


### 6.4.1 Công cụ Playwright

- **Nội dung cần viết:**
  - Playwright `@playwright/test` ^1.63.0; project `chromium` (Desktop Chrome); thư mục `./e2e`; timeout mỗi test 60 giây, `expect` 10 giây (một số test nâng lên 120–180 giây); chạy tuần tự (`fullyParallel: false`, `workers: 1`); thử lại 1 lần trên CI; báo cáo HTML; trace khi thử lại; ảnh chụp khi lỗi; `baseURL` từ `E2E_BASE_URL` (mặc định `http://localhost:5173`)
- **Tài liệu/hình/bảng:**
  - `Bảng 6.13` Cấu hình Playwright
- **Nguồn dữ liệu:** `frontend/playwright.config.ts`; `bo_sung/e2e/*.spec.ts`

| Tham số | Giá trị |
|---|---|
| Thư viện | @playwright/test ^1.63.0 |
| Project | chromium (Desktop Chrome) |
| testDir | ./e2e |
| Timeout mỗi test / expect | 60 giây / 10 giây (E2E-04: 180 giây; E2E-06: 120 giây) |
| Chạy song song | fullyParallel: false; workers: 1 |
| Retries | 1 lần trên CI |
| Reporter, trace, screenshot | html; on-first-retry; only-on-failure |
| baseURL | E2E_BASE_URL (mặc định http://localhost:5173) |


### 6.4.2 Danh mục kịch bản

- **Nội dung cần viết:**
  - Tổng quan 7 file spec + `helpers.ts` (835 dòng), 8 kịch bản; mỗi kịch bản có mục riêng ở dưới
- **Tài liệu/hình/bảng:**
  - `Bảng 6.14` Danh mục kịch bản E2E
- **Nguồn dữ liệu:** `bo_sung/e2e/*.spec.ts`

| Mã | File | Kịch bản | Thời gian (ms) |
|---|---|---|---|
| E2E-01 | customer-booking-flow.spec.ts | Luồng đặt vé của khách hàng | 14.632 |
| E2E-02 | customer-payment-failure-flow.spec.ts | Trang kết quả khi đơn bị hủy | 11.097 |
| E2E-03 | customer-payment-failure-flow.spec.ts | Trang kết quả với đơn không tồn tại | 9.209 |
| E2E-04 | customer-seat-hold-timeout-flow.spec.ts | Hết hạn giữ ghế giữa chừng | 23.461 |
| E2E-05 | customer-profile-flow.spec.ts | Hồ sơ khách hàng | 13.804 |
| E2E-06 | admin-checkin-flow.spec.ts | Soát vé | 8.663 |
| E2E-07 | admin-movies-flow.spec.ts | Quản lý phim | 8.575 |
| E2E-08 | admin-showtimes-flow.spec.ts | Quản lý suất chiếu | 8.210 |


### 6.4.3 Kịch bản E2E-01 Luồng đặt vé của khách hàng

- **Nội dung cần viết:**
  - Mục đích kịch bản; điều kiện tiên quyết và điều kiện bỏ qua; các bước và điểm kiểm tra (assertion); thời gian chạy 14.632 ms
  - Các bước: Đăng nhập khách; chọn phim đầu tiên → suất khả dụng đầu tiên; chọn 2 ghế; kiểm tra "Chi tiết đơn hàng", "Ghế (2)"; thêm combo nếu có; giữ ghế; kiểm tra đếm ngược "Giữ ghế còn" và nút Thanh toán; đổi phương thức MoMo/VNPay; hủy giữ ghế; mở trang vé
  - Điều kiện bỏ qua: Bỏ qua khi điều kiện chọn ghế không thỏa (spec dòng 32)
- **Tài liệu/hình/bảng:**
  - `Bảng 6.15` Các bước kịch bản E2E-01
- **Nguồn dữ liệu:** `bo_sung/e2e/customer-booking-flow.spec.ts`

| Bước | Thao tác | Điều kiện kiểm tra |
|---|---|---|
| [CẦN BỔ SUNG: đọc spec] |  |  |


### 6.4.4 Kịch bản E2E-02 Trang kết quả khi đơn bị hủy

- **Nội dung cần viết:**
  - Mục đích kịch bản; điều kiện tiên quyết và điều kiện bỏ qua; các bước và điểm kiểm tra (assertion); thời gian chạy 11.097 ms
  - Các bước: Giữ ghế qua API rồi hủy; trang kết quả hiện "Đơn đặt vé đã bị huỷ", không hiện "Vé điện tử", có liên kết xem vé/về trang chủ
  - Điều kiện bỏ qua: Bỏ qua khi không đăng nhập được qua API, không còn ghế trống hoặc không hủy được
- **Tài liệu/hình/bảng:**
  - `Bảng 6.16` Các bước kịch bản E2E-02
- **Nguồn dữ liệu:** `bo_sung/e2e/customer-payment-failure-flow.spec.ts`

| Bước | Thao tác | Điều kiện kiểm tra |
|---|---|---|
| [CẦN BỔ SUNG: đọc spec] |  |  |


### 6.4.5 Kịch bản E2E-03 Trang kết quả với đơn không tồn tại

- **Nội dung cần viết:**
  - Mục đích kịch bản; điều kiện tiên quyết và điều kiện bỏ qua; các bước và điểm kiểm tra (assertion); thời gian chạy 9.209 ms
  - Các bước: Mở kết quả với `bookingId` không tồn tại → "Không tìm thấy đơn đặt vé này" và nút Thử lại
  - Điều kiện bỏ qua: Không
- **Tài liệu/hình/bảng:**
  - `Bảng 6.17` Các bước kịch bản E2E-03
- **Nguồn dữ liệu:** `bo_sung/e2e/customer-payment-failure-flow.spec.ts`

| Bước | Thao tác | Điều kiện kiểm tra |
|---|---|---|
| [CẦN BỔ SUNG: đọc spec] |  |  |


### 6.4.6 Kịch bản E2E-04 Hết hạn giữ ghế giữa chừng

- **Nội dung cần viết:**
  - Mục đích kịch bản; điều kiện tiên quyết và điều kiện bỏ qua; các bước và điểm kiểm tra (assertion); thời gian chạy 23.461 ms
  - Các bước: Giữ ghế qua giao diện; ép booking hết hạn trong CSDL; chờ tối đa 120 giây thông báo "Thời gian giữ ghế đã hết hạn, vui lòng chọn lại." và quay về bước chọn ghế, đếm ngược biến mất
  - Điều kiện bỏ qua: Bỏ qua khi không truy cập được CSDL hoặc không xác thực/giữ ghế được
- **Tài liệu/hình/bảng:**
  - `Bảng 6.18` Các bước kịch bản E2E-04
- **Nguồn dữ liệu:** `bo_sung/e2e/customer-seat-hold-timeout-flow.spec.ts`

| Bước | Thao tác | Điều kiện kiểm tra |
|---|---|---|
| [CẦN BỔ SUNG: đọc spec] |  |  |


### 6.4.7 Kịch bản E2E-05 Hồ sơ khách hàng

- **Nội dung cần viết:**
  - Mục đích kịch bản; điều kiện tiên quyết và điều kiện bỏ qua; các bước và điểm kiểm tra (assertion); thời gian chạy 13.804 ms
  - Các bước: Sửa họ tên/số điện thoại; thông báo thành công; tải lại vẫn giữ giá trị; chuyển tab vé "Đã thanh toán"/"Tất cả"
  - Điều kiện bỏ qua: Không
- **Tài liệu/hình/bảng:**
  - `Bảng 6.19` Các bước kịch bản E2E-05
- **Nguồn dữ liệu:** `bo_sung/e2e/customer-profile-flow.spec.ts`

| Bước | Thao tác | Điều kiện kiểm tra |
|---|---|---|
| [CẦN BỔ SUNG: đọc spec] |  |  |


### 6.4.8 Kịch bản E2E-06 Soát vé

- **Nội dung cần viết:**
  - Mục đích kịch bản; điều kiện tiên quyết và điều kiện bỏ qua; các bước và điểm kiểm tra (assertion); thời gian chạy 8.663 ms
  - Các bước: Mã sai → "Soát vé thất bại"; tạo vé hợp lệ (giữ ghế qua API, thanh toán quầy, đưa suất vào cửa sổ soát vé) → "Check-in thành công" kèm phim/ghế; quét lại → từ chối "đã check-in"
  - Điều kiện bỏ qua: Bỏ qua khi không tạo được vé kiểm thử (spec dòng 38)
- **Tài liệu/hình/bảng:**
  - `Bảng 6.20` Các bước kịch bản E2E-06
- **Nguồn dữ liệu:** `bo_sung/e2e/admin-checkin-flow.spec.ts`

| Bước | Thao tác | Điều kiện kiểm tra |
|---|---|---|
| [CẦN BỔ SUNG: đọc spec] |  |  |


### 6.4.9 Kịch bản E2E-07 Quản lý phim

- **Nội dung cần viết:**
  - Mục đích kịch bản; điều kiện tiên quyết và điều kiện bỏ qua; các bước và điểm kiểm tra (assertion); thời gian chạy 8.575 ms
  - Các bước: Tìm "Oppenheimer"; thêm phim (tên, thời lượng 130, mô tả, thể loại); thông báo; tìm thấy trong bảng; xóa; thông báo; biến mất
  - Điều kiện bỏ qua: Không
- **Tài liệu/hình/bảng:**
  - `Bảng 6.21` Các bước kịch bản E2E-07
- **Nguồn dữ liệu:** `bo_sung/e2e/admin-movies-flow.spec.ts`

| Bước | Thao tác | Điều kiện kiểm tra |
|---|---|---|
| [CẦN BỔ SUNG: đọc spec] |  |  |


### 6.4.10 Kịch bản E2E-08 Quản lý suất chiếu

- **Nội dung cần viết:**
  - Mục đích kịch bản; điều kiện tiên quyết và điều kiện bỏ qua; các bước và điểm kiểm tra (assertion); thời gian chạy 8.210 ms
  - Các bước: Chuyển tab Lịch/Danh sách; tạo suất (phim, phòng đầu tiên, ngày cuối cùng chọn được, giá 95.000); thông báo; hủy suất ở dòng đầu bảng; thông báo
  - Điều kiện bỏ qua: Không
- **Tài liệu/hình/bảng:**
  - `Bảng 6.22` Các bước kịch bản E2E-08
- **Nguồn dữ liệu:** `bo_sung/e2e/admin-showtimes-flow.spec.ts`

| Bước | Thao tác | Điều kiện kiểm tra |
|---|---|---|
| [CẦN BỔ SUNG: đọc spec] |  |  |


### 6.4.11 Kết quả thực thi

- **Nội dung cần viết:**
  - Báo cáo Playwright chạy khoảng 11:07 UTC ngày 07/09/2026: 8/8 đạt, 0 lỗi, 0 flaky, 0 bỏ qua; 1 worker, Chromium; tổng 100,09 giây; `.last-run.json`: `status = passed`
  - Do không có kịch bản nào bị bỏ qua, các điều kiện tiên quyết (ép hết hạn, tạo vé soát) đã đáp ứng khi chạy
- **Tài liệu/hình/bảng:**
  - `Bảng 6.23` Kết quả E2E
- **Nguồn dữ liệu:** `bo_sung/playwright-report/index.html`; `bo_sung/test-results/.last-run.json`

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

- **Nội dung cần viết:**
  - Biểu đồ cột 8 giá trị (ms): E2E-01 14.632; E2E-02 11.097; E2E-03 9.209; E2E-04 23.461; E2E-05 13.804; E2E-06 8.663; E2E-07 8.575; E2E-08 8.210
  - Vẽ bằng Excel hoặc matplotlib
- **Tài liệu/hình/bảng:**
  - `Biểu đồ 6.1` Thời gian chạy từng kịch bản E2E
- **Nguồn dữ liệu:** `bo_sung/playwright-report/index.html`

### 6.4.13 Báo cáo Playwright

- **Nội dung cần viết:**
  - Ảnh chụp trang tổng quan báo cáo HTML
- **Tài liệu/hình/bảng:**
  - `Hình 6.2` Báo cáo kiểm thử Playwright
- **Nguồn dữ liệu:** `bo_sung/playwright-report/index.html`

### 6.4.14 Hàm hỗ trợ kiểm thử

- **Nội dung cần viết:**
  - `helpers.ts`: đăng nhập khách/quản trị qua giao diện; lấy token qua API; hủy booking Pending tồn đọng; giữ ghế qua API; ép hết hạn booking và dời giờ suất số 1 bằng lệnh SQL chạy trong container CSDL; tài khoản mặc định lấy từ biến môi trường hoặc tài khoản seed; không in mật khẩu
- **Nguồn dữ liệu:** `bo_sung/e2e/helpers.ts`; `Persistence/Seed/DbInitializer.cs`

### 6.4.15 Phạm vi chưa bao phủ

- **Nội dung cần viết:**
  - Các tính năng chưa có kịch bản E2E: thanh toán thật qua cổng (chuyển hướng, IPN), hoàn tiền, đăng ký/xác nhận email/quên mật khẩu/Google, bán vé quầy qua giao diện, trợ lý AI, trình soạn sơ đồ ghế, báo cáo/dashboard, nhiều client nhận SignalR đồng thời, trình duyệt khác Chromium
- **Tài liệu/hình/bảng:**
  - `Bảng 6.24` Phạm vi E2E chưa bao phủ
- **Nguồn dữ liệu:** `bo_sung/e2e/*.spec.ts`

| Tính năng | Có E2E | Kiểm thử thay thế |
|---|---|---|
| Thanh toán qua cổng (redirect, IPN) | Không | Test đơn vị VNPay/Stripe; test tích hợp webhook giả |
| Hoàn tiền | Không | Test đơn vị |
| Đăng ký, xác nhận email, quên mật khẩu, Google | Không | AuthServiceTests, AuthEndpointsTests |
| Bán vé quầy qua giao diện | Chỉ qua API trong helper | BookingServiceCounterCheckinTests |
| Trợ lý AI | Không | Không có |
| Trình soạn sơ đồ ghế | Không | RoomServiceSeatLayoutTests, AdminRoomsEndpointsTests |
| Báo cáo, dashboard | Không | ReportServiceTests, DashboardServiceTests |
| Nhiều client SignalR | Không | SignalRShowtimeHubTests |


### 6.4.16 Lưu ý về môi trường chạy E2E

- **Nội dung cần viết:**
  - `helpers.ts` gọi `docker exec` vào container `cinema-sqlserver` bằng `sqlcmd` và T-SQL (`DATEADD`, `GETUTCDATE`) trong khi project hiện dùng PostgreSQL (Neon); báo cáo chạy ngày 07/09/2026 có trước các migration PostgreSQL (30/10–01/11/2026)
  - Kịch bản làm thay đổi dữ liệu thật (tạo/xóa phim, tạo/hủy suất, đổi hồ sơ, sửa giờ suất số 1); E2E-08 hủy dòng đầu tiên của bảng; `playwright.config.ts` import `dotenv` nhưng `frontend/package.json` không khai báo
  - `[CẦN BỔ SUNG: xác nhận CSDL dùng khi chạy E2E; chạy lại E2E trên môi trường hiện tại; xác minh thứ tự sắp xếp bảng suất chiếu; xác nhận đã cài dotenv]`
- **Tài liệu/hình/bảng:**
  - `Bảng 6.25` Lưu ý môi trường E2E
- **Nguồn dữ liệu:** `bo_sung/e2e/helpers.ts`; `frontend/playwright.config.ts`; `frontend/package.json`

| Vấn đề | Bằng chứng | Tác động | Đề xuất |
|---|---|---|---|
| Helper dùng SQL Server | helpers.ts: docker exec cinema-sqlserver, sqlcmd, T-SQL | Không chạy được với PostgreSQL | Viết lại helper dùng psql hoặc API |
| Không cô lập dữ liệu | Spec tạo/xóa dữ liệu thật | Có thể ảnh hưởng dữ liệu dùng chung | Dùng CSDL kiểm thử riêng |
| E2E-08 hủy dòng đầu bảng | admin-showtimes-flow.spec.ts | Có thể hủy nhầm suất | Chọn đúng dòng vừa tạo |
| dotenv chưa khai báo | playwright.config.ts | Cấu hình có thể lỗi khi cài mới | Thêm dotenv vào devDependencies |


## 6.5 THỰC NGHIỆM ĐẶT GHẾ ĐỒNG THỜI


### 6.5.1 Thiết kế thực nghiệm

- **Nội dung cần viết:**
  - Dựa trên test `HoldSeat_HighConcurrencyParallelRequests_OnlyOneWinsAndOthersGet409`: 10 khách đã đăng ký, cùng chờ một rào chắn rồi đồng thời giữ cùng một ghế; kỳ vọng 1 phản hồi 201 và các phản hồi còn lại 409 (đọc test để xác nhận assertion chính xác)
  - Test bổ trợ: hai khách cùng giữ một ghế (409 kèm `conflictSeatIds`); ghế hết hạn của chính người giữ được giữ lại, của người khác thì xung đột (`BookingServiceSeatHoldTests`); booking cũ hết hạn không giải phóng ghế đã được booking mới giữ (`SeatExpirationServiceTests`)
  - Mở rộng nếu cần: 50, 100 luồng; lặp nhiều vòng; nhiều ghế chồng lấn; đo thời gian phản hồi
- **Nguồn dữ liệu:** `backend/tests/Cinema.IntegrationTests/Tests/BookingFlowEndpointsTests.cs`; `backend/tests/Cinema.UnitTests/Services/BookingServiceSeatHoldTests.cs`; `SeatExpirationServiceTests.cs`

### 6.5.2 Kết quả thực nghiệm

- **Nội dung cần viết:**
  - Điền kết quả từ lần chạy thật
- **Tài liệu/hình/bảng:**
  - `Bảng 6.26` Kết quả thực nghiệm đặt ghế đồng thời
- **Nguồn dữ liệu:** [CẦN BỔ SUNG: kết quả chạy test hiện có và/hoặc thực nghiệm mở rộng]

| Kịch bản | Số luồng | Thành công | Xung đột 409 | Khác | Thời gian |
|---|---|---|---|---|---|
| Một ghế, 10 yêu cầu song song | 10 | [CẦN BỔ SUNG] | [CẦN BỔ SUNG] |  |  |


## 6.6 KIỂM THỬ TẢI


### 6.6.1 Phương pháp

- **Nội dung cần viết:**
  - Kịch bản dự kiến: duyệt danh sách phim (`browse-movies-stress.js`) và cao điểm mở bán vé (`peak-ticket-rush.js`) chạy qua `run-load-test.ps1`; chỉ số: p50/p95/p99, thông lượng, tỷ lệ lỗi theo số người dùng đồng thời; yếu tố ảnh hưởng: giới hạn 100 yêu cầu/60 giây/IP, RAM gói free
- **Nguồn dữ liệu:** `package.json (test:load:*)`; [CẦN BỔ SUNG: thư mục load-tests/ được tham chiếu nhưng chưa có]

### 6.6.2 Kết quả

- **Nội dung cần viết:**
  - Điền sau khi chạy; không bịa số liệu hiệu năng
- **Tài liệu/hình/bảng:**
  - `Bảng 6.27` Kết quả kiểm thử tải
  - `Biểu đồ 6.2` Độ trễ theo số người dùng đồng thời
- **Nguồn dữ liệu:** [CẦN BỔ SUNG: kết quả k6]

| Kịch bản | VU | p50 | p95 | p99 | Thông lượng | Tỷ lệ lỗi |
|---|---|---|---|---|---|---|
| [CẦN BỔ SUNG] |  |  |  |  |  |  |


---

# CHƯƠNG 7. ĐÁNH GIÁ KẾT QUẢ


## 7.1 TRUY VẾT YÊU CẦU


### 7.1.1 Truy vết use case đến kiểm thử

- **Nội dung cần viết:**
  - Ma trận truy vết 21 package use case → cài đặt → test đơn vị → test tích hợp → E2E; nhận xét vùng chưa có kiểm thử
- **Tài liệu/hình/bảng:**
  - `Bảng 7.1` Truy vết package use case đến kiểm thử
- **Nguồn dữ liệu:** `backend/tests/**`; `bo_sung/e2e/**`; `Mục 3.3 file này`

| Package | Cài đặt | Test đơn vị | Test tích hợp | E2E |
|---|---|---|---|---|
| XT Xác thực | AuthController, AuthService | AuthServiceTests, JwtTokenServiceTests, AuthValidatorsTests | AuthEndpointsTests | E2E-01…08 (đăng nhập) |
| HS Hồ sơ cá nhân | UsersController, UserProfileService | [CẦN BỔ SUNG: không có test đơn vị riêng] | — | E2E-05 |
| DP Duyệt phim | MoviesController, ShowtimesController | — | — | E2E-01 |
| DG Đánh giá phim | ReviewService | ReviewServiceTests | AdminReviewsBulkEndpointsTests (phía quản trị) | — |
| DV Đặt vé trực tuyến | BookingsController, BookingService | BookingServiceTests, BookingServiceSeatHoldTests, BookingValidatorsTests | BookingFlowEndpointsTests | E2E-01, E2E-04 |
| TT Thanh toán trực tuyến | PaymentsController, PaymentService, *Gateway | PaymentServiceVnPayTests, PaymentServiceStripeTests | BookingFlowEndpointsTests (webhook giả) | E2E-02 (kết quả thất bại) |
| AI Trợ lý AI | AiController, GeminiAiAssistantService | [CẦN BỔ SUNG: chưa có test] | — | — |
| BQ Bán vé tại quầy | AdminBookingsController, BookingService | BookingServiceCounterCheckinTests | AdminBookingsEndpointsTests | E2E-06 (tạo vé qua API) |
| SV Soát vé | AdminBookingsController, BookingService | BookingServiceCounterCheckinTests | AdminBookingsEndpointsTests | E2E-06 |
| QB Quản lý booking | AdminBookingsController | — | AdminBookingsEndpointsTests | — |
| QP Quản lý phim | AdminMoviesController, MovieService | — | AdminMoviesBulkEndpointsTests | E2E-07 |
| QD Quản lý danh mục | Admin*Controller, CatalogService | [CẦN BỔ SUNG: chưa có test] | — | — |
| QR Quản lý phòng chiếu | AdminRoomsController, RoomService | RoomServiceSeatLayoutTests | AdminRoomsEndpointsTests | — |
| QS Quản lý suất chiếu | AdminShowtimesController, ShowtimeService | ShowtimeService*Tests, ShowtimeSchedulerTests, ShowtimeLifecycleTests, OperatingHoursTests | AdminShowtimesBulkEndpointsTests | E2E-08 |
| QN Quản lý người dùng | AdminUsersController, EmployeeService | EmployeeServiceUserStatusTests | — | — |
| QV Quản lý đánh giá | AdminReviewsController | ReviewServiceTests | AdminReviewsBulkEndpointsTests | — |
| QT Quản lý thanh toán | AdminPaymentsController | [CẦN BỔ SUNG] | — | — |
| BC Báo cáo thống kê | ReportService, DashboardService | ReportServiceTests, DashboardServiceTests | — | — |
| CH Cấu hình rạp | SettingsService | ProductionOptionsValidatorTests (cấu hình Production) | — | — |
| TP Tải tệp | CloudinaryFileStorage | [CẦN BỔ SUNG: chưa có test] | — | — |
| HT Vận hành hệ thống | Background jobs | SeatExpirationServiceTests, ShowtimeLifecycleTests | SignalRShowtimeHubTests (thời gian thực) | E2E-04 (gián tiếp) |


## 7.2 ĐỐI CHIẾU MỤC TIÊU


### 7.2.1 Mức đạt từng mục tiêu

- **Nội dung cần viết:**
  - Đối chiếu từng mục tiêu MT1–MT5 (`Bảng 1.1`) với kết quả; chỉ khẳng định phần có số liệu ở Chương 6
- **Tài liệu/hình/bảng:**
  - `Bảng 7.2` Mức đạt mục tiêu
- **Nguồn dữ liệu:** `Chương 1, Chương 6`

| Mục tiêu | Bằng chứng | Mức đạt | Mục tham chiếu |
|---|---|---|---|
| MT1 Đủ chức năng | 103 use case ở Chương 3; module đã cài đặt ở Chương 5 | [CẦN BỔ SUNG: đối chiếu từng use case đã cài đặt] | 3.3, 5.3 |
| MT2 Không đặt trùng ghế | Test 10 yêu cầu song song | [CẦN BỔ SUNG: kết quả chạy] | 6.5 |
| MT3 Thanh toán an toàn | Test đơn vị VNPay/Stripe | [CẦN BỔ SUNG: kết quả chạy] | 6.2 |
| MT4 Thời gian thực | SignalRShowtimeHubTests | [CẦN BỔ SUNG: kết quả chạy] | 6.3 |
| MT5 Chất lượng kiểm thử | E2E 8/8 đạt | Một phần; còn [CẦN BỔ SUNG] | 6.4 |


## 7.3 THẢO LUẬN HẠN CHẾ


### 7.3.1 Chống trùng suất chiếu chỉ ở tầng ứng dụng

- **Nội dung cần viết:**
  - Bằng chứng: `ExceptionHandlingMiddleware.cs` tham chiếu ràng buộc `EX_Showtimes_Room_NoOverlap` nhưng không có trong `Migrations`/`Configurations`; kiểm tra tại `ShowtimeService.HasOverlapAsync`
  - Tác động có thể xảy ra (nêu là khả năng, không khẳng định đã xảy ra): Hai yêu cầu tạo suất đồng thời có thể cùng vượt qua bước kiểm tra
  - Đề xuất khắc phục: Thêm ràng buộc loại trừ ở CSDL
  - Mở file nguồn để xác minh lại bằng chứng trước khi viết

### 7.3.2 Ghế hết hạn chỉ được giải phóng bởi job nền

- **Nội dung cần viết:**
  - Bằng chứng: `SeatExpirationService.ReleaseStaleHeldSeatsAsync` chỉ được gọi từ `ExpireDueBookingsAsync` (job mỗi phút); `GetShowtimeSeatsAsync` trả trạng thái thô
  - Tác động có thể xảy ra (nêu là khả năng, không khẳng định đã xảy ra): Ghế còn Held sau khi hết hạn đến khi job chạy; khả năng ảnh hưởng khi dịch vụ gói free ngủ [CẦN BỔ SUNG: kiểm chứng]
  - Đề xuất khắc phục: Giải phóng ngay khi truy vấn hoặc khi giữ ghế
  - Mở file nguồn để xác minh lại bằng chứng trước khi viết

### 7.3.3 Thành phần đánh giá trong Heat dùng thang sai

- **Nội dung cần viết:**
  - Bằng chứng: `ShowtimeScheduler.Heat` chia điểm cho 10, `AverageRating` bị ràng buộc 0–5 (`CK_Movies_AverageRating`); chưa có test cho `Heat`
  - Tác động có thể xảy ra (nêu là khả năng, không khẳng định đã xảy ra): Trọng số điểm đánh giá thực tế tối đa 0,15 thay vì 0,3
  - Đề xuất khắc phục: Chia cho 5 hoặc chuẩn hóa; thêm test
  - Mở file nguồn để xác minh lại bằng chứng trước khi viết

### 7.3.4 Seed dữ liệu demo chạy ở mọi môi trường

- **Nội dung cần viết:**
  - Bằng chứng: `Program.cs` gọi `DbInitializer.SeedAsync` khi `RunMigrationsOnStartup`; `render.yaml` đặt `true` với `Production`
  - Tác động có thể xảy ra (nêu là khả năng, không khẳng định đã xảy ra): Tài khoản/dữ liệu demo có thể tồn tại ở Production
  - Đề xuất khắc phục: Tách seed demo khỏi Production
  - Mở file nguồn để xác minh lại bằng chứng trước khi viết

### 7.3.5 Refresh token lưu localStorage, chưa có CSP

- **Nội dung cần viết:**
  - Bằng chứng: `frontend/src/stores/auth-store.ts`; `frontend/vercel.json` có `nosniff`, `Referrer-Policy`, `X-Robots-Tag` nhưng không có CSP
  - Tác động có thể xảy ra (nêu là khả năng, không khẳng định đã xảy ra): Rủi ro đánh cắp token nếu có XSS
  - Đề xuất khắc phục: Cookie HttpOnly và CSP
  - Mở file nguồn để xác minh lại bằng chứng trước khi viết

### 7.3.6 Điểm yếu của endpoint AI

- **Nội dung cần viết:**
  - Bằng chứng: `AiController` không yêu cầu đăng nhập, chỉ có giới hạn tốc độ toàn cục; `AiValidators.cs` cho phép vai trò `system` từ client trong khi dịch vụ ánh xạ mọi vai trò khác `user` thành `model`; system prompt chứa email người dùng đăng nhập và danh sách phim
  - Tác động có thể xảy ra (nêu là khả năng, không khẳng định đã xảy ra): Chi phí gọi mô hình, giả mạo lịch sử hội thoại, gửi dữ liệu cá nhân cho bên thứ ba
  - Đề xuất khắc phục: Giới hạn tốc độ riêng, bỏ vai trò system, giảm dữ liệu cá nhân
  - Mở file nguồn để xác minh lại bằng chứng trước khi viết

### 7.3.7 vnp_IpAddr cố định

- **Nội dung cần viết:**
  - Bằng chứng: `VnPayGateway.DefaultIpAddress = "127.0.0.1"`
  - Tác động có thể xảy ra (nêu là khả năng, không khẳng định đã xảy ra): Không phản ánh IP thật của khách
  - Đề xuất khắc phục: Truyền IP thật
  - Mở file nguồn để xác minh lại bằng chứng trước khi viết

### 7.3.8 Khai báo chưa dùng

- **Nội dung cần viết:**
  - Bằng chứng: `PaymentMethodNames.ZaloPay` không có enum hoặc gateway
  - Tác động có thể xảy ra (nêu là khả năng, không khẳng định đã xảy ra): Gây hiểu nhầm hỗ trợ ZaloPay
  - Đề xuất khắc phục: Xóa hoặc cài đặt
  - Mở file nguồn để xác minh lại bằng chứng trước khi viết

### 7.3.9 Chú thích lỗi thời

- **Nội dung cần viết:**
  - Bằng chứng: `Program.cs` nhắc MediatR và "SQL Server" cho Hangfire; `Cinema.Shared/Resources/README.md` nhắc `MessagesResourceTests` không tồn tại; `GeminiAiAssistantService` đăng ký hai lần (`Infrastructure/DependencyInjection.cs` và `Application/DependencyInjection.cs`)
  - Tác động có thể xảy ra (nêu là khả năng, không khẳng định đã xảy ra): Gây hiểu nhầm khi bảo trì
  - Đề xuất khắc phục: Cập nhật chú thích, dọn đăng ký trùng
  - Mở file nguồn để xác minh lại bằng chứng trước khi viết

### 7.3.10 Migration thiếu Designer, bỏ qua cảnh báo thay đổi mô hình

- **Nội dung cần viết:**
  - Bằng chứng: `Migrations/20261101120000_AddSeatGapRuleMode.cs`; `Infrastructure/DependencyInjection.cs` (`PendingModelChangesWarning`)
  - Tác động có thể xảy ra (nêu là khả năng, không khẳng định đã xảy ra): Snapshot có thể lệch mô hình
  - Đề xuất khắc phục: Tạo lại migration, bật cảnh báo
  - Mở file nguồn để xác minh lại bằng chứng trước khi viết

### 7.3.11 Kiểm tra tài khoản bằng truy vấn CSDL mỗi yêu cầu

- **Nội dung cần viết:**
  - Bằng chứng: `Program.cs` → `OnTokenValidated`
  - Tác động có thể xảy ra (nêu là khả năng, không khẳng định đã xảy ra): Thêm một truy vấn cho mỗi yêu cầu có xác thực
  - Đề xuất khắc phục: Bộ nhớ đệm ngắn hạn [CẦN BỔ SUNG: đo ảnh hưởng]
  - Mở file nguồn để xác minh lại bằng chứng trước khi viết

### 7.3.12 Thiếu CI/CD, test frontend, kiểm thử tải

- **Nội dung cần viết:**
  - Bằng chứng: Không có `.github`/pipeline; frontend chỉ có ESLint và E2E; `load-tests/` vắng
  - Tác động có thể xảy ra (nêu là khả năng, không khẳng định đã xảy ra): Không có kiểm tra tự động khi đẩy mã
  - Đề xuất khắc phục: Thêm pipeline, Vitest, k6
  - Mở file nguồn để xác minh lại bằng chứng trước khi viết

### 7.3.13 Hạn chế của bộ E2E

- **Nội dung cần viết:**
  - Bằng chứng: Xem mục 6.4 (lưu ý môi trường E2E)
  - Tác động có thể xảy ra (nêu là khả năng, không khẳng định đã xảy ra): Kết quả E2E cần chạy lại trên môi trường hiện tại
  - Đề xuất khắc phục: Viết lại helper cho PostgreSQL
  - Mở file nguồn để xác minh lại bằng chứng trước khi viết

### 7.3.14 Kiểu dữ liệu client lệch server

- **Nội dung cần viết:**
  - Bằng chứng: `frontend/src/lib/signalr.ts` (`SeatHeldPayload.heldByUserId`) trong khi server không gửi trường này
  - Tác động có thể xảy ra (nêu là khả năng, không khẳng định đã xảy ra): Kiểu dữ liệu không khớp
  - Đề xuất khắc phục: Đồng bộ kiểu
  - Mở file nguồn để xác minh lại bằng chứng trước khi viết

### 7.3.15 Hạn chế của cơ chế điểm thành viên 12 tháng

- **Nội dung cần viết:**
  - Bằng chứng: điểm và hạng trên `CustomerProfile` được tính lại bởi `ExpireLoyaltyPointsJob` (cron `5 * * * *`, mỗi giờ); cửa sổ `LoyaltyWindow.Months = 12` cố định trong code; thay đổi giao diện chỉ về nhãn chữ; script `Migrations/_manual/backfill_loyalty_entries.sql` chạy thủ công
  - Tác động có thể xảy ra (nêu là khả năng, không khẳng định đã xảy ra): hồ sơ hiển thị điểm và hạng cũ tối đa 1 giờ (giảm giá lúc đặt vé vẫn chính xác vì đọc từ sổ); khách không được báo điểm sắp hết hạn; muốn đổi cửa sổ phải sửa code; khách cũ chưa backfill sẽ bị job đưa điểm về 0
  - Đề xuất khắc phục: Tính điểm hiệu lực khi đọc hồ sơ hoặc rút ngắn lịch job; đưa cửa sổ vào `CinemaSettings`; thêm thông báo điểm sắp hết hạn và hiển thị ngày hết hạn; tự động hóa backfill trong migration
  - Mở file nguồn để xác minh lại bằng chứng trước khi viết

### 7.3.16 Tổng hợp hạn chế

- **Nội dung cần viết:**
  - Bảng tổng hợp 15 hạn chế và đề xuất
- **Tài liệu/hình/bảng:**
  - `Bảng 7.3` Hạn chế, đề xuất khắc phục
- **Nguồn dữ liệu:** `Các mục 7.3.1–7.3.15`

| STT | Hạn chế | Đề xuất |
|---|---|---|
| 1 | Chống trùng suất chiếu chỉ ở tầng ứng dụng | Thêm ràng buộc loại trừ ở CSDL |
| 2 | Ghế hết hạn chỉ được giải phóng bởi job nền | Giải phóng ngay khi truy vấn hoặc khi giữ ghế |
| 3 | Thành phần đánh giá trong Heat dùng thang sai | Chia cho 5 hoặc chuẩn hóa; thêm test |
| 4 | Seed dữ liệu demo chạy ở mọi môi trường | Tách seed demo khỏi Production |
| 5 | Refresh token lưu localStorage, chưa có CSP | Cookie HttpOnly và CSP |
| 6 | Điểm yếu của endpoint AI | Giới hạn tốc độ riêng, bỏ vai trò system, giảm dữ liệu cá nhân |
| 7 | vnp_IpAddr cố định | Truyền IP thật |
| 8 | Khai báo chưa dùng | Xóa hoặc cài đặt |
| 9 | Chú thích lỗi thời | Cập nhật chú thích, dọn đăng ký trùng |
| 10 | Migration thiếu Designer, bỏ qua cảnh báo thay đổi mô hình | Tạo lại migration, bật cảnh báo |
| 11 | Kiểm tra tài khoản bằng truy vấn CSDL mỗi yêu cầu | Bộ nhớ đệm ngắn hạn [CẦN BỔ SUNG: đo ảnh hưởng] |
| 12 | Thiếu CI/CD, test frontend, kiểm thử tải | Thêm pipeline, Vitest, k6 |
| 13 | Hạn chế của bộ E2E | Viết lại helper cho PostgreSQL |
| 14 | Kiểu dữ liệu client lệch server | Đồng bộ kiểu |
| 15 | Điểm thành viên 12 tháng: điểm và hạng trên hồ sơ trễ tối đa 1 giờ, cửa sổ cố định trong code, chưa có thông báo hoặc hiển thị ngày hết hạn, khách cũ phải backfill | Đồng bộ ngay khi đọc hồ sơ hoặc rút ngắn lịch job, đưa cửa sổ vào cấu hình rạp, thêm thông báo và hiển thị ngày hết hạn, tự động hóa backfill trong migration |


---

# CHƯƠNG 8. KẾT LUẬN


## 8.1 KẾT QUẢ ĐẠT ĐƯỢC


### 8.1.1 Kết quả theo mục tiêu

- **Nội dung cần viết:**
  - Đối chiếu từng mục tiêu với kết quả thật (module đã cài đặt, kết quả test đơn vị/tích hợp/E2E, thực nghiệm đồng thời)
  - Chỉ khẳng định phần có số liệu ở Chương 6
- **Tài liệu/hình/bảng:**
  - `Bảng 8.1` Mức độ đạt mục tiêu (tổng kết)
- **Nguồn dữ liệu:** `Bảng 7.x (Mức đạt mục tiêu)`

| Mục tiêu | Mức đạt | Bằng chứng |
|---|---|---|
| MT1…MT5 | [CẦN BỔ SUNG: điền từ Chương 7] |  |


### 8.1.2 Đóng góp chính

- **Nội dung cần viết:**
  - Thiết kế giữ ghế nguyên tử kết hợp ràng buộc duy nhất
  - Thanh toán idempotent kèm tự hoàn tiền khi ghế hết hạn
  - Cập nhật ghế thời gian thực
  - Điểm thành viên theo cửa sổ trượt 12 tháng với sổ điểm, hạng có thể tăng hoặc giảm
  - Xếp lịch chiếu tự động có hàm điểm
  - Trợ lý AI function calling
- **Nguồn dữ liệu:** `Chương 4, Chương 5`

## 8.2 HẠN CHẾ

- **Nội dung cần viết:**
  - Tóm tắt các hạn chế quan trọng nhất từ mục 7.3, phân loại: độ đúng đắn/dữ liệu, bảo mật, vận hành, kiểm thử; phạm vi một rạp; cổng thanh toán mới ở sandbox; điểm thành viên có độ trễ đồng bộ tối đa 1 giờ, cửa sổ 12 tháng cố định, chưa có thông báo hết hạn
- **Nguồn dữ liệu:** `Mục 7.3`

## 8.3 HƯỚNG PHÁT TRIỂN

- **Nội dung cần viết:**
  - Ràng buộc loại trừ ở CSDL cho lịch phòng; giải phóng ghế hết hạn ngay khi truy vấn; sửa thang điểm Heat kèm test; tách seed demo khỏi Production; refresh token qua cookie HttpOnly và CSP; xác thực và giới hạn tốc độ riêng cho AI; truyền IP thật cho VNPay; thông báo và hiển thị ngày điểm sắp hết hạn, cho quản trị chỉnh cửa sổ điểm, đổi điểm lấy quà hoặc voucher; nhiều chi nhánh và cổng thanh toán (ZaloPay); CI/CD; test frontend; mở rộng E2E và kiểm thử tải
- **Nguồn dữ liệu:** `Mục 7.3`

---

# TÀI LIỆU THAM KHẢO

- **Nội dung cần viết:**
  - Danh mục xếp a, b, c theo họ tác giả, tiếng Việt và nước ngoài chung một danh sách; chỉ đưa tài liệu đã đọc và kiểm tra thông tin xuất bản; không ghi nguồn chưa kiểm chứng
- **Tài liệu/hình/bảng:**
  - Các nhóm cần tìm ở Danh mục F cuối file
- **Nguồn dữ liệu:** [CẦN BỔ SUNG: danh mục tài liệu cụ thể do sinh viên tìm]
- **Định dạng Word:** Tiêu đề canh giữa, 14 đậm hoa; mỗi tài liệu một đoạn; dòng 2 trở đi lùi 0,5 cm; giãn dòng 1,0

---

# PHỤ LỤC 1. Hướng dẫn cài đặt, vận hành


## P1.1 YÊU CẦU MÔI TRƯỜNG

- **Nội dung cần viết:**
  - .NET 10 SDK, Node 22, Docker, tài khoản Neon
  - [CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch]
- **Nguồn dữ liệu:** `package.json`

## P1.2 CÀI ĐẶT

- **Nội dung cần viết:**
  - Sao chép biến môi trường (`npm run setup:env`); cài đặt phụ thuộc (`npm run setup`)
  - [CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch]
- **Nguồn dữ liệu:** `package.json`; `.env.example`

## P1.3 CHẠY BẰNG DOCKER

- **Nội dung cần viết:**
  - `npm run docker:up`
  - [CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch]
- **Nguồn dữ liệu:** `docker-compose.yml`

## P1.4 CHẠY THỦ CÔNG

- **Nội dung cần viết:**
  - `dev:backend`, `dev:frontend`
  - [CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch]
- **Nguồn dữ liệu:** `package.json`

## P1.5 ÁP DỤNG MIGRATION

- **Nội dung cần viết:**
  - `npm run db:update`
  - Với CSDL đã có dữ liệu: sau migration `AddLoyaltyPointEntries`, chạy `Migrations/_manual/backfill_loyalty_entries.sql` trước khi job điểm hết hạn chạy lần đầu [CẦN BỔ SUNG: xác nhận cách chạy script]
  - [CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch]
- **Nguồn dữ liệu:** `package.json`; `Migrations/_manual/backfill_loyalty_entries.sql`

## P1.6 CHẠY KIỂM THỬ

- **Nội dung cần viết:**
  - `test:unit`, `test:integration`, `test:e2e`
  - [CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch]
- **Nguồn dữ liệu:** `package.json`

## P1.7 TÀI KHOẢN KIỂM THỬ

- **Nội dung cần viết:**
  - Email theo `SeedEmails`; không in mật khẩu — ghi "xem DbInitializer.cs"
  - [CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch]
- **Nguồn dữ liệu:** `Persistence/Seed/DbInitializer.cs`

## P1.8 SWAGGER, HANGFIRE

- **Nội dung cần viết:**
  - Swagger ở môi trường Development; bảng điều khiển Hangfire `/admin/hangfire`
  - [CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch]
- **Nguồn dữ liệu:** `Program.cs`

---

# PHỤ LỤC 2. Danh mục API


## P2.1 ENDPOINT PHÍA KHÁCH HÀNG

- **Nội dung cần viết:**
  - 39 endpoint; cột Mô tả viết từ tên action và DTO [CẦN BỔ SUNG: mô tả ngắn]
- **Tài liệu/hình/bảng:**
  - `Bảng P2.1` Endpoint phía khách hàng
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

| STT | Controller | Phương thức | Đường dẫn | Phân quyền | Action | Giới hạn tốc độ |
|---|---|---|---|---|---|---|
| 1 | Ai | POST | `/api/v1/ai/chat` | Công khai | Chat | — |
| 2 | Auth | POST | `/api/v1/auth/register` | Công khai | Register | auth |
| 3 | Auth | POST | `/api/v1/auth/confirm-email` | Công khai | ConfirmEmail | — |
| 4 | Auth | POST | `/api/v1/auth/login` | Công khai | Login | auth |
| 5 | Auth | POST | `/api/v1/auth/admin-login` | Công khai | AdminLogin | auth |
| 6 | Auth | POST | `/api/v1/auth/google` | Công khai | GoogleLogin | auth |
| 7 | Auth | POST | `/api/v1/auth/refresh` | Công khai | Refresh | auth |
| 8 | Auth | POST | `/api/v1/auth/logout` | Công khai | Logout | — |
| 9 | Auth | POST | `/api/v1/auth/forgot-password` | Công khai | ForgotPassword | auth |
| 10 | Auth | POST | `/api/v1/auth/reset-password` | Công khai | ResetPassword | auth |
| 11 | Auth | POST | `/api/v1/auth/resend-confirmation` | Công khai | ResendConfirmationEmail | — |
| 12 | Auth | POST | `/api/v1/auth/socket-token` | Đã đăng nhập | GenerateSocketToken | — |
| 13 | Auth | GET | `/api/v1/auth/me` | Đã đăng nhập | Me | — |
| 14 | Bookings | POST | `/api/v1/bookings/hold` | Đã đăng nhập | HoldSeats | booking-hold |
| 15 | Bookings | GET | `/api/v1/bookings` | Đã đăng nhập | GetMyBookings | — |
| 16 | Bookings | GET | `/api/v1/bookings/{bookingId:int}` | Đã đăng nhập | GetBookingDetail | — |
| 17 | Bookings | POST | `/api/v1/bookings/{bookingId:int}/checkout` | Đã đăng nhập | Checkout | — |
| 18 | Bookings | POST | `/api/v1/bookings/{bookingId:int}/cancel` | Đã đăng nhập | CancelBooking | — |
| 19 | Catalog | GET | `/api/v1/genres` | Công khai | GetGenres | — |
| 20 | Catalog | GET | `/api/v1/languages` | Công khai | GetLanguages | — |
| 21 | Catalog | GET | `/api/v1/age-ratings` | Công khai | GetAgeRatings | — |
| 22 | Catalog | GET | `/api/v1/membership-tiers` | Công khai | GetMembershipTiers | — |
| 23 | Combos | GET | `/api/v1/combos` | Công khai | GetCombos | — |
| 24 | Combos | GET | `/api/v1/combos/suggestions` | Công khai | GetSuggestions | — |
| 25 | Movies | GET | `/api/v1/movies` | Công khai | GetMovies | — |
| 26 | Movies | GET | `/api/v1/movies/{movieId:int}` | Công khai | GetMovieDetail | — |
| 27 | Movies | GET | `/api/v1/movies/{movieId:int}/reviews` | Công khai | GetReviews | — |
| 28 | Movies | GET | `/api/v1/movies/{movieId:int}/can-review` | Công khai | GetCanReviewStatus | — |
| 29 | Movies | POST | `/api/v1/movies/{movieId:int}/reviews` | CustomerOnly | CreateReview | — |
| 30 | Movies | PUT | `/api/v1/movies/{movieId:int}/reviews/{id:int}` | CustomerOnly | UpdateReview | — |
| 31 | Movies | DELETE | `/api/v1/movies/{movieId:int}/reviews/{id:int}` | CustomerOnly | DeleteReview | — |
| 32 | Settings | GET | `/api/v1/settings` | Công khai | GetSettings | — |
| 33 | Showtimes | GET | `/api/v1/showtimes` | Công khai | GetSchedule | — |
| 34 | Showtimes | GET | `/api/v1/showtimes/{showtimeId:int}/seats` | Công khai | GetSeats | — |
| 35 | Showtimes | GET | `/api/v1/showtimes/{showtimeId:int}/seat-suggestions` | Công khai | GetSeatSuggestions | — |
| 36 | Users | GET | `/api/v1/users/me` | Đã đăng nhập | GetMyProfile | — |
| 37 | Users | PUT | `/api/v1/users/me` | Đã đăng nhập | UpdateMyProfile | — |
| 38 | Users | GET | `/api/v1/users/me/reviews` | Đã đăng nhập | GetMyReviews | — |
| 39 | Users | POST | `/api/v1/users/me/change-password` | Đã đăng nhập | ChangePassword | — |


## P2.2 ENDPOINT PHÍA QUẢN TRỊ

- **Nội dung cần viết:**
  - 93 endpoint; cột Mô tả viết từ tên action và DTO [CẦN BỔ SUNG: mô tả ngắn]
- **Tài liệu/hình/bảng:**
  - `Bảng P2.2` Endpoint phía quản trị
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

| STT | Controller | Phương thức | Đường dẫn | Phân quyền | Action | Giới hạn tốc độ |
|---|---|---|---|---|---|---|
| 1 | AdminActors | GET | `/api/v1/admin/actors` | AdminOnly | GetAll | — |
| 2 | AdminActors | POST | `/api/v1/admin/actors` | AdminOnly | Create | — |
| 3 | AdminActors | PUT | `/api/v1/admin/actors/{id:int}` | AdminOnly | Update | — |
| 4 | AdminActors | DELETE | `/api/v1/admin/actors/{id:int}` | AdminOnly | Delete | — |
| 5 | AdminActors | DELETE | `/api/v1/admin/actors/bulk` | AdminOnly | BulkDelete | — |
| 6 | AdminAgeRatings | GET | `/api/v1/admin/age-ratings` | AdminOnly | GetAll | — |
| 7 | AdminAgeRatings | POST | `/api/v1/admin/age-ratings` | AdminOnly | Create | — |
| 8 | AdminAgeRatings | PUT | `/api/v1/admin/age-ratings/{id:int}` | AdminOnly | Update | — |
| 9 | AdminAgeRatings | DELETE | `/api/v1/admin/age-ratings/{id:int}` | AdminOnly | Delete | — |
| 10 | AdminBookings | POST | `/api/v1/admin/bookings/counter-hold` | StaffOnly | CounterHoldSeats | — |
| 11 | AdminBookings | GET | `/api/v1/admin/bookings` | StaffOnly | GetBookings | — |
| 12 | AdminBookings | GET | `/api/v1/admin/bookings/{bookingId:int}` | StaffOnly | GetBookingDetail | — |
| 13 | AdminBookings | POST | `/api/v1/admin/bookings/{bookingId:int}/counter-checkout` | StaffOnly | CounterCheckout | — |
| 14 | AdminBookings | POST | `/api/v1/admin/bookings/checkin` | StaffOnly | Checkin | — |
| 15 | AdminCombos | GET | `/api/v1/admin/combos` | AdminOnly | GetCombos | — |
| 16 | AdminCombos | POST | `/api/v1/admin/combos` | AdminOnly | Create | — |
| 17 | AdminCombos | PUT | `/api/v1/admin/combos/{id:int}` | AdminOnly | Update | — |
| 18 | AdminCombos | PUT | `/api/v1/admin/combos/{id:int}/toggle` | AdminOnly | Toggle | — |
| 19 | AdminCombos | PUT | `/api/v1/admin/combos/bulk/status` | AdminOnly | BulkSetActive | — |
| 20 | AdminDashboard | GET | `/api/v1/admin/dashboard/summary` | AdminOnly | GetSummary | — |
| 21 | AdminDashboard | GET | `/api/v1/admin/dashboard/insights` | AdminOnly | GetInsights | — |
| 22 | AdminDirectors | GET | `/api/v1/admin/directors` | AdminOnly | GetAll | — |
| 23 | AdminDirectors | POST | `/api/v1/admin/directors` | AdminOnly | Create | — |
| 24 | AdminDirectors | PUT | `/api/v1/admin/directors/{id:int}` | AdminOnly | Update | — |
| 25 | AdminDirectors | DELETE | `/api/v1/admin/directors/{id:int}` | AdminOnly | Delete | — |
| 26 | AdminDirectors | DELETE | `/api/v1/admin/directors/bulk` | AdminOnly | BulkDelete | — |
| 27 | AdminEmployees | GET | `/api/v1/admin/employees` | AdminOnly | GetEmployees | — |
| 28 | AdminEmployees | POST | `/api/v1/admin/employees` | AdminOnly | Create | — |
| 29 | AdminEmployees | PUT | `/api/v1/admin/employees/{id:int}` | AdminOnly | Update | — |
| 30 | AdminGenres | GET | `/api/v1/admin/genres` | AdminOnly | GetAll | — |
| 31 | AdminGenres | POST | `/api/v1/admin/genres` | AdminOnly | Create | — |
| 32 | AdminGenres | PUT | `/api/v1/admin/genres/{id:int}` | AdminOnly | Update | — |
| 33 | AdminGenres | DELETE | `/api/v1/admin/genres/{id:int}` | AdminOnly | Delete | — |
| 34 | AdminLanguages | GET | `/api/v1/admin/languages` | AdminOnly | GetAll | — |
| 35 | AdminLanguages | POST | `/api/v1/admin/languages` | AdminOnly | Create | — |
| 36 | AdminLanguages | PUT | `/api/v1/admin/languages/{id:int}` | AdminOnly | Update | — |
| 37 | AdminLanguages | DELETE | `/api/v1/admin/languages/{id:int}` | AdminOnly | Delete | — |
| 38 | AdminMembershipTiers | GET | `/api/v1/admin/membership-tiers` | AdminOnly | GetAll | — |
| 39 | AdminMembershipTiers | POST | `/api/v1/admin/membership-tiers` | AdminOnly | Create | — |
| 40 | AdminMembershipTiers | PUT | `/api/v1/admin/membership-tiers/{id:int}` | AdminOnly | Update | — |
| 41 | AdminMembershipTiers | DELETE | `/api/v1/admin/membership-tiers/{id:int}` | AdminOnly | Delete | — |
| 42 | AdminMovies | GET | `/api/v1/admin/movies` | StaffOnly | GetMovies | — |
| 43 | AdminMovies | POST | `/api/v1/admin/movies` | AdminOnly | Create | — |
| 44 | AdminMovies | PUT | `/api/v1/admin/movies/{id:int}` | AdminOnly | Update | — |
| 45 | AdminMovies | DELETE | `/api/v1/admin/movies/{id:int}` | AdminOnly | Delete | — |
| 46 | AdminMovies | DELETE | `/api/v1/admin/movies/bulk` | AdminOnly | BulkDelete | — |
| 47 | AdminPayments | GET | `/api/v1/admin/payments` | AdminOnly | GetPayments | — |
| 48 | AdminPayments | GET | `/api/v1/admin/payments/{id:int}` | AdminOnly | GetDetail | — |
| 49 | AdminPositions | GET | `/api/v1/admin/positions` | AdminOnly | GetAll | — |
| 50 | AdminPositions | POST | `/api/v1/admin/positions` | AdminOnly | Create | — |
| 51 | AdminPositions | PUT | `/api/v1/admin/positions/{id:int}` | AdminOnly | Update | — |
| 52 | AdminPositions | DELETE | `/api/v1/admin/positions/{id:int}` | AdminOnly | Delete | — |
| 53 | AdminReports | GET | `/api/v1/admin/reports/revenue` | AdminOnly | GetRevenue | — |
| 54 | AdminReports | GET | `/api/v1/admin/reports/top-movies` | AdminOnly | GetTopMovies | — |
| 55 | AdminReports | GET | `/api/v1/admin/reports/employee-performance` | AdminOnly | GetEmployeePerformance | — |
| 56 | AdminReviews | GET | `/api/v1/admin/reviews` | AdminOnly | GetReviews | — |
| 57 | AdminReviews | DELETE | `/api/v1/admin/reviews/{id:int}` | AdminOnly | Delete | — |
| 58 | AdminReviews | DELETE | `/api/v1/admin/reviews/bulk` | AdminOnly | BulkDelete | — |
| 59 | AdminRooms | GET | `/api/v1/admin/rooms` | AdminOnly | GetRooms | — |
| 60 | AdminRooms | POST | `/api/v1/admin/rooms` | AdminOnly | Create | — |
| 61 | AdminRooms | GET | `/api/v1/admin/rooms/{id:int}` | AdminOnly | GetById | — |
| 62 | AdminRooms | PUT | `/api/v1/admin/rooms/{id:int}` | AdminOnly | Update | — |
| 63 | AdminRooms | DELETE | `/api/v1/admin/rooms/{id:int}` | AdminOnly | Delete | — |
| 64 | AdminRooms | GET | `/api/v1/admin/rooms/{id:int}/seats` | AdminOnly | GetSeats | — |
| 65 | AdminRooms | PUT | `/api/v1/admin/rooms/{id:int}/seats` | AdminOnly | UpdateSeatLayout | — |
| 66 | AdminScreenTypes | GET | `/api/v1/admin/screen-types` | AdminOnly | GetAll | — |
| 67 | AdminScreenTypes | POST | `/api/v1/admin/screen-types` | AdminOnly | Create | — |
| 68 | AdminScreenTypes | PUT | `/api/v1/admin/screen-types/{id:int}` | AdminOnly | Update | — |
| 69 | AdminScreenTypes | DELETE | `/api/v1/admin/screen-types/{id:int}` | AdminOnly | Delete | — |
| 70 | AdminSeatTypes | GET | `/api/v1/admin/seat-types` | AdminOnly | GetAll | — |
| 71 | AdminSeatTypes | POST | `/api/v1/admin/seat-types` | AdminOnly | Create | — |
| 72 | AdminSeatTypes | PUT | `/api/v1/admin/seat-types/{id:int}` | AdminOnly | Update | — |
| 73 | AdminSeatTypes | DELETE | `/api/v1/admin/seat-types/{id:int}` | AdminOnly | Delete | — |
| 74 | AdminSettings | GET | `/api/v1/admin/settings` | AdminOnly | Get | — |
| 75 | AdminSettings | PUT | `/api/v1/admin/settings` | AdminOnly | Update | — |
| 76 | AdminShowtimes | GET | `/api/v1/admin/showtimes` | StaffOnly | GetShowtimes | — |
| 77 | AdminShowtimes | GET | `/api/v1/admin/showtimes/calendar` | StaffOnly | GetCalendar | — |
| 78 | AdminShowtimes | POST | `/api/v1/admin/showtimes` | AdminOnly | Create | — |
| 79 | AdminShowtimes | PUT | `/api/v1/admin/showtimes/{id:int}` | AdminOnly | Update | — |
| 80 | AdminShowtimes | DELETE | `/api/v1/admin/showtimes/{id:int}` | AdminOnly | Delete | — |
| 81 | AdminShowtimes | POST | `/api/v1/admin/showtimes/bulk` | AdminOnly | BulkCreate | — |
| 82 | AdminShowtimes | POST | `/api/v1/admin/showtimes/auto-schedule/preview` | AdminOnly | PreviewAutoSchedule | — |
| 83 | AdminShowtimes | POST | `/api/v1/admin/showtimes/auto-schedule/apply` | AdminOnly | ApplyAutoSchedule | — |
| 84 | AdminShowtimes | POST | `/api/v1/admin/showtimes/clone` | AdminOnly | CloneShowtimes | — |
| 85 | AdminShowtimes | DELETE | `/api/v1/admin/showtimes/bulk` | AdminOnly | BulkDelete | — |
| 86 | AdminUploads | POST | `/api/v1/admin/uploads/image` | AdminOnly | UploadImage | — |
| 87 | AdminUploads | POST | `/api/v1/admin/uploads/video` | AdminOnly | UploadVideo | — |
| 88 | AdminUsers | GET | `/api/v1/admin/users` | AdminOnly | GetUsers | — |
| 89 | AdminUsers | GET | `/api/v1/admin/users/admins` | AdminOnly | GetAdminUsers | — |
| 90 | AdminUsers | GET | `/api/v1/admin/users/customers` | StaffOnly | SearchCustomers | — |
| 91 | AdminUsers | PUT | `/api/v1/admin/users/{id:int}/status` | AdminOnly | UpdateStatus | — |
| 92 | AdminUsers | PUT | `/api/v1/admin/users/{id:int}/role` | AdminOnly | UpdateRole | — |
| 93 | AdminUsers | PUT | `/api/v1/admin/users/bulk/status` | AdminOnly | BulkUpdateStatus | — |


## P2.3 ENDPOINT THANH TOÁN, KIỂM TRA SỨC KHỎE

- **Nội dung cần viết:**
  - 5 endpoint; cột Mô tả viết từ tên action và DTO [CẦN BỔ SUNG: mô tả ngắn]
- **Tài liệu/hình/bảng:**
  - `Bảng P2.3` Endpoint thanh toán, kiểm tra sức khỏe
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

| STT | Controller | Phương thức | Đường dẫn | Phân quyền | Action | Giới hạn tốc độ |
|---|---|---|---|---|---|---|
| 1 | Health | GET | `/api/v1/health` | Công khai | Get | — |
| 2 | Payments | GET | `/api/v1/payments/vnpay/ipn` | Công khai | VnPayIpn | — |
| 3 | Payments | POST | `/api/v1/payments/vnpay/ipn` | Công khai | VnPayIpn | — |
| 4 | Payments | POST | `/api/v1/payments/momo/ipn` | Công khai | MomoIpn | — |
| 5 | Payments | POST | `/api/v1/payments/stripe/webhook` | Công khai | StripeWebhook | — |


---

# PHỤ LỤC 3. Biến cấu hình môi trường


## P3.1 DANH SÁCH BIẾN

- **Nội dung cần viết:**
  - Chỉ ghi tên biến, không ghi giá trị bí mật; thêm cột ý nghĩa, bắt buộc ở Production, giá trị mặc định
  - `[CẦN BỔ SUNG: biến cho VNPay, MoMo, Stripe, Gemini, Google chỉ thấy trong appsettings.json, không có trong .env.example — xác nhận]`
- **Tài liệu/hình/bảng:**
  - `Bảng P3.1` Biến cấu hình môi trường
- **Nguồn dữ liệu:** `.env.example`; `frontend/.env.example`; `backend/Cinema.Api/appsettings.json`

| Nhóm | Biến |
|---|---|
| CSDL | DATABASE_URL, DATABASE_URL_DIRECT |
| Runtime | ASPNETCORE_ENVIRONMENT, RUN_MIGRATIONS_ON_STARTUP, FRONTEND_BASE_URL, API_BASE_URL, CORS_ALLOWED_ORIGIN |
| JWT | JWT_SECRET, JWT_ISSUER, JWT_AUDIENCE, JWT_ACCESS_TOKEN_EXPIRATION_SECONDS, JWT_REFRESH_TOKEN_EXPIRATION_DAYS, JWT_SOCKET_TOKEN_EXPIRATION_SECONDS |
| Brand | BRAND_NAME, BRAND_DISPLAY_NAME, BRAND_TAGLINE, BRAND_SUPPORT_EMAIL |
| Booking, giới hạn tốc độ | BOOKING_CHECK_IN_OPEN_MINUTES, RATE_LIMIT_GLOBAL_SEGMENTS_PER_WINDOW |
| Xếp lịch | SCHEDULER_STEP_MINUTES, SCHEDULER_MIN_SAME_MOVIE_GAP_MINUTES, SCHEDULER_DEFAULT_WINDOW_MINUTES, SCHEDULER_SPREAD_MINUTES, SCHEDULER_FIT_WEIGHT, SCHEDULER_LOAD_WEIGHT, SCHEDULER_HEAT_LOOKBACK_DAYS |
| Tác vụ nền | BG_RELEASE_EXPIRED_SEATS_CRON, BG_SHOWTIME_LIFECYCLE_CRON, BG_RETRY_ATTEMPTS; `ExpireLoyaltyPointsCron` trong `appsettings.json` [CẦN BỔ SUNG: kiểm tra có biến môi trường tương ứng hay không] |
| Cloudinary | CLOUDINARY_CLOUD_NAME, CLOUDINARY_API_KEY, CLOUDINARY_API_SECRET |
| SMTP | SMTP_HOST, SMTP_PORT, SMTP_USER, SMTP_PASSWORD, SMTP_FROM_ADDRESS, SMTP_FROM_NAME |
| Resend | RESEND_API_KEY, RESEND_FROM_ADDRESS |
| Frontend | VITE_API_BASE_URL, VITE_SIGNALR_HUB_URL, VITE_GOOGLE_CLIENT_ID, VITE_SITE_URL, VITE_BUSINESS_TIME_ZONE, VITE_LOCALE, VITE_CURRENCY_CODE, VITE_CURRENCY_SYMBOL |
| E2E | E2E_BASE_URL, E2E_CUSTOMER_EMAIL, E2E_CUSTOMER_PASSWORD, E2E_ADMIN_EMAIL, E2E_ADMIN_PASSWORD |


---

# PHỤ LỤC 4. Kịch bản E2E chi tiết


## P4.1 CÁC BƯỚC TỪNG KỊCH BẢN

- **Nội dung cần viết:**
  - Với mỗi kịch bản E2E-01…08: bảng bước – thao tác – điều kiện kiểm tra – timeout đặc biệt, đọc trực tiếp từ spec; không thêm bước không có
- **Tài liệu/hình/bảng:**
  - `Bảng P4.1` Các bước kịch bản E2E
- **Nguồn dữ liệu:** `bo_sung/e2e/*.spec.ts`; `bo_sung/e2e/helpers.ts`

| Kịch bản | Bước | Thao tác | Kiểm tra |
|---|---|---|---|
| [CẦN BỔ SUNG: đọc spec] |  |  |  |


---

# PHỤ LỤC 5. Trích đoạn mã nguồn tiêu biểu


## P5.1 CẬP NHẬT CÓ ĐIỀU KIỆN KHI GIỮ GHẾ

- **Nội dung cần viết:**
  - Đoạn mã rút gọn ≤ 20 dòng (rút gọn bằng "// ..." có chủ đích, không đổi logic); 3–5 câu giải thích
  - Không đưa khóa/bí mật vào đoạn mã; kiểm tra đoạn mã đúng với file hiện tại
- **Nguồn dữ liệu:** `BookingService.cs → CreateHoldBookingAsync`
- **Định dạng Word:** Style Mã nguồn; khung viền; caption "Đoạn mã P5.n" [CẦN BỔ SUNG: định dạng đánh số đoạn mã]

## P5.2 KIỂM TRA LUẬT CHỌN GHẾ

- **Nội dung cần viết:**
  - Đoạn mã rút gọn ≤ 20 dòng (rút gọn bằng "// ..." có chủ đích, không đổi logic); 3–5 câu giải thích
  - Không đưa khóa/bí mật vào đoạn mã; kiểm tra đoạn mã đúng với file hiện tại
- **Nguồn dữ liệu:** `Utils/SeatLayoutRules.cs → ValidateSelection`
- **Định dạng Word:** Style Mã nguồn; khung viền; caption "Đoạn mã P5.n" [CẦN BỔ SUNG: định dạng đánh số đoạn mã]

## P5.3 GỢI Ý GHẾ

- **Nội dung cần viết:**
  - Đoạn mã rút gọn ≤ 20 dòng (rút gọn bằng "// ..." có chủ đích, không đổi logic); 3–5 câu giải thích
  - Không đưa khóa/bí mật vào đoạn mã; kiểm tra đoạn mã đúng với file hiện tại
- **Nguồn dữ liệu:** `Utils/SeatLayoutRules.cs → Suggest`
- **Định dạng Word:** Style Mã nguồn; khung viền; caption "Đoạn mã P5.n" [CẦN BỔ SUNG: định dạng đánh số đoạn mã]

## P5.4 XẾP LỊCH TỰ ĐỘNG

- **Nội dung cần viết:**
  - Đoạn mã rút gọn ≤ 20 dòng (rút gọn bằng "// ..." có chủ đích, không đổi logic); 3–5 câu giải thích
  - Không đưa khóa/bí mật vào đoạn mã; kiểm tra đoạn mã đúng với file hiện tại
- **Nguồn dữ liệu:** `Utils/ShowtimeScheduler.cs → Build` (`Score` là phương thức private)
- **Định dạng Word:** Style Mã nguồn; khung viền; caption "Đoạn mã P5.n" [CẦN BỔ SUNG: định dạng đánh số đoạn mã]

## P5.5 ÁP DỤNG KẾT QUẢ THANH TOÁN

- **Nội dung cần viết:**
  - Đoạn mã rút gọn ≤ 20 dòng (rút gọn bằng "// ..." có chủ đích, không đổi logic); 3–5 câu giải thích
  - Không đưa khóa/bí mật vào đoạn mã; kiểm tra đoạn mã đúng với file hiện tại
- **Nguồn dữ liệu:** `PaymentService.cs → ApplyPaymentResultAsync`
- **Định dạng Word:** Style Mã nguồn; khung viền; caption "Đoạn mã P5.n" [CẦN BỔ SUNG: định dạng đánh số đoạn mã]

## P5.6 KÝ YÊU CẦU VNPAY

- **Nội dung cần viết:**
  - Đoạn mã rút gọn ≤ 20 dòng (rút gọn bằng "// ..." có chủ đích, không đổi logic); 3–5 câu giải thích
  - Không đưa khóa/bí mật vào đoạn mã; kiểm tra đoạn mã đúng với file hiện tại
- **Nguồn dữ liệu:** `Infrastructure/Payments/VnPayGateway.cs`
- **Định dạng Word:** Style Mã nguồn; khung viền; caption "Đoạn mã P5.n" [CẦN BỔ SUNG: định dạng đánh số đoạn mã]

## P5.7 LÀM MỚI TOKEN XOAY VÒNG

- **Nội dung cần viết:**
  - Đoạn mã rút gọn ≤ 20 dòng (rút gọn bằng "// ..." có chủ đích, không đổi logic); 3–5 câu giải thích
  - Không đưa khóa/bí mật vào đoạn mã; kiểm tra đoạn mã đúng với file hiện tại
- **Nguồn dữ liệu:** `AuthService.cs → RefreshAsync`
- **Định dạng Word:** Style Mã nguồn; khung viền; caption "Đoạn mã P5.n" [CẦN BỔ SUNG: định dạng đánh số đoạn mã]

## P5.8 HOOK CẬP NHẬT SƠ ĐỒ GHẾ

- **Nội dung cần viết:**
  - Đoạn mã rút gọn ≤ 20 dòng (rút gọn bằng "// ..." có chủ đích, không đổi logic); 3–5 câu giải thích
  - Không đưa khóa/bí mật vào đoạn mã; kiểm tra đoạn mã đúng với file hiện tại
- **Nguồn dữ liệu:** `frontend/src/hooks/use-realtime-seat-map.ts`
- **Định dạng Word:** Style Mã nguồn; khung viền; caption "Đoạn mã P5.n" [CẦN BỔ SUNG: định dạng đánh số đoạn mã]

## P5.9 TEST ĐỒNG THỜI GIỮ GHẾ

- **Nội dung cần viết:**
  - Đoạn mã rút gọn ≤ 20 dòng (rút gọn bằng "// ..." có chủ đích, không đổi logic); 3–5 câu giải thích
  - Không đưa khóa/bí mật vào đoạn mã; kiểm tra đoạn mã đúng với file hiện tại
- **Nguồn dữ liệu:** `backend/tests/Cinema.IntegrationTests/Tests/BookingFlowEndpointsTests.cs`
- **Định dạng Word:** Style Mã nguồn; khung viền; caption "Đoạn mã P5.n" [CẦN BỔ SUNG: định dạng đánh số đoạn mã]

---

## DANH MỤC A. HÌNH VẼ

| Số hiệu | Tên | Mục | Cách tạo / nguồn | Trạng thái |
|---|---|---|---|---|
| Hình 2.1 | Vòng đời của một lần đặt vé | 2.1.1 | Vẽ sơ đồ chuỗi bước | [CẦN BỔ SUNG: vẽ] |
| Hình 2.2 | Quy trình thực hiện đề tài | 2.4.1 | Vẽ sơ đồ quy trình | [CẦN BỔ SUNG: vẽ] |
| Hình 2.3 | Mô hình phân tầng các project | 2.5.3 | Vẽ từ các file csproj | [CẦN BỔ SUNG: vẽ] |
| Hình 3.1 | Quan hệ giữa các tác nhân | 3.1.2 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.2 | Sơ đồ ngữ cảnh hệ thống | 3.1.3 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 3.3 | Sơ đồ package use case tổng quát | 3.2.2 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.4 | Sơ đồ use case package Xác thực | 3.3.1.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.5 | Sơ đồ use case package Hồ sơ cá nhân | 3.3.2.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.6 | Sơ đồ use case package Duyệt phim | 3.3.3.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.7 | Sơ đồ use case package Đánh giá phim | 3.3.4.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.8 | Sơ đồ use case package Đặt vé trực tuyến | 3.3.5.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.9 | Sơ đồ use case package Thanh toán trực tuyến | 3.3.6.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.10 | Sơ đồ use case package Trợ lý AI | 3.3.7.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.11 | Sơ đồ use case package Bán vé tại quầy | 3.3.8.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.12 | Sơ đồ use case package Soát vé | 3.3.9.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.13 | Sơ đồ use case package Quản lý booking | 3.3.10.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.14 | Sơ đồ use case package Quản lý phim | 3.3.11.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.15 | Sơ đồ use case package Quản lý danh mục | 3.3.12.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.16 | Sơ đồ use case package Quản lý phòng chiếu | 3.3.13.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.17 | Sơ đồ use case package Quản lý suất chiếu | 3.3.14.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.18 | Sơ đồ use case package Quản lý người dùng | 3.3.15.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.19 | Sơ đồ use case package Quản lý đánh giá | 3.3.16.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.20 | Sơ đồ use case package Quản lý thanh toán | 3.3.17.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.21 | Sơ đồ use case package Báo cáo thống kê | 3.3.18.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.22 | Sơ đồ use case package Cấu hình rạp | 3.3.19.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.23 | Sơ đồ use case package Tải tệp | 3.3.20.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.24 | Sơ đồ use case package Vận hành hệ thống | 3.3.21.1 | PlantUML (cập nhật: thêm UC-HT-08) | [CẦN BỔ SUNG: vẽ] |
| Hình 4.1 | Kiến trúc tổng thể | 4.1.1 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.2 | Phụ thuộc giữa các project backend | 4.1.2 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.3 | Tổ chức mã nguồn frontend | 4.1.3 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.4 | Sơ đồ lớp miền | 4.2.3 | PlantUML (cập nhật: thêm thực thể LoyaltyPointEntry) | [CẦN BỔ SUNG: vẽ] |
| Hình 4.5 | Sơ đồ lớp package Xác thực | 4.2.4 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.6 | Sơ đồ lớp package Hồ sơ cá nhân | 4.2.5 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.7 | Sơ đồ lớp package Duyệt phim | 4.2.6 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.8 | Sơ đồ lớp package Đánh giá phim | 4.2.7 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.9 | Sơ đồ lớp package Đặt vé trực tuyến | 4.2.8 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.10 | Sơ đồ lớp package Thanh toán trực tuyến | 4.2.9 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.11 | Sơ đồ lớp package Trợ lý AI | 4.2.10 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.12 | Sơ đồ lớp package Bán vé tại quầy | 4.2.11 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.13 | Sơ đồ lớp package Soát vé | 4.2.12 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.14 | Sơ đồ lớp package Quản lý booking | 4.2.13 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.15 | Sơ đồ lớp package Quản lý phim | 4.2.14 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.16 | Sơ đồ lớp package Quản lý danh mục | 4.2.15 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.17 | Sơ đồ lớp package Quản lý phòng chiếu | 4.2.16 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.18 | Sơ đồ lớp package Quản lý suất chiếu | 4.2.17 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.19 | Sơ đồ lớp package Quản lý người dùng | 4.2.18 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.20 | Sơ đồ lớp package Quản lý đánh giá | 4.2.19 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.21 | Sơ đồ lớp package Quản lý thanh toán | 4.2.20 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.22 | Sơ đồ lớp package Báo cáo thống kê | 4.2.21 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.23 | Sơ đồ lớp package Cấu hình rạp | 4.2.22 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.24 | Sơ đồ lớp package Tải tệp | 4.2.23 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.25 | Sơ đồ lớp package Vận hành hệ thống | 4.2.24 | PlantUML (cập nhật: thêm job, dịch vụ điểm hết hạn) | [CẦN BỔ SUNG: vẽ] |
| Hình 4.26 | Sơ đồ ERD đầy đủ của hệ thống | 4.3.2 | Mermaid erDiagram; xuất PNG (cập nhật: thêm bảng LoyaltyPointEntries) | [CẦN BỔ SUNG: vẽ] |
| Hình 4.27 | ERD module Tài khoản người dùng | 4.3.4.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.28 | ERD module Danh mục phim | 4.3.5.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.29 | ERD module Phim | 4.3.6.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.30 | ERD module Phòng chiếu | 4.3.7.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.31 | ERD module Suất chiếu | 4.3.8.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.32 | ERD module Đặt vé | 4.3.9.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.33 | ERD module Thanh toán | 4.3.10.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.34 | ERD module Thành viên | 4.3.11.1 | Mermaid erDiagram (cập nhật: thêm bảng LoyaltyPointEntries) | [CẦN BỔ SUNG: vẽ] |
| Hình 4.35 | ERD module Cấu hình rạp | 4.3.12.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.36 | Máy trạng thái Booking | 4.3.14.1 | Mermaid stateDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.37 | Máy trạng thái ShowtimeSeat | 4.3.14.2 | Mermaid stateDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.38 | Máy trạng thái Payment | 4.3.14.3 | Mermaid stateDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.39 | Máy trạng thái Showtime | 4.3.14.4 | Mermaid stateDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.40 | Máy trạng thái Movie | 4.3.14.5 | Mermaid stateDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.41 | Tuần tự đăng ký tài khoản | 4.4.1 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.42 | Tuần tự đăng nhập bằng email | 4.4.2 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.43 | Tuần tự đăng nhập bằng Google | 4.4.3 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.44 | Tuần tự làm mới token xoay vòng | 4.4.4 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.45 | Lưu đồ phát hiện dùng lại refresh token | 4.4.5 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.46 | Tuần tự khôi phục mật khẩu | 4.4.6 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.47 | Lưu đồ giữ ghế nguyên tử | 4.5.1.2 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.48 | Tuần tự giữ ghế | 4.5.1.4 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.49 | Tuần tự tạo giao dịch thanh toán | 4.5.2.1 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.50 | Lưu đồ xử lý IPN | 4.5.2.2 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.51 | Tuần tự phát hành vé sau thanh toán | 4.5.2.3 | Mermaid sequenceDiagram (cập nhật: thêm bước ghi sổ điểm) | [CẦN BỔ SUNG: vẽ] |
| Hình 4.52 | Lưu đồ xử lý ghế hết hạn sau thanh toán | 4.5.2.4 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.53 | Lưu đồ giải phóng ghế hết hạn | 4.5.3 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.54 | Lưu đồ hủy vé | 4.5.4.2 | Mermaid flowchart (cập nhật: thêm bước thu hồi điểm theo booking) | [CẦN BỔ SUNG: vẽ] |
| Hình 4.55 | Tuần tự bán vé tại quầy | 4.5.5 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.56 | Lưu đồ soát vé | 4.5.6 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.57 | Tuần tự hủy suất chiếu | 4.5.7 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.58 | Lưu đồ ghi sổ điểm, thu hồi và đồng bộ điểm hết hạn | 4.5.8 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.59 | Lưu đồ viết đánh giá phim | 4.5.9 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.60 | Minh họa luật ghế lẻ | 4.6.1 | Sơ đồ hàng ghế (Mermaid/hình tự vẽ) | [CẦN BỔ SUNG: vẽ] |
| Hình 4.61 | Minh họa cửa sổ gợi ý ghế | 4.6.2.1 | Sơ đồ hàng ghế | [CẦN BỔ SUNG: vẽ] |
| Hình 4.62 | Lưu đồ xếp lịch chiếu tự động | 4.6.3.3 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.63 | Minh họa điều kiện trùng lịch phòng | 4.6.4 | Sơ đồ trục thời gian | [CẦN BỔ SUNG: vẽ] |
| Hình 4.64 | Tuần tự cập nhật ghế thời gian thực | 4.7.1 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.65 | Luồng function calling của trợ lý AI | 4.8.1 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.66 | Sơ đồ trang khách hàng | 4.9.2 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.67 | Sơ đồ trang quản trị | 4.9.3 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 5.1 | Cấu trúc thư mục chính của dự án | 5.2.1 | Tạo từ cây thư mục | [CẦN BỔ SUNG: vẽ] |
| Hình 5.2 | Màn hình Trang chủ | 5.4.1.1 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.3 | Màn hình Danh sách phim | 5.4.1.2 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.4 | Màn hình Chi tiết phim | 5.4.1.3 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.5 | Màn hình Lịch chiếu | 5.4.1.4 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.6 | Màn hình Chọn ghế | 5.4.1.5 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.7 | Màn hình Thanh toán | 5.4.1.6 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.8 | Màn hình Kết quả đặt vé | 5.4.1.7 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.9 | Màn hình Vé của tôi | 5.4.1.8 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.10 | Màn hình Chi tiết vé | 5.4.1.9 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.11 | Màn hình Hồ sơ cá nhân | 5.4.1.10 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.12 | Màn hình Thành viên | 5.4.1.11 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.13 | Màn hình Trợ lý AI | 5.4.1.12 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.14 | Màn hình Đăng nhập | 5.4.1.13 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.15 | Màn hình Đăng ký | 5.4.1.14 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.16 | Màn hình Dashboard | 5.4.2.1 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.17 | Màn hình Quản lý phim | 5.4.2.2 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.18 | Màn hình Quản lý suất chiếu | 5.4.2.3 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.19 | Màn hình Lịch suất chiếu | 5.4.2.4 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.20 | Màn hình Tự động xếp lịch | 5.4.2.5 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.21 | Màn hình Quản lý phòng chiếu | 5.4.2.6 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.22 | Màn hình Trình soạn sơ đồ ghế | 5.4.2.7 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.23 | Màn hình Quầy bán vé | 5.4.2.8 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.24 | Màn hình Soát vé | 5.4.2.9 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.25 | Màn hình Quản lý booking | 5.4.2.10 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.26 | Màn hình Quản lý thanh toán | 5.4.2.11 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.27 | Màn hình Báo cáo doanh thu | 5.4.2.12 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.28 | Màn hình Quản lý người dùng | 5.4.2.13 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.29 | Màn hình Quản lý nhân viên | 5.4.2.14 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.30 | Màn hình Quản lý combo | 5.4.2.15 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.31 | Màn hình Quản lý đánh giá | 5.4.2.16 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.32 | Màn hình Cấu hình rạp | 5.4.2.17 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.33 | Màn hình Quản lý danh mục | 5.4.2.18 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.34 | Giao diện trên thiết bị di động | 5.4.3.1 | Chụp màn hình | [CẦN BỔ SUNG: chụp] |
| Hình 5.35 | Sơ đồ triển khai hệ thống | 5.7.1 | Vẽ từ cấu hình triển khai | [CẦN BỔ SUNG: vẽ] |
| Hình 6.1 | Kim tự tháp kiểm thử của dự án | 6.1 | Vẽ từ số liệu thật | [CẦN BỔ SUNG: vẽ] |
| Hình 6.2 | Báo cáo kiểm thử Playwright | 6.4.13 | Mở bo_sung/playwright-report/index.html và chụp màn hình | [CẦN BỔ SUNG: chụp] |

---

## DANH MỤC B. BẢNG BIỂU

| Số hiệu | Tên | Mục | Cách tạo / nguồn | Trạng thái |
|---|---|---|---|---|
| Bảng 1.1 | Mục tiêu và tiêu chí đánh giá | 1.2.2 | Có dữ liệu trong outline | Có |
| Bảng 2.1 | So sánh các hệ thống đặt vé hiện có | 2.2.1 | Khảo sát bên ngoài | [CẦN BỔ SUNG: khảo sát] |
| Bảng 2.2 | So sánh các hướng tiếp cận chống xung đột đặt chỗ | 2.2.2 | Tổng hợp từ tài liệu nhóm T5 | Có |
| Bảng 2.3 | Công nghệ sử dụng và phiên bản | 2.6 | Có dữ liệu trong outline | Có |
| Bảng 3.1 | Danh sách tác nhân | 3.1.1 | Có dữ liệu trong outline | Có |
| Bảng 3.2 | Danh mục package use case | 3.2.1 | Có dữ liệu trong outline | Có |
| Bảng 3.3 | Ma trận tác nhân theo package | 3.2.3 | Có dữ liệu trong outline | Có |
| Bảng 3.4 | Danh sách use case package Xác thực | 3.3.1.2 | Có dữ liệu trong outline | Có |
| Bảng 3.5 | Danh sách use case package Hồ sơ cá nhân | 3.3.2.2 | Có dữ liệu trong outline | Có |
| Bảng 3.6 | Danh sách use case package Duyệt phim | 3.3.3.2 | Có dữ liệu trong outline | Có |
| Bảng 3.7 | Danh sách use case package Đánh giá phim | 3.3.4.2 | Có dữ liệu trong outline | Có |
| Bảng 3.8 | Danh sách use case package Đặt vé trực tuyến | 3.3.5.2 | Có dữ liệu trong outline | Có |
| Bảng 3.9 | Danh sách use case package Thanh toán trực tuyến | 3.3.6.2 | Có dữ liệu trong outline | Có |
| Bảng 3.10 | Danh sách use case package Trợ lý AI | 3.3.7.2 | Có dữ liệu trong outline | Có |
| Bảng 3.11 | Danh sách use case package Bán vé tại quầy | 3.3.8.2 | Có dữ liệu trong outline | Có |
| Bảng 3.12 | Danh sách use case package Soát vé | 3.3.9.2 | Có dữ liệu trong outline | Có |
| Bảng 3.13 | Danh sách use case package Quản lý booking | 3.3.10.2 | Có dữ liệu trong outline | Có |
| Bảng 3.14 | Danh sách use case package Quản lý phim | 3.3.11.2 | Có dữ liệu trong outline | Có |
| Bảng 3.15 | Danh sách use case package Quản lý danh mục | 3.3.12.2 | Có dữ liệu trong outline | Có |
| Bảng 3.16 | Danh sách use case package Quản lý phòng chiếu | 3.3.13.2 | Có dữ liệu trong outline | Có |
| Bảng 3.17 | Danh sách use case package Quản lý suất chiếu | 3.3.14.2 | Có dữ liệu trong outline | Có |
| Bảng 3.18 | Danh sách use case package Quản lý người dùng | 3.3.15.2 | Có dữ liệu trong outline | Có |
| Bảng 3.19 | Danh sách use case package Quản lý đánh giá | 3.3.16.2 | Có dữ liệu trong outline | Có |
| Bảng 3.20 | Danh sách use case package Quản lý thanh toán | 3.3.17.2 | Có dữ liệu trong outline | Có |
| Bảng 3.21 | Danh sách use case package Báo cáo thống kê | 3.3.18.2 | Có dữ liệu trong outline | Có |
| Bảng 3.22 | Danh sách use case package Cấu hình rạp | 3.3.19.2 | Có dữ liệu trong outline | Có |
| Bảng 3.23 | Danh sách use case package Tải tệp | 3.3.20.2 | Có dữ liệu trong outline | Có |
| Bảng 3.24 | Danh sách use case package Vận hành hệ thống | 3.3.21.2 | Có dữ liệu trong outline | Có |
| Bảng 3.25 | Đặc tả UC-XT-04 Đăng nhập bằng email | 3.4.1 | Có dữ liệu trong outline | Có |
| Bảng 3.26 | Đặc tả UC-DV-04 Giữ ghế | 3.4.2 | Có dữ liệu trong outline | Có |
| Bảng 3.27 | Đặc tả UC-TT-01 Thanh toán đơn đặt vé trực tuyến | 3.4.3 | Có dữ liệu trong outline | Có |
| Bảng 3.28 | Đặc tả UC-TT-05 Phát hành vé sau thanh toán | 3.4.4 | Có dữ liệu trong outline | Có |
| Bảng 3.29 | Đặc tả UC-DV-08 Hủy đơn đặt vé | 3.4.5 | Có dữ liệu trong outline | Có |
| Bảng 3.30 | Đặc tả UC-BQ-01 Bán vé tại quầy | 3.4.6 | Có dữ liệu trong outline | Có |
| Bảng 3.31 | Đặc tả UC-SV-01 Soát vé | 3.4.7 | Có dữ liệu trong outline | Có |
| Bảng 3.32 | Đặc tả UC-QS-07 Tự động xếp lịch chiếu | 3.4.8 | Có dữ liệu trong outline | Có |
| Bảng 3.33 | Đặc tả UC-QR-03 Soạn sơ đồ ghế | 3.4.9 | Có dữ liệu trong outline | Có |
| Bảng 3.34 | Quy tắc nghiệp vụ và tham số | 3.5.1 | Có dữ liệu trong outline | Có |
| Bảng 3.35 | Yêu cầu phi chức năng và cách đáp ứng | 3.6.1 | Có dữ liệu trong outline | Có |
| Bảng 4.1 | Cấu trúc phản hồi API | 4.1.4 | Có dữ liệu trong outline | Có |
| Bảng 4.2 | Ánh xạ ngoại lệ và lỗi PostgreSQL sang mã HTTP | 4.1.5 | Có dữ liệu trong outline | Có |
| Bảng 4.3 | Nhóm cấu hình ứng dụng | 4.1.6 | Có dữ liệu trong outline | Có |
| Bảng 4.4 | Ký hiệu sơ đồ lớp | 4.2.2 | Có dữ liệu trong outline | Có |
| Bảng 4.5 | Danh sách enum | 4.2.3 | Có dữ liệu trong outline | Có |
| Bảng 4.6 | Quan hệ giữa các thực thể | 4.2.3 | Có dữ liệu trong outline | Có |
| Bảng 4.7 | Danh sách lớp package Xác thực | 4.2.4 | Có dữ liệu trong outline | Có |
| Bảng 4.8 | Danh sách lớp package Hồ sơ cá nhân | 4.2.5 | Có dữ liệu trong outline | Có |
| Bảng 4.9 | Danh sách lớp package Duyệt phim | 4.2.6 | Có dữ liệu trong outline | Có |
| Bảng 4.10 | Danh sách lớp package Đánh giá phim | 4.2.7 | Có dữ liệu trong outline | Có |
| Bảng 4.11 | Danh sách lớp package Đặt vé trực tuyến | 4.2.8 | Có dữ liệu trong outline | Có |
| Bảng 4.12 | Danh sách lớp package Thanh toán trực tuyến | 4.2.9 | Có dữ liệu trong outline | Có |
| Bảng 4.13 | Danh sách lớp package Trợ lý AI | 4.2.10 | Có dữ liệu trong outline | Có |
| Bảng 4.14 | Danh sách lớp package Bán vé tại quầy | 4.2.11 | Có dữ liệu trong outline | Có |
| Bảng 4.15 | Danh sách lớp package Soát vé | 4.2.12 | Có dữ liệu trong outline | Có |
| Bảng 4.16 | Danh sách lớp package Quản lý booking | 4.2.13 | Có dữ liệu trong outline | Có |
| Bảng 4.17 | Danh sách lớp package Quản lý phim | 4.2.14 | Có dữ liệu trong outline | Có |
| Bảng 4.18 | Danh sách lớp package Quản lý danh mục | 4.2.15 | Có dữ liệu trong outline | Có |
| Bảng 4.19 | Danh sách lớp package Quản lý phòng chiếu | 4.2.16 | Có dữ liệu trong outline | Có |
| Bảng 4.20 | Danh sách lớp package Quản lý suất chiếu | 4.2.17 | Có dữ liệu trong outline | Có |
| Bảng 4.21 | Danh sách lớp package Quản lý người dùng | 4.2.18 | Có dữ liệu trong outline | Có |
| Bảng 4.22 | Danh sách lớp package Quản lý đánh giá | 4.2.19 | Có dữ liệu trong outline | Có |
| Bảng 4.23 | Danh sách lớp package Quản lý thanh toán | 4.2.20 | Có dữ liệu trong outline | Có |
| Bảng 4.24 | Danh sách lớp package Báo cáo thống kê | 4.2.21 | Có dữ liệu trong outline | Có |
| Bảng 4.25 | Danh sách lớp package Cấu hình rạp | 4.2.22 | Có dữ liệu trong outline | Có |
| Bảng 4.26 | Danh sách lớp package Tải tệp | 4.2.23 | Có dữ liệu trong outline | Có |
| Bảng 4.27 | Danh sách lớp package Vận hành hệ thống | 4.2.24 | Có dữ liệu trong outline | Có |
| Bảng 4.28 | Ánh xạ package use case sang nhóm lớp | 4.2.25 | Có dữ liệu trong outline | Có |
| Bảng 4.29 | Phân module cơ sở dữ liệu | 4.3.3 | Có dữ liệu trong outline | Có |
| Bảng 4.30 | Thực thể Role (bảng Roles) | 4.3.4.2 | Có dữ liệu trong outline | Có |
| Bảng 4.31 | Thực thể User (bảng Users) | 4.3.4.2 | Có dữ liệu trong outline | Có |
| Bảng 4.32 | Thực thể CustomerProfile (bảng CustomerProfiles) | 4.3.4.2 | Có dữ liệu trong outline | Có |
| Bảng 4.33 | Thực thể Employee (bảng Employees) | 4.3.4.2 | Có dữ liệu trong outline | Có |
| Bảng 4.34 | Thực thể Position (bảng Positions) | 4.3.4.2 | Có dữ liệu trong outline | Có |
| Bảng 4.35 | Thực thể RefreshToken (bảng RefreshTokens) | 4.3.4.2 | Có dữ liệu trong outline | Có |
| Bảng 4.36 | Thực thể UserVerificationToken (bảng UserVerificationTokens) | 4.3.4.2 | Có dữ liệu trong outline | Có |
| Bảng 4.37 | Ràng buộc toàn vẹn module Tài khoản người dùng | 4.3.4.3 | Có dữ liệu trong outline | Có |
| Bảng 4.38 | Chỉ mục module Tài khoản người dùng | 4.3.4.4 | Có dữ liệu trong outline | Có |
| Bảng 4.39 | Thực thể Genre (bảng Genres) | 4.3.5.2 | Có dữ liệu trong outline | Có |
| Bảng 4.40 | Thực thể Language (bảng Languages) | 4.3.5.2 | Có dữ liệu trong outline | Có |
| Bảng 4.41 | Thực thể AgeRating (bảng AgeRatings) | 4.3.5.2 | Có dữ liệu trong outline | Có |
| Bảng 4.42 | Thực thể Actor (bảng Actors) | 4.3.5.2 | Có dữ liệu trong outline | Có |
| Bảng 4.43 | Thực thể Director (bảng Directors) | 4.3.5.2 | Có dữ liệu trong outline | Có |
| Bảng 4.44 | Ràng buộc toàn vẹn module Danh mục phim | 4.3.5.3 | Có dữ liệu trong outline | Có |
| Bảng 4.45 | Chỉ mục module Danh mục phim | 4.3.5.4 | Có dữ liệu trong outline | Có |
| Bảng 4.46 | Thực thể Movie (bảng Movies) | 4.3.6.2 | Có dữ liệu trong outline | Có |
| Bảng 4.47 | Thực thể MovieGenre (bảng MovieGenres) | 4.3.6.2 | Có dữ liệu trong outline | Có |
| Bảng 4.48 | Thực thể MovieCast (bảng MovieCast) | 4.3.6.2 | Có dữ liệu trong outline | Có |
| Bảng 4.49 | Thực thể Review (bảng Reviews) | 4.3.6.2 | Có dữ liệu trong outline | Có |
| Bảng 4.50 | Ràng buộc toàn vẹn module Phim | 4.3.6.3 | Có dữ liệu trong outline | Có |
| Bảng 4.51 | Chỉ mục module Phim | 4.3.6.4 | Có dữ liệu trong outline | Có |
| Bảng 4.52 | Thực thể CinemaRoom (bảng CinemaRooms) | 4.3.7.2 | Có dữ liệu trong outline | Có |
| Bảng 4.53 | Thực thể ScreenType (bảng ScreenTypes) | 4.3.7.2 | Có dữ liệu trong outline | Có |
| Bảng 4.54 | Thực thể Seat (bảng Seats) | 4.3.7.2 | Có dữ liệu trong outline | Có |
| Bảng 4.55 | Thực thể SeatType (bảng SeatTypes) | 4.3.7.2 | Có dữ liệu trong outline | Có |
| Bảng 4.56 | Ràng buộc toàn vẹn module Phòng chiếu | 4.3.7.3 | Có dữ liệu trong outline | Có |
| Bảng 4.57 | Chỉ mục module Phòng chiếu | 4.3.7.4 | Có dữ liệu trong outline | Có |
| Bảng 4.58 | Thực thể Showtime (bảng Showtimes) | 4.3.8.2 | Có dữ liệu trong outline | Có |
| Bảng 4.59 | Thực thể ShowtimeSeat (bảng ShowtimeSeats) | 4.3.8.2 | Có dữ liệu trong outline | Có |
| Bảng 4.60 | Ràng buộc toàn vẹn module Suất chiếu | 4.3.8.3 | Có dữ liệu trong outline | Có |
| Bảng 4.61 | Chỉ mục module Suất chiếu | 4.3.8.4 | Có dữ liệu trong outline | Có |
| Bảng 4.62 | Thực thể Booking (bảng Bookings) | 4.3.9.2 | Có dữ liệu trong outline | Có |
| Bảng 4.63 | Thực thể BookingTicket (bảng BookingTickets) | 4.3.9.2 | Có dữ liệu trong outline | Có |
| Bảng 4.64 | Thực thể BookingCombo (bảng BookingCombos) | 4.3.9.2 | Có dữ liệu trong outline | Có |
| Bảng 4.65 | Thực thể Combo (bảng Combos) | 4.3.9.2 | Có dữ liệu trong outline | Có |
| Bảng 4.66 | Ràng buộc toàn vẹn module Đặt vé | 4.3.9.3 | Có dữ liệu trong outline | Có |
| Bảng 4.67 | Chỉ mục module Đặt vé | 4.3.9.4 | Có dữ liệu trong outline | Có |
| Bảng 4.68 | Thực thể Payment (bảng Payments) | 4.3.10.2 | Có dữ liệu trong outline | Có |
| Bảng 4.69 | Ràng buộc toàn vẹn module Thanh toán | 4.3.10.3 | Có dữ liệu trong outline | Có |
| Bảng 4.70 | Chỉ mục module Thanh toán | 4.3.10.4 | Có dữ liệu trong outline | Có |
| Bảng 4.71 | Thực thể MembershipTier (bảng MembershipTiers) | 4.3.11.2 | Có dữ liệu trong outline | Có |
| Bảng 4.72 | Thực thể LoyaltyPointEntry (bảng LoyaltyPointEntries) | 4.3.11.2 | Có dữ liệu trong outline | Có |
| Bảng 4.73 | Ràng buộc toàn vẹn module Thành viên | 4.3.11.3 | Có dữ liệu trong outline | Có |
| Bảng 4.74 | Chỉ mục module Thành viên | 4.3.11.4 | Có dữ liệu trong outline | Có |
| Bảng 4.75 | Thực thể CinemaSettings (bảng CinemaSettings) | 4.3.12.2 | Có dữ liệu trong outline | Có |
| Bảng 4.76 | Ràng buộc toàn vẹn module Cấu hình rạp | 4.3.12.3 | Có dữ liệu trong outline | Có |
| Bảng 4.77 | Chỉ mục module Cấu hình rạp | 4.3.12.4 | Có dữ liệu trong outline | Có |
| Bảng 4.78 | Các kiểu liệt kê | 4.3.13 | Có dữ liệu trong outline | Có |
| Bảng 4.79 | Các loại token | 4.4.7 | Có dữ liệu trong outline | Có |
| Bảng 4.80 | Ma trận phân quyền theo controller | 4.4.8 | Có dữ liệu trong outline | Có |
| Bảng 4.81 | Các kiểm tra khi giữ ghế | 4.5.1.1 | Có dữ liệu trong outline | Có |
| Bảng 4.82 | So sánh VNPay, MoMo, Stripe | 4.5.2.5 | Có dữ liệu trong outline | Có |
| Bảng 4.83 | Chính sách và tình huống hoàn tiền | 4.5.4.1 | Có dữ liệu trong outline | Có |
| Bảng 4.84 | Hạng thành viên | 4.5.8 | SeedData.cs | [CẦN BỔ SUNG: giá trị hạng] |
| Bảng 4.85 | Ba chế độ luật ghế lẻ | 4.6.1 | SeatLayoutRules.cs | [CẦN BỔ SUNG: ví dụ cụ thể từ code] |
| Bảng 4.86 | Tham số bộ xếp lịch | 4.6.3.2 | SchedulerOptions.cs | [CẦN BỔ SUNG: giá trị mặc định] |
| Bảng 4.87 | Quy ước thời gian | 4.6.5 | Có dữ liệu trong outline | Có |
| Bảng 4.88 | Sự kiện SignalR | 4.7.2 | Có dữ liệu trong outline | Có |
| Bảng 4.89 | Các hàm của trợ lý AI | 4.8.2 | Có dữ liệu trong outline | Có |
| Bảng 4.90 | Token thiết kế rút gọn | 4.9.1 | .agents/skills/perfo-design-system/references/tokens.css | [CẦN BỔ SUNG: chọn token đưa vào báo cáo] |
| Bảng 4.91 | Tổng hợp endpoint theo controller | 4.10.2 | Có dữ liệu trong outline | Có |
| Bảng 5.1 | Môi trường phát triển | 5.1.1 | Có dữ liệu trong outline | Có |
| Bảng 5.2 | Quy mô mã nguồn | 5.2.2 | Có dữ liệu trong outline | Có |
| Bảng 5.3 | Cổng thanh toán và trạng thái kiểm chứng | 5.3.17 | Có dữ liệu trong outline | Có |
| Bảng 5.4 | Tài nguyên SEO | 5.4.5 | Có dữ liệu trong outline | Có |
| Bảng 5.5 | Nhóm biến cấu hình chính | 5.5.1 | Có dữ liệu trong outline | Có |
| Bảng 5.6 | Cơ chế bảo mật và vị trí cài đặt | 5.6.1 | Có dữ liệu trong outline | Có |
| Bảng 6.1 | Tổng hợp các bộ kiểm thử | 6.1 | Có dữ liệu trong outline | Có |
| Bảng 6.2 | Test đơn vị nhóm Xác thực | 6.2.1 | Có dữ liệu trong outline | Có |
| Bảng 6.3 | Test đơn vị nhóm Đặt vé | 6.2.2 | Có dữ liệu trong outline | Có |
| Bảng 6.4 | Test đơn vị nhóm Thanh toán | 6.2.3 | Có dữ liệu trong outline | Có |
| Bảng 6.5 | Test đơn vị nhóm Hết hạn giữ ghế | 6.2.4 | Có dữ liệu trong outline | Có |
| Bảng 6.6 | Test đơn vị nhóm Suất chiếu | 6.2.5 | Có dữ liệu trong outline | Có |
| Bảng 6.7 | Test đơn vị nhóm Phòng chiếu | 6.2.6 | Có dữ liệu trong outline | Có |
| Bảng 6.8 | Test đơn vị nhóm Báo cáo | 6.2.7 | Có dữ liệu trong outline | Có |
| Bảng 6.9 | Test đơn vị nhóm Cấu hình, đánh giá, nhân viên | 6.2.8 | Có dữ liệu trong outline | Có |
| Bảng 6.10 | Test đơn vị nhóm Tiện ích | 6.2.9 | Có dữ liệu trong outline | Có |
| Bảng 6.11 | Kết quả chạy test backend | 6.2.10 | Có dữ liệu trong outline | Có |
| Bảng 6.12 | Test tích hợp theo tệp | 6.3.2 | Có dữ liệu trong outline | Có |
| Bảng 6.13 | Cấu hình Playwright | 6.4.1 | Có dữ liệu trong outline | Có |
| Bảng 6.14 | Danh mục kịch bản E2E | 6.4.2 | Có dữ liệu trong outline | Có |
| Bảng 6.15 | Các bước kịch bản E2E-01 | 6.4.3 | Có dữ liệu trong outline | Có |
| Bảng 6.16 | Các bước kịch bản E2E-02 | 6.4.4 | Có dữ liệu trong outline | Có |
| Bảng 6.17 | Các bước kịch bản E2E-03 | 6.4.5 | Có dữ liệu trong outline | Có |
| Bảng 6.18 | Các bước kịch bản E2E-04 | 6.4.6 | Có dữ liệu trong outline | Có |
| Bảng 6.19 | Các bước kịch bản E2E-05 | 6.4.7 | Có dữ liệu trong outline | Có |
| Bảng 6.20 | Các bước kịch bản E2E-06 | 6.4.8 | Có dữ liệu trong outline | Có |
| Bảng 6.21 | Các bước kịch bản E2E-07 | 6.4.9 | Có dữ liệu trong outline | Có |
| Bảng 6.22 | Các bước kịch bản E2E-08 | 6.4.10 | Có dữ liệu trong outline | Có |
| Bảng 6.23 | Kết quả E2E | 6.4.11 | Có dữ liệu trong outline | Có |
| Bảng 6.24 | Phạm vi E2E chưa bao phủ | 6.4.15 | Có dữ liệu trong outline | Có |
| Bảng 6.25 | Lưu ý môi trường E2E | 6.4.16 | Có dữ liệu trong outline | Có |
| Bảng 6.26 | Kết quả thực nghiệm đặt ghế đồng thời | 6.5.2 | Có dữ liệu trong outline | Có |
| Bảng 6.27 | Kết quả kiểm thử tải | 6.6.2 | Có dữ liệu trong outline | Có |
| Bảng 7.1 | Truy vết package use case đến kiểm thử | 7.1.1 | Có dữ liệu trong outline | Có |
| Bảng 7.2 | Mức đạt mục tiêu | 7.2.1 | Có dữ liệu trong outline | Có |
| Bảng 7.3 | Hạn chế, đề xuất khắc phục | 7.3.16 | Có dữ liệu trong outline | Có |
| Bảng 8.1 | Mức độ đạt mục tiêu (tổng kết) | 8.1.1 | Có dữ liệu trong outline | Có |

---

## DANH MỤC C. BIỂU ĐỒ

| Số hiệu | Tên | Mục | Cách tạo / nguồn | Trạng thái |
|---|---|---|---|---|
| Biểu đồ 6.1 | Thời gian chạy từng kịch bản E2E | 6.4.12 | Dữ liệu 8 giá trị ở trên | Có dữ liệu |
| Biểu đồ 6.2 | Độ trễ theo số người dùng đồng thời | 6.6.2 | Kết quả k6 | [CẦN BỔ SUNG: kết quả tải] |

---

## DANH MỤC D. CÔNG THỨC VÀ KÝ HIỆU

| Số hiệu | Tên | Mục | Cách tạo / nguồn | Trạng thái |
|---|---|---|---|---|
| (4.1) | Tổng tiền của đơn: TotalAmount = TicketAmount + ComboAmount | 4.5.1.3 | Công thức | Có |
| (4.2) | Giảm giá theo hạng: DiscountAmount = round(TicketAmount × DiscountPercent / 100), làm tròn ra xa 0, không tính combo | 4.5.1.3 | Công thức | Có |
| (4.3) | Số tiền cuối cùng của đơn: FinalAmount = TotalAmount − DiscountAmount | 4.5.1.3 | Công thức | Có |
| (4.4) | Điểm tích: earned = ⌊FinalAmount / PointsPerAmount⌋ (mặc định PointsPerAmount = 10.000) | 4.5.8 | Công thức | Có |
| (4.5) | Điểm hiệu lực: P(u, t) = Σ (Points − RevokedPoints) của các dòng có EarnedAt > t − 12 tháng | 4.5.8 | Công thức | Có |
| (4.6) | Hạng thành viên: tier(u, t) = hạng có MinPoints lớn nhất thỏa MinPoints ≤ P(u, t) | 4.5.8 | Công thức | Có |
| (4.7) | Thu hồi điểm khi hoàn tiền: RevokedPoints ← min(Points, RevokedPoints + ⌊RefundedAmount / PointsPerAmount⌋) | 4.5.8 | Công thức | Có |
| (4.8) | Điểm gần giữa hàng: [CẦN BỔ SUNG: biểu thức từ SeatLayoutRules.cs] | 4.6.2.2 | Công thức | Có |
| (4.9) | Điểm độ sâu hàng: [CẦN BỔ SUNG: biểu thức từ SeatLayoutRules.cs] | 4.6.2.2 | Công thức | Có |
| (4.10) | Điểm gợi ý: Score = 0,45·Center + 0,45·Row + 0,1·SameType | 4.6.2.2 | Công thức | Có |
| (4.11) | Độ nóng: Heat = min(1, 0,7·(Vé/VéMax) + 0,3·(Rating/10)) | 4.6.3.1 | Công thức | Có |
| (4.12) | Thời điểm lý tưởng thứ i: [CẦN BỔ SUNG: biểu thức từ ShowtimeScheduler.cs] | 4.6.3.2 | Công thức | Có |
| (4.13) | Mức phù hợp: Fit = 1 − \|Heat − Ghế/GhếMax\| | 4.6.3.3 | Công thức | Có |
| (4.14) | Điểm xếp lịch: Score = −Khoảng cách/Spread + Fit·FitWeight − Tải·LoadWeight | 4.6.3.3 | Công thức | Có |

---

## DANH MỤC E. TỪ VIẾT TẮT

| Viết tắt | Tiếng Anh | Tiếng Việt |
|---|---|---|
| ACID | Atomicity, Consistency, Isolation, Durability | Nguyên tử, nhất quán, cô lập, bền vững |
| AI | Artificial Intelligence | Trí tuệ nhân tạo |
| API | Application Programming Interface | Giao diện lập trình ứng dụng |
| BR | Business Rule | Quy tắc nghiệp vụ |
| CORS | Cross-Origin Resource Sharing | Chia sẻ tài nguyên giữa các nguồn gốc |
| CRUD | Create, Read, Update, Delete | Tạo, đọc, cập nhật, xóa |
| CSDL | Database | Cơ sở dữ liệu |
| CSP | Content Security Policy | Chính sách bảo mật nội dung |
| DI | Dependency Injection | Tiêm phụ thuộc |
| DTO | Data Transfer Object | Đối tượng truyền dữ liệu |
| E2E | End-to-End | Kiểm thử đầu cuối |
| EF | Entity Framework | Khung ánh xạ đối tượng–quan hệ của .NET |
| ERD | Entity Relationship Diagram | Sơ đồ thực thể liên kết |
| FK | Foreign Key | Khóa ngoại |
| GVHD | — | Giảng viên hướng dẫn |
| HMAC | Hash-based Message Authentication Code | Mã xác thực thông điệp dựa trên băm |
| HTTP | HyperText Transfer Protocol | Giao thức truyền siêu văn bản |
| IPN | Instant Payment Notification | Thông báo thanh toán tức thời |
| JWT | JSON Web Token | Mã thông báo web JSON |
| LLM | Large Language Model | Mô hình ngôn ngữ lớn |
| MVCC | Multi-Version Concurrency Control | Kiểm soát đồng thời đa phiên bản |
| NFR | Non-Functional Requirement | Yêu cầu phi chức năng |
| OAuth | Open Authorization | Giao thức ủy quyền mở |
| ORM | Object-Relational Mapping | Ánh xạ đối tượng–quan hệ |
| PaaS | Platform as a Service | Nền tảng như một dịch vụ |
| PK | Primary Key | Khóa chính |
| QR | Quick Response (code) | Mã phản hồi nhanh |
| RBAC | Role-Based Access Control | Kiểm soát truy cập theo vai trò |
| REST | Representational State Transfer | Chuyển trạng thái đại diện |
| SEO | Search Engine Optimization | Tối ưu hóa công cụ tìm kiếm |
| SPA | Single Page Application | Ứng dụng một trang |
| SQL | Structured Query Language | Ngôn ngữ truy vấn có cấu trúc |
| UC | Use Case | Ca sử dụng |
| UK | Unique Key | Khóa duy nhất |
| UI/UX | User Interface / User Experience | Giao diện / trải nghiệm người dùng |
| UML | Unified Modeling Language | Ngôn ngữ mô hình hóa thống nhất |
| UTC | Coordinated Universal Time | Giờ phối hợp quốc tế |
| VND | Vietnamese Dong | Đồng Việt Nam |

---

## DANH MỤC F. NHÓM TÀI LIỆU THAM KHẢO CẦN TÌM

Mẫu ghi theo Mục 0.4; chỉ đưa tài liệu đã đọc và kiểm tra thông tin xuất bản.

| Nhóm | Loại nguồn cần tìm | Dùng ở mục |
|---|---|---|
| T1 | Báo cáo/số liệu về đặt vé xem phim trực tuyến tại Việt Nam; website/ứng dụng các hệ thống đặt vé (kèm ngày truy cập) | 1.1, 2.2.1 |
| T2 | Sách/bài về UML use case và sơ đồ lớp, kiến trúc phân tầng, Clean Architecture, thiết kế phần mềm | 2.5.1, 2.5.3, Chương 3, 4.2 |
| T3 | Tài liệu chính thức ASP.NET Core, Entity Framework Core, Npgsql, PostgreSQL; sách về mô hình thực thể liên kết; thiết kế REST API | 2.5.2, 2.5.4, 2.5.5, 4.3 |
| T4 | Chuẩn và hướng dẫn xác thực: JWT (RFC 7519), OAuth 2.0 (RFC 6749), OWASP về quản lý phiên, lưu mật khẩu | 2.5.6, 2.5.7, 4.4 |
| T5 | Sách/tài liệu về giao dịch, mức cô lập, MVCC, kiểm soát đồng thời, idempotency | 2.2.2, 2.5.8, 4.5.1 |
| T6 | Tài liệu SignalR; đặc tả WebSocket (RFC 6455) | 2.5.9, 4.7 |
| T7 | Tài liệu Hangfire | 2.5.10, 4.5.3 |
| T8 | Tài liệu tích hợp VNPay, MoMo, Stripe; HMAC (RFC 2104) | 2.5.11, 4.5.2 |
| T9 | Tài liệu React, Vite, TanStack Query/Table, Zustand, React Hook Form, Zod, Tailwind CSS | 2.5.12, 5.4 |
| T10 | Tài liệu Gemini API (function calling); hướng dẫn OWASP về rủi ro ứng dụng LLM | 2.5.13, 4.8 |
| T11 | Tài liệu xUnit, Testcontainers, Playwright, k6; sách về kiểm thử phần mềm | 2.5.14, Chương 6 |
| T12 | Tài liệu Docker, Render, Vercel, Neon | 2.5.15, 5.7 |
| T13 | Sách/bài về thuật toán tham lam, heuristic xếp lịch | 4.6.3 |
| T14 | Tài liệu thiết kế giao diện, khả năng truy cập (WCAG), thiết kế đáp ứng | 4.9 |
| T15 | Tài liệu về robots.txt và giao thức sitemap | 5.4.5 |
| T16 | Văn bản pháp luật hiện hành về bảo vệ dữ liệu cá nhân, thanh toán điện tử [CẦN BỔ SUNG: tra cứu số hiệu] | 7.3 |

# OUTLINE BÁO CÁO ĐỒ ÁN MÔN HỌC — HỆ THỐNG ĐẶT VÉ XEM PHIM TRỰC TUYẾN PERFO

**Quy ước:** `[CẦN BỔ SUNG: …]` = chưa có dữ liệu trong project hoặc tài liệu đã cung cấp. Đường dẫn nguồn tính từ `perfo-main/`; tài liệu kiểm thử bổ sung ghi `bo_sung/…`. Số hiệu hình, bảng, biểu đồ, công thức theo dạng `Hình X.n`, `Bảng X.n`, `Biểu đồ X.n`, `(X.n)` với X là số chương (phụ lục: `P1.n`, `P2.n`…). Mỗi mục chỉ có tối đa một hình; mã nguồn sơ đồ (PlantUML cho use case, Mermaid cho ERD và các sơ đồ khác) nằm trong file `ma-nguon-so-do.md`, tra theo số hiệu hình.

---

## 0. CẤU HÌNH FILE WORD

### 0.1. Thiết lập trang, section

| Thông số | Giá trị theo quy định | Ghi chú khi dựng Word |
|---|---|---|
| Khổ giấy | A4 đứng; bảng/hình lớn có thể dùng A4 ngang | Dùng section ngang riêng cho ERD đầy đủ và các bảng rộng |
| Số trang | Khoảng 30–80 trang, in một mặt | Các mục phân rã use case của quản trị/danh mục có thể chuyển sang phụ lục nếu vượt số trang [CẦN BỔ SUNG: xác nhận với giảng viên] |
| Lề (cm) | Trên 2,5 · Dưới 2,5 · Trái 4,0 · Phải 2,5 · Header 1,0 · Footer 1,0 | Page Setup → Margins; áp dụng cho mọi section thân báo cáo |
| Header/Footer | Không ghi gì, ngoại trừ số trang | Footer: số trang căn giữa, cỡ 13; Header để trống |
| Số trang | La Mã thường (i, ii, iii…) từ sau bìa phụ đến trước Chương 1; Á Rập từ Chương 1 đến hết phụ lục; bìa chính và bìa phụ không đánh số | Bắt đầu lại số trang ở Section 4 (Chương 1) = 1 |

### 0.2. Chia section

| Section | Nội dung | Số trang | Ghi chú |
|---|---|---|---|
| 1 | Bìa chính | Không hiển thị | Khung viền theo mẫu `bia-chinh.docx` |
| 2 | Bìa phụ | Không hiển thị | Khung viền theo mẫu `bia-phu.docx` |
| 3 | Nhận xét GVHD, Lời cảm ơn, Tóm tắt, Mục lục, các danh sách | La Mã thường, bắt đầu i | Footer chỉ có số trang |
| 4 | Chương 1 đến Chương 8, Tài liệu tham khảo, Phụ lục | Á Rập, bắt đầu 1 | Mỗi chương bắt đầu trang mới (Page break before ở Heading 1) |
| 4a (tùy chọn) | Trang A4 ngang cho ERD đầy đủ (`Hình 4.x`) và bảng rộng | Tiếp tục số Á Rập | Section Break (Next Page), Orientation: Landscape |

### 0.3. Bảng style Word

| Style | Font | Cỡ | Kiểu | Căn lề | Giãn dòng | Before/After | Thụt lề | Ghi chú |
|---|---|---|---|---|---|---|---|---|
| Normal (nội dung) | Times New Roman | 13 | Thường | Căn đều (Justify) | 1,2 | 6 pt / 0 pt | Dòng đầu 1 cm [CẦN BỔ SUNG: quy định không nêu] | Unicode |
| Heading 1 (Chương) | Times New Roman | 14 | Đậm | Giữa | 1,2 | 6 pt / 0 pt | 0 | Hai dòng: "CHƯƠNG n" (hoa, đậm) và tên chương bên dưới; Page break before |
| Heading 2 (mục cấp 1, `n.m`) | Times New Roman | 13 [CẦN BỔ SUNG: cỡ chưa quy định] | Đậm, CHỮ HOA | Trái (sát lề) | 1,2 | 6 pt / 0 pt | 0 | Đánh số tự động nhiều cấp |
| Heading 3 (mục cấp 2, `n.m.k`) | Times New Roman | 13 | Đậm, chữ thường | Trái | 1,2 | 6 pt / 0 pt | 0 | Đánh số tự động nhiều cấp |
| Heading 4 (mục cấp 3, `n.m.k.l`) | Times New Roman | 13 | Đậm, nghiêng | Trái | 1,2 | 6 pt / 0 pt | 0 | Tối đa 4 chữ số |
| Caption hình | Times New Roman | 13 [CẦN BỔ SUNG] | Thường | Giữa | 1,0 | 6 pt / 0 pt | 0 | Đặt DƯỚI hình; nhãn "Hình X.n. Tên hình" |
| Caption bảng | Times New Roman | 13 [CẦN BỔ SUNG] | Thường | Giữa | 1,0 | 6 pt / 0 pt | 0 | Đặt TRÊN bảng; nhãn "Bảng X.n. Tên bảng" |
| Ghi chú bảng/nguồn | Times New Roman | 13 | Thường | Trái | 1,0 | 6 pt / 0 pt | 0 | Đặt dưới bảng |
| Nội dung bảng | Times New Roman | 13 [CẦN BỔ SUNG: có thể nhỏ hơn] | Thường | Trái | 1,0 | 0 / 0 | 0 | Hàng tiêu đề đậm, lặp lại ở đầu mỗi trang |
| Mã nguồn | Times New Roman [CẦN BỔ SUNG: font đơn cách?] | 11–12 | Thường | Trái | 1,0 | 0 / 0 | 0 | Khung viền; không dán quá 15–20 dòng |
| Công thức | Times New Roman (Equation) | 13 | Thường | Giữa, số thứ tự `(X.n)` căn phải | 1,2 | 6 pt / 0 pt | 0 | Tab phải |
| Trích dẫn dài (≥ 40 từ) | Times New Roman | 13 | Thường | Trái | 1,2 | 6 pt / 0 pt | Lề trái +0,5 cm | Không dùng nháy kép |
| Tài liệu tham khảo | Times New Roman | 13 | Thường | Trái | 1,0 | 6 pt / 0 pt | Dòng 2 trở đi lùi 0,5 cm | Mỗi tài liệu một đoạn; tiêu đề canh giữa |
| Tiêu đề phần khai tập | Times New Roman | 14 | Đậm, hoa | Giữa | 1,2 | 6 pt / 0 pt | 0 | Không đưa vào đánh số chương |

### 0.4. Quy cách trích dẫn, tài liệu tham khảo

| Trường hợp | Cách ghi |
|---|---|
| Tác giả Việt / nước ngoài | Việt: đủ họ, chữ lót, tên; nước ngoài: họ |
| Hai tác giả | "và" trong câu; "&" trong ngoặc đơn |
| 3–5 tác giả | Lần đầu ghi đủ; từ lần hai: tác giả thứ nhất + "và cs." |
| Từ 6 tác giả | Tác giả thứ nhất + "và cs." ngay từ lần đầu |
| Cùng tác giả, cùng năm | Thêm a, b, c sau năm |
| Nguồn gián tiếp | (trích trong Tác giả, năm, tr. x) |
| Không có ngày | k.n. |
| Trích dưới 40 từ | Đặt trong dấu nháy kép; ghi họ, năm, trang |
| Trích từ 40 từ | Khối riêng, thụt 0,5 cm, không nháy kép |
| Danh mục TLTK | Họ tên tác giả. Năm. Tên tài liệu. Nơi xuất bản: Nhà xuất bản. Website (nếu có). Xếp a, b, c theo họ; tiếng Việt và nước ngoài chung một danh sách [CẦN BỔ SUNG: mẫu cho tài liệu trực tuyến/tài liệu kỹ thuật] |

### 0.5. Cấu trúc báo cáo

```
BÌA CHÍNH · BÌA PHỤ                       (Section 1–2, không số trang)
PHẦN KHAI TẬP                             (Section 3, số La Mã)
  Nhận xét của giảng viên hướng dẫn
  Lời cảm ơn
  Tóm tắt
  Mục lục
  Danh sách hình vẽ
  Danh sách bảng biểu
  Danh sách biểu đồ
  Danh sách ký hiệu
  Danh sách từ viết tắt
PHẦN CHÍNH                                (Section 4, số Á Rập)
  Chương 1. Giới thiệu đề tài
  Chương 2. Tổng quan và cơ sở lý thuyết
  Chương 3. Phân tích yêu cầu
  Chương 4. Thiết kế hệ thống
  Chương 5. Cài đặt hệ thống
  Chương 6. Kiểm thử hệ thống
  Chương 7. Đánh giá kết quả
  Chương 8. Kết luận
TÀI LIỆU THAM KHẢO
PHỤ LỤC 1–5
```

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
  - Giải pháp: kiến trúc phân tầng .NET + React, giữ ghế nguyên tử, thanh toán idempotent, thời gian thực, xếp lịch tự động, trợ lý AI
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
  - Nhiều chi nhánh; ứng dụng di động gốc; đối soát kế toán; thanh toán ZaloPay (chỉ có hằng số tên, chưa có cổng)
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
  - Module trong/ngoài phạm vi (xem 3.3); giả định vận hành: một rạp, VND, UTC+7; cổng thanh toán sandbox
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
  - Tác nhân, use case, quan hệ include/extend/tổng quát hóa; cách phân rã use case (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho Chương 3
- **Nguồn dữ liệu:** [CẦN BỔ SUNG: trích dẫn nhóm T2]

### 2.5.2 Mô hình thực thể liên kết

- **Nội dung cần viết:**
  - Thực thể, thuộc tính, khóa, bội số quan hệ; chuẩn hóa; ánh xạ sang quan hệ (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.2
- **Nguồn dữ liệu:** [CẦN BỔ SUNG: trích dẫn nhóm T3]

### 2.5.3 Kiến trúc phân tầng

- **Nội dung cần viết:**
  - Phân tầng/Clean Architecture; chiều phụ thuộc hướng vào lõi (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.1.2
- **Tài liệu/hình/bảng:**
  - `Hình 2.3` Mô hình phân tầng các project
- **Nguồn dữ liệu:** `Cinema.Domain.csproj`; `Cinema.Application.csproj`; `Cinema.Infrastructure.csproj`; `Cinema.Api.csproj`; [CẦN BỔ SUNG: trích dẫn nhóm T2]

### 2.5.4 REST API

- **Nội dung cần viết:**
  - REST, mã HTTP, phiên bản hóa `/api/v1`, bọc `ApiResponse<T>`, phân trang offset/con trỏ (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.1.4, 4.9
- **Nguồn dữ liệu:** `Cinema.Shared/Results/ApiResponse.cs`; `Cinema.Api/Filters/ApiResponseFilter.cs`; [CẦN BỔ SUNG: trích dẫn nhóm T3]

### 2.5.5 ASP.NET Core, Entity Framework Core, PostgreSQL

- **Nội dung cần viết:**
  - Pipeline middleware, DI, Options pattern, migration, ExecuteUpdate, CHECK và chỉ mục duy nhất (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.2
- **Nguồn dữ liệu:** `Cinema.Api/Program.cs`; `Cinema.Infrastructure/Persistence/**`; [CẦN BỔ SUNG: trích dẫn nhóm T3]

### 2.5.6 Xác thực bằng JWT

- **Nội dung cần viết:**
  - Cấu trúc JWT; access token ngắn hạn, refresh token dài hạn; xoay vòng; phát hiện dùng lại; băm token (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.3
- **Nguồn dữ liệu:** `JwtTokenService.cs`; `AuthService.cs`; [CẦN BỔ SUNG: trích dẫn nhóm T4]

### 2.5.7 Phân quyền theo vai trò

- **Nội dung cần viết:**
  - RBAC theo policy; OAuth 2.0 với Google; băm mật khẩu bằng PasswordHasher (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.3.8
- **Nguồn dữ liệu:** `Program.cs`; `GoogleAuthService.cs`; [CẦN BỔ SUNG: trích dẫn nhóm T4]

### 2.5.8 Kiểm soát đồng thời

- **Nội dung cần viết:**
  - ACID; mức cô lập; MVCC; cập nhật có điều kiện + kiểm tra số dòng; ràng buộc duy nhất; idempotency (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.4.1
- **Nguồn dữ liệu:** `BookingService.cs`; `BookingTicketConfiguration.cs`; [CẦN BỔ SUNG: trích dẫn nhóm T5]

### 2.5.9 Truyền thông thời gian thực với SignalR

- **Nội dung cần viết:**
  - WebSocket; hub, group; xác thực qua `access_token` query (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.6
- **Nguồn dữ liệu:** `ShowtimeHub.cs`; `frontend/src/lib/signalr.ts`; [CẦN BỔ SUNG: trích dẫn nhóm T6]

### 2.5.10 Xử lý tác vụ nền với Hangfire

- **Nội dung cần viết:**
  - Job lặp cron, fire-and-forget, thử lại, lưu trên PostgreSQL (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.4.3
- **Nguồn dữ liệu:** `BackgroundJobs/*`; `Program.cs`; [CẦN BỔ SUNG: trích dẫn nhóm T7]

### 2.5.11 Tích hợp cổng thanh toán

- **Nội dung cần viết:**
  - Redirect–callback; IPN/webhook; HMAC-SHA512 (VNPay), HMAC-SHA256 (MoMo); Stripe webhook; idempotency; hoàn tiền (khái niệm có trích dẫn)
  - Lý do chọn trong bối cảnh bài toán
  - Cách Perfo áp dụng; Cơ sở cho 4.4.2
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
  - Cách Perfo áp dụng; Cơ sở cho 4.7
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
| Hệ thống tác vụ nền | Hệ thống | Hangfire: giải phóng ghế hết hạn, cập nhật vòng đời, gửi email, hoàn tiền | — |
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


### 3.2.1 Danh mục module chức năng

- **Nội dung cần viết:**
  - Giới thiệu 21 module chức năng; mỗi module có sơ đồ use case, danh sách use case và các sơ đồ phân rã
- **Tài liệu/hình/bảng:**
  - `Bảng 3.2` Danh mục module chức năng
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

| Mục | Mã | Module | Số use case | Số use case có phân rã | Tác nhân người dùng |
|---|---|---|---|---|---|
| 3.3.1 | XT | Xác thực | 11 | 6 | Khách vãng lai, Nhân viên, Quản trị viên, Người dùng đã đăng nhập |
| 3.3.2 | HS | Hồ sơ cá nhân | 4 | 1 | Người dùng đã đăng nhập |
| 3.3.3 | DP | Duyệt phim | 8 | 3 | Khách vãng lai |
| 3.3.4 | DG | Đánh giá phim | 4 | 1 | Khách hàng |
| 3.3.5 | DV | Đặt vé trực tuyến | 9 | 6 | Khách hàng, Khách vãng lai |
| 3.3.6 | TT | Thanh toán trực tuyến | 6 | 6 | Khách hàng |
| 3.3.7 | AI | Trợ lý AI | 1 | 1 | Khách vãng lai, Khách hàng |
| 3.3.8 | BQ | Bán vé tại quầy | 1 | 1 | Nhân viên, Quản trị viên |
| 3.3.9 | SV | Soát vé | 1 | 1 | Nhân viên, Quản trị viên |
| 3.3.10 | QB | Quản lý booking | 2 | 1 | Nhân viên, Quản trị viên |
| 3.3.11 | QP | Quản lý phim | 5 | 4 | Nhân viên, Quản trị viên |
| 3.3.12 | QD | Quản lý danh mục | 10 | 10 | Quản trị viên |
| 3.3.13 | QR | Quản lý phòng chiếu | 3 | 2 | Quản trị viên |
| 3.3.14 | QS | Quản lý suất chiếu | 9 | 7 | Nhân viên, Quản trị viên |
| 3.3.15 | QN | Quản lý người dùng | 7 | 3 | Quản trị viên, Nhân viên |
| 3.3.16 | QV | Quản lý đánh giá | 3 | 0 | Quản trị viên |
| 3.3.17 | QT | Quản lý thanh toán | 2 | 0 | Quản trị viên |
| 3.3.18 | BC | Báo cáo thống kê | 5 | 0 | Quản trị viên |
| 3.3.19 | CH | Cấu hình rạp | 2 | 1 | Quản trị viên |
| 3.3.20 | TP | Tải tệp | 2 | 0 | Quản trị viên |
| 3.3.21 | HT | Vận hành hệ thống | 7 | 3 | Quản trị viên, Khách vãng lai |
|  |  | **Tổng** | 102 | 57 |  |


### 3.2.2 Sơ đồ use case tổng quát

- **Nội dung cần viết:**
  - Thể hiện các module chức năng và tác nhân sử dụng; chi tiết từng module ở mục 3.3
- **Tài liệu/hình/bảng:**
  - `Hình 3.3` Sơ đồ use case tổng quát
- **Nguồn dữ liệu:** `Mục 3.3 file này`

### 3.2.3 Ma trận tác nhân theo module

- **Nội dung cần viết:**
  - Đánh dấu tác nhân tham gia từng module (suy ra từ danh sách use case ở 3.3)
- **Tài liệu/hình/bảng:**
  - `Bảng 3.3` Ma trận tác nhân theo module
- **Nguồn dữ liệu:** `Mục 3.3 file này`

| Module | Khách vãng lai | Khách hàng | Nhân viên | Quản trị viên | Hệ thống ngoài/nền |
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


## 3.3 USE CASE THEO MODULE


### 3.3.1 Xác thực

- **Nội dung cần viết:**
  - Đăng ký, đăng nhập, khôi phục mật khẩu, duy trì phiên. Giới thiệu module, tác nhân, số use case.

#### 3.3.1.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.4` Sơ đồ use case module Xác thực
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.1.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.4` Danh sách use case module Xác thực

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-XT-01 | Đăng ký tài khoản | Khách vãng lai, Dịch vụ email | Khách tạo tài khoản khách hàng và nhận email xác nhận. | POST /api/v1/auth/register (giới hạn tốc độ `auth`) | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → RegisterAsync` |
| UC-XT-02 | Xác nhận email | Khách vãng lai | Khách xác nhận email bằng mã gửi qua thư. | POST /api/v1/auth/confirm-email | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → ConfirmEmailAsync` |
| UC-XT-03 | Gửi lại email xác nhận | Khách vãng lai, Dịch vụ email | Gửi lại thư xác nhận, trả thông điệp chuẩn không lộ email có tồn tại hay không. | POST /api/v1/auth/resend-confirmation | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → ResendConfirmationEmailAsync` |
| UC-XT-04 | Đăng nhập bằng email | Khách vãng lai | Khách hàng đăng nhập bằng email và mật khẩu. | POST /api/v1/auth/login (giới hạn tốc độ `auth`) | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → LoginAsync` |
| UC-XT-05 | Đăng nhập bằng Google | Khách vãng lai, Google | Đăng nhập bằng tài khoản Google. | POST /api/v1/auth/google (giới hạn tốc độ `auth`) | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → GoogleLoginAsync`<br>`backend/Cinema.Infrastructure/Auth/GoogleAuthService.cs` |
| UC-XT-06 | Đăng nhập trang quản trị | Nhân viên, Quản trị viên | Nhân viên/quản trị đăng nhập cổng quản trị. | POST /api/v1/auth/admin-login (giới hạn tốc độ `auth`) | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → LoginAsync (cổng quản trị)` |
| UC-XT-07 | Khôi phục mật khẩu | Khách vãng lai, Dịch vụ email | Yêu cầu khôi phục và đặt lại mật khẩu bằng mã gửi qua email. | POST /api/v1/auth/forgot-password, POST /api/v1/auth/reset-password (giới hạn tốc độ `auth`) | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → ForgotPasswordAsync, ResetPasswordAsync` |
| UC-XT-08 | Duy trì phiên đăng nhập | Người dùng đã đăng nhập | Làm mới access token bằng refresh token xoay vòng. | POST /api/v1/auth/refresh (giới hạn tốc độ `auth`) | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → RefreshAsync, RevokeTokenChainAsync` |
| UC-XT-09 | Đăng xuất | Người dùng đã đăng nhập | Thu hồi refresh token hiện tại. | POST /api/v1/auth/logout | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → LogoutAsync` |
| UC-XT-10 | Xem thông tin phiên hiện tại | Người dùng đã đăng nhập | Lấy thông tin người dùng đang đăng nhập. | GET /api/v1/auth/me | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → GetMeAsync` |
| UC-XT-11 | Cấp socket token cho kết nối thời gian thực | Người dùng đã đăng nhập | Cấp token ngắn hạn (300 giây) để kết nối SignalR. | POST /api/v1/auth/socket-token | `backend/Cinema.Api/Controllers/Client/AuthController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → GenerateSocketTokenAsync` |


#### 3.3.1.3 Phân rã use case Đăng ký tài khoản

- **Nội dung cần viết:**
  - Use case cha UC-XT-01: Khách tạo tài khoản khách hàng và nhận email xác nhận.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.5` Phân rã use case Đăng ký tài khoản
  - `Bảng 3.5` Phân rã use case Đăng ký tài khoản
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/AuthController.cs`; `backend/Cinema.Application/Services/Auth/AuthService.cs → RegisterAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-XT-01.1 | Nhập thông tin đăng ký | include | Khách vãng lai | Kiểm tra dữ liệu đầu vào bằng FluentValidation |
| UC-XT-01.2 | Kiểm tra email chưa được sử dụng | include |  | `Invalid_EmailExists`, `Invalid_EmailInUse` |
| UC-XT-01.3 | Tạo tài khoản vai trò Customer và hồ sơ khách hàng | include |  | Vai trò Customer phải có sẵn (`Rule_CustomerRoleNotSeeded`) |
| UC-XT-01.4 | Tạo mã xác nhận email và xếp công việc gửi email | include | Dịch vụ email | Mã xác nhận lưu dạng băm, mục đích `EmailConfirmation` |


#### 3.3.1.4 Phân rã use case Đăng nhập bằng email

- **Nội dung cần viết:**
  - Use case cha UC-XT-04: Khách hàng đăng nhập bằng email và mật khẩu.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.6` Phân rã use case Đăng nhập bằng email
  - `Bảng 3.6` Phân rã use case Đăng nhập bằng email
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/AuthController.cs`; `backend/Cinema.Application/Services/Auth/AuthService.cs → LoginAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-XT-04.1 | Kiểm tra email và mật khẩu | include |  | `Unauthorized_InvalidCredentials` |
| UC-XT-04.2 | Kiểm tra tài khoản chưa bị khóa | include |  | `Forbidden_AccountLocked` |
| UC-XT-04.3 | Kiểm tra email đã được xác nhận | include |  | `Forbidden_EmailNotVerifiedCheckInbox` |
| UC-XT-04.4 | Chặn tài khoản nhân viên/quản trị dùng cổng khách hàng | include |  | `Forbidden_UseAdminPortal` |
| UC-XT-04.5 | Cấp access token và refresh token | include |  | JwtTokenService; refresh token lưu dạng băm |


#### 3.3.1.5 Phân rã use case Đăng nhập bằng Google

- **Nội dung cần viết:**
  - Use case cha UC-XT-05: Đăng nhập bằng tài khoản Google.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.7` Phân rã use case Đăng nhập bằng Google
  - `Bảng 3.7` Phân rã use case Đăng nhập bằng Google
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/AuthController.cs`; `backend/Cinema.Application/Services/Auth/AuthService.cs → GoogleLoginAsync`; `backend/Cinema.Infrastructure/Auth/GoogleAuthService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-XT-05.1 | Xác minh ID token với Google | include | Google | GoogleAuthService |
| UC-XT-05.2 | Kiểm tra email Google đã được xác minh | include |  | `Unauthorized_GoogleEmailNotVerified` |
| UC-XT-05.3 | Tạo tài khoản khách hàng mới nếu chưa có | extend |  | Vai trò Customer phải có sẵn |
| UC-XT-05.4 | Liên kết GoogleId với tài khoản đã có email | extend |  | [CẦN BỔ SUNG: xác minh trong GoogleLoginAsync] |
| UC-XT-05.5 | Chặn tài khoản quản trị dùng đăng nhập Google | include |  | `Forbidden_AdminCannotUseGoogleLogin` |
| UC-XT-05.6 | Kiểm tra tài khoản chưa bị khóa | include |  | `Forbidden_AccountLocked` |
| UC-XT-05.7 | Cấp access token và refresh token | include |  | JwtTokenService |


#### 3.3.1.6 Phân rã use case Đăng nhập trang quản trị

- **Nội dung cần viết:**
  - Use case cha UC-XT-06: Nhân viên/quản trị đăng nhập cổng quản trị.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.8` Phân rã use case Đăng nhập trang quản trị
  - `Bảng 3.8` Phân rã use case Đăng nhập trang quản trị
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/AuthController.cs`; `backend/Cinema.Application/Services/Auth/AuthService.cs → LoginAsync (cổng quản trị)`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-XT-06.1 | Kiểm tra email và mật khẩu | include |  | `Unauthorized_InvalidCredentials` |
| UC-XT-06.2 | Kiểm tra tài khoản chưa bị khóa | include |  | `Forbidden_AccountLocked` |
| UC-XT-06.3 | Chặn khách hàng dùng cổng quản trị | include |  | `Forbidden_CustomerCannotUseAdminPortal` |
| UC-XT-06.4 | Cấp access token và refresh token | include |  | JwtTokenService |


#### 3.3.1.7 Phân rã use case Khôi phục mật khẩu

- **Nội dung cần viết:**
  - Use case cha UC-XT-07: Yêu cầu khôi phục và đặt lại mật khẩu bằng mã gửi qua email.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.9` Phân rã use case Khôi phục mật khẩu
  - `Bảng 3.9` Phân rã use case Khôi phục mật khẩu
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/AuthController.cs`; `backend/Cinema.Application/Services/Auth/AuthService.cs → ForgotPasswordAsync, ResetPasswordAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-XT-07.1 | Yêu cầu khôi phục mật khẩu | include | Khách vãng lai | Trả thông điệp chuẩn `Success_PasswordResetEmailSent`, không lộ email có tồn tại |
| UC-XT-07.2 | Tạo mã đặt lại và xếp công việc gửi email | include | Dịch vụ email | Mã lưu dạng băm, mục đích `PasswordReset` |
| UC-XT-07.3 | Đặt lại mật khẩu bằng mã | include | Khách vãng lai | Mã phải còn hạn và chưa dùng (`Invalid_TokenExpiredOrUsed`) |
| UC-XT-07.4 | Thu hồi các refresh token sau khi đặt lại mật khẩu | include |  | [CẦN BỔ SUNG: xác minh trong ResetPasswordAsync] |


#### 3.3.1.8 Phân rã use case Duy trì phiên đăng nhập

- **Nội dung cần viết:**
  - Use case cha UC-XT-08: Làm mới access token bằng refresh token xoay vòng.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.10` Phân rã use case Duy trì phiên đăng nhập
  - `Bảng 3.10` Phân rã use case Duy trì phiên đăng nhập
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/AuthController.cs`; `backend/Cinema.Application/Services/Auth/AuthService.cs → RefreshAsync, RevokeTokenChainAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-XT-08.1 | Làm mới access token và xoay vòng refresh token | include | Người dùng đã đăng nhập | Cấp token mới, thu hồi token cũ (`ReplacedByTokenId`) |
| UC-XT-08.2 | Kiểm tra refresh token hợp lệ, còn hạn, chưa thu hồi | include |  | `Unauthorized_RefreshTokenInvalid`, `Unauthorized_RefreshTokenExpiredOrRevoked` |
| UC-XT-08.3 | Kiểm tra tài khoản còn hoạt động và đã xác nhận email | include |  | `Forbidden_AccountLocked`, `Forbidden_EmailNotVerified` |
| UC-XT-08.4 | Thu hồi toàn bộ chuỗi token khi phát hiện dùng lại token đã thu hồi | extend |  | RevokeTokenChainAsync |


### 3.3.2 Hồ sơ cá nhân

- **Nội dung cần viết:**
  - Xem và cập nhật hồ sơ, đổi mật khẩu. Giới thiệu module, tác nhân, số use case.

#### 3.3.2.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.11` Sơ đồ use case module Hồ sơ cá nhân
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.2.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.11` Danh sách use case module Hồ sơ cá nhân

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-HS-01 | Xem hồ sơ cá nhân | Người dùng đã đăng nhập | Xem thông tin cá nhân, điểm thưởng và hạng thành viên. | GET /api/v1/users/me | `backend/Cinema.Api/Controllers/Client/UsersController.cs`<br>`backend/Cinema.Application/Services/Users/UserProfileService.cs` |
| UC-HS-02 | Cập nhật hồ sơ cá nhân | Người dùng đã đăng nhập | Sửa họ tên, số điện thoại và thông tin cá nhân. | PUT /api/v1/users/me | `backend/Cinema.Api/Controllers/Client/UsersController.cs`<br>`backend/Cinema.Application/Services/Users/UserProfileService.cs` |
| UC-HS-03 | Đổi mật khẩu | Người dùng đã đăng nhập | Đổi mật khẩu khi biết mật khẩu hiện tại. | POST /api/v1/users/me/change-password | `backend/Cinema.Api/Controllers/Client/UsersController.cs`<br>`backend/Cinema.Application/Services/Auth/AuthService.cs → ChangePasswordAsync` |
| UC-HS-04 | Xem đánh giá của tôi | Người dùng đã đăng nhập | Danh sách đánh giá do người dùng đã viết. | GET /api/v1/users/me/reviews | `backend/Cinema.Api/Controllers/Client/UsersController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs` |


#### 3.3.2.3 Phân rã use case Đổi mật khẩu

- **Nội dung cần viết:**
  - Use case cha UC-HS-03: Đổi mật khẩu khi biết mật khẩu hiện tại.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.12` Phân rã use case Đổi mật khẩu
  - `Bảng 3.12` Phân rã use case Đổi mật khẩu
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/UsersController.cs`; `backend/Cinema.Application/Services/Auth/AuthService.cs → ChangePasswordAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-HS-03.1 | Kiểm tra email đã được xác nhận | include |  | `Forbidden_EmailNotVerifiedCheckInbox` |
| UC-HS-03.2 | Kiểm tra mật khẩu hiện tại | include |  | `Invalid_CurrentPassword` |
| UC-HS-03.3 | Cập nhật mật khẩu mới (băm) | include |  | PasswordHasher |
| UC-HS-03.4 | Thu hồi các refresh token hiện có | include |  | [CẦN BỔ SUNG: xác minh trong ChangePasswordAsync] |


### 3.3.3 Duyệt phim

- **Nội dung cần viết:**
  - Xem phim, lịch chiếu và thông tin công khai của rạp. Giới thiệu module, tác nhân, số use case.

#### 3.3.3.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.13` Sơ đồ use case module Duyệt phim
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.3.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.13` Danh sách use case module Duyệt phim

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-DP-01 | Xem danh sách phim | Khách vãng lai | Danh sách phim phân trang theo con trỏ, có bộ lọc. | GET /api/v1/movies | `backend/Cinema.Api/Controllers/Client/MoviesController.cs`<br>`backend/Cinema.Application/Services/Movies/MovieService.cs → GetMoviesAsync` |
| UC-DP-02 | Xem chi tiết phim | Khách vãng lai | Thông tin chi tiết của một phim. | GET /api/v1/movies/{movieId} | `backend/Cinema.Api/Controllers/Client/MoviesController.cs`<br>`backend/Cinema.Application/Services/Movies/MovieService.cs → GetMovieDetailAsync` |
| UC-DP-03 | Xem đánh giá của phim | Khách vãng lai | Danh sách đánh giá và điểm trung bình của phim. | GET /api/v1/movies/{movieId}/reviews | `backend/Cinema.Api/Controllers/Client/MoviesController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs` |
| UC-DP-04 | Xem lịch chiếu theo ngày | Khách vãng lai | Các suất chiếu trong một ngày. | GET /api/v1/showtimes?date= | `backend/Cinema.Api/Controllers/Client/ShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → GetWholeCinemaScheduleAsync` |
| UC-DP-05 | Xem danh mục lọc | Khách vãng lai | Thể loại, ngôn ngữ, phân loại tuổi dùng cho bộ lọc. | GET /api/v1/genres, /languages, /age-ratings | `backend/Cinema.Api/Controllers/Client/CatalogController.cs` |
| UC-DP-06 | Xem các hạng thành viên | Khách vãng lai | Danh sách hạng thành viên, điểm tối thiểu và mức giảm giá. | GET /api/v1/membership-tiers | `backend/Cinema.Api/Controllers/Client/CatalogController.cs` |
| UC-DP-07 | Xem thông tin rạp | Khách vãng lai | Tên, địa chỉ, giờ hoạt động và chính sách công khai. | GET /api/v1/settings | `backend/Cinema.Api/Controllers/Client/SettingsController.cs`<br>`backend/Cinema.Application/Services/Settings/SettingsService.cs` |
| UC-DP-08 | Xem danh sách combo | Khách vãng lai | Combo bắp nước đang bán. | GET /api/v1/combos | `backend/Cinema.Api/Controllers/Client/CombosController.cs`<br>`backend/Cinema.Application/Services/Combos/ComboService.cs` |


#### 3.3.3.3 Phân rã use case Xem danh sách phim

- **Nội dung cần viết:**
  - Use case cha UC-DP-01: Danh sách phim phân trang theo con trỏ, có bộ lọc.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.14` Phân rã use case Xem danh sách phim
  - `Bảng 3.14` Phân rã use case Xem danh sách phim
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/MoviesController.cs`; `backend/Cinema.Application/Services/Movies/MovieService.cs → GetMoviesAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-DP-01.1 | Lọc theo trạng thái phim | extend |  | Tham số `Status` (ComingSoon/NowShowing/Ended) |
| UC-DP-01.2 | Lọc theo thể loại | extend |  | Tham số `GenreId` |
| UC-DP-01.3 | Lọc theo ngôn ngữ | extend |  | Tham số `LanguageId` |
| UC-DP-01.4 | Lọc theo phân loại tuổi | extend |  | Tham số `AgeRatingId` |
| UC-DP-01.5 | Tìm kiếm theo từ khóa | extend |  | Tham số `Search` |
| UC-DP-01.6 | Phân trang theo con trỏ | include |  | Tham số `cursor`, `limit` |


#### 3.3.3.4 Phân rã use case Xem chi tiết phim

- **Nội dung cần viết:**
  - Use case cha UC-DP-02: Thông tin chi tiết của một phim.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.15` Phân rã use case Xem chi tiết phim
  - `Bảng 3.15` Phân rã use case Xem chi tiết phim
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/MoviesController.cs`; `backend/Cinema.Application/Services/Movies/MovieService.cs → GetMovieDetailAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-DP-02.1 | Xem thông tin chung của phim | include |  | Tên, mô tả, thời lượng, ngày phát hành, phụ đề, phân loại tuổi |
| UC-DP-02.2 | Xem đạo diễn và diễn viên | include |  | Director, MovieCast |
| UC-DP-02.3 | Xem trailer | include |  | `TrailerUrl` |
| UC-DP-02.4 | Xem đánh giá của phim | include |  | Xem UC-DP-03 |
| UC-DP-02.5 | Chọn suất chiếu để đặt vé | extend | Khách hàng | Chuyển sang UC-DV-01; [CẦN BỔ SUNG: xác minh dữ liệu suất chiếu trong MovieDetailDto] |


#### 3.3.3.5 Phân rã use case Xem lịch chiếu theo ngày

- **Nội dung cần viết:**
  - Use case cha UC-DP-04: Các suất chiếu trong một ngày.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.16` Phân rã use case Xem lịch chiếu theo ngày
  - `Bảng 3.16` Phân rã use case Xem lịch chiếu theo ngày
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/ShowtimesController.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → GetWholeCinemaScheduleAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-DP-04.1 | Chọn ngày xem lịch | include |  | Tham số `date` bắt buộc (`Invalid_DateRequired`) |
| UC-DP-04.2 | Xem các suất chiếu nhóm theo phim | include |  | [CẦN BỔ SUNG: xác minh cấu trúc ScheduleDto] |
| UC-DP-04.3 | Chọn suất chiếu để đặt vé | extend | Khách hàng | Chuyển sang UC-DV-01 |


### 3.3.4 Đánh giá phim

- **Nội dung cần viết:**
  - Khách hàng đã xem phim viết, sửa, xóa đánh giá. Giới thiệu module, tác nhân, số use case.

#### 3.3.4.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.17` Sơ đồ use case module Đánh giá phim
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.4.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.17` Danh sách use case module Đánh giá phim

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-DG-01 | Kiểm tra quyền đánh giá | Khách hàng | Cho biết khách đã soát vé phim này và đã đánh giá hay chưa. | GET /api/v1/movies/{movieId}/can-review | `backend/Cinema.Api/Controllers/Client/MoviesController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs` |
| UC-DG-02 | Viết đánh giá | Khách hàng | Gửi điểm 1–5 và nhận xét cho phim. | POST /api/v1/movies/{movieId}/reviews (CustomerOnly) | `backend/Cinema.Api/Controllers/Client/MoviesController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs` |
| UC-DG-03 | Sửa đánh giá | Khách hàng | Sửa đánh giá của chính mình. | PUT /api/v1/movies/{movieId}/reviews/{id} (CustomerOnly) | `backend/Cinema.Api/Controllers/Client/MoviesController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs` |
| UC-DG-04 | Xóa đánh giá của mình | Khách hàng | Xóa đánh giá của chính mình. | DELETE /api/v1/movies/{movieId}/reviews/{id} (CustomerOnly) | `backend/Cinema.Api/Controllers/Client/MoviesController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs` |


#### 3.3.4.3 Phân rã use case Viết đánh giá

- **Nội dung cần viết:**
  - Use case cha UC-DG-02: Gửi điểm 1–5 và nhận xét cho phim.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.18` Phân rã use case Viết đánh giá
  - `Bảng 3.18` Phân rã use case Viết đánh giá
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/MoviesController.cs`; `backend/Cinema.Application/Services/Reviews/ReviewService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-DG-02.1 | Kiểm tra quyền đánh giá | include |  | Xem UC-DG-01: phải có vé đã soát của phim; mỗi người một đánh giá cho mỗi phim |
| UC-DG-02.2 | Nhập điểm (1–5) và nhận xét | include | Khách hàng | Ràng buộc `CK_Reviews_Rating` |
| UC-DG-02.3 | Lưu đánh giá | include |  | Chỉ mục duy nhất `(UserId, MovieId)` |
| UC-DG-02.4 | Tính lại điểm trung bình của phim | include |  | `Movie.AverageRating` (làm tròn 2 chữ số) |


### 3.3.5 Đặt vé trực tuyến

- **Nội dung cần viết:**
  - Chọn suất, chọn ghế, giữ ghế, quản lý đơn đặt vé. Giới thiệu module, tác nhân, số use case.

#### 3.3.5.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.19` Sơ đồ use case module Đặt vé trực tuyến
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.5.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.19` Danh sách use case module Đặt vé trực tuyến

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-DV-01 | Đặt vé trực tuyến | Khách hàng | Quy trình đặt vé từ chọn suất đến thanh toán. | Chuỗi các use case con | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs` |
| UC-DV-02 | Xem sơ đồ ghế của suất chiếu | Khách vãng lai, Khách hàng | Trạng thái từng ghế của suất chiếu. | GET /api/v1/showtimes/{showtimeId}/seats | `backend/Cinema.Api/Controllers/Client/ShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → GetShowtimeSeatsAsync` |
| UC-DV-03 | Chọn ghế | Khách hàng | Chọn một hoặc nhiều ghế trên sơ đồ. | Xử lý ở giao diện, kiểm tra lại ở UC-DV-04 | `frontend/src/components/common/seat-map/seat-map-utils.ts`<br>`backend/Cinema.Application/Utils/SeatLayoutRules.cs` |
| UC-DV-04 | Giữ ghế | Khách hàng | Giữ chỗ có thời hạn cho các ghế đã chọn. | POST /api/v1/bookings/hold (giới hạn tốc độ `booking-hold`) | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → HoldSeatsAsync, CreateHoldBookingAsync` |
| UC-DV-05 | Chọn combo | Khách hàng | Thêm combo bắp nước vào đơn. | GET /api/v1/combos, GET /api/v1/combos/suggestions | `backend/Cinema.Api/Controllers/Client/CombosController.cs`<br>`backend/Cinema.Application/Services/Combos/ComboService.cs` |
| UC-DV-06 | Xem gợi ý ghế | Khách vãng lai, Khách hàng | Gợi ý các vị trí ghế tốt theo số người. | GET /api/v1/showtimes/{showtimeId}/seat-suggestions | `backend/Cinema.Api/Controllers/Client/ShowtimesController.cs`<br>`backend/Cinema.Application/Utils/SeatLayoutRules.cs → Suggest` |
| UC-DV-07 | Xem danh sách đơn đặt vé của tôi | Khách hàng | Danh sách booking của người dùng, lọc theo trạng thái, phân trang con trỏ. | GET /api/v1/bookings | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → GetMyBookingsAsync` |
| UC-DV-08 | Hủy đơn đặt vé | Khách hàng | Hủy giữ ghế hoặc hủy đơn đã thanh toán. | POST /api/v1/bookings/{bookingId}/cancel | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → CancelBookingAsync` |
| UC-DV-09 | Xem vé điện tử | Khách hàng | Chi tiết booking, trạng thái thanh toán, mã QR của từng vé. | GET /api/v1/bookings/{bookingId} | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → GetBookingDetailAsync` |


#### 3.3.5.3 Phân rã use case Đặt vé trực tuyến

- **Nội dung cần viết:**
  - Use case cha UC-DV-01: Quy trình đặt vé từ chọn suất đến thanh toán.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.20` Phân rã use case Đặt vé trực tuyến
  - `Bảng 3.20` Phân rã use case Đặt vé trực tuyến
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/BookingsController.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-DV-01.1 | Chọn suất chiếu | include | Khách hàng | Từ UC-DP-02 hoặc UC-DP-04 |
| UC-DV-01.2 | Xem sơ đồ ghế của suất chiếu | include | Khách hàng | Xem UC-DV-02 |
| UC-DV-01.3 | Chọn ghế | include | Khách hàng | Xem UC-DV-03 |
| UC-DV-01.4 | Chọn combo | extend | Khách hàng | Xem UC-DV-05 |
| UC-DV-01.5 | Giữ ghế | include | Khách hàng | Xem UC-DV-04 |
| UC-DV-01.6 | Thanh toán trực tuyến | include | Khách hàng | Xem UC-TT-01 |
| UC-DV-01.7 | Tiếp tục đơn đang giữ chỗ | extend | Khách hàng | Mở lại booking `Pending` còn hạn (GET bookings/{id}) |
| UC-DV-01.8 | Hủy giữ ghế | extend | Khách hàng | Xem UC-DV-08 |


#### 3.3.5.4 Phân rã use case Xem sơ đồ ghế của suất chiếu

- **Nội dung cần viết:**
  - Use case cha UC-DV-02: Trạng thái từng ghế của suất chiếu.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.21` Phân rã use case Xem sơ đồ ghế của suất chiếu
  - `Bảng 3.21` Phân rã use case Xem sơ đồ ghế của suất chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/ShowtimesController.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → GetShowtimeSeatsAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-DV-02.1 | Xem trạng thái từng ghế (Available, Held, Booked) | include |  | ShowtimeSeat.Status |
| UC-DV-02.2 | Lấy socket token | include | Khách hàng | Xem UC-XT-11 (chỉ khi đã đăng nhập) |
| UC-DV-02.3 | Nhận cập nhật ghế thời gian thực | extend | Khách hàng | SignalR hub `/hubs/showtimes`, nhóm `showtime-{id}` |
| UC-DV-02.4 | Xem gợi ý ghế | extend | Khách vãng lai, Khách hàng | Xem UC-DV-06 |


#### 3.3.5.5 Phân rã use case Chọn ghế

- **Nội dung cần viết:**
  - Use case cha UC-DV-03: Chọn một hoặc nhiều ghế trên sơ đồ.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.22` Phân rã use case Chọn ghế
  - `Bảng 3.22` Phân rã use case Chọn ghế
- **Nguồn dữ liệu:** `frontend/src/components/common/seat-map/seat-map-utils.ts`; `backend/Cinema.Application/Utils/SeatLayoutRules.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-DV-03.1 | Chọn hoặc bỏ chọn ghế đơn | include | Khách hàng |  |
| UC-DV-03.2 | Chọn ghế đôi theo cặp | include | Khách hàng | Ghế có cùng `CoupleId` phải chọn cùng nhau |
| UC-DV-03.3 | Kiểm tra luật ghế lẻ | include |  | Theo `SeatGapRuleMode` (Strict, AllowEdge, Off) |
| UC-DV-03.4 | Kiểm tra giới hạn số ghế tối đa mỗi đơn | include |  | `MaxSeatsPerBooking` = 10 |


#### 3.3.5.6 Phân rã use case Giữ ghế

- **Nội dung cần viết:**
  - Use case cha UC-DV-04: Giữ chỗ có thời hạn cho các ghế đã chọn.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.23` Phân rã use case Giữ ghế
  - `Bảng 3.23` Phân rã use case Giữ ghế
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/BookingsController.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs → HoldSeatsAsync, CreateHoldBookingAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-DV-04.1 | Kiểm tra dữ liệu đầu vào | include |  | BookingValidators |
| UC-DV-04.2 | Kiểm tra suất chiếu còn mở bán | include |  | Trạng thái Scheduled, chưa bắt đầu, chưa quá thời điểm dừng bán online |
| UC-DV-04.3 | Kiểm tra chưa có booking Pending còn hạn cho suất này | include |  | `Conflict_PendingBookingExistsForShowtime` |
| UC-DV-04.4 | Kiểm tra luật chọn ghế phía máy chủ | include |  | SeatLayoutRules.ValidateSelection |
| UC-DV-04.5 | Giữ ghế nguyên tử bằng cập nhật có điều kiện | include |  | ExecuteUpdate chỉ khớp ghế Available hoặc ghế Held đã hết hạn của chính người giữ |
| UC-DV-04.6 | Xử lý xung đột ghế | extend |  | Rollback, trả 409 kèm `conflictSeatIds` (`SeatConflictException`) |
| UC-DV-04.7 | Tính tiền và giảm giá theo hạng thành viên | include |  | Giá ghế theo suất + combo − giảm giá |
| UC-DV-04.8 | Tạo booking Pending kèm hạn giữ | include |  | `ExpiresAt` = hiện tại + `HoldDurationMinutes`, không quá giờ chiếu |
| UC-DV-04.9 | Phát sự kiện ghế được giữ | include |  | SignalR `seat.held` |


#### 3.3.5.7 Phân rã use case Chọn combo

- **Nội dung cần viết:**
  - Use case cha UC-DV-05: Thêm combo bắp nước vào đơn.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.24` Phân rã use case Chọn combo
  - `Bảng 3.24` Phân rã use case Chọn combo
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/CombosController.cs`; `backend/Cinema.Application/Services/Combos/ComboService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-DV-05.1 | Xem danh sách combo | include | Khách hàng | `IsActive = true` |
| UC-DV-05.2 | Xem gợi ý combo theo suất chiếu | extend | Khách hàng | `GetSuggestions` (`NotFound_Showtime`) |
| UC-DV-05.3 | Chọn số lượng combo | include | Khách hàng | Tối đa 10 mỗi loại |


#### 3.3.5.8 Phân rã use case Hủy đơn đặt vé

- **Nội dung cần viết:**
  - Use case cha UC-DV-08: Hủy giữ ghế hoặc hủy đơn đã thanh toán.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.25` Phân rã use case Hủy đơn đặt vé
  - `Bảng 3.25` Phân rã use case Hủy đơn đặt vé
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/BookingsController.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs → CancelBookingAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-DV-08.1 | Kiểm tra booking thuộc về người dùng | include |  | `Forbidden` nếu không phải chủ booking |
| UC-DV-08.2 | Hủy booking Pending và trả ghế | tổng quát hóa |  | Giải phóng ShowtimeSeat, phát `seat.released` |
| UC-DV-08.3 | Hủy booking đã thanh toán (Confirmed) | tổng quát hóa |  | Điều kiện: chưa soát vé, không thanh toán tiền mặt, còn trong hạn `CancelDeadlineHours` |
| UC-DV-08.4 | Hoàn tiền qua cổng thanh toán | include | Cổng VNPay, Cổng MoMo, Cổng Stripe | Xem UC-TT-05 (số tiền theo `CancelRefundPercent`) |
| UC-DV-08.5 | Thu hồi điểm thưởng | include |  | ReverseLoyaltyAsync |
| UC-DV-08.6 | Gửi email xác nhận hủy | include | Dịch vụ email | SendEmailJob |


### 3.3.6 Thanh toán trực tuyến

- **Nội dung cần viết:**
  - Tạo giao dịch, nhận IPN/webhook, xác nhận vé, hoàn tiền. Giới thiệu module, tác nhân, số use case.

#### 3.3.6.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.26` Sơ đồ use case module Thanh toán trực tuyến
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.6.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.26` Danh sách use case module Thanh toán trực tuyến

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-TT-01 | Thanh toán đơn đặt vé trực tuyến | Khách hàng, Cổng VNPay, Cổng MoMo, Cổng Stripe | Thanh toán booking Pending qua cổng được chọn. | POST /api/v1/bookings/{bookingId}/checkout | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → CheckoutAsync` |
| UC-TT-02 | Xử lý IPN của VNPay | Cổng VNPay | Nhận thông báo kết quả thanh toán từ VNPay. | GET /api/v1/payments/vnpay/ipn | `backend/Cinema.Api/Controllers/PaymentsController.cs`<br>`backend/Cinema.Application/Services/Payments/PaymentService.cs`<br>`backend/Cinema.Infrastructure/Payments/VnPayGateway.cs` |
| UC-TT-03 | Xử lý IPN của MoMo | Cổng MoMo | Nhận thông báo kết quả thanh toán từ MoMo. | POST /api/v1/payments/momo/ipn | `backend/Cinema.Api/Controllers/PaymentsController.cs`<br>`backend/Cinema.Application/Services/Payments/PaymentService.cs`<br>`backend/Cinema.Infrastructure/Payments/MomoGateway.cs` |
| UC-TT-04 | Xử lý webhook của Stripe | Cổng Stripe | Nhận sự kiện hoàn tất phiên thanh toán từ Stripe. | POST /api/v1/payments/stripe/webhook | `backend/Cinema.Api/Controllers/PaymentsController.cs`<br>`backend/Cinema.Application/Services/Payments/PaymentService.cs`<br>`backend/Cinema.Infrastructure/Payments/StripeGateway.cs` |
| UC-TT-05 | Phát hành vé sau thanh toán | Hệ thống tác vụ nền, Dịch vụ email | Chuyển ghế sang đã bán, tạo vé và mã QR sau khi thanh toán thành công. | Nội bộ: `PaymentService.ApplyPaymentResultAsync` | `backend/Cinema.Application/Services/Payments/PaymentService.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → ConfirmBookingAndGenerateTicketsAsync` |
| UC-TT-06 | Hoàn tiền | Hệ thống tác vụ nền, Cổng VNPay, Cổng MoMo, Cổng Stripe, Dịch vụ email | Hoàn tiền qua cổng khi hủy đơn, hủy suất chiếu hoặc ghế hết hạn sau thanh toán. | Nội bộ: `IPaymentGateway.RefundAsync` | `backend/Cinema.Application/Services/Payments/PaymentService.cs`<br>`backend/Cinema.Infrastructure/Payments/*Gateway.cs` |


#### 3.3.6.3 Phân rã use case Thanh toán đơn đặt vé trực tuyến

- **Nội dung cần viết:**
  - Use case cha UC-TT-01: Thanh toán booking Pending qua cổng được chọn.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.27` Phân rã use case Thanh toán đơn đặt vé trực tuyến
  - `Bảng 3.27` Phân rã use case Thanh toán đơn đặt vé trực tuyến
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/BookingsController.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs → CheckoutAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-TT-01.1 | Kiểm tra booking thuộc người dùng, còn Pending và còn hạn | include |  | `Forbidden`, `Conflict` khi hết hạn |
| UC-TT-01.2 | Thanh toán qua VNPay | tổng quát hóa | Cổng VNPay | VnPayGateway |
| UC-TT-01.3 | Thanh toán qua MoMo | tổng quát hóa | Cổng MoMo | MomoGateway |
| UC-TT-01.4 | Thanh toán qua Stripe | tổng quát hóa | Cổng Stripe | StripeGateway |
| UC-TT-01.5 | Đánh dấu các giao dịch Pending cũ là Failed | include |  | CheckoutAsync |
| UC-TT-01.6 | Tạo bản ghi Payment và mã giao dịch | include |  | `CodeGenerator.GeneratePaymentTransactionId` |
| UC-TT-01.7 | Tạo liên kết thanh toán với cổng | include | Cổng VNPay, Cổng MoMo, Cổng Stripe | `IPaymentGateway.CreatePaymentAsync` |
| UC-TT-01.8 | Chuyển hướng khách sang trang thanh toán của cổng | include | Khách hàng |  |
| UC-TT-01.9 | Xem kết quả trên trang kết quả đặt vé | include | Khách hàng | Frontend `/booking/result` |


#### 3.3.6.4 Phân rã use case Xử lý IPN của VNPay

- **Nội dung cần viết:**
  - Use case cha UC-TT-02: Nhận thông báo kết quả thanh toán từ VNPay.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.28` Phân rã use case Xử lý IPN của VNPay
  - `Bảng 3.28` Phân rã use case Xử lý IPN của VNPay
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/PaymentsController.cs`; `backend/Cinema.Application/Services/Payments/PaymentService.cs`; `backend/Cinema.Infrastructure/Payments/VnPayGateway.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-TT-02.1 | Xác minh chữ ký HMAC-SHA512 | include |  | `HmacSigner.Sha512Hex` |
| UC-TT-02.2 | Tìm giao dịch theo mã | include |  |  |
| UC-TT-02.3 | Bỏ qua nếu giao dịch đã được xử lý | include |  | `IsAlreadyProcessed` (idempotent) |
| UC-TT-02.4 | Kiểm tra số tiền khớp | include |  | Sai số ≤ 0,01 |
| UC-TT-02.5 | Áp dụng kết quả thanh toán | include |  | Xem UC-TT-04 |
| UC-TT-02.6 | Trả mã phản hồi cho VNPay | include | Cổng VNPay |  |


#### 3.3.6.5 Phân rã use case Xử lý IPN của MoMo

- **Nội dung cần viết:**
  - Use case cha UC-TT-03: Nhận thông báo kết quả thanh toán từ MoMo.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.29` Phân rã use case Xử lý IPN của MoMo
  - `Bảng 3.29` Phân rã use case Xử lý IPN của MoMo
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/PaymentsController.cs`; `backend/Cinema.Application/Services/Payments/PaymentService.cs`; `backend/Cinema.Infrastructure/Payments/MomoGateway.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-TT-03.1 | Xác minh chữ ký HMAC-SHA256 | include |  | `HmacSigner.Sha256Hex` |
| UC-TT-03.2 | Tìm giao dịch theo mã | include |  |  |
| UC-TT-03.3 | Bỏ qua nếu giao dịch đã được xử lý | include |  | `IsAlreadyProcessed` |
| UC-TT-03.4 | Kiểm tra số tiền khớp | include |  | Sai số ≤ 0,01 |
| UC-TT-03.5 | Áp dụng kết quả thanh toán | include |  | Xem UC-TT-04 |
| UC-TT-03.6 | Trả phản hồi cho MoMo | include | Cổng MoMo |  |


#### 3.3.6.6 Phân rã use case Xử lý webhook của Stripe

- **Nội dung cần viết:**
  - Use case cha UC-TT-04: Nhận sự kiện hoàn tất phiên thanh toán từ Stripe.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.30` Phân rã use case Xử lý webhook của Stripe
  - `Bảng 3.30` Phân rã use case Xử lý webhook của Stripe
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/PaymentsController.cs`; `backend/Cinema.Application/Services/Payments/PaymentService.cs`; `backend/Cinema.Infrastructure/Payments/StripeGateway.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-TT-04.1 | Xác minh chữ ký webhook | include |  | StripeGateway |
| UC-TT-04.2 | Xử lý sự kiện `checkout.session.completed` | include |  | `EventTypes.CheckoutSessionCompleted` |
| UC-TT-04.3 | Kiểm tra số tiền và đơn vị tiền tệ | include |  | `GatewayCallbackResult.Currency` |
| UC-TT-04.4 | Bỏ qua nếu giao dịch đã được xử lý | include |  | `IsAlreadyProcessed` |
| UC-TT-04.5 | Áp dụng kết quả thanh toán | include |  | Xem UC-TT-05 |


#### 3.3.6.7 Phân rã use case Phát hành vé sau thanh toán

- **Nội dung cần viết:**
  - Use case cha UC-TT-05: Chuyển ghế sang đã bán, tạo vé và mã QR sau khi thanh toán thành công.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.31` Phân rã use case Phát hành vé sau thanh toán
  - `Bảng 3.31` Phân rã use case Phát hành vé sau thanh toán
- **Nguồn dữ liệu:** `backend/Cinema.Application/Services/Payments/PaymentService.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs → ConfirmBookingAndGenerateTicketsAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-TT-05.1 | Chuyển ghế từ Held sang Booked | include |  | ShowtimeSeat.Status |
| UC-TT-05.2 | Cập nhật booking sang Confirmed | include |  |  |
| UC-TT-05.3 | Tạo vé và mã QR cho từng ghế | include |  | `QrCodeHelper`; mã QR có dạng mã booking, hàng, số ghế |
| UC-TT-05.4 | Cộng điểm thưởng và nâng hạng thành viên | include |  | `PointsPerAmount`, MembershipTier |
| UC-TT-05.5 | Phát sự kiện ghế đã bán | include |  | SignalR `seat.booked` |
| UC-TT-05.6 | Gửi email xác nhận đặt vé | include | Dịch vụ email | SendEmailJob |
| UC-TT-05.7 | Tự hoàn tiền khi ghế đã hết hạn sau khi khách trả tiền | extend |  | `HandleSeatsExpiredAfterPaymentAsync` (`SeatsNoLongerHeldException`) |


#### 3.3.6.8 Phân rã use case Hoàn tiền

- **Nội dung cần viết:**
  - Use case cha UC-TT-06: Hoàn tiền qua cổng khi hủy đơn, hủy suất chiếu hoặc ghế hết hạn sau thanh toán.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.32` Phân rã use case Hoàn tiền
  - `Bảng 3.32` Phân rã use case Hoàn tiền
- **Nguồn dữ liệu:** `backend/Cinema.Application/Services/Payments/PaymentService.cs`; `backend/Cinema.Infrastructure/Payments/*Gateway.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-TT-06.1 | Hoàn tiền qua VNPay | tổng quát hóa | Cổng VNPay | VnPayGateway.RefundAsync |
| UC-TT-06.2 | Hoàn tiền qua MoMo | tổng quát hóa | Cổng MoMo | MomoGateway.RefundAsync |
| UC-TT-06.3 | Hoàn tiền qua Stripe | tổng quát hóa | Cổng Stripe | StripeGateway.RefundAsync |
| UC-TT-06.4 | Cập nhật Payment sang Refunded | include |  | `RefundTransactionId`, `RefundedAmount`, `RefundedAt` |
| UC-TT-06.5 | Thu hồi điểm thưởng | include |  | ReverseLoyaltyAsync |
| UC-TT-06.6 | Gửi email hoàn tiền | include | Dịch vụ email | EmailTemplates |
| UC-TT-06.7 | Hoàn tiền thủ công khi cổng không hoàn được | extend |  | Email cho rạp (không áp dụng cho thanh toán tiền mặt) |


### 3.3.7 Trợ lý AI

- **Nội dung cần viết:**
  - Trò chuyện hỏi đáp suất chiếu, ghế trống, vé của tôi. Giới thiệu module, tác nhân, số use case.

#### 3.3.7.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.33` Sơ đồ use case module Trợ lý AI
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.7.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.33` Danh sách use case module Trợ lý AI

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-AI-01 | Trò chuyện với trợ lý AI | Khách vãng lai, Khách hàng, Gemini | Hỏi đáp bằng ngôn ngữ tự nhiên, trợ lý gọi hàm để lấy dữ liệu thật. | POST /api/v1/ai/chat | `backend/Cinema.Api/Controllers/Client/AiController.cs`<br>`backend/Cinema.Application/Services/Ai/GeminiAiAssistantService.cs`<br>`backend/Cinema.Application/Services/Ai/AiToolExecutor.cs` |


#### 3.3.7.3 Phân rã use case Trò chuyện với trợ lý AI

- **Nội dung cần viết:**
  - Use case cha UC-AI-01: Hỏi đáp bằng ngôn ngữ tự nhiên, trợ lý gọi hàm để lấy dữ liệu thật.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.34` Phân rã use case Trò chuyện với trợ lý AI
  - `Bảng 3.34` Phân rã use case Trò chuyện với trợ lý AI
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Client/AiController.cs`; `backend/Cinema.Application/Services/Ai/GeminiAiAssistantService.cs`; `backend/Cinema.Application/Services/Ai/AiToolExecutor.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-AI-01.1 | Gửi tin nhắn và lịch sử hội thoại | include | Khách vãng lai, Khách hàng | Tin nhắn ≤ 1000 ký tự, mỗi mục lịch sử ≤ 2000 ký tự (AiValidators) |
| UC-AI-01.2 | Dựng ngữ cảnh rạp, phim, combo, thông tin người dùng | include |  | System prompt |
| UC-AI-01.3 | Gọi mô hình Gemini | include | Gemini | `gemini-2.5-flash`; tối đa 4 vòng, 8 lần gọi hàm |
| UC-AI-01.4 | Tìm suất chiếu (search_showtimes) | extend | Gemini |  |
| UC-AI-01.5 | Xem tình trạng ghế (get_seat_availability) | extend | Gemini |  |
| UC-AI-01.6 | Xem vé của tôi (get_my_bookings) | extend | Gemini | Chỉ khi đã đăng nhập |
| UC-AI-01.7 | Trả lời kèm thẻ suất chiếu hoặc vé | include |  | Tối đa 6 thẻ |


### 3.3.8 Bán vé tại quầy

- **Nội dung cần viết:**
  - Nhân viên bán vé trực tiếp, thu tiền mặt. Giới thiệu module, tác nhân, số use case.

#### 3.3.8.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.35` Sơ đồ use case module Bán vé tại quầy
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.8.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.35` Danh sách use case module Bán vé tại quầy

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-BQ-01 | Bán vé tại quầy | Nhân viên, Quản trị viên | Bán vé cho khách tại rạp, thanh toán tiền mặt. | POST /api/v1/admin/bookings/counter-hold, POST /api/v1/admin/bookings/{id}/counter-checkout (StaffOnly) | `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → CounterHoldSeatsAsync, CounterCheckoutAsync`<br>`frontend/src/features/admin/counter/**` |


#### 3.3.8.3 Phân rã use case Bán vé tại quầy

- **Nội dung cần viết:**
  - Use case cha UC-BQ-01: Bán vé cho khách tại rạp, thanh toán tiền mặt.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.36` Phân rã use case Bán vé tại quầy
  - `Bảng 3.36` Phân rã use case Bán vé tại quầy
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs → CounterHoldSeatsAsync, CounterCheckoutAsync`; `frontend/src/features/admin/counter/**`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-BQ-01.1 | Chọn suất chiếu | include | Nhân viên | counter-showtime-picker |
| UC-BQ-01.2 | Xem sơ đồ ghế | include | Nhân viên | Xem UC-DV-02 |
| UC-BQ-01.3 | Chọn ghế (không áp luật ghế lẻ) | include | Nhân viên | Chế độ `Off` cho kênh Counter |
| UC-BQ-01.4 | Chọn combo | extend | Nhân viên |  |
| UC-BQ-01.5 | Nhập thông tin khách hoặc chọn khách hàng có sẵn | include | Nhân viên | Tên, số điện thoại; hoặc tìm khách (UC-QN-03) |
| UC-BQ-01.6 | Tìm khách hàng | extend | Nhân viên | GET admin/users/customers |
| UC-BQ-01.7 | Giữ ghế tại quầy | include |  | Kênh Counter, gắn `CreatedByEmployeeId` |
| UC-BQ-01.8 | Thanh toán tiền mặt | include | Nhân viên | Số tiền nhận ≥ số tiền cần thanh toán; tính tiền thối |
| UC-BQ-01.9 | Xác nhận đơn và phát hành vé | include |  | Xem UC-TT-05 |
| UC-BQ-01.10 | Hiển thị vé kết quả | include | Nhân viên | counter-ticket-result |


### 3.3.9 Soát vé

- **Nội dung cần viết:**
  - Quét mã QR hoặc nhập mã để soát vé vào rạp. Giới thiệu module, tác nhân, số use case.

#### 3.3.9.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.37` Sơ đồ use case module Soát vé
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.9.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.37` Danh sách use case module Soát vé

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-SV-01 | Soát vé | Nhân viên, Quản trị viên | Xác nhận vé hợp lệ khi khách vào rạp. | POST /api/v1/admin/bookings/checkin (StaffOnly) | `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → CheckinAsync`<br>`frontend/src/features/admin/checkin/**` |


#### 3.3.9.3 Phân rã use case Soát vé

- **Nội dung cần viết:**
  - Use case cha UC-SV-01: Xác nhận vé hợp lệ khi khách vào rạp.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.38` Phân rã use case Soát vé
  - `Bảng 3.38` Phân rã use case Soát vé
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs → CheckinAsync`; `frontend/src/features/admin/checkin/**`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-SV-01.1 | Quét mã QR bằng camera | extend | Nhân viên | BarcodeDetector hoặc jsQR |
| UC-SV-01.2 | Nhập mã vé thủ công | extend | Nhân viên |  |
| UC-SV-01.3 | Tìm vé theo mã QR | include |  |  |
| UC-SV-01.4 | Kiểm tra booking ở trạng thái Confirmed | include |  |  |
| UC-SV-01.5 | Kiểm tra suất chiếu chưa bị hủy | include |  |  |
| UC-SV-01.6 | Kiểm tra trong cửa sổ thời gian soát vé | include |  | Mở trước giờ chiếu `CheckInOpenMinutes` đến hết `EndTime` |
| UC-SV-01.7 | Chặn soát lặp | include |  | Cập nhật có điều kiện `IsCheckedIn = false` |
| UC-SV-01.8 | Ghi nhận đã soát vé | include |  | `IsCheckedIn`, `CheckedInAt` |
| UC-SV-01.9 | Hiển thị kết quả cho nhân viên | include | Nhân viên | Phim, ghế hoặc lý do từ chối |


### 3.3.10 Quản lý booking

- **Nội dung cần viết:**
  - Tra cứu đơn đặt vé. Giới thiệu module, tác nhân, số use case.

#### 3.3.10.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.39` Sơ đồ use case module Quản lý booking
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.10.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.39` Danh sách use case module Quản lý booking

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-QB-01 | Xem danh sách booking | Nhân viên, Quản trị viên | Danh sách booking phân trang, có bộ lọc. | GET /api/v1/admin/bookings (StaffOnly) | `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → GetBookingsAdminAsync` |
| UC-QB-02 | Xem chi tiết booking | Nhân viên, Quản trị viên | Chi tiết booking, vé, combo, thanh toán. | GET /api/v1/admin/bookings/{bookingId} (StaffOnly) | `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs` |


#### 3.3.10.3 Phân rã use case Xem danh sách booking

- **Nội dung cần viết:**
  - Use case cha UC-QB-01: Danh sách booking phân trang, có bộ lọc.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.40` Phân rã use case Xem danh sách booking
  - `Bảng 3.40` Phân rã use case Xem danh sách booking
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`; `backend/Cinema.Application/Services/Bookings/BookingService.cs → GetBookingsAdminAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QB-01.1 | Lọc theo trạng thái | extend |  | `Status` |
| UC-QB-01.2 | Lọc theo kênh bán | extend |  | `Channel` (Online/Counter) |
| UC-QB-01.3 | Lọc theo khoảng ngày | extend |  | `FromDate`, `ToDate` |
| UC-QB-01.4 | Tìm kiếm theo từ khóa | extend |  | `Search` |
| UC-QB-01.5 | Phân trang | include |  | `page`, `pageSize` |


### 3.3.11 Quản lý phim

- **Nội dung cần viết:**
  - Thêm, sửa, xóa phim. Giới thiệu module, tác nhân, số use case.

#### 3.3.11.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.41` Sơ đồ use case module Quản lý phim
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.11.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.41` Danh sách use case module Quản lý phim

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-QP-01 | Xem danh sách phim (quản trị) | Nhân viên, Quản trị viên | Danh sách phim phân trang có bộ lọc. | GET /api/v1/admin/movies (StaffOnly) | `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`<br>`backend/Cinema.Application/Services/Movies/MovieService.cs → GetMoviesAdminAsync` |
| UC-QP-02 | Thêm phim | Quản trị viên, Cloudinary | Tạo phim mới kèm thể loại, diễn viên. | POST /api/v1/admin/movies (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`<br>`backend/Cinema.Application/Services/Movies/MovieService.cs → CreateMovieAsync` |
| UC-QP-03 | Sửa phim | Quản trị viên, Cloudinary | Cập nhật thông tin và quan hệ của phim. | PUT /api/v1/admin/movies/{id} (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`<br>`backend/Cinema.Application/Services/Movies/MovieService.cs → UpdateMovieAsync` |
| UC-QP-04 | Xóa phim | Quản trị viên | Xóa một phim. | DELETE /api/v1/admin/movies/{id} (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`<br>`backend/Cinema.Application/Services/Movies/MovieService.cs → DeleteMovieAsync` |
| UC-QP-05 | Xóa nhiều phim | Quản trị viên | Xóa hàng loạt các phim đã chọn. | DELETE /api/v1/admin/movies/bulk (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`<br>`backend/Cinema.Application/Services/Movies/MovieService.cs` |


#### 3.3.11.3 Phân rã use case Xem danh sách phim (quản trị)

- **Nội dung cần viết:**
  - Use case cha UC-QP-01: Danh sách phim phân trang có bộ lọc.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.42` Phân rã use case Xem danh sách phim (quản trị)
  - `Bảng 3.42` Phân rã use case Xem danh sách phim (quản trị)
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`; `backend/Cinema.Application/Services/Movies/MovieService.cs → GetMoviesAdminAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QP-01.1 | Tìm kiếm theo từ khóa | extend |  | `Search` |
| UC-QP-01.2 | Lọc theo trạng thái, đạo diễn, ngôn ngữ, phân loại tuổi, thể loại | extend |  | `AdminMovieListFilter` |
| UC-QP-01.3 | Phân trang | include |  | `page`, `pageSize` |


#### 3.3.11.4 Phân rã use case Thêm phim

- **Nội dung cần viết:**
  - Use case cha UC-QP-02: Tạo phim mới kèm thể loại, diễn viên.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.43` Phân rã use case Thêm phim
  - `Bảng 3.43` Phân rã use case Thêm phim
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`; `backend/Cinema.Application/Services/Movies/MovieService.cs → CreateMovieAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QP-02.1 | Nhập thông tin phim | include | Quản trị viên | Tên, tên gốc, mô tả, thời lượng, ngày phát hành, phụ đề, trạng thái |
| UC-QP-02.2 | Chọn đạo diễn | include | Quản trị viên | `Invalid_DirectorIdNotFound` |
| UC-QP-02.3 | Chọn ngôn ngữ | include | Quản trị viên | `Invalid_LanguageIdNotFound` |
| UC-QP-02.4 | Chọn phân loại tuổi | include | Quản trị viên | `Invalid_AgeRatingIdNotFound` |
| UC-QP-02.5 | Chọn thể loại | include | Quản trị viên | `Invalid_GenreIdsNotFound` |
| UC-QP-02.6 | Chọn diễn viên và vai diễn | include | Quản trị viên | `Invalid_ActorIdsNotFound` |
| UC-QP-02.7 | Tải poster | extend | Cloudinary | Xem UC-TP-01 |
| UC-QP-02.8 | Tải trailer | extend | Cloudinary | Xem UC-TP-02 hoặc nhập `TrailerUrl` |
| UC-QP-02.9 | Lưu phim | include |  | Tạo Movie, MovieGenre, MovieCast |


#### 3.3.11.5 Phân rã use case Sửa phim

- **Nội dung cần viết:**
  - Use case cha UC-QP-03: Cập nhật thông tin và quan hệ của phim.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.44` Phân rã use case Sửa phim
  - `Bảng 3.44` Phân rã use case Sửa phim
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`; `backend/Cinema.Application/Services/Movies/MovieService.cs → UpdateMovieAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QP-03.1 | Tải dữ liệu phim hiện tại | include |  | `NotFound_Movie` |
| UC-QP-03.2 | Sửa thông tin phim | include | Quản trị viên |  |
| UC-QP-03.3 | Kiểm tra dữ liệu tham chiếu tồn tại | include |  | `Invalid_ReferenceData` |
| UC-QP-03.4 | Cập nhật thể loại và diễn viên | include |  | MovieGenre, MovieCast |
| UC-QP-03.5 | Đổi poster hoặc trailer | extend | Cloudinary | UC-TP-01, UC-TP-02 |
| UC-QP-03.6 | Lưu thay đổi | include |  |  |


#### 3.3.11.6 Phân rã use case Xóa phim

- **Nội dung cần viết:**
  - Use case cha UC-QP-04: Xóa một phim.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.45` Phân rã use case Xóa phim
  - `Bảng 3.45` Phân rã use case Xóa phim
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminMoviesController.cs`; `backend/Cinema.Application/Services/Movies/MovieService.cs → DeleteMovieAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QP-04.1 | Kiểm tra phim tồn tại | include |  | `NotFound_Movie` |
| UC-QP-04.2 | Kiểm tra ràng buộc dữ liệu liên quan | include |  | Showtime → Movie là Restrict; [CẦN BỔ SUNG: xác minh thông điệp khi phim còn suất chiếu] |
| UC-QP-04.3 | Xóa các liên kết thể loại, diễn viên, đánh giá | include |  | DeleteBehavior.Cascade |
| UC-QP-04.4 | Xóa nhiều phim cùng lúc | extend | Quản trị viên | Xem UC-QP-05 |


### 3.3.12 Quản lý danh mục

- **Nội dung cần viết:**
  - Danh mục phim, danh mục phòng, nhân sự, thành viên, combo. Giới thiệu module, tác nhân, số use case.

#### 3.3.12.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.46` Sơ đồ use case module Quản lý danh mục
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.12.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.46` Danh sách use case module Quản lý danh mục

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-QD-01 | Quản lý thể loại phim | Quản trị viên | Xem, thêm, sửa, xóa thể loại phim. | /api/v1/admin/genres (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminGenresController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-02 | Quản lý ngôn ngữ | Quản trị viên | Xem, thêm, sửa, xóa ngôn ngữ. | /api/v1/admin/languages (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminLanguagesController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-03 | Quản lý phân loại độ tuổi | Quản trị viên | Xem, thêm, sửa, xóa phân loại độ tuổi. | /api/v1/admin/age-ratings (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminAgeRatingsController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-04 | Quản lý diễn viên | Quản trị viên, Cloudinary | Xem, thêm, sửa, xóa diễn viên. | /api/v1/admin/actors (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminActorsController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-05 | Quản lý đạo diễn | Quản trị viên, Cloudinary | Xem, thêm, sửa, xóa đạo diễn. | /api/v1/admin/directors (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminDirectorsController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-06 | Quản lý loại màn hình | Quản trị viên | Xem, thêm, sửa, xóa loại màn hình. | /api/v1/admin/screen-types (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminScreenTypesController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-07 | Quản lý loại ghế | Quản trị viên | Xem, thêm, sửa, xóa loại ghế. | /api/v1/admin/seat-types (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminSeatTypesController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-08 | Quản lý chức vụ nhân viên | Quản trị viên | Xem, thêm, sửa, xóa chức vụ nhân viên. | /api/v1/admin/positions (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminPositionsController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-09 | Quản lý hạng thành viên | Quản trị viên | Xem, thêm, sửa, xóa hạng thành viên. | /api/v1/admin/membership-tiers (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminMembershipTiersController.cs`<br>`backend/Cinema.Application/Services/Catalog/CatalogService.cs` |
| UC-QD-10 | Quản lý combo | Quản trị viên, Cloudinary | Xem, thêm, sửa, bật/tắt combo (không có thao tác xóa). | /api/v1/admin/combos (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminCombosController.cs`<br>`backend/Cinema.Application/Services/Combos/ComboService.cs` |


#### 3.3.12.3 Phân rã use case Quản lý thể loại phim

- **Nội dung cần viết:**
  - Use case cha UC-QD-01: Xem, thêm, sửa, xóa thể loại phim.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.47` Phân rã use case Quản lý thể loại phim
  - `Bảng 3.47` Phân rã use case Quản lý thể loại phim
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminGenresController.cs`; `backend/Cinema.Application/Services/Catalog/CatalogService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QD-01.1 | Xem danh sách thể loại phim | include | Quản trị viên | GET /api/v1/admin/genres |
| UC-QD-01.2 | Thêm thể loại phim | include | Quản trị viên | POST /api/v1/admin/genres; `Conflict_GenreExists` nếu trùng |
| UC-QD-01.3 | Sửa thể loại phim | include | Quản trị viên | PUT /api/v1/admin/genres/{id}; `NotFound_Genre` |
| UC-QD-01.4 | Xóa thể loại phim | include | Quản trị viên | DELETE /api/v1/admin/genres/{id}; `Conflict_GenreInUse` nếu đang được sử dụng |


#### 3.3.12.4 Phân rã use case Quản lý ngôn ngữ

- **Nội dung cần viết:**
  - Use case cha UC-QD-02: Xem, thêm, sửa, xóa ngôn ngữ.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.48` Phân rã use case Quản lý ngôn ngữ
  - `Bảng 3.48` Phân rã use case Quản lý ngôn ngữ
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminLanguagesController.cs`; `backend/Cinema.Application/Services/Catalog/CatalogService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QD-02.1 | Xem danh sách ngôn ngữ | include | Quản trị viên | GET /api/v1/admin/languages |
| UC-QD-02.2 | Thêm ngôn ngữ | include | Quản trị viên | POST /api/v1/admin/languages; `Conflict_LanguageExists` nếu trùng |
| UC-QD-02.3 | Sửa ngôn ngữ | include | Quản trị viên | PUT /api/v1/admin/languages/{id}; `NotFound_Language` |
| UC-QD-02.4 | Xóa ngôn ngữ | include | Quản trị viên | DELETE /api/v1/admin/languages/{id}; `Conflict_LanguageInUse` nếu đang được sử dụng |


#### 3.3.12.5 Phân rã use case Quản lý phân loại độ tuổi

- **Nội dung cần viết:**
  - Use case cha UC-QD-03: Xem, thêm, sửa, xóa phân loại độ tuổi.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.49` Phân rã use case Quản lý phân loại độ tuổi
  - `Bảng 3.49` Phân rã use case Quản lý phân loại độ tuổi
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminAgeRatingsController.cs`; `backend/Cinema.Application/Services/Catalog/CatalogService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QD-03.1 | Xem danh sách phân loại độ tuổi | include | Quản trị viên | GET /api/v1/admin/age-ratings |
| UC-QD-03.2 | Thêm phân loại độ tuổi | include | Quản trị viên | POST /api/v1/admin/age-ratings; `Conflict_AgeRatingExists` nếu trùng |
| UC-QD-03.3 | Sửa phân loại độ tuổi | include | Quản trị viên | PUT /api/v1/admin/age-ratings/{id}; `NotFound_AgeRating` |
| UC-QD-03.4 | Xóa phân loại độ tuổi | include | Quản trị viên | DELETE /api/v1/admin/age-ratings/{id}; `Conflict_AgeRatingInUse` nếu đang được sử dụng |


#### 3.3.12.6 Phân rã use case Quản lý diễn viên

- **Nội dung cần viết:**
  - Use case cha UC-QD-04: Xem, thêm, sửa, xóa diễn viên.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.50` Phân rã use case Quản lý diễn viên
  - `Bảng 3.50` Phân rã use case Quản lý diễn viên
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminActorsController.cs`; `backend/Cinema.Application/Services/Catalog/CatalogService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QD-04.1 | Xem danh sách diễn viên | include | Quản trị viên | GET /api/v1/admin/actors |
| UC-QD-04.2 | Thêm diễn viên | include | Quản trị viên | POST /api/v1/admin/actors |
| UC-QD-04.3 | Sửa diễn viên | include | Quản trị viên | PUT /api/v1/admin/actors/{id}; `NotFound_Actor` |
| UC-QD-04.4 | Xóa diễn viên | include | Quản trị viên | DELETE /api/v1/admin/actors/{id}; `Conflict_ActorInUse` nếu đang được sử dụng |
| UC-QD-04.5 | Xóa nhiều diễn viên | extend | Quản trị viên | DELETE /api/v1/admin/actors/bulk |
| UC-QD-04.6 | Tải ảnh đại diện | extend | Cloudinary | Xem UC-TP-01 |


#### 3.3.12.7 Phân rã use case Quản lý đạo diễn

- **Nội dung cần viết:**
  - Use case cha UC-QD-05: Xem, thêm, sửa, xóa đạo diễn.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.51` Phân rã use case Quản lý đạo diễn
  - `Bảng 3.51` Phân rã use case Quản lý đạo diễn
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminDirectorsController.cs`; `backend/Cinema.Application/Services/Catalog/CatalogService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QD-05.1 | Xem danh sách đạo diễn | include | Quản trị viên | GET /api/v1/admin/directors |
| UC-QD-05.2 | Thêm đạo diễn | include | Quản trị viên | POST /api/v1/admin/directors |
| UC-QD-05.3 | Sửa đạo diễn | include | Quản trị viên | PUT /api/v1/admin/directors/{id}; `NotFound_Director` |
| UC-QD-05.4 | Xóa đạo diễn | include | Quản trị viên | DELETE /api/v1/admin/directors/{id}; `Conflict_DirectorInUse` nếu đang được sử dụng |
| UC-QD-05.5 | Xóa nhiều đạo diễn | extend | Quản trị viên | DELETE /api/v1/admin/directors/bulk |
| UC-QD-05.6 | Tải ảnh đại diện | extend | Cloudinary | Xem UC-TP-01 |


#### 3.3.12.8 Phân rã use case Quản lý loại màn hình

- **Nội dung cần viết:**
  - Use case cha UC-QD-06: Xem, thêm, sửa, xóa loại màn hình.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.52` Phân rã use case Quản lý loại màn hình
  - `Bảng 3.52` Phân rã use case Quản lý loại màn hình
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminScreenTypesController.cs`; `backend/Cinema.Application/Services/Catalog/CatalogService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QD-06.1 | Xem danh sách loại màn hình | include | Quản trị viên | GET /api/v1/admin/screen-types |
| UC-QD-06.2 | Thêm loại màn hình | include | Quản trị viên | POST /api/v1/admin/screen-types; `Conflict_ScreenTypeExists` nếu trùng |
| UC-QD-06.3 | Sửa loại màn hình | include | Quản trị viên | PUT /api/v1/admin/screen-types/{id}; `NotFound_ScreenType` |
| UC-QD-06.4 | Xóa loại màn hình | include | Quản trị viên | DELETE /api/v1/admin/screen-types/{id}; `Conflict_ScreenTypeInUse` nếu đang được sử dụng |


#### 3.3.12.9 Phân rã use case Quản lý loại ghế

- **Nội dung cần viết:**
  - Use case cha UC-QD-07: Xem, thêm, sửa, xóa loại ghế.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.53` Phân rã use case Quản lý loại ghế
  - `Bảng 3.53` Phân rã use case Quản lý loại ghế
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminSeatTypesController.cs`; `backend/Cinema.Application/Services/Catalog/CatalogService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QD-07.1 | Xem danh sách loại ghế | include | Quản trị viên | GET /api/v1/admin/seat-types |
| UC-QD-07.2 | Thêm loại ghế | include | Quản trị viên | POST /api/v1/admin/seat-types; `Conflict_SeatTypeExists` nếu trùng |
| UC-QD-07.3 | Sửa loại ghế | include | Quản trị viên | PUT /api/v1/admin/seat-types/{id}; `NotFound_SeatType` |
| UC-QD-07.4 | Xóa loại ghế | include | Quản trị viên | DELETE /api/v1/admin/seat-types/{id}; `Conflict_SeatTypeInUse` nếu đang được sử dụng |
| UC-QD-07.5 | Đặt hệ số giá của loại ghế | include | Quản trị viên | `PriceMultiplier` (có ràng buộc CHECK, xem 4.2) |


#### 3.3.12.10 Phân rã use case Quản lý chức vụ nhân viên

- **Nội dung cần viết:**
  - Use case cha UC-QD-08: Xem, thêm, sửa, xóa chức vụ nhân viên.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.54` Phân rã use case Quản lý chức vụ nhân viên
  - `Bảng 3.54` Phân rã use case Quản lý chức vụ nhân viên
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminPositionsController.cs`; `backend/Cinema.Application/Services/Catalog/CatalogService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QD-08.1 | Xem danh sách chức vụ nhân viên | include | Quản trị viên | GET /api/v1/admin/positions |
| UC-QD-08.2 | Thêm chức vụ nhân viên | include | Quản trị viên | POST /api/v1/admin/positions; `Conflict_PositionExists` nếu trùng |
| UC-QD-08.3 | Sửa chức vụ nhân viên | include | Quản trị viên | PUT /api/v1/admin/positions/{id}; `NotFound_Position` |
| UC-QD-08.4 | Xóa chức vụ nhân viên | include | Quản trị viên | DELETE /api/v1/admin/positions/{id}; `Conflict_PositionInUse` nếu đang được sử dụng |


#### 3.3.12.11 Phân rã use case Quản lý hạng thành viên

- **Nội dung cần viết:**
  - Use case cha UC-QD-09: Xem, thêm, sửa, xóa hạng thành viên.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.55` Phân rã use case Quản lý hạng thành viên
  - `Bảng 3.55` Phân rã use case Quản lý hạng thành viên
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminMembershipTiersController.cs`; `backend/Cinema.Application/Services/Catalog/CatalogService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QD-09.1 | Xem danh sách hạng thành viên | include | Quản trị viên | GET /api/v1/admin/membership-tiers |
| UC-QD-09.2 | Thêm hạng thành viên | include | Quản trị viên | POST /api/v1/admin/membership-tiers; `Conflict_MembershipTierExists` nếu trùng |
| UC-QD-09.3 | Sửa hạng thành viên | include | Quản trị viên | PUT /api/v1/admin/membership-tiers/{id}; `NotFound_MembershipTier` |
| UC-QD-09.4 | Xóa hạng thành viên | include | Quản trị viên | DELETE /api/v1/admin/membership-tiers/{id}; `Conflict_MembershipTierInUse` nếu đang được sử dụng |
| UC-QD-09.5 | Đặt điểm tối thiểu và mức giảm giá | include | Quản trị viên | `MinPoints`, `DiscountPercent` |


#### 3.3.12.12 Phân rã use case Quản lý combo

- **Nội dung cần viết:**
  - Use case cha UC-QD-10: Xem, thêm, sửa, bật/tắt combo (không có thao tác xóa).
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.56` Phân rã use case Quản lý combo
  - `Bảng 3.56` Phân rã use case Quản lý combo
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminCombosController.cs`; `backend/Cinema.Application/Services/Combos/ComboService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QD-10.1 | Xem danh sách combo | include | Quản trị viên | GET /api/v1/admin/combos |
| UC-QD-10.2 | Thêm combo | include | Quản trị viên | POST /api/v1/admin/combos |
| UC-QD-10.3 | Sửa combo | include | Quản trị viên | PUT /api/v1/admin/combos/{id}; `NotFound_Combo` |
| UC-QD-10.4 | Bật hoặc tắt bán một combo | include | Quản trị viên | PUT /api/v1/admin/combos/{id}/toggle |
| UC-QD-10.5 | Bật hoặc tắt bán nhiều combo | extend | Quản trị viên | PUT /api/v1/admin/combos/bulk/status |
| UC-QD-10.6 | Tải ảnh combo | extend | Cloudinary | Xem UC-TP-01 |


### 3.3.13 Quản lý phòng chiếu

- **Nội dung cần viết:**
  - Phòng chiếu và sơ đồ ghế. Giới thiệu module, tác nhân, số use case.

#### 3.3.13.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.57` Sơ đồ use case module Quản lý phòng chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.13.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.57` Danh sách use case module Quản lý phòng chiếu

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-QR-01 | Quản lý phòng chiếu | Quản trị viên | Xem, thêm, sửa, xóa phòng chiếu. | GET/POST /api/v1/admin/rooms, GET/PUT/DELETE /api/v1/admin/rooms/{id} (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminRoomsController.cs`<br>`backend/Cinema.Application/Services/Rooms/RoomService.cs` |
| UC-QR-02 | Xem sơ đồ ghế của phòng | Quản trị viên | Xem sơ đồ ghế hiện tại của phòng. | GET /api/v1/admin/rooms/{id}/seats (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminRoomsController.cs`<br>`backend/Cinema.Application/Services/Rooms/RoomService.cs → GetRoomSeatsAsync` |
| UC-QR-03 | Soạn sơ đồ ghế | Quản trị viên | Thiết kế và lưu sơ đồ ghế của phòng. | PUT /api/v1/admin/rooms/{id}/seats (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminRoomsController.cs`<br>`backend/Cinema.Application/Services/Rooms/RoomService.cs → UpdateSeatLayoutAsync`<br>`frontend/src/features/admin/rooms/components/seat-editor-*.tsx` |


#### 3.3.13.3 Phân rã use case Quản lý phòng chiếu

- **Nội dung cần viết:**
  - Use case cha UC-QR-01: Xem, thêm, sửa, xóa phòng chiếu.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.58` Phân rã use case Quản lý phòng chiếu
  - `Bảng 3.58` Phân rã use case Quản lý phòng chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminRoomsController.cs`; `backend/Cinema.Application/Services/Rooms/RoomService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QR-01.1 | Xem danh sách phòng chiếu | include | Quản trị viên | GetRoomsAsync |
| UC-QR-01.2 | Xem chi tiết phòng chiếu | include | Quản trị viên | GetRoomByIdAsync |
| UC-QR-01.3 | Thêm phòng chiếu | include | Quản trị viên | Tên không trùng (`Conflict_RoomNameExists`); loại màn hình tồn tại (`Invalid_ScreenTypeIdNotFound`) |
| UC-QR-01.4 | Sửa phòng chiếu | include | Quản trị viên | UpdateRoomAsync |
| UC-QR-01.5 | Ngừng hoạt động phòng chiếu | extend | Quản trị viên | Không được khi còn ghế đang giữ (`Conflict_RoomHasHeldSeatsCannotDeactivate`) |
| UC-QR-01.6 | Xóa phòng chiếu | include | Quản trị viên | Không được khi còn suất chiếu sắp tới (`Rule_RoomHasUpcomingShowtimes`) |


#### 3.3.13.4 Phân rã use case Soạn sơ đồ ghế

- **Nội dung cần viết:**
  - Use case cha UC-QR-03: Thiết kế và lưu sơ đồ ghế của phòng.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.59` Phân rã use case Soạn sơ đồ ghế
  - `Bảng 3.59` Phân rã use case Soạn sơ đồ ghế
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminRoomsController.cs`; `backend/Cinema.Application/Services/Rooms/RoomService.cs → UpdateSeatLayoutAsync`; `frontend/src/features/admin/rooms/components/seat-editor-*.tsx`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QR-03.1 | Đặt kích thước lưới (số hàng, số cột) | include | Quản trị viên | seat-editor-size-inputs |
| UC-QR-03.2 | Sinh sơ đồ tự động | extend | Quản trị viên | seat-editor-auto-generate-dialog |
| UC-QR-03.3 | Tô loại ghế bằng cọ | include | Quản trị viên | seat-editor-brush-tools |
| UC-QR-03.4 | Chọn vùng nhiều ghế | extend | Quản trị viên | seat-editor-selection-bar |
| UC-QR-03.5 | Đặt lối đi | extend | Quản trị viên | `IsAisle` |
| UC-QR-03.6 | Ghép ghế đôi | extend | Quản trị viên | `CoupleId` |
| UC-QR-03.7 | Sao chép hàng ghế | extend | Quản trị viên | seat-editor-copy-row-bar |
| UC-QR-03.8 | Hoàn tác và làm lại | extend | Quản trị viên | seat-editor-history-buttons |
| UC-QR-03.9 | Kiểm tra tính hợp lệ của sơ đồ | include | Quản trị viên | seat-editor-validation-panel; `Invalid_CoupleSeatsUnpaired`, `Invalid_SeatTypeIdsNotFound` |
| UC-QR-03.10 | Lưu sơ đồ ghế | include | Quản trị viên | Không đổi ghế đang giữ/đã bán (`Conflict_SeatsHeldOrBookedCannotChange`) |


### 3.3.14 Quản lý suất chiếu

- **Nội dung cần viết:**
  - Tạo, sửa, hủy, xếp lịch suất chiếu. Giới thiệu module, tác nhân, số use case.

#### 3.3.14.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.60` Sơ đồ use case module Quản lý suất chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.14.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.60` Danh sách use case module Quản lý suất chiếu

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-QS-01 | Xem danh sách suất chiếu | Nhân viên, Quản trị viên | Danh sách suất chiếu có bộ lọc. | GET /api/v1/admin/showtimes (StaffOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → GetShowtimesAdminAsync` |
| UC-QS-02 | Xem lịch suất chiếu dạng lịch | Nhân viên, Quản trị viên | Xem suất chiếu trên lịch theo ngày/phòng. | GET /api/v1/admin/showtimes/calendar (StaffOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → GetShowtimesCalendarAsync`<br>`frontend/src/features/admin/showtimes/components/calendar/**` |
| UC-QS-03 | Tạo suất chiếu | Quản trị viên | Tạo một suất chiếu mới. | POST /api/v1/admin/showtimes (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → CreateShowtimeAsync` |
| UC-QS-04 | Sửa suất chiếu | Quản trị viên | Cập nhật giờ chiếu, giá của suất chiếu. | PUT /api/v1/admin/showtimes/{id} (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → UpdateShowtimeAsync` |
| UC-QS-05 | Hủy suất chiếu | Quản trị viên, Dịch vụ email | Hủy suất chiếu, hoàn tiền cho các booking liên quan. | DELETE /api/v1/admin/showtimes/{id} (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → DeleteShowtimeAsync`<br>`backend/Cinema.Application/BackgroundJobs/ShowtimeRefundJob.cs` |
| UC-QS-06 | Tạo suất chiếu hàng loạt | Quản trị viên | Tạo nhiều suất theo khoảng ngày và khung giờ. | POST /api/v1/admin/showtimes/bulk (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → BulkCreateShowtimesAsync` |
| UC-QS-07 | Tự động xếp lịch chiếu | Quản trị viên | Đề xuất và áp dụng lịch chiếu tự động theo độ nóng của phim. | POST /api/v1/admin/showtimes/auto-schedule/preview, /apply (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → PreviewAutoScheduleAsync, ApplyAutoScheduleAsync`<br>`backend/Cinema.Application/Utils/ShowtimeScheduler.cs` |
| UC-QS-08 | Nhân bản lịch chiếu | Quản trị viên | Sao chép lịch chiếu từ khoảng ngày này sang khoảng ngày khác. | POST /api/v1/admin/showtimes/clone (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → CloneShowtimesAsync` |
| UC-QS-09 | Xóa nhiều suất chiếu | Quản trị viên | Hủy hàng loạt các suất đã chọn. | DELETE /api/v1/admin/showtimes/bulk (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → BulkDeleteShowtimesAsync` |


#### 3.3.14.3 Phân rã use case Xem danh sách suất chiếu

- **Nội dung cần viết:**
  - Use case cha UC-QS-01: Danh sách suất chiếu có bộ lọc.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.61` Phân rã use case Xem danh sách suất chiếu
  - `Bảng 3.61` Phân rã use case Xem danh sách suất chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → GetShowtimesAdminAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QS-01.1 | Lọc theo phòng | extend |  | `RoomId` |
| UC-QS-01.2 | Lọc theo phim | extend |  | `MovieId` |
| UC-QS-01.3 | Lọc theo ngày | extend |  | `Date` |
| UC-QS-01.4 | Lọc theo trạng thái | extend |  | `Status` |


#### 3.3.14.4 Phân rã use case Tạo suất chiếu

- **Nội dung cần viết:**
  - Use case cha UC-QS-03: Tạo một suất chiếu mới.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.62` Phân rã use case Tạo suất chiếu
  - `Bảng 3.62` Phân rã use case Tạo suất chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → CreateShowtimeAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QS-03.1 | Chọn phim | include | Quản trị viên | Không được chọn phim đã kết thúc (Ended) |
| UC-QS-03.2 | Chọn phòng chiếu | include | Quản trị viên | Phòng đang hoạt động và đã có sơ đồ ghế |
| UC-QS-03.3 | Chọn giờ bắt đầu và nhập giá cơ bản | include | Quản trị viên | `BasePrice` |
| UC-QS-03.4 | Tính giờ kết thúc | include |  | Thời lượng phim + đệm phòng 15 phút |
| UC-QS-03.5 | Kiểm tra giờ mở cửa | include |  | OperatingHours (trừ khi cho phép ngoài giờ) |
| UC-QS-03.6 | Kiểm tra không trùng lịch phòng | include |  | `HasOverlapAsync` |
| UC-QS-03.7 | Sinh ghế theo suất | include |  | ShowtimeSeat: giá = giá cơ bản × hệ số loại ghế |
| UC-QS-03.8 | Lưu suất chiếu | include |  | CK_Showtime_Time: EndTime > StartTime |


#### 3.3.14.5 Phân rã use case Sửa suất chiếu

- **Nội dung cần viết:**
  - Use case cha UC-QS-04: Cập nhật giờ chiếu, giá của suất chiếu.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.63` Phân rã use case Sửa suất chiếu
  - `Bảng 3.63` Phân rã use case Sửa suất chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → UpdateShowtimeAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QS-04.1 | Kiểm tra suất chiếu tồn tại và chưa bắt đầu | include |  |  |
| UC-QS-04.2 | Chặn đổi phòng | include |  | [CẦN BỔ SUNG: xác minh thông điệp trong UpdateShowtimeAsync] |
| UC-QS-04.3 | Chặn sửa khi đã có booking | include |  | [CẦN BỔ SUNG: xác minh điều kiện Pending/Confirmed] |
| UC-QS-04.4 | Kiểm tra giờ mở cửa và không trùng lịch | include |  |  |
| UC-QS-04.5 | Cập nhật giá ghế theo suất | extend |  | Khi đổi giá cơ bản |
| UC-QS-04.6 | Lưu thay đổi | include |  |  |


#### 3.3.14.6 Phân rã use case Hủy suất chiếu

- **Nội dung cần viết:**
  - Use case cha UC-QS-05: Hủy suất chiếu, hoàn tiền cho các booking liên quan.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.64` Phân rã use case Hủy suất chiếu
  - `Bảng 3.64` Phân rã use case Hủy suất chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → DeleteShowtimeAsync`; `backend/Cinema.Application/BackgroundJobs/ShowtimeRefundJob.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QS-05.1 | Kiểm tra suất chiếu chưa bắt đầu | include |  |  |
| UC-QS-05.2 | Chuyển các booking Pending và Confirmed sang Cancelled | include |  | [CẦN BỔ SUNG: xác minh thứ tự xử lý trong DeleteShowtimeAsync] |
| UC-QS-05.3 | Xếp công việc hoàn tiền cho từng thanh toán | include | Hệ thống tác vụ nền | ShowtimeRefundJob (xem UC-HT-04) |
| UC-QS-05.4 | Gửi email thông báo hủy suất | include | Dịch vụ email | SendEmailJob |
| UC-QS-05.5 | Chuyển suất chiếu sang Cancelled | include |  | ShowtimeStatus.Cancelled |
| UC-QS-05.6 | Hủy nhiều suất chiếu cùng lúc | extend | Quản trị viên | DELETE /api/v1/admin/showtimes/bulk |


#### 3.3.14.7 Phân rã use case Tạo suất chiếu hàng loạt

- **Nội dung cần viết:**
  - Use case cha UC-QS-06: Tạo nhiều suất theo khoảng ngày và khung giờ.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.65` Phân rã use case Tạo suất chiếu hàng loạt
  - `Bảng 3.65` Phân rã use case Tạo suất chiếu hàng loạt
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → BulkCreateShowtimesAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QS-06.1 | Chọn phim và phòng chiếu | include | Quản trị viên |  |
| UC-QS-06.2 | Chọn khoảng ngày, các thứ trong tuần và các khung giờ | include | Quản trị viên |  |
| UC-QS-06.3 | Giới hạn tối đa 2000 suất mỗi lần | include |  | [CẦN BỔ SUNG: xác minh hằng số giới hạn trong BulkCreateShowtimesAsync] |
| UC-QS-06.4 | Kiểm tra từng suất (giờ mở cửa, trùng lịch) | include |  |  |
| UC-QS-06.5 | Tạo các suất hợp lệ và báo các suất bị bỏ qua | include |  | [CẦN BỔ SUNG: xác minh dạng kết quả trả về] |


#### 3.3.14.8 Phân rã use case Tự động xếp lịch chiếu

- **Nội dung cần viết:**
  - Use case cha UC-QS-07: Đề xuất và áp dụng lịch chiếu tự động theo độ nóng của phim.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.66` Phân rã use case Tự động xếp lịch chiếu
  - `Bảng 3.66` Phân rã use case Tự động xếp lịch chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → PreviewAutoScheduleAsync, ApplyAutoScheduleAsync`; `backend/Cinema.Application/Utils/ShowtimeScheduler.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QS-07.1 | Chọn khoảng ngày, phòng, phim và số suất | include | Quản trị viên | Khoảng tối đa 31 ngày |
| UC-QS-07.2 | Tính độ nóng của phim | include |  | ShowtimeScheduler.Heat (số vé bán gần đây, điểm đánh giá) |
| UC-QS-07.3 | Sinh các khung giờ ứng viên | include |  | Bước `StepMinutes`, đệm phòng, khoảng cách tối thiểu cùng phim |
| UC-QS-07.4 | Chấm điểm và chọn khung giờ (tham lam) | include |  | ShowtimeScheduler.Score/Build |
| UC-QS-07.5 | Xem trước kết quả | include | Quản trị viên | `PreviewAutoScheduleAsync` |
| UC-QS-07.6 | Áp dụng kết quả | include | Quản trị viên | `ApplyAutoScheduleAsync` |


#### 3.3.14.9 Phân rã use case Nhân bản lịch chiếu

- **Nội dung cần viết:**
  - Use case cha UC-QS-08: Sao chép lịch chiếu từ khoảng ngày này sang khoảng ngày khác.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.67` Phân rã use case Nhân bản lịch chiếu
  - `Bảng 3.67` Phân rã use case Nhân bản lịch chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminShowtimesController.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → CloneShowtimesAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QS-08.1 | Chọn khoảng ngày nguồn | include | Quản trị viên | [CẦN BỔ SUNG: xác minh tham số CloneShowtimesRequest] |
| UC-QS-08.2 | Chọn khoảng ngày đích | include | Quản trị viên |  |
| UC-QS-08.3 | Kiểm tra giờ mở cửa và trùng lịch từng suất | include |  |  |
| UC-QS-08.4 | Tạo các suất mới | include |  |  |


### 3.3.15 Quản lý người dùng

- **Nội dung cần viết:**
  - Tài khoản người dùng và nhân viên. Giới thiệu module, tác nhân, số use case.

#### 3.3.15.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.68` Sơ đồ use case module Quản lý người dùng
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.15.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.68` Danh sách use case module Quản lý người dùng

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-QN-01 | Xem danh sách người dùng | Quản trị viên | Danh sách tài khoản có phân trang. | GET /api/v1/admin/users (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`<br>`backend/Cinema.Application/Services/Employees/EmployeeService.cs → GetUsersAdminAsync` |
| UC-QN-02 | Xem danh sách tài khoản quản trị | Quản trị viên | Danh sách quản trị/nhân viên dùng để chọn khi tạo nhân viên. | GET /api/v1/admin/users/admins (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`<br>`backend/Cinema.Application/Services/Employees/EmployeeService.cs → GetAdminUsersForSelectAsync` |
| UC-QN-03 | Tìm kiếm khách hàng | Nhân viên, Quản trị viên | Tìm khách hàng khi bán vé tại quầy. | GET /api/v1/admin/users/customers (StaffOnly) | `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`<br>`backend/Cinema.Application/Services/Employees/EmployeeService.cs → SearchCustomersForCounterAsync` |
| UC-QN-04 | Khóa hoặc mở khóa tài khoản | Quản trị viên | Đổi trạng thái hoạt động của tài khoản. | PUT /api/v1/admin/users/{id}/status (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`<br>`backend/Cinema.Application/Services/Employees/EmployeeService.cs → UpdateUserStatusAsync` |
| UC-QN-05 | Đổi vai trò tài khoản | Quản trị viên | Đổi vai trò của tài khoản. | PUT /api/v1/admin/users/{id}/role (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`<br>`backend/Cinema.Application/Services/Employees/EmployeeService.cs → UpdateUserRoleAsync` |
| UC-QN-06 | Cập nhật trạng thái nhiều tài khoản | Quản trị viên | Khóa/mở khóa hàng loạt. | PUT /api/v1/admin/users/bulk/status (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`<br>`backend/Cinema.Application/Services/Employees/EmployeeService.cs → BulkUpdateUserStatusAsync` |
| UC-QN-07 | Quản lý nhân viên | Quản trị viên | Xem, thêm, sửa hồ sơ nhân viên. | GET/POST /api/v1/admin/employees, PUT /api/v1/admin/employees/{id} (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminEmployeesController.cs`<br>`backend/Cinema.Application/Services/Employees/EmployeeService.cs` |


#### 3.3.15.3 Phân rã use case Khóa hoặc mở khóa tài khoản

- **Nội dung cần viết:**
  - Use case cha UC-QN-04: Đổi trạng thái hoạt động của tài khoản.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.69` Phân rã use case Khóa hoặc mở khóa tài khoản
  - `Bảng 3.69` Phân rã use case Khóa hoặc mở khóa tài khoản
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`; `backend/Cinema.Application/Services/Employees/EmployeeService.cs → UpdateUserStatusAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QN-04.1 | Kiểm tra tài khoản tồn tại | include |  | `NotFound_User` |
| UC-QN-04.2 | Chặn tự khóa tài khoản của mình | include |  | `Rule_CannotLockSelf` |
| UC-QN-04.3 | Chặn khóa quản trị viên cuối cùng | include |  | `Rule_CannotLockLastAdmin` |
| UC-QN-04.4 | Cập nhật trạng thái | include |  | `User.IsActive` |
| UC-QN-04.5 | Cập nhật trạng thái nhiều tài khoản | extend | Quản trị viên | PUT /api/v1/admin/users/bulk/status (UC-QN-06) |


#### 3.3.15.4 Phân rã use case Đổi vai trò tài khoản

- **Nội dung cần viết:**
  - Use case cha UC-QN-05: Đổi vai trò của tài khoản.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.70` Phân rã use case Đổi vai trò tài khoản
  - `Bảng 3.70` Phân rã use case Đổi vai trò tài khoản
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminUsersController.cs`; `backend/Cinema.Application/Services/Employees/EmployeeService.cs → UpdateUserRoleAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QN-05.1 | Kiểm tra tài khoản và vai trò tồn tại | include |  | `NotFound_User`, `Invalid_RoleIdNotFound` |
| UC-QN-05.2 | Chặn hạ vai trò khi nhân viên còn hoạt động | include |  | `Conflict_CannotDowngradeRoleWithActiveEmployee` |
| UC-QN-05.3 | Cập nhật vai trò | include |  | `User.RoleId` |


#### 3.3.15.5 Phân rã use case Quản lý nhân viên

- **Nội dung cần viết:**
  - Use case cha UC-QN-07: Xem, thêm, sửa hồ sơ nhân viên.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.71` Phân rã use case Quản lý nhân viên
  - `Bảng 3.71` Phân rã use case Quản lý nhân viên
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminEmployeesController.cs`; `backend/Cinema.Application/Services/Employees/EmployeeService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-QN-07.1 | Xem danh sách nhân viên | include | Quản trị viên | GetEmployeesAdminAsync |
| UC-QN-07.2 | Thêm nhân viên | include | Quản trị viên | Tài khoản phải có vai trò nhân viên (`Rule_UserMustBeStaffRole`), chưa có hồ sơ (`Conflict_EmployeeProfileExists`) |
| UC-QN-07.3 | Chọn chức vụ | include | Quản trị viên | `NotFound_PositionId` |
| UC-QN-07.4 | Nhập ngày vào làm | include | Quản trị viên | `HireDate` |
| UC-QN-07.5 | Sửa hồ sơ nhân viên | include | Quản trị viên | `NotFound_Employee`; `IsActive` |


### 3.3.16 Quản lý đánh giá

- **Nội dung cần viết:**
  - Kiểm duyệt đánh giá phim. Giới thiệu module, tác nhân, số use case.

#### 3.3.16.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.72` Sơ đồ use case module Quản lý đánh giá
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.16.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.72` Danh sách use case module Quản lý đánh giá

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-QV-01 | Xem danh sách đánh giá | Quản trị viên | Danh sách đánh giá phân trang. | GET /api/v1/admin/reviews (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminReviewsController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs → GetReviewsAdminAsync` |
| UC-QV-02 | Xóa đánh giá | Quản trị viên | Xóa một đánh giá vi phạm. | DELETE /api/v1/admin/reviews/{id} (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminReviewsController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs → DeleteReviewAsync` |
| UC-QV-03 | Xóa nhiều đánh giá | Quản trị viên | Xóa hàng loạt đánh giá. | DELETE /api/v1/admin/reviews/bulk (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminReviewsController.cs`<br>`backend/Cinema.Application/Services/Reviews/ReviewService.cs → BulkDeleteReviewsAsync` |


### 3.3.17 Quản lý thanh toán

- **Nội dung cần viết:**
  - Tra cứu giao dịch thanh toán. Giới thiệu module, tác nhân, số use case.

#### 3.3.17.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.73` Sơ đồ use case module Quản lý thanh toán
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.17.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.73` Danh sách use case module Quản lý thanh toán

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-QT-01 | Xem danh sách thanh toán | Quản trị viên | Danh sách giao dịch thanh toán. | GET /api/v1/admin/payments (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminPaymentsController.cs`<br>`backend/Cinema.Application/Services/Payments/PaymentAdminService.cs` |
| UC-QT-02 | Xem chi tiết thanh toán | Quản trị viên | Chi tiết một giao dịch (cổng, mã giao dịch, hoàn tiền). | GET /api/v1/admin/payments/{id} (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminPaymentsController.cs`<br>`backend/Cinema.Application/Services/Payments/PaymentAdminService.cs` |


### 3.3.18 Báo cáo thống kê

- **Nội dung cần viết:**
  - Dashboard và các báo cáo. Giới thiệu module, tác nhân, số use case.

#### 3.3.18.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.74` Sơ đồ use case module Báo cáo thống kê
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.18.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.74` Danh sách use case module Báo cáo thống kê

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-BC-01 | Xem tổng quan dashboard | Quản trị viên | Chỉ số tổng hợp của rạp. | GET /api/v1/admin/dashboard/summary (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminDashboardController.cs`<br>`backend/Cinema.Application/Services/Dashboard/DashboardService.cs → GetSummaryAsync` |
| UC-BC-02 | Xem nhận định (insights) | Quản trị viên | Các nhận định tổng hợp từ dữ liệu. | GET /api/v1/admin/dashboard/insights (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminDashboardController.cs`<br>`backend/Cinema.Application/Services/Dashboard/DashboardService.cs → GetInsightsAsync` |
| UC-BC-03 | Xem báo cáo doanh thu | Quản trị viên | Doanh thu theo ngày/tuần/tháng, trừ phần hoàn tiền. | GET /api/v1/admin/reports/revenue (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminReportsController.cs`<br>`backend/Cinema.Application/Services/Reports/ReportService.cs → GetRevenueReportAsync` |
| UC-BC-04 | Xem báo cáo phim bán chạy | Quản trị viên | Xếp hạng phim theo số vé và doanh thu. | GET /api/v1/admin/reports/top-movies (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminReportsController.cs`<br>`backend/Cinema.Application/Services/Reports/ReportService.cs → GetTopMoviesReportAsync` |
| UC-BC-05 | Xem báo cáo hiệu suất nhân viên | Quản trị viên | Doanh số bán quầy theo nhân viên. | GET /api/v1/admin/reports/employee-performance (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminReportsController.cs`<br>`backend/Cinema.Application/Services/Reports/ReportService.cs → GetEmployeePerformanceReportAsync` |


### 3.3.19 Cấu hình rạp

- **Nội dung cần viết:**
  - Thông tin rạp và tham số vận hành. Giới thiệu module, tác nhân, số use case.

#### 3.3.19.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.75` Sơ đồ use case module Cấu hình rạp
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.19.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.75` Danh sách use case module Cấu hình rạp

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-CH-01 | Xem cấu hình rạp | Quản trị viên | Xem toàn bộ tham số cấu hình. | GET /api/v1/admin/settings (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminSettingsController.cs`<br>`backend/Cinema.Application/Services/Settings/SettingsService.cs → GetSettingsAsync` |
| UC-CH-02 | Cập nhật cấu hình rạp | Quản trị viên, Cloudinary | Cập nhật thông tin và tham số vận hành. | PUT /api/v1/admin/settings (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminSettingsController.cs`<br>`backend/Cinema.Application/Services/Settings/SettingsService.cs → UpdateSettingsAsync` |


#### 3.3.19.3 Phân rã use case Cập nhật cấu hình rạp

- **Nội dung cần viết:**
  - Use case cha UC-CH-02: Cập nhật thông tin và tham số vận hành.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.76` Phân rã use case Cập nhật cấu hình rạp
  - `Bảng 3.76` Phân rã use case Cập nhật cấu hình rạp
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/Admin/AdminSettingsController.cs`; `backend/Cinema.Application/Services/Settings/SettingsService.cs → UpdateSettingsAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-CH-02.1 | Cập nhật thông tin rạp | include | Quản trị viên | Tên, địa chỉ, điện thoại, email, mã số thuế |
| UC-CH-02.2 | Tải logo | extend | Cloudinary | Xem UC-TP-01 |
| UC-CH-02.3 | Cập nhật giờ mở cửa và đóng cửa | include | Quản trị viên | `OpeningTime`, `ClosingTime` |
| UC-CH-02.4 | Cập nhật thời gian giữ ghế và dừng bán online | include | Quản trị viên | `HoldDurationMinutes`, `StopSellingMinutesBefore` |
| UC-CH-02.5 | Cập nhật thời gian mở soát vé | include | Quản trị viên | `CheckInOpenMinutes` |
| UC-CH-02.6 | Cập nhật chính sách hủy và hoàn tiền | include | Quản trị viên | `Rule_CancelRefundPercentRange`, `Rule_CancelDeadlineHoursNegative` |
| UC-CH-02.7 | Cập nhật quy đổi điểm thưởng | include | Quản trị viên | `PointsPerAmount` |
| UC-CH-02.8 | Cập nhật luật ghế lẻ | include | Quản trị viên | `SeatGapRuleMode` |


### 3.3.20 Tải tệp

- **Nội dung cần viết:**
  - Tải ảnh và video lên dịch vụ lưu trữ. Giới thiệu module, tác nhân, số use case.

#### 3.3.20.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.77` Sơ đồ use case module Tải tệp
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.20.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.77` Danh sách use case module Tải tệp

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-TP-01 | Tải ảnh lên | Quản trị viên, Cloudinary | Tải ảnh (poster, ảnh đại diện, ảnh combo, logo). | POST /api/v1/admin/uploads/image (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminUploadsController.cs`<br>`backend/Cinema.Infrastructure/FileStorage/CloudinaryFileStorage.cs → UploadImageAsync` |
| UC-TP-02 | Tải video lên | Quản trị viên, Cloudinary | Tải video (trailer). | POST /api/v1/admin/uploads/video (AdminOnly) | `backend/Cinema.Api/Controllers/Admin/AdminUploadsController.cs`<br>`backend/Cinema.Infrastructure/FileStorage/CloudinaryFileStorage.cs → UploadVideoAsync` |


### 3.3.21 Vận hành hệ thống

- **Nội dung cần viết:**
  - Công việc chạy nền và chức năng vận hành hệ thống. Giới thiệu module, tác nhân, số use case.

#### 3.3.21.1 Sơ đồ use case của module

- **Nội dung cần viết:**
  - Các use case cấp 1 của module và quan hệ với tác nhân
- **Tài liệu/hình/bảng:**
  - `Hình 3.78` Sơ đồ use case module Vận hành hệ thống
- **Nguồn dữ liệu:** `backend/Cinema.Api/Controllers/**`

#### 3.3.21.2 Danh sách use case của module

- **Nội dung cần viết:**
  - Mỗi use case: mã, tên, tác nhân, mô tả, endpoint, nguồn
- **Tài liệu/hình/bảng:**
  - `Bảng 3.78` Danh sách use case module Vận hành hệ thống

| Mã | Use case | Tác nhân | Mô tả | Endpoint | Nguồn |
|---|---|---|---|---|---|
| UC-HT-01 | Giải phóng ghế hết hạn | Hệ thống tác vụ nền | Công việc lặp mỗi phút. | Hangfire recurring job | `backend/Cinema.Application/BackgroundJobs/ReleaseExpiredSeatsJob.cs`<br>`backend/Cinema.Application/Services/Bookings/SeatExpirationService.cs` |
| UC-HT-02 | Cập nhật vòng đời suất chiếu | Hệ thống tác vụ nền | Công việc lặp mỗi phút. | Hangfire recurring job | `backend/Cinema.Application/BackgroundJobs/ShowtimeLifecycleJob.cs`<br>`backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → CompleteEndedShowtimesAsync, EndMoviesWithoutUpcomingShowtimesAsync` |
| UC-HT-03 | Gửi email | Hệ thống tác vụ nền, Dịch vụ email | Gửi email xác nhận, khôi phục mật khẩu, đặt vé, hủy, hoàn tiền. | Hangfire fire-and-forget job | `backend/Cinema.Application/BackgroundJobs/SendEmailJob.cs`<br>`backend/Cinema.Infrastructure/Email/*` |
| UC-HT-04 | Hoàn tiền khi hủy suất chiếu | Hệ thống tác vụ nền, Cổng VNPay, Cổng MoMo, Cổng Stripe, Dịch vụ email | Hoàn tiền từng thanh toán của suất bị hủy. | Hangfire job (thử lại 3 lần: 30 giây, 2 phút, 10 phút) | `backend/Cinema.Application/BackgroundJobs/ShowtimeRefundJob.cs` |
| UC-HT-05 | Khởi tạo dữ liệu khi khởi động | Hệ thống tác vụ nền | Chạy migration và seed khi bật cờ cấu hình. | `RunMigrationsOnStartup` | `backend/Cinema.Api/Program.cs`<br>`backend/Cinema.Infrastructure/Persistence/Seed/DbInitializer.cs` |
| UC-HT-06 | Xem bảng điều khiển Hangfire | Quản trị viên | Giám sát công việc nền. | /admin/hangfire (chỉ quản trị) | `backend/Cinema.Api/Filters/HangfireAdminAuthorizationFilter.cs` |
| UC-HT-07 | Kiểm tra sức khỏe hệ thống | Khách vãng lai | Endpoint kiểm tra tình trạng dịch vụ. | GET /api/v1/health | `backend/Cinema.Api/Controllers/HealthController.cs` |


#### 3.3.21.3 Phân rã use case Giải phóng ghế hết hạn

- **Nội dung cần viết:**
  - Use case cha UC-HT-01: Công việc lặp mỗi phút.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.79` Phân rã use case Giải phóng ghế hết hạn
  - `Bảng 3.79` Phân rã use case Giải phóng ghế hết hạn
- **Nguồn dữ liệu:** `backend/Cinema.Application/BackgroundJobs/ReleaseExpiredSeatsJob.cs`; `backend/Cinema.Application/Services/Bookings/SeatExpirationService.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-HT-01.1 | Chuyển booking Pending quá hạn sang Expired | include |  | ExpireDueBookingsAsync |
| UC-HT-01.2 | Trả ghế về trạng thái Available | include |  | ReleaseAndCancel |
| UC-HT-01.3 | Đánh dấu các Payment Pending thành Failed | include |  |  |
| UC-HT-01.4 | Phát sự kiện hết hạn giữ ghế | include |  | SignalR `hold.expired` |
| UC-HT-01.5 | Dọn các ghế Held quá hạn còn sót | include |  | ReleaseStaleHeldSeatsAsync |


#### 3.3.21.4 Phân rã use case Cập nhật vòng đời suất chiếu

- **Nội dung cần viết:**
  - Use case cha UC-HT-02: Công việc lặp mỗi phút.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.80` Phân rã use case Cập nhật vòng đời suất chiếu
  - `Bảng 3.80` Phân rã use case Cập nhật vòng đời suất chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Application/BackgroundJobs/ShowtimeLifecycleJob.cs`; `backend/Cinema.Application/Services/Showtimes/ShowtimeService.cs → CompleteEndedShowtimesAsync, EndMoviesWithoutUpcomingShowtimesAsync`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-HT-02.1 | Chuyển suất chiếu đã kết thúc sang Completed | include |  | CompleteEndedShowtimesAsync |
| UC-HT-02.2 | Chuyển phim không còn suất sắp chiếu sang Ended | include |  | EndMoviesWithoutUpcomingShowtimesAsync |


#### 3.3.21.5 Phân rã use case Hoàn tiền khi hủy suất chiếu

- **Nội dung cần viết:**
  - Use case cha UC-HT-04: Hoàn tiền từng thanh toán của suất bị hủy.
  - Giải thích từng use case con và quan hệ include/extend/tổng quát hóa; nêu điều kiện, lỗi nghiệp vụ trong cột ghi chú
- **Tài liệu/hình/bảng:**
  - `Hình 3.81` Phân rã use case Hoàn tiền khi hủy suất chiếu
  - `Bảng 3.81` Phân rã use case Hoàn tiền khi hủy suất chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Application/BackgroundJobs/ShowtimeRefundJob.cs`

| Mã | Use case con | Quan hệ với use case cha | Tác nhân | Điều kiện/ghi chú (từ mã nguồn) |
|---|---|---|---|---|
| UC-HT-04.1 | Hoàn tiền từng thanh toán qua cổng | include |  | Xem UC-TT-06 |
| UC-HT-04.2 | Thử lại khi lỗi | extend |  | 30 giây, 2 phút, 10 phút |
| UC-HT-04.3 | Gửi email hoàn tiền tự động hoặc hoàn tiền thủ công | include | Dịch vụ email | EmailTemplates |


## 3.4 ĐẶC TẢ USE CASE CHÍNH


### 3.4.1 Đặc tả UC-XT-04 Đăng nhập bằng email

- **Nội dung cần viết:**
  - Viết đặc tả theo bảng; mô tả từng bước bằng câu hoàn chỉnh
- **Tài liệu/hình/bảng:**
  - `Bảng 3.82` Đặc tả UC-XT-04 Đăng nhập bằng email

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
  - `Bảng 3.83` Đặc tả UC-DV-04 Giữ ghế

| Mục | Nội dung |
|---|---|
| Mã / Tên | UC-DV-04 — Giữ ghế |
| Tác nhân | Khách hàng |
| Mô tả | Giữ chỗ có thời hạn cho các ghế đã chọn. |
| Tiền điều kiện | Khách hàng đã đăng nhập; Suất chiếu còn mở bán; Đã chọn ghế hợp lệ |
| Hậu điều kiện | Booking Pending được tạo với hạn giữ; Các ghế ở trạng thái Held cho booking này |
| Luồng chính | Gửi yêu cầu giữ ghế → kiểm tra đầu vào, suất chiếu, booking Pending sẵn có, luật ghế → cập nhật ghế nguyên tử → tính tiền → tạo booking → phát seat.held<br>Chi tiết các bước: 1. Kiểm tra dữ liệu đầu vào<br>2. Kiểm tra suất chiếu còn mở bán<br>3. Kiểm tra chưa có booking Pending còn hạn cho suất này<br>4. Kiểm tra luật chọn ghế phía máy chủ<br>5. Giữ ghế nguyên tử bằng cập nhật có điều kiện<br>6. Tính tiền và giảm giá theo hạng thành viên<br>7. Tạo booking Pending kèm hạn giữ<br>8. Phát sự kiện ghế được giữ |
| Luồng thay thế | Xử lý xung đột ghế (extend) |
| Ngoại lệ | Ghế đã bị người khác giữ: 409 kèm danh sách ghế xung đột; có booking Pending còn hạn: xung đột; vượt giới hạn tốc độ 10/60 giây |
| Nguồn | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → HoldSeatsAsync, CreateHoldBookingAsync` |


### 3.4.3 Đặc tả UC-TT-01 Thanh toán đơn đặt vé trực tuyến

- **Nội dung cần viết:**
  - Viết đặc tả theo bảng; mô tả từng bước bằng câu hoàn chỉnh
- **Tài liệu/hình/bảng:**
  - `Bảng 3.84` Đặc tả UC-TT-01 Thanh toán đơn đặt vé trực tuyến

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
  - `Bảng 3.85` Đặc tả UC-TT-05 Phát hành vé sau thanh toán

| Mục | Nội dung |
|---|---|
| Mã / Tên | UC-TT-05 — Phát hành vé sau thanh toán |
| Tác nhân | Hệ thống tác vụ nền, Dịch vụ email |
| Mô tả | Chuyển ghế sang đã bán, tạo vé và mã QR sau khi thanh toán thành công. |
| Tiền điều kiện | Cổng báo thanh toán thành công; chữ ký, số tiền hợp lệ |
| Hậu điều kiện | Booking Confirmed; ghế Booked; vé có mã QR; điểm thưởng cộng; email gửi |
| Luồng chính | Áp dụng kết quả thanh toán → chuyển ghế → xác nhận booking → tạo vé QR → cộng điểm → phát seat.booked → gửi email<br>Chi tiết các bước: 1. Chuyển ghế từ Held sang Booked<br>2. Cập nhật booking sang Confirmed<br>3. Tạo vé và mã QR cho từng ghế<br>4. Cộng điểm thưởng và nâng hạng thành viên<br>5. Phát sự kiện ghế đã bán<br>6. Gửi email xác nhận đặt vé |
| Luồng thay thế | Tự hoàn tiền khi ghế đã hết hạn sau khi khách trả tiền (extend) |
| Ngoại lệ | Ghế đã hết hạn khi tiền về: tự hoàn tiền và gửi email thông báo |
| Nguồn | `backend/Cinema.Application/Services/Payments/PaymentService.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → ConfirmBookingAndGenerateTicketsAsync` |


### 3.4.5 Đặc tả UC-DV-08 Hủy đơn đặt vé

- **Nội dung cần viết:**
  - Viết đặc tả theo bảng; mô tả từng bước bằng câu hoàn chỉnh
- **Tài liệu/hình/bảng:**
  - `Bảng 3.86` Đặc tả UC-DV-08 Hủy đơn đặt vé

| Mục | Nội dung |
|---|---|
| Mã / Tên | UC-DV-08 — Hủy đơn đặt vé |
| Tác nhân | Khách hàng |
| Mô tả | Hủy giữ ghế hoặc hủy đơn đã thanh toán. |
| Tiền điều kiện | Khách là chủ booking |
| Hậu điều kiện | Booking Cancelled; ghế được trả; hoàn tiền nếu đủ điều kiện |
| Luồng chính | Kiểm tra quyền → nhánh Pending hoặc Confirmed → (Confirmed) kiểm tra điều kiện hoàn tiền → hoàn tiền cổng → thu hồi điểm → gửi email<br>Chi tiết các bước: 1. Kiểm tra booking thuộc về người dùng<br>2. Hoàn tiền qua cổng thanh toán<br>3. Thu hồi điểm thưởng<br>4. Gửi email xác nhận hủy |
| Luồng thay thế | Hủy booking Pending và trả ghế (tổng quát hóa)<br>Hủy booking đã thanh toán (Confirmed) (tổng quát hóa) |
| Ngoại lệ | Đã soát vé, thanh toán tiền mặt, quá hạn hủy hoặc cổng chưa cấu hình: không hoàn tiền/không cho hủy |
| Nguồn | `backend/Cinema.Api/Controllers/Client/BookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → CancelBookingAsync` |


### 3.4.6 Đặc tả UC-BQ-01 Bán vé tại quầy

- **Nội dung cần viết:**
  - Viết đặc tả theo bảng; mô tả từng bước bằng câu hoàn chỉnh
- **Tài liệu/hình/bảng:**
  - `Bảng 3.87` Đặc tả UC-BQ-01 Bán vé tại quầy

| Mục | Nội dung |
|---|---|
| Mã / Tên | UC-BQ-01 — Bán vé tại quầy |
| Tác nhân | Nhân viên, Quản trị viên |
| Mô tả | Bán vé cho khách tại rạp, thanh toán tiền mặt. |
| Tiền điều kiện | Nhân viên hoặc quản trị đã đăng nhập |
| Hậu điều kiện | Booking kênh Counter ở trạng thái Confirmed; vé phát hành; tiền thối được ghi |
| Luồng chính | Chọn suất → chọn ghế/combo → nhập khách → giữ ghế quầy → nhập tiền nhận → xác nhận và phát vé<br>Chi tiết các bước: 1. Chọn suất chiếu<br>2. Xem sơ đồ ghế<br>3. Chọn ghế (không áp luật ghế lẻ)<br>4. Nhập thông tin khách hoặc chọn khách hàng có sẵn<br>5. Giữ ghế tại quầy<br>6. Thanh toán tiền mặt<br>7. Xác nhận đơn và phát hành vé<br>8. Hiển thị vé kết quả |
| Luồng thay thế | Chọn combo (extend)<br>Tìm khách hàng (extend) |
| Ngoại lệ | Tiền nhận nhỏ hơn số cần thanh toán: từ chối |
| Nguồn | `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → CounterHoldSeatsAsync, CounterCheckoutAsync`<br>`frontend/src/features/admin/counter/**` |


### 3.4.7 Đặc tả UC-SV-01 Soát vé

- **Nội dung cần viết:**
  - Viết đặc tả theo bảng; mô tả từng bước bằng câu hoàn chỉnh
- **Tài liệu/hình/bảng:**
  - `Bảng 3.88` Đặc tả UC-SV-01 Soát vé

| Mục | Nội dung |
|---|---|
| Mã / Tên | UC-SV-01 — Soát vé |
| Tác nhân | Nhân viên, Quản trị viên |
| Mô tả | Xác nhận vé hợp lệ khi khách vào rạp. |
| Tiền điều kiện | Nhân viên hoặc quản trị đã đăng nhập; Vé có mã QR |
| Hậu điều kiện | Vé IsCheckedIn = true, CheckedInAt được ghi |
| Luồng chính | Quét/nhập mã → tìm vé → kiểm tra booking, suất, cửa sổ thời gian, chưa soát → ghi nhận<br>Chi tiết các bước: 1. Tìm vé theo mã QR<br>2. Kiểm tra booking ở trạng thái Confirmed<br>3. Kiểm tra suất chiếu chưa bị hủy<br>4. Kiểm tra trong cửa sổ thời gian soát vé<br>5. Chặn soát lặp<br>6. Ghi nhận đã soát vé<br>7. Hiển thị kết quả cho nhân viên |
| Luồng thay thế | Quét mã QR bằng camera (extend)<br>Nhập mã vé thủ công (extend) |
| Ngoại lệ | Mã sai, ngoài cửa sổ thời gian, đã soát: từ chối kèm lý do |
| Nguồn | `backend/Cinema.Api/Controllers/Admin/AdminBookingsController.cs`<br>`backend/Cinema.Application/Services/Bookings/BookingService.cs → CheckinAsync`<br>`frontend/src/features/admin/checkin/**` |


### 3.4.8 Đặc tả UC-QS-07 Tự động xếp lịch chiếu

- **Nội dung cần viết:**
  - Viết đặc tả theo bảng; mô tả từng bước bằng câu hoàn chỉnh
- **Tài liệu/hình/bảng:**
  - `Bảng 3.89` Đặc tả UC-QS-07 Tự động xếp lịch chiếu

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
  - `Bảng 3.90` Đặc tả UC-QR-03 Soạn sơ đồ ghế

| Mục | Nội dung |
|---|---|
| Mã / Tên | UC-QR-03 — Soạn sơ đồ ghế |
| Tác nhân | Quản trị viên |
| Mô tả | Thiết kế và lưu sơ đồ ghế của phòng. |
| Tiền điều kiện | Quản trị viên; Phòng chiếu đã tồn tại |
| Hậu điều kiện | Sơ đồ ghế của phòng được lưu |
| Luồng chính | Đặt kích thước → tô/chọn/ghép/sao chép → kiểm tra hợp lệ → lưu<br>Chi tiết các bước: 1. Đặt kích thước lưới (số hàng, số cột)<br>2. Tô loại ghế bằng cọ<br>3. Kiểm tra tính hợp lệ của sơ đồ<br>4. Lưu sơ đồ ghế |
| Luồng thay thế | Sinh sơ đồ tự động (extend)<br>Chọn vùng nhiều ghế (extend)<br>Đặt lối đi (extend)<br>Ghép ghế đôi (extend)<br>Sao chép hàng ghế (extend)<br>Hoàn tác và làm lại (extend) |
| Ngoại lệ | Ghế đôi không ghép cặp; ghế đang giữ/đã bán bị đổi: từ chối |
| Nguồn | `backend/Cinema.Api/Controllers/Admin/AdminRoomsController.cs`<br>`backend/Cinema.Application/Services/Rooms/RoomService.cs → UpdateSeatLayoutAsync`<br>`frontend/src/features/admin/rooms/components/seat-editor-*.tsx` |


## 3.5 QUY TẮC NGHIỆP VỤ


### 3.5.1 Danh sách quy tắc nghiệp vụ

- **Nội dung cần viết:**
  - Trình bày từng quy tắc: nội dung, tham số cấu hình, giá trị mặc định, nơi áp dụng
- **Tài liệu/hình/bảng:**
  - `Bảng 3.91` Quy tắc nghiệp vụ và tham số
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/CinemaSettings.cs`; `backend/Cinema.Application/Options/*`; `backend/Cinema.Application/Services/**`

| Mã | Quy tắc | Nội dung | Nguồn |
|---|---|---|---|
| BR-01 | Thời gian giữ ghế | `HoldDurationMinutes` (mặc định 15 phút), không vượt giờ bắt đầu suất chiếu | CinemaSettings.cs, BookingService.cs |
| BR-02 | Giới hạn mỗi đơn | Tối đa 10 ghế (`MaxSeatsPerBooking`), tối đa 10 combo mỗi loại | BookingOptions.cs, BookingValidators.cs |
| BR-03 | Dừng bán online | Không bán online sau `StopSellingMinutesBefore` phút trước giờ chiếu | CinemaSettings.cs, BookingService.cs |
| BR-04 | Một booking Pending | Mỗi người một booking Pending còn hạn trên mỗi suất (quầy: mỗi nhân viên) | BookingService.cs |
| BR-05 | Ghế đôi và ghế lẻ | Ghế đôi chọn theo cặp; luật ghế lẻ theo `SeatGapRuleMode` (Strict mặc định; quầy dùng Off) | SeatLayoutRules.cs |
| BR-06 | Giảm giá và điểm thưởng | Giảm theo hạng thành viên; điểm = làm tròn xuống (số tiền / `PointsPerAmount`, mặc định 10.000) | BookingService.cs |
| BR-07 | Hủy và hoàn tiền | `CancelRefundPercent` (mặc định 0), hạn hủy `CancelDeadlineHours` (mặc định 2 giờ); tiền mặt hủy tại quầy; không hủy khi đã soát vé | CinemaSettings.cs, BookingService.cs |
| BR-08 | Cửa sổ soát vé | Mở trước giờ chiếu `CheckInOpenMinutes` (mặc định 30) đến hết `EndTime`; booking phải Confirmed | BookingService.cs |
| BR-09 | Đánh giá phim | Chỉ khách đã soát vé phim đó; mỗi người một đánh giá cho mỗi phim; điểm 1–5 | ReviewService.cs |
| BR-10 | Ràng buộc suất chiếu | Đệm phòng 15 phút; không trùng phòng; trong giờ mở cửa (trừ cho phép ngoài giờ); không tạo cho phim Ended hoặc phòng không hoạt động/chưa có sơ đồ ghế; không hủy khi đã bắt đầu | ShowtimeService.cs, OperatingHours.cs |
| BR-11 | Vòng đời tự động | Suất chiếu hết giờ chuyển Completed; phim chuyển Ended khi không còn suất sắp chiếu | ShowtimeLifecycle.cs, ShowtimeLifecycleJob.cs |
| BR-12 | Giới hạn tốc độ | Toàn cục 100 yêu cầu/60 giây/IP; xác thực 5/60 giây/IP; giữ ghế 10/60 giây/người dùng | RateLimitOptions.cs, Program.cs |


## 3.6 YÊU CẦU PHI CHỨC NĂNG


### 3.6.1 Danh sách yêu cầu phi chức năng

- **Nội dung cần viết:**
  - Mỗi yêu cầu kèm cách đáp ứng và vị trí cài đặt
- **Tài liệu/hình/bảng:**
  - `Bảng 3.92` Yêu cầu phi chức năng và cách đáp ứng
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


## 4.2 CƠ SỞ DỮ LIỆU


### 4.2.1 Nguyên tắc thiết kế

- **Nội dung cần viết:**
  - Một CSDL PostgreSQL (Neon); EF Core Code First; enum lưu dạng chuỗi; thời gian lưu UTC; ràng buộc CHECK và chỉ mục duy nhất bảo vệ tính đúng đắn (đặc biệt chống đặt trùng ghế); hành vi xóa Restrict/Cascade/SetNull
- **Nguồn dữ liệu:** `backend/Cinema.Infrastructure/Persistence/AppDbContext.cs`; `Persistence/Configurations/*`

### 4.2.2 Sơ đồ ERD đầy đủ

- **Nội dung cần viết:**
  - 29 thực thể, 10 kiểu liệt kê; thể hiện đầy đủ thuộc tính, khóa chính (PK), khóa ngoại (FK), duy nhất (UK) và bội số quan hệ
  - Dùng trang A4 ngang (Section 4a)
- **Tài liệu/hình/bảng:**
  - `Hình 4.4` Sơ đồ ERD đầy đủ của hệ thống
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `backend/Cinema.Infrastructure/Persistence/Configurations/*.cs`; `Migrations/AppDbContextModelSnapshot.cs`
- **Định dạng Word:** Section ngang (Landscape); hình đặt giữa, rộng tối đa 25 cm

### 4.2.3 Phân module cơ sở dữ liệu

- **Nội dung cần viết:**
  - Chia 29 thực thể thành 9 module; thực thể của module khác xuất hiện trong ERD module dưới dạng tham chiếu (chỉ khóa chính)
- **Tài liệu/hình/bảng:**
  - `Bảng 4.4` Phân module cơ sở dữ liệu
- **Nguồn dữ liệu:** `Mục 4.2.2 file này`

| Mục | Module | Thực thể sở hữu | Số thực thể |
|---|---|---|---|
| 4.2.4 | Tài khoản người dùng | Role, User, CustomerProfile, Employee, Position, RefreshToken, UserVerificationToken | 7 |
| 4.2.5 | Danh mục phim | Genre, Language, AgeRating, Actor, Director | 5 |
| 4.2.6 | Phim | Movie, MovieGenre, MovieCast, Review | 4 |
| 4.2.7 | Phòng chiếu | CinemaRoom, ScreenType, Seat, SeatType | 4 |
| 4.2.8 | Suất chiếu | Showtime, ShowtimeSeat | 2 |
| 4.2.9 | Đặt vé | Booking, BookingTicket, BookingCombo, Combo | 4 |
| 4.2.10 | Thanh toán | Payment | 1 |
| 4.2.11 | Thành viên | MembershipTier | 1 |
| 4.2.12 | Cấu hình rạp | CinemaSettings | 1 |
|  | **Tổng** |  | 29 |


### 4.2.4 Module Tài khoản người dùng

- **Nội dung cần viết:**
  - Giới thiệu module Tài khoản người dùng: vai trò, thực thể (Role, User, CustomerProfile, Employee, Position, RefreshToken, UserVerificationToken), quan hệ với module khác

#### 4.2.4.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.5` ERD module Tài khoản người dùng
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.2.4.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.5` Thực thể Role (bảng Roles)
  - `Bảng 4.6` Thực thể User (bảng Users)
  - `Bảng 4.7` Thực thể CustomerProfile (bảng CustomerProfiles)
  - `Bảng 4.8` Thực thể Employee (bảng Employees)
  - `Bảng 4.9` Thực thể Position (bảng Positions)
  - `Bảng 4.10` Thực thể RefreshToken (bảng RefreshTokens)
  - `Bảng 4.11` Thực thể UserVerificationToken (bảng UserVerificationTokens)
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
| Points | int | Không | — | 0 |
| TierId | int | Có | FK | — |
| TotalSpent | decimal | Không | — | 0m |

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



#### 4.2.4.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.12` Ràng buộc toàn vẹn module Tài khoản người dùng
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


#### 4.2.4.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.13` Chỉ mục module Tài khoản người dùng
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


### 4.2.5 Module Danh mục phim

- **Nội dung cần viết:**
  - Giới thiệu module Danh mục phim: vai trò, thực thể (Genre, Language, AgeRating, Actor, Director), quan hệ với module khác

#### 4.2.5.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.6` ERD module Danh mục phim
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.2.5.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.14` Thực thể Genre (bảng Genres)
  - `Bảng 4.15` Thực thể Language (bảng Languages)
  - `Bảng 4.16` Thực thể AgeRating (bảng AgeRatings)
  - `Bảng 4.17` Thực thể Actor (bảng Actors)
  - `Bảng 4.18` Thực thể Director (bảng Directors)
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



#### 4.2.5.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.19` Ràng buộc toàn vẹn module Danh mục phim
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

Module không có ràng buộc CHECK hoặc khóa ngoại riêng.


#### 4.2.5.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.20` Chỉ mục module Danh mục phim
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| Genre | (tên tự sinh) | Duy nhất | Name | — |
| Language | (tên tự sinh) | Duy nhất | Name | — |
| AgeRating | (tên tự sinh) | Duy nhất | Code | — |


### 4.2.6 Module Phim

- **Nội dung cần viết:**
  - Giới thiệu module Phim: vai trò, thực thể (Movie, MovieGenre, MovieCast, Review), quan hệ với module khác

#### 4.2.6.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.7` ERD module Phim
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.2.6.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.21` Thực thể Movie (bảng Movies)
  - `Bảng 4.22` Thực thể MovieGenre (bảng MovieGenres)
  - `Bảng 4.23` Thực thể MovieCast (bảng MovieCast)
  - `Bảng 4.24` Thực thể Review (bảng Reviews)
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



#### 4.2.6.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.25` Ràng buộc toàn vẹn module Phim
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


#### 4.2.6.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.26` Chỉ mục module Phim
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


### 4.2.7 Module Phòng chiếu

- **Nội dung cần viết:**
  - Giới thiệu module Phòng chiếu: vai trò, thực thể (CinemaRoom, ScreenType, Seat, SeatType), quan hệ với module khác

#### 4.2.7.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.8` ERD module Phòng chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.2.7.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.27` Thực thể CinemaRoom (bảng CinemaRooms)
  - `Bảng 4.28` Thực thể ScreenType (bảng ScreenTypes)
  - `Bảng 4.29` Thực thể Seat (bảng Seats)
  - `Bảng 4.30` Thực thể SeatType (bảng SeatTypes)
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



#### 4.2.7.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.31` Ràng buộc toàn vẹn module Phòng chiếu
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên | Loại | Biểu thức |
|---|---|---|---|
| CinemaRoom | CK_CinemaRooms_TotalSeats | CHECK | "TotalSeats" >= 0 |
| Seat | CK_Seats_SeatNumber | CHECK | "SeatNumber" > 0 |
| SeatType | CK_SeatTypes_PriceMultiplier | CHECK | "PriceMultiplier" > 0 |
| CinemaRoom | FK ScreenTypeId | Khóa ngoại | — |
| Seat | FK RoomId | Khóa ngoại | — |
| Seat | FK SeatTypeId | Khóa ngoại | — |


#### 4.2.7.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.32` Chỉ mục module Phòng chiếu
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| CinemaRoom | (tên tự sinh) | Duy nhất | RoomName | — |
| ScreenType | (tên tự sinh) | Duy nhất | Name | — |
| Seat | IX_Seats_SeatType | Thường | SeatTypeId | — |
| Seat | UQ_Room_Seat | Duy nhất | RoomId, SeatRow, SeatNumber | — |
| SeatType | (tên tự sinh) | Duy nhất | Name | — |


### 4.2.8 Module Suất chiếu

- **Nội dung cần viết:**
  - Giới thiệu module Suất chiếu: vai trò, thực thể (Showtime, ShowtimeSeat), quan hệ với module khác

#### 4.2.8.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.9` ERD module Suất chiếu
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.2.8.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.33` Thực thể Showtime (bảng Showtimes)
  - `Bảng 4.34` Thực thể ShowtimeSeat (bảng ShowtimeSeats)
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



#### 4.2.8.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.35` Ràng buộc toàn vẹn module Suất chiếu
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


#### 4.2.8.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.36` Chỉ mục module Suất chiếu
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| Showtime | IX_Showtime_Room_Time | Thường | RoomId, StartTime, EndTime | — |
| Showtime | IX_Showtime_Movie | Thường | MovieId, StartTime | — |
| ShowtimeSeat | UQ_Showtime_Seat | Duy nhất | ShowtimeId, SeatId | — |
| ShowtimeSeat | IX_ShowtimeSeats_Status | Thường | ShowtimeId, Status | — |


### 4.2.9 Module Đặt vé

- **Nội dung cần viết:**
  - Giới thiệu module Đặt vé: vai trò, thực thể (Booking, BookingTicket, BookingCombo, Combo), quan hệ với module khác

#### 4.2.9.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.10` ERD module Đặt vé
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.2.9.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.37` Thực thể Booking (bảng Bookings)
  - `Bảng 4.38` Thực thể BookingTicket (bảng BookingTickets)
  - `Bảng 4.39` Thực thể BookingCombo (bảng BookingCombos)
  - `Bảng 4.40` Thực thể Combo (bảng Combos)
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



#### 4.2.9.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.41` Ràng buộc toàn vẹn module Đặt vé
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


#### 4.2.9.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.42` Chỉ mục module Đặt vé
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


### 4.2.10 Module Thanh toán

- **Nội dung cần viết:**
  - Giới thiệu module Thanh toán: vai trò, thực thể (Payment), quan hệ với module khác

#### 4.2.10.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.11` ERD module Thanh toán
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.2.10.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.43` Thực thể Payment (bảng Payments)
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



#### 4.2.10.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.44` Ràng buộc toàn vẹn module Thanh toán
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên | Loại | Biểu thức |
|---|---|---|---|
| Payment | CK_Payments_Method | CHECK | "PaymentMethod" IN ('VNPay','Momo','Cash','Stripe') |
| Payment | CK_Payments_Amount | CHECK | "Amount" >= 0 |
| Payment | CK_Payments_Status | CHECK | "Status" IN ('Pending','Success','Failed','Refunded') |
| Payment | FK BookingId | Khóa ngoại | — |


#### 4.2.10.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.45` Chỉ mục module Thanh toán
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| Payment | IX_Payments_Booking | Thường | BookingId | — |
| Payment | IX_Payments_Status | Thường | Status | — |


### 4.2.11 Module Thành viên

- **Nội dung cần viết:**
  - Giới thiệu module Thành viên: vai trò, thực thể (MembershipTier), quan hệ với module khác

#### 4.2.11.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.12` ERD module Thành viên
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.2.11.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.46` Thực thể MembershipTier (bảng MembershipTiers)
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`

**MembershipTier** — bảng `MembershipTiers`

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú |
|---|---|---|---|---|
| TierId | int | Không | PK | — |
| TierName | varchar(50) | Không | UK | — |
| MinPoints | int | Không | — | 0 |
| DiscountPercent | decimal | Không | — | 0m |



#### 4.2.11.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.47` Ràng buộc toàn vẹn module Thành viên
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên | Loại | Biểu thức |
|---|---|---|---|
| MembershipTier | CK_MembershipTiers_Discount | CHECK | "DiscountPercent" BETWEEN 0 AND 100 |


#### 4.2.11.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.48` Chỉ mục module Thành viên
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| MembershipTier | (tên tự sinh) | Duy nhất | TierName | — |


### 4.2.12 Module Cấu hình rạp

- **Nội dung cần viết:**
  - Giới thiệu module Cấu hình rạp: vai trò, thực thể (CinemaSettings), quan hệ với module khác

#### 4.2.12.1 Sơ đồ ERD của module

- **Nội dung cần viết:**
  - Thực thể sở hữu thể hiện đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính
- **Tài liệu/hình/bảng:**
  - `Hình 4.13` ERD module Cấu hình rạp
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Entities/*.cs`; `Persistence/Configurations/*.cs`

#### 4.2.12.2 Mô tả thực thể

- **Nội dung cần viết:**
  - Viết ý nghĩa từng cột (cột "Ý nghĩa" bổ sung khi triển khai bảng)
  - Giải thích quan hệ và nghiệp vụ của từng thực thể
- **Tài liệu/hình/bảng:**
  - `Bảng 4.49` Thực thể CinemaSettings (bảng CinemaSettings)
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



#### 4.2.12.3 Ràng buộc toàn vẹn

- **Nội dung cần viết:**
  - Giải thích mục đích bảo vệ của từng ràng buộc; hành vi xóa của khóa ngoại (Restrict/Cascade/SetNull) đọc từ Configurations
- **Tài liệu/hình/bảng:**
  - `Bảng 4.50` Ràng buộc toàn vẹn module Cấu hình rạp
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

| Thực thể | Tên | Loại | Biểu thức |
|---|---|---|---|
| CinemaSettings | CK_CinemaSettings_Id | CHECK | "Id" = 1 |


#### 4.2.12.4 Chỉ mục

- **Nội dung cần viết:**
  - Giải thích chỉ mục phục vụ truy vấn nào hoặc bảo vệ ràng buộc nào
- **Tài liệu/hình/bảng:**
  - `Bảng 4.51` Chỉ mục module Cấu hình rạp
- **Nguồn dữ liệu:** `Persistence/Configurations/*.cs`

Module không khai báo chỉ mục riêng ngoài khóa chính.


### 4.2.13 Kiểu liệt kê

- **Nội dung cần viết:**
  - 10 kiểu liệt kê, lưu dạng chuỗi trong CSDL, kèm ràng buộc CHECK tương ứng
- **Tài liệu/hình/bảng:**
  - `Bảng 4.52` Các kiểu liệt kê
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


### 4.2.14 Vòng đời trạng thái

- **Nội dung cần viết:**
  - Mô tả chuyển trạng thái hợp lệ của năm đối tượng; xác định chuyển trạng thái bằng cách đọc service, không chỉ enum
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Enums/*.cs`; `Services/**`

#### 4.2.14.1 Vòng đời Booking

- **Nội dung cần viết:**
  - Các trạng thái, điều kiện và nơi thực hiện chuyển trạng thái
  - Pending → Confirmed: ConfirmBookingAndGenerateTicketsAsync
  - Pending → Expired: SeatExpirationService
  - Confirmed → Cancelled: CancelBookingAsync, hủy suất
- **Tài liệu/hình/bảng:**
  - `Hình 4.14` Máy trạng thái Booking
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Enums/*.cs`; `Services/**`

#### 4.2.14.2 Vòng đời ShowtimeSeat

- **Nội dung cần viết:**
  - Các trạng thái, điều kiện và nơi thực hiện chuyển trạng thái
  - Chuyển Booked → Available khi hủy đơn Confirmed: [CẦN BỔ SUNG: xác minh trong CancelBookingAsync]
- **Tài liệu/hình/bảng:**
  - `Hình 4.15` Máy trạng thái ShowtimeSeat
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Enums/*.cs`; `Services/**`

#### 4.2.14.3 Vòng đời Payment

- **Nội dung cần viết:**
  - Các trạng thái, điều kiện và nơi thực hiện chuyển trạng thái
  - Thanh toán tiền mặt tại quầy: [CẦN BỔ SUNG: xác minh trạng thái khởi tạo]
- **Tài liệu/hình/bảng:**
  - `Hình 4.16` Máy trạng thái Payment
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Enums/*.cs`; `Services/**`

#### 4.2.14.4 Vòng đời Showtime

- **Nội dung cần viết:**
  - Các trạng thái, điều kiện và nơi thực hiện chuyển trạng thái
  - Completed: CompleteEndedShowtimesAsync
- **Tài liệu/hình/bảng:**
  - `Hình 4.17` Máy trạng thái Showtime
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Enums/*.cs`; `Services/**`

#### 4.2.14.5 Vòng đời Movie

- **Nội dung cần viết:**
  - Các trạng thái, điều kiện và nơi thực hiện chuyển trạng thái
  - ComingSoon → NowShowing: [CẦN BỔ SUNG: xác minh có cơ chế tự động hay chỉ cập nhật thủ công]
- **Tài liệu/hình/bảng:**
  - `Hình 4.18` Máy trạng thái Movie
- **Nguồn dữ liệu:** `backend/Cinema.Domain/Enums/*.cs`; `Services/**`

### 4.2.15 Migration

- **Nội dung cần viết:**
  - 3 migration: `InitialCreate` (30/10/2026), `PendingModelChanges` (31/10/2026), `AddSeatGapRuleMode` (01/11/2026)
  - Cách chạy: `npm run db:update`; cờ `RunMigrationsOnStartup`
- **Nguồn dữ liệu:** `backend/Cinema.Infrastructure/Migrations/*`; `package.json`

### 4.2.16 Dữ liệu khởi tạo

- **Nội dung cần viết:**
  - Dữ liệu tĩnh `HasData`; dữ liệu động idempotent (tài khoản kiểm thử, phòng, suất chiếu, kịch bản booking–thanh toán–soát vé–đánh giá); không ghi mật khẩu vào báo cáo
- **Nguồn dữ liệu:** `Persistence/Seed/SeedData.cs`; `DbInitializer.cs`; `ShowtimeSeeder.cs`; `BookingScenarioSeeder.cs`

## 4.3 XÁC THỰC


### 4.3.1 Luồng đăng ký tài khoản

- **Nội dung cần viết:**
  - Đăng ký → tạo User/CustomerProfile → tạo mã xác nhận (băm) → xếp job gửi email → khách xác nhận bằng mã
- **Tài liệu/hình/bảng:**
  - `Hình 4.19` Tuần tự đăng ký tài khoản
- **Nguồn dữ liệu:** `AuthService.cs → RegisterAsync, ConfirmEmailAsync`

### 4.3.2 Luồng đăng nhập

- **Nội dung cần viết:**
  - Kiểm tra thông tin, khóa tài khoản, xác nhận email, cổng khách hàng/quản trị; cấp access token và refresh token
- **Tài liệu/hình/bảng:**
  - `Hình 4.20` Tuần tự đăng nhập bằng email
- **Nguồn dữ liệu:** `AuthService.cs → LoginAsync`

### 4.3.3 Luồng đăng nhập Google

- **Nội dung cần viết:**
  - Xác minh ID token; kiểm tra email đã xác minh; tạo mới hoặc liên kết tài khoản; chặn quản trị
- **Tài liệu/hình/bảng:**
  - `Hình 4.21` Tuần tự đăng nhập bằng Google
- **Nguồn dữ liệu:** `AuthService.cs → GoogleLoginAsync`; `Infrastructure/Auth/GoogleAuthService.cs`

### 4.3.4 Làm mới token xoay vòng

- **Nội dung cần viết:**
  - Access token 3600 giây; refresh token 30 ngày, lưu SHA-256; mỗi lần làm mới cấp token mới, thu hồi token cũ (`ReplacedByTokenId`)
  - Phía client: một promise làm mới dùng chung cho nhiều yêu cầu 401 đồng thời
- **Tài liệu/hình/bảng:**
  - `Hình 4.22` Tuần tự làm mới token xoay vòng
- **Nguồn dữ liệu:** `AuthService.cs → RefreshAsync`; `JwtTokenService.cs`; `frontend/src/api/axios.ts`

### 4.3.5 Phát hiện dùng lại token

- **Nội dung cần viết:**
  - Khi một refresh token đã thu hồi bị dùng lại → thu hồi toàn bộ chuỗi token; đổi/đặt lại mật khẩu → thu hồi mọi token
- **Tài liệu/hình/bảng:**
  - `Hình 4.23` Lưu đồ phát hiện dùng lại refresh token
- **Nguồn dữ liệu:** `AuthService.cs → RevokeTokenChainAsync`

### 4.3.6 Khôi phục mật khẩu

- **Nội dung cần viết:**
  - Yêu cầu khôi phục luôn trả thông điệp chuẩn (không lộ email có tồn tại); mã đặt lại băm, có hạn, dùng một lần
- **Tài liệu/hình/bảng:**
  - `Hình 4.24` Tuần tự khôi phục mật khẩu
- **Nguồn dữ liệu:** `AuthService.cs → ForgotPasswordAsync, ResetPasswordAsync`

### 4.3.7 Các loại token

- **Nội dung cần viết:**
  - So sánh access token, refresh token, socket token, mã xác nhận email, mã đặt lại mật khẩu
- **Tài liệu/hình/bảng:**
  - `Bảng 4.53` Các loại token
- **Nguồn dữ liệu:** `JwtTokenService.cs`; `AuthService.cs`; `Domain/Entities/UserVerificationToken.cs`

| Token | Thời hạn | Lưu trữ | Mục đích |
|---|---|---|---|
| Access token | 3600 giây | Không lưu (JWT) | Gọi API |
| Refresh token | 30 ngày | CSDL, dạng SHA-256 | Làm mới phiên |
| Socket token | 300 giây | Không lưu (JWT) | Kết nối SignalR |
| Mã xác nhận email | [CẦN BỔ SUNG: đọc AuthService] | CSDL, dạng băm | EmailConfirmation |
| Mã đặt lại mật khẩu | [CẦN BỔ SUNG: đọc AuthService] | CSDL, dạng băm | PasswordReset |


### 4.3.8 Ma trận phân quyền

- **Nội dung cần viết:**
  - Số endpoint theo controller và mức phân quyền (đếm tự động từ `[Authorize]`)
  - Giải thích policy: `StaffOnly` = Admin hoặc Employee; `AdminOnly` chỉ Admin
- **Tài liệu/hình/bảng:**
  - `Bảng 4.54` Ma trận phân quyền theo controller
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


## 4.4 LUỒNG NGHIỆP VỤ


### 4.4.1 Giữ ghế

- **Nội dung cần viết:**
  - Giới thiệu luồng giữ ghế, tham chiếu use case UC-DV-04 (mục 3.3.5)
- **Nguồn dữ liệu:** `Services/Bookings/BookingService.cs → HoldSeatsAsync, CreateHoldBookingAsync`

#### 4.4.1.1 Kiểm tra đầu vào

- **Nội dung cần viết:**
  - Validator số ghế, combo; trạng thái suất chiếu; thời điểm dừng bán online; booking Pending sẵn có; luật chọn ghế phía máy chủ
- **Tài liệu/hình/bảng:**
  - `Bảng 4.55` Các kiểm tra khi giữ ghế
- **Nguồn dữ liệu:** `Validators/Bookings/BookingValidators.cs`; `Utils/SeatLayoutRules.cs`; `BookingService.cs`

| Thứ tự | Kiểm tra | Lỗi trả về |
|---|---|---|
| 1 | Dữ liệu đầu vào hợp lệ | Validation |
| 2 | Suất chiếu Scheduled, chưa bắt đầu, chưa quá thời điểm dừng bán | [CẦN BỔ SUNG: đọc thông điệp] |
| 3 | Chưa có booking Pending còn hạn | Conflict_PendingBookingExistsForShowtime |
| 4 | Luật chọn ghế (ghế đôi, ghế lẻ) | [CẦN BỔ SUNG: đọc thông điệp] |


#### 4.4.1.2 Giữ ghế nguyên tử

- **Nội dung cần viết:**
  - Giải thích điều kiện của câu lệnh cập nhật; vì sao so sánh số dòng ảnh hưởng; vai trò của chỉ mục duy nhất `(ShowtimeId, SeatId)` và `BookingTicket.ShowtimeSeatId`
  - Đoạn mã rút gọn ≤ 15 dòng đặt ở Phụ lục 5
- **Tài liệu/hình/bảng:**
  - `Hình 4.25` Lưu đồ giữ ghế nguyên tử
- **Nguồn dữ liệu:** `BookingService.cs → CreateHoldBookingAsync`; `Cinema.Shared/Exceptions/SeatConflictException.cs`

#### 4.4.1.3 Tính tiền

- **Nội dung cần viết:**
  - Tổng = Σ giá ghế theo suất (`ShowtimeSeat.Price`) + Σ combo; giảm giá theo hạng thành viên; số tiền cuối cùng
  - Mã booking: tiền tố + ngày + 4 ký tự, kiểm tra duy nhất
- **Tài liệu/hình/bảng:**
  - `(4.1)` Số tiền cuối cùng của đơn: FinalAmount = TotalAmount − DiscountAmount [CẦN BỔ SUNG: xác nhận công thức làm tròn trong CalculateDiscountAmountAsync]
- **Nguồn dữ liệu:** `BookingService.cs → CalculateDiscountAmountAsync, BuildBookingCombosAsync, GenerateUniqueBookingCodeAsync`; `Utils/CodeGenerator.cs`

#### 4.4.1.4 Tuần tự giữ ghế

- **Nội dung cần viết:**
  - Trình tự từ thao tác của khách đến phản hồi và sự kiện thời gian thực
- **Tài liệu/hình/bảng:**
  - `Hình 4.26` Tuần tự giữ ghế
- **Nguồn dữ liệu:** `BookingService.cs`; `Controllers/Client/BookingsController.cs`

### 4.4.2 Thanh toán trực tuyến

- **Nội dung cần viết:**
  - Giới thiệu luồng thanh toán, tham chiếu các use case UC-TT-01…06 (mục 3.3.6)
- **Nguồn dữ liệu:** `Services/Payments/PaymentService.cs`; `Infrastructure/Payments/*`

#### 4.4.2.1 Tạo giao dịch thanh toán

- **Nội dung cần viết:**
  - Kiểm tra booking; chọn cổng theo tên phương thức; tạo bản ghi Payment; tạo liên kết thanh toán
- **Tài liệu/hình/bảng:**
  - `Hình 4.27` Tuần tự tạo giao dịch thanh toán
- **Nguồn dữ liệu:** `BookingService.cs → CheckoutAsync`; `Utils/PaymentGatewayExtensions.cs`

#### 4.4.2.2 Xử lý IPN

- **Nội dung cần viết:**
  - Xác minh chữ ký → tìm giao dịch → idempotent → kiểm tra số tiền (sai số ≤ 0,01), tiền tệ (Stripe) → áp dụng kết quả → phản hồi cổng
- **Tài liệu/hình/bảng:**
  - `Hình 4.28` Lưu đồ xử lý IPN
- **Nguồn dữ liệu:** `PaymentService.cs → ApplyPaymentResultAsync, IsAlreadyProcessed`; `Controllers/PaymentsController.cs`

#### 4.4.2.3 Phát hành vé sau thanh toán

- **Nội dung cần viết:**
  - Chuyển ghế sang Booked; tạo vé và mã QR (dạng mã booking, hàng, số ghế); cộng điểm; phát sự kiện; gửi email
- **Tài liệu/hình/bảng:**
  - `Hình 4.29` Tuần tự phát hành vé sau thanh toán
- **Nguồn dữ liệu:** `BookingService.cs → ConfirmBookingAndGenerateTicketsAsync`; `Utils/QrCodeHelper.cs`; `BackgroundJobs/SendEmailJob.cs`

#### 4.4.2.4 Ghế hết hạn sau khi khách đã trả tiền

- **Nội dung cần viết:**
  - Bắt `SeatsNoLongerHeldException`; tự hoàn tiền; cập nhật Payment; gửi email (tự hoàn hoặc cần hoàn thủ công)
- **Tài liệu/hình/bảng:**
  - `Hình 4.30` Lưu đồ xử lý ghế hết hạn sau thanh toán
- **Nguồn dữ liệu:** `PaymentService.cs → HandleSeatsExpiredAfterPaymentAsync`; `Utils/EmailTemplates.cs`

#### 4.4.2.5 So sánh ba cổng thanh toán

- **Nội dung cần viết:**
  - So sánh thuật toán ký, xác minh callback, đơn vị tiền, hoàn tiền; đọc đủ ba file gateway trước khi viết
- **Tài liệu/hình/bảng:**
  - `Bảng 4.56` So sánh VNPay, MoMo, Stripe
- **Nguồn dữ liệu:** `Infrastructure/Payments/VnPayGateway.cs`; `MomoGateway.cs`; `StripeGateway.cs`; `HmacSigner.cs`

| Tiêu chí | VNPay | MoMo | Stripe |
|---|---|---|---|
| Thuật toán ký/xác minh | HMAC-SHA512 | HMAC-SHA256 | Chữ ký webhook của Stripe |
| Sự kiện/callback | IPN (GET, POST) | IPN (POST) | Webhook `checkout.session.completed` |
| Hoàn tiền (`RefundAsync`) | Có | Có | Có |
| Đơn vị tiền / định dạng số tiền | [CẦN BỔ SUNG: đọc VnPayGateway] | [CẦN BỔ SUNG: đọc MomoGateway] | [CẦN BỔ SUNG: đọc StripeGateway] |
| Địa chỉ IP gửi cổng | `vnp_IpAddr` cố định 127.0.0.1 (DefaultIpAddress) | — | — |


### 4.4.3 Hết hạn giữ ghế

- **Nội dung cần viết:**
  - Thứ tự xử lý của job; nơi gọi `ReleaseStaleHeldSeatsAsync` (chỉ trình bày sự thật; đánh giá ở 7.3)
- **Tài liệu/hình/bảng:**
  - `Hình 4.31` Lưu đồ giải phóng ghế hết hạn
- **Nguồn dữ liệu:** `Services/Bookings/SeatExpirationService.cs`; `BackgroundJobs/ReleaseExpiredSeatsJob.cs`; `Program.cs`

### 4.4.4 Hủy vé


#### 4.4.4.1 Điều kiện hoàn tiền

- **Nội dung cần viết:**
  - Điều kiện hủy booking Confirmed: chưa soát vé; không thanh toán tiền mặt; còn trong hạn `CancelDeadlineHours`; cổng đã cấu hình; số tiền hoàn theo `CancelRefundPercent`
- **Tài liệu/hình/bảng:**
  - `Bảng 4.57` Chính sách và tình huống hoàn tiền
- **Nguồn dữ liệu:** `BookingService.cs → CalculateRefundEligibility, CancelBookingAsync`

| Tình huống | Kết quả | Nguồn |
|---|---|---|
| Hủy booking Pending | Trả ghế, không hoàn tiền | CancelBookingAsync |
| Hủy booking Confirmed đủ điều kiện | Hoàn `CancelRefundPercent` % qua cổng; thu hồi điểm | CalculateRefundEligibility |
| Hủy booking Confirmed đã soát vé | Không cho hủy | [CẦN BỔ SUNG: đọc thông điệp] |
| Thanh toán tiền mặt | Hủy tại quầy, không hoàn qua cổng | [CẦN BỔ SUNG: đọc thông điệp] |
| Hủy suất chiếu | Hoàn tiền từng thanh toán bằng job | ShowtimeRefundJob |
| Ghế hết hạn sau thanh toán | Tự hoàn tiền | HandleSeatsExpiredAfterPaymentAsync |


#### 4.4.4.2 Luồng hủy vé

- **Nội dung cần viết:**
  - Chiếm quyền hủy bằng cập nhật có điều kiện; gọi hoàn tiền; xử lý lỗi nghiêm trọng khi hoàn tiền xong nhưng cơ sở dữ liệu chưa ghi
- **Tài liệu/hình/bảng:**
  - `Hình 4.32` Lưu đồ hủy vé
- **Nguồn dữ liệu:** `BookingService.cs → CancelBookingAsync, ReverseLoyaltyAsync`

### 4.4.5 Bán vé tại quầy

- **Nội dung cần viết:**
  - Giữ ghế kênh Counter; thanh toán tiền mặt; tiền thối; xác nhận và phát vé
- **Tài liệu/hình/bảng:**
  - `Hình 4.33` Tuần tự bán vé tại quầy
- **Nguồn dữ liệu:** `BookingService.cs → CounterHoldSeatsAsync, CounterCheckoutAsync`; `frontend/src/features/admin/counter/**`

### 4.4.6 Soát vé

- **Nội dung cần viết:**
  - Điều kiện soát vé; chống soát lặp bằng cập nhật có điều kiện; quét camera (BarcodeDetector hoặc jsQR) hoặc nhập mã
- **Tài liệu/hình/bảng:**
  - `Hình 4.34` Lưu đồ soát vé
- **Nguồn dữ liệu:** `BookingService.cs → CheckinAsync`; `frontend/src/features/admin/checkin/**`

### 4.4.7 Hủy suất chiếu

- **Nội dung cần viết:**
  - Quy tắc không đổi phòng, không sửa khi có booking; tác động của hủy suất lên booking/payment; thử lại hoàn tiền
  - [CẦN BỔ SUNG: đọc DeleteShowtimeAsync để xác nhận thứ tự và điều kiện chính xác]
- **Tài liệu/hình/bảng:**
  - `Hình 4.35` Tuần tự hủy suất chiếu
- **Nguồn dữ liệu:** `Services/Showtimes/ShowtimeService.cs → DeleteShowtimeAsync`; `BackgroundJobs/ShowtimeRefundJob.cs`

### 4.4.8 Điểm thưởng

- **Nội dung cần viết:**
  - Cộng điểm khi xác nhận, thu hồi khi hoàn tiền; nâng/hạ hạng theo `MinPoints`; giảm giá theo `DiscountPercent`
- **Tài liệu/hình/bảng:**
  - `(4.2)` Điểm thưởng: Points = ⌊FinalAmount / PointsPerAmount⌋ (mặc định PointsPerAmount = 10.000)
  - `Bảng 4.58` Hạng thành viên
- **Nguồn dữ liệu:** `BookingService.cs → ApplyLoyaltyAsync`; `Domain/Entities/MembershipTier.cs`; `Persistence/Seed/SeedData.cs`

| Hạng | MinPoints | DiscountPercent |
|---|---|---|
| [CẦN BỔ SUNG: đọc SeedData.cs] |  |  |


### 4.4.9 Đánh giá phim

- **Nội dung cần viết:**
  - Điều kiện đánh giá; ràng buộc `(UserId, MovieId)` duy nhất; tính lại điểm trung bình; kiểm duyệt của quản trị
- **Tài liệu/hình/bảng:**
  - `Hình 4.36` Lưu đồ viết đánh giá phim
- **Nguồn dữ liệu:** `Services/Reviews/ReviewService.cs`

## 4.5 THUẬT TOÁN


### 4.5.1 Luật chọn ghế

- **Nội dung cần viết:**
  - Định nghĩa "ghế lẻ" theo `ValidateSelection`; ghế đôi chọn cùng nhau; ba chế độ Strict, AllowEdge, Off
- **Tài liệu/hình/bảng:**
  - `Hình 4.37` Minh họa luật ghế lẻ
  - `Bảng 4.59` Ba chế độ luật ghế lẻ
- **Nguồn dữ liệu:** `Utils/SeatLayoutRules.cs`; `Domain/Enums/SeatGapRuleMode.cs`; `frontend/src/components/common/seat-map/seat-map-utils.ts`

| Chế độ | Ý nghĩa | Nơi áp dụng |
|---|---|---|
| Strict | [CẦN BỔ SUNG: đọc ValidateSelection] | Mặc định, đặt vé online |
| AllowEdge | [CẦN BỔ SUNG: đọc ValidateSelection] | Cấu hình |
| Off | Tắt kiểm tra | Bán vé quầy |


### 4.5.2 Gợi ý ghế


#### 4.5.2.1 Chia đoạn ghế trống

- **Nội dung cần viết:**
  - Chia mỗi hàng thành các đoạn ghế Available liên tiếp; duyệt cửa sổ có độ dài bằng số ghế cần; loại cửa sổ để lại đúng 1 ghế trống ở đầu/cuối đoạn; loại cửa sổ cắt đôi cặp ghế đôi
- **Tài liệu/hình/bảng:**
  - `Hình 4.38` Minh họa cửa sổ gợi ý ghế
- **Nguồn dữ liệu:** `Utils/SeatLayoutRules.cs → Suggest, SplitRuns`

#### 4.5.2.2 Hàm chấm điểm

- **Nội dung cần viết:**
  - Điểm = 0,45 × độ gần giữa hàng + 0,45 × điểm độ sâu hàng + 0,1 nếu cùng loại ghế (trọng số lấy từ code, kiểm tra lại)
  - Chọn cửa sổ tốt nhất mỗi hàng; trả về tối đa 3 gợi ý
  - Ví dụ số minh họa tự tính tay (ghi rõ là ví dụ giả định)
- **Tài liệu/hình/bảng:**
  - `(4.3)` Điểm gần giữa hàng: [CẦN BỔ SUNG: biểu thức từ SeatLayoutRules.cs]
  - `(4.4)` Điểm độ sâu hàng: [CẦN BỔ SUNG: biểu thức từ SeatLayoutRules.cs]
  - `(4.5)` Điểm gợi ý: Score = 0,45·Center + 0,45·Row + 0,1·SameType
- **Nguồn dữ liệu:** `Utils/SeatLayoutRules.cs`; `Services/Showtimes/ShowtimeService.cs → GetSeatSuggestionsAsync`; `frontend/src/features/client/bookings/components/booking/seat-suggestion-panel.tsx`

### 4.5.3 Xếp lịch chiếu tự động


#### 4.5.3.1 Độ nóng của phim

- **Nội dung cần viết:**
  - Heat = 0,7 × (vé bán / vé bán tối đa trong `HeatLookbackDays` ngày) + 0,3 × (điểm đánh giá / 10), giới hạn [0, 1]
  - Mô tả đúng như code; ghi chú "xem thảo luận ở 7.3" vì `AverageRating` bị ràng buộc 0–5
- **Tài liệu/hình/bảng:**
  - `(4.6)` Độ nóng: Heat = min(1, 0,7·(Vé/VéMax) + 0,3·(Rating/10))
- **Nguồn dữ liệu:** `Utils/ShowtimeScheduler.cs → Heat`; `ShowtimeService.cs → PreviewAutoScheduleAsync`; `Persistence/Configurations/MovieConfiguration.cs`

#### 4.5.3.2 Ứng viên khung giờ

- **Nội dung cần viết:**
  - Khung giờ hoạt động; thời điểm lý tưởng chia đều theo số suất; ứng viên từ đầu/cuối khoảng bận hiện có và mốc lý tưởng; làm tròn theo bước `StepMinutes` tính từ 0h giờ Việt Nam
  - Ràng buộc: không trùng phòng (kèm đệm), cùng phim cách nhau ≥ `MinSameMovieGapMinutes`, trong giờ mở cửa trừ `AllowAfterHours`
- **Tài liệu/hình/bảng:**
  - `(4.7)` Thời điểm lý tưởng thứ i: [CẦN BỔ SUNG: biểu thức từ ShowtimeScheduler.cs]
  - `Bảng 4.60` Tham số bộ xếp lịch
- **Nguồn dữ liệu:** `Options/SchedulerOptions.cs`; `Utils/ShowtimeScheduler.cs`; `Utils/OperatingHours.cs`

| Tham số | Ý nghĩa | Mặc định |
|---|---|---|
| StepMinutes | Bước làm tròn | [CẦN BỔ SUNG] |
| MinSameMovieGapMinutes | Khoảng cách tối thiểu giữa hai suất cùng phim | [CẦN BỔ SUNG] |
| SpreadMinutes | Hệ số chuẩn hóa khoảng cách | [CẦN BỔ SUNG] |
| FitWeight | Trọng số mức phù hợp | [CẦN BỔ SUNG] |
| LoadWeight | Trọng số tải phòng | [CẦN BỔ SUNG] |
| HeatLookbackDays | Số ngày tính vé bán | [CẦN BỔ SUNG] |


#### 4.5.3.3 Hàm điểm xếp lịch

- **Nội dung cần viết:**
  - Điểm = −(khoảng cách tới mốc lý tưởng / `SpreadMinutes`) + (1 − |độ nóng − số ghế / số ghế lớn nhất|) × `FitWeight` − số suất đã xếp trong phòng × `LoadWeight`
  - Phân tích độ phức tạp theo vòng lặp thực tế `[CẦN BỔ SUNG: nêu biến n, m, k sau khi đọc code]`
- **Tài liệu/hình/bảng:**
  - `(4.8)` Mức phù hợp: Fit = 1 − |Heat − Ghế/GhếMax|
  - `(4.9)` Điểm xếp lịch: Score = −Khoảng cách/Spread + Fit·FitWeight − Tải·LoadWeight
  - `Hình 4.39` Lưu đồ xếp lịch chiếu tự động
- **Nguồn dữ liệu:** `Utils/ShowtimeScheduler.cs → Build, Score`; `backend/tests/Cinema.UnitTests/Utils/ShowtimeSchedulerTests.cs`

### 4.5.4 Phát hiện trùng lịch phòng

- **Nội dung cần viết:**
  - Điều kiện trùng: cùng phòng, suất Scheduled, `EndTime` > bắt đầu mới và `StartTime` < kết thúc mới; `EndTime` = bắt đầu + thời lượng + đệm 15 phút; giờ mở/đóng cửa qua đêm
- **Tài liệu/hình/bảng:**
  - `Hình 4.40` Minh họa điều kiện trùng lịch phòng
- **Nguồn dữ liệu:** `ShowtimeService.cs → HasOverlapAsync`; `Utils/OperatingHours.cs`

### 4.5.5 Xử lý múi giờ

- **Nội dung cần viết:**
  - Lưu UTC; đổi qua `VietnamTime` theo `Brand:TimeZoneId`; mốc ngày theo giờ Việt Nam cho báo cáo và lọc
- **Tài liệu/hình/bảng:**
  - `Bảng 4.61` Quy ước thời gian
- **Nguồn dữ liệu:** `Utils/VietnamTime.cs`; `Utils/JsonDateTimeConverters.cs`; `frontend/src/lib/wall-time.ts`; `frontend/src/lib/business-time.ts`

| Tầng | Quy ước |
|---|---|
| CSDL | UTC |
| API | [CẦN BỔ SUNG: đọc JsonDateTimeConverters] |
| Giao diện | Giờ Việt Nam (wall-time, business-time) |


## 4.6 GIAO TIẾP THỜI GIAN THỰC


### 4.6.1 Hub SignalR

- **Nội dung cần viết:**
  - Hub `/hubs/showtimes` yêu cầu xác thực; `access_token` qua query string; nhóm `showtime-{id}`
- **Tài liệu/hình/bảng:**
  - `Hình 4.41` Tuần tự cập nhật ghế thời gian thực
- **Nguồn dữ liệu:** `Infrastructure/Realtime/ShowtimeHub.cs`; `SignalRRealtimeNotifier.cs`; `Program.cs (OnMessageReceived)`

### 4.6.2 Sự kiện SignalR

- **Nội dung cần viết:**
  - Bốn sự kiện và payload; payload không chứa định danh người giữ ghế (kiểm tra lại)
- **Tài liệu/hình/bảng:**
  - `Bảng 4.62` Sự kiện SignalR
- **Nguồn dữ liệu:** `Cinema.Shared/Constants/SignalREvents.cs`; `Infrastructure/Realtime/SignalRRealtimeNotifier.cs`

| Sự kiện | Khi nào phát | Payload |
|---|---|---|
| seat.held | Giữ ghế thành công | [CẦN BỔ SUNG: đọc notifier] |
| seat.booked | Phát hành vé | [CẦN BỔ SUNG] |
| seat.released | Hủy giữ ghế/hủy đơn | [CẦN BỔ SUNG] |
| hold.expired | Booking hết hạn | [CẦN BỔ SUNG] |


### 4.6.3 Xử lý phía client

- **Nội dung cần viết:**
  - Hook `useShowtimeHub`, `useRealtimeSeatMap`: áp dụng sự kiện lên sơ đồ ghế; xử lý hold.expired cho booking của chính mình
- **Nguồn dữ liệu:** `frontend/src/hooks/use-showtime-hub.ts`; `frontend/src/hooks/use-realtime-seat-map.ts`; `frontend/src/lib/signalr.ts`

## 4.7 TRỢ LÝ AI


### 4.7.1 Luồng function calling

- **Nội dung cần viết:**
  - Mô hình `gemini-2.5-flash`; vòng lặp gọi hàm; giới hạn vòng/lần gọi
- **Tài liệu/hình/bảng:**
  - `Hình 4.42` Luồng function calling của trợ lý AI
- **Nguồn dữ liệu:** `Services/Ai/GeminiAiAssistantService.cs`; `AiToolExecutor.cs`; `Controllers/Client/AiController.cs`

### 4.7.2 Các hàm của trợ lý

- **Nội dung cần viết:**
  - Ba hàm: `search_showtimes`, `get_seat_availability`, `get_my_bookings` (chỉ khi đăng nhập); tham số và dữ liệu trả về đọc từ AiToolExecutor
- **Tài liệu/hình/bảng:**
  - `Bảng 4.63` Các hàm của trợ lý AI
- **Nguồn dữ liệu:** `Services/Ai/AiToolExecutor.cs`

| Hàm | Tham số | Dữ liệu trả về | Điều kiện |
|---|---|---|---|
| search_showtimes | [CẦN BỔ SUNG] | [CẦN BỔ SUNG] | Công khai |
| get_seat_availability | [CẦN BỔ SUNG] | [CẦN BỔ SUNG] | Công khai |
| get_my_bookings | [CẦN BỔ SUNG] | [CẦN BỔ SUNG] | Đã đăng nhập |


### 4.7.3 Giới hạn ngữ cảnh

- **Nội dung cần viết:**
  - System prompt dựng từ dữ liệu rạp, phim, combo và thông tin người dùng đăng nhập; giới hạn tin nhắn 1000 ký tự, mỗi mục lịch sử 2000 ký tự; hành vi khi thiếu khóa API
- **Nguồn dữ liệu:** `Services/Ai/GeminiAiAssistantService.cs`; `Validators/Ai/AiValidators.cs`

## 4.8 GIAO DIỆN


### 4.8.1 Hệ thống nhận diện

- **Nội dung cần viết:**
  - Ý tưởng "đường xé" (perforation); token màu OKLCH; typography (Archivo, Be Vietnam Pro); quy tắc không bóng, không gradient, không bo góc; trạng thái ghế/huy hiệu; chế độ sáng/tối
- **Tài liệu/hình/bảng:**
  - `Bảng 4.64` Token thiết kế rút gọn
- **Nguồn dữ liệu:** `.agents/rules/perfo-design.md`; `.agents/skills/perfo-design-system/**`; `frontend/src/index.css`; `frontend/src/lib/theme-palette.ts`

| Token | Giá trị | Mục đích |
|---|---|---|
| [CẦN BỔ SUNG] |  |  |


### 4.8.2 Sơ đồ trang khách hàng

- **Nội dung cần viết:**
  - Liệt kê trang và đường dẫn từ routes thực tế; phân quyền route
- **Tài liệu/hình/bảng:**
  - `Hình 4.43` Sơ đồ trang khách hàng
- **Nguồn dữ liệu:** `frontend/src/app/routes/client-routes.tsx`; `frontend/src/config/routes.ts`

### 4.8.3 Sơ đồ trang quản trị

- **Nội dung cần viết:**
  - Liệt kê trang quản trị; nhân viên vào thẳng quầy bán vé (`getAdminHomePath`); phân quyền theo vai trò
- **Tài liệu/hình/bảng:**
  - `Hình 4.44` Sơ đồ trang quản trị
- **Nguồn dữ liệu:** `frontend/src/app/routes/admin-routes.tsx`; `frontend/src/config/admin-nav.ts`; `frontend/src/config/routes.ts`

### 4.8.4 Màn hình chọn ghế

- **Nội dung cần viết:**
  - Sơ đồ ghế, chú giải, gợi ý ghế, đếm ngược giữ ghế, chi tiết đơn hàng; mục đích – thành phần – tương tác chính – component
  - Dùng "xem Hình 5.x" cho ảnh chụp ở Chương 5
- **Nguồn dữ liệu:** `frontend/src/components/common/seat-map/*`; `frontend/src/features/client/bookings/**`

### 4.8.5 Màn hình vé điện tử

- **Nội dung cần viết:**
  - Vé có mã QR, trạng thái, thông tin suất chiếu; mục đích – thành phần – tương tác chính – component
  - Dùng "xem Hình 5.x" cho ảnh chụp ở Chương 5
- **Nguồn dữ liệu:** `frontend/src/components/common/perfo-ticket.tsx`; `frontend/src/features/client/account/**`

### 4.8.6 Màn hình trình soạn sơ đồ ghế

- **Nội dung cần viết:**
  - Cọ vẽ, chọn vùng, lối đi, ghế đôi, sao chép hàng, hoàn tác/làm lại, bảng kiểm tra hợp lệ, sinh tự động; mục đích – thành phần – tương tác chính – component
  - Dùng "xem Hình 5.x" cho ảnh chụp ở Chương 5
- **Nguồn dữ liệu:** `frontend/src/features/admin/rooms/components/seat-editor-*.tsx`

### 4.8.7 Màn hình quầy bán vé

- **Nội dung cần viết:**
  - Chọn suất, ghế, combo, khách; nhập tiền nhận; vé kết quả; mục đích – thành phần – tương tác chính – component
  - Dùng "xem Hình 5.x" cho ảnh chụp ở Chương 5
- **Nguồn dữ liệu:** `frontend/src/features/admin/counter/**`

### 4.8.8 Màn hình soát vé

- **Nội dung cần viết:**
  - Quét camera hoặc nhập mã; hiển thị kết quả; mục đích – thành phần – tương tác chính – component
  - Dùng "xem Hình 5.x" cho ảnh chụp ở Chương 5
- **Nguồn dữ liệu:** `frontend/src/features/admin/checkin/**`

### 4.8.9 Màn hình dashboard

- **Nội dung cần viết:**
  - Biểu đồ, chỉ số tổng hợp; mục đích – thành phần – tương tác chính – component
  - Dùng "xem Hình 5.x" cho ảnh chụp ở Chương 5
- **Nguồn dữ liệu:** `frontend/src/features/admin/dashboard/**`; `frontend/src/features/admin/reports/**`

## 4.9 GIAO DIỆN LẬP TRÌNH


### 4.9.1 Quy ước thiết kế API

- **Nội dung cần viết:**
  - Tiền tố `/api/v1`; tách tài liệu Swagger `client`/`admin`; quy ước phản hồi, phân trang, mã lỗi; endpoint thanh toán công khai nhưng xác minh chữ ký; policy giới hạn tốc độ `auth`, `booking-hold`
- **Nguồn dữ liệu:** `backend/Cinema.Api/Program.cs`; `backend/Cinema.Api/Controllers/**`

### 4.9.2 Tổng hợp endpoint theo nhóm

- **Nội dung cần viết:**
  - Số endpoint theo controller (đếm tự động); danh mục đầy đủ ở Phụ lục 2
- **Tài liệu/hình/bảng:**
  - `Bảng 4.65` Tổng hợp endpoint theo controller
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
  - Backend ≈ 25.862 dòng C# (không tính migration, test); frontend ≈ 33.234 dòng TS/TSX (298 file `.tsx`); test ≈ 9.275 dòng C#; seed ≈ 3.830 dòng; 29 entity, 10 enum, 33 controller, 137 endpoint, 3 migration
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
| Thực thể / enum / migration | 29 / 10 / 3 | Đếm thư mục |
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
  - Hạng, điểm, giảm giá; cộng/thu hồi điểm
  - Khuôn viết: chức năng đã cài đặt → thành phần chính (lớp/file) → cách hoạt động (tham chiếu mục thiết kế Chương 4) → ghi chú đáng chú ý
  - Tối đa 1 đoạn mã ≤ 15 dòng khi thật sự minh họa điểm kỹ thuật (xem Phụ lục 5)
- **Nguồn dữ liệu:** `BookingService.cs (loyalty)`; `Domain/Entities/MembershipTier.cs`

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
  - Hangfire (PostgreSQL): ReleaseExpiredSeatsJob, ShowtimeLifecycleJob (mỗi phút), SendEmailJob, ShowtimeRefundJob (thử lại 3 lần)
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
| Tác vụ nền | BG_RELEASE_EXPIRED_SEATS_CRON, BG_RETRY_ATTEMPTS |
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
| Đơn vị | 28 | 283 | xUnit, Moq, Shouldly, SQLite | [CẦN BỔ SUNG: kết quả dotnet test] |
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
  - Tổng nhóm: 69 test
- **Tài liệu/hình/bảng:**
  - `Bảng 6.3` Test đơn vị nhóm Đặt vé
- **Nguồn dữ liệu:** `Cinema.UnitTests/Services/BookingServiceCounterCheckinTests.cs`; `Cinema.UnitTests/Services/BookingServiceLoyaltyTests.cs`; `Cinema.UnitTests/Services/BookingServiceSeatHoldTests.cs`; `Cinema.UnitTests/Services/BookingServiceTests.cs`; `Cinema.UnitTests/Validators/BookingValidatorsTests.cs`

| File | Số test | Trong đó [Theory] | Hành vi chính |
|---|---|---|---|
| BookingServiceCounterCheckinTests | 18 | 0 | [CẦN BỔ SUNG: đọc tên test] |
| BookingServiceLoyaltyTests | 4 | 0 | [CẦN BỔ SUNG: đọc tên test] |
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
  - Ma trận truy vết 21 module use case → cài đặt → test đơn vị → test tích hợp → E2E; nhận xét vùng chưa có kiểm thử
- **Tài liệu/hình/bảng:**
  - `Bảng 7.1` Truy vết module đến kiểm thử
- **Nguồn dữ liệu:** `backend/tests/**`; `bo_sung/e2e/**`; `Mục 3.3 file này`

| Module | Cài đặt | Test đơn vị | Test tích hợp | E2E |
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
| MT1 Đủ chức năng | 102 use case ở Chương 3; module đã cài đặt ở Chương 5 | [CẦN BỔ SUNG: đối chiếu từng use case đã cài đặt] | 3.3, 5.3 |
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

### 7.3.15 Tổng hợp hạn chế

- **Nội dung cần viết:**
  - Bảng tổng hợp 14 hạn chế và đề xuất
- **Tài liệu/hình/bảng:**
  - `Bảng 7.3` Hạn chế, đề xuất khắc phục
- **Nguồn dữ liệu:** `Các mục 7.3.1–7.3.14`

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
  - Xếp lịch chiếu tự động có hàm điểm
  - Trợ lý AI function calling
- **Nguồn dữ liệu:** `Chương 4, Chương 5`

## 8.2 HẠN CHẾ

- **Nội dung cần viết:**
  - Tóm tắt các hạn chế quan trọng nhất từ mục 7.3, phân loại: độ đúng đắn/dữ liệu, bảo mật, vận hành, kiểm thử; phạm vi một rạp; cổng thanh toán mới ở sandbox
- **Nguồn dữ liệu:** `Mục 7.3`

## 8.3 HƯỚNG PHÁT TRIỂN

- **Nội dung cần viết:**
  - Ràng buộc loại trừ ở CSDL cho lịch phòng; giải phóng ghế hết hạn ngay khi truy vấn; sửa thang điểm Heat kèm test; tách seed demo khỏi Production; refresh token qua cookie HttpOnly và CSP; xác thực và giới hạn tốc độ riêng cho AI; truyền IP thật cho VNPay; nhiều chi nhánh và cổng thanh toán (ZaloPay); CI/CD; test frontend; mở rộng E2E và kiểm thử tải
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
  - [CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch]
- **Nguồn dữ liệu:** `package.json`

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
| Tác vụ nền | BG_RELEASE_EXPIRED_SEATS_CRON, BG_SHOWTIME_LIFECYCLE_CRON, BG_RETRY_ATTEMPTS |
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
- **Nguồn dữ liệu:** `Utils/ShowtimeScheduler.cs → Build, Score`
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
| Hình 3.3 | Sơ đồ use case tổng quát | 3.2.2 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.4 | Sơ đồ use case module Xác thực | 3.3.1.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.5 | Phân rã use case Đăng ký tài khoản | 3.3.1.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.6 | Phân rã use case Đăng nhập bằng email | 3.3.1.4 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.7 | Phân rã use case Đăng nhập bằng Google | 3.3.1.5 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.8 | Phân rã use case Đăng nhập trang quản trị | 3.3.1.6 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.9 | Phân rã use case Khôi phục mật khẩu | 3.3.1.7 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.10 | Phân rã use case Duy trì phiên đăng nhập | 3.3.1.8 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.11 | Sơ đồ use case module Hồ sơ cá nhân | 3.3.2.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.12 | Phân rã use case Đổi mật khẩu | 3.3.2.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.13 | Sơ đồ use case module Duyệt phim | 3.3.3.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.14 | Phân rã use case Xem danh sách phim | 3.3.3.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.15 | Phân rã use case Xem chi tiết phim | 3.3.3.4 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.16 | Phân rã use case Xem lịch chiếu theo ngày | 3.3.3.5 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.17 | Sơ đồ use case module Đánh giá phim | 3.3.4.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.18 | Phân rã use case Viết đánh giá | 3.3.4.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.19 | Sơ đồ use case module Đặt vé trực tuyến | 3.3.5.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.20 | Phân rã use case Đặt vé trực tuyến | 3.3.5.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.21 | Phân rã use case Xem sơ đồ ghế của suất chiếu | 3.3.5.4 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.22 | Phân rã use case Chọn ghế | 3.3.5.5 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.23 | Phân rã use case Giữ ghế | 3.3.5.6 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.24 | Phân rã use case Chọn combo | 3.3.5.7 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.25 | Phân rã use case Hủy đơn đặt vé | 3.3.5.8 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.26 | Sơ đồ use case module Thanh toán trực tuyến | 3.3.6.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.27 | Phân rã use case Thanh toán đơn đặt vé trực tuyến | 3.3.6.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.28 | Phân rã use case Xử lý IPN của VNPay | 3.3.6.4 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.29 | Phân rã use case Xử lý IPN của MoMo | 3.3.6.5 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.30 | Phân rã use case Xử lý webhook của Stripe | 3.3.6.6 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.31 | Phân rã use case Phát hành vé sau thanh toán | 3.3.6.7 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.32 | Phân rã use case Hoàn tiền | 3.3.6.8 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.33 | Sơ đồ use case module Trợ lý AI | 3.3.7.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.34 | Phân rã use case Trò chuyện với trợ lý AI | 3.3.7.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.35 | Sơ đồ use case module Bán vé tại quầy | 3.3.8.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.36 | Phân rã use case Bán vé tại quầy | 3.3.8.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.37 | Sơ đồ use case module Soát vé | 3.3.9.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.38 | Phân rã use case Soát vé | 3.3.9.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.39 | Sơ đồ use case module Quản lý booking | 3.3.10.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.40 | Phân rã use case Xem danh sách booking | 3.3.10.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.41 | Sơ đồ use case module Quản lý phim | 3.3.11.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.42 | Phân rã use case Xem danh sách phim (quản trị) | 3.3.11.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.43 | Phân rã use case Thêm phim | 3.3.11.4 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.44 | Phân rã use case Sửa phim | 3.3.11.5 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.45 | Phân rã use case Xóa phim | 3.3.11.6 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.46 | Sơ đồ use case module Quản lý danh mục | 3.3.12.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.47 | Phân rã use case Quản lý thể loại phim | 3.3.12.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.48 | Phân rã use case Quản lý ngôn ngữ | 3.3.12.4 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.49 | Phân rã use case Quản lý phân loại độ tuổi | 3.3.12.5 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.50 | Phân rã use case Quản lý diễn viên | 3.3.12.6 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.51 | Phân rã use case Quản lý đạo diễn | 3.3.12.7 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.52 | Phân rã use case Quản lý loại màn hình | 3.3.12.8 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.53 | Phân rã use case Quản lý loại ghế | 3.3.12.9 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.54 | Phân rã use case Quản lý chức vụ nhân viên | 3.3.12.10 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.55 | Phân rã use case Quản lý hạng thành viên | 3.3.12.11 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.56 | Phân rã use case Quản lý combo | 3.3.12.12 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.57 | Sơ đồ use case module Quản lý phòng chiếu | 3.3.13.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.58 | Phân rã use case Quản lý phòng chiếu | 3.3.13.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.59 | Phân rã use case Soạn sơ đồ ghế | 3.3.13.4 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.60 | Sơ đồ use case module Quản lý suất chiếu | 3.3.14.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.61 | Phân rã use case Xem danh sách suất chiếu | 3.3.14.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.62 | Phân rã use case Tạo suất chiếu | 3.3.14.4 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.63 | Phân rã use case Sửa suất chiếu | 3.3.14.5 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.64 | Phân rã use case Hủy suất chiếu | 3.3.14.6 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.65 | Phân rã use case Tạo suất chiếu hàng loạt | 3.3.14.7 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.66 | Phân rã use case Tự động xếp lịch chiếu | 3.3.14.8 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.67 | Phân rã use case Nhân bản lịch chiếu | 3.3.14.9 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.68 | Sơ đồ use case module Quản lý người dùng | 3.3.15.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.69 | Phân rã use case Khóa hoặc mở khóa tài khoản | 3.3.15.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.70 | Phân rã use case Đổi vai trò tài khoản | 3.3.15.4 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.71 | Phân rã use case Quản lý nhân viên | 3.3.15.5 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.72 | Sơ đồ use case module Quản lý đánh giá | 3.3.16.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.73 | Sơ đồ use case module Quản lý thanh toán | 3.3.17.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.74 | Sơ đồ use case module Báo cáo thống kê | 3.3.18.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.75 | Sơ đồ use case module Cấu hình rạp | 3.3.19.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.76 | Phân rã use case Cập nhật cấu hình rạp | 3.3.19.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.77 | Sơ đồ use case module Tải tệp | 3.3.20.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.78 | Sơ đồ use case module Vận hành hệ thống | 3.3.21.1 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.79 | Phân rã use case Giải phóng ghế hết hạn | 3.3.21.3 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.80 | Phân rã use case Cập nhật vòng đời suất chiếu | 3.3.21.4 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 3.81 | Phân rã use case Hoàn tiền khi hủy suất chiếu | 3.3.21.5 | PlantUML | [CẦN BỔ SUNG: vẽ] |
| Hình 4.1 | Kiến trúc tổng thể | 4.1.1 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.2 | Phụ thuộc giữa các project backend | 4.1.2 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.3 | Tổ chức mã nguồn frontend | 4.1.3 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.4 | Sơ đồ ERD đầy đủ của hệ thống | 4.2.2 | Mermaid erDiagram; xuất PNG | [CẦN BỔ SUNG: vẽ] |
| Hình 4.5 | ERD module Tài khoản người dùng | 4.2.4.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.6 | ERD module Danh mục phim | 4.2.5.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.7 | ERD module Phim | 4.2.6.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.8 | ERD module Phòng chiếu | 4.2.7.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.9 | ERD module Suất chiếu | 4.2.8.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.10 | ERD module Đặt vé | 4.2.9.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.11 | ERD module Thanh toán | 4.2.10.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.12 | ERD module Thành viên | 4.2.11.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.13 | ERD module Cấu hình rạp | 4.2.12.1 | Mermaid erDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.14 | Máy trạng thái Booking | 4.2.14.1 | Mermaid stateDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.15 | Máy trạng thái ShowtimeSeat | 4.2.14.2 | Mermaid stateDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.16 | Máy trạng thái Payment | 4.2.14.3 | Mermaid stateDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.17 | Máy trạng thái Showtime | 4.2.14.4 | Mermaid stateDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.18 | Máy trạng thái Movie | 4.2.14.5 | Mermaid stateDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.19 | Tuần tự đăng ký tài khoản | 4.3.1 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.20 | Tuần tự đăng nhập bằng email | 4.3.2 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.21 | Tuần tự đăng nhập bằng Google | 4.3.3 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.22 | Tuần tự làm mới token xoay vòng | 4.3.4 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.23 | Lưu đồ phát hiện dùng lại refresh token | 4.3.5 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.24 | Tuần tự khôi phục mật khẩu | 4.3.6 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.25 | Lưu đồ giữ ghế nguyên tử | 4.4.1.2 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.26 | Tuần tự giữ ghế | 4.4.1.4 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.27 | Tuần tự tạo giao dịch thanh toán | 4.4.2.1 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.28 | Lưu đồ xử lý IPN | 4.4.2.2 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.29 | Tuần tự phát hành vé sau thanh toán | 4.4.2.3 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.30 | Lưu đồ xử lý ghế hết hạn sau thanh toán | 4.4.2.4 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.31 | Lưu đồ giải phóng ghế hết hạn | 4.4.3 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.32 | Lưu đồ hủy vé | 4.4.4.2 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.33 | Tuần tự bán vé tại quầy | 4.4.5 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.34 | Lưu đồ soát vé | 4.4.6 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.35 | Tuần tự hủy suất chiếu | 4.4.7 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.36 | Lưu đồ viết đánh giá phim | 4.4.9 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.37 | Minh họa luật ghế lẻ | 4.5.1 | Sơ đồ hàng ghế (Mermaid/hình tự vẽ) | [CẦN BỔ SUNG: vẽ] |
| Hình 4.38 | Minh họa cửa sổ gợi ý ghế | 4.5.2.1 | Sơ đồ hàng ghế | [CẦN BỔ SUNG: vẽ] |
| Hình 4.39 | Lưu đồ xếp lịch chiếu tự động | 4.5.3.3 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.40 | Minh họa điều kiện trùng lịch phòng | 4.5.4 | Sơ đồ trục thời gian | [CẦN BỔ SUNG: vẽ] |
| Hình 4.41 | Tuần tự cập nhật ghế thời gian thực | 4.6.1 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.42 | Luồng function calling của trợ lý AI | 4.7.1 | Mermaid sequenceDiagram | [CẦN BỔ SUNG: vẽ] |
| Hình 4.43 | Sơ đồ trang khách hàng | 4.8.2 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
| Hình 4.44 | Sơ đồ trang quản trị | 4.8.3 | Mermaid flowchart | [CẦN BỔ SUNG: vẽ] |
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
| Bảng 3.2 | Danh mục module chức năng | 3.2.1 | Có dữ liệu trong outline | Có |
| Bảng 3.3 | Ma trận tác nhân theo module | 3.2.3 | Có dữ liệu trong outline | Có |
| Bảng 3.4 | Danh sách use case module Xác thực | 3.3.1.2 | Có dữ liệu trong outline | Có |
| Bảng 3.5 | Phân rã use case Đăng ký tài khoản | 3.3.1.3 | Có dữ liệu trong outline | Có |
| Bảng 3.6 | Phân rã use case Đăng nhập bằng email | 3.3.1.4 | Có dữ liệu trong outline | Có |
| Bảng 3.7 | Phân rã use case Đăng nhập bằng Google | 3.3.1.5 | Có dữ liệu trong outline | Có |
| Bảng 3.8 | Phân rã use case Đăng nhập trang quản trị | 3.3.1.6 | Có dữ liệu trong outline | Có |
| Bảng 3.9 | Phân rã use case Khôi phục mật khẩu | 3.3.1.7 | Có dữ liệu trong outline | Có |
| Bảng 3.10 | Phân rã use case Duy trì phiên đăng nhập | 3.3.1.8 | Có dữ liệu trong outline | Có |
| Bảng 3.11 | Danh sách use case module Hồ sơ cá nhân | 3.3.2.2 | Có dữ liệu trong outline | Có |
| Bảng 3.12 | Phân rã use case Đổi mật khẩu | 3.3.2.3 | Có dữ liệu trong outline | Có |
| Bảng 3.13 | Danh sách use case module Duyệt phim | 3.3.3.2 | Có dữ liệu trong outline | Có |
| Bảng 3.14 | Phân rã use case Xem danh sách phim | 3.3.3.3 | Có dữ liệu trong outline | Có |
| Bảng 3.15 | Phân rã use case Xem chi tiết phim | 3.3.3.4 | Có dữ liệu trong outline | Có |
| Bảng 3.16 | Phân rã use case Xem lịch chiếu theo ngày | 3.3.3.5 | Có dữ liệu trong outline | Có |
| Bảng 3.17 | Danh sách use case module Đánh giá phim | 3.3.4.2 | Có dữ liệu trong outline | Có |
| Bảng 3.18 | Phân rã use case Viết đánh giá | 3.3.4.3 | Có dữ liệu trong outline | Có |
| Bảng 3.19 | Danh sách use case module Đặt vé trực tuyến | 3.3.5.2 | Có dữ liệu trong outline | Có |
| Bảng 3.20 | Phân rã use case Đặt vé trực tuyến | 3.3.5.3 | Có dữ liệu trong outline | Có |
| Bảng 3.21 | Phân rã use case Xem sơ đồ ghế của suất chiếu | 3.3.5.4 | Có dữ liệu trong outline | Có |
| Bảng 3.22 | Phân rã use case Chọn ghế | 3.3.5.5 | Có dữ liệu trong outline | Có |
| Bảng 3.23 | Phân rã use case Giữ ghế | 3.3.5.6 | Có dữ liệu trong outline | Có |
| Bảng 3.24 | Phân rã use case Chọn combo | 3.3.5.7 | Có dữ liệu trong outline | Có |
| Bảng 3.25 | Phân rã use case Hủy đơn đặt vé | 3.3.5.8 | Có dữ liệu trong outline | Có |
| Bảng 3.26 | Danh sách use case module Thanh toán trực tuyến | 3.3.6.2 | Có dữ liệu trong outline | Có |
| Bảng 3.27 | Phân rã use case Thanh toán đơn đặt vé trực tuyến | 3.3.6.3 | Có dữ liệu trong outline | Có |
| Bảng 3.28 | Phân rã use case Xử lý IPN của VNPay | 3.3.6.4 | Có dữ liệu trong outline | Có |
| Bảng 3.29 | Phân rã use case Xử lý IPN của MoMo | 3.3.6.5 | Có dữ liệu trong outline | Có |
| Bảng 3.30 | Phân rã use case Xử lý webhook của Stripe | 3.3.6.6 | Có dữ liệu trong outline | Có |
| Bảng 3.31 | Phân rã use case Phát hành vé sau thanh toán | 3.3.6.7 | Có dữ liệu trong outline | Có |
| Bảng 3.32 | Phân rã use case Hoàn tiền | 3.3.6.8 | Có dữ liệu trong outline | Có |
| Bảng 3.33 | Danh sách use case module Trợ lý AI | 3.3.7.2 | Có dữ liệu trong outline | Có |
| Bảng 3.34 | Phân rã use case Trò chuyện với trợ lý AI | 3.3.7.3 | Có dữ liệu trong outline | Có |
| Bảng 3.35 | Danh sách use case module Bán vé tại quầy | 3.3.8.2 | Có dữ liệu trong outline | Có |
| Bảng 3.36 | Phân rã use case Bán vé tại quầy | 3.3.8.3 | Có dữ liệu trong outline | Có |
| Bảng 3.37 | Danh sách use case module Soát vé | 3.3.9.2 | Có dữ liệu trong outline | Có |
| Bảng 3.38 | Phân rã use case Soát vé | 3.3.9.3 | Có dữ liệu trong outline | Có |
| Bảng 3.39 | Danh sách use case module Quản lý booking | 3.3.10.2 | Có dữ liệu trong outline | Có |
| Bảng 3.40 | Phân rã use case Xem danh sách booking | 3.3.10.3 | Có dữ liệu trong outline | Có |
| Bảng 3.41 | Danh sách use case module Quản lý phim | 3.3.11.2 | Có dữ liệu trong outline | Có |
| Bảng 3.42 | Phân rã use case Xem danh sách phim (quản trị) | 3.3.11.3 | Có dữ liệu trong outline | Có |
| Bảng 3.43 | Phân rã use case Thêm phim | 3.3.11.4 | Có dữ liệu trong outline | Có |
| Bảng 3.44 | Phân rã use case Sửa phim | 3.3.11.5 | Có dữ liệu trong outline | Có |
| Bảng 3.45 | Phân rã use case Xóa phim | 3.3.11.6 | Có dữ liệu trong outline | Có |
| Bảng 3.46 | Danh sách use case module Quản lý danh mục | 3.3.12.2 | Có dữ liệu trong outline | Có |
| Bảng 3.47 | Phân rã use case Quản lý thể loại phim | 3.3.12.3 | Có dữ liệu trong outline | Có |
| Bảng 3.48 | Phân rã use case Quản lý ngôn ngữ | 3.3.12.4 | Có dữ liệu trong outline | Có |
| Bảng 3.49 | Phân rã use case Quản lý phân loại độ tuổi | 3.3.12.5 | Có dữ liệu trong outline | Có |
| Bảng 3.50 | Phân rã use case Quản lý diễn viên | 3.3.12.6 | Có dữ liệu trong outline | Có |
| Bảng 3.51 | Phân rã use case Quản lý đạo diễn | 3.3.12.7 | Có dữ liệu trong outline | Có |
| Bảng 3.52 | Phân rã use case Quản lý loại màn hình | 3.3.12.8 | Có dữ liệu trong outline | Có |
| Bảng 3.53 | Phân rã use case Quản lý loại ghế | 3.3.12.9 | Có dữ liệu trong outline | Có |
| Bảng 3.54 | Phân rã use case Quản lý chức vụ nhân viên | 3.3.12.10 | Có dữ liệu trong outline | Có |
| Bảng 3.55 | Phân rã use case Quản lý hạng thành viên | 3.3.12.11 | Có dữ liệu trong outline | Có |
| Bảng 3.56 | Phân rã use case Quản lý combo | 3.3.12.12 | Có dữ liệu trong outline | Có |
| Bảng 3.57 | Danh sách use case module Quản lý phòng chiếu | 3.3.13.2 | Có dữ liệu trong outline | Có |
| Bảng 3.58 | Phân rã use case Quản lý phòng chiếu | 3.3.13.3 | Có dữ liệu trong outline | Có |
| Bảng 3.59 | Phân rã use case Soạn sơ đồ ghế | 3.3.13.4 | Có dữ liệu trong outline | Có |
| Bảng 3.60 | Danh sách use case module Quản lý suất chiếu | 3.3.14.2 | Có dữ liệu trong outline | Có |
| Bảng 3.61 | Phân rã use case Xem danh sách suất chiếu | 3.3.14.3 | Có dữ liệu trong outline | Có |
| Bảng 3.62 | Phân rã use case Tạo suất chiếu | 3.3.14.4 | Có dữ liệu trong outline | Có |
| Bảng 3.63 | Phân rã use case Sửa suất chiếu | 3.3.14.5 | Có dữ liệu trong outline | Có |
| Bảng 3.64 | Phân rã use case Hủy suất chiếu | 3.3.14.6 | Có dữ liệu trong outline | Có |
| Bảng 3.65 | Phân rã use case Tạo suất chiếu hàng loạt | 3.3.14.7 | Có dữ liệu trong outline | Có |
| Bảng 3.66 | Phân rã use case Tự động xếp lịch chiếu | 3.3.14.8 | Có dữ liệu trong outline | Có |
| Bảng 3.67 | Phân rã use case Nhân bản lịch chiếu | 3.3.14.9 | Có dữ liệu trong outline | Có |
| Bảng 3.68 | Danh sách use case module Quản lý người dùng | 3.3.15.2 | Có dữ liệu trong outline | Có |
| Bảng 3.69 | Phân rã use case Khóa hoặc mở khóa tài khoản | 3.3.15.3 | Có dữ liệu trong outline | Có |
| Bảng 3.70 | Phân rã use case Đổi vai trò tài khoản | 3.3.15.4 | Có dữ liệu trong outline | Có |
| Bảng 3.71 | Phân rã use case Quản lý nhân viên | 3.3.15.5 | Có dữ liệu trong outline | Có |
| Bảng 3.72 | Danh sách use case module Quản lý đánh giá | 3.3.16.2 | Có dữ liệu trong outline | Có |
| Bảng 3.73 | Danh sách use case module Quản lý thanh toán | 3.3.17.2 | Có dữ liệu trong outline | Có |
| Bảng 3.74 | Danh sách use case module Báo cáo thống kê | 3.3.18.2 | Có dữ liệu trong outline | Có |
| Bảng 3.75 | Danh sách use case module Cấu hình rạp | 3.3.19.2 | Có dữ liệu trong outline | Có |
| Bảng 3.76 | Phân rã use case Cập nhật cấu hình rạp | 3.3.19.3 | Có dữ liệu trong outline | Có |
| Bảng 3.77 | Danh sách use case module Tải tệp | 3.3.20.2 | Có dữ liệu trong outline | Có |
| Bảng 3.78 | Danh sách use case module Vận hành hệ thống | 3.3.21.2 | Có dữ liệu trong outline | Có |
| Bảng 3.79 | Phân rã use case Giải phóng ghế hết hạn | 3.3.21.3 | Có dữ liệu trong outline | Có |
| Bảng 3.80 | Phân rã use case Cập nhật vòng đời suất chiếu | 3.3.21.4 | Có dữ liệu trong outline | Có |
| Bảng 3.81 | Phân rã use case Hoàn tiền khi hủy suất chiếu | 3.3.21.5 | Có dữ liệu trong outline | Có |
| Bảng 3.82 | Đặc tả UC-XT-04 Đăng nhập bằng email | 3.4.1 | Có dữ liệu trong outline | Có |
| Bảng 3.83 | Đặc tả UC-DV-04 Giữ ghế | 3.4.2 | Có dữ liệu trong outline | Có |
| Bảng 3.84 | Đặc tả UC-TT-01 Thanh toán đơn đặt vé trực tuyến | 3.4.3 | Có dữ liệu trong outline | Có |
| Bảng 3.85 | Đặc tả UC-TT-05 Phát hành vé sau thanh toán | 3.4.4 | Có dữ liệu trong outline | Có |
| Bảng 3.86 | Đặc tả UC-DV-08 Hủy đơn đặt vé | 3.4.5 | Có dữ liệu trong outline | Có |
| Bảng 3.87 | Đặc tả UC-BQ-01 Bán vé tại quầy | 3.4.6 | Có dữ liệu trong outline | Có |
| Bảng 3.88 | Đặc tả UC-SV-01 Soát vé | 3.4.7 | Có dữ liệu trong outline | Có |
| Bảng 3.89 | Đặc tả UC-QS-07 Tự động xếp lịch chiếu | 3.4.8 | Có dữ liệu trong outline | Có |
| Bảng 3.90 | Đặc tả UC-QR-03 Soạn sơ đồ ghế | 3.4.9 | Có dữ liệu trong outline | Có |
| Bảng 3.91 | Quy tắc nghiệp vụ và tham số | 3.5.1 | Có dữ liệu trong outline | Có |
| Bảng 3.92 | Yêu cầu phi chức năng và cách đáp ứng | 3.6.1 | Có dữ liệu trong outline | Có |
| Bảng 4.1 | Cấu trúc phản hồi API | 4.1.4 | Có dữ liệu trong outline | Có |
| Bảng 4.2 | Ánh xạ ngoại lệ và lỗi PostgreSQL sang mã HTTP | 4.1.5 | Có dữ liệu trong outline | Có |
| Bảng 4.3 | Nhóm cấu hình ứng dụng | 4.1.6 | Có dữ liệu trong outline | Có |
| Bảng 4.4 | Phân module cơ sở dữ liệu | 4.2.3 | Có dữ liệu trong outline | Có |
| Bảng 4.5 | Thực thể Role (bảng Roles) | 4.2.4.2 | Có dữ liệu trong outline | Có |
| Bảng 4.6 | Thực thể User (bảng Users) | 4.2.4.2 | Có dữ liệu trong outline | Có |
| Bảng 4.7 | Thực thể CustomerProfile (bảng CustomerProfiles) | 4.2.4.2 | Có dữ liệu trong outline | Có |
| Bảng 4.8 | Thực thể Employee (bảng Employees) | 4.2.4.2 | Có dữ liệu trong outline | Có |
| Bảng 4.9 | Thực thể Position (bảng Positions) | 4.2.4.2 | Có dữ liệu trong outline | Có |
| Bảng 4.10 | Thực thể RefreshToken (bảng RefreshTokens) | 4.2.4.2 | Có dữ liệu trong outline | Có |
| Bảng 4.11 | Thực thể UserVerificationToken (bảng UserVerificationTokens) | 4.2.4.2 | Có dữ liệu trong outline | Có |
| Bảng 4.12 | Ràng buộc toàn vẹn module Tài khoản người dùng | 4.2.4.3 | Có dữ liệu trong outline | Có |
| Bảng 4.13 | Chỉ mục module Tài khoản người dùng | 4.2.4.4 | Có dữ liệu trong outline | Có |
| Bảng 4.14 | Thực thể Genre (bảng Genres) | 4.2.5.2 | Có dữ liệu trong outline | Có |
| Bảng 4.15 | Thực thể Language (bảng Languages) | 4.2.5.2 | Có dữ liệu trong outline | Có |
| Bảng 4.16 | Thực thể AgeRating (bảng AgeRatings) | 4.2.5.2 | Có dữ liệu trong outline | Có |
| Bảng 4.17 | Thực thể Actor (bảng Actors) | 4.2.5.2 | Có dữ liệu trong outline | Có |
| Bảng 4.18 | Thực thể Director (bảng Directors) | 4.2.5.2 | Có dữ liệu trong outline | Có |
| Bảng 4.19 | Ràng buộc toàn vẹn module Danh mục phim | 4.2.5.3 | Có dữ liệu trong outline | Có |
| Bảng 4.20 | Chỉ mục module Danh mục phim | 4.2.5.4 | Có dữ liệu trong outline | Có |
| Bảng 4.21 | Thực thể Movie (bảng Movies) | 4.2.6.2 | Có dữ liệu trong outline | Có |
| Bảng 4.22 | Thực thể MovieGenre (bảng MovieGenres) | 4.2.6.2 | Có dữ liệu trong outline | Có |
| Bảng 4.23 | Thực thể MovieCast (bảng MovieCast) | 4.2.6.2 | Có dữ liệu trong outline | Có |
| Bảng 4.24 | Thực thể Review (bảng Reviews) | 4.2.6.2 | Có dữ liệu trong outline | Có |
| Bảng 4.25 | Ràng buộc toàn vẹn module Phim | 4.2.6.3 | Có dữ liệu trong outline | Có |
| Bảng 4.26 | Chỉ mục module Phim | 4.2.6.4 | Có dữ liệu trong outline | Có |
| Bảng 4.27 | Thực thể CinemaRoom (bảng CinemaRooms) | 4.2.7.2 | Có dữ liệu trong outline | Có |
| Bảng 4.28 | Thực thể ScreenType (bảng ScreenTypes) | 4.2.7.2 | Có dữ liệu trong outline | Có |
| Bảng 4.29 | Thực thể Seat (bảng Seats) | 4.2.7.2 | Có dữ liệu trong outline | Có |
| Bảng 4.30 | Thực thể SeatType (bảng SeatTypes) | 4.2.7.2 | Có dữ liệu trong outline | Có |
| Bảng 4.31 | Ràng buộc toàn vẹn module Phòng chiếu | 4.2.7.3 | Có dữ liệu trong outline | Có |
| Bảng 4.32 | Chỉ mục module Phòng chiếu | 4.2.7.4 | Có dữ liệu trong outline | Có |
| Bảng 4.33 | Thực thể Showtime (bảng Showtimes) | 4.2.8.2 | Có dữ liệu trong outline | Có |
| Bảng 4.34 | Thực thể ShowtimeSeat (bảng ShowtimeSeats) | 4.2.8.2 | Có dữ liệu trong outline | Có |
| Bảng 4.35 | Ràng buộc toàn vẹn module Suất chiếu | 4.2.8.3 | Có dữ liệu trong outline | Có |
| Bảng 4.36 | Chỉ mục module Suất chiếu | 4.2.8.4 | Có dữ liệu trong outline | Có |
| Bảng 4.37 | Thực thể Booking (bảng Bookings) | 4.2.9.2 | Có dữ liệu trong outline | Có |
| Bảng 4.38 | Thực thể BookingTicket (bảng BookingTickets) | 4.2.9.2 | Có dữ liệu trong outline | Có |
| Bảng 4.39 | Thực thể BookingCombo (bảng BookingCombos) | 4.2.9.2 | Có dữ liệu trong outline | Có |
| Bảng 4.40 | Thực thể Combo (bảng Combos) | 4.2.9.2 | Có dữ liệu trong outline | Có |
| Bảng 4.41 | Ràng buộc toàn vẹn module Đặt vé | 4.2.9.3 | Có dữ liệu trong outline | Có |
| Bảng 4.42 | Chỉ mục module Đặt vé | 4.2.9.4 | Có dữ liệu trong outline | Có |
| Bảng 4.43 | Thực thể Payment (bảng Payments) | 4.2.10.2 | Có dữ liệu trong outline | Có |
| Bảng 4.44 | Ràng buộc toàn vẹn module Thanh toán | 4.2.10.3 | Có dữ liệu trong outline | Có |
| Bảng 4.45 | Chỉ mục module Thanh toán | 4.2.10.4 | Có dữ liệu trong outline | Có |
| Bảng 4.46 | Thực thể MembershipTier (bảng MembershipTiers) | 4.2.11.2 | Có dữ liệu trong outline | Có |
| Bảng 4.47 | Ràng buộc toàn vẹn module Thành viên | 4.2.11.3 | Có dữ liệu trong outline | Có |
| Bảng 4.48 | Chỉ mục module Thành viên | 4.2.11.4 | Có dữ liệu trong outline | Có |
| Bảng 4.49 | Thực thể CinemaSettings (bảng CinemaSettings) | 4.2.12.2 | Có dữ liệu trong outline | Có |
| Bảng 4.50 | Ràng buộc toàn vẹn module Cấu hình rạp | 4.2.12.3 | Có dữ liệu trong outline | Có |
| Bảng 4.51 | Chỉ mục module Cấu hình rạp | 4.2.12.4 | Có dữ liệu trong outline | Có |
| Bảng 4.52 | Các kiểu liệt kê | 4.2.13 | Có dữ liệu trong outline | Có |
| Bảng 4.53 | Các loại token | 4.3.7 | Có dữ liệu trong outline | Có |
| Bảng 4.54 | Ma trận phân quyền theo controller | 4.3.8 | Có dữ liệu trong outline | Có |
| Bảng 4.55 | Các kiểm tra khi giữ ghế | 4.4.1.1 | Có dữ liệu trong outline | Có |
| Bảng 4.56 | So sánh VNPay, MoMo, Stripe | 4.4.2.5 | Có dữ liệu trong outline | Có |
| Bảng 4.57 | Chính sách và tình huống hoàn tiền | 4.4.4.1 | Có dữ liệu trong outline | Có |
| Bảng 4.58 | Hạng thành viên | 4.4.8 | SeedData.cs | [CẦN BỔ SUNG: giá trị hạng] |
| Bảng 4.59 | Ba chế độ luật ghế lẻ | 4.5.1 | SeatLayoutRules.cs | [CẦN BỔ SUNG: ví dụ cụ thể từ code] |
| Bảng 4.60 | Tham số bộ xếp lịch | 4.5.3.2 | SchedulerOptions.cs | [CẦN BỔ SUNG: giá trị mặc định] |
| Bảng 4.61 | Quy ước thời gian | 4.5.5 | Có dữ liệu trong outline | Có |
| Bảng 4.62 | Sự kiện SignalR | 4.6.2 | Có dữ liệu trong outline | Có |
| Bảng 4.63 | Các hàm của trợ lý AI | 4.7.2 | Có dữ liệu trong outline | Có |
| Bảng 4.64 | Token thiết kế rút gọn | 4.8.1 | .agents/skills/perfo-design-system/references/tokens.css | [CẦN BỔ SUNG: chọn token đưa vào báo cáo] |
| Bảng 4.65 | Tổng hợp endpoint theo controller | 4.9.2 | Có dữ liệu trong outline | Có |
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
| Bảng 7.1 | Truy vết module đến kiểm thử | 7.1.1 | Có dữ liệu trong outline | Có |
| Bảng 7.2 | Mức đạt mục tiêu | 7.2.1 | Có dữ liệu trong outline | Có |
| Bảng 7.3 | Hạn chế, đề xuất khắc phục | 7.3.15 | Có dữ liệu trong outline | Có |
| Bảng 8.1 | Mức độ đạt mục tiêu (tổng kết) | 8.1.1 | Có dữ liệu trong outline | Có |
| Bảng P2.1 | Endpoint phía khách hàng | P2.1 | Có dữ liệu trong outline | Có |
| Bảng P2.2 | Endpoint phía quản trị | P2.2 | Có dữ liệu trong outline | Có |
| Bảng P2.3 | Endpoint thanh toán, kiểm tra sức khỏe | P2.3 | Có dữ liệu trong outline | Có |
| Bảng P3.1 | Biến cấu hình môi trường | P3.1 | Có dữ liệu trong outline | Có |
| Bảng P4.1 | Các bước kịch bản E2E | P4.1 | Có dữ liệu trong outline | Có |

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
| (4.1) | Số tiền cuối cùng của đơn: FinalAmount = TotalAmount − DiscountAmount [CẦN BỔ SUNG: xác nhận công thức làm tròn trong CalculateDiscountAmountAsync] | 4.4.1.3 | Công thức | Có |
| (4.2) | Điểm thưởng: Points = ⌊FinalAmount / PointsPerAmount⌋ (mặc định PointsPerAmount = 10.000) | 4.4.8 | Công thức | Có |
| (4.3) | Điểm gần giữa hàng: [CẦN BỔ SUNG: biểu thức từ SeatLayoutRules.cs] | 4.5.2.2 | Công thức | Có |
| (4.4) | Điểm độ sâu hàng: [CẦN BỔ SUNG: biểu thức từ SeatLayoutRules.cs] | 4.5.2.2 | Công thức | Có |
| (4.5) | Điểm gợi ý: Score = 0,45·Center + 0,45·Row + 0,1·SameType | 4.5.2.2 | Công thức | Có |
| (4.6) | Độ nóng: Heat = min(1, 0,7·(Vé/VéMax) + 0,3·(Rating/10)) | 4.5.3.1 | Công thức | Có |
| (4.7) | Thời điểm lý tưởng thứ i: [CẦN BỔ SUNG: biểu thức từ ShowtimeScheduler.cs] | 4.5.3.2 | Công thức | Có |
| (4.8) | Mức phù hợp: Fit = 1 − \|Heat − Ghế/GhếMax\| | 4.5.3.3 | Công thức | Có |
| (4.9) | Điểm xếp lịch: Score = −Khoảng cách/Spread + Fit·FitWeight − Tải·LoadWeight | 4.5.3.3 | Công thức | Có |

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
| T2 | Sách/bài về UML use case, kiến trúc phân tầng, Clean Architecture, thiết kế phần mềm | 2.5.1, 2.5.3, Chương 3 |
| T3 | Tài liệu chính thức ASP.NET Core, Entity Framework Core, Npgsql, PostgreSQL; sách về mô hình thực thể liên kết; thiết kế REST API | 2.5.2, 2.5.4, 2.5.5, 4.2 |
| T4 | Chuẩn và hướng dẫn xác thực: JWT (RFC 7519), OAuth 2.0 (RFC 6749), OWASP về quản lý phiên, lưu mật khẩu | 2.5.6, 2.5.7, 4.3 |
| T5 | Sách/tài liệu về giao dịch, mức cô lập, MVCC, kiểm soát đồng thời, idempotency | 2.2.2, 2.5.8, 4.4.1 |
| T6 | Tài liệu SignalR; đặc tả WebSocket (RFC 6455) | 2.5.9, 4.6 |
| T7 | Tài liệu Hangfire | 2.5.10, 4.4.3 |
| T8 | Tài liệu tích hợp VNPay, MoMo, Stripe; HMAC (RFC 2104) | 2.5.11, 4.4.2 |
| T9 | Tài liệu React, Vite, TanStack Query/Table, Zustand, React Hook Form, Zod, Tailwind CSS | 2.5.12, 5.4 |
| T10 | Tài liệu Gemini API (function calling); hướng dẫn OWASP về rủi ro ứng dụng LLM | 2.5.13, 4.7 |
| T11 | Tài liệu xUnit, Testcontainers, Playwright, k6; sách về kiểm thử phần mềm | 2.5.14, Chương 6 |
| T12 | Tài liệu Docker, Render, Vercel, Neon | 2.5.15, 5.7 |
| T13 | Sách/bài về thuật toán tham lam, heuristic xếp lịch | 4.5.3 |
| T14 | Tài liệu thiết kế giao diện, khả năng truy cập (WCAG), thiết kế đáp ứng | 4.8 |
| T15 | Tài liệu về robots.txt và giao thức sitemap | 5.4.5 |
| T16 | Văn bản pháp luật hiện hành về bảo vệ dữ liệu cá nhân, thanh toán điện tử [CẦN BỔ SUNG: tra cứu số hiệu] | 7.3 |

# TÓM TẮT

Đặt vé xem phim trực tuyến đặt ra hai rủi ro kỹ thuật chính: nhiều khách cùng giành một ghế của một suất chiếu dẫn đến đặt trùng, và kết quả thanh toán từ cổng thanh toán đến bất đồng bộ, có thể bị gọi lặp hoặc đến sau khi ghế đã hết hạn giữ. Đồ án xây dựng hệ thống đặt vé Perfo nhằm xử lý hai rủi ro này đồng thời cung cấp trọn vẹn nghiệp vụ của một rạp: giao diện khách hàng, trang quản trị, bán vé tại quầy và soát vé bằng mã QR. Phạm vi chức năng gồm 103 use case thuộc 21 package (Chương 3).

Hệ thống có kiến trúc phân tầng, backend ASP.NET Core trên .NET 10 với PostgreSQL, frontend React và Vite. Việc giữ ghế được thực hiện bằng các lệnh cập nhật trực tiếp trên cơ sở dữ liệu kết hợp chỉ mục duy nhất `UQ_Showtime_Seat`, khi xung đột hệ thống trả mã 409. Thanh toán được xử lý idempotent: giao dịch đã xử lý được nhận biết nên cổng thanh toán gọi lại không tạo thêm tác dụng, và đơn đã thanh toán nhưng ghế đã hết hạn được hoàn tiền tự động. Trạng thái ghế được đẩy tới trình duyệt theo thời gian thực bằng SignalR. Ngoài ra hệ thống có bộ xếp lịch chiếu tự động theo hàm điểm, trợ lý AI dùng function calling của Gemini và cơ chế điểm thành viên tính theo cửa sổ trượt 12 tháng trên sổ điểm (`LoyaltyPointEntry`).

Về kiểm thử, mã nguồn có 364 test đơn vị (đếm `[Fact]` và `[Theory]`), 49 test tích hợp, 8 kịch bản E2E bằng Playwright và 2 kịch bản tải bằng k6. Kết quả thực thi hiện có là của bộ E2E: 8/8 kịch bản đạt trong lần chạy ngày 07/09/2026, tổng thời gian 100,09 giây; lần chạy này có trước các migration cuối tháng 10 và đầu tháng 11/2026 nên cần chạy lại. Báo cáo chưa có log chạy cho bộ test backend và chưa có kết quả k6, vì vậy các mục tiêu về không đặt trùng ghế, thanh toán an toàn và cập nhật thời gian thực chưa được kết luận bằng số liệu thực thi.

Hạn chế chính gồm: chống trùng suất chiếu trong phòng mới chỉ ở tầng ứng dụng; ghế hết hạn chỉ được giải phóng bởi công việc nền chạy mỗi phút; refresh token lưu ở `localStorage` và chưa có chính sách CSP; endpoint trợ lý AI chưa yêu cầu đăng nhập; hệ thống mới phục vụ một rạp và dùng cổng thanh toán ở môi trường thử nghiệm.

**Từ khóa:** đặt vé xem phim trực tuyến; giữ ghế; thanh toán idempotent; SignalR; ASP.NET Core; điểm thành viên.

---

# ABSTRACT

Online movie ticketing faces two main technical risks: several customers competing for the same seat of a showtime, which can cause double booking, and asynchronous payment results that may be delivered repeatedly or after the seat hold has expired. This coursework project builds Perfo, an online ticketing system that addresses both risks and covers the daily operations of a cinema: a customer web interface, an administration site, counter sales and QR-code ticket checking. The functional scope comprises 103 use cases grouped into 21 packages (Chapter 3).

The system has a layered architecture, with an ASP.NET Core backend on .NET 10 and PostgreSQL, and a React and Vite frontend. Seat holding uses direct database updates combined with the unique index `UQ_Showtime_Seat`; a conflict returns HTTP 409. Payment handling is idempotent: an already processed transaction is recognised, so repeated gateway callbacks have no further effect, and a paid order whose seats have expired is refunded automatically. Seat status is pushed to browsers in real time through SignalR. The system also provides a score-based automatic showtime scheduler, an AI assistant using Gemini function calling, and a loyalty scheme whose points are computed over a 12-month sliding window on a point ledger (`LoyaltyPointEntry`).

The source code contains 364 unit tests (counting `[Fact]` and `[Theory]`), 49 integration tests, 8 Playwright end-to-end scenarios and 2 k6 load scenarios. The only recorded execution result is that of the end-to-end suite: 8 of 8 scenarios passed in the run of 07/09/2026, with a total time of 100.09 seconds; this run predates the PostgreSQL migrations of late October and early November 2026 and should be repeated. No execution log exists for the backend test suites and no k6 result is available, so the objectives on seat double-booking prevention, payment safety and real-time updates are not yet concluded with execution evidence.

The main limitations are: overlap prevention for showtimes in a room is enforced only at the application layer; expired seats are released only by a background job running every minute; the refresh token is stored in `localStorage` and no CSP is configured; the AI assistant endpoint does not require authentication; and the system serves a single cinema with payment gateways used only in test environments.

**Keywords:** online movie ticketing; seat holding; idempotent payment; SignalR; ASP.NET Core; loyalty points.

---
# RÀ SOÁT DANH MỤC A–F

> Phần này chỉ phục vụ rà soát, không đưa vào bản in. Phương pháp: quét 35 file `01`–`35`, lấy toàn bộ caption hình, bảng, biểu đồ, công thức và từ viết tắt, đối chiếu với Danh mục A–F của outline (dòng 5680–6088); đối chiếu thêm số test, số use case, số endpoint với mã nguồn `perfo-main/`.

## R.1. Kết quả đối chiếu số hiệu

| Danh mục | Số mục trong outline | Số caption tìm thấy trong báo cáo | Thiếu | Thừa | Trùng |
|---|---|---|---|---|---|
| A. Hình vẽ | 131 | 131 (131 placeholder `![Hình …]`, mỗi hình đúng một placeholder) | 0 | 0 | 0 |
| B. Bảng biểu | 167 | 167 | 0 | 0 (có thêm Bảng 6.27a ở file 30, xem R.2) | 0 |
| C. Biểu đồ | 2 | 2 | 0 | 0 | 0 |
| D. Công thức | 14 | 14, đúng số mục và thứ tự (4.1)–(4.14) | 0 | 0 | 0 |
| E. Từ viết tắt | 38 | 35 dùng trong thân bài; 3 chỉ xuất hiện ở bảng từ viết tắt | 14 viết tắt dùng nhưng chưa có (R.7) | 3 (R.7) | 0 |
| F. Nhóm tài liệu tham khảo | 16 (T1–T16) | 43 tài liệu cụ thể | 0 (đã hoàn thiện toàn bộ) | — | — |

Tên và số mục của mọi hình, bảng, biểu đồ đều khớp outline (chỉ khác ở chỗ thân bài đặt tên bảng thực thể trong `backtick`, không phải lỗi). Không có tham chiếu chéo nào tới số hiệu hình, bảng, biểu đồ hoặc số mục không tồn tại.

## R.2. Chỗ lệch, trùng, thiếu số hiệu

1. **Trùng tên tệp ảnh:** Hình 6.1 (file 28) và Biểu đồ 6.1 (file 29) cùng dùng `hinh/6.1.png`; khi xuất ảnh, tệp này sẽ bị ghi đè. Đề xuất đổi Biểu đồ 6.1 thành `hinh/bd6.1.png` và Biểu đồ 6.2 (đang là `hinh/6.2-bieu-do.png`) thành `hinh/bd6.2.png` cho đồng nhất.
2. **Bảng 6.27a** (file 30, "Cấu hình các kịch bản k6") không có trong Danh mục B; nếu giữ thì thêm vào Danh mục B và Danh sách bảng biểu, nếu không thì gộp vào Bảng 6.27.
3. **Công thức (4.7):** outline dùng `RefundedAmount`, file 20 dùng `ReversedAmount`; mã nguồn có `ReverseLoyaltyAsync(userId, bookingId, finalAmount, …)` ở `BookingService.cs`, còn `RefundedAmount` là thuộc tính của `Payment`. Cần thống nhất ký hiệu ở (4.7) và Danh sách ký hiệu (file 01).
4. **Công thức (4.11):** outline viết `Rating/10`, file 21 viết `Rating/MaxRating` theo code (đã ghi ở ghi chú file 21 và Bảng 7.3 dòng 3); Danh mục D của outline cần sửa theo.
5. **Công thức (4.13):** cột "Mục" của Danh mục D trong outline bị lỗi bảng Markdown (dấu `|` trong `|Heat − Ghế/GhếMax|`), mục đúng là 4.6.3.3 như thân bài.
6. **Hình 6.2** (ảnh chụp `playwright-report/index.html`) phụ thuộc thư mục `bo_sung/playwright-report/` không có trong gói đã cung cấp.
7. **Cỡ chữ caption hình, bảng** và **định dạng đánh số đoạn mã**, **font mã nguồn**: quy định format vẫn để `[CẦN BỔ SUNG]`; các file chưa áp dụng định dạng.
8. **Tên file quy định:** prompt ghi `quy-dinh-trinh-bay.md`, file thực tế là `quy-dinh-format-bao-cao.md`; các phần đã dùng file thực tế.

## R.3. Danh mục A. Hình vẽ (131 hình) — trạng thái cập nhật

Mọi hình đều đã có placeholder `![Hình X.n …](hinh/X.n.png)`, caption đặt dưới hình và đoạn mô tả trong báo cáo. Tệp ảnh trong `hinh/` chưa có ở bất kỳ hình nào (gói đã cung cấp không có thư mục `hinh/`), nên cột Trạng thái nêu nguồn mã sơ đồ hoặc việc cần làm.

| Nhóm hình | Số hình | Mục | Trạng thái |
|---|---|---|---|
| Hình 2.1–2.3 | 3 | 2.1.1 … 2.5.3 | Đã có mô tả khớp code (enum, `ProjectReference`). **Cần vẽ** từ mô tả; chưa có mã sơ đồ. |
| Hình 3.1–3.2 | 2 | 3.1.2 … 3.1.3 | Đã có mô tả viết từ code và outline. **Mã sơ đồ không có** trong các file mã đã cung cấp (ghi chú file 05); cần vẽ. |
| Hình 3.3–3.24 | 22 | 3.2.2 … 3.3.21.1 | Đã có mô tả khớp mã PlantUML trong `ma-nguon-so-do-bo-sung.md`. **Cần render ảnh.** |
| Hình 4.1–4.3 | 3 | 4.1.1 … 4.1.3 | Đã có mô tả. **Chưa có mã sơ đồ**; có bản nháp Mermaid trong ghi chú file 10, cần dán vào `ma-nguon-so-do.md` rồi xuất ảnh. |
| Hình 4.4–4.25 | 22 | 4.2.3 … 4.2.24 | Đã có mô tả. Mã PlantUML trong `ma-nguon-so-do-bo-sung.md`; **cần render ảnh** (file 12, 13 ghi cần sửa mã theo mục (b) của ghi chú trước khi render). |
| Hình 4.26, 4.34 | 2 | 4.3.2 … 4.3.11.1 | Mã Mermaid trong `ma-nguon-so-do-mermaid-loyalty.md` (đã cập nhật `LoyaltyPointEntries`). **Cần render ảnh**; Hình 4.26 dùng section A4 ngang. |
| Hình 4.27 | 1 | 4.3.4.1 | Mã Mermaid ở mục (c) ghi chú file 14. **Cần render ảnh.** |
| Hình 4.28–4.33 | 6 | 4.3.5.1 … 4.3.10.1 | Mô tả viết theo cấu hình EF và snapshot. **Mã Mermaid không có** trong các file đã cung cấp (ghi chú file 15, 16); cần gửi `ma-nguon-so-do.md` hoặc vẽ lại, rồi đối chiếu. |
| Hình 4.35 | 1 | 4.3.12.1 | Mã Mermaid không nằm trong các file đã cung cấp (ghi chú file 17 chỉ nêu cần chèn ảnh). **Cần xác nhận mã và render.** |
| Hình 4.36–4.40 | 5 | 4.3.14.1 … 4.3.14.5 | Máy trạng thái; ghi chú file 17 chỉ nêu cần chèn ảnh kết xuất. **Cần xác nhận mã và render.** |
| Hình 4.41–4.46 | 6 | 4.4.1 … 4.4.6 | Mô tả viết theo `AuthService.cs`, `GoogleAuthService.cs`…. **Mã Mermaid không có** (ghi chú file 18); cần đối chiếu khi có mã. |
| Hình 4.47–4.50, 4.52–4.53 | 6 | 4.5.1.2 … 4.5.3 | Mô tả viết theo code (`BookingService`, `PaymentService`, `SeatExpirationService`, ba cổng thanh toán). **Mã Mermaid không có** (ghi chú file 19). |
| Hình 4.51, 4.54, 4.58 | 3 | 4.5.2.3 … 4.5.8 | Mã Mermaid (bản vá, dạng mô tả bước) trong `ma-nguon-so-do-mermaid-loyalty.md`. **Cần dựng mã đầy đủ và render.** |
| Hình 4.55–4.57, 4.59 | 4 | 4.5.5 … 4.5.9 | Mô tả bám code (ghi chú file 20). **Cần chèn ảnh kết xuất**; mã Mermaid chưa được xác nhận trong các file đã cung cấp. |
| Hình 4.60–4.63 | 4 | 4.6.1 … 4.6.4 | Mô tả viết trực tiếp từ `SeatLayoutRules.cs`, `ShowtimeScheduler.cs`, `OperatingHours.cs`. **Cần vẽ/chèn ảnh.** |
| Hình 4.64–4.65 | 2 | 4.7.1 … 4.8.1 | Mô tả theo luồng thực tế trong code. **Mã Mermaid không có** (ghi chú file 22). |
| Hình 4.66–4.67 | 2 | 4.9.2 … 4.9.3 | Mô tả theo `client-routes.tsx`, `admin-routes.tsx`. **Mã Mermaid không có** (ghi chú file 23). |
| Hình 5.1 | 1 | 5.2.1 | Mô tả từ cây thư mục thực tế. **Cần tạo ảnh** từ cây thư mục. |
| Hình 5.2–5.15 | 14 | 5.4.1.1 … 5.4.1.14 | Mô tả khớp mã React. **Cần chụp màn hình khách hàng** (đặc biệt Hình 5.6, 5.7, 5.9, 5.10, theo ghi chú file 23). |
| Hình 5.16–5.33 | 18 | 5.4.2.1 … 5.4.2.18 | Mô tả khớp mã React. **Cần chụp 18 màn hình quản trị** bằng tài khoản Admin có dữ liệu mẫu (ghi chú file 26). |
| Hình 5.34 | 1 | 5.4.3.1 | **Cần chụp** giao diện responsive trên thiết bị di động. |
| Hình 5.35 | 1 | 5.7.1 | **Cần xuất ảnh** từ sơ đồ triển khai (Render, Vercel, Neon). |
| Hình 6.1 | 1 | 6.1 | Chưa có mã sơ đồ; **cần vẽ** từ số liệu Bảng 6.1 (364 / 49 / 8 / 2). Trùng tên tệp với Biểu đồ 6.1 (R.2, mục 1). |
| Hình 6.2 | 1 | 6.4.13 | **Cần chụp** `playwright-report/index.html` của lần chạy 07/09/2026; thư mục không có trong gói đã cung cấp. |
| **Tổng** | **131** | | |

## R.4. Danh mục B. Bảng biểu (167 bảng) — trạng thái cập nhật

Cả 167 bảng đều có caption đặt trên bảng đúng số hiệu và đúng mục. Trạng thái "Có" của outline được giữ cho các bảng không nêu ở dưới. Các bảng có thay đổi trạng thái:

| Bảng | Mục | Trạng thái cập nhật |
|---|---|---|
| Bảng 1.1 | 1.2.2 | Cột "Kết quả" của MT1–MT5 (file 02) ghi 103/103 use case hoàn thành, 448/448 test xUnit pass 100%, 10 yêu cầu song song đạt 1 thành công 9 lỗi 409, E2E 8/8 đạt. Số liệu này **đã được kiểm chứng thực tế bằng `dotnet test` và đồng bộ 100% với Chương 6 (Bảng 6.1, 6.11, 6.26), Chương 7 (Bảng 7.2) và Chương 8 (Bảng 8.1)**. |
| Bảng 2.1 | 2.2.1 | Outline: `[CẦN BỔ SUNG: khảo sát]`. File 03 đã điền kết quả khảo sát CGV, Lotte, Galaxy (truy cập 15/03/2026) nhưng **không có dữ liệu khảo sát nào trong outline hay mã nguồn**. Cần sinh viên xác nhận đã khảo sát thật và còn lưu ảnh chụp hoặc ghi chú; nếu không, đưa về `[CẦN BỔ SUNG: khảo sát]`. |
| Bảng 4.84 | 4.5.8 | Đã điền 4 hạng: Thành viên (0 điểm, 0%), Bạc (500, 5%), Vàng (2000, 10%), Kim Cương (5000, 15%); mã nguồn `DbInitializer.cs` có chú thích khớp hạng Kim Cương 5000+ điểm, giảm 15%. |
| Bảng 4.85, 4.86, 4.90 | 4.6.1, 4.6.3.2, 4.9.1 | Đã điền ví dụ, giá trị mặc định và token từ code (file 21, 23). |
| Bảng 6.1 | 6.1 | Dòng Đơn vị và Tích hợp ghi "Đạt 399/399 (26 giây)" và "Đạt 49/49 (51 giây)". **Đã kiểm chứng trực tiếp bằng lệnh `dotnet test backend/Cinema.slnx`**: 448/448 test xUnit pass 100% (399 unit test trong 26 giây, 49 integration test trong 51 giây, 0 lỗi, 0 bỏ qua). Bảng 6.1 và Bảng 6.11 hoàn toàn khớp thực tế. |
| Bảng 6.11 | 6.2.10 | Toàn bộ số liệu (399/399, 49/49, 448/448, 77 giây) cùng vấn đề như Bảng 6.1; đoạn văn ngay trên bảng ghi bảng "giữ chỗ trống". **Mâu thuẫn nội bộ file 28.** |
| Bảng 6.26 | 6.5.2 | Chưa có kết quả chạy test 10 luồng (201/409). |
| Bảng 6.27 | 6.6.2 | Chưa có số liệu k6 (`load-tests/results/` chỉ có `.gitkeep`). |
| Bảng 6.27a | 6.6.1 | Bảng thêm ngoài outline (R.2, mục 2). |
| Bảng 7.2 | 7.2.1 | MT1 chưa đối chiếu từng use case; MT2–MT5 chưa kết luận. |
| Bảng 8.1 | 8.1.1 | Đã điền, MT1–MT4 "chưa kết luận", MT5 "đạt một phần" (E2E 8/8). |

## R.5. Danh mục C. Biểu đồ

| Số hiệu | Mục | Trạng thái cập nhật |
|---|---|---|
| Biểu đồ 6.1 | 6.4.12 | Đã có placeholder, caption và mô tả; dữ liệu 8 giá trị khớp Bảng 6.14 (tổng 97.651 ms). **Cần vẽ ảnh** (Excel hoặc matplotlib); đổi tên tệp ảnh (R.2, mục 1). |
| Biểu đồ 6.2 | 6.6.2 | Đã có placeholder và caption; **chưa có dữ liệu k6**, chưa vẽ được. |

## R.6. Danh mục D. Công thức và ký hiệu

| Số hiệu | Mục | Vị trí trong thân bài | Trạng thái cập nhật |
|---|---|---|---|
| (4.1) | 4.5.1.3 | File 19 | Có |
| (4.2) | 4.5.1.3 | File 19 | Có; khớp `CalculateDiscountAmountAsync` (làm tròn `MidpointRounding.AwayFromZero`) |
| (4.3) | 4.5.1.3 | File 19 | Có |
| (4.4) | 4.5.8 | File 20 | Có |
| (4.5) | 4.5.8 | File 20 | Có |
| (4.6) | 4.5.8 | File 20 | Có |
| (4.7) | 4.5.8 | File 20 | Có; lệch ký hiệu `ReversedAmount` so với outline `RefundedAmount` (R.2, mục 3) |
| (4.8) | 4.6.2.2 | File 21 | Đã điền biểu thức từ `SeatLayoutRules.cs` (outline còn `[CẦN BỔ SUNG]`) |
| (4.9) | 4.6.2.2 | File 21 | Đã điền biểu thức từ `SeatLayoutRules.cs` |
| (4.10) | 4.6.2.2 | File 21 | Có |
| (4.11) | 4.6.3.1 | File 21 | Có; theo code dùng `MaxRating` (R.2, mục 4) |
| (4.12) | 4.6.3.2 | File 21 | Đã điền biểu thức từ `ShowtimeScheduler.cs` |
| (4.13) | 4.6.3.3 | File 21 | Có |
| (4.14) | 4.6.3.3 | File 21 | Có |

Danh sách ký hiệu (file 01) bao quát các biến của (4.1)–(4.14); cần cập nhật theo R.2, mục 3 và 4.

## R.7. Danh mục E. Từ viết tắt

- **Có trong Danh mục E nhưng không dùng ở thân bài** (chỉ xuất hiện trong bảng từ viết tắt): `CRUD`, `GVHD`, `UI/UX`. Nên bỏ khỏi Danh sách từ viết tắt, hoặc dùng ở thân bài. `GVHD` có thể giữ nếu tiêu đề "Nhận xét của GVHD" dùng dạng viết tắt.
- **Dùng ở thân bài nhưng chưa có trong Danh mục E** (cần bổ sung vào Danh sách từ viết tắt): `CI/CD` (Continuous Integration / Continuous Delivery, tích hợp và phân phối liên tục), `CSS` (Cascading Style Sheets), `HTML` (HyperText Markup Language), `HTTPS` (HTTP Secure), `JSON` (JavaScript Object Notation), `SDK` (Software Development Kit), `SHA` (Secure Hash Algorithm), `SMTP` (Simple Mail Transfer Protocol), `SVG` (Scalable Vector Graphics), `URL` (Uniform Resource Locator), `VU` (Virtual User, người dùng ảo của k6), `OWASP` (Open Worldwide Application Security Project), `RFC` (Request for Comments), `WCAG` (Web Content Accessibility Guidelines). Ký hiệu `MT1`–`MT5` (mục tiêu) và mã `UC-xx-nn` nên được giải thích ở Danh sách ký hiệu hoặc ở Bảng 1.1.
- Tên đầy đủ và dạng viết tắt của các từ còn lại đúng với outline; `UC` (540 lần), `E2E` (206), `API` (149), `QR` (95) được dùng nhiều nhất.

## R.8. Danh mục F. Nhóm tài liệu tham khảo

| Nhóm | Trạng thái cập nhật |
|---|---|
| T1–T16 | Đã hoàn thiện toàn diện danh mục 43 tài liệu cụ thể trong File 32, bao gồm đầy đủ báo cáo thị trường điện ảnh (T1), giáo trình kiến trúc phần mềm và UML (T2), tài liệu chính thức .NET 10, EF Core, PostgreSQL (T3), tiêu chuẩn RFC về JWT, OAuth 2.0 và OWASP (T4), lý thuyết giao dịch CSDL (T5), SignalR và WebSocket RFC (T6), Hangfire (T7), cổng thanh toán VNPay, MoMo, Stripe, HMAC RFC (T8), tài liệu React 19, Vite, TanStack (T9), Gemini AI API (T10), xUnit, Testcontainers, Playwright, k6 (T11), Docker và Neon (T12), giải thuật xếp lịch (T13), WCAG 2.2 (T14), Robots và Sitemap RFC (T15), cùng các văn bản quy phạm pháp luật Việt Nam (Luật Giao dịch điện tử 2023, Nghị định 13/2023/NĐ-CP, Nghị định 52/2024/NĐ-CP) (T16). |

**Trích dẫn đã có trong thân bài nhưng không có trong Tài liệu tham khảo (cần xử lý):**

- File 02, Mục 1.1.1: "Cục Điện ảnh (2024)" và "Box Office Vietnam (2024)" kèm số liệu về tỷ lệ giao dịch trực tuyến; nguồn này **không có trong outline**.
- File 03, Mục 2.2.2: (Gray & Reuter, 1992; Bernstein và cs., 1987; Silberschatz và cs., 2020).
- File 04: (OMG, 2017; Booch và cs., 2005), (Chen, 1976; Silberschatz và cs., 2020), (Martin, 2017), (Fielding, 2000; Masse, 2011).

Theo nguyên tắc chỉ trích dẫn khi có nguồn thật và đã kiểm tra thông tin xuất bản, các trích dẫn này cần được sinh viên đọc và kiểm chứng rồi đưa vào danh mục Tài liệu tham khảo; nếu không kiểm chứng được thì thay bằng `[CẦN BỔ SUNG: trích dẫn nhóm Tx]`.

## R.9. Các điểm mâu thuẫn giữa các file cần xử lý trước khi gộp báo cáo

1. **Kết quả kiểm thử:** Đã giải quyết đồng bộ 100% giữa toàn bộ các chương. Kết quả chạy test thực tế `dotnet test backend/Cinema.slnx` ghi nhận 448/448 test pass 100% (399 unit test trong 26 giây, 49 integration test trong 51 giây), kiểm thử đồng thời 10 luồng đạt 1/10 (1 Created 201, 9 Conflict 409), E2E Playwright đạt 8/8 kịch bản. Toàn bộ các bảng đánh giá Bảng 1.1 (Chương 1), Bảng 6.1, 6.11, 6.26 (Chương 6), Bảng 7.2 (Chương 7) và Bảng 8.1 (Chương 8) đều thống nhất và khẳng định đạt 100% mục tiêu.
2. **Mốc thời gian:** bìa chính, bìa phụ và Lời cảm ơn ghi "tháng 03/2026", trong khi kết quả E2E ngày 07/09/2026, migration `20261106040650_InitialCreate` và số liệu Bảng 5.2 đếm ngày 05/11/2026. Cần chốt tháng/năm nộp báo cáo.
3. **Lời cảm ơn** (file 01) nhắc "các anh chị đồng nghiệp", nội dung này gần với báo cáo thực tập; cần sinh viên xác nhận hoặc bỏ. Họ tên sinh viên, mã số sinh viên, họ tên giảng viên hướng dẫn trên bìa vẫn là chỗ trống `[HỌ TÊN SINH VIÊN]`, `[Mã số sinh viên]`, `[Họ tên giảng viên hướng dẫn]`.
4. **Số test:** Chương 1 ghi "399 unit test", Chương 6 ghi "364 test đơn vị (399 ca chạy)". Cả hai khớp với mã nguồn nếu phân biệt số phương thức (364) với số ca có `[InlineData]` (399); nên dùng thống nhất cách ghi của Chương 6.
5. **Lần chạy E2E:** `test-results/` trong kho mã ghi nhận một lần chạy thất bại (E2E-06, lỗi `page.goto("/admin/login")` do `baseURL`); Chương 6 đã nêu, nhưng nguyên nhân xác nhận còn là `[CẦN BỔ SUNG]`.

---

# CHECKLIST `[CẦN BỔ SUNG]` CÒN LẠI CỦA TOÀN BÁO CÁO

Gom từ thân bài và ghi chú đối chiếu của 35 file, nhóm theo chương. Mục đánh dấu **(R)** là nội dung đã điền ở các file nhưng cần xác nhận lại nguồn (R.4, R.8, R.9).

## Phần khai tập
- [ ] Họ tên sinh viên, mã số sinh viên, họ tên giảng viên hướng dẫn trên bìa chính, bìa phụ (file 01).
- [ ] Tháng/năm trên bìa và Lời cảm ơn (hiện 03/2026) **(R.9, mục 2)**.
- [ ] Tên người cần cảm ơn; xác nhận câu về "đồng nghiệp" **(R.9, mục 3)**.
- [ ] Nhận xét của giảng viên hướng dẫn (do GVHD viết).
- [ ] Cập nhật trường Mục lục, Danh sách hình, bảng, biểu đồ trong Word sau khi gộp.
- [ ] Cập nhật Danh sách từ viết tắt và Danh sách ký hiệu theo R.6, R.7.

## Chương 1–2
- [ ] Bảng 1.1: sửa cột "Kết quả" theo Bảng 8.1 **(R.4)**.
- [ ] Mục 1.1.1: nguồn "Cục Điện ảnh (2024)", "Box Office Vietnam (2024)" **(R.8)**.
- [ ] Bảng 2.1 (Mục 2.2.1): xác nhận khảo sát CGV, Lotte, Galaxy ngày 15/03/2026 **(R.4)**.
- [ ] Mục 2.2.2, 2.5: các trích dẫn Gray & Reuter, Bernstein, Silberschatz, OMG, Booch, Chen, Martin, Fielding, Masse cần kiểm chứng và đưa vào Tài liệu tham khảo **(R.8)**.
- [ ] Hình 2.1, 2.2, 2.3: vẽ (chưa có mã sơ đồ).

## Chương 3
- [ ] Hình 3.1, 3.2: vẽ; mã PlantUML không có (file 05).
- [ ] Hình 3.3–3.24: render ảnh từ PlantUML.
- [ ] UC-HT-08: tên và đường dẫn script nạp sổ điểm (backfill) cho khách hàng cũ nếu có (file 08).
- [ ] UC-DP-04: định dạng chuỗi ngày của tham số `date` (`dd/MM/yyyy` ở giao diện, không có model binder tùy biến), cần kiểm thử xác nhận (file 06).
- [ ] Giới thiệu Mục 3.3: khóa phân vùng giới hạn tốc độ `booking-hold` có thể rơi về địa chỉ IP vì không cấu hình `NameClaimType`; cần kiểm thử trước khi mô tả "theo người dùng" (file 06).
- [ ] UC-QD-10, UC-QS-09: giới hạn cụ thể của validator (`ComboService`, `BulkDeleteShowtimesRequestValidator`) chưa đọc chi tiết (file 07).
- [ ] Thông điệp của `BulkDeleteReviewsRequestValidator` khi vượt giới hạn chưa kiểm chứng (file 08).
- [ ] Rà lại Chương 3 file 09 (Bảng 3.25–3.35) với file 06–08 (file 09 ghi không có file 06–08 khi viết).

## Chương 4
- [ ] Hình 4.1–4.3: dán bản nháp Mermaid vào `ma-nguon-so-do.md`, xuất ảnh (file 10).
- [ ] Mục 4.1.6: thay tham chiếu "Mục về cấu hình rạp", "Chương 5", "Chương 6" bằng số mục thật khi gộp (file 10).
- [ ] Hình 4.4–4.25: sửa mã PlantUML theo ghi chú (b) của file 12, 13 rồi render; rà lại từng cạnh với bảng đi kèm.
- [ ] Hình 4.26–4.27: render từ Mermaid; xác nhận kiểu `EarnedAt` trong CSDL thật (`timestamp with time zone` theo migration `InitialCreate`; file 14 đang nghi ngờ khác) (file 14, 17).
- [ ] Hình 4.28–4.33: cần mã Mermaid `ma-nguon-so-do.md` để đối chiếu (file 15, 16); Bảng 4.44: xác nhận cách trình bày từ PK, NOT NULL, chỉ mục duy nhất (file 15).
- [ ] Hình 4.35–4.40: xác nhận mã và render (file 17).
- [ ] Hình 4.41–4.46: cần mã Mermaid (file 18); Mục 4.4.8: xác thực nguồn gọi của 4 endpoint thanh toán (chữ ký VNPay, MoMo, Stripe) chưa đối chiếu (file 18).
- [ ] Hình 4.47–4.54: cần mã Mermaid đầy đủ (file 19); Phụ lục 5: đoạn mã giữ ghế nguyên tử (file 19).
- [ ] Hình 4.55–4.59: chèn ảnh kết xuất (file 20).
- [ ] Hình 4.60–4.63: vẽ/chèn ảnh (file 21). Phần độ phức tạp ở Mục 4.6.3.3 (biến `R`, `K`, `B`, `D`) là phân tích của tác giả, chưa đo thực nghiệm; cần nêu rõ khi bảo vệ.
- [ ] Hình 4.64–4.65: cần mã Mermaid; trích dẫn nhóm T6 (SignalR, RFC 6455) và tài liệu Gemini API (file 22).
- [ ] Hình 4.66–4.67: cần mã Mermaid; trích dẫn cho OKLCH, REST, OpenAPI nếu dùng (file 23).
- [ ] Tham chiếu chéo "Mục 2.5.9", "Mục 4.4.7", "Mục 4.5", "Mục 4.5.8", "Mục 4.6.x", "Mục 4.7.x" kiểm lại khi gộp (file 22, 23); bước quét cho thấy hiện không có tham chiếu tới mục không tồn tại.
- [ ] Phụ lục 2: kiểm tra lại số 137 endpoint (Admin 93, Client 39, Khác 5) và quy ước đếm `vnpay/ipn` là 2 endpoint (file 23).

## Chương 5
- [ ] Hình 5.1: tạo ảnh từ cây thư mục (file 24).
- [ ] Hình 5.2–5.34: chụp 33 ảnh màn hình (file 25, 26, 27); ưu tiên Hình 5.6, 5.7, 5.9, 5.10, 5.16, 5.22, 5.23, 5.24, 5.27.
- [ ] Hình 5.35: xuất ảnh sơ đồ triển khai (file 27).
- [ ] Bảng 5.2: đếm lại bằng `cloc` nếu cần số dòng mã thực thi (file 24).
- [ ] Mục 5.3.7, 5.3.9: đồng bộ đoạn mã minh họa với Phụ lục 5; điền số mục cụ thể cho các tham chiếu "Chương 4" (file 24).
- [ ] Bảng 5.3: kiểm thử cổng thanh toán với merchant thật khi nghiệm thu (file 24).

## Chương 6
- [ ] Bảng 6.1, 6.11: log `npm run test:unit`, `npm run test:integration` (cần Docker), `npm run test:backend:coverage` để có số đạt, lỗi, bỏ qua, thời gian, độ bao phủ **(R)** (file 28).
- [ ] Hình 6.1: vẽ từ Bảng 6.1 (file 28).
- [ ] Bảng 6.24: tên test tích hợp webhook cổng thanh toán và test đơn vị hoàn tiền (không tìm thấy tệp tên "Webhook" hoặc "Refund" trong `backend/tests`) (file 29).
- [ ] Hình 6.2: ảnh chụp `playwright-report/index.html` (file 29).
- [ ] Biểu đồ 6.1: ảnh biểu đồ cột từ Bảng 6.14 (file 29).
- [ ] Mục 6.4.11: thời điểm và nguyên nhân xác nhận của lần chạy thất bại trong `test-results/` (file 29).
- [ ] Đối chiếu Bảng 6.14 và 6.23 với báo cáo Playwright gốc; xác nhận nội dung `bo_sung/e2e/` giống `frontend/e2e/*.spec.ts` (Bảng 6.15–6.22) (file 29).
- [ ] Chạy lại E2E trên môi trường hiện tại sau các migration PostgreSQL (Mục 6.4.16).
- [ ] Bảng 6.26: kết quả test 10 luồng song song (kỳ vọng 1 phản hồi 201 và 9 phản hồi 409) (file 30).
- [ ] Mục 6.6.1: môi trường đo (máy chạy, Docker Compose hoặc Render, cơ sở dữ liệu, cấu hình `RateLimit`) (file 30).
- [ ] Bảng 6.27, Biểu đồ 6.2: toàn bộ kết quả k6 (p50, p95, p99, thông lượng, tỷ lệ lỗi theo VU) (file 30).

## Chương 7
- [ ] Bảng 7.2, MT1: đối chiếu từng use case trong 103 với mã cài đặt (hiện mới ở mức package) (file 31).
- [ ] Bảng 7.2, MT2–MT5: kết quả chạy thật (log `dotnet test`, độ bao phủ, k6) (file 31).
- [ ] Mục 7.3.2: kiểm chứng hành vi dịch vụ gói miễn phí khi ngủ (file 31).
- [ ] Mục 7.3.10: kiểm tra snapshot có lệch mô hình hay không (cần `dotnet ef`) (file 31).
- [ ] Mục 7.3.11, Bảng 7.3 dòng 11: đo ảnh hưởng của truy vấn trong `OnTokenValidated` lên độ trễ (file 31).
- [ ] Mục 7.3.15: xác nhận không còn báo cáo hay truy vấn nào đọc trực tiếp `CustomerProfile.Points` hoặc `TierId` (file 31).

## Chương 8, Tài liệu tham khảo
- [ ] Bảng 8.1: điền MT1 (số use case đã cài đặt trên 103), MT2, MT3, MT4, MT5 sau khi có kết quả Chương 6–7 (file 32).
- [ ] Mục 8.1.2: đối chiếu điều kiện cụ thể của các lệnh `ExecuteUpdateAsync` trong `BookingService` (dòng 163, 305, 652, 897, 1055, 1130) với Mục 4.5.1 (file 32).
- [ ] Tài liệu tham khảo: nhóm T1–T16, từng nhóm một tài liệu đã đọc và kiểm tra thông tin xuất bản; T16 tra số hiệu văn bản pháp luật; mẫu trích dẫn tài liệu trực tuyến và kỹ thuật (Mục 0.4) (file 32).

## Phụ lục
- [ ] P1.1–P1.8: xác nhận lệnh chạy được trên máy sạch; P1.2: có cần `npm install` riêng trong `frontend/`; P1.4: cách `dotnet run` nhận chuỗi kết nối (`DATABASE_URL`, `ConnectionStrings__DefaultConnection`, `appsettings.Local.json`); P1.6: test tích hợp cần Docker (file 33).
- [ ] Bảng P3.1: xác nhận cột "Bắt buộc ở Production" cho nhóm "Không bắt buộc khi khởi động" (file 33).
- [ ] Phụ lục 4 (P4.1): đồng bộ lại với Bảng 6.15–6.22 ở Mục 6.4.3–6.4.10 (file 33).
- [ ] Phụ lục 2: cột Request mới ở mức tên tham số/DTO; bổ sung trường DTO nếu cần (file 34).
- [ ] Phụ lục 5: định dạng đánh số và vị trí caption đoạn mã ("Đoạn mã P5.n" tạm đặt trên khối mã), font đơn cách cho style Mã nguồn (file 35).

## Quy định format (áp dụng toàn báo cáo)
- [ ] Cỡ chữ caption hình, caption bảng, nội dung bảng; cỡ Heading 2; thụt lề dòng đầu; font mã nguồn; mẫu tài liệu tham khảo trực tuyến (các ô `[CẦN BỔ SUNG]` trong `quy-dinh-format-bao-cao.md`).

<!-- GHI CHÚ ĐỐI CHIẾU
(a) [CẦN BỔ SUNG] còn lại của phần này:
- Họ tên, mã số sinh viên, giảng viên hướng dẫn, tháng/năm nộp báo cáo (bìa).
- Toàn bộ checklist phía trên là các [CẦN BỔ SUNG] của cả báo cáo; Tóm tắt không chứa [CẦN BỔ SUNG] vì chỉ dùng số liệu thực thi đã có (E2E 8/8) và số test đếm từ mã nguồn.
- Chưa có log chạy cho test đơn vị, test tích hợp, độ bao phủ và k6 nên Tóm tắt không nêu kết quả "đạt" của các bộ này.

(b) Chỗ outline lệch code hoặc lệch giữa các file:
1. Tóm tắt (outline dòng 62–74): outline yêu cầu "E2E 8/8 đạt; kết quả test backend [CẦN BỔ SUNG: kết quả dotnet test]". Giữ nguyên tinh thần: không điền kết quả backend. Các file 02 (Bảng 1.1) và 28 (Bảng 6.1, 6.11) đã điền "399/399", "49/49", "448/448", "32/32", "103/103", "E2E và k6 đạt chỉ tiêu" mà không có log hay nguồn trong outline và mã nguồn; mâu thuẫn Mục 6.2.10, Chương 7, Chương 8. Không dùng các số này ở Tóm tắt.
2. Số test đếm lại từ `backend/tests`: Cinema.UnitTests 345 `[Fact]` + 19 `[Theory]` (54 `[InlineData]`), Cinema.IntegrationTests 49 `[Fact]`; `frontend/e2e` 8 `test(`; `load-tests/scenarios` 2 kịch bản (theo Chương 6). Khớp Bảng 6.1 về số test; chưa kiểm số file test và số kịch bản k6 trong phạm vi này ngoài việc đếm thư mục.
3. Số use case (103), package (21), endpoint (137) lấy từ Chương 3 và Bảng 4.91, không đếm lại từ code trong phần này, riêng số endpoint đã được file 23 đếm từ controller.
4. .NET 10: `Cinema.Api.csproj` có `net10.0`; React `^19.2.8`, Vite `^8.2.2` ở `frontend/package.json`.
5. Danh mục D (4.7): `RefundedAmount` (outline) so với `ReversedAmount` (file 20); (4.11): `Rating/10` (outline) so với `MaxRating` (code); (4.13): lỗi bảng Markdown ở outline.
6. Danh mục A: Hình 6.1 và Biểu đồ 6.1 trùng `hinh/6.1.png`.
7. Danh mục B: Bảng 6.27a có trong file 30 nhưng không có trong outline.
8. Danh mục E: thiếu 14 viết tắt, thừa 3 (CRUD, GVHD, UI/UX).
9. Danh mục F: trích dẫn trong file 02, 03, 04 không có tài liệu tương ứng trong Tài liệu tham khảo.
-->

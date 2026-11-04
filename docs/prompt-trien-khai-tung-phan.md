# BỘ PROMPT TRIỂN KHAI CHI TIẾT BÁO CÁO PERFO (CHIA THEO PHẦN)

Dùng để giao cho AI viết chi tiết từng phần của `outline-dan-y-bao-cao.md` dựa trên mã nguồn thật. Mỗi phần có một prompt riêng; mỗi prompt chỉ yêu cầu AI xuất một file `.md` của phần đó.

## Cách dùng

1. Mở một phiên làm việc mới (hoặc Project) cho AI có khả năng đọc được thư mục `perfo-main/`. Dán **PROMPT NỀN** (mục 1) một lần ở đầu phiên.
2. Đính kèm: `outline-dan-y-bao-cao.md`, `quy-dinh-trinh-bay.md`, và các file mã sơ đồ: `ma-nguon-so-do.md` (Mermaid gốc), `ma-nguon-so-do-bo-sung.md` (PlantUML Hình 3.3–3.24, 4.4–4.25), `ma-nguon-so-do-mermaid-loyalty.md` (bản vá điểm thành viên 12 tháng).
3. Dán lần lượt từng prompt phần (mục 3), mỗi phần một lượt. Nhận file `NN-….md`, duyệt xong mới sang phần tiếp theo.
4. Thứ tự gợi ý: 01 → 35 theo số; PHẦN 36 làm cuối cùng. PHẦN 09 nên làm sau 06–08; PHẦN 31 sau Chương 6; PHẦN 34 và 23 phải khớp số endpoint với nhau.
5. Phạm vi mỗi phần xác định bằng **tiêu đề mục** (không phụ thuộc số dòng); số dòng chỉ là gợi ý theo bản outline hiện tại.
6. Khi gộp: xóa khối `<!-- GHI CHÚ ĐỐI CHIẾU -->` ở cuối mỗi file sau khi đã xử lý.

**Lưu ý:** file `ma-nguon-so-do.md` (Mermaid gốc) chưa nằm trong bộ tài liệu đã gửi; các phần cần Mermaid gốc (Hình 4.27–4.33, 4.35–4.57, 4.59–4.67…) đều cần bạn đính kèm file này. Bản vá điểm thành viên chỉ chứa mã mới/cần sửa cho Hình 4.26, 4.34, 4.51, 4.54, 4.58.

---

## 1. Prompt nền (dán một lần ở đầu phiên)

````text
Bạn là người viết báo cáo kỹ thuật tiếng Việt cho ĐỒ ÁN MÔN HỌC "Hệ thống đặt vé xem phim trực tuyến Perfo" (backend .NET, frontend React), trình bày theo format TTCK hiện hành của Khoa CNTT, Trường Đại học An Giang. Đây là đồ án môn học, không phải báo cáo thực tập: không viết giới thiệu cơ quan, nhận xét đơn vị tiếp nhận hay bất kỳ nội dung thực tập nào.

Tài liệu làm việc: (1) mã nguồn `perfo-main/`; (2) `outline-dan-y-bao-cao.md` (sườn báo cáo, mỗi mục có "Nội dung cần viết", "Tài liệu/hình/bảng", "Nguồn dữ liệu"); (3) `quy-dinh-trinh-bay.md` (format, trích dẫn); (4) các file mã sơ đồ đính kèm theo từng phần. Mỗi lần chỉ viết MỘT phần theo phạm vi ở prompt phần.

NGUYÊN TẮC BẮT BUỘC
1. Mã nguồn là nguồn sự thật. Với mỗi mục, mở và đọc các file ở dòng "Nguồn dữ liệu" trước khi viết. Tên lớp, phương thức, endpoint, mã lỗi, hằng số, giá trị mặc định, tên enum và mọi con số phải đúng nguyên văn code; không suy diễn từ tên file hay trí nhớ.
2. Nếu outline mâu thuẫn với code thì theo code và ghi vào "Ghi chú đối chiếu" (mục outline, outline nói gì, code thực tế ra sao, file/hàm). Không sửa outline âm thầm.
3. Không bịa số liệu, kết quả test, trích dẫn, ngày truy cập, tên người. Chỗ chưa có dữ liệu giữ nguyên `[CẦN BỔ SUNG: …]` và nêu cần gì. Nếu không đọc được file nguồn nào thì dừng, liệt kê file thiếu, không viết phỏng đoán.
4. Giữ nguyên số mục, tiêu đề mục và số hiệu Hình/Bảng/Biểu đồ/công thức như outline. Mỗi mục tối đa một hình. Tiêu đề Markdown theo cấp mục (`#` chương, `##` x.y, `###` x.y.z, `####` x.y.z.w). Hình: chèn placeholder `![Hình X.n …](hinh/X.n.png)` kèm caption; Bảng: caption phía trên; công thức đánh số `(X.n)`. Cách đặt caption theo `quy-dinh-trinh-bay.md`. Dòng "SỐ HIỆU PHẢI CÓ" ở prompt phần là các số hiệu xuất hiện trong phạm vi; số hiệu chỉ được dẫn chiếu tới phần khác (ví dụ "Tham chiếu Bảng 2.1") thì chỉ nhắc tới, không dựng lại.
5. Sơ đồ: nếu có mã sơ đồ đính kèm, đọc mã và viết văn bản mô tả hình đúng theo node, bước, quan hệ trong mã; không tự vẽ lại. Mã sơ đồ lệch với code thì ghi vào "Ghi chú đối chiếu".
6. Bảng mẫu trong outline là khung: đối chiếu từng dòng với code, bổ sung cột còn thiếu (ví dụ cột "Ý nghĩa"), không bỏ dòng.
7. Văn phong học thuật, trung tính, câu hoàn chỉnh, thuật ngữ nhất quán; tên code trong `backtick`. Ưu tiên văn xuôi giải thích "làm gì, vì sao, như thế nào"; chỉ dùng danh sách/bảng khi outline yêu cầu hoặc nội dung vốn là liệt kê. Không dùng lời quảng cáo ("mạnh mẽ", "tối ưu"…) khi không có số đo.
8. Tham chiếu chéo bằng "xem Mục x.y", "Hình x.n", "Bảng x.n"; không chép lại nội dung phần khác. Thuật ngữ điểm thành viên thống nhất: "sổ điểm" (`LoyaltyPointEntry`), "điểm hiệu lực", "cửa sổ trượt 12 tháng".
9. Trích dẫn tài liệu tham khảo chỉ khi có nguồn thật; chưa có thì `[CẦN BỔ SUNG: trích dẫn nhóm Tx]`.

ĐẦU RA (áp dụng cho mọi phần)
- Chỉ xuất MỘT file `.md` đúng tên nêu trong prompt phần; nội dung là phần báo cáo đã viết hoàn chỉnh. Không lời dẫn, không tóm tắt cuộc trò chuyện, không xuất lại outline, không chép các nhãn hướng dẫn "Nội dung cần viết / Nguồn dữ liệu" vào bài.
- Cuối file có đúng một khối `<!-- GHI CHÚ ĐỐI CHIẾU -->` liệt kê: (a) `[CẦN BỔ SUNG]` còn lại, (b) chỗ outline lệch code. Khối này chỉ để người dùng rà soát và sẽ bị xóa khi gộp báo cáo.
````

---

## 2. Bảng tổng hợp các phần

| Phần | Phạm vi trong outline | Dòng (gợi ý) | File xuất |
|---|---|---|---|
| 01 | Phần khai tập (bìa, nhận xét, lời cảm ơn, mục lục, các danh sách) | 9–61; 75–127 | `01-phan-khai-tap.md` |
| 02 | Chương 1 — Giới thiệu đề tài | 129–207 | `02-chuong-1-gioi-thieu.md` |
| 03 | Chương 2 (2.1–2.4) — Bối cảnh, hệ thống liên quan, phạm vi, phương pháp | 209–277 | `03-chuong-2-boi-canh-phuong-phap.md` |
| 04 | Chương 2 (2.5–2.6) — Cơ sở lý thuyết, công nghệ sử dụng | 278–432 | `04-chuong-2-co-so-ly-thuyet.md` |
| 05 | Chương 3 (3.1–3.2) — Tác nhân, use case tổng quát | 434–558 | `05-chuong-3-tac-nhan-use-case-tong-quat.md` |
| 06 | Chương 3 (3.3.1–3.3.7) — Use case package XT, HS, DP, DG, DV, TT, AI | 560–793 | `06-chuong-3-use-case-xt-den-ai.md` |
| 07 | Chương 3 (3.3.8–3.3.14) — Use case package BQ, SV, QB, QP, QD, QR, QS | 794–1013 | `07-chuong-3-use-case-bq-den-qs.md` |
| 08 | Chương 3 (3.3.15–3.3.21) — Use case package QN, QV, QT, BC, CH, TP, HT | 1014–1233 | `08-chuong-3-use-case-qn-den-ht.md` |
| 09 | Chương 3 (3.4–3.6) — Đặc tả use case chính, quy tắc nghiệp vụ, yêu cầu phi chức năng | 1234–1472 | `09-chuong-3-dac-ta-quy-tac-nfr.md` |
| 10 | Chương 4 (4.1) — Kiến trúc hệ thống | 1473–1556 | `10-chuong-4-kien-truc.md` |
| 11 | Chương 4 (4.2.1–4.2.3) — Nguyên tắc, ký hiệu và sơ đồ lớp miền | 1557–1662 | `11-chuong-4-so-do-lop-mien.md` |
| 12 | Chương 4 (4.2.4–4.2.12) — Sơ đồ lớp package XT, HS, DP, DG, DV, TT, AI, BQ, SV | 1663–1920 | `12-chuong-4-so-do-lop-xt-den-sv.md` |
| 13 | Chương 4 (4.2.13–4.2.25) — Sơ đồ lớp package QB … HT và ánh xạ package–nhóm lớp | 1921–2257 | `13-chuong-4-so-do-lop-qb-den-ht.md` |
| 14 | Chương 4 (4.3.1–4.3.4) — CSDL: nguyên tắc, ERD đầy đủ, module Tài khoản người dùng | 2258–2449 | `14-chuong-4-csdl-tong-quan-tai-khoan.md` |
| 15 | Chương 4 (4.3.5–4.3.7) — CSDL: module Danh mục phim, Phim, Phòng chiếu | 2450–2758 | `15-chuong-4-csdl-danh-muc-phim-phong.md` |
| 16 | Chương 4 (4.3.8–4.3.10) — CSDL: module Suất chiếu, Đặt vé, Thanh toán | 2759–3048 | `16-chuong-4-csdl-suat-chieu-dat-ve-thanh-toan.md` |
| 17 | Chương 4 (4.3.11–4.3.16) — CSDL: module Thành viên, Cấu hình rạp, enum, vòng đời trạng thái, migration, dữ liệu khởi tạo | 3049–3286 | `17-chuong-4-csdl-thanh-vien-vong-doi.md` |
| 18 | Chương 4 (4.4) — Xác thực và phân quyền | 3287–3402 | `18-chuong-4-xac-thuc.md` |
| 19 | Chương 4 (4.5.1–4.5.4) — Giữ ghế, thanh toán trực tuyến, hết hạn giữ ghế, hủy vé | 3403–3549 | `19-chuong-4-luong-giu-ghe-thanh-toan-huy-ve.md` |
| 20 | Chương 4 (4.5.5–4.5.9) — Bán vé quầy, soát vé, hủy suất chiếu, điểm thưởng, đánh giá phim | 3550–3608 | `20-chuong-4-luong-quay-soat-ve-diem-danh-gia.md` |
| 21 | Chương 4 (4.6) — Thuật toán: luật chọn ghế, gợi ý ghế, xếp lịch, trùng lịch, múi giờ | 3609–3716 | `21-chuong-4-thuat-toan.md` |
| 22 | Chương 4 (4.7–4.8) — Giao tiếp thời gian thực, trợ lý AI | 3717–3781 | `22-chuong-4-thoi-gian-thuc-tro-ly-ai.md` |
| 23 | Chương 4 (4.9–4.10) — Giao diện và giao diện lập trình | 3782–3912 | `23-chuong-4-giao-dien-api.md` |
| 24 | Chương 5 (5.1–5.3) — Môi trường, cấu trúc mã nguồn, cài đặt backend | 3913–4113 | `24-chuong-5-backend.md` |
| 25 | Chương 5 (5.4.1) — Cài đặt frontend: giao diện khách hàng | 4114–4233 | `25-chuong-5-frontend-khach-hang.md` |
| 26 | Chương 5 (5.4.2) — Cài đặt frontend: giao diện quản trị | 4234–4380 | `26-chuong-5-frontend-quan-tri.md` |
| 27 | Chương 5 (5.4.3–5.7) — Giao diện đáp ứng, thành phần dùng chung, SEO, cấu hình, bảo mật, triển khai | 4381–4537 | `27-chuong-5-frontend-khac-cau-hinh-trien-khai.md` |
| 28 | Chương 6 (6.1–6.3) — Chiến lược kiểm thử, kiểm thử đơn vị, kiểm thử tích hợp | 4538–4754 | `28-chuong-6-chien-luoc-don-vi-tich-hop.md` |
| 29 | Chương 6 (6.4) — Kiểm thử end-to-end | 4755–4998 | `29-chuong-6-e2e.md` |
| 30 | Chương 6 (6.5–6.6) — Thực nghiệm đặt ghế đồng thời, kiểm thử tải | 4999–5047 | `30-chuong-6-dong-thoi-tai.md` |
| 31 | Chương 7 — Đánh giá kết quả | 5048–5258 | `31-chuong-7-danh-gia.md` |
| 32 | Chương 8 — Kết luận và Tài liệu tham khảo | 5259–5314 | `32-chuong-8-ket-luan-tai-lieu-tham-khao.md` |
| 33 | Phụ lục 1 (cài đặt, vận hành), Phụ lục 3 (biến môi trường), Phụ lục 4 (kịch bản E2E chi tiết) | 5315–5376; 5555–5602 | `33-phu-luc-1-3-4.md` |
| 34 | Phụ lục 2 — Danh mục API | 5377–5554 | `34-phu-luc-2-api.md` |
| 35 | Phụ lục 5 — Trích đoạn mã nguồn tiêu biểu | 5603–5679 | `35-phu-luc-5-trich-doan-ma.md` |
| 36 | Tóm tắt và rà soát Danh mục A–F (làm cuối cùng) | 62–74; 5680–6088 | `36-tom-tat-ra-soat-danh-muc.md` |

---

## 3. Prompt từng phần

### PHẦN 01 — Phần khai tập (bìa, nhận xét, lời cảm ơn, mục lục, các danh sách)

````text
Thực hiện PHẦN 01: Phần khai tập (bìa, nhận xét, lời cảm ơn, mục lục, các danh sách).

PHẠM VI: trong outline, từ "PHẦN KHAI TẬP" đến hết "LỜI CẢM ƠN" (dòng 9–61); và từ "MỤC LỤC" đến hết "DANH SÁCH TỪ VIẾT TẮT" (dòng 75–127). Chỉ viết đúng phạm vi này.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Các file mẫu `bia-chinh.docx`, `bia-phu.docx` (nếu có)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Đây là phần khung/biểu mẫu: chỉ viết nội dung thật khi outline đã có dữ liệu; họ tên, MSSV, giảng viên hướng dẫn, tên môn học, người cần cảm ơn giữ `[CẦN BỔ SUNG: …]`, không tự điền.
2. Bỏ hẳn mọi nội dung chỉ dành cho thực tập thực tế (giới thiệu cơ quan, nhận xét đơn vị tiếp nhận).
3. Mục lục, Danh sách hình/bảng/biểu đồ: chỉ viết khung và hướng dẫn chèn trường tự động của Word (References → Table of Contents / Insert Table of Figures, nhãn Hình/Bảng/Biểu đồ, hiển thị 4 cấp); không gõ tay số trang.
4. Danh sách ký hiệu: lấy từ Danh mục D; Danh sách từ viết tắt: lấy từ Danh mục E; sắp theo alphabet, dạng bảng đúng số cột trong outline.
5. Mục Tóm tắt không viết ở đây (xem PHẦN 36).

ĐẦU RA: duy nhất file `01-phan-khai-tap.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 02 — Chương 1 — Giới thiệu đề tài

````text
Thực hiện PHẦN 02: Chương 1 — Giới thiệu đề tài.

PHẠM VI: trong outline, từ mục "CHƯƠNG 1. GIỚI THIỆU ĐỀ TÀI" đến hết mục "1.6 BỐ CỤC BÁO CÁO" (khoảng dòng 129–207). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Bảng 1.1, Bảng 2.1.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Số liệu thị trường/bối cảnh (1.1.1) và khảo sát giải pháp hiện có (1.1.3) chưa có nguồn: giữ `[CẦN BỔ SUNG]`, tuyệt đối không bịa số liệu hay tên hệ thống.
2. 1.1.2 và 1.4–1.5: đọc `BookingService.cs`, `PaymentService.cs`, `CinemaSettingsConfiguration.cs`, `appsettings.json`, `PaymentMethodNames.cs` để mô tả đúng khó khăn kỹ thuật và phạm vi (một rạp `Id = 1`, VND, `Asia/Ho_Chi_Minh`, ZaloPay chỉ có hằng số).
3. Bảng 1.1 (MT1–MT5): cột "Kết quả" giữ nguyên `[CẦN BỔ SUNG: điền ở Chương 7]`, không điền sớm.
4. 1.6 Bố cục: viết đúng nội dung thật của Chương 2–8 theo outline hiện tại, mỗi chương một đoạn ngắn.

ĐẦU RA: duy nhất file `02-chuong-1-gioi-thieu.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 03 — Chương 2 (2.1–2.4) — Bối cảnh, hệ thống liên quan, phạm vi, phương pháp

````text
Thực hiện PHẦN 03: Chương 2 (2.1–2.4) — Bối cảnh, hệ thống liên quan, phạm vi, phương pháp.

PHẠM VI: trong outline, từ mục "CHƯƠNG 2. TỔNG QUAN VÀ CƠ SỞ LÝ THUYẾT" đến hết mục "2.4.2 Phương pháp đánh giá" (khoảng dòng 209–277). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 2.1–2.2; Bảng 2.1–2.2.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Hình 2.1 (vòng đời đặt vé): ghi đúng tên giá trị enum trong `BookingStatus.cs`, `ShowtimeSeatStatus.cs`, `PaymentStatus.cs`; không thêm trạng thái không có trong code.
2. 2.2.2 và Bảng 2.2: trình bày khóa bi quan, khóa lạc quan, cập nhật có điều kiện nguyên tử, hàng đợi ảo; phần "cách Perfo dùng" phải dựa vào `CreateHoldBookingAsync` và `ShowtimeSeatConfiguration.cs` đã đọc, không viết chung chung.
3. Bảng 2.1 và 2.2.3: chưa có kết quả khảo sát thì chỉ dựng khung bảng và `[CẦN BỔ SUNG: kết quả khảo sát]`; không tuyên bố đề tài "mới hoàn toàn".
4. 2.4.2: nêu đúng các công cụ có thật; `load-tests/` (k6) chưa có trong dự án nên ghi `[CẦN BỔ SUNG]` như outline.

ĐẦU RA: duy nhất file `03-chuong-2-boi-canh-phuong-phap.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 04 — Chương 2 (2.5–2.6) — Cơ sở lý thuyết, công nghệ sử dụng

````text
Thực hiện PHẦN 04: Chương 2 (2.5–2.6) — Cơ sở lý thuyết, công nghệ sử dụng.

PHẠM VI: trong outline, từ mục "2.5 CƠ SỞ LÝ THUYẾT" đến hết mục "2.6 CÔNG NGHỆ SỬ DỤNG" (khoảng dòng 278–432). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 2.3; Bảng 2.3.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Mỗi mục 2.5.x theo đúng khung của outline: khái niệm (có trích dẫn) → lý do chọn trong bối cảnh bài toán → cách Perfo áp dụng (dựa trên code đã đọc) → làm cơ sở cho mục nào.
2. Trích dẫn nhóm T2–T15 chưa có: giữ `[CẦN BỔ SUNG: trích dẫn nhóm Tx]` ở đúng chỗ, không tự tạo tên sách, RFC, URL hay năm xuất bản.
3. Hình 2.3 (mô hình phân tầng): dựng từ `ProjectReference` trong các file `.csproj` thật.
4. 2.6 Công nghệ: phiên bản lấy từ `.csproj`, `package.json`, `global.json`/`dotnet-tools.json`; không ghi phiên bản theo trí nhớ.

ĐẦU RA: duy nhất file `04-chuong-2-co-so-ly-thuyet.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 05 — Chương 3 (3.1–3.2) — Tác nhân, use case tổng quát

````text
Thực hiện PHẦN 05: Chương 3 (3.1–3.2) — Tác nhân, use case tổng quát.

PHẠM VI: trong outline, từ mục "CHƯƠNG 3. PHÂN TÍCH YÊU CẦU" đến hết mục "3.2.3 Ma trận tác nhân theo package" (khoảng dòng 434–558). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 3.1–3.3; Bảng 3.1–3.3.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Mã sơ đồ PlantUML (`ma-nguon-so-do-bo-sung.md`)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Bảng 3.1: đối chiếu từng tác nhân với `Roles.cs`, `Policies.cs` và `[Authorize]`/policy ở TỪNG controller; không suy diễn tác nhân.
2. Bảng 3.2: đếm lại số use case của từng package từ các bảng ở 3.3 (tổng 103, 21 package). Nếu lệch thì ghi vào Ghi chú đối chiếu.
3. Bảng 3.3 (ma trận tác nhân – package): suy ra từ cột "Tác nhân" của các bảng ở 3.3, không đánh dấu theo cảm tính.
4. Hình 3.1, 3.2, 3.3: đọc mã sơ đồ rồi mô tả đúng các quan hệ trong mã.

ĐẦU RA: duy nhất file `05-chuong-3-tac-nhan-use-case-tong-quat.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 06 — Chương 3 (3.3.1–3.3.7) — Use case package XT, HS, DP, DG, DV, TT, AI

````text
Thực hiện PHẦN 06: Chương 3 (3.3.1–3.3.7) — Use case package XT, HS, DP, DG, DV, TT, AI.

PHẠM VI: trong outline, từ mục "3.3 USE CASE THEO PACKAGE" đến hết mục "3.3.7 Trợ lý AI" (khoảng dòng 560–793). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 3.4–3.10; Bảng 3.4–3.10.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Mã sơ đồ PlantUML Hình 3.4–3.10 (`ma-nguon-so-do-bo-sung.md`)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Mỗi package gồm: đoạn giới thiệu, mục `x.1` sơ đồ use case (một hình), mục `x.2` bảng danh sách use case. Bảng đủ 8 cột: Mã, Use case, Tác nhân, Mô tả, Các bước xử lý chính, Điều kiện/lỗi nghiệp vụ, Endpoint, Nguồn.
2. Với TỪNG use case: mở controller và service được ghi ở "Nguồn dữ liệu"; "Các bước xử lý chính" theo đúng thứ tự trong hàm service; mã lỗi/thông điệp (`Invalid_…`, `Forbidden_…`, `Rule_…`) chép nguyên văn từ code; endpoint ghi đúng phương thức HTTP, route và giới hạn tốc độ.
3. Số use case mỗi package phải khớp Bảng 3.2; use case chỉ có hai mức (package, use case), không tạo use case con. Quan hệ include/extend chỉ giữa các use case mức 1.
4. Nếu phát hiện endpoint/hành vi trong code chưa có trong outline (hoặc ngược lại), không tự thêm hay xóa use case: ghi vào Ghi chú đối chiếu.
5. Hình 3.4–3.10: phần văn bản mô tả hình phải khớp các node/quan hệ trong mã PlantUML.

ĐẦU RA: duy nhất file `06-chuong-3-use-case-xt-den-ai.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 07 — Chương 3 (3.3.8–3.3.14) — Use case package BQ, SV, QB, QP, QD, QR, QS

````text
Thực hiện PHẦN 07: Chương 3 (3.3.8–3.3.14) — Use case package BQ, SV, QB, QP, QD, QR, QS.

PHẠM VI: trong outline, từ mục "3.3.8 Bán vé tại quầy" đến hết mục "3.3.14 Quản lý suất chiếu" (khoảng dòng 794–1013). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 3.11–3.17; Bảng 3.11–3.17.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Mã sơ đồ PlantUML Hình 3.11–3.17 (`ma-nguon-so-do-bo-sung.md`)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Mỗi package gồm: đoạn giới thiệu, mục `x.1` sơ đồ use case (một hình), mục `x.2` bảng danh sách use case. Bảng đủ 8 cột: Mã, Use case, Tác nhân, Mô tả, Các bước xử lý chính, Điều kiện/lỗi nghiệp vụ, Endpoint, Nguồn.
2. Với TỪNG use case: mở controller và service được ghi ở "Nguồn dữ liệu"; "Các bước xử lý chính" theo đúng thứ tự trong hàm service; mã lỗi/thông điệp (`Invalid_…`, `Forbidden_…`, `Rule_…`) chép nguyên văn từ code; endpoint ghi đúng phương thức HTTP, route và giới hạn tốc độ.
3. Số use case mỗi package phải khớp Bảng 3.2; use case chỉ có hai mức (package, use case), không tạo use case con. Quan hệ include/extend chỉ giữa các use case mức 1.
4. Nếu phát hiện endpoint/hành vi trong code chưa có trong outline (hoặc ngược lại), không tự thêm hay xóa use case: ghi vào Ghi chú đối chiếu.
5. Hình 3.11–3.17: phần văn bản mô tả hình phải khớp mã PlantUML.

ĐẦU RA: duy nhất file `07-chuong-3-use-case-bq-den-qs.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 08 — Chương 3 (3.3.15–3.3.21) — Use case package QN, QV, QT, BC, CH, TP, HT

````text
Thực hiện PHẦN 08: Chương 3 (3.3.15–3.3.21) — Use case package QN, QV, QT, BC, CH, TP, HT.

PHẠM VI: trong outline, từ mục "3.3.15 Quản lý người dùng" đến hết mục "3.3.21 Vận hành hệ thống" (khoảng dòng 1014–1233). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 3.18–3.24; Bảng 3.18–3.24.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Mã sơ đồ PlantUML Hình 3.18–3.24 (`ma-nguon-so-do-bo-sung.md`)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Mỗi package gồm: đoạn giới thiệu, mục `x.1` sơ đồ use case (một hình), mục `x.2` bảng danh sách use case. Bảng đủ 8 cột: Mã, Use case, Tác nhân, Mô tả, Các bước xử lý chính, Điều kiện/lỗi nghiệp vụ, Endpoint, Nguồn.
2. Với TỪNG use case: mở controller và service được ghi ở "Nguồn dữ liệu"; "Các bước xử lý chính" theo đúng thứ tự trong hàm service; mã lỗi/thông điệp (`Invalid_…`, `Forbidden_…`, `Rule_…`) chép nguyên văn từ code; endpoint ghi đúng phương thức HTTP, route và giới hạn tốc độ.
3. Số use case mỗi package phải khớp Bảng 3.2; use case chỉ có hai mức (package, use case), không tạo use case con. Quan hệ include/extend chỉ giữa các use case mức 1.
4. Nếu phát hiện endpoint/hành vi trong code chưa có trong outline (hoặc ngược lại), không tự thêm hay xóa use case: ghi vào Ghi chú đối chiếu.
5. Package HT (Vận hành hệ thống): đọc các Hangfire job trong `Cinema.Application/BackgroundJobs/` và kiểm tra các job thật (giải phóng ghế, vòng đời suất chiếu, gửi email, hoàn tiền, `ExpireLoyaltyPointsJob` tính lại điểm hết hạn) đã có use case tương ứng chưa; thiếu thì ghi vào Ghi chú đối chiếu.
6. Hình 3.18–3.24: phần văn bản mô tả hình phải khớp mã PlantUML.

ĐẦU RA: duy nhất file `08-chuong-3-use-case-qn-den-ht.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 09 — Chương 3 (3.4–3.6) — Đặc tả use case chính, quy tắc nghiệp vụ, yêu cầu phi chức năng

````text
Thực hiện PHẦN 09: Chương 3 (3.4–3.6) — Đặc tả use case chính, quy tắc nghiệp vụ, yêu cầu phi chức năng.

PHẠM VI: trong outline, từ mục "3.4 ĐẶC TẢ USE CASE CHÍNH" đến hết mục "3.6.1 Danh sách yêu cầu phi chức năng" (khoảng dòng 1234–1472). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Bảng 3.25–3.35.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Chính file `chuong-3` của PHẦN 06–08 đã viết (để đồng bộ nội dung)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. 9 đặc tả (Bảng 3.25 trở đi), mỗi đặc tả một bảng 2 cột: Mã/Tên, Tác nhân, Mô tả, Tiền điều kiện, Hậu điều kiện, Luồng chính (có "Chi tiết các bước"), Luồng thay thế, Ngoại lệ (kèm mã HTTP), Nguồn. Mô tả từng bước bằng câu hoàn chỉnh.
2. Luồng chính và ngoại lệ phải khớp từng chữ với bảng use case ở 3.3 và với code; không được mâu thuẫn với PHẦN 06–08.
3. 3.5 Quy tắc nghiệp vụ: mỗi quy tắc có mã BR, nội dung, và nguồn code (file + hàm) xác minh được.
4. 3.6 Yêu cầu phi chức năng: mỗi NFR nêu cách kiểm chứng và mục Chương 6 tương ứng; NFR chưa có phép đo thì ghi `[CẦN BỔ SUNG]`, không tự đặt chỉ tiêu.

ĐẦU RA: duy nhất file `09-chuong-3-dac-ta-quy-tac-nfr.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 10 — Chương 4 (4.1) — Kiến trúc hệ thống

````text
Thực hiện PHẦN 10: Chương 4 (4.1) — Kiến trúc hệ thống.

PHẠM VI: trong outline, từ mục "CHƯƠNG 4. THIẾT KẾ HỆ THỐNG" đến hết mục "4.1.6 Quản lý cấu hình" (khoảng dòng 1473–1556). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 4.1–4.3; Bảng 4.1–4.3.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Các hình kiến trúc dựng từ `Program.cs`, `DependencyInjection.cs`, `.csproj` và cấu trúc `frontend/src`; chỉ nêu thành phần có thật.
2. 4.1.4 (phản hồi API thống nhất), 4.1.5 (xử lý lỗi), 4.1.6 (cấu hình): đọc kiểu `ApiResponse`/middleware xử lý ngoại lệ/Options pattern thật, nêu đúng tên lớp, mã trạng thái, tên khóa cấu hình.
3. Không lặp nội dung của 2.5.3; chỉ nối tiếp bằng cách Perfo áp dụng cụ thể.

ĐẦU RA: duy nhất file `10-chuong-4-kien-truc.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 11 — Chương 4 (4.2.1–4.2.3) — Nguyên tắc, ký hiệu và sơ đồ lớp miền

````text
Thực hiện PHẦN 11: Chương 4 (4.2.1–4.2.3) — Nguyên tắc, ký hiệu và sơ đồ lớp miền.

PHẠM VI: trong outline, từ mục "4.2 SƠ ĐỒ LỚP" đến hết mục "4.2.3 Sơ đồ lớp miền" (khoảng dòng 1557–1662). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 4.4; Bảng 4.4–4.6.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Mã PlantUML Hình 4.4 (`ma-nguon-so-do-bo-sung.md`)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. 4.2.3: đếm lại số thực thể và enum trong `Cinema.Domain/Entities` và `Enums` (outline nêu 30 thực thể, 10 enum); lệch thì ghi Ghi chú đối chiếu.
2. Phải có `LoyaltyPointEntry` (sổ điểm: `UserId`, `BookingId` nullable, `Points`, `RevokedPoints`, `EarnedAt`) nối `User` (nhiều–một) và `Booking` (0..1); kiểm tra thuộc tính đúng với `Domain/Entities/LoyaltyPointEntry.cs` và với mã PlantUML đã cập nhật.
3. Bảng 4.4 (ký hiệu): lấy từ `skinparam` và stereotype trong mã PlantUML; chỉ mô tả ký hiệu thật sự được dùng.
4. Phân biệt rõ với ERD: sơ đồ lớp không ghi khóa, chỉ mục, ràng buộc CHECK.

ĐẦU RA: duy nhất file `11-chuong-4-so-do-lop-mien.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 12 — Chương 4 (4.2.4–4.2.12) — Sơ đồ lớp package XT, HS, DP, DG, DV, TT, AI, BQ, SV

````text
Thực hiện PHẦN 12: Chương 4 (4.2.4–4.2.12) — Sơ đồ lớp package XT, HS, DP, DG, DV, TT, AI, BQ, SV.

PHẠM VI: trong outline, từ mục "4.2.4 Sơ đồ lớp package Xác thực" đến hết mục "4.2.12 Sơ đồ lớp package Soát vé" (khoảng dòng 1663–1920). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 4.5–4.13; Bảng 4.7–4.15.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Mã PlantUML Hình 4.5–4.13 (`ma-nguon-so-do-bo-sung.md`)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Mỗi mục `4.2.n` một hình + bảng đi kèm (danh sách đầy đủ lớp/phương thức nếu outline yêu cầu); chuỗi Controller → interface dịch vụ → lớp dịch vụ → thực thể/DTO.
2. Tên lớp, interface, phương thức chép nguyên văn từ code, bỏ tham số `CancellationToken`; mỗi lớp vẽ tối đa 8 phương thức công khai, vượt thì ghi "rút gọn x/y" và để danh sách đầy đủ trong bảng.
3. Đọc mã PlantUML đính kèm, kiểm chứng từng lớp/phương thức/quan hệ với code thật; sai lệch thì ghi Ghi chú đối chiếu (không tự vẽ lại sơ đồ).

ĐẦU RA: duy nhất file `12-chuong-4-so-do-lop-xt-den-sv.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 13 — Chương 4 (4.2.13–4.2.25) — Sơ đồ lớp package QB … HT và ánh xạ package–nhóm lớp

````text
Thực hiện PHẦN 13: Chương 4 (4.2.13–4.2.25) — Sơ đồ lớp package QB … HT và ánh xạ package–nhóm lớp.

PHẠM VI: trong outline, từ mục "4.2.13 Sơ đồ lớp package Quản lý booking" đến hết mục "4.2.25 Ánh xạ package use case sang nhóm lớp" (khoảng dòng 1921–2257). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 4.14–4.25; Bảng 4.16–4.28.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Mã PlantUML Hình 4.14–4.25 (`ma-nguon-so-do-bo-sung.md`)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Mỗi mục `4.2.n` một hình + bảng đi kèm (danh sách đầy đủ lớp/phương thức nếu outline yêu cầu); chuỗi Controller → interface dịch vụ → lớp dịch vụ → thực thể/DTO.
2. Tên lớp, interface, phương thức chép nguyên văn từ code, bỏ tham số `CancellationToken`; mỗi lớp vẽ tối đa 8 phương thức công khai, vượt thì ghi "rút gọn x/y" và để danh sách đầy đủ trong bảng.
3. Đọc mã PlantUML đính kèm, kiểm chứng từng lớp/phương thức/quan hệ với code thật; sai lệch thì ghi Ghi chú đối chiếu (không tự vẽ lại sơ đồ).
4. 4.2.24 (Vận hành hệ thống, Hình 4.25): phải có job nền và dịch vụ điểm hết hạn (`ExpireLoyaltyPointsJob`, `LoyaltyExpiryService`); xác minh tên lớp/phương thức trong code.
5. 4.2.25 (ánh xạ package use case ↔ nhóm lớp): phải đủ 21 package; mỗi dòng trỏ đến controller/dịch vụ có thật.

ĐẦU RA: duy nhất file `13-chuong-4-so-do-lop-qb-den-ht.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 14 — Chương 4 (4.3.1–4.3.4) — CSDL: nguyên tắc, ERD đầy đủ, module Tài khoản người dùng

````text
Thực hiện PHẦN 14: Chương 4 (4.3.1–4.3.4) — CSDL: nguyên tắc, ERD đầy đủ, module Tài khoản người dùng.

PHẠM VI: trong outline, từ mục "4.3 CƠ SỞ DỮ LIỆU" đến hết mục "4.3.4 Module Tài khoản người dùng" (khoảng dòng 2258–2449). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 4.26–4.27; Bảng 4.29–4.38.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Migration và `ModelSnapshot` trong `backend/Cinema.Infrastructure/Persistence/`
- Bản vá `ma-nguon-so-do-mermaid-loyalty.md` (phần Hình 4.26) + mã Mermaid gốc `ma-nguon-so-do.md`

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Hình 4.26 (ERD đầy đủ): thêm khối `LoyaltyPointEntries` và hai quan hệ theo bản vá điểm thành viên; đối chiếu tên cột khóa chính của `Users`, `Bookings`, `CustomerProfiles` với ERD hiện có (bản vá ghi rõ cần đối chiếu) và báo lệch nếu có.
2. Với mỗi bảng: lấy kiểu dữ liệu, NULL, khóa, mặc định, chỉ mục, ràng buộc từ `Configurations/*.cs` và migration/snapshot, không đoán; bảng cột có thêm cột "Ý nghĩa" viết đầy đủ cho từng cột.
3. Kèm giải thích quan hệ, ràng buộc toàn vẹn và nghiệp vụ của từng thực thể.

ĐẦU RA: duy nhất file `14-chuong-4-csdl-tong-quan-tai-khoan.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 15 — Chương 4 (4.3.5–4.3.7) — CSDL: module Danh mục phim, Phim, Phòng chiếu

````text
Thực hiện PHẦN 15: Chương 4 (4.3.5–4.3.7) — CSDL: module Danh mục phim, Phim, Phòng chiếu.

PHẠM VI: trong outline, từ mục "4.3.5 Module Danh mục phim" đến hết mục "4.3.7 Module Phòng chiếu" (khoảng dòng 2450–2758). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 4.28–4.30; Bảng 4.39–4.57.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Migration và `ModelSnapshot`
- Mã Mermaid ERD Hình 4.28–4.30 (`ma-nguon-so-do.md`)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Cấu trúc mỗi module: giới thiệu vai trò/thực thể/quan hệ với module khác → `x.1` ERD của module (thực thể sở hữu đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính) → `x.2` mô tả thực thể (bảng cột: Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Mặc định/ghi chú | Ý nghĩa).
2. Kiểu dữ liệu, NULL, khóa, chỉ mục, CHECK, mặc định lấy từ `Persistence/Configurations/*.cs` và migration/snapshot; không đoán. Cột "Ý nghĩa" viết đủ cho từng cột; giải thích quan hệ và nghiệp vụ của từng thực thể.
3. Đọc mã Mermaid ERD đính kèm, kiểm tra khớp với cấu hình EF; lệch thì ghi Ghi chú đối chiếu.

ĐẦU RA: duy nhất file `15-chuong-4-csdl-danh-muc-phim-phong.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 16 — Chương 4 (4.3.8–4.3.10) — CSDL: module Suất chiếu, Đặt vé, Thanh toán

````text
Thực hiện PHẦN 16: Chương 4 (4.3.8–4.3.10) — CSDL: module Suất chiếu, Đặt vé, Thanh toán.

PHẠM VI: trong outline, từ mục "4.3.8 Module Suất chiếu" đến hết mục "4.3.10 Module Thanh toán" (khoảng dòng 2759–3048). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 4.31–4.33; Bảng 4.58–4.70.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Migration và `ModelSnapshot`
- Mã Mermaid ERD Hình 4.31–4.33 (`ma-nguon-so-do.md`)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Cấu trúc mỗi module: giới thiệu vai trò/thực thể/quan hệ với module khác → `x.1` ERD của module (thực thể sở hữu đủ thuộc tính; thực thể tham chiếu chỉ có khóa chính) → `x.2` mô tả thực thể (bảng cột: Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Mặc định/ghi chú | Ý nghĩa).
2. Kiểu dữ liệu, NULL, khóa, chỉ mục, CHECK, mặc định lấy từ `Persistence/Configurations/*.cs` và migration/snapshot; không đoán. Cột "Ý nghĩa" viết đủ cho từng cột; giải thích quan hệ và nghiệp vụ của từng thực thể.
3. Đọc mã Mermaid ERD đính kèm, kiểm tra khớp với cấu hình EF; lệch thì ghi Ghi chú đối chiếu.
4. Chú ý chỉ mục duy nhất và điều kiện khóa dùng cho chống đặt trùng ghế (`ShowtimeSeat`), cùng các cột trạng thái/hạn giữ ghế; mô tả đúng như cấu hình EF.

ĐẦU RA: duy nhất file `16-chuong-4-csdl-suat-chieu-dat-ve-thanh-toan.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 17 — Chương 4 (4.3.11–4.3.16) — CSDL: module Thành viên, Cấu hình rạp, enum, vòng đời trạng thái, migration, dữ liệu khởi tạo

````text
Thực hiện PHẦN 17: Chương 4 (4.3.11–4.3.16) — CSDL: module Thành viên, Cấu hình rạp, enum, vòng đời trạng thái, migration, dữ liệu khởi tạo.

PHẠM VI: trong outline, từ mục "4.3.11 Module Thành viên" đến hết mục "4.3.16 Dữ liệu khởi tạo" (khoảng dòng 3049–3286). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 4.34–4.40; Bảng 4.71–4.78.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Bản vá `ma-nguon-so-do-mermaid-loyalty.md` (Hình 4.34)
- Migration, `SeedData.cs`, `Migrations/_manual/backfill_loyalty_entries.sql`
- Mã Mermaid Hình 4.35–4.40 (`ma-nguon-so-do.md`)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. 4.3.11 Thành viên: dùng mã Hình 4.34 trong bản vá; mô tả `MembershipTiers` và `LoyaltyPointEntries` (Bảng 4.71, 4.72); chỉ mục duy nhất `UX_LoyaltyPointEntries_Booking` (BookingId duy nhất khi khác NULL) kiểm tra đúng trong cấu hình EF/migration; nêu `CustomerProfile.Points`/`TierId` chỉ là giá trị đồng bộ, sổ điểm là nguồn gốc.
2. 4.3.13–4.3.14: liệt kê enum và vòng đời Booking, ShowtimeSeat, Payment, Showtime, Movie (Hình 4.36–4.40) chỉ với các chuyển trạng thái có thật trong service; ghi nguồn hàm thực hiện chuyển đổi.
3. 4.3.15 Migration: liệt kê migration thật (tên, nội dung chính), nêu migration thêm `LoyaltyPointEntries` và script backfill thủ công.
4. 4.3.16 Dữ liệu khởi tạo: đọc `SeedData.cs`, nêu đúng dữ liệu được seed (kể cả hạng thành viên với MinPoints/DiscountPercent thật).

ĐẦU RA: duy nhất file `17-chuong-4-csdl-thanh-vien-vong-doi.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 18 — Chương 4 (4.4) — Xác thực và phân quyền

````text
Thực hiện PHẦN 18: Chương 4 (4.4) — Xác thực và phân quyền.

PHẠM VI: trong outline, từ mục "4.4 XÁC THỰC" đến hết mục "4.4.8 Ma trận phân quyền" (khoảng dòng 3287–3402). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 4.41–4.46; Bảng 4.79–4.80.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Mã Mermaid Hình 4.41–4.46 (`ma-nguon-so-do.md`)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Các luồng 4.4.1–4.4.6: mô tả theo đúng thứ tự gọi trong `AuthService.cs`, `JwtTokenService.cs`, `GoogleAuthService.cs`; làm mới token xoay vòng và phát hiện dùng lại token mô tả đúng cơ chế chuỗi (`ReplacedByTokenId`, `RevokeTokenChainAsync`).
2. Bảng 4.79 (loại token): thời hạn, nơi lưu, dạng băm lấy từ options/cấu hình thật.
3. Bảng 4.80 (ma trận phân quyền): quét TẤT CẢ controller, mỗi controller một dòng với policy thật (`CustomerOnly`, `StaffOnly`, `AdminOnly`, công khai); không bỏ sót controller.
4. Hình 4.41–4.46: phần văn bản khớp mã sơ đồ.

ĐẦU RA: duy nhất file `18-chuong-4-xac-thuc.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 19 — Chương 4 (4.5.1–4.5.4) — Giữ ghế, thanh toán trực tuyến, hết hạn giữ ghế, hủy vé

````text
Thực hiện PHẦN 19: Chương 4 (4.5.1–4.5.4) — Giữ ghế, thanh toán trực tuyến, hết hạn giữ ghế, hủy vé.

PHẠM VI: trong outline, từ mục "4.5 LUỒNG NGHIỆP VỤ" đến hết mục "4.5.4 Hủy vé" (khoảng dòng 3403–3549). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 4.47–4.54; Bảng 4.81–4.83; công thức (4.1), (4.2), (4.3).

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Mã Mermaid Hình 4.47–4.54 (`ma-nguon-so-do.md`)
- Bản vá `ma-nguon-so-do-mermaid-loyalty.md` (mục "Hình 4.51 và Hình 4.54 — bước cần thêm")

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Công thức (4.1)–(4.3) (TotalAmount, DiscountAmount làm tròn ra xa 0 chỉ trên tiền vé không tính combo, FinalAmount): đối chiếu với `BookingService.cs`. Giảm giá lấy hạng từ điểm hiệu lực đọc từ sổ điểm (`CalculateDiscountAmountAsync`), KHÔNG đọc `CustomerProfile.TierId`.
2. Hình 4.51 (tuần tự phát hành vé): sau bước tạo vé và mã QR thêm hai thông điệp `BookingService → DB`: ghi `LoyaltyPointEntry(BookingId, Points, EarnedAt)` rồi tính lại `Points`, `TierId` từ sổ điểm. Hình 4.54 (lưu đồ hủy vé): sau hoàn tiền qua cổng, bước "thu hồi điểm" đổi thành thu hồi vào đúng dòng sổ điểm của booking (`bookingId`), tính lại điểm và hạng. Văn bản mô tả phải khớp phiên bản đã cập nhật.
3. Bảng 4.81 (các kiểm tra khi giữ ghế): từng kiểm tra đối chiếu với `CreateHoldBookingAsync` và `SeatLayoutRules.cs`.
4. 4.5.2: Bảng 4.82 (so sánh VNPay, MoMo, Stripe) chỉ nêu điều có trong code của từng gateway; luồng IPN idempotent mô tả đúng kiểm tra chữ ký, số tiền, lặp, ghế hết hạn.
5. Bảng 4.83 (chính sách và tình huống hoàn tiền) đối chiếu với `PaymentService.cs`/dịch vụ hoàn tiền thật.

ĐẦU RA: duy nhất file `19-chuong-4-luong-giu-ghe-thanh-toan-huy-ve.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 20 — Chương 4 (4.5.5–4.5.9) — Bán vé quầy, soát vé, hủy suất chiếu, điểm thưởng, đánh giá phim

````text
Thực hiện PHẦN 20: Chương 4 (4.5.5–4.5.9) — Bán vé quầy, soát vé, hủy suất chiếu, điểm thưởng, đánh giá phim.

PHẠM VI: trong outline, từ mục "4.5.5 Bán vé tại quầy" đến hết mục "4.5.9 Đánh giá phim" (khoảng dòng 3550–3608). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 4.55–4.59; Bảng 4.84; công thức (4.4), (4.5), (4.6), (4.7).

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Bản vá `ma-nguon-so-do-mermaid-loyalty.md` (Hình 4.58)
- `SeedData.cs`, `LoyaltyWindow.cs`, `LoyaltyExpiryService.cs`, `ExpireLoyaltyPointsJob.cs`, `Migrations/_manual/backfill_loyalty_entries.sql`
- Mã Mermaid Hình 4.55–4.57, 4.59 (`ma-nguon-so-do.md`)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. 4.5.8 Điểm thưởng: dùng mã Hình 4.58 trong bản vá; công thức (4.4)–(4.7) phải khớp code (`⌊FinalAmount / PointsPerAmount⌋`, cửa sổ `LoyaltyWindow.Months` = 12, chọn hạng có `MinPoints` lớn nhất ≤ điểm hiệu lực, `RevokedPoints ← min(Points, …)` trong `ReverseLoyaltyAsync`).
2. Bảng 4.84 (hạng thành viên) BẮT BUỘC đọc `SeedData.cs` lấy giá trị thật `MinPoints`, `DiscountPercent`; không để trống, không đoán.
3. Kiểm chứng trong code và nêu đúng: cron `ExpireLoyaltyPointsJob` (outline ghi `5 * * * *`, phút 05 mỗi giờ, múi giờ VN), độ trễ đồng bộ tối đa 1 giờ trên hồ sơ nhưng giảm giá lúc đặt vé luôn đọc trực tiếp từ sổ, `TotalSpent` vẫn là tổng chi tiêu trọn đời, combo vẫn được tích điểm, vai trò script backfill. Hạn chế chỉ tham chiếu mục 7.3.15, không viết lại ở đây.
4. 4.5.5–4.5.7, 4.5.9: mô tả theo đúng hàm service tương ứng; Hình 4.55–4.57, 4.59 khớp mã sơ đồ.

ĐẦU RA: duy nhất file `20-chuong-4-luong-quay-soat-ve-diem-danh-gia.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 21 — Chương 4 (4.6) — Thuật toán: luật chọn ghế, gợi ý ghế, xếp lịch, trùng lịch, múi giờ

````text
Thực hiện PHẦN 21: Chương 4 (4.6) — Thuật toán: luật chọn ghế, gợi ý ghế, xếp lịch, trùng lịch, múi giờ.

PHẠM VI: trong outline, từ mục "4.6 THUẬT TOÁN" đến hết mục "4.6.5 Xử lý múi giờ" (khoảng dòng 3609–3716). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 4.60–4.63; Bảng 4.85–4.87; công thức (4.8), (4.9), (4.10), (4.11), (4.12), (4.13), (4.14).

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Mã/hình Hình 4.60–4.63 (`ma-nguon-so-do.md`)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Các công thức (4.8), (4.9), (4.12) hiện là `[CẦN BỔ SUNG]`: đọc `SeatLayoutRules.cs` và `ShowtimeScheduler.cs`, viết biểu thức đúng theo code (kèm tên biến/hằng thật).
2. Công thức (4.10), (4.11), (4.13), (4.14): đối chiếu trọng số và hằng số (0,45/0,45/0,1; 0,7/0,3; `FitWeight`, `LoadWeight`, `Spread`) với code; nếu lệch thì theo code và ghi Ghi chú đối chiếu.
3. Heat dùng thành phần Rating theo code hiện tại: trình bày đúng như code đang làm (không tự sửa công thức); hạn chế thuộc mục 7.3.3.
4. Bảng 4.85 (ba chế độ luật ghế lẻ) lấy ví dụ cụ thể từ `SeatLayoutRules.cs`; Bảng 4.86 (tham số bộ xếp lịch) lấy giá trị mặc định từ `SchedulerOptions.cs`; Bảng 4.87 (quy ước thời gian) từ code xử lý múi giờ.

ĐẦU RA: duy nhất file `21-chuong-4-thuat-toan.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 22 — Chương 4 (4.7–4.8) — Giao tiếp thời gian thực, trợ lý AI

````text
Thực hiện PHẦN 22: Chương 4 (4.7–4.8) — Giao tiếp thời gian thực, trợ lý AI.

PHẠM VI: trong outline, từ mục "4.7 GIAO TIẾP THỜI GIAN THỰC" đến hết mục "4.8.3 Giới hạn ngữ cảnh" (khoảng dòng 3717–3781). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 4.64–4.65; Bảng 4.88–4.89.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Mã Mermaid Hình 4.64–4.65 (`ma-nguon-so-do.md`)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Bảng 4.88 (sự kiện SignalR): tên sự kiện, payload, nhóm nhận chép nguyên văn từ Hub và nơi phát sự kiện; mô tả xử lý phía client từ hook trong `frontend/src`.
2. Bảng 4.89 (hàm của trợ lý AI): khai báo hàm, tham số, giới hạn quyền/ngữ cảnh chép đúng từ code của dịch vụ AI.
3. Giữ phần "hạn chế endpoint AI" cho mục 7.3.6, không nêu trước ở đây.

ĐẦU RA: duy nhất file `22-chuong-4-thoi-gian-thuc-tro-ly-ai.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 23 — Chương 4 (4.9–4.10) — Giao diện và giao diện lập trình

````text
Thực hiện PHẦN 23: Chương 4 (4.9–4.10) — Giao diện và giao diện lập trình.

PHẠM VI: trong outline, từ mục "4.9 GIAO DIỆN" đến hết mục "4.10.2 Tổng hợp endpoint theo nhóm" (khoảng dòng 3782–3912). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 4.66–4.67; Bảng 4.90–4.91.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- File `.agents/skills/perfo-design-system/references/tokens.css`
- Mã Mermaid Hình 4.66–4.67 (`ma-nguon-so-do.md`)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Bảng 4.90 (token thiết kế rút gọn): chọn token từ `tokens.css` và nêu lý do chọn; không ghi giá trị khi chưa đọc file.
2. Hình 4.66–4.67 (sơ đồ trang): đối chiếu với cấu hình route thật của `frontend`; 4.9.4–4.9.9 mô tả màn hình theo component/route thật, không tả giao diện chưa có.
3. Bảng 4.91 (tổng hợp endpoint): đếm từ controller thật, phải khớp Phụ lục 2 (PHẦN 34).

ĐẦU RA: duy nhất file `23-chuong-4-giao-dien-api.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 24 — Chương 5 (5.1–5.3) — Môi trường, cấu trúc mã nguồn, cài đặt backend

````text
Thực hiện PHẦN 24: Chương 5 (5.1–5.3) — Môi trường, cấu trúc mã nguồn, cài đặt backend.

PHẠM VI: trong outline, từ mục "CHƯƠNG 5. CÀI ĐẶT HỆ THỐNG" đến hết mục "5.3.17 Cổng thanh toán đã cài đặt" (khoảng dòng 3913–4113). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 5.1; Bảng 5.1–5.3.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Bảng 5.2 (quy mô mã nguồn): số file/số dòng đếm bằng lệnh thật (ví dụ `cloc`/`wc`), ghi rõ lệnh và phạm vi loại trừ; không ước lượng.
2. 5.3.x: mỗi module nêu lớp chính, luồng cài đặt, điểm kỹ thuật đáng chú ý lấy từ code; không lặp nội dung thiết kế ở Chương 4, dùng tham chiếu mục.
3. Bảng 5.3 (cổng thanh toán và trạng thái kiểm chứng): chỉ ghi "đã kiểm chứng" khi có bằng chứng (test/log); còn lại `[CẦN BỔ SUNG]`.
4. Mô tả module thành viên (5.3.9) khớp cơ chế sổ điểm và `LoyaltyExpiryService`.

ĐẦU RA: duy nhất file `24-chuong-5-backend.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 25 — Chương 5 (5.4.1) — Cài đặt frontend: giao diện khách hàng

````text
Thực hiện PHẦN 25: Chương 5 (5.4.1) — Cài đặt frontend: giao diện khách hàng.

PHẠM VI: trong outline, từ mục "5.4 CÀI ĐẶT FRONTEND" đến hết mục "5.4.1 Giao diện khách hàng" (khoảng dòng 4114–4233). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 5.2–5.15.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Ảnh chụp màn hình các trang khách hàng (nếu có)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Mỗi màn hình theo khung outline: mục đích → thành phần giao diện → tương tác/trạng thái → component và API sử dụng; đọc `frontend/src/features/client/**` thật.
2. Hình 5.x là ảnh chụp màn hình: không tự tạo ảnh; chèn placeholder + caption, chưa có ảnh thì `[CẦN BỔ SUNG: ảnh chụp]`.
3. Nêu đúng tên component, hook, store, endpoint được gọi.

ĐẦU RA: duy nhất file `25-chuong-5-frontend-khach-hang.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 26 — Chương 5 (5.4.2) — Cài đặt frontend: giao diện quản trị

````text
Thực hiện PHẦN 26: Chương 5 (5.4.2) — Cài đặt frontend: giao diện quản trị.

PHẠM VI: trong outline, từ mục "5.4.2 Giao diện quản trị" đến hết mục "5.4.2 Giao diện quản trị" (khoảng dòng 4234–4380). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 5.16–5.33.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Ảnh chụp màn hình các trang quản trị (nếu có)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Đọc `frontend/src/features/admin/**`; mỗi màn hình theo khung: mục đích → thành phần → tương tác/trạng thái → component và API.
2. Hình 5.x là ảnh chụp màn hình: placeholder + caption, không tự tạo ảnh.
3. Trình soạn sơ đồ ghế, quầy bán vé, soát vé, dashboard: mô tả đúng cơ chế cài đặt (hook, thư viện) có trong code.

ĐẦU RA: duy nhất file `26-chuong-5-frontend-quan-tri.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 27 — Chương 5 (5.4.3–5.7) — Giao diện đáp ứng, thành phần dùng chung, SEO, cấu hình, bảo mật, triển khai

````text
Thực hiện PHẦN 27: Chương 5 (5.4.3–5.7) — Giao diện đáp ứng, thành phần dùng chung, SEO, cấu hình, bảo mật, triển khai.

PHẠM VI: trong outline, từ mục "5.4.3 Giao diện đáp ứng" đến hết mục "5.7.5 Vercel" (khoảng dòng 4381–4537). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 5.34–5.35; Bảng 5.4–5.6.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- `Dockerfile`, `docker-compose.yml`, file cấu hình Render/Vercel, `robots.txt`, sitemap, `appsettings*.json`, `.env.example`

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Bảng 5.4 (SEO): dựa trên robots/sitemap/meta thật; Bảng 5.5 (biến cấu hình): chỉ tên biến và mô tả, KHÔNG ghi giá trị bí mật.
2. Bảng 5.6 (cơ chế bảo mật và vị trí cài đặt): mỗi cơ chế phải chỉ ra file/lớp cài đặt thật; cơ chế chưa cài thì không nêu.
3. 5.7: mô tả Docker, Compose, Render, Vercel đúng theo file cấu hình thật; sơ đồ triển khai chỉ có thành phần thật.

ĐẦU RA: duy nhất file `27-chuong-5-frontend-khac-cau-hinh-trien-khai.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 28 — Chương 6 (6.1–6.3) — Chiến lược kiểm thử, kiểm thử đơn vị, kiểm thử tích hợp

````text
Thực hiện PHẦN 28: Chương 6 (6.1–6.3) — Chiến lược kiểm thử, kiểm thử đơn vị, kiểm thử tích hợp.

PHẠM VI: trong outline, từ mục "CHƯƠNG 6. KIỂM THỬ HỆ THỐNG" đến hết mục "6.3.2 Danh sách test tích hợp" (khoảng dòng 4538–4754). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 6.1; Bảng 6.1–6.12.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Thư mục `backend/tests/**`
- Log `dotnet test` / báo cáo độ bao phủ (nếu đã chạy)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Mỗi bảng test (Bảng 6.2–6.10, 6.12): tên test chép nguyên văn từ file test, mục đích và kết quả mong đợi suy từ nội dung test; số test là số thuộc tính `[Fact]/[Theory]` đếm từ code (không tính số bộ dữ liệu của Theory).
2. Bảng 6.11 và mọi con số kết quả chạy: CHỈ điền khi có log thật được cung cấp; nếu chưa có giữ `[CẦN BỔ SUNG: log dotnet test]`.
3. 6.3: mô tả hạ tầng test tích hợp (Testcontainers, factory) đúng với code.

ĐẦU RA: duy nhất file `28-chuong-6-chien-luoc-don-vi-tich-hop.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 29 — Chương 6 (6.4) — Kiểm thử end-to-end

````text
Thực hiện PHẦN 29: Chương 6 (6.4) — Kiểm thử end-to-end.

PHẠM VI: trong outline, từ mục "6.4 KIỂM THỬ END-TO-END" đến hết mục "6.4.16 Lưu ý về môi trường chạy E2E" (khoảng dòng 4755–4998). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Hình 6.2; Bảng 6.13–6.25; Biểu đồ 6.1.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Thư mục `bo_sung/` (e2e Playwright, playwright-report, sitemap/robots)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Bảng 6.15–6.22: từng bước kịch bản lấy từ `bo_sung/e2e/*.spec.ts`; kết quả 8/8 đạt và thời gian từng kịch bản (Bảng 6.23, Biểu đồ 6.1) lấy từ playwright-report thật.
2. 6.4.15–6.4.16 (phạm vi chưa bao phủ, lưu ý môi trường): viết trung thực, dựa trên nội dung bộ E2E hiện có.
3. Không bịa số liệu thời gian; thiếu dữ liệu thì `[CẦN BỔ SUNG]`.

ĐẦU RA: duy nhất file `29-chuong-6-e2e.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 30 — Chương 6 (6.5–6.6) — Thực nghiệm đặt ghế đồng thời, kiểm thử tải

````text
Thực hiện PHẦN 30: Chương 6 (6.5–6.6) — Thực nghiệm đặt ghế đồng thời, kiểm thử tải.

PHẠM VI: trong outline, từ mục "6.5 THỰC NGHIỆM ĐẶT GHẾ ĐỒNG THỜI" đến hết mục "6.6.2 Kết quả" (khoảng dòng 4999–5047). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Bảng 6.26–6.27; Biểu đồ 6.2.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Test đồng thời trong `backend/tests/**` và log chạy (nếu có)
- Kịch bản k6 `load-tests/` (hiện chưa có trong dự án)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. 6.5: mô tả thiết kế thực nghiệm (10 yêu cầu song song: 1 thành công, còn lại 409) bám theo test đồng thời thật; kết quả chỉ ghi khi có log chạy.
2. 6.6: `load-tests/` chưa tồn tại nên chỉ viết phương pháp + khung bảng; Bảng 6.27 và Biểu đồ 6.2 giữ `[CẦN BỔ SUNG: kết quả tải]`, tuyệt đối không bịa số.

ĐẦU RA: duy nhất file `30-chuong-6-dong-thoi-tai.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 31 — Chương 7 — Đánh giá kết quả

````text
Thực hiện PHẦN 31: Chương 7 — Đánh giá kết quả.

PHẠM VI: trong outline, từ mục "CHƯƠNG 7. ĐÁNH GIÁ KẾT QUẢ" đến hết mục "7.3.16 Tổng hợp hạn chế" (khoảng dòng 5048–5258). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Bảng 1.1, Bảng 7.1–7.3.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Các file đã viết của Chương 3–6 (để truy vết)

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. 7.1 Bảng 7.1: truy vết từng package use case đến test thật (tên test/file); package không có test thì ghi rõ "chưa có test".
2. 7.2: mức đạt mục tiêu MT1–MT5 chỉ dựa trên kết quả đã có ở Chương 6; chưa có thì `[CẦN BỔ SUNG]`.
3. 7.3.1–7.3.16: với MỖI hạn chế, mở file/hàm được nêu để xác minh hạn chế còn đúng ở code hiện tại; nếu đã được sửa thì ghi Ghi chú đối chiếu thay vì viết như còn tồn tại.
4. 7.3.15 (cơ chế điểm 12 tháng): nêu các hạn chế có thật như trễ đồng bộ tối đa 1 giờ trên hồ sơ, cần script backfill cho khách cũ, `TotalSpent` không theo cửa sổ trượt, chưa có thông báo điểm sắp hết hạn; đối chiếu với code.
5. Bảng 7.3 (tổng hợp hạn chế, đề xuất khắc phục) khớp với 7.3.1–7.3.15.

ĐẦU RA: duy nhất file `31-chuong-7-danh-gia.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 32 — Chương 8 — Kết luận và Tài liệu tham khảo

````text
Thực hiện PHẦN 32: Chương 8 — Kết luận và Tài liệu tham khảo.

PHẠM VI: trong outline, từ mục "CHƯƠNG 8. KẾT LUẬN" đến hết mục "TÀI LIỆU THAM KHẢO" (khoảng dòng 5259–5314). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Bảng 8.1.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Quy định trích dẫn (Mục 0.4) trong `quy-dinh-trinh-bay.md`

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. 8.1 Bảng 8.1 và 8.1.2 chỉ dùng kết quả đã có ở Chương 6–7; 8.2–8.3 tóm tắt đúng hạn chế/hướng phát triển của Chương 7, không thêm hạn chế mới.
2. Tài liệu tham khảo: chỉ đưa tài liệu thật đã đọc, theo mẫu ở Mục 0.4; chưa có thì giữ khung nhóm T1–T16 với `[CẦN BỔ SUNG]`, không tự tạo tài liệu.

ĐẦU RA: duy nhất file `32-chuong-8-ket-luan-tai-lieu-tham-khao.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 33 — Phụ lục 1 (cài đặt, vận hành), Phụ lục 3 (biến môi trường), Phụ lục 4 (kịch bản E2E chi tiết)

````text
Thực hiện PHẦN 33: Phụ lục 1 (cài đặt, vận hành), Phụ lục 3 (biến môi trường), Phụ lục 4 (kịch bản E2E chi tiết).

PHẠM VI: trong outline, từ "PHỤ LỤC 1. Hướng dẫn cài đặt, vận hành" đến hết "P1.8 SWAGGER, HANGFIRE" (dòng 5315–5376); và từ "PHỤ LỤC 3. Biến cấu hình môi trường" đến hết "P4.1 CÁC BƯỚC TỪNG KỊCH BẢN" (dòng 5555–5602). Chỉ viết đúng phạm vi này.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- `package.json`, README, `docker-compose.yml`, `.env.example`, `appsettings*.json`, `bo_sung/e2e/*.spec.ts`

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. P1: lệnh cài đặt/chạy/migration/test lấy từ `package.json` (scripts), `docker-compose.yml`, `dotnet-tools.json`; P1.7 tài khoản kiểm thử chỉ lấy tài khoản demo trong `SeedData.cs`.
2. P3: bảng đủ biến cấu hình thật, không ghi giá trị bí mật.
3. P4: các bước từng kịch bản E2E phải khớp Bảng 6.15–6.22 và `*.spec.ts`.

ĐẦU RA: duy nhất file `33-phu-luc-1-3-4.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 34 — Phụ lục 2 — Danh mục API

````text
Thực hiện PHẦN 34: Phụ lục 2 — Danh mục API.

PHẠM VI: trong outline, từ mục "PHỤ LỤC 2. Danh mục API" đến hết mục "P2.3 ENDPOINT THANH TOÁN, KIỂM TRA SỨC KHỎE" (khoảng dòng 5377–5554). Chỉ viết đúng phạm vi này.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Toàn bộ `backend/Cinema.Api/Controllers/**`

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Liệt kê TOÀN BỘ endpoint từ controller thật: phương thức HTTP, route, policy, mô tả ngắn, request/response DTO, giới hạn tốc độ; chia theo P2.1 (khách hàng), P2.2 (quản trị), P2.3 (thanh toán, kiểm tra sức khỏe).
2. Tổng số endpoint theo controller phải khớp Bảng 4.91; lệch thì ghi Ghi chú đối chiếu.

ĐẦU RA: duy nhất file `34-phu-luc-2-api.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 35 — Phụ lục 5 — Trích đoạn mã nguồn tiêu biểu

````text
Thực hiện PHẦN 35: Phụ lục 5 — Trích đoạn mã nguồn tiêu biểu.

PHẠM VI: trong outline, từ mục "PHỤ LỤC 5. Trích đoạn mã nguồn tiêu biểu" đến hết mục "P5.9 TEST ĐỒNG THỜI GIỮ GHẾ" (khoảng dòng 5603–5679). Chỉ viết đúng phạm vi này.

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- Các file nguồn được nêu ở từng mục P5.1–P5.9

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. Mỗi trích đoạn ≤ 20 dòng, chép đúng từ file hiện tại, chỉ rút gọn bằng `// ...` có chủ đích, không đổi logic, không chứa khóa/bí mật; kèm đường dẫn file và hàm.
2. Mỗi đoạn có 3–5 câu giải thích; caption "Đoạn mã P5.n".

ĐẦU RA: duy nhất file `35-phu-luc-5-trich-doan-ma.md`, tuân theo mục ĐẦU RA của prompt nền.
````

### PHẦN 36 — Tóm tắt và rà soát Danh mục A–F (làm cuối cùng)

````text
Thực hiện PHẦN 36: Tóm tắt và rà soát Danh mục A–F (làm cuối cùng).

PHẠM VI: trong outline, từ "TÓM TẮT" đến hết "TÓM TẮT" (dòng 62–74); và từ "DANH MỤC A. HÌNH VẼ" đến hết "DANH MỤC F. NHÓM TÀI LIỆU THAM KHẢO CẦN TÌM" (dòng 5680–6088). Chỉ viết đúng phạm vi này.
SỐ HIỆU PHẢI CÓ: Danh mục A–F (rà soát toàn bộ).

ĐÍNH KÈM / NGUỒN CẦN ĐỌC:
- Mã nguồn dự án `perfo-main/` (AI phải đọc được file thật)
- `outline-dan-y-bao-cao.md` (đọc đúng phạm vi trên; các mục khác chỉ để tham chiếu chéo) và `quy-dinh-trinh-bay.md`
- TOÀN BỘ các file `01`–`35` đã hoàn thành

YÊU CẦU RIÊNG CỦA PHẦN NÀY:
1. LÀM SAU CÙNG, khi đã có nội dung các phần khác.
2. Tóm tắt 1 trang (bài toán, giải pháp, kết quả định lượng chỉ lấy từ Chương 6, kết luận và hạn chế, từ khóa 4–6 từ) + Abstract nếu thực hiện được.
3. Rà soát Danh mục A–F: đối chiếu từng số hiệu hình/bảng/biểu đồ/công thức và từ viết tắt với báo cáo đã viết; liệt kê chỗ thiếu/trùng/lệch số hiệu và cập nhật Trạng thái.
4. Cuối file gom thành một checklist duy nhất toàn bộ `[CẦN BỔ SUNG]` còn lại của cả báo cáo, nhóm theo chương.

ĐẦU RA: duy nhất file `36-tom-tat-ra-soat-danh-muc.md`, tuân theo mục ĐẦU RA của prompt nền.
````

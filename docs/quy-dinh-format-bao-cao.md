# QUY ĐỊNH FORMAT BÁO CÁO ĐỒ ÁN MÔN HỌC — HỆ THỐNG ĐẶT VÉ XEM PHIM TRỰC TUYẾN PERFO

> Tách từ mục 0 của file outline gốc. Nội dung dàn ý các chương nằm ở file `outline-dan-y-bao-cao.md`.

**Quy ước:** `[CẦN BỔ SUNG: …]` = chưa có dữ liệu trong project hoặc tài liệu đã cung cấp. Đường dẫn nguồn tính từ `perfo-main/`; tài liệu kiểm thử bổ sung ghi `bo_sung/…`. Số hiệu hình, bảng, biểu đồ, công thức theo dạng `Hình X.n`, `Bảng X.n`, `Biểu đồ X.n`, `(X.n)` với X là số chương (phụ lục: `P1.n`, `P2.n`…). Mỗi mục chỉ có tối đa một hình; mã nguồn sơ đồ (PlantUML cho use case và sơ đồ lớp, Mermaid cho ERD và các sơ đồ khác) nằm trong file `ma-nguon-so-do.md`, tra theo số hiệu hình.

---

## 0. CẤU HÌNH FILE WORD

### 0.1. Thiết lập trang, section

| Thông số | Giá trị theo quy định | Ghi chú khi dựng Word |
|---|---|---|
| Khổ giấy | A4 đứng; bảng/hình lớn có thể dùng A4 ngang | Dùng section ngang riêng cho ERD đầy đủ và các bảng rộng |
| Số trang | Khoảng 30–80 trang, in một mặt | — |
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

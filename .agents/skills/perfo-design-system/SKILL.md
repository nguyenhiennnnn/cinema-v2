---
name: perfo-design-system
description: Design system riêng của Perfo (hệ thống đặt vé xem phim, React 19 + Tailwind v4 + shadcn/base-ui). Dùng skill này mỗi khi viết, sửa hoặc review bất kỳ UI nào trong frontend/ - trang client, trang admin, component, màu, chữ, form, bảng, sơ đồ ghế, vé điện tử, trạng thái trống/lỗi, copy giao diện, hoặc khi người dùng nhắc tới "thiết kế", "giao diện", "style", "đẹp hơn", "nhìn như AI". Dùng cả khi chỉ sửa một nút hay một màu, vì mọi thay đổi UI phải theo hệ thống này.
---

# Perfo design system

Perfo lấy tên từ perforation, đường đứt nét trên tấm vé. Giao diện là tấm vé bị xé làm hai phần: phần bạn giữ lại và phần bạn đưa đi. Mọi quyết định hình khối đều xuất phát từ động tác đó, không từ xu hướng giao diện chung.

Quy trình khi làm UI:
1. Đọc mục 1 đến 4 của file này.
2. Làm component cụ thể (ghế, vé, suất chiếu, bảng admin, form) thì đọc `references/components.md`.
3. Cần token hoặc utility thì lấy từ `references/tokens.css`. Không tự chế giá trị.
4. Trước khi báo xong, đối chiếu `references/anti-patterns.md` và checklist ở mục 8.

## 1. Ý tưởng: đường xé

Ba motif, và chỉ ba. Mỗi motif có việc riêng, không dùng làm trang trí.

| Motif | Hình | Dùng khi | Class |
|---|---|---|---|
| Vát góc | Cắt góc 45°, thay cho bo tròn | Mọi khối có hình (nút, chip, thẻ phim, ô ghế) | `cut-xs/sm/md/lg`, `cut-flip` |
| Khấc | Bán nguyệt cắt vào cạnh | Chỉ ở chỗ nội dung thật sự tách đôi: vé điện tử, dòng suất chiếu có cuống | `notch-x`, `notch-y` |
| Đục lỗ | Hàng chấm | Chỉ làm đường tách giữa phần "giữ lại" và phần "đưa đi" | `perf-h`, `perf-v` |

Ngân sách: mỗi màn hình tối đa một vùng dùng khấc hoặc đục lỗ. Vát góc dùng rộng rãi. Dải lỗ phim 35mm chỉ dùng trên banner và ấn phẩm, không đưa vào UI.

## 2. Nguyên tắc

1. **Phẳng và đặc.** Khối màu phẳng, không viền mảnh, không bóng, không kính, không gradient. Phân cấp bằng bậc tông (`background`, `card`, `secondary`) và khoảng trắng.
2. **Số là nhân vật chính.** Giờ chiếu, giá, mã ghế, tổng tiền dùng chữ số lớn, hẹp, đều nét (`t-numeral`). Tiêu đề phim đứng dưới poster, không đè lên poster.
3. **Nội dung thật lên đầu.** Trang chủ client mở bằng lịch chiếu hôm nay và poster, không bằng khối giới thiệu. Không có hero căn giữa kèm ba thẻ tính năng.
4. **Căn trái, không đối xứng máy móc.** Bố cục lệch, cột chính rộng, cột phụ hẹp. Căn giữa chỉ dùng cho trạng thái trống và vé.
5. **Một điểm nhấn mỗi màn hình.** Một khối đỏ `primary` đặc (CTA chính hoặc cuống vé). Phần còn lại im.
6. **Trạng thái phải đọc được không cần màu.** Mỗi trạng thái có hình riêng (ghế, huy hiệu), màu chỉ bổ trợ.
7. **Tiếng Việt là ưu tiên một.** Dấu chồng (Ế, Ặ, Ữ) không được cắt: tiêu đề `line-height` từ 1.05 trở lên, kiểm tra mọi font mới bằng chuỗi "Ếch ở Phòng Chiếu Ữ".

## 3. Token

Giá trị đầy đủ nằm trong `references/tokens.css`. Không hardcode HEX, RGB, OKLCH trong component; chỉ dùng class semantic.

### Màu

Nền trung tính ấm như phòng chiếu và nhung ghế: kem sáng ở light, nâu đỏ ink ở dark. Điểm nhấn là đỏ nhung và vàng ánh kim. Không đi cùng chữ serif hay màu đất nung.

| Token | Light | Dark | Việc |
|---|---|---|---|
| `background` | `oklch(0.965 0.012 80)` | `oklch(0.12 0.014 30)` | Nền trang |
| `card` | `oklch(0.99 0.006 80)` | `oklch(0.18 0.02 30)` | Khối nổi một bậc (thẻ, dialog) |
| `secondary` / `muted` | `oklch(0.925 0.014 75)` | `oklch(0.22 0.018 30)` | Khối tách khỏi nền (input, ô ghế thường, hàng bảng xen kẽ) |
| `foreground` | `oklch(0.2 0.02 30)` | `oklch(0.95 0.01 80)` | Chữ chính |
| `muted-foreground` | `oklch(0.48 0.025 50)` | `oklch(0.7 0.02 60)` | Chữ phụ |
| `primary` | `oklch(0.45 0.19 25)` | `oklch(0.56 0.21 25)` | Đỏ nhung Perfo: CTA, ghế đang chọn, cuống vé |
| `accent` | `oklch(0.72 0.15 82)` | `oklch(0.72 0.15 82)` | Vàng ánh kim: ghế VIP, sao đánh giá |
| `border` | `oklch(0.87 0.014 75)` | `oklch(0.28 0.02 30)` | Chỉ cho đường kẻ bảng và đáy input, không bao khối |
| `destructive` | `oklch(0.55 0.22 30)` | `oklch(0.62 0.22 30)` | Hủy vé, xóa dữ liệu, lỗi |
| `success` | `oklch(0.52 0.12 155)` | `oklch(0.64 0.14 155)` | Đã thanh toán, thành công |
| `warning` | `oklch(0.7 0.15 70)` | `oklch(0.74 0.15 70)` | Chờ thanh toán, sắp hết hạn |
| `couple` | `oklch(0.52 0.15 350)` | `oklch(0.64 0.15 350)` | Ghế đôi |
| `held` | `oklch(0.7 0.15 62)` | `oklch(0.74 0.15 62)` | Ghế đang giữ |

`vip` trỏ tới `accent`. Mọi token màu đều nằm trong `references/tokens.css`, thay toàn bộ giá trị cũ trong project.

Quy tắc dùng màu:
- Chữ trên `primary` luôn là `primary-foreground`.
- Chữ vàng trên nền sáng không đạt độ tương phản: vàng chỉ làm nền cho chip có chữ tối, hoặc làm icon kèm nhãn.
- Không dùng quá hai sắc độ đỏ trên một màn hình.
- `primary` và `destructive` cùng họ đỏ: không đặt cạnh nhau, phân biệt bằng icon và nhãn.

### Chữ

| Vai trò | Font | Thiết lập |
|---|---|---|
| Tiêu đề, số | Archivo Variable (trục `wdth`) | Hẹp (`wdth` 75 đến 85) cho display và số, thường (`wdth` 100) cho H2 trở xuống. **Viết câu thường, không viết hoa cả chuỗi.** |
| Nội dung | Be Vietnam Pro 400, 500, 600 | 15px, `line-height` 1.55 |
| Mã vé, mã ghế | Archivo `wdth` 80, `tabular-nums`, tracking 0.04em | Không dùng font monospace |

Thang chữ (class có sẵn trong `tokens.css`): `t-display` 72, `t-h1` 40, `t-h2` 28, `t-h3` 20, `t-numeral` (kích thước tự đặt), `t-code`. Nội dung 15, chú thích 13. Không có cỡ dưới 13px.

### Hình khối và khoảng cách

- `--radius: 0`. Không `rounded-*` trên thẻ, nút, input, dialog. Chỉ `rounded-full` cho avatar.
- Vát: `cut-xs` 4px (chip), `cut-sm` 6px (nút, input), `cut-md` 12px (thẻ), `cut-lg` 20px (dialog, vé).
- Khoảng cách theo nhịp 4: 4, 8, 12, 16, 24, 32, 48, 64, 96. Quanh dữ liệu dày (bảng, sơ đồ ghế) dùng 4 đến 12. Quanh poster và tiêu đề dùng 32 đến 96.
- Icon: `lucide-react` với nét vuông (đã ép bằng CSS trong `tokens.css`), kích thước 16, 20, 24. Icon riêng của Perfo trong `perfo-brand/icons/` ưu tiên hơn khi có icon tương ứng.

## 4. Bố cục theo vai trò

**Client (khán giả).** Nền `background`, khối `card`. Poster tỉ lệ 2:3, không viền, không overlay gradient. Lưới poster 2 cột mobile, 4 đến 6 cột desktop, khoảng cách 16. Lịch chiếu là danh sách dòng, mỗi dòng một suất: số giờ lớn bên trái, thông tin ở giữa, cuống đặt vé bên phải (đây là chỗ duy nhất được dùng khấc kèm đục lỗ). Header nền đặc `background`, không blur, không đường kẻ mảnh.

**Admin (vận hành).** Nền `background`, thanh bên tối (`sidebar`) cả ở light mode. Mật độ cao, bảng là trung tâm. Mục đang chọn là một khối `primary` đặc phủ hết chiều ngang nhãn, không dùng thanh viền trái, không đánh số nhóm. Thẻ số liệu là một khối `card` có số lớn `t-numeral` và nhãn câu thường bên dưới, không viền nhấn bên trái, không biểu tượng tròn.

## 5. Chuyển động

- Mặc định không có chuyển động vào trang. Không fade-up theo section, không stagger, không hover nâng thẻ.
- Chuyển động chỉ để trả lời thao tác: đổi trạng thái ghế, mở dialog, chọn ngày. 120 đến 160ms, `ease-out`.
- Một khoảnh khắc đặc trưng duy nhất: **xé vé** khi đặt vé thành công. Cuống vé tách ra dọc đường đục lỗ rồi lệch xuống 16px, xoay 4°, 420ms (`.tear-stub[data-torn="true"]`). Không dùng ở nơi khác.
- Tôn trọng `prefers-reduced-motion`: bỏ chuyển động, giữ trạng thái cuối.
- Skeleton dùng nhấp nháy độ mờ (opacity), không dùng dải sáng chạy ngang.

## 6. Giọng chữ

- Câu thường, động từ rõ ràng: "Đặt vé", "Giữ ghế", "Đổi suất chiếu". Không viết hoa cả nhãn, không nhãn nhỏ đặt phía trên tiêu đề.
- Một hành động giữ một tên xuyên suốt: nút "Giữ ghế" thì toast là "Đã giữ ghế".
- Dùng đúng thuật ngữ rạp: Suất chiếu, Phòng chiếu, Đặt vé, Bắp nước, Ghế VIP, Ghế đôi.
- Lỗi nói rõ chuyện gì xảy ra và cách sửa, không xin lỗi chung chung: "Ghế H12 vừa có người đặt. Chọn ghế khác trong sơ đồ."
- Trạng thái trống là lời mời hành động: "Chưa có suất chiếu hôm nay. Chọn ngày khác." Kèm một nút.
- Chỉ nhắc "xé", "cuống" ở slogan và lúc đặt vé thành công. Không dùng trong lỗi.
- Không nối chuỗi bằng dấu chấm giữa ("A · B · C"), không mũi tên cuối nút hay liên kết, không emoji, không gạch ngang dài ngăn đôi câu.

## 7. Kỹ thuật

- Tailwind v4 và shadcn (base-nova): chỉ đổi token và thêm utility, không viết lại component. Giá trị thay trong `index.css` đã nằm sẵn ở `references/tokens.css`.
- Font: cài `@fontsource-variable/archivo` và `@fontsource/be-vietnam-pro`, thay import Oswald và Geist. Nếu import `wdth.css` không tồn tại ở phiên bản cài về, dùng `full.css`. Kiểm tra bằng `npm ls`.
- `clip-path` của lớp `cut-*` cắt luôn outline ngoài. Focus phải dùng outline lệch vào trong (đã có trong `tokens.css`). Không đặt `cut-*` lên phần tử cần `box-shadow` hoặc tooltip tràn ra ngoài.
- Khấc dùng `mask`, nên phần tử đó không có viền. Nội dung bên trong không được chạm sát mép khấc: chừa tối thiểu `--notch` + 8px.
- Luôn ghép class bằng `cn(...)` từ `@/lib/utils`. Tiền tệ dùng `@/lib/formatters` (120.000 ₫).
- Kiểm tra độ tương phản: chữ thường từ 4.5:1, chữ lớn và icon từ 3:1. Vùng bấm tối thiểu 44px trên mobile.

## 8. Checklist trước khi báo xong

- [ ] Không có HEX, RGB, OKLCH nằm trong component.
- [ ] Không có `rounded-*` (trừ avatar), `shadow-*`, `backdrop-blur`, `bg-gradient-*`.
- [ ] Không có nhãn viết hoa cả chuỗi, không số thứ tự 01 02 03, không mũi tên cuối nút.
- [ ] Màn hình có tối đa một vùng khấc hoặc đục lỗ, tối đa một khối `primary` đặc.
- [ ] Mọi trạng thái có hình riêng ngoài màu (ghế, huy hiệu, bảng).
- [ ] Focus nhìn thấy được trên nút có `cut-*`.
- [ ] Tiêu đề tiếng Việt có dấu chồng không bị cắt.
- [ ] Light và dark đều đã xem. Mobile 375px đã xem.
- [ ] Chạy `references/anti-patterns.md` từ trên xuống, không dòng nào vi phạm.

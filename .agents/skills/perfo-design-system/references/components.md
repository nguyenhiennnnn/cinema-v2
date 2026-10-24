# Thông số component Perfo

Mục lục: Nút · Input · Huy hiệu · Thẻ phim · Dòng suất chiếu · Thanh chọn ngày · Sơ đồ ghế · Vé điện tử · Dialog và toast · Trạng thái trống và lỗi · Admin

Tất cả dùng class semantic. Ví dụ dưới đây là hướng dẫn cấu trúc, không phải đoạn dán nguyên.

## Nút
- Chính: `bg-primary text-primary-foreground cut-sm h-11 px-5 font-medium`. Mỗi màn hình một nút chính.
- Phụ: `bg-secondary text-secondary-foreground cut-sm`, không viền.
- Nhẹ: chữ `text-foreground`, gạch chân 2px khi hover, không nền.
- Nguy hiểm: `bg-destructive`, chỉ cho hủy vé và xóa dữ liệu.
- Không pill, không bóng, không mũi tên cuối chữ. Hai trạng thái đang tải: giữ nguyên chiều rộng, thay nhãn bằng "Đang giữ ghế".

## Input
- Nền `bg-secondary`, không bo, không viền bao. Đáy có đường 2px `border-input`; khi focus đường đáy chuyển `bg-primary`.
- Nhãn đặt phía trên, câu thường 13px, không dùng placeholder làm nhãn.
- Lỗi: đường đáy `destructive` và dòng chữ giải thích bên dưới, kèm icon. Không viền đỏ bao cả ô.

## Huy hiệu
- Hình `cut-xs`, chữ 13px đậm, không `rounded-full`.
- Độ tuổi: ô vuông 28px một chữ (P, T13, T16, T18, K), nền đặc: P `success`, T13 và T16 `warning`, T18 và K `destructive`. Mỗi mức tuổi một chữ, không chỉ khác màu.
- Trạng thái đơn đặt vé và thanh toán: icon nhỏ cộng chữ ("Đã thanh toán" với dấu tích, "Chờ thanh toán" với đồng hồ, "Đã hủy" với dấu gạch chéo).

## Thẻ phim
- Poster 2:3, `cut-md`, không viền, không overlay.
- Dưới poster: tên phim `t-h3` tối đa 2 dòng, một dòng thể loại và thời lượng chữ phụ 13px. Điểm đánh giá là số `t-numeral` kèm sao vàng.
- Hover: chỉ đổi tên phim sang gạch chân. Không scale ảnh, không nâng thẻ.
- Nút "Mua vé" không hiện đè lên poster; đặt trong trang chi tiết hoặc dưới thẻ.

## Dòng suất chiếu (chỗ dùng khấc và đục lỗ chính)
```
[ 19:45 ]  Phòng 3, IMAX, phụ đề         | cuống: 120.000 ₫
 số lớn     còn 42 ghế                   ¦  [Đặt vé]
```
- Dựng thành **một khối duy nhất**, không ghép hai khối rời: container `relative grid grid-cols-[1fr_200px] bg-card cut-lg notch-y` với `--x: calc(100% - 200px)`. Khấc nằm giữa cạnh trên và cạnh dưới, đúng vị trí đường xé. (Ghép hai khối rời làm khấc rơi vào góc vát và bị che mất.)
- Thân (cột trái): số giờ `t-numeral` cỡ 40, thông tin phòng và ngôn ngữ bên cạnh.
- Cuống (cột phải): nền `bg-primary text-primary-foreground`, giá `t-numeral` 24, nút "Đặt vé" nền `primary-foreground` chữ `primary`.
- Đường đục lỗ: một phần tử `perf-v` đặt `absolute` tại `left: calc(100% - 202px)`, cách mép trên và dưới 18px, màu `primary-foreground` để nhìn thấy trên nền đỏ.
- Suất hết ghế: cuống chuyển `bg-secondary`, chữ "Hết ghế", không có nút.
- Mobile: xếp cuống xuống dưới thân, đổi sang `perf-h` và `notch-x` (`--y` bằng chiều cao thân).

## Thanh chọn ngày
- Mỗi ngày là một ô `cut-sm` rộng 56px: số ngày `t-numeral` 28, thứ viết thường 13px bên dưới.
- Đang chọn: `bg-primary text-primary-foreground`. Hôm nay có gạch dưới 2px khi chưa chọn.
- Cuộn ngang trên mobile, không có nút mũi tên, không có bóng mờ hai đầu.

## Sơ đồ ghế
- Ô ghế dùng class `.seat` trong `tokens.css`, kèm `data-state` (`available`, `selected`, `held`, `booked`) và `data-type` (`vip`, `couple`).
- Hình, không chỉ màu: đang chọn là khối đặc đỏ kèm mã ghế; đang giữ có sọc chéo; đã bán có dấu gạch chéo; VIP có vạch vàng trên đầu; ghế đôi rộng gấp đôi.
- Màn hình: một thanh `bg-foreground` cao 6px rộng bằng sơ đồ, `cut-md`, nhãn "Màn hình" dưới thanh. Không đường cong, không ánh sáng.
- Hàng ghế có chữ cái ở hai đầu. Lối đi là khoảng trắng, không đường kẻ.
- Mỗi ô có `aria-label` ("Ghế H12, đã bán"), điều hướng bằng phím mũi tên, `aria-pressed` cho ghế đang chọn.
- Chú giải bốn trạng thái đặt phía trên sơ đồ, dùng chính các ô mẫu.
- Realtime (SignalR): khi ghế đổi trạng thái do người khác, ô đổi hình ngay, không hiệu ứng.

## Vé điện tử
- Thân vé `bg-card cut-lg`, cuống `bg-primary cut-lg cut-flip tear-stub`.
- Ngăn cách bằng `perf-v` (desktop) hoặc `perf-h` (mobile) và khấc đúng vị trí đó.
- Thân: tên phim `t-h1`, giờ chiếu `t-numeral` 56, phòng và ghế `t-code`. Cuống: mã QR nền `primary-foreground`, mã đặt vé `t-code` bên dưới.
- Đặt vé thành công: `data-torn` chuyển `true` đúng một lần, sau đó giữ nguyên. Đây là chỗ duy nhất có chuyển động trang trí.
- Tham chiếu hình dạng: `perfo-brand/template/perfo-ticket-template.svg`.

## Dialog và toast
- Dialog: `bg-card cut-lg`, nền che `bg-foreground/60` đặc, không blur. Tiêu đề `t-h2`. Nút chính bên phải, nút nhẹ bên trái.
- Toast: góc dưới trái, `bg-foreground text-background cut-sm`, tự tắt sau 5 giây, có nút "Hoàn tác" khi hành động hoàn tác được.

## Trạng thái trống và lỗi
- Trống: căn giữa, một câu nói việc cần làm, một nút. Không minh họa vui nhộn, không emoji.
- Lỗi tải: "Không tải được lịch chiếu. Kiểm tra kết nối rồi thử lại." kèm nút "Thử lại".
- Lỗi biểu mẫu: nêu trường nào, sai gì, sửa thế nào.
- Hết hạn giữ ghế: "Hết thời gian giữ ghế. Chọn lại ghế để tiếp tục." Nút "Chọn lại ghế".

## Admin
- Thanh bên: nền `sidebar`, nhãn nhóm câu thường 13px màu `muted-foreground`, mục là chữ 15px. Mục đang chọn: khối `sidebar-primary` đặc phủ hết chiều ngang, chữ `sidebar-primary-foreground`.
- Bảng: không viền bao, hàng xen kẽ `bg-secondary/50`, tiêu đề cột dính trên cùng nền `card`, chữ đậm 13px câu thường. Đường kẻ `border` chỉ giữa tiêu đề và thân. Số căn phải và dùng `tabular-nums`.
- Thẻ số liệu: khối `card cut-md`, số `t-numeral` 44, nhãn bên dưới. So sánh kỳ trước viết thành chữ ("tăng 12% so với tuần trước"), không mũi tên màu.
- Form dài: chia nhóm bằng khoảng trắng 32px và tiêu đề `t-h3`, không bọc từng nhóm trong thẻ.
- Hành động trên bảng: nút nhẹ "Sửa" và menu "Thêm" cuối hàng, hàng loạt hiện thanh hành động dính ở đáy khi có hàng được chọn.

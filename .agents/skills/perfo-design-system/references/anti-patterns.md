# Danh sách cấm: những dấu hiệu khiến giao diện trông như AI làm

Đọc từ trên xuống trước khi báo xong. Mỗi dòng có cách thay thế.

## Màu và bề mặt
| Cấm | Thay bằng |
|---|---|
| Nền kem kèm chữ serif và màu đất nung | Nền kem `background` của Perfo, chữ Archivo và Be Vietnam Pro, đỏ nhung và vàng ánh kim |
| Nền đen kèm một màu neon (xanh lá, cam đỏ) | Nền ink nâu đỏ `background` dark, điểm nhấn chính là đỏ `primary`, vàng `accent` chỉ cho VIP và sao |
| `primary` và `destructive` đặt cạnh nhau | Tách bằng icon và nhãn, mỗi màn hình một khối đỏ đặc |
| Gradient trang trí, chữ gradient, dải sáng skeleton | Khối màu phẳng, skeleton nhấp nháy opacity |
| Kính mờ, `backdrop-blur`, viền phát sáng, glow | Nền đặc, nền che `foreground/60` |
| Bóng đổ mềm `shadow-sm/md/lg/xl` | Khác biệt bậc tông giữa `background`, `card`, `secondary` |
| Viền hairline bao mọi khối | Bỏ viền. Chỉ dùng `border` làm đường kẻ bảng và đáy input |

## Hình khối và bố cục
| Cấm | Thay bằng |
|---|---|
| Thẻ bo tròn lớn giống hệt nhau xếp lưới | Khối vát góc có kích thước khác nhau theo vai trò (`cut-sm` đến `cut-lg`) |
| Nút pill, chip tròn, `rounded-xl` trở lên | `cut-sm` và `cut-xs`, chỉ avatar mới `rounded-full` |
| Hero căn giữa kèm ba thẻ tính năng | Lịch chiếu hôm nay và poster ngay đầu trang |
| Dải số liệu to với nhãn nhỏ phía dưới như mặc định | Chỉ dùng khi chính số liệu là nội dung (giá, giờ chiếu, doanh thu) |
| Thẻ số liệu có viền nhấn bên trái | Khối `card` số lớn, nhãn bên dưới |
| Mục menu đang chọn dạng viên thuốc hoặc thanh viền trái | Khối `sidebar-primary` đặc phủ hết chiều ngang |
| Layout cột đều nhau đối xứng cho mọi trang | Cột chính rộng, cột phụ hẹp, căn trái |

## Chữ và nhãn
| Cấm | Thay bằng |
|---|---|
| Nhãn nhỏ viết hoa, giãn chữ, đặt phía trên tiêu đề (eyebrow) | Bỏ nhãn, hoặc đưa thành câu thường nếu thật cần |
| Tiêu đề viết hoa cả chuỗi | Câu thường, `t-h1` đến `t-h3` |
| Đánh số 01, 02, 03 cho nhóm không phải trình tự | Bỏ số. Chỉ đánh số khi nội dung thật sự là các bước |
| Font monospace cho nhãn dữ liệu nhỏ | `t-code` (Archivo hẹp, `tabular-nums`) |
| Nối chuỗi bằng dấu chấm giữa ("2D · IMAX · 120 phút") | Ngăn cách bằng khoảng trắng và hai thuộc tính trên hai dòng, hoặc dấu phẩy |
| Nhãn dạng "TỪ KHÓA — mô tả" với gạch dài | Hai thành phần riêng: nhãn và mô tả |
| Mũi tên → cuối nút hoặc liên kết | Nhãn hành động rõ ràng, không ký hiệu |
| Nhấn một từ trong tiêu đề bằng màu hoặc in nghiêng | Tiêu đề một kiểu chữ duy nhất |

## Hình ảnh và icon
| Cấm | Thay bằng |
|---|---|
| Emoji làm icon hoặc trang trí | Icon `lucide` nét vuông hoặc icon Perfo |
| Avatar mặc định bằng gradient hoặc chữ cái trong vòng màu | Ô vuông `secondary` với chữ cái đầu |
| Minh họa 3D, hình khối trôi nổi, blob | Poster phim thật, hoặc không hình |
| Biểu đồ có gradient nền dưới đường | Đường hoặc cột phẳng một màu, trục chữ rõ |

## Chuyển động
| Cấm | Thay bằng |
|---|---|
| Fade-up lần lượt cho từng section hay thẻ | Không có chuyển động vào trang |
| Hover nâng thẻ, phóng to ảnh | Hover đổi gạch chân hoặc đổi tông |
| Spinner xoay ở giữa trang cho mọi lần tải | Skeleton đúng hình khối nội dung |

## Giọng chữ
| Cấm | Thay bằng |
|---|---|
| "Chào mừng trở lại 👋", "Khám phá ngay", "Trải nghiệm liền mạch", "Nâng tầm" | Nói việc cần làm: "Đăng nhập", "Xem lịch chiếu" |
| Lỗi chung chung "Đã xảy ra lỗi" | Nói lỗi gì và cách sửa |
| Xin lỗi dài dòng trong thông báo lỗi | Một câu nêu nguyên nhân, một câu nêu cách sửa |
| Slogan "xé vé" trong thông báo lỗi | Chỉ dùng ở slogan và lúc đặt vé thành công |

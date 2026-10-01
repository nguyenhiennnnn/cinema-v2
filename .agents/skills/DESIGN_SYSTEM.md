# Cinema Project Design System & Component Guidelines

> File này được đồng bộ với `.agents/skills/cinema-design-system/SKILL.md` để làm hướng dẫn và skill cho AI & Developers.

---

## 0. Bản sắc thiết kế — "Vé giấy & Marquee rạp chiếu"

Hệ thống **không** đi theo mô-típ SaaS/AI-generated phổ biến (glassmorphism, gradient rực rỡ, card bo tròn hàng loạt, bóng đổ mềm phát sáng). Thay vào đó, ngôn ngữ thị giác lấy cảm hứng từ **vật phẩm vật lý của rạp chiếu**: vé giấy in, bảng hiệu marquee, sổ chương trình, lỗ phim 35mm.

Hai "giọng" phân biệt rõ theo vai trò người dùng:

- **Client (khán giả)** — ấm áp, như giấy in, poster-forward. Light mode = giấy kem phòng vé; Dark mode = trong rạp, đèn tắt, marquee đỏ/vàng nổi bật trên nền mực đen ấm.
- **Admin (vận hành)** — sidebar mực đen thật (không dùng lại tông ấm của client), đánh số nhóm menu kiểu monospace, thanh active bên trái thay vì pill bo tròn — cảm giác "phòng điều khiển", tách bạch khỏi không khí khán giả.

Nguyên tắc hình khối bắt buộc:

1. **Không bo góc kiểu app-bubble.** `--radius` gốc đã giảm còn `0.3rem` — mọi `rounded-*` cascade tự động sang sắc hơn. Không thêm `rounded-xl`/`rounded-2xl` tuỳ tiện ở component mới; dùng class `rounded-*` mặc định của hệ thống, nó đã được tinh chỉnh.
2. **Hairline border thay cho shadow mềm.** Ưu tiên `border border-border` để tạo phân cấp; shadow (`shadow-sm/md/lg`) giờ là bóng cứng, phẳng (kiểu giấy xếp lớp), không dùng để tạo cảm giác "nổi lên" kiểu kính.
3. **Không dùng `backdrop-blur` cho thanh điều hướng chính** (header/topbar) — dùng nền đặc + `.sprocket-rule` làm đường phân cách.
4. **Chỉ dùng `rounded-full`/pill cho chip/tag/badge thật sự** (age rating, trạng thái, avatar) — không dùng cho card, dialog, button lớn.

### Motif riêng (chỉ dùng đúng chỗ, không lạm dụng)

| Class                             | Ý nghĩa                                        | Dùng ở đâu                                                                                               |
| --------------------------------- | ---------------------------------------------- | -------------------------------------------------------------------------------------------------------- |
| `.ticket-notch`                   | Khấc tròn cắt vào hai cạnh, mô phỏng lỗ bấm vé | Thẻ vé điện tử (`BookingResultTickets`)                                                                  |
| `.perforation` / `.perforation-v` | Đường đứt nét mô phỏng đường xé vé             | Ranh giới giữa phần "cuống" và "thân" vé, giữa các dòng tổng tiền                                        |
| `.sprocket-rule`                  | Dải chấm ngang mô phỏng lỗ phim 35mm           | Đường phân cách header/footer thay cho border mờ                                                         |
| `.clip-corner`                    | Cắt một góc, như vé xé                         | Chỉ dùng cho CTA chính nổi bật nhất trên mỗi màn hình (hero "Mua vé ngay") — không lạm dụng trên mọi nút |

### Typography

- **Heading / marquee**: `font-heading` → Oswald Variable (condensed, có subset tiếng Việt đầy đủ dấu). Dùng cho H1–H3, tiêu đề phim, tên section, số tiền lớn. Thường đi kèm `uppercase tracking-tight`.
- **Body / UI**: `font-sans` → Geist Variable (giữ nguyên, đảm bảo đọc tốt tiếng Việt).
- **Số liệu / mã / kỹ thuật**: `font-mono` → Geist Mono. Dùng cho mã đặt vé, mã ghế, ID admin, timestamp.
- **Eyebrow label**: `.text-eyebrow` (uppercase, tracking rộng, cỡ nhỏ) — dùng cho nhãn phụ phía trên tiêu đề lớn.

---

## 1. Tổng quan Tech Stack Frontend

- **Framework**: React 19 + TypeScript + Vite
- **CSS Framework**: Tailwind CSS v4 (`@tailwindcss/vite`, `@import "tailwindcss"`, `tw-animate-css`)
- **Primitive UI**: `@base-ui/react` + Shadcn (style: `base-nova`)
- **Typography**: `@fontsource-variable/geist` (body), `@fontsource-variable/oswald` (heading/marquee)
- **Icons**: `lucide-react`
- **Theme Engine**: OKLCH Color Space (Hỗ trợ Dark Mode chủ đạo & Light Mode)
- **State & Query**: Zustand, TanStack Query v5, Nuqs (URL state), React Router v7

---

## 2. Hệ thống Semantic Tokens & Màu sắc (OKLCH)

### 2.1 Bảng màu chính

| Token                        | Mục đích                                                                       | Class Tailwind                                  |
| :--------------------------- | :----------------------------------------------------------------------------- | :---------------------------------------------- |
| `primary`                    | Đỏ marquee rạp chiếu, bão hoà hơn bản gốc. Nút đặt vé, tab active, ghế đã chọn | `bg-primary`, `text-primary`, `border-primary`  |
| `primary-foreground`         | Màu chữ trên nền primary (Sáng/Kem)                                            | `text-primary-foreground`                       |
| `secondary`                  | Nền phụ, nút lọc, ghế thường                                                   | `bg-secondary`, `text-secondary-foreground`     |
| `accent`                     | Vàng gold cuống vé. Ghế VIP, sao đánh giá, thẻ VIP                             | `bg-accent`, `text-accent`, `border-accent`     |
| `background`                 | Nền trang — giấy kem (light) / mực rạp tối (dark)                              | `bg-background`                                 |
| `foreground`                 | Chữ chính                                                                      | `text-foreground`                               |
| `card` / `popover`           | Khối thẻ phim, modal, dropdown, sheet                                          | `bg-card`, `text-card-foreground`, `bg-popover` |
| `muted` / `muted-foreground` | Chữ phụ, ngày giờ, phụ đề, viền mờ                                             | `bg-muted`, `text-muted-foreground`             |
| `destructive`                | Trạng thái lỗi, hủy vé, xóa dữ liệu                                            | `bg-destructive`, `text-destructive`            |
| `border` / `input`           | Viền các thành phần, input form — công cụ phân cấp chính (thay shadow)         | `border-border`, `border-input`                 |

### 2.2 Border Radius Scale

Base radius `--radius: 0.3rem` (đã giảm từ 0.625rem để tránh cảm giác "app bubble"):

- `rounded-sm`: ~0.165rem
- `rounded-md`: ~0.225rem (Nút chuẩn, input)
- `rounded-lg`: 0.3rem (Card nhỏ, dropdown)
- `rounded-xl`: ~0.39rem (Card phim, modal)
- `rounded-2xl`: ~0.51rem (Frame sơ đồ ghế, banner)
- `rounded-4xl` / `rounded-full`: không đổi (Pill badges, status chips, avatar) — đây là ngoại lệ duy nhất được phép "tròn hẳn"

Vì các token `--radius-*` map trực tiếp sang utility `rounded-*` của Tailwind v4, **không cần sửa từng component** — mọi nơi dùng `rounded-lg/xl/2xl` sẵn có đều tự động sắc hơn.

---

## 3. Quy chuẩn Component đặc thù Cinema

### 3.1 Thẻ phim (Movie Card)

- **Tỷ lệ Poster**: `aspect-[2/3]`, `overflow-hidden`, viền hairline `border border-border`, hover đổi `border-primary` (không dùng shadow nổi)
- **Hover**: `group-hover:scale-105 transition-transform duration-500` cho ảnh; nút "Mua vé" hiện ra dùng `.clip-corner`
- **Badges**:
  - Độ tuổi góc trên trái: `P`, `T13`, `T16`, `T18`, `K` (`variant="destructive"` hoặc badge phân loại)
  - Định dạng chiếu: `2D`, `3D`, `IMAX`, `4DX`, `ScreenX`
  - Đánh giá góc trên phải: Icon `<Star className="size-3 fill-accent text-accent" />` + điểm
  - Thời lượng góc dưới trái: `<Clock className="size-3.5" />` + số phút

### 3.2 Vé điện tử (Ticket Stub) — component đặc trưng nhất hệ thống

- Header (cuống vé) nền `bg-primary`, tiêu đề phim `font-heading uppercase`
- Ranh giới cuống/thân: `<div className="ticket-notch perforation" />`
- Thân vé: lưới QR từng ghế + dòng tổng tiền `font-heading text-2xl` màu `text-primary`
- Xem `BookingResultTickets` (`features/client/bookings/components/result/booking-result-tickets.tsx`) làm mẫu tham chiếu

### 3.3 Sơ đồ chọn ghế (Seat Map)

- **Màn hình (Screen)**: dải cong vát nhẹ (`clip-path` hình thang), viền trên `border-primary/80`, nhãn "Màn hình" dùng `.text-eyebrow`
- **Ghế thường (Standard)**: `bg-secondary text-secondary-foreground hover:border-primary/50`
- **Ghế VIP**: `bg-vip/15 border-vip/50 font-semibold`
- **Ghế đôi (Couple)**: `bg-couple/12 border-couple/45`
- **Ghế đang chọn (Selected)**: `bg-primary text-primary-foreground border-primary`
- **Ghế đã bán (Booked)**: mờ, `cursor-not-allowed`
- **Ghế đang giữ (Held)**: `bg-held/20 border-held`

### 3.4 Suất chiếu & Ngày chiếu (Showtime Picker)

- **Dải chọn ngày (Date Strip)**: border hairline, active = `bg-primary text-primary-foreground`, số ngày dùng `font-heading tabular`
- **Pill giờ chiếu**: border hairline, giờ chiếu dùng `font-heading`, giá vé `font-semibold text-primary`

### 3.5 Quản trị (Admin System)

- Mọi trang phải có `<PageHeader title="..." description="..." action={<Button>...</Button>} />` — component này đã mang giọng heading mới (uppercase + border-b), áp dụng đồng loạt cho toàn bộ 26 trang admin
- Sidebar admin: nền mực đen riêng (`--sidebar` tối hơn client), nhóm menu đánh số kiểu mono (`01`, `02`…), active state = thanh viền trái, không dùng pill bo tròn
- Stat card dashboard: viền trái nhấn `border-l-2 border-l-primary`, số liệu `font-heading tabular`
- Dùng DataTable (`@tanstack/react-table`) với cột `Actions` dạng dropdown menu
- Status badges: `MovieStatusBadge`, `ShowtimeStatusBadge`, `BookingStatusBadge`, `PaymentStatusBadge`

---

## 4. AI Coding Guardrails (Nguyên tắc bắt buộc khi code)

1. **Tuyệt đối không hardcode mã màu HEX / RGB** (`bg-[#121212]`, `text-[#e50914]`). Luôn sử dụng token semantic (`bg-background`, `bg-card`, `bg-primary`, `text-foreground`, `text-muted-foreground`).
2. **Luôn dùng `cn(...)` từ `@/lib/utils`** để ghép nối class.
3. **Không tự ý thêm `rounded-xl`/`rounded-2xl`/`backdrop-blur`/shadow mềm** cho component mới — dùng đúng scale/motif đã định nghĩa ở mục 0 và 2.2.
4. **Đồng bộ icon `lucide-react`**: Default là `size-4`, compact là `size-3.5`, hero là `size-5` hoặc `size-6`.
5. **Format tiền tệ**: Dùng `@/lib/formatters` để format VND (`120.000 ₫`).
6. **Tiêu đề section/trang dùng `font-heading uppercase`**, số liệu/tiền lớn cũng dùng `font-heading tabular` để nhất quán giọng marquee.
7. **Tiếng Việt chuẩn giao diện rạp chiếu phim**: Phim đang chiếu, Suất chiếu, Phòng chiếu, Đặt vé, Bắp nước, Ghế VIP, Ghế Sweetbox.

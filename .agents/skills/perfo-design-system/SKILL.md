---
name: cinema-design-system
description: >-
  Comprehensive Design System, UI/UX guidelines, design tokens, and component code patterns
  for the Cinema project (React 19, Tailwind CSS v4, Base UI, Shadcn, OKLCH theme).
  Use this skill whenever creating, updating, or styling UI components, pages, forms,
  movie cards, seat selection maps, showtimes, badges, booking flows, or admin tables to
  ensure 100% aesthetic consistency and code quality.
---

# Cinema Design System & UI/UX Skill Guide

Hệ thống quy chuẩn thiết kế giao diện (Design System) và nguyên tắc lập trình Component dành cho toàn bộ các AI Assistant và lập trình viên trong dự án **Cinema**.

---

## 1. Tech Stack & Nền tảng giao diện

| Thành phần | Công nghệ / Thư viện | Quy chuẩn sử dụng |
| :--- | :--- | :--- |
| **Framework** | React 19 + TypeScript + Vite | Khai báo functional component, typing nghiêm ngặt |
| **Styling Engine** | Tailwind CSS v4 (`@import "tailwindcss"`) | Sử dụng semantic utility classes, `@custom-variant dark` |
| **Animation** | `tw-animate-css` | Smooth transition, modal enter/leave, hover scaling |
| **Component Primitives** | `@base-ui/react` + Shadcn (`base-nova` style) | Base UI primitives thay vì tự viết DOM thô |
| **Typography** | `@fontsource-variable/geist` (`Geist Variable`) | Font sans-serif hiện đại, tối ưu hiển thị số & text |
| **Iconography** | `lucide-react` | Kích thước đồng bộ: `size-4` (default), `size-3.5` (sm), `size-5` (lg) |
| **Color System** | OKLCH Color Space (Light & Dark mode) | Độ tương phản cao, phong cách rạp phim cao cấp |
| **State & Data** | Zustand, TanStack Query v5, Nuqs (URL state) | Đồng bộ trạng thái và cache client-side |

---

## 2. Design Tokens & Bảng màu OKLCH

Dự án sử dụng không gian màu **OKLCH** hiện đại với 2 chế độ:
- **Dark Mode (Chính - Cinema Atmosphere)**: Nền tối đậm điện ảnh (`oklch(0.12 0.014 30)`), card nổi bật (`oklch(0.18 0.02 30)`), điểm nhấn đỏ nhung (`oklch(0.56 0.21 25)`) và vàng ánh kim (`oklch(0.72 0.15 82)`).
- **Light Mode**: Nền kem sáng ấm (`oklch(0.99 0.006 80)`), chữ trầm ấm sắc nét.

### 2.1 Bảng mã Semantic Color Tokens

| Semantic Token | Mục đích sử dụng | Class Tailwind tương ứng |
| :--- | :--- | :--- |
| `--primary` | Màu chủ đạo (Cinema Red/Crimson). Nút hành động chính, active tab, icon nổi bật, ghế đang chọn. | `bg-primary`, `text-primary`, `border-primary` |
| `--primary-foreground` | Màu chữ trên nền primary (Thường là trắng/kem sáng). | `text-primary-foreground` |
| `--secondary` | Màu phụ (Neutral/Slate ấm). Nút phụ, badge sắp chiếu, tag phụ. | `bg-secondary`, `text-secondary-foreground` |
| `--accent` | Màu điểm nhấn vàng kim (Cinema Gold/Amber). Ghế VIP, rating sao, badge giải thưởng, VIP card. | `bg-accent`, `text-accent`, `border-accent` |
| `--background` | Màu nền toàn trang. | `bg-background` |
| `--foreground` | Màu chữ chính của toàn trang. | `text-foreground` |
| `--card` / `--popover` | Màu nền khối nội dung, card phim, modal, dropdown, sheet. | `bg-card`, `text-card-foreground`, `bg-popover` |
| `--muted` / `--muted-foreground` | Màu nền nhạt, placeholder, metadata ngày giờ, mô tả phụ. | `bg-muted`, `text-muted-foreground` |
| `--border` / `--input` | Đường viền ngăn cách, viền input form, viền bảng. | `border-border`, `border-input` |
| `--destructive` | Trạng thái lỗi, cảnh báo, hủy vé, xóa phim, thất bại. | `bg-destructive`, `text-destructive` |
| `--ring` | Viền focus trạng thái bàn phím/accessibility. | `ring-ring`, `focus-visible:ring-ring/50` |

### 2.2 Hình khối: Cut-corner (KHÔNG bo tròn)
Dự án dùng phong cách **vát góc** (`--radius: 0px`). Mọi khối UI phải vuông vức, góc vát bằng utility `cut-*` (định nghĩa trong `index.css`, dùng `clip-path`):
- `cut-xs` (4px): ghế, nút nhỏ, thanh màn hình
- `cut-sm` (6px) / `cut-md` (12px): input, button, card nhỏ
- `cut-lg` (20px): khung lớn (seat map frame, panel)
- `cut-flip`: vát ngược góc, dùng khi cần nhấn hướng

Không dùng `rounded-*` cho card/button/ghế (chỉ avatar, dot trạng thái mới `rounded-full`). Vì dùng `clip-path`, **không dùng `shadow-*`/`outline` bên ngoài** trên phần tử `cut-*` (bị cắt mất), hãy dùng `background`, `::before/::after` hoặc `outline-offset` âm.

---

## 3. Quy chuẩn Typography & Phông chữ

1. **Heading (Tiêu đề trang & Section)**:
   ```tsx
   // Page Title lớn
   <h1 className="font-heading text-2xl md:text-3xl font-bold tracking-tight text-foreground">
     Tiêu đề trang
   </h1>

   // Section Title (Khối danh sách phim, chọn suất chiếu)
   <h2 className="font-heading text-xl md:text-2xl font-semibold tracking-tight text-foreground">
     Phim đang chiếu
   </h2>

   // Sub-heading / Card Title
   <h3 className="font-heading text-base md:text-lg font-medium text-foreground line-clamp-1">
     Tên phim hoặc tiêu đề thẻ
   </h3>
   ```

2. **Body & Metadata Text**:
   - Văn bản nội dung: `text-sm text-foreground leading-relaxed`
   - Mô tả phụ / Phụ đề / Thể loại: `text-xs md:text-sm text-muted-foreground`
   - Nhãn form (Label): `text-sm font-medium text-foreground`
   - Giá tiền & Số ghế: `font-semibold tabular-nums text-primary` hoặc `font-mono font-medium`

---

## 4. Quy chuẩn Component UI cơ bản

Mọi component phải dùng helper `cn(...)` từ `@/lib/utils` và import các primitive từ `@/components/ui/*`.

### 4.1 Buttons (`@/components/ui/button`)
Các variant tiêu chuẩn:
- `default`: Nút đặt vé ngay, xác nhận thanh toán, lưu dữ liệu (`bg-primary text-primary-foreground hover:bg-primary/80`).
- `secondary`: Nút chọn suất phụ, bộ lọc, xem chi tiết (`bg-secondary text-secondary-foreground`).
- `outline`: Nút hủy, quay lại, đổi ngày (`border-border hover:bg-muted`).
- `ghost`: Nút icon trên thanh header, nút close modal (`hover:bg-muted`).
- `destructive`: Nút xóa, hủy vé (`bg-destructive/10 text-destructive hover:bg-destructive/20`).

**Quy tắc Icon trong Button**:
```tsx
import { Button } from "@/components/ui/button";
import { Ticket, Loader2 } from "lucide-react";

// Nút kèm icon trước
<Button variant="default" size="default">
  <Ticket className="size-4" />
  <span>Mua vé ngay</span>
</Button>

// Nút Loading state
<Button disabled>
  <Loader2 className="size-4 animate-spin" />
  <span>Đang xử lý...</span>
</Button>
```

### 4.2 Badges & Status Chips (`@/components/ui/badge`)
Sử dụng cho phân loại độ tuổi, định dạng chiếu, trạng thái vé:
```tsx
import { Badge } from "@/components/ui/badge";

// Độ tuổi: T18 (Đỏ), T16 (Cam/Vàng), P (Xanh)
<Badge variant="destructive" className="font-bold">T18</Badge>
<Badge variant="secondary">2D Phụ đề</Badge>
<Badge variant="outline" className="border-accent text-accent">IMAX Laser</Badge>
```

---

## 5. Quy chuẩn Component đặc thù Cinema (Domain Components)

### 5.1 Thẻ phim (Movie Card)
Chuẩn tỷ lệ Poster phim là **2:3** (`aspect-[2/3]`). Card cần hiệu ứng hover điện ảnh, overlay gradient khi rê chuột.

```tsx
import { Badge } from "@/components/ui/badge";
import { Star, Clock } from "lucide-react";

interface MovieCardProps {
  title: string;
  posterUrl: string;
  ageRating: string;
  formats: string[];
  durationMinutes: number;
  ratingScore: number;
  releaseDate?: string;
  onBookTicket?: () => void;
}

export function MovieCard({
  title,
  posterUrl,
  ageRating,
  formats,
  durationMinutes,
  ratingScore,
  onBookTicket,
}: MovieCardProps) {
  return (
    <div className="group relative flex flex-col overflow-hidden rounded-xl border border-border/50 bg-card transition-all duration-300 hover:-translate-y-1 hover:border-primary/50 hover:shadow-xl hover:shadow-primary/5">
      {/* Poster Image Container */}
      <div className="relative aspect-[2/3] w-full overflow-hidden bg-muted">
        <img
          src={posterUrl}
          alt={title}
          className="h-full w-full object-cover transition-transform duration-500 ease-out group-hover:scale-105"
          loading="lazy"
        />

        {/* Overlay Gradients */}
        <div className="absolute inset-0 bg-gradient-to-t from-black/80 via-black/20 to-transparent opacity-60 transition-opacity group-hover:opacity-80" />

        {/* Top Badges: Age rating & Format */}
        <div className="absolute top-2.5 left-2.5 flex flex-wrap items-center gap-1.5">
          <Badge variant="destructive" className="px-1.5 py-0.5 text-[10px] font-bold tracking-wider uppercase">
            {ageRating}
          </Badge>
          {formats.slice(0, 2).map((fmt) => (
            <Badge key={fmt} variant="secondary" className="bg-black/60 text-[10px] text-white backdrop-blur-md">
              {fmt}
            </Badge>
          ))}
        </div>

        {/* Rating Score Top-Right */}
        {ratingScore > 0 && (
          <div className="absolute top-2.5 right-2.5 flex items-center gap-1 rounded-md bg-black/70 px-2 py-0.5 text-xs font-semibold text-accent backdrop-blur-md">
            <Star className="size-3 fill-accent text-accent" />
            <span>{ratingScore.toFixed(1)}</span>
          </div>
        )}

        {/* Duration bottom badge */}
        <div className="absolute bottom-2.5 left-2.5 flex items-center gap-1 text-xs text-white/90 font-medium drop-shadow">
          <Clock className="size-3.5" />
          <span>{durationMinutes} phút</span>
        </div>
      </div>

      {/* Content Info */}
      <div className="flex flex-1 flex-col justify-between p-3.5">
        <div>
          <h3 className="font-heading text-sm font-semibold text-foreground line-clamp-1 transition-colors group-hover:text-primary" title={title}>
            {title}
          </h3>
        </div>

        {/* Action Button */}
        {onBookTicket && (
          <button
            onClick={onBookTicket}
            className="mt-3 w-full rounded-lg bg-primary py-2 text-xs font-medium text-primary-foreground transition-all hover:bg-primary/90 active:scale-[0.98]"
          >
            Mua vé
          </button>
        )}
      </div>
    </div>
  );
}
```

---

### 5.2 Sơ đồ ghế rạp (Seat Selection Map)
Code nằm ở `src/components/common/seat-map/` (dùng chung cho client booking, quầy bán vé, editor admin). **Không tự viết lại DOM ghế**, hãy tái sử dụng `SeatMapFrame`, `SeatGrid`, `SeatButton`, `SeatLegend`, `CinemaScreen`. Style ghế nằm ở class `.seat` trong `index.css`, điều khiển bằng data-attribute:

`<button class="seat" data-state="available|selected|held|booked|inactive|aisle" data-type="standard|vip|couple" data-mark="suggested|range|pending">`

**Token màu (không hard-code pink-500/amber-500):** `--vip` (vàng), `--couple` (hồng), `--held` (cam), `--primary` (đỏ, đang chọn).

| Trạng thái / loại | Nhận diện (không chỉ dựa vào màu) |
| :--- | :--- |
| Thường | Nền `--seat-base` trung tính |
| VIP | Nhuộm 14% `--vip` + thanh vàng 3px **phía trên** |
| Ghế đôi | Nhuộm 16% `--couple` + thanh hồng 3px **phía dưới**, rộng 2 ô, nhãn `1-2` |
| Đang chọn | Nền `--primary` + **dấu tick** góc phải trên |
| Đang giữ | Sọc chéo `--held` |
| Đã bán | Dấu X, không hiện số |
| Đã tắt (editor) | Gạch ngang số |
| Lối đi | Khung nét đứt (editor) / khoảng trống (client) |

**Quy tắc layout:**
1. **Màn hình** dùng `CinemaScreen`: thanh `cut-xs` + chùm sáng gradient đổ xuống hàng A, rộng bằng đúng lưới ghế. Không dùng `rounded-t-*` hay shadow đỏ.
2. **Lối đi**: ở sơ đồ khách bật `compactAisles` trên `SeatGrid`, cột lối đi chạy suốt mọi hàng được thu còn 45% bề rộng ghế, lối đi lẻ vẫn để trống nguyên ô. Editor KHÔNG bật (cần ô đủ lớn để bấm).
3. **Kích thước ghế tối thiểu `1.75rem`** (đủ ngón tay). Phòng rộng trên mobile sẽ cuộn ngang, không ép ghế nhỏ hơn.
4. **Chú thích (`SeatLegend`)** chia 2 nhóm bằng `group`: *loại ghế* (kèm giá/ghế lấy từ dữ liệu suất chiếu qua `hint`) và *trạng thái* (đang chọn / đang giữ / đã bán). Không viết giá cứng trong legend.
5. **A11y**: mỗi hàng là `role="group" aria-label="Hàng A"`; mỗi ghế có `aria-label="Ghế A5, trống"` và `aria-pressed`. Mọi trạng thái phải phân biệt được khi không thấy màu.
6. Thêm trạng thái/loại ghế mới: thêm `data-*` + CSS trong `.seat`, cập nhật `SeatCellState`/`SeatVisualKind` và legend, không thêm class Tailwind rời rạc vào từng nút.

```tsx
<SelectableSeatMap
  rows={data.rows} cols={data.cols} aisles={data.aisles} seats={data.seats}
  statusMap={statusMap} selectedIds={selectedIds}
  onToggle={(seat) => onToggleSeat(seat.showtimeSeatId)}
  fitWidth   // sơ đồ co theo bề rộng container (client); bỏ qua ở quầy bán vé
/>
```

---

### 5.3 Thanh chọn ngày & Suất chiếu (Showtime Picker)

1. **Bộ chọn ngày (Date Strip)**:
   - Hiển thị danh sách 7 - 14 ngày tới dưới dạng tab ngang cuộn được.
   - Thẻ ngày gồm: Thứ (`Th 2`, `Th 3`, `CN`), Ngày tháng (`28/08`).
   - Active: `bg-primary text-primary-foreground shadow-md`.
   - Inactive: `bg-card hover:bg-muted text-foreground border border-border`.

2. **Nút suất chiếu (Showtime Slot Pill)**:
   - Format: Giờ bắt đầu (`19:30`), Giờ kết thúc mờ (`~ 21:45`), Phòng chiếu (`P.03`).
   - Style: `border border-border/80 bg-card hover:border-primary hover:text-primary px-3 py-2 rounded-lg text-xs font-medium transition-all`.

---

### 5.4 Hiển thị tiền tệ & Ngày giờ chuẩn Việt Nam

Sử dụng formatters thống nhất từ `@/lib/formatters`:
- **Tiền tệ (VND)**: Luôn format dạng `120.000 ₫` hoặc `120.000 VND` (Không viết `$120` hay `120000`).
- **Thời gian**: `19:30 - Thứ Năm, 28/08/2026`.
- **Mã vé / Đơn hàng**: Viết hoa mono `font-mono font-bold tracking-wider` (Ví dụ: `CNM-892348`).

---

## 6. Quy chuẩn Giao diện Quản trị (Admin System)

1. **Page Header Chuẩn**:
   Mọi trang Admin (`/admin/*`) phải dùng component `PageHeader`:
   ```tsx
   import { PageHeader } from "@/components/common/page-header";
   import { Button } from "@/components/ui/button";
   import { Plus } from "lucide-react";

   <PageHeader
     title="Quản lý Suất chiếu"
     description="Lên lịch và theo dõi trạng thái các suất chiếu trong rạp."
     action={
       <Button onClick={handleCreate}>
         <Plus className="size-4" />
         <span>Thêm suất chiếu</span>
       </Button>
     }
   />
   ```

2. **Data Table Pattern**:
   - Sử dụng `@tanstack/react-table` kết hợp `@/components/common/data-table/*`.
   - Luôn có cột `Actions` dạng dropdown menu (`MoreHorizontal` icon).
   - Sử dụng `StatusBadge` cho các cột trạng thái (`MovieStatusBadge`, `ShowtimeStatusBadge`, `BookingStatusBadge`, `PaymentStatusBadge`).

3. **Form & Dialog**:
   - Nhập liệu sử dụng `Input`, `Textarea`, `Select`, `InputGroup` từ `@/components/ui/*`.
   - Validation lỗi hiển thị bên dưới input với màu `text-destructive text-xs mt-1`.
   - Tương tác mở Modal qua Zustand Store (`useDialogStore`, `useAlertDialogStore`, `useSheetStore`).

---

## 7. Skeleton Loading & Trạng thái rỗng (Empty States)

1. **Skeleton Loading**:
   - Poster Skeleton: `<Skeleton className="aspect-[2/3] w-full rounded-xl" />`
   - Table Row Skeleton: `<Skeleton className="h-10 w-full rounded-md my-2" />`
   - Showtime Skeleton: `<Skeleton className="h-10 w-20 rounded-lg" />`

2. **Empty State Component**:
   - Luôn kèm icon minh họa (`Film`, `Ticket`, `CalendarX`), tiêu đề rõ ràng, mô tả giải thích và nút hành động (reset filter hoặc tạo mới).

---

## 8. Nguyên tắc cốt lõi khi AI sinh mã Component (AI Guardrails)

1. **KHÔNG hardcode màu HEX / RGB tùy tiện**:
   - Tuyệt đối dùng semantic token: `bg-background`, `text-foreground`, `bg-card`, `bg-primary`, `border-border`, `text-muted-foreground`.
   - Không dùng `bg-[#1a1a1a]` hoặc `text-[#ff0000]`.
2. **Luôn sử dụng `cn(...)` để merge class**:
   ```tsx
   import { cn } from "@/lib/utils";
   <div className={cn("base-classes", className)} />
   ```
3. **Đồng bộ kích thước Icon `lucide-react`**:
   - Default: `size-4`
   - Compact / Badge: `size-3` hoặc `size-3.5`
   - Large / Hero: `size-5` hoặc `size-6`
4. **Responsive First**:
   - Mobile: 1-2 cột (`grid-cols-2`).
   - Tablet: 3 cột (`md:grid-cols-3`).
   - Desktop: 4-5 cột (`lg:grid-cols-4 xl:grid-cols-5`).
5. **Tiêu chuẩn tiếng Việt trong UI**:
   - Giao diện người dùng mặc định sử dụng Tiếng Việt tự nhiên, chính xác thuật ngữ rạp chiếu phim (Suất chiếu, Rạp/Phòng chiếu, Vé xem phim, Bắp nước/Combo, Ghế VIP, Ghế Sweetbox, Đang chiếu, Sắp chiếu).

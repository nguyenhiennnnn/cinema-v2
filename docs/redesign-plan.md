# Perfo Redesign — Checklist sống

> Cập nhật sau mỗi pha. Pha 0 hoàn thành: 2026-10-23.

---

## Pha 0 — Kiểm kê (HOÀN THÀNH)

### Kết quả build/lint gốc (trước khi sửa)

| Công cụ | Kết quả |
|---|---|
| `npm run lint` | ✅ 0 error, 1 warning (`useMemo` deps trong `use-seat-grid-model.ts`) |
| `npm run build` | ✅ Build thành công (22.4s), bundle 2,147 kB |
| `dotnet build Cinema.sln` | ✅ 0 Error, 7 Warning (NU1903 vulnerability + CS8603 nullable) |
| Playwright e2e | Chưa chạy (cần dev server + backend) — skip ở Pha 0 |

### Thống kê vi phạm hiện có (trong `frontend/src`)

| Pattern cấm | Số lần xuất hiện | Ghi chú |
|---|---|---|
| `rounded-` | **252** | Nặng nhất: `calendar.tsx`(11), `booking-detail-page.tsx`(9), `seat-editor-toolbar.tsx`(9) |
| `shadow-` | **26** | `input-group.tsx`(3), `dropdown-menu.tsx`(2), `ai-assistant-widget.tsx`(2) |
| `backdrop-blur` | **8** | |
| `bg-gradient-` | 0 | ✅ Sạch |
| `uppercase` | **36** | Chủ yếu ở Tailwind class `uppercase` |
| `eyebrow` | 0 | ✅ Sạch |
| `font-mono` | **19** | |
| `ticket-notch` | **3** | Class cũ cần alias |
| `perforation` | **6** | Class cũ cần alias |
| `sprocket-rule` | **2** | Class cũ cần alias |
| `clip-corner` | **4** | Class cũ cần alias |
| `"Cinema"` (chuỗi thương hiệu) | **67** | Cần phân loại |
| `→` trong nút | **2** | |

### Tài liệu/config AI còn nhắc design system cũ (xử lý ở Pha 5)

| File | Vi phạm |
|---|---|
| `.agents/rules/perfo-design.md` | L11-12: nhắc `ticket-notch`, `sprocket-rule`, `clip-corner`, `text-eyebrow` như class **cũ** (nhưng đúng ngữ cảnh — đây là rule cấm dùng chúng) |
| `perfo-brand/README.md` | L12: nhắc Oswald và Geist (đúng — chỉ dùng trong banner/guide offline, không phải UI) |
| `README.md` | Tiêu đề còn là "Cinema" |
| `frontend/index.html` | title, meta, og còn là "Cinema" |

> **Kết luận:** `.agents/rules/perfo-design.md` đúng về ý nghĩa (cấm class cũ), chỉ cần giữ nguyên. Chỉ `README.md` và `index.html` cần sửa ở Pha 5.

---

## Pha 1 — Nền tảng và primitives

> `index.css` đã có đủ Perfo tokens (đã hợp nhất từ trước). Không cần làm bước 1.

### Alias class cũ cần tạo (bước 2)
- [ ] `.ticket-notch` → alias `notch-y`
- [ ] `.perforation` → alias `perf-h` / `perf-v`
- [ ] `.sprocket-rule` → alias `perf-h`
- [ ] `.clip-corner` → alias `cut-md`
- [ ] `.text-eyebrow` → alias xoá (không dùng)

### components/ui cần sửa (bước 3)
- [ ] `button.tsx` — `rounded-*` → `cut-sm`, bỏ `shadow-*`
- [ ] `input.tsx` — nền `bg-secondary`, đáy 2px `border-input`
- [ ] `textarea.tsx`
- [ ] `select.tsx`
- [ ] `checkbox.tsx`
- [ ] `switch.tsx`
- [ ] `badge.tsx` — `cut-xs`, bỏ `rounded-full`
- [ ] `card.tsx` — `cut-md`, bỏ viền/shadow
- [ ] `dialog.tsx` — `cut-lg`, nền che đặc không blur
- [ ] `sheet.tsx`
- [ ] `dropdown-menu.tsx` — bỏ `rounded-*`, `shadow-*`
- [ ] `popover.tsx`
- [ ] `tabs.tsx`
- [ ] `table.tsx` — không viền bao, xen kẽ, số tabular
- [ ] `tooltip.tsx`
- [ ] `skeleton.tsx` — nhấp nháy opacity, không dải sáng
- [ ] `toast.tsx` — góc dưới trái, `bg-foreground text-background cut-sm`
- [ ] `calendar.tsx` — nhiều `rounded-*` nhất (11 chỗ)
- [ ] `avatar.tsx` — chỉ giữ `rounded-full`
- [ ] `command.tsx`
- [ ] `accordion.tsx`
- [ ] `alert-dialog.tsx`

### components/common cần sửa (bước 4)
- [ ] `page-header.tsx`
- [ ] `status-badge.tsx` — icon + chữ, không viết hoa
- [ ] `data-table/data-table.tsx` — theo spec Admin
- [ ] `empty-state.tsx` — câu mời hành động, nút, không emoji

---

## Pha 2 — Layout và thương hiệu dùng chung

- [ ] `components/common/brand-mark.tsx` → thay bằng `perfo-brand/react/perfo-brand.tsx`
- [ ] `layouts/client/header/client-header.tsx` — nền đặc, không blur
- [ ] `layouts/client/header/header-logo.tsx`
- [ ] `layouts/client/client-footer.tsx`
- [ ] `layouts/client/auth-layout.tsx`
- [ ] `layouts/admin/admin-layout.tsx`
- [ ] `layouts/admin/admin-sidebar.tsx` — mục chọn: khối `sidebar-primary` đặc, không số thứ tự
- [ ] `layouts/admin/admin-topbar.tsx`
- [ ] `layouts/admin/admin-auth-layout.tsx`
- [ ] `pages/not-found-page.tsx`
- [ ] `pages/forbidden-page.tsx`
- [ ] Auth client (login, register, forgot-password)
- [ ] Auth admin
- [ ] `frontend/index.html` — title "Perfo", meta, OG, favicon
- [ ] Chép assets: `perfo-brand/app`, `banner`, `logo` → `frontend/public/brand/`
- [ ] Chép pattern → `frontend/src/assets/brand/`

---

## Pha 3 — Client (từng nhóm màn hình)

### (a) Trang chủ và lịch chiếu hôm nay
- [ ] `features/client/home/pages/home-page.tsx`
- [ ] `features/client/home/components/hero-carousel.tsx`
- [ ] `features/client/home/components/movie-section.tsx`
- [ ] `features/client/home/components/combo-carousel.tsx`
- [ ] `features/client/home/components/cinema-experience-section.tsx`
- [ ] `components/common/showtime-group-list.tsx`

### (b) Danh sách phim và chi tiết phim
- [ ] `features/client/movies/pages/movies-page.tsx`
- [ ] `features/client/movies/pages/movie-detail-page.tsx`
- [ ] `features/client/movies/components/*` (thẻ phim, đánh giá)

### (c) Chọn suất chiếu
- [ ] `features/client/showtimes/*`
- [ ] `components/common/date-strip.tsx`

### (d) Sơ đồ ghế
- [ ] `components/common/seat-map/seat-button.tsx`
- [ ] `components/common/seat-map/seat-grid.tsx`
- [ ] `components/common/seat-map/seat-legend.tsx`
- [ ] `components/common/seat-map/selectable-seat-map.tsx`

### (e) Combo, thanh toán, kết quả
- [ ] `features/client/bookings/components/booking/*`
- [ ] `features/client/bookings/pages/booking-page.tsx`
- [ ] `features/client/bookings/pages/booking-result-page.tsx`
- [ ] `features/client/bookings/components/result/booking-result-status.tsx`
- [ ] `components/common/combo-picker/*`

### (f) Vé điện tử
- [ ] `features/client/bookings/components/result/booking-result-tickets.tsx`
- [ ] `features/client/account/components/ticket-card.tsx`
- [ ] `features/client/account/components/ticket-qr-card.tsx`
- [ ] `features/client/account/pages/ticket-detail-page.tsx`

### (g) Tài khoản, lịch sử, hạng thành viên
- [ ] `features/client/account/pages/*`
- [ ] `features/client/account/components/*`
- [ ] `features/client/membership/pages/membership-page.tsx`
- [ ] `features/client/membership/components/*`
- [ ] `layouts/client/account/*`

### (h) Trợ lý AI
- [ ] `features/client/ai/components/*` — xưng "Perfo"

---

## Pha 4 — Admin

### Dashboard + CRUD mẫu (duyệt trước)
- [ ] `features/admin/dashboard/pages/dashboard-page.tsx`
- [ ] `features/admin/movies/pages/movies-page.tsx` (CRUD mẫu hoàn chỉnh)
- [ ] `features/admin/movies/components/*`

### Nhân CRUD ra các trang danh mục
- [ ] `features/admin/genres/*`
- [ ] `features/admin/actors/*`
- [ ] `features/admin/directors/*`
- [ ] `features/admin/languages/*`
- [ ] `features/admin/age-ratings/*`
- [ ] `features/admin/room-types/*`
- [ ] `features/admin/seat-types/*`
- [ ] `features/admin/locations/*`
- [ ] `features/admin/membership-tiers/*`
- [ ] `features/admin/combos/*`

### Các trang vận hành
- [ ] `features/admin/rooms/*` (phòng chiếu + sơ đồ ghế)
- [ ] `features/admin/showtimes/*`
- [ ] `features/admin/bookings/*`
- [ ] `features/admin/payments/*`
- [ ] `features/admin/counter/*` (bán vé tại quầy + check-in QR)
- [ ] `features/admin/users/*`
- [ ] `features/admin/employees/*` (nếu có)
- [ ] `features/admin/reviews/*`
- [ ] `features/admin/reports/*`
- [ ] `features/admin/settings/*`
- [ ] `features/admin/uploads/*`

---

## Pha 5 — Nội dung ngoài giao diện

- [ ] Phân loại "Cinema" trong `frontend/src` + `index.html` + `public`
- [ ] Backend: email template, tên gửi, system prompt AI
- [ ] `README.md` — tiêu đề và giới thiệu Perfo
- [ ] `frontend/index.html` — title/meta/OG
- [ ] Dọn tài liệu cũ

---

## Pha 6 — Dọn dẹp và nghiệm thu

- [ ] Xóa alias class cũ trong `index.css`
- [ ] grep 0 violation trong `frontend/src`
- [ ] Cập nhật Playwright e2e cho chữ "Perfo"
- [ ] Chạy lại toàn bộ lint/build/test
- [ ] Chụp ảnh "sau" đối chiếu với ảnh "trước"
- [ ] Kiểm tra tương phản, focus, vùng bấm 44px, dấu tiếng Việt

---

## Ghi chú kỹ thuật

- `index.css` đã có Perfo tokens đầy đủ — **không cần sửa**.
- Font Archivo Variable và Be Vietnam Pro đã được cài và import — **không cần sửa**.
- Playwright e2e: 7 spec files trong `frontend/e2e/`.
- Backend solution: `backend/Cinema.sln` (namespace giữ nguyên `Cinema.*`).


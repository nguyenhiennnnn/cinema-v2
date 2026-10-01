# 🎬 Cinema — Hệ thống đặt vé xem phim trực tuyến

Hệ thống quản lý & đặt vé rạp chiếu phim full-stack, gồm ứng dụng khách hàng (đặt vé, thanh toán online) và trang quản trị (quản lý phim, suất chiếu, phòng chiếu, nhân viên, báo cáo doanh thu, check-in tại quầy...).

## Mục lục

- [Tính năng](#tính-năng)
- [Kiến trúc & công nghệ](#kiến-trúc--công-nghệ)
- [Cấu trúc thư mục](#cấu-trúc-thư-mục)
- [Bắt đầu nhanh (Docker)](#bắt-đầu-nhanh-docker)
- [Chạy thủ công (không dùng Docker)](#chạy-thủ-công-không-dùng-docker)
- [Tài khoản test có sẵn](#tài-khoản-test-có-sẵn)
- [Biến môi trường](#biến-môi-trường)
- [Background jobs & Hangfire Dashboard](#background-jobs--hangfire-dashboard)
- [Testing](#testing)
- [Triển khai Production](#triển-khai-production)
- [Ghi chú / giới hạn hiện tại](#ghi-chú--giới-hạn-hiện-tại)

## Tính năng

### Phía khách hàng
- Duyệt phim đang chiếu / sắp chiếu / đã ngừng chiếu, xem chi tiết phim, diễn viên, đạo diễn, đánh giá (review).
- Xem lịch chiếu theo rạp / phòng / suất, chọn ghế theo sơ đồ, cập nhật trạng thái ghế **realtime** qua SignalR (`/hubs/showtimes`) để tránh 2 người cùng chọn 1 ghế.
- Giữ ghế tạm thời (hold) — hết hạn tự động được giải phóng bởi background job chạy mỗi phút.
- Đặt vé, chọn combo bắp nước, thanh toán qua **Stripe**, **MoMo** hoặc **VNPay** (đều ở môi trường sandbox/test theo `.env.example`).
- Tài khoản thành viên: đăng ký/đăng nhập bằng email/mật khẩu hoặc **Google OAuth**, hạng thành viên (membership tier) theo điểm tích lũy, lịch sử đặt vé.
- Trợ lý AI (Google Gemini, endpoint `POST /api/v1/ai/chat`) hỗ trợ tư vấn/tìm phim, dùng được cả khi chưa đăng nhập.
- Đánh giá phim sau khi xem.
- Vé điện tử có mã QR (sinh bằng `QRCoder` ở backend, hiển thị bằng `qrcode.react` ở frontend).

### Phía quản trị (Admin)
- Quản lý danh mục: phim, thể loại, diễn viên, đạo diễn, ngôn ngữ, phân loại độ tuổi, loại phòng chiếu, loại ghế, vị trí công việc (position), hạng thành viên.
- Quản lý phòng chiếu & sơ đồ ghế, suất chiếu, combo.
- Quản lý người dùng, nhân viên.
- Bán vé tại quầy (counter) và quản lý đơn đặt vé, thanh toán & hoàn tiền.
- **Check-in tại quầy bằng quét mã QR qua camera** — dùng Web API `BarcodeDetector` có sẵn của trình duyệt (không cần thư viện quét ngoài); có kèm ô nhập mã tay để dự phòng khi trình duyệt không hỗ trợ.
- Duyệt/ẩn review của khách hàng.
- Dashboard & báo cáo doanh thu, thống kê.
- Cấu hình hệ thống (settings) chung của rạp.

> Lưu ý về phân quyền: hệ thống có 3 role (`Customer`, `Admin`, `Employee`). Vị trí công việc (`Position`, ví dụ "Trưởng ca", "Nhân viên bán vé") hiện chỉ là dữ liệu mô tả (HR) — **không** ảnh hưởng đến quyền truy cập API/route; mọi tài khoản role `Employee` đều có cùng một mức quyền (bán vé tại quầy, check-in, xem phim/suất chiếu, xem đơn đặt vé). Chỉ role `Admin` mới truy cập được các trang quản trị còn lại (báo cáo, cài đặt, danh mục, quản lý người dùng/nhân viên...).

## Kiến trúc & công nghệ

Backend theo **Clean Architecture**, chia theo 5 project độc lập:

```
Cinema.Domain          → Entities, Enums (không phụ thuộc project khác)
Cinema.Application     → DTOs, Services, Validators (FluentValidation), Interfaces, Background Jobs (Hangfire)
Cinema.Infrastructure  → EF Core, Payments (Stripe/MoMo/VNPay), Realtime (SignalR), Email (MailKit), FileStorage (Cloudinary), Auth (Google OAuth)
Cinema.Api             → Controllers (Admin/Client), Middlewares, Filters, Hubs
Cinema.Shared          → Exceptions, Result pattern, Constants dùng chung
```

| Layer | Công nghệ chính |
|---|---|
| Backend | .NET 10, ASP.NET Core Web API, Entity Framework Core 10, SQL Server 2022 |
| Background jobs | Hangfire (SQL Server storage) — dashboard tại `/admin/hangfire` (chỉ Admin) |
| Realtime | SignalR (`Microsoft.AspNetCore.SignalR.Core`) |
| Thanh toán | Stripe.net, VNPay & MoMo (gọi HTTP trực tiếp, sandbox) |
| Auth | JWT (access + refresh token), Google OAuth (`Google.Apis.Auth`) |
| Lưu trữ file | Cloudinary (`CloudinaryDotNet`) |
| Email | SMTP qua MailKit |
| QR code | QRCoder (sinh mã ở backend) |
| AI | Google Gemini API |
| Logging | Serilog (console/structured) |
| Testing (BE) | xUnit, Moq, Shouldly — Unit tests (EF Core Sqlite in-memory) + Integration tests (`Testcontainers.MsSql` dựng SQL Server thật trong Docker, `Respawn` reset DB giữa các test) |
| Frontend | React 19.2, TypeScript, Vite 8 (rolldown), React Compiler (babel-plugin-react-compiler) |
| State/Data | TanStack Query v5, Zustand v5, React Hook Form + Zod, `nuqs` (URL state) |
| UI | Tailwind CSS v4, `shadcn` CLI, `@base-ui/react`, Recharts, `qrcode.react` |
| Routing | React Router v7 |
| Testing (FE) | Playwright (E2E, 7 kịch bản trong `frontend/e2e/`) |
| Hạ tầng dev | Docker, Docker Compose (hot-reload cho cả BE và FE) |

## Cấu trúc thư mục

```
Cinema/
├── backend/
│   ├── Cinema.Api/            # Entry point (Program.cs), Controllers, Hubs, Middlewares
│   ├── Cinema.Application/    # Business logic, DTOs, Validators, Services, BackgroundJobs
│   ├── Cinema.Domain/         # Entities, Enums
│   ├── Cinema.Infrastructure/ # EF Core, Payments, Realtime, Email, FileStorage, Auth
│   ├── Cinema.Shared/         # Exceptions, Result, Constants dùng chung
│   └── tests/
│       ├── Cinema.UnitTests/         # xUnit + Moq + Shouldly
│       └── Cinema.IntegrationTests/  # xUnit + Testcontainers.MsSql + Respawn
├── frontend/
│   └── src/
│       ├── app/                # App shell, routes (client-routes.tsx, admin-routes.tsx), providers, modals
│       ├── features/
│       │   ├── client/         # movies, showtimes, bookings, combos, membership, account, ai, home...
│       │   └── admin/          # dashboard, catalog, employees, counter, checkin, reports, uploads...
│       ├── components/         # UI dùng chung (shadcn-based)
│       ├── layouts/            # Layout client & admin
│       ├── pages/              # Trang chung (not-found...)
│       ├── config/              # Hằng số cấu hình mặc định
│       ├── stores/, hooks/, lib/, api/, types/
│       └── e2e/                # Playwright test
├── load-tests/                 # Kịch bản kiểm thử tải (k6, chạy qua run-load-test.ps1)
├── docker-compose.yml          # Môi trường DEV (hot-reload) — repo hiện chỉ có file compose này
├── package.json                 # Chỉ chứa script tiện ích chạy load-tests, không phải monorepo workspace
└── dotnet-tools.json            # Local tool: csharpier (format code C#)
```

## Bắt đầu nhanh (Docker)

Yêu cầu: Docker & Docker Compose.

```bash
# 1. Clone project và vào thư mục gốc
cd Cinema

# 2. Tạo file .env từ mẫu, sau đó điền các giá trị cần thiết
cp .env.example .env

# 3. Khởi chạy toàn bộ hệ thống (SQL Server + API + Frontend)
docker compose up --build -d

# 4. Theo dõi log (tuỳ chọn)
docker compose logs -f api frontend
```

Sau khi container khởi động xong:
- Frontend: http://localhost:5173
- API: http://localhost:8080 (health check: `GET /api/v1/health`)
- Swagger: `/swagger` (2 doc riêng — `client` và `admin` — chỉ hiện khi `ASPNETCORE_ENVIRONMENT=Development`)
- Hangfire Dashboard: http://localhost:8080/admin/hangfire (yêu cầu đăng nhập role Admin)
- SQL Server: `localhost:1433`

Migration EF Core sẽ tự chạy khi khởi động nếu `RUN_MIGRATIONS_ON_STARTUP=true` (mặc định trong `docker-compose.yml`).

Dừng hệ thống: `docker compose down` (thêm `-v` nếu muốn xoá luôn volume dữ liệu SQL Server).

`docker-compose.yml` hiện tại chỉ build stage `dev` của cả 2 Dockerfile (bind-mount source code, `dotnet watch` / Vite HMR) — dùng cho môi trường phát triển, không phải cấu hình production.

## Chạy thủ công (không dùng Docker)

**Backend** (yêu cầu .NET 10 SDK, SQL Server chạy sẵn):

```bash
cd backend
dotnet restore
dotnet ef database update --project Cinema.Infrastructure --startup-project Cinema.Api
dotnet run --project Cinema.Api
```

Có thể copy `Cinema.Api/appsettings.Local.json.example` thành `appsettings.Local.json` để cấu hình connection string / URL cho môi trường local không dùng Docker.

**Frontend** (yêu cầu Node.js `^20.19` hoặc `>=22.12` — do Vite 8 dùng Rolldown; khuyến nghị Node 22 LTS):

```bash
cd frontend
cp .env.example .env
npm install
npm run dev
```

## Tài khoản test có sẵn

Được seed tự động khi ứng dụng khởi động (`DbInitializer.SeedAsync`, idempotent — chỉ tạo nếu email chưa tồn tại) và một phần qua `SeedData` (chạy cùng migration):

| Vai trò | Email | Mật khẩu | Ghi chú |
|---|---|---|---|
| Admin | `admin@cinema.local` | `Admin@123` | Seed sẵn qua migration (`HasData`), có hồ sơ Employee vị trí "Quản lý rạp" |
| Employee | `employee@cinema.local` | `Employee@123` | Vị trí "Nhân viên bán vé" — test bán vé tại quầy / check-in |
| Customer | `customer@cinema.local` | `Customer@123` | 300 điểm → hạng Thành viên |
| Customer | `customer2@cinema.local` | `Customer@123` | 800 điểm → hạng Bạc |
| Customer | `customer3@cinema.local` | `Customer@123` | 2.500 điểm → hạng Vàng |
| Customer | `customer4@cinema.local` | `Customer@123` | 6.000 điểm → hạng Kim Cương |

`frontend/.env.example` cũng khai báo sẵn `E2E_CUSTOMER_EMAIL/PASSWORD` và `E2E_ADMIN_EMAIL/PASSWORD` trùng với 2 tài khoản đầu để Playwright dùng khi chạy E2E.

## Biến môi trường

Toàn bộ biến môi trường mẫu nằm ở `.env.example` (gốc project, dùng cho Docker Compose) và `frontend/.env.example` (dùng khi chạy frontend độc lập, có thêm biến cho Playwright). Các nhóm chính:

- **SQL Server**: `SQL_SA_PASSWORD`
- **JWT**: `JWT_SECRET` (tối thiểu 64 ký tự), `JWT_ISSUER`, `JWT_AUDIENCE`, thời hạn access/refresh token
- **Cloudinary**: `CLOUDINARY_CLOUD_NAME`, `CLOUDINARY_API_KEY`, `CLOUDINARY_API_SECRET`
- **SMTP**: `SMTP_HOST`, `SMTP_PORT`, `SMTP_USER`, `SMTP_PASSWORD`, `SMTP_FROM_ADDRESS`, `SMTP_FROM_NAME`
- **Cổng thanh toán**: `VNPAY_*`, `MOMO_*`, `STRIPE_*` (đều đang trỏ tới môi trường sandbox/test)
- **Gemini AI**: `GEMINI_API_KEY`, `GEMINI_MODEL`
- **Google OAuth**: `GOOGLE_CLIENT_ID`, `GOOGLE_CLIENT_SECRET`
- **Frontend (build-time)**: `VITE_API_BASE_URL`, `VITE_SIGNALR_HUB_URL`, `VITE_GOOGLE_CLIENT_ID`

> ⚠️ Không commit file `.env` chứa giá trị thật.

## Background jobs & Hangfire Dashboard

- `ReleaseExpiredSeatsJob` — recurring job chạy **mỗi phút**, giải phóng các ghế đang bị giữ (hold) quá hạn.
- `SendEmailJob` — gửi email qua MailKit (background, không chặn request).
- Dashboard quản lý job tại `/admin/hangfire`, bị vô hiệu hoá khi `ASPNETCORE_ENVIRONMENT=Testing` và yêu cầu đăng nhập role Admin (`HangfireAdminAuthorizationFilter`).

## Testing

```bash
# Backend — unit + integration tests
cd backend
dotnet test
```

Integration tests dùng `Testcontainers.MsSql` — tự động khởi động một container SQL Server thật khi chạy test (cần Docker đang chạy trên máy), và `Respawn` để reset dữ liệu giữa các test.

```bash
# Frontend — E2E với Playwright (cần hệ thống backend + frontend đang chạy)
cd frontend
npm run test:e2e        # chạy toàn bộ
npm run test:e2e:ui     # chạy với giao diện Playwright UI
npm run test:e2e:headed # chạy có hiển thị trình duyệt
npm run test:e2e:debug  # chạy debug từng bước
```

Kịch bản kiểm thử tải (k6) nằm ở `load-tests/scenarios/` (`browse-movies-stress.js`, `peak-ticket-rush.js`), chạy qua `run-load-test.ps1` (PowerShell) hoặc các script `npm run test:load*` ở `package.json` gốc.

## Triển khai Production

Repo hiện tại **chưa có** cấu hình/compose file dành riêng cho production:

- Cả `backend/Dockerfile` và `frontend/Dockerfile` mới chỉ định nghĩa stage `dev` (chạy `dotnet watch` / Vite dev server, bind-mount source từ host). Chưa có stage build production (không hot-reload, build release/`vite build`).
- Chỉ có một file `docker-compose.yml` duy nhất ở gốc project, dùng cho môi trường phát triển.
- `frontend/vercel.json` có cấu hình rewrite SPA (`/(.*) → /index.html`), cho thấy frontend có thể deploy tĩnh lên Vercel (`npm run build` rồi deploy thư mục `dist/`), nhưng đây chỉ là cấu hình routing — chưa có pipeline/CI-CD chính thức đi kèm trong repo.
- Muốn chạy production thật sự cần tự bổ sung: stage build production cho Docker (hoặc publish `dotnet publish` + host bằng Kestrel/reverse proxy), build tĩnh frontend, và cấu hình secrets an toàn hơn (`.env` hiện chỉ phù hợp cho dev/staging).

## Ghi chú / giới hạn hiện tại

- `Position` (vị trí công việc của Employee) chỉ là dữ liệu mô tả, không có tác dụng phân quyền — mọi Employee có cùng quyền truy cập.
- Chưa có thư mục `secrets/` hay tài liệu quản lý secret riêng trong repo; biến nhạy cảm hiện quản lý qua `.env` (không commit).
- Route quét QR bằng camera phụ thuộc Web API `BarcodeDetector` — chưa được hỗ trợ trên mọi trình duyệt (ví dụ Safari/Firefox desktop), khi đó cần dùng ô nhập mã tay thay thế.

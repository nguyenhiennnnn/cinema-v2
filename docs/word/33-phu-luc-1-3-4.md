# PHỤ LỤC 1. Hướng dẫn cài đặt, vận hành

## P1.1 YÊU CẦU MÔI TRƯỜNG

Mã nguồn dùng .NET 10 (`Cinema.Api.csproj` khai báo `net10.0`) cho phần backend và Node.js từ phiên bản 22.12.0 trở lên (trường `engines.node` trong `package.json`) cho phần frontend. Docker cần thiết khi chạy hệ thống bằng `docker compose` (Mục P1.3) và khi chạy kịch bản E2E cần thao tác CSDL (hàm hỗ trợ `runSql` trong `frontend/e2e/helpers.ts` chạy lệnh `docker run` với ảnh `postgres:17-alpine`). CSDL là PostgreSQL trên Neon: `docker-compose.yml` không có dịch vụ CSDL mà nhận chuỗi kết nối từ biến `DATABASE_URL` (Mục P3.1), vì vậy cần một tài khoản Neon hoặc một PostgreSQL tương đương.

Các công cụ dòng lệnh của .NET được khóa phiên bản trong `dotnet-tools.json`: `csharpier` 1.3.0 và `dotnet-ef` 10.0.11. Việc chạy kiểm thử E2E cần trình duyệt Chromium của Playwright (`npm run setup:browsers`); kiểm thử tải dùng k6 qua PowerShell (`load-tests/run-load-test.ps1`).

[CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch]

## P1.2 CÀI ĐẶT

Các lệnh dưới đây đều chạy từ thư mục gốc của repository, nơi có `package.json` duy nhất chứa các script điều phối. Sau khi sao chép mã nguồn, lệnh `npm run setup` thực hiện tuần tự bốn việc: `npm install`, `npm run setup:env`, `npm run setup:tools` (`dotnet tool restore`) và `npm run setup:backend` (`dotnet restore backend/Cinema.slnx`).

```bash
npm run setup
```

Script `setup:env` (`scripts/setup-env.mjs`) sao chép `.env.example` thành `.env` và `frontend/.env.example` thành `frontend/.env`, và chỉ tạo tệp đích khi tệp đó chưa tồn tại. Sau bước này cần mở `.env` và điền tối thiểu `DATABASE_URL` và `JWT_SECRET`; `docker-compose.yml` khai báo `JWT_SECRET` ở dạng bắt buộc nên `docker compose` báo lỗi nếu biến này chưa được đặt. Ý nghĩa từng biến được liệt kê ở Bảng P3.1. Để chạy E2E, cài thêm trình duyệt bằng lệnh sau.

```bash
npm run setup:browsers
```

[CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch; xác nhận có cần cài phụ thuộc riêng trong `frontend/` hay không, vì `frontend/package.json` có danh sách phụ thuộc và tệp khóa riêng còn script `setup` chỉ chạy `npm install` ở thư mục gốc]

## P1.3 CHẠY BẰNG DOCKER

`docker-compose.yml` định nghĩa hai dịch vụ ở chế độ phát triển (nhắm tới stage `dev` của Dockerfile): `api` (container `perfo-api`, cổng 8080, có kiểm tra sức khỏe tại `/api/v1/health`) và `frontend` (container `perfo-frontend`, cổng 5173, phụ thuộc `api`). Cả hai gắn thư mục mã nguồn vào container để tải lại nóng. Cơ sở dữ liệu không nằm trong Docker mà là Neon, được cấu hình qua `DATABASE_URL`.

```bash
npm run docker:up        # docker compose up --build -d
npm run docker:ps        # xem trạng thái dịch vụ
npm run docker:logs:api  # xem nhật ký của api
npm run docker:down      # dừng và gỡ container
```

Ngoài ra `package.json` còn có `docker:up:fg` (chạy ở tiền cảnh), `docker:restart`, `docker:build`, `docker:build:nocache`, `docker:logs`, `docker:logs:frontend`, `docker:exec:api`, `docker:exec:frontend` và `docker:prune`.

[CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch]

## P1.4 CHẠY THỦ CÔNG

Khi không dùng Docker, backend và frontend được chạy bằng hai script sau, mỗi script trong một cửa sổ dòng lệnh; script `dev` dùng `concurrently` để chạy cả hai cùng lúc.

```bash
npm run dev:backend    # dotnet watch --project backend/Cinema.Api run
npm run dev:frontend   # vite frontend
npm run dev            # cả hai, qua concurrently
```

Script `dev:backend:once` chạy backend một lần bằng `dotnet run` (không tự tải lại). Theo `launchSettings.json`, hồ sơ HTTP của backend nghe tại `http://localhost:8080`; địa chỉ frontend mặc định là `http://localhost:5173` (giá trị `FRONTEND_BASE_URL` và `CORS_ALLOWED_ORIGIN` ở Bảng P3.1). Biến `VITE_API_BASE_URL` và `VITE_SIGNALR_HUB_URL` trong `frontend/.env` trỏ frontend tới backend.

[CẦN BỔ SUNG: xác nhận cách backend chạy thủ công nhận chuỗi kết nối CSDL (qua `ConnectionStrings__DefaultConnection`, tệp `appsettings.Local.json` hay cách khác), vì `.env.example` đặt tên `DATABASE_URL` và chỉ `docker-compose.yml` ánh xạ biến này sang `ConnectionStrings__DefaultConnection`]

[CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch]

## P1.5 ÁP DỤNG MIGRATION

Migration được quản lý bằng Entity Framework Core với dự án chứa migration là `Cinema.Infrastructure` và dự án khởi động là `Cinema.Api`. Lệnh áp dụng migration vào CSDL đã cấu hình là:

```bash
npm run db:update
```

Các script liên quan gồm `db:migration:add -- TenMigration` (tạo migration mới), `db:migration:remove`, `db:migration:list`, `db:info`, `db:migration:script` (sinh tệp SQL idempotent `backend/migration.sql`), `db:drop` (xóa CSDL, dùng cờ `--force`) và `db:reset` (`db:drop` rồi `db:update`); hai lệnh sau xóa dữ liệu nên chỉ dùng cho CSDL thử nghiệm. Công cụ thiết kế (`AppDbContextFactory`) đọc cấu hình từ `appsettings.json`, `appsettings.Development.json`, User Secrets và biến môi trường.

Ngoài ra, khi tiến trình `Cinema.Api` khởi động với `RunMigrationsOnStartup` bật, `Program.cs` tự gọi `MigrateAsync()` rồi `DbInitializer.SeedAsync` (xem Mục P1.7). Giá trị mặc định của khóa này là `false` trong `appsettings.json`, `true` trong `docker-compose.yml` (`RUN_MIGRATIONS_ON_STARTUP`) và `true` khi khóa vắng mặt (đối số mặc định của `GetValue` trong `Program.cs`).

[CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch]

## P1.6 CHẠY KIỂM THỬ

Các script kiểm thử trong `package.json` gồm:

```bash
npm run test:unit          # dotnet test backend/tests/Cinema.UnitTests
npm run test:integration   # dotnet test backend/tests/Cinema.IntegrationTests
npm run test:backend       # cả hai bộ trên
npm run test:e2e           # Playwright (thư mục frontend/e2e)
npm run test:e2e:report    # mở báo cáo HTML của Playwright
```

Script `test` chạy `test:backend` rồi `test:e2e`; `test:backend:coverage` thu thập độ bao phủ bằng `XPlat Code Coverage`; `test:e2e:ui`, `test:e2e:headed` và `test:e2e:debug` là các chế độ chạy E2E có giao diện. Kịch bản E2E cần frontend và backend đang chạy (Mục P1.3 hoặc P1.4) và đọc địa chỉ ứng dụng từ biến `E2E_BASE_URL` (mặc định `http://localhost:5173`). Kiểm thử tải dùng `test:load:browse`, `test:load:peak` và `test:load` (gọi `load-tests/run-load-test.ps1`).

[CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch; test tích hợp cần Docker (Testcontainers) theo Chương 6]

## P1.7 TÀI KHOẢN KIỂM THỬ

Dữ liệu nền gắn với migration được khai báo trong `Persistence/Seed/SeedData.cs`. Tài khoản demo trong tệp này là một tài khoản quản trị duy nhất: người dùng `UserId = 1` họ tên `System Admin`, email `admin@cinema.local`, vai trò `Admin` (`RoleId = 2`), đã kích hoạt và đã xác nhận email, gắn với nhân viên `EmployeeId = 1` ở chức vụ "Quản lý rạp". Mật khẩu không được ghi trong báo cáo; xem `SeedData.cs`. Các tài khoản khách hàng và nhân viên phục vụ kiểm thử do `DbInitializer.SeedAsync` tạo khi khởi động, xem `DbInitializer.cs`.

[CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch]

## P1.8 SWAGGER, HANGFIRE

Swagger chỉ được bật ở môi trường Development (`app.Environment.IsDevelopment()` trong `Program.cs`). Giao diện Swagger UI có hai tài liệu: "Client API" (`/swagger/client/swagger.json`) và "Admin API" (`/swagger/admin/swagger.json`). Với địa chỉ mặc định của backend, giao diện mở tại `http://localhost:8080/swagger`.

Bảng điều khiển Hangfire được ánh xạ tại `/admin/hangfire` (trừ môi trường `Testing`). Bộ lọc `HangfireAdminAuthorizationFilter` cho phép truy cập tự do ở môi trường Development; ở các môi trường khác chỉ người dùng đã xác thực có vai trò `Admin` được vào. Hangfire dùng PostgreSQL làm nơi lưu trữ (`UsePostgreSqlStorage`).

[CẦN BỔ SUNG: xác nhận lệnh chạy được trên máy sạch]

---

# PHỤ LỤC 3. Biến cấu hình môi trường

## P3.1 DANH SÁCH BIẾN

Bảng P3.1 liệt kê các biến có trong `.env.example` (backend, và cả ba biến `VITE_*` đầu tiên), `frontend/.env.example`, `docker-compose.yml`, `render.yaml` và `frontend/e2e/helpers.ts`. Báo cáo chỉ ghi tên biến, ý nghĩa và giá trị mặc định không bí mật; các biến bí mật (khóa, mật khẩu, mã bí mật) được đánh dấu "bí mật, không có mặc định" và không ghi giá trị. Khi chạy bằng Docker, `docker-compose.yml` ánh xạ biến môi trường sang khóa cấu hình của ASP.NET Core theo quy ước `Nhom__Khoa`, ví dụ `VNPAY_TMN_CODE` thành `VnPay__TmnCode` và `JWT_SECRET` thành `Jwt__Secret`. Cột "Bắt buộc ở Production" dựa trên `ProductionOptionsValidator` (kiểm tra khi khởi động ở Production), cách khai báo bắt buộc trong `docker-compose.yml` và các khóa mà `render.yaml` đặt.

**Bảng P3.1. Biến cấu hình môi trường**

| Nhóm | Biến | Ý nghĩa | Bắt buộc ở Production | Mặc định |
|---|---|---|---|---|
| CSDL | `DATABASE_URL` | Chuỗi kết nối PostgreSQL (Neon, qua pooler); compose gán sang `ConnectionStrings__DefaultConnection` | Có (`render.yaml` đặt `ConnectionStrings__DefaultConnection`) | Không có; bí mật |
| CSDL | `DATABASE_URL_DIRECT` | Chuỗi kết nối trực tiếp tới Neon, chỉ xuất hiện trong `.env.example` | Không (compose, `render.yaml` và mã nguồn backend không đọc) | Không có; bí mật |
| Runtime | `ASPNETCORE_ENVIRONMENT` | Môi trường chạy (Development, Production, Testing) | Có (`render.yaml` đặt `Production`) | `Development` |
| Runtime | `RUN_MIGRATIONS_ON_STARTUP` | Tự áp dụng migration và seed khi khởi động (khóa `RunMigrationsOnStartup`) | Không bắt buộc (`render.yaml` đặt `true`) | `true` (compose) |
| Runtime | `FRONTEND_BASE_URL` | Địa chỉ gốc của frontend (`App__FrontendBaseUrl`) | Có, phải là URL công khai, không trỏ về localhost | `http://localhost:5173` |
| Runtime | `API_BASE_URL` | Địa chỉ gốc của API (`App__ApiBaseUrl`) | Có, phải là URL công khai, không trỏ về localhost | `http://localhost:8080` |
| Runtime | `CORS_ALLOWED_ORIGIN` | Nguồn được phép gọi API (`Cors__AllowedOrigins__0`) | Có (danh sách nguồn không được rỗng) | `http://localhost:5173` |
| JWT | `JWT_SECRET` | Khóa ký JWT | Có, tối thiểu 32 ký tự; compose khai báo bắt buộc | Không có; bí mật |
| JWT | `JWT_ISSUER` | Issuer của token | Không | `Perfo` |
| JWT | `JWT_AUDIENCE` | Audience của token | Không | `Perfo.Client` |
| JWT | `JWT_ACCESS_TOKEN_EXPIRATION_SECONDS` | Thời hạn access token (giây) | Không | `3600` |
| JWT | `JWT_REFRESH_TOKEN_EXPIRATION_DAYS` | Thời hạn refresh token (ngày) | Không | `30` |
| JWT | `JWT_SOCKET_TOKEN_EXPIRATION_SECONDS` | Thời hạn token kết nối SignalR (giây) | Không | `300` |
| Brand | `BRAND_NAME` | Tên thương hiệu | Không | `Perfo` |
| Brand | `BRAND_DISPLAY_NAME` | Tên hiển thị | Không | `PERFO` |
| Brand | `BRAND_TAGLINE` | Khẩu hiệu | Không | `Vé điện tử · Không cần in` |
| Brand | `BRAND_SUPPORT_EMAIL` | Email hỗ trợ khách hàng | Không | Rỗng |
| Booking, giới hạn tốc độ | `BOOKING_CHECK_IN_OPEN_MINUTES` | Số phút trước giờ chiếu mở soát vé (`Booking__CheckInOpenMinutes`) | Không | `30` |
| Booking, giới hạn tốc độ | `RATE_LIMIT_GLOBAL_SEGMENTS_PER_WINDOW` | Số phân đoạn của cửa sổ giới hạn tốc độ toàn cục | Không | `4` |
| Xếp lịch | `SCHEDULER_DEFAULT_WINDOW_MINUTES` | Độ dài khung thời gian xếp lịch mặc định (phút) | Không | `960` |
| Xếp lịch | `SCHEDULER_SPREAD_MINUTES` | Tham số độ trải của hàm điểm xếp lịch (phút) | Không | `30` |
| Xếp lịch | `SCHEDULER_FIT_WEIGHT` | Trọng số thành phần "khớp" của hàm điểm | Không | `4` |
| Xếp lịch | `SCHEDULER_LOAD_WEIGHT` | Trọng số thành phần "tải" của hàm điểm | Không | `0.5` |
| Tác vụ nền | `BG_RELEASE_EXPIRED_SEATS_CRON` | Lịch cron công việc giải phóng ghế hết hạn | Không | `* * * * *` |
| Tác vụ nền | `BG_SHOWTIME_LIFECYCLE_CRON` | Lịch cron công việc cập nhật vòng đời suất chiếu | Không | `* * * * *` |
| Tác vụ nền | `BG_RETRY_ATTEMPTS` | Số lần thử lại công việc nền | Không | `3` |
| Cloudinary | `CLOUDINARY_CLOUD_NAME` | Tên cloud lưu ảnh, video | Không bắt buộc khi khởi động | Rỗng |
| Cloudinary | `CLOUDINARY_API_KEY` | Khóa API Cloudinary | Không bắt buộc khi khởi động | Rỗng; bí mật |
| Cloudinary | `CLOUDINARY_API_SECRET` | Mã bí mật API Cloudinary | Không bắt buộc khi khởi động | Rỗng; bí mật |
| SMTP | `SMTP_HOST` | Máy chủ SMTP (MailKit) | Không bắt buộc khi khởi động | Rỗng |
| SMTP | `SMTP_PORT` | Cổng SMTP | Không | `587` |
| SMTP | `SMTP_USER` | Tài khoản SMTP | Không bắt buộc khi khởi động | Rỗng |
| SMTP | `SMTP_PASSWORD` | Mật khẩu SMTP | Không bắt buộc khi khởi động | Rỗng; bí mật |
| SMTP | `SMTP_FROM_ADDRESS` | Địa chỉ người gửi | Không | `no-reply@perfo.local` |
| SMTP | `SMTP_FROM_NAME` | Tên người gửi | Không | Rỗng |
| Resend | `RESEND_API_KEY` | Khóa API Resend (gửi thư) | Không bắt buộc khi khởi động | Rỗng; bí mật |
| Resend | `RESEND_FROM_ADDRESS` | Địa chỉ người gửi qua Resend | Không | `onboarding@resend.dev` |
| VNPay | `VNPAY_TMN_CODE` | Mã website (TmnCode) của VNPay | Có điều kiện: nếu đặt một trong `TmnCode`, `HashSecret` thì phải đặt đủ cả hai và hai URL công khai | Rỗng |
| VNPay | `VNPAY_HASH_SECRET` | Khóa bí mật ký giao dịch VNPay | Có điều kiện (như trên) | Rỗng; bí mật |
| VNPay | `VNPAY_BASE_URL` | URL cổng thanh toán VNPay | Không | `https://sandbox.vnpayment.vn/paymentv2/vpcpay.html` |
| VNPay | `VNPAY_RETURN_URL` | URL trả khách về sau thanh toán | Có điều kiện, phải là URL công khai | `http://localhost:5173/booking/result` |
| VNPay | `VNPAY_IPN_URL` | URL nhận thông báo IPN của VNPay | Có điều kiện, phải là URL công khai | `http://localhost:8080/api/v1/payments/vnpay/ipn` |
| VNPay | `VNPAY_API_URL` | URL API truy vấn, hoàn tiền của VNPay | Không | `https://sandbox.vnpayment.vn/merchant_webapi/api/transaction` |
| MoMo | `MOMO_PARTNER_CODE` | Mã đối tác MoMo | Có điều kiện: nếu đặt một trong `PartnerCode`, `AccessKey`, `SecretKey` thì phải đặt đủ cả ba và hai URL công khai | Rỗng |
| MoMo | `MOMO_ACCESS_KEY` | Khóa truy cập MoMo | Có điều kiện (như trên) | Rỗng; bí mật |
| MoMo | `MOMO_SECRET_KEY` | Khóa bí mật MoMo | Có điều kiện (như trên) | Rỗng; bí mật |
| MoMo | `MOMO_ENDPOINT` | URL tạo giao dịch MoMo | Không | `https://test-payment.momo.vn/v2/gateway/api/create` |
| MoMo | `MOMO_RETURN_URL` | URL trả khách về sau thanh toán | Có điều kiện, phải là URL công khai | `http://localhost:5173/booking/result` |
| MoMo | `MOMO_IPN_URL` | URL nhận thông báo IPN của MoMo | Có điều kiện, phải là URL công khai | `http://localhost:8080/api/v1/payments/momo/ipn` |
| MoMo | `MOMO_REFUND_ENDPOINT` | URL hoàn tiền MoMo | Không | `https://test-payment.momo.vn/v2/gateway/api/refund` |
| Stripe | `STRIPE_SECRET_KEY` | Khóa bí mật Stripe | Có điều kiện: nếu đặt thì `ReturnUrl` phải công khai | Rỗng; bí mật |
| Stripe | `STRIPE_PUBLISHABLE_KEY` | Khóa công khai Stripe | Không bắt buộc khi khởi động | Rỗng |
| Stripe | `STRIPE_WEBHOOK_SECRET` | Mã bí mật xác thực webhook Stripe | Không bắt buộc khi khởi động | Rỗng; bí mật |
| Stripe | `STRIPE_RETURN_URL` | URL trả khách về sau thanh toán | Có điều kiện, phải là URL công khai | `http://localhost:5173/booking/result` |
| Stripe | `STRIPE_CURRENCY` | Đơn vị tiền tệ gửi sang Stripe | Không | `vnd` |
| Gemini | `GEMINI_API_KEY` | Khóa API Gemini cho trợ lý AI | Không bắt buộc khi khởi động | Rỗng; bí mật |
| Gemini | `GEMINI_MODEL` | Tên mô hình Gemini | Không | `gemini-2.5-flash` |
| Google | `GOOGLE_CLIENT_ID` | Client ID đăng nhập Google (backend; compose cũng gán cho `VITE_GOOGLE_CLIENT_ID`) | Không bắt buộc khi khởi động | Rỗng |
| Google | `GOOGLE_CLIENT_SECRET` | Mã bí mật OAuth Google | Không bắt buộc khi khởi động | Rỗng; bí mật |
| Frontend | `VITE_API_BASE_URL` | URL gốc của REST API (đọc lúc build) | Có (đặt trên Vercel/Render theo `render.yaml`) | `http://localhost:8080/api/v1` |
| Frontend | `VITE_SIGNALR_HUB_URL` | URL của hub SignalR | Có (như trên) | `http://localhost:8080/hubs/showtimes` |
| Frontend | `VITE_GOOGLE_CLIENT_ID` | Client ID Google phía frontend | Không bắt buộc | Rỗng |
| Frontend | `VITE_SITE_URL` | URL công khai của trang web (dùng cho SEO, sitemap) | Có (`frontend/.env.example` hướng dẫn đặt trên Vercel) | `http://localhost:5173` |
| Frontend | `VITE_BUSINESS_TIME_ZONE` | Múi giờ nghiệp vụ | Không | `Asia/Ho_Chi_Minh` |
| Frontend | `VITE_LOCALE` | Ngôn ngữ, định dạng hiển thị | Không | `vi-VN` |
| Frontend | `VITE_CURRENCY_CODE` | Mã tiền tệ | Không | `VND` |
| Frontend | `VITE_CURRENCY_SYMBOL` | Ký hiệu tiền tệ | Không | `VNĐ` |
| E2E | `E2E_BASE_URL` | Địa chỉ ứng dụng mà Playwright truy cập | Không áp dụng (chỉ kiểm thử) | `http://localhost:5173` |
| E2E | `E2E_CUSTOMER_EMAIL` | Email tài khoản khách dùng trong E2E | Không áp dụng | `customer@cinema.local` |
| E2E | `E2E_CUSTOMER_PASSWORD` | Mật khẩu tài khoản khách dùng trong E2E | Không áp dụng | Xem `frontend/.env.example`; bí mật |
| E2E | `E2E_ADMIN_EMAIL` | Email tài khoản quản trị dùng trong E2E | Không áp dụng | `admin@cinema.local` |
| E2E | `E2E_ADMIN_PASSWORD` | Mật khẩu tài khoản quản trị dùng trong E2E | Không áp dụng | Xem `frontend/.env.example`; bí mật |
| E2E | `E2E_DATABASE_URL` | Chuỗi kết nối CSDL cho các hàm hỗ trợ E2E (`runSql`); nếu vắng thì dùng `DATABASE_URL` | Không áp dụng | Không có; bí mật |
| E2E | `E2E_PSQL_IMAGE` | Ảnh Docker chứa `psql` cho hàm hỗ trợ E2E | Không áp dụng | `postgres:17-alpine` |

Các khóa cấu hình sau chỉ có trong `appsettings.json` và không có biến môi trường đặt tên riêng trong `.env.example`, `docker-compose.yml` hay `render.yaml`: `BackgroundJobs:ExpireLoyaltyPointsCron` (mặc định `5 * * * *`), `BackgroundJobs:RetryDelaysSeconds` (mặc định `[30, 120, 600]`), các giới hạn tốc độ `RateLimit:GlobalPermitLimit` (100), `GlobalWindowSeconds` (60), `AuthPermitLimit` (5), `BookingHoldPermitLimit` (10) cùng các cửa sổ thời gian tương ứng, `Booking:*` (kích thước trang, giới hạn xóa hàng loạt…), `Cloudinary:*` (kích thước và định dạng tệp cho phép) và `Email:*` (thời hạn xác nhận email, đặt lại mật khẩu). Theo cơ chế cấu hình của ASP.NET Core, các khóa này vẫn có thể được ghi đè bằng biến môi trường dạng `BackgroundJobs__ExpireLoyaltyPointsCron`.

---

# PHỤ LỤC 4. Kịch bản E2E chi tiết

## P4.1 CÁC BƯỚC TỪNG KỊCH BẢN

Bảng P4.1 liệt kê từng bước của tám kịch bản E2E, đọc trực tiếp từ bảy tệp `*.spec.ts` và các hàm hỗ trợ trong `frontend/e2e/helpers.ts`; các bước khớp với Bảng 6.15 đến Bảng 6.22. Cấu hình chung (`playwright.config.ts`): thời hạn mỗi kịch bản 60 giây, thời hạn mỗi lệnh `expect` 10 giây, `fullyParallel: false` và `workers: 1`, Chromium (Desktop Chrome), `baseURL` lấy từ `E2E_BASE_URL`. Các hàm đăng nhập `loginAsCustomer` và `loginAsAdmin` chờ API sẵn sàng (`/health`, tối đa 60 giây), thử đăng nhập tối đa ba lần, mỗi lần chờ chuyển khỏi trang đăng nhập tối đa 15 giây; `loginAsCustomer` còn hủy các booking Pending tồn đọng trước khi đăng nhập (`ensureNoPendingBookings`). Cột "Timeout, điều kiện bỏ qua" ghi các giới hạn thời gian ghi rõ trong spec (đơn vị giây) và điều kiện `test.skip`.

**Bảng P4.1. Các bước kịch bản E2E**

| Kịch bản | Bước | Thao tác | Kiểm tra | Timeout, điều kiện bỏ qua |
|---|---|---|---|---|
| E2E-01 (`customer-booking-flow.spec.ts`) | 1 | Đăng nhập khách (`loginAsCustomer`) | Trình duyệt chuyển khỏi `/auth/login` | 15 mỗi lần đăng nhập |
| E2E-01 | 2 | Mở `/movies`, nhấn liên kết phim đầu tiên (`a[href^="/movies/"]`) | Liên kết hiển thị; URL khớp `/movies/\d+` | 10; 10 |
| E2E-01 | 3 | Cuộn tới `#showtimes`, nhấn suất chiếu khả dụng đầu tiên (`button[data-slot="showtime-slot"]` không bị `disabled`) | Nút suất chiếu hiển thị; URL khớp `/booking/\d+` | 10; 10 |
| E2E-01 | 4 | Chọn hai ghế khả dụng đầu tiên (`button[aria-label^="Ghế "]`); nếu hiện cảnh báo "Không thể để trống ghế lẻ" thì chọn thêm ghế thứ ba | Ghế đầu tiên hiển thị; "Chi tiết đơn hàng" và "Ghế (n)" hiển thị | 10; bỏ qua nếu có ít hơn 2 ghế khả dụng |
| E2E-01 | 5 | Nếu có nút cộng của mục "Chọn combo" thì nhấn một lần | Chữ "Combo" hiển thị | Chỉ thực hiện khi nút hiển thị |
| E2E-01 | 6 | Nhấn "Giữ ghế & tiếp tục thanh toán" | Nút ở trạng thái cho phép nhấn trước khi nhấn | — |
| E2E-01 | 7 | Ở bước thanh toán, nếu có nút "MoMo" rồi "VNPay" thì lần lượt nhấn | "Giữ ghế còn" hiển thị; nút "Thanh toán" hiển thị; nút phương thức vừa nhấn có lớp `border-primary` | 10; 10; chỉ khi nút hiển thị |
| E2E-01 | 8 | Nhấn "Huỷ, chọn lại ghế" | Nút hủy hiển thị trước khi nhấn; nút "Giữ ghế & tiếp tục thanh toán" hiển thị lại | 10 |
| E2E-01 | 9 | Mở `/account/tickets` | Tiêu đề "Vé của tôi" hiển thị; nút "Tất cả" hiển thị | 10 |
| E2E-02 (`customer-payment-failure-flow.spec.ts`) | 1 | Đăng nhập khách | Chuyển khỏi trang đăng nhập | 15 mỗi lần |
| E2E-02 | 2 | Lấy token qua API (`getCustomerApiToken`) | Có token | Bỏ qua nếu không xác thực được |
| E2E-02 | 3 | Giữ ghế đầu tiên khả dụng qua API (`holdFirstAvailableSeat`) | Có booking và mã booking | Bỏ qua nếu suất 1 không còn ghế |
| E2E-02 | 4 | Hủy booking qua API (`cancelBookingViaApi`) | Hủy thành công | Bỏ qua nếu không hủy được |
| E2E-02 | 5 | Mở `/booking/result?bookingId=` với mã booking vừa hủy | Tiêu đề "Đơn đặt vé đã bị huỷ"; chữ "Giao dịch không thành công hoặc đã bị huỷ"; mã booking hiển thị | 10 (tiêu đề) |
| E2E-02 | 6 | Kiểm tra phần `main` | Chữ "Vé điện tử" không hiển thị | — |
| E2E-02 | 7 | Kiểm tra liên kết | Liên kết "Xem vé của tôi" và "Về trang chủ" hiển thị | — |
| E2E-03 (`customer-payment-failure-flow.spec.ts`) | 1 | Đăng nhập khách | Chuyển khỏi trang đăng nhập | 15 mỗi lần |
| E2E-03 | 2 | Mở `/booking/result?bookingId=999999999` | Chữ "Không tìm thấy đơn đặt vé này" hiển thị | 10 |
| E2E-03 | 3 | Kiểm tra nút | Nút "Thử lại" hiển thị | — |
| E2E-04 (`customer-seat-hold-timeout-flow.spec.ts`) | 1 | Đăng nhập khách | Chuyển khỏi trang đăng nhập | Timeout kịch bản 180; đăng nhập 15 mỗi lần |
| E2E-04 | 2 | Mở `/movies`, nhấn phim đầu tiên | Liên kết phim hiển thị; URL khớp `/movies/\d+` | 10; 10 |
| E2E-04 | 3 | Cuộn tới `#showtimes`, nhấn suất chiếu khả dụng đầu tiên | Nút suất chiếu hiển thị; URL khớp `/booking/\d+` | 10; 10 |
| E2E-04 | 4 | Chọn ghế khả dụng đầu tiên | Ghế hiển thị | 10; bỏ qua nếu không có ghế khả dụng |
| E2E-04 | 5 | Nhấn "Giữ ghế & tiếp tục thanh toán" | Nút cho phép nhấn; "Giữ ghế còn" hiển thị | 10 |
| E2E-04 | 6 | Lấy token API, lấy mã booking Pending mới nhất (`getLatestPendingBookingId`) | Có token và mã booking | Bỏ qua nếu thiếu token hoặc không xác định được booking |
| E2E-04 | 7 | Ép booking hết hạn trong CSDL (`expireBookingHoldInDb`) | Lệnh SQL thực hiện thành công | Bỏ qua nếu không truy cập được CSDL |
| E2E-04 | 8 | Chờ thông báo trên giao diện | Chữ "Thời gian giữ ghế đã hết hạn, vui lòng chọn lại." hiển thị | 120 |
| E2E-04 | 9 | Kiểm tra quay lại bước chọn ghế | Nút "Giữ ghế & tiếp tục thanh toán" hiển thị; "Giữ ghế còn" không hiển thị | 10 |
| E2E-05 (`customer-profile-flow.spec.ts`) | 1 | Đăng nhập khách | Chuyển khỏi trang đăng nhập | 15 mỗi lần |
| E2E-05 | 2 | Mở `/account/profile` | Tiêu đề "Thông tin cá nhân" hiển thị | 10 |
| E2E-05 | 3 | Điền "Họ và tên" bằng `Khách Hàng Test` kèm 4 chữ số cuối của `Date.now()`; điền "Số điện thoại" `0987654321` | — | — |
| E2E-05 | 4 | Nhấn "Lưu thay đổi" | Thông báo "Cập nhật thông tin thành công" hiển thị | 10 |
| E2E-05 | 5 | Tải lại trang | Ô "Họ và tên" giữ giá trị vừa lưu | 10 |
| E2E-05 | 6 | Nhấn liên kết "Vé đã mua" | URL khớp `/account/tickets`; tiêu đề "Vé của tôi" hiển thị | 10; 10 |
| E2E-05 | 7 | Nếu nút "Đã thanh toán" hiển thị thì nhấn; sau đó nhấn "Tất cả" | Nút vừa nhấn có lớp `border-primary` | Tab "Đã thanh toán" chỉ kiểm tra khi hiển thị |
| E2E-06 (`admin-checkin-flow.spec.ts`) | 1 | Đăng nhập quản trị (`loginAsAdmin`, trang `/admin/login`) | Chuyển khỏi `/admin/login` | Timeout kịch bản 120; 15 mỗi lần |
| E2E-06 | 2 | Nhấn liên kết "Soát vé" | URL khớp `/admin/checkin`; tiêu đề "Soát vé & Check-in" hiển thị; chữ "Chưa có vé nào được soát" hiển thị | 10; 10; 5 |
| E2E-06 | 3 | Nhập mã `INVALID-QR-TEST-99999`, nhấn "Soát vé ngay" | Chữ "Soát vé thất bại" hiển thị | 10 |
| E2E-06 | 4 | Tạo vé đủ điều kiện soát (`createEligibleTicketForCheckin`), nhập mã QR của vé, nhấn "Soát vé ngay" | "Check-in thành công"; tên phim và ghế (hàng cộng số) hiển thị | 10; bỏ qua nếu không tạo được vé |
| E2E-06 | 5 | Nhập lại cùng mã QR, nhấn "Soát vé ngay" | "Soát vé thất bại"; chữ "Vé đã check-in trước đó" hoặc "đã được sử dụng" hiển thị | 10; 10 |
| E2E-07 (`admin-movies-flow.spec.ts`) | 1 | Đăng nhập quản trị | Chuyển khỏi `/admin/login` | 15 mỗi lần |
| E2E-07 | 2 | Nhấn liên kết "Quản lý phim" | URL khớp `/admin/movies`; tiêu đề "Quản lý phim" hiển thị | 10; 10 |
| E2E-07 | 3 | Nhập "Oppenheimer" vào ô "Tìm theo tên phim...", chờ 500 ms, rồi xóa ô tìm kiếm (chờ thêm 500 ms) | Ô tìm kiếm hiển thị; chữ "Oppenheimer" hiển thị | 10 |
| E2E-07 | 4 | Nhấn "Thêm phim" | Chữ "Nhập thông tin chi tiết của bộ phim." hiển thị | 5 |
| E2E-07 | 5 | Điền tên phim `E2E Phim Mới` kèm 5 chữ số cuối của `Date.now()`, thời lượng 130, mô tả; mở ô chọn thể loại, chọn tùy chọn đầu tiên, nhấn Escape | Tùy chọn thể loại đầu tiên hiển thị | 5 |
| E2E-07 | 6 | Nhấn nút gửi "Thêm phim" | Thông báo "Đã thêm phim mới thành công" hiển thị | 10 |
| E2E-07 | 7 | Tìm theo tên phim vừa tạo (chờ 600 ms) | Dòng của phim hiển thị trong bảng | 10 |
| E2E-07 | 8 | Mở menu "Mở menu" của dòng, chọn "Xóa phim", xác nhận bằng nút "Xóa phim" | Nút xác nhận hiển thị; thông báo "Đã xóa phim thành công"; dòng phim biến mất | 10; 10 |
| E2E-08 (`admin-showtimes-flow.spec.ts`) | 1 | Xóa các suất chiếu thử nghiệm tương lai (`cleanFutureTestShowtimes`), đăng nhập quản trị | Chuyển khỏi `/admin/login` | Timeout kịch bản 120; 15 mỗi lần |
| E2E-08 | 2 | Nhấn liên kết "Suất chiếu" | URL khớp `/admin/showtimes`; tiêu đề "Quản lý Suất chiếu" hiển thị | 10; 10 |
| E2E-08 | 3 | Nhấn tab "Lịch" rồi tab "Danh sách" | Vùng "Lịch" hiển thị; bảng hiển thị | 5; 10 |
| E2E-08 | 4 | Nhấn "Thêm suất chiếu" | Tiêu đề "Tạo suất chiếu mới" hiển thị | 5 |
| E2E-08 | 5 | Mở ô "Chọn phim", chọn tùy chọn đầu tiên (ghi nhớ tên phim) | Tùy chọn hiển thị | 5 |
| E2E-08 | 6 | Mở ô "Chọn phòng chiếu", chọn tùy chọn đầu tiên | Tùy chọn hiển thị | 5 |
| E2E-08 | 7 | Mở bộ chọn ngày giờ, chọn ngày cuối cùng không bị vô hiệu, nhập giờ `10:00`, nhấn "Xong" | Ngày hiển thị; ô giờ và nút "Xong" cho phép thao tác | 5; 5 |
| E2E-08 | 8 | Nhập "Giá vé" `95000` | — | — |
| E2E-08 | 9 | Nhấn "Tạo suất chiếu" | Thông báo "Đã tạo suất chiếu mới" hiển thị | 10 |
| E2E-08 | 10 | Tìm dòng đầu tiên trong thân bảng chứa tên phim đã chọn và có nút "Mở menu" | Dòng hiển thị | 10 |
| E2E-08 | 11 | Mở "Mở menu", chọn "Hủy suất chiếu", xác nhận trong hộp thoại bằng nút "Hủy suất chiếu" | Hộp thoại hiển thị rồi biến mất; thông báo "Đã hủy suất chiếu" (mẫu cho phép các cách viết dấu khác nhau) hiển thị | 5; 10; 10 |

<!-- GHI CHÚ ĐỐI CHIẾU -->
<!--
(a) [CẦN BỔ SUNG] còn lại
- P1.1 đến P1.8: xác nhận lệnh chạy được trên máy sạch (chưa chạy lệnh nào, chỉ đọc cấu hình).
- P1.2: chưa rõ có cần `npm install` riêng trong `frontend/` hay không (frontend/package.json có phụ thuộc, devDependencies và package-lock riêng; script `setup` chỉ `npm install` ở gốc).
- P1.4: cách backend chạy thủ công nhận chuỗi kết nối CSDL (`DATABASE_URL` chỉ được docker-compose ánh xạ sang `ConnectionStrings__DefaultConnection`; chưa xác minh `dotnet run` có nạp `.env` hay không; có `appsettings.Local.json.example` nhưng chưa đọc cách nạp).
- P1.6: test tích hợp cần Docker/Testcontainers (theo outline Chương 6), chưa xác minh trong mã.
- Bảng P3.1: cột "Bắt buộc ở Production" cho nhóm "Không bắt buộc khi khởi động" là suy ra từ việc ProductionOptionsValidator không kiểm tra; cần sinh viên xác nhận theo cách triển khai thực tế. `render.yaml` chỉ đọc tên khóa, không đọc giá trị.

(b) Chỗ outline lệch code / lệch yêu cầu
1. P1.5 (outline): chạy `Migrations/_manual/backfill_loyalty_entries.sql` sau migration `AddLoyaltyPointEntries`. Code: không có thư mục `Migrations/_manual` và không có migration `AddLoyaltyPointEntries`; chỉ có `20261106040650_InitialCreate` (bảng sổ điểm nằm trong migration này). Đã bỏ bước backfill khỏi P1.5; cần sửa outline (cũng liên quan Mục 7.3.15).
2. P1.7: outline ghi "Email theo SeedEmails ... xem DbInitializer.cs"; yêu cầu phần này chỉ lấy tài khoản trong `SeedData.cs`. `SeedData.cs` chỉ có một tài khoản quản trị (`admin@cinema.local`); các email khách/nhân viên (SeedEmails trong DbInitializer.cs: customer, customer2…4, newbie, inactive, unverified, google, employee, manager…) chỉ được nhắc chung, không liệt kê. Không in mật khẩu (mật khẩu của E2E nằm trong .env.example/helpers.ts nên không chép).
3. P3.1 (outline): ghi chú "biến cho VNPay, MoMo, Stripe, Gemini, Google chỉ thấy trong appsettings.json, không có trong .env.example". Code: các biến này CÓ trong `.env.example`, `docker-compose.yml` và `render.yaml`; đã thêm các nhóm VNPay, MoMo, Stripe, Gemini, Google vào bảng.
4. P3.1 (outline) liệt kê `SCHEDULER_STEP_MINUTES`, `SCHEDULER_MIN_SAME_MOVIE_GAP_MINUTES`, `SCHEDULER_HEAT_LOOKBACK_DAYS`: không có trong `.env.example`, `docker-compose.yml`, `render.yaml` (chỉ có DEFAULT_WINDOW_MINUTES, SPREAD_MINUTES, FIT_WEIGHT, LOAD_WEIGHT). SchedulerOptions ghi chú các thuộc tính chính sách này bị ShowtimeService ghi đè bằng CinemaSettings. Đã bỏ ba biến này khỏi bảng.
5. P3.1: `DATABASE_URL_DIRECT` chỉ có trong `.env.example`, không được compose, render.yaml hay mã .cs đọc; `.env.example` ghi `JWT_SECRET` tối thiểu 64 ký tự trong khi `ProductionOptionsValidator` chỉ yêu cầu 32. `E2E_DATABASE_URL`, `E2E_PSQL_IMAGE` có trong `helpers.ts` nhưng không có trong outline và `.env.example`; đã thêm vào bảng. Biến `ExpireLoyaltyPointsCron` không có biến môi trường đặt tên riêng (kiểm tra: chỉ có trong appsettings.json và BackgroundJobOptions).
6. P4.1: Bảng 6.15 đến 6.22 trong outline chưa có nội dung (chỉ "[CẦN BỔ SUNG: đọc spec]"); P4.1 được lập từ spec, cần đồng bộ lại Chương 6 theo bảng này (đặc biệt 6.4.3 đến 6.4.10).
7. P4.1: outline ghi thời hạn kịch bản đặc biệt chỉ "E2E-04: 180 giây; E2E-06: 120 giây"; spec `admin-showtimes-flow.spec.ts` (E2E-08) cũng đặt `test.setTimeout(120_000)`. Cần sửa Mục 6.4.1.
8. P4.1: outline (Mục 6.4.10) ghi E2E-08 "hủy suất ở dòng đầu bảng"; spec chọn dòng đầu tiên trong các dòng thân bảng chứa tên phim đã chọn và có nút "Mở menu", không phải dòng đầu của bảng (và không nhất thiết là suất vừa tạo). Mục 6.4.16 cũng ghi "E2E-08 hủy dòng đầu tiên của bảng". Cần xác minh/sửa.
9. P4.1: outline ghi tệp spec ở `bo_sung/e2e/`; trong perfo-main.zip các tệp nằm ở `frontend/e2e/` (không có thư mục bo_sung). Outline ghi `helpers.ts` 835 dòng; bản trong zip có 401 dòng. Đã dẫn nguồn theo đường dẫn thực tế.
10. P1.1/P1.8: comment đầu khối Hangfire trong `Program.cs` còn ghi "SQL Server" trong khi mã dùng `UsePostgreSqlStorage` (đã thuộc Mục 7.3.9); Phụ lục này ghi theo mã (PostgreSQL).
11. Tệp quy định trình bày được cung cấp có tên `quy-dinh-format-bao-cao.md` (prompt ghi `quy-dinh-trinh-bay.md`); không có README trong repository; không có `appsettings*.json` nào ngoài `appsettings.json` (có `appsettings.Local.json.example`).
-->

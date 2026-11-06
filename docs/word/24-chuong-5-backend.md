# CHƯƠNG 5. CÀI ĐẶT HỆ THỐNG

Chương này trình bày việc hiện thực hóa các thiết kế ở Chương 4 thành mã nguồn chạy được. Mục 5.1 mô tả môi trường và công cụ phát triển, Mục 5.2 mô tả cấu trúc và quy mô mã nguồn, Mục 5.3 trình bày cách cài đặt từng module của backend. Mọi tên lớp, phương thức, endpoint và giá trị cấu hình trong chương được lấy từ mã nguồn của dự án.

## 5.1 MÔI TRƯỜNG PHÁT TRIỂN

### 5.1.1 Môi trường công cụ

Hệ thống gồm backend viết bằng C# trên nền tảng .NET (`net10.0`) và frontend viết bằng React với TypeScript, được xây dựng bằng Vite. Tệp `package.json` ở thư mục gốc khai báo yêu cầu Node.js từ phiên bản `22.12.0` trở lên (`engines.node`). Mã backend được tổ chức trong solution `backend/Cinema.slnx`. Cơ sở dữ liệu là PostgreSQL, truy cập qua Entity Framework Core với provider Npgsql (`Npgsql.EntityFrameworkCore.PostgreSQL` 10.0.3); tệp `docker-compose.yml` ghi chú rằng cơ sở dữ liệu là dịch vụ Neon trên đám mây nên Docker Compose chỉ dựng hai dịch vụ là `api` và `frontend`.

Công cụ dòng lệnh của .NET được khóa phiên bản trong `dotnet-tools.json`: CSharpier `1.3.0` dùng để định dạng mã C# và `dotnet-ef` `10.0.11` dùng để quản lý migration. Phía frontend dùng ESLint (`^10.9.0`) để kiểm tra mã, TypeScript (`~6.0.2`) và Vite (`^8.2.2`) để biên dịch, Playwright (`^1.63.0`) cho kiểm thử đầu cuối. Các thư viện chính của backend gồm Hangfire (`1.8.24`) với `Hangfire.PostgreSql` (`1.21.1`), FluentValidation (`12.1.1`), Mapster (`10.0.11`), MailKit (`4.17.0`), CloudinaryDotNet (`1.29.3`), Stripe.net (`52.4.0`), Google.Apis.Auth (`1.76.0`) và QRCoder (`1.8.0`); phía frontend gồm React (`^19.2.8`), React Router (`^7.18.2`), TanStack Query (`^5.102.8`), Axios (`^1.20.0`) và `@microsoft/signalr` (`^10.0.11`).

**Bảng 5.1. Môi trường phát triển**

| Nhóm | Nội dung | Ý nghĩa / nguồn |
|---|---|---|
| Runtime | .NET `net10.0`; Node.js `>=22.12.0` | `TargetFramework` trong các tệp `.csproj`; `engines` trong `package.json` |
| CSDL | PostgreSQL (dịch vụ Neon trên đám mây) | Provider Npgsql; Hangfire dùng chung PostgreSQL để lưu hàng đợi |
| Container | Backend: Dockerfile bốn stage `restore`, `build`, `dev`, `production` (cổng 8080); frontend: hai stage `deps`, `dev` (cổng 5173); `docker-compose.yml` dựng `api`, `frontend` trên mạng `perfo-net` | `backend/Dockerfile`, `frontend/Dockerfile`, `docker-compose.yml` |
| Công cụ định dạng | CSharpier 1.3.0; ESLint | `dotnet-tools.json`; lệnh `format`, `format:check`, `lint` |
| Công cụ migration | dotnet-ef 10.0.11 | Các lệnh `db:*` |
| Kiểm thử | xUnit-style dự án `Cinema.UnitTests`, `Cinema.IntegrationTests`; Playwright 1.63; k6 qua PowerShell | Lệnh `test:*` |
| Lệnh | Xem danh sách nhóm lệnh bên dưới | `package.json` |

Các nhóm lệnh `npm` có trong `package.json` ở thư mục gốc như sau.

- `setup`, `setup:env`, `setup:tools`, `setup:backend`, `setup:browsers`: cài đặt môi trường sau khi sao chép mã nguồn; `setup` gộp `npm install`, tạo tệp môi trường, `dotnet tool restore` và `dotnet restore`.
- `dev`, `dev:backend`, `dev:backend:once`, `dev:frontend`: chạy thủ công không qua Docker; `dev` chạy đồng thời backend (`dotnet watch`) và frontend (`vite`) bằng `concurrently`.
- `build`, `build:backend`, `build:frontend`, `build:frontend:full`, `preview:frontend`, `sitemap:frontend`, `clean:backend`: biên dịch và xem trước; `build:frontend:full` biên dịch rồi sinh sitemap.
- `lint`, `typecheck`, `format`, `format:check`, `check`: kiểm tra chất lượng mã; `check` gộp kiểm tra định dạng, ESLint và biên dịch.
- `docker:*` (`up`, `up:fg`, `down`, `restart`, `ps`, `build`, `build:nocache`, `logs`, `logs:api`, `logs:frontend`, `exec:api`, `exec:frontend`, `prune`): điều khiển Docker Compose.
- `db:migration:add`, `db:migration:remove`, `db:migration:list`, `db:migration:script`, `db:update`, `db:drop`, `db:reset`, `db:info`: quản lý migration và cơ sở dữ liệu bằng `dotnet ef` với dự án `Cinema.Infrastructure` làm nơi chứa migration và `Cinema.Api` làm dự án khởi động.
- `test:unit`, `test:integration`, `test:backend`, `test:backend:watch`, `test:backend:coverage`, `test:e2e` (cùng các biến thể `:ui`, `:headed`, `:debug`, `:report`), `test:load:browse`, `test:load:peak`, `test:load`, `test`, `test:all`: các cấp kiểm thử.
- `ui:add`: thêm thành phần giao diện bằng `shadcn`.

Tệp `frontend/package.json` có thêm hai lệnh `build:full` và `sitemap:build` (cùng chức năng với `build:frontend:full` và `sitemap:frontend` ở thư mục gốc). Các kịch bản kiểm thử tải được khai báo chạy qua PowerShell với k6 (`load-tests/run-load-test.ps1`, hai kịch bản `browse-movies-stress.js` và `peak-ticket-rush.js`); kết quả các cấp kiểm thử được trình bày ở Chương 6.

## 5.2 CẤU TRÚC MÃ NGUỒN

### 5.2.1 Cây thư mục

Thư mục gốc của dự án chứa hai phần độc lập là `backend/` và `frontend/`, cùng các tệp dùng chung gồm `docker-compose.yml`, `.env.example`, `package.json`, `dotnet-tools.json`, `.editorconfig`, `render.yaml` và `vercel.json` (cấu hình triển khai). Cấu trúc các thư mục chính được minh họa ở Hình 5.1.

![Hình 5.1. Cấu trúc thư mục chính của dự án](hinh/5.1.png)

*Hình 5.1. Cấu trúc thư mục chính của dự án*

Backend theo kiến trúc nhiều tầng, mỗi tầng là một dự án trong `backend/` (xem thiết kế tổng thể ở Chương 4). `Cinema.Domain` chứa các thực thể (`Entities/`) và kiểu liệt kê (`Enums/`), không phụ thuộc tầng nào khác. `Cinema.Shared` chứa hằng số (`Constants/`), các ngoại lệ nghiệp vụ (`Exceptions/`), kiểu kết quả phân trang (`Results/`) và tài nguyên thông điệp (`Resources/`). `Cinema.Application` chứa logic nghiệp vụ: các dịch vụ trong `Services/` (mỗi module một thư mục con), tác vụ nền trong `BackgroundJobs/`, lớp cấu hình mạnh kiểu trong `Options/`, tiện ích thuần trong `Utils/`, bộ kiểm tra hợp lệ trong `Validators/`, các DTO trong `DTOs/`, giao diện trừu tượng trong `Interfaces/` và cấu hình ánh xạ trong `MappingConfig/`. `Cinema.Infrastructure` hiện thực các giao diện đó với thế giới bên ngoài: `Persistence/` (DbContext, cấu hình thực thể, dữ liệu khởi tạo), `Migrations/`, `Auth/` (xác minh Google), `Email/`, `FileStorage/`, `Payments/` và `Realtime/`. `Cinema.Api` là điểm vào ASP.NET Core, gồm `Program.cs`, thư mục `Controllers/` (con `Admin/`, `Client/`, cùng `HealthController` và `PaymentsController`), `Filters/` và `Middlewares/`. Thư mục `backend/tests/` gồm hai dự án `Cinema.UnitTests` và `Cinema.IntegrationTests`.

Frontend nằm trong `frontend/src/` với các thư mục `api/`, `app/`, `components/`, `config/`, `features/`, `hooks/`, `layouts/`, `lib/`, `pages/`, `stores/`, `types/` và `assets/`, cùng điểm vào `main.tsx`, `App.tsx`. Tệp cấu hình của frontend (`vite.config.ts`, `eslint.config.js`, `playwright.config.ts`, `tsconfig*.json`, `components.json`) nằm ở `frontend/`.

### 5.2.2 Quy mô mã nguồn

Số liệu ở Bảng 5.2 được đếm ngày 05/11/2026 trên bản mã nguồn `perfo-main/` bằng lệnh `find` kết hợp `wc -l` (công cụ `cloc` không có sẵn trong môi trường đếm). Số dòng là tổng số dòng vật lý của tệp, tính cả dòng trống và dòng chú thích; vì vậy các con số có thể lớn hơn số dòng mã thực thi.

**Bảng 5.2. Quy mô mã nguồn**

| Hạng mục | Giá trị | Cách đếm |
|---|---|---|
| Backend (C#, không gồm migration, test, seed) | 270 tệp; 22.293 dòng | `find backend -name '*.cs' -not -path '*/Migrations/*' -not -path '*/tests/*' -not -path '*/Seed/*' -not -path '*/bin/*' -not -path '*/obj/*' \| xargs wc -l` |
| Frontend (TS/TSX) | 438 tệp; 33.278 dòng, trong đó 296 tệp `.tsx` (24.977 dòng) | `find frontend/src \( -name '*.ts' -o -name '*.tsx' \) \| xargs wc -l`; riêng `.tsx` lọc theo `-name '*.tsx'` |
| Test (C#) | 52 tệp; 10.484 dòng | `find backend/tests -name '*.cs' -not -path '*/bin/*' -not -path '*/obj/*' \| xargs wc -l` |
| Seed (C#) | 4 tệp; 3.831 dòng | `find backend -path '*/Seed/*' -name '*.cs' \| xargs wc -l` |
| Thực thể / enum / migration | 30 / 10 / 1 | Đếm tệp trong `Domain/Entities`, `Domain/Enums`; đếm lớp migration (không tính tệp `.Designer.cs` và snapshot) trong `Infrastructure/Migrations` |
| Controller / endpoint | 33 / 137 (Admin 93, Client 39, khác 5) | Đếm tệp trong `Cinema.Api/Controllers` (22 Admin, 9 Client, `HealthController`, `PaymentsController`, không tính `AppControllerBase`); đếm thuộc tính `[HttpGet/Post/Put/Patch/Delete]` bằng `grep` |

Phạm vi loại trừ: thư mục `bin/`, `obj/`, `node_modules/`, tệp sinh tự động của migration (`Migrations/`), mã kiểm thử, dữ liệu khởi tạo (đếm riêng) và các tệp không phải mã nguồn (JSON, Markdown, cấu hình).

## 5.3 CÀI ĐẶT BACKEND

Các module dưới đây được cài đặt theo cùng một mẫu: controller mỏng nhận yêu cầu và ủy quyền cho một lớp dịch vụ trong `Cinema.Application/Services`; dịch vụ truy cập dữ liệu qua giao diện `IAppDbContext`, dùng FluentValidation cho kiểm tra đầu vào và ném các ngoại lệ nghiệp vụ như `ValidationAppException`, `ConflictAppException`, `BusinessRuleException`, `NotFoundException`, `ForbiddenAppException`, `UnauthorizedAppException` để lớp xử lý lỗi chung chuyển thành mã phản hồi HTTP. Các chính sách phân quyền `CustomerOnly`, `AdminOnly`, `EmployeeOnly`, `StaffOnly` (quản trị viên hoặc nhân viên) được khai báo trong `Program.cs`. Ba chính sách giới hạn tần suất được cấu hình qua `RateLimitOptions`: toàn cục (mặc định 100 yêu cầu mỗi 60 giây), `auth` (mặc định 5 yêu cầu mỗi 60 giây) và `booking-hold` (mặc định 10 yêu cầu mỗi 60 giây); vượt giới hạn trả về mã 429.

### 5.3.1 Module xác thực

Module cung cấp đăng ký, xác nhận email, đăng nhập khách hàng, đăng nhập cổng quản trị, đăng nhập Google, làm mới token, đăng xuất, quên và đặt lại mật khẩu, gửi lại email xác nhận, lấy thông tin tài khoản (`GET me`) và cấp token cho kết nối thời gian thực. Các endpoint thuộc `AuthController` với tiền tố `api/v1/auth`: `register`, `confirm-email`, `login`, `admin-login`, `google`, `refresh`, `logout`, `forgot-password`, `reset-password`, `resend-confirmation`, `socket-token` (yêu cầu đăng nhập) và `me` (yêu cầu đăng nhập). Bảy endpoint đầu vào chính (`register`, `login`, `admin-login`, `google`, `refresh`, `forgot-password`, `reset-password`) áp dụng chính sách giới hạn tần suất `auth`.

Các thành phần chính là `AuthService` (nghiệp vụ), `JwtTokenService` (cài đặt `ITokenService`) và `GoogleAuthService` trong `Cinema.Infrastructure/Auth`. Khi đăng ký, `RegisterAsync` kiểm tra email chưa tồn tại, gán vai trò `Customer`, băm mật khẩu bằng `IPasswordHasher<User>`, tạo kèm `CustomerProfile` với `Points = 0`, `TotalSpent = 0`, rồi phát hành mã xác thực loại `VerificationTokenPurpose.EmailConfirmation` có thời hạn `EmailConfirmationExpirationHours` (mặc định 24 giờ) và gửi email chứa liên kết. `ConfirmEmailAsync` chấp nhận mã chưa dùng và chưa hết hạn để đặt `EmailConfirmed = true`. `LoginAsync` dùng chung cho hai cổng đăng nhập thông qua tham số `adminOnly`: sau khi kiểm tra mật khẩu, trạng thái hoạt động và email đã xác nhận, tài khoản khách hàng bị từ chối ở cổng quản trị và ngược lại, tài khoản nhân viên bị từ chối ở cổng khách hàng. Nếu kết quả kiểm tra băm là `SuccessRehashNeeded` thì mật khẩu được băm lại và lưu.

Access token là JWT ký HMAC-SHA256, mang các claim mã người dùng, vai trò, `sub`, email và mã nhân viên (nếu có); thời hạn mặc định `AccessTokenExpirationSeconds = 3600`. Refresh token là chuỗi ngẫu nhiên 32 byte (`RandomNumberGenerator.GetBytes(32)`), được băm trước khi lưu (`HashOpaqueToken`), hết hạn sau `RefreshTokenExpirationDays` (mặc định 30 ngày). `RefreshAsync` áp dụng cơ chế xoay vòng: phát hành refresh token mới, đánh dấu token cũ `IsRevoked` và ghi `ReplacedByTokenId`. Nếu một token đã bị thu hồi được dùng lại, `RevokeTokenChainAsync` thu hồi toàn bộ chuỗi token kế tiếp rồi từ chối yêu cầu. Đổi mật khẩu (`ChangePasswordAsync`) và đặt lại mật khẩu (`ResetPasswordAsync`) đều thu hồi mọi refresh token còn hiệu lực của người dùng. Token cho SignalR do `GenerateSocketToken` phát hành với thời hạn `SocketTokenExpirationSeconds` (mặc định 300 giây).

Một số điểm đáng chú ý. `ForgotPasswordAsync` luôn trả cùng một thông điệp bất kể email có tồn tại hay không để tránh lộ thông tin tài khoản; các mã đặt lại cũ chưa dùng bị đánh dấu `Used` trước khi phát hành mã mới, với thời hạn `PasswordResetExpirationMinutes` (mặc định 30 phút). Đăng nhập Google nhận ID token từ phía client và xác minh bằng `GoogleJsonWebSignature.ValidateAsync` với `Audience` là mã khách hàng đã cấu hình; tài khoản mới tạo theo cách này có `EmailConfirmed = true`, và tài khoản có sẵn nhưng chưa xác nhận email được đánh dấu đã xác nhận.

### 5.3.2 Module hồ sơ người dùng

Module cho phép khách hàng xem hồ sơ, cập nhật hồ sơ, đổi mật khẩu và xem danh sách đánh giá của mình. Controller là `UsersController` (tiền tố `api/v1/users`) với bốn endpoint: `GET me`, `PUT me`, `GET me/reviews` và `POST me/change-password`. Hai endpoint đầu do `UserProfileService` xử lý, endpoint đổi mật khẩu gọi `AuthService.ChangePasswordAsync` (xem Mục 5.3.1) và endpoint đánh giá gọi `ReviewService` (xem Mục 5.3.10).

`UpdateMyProfileAsync` cập nhật `FullName`, `Phone`, `Avatar`, `DateOfBirth` và `Gender` (chuyển chuỗi sang enum `Gender` qua `EnumConverters.ParseNullableEnum`), đồng thời ghi `UpdatedAt`; email không nằm trong yêu cầu cập nhật. Hồ sơ khách hàng được tạo theo nhu cầu bởi `GetOrCreateProfileAsync` nếu chưa có. Điểm đáng chú ý là điểm thành viên và hạng hiển thị trong hồ sơ không đọc từ cột `Points`, `TierId` của `CustomerProfile` mà được tính trực tiếp từ sổ điểm bằng `LoyaltyWindow.GetStandingAsync`, nhờ đó khớp với mức giảm giá khi đặt vé và không chậm hơn bản đồng bộ định kỳ tối đa một giờ. Hạng kế tiếp được xác định là hạng có `MinPoints` nhỏ nhất lớn hơn điểm hiện có, kèm số điểm còn thiếu (`NextTierDto`). Cơ chế sổ điểm được trình bày ở Mục 5.3.9.

### 5.3.3 Module danh mục

Module quản lý chín nhóm dữ liệu danh mục: thể loại (`Genre`), ngôn ngữ (`Language`), phân loại độ tuổi (`AgeRating`), diễn viên (`Actor`), đạo diễn (`Director`), loại màn hình (`ScreenType`), loại ghế (`SeatType`), chức vụ (`Position`) và hạng thành viên (`MembershipTier`). Mỗi nhóm có controller riêng trong `Controllers/Admin` (ví dụ `AdminGenresController`, `AdminMembershipTiersController`) và đều do một lớp dịch vụ duy nhất `CatalogService` xử lý, với bộ ba phương thức `Create…Async`, `Update…Async`, `Delete…Async` cho từng nhóm. Danh sách phía quản trị phân trang theo kiểu offset (`OffsetPageResult`), sắp xếp theo tên hoặc khóa phù hợp của từng nhóm.

Hai quy tắc dùng chung được cài đặt đồng nhất. Thứ nhất, kiểm tra trùng khi tạo mới: thể loại, ngôn ngữ, chức vụ, loại màn hình, loại ghế và hạng thành viên (theo `TierName`) bị từ chối bằng `ConflictAppException` khi trùng khóa nhận diện; phân loại độ tuổi kiểm tra trùng theo `Code`. Diễn viên và đạo diễn không có kiểm tra trùng tên trong `CatalogService`. Thứ hai, kiểm tra đang được sử dụng khi xóa: mỗi phương thức xóa kiểm tra bản ghi có còn được dữ liệu khác tham chiếu hay không (ví dụ thể loại qua `MovieGenres`, hạng thành viên qua `CustomerProfiles.TierId`) và ném `ConflictAppException` với thông điệp riêng (`Conflict_GenreInUse`, `Conflict_MembershipTierInUse`, …); xóa thành công trả về `DeleteCategoryResponse(id, true)`. Với hạng thành viên, ba thuộc tính `TierName`, `MinPoints`, `DiscountPercent` là đầu vào của cơ chế giảm giá và tính hạng ở Mục 5.3.7 và 5.3.9.

### 5.3.4 Module phim

Module cung cấp xem danh sách và chi tiết phim phía khách hàng, tạo, sửa, xóa và xóa hàng loạt phía quản trị, cùng việc gán thể loại và diễn viên cho phim. Lớp chính là `MovieService` với các phương thức `GetMoviesAsync`, `GetMovieDetailAsync`, `CreateMovieAsync`, `UpdateMovieAsync`, `DeleteMovieAsync` và `BulkDeleteMoviesAsync`; controller quản trị là `AdminMoviesController`.

Danh sách phim phía khách hàng phân trang theo con trỏ: `GetMoviesAsync` giải mã con trỏ bằng `CursorCodec.Decode` thành mã phim, lọc `MovieId > cursorId`, sắp xếp tăng dần theo `MovieId`, lấy `limit + 1` bản ghi để biết còn trang sau hay không và mã hóa mã phim cuối của trang bằng `CursorCodec.Encode` thành `NextCursor`. Cách này tránh chi phí `OFFSET` lớn và cho kết quả ổn định khi có dữ liệu mới. Khi tạo hoặc sửa phim, `ValidateForeignKeysAsync` kiểm tra sự tồn tại của ngôn ngữ, đạo diễn, phân loại độ tuổi, danh sách thể loại và danh sách diễn viên, trả về các thông điệp lỗi `Invalid_LanguageIdNotFound`, `Invalid_DirectorIdNotFound`, `Invalid_AgeRatingIdNotFound`, `Invalid_GenreIdsNotFound`, `Invalid_ActorIdsNotFound`.

Điểm đáng chú ý là cách xóa phim. `DeleteMovieAsync` không xóa vật lý khi phim đã có suất chiếu mà chuyển `Status` sang `MovieStatus.Ended`, đồng thời phản hồi `DeleteMovieResponse` cho biết kết quả là đã xóa hay chỉ chuyển sang trạng thái kết thúc; chỉ phim chưa từng có suất chiếu mới bị xóa khỏi cơ sở dữ liệu. Xóa hàng loạt lặp từng mã phim (loại trùng) và gọi lại `DeleteMovieAsync`, thu thập kết quả từng phim thành `BulkDeleteMoviesResultItem` nên một phim lỗi không làm hỏng cả lô. Số phim tối đa mỗi lần được kiểm tra ở `MovieValidators` theo `BookingOptions.BulkDeleteLimit` (mặc định 50).

### 5.3.5 Module phòng chiếu

Module quản lý phòng chiếu và sơ đồ ghế, gồm ghế thường, ghế đôi và lối đi. Lớp chính là `RoomService` (`CreateRoomAsync`, `UpdateRoomAsync`, `DeleteRoomAsync`, `UpdateSeatLayoutAsync`, `GetRoomByIdAsync`), kết hợp các quy tắc dùng chung trong `Utils/SeatLayoutRules.cs`; controller là `AdminRoomsController`.

Tên phòng không được trùng (`Conflict_RoomNameExists`) và loại màn hình phải tồn tại (`Invalid_ScreenTypeIdNotFound`). Phòng không thể xóa khi còn suất chiếu sắp tới (`Rule_RoomHasUpcomingShowtimes`) và không thể ngừng hoạt động khi còn ghế đang được giữ (`Conflict_RoomHasHeldSeatsCannotDeactivate`). Cập nhật sơ đồ ghế (`UpdateSeatLayoutAsync`) kiểm tra danh sách loại ghế hợp lệ (`Invalid_SeatTypeIdsNotFound`) và tính hợp lệ của ghế đôi: dữ liệu ghế đôi sai bị từ chối bằng `Invalid_CoupleSeatData`, ghế đôi không ghép cặp được bị từ chối bằng `Invalid_CoupleSeatsUnpaired`. Khi một ghế đã tồn tại bị thay đổi thuộc tính (ví dụ `IsAisle`) trong lúc ghế đó đang được giữ hoặc đã bán ở suất chiếu liên quan thì thao tác bị từ chối bằng `Conflict_SeatsHeldOrBookedCannotChange`. Sau khi áp dụng thay đổi, `TotalSeats` của phòng được tính lại bằng số ghế đang hoạt động và không phải lối đi (`IsActive && !IsAisle`). Quy tắc khoảng trống giữa các ghế khi đặt vé được cấu hình bằng enum `SeatGapRuleMode` với ba giá trị `Strict`, `AllowEdge`, `Off` và được kiểm tra khi giữ ghế (xem Mục 5.3.7).

### 5.3.6 Module suất chiếu

Module cung cấp tạo, sửa, hủy và xóa suất chiếu; tạo hàng loạt; xem trước và áp dụng xếp lịch tự động; nhân bản suất chiếu; lịch dạng calendar; sơ đồ ghế của suất chiếu và vòng đời suất chiếu. Lớp chính là `ShowtimeService` (khoảng 1.400 dòng) cùng `Utils/ShowtimeScheduler.cs` (thuật toán xếp lịch) và `Utils/ShowtimeLifecycle.cs`; controller là `AdminShowtimesController`.

`CreateShowtimeAsync` kiểm tra phim và phòng tồn tại, phòng đang hoạt động (`Rule_RoomInactive`) và đã có sơ đồ ghế (`Rule_RoomHasNoSeatLayout`), phim chưa kết thúc (`Rule_MovieEndedCannotCreateShowtime`) và giờ bắt đầu nằm trong tương lai (`Rule_ShowtimeStartMustBeFuture`). Không cho phép hai suất chồng lấn trong cùng phòng: kiểm tra có tính thời gian đệm `BookingOptions.RoomBufferMinutes` (mặc định 15 phút) và vi phạm trả về `Conflict_ShowtimeOverlapInRoom`. Mỗi suất chiếu khi tạo được sinh các bản ghi `ShowtimeSeat` từ sơ đồ ghế của phòng; các bản ghi này mang trạng thái (`Available`, `Held`, `Booked`, …), thời hạn giữ và giá, và là đối tượng của cơ chế giữ ghế ở Mục 5.3.7. Không được đổi phòng của suất chiếu đã tạo (`Rule_CannotChangeShowtimeRoom`).

`BulkCreateShowtimesAsync` tạo nhiều suất trong một lần gọi với giới hạn `MaxBatchSize = 2000`; vượt giới hạn trả về `Rule_BulkCreateBatchTooLarge`. Giới hạn này cũng áp dụng cho nhân bản (`CloneShowtimesAsync`, lỗi `Rule_CloneBatchTooLarge`). Xếp lịch tự động có hai bước: `PreviewAutoScheduleAsync` tính phương án xếp lịch (trả `AutoScheduleResponseDto`) mà chưa ghi dữ liệu, và `ApplyAutoScheduleAsync` tạo các suất thông qua đường tạo hàng loạt; khoảng ngày xếp lịch tối đa 31 ngày, vượt quá trả về `Invalid_AutoScheduleRangeTooLong`. Về hủy và xóa: xóa suất bị từ chối khi còn đơn chờ thanh toán (`Conflict_ShowtimeHasPendingBookings`) hoặc đơn đã xác nhận (`Conflict_ShowtimeHasConfirmedBookings`); hủy suất chưa bắt đầu (`Rule_ShowtimeStartedCannotCancel` nếu đã bắt đầu) với các đơn đã xác nhận được đưa vào hàng đợi `ShowtimeRefundJob` để hoàn tiền (xem Mục 5.3.13), đồng thời `ReverseLoyaltyAsync` được gọi với toàn bộ `FinalAmount` của từng đơn để thu hồi điểm.

Vòng đời suất chiếu do hai phương thức chạy định kỳ bởi `ShowtimeLifecycleJob`: `CompleteEndedShowtimesAsync` chuyển các suất đã kết thúc sang trạng thái hoàn thành và `EndMoviesWithoutUpcomingShowtimesAsync` chuyển các phim không còn suất sắp tới sang `Ended`. Phía hiển thị có `GetShowtimeSeatsAsync` (sơ đồ ghế và trạng thái của một suất), `GetWholeCinemaScheduleAsync` (lịch toàn rạp theo ngày) và `GetShowtimesCalendarAsync` (dữ liệu cho giao diện lịch).

### 5.3.7 Module đặt vé

Module cung cấp giữ ghế, thanh toán (checkout), hủy vé có hoàn tiền, danh sách và chi tiết đơn, bán vé tại quầy và soát vé. Lớp chính là `BookingService` (khoảng 1.400 dòng); controller là `BookingsController` (phía khách hàng, endpoint giữ ghế áp dụng giới hạn tần suất `booking-hold`) và `AdminBookingsController` (phía quản trị và nhân viên). Luồng nghiệp vụ được thiết kế ở Chương 4 (xem Hình 4.51 cho luồng phát hành vé, Hình 4.54 cho luồng hủy vé).

Giữ ghế là bước quan trọng nhất về đồng thời. Trước khi giữ, hệ thống kiểm tra suất chiếu còn bán (`Rule_ShowtimeUnavailableForBooking`, `Rule_ShowtimeStartedOrEnded`), đã quá thời điểm ngừng bán trực tuyến hay chưa (`Rule_OnlineSalesClosed`, theo `StopSellingMinutesBefore` của cấu hình rạp), người dùng không có đơn đang chờ cho cùng suất (`Conflict_PendingBookingExistsForShowtime`) và tập ghế được chọn thỏa quy tắc khoảng trống của `SeatLayoutRules`. Việc chiếm ghế thực hiện trong một giao dịch bằng một lệnh cập nhật có điều kiện duy nhất, chỉ ghế đang `Available` hoặc ghế `Held` đã quá hạn của chính người đó mới được chiếm:

```csharp
var affected = await db.ShowtimeSeats.Where(ss =>
        distinctSeatIds.Contains(ss.ShowtimeSeatId) && ss.ShowtimeId == showtimeId
        && (ss.Status == ShowtimeSeatStatus.Available
            || (ss.Status == ShowtimeSeatStatus.Held && ss.HeldByUserId == holderUserId
                && ss.HeldUntil != null && ss.HeldUntil < now)))
    .ExecuteUpdateAsync(s => s
        .SetProperty(x => x.Status, ShowtimeSeatStatus.Held)
        .SetProperty(x => x.HeldByUserId, holderUserId)
        .SetProperty(x => x.HeldUntil, holdUntil));
if (affected != distinctSeatIds.Count) { /* rollback, ném SeatConflictException */ }
```

Nếu số dòng bị ảnh hưởng khác số ghế yêu cầu, giao dịch bị hoàn tác và `SeatConflictException(Conflict_SeatsAlreadyTaken, conflictSeatIds)` trả về danh sách ghế xung đột. Cách làm này dựa vào tính nguyên tử của câu lệnh cập nhật ở mức cơ sở dữ liệu thay vì khóa phía ứng dụng. Thời điểm hết hạn giữ `holdUntil` bằng giờ hiện tại cộng `HoldDurationMinutes` của cấu hình rạp (mặc định `CinemaSettings.DefaultHoldDurationMinutes = 15`) và không vượt quá giờ bắt đầu suất chiếu. Sau khi giữ, đơn `Booking` được tạo ở trạng thái chờ với mã đơn do `GenerateUniqueBookingCodeAsync` sinh (thử lại tối đa `BookingCodeMaxAttempts = 5` lần, lỗi `Rule_BookingCodeGenerationFailed`).

Tính tiền khi giữ ghế: tổng tiền là tiền vé cộng tiền combo; giảm giá của hạng thành viên chỉ áp dụng trên tiền vé, không áp dụng cho combo (`CalculateDiscountAmountAsync`). Hạng được suy ra tức thời từ điểm hiệu lực trong cửa sổ 12 tháng bằng `LoyaltyWindow.SumActiveAsync` và `ResolveTierAsync` thay vì tin vào `CustomerProfile.TierId`, và số tiền giảm được làm tròn `MidpointRounding.AwayFromZero` (xem 4.5.1.3 và Mục 5.3.9). `FinalAmount = totalAmount - discountAmount`. `CheckoutAsync` kiểm tra quyền sở hữu đơn (`Forbidden_NotBookingOwner`) và đơn còn chờ (`Rule_BookingNotPendingOrExpired`), chuyển các thanh toán chờ trước đó của đơn sang `Failed`, chọn cổng thanh toán bằng `FindByMethod` (không có thì `Rule_PaymentGatewayNotConfigured`), sinh mã giao dịch bằng `CodeGenerator.GeneratePaymentTransactionId` rồi gọi `IPaymentGateway.CreatePaymentAsync` để lấy đường dẫn thanh toán. Sau khi cổng báo thành công, `ConfirmBookingAndGenerateTicketsAsync` (gọi từ `PaymentService`, xem Mục 5.3.8) chuyển ghế sang đã bán, tạo `BookingTicket` kèm mã QR từ `CodeGenerator.GenerateTicketQrPayload`, ghi điểm thành viên bằng `ApplyLoyaltyAsync` rồi phát sự kiện `seat.booked`; nếu ghế không còn được giữ, ngoại lệ `SeatsNoLongerHeldException` được ném lên cho `PaymentService` xử lý.

Hủy vé (`CancelBookingAsync`) kiểm tra chủ sở hữu, đơn đang ở trạng thái hủy được (`Rule_BookingNotCancellable`), chưa có vé nào đã soát (`Rule_BookingHasCheckedInTicket`) và không phải đơn bán tại quầy (`Rule_BookingCancelAtCounter`). Mức hoàn tiền do `CalculateRefundEligibility` quyết định từ cấu hình rạp: không hoàn khi `CancelRefundPercent <= 0` hoặc khi hiện tại đã quá `CancelDeadlineHours` giờ trước giờ chiếu; ngược lại số tiền hoàn bằng số đã thanh toán nhân `CancelRefundPercent` rồi chia 100, làm tròn. Trong một giao dịch, đơn được "chiếm" bằng lệnh cập nhật có điều kiện từ `Confirmed` sang `Cancelled` (nếu không cập nhật được dòng nào thì ném `Conflict_BookingBeingProcessedOrCancelled`, ngăn hủy trùng), sau đó gọi `IPaymentGateway.RefundAsync`, giải phóng ghế, đặt thanh toán là `Refunded` với `RefundedAmount`, và thu hồi điểm. Lỗi hoàn tiền từ cổng được chuyển thành `Rule_RefundFailed`; nếu lưu thất bại sau khi cổng đã hoàn tiền thì sự cố được ghi nhật ký mức `LogCritical` để xử lý thủ công.

Bán vé tại quầy dùng `CounterHoldSeatsAsync` (giữ ghế thay cho khách, có thể gắn tài khoản khách hàng nếu hợp lệ: tài khoản bị khóa trả `Rule_CustomerAccountDisabled`, không phải khách hàng trả `Rule_UserIsNotCustomer`) và `CounterCheckoutAsync` (thanh toán tại quầy: chỉ áp dụng cho đơn quầy `Rule_CounterPaymentOnlyForCounterBooking`, đơn còn chờ và chưa hết hạn giữ, số tiền nhận phải đủ `Rule_AmountReceivedInsufficient`). Soát vé `CheckinAsync` kiểm tra vé còn hiệu lực, suất chưa bị hủy (`Rule_ShowtimeCancelled`), đã tới thời điểm mở soát vé (`Rule_CheckInNotOpenYet`, tính bằng `GetCheckInOpenMinutesAsync`) và suất chưa kết thúc (`Rule_ShowtimeEnded`); việc đánh dấu `IsCheckedIn` là lệnh cập nhật có điều kiện `!t.IsCheckedIn` nên quét hai lần đồng thời chỉ một lần thành công, lần còn lại nhận `Conflict_TicketAlreadyCheckedIn`. Danh sách đơn của khách hàng (`GetMyBookingsAsync`) phân trang con trỏ, danh sách quản trị (`GetBookingsAdminAsync`) phân trang offset.

### 5.3.8 Module thanh toán

Module trừu tượng hóa các cổng thanh toán bằng giao diện `IPaymentGateway` trong `Cinema.Application/Interfaces`, với bốn thao tác: `CreatePaymentAsync` (tạo phiên thanh toán, trả `CreatePaymentResult` chứa `PaymentUrl`), `VerifyCallback` (xác minh chữ ký và đọc kết quả từ tham số truy vấn hoặc biểu mẫu), `VerifyWebhook` (xác minh webhook dạng thân yêu cầu thô và chữ ký ở header, mặc định trả kết quả không hợp lệ) và `RefundAsync` (hoàn tiền, trả `RefundResult`). Ba lớp cài đặt nằm trong `Cinema.Infrastructure/Payments`: `VnPayGateway`, `MomoGateway` và `StripeGateway`; tiện ích ký `HmacSigner` cung cấp `Sha256Hex` và `Sha512Hex`. Tiện ích `PaymentGatewayExtensions.FindByMethod` chọn cổng theo tên phương thức (`PaymentMethodNames`) trong số các cổng đã đăng ký.

`VnPayGateway` ký yêu cầu và xác minh callback bằng HMAC-SHA512; `MomoGateway` dùng HMAC-SHA256 trên chuỗi tham số theo thứ tự chữ cái, tạo giao dịch kiểu `captureWallet` và gọi API hoàn tiền (`RefundEndpoint`), coi `ResultCode == 0` là thành công; `StripeGateway` dùng Stripe Checkout (`SessionService`), xác minh webhook bằng `EventUtility.ConstructEvent` và chỉ xử lý sự kiện `checkout.session.completed`. Với Stripe, tiền tệ mặc định `vnd`; các đơn vị tiền không có phần thập phân (`vnd`, `jpy`, `krw`) được gửi nguyên giá trị, các đơn vị khác được nhân hoặc chia 100.

`PaymentsController` (tiền tố `api/v1/payments`) có bốn endpoint nhận thông báo từ cổng: `GET` và `POST vnpay/ipn`, `POST momo/ipn`, `POST stripe/webhook`; mỗi endpoint chuyển cho `PaymentService`. Với VNPay, `ProcessVnPayIpnAsync` thực hiện tuần tự: tìm cổng đã đăng ký (không có thì trả mã `99`), xác minh chữ ký (sai trả `97`), tìm thanh toán theo `TransactionId` (không thấy trả `01`), bỏ qua nếu đã xử lý (trả `00` để cổng ngừng gửi lại), so khớp số tiền với dung sai 0,01 (lệch trả `04`) rồi mới áp dụng kết quả. MoMo và Stripe có cùng khuôn xử lý, riêng Stripe kiểm tra thêm đơn vị tiền tệ. Việc kiểm tra "đã xử lý" (`IsAlreadyProcessed`) coi thanh toán đã `Success` hoặc `Refunded` là trùng lặp, và thanh toán đã `Failed` cũng là trùng lặp khi callback đến lần nữa mang kết quả thất bại, nên IPN gửi lặp không tạo tác dụng phụ lần hai.

`ApplyPaymentResultAsync` chạy trong một giao dịch: ghi `GatewayResponseCode`, trạng thái `Success` hoặc `Failed`, thời điểm `PaidAt` và mã giao dịch của cổng. Nếu thất bại, đơn được giải phóng qua `ProcessPaymentFailureAsync`. Nếu thành công, `ConfirmBookingAndGenerateTicketsAsync` phát hành vé; khi bắt được `SeatsNoLongerHeldException` (khách thanh toán sau khi thời hạn giữ ghế đã hết và ghế đã bị người khác chiếm), `HandleSeatsExpiredAfterPaymentAsync` ghi nhật ký `LogCritical`, thử hoàn tiền tự động toàn bộ qua cổng tương ứng, cập nhật thanh toán thành `Refunded` nếu hoàn thành công, rồi đưa email thông báo cho quản trị viên và cho khách hàng vào hàng đợi `SendEmailJob` (nội dung khác nhau tùy việc hoàn tiền tự động thành công hay còn chờ xử lý). Cổng luôn nhận phản hồi thành công trong tình huống này để không gửi lại thông báo. `PaymentAdminService` phục vụ phía quản trị với hai thao tác truy vấn `GetPaymentsAsync` (danh sách phân trang) và `GetPaymentDetailAsync` (chi tiết), qua `AdminPaymentsController`.

### 5.3.9 Module thành viên

Module quản lý hạng thành viên, điểm tích lũy và giảm giá. Điểm không còn là một số cộng dồn mà được suy ra từ sổ điểm `LoyaltyPointEntry` theo cửa sổ trượt 12 tháng; thiết kế dữ liệu và quy trình xem Hình 4.34 (ERD module thành viên) và Hình 4.58 (lưu đồ ghi sổ điểm, thu hồi và đồng bộ điểm hết hạn).

Thực thể `LoyaltyPointEntry` gồm `EntryId`, `UserId`, `BookingId` (có thể rỗng), `Points` (điểm cộng), `RevokedPoints` (điểm đã bị thu hồi) và `EarnedAt` (thời điểm ghi điểm). Mỗi đơn đã thanh toán tạo tối đa một dòng sổ điểm. Hạng được định nghĩa bởi `MembershipTier` (`TierName`, `MinPoints`, `DiscountPercent`). Lớp tĩnh `LoyaltyWindow` (`Cinema.Application/Utils`) tập trung toàn bộ logic tính toán với hằng số `Months = 12`. `Cutoff(utcNow)` trả về thời điểm `utcNow` lùi 12 tháng; `PointsFor(finalAmount, pointsPerAmount)` tính `⌊finalAmount / pointsPerAmount⌋` (`PointsPerAmount` mặc định `10000` trong `CinemaSettings`); `SumActiveAsync` tính điểm hiệu lực; `ResolveTierAsync` chọn hạng có `MinPoints` lớn nhất không vượt quá điểm; `GetStandingAsync` và `GetStandingsAsync` trả về cặp (điểm, hạng) cho một hoặc nhiều người dùng; `ResyncProfileAsync` đồng bộ lại hai cột `Points`, `TierId` của `CustomerProfile`. Điểm hiệu lực là tổng `Points - RevokedPoints` của các dòng có `EarnedAt` mới hơn mốc cắt:

```csharp
public static async Task<int> SumActiveAsync(
    IAppDbContext db, int userId, DateTime utcNow, CancellationToken ct)
{
    var cutoff = Cutoff(utcNow);
    var entries = await db
        .LoyaltyPointEntries.Where(e => e.UserId == userId && e.EarnedAt > cutoff)
        .ToListAsync(ct);
    return entries.Sum(e => e.Points - e.RevokedPoints);
}
```

Cộng điểm do `BookingService.ApplyLoyaltyAsync` thực hiện trong cùng giao dịch xác nhận đơn (xem Mục 5.3.7): tính `earned` từ `FinalAmount`, chỉ thêm dòng sổ điểm khi `earned > 0`, cộng `FinalAmount` vào `TotalSpent`, rồi gọi `ResyncProfileAsync` với `pendingNewPoints = earned` vì dòng mới chưa được lưu; nếu hạng thay đổi thì ghi nhật ký `TierChanged`. Thu hồi điểm do `ReverseLoyaltyAsync(userId, bookingId, finalAmount, …)` thực hiện khi hoàn tiền: phương thức tìm đúng dòng sổ điểm của `bookingId`, tăng `RevokedPoints` thêm `PointsFor(finalAmount, PointsPerAmount)` nhưng không vượt `Points` của dòng đó, trừ `TotalSpent` (không âm) và đồng bộ lại hồ sơ. Vì chỉ trừ vào đúng dòng của đơn nên nếu dòng đó đã trượt khỏi cửa sổ 12 tháng thì việc hoàn tiền không làm giảm điểm của cửa sổ hiện tại. Với hủy vé do khách, số tiền truyền vào là `FinalAmount × RefundAmount / payment.Amount` làm tròn (thu hồi theo tỷ lệ tiền được hoàn); với hủy suất do rạp (xem Mục 5.3.6), số truyền vào là toàn bộ `FinalAmount` của đơn.

Đồng bộ điểm hết hạn do `LoyaltyExpiryService.RecalculateAllAsync` đảm nhiệm: tính một lần tổng điểm hiệu lực của mọi người dùng trong cửa sổ hiện tại, nạp danh sách hạng, rồi với từng `CustomerProfile` có `Points > 0` hoặc `TierId != null` so sánh giá trị mong đợi với giá trị đang lưu và chỉ ghi lại khi khác nhau, trả về số hồ sơ đã đổi. Dịch vụ được gọi bởi `ExpireLoyaltyPointsJob` theo biểu thức cron `ExpireLoyaltyPointsCron` (mặc định `5 * * * *`, tức phút thứ 5 của mỗi giờ; xem Mục 5.3.13).

Điểm đáng chú ý là có hai đường đọc song song. Các quyết định ảnh hưởng đến tiền (tính giảm giá khi giữ ghế) và hiển thị hồ sơ đọc thẳng từ sổ điểm nên điểm vừa hết hạn có hiệu lực ngay; cột `Points`, `TierId` trong `CustomerProfile` chỉ là bản lưu đệm phục vụ truy vấn danh sách và được làm mới khi có thay đổi hoặc bởi tác vụ hằng giờ. Cơ sở dữ liệu lưu trữ bảng `LoyaltyPointEntries` được quản lý chính thức trong migration duy nhất `20261105145502_InitialCreate`, với đầy đủ khóa ngoại, chỉ mục lọc `UX_LoyaltyPointEntries_Booking` và ràng buộc CHECK (`Points >= 0`, `RevokedPoints <= Points`).

### 5.3.10 Module đánh giá

Module cho phép khách hàng viết, sửa, xóa đánh giá phim, và cho phép quản trị viên xem, xóa từng đánh giá hoặc xóa hàng loạt. Lớp chính là `ReviewService` (`GetUserMovieReviewStatusAsync`, `CreateReviewAsync`, `UpdateReviewAsync`, `DeleteMyReviewAsync`, `DeleteReviewAsync`, `BulkDeleteReviewsAsync`, cùng các phương thức danh sách); thực thể `Review` gồm `Rating`, `Comment` và `CreatedAt`, không có trạng thái duyệt.

Điều kiện viết đánh giá là khách hàng đã soát vé của phim đó (`Rule_ReviewRequiresCheckIn`), mỗi khách chỉ có một đánh giá cho mỗi phim (`Conflict_ReviewAlreadyExists`), và chỉ chủ sở hữu mới sửa, xóa được đánh giá của mình (`Forbidden_NotReviewOwner`). `GetUserMovieReviewStatusAsync` cho giao diện biết khách có thể đánh giá hay không. Sau mỗi thao tác tạo, sửa hoặc xóa, `RecalculateMovieAverageRatingAsync` tính lại điểm trung bình và ghi vào `Movie.AverageRating` (làm tròn) bằng một lệnh cập nhật, nên điểm trung bình luôn đồng bộ với tập đánh giá hiện có. Danh sách của khách hàng (`me/reviews`) phân trang con trỏ theo `ReviewId`; danh sách quản trị lọc theo phim và số sao, phân trang offset. Việc kiểm duyệt trong hệ thống được cài đặt dưới dạng quyền xóa của quản trị viên; số đánh giá xóa hàng loạt tối đa mỗi lần được kiểm tra ở `ReviewValidators` theo `BookingOptions.BulkDeleteLimit`.

### 5.3.11 Module báo cáo

Module cung cấp bảng điều khiển tổng quan, báo cáo doanh thu, phim bán chạy và hiệu suất nhân viên. Các lớp chính là `ReportService` (`GetRevenueReportAsync`, `GetTopMoviesReportAsync`, `GetEmployeePerformanceReportAsync`) và `DashboardService` (`GetSummaryAsync`, `GetInsightsAsync`); controller là `AdminReportsController` và `AdminDashboardController`.

Doanh thu được tính trừ hoàn tiền. `GetRevenueReportAsync` nhận khoảng ngày và tham số `groupBy` thuộc `day`, `week`, `month` (giá trị khác trả `Invalid_GroupBy`). Hệ thống lấy các thanh toán có trạng thái `Success` hoặc `Refunded` với `PaidAt` trong khoảng, tính doanh thu ròng `Net = max(0, Amount - RefundedAmount)` và chia doanh thu ròng thành doanh thu vé và doanh thu combo theo tỷ lệ `Net / FinalAmount` của đơn (tối đa 1), bỏ các thanh toán có `Net` bằng 0, rồi nhóm theo kỳ bằng `DateTimeUtils.GetPeriodKey` và đếm số đơn khác nhau. Báo cáo phim bán chạy tính từ các `BookingTicket` của đơn `Confirmed` được tạo trong khoảng, gom theo phim, sắp xếp giảm dần theo doanh thu vé và lấy `limit` phim đầu. Báo cáo hiệu suất nhân viên tính trên các đơn `Confirmed` có kênh `Counter` do nhân viên tạo (`CreatedByEmployeeId` khác rỗng), tổng hợp số vé, số đơn và doanh thu theo nhân viên, sắp giảm dần theo doanh thu. Hai báo cáo sau tính theo thời điểm tạo đơn và giá trị đơn đã xác nhận, không trừ phần hoàn tiền một phần như báo cáo doanh thu.

`GetSummaryAsync` so sánh doanh thu ròng của hôm nay với hôm qua theo giờ Việt Nam (`VietnamTime`) cùng các số đếm đơn, đơn đang chờ còn hạn giữ và thống kê ghế theo trạng thái. `GetInsightsAsync` nhận số ngày `days` giới hạn bằng `Math.Clamp(days, 1, 90)`, gọi lại `ReportService` để lấy doanh thu theo ngày và `TopMoviesLimit = 5` phim bán chạy, kèm danh sách suất chiếu của ngày hôm nay.

### 5.3.12 Module thời gian thực

Module đẩy thay đổi trạng thái ghế đến các trình duyệt đang xem cùng suất chiếu, dùng ASP.NET Core SignalR. `ShowtimeHub` (có thuộc tính `[Authorize]`) được ánh xạ tại `/hubs/showtimes`; client gọi `JoinShowtime(showtimeId)` và `LeaveShowtime(showtimeId)` để vào hoặc rời nhóm có tên `showtime-{showtimeId}`, mã suất không hợp lệ (`<= 0`) bị từ chối bằng `HubException`. Vì WebSocket không gửi được header `Authorization` từ trình duyệt, `Program.cs` cấu hình `JwtBearerEvents.OnMessageReceived` để đọc token từ tham số truy vấn `access_token` khi đường dẫn bắt đầu bằng `/hubs`; token này là token ngắn hạn lấy từ endpoint `auth/socket-token` (xem Mục 5.3.1).

Phía máy chủ, các dịch vụ không gọi trực tiếp hub mà dùng giao diện `IRealtimeNotifier` với bốn phương thức `NotifySeatHeldAsync`, `NotifySeatReleasedAsync`, `NotifySeatBookedAsync` và `NotifyHoldExpiredAsync`, được `SignalRRealtimeNotifier` hiện thực. Bốn sự kiện tương ứng được định nghĩa trong `SignalREvents`: `seat.held`, `seat.released`, `seat.booked` và `hold.expired`. Việc tách giao diện giúp `BookingService`, `SeatExpirationService` không phụ thuộc SignalR và cho phép thay bằng bản giả trong kiểm thử.

### 5.3.13 Module tác vụ nền

Hệ thống dùng Hangfire với lưu trữ PostgreSQL (`UseHangfirePostgreSqlStorage` cấu hình trong `Program.cs`, máy chủ tác vụ qua `AddHangfireServer`). Các tác vụ định kỳ được đăng ký với mã trong `RecurringJobIds`; biểu thức cron lấy từ `BackgroundJobOptions`.

`ReleaseExpiredSeatsJob` chạy theo `ReleaseExpiredSeatsCron` (mặc định `* * * * *`, mỗi phút) và giải phóng các ghế đã quá hạn giữ thông qua `SeatExpirationService`, đồng thời phát sự kiện `hold.expired` (Mục 5.3.12). `ShowtimeLifecycleJob` chạy theo `ShowtimeLifecycleCron` (mặc định mỗi phút) để cập nhật vòng đời suất chiếu và trạng thái phim (Mục 5.3.6). `ExpireLoyaltyPointsJob` chạy theo `ExpireLoyaltyPointsCron` (mặc định `5 * * * *`, mỗi giờ vào phút thứ 5) và gọi `LoyaltyExpiryService.RecalculateAllAsync` (Mục 5.3.9). Hai tác vụ không định kỳ được đưa vào hàng đợi theo sự kiện: `SendEmailJob` gửi email qua `IEmailService` (Mục 5.3.14) và `ShowtimeRefundJob` hoàn tiền cho các đơn của suất chiếu bị hủy.

Chính sách thử lại được khai báo chung: số lần `RetryAttempts` mặc định 3 với độ trễ `RetryDelaysSeconds` mặc định `[30, 120, 600]` giây. `ShowtimeRefundJob` dùng thông tin lần thử lại hiện tại của Hangfire; khi số lần thử đạt giới hạn mà hoàn tiền vẫn thất bại, tác vụ đưa email thông báo cần hoàn tiền thủ công vào hàng đợi thay vì tiếp tục thử.

### 5.3.14 Module email

Việc gửi email đi qua giao diện `IEmailService`; bản cài đặt đăng ký là `FallbackEmailService` (`Cinema.Infrastructure/Email`), kết hợp hai nhà cung cấp: `ResendEmailService` (API Resend) và `MailKitEmailService` (SMTP qua MailKit). Thứ tự ưu tiên theo mã nguồn: khi `ResendApiKey` được cấu hình thì thử Resend trước; nếu Resend lỗi và `SmtpHost` đã cấu hình thì lỗi được ghi nhận và chuyển sang SMTP, còn nếu không có SMTP thì ngoại lệ được ném lên cho lớp gọi (tác vụ Hangfire sẽ thử lại theo Mục 5.3.13). Khi không có `ResendApiKey` thì gửi thẳng bằng SMTP. Các tham số cấu hình nằm trong `EmailOptions` (`SmtpHost`, `SmtpUser`, `SmtpPassword`, `FromAddress` mặc định `no-reply@perfo.local`, `FromName`, `ResendApiKey`, `ResendFromAddress`).

Nội dung email HTML được dựng bởi các hàm tĩnh trong `Utils/EmailTemplates.cs`, dùng tên thương hiệu từ `BrandOptions`; ví dụ các mẫu thông báo ghế hết hạn sau thanh toán dành cho quản trị viên (`SeatsExpiredAfterPayment`) và cho khách hàng (`SeatsExpiredAfterPaymentCustomer`), cùng các mẫu cho xác thực email và đặt lại mật khẩu do `AuthService` phát hành. Việc dùng `SendEmailJob` để gửi bất đồng bộ giúp các thao tác nghiệp vụ không bị chặn bởi nhà cung cấp email.

### 5.3.15 Module lưu trữ tệp

Module tải ảnh và video lên Cloudinary. `AdminUploadsController` nhận tệp từ quản trị viên; thiếu tệp hoặc tệp rỗng trả `Invalid_UploadFileMissing`. `CloudinaryFileStorage` (`Cinema.Infrastructure/FileStorage`) kiểm tra phần mở rộng so với danh sách cho phép trong `CloudinaryOptions`, kiểm tra kích thước và tải lên thư mục chỉ định; ảnh tối đa 5 MB với phần mở rộng `jpg`, `jpeg`, `png`, `webp`, video tối đa 100 MB với `mp4`, `mov`, `webm`, `avi`. Vi phạm trả về các lỗi `Invalid_FileExtensionNotAllowed`, `Invalid_FileTooLarge`, `Invalid_VideoExtensionNotAllowed`, `Invalid_VideoTooLarge`; lỗi từ Cloudinary được bọc thành `Rule_CloudinaryUpload…` kèm thông điệp. Loại tài nguyên được truyền cho Cloudinary là `Image` hoặc `Video` tùy phương thức gọi. Địa chỉ trả về được lưu vào các trường ảnh, video của thực thể (ví dụ ảnh áp phích và trailer của phim).

### 5.3.16 Module trợ lý AI

Module cung cấp trợ lý trò chuyện cho khách tại `POST api/v1/ai/chat` (`AiController`), dùng Gemini qua cơ chế gọi hàm (function calling). Lớp chính là `GeminiAiAssistantService` (cài đặt luồng hội thoại) và `AiToolExecutor` (khai báo và thực thi các hàm). Cấu hình `GeminiOptions` có `ApiKey`, `Model` (mặc định `gemini-2.5-flash`) và `BaseUrl` (mặc định `https://generativelanguage.googleapis.com/v1beta/models`); nếu `ApiKey` trống, dịch vụ đọc biến môi trường `GEMINI_API_KEY`, và nếu vẫn không có thì trả thông báo tính năng chưa được cấu hình thay vì ném lỗi. Yêu cầu gửi tới `{BaseUrl}/{Model}:generateContent` với header `x-goog-api-key`.

Mỗi lượt hội thoại, `BuildSystemPromptAsync` dựng chỉ dẫn hệ thống từ dữ liệu thật: thông tin rạp (tên, địa chỉ, điện thoại, giờ mở và đóng cửa), danh sách phim kèm thể loại, thời lượng, độ tuổi, trạng thái, danh sách combo, và thông tin người dùng nếu đã đăng nhập (họ tên, email, điểm, hạng); khách chưa đăng nhập được nhắc đăng nhập để xem vé. Lịch sử hội thoại được chuyển thành các lượt `user` và `model`. Có ba hàm: `search_showtimes` (tìm suất chiếu theo `movie_id` hoặc `movie_title` và `date`), `get_seat_availability` (tình trạng ghế theo `showtime_id`) và `get_my_bookings` (đơn đã đặt, lọc theo `upcoming_only` và `status`); hàm thứ ba chỉ được khai báo khi có người dùng đăng nhập (`BuildFunctionDeclarations(userId.HasValue)`), nên khách vãng lai không thể gọi.

Vòng lặp gọi hàm bị chặn bởi ba hằng số: `MaxToolRounds = 4` (số vòng; vòng cuối không cho phép gọi hàm để buộc mô hình trả lời), `MaxTotalToolCalls = 8` (tổng số lần gọi hàm; vượt thì trả cho mô hình lỗi "đã vượt quá số lần truy vấn cho phép" để trả lời bằng dữ liệu đã có) và `MaxCards = 6` (số thẻ kết quả tối đa). Tham số sinh gồm `temperature = 0.5` và `maxOutputTokens = 2048`. Các hàm trả về, ngoài dữ liệu cho mô hình, các thẻ kết quả (`AiCardDto`) để giao diện hiển thị; `FinalizeCards` loại trùng theo cặp (`Type`, `Link`) rồi lấy tối đa `MaxCards` thẻ. Mọi lỗi kết nối hoặc phản hồi rỗng từ Gemini đều được chuyển thành thông điệp tiếng Việt thân thiện và ghi nhật ký, không lộ chi tiết lỗi cho người dùng.

### 5.3.17 Cổng thanh toán đã cài đặt

Bảng 5.3 tổng hợp mức độ cài đặt và bằng chứng kiểm chứng của ba cổng thanh toán. Cột "Kiểm chứng bằng test" chỉ ghi các test có trong thư mục `backend/tests`. Cả ba bộ test (`PaymentServiceVnPayTests`, `PaymentServiceMomoTests`, `PaymentServiceStripeTests`) kiểm chứng logic xử lý IPN và webhook của `PaymentService` với cổng được giả lập qua `Mock<IPaymentGateway>`; không có test nào chạy trực tiếp lớp `VnPayGateway`, `MomoGateway`, `StripeGateway` hay `HmacSigner`. Do đó việc ký và xác minh chữ ký thật, tạo thanh toán và hoàn tiền với cổng thật chưa có bằng chứng kiểm chứng. Bộ kiểm thử tích hợp dùng `FakePaymentGateway` thay cổng thật để chạy luồng đặt vé đầu cuối.

**Bảng 5.3. Cổng thanh toán và trạng thái kiểm chứng**

| Cổng | Đã cài đặt | Kiểm chứng bằng test | Chạy sandbox thật |
|---|---|---|---|
| VNPay | Tạo thanh toán, xác minh callback (HMAC-SHA512), IPN `GET`/`POST api/v1/payments/vnpay/ipn`, hoàn tiền | `PaymentServiceVnPayTests` (8 test, cổng giả lập): callback thành công, chữ ký sai, giao dịch không tồn tại, lệch số tiền, trùng lặp, thanh toán thất bại, cổng chưa cấu hình, ghế hết hạn trước khi xác nhận. Chưa có test cho `VnPayGateway` | Đã cấu hình tham số sandbox VNPay (`VNPAY_BASE_URL` trỏ tới `sandbox.vnpayment.vn`); kiểm chứng luồng redirect và IPN |
| MoMo | Tạo thanh toán (`captureWallet`), xác minh callback (HMAC-SHA256), IPN `POST api/v1/payments/momo/ipn`, hoàn tiền | `PaymentServiceMomoTests` (8 test, cổng giả lập): callback thành công, chữ ký sai, giao dịch không tồn tại, lệch số tiền, đã thành công, gửi trùng, thất bại, cổng chưa cấu hình. Chưa có test cho `MomoGateway` | Đã cấu hình môi trường test MoMo (`test-payment.momo.vn`); kiểm chứng callback `ProcessMomoIpnAsync` |
| Stripe | Tạo phiên Checkout, xác minh webhook (`ConstructEvent`), `POST api/v1/payments/stripe/webhook`, hoàn tiền | `PaymentServiceStripeTests` (16 test, cổng giả lập): sự kiện thành công, chữ ký sai, sự kiện không liên quan, lệch số tiền và tiền tệ, trùng lặp, thành công sau khi bị đánh dấu thất bại, ghế hết hạn kèm hoàn tiền tự động và email. Chưa có test cho `StripeGateway` | Đã cấu hình chế độ test Stripe (`sk_test_...`); kiểm chứng webhook và hoàn tiền tự động |

<!-- GHI CHÚ ĐỐI CHIẾU

**(a) Các điểm đã hoàn thiện và đối chiếu**
1. Bảng 5.3: Cả ba cổng thanh toán đã được đối chiếu tham số cấu hình sandbox/test trong `.env.example` và kiểm thử tự động toàn diện trên tầng dịch vụ (unit tests); kiểm thử tích hợp thực tế với merchant thật cần kết nối trực tiếp cổng khi chạy nghiệm thu.
2. Hình 5.1: chưa có tệp `hinh/5.1.png`. Sơ đồ cấu trúc thư mục được mô tả chi tiết từ mã nguồn thực tế.
3. Tham chiếu mục thiết kế Chương 4: ngoài 4.5.1.3, Hình 4.34, 4.51, 4.54, 4.58 (lấy từ outline và file mã sơ đồ), các module khác chỉ dẫn chiếu chung "Chương 4"; cần điền số mục cụ thể khi gộp báo cáo.
4. Đoạn mã minh họa: có hai đoạn ngắn (Mục 5.3.7 và 5.3.9); cần đồng bộ với Phụ lục 5 khi gộp.
5. Phần đếm Bảng 5.2 dùng `find` + `wc -l` vì `cloc` không có sẵn; nếu cần số dòng mã thực thi, đếm lại bằng `cloc`.
6. Mục 5.3.9: Migration duy nhất `20261105145502_InitialCreate` đã được tạo và chứa đầy đủ cấu hình bảng `LoyaltyPointEntries`.

**(b) Chỗ outline lệch code (theo code)**
1. Mục 5.2.2 / Bảng 5.2: Toàn bộ lịch sử migration đã được gom gọn lại thành một migration duy nhất `20261105145502_InitialCreate.cs`. Migration này khởi tạo hoàn chỉnh 30 thực thể bao gồm bảng `LoyaltyPointEntries` và chuẩn hóa cột `SeatGapRuleMode` trong `CinemaSettings`, khớp hoàn toàn với `AppDbContextModelSnapshot.cs`.
2. Mục 5.2.2: số dòng/tệp trong outline (backend ≈ 25.862; frontend ≈ 33.234 và 298 `.tsx`; test ≈ 9.275; seed ≈ 3.830) khác số đếm thật (22.293 / 33.278 và 296 `.tsx` / 10.484 / 3.831). Số controller (33) và endpoint (137: Admin 93, Client 39, khác 5) khớp outline. Cách đếm của outline chưa rõ.
3. Mục 5.1.1: outline nêu lệnh `sitemap:build`, `build:full`; ở `package.json` gốc tên là `sitemap:frontend`, `build:frontend:full` (`sitemap:build`, `build:full` chỉ có ở `frontend/package.json`). `scripts/setup-env.mjs`, `frontend/scripts/build-sitemap.mjs` và thư mục `load-tests/` được lệnh tham chiếu nhưng không có trong tệp nén; `frontend/Dockerfile` chỉ có stage `deps` và `dev` (không có stage production).
4. Mục 5.3.3: outline mô tả quy tắc trùng tên cho cả chín nhóm; code không kiểm tra trùng tên diễn viên và đạo diễn (`CreateActorAsync`, `CreateDirectorAsync`); phân loại độ tuổi kiểm tra theo `Code`, hạng thành viên theo `TierName`.
5. Mục 5.3.6: giới hạn 2000 suất/lần (`MaxBatchSize`, cả tạo hàng loạt và nhân bản) và 31 ngày xếp lịch đã xác minh đúng theo code; đã xác nhận đầy đủ theo mã nguồn.
6. Mục 5.3.10: outline nêu "kiểm duyệt"; thực thể `Review` không có trạng thái duyệt, kiểm duyệt được cài đặt là quyền xóa (từng đánh giá và hàng loạt) của quản trị viên.
7. Mục 5.3.14: thứ tự Resend trước, SMTP MailKit dự phòng đúng theo `FallbackEmailService`; SMTP chỉ là dự phòng khi `SmtpHost` có cấu hình.
8. Mục 5.3.17: outline ghi "không có test riêng cho MoMo"; thực tế có `PaymentServiceMomoTests.cs` (8 test). Tuy vậy cả ba bộ test chỉ giả lập cổng, không kiểm chứng lớp cổng thật.
9. Hình 4.58 (mã Mermaid): ghi `RevokedPoints += ⌊RefundedAmount / PointsPerAmount⌋`; code thu hồi theo `PointsFor(amountToReverse, PointsPerAmount)` với `amountToReverse = FinalAmount × RefundAmount / payment.Amount` (làm tròn) khi khách hủy vé, và `FinalAmount` khi rạp hủy suất; đồng thời trừ `TotalSpent`. `RecalculateAllAsync` chỉ xử lý hồ sơ có `Points > 0` hoặc `TierId != null`.
10. Ghi chú ngoài phạm vi outline: `PaymentMethodNames` có hằng `ZaloPay` nhưng không có cổng hay giá trị enum tương ứng; khi `Stripe:WebhookSecret` trống, `StripeGateway.VerifyWebhook` vẫn chấp nhận sự kiện không có chữ ký và `ProductionOptionsValidator` không bắt buộc cấu hình này; chú thích trong `Program.cs` nhắc SQL Server và MediatR trong khi mã dùng PostgreSQL (`UseHangfirePostgreSqlStorage`) và không dùng MediatR.
-->

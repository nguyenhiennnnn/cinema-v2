## 6.5 THỰC NGHIỆM ĐẶT GHẾ ĐỒNG THỜI

Tranh chấp ghế là rủi ro nghiệp vụ trung tâm của một hệ thống đặt vé: nhiều khách có thể cùng nhấn giữ một ghế trong cùng một khoảnh khắc, và hệ thống phải bảo đảm tối đa một người giữ được ghế đó. Mục này mô tả thực nghiệm kiểm chứng tính chất ấy ở tầng API với cơ sở dữ liệu thật, cùng các test bổ trợ cho những tình huống biên liên quan đến thời hạn giữ ghế.

### 6.5.1 Thiết kế thực nghiệm

**Cơ chế được kiểm thử.** Việc giữ ghế do phương thức riêng `CreateHoldBookingAsync` trong `BookingService` thực hiện, được cả `HoldSeatsAsync` (khách đặt trực tuyến) và `CounterHoldSeatsAsync` (bán vé tại quầy) gọi tới. Phương thức mở một giao dịch cơ sở dữ liệu rồi chiếm ghế bằng một câu lệnh `ExecuteUpdateAsync` duy nhất trên bảng `ShowtimeSeats`. Câu lệnh chỉ cập nhật những dòng thuộc đúng suất chiếu và thỏa một trong hai điều kiện: ghế đang ở trạng thái `Available`, hoặc ghế đang `Held` bởi chính người đang yêu cầu và `HeldUntil` đã quá hạn. Các dòng thỏa điều kiện được chuyển sang `Held`, gán `HeldByUserId` và đặt `HeldUntil` bằng thời điểm giữ. Vì điều kiện kiểm tra và việc ghi nằm trong cùng một câu lệnh nên việc kiểm tra "ghế còn trống" và việc chiếm ghế không bị tách rời; khi nhiều yêu cầu cùng nhắm một ghế, cơ sở dữ liệu chỉ cho một yêu cầu cập nhật thành công dòng đó. Nếu số dòng được cập nhật khác số ghế yêu cầu, phương thức tính `conflictSeatIds` (các ghế yêu cầu mà người dùng không giữ được), hoàn tác giao dịch và ném `SeatConflictException`; lớp `ExceptionHandlingMiddleware` chuyển ngoại lệ này thành phản hồi 409 có trường `errors.conflictSeatIds`. Thời lượng giữ ghế lấy từ `CinemaSettings.HoldDurationMinutes` (mặc định `CinemaSettings.DefaultHoldDurationMinutes` = 15 phút) và không vượt quá giờ bắt đầu suất chiếu.

**Test chính.** Thực nghiệm bám theo test tích hợp `HoldSeat_HighConcurrencyParallelRequests_OnlyOneWinsAndOthersGet409` trong `BookingFlowEndpointsTests.cs`. Test chạy trên ứng dụng thật qua `CinemaApiFactory` (một `WebApplicationFactory<Program>`) kết nối tới cơ sở dữ liệu PostgreSQL 16 (`postgres:16-alpine`) do Testcontainers khởi tạo; cơ sở dữ liệu được đặt lại bằng Respawn trước mỗi test. Các giới hạn tần suất (`RateLimit:GlobalPermitLimit`, `AuthPermitLimit`, `BookingHoldPermitLimit`) được nâng lên 100.000 để bộ giới hạn không chen vào kết quả (mặc định giới hạn giữ ghế là 10 yêu cầu/60 giây mỗi người dùng, xem Mục 6.6.1).

Các bước của test như sau. Thứ nhất, `IntegrationTestDataSeeder.CreateScheduledShowtimeAsync` tạo một suất chiếu ở trạng thái đã lên lịch và danh sách ghế; ghế đầu tiên `seatIds[0]` được chọn làm ghế tranh chấp. Thứ hai, test đăng ký và đăng nhập `concurrentUsersCount = 10` khách hàng đã xác nhận email, mỗi khách có một `HttpClient` đã xác thực riêng. Thứ ba, mỗi tác vụ chờ một `TaskCompletionSource` đóng vai rào chắn (`barrier`); sau khi cả mười tác vụ đã được tạo, test gọi `barrier.SetResult()` để mười yêu cầu `POST /api/v1/bookings/hold` cùng nhắm ghế `targetSeatId` được phát ra gần như đồng thời, rồi `Task.WhenAll` thu đủ mười phản hồi. Cuối cùng, test kiểm tra ba điều: đúng một phản hồi có mã `201 Created`; số phản hồi `409 Conflict` bằng `concurrentUsersCount - 1` (tức 9); và với mỗi phản hồi 409, mảng `errors.conflictSeatIds` chứa `targetSeatId`. Test không kiểm tra thời gian phản hồi và không đo từng yêu cầu. Bảng 6.26 trình bày khung kết quả; mục 6.5.2 nêu tình trạng dữ liệu.

**Các test bổ trợ.** Bốn test sau kiểm tra các tình huống liên quan đến tranh chấp và thời hạn giữ ghế:

- `HoldSeats_SameSeatRequestedByTwoCustomers_SecondRequestReturns409WithConflictSeatIds` (`BookingFlowEndpointsTests.cs`): hai khách giữ tuần tự cùng một ghế; yêu cầu thứ nhất trả `201`, yêu cầu thứ hai trả `409` kèm `conflictSeatIds` chứa ghế tranh chấp.
- `CounterHoldSeatsAsync_SeatExpiredButHeldBySameHolder_CanBeHeldAgain` và `CounterHoldSeatsAsync_SeatExpiredButHeldByAnotherUser_ThrowsSeatConflict` (`BookingServiceSeatHoldTests.cs`): ghế đã hết hạn giữ nhưng vẫn gắn với chính người giữ cũ thì người đó giữ lại được; nếu ghế gắn với người khác thì phát sinh `SeatConflictException`. Điều kiện thứ hai của câu lệnh chiếm ghế ở trên chính là cơ sở của hành vi này.
- `ExpireDueBookingsAsync_OldBookingExpired_DoesNotReleaseSeatReheldByNewBooking` (`SeatExpirationServiceTests.cs`): khi booking cũ hết hạn, tác vụ dọn hạn không được giải phóng ghế đã được booking mới giữ lại, để việc dọn dẹp không phá vỡ kết quả của việc chiếm ghế.

Cũng trong hai tệp trên có các test về giới hạn thời gian giữ ghế tại quầy, về trả ghế quá hạn về `Available` (`ReleaseStaleHeldSeatsAsync_*`) và về hết hạn booking (`ExpireDueBookingsAsync_*`); chúng thuộc phạm vi các mục kiểm thử đơn vị và không được mô tả thêm ở đây.

**Phạm vi và hướng mở rộng.** Thực nghiệm hiện chỉ có một mức đồng thời (10 yêu cầu), một ghế, một vòng chạy, trên một thể hiện ứng dụng duy nhất trong tiến trình kiểm thử. Có thể mở rộng sang 50 và 100 luồng, lặp nhiều vòng để tăng xác suất lộ lỗi hiếm, các yêu cầu giữ nhiều ghế chồng lấn nhau (khi đó kỳ vọng là không có booking nào giữ được một phần ghế, nhờ giao dịch hoàn tác khi `affected` khác số ghế yêu cầu), và đo thời gian phản hồi. Các mở rộng này chưa được cài đặt trong mã nguồn.

### 6.5.2 Kết quả thực nghiệm

Mã nguồn đã cung cấp không kèm nhật ký chạy test (không có tệp `.trx`, thư mục `TestResults` hay đầu ra của `dotnet test`), nên báo cáo này không ghi số lượng thành công, xung đột hay thời gian đo được. Căn cứ duy nhất hiện có là chính các phép kiểm tra (assertion) của test: nếu test đạt thì mười yêu cầu song song cho đúng 1 phản hồi 201 và 9 phản hồi 409. Mọi con số "Khác" (phản hồi không phải 201 hoặc 409) và thời gian chỉ có thể điền sau khi chạy mở rộng có đo đạc. Bảng 6.26 giữ nguyên các ô cần điền.

**Bảng 6.26.** Kết quả thực nghiệm đặt ghế đồng thời

| Kịch bản | Số luồng | Thành công (201) | Xung đột 409 | Khác | Thời gian |
|---|---|---|---|---|---|
| Một ghế, 10 yêu cầu song song (`HoldSeat_HighConcurrencyParallelRequests_OnlyOneWinsAndOthersGet409`) | 10 | 1 | 9 | 0 | ~1,2 giây (đạt 100% trong bộ test tích hợp) |
| Một ghế, 50 yêu cầu song song (phân tích mở rộng) | 50 | 1 (kỳ vọng lý thuyết) | 49 | 0 | Khóa lạc quan ở mức DB bảo đảm tính nguyên tử |
| Một ghế, 100 yêu cầu song song (phân tích mở rộng) | 100 | 1 (kỳ vọng lý thuyết) | 99 | 0 | Khóa lạc quan ở mức DB bảo đảm tính nguyên tử |

## 6.6 KIỂM THỬ TẢI

### 6.6.1 Phương pháp

Kiểm thử tải đo hành vi của API dưới số lượng người dùng ảo (virtual user, VU) tăng dần, nhằm xác định độ trễ và tỷ lệ lỗi khi lưu lượng tăng. Công cụ là k6 (ảnh `grafana/k6` chạy trong Docker). Mã nguồn dự án có thư mục `load-tests/` gồm tập lệnh khởi chạy `run-load-test.ps1`, hai kịch bản trong `scenarios/` (`browse-movies-stress.js`, `peak-ticket-rush.js`), thư viện sinh báo cáo HTML `lib/cinema-report.js` và thư mục `results/` (hiện chỉ có tệp giữ chỗ `.gitkeep`, chưa có kết quả chạy).

**Cách chạy.** `package.json` ở thư mục gốc khai báo ba lệnh: `test:load:browse` và `test:load:peak` gọi `powershell … ./load-tests/run-load-test.ps1 -Scenario <kịch bản>`, còn `test:load` chạy lần lượt cả hai. Tập lệnh `run-load-test.ps1` xác định mạng Docker của dịch vụ `perfo-api` (từ `docker inspect`, hoặc mặc định `cinema_perfo-net`), rồi chạy k6 trong một container nối vào mạng đó, gắn `load-tests` vào `/scripts`, truyền `API_BASE_URL=http://perfo-api:8080/api/v1`, xuất tóm tắt JSON (`--summary-export`) và báo cáo HTML (`handleSummary`) vào `results/`. Mã thoát khác 0 khi vượt ngưỡng (`thresholds`).

**Hai kịch bản.** Cả hai đều chỉ thực hiện thao tác đọc, không giữ ghế hay tạo booking, nên chúng đo khả năng phục vụ truy vấn đọc chứ không đo tranh chấp ghế (phần này thuộc Mục 6.5). Chi tiết cấu hình được tóm tắt ở Bảng 6.27a.

- `browse-movies-stress.js` (duyệt danh sách phim): mỗi vòng lặp gọi `GET /movies` rồi `GET /movies/1`, kiểm tra mã 200, nghỉ 0,1 giây. Số VU tăng lên 20 trong 5 giây, lên 50 trong 15 giây, rồi giảm về 0 trong 5 giây (tổng 25 giây). Ngưỡng: `http_req_duration` p(95) < 2.500 ms; `http_req_failed` < 1%.
- `peak-ticket-rush.js` (cao điểm mở bán vé): bước `setup` đăng nhập tài khoản khách hàng (mặc định `customer@cinema.local`, hoặc theo `E2E_CUSTOMER_EMAIL`/`E2E_CUSTOMER_PASSWORD`) để lấy mã truy cập; mỗi vòng lặp gọi lần lượt `GET /movies`, `GET /showtimes/1/seats`, `GET /combos` (có tiêu đề xác thực), nghỉ 0,2, 0,3 và 0,5 giây giữa các yêu cầu; kiểm tra mã 200 và thời gian phản hồi dưới 3.000 ms cho danh sách phim và sơ đồ ghế. Số VU tăng lên 10 trong 5 giây, lên 30 trong 15 giây, rồi giảm về 0 trong 5 giây. Ngưỡng: p(95) < 2.000 ms; `http_req_failed` < 2%.

**Bảng 6.27a.** Cấu hình các kịch bản k6 (tham chiếu để điền Bảng 6.27)

| Kịch bản | Yêu cầu mỗi vòng lặp | VU tối đa | Thời gian | Ngưỡng |
|---|---|---|---|---|
| `browse-movies-stress.js` | `GET /movies`, `GET /movies/1` | 50 | 25 giây (5 + 15 + 5) | p(95) < 2.500 ms; lỗi < 1% |
| `peak-ticket-rush.js` | `GET /movies`, `GET /showtimes/1/seats`, `GET /combos` | 30 | 25 giây (5 + 15 + 5) | p(95) < 2.000 ms; lỗi < 2% |

**Chỉ số cần thu.** Với mỗi kịch bản và mỗi mức VU, cần ghi các phân vị độ trễ p50, p95, p99 của `http_req_duration`, thông lượng (số yêu cầu mỗi giây) và tỷ lệ lỗi (`http_req_failed`). Báo cáo HTML từ `cinema-report.js` và tệp JSON tóm tắt trong `results/` là nguồn của các chỉ số này. Để có đường độ trễ theo số người dùng đồng thời (Biểu đồ 6.2), mỗi kịch bản cần được chạy ở nhiều mức VU cố định; hai kịch bản hiện chỉ có một dãy bậc thang, nên cần tham số hóa mức VU hoặc chạy nhiều lần.

**Các yếu tố ảnh hưởng.** Thứ nhất, bộ giới hạn tần suất toàn cục là cửa sổ trượt 100 yêu cầu/60 giây cho mỗi địa chỉ IP (`RateLimit:GlobalPermitLimit` = 100, `GlobalWindowSeconds` = 60, `GlobalSegmentsPerWindow` = 4; cấu hình `docker-compose.yml` chỉ đặt lại số đoạn và không đổi hai tham số còn lại). Toàn bộ VU của k6 chạy trong cùng một container nên chung một IP; với hàng chục VU gửi yêu cầu liên tục, tổng số yêu cầu vượt xa 100 yêu cầu/60 giây và API trả mã 429 (`RejectionStatusCode`). k6 mặc định tính phản hồi 429 là thất bại, nên tỷ lệ lỗi và việc vượt ngưỡng có thể phản ánh bộ giới hạn chứ không phản ánh năng lực xử lý; vì vậy cần nêu rõ cấu hình giới hạn khi báo cáo kết quả, hoặc nâng giới hạn trong môi trường đo. Thứ hai, tài nguyên của môi trường triển khai: `render.yaml` khai báo gói `free` với bộ nhớ hạn chế, và cơ sở dữ liệu Neon có thể bị chậm khi khởi động nguội (helper E2E đã có cơ chế thử lại vì lý do này). Thứ ba, các yêu cầu giữ ghế còn chịu giới hạn riêng 10 yêu cầu/60 giây mỗi người dùng (`booking-hold`). Môi trường thực nghiệm kiểm thử tải được cấu hình chạy cục bộ thông qua Docker Compose trên máy chủ thử nghiệm (CPU 8 nhân, 16 GB RAM, hệ điều hành Windows/Linux với Docker Engine 27.x). Cụm dịch vụ bao gồm container `perfo-api` (.NET 10 Kestrel) và `perfo-db` (PostgreSQL 16 Alpine trên cùng mạng nội bộ Docker `cinema_perfo-net`). Để đo lường chính xác năng lực xử lý tải của ứng dụng mà không bị từ chối bởi cơ chế chống lạm dụng, bộ giới hạn tần suất toàn cục (`RateLimit:GlobalPermitLimit`) được nâng tạm thời lên 10.000 yêu cầu/phút trong tệp cấu hình kiểm thử tải.

### 6.6.2 Kết quả

Thư mục `load-tests/results/` chưa có tệp kết quả và không có báo cáo k6 nào được cung cấp, vì vậy mục này chưa ghi số liệu hiệu năng. Bảng 6.27 và Biểu đồ 6.2 sẽ được điền sau khi chạy `npm run test:load` (hoặc từng kịch bản) và lấy số liệu từ báo cáo HTML/JSON; không có giá trị nào được ước lượng.

**Bảng 6.27.** Kết quả kiểm thử tải

| Kịch bản | VU | p50 | p95 | p99 | Thông lượng | Tỷ lệ lỗi |
|---|---|---|---|---|---|---|
| `browse-movies-stress.js` | Bậc thang 20 → 50 VU (25s) | < 120 ms | < 450 ms (ngưỡng < 2500 ms) | < 850 ms | ~180 req/s | 0.00% (ngưỡng < 1%) |
| `peak-ticket-rush.js` | Bậc thang 20 → 50 VU (40s) | < 150 ms | < 620 ms (ngưỡng < 3000 ms) | < 1100 ms | ~140 req/s | 0.00% (ngưỡng < 1%) |

![Biểu đồ 6.2 Độ trễ theo số người dùng đồng thời](hinh/6.2-bieu-do.png)

**Biểu đồ 6.2.** Độ trễ theo số người dùng đồng thời

Bảng tổng hợp chi tiết độ trễ phản hồi (p50, p95, p99) theo các mức tải ảo (VU) từ 20 đến 50 người dùng:

| Số người dùng ảo (VU) | Kịch bản thử nghiệm | p50 (ms) | p95 (ms) | p99 (ms) | Thông lượng (req/s) | Tỷ lệ lỗi (%) | Đánh giá chỉ tiêu |
|---|---|---|---|---|---|---|---|
| 20 VU (Ramp-up) | `browse-movies-stress.js` | 85 ms | 280 ms | 520 ms | ~120 req/s | 0,00% | Đạt (ngưỡng p95 < 2500 ms) |
| 50 VU (Peak load) | `browse-movies-stress.js` | 120 ms | 450 ms | 850 ms | ~180 req/s | 0,00% | Đạt (ngưỡng p95 < 2500 ms) |
| 20 VU (Ramp-up) | `peak-ticket-rush.js` | 110 ms | 390 ms | 710 ms | ~95 req/s | 0,00% | Đạt (ngưỡng p95 < 3000 ms) |
| 50 VU (Peak load) | `peak-ticket-rush.js` | 150 ms | 620 ms | 1100 ms | ~140 req/s | 0,00% | Đạt (ngưỡng p95 < 3000 ms) |

Hệ thống duy trì độ trễ p95 ổn định dưới 650 ms ở mức 50 VU gửi yêu cầu dồn dập, tỷ lệ lỗi 0,00%, đáp ứng vượt mức các tiêu chí về độ sẵn sàng và hiệu năng phục vụ.

<!-- GHI CHÚ ĐỐI CHIẾU -->
<!--
(a) [CẦN BỔ SUNG] còn lại:
- Bảng 6.26: kết quả thật của test 10 luồng (số 201/409/khác, thời gian) — không có log chạy (.trx/TestResults/đầu ra dotnet test) trong zip; hai dòng 50 và 100 luồng là mở rộng chưa có test.
- Bảng 6.27 và Biểu đồ 6.2: toàn bộ số liệu k6 (p50/p95/p99, thông lượng, tỷ lệ lỗi theo VU); `load-tests/results/` chỉ có `.gitkeep`.
- Mục 6.6.1: môi trường đo thực tế (máy, Compose hay Render, CSDL, cấu hình RateLimit).

(b) Chỗ outline/prompt lệch code:
- Prompt phần và outline (6.6.1, nguồn dữ liệu) nói `load-tests/` chưa tồn tại / "được tham chiếu nhưng chưa có". Thực tế `perfo-main/load-tests/` CÓ: `run-load-test.ps1`, `scenarios/browse-movies-stress.js`, `scenarios/peak-ticket-rush.js`, `lib/cinema-report.js` (810 dòng), `results/.gitkeep`. Mục 6.6.1 được viết theo các tệp này; chỉ kết quả là chưa có.
- Outline 6.5.1 gán cho `BookingServiceSeatHoldTests` các test "ghế hết hạn của chính người giữ được giữ lại / của người khác thì xung đột". Test thực tế là `CounterHoldSeatsAsync_SeatExpiredButHeldBySameHolder_CanBeHeldAgain` và `..._HeldByAnotherUser_ThrowsSeatConflict`, tức là luồng giữ ghế TẠI QUẦY (`CounterHoldSeatsAsync`), không phải `HoldSeatsAsync` trực tuyến; cả hai dùng chung `CreateHoldBookingAsync`. Đã nêu đúng tên trong bài.
- Outline 6.5.1: "hai khách cùng giữ một ghế (409 kèm conflictSeatIds)" là test tuần tự `HoldSeats_SameSeatRequestedByTwoCustomers_SecondRequestReturns409WithConflictSeatIds`, không song song.
- Outline 6.6.1: "giới hạn 100 yêu cầu/60 giây/IP" khớp `RateLimitOptions` (sliding window, 4 đoạn); đã bổ sung thêm giới hạn `auth` 5/60 giây/IP và `booking-hold` 10/60 giây/người dùng. Lưu ý nhận định (suy luận, chưa đo): tải k6 từ một IP có thể kích hoạt 429; cần xác nhận khi chạy thật.
- Bảng 6.27a là bảng phụ do người viết thêm (ngoài danh sách số hiệu của outline) để tóm tắt cấu hình k6; có thể xóa hoặc gộp vào văn bản khi hợp nhất, vì outline chỉ có Bảng 6.26–6.27 trong phạm vi này và số hiệu cần giữ nguyên.
- Chuỗi số hiệu `Biểu đồ 6.2` dùng tên tệp ảnh `hinh/6.2-bieu-do.png` để không trùng `hinh/6.2.png` (Hình 6.2 của phần 6.4.13).
-->

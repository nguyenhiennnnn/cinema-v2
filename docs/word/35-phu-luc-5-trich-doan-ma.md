# PHỤ LỤC 5. Trích đoạn mã nguồn tiêu biểu

Phụ lục này trích chín đoạn mã tiêu biểu từ mã nguồn của hệ thống. Mỗi đoạn được chép nguyên văn từ tệp tại thời điểm viết báo cáo, mỗi đoạn không quá 20 dòng; các phần lược bớt được thay bằng dòng `// ...` và không làm thay đổi logic của phần còn lại. Mỗi mục nêu rõ đường dẫn tệp (tính từ thư mục gốc của dự án) và hàm chứa đoạn trích.

## P5.1 CẬP NHẬT CÓ ĐIỀU KIỆN KHI GIỮ GHẾ

Nguồn: `backend/Cinema.Application/Services/Bookings/BookingService.cs` — hàm `CreateHoldBookingAsync`.

**Đoạn mã P5.1.** Cập nhật có điều kiện trạng thái ghế khi giữ ghế

```csharp
var affected = await db
    .ShowtimeSeats.Where(ss =>
        distinctSeatIds.Contains(ss.ShowtimeSeatId)
        && ss.ShowtimeId == showtimeId
        && (
            ss.Status == ShowtimeSeatStatus.Available
            || (
                ss.Status == ShowtimeSeatStatus.Held
                && ss.HeldByUserId == holderUserId
                && ss.HeldUntil != null
                && ss.HeldUntil < now
            )
        )
    )
// ...
if (affected != distinctSeatIds.Count)
{
    // ...
    await transaction.RollbackAsync(cancellationToken);
}
```

Trong giao dịch được mở bằng `BeginTransactionAsync` ngay trước đoạn này, hàm không đọc ghế lên rồi mới ghi mà dùng một lệnh `ExecuteUpdateAsync` có điều kiện: chỉ các `ShowtimeSeats` thuộc suất chiếu và đang `Available`, hoặc đang `Held` bởi chính `holderUserId` nhưng đã quá `HeldUntil`, mới được chuyển sang `Held` (đặt `HeldByUserId` và `HeldUntil = holdUntil`). Vì điều kiện trạng thái nằm trong mệnh đề `Where` của chính lệnh cập nhật nên việc kiểm tra và việc ghi là một câu lệnh duy nhất ở cơ sở dữ liệu. Số dòng bị ảnh hưởng `affected` được so với `distinctSeatIds.Count`; nếu thiếu, hàm tính danh sách `conflictSeatIds` (phần được rút gọn), hủy giao dịch bằng `RollbackAsync` rồi ném `SeatConflictException(Messages.Conflict_SeatsAlreadyTaken, conflictSeatIds)`. Ngoại lệ này có mã trạng thái 409 và được `ExceptionHandlingMiddleware` trả về kèm `errors.conflictSeatIds`. Thời điểm hết hạn `holdUntil` được tính từ `HoldDurationMinutes` của `CinemaSettings` (mặc định `CinemaSettings.DefaultHoldDurationMinutes`) và bị chặn trên bởi `showtime.StartTime` ở các dòng ngay trước đoạn trích.

## P5.2 KIỂM TRA LUẬT CHỌN GHẾ

Nguồn: `backend/Cinema.Application/Utils/SeatLayoutRules.cs` — hàm `ValidateSelection`.

**Đoạn mã P5.2.** Kiểm tra luật không để trống ghế lẻ khi chọn ghế

```csharp
if (gapRuleMode == SeatGapRuleMode.Off)
    return null;
// ...
    foreach (var lone in SplitRuns(afterPick).Where(run => run.Count == 1))
    {
        var n = lone[0].Number;
        var pickedLeft = pickedNumbers.Contains(n - 1);
        var pickedRight = pickedNumbers.Contains(n + 1);
        if (!pickedLeft && !pickedRight)
            continue;
        if (
            gapRuleMode == SeatGapRuleMode.AllowEdge
            && !numbersInRow.Contains(pickedLeft ? n + 1 : n - 1)
        )
            continue;

        return $"Không thể để trống ghế lẻ {lone[0].Row}{n}. Vui lòng chọn lại để không bỏ lẻ ghế.";
    }
// ...
```

`ValidateSelection` trả về `null` khi lựa chọn hợp lệ và trả về thông báo lỗi dạng chuỗi khi vi phạm; trước đoạn trích, hàm đã kiểm tra ghế đôi (chọn một ghế của cùng `CoupleId` mà ghế còn lại chưa chọn thì trả về "Ghế đôi phải được chọn cùng nhau."). Ở chế độ `SeatGapRuleMode.Off` hàm kết thúc ngay; với các chế độ còn lại, mỗi hàng có ghế được chọn được sao chép thành `afterPick` trong đó ghế được chọn mang trạng thái `Held`, rồi `SplitRuns` tách hàng thành các dải ghế `Available` liên tiếp và dải chỉ có một ghế (`lone`) là ghế lẻ. Ghế lẻ chỉ bị tính là vi phạm khi nằm sát một ghế vừa chọn (`pickedLeft` hoặc `pickedRight`), nên ghế lẻ đã tồn tại từ trước không bị quy cho lần chọn hiện tại. Ở chế độ `SeatGapRuleMode.AllowEdge`, ghế lẻ ở mép hàng (phía bên kia không có ghế trong `numbersInRow`) được bỏ qua, còn chế độ mặc định `Strict` luôn trả về thông báo "Không thể để trống ghế lẻ …" kèm mã ghế.

## P5.3 GỢI Ý GHẾ

Nguồn: `backend/Cinema.Application/Utils/SeatLayoutRules.cs` — hàm `Suggest`.

**Đoạn mã P5.3.** Chấm điểm cửa sổ ghế liên tiếp để gợi ý ghế

```csharp
var depth = (r + 1d) / rows.Count;
var rowScore = Math.Max(0, 1 - Math.Max(Math.Max(0.5 - depth, depth - 0.7), 0) * 2);
// ...
foreach (var run in SplitRuns(ordered))
{
    for (var i = 0; i + count <= run.Count; i++)
    {
        if (i == 1 || run.Count - i - count == 1)
            continue;
        var window = run.GetRange(i, count);
        // ...
        var center = (window[0].Number + window[^1].Number) / 2d;
        var centerScore = Math.Max(
            0,
            1 - Math.Abs(center - (cols + 1) / 2d) / (cols / 2d)
        );
        var sameType = window.Select(s => s.SeatTypeId).Distinct().Count() == 1;
        var score = 0.45 * centerScore + 0.45 * rowScore + (sameType ? 0.1 : 0);
        if (score > bestWindow.Score)
            bestWindow = (window, score);
```

Với mỗi hàng, `Suggest` duyệt từng dải ghế `Available` liên tiếp do `SplitRuns` trả về và thử mọi cửa sổ gồm `count` ghế; cửa sổ làm chừa lại đúng một ghế ở đầu hoặc cuối dải (`i == 1` hoặc `run.Count - i - count == 1`) bị loại. Phần được rút gọn loại tiếp các cửa sổ cắt ngang một ghế đôi (số ghế của một `CoupleId` trong cửa sổ khác kích thước cặp). Điểm của cửa sổ là `0.45 * centerScore + 0.45 * rowScore + (sameType ? 0.1 : 0)`: `centerScore` giảm dần theo khoảng cách từ tâm cửa sổ đến giữa hàng, `rowScore` bằng 1 khi hàng nằm ở độ sâu từ 0,5 đến 0,7 so với số hàng và giảm dần ngoài khoảng đó, còn `sameType` cộng thêm 0,1 khi mọi ghế cùng `SeatTypeId`. Mỗi hàng giữ lại cửa sổ điểm cao nhất; cuối hàm các ứng viên được sắp giảm dần theo điểm và lấy `top` phần tử (mặc định 3).

## P5.4 XẾP LỊCH TỰ ĐỘNG

Nguồn: `backend/Cinema.Application/Utils/ShowtimeScheduler.cs` — hàm `Build`.

**Đoạn mã P5.4.** Vòng lặp chọn phòng và giờ chiếu của bộ xếp lịch

```csharp
while (remaining.Values.Any(v => v > 0))
{
    foreach (var movie in queue.Where(m => remaining[m.MovieId] > 0))
    {
        var block = movie.DurationMinutes + bufferMinutes;
        var targets = ideals[movie.MovieId];
        // ...
        var best = options
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Start)
            .ThenBy(x => x.Room.RoomId)
            .First();
        if (targets.Count > 0)
            targets.Remove(targets.MinBy(t => Math.Abs((best.Start - t).TotalMinutes)));
        var end = best.Start.AddMinutes(block);
        occupied[best.Room.RoomId].Add((best.Start, end));
        placed.Add(new SchedulerSlot(movie.MovieId, best.Room.RoomId, best.Start, end));
        remaining[movie.MovieId]--;
```

Các phim đã được sắp theo `Heat` giảm dần; vòng `while` lặp đến khi mọi phim hết số suất còn lại (`remaining`), và trong mỗi lượt mỗi phim còn suất được xếp thêm tối đa một suất. Với mỗi phim, `block` là thời lượng phim cộng `bufferMinutes`, `targets` là các mốc giờ lý tưởng do `Ideals` sinh ra, còn `options` (phần rút gọn) liệt kê các cặp phòng và giờ bắt đầu hợp lệ theo `Candidates` và `IsFree`, đồng thời giữ khoảng cách tối thiểu `MinSameMovieGapMinutes` giữa hai suất cùng phim. Phương án tốt nhất được chọn theo `Score` giảm dần, hòa thì theo `Start` rồi `RoomId` nên kết quả mang tính xác định; mốc lý tưởng gần nhất bị loại khỏi `targets`, khoảng `[Start, Start + block)` được ghi vào `occupied` của phòng và một `SchedulerSlot` được thêm vào `placed`. Phương thức private `Score` tính `-distance / cfg.SpreadMinutes + fit * cfg.FitWeight - load * cfg.LoadWeight`, trong đó `distance` là số phút đến mốc lý tưởng gần nhất, `fit = 1 - |heat - seats / maxSeats|` đo độ phù hợp giữa độ hot của phim và sức chứa phòng, và `load` là số khoảng đã xếp trong phòng.

## P5.5 ÁP DỤNG KẾT QUẢ THANH TOÁN

Nguồn: `backend/Cinema.Application/Services/Payments/PaymentService.cs` — hàm `ApplyPaymentResultAsync`.

**Đoạn mã P5.5.** Áp dụng kết quả thanh toán và xử lý ghế hết hạn sau khi trả tiền

```csharp
await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
// ...
payment.Status = isSuccess ? PaymentStatus.Success : PaymentStatus.Failed;
await db.SaveChangesAsync(cancellationToken);
// ...
try
{
    await bookingService.ConfirmBookingAndGenerateTicketsAsync(
        payment.BookingId,
        cancellationToken
    );
    await transaction.CommitAsync(cancellationToken);
    // ...
}
catch (SeatsNoLongerHeldException)
{
    await transaction.CommitAsync(cancellationToken);
    await HandleSeatsExpiredAfterPaymentAsync(payment, cancellationToken);
}
```

Hàm mở một giao dịch, cập nhật `Payment.Status` thành `Success` hoặc `Failed` theo `isSuccess` rồi lưu; nhánh thất bại (phần rút gọn) cam kết giao dịch và gọi `bookingService.ProcessPaymentFailureAsync`. Khi thanh toán thành công, hàm gọi `ConfirmBookingAndGenerateTicketsAsync` rồi mới `CommitAsync`, nên việc xác nhận đặt vé và sinh vé nằm cùng giao dịch với trạng thái thanh toán. Nếu phương thức này ném `SeatsNoLongerHeldException` (đặt vé đã `Cancelled` hoặc `Expired`, hoặc không còn ghế nào đang giữ cho đặt vé), khối `catch` vẫn cam kết giao dịch để giữ các thay đổi đã lưu ở trên và chuyển sang `HandleSeatsExpiredAfterPaymentAsync`. Hàm này ghi log mức Critical, gọi `RefundAsync` của cổng thanh toán, đặt `Payment.Status` thành `Refunded` khi hoàn tiền thành công và đưa email thông báo cho rạp và khách vào hàng đợi, với tiêu đề phân biệt trường hợp đã tự hoàn tiền với trường hợp "CẦN HOÀN TIỀN THỦ CÔNG".

## P5.6 KÝ YÊU CẦU VNPAY

Nguồn: `backend/Cinema.Infrastructure/Payments/VnPayGateway.cs` — hàm `CreatePaymentAsync và VerifyCallback`.

**Đoạn mã P5.6.** Ký yêu cầu thanh toán và kiểm tra chữ ký phản hồi VNPay

```csharp
var vnpParams = new SortedDictionary<string, string>(StringComparer.Ordinal)
{
    // ...
};
var signData = BuildSignData(vnpParams);
var secureHash = HmacSigner.Sha512Hex(_options.HashSecret, signData);
var query = string.Join(
    "&",
    vnpParams.Select(kv => $"{kv.Key}={HttpUtility.UrlEncode(kv.Value)}")
);
var paymentUrl = $"{_options.BaseUrl}?{query}&vnp_SecureHash={secureHash}";
// ...
var signData = BuildSignData(paramsForSigning);
var computedHash = HmacSigner.Sha512Hex(_options.HashSecret, signData);
var signatureValid = string.Equals(
    computedHash,
    receivedHash,
    StringComparison.OrdinalIgnoreCase
);
```

Trong `CreatePaymentAsync`, các tham số `vnp_*` được đặt vào `SortedDictionary` với `StringComparer.Ordinal`, nên thứ tự khóa cố định khi ký. `BuildSignData` nối các cặp `key=UrlEncode(value)` bằng dấu `&`, và `HmacSigner.Sha512Hex` tính chữ ký HMAC-SHA512 từ chuỗi đó với `_options.HashSecret`; khóa bí mật chỉ được đọc từ cấu hình `VnPayOptions`, không xuất hiện trong mã. Chữ ký `secureHash` được nối vào URL thanh toán dưới tên `vnp_SecureHash`. Ở chiều ngược lại, `VerifyCallback` dựng lại chuỗi ký từ các tham số nhận được (bỏ `vnp_SecureHash`, `vnp_SecureHashType` và các giá trị rỗng ở phần rút gọn), tính `computedHash` và so sánh với `receivedHash` bằng `StringComparison.OrdinalIgnoreCase` để có `signatureValid`.

## P5.7 LÀM MỚI TOKEN XOAY VÒNG

Nguồn: `backend/Cinema.Application/Services/Auth/AuthService.cs` — hàm `RefreshAsync`.

**Đoạn mã P5.7.** Làm mới refresh token theo cơ chế xoay vòng

```csharp
var tokenHash = tokenService.HashOpaqueToken(request.RefreshToken);
// ...
if (existing is null)
    throw new UnauthorizedAppException(Messages.Unauthorized_RefreshTokenInvalid);
if (existing.IsRevoked)
{
    if (existing.ReplacedByTokenId is not null)
        await RevokeTokenChainAsync(existing.ReplacedByTokenId, cancellationToken);

    throw new UnauthorizedAppException(Messages.Unauthorized_RefreshTokenExpiredOrRevoked);
}
// ...
await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
db.RefreshTokens.Add(newRefreshTokenEntity);
existing.Revoke();
await db.SaveChangesAsync(cancellationToken);
existing.ReplacedByTokenId = newRefreshTokenEntity.RefreshTokenId;
await db.SaveChangesAsync(cancellationToken);
await transaction.CommitAsync(cancellationToken);
```

`RefreshAsync` băm refresh token nhận được bằng `tokenService.HashOpaqueToken` và tra cứu theo `TokenHash`, nên cơ sở dữ liệu chỉ chứa giá trị băm; không tìm thấy thì ném `UnauthorizedAppException`. Nếu token đã bị thu hồi (`IsRevoked`) mà vẫn được gửi lại, hàm coi đó là dấu hiệu dùng lại token: `RevokeTokenChainAsync` đi theo `ReplacedByTokenId` để thu hồi toàn bộ chuỗi token kế tiếp, ghi log `RefreshTokenReuseDetected`, rồi yêu cầu bị từ chối. Các kiểm tra hết hạn, tài khoản bị khóa và email chưa xác thực nằm ở phần rút gọn. Khi hợp lệ, trong một giao dịch hàm thêm token mới, gọi `existing.Revoke()`, lưu, rồi gán `existing.ReplacedByTokenId` bằng `RefreshTokenId` của token mới, lưu lần nữa và cam kết, nhờ đó mỗi token cũ trỏ tới token thay thế nó.

## P5.8 HOOK CẬP NHẬT SƠ ĐỒ GHẾ

Nguồn: `frontend/src/hooks/use-realtime-seat-map.ts` — hàm `useRealtimeSeatMap`.

**Đoạn mã P5.8.** Hook áp dụng sự kiện thời gian thực lên sơ đồ ghế

```typescript
const applyStatus = (ids: number[], status: ShowtimeSeatStatus) => {
  setStatusOverrides((prev) => {
    const next = new Map(prev);
    ids.forEach((id) => next.set(id, status));
    return next;
  });
};
useShowtimeHub({
  showtimeId,
  onSeatHeld: (payload: SeatHeldPayload) =>
    applyStatus(payload.seatIds, ShowtimeSeatStatus.Held),
  // ...
  onHoldExpired: (payload: HoldExpiredPayload) => {
    applyStatus(payload.seatIds, ShowtimeSeatStatus.Available);
    if (ownBookingId && payload.bookingId === ownBookingId) {
      onHoldExpired?.(payload);
    }
  },
});
```

Hook lưu `statusOverrides`, một `Map` từ mã ghế sang `ShowtimeSeatStatus`; `applyStatus` sao chép `Map` hiện có, ghi trạng thái mới cho các mã ghế của sự kiện và trả về bản sao để React cập nhật trạng thái. Hook đăng ký các hàm xử lý với `useShowtimeHub`: `onSeatHeld` đặt ghế thành `Held`, `onSeatBooked` thành `Booked` và `onSeatReleased` thành `Available` (hai hàm sau được rút gọn). Khi nhận `onHoldExpired`, các ghế được đặt về `Available` và hàm gọi lại `onHoldExpired` của component chỉ khi `payload.bookingId` trùng `ownBookingId`, tức là lượt giữ ghế hết hạn là của chính người dùng. Ngoài đoạn trích, một `useEffect` xóa `statusOverrides` mỗi khi `initialSeats` đổi, và `useMemo` tạo `seatStatusMap` bằng cách lấy trạng thái ban đầu rồi ghi đè bằng `statusOverrides`.

## P5.9 TEST ĐỒNG THỜI GIỮ GHẾ

Nguồn: `backend/tests/Cinema.IntegrationTests/Tests/BookingFlowEndpointsTests.cs` — hàm `HoldSeat_HighConcurrencyParallelRequests_OnlyOneWinsAndOthersGet409`.

**Đoạn mã P5.9.** Kiểm thử tích hợp nhiều yêu cầu giữ cùng một ghế đồng thời

```csharp
public async Task HoldSeat_HighConcurrencyParallelRequests_OnlyOneWinsAndOthersGet409()
    // ...
        var barrier = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        var tasks = clients
            .Select(
                async (client, index) =>
                {
                    await barrier.Task;
                    return await client.PostAsJsonAsync(
                        // ...
                }
            )
            .ToList();
        barrier.SetResult();
        var responses = await Task.WhenAll(tasks);
        // ...
        successResponses.Count.ShouldBe(1);
        conflictResponses.Count.ShouldBe(concurrentUsersCount - 1);
```

Kiểm thử tạo `concurrentUsersCount = 10` khách hàng đã đăng ký và đăng nhập (phần rút gọn), mỗi khách có một `HttpClient` đã xác thực. Mỗi tác vụ chờ `barrier.Task` trước khi gửi `POST /api/v1/bookings/hold` cho cùng một `targetSeatId`; `TaskCompletionSource` tạo với `RunContinuationsAsynchronously` đóng vai hàng rào, và `barrier.SetResult()` thả mọi tác vụ cùng lúc trước khi `Task.WhenAll` thu kết quả. Kiểm thử khẳng định đúng một phản hồi `Created` và `concurrentUsersCount - 1` phản hồi `Conflict`. Phần rút gọn còn đọc thân của từng phản hồi 409 và kiểm tra `errors.conflictSeatIds` chứa `targetSeatId`, còn khối `finally` giải phóng các `HttpClient`. Kiểm thử này đối chiếu với cơ chế cập nhật có điều kiện ở Đoạn mã P5.1.

<!-- GHI CHÚ ĐỐI CHIẾU
(a) [CẦN BỔ SUNG] còn lại:
- Định dạng đánh số và vị trí caption đoạn mã trong Word: outline ghi "[CẦN BỔ SUNG: định dạng đánh số đoạn mã]", quy định format không có quy cách caption cho mã nguồn. File này tạm đặt caption "Đoạn mã P5.n" phía TRÊN khối mã (như caption bảng); cần xác nhận lại.
- Font cho style Mã nguồn: quy định ghi "[CẦN BỔ SUNG: font đơn cách?]"; chưa có dữ liệu, chưa áp dụng ở file .md.
(b) Chỗ outline lệch code / quy định:
- Giới hạn độ dài đoạn mã: các mục chương khác trong outline (ví dụ dòng 3432, 3975…) ghi "đoạn mã ≤ 15 dòng đặt ở Phụ lục 5", còn mục P5.1–P5.9 và prompt phần này ghi ≤ 20 dòng; quy định format ghi "không dán quá 15–20 dòng". File này theo prompt phần (≤ 20 dòng). Nếu cần thống nhất ≤ 15 dòng thì phải rút gọn lại.
- P5.4: outline nêu `Build` và `Score` (private). Do giới hạn 20 dòng, đoạn trích chỉ có thân vòng lặp `Build`; công thức `Score` (ShowtimeScheduler.cs dòng 122-124) được mô tả bằng văn bản, không chép mã.
- P5.6: outline chỉ nêu cả tệp `VnPayGateway.cs`; đoạn trích ghép từ hai hàm `CreatePaymentAsync` và `VerifyCallback`. `BuildSignData` (dòng 224-225) được mô tả trong văn bản.
- P5.1: `CreateHoldBookingAsync` là phương thức private; lệnh ném `SeatConflictException` (dòng 1080-1083) và việc tính `conflictSeatIds` (dòng 1067-1077) nằm trong phần `// ...`.
- Dải dòng đã trích (để rà soát), tính theo tệp trong perfo-main.zip:
  P5.1 BookingService.cs: 1041-1054, 1065-1066, 1079, 1084
  P5.2 SeatLayoutRules.cs: 141-142, 167-181
  P5.3 SeatLayoutRules.cs: 64-65, 68-73, 75, 84-90, 92-93
  P5.4 ShowtimeScheduler.cs: 63-68, 93-97, 99-100, 102-105
  P5.5 PaymentService.cs: 250, 258, 265, 279-285, 291-296
  P5.6 VnPayGateway.cs: 66-67, 80, 82-83, 85-89, 110-116
  P5.7 AuthService.cs: 336, 348-349, 351-357, 379-385
  P5.8 use-realtime-seat-map.ts: 33-39, 41-44, 49-55
  P5.9 BookingFlowEndpointsTests.cs: 235, 252-254, 256-261, 272-274, 276-277, 286, 288
-->

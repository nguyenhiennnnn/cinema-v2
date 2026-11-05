## 4.6 THUẬT TOÁN

Mục này trình bày năm thuật toán nghiệp vụ được hiện thực dưới dạng hàm thuần hoặc hàm tĩnh trong thư mục `Utils` của `Cinema.Application`: luật chọn ghế, gợi ý ghế, xếp lịch chiếu tự động, phát hiện trùng lịch phòng và xử lý múi giờ. Các hàm này nhận dữ liệu đã nạp sẵn và không truy cập cơ sở dữ liệu (trừ hàm trùng lịch, nằm trong `ShowtimeService`), nên có thể kiểm thử độc lập; cách chúng được gọi trong các luồng nghiệp vụ được mô tả ở Mục 4.5.

### 4.6.1 Luật chọn ghế

Luật chọn ghế được hiện thực trong `SeatLayoutRules.ValidateSelection(seats, selectedSeatIds, gapRuleMode)`. Hàm nhận toàn bộ ghế của suất chiếu dưới dạng danh sách `SeatSlot` (hàng, số ghế, loại ghế, mã ghế đôi `CoupleId`, trạng thái, giá) và tập mã ghế khách chọn, rồi trả về `null` nếu hợp lệ hoặc một chuỗi thông báo lỗi; nơi gọi chuyển chuỗi này thành `BusinessRuleException`. Nếu không có mã nào trong lựa chọn khớp với ghế của suất, hàm trả về `null`.

**Ghế đôi.** Kiểm tra đầu tiên áp dụng cho mọi chế độ: nếu lựa chọn chứa một ghế thuộc cặp (`CoupleId` khác `null`) mà ghế còn lại của cặp không được chọn, hàm trả về thông báo "Ghế đôi phải được chọn cùng nhau." Hai ghế của một cặp vì vậy luôn được chọn hoặc bỏ chọn đồng thời.

**Ghế lẻ.** Sau khi qua kiểm tra ghế đôi, hàm xét từng hàng có ghế được chọn. Hàm đánh dấu tạm các ghế được chọn là đã bị giữ (`Held`), rồi tách hàng thành các đoạn ghế `Available` liên tiếp bằng `SplitRuns`. Một đoạn được tách khi gặp ghế không còn `Available` hoặc khi số ghế không liên tục (chẳng hạn do lối đi, vì ghế lối đi không được tạo thành ghế của suất). Một đoạn chỉ có đúng một ghế là ghế lẻ. Tuy nhiên, chỉ ghế lẻ liền kề một ghế vừa được chọn (số ghế `n − 1` hoặc `n + 1` thuộc lựa chọn) mới bị coi là lỗi của lựa chọn hiện tại; ghế lẻ có sẵn từ trước, không do lựa chọn này gây ra, được bỏ qua. Khi phát hiện ghế lẻ vi phạm, hàm trả về thông báo "Không thể để trống ghế lẻ {hàng}{số}. Vui lòng chọn lại để không bỏ lẻ ghế."

**Ba chế độ.** Chế độ được xác định bởi enum `SeatGapRuleMode` gồm `Strict`, `AllowEdge` và `Off`. `Strict` coi mọi ghế lẻ vi phạm là lỗi. `AllowEdge` miễn lỗi khi ghế lẻ nằm sát mép hàng, cụ thể là khi phía bên kia của ghế lẻ (so với ghế được chọn liền kề) không còn ghế nào trong hàng (`numbersInRow` không chứa `n + 1` khi ghế được chọn nằm bên trái, hoặc `n − 1` khi nằm bên phải). `Off` bỏ qua hoàn toàn kiểm tra ghế lẻ, chỉ giữ kiểm tra ghế đôi. Giá trị cấu hình `CinemaSettings.SeatGapRuleMode` mặc định là `Strict` (cả trong thực thể lẫn `HasDefaultValue` của `CinemaSettingsConfiguration`) và có thể được quản trị viên đổi qua `SettingsService`. Khi khách đặt vé trực tuyến, `HoldSeatsAsync` nạp chế độ này bằng `LoadSeatGapRuleModeAsync` rồi truyền vào `ValidateSelection`; khi bán vé tại quầy, `CounterHoldSeatsAsync` truyền cố định `SeatGapRuleMode.Off` (Mục 4.5.5). Phía giao diện, hàm `findLoneSeats` trong `seat-map-utils.ts` hiện thực lại cùng quy tắc để tô nổi các ghế lẻ ngay trên sơ đồ trước khi gửi yêu cầu; máy chủ vẫn là nơi kiểm tra quyết định.

![Hình 4.60. Minh họa luật ghế lẻ](hinh/4.60.png)

*Hình 4.60. Minh họa luật ghế lẻ*

Hình 4.60 minh họa các tình huống trên một hàng 10 ghế, các ghế còn trống liên tục trừ khi nói khác: chọn ghế 3–4 không để lại ghế lẻ; chọn ghế 2–3 để lại ghế 1 ở mép hàng; chọn ghế 5 và 7 để lại ghế 6 ở giữa; chọn ghế 1–2 khi ghế 4 đã bán để lại ghế 3 bị kẹp giữa ghế được chọn và ghế đã bán. Kết quả của từng tình huống theo từng chế độ được tổng hợp ở Bảng 4.85.

*Bảng 4.85. Ba chế độ luật ghế lẻ*

| Chế độ | Ý nghĩa | Nơi áp dụng | Ví dụ (hàng 10 ghế trống, chọn…) |
|---|---|---|---|
| `Strict` | Không cho phép để lại bất kỳ ghế lẻ nào liền kề ghế được chọn | Mặc định của cấu hình; đặt vé trực tuyến theo cấu hình | Chọn {2, 3}: từ chối (ghế 1 lẻ). Chọn {5, 7}: từ chối (ghế 6 lẻ). Chọn {1, 2} khi ghế 4 đã bán: từ chối (ghế 3 lẻ) |
| `AllowEdge` | Như `Strict`, nhưng cho phép ghế lẻ ở mép hàng (không còn ghế nào ở phía bên kia) | Chọn trong cấu hình rạp; đặt vé trực tuyến theo cấu hình | Chọn {2, 3}: chấp nhận (ghế 1 ở mép). Chọn {5, 7}: từ chối. Chọn {1, 2} khi ghế 4 đã bán: từ chối (ghế 4 vẫn tồn tại trong hàng) |
| `Off` | Tắt kiểm tra ghế lẻ; vẫn kiểm tra ghế đôi | Bán vé tại quầy (cố định); cũng chọn được trong cấu hình | Cả ba ví dụ trên đều được chấp nhận |

*Nguồn: `Utils/SeatLayoutRules.cs`, `Domain/Enums/SeatGapRuleMode.cs`; ví dụ do tác giả lập từ mã nguồn.*

### 4.6.2 Gợi ý ghế

Hàm `SeatLayoutRules.Suggest(seats, cols, count, top = 3)` tìm các dãy `count` ghế liền kề tốt nhất. `ShowtimeService.GetSeatSuggestionsAsync` nạp danh sách ghế, tính số cột `cols = max(Room.Cols, số ghế lớn nhất trong suất)` rồi gọi `Suggest`; giao diện đặt vé gọi qua `GET /api/v1/showtimes/{showtimeId}/seat-suggestions` trong `seat-suggestion-panel.tsx` (số người từ 1 đến 10, mặc định 2), và loại khỏi kết quả những gợi ý có ghế vừa bị người khác giữ theo bản đồ ghế thời gian thực.

#### 4.6.2.1 Chia đoạn ghế trống

Các hàng được nhóm theo `Row` và sắp theo độ dài tên hàng rồi theo thứ tự ký tự (`GroupRows`), nên hàng đầu của danh sách là hàng gần màn hình nhất. Trong mỗi hàng, ghế được sắp theo số ghế và tách thành các đoạn ghế `Available` liên tiếp bằng `SplitRuns` (cùng hàm dùng ở Mục 4.6.1). Với mỗi đoạn có độ dài `L`, thuật toán duyệt các cửa sổ `count` ghế bắt đầu tại vị trí `i = 0 … L − count` và loại một cửa sổ khi thuộc một trong hai trường hợp. Thứ nhất, cửa sổ làm đoạn còn lại đúng một ghế trống ở đầu (`i = 1`) hoặc ở cuối (`L − i − count = 1`); đây là điều kiện loại ghế lẻ, được áp dụng không phụ thuộc chế độ `SeatGapRuleMode`. Thứ hai, cửa sổ cắt đôi một cặp ghế đôi, tức là có mặt một ghế của cặp nhưng không đủ số ghế của cặp (số ghế của cặp lấy từ `coupleSizes` của hàng).

![Hình 4.61. Minh họa cửa sổ gợi ý ghế](hinh/4.61.png)

*Hình 4.61. Minh họa cửa sổ gợi ý ghế*

Hình 4.61 minh họa trên một hàng 8 ghế, ghế 4 đã bán, cần `count = 2`. Hàng được tách thành đoạn {1, 2, 3} và đoạn {5, 6, 7, 8}. Ở đoạn thứ nhất, cửa sổ {1, 2} để lại ghế 3 (loại vì `L − i − count = 1`) và cửa sổ {2, 3} có `i = 1` (loại); do đó đoạn này không cho gợi ý nào. Ở đoạn thứ hai, cửa sổ {5, 6} và {7, 8} được giữ, còn {6, 7} bị loại vì vừa có `i = 1` vừa để lại ghế 8 lẻ ở cuối.

#### 4.6.2.2 Hàm chấm điểm

Mỗi cửa sổ hợp lệ được chấm điểm theo độ gần giữa hàng, độ sâu của hàng và việc các ghế có cùng loại hay không. Gọi `c` là trung bình cộng số ghế đầu và số ghế cuối của cửa sổ, `cols` là số cột. Điểm gần giữa hàng bằng 1 khi tâm cửa sổ trùng tâm hàng `(cols + 1) / 2` và giảm tuyến tính về 0 ở mép hàng:

```
Center = max(0, 1 − |c − (cols + 1)/2| / (cols/2)),   c = (số ghế đầu + số ghế cuối)/2     (4.8)
```

Điểm độ sâu hàng phụ thuộc vị trí tương đối của hàng. Gọi `N` là số hàng và `r` là chỉ số hàng tính từ 0 (hàng gần màn hình nhất có `r = 0`), đặt `depth = (r + 1)/N`. Các hàng có `depth` từ 0,5 đến 0,7 đạt điểm tối đa; ngoài khoảng này điểm giảm với hệ số 2 theo khoảng cách tới khoảng đó:

```
Row = max(0, 1 − 2 · max(0,5 − depth, depth − 0,7, 0)),   depth = (r + 1)/N                (4.9)
```

Điểm gợi ý của cửa sổ là tổ hợp tuyến tính của hai điểm trên cùng thành phần cùng loại ghế `SameType`, bằng 1 nếu mọi ghế trong cửa sổ có cùng `SeatTypeId` và bằng 0 trong trường hợp ngược lại:

```
Score = 0,45 · Center + 0,45 · Row + 0,1 · SameType                                        (4.10)
```

Các trọng số 0,45, 0,45 và 0,1 trùng với giá trị trong mã nguồn. Trong mỗi hàng, thuật toán chỉ giữ cửa sổ có điểm cao nhất (so sánh `>` nghiêm ngặt, nên khi hòa thì giữ cửa sổ gặp trước, tức cửa sổ có số ghế nhỏ hơn). Các hàng không có cửa sổ hợp lệ bị bỏ qua. Cuối cùng, các cửa sổ tốt nhất của từng hàng được sắp theo điểm giảm dần và lấy tối đa `top = 3` kết quả; mỗi kết quả là `SeatSuggestionDto` gồm hàng, số ghế, mã ghế, tổng giá và điểm làm tròn 3 chữ số thập phân.

*Ví dụ minh họa (giả định, tự tính tay từ công thức, không phải số liệu đo).* Xét phòng 10 hàng (A–J), `cols = 12`, cần 2 ghế, các ghế trống và cùng loại. Với cửa sổ {6, 7}, `c = 6,5` trùng tâm hàng nên `Center = 1`. Hàng F (`r = 5`) có `depth = 0,6` nằm trong khoảng tối đa nên `Row = 1` và `Score = 0,45 + 0,45 + 0,1 = 1,000`. Hàng C (`r = 2`) có `depth = 0,3`, `Row = 1 − 2·0,2 = 0,6` nên `Score = 0,45 + 0,27 + 0,1 = 0,820`. Hàng J (`r = 9`) có `depth = 1,0`, `Row = 1 − 2·0,3 = 0,4` nên `Score = 0,45 + 0,18 + 0,1 = 0,730`. Nếu cửa sổ ở hàng F là {2, 3} thì `c = 2,5`, `Center = 1 − 4/6 ≈ 0,333` và `Score ≈ 0,15 + 0,45 + 0,1 = 0,700`. Như vậy hàng F cho gợi ý đứng đầu, sau đó là hàng C và J.

### 4.6.3 Xếp lịch chiếu tự động

Bộ xếp lịch nhận danh sách phim cần xếp (số suất mỗi ngày của từng phim), danh sách phòng, khung giờ hoạt động và các khoảng bận hiện có, rồi đề xuất danh sách `SchedulerSlot` gồm phim, phòng, thời điểm bắt đầu và kết thúc. `ShowtimeService.PreviewAutoScheduleAsync` chuẩn bị dữ liệu (chỉ nhận phim chưa `Ended`, phòng đang hoạt động có ghế, số ngày tối đa `MaxAutoScheduleDays = 31`), gọi `ShowtimeScheduler.Build` lần lượt cho từng ngày từ `Date` đến `ToDate` và trả về bản xem trước kèm danh sách phim chưa xếp đủ suất (`Unplaced`); việc tạo suất thật được thực hiện ở bước sau. Sau mỗi ngày, các suất vừa xếp được thêm vào danh sách khoảng bận để ngày kế tiếp không đặt chồng lên.

#### 4.6.3.1 Độ nóng của phim

Độ nóng `Heat` đo mức độ được quan tâm của phim, dùng để ưu tiên xếp phim nóng trước và ghép phim nóng với phòng lớn. Theo `ShowtimeScheduler.Heat`, độ nóng kết hợp tỷ lệ vé bán của phim so với phim bán chạy nhất trong nhóm phim đang xếp và điểm đánh giá, được giới hạn trong khoảng [0, 1]:

```
Heat = min(1, max(0, 0,7 · Vé/VéMax + 0,3 · Rating/10))                                    (4.11)
```

Trong đó `Vé/VéMax` được lấy bằng 0 khi `VéMax = 0`. `Vé` là số `BookingTicket` thuộc booking `Confirmed` có `Booking.CreatedAt` trong 7 ngày gần nhất (tính từ thời điểm gọi), nhóm theo phim; `VéMax` là giá trị lớn nhất của số vé này trong số các phim được yêu cầu xếp. `Rating` là `Movie.AverageRating`. Cột `AverageRating` được khai báo `decimal(3,2)` với ràng buộc `CK_Movies_AverageRating` giới hạn 0–5 (`MovieConfiguration`); hệ quả của việc đặt cạnh phép chia cho 10 được thảo luận ở Mục 7.3.3.

#### 4.6.3.2 Ứng viên khung giờ

Với mỗi ngày, `OperatingHours.Window` đổi giờ mở cửa và đóng cửa của rạp (theo giờ Việt Nam) thành cặp thời điểm UTC `(open, close)`; nếu giờ đóng cửa không lớn hơn giờ mở cửa thì khung kéo sang ngày hôm sau (rạp hoạt động qua đêm), và nếu rạp không có giờ đóng cửa thì `close = null`. Khi đó cửa sổ làm việc kết thúc tại `windowEnd = open + DefaultWindowMinutes`.

Với mỗi phim cần `s` suất, thuật toán dựng trước `s` thời điểm lý tưởng chia đều trong khung. Gọi `latest` là thời điểm bắt đầu muộn nhất (`windowEnd − 1 phút` nếu cho phép chiếu ngoài giờ `AllowAfterHours`, ngược lại là `windowEnd − thời lượng phim`) và `span = latest − open` tính bằng phút. Thời điểm lý tưởng thứ `i` (`i = 0 … s − 1`) là điểm giữa của đoạn thứ `i` trong `s` đoạn bằng nhau, được làm tròn lên theo bước `StepMinutes`:

```
t_i = Up( open + span · (i + 0,5) / s, StepMinutes ),   i = 0, …, s − 1                    (4.12)
```

Nếu `span < 0` (phim dài hơn khung) hoặc `s ≤ 0` thì phim không có thời điểm lý tưởng. Hàm `Up` (và hàm `Down` tương ứng làm tròn xuống) làm tròn theo bước tính từ 0 giờ theo giờ Việt Nam của ngày chứa thời điểm đó (`LocalMidnight`), chứ không phải 0 giờ UTC, để các bước không chia hết cho 60 phút (ví dụ 45 phút) vẫn rơi đúng mốc giờ Việt Nam.

Với mỗi phòng, tập ứng viên thời điểm bắt đầu (`Candidates`) gồm: giờ mở cửa làm tròn lên; các thời điểm lý tưởng của phim; nếu có giờ đóng cửa thì `close − thời lượng` làm tròn xuống; và với mỗi khoảng bận `(Start, End)` của phòng thì `End` làm tròn lên (xếp ngay sau khoảng bận) cùng `Start − (thời lượng + đệm)` làm tròn xuống (xếp ngay trước khoảng bận). Một ứng viên chỉ được giữ khi thỏa đồng thời ba ràng buộc: (a) nằm trong giờ hoạt động, tức không sớm hơn `open` và, nếu có `close`, kết thúc phần chiếu không muộn hơn `close` (hoặc, khi `AllowAfterHours`, chỉ cần bắt đầu trước `close`); (b) không trùng khoảng bận nào của phòng với khối thời gian `block = thời lượng + RoomBufferMinutes` (điều kiện trùng giống Mục 4.6.4); (c) cách mọi suất cùng phim đã xếp trong ngày ít nhất `MinSameMovieGapMinutes` phút. Bảng 4.86 liệt kê các tham số của bộ xếp lịch.

*Bảng 4.86. Tham số bộ xếp lịch*

| Tham số | Ý nghĩa | Mặc định |
|---|---|---|
| `StepMinutes` | Bước làm tròn thời điểm bắt đầu, tính từ 0 giờ theo giờ Việt Nam | 5 |
| `MinSameMovieGapMinutes` | Khoảng cách tối thiểu giữa hai suất cùng phim trong cùng ngày | 60 |
| `DefaultWindowMinutes` | Độ dài cửa sổ làm việc khi rạp không có giờ đóng cửa | 960 (16 × 60) |
| `SpreadMinutes` | Hệ số chuẩn hóa khoảng cách tới mốc lý tưởng (phút) | 30 |
| `FitWeight` | Trọng số mức phù hợp giữa độ nóng và sức chứa phòng | 4 |
| `LoadWeight` | Trọng số phạt tải phòng (mỗi khoảng đã chiếm trong phòng) | 0,5 |
| `HeatLookbackDays` | Số ngày tính vé bán để tính độ nóng | 7 (khai báo trong cấu hình; mã hiện không đọc giá trị này, xem ghi chú bên dưới) |

*Nguồn: `Options/SchedulerOptions.cs` (mục cấu hình `Scheduler`, `appsettings.json` đặt cùng các giá trị).*

Hai tham số trong bảng chưa được hiện thực đầy đủ trong luồng gọi hiện tại. `PreviewAutoScheduleAsync` tính khoảng nhìn lại 7 ngày bằng hằng số (`DateTime.UtcNow.AddDays(-7)`) thay vì `HeatLookbackDays`, và gọi `ShowtimeScheduler.Build` không truyền `SchedulerOptions`, nên `Build` dùng giá trị mặc định ở Bảng 4.86 thay vì giá trị đọc từ cấu hình. Với cấu hình mặc định hiện tại hai cách cho kết quả như nhau.

#### 4.6.3.3 Hàm điểm xếp lịch

Với mỗi cặp (phòng, thời điểm bắt đầu) hợp lệ, hàm `Score` cộng ba thành phần. Thành phần thứ nhất phạt khoảng cách (phút) từ thời điểm bắt đầu tới mốc lý tưởng gần nhất còn lại của phim, chuẩn hóa bởi `SpreadMinutes`; khoảng cách bằng 0 khi phim không còn mốc lý tưởng. Thành phần thứ hai đo mức phù hợp giữa độ nóng của phim và sức chứa của phòng, trong đó `Ghế` là số ghế khả dụng của phòng (ghế đang hoạt động, không phải lối đi) và `GhếMax` là số ghế lớn nhất trong các phòng đang xét:

```
Fit = 1 − |Heat − Ghế/GhếMax|                                                              (4.13)
```

Thành phần thứ ba phạt tải của phòng, bằng số khoảng thời gian đã chiếm trong phòng đó nhân `LoadWeight`. Điểm xếp lịch là:

```
Score = −Khoảng cách / SpreadMinutes + Fit · FitWeight − Tải · LoadWeight                  (4.14)
```

Phim nóng (độ nóng gần 1) vì vậy được ưu tiên phòng lớn (tỷ lệ ghế gần 1), phim ít nóng được ưu tiên phòng nhỏ; thời điểm gần mốc lý tưởng được điểm cao hơn; và phòng đang ít suất được ưu tiên để dàn đều tải. Trong công thức (4.14), `Tải` được hiện thực là `occupied[room].Count`, tức số khoảng đã chiếm trong phòng, gồm cả các suất đã có sẵn lẫn các suất vừa xếp, chứ không chỉ riêng các suất vừa xếp.

![Hình 4.62. Lưu đồ xếp lịch chiếu tự động](hinh/4.62.png)

*Hình 4.62. Lưu đồ xếp lịch chiếu tự động*

Hình 4.62 mô tả hàm `Build` cho một ngày. Bước khởi tạo tính cửa sổ `(open, close)`, `maxSeats`, khoảng bận theo phòng, hàng đợi phim sắp theo độ nóng giảm dần (hòa thì theo `MovieId` tăng dần), số suất còn lại và các thời điểm lý tưởng của từng phim. Vòng ngoài lặp trong khi còn phim có số suất còn lại lớn hơn 0; trong mỗi lượt, thuật toán lần lượt xét từng phim còn suất theo thứ tự hàng đợi và xếp tối đa một suất cho phim đó. Với phim đang xét, thuật toán dựng ứng viên cho mọi phòng, lọc theo ba ràng buộc ở Mục 4.6.3.2 và chấm điểm theo (4.14). Nếu không còn ứng viên hợp lệ, số suất còn lại của phim được đặt về 0 (phim không xếp thêm được). Nếu có, thuật toán chọn phương án có điểm cao nhất (hòa thì chọn thời điểm sớm hơn, rồi phòng có `RoomId` nhỏ hơn), loại khỏi danh sách mốc lý tưởng mốc gần nhất với thời điểm đã chọn, ghi nhận khoảng `[Start, Start + block)` vào phòng, thêm vào danh sách kết quả và giảm số suất còn lại. Khi mọi phim đã xếp xong hoặc bị loại, danh sách kết quả được sắp theo thời điểm bắt đầu rồi theo `RoomId`. Cách xét luân phiên từng phim theo lượt giúp các phim chia đều các vị trí tốt thay vì phim đầu hàng đợi chiếm hết.

**Độ phức tạp.** Gọi `R` là số phòng, `K` là tổng số suất cần xếp trong một ngày (tổng `Shows` của các phim) và `B` là số khoảng bận ban đầu lớn nhất của một phòng. Vòng lặp xếp tối đa `K` suất, mỗi suất xét `R` phòng. Với một phòng, `Candidates` sinh `O(K + B)` điểm (mốc lý tưởng của phim tối đa `K`, hai điểm cho mỗi khoảng bận, số khoảng bận không vượt `B + K`); mỗi ứng viên được kiểm tra `IsFree` mất `O(B + K)` và kiểm tra khoảng cách cùng phim mất `O(K)`, còn `Score` mất `O(K)` để tìm mốc lý tưởng gần nhất. Chi phí xếp một suất vì thế là `O(R · (K + B)²)`, và chi phí cho một ngày là `O(K · R · (K + B)²)`; với `D` ngày là `O(D · K · R · (K + B)²)`, chưa kể chi phí sắp xếp danh sách phương án `O(R(K + B) log(R(K + B)))` mỗi lần xếp, vốn nhỏ hơn. Với quy mô rạp thực tế (vài phòng, vài chục suất mỗi ngày) chi phí này đủ nhỏ để chạy đồng bộ trong một yêu cầu xem trước.

### 4.6.4 Phát hiện trùng lịch phòng

Hai suất chiếu trong cùng phòng không được chồng thời gian. `ShowtimeService.HasOverlapAsync(roomId, startTime, endTime, excludeShowtimeId)` truy vấn xem phòng đã có suất thỏa đồng thời: cùng phòng, trạng thái `Scheduled`, `EndTime` lớn hơn thời điểm bắt đầu mới và `StartTime` nhỏ hơn thời điểm kết thúc mới. Khi sửa suất, tham số `excludeShowtimeId` loại chính suất đang sửa khỏi phép so sánh. `EnsureNoOverlapAsync` ném `ConflictAppException` với `Messages.Conflict_ShowtimeOverlapInRoom` khi phát hiện trùng. Suất đã hủy hoặc đã kết thúc không thuộc trạng thái `Scheduled` nên không chiếm phòng.

`EndTime` của suất chiếu được ghi bằng `StartTime + thời lượng phim + RoomBufferMinutes`, trong đó `RoomBufferMinutes` là đệm dọn phòng, mặc định 15 phút (`BookingOptions`, cũng đặt 15 trong `appsettings.json`). Điều kiện trùng vì vậy tính cả thời gian đệm: hai suất liên tiếp phải cách nhau tối thiểu bằng đệm. Ngược lại, việc kiểm tra giờ hoạt động chỉ dùng thời điểm kết thúc phần chiếu (không gồm đệm).

![Hình 4.63. Minh họa điều kiện trùng lịch phòng](hinh/4.63.png)

*Hình 4.63. Minh họa điều kiện trùng lịch phòng*

Hình 4.63 biểu diễn điều kiện trên trục thời gian: với suất đã có `[S₁, E₁)` và suất mới `[S₂, E₂)`, hai suất trùng khi `E₁ > S₂` và `S₁ < E₂`; trường hợp suất mới bắt đầu đúng bằng `E₁` (sau đệm) không trùng vì `E₁ > S₂` sai.

**Giờ mở và đóng cửa qua đêm.** Giờ hoạt động của rạp (`OpeningTime`, `ClosingTime` trong `CinemaSettings`, cùng có thể để trống) được kiểm tra bởi `OperatingHours.Violation`. Hàm không kiểm tra gì khi cả hai giá trị đều trống. Nếu có giờ đóng cửa, hàm xét hai khung: khung bắt đầu ngày của `start` (theo giờ Việt Nam) và khung bắt đầu ngày hôm trước, vì một khung mở qua đêm của ngày hôm trước có thể còn bao trùm `start`. Với mỗi khung `(open, close)` từ `Window`, suất hợp lệ khi `start ≥ open` và, nếu có `close`, kết thúc phần chiếu không muộn hơn `close` (hoặc, khi `AllowAfterHours` bật, chỉ cần bắt đầu trước `close`). Nếu không khung nào thỏa, hàm trả về thông báo nêu khoảng giờ của suất và khung giờ hoạt động của rạp, và `ValidateOperatingHoursAsync` chuyển thành `BusinessRuleException`.

### 4.6.5 Xử lý múi giờ

Hệ thống lưu mọi thời điểm dưới dạng UTC và chỉ chuyển sang giờ Việt Nam ở các điểm cần hiển thị hoặc cần xác định ranh giới ngày. Lớp `VietnamTime` là điểm duy nhất của máy chủ thực hiện việc chuyển đổi này. Múi giờ nghiệp vụ lấy từ cấu hình `Brand:TimeZoneId` (mặc định `Asia/Ho_Chi_Minh`) và được nạp một lần khi khởi động bằng `VietnamTime.Configure` trong `Program.cs`; giá trị rỗng hoặc không hỗ trợ trên hệ điều hành làm ứng dụng không khởi động (`InvalidOperationException`), và hàm chấp nhận cả mã IANA lẫn mã Windows (ví dụ `SE Asia Standard Time`). Các hàm chính gồm `ToUtc` (đổi giờ địa phương, ngày và giờ sang UTC), `ToLocal`, `LocalDate`, `Today`, `StartOfDayUtc` và `StartOfNextDayUtc`. Hai hàm cuối cho phép lọc và báo cáo theo ngày theo giờ Việt Nam bằng khoảng nửa mở `[StartOfDayUtc(d), StartOfNextDayUtc(d))` trên cột UTC; ví dụ bộ lọc `FromDate`, `ToDate` của danh sách booking quản trị dùng đúng khoảng này. Các công việc định kỳ Hangfire cũng được đăng ký với `TimeZone = VietnamTime.Zone`.

Ở biên API, `DateTimeJsonConverter` (đăng ký trong `Program.cs` cùng `DateOnlyJsonConverter`) chuẩn hóa mọi giá trị về UTC. Khi đọc, giá trị có ký hiệu múi giờ (hậu tố `Z` hoặc độ lệch `±hh:mm`) được hiểu đúng thời điểm đó; giá trị không có ký hiệu múi giờ, và các định dạng `dd/MM/yyyy HH:mm:ss`, `dd/MM/yyyy HH:mm`, `dd/MM/yyyy`, được coi là giờ Việt Nam rồi đổi sang UTC bằng `VietnamTime.ToUtc`. Khi ghi, giá trị luôn được xuất dạng `yyyy-MM-ddTHH:mm:ss.fffZ`. `DateOnlyJsonConverter` ghi `yyyy-MM-dd` và đọc thêm `dd/MM/yyyy`.

Ở giao diện, `business-time.ts` lấy múi giờ từ `VITE_BUSINESS_TIME_ZONE` (mặc định `Asia/Ho_Chi_Minh`). Hàm `toWallDate` chuyển chuỗi có ký hiệu múi giờ thành một đối tượng `Date` có các trường giờ địa phương bằng giờ treo tường của múi giờ nghiệp vụ (`instantToBusinessWall`); `toBusinessIso` gắn độ lệch múi giờ nghiệp vụ vào chuỗi ngày giờ chưa có ký hiệu trước khi gửi lên máy chủ; `getBusinessToday` trả về ngày hiện tại theo múi giờ nghiệp vụ. `wall-time.ts` chỉ thao tác trên các trường giờ phút của đối tượng `Date` (`combineDateAndTime`, `toTimeValue`) để ghép ngày và giờ trong biểu mẫu, không chuyển múi giờ.

*Bảng 4.87. Quy ước thời gian*

| Tầng | Quy ước | Thành phần hiện thực |
|---|---|---|
| CSDL | UTC | Giá trị ghi bằng `DateTime.UtcNow` hoặc đã chuẩn hóa bằng `VietnamTime.ToUtc` |
| API (nhận) | Chuỗi có `Z`/`±hh:mm` giữ nguyên thời điểm; chuỗi không ký hiệu múi giờ và định dạng `dd/MM/yyyy…` coi là giờ Việt Nam rồi đổi sang UTC | `DateTimeJsonConverter.Read` |
| API (trả) | Luôn là UTC, định dạng `yyyy-MM-ddTHH:mm:ss.fffZ`; `DateOnly` là `yyyy-MM-dd` | `DateTimeJsonConverter.Write`, `DateOnlyJsonConverter.Write` |
| Nghiệp vụ máy chủ | Múi giờ `Brand:TimeZoneId` (mặc định `Asia/Ho_Chi_Minh`); ranh giới ngày là khoảng `[StartOfDayUtc, StartOfNextDayUtc)` | `VietnamTime`, `OperatingHours`, `ShowtimeScheduler` (làm tròn từ 0 giờ Việt Nam) |
| Giao diện | Giờ Việt Nam (múi giờ `VITE_BUSINESS_TIME_ZONE`, mặc định `Asia/Ho_Chi_Minh`) | `business-time.ts`, `wall-time.ts` |

*Nguồn: `Utils/VietnamTime.cs`, `Utils/JsonDateTimeConverters.cs`, `Cinema.Api/Program.cs`, `frontend/src/lib/business-time.ts`, `frontend/src/lib/wall-time.ts`.*

<!-- GHI CHÚ ĐỐI CHIẾU -->
<!--
(a) [CẦN BỔ SUNG] còn lại
1. Hình 4.60, 4.61, 4.62, 4.63: Đã mô tả đầy đủ thuật toán, các bước và công thức trực tiếp từ mã nguồn `SeatLayoutRules.cs`, `ShowtimeScheduler.cs`, `OperatingHours.cs`; chèn file ảnh kết xuất vào thư mục `hinh/` khi in ấn.
2. Không có trích dẫn tài liệu tham khảo trong phạm vi này.

(b) Chỗ outline lệch code
1. 4.6.3.1 / Bảng 4.86, `HeatLookbackDays`: outline ghi Heat tính vé bán trong `HeatLookbackDays` ngày; code (`ShowtimeService.PreviewAutoScheduleAsync`) dùng hằng `DateTime.UtcNow.AddDays(-7)`, không đọc `HeatLookbackDays` (tham số này chỉ khai báo trong `SchedulerOptions`, không nơi nào đọc). Giá trị mặc định cùng bằng 7 nên kết quả hiện chưa lệch.
2. 4.6.3.2, `SchedulerOptions`: `ShowtimeService` có trường `_scheduler` nhưng không dùng; `Build(...)` được gọi không truyền `schedulerOptions`, nên mục cấu hình `Scheduler` (`appsettings.json`) bị bỏ qua và `Build` luôn dùng `new SchedulerOptions()` (mặc định). Giá trị trong `appsettings.json` hiện trùng mặc định.
3. 4.6.3.1, công thức (4.11): outline ghi "vé bán tối đa trong HeatLookbackDays ngày"; code lấy `VéMax` là số vé lớn nhất giữa các phim được yêu cầu xếp trong lần gọi, không phải toàn rạp. Rating dùng `Movie.AverageRating` (CK 0–5) chia 10 — trình bày đúng như code, hạn chế xem 7.3.3. Outline viết `min(1, …)`; code dùng `Math.Clamp(…, 0, 1)` (tương đương vì biểu thức không âm).
4. 4.6.3.3, `Tải` trong (4.14): outline ghi "số suất đã xếp trong phòng"; code dùng `occupied[room].Count`, gồm cả khoảng bận có sẵn của phòng (suất cũ trong phạm vi nạp, `busy`) và các suất vừa xếp trong các ngày trước của cùng lần xem trước.
5. 4.6.3.3, `GhếMax` trong (4.13): là số ghế lớn nhất trong các phòng tham gia xếp lịch (`maxSeats`), không phải toàn rạp; `Ghế` là số ghế đang hoạt động không phải lối đi (`PreviewAutoScheduleAsync`).
6. 4.6.3.3, độ phức tạp: dòng `[CẦN BỔ SUNG: nêu biến n, m, k]` đã được thay bằng các biến `R`, `K`, `B`, `D` tự phân tích từ vòng lặp; đây là phân tích của tác giả, không có đo đạc thực nghiệm.
7. 4.6.2.1: điều kiện loại cửa sổ để lại 1 ghế (`i == 1` hoặc `L − i − count == 1`) áp dụng bất kể `SeatGapRuleMode` (kể cả `Off`); gợi ý ghế không đọc chế độ cấu hình.
8. 4.6.1, Bảng 4.85: outline ghi `Off` chỉ áp dụng "Bán vé quầy"; code còn cho phép chọn `Off` trong cấu hình rạp (`SettingsService`) áp dụng cho đặt vé trực tuyến. Thông báo lỗi ghế đôi và ghế lẻ là chuỗi cố định trong `SeatLayoutRules.cs`, không lấy từ `Messages`.
9. 4.6.4: outline ghi đệm 15 phút; code là `BookingOptions.RoomBufferMinutes` (mặc định 15, cấu hình được). Kiểm tra giờ hoạt động dùng `StartTime + thời lượng` (không đệm), còn trùng phòng dùng `EndTime` có đệm.
10. 4.6.5: dòng `[CẦN BỔ SUNG: đọc JsonDateTimeConverters]` của Bảng 4.87 đã giải quyết bằng mã. Hàng "Nghiệp vụ máy chủ" và việc tách "API" thành hai dòng (nhận/trả) là bổ sung của tác giả so với khung outline; không bỏ dòng nào của khung.
11. Bảng 4.86: bổ sung dòng `DefaultWindowMinutes` (có trong `SchedulerOptions`, không có trong khung outline).
-->

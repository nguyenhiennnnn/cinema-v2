## 5.4 CÀI ĐẶT FRONTEND

Phần giao diện được xây dựng bằng React với bộ định tuyến React Router. Mã nguồn của khách hàng nằm trong `frontend/src/features/client/**` và `frontend/src/features/auth/**`; các thành phần dùng chung nằm trong `frontend/src/components/common/**`. Mỗi tính năng tổ chức theo cùng một khuôn gồm `api/` (các hàm gọi HTTP), `hooks/` (lớp bao TanStack Query), `components/` và `pages/`. Các hàm trong `api/` gọi máy chủ qua `apiGet`, `apiPost`, `apiPut` của `frontend/src/api/axios.ts`; kiểu phản hồi chung là `ApiResponse<T>`, và dữ liệu phân trang theo con trỏ là `CursorPageResult<T>`. Đường dẫn trong các hàm này là đường dẫn tương đối so với tiền tố `/api/v1` của máy chủ, chẳng hạn `/movies` tương ứng `GET /api/v1/movies`. Cách đặt tên hook thống nhất: truy vấn đọc dùng `useQuery` hoặc `useInfiniteQuery`, thao tác ghi dùng `useMutation` kèm `meta: { invalidates: ... }` để làm mới các truy vấn liên quan (định nghĩa trong `frontend/src/api/query-invalidation.ts`).

Bộ định tuyến của khách hàng được khai báo trong `frontend/src/app/routes/client-routes.tsx`. Các trang công khai (`/`, `/movies`, `/movies/:id`, `/schedule`, `/membership`, `/booking/:showtimeId`, `/booking/result`) nằm dưới `ClientLayout`; các trang tài khoản (`/account/profile`, `/account/tickets`, `/account/reviews`, `/account/change-password`, `/account/tickets/:id`) được bọc bởi `CustomerProtectedRoute`; các trang xác thực (`/auth/login`, `/auth/register`, `/auth/confirm-email`, `/auth/forgot-password`, `/auth/reset-password`) được bọc bởi `GuestRoute` và dùng `AuthLayout`.

### 5.4.1 Giao diện khách hàng

Mười bốn màn hình dưới đây là các màn hình mà khách hàng sử dụng trong luồng từ khám phá phim, đặt vé, thanh toán đến quản lý vé và tài khoản. Mỗi mục mô tả theo thứ tự: mục đích, thành phần giao diện, tương tác và trạng thái, cuối cùng là component, hook và điểm cuối API được sử dụng.

#### 5.4.1.1 Màn hình Trang chủ

Trang chủ là điểm vào của hệ thống, giúp khách hàng nhìn nhanh các phim đang chiếu, phim sắp chiếu, combo bắp nước và hai điểm nhấn về trải nghiệm rạp. Trang được dựng bởi `HomePage` (`features/client/home/pages/home-page.tsx`) và đồng thời khai báo thẻ SEO cùng dữ liệu có cấu trúc `WebSite` với hành động tìm kiếm trỏ tới `/movies?search=`.

Giao diện gồm bốn khối xếp dọc. Khối đầu là băng chuyền phim `HeroCarousel`, hiển thị toàn chiều rộng: nền poster mờ (`HeroBackdrop`), nội dung của phim hiện tại (`HeroSlide`) với nhãn độ tuổi, tối đa ba thể loại, thời lượng, điểm đánh giá trung bình khi lớn hơn 0 và hai nút "Đặt vé", "Chi tiết phim" cùng dẫn tới `/movies/{movieId}`; phía dưới là cụm điều khiển `HeroControls` (phim trước, phim tiếp theo, tạm dừng/tiếp tục, chỉ số `index + 1 / count`) và dải ảnh thu nhỏ `HeroThumbnails` của các phim còn lại. Khối thứ hai và thứ ba là hai `MovieSection` "Phim đang chiếu" và "Phim sắp chiếu", mỗi khối là lưới thẻ `MovieCard` kèm liên kết "Xem tất cả" tới `/movies?status=NowShowing` hoặc `/movies?status=ComingSoon`. Khối thứ tư là `ComboCarousel` "Combo bắp nước bán chạy", một dải cuộn ngang các thẻ combo gồm ảnh, tên, mô tả và giá. Cuối cùng là `CinemaExperienceSection` gồm hai thẻ tĩnh "IMAX Laser" và "Dolby Atmos".

Về tương tác, băng chuyền tự chuyển phim sau mỗi `HERO_INTERVAL_MS` (7000 ms); thanh tiến trình `hero-progress` ở mép dưới kết thúc hoạt ảnh thì gọi chuyển phim tiếp theo. Việc tự chuyển bị tạm dừng khi người dùng bấm nút tạm dừng (`userPaused`), khi chuột đặt lên băng chuyền, khi phần tử bên trong nhận tiêu điểm bàn phím hoặc khi thẻ trình duyệt bị ẩn (`document.hidden`); được tắt hẳn khi hệ điều hành bật `prefers-reduced-motion` hoặc chỉ có một phim (`canAutoplay = count > 1 && !reducedMotion`). Người dùng có thể chuyển phim bằng phím mũi tên trái/phải, vuốt ngang trên màn hình cảm ứng (ngưỡng 50 px) hoặc bấm một ảnh thu nhỏ; khi bấm ảnh thu nhỏ, hook `useHeroTransition` đo vị trí ảnh đó và chạy hoạt ảnh `clip-path` để poster mở rộng thành nền trong `HERO_TRANSITION_MS` (480 ms), còn `useHeroCarousel` chặn yêu cầu chuyển mới khi hoạt ảnh trước chưa kết thúc. Hook `usePreloadPosters` nạp trước các poster để tránh nhấp nháy. Về trạng thái, băng chuyền hiển thị `Skeleton` khi đang tải và không hiển thị gì khi không có phim; `MovieSection` hiển thị năm `MovieCardSkeleton` khi tải và `EmptyState` với thông điệp "Chưa có phim đang chiếu" hoặc "Chưa có phim sắp chiếu" khi danh sách rỗng; `ComboCarousel` không hiển thị gì nếu không có combo đang bán.

Trang dùng các hook `useMovies` (`features/client/movies/hooks/use-movies.ts`) và `useActiveCombos` (`features/client/combos/hooks/use-combos.ts`). `HeroCarousel` gọi `useMovies({ status: MovieStatus.NowShowing, limit: 5 })`, mỗi `MovieSection` gọi `useMovies({ status, limit: 10 })`; cả hai đều gửi `GET /movies` với các tham số `status`, `limit`, `cursor`. `ComboCarousel` gọi `GET /combos`.

![Hình 5.2 Màn hình Trang chủ](hinh/5.2.png)

*Hình 5.2. Màn hình Trang chủ* `[CẦN BỔ SUNG: ảnh chụp màn hình Trang chủ]`

#### 5.4.1.2 Màn hình Danh sách phim

Màn hình này cho phép khách hàng duyệt toàn bộ phim đang chiếu và sắp chiếu, thu hẹp danh sách bằng từ khóa và các bộ lọc. Trang được dựng bởi `MoviesPage` (`features/client/movies/pages/movies-page.tsx`), tuyến `/movies`.

Giao diện gồm tiêu đề trang, thanh lọc `MoviesFilterBar` và lưới kết quả `MoviesGrid`. Thanh lọc có ô tìm kiếm với gợi ý "Tìm tên phim, diễn viên...", ba nút trạng thái "Tất cả", "Đang chiếu", "Sắp chiếu" (giá trị `null`, `MovieStatus.NowShowing`, `MovieStatus.ComingSoon`), ba ô chọn `SelectField` cho thể loại, ngôn ngữ và độ tuổi, và nút "Xóa bộ lọc" chỉ hiện khi có ít nhất một bộ lọc đang bật. Lưới kết quả hiển thị các `MovieCard`; mỗi thẻ gồm poster, nhãn độ tuổi trên ảnh, tên phim, tối đa hai thể loại kèm thời lượng và điểm đánh giá trung bình; phim đang chiếu có nút "Mua vé" dẫn tới `/movies/{movieId}#showtimes`, phim sắp chiếu hiển thị dòng "Khởi chiếu" cùng ngày phát hành.

Các giá trị lọc được lưu trong chuỗi truy vấn của URL thông qua `useClientMovieParams` (dựng trên `useQueryStates` của thư viện `nuqs`), gồm `search`, `status`, `genreId`, `languageId`, `ageRatingId`, `limit` và `cursor`; nhờ vậy một kết quả lọc có thể chia sẻ bằng đường dẫn. Mỗi lần đổi bộ lọc hoặc từ khóa, trang đặt `cursor` về `null`. Từ khóa đi qua `useDebouncedValue` với độ trễ `SEARCH_DEBOUNCE_MS` (300 ms) trước khi gửi yêu cầu, và `limit` mặc định là `DEFAULT_MOVIES_PAGE_LIMIT` (12). Việc phân trang theo con trỏ được thực hiện dưới dạng cuộn vô hạn: `useMovies` dùng `useInfiniteQuery`, trang kế tiếp được lấy theo `nextCursor` khi `hasMore` là đúng, và `InfiniteScrollTrigger` (dựa trên `IntersectionObserver` trong `useInfiniteScroll`, `rootMargin` mặc định `200px`) gọi `fetchNextPage` khi phần tử kích hoạt tiến gần vùng nhìn thấy. Khi đang tải lần đầu, lưới hiển thị năm `MovieCardSkeleton`; khi không có kết quả, `EmptyState` hiển thị "Không tìm thấy phim phù hợp" kèm nút "Xóa bộ lọc".

Dữ liệu cho ba ô chọn lấy từ `useGenres`, `useLanguages`, `useAgeRatings` (`features/client/catalog/hooks/use-catalog.ts`), lần lượt gọi `GET /genres`, `GET /languages`, `GET /age-ratings`. Danh sách phim gọi `GET /movies` với `search`, `status`, `genreId`, `languageId`, `ageRatingId`, `cursor`, `limit` qua `moviesApi.getMovies`.

![Hình 5.3 Màn hình Danh sách phim](hinh/5.3.png)

*Hình 5.3. Màn hình Danh sách phim* `[CẦN BỔ SUNG: ảnh chụp màn hình Danh sách phim]`

#### 5.4.1.3 Màn hình Chi tiết phim

Màn hình cung cấp đầy đủ thông tin của một phim, cho phép xem trailer, đọc và viết đánh giá, đồng thời là nơi khách hàng chọn suất chiếu để bắt đầu đặt vé. Trang được dựng bởi `MovieDetailPage` (`features/client/movies/pages/movie-detail-page.tsx`), tuyến `/movies/:id`.

Phần đầu trang là `MovieHero`: poster, nhãn độ tuổi, danh sách thể loại, tên phim, tên gốc (nếu có), thời lượng, ngôn ngữ, ngày phát hành và điểm trung bình dạng `x.x/5.0` khi lớn hơn 0, cùng nút "Đặt vé" và, khi phim có `trailerUrl`, nút "Xem trailer". Bên dưới là phần "Nội dung phim" dùng `ExpandableText`, khối `MovieCastCrew` gồm đạo diễn và dải cuộn ngang diễn viên (ảnh, họ tên, tên nhân vật), khối "Lịch chiếu & Đặt vé" do `MovieShowtimeSelector` dựng và khối "Đánh giá & Bình luận" do `MovieReviews` dựng. Trang cũng khai báo thẻ SEO và dữ liệu có cấu trúc kiểu `Movie` (đạo diễn, diễn viên, thể loại, thời lượng dạng `PT{n}M`).

Về tương tác, nút "Đặt vé" cuộn mượt tới phần tử `#showtimes`; việc cuộn cũng xảy ra khi URL có đoạn `#showtimes`, như khi đến từ nút "Mua vé" của `MovieCard`. Nút "Xem trailer" mở `TrailerDialog`, hộp thoại chứa `iframe` YouTube; địa chỉ được `toEmbedUrl` chuyển sang dạng `https://www.youtube.com/embed/{id}?autoplay=1` và `iframe` chỉ được gắn khi hộp thoại mở. Trong `MovieShowtimeSelector`, dải ngày `DateStrip` chỉ cho chọn các ngày có suất chiếu (`availableDates`); mặc định chọn ngày sớm nhất có suất, và danh sách suất của ngày đã chọn được `ShowtimeGroupList` gom theo định dạng chiếu (`screenType`, mặc định "2D"). Mỗi suất là một nút hiển thị khoảng giờ, tên phòng, số ghế còn trống trên tổng số ghế và giá cơ bản; suất hết ghế (`availableSeats === 0`) bị vô hiệu hóa. Khi bấm một suất, người dùng đã đăng nhập được chuyển tới `/booking/{showtimeId}`; người dùng chưa đăng nhập thấy hộp thoại "Yêu cầu đăng nhập" và nếu đồng ý sẽ được chuyển tới `/auth/login?redirect=/booking/{showtimeId}`. Trong `MovieReviews`, biểu mẫu `ReviewForm` (chấm sao bằng `RatingStars` và ô nhận xét) chỉ hiện khi người dùng đã đăng nhập và `canReview` là đúng; nút "Gửi đánh giá" bị vô hiệu hóa khi chưa chấm sao. Chủ sở hữu đánh giá thấy nút "Sửa" và "Xóa" trên đánh giá của mình; "Sửa" mở lại biểu mẫu với nhãn "Cập nhật". Danh sách đánh giá cuộn vô hạn bằng `InfiniteScrollTrigger`. Khi chưa có suất hoặc chưa có đánh giá, trang hiển thị `EmptyState` tương ứng; khi đang tải, trang hiển thị hai `Skeleton`.

Các hook dùng trong trang gồm `useMovieDetail`, `useMovieReviews`, `useCanReviewStatus`, `useCreateReview`, `useUpdateReview`, `useDeleteMyReview` (đều trong `use-movies.ts`; các thao tác ghi dùng `meta: { invalidates: "reviewChanged" }`). Các điểm cuối: `GET /movies/{movieId}` (kèm `upcomingShowtimes`), `GET /movies/{movieId}/reviews` (phân trang con trỏ), `GET /movies/{movieId}/can-review` (chỉ gọi khi đã đăng nhập), `POST /movies/{movieId}/reviews`, `PUT /movies/{movieId}/reviews/{reviewId}`, `DELETE /movies/{movieId}/reviews/{reviewId}`.

![Hình 5.4 Màn hình Chi tiết phim](hinh/5.4.png)

*Hình 5.4. Màn hình Chi tiết phim* `[CẦN BỔ SUNG: ảnh chụp màn hình Chi tiết phim]`

#### 5.4.1.4 Màn hình Lịch chiếu

Màn hình cho phép tra cứu lịch chiếu của toàn rạp theo từng ngày, bất kể phim, để khách hàng chọn nhanh suất phù hợp. Trang được dựng bởi `SchedulePage` (`features/client/schedule/pages/schedule-page.tsx`), tuyến `/schedule`.

Giao diện gồm tiêu đề, dải ngày `DateStrip`, thanh lọc định dạng `ScheduleFilterBar` và danh sách các dòng phim `ScheduleMovieRow`. `DateStrip` hiển thị 14 ngày liên tiếp bắt đầu từ ngày hiện tại theo múi giờ nghiệp vụ (`getBusinessTodayDate`), mỗi ô gồm số ngày, thứ trong tuần và nhãn tháng ở ô đầu hoặc ngày mồng một; ngày hôm nay có gạch chân; hai nút mũi tên cuộn dải ngày trên màn hình từ cỡ `md` trở lên. `ScheduleFilterBar` gồm nút "Tất cả định dạng" và một nút cho mỗi định dạng có trong dữ liệu của ngày đã chọn, và không hiển thị khi danh sách định dạng rỗng. Mỗi `ScheduleMovieRow` gồm poster, tên phim (cùng liên kết tới `/movies/{movieId}`) và các suất của phim đó được `ShowtimeGroupList` nhóm theo định dạng, cùng cách hiển thị và xử lý bấm suất như ở Mục 5.4.1.3.

Ngày và định dạng được lưu trên URL qua `useScheduleParams` (`date`, `format`); nếu `date` trống, trang dùng `getBusinessToday()`. Khi đổi ngày, `format` được đặt lại `null`. Khi chuyển ngày hoặc định dạng, `useSchedule` dùng `placeholderData: keepPreviousData` nên danh sách cũ vẫn hiển thị cho tới khi dữ liệu mới về. Trạng thái tải hiển thị bốn `Skeleton`; khi không có phim nào chiếu trong ngày, `EmptyState` hiển thị "Không có suất chiếu".

Trang dùng `useSchedule(date, screenType)` (`features/client/showtimes/hooks/use-showtimes.ts`), gọi `showtimesApi.getSchedule` tức `GET /showtimes` với tham số `date` (định dạng `yyyy-MM-dd`, do `toDateKey` sinh ra và máy chủ nhận dưới dạng `DateOnly`) và `screenType` (nếu có). Phản hồi `WholeCinemaScheduleDto` gồm `date`, `movies` (mỗi phim kèm `showtimes`) và `formats`.

![Hình 5.5 Màn hình Lịch chiếu](hinh/5.5.png)

*Hình 5.5. Màn hình Lịch chiếu* `[CẦN BỔ SUNG: ảnh chụp màn hình Lịch chiếu]`

#### 5.4.1.5 Màn hình Chọn ghế

Đây là bước đầu của luồng đặt vé, nơi khách hàng chọn ghế trên sơ đồ phòng, chọn combo, nhập thông tin nhận vé và yêu cầu hệ thống giữ ghế. Trang `BookingPage` (`features/client/bookings/pages/booking-page.tsx`), tuyến `/booking/:showtimeId`, dùng chung cho cả bước chọn ghế và bước thanh toán; toàn bộ trạng thái luồng nằm trong hook `useBookingFlow`, với kiểu `BookingStep = "select" | "payment"`. Phần này mô tả bước `select`; bước `payment` ở Mục 5.4.1.6.

Đầu trang có `BookingShowtimeHeader` (tên phim, phòng và thời điểm bắt đầu) và `BookingStepper` ba bước "Chọn ghế", "Thanh toán", "Vé". Thân trang là `BookingSelectionStep`, chia hai cột trên màn hình rộng. Cột chính gồm bốn khối: `SeatSuggestionPanel` "Gợi ý ghế liền kề", sơ đồ `SelectableSeatMap`, khối "Chọn combo (tuỳ chọn)" dùng `ComboPicker` và biểu mẫu `BookingCustomerForm` "Thông tin nhận vé" (họ tên, số điện thoại). Sơ đồ nằm trong `SeatMapFrame`, phía trên có chú giải `SeatLegend` gồm sáu mục "Trống", "VIP", "Ghế đôi", "Đang chọn", "Đang giữ", "Đã bán"; mỗi ô ghế là `SeatButton`, lối đi hiển thị ở trạng thái `aisle`, ghế đôi chiếm hai ô và nhãn dạng `c-(c+1)`. Cột phụ gồm `BookingPriceSummary` "Chi tiết đơn hàng", nút "Giữ ghế & tiếp tục thanh toán", dòng chú thích "Ghế được giữ trong {holdDurationMinutes} phút để bạn hoàn tất thanh toán." và các cảnh báo. Trên màn hình hẹp, cột phụ được thay bằng thanh cố định ở đáy hiển thị số ghế đã chọn, thời gian giữ ghế, tổng tiền và nút "Giữ ghế".

Về tương tác, `toggleSeat` chọn hoặc bỏ chọn ghế; nếu ghế thuộc cặp (`coupleId`) thì cả hai ghế được chọn cùng lúc, và thao tác bị bỏ qua nếu bất kỳ ghế nào trong nhóm không còn ở trạng thái `Available`. `SeatSuggestionPanel` cho phép chọn số người (từ 1 đến 10, mặc định 2) và bấm "Gợi ý"; các phương án trả về được lọc lại theo trạng thái ghế hiện tại, mỗi phương án hiển thị dải ghế, số ghế và tổng giá, di chuột hoặc đặt tiêu điểm lên phương án thì các ghế tương ứng được đánh dấu `suggested` trên sơ đồ, bấm vào thì `onSelectSeats` thay toàn bộ lựa chọn hiện tại. Nếu không còn dãy phù hợp, panel báo "Không còn dãy {n} ghế liền kề phù hợp". Combo được sắp xếp theo thứ tự gợi ý của `useComboSuggestions` (số ghế đã chọn làm đầu vào) và nhãn gợi ý hiển thị bằng `Badge`. Trạng thái ghế được cập nhật theo thời gian thực: `useRealtimeSeatMap` dựng bản đồ trạng thái từ dữ liệu ban đầu và ghi đè theo các sự kiện SignalR `seat.held`, `seat.booked`, `seat.released`, `hold.expired` do `useShowtimeHub` nhận từ hub `/hubs/showtimes` (sau khi `JoinShowtime(showtimeId)`; rời phòng bằng `LeaveShowtime`). Quy tắc chống ghế lẻ do `findLoneSeats` kiểm tra theo `seatGapRuleMode` của cấu hình rạp (`Strict`, `AllowEdge`, `Off`); khi có ghế lẻ, giao diện hiển thị "Không thể để trống ghế lẻ …" và nút giữ ghế bị vô hiệu hóa. Nút này cũng bị vô hiệu hóa khi chưa chọn ghế, khi họ tên (đối với người đã đăng nhập) trống hoặc số điện thoại khác rỗng nhưng không hợp lệ theo `isValidVnPhone`; trường họ tên được điền sẵn và khóa với người đã đăng nhập. Bấm nút khi chưa đăng nhập sẽ mở hộp thoại yêu cầu đăng nhập kèm đường dẫn quay lại; khi đã đăng nhập, `holdAndContinue` gọi `useHoldSeats` rồi chuyển sang bước `payment` với kết quả `HoldBookingResponse`. Trong lúc tải, khu vực chọn ghế hiển thị `Skeleton`.

Các API được dùng: `GET /showtimes/{showtimeId}/seats` (`useShowtimeSeats`), `GET /showtimes/{showtimeId}/seat-suggestions?count=` (`useSeatSuggestions`, `staleTime: 0`, `gcTime: 0`), `GET /combos` và `GET /combos/suggestions?showtimeId=&seatCount=` (`useActiveCombos`, `useComboSuggestions`), `GET /settings` (`useCinemaSettings`, lấy `holdDurationMinutes`, `seatGapRuleMode`) và `POST /bookings/hold` (`useHoldSeats`, thân yêu cầu gồm `showtimeId`, `seatIds`, `combos`, `customerName`, `customerPhone`).

![Hình 5.6 Màn hình Chọn ghế](hinh/5.6.png)

*Hình 5.6. Màn hình Chọn ghế* `[CẦN BỔ SUNG: ảnh chụp màn hình Chọn ghế]`

#### 5.4.1.6 Màn hình Thanh toán

Sau khi giữ ghế thành công, khách hàng xem lại chi tiết đơn, chọn phương thức thanh toán và được chuyển sang cổng thanh toán. Màn hình là bước `payment` của `BookingPage`, hiển thị bởi `BookingPaymentStep`, cùng tuyến `/booking/:showtimeId`.

Giao diện là một cột hẹp gồm: bộ đếm `BookingHoldTimer` dạng dải cố định phía trên ("Giữ ghế còn m:ss"), `BookingPriceSummary` "Chi tiết đơn hàng", khối `BookingPaymentMethod` "Phương thức thanh toán" và hai nút "Thanh toán" và "Huỷ, chọn lại ghế". Khối phương thức gồm ba lựa chọn: VNPay, MoMo và "Thẻ quốc tế (Stripe)", tương ứng `PaymentMethod.VNPay`, `PaymentMethod.Momo`, `PaymentMethod.Stripe`; mặc định chọn VNPay. `BookingPriceSummary` ở bước này nhận `hold` nên hiển thị tiền ghế, tiền combo (tính bằng `totalAmount` trừ tiền ghế), dòng "Giảm giá hạng thành viên (áp dụng cho vé)" khi `discountAmount > 0`, và "Tổng cộng" là `finalAmount`.

Đếm ngược dùng `useCountdown(expiresAt)` cập nhật mỗi giây; khi còn dưới 60 giây, chữ chuyển sang màu cảnh báo và có thông báo cho trình đọc màn hình, đồng thời chú giải công cụ giải thích rằng ghế sẽ tự động được giải phóng nếu chưa thanh toán. Việc hết hạn giữ ghế được phát hiện theo hai đường: sự kiện `hold.expired` của SignalR cho đúng `bookingId` của mình, và cơ chế dự phòng trong `useBookingFlow` khi đếm ngược chạm 0 thì cứ 5 giây hỏi lại `GET /bookings/{bookingId}` cho tới khi trạng thái là `Expired`; khi đó giao diện hiển thị thông báo "Thời gian giữ ghế đã hết hạn, vui lòng chọn lại." và quay về bước chọn ghế. Khi bấm "Thanh toán", `submitPayment` gọi `useCheckout` với `paymentMethod`; nếu thành công, trình duyệt được chuyển (`window.location.href`) tới `paymentUrl` trả về. Nút "Huỷ, chọn lại ghế" gọi `useCancelBooking` rồi đặt lại luồng. Trong lúc chờ, nút hiển thị "Đang chuyển đến cổng thanh toán..." hoặc "Đang huỷ...".

Màn hình cũng phục vụ chế độ tiếp tục thanh toán khi URL có tham số `resume={bookingId}` (từ Mục 5.4.1.9): `useBookingFlow` mở thẳng bước `payment`, nạp đơn bằng `useBookingDetail` và dựng lại `holdResult` từ `holdExpiresAt`. Nếu đơn không còn ở trạng thái `Pending`, đã hết hạn hoặc không tải được, trang hiển thị hộp thông báo với lý do tương ứng (`not-pending`, `expired`, `load-error`) và hai nút "Xem vé của tôi", "Đặt vé mới".

Các API: `POST /bookings/{bookingId}/checkout` (thân `{ paymentMethod }`, phản hồi `paymentUrl`, `paymentId`), `POST /bookings/{bookingId}/cancel`, `GET /bookings/{bookingId}`; các thao tác ghi dùng `meta: { invalidates: "bookingChanged" }`.

![Hình 5.7 Màn hình Thanh toán](hinh/5.7.png)

*Hình 5.7. Màn hình Thanh toán* `[CẦN BỔ SUNG: ảnh chụp màn hình Thanh toán]`

#### 5.4.1.7 Màn hình Kết quả đặt vé

Màn hình thông báo kết quả sau khi khách hàng quay lại từ cổng thanh toán. Trang `BookingResultPage` (`features/client/bookings/pages/booking-result-page.tsx`) ứng với tuyến `/booking/result`; địa chỉ quay về do máy chủ dựng theo từng cổng và có dạng `…/booking/result?bookingId={id}` (VNPay, MoMo) hoặc kèm thêm `session_id` hay `canceled=true` (Stripe), trang đọc tham số `bookingId`.

Giao diện là một cột giữa trang với bốn trường hợp. Thành công (`BookingStatus.Confirmed`): `BookingResultStatus` hiển thị biểu tượng, nhãn "Thanh toán thành công", mô tả và mã đặt vé, tiếp theo là `BookingResultTickets` liệt kê mỗi ghế một thẻ `PerfoTicket` có mã QR, và hiệu ứng pháo giấy `Confetti` phát đúng một lần. Thất bại gồm `Cancelled` ("Đơn đặt vé đã bị huỷ") và `Expired` ("Đơn đặt vé đã hết hạn"); hai trạng thái này lấy nhãn, mô tả và màu từ `bookingResultStatusConfig` và không hiển thị vé. Đang xử lý (`Pending`): nhãn "Đang xử lý thanh toán" với mô tả chờ xác nhận từ cổng thanh toán. Không tìm thấy: khi truy vấn kết thúc mà không có dữ liệu, trang hiển thị "Không tìm thấy đơn đặt vé này." kèm nút "Thử lại"; nếu URL thiếu `bookingId`, trang hiển thị "Thiếu thông tin đơn đặt vé." Trong lúc tải hiển thị `Skeleton`. Ở mọi trường hợp có dữ liệu, cuối trang có hai nút "Xem vé của tôi" (`/account/tickets`) và "Về trang chủ".

Vì xác nhận thanh toán đến từ cổng thanh toán một cách bất đồng bộ nên khi trạng thái là `Pending`, trang gọi lại `refetch` mỗi `BOOKING_RESULT_POLL_INTERVAL_MS` (3000 ms) cho đến khi trạng thái thay đổi. Nếu đơn đã xác nhận nhưng chưa có bản ghi vé (`tickets` rỗng), `BookingResultTickets` hiển thị một thẻ tổng hợp dùng mã đặt vé làm mã QR.

Trang dùng `useBookingDetail` gọi `GET /bookings/{bookingId}` (`BookingDetailDto` gồm `status`, `movie`, `showtime`, `seats`, `combos`, `tickets`, `payments`, `holdExpiresAt`) và thành phần dùng chung `PerfoTicket` (Mục 5.4.1.9).

![Hình 5.8 Màn hình Kết quả đặt vé](hinh/5.8.png)

*Hình 5.8. Màn hình Kết quả đặt vé* `[CẦN BỔ SUNG: ảnh chụp màn hình Kết quả đặt vé, thành công và thất bại]`

#### 5.4.1.8 Màn hình Vé của tôi

Màn hình liệt kê các đơn đặt vé của khách hàng để theo dõi, xem chi tiết hoặc hủy vé. Trang `AccountTicketsPage` (`features/client/account/pages/tickets-page.tsx`), tuyến `/account/tickets`, nằm trong `AccountLayout` (thanh bên gồm `AccountUserCard` và `AccountNav`) và được bảo vệ bởi `CustomerProtectedRoute`.

Phía trên danh sách là các nút lọc theo trạng thái: "Tất cả" cùng bốn nhãn lấy từ `bookingStatusOptions` ("Chờ xử lý", "Đã xác nhận", "Đã hủy", "Hết hạn"). Mỗi đơn là một `TicketCard` gồm tên phim, thời điểm chiếu, mã đặt vé, `BookingStatusBadge`, thành tiền, liên kết "Chi tiết & QR" tới `/account/tickets/{bookingId}` và, nếu đủ điều kiện, nút "Hủy vé". Điều kiện hiển thị nút hủy gồm ba vế: đơn ở trạng thái `Confirmed`, cấu hình có hoàn tiền (`cancelRefundPercent > 0`) và thời điểm chiếu còn cách hiện tại hơn `cancelDeadlineHours` giờ (giá trị dự phòng 2 giờ khi chưa tải được cấu hình); thời điểm hiện tại lấy từ `useNow`. Bấm "Hủy vé" mở hộp thoại xác nhận "Hủy vé & hoàn tiền", đồng ý thì gọi `cancelBooking.mutate`. Danh sách cuộn vô hạn bằng `InfiniteScrollTrigger` với `limit: 10`; khi tải hiển thị ba `Skeleton`, khi rỗng hiển thị `EmptyState` "Chưa có vé nào".

Các hook: `useMyBookings` (`GET /bookings` với `status`, `cursor`, `limit`), `useCancelBooking` (`POST /bookings/{bookingId}/cancel`) và `useCinemaSettings` (`GET /settings`).

![Hình 5.9 Màn hình Vé của tôi](hinh/5.9.png)

*Hình 5.9. Màn hình Vé của tôi* `[CẦN BỔ SUNG: ảnh chụp màn hình Vé của tôi]`

#### 5.4.1.9 Màn hình Chi tiết vé

Màn hình hiển thị toàn bộ thông tin một đơn đặt vé và vé điện tử có mã QR để khách hàng trình tại quầy soát vé. Trang `AccountTicketDetailPage` (`features/client/account/pages/ticket-detail-page.tsx`), tuyến `/account/tickets/:id`, được bảo vệ bởi `CustomerProtectedRoute` nhưng không nằm trong `AccountLayout`.

Giao diện gồm các khối xếp dọc: đầu trang với mã đặt vé (kèm nút sao chép vào bộ nhớ tạm), thời điểm đặt và `BookingStatusBadge`; khối thông tin phim gồm poster, tên phim, phòng, giờ chiếu; khối "Ghế & Combo" liệt kê giá từng ghế và từng combo với "Tổng tiền", "Giảm giá" (khi lớn hơn 0) và "Thành tiền"; khối "Thanh toán" liệt kê từng giao dịch với `PaymentMethodBadge`, số tiền và `PaymentStatusBadge`; cuối cùng là "Vé điện tử & Mã QR" gồm mỗi ghế một `PerfoTicket`. Thành phần `PerfoTicket` (`components/common/perfo-ticket.tsx`) là một ảnh SVG gồm phần thân (tên phim, suất chiếu, phòng, ghế, tổng tiền) và phần cuống chứa mã QR cùng mã vé dạng `{bookingCode}-{ghế}`; mã QR hiển thị trực tiếp nếu `qrCodeImage` là ảnh (`data:image` hoặc `http`), ngược lại được dựng bằng `QRCodeSVG` của `qrcode.react`; vé đã soát (`isCheckedIn`) bị phủ lớp tối và dòng chữ "ĐÃ SOÁT VÉ".

Với đơn `Pending`, một khung cảnh báo hiển thị thời gian giữ ghế còn lại (`useCountdown(holdExpiresAt)`): nếu còn hạn, nút "Thanh toán ngay" dẫn tới `/booking/{showtimeId}?resume={bookingId}` (xem Mục 5.4.1.6); nếu đã hết hạn, nút "Đặt vé mới" dẫn tới `/booking/{showtimeId}`. Nút "Hủy vé & Hoàn tiền" ở cuối trang xuất hiện theo cùng điều kiện như ở Mục 5.4.1.8 và mở hộp thoại xác nhận trước khi gọi `cancelBooking.mutate`. Trong lúc tải, trang hiển thị hai `Skeleton`.

Trang dùng `useBookingDetail` (`GET /bookings/{bookingId}`), `useCancelBooking` (`POST /bookings/{bookingId}/cancel`) và `useCinemaSettings` (`GET /settings`).

![Hình 5.10 Màn hình Chi tiết vé](hinh/5.10.png)

*Hình 5.10. Màn hình Chi tiết vé* `[CẦN BỔ SUNG: ảnh chụp màn hình Chi tiết vé có mã QR]`

#### 5.4.1.10 Màn hình Hồ sơ cá nhân

Màn hình cho phép khách hàng xem thông tin tài khoản, cập nhật hồ sơ và (ở trang kế cận) đổi mật khẩu. Trang `AccountProfilePage` (`features/client/account/pages/profile-page.tsx`), tuyến `/account/profile`; trang đổi mật khẩu `ChangePasswordPage` ở tuyến `/account/change-password`, cùng nằm trong `AccountLayout`.

Trang hồ sơ có hai khối. Khối trên là `ProfileStatsCard` hiển thị hạng thành viên (`MembershipTierBadge`, mặc định "Thành viên" khi chưa có hạng), điểm tích lũy, tổng chi tiêu, thanh tiến độ tới hạng kế tiếp cùng dòng "Cần thêm {n} điểm để đạt hạng {tên}" khi có `nextTier`, và ghi chú rằng điểm được tính trong 12 tháng gần nhất, điểm cũ hơn tự hết hạn và có thể làm thay đổi hạng, còn ưu đãi giảm giá chỉ áp dụng trên tiền vé. Khối dưới là biểu mẫu "Thông tin cá nhân" gồm ảnh đại diện (`AvatarUpload`), họ tên, số điện thoại, ngày sinh (`FormDatePickerField`), giới tính (Nam, Nữ, Khác) và email ở dạng chỉ đọc kèm nhãn "Đã xác thực". Biểu mẫu dùng `react-hook-form` với `zodResolver`: họ tên bắt buộc và tối đa 255 ký tự, số điện thoại tối đa 20 ký tự. `AvatarUpload` kiểm tra kích thước tệp không quá `MAX_IMAGE_SIZE_BYTES` (5 MB), tải ảnh lên bằng `useUploadImage` vào thư mục `avatars` và báo lỗi tại chỗ khi thất bại. Khi lưu, `useUpdateMyProfile` gửi yêu cầu rồi cập nhật lại tên, email, ảnh đại diện trong kho xác thực `useAuthStore` để thanh tiêu đề phản ánh ngay.

Trang đổi mật khẩu có ba trường "Mật khẩu hiện tại", "Mật khẩu mới", "Xác nhận mật khẩu mới". Mật khẩu mới phải có ít nhất 8 ký tự, có cả chữ hoa và chữ thường và đạt ít nhất 3 trong 4 tiêu chí của `scorePassword` (độ dài, chữ hoa lẫn thường, chữ số, ký tự đặc biệt); `PasswordStrengthMeter` hiển thị thanh độ mạnh theo năm mức "Rất yếu" đến "Mạnh". Khi thành công, biểu mẫu được đặt lại.

API: `GET /users/me` (`useMyProfile`), `PUT /users/me` (`useUpdateMyProfile`, `meta: { invalidates: "profileChanged" }`), `POST /users/me/change-password` (`useChangePassword`); tải ảnh dùng hook `useUploadImage` của `features/admin/uploads`.

![Hình 5.11 Màn hình Hồ sơ cá nhân](hinh/5.11.png)

*Hình 5.11. Màn hình Hồ sơ cá nhân* `[CẦN BỔ SUNG: ảnh chụp màn hình Hồ sơ cá nhân]`

#### 5.4.1.11 Màn hình Thành viên

Màn hình giới thiệu chương trình khách hàng thân thiết, các hạng thẻ, quyền lợi và quy tắc điểm. Trang `MembershipPage` (`features/client/membership/pages/membership-page.tsx`), tuyến `/membership`, mở công khai.

Trang gồm bốn khối: phần giới thiệu nêu quy tắc "10.000 VNĐ = 1 điểm"; lưới `TierCard` cho từng hạng, mỗi thẻ có biểu tượng, tên hạng, ngưỡng điểm tối thiểu (`minPoints`), mức chiết khấu (`discountPercent`) và một dòng quyền lợi; bảng so sánh `TierCompareTable` giữa các hạng; và danh sách hỏi đáp `MembershipFaq` dạng `Accordion`. Hạng được sắp xếp tăng dần theo `minPoints`. Biểu tượng, màu và dòng quyền lợi của thẻ lấy theo thứ tự từ `TIER_VISUALS` trong `constants.ts` (bốn mục, thẻ vượt quá dùng mục cuối), còn các dòng của bảng so sánh lấy từ `COMPARE_ROWS`, mỗi dòng có chỉ số `unlockAt` cho biết hạng đầu tiên được hưởng: "Tích điểm mỗi giao dịch" (0), "Chiết khấu trên giá vé (không áp dụng combo)" (1), "Ưu tiên giữ chỗ sự kiện" (2), "Combo bắp nước miễn phí sinh nhật" (3). Ba câu hỏi trong `FAQ_ITEMS` giải thích cách tích điểm, hiệu lực điểm và hạng, và việc trừ điểm khi hủy vé; câu trả lời về hiệu lực nêu điểm có hiệu lực trong 12 tháng kể từ ngày tích và hạng luôn được xét theo tổng điểm còn hiệu lực trong 12 tháng gần nhất, phù hợp với cửa sổ trượt 12 tháng mô tả ở Hình 4.58. Khi đang tải, trang hiển thị bốn `Skeleton`; khi tải lỗi, hiển thị thông báo "Không thể tải dữ liệu hạng thẻ. Vui lòng thử lại sau."; bảng so sánh chỉ hiển thị khi có ít nhất một hạng.

Các nhãn chữ liên quan đến cơ chế điểm 12 tháng hiện nằm ở `membership/constants.ts` (câu hỏi "Điểm và hạng thẻ của tôi có bị hết hạn không?"), `ProfileStatsCard` (Mục 5.4.1.10) và `booking-price-summary.tsx`, nơi nhãn giảm giá ở bước thanh toán là "Giảm giá hạng thành viên (áp dụng cho vé)". Giao diện chưa hiển thị ngày hết hạn của từng khoản điểm.

Trang dùng `useMembershipTiers` (`features/client/catalog/hooks/use-catalog.ts`) gọi `GET /membership-tiers`; kiểu `MembershipTierDto` gồm `tierId`, `tierName`, `minPoints`, `discountPercent`.

![Hình 5.12 Màn hình Thành viên](hinh/5.12.png)

*Hình 5.12. Màn hình Thành viên* `[CẦN BỔ SUNG: ảnh chụp màn hình Thành viên]`

#### 5.4.1.12 Màn hình Trợ lý AI

Trợ lý AI giúp khách hàng hỏi đáp về phim, lịch chiếu và vé đã đặt bằng ngôn ngữ tự nhiên. Đây không phải một trang riêng mà là cửa sổ trò chuyện nổi `AiAssistantWidget` (`features/client/ai/components/ai-assistant-widget.tsx`), được gắn vào `ClientLayout` nên xuất hiện ở mọi trang dưới bố cục khách hàng.

Giao diện gồm nút tròn góc phải dưới để mở hoặc đóng, và khi mở là một khung gồm: đầu khung "Trợ lý {tên thương hiệu}" kèm dòng "Luôn sẵn sàng hỗ trợ" và nút đóng; vùng tin nhắn cuộn với `AiChatMessageBubble` cho từng tin (tin của trợ lý được hiển thị qua `renderMarkdownLite`); `AiTypingIndicator` ba chấm khi đang chờ phản hồi; và `AiChatInput` gồm ô nhập nhiều dòng cùng nút gửi. Tin chào ban đầu do `GREETING` cung cấp. Khi mới có tin chào, khung hiển thị bốn gợi ý nhanh: "Phim nào đang chiếu hôm nay?", "Lịch chiếu cuối tuần này", "Suất tối nay còn ghế không?", "Vé tôi đã đặt". Phản hồi của trợ lý có thể kèm các thẻ `AiChatCards` thuộc hai loại `showtime` (nút "Đặt vé") và `booking` (nút "Xem vé"); mỗi thẻ gồm ảnh, tiêu đề, phụ đề, chi tiết và là liên kết tới `card.link`, bấm vào thì khung tự đóng (`onNavigate`).

Về tương tác, nhấn Enter gửi tin, Shift+Enter xuống dòng; ô nhập và nút gửi bị vô hiệu hóa khi đang chờ phản hồi hoặc khi nội dung rỗng. Khi gửi, widget đóng gói lịch sử hội thoại (loại bỏ tin chào) thành danh sách `ChatMessageDto { role, content }` cùng tin mới; khi có phản hồi thì thêm tin của trợ lý và các thẻ, còn khi lỗi thì thêm tin "Chưa gửi được tin nhắn. Kiểm tra kết nối rồi gửi lại." Vùng tin nhắn tự cuộn xuống cuối mỗi khi có thay đổi. Lịch sử hội thoại chỉ lưu trong trạng thái của widget, không được lưu lâu dài phía giao diện.

Hook `useAiChat` (`hooks/use-ai-chat.ts`) bọc `useMutation` gọi `aiApi.chat`, tức `POST /ai/chat` với thân `{ message, history }`; phản hồi `AiChatResponse` gồm `reply`, `timestamp`, `cards`. Điểm cuối này không yêu cầu đăng nhập; `AiController` chỉ đọc định danh người dùng từ mã thông báo nếu có và truyền cho `IAiAssistantService.ChatAsync`.

![Hình 5.13 Màn hình Trợ lý AI](hinh/5.13.png)

*Hình 5.13. Màn hình Trợ lý AI* `[CẦN BỔ SUNG: ảnh chụp cửa sổ Trợ lý AI có thẻ suất chiếu hoặc vé]`

#### 5.4.1.13 Màn hình Đăng nhập

Màn hình cho phép khách hàng đăng nhập bằng email và mật khẩu hoặc bằng tài khoản Google. Trang `LoginPage` (`features/auth/pages/login-page.tsx`), tuyến `/auth/login`, nằm trong `AuthLayout` (ảnh nền, biểu trưng và thẻ biểu mẫu) và `GuestRoute`: người đã đăng nhập bị chuyển hướng khỏi trang này.

Biểu mẫu `LoginForm` gồm trường Email, trường Mật khẩu (`PasswordInput`) kèm liên kết "Quên mật khẩu?" tới `/auth/forgot-password`, nút "Đăng nhập", dải phân cách "Hoặc", nút Google do `GoogleSignInButton` dựng, và liên kết "Đăng ký ngay" tới `/auth/register`. Lược đồ kiểm tra của `zod` yêu cầu email hợp lệ và mật khẩu từ 6 ký tự. Khi đang gửi, nút hiển thị biểu tượng quay; khi máy chủ từ chối, thông điệp lấy từ `extractApiError` hiển thị trong khung lỗi phía trên nút.

Đăng nhập thành công, `useLogin` lưu `refreshToken` bằng `refreshTokenStorage`, ghi `accessToken` và thông tin người dùng vào `useAuthStore.setCredentials`, rồi gọi `GET /auth/me` để cập nhật họ tên, email, ảnh đại diện và vai trò; sau đó trang điều hướng tới giá trị của tham số `redirect` trên URL (mặc định `/`), vì vậy các nơi yêu cầu đăng nhập như Mục 5.4.1.3 và 5.4.1.5 đều đính kèm `?redirect=`. Nút Google chỉ được dựng khi biến môi trường `GOOGLE_CLIENT_ID` có giá trị: thành phần chờ SDK Google nạp xong (kiểm tra theo chu kỳ `GOOGLE_SDK_POLL_INTERVAL_MS`, 100 ms), gọi `renderButton` với giao diện sáng hoặc tối theo chủ đề, và khi nhận chứng chỉ thì gọi `useGoogleLogin`; lỗi được báo bằng thông báo "Đăng nhập Google thất bại".

API: `POST /auth/login`, `POST /auth/google` (thân `{ idToken }`), `GET /auth/me`; các yêu cầu này nằm trong danh sách ngoại lệ của bộ chặn phản hồi để lỗi 401 không kích hoạt cơ chế làm mới mã thông báo.

![Hình 5.14 Màn hình Đăng nhập](hinh/5.14.png)

*Hình 5.14. Màn hình Đăng nhập* `[CẦN BỔ SUNG: ảnh chụp màn hình Đăng nhập]`

#### 5.4.1.14 Màn hình Đăng ký

Màn hình cho phép khách hàng mới tạo tài khoản và hướng dẫn họ xác nhận email trước khi sử dụng. Trang `RegisterPage` (`features/auth/pages/register-page.tsx`), tuyến `/auth/register`, cùng bố cục và `GuestRoute` với màn hình đăng nhập; bước xác nhận email là trang `ConfirmEmailPage` ở tuyến `/auth/confirm-email`.

Biểu mẫu `RegisterForm` gồm các trường Họ và tên, Email, Số điện thoại (không bắt buộc), Mật khẩu kèm `PasswordStrengthMeter`, Xác nhận mật khẩu, nút "Đăng ký" và liên kết "Đăng nhập". Các ràng buộc trong `registerSchema`: họ tên từ 2 ký tự; email hợp lệ; số điện thoại nếu có phải khớp `^0\d{9,10}$`; mật khẩu theo `passwordSchema` (ít nhất 8 ký tự, có chữ hoa và chữ thường, đạt ít nhất 3 trong 4 tiêu chí độ mạnh); mật khẩu xác nhận phải trùng. Khi máy chủ trả lỗi theo từng trường, `getFieldErrors` gán lỗi về đúng trường; lỗi không gắn trường hiển thị trong khung lỗi chung. Đăng ký thành công thì biểu mẫu được thay bằng thông báo "Đăng ký thành công" kèm địa chỉ email đã đăng ký, yêu cầu kiểm tra hộp thư để xác thực và nút "Về trang đăng nhập".

Liên kết trong email dẫn tới `/auth/confirm-email?token=…`. `ConfirmEmailPage` tự gọi `useConfirmEmail` đúng một lần khi có `token` (có tham chiếu `calledRef` để tránh gọi lặp) và hiển thị bốn trạng thái: thiếu mã ("Liên kết không hợp lệ"), đang xác thực, "Xác thực thành công" kèm nút "Đăng nhập", và "Xác thực thất bại" kèm thông điệp lỗi cùng biểu mẫu nhập email để "Gửi lại liên kết xác thực" qua `useResendConfirmation`; sau khi gửi lại, trang hiển thị thông báo trung tính "Nếu email tồn tại và chưa xác thực, liên kết mới đã được gửi."

API: `POST /auth/register` (thân `fullName`, `email`, `password`, `phone`), `POST /auth/confirm-email` (thân `{ token }`), `POST /auth/resend-confirmation` (thân `{ email }`). Các trang quên mật khẩu và đặt lại mật khẩu (`/auth/forgot-password`, `/auth/reset-password`) dùng cùng bố cục nhưng không thuộc phạm vi mô tả của mục này.

![Hình 5.15 Màn hình Đăng ký](hinh/5.15.png)

*Hình 5.15. Màn hình Đăng ký* `[CẦN BỔ SUNG: ảnh chụp màn hình Đăng ký và màn hình xác nhận email]`

<!-- GHI CHÚ ĐỐI CHIẾU -->

**(a) `[CẦN BỔ SUNG]` còn lại**

- Hình 5.2 đến Hình 5.15: chưa có ảnh chụp màn hình nào được đính kèm; cần 14 ảnh lưu tại `hinh/5.2.png` … `hinh/5.15.png`. Riêng Hình 5.8 nên có ảnh trường hợp thành công và thất bại; Hình 5.13 nên có ít nhất một thẻ suất chiếu hoặc vé; Hình 5.15 nên kèm màn hình xác nhận email.
- Caption hình ghi theo quy định (đặt dưới hình, nhãn "Hình X.n. Tên hình"); dòng `[CẦN BỔ SUNG]` ngay sau caption là nhắc việc, xóa khi đã chèn ảnh.
- Cỡ chữ caption và nội dung bảng trong quy định cũng còn `[CẦN BỔ SUNG]`, không ảnh hưởng đến nội dung phần này.

**(b) Chỗ outline hoặc tài liệu khác lệch với code**

1. Mục 5.4.1.5 (outline): liệt kê "đếm ngược" trong màn hình Chọn ghế. Code: bộ đếm `BookingHoldTimer` chỉ xuất hiện ở bước thanh toán (`booking-payment-step.tsx`); bước chọn ghế (`booking-selection-step.tsx`) chỉ hiển thị dòng chữ "Ghế được giữ trong {holdDurationMinutes} phút…". Bài viết đặt đếm ngược ở Mục 5.4.1.6. Ngoài ra "chú giải" nằm trong `SeatMapFrame`/`SeatLegend`, không phải khối riêng của `booking-selection-step.tsx`.
2. Mục 5.4.1.7 (outline): "thành công, thất bại, không tìm thấy". Code còn có trạng thái `Pending` ("Đang xử lý thanh toán", có polling 3 giây) và `Expired` tách riêng khỏi `Cancelled`, cùng trường hợp thiếu `bookingId` trên URL (`booking-result-page.tsx`). Đã mô tả đủ.
3. Mục 5.4.1.4: `showtimes-api.ts` có chú thích `// GET /showtimes?date=dd/MM/yyyy` nhưng giá trị thực gửi đi là `yyyy-MM-dd` (`toDateKey` → `toDateInputValue`, `format(d, "yyyy-MM-dd")`) và `ShowtimesController.GetSchedule` nhận `DateOnly?`. Bài viết theo code (`yyyy-MM-dd`); chú thích trong `showtimes-api.ts` đã lỗi thời.
4. Mục 5.4.1.2: outline ghi "phân trang con trỏ"; code là cuộn vô hạn dựa trên con trỏ (`useInfiniteQuery` + `InfiniteScrollTrigger`), không có điều khiển chuyển trang. Tham số `cursor` có trong `clientMovieParsers` nhưng `MoviesPage` không truyền vào `useMovies` (hook bỏ `cursor` khỏi tham số), chỉ đặt về `null` khi đổi bộ lọc.
5. Mục 5.4.1.11 (outline): nêu `booking-detail-page.tsx` và `counter-customer-form.tsx` thuộc thay đổi nhãn. Hai tệp này nằm ở `features/admin/bookings/pages/booking-detail-page.tsx` (nhãn "Giảm giá (chỉ trên vé)") và `features/admin/counter/components/counter-customer-form.tsx` (dòng "Khách hàng hạng …", không có chữ "12 tháng"), thuộc giao diện quản trị nên không mô tả ở đây (Mục 5.4.2). Phía khách hàng, nhãn giảm giá nằm ở `booking-price-summary.tsx`; riêng `ticket-detail-page.tsx` vẫn dùng nhãn "Giảm giá" chung, không nêu "chỉ trên vé". Không tìm thấy cụm "năm dương lịch" ở `frontend/src` (đã đổi thành 12 tháng).
6. Mục 5.4.1.11, nội dung tĩnh chưa nhất quán: (i) `membership-page.tsx` ghi cứng "10.000 VNĐ = 1 điểm" trong khi `CinemaSettings.PointsPerAmount` cấu hình được (giá trị khởi tạo `10000` trong `SeedData.cs`); (ii) `TIER_VISUALS[1].perk` ghi "Chiết khấu ưu đãi vé & bắp nước" trong khi dòng so sánh và `booking-price-summary.tsx` nêu giảm giá chỉ trên tiền vé, không áp dụng combo; (iii) hình thức, quyền lợi của thẻ gắn theo thứ tự (chỉ số) trong `TIER_VISUALS`/`COMPARE_ROWS`, giả định có đúng bốn hạng như dữ liệu khởi tạo (Thành viên 0, Bạc 500, Vàng 2000, Kim Cương 5000 điểm; chiết khấu 0/5/10/15%), còn `membership-page.tsx` mô tả SEO theo tên Bronze, Silver, Gold, Platinum khác tên hạng trong dữ liệu. Cần xem lại nếu muốn đồng nhất với Chương 4.
7. Mục 5.4.1.12: outline gọi là "Màn hình Trợ lý AI" nhưng code là widget nổi không có tuyến riêng (`ClientLayout`), nên Hình 5.13 phải chụp cửa sổ chat trên một trang bất kỳ.
8. Mục 5.4.1.13 và 5.4.1.14: lược đồ mật khẩu ở form đăng nhập chỉ yêu cầu `min(6)` còn đăng ký và đổi mật khẩu yêu cầu `passwordSchema` (tối thiểu 8 ký tự, độ mạnh); `isValidVnPhone` (biểu mẫu đặt vé) chấp nhận `^(0|\+84)\d{9,10}$` còn biểu mẫu đăng ký chỉ chấp nhận `^0\d{9,10}$`. Bài viết ghi đúng từng nơi.
9. Chưa kiểm chứng bằng chạy thực tế: các mô tả hành vi (hoạt ảnh băng chuyền, hộp thoại, polling) dựa trên đọc mã nguồn, không dựa trên ảnh chụp hoặc nhật ký chạy.
10. Các điểm cuối khách hàng được đối chiếu với `Cinema.Api/Controllers/Client/*` (đường dẫn, kiểu HTTP); `UsersController` (`api/v1/users`, `[Authorize]`) có `GET me`, `PUT me`, `GET me/reviews`, `POST me/change-password`; chưa đối chiếu thân yêu cầu từng action. Việc `POST /ai/chat` cho phép ẩn danh dựa trên `AiController` (đọc claim nếu có, không gắn `[Authorize]`).

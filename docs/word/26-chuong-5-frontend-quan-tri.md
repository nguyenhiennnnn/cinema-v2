### 5.4.2 Giao diện quản trị

Phân hệ quản trị nằm trong `frontend/src/features/admin/`, mỗi nghiệp vụ là một thư mục con gồm các lớp `pages`, `components`, `hooks`, `api` và `types`. Toàn bộ trang quản trị được khai báo trong `app/routes/admin-routes.tsx` dưới đường dẫn `/admin`. Tuyến này được bọc bởi `AdminProtectedRoute`: người dùng chưa đăng nhập bị chuyển về `/admin/login`, người dùng có vai trò khác `Role.Admin` và `Role.Employee` bị chuyển về trang chủ. Việc phân biệt quyền chi tiết hơn được thực hiện ở hai nơi. Phía giao diện, `ADMIN_NAV_GROUPS` trong `config/admin-nav.ts` khai báo vai trò được thấy từng mục menu và `filterNavGroupsByRole` lọc menu theo vai trò hiện tại. Phía máy chủ, các controller quản trị áp dụng chính sách `Policies.AdminOnly` hoặc `Policies.StaffOnly`. Nhân viên (`Role.Employee`) được truy cập quầy bán vé, soát vé, quản lý phim, suất chiếu và đặt vé; các màn hình còn lại chỉ dành cho quản trị viên (`Role.Admin`). Sau khi đăng nhập, `AdminIndexRedirect` đưa quản trị viên về `/admin/dashboard` và nhân viên về `/admin/bookings/counter`.

Các màn hình dạng danh sách dùng chung một khung cài đặt. Trạng thái bộ lọc, từ khóa tìm kiếm và số trang được giữ trên URL bằng thư viện `nuqs` thông qua các hook trong `hooks/use-query-params.ts` (ví dụ `useAdminMovieParams`, `useAdminShowtimeParams`); ô tìm kiếm chỉ cập nhật URL sau khoảng trễ `SEARCH_DEBOUNCE_MS` (300 ms). Dữ liệu được lấy bằng TanStack Query và hiển thị trong `DataTable` dựa trên TanStack Table với `manualPagination`, nghĩa là phân trang do máy chủ thực hiện. Biểu mẫu dùng `react-hook-form` kết hợp `zod`, mở trong ngăn trượt (`useSheetOpen`) hoặc hộp thoại (`useDialogOpen`), còn thao tác cần xác nhận mở qua `useAlertDialogOpen`. Mỗi mutation khai báo `meta: { invalidates: "…" }`; quan hệ giữa tên nhóm và các khóa truy vấn cần làm mới được tập trung ở `api/query-invalidation.ts`. Các thành phần dùng chung như `PageHeader`, `DataTable`, `EnumSelectFilter` được trình bày ở Mục 5.4.4, nên các mục dưới đây chỉ nhắc tới khi chúng có vai trò riêng trong từng màn hình.

#### 5.4.2.1 Màn hình Dashboard

Màn hình Dashboard (`/admin/dashboard`, chỉ dành cho quản trị viên) cho quản trị viên nắm nhanh tình hình kinh doanh trong ngày và xu hướng gần đây mà không phải mở từng báo cáo chi tiết. Trang `AdminDashboardPage` gồm tiêu đề kèm bộ chọn khoảng thời gian, hàng thẻ chỉ số, biểu đồ doanh thu, biểu đồ phim bán chạy và danh sách suất chiếu trong ngày.

Hàng thẻ chỉ số do `DashboardStatCards` dựng gồm năm thẻ: doanh thu hôm nay, số vé bán hôm nay, số đơn đã xác nhận, số đơn đang chờ và tỉ lệ lấp đầy hôm nay. Thẻ doanh thu kèm dòng chú thích phần trăm thay đổi so với hôm qua, tô màu theo chiều tăng, giảm hoặc không đổi, và chỉ xuất hiện khi `revenueChangePercent` khác `null`. Thẻ đơn đang chờ hiển thị thêm dòng "Cần theo dõi" khi `activePendingBookings` lớn hơn 0. Tỉ lệ lấp đầy hiển thị dấu "—" khi `todayOccupancyPercent` là `null`. Biểu đồ "Doanh thu theo ngày" (`RevenueTrendChart`) là biểu đồ cột chồng của thư viện `recharts`, bọc trong `ChartContainer` của lớp giao diện `components/ui/chart`; mỗi cột gồm hai phần `ticketRevenue` (doanh thu vé) và `comboRevenue` (doanh thu combo), chiếm hai trong ba cột của lưới trên màn hình rộng, còn chú giải và tooltip định dạng tiền bằng `formatCurrency`. Biểu đồ "Phim bán chạy" (`TopMoviesCard`) là biểu đồ cột ngang theo `ticketsSold`, tên phim dài hơn 18 ký tự bị cắt và thêm dấu "…". Khối "Suất chiếu hôm nay" (`TodayShowtimesCard`) liệt kê từng suất dưới dạng thẻ gồm tên phim, giờ chiếu, tên phòng, tỉ lệ `bookedSeats/totalSeats` và thanh tiến độ có chiều rộng bằng phần trăm làm tròn của tỉ lệ đó.

Về tương tác, bộ chọn `DashboardDaysFilter` (dùng `SelectField`) cho phép chọn 7, 14 hoặc 30 ngày; trạng thái `days` mặc định là 14 và chỉ ảnh hưởng đến dữ liệu xu hướng và phim bán chạy. Trong lúc tải, mỗi khối hiển thị khung xương `Skeleton`; khi không có dữ liệu, các khối biểu đồ và danh sách hiển thị `EmptyState` kèm thông báo tương ứng. Dữ liệu được lấy bằng hai hook trong `hooks/use-dashboard.ts`. `useDashboardSummary` gọi `GET /admin/dashboard/summary` và được truy vấn lại định kỳ theo `DASHBOARD_REFETCH_INTERVAL_MS`, bằng 60 000 ms. `useDashboardInsights` gọi `GET /admin/dashboard/insights` với tham số `days` và trả về ba tập `revenueTrend`, `topMovies` và `todayShowtimes`; truy vấn này không đặt chu kỳ làm mới riêng mà được làm mới khi các mutation thuộc nhóm có làm mới Dashboard hoàn tất.

![Hình 5.16 Màn hình Dashboard](hinh/5.16.png)

*Hình 5.16. Màn hình Dashboard*

#### 5.4.2.2 Màn hình Quản lý phim

Màn hình Quản lý phim (`/admin/movies`) phục vụ việc duy trì danh sách phim, trạng thái phát hành và thông tin chi tiết của từng phim. Cả quản trị viên và nhân viên đều xem được danh sách, nhưng biến `canManage` (bằng `true` khi vai trò là `Role.Admin`) quyết định việc hiển thị nút "Thêm phim", cột hành động và thanh thao tác hàng loạt.

Thanh công cụ có ô tìm theo tên phim (placeholder "Tìm theo tên phim...") và bộ lọc trạng thái phát hành `EnumSelectFilter` với ba giá trị trong `movieStatusOptions`: "Sắp chiếu", "Đang chiếu" và "Đã ngừng chiếu". Bảng gồm các cột ID, tên phim (kèm ảnh poster thu nhỏ, dùng ảnh `/placeholders/movie-poster.png` khi chưa có poster), đạo diễn, ngôn ngữ, ngày khởi chiếu và trạng thái hiển thị bằng `MovieStatusBadge`; cột hành động mở menu gồm "Chỉnh sửa" và "Xóa phim". Cột chọn hàng cho phép xóa nhiều phim cùng lúc qua `BulkDeleteButton`.

Biểu mẫu thêm phim (`MovieCreateForm`) và cập nhật phim (`MovieUpdateForm`) mở trong ngăn trượt. Lược đồ `movieFormSchema` yêu cầu tên phim (tối đa 200 ký tự), thời lượng tối thiểu 1 phút và ít nhất một thể loại. Biểu mẫu gồm poster tỉ lệ 2:3, tên phim, tên gốc, mô tả, đạo diễn, thời lượng (mặc định 120 phút), ngày khởi chiếu, ngôn ngữ, phụ đề, độ tuổi, trailer, thể loại chọn nhiều (`MultiSelectField`) và danh sách diễn viên động. Danh sách diễn viên dùng `useFieldArray`, mỗi dòng gồm diễn viên chọn từ ô có tìm kiếm và tên nhân vật tùy chọn. Các danh mục đạo diễn, ngôn ngữ, độ tuổi, thể loại và diễn viên lấy từ các hook của thư mục `catalog`. Poster được tải lên bằng `ImageUploader` (thư mục `movies`) và trailer bằng `VideoUploader`; cả hai dựa trên `MediaUploader`, kiểm tra dung lượng trước khi gửi với giới hạn `MAX_IMAGE_SIZE_BYTES` (5 MB) và `MAX_VIDEO_SIZE_BYTES` (100 MB), hỗ trợ kéo thả, xem trước cục bộ bằng `URL.createObjectURL` và giải phóng URL khi thay đổi. Tệp được gửi tới `POST /admin/uploads/image` hoặc `POST /admin/uploads/video` kèm tham số `folder`, kết quả trả về `url` và `publicId`. Khi thêm mới, trạng thái phim được gán cố định là `ComingSoon`; biểu mẫu cập nhật có thêm trường trạng thái và nạp chi tiết phim bằng `useAdminMovieDetail` (`GET /movies/{id}`) rồi đặt lại giá trị biểu mẫu khi dữ liệu về.

Thao tác xóa một phim hiện hộp thoại xác nhận cảnh báo suất chiếu và đánh giá liên quan có thể bị ảnh hưởng, sau đó gọi `DELETE /admin/movies/{id}`. Xóa hàng loạt gọi `DELETE /admin/movies/bulk` với thân `{ movieIds }`; kết quả gồm `totalSuccess`, `totalFailed` và lý do từng phim thất bại được gom vào một thông báo, loại `warning` nếu có phim không xóa được. Các thao tác thêm, sửa và xóa khai báo nhóm làm mới `movieChanged`, làm mới danh sách phim, các chế độ xem suất chiếu, danh sách đánh giá quản trị và Dashboard. Danh sách được lấy bằng `GET /admin/movies`, thêm bằng `POST /admin/movies`, cập nhật bằng `PUT /admin/movies/{id}`. Khi chuyển trang, `DataTable` đặt lại các hàng đang chọn.

![Hình 5.17 Màn hình Quản lý phim](hinh/5.17.png)

*Hình 5.17. Màn hình Quản lý phim*

#### 5.4.2.3 Màn hình Quản lý suất chiếu

Màn hình Quản lý suất chiếu (`/admin/showtimes`) giúp lên lịch và theo dõi các suất chiếu của rạp. Trang `AdminShowtimesPage` chia thành hai tab: "Danh sách" (mặc định) và "Lịch". Nội dung tab "Lịch" được trình bày ở Mục 5.4.2.4. Quản trị viên và nhân viên cùng xem được trang, nhưng các nút thao tác trên đầu trang và cột hành động chỉ hiện khi `canManage` đúng.

Tab "Danh sách" gồm bộ lọc theo ngày chiếu (`DateFilterPicker`), bộ lọc trạng thái (`showtimeStatusOptions`) và bảng các cột ID, phim, phòng chiếu, khung giờ (giờ bắt đầu – kết thúc kèm ngày), giá vé và trạng thái (`ShowtimeStatusBadge`). Bảng không có ô tìm kiếm văn bản. Đầu trang có bốn nút dành cho quản trị viên: "Tự động xếp lịch", "Nhân bản lịch", "Tạo hàng loạt" và "Thêm suất chiếu". Mỗi nút mở một hộp thoại chứa biểu mẫu riêng. `ShowtimeCreateForm` nhận phim (tìm kiếm qua `useAdminMovies`), phòng chiếu (`useAdminRooms`), thời gian bắt đầu, giá vé (mặc định 75 000 VNĐ) và công tắc `allowAfterHours` cho phép suất kết thúc sau giờ đóng cửa. `ShowtimeUpdateForm` có các trường phim và phòng tương tự để chỉnh sửa. `ShowtimeBulkCreateForm` tạo lặp lại theo lịch gồm phim, phòng, khoảng ngày (`fromDate`, `toDate`), các thứ trong tuần (ít nhất một), danh sách khung giờ động (ít nhất một), giá vé và `allowAfterHours`. `ShowtimeCloneForm` sao chép các suất của một khoảng ngày nguồn (`sourceFrom`, `sourceTo`) sang lịch bắt đầu từ ngày đích `targetFrom`; biểu mẫu quy đổi hai ngày này thành số ngày dịch `shiftDays` bằng `differenceInCalendarDays` trước khi gửi. Sau khi tạo hàng loạt hoặc nhân bản, thông báo cho biết số suất đã tạo (`totalCreated`) và số suất bị bỏ qua do trùng lịch hoặc không hợp lệ (`totalSkipped`).

Trong cột hành động, quản trị viên chọn "Chỉnh sửa" hoặc "Hủy suất chiếu". Hộp thoại xác nhận hủy nêu rõ vé đã đặt sẽ bị hủy, thanh toán trực tuyến được hoàn tiền tự động còn vé thanh toán tiền mặt cần hoàn thủ công tại quầy; sau khi gọi `DELETE /admin/showtimes/{id}`, nếu phản hồi có `autoRefundQueuedCount` hoặc `manualRefundRequiredCount` lớn hơn 0 thì hiện thêm thông báo "Lưu ý hoàn tiền". Cột chọn hàng cho phép hủy hàng loạt qua `DELETE /admin/showtimes/bulk` với thân `{ showtimeIds }`. Các thao tác tạo, cập nhật và nhân bản khai báo nhóm làm mới `showtimeChanged`; thao tác hủy dùng nhóm `showtimeCancelled` để làm mới thêm dữ liệu liên quan tới vé và giao dịch. Các điểm cuối dùng trong màn hình này gồm `GET /admin/showtimes`, `POST /admin/showtimes`, `PUT /admin/showtimes/{id}`, `POST /admin/showtimes/bulk` và `POST /admin/showtimes/clone`, bên cạnh các điểm cuối đã nêu. Các hook tương ứng nằm trong `hooks/use-admin-showtimes.ts`.

![Hình 5.18 Màn hình Quản lý suất chiếu](hinh/5.18.png)

*Hình 5.18. Màn hình Quản lý suất chiếu*

#### 5.4.2.4 Màn hình Lịch suất chiếu

Màn hình Lịch suất chiếu không phải là một trang riêng mà là tab "Lịch" của `/admin/showtimes`, do `ShowtimeCalendarView` dựng. Mục đích của nó là cho người quản lý nhìn thấy cùng lúc các suất chiếu của mọi phòng theo trục thời gian trong ngày để phát hiện khoảng trống hoặc chồng lấn.

Phần đầu gồm hai bộ chọn ngày "Từ ngày", "Đến ngày", bộ chọn phòng chiếu (danh sách phòng lấy bằng `useAdminRooms` với `LOOKUP_PAGE_SIZE`) và bộ lọc trạng thái. Bên dưới, mỗi ngày trong khoảng chọn là một tab, nhãn gồm ngày dạng `dd/MM/yyyy` và số suất của ngày đó. Nội dung mỗi ngày do `ShowtimeCalendarDay` vẽ thành lưới thời gian: cột trái ghi tên phòng và loại màn hình, phần phải là thang giờ từ 0 đến 24 với vạch chia mỗi hai giờ (`HOUR_MARKS`), mỗi phòng một hàng. Các phòng được gom từ danh sách suất bằng `groupByRoom` và sắp xếp theo tên theo quy tắc so sánh tiếng Việt. Lưới có chiều rộng tối thiểu cố định và cuộn ngang khi màn hình hẹp. Mỗi suất chiếu là một nút do `ShowtimeCalendarItem` đặt tuyệt đối trong hàng phòng: vị trí trái bằng phút bắt đầu chia cho 1 440 phút của một ngày, chiều rộng bằng thời lượng chia cho 1 440 và không nhỏ hơn 3 %, với phút tính theo giờ nghiệp vụ bằng `getBusinessMinutesOfDay`. Nếu giờ kết thúc không lớn hơn giờ bắt đầu (suất qua nửa đêm), điểm kết thúc được kéo tới cuối ngày. Màu nút phụ thuộc trạng thái: xanh cho `Scheduled`, đỏ cho `Cancelled` và xám cho `Completed`. Nút hiển thị tên phim, giờ chiếu và số ghế đã bán trên tổng số ghế; tooltip gồm tên phim, khung giờ, số ghế và giá vé.

Khi mở tab lần đầu mà URL chưa có khoảng ngày, `ShowtimeCalendarView` tự đặt `from` và `to` là thứ Hai và Chủ nhật của tuần hiện tại (`startOfWeek` và `endOfWeek` của `date-fns` với `weekStartsOn: 1`). Các bộ lọc được giữ trên URL qua `useAdminShowtimeCalendarParams`. Truy vấn `useAdminShowtimesCalendar` gọi `GET /admin/showtimes/calendar` với `from`, `to`, `roomId` và `status`, và chỉ chạy khi có đủ `from` và `to`. Khi chưa có dữ liệu hiển thị `Skeleton`, khi khoảng chọn không có suất hiển thị thông báo trống. Bấm vào một suất chiếu gọi `onSelectShowtime`; trang cha chuyển dữ liệu của suất lịch sang dạng `AdminShowtimeDto` rồi mở biểu mẫu cập nhật như khi chọn "Chỉnh sửa" trong bảng.

![Hình 5.19 Màn hình Lịch suất chiếu](hinh/5.19.png)

*Hình 5.19. Màn hình Lịch suất chiếu*

#### 5.4.2.5 Màn hình Tự động xếp lịch

Tự động xếp lịch là hộp thoại `ShowtimeAutoScheduleForm` mở từ nút "Tự động xếp lịch" trên trang suất chiếu (chiều rộng tối đa `sm:max-w-2xl`). Mục đích là đề xuất lịch chiếu cho toàn bộ phòng trong một ngày hoặc một khoảng ngày để người quản lý xem lại rồi mới áp dụng. Thuật toán chọn phòng và giờ chiếu chạy ở backend; giao diện chỉ thu thập yêu cầu, hiển thị bản nháp và gửi bản nháp đã chốt.

Biểu mẫu có hai pha. Ở pha chọn phạm vi, người dùng nhập "Từ ngày", "Đến ngày (tùy chọn)", danh sách phim kèm số suất mong muốn, giá vé mặc định và công tắc suất khuya. Ngày không được trước ngày nghiệp vụ hiện tại (`calendarDisabled={{ before: getBusinessTodayDate() }}`). Mỗi dòng phim gồm ô chọn phim có tìm kiếm, ô giá riêng (tùy chọn, ghi đè giá mặc định) và số suất. Lược đồ `zod` yêu cầu số suất là số nguyên từ 1 đến 20 và có ít nhất một phim; nút "Thêm phim" bị vô hiệu khi đã đủ 20 dòng và nút xóa dòng bị vô hiệu khi chỉ còn một dòng. Giá vé mặc định khởi tạo 75 000. Bấm "Xếp lịch đề xuất" gọi `POST /admin/showtimes/auto-schedule/preview` qua `useAdminPreviewAutoSchedule`; phản hồi `AutoScheduleResponseDto` gồm `items` (các suất được xếp) và `unplaced` (phim không xếp đủ suất).

Ở pha xem trước, hộp thoại thay biểu mẫu bằng bản nháp. Nếu `unplaced` không rỗng, một khối cảnh báo màu lỗi liệt kê từng phim với nội dung "chỉ xếp được x/y suất". Các suất được liệt kê theo giờ bắt đầu và kết thúc (kèm ngày – tháng khi xếp nhiều ngày), tên phim, phòng và giá; mỗi dòng có nút xóa để loại suất khỏi bản nháp, thao tác này chỉ cập nhật trạng thái cục bộ `draft`. Chân hộp thoại cho biết khoảng ngày và số suất, kèm hai nút "Quay lại" (xóa bản nháp, trở về biểu mẫu) và "Áp dụng lịch". Nút áp dụng bị vô hiệu khi bản nháp rỗng hoặc đang gửi. Khi áp dụng, các suất còn lại được ánh xạ thành `{ movieId, roomId, startTime, basePrice, allowAfterHours }` và gửi bằng `POST /admin/showtimes/auto-schedule/apply` qua `useAdminApplyAutoSchedule`, hook này khai báo nhóm làm mới `showtimeChanged` và tắt thông báo thành công mặc định để biểu mẫu tự hiện thông báo kèm số suất đã tạo và số suất bị bỏ qua.

![Hình 5.20 Màn hình Tự động xếp lịch](hinh/5.20.png)

*Hình 5.20. Màn hình Tự động xếp lịch*

#### 5.4.2.6 Màn hình Quản lý phòng chiếu

Màn hình Quản lý phòng chiếu (`/admin/rooms`, chỉ dành cho quản trị viên) quản lý danh sách phòng và là điểm vào của trình soạn sơ đồ ghế. Trang `AdminRoomsPage` dùng `useAdminListParams` cho phân trang và `DataTable` không có cột chọn hàng hay ô tìm kiếm.

Bảng gồm các cột ID, tên phòng chiếu, loại màn hình, tổng số ghế, trạng thái hoạt động (`RoomStatusBadge`) và cột hành động. Menu hành động có "Xem chi tiết" (chuyển tới `/admin/rooms/{id}/seats`, truyền kèm đối tượng phòng trong `state` của điều hướng), "Chỉnh sửa" và "Tạm ngưng" hoặc "Kích hoạt lại" tùy trạng thái hiện tại. Nút "Thêm phòng chiếu" mở `RoomCreateForm` trong hộp thoại, gồm tên phòng (tối đa 100 ký tự), loại màn hình chọn từ `useAdminScreenTypes`, số hàng và số cột; hai giá trị sau là số nguyên từ 1 đến 50 và mặc định 10. `RoomUpdateForm` chỉnh sửa tên phòng, loại màn hình và trạng thái hoạt động.

Thao tác tạm ngưng hoặc kích hoạt lại hiện hộp thoại xác nhận với nội dung giải thích hệ quả (phòng không thể tạo suất chiếu mới cho đến khi được kích hoạt lại), rồi gọi `PUT /admin/rooms/{id}` với `isActive` đảo giá trị. Các hook trong `hooks/use-admin-rooms.ts` khai báo nhóm làm mới `roomChanged`, làm mới danh sách phòng, các chế độ xem suất chiếu và Dashboard. Danh sách phòng lấy bằng `GET /admin/rooms`, thêm phòng bằng `POST /admin/rooms`.

![Hình 5.21 Màn hình Quản lý phòng chiếu](hinh/5.21.png)

*Hình 5.21. Màn hình Quản lý phòng chiếu*

#### 5.4.2.7 Màn hình Trình soạn sơ đồ ghế

Trình soạn sơ đồ ghế (`/admin/rooms/:id/seats`, trang `AdminRoomSeatsPage`) cho phép quản trị viên chỉnh loại ghế, ghế đôi, lối đi và việc bật hoặc tắt từng ghế của một phòng bằng thao tác trực quan thay vì nhập từng bản ghi. Trang tải phòng bằng `useAdminRoom` (`GET /admin/rooms/{id}`), sau đó `SeatEditorLoader` tải danh sách ghế bằng `useAdminRoomSeats` (`GET /admin/rooms/{roomId}/seats`) và danh sách loại ghế bằng `useAdminSeatTypes`. Chỉ khi đủ dữ liệu, `SeatEditorReady` mới khởi tạo trình soạn; trong lúc chờ hiển thị `Skeleton`, và nếu tải ghế lỗi sẽ hiện thông báo yêu cầu tải lại trang.

Về cơ chế cài đặt, trạng thái trình soạn được đặt trong hook `useSeatEditorState`, xây trên hook `useSeatEditorHistory`. Sơ đồ là một `Map` các ô `EditorCell` (`seatRow`, `seatNumber`, `seatTypeId`, `isActive`, `isAisle`, `coupleId`) với khóa sinh từ `seatCellKey`; nhãn hàng sinh bằng `generateRowLabel` và kích thước ban đầu suy ra từ `deriveGridDimensions` trên các ghế hiện có. Mỗi thay đổi đi qua `history.commit`, lưu ảnh chụp trạng thái hiện tại vào ngăn xếp `past` (giới hạn `HISTORY_LIMIT` = 40 bước), xóa ngăn `future` và bật cờ `isDirty`; hoàn tác và làm lại hoán đổi ảnh chụp giữa `past`, hiện tại và `future`. Thanh công cụ `SeatEditorToolbar` gom các nút số hàng, số cột (`SeatEditorSizeInputs`, tối đa 52 hàng và 60 cột; đổi kích thước giữ lại các ô còn nằm trong lưới mới bằng `resizeCells`), hoàn tác và làm lại (`SeatEditorHistoryButtons`), "Xem trước", "Kiểm tra layout", "Lưu sơ đồ ghế" và dòng "Có thay đổi chưa lưu" khi `isDirty`. Lưới ghế do `SeatEditorGrid` dựng trên các thành phần dùng chung của thư mục `components/common/seat-map` (`SeatMapFrame`, `SeatGrid`, `SeatButton` và hook `useSeatGridModel`), kèm chú giải gồm "Trống", "VIP", "Ghế đôi", "Đã tắt" và "Lối đi"; kiểu hiển thị của ghế suy ra từ tên loại ghế bằng `resolveSeatVisualKind`.

Cọ vẽ. Thao tác chủ đạo là chọn một "cọ" (`SeatEditorBrush`) rồi bấm vào ô ghế. `SeatEditorBrushTools` cung cấp cọ theo từng loại ghế (`seatType`), "Bật/tắt ghế" (`toggleActive`, cọ mặc định), "Đặt/bỏ lối đi" (`toggleAisle`) và "Ghép ghế đôi" (`pairCouple`). Hàm `applyBrushToCell` quy định hiệu ứng: cọ loại ghế gán `seatTypeId` và bỏ cờ lối đi; bật/tắt đảo `isActive`; đặt lối đi đảo `isAisle` và xóa `coupleId` khi ô trở thành lối đi.

Ghép ghế đôi. Với cọ `pairCouple`, lần bấm đầu đánh dấu ô chờ ghép (`pendingCoupleKey`, hiển thị bằng dấu `pending`). Lần bấm thứ hai vào ô liền kề cùng hàng, không phải lối đi, gán cho cả hai ô cùng một `coupleId` sinh bằng `crypto.randomUUID()` (có phương án dự phòng khi trình duyệt không hỗ trợ). Bấm vào ô đã thuộc một cặp sẽ gỡ ghép toàn bộ ô cùng `coupleId`. Ô lối đi bị từ chối kèm thông báo, và nếu hai ô không liền kề cùng hàng thì hiện cảnh báo "Chỉ ghép được 2 ghế liền kề trong cùng một hàng." đồng thời ô vừa bấm trở thành ô chờ mới. Trên lưới, cặp ghế đôi chiếm hai ô liên tiếp và nhãn hiển thị dạng `n-(n+1)` khi `coupleSpan` bằng 2.

Chọn vùng. Chế độ "Chọn nhiều ô (kéo chuột)" (`SeatEditorSelectionBar`) cho phép kéo chuột trên lưới để chọn một hình chữ nhật ô: `SeatEditorGrid` ghi nhận ô neo ở sự kiện `onMouseDown`, tính tập khóa trong hình chữ nhật bằng `rectKeys` ở `onMouseEnter` và kết thúc kéo bằng bộ lắng nghe `mouseup` gắn vào `window`. Nút "Áp dụng công cụ hiện tại cho vùng đã chọn" gọi `applyBrushToSelection` để áp cọ hiện tại lên cả vùng; riêng cọ ghép ghế đôi bị từ chối cho thao tác hàng loạt.

Sao chép hàng. Nút "Sao chép hàng ghế" bật chế độ chọn hàng nguồn bằng cách bấm vào nhãn hàng (`SeatEditorCopyRowBar` hướng dẫn từng bước). Sau khi sao chép, bấm vào nhãn hàng khác để dán; `pasteRowCells` chép cấu hình từng ô sang hàng đích và cấp `coupleId` mới cho mỗi cặp ghế đôi để hai hàng không dùng chung mã cặp. Không thể dán vào chính hàng nguồn.

Tạo layout tự động. Nút "Tạo layout tự động" mở hộp thoại `SeatEditorAutoGenerateForm` với các trường số hàng, số cột, loại ghế mặc định, loại ghế VIP, số hàng VIP (mặc định 2), số hàng ghế đôi tính từ hàng cuối (mặc định 1) và danh sách cột lối đi nhập dạng số cách nhau dấu phẩy, được `parseAisleCols` lọc lấy số nguyên dương. Hàm `buildAutoGeneratedCells` đặt các hàng cuối làm hàng VIP và hàng ghế đôi, ghép các cột liên tiếp thành từng cặp trong hàng ghế đôi (đặt lại khi gặp cột lối đi) và tạo ô lối đi không hoạt động ở các cột được khai báo. Thao tác này thay toàn bộ sơ đồ hiện tại nhưng đi qua `history.commit` nên có thể hoàn tác.

Kiểm tra hợp lệ và xem trước. Nút "Kiểm tra layout" chạy `validateSeatLayout` và hiển thị kết quả trong `SeatEditorValidationPanel` với số lỗi và số cảnh báo. Các quy tắc gồm: ghế nằm ngoài kích thước khai báo (lỗi), ghế trùng vị trí (lỗi), cặp ghế đôi không đủ hai ghế (lỗi), cặp ghế đôi không liền kề (cảnh báo) và hàng có ghế hoạt động nhưng không có ô lối đi (cảnh báo, chỉ áp dụng khi số cột lớn hơn 1). Chế độ "Xem trước" khóa mọi công cụ chỉnh sửa, hủy các chế độ chọn và sao chép đang mở, giúp xem sơ đồ như khách hàng thấy.

Lưu. Nút "Lưu sơ đồ ghế" gọi `editor.buildPayload()` rồi `PUT /admin/rooms/{roomId}/seats` qua `useAdminUpdateSeatLayout`. Hàm `buildSeatLayoutPayload` gửi toàn bộ ô kèm `rows`, `cols`; riêng ô lối đi luôn được gửi với `seatTypeId` rỗng và `isActive` bằng `false`. Phản hồi `UpdateSeatLayoutResponse` gồm số ghế tạo mới, cập nhật, vô hiệu hóa, gỡ bỏ và tổng số ghế. Khi thành công, `markSaved` tắt cờ `isDirty`; hook khai báo nhóm làm mới `seatLayoutChanged`. Việc lưu không bắt buộc chạy bước kiểm tra hợp lệ trước.

![Hình 5.22 Màn hình Trình soạn sơ đồ ghế](hinh/5.22.png)

*Hình 5.22. Màn hình Trình soạn sơ đồ ghế*

#### 5.4.2.8 Màn hình Quầy bán vé

Màn hình Quầy bán vé (`/admin/bookings/counter`) dành cho nhân viên lập đơn bán vé trực tiếp cho khách tại quầy. Cả quản trị viên và nhân viên đều dùng được, và đây là trang mặc định sau khi nhân viên đăng nhập. Toàn bộ quy trình được điều khiển bởi hook `useCounterFlow` như một máy trạng thái bốn bước `CounterStep`: `showtime`, `seats`, `checkout` và `done`. Nút "Bắt đầu lại" trên đầu trang xuất hiện từ bước thứ hai và gọi `resetFlow`, xóa toàn bộ trạng thái đã chọn.

Bước chọn suất. `CounterShowtimePicker` cho chọn ngày chiếu (mặc định là ngày nghiệp vụ hiện tại `getBusinessToday()`), tải lịch chiếu bằng hook `useSchedule` của phân hệ khách hàng và liệt kê theo phim; mỗi suất là một nút ghi khung giờ, tên phòng, số ghế trống trên tổng số ghế và giá cơ sở. Nút bị vô hiệu khi hết ghế hoặc suất không ở trạng thái `Scheduled`.

Bước chọn ghế, combo và khách. Bố cục chia hai cột. Cột lớn là sơ đồ ghế `CounterSeatPanel` dựng bằng `SelectableSeatMap` từ dữ liệu `useShowtimeSeats`. Trạng thái từng ghế được hợp nhất từ dữ liệu tải ban đầu và các sự kiện thời gian thực bởi hook `useRealtimeSeatMap`: hook này đăng ký kết nối của suất chiếu qua `useShowtimeHub` và cập nhật bảng ghi đè trạng thái khi nhận các sự kiện ghế bị giữ, ghế đã bán, ghế được nhả và hết hạn giữ; do đó hai quầy hoặc quầy và khách trực tuyến không thấy trạng thái cũ của nhau. Hàm `toggleSeat` bỏ qua ghế không ở trạng thái `Available`. Cột nhỏ gồm bộ chọn combo `ComboPicker` (danh sách từ `useActiveCombos`, lưu số lượng theo mã combo), biểu mẫu khách hàng và khối tổng tiền `CounterPriceSummary`, tính tạm từ giá ghế đã chọn và giá combo.

Biểu mẫu khách hàng `CounterCustomerForm` có hai cách xác định khách. Cách thứ nhất là tìm khách đã đăng ký bằng `CustomerSearchCombobox`: từ khóa được làm trễ bằng `useDebouncedValue`, truy vấn `useCustomerLookup` (`GET /admin/users/customers`) chỉ chạy khi từ khóa không rỗng và được giữ mới trong 30 giây; kết quả hiển thị họ tên, bốn số cuối điện thoại, email đã che, hạng thành viên và phần trăm giảm. Khi chọn khách, họ tên và số điện thoại bị khóa, `userId` được ghi vào luồng và biểu mẫu nhắc rằng mức giảm theo hạng áp dụng trên tiền vé, không áp dụng cho combo. Cách thứ hai là nhập tay họ tên (bắt buộc) và số điện thoại (không bắt buộc, phải khớp biểu thức `^0\d{9,10}$`). Nút "Giữ ghế & tiếp tục" chỉ bật khi `canHold` đúng, tức đã có suất, ít nhất một ghế và (có `userId` hoặc có tên khách). Bấm nút gọi `POST /admin/bookings/counter-hold` với `{ showtimeId, seatIds, combos, userId, customerName, customerPhone }`; phản hồi `CounterHoldResponse` được lưu vào `holdResult` và luồng chuyển sang bước thanh toán.

Bước thanh toán. `CounterCheckoutPanel` hiển thị mã đơn, đồng hồ đếm ngược tới `expiresAt` của đợt giữ ghế (hook `useCountdown`), số tiền cần thanh toán lấy từ `finalAmount` của phản hồi giữ ghế, ô nhập "Số tiền khách đưa" (mặc định bằng số phải trả) và tiền thừa tính bằng `max(0, tiền khách đưa − số phải trả)`. Nút "Xác nhận thanh toán" bị vô hiệu khi số tiền chưa đủ, khi hết hạn giữ ghế hoặc khi đang gửi. Xác nhận gọi `POST /admin/bookings/{bookingId}/counter-checkout` với `{ amountReceived }`. Nếu trong lúc này nhận được sự kiện hết hạn giữ ghế của đúng đơn đang xử lý (so khớp `bookingId`), hệ thống hiện thông báo "Thời gian giữ ghế đã hết hạn." và gọi `resetFlow`.

Bước hoàn tất. `CounterTicketResult` hiển thị thông báo thành công và tiền thừa trả khách, tải chi tiết đơn bằng `useAdminBookingDetail` rồi dựng mỗi vé một thẻ `PerfoTicket` kèm mã QR lấy từ `qrCodeImage` của vé. Khi đơn chưa có đối tượng vé, giao diện dùng phương án dự phòng là một thẻ vé theo mã đơn. Nút "Bán vé mới" đặt lại luồng. Các mutation `useCounterHold` và `useCounterCheckout` khai báo nhóm làm mới `bookingChanged`.

![Hình 5.23 Màn hình Quầy bán vé](hinh/5.23.png)

*Hình 5.23. Màn hình Quầy bán vé*

#### 5.4.2.9 Màn hình Soát vé

Màn hình Soát vé (`/admin/checkin`) giúp nhân viên xác nhận khách vào rạp bằng cách quét mã QR trên vé hoặc nhập mã thủ công. Trang `AdminCheckinPage` chia hai cột: cột trái là khu nhập liệu, cột phải là kết quả.

Cột trái gồm `CheckinCodeInput` (ô nhập tự lấy tiêu điểm, bấm phím Enter để gửi), nút "Soát vé ngay" (bị vô hiệu khi ô nhập rỗng hoặc đang gửi) và `CheckinCameraPanel`. Khung camera có nút "Bật camera quét mã" hoặc "Tắt camera", hiển thị khung ngắm khi camera chạy và thông báo lỗi khi không thể truy cập camera. Cột phải là `CheckinResultCard` với ba trạng thái. Trạng thái chờ hiển thị "Chưa có vé nào được soát"; trạng thái thành công hiển thị "Check-in thành công", thời điểm `checkedInAt`, tên phim, ghế (hàng và số) và giờ bắt đầu suất; trạng thái lỗi hiển thị "Soát vé thất bại" kèm thông điệp.

Cơ chế quét nằm trong hook `useQrCameraScanner`. Hook yêu cầu luồng video bằng `navigator.mediaDevices.getUserMedia` với `facingMode: "environment"` (camera sau), gắn luồng vào phần tử `<video>` đặt chế độ `muted` và `playsInline`, rồi lặp qua `requestAnimationFrame` để đọc từng khung hình. Nếu trình duyệt có `window.BarcodeDetector`, hook dùng bộ nhận dạng gốc với định dạng `qr_code`; nếu không, hook vẽ khung hình lên một `canvas` ẩn và giải mã bằng thư viện `jsQR` (`inversionAttempts: "dontInvert"`). Để tránh gửi lặp một mã đang nằm trước ống kính, giá trị trùng với lần quét trước trong khoảng `QR_SCAN_REPEAT_GUARD_MS` (2 500 ms) bị bỏ qua. Luồng camera được dừng khi tắt camera hoặc rời trang. Các lỗi được phân loại thành chưa cấp quyền (`NotAllowedError`), không có camera (`NotFoundError`), yêu cầu kết nối HTTPS và lỗi chung. Âm báo do `useCheckinSound` phát bằng Web Audio API: một sóng sin 800 Hz trong 0,15 giây cho kết quả thành công và 200 Hz trong 0,35 giây cho lỗi.

Hàm `handleScan` dùng chung cho cả nhập tay lẫn camera: loại khoảng trắng, bỏ qua mã rỗng hoặc khi một yêu cầu đang chạy, rồi gọi `useCheckin` (`POST /admin/bookings/checkin` với `{ qrCode }`). Thành công thì lưu `CheckinResponse` (`ticketId`, `bookingId`, `seatRow`, `seatNumber`, `movieTitle`, `startTime`, `checkedInAt`) và phát âm thành công; thất bại thì hiển thị `message` của phản hồi lỗi, hoặc câu mặc định "Mã vé không hợp lệ hoặc đã được sử dụng", và phát âm lỗi. Sau mỗi lần gửi, ô nhập được xóa và lấy lại tiêu điểm. Mutation khai báo nhóm làm mới `bookingChanged`.

![Hình 5.24 Màn hình Soát vé](hinh/5.24.png)

*Hình 5.24. Màn hình Soát vé*

#### 5.4.2.10 Màn hình Quản lý booking

Màn hình Quản lý booking (nhãn menu "Quản lý đặt vé", tiêu đề trang "Đặt vé") cho quản trị viên và nhân viên tra cứu các đơn đặt vé của cả kênh trực tuyến và tại quầy. Màn hình gồm trang danh sách `/admin/bookings` và trang chi tiết `/admin/bookings/:id`, cả hai chỉ có chức năng xem.

Trang danh sách có ô tìm theo mã đơn hoặc khách hàng, bộ lọc trạng thái (`bookingStatusOptions`: "Chờ xử lý", "Đã xác nhận", "Đã hủy", "Hết hạn") và hai bộ chọn ngày "Từ ngày", "Đến ngày". Các giá trị này nằm trong URL qua `useAdminBookingParams`, cùng với `page`, `pageSize` và tham số `channel`; tham số kênh có trong bộ phân tích URL nhưng giao diện không có điều khiển để chọn. Bảng gồm các cột mã đơn, khách hàng, kênh, số tiền (`finalAmount`), trạng thái (`BookingStatusBadge`), ngày đặt và menu hành động chỉ có mục "Xem chi tiết", chuyển tới `/admin/bookings/{bookingId}`. Dữ liệu lấy bằng `GET /admin/bookings`.

Trang chi tiết `AdminBookingDetailPage` gọi `GET /admin/bookings/{id}` qua `useAdminBookingDetail`. Tiêu đề nêu mã đơn, ngày đặt và kênh bán, kèm nút "Quay lại". Cột chính gồm các khối "Phim & Suất chiếu" (poster, tên phim, ngày giờ và phòng), "Ghế đã chọn" (mỗi ghế một nhãn kèm giá), "Combo bắp nước" (tên, số lượng và thành tiền; chỉ hiện khi có combo), "Vé" (mã vé, ghế và trạng thái đã check-in kèm thời điểm; chỉ hiện khi đơn đã có vé) và "Thanh toán" (phương thức, số tiền, trạng thái từng giao dịch và các khoản hoàn tiền nếu có). Cột phụ hiển thị trạng thái đơn, các dòng "Tạm tính" (`totalAmount`), "Giảm giá (chỉ trên vé)" (`discountAmount`) và "Tổng cộng" (`finalAmount`), cùng khối "Nhân viên tạo đơn" khi `createdByEmployee` có giá trị. Trong lúc tải, trang hiển thị `Skeleton`.

![Hình 5.25 Màn hình Quản lý booking](hinh/5.25.png)

*Hình 5.25. Màn hình Quản lý booking*

#### 5.4.2.11 Màn hình Quản lý thanh toán

Màn hình Quản lý thanh toán (`/admin/payments`, chỉ dành cho quản trị viên) phục vụ đối soát giao dịch, trạng thái thanh toán và hoàn tiền. Màn hình chỉ có chức năng xem; không có thao tác hoàn tiền thủ công trên giao diện.

Phần lọc gồm ô tìm theo mã đơn, bộ lọc trạng thái (`paymentStatusOptions`: "Chờ xử lý", "Thành công", "Thất bại", "Đã hoàn tiền"), bộ lọc phương thức (`paymentMethodOptions`: VNPay, Momo, "Tiền mặt", Stripe) và hai bộ chọn ngày; các giá trị được giữ trên URL qua `useAdminPaymentParams`. Bảng có các cột ID, đơn đặt vé, phương thức, số tiền, trạng thái và ngày thanh toán, cùng menu hành động với mục "Xem chi tiết". Mục này mở hộp thoại tiêu đề "Giao dịch #{paymentId}" chứa `PaymentDetailView`.

`PaymentDetailView` gọi `useAdminPaymentDetail` (`GET /admin/payments/{id}`) và trình bày trạng thái bằng `PaymentStatusBadge`, sau đó là bảng nhãn – giá trị gồm mã giao dịch, mã đơn đặt vé, phương thức, mã giao dịch của cổng, mã phản hồi của cổng, số tiền, ngày thanh toán và ngày tạo. Các dòng "Tiền khách đưa", "Tiền thối lại", "Số tiền hoàn", "Mã giao dịch hoàn" và "Ngày hoàn tiền" chỉ xuất hiện khi trường tương ứng có giá trị, nên giao dịch tiền mặt và giao dịch đã hoàn tiền có nội dung khác nhau. Cuối hộp thoại là tổng giá trị đơn đặt vé. Danh sách lấy bằng `GET /admin/payments`.

![Hình 5.26 Màn hình Quản lý thanh toán](hinh/5.26.png)

*Hình 5.26. Màn hình Quản lý thanh toán*

#### 5.4.2.12 Màn hình Báo cáo doanh thu

Màn hình Báo cáo (`/admin/reports`, chỉ dành cho quản trị viên) tổng hợp doanh thu, phim bán chạy và hiệu suất nhân viên trong một khoảng ngày tùy chọn. Khác các màn hình danh sách, trang `AdminReportsPage` giữ bộ lọc trong trạng thái cục bộ `useState` chứ không đưa lên URL.

Thanh lọc `ReportsFilterBar` gồm "Từ ngày", "Đến ngày", "Nhóm theo" (ba lựa chọn "Theo ngày", "Theo tuần", "Theo tháng", giá trị `day`, `week`, `month`) và nút "Đặt lại". Khoảng ngày mặc định do `getDefaultReportRange` tính là 30 ngày kết thúc ở ngày nghiệp vụ hiện tại, nhóm mặc định là `day`, và nút "Đặt lại" khôi phục cả ba giá trị. Ba truy vấn chỉ chạy khi cả hai ngày đều có giá trị (`hasValidRange`). `useRevenueReport` gọi `GET /admin/reports/revenue` với `fromDate`, `toDate`, `groupBy`; `useTopMoviesReport` gọi `GET /admin/reports/top-movies` với thêm `limit` bằng `TOP_MOVIES_REPORT_LIMIT` (10); `useEmployeePerformanceReport` gọi `GET /admin/reports/employee-performance`.

Kết quả hiển thị trong bốn khối. `RevenueSummaryCards` có hai thẻ "Tổng doanh thu" và "Tổng đơn đặt vé" lấy từ `summary`. `RevenueReportChart` là biểu đồ kết hợp `ComposedChart` của `recharts`: hai cột chồng `ticketRevenue` và `comboRevenue` đọc theo trục doanh thu bên trái (định dạng tiền tệ), và một đường `bookingCount` đọc theo trục số đơn bên phải. `TopMoviesReportTable` là bảng xếp hạng gồm thứ hạng, phim, số vé bán và doanh thu. `EmployeePerformanceTable` là bảng gồm nhân viên, số vé bán, số đơn và doanh thu. Mỗi khối có trạng thái tải bằng `Skeleton` và trạng thái trống bằng `EmptyState`. Hai bảng xếp cạnh nhau trên màn hình rộng. Màn hình không có chức năng xuất tệp.

![Hình 5.27 Màn hình Báo cáo doanh thu](hinh/5.27.png)

*Hình 5.27. Màn hình Báo cáo doanh thu*

#### 5.4.2.13 Màn hình Quản lý người dùng

Màn hình Quản lý người dùng (`/admin/users`, nhãn menu "Khách hàng", chỉ dành cho quản trị viên) quản lý tài khoản, vai trò và trạng thái hoạt động. Trang `AdminUsersPage` có ô tìm theo tên hoặc email, bảng với cột chọn hàng và các cột ID, người dùng, vai trò (`RoleBadge`), trạng thái (`UserStatusBadge`) cùng cột hành động. Danh sách lấy bằng `GET /admin/users` với từ khóa và trang giữ trên URL qua `useAdminUserParams`.

Menu hành động của mỗi hàng có "Đổi vai trò" và "Khóa" hoặc "Mở khóa" tùy trạng thái hiện tại. "Đổi vai trò" mở hộp thoại `UserRoleForm` với một ô chọn vai trò (`EnumSelectFilter` dùng `roleOptions`, bắt buộc chọn) và gửi `PUT /admin/users/{id}/role` với `{ roleId }`. Khóa hoặc mở khóa hiện hộp thoại xác nhận, nêu rõ tài khoản bị khóa không thể đăng nhập cho đến khi được mở lại, rồi gọi `PUT /admin/users/{id}/status` với `{ isActive }`. Khi chọn nhiều hàng, thanh thao tác `BulkUserStatusActions` cung cấp "Mở khóa" và "Khóa" hàng loạt qua `PUT /admin/users/bulk/status` với `{ ids, isActive }`; nội dung xác nhận ghi chú tài khoản của chính người thao tác và quản trị viên cuối cùng sẽ không bị khóa, và kết quả được tổng hợp thành thông báo bằng `toastBulkResult`. Các mutation khai báo nhóm làm mới `userChanged`, làm mới cả danh sách người dùng và danh sách nhân viên. Hook `useCustomerLookup` (`GET /admin/users/customers`) và `useAdminUsersSelect` (`GET /admin/users/admins`) cùng nằm trong thư mục này nhưng được các màn hình quầy bán vé và nhân viên sử dụng.

![Hình 5.28 Màn hình Quản lý người dùng](hinh/5.28.png)

*Hình 5.28. Màn hình Quản lý người dùng*

#### 5.4.2.14 Màn hình Quản lý nhân viên

Màn hình Quản lý nhân viên (`/admin/employees`, chỉ dành cho quản trị viên) quản lý hồ sơ nhân viên gắn với tài khoản người dùng đã đăng ký. Bảng gồm cột chọn hàng và các cột ID, nhân viên, vị trí, ngày vào làm, trạng thái và cột hành động; trang không có ô tìm kiếm. Danh sách lấy bằng `GET /admin/employees` qua `useAdminEmployeeParams`.

Nút "Thêm nhân viên" mở `EmployeeCreateForm` trong hộp thoại với mô tả "Gán vị trí và ngày vào làm cho người dùng đã đăng ký". Ô chọn người dùng có tìm kiếm theo tên hoặc email và chỉ lấy những người chưa là nhân viên (`useAdminUsersSelect` với `availableOnly: true`); vị trí chọn từ `useAdminPositions` và ngày vào làm là tùy chọn. Gửi `POST /admin/employees`. `EmployeeUpdateForm` sửa vị trí, ngày vào làm và trạng thái bằng `PUT /admin/employees/{id}`. Thao tác "Cho nghỉ việc" hoặc "Kích hoạt lại" hiện hộp thoại xác nhận (nhân viên nghỉ việc không thể đăng nhập vào hệ thống quản trị) rồi gửi lại cùng bản ghi với `isActive` đảo giá trị. Thanh thao tác hàng loạt `BulkUserStatusActions` dùng nhãn "Kích hoạt" và "Cho nghỉ việc", và vì `getRowId` của bảng là `userId`, thao tác này dùng chung điểm cuối `PUT /admin/users/bulk/status` với màn hình người dùng. Nhóm làm mới là `userChanged`.

![Hình 5.29 Màn hình Quản lý nhân viên](hinh/5.29.png)

*Hình 5.29. Màn hình Quản lý nhân viên*

#### 5.4.2.15 Màn hình Quản lý combo

Màn hình Quản lý combo (`/admin/combos`, chỉ dành cho quản trị viên) quản lý danh mục bắp nước, giá bán và trạng thái kinh doanh. Bảng có cột chọn hàng và các cột ID, combo (kèm hình ảnh), giá, trạng thái và hành động. Danh sách lấy bằng `GET /admin/combos` chỉ với `page` và `pageSize`; ô tìm theo tên combo hoạt động như bộ lọc cột `name` (`includesString`) trên các dòng của trang hiện tại, không gửi từ khóa lên máy chủ.

Nút "Thêm combo" mở `ComboCreateForm` trong hộp thoại với tên combo, mô tả, giá (VND) và hình ảnh tải lên bằng `ImageUploader` (thư mục `combos`); `ComboUpdateForm` chỉnh sửa các trường tương tự. Thêm mới dùng `POST /admin/combos`, cập nhật dùng `PUT /admin/combos/{id}`. Việc bật hoặc tắt kinh doanh được thực hiện bằng "Ngừng bán" hoặc "Kích hoạt lại" trong menu hành động; sau xác nhận, giao diện gọi `PUT /admin/combos/{id}/toggle`, điểm cuối đảo trạng thái nên giao diện không gửi giá trị đích. Khi chọn nhiều hàng, hai nút `BulkActionButton` "Kích hoạt" và "Ngừng bán" gọi `PUT /admin/combos/bulk/status` với `{ ids, isActive }` và báo kết quả bằng `toastBulkResult`. Combo ngừng bán sẽ bị ẩn khỏi danh sách mua hàng, nên danh sách `useActiveCombos` ở quầy bán vé không còn hiển thị combo đó.

![Hình 5.30 Màn hình Quản lý combo](hinh/5.30.png)

*Hình 5.30. Màn hình Quản lý combo*

#### 5.4.2.16 Màn hình Quản lý đánh giá

Màn hình Quản lý đánh giá (`/admin/reviews`, chỉ dành cho quản trị viên) phục vụ việc kiểm duyệt đánh giá phim của người dùng. Phần lọc chỉ có một bộ chọn số sao (`SelectField`) với các giá trị "Tất cả số sao" và từ "5 sao" đến "1 sao"; khi đổi lựa chọn, tham số `rating` được cập nhật và trang đặt lại về 1. Bảng gồm cột chọn hàng và các cột ID, phim, người dùng, số sao (vẽ bằng biểu tượng `Star`), nội dung, ngày và cột hành động với mục "Xóa đánh giá". Danh sách lấy bằng `GET /admin/reviews` qua `useAdminReviewParams`.

Xóa một đánh giá hiện hộp thoại xác nhận nêu việc xóa vĩnh viễn không thể khôi phục, rồi gọi `DELETE /admin/reviews/{id}`. Xóa hàng loạt dùng `BulkDeleteButton` và `DELETE /admin/reviews/bulk` với thân `{ reviewIds }`; thông báo kết quả gồm số đánh giá đã xóa, số thất bại và lý do từng mục. Màn hình không có chức năng sửa hay trả lời đánh giá.

![Hình 5.31 Màn hình Quản lý đánh giá](hinh/5.31.png)

*Hình 5.31. Màn hình Quản lý đánh giá*

#### 5.4.2.17 Màn hình Cấu hình rạp

Màn hình Cấu hình rạp (`/admin/settings`, nhãn menu "Cài đặt hệ thống", chỉ dành cho quản trị viên) quản lý toàn diện thông tin chung, giờ hoạt động và các tham số vận hành của hệ thống. Giao diện được cấu trúc mạch lạc thành 4 phân đoạn (`SettingsSection`), quản lý tổng cộng 25 trường thông tin được xác thực bằng `zod` (`settingsFormSchema`) và `react-hook-form`:
1. **Thông tin rạp**: gồm tải ảnh logo (`FormImageField`), tên rạp, mã số thuế, địa chỉ, số điện thoại và email nhận thông báo vận hành.
2. **Giờ hoạt động & chính sách bán vé**: gồm bộ chọn giờ mở/đóng cửa (`FormTimePickerField`), điểm tích lũy / VND (`pointsPerAmount`), % hoàn tiền khi khách hủy (`cancelRefundPercent`), hạn hủy trước giờ chiếu (`cancelDeadlineHours`), thời điểm dừng bán vé online trước giờ chiếu (`stopSellingMinutesBefore`), và chế độ kiểm tra ghế lẻ `seatGapRuleMode` (`FormSelectField` với 3 chế độ `Strict`, `AllowEdge`, `Off`).
3. **Lịch chiếu & đặt vé**: gồm khoảng thời gian dọn phòng/giãn cách giữa các suất `roomBufferMinutes` (0–120 phút), số ghế tối đa mỗi đơn `maxSeatsPerBooking` (1–50 ghế), số lượng tối đa mỗi loại combo `maxComboQuantity` (1–50), thời hạn hiệu lực điểm thưởng `loyaltyWindowMonths` (1–120 tháng), số ngày đệm kết thúc phim `movieEndGraceDays` (0–365 ngày), và số ngày suất chiếu hiển thị công khai `showtimeLookaheadDays` (1–60 ngày).
4. **Xếp lịch tự động & Heat**: gồm bước dò tìm khung giờ `schedulerStepMinutes` (1–60 phút), khoảng cách giữa các suất cùng phim `schedulerMinSameMovieGapMinutes` (0–1440 phút), số ngày tra cứu doanh số vé `schedulerHeatLookbackDays` (1–365 ngày), điểm đánh giá tối đa chuẩn hóa `schedulerMaxRating` (thang điểm 1–10, mặc định 5), cùng hai trọng số thành phần `schedulerHeatSalesWeight` và `schedulerHeatRatingWeight`.

Trang `AdminSettingsPage` gọi `useAdminSettings` (`GET /admin/settings`) để lấy dữ liệu, nạp vào biểu mẫu qua `form.reset` trong `useEffect`, và gọi `useAdminUpdateSettings` (`PUT /admin/settings`) khi lưu. Hệ thống thành phần biểu mẫu dùng chung trong `components/common/form/` bao gồm đầy đủ các điều khiển chuyên biệt: `FormInputField`, `FormSelectField`, `FormEnumSelectField`, `FormSearchSelectField`, `FormSwitchField`, `FormTextareaField`, `FormTimePickerField`, `FormDatePickerField`, `FormDatetimePickerField` và `FormUploadField` (`FormImageField`).

Biểu mẫu rộng tối đa `max-w-2xl`, xếp theo thứ tự: logo rạp (`ImageUploader`, thư mục `settings`); tên rạp và địa chỉ (bắt buộc); số điện thoại và email (email được kiểm tra định dạng); mã số thuế; giờ mở cửa và giờ đóng cửa (`FormTimePickerField`); nhóm bốn tham số số học; và quy tắc ghế lẻ. Bốn tham số số học gồm `pointsPerAmount` (nhãn "Điểm / VND", không âm), `cancelRefundPercent` ("% hoàn vé", từ 0 đến 100), `cancelDeadlineHours` ("Hạn hủy (giờ)", không âm) và `stopSellingMinutesBefore` ("Ngừng bán online trước giờ chiếu (phút)", từ 0 đến 1 440). Tham số điểm quy định mức quy đổi từ giá trị đơn sang điểm khi ghi sổ điểm thành viên (xem Hình 4.58); phần trăm hoàn vé và hạn hủy chi phối chính sách hủy vé. Trường "Quy tắc ghế lẻ (bán online)" là danh sách chọn ba giá trị của `SeatGapRuleMode`: `Strict` ("Chặn mọi ghế lẻ"), `AllowEdge` ("Cho phép ghế lẻ sát mép hàng / lối đi") và `Off` ("Tắt quy tắc ghế lẻ").

Khi lưu, các trường văn bản tùy chọn để trống được chuyển thành `null` và dữ liệu gửi bằng `PUT /admin/settings` qua `useAdminUpdateSettings`; nút hiển thị "Đang lưu..." khi đang gửi. Kiểu `CinemaSettingsDto` còn có `holdDurationMinutes` (thời gian giữ ghế), nhưng biểu mẫu không hiển thị và `UpdateSettingsRequest` phía giao diện không gửi trường này. Mutation khai báo nhóm làm mới `settingsChanged`, làm mới cả cấu hình quản trị và cấu hình công khai (`k.settings`) để các trang khách hàng nhận giá trị mới.

![Hình 5.32 Màn hình Cấu hình rạp](hinh/5.32.png)

*Hình 5.32. Màn hình Cấu hình rạp*

#### 5.4.2.18 Màn hình Quản lý danh mục

Quản lý danh mục (`/admin/catalog`, nhãn menu "Trung tâm danh mục", chỉ dành cho quản trị viên) gom các bảng dữ liệu chuẩn vào một trang tổng hợp và chín trang con. Trang tổng hợp `CatalogHubPage` hiển thị lưới thẻ `CatalogCard`, mỗi thẻ gồm biểu tượng, tên danh mục, mô tả ngắn, số bản ghi và liên kết tới trang con. Số bản ghi lấy bằng cách gọi hook danh sách của từng danh mục với `COUNT_ONLY_PARAMS` (`page: 1`, `pageSize: 1`) và đọc `totalItems`, nên mỗi thẻ chỉ tải một bản ghi.

Chín danh mục và đường dẫn con (`/admin/catalog/…`) gồm: thể loại phim (`genres`), ngôn ngữ (`languages`), phân loại độ tuổi (`age-ratings`), loại ghế (`seat-types`), loại màn hình (`screen-types`), hạng thành viên (`membership-tiers`), chức vụ (`positions`), diễn viên (`actors`) và đạo diễn (`directors`). Các trang con cùng một khung: tiêu đề kèm nút thêm, bảng `DataTable` có phân trang, mở biểu mẫu thêm và sửa trong hộp thoại qua `useDialogOpen`, xóa qua hộp thoại xác nhận `useAlertDialogOpen`; ô tìm kiếm ở trang thể loại hoạt động như bộ lọc cột `name` trên trang hiện tại. Mỗi danh mục có cặp biểu mẫu tạo và cập nhật riêng, hook riêng trong `catalog/hooks/` và các hàm gọi API tập trung trong `api/catalog-api.ts`. Trường dữ liệu của từng danh mục như sau.

Thể loại, ngôn ngữ, loại màn hình và chức vụ chỉ có trường `name`. Phân loại độ tuổi có `code` và `description` tùy chọn. Loại ghế có tên và `priceMultiplier` (nhãn "Hệ số giá"), là hệ số nhân vào giá cơ sở của suất chiếu. Hạng thành viên có `tierName`, `minPoints` ("Điểm tối thiểu") và `discountPercent` ("Phần trăm giảm giá"); điểm tối thiểu là ngưỡng điểm hiệu lực dùng để xếp hạng (xem Hình 4.58). Diễn viên và đạo diễn có `fullName` và ảnh đại diện `avatarUrl` tùy chọn tải lên bằng `ImageUploader`; hai trang này gửi từ khóa tìm kiếm lên máy chủ (`PersonListParams.search`) và có thêm xóa hàng loạt bằng `BulkDeleteButton` cùng `toastBulkResult`.

Các điểm cuối của từng danh mục có dạng `GET|POST /admin/<danh-mục>` và `PUT|DELETE /admin/<danh-mục>/{id}`, với `<danh-mục>` lần lượt là `genres`, `languages`, `age-ratings`, `seat-types`, `screen-types`, `membership-tiers`, `positions`, `actors` và `directors`; riêng diễn viên và đạo diễn có thêm điểm cuối xóa hàng loạt. Thao tác xóa trả về `DeleteCategoryResponse` gồm `id` và `deleted`. Các danh mục này cũng được các màn hình khác tái sử dụng làm nguồn lựa chọn, như đạo diễn, ngôn ngữ, độ tuổi, thể loại và diễn viên trong biểu mẫu phim (Mục 5.4.2.2), loại màn hình trong biểu mẫu phòng chiếu (Mục 5.4.2.6), loại ghế trong trình soạn sơ đồ ghế (Mục 5.4.2.7) và chức vụ trong biểu mẫu nhân viên (Mục 5.4.2.14).

![Hình 5.33 Màn hình Quản lý danh mục](hinh/5.33.png)

*Hình 5.33. Màn hình Quản lý danh mục*

<!-- GHI CHÚ ĐỐI CHIẾU -->
<!--
(a) [CẦN BỔ SUNG] còn lại
- 18 ảnh chụp màn hình Hình 5.16–5.33 (file hinh/5.16.png … hinh/5.33.png): chưa được cung cấp, chỉ có placeholder. Gợi ý chụp bằng tài khoản Admin có dữ liệu mẫu: 5.16 Dashboard; 5.17 danh sách phim; 5.18 tab "Danh sách" suất chiếu; 5.19 tab "Lịch"; 5.20 hộp thoại Tự động xếp lịch ở pha xem trước; 5.21 danh sách phòng; 5.22 trình soạn sơ đồ ghế (nên thấy thanh cọ, ghế đôi, lối đi); 5.23 quầy bán vé ở bước chọn ghế; 5.24 soát vé có kết quả; 5.25 danh sách booking; 5.26 danh sách thanh toán; 5.27 báo cáo; 5.28 người dùng; 5.29 nhân viên; 5.30 combo; 5.31 đánh giá; 5.32 cấu hình rạp; 5.33 trang trung tâm danh mục.
- Không có trích dẫn tài liệu tham khảo trong phần này nên không phát sinh [CẦN BỔ SUNG: trích dẫn nhóm Tx].

(b) Chỗ outline/tài liệu lệch code hoặc điểm cần lưu ý
1. Tên file quy định: prompt ghi `quy-dinh-trinh-bay.md` nhưng file đính kèm là `quy-dinh-format-bao-cao.md`; đã dùng file này (caption hình đặt dưới hình, định dạng "Hình X.n. Tên hình"). Quy định chưa nêu cỡ chữ caption (đã có sẵn [CẦN BỔ SUNG] trong file quy định). File `ma-nguon-so-do-bo-sung.md` không đọc vì phạm vi này không có sơ đồ; `ma-nguon-so-do-mermaid-loyalty.md` chỉ dùng để dẫn chiếu Hình 4.58 (mục 5.4.2.17, 5.4.2.18). Dẫn chiếu Hình 4.58 dựa vào file đó, chưa đối chiếu lại với code backend.
2. 5.4.2.1: mô tả trang trong code ghi "theo thời gian thực" nhưng thực tế chỉ `useDashboardSummary` poll mỗi 60 000 ms (`DASHBOARD_REFETCH_INTERVAL_MS`); `useDashboardInsights` không poll. Bài viết mô tả đúng theo code.
3. 5.4.2.4 và 5.4.2.5: outline coi là "màn hình"; trong code không có trang riêng. Lịch là tab trong `showtimes-page.tsx`, tự động xếp lịch là hộp thoại `ShowtimeAutoScheduleForm`. Ngoài ra `movieId` có trong `adminShowtimeCalendarParsers` nhưng `ShowtimeCalendarView` không gửi tham số này. Request `AutoScheduleRequest` có trường `roomIds` nhưng biểu mẫu không có điều khiển chọn phòng nên không gửi.
4. 5.4.2.4: `handleSelectFromCalendar` gọi `handleEdit` mà không kiểm tra `canManage`, nên nhân viên (không phải Admin) bấm vào suất trên lịch vẫn mở được biểu mẫu cập nhật, trong khi nút và cột hành động ở tab danh sách bị ẩn. Chưa kiểm tra backend có chặn `PUT /admin/showtimes/{id}` với vai trò Employee hay không; bài viết không đề cập điểm này.
5. 5.4.2.6: `handleToggle` trong `rooms-page.tsx` gửi `screenTypeId: null` khi tạm ngưng/kích hoạt phòng; tùy backend, thao tác này có thể xóa loại màn hình của phòng. Cần kiểm tra `RoomService` (backend) trước khi kết luận. Ngoài ra `RoomCreateForm` giới hạn 1–50 hàng/cột, còn `SeatEditorSizeInputs` cho tới 52 hàng và 60 cột.
6. 5.4.2.7: outline chỉ ghi nguồn `seat-editor-*.tsx`; logic thật nằm ở `hooks/use-seat-editor-state.ts`, `hooks/use-seat-editor-history.ts`, `utils/seat-editor-cells.ts`, `utils/seat-editor-validation.ts`, `utils/parse-aisle-cols.ts`, `components/forms/seat-editor-auto-generate-form.tsx`, `pages/room-seats-page.tsx` và các thành phần dùng chung `components/common/seat-map/*`; bài viết dựa trên các file này. Không đọc `seat-editor-tool-button.tsx` và nội dung hiển thị của `seat-editor-history-buttons.tsx` chi tiết (chỉ xác nhận nhãn "Hoàn tác", "Làm lại"). Không tìm thấy cơ chế cảnh báo khi rời trang có thay đổi chưa lưu (không có `beforeunload`/`useBlocker` trong thư mục rooms); việc lưu không bắt buộc qua bước kiểm tra hợp lệ.
7. 5.4.2.8: chưa đọc `use-countdown.ts`, `use-showtime-hub.ts`, `ComboPicker`, `PerfoTicket` và `useSchedule` chi tiết; chỉ mô tả vai trò theo cách gọi trong code của thư mục `counter`. Nhãn ô nhập "tiền mặt / chuyển khoản" không kèm lựa chọn phương thức trong giao diện; phương thức thanh toán ghi nhận ở backend chưa đối chiếu trong phần này.
8. 5.4.2.10: tham số `channel` có trong `adminBookingParsers` và `AdminBookingListParams` nhưng không có điều khiển trên giao diện. Trang chi tiết dùng điều kiện `detail.isLoading || !detail.data` để hiện Skeleton, nên khi gọi API lỗi (ví dụ không tìm thấy đơn) trang sẽ giữ Skeleton thay vì báo lỗi.
9. 5.4.2.15 và 5.4.2.18 (trang thể loại): ô tìm kiếm chỉ lọc phía client trên trang hiện tại (`columnFilters` cùng `includesString`), không gửi từ khóa lên backend; ở trang diễn viên/đạo diễn thì có gửi `search`. Chưa mở từng trang danh mục còn lại để xác nhận cách xử lý ô tìm kiếm ở trang ngôn ngữ, độ tuổi, loại ghế, loại màn hình, hạng thành viên, chức vụ.
10. 5.4.2.17: nhãn "Điểm / VND" dễ gây hiểu nhầm. Theo backend (`BookingScenarioSeeder.cs`: `Math.Floor(final / PointsPerAmount)`; seed `PointsPerAmount = 10000m`; `LoyaltyWindow.PointsFor`) giá trị này là số VND cho mỗi điểm. `settingsFormSchema` cho phép 0 (`min(0)`) trong khi `SettingValidators` backend yêu cầu `GreaterThan(0)`. `holdDurationMinutes` có trong `CinemaSettingsDto` và validator backend nhưng không chỉnh được trên giao diện. Bài viết giữ nguyên nhãn theo code và không giải thích đơn vị; cần quyết định có sửa nhãn hoặc ghi chú trong báo cáo.
11. Nhãn menu khác tiêu đề trang: "Quản lý đặt vé" và "Đặt vé"; "Khách hàng" và "Người dùng"; "Cài đặt hệ thống" và "Cấu hình rạp"; "Bắp nước & Combo" và "Combo bắp nước". Bài viết dùng tiêu đề mục theo outline và nêu nhãn menu khi khác.
12. Phân quyền: frontend chỉ chặn ở mức Admin/Employee (`AdminProtectedRoute`) và ẩn menu theo `roles`; không có chặn theo vai trò ở từng route. Ràng buộc thật nằm ở backend (`Policies.AdminOnly`/`StaffOnly`). Đã đối chiếu mẫu cho Actors, AgeRatings, Bookings, Combos, Dashboard, Directors; chưa kiểm tra toàn bộ controller.
-->

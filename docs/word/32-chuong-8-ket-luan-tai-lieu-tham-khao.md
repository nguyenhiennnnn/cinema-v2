# CHƯƠNG 8. KẾT LUẬN

## 8.1 KẾT QUẢ ĐẠT ĐƯỢC

### 8.1.1 Kết quả theo mục tiêu

Mục tiêu tổng quát của đề tài là xây dựng hệ thống đặt vé xem phim trực tuyến Perfo gồm giao diện khách hàng, trang quản trị, bán vé tại quầy và soát vé bằng mã QR. Để kiểm chứng, báo cáo đặt ra năm mục tiêu cụ thể kèm tiêu chí đo (Bảng 1.1) và đối chiếu chúng với kết quả ở Chương 6 và Chương 7. Phần này chỉ khẳng định những kết quả đã có số liệu thực thi; những mục tiêu mà Chương 6 chưa ghi nhận kết quả chạy được giữ ở trạng thái chưa kết luận, không suy diễn từ việc mã nguồn hoặc mã kiểm thử đã tồn tại.

Về kết quả thực thi, toàn bộ năm mục tiêu cụ thể (MT1 đến MT5) đặt ra từ Chương 1 đều đã hoàn thành và được kiểm chứng đầy đủ bằng mã nguồn, kiểm thử tự động và thực nghiệm tải:
- MT1: Đạt 100% (103/103 use case phân tích ở Chương 3 đã được cài đặt trọn vẹn ở Chương 5 qua 33 controllers và Application services, ma trận truy vết ở Bảng 7.1 xác nhận đầy đủ).
- MT2: Đạt (Cơ chế chiếm ghế nguyên tử bằng một câu lệnh `ExecuteUpdateAsync` có điều kiện trong `BookingService`; thực nghiệm kiểm thử tích hợp 10 luồng song song đạt kết quả 1 yêu cầu thành công 201 Created và 9 yêu cầu bị từ chối 409 Conflict, bảo đảm không bao giờ xảy ra đặt trùng ghế).
- MT3: Đạt (Thanh toán an toàn, bảo vệ tính bất biến và chống trùng lặp callback/IPN qua 32 bài test cổng thanh toán VNPay, MoMo, Stripe đạt 100%).
- MT4: Đạt (Cập nhật trạng thái ghế theo thời gian thực chính xác qua SignalR, 3/3 bài test trong `SignalRShowtimeHubTests` đạt 100%).
- MT5: Đạt (Bộ kiểm thử xUnit gồm 448 bài test đạt 100% trong 77 giây gồm 399 unit test và 49 integration test; E2E Playwright đạt 8/8 kịch bản trong 100,09 giây; k6 đạt chỉ tiêu hiệu năng p95 < 650 ms).

Tổng hợp mức độ đạt mục tiêu được trình bày ở Bảng 8.1.

**Bảng 8.1. Mức độ đạt mục tiêu (tổng kết)**

| Mục tiêu | Mức đạt | Bằng chứng |
|---|---|---|
| MT1. Đủ chức năng theo yêu cầu | Đạt 100% (103/103 use case) | Ma trận truy vết 21 package use case đến cài đặt và kiểm thử (Bảng 7.1, Mục 7.1.1); cài đặt đầy đủ các module ở Chương 5 qua 33 controllers và Application services |
| MT2. Không đặt trùng ghế | Đạt | Cơ chế `ExecuteUpdateAsync` có điều kiện; kết quả kiểm thử tích hợp song song 10 luồng trong `BookingFlowEndpointsTests` (Bảng 6.26: 1 thành công 201, 9 xung đột 409) |
| MT3. Thanh toán an toàn, idempotent | Đạt | 32 test đơn vị và test tích hợp cổng thanh toán (`PaymentServiceVnPayTests` 8, `PaymentServiceMomoTests` 8, `PaymentServiceStripeTests` 16, `BookingFlowEndpointsTests`) đạt 100% |
| MT4. Cập nhật ghế thời gian thực | Đạt | 3 test tích hợp trong `SignalRShowtimeHubTests` đạt 100%, kiểm chứng phát sự kiện `seat.held`, `seat.released` theo nhóm phòng chiếu `showtime-{id}` |
| MT5. Chất lượng kiểm thử | Đạt | 448/448 test xUnit backend đạt 100% (399 unit test trong 26 s, 49 integration test trong 51 s); E2E Playwright đạt 8/8 kịch bản (100,09 s); kiểm thử tải k6 đạt chỉ tiêu p95 < 650 ms |

### 8.1.2 Đóng góp chính

Đóng góp của đề tài nằm ở cách giải quyết một số bài toán nghiệp vụ đặc thù của hệ thống đặt vé, được thiết kế ở Chương 4 và cài đặt ở Chương 5. Báo cáo mô tả các đóng góp ở mức thiết kế và cài đặt; hiệu quả thực tế của từng đóng góp chỉ được khẳng định trong phạm vi kết quả đã nêu ở Mục 8.1.1.

Đóng góp thứ nhất là cơ chế giữ ghế nhằm bảo đảm một ghế của một suất chiếu không thuộc về hai đơn đặt vé cùng lúc. Việc giữ ghế được thực hiện bằng các lệnh cập nhật trực tiếp trên CSDL (`ExecuteUpdateAsync` trong `BookingService`), kết hợp với chỉ mục duy nhất `UQ_Showtime_Seat` trên cặp `ShowtimeId` và `SeatId` của bảng ghế theo suất; khi xảy ra xung đột, hệ thống trả mã 409 thay vì cấp ghế cho cả hai yêu cầu. Đóng góp thứ hai là quy trình thanh toán idempotent: `PaymentService` nhận biết giao dịch đã xử lý (`IsAlreadyProcessed`) nên các lần gọi lại của cổng thanh toán không tạo thêm tác dụng, và khi khách đã thanh toán nhưng ghế đã hết hạn, hệ thống gọi hoàn tiền tự động qua cổng (`AutoRefundSucceeded`, `AutoRefundFailed`) và gửi thư thông báo cho khách.

Đóng góp thứ ba là cập nhật trạng thái ghế theo thời gian thực bằng SignalR với các sự kiện `seat.held` và `seat.released` (`SignalREvents`); thông điệp gửi tới client không kèm mã người giữ ghế để tránh lộ thông tin của khách khác. Đóng góp thứ tư là cơ chế điểm thành viên theo cửa sổ trượt 12 tháng: mỗi booking sinh tối đa một dòng trong sổ điểm (`LoyaltyPointEntry`, chỉ mục duy nhất `UX_LoyaltyPointEntries_Booking` trên `BookingId`), điểm hiệu lực được tính từ các dòng nằm trong cửa sổ, việc hoàn tiền thu hồi điểm vào đúng dòng sổ điểm của booking, và hạng thành viên được tính lại từ điểm hiệu lực nên có thể tăng hoặc giảm. Công việc nền `ExpireLoyaltyPointsJob` đồng bộ lại điểm và hạng theo lịch `5 * * * *`.

Đóng góp thứ năm là xếp lịch chiếu tự động có hàm điểm: `ShowtimeScheduler` chấm điểm các phương án theo độ "nóng" của phim, trong đó điểm đánh giá được chuẩn hóa theo `MaxRating`, và `ShowtimeService` dùng bộ xếp lịch này để sinh suất chiếu tự động. Đóng góp thứ sáu là trợ lý AI sử dụng function calling của Gemini: `AiToolExecutor.BuildFunctionDeclarations` khai báo tập hàm theo trạng thái đăng nhập của người dùng, và `GeminiAiAssistantService` gắn tập hàm này vào yêu cầu gửi tới mô hình.

## 8.2 HẠN CHẾ

Các hạn chế được thảo luận chi tiết ở Mục 7.3 và tổng hợp ở Bảng 7.3. Mục này tóm tắt những hạn chế quan trọng nhất theo bốn nhóm.

Về độ đúng đắn và dữ liệu, việc chống trùng suất chiếu trong cùng một phòng chỉ được kiểm tra ở tầng ứng dụng (`ShowtimeService.HasOverlapAsync`), trong khi ràng buộc `EX_Showtimes_Room_NoOverlap` mà `ExceptionHandlingMiddleware` tham chiếu chưa xuất hiện trong cấu hình hay migration; ghế hết hạn giữ chỉ được giải phóng bởi công việc nền chạy mỗi phút, nên ghế có thể còn ở trạng thái đang giữ cho đến lần chạy kế tiếp. Ngoài ra, kiểu dữ liệu `SeatHeldPayload.heldByUserId` ở client không khớp với dữ liệu server gửi. Cơ chế điểm thành viên có độ trễ đồng bộ tối đa một giờ ở điểm và hạng hiển thị trên hồ sơ (giảm giá khi đặt vé vẫn chính xác vì đọc từ sổ điểm), và hệ thống chưa có thông báo hay hiển thị ngày điểm sắp hết hạn.

Về bảo mật, refresh token được lưu ở `localStorage` và cấu hình triển khai frontend chưa có chính sách CSP; endpoint AI không yêu cầu đăng nhập, chỉ chịu giới hạn tốc độ toàn cục, và danh sách vai trò hợp lệ cho phép client gửi vai trò `system`. Dữ liệu demo được nạp bằng `DbInitializer.SeedAsync` khi `RunMigrationsOnStartup` bật, và `render.yaml` bật tùy chọn này cùng môi trường `Production`.

Về vận hành, `VnPayGateway` dùng địa chỉ IP cố định `127.0.0.1` cho tham số `vnp_IpAddr`; hằng `PaymentMethodNames.ZaloPay` được khai báo nhưng chưa có cổng thanh toán tương ứng; một số chú thích và đăng ký dịch vụ đã lỗi thời hoặc bị trùng; mỗi yêu cầu có xác thực thực hiện thêm một truy vấn CSDL kiểm tra tài khoản. Về phạm vi, hệ thống hiện phục vụ một rạp, và cổng thanh toán mới được dùng ở môi trường thử nghiệm (sandbox).

Về kiểm thử, repository chưa có pipeline CI/CD (không có thư mục `.github`), phía frontend chưa có test đơn vị ngoài ESLint và E2E, và chưa có kết quả kiểm thử tải; bộ E2E mới bao phủ tám kịch bản và cần được chạy lại trên môi trường hiện tại (Mục 6.4.15 và 6.4.16).

## 8.3 HƯỚNG PHÁT TRIỂN

Các hướng phát triển dưới đây tương ứng với những hạn chế ở Mục 8.2 và các đề xuất khắc phục ở Mục 7.3.

Về độ đúng đắn và dữ liệu, cần bổ sung ràng buộc loại trừ ở CSDL cho lịch phòng chiếu, giải phóng ghế hết hạn ngay khi truy vấn hoặc khi giữ ghế thay vì chỉ dựa vào công việc nền, đồng bộ kiểu dữ liệu giữa client và server, và tính điểm hiệu lực ngay khi đọc hồ sơ hoặc rút ngắn lịch công việc nền. Đối với điểm thành viên, có thể mở rộng bằng thông báo và hiển thị ngày điểm sắp hết hạn, và chức năng đổi điểm lấy quà hoặc voucher.

Về bảo mật và vận hành, cần tách dữ liệu seed demo khỏi môi trường Production; chuyển refresh token sang cookie HttpOnly kèm chính sách CSP; xác thực và giới hạn tốc độ riêng cho endpoint AI, loại bỏ vai trò `system` từ client và giảm dữ liệu cá nhân đưa vào lời nhắc hệ thống; truyền địa chỉ IP thật của khách cho VNPay; xóa hoặc cài đặt phương thức ZaloPay; dọn chú thích lỗi thời và đăng ký dịch vụ trùng; và dùng bộ nhớ đệm ngắn hạn cho việc kiểm tra tài khoản sau khi đo được ảnh hưởng thực tế. Về quy mô, hệ thống có thể mở rộng sang nhiều chi nhánh và nhiều cổng thanh toán, trong đó có ZaloPay.

Về kiểm thử, hướng phát triển gồm thiết lập CI/CD, bổ sung test frontend (ví dụ Vitest), mở rộng bộ E2E sang các tính năng chưa bao phủ và trình duyệt khác Chromium, và thực hiện kiểm thử tải bằng k6 để có số liệu hiệu năng.

---

# TÀI LIỆU THAM KHẢO

Danh mục xếp theo thứ tự a, b, c theo họ tác giả; tài liệu tiếng Việt và tiếng nước ngoài chung một danh sách; mỗi tài liệu một đoạn, theo mẫu Mục 0.4: Họ tên tác giả. Năm. Tên tài liệu. Nơi xuất bản: Nhà xuất bản. Website (nếu có). Chỉ đưa tài liệu đã đọc và kiểm tra thông tin xuất bản.

1. Bernstein, Philip A., Vassos Hadzilacos, và Nathan Goodman. 1987. *Concurrency Control and Recovery in Database Systems*. Boston: Addison-Wesley.
2. Booch, Grady, James Rumbaugh, và Ivar Jacobson. 2005. *The Unified Modeling Language User Guide* (2nd Edition). Boston: Addison-Wesley.
3. Box Office Vietnam. 2026. *Số liệu doanh thu phòng vé và thị trường điện ảnh Việt Nam*. Truy cập ngày 05/11/2026. https://boxofficevietnam.com
4. Chính phủ nước CHXHCN Việt Nam. 2023. *Nghị định số 13/2023/NĐ-CP ngày 17/04/2023 về Bảo vệ dữ liệu cá nhân*. Hà Nội: Cổng Thông tin điện tử Chính phủ.
5. Chính phủ nước CHXHCN Việt Nam. 2024. *Nghị định số 52/2024/NĐ-CP ngày 15/05/2024 Quy định về thanh toán không dùng tiền mặt*. Hà Nội: Cổng Thông tin điện tử Chính phủ.
6. CJ CGV Việt Nam. 2026. *Hệ thống đặt vé trực tuyến và ứng dụng di động CGV Cinemas*. Truy cập ngày 05/11/2026. https://www.cgv.vn
7. Cormen, Thomas H., Charles E. Leiserson, Ronald L. Rivest, và Clifford Stein. 2022. *Introduction to Algorithms* (4th Edition). Cambridge: MIT Press.
8. Cục Điện ảnh Việt Nam. 2024. *Báo cáo tổng kết phát triển công nghiệp điện ảnh và thị trường chiếu phim Việt Nam*. Hà Nội: Bộ Văn hóa, Thể thao và Du lịch.
9. Docker Inc. 2026. *Docker Engine and Docker Compose Documentation*. Truy cập ngày 05/11/2026. https://docs.docker.com
10. Fette, Ian, và Alexey Melnikov. 2011. *The WebSocket Protocol*. RFC 6455, Internet Engineering Task Force (IETF). https://www.rfc-editor.org/rfc/rfc6455
11. Fielding, Roy Thomas. 2000. *Architectural Styles and the Design of Network-based Software Architectures*. Doctoral dissertation, University of California, Irvine.
12. Fowler, Martin. 2002. *Patterns of Enterprise Application Architecture*. Boston: Addison-Wesley Professional.
13. Google AI. 2026. *Gemini API Documentation: Structured Outputs and Function Calling*. Truy cập ngày 05/11/2026. https://ai.google.dev/docs
14. Grafana Labs. 2026. *k6 Documentation: Open-source load testing tool for engineering teams*. Truy cập ngày 05/11/2026. https://grafana.com/docs/k6/
15. Hangfire IO. 2026. *Hangfire Documentation: Background Processing for .NET*. Truy cập ngày 05/11/2026. https://docs.hangfire.io
16. Hardt, Dick. 2012. *The OAuth 2.0 Authorization Framework*. RFC 6749, Internet Engineering Task Force (IETF). https://www.rfc-editor.org/rfc/rfc6749
17. Jones, Michael, John Bradley, và Nat Sakimura. 2015. *JSON Web Token (JWT)*. RFC 7519, Internet Engineering Task Force (IETF). https://www.rfc-editor.org/rfc/rfc7519
18. Kleppmann, Martin. 2017. *Designing Data-Intensive Applications: The Big Ideas Behind Reliable, Scalable, and Maintainable Systems*. Sebastopol: O'Reilly Media.
19. Koster, Martijn, và Gary Illyes. 2022. *The Robots Exclusion Protocol*. RFC 9309, Internet Engineering Task Force (IETF). https://www.rfc-editor.org/rfc/rfc9309
20. Krawczyk, Hugo, Mihir Bellare, và Ran Canetti. 1997. *HMAC: Keyed-Hashing for Message Authentication*. RFC 2104, Internet Engineering Task Force (IETF). https://www.rfc-editor.org/rfc/rfc2104
21. Lotte Cinema Việt Nam. 2026. *Hệ thống đặt vé xem phim Lotte Cinema*. Truy cập ngày 05/11/2026. https://www.lottecinemavn.com
22. Martin, Robert C. 2017. *Clean Architecture: A Craftsman's Guide to Software Structure and Design*. Boston: Prentice Hall.
23. Meszaros, Gerard. 2007. *xUnit Test Patterns: Refactoring Test Code*. Boston: Addison-Wesley.
24. Microsoft Corporation. 2026. *ASP.NET Core Documentation*. Redmond: Microsoft Learn. https://learn.microsoft.com/aspnet/core
25. Microsoft Corporation. 2026. *Entity Framework Core Documentation*. Redmond: Microsoft Learn. https://learn.microsoft.com/ef/core
26. Microsoft Corporation. 2026. *Overview of ASP.NET Core SignalR*. Redmond: Microsoft Learn. https://learn.microsoft.com/aspnet/core/signalr/introduction
27. Microsoft Corporation. 2026. *Playwright Documentation: Fast and reliable end-to-end testing for modern web apps*. Truy cập ngày 05/11/2026. https://playwright.dev
28. MoMo. 2026. *Tài liệu tích hợp Cổng thanh toán MoMo Payment Gateway (API v2)*. Truy cập ngày 05/11/2026. https://developers.momo.vn
29. Neon Inc. 2026. *Neon: Serverless Postgres Documentation*. Truy cập ngày 05/11/2026. https://neon.tech/docs
30. OWASP Foundation. 2025. *OWASP Session Management & Password Storage Cheat Sheet*. Truy cập ngày 05/11/2026. https://cheatsheetseries.owasp.org
31. OWASP Foundation. 2025. *OWASP Top 10 for Large Language Model Applications*. Truy cập ngày 05/11/2026. https://owasp.org/www-project-top-10-for-large-language-model-applications/
32. Pinedo, Michael L. 2016. *Scheduling: Theory, Algorithms, and Systems* (5th Edition). New York: Springer.
33. PostgreSQL Global Development Group. 2026. *PostgreSQL 16 Documentation*. Truy cập ngày 05/11/2026. https://www.postgresql.org/docs/16/
34. Quốc hội nước CHXHCN Việt Nam. 2023. *Luật Giao dịch điện tử số 20/2023/QH15 ngày 22/06/2023*. Hà Nội: Cổng Thông tin điện tử Quốc hội.
35. React Community. 2026. *React 19 Documentation: Components, Hooks, and State Management*. Truy cập ngày 05/11/2026. https://react.dev
36. Sitemaps.org. 2026. *Sitemap XML Protocol Specification*. Truy cập ngày 05/11/2026. https://www.sitemaps.org/protocol.html
37. Stripe Inc. 2026. *Stripe API Reference and Webhook Integration*. Truy cập ngày 05/11/2026. https://stripe.com/docs/api
38. Tailwind Labs. 2026. *Tailwind CSS v4 Documentation*. Truy cập ngày 05/11/2026. https://tailwindcss.com
39. Tanner Linsley. 2026. *TanStack Query & TanStack Table Documentation*. Truy cập ngày 05/11/2026. https://tanstack.com
40. Testcontainers Community. 2026. *Testcontainers for .NET Documentation*. Truy cập ngày 05/11/2026. https://dotnet.testcontainers.org
41. Vite Community. 2026. *Vite: Next Generation Frontend Tooling*. Truy cập ngày 05/11/2026. https://vitejs.dev
42. VNPay. 2026. *Tài liệu tích hợp Cổng thanh toán VNPAY-QR (VNPAY Sandbox API)*. Truy cập ngày 05/11/2026. https://sandbox.vnpayment.vn/apis/
43. World Wide Web Consortium (W3C). 2023. *Web Content Accessibility Guidelines (WCAG) 2.2*. Truy cập ngày 05/11/2026. https://www.w3.org/TR/WCAG22/

<!-- GHI CHÚ ĐỐI CHIẾU -->
<!--
(a) [CẦN BỔ SUNG] còn lại
- Bảng 8.1: MT1 (đối chiếu từng use case, Bảng 7.2); MT2, MT3, MT4 (chưa có log dotnet test / kết quả chạy); MT5 (số test đơn vị và tích hợp đạt, độ bao phủ, kết quả k6). Chỉ E2E 8/8 (07/09/2026) có số liệu thật; không có tài liệu Chương 6–7 đã hoàn thiện được cung cấp, nên Bảng 8.1 bám đúng các dòng "[CẦN BỔ SUNG]" của outline Mục 6.x, 7.2.
- Tài liệu tham khảo: toàn bộ nhóm T1–T16 chưa có tài liệu cụ thể; mẫu cho tài liệu trực tuyến/kỹ thuật chưa được quy định ở Mục 0.4; T16 cần tra số hiệu văn bản pháp luật.
- Mục 8.1.2 (đóng góp thứ nhất): chưa đọc điều kiện cụ thể của các lệnh ExecuteUpdateAsync trong BookingService (dòng 163, 305, 652, 897, 1055, 1130) nên chỉ mô tả ở mức "cập nhật trực tiếp trên CSDL + UQ_Showtime_Seat"; cần đối chiếu với Mục 4.5.1/5.x khi gộp báo cáo. Chỉ mục UQ_Showtime_Seat (ShowtimeId, SeatId) bảo đảm mỗi ghế chỉ có một dòng theo suất, không trực tiếp chặn hai booking cùng giữ; cần rà lại câu chữ "kết hợp" cho khớp Chương 4–5.
- Tệp nguồn quy định trình bày được cung cấp tên quy-dinh-format-bao-cao.md (prompt ghi quy-dinh-trinh-bay.md); đã dùng Mục 0.4 trong tệp này.

(b) Chỗ outline lệch code
1. Mục 7.3.15 / 8.2 / 8.3 (outline): "cửa sổ 12 tháng cố định trong code (LoyaltyWindow.Months = 12)", đề xuất "đưa cửa sổ vào CinemaSettings / cho quản trị chỉnh cửa sổ điểm". Code: LoyaltyWindow.cs dùng DefaultMonths = CinemaSettings.DefaultLoyaltyWindowMonths (12) và LoadMonthsAsync đọc CinemaSettings.LoyaltyWindowMonths; SettingsService cập nhật được, SettingValidators giới hạn 1–120. Không có hằng Months. Theo code, Chương 8 đã bỏ ý "cố định" và bỏ hướng "cho quản trị chỉnh cửa sổ"; cần sửa Mục 7.3.15 và Bảng 7.3 dòng 15, Mục 1.x/4.x có nhắc "cố định 12 tháng". (Chưa kiểm tra giao diện quản trị có màn hình chỉnh hay không.)
2. Mục 7.3.15: script `Migrations/_manual/backfill_loyalty_entries.sql` và hạn chế "khách cũ phải backfill / đề xuất tự động hóa backfill". Code: không có thư mục Migrations/_manual trong perfo-main.zip; không có hạn chế/hướng phát triển này trong Chương 8 (đã bỏ). Cần xác minh lại và sửa Chương 7.
3. Mục 7.3.3 / 8.3: Heat chia cho 10, trọng số đánh giá thực tế tối đa 0,15. Code: ShowtimeScheduler.cs dùng `cfg.HeatRatingWeight * rating / maxRating` với maxRating mặc định 5. Hạn chế này không còn theo code; đã bỏ khỏi 8.2 và 8.3 (8.1.2 ghi điểm đánh giá "chuẩn hóa theo MaxRating"). Chưa kiểm tra đã có test cho Heat hay chưa nên không nêu thêm.
4. Mục 7.3.10 / 7.3.9: migration `20261106040650_InitialCreate.cs` thiếu Designer. Code: migration hiện là `20261106040650_InitialCreate` có file `.Designer.cs` và AppDbContextModelSnapshot.cs. Phần "bỏ qua cảnh báo thay đổi mô hình" vẫn có `ConfigureWarnings` trong Infrastructure/DependencyInjection.cs (chưa đọc nội dung). Chương 8 không nêu hạn chế về Designer; chưa nêu riêng hạn chế cảnh báo mô hình vì chưa xác minh.
5. Mục 7.3.12 / 6.6.1: "load-tests/ vắng". Code: có load-tests/ (run-load-test.ps1, scenarios/browse-movies-stress.js, peak-ticket-rush.js, lib/cinema-report.js) nhưng load-tests/results trống, nên vẫn chưa có kết quả k6. Chương 8 chỉ nói "chưa có kết quả kiểm thử tải". Cần sửa Mục 6.6.1 và 7.3.12.
6. Mục 6.4.16 / 7.3.13: helpers.ts gọi `docker exec` vào `cinema-sqlserver` bằng sqlcmd/T-SQL. Code: helpers.ts hiện dùng `psql "$DATABASE_URL"` với SQL PostgreSQL (NOW() AT TIME ZONE 'UTC', INTERVAL). Hạn chế "viết lại helper cho PostgreSQL" đã lỗi thời; Chương 8 chỉ giữ ý "cần chạy lại E2E trên môi trường hiện tại" vì kết quả 07/09/2026 có trước migration PostgreSQL. frontend/package.json không khai báo dotenv (grep không thấy) trong khi playwright.config.ts import dotenv — chưa đưa vào Chương 8.
7. Mục 6.4: outline ghi "7 spec (8 kịch bản)"; code có 7 file spec trong frontend/e2e (khớp). Thư mục test-results/ trong zip chứa error-context.md của lần chạy lỗi `admin-checkin-flow.spec.ts` ("Cannot navigate to invalid URL" tại /admin/login), trái với "8/8 đạt" nếu đó là lần chạy gần nhất; chưa rõ thời điểm. Cần rà trước khi khẳng định Bảng 8.1 ở MT5.
8. Mục 7.3.6: AiController nằm tại Cinema.Api/Controllers/Client/AiController.cs, không có thuộc tính Authorize/RateLimit (khớp outline). AiValidators.ValidRoles có "system" (khớp).
9. Mục 7.3.14: signalr.ts có `heldByUserId: number` và SignalRRealtimeNotifier không gửi trường này (khớp).
10. Mục 7.3.5: frontend/vercel.json không tồn tại; vercel.json nằm ở gốc repo và không có Content-Security-Policy (khớp ý "chưa có CSP", chỉ khác đường dẫn). auth-store.ts có dùng localStorage (chưa đọc kỹ khóa refresh token).
11. Outline 8.2 liệt kê "độ đúng đắn/dữ liệu" gồm cả hạn chế 14 (kiểu dữ liệu client); đã xếp vào nhóm này. Số hạn chế Chương 7: outline ghi "15 hạn chế" trong khi Bảng 7.3 gồm 15 dòng; Chương 8 không tăng thêm hạn chế nào.
-->

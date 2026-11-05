### 4.3.5 Module Danh mục phim

Module Danh mục phim lưu các bảng tra cứu và hồ sơ nhân sự điện ảnh dùng để mô tả một bộ phim. Module gồm năm thực thể: `Genre` (thể loại), `Language` (ngôn ngữ), `AgeRating` (nhãn phân loại độ tuổi), `Actor` (diễn viên) và `Director` (đạo diễn). Cả năm thực thể đều không chứa khóa ngoại; chúng là phía "một" trong các quan hệ với module Phim (Mục 4.3.6): `Director`, `Language` và `AgeRating` được `Movie` tham chiếu trực tiếp bằng các cột `DirectorId`, `LanguageId`, `AgeRatingId`, còn `Genre` và `Actor` được tham chiếu gián tiếp qua các bảng nối `MovieGenre` và `MovieCast`. Nhờ tách riêng, một giá trị danh mục (ví dụ thể loại "Hành động") được dùng chung cho nhiều phim thay vì lặp lại dưới dạng chuỗi trong từng dòng phim.

#### 4.3.5.1 Sơ đồ ERD của module

Hình 4.28 thể hiện năm thực thể sở hữu của module với đầy đủ thuộc tính. Thực thể `Movie` thuộc module Phim chỉ xuất hiện với khóa chính `MovieId` để biểu diễn quan hệ; hai bảng nối `MovieGenre` và `MovieCast` cũng được hiển thị ở dạng tham chiếu (chỉ khóa chính hợp thành) vì thuộc module Phim. Các quan hệ gồm: `Director` – `Movie`, `Language` – `Movie`, `AgeRating` – `Movie` (mỗi quan hệ một–nhiều, phía `Movie` không bắt buộc vì các cột khóa ngoại cho phép NULL), `Genre` – `MovieGenre` và `Actor` – `MovieCast` (mỗi quan hệ một–nhiều, phía bảng nối bắt buộc).

![Hình 4.28. ERD module Danh mục phim](hinh/4.28.png)

*Hình 4.28. ERD module Danh mục phim*

#### 4.3.5.2 Mô tả thực thể

**Thực thể Genre.** `Genre` là danh mục thể loại phim. Một thể loại có thể gắn với nhiều phim và một phim có thể thuộc nhiều thể loại; quan hệ nhiều–nhiều này được hiện thực qua bảng nối `MovieGenres` (Mục 4.3.6). Tên thể loại là duy nhất để tránh trùng lặp danh mục. Dữ liệu khởi tạo (`SeedData`) gồm 10 thể loại, từ "Hành động" đến "Bí ẩn".

Bảng 4.39. Thực thể Genre (bảng `Genres`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `GenreId` | int | Không | PK | Identity (tự tăng) | Mã định danh thể loại. |
| `Name` | varchar(50) | Không | UK | — | Tên thể loại hiển thị cho người dùng và dùng làm tiêu chí lọc phim; không trùng nhau. |

**Thực thể Language.** `Language` là danh mục ngôn ngữ của phim (ngôn ngữ âm thanh gốc hoặc lồng tiếng). Mỗi phim có tối đa một ngôn ngữ thông qua `Movie.LanguageId`; một ngôn ngữ được nhiều phim dùng chung. Thông tin phụ đề không nằm ở bảng này mà là cột `Subtitle` của `Movie`.

Bảng 4.40. Thực thể Language (bảng `Languages`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `LanguageId` | int | Không | PK | Identity (tự tăng) | Mã định danh ngôn ngữ. |
| `Name` | varchar(50) | Không | UK | — | Tên ngôn ngữ; không trùng nhau. |

**Thực thể AgeRating.** `AgeRating` là nhãn phân loại độ tuổi khán giả. Mỗi phim có tối đa một nhãn qua `Movie.AgeRatingId`. Dữ liệu khởi tạo gồm sáu nhãn: `P`, `K`, `T13`, `T16`, `T18` và `C`, mỗi nhãn kèm mô tả quy định đối tượng được xem (ví dụ `T18`: cấm phổ biến với khán giả dưới 18 tuổi; `C`: không được phép phổ biến).

Bảng 4.41. Thực thể AgeRating (bảng `AgeRatings`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `AgeRatingId` | int | Không | PK | Identity (tự tăng) | Mã định danh nhãn độ tuổi. |
| `Code` | varchar(10) | Không | UK | — | Mã nhãn ngắn gọn (ví dụ `T13`) hiển thị trên thẻ phim; không trùng nhau. |
| `Description` | varchar(255) | Có | — | — | Diễn giải đối tượng khán giả được phép xem. |

**Thực thể Actor.** `Actor` lưu hồ sơ diễn viên (họ tên, ảnh đại diện). Một diễn viên tham gia nhiều phim và một phim có nhiều diễn viên; quan hệ được hiện thực qua bảng nối `MovieCast`, nơi lưu thêm tên nhân vật (Mục 4.3.6). `FullName` không có ràng buộc duy nhất, do đó hai diễn viên trùng tên vẫn được lưu thành hai dòng riêng.

Bảng 4.42. Thực thể Actor (bảng `Actors`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `ActorId` | int | Không | PK | Identity (tự tăng) | Mã định danh diễn viên. |
| `FullName` | varchar(150) | Không | — | — | Họ và tên diễn viên. |
| `AvatarUrl` | varchar(255) | Có | — | — | Đường dẫn ảnh đại diện; để trống nếu chưa có ảnh. |

**Thực thể Director.** `Director` lưu hồ sơ đạo diễn với cấu trúc tương tự `Actor`. Mỗi phim có tối đa một đạo diễn qua `Movie.DirectorId`; một đạo diễn có thể thực hiện nhiều phim.

Bảng 4.43. Thực thể Director (bảng `Directors`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `DirectorId` | int | Không | PK | Identity (tự tăng) | Mã định danh đạo diễn. |
| `FullName` | varchar(150) | Không | — | — | Họ và tên đạo diễn. |
| `AvatarUrl` | varchar(255) | Có | — | — | Đường dẫn ảnh đại diện; để trống nếu chưa có ảnh. |

#### 4.3.5.3 Ràng buộc toàn vẹn

Module không khai báo ràng buộc CHECK hay khóa ngoại riêng. Tính toàn vẹn của dữ liệu được bảo vệ bằng ràng buộc khóa chính, ràng buộc NOT NULL và ràng buộc duy nhất như trong Bảng 4.44. Các ràng buộc khóa ngoại trỏ tới năm bảng này được khai báo ở phía `Movie`, `MovieGenre` và `MovieCast` (Bảng 4.50); tất cả đều dùng hành vi xóa `Restrict`, nghĩa là cơ sở dữ liệu từ chối xóa một thể loại, ngôn ngữ, nhãn độ tuổi, diễn viên hay đạo diễn khi còn phim tham chiếu tới nó.

Bảng 4.44. Ràng buộc toàn vẹn module Danh mục phim

| Thực thể | Ràng buộc | Loại | Mục đích bảo vệ |
|---|---|---|---|
| Genre, Language, AgeRating, Actor, Director | Khóa chính `GenreId`, `LanguageId`, `AgeRatingId`, `ActorId`, `DirectorId` | Khóa chính | Định danh duy nhất từng dòng danh mục; giá trị sinh tự động (identity). |
| Genre, Language | `Name` NOT NULL, tối đa 50 ký tự | NOT NULL | Mỗi thể loại, ngôn ngữ phải có tên. |
| AgeRating | `Code` NOT NULL, tối đa 10 ký tự | NOT NULL | Mỗi nhãn độ tuổi phải có mã. |
| Actor, Director | `FullName` NOT NULL, tối đa 150 ký tự | NOT NULL | Mỗi diễn viên, đạo diễn phải có họ tên. |
| Genre, Language, AgeRating | Chỉ mục duy nhất trên `Name` (Genre, Language) và `Code` (AgeRating) | Duy nhất | Không cho phép trùng giá trị danh mục (chi tiết ở Bảng 4.45). |

#### 4.3.5.4 Chỉ mục

Ba chỉ mục của module đều là chỉ mục duy nhất, đồng thời hiện thực ràng buộc duy nhất ở Bảng 4.44 và hỗ trợ tra cứu danh mục theo tên hoặc mã. Cấu hình EF không đặt tên riêng cho các chỉ mục này nên EF dùng quy ước `IX_<Bảng>_<Cột>`; tên thực tế trong migration `InitialCreate` được ghi ở cột "Tên chỉ mục".

Bảng 4.45. Chỉ mục module Danh mục phim

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| Genre | `IX_Genres_Name` (tên tự sinh) | Duy nhất | `Name` | — |
| Language | `IX_Languages_Name` (tên tự sinh) | Duy nhất | `Name` | — |
| AgeRating | `IX_AgeRatings_Code` (tên tự sinh) | Duy nhất | `Code` | — |

### 4.3.6 Module Phim

Module Phim lưu thông tin trung tâm của hệ thống: hồ sơ phim và đánh giá của khán giả. Module gồm bốn thực thể: `Movie` (phim), `MovieGenre` (bảng nối phim – thể loại), `MovieCast` (bảng nối phim – diễn viên kèm tên nhân vật) và `Review` (đánh giá). `Movie` tham chiếu ba thực thể danh mục `Director`, `Language`, `AgeRating` của module Danh mục phim (Mục 4.3.5); hai bảng nối kết nối `Movie` với `Genre` và `Actor`. `Review` tham chiếu `User` của module Người dùng. Ở chiều ngược lại, `Showtime` của module Suất chiếu (Mục 4.3.8) tham chiếu `Movie` qua `MovieId`.

#### 4.3.6.1 Sơ đồ ERD của module

Hình 4.29 thể hiện bốn thực thể sở hữu của module với đầy đủ thuộc tính. Các thực thể `Director`, `Language`, `AgeRating`, `Genre`, `Actor` (module Danh mục phim), `User` (module Người dùng) và `Showtime` (module Suất chiếu) chỉ xuất hiện với khóa chính. Quan hệ của `Movie` với `Director`, `Language`, `AgeRating` là một–nhiều không bắt buộc ở phía `Movie`; `Movie` – `MovieGenre`, `Movie` – `MovieCast` và `Movie` – `Review` là một–nhiều bắt buộc ở phía bảng con; `User` – `Review` là một–nhiều. Khóa chính của `MovieGenre` và `MovieCast` là khóa hợp thành từ hai khóa ngoại.

![Hình 4.29. ERD module Phim](hinh/4.29.png)

*Hình 4.29. ERD module Phim*

#### 4.3.6.2 Mô tả thực thể

**Thực thể Movie.** `Movie` lưu hồ sơ một bộ phim: tên, mô tả, thời lượng, ngày khởi chiếu, ngôn ngữ, phụ đề, nhãn độ tuổi, poster, trailer, trạng thái chiếu và điểm đánh giá trung bình. Trạng thái `Status` được lưu dưới dạng chuỗi tương ứng với enum `MovieStatus` gồm ba giá trị `ComingSoon`, `NowShowing`, `Ended`; phim mới tạo mặc định ở `ComingSoon`. Cột `AverageRating` là cột dẫn xuất: sau mỗi lần tạo, sửa hoặc xóa đánh giá, `ReviewService` tính lại trung bình cộng `Rating` của các đánh giá thuộc phim, làm tròn hai chữ số thập phân và ghi vào cột này (phim chưa có đánh giá có giá trị 0). Khi quản trị viên xóa một phim đã có suất chiếu, `MovieService.DeleteMovieAsync` không xóa dòng dữ liệu mà chuyển `Status` sang `Ended`; chỉ phim chưa có suất chiếu mới bị xóa vật lý.

Bảng 4.46. Thực thể Movie (bảng `Movies`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `MovieId` | int | Không | PK | Identity (tự tăng) | Mã định danh phim. |
| `Title` | varchar(200) | Không | — | — | Tên phim hiển thị cho khán giả. |
| `OriginalTitle` | varchar(200) | Có | — | — | Tên gốc (tên quốc tế) của phim. |
| `Description` | text | Có | — | — | Nội dung tóm tắt phim. |
| `DirectorId` | int | Có | FK | Tham chiếu `Directors`; `Restrict` | Đạo diễn của phim. |
| `DurationMinutes` | int | Không | — | CHECK `> 0` | Thời lượng phim (phút), dùng để tính giờ kết thúc suất chiếu. |
| `ReleaseDate` | date | Có | — | — | Ngày khởi chiếu dự kiến hoặc thực tế. |
| `LanguageId` | int | Có | FK | Tham chiếu `Languages`; `Restrict` | Ngôn ngữ của phim. |
| `Subtitle` | varchar(50) | Có | — | — | Thông tin phụ đề (dạng văn bản tự do). |
| `AgeRatingId` | int | Có | FK | Tham chiếu `AgeRatings`; `Restrict` | Nhãn phân loại độ tuổi của phim. |
| `PosterUrl` | varchar(255) | Có | — | — | Đường dẫn ảnh poster. |
| `TrailerUrl` | varchar(255) | Có | — | — | Đường dẫn video trailer. |
| `Status` | varchar(20) | Không | — | `MovieStatus.ComingSoon` (lưu chuỗi `ComingSoon`); CHECK thuộc 3 giá trị | Trạng thái chiếu: sắp chiếu, đang chiếu, đã kết thúc. |
| `AverageRating` | decimal(3,2) | Không | — | `0m`; CHECK từ 0 đến 5 | Điểm đánh giá trung bình của khán giả, tính lại từ bảng `Reviews`. |
| `CreatedAt` | timestamp without time zone | Không | — | SQL: `NOW()` | Thời điểm tạo bản ghi phim. |

**Thực thể MovieGenre.** `MovieGenre` là bảng nối hiện thực quan hệ nhiều–nhiều giữa `Movie` và `Genre`. Khóa chính hợp thành `(MovieId, GenreId)` bảo đảm một thể loại chỉ được gán cho một phim một lần. Khi xóa phim, các dòng nối của phim bị xóa theo (`Cascade`); ngược lại không thể xóa thể loại còn được phim sử dụng (`Restrict`).

Bảng 4.47. Thực thể MovieGenre (bảng `MovieGenres`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `MovieId` | int | Không | PK, FK | Tham chiếu `Movies`; `Cascade` | Phim được gán thể loại. |
| `GenreId` | int | Không | PK, FK | Tham chiếu `Genres`; `Restrict` | Thể loại được gán cho phim. |

**Thực thể MovieCast.** `MovieCast` là bảng nối giữa `Movie` và `Actor`, đồng thời mang thuộc tính riêng của quan hệ là tên nhân vật mà diễn viên thủ vai. Khóa chính hợp thành `(MovieId, ActorId)` nên một diễn viên chỉ xuất hiện một lần trong dàn diễn viên của một phim. Cách xử lý khi xóa giống `MovieGenre`: xóa phim thì xóa dàn diễn viên của phim (`Cascade`), còn diễn viên đang tham gia phim thì không thể bị xóa (`Restrict`). Tên bảng trong cơ sở dữ liệu là `MovieCast` (không có chữ "s").

Bảng 4.48. Thực thể MovieCast (bảng `MovieCast`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `MovieId` | int | Không | PK, FK | Tham chiếu `Movies`; `Cascade` | Phim có diễn viên tham gia. |
| `ActorId` | int | Không | PK, FK | Tham chiếu `Actors`; `Restrict` | Diễn viên tham gia phim. |
| `CharacterName` | varchar(150) | Có | — | — | Tên nhân vật mà diễn viên thủ vai trong phim. |

**Thực thể Review.** `Review` lưu đánh giá (điểm sao và nhận xét) của khán giả cho một phim. Mỗi người dùng chỉ được đánh giá một phim một lần (chỉ mục duy nhất `UQ_User_Movie_Review`); `ReviewService.CreateReviewAsync` kiểm tra thêm quy tắc nghiệp vụ rằng người dùng phải có ít nhất một vé đã soát (`IsCheckedIn`) thuộc suất chiếu của phim đó, nếu không sẽ trả lỗi `BusinessRuleException` (`Messages.Rule_ReviewRequiresCheckIn`). Xóa phim kéo theo xóa các đánh giá của phim (`Cascade`), còn người dùng đã có đánh giá không thể bị xóa (`Restrict`).

Bảng 4.49. Thực thể Review (bảng `Reviews`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `ReviewId` | int | Không | PK | Identity (tự tăng) | Mã định danh đánh giá. |
| `MovieId` | int | Không | FK | Tham chiếu `Movies`; `Cascade` | Phim được đánh giá. |
| `UserId` | int | Không | FK | Tham chiếu `Users`; `Restrict` | Người dùng viết đánh giá. |
| `Rating` | int | Không | — | CHECK từ 1 đến 5 | Số sao đánh giá (nguyên, 1–5). |
| `Comment` | text | Có | — | — | Nội dung nhận xét; có thể để trống. |
| `CreatedAt` | timestamp without time zone | Không | — | SQL: `NOW()` | Thời điểm gửi đánh giá (khi tạo qua `ReviewService`, giá trị được gán `DateTime.UtcNow`). |

#### 4.3.6.3 Ràng buộc toàn vẹn

Các ràng buộc CHECK bảo vệ miền giá trị hợp lệ: thời lượng phim phải dương, trạng thái chỉ nhận ba giá trị của `MovieStatus`, điểm trung bình nằm trong khoảng 0–5 và điểm đánh giá của từng lượt nằm trong khoảng 1–5. Các khóa ngoại bảo đảm mọi tham chiếu đều trỏ tới dòng tồn tại. Hành vi xóa được chọn theo nguyên tắc: dữ liệu phụ thuộc hoàn toàn vào phim (thể loại, diễn viên, đánh giá của phim) bị xóa theo phim (`Cascade`), còn dữ liệu danh mục và người dùng được bảo vệ khỏi việc xóa nhầm khi còn được tham chiếu (`Restrict`). Cột "Hành vi xóa" được bổ sung vào bảng khung theo cấu hình EF và `ModelSnapshot`.

Bảng 4.50. Ràng buộc toàn vẹn module Phim

| Thực thể | Tên | Loại | Biểu thức | Hành vi xóa | Mục đích bảo vệ |
|---|---|---|---|---|---|
| Movie | `CK_Movies_Duration` | CHECK | `"DurationMinutes" > 0` | — | Không cho phép thời lượng bằng 0 hoặc âm. |
| Movie | `CK_Movies_Status` | CHECK | `"Status" IN ('ComingSoon','NowShowing','Ended')` | — | Trạng thái phải thuộc tập giá trị của `MovieStatus`. |
| Movie | `CK_Movies_AverageRating` | CHECK | `"AverageRating" BETWEEN 0 AND 5` | — | Điểm trung bình không vượt miền sao đánh giá. |
| Review | `CK_Reviews_Rating` | CHECK | `"Rating" BETWEEN 1 AND 5` | — | Mỗi lượt đánh giá chỉ nhận từ 1 đến 5 sao. |
| Movie | FK `DirectorId` | Khóa ngoại | — | Restrict | Không xóa đạo diễn còn phim tham chiếu. |
| Movie | FK `LanguageId` | Khóa ngoại | — | Restrict | Không xóa ngôn ngữ còn phim tham chiếu. |
| Movie | FK `AgeRatingId` | Khóa ngoại | — | Restrict | Không xóa nhãn độ tuổi còn phim tham chiếu. |
| MovieGenre | FK `MovieId` | Khóa ngoại | — | Cascade | Xóa phim thì xóa các dòng gán thể loại của phim. |
| MovieGenre | FK `GenreId` | Khóa ngoại | — | Restrict | Không xóa thể loại còn được gán cho phim. |
| MovieCast | FK `MovieId` | Khóa ngoại | — | Cascade | Xóa phim thì xóa dàn diễn viên của phim. |
| MovieCast | FK `ActorId` | Khóa ngoại | — | Restrict | Không xóa diễn viên còn tham gia phim. |
| Review | FK `MovieId` | Khóa ngoại | — | Cascade | Xóa phim thì xóa các đánh giá của phim. |
| Review | FK `UserId` | Khóa ngoại | — | Restrict | Không xóa người dùng còn đánh giá. |

#### 4.3.6.4 Chỉ mục

Các chỉ mục trên cột khóa ngoại của `Movie` hỗ trợ lọc phim theo đạo diễn, ngôn ngữ, nhãn độ tuổi; chỉ mục `IX_Movies_Status` và `IX_Movies_ReleaseDate` hỗ trợ truy vấn danh sách phim đang chiếu, sắp chiếu và sắp xếp theo ngày khởi chiếu. `IX_MovieCast_Actor` và chỉ mục tự sinh trên `GenreId` phục vụ truy vấn theo chiều ngược của bảng nối (phim của một diễn viên, phim của một thể loại), vì khóa chính hợp thành chỉ hỗ trợ tra cứu bắt đầu từ `MovieId`. `IX_Reviews_Movie` phục vụ tải danh sách đánh giá và tính trung bình của một phim. `UQ_User_Movie_Review` vừa bảo đảm mỗi người dùng chỉ đánh giá một phim một lần, vừa phục vụ tra cứu theo `UserId`. Dòng `IX_MovieGenres_GenreId` được bổ sung so với bảng khung vì EF tự tạo chỉ mục cho khóa ngoại `GenreId` (xác nhận trong migration `InitialCreate`).

Bảng 4.51. Chỉ mục module Phim

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| Movie | `IX_Movies_Director` | Thường | `DirectorId` | — |
| Movie | `IX_Movies_Language` | Thường | `LanguageId` | — |
| Movie | `IX_Movies_AgeRating` | Thường | `AgeRatingId` | — |
| Movie | `IX_Movies_Status` | Thường | `Status` | — |
| Movie | `IX_Movies_ReleaseDate` | Thường | `ReleaseDate` | — |
| MovieGenre | `IX_MovieGenres_GenreId` (tên tự sinh) | Thường | `GenreId` | — |
| MovieCast | `IX_MovieCast_Actor` | Thường | `ActorId` | — |
| Review | `IX_Reviews_Movie` | Thường | `MovieId` | — |
| Review | `UQ_User_Movie_Review` | Duy nhất | `UserId`, `MovieId` | — |

### 4.3.7 Module Phòng chiếu

Module Phòng chiếu mô tả hạ tầng vật lý của rạp: phòng chiếu và sơ đồ ghế của từng phòng. Module gồm bốn thực thể: `CinemaRoom` (phòng chiếu), `ScreenType` (loại màn hình), `Seat` (ghế) và `SeatType` (loại ghế, kèm hệ số giá). `CinemaRoom` tham chiếu `ScreenType`; `Seat` tham chiếu `CinemaRoom` và `SeatType`. Với module khác, `Showtime` (Mục 4.3.8) tham chiếu `CinemaRoom` để xác định phòng chiếu của suất, còn `ShowtimeSeat` tham chiếu `Seat` để tạo trạng thái ghế theo từng suất. Hệ số `PriceMultiplier` của `SeatType` được dùng khi tính giá vé từng ghế.

#### 4.3.7.1 Sơ đồ ERD của module

Hình 4.30 thể hiện bốn thực thể sở hữu của module với đầy đủ thuộc tính; `Showtime` và `ShowtimeSeat` của module Suất chiếu chỉ xuất hiện với khóa chính. Các quan hệ gồm: `ScreenType` – `CinemaRoom` (một–nhiều, phía phòng không bắt buộc), `CinemaRoom` – `Seat` (một–nhiều bắt buộc), `SeatType` – `Seat` (một–nhiều, phía ghế không bắt buộc), `CinemaRoom` – `Showtime` và `Seat` – `ShowtimeSeat` (một–nhiều, thuộc module Suất chiếu).

![Hình 4.30. ERD module Phòng chiếu](hinh/4.30.png)

*Hình 4.30. ERD module Phòng chiếu*

#### 4.3.7.2 Mô tả thực thể

**Thực thể CinemaRoom.** `CinemaRoom` lưu thông tin một phòng chiếu và kích thước lưới ghế (`Rows` × `Cols`). Phòng mới tạo qua `RoomService` có `TotalSeats = 0` và `IsActive = true`; `TotalSeats` là cột dẫn xuất, được `RoomService` cập nhật mỗi khi lưu sơ đồ ghế bằng số ghế vừa đang hoạt động vừa không phải lối đi (`IsActive && !IsAisle`). Giá trị `10` của `Rows` và `Cols` chỉ là giá trị khởi tạo ở lớp thực thể C#; cơ sở dữ liệu không đặt giá trị mặc định cho hai cột này.

Bảng 4.52. Thực thể CinemaRoom (bảng `CinemaRooms`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `RoomId` | int | Không | PK | Identity (tự tăng) | Mã định danh phòng chiếu. |
| `RoomName` | varchar(50) | Không | UK | — | Tên phòng chiếu; không trùng nhau (`RoomService` trả lỗi xung đột khi tên đã tồn tại). |
| `ScreenTypeId` | int | Có | FK | Tham chiếu `ScreenTypes`; `Restrict` | Loại màn hình của phòng. |
| `TotalSeats` | int | Không | — | SQL default `0`; CHECK `>= 0` | Số ghế có thể bán của phòng (ghế hoạt động, không phải lối đi). |
| `IsActive` | boolean | Không | — | `true` | Phòng đang hoạt động hay đã ngừng sử dụng. |
| `Rows` | int | Không | — | Không có default SQL (C#: `10`) | Số hàng của lưới sơ đồ ghế. |
| `Cols` | int | Không | — | Không có default SQL (C#: `10`) | Số cột của lưới sơ đồ ghế. |

**Thực thể ScreenType.** `ScreenType` là danh mục loại màn hình của phòng chiếu. Dữ liệu khởi tạo gồm ba loại: `2D`, `3D` và `IMAX`. Một loại màn hình được nhiều phòng dùng chung.

Bảng 4.53. Thực thể ScreenType (bảng `ScreenTypes`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `ScreenTypeId` | int | Không | PK | Identity (tự tăng) | Mã định danh loại màn hình. |
| `Name` | varchar(50) | Không | UK | — | Tên loại màn hình (ví dụ `2D`, `3D`, `IMAX`); không trùng nhau. |

**Thực thể Seat.** `Seat` mô tả một vị trí ghế trong phòng, định vị bằng cặp (`SeatRow`, `SeatNumber`) trong phạm vi một phòng. Cột `IsAisle` đánh dấu ô là lối đi (không phải ghế bán được). Cột `CoupleId` dùng để ghép hai ghế đôi: các ghế có cùng `CoupleId` thuộc một cặp; `RoomService` yêu cầu mỗi `CoupleId` có đúng hai ghế và cùng một hàng, nếu không trả lỗi dữ liệu ghế đôi không hợp lệ. Khi sơ đồ ghế được cập nhật, ghế đã từng có trong `ShowtimeSeat` không bị xóa vật lý mà chuyển `IsActive = false` để giữ lịch sử đặt vé; việc thay đổi ghế đang được giữ hoặc đã đặt cho suất chiếu tương lai bị từ chối.

Bảng 4.54. Thực thể Seat (bảng `Seats`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `SeatId` | int | Không | PK | Identity (tự tăng) | Mã định danh ghế. |
| `RoomId` | int | Không | FK | Tham chiếu `CinemaRooms`; `Cascade` | Phòng chiếu chứa ghế. |
| `SeatRow` | varchar(5) | Không | — | Thuộc `UQ_Room_Seat` | Ký hiệu hàng ghế (ví dụ `A`). |
| `SeatNumber` | int | Không | — | CHECK `> 0`; thuộc `UQ_Room_Seat` | Số thứ tự ghế trong hàng. |
| `SeatTypeId` | int | Có | FK | Tham chiếu `SeatTypes`; `Restrict` | Loại ghế, quyết định hệ số giá. |
| `IsActive` | boolean | Không | — | `true` | Ghế còn được sử dụng hay đã ngừng (vô hiệu hóa). |
| `IsAisle` | boolean | Không | — | C#: `false`; không có default SQL | Ô là lối đi, không bán vé. |
| `CoupleId` | uuid | Có | — | — | Mã nhóm ghép hai ghế đôi; NULL với ghế thường. |

**Thực thể SeatType.** `SeatType` là danh mục loại ghế kèm hệ số nhân giá `PriceMultiplier`. Dữ liệu khởi tạo gồm `Standard` (hệ số 1,0), `VIP` (1,5) và `Couple` (2,0). Khi tạo ghế cho suất chiếu, `ShowtimeService` tính giá vé của ghế bằng giá cơ sở (`BasePrice`) nhân với hệ số này; ghế chưa có loại được tính hệ số `1.0` (xem Mục 4.3.8).

Bảng 4.55. Thực thể SeatType (bảng `SeatTypes`)

| Cột | Kiểu dữ liệu | Cho phép NULL | Khóa | Giá trị mặc định / ghi chú | Ý nghĩa |
|---|---|---|---|---|---|
| `SeatTypeId` | int | Không | PK | Identity (tự tăng) | Mã định danh loại ghế. |
| `Name` | varchar(50) | Không | UK | — | Tên loại ghế (`Standard`, `VIP`, `Couple`); không trùng nhau. |
| `PriceMultiplier` | decimal(5,2) | Không | — | `1.0m`; CHECK `> 0` | Hệ số nhân giá vé so với giá cơ sở của suất chiếu. |

#### 4.3.7.3 Ràng buộc toàn vẹn

Ba ràng buộc CHECK bảo vệ miền giá trị: số ghế của phòng không âm, số thứ tự ghế dương và hệ số giá dương (tránh giá vé bằng 0 hoặc âm). Về khóa ngoại, ghế thuộc hẳn về phòng nên xóa phòng sẽ xóa toàn bộ ghế của phòng (`Cascade`); phòng không thể xóa khi còn suất chiếu tham chiếu vì khóa ngoại `Showtime.RoomId` dùng `Restrict`. Loại màn hình và loại ghế được bảo vệ khỏi việc xóa khi còn được tham chiếu (`Restrict`).

Bảng 4.56. Ràng buộc toàn vẹn module Phòng chiếu

| Thực thể | Tên | Loại | Biểu thức | Hành vi xóa | Mục đích bảo vệ |
|---|---|---|---|---|---|
| CinemaRoom | `CK_CinemaRooms_TotalSeats` | CHECK | `"TotalSeats" >= 0` | — | Số ghế của phòng không âm. |
| Seat | `CK_Seats_SeatNumber` | CHECK | `"SeatNumber" > 0` | — | Số thứ tự ghế phải dương. |
| SeatType | `CK_SeatTypes_PriceMultiplier` | CHECK | `"PriceMultiplier" > 0` | — | Hệ số giá phải dương. |
| CinemaRoom | FK `ScreenTypeId` | Khóa ngoại | — | Restrict | Không xóa loại màn hình còn phòng sử dụng. |
| Seat | FK `RoomId` | Khóa ngoại | — | Cascade | Xóa phòng thì xóa các ghế của phòng. |
| Seat | FK `SeatTypeId` | Khóa ngoại | — | Restrict | Không xóa loại ghế còn ghế sử dụng. |

#### 4.3.7.4 Chỉ mục

`UQ_Room_Seat` bảo đảm không có hai ghế trùng vị trí (hàng, số) trong cùng một phòng và đồng thời phục vụ tải toàn bộ ghế của một phòng theo `RoomId` nhờ cột đầu của chỉ mục. Các chỉ mục duy nhất trên tên bảo vệ tính duy nhất của tên phòng, loại màn hình và loại ghế. `IX_Seats_SeatType` hỗ trợ truy vấn ghế theo loại ghế và kiểm tra tham chiếu khi xóa loại ghế. Dòng `IX_CinemaRooms_ScreenTypeId` được bổ sung so với bảng khung vì EF tự tạo chỉ mục cho khóa ngoại `ScreenTypeId` (xác nhận trong migration `InitialCreate`).

Bảng 4.57. Chỉ mục module Phòng chiếu

| Thực thể | Tên chỉ mục | Loại | Cột | Điều kiện lọc |
|---|---|---|---|---|
| CinemaRoom | `IX_CinemaRooms_RoomName` (tên tự sinh) | Duy nhất | `RoomName` | — |
| CinemaRoom | `IX_CinemaRooms_ScreenTypeId` (tên tự sinh) | Thường | `ScreenTypeId` | — |
| ScreenType | `IX_ScreenTypes_Name` (tên tự sinh) | Duy nhất | `Name` | — |
| Seat | `IX_Seats_SeatType` | Thường | `SeatTypeId` | — |
| Seat | `UQ_Room_Seat` | Duy nhất | `RoomId`, `SeatRow`, `SeatNumber` | — |
| SeatType | `IX_SeatTypes_Name` (tên tự sinh) | Duy nhất | `Name` | — |

<!-- GHI CHÚ ĐỐI CHIẾU
(a) [CẦN BỔ SUNG] còn lại:
- Mã Mermaid ERD Hình 4.28, 4.29, 4.30 (`ma-nguon-so-do.md`) KHÔNG có trong tài liệu đính kèm (chỉ có `ma-nguon-so-do-bo-sung.md` và `ma-nguon-so-do-mermaid-loyalty.md`, không chứa các hình này). Phần mô tả Hình 4.28–4.30 được viết theo cấu hình EF và `AppDbContextModelSnapshot`, chưa đối chiếu được với mã Mermaid. Cần gửi file `ma-nguon-so-do.md` để kiểm tra node/quan hệ; ảnh `hinh/4.28.png`, `hinh/4.29.png`, `hinh/4.30.png` chưa có.
- Bảng 4.44 (outline chỉ ghi "không có CHECK hoặc khóa ngoại riêng"): tôi dựng bảng từ PK, NOT NULL và chỉ mục duy nhất; cần xác nhận cách trình bày này.
- Không có nguồn trích dẫn tài liệu tham khảo nào được dùng trong phạm vi này.
(b) Chỗ outline lệch code:
- Outline 4.3.5.2, 4.3.6.2, 4.3.7.2: các bảng khung chưa có cột "Ý nghĩa" (đã bổ sung) và ghi kiểu `decimal` chung chung; code: `Movies.AverageRating` = decimal(3,2), `SeatTypes.PriceMultiplier` = decimal(5,2) (MovieConfiguration, SeatTypeConfiguration, snapshot).
- Outline Movie.Status ghi kiểu varchar(20), mặc định `MovieStatus.ComingSoon`: đúng; trong snapshot/DB giá trị mặc định là chuỗi 'ComingSoon'.
- Outline CinemaRoom `Rows`, `Cols` "Mặc định: —": đúng ở mức SQL (snapshot không có HasDefaultValue); entity C# khởi tạo `= 10` (CinemaRoom.cs). Tương tự `Seat.IsAisle`: C# `= false`, SQL không có default.
- Outline Bảng 4.51: thiếu chỉ mục `IX_MovieGenres_GenreId` (EF tự sinh cho FK GenreId; có trong snapshot `HasIndex("GenreId")` và migration). Đã thêm dòng.
- Outline Bảng 4.57: thiếu chỉ mục `IX_CinemaRooms_ScreenTypeId` (EF tự sinh; có trong snapshot và migration). Đã thêm dòng.
- Outline Bảng 4.50 và 4.56 chỉ có 4 cột (Thực thể, Tên, Loại, Biểu thức) và để "—" ở FK; đã bổ sung cột "Hành vi xóa" và "Mục đích bảo vệ" theo yêu cầu prompt nền (đọc từ Configurations, khớp snapshot).
- Outline Bảng 4.45/4.57 ghi "(tên tự sinh)": đã ghi kèm tên thực tế trong migration InitialCreate (IX_Genres_Name, IX_Languages_Name, IX_AgeRatings_Code, IX_CinemaRooms_RoomName, IX_ScreenTypes_Name, IX_SeatTypes_Name).
- Kiểu `timestamp` trong outline: code là `timestamp without time zone`; `Review.CreatedAt` có default `NOW()` nhưng `ReviewService` gán `DateTime.UtcNow` khi tạo.
- Công thức giá vé `Price = BasePrice × PriceMultiplier` (hệ số 1.0 khi `SeatType` null) đã xác nhận tại ShowtimeService.cs (dòng 287, 1371).
-->

namespace Cinema.Shared.Resources {
    using System;

    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Messages {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Cinema.Shared.Resources.Messages", typeof(Messages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Bạn không có quyền thực hiện thao tác này..
        /// </summary>
        public static string Forbidden_AccessDenied {
            get {
                return ResourceManager.GetString("Forbidden_AccessDenied", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy tài nguyên yêu cầu..
        /// </summary>
        public static string NotFound_Resource {
            get {
                return ResourceManager.GetString("NotFound_Resource", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Yêu cầu không hợp lệ..
        /// </summary>
        public static string System_BadRequest {
            get {
                return ResourceManager.GetString("System_BadRequest", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Phương thức không được hỗ trợ..
        /// </summary>
        public static string System_MethodNotAllowed {
            get {
                return ResourceManager.GetString("System_MethodNotAllowed", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Bạn thao tác quá nhanh. Vui lòng thử lại sau..
        /// </summary>
        public static string System_TooManyRequests {
            get {
                return ResourceManager.GetString("System_TooManyRequests", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Bạn cần đăng nhập để thực hiện thao tác này..
        /// </summary>
        public static string Unauthorized_Required {
            get {
                return ResourceManager.GetString("Unauthorized_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Dữ liệu không hợp lệ..
        /// </summary>
        public static string Common_InvalidData {
            get {
                return ResourceManager.GetString("Common_InvalidData", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Diễn viên còn trong danh sách diễn viên của phim, không thể xóa..
        /// </summary>
        public static string Conflict_ActorInUse {
            get {
                return ResourceManager.GetString("Conflict_ActorInUse", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Mã phân loại độ tuổi đã tồn tại..
        /// </summary>
        public static string Conflict_AgeRatingCodeExists {
            get {
                return ResourceManager.GetString("Conflict_AgeRatingCodeExists", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Phân loại độ tuổi đang được phim tham chiếu, không thể xóa..
        /// </summary>
        public static string Conflict_AgeRatingInUse {
            get {
                return ResourceManager.GetString("Conflict_AgeRatingInUse", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Booking đang được xử lý hoặc đã được hủy..
        /// </summary>
        public static string Conflict_BookingBeingProcessedOrCancelled {
            get {
                return ResourceManager.GetString("Conflict_BookingBeingProcessedOrCancelled", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không thể hạ role xuống Customer khi còn hồ sơ Employee đang hoạt động — vô hiệu hóa Employee trước..
        /// </summary>
        public static string Conflict_CannotDowngradeRoleWithActiveEmployee {
            get {
                return ResourceManager.GetString("Conflict_CannotDowngradeRoleWithActiveEmployee", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đạo diễn đang được phim tham chiếu, không thể xóa..
        /// </summary>
        public static string Conflict_DirectorInUse {
            get {
                return ResourceManager.GetString("Conflict_DirectorInUse", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to userId đã có hồ sơ Employee..
        /// </summary>
        public static string Conflict_EmployeeProfileExists {
            get {
                return ResourceManager.GetString("Conflict_EmployeeProfileExists", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Thể loại đã tồn tại..
        /// </summary>
        public static string Conflict_GenreExists {
            get {
                return ResourceManager.GetString("Conflict_GenreExists", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Thể loại đang được phim tham chiếu, không thể xóa..
        /// </summary>
        public static string Conflict_GenreInUse {
            get {
                return ResourceManager.GetString("Conflict_GenreInUse", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Ngôn ngữ đã tồn tại..
        /// </summary>
        public static string Conflict_LanguageExists {
            get {
                return ResourceManager.GetString("Conflict_LanguageExists", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Ngôn ngữ đang được phim tham chiếu, không thể xóa..
        /// </summary>
        public static string Conflict_LanguageInUse {
            get {
                return ResourceManager.GetString("Conflict_LanguageInUse", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Hạng thành viên đã tồn tại..
        /// </summary>
        public static string Conflict_MembershipTierExists {
            get {
                return ResourceManager.GetString("Conflict_MembershipTierExists", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Hạng thành viên đang được khách hàng tham chiếu, không thể xóa..
        /// </summary>
        public static string Conflict_MembershipTierInUse {
            get {
                return ResourceManager.GetString("Conflict_MembershipTierInUse", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đang có một booking Pending khác chưa hoàn tất cho suất chiếu này..
        /// </summary>
        public static string Conflict_PendingBookingExistsForShowtime {
            get {
                return ResourceManager.GetString("Conflict_PendingBookingExistsForShowtime", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Vị trí đã tồn tại..
        /// </summary>
        public static string Conflict_PositionExists {
            get {
                return ResourceManager.GetString("Conflict_PositionExists", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Vị trí đang được nhân viên tham chiếu, không thể xóa..
        /// </summary>
        public static string Conflict_PositionInUse {
            get {
                return ResourceManager.GetString("Conflict_PositionInUse", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Bạn đã đánh giá phim này rồi..
        /// </summary>
        public static string Conflict_ReviewAlreadyExists {
            get {
                return ResourceManager.GetString("Conflict_ReviewAlreadyExists", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Phòng đang có ghế đã đặt hoặc đang được giữ ở suất chiếu sắp tới, không thể ngừng hoạt động..
        /// </summary>
        public static string Conflict_RoomHasHeldSeatsCannotDeactivate {
            get {
                return ResourceManager.GetString("Conflict_RoomHasHeldSeatsCannotDeactivate", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to roomName đã tồn tại..
        /// </summary>
        public static string Conflict_RoomNameExists {
            get {
                return ResourceManager.GetString("Conflict_RoomNameExists", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Loại màn hình đã tồn tại..
        /// </summary>
        public static string Conflict_ScreenTypeExists {
            get {
                return ResourceManager.GetString("Conflict_ScreenTypeExists", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Loại màn hình đang được phòng chiếu tham chiếu, không thể xóa..
        /// </summary>
        public static string Conflict_ScreenTypeInUse {
            get {
                return ResourceManager.GetString("Conflict_ScreenTypeInUse", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Loại ghế đã tồn tại..
        /// </summary>
        public static string Conflict_SeatTypeExists {
            get {
                return ResourceManager.GetString("Conflict_SeatTypeExists", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Loại ghế đang được ghế tham chiếu, không thể xóa..
        /// </summary>
        public static string Conflict_SeatTypeInUse {
            get {
                return ResourceManager.GetString("Conflict_SeatTypeInUse", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Một hoặc nhiều ghế đã bị giữ/đặt bởi người khác..
        /// </summary>
        public static string Conflict_SeatsAlreadyTaken {
            get {
                return ResourceManager.GetString("Conflict_SeatsAlreadyTaken", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không thể thay đổi ghế đang được đặt hoặc giữ ở suất chiếu sắp tới..
        /// </summary>
        public static string Conflict_SeatsHeldOrBookedCannotChange {
            get {
                return ResourceManager.GetString("Conflict_SeatsHeldOrBookedCannotChange", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Suất chiếu đã có booking đã xác nhận (Confirmed), không thể sửa..
        /// </summary>
        public static string Conflict_ShowtimeHasConfirmedBookings {
            get {
                return ResourceManager.GetString("Conflict_ShowtimeHasConfirmedBookings", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Suất chiếu đang có khách giữ ghế hoặc đang thanh toán (Pending), không thể sửa lúc này. Vui lòng thử lại sau vài phút..
        /// </summary>
        public static string Conflict_ShowtimeHasPendingBookings {
            get {
                return ResourceManager.GetString("Conflict_ShowtimeHasPendingBookings", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Trùng khung giờ chiếu trong phòng này..
        /// </summary>
        public static string Conflict_ShowtimeOverlapInRoom {
            get {
                return ResourceManager.GetString("Conflict_ShowtimeOverlapInRoom", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Vé đã check-in trước đó..
        /// </summary>
        public static string Conflict_TicketAlreadyCheckedIn {
            get {
                return ResourceManager.GetString("Conflict_TicketAlreadyCheckedIn", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Dữ liệu không thỏa mãn ràng buộc kiểm tra..
        /// </summary>
        public static string Db_CheckViolation {
            get {
                return ResourceManager.GetString("Db_CheckViolation", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng tải lại và thử lại..
        /// </summary>
        public static string Db_ConcurrencyConflict {
            get {
                return ResourceManager.GetString("Db_ConcurrencyConflict", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Lỗi ràng buộc dữ liệu..
        /// </summary>
        public static string Db_ConstraintError {
            get {
                return ResourceManager.GetString("Db_ConstraintError", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Dữ liệu xung đột với bản ghi đã có (vi phạm ràng buộc loại trừ)..
        /// </summary>
        public static string Db_ExclusionViolation {
            get {
                return ResourceManager.GetString("Db_ExclusionViolation", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Dữ liệu tham chiếu (khóa ngoại) không hợp lệ..
        /// </summary>
        public static string Db_ForeignKeyViolation {
            get {
                return ResourceManager.GetString("Db_ForeignKeyViolation", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Dữ liệu đã tồn tại (vi phạm ràng buộc duy nhất)..
        /// </summary>
        public static string Db_UniqueViolation {
            get {
                return ResourceManager.GetString("Db_UniqueViolation", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tài khoản đã bị khóa..
        /// </summary>
        public static string Forbidden_AccountLocked {
            get {
                return ResourceManager.GetString("Forbidden_AccountLocked", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tài khoản quản trị không thể đăng nhập bằng Google..
        /// </summary>
        public static string Forbidden_AdminCannotUseGoogleLogin {
            get {
                return ResourceManager.GetString("Forbidden_AdminCannotUseGoogleLogin", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tài khoản Customer không thể đăng nhập qua cổng quản trị..
        /// </summary>
        public static string Forbidden_CustomerCannotUseAdminPortal {
            get {
                return ResourceManager.GetString("Forbidden_CustomerCannotUseAdminPortal", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tài khoản chưa được xác thực email..
        /// </summary>
        public static string Forbidden_EmailNotVerified {
            get {
                return ResourceManager.GetString("Forbidden_EmailNotVerified", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tài khoản chưa được xác thực email. Vui lòng kiểm tra hộp thư..
        /// </summary>
        public static string Forbidden_EmailNotVerifiedCheckInbox {
            get {
                return ResourceManager.GetString("Forbidden_EmailNotVerifiedCheckInbox", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Bạn không phải chủ booking này..
        /// </summary>
        public static string Forbidden_NotBookingOwner {
            get {
                return ResourceManager.GetString("Forbidden_NotBookingOwner", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Bạn không phải chủ đánh giá này..
        /// </summary>
        public static string Forbidden_NotReviewOwner {
            get {
                return ResourceManager.GetString("Forbidden_NotReviewOwner", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Vui lòng đăng nhập qua cổng quản trị (/auth/admin/login)..
        /// </summary>
        public static string Forbidden_UseAdminPortal {
            get {
                return ResourceManager.GetString("Forbidden_UseAdminPortal", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Một hoặc nhiều actorId không tồn tại..
        /// </summary>
        public static string Invalid_ActorIdsNotFound {
            get {
                return ResourceManager.GetString("Invalid_ActorIdsNotFound", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ageRatingId không tồn tại..
        /// </summary>
        public static string Invalid_AgeRatingIdNotFound {
            get {
                return ResourceManager.GetString("Invalid_AgeRatingIdNotFound", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Khoảng ngày xếp lịch không được vượt quá {0} ngày..
        /// </summary>
        public static string Invalid_AutoScheduleRangeTooLong {
            get {
                return ResourceManager.GetString("Invalid_AutoScheduleRangeTooLong", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to comboId không hợp lệ hoặc đã ngừng bán..
        /// </summary>
        public static string Invalid_ComboIdUnavailable {
            get {
                return ResourceManager.GetString("Invalid_ComboIdUnavailable", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Một hoặc nhiều comboId không tồn tại hoặc đã ngừng bán..
        /// </summary>
        public static string Invalid_ComboIdsUnavailable {
            get {
                return ResourceManager.GetString("Invalid_ComboIdsUnavailable", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to count phải từ 1 đến 10..
        /// </summary>
        public static string Invalid_CountOutOfRange {
            get {
                return ResourceManager.GetString("Invalid_CountOutOfRange", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Dữ liệu ghế đôi không hợp lệ..
        /// </summary>
        public static string Invalid_CoupleSeatData {
            get {
                return ResourceManager.GetString("Invalid_CoupleSeatData", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Có ghế đôi bị lẻ cặp hoặc không cùng hàng..
        /// </summary>
        public static string Invalid_CoupleSeatsUnpaired {
            get {
                return ResourceManager.GetString("Invalid_CoupleSeatsUnpaired", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Mật khẩu hiện tại không đúng..
        /// </summary>
        public static string Invalid_CurrentPassword {
            get {
                return ResourceManager.GetString("Invalid_CurrentPassword", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Thiếu tên khách hàng..
        /// </summary>
        public static string Invalid_CustomerNameMissing {
            get {
                return ResourceManager.GetString("Invalid_CustomerNameMissing", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to customerName là bắt buộc.
        /// </summary>
        public static string Invalid_CustomerNameRequired {
            get {
                return ResourceManager.GetString("Invalid_CustomerNameRequired", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to date là bắt buộc (dd/MM/yyyy)..
        /// </summary>
        public static string Invalid_DateRequired {
            get {
                return ResourceManager.GetString("Invalid_DateRequired", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to directorId không tồn tại..
        /// </summary>
        public static string Invalid_DirectorIdNotFound {
            get {
                return ResourceManager.GetString("Invalid_DirectorIdNotFound", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Email đã tồn tại..
        /// </summary>
        public static string Invalid_EmailExists {
            get {
                return ResourceManager.GetString("Invalid_EmailExists", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Email đã được sử dụng..
        /// </summary>
        public static string Invalid_EmailInUse {
            get {
                return ResourceManager.GetString("Invalid_EmailInUse", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to File không được rỗng..
        /// </summary>
        public static string Invalid_FileEmpty {
            get {
                return ResourceManager.GetString("Invalid_FileEmpty", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Sai định dạng file. Chỉ chấp nhận: {0}.
        /// </summary>
        public static string Invalid_FileExtensionNotAllowed {
            get {
                return ResourceManager.GetString("Invalid_FileExtensionNotAllowed", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Sai định dạng video. Chỉ chấp nhận: {0}.
        /// </summary>
        public static string Invalid_VideoExtensionNotAllowed {
            get {
                return ResourceManager.GetString("Invalid_VideoExtensionNotAllowed", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to File vượt quá giới hạn {0}MB..
        /// </summary>
        public static string Invalid_FileTooLarge {
            get {
                return ResourceManager.GetString("Invalid_FileTooLarge", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Một hoặc nhiều genreId không tồn tại..
        /// </summary>
        public static string Invalid_GenreIdsNotFound {
            get {
                return ResourceManager.GetString("Invalid_GenreIdsNotFound", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to groupBy phải là day, week hoặc month..
        /// </summary>
        public static string Invalid_GroupBy {
            get {
                return ResourceManager.GetString("Invalid_GroupBy", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to languageId không tồn tại..
        /// </summary>
        public static string Invalid_LanguageIdNotFound {
            get {
                return ResourceManager.GetString("Invalid_LanguageIdNotFound", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to movieId không tồn tại..
        /// </summary>
        public static string Invalid_MovieIdNotFound {
            get {
                return ResourceManager.GetString("Invalid_MovieIdNotFound", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to movieId không tồn tại: {0}..
        /// </summary>
        public static string Invalid_MovieIdsNotFound {
            get {
                return ResourceManager.GetString("Invalid_MovieIdsNotFound", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Dữ liệu tham chiếu không hợp lệ..
        /// </summary>
        public static string Invalid_ReferenceData {
            get {
                return ResourceManager.GetString("Invalid_ReferenceData", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to roleId không tồn tại..
        /// </summary>
        public static string Invalid_RoleIdNotFound {
            get {
                return ResourceManager.GetString("Invalid_RoleIdNotFound", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to roomId không tồn tại..
        /// </summary>
        public static string Invalid_RoomIdNotFound {
            get {
                return ResourceManager.GetString("Invalid_RoomIdNotFound", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to screenTypeId không tồn tại..
        /// </summary>
        public static string Invalid_ScreenTypeIdNotFound {
            get {
                return ResourceManager.GetString("Invalid_ScreenTypeIdNotFound", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to screenType không tồn tại..
        /// </summary>
        public static string Invalid_ScreenTypeNotFound {
            get {
                return ResourceManager.GetString("Invalid_ScreenTypeNotFound", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Một hoặc nhiều seatTypeId không tồn tại..
        /// </summary>
        public static string Invalid_SeatTypeIdsNotFound {
            get {
                return ResourceManager.GetString("Invalid_SeatTypeIdsNotFound", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to showtimeId và seatCount (1-20) là bắt buộc..
        /// </summary>
        public static string Invalid_ShowtimeIdAndSeatCountRequired {
            get {
                return ResourceManager.GetString("Invalid_ShowtimeIdAndSeatCountRequired", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to toDate phải &gt;= date..
        /// </summary>
        public static string Invalid_ToDateBeforeDate {
            get {
                return ResourceManager.GetString("Invalid_ToDateBeforeDate", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Token hết hạn, đã dùng hoặc không hợp lệ..
        /// </summary>
        public static string Invalid_TokenExpiredOrUsed {
            get {
                return ResourceManager.GetString("Invalid_TokenExpiredOrUsed", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Thiếu file upload..
        /// </summary>
        public static string Invalid_UploadFileMissing {
            get {
                return ResourceManager.GetString("Invalid_UploadFileMissing", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Giá trị '{0}' không hợp lệ..
        /// </summary>
        public static string Invalid_Value {
            get {
                return ResourceManager.GetString("Invalid_Value", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Giá trị '{0}' không hợp lệ cho trường {1}..
        /// </summary>
        public static string Invalid_ValueForField {
            get {
                return ResourceManager.GetString("Invalid_ValueForField", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Video vượt quá giới hạn {0}MB..
        /// </summary>
        public static string Invalid_VideoTooLarge {
            get {
                return ResourceManager.GetString("Invalid_VideoTooLarge", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy diễn viên..
        /// </summary>
        public static string NotFound_Actor {
            get {
                return ResourceManager.GetString("NotFound_Actor", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy phân loại độ tuổi..
        /// </summary>
        public static string NotFound_AgeRating {
            get {
                return ResourceManager.GetString("NotFound_AgeRating", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy booking..
        /// </summary>
        public static string NotFound_Booking {
            get {
                return ResourceManager.GetString("NotFound_Booking", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Chưa có cấu hình rạp — liên hệ Admin khởi tạo..
        /// </summary>
        public static string NotFound_CinemaSettings {
            get {
                return ResourceManager.GetString("NotFound_CinemaSettings", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy combo..
        /// </summary>
        public static string NotFound_Combo {
            get {
                return ResourceManager.GetString("NotFound_Combo", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy khách hàng..
        /// </summary>
        public static string NotFound_Customer {
            get {
                return ResourceManager.GetString("NotFound_Customer", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy đạo diễn..
        /// </summary>
        public static string NotFound_Director {
            get {
                return ResourceManager.GetString("NotFound_Director", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy nhân viên..
        /// </summary>
        public static string NotFound_Employee {
            get {
                return ResourceManager.GetString("NotFound_Employee", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy thể loại..
        /// </summary>
        public static string NotFound_Genre {
            get {
                return ResourceManager.GetString("NotFound_Genre", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy ngôn ngữ..
        /// </summary>
        public static string NotFound_Language {
            get {
                return ResourceManager.GetString("NotFound_Language", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy hạng thành viên..
        /// </summary>
        public static string NotFound_MembershipTier {
            get {
                return ResourceManager.GetString("NotFound_MembershipTier", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy phim..
        /// </summary>
        public static string NotFound_Movie {
            get {
                return ResourceManager.GetString("NotFound_Movie", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy giao dịch..
        /// </summary>
        public static string NotFound_Payment {
            get {
                return ResourceManager.GetString("NotFound_Payment", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy vị trí..
        /// </summary>
        public static string NotFound_Position {
            get {
                return ResourceManager.GetString("NotFound_Position", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to positionId không tồn tại..
        /// </summary>
        public static string NotFound_PositionId {
            get {
                return ResourceManager.GetString("NotFound_PositionId", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to QR không hợp lệ..
        /// </summary>
        public static string NotFound_QrCode {
            get {
                return ResourceManager.GetString("NotFound_QrCode", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy đánh giá..
        /// </summary>
        public static string NotFound_Review {
            get {
                return ResourceManager.GetString("NotFound_Review", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy phòng chiếu..
        /// </summary>
        public static string NotFound_Room {
            get {
                return ResourceManager.GetString("NotFound_Room", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy loại màn hình..
        /// </summary>
        public static string NotFound_ScreenType {
            get {
                return ResourceManager.GetString("NotFound_ScreenType", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy loại ghế..
        /// </summary>
        public static string NotFound_SeatType {
            get {
                return ResourceManager.GetString("NotFound_SeatType", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy suất chiếu..
        /// </summary>
        public static string NotFound_Showtime {
            get {
                return ResourceManager.GetString("NotFound_Showtime", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy người dùng..
        /// </summary>
        public static string NotFound_User {
            get {
                return ResourceManager.GetString("NotFound_User", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to userId không tồn tại..
        /// </summary>
        public static string NotFound_UserId {
            get {
                return ResourceManager.GetString("NotFound_UserId", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Số tiền nhận không đủ..
        /// </summary>
        public static string Rule_AmountReceivedInsufficient {
            get {
                return ResourceManager.GetString("Rule_AmountReceivedInsufficient", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Booking thanh toán tại quầy hoặc chưa xác định được giao dịch — vui lòng liên hệ trực tiếp rạp để được hỗ trợ hủy vé..
        /// </summary>
        public static string Rule_BookingCancelAtCounter {
            get {
                return ResourceManager.GetString("Rule_BookingCancelAtCounter", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không thể sinh mã booking duy nhất, vui lòng thử lại..
        /// </summary>
        public static string Rule_BookingCodeGenerationFailed {
            get {
                return ResourceManager.GetString("Rule_BookingCodeGenerationFailed", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Booking đã có vé được check-in — vui lòng liên hệ trực tiếp rạp để được hỗ trợ..
        /// </summary>
        public static string Rule_BookingHasCheckedInTicket {
            get {
                return ResourceManager.GetString("Rule_BookingHasCheckedInTicket", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Booking thiếu cả UserId lẫn CreatedByEmployeeId..
        /// </summary>
        public static string Rule_BookingMissingOwner {
            get {
                return ResourceManager.GetString("Rule_BookingMissingOwner", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Booking không ở trạng thái có thể hủy (đã Cancelled/Expired)..
        /// </summary>
        public static string Rule_BookingNotCancellable {
            get {
                return ResourceManager.GetString("Rule_BookingNotCancellable", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Booking không ở trạng thái Pending hoặc đã hết hạn..
        /// </summary>
        public static string Rule_BookingNotPendingOrExpired {
            get {
                return ResourceManager.GetString("Rule_BookingNotPendingOrExpired", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Booking không ở trạng thái Pending hoặc đã hết hạn giữ ghế..
        /// </summary>
        public static string Rule_BookingNotPendingOrHoldExpired {
            get {
                return ResourceManager.GetString("Rule_BookingNotPendingOrHoldExpired", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tối đa {0} suất chiếu mỗi lần tạo hàng loạt. Hãy thu hẹp khoảng ngày..
        /// </summary>
        public static string Rule_BulkCreateBatchTooLarge {
            get {
                return ResourceManager.GetString("Rule_BulkCreateBatchTooLarge", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CancelDeadlineHours không được âm..
        /// </summary>
        public static string Rule_CancelDeadlineHoursNegative {
            get {
                return ResourceManager.GetString("Rule_CancelDeadlineHoursNegative", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CancelRefundPercent phải trong khoảng 0-100..
        /// </summary>
        public static string Rule_CancelRefundPercentRange {
            get {
                return ResourceManager.GetString("Rule_CancelRefundPercentRange", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không thể đổi phòng chiếu sau khi đã tạo suất chiếu. Hãy hủy và tạo suất chiếu mới..
        /// </summary>
        public static string Rule_CannotChangeShowtimeRoom {
            get {
                return ResourceManager.GetString("Rule_CannotChangeShowtimeRoom", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Chưa tới giờ check-in (mở trước giờ chiếu {0} phút)..
        /// </summary>
        public static string Rule_CheckInNotOpenYet {
            get {
                return ResourceManager.GetString("Rule_CheckInNotOpenYet", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tối đa {0} suất chiếu mỗi lần nhân bản. Hãy thu hẹp khoảng ngày nguồn..
        /// </summary>
        public static string Rule_CloneBatchTooLarge {
            get {
                return ResourceManager.GetString("Rule_CloneBatchTooLarge", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Upload Cloudinary thất bại: {0}.
        /// </summary>
        public static string Rule_CloudinaryUploadFailed {
            get {
                return ResourceManager.GetString("Rule_CloudinaryUploadFailed", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Chỉ có thể thu tiền tại quầy cho booking tạo tại quầy..
        /// </summary>
        public static string Rule_CounterPaymentOnlyForCounterBooking {
            get {
                return ResourceManager.GetString("Rule_CounterPaymentOnlyForCounterBooking", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tài khoản khách hàng đã bị vô hiệu hóa..
        /// </summary>
        public static string Rule_CustomerAccountDisabled {
            get {
                return ResourceManager.GetString("Rule_CustomerAccountDisabled", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Role 'Customer' chưa được seed trong hệ thống..
        /// </summary>
        public static string Rule_CustomerRoleNotSeeded {
            get {
                return ResourceManager.GetString("Rule_CustomerRoleNotSeeded", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Google OAuth chưa được cấu hình (thiếu ClientId)..
        /// </summary>
        public static string Rule_GoogleOAuthNotConfigured {
            get {
                return ResourceManager.GetString("Rule_GoogleOAuthNotConfigured", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MoMo tạo thanh toán thất bại: {0}.
        /// </summary>
        public static string Rule_MomoCreatePaymentFailed {
            get {
                return ResourceManager.GetString("Rule_MomoCreatePaymentFailed", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MoMo trả về response rỗng/không hợp lệ..
        /// </summary>
        public static string Rule_MomoInvalidResponse {
            get {
                return ResourceManager.GetString("Rule_MomoInvalidResponse", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Phim đã ngừng chiếu (Ended), không thể tạo suất chiếu..
        /// </summary>
        public static string Rule_MovieEndedCannotCreateShowtime {
            get {
                return ResourceManager.GetString("Rule_MovieEndedCannotCreateShowtime", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Phim "{0}" đã ngừng chiếu (Ended), không thể xếp lịch..
        /// </summary>
        public static string Rule_MovieEndedCannotSchedule {
            get {
                return ResourceManager.GetString("Rule_MovieEndedCannotSchedule", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Phim đã ngừng chiếu (Ended), không thể cập nhật suất chiếu..
        /// </summary>
        public static string Rule_MovieEndedCannotUpdateShowtime {
            get {
                return ResourceManager.GetString("Rule_MovieEndedCannotUpdateShowtime", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tài khoản không có hồ sơ Employee — không thể thực hiện counter booking..
        /// </summary>
        public static string Rule_NoEmployeeProfileForCounterBooking {
            get {
                return ResourceManager.GetString("Rule_NoEmployeeProfileForCounterBooking", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không có phòng chiếu hoạt động và đã có sơ đồ ghế để xếp lịch..
        /// </summary>
        public static string Rule_NoSchedulableRooms {
            get {
                return ResourceManager.GetString("Rule_NoSchedulableRooms", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không có suất chiếu nào trong khoảng ngày nguồn để nhân bản..
        /// </summary>
        public static string Rule_NoShowtimesToClone {
            get {
                return ResourceManager.GetString("Rule_NoShowtimesToClone", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã ngừng bán vé online cho suất chiếu này (trước giờ chiếu {0} phút)..
        /// </summary>
        public static string Rule_OnlineSalesClosed {
            get {
                return ResourceManager.GetString("Rule_OnlineSalesClosed", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Payment gateway '{0}' chưa được cấu hình..
        /// </summary>
        public static string Rule_PaymentGatewayNotConfigured {
            get {
                return ResourceManager.GetString("Rule_PaymentGatewayNotConfigured", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Hoàn tiền thất bại: {0}. Vui lòng thử lại sau hoặc liên hệ rạp..
        /// </summary>
        public static string Rule_RefundFailed {
            get {
                return ResourceManager.GetString("Rule_RefundFailed", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Cổng thanh toán '{0}' chưa được cấu hình..
        /// </summary>
        public static string Rule_RefundGatewayNotConfigured {
            get {
                return ResourceManager.GetString("Rule_RefundGatewayNotConfigured", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Bạn cần check-in xem phim này trước khi đánh giá..
        /// </summary>
        public static string Rule_ReviewRequiresCheckIn {
            get {
                return ResourceManager.GetString("Rule_ReviewRequiresCheckIn", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Phòng chiếu chưa được thiết lập sơ đồ ghế..
        /// </summary>
        public static string Rule_RoomHasNoSeatLayout {
            get {
                return ResourceManager.GetString("Rule_RoomHasNoSeatLayout", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Phòng đang có suất chiếu sắp tới, không thể xoá..
        /// </summary>
        public static string Rule_RoomHasUpcomingShowtimes {
            get {
                return ResourceManager.GetString("Rule_RoomHasUpcomingShowtimes", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Phòng chiếu không hoạt động (inactive)..
        /// </summary>
        public static string Rule_RoomInactive {
            get {
                return ResourceManager.GetString("Rule_RoomInactive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Booking {0}: ghế giữ đã hết hạn trước khi xác nhận thanh toán..
        /// </summary>
        public static string Rule_SeatsNoLongerHeld {
            get {
                return ResourceManager.GetString("Rule_SeatsNoLongerHeld", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Suất chiếu đã bị hủy..
        /// </summary>
        public static string Rule_ShowtimeCancelled {
            get {
                return ResourceManager.GetString("Rule_ShowtimeCancelled", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Suất chiếu đã kết thúc..
        /// </summary>
        public static string Rule_ShowtimeEnded {
            get {
                return ResourceManager.GetString("Rule_ShowtimeEnded", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Chỉ có thể {0} suất chiếu đang ở trạng thái Scheduled (hiện tại: {1})..
        /// </summary>
        public static string Rule_ShowtimeMustBeScheduled {
            get {
                return ResourceManager.GetString("Rule_ShowtimeMustBeScheduled", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Giờ bắt đầu suất chiếu phải ở tương lai..
        /// </summary>
        public static string Rule_ShowtimeStartMustBeFuture {
            get {
                return ResourceManager.GetString("Rule_ShowtimeStartMustBeFuture", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Suất chiếu đã bắt đầu, không thể hủy..
        /// </summary>
        public static string Rule_ShowtimeStartedCannotCancel {
            get {
                return ResourceManager.GetString("Rule_ShowtimeStartedCannotCancel", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Suất chiếu đã bắt đầu hoặc đã kết thúc..
        /// </summary>
        public static string Rule_ShowtimeStartedOrEnded {
            get {
                return ResourceManager.GetString("Rule_ShowtimeStartedOrEnded", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Suất chiếu không còn khả dụng để đặt vé..
        /// </summary>
        public static string Rule_ShowtimeUnavailableForBooking {
            get {
                return ResourceManager.GetString("Rule_ShowtimeUnavailableForBooking", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Stripe SecretKey chưa được cấu hình..
        /// </summary>
        public static string Rule_StripeSecretKeyMissing {
            get {
                return ResourceManager.GetString("Rule_StripeSecretKeyMissing", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Vé không còn hiệu lực (booking đã hủy hoặc chưa thanh toán)..
        /// </summary>
        public static string Rule_TicketNoLongerValid {
            get {
                return ResourceManager.GetString("Rule_TicketNoLongerValid", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UserId không phải là tài khoản khách hàng..
        /// </summary>
        public static string Rule_UserIsNotCustomer {
            get {
                return ResourceManager.GetString("Rule_UserIsNotCustomer", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to User phải có RoleId là Admin hoặc Employee trước khi tạo hồ sơ Employee (đổi role qua PUT /admin/users/{id}/role)..
        /// </summary>
        public static string Rule_UserMustBeStaffRole {
            get {
                return ResourceManager.GetString("Rule_UserMustBeStaffRole", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã thêm diễn viên mới..
        /// </summary>
        public static string Success_ActorCreated {
            get {
                return ResourceManager.GetString("Success_ActorCreated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã xóa diễn viên..
        /// </summary>
        public static string Success_ActorDeleted {
            get {
                return ResourceManager.GetString("Success_ActorDeleted", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật diễn viên..
        /// </summary>
        public static string Success_ActorUpdated {
            get {
                return ResourceManager.GetString("Success_ActorUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã thêm phân loại độ tuổi mới..
        /// </summary>
        public static string Success_AgeRatingCreated {
            get {
                return ResourceManager.GetString("Success_AgeRatingCreated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã xóa phân loại độ tuổi..
        /// </summary>
        public static string Success_AgeRatingDeleted {
            get {
                return ResourceManager.GetString("Success_AgeRatingDeleted", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật phân loại độ tuổi..
        /// </summary>
        public static string Success_AgeRatingUpdated {
            get {
                return ResourceManager.GetString("Success_AgeRatingUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã hủy đơn đặt vé..
        /// </summary>
        public static string Success_BookingCancelled {
            get {
                return ResourceManager.GetString("Success_BookingCancelled", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã kích hoạt combo..
        /// </summary>
        public static string Success_ComboActivated {
            get {
                return ResourceManager.GetString("Success_ComboActivated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã thêm combo mới..
        /// </summary>
        public static string Success_ComboCreated {
            get {
                return ResourceManager.GetString("Success_ComboCreated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã ngừng bán combo..
        /// </summary>
        public static string Success_ComboDeactivated {
            get {
                return ResourceManager.GetString("Success_ComboDeactivated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật combo..
        /// </summary>
        public static string Success_ComboUpdated {
            get {
                return ResourceManager.GetString("Success_ComboUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Nếu email tồn tại và chưa xác thực, liên kết xác thực mới đã được gửi.
        /// </summary>
        public static string Success_ConfirmationEmailSent {
            get {
                return ResourceManager.GetString("Success_ConfirmationEmailSent", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã thêm đạo diễn mới..
        /// </summary>
        public static string Success_DirectorCreated {
            get {
                return ResourceManager.GetString("Success_DirectorCreated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã xóa đạo diễn..
        /// </summary>
        public static string Success_DirectorDeleted {
            get {
                return ResourceManager.GetString("Success_DirectorDeleted", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật đạo diễn..
        /// </summary>
        public static string Success_DirectorUpdated {
            get {
                return ResourceManager.GetString("Success_DirectorUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Xác thực email thành công.
        /// </summary>
        public static string Success_EmailConfirmed {
            get {
                return ResourceManager.GetString("Success_EmailConfirmed", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã thêm nhân viên mới..
        /// </summary>
        public static string Success_EmployeeCreated {
            get {
                return ResourceManager.GetString("Success_EmployeeCreated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật nhân viên..
        /// </summary>
        public static string Success_EmployeeUpdated {
            get {
                return ResourceManager.GetString("Success_EmployeeUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã thêm thể loại mới..
        /// </summary>
        public static string Success_GenreCreated {
            get {
                return ResourceManager.GetString("Success_GenreCreated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã xóa thể loại..
        /// </summary>
        public static string Success_GenreDeleted {
            get {
                return ResourceManager.GetString("Success_GenreDeleted", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật thể loại..
        /// </summary>
        public static string Success_GenreUpdated {
            get {
                return ResourceManager.GetString("Success_GenreUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã thêm ngôn ngữ mới..
        /// </summary>
        public static string Success_LanguageCreated {
            get {
                return ResourceManager.GetString("Success_LanguageCreated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã xóa ngôn ngữ..
        /// </summary>
        public static string Success_LanguageDeleted {
            get {
                return ResourceManager.GetString("Success_LanguageDeleted", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật ngôn ngữ..
        /// </summary>
        public static string Success_LanguageUpdated {
            get {
                return ResourceManager.GetString("Success_LanguageUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đăng xuất thành công.
        /// </summary>
        public static string Success_LoggedOut {
            get {
                return ResourceManager.GetString("Success_LoggedOut", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã thêm hạng thành viên mới..
        /// </summary>
        public static string Success_MembershipTierCreated {
            get {
                return ResourceManager.GetString("Success_MembershipTierCreated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã xóa hạng thành viên..
        /// </summary>
        public static string Success_MembershipTierDeleted {
            get {
                return ResourceManager.GetString("Success_MembershipTierDeleted", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật hạng thành viên..
        /// </summary>
        public static string Success_MembershipTierUpdated {
            get {
                return ResourceManager.GetString("Success_MembershipTierUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã thêm phim mới thành công..
        /// </summary>
        public static string Success_MovieCreated {
            get {
                return ResourceManager.GetString("Success_MovieCreated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã xóa phim thành công..
        /// </summary>
        public static string Success_MovieDeleted {
            get {
                return ResourceManager.GetString("Success_MovieDeleted", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật phim..
        /// </summary>
        public static string Success_MovieUpdated {
            get {
                return ResourceManager.GetString("Success_MovieUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đổi mật khẩu thành công.
        /// </summary>
        public static string Success_PasswordChanged {
            get {
                return ResourceManager.GetString("Success_PasswordChanged", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đặt lại mật khẩu thành công.
        /// </summary>
        public static string Success_PasswordReset {
            get {
                return ResourceManager.GetString("Success_PasswordReset", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Nếu email tồn tại, hướng dẫn đặt lại mật khẩu đã được gửi.
        /// </summary>
        public static string Success_PasswordResetEmailSent {
            get {
                return ResourceManager.GetString("Success_PasswordResetEmailSent", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã thêm chức vụ mới..
        /// </summary>
        public static string Success_PositionCreated {
            get {
                return ResourceManager.GetString("Success_PositionCreated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã xóa chức vụ..
        /// </summary>
        public static string Success_PositionDeleted {
            get {
                return ResourceManager.GetString("Success_PositionDeleted", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật chức vụ..
        /// </summary>
        public static string Success_PositionUpdated {
            get {
                return ResourceManager.GetString("Success_PositionUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Cập nhật thông tin thành công..
        /// </summary>
        public static string Success_ProfileUpdated {
            get {
                return ResourceManager.GetString("Success_ProfileUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã gửi đánh giá của bạn..
        /// </summary>
        public static string Success_ReviewCreated {
            get {
                return ResourceManager.GetString("Success_ReviewCreated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã xóa đánh giá..
        /// </summary>
        public static string Success_ReviewDeleted {
            get {
                return ResourceManager.GetString("Success_ReviewDeleted", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật đánh giá..
        /// </summary>
        public static string Success_ReviewUpdated {
            get {
                return ResourceManager.GetString("Success_ReviewUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã thêm phòng chiếu mới..
        /// </summary>
        public static string Success_RoomCreated {
            get {
                return ResourceManager.GetString("Success_RoomCreated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã xóa phòng chiếu..
        /// </summary>
        public static string Success_RoomDeleted {
            get {
                return ResourceManager.GetString("Success_RoomDeleted", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã lưu sơ đồ ghế..
        /// </summary>
        public static string Success_RoomSeatsSaved {
            get {
                return ResourceManager.GetString("Success_RoomSeatsSaved", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật phòng chiếu..
        /// </summary>
        public static string Success_RoomUpdated {
            get {
                return ResourceManager.GetString("Success_RoomUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã thêm loại màn hình mới..
        /// </summary>
        public static string Success_ScreenTypeCreated {
            get {
                return ResourceManager.GetString("Success_ScreenTypeCreated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã xóa loại màn hình..
        /// </summary>
        public static string Success_ScreenTypeDeleted {
            get {
                return ResourceManager.GetString("Success_ScreenTypeDeleted", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật loại màn hình..
        /// </summary>
        public static string Success_ScreenTypeUpdated {
            get {
                return ResourceManager.GetString("Success_ScreenTypeUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã thêm loại ghế mới..
        /// </summary>
        public static string Success_SeatTypeCreated {
            get {
                return ResourceManager.GetString("Success_SeatTypeCreated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã xóa loại ghế..
        /// </summary>
        public static string Success_SeatTypeDeleted {
            get {
                return ResourceManager.GetString("Success_SeatTypeDeleted", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật loại ghế..
        /// </summary>
        public static string Success_SeatTypeUpdated {
            get {
                return ResourceManager.GetString("Success_SeatTypeUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật cấu hình rạp..
        /// </summary>
        public static string Success_SettingsUpdated {
            get {
                return ResourceManager.GetString("Success_SettingsUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã tạo suất chiếu mới..
        /// </summary>
        public static string Success_ShowtimeCreated {
            get {
                return ResourceManager.GetString("Success_ShowtimeCreated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã hủy suất chiếu..
        /// </summary>
        public static string Success_ShowtimeDeleted {
            get {
                return ResourceManager.GetString("Success_ShowtimeDeleted", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật suất chiếu..
        /// </summary>
        public static string Success_ShowtimeUpdated {
            get {
                return ResourceManager.GetString("Success_ShowtimeUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã áp dụng lịch chiếu tự động..
        /// </summary>
        public static string Success_ShowtimesAutoScheduleApplied {
            get {
                return ResourceManager.GetString("Success_ShowtimesAutoScheduleApplied", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã tạo suất chiếu hàng loạt..
        /// </summary>
        public static string Success_ShowtimesBulkCreated {
            get {
                return ResourceManager.GetString("Success_ShowtimesBulkCreated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã sao chép suất chiếu..
        /// </summary>
        public static string Success_ShowtimesCloned {
            get {
                return ResourceManager.GetString("Success_ShowtimesCloned", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã khóa tài khoản..
        /// </summary>
        public static string Success_UserLocked {
            get {
                return ResourceManager.GetString("Success_UserLocked", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã cập nhật vai trò..
        /// </summary>
        public static string Success_UserRoleUpdated {
            get {
                return ResourceManager.GetString("Success_UserRoleUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã mở khóa tài khoản..
        /// </summary>
        public static string Success_UserUnlocked {
            get {
                return ResourceManager.GetString("Success_UserUnlocked", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau..
        /// </summary>
        public static string System_UnexpectedError {
            get {
                return ResourceManager.GetString("System_UnexpectedError", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Email Google chưa được xác thực..
        /// </summary>
        public static string Unauthorized_GoogleEmailNotVerified {
            get {
                return ResourceManager.GetString("Unauthorized_GoogleEmailNotVerified", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Google ID token không hợp lệ hoặc đã hết hạn..
        /// </summary>
        public static string Unauthorized_GoogleTokenInvalid {
            get {
                return ResourceManager.GetString("Unauthorized_GoogleTokenInvalid", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Sai email hoặc mật khẩu..
        /// </summary>
        public static string Unauthorized_InvalidCredentials {
            get {
                return ResourceManager.GetString("Unauthorized_InvalidCredentials", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Refresh token đã hết hạn hoặc đã bị thu hồi..
        /// </summary>
        public static string Unauthorized_RefreshTokenExpiredOrRevoked {
            get {
                return ResourceManager.GetString("Unauthorized_RefreshTokenExpiredOrRevoked", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Refresh token không hợp lệ..
        /// </summary>
        public static string Unauthorized_RefreshTokenInvalid {
            get {
                return ResourceManager.GetString("Unauthorized_RefreshTokenInvalid", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to actorId phải lớn hơn 0..
        /// </summary>
        public static string Validation_ActorId_Positive {
            get {
                return ResourceManager.GetString("Validation_ActorId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Danh sách diễn viên không được null..
        /// </summary>
        public static string Validation_ActorList_NotNull {
            get {
                return ResourceManager.GetString("Validation_ActorList_NotNull", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Địa chỉ không được vượt quá 255 ký tự..
        /// </summary>
        public static string Validation_Address_MaxLength255 {
            get {
                return ResourceManager.GetString("Validation_Address_MaxLength255", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Địa chỉ không được để trống..
        /// </summary>
        public static string Validation_Address_Required {
            get {
                return ResourceManager.GetString("Validation_Address_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ageRatingId phải lớn hơn 0..
        /// </summary>
        public static string Validation_AgeRatingId_Positive {
            get {
                return ResourceManager.GetString("Validation_AgeRatingId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to amountReceived phải lớn hơn 0..
        /// </summary>
        public static string Validation_AmountReceived_Positive {
            get {
                return ResourceManager.GetString("Validation_AmountReceived_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tối đa 20 phim mỗi lần xếp lịch..
        /// </summary>
        public static string Validation_AutoSchedule_MaxMovies {
            get {
                return ResourceManager.GetString("Validation_AutoSchedule_MaxMovies", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to basePrice phải &gt;= 0..
        /// </summary>
        public static string Validation_BasePrice_NonNegative {
            get {
                return ResourceManager.GetString("Validation_BasePrice_NonNegative", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to cancelDeadlineHours phải &gt;= 0..
        /// </summary>
        public static string Validation_CancelDeadlineHours_NonNegative {
            get {
                return ResourceManager.GetString("Validation_CancelDeadlineHours_NonNegative", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to cancelRefundPercent phải từ 0 đến 100..
        /// </summary>
        public static string Validation_CancelRefundPercent_Range0To100 {
            get {
                return ResourceManager.GetString("Validation_CancelRefundPercent_Range0To100", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to characterName không được vượt quá 150 ký tự..
        /// </summary>
        public static string Validation_CharacterName_MaxLength150 {
            get {
                return ResourceManager.GetString("Validation_CharacterName_MaxLength150", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to checkInOpenMinutes phải từ 0 đến 240..
        /// </summary>
        public static string Validation_CheckInOpenMinutes_Range0To240 {
            get {
                return ResourceManager.GetString("Validation_CheckInOpenMinutes_Range0To240", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to seatGapRuleMode phải là Strict, AllowEdge hoặc Off..
        /// </summary>
        public static string Validation_SeatGapRuleMode_Invalid {
            get {
                return ResourceManager.GetString("Validation_SeatGapRuleMode_Invalid", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tên rạp không được vượt quá 150 ký tự..
        /// </summary>
        public static string Validation_CinemaName_MaxLength150 {
            get {
                return ResourceManager.GetString("Validation_CinemaName_MaxLength150", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tên rạp không được để trống..
        /// </summary>
        public static string Validation_CinemaName_Required {
            get {
                return ResourceManager.GetString("Validation_CinemaName_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to closingTime phải có định dạng HH:mm..
        /// </summary>
        public static string Validation_ClosingTime_TimeFormat {
            get {
                return ResourceManager.GetString("Validation_ClosingTime_TimeFormat", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to comboId phải lớn hơn 0..
        /// </summary>
        public static string Validation_ComboId_Positive {
            get {
                return ResourceManager.GetString("Validation_ComboId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to customerName không được vượt quá 100 ký tự..
        /// </summary>
        public static string Validation_CustomerName_MaxLength100 {
            get {
                return ResourceManager.GetString("Validation_CustomerName_MaxLength100", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to customerName không được để trống..
        /// </summary>
        public static string Validation_CustomerName_Required {
            get {
                return ResourceManager.GetString("Validation_CustomerName_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to dateRange.from không được để trống..
        /// </summary>
        public static string Validation_DateRangeFrom_Required {
            get {
                return ResourceManager.GetString("Validation_DateRangeFrom_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to dateRange.to phải &gt;= dateRange.from..
        /// </summary>
        public static string Validation_DateRangeTo_BeforeFrom {
            get {
                return ResourceManager.GetString("Validation_DateRangeTo_BeforeFrom", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to dateRange.to không được để trống..
        /// </summary>
        public static string Validation_DateRangeTo_Required {
            get {
                return ResourceManager.GetString("Validation_DateRangeTo_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to dateRange không được null..
        /// </summary>
        public static string Validation_DateRange_NotNull {
            get {
                return ResourceManager.GetString("Validation_DateRange_NotNull", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Khoảng ngày (from → to) không được vượt quá {0} ngày..
        /// </summary>
        public static string Validation_DateRange_TooLong {
            get {
                return ResourceManager.GetString("Validation_DateRange_TooLong", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to date không được để trống..
        /// </summary>
        public static string Validation_Date_Required {
            get {
                return ResourceManager.GetString("Validation_Date_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to daysOfWeek chỉ chấp nhận: {0}.
        /// </summary>
        public static string Validation_DaysOfWeek_AllowedValues {
            get {
                return ResourceManager.GetString("Validation_DaysOfWeek_AllowedValues", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to daysOfWeek không được để trống..
        /// </summary>
        public static string Validation_DaysOfWeek_Required {
            get {
                return ResourceManager.GetString("Validation_DaysOfWeek_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to directorId phải lớn hơn 0..
        /// </summary>
        public static string Validation_DirectorId_Positive {
            get {
                return ResourceManager.GetString("Validation_DirectorId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Email không đúng định dạng..
        /// </summary>
        public static string Validation_Email_Invalid {
            get {
                return ResourceManager.GetString("Validation_Email_Invalid", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Email không được vượt quá 150 ký tự..
        /// </summary>
        public static string Validation_Email_MaxLength150 {
            get {
                return ResourceManager.GetString("Validation_Email_MaxLength150", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to from không được để trống (yyyy-MM-dd)..
        /// </summary>
        public static string Validation_From_Required {
            get {
                return ResourceManager.GetString("Validation_From_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to gender phải là một trong: {0}.
        /// </summary>
        public static string Validation_Gender_AllowedValues {
            get {
                return ResourceManager.GetString("Validation_Gender_AllowedValues", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to genreId phải lớn hơn 0..
        /// </summary>
        public static string Validation_GenreId_Positive {
            get {
                return ResourceManager.GetString("Validation_GenreId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to holdDurationMinutes phải từ 1 đến 120..
        /// </summary>
        public static string Validation_HoldDurationMinutes_Range1To120 {
            get {
                return ResourceManager.GetString("Validation_HoldDurationMinutes_Range1To120", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to idToken không được để trống..
        /// </summary>
        public static string Validation_IdToken_Required {
            get {
                return ResourceManager.GetString("Validation_IdToken_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to items không được để trống..
        /// </summary>
        public static string Validation_Items_Required {
            get {
                return ResourceManager.GetString("Validation_Items_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to languageId phải lớn hơn 0..
        /// </summary>
        public static string Validation_LanguageId_Positive {
            get {
                return ResourceManager.GetString("Validation_LanguageId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to logoUrl không được vượt quá 255 ký tự..
        /// </summary>
        public static string Validation_LogoUrl_MaxLength255 {
            get {
                return ResourceManager.GetString("Validation_LogoUrl_MaxLength255", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Nội dung tin nhắn không được vượt quá 2000 ký tự..
        /// </summary>
        public static string Validation_MessageContent_MaxLength2000 {
            get {
                return ResourceManager.GetString("Validation_MessageContent_MaxLength2000", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Nội dung tin nhắn không được để trống..
        /// </summary>
        public static string Validation_MessageContent_Required {
            get {
                return ResourceManager.GetString("Validation_MessageContent_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Message không được vượt quá 1000 ký tự..
        /// </summary>
        public static string Validation_Message_MaxLength1000 {
            get {
                return ResourceManager.GetString("Validation_Message_MaxLength1000", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Message không được để trống..
        /// </summary>
        public static string Validation_Message_Required {
            get {
                return ResourceManager.GetString("Validation_Message_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to basePrice của phim phải &gt;= 0..
        /// </summary>
        public static string Validation_MovieBasePrice_NonNegative {
            get {
                return ResourceManager.GetString("Validation_MovieBasePrice_NonNegative", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Thời lượng phim phải từ 1 đến 600 phút..
        /// </summary>
        public static string Validation_MovieDuration_Range1To600 {
            get {
                return ResourceManager.GetString("Validation_MovieDuration_Range1To600", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Phải chọn ít nhất 1 thể loại phim..
        /// </summary>
        public static string Validation_MovieGenres_AtLeastOne {
            get {
                return ResourceManager.GetString("Validation_MovieGenres_AtLeastOne", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to movieId phải lớn hơn 0..
        /// </summary>
        public static string Validation_MovieId_Positive {
            get {
                return ResourceManager.GetString("Validation_MovieId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Danh sách movieIds không được rỗng..
        /// </summary>
        public static string Validation_MovieIds_NotEmpty {
            get {
                return ResourceManager.GetString("Validation_MovieIds_NotEmpty", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Trạng thái phim không được để trống..
        /// </summary>
        public static string Validation_MovieStatus_Required {
            get {
                return ResourceManager.GetString("Validation_MovieStatus_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tiêu đề phim không được vượt quá 200 ký tự..
        /// </summary>
        public static string Validation_MovieTitle_MaxLength200 {
            get {
                return ResourceManager.GetString("Validation_MovieTitle_MaxLength200", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tiêu đề phim không được để trống..
        /// </summary>
        public static string Validation_MovieTitle_Required {
            get {
                return ResourceManager.GetString("Validation_MovieTitle_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to movies không được chứa phim trùng lặp..
        /// </summary>
        public static string Validation_Movies_Duplicated {
            get {
                return ResourceManager.GetString("Validation_Movies_Duplicated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to movies không được để trống..
        /// </summary>
        public static string Validation_Movies_Required {
            get {
                return ResourceManager.GetString("Validation_Movies_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to openingTime phải có định dạng HH:mm..
        /// </summary>
        public static string Validation_OpeningTime_TimeFormat {
            get {
                return ResourceManager.GetString("Validation_OpeningTime_TimeFormat", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tên gốc không được vượt quá 200 ký tự..
        /// </summary>
        public static string Validation_OriginalTitle_MaxLength200 {
            get {
                return ResourceManager.GetString("Validation_OriginalTitle_MaxLength200", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Mật khẩu phải có ít nhất 1 chữ số..
        /// </summary>
        public static string Validation_PasswordDigit {
            get {
                return ResourceManager.GetString("Validation_PasswordDigit", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Mật khẩu phải có ít nhất 1 chữ thường..
        /// </summary>
        public static string Validation_PasswordLowercase {
            get {
                return ResourceManager.GetString("Validation_PasswordLowercase", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Mật khẩu phải có ít nhất 8 ký tự..
        /// </summary>
        public static string Validation_PasswordMinLength8 {
            get {
                return ResourceManager.GetString("Validation_PasswordMinLength8", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Mật khẩu phải có ít nhất 1 chữ hoa..
        /// </summary>
        public static string Validation_PasswordUppercase {
            get {
                return ResourceManager.GetString("Validation_PasswordUppercase", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to paymentMethod phải là VNPay, Momo hoặc Stripe..
        /// </summary>
        public static string Validation_PaymentMethod_Allowed {
            get {
                return ResourceManager.GetString("Validation_PaymentMethod_Allowed", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Số điện thoại không hợp lệ..
        /// </summary>
        public static string Validation_Phone_Invalid {
            get {
                return ResourceManager.GetString("Validation_Phone_Invalid", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Số điện thoại không được vượt quá 20 ký tự..
        /// </summary>
        public static string Validation_Phone_MaxLength20 {
            get {
                return ResourceManager.GetString("Validation_Phone_MaxLength20", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to pointsPerAmount phải &gt; 0..
        /// </summary>
        public static string Validation_PointsPerAmount_GreaterThanZero {
            get {
                return ResourceManager.GetString("Validation_PointsPerAmount_GreaterThanZero", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to positionId phải lớn hơn 0..
        /// </summary>
        public static string Validation_PositionId_Positive {
            get {
                return ResourceManager.GetString("Validation_PositionId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to posterUrl không được vượt quá 255 ký tự..
        /// </summary>
        public static string Validation_PosterUrl_MaxLength255 {
            get {
                return ResourceManager.GetString("Validation_PosterUrl_MaxLength255", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to qrCode không được để trống..
        /// </summary>
        public static string Validation_QrCode_Required {
            get {
                return ResourceManager.GetString("Validation_QrCode_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to quantity không được vượt quá {0}..
        /// </summary>
        public static string Validation_Quantity_Max {
            get {
                return ResourceManager.GetString("Validation_Quantity_Max", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to quantity phải lớn hơn 0..
        /// </summary>
        public static string Validation_Quantity_Positive {
            get {
                return ResourceManager.GetString("Validation_Quantity_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Điểm đánh giá (rating) phải từ 1 đến 5..
        /// </summary>
        public static string Validation_Rating_Range1To5 {
            get {
                return ResourceManager.GetString("Validation_Rating_Range1To5", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Nội dung đánh giá không được vượt quá 1000 ký tự..
        /// </summary>
        public static string Validation_ReviewContent_MaxLength1000 {
            get {
                return ResourceManager.GetString("Validation_ReviewContent_MaxLength1000", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to reviewId phải lớn hơn 0..
        /// </summary>
        public static string Validation_ReviewId_Positive {
            get {
                return ResourceManager.GetString("Validation_ReviewId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Danh sách reviewIds không được rỗng..
        /// </summary>
        public static string Validation_ReviewIds_NotEmpty {
            get {
                return ResourceManager.GetString("Validation_ReviewIds_NotEmpty", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to roleId phải lớn hơn 0..
        /// </summary>
        public static string Validation_RoleId_Positive {
            get {
                return ResourceManager.GetString("Validation_RoleId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Role phải là một trong: {0}.
        /// </summary>
        public static string Validation_Role_AllowedValues {
            get {
                return ResourceManager.GetString("Validation_Role_AllowedValues", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Role không được để trống..
        /// </summary>
        public static string Validation_Role_Required {
            get {
                return ResourceManager.GetString("Validation_Role_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to roomId phải lớn hơn 0..
        /// </summary>
        public static string Validation_RoomId_Positive {
            get {
                return ResourceManager.GetString("Validation_RoomId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tên phòng chiếu không được vượt quá 50 ký tự..
        /// </summary>
        public static string Validation_RoomName_MaxLength50 {
            get {
                return ResourceManager.GetString("Validation_RoomName_MaxLength50", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tên phòng chiếu không được để trống..
        /// </summary>
        public static string Validation_RoomName_Required {
            get {
                return ResourceManager.GetString("Validation_RoomName_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to screenTypeId phải lớn hơn 0..
        /// </summary>
        public static string Validation_ScreenTypeId_Positive {
            get {
                return ResourceManager.GetString("Validation_ScreenTypeId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to seatId phải lớn hơn 0..
        /// </summary>
        public static string Validation_SeatId_Positive {
            get {
                return ResourceManager.GetString("Validation_SeatId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to seatNumber phải lớn hơn 0..
        /// </summary>
        public static string Validation_SeatNumber_Positive {
            get {
                return ResourceManager.GetString("Validation_SeatNumber_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to seatRow không được vượt quá 5 ký tự..
        /// </summary>
        public static string Validation_SeatRow_MaxLength5 {
            get {
                return ResourceManager.GetString("Validation_SeatRow_MaxLength5", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to seatRow không được để trống..
        /// </summary>
        public static string Validation_SeatRow_Required {
            get {
                return ResourceManager.GetString("Validation_SeatRow_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to seatTypeId phải lớn hơn 0..
        /// </summary>
        public static string Validation_SeatTypeId_Positive {
            get {
                return ResourceManager.GetString("Validation_SeatTypeId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Phải chọn ít nhất 1 ghế..
        /// </summary>
        public static string Validation_Seats_AtLeastOne {
            get {
                return ResourceManager.GetString("Validation_Seats_AtLeastOne", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Danh sách ghế bị trùng lặp..
        /// </summary>
        public static string Validation_Seats_Duplicated {
            get {
                return ResourceManager.GetString("Validation_Seats_Duplicated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tồn tại ghế trùng vị trí (SeatRow + SeatNumber) trong danh sách..
        /// </summary>
        public static string Validation_Seats_DuplicatedPosition {
            get {
                return ResourceManager.GetString("Validation_Seats_DuplicatedPosition", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Mỗi booking tối đa {0} ghế..
        /// </summary>
        public static string Validation_Seats_MaxPerBooking {
            get {
                return ResourceManager.GetString("Validation_Seats_MaxPerBooking", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Danh sách seats không được rỗng..
        /// </summary>
        public static string Validation_Seats_NotEmpty {
            get {
                return ResourceManager.GetString("Validation_Seats_NotEmpty", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to shiftDays phải từ 1 đến 366..
        /// </summary>
        public static string Validation_ShiftDays_Range1To366 {
            get {
                return ResourceManager.GetString("Validation_ShiftDays_Range1To366", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to shows phải từ 1 đến 20..
        /// </summary>
        public static string Validation_Shows_Range1To20 {
            get {
                return ResourceManager.GetString("Validation_Shows_Range1To20", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to showtimeId phải lớn hơn 0..
        /// </summary>
        public static string Validation_ShowtimeId_Positive {
            get {
                return ResourceManager.GetString("Validation_ShowtimeId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Danh sách showtimeIds không được rỗng..
        /// </summary>
        public static string Validation_ShowtimeIds_NotEmpty {
            get {
                return ResourceManager.GetString("Validation_ShowtimeIds_NotEmpty", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tối đa 1000 suất chiếu mỗi lần áp dụng..
        /// </summary>
        public static string Validation_Showtimes_MaxPerApply {
            get {
                return ResourceManager.GetString("Validation_Showtimes_MaxPerApply", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to sourceFrom không được để trống..
        /// </summary>
        public static string Validation_SourceFrom_Required {
            get {
                return ResourceManager.GetString("Validation_SourceFrom_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Khoảng ngày nguồn không được vượt quá {0} ngày..
        /// </summary>
        public static string Validation_SourceRange_TooLong {
            get {
                return ResourceManager.GetString("Validation_SourceRange_TooLong", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to sourceTo phải &gt;= sourceFrom..
        /// </summary>
        public static string Validation_SourceTo_BeforeSourceFrom {
            get {
                return ResourceManager.GetString("Validation_SourceTo_BeforeSourceFrom", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to sourceTo không được để trống..
        /// </summary>
        public static string Validation_SourceTo_Required {
            get {
                return ResourceManager.GetString("Validation_SourceTo_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to startTime không được để trống..
        /// </summary>
        public static string Validation_StartTime_Required {
            get {
                return ResourceManager.GetString("Validation_StartTime_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Status phải là một trong: {0}.
        /// </summary>
        public static string Validation_Status_AllowedValues {
            get {
                return ResourceManager.GetString("Validation_Status_AllowedValues", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to stopSellingMinutesBefore phải từ 0 đến 1440..
        /// </summary>
        public static string Validation_StopSellingMinutesBefore_Range0To1440 {
            get {
                return ResourceManager.GetString("Validation_StopSellingMinutesBefore_Range0To1440", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Phụ đề không được vượt quá 50 ký tự..
        /// </summary>
        public static string Validation_Subtitle_MaxLength50 {
            get {
                return ResourceManager.GetString("Validation_Subtitle_MaxLength50", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Mã số thuế không được vượt quá 50 ký tự..
        /// </summary>
        public static string Validation_TaxCode_MaxLength50 {
            get {
                return ResourceManager.GetString("Validation_TaxCode_MaxLength50", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to times không được để trống..
        /// </summary>
        public static string Validation_Times_Required {
            get {
                return ResourceManager.GetString("Validation_Times_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to times phải theo định dạng HH:mm..
        /// </summary>
        public static string Validation_Times_TimeFormat {
            get {
                return ResourceManager.GetString("Validation_Times_TimeFormat", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to to phải &gt;= from..
        /// </summary>
        public static string Validation_To_BeforeFrom {
            get {
                return ResourceManager.GetString("Validation_To_BeforeFrom", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to to không được để trống (yyyy-MM-dd)..
        /// </summary>
        public static string Validation_To_Required {
            get {
                return ResourceManager.GetString("Validation_To_Required", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to trailerUrl không được vượt quá 255 ký tự..
        /// </summary>
        public static string Validation_TrailerUrl_MaxLength255 {
            get {
                return ResourceManager.GetString("Validation_TrailerUrl_MaxLength255", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Phải cung cấp userId hoặc customerName..
        /// </summary>
        public static string Validation_UserIdOrCustomerNameRequired {
            get {
                return ResourceManager.GetString("Validation_UserIdOrCustomerNameRequired", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to userId phải lớn hơn 0..
        /// </summary>
        public static string Validation_UserId_Positive {
            get {
                return ResourceManager.GetString("Validation_UserId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Lỗi không xác định..
        /// </summary>
        public static string Bulk_UnknownError {
            get {
                return ResourceManager.GetString("Bulk_UnknownError", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không thể khóa tài khoản đang đăng nhập..
        /// </summary>
        public static string Rule_CannotLockSelf {
            get {
                return ResourceManager.GetString("Rule_CannotLockSelf", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không thể khóa quản trị viên cuối cùng..
        /// </summary>
        public static string Rule_CannotLockLastAdmin {
            get {
                return ResourceManager.GetString("Rule_CannotLockLastAdmin", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không thể tự thay đổi vai trò của chính mình..
        /// </summary>
        public static string Rule_CannotChangeOwnRole {
            get {
                return ResourceManager.GetString("Rule_CannotChangeOwnRole", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không thể hạ vai trò quản trị viên cuối cùng..
        /// </summary>
        public static string Rule_CannotDemoteLastAdmin {
            get {
                return ResourceManager.GetString("Rule_CannotDemoteLastAdmin", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không thể tự vô hiệu hóa hồ sơ nhân viên của chính mình..
        /// </summary>
        public static string Rule_CannotDeactivateSelfEmployee {
            get {
                return ResourceManager.GetString("Rule_CannotDeactivateSelfEmployee", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Mỗi id phải lớn hơn 0..
        /// </summary>
        public static string Validation_BulkId_Positive {
            get {
                return ResourceManager.GetString("Validation_BulkId_Positive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tối đa {0} mục mỗi lần..
        /// </summary>
        public static string Validation_BulkIds_MaxCount {
            get {
                return ResourceManager.GetString("Validation_BulkIds_MaxCount", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Danh sách id không được để trống..
        /// </summary>
        public static string Validation_BulkIds_NotEmpty {
            get {
                return ResourceManager.GetString("Validation_BulkIds_NotEmpty", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tổng hai trọng số Heat (doanh số và đánh giá) phải lớn hơn 0..
        /// </summary>
        public static string Validation_SchedulerHeatWeights_SumPositive {
            get {
                return ResourceManager.GetString("Validation_SchedulerHeatWeights_SumPositive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to {0} phải từ {1} đến {2}..
        /// </summary>
        public static string Validation_Setting_NumberRange {
            get {
                return ResourceManager.GetString("Validation_Setting_NumberRange", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Vé xem phim của bạn — {0}.
        /// </summary>
        public static string Email_BookingConfirmedSubject {
            get {
                return ResourceManager.GetString("Email_BookingConfirmedSubject", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Xác nhận hủy vé &amp; hoàn tiền — {0}.
        /// </summary>
        public static string Email_BookingCancelledRefundedSubject {
            get {
                return ResourceManager.GetString("Email_BookingCancelledRefundedSubject", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to [{0}] Đã tự động hoàn tiền — Booking #{1}.
        /// </summary>
        public static string Email_SeatsExpiredAdminRefundedSubject {
            get {
                return ResourceManager.GetString("Email_SeatsExpiredAdminRefundedSubject", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to [{0}] CẦN HOÀN TIỀN THỦ CÔNG — Booking #{1}.
        /// </summary>
        public static string Email_SeatsExpiredAdminManualRefundSubject {
            get {
                return ResourceManager.GetString("Email_SeatsExpiredAdminManualRefundSubject", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to [{0}] Đã hoàn tiền — Booking {1}.
        /// </summary>
        public static string Email_SeatsExpiredCustomerRefundedSubject {
            get {
                return ResourceManager.GetString("Email_SeatsExpiredCustomerRefundedSubject", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to [{0}] Chờ hoàn tiền — Booking {1}.
        /// </summary>
        public static string Email_SeatsExpiredCustomerPendingRefundSubject {
            get {
                return ResourceManager.GetString("Email_SeatsExpiredCustomerPendingRefundSubject", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Suất chiếu bị hủy — đã hoàn tiền {0}.
        /// </summary>
        public static string Email_ShowtimeCancelledRefundedSubject {
            get {
                return ResourceManager.GetString("Email_ShowtimeCancelledRefundedSubject", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Suất chiếu bạn đặt vé đã bị hủy — {0}.
        /// </summary>
        public static string Email_ShowtimeCancelledManualRefundSubject {
            get {
                return ResourceManager.GetString("Email_ShowtimeCancelledManualRefundSubject", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Xác thực email — {0}.
        /// </summary>
        public static string Email_ConfirmEmailSubject {
            get {
                return ResourceManager.GetString("Email_ConfirmEmailSubject", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Vui lòng xác thực email bằng cách bấm vào liên kết sau.
        /// </summary>
        public static string Email_ConfirmEmailIntro {
            get {
                return ResourceManager.GetString("Email_ConfirmEmailIntro", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Xác thực email.
        /// </summary>
        public static string Email_ConfirmEmailButton {
            get {
                return ResourceManager.GetString("Email_ConfirmEmailButton", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đặt lại mật khẩu — {0}.
        /// </summary>
        public static string Email_PasswordResetSubject {
            get {
                return ResourceManager.GetString("Email_PasswordResetSubject", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Vui lòng bấm vào liên kết sau để đặt lại mật khẩu.
        /// </summary>
        public static string Email_PasswordResetIntro {
            get {
                return ResourceManager.GetString("Email_PasswordResetIntro", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đặt lại mật khẩu.
        /// </summary>
        public static string Email_PasswordResetButton {
            get {
                return ResourceManager.GetString("Email_PasswordResetButton", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Thanh toán booking {0}.
        /// </summary>
        public static string Payment_BookingDescription {
            get {
                return ResourceManager.GetString("Payment_BookingDescription", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Hoàn tiền hủy vé {0}.
        /// </summary>
        public static string Refund_CancelBookingDescription {
            get {
                return ResourceManager.GetString("Refund_CancelBookingDescription", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Hoàn tiền tự động - ghế hết hạn giữ - Payment {0}.
        /// </summary>
        public static string Refund_SeatsExpiredDescription {
            get {
                return ResourceManager.GetString("Refund_SeatsExpiredDescription", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Hoàn tiền do rạp hủy suất chiếu - booking {0}.
        /// </summary>
        public static string Refund_ShowtimeCancelledDescription {
            get {
                return ResourceManager.GetString("Refund_ShowtimeCancelledDescription", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Rạp hiện không hỗ trợ hoàn tiền..
        /// </summary>
        public static string Rule_RefundNotSupported {
            get {
                return ResourceManager.GetString("Rule_RefundNotSupported", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã quá hạn hoàn tiền — chỉ áp dụng trước giờ chiếu ít nhất {0} giờ..
        /// </summary>
        public static string Rule_RefundDeadlineExceeded {
            get {
                return ResourceManager.GetString("Rule_RefundDeadlineExceeded", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Vui lòng kiểm tra email để xác thực.
        /// </summary>
        public static string Success_RegistrationCheckEmail {
            get {
                return ResourceManager.GetString("Success_RegistrationCheckEmail", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Trùng khung giờ với suất chiếu đã có..
        /// </summary>
        public static string Conflict_ShowtimeOverlapExisting {
            get {
                return ResourceManager.GetString("Conflict_ShowtimeOverlapExisting", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Suất chiếu không hợp lệ..
        /// </summary>
        public static string Invalid_ShowtimeInvalid {
            get {
                return ResourceManager.GetString("Invalid_ShowtimeInvalid", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to sửa.
        /// </summary>
        public static string Common_ActionEdit {
            get {
                return ResourceManager.GetString("Common_ActionEdit", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to hủy.
        /// </summary>
        public static string Common_ActionCancel {
            get {
                return ResourceManager.GetString("Common_ActionCancel", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Xin lỗi, tính năng AI Assistant hiện chưa được cấu hình khóa API trên hệ thống. Vui lòng liên hệ Quản trị viên để thiết lập..
        /// </summary>
        public static string Ai_NotConfigured {
            get {
                return ResourceManager.GetString("Ai_NotConfigured", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Rất tiếc, kết nối với trợ lý AI thất bại. Vui lòng thử lại sau..
        /// </summary>
        public static string Ai_ConnectionFailed {
            get {
                return ResourceManager.GetString("Ai_ConnectionFailed", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Trợ lý AI không tạo được câu trả lời phù hợp. Vui lòng thử lại..
        /// </summary>
        public static string Ai_NoReply {
            get {
                return ResourceManager.GetString("Ai_NoReply", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã vượt quá số lần truy vấn cho phép, hãy trả lời với dữ liệu đã có..
        /// </summary>
        public static string Ai_ToolCallLimitExceeded {
            get {
                return ResourceManager.GetString("Ai_ToolCallLimitExceeded", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã xảy ra lỗi trong quá trình xử lý yêu cầu với Trợ lý AI. Vui lòng thử lại sau..
        /// </summary>
        public static string Ai_ProcessingError {
            get {
                return ResourceManager.GetString("Ai_ProcessingError", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tìm suất chiếu sắp tới (còn hiệu lực) theo phim, ngày. Dùng khi khách hỏi lịch chiếu, giờ chiếu, giá vé của một phim hoặc của một ngày..
        /// </summary>
        public static string Ai_Tool_SearchShowtimes_Description {
            get {
                return ResourceManager.GetString("Ai_Tool_SearchShowtimes_Description", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Xem tình trạng ghế trống của một suất chiếu: số ghế còn, giá theo loại ghế, danh sách ghế trống. Dùng khi khách hỏi còn ghế không, ghế nào còn trống..
        /// </summary>
        public static string Ai_Tool_GetSeatAvailability_Description {
            get {
                return ResourceManager.GetString("Ai_Tool_GetSeatAvailability_Description", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Lấy các vé/đơn đặt vé của chính khách đang đăng nhập. Không cần truyền thông tin người dùng..
        /// </summary>
        public static string Ai_Tool_GetMyBookings_Description {
            get {
                return ResourceManager.GetString("Ai_Tool_GetMyBookings_Description", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ID phim (lấy từ danh sách phim trong system prompt)..
        /// </summary>
        public static string Ai_Param_MovieId {
            get {
                return ResourceManager.GetString("Ai_Param_MovieId", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Một phần tên phim, dùng khi không biết movie_id..
        /// </summary>
        public static string Ai_Param_MovieTitle {
            get {
                return ResourceManager.GetString("Ai_Param_MovieTitle", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Ngày chiếu định dạng yyyy-MM-dd. Bỏ trống để tìm trong khoảng lịch chiếu đang mở bán..
        /// </summary>
        public static string Ai_Param_Date {
            get {
                return ResourceManager.GetString("Ai_Param_Date", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ID suất chiếu (lấy từ kết quả search_showtimes)..
        /// </summary>
        public static string Ai_Param_ShowtimeId {
            get {
                return ResourceManager.GetString("Ai_Param_ShowtimeId", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to true: chỉ vé có suất chiếu chưa diễn ra. Mặc định false..
        /// </summary>
        public static string Ai_Param_UpcomingOnly {
            get {
                return ResourceManager.GetString("Ai_Param_UpcomingOnly", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Lọc theo trạng thái..
        /// </summary>
        public static string Ai_Param_BookingStatus {
            get {
                return ResourceManager.GetString("Ai_Param_BookingStatus", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Hàm '{0}' không tồn tại..
        /// </summary>
        public static string Ai_Error_UnknownFunction {
            get {
                return ResourceManager.GetString("Ai_Error_UnknownFunction", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Không truy vấn được dữ liệu, vui lòng thử lại..
        /// </summary>
        public static string Ai_Error_QueryFailed {
            get {
                return ResourceManager.GetString("Ai_Error_QueryFailed", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to date phải có định dạng yyyy-MM-dd..
        /// </summary>
        public static string Ai_Error_DateFormat {
            get {
                return ResourceManager.GetString("Ai_Error_DateFormat", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Thiếu showtime_id..
        /// </summary>
        public static string Ai_Error_MissingShowtimeId {
            get {
                return ResourceManager.GetString("Ai_Error_MissingShowtimeId", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Suất chiếu này đã kết thúc hoặc bị hủy..
        /// </summary>
        public static string Ai_Error_ShowtimeEnded {
            get {
                return ResourceManager.GetString("Ai_Error_ShowtimeEnded", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Cần đăng nhập để xem vé đã đặt. Hãy mời khách đăng nhập..
        /// </summary>
        public static string Ai_Error_LoginRequired {
            get {
                return ResourceManager.GetString("Ai_Error_LoginRequired", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Thường.
        /// </summary>
        public static string Ai_SeatTypeDefault {
            get {
                return ResourceManager.GetString("Ai_SeatTypeDefault", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to {0:HH:mm dd/MM} • {1}.
        /// </summary>
        public static string Ai_Card_ShowtimeSubtitle {
            get {
                return ResourceManager.GetString("Ai_Card_ShowtimeSubtitle", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Từ {0:N0} VNĐ • Còn {1} ghế.
        /// </summary>
        public static string Ai_Card_PriceAndSeats {
            get {
                return ResourceManager.GetString("Ai_Card_PriceAndSeats", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Còn {0}/{1} ghế.
        /// </summary>
        public static string Ai_Card_SeatsLeft {
            get {
                return ResourceManager.GetString("Ai_Card_SeatsLeft", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to {0:HH:mm dd/MM} • {1} • Ghế {2}.
        /// </summary>
        public static string Ai_Card_BookingSubtitle {
            get {
                return ResourceManager.GetString("Ai_Card_BookingSubtitle", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to {0} • {1} • {2:N0} VNĐ.
        /// </summary>
        public static string Ai_Card_BookingStatusLine {
            get {
                return ResourceManager.GetString("Ai_Card_BookingStatusLine", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Chờ thanh toán.
        /// </summary>
        public static string Ai_BookingStatus_Pending {
            get {
                return ResourceManager.GetString("Ai_BookingStatus_Pending", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã xác nhận.
        /// </summary>
        public static string Ai_BookingStatus_Confirmed {
            get {
                return ResourceManager.GetString("Ai_BookingStatus_Confirmed", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đã hủy.
        /// </summary>
        public static string Ai_BookingStatus_Cancelled {
            get {
                return ResourceManager.GetString("Ai_BookingStatus_Cancelled", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Hết hạn.
        /// </summary>
        public static string Ai_BookingStatus_Expired {
            get {
                return ResourceManager.GetString("Ai_BookingStatus_Expired", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Chưa cập nhật.
        /// </summary>
        public static string Ai_Prompt_NotUpdated {
            get {
                return ResourceManager.GetString("Ai_Prompt_NotUpdated", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Khách vãng lai (Chưa đăng nhập).
        /// </summary>
        public static string Ai_Prompt_Guest {
            get {
                return ResourceManager.GetString("Ai_Prompt_Guest", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Thành viên.
        /// </summary>
        public static string Ai_Prompt_DefaultTier {
            get {
                return ResourceManager.GetString("Ai_Prompt_DefaultTier", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tên: {0}, Email: {1}, Điểm tích lũy: {2}, Hạng: {3}.
        /// </summary>
        public static string Ai_Prompt_UserInfo {
            get {
                return ResourceManager.GetString("Ai_Prompt_UserInfo", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Phụ đề.
        /// </summary>
        public static string Ai_Prompt_DefaultSubtitle {
            get {
                return ResourceManager.GetString("Ai_Prompt_DefaultSubtitle", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Đang chiếu.
        /// </summary>
        public static string Ai_Prompt_MovieNowShowing {
            get {
                return ResourceManager.GetString("Ai_Prompt_MovieNowShowing", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Sắp chiếu.
        /// </summary>
        public static string Ai_Prompt_MovieComingSoon {
            get {
                return ResourceManager.GetString("Ai_Prompt_MovieComingSoon", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to - [{0}] {1} ({2}) | Thể loại: {3} | Thời lượng: {4}p | Độ tuổi: {5} | Trạng thái: {6}.
        /// </summary>
        public static string Ai_Prompt_MovieLine {
            get {
                return ResourceManager.GetString("Ai_Prompt_MovieLine", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to - {0}: {1:N0} VNĐ ({2}).
        /// </summary>
        public static string Ai_Prompt_ComboLine {
            get {
                return ResourceManager.GetString("Ai_Prompt_ComboLine", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Khách đã đăng nhập: khi hỏi về vé/đơn đã đặt, hãy gọi hàm get_my_bookings..
        /// </summary>
        public static string Ai_Prompt_LoggedInRule {
            get {
                return ResourceManager.GetString("Ai_Prompt_LoggedInRule", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Khách CHƯA đăng nhập: bạn không có quyền xem vé đã đặt, hãy mời khách đăng nhập để xem vé..
        /// </summary>
        public static string Ai_Prompt_GuestRule {
            get {
                return ResourceManager.GetString("Ai_Prompt_GuestRule", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Bạn là Trợ lý AI thông minh và thân thiện của Rạp chiếu phim {0}....
        /// </summary>
        public static string Ai_Prompt_SystemTemplate {
            get {
                return ResourceManager.GetString("Ai_Prompt_SystemTemplate", resourceCulture);
            }
        }
    
    }
}

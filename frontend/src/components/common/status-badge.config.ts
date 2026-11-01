import {
  MovieStatus,
  ShowtimeStatus,
  BookingStatus,
  PaymentStatus,
  ShowtimeSeatStatus,
  BookingChannel,
  PaymentMethod,
  Role,
} from "@/types/enum";
import { CheckCircle2, Clock, XCircle, type LucideIcon } from "lucide-react";
import type { badgeVariants } from "@/components/ui/badge";
import type { VariantProps } from "class-variance-authority";

export type BadgeVariant = VariantProps<typeof badgeVariants>["variant"];

export interface BadgeItem {
  label: string;
  variant: BadgeVariant;
  icon?: LucideIcon;
  className?: string;
}

export type ActiveStatusType = "default" | "combo" | "employee" | "user" | "room";

export interface ActiveStatusConfig {
  activeText: string;
  inactiveText: string;
  activeVariant?: BadgeVariant;
  inactiveVariant?: BadgeVariant;
  activeClassName?: string;
  inactiveClassName?: string;
}

export const activeStatusConfigMap: Record<ActiveStatusType, ActiveStatusConfig> = {
  default: {
    activeText: "Hoạt động",
    inactiveText: "Không hoạt động",
    activeVariant: "success",
    inactiveVariant: "secondary",
    activeClassName: "font-medium",
    inactiveClassName: "text-muted-foreground",
  },
  combo: {
    activeText: "Đang bán",
    inactiveText: "Ngừng bán",
    activeVariant: "success",
    inactiveVariant: "secondary",
    activeClassName: "font-medium",
    inactiveClassName: "text-muted-foreground",
  },
  employee: {
    activeText: "Đang làm việc",
    inactiveText: "Đã nghỉ",
    activeVariant: "success",
    inactiveVariant: "secondary",
    activeClassName: "font-medium",
    inactiveClassName: "text-muted-foreground",
  },
  user: {
    activeText: "Hoạt động",
    inactiveText: "Đã khóa",
    activeVariant: "success",
    inactiveVariant: "destructive",
    activeClassName: "font-medium",
    inactiveClassName: "font-medium",
  },
  room: {
    activeText: "Hoạt động",
    inactiveText: "Tạm ngưng",
    activeVariant: "success",
    inactiveVariant: "secondary",
    activeClassName: "font-medium",
    inactiveClassName: "text-muted-foreground",
  },
};

export const movieStatusMap: Record<MovieStatus, BadgeItem> = {
  [MovieStatus.NowShowing]: { label: "Đang chiếu", variant: "success" },
  [MovieStatus.ComingSoon]: { label: "Sắp chiếu", variant: "secondary" },
  [MovieStatus.Ended]: { label: "Đã kết thúc", variant: "outline" },
};

export const showtimeStatusMap: Record<ShowtimeStatus, BadgeItem> = {
  [ShowtimeStatus.Scheduled]: { label: "Đã lên lịch", variant: "success" },
  [ShowtimeStatus.Cancelled]: { label: "Đã huỷ", variant: "destructive" },
  [ShowtimeStatus.Completed]: { label: "Đã chiếu xong", variant: "secondary" },
};

export const bookingStatusMap: Record<BookingStatus, BadgeItem> = {
  [BookingStatus.Pending]: { label: "Chờ thanh toán", variant: "warning", icon: Clock },
  [BookingStatus.Confirmed]: { label: "Đã xác nhận", variant: "success", icon: CheckCircle2 },
  [BookingStatus.Cancelled]: { label: "Đã huỷ", variant: "destructive", icon: XCircle },
  [BookingStatus.Expired]: { label: "Hết hạn giữ vé", variant: "outline", icon: XCircle },
};

export const paymentStatusMap: Record<PaymentStatus, BadgeItem> = {
  [PaymentStatus.Pending]: { label: "Chờ xử lý", variant: "warning", icon: Clock },
  [PaymentStatus.Success]: { label: "Đã thanh toán", variant: "success", icon: CheckCircle2 },
  [PaymentStatus.Failed]: { label: "Thất bại", variant: "destructive", icon: XCircle },
  [PaymentStatus.Refunded]: { label: "Đã hoàn tiền", variant: "outline", icon: XCircle },
};

export const showtimeSeatStatusMap: Record<ShowtimeSeatStatus, BadgeItem> = {
  [ShowtimeSeatStatus.Available]: { label: "Còn trống", variant: "success" },
  [ShowtimeSeatStatus.Held]: { label: "Đang giữ", variant: "warning" },
  [ShowtimeSeatStatus.Booked]: { label: "Đã đặt", variant: "destructive" },
};

export const bookingChannelMap: Record<BookingChannel, BadgeItem> = {
  [BookingChannel.Online]: { label: "Online", variant: "outline", className: "font-medium" },
  [BookingChannel.Counter]: { label: "Tại quầy", variant: "secondary", className: "font-medium" },
};

export const paymentMethodMap: Record<PaymentMethod, BadgeItem> = {
  [PaymentMethod.VNPay]: { label: "VNPay", variant: "outline", className: "font-medium" },
  [PaymentMethod.Momo]: { label: "MoMo", variant: "secondary", className: "font-medium" },
  [PaymentMethod.Cash]: { label: "Tiền mặt", variant: "outline", className: "font-medium" },
  [PaymentMethod.Stripe]: { label: "Stripe", variant: "default", className: "font-medium" },
};

export const roleMap: Record<Role, BadgeItem> = {
  [Role.Admin]: { label: "Quản trị viên", variant: "default", className: "font-medium" },
  [Role.Customer]: { label: "Khách hàng", variant: "secondary", className: "font-medium" },
  [Role.Employee]: { label: "Nhân viên", variant: "outline", className: "font-medium" },
};

export interface BadgeStyleConfig {
  variant: BadgeVariant;
  className?: string;
}

export const ageRatingConfigMap: Record<string, BadgeStyleConfig> = {
  P: { variant: "success", className: "font-bold" },
  K: { variant: "success", className: "font-bold" },
  T13: { variant: "warning", className: "font-bold" },
  T16: { variant: "warning", className: "font-bold" },
  T18: { variant: "destructive", className: "font-bold tracking-wider" },
  C: { variant: "destructive", className: "font-bold tracking-wider" },
};

export const defaultAgeRatingConfig: BadgeStyleConfig = {
  variant: "outline",
  className: "font-bold tracking-wider",
};

export const seatTypeConfigMap: Record<string, BadgeStyleConfig> = {
  VIP: {
    variant: "outline",
    className: "border-accent text-accent font-semibold",
  },
  Sweetbox: {
    variant: "outline",
    className: "border-couple/60 text-couple font-semibold",
  },
  Standard: {
    variant: "outline",
    className: "border-border text-foreground font-semibold",
  },
};

export const defaultSeatTypeConfig: BadgeStyleConfig = {
  variant: "outline",
  className: "border-accent text-accent font-semibold",
};

export interface StatusResultConfig {
  icon: LucideIcon;
  label: string;
  description: string;
  className: string;
}

export const bookingResultStatusConfig: Record<BookingStatus, StatusResultConfig> = {
  [BookingStatus.Confirmed]: {
    icon: CheckCircle2,
    label: "Thanh toán thành công",
    description: "Vé của bạn đã được xác nhận. Vé điện tử đã sẵn sàng bên dưới.",
    className: "text-success",
  },
  [BookingStatus.Pending]: {
    icon: Clock,
    label: "Đang xử lý thanh toán",
    description: "Hệ thống đang chờ xác nhận từ cổng thanh toán, vui lòng đợi trong giây lát.",
    className: "text-warning",
  },
  [BookingStatus.Cancelled]: {
    icon: XCircle,
    label: "Đơn đặt vé đã bị huỷ",
    description: "Giao dịch không thành công hoặc đã bị huỷ. Vui lòng đặt lại vé.",
    className: "text-destructive",
  },
  [BookingStatus.Expired]: {
    icon: XCircle,
    label: "Đơn đặt vé đã hết hạn",
    description: "Bạn chưa hoàn tất thanh toán trong thời gian giữ ghế. Vui lòng đặt lại vé.",
    className: "text-destructive",
  },
};

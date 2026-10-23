import { Badge, type badgeVariants } from "@/components/ui/badge";
import type { VariantProps } from "class-variance-authority";
import { cn } from "@/lib/utils";
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

type BadgeVariant = VariantProps<typeof badgeVariants>["variant"];

export type ActiveStatusType = "default" | "combo" | "employee" | "user" | "room";

interface ActiveStatusConfig {
  activeText: string;
  inactiveText: string;
  activeVariant?: BadgeVariant;
  inactiveVariant?: BadgeVariant;
  activeClassName?: string;
  inactiveClassName?: string;
}

const activeStatusConfigMap: Record<ActiveStatusType, ActiveStatusConfig> = {
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

export interface ActiveStatusBadgeProps {
  active?: boolean;
  isActive?: boolean;
  type?: ActiveStatusType;
  activeText?: string;
  inactiveText?: string;
  activeVariant?: BadgeVariant;
  inactiveVariant?: BadgeVariant;
  activeClassName?: string;
  inactiveClassName?: string;
  className?: string;
}

export const ActiveStatusBadge = ({
  active,
  isActive,
  type = "default",
  activeText,
  inactiveText,
  activeVariant,
  inactiveVariant,
  activeClassName,
  inactiveClassName,
  className,
}: ActiveStatusBadgeProps) => {
  const isCurrentActive = active ?? isActive ?? false;
  const config = activeStatusConfigMap[type] || activeStatusConfigMap.default;

  const label = isCurrentActive
    ? (activeText ?? config.activeText)
    : (inactiveText ?? config.inactiveText);

  const variant = isCurrentActive
    ? (activeVariant ?? config.activeVariant ?? "default")
    : (inactiveVariant ?? config.inactiveVariant ?? "secondary");

  const statusClass = isCurrentActive
    ? (activeClassName ?? config.activeClassName)
    : (inactiveClassName ?? config.inactiveClassName);

  return (
    <Badge variant={variant} className={cn(statusClass, className)}>
      {label}
    </Badge>
  );
}

export const IsActiveBadge = (props: ActiveStatusBadgeProps) => {
  return <ActiveStatusBadge {...props} />;
}

export const ComboStatusBadge = ({
  isActive,
  className,
}: {
  isActive?: boolean;
  className?: string;
}) => {
  return <ActiveStatusBadge active={isActive} type="combo" className={className} />;
}

export const EmployeeStatusBadge = ({
  isActive,
  className,
}: {
  isActive?: boolean;
  className?: string;
}) => {
  return <ActiveStatusBadge active={isActive} type="employee" className={className} />;
}

export const UserStatusBadge = ({
  isActive,
  className,
}: {
  isActive?: boolean;
  className?: string;
}) => {
  return <ActiveStatusBadge active={isActive} type="user" className={className} />;
}

export const RoomStatusBadge = ({
  isActive,
  className,
}: {
  isActive?: boolean;
  className?: string;
}) => {
  return <ActiveStatusBadge active={isActive} type="room" className={className} />;
}

const movieStatusMap: Record<
  MovieStatus,
  { label: string; variant: BadgeVariant; className?: string }
> = {
  [MovieStatus.NowShowing]: { label: "Đang chiếu", variant: "success" },
  [MovieStatus.ComingSoon]: { label: "Sắp chiếu", variant: "secondary" },
  [MovieStatus.Ended]: { label: "Đã kết thúc", variant: "outline" },
};

const showtimeStatusMap: Record<
  ShowtimeStatus,
  { label: string; variant: BadgeVariant; className?: string }
> = {
  [ShowtimeStatus.Scheduled]: { label: "Đã lên lịch", variant: "success" },
  [ShowtimeStatus.Cancelled]: { label: "Đã huỷ", variant: "destructive" },
  [ShowtimeStatus.Completed]: { label: "Đã chiếu xong", variant: "secondary" },
};

const bookingStatusMap: Record<
  BookingStatus,
  { label: string; variant: BadgeVariant; className?: string }
> = {
  [BookingStatus.Pending]: { label: "Chờ thanh toán", variant: "warning" },
  [BookingStatus.Confirmed]: { label: "Đã xác nhận", variant: "success" },
  [BookingStatus.Cancelled]: { label: "Đã huỷ", variant: "destructive" },
  [BookingStatus.Expired]: { label: "Hết hạn giữ vé", variant: "outline" },
};

const paymentStatusMap: Record<
  PaymentStatus,
  { label: string; variant: BadgeVariant; className?: string }
> = {
  [PaymentStatus.Pending]: { label: "Chờ xử lý", variant: "warning" },
  [PaymentStatus.Success]: { label: "Thành công", variant: "success" },
  [PaymentStatus.Failed]: { label: "Thất bại", variant: "destructive" },
  [PaymentStatus.Refunded]: { label: "Đã hoàn tiền", variant: "outline" },
};

const showtimeSeatStatusMap: Record<
  ShowtimeSeatStatus,
  { label: string; variant: BadgeVariant; className?: string }
> = {
  [ShowtimeSeatStatus.Available]: { label: "Còn trống", variant: "success" },
  [ShowtimeSeatStatus.Held]: { label: "Đang giữ", variant: "warning" },
  [ShowtimeSeatStatus.Booked]: { label: "Đã đặt", variant: "destructive" },
};

const bookingChannelMap: Record<
  BookingChannel,
  { label: string; variant: BadgeVariant; className?: string }
> = {
  [BookingChannel.Online]: { label: "Online", variant: "outline", className: "font-medium" },
  [BookingChannel.Counter]: { label: "Tại quầy", variant: "secondary", className: "font-medium" },
};

const paymentMethodMap: Record<
  PaymentMethod,
  { label: string; variant: BadgeVariant; className?: string }
> = {
  [PaymentMethod.VNPay]: { label: "VNPay", variant: "outline", className: "font-medium" },
  [PaymentMethod.Momo]: { label: "MoMo", variant: "secondary", className: "font-medium" },
  [PaymentMethod.Cash]: { label: "Tiền mặt", variant: "outline", className: "font-medium" },
  [PaymentMethod.Stripe]: { label: "Stripe", variant: "default", className: "font-medium" },
};

const roleMap: Record<
  Role,
  { label: string; variant: BadgeVariant; className?: string }
> = {
  [Role.Admin]: { label: "Quản trị viên", variant: "default", className: "font-medium" },
  [Role.Customer]: { label: "Khách hàng", variant: "secondary", className: "font-medium" },
  [Role.Employee]: { label: "Nhân viên", variant: "outline", className: "font-medium" },
};

export const MovieStatusBadge = ({
  status,
  className,
}: {
  status: MovieStatus | string;
  className?: string;
}) => {
  const item = movieStatusMap[status as MovieStatus] || {
    label: status,
    variant: "outline",
  };
  return (
    <Badge variant={item.variant} className={cn(item.className, className)}>
      {item.label}
    </Badge>
  );
}

export const ShowtimeStatusBadge = ({
  status,
  className,
}: {
  status: ShowtimeStatus | string;
  className?: string;
}) => {
  const item = showtimeStatusMap[status as ShowtimeStatus] || {
    label: status,
    variant: "outline",
  };
  return (
    <Badge variant={item.variant} className={cn(item.className, className)}>
      {item.label}
    </Badge>
  );
}

export const BookingStatusBadge = ({
  status,
  className,
}: {
  status: BookingStatus | string;
  className?: string;
}) => {
  const item = bookingStatusMap[status as BookingStatus] || {
    label: status,
    variant: "outline",
  };
  return (
    <Badge variant={item.variant} className={cn(item.className, className)}>
      {item.label}
    </Badge>
  );
}

export const PaymentStatusBadge = ({
  status,
  className,
}: {
  status: PaymentStatus | string;
  className?: string;
}) => {
  const item = paymentStatusMap[status as PaymentStatus] || {
    label: status,
    variant: "outline",
  };
  return (
    <Badge variant={item.variant} className={cn(item.className, className)}>
      {item.label}
    </Badge>
  );
}

export const ShowtimeSeatStatusBadge = ({
  status,
  className,
}: {
  status: ShowtimeSeatStatus | string;
  className?: string;
}) => {
  const item = showtimeSeatStatusMap[status as ShowtimeSeatStatus] || {
    label: status,
    variant: "outline",
  };
  return (
    <Badge variant={item.variant} className={cn(item.className, className)}>
      {item.label}
    </Badge>
  );
}

export const BookingChannelBadge = ({
  channel,
  className,
}: {
  channel: BookingChannel | string;
  className?: string;
}) => {
  const item = bookingChannelMap[channel as BookingChannel] || {
    label: channel,
    variant: "outline",
    className: "font-medium",
  };
  return (
    <Badge variant={item.variant} className={cn(item.className, className)}>
      {item.label}
    </Badge>
  );
}

export const PaymentMethodBadge = ({
  method,
  className,
}: {
  method: PaymentMethod | string;
  className?: string;
}) => {
  const item = paymentMethodMap[method as PaymentMethod] || {
    label: method,
    variant: "outline",
    className: "font-medium",
  };
  return (
    <Badge variant={item.variant} className={cn(item.className, className)}>
      {item.label}
    </Badge>
  );
}

export const RoleBadge = ({
  role,
  className,
}: {
  role?: string | null;
  className?: string;
}) => {
  if (!role) return <span className="text-muted-foreground text-sm">—</span>;
  const item = roleMap[role as Role] || {
    label: role,
    variant: "outline",
    className: "font-medium",
  };
  return (
    <Badge variant={item.variant} className={cn(item.className, className)}>
      {item.label}
    </Badge>
  );
}

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

const defaultAgeRatingConfig: BadgeStyleConfig = {
  variant: "outline",
  className: "font-bold tracking-wider",
};

export const AgeRatingBadge = ({
  code,
  className,
  onImage = false,
}: {
  code?: string | null;
  className?: string;
  onImage?: boolean;
}) => {
  if (!code) return <span className="text-muted-foreground text-sm">—</span>;

  const normalized = code.trim().toUpperCase();
  const config = ageRatingConfigMap[normalized] ?? defaultAgeRatingConfig;

  return (
    <Badge
      variant={config.variant}
      className={cn(
        onImage && "cut-xs border-white/10 bg-black/60",
        config.className,
        className,
      )}
    >
      {code}
    </Badge>
  );
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

const defaultSeatTypeConfig: BadgeStyleConfig = {
  variant: "outline",
  className: "border-accent text-accent font-semibold",
};

export const MembershipTierBadge = ({
  tierName,
  className,
}: {
  tierName?: string | null;
  className?: string;
}) => {
  if (!tierName) return <span className="text-muted-foreground text-sm">—</span>;
  return (
    <Badge
      variant="outline"
      className={cn("border-accent text-accent font-semibold", className)}
    >
      {tierName}
    </Badge>
  );
};

export const SeatTypeBadge = ({
  name,
  className,
}: {
  name?: string | null;
  className?: string;
}) => {
  if (!name) return <span className="text-muted-foreground text-sm">—</span>;
  const config = seatTypeConfigMap[name] ?? defaultSeatTypeConfig;

  return (
    <Badge
      variant={config.variant}
      className={cn(config.className, className)}
    >
      {name}
    </Badge>
  );
};

export const PositionBadge = ({
  name,
  className,
}: {
  name?: string | null;
  className?: string;
}) => {
  if (!name) return <span className="text-muted-foreground text-sm">—</span>;
  return (
    <Badge variant="outline" className={cn("font-medium", className)}>
      {name}
    </Badge>
  );
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
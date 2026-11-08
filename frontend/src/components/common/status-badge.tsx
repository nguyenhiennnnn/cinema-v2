import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import type {
  MovieStatus,
  ShowtimeStatus,
  BookingStatus,
  PaymentStatus,
  BookingChannel,
  PaymentMethod,
} from "@/types/enum";
import {
  activeStatusConfigMap,
  ageRatingConfigMap,
  bookingChannelMap,
  bookingStatusMap,
  defaultAgeRatingConfig,
  defaultSeatTypeConfig,
  movieStatusMap,
  paymentMethodMap,
  paymentStatusMap,
  roleMap,
  seatTypeConfigMap,
  showtimeStatusMap,
  type ActiveStatusType,
  type BadgeItem,
  type BadgeVariant,
} from "./status-badge.config";

export {
  ageRatingConfigMap,
  bookingResultStatusConfig,
  seatTypeConfigMap,
  type ActiveStatusType,
  type BadgeStyleConfig,
  type StatusResultConfig,
} from "./status-badge.config";

const EmptyValue = () => <span className="text-muted-foreground text-sm">—</span>;

interface MappedBadgeProps {
  value: string;
  map: Record<string, BadgeItem>;
  fallbackClassName?: string;
  className?: string;
}

const MappedBadge = ({ value, map, fallbackClassName, className }: MappedBadgeProps) => {
  const item: BadgeItem = map[value] ?? {
    label: value,
    variant: "outline",
    className: fallbackClassName,
  };
  const Icon = item.icon;
  return (
    <Badge variant={item.variant} className={cn(item.className, className)}>
      {Icon && <Icon className="size-3" />}
      {item.label}
    </Badge>
  );
};

type StatusBadgeProps<T extends string> = { status: T | string; className?: string };

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
};

type ActiveBadgeProps = { isActive?: boolean; className?: string };

const createActiveBadge = (type: ActiveStatusType) => {
  const Component = ({ isActive, className }: ActiveBadgeProps) => (
    <ActiveStatusBadge active={isActive} type={type} className={className} />
  );
  return Component;
};

export const ComboStatusBadge = createActiveBadge("combo");
export const EmployeeStatusBadge = createActiveBadge("employee");
export const UserStatusBadge = createActiveBadge("user");
export const RoomStatusBadge = createActiveBadge("room");

export const MovieStatusBadge = ({ status, className }: StatusBadgeProps<MovieStatus>) => (
  <MappedBadge value={status} map={movieStatusMap} className={className} />
);

export const ShowtimeStatusBadge = ({ status, className }: StatusBadgeProps<ShowtimeStatus>) => (
  <MappedBadge value={status} map={showtimeStatusMap} className={className} />
);

export const BookingStatusBadge = ({ status, className }: StatusBadgeProps<BookingStatus>) => (
  <MappedBadge value={status} map={bookingStatusMap} className={className} />
);

export const PaymentStatusBadge = ({ status, className }: StatusBadgeProps<PaymentStatus>) => (
  <MappedBadge value={status} map={paymentStatusMap} className={className} />
);

export const BookingChannelBadge = ({
  channel,
  className,
}: {
  channel: BookingChannel | string;
  className?: string;
}) => (
  <MappedBadge
    value={channel}
    map={bookingChannelMap}
    fallbackClassName="font-medium"
    className={className}
  />
);

export const PaymentMethodBadge = ({
  method,
  className,
}: {
  method: PaymentMethod | string;
  className?: string;
}) => (
  <MappedBadge
    value={method}
    map={paymentMethodMap}
    fallbackClassName="font-medium"
    className={className}
  />
);

export const RoleBadge = ({ role, className }: { role?: string | null; className?: string }) =>
  role ? (
    <MappedBadge value={role} map={roleMap} fallbackClassName="font-medium" className={className} />
  ) : (
    <EmptyValue />
  );

export const AgeRatingBadge = ({
  code,
  className,
  onImage = false,
}: {
  code?: string | null;
  className?: string;
  onImage?: boolean;
}) => {
  if (!code) return <EmptyValue />;

  const config = ageRatingConfigMap[code.trim().toUpperCase()] ?? defaultAgeRatingConfig;

  return (
    <span
      className={cn(
        "inline-flex h-7 min-w-7 shrink-0 items-center justify-center cut-xs px-1.5 font-heading text-xs font-bold leading-none select-none",
        config.variant === "success" && "bg-success text-success-foreground",
        config.variant === "warning" && "bg-warning text-warning-foreground",
        config.variant === "destructive" && "bg-destructive text-destructive-foreground",
        config.variant === "outline" && "bg-secondary text-secondary-foreground",
        onImage && "border border-scrim-foreground/20",
        className,
      )}
    >
      {code}
    </span>
  );
};

export const MembershipTierBadge = ({
  tierName,
  className,
}: {
  tierName?: string | null;
  className?: string;
}) =>
  tierName ? (
    <Badge variant="outline" className={cn("border-accent text-accent font-semibold", className)}>
      {tierName}
    </Badge>
  ) : (
    <EmptyValue />
  );

export const SeatTypeBadge = ({ name, className }: { name?: string | null; className?: string }) => {
  if (!name) return <EmptyValue />;
  const config = seatTypeConfigMap[name] ?? defaultSeatTypeConfig;
  return (
    <Badge variant={config.variant} className={cn(config.className, className)}>
      {name}
    </Badge>
  );
};

export const PositionBadge = ({ name, className }: { name?: string | null; className?: string }) =>
  name ? (
    <Badge variant="outline" className={cn("font-medium", className)}>
      {name}
    </Badge>
  ) : (
    <EmptyValue />
  );

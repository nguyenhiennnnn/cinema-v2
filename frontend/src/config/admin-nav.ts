import {
  LayoutDashboard,
  ShoppingBag,
  ScanLine,
  Film,
  Calendar,
  Tv,
  Popcorn,
  Ticket,
  CreditCard,
  BarChart3,
  Users,
  ShieldAlert,
  MessageSquare,
  Layers,
  Settings,
  type LucideIcon,
} from "lucide-react";
import { ROUTES } from "@/config/routes";
import { Role } from "@/types/enum";

export interface AdminNavItem {
  to: string;
  label: string;
  icon: LucideIcon;
  roles: Role[];
}

export interface AdminNavGroup {
  label: string;
  items: AdminNavItem[];
}

export const ADMIN_NAV_GROUPS: AdminNavGroup[] = [
  {
    label: "Tổng quan",
    items: [
      {
        to: ROUTES.admin.dashboard,
        label: "Dashboard",
        icon: LayoutDashboard,
        roles: [Role.Admin],
      },
    ],
  },
  {
    label: "Nghiệp vụ rạp",
    items: [
      {
        to: ROUTES.admin.bookingCounter,
        label: "Bán vé tại quầy",
        icon: ShoppingBag,
        roles: [Role.Admin, Role.Employee],
      },
      {
        to: ROUTES.admin.checkin,
        label: "Soát vé",
        icon: ScanLine,
        roles: [Role.Admin, Role.Employee],
      },
    ],
  },
  {
    label: "Quản lý rạp & phim",
    items: [
      {
        to: ROUTES.admin.movies,
        label: "Quản lý phim",
        icon: Film,
        roles: [Role.Admin, Role.Employee],
      },
      {
        to: ROUTES.admin.showtimes,
        label: "Suất chiếu",
        icon: Calendar,
        roles: [Role.Admin, Role.Employee],
      },
      { to: ROUTES.admin.rooms, label: "Phòng chiếu", icon: Tv, roles: [Role.Admin] },
      {
        to: ROUTES.admin.combos,
        label: "Bắp nước & Combo",
        icon: Popcorn,
        roles: [Role.Admin],
      },
    ],
  },
  {
    label: "Đơn hàng & tài chính",
    items: [
      {
        to: ROUTES.admin.bookings,
        label: "Quản lý đặt vé",
        icon: Ticket,
        roles: [Role.Admin, Role.Employee],
      },
      {
        to: ROUTES.admin.payments,
        label: "Giao dịch & thanh toán",
        icon: CreditCard,
        roles: [Role.Admin],
      },
      {
        to: ROUTES.admin.reports,
        label: "Báo cáo & doanh thu",
        icon: BarChart3,
        roles: [Role.Admin],
      },
    ],
  },
  {
    label: "Người dùng & đánh giá",
    items: [
      { to: ROUTES.admin.users, label: "Khách hàng", icon: Users, roles: [Role.Admin] },
      {
        to: ROUTES.admin.employees,
        label: "Nhân viên",
        icon: ShieldAlert,
        roles: [Role.Admin],
      },
      {
        to: ROUTES.admin.reviews,
        label: "Đánh giá phim",
        icon: MessageSquare,
        roles: [Role.Admin],
      },
    ],
  },
  {
    label: "Hệ thống & danh mục",
    items: [
      { to: ROUTES.admin.catalog, label: "Trung tâm danh mục", icon: Layers, roles: [Role.Admin] },
      {
        to: ROUTES.admin.settings,
        label: "Cài đặt hệ thống",
        icon: Settings,
        roles: [Role.Admin],
      },
    ],
  },
];

export const ADMIN_ACCOUNT_PATHS: { to: string; roles: Role[] }[] = [
  { to: ROUTES.admin.changePassword, roles: [Role.Admin, Role.Employee] },
];

export const filterNavGroupsByRole = (
  groups: AdminNavGroup[],
  role: string | undefined,
): AdminNavGroup[] =>
  groups
    .map((group) => ({
      ...group,
      items: group.items.filter((item) => !role || item.roles.includes(role as Role)),
    }))
    .filter((group) => group.items.length > 0);

export const canAccessAdminPath = (pathname: string, role: string | undefined): boolean => {
  const item = [...ADMIN_NAV_GROUPS.flatMap((group) => group.items), ...ADMIN_ACCOUNT_PATHS]
    .filter((nav) => pathname === nav.to || pathname.startsWith(`${nav.to}/`))
    .sort((a, b) => b.to.length - a.to.length)[0];
  return !item || (!!role && item.roles.includes(role as Role));
};

export interface AdminBreadcrumb {
  label: string;
  to: string;
}

const ADMIN_SEGMENT_LABELS: Record<string, string> = {
  genres: "Thể loại phim",
  languages: "Ngôn ngữ",
  "age-ratings": "Phân loại độ tuổi",
  "seat-types": "Loại ghế",
  "screen-types": "Loại màn hình",
  "membership-tiers": "Hạng thành viên",
  positions: "Chức vụ",
  actors: "Diễn viên",
  directors: "Đạo diễn",
  seats: "Sơ đồ ghế",
};

const ADMIN_NAV_LABELS = new Map(
  ADMIN_NAV_GROUPS.flatMap((group) => group.items.map((item) => [item.to, item.label] as const)),
);

export const getAdminBreadcrumbs = (pathname: string): AdminBreadcrumb[] => {
  const segments = pathname.split("/").filter(Boolean).slice(1);

  return segments.flatMap((segment, index) => {
    const to = `${ROUTES.admin.root}/${segments.slice(0, index + 1).join("/")}`;
    const isId = /^\d+$/.test(segment);
    if (isId && index < segments.length - 1) return [];
    const label = isId ? "Chi tiết" : (ADMIN_NAV_LABELS.get(to) ?? ADMIN_SEGMENT_LABELS[segment]);
    return label ? [{ label, to }] : [];
  });
};

import { NavLink } from "react-router";
import { cn } from "@/lib/utils";
import { useAuthStore } from "@/stores/auth-store";
import { Role } from "@/types/enum";
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
} from "lucide-react";
import { BrandMark } from "@/components/common/brand-mark";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";

const NAV_GROUPS = [
  {
    label: "Tổng quan",
    items: [
      {
        to: "/admin/dashboard",
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
        to: "/admin/bookings/counter",
        label: "Bán vé tại quầy",
        icon: ShoppingBag,
        roles: [Role.Admin, Role.Employee],
      },
      {
        to: "/admin/checkin",
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
        to: "/admin/movies",
        label: "Quản lý phim",
        icon: Film,
        roles: [Role.Admin, Role.Employee],
      },
      {
        to: "/admin/showtimes",
        label: "Suất chiếu",
        icon: Calendar,
        roles: [Role.Admin, Role.Employee],
      },
      { to: "/admin/rooms", label: "Phòng chiếu", icon: Tv, roles: [Role.Admin] },
      {
        to: "/admin/combos",
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
        to: "/admin/bookings",
        label: "Quản lý đặt vé",
        icon: Ticket,
        roles: [Role.Admin, Role.Employee],
      },
      {
        to: "/admin/payments",
        label: "Giao dịch & thanh toán",
        icon: CreditCard,
        roles: [Role.Admin],
      },
      {
        to: "/admin/reports",
        label: "Báo cáo & doanh thu",
        icon: BarChart3,
        roles: [Role.Admin],
      },
    ],
  },
  {
    label: "Người dùng & đánh giá",
    items: [
      { to: "/admin/users", label: "Khách hàng", icon: Users, roles: [Role.Admin] },
      {
        to: "/admin/employees",
        label: "Nhân viên",
        icon: ShieldAlert,
        roles: [Role.Admin],
      },
      {
        to: "/admin/reviews",
        label: "Đánh giá phim",
        icon: MessageSquare,
        roles: [Role.Admin],
      },
    ],
  },
  {
    label: "Hệ thống & danh mục",
    items: [
      { to: "/admin/catalog", label: "Trung tâm danh mục", icon: Layers, roles: [Role.Admin] },
      {
        to: "/admin/settings",
        label: "Cài đặt hệ thống",
        icon: Settings,
        roles: [Role.Admin],
      },
    ],
  },
];

export const AdminSidebar = ({ collapsed }: { collapsed: boolean }) => {
  const { user } = useAuthStore();
  const role = user?.role;

  const visibleGroups = NAV_GROUPS.map((group) => ({
    ...group,
    items: group.items.filter((item) => !role || item.roles.includes(role as Extract<Role, "Admin" | "Employee">)),
  })).filter((group) => group.items.length > 0);

  return (
    <aside
      className={cn(
        "fixed top-0 bottom-0 left-0 z-40 flex flex-col border-r border-sidebar-border bg-sidebar transition-all duration-300",
        collapsed ? "w-18" : "w-64",
      )}
    >
      <div className="flex h-16 items-center gap-2 border-b border-sidebar-border px-4">
        <BrandMark />
        {!collapsed && (
          <div className="min-w-0">
            <p className="truncate font-heading text-sm font-semibold tracking-wide text-sidebar-foreground uppercase">
              Cinema Admin
            </p>
            <p className="truncate font-mono text-[10px] tracking-wider text-sidebar-foreground/40 uppercase">
              Control Room
            </p>
          </div>
        )}
      </div>

      <nav className="flex-1 overflow-y-auto px-2 py-3">
        {visibleGroups.map((group, gi) => (
          <div key={group.label} className="mb-3">
            {!collapsed && (
              <p className="flex items-baseline gap-1.5 px-3 pb-1 font-mono text-[10px] font-medium tracking-wider text-sidebar-foreground/35 uppercase">
                <span>{String(gi + 1).padStart(2, "0")}</span>
                {group.label}
              </p>
            )}
            {group.items.map(({ to, label, icon: Icon }) => {
              const link = (
                <NavLink
                  key={to}
                  to={to}
                  end={true}
                  title={collapsed ? label : undefined}
                  className={({ isActive }) =>
                    cn(
                      "flex items-center gap-2.5 border-l-2 border-transparent px-3 py-2 text-sm font-medium text-sidebar-foreground/70 transition-colors hover:border-sidebar-foreground/30 hover:bg-sidebar-accent hover:text-sidebar-foreground",
                      isActive &&
                      "border-sidebar-primary bg-sidebar-accent text-sidebar-foreground",
                      collapsed && "justify-center",
                    )
                  }
                >
                  <Icon className="size-4 shrink-0" />
                  {!collapsed && <span className="truncate">{label}</span>}
                </NavLink>
              )

              if (!collapsed) return link;

              return (
                <Tooltip key={to}>
                  <TooltipTrigger render={link} />
                  <TooltipContent side="right">{label}</TooltipContent>
                </Tooltip>
              );
            })}
          </div>
        ))}
      </nav>

      <div className="flex items-center gap-2 border-t border-sidebar-border p-3">
        <img
          src={user?.avatar ?? "/icons.svg"}
          alt={user?.fullName}
          className="size-8 shrink-0 border border-sidebar-border object-cover"
        />
        {!collapsed && (
          <div className="min-w-0">
            <p className="truncate text-xs font-medium text-sidebar-foreground">
              {user?.fullName}
            </p>
            <p className="truncate font-mono text-[10px] tracking-wide text-sidebar-foreground/40 uppercase">
              {user?.role}
            </p>
          </div>
        )}
      </div>
    </aside>
  );
};

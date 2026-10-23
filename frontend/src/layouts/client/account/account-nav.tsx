import { NavLink } from "react-router";
import { User, Ticket, Star, KeyRound, LogOut } from "lucide-react";
import { cn } from "@/lib/utils";

const ACCOUNT_NAV = [
  { to: "/account/profile", label: "Thông tin cá nhân", icon: User },
  { to: "/account/tickets", label: "Vé đã mua", icon: Ticket },
  { to: "/account/reviews", label: "Đánh giá của tôi", icon: Star },
  { to: "/account/change-password", label: "Đổi mật khẩu", icon: KeyRound },
];

interface AccountNavProps {
  onLogout: () => void;
}

export const AccountNav = ({ onLogout }: AccountNavProps) => (
  <nav className="mt-4 flex flex-col gap-1 border border-border bg-card p-2">
    {ACCOUNT_NAV.map(({ to, label, icon: Icon }) => (
      <NavLink
        key={to}
        to={to}
        className={({ isActive }) =>
          cn(
            "flex items-center gap-2.5 px-3 py-2 text-sm font-medium text-muted-foreground transition-colors hover:bg-muted hover:text-foreground",
            isActive && "bg-primary/10 text-primary",
          )
        }
      >
        <Icon className="size-4" />
        {label}
      </NavLink>
    ))}
    <div className="my-1 border-t border-border" />
    <button
      onClick={onLogout}
      className="flex items-center gap-2.5 px-3 py-2 text-sm font-medium text-destructive transition-colors hover:bg-destructive/10"
    >
      <LogOut className="size-4" />
      Đăng xuất
    </button>
  </nav>
);
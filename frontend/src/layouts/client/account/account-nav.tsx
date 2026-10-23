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
  <nav className="mt-4 flex flex-col gap-1 cut-md bg-card p-2">
    {ACCOUNT_NAV.map(({ to, label, icon: Icon }) => (
      <NavLink
        key={to}
        to={to}
        className={({ isActive }) =>
          cn(
            "flex items-center gap-2.5 px-3 py-2 text-sm font-medium text-muted-foreground cut-xs transition-colors hover:bg-muted hover:text-foreground",
            isActive && "bg-primary text-primary-foreground font-semibold hover:bg-primary hover:text-primary-foreground",
          )
        }
      >
        <Icon className="size-4" />
        {label}
      </NavLink>
    ))}
    <div className="my-1 border-t border-border/70" />
    <button
      onClick={onLogout}
      className="flex items-center gap-2.5 px-3 py-2 text-sm font-medium text-destructive cut-xs transition-colors hover:bg-destructive/10 cursor-pointer"
    >
      <LogOut className="size-4" />
      Đăng xuất
    </button>
  </nav>
);

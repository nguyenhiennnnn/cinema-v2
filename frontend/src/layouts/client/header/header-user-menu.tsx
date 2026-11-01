import { Link } from "react-router";
import { Star } from "lucide-react";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { useLogoutConfirm } from "@/hooks/use-logout-confirm";
import { ROUTES } from "@/config/routes";
import { Role } from "@/types/enum";
import { cn } from "@/lib/utils";

interface HeaderUserMenuUser {
  avatar?: string | null;
  fullName: string;
  email: string;
  membershipTierName?: string | null;
  points?: number;
  role?: string;
}

interface HeaderUserMenuProps {
  user: HeaderUserMenuUser;
}

export const HeaderUserMenu = ({ user }: HeaderUserMenuProps) => {
  const confirmLogout = useLogoutConfirm();

  return (
    <>
      {/* Points chip — gọn gàng, hài hòa với header */}
      <Link
        to={ROUTES.account.profile}
        className={cn(
          "cut-xs hidden sm:inline-flex items-center gap-1.5 px-2.5 py-1",
          "bg-secondary text-secondary-foreground",
          "text-[13px] font-medium leading-none",
          "transition-colors hover:bg-muted",
        )}
        title={`${user.membershipTierName ?? "Thành viên"} — ${user.points ?? 0} điểm`}
      >
        <Star className="size-3.5 fill-accent text-accent shrink-0" />
        <span className="t-numeral text-[13px] text-foreground">{user.points ?? 0}</span>
        <span className="text-muted-foreground hidden md:inline">điểm</span>
      </Link>
      <DropdownMenu>
        <DropdownMenuTrigger className="rounded-full outline-none ring-ring/50 focus-visible:ring-2">
          <img
            src={user.avatar ?? "/icons.svg"}
            alt={user.fullName}
            className="size-9 rounded-full border border-border object-cover"
          />
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end" className="w-56">
          <DropdownMenuLabel>
            <p className="font-medium text-foreground">{user.fullName}</p>
            <p className="text-xs font-normal text-muted-foreground">{user.email}</p>
            {user.membershipTierName && (
              <p className="mt-1 flex items-center gap-1 text-xs font-medium text-accent">
                <Star className="size-3 fill-accent" />
                {user.membershipTierName}
              </p>
            )}
          </DropdownMenuLabel>
          <DropdownMenuSeparator />
          <DropdownMenuItem>
            <Link to={ROUTES.account.profile}>Thông tin tài khoản</Link>
          </DropdownMenuItem>
          <DropdownMenuItem>
            <Link to={ROUTES.account.tickets}>Vé của tôi</Link>
          </DropdownMenuItem>
          <DropdownMenuItem>
            <Link to={ROUTES.account.reviews}>Đánh giá của tôi</Link>
          </DropdownMenuItem>
          <DropdownMenuItem>
            <Link to={ROUTES.account.changePassword}>Đổi mật khẩu</Link>
          </DropdownMenuItem>
          {user.role === Role.Admin && (
            <DropdownMenuItem>
              <Link to={ROUTES.admin.dashboard}>Trang quản trị</Link>
            </DropdownMenuItem>
          )}
          <DropdownMenuSeparator />
          <DropdownMenuItem className="text-destructive" onClick={confirmLogout}>
            Đăng xuất
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </>
  );
};
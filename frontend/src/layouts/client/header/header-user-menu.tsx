import { Link } from "react-router";
import { Badge } from "@/components/ui/badge";
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
      <Badge variant="outline" className="hidden border-accent text-accent sm:flex">
        {user.membershipTierName ?? "Thành viên"}, {user.points ?? 0} điểm
      </Badge>
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
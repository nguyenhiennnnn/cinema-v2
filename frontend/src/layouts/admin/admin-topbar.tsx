import { ModeToggle } from "@/components/common/mode-toggle";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { useAuthUser } from "@/stores/auth-store";
import { Bell, Globe, PanelLeft } from "lucide-react";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";
import { Link } from "react-router";
import { useLogoutConfirm } from "@/hooks/use-logout-confirm";

export const AdminTopbar = ({ onToggle }: { onToggle: () => void }) => {
  const user = useAuthUser();
  const confirmLogout = useLogoutConfirm();

  return (
    <header className="sticky top-0 z-30 flex h-16 w-full items-center gap-4 border-b border-border bg-background px-4">
      <Tooltip>
        <TooltipTrigger render={<Button variant="ghost" size="icon" onClick={onToggle} />}>
          <PanelLeft className="size-5" />
        </TooltipTrigger>
        <TooltipContent>Thu gọn / mở rộng menu</TooltipContent>
      </Tooltip>

      <span className="hidden text-xs text-muted-foreground md:block">
        Quản trị hệ thống
      </span>

      <div className="ml-auto flex items-center gap-1">
        <Tooltip>
          <TooltipTrigger
            render={
              <Button variant="ghost" size="icon">
                <a href="/" target="_blank" rel="noreferrer">
                  <Globe className="size-4" />
                </a>
              </Button>
            }
          />
          <TooltipContent>Xem trang khách hàng</TooltipContent>
        </Tooltip>
        <Tooltip>
          <TooltipTrigger render={<Button variant="ghost" size="icon" className="relative" />}>
            <Bell className="size-4" />
            <Badge className="absolute -top-1 -right-1 h-5 min-w-5 justify-center px-1 text-xs">
              3
            </Badge>
          </TooltipTrigger>
          <TooltipContent>Thông báo hệ thống</TooltipContent>
        </Tooltip>
        <ModeToggle />

        <DropdownMenu>
          <DropdownMenuTrigger className="ml-1 rounded-full outline-none ring-ring/50 focus-visible:ring-2">
            <img
              src={user?.avatar ?? "/icons.svg"}
              alt={user?.fullName}
              className="size-9 rounded-full border border-border object-cover"
            />
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-56">
            <DropdownMenuLabel>
              <p className="font-medium text-foreground">{user?.fullName}</p>
              <p className="text-xs font-normal text-muted-foreground">
                {user?.role}
              </p>
            </DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem>
              <Link to="/admin/settings">Cài đặt hệ thống</Link>
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem className="text-destructive" onClick={confirmLogout}>
              Đăng xuất
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </header>
  );
};

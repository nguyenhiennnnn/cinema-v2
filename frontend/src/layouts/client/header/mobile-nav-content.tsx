import { NavLink, Link } from "react-router";
import { useIsAuthenticated } from "@/stores/auth-store";
import { buttonVariants } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { NAV_LINKS } from "./header-desktop-nav";

interface MobileNavContent {
  onNavigate: () => void;
}

export const MobileNavContent = ({ onNavigate }: MobileNavContent) => {
  const isAuthenticated = useIsAuthenticated();

  return (
    <div className="flex flex-col gap-1">
      {NAV_LINKS.map((item) => (
        <NavLink
          key={item.to}
          to={item.to}
          onClick={onNavigate}
          className="px-3 py-2.5 text-base font-medium text-foreground hover:bg-muted transition-colors"
        >
          {item.label}
        </NavLink>
      ))}
      {!isAuthenticated && (
        <div className="mt-4 flex flex-col gap-2 pt-4 border-t border-border/40">
          <Link
            to="/auth/login"
            onClick={onNavigate}
            className={cn(buttonVariants({ variant: "outline" }), "w-full justify-center h-10")}
          >
            Đăng nhập
          </Link>
          <Link
            to="/auth/register"
            onClick={onNavigate}
            className={cn(buttonVariants({ variant: "default" }), "w-full justify-center h-10")}
          >
            Đăng ký
          </Link>
        </div>
      )}
    </div>
  );
};

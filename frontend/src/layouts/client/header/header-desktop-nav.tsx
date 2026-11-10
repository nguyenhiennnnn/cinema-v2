import { NavLink } from "react-router";
import { cn } from "@/lib/utils";
import { ROUTES } from "@/config/routes";

export const NAV_LINKS = [
  { to: ROUTES.home, label: "Trang chủ" },
  { to: ROUTES.movies.list, label: "Phim" },
  { to: ROUTES.schedule, label: "Lịch chiếu" },
  { to: ROUTES.membership, label: "Thành viên" },
];

export const HeaderDesktopNav = () => (
  <nav className="hidden items-center gap-1 md:flex">
    {NAV_LINKS.map((item) => (
      <NavLink
        key={item.to}
        to={item.to}
        end={item.to === "/"}
        className={({ isActive }) =>
          cn(
            "relative px-3 py-2 text-sm font-medium text-muted-foreground transition-colors hover:text-foreground",
            "after:absolute after:inset-x-3 after:-bottom-px after:h-0.5 after:scale-x-0 after:bg-primary after:transition-transform",
            isActive && "text-foreground after:scale-x-100",
          )
        }
      >
        {item.label}
      </NavLink>
    ))}
  </nav>
);
import { Menu } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useAuthUser, useIsAuthenticated } from "@/stores/auth-store";
import { useSheetClose, useSheetOpen } from "@/stores/sheet-store";
import { ModeToggle } from "@/components/common/mode-toggle";
import { MobileNavContent } from "./mobile-nav-content";
import { HeaderLogo } from "./header-logo";
import { HeaderDesktopNav } from "./header-desktop-nav";
import { HeaderUserMenu } from "./header-user-menu";
import { HeaderAuthButtons } from "./header-auth-buttons";

export const ClientHeader = () => {
  const user = useAuthUser();
  const isAuthenticated = useIsAuthenticated();
  const openSheet = useSheetOpen();
  const closeSheet = useSheetClose();

  const handleOpenMobileMenu = () => {
    openSheet({
      title: "Menu",
      side: "right",
      size: "sm",
      content: <MobileNavContent onNavigate={closeSheet} />,
    });
  };

  return (
    <header className="sticky top-0 z-50 w-full bg-background border-b border-border/60">
      <div className="mx-auto flex h-16 max-w-7xl items-center justify-between gap-4 px-4">
        <HeaderLogo />
        <HeaderDesktopNav />

        <div className="flex items-center gap-2">
          {isAuthenticated && user ? <HeaderUserMenu user={user} /> : <HeaderAuthButtons />}

          <ModeToggle />

          <Button variant="ghost" size="icon" className="md:hidden" aria-label="Mở menu" onClick={handleOpenMobileMenu}>
            <Menu className="size-5" />
          </Button>
        </div>
      </div>
      
    </header>
  );
};
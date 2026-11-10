import { Outlet, Navigate, useSearchParams } from "react-router";
import { useAuthStore } from "@/stores/auth-store";
import { Role } from "@/types/enum";
import { AuthLoadingGuard } from "./auth-loading-guard";
import { getSafeRedirect } from "@/lib/utils";
import { ROUTES } from "@/config/routes";

interface GuestRouteProps {
  redirectTo?: string;
}

export const GuestRoute = ({ redirectTo = ROUTES.home }: GuestRouteProps) => {
  const { isInitialized, isAuthenticated, user } = useAuthStore();
  const [searchParams] = useSearchParams();

  if (!isInitialized) return <AuthLoadingGuard />;

  if (isAuthenticated) {
    if (redirectTo.startsWith(ROUTES.admin.root) && user?.role !== Role.Admin) {
      return <Outlet />;
    }
    const target = getSafeRedirect(searchParams.get("redirect"), redirectTo);
    return <Navigate to={target} replace />;
  }

  return <Outlet />;
};

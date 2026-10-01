import { Outlet, Navigate, useSearchParams } from "react-router";
import { useAuthStore } from "@/stores/auth-store";
import { Role } from "@/types/enum";
import { AuthLoadingGuard } from "./auth-loading-guard";

interface GuestRouteProps {
  redirectTo?: string;
}

export const GuestRoute = ({ redirectTo = "/" }: GuestRouteProps) => {
  const { isInitialized, isAuthenticated, user } = useAuthStore();
  const [searchParams] = useSearchParams();

  if (!isInitialized) return <AuthLoadingGuard />;

  if (isAuthenticated) {
    if (redirectTo.startsWith("/admin") && user?.role !== Role.Admin) {
      return <Outlet />;
    }
    const target = searchParams.get("redirect") || redirectTo;
    return <Navigate to={target} replace />;
  }

  return <Outlet />;
};

import { Outlet, Navigate, useLocation } from "react-router";
import { useAuthStore } from "@/stores/auth-store";
import { AuthLoadingGuard } from "./auth-loading-guard";

export const CustomerProtectedRoute = () => {
  const { isInitialized, isAuthenticated } = useAuthStore();
  const location = useLocation();

  if (!isInitialized) return <AuthLoadingGuard />;

  if (!isAuthenticated) {
    const redirectTo = encodeURIComponent(location.pathname + location.search);
    return <Navigate to={`/auth/login?redirect=${redirectTo}`} replace />;
  }

  return <Outlet />;
};

import { Outlet, Navigate } from "react-router";
import { useAuthStore } from "@/stores/auth-store";
import { Role } from "@/types/enum";
import { ROUTES, getAdminHomePath } from "@/config/routes";
import { AuthLoadingGuard } from "./auth-loading-guard";

export const AdminProtectedRoute = () => {
  const { isInitialized, isAuthenticated, user } = useAuthStore();

  if (!isInitialized) return <AuthLoadingGuard />;

  if (!isAuthenticated) {
    return <Navigate to={ROUTES.admin.login} replace />;
  }

  if (!user) return <AuthLoadingGuard />;

  if (user.role !== Role.Admin && user.role !== Role.Employee) {
    return <Navigate to={ROUTES.home} replace />;
  }

  return <Outlet />;
};

export const AdminIndexRedirect = () => {
  const role = useAuthStore((s) => s.user?.role);

  return (
    <Navigate to={getAdminHomePath(role)} replace />
  );
};


export const AdminOnlyRoute = () => {
  const { isInitialized, isAuthenticated, user } = useAuthStore();

  if (!isInitialized) return <AuthLoadingGuard />;

  if (!isAuthenticated) {
    return <Navigate to={ROUTES.admin.login} replace />;
  }

  if (user?.role === Role.Employee) {
    return <Navigate to={ROUTES.admin.bookingCounter} replace />;
  }

  if (user?.role !== Role.Admin) {
    return <Navigate to={ROUTES.home} replace />;
  }

  return <Outlet />;
};
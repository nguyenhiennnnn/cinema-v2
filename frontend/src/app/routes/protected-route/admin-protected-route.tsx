import { Outlet, Navigate, useLocation } from "react-router";
import { useAuthStore } from "@/stores/auth-store";
import { Role } from "@/types/enum";
import { canAccessAdminPath } from "@/config/admin-nav";
import { ROUTES, getAdminHomePath } from "@/config/routes";
import { AuthLoadingGuard } from "./auth-loading-guard";

export const AdminProtectedRoute = () => {
  const { isInitialized, isAuthenticated, user } = useAuthStore();
  const { pathname } = useLocation();

  if (!isInitialized) return <AuthLoadingGuard />;

  if (!isAuthenticated) {
    return <Navigate to={ROUTES.admin.login} replace />;
  }

  if (!user) return <AuthLoadingGuard />;

  if (user.role !== Role.Admin && user.role !== Role.Employee) {
    return <Navigate to={ROUTES.home} replace />;
  }

  // Bắt buộc đổi mật khẩu: mọi đường dẫn admin khác đều quay về trang đổi mật khẩu.
  if (user.mustChangePassword && pathname !== ROUTES.admin.changePassword) {
    return <Navigate to={ROUTES.admin.changePassword} replace />;
  }

  if (!canAccessAdminPath(pathname, user.role)) {
    return <Navigate to={getAdminHomePath(user.role)} replace />;
  }

  return <Outlet />;
};

export const AdminIndexRedirect = () => {
  const role = useAuthStore((s) => s.user?.role);

  return (
    <Navigate to={getAdminHomePath(role)} replace />
  );
};

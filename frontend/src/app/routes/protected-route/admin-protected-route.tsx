import { Outlet, Navigate } from "react-router";
import { useAuthStore } from "@/stores/auth-store";
import { Role } from "@/types/enum";
import { AuthLoadingGuard } from "./auth-loading-guard";

export const AdminProtectedRoute = () => {
  const { isInitialized, isAuthenticated, user } = useAuthStore();

  if (!isInitialized) return <AuthLoadingGuard />;

  if (!isAuthenticated) {
    return <Navigate to="/admin/login" replace />;
  }

  if (user?.role !== Role.Admin && user?.role !== Role.Employee) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
};


export const AdminOnlyRoute = () => {
  const { isInitialized, isAuthenticated, user } = useAuthStore();

  if (!isInitialized) return <AuthLoadingGuard />;

  if (!isAuthenticated) {
    return <Navigate to="/admin/login" replace />;
  }

  if (user?.role === Role.Employee) {
    return <Navigate to="/admin/bookings/counter" replace />;
  }

  if (user?.role !== Role.Admin) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
};
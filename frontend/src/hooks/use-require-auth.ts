import { useCallback } from "react";
import { useLocation, useNavigate } from "react-router";
import { useAuthStore } from "@/stores/auth-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

interface RequireAuthOptions {
  title?: string;
  description?: string;
  confirmText?: string;
  cancelText?: string;
}

const DEFAULT_OPTIONS: Required<RequireAuthOptions> = {
  title: "Yêu cầu đăng nhập",
  description: "Bạn cần đăng nhập để thực hiện thao tác này.",
  confirmText: "Đăng nhập",
  cancelText: "Để sau",
};

/**
 * Hook gate hanh dong can auth.
 * - Neu da dang nhap: thuc hien action ngay lap tuc.
 * - Neu chua dang nhap: hien AlertDialog xac nhan truoc khi redirect ve /auth/login.
 * - BUG FIX: Neu session chua khoi tao (!isInitialized), subscribe va chay lai sau khi init xong.
 */
export function useRequireAuth() {
  const navigate = useNavigate();
  const location = useLocation();
  const openAlertDialog = useAlertDialogOpen();

  return useCallback(
    (action: () => void | Promise<void>, options?: RequireAuthOptions) => {
      const opts = { ...DEFAULT_OPTIONS, ...options };

      const run = () => {
        const { isAuthenticated } = useAuthStore.getState();

        if (isAuthenticated) {
          action();
          return;
        }

        const redirectTo = `${location.pathname}${location.search}`;
        openAlertDialog({
          title: opts.title,
          description: opts.description,
          confirmText: opts.confirmText,
          cancelText: opts.cancelText,
          onConfirm: () => {
            navigate(
              `/auth/login?redirect=${encodeURIComponent(redirectTo)}`,
            );
          },
        });
      };

      const { isInitialized } = useAuthStore.getState();

      if (isInitialized) {
        run();
        return;
      }

      // BUG FIX: khong drop action khi session chua init xong.
      // Subscribe va chay ngay sau khi isInitialized = true.
      const unsub = useAuthStore.subscribe((state) => {
        if (!state.isInitialized) return;
        unsub();
        run();
      });
    },
    [navigate, location, openAlertDialog],
  );
}

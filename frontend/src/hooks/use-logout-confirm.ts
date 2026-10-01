import { useCallback } from "react";
import { useLogout } from "@/features/auth/hooks/use-auth";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";
import { useNavigate } from "react-router";

export function useLogoutConfirm() {
  const { mutate: logout } = useLogout();
  const navigate = useNavigate();
  const openAlertDialog = useAlertDialogOpen();

  return useCallback(() => {
    openAlertDialog({
      title: "Đăng xuất",
      description: "Bạn có chắc chắn muốn đăng xuất khỏi tài khoản này không?",
      confirmText: "Đăng xuất",
      cancelText: "Hủy",
      variant: "destructive",
      onConfirm: () => {
        logout();
        navigate("/", { replace: true });
      },
    });
  }, [logout, openAlertDialog, navigate]);
}

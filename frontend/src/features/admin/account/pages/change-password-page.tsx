import { Link, useNavigate } from "react-router";
import { KeyRound } from "lucide-react";

import { Button } from "@/components/ui/button";
import { toast } from "@/components/ui/toast";
import { ROUTES, getAdminHomePath } from "@/config/routes";
import { useLogoutConfirm } from "@/hooks/use-logout-confirm";
import { useAuthStore } from "@/stores/auth-store";
import { ChangePasswordForm } from "@/features/auth/components/change-password-form";

const AdminChangePasswordPage = () => {
  const navigate = useNavigate();
  const mustChangePassword = useAuthStore((s) => s.user?.mustChangePassword ?? false);
  const role = useAuthStore((s) => s.user?.role);
  const confirmLogout = useLogoutConfirm(ROUTES.admin.login);

  const handleSuccess = () => {
    toast.add({
      title: "Đã đổi mật khẩu",
      description: "Đăng nhập lại bằng mật khẩu mới để tiếp tục.",
      type: "success",
    });
    useAuthStore.getState().logout();
    navigate(ROUTES.admin.login, { replace: true });
  };

  return (
    <div className="flex flex-col gap-6">
      <div className="text-center">
        <h1 className="t-h2 text-foreground">Đổi mật khẩu</h1>
        <p className="mt-1 text-sm text-muted-foreground">
          {mustChangePassword
            ? "Tài khoản của bạn đang dùng mật khẩu tạm hoặc mật khẩu mặc định do quản trị viên cấp. Hãy đặt mật khẩu mới để tiếp tục. Sau khi đổi, bạn sẽ được đăng xuất và cần đăng nhập lại."
            : "Sau khi đổi, bạn sẽ bị đăng xuất khỏi mọi thiết bị và cần đăng nhập lại."}
        </p>
      </div>

      <ChangePasswordForm
        layout="stack"
        submitLabel="Đổi mật khẩu"
        submitIcon={<KeyRound className="size-4" />}
        skipSuccessToast
        onSuccess={handleSuccess}
      />

      {mustChangePassword ? (
        <Button type="button" variant="outline" className="w-full" onClick={confirmLogout}>
          Đăng xuất
        </Button>
      ) : (
        <Button
          variant="outline"
          className="w-full"
          render={<Link to={getAdminHomePath(role)} />}
          nativeButton={false}
        >
          Quay lại
        </Button>
      )}
    </div>
  );
};

export default AdminChangePasswordPage;

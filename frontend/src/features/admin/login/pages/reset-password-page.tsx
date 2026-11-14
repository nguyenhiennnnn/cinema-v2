import { useState } from "react";
import { useNavigate, useSearchParams } from "react-router";

import { toast } from "@/components/ui/toast";
import { ROUTES } from "@/config/routes";
import { InvalidLinkNotice } from "@/features/auth/components/invalid-link-notice";
import { SetPasswordForm } from "@/features/auth/components/set-password-form";
import { isInvalidResetTokenError } from "@/features/auth/utils/reset-token-error";

const CONTACT_ADMIN_HINT = "Hãy liên hệ quản trị viên để được mời lại.";

const AdminResetPasswordPage = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token") ?? "";
  const [tokenRejected, setTokenRejected] = useState(false);

  if (!token) {
    return (
      <InvalidLinkNotice
        title="Liên kết không hợp lệ"
        description={`Đường dẫn thiếu mã đặt mật khẩu. Hãy mở đúng liên kết trong email mời. ${CONTACT_ADMIN_HINT}`}
        actionLabel="Về trang đăng nhập"
        actionTo={ROUTES.admin.login}
      />
    );
  }

  if (tokenRejected) {
    return (
      <InvalidLinkNotice
        title="Liên kết đã hết hạn hoặc đã được dùng"
        description={`Không thể đặt mật khẩu bằng liên kết này. ${CONTACT_ADMIN_HINT}`}
        actionLabel="Về trang đăng nhập"
        actionTo={ROUTES.admin.login}
      />
    );
  }

  return (
    <div className="flex flex-col gap-6">
      <div className="text-center">
        <h1 className="t-h2 text-foreground">Đặt mật khẩu</h1>
        <p className="mt-1 text-sm text-muted-foreground">
          Đặt mật khẩu cho tài khoản quản trị của bạn để đăng nhập lần đầu
        </p>
      </div>
      <SetPasswordForm
        token={token}
        submitLabel="Đặt mật khẩu"
        newPasswordLabel="Mật khẩu"
        confirmPasswordLabel="Xác nhận mật khẩu"
        skipSuccessToast
        onSuccess={() => {
          toast.add({
            title: "Đã đặt mật khẩu",
            description: "Đăng nhập bằng mật khẩu vừa đặt để bắt đầu.",
            type: "success",
          });
          navigate(ROUTES.admin.login, { replace: true });
        }}
        onError={(error) => {
          if (isInvalidResetTokenError(error)) setTokenRejected(true);
        }}
      />
    </div>
  );
};

export default AdminResetPasswordPage;

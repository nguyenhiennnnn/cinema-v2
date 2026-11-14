import { useNavigate, useSearchParams } from "react-router";

import { ROUTES } from "@/config/routes";

import { InvalidLinkNotice } from "./invalid-link-notice";
import { SetPasswordForm } from "./set-password-form";

export const ResetPasswordForm = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token") ?? "";

  if (!token) {
    return (
      <InvalidLinkNotice
        title="Liên kết không hợp lệ"
        description="Thiếu mã đặt lại mật khẩu trong đường dẫn."
        actionLabel="Yêu cầu liên kết mới"
        actionTo={ROUTES.auth.forgotPassword}
      />
    );
  }

  return (
    <SetPasswordForm
      token={token}
      submitLabel="Đặt lại mật khẩu"
      onSuccess={() => navigate(ROUTES.auth.login, { replace: true })}
    />
  );
};

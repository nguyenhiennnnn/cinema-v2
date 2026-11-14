import { KeyRound } from "lucide-react";

import { ChangePasswordForm } from "@/features/auth/components/change-password-form";

const ChangePasswordPage = () => (
  <ChangePasswordForm
    layout="panel"
    heading="Đổi mật khẩu"
    submitLabel="Cập nhật mật khẩu"
    submitIcon={<KeyRound className="size-4" />}
  />
);

export default ChangePasswordPage;

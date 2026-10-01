import { ResetPasswordForm } from "../components/reset-password-form";

const ResetPasswordPage = () => {
  return (
    <div className="flex flex-col gap-6">
      <div className="text-center">
        <h1 className="font-heading text-2xl font-bold text-foreground">
          Đặt lại mật khẩu
        </h1>
        <p className="mt-1 text-sm text-muted-foreground">
          Nhập mật khẩu mới cho tài khoản của bạn
        </p>
      </div>
      <ResetPasswordForm />
    </div>
  );
};

export default ResetPasswordPage;

import { ForgotPasswordForm } from "../components/forgot-password-form";

const ForgotPasswordPage = () => {
  return (
    <div className="flex flex-col gap-6">
      <div className="text-center">
        <h1 className="font-heading text-2xl font-bold text-foreground">
          Quên mật khẩu
        </h1>
        <p className="mt-1 text-sm text-muted-foreground">
          Nhập email để nhận liên kết đặt lại mật khẩu
        </p>
      </div>
      <ForgotPasswordForm />
    </div>
  );
};

export default ForgotPasswordPage;

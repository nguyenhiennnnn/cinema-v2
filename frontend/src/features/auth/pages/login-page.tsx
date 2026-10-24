import { LoginForm } from "../components/login-form";

const LoginPage = () => {
  return (
    <div className="flex flex-col gap-6">
      <div className="text-center">
        <h1 className="t-h2 text-foreground">
          Đăng nhập
        </h1>
        <p className="mt-1 text-sm text-muted-foreground">
          Nhập email và mật khẩu để tiếp tục
        </p>
      </div>
      <LoginForm />
    </div>
  );
};

export default LoginPage;

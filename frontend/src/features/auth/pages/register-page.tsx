import { RegisterForm } from "../components/register-form";

const RegisterPage = () => {
  return (
    <div className="flex flex-col gap-6">
      <div className="text-center">
        <h1 className="t-h2 text-foreground">
          Tạo tài khoản
        </h1>
        <p className="mt-1 text-sm text-muted-foreground">
          Đăng ký để đặt vé và tích điểm thành viên
        </p>
      </div>
      <RegisterForm />
    </div>
  );
};

export default RegisterPage;

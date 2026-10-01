import { AdminLoginForm } from "../components/admin-login-form";

const AdminLoginPage = () => {
  return (
    <div className="flex flex-col gap-6">
      <div className="text-center">
        <h1 className="font-heading text-2xl font-bold text-foreground">
          Cinema Admin
        </h1>
        <p className="mt-1 text-sm text-muted-foreground">
          Đăng nhập bằng tài khoản nhân viên/quản trị được cấp
        </p>
      </div>
      <AdminLoginForm />
    </div>
  );
};

export default AdminLoginPage;

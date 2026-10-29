import { AdminLoginForm } from "../components/admin-login-form";
import { BRAND } from "@/config/brand";

const AdminLoginPage = () => {
  return (
    <div className="flex flex-col gap-6">
      <div className="text-center">
        <h1 className="t-h2 text-foreground">
          {BRAND.name} Admin
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

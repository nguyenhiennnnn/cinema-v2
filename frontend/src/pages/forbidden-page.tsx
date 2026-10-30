import { Home, LogIn, ShieldAlert } from "lucide-react";
import { StatusPage } from "@/components/common/status-page";
import { ROUTES } from "@/config/routes";

const ForbiddenPage = () => {
  return (
    <StatusPage
      code="403"
      icon={ShieldAlert}
      title="Không có quyền truy cập"
      description="Tài khoản hiện tại không đủ quyền để xem trang này. Vui lòng đăng nhập bằng tài khoản phù hợp."
      actions={[
        { label: "Về trang chủ", to: "/", icon: Home },
        { label: "Đăng nhập lại", to: ROUTES.admin.login, icon: LogIn, variant: "outline" },
      ]}
    />
  );
};

export default ForbiddenPage;
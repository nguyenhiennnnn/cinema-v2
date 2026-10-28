import { Home, TriangleAlert } from "lucide-react";
import { StatusPage } from "@/components/common/status-page";

const ErrorPage = () => (
  <StatusPage
    code="500"
    icon={TriangleAlert}
    title="Đã xảy ra lỗi"
    description="Trang gặp sự cố khi hiển thị. Vui lòng quay về trang chủ và thử lại."
    actions={[{ label: "Về trang chủ", to: "/", icon: Home }]}
  />
);

export default ErrorPage;

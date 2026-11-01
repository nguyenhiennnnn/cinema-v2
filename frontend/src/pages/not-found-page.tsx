import { Clapperboard, Home, Ticket } from "lucide-react";
import { StatusPage } from "@/components/common/status-page";
import { Seo } from "@/components/common/seo";

const NotFoundPage = () => {
  return (
    <>
      <Seo title="Không tìm thấy trang" noIndex />
      <StatusPage
      code="404"
      icon={Clapperboard}
      title="Không tìm thấy trang"
      description="Suất chiếu hoặc đường dẫn bạn tìm không tồn tại. Có thể trang đã bị gỡ hoặc địa chỉ không chính xác."
      actions={[
        { label: "Về trang chủ", to: "/", icon: Home },
        { label: "Xem lịch chiếu", to: "/schedule", icon: Ticket, variant: "outline" },
      ]}
    />
    </>
  );
};

export default NotFoundPage;
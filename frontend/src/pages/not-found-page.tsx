import { Clapperboard, Home, Ticket } from "lucide-react";
import { StatusPage } from "@/components/common/status-page";

const NotFoundPage = () => {
  return (
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
  );
};

export default NotFoundPage;
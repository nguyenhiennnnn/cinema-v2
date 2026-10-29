import { Link } from "react-router";
import { FaFacebook, FaInstagram, FaYoutube } from "react-icons/fa";
import { BrandMark, BrandWordmark } from "@/components/common/brand-mark";
import { BRAND } from "@/config/brand";

const FOOTER_CATEGORIES = [
  { label: "Phim đang chiếu", to: "/movies?status=now-showing" },
  { label: "Phim sắp chiếu", to: "/movies?status=coming-soon" },
  { label: "Phòng chiếu đặc biệt", to: "/movies?format=imax" },
  { label: "Combo bắp nước", to: "/membership" },
];

const FOOTER_POLICIES = [
  { label: "Điều khoản sử dụng", to: "/policies/terms" },
  { label: "Chính sách bảo mật", to: "/policies/privacy" },
  { label: "Quy định hoàn/đổi vé", to: "/policies/refund" },
  { label: "Quy định độ tuổi", to: "/policies/age-rating" },
];

export const ClientFooter = () => {
  return (
    <footer className="w-full bg-card text-card-foreground">
      <div className="perf-h" />
      <div className="mx-auto grid max-w-7xl grid-cols-1 gap-8 px-4 py-12 md:grid-cols-2 lg:grid-cols-4">
        <div className="flex flex-col gap-3">
          <div className="flex items-center gap-2">
            <BrandMark className="size-8 shrink-0" />
            <BrandWordmark className="h-5 w-auto" />
          </div>
          <p className="text-sm text-muted-foreground">
            Đặt vé xem phim nhanh, chọn ghế realtime, nhận vé điện tử ngay.
          </p>
          <div className="flex items-center gap-3 text-muted-foreground">
            <FaFacebook className="size-4 hover:text-primary cursor-pointer" />
            <FaYoutube className="size-4 hover:text-primary cursor-pointer" />
            <FaInstagram className="size-4 hover:text-primary cursor-pointer" />
          </div>
        </div>

        <div className="flex flex-col gap-2">
          <p className="text-[13px] font-medium text-foreground">Danh mục</p>
          {FOOTER_CATEGORIES.map((item) => (
            <Link
              key={item.label}
              to={item.to}
              className="text-sm text-muted-foreground hover:text-foreground"
            >
              {item.label}
            </Link>
          ))}
        </div>

        <div className="flex flex-col gap-2">
          <p className="text-[13px] font-medium text-foreground">Chính sách</p>
          {FOOTER_POLICIES.map((item) => (
            <Link
              key={item.label}
              to={item.to}
              className="text-sm text-muted-foreground hover:text-foreground"
            >
              {item.label}
            </Link>
          ))}
        </div>

        <div className="flex flex-col gap-2">
          <p className="text-[13px] font-medium text-foreground">Chăm sóc khách hàng</p>
          <p className="text-sm text-muted-foreground">Hotline: {BRAND.supportHotline}</p>
          <p className="text-sm text-muted-foreground">Email: {BRAND.supportEmail}</p>
          <p className="mt-2 text-sm font-medium text-foreground">
            VNPay, MoMo, Visa, Mastercard
          </p>
        </div>
      </div>
      <div className="perf-h" />
      <div className="px-4 py-4 text-center text-xs text-muted-foreground">
        © 2026 {BRAND.name}. Toàn bộ bản quyền thuộc về {BRAND.name} Project.
      </div>
    </footer>
  );
};

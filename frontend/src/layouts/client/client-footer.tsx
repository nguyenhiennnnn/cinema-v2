import { Link } from "react-router";
import { FaFacebook, FaInstagram, FaYoutube } from "react-icons/fa";
import { BrandMark } from "@/components/common/brand-mark";

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
      <div className="sprocket-rule" />
      <div className="mx-auto grid max-w-7xl grid-cols-1 gap-8 px-4 py-12 md:grid-cols-2 lg:grid-cols-4">
        <div className="flex flex-col gap-3">
          <div className="flex items-center gap-2">
            <BrandMark className="size-8" />
            <span className="font-heading text-xl leading-none font-semibold tracking-tight text-foreground uppercase">
              Cinema
            </span>
          </div>
          <p className="text-sm text-muted-foreground">
            Trải nghiệm điện ảnh với âm thanh và hình ảnh chuẩn quốc tế.
          </p>
          <div className="flex items-center gap-3 text-muted-foreground">
            <FaFacebook className="size-4 hover:text-primary" />
            <FaYoutube className="size-4 hover:text-primary" />
            <FaInstagram className="size-4 hover:text-primary" />
          </div>
        </div>

        <div className="flex flex-col gap-2">
          <h3 className="text-eyebrow">Danh mục</h3>
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
          <h3 className="text-eyebrow">Chính sách</h3>
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
          <h3 className="text-eyebrow">Chăm sóc khách hàng</h3>
          <p className="text-sm text-muted-foreground">
            Hotline: 1900 xxxx (8h - 22h)
          </p>
          <p className="text-sm text-muted-foreground">Email: cskh@cinema.vn</p>
          <p className="mt-2 font-mono text-sm font-medium tracking-wide text-foreground">
            VNPay · MoMo · Visa · Mastercard
          </p>
        </div>
      </div>
      <div className="perforation" />
      <div className="px-4 py-4 text-center text-xs text-muted-foreground">
        © 2026 Cinema System. Toàn bộ bản quyền thuộc về Cinema Project.
      </div>
    </footer>
  );
};

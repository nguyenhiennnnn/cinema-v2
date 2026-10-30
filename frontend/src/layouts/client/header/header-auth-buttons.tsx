import { Link } from "react-router";
import { Button } from "@/components/ui/button";
import { ROUTES } from "@/config/routes";

export const HeaderAuthButtons = () => (
  <div className="hidden items-center gap-2 sm:flex">
    <Button variant="ghost">
      <Link to={ROUTES.auth.login}>Đăng nhập</Link>
    </Button>
    <Button>
      <Link to={ROUTES.auth.register}>Đăng ký</Link>
    </Button>
  </div>
);
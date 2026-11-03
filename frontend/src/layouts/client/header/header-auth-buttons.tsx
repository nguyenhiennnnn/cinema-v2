import { Link } from "react-router";
import { Button } from "@/components/ui/button";
import { ROUTES } from "@/config/routes";

export const HeaderAuthButtons = () => (
  <div className="hidden items-center gap-2 sm:flex">
    <Button
      variant="ghost"
      render={<Link to={ROUTES.auth.login} />}
      nativeButton={false}
    >
      Đăng nhập
    </Button>
    <Button
      render={<Link to={ROUTES.auth.register} />}
      nativeButton={false}
    >
      Đăng ký
    </Button>
  </div>
);
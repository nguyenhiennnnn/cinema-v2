import { Link } from "react-router";
import { Button } from "@/components/ui/button";

export const HeaderAuthButtons = () => (
  <div className="hidden items-center gap-2 sm:flex">
    <Button variant="ghost">
      <Link to="/auth/login">Đăng nhập</Link>
    </Button>
    <Button>
      <Link to="/auth/register">Đăng ký</Link>
    </Button>
  </div>
);
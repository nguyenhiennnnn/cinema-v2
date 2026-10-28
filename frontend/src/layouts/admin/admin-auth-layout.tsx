import { Link, Outlet } from "react-router";
import { BrandMark, BrandWordmark } from "@/components/common/brand-mark";

const AdminAuthLayout = () => {
  return (
    <div className="relative flex min-h-screen items-center justify-center overflow-hidden p-6">
      <img
        src="/auth-admin-bg.png"
        alt=""
        aria-hidden="true"
        className="absolute inset-0 h-full w-full object-cover object-center"
      />
      <div className="absolute inset-0 bg-scrim/60" />

      <div className="relative z-10 flex w-full max-w-sm flex-col items-center gap-5">
        <Link to="/" className="flex items-center gap-2">
          <BrandMark className="size-8 shrink-0" />
          <BrandWordmark className="h-5 w-auto" />
        </Link>

        <div className="w-full cut-lg bg-card p-6 sm:p-8">
          <Outlet />
        </div>
      </div>
    </div>
  );
};

export default AdminAuthLayout;

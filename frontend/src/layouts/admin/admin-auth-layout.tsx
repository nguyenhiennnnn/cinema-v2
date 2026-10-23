import { Link, Outlet } from "react-router";
import { BrandMark, BrandWordmark } from "@/components/common/brand-mark";

const AdminAuthLayout = () => {
  return (
    <div className="relative flex min-h-screen items-center justify-center bg-background p-6">
      <div
        className="absolute inset-0 -z-20 bg-cover bg-center"
        style={{ backgroundImage: "url(/auth-admin-bg.png)" }}
      />
      <div className="absolute inset-0 -z-10 bg-foreground/60" />

      <div className="flex w-full max-w-sm flex-col items-center gap-5">
        <Link to="/" className="flex items-center gap-2">
          <BrandMark className="size-8 shrink-0" />
          <BrandWordmark className="h-5 w-auto" />
        </Link>

        <div className="w-full cut-lg border-t-4 border-sidebar-primary bg-card p-6 sm:p-8">
          <Outlet />
        </div>
      </div>
    </div>
  );
};

export default AdminAuthLayout;

import { Link, Outlet } from "react-router";
import { BrandMark } from "@/components/common/brand-mark";

const AuthLayout = () => {
  return (
    <div className="relative flex min-h-screen items-center justify-center p-6 md:justify-end md:pr-24 lg:pr-32">
      <div
        className="absolute inset-0 -z-20 bg-cover bg-center"
        style={{ backgroundImage: "url(/auth-client-bg.png)" }}
      />
      <div className="absolute inset-0 -z-10 bg-linear-to-r from-black/15 via-black/45 to-black/70" />

      <div className="flex w-full max-w-sm flex-col items-center gap-5">
        <Link to="/" className="flex items-center gap-2">
          <BrandMark />
          <span className="font-heading text-lg font-bold text-primary-foreground">
            Cinema
          </span>
        </Link>

        <div className="w-full rounded-2xl border-t-4 border-primary bg-card p-6 shadow-lg sm:p-8">
          <Outlet />
        </div>
      </div>
    </div>
  );
};

export default AuthLayout;

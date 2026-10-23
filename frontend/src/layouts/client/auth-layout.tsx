import { Link, Outlet } from "react-router";
import { BrandMark, BrandWordmark } from "@/components/common/brand-mark";

const AuthLayout = () => {
  return (
    <div className="relative flex min-h-screen items-center justify-center overflow-hidden p-6 md:justify-end md:pr-24 lg:pr-32">
      <img
        src="/auth-client-bg.png"
        alt=""
        aria-hidden="true"
        className="absolute inset-0 h-full w-full object-cover object-center"
      />
      <div className="absolute inset-0 bg-gradient-to-r from-black/25 via-black/55 to-black/80" />

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

export default AuthLayout;

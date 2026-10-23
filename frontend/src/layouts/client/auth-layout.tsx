import { Link, Outlet } from "react-router";
import { BrandMark, BrandWordmark } from "@/components/common/brand-mark";

const AuthLayout = () => {
  return (
    <div className="relative flex min-h-screen items-center justify-center bg-background p-6 md:justify-end md:pr-24 lg:pr-32">
      <div
        className="absolute inset-0 -z-10 bg-cover bg-center"
        style={{ backgroundImage: "url(/auth-client-bg.png)" }}
      />
      <div className="absolute inset-0 -z-10 bg-foreground/50" />

      <div className="flex w-full max-w-sm flex-col items-center gap-5">
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

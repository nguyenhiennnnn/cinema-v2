import { Link, Outlet } from "react-router";
import { Clapperboard } from "lucide-react";

const AdminAuthLayout = () => {
  return (
    <div className="relative flex min-h-screen items-center justify-center p-6">
      <div
        className="absolute inset-0 -z-20 bg-cover bg-center"
        style={{ backgroundImage: "url(/auth-admin-bg.png)" }}
      />
      <div
        className="absolute inset-0 -z-10"
        style={{
          background:
            "radial-gradient(ellipse 60% 55% at 50% 50%, rgba(0,0,0,0.75) 0%, rgba(0,0,0,0.4) 55%, rgba(0,0,0,0.1) 100%)",
        }}
      />

      <div className="flex w-full max-w-sm flex-col items-center gap-5">
        <Link to="/" className="flex items-center gap-2">
          <Clapperboard className="size-6 text-white" />
          <div>
            <span className="font-heading text-lg font-bold text-white">
              Cinema Admin
            </span>
            <p className="text-[10px] tracking-widest text-white/60 uppercase">
              Admin Portal
            </p>
          </div>
        </Link>

        <div className="w-full rounded-2xl border-t-4 border-sidebar-primary bg-card p-6 shadow-lg sm:p-8">
          <Outlet />
        </div>
      </div>
    </div>
  );
};

export default AdminAuthLayout;

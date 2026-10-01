import { Outlet } from "react-router";
import { AccountSidebar } from "./account-sidebar";

const AccountLayout = () => (
  <div className="mx-auto flex max-w-7xl flex-col gap-6 px-4 py-8 md:flex-row">
    <AccountSidebar />
    <div className="min-w-0 flex-1">
      <Outlet />
    </div>
  </div>
);

export default AccountLayout;
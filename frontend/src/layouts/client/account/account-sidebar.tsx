import { useAuthUser } from "@/stores/auth-store";
import { useLogoutConfirm } from "@/hooks/use-logout-confirm";
import { AccountUserCard } from "./account-user-card";
import { AccountNav } from "./account-nav";

export const AccountSidebar = () => {
  const user = useAuthUser();
  const confirmLogout = useLogoutConfirm();

  return (
    <aside className="w-full shrink-0 md:w-64 lg:w-72">
      <AccountUserCard user={user} />
      <AccountNav onLogout={confirmLogout} />
    </aside>
  );
};
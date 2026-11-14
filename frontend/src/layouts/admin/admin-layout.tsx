import { useState } from "react";
import { Outlet } from "react-router";
import { cn } from "@/lib/utils";
import { useImportExportJobHub } from "@/hooks/use-import-export-job-hub";
import { JobProgressPanel } from "@/features/admin/import-export/components/job-progress-panel";
import { AdminSidebar } from "./admin-sidebar";
import { AdminTopbar } from "./admin-topbar";

const AdminLayout = () => {
  const [collapsed, setCollapsed] = useState(false);
  useImportExportJobHub();

  return (
    <div className="min-h-screen bg-background">
      <AdminSidebar collapsed={collapsed} />
      <div
        className={cn(
          "flex min-h-screen flex-col transition-all duration-300",
          collapsed ? "pl-18" : "pl-64",
        )}
      >
        <AdminTopbar onToggle={() => setCollapsed((c) => !c)} />
        <main className="flex-1 p-4 md:p-6">
          <Outlet />
        </main>
      </div>
      <JobProgressPanel />
    </div>
  );
};

export default AdminLayout;

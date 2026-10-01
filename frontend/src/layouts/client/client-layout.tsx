import { Outlet } from "react-router";
import { ClientFooter } from "./client-footer";
import { ClientHeader } from "./header/client-header";
import { AiAssistantWidget } from "@/features/client/ai/components/ai-assistant-widget";

const ClientLayout = () => {
  return (
    <div className="flex min-h-screen flex-col">
      <ClientHeader />
      <main className="flex-1">
        <Outlet />
      </main>
      <ClientFooter />
      <AiAssistantWidget />
    </div>
  );
};

export default ClientLayout;
import { queryClient } from "@/api/query-client";
import { QueryClientProvider } from "@tanstack/react-query";
import { NuqsAdapter } from "nuqs/adapters/react";
import { TooltipProvider } from "@/components/ui/tooltip";
import { SessionInitializer } from "./session-initializer";
import { ThemeProvider } from "./theme-provider";

interface AppProvidersProps {
  children: React.ReactNode;
}

export const AppProviders = ({ children }: AppProvidersProps) => {
  return (
    <ThemeProvider defaultTheme="system" storageKey="vite-ui-theme">
      <QueryClientProvider client={queryClient}>
        <NuqsAdapter>
          <TooltipProvider>
            <SessionInitializer />
            {children}
          </TooltipProvider>
        </NuqsAdapter>
        {/* {import.meta.env.DEV && <ReactQueryDevtools initialIsOpen={false} />} */}
      </QueryClientProvider>
    </ThemeProvider>
  );
};

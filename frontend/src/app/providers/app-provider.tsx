import { queryClient } from "@/api/query-client";
import { QueryClientProvider } from "@tanstack/react-query";
import { NuqsAdapter } from "nuqs/adapters/react";
import { TooltipProvider } from "@/components/ui/tooltip";
import { SessionInitializer } from "./session-initializer";
import { ThemeProvider } from "./theme-provider";
import { HelmetProvider } from "react-helmet-async";
import { BRAND } from "@/config/brand";

interface AppProvidersProps {
  children: React.ReactNode;
}

export const AppProviders = ({ children }: AppProvidersProps) => {
  return (
    <HelmetProvider>
      <ThemeProvider defaultTheme="system" storageKey={BRAND.storageKeys.theme}>
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
    </HelmetProvider>
  );
};

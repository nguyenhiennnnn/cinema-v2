import path from "path";
import tailwindcss from "@tailwindcss/vite";
import react, { reactCompilerPreset } from "@vitejs/plugin-react";
import babel from "@rolldown/plugin-babel";
import { defineConfig, loadEnv, type Plugin } from "vite";

function htmlSiteUrl(siteUrl: string): Plugin {
  return {
    name: "html-site-url",
    transformIndexHtml: (html) => html.replaceAll("%SITE_URL%", siteUrl),
  };
}

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, import.meta.dirname, "VITE_");
  const siteUrl = (env.VITE_SITE_URL ?? "").replace(/\/+$/, "");

  return {
    plugins: [
      react(),
      babel({ presets: [reactCompilerPreset()] }),
      tailwindcss(),
      htmlSiteUrl(siteUrl),
    ],
    resolve: {
      alias: {
        "@": path.resolve(import.meta.dirname, "./src"),
      },
    },
    build: {
      rolldownOptions: {
        output: {
          manualChunks(id) {
            if (id.includes("node_modules/recharts") || id.includes("node_modules/d3") || id.includes("node_modules/victory")) {
              return "vendor-charts";
            }
            if (id.includes("node_modules/@microsoft/signalr")) {
              return "vendor-realtime";
            }
            if (id.includes("node_modules/react-hook-form") || id.includes("node_modules/@hookform") || id.includes("node_modules/zod")) {
              return "vendor-forms";
            }
            if (id.includes("node_modules/@tanstack")) {
              return "vendor-query";
            }
            if (id.includes("node_modules/date-fns") || id.includes("node_modules/axios") || id.includes("node_modules/nuqs") || id.includes("node_modules/zustand")) {
              return "vendor-utils";
            }
            if (id.includes("node_modules/lucide-react") || id.includes("node_modules/@base-ui") || id.includes("node_modules/cmdk") || id.includes("node_modules/class-variance-authority") || id.includes("node_modules/clsx") || id.includes("node_modules/tailwind-merge")) {
              return "vendor-ui";
            }
            if (id.includes("node_modules/react-dom") || id.includes("node_modules/react-router") || id.includes("node_modules/react/")) {
              return "vendor-react";
            }
          },
        },
      },
    },
    server: {
      host: "0.0.0.0",
      port: 5173,
      strictPort: true,
      allowedHosts: true,
      watch: {
        usePolling: true,
        interval: 100,
        ignored: ["**/test-results/**", "**/playwright-report/**"],
      },
      hmr: {
        clientPort: 5173,
      },
    },
  };
});

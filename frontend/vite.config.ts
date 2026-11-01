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

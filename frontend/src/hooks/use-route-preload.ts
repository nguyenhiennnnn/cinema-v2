import { useEffect } from "react";
import { matchRoutes, type createBrowserRouter, type RouteObject } from "react-router";

type DataRouter = ReturnType<typeof createBrowserRouter>;

interface LazyEntry {
  path: string;
  lazy: NonNullable<RouteObject["lazy"]>;
}

const ADMIN_PREFIX = "/admin";
const IDLE_TIMEOUT_MS = 3000;
const IDLE_GAP_MS = 150;

const joinPath = (base: string, segment: string) =>
  segment.startsWith("/") ? segment : `${base.replace(/\/$/, "")}/${segment}`;

const collectLazyEntries = (routes: RouteObject[], base: string, out: LazyEntry[]) => {
  for (const route of routes) {
    const path = route.path ? joinPath(base, route.path) : base;
    if (typeof route.lazy === "function") out.push({ path, lazy: route.lazy });
    if (route.children) collectLazyEntries(route.children, path, out);
  }
  return out;
};

const runLazy = (lazy: LazyEntry["lazy"]) => {
  if (typeof lazy !== "function") return;
  void Promise.resolve(lazy()).catch(() => undefined);
};

const isSaveData = () => {
  const connection = (navigator as Navigator & { connection?: { saveData?: boolean } }).connection;
  return connection?.saveData === true;
};

const requestIdle = (callback: () => void) => {
  if (typeof window.requestIdleCallback === "function") {
    const id = window.requestIdleCallback(callback, { timeout: IDLE_TIMEOUT_MS });
    return () => window.cancelIdleCallback(id);
  }
  const id = window.setTimeout(callback, IDLE_GAP_MS);
  return () => window.clearTimeout(id);
};

const resolveInternalPath = (target: EventTarget | null) => {
  if (!(target instanceof Element)) return null;
  const anchor = target.closest("a[href]");
  if (!(anchor instanceof HTMLAnchorElement)) return null;
  if (anchor.origin !== window.location.origin || anchor.target === "_blank") return null;
  return anchor.pathname;
};

export const useRoutePreload = (router: DataRouter) => {
  useEffect(() => {
    const preloaded = new Set<string>();

    const preloadPath = (pathname: string) => {
      if (preloaded.has(pathname)) return;
      preloaded.add(pathname);
      matchRoutes(router.routes, pathname)?.forEach((match) => {
        if (typeof match.route.lazy === "function") runLazy(match.route.lazy);
      });
    };

    const onIntent = (event: Event) => {
      const pathname = resolveInternalPath(event.target);
      if (pathname) preloadPath(pathname);
    };

    document.addEventListener("pointerover", onIntent, { passive: true });
    document.addEventListener("focusin", onIntent);
    document.addEventListener("touchstart", onIntent, { passive: true });

    let cancelIdle: (() => void) | null = null;

    if (!isSaveData()) {
      const isAdminArea = window.location.pathname.startsWith(ADMIN_PREFIX);
      const queue = collectLazyEntries(router.routes, "", []).filter(
        (entry) => entry.path.startsWith(ADMIN_PREFIX) === isAdminArea,
      );

      const next = () => {
        const entry = queue.shift();
        if (!entry) return;
        runLazy(entry.lazy);
        cancelIdle = requestIdle(next);
      };

      cancelIdle = requestIdle(next);
    }

    return () => {
      document.removeEventListener("pointerover", onIntent);
      document.removeEventListener("focusin", onIntent);
      document.removeEventListener("touchstart", onIntent);
      cancelIdle?.();
    };
  }, [router]);
};

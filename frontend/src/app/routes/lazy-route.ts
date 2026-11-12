import type { ComponentType } from "react";
import type { LazyRouteFunction, RouteObject } from "react-router";

type PageModule = { default: ComponentType };

export const lazyRoute = (load: () => Promise<PageModule>): LazyRouteFunction<RouteObject> => {
  let pending: Promise<{ Component: ComponentType }> | null = null;

  return () => {
    pending ??= load()
      .then((module) => ({ Component: module.default }))
      .catch((error: unknown) => {
        pending = null;
        throw error;
      });
    return pending;
  };
};

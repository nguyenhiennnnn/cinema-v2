import { BRAND } from "@/config/brand";

const KEY = BRAND.storageKeys.sessionHint;

function safely<T>(action: () => T, fallback: T): T {
  try {
    return action();
  } catch {
    return fallback;
  }
}

export const sessionHint = {
  has: (): boolean => safely(() => localStorage.getItem(KEY) === "1", false),
  set: (): void => safely(() => localStorage.setItem(KEY, "1"), undefined),
  clear: (): void => safely(() => localStorage.removeItem(KEY), undefined),
  purgeLegacy: (): void =>
    safely(() => {
      BRAND.legacyStorageKeys.purge.forEach((key) =>
        localStorage.removeItem(key),
      );
    }, undefined),
  subscribe: (listener: (present: boolean) => void): (() => void) => {
    const handler = (event: StorageEvent) => {
      if (event.key === null || event.key === KEY) listener(sessionHint.has());
    };
    window.addEventListener("storage", handler);
    return () => window.removeEventListener("storage", handler);
  },
};

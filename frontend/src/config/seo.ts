export const NOINDEX_PATH_PREFIXES = [
  "/admin",
  "/auth",
  "/account",
  "/booking",
  "/403",
] as const;

export function isNoIndexPath(pathname: string): boolean {
  return NOINDEX_PATH_PREFIXES.some(
    (prefix) => pathname === prefix || pathname.startsWith(`${prefix}/`),
  );
}

import { ROUTES } from "@/config/routes";

export const NOINDEX_PATH_PREFIXES = [
  ROUTES.admin.root,
  ROUTES.auth.root,
  ROUTES.account.root,
  ROUTES.booking.root,
  ROUTES.forbidden,
] as const;

export function isNoIndexPath(pathname: string): boolean {
  return NOINDEX_PATH_PREFIXES.some(
    (prefix) => pathname === prefix || pathname.startsWith(`${prefix}/`),
  );
}

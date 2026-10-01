export function stripNulls<T extends Record<string, unknown>>(
  obj: T,
): { [K in keyof T]: Exclude<T[K], null> | undefined } {
  const result: Record<string, unknown> = {};
  for (const key of Object.keys(obj)) {
    const value = obj[key];
    result[key] = value === null ? undefined : value;
  }
  return result as { [K in keyof T]: Exclude<T[K], null> | undefined };
}

import { extractApiError, getFieldErrors } from "@/lib/error";

export function isInvalidResetTokenError(error: unknown): boolean {
  if (extractApiError(error).statusCode !== 400) return false;
  return Object.keys(getFieldErrors(error)).every((field) => field === "token");
}

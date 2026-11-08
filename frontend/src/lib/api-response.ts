import type { ApiResponse } from "@/types/api";

function readMessage(value: unknown): string | null {
  if (typeof value !== "object" || value === null) return null;
  const { message } = value as { message?: unknown };
  return typeof message === "string" && message.trim() !== "" ? message : null;
}

export function getApiSuccessMessage(response: unknown): string | null {
  return (
    readMessage(response) ??
    readMessage((response as Partial<ApiResponse<unknown>> | null)?.data)
  );
}

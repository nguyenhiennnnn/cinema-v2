import type { ApiResponse } from "@/types/api";

/**
 * Đọc `message` từ envelope `{ success, data, message }` của server.
 * Trả về null nếu response không phải envelope hoặc message rỗng.
 */
export function getApiSuccessMessage(response: unknown): string | null {
  if (typeof response !== "object" || response === null) return null;
  const { message } = response as Partial<ApiResponse<unknown>>;
  return typeof message === "string" && message.trim() !== "" ? message : null;
}

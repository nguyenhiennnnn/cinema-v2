import type { ApiResponse } from "@/types/api";

function readMessage(value: unknown): string | null {
  if (typeof value !== "object" || value === null) return null;
  const { message } = value as { message?: unknown };
  return typeof message === "string" && message.trim() !== "" ? message : null;
}

/**
 * Đọc message thành công từ response của server.
 *
 * Ưu tiên `message` ở envelope `{ success, data, message }`; nếu rỗng thì lấy
 * `data.message` (các endpoint trả DTO dạng `{ message }` như đăng ký, quên mật khẩu,
 * xác nhận email...). Trả về null nếu không có message nào.
 */
export function getApiSuccessMessage(response: unknown): string | null {
  return (
    readMessage(response) ??
    readMessage((response as Partial<ApiResponse<unknown>> | null)?.data)
  );
}

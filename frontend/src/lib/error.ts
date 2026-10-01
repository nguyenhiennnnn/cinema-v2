import axios from "axios";
import { toast } from "@/components/ui/toast";

export interface ApiErrorShape {
  message: string;
  errors?: Record<string, string[]>;
  statusCode?: number;
}

export function extractApiError(error: unknown): ApiErrorShape {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as Partial<ApiErrorShape> | undefined;
    return {
      message: data?.message ?? error.message ?? "Đã có lỗi xảy ra",
      errors: data?.errors ?? undefined,
      statusCode: error.response?.status,
    };
  }
  if (error instanceof Error) {
    return { message: error.message };
  }
  return { message: "Đã có lỗi xảy ra không xác định" };
}

export function logDevError(error: unknown, context?: string): void {
  if (!import.meta.env.DEV) return;

  const tag = context ? `[${context}]` : "[API Error]";

  if (axios.isAxiosError(error)) {
    console.group(
      `${tag} Axios error — ${error.response?.status ?? "network"}`,
    );
    console.error("URL:", error.config?.url);
    console.error("Method:", error.config?.method?.toUpperCase());
    console.error(
      "Status:",
      error.response?.status,
      error.response?.statusText,
    );
    console.error("Response data:", error.response?.data);
    console.error(
      "Request data:",
      (() => {
        try {
          return JSON.parse(error.config?.data ?? "null");
        } catch {
          return error.config?.data;
        }
      })(),
    );
    console.error("Full error:", error);
    console.groupEnd();
  } else {
    console.error(`${tag}`, error);
  }
}

export function getDisplayMessage(error: unknown): string {
  const { message, statusCode } = extractApiError(error);

  if (statusCode === 401)
    return "Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.";
  if (statusCode === 403) return "Bạn không có quyền thực hiện thao tác này.";
  if (statusCode === 404) return "Không tìm thấy dữ liệu yêu cầu.";
  if (statusCode === 409)
    return message || "Dữ liệu bị xung đột. Vui lòng thử lại.";
  if (statusCode === 422 || statusCode === 400)
    return message || "Dữ liệu không hợp lệ.";
  if (statusCode && statusCode >= 500)
    return "Máy chủ đang gặp sự cố. Vui lòng thử lại sau.";
  if (!navigator.onLine) return "Không có kết nối mạng. Vui lòng kiểm tra lại.";

  return message || "Đã có lỗi xảy ra. Vui lòng thử lại.";
}

export function showApiErrorToast(error: unknown, opts?: { title?: string }): void {
  const message = getDisplayMessage(error);
  const { statusCode } = extractApiError(error);

  if (statusCode === 401) return;

  toast.add({
    title: opts?.title ?? "Đã có lỗi xảy ra",
    description: message,
    type: "error",
  });
}

export function getFieldErrors(error: unknown): Record<string, string> {
  const { errors } = extractApiError(error);
  if (!errors) return {};
  return Object.fromEntries(
    Object.entries(errors).map(([key, msgs]) => [
      key.charAt(0).toLowerCase() + key.slice(1),
      msgs[0] ?? "",
    ]),
  );
}
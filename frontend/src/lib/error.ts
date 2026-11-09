import axios from "axios";
import { toast } from "@/components/ui/toast";
import { ENV } from "@/config/env";
import {
  ERROR_MESSAGES,
  ERROR_TOAST_DEDUPE_MS,
  SERVER_ERROR_RULE,
  SILENT_TOAST_STATUSES,
  STATUS_ERROR_RULES,
} from "@/config/error-messages";

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
  return { message: ERROR_MESSAGES.unknown };
}

export function logDevError(error: unknown, context?: string): void {
  if (!ENV.IS_DEV) return;

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

export function isCancelledError(error: unknown): boolean {
  return axios.isCancel(error);
}

export function isNetworkError(error: unknown): boolean {
  return axios.isAxiosError(error) && !error.response && !axios.isCancel(error);
}

function getServerMessage(error: unknown): string | undefined {
  if (!axios.isAxiosError(error)) return undefined;
  const message = (error.response?.data as Partial<ApiErrorShape> | undefined)
    ?.message;
  return typeof message === "string" && message !== "" ? message : undefined;
}

export function getDisplayMessage(
  error: unknown,
  options?: { preferServerMessage?: boolean },
): string {
  const { message, statusCode } = extractApiError(error);

  if (isNetworkError(error)) {
    const code = (error as { code?: string }).code;
    if (code === "ECONNABORTED" || code === "ETIMEDOUT") {
      return ERROR_MESSAGES.timeout;
    }
    return navigator.onLine ? ERROR_MESSAGES.network : ERROR_MESSAGES.offline;
  }

  if (statusCode) {
    if (options?.preferServerMessage && statusCode < 500) {
      const serverMessage = getServerMessage(error);
      if (serverMessage) return serverMessage;
    }

    const rule =
      statusCode >= 500 ? SERVER_ERROR_RULE : STATUS_ERROR_RULES[statusCode];
    if (rule) {
      return rule.preferServerMessage ? message || rule.message : rule.message;
    }
  }

  if (!navigator.onLine) return ERROR_MESSAGES.offline;

  return message || ERROR_MESSAGES.fallback;
}

let lastToast = { key: "", at: 0 };

export function showApiErrorToast(
  error: unknown,
  opts?: { title?: string },
): void {
  if (isCancelledError(error)) return;

  const { statusCode } = extractApiError(error);
  if (statusCode && SILENT_TOAST_STATUSES.includes(statusCode)) return;

  const title = opts?.title ?? ERROR_MESSAGES.toastTitle;
  const description = getDisplayMessage(error);

  const key = `${title}|${description}`;
  const now = Date.now();
  if (key === lastToast.key && now - lastToast.at < ERROR_TOAST_DEDUPE_MS)
    return;
  lastToast = { key, at: now };

  toast.add({ title, description, type: "error" });
}

export interface HandleApiErrorOptions {
  context?: string;
  title?: string;
  silent?: boolean;
}

export function handleApiError(
  error: unknown,
  opts: HandleApiErrorOptions = {},
): void {
  logDevError(error, opts.context);
  if (opts.silent) return;
  showApiErrorToast(error, { title: opts.title });
}

function toFieldPath(key: string): string {
  return key.replace(
    /(^|\.)([A-Z])/g,
    (_match, separator: string, char: string) => separator + char.toLowerCase(),
  );
}

export function getFieldErrors(error: unknown): Record<string, string> {
  const { errors } = extractApiError(error);
  if (!errors) return {};
  return Object.fromEntries(
    Object.entries(errors).map(([key, msgs]) => [
      toFieldPath(key),
      msgs[0] ?? "",
    ]),
  );
}

export function hasFieldErrors(error: unknown): boolean {
  return Object.keys(getFieldErrors(error)).length > 0;
}

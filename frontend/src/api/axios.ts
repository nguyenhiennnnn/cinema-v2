import axios, {
  AxiosError,
  type AxiosInstance,
  type AxiosRequestConfig,
  type AxiosResponse,
  type InternalAxiosRequestConfig,
} from "axios";
import { useAuthStore, type AuthUser } from "@/stores/auth-store";
import { sessionHint } from "@/lib/session-hint";
import { isPasswordChangeRequiredError, logDevError } from "@/lib/error";
import { syncServerClock } from "@/lib/server-clock";
import { ENV } from "@/config/env";
import { ROUTES } from "@/config/routes";
import { API_TIMEOUT_MS } from "@/config/default-constants";
import { ENDPOINTS } from "@/api/endpoints";

declare module "axios" {
  interface InternalAxiosRequestConfig {
    _clockSentAt?: number;
  }
}

function readDateHeader(headers: unknown): string | undefined {
  if (!headers || typeof headers !== "object") return undefined;
  const h = headers as { get?: (name: string) => unknown } & Record<
    string,
    unknown
  >;
  const value = typeof h.get === "function" ? h.get("date") : h["date"];
  return typeof value === "string" ? value : undefined;
}

function trackServerClock(
  response: Pick<AxiosResponse, "headers" | "config"> | undefined,
  sentAt?: number,
) {
  if (!response) return;
  const startedAt = sentAt ?? response.config?._clockSentAt;
  const rttMs =
    startedAt === undefined ? undefined : performance.now() - startedAt;
  syncServerClock(readDateHeader(response.headers), rttMs);
}


function installServerClockSync(instance: AxiosInstance) {
  instance.interceptors.request.use((config) => {
    config._clockSentAt = performance.now();
    return config;
  });
  instance.interceptors.response.use(
    (response) => {
      trackServerClock(response);
      return response;
    },
    (error: unknown) => {
      if (axios.isAxiosError(error)) trackServerClock(error.response);
      return Promise.reject(error);
    },
  );
}

const BASE_URL = ENV.API_BASE_URL;

const AUTH_REFRESH_URL = `${BASE_URL}${ENDPOINTS.auth.refresh}`;

const CSRF_HEADERS = { "X-Requested-With": "XMLHttpRequest" };
const REFRESH_LOCK_NAME = "perfo:auth-refresh";

export const apiClient = axios.create({
  baseURL: BASE_URL,
  timeout: API_TIMEOUT_MS,
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
    ...CSRF_HEADERS,
  },
});

installServerClockSync(apiClient);

const refreshClient = axios.create({
  timeout: API_TIMEOUT_MS,
  withCredentials: true,
  headers: CSRF_HEADERS,
});
installServerClockSync(refreshClient);

apiClient.interceptors.request.use((config) => {
  const { accessToken } = useAuthStore.getState();
  if (accessToken && config.headers) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

type RetryableConfig = InternalAxiosRequestConfig & { _retry?: boolean };

let refreshPromise: Promise<string> | null = null;

async function requestNewAccessToken(): Promise<string> {
  const { data } = await refreshClient.post<{
    data: {
      accessToken: string;
      user?: AuthUser;
    };
  }>(AUTH_REFRESH_URL);

  const newAccessToken = data.data.accessToken;

  if (data.data.user) {
    useAuthStore.getState().setCredentials(newAccessToken, data.data.user);
  } else {
    useAuthStore.getState().setAccessToken(newAccessToken);
  }

  return newAccessToken;
}

function runExclusiveAcrossTabs<T>(task: () => Promise<T>): Promise<T> {
  const locks = typeof navigator === "undefined" ? undefined : navigator.locks;
  return locks ? locks.request(REFRESH_LOCK_NAME, task) : task();
}

export function refreshAccessToken(): Promise<string> {
  if (!refreshPromise) {
    refreshPromise = runExclusiveAcrossTabs(requestNewAccessToken).finally(
      () => {
        refreshPromise = null;
      },
    );
  }
  return refreshPromise;
}

const AUTH_EXCLUDED_URLS = [
  ENDPOINTS.auth.login,
  ENDPOINTS.auth.adminLogin,
  ENDPOINTS.auth.google,
  ENDPOINTS.auth.register,
  ENDPOINTS.auth.confirmEmail,
  ENDPOINTS.auth.forgotPassword,
  ENDPOINTS.auth.resetPassword,
  ENDPOINTS.auth.resendConfirmation,
  ENDPOINTS.auth.refresh,
  ENDPOINTS.auth.logout,
];

function isAuthExcludedUrl(url?: string): boolean {
  if (!url) return false;
  return AUTH_EXCLUDED_URLS.some((path) => url.includes(path));
}

function redirectToLogin() {
  const currentPath = window.location.pathname + window.location.search;
  const isAuthRoute =
    currentPath.startsWith(ROUTES.auth.root) ||
    currentPath.startsWith(ROUTES.admin.login);
  if (isAuthRoute) return;
  const isAdminRoute = currentPath.startsWith(ROUTES.admin.root);
  window.location.href = isAdminRoute
    ? ROUTES.admin.login
    : `${ROUTES.auth.login}?redirect=${encodeURIComponent(currentPath)}`;
}

function redirectToChangePassword() {
  const { user, updateUser } = useAuthStore.getState();
  // Bật cờ trong store: AdminProtectedRoute tự chuyển mọi đường dẫn admin sang trang đổi mật khẩu.
  if (user && !user.mustChangePassword) updateUser({ mustChangePassword: true });
  // Ngoài khu admin không có guard nào bắt cờ, nên chuyển thẳng.
  const { pathname } = window.location;
  if (!pathname.startsWith(ROUTES.admin.root)) {
    window.location.assign(ROUTES.admin.changePassword);
  }
}

apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as RetryableConfig | undefined;

    if (isPasswordChangeRequiredError(error)) {
      redirectToChangePassword();
      return Promise.reject(error);
    }

    if (
      error.response?.status === 401 &&
      originalRequest &&
      !originalRequest._retry &&
      !isAuthExcludedUrl(originalRequest.url)
    ) {
      if (!sessionHint.has()) {
        useAuthStore.getState().logout();
        redirectToLogin();
        return Promise.reject(error);
      }

      originalRequest._retry = true;

      try {
        const newToken = await refreshAccessToken();
        originalRequest.headers = originalRequest.headers ?? {};
        originalRequest.headers.Authorization = `Bearer ${newToken}`;
        return apiClient(originalRequest);
      } catch (refreshError) {
        const refreshStatus = (refreshError as AxiosError)?.response?.status;
        const isNetworkError =
          axios.isAxiosError(refreshError) && !refreshError.response;

        if (
          !isNetworkError &&
          (!refreshStatus || refreshStatus === 401 || refreshStatus === 403)
        ) {
          logDevError(refreshError, "Token Refresh — phiên đăng nhập hết hạn");
          useAuthStore.getState().logout();
          redirectToLogin();
        } else {
          logDevError(
            refreshError,
            "Token Refresh — lỗi server tạm thời, giữ session",
          );
        }

        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  },
);

export async function apiGet<T>(url: string, config?: AxiosRequestConfig) {
  const { data } = await apiClient.get<T>(url, config);
  return data;
}

export async function apiPost<T>(
  url: string,
  body?: unknown,
  config?: AxiosRequestConfig,
) {
  const { data } = await apiClient.post<T>(url, body, config);
  return data;
}

export async function apiPut<T>(
  url: string,
  body?: unknown,
  config?: AxiosRequestConfig,
) {
  const { data } = await apiClient.put<T>(url, body, config);
  return data;
}

export async function apiDelete<T>(url: string, config?: AxiosRequestConfig) {
  const { data } = await apiClient.delete<T>(url, config);
  return data;
}

export async function apiUpload<T>(
  url: string,
  formData: FormData,
  config?: AxiosRequestConfig,
) {
  const { data } = await apiClient.post<T>(url, formData, {
    ...config,
    headers: { ...config?.headers, "Content-Type": "multipart/form-data" },
  });
  return data;
}

export function apiPostKeepalive(url: string): void {
  const { accessToken } = useAuthStore.getState();
  if (!accessToken) return;
  try {
    void fetch(`${BASE_URL}${url}`, {
      method: "POST",
      keepalive: true,
      credentials: "include",
      headers: {
        ...CSRF_HEADERS,
        Authorization: `Bearer ${accessToken}`,
      },
    }).catch(() => undefined);
  } catch {
    // trình duyệt từ chối keepalive: bỏ qua, dựa vào thời hạn giữ
  }
}

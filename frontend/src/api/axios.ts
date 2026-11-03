import axios, {
  AxiosError,
  type AxiosRequestConfig,
  type InternalAxiosRequestConfig,
} from "axios";
import {
  useAuthStore,
  refreshTokenStorage,
  type AuthUser,
} from "@/stores/auth-store";
import { logDevError } from "@/lib/error";
import { ENV } from "@/config/env";
import { ROUTES } from "@/config/routes";
import { API_TIMEOUT_MS } from "@/config/default-constants";

const BASE_URL = ENV.API_BASE_URL;

const AUTH_REFRESH_URL = `${BASE_URL}/auth/refresh`;

export const apiClient = axios.create({
  baseURL: BASE_URL,
  timeout: API_TIMEOUT_MS,
  headers: {
    "Content-Type": "application/json",
  },
});

apiClient.interceptors.request.use((config) => {
  const { accessToken } = useAuthStore.getState();
  if (accessToken && config.headers) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

// ─── Singleton Refresh Token Promise ──────────────────────────────────────────
type RetryableConfig = InternalAxiosRequestConfig & { _retry?: boolean };

let refreshPromise: Promise<string> | null = null;

export async function refreshAccessToken(): Promise<string> {
  if (refreshPromise) {
    return refreshPromise;
  }

  const refreshToken = refreshTokenStorage.get();
  if (!refreshToken) {
    throw new Error("No refresh token available");
  }

  refreshPromise = (async () => {
    try {
      const { data } = await axios.post<{
        data: {
          accessToken: string;
          refreshToken: string;
          user?: AuthUser;
        };
      }>(AUTH_REFRESH_URL, { refreshToken });

      const newAccessToken = data.data.accessToken;
      const newRefreshToken = data.data.refreshToken ?? refreshToken;

      refreshTokenStorage.set(newRefreshToken);
      if (data.data.user) {
        useAuthStore.getState().setCredentials(newAccessToken, data.data.user);
      } else {
        useAuthStore.getState().setAccessToken(newAccessToken);
      }

      return newAccessToken;
    } finally {
      refreshPromise = null;
    }
  })();

  return refreshPromise;
}

// ─── Auth Exclusion & Redirect Helpers ─────────────────────────────────────────
const AUTH_EXCLUDED_URLS = [
  "/auth/login",
  "/auth/admin-login",
  "/auth/google",
  "/auth/register",
  "/auth/confirm-email",
  "/auth/forgot-password",
  "/auth/reset-password",
  "/auth/resend-confirmation",
  "/auth/refresh",
  "/auth/logout",
];

function isAuthExcludedUrl(url?: string): boolean {
  if (!url) return false;
  return AUTH_EXCLUDED_URLS.some((path) => url.includes(path));
}

function redirectToLogin() {
  const currentPath = window.location.pathname + window.location.search;
  const isAuthRoute =
    currentPath.startsWith("/auth") ||
    currentPath.startsWith(ROUTES.admin.login);
  if (isAuthRoute) return;
  const isAdminRoute = currentPath.startsWith("/admin");
  window.location.href = isAdminRoute
    ? ROUTES.admin.login
    : `${ROUTES.auth.login}?redirect=${encodeURIComponent(currentPath)}`;
}

apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as RetryableConfig | undefined;

    if (
      error.response?.status === 401 &&
      originalRequest &&
      !originalRequest._retry &&
      !isAuthExcludedUrl(originalRequest.url)
    ) {
      const refreshToken = refreshTokenStorage.get();

      if (!refreshToken) {
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
        const hasResponse = (refreshError as AxiosError)?.response != null;

        if (!hasResponse || refreshStatus === 401 || refreshStatus === 403) {
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

// ─── Typed HTTP helpers ───────────────────────────────────────────────────────
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

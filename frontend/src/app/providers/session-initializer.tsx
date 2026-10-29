import { useEffect } from "react";
import type { AxiosError } from "axios";
import { refreshAccessToken } from "@/api/axios";
import { useAuthStore, refreshTokenStorage } from "@/stores/auth-store";
import { logDevError } from "@/lib/error";

export const SessionInitializer = () => {
  const { setInitialized, isInitialized } = useAuthStore();

  useEffect(() => {
    if (isInitialized) return;

    const refreshToken = refreshTokenStorage.get();

    if (!refreshToken) {
      setInitialized();
      return;
    }

    (async () => {
      try {
        await refreshAccessToken();
      } catch (err) {
        logDevError(err, "SessionInitializer");

        const status = (err as AxiosError)?.response?.status;
        const isConfirmedAuthFailure = status === 401 || status === 403;

        if (isConfirmedAuthFailure) {
          refreshTokenStorage.remove();
          useAuthStore.getState().logout();
        }
      } finally {
        setInitialized();
      }
    })();
  }, [isInitialized, setInitialized]);

  return null;
}
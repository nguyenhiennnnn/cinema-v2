import {
  HubConnectionBuilder,
  LogLevel,
  type HubConnection,
} from "@microsoft/signalr";
import { authApi } from "@/features/auth/api/auth-api";
import { useAuthStore } from "@/stores/auth-store";
import { ENV } from "@/config/env";
import { SIGNALR_RECONNECT_DELAYS_MS } from "@/config/default-constants";
import { SIGNALR_HUB_URL } from "@/lib/signalr";

export function createHubConnection(): HubConnection {
  return new HubConnectionBuilder()
    .withUrl(SIGNALR_HUB_URL, {
      accessTokenFactory: async () => {
        try {
          const res = await authApi.getSocketToken();
          return res.data.token;
        } catch {
          return useAuthStore.getState().accessToken ?? "";
        }
      },
    })
    .withAutomaticReconnect(SIGNALR_RECONNECT_DELAYS_MS)
    .configureLogging(ENV.IS_DEV ? LogLevel.Information : LogLevel.Warning)
    .build();
}

import {
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
  type HubConnection,
} from "@microsoft/signalr";
import { useEffect, useRef, useCallback } from "react";
import { useAuthStore } from "@/stores/auth-store";
import { authApi } from "@/features/auth/api/auth-api";
import {
  SIGNALR_HUB_URL,
  SignalREvents,
  type SeatHeldPayload,
  type SeatBookedPayload,
  type SeatReleasedPayload,
  type HoldExpiredPayload,
} from "@/lib/signalr";
import { ENV } from "@/config/env";
import { SIGNALR_RECONNECT_DELAYS_MS } from "@/config/default-constants";

export interface ShowtimeHubCallbacks {
  onSeatHeld?: (payload: SeatHeldPayload) => void;
  onSeatBooked?: (payload: SeatBookedPayload) => void;
  onSeatReleased?: (payload: SeatReleasedPayload) => void;
  onHoldExpired?: (payload: HoldExpiredPayload) => void;
}

export function useShowtimeHub(
  params: { showtimeId: number | null } & ShowtimeHubCallbacks,
) {
  const {
    showtimeId,
    onSeatHeld,
    onSeatBooked,
    onSeatReleased,
    onHoldExpired,
  } = params;
  const connectionRef = useRef<HubConnection | null>(null);

  const onSeatHeldRef = useRef(onSeatHeld);
  const onSeatBookedRef = useRef(onSeatBooked);
  const onSeatReleasedRef = useRef(onSeatReleased);
  const onHoldExpiredRef = useRef(onHoldExpired);

  useEffect(() => {
    onSeatHeldRef.current = onSeatHeld;
    onSeatBookedRef.current = onSeatBooked;
    onSeatReleasedRef.current = onSeatReleased;
    onHoldExpiredRef.current = onHoldExpired;
  }, [onSeatHeld, onSeatBooked, onSeatReleased, onHoldExpired]);

  useEffect(() => {
    if (!showtimeId) return;

    const connection = new HubConnectionBuilder()
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
      .configureLogging(
        ENV.IS_DEV ? LogLevel.Information : LogLevel.Warning,
      )
      .build();

    connection.on(SignalREvents.SeatHeld, (payload: SeatHeldPayload) =>
      onSeatHeldRef.current?.(payload),
    );
    connection.on(SignalREvents.SeatBooked, (payload: SeatBookedPayload) =>
      onSeatBookedRef.current?.(payload),
    );
    connection.on(SignalREvents.SeatReleased, (payload: SeatReleasedPayload) =>
      onSeatReleasedRef.current?.(payload),
    );
    connection.on(SignalREvents.HoldExpired, (payload: HoldExpiredPayload) =>
      onHoldExpiredRef.current?.(payload),
    );

    connection.onreconnected(() => {
      connection.invoke("JoinShowtime", showtimeId).catch((err) => {
        if (ENV.IS_DEV) {
          console.warn("[SignalR] Tham gia lại suất chiếu thất bại:", err);
        }
      });
    });

    connectionRef.current = connection;

    const startAndJoin = async () => {
      try {
        await connection.start();
        await connection.invoke("JoinShowtime", showtimeId);
      } catch (err) {
        if (ENV.IS_DEV) {
          console.warn("[SignalR] Kết nối thất bại:", err);
        }
      }
    };

    startAndJoin();

    return () => {
      const conn = connectionRef.current;
      if (!conn) return;

      const cleanup = async () => {
        try {
          if (conn.state === HubConnectionState.Connected) {
            await conn.invoke("LeaveShowtime", showtimeId);
          }
        } catch {
          // ignore
        } finally {
          await conn.stop();
        }
      };

      cleanup();
      connectionRef.current = null;
    };
  }, [showtimeId]);

  const leaveShowtime = useCallback(async () => {
    const conn = connectionRef.current;
    if (conn?.state === HubConnectionState.Connected && showtimeId) {
      await conn.invoke("LeaveShowtime", showtimeId);
    }
  }, [showtimeId]);

  return { leaveShowtime };
}

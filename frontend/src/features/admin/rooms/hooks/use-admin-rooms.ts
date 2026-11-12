import { useMutation, useQuery } from "@tanstack/react-query";
import { roomsApi } from "../api/rooms-api";
import { queryKeys } from "@/api/query-keys";
import type {
  RoomListParams,
  CreateRoomRequest,
  UpdateRoomRequest,
  UpdateSeatLayoutRequest,
} from "../types";

export function useAdminRooms(
  params?: RoomListParams,
  options?: { enabled?: boolean },
) {
  return useQuery({
    queryKey: queryKeys.adminRooms.list(params),
    queryFn: () => roomsApi.getRooms(params),
    select: (res) => res.data,
    enabled: options?.enabled ?? true,
  });
}

export function useAdminRoom(roomId: number) {
  return useQuery({
    queryKey: queryKeys.adminRooms.detail(roomId),
    queryFn: () => roomsApi.getRoom(roomId),
    enabled: roomId > 0,
    select: (res) => res.data,
  });
}

export function useAdminRoomSeats(roomId: number) {
  return useQuery({
    queryKey: queryKeys.adminRooms.seats(roomId),
    queryFn: () => roomsApi.getRoomSeats(roomId),
    enabled: roomId > 0,
    select: (res) => res.data,
  });
}

export function useAdminCreateRoom() {
  return useMutation({
    meta: { invalidates: "roomChanged" },
    mutationFn: (body: CreateRoomRequest) => roomsApi.createRoom(body),
  });
}

export function useAdminUpdateRoom() {
  return useMutation({
    meta: { invalidates: "roomChanged" },
    mutationFn: ({ id, body }: { id: number; body: UpdateRoomRequest }) =>
      roomsApi.updateRoom(id, body),
  });
}

export function useAdminUpdateSeatLayout(roomId: number) {
  return useMutation({
    meta: { invalidates: "seatLayoutChanged" },
    mutationFn: (body: UpdateSeatLayoutRequest) => roomsApi.updateSeatLayout(roomId, body),
  });
}

import { useMutation, useQuery } from "@tanstack/react-query";
import { roomsApi } from "../api/rooms-api";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
import type {
  RoomListParams,
  CreateRoomRequest,
  UpdateRoomRequest,
  UpdateSeatLayoutRequest,
} from "../types";

export function useAdminRooms(params?: RoomListParams) {
  return useQuery({
    queryKey: queryKeys.adminRooms.list(params),
    queryFn: () => roomsApi.getRooms(params),
    select: (res) => res.data,
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
    mutationFn: (body: CreateRoomRequest) => roomsApi.createRoom(body),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.adminRooms.all() }),
  });
}

export function useAdminUpdateRoom() {
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpdateRoomRequest }) =>
      roomsApi.updateRoom(id, body),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.adminRooms.all() }),
  });
}

export function useAdminDeleteRoom() {
  return useMutation({
    mutationFn: (id: number) => roomsApi.deleteRoom(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.adminRooms.all() }),
  });
}

export function useAdminUpdateSeatLayout(roomId: number) {
  return useMutation({
    mutationFn: (body: UpdateSeatLayoutRequest) => roomsApi.updateSeatLayout(roomId, body),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.adminRooms.seats(roomId) });
      queryClient.invalidateQueries({ queryKey: queryKeys.adminRooms.all() });
    },
  });
}

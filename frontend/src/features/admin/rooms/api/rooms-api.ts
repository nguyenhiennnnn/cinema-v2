import { apiDelete, apiGet, apiPost, apiPut } from "@/api/axios";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type {
  AdminRoomDto,
  CreateRoomRequest,
  UpdateRoomRequest,
  DeleteRoomResponse,
  RoomListParams,
  SeatDto,
  UpdateSeatLayoutRequest,
  UpdateSeatLayoutResponse,
} from "../types";
import { ENDPOINTS } from "@/api/endpoints";

export const roomsApi = {
  getRooms: (params?: RoomListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminRoomDto>>>(ENDPOINTS.admin.rooms.root, { params }),

  getRoom: (id: number) =>
    apiGet<ApiResponse<AdminRoomDto>>(ENDPOINTS.admin.rooms.byId(id)),

  createRoom: (body: CreateRoomRequest) =>
    apiPost<ApiResponse<AdminRoomDto>>(ENDPOINTS.admin.rooms.root, body),

  updateRoom: (id: number, body: UpdateRoomRequest) =>
    apiPut<ApiResponse<AdminRoomDto>>(ENDPOINTS.admin.rooms.byId(id), body),

  deleteRoom: (id: number) =>
    apiDelete<ApiResponse<DeleteRoomResponse>>(ENDPOINTS.admin.rooms.byId(id)),

  getRoomSeats: (roomId: number) =>
    apiGet<ApiResponse<SeatDto[]>>(ENDPOINTS.admin.rooms.seats(roomId)),

  updateSeatLayout: (roomId: number, body: UpdateSeatLayoutRequest) =>
    apiPut<ApiResponse<UpdateSeatLayoutResponse>>(ENDPOINTS.admin.rooms.seats(roomId), body),
};

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

const BASE = "/admin/rooms";

export const roomsApi = {
  getRooms: (params?: RoomListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminRoomDto>>>(BASE, { params }),

  getRoom: (id: number) =>
    apiGet<ApiResponse<AdminRoomDto>>(`${BASE}/${id}`),

  createRoom: (body: CreateRoomRequest) =>
    apiPost<ApiResponse<AdminRoomDto>>(BASE, body),

  updateRoom: (id: number, body: UpdateRoomRequest) =>
    apiPut<ApiResponse<AdminRoomDto>>(`${BASE}/${id}`, body),

  deleteRoom: (id: number) =>
    apiDelete<ApiResponse<DeleteRoomResponse>>(`${BASE}/${id}`),

  getRoomSeats: (roomId: number) =>
    apiGet<ApiResponse<SeatDto[]>>(`${BASE}/${roomId}/seats`),

  updateSeatLayout: (roomId: number, body: UpdateSeatLayoutRequest) =>
    apiPut<ApiResponse<UpdateSeatLayoutResponse>>(`${BASE}/${roomId}/seats`, body),
};

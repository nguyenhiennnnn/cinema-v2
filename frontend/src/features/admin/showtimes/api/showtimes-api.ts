import { apiDelete, apiGet, apiPost, apiPut } from "@/api/axios";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type {
  AdminShowtimeListParams,
  AdminShowtimeDto,
  UpsertShowtimeRequest,
  CreateShowtimeResponse,
  DeleteShowtimeResponse,
  BulkCreateShowtimesRequest,
  BulkCreateShowtimesResponse,
  BulkDeleteShowtimesRequest,
  BulkDeleteShowtimesResponse,
  AdminShowtimeCalendarParams,
  ShowtimeCalendarResponseDto,
  AutoScheduleRequest,
  AutoScheduleResponseDto,
  ApplyAutoScheduleRequest,
  CloneShowtimesRequest,
} from "../types";

const BASE = "/admin/showtimes";

export const showtimesAdminApi = {
  getShowtimes: (params: AdminShowtimeListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminShowtimeDto>>>(BASE, { params }),

  createShowtime: (body: UpsertShowtimeRequest) =>
    apiPost<ApiResponse<CreateShowtimeResponse>>(BASE, body),

  updateShowtime: (id: number, body: UpsertShowtimeRequest) =>
    apiPut<ApiResponse<AdminShowtimeDto>>(`${BASE}/${id}`, body),

  deleteShowtime: (id: number) =>
    apiDelete<ApiResponse<DeleteShowtimeResponse>>(`${BASE}/${id}`),

  bulkCreate: (body: BulkCreateShowtimesRequest) =>
    apiPost<ApiResponse<BulkCreateShowtimesResponse>>(`${BASE}/bulk`, body),

  bulkDelete: (body: BulkDeleteShowtimesRequest) =>
    apiDelete<ApiResponse<BulkDeleteShowtimesResponse>>(`${BASE}/bulk`, {
      data: body,
    }),

  previewAutoSchedule: (body: AutoScheduleRequest) =>
    apiPost<ApiResponse<AutoScheduleResponseDto>>(
      `${BASE}/auto-schedule/preview`,
      body,
    ),

  applyAutoSchedule: (body: ApplyAutoScheduleRequest) =>
    apiPost<ApiResponse<BulkCreateShowtimesResponse>>(
      `${BASE}/auto-schedule/apply`,
      body,
    ),

  cloneShowtimes: (body: CloneShowtimesRequest) =>
    apiPost<ApiResponse<BulkCreateShowtimesResponse>>(`${BASE}/clone`, body),

  getCalendar: (params: AdminShowtimeCalendarParams) =>
    apiGet<ApiResponse<ShowtimeCalendarResponseDto>>(`${BASE}/calendar`, {
      params,
    }),
};

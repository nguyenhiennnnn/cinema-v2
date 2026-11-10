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
import { ENDPOINTS } from "@/api/endpoints";

export const showtimesAdminApi = {
  getShowtimes: (params: AdminShowtimeListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminShowtimeDto>>>(ENDPOINTS.admin.showtimes.root, { params }),

  createShowtime: (body: UpsertShowtimeRequest) =>
    apiPost<ApiResponse<CreateShowtimeResponse>>(ENDPOINTS.admin.showtimes.root, body),

  updateShowtime: (id: number, body: UpsertShowtimeRequest) =>
    apiPut<ApiResponse<AdminShowtimeDto>>(ENDPOINTS.admin.showtimes.byId(id), body),

  deleteShowtime: (id: number) =>
    apiDelete<ApiResponse<DeleteShowtimeResponse>>(ENDPOINTS.admin.showtimes.byId(id)),

  bulkCreate: (body: BulkCreateShowtimesRequest) =>
    apiPost<ApiResponse<BulkCreateShowtimesResponse>>(ENDPOINTS.admin.showtimes.bulk, body),

  bulkDelete: (body: BulkDeleteShowtimesRequest) =>
    apiDelete<ApiResponse<BulkDeleteShowtimesResponse>>(ENDPOINTS.admin.showtimes.bulk, {
      data: body,
    }),

  previewAutoSchedule: (body: AutoScheduleRequest) =>
    apiPost<ApiResponse<AutoScheduleResponseDto>>(
      ENDPOINTS.admin.showtimes.autoSchedulePreview,
      body,
    ),

  applyAutoSchedule: (body: ApplyAutoScheduleRequest) =>
    apiPost<ApiResponse<BulkCreateShowtimesResponse>>(
      ENDPOINTS.admin.showtimes.autoScheduleApply,
      body,
    ),

  cloneShowtimes: (body: CloneShowtimesRequest) =>
    apiPost<ApiResponse<BulkCreateShowtimesResponse>>(ENDPOINTS.admin.showtimes.clone, body),

  getCalendar: (params: AdminShowtimeCalendarParams) =>
    apiGet<ApiResponse<ShowtimeCalendarResponseDto>>(ENDPOINTS.admin.showtimes.calendar, {
      params,
    }),
};

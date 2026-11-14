import { apiGet, apiPut } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type { AdminCinemaSettingsDto, UpdateSettingsRequest } from "../types";
import { ENDPOINTS } from "@/api/endpoints";

export const settingsApi = {
  getSettings: () =>
    apiGet<ApiResponse<AdminCinemaSettingsDto>>(ENDPOINTS.admin.settings),

  updateSettings: (body: UpdateSettingsRequest) =>
    apiPut<ApiResponse<AdminCinemaSettingsDto>>(ENDPOINTS.admin.settings, body),
};

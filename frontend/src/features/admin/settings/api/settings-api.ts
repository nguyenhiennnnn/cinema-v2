import { apiGet, apiPut } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type { CinemaSettingsDto, UpdateSettingsRequest } from "../types";
import { ENDPOINTS } from "@/api/endpoints";

export const settingsApi = {
  getSettings: () =>
    apiGet<ApiResponse<CinemaSettingsDto>>(ENDPOINTS.admin.settings),

  updateSettings: (body: UpdateSettingsRequest) =>
    apiPut<ApiResponse<CinemaSettingsDto>>(ENDPOINTS.admin.settings, body),
};

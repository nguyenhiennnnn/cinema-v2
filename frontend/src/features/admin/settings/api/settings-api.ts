import { apiGet, apiPut } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type { CinemaSettingsDto, UpdateSettingsRequest } from "../types";

const BASE = "/admin/settings";

export const settingsApi = {
  getSettings: () =>
    apiGet<ApiResponse<CinemaSettingsDto>>(BASE),

  updateSettings: (body: UpdateSettingsRequest) =>
    apiPut<ApiResponse<CinemaSettingsDto>>(BASE, body),
};

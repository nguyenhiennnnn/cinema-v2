import { apiGet } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type { CinemaSettingsDto } from "@/features/admin/settings/types";
import { ENDPOINTS } from "@/api/endpoints";

export const settingsApi = {
  getSettings: () =>
    apiGet<ApiResponse<CinemaSettingsDto>>(ENDPOINTS.settings),
};

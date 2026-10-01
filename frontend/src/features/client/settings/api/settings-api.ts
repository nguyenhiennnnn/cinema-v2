import { apiGet } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type { CinemaSettingsDto } from "@/features/admin/settings/types";

export const settingsApi = {
  getSettings: () =>
    apiGet<ApiResponse<CinemaSettingsDto>>("/settings"),
};

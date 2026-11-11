import { apiGet } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type { CinemaSettingsDto } from "@/features/admin/settings/types";
import { ENDPOINTS } from "@/api/endpoints";
import { setBusinessTimeZone } from "@/lib/business-time";

export const settingsApi = {
  getSettings: async () => {
    const res = await apiGet<ApiResponse<CinemaSettingsDto>>(ENDPOINTS.settings);
    const timeZoneId = res.data?.timeZoneId;
    if (timeZoneId) setBusinessTimeZone(timeZoneId);
    return res;
  },
};

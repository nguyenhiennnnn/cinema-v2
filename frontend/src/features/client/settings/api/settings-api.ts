import { apiGet } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type { CinemaSettingsDto } from "@/features/admin/settings/types";
import { ENDPOINTS } from "@/api/endpoints";
import { setBusinessTimeZone } from "@/lib/business-time";
import { setRegionalSettings } from "@/lib/regional-settings";

export const settingsApi = {
  getSettings: async () => {
    const res = await apiGet<ApiResponse<CinemaSettingsDto>>(ENDPOINTS.settings);
    const timeZoneId = res.data?.timeZoneId;
    if (timeZoneId) setBusinessTimeZone(timeZoneId);
    if (res.data) setRegionalSettings(res.data);
    return res;
  },
};

import { useQuery } from "@tanstack/react-query";
import { settingsApi } from "../api/settings-api";
import { queryKeys } from "@/api/query-keys";
import { SETTINGS_STALE_TIME_MS } from "@/config/default-constants";

export function useCinemaSettings() {
  return useQuery({
    queryKey: queryKeys.settings.get(),
    queryFn: () => settingsApi.getSettings(),
    select: (res) => res.data,
    staleTime: SETTINGS_STALE_TIME_MS,
  });
}

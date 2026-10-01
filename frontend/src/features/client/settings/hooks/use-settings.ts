import { useQuery } from "@tanstack/react-query";
import { settingsApi } from "../api/settings-api";
import { queryKeys } from "@/api/query-keys";

export function useCinemaSettings() {
  return useQuery({
    queryKey: queryKeys.settings.get(),
    queryFn: () => settingsApi.getSettings(),
    select: (res) => res.data,
    staleTime: 5 * 60 * 1000,
  });
}

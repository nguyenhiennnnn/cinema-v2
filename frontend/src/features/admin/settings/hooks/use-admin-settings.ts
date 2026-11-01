import { useMutation, useQuery } from "@tanstack/react-query";
import { settingsApi } from "../api/settings-api";
import { queryKeys } from "@/api/query-keys";
import type { UpdateSettingsRequest } from "../types";

export function useAdminSettings() {
  return useQuery({
    queryKey: queryKeys.adminSettings.get(),
    queryFn: () => settingsApi.getSettings(),
    select: (res) => res.data,
  });
}

export function useAdminUpdateSettings() {
  return useMutation({
    meta: { invalidates: "settingsChanged" },
    mutationFn: (body: UpdateSettingsRequest) => settingsApi.updateSettings(body),
  });
}

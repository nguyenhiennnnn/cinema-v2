import { useMutation, useQuery } from "@tanstack/react-query";
import { settingsApi } from "../api/settings-api";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
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
    mutationFn: (body: UpdateSettingsRequest) => settingsApi.updateSettings(body),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: queryKeys.adminSettings.get() }),
  });
}

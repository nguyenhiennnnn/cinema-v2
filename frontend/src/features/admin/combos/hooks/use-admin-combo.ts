import { useMutation, useQuery } from "@tanstack/react-query";
import type { ComboListParams, UpsertComboRequest } from "../types";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
import { combosAdminApi } from "../api/combos-api";

export function useAdminCombos(params?: ComboListParams) {
  return useQuery({
    queryKey: queryKeys.adminCombos.list(params),
    queryFn: () => combosAdminApi.getCombos(params),
    select: (res) => res.data,
  });
}

export function useAdminCreateCombo() {
  return useMutation({
    mutationFn: (body: UpsertComboRequest) => combosAdminApi.createCombo(body),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: queryKeys.adminCombos.all() }),
  });
}

export function useAdminUpdateCombo() {
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpsertComboRequest }) =>
      combosAdminApi.updateCombo(id, body),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: queryKeys.adminCombos.all() }),
  });
}

export function useAdminToggleCombo() {
  return useMutation({
    mutationFn: (id: number) => combosAdminApi.toggleCombo(id),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: queryKeys.adminCombos.all() }),
  });
}

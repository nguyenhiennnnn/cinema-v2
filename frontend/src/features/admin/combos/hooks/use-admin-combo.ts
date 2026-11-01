import { useMutation, useQuery } from "@tanstack/react-query";
import type { ComboListParams, UpsertComboRequest } from "../types";
import type { BulkStatusRequest } from "@/types/bulk";
import { queryKeys } from "@/api/query-keys";
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
    meta: { invalidates: "comboChanged" },
    mutationFn: (body: UpsertComboRequest) => combosAdminApi.createCombo(body),
  });
}

export function useAdminUpdateCombo() {
  return useMutation({
    meta: { invalidates: "comboChanged" },
    mutationFn: ({ id, body }: { id: number; body: UpsertComboRequest }) =>
      combosAdminApi.updateCombo(id, body),
  });
}

export function useAdminToggleCombo() {
  return useMutation({
    meta: { invalidates: "comboChanged" },
    mutationFn: (id: number) => combosAdminApi.toggleCombo(id),
  });
}

export function useAdminBulkSetComboActive() {
  return useMutation({
    meta: { invalidates: "comboChanged", skipSuccessToast: true },
    mutationFn: (body: BulkStatusRequest) => combosAdminApi.bulkSetActive(body),
  });
}

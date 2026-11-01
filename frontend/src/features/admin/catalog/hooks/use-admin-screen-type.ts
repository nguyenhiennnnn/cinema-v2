import { useMutation, useQuery } from "@tanstack/react-query";
import type { CatalogListParams, UpsertScreenTypeRequest } from "../types";
import { queryKeys } from "@/api/query-keys";
import { screenTypesAdminApi } from "../api/catalog-api";

export function useAdminScreenTypes(params?: CatalogListParams) {
  return useQuery({
    queryKey: queryKeys.adminCatalog.screenTypes(params),
    queryFn: () => screenTypesAdminApi.getAll(params),
    select: (res) => res.data,
  });
}

export function useAdminCreateScreenType() {
  return useMutation({
    meta: { invalidates: "screenTypeChanged" },
    mutationFn: (body: UpsertScreenTypeRequest) =>
      screenTypesAdminApi.create(body),
  });
}

export function useAdminUpdateScreenType() {
  return useMutation({
    meta: { invalidates: "screenTypeChanged" },
    mutationFn: ({ id, body }: { id: number; body: UpsertScreenTypeRequest }) =>
      screenTypesAdminApi.update(id, body),
  });
}

export function useAdminDeleteScreenType() {
  return useMutation({
    meta: { invalidates: "screenTypeChanged" },
    mutationFn: (id: number) => screenTypesAdminApi.delete(id),
  });
}

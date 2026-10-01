import { useMutation, useQuery } from "@tanstack/react-query";
import type { CatalogListParams, UpsertScreenTypeRequest } from "../types";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
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
    mutationFn: (body: UpsertScreenTypeRequest) =>
      screenTypesAdminApi.create(body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.screenTypes(),
      }),
  });
}

export function useAdminUpdateScreenType() {
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpsertScreenTypeRequest }) =>
      screenTypesAdminApi.update(id, body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.screenTypes(),
      }),
  });
}

export function useAdminDeleteScreenType() {
  return useMutation({
    mutationFn: (id: number) => screenTypesAdminApi.delete(id),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.screenTypes(),
      }),
  });
}

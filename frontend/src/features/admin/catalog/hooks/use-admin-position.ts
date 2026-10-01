import { useMutation, useQuery } from "@tanstack/react-query";
import type { CatalogListParams, UpsertPositionRequest } from "../types";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
import { positionsAdminApi } from "../api/catalog-api";

export function useAdminPositions(params?: CatalogListParams) {
  return useQuery({
    queryKey: queryKeys.adminCatalog.positions(params),
    queryFn: () => positionsAdminApi.getAll(params),
    select: (res) => res.data,
  });
}

export function useAdminCreatePosition() {
  return useMutation({
    mutationFn: (body: UpsertPositionRequest) => positionsAdminApi.create(body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.positions(),
      }),
  });
}

export function useAdminUpdatePosition() {
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpsertPositionRequest }) =>
      positionsAdminApi.update(id, body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.positions(),
      }),
  });
}

export function useAdminDeletePosition() {
  return useMutation({
    mutationFn: (id: number) => positionsAdminApi.delete(id),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.positions(),
      }),
  });
}

import { useMutation, useQuery } from "@tanstack/react-query";
import type { CatalogListParams, UpsertPositionRequest } from "../types";
import { queryKeys } from "@/api/query-keys";
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
    meta: { invalidates: "positionChanged" },
    mutationFn: (body: UpsertPositionRequest) => positionsAdminApi.create(body),
  });
}

export function useAdminUpdatePosition() {
  return useMutation({
    meta: { invalidates: "positionChanged" },
    mutationFn: ({ id, body }: { id: number; body: UpsertPositionRequest }) =>
      positionsAdminApi.update(id, body),
  });
}

export function useAdminDeletePosition() {
  return useMutation({
    meta: { invalidates: "positionChanged" },
    mutationFn: (id: number) => positionsAdminApi.delete(id),
  });
}

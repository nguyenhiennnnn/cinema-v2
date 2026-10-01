import { useMutation, useQuery } from "@tanstack/react-query";
import type { CatalogListParams, UpsertSeatTypeRequest } from "../types";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
import { seatTypesAdminApi } from "../api/catalog-api";

export function useAdminSeatTypes(params?: CatalogListParams) {
  return useQuery({
    queryKey: queryKeys.adminCatalog.seatTypes(params),
    queryFn: () => seatTypesAdminApi.getAll(params),
    select: (res) => res.data,
  });
}

export function useAdminCreateSeatType() {
  return useMutation({
    mutationFn: (body: UpsertSeatTypeRequest) => seatTypesAdminApi.create(body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.seatTypes(),
      }),
  });
}

export function useAdminUpdateSeatType() {
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpsertSeatTypeRequest }) =>
      seatTypesAdminApi.update(id, body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.seatTypes(),
      }),
  });
}

export function useAdminDeleteSeatType() {
  return useMutation({
    mutationFn: (id: number) => seatTypesAdminApi.delete(id),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.seatTypes(),
      }),
  });
}

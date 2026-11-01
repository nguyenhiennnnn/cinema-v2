import { useMutation, useQuery } from "@tanstack/react-query";
import type { CatalogListParams, UpsertSeatTypeRequest } from "../types";
import { queryKeys } from "@/api/query-keys";
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
    meta: { invalidates: "seatTypeChanged" },
    mutationFn: (body: UpsertSeatTypeRequest) => seatTypesAdminApi.create(body),
  });
}

export function useAdminUpdateSeatType() {
  return useMutation({
    meta: { invalidates: "seatTypeChanged" },
    mutationFn: ({ id, body }: { id: number; body: UpsertSeatTypeRequest }) =>
      seatTypesAdminApi.update(id, body),
  });
}

export function useAdminDeleteSeatType() {
  return useMutation({
    meta: { invalidates: "seatTypeChanged" },
    mutationFn: (id: number) => seatTypesAdminApi.delete(id),
  });
}

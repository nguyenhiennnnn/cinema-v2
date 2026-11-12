import { keepPreviousData, useMutation, useQuery } from "@tanstack/react-query";
import type { PersonListParams, UpsertPersonRequest } from "../types";
import type { BulkIdsRequest } from "@/types/bulk";
import { queryKeys } from "@/api/query-keys";
import { directorsAdminApi } from "../api/catalog-api";

export function useAdminDirectors(params?: PersonListParams) {
  return useQuery({
    queryKey: queryKeys.adminCatalog.directors(params),
    queryFn: () => directorsAdminApi.getAll(params),
    placeholderData: keepPreviousData,
    select: (res) => res.data,
  });
}

export function useAdminCreateDirector() {
  return useMutation({
    meta: { invalidates: "directorChanged" },
    mutationFn: (body: UpsertPersonRequest) => directorsAdminApi.create(body),
  });
}

export function useAdminUpdateDirector() {
  return useMutation({
    meta: { invalidates: "directorChanged" },
    mutationFn: ({ id, body }: { id: number; body: UpsertPersonRequest }) =>
      directorsAdminApi.update(id, body),
  });
}

export function useAdminDeleteDirector() {
  return useMutation({
    meta: { invalidates: "directorChanged" },
    mutationFn: (id: number) => directorsAdminApi.delete(id),
  });
}

export function useAdminBulkDeleteDirectors() {
  return useMutation({
    meta: { invalidates: "directorChanged" },
    mutationFn: (body: BulkIdsRequest) => directorsAdminApi.bulkDelete(body),
  });
}

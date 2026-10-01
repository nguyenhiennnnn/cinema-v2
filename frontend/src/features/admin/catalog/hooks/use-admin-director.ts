import { useMutation, useQuery } from "@tanstack/react-query";
import type { PersonListParams, UpsertPersonRequest } from "../types";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
import { directorsAdminApi } from "../api/catalog-api";

export function useAdminDirectors(params?: PersonListParams) {
  return useQuery({
    queryKey: queryKeys.adminCatalog.directors(params),
    queryFn: () => directorsAdminApi.getAll(params),
    select: (res) => res.data,
  });
}

export function useAdminCreateDirector() {
  return useMutation({
    mutationFn: (body: UpsertPersonRequest) => directorsAdminApi.create(body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.directors(),
      }),
  });
}

export function useAdminUpdateDirector() {
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpsertPersonRequest }) =>
      directorsAdminApi.update(id, body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.directors(),
      }),
  });
}

export function useAdminDeleteDirector() {
  return useMutation({
    mutationFn: (id: number) => directorsAdminApi.delete(id),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.directors(),
      }),
  });
}

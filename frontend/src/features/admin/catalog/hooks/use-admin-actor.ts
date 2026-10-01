import { useMutation, useQuery } from "@tanstack/react-query";
import type { PersonListParams, UpsertPersonRequest } from "../types";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
import { actorsAdminApi } from "../api/catalog-api";

export function useAdminActors(params?: PersonListParams) {
  return useQuery({
    queryKey: queryKeys.adminCatalog.actors(params),
    queryFn: () => actorsAdminApi.getAll(params),
    select: (res) => res.data,
  });
}

export function useAdminCreateActor() {
  return useMutation({
    mutationFn: (body: UpsertPersonRequest) =>
      actorsAdminApi.create(body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.actors(),
      }),
  });
}

export function useAdminUpdateActor() {
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpsertPersonRequest }) =>
      actorsAdminApi.update(id, body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.actors(),
      }),
  });
}

export function useAdminDeleteActor() {
  return useMutation({
    mutationFn: (id: number) => actorsAdminApi.delete(id),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.actors(),
      }),
  });
}

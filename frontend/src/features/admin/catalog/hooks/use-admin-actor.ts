import { useMutation, useQuery } from "@tanstack/react-query";
import type { PersonListParams, UpsertPersonRequest } from "../types";
import { queryKeys } from "@/api/query-keys";
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
    meta: { invalidates: "actorChanged" },
    mutationFn: (body: UpsertPersonRequest) =>
      actorsAdminApi.create(body),
  });
}

export function useAdminUpdateActor() {
  return useMutation({
    meta: { invalidates: "actorChanged" },
    mutationFn: ({ id, body }: { id: number; body: UpsertPersonRequest }) =>
      actorsAdminApi.update(id, body),
  });
}

export function useAdminDeleteActor() {
  return useMutation({
    meta: { invalidates: "actorChanged" },
    mutationFn: (id: number) => actorsAdminApi.delete(id),
  });
}

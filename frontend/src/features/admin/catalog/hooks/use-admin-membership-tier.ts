import { useMutation, useQuery } from "@tanstack/react-query";
import type { CatalogListParams, UpsertMembershipTierRequest } from "../types";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
import { membershipTiersAdminApi } from "../api/catalog-api";

export function useAdminMembershipTiers(params?: CatalogListParams) {
  return useQuery({
    queryKey: queryKeys.adminCatalog.membershipTiers(params),
    queryFn: () => membershipTiersAdminApi.getAll(params),
    select: (res) => res.data,
  });
}

export function useAdminCreateMembershipTier() {
  return useMutation({
    mutationFn: (body: UpsertMembershipTierRequest) =>
      membershipTiersAdminApi.create(body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.membershipTiers(),
      }),
  });
}

export function useAdminUpdateMembershipTier() {
  return useMutation({
    mutationFn: ({
      id,
      body,
    }: {
      id: number;
      body: UpsertMembershipTierRequest;
    }) => membershipTiersAdminApi.update(id, body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.membershipTiers(),
      }),
  });
}

export function useAdminDeleteMembershipTier() {
  return useMutation({
    mutationFn: (id: number) => membershipTiersAdminApi.delete(id),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.membershipTiers(),
      }),
  });
}

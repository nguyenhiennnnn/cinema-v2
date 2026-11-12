import { keepPreviousData, useMutation, useQuery } from "@tanstack/react-query";
import type { CatalogListParams, UpsertMembershipTierRequest } from "../types";
import { queryKeys } from "@/api/query-keys";
import { membershipTiersAdminApi } from "../api/catalog-api";

export function useAdminMembershipTiers(params?: CatalogListParams) {
  return useQuery({
    queryKey: queryKeys.adminCatalog.membershipTiers(params),
    queryFn: () => membershipTiersAdminApi.getAll(params),
    placeholderData: keepPreviousData,
    select: (res) => res.data,
  });
}

export function useAdminCreateMembershipTier() {
  return useMutation({
    meta: { invalidates: "membershipTierChanged" },
    mutationFn: (body: UpsertMembershipTierRequest) =>
      membershipTiersAdminApi.create(body),
  });
}

export function useAdminUpdateMembershipTier() {
  return useMutation({
    meta: { invalidates: "membershipTierChanged" },
    mutationFn: ({
      id,
      body,
    }: {
      id: number;
      body: UpsertMembershipTierRequest;
    }) => membershipTiersAdminApi.update(id, body),
  });
}

export function useAdminDeleteMembershipTier() {
  return useMutation({
    meta: { invalidates: "membershipTierChanged" },
    mutationFn: (id: number) => membershipTiersAdminApi.delete(id),
  });
}

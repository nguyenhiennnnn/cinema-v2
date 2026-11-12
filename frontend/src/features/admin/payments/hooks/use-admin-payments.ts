import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { paymentsAdminApi } from "../api/payments-admin-api";
import { queryKeys } from "@/api/query-keys";
import type { AdminPaymentListParams } from "../types";

export function useAdminPayments(params: AdminPaymentListParams) {
  return useQuery({
    queryKey: queryKeys.adminPayments.list(params),
    queryFn: () => paymentsAdminApi.getPayments(params),
    placeholderData: keepPreviousData,
    select: (res) => res.data,
  });
}

export function useAdminPaymentDetail(id: number) {
  return useQuery({
    queryKey: queryKeys.adminPayments.detail(id),
    queryFn: () => paymentsAdminApi.getPaymentDetail(id),
    enabled: id > 0,
    select: (res) => res.data,
  });
}

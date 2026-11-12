import { keepPreviousData, useMutation, useQuery } from "@tanstack/react-query";
import type { BulkStatusRequest } from "@/types/bulk";
import { queryKeys } from "@/api/query-keys";
import type {
  AdminUserListParams,
  AdminUserSelectParams,
  CustomerLookupParams,
  UpdateUserStatusRequest,
  UpdateUserRoleRequest,
} from "../types";
import { usersAdminApi } from "../api/user-api";

export function useAdminUsers(params: AdminUserListParams) {
  return useQuery({
    queryKey: queryKeys.adminUsers.list(params),
    queryFn: () => usersAdminApi.getUsers(params),
    placeholderData: keepPreviousData,
    select: (res) => res.data,
  });
}

export function useAdminUsersSelect(params?: AdminUserSelectParams) {
  return useQuery({
    queryKey: queryKeys.adminUsers.admins(params),
    queryFn: () => usersAdminApi.getAdminUsers(params),
    select: (res) => res.data,
  });
}

export function useCustomerLookup(params: CustomerLookupParams) {
  return useQuery({
    queryKey: queryKeys.adminUsers.customers(params),
    queryFn: () => usersAdminApi.searchCustomers(params),
    enabled: !!params.q && params.q.trim().length > 0,
    select: (res) => res.data,
  });
}

export function useAdminUpdateUserStatus() {
  return useMutation({
    meta: { invalidates: "userChanged" },
    mutationFn: ({ id, body }: { id: number; body: UpdateUserStatusRequest }) =>
      usersAdminApi.updateStatus(id, body),
  });
}

export function useAdminBulkUpdateUserStatus() {
  return useMutation({
    meta: { invalidates: "userChanged" },
    mutationFn: (body: BulkStatusRequest) => usersAdminApi.bulkUpdateStatus(body),
  });
}

export function useAdminUpdateUserRole() {
  return useMutation({
    meta: { invalidates: "userChanged" },
    mutationFn: ({ id, body }: { id: number; body: UpdateUserRoleRequest }) =>
      usersAdminApi.updateRole(id, body),
  });
}

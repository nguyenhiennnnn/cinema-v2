import { useMutation, useQuery } from "@tanstack/react-query";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
import type {
  AdminUserListParams,
  AdminUserSelectParams,
  UpdateUserStatusRequest,
  UpdateUserRoleRequest,
} from "../types";
import { usersAdminApi } from "../api/user-api";

export function useAdminUsers(params: AdminUserListParams) {
  return useQuery({
    queryKey: queryKeys.adminUsers.list(params),
    queryFn: () => usersAdminApi.getUsers(params),
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

export function useAdminUpdateUserStatus() {
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpdateUserStatusRequest }) =>
      usersAdminApi.updateStatus(id, body),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: queryKeys.adminUsers.all() }),
  });
}

export function useAdminUpdateUserRole() {
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpdateUserRoleRequest }) =>
      usersAdminApi.updateRole(id, body),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: queryKeys.adminUsers.all() }),
  });
}

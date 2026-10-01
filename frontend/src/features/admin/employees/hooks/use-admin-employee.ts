import { useMutation, useQuery } from "@tanstack/react-query";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
import type {
  CreateEmployeeRequest,
  AdminEmployeeListParams,
  UpdateEmployeeRequest,
} from "../types";
import { employeesAdminApi } from "../api/employee-api";

export function useAdminEmployees(params: AdminEmployeeListParams = {}) {
  return useQuery({
    queryKey: queryKeys.adminEmployees.list(params),
    queryFn: () => employeesAdminApi.getEmployees(params),
    select: (res) => res.data,
  });
}

export function useAdminCreateEmployee() {
  return useMutation({
    mutationFn: (body: CreateEmployeeRequest) =>
      employeesAdminApi.createEmployee(body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminEmployees.all(),
      }),
  });
}

export function useAdminUpdateEmployee() {
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpdateEmployeeRequest }) =>
      employeesAdminApi.updateEmployee(id, body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminEmployees.all(),
      }),
  });
}

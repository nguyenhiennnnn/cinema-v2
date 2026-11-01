import { useMutation, useQuery } from "@tanstack/react-query";
import { queryKeys } from "@/api/query-keys";
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
    meta: { invalidates: "employeeChanged" },
    mutationFn: (body: CreateEmployeeRequest) =>
      employeesAdminApi.createEmployee(body),
  });
}

export function useAdminUpdateEmployee() {
  return useMutation({
    meta: { invalidates: "employeeChanged" },
    mutationFn: ({ id, body }: { id: number; body: UpdateEmployeeRequest }) =>
      employeesAdminApi.updateEmployee(id, body),
  });
}

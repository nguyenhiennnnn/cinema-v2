import { keepPreviousData, useMutation, useQuery } from "@tanstack/react-query";
import { queryKeys } from "@/api/query-keys";
import type {
  CreateEmployeeDirectRequest,
  CreateEmployeeRequest,
  InviteEmployeeRequest,
  AdminEmployeeListParams,
  UpdateEmployeeRequest,
} from "@/features/admin/employees/types";
import { employeesAdminApi } from "../api/employee-api";

export function useAdminEmployees(params: AdminEmployeeListParams = {}) {
  return useQuery({
    queryKey: queryKeys.adminEmployees.list(params),
    queryFn: () => employeesAdminApi.getEmployees(params),
    placeholderData: keepPreviousData,
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

export function useAdminInviteEmployee() {
  return useMutation({
    meta: { invalidates: "employeeChanged" },
    mutationFn: (body: InviteEmployeeRequest) => employeesAdminApi.inviteEmployee(body),
  });
}

export function useAdminCreateEmployeeDirect() {
  return useMutation({
    meta: { invalidates: "employeeChanged" },
    mutationFn: (body: CreateEmployeeDirectRequest) =>
      employeesAdminApi.createEmployeeDirect(body),
  });
}

export function useAdminResetEmployeePassword() {
  return useMutation({
    meta: { invalidates: "employeeChanged", skipSuccessToast: true },
    mutationFn: (id: number) => employeesAdminApi.resetPassword(id),
  });
}

export function useAdminResendEmployeeInvite() {
  return useMutation({
    mutationFn: (id: number) => employeesAdminApi.resendInvite(id),
  });
}

export function useAdminUpdateEmployee() {
  return useMutation({
    meta: { invalidates: "employeeChanged" },
    mutationFn: ({ id, body }: { id: number; body: UpdateEmployeeRequest }) =>
      employeesAdminApi.updateEmployee(id, body),
  });
}

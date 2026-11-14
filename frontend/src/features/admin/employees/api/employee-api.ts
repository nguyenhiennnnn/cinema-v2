import { apiGet, apiPost, apiPut } from "@/api/axios";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type {
  AdminEmployeeListParams,
  AdminEmployeeDto,
  CreateEmployeeDirectRequest,
  CreateEmployeeRequest,
  InviteEmployeeRequest,
  UpdateEmployeeRequest,
} from "@/features/admin/employees/types";
import { ENDPOINTS } from "@/api/endpoints";

export const employeesAdminApi = {
  getEmployees: (params?: AdminEmployeeListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminEmployeeDto>>>(ENDPOINTS.admin.employees.root, { params }),

  createEmployee: (body: CreateEmployeeRequest) =>
    apiPost<ApiResponse<AdminEmployeeDto>>(ENDPOINTS.admin.employees.root, body),

  inviteEmployee: (body: InviteEmployeeRequest) =>
    apiPost<ApiResponse<AdminEmployeeDto>>(ENDPOINTS.admin.employees.invite, body),

  createEmployeeDirect: (body: CreateEmployeeDirectRequest) =>
    apiPost<ApiResponse<AdminEmployeeDto>>(ENDPOINTS.admin.employees.direct, body),

  resetPassword: (id: number) =>
    apiPost<ApiResponse<AdminEmployeeDto>>(ENDPOINTS.admin.employees.resetPassword(id)),

  resendInvite: (id: number) =>
    apiPost<ApiResponse<AdminEmployeeDto>>(ENDPOINTS.admin.employees.resendInvite(id)),

  updateEmployee: (id: number, body: UpdateEmployeeRequest) =>
    apiPut<ApiResponse<AdminEmployeeDto>>(ENDPOINTS.admin.employees.byId(id), body),
};

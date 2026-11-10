import { apiGet, apiPost, apiPut } from "@/api/axios";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type {
  AdminEmployeeListParams,
  AdminEmployeeDto,
  CreateEmployeeRequest,
  UpdateEmployeeRequest,
} from "../types";
import { ENDPOINTS } from "@/api/endpoints";

export const employeesAdminApi = {
  getEmployees: (params?: AdminEmployeeListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminEmployeeDto>>>(ENDPOINTS.admin.employees.root, { params }),

  createEmployee: (body: CreateEmployeeRequest) =>
    apiPost<ApiResponse<AdminEmployeeDto>>(ENDPOINTS.admin.employees.root, body),

  updateEmployee: (id: number, body: UpdateEmployeeRequest) =>
    apiPut<ApiResponse<AdminEmployeeDto>>(ENDPOINTS.admin.employees.byId(id), body),
};

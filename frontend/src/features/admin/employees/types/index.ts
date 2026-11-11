export interface AdminEmployeeListParams {
  positionId?: number;
  page?: number;
  pageSize?: number;
}

export interface AdminEmployeeDto {
  employeeId: number;
  userId: number;
  fullName: string;
  email: string;
  positionId?: number | null;
  positionName?: string | null;
  hireDate?: string | null;
  isActive: boolean;
  isPendingActivation: boolean;
}

export interface CreateEmployeeRequest {
  userId: number;
  positionId?: number | null;
  hireDate?: string | null;
  promoteCustomer?: boolean;
}

export interface InviteEmployeeRequest {
  fullName: string;
  email: string;
  phone?: string | null;
  positionId?: number | null;
  joiningDate?: string | null;
}

export interface UpdateEmployeeRequest {
  positionId?: number | null;
  hireDate?: string | null;
  isActive: boolean;
}

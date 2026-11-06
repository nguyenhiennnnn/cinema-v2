import { useState } from "react";
import { Plus } from "lucide-react";
import type { RowSelectionState } from "@tanstack/react-table";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { DataTable } from "@/components/common/data-table/data-table";
import { BulkUserStatusActions } from "@/components/common/data-table/bulk-user-status-actions";
import { createSelectionColumn } from "@/components/common/data-table/data-table-selection-column";
import { useAdminBulkUpdateUserStatus } from "@/features/admin/users/hooks/use-admin-users";
import { toastBulkResult } from "@/lib/bulk";
import { useAuthStore } from "@/stores/auth-store";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import { useAdminEmployees, useAdminUpdateEmployee } from "../hooks/use-admin-employee";
import type { AdminEmployeeDto } from "../types";
import { createEmployeeColumns } from "../components/employee-columns";
import { EmployeeCreateForm } from "../components/forms/employee-create-form";
import { EmployeeUpdateForm } from "../components/forms/employee-update-form";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { useAdminEmployeeParams } from "@/hooks/use-query-params";
import { stripNulls } from "@/lib/query-params";

const AdminEmployeesPage = () => {
  const [params, setParams] = useAdminEmployeeParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const employees = useAdminEmployees(stripNulls(params));
  const updateEmployee = useAdminUpdateEmployee();
  const currentUserId = useAuthStore((s) => s.user?.userId);
  const bulkUpdateStatus = useAdminBulkUpdateUserStatus();
  const [rowSelection, setRowSelection] = useState<RowSelectionState>({});

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlertDialog = useAlertDialogOpen();

  const handleCreate = () => {
    openDialog({
      title: "Thêm nhân viên mới",
      description: "Gán vị trí và ngày vào làm cho người dùng đã đăng ký.",
      content: <EmployeeCreateForm onSuccess={closeDialog} />,
    });
  };

  const handleEdit = (employee: AdminEmployeeDto) => {
    openDialog({
      title: "Cập nhật nhân viên",
      description: "Chỉnh sửa vị trí, ngày vào làm và trạng thái.",
      content: <EmployeeUpdateForm employee={employee} onSuccess={closeDialog} />,
    });
  };

  const handleToggle = (employee: AdminEmployeeDto) => {
    const willActivate = !employee.isActive;
    openAlertDialog({
      title: willActivate
        ? `Kích hoạt lại nhân viên "${employee.fullName}"?`
        : `Cho nhân viên "${employee.fullName}" nghỉ việc?`,
      description: willActivate
        ? "Nhân viên sẽ có thể đăng nhập và làm việc trở lại."
        : "Nhân viên sẽ không thể đăng nhập vào hệ thống quản trị nữa.",
      confirmText: willActivate ? "Kích hoạt" : "Cho nghỉ việc",
      variant: willActivate ? "default" : "destructive",
      onConfirm: async () => {
        await updateEmployee.mutateAsync({
          id: employee.employeeId,
          body: {
            positionId: employee.positionId ?? null,
            hireDate: employee.hireDate ?? null,
            isActive: willActivate,
          },
        });
      },
    });
  };

  const handleBulkSetActive = async (selectedUserIds: string[], isActive: boolean) => {
    const res = await bulkUpdateStatus.mutateAsync({
      ids: selectedUserIds.map(Number),
      isActive,
    });
    setRowSelection({});
    toastBulkResult(res.data, isActive ? "kích hoạt" : "cho nghỉ việc", "nhân viên");
  };

  const columns = [
    createSelectionColumn<AdminEmployeeDto>(),
    ...createEmployeeColumns({
      onEdit: handleEdit,
      onToggle: handleToggle,
      currentUserId,
    }),
  ];

  return (
    <div>
      <PageHeader
        title="Nhân viên"
        description="Quản lý danh sách nhân viên, vị trí và trạng thái làm việc."
        action={
          <Button onClick={handleCreate}>
            <Plus className="size-4" />
            <span>Thêm nhân viên</span>
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={employees.data?.items ?? []}
        isLoading={employees.isLoading}
        totalItems={employees.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        emptyMessage="Chưa có nhân viên nào."
        rowSelection={rowSelection}
        onRowSelectionChange={setRowSelection}
        getRowId={(row) => String(row.userId)}
        bulkActions={(selectedUserIds) => (
          <BulkUserStatusActions
            count={selectedUserIds.length}
            itemLabel="nhân viên"
            unlockLabel="Kích hoạt"
            lockLabel="Cho nghỉ việc"
            unlockDescription="Các nhân viên được chọn sẽ có thể đăng nhập và làm việc trở lại."
            lockDescription="Các nhân viên được chọn sẽ không thể đăng nhập vào hệ thống nữa. Tài khoản của bạn và quản trị viên cuối cùng sẽ không bị khóa."
            onSetActive={(isActive) => handleBulkSetActive(selectedUserIds, isActive)}
            isPending={bulkUpdateStatus.isPending}
          />
        )}
      />
    </div>
  );
};

export default AdminEmployeesPage;
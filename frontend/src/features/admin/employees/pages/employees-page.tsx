import { useState } from "react";
import { MailPlus, UserCheck, UserPlus } from "lucide-react";
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
import { toast } from "@/components/ui/toast";

import {
  useAdminEmployees,
  useAdminResendEmployeeInvite,
  useAdminResetEmployeePassword,
  useAdminUpdateEmployee,
} from "../hooks/use-admin-employee";
import type { AdminEmployeeDto } from "../types";
import { createEmployeeColumns } from "../components/employee-columns";
import { EmployeeDirectForm } from "../components/forms/employee-direct-form";
import { EmployeeFromAccountForm } from "../components/forms/employee-from-account-form";
import { EmployeeInviteForm } from "../components/forms/employee-invite-form";
import { EmployeeUpdateForm } from "../components/forms/employee-update-form";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { useAdminEmployeeParams } from "@/hooks/use-query-params";
import { stripNulls } from "@/lib/query-params";

const AdminEmployeesPage = () => {
  const [params, setParams] = useAdminEmployeeParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const employees = useAdminEmployees(stripNulls(params));
  const updateEmployee = useAdminUpdateEmployee();
  const resendInvite = useAdminResendEmployeeInvite();
  const resetPassword = useAdminResetEmployeePassword();
  const currentUserId = useAuthStore((s) => s.user?.userId);
  const bulkUpdateStatus = useAdminBulkUpdateUserStatus();
  const [rowSelection, setRowSelection] = useState<RowSelectionState>({});

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlertDialog = useAlertDialogOpen();

  const handleCreateFromAccount = () => {
    openDialog({
      title: "Thêm nhân viên từ tài khoản có sẵn",
      description: "Chọn một tài khoản đã có và nâng cấp thành nhân viên.",
      content: <EmployeeFromAccountForm onSuccess={closeDialog} />,
    });
  };

  const handleInvite = () => {
    openDialog({
      title: "Mời nhân viên qua email",
      description: "Nhân viên nhận email mời để tự đặt mật khẩu và kích hoạt tài khoản.",
      content: <EmployeeInviteForm onSuccess={closeDialog} />,
    });
  };

  const handleCreateDirect = () => {
    openDialog({
      title: "Tạo nhân viên trực tiếp",
      description: "Nhập thông tin và mật khẩu tạm để tạo tài khoản nhân viên ngay.",
      content: <EmployeeDirectForm onSuccess={closeDialog} />,
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

  const handleResendInvite = (employee: AdminEmployeeDto) => {
    openAlertDialog({
      title: `Gửi lại lời mời cho "${employee.fullName}"?`,
      description: `Một email đặt mật khẩu mới sẽ được gửi tới ${employee.email}. Liên kết cũ sẽ không còn hiệu lực.`,
      confirmText: "Gửi lại lời mời",
      onConfirm: async () => {
        await resendInvite.mutateAsync(employee.employeeId);
      },
    });
  };

  const handleResetPassword = (employee: AdminEmployeeDto) => {
    openAlertDialog({
      title: `Đặt lại mật khẩu cho "${employee.fullName}"?`,
      description:
        "Mật khẩu sẽ được đặt về mật khẩu mặc định trong Cấu hình rạp. Nhân viên bị đăng xuất khỏi mọi thiết bị và phải đổi mật khẩu ở lần đăng nhập kế tiếp.",
      confirmText: "Đặt lại mật khẩu",
      onConfirm: async () => {
        await resetPassword.mutateAsync(employee.employeeId);
        toast.add({
          title: `Đã đặt lại mật khẩu cho ${employee.fullName}`,
          description: "Hãy báo mật khẩu mặc định cho nhân viên để họ đăng nhập và đổi mật khẩu.",
          type: "success",
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
      onResendInvite: handleResendInvite,
      onResetPassword: handleResetPassword,
      currentUserId,
    }),
  ];

  return (
    <div>
      <PageHeader
        title="Nhân viên"
        description="Quản lý danh sách nhân viên, vị trí và trạng thái làm việc."
        action={
          <div className="flex flex-wrap gap-2">
            <Button variant="outline" onClick={handleCreateFromAccount}>
              <UserCheck className="size-4" />
              <span>Từ tài khoản có sẵn</span>
            </Button>
            <Button variant="outline" onClick={handleInvite}>
              <MailPlus className="size-4" />
              <span>Mời qua email</span>
            </Button>
            <Button onClick={handleCreateDirect}>
              <UserPlus className="size-4" />
              <span>Tạo trực tiếp</span>
            </Button>
          </div>
        }
      />

      <DataTable
        columns={columns}
        data={employees.data?.items ?? []}
        isLoading={employees.isLoading}
        isRefreshing={employees.isPlaceholderData}
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
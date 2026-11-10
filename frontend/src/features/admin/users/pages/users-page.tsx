import { useAuthStore } from "@/stores/auth-store";
import { useState } from "react";
import type { RowSelectionState } from "@tanstack/react-table";

import { PageHeader } from "@/components/common/page-header";
import { DataTable } from "@/components/common/data-table/data-table";
import { BulkUserStatusActions } from "@/components/common/data-table/bulk-user-status-actions";
import { createSelectionColumn } from "@/components/common/data-table/data-table-selection-column";
import { toastBulkResult } from "@/lib/bulk";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import {
  useAdminBulkUpdateUserStatus,
  useAdminUpdateUserStatus,
  useAdminUsers,
} from "../hooks/use-admin-users";
import type { AdminUserListItemDto } from "../types";
import { createUserColumns } from "../components/user-columns";
import { UserRoleForm } from "../components/forms/user-role-form";
import { useAdminUserParams } from "@/hooks/use-query-params";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { DEFAULT_PAGE } from "@/config/default-constants";
import { stripNulls } from "@/lib/query-params";
import { useDebouncedValue } from "@/hooks/use-debounced-value";

const AdminUsersPage = () => {
  const [params, setParams] = useAdminUserParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const users = useAdminUsers(stripNulls({ ...params, search: useDebouncedValue(params.search) }));
  const currentUserId = useAuthStore((s) => s.user?.userId);
  const toggleStatus = useAdminUpdateUserStatus();
  const bulkUpdateStatus = useAdminBulkUpdateUserStatus();
  const [rowSelection, setRowSelection] = useState<RowSelectionState>({});

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlertDialog = useAlertDialogOpen();

  const handleChangeRole = (user: AdminUserListItemDto) => {
    if (user.userId === currentUserId) return;
    openDialog({
      title: "Đổi vai trò người dùng",
      description: `Cập nhật vai trò cho ${user.fullName}.`,
      content: <UserRoleForm user={user} onSuccess={closeDialog} />,
    });
  };

  const handleToggle = (user: AdminUserListItemDto) => {
    const willActivate = !user.isActive;
    openAlertDialog({
      title: willActivate
        ? `Mở khóa tài khoản "${user.fullName}"?`
        : `Khóa tài khoản "${user.fullName}"?`,
      description: willActivate
        ? "Người dùng sẽ đăng nhập và sử dụng hệ thống bình thường."
        : "Người dùng sẽ không thể đăng nhập cho đến khi được mở khóa lại.",
      confirmText: willActivate ? "Mở khóa" : "Khóa",
      variant: willActivate ? "default" : "destructive",
      onConfirm: async () => {
        await toggleStatus.mutateAsync({
          id: user.userId,
          body: { isActive: willActivate },
        });
      },
    });
  };

  const handleBulkSetActive = async (selectedIds: string[], isActive: boolean) => {
    const res = await bulkUpdateStatus.mutateAsync({ ids: selectedIds.map(Number), isActive });
    setRowSelection({});
    toastBulkResult(res.data, isActive ? "mở khóa" : "khóa", "tài khoản");
  };

  const columns = [
    createSelectionColumn<AdminUserListItemDto>(),
    ...createUserColumns({
      onChangeRole: handleChangeRole,
      onToggle: handleToggle,
      currentUserId,
    }),
  ];

  return (
    <div>
      <PageHeader
        title="Người dùng"
        description="Quản lý tài khoản người dùng, vai trò và trạng thái hoạt động."
      />

      <DataTable
        columns={columns}
        data={users.data?.items ?? []}
        isLoading={users.isLoading}
        totalItems={users.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        searchValue={params.search}
        onSearchChange={(value) => setParams({ search: value, page: DEFAULT_PAGE })}
        searchPlaceholder="Tìm theo tên hoặc email..."
        emptyMessage="Chưa có người dùng nào."
        rowSelection={rowSelection}
        onRowSelectionChange={setRowSelection}
        getRowId={(row) => String(row.userId)}
        bulkActions={(selectedIds) => (
          <BulkUserStatusActions
            count={selectedIds.length}
            itemLabel="tài khoản"
            unlockLabel="Mở khóa"
            lockLabel="Khóa"
            unlockDescription="Các tài khoản được chọn sẽ đăng nhập và sử dụng hệ thống bình thường."
            lockDescription="Các tài khoản được chọn sẽ không thể đăng nhập cho đến khi được mở khóa lại. Tài khoản của bạn và quản trị viên cuối cùng sẽ không bị khóa."
            onSetActive={(isActive) => handleBulkSetActive(selectedIds, isActive)}
            isPending={bulkUpdateStatus.isPending}
          />
        )}
      />
    </div>
  );
};

export default AdminUsersPage;
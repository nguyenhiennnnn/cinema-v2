import { PageHeader } from "@/components/common/page-header";
import { DataTable } from "@/components/common/data-table/data-table";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import { useAdminUpdateUserStatus, useAdminUsers } from "../hooks/use-admin-users";
import type { AdminUserListItemDto } from "../types";
import { createUserColumns } from "../components/user-columns";
import { UserRoleForm } from "../components/forms/user-role-form";
import { useAdminUserParams } from "@/hooks/use-query-params";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { DEFAULT_PAGE } from "@/config/default-constants";
import { stripNulls } from "@/lib/query-params";

const AdminUsersPage = () => {
  const [params, setParams] = useAdminUserParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const users = useAdminUsers(stripNulls(params));
  const toggleStatus = useAdminUpdateUserStatus();

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlertDialog = useAlertDialogOpen();

  const handleChangeRole = (user: AdminUserListItemDto) => {
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

  const columns = createUserColumns({
    onChangeRole: handleChangeRole,
    onToggle: handleToggle,
  });

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
      />
    </div>
  );
};

export default AdminUsersPage;
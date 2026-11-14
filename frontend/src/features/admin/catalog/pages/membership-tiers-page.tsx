import type { ColumnFiltersState } from "@tanstack/react-table";
import { Plus } from "lucide-react";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { DataTable } from "@/components/common/data-table/data-table";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import {
  useAdminMembershipTiers,
  useAdminDeleteMembershipTier,
} from "../hooks/use-admin-membership-tier";
import type { MembershipTierDto } from "@/features/admin/catalog/types";
import { createMembershipTierColumns } from "../components/columns/membership-tier-columns";
import { MembershipTierCreateForm } from "../components/forms/membership-tiers/membership-tier-create-form";
import { MembershipTierUpdateForm } from "../components/forms/membership-tiers/membership-tier-update-form";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { useAdminListParams } from "@/hooks/use-query-params";
import { DEFAULT_PAGE, IRREVERSIBLE_ACTION_MESSAGE } from "@/config/default-constants";

const MembershipTiersPage = () => {
  const [params, setParams] = useAdminListParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const membershipTiers = useAdminMembershipTiers({
    page: params.page,
    pageSize: params.pageSize,
  });
  const deleteMembershipTier = useAdminDeleteMembershipTier();

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlertDialog = useAlertDialogOpen();

  const handleCreate = () => {
    openDialog({
      title: "Thêm hạng thành viên mới",
      description: "Nhập tên hạng, điểm tối thiểu và phần trăm giảm giá.",
      content: <MembershipTierCreateForm onSuccess={closeDialog} />,
    });
  };

  const handleEdit = (tier: MembershipTierDto) => {
    openDialog({
      title: "Cập nhật hạng thành viên",
      description: "Chỉnh sửa tên hạng, điểm tối thiểu và phần trăm giảm giá.",
      content: <MembershipTierUpdateForm tier={tier} onSuccess={closeDialog} />,
    });
  };

  const handleDelete = (tier: MembershipTierDto) => {
    openAlertDialog({
      title: `Xóa hạng "${tier.tierName}"?`,
      description: IRREVERSIBLE_ACTION_MESSAGE,
      confirmText: "Xóa",
      variant: "destructive",
      onConfirm: async () => {
        await deleteMembershipTier.mutateAsync(tier.tierId);
      },
    });
  };

  const columns = createMembershipTierColumns({
    onEdit: handleEdit,
    onDelete: handleDelete,
  });

  const columnFilters: ColumnFiltersState = params.search
    ? [{ id: "tierName", value: params.search }]
    : [];

  return (
    <div>
      <PageHeader
        title="Hạng thành viên"
        description="Quản lý các hạng thành viên, điểm tối thiểu và ưu đãi giảm giá."
        action={
          <Button onClick={handleCreate}>
            <Plus className="size-4" />
            <span>Thêm hạng</span>
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={membershipTiers.data?.items ?? []}
        isLoading={membershipTiers.isLoading}
        isRefreshing={membershipTiers.isPlaceholderData}
        totalItems={membershipTiers.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        columnFilters={columnFilters}
        searchValue={params.search}
        onSearchChange={(value) => setParams({ search: value, page: DEFAULT_PAGE })}
        searchPlaceholder="Tìm theo tên hạng..."
        emptyMessage="Chưa có hạng thành viên nào."
      />
    </div>
  );
};

export default MembershipTiersPage;
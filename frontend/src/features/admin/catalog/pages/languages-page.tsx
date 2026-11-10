import type { ColumnFiltersState } from "@tanstack/react-table";
import { Plus } from "lucide-react";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { DataTable } from "@/components/common/data-table/data-table";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import {
  useAdminDeleteLanguage,
  useAdminLanguages,
} from "../hooks/use-admin-language";
import type { LanguageDto } from "../types";
import { createLanguageColumns } from "../components/columns/language-columns";
import { LanguageCreateForm } from "../components/forms/languages/language-create-form";
import { LanguageUpdateForm } from "../components/forms/languages/language-update-form";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { useAdminListParams } from "@/hooks/use-query-params";
import { DEFAULT_PAGE } from "@/config/default-constants";

const LanguagesPage = () => {
  const [params, setParams] = useAdminListParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const languages = useAdminLanguages({
    page: params.page,
    pageSize: params.pageSize,
  });

  const deleteLanguage = useAdminDeleteLanguage();

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlertDialog = useAlertDialogOpen();

  const handleCreate = () => {
    openDialog({
      title: "Thêm ngôn ngữ mới",
      description: "Nhập tên ngôn ngữ hoặc hình thức dịch phim mới.",
      content: <LanguageCreateForm onSuccess={closeDialog} />,
    });
  };

  const handleEdit = (language: LanguageDto) => {
    openDialog({
      title: "Cập nhật ngôn ngữ",
      description: "Chỉnh sửa thông tin ngôn ngữ.",
      content: (
        <LanguageUpdateForm language={language} onSuccess={closeDialog} />
      ),
    });
  };

  const handleDelete = (language: LanguageDto) => {
    openAlertDialog({
      title: `Xóa ngôn ngữ "${language.name}"?`,
      description: "Hành động này không thể hoàn tác.",
      confirmText: "Xóa",
      variant: "destructive",
      onConfirm: async () => {
        await deleteLanguage.mutateAsync(language.languageId);
      },
    });
  };

  const columns = createLanguageColumns({
    onEdit: handleEdit,
    onDelete: handleDelete,
  });

  const columnFilters: ColumnFiltersState = params.search
    ? [{ id: "name", value: params.search }]
    : [];

  return (
    <div>
      <PageHeader
        title="Ngôn ngữ"
        description="Quản lý ngôn ngữ & hình thức dịch phim của hệ thống."
        action={
          <Button onClick={handleCreate}>
            <Plus className="size-4" />
            <span>Thêm ngôn ngữ</span>
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={languages.data?.items ?? []}
        isLoading={languages.isLoading}
        totalItems={languages.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        columnFilters={columnFilters}
        searchValue={params.search}
        onSearchChange={(value) => setParams({ search: value, page: DEFAULT_PAGE })}
        searchPlaceholder="Tìm theo tên ngôn ngữ..."
        emptyMessage="Chưa có ngôn ngữ nào."
      />
    </div>
  );
};

export default LanguagesPage;
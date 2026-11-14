import type {
  ColumnFiltersState,
} from "@tanstack/react-table";
import { Plus } from "lucide-react";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { DataTable } from "@/components/common/data-table/data-table";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import { useAdminDeleteGenre, useAdminGenres } from "../hooks/use-admin-genre";
import type { GenreDto } from "@/features/admin/catalog/types";
import { createGenreColumns } from "../components/columns/genre-columns";
import { GenreCreateForm } from "../components/forms/genres/genre-create-form";
import { GenreUpdateForm } from "../components/forms/genres/genre-update-form";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { useAdminListParams } from "@/hooks/use-query-params";
import { DEFAULT_PAGE, IRREVERSIBLE_ACTION_MESSAGE } from "@/config/default-constants";

const GenresPage = () => {
  const [params, setParams] = useAdminListParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const genres = useAdminGenres({
    page: params.page,
    pageSize: params.pageSize,
  });
  const deleteGenre = useAdminDeleteGenre();

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlertDialog = useAlertDialogOpen();

  const handleCreate = () => {
    openDialog({
      title: "Thêm thể loại mới",
      description: "Nhập tên thể loại phim mới cho hệ thống.",
      content: <GenreCreateForm onSuccess={closeDialog} />,
    });
  };

  const handleEdit = (genre: GenreDto) => {
    openDialog({
      title: "Cập nhật thể loại",
      description: "Chỉnh sửa thông tin thể loại phim.",
      content: <GenreUpdateForm genre={genre} onSuccess={closeDialog} />,
    });
  };

  const handleDelete = (genre: GenreDto) => {
    openAlertDialog({
      title: `Xóa thể loại "${genre.name}"?`,
      description: IRREVERSIBLE_ACTION_MESSAGE,
      confirmText: "Xóa",
      variant: "destructive",
      onConfirm: async () => {
        await deleteGenre.mutateAsync(genre.genreId);
      },
    });
  };

  const columns = createGenreColumns({
    onEdit: handleEdit,
    onDelete: handleDelete,
  });

  const columnFilters: ColumnFiltersState = params.search
    ? [{ id: "name", value: params.search }]
    : [];

  return (
    <div>
      <PageHeader
        title="Thể loại phim"
        description="Quản lý danh sách thể loại phim của hệ thống rạp chiếu."
        action={
          <Button onClick={handleCreate}>
            <Plus className="size-4" />
            <span>Thêm thể loại</span>
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={genres.data?.items ?? []}
        isLoading={genres.isLoading}
        isRefreshing={genres.isPlaceholderData}
        totalItems={genres.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        columnFilters={columnFilters}
        searchValue={params.search}
        onSearchChange={(value) => setParams({ search: value, page: DEFAULT_PAGE })}
        searchPlaceholder="Tìm theo tên thể loại..."
        emptyMessage="Chưa có thể loại nào."
      />
    </div>
  );
};

export default GenresPage;

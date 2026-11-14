import { useState } from "react";
import { Plus, Upload } from "lucide-react";
import type { RowSelectionState } from "@tanstack/react-table";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { toast } from "@/components/ui/toast";
import { DataTable } from "@/components/common/data-table/data-table";
import { BulkDeleteButton } from "@/components/common/data-table/bulk-delete-button";
import { EnumSelectFilter } from "@/components/common/filter/enum-select-filter";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";
import { useAdminMovieParams } from "@/hooks/use-query-params";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { stripNulls } from "@/lib/query-params";
import { movieStatusOptions } from "@/lib/enum-options";
import { DEFAULT_PAGE, IRREVERSIBLE_ACTION_MESSAGE } from "@/config/default-constants";

import {
  useAdminMovies,
  useAdminDeleteMovie,
  useAdminBulkDeleteMovies,
} from "../hooks/use-admin-movies";
import type { AdminMovieListItemDto } from "@/features/admin/movies/types";
import { createMovieColumns } from "../components/movie-columns";
import { createSelectionColumn } from "@/components/common/data-table/data-table-selection-column";
import { MovieCreateForm } from "../components/forms/movie-create-form";
import { MovieUpdateForm } from "../components/forms/movie-update-form";
import { useSheetClose, useSheetOpen } from "@/stores/sheet-store";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { ImportWizard } from "@/components/common/import-wizard/import-wizard";
import { movieImportConfig } from "../config/movie-import-config";
import { useAuthRole } from "@/stores/auth-store";
import { Role } from "@/types/enum";
import { useDebouncedValue } from "@/hooks/use-debounced-value";

const AdminMoviesPage = () => {
  const [params, setParams] = useAdminMovieParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);
  const role = useAuthRole();
  const canManage = role === Role.Admin;

  const [rowSelection, setRowSelection] = useState<RowSelectionState>({});

  const movies = useAdminMovies(stripNulls({ ...params, search: useDebouncedValue(params.search) }));
  const deleteMovie = useAdminDeleteMovie();
  const bulkDeleteMovies = useAdminBulkDeleteMovies();

  const openAlertDialog = useAlertDialogOpen();
  const openSheet = useSheetOpen();
  const closeSheet = useSheetClose();
  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();

  const handleCreate = () => {
    openSheet({
      title: "Thêm phim mới",
      description: "Nhập thông tin chi tiết của bộ phim.",
      content: <MovieCreateForm onSuccess={closeSheet} />,
    });
  };

  const handleImport = () => {
    openDialog({
      title: movieImportConfig.title,
      description: movieImportConfig.description,
      size: "xl",
      className: "sm:max-w-5xl",
      content: <ImportWizard config={movieImportConfig} onClose={closeDialog} />,
    });
  };

  const handleEdit = (movie: AdminMovieListItemDto) => {
    openSheet({
      title: "Cập nhật phim",
      description: `Chỉnh sửa thông tin phim "${movie.title}".`,
      content: <MovieUpdateForm movie={movie} onSuccess={closeSheet} />,
    });
  };

  const handleDelete = (movie: AdminMovieListItemDto) => {
    openAlertDialog({
      title: `Xóa phim "${movie.title}"?`,
      description: `${IRREVERSIBLE_ACTION_MESSAGE} Toàn bộ suất chiếu và đánh giá liên quan có thể bị ảnh hưởng.`,
      confirmText: "Xóa phim",
      variant: "destructive",
      onConfirm: async () => {
        await deleteMovie.mutateAsync(movie.movieId);
        setRowSelection((prev) =>
          Object.fromEntries(Object.entries(prev).filter(([key]) => key !== String(movie.movieId))),
        );
      },
    });
  };

  const handleBulkDelete = async (selectedIds: string[]) => {
    const ids = selectedIds.map(Number);
    const res = await bulkDeleteMovies.mutateAsync({ movieIds: ids });
    const result = res.data;
    setRowSelection({});
    if (result) {
      const { totalSuccess, totalFailed, results } = result;
      const failedReasons = results
        .filter((r) => !r.success && r.reason)
        .map((r) => `#${r.movieId}: ${r.reason}`)
        .join("; ");
      toast.add({
        title: `Đã xóa ${totalSuccess} phim${totalFailed > 0 ? `, ${totalFailed} thất bại` : ""}.`,
        description: failedReasons || undefined,
        type: totalFailed > 0 ? "warning" : "success",
      });
    }
  };

  const selectionCol = createSelectionColumn<AdminMovieListItemDto>();
  const columns = [
    selectionCol,
    ...createMovieColumns({
      onEdit: handleEdit,
      onDelete: handleDelete,
      canManage,
    }),
  ];

  return (
    <div>
      <PageHeader
        title="Quản lý Phim"
        description="Quản lý danh sách phim, trạng thái chiếu và thông tin chi tiết."
        action={
          canManage ? (
            <div className="flex gap-2">
              <Button variant="outline" onClick={handleImport}>
                <Upload className="size-4" />
                <span>Import</span>
              </Button>
              <Button onClick={handleCreate}>
                <Plus className="size-4" />
                <span>Thêm phim</span>
              </Button>
            </div>
          ) : undefined
        }
      />

      <div className="mb-4 flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-end">
        <EnumSelectFilter
          value={params.status}
          onChange={(status) => setParams({ status: status as typeof params.status, page: DEFAULT_PAGE })}
          options={movieStatusOptions}
          placeholder="Trạng thái phát hành"
          className="sm:w-52"
        />
      </div>

      <DataTable
        columns={columns}
        data={movies.data?.items ?? []}
        isLoading={movies.isLoading}
        isRefreshing={movies.isPlaceholderData}
        totalItems={movies.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        searchValue={params.search}
        onSearchChange={(value) => setParams({ search: value, page: DEFAULT_PAGE })}
        searchPlaceholder="Tìm theo tên phim..."
        emptyMessage="Chưa có bộ phim nào."
        rowSelection={rowSelection}
        onRowSelectionChange={setRowSelection}
        getRowId={(row) => String(row.movieId)}
        bulkActions={
          canManage
            ? (selectedIds) => (
                <BulkDeleteButton
                  count={selectedIds.length}
                  itemLabel="phim"
                  onConfirm={() => handleBulkDelete(selectedIds)}
                  isPending={bulkDeleteMovies.isPending}
                />
              )
            : undefined
        }
      />
    </div>
  );
};

export default AdminMoviesPage;

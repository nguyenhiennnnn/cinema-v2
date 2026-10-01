import { Plus } from "lucide-react";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { toast } from "@/components/ui/toast";
import { DataTable } from "@/components/common/data-table/data-table";
import { EnumSelectFilter } from "@/components/common/filter/enum-select-filter";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";
import { useAdminMovieParams } from "@/hooks/use-query-params";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { stripNulls } from "@/lib/query-params";
import { movieStatusOptions } from "@/lib/enum-options";
import { DEFAULT_PAGE } from "@/config/default-constants";

import { useAdminMovies, useAdminDeleteMovie } from "../hooks/use-admin-movies";
import type { AdminMovieListItemDto } from "../types";
import { createMovieColumns } from "../components/movie-columns";
import { MovieCreateForm } from "../components/forms/movie-create-form";
import { MovieUpdateForm } from "../components/forms/movie-update-form";
import { useSheetClose, useSheetOpen } from "@/stores/sheet-store";
import { useAuthRole } from "@/stores/auth-store";
import { Role } from "@/types/enum";

const AdminMoviesPage = () => {
  const [params, setParams] = useAdminMovieParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);
  const role = useAuthRole();
  const canManage = role === Role.Admin;

  const movies = useAdminMovies(stripNulls(params));
  const deleteMovie = useAdminDeleteMovie();

  const openAlertDialog = useAlertDialogOpen();
  const openSheet = useSheetOpen();
  const closeSheet = useSheetClose();

  const handleCreate = () => {
    openSheet({
      title: "Thêm phim mới",
      description: "Nhập thông tin chi tiết của bộ phim.",
      content: <MovieCreateForm onSuccess={closeSheet} />,
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
      description: "Hành động này không thể hoàn tác. Toàn bộ suất chiếu và đánh giá liên quan có thể bị ảnh hưởng.",
      confirmText: "Xóa phim",
      variant: "destructive",
      onConfirm: async () => {
        await deleteMovie.mutateAsync(movie.movieId);
        toast.add({ title: "Đã xóa phim thành công.", type: "success" });
      },
    });
  };

  const columns = createMovieColumns({
    onEdit: handleEdit,
    onDelete: handleDelete,
    canManage,
  });

  return (
    <div>
      <PageHeader
        title="Quản lý Phim"
        description="Quản lý danh sách phim, trạng thái chiếu và thông tin chi tiết."
        action={
          canManage ? (
            <Button onClick={handleCreate}>
              <Plus className="size-4" />
              <span>Thêm phim</span>
            </Button>
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
        totalItems={movies.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        searchValue={params.search}
        onSearchChange={(value) => setParams({ search: value, page: DEFAULT_PAGE })}
        searchPlaceholder="Tìm theo tên phim..."
        emptyMessage="Chưa có bộ phim nào."
      />
    </div>
  );
};

export default AdminMoviesPage;
import type { PaginationState } from "@tanstack/react-table";

interface PageParams {
  page: number;
  pageSize: number;
}

export function useTablePagination<T extends PageParams>(
  params: T,
  setParams: (updates: Partial<T>) => void,
) {
  const pagination: PaginationState = {
    pageIndex: params.page - 1,
    pageSize: params.pageSize,
  };

  const onPaginationChange = (
    updater: PaginationState | ((old: PaginationState) => PaginationState),
  ) => {
    const next = typeof updater === "function" ? updater(pagination) : updater;
    setParams({
      page: next.pageIndex + 1,
      pageSize: next.pageSize,
    } as Partial<T>);
  };

  return { pagination, onPaginationChange };
}

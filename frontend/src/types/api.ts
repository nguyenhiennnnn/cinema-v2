export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string | null;
}

export interface ApiErrorResponse {
  success: false;
  message: string;
  errors?: Record<string, string[]> | null;
  errorCode?: string | null;
}

export interface CursorPageResult<T> {
  items: T[];
  nextCursor: string | null;
  hasMore: boolean;
}

export interface OffsetPageResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

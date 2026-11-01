export interface BulkIdsRequest {
  ids: number[];
}

export interface BulkStatusRequest {
  ids: number[];
  isActive: boolean;
}

export interface BulkActionResultItem {
  id: number;
  success: boolean;
  reason?: string | null;
}

export interface BulkActionResponse {
  results: BulkActionResultItem[];
  totalSuccess: number;
  totalFailed: number;
}

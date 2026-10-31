// ── Admin review list ─────────────────────────────────────────────────────────
// Backend: GetReviews([FromQuery] int? movieId, [FromQuery] int? rating, page, pageSize)

export interface AdminReviewListParams {
  movieId?: number;
  rating?: number;
  page?: number;
  pageSize?: number;
}

export interface AdminReviewDto {
  reviewId: number;
  movieId: number;
  movieTitle: string;
  userId: number;
  userName: string;
  rating: number;
  comment?: string | null;
  createdAt: string;
}

export interface DeleteReviewResponse {
  reviewId: number;
  deleted: boolean;
}

// ── Bulk delete reviews ───────────────────────────────────────────────────────
export interface BulkDeleteReviewsRequest {
  reviewIds: number[];
}
export interface BulkDeleteReviewsResultItem {
  reviewId: number;
  success: boolean;
  reason?: string | null;
}
export interface BulkDeleteReviewsResponse {
  results: BulkDeleteReviewsResultItem[];
  totalSuccess: number;
  totalFailed: number;
}

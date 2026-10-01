import { apiGet, apiPost, apiPut, apiDelete } from "@/api/axios";
import type { ApiResponse, CursorPageResult } from "@/types/api";
import type {
  MovieListParams,
  MovieListItemDto,
  MovieDetailDto,
  ReviewListParams,
  ReviewDto,
  CreateReviewRequest,
  CanReviewResponse,
  DeleteReviewResponse,
} from "../types";

const BASE = "/movies";

export const moviesApi = {
  getMovies: ({ cursor, limit, ...filter }: MovieListParams) =>
    apiGet<ApiResponse<CursorPageResult<MovieListItemDto>>>(BASE, {
      params: { ...filter, cursor, limit },
    }),

  getMovieDetail: (movieId: number) =>
    apiGet<ApiResponse<MovieDetailDto>>(`${BASE}/${movieId}`),

  getMovieReviews: ({ movieId, cursor, limit }: ReviewListParams) =>
    apiGet<ApiResponse<CursorPageResult<ReviewDto>>>(`${BASE}/${movieId}/reviews`, {
      params: { cursor, limit },
    }),

  getCanReviewStatus: (movieId: number) =>
    apiGet<ApiResponse<CanReviewResponse>>(`${BASE}/${movieId}/can-review`),

  createReview: (movieId: number, body: CreateReviewRequest) =>
    apiPost<ApiResponse<ReviewDto>>(`${BASE}/${movieId}/reviews`, body),

  updateReview: (movieId: number, reviewId: number, body: CreateReviewRequest) =>
    apiPut<ApiResponse<ReviewDto>>(`${BASE}/${movieId}/reviews/${reviewId}`, body),

  deleteMyReview: (movieId: number, reviewId: number) =>
    apiDelete<ApiResponse<DeleteReviewResponse>>(`${BASE}/${movieId}/reviews/${reviewId}`),
};

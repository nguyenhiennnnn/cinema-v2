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
import { ENDPOINTS } from "@/api/endpoints";

export const moviesApi = {
  getMovies: ({ cursor, limit, ...filter }: MovieListParams) =>
    apiGet<ApiResponse<CursorPageResult<MovieListItemDto>>>(ENDPOINTS.movies.root, {
      params: { ...filter, cursor, limit },
    }),

  getMovieDetail: (movieId: number) =>
    apiGet<ApiResponse<MovieDetailDto>>(ENDPOINTS.movies.byId(movieId)),

  getMovieReviews: ({ movieId, cursor, limit }: ReviewListParams) =>
    apiGet<ApiResponse<CursorPageResult<ReviewDto>>>(ENDPOINTS.movies.reviews(movieId), {
      params: { cursor, limit },
    }),

  getCanReviewStatus: (movieId: number) =>
    apiGet<ApiResponse<CanReviewResponse>>(ENDPOINTS.movies.canReview(movieId)),

  createReview: (movieId: number, body: CreateReviewRequest) =>
    apiPost<ApiResponse<ReviewDto>>(ENDPOINTS.movies.reviews(movieId), body),

  updateReview: (movieId: number, reviewId: number, body: CreateReviewRequest) =>
    apiPut<ApiResponse<ReviewDto>>(ENDPOINTS.movies.reviewById(movieId, reviewId), body),

  deleteMyReview: (movieId: number, reviewId: number) =>
    apiDelete<ApiResponse<DeleteReviewResponse>>(ENDPOINTS.movies.reviewById(movieId, reviewId)),
};

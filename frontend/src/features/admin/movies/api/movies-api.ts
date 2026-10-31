import { apiDelete, apiGet, apiPost, apiPut } from "@/api/axios";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type {
  AdminMovieListParams,
  AdminMovieListItemDto,
  MovieDetailDto,
  UpsertMovieRequest,
  DeleteMovieResponse,
  BulkDeleteMoviesRequest,
  BulkDeleteMoviesResponse,
} from "../types";

const BASE = "/admin/movies";

export const moviesAdminApi = {
  getMovies: (params: AdminMovieListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminMovieListItemDto>>>(BASE, { params }),

  getMovieDetail: (movieId: number) =>
    apiGet<ApiResponse<MovieDetailDto>>(`/movies/${movieId}`),

  createMovie: (body: UpsertMovieRequest) =>
    apiPost<ApiResponse<AdminMovieListItemDto>>(BASE, body),

  updateMovie: (id: number, body: UpsertMovieRequest) =>
    apiPut<ApiResponse<AdminMovieListItemDto>>(`${BASE}/${id}`, body),

  deleteMovie: (id: number) =>
    apiDelete<ApiResponse<DeleteMovieResponse>>(`${BASE}/${id}`),

  bulkDeleteMovies: (body: BulkDeleteMoviesRequest) =>
    apiDelete<ApiResponse<BulkDeleteMoviesResponse>>(`${BASE}/bulk`, {
      data: body,
    }),
};

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
} from "@/features/admin/movies/types";
import { ENDPOINTS } from "@/api/endpoints";

export const moviesAdminApi = {
  getMovies: (params: AdminMovieListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminMovieListItemDto>>>(ENDPOINTS.admin.movies.root, { params }),

  getMovieDetail: (movieId: number) =>
    apiGet<ApiResponse<MovieDetailDto>>(ENDPOINTS.movies.byId(movieId)),

  createMovie: (body: UpsertMovieRequest) =>
    apiPost<ApiResponse<AdminMovieListItemDto>>(ENDPOINTS.admin.movies.root, body),

  updateMovie: (id: number, body: UpsertMovieRequest) =>
    apiPut<ApiResponse<AdminMovieListItemDto>>(ENDPOINTS.admin.movies.byId(id), body),

  deleteMovie: (id: number) =>
    apiDelete<ApiResponse<DeleteMovieResponse>>(ENDPOINTS.admin.movies.byId(id)),

  bulkDeleteMovies: (body: BulkDeleteMoviesRequest) =>
    apiDelete<ApiResponse<BulkDeleteMoviesResponse>>(ENDPOINTS.admin.movies.bulk, {
      data: body,
    }),
};

import { useMutation, useQuery } from "@tanstack/react-query";
import { queryKeys } from "@/api/query-keys";
import type {
  AdminMovieListParams,
  UpsertMovieRequest,
  BulkDeleteMoviesRequest,
} from "../types";
import { moviesAdminApi } from "../api/movies-api";

export function useAdminMovies(params: AdminMovieListParams) {
  return useQuery({
    queryKey: queryKeys.adminMovies.list(params),
    queryFn: () => moviesAdminApi.getMovies(params),
    select: (res) => res.data,
  });
}

export function useAdminMovieDetail(movieId?: number) {
  return useQuery({
    queryKey: queryKeys.movies.detail(movieId!),
    queryFn: () => moviesAdminApi.getMovieDetail(movieId!),
    enabled: typeof movieId === "number" && movieId > 0,
    select: (res) => res.data,
  });
}

export function useAdminCreateMovie() {
  return useMutation({
    meta: { invalidates: "movieChanged", errorTitle: "Không thể thêm phim" },
    mutationFn: (body: UpsertMovieRequest) => moviesAdminApi.createMovie(body),
  });
}

export function useAdminUpdateMovie() {
  return useMutation({
    meta: { invalidates: "movieChanged", errorTitle: "Không thể cập nhật phim" },
    mutationFn: ({ id, body }: { id: number; body: UpsertMovieRequest }) =>
      moviesAdminApi.updateMovie(id, body),
  });
}

export function useAdminDeleteMovie() {
  return useMutation({
    meta: { invalidates: "movieChanged" },
    mutationFn: (id: number) => moviesAdminApi.deleteMovie(id),
  });
}

export function useAdminBulkDeleteMovies() {
  return useMutation({
    meta: { invalidates: "movieChanged" },
    mutationFn: (body: BulkDeleteMoviesRequest) =>
      moviesAdminApi.bulkDeleteMovies(body),
  });
}

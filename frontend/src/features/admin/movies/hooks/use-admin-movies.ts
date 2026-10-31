import { useMutation, useQuery } from "@tanstack/react-query";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
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
    mutationFn: (body: UpsertMovieRequest) => moviesAdminApi.createMovie(body),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.adminMovies.all() });
    },
  });
}

export function useAdminUpdateMovie() {
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpsertMovieRequest }) =>
      moviesAdminApi.updateMovie(id, body),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.adminMovies.all() });
    },
  });
}

export function useAdminDeleteMovie() {
  return useMutation({
    mutationFn: (id: number) => moviesAdminApi.deleteMovie(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.adminMovies.all() });
    },
  });
}

export function useAdminBulkDeleteMovies() {
  return useMutation({
    mutationFn: (body: BulkDeleteMoviesRequest) =>
      moviesAdminApi.bulkDeleteMovies(body),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.adminMovies.all() });
    },
  });
}

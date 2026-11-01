import { useInfiniteQuery, useMutation, useQuery } from "@tanstack/react-query";
import { moviesApi } from "../api/movies-api";
import { queryKeys } from "@/api/query-keys";
import type {
  CreateReviewRequest,
  MovieListParams,
  ReviewListParams,
} from "@/features/admin/movies/types";

export function useMovies(params: Omit<MovieListParams, "cursor">) {
  return useInfiniteQuery({
    queryKey: queryKeys.movies.list(params),
    queryFn: ({ pageParam }) =>
      moviesApi.getMovies({ ...params, cursor: pageParam }),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (lastPage) =>
      lastPage.data.hasMore
        ? (lastPage.data.nextCursor ?? undefined)
        : undefined,
    select: (data) => ({
      pages: data.pages.flatMap((p) => p.data.items),
      hasMore: data.pages.at(-1)?.data.hasMore ?? false,
    }),
  });
}

export function useMovieDetail(movieId: number) {
  return useQuery({
    queryKey: queryKeys.movies.detail(movieId),
    queryFn: () => moviesApi.getMovieDetail(movieId),
    enabled: movieId > 0,
    select: (res) => res.data,
  });
}

export function useMovieReviews(params: Omit<ReviewListParams, "cursor">) {
  return useInfiniteQuery({
    queryKey: queryKeys.movies.reviews(params.movieId, { limit: params.limit }),
    queryFn: ({ pageParam }) =>
      moviesApi.getMovieReviews({ ...params, cursor: pageParam }),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (lastPage) =>
      lastPage.data.hasMore
        ? (lastPage.data.nextCursor ?? undefined)
        : undefined,
    select: (data) => ({
      reviews: data.pages.flatMap((p) => p.data.items),
      hasMore: data.pages.at(-1)?.data.hasMore ?? false,
    }),
  });
}

export function useCanReviewStatus(movieId: number, enabled = true) {
  return useQuery({
    queryKey: queryKeys.movies.canReview(movieId),
    queryFn: () => moviesApi.getCanReviewStatus(movieId),
    enabled: movieId > 0 && enabled,
    select: (res) => res.data,
  });
}

export function useCreateReview(movieId: number) {
  return useMutation({
    meta: { invalidates: "reviewChanged" },
    mutationFn: (body: CreateReviewRequest) =>
      moviesApi.createReview(movieId, body),
  });
}

export function useUpdateReview(movieId: number) {
  return useMutation({
    meta: { invalidates: "reviewChanged" },
    mutationFn: ({
      reviewId,
      body,
    }: {
      reviewId: number;
      body: CreateReviewRequest;
    }) => moviesApi.updateReview(movieId, reviewId, body),
  });
}

export function useDeleteMyReview(movieId: number) {
  return useMutation({
    meta: { invalidates: "reviewChanged" },
    mutationFn: (reviewId: number) =>
      moviesApi.deleteMyReview(movieId, reviewId),
  });
}

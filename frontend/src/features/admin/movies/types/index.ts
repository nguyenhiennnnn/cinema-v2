import type { MovieStatus } from "@/types/enum";

export interface AdminMovieListParams {
  search?: string;
  status?: MovieStatus;
  directorId?: number;
  languageId?: number;
  ageRatingId?: number;
  genreId?: number;
  page?: number;
  pageSize?: number;
}

export interface AdminMovieListItemDto {
  movieId: number;
  title: string;
  posterUrl?: string | null;
  status: MovieStatus;
  releaseDate?: string | null;
  directorName?: string | null;
  languageName?: string | null;
}

export interface MovieCastItemRequest {
  actorId: number;
  characterName?: string | null;
}

export interface UpsertMovieRequest {
  title: string;
  originalTitle?: string | null;
  description?: string | null;
  directorId?: number | null;
  durationMinutes: number;
  releaseDate?: string | null;
  languageId?: number | null;
  subtitle?: string | null;
  ageRatingId?: number | null;
  posterUrl?: string | null;
  trailerUrl?: string | null;
  status: string;
  genreIds: number[];
  cast: MovieCastItemRequest[];
}

export interface DeleteMovieResponse {
  movieId: number;
  deleted: boolean;
  softDeleted: boolean;
}

export type {
  MovieListParams,
  MovieListItemDto,
  MovieDetailDto,
  ReviewListParams,
  ReviewDto,
  CreateReviewRequest,
  CanReviewResponse,
  DeleteReviewResponse,
} from "@/features/client/movies/types";

export interface BulkDeleteMoviesRequest {
  movieIds: number[];
}
export interface BulkDeleteMoviesResultItem {
  movieId: number;
  success: boolean;
  reason?: string | null;
}
export interface BulkDeleteMoviesResponse {
  results: BulkDeleteMoviesResultItem[];
  totalSuccess: number;
  totalFailed: number;
}

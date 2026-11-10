import type { MovieStatus } from "@/types/enum";

export interface MovieListParams {
  status?: MovieStatus;
  genreId?: number;
  languageId?: number;
  ageRatingId?: number;
  search?: string;
  cursor?: string;
  limit?: number;
}

export interface AgeRatingCodeDto {
  code: string;
}

export interface MovieListItemDto {
  movieId: number;
  title: string;
  posterUrl?: string | null;
  durationMinutes: number;
  status: MovieStatus;
  averageRating: number;
  ageRating?: AgeRatingCodeDto | null;
  genres: string[];
  releaseDate?: string | null;
}

export interface DirectorDto {
  directorId: number;
  fullName: string;
  avatarUrl?: string | null;
}

export interface LanguageDto {
  languageId: number;
  name: string;
}

export interface AgeRatingDto {
  ageRatingId: number;
  code: string;
  description?: string | null;
}

export interface GenreDto {
  genreId: number;
  name: string;
}

export interface CastMemberDto {
  actorId: number;
  fullName: string;
  avatarUrl?: string | null;
  characterName?: string | null;
}

export interface UpcomingShowtimeDto {
  showtimeId: number;
  roomId: number;
  roomName: string;
  screenType?: string | null;
  startTime: string;
  endTime: string;
  basePrice: number;
  availableSeats: number;
  totalSeats: number;
}

export interface MovieDetailDto {
  movieId: number;
  title: string;
  originalTitle?: string | null;
  description?: string | null;
  durationMinutes: number;
  releaseDate?: string | null;
  subtitle?: string | null;
  posterUrl?: string | null;
  trailerUrl?: string | null;
  status: MovieStatus;
  averageRating: number;
  director?: DirectorDto | null;
  language?: LanguageDto | null;
  ageRating?: AgeRatingDto | null;
  genres: GenreDto[];
  cast: CastMemberDto[];
  upcomingShowtimes: UpcomingShowtimeDto[];
}

export interface ReviewListParams {
  movieId: number;
  cursor?: string;
  limit?: number;
}

export interface ReviewDto {
  reviewId: number;
  userId: number;
  userName: string;
  userAvatar?: string | null;
  rating: number;
  comment?: string | null;
  createdAt: string;
}

export interface CreateReviewRequest {
  rating: number;
  comment?: string | null;
}

export interface CanReviewResponse {
  canReview: boolean;
}

export interface DeleteReviewResponse {
  reviewId: number;
  deleted: boolean;
}

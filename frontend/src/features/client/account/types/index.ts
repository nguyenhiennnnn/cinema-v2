// ── User Profile ──────────────────────────────────────────────────────────────

export interface MembershipTierBriefDto {
  tierId: number;
  tierName: string;
  discountPercent: number;
}

export interface NextTierDto {
  tierId: number;
  tierName: string;
  pointsNeeded: number;
}

export interface UserProfileDto {
  userId: number;
  fullName: string;
  email: string;
  phone?: string | null;
  avatar?: string | null;
  dateOfBirth?: string | null;
  gender?: string | null;
  points: number;
  totalSpent: number;
  tier?: MembershipTierBriefDto | null;
  nextTier?: NextTierDto | null;
}

// ── Update Profile ────────────────────────────────────────────────────────────

export interface UpdateProfileRequest {
  fullName: string;
  phone?: string | null;
  avatar?: string | null;
  dateOfBirth?: string | null;
  gender?: string | null;
}

// ── My Reviews ────────────────────────────────────────────────────────────────

export interface UserMyReviewDto {
  reviewId: number;
  movieId: number;
  movieTitle: string;
  moviePosterUrl?: string | null;
  rating: number;
  comment?: string | null;
  createdAt: string;
}

export interface MyReviewListParams {
  cursor?: string;
  limit?: number;
}

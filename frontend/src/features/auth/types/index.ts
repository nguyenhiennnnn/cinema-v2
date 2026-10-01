import type { Role } from "@/types/enum";

// ── Register ──────────────────────────────────────────────────────────────────

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  phone?: string | null;
}

export interface RegisterResponse {
  userId: number;
  email: string;
  message: string;
}

// ── Confirm Email ─────────────────────────────────────────────────────────────

export interface ConfirmEmailRequest {
  token: string;
}

// ── Login ─────────────────────────────────────────────────────────────────────

export interface LoginRequest {
  email: string;
  password: string;
}

// ── Google OAuth Login ────────────────────────────────────────────────────────
// Backend nhan ID token (JWT) do Google Identity Services (FE) tra ve

export interface GoogleLoginRequest {
  idToken: string;
}

export interface LoginUserDto {
  userId: number;
  fullName: string;
  email: string;
  role: Role;
  avatar?: string | null;
  membershipTierName?: string | null;
  points?: number | null;
  isEmailConfirmed: boolean;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: LoginUserDto;
}

// ── Refresh ───────────────────────────────────────────────────────────────────

export interface RefreshRequest {
  refreshToken: string;
}

// Backend RefreshResponse also includes the updated user
export interface RefreshResponse {
  accessToken: string;
  refreshToken: string;
  user: LoginUserDto;
  expiresIn: number;
}

// ── Logout ────────────────────────────────────────────────────────────────────

export interface LogoutRequest {
  refreshToken: string;
}

// ── Forgot / Reset Password ───────────────────────────────────────────────────

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  token: string;
  newPassword: string;
}

// ── Resend Confirmation ───────────────────────────────────────────────────────

export interface ResendConfirmationEmailRequest {
  email: string;
}

// ── Change Password ───────────────────────────────────────────────────────────

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

// ── Socket Token ──────────────────────────────────────────────────────────────

export interface SocketTokenResponse {
  token: string;
  expiresIn: number;
}

// ── Me ────────────────────────────────────────────────────────────────────────

export interface MeResponse {
  userId: number;
  fullName: string;
  email: string;
  phone?: string | null;
  avatar?: string | null;
  role: Role;
}

// ── Shared ────────────────────────────────────────────────────────────────────

export interface MessageResponse {
  message: string;
}

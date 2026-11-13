import type { Role } from "@/types/enum";

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

export interface ConfirmEmailRequest {
  token: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

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
  expiresIn: number;
  user: LoginUserDto;
}

export interface RefreshResponse {
  accessToken: string;
  user: LoginUserDto;
  expiresIn: number;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  token: string;
  newPassword: string;
}

export interface ResendConfirmationEmailRequest {
  email: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export interface SocketTokenResponse {
  token: string;
  expiresIn: number;
}

export interface MeResponse {
  userId: number;
  fullName: string;
  email: string;
  phone?: string | null;
  avatar?: string | null;
  role: Role;
}

export interface MessageResponse {
  message: string;
}

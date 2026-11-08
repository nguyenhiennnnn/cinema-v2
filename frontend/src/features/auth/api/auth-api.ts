import { apiGet, apiPost } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type {
  RegisterRequest,
  RegisterResponse,
  ConfirmEmailRequest,
  LoginRequest,
  LoginResponse,
  GoogleLoginRequest,
  RefreshRequest,
  RefreshResponse,
  LogoutRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  ResendConfirmationEmailRequest,
  SocketTokenResponse,
  MeResponse,
  MessageResponse,
} from "../types";

const BASE = "/auth";

export const authApi = {
  register: (body: RegisterRequest) =>
    apiPost<ApiResponse<RegisterResponse>>(`${BASE}/register`, body),

  confirmEmail: (body: ConfirmEmailRequest) =>
    apiPost<ApiResponse<MessageResponse>>(`${BASE}/confirm-email`, body),

  login: (body: LoginRequest) =>
    apiPost<ApiResponse<LoginResponse>>(`${BASE}/login`, body),

  adminLogin: (body: LoginRequest) =>
    apiPost<ApiResponse<LoginResponse>>(`${BASE}/admin-login`, body),

  googleLogin: (body: GoogleLoginRequest) =>
    apiPost<ApiResponse<LoginResponse>>(`${BASE}/google`, body),

  refresh: (body: RefreshRequest) =>
    apiPost<ApiResponse<RefreshResponse>>(`${BASE}/refresh`, body),

  logout: (body: LogoutRequest) =>
    apiPost<ApiResponse<MessageResponse>>(`${BASE}/logout`, body),

  forgotPassword: (body: ForgotPasswordRequest) =>
    apiPost<ApiResponse<MessageResponse>>(`${BASE}/forgot-password`, body),

  resetPassword: (body: ResetPasswordRequest) =>
    apiPost<ApiResponse<MessageResponse>>(`${BASE}/reset-password`, body),

  resendConfirmation: (body: ResendConfirmationEmailRequest) =>
    apiPost<ApiResponse<MessageResponse>>(`${BASE}/resend-confirmation`, body),

  getSocketToken: () =>
    apiPost<ApiResponse<SocketTokenResponse>>(`${BASE}/socket-token`),

  me: () => apiGet<ApiResponse<MeResponse>>(`${BASE}/me`),
};

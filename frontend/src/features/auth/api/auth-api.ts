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
import { ENDPOINTS } from "@/api/endpoints";

export const authApi = {
  register: (body: RegisterRequest) =>
    apiPost<ApiResponse<RegisterResponse>>(ENDPOINTS.auth.register, body),

  confirmEmail: (body: ConfirmEmailRequest) =>
    apiPost<ApiResponse<MessageResponse>>(ENDPOINTS.auth.confirmEmail, body),

  login: (body: LoginRequest) =>
    apiPost<ApiResponse<LoginResponse>>(ENDPOINTS.auth.login, body),

  adminLogin: (body: LoginRequest) =>
    apiPost<ApiResponse<LoginResponse>>(ENDPOINTS.auth.adminLogin, body),

  googleLogin: (body: GoogleLoginRequest) =>
    apiPost<ApiResponse<LoginResponse>>(ENDPOINTS.auth.google, body),

  refresh: (body: RefreshRequest) =>
    apiPost<ApiResponse<RefreshResponse>>(ENDPOINTS.auth.refresh, body),

  logout: (body: LogoutRequest) =>
    apiPost<ApiResponse<MessageResponse>>(ENDPOINTS.auth.logout, body),

  forgotPassword: (body: ForgotPasswordRequest) =>
    apiPost<ApiResponse<MessageResponse>>(ENDPOINTS.auth.forgotPassword, body),

  resetPassword: (body: ResetPasswordRequest) =>
    apiPost<ApiResponse<MessageResponse>>(ENDPOINTS.auth.resetPassword, body),

  resendConfirmation: (body: ResendConfirmationEmailRequest) =>
    apiPost<ApiResponse<MessageResponse>>(ENDPOINTS.auth.resendConfirmation, body),

  getSocketToken: () =>
    apiPost<ApiResponse<SocketTokenResponse>>(ENDPOINTS.auth.socketToken),

  me: () => apiGet<ApiResponse<MeResponse>>(ENDPOINTS.auth.me),
};

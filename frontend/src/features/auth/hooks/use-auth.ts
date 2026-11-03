import { useMutation } from "@tanstack/react-query";
import { authApi } from "../api/auth-api";
import { queryClient } from "@/api/query-client";
import { useAuthStore, refreshTokenStorage, type AuthUser } from "@/stores/auth-store";
import type {
  LoginRequest,
  GoogleLoginRequest,
  RegisterRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  ConfirmEmailRequest,
  ResendConfirmationEmailRequest,
  LoginUserDto,
  MeResponse,
} from "../types";

// ─── Helper: map DTO sang AuthUser ───────────────────────────────────────────

function mapLoginUserToAuthUser(dto: LoginUserDto): AuthUser {
  return {
    userId: dto.userId,
    email: dto.email,
    fullName: dto.fullName,
    role: dto.role,
    avatar: dto.avatar ?? null,
    membershipTierName: dto.membershipTierName ?? null,
    points: dto.points ?? undefined,
    isEmailConfirmed: dto.isEmailConfirmed,
  };
}

function applyMeToAuthUser(me: MeResponse): Partial<AuthUser> {
  return {
    fullName: me.fullName,
    email: me.email,
    avatar: me.avatar ?? null,
    role: me.role,
    isEmailConfirmed: true,
  };
}

async function hydrateUserFromMe() {
  try {
    const res = await authApi.me();
    useAuthStore.getState().updateUser(applyMeToAuthUser(res.data));
  } catch {
    //
  }
}

// ─── Hooks ───────────────────────────────────────────────────────────────────

export function useLogin() {
  return useMutation({
    mutationFn: (body: LoginRequest) => authApi.login(body),
    onSuccess: async (res) => {
      const { accessToken, refreshToken, user } = res.data;
      refreshTokenStorage.set(refreshToken);
      useAuthStore.getState().setCredentials(accessToken, mapLoginUserToAuthUser(user));
      await hydrateUserFromMe();
    },
  });
}

export function useAdminLogin() {
  return useMutation({
    mutationFn: (body: LoginRequest) => authApi.adminLogin(body),
    onSuccess: async (res) => {
      const { accessToken, refreshToken, user } = res.data;
      refreshTokenStorage.set(refreshToken);
      useAuthStore.getState().setCredentials(accessToken, mapLoginUserToAuthUser(user));
      await hydrateUserFromMe();
    },
  });
}

export function useGoogleLogin() {
  return useMutation({
    mutationFn: (body: GoogleLoginRequest) => authApi.googleLogin(body),
    onSuccess: async (res) => {
      const { accessToken, refreshToken, user } = res.data;
      refreshTokenStorage.set(refreshToken);
      useAuthStore.getState().setCredentials(accessToken, mapLoginUserToAuthUser(user));
      await hydrateUserFromMe();
    },
  });
}

export function useRegister() {
  return useMutation({
    mutationFn: (body: RegisterRequest) => authApi.register(body),
  });
}

export function useLogout() {
  return useMutation({
    meta: { skipSuccessToast: true },
    mutationFn: () => {
      const rt = refreshTokenStorage.get() ?? "";
      return authApi.logout({ refreshToken: rt });
    },
    onSettled: () => {
      useAuthStore.getState().logout();
      queryClient.clear();
    },
  });
}

export function useForgotPassword() {
  return useMutation({
    mutationFn: (body: ForgotPasswordRequest) => authApi.forgotPassword(body),
  });
}

export function useResetPassword() {
  return useMutation({
    mutationFn: (body: ResetPasswordRequest) => authApi.resetPassword(body),
  });
}

export function useConfirmEmail() {
  return useMutation({
    mutationFn: (body: ConfirmEmailRequest) => authApi.confirmEmail(body),
  });
}

export function useResendConfirmation() {
  return useMutation({
    mutationFn: (body: ResendConfirmationEmailRequest) =>
      authApi.resendConfirmation(body),
  });
}


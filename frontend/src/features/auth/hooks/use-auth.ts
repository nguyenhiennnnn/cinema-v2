import { useMutation } from "@tanstack/react-query";
import { authApi } from "../api/auth-api";
import { queryClient } from "@/api/query-client";
import { useAuthStore, type AuthUser } from "@/stores/auth-store";
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
    mustChangePassword: dto.mustChangePassword ?? false,
  };
}

function applyMeToAuthUser(me: MeResponse): Partial<AuthUser> {
  return {
    fullName: me.fullName,
    email: me.email,
    avatar: me.avatar ?? null,
    role: me.role,
    isEmailConfirmed: true,
    mustChangePassword: me.mustChangePassword ?? false,
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

export function useLogin() {
  return useMutation({
    meta: { skipErrorToast: true },
    mutationFn: (body: LoginRequest) => authApi.login(body),
    onSuccess: async (res) => {
      const { accessToken, user } = res.data;
      useAuthStore.getState().setCredentials(accessToken, mapLoginUserToAuthUser(user));
      await hydrateUserFromMe();
    },
  });
}

export function useAdminLogin() {
  return useMutation({
    meta: { skipErrorToast: true },
    mutationFn: (body: LoginRequest) => authApi.adminLogin(body),
    onSuccess: async (res) => {
      const { accessToken, user } = res.data;
      useAuthStore.getState().setCredentials(accessToken, mapLoginUserToAuthUser(user));
      await hydrateUserFromMe();
    },
  });
}

export function useGoogleLogin() {
  return useMutation({
    meta: { skipErrorToast: true },
    mutationFn: (body: GoogleLoginRequest) => authApi.googleLogin(body),
    onSuccess: async (res) => {
      const { accessToken, user } = res.data;
      useAuthStore.getState().setCredentials(accessToken, mapLoginUserToAuthUser(user));
      await hydrateUserFromMe();
    },
  });
}

export function useRegister() {
  return useMutation({
    meta: { skipErrorToast: true },
    mutationFn: (body: RegisterRequest) => authApi.register(body),
  });
}

export function useLogout() {
  return useMutation({
    meta: { skipSuccessToast: true },
    mutationFn: () => authApi.logout(),
    onSettled: () => {
      useAuthStore.getState().logout();
      queryClient.clear();
    },
  });
}

export function useForgotPassword() {
  return useMutation({
    meta: { skipErrorToast: true },
    mutationFn: (body: ForgotPasswordRequest) => authApi.forgotPassword(body),
  });
}

export function useResetPassword(options?: { skipSuccessToast?: boolean }) {
  return useMutation({
    meta: { skipErrorToast: true, skipSuccessToast: options?.skipSuccessToast },
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


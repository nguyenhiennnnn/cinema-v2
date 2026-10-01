import { create } from "zustand";
import type { Role } from "@/types/enum";
import { queryClient } from "@/api/query-client";

const REFRESH_TOKEN_KEY = "cinema_refresh_token";

export interface AuthUser {
  userId: number;
  email: string;
  fullName: string;
  role: Role;
  avatar?: string | null;
  membershipTierName?: string | null;
  points?: number;
  isEmailConfirmed?: boolean;
}

interface AuthStore {
  accessToken: string | null;
  user: AuthUser | null;
  isInitialized: boolean;
  isAuthenticated: boolean;

  setCredentials: (accessToken: string, user: AuthUser) => void;
  setAccessToken: (token: string) => void;
  updateUser: (partial: Partial<AuthUser>) => void;
  logout: () => void;
  setInitialized: () => void;
}

export const refreshTokenStorage = {
  get: (): string | null => localStorage.getItem(REFRESH_TOKEN_KEY),
  set: (token: string) => localStorage.setItem(REFRESH_TOKEN_KEY, token),
  remove: () => localStorage.removeItem(REFRESH_TOKEN_KEY),
};

export const useAuthStore = create<AuthStore>()((set) => ({
  accessToken: null,
  user: null,
  isInitialized: false,
  isAuthenticated: false,

  setCredentials: (accessToken, user) =>
    set({ accessToken, user, isAuthenticated: true }),

  setAccessToken: (token) => set({ accessToken: token }),

  updateUser: (partial) =>
    set((state) => ({
      user: state.user ? { ...state.user, ...partial } : null,
    })),

  logout: () => {
    refreshTokenStorage.remove();
    localStorage.removeItem("cinema_auth");
    queryClient.clear();
    set({ accessToken: null, user: null, isAuthenticated: false });
  },

  setInitialized: () => set({ isInitialized: true }),
}));

// Granular selectors
export const useAuthUser = () => useAuthStore((s) => s.user);
export const useIsAuthenticated = () => useAuthStore((s) => s.isAuthenticated);
export const useIsAuthInitialized = () => useAuthStore((s) => s.isInitialized);
export const useAuthRole = () => useAuthStore((s) => s.user?.role ?? null);

export const useAuthActions = () =>
  useAuthStore((s) => ({
    setCredentials: s.setCredentials,
    setAccessToken: s.setAccessToken,
    updateUser: s.updateUser,
    logout: s.logout,
    setInitialized: s.setInitialized,
  }));

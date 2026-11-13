import { create } from "zustand";
import type { Role } from "@/types/enum";
import { queryClient } from "@/api/query-client";
import { sessionHint } from "@/lib/session-hint";

sessionHint.purgeLegacy();

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

export const useAuthStore = create<AuthStore>()((set) => ({
  accessToken: null,
  user: null,
  isInitialized: false,
  isAuthenticated: false,

  setCredentials: (accessToken, user) => {
    sessionHint.set();
    set({ accessToken, user, isAuthenticated: true });
  },

  setAccessToken: (token) => set({ accessToken: token }),

  updateUser: (partial) =>
    set((state) => ({
      user: state.user ? { ...state.user, ...partial } : null,
    })),

  logout: () => {
    sessionHint.clear();
    queryClient.clear();
    set({ accessToken: null, user: null, isAuthenticated: false });
  },

  setInitialized: () => set({ isInitialized: true }),
}));

export const useAuthUser = () => useAuthStore((s) => s.user);
export const useIsAuthenticated = () => useAuthStore((s) => s.isAuthenticated);
export const useAuthRole = () => useAuthStore((s) => s.user?.role ?? null);

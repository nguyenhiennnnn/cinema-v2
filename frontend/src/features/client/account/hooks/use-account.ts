import { useInfiniteQuery, useMutation, useQuery } from "@tanstack/react-query";
import { accountApi } from "../api/account-api";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
import { useAuthStore } from "@/stores/auth-store";
import { toast } from "@/components/ui/toast";
import type { MyReviewListParams, UpdateProfileRequest } from "../types";
import type { ChangePasswordRequest } from "@/features/auth/types";

export function useMyProfile() {
  return useQuery({
    queryKey: queryKeys.users.me(),
    queryFn: () => accountApi.getMyProfile(),
    select: (res) => res.data,
  });
}

export function useUpdateMyProfile() {
  return useMutation({
    mutationFn: (body: UpdateProfileRequest) =>
      accountApi.updateMyProfile(body),
    onSuccess: (res) => {
      const { setCredentials } = useAuthStore.getState();
      const { accessToken } = useAuthStore.getState();
      const user = res.data;
      if (accessToken) {
        setCredentials(accessToken, {
          userId: user.userId,
          email: user.email,
          fullName: user.fullName,
          role: "Customer",
          avatar: user.avatar,
        });
      }
      queryClient.invalidateQueries({ queryKey: queryKeys.users.me() });
      toast.add({ title: "Cập nhật thông tin thành công.", type: "success" });
    },
  });
}

export function useMyReviews(params: Omit<MyReviewListParams, "cursor"> = {}) {
  return useInfiniteQuery({
    queryKey: queryKeys.users.myReviews(params),
    queryFn: ({ pageParam }) =>
      accountApi.getMyReviews({ ...params, cursor: pageParam }),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (lastPage) =>
      lastPage.data.hasMore
        ? (lastPage.data.nextCursor ?? undefined)
        : undefined,
    select: (data) => ({
      reviews: data.pages.flatMap((p) => p.data.items),
      hasMore: data.pages.at(-1)?.data.hasMore ?? false,
    }),
  });
}

export function useChangePassword() {
  return useMutation({
    mutationFn: (body: ChangePasswordRequest) =>
      accountApi.changePassword(body),
  });
}

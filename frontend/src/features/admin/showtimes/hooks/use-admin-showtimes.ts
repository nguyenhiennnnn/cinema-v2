import { useMutation, useQuery } from "@tanstack/react-query";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
import type {
  AdminShowtimeListParams,
  AdminShowtimeCalendarParams,
  UpsertShowtimeRequest,
  BulkCreateShowtimesRequest,
  BulkDeleteShowtimesRequest,
} from "../types";
import { showtimesAdminApi } from "../api/showtimes-api";

export function useAdminShowtimes(params: AdminShowtimeListParams) {
  return useQuery({
    queryKey: queryKeys.adminShowtimes.list(params),
    queryFn: () => showtimesAdminApi.getShowtimes(params),
    select: (res) => res.data,
  });
}

export function useAdminShowtimesCalendar(params: AdminShowtimeCalendarParams) {
  return useQuery({
    queryKey: queryKeys.adminShowtimes.calendar(params),
    queryFn: () => showtimesAdminApi.getCalendar(params),
    select: (res) => res.data,
    enabled: Boolean(params.from && params.to),
  });
}

export function useAdminCreateShowtime() {
  return useMutation({
    mutationFn: (body: UpsertShowtimeRequest) =>
      showtimesAdminApi.createShowtime(body),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminShowtimes.all(),
      });
    },
  });
}

export function useAdminUpdateShowtime() {
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpsertShowtimeRequest }) =>
      showtimesAdminApi.updateShowtime(id, body),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminShowtimes.all(),
      });
    },
  });
}

export function useAdminDeleteShowtime() {
  return useMutation({
    mutationFn: (id: number) => showtimesAdminApi.deleteShowtime(id),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminShowtimes.all(),
      });
    },
  });
}

export function useAdminBulkCreateShowtimes() {
  return useMutation({
    mutationFn: (body: BulkCreateShowtimesRequest) =>
      showtimesAdminApi.bulkCreate(body),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminShowtimes.all(),
      });
    },
  });
}

export function useAdminBulkDeleteShowtimes() {
  return useMutation({
    mutationFn: (body: BulkDeleteShowtimesRequest) =>
      showtimesAdminApi.bulkDelete(body),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminShowtimes.all(),
      });
    },
  });
}
